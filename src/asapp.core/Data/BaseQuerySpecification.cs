using System.Collections.Concurrent;
using System.Linq.Expressions;
using System.Reflection;

namespace Softfluent.Asapp.Core.Data
{
    /// <summary>
    /// https://docs.microsoft.com/en-us/dotnet/architecture/microservices/microservice-ddd-cqrs-patterns/infrastructure-persistence-layer-implementation-entity-framework-core#implement-the-query-specification-pattern
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class BaseQuerySpecification<T> : IQuerySpecification<T> where T : class
    {
        private static ConcurrentDictionary<Type, IEnumerable<Expression<Func<T, object>>>> _sortForPagingCache;

        static BaseQuerySpecification()
        {
            _sortForPagingCache = new ConcurrentDictionary<Type, IEnumerable<Expression<Func<T, object>>>>();
        }

        public BaseQuerySpecification()
        {
        }

        public BaseQuerySpecification(Expression<Func<T, bool>> criteria)
            : this()
        {
            this.Criteria = criteria;
        }

        public Expression<Func<T, bool>>? Criteria { get; private set; }

        public Expression<Func<T, object>>? GroupBy { get; private set; }

        public List<Expression<Func<T, object>>> Includes { get; } = new List<Expression<Func<T, object>>>();

        public List<string> IncludeStrings { get; } = new List<string>();

        public bool IsPagingEnabled { get; private set; } = false;

        public List<Tuple<SortDirection, Expression<Func<T, object>>>> OrderBy { get; private set; } = new List<Tuple<SortDirection, Expression<Func<T, object>>>>();

        public int Skip { get; private set; }

        public Expression? SortExpression { get; private set; }

        public int Take { get; private set; }

        public virtual void AddInclude(Expression<Func<T, object>> includeExpression)
        {
            this.Includes.Add(includeExpression);
        }

        public virtual void AddInclude(string includeString)
        {
            this.IncludeStrings.Add(includeString);
        }

        public virtual void ApplyCriteria(GridCriteria gridCriteria)
        {
            IQueryBuilder queryBuilder = new QueryBuilder();
            this.Criteria = queryBuilder.GetCriteria<T>(gridCriteria.Filters);
            this.ApplyOrdering(gridCriteria.Sorts, gridCriteria.PageSize > 0);
            if (gridCriteria.PageSize > 0)
            {
                this.ApplyPaging(gridCriteria.Page ?? 0, gridCriteria.PageSize ?? 0);
            }
        }

        public virtual void ApplyCriteriaWithoutPaging(GridCriteria gridCriteria)
        {
            IQueryBuilder queryBuilder = new QueryBuilder();
            this.Criteria = queryBuilder.GetCriteria<T>(gridCriteria.Filters);
            this.ApplyOrdering(gridCriteria.Sorts, gridCriteria.PageSize > 0);
        }


        public virtual void ApplyCriteria(IEnumerable<(string name, List<object> values)> filters, string gloablSearch, string sortedColumn, bool sortDescending, int currentpage, int pagesize)
        {
            this.ApplyFilter(gloablSearch, filters);
            this.ApplySorting(sortedColumn, sortDescending, pagesize > 0);
            if (pagesize > 0)
            {
                this.ApplyPaging(currentpage, pagesize);
            }
        }

        public virtual void ApplyFilter(GridCriteria gridCriteria)
        {
            IQueryBuilder queryBuilder = new QueryBuilder();
            this.Criteria = queryBuilder.GetCriteria<T>(gridCriteria.Filters);
        }

        public void ApplyFilter(string globalSearch, IEnumerable<(string name, List<object> values)> keyValues)
        {
            IQueryBuilder queryBuilder = new QueryBuilder();
            IEnumerable<FilterCriteria> filtersCriteria = queryBuilder.GetFilters<T>(globalSearch, keyValues);
            this.Criteria = queryBuilder.GetCriteria<T>(filtersCriteria);
        }

        public virtual void ApplyGroupBy(Expression<Func<T, object>> groupByExpression)
        {
            this.GroupBy = groupByExpression;
        }

        public virtual void ApplyOrderBy(Expression<Func<T, object>> orderByExpression)
        {
            this.OrderBy.Add(Tuple.Create(SortDirection.Ascending, orderByExpression));
        }

        public virtual void ApplyOrderByDescending(Expression<Func<T, object>> orderByDescendingExpression)
        {
            this.OrderBy.Add(Tuple.Create(SortDirection.Descending, orderByDescendingExpression));
        }

        public virtual void ApplyPaging(int pageIndex, int pageSize)
        {
            this.Skip = pageIndex * pageSize;
            this.Take = pageSize;
            this.IsPagingEnabled = true;
        }

        public void ApplySorting(string sortedColumn, bool desc, bool hasPaging)
        {
            IQueryBuilder queryBuilder = new QueryBuilder();
            IEnumerable<SortCriteria> sortsCriteria = queryBuilder.GetSorts<T>(sortedColumn, desc);

            this.ApplyOrdering(sortsCriteria, hasPaging);
        }

        private void ApplyOrdering(IEnumerable<SortCriteria> sortsCriteria, bool hasPaging)
        {
            ParameterExpression lambdaParamX = Expression.Parameter(typeof(T), "x");

            foreach (SortCriteria sc in sortsCriteria)
            {
                PropertyInfo property = typeof(T).GetRuntimeProperty(sc.FieldName) ?? throw new InvalidOperationException($"Because the property {sc.FieldName} does not exist it cannot be sorted.");

                // Create an Expression<Func<T, object>>.
                LambdaExpression propertyReturningExpression = Expression.Lambda(Expression.Convert(Expression.Property(lambdaParamX, property), typeof(object)), lambdaParamX);

                this.OrderBy.Add(Tuple.Create(sc.Direction, (Expression<Func<T, object>>)propertyReturningExpression));
            }

            if (hasPaging)
            {
                if (_sortForPagingCache.TryGetValue(typeof(T), out IEnumerable<Expression<Func<T, object>>>? sortToAdd) == false)
                {
                    List<Tuple<int, Expression<Func<T, object>>>> pagingOrderBy = new List<Tuple<int, Expression<Func<T, object>>>>();

                    // Retreive all decorated properties
                    foreach (PropertyInfo pi in typeof(T).GetProperties())
                    {
                        PagingOrderByAttribute? orderByAttribute = pi.GetCustomAttribute<PagingOrderByAttribute>(true);
                        if (orderByAttribute != null)
                        {
                            LambdaExpression propertyReturningExpression = Expression.Lambda(Expression.Convert(Expression.Property(lambdaParamX, pi), typeof(object)), lambdaParamX);

                            pagingOrderBy.Add(new Tuple<int, Expression<Func<T, object>>>(orderByAttribute.Order, (Expression<Func<T, object>>)propertyReturningExpression));
                        }
                    }
                    sortToAdd = pagingOrderBy.OrderBy(x => x.Item1).Select(x => x.Item2);
                    _sortForPagingCache.TryAdd(typeof(T), sortToAdd);
                }

                foreach (Expression<Func<T, object>> orderByStatement in sortToAdd)
                {
                    this.OrderBy.Add(Tuple.Create(SortDirection.Ascending, orderByStatement));
                }
            }
        }
    }
}
