using System.Linq.Expressions;

namespace Softfluent.Asapp.Core.Data
{
    public interface IQuerySpecification<T>
    {
        Expression<Func<T, bool>> Criteria { get; }

        Expression<Func<T, object>> GroupBy { get; }

        List<Expression<Func<T, object>>> Includes { get; }

        List<string> IncludeStrings { get; }

        bool IsPagingEnabled { get; }

        List<Tuple<SortDirection, Expression<Func<T, object>>>> OrderBy { get; }

        int Skip { get; }

        int Take { get; }

        void AddInclude(Expression<Func<T, object>> includeExpression);

        void AddInclude(string includeString);

        void ApplyCriteria(GridCriteria gridCriteria);

        void ApplyCriteriaWithoutPaging(GridCriteria gridCriteria);

        void ApplyFilter(GridCriteria gridCriteria);

        void ApplyGroupBy(Expression<Func<T, object>> groupByExpression);
    }
}
