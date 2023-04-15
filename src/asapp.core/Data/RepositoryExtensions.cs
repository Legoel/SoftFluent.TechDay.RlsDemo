using Microsoft.Extensions.DependencyInjection;
using Softfluent.Asapp.Core.Context;

namespace Softfluent.Asapp.Core.Data
{
    public static class RepositoryExtensions
    {
        public static IServiceCollection AddBaseRepository(this IServiceCollection services)
        {
            return services.AddExecutionContext()
                    .AddScoped(typeof(IBaseRepository<>), typeof(BaseRepository<>));
        }
    }
}
