using Microsoft.AspNetCore.Mvc.Filters;
using Softfluent.Asapp.Core.Context;

namespace RlsDemo.Web
{
	public class UserContexteActionFilter : IAsyncActionFilter
	{
		private readonly CallContext _callContext;

		public UserContexteActionFilter(CallContext callContext)
		{
			_callContext = callContext;
		}

		public Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
		{
			string? tenant = context.HttpContext.User.FindFirst("TenantId")?.Value;
			if (string.IsNullOrEmpty(tenant) || !int.TryParse(tenant, out int tenantId))
				throw new UnauthorizedAccessException("KO");

			_callContext.TenantId = tenantId;
			_callContext.ExecutionIdentity = context.HttpContext.User.Identity?.Name ?? "Unknown";

			return next();
		}
	}
}
