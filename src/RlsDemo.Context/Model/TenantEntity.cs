using Softfluent.Asapp.Core.Data;

namespace RlsDemo.Context.Model
{
	public class TenantEntity : Entity<int>, ITenantEntity
	{
		public int TenantId { get ; set; }
	}
}
