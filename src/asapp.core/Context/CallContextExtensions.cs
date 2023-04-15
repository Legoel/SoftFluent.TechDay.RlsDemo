using Microsoft.Extensions.DependencyInjection;

namespace Softfluent.Asapp.Core.Context
{
    public static class CallContextExtensions
    {
        public static IServiceCollection AddExecutionContext(this IServiceCollection services)
        {
            return services.AddScoped<CallContext>();
        }
    }
}
