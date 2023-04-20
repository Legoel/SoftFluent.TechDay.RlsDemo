namespace RlsDemo.Context.Model
{
	public class SensitiveDatumDto
	{
		public int Identifier { get; set; }
		public SensitiveDatumTypeDto Type { get; set; }
		public string Name { get; set; } = null!;
		public string? Content { get; set; }
    }
}
