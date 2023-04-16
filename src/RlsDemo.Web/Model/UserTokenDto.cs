namespace RlsDemo.Context.Model
{
	public class UserTokenDto
	{
		public string Token { get; set; } = null!;
		public string Login { get; set; } = null!;
		public int TenantId { get; set; }
		public DateTime ExpiresOn { get; set; }
		public IEnumerable<string> Roles { get; set; } = new List<string>();
	}
}
