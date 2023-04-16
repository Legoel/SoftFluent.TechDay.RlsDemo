using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using RlsDemo.Context.Model;
using RslDemo.Context;
using Softfluent.Asapp.Core.Data;

namespace RlsDemo.Web.Controllers
{
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
			return Ok(_mapper.Map<IEnumerable<SensitiveDatum>, IEnumerable<SensitiveDatumDto>>(_repository.GetEnumerable(querySpecification)));
		}

		[HttpGet("{id}")]
		public ActionResult<Task<SensitiveDatumDto>> Get([FromRoute] int id)
		{
			var result = _repository.GetAsync<SensitiveDatum>(sd => sd.Identifier == id)
				.ContinueWith(task =>
				{
				}));
			if (result is null)
				return NotFound();

			return Ok(_mapper.Map<SensitiveDatumDto>(result));
		}

		[HttpPost]
		public ActionResult<SensitiveDatumDto> Post([FromBody] SensitiveDatumDto datum)
		{
			var model = _mapper.Map<SensitiveDatum>(datum);
			_repository.Create(model);
			return Ok(_mapper.Map<SensitiveDatumDto>(model));
		}

		[HttpDelete("{id}")]
		public ActionResult<bool> Delete([FromRoute] int id)
		{
			var result = _repository.DeleteAsync<SensitiveDatum>(sd => sd.Identifier == id);
			if (result is null)
				return NotFound();

			return Ok(_mapper.Map<SensitiveDatumDto>(result));
		}
	}
}