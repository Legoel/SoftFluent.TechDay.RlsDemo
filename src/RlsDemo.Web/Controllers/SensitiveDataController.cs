using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using RlsDemo.Context.Model;
using RslDemo.Context;
using Softfluent.Asapp.Core.Data;

namespace RlsDemo.Web.Controllers
{
	[ApiController]
	[Route("[controller]")]
	public class SensitiveDataController : ControllerBase
	{
		private readonly RlsDemoContext _context;
		private readonly IBaseRepository<RlsDemoContext> _repository;
		private readonly IMapper _mapper;
		private readonly ILogger<SensitiveDataController> _logger;

		public SensitiveDataController(RlsDemoContext context, IBaseRepository<RlsDemoContext> repository, IMapper mapper, ILogger<SensitiveDataController> logger)
		{
			_context = context;
			_repository = repository;
			_mapper = mapper;
			_logger = logger;
		}

		private void EnsureContextTenant(out int tenantId)
		{
			tenantId = 0;
			var tenant = User.FindAll("Tenant").FirstOrDefault()?.Value;
			if (string.IsNullOrEmpty(tenant) || !int.TryParse(tenant, out tenantId))
				throw new UnauthorizedAccessException();

			_context.ContextTenantId = tenantId;
		}

		[HttpGet]
		public ActionResult<IEnumerable<SensitiveDatumDto>> GetAll()
		{
			EnsureContextTenant(out _);

			var querySpecification = new BaseQuerySpecification<SensitiveDatum>();
			querySpecification.AddInclude(sd => sd.Tenant);
			querySpecification.ApplyOrderBy(sd => sd.Name);
			return Ok(_mapper.Map<IEnumerable<SensitiveDatum>, IEnumerable<SensitiveDatumDto>>(_repository.GetEnumerable(querySpecification)));
		}

		[HttpGet("type/{type}")]
		public ActionResult<IEnumerable<SensitiveDatumDto>> GetbyType([FromRoute] SensitiveDatumTypeDto type)
		{
			EnsureContextTenant(out _);

			var entityType = _mapper.Map<SensitiveDatumType>(type);
			var querySpecification = new BaseQuerySpecification<SensitiveDatum>();
			querySpecification.AddInclude(sd => sd.Tenant);
			querySpecification.ApplyOrderBy(sd => sd.Name);
			return Ok(_mapper.Map<IEnumerable<SensitiveDatum>, IEnumerable<SensitiveDatumDto>>(_repository.GetEnumerable(querySpecification, sd => sd.Type == entityType)));
		}

		[HttpGet("{id}")]
		public ActionResult<SensitiveDatumDto> Get([FromRoute] int id)
		{
			EnsureContextTenant(out _);

			var result = _repository.Get<SensitiveDatum>(sd => sd.Identifier == id);
			if (result is null)
				return NotFound();

			return Ok(_mapper.Map<SensitiveDatumDto>(result));
		}

		[HttpPost]
		[Authorize(Roles = "Administrator")]
		public ActionResult<SensitiveDatumDto> Post([FromBody] SensitiveDatumDto datum)
		{
			EnsureContextTenant(out int tenantId);

			var entity = _mapper.Map<SensitiveDatum>(datum);
			if (entity.TenantId != tenantId)
				return Unauthorized();

			_repository.Create(entity);
			return Ok(_mapper.Map<SensitiveDatumDto>(entity));
		}

		[HttpPut("{id}")]
		[Authorize(Roles = "Administrator")]
		public ActionResult<SensitiveDatumDto> Put([FromRoute] int id, [FromBody] SensitiveDatumDto datum)
		{
			EnsureContextTenant(out int tenantId);

			var entity = _mapper.Map<SensitiveDatum>(datum);

			if (entity.TenantId != tenantId)
				return Unauthorized();

			if (entity.Identifier != id)
				return BadRequest();

			var result = _repository.Update(entity);
			if (result == 0)
				return NotFound();

			return Ok(_mapper.Map<SensitiveDatumDto>(entity));
		}

		[HttpDelete("{id}")]
		[Authorize(Roles = "Administrator")]
		public async Task<ActionResult<bool>> Delete([FromRoute] int id)
		{
			EnsureContextTenant(out int tenantId);

			var result = await _repository.DeleteAsync<SensitiveDatum>(sd => sd.Identifier == id);
			if (result == 0)
				return NotFound();

			return NoContent();
		}
	}
}