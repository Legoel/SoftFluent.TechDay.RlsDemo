namespace Softfluent.Asapp.Core.Data
{
    public class GridPage<TEntity> where TEntity : class
    {
        public IList<TEntity>? Items { get; set; }

        public int Page { get; set; }

        public int PageSize { get; set; }

        public int TotalCount { get; set; }
    }
}
