namespace RlsDemo.Context.Model
{
	public class SensitiveDatumDto
	{
		public SensitiveDatumTypeDto Type { get; set; }
		public string Name { get; set; } = null!;
		public string? Content { get; set; }

		public int TenantId { get; set; }
	}
}
