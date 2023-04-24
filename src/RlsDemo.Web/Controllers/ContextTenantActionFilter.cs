using Microsoft.AspNetCore.Mvc.Filters;
using Softfluent.Asapp.Core.Context;

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
}