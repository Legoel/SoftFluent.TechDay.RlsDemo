using Softfluent.Asapp.Core.Data;

namespace RlsDemo.Context.Model
{
	public interface ITenantEntity : IEntity<int>
	{
		int TenantId { get; set; }
	}

	public class TenantEntity : Entity<int>, ITenantEntity
	{
		public int TenantId { get; set; }
	}
}
