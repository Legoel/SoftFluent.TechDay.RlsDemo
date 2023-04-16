using Softfluent.Asapp.Core.Data;

namespace RlsDemo.Context.Model
{
    public class Tenant : Entity<int>
    {
        public string Name { get; set; } = null!;
    }
}
