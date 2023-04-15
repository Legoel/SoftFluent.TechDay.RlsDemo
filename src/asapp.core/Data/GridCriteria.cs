namespace Softfluent.Asapp.Core.Data
{
    public class GridCriteria
    {
        public GridCriteria()
        {
            this.Filters = new List<FilterCriteria>();
            this.Sorts = new List<SortCriteria>();
        }

        public int? Page { get; set; }

        public List<FilterCriteria> Filters { get; set; }

        public int? PageSize { get; set; }

        public List<SortCriteria> Sorts { get; set; }
    }
}
