namespace Softfluent.Asapp.Core.Data
{
    public class FilterCriteriaAttribute : Attribute
    {
        public FilterCriteriaAttribute(string name, FilterPredicates predicate, FilterOperator opertor = FilterOperator.Unknown)
        {
            this.Name = name;
            this.Predicate = predicate;
            this.Operator = opertor;
        }

        public string Name { get; set; }

        public FilterOperator Operator { get; set; }

        public FilterPredicates Predicate { get; set; }
    }
}
