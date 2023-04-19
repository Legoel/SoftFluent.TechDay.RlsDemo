using Softfluent.Asapp.Core.Data;

namespace RlsDemo.Context.Model
{
	public interface ITenantEntity : IEntity<int>
	{
		int TenantId { get; set; }
	}
}
