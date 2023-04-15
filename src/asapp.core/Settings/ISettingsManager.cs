namespace Softfluent.Asapp.Core.Settings
{
    public interface ISettingsManager : IDictionary<string, string?>
    {
    }

    public interface ISettingsManager<TConfigSection> : ISettingsManager where TConfigSection : SettingSectionBase
    {
        TConfigSection DefaultSection { get; }
    }
}
