namespace RlsDemo.Context.Model
{
	public class SensitiveDatum : TenantEntity
	{
		public SensitiveDatumType Type { get; set; }
		public string Name { get; set; } = null!;
		public string? Content { get; set; }

		public Tenant Tenant { get; set; } = null!;
	}
}
