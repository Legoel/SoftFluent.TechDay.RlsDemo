using Softfluent.Asapp.Core.Data;

namespace Softfluent.Asapp.Core.Settings
{
    public class Setting : Entity<long>
    {
        public string Key { get; set; } = String.Empty;

        public string Targets { get; set; } = String.Empty;

        public string? Value { get; set; }
    }
}
