namespace Softfluent.Asapp.Core.Data
{
    public class FilterCriteria
    {
        public string FieldName { get; set; }

        public FilterOperator? Operator { get; set; }

        public FilterPredicates? Predicate { get; set; }

        public object? Value { get; set; }

        public List<object>? Values { get; set; }
    }
}
