using System.Linq.Expressions;

namespace Softfluent.Asapp.Core.Data
{
    public interface IQueryBuilder
    {
        Expression<Func<T, bool>> GetCriteria<T>(IEnumerable<FilterCriteria> filterConditions) where T : class;

        IEnumerable<FilterCriteria> GetFilters<T>(string globalSearch, IEnumerable<(string name, List<object> values)> filters);

        IEnumerable<SortCriteria> GetSorts<T>(string sortColumn, bool desc);
    }
}
