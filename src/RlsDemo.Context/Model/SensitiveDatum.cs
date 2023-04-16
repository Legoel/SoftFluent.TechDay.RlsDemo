using Softfluent.Asapp.Core.Data;

namespace RlsDemo.Context.Model
{
    public class SensitiveDatum : Entity<int>
    {
        public SensitiveDatumType Type { get; set; }
        public string Name { get; set; } = null!;
        public string? Content { get; set; }

        public int TenantId { get; set; }
        public Tenant Tenant { get; set; } = null!;
    }
}
