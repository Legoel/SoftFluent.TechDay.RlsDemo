namespace Softfluent.Asapp.Core.Data
{
    public class GlobalSearchAttribute : Attribute
    {
        public GlobalSearchAttribute(string name, FilterPredicates predicate = FilterPredicates.Contains)
        {
            this.Name = name;
            this.Predicate = predicate;
        }

        public string Name { get; set; }

        public FilterPredicates Predicate { get; set; }
    }
}
