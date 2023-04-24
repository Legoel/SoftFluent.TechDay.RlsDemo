using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using RlsDemo.Context.Model;
using RslDemo.Context;
using Softfluent.Asapp.Core.Context;
using Softfluent.Asapp.Core.Data;

namespace RlsDemo.Web.Controllers
{
	public class ContextTenantActionFilter : IAsyncActionFilter
	{
		private readonly CallContext _context;

		public ContextTenantActionFilter(CallContext context)
        {
			_context = context;
		}

        public Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
		{
			var tenant = context.HttpContext.User.FindAll("TenantId").FirstOrDefault()?.Value;
			if (tenant is null || !int.TryParse(tenant, out int tenantId))
				throw new UnauthorizedAccessException();

			_context.TenantId = tenantId;
			_context.ExecutionIdentity = context.HttpContext.User.Identity?.Name ?? "Anonymous";

			return next();
		}
	}

	[TypeFilter(typeof(ContextTenantActionFilter))]
	[ApiController]
	[Route("[controller]")]
	public class SensitiveDataController : ControllerBase
	{
		private readonly IBaseRepository<RlsDemoContext> _repository;
		private readonly IMapper _mapper;
		private readonly ILogger<SensitiveDataController> _logger;

		public SensitiveDataController(IBaseRepository<RlsDemoContext> repository, IMapper mapper, ILogger<SensitiveDataController> logger)
		{
			_repository = repository;
			_mapper = mapper;
			_logger = logger;
		}

		[HttpGet]
		public ActionResult<IEnumerable<SensitiveDatumDto>> GetAll()
		{
			var querySpecification = new BaseQuerySpecification<SensitiveDatum>();
			querySpecification.AddInclude(sd => sd.Tenant);
			querySpecification.ApplyOrderBy(sd => sd.Name);
			return Ok(_mapper.Map<IEnumerable<SensitiveDatum>,
				IEnumerable<SensitiveDatumDto>>(_repository.GetEnumerable(querySpecification)));
		}

		[HttpGet("type/{type}")]
		public ActionResult<IEnumerable<SensitiveDatumDto>> GetbyType([FromRoute] SensitiveDatumTypeDto type)
		{
			var entityType = _mapper.Map<SensitiveDatumType>(type);
			var querySpecification = new BaseQuerySpecification<SensitiveDatum>();
			querySpecification.AddInclude(sd => sd.Tenant);
			querySpecification.ApplyOrderBy(sd => sd.Name);
			return Ok(_mapper.Map<IEnumerable<SensitiveDatum>, IEnumerable<SensitiveDatumDto>>(_repository.GetEnumerable(querySpecification, sd => sd.Type == entityType)));
		}

		[HttpGet("{id}")]
		public ActionResult<SensitiveDatumDto> Get([FromRoute] int id)
		{
			var result = _repository.Get<SensitiveDatum>(sd => sd.Identifier == id);
			if (result is null)
				return NotFound();

			return Ok(_mapper.Map<SensitiveDatumDto>(result));
		}

		[HttpPost]
		[Authorize(Roles = "Administrator")]
		public ActionResult<SensitiveDatumDto> Post([FromBody] SensitiveDatumDto datum)
		{
			var entity = _mapper.Map<SensitiveDatum>(datum);
			if (entity.TenantId != (int)(HttpContext.Items["TenantId"]))
				return Unauthorized();

			_repository.Create(entity);
			return Ok(_mapper.Map<SensitiveDatumDto>(entity));
		}

		[HttpPut("{id}")]
		[Authorize(Roles = "Administrator")]
		public ActionResult<SensitiveDatumDto> Put([FromRoute] int id, [FromBody] SensitiveDatumDto datum)
		{
			var entity = _mapper.Map<SensitiveDatum>(datum);

			var result = _repository.Update(entity);
			if (result == 0)
				return NotFound();

			return Ok(_mapper.Map<SensitiveDatumDto>(entity));
		}

		[HttpDelete("{id}")]
		[Authorize(Roles = "Administrator")]
		public async Task<ActionResult<bool>> Delete([FromRoute] int id)
		{
			var result = await _repository.DeleteAsync<SensitiveDatum>(sd => sd.Identifier == id);
			if (result == 0)
				return NotFound();

			return NoContent();
		}
	}
}