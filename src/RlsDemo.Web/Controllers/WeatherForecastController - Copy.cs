using Microsoft.AspNetCore.Mvc;
using RslDemo.Context;
using Softfluent.Asapp.Core.Data;

namespace RlsDemo.Web.Controllers
{
	[ApiController]
	[Route("[controller]")]
	public class SensitiveDataController : ControllerBase
	{
		private readonly IBaseRepository<RlsDemoContext> _repository;
		private readonly ILogger<SensitiveDataController> _logger;

		public SensitiveDataController(IBaseRepository<RlsDemoContext> repository, ILogger<SensitiveDataController> logger)
		{
			_repository = repository;
			_logger = logger;
		}

		[HttpGet]
		public IEnumerable<SensitiveDatum> Get()
		{
			var querySpecification = new BaseQuerySpecification<SensitiveDatum>();
			querySpecification.ApplyOrderBy(sd => sd.Name);
			return _repository.GetEnumerable<SensitiveDatum>(querySpecification);
		}
	}
}