using Microsoft.AspNetCore.Mvc.Filters;
using RslDemo.Context;

namespace RlsDemo.Web
{
	public class UserContexteActionFilter : IAsyncActionFilter
	{
		private readonly RlsDemoContext _context;

		public UserContexteActionFilter(RlsDemoContext context)
		{
			_context = context;
		}

		public Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
		{
			string? tenant = context.HttpContext.User.FindFirst("TenantId")?.Value;
			if (string.IsNullOrEmpty(tenant) || !int.TryParse(tenant, out int tenantId))
				throw new UnauthorizedAccessException("KO");

			_context.CurrentTenantId = tenantId;

			return next();
		}
	}
}
