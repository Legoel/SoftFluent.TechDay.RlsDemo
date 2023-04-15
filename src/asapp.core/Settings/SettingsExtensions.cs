using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Softfluent.Asapp.Core.Settings
{
    public static class SettingsExtensions
    {
        public static IServiceCollection AddCustomSettingsManager<TConfigSection, TSettingManager>(this IServiceCollection services)
                                                        where TConfigSection : SettingSectionBase, new()
                                                        where TSettingManager : class, ISettingsManager<TConfigSection>
        {
            return services.AddSingleton<TSettingManager>()
                    .AddSingleton<ISettingsManager<TConfigSection>>(x => x.GetRequiredService<TSettingManager>())
                    .AddSingleton<ISettingsManager>(x => x.GetRequiredService<TSettingManager>());
        }

        public static IServiceCollection AddSettingsManager<TConfigSection, TDbContext>(this IServiceCollection services)
                                                                where TConfigSection : SettingSectionBase, new()
                                                                where TDbContext : DbContext
        {
            return services.AddSingleton<ISettingsManager<TConfigSection>, SettingsManager<TConfigSection, TDbContext>>()
                    .AddSingleton<ISettingsManager>(x => x.GetRequiredService<ISettingsManager<TConfigSection>>());
        }

        public static IServiceCollection AddSettingsManager<TSettingManager>(this IServiceCollection services)
                                                        where TSettingManager : class, ISettingsManager
        {
            return services.AddSingleton<TSettingManager>()
                            .AddSingleton<ISettingsManager>(x => x.GetRequiredService<TSettingManager>());
        }
    }
}
