using System.Linq.Expressions;

namespace Softfluent.Asapp.Core.Data
{
    public static class QueryableExtensions
    {
        public static IQueryable<T> SelectMembers<T>(this IQueryable<T> source, params string[] memberNames)
        {
            ParameterExpression parameter = Expression.Parameter(typeof(T), "e");
            IEnumerable<MemberAssignment> bindings = memberNames
                .Select(name => Expression.PropertyOrField(parameter, name))
                .Select(member => Expression.Bind(member.Member, member));
            MemberInitExpression body = Expression.MemberInit(Expression.New(typeof(T)), bindings);
            Expression<Func<T, T>> selector = Expression.Lambda<Func<T, T>>(body, parameter);
            return source.Select(selector);
        }
    }
}
