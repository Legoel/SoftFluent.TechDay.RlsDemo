using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq.Expressions;
using System.Reflection;

namespace Softfluent.Asapp.Core.Data
{
    public class QueryBuilder : IQueryBuilder
    {
        public Expression<Func<T, bool>> GetCriteria<T>(IEnumerable<FilterCriteria> filterConditions) where T : class
        {
            if (!filterConditions.Any())
            {
                return null;
            }

            ParameterExpression param = Expression.Parameter(typeof(T), "param");
            Expression filterExpression = null;

            foreach (FilterCriteria filterCondition in filterConditions)
            {
                if (filterExpression == null)
                {
                    filterExpression = this.ApplyPredicate(param, filterCondition);
                }
                else
                {
                    if (filterCondition.Operator == FilterOperator.And)
                    {
                        filterExpression = Expression.AndAlso(filterExpression, this.ApplyPredicate(param, filterCondition));
                    }
                    else
                    {
                        filterExpression = Expression.OrElse(filterExpression, this.ApplyPredicate(param, filterCondition));
                    }
                }
            }

            return Expression.Lambda<Func<T, bool>>(filterExpression, param);
        }

        public IEnumerable<FilterCriteria> GetFilters<T>(string globalSearch, IEnumerable<(string name, List<object> values)> filters)
        {
            filters ??= new List<(string name, List<object> values)>();
            PropertyInfo[] properties = typeof(T).GetCustomAttribute<MetadataTypeAttribute>(false)?.MetadataClassType.GetProperties() ?? Array.Empty<PropertyInfo>();
            IEnumerable<FilterCriteria> filterCriterias = (
                                   from pi in properties
                                   let searchAtt = pi.GetCustomAttribute<GlobalSearchAttribute>()
                                   where !string.IsNullOrEmpty(globalSearch) && searchAtt != null
                                   select new FilterCriteria()
                                   {
                                       FieldName = searchAtt.Name,
                                       Predicate = searchAtt.Predicate,
                                       Value = globalSearch
                                   }
                                  ).Union
                                  (from pi in properties
                                   let filterAtt = pi.GetCustomAttribute<FilterCriteriaAttribute>()
                                   where filterAtt != null
                                   join filter in filters on filterAtt.Name equals filter.name into result
                                   from x in result
                                   select new FilterCriteria()
                                   {
                                       FieldName = filterAtt.Name,
                                       Predicate = filterAtt.Predicate,
                                       Operator = filterAtt.Operator,
                                       Value = filterAtt.Predicate == FilterPredicates.In ? null : x.values?.FirstOrDefault(),
                                       Values = filterAtt.Predicate == FilterPredicates.In ? x.values : null,
                                   });

            return filterCriterias;
        }

        public IEnumerable<SortCriteria> GetSorts<T>(string sortColumn, bool desc)
        {
            PropertyInfo[] properties = typeof(T).GetCustomAttribute<MetadataTypeAttribute>(false)?.MetadataClassType.GetProperties() ?? Array.Empty<PropertyInfo>();

            IEnumerable<SortCriteria> filterCriterias = from pi in properties
                                                        let sortAtt = pi.GetCustomAttribute<SortColumnAttribute>()
                                                        where !string.IsNullOrEmpty(sortColumn) && sortAtt != null && (sortAtt.GroupName.Equals(sortColumn, StringComparison.InvariantCultureIgnoreCase) || sortAtt.Name.Equals(sortColumn, StringComparison.InvariantCultureIgnoreCase))
                                                        orderby sortAtt.Order
                                                        select new SortCriteria()
                                                        {
                                                            FieldName = pi.Name,
                                                            Direction = desc ? SortDirection.Descending : SortDirection.Ascending
                                                        };

            return filterCriterias;
        }

        private Expression ApplyPredicate(ParameterExpression parameter, FilterCriteria filterCondition)
        {
            switch (filterCondition.Predicate)
            {
                case FilterPredicates.Contains:
                {
                    return this.ContainsPredicate(parameter, filterCondition.FieldName, filterCondition.Value);
                }

                case FilterPredicates.StartsWith:
                {
                    return this.StartsWithPredicate(parameter, filterCondition.FieldName, filterCondition.Value);
                }

                case FilterPredicates.EndsWith:
                {
                    return this.EndsWithPredicate(parameter, filterCondition.FieldName, filterCondition.Value);
                }

                case FilterPredicates.Equal:
                {
                    return this.EqualPredicate(parameter, filterCondition.FieldName, filterCondition.Value);
                }

                case FilterPredicates.DoesNotEqual:
                {
                    return this.DoesNotEqualPredicate(parameter, filterCondition.FieldName, filterCondition.Value);
                }

                case FilterPredicates.LessThan:
                {
                    return this.LessThanPredicate(parameter, filterCondition.FieldName, filterCondition.Value);
                }

                case FilterPredicates.LessThanOrEqual:
                {
                    return this.LessThanOrEqualPredicate(parameter, filterCondition.FieldName, filterCondition.Value);
                }

                case FilterPredicates.GreaterThan:
                {
                    return this.GreaterThanPredicate(parameter, filterCondition.FieldName, filterCondition.Value);
                }

                case FilterPredicates.GreaterThanOrEqual:
                {
                    return this.GreaterThanOrEqualPredicate(parameter, filterCondition.FieldName, filterCondition.Value);
                }

                case FilterPredicates.In:
                {
                    return this.InPredicate(parameter, filterCondition.FieldName, filterCondition.Values);
                }

                default:
                    return this.ContainsPredicate(parameter, filterCondition.FieldName, filterCondition.Value);
            }
        }

        private Expression ContainsPredicate(ParameterExpression parameter, string fieldName, object? fieldValue)
        {
            MethodInfo containsMethod = typeof(string).GetMethod("Contains", new[] { typeof(string) });
            Expression property = this.GetPropertyEnsureIsString(parameter, fieldName);

            ConstantExpression value = Expression.Constant(Convert.ToString(fieldValue));

            return Expression.Call(property, containsMethod, value);
        }

        private Expression CreatePredicate(ParameterExpression parameter, string fieldName, object fieldValue, Func<Expression, Expression, BinaryExpression> binaryExpression)
        {
            (ConstantExpression value, MemberExpression property) = this.GetValueAndProperty(parameter, fieldName, fieldValue);

            return binaryExpression(property, value);
        }

        private Expression DoesNotEqualPredicate(ParameterExpression parameter, string fieldName, object fieldValue)
        {
            return this.CreatePredicate(parameter, fieldName, fieldValue, Expression.NotEqual);
        }

        private Expression EndsWithPredicate(ParameterExpression parameter, string fieldName, object fieldValue)
        {
            MethodInfo containsMethod = typeof(string).GetMethod("EndsWith", new[] { typeof(string) });
            Expression property = this.GetPropertyEnsureIsString(parameter, fieldName);

            ConstantExpression value = Expression.Constant(Convert.ToString(fieldValue));

            return Expression.Call(property, containsMethod, value);
        }

        private Expression EqualPredicate(ParameterExpression parameter, string fieldName, object fieldValue)
        {
            return this.CreatePredicate(parameter, fieldName, fieldValue, Expression.Equal);
        }

        private bool FilterByFieldName(PropertyInfo propertyInfo, string fieldName)
        {
            ColumnAttribute ca = propertyInfo.GetCustomAttribute<ColumnAttribute>();
            return ca != null ? ca.Name == fieldName : propertyInfo.Name == fieldName;
        }

        private MemberExpression GetProperty(Expression left, string fieldName)
        {
            PropertyInfo[] propertyInfos = left.Type.GetProperties();

            //.Where(prop => prop.IsDefined(typeof(ColumnAttribute), false));

            int dotIndex = fieldName.IndexOf(".", StringComparison.Ordinal);
            PropertyInfo propertyInfo = GetPropertyInfo();

            if (dotIndex > 0)
            {
                return this.GetProperty(
                    Expression.Property(left, propertyInfo.Name),
                    fieldName.Substring(dotIndex + 1));
            }

            return Expression.Property(left, propertyInfo.Name);

            PropertyInfo GetPropertyInfo()
            {
                if (dotIndex > 0)
                {
                    return propertyInfos.FirstOrDefault(prop => this.FilterByFieldName(prop, fieldName.Substring(0, dotIndex)));
                }

                return propertyInfos.FirstOrDefault(prop => this.FilterByFieldName(prop, fieldName));
            }
        }

        private Expression GetPropertyEnsureIsString(ParameterExpression parameter, string fieldName)
        {
            MemberExpression propertyMaybeString = this.GetProperty(parameter, fieldName);

            if (propertyMaybeString.Type != typeof(string))
            {
                MethodInfo toStringMethod = typeof(object).GetMethod("ToString");
                return Expression.Call(propertyMaybeString, toStringMethod);
            }

            return propertyMaybeString;
        }

        private (ConstantExpression value, MemberExpression property) GetValueAndProperty(ParameterExpression parameter, string fieldName, object fieldValue)
        {
            MemberExpression property = this.GetProperty(parameter, fieldName);
            if (property.Type.IsEnum)
            {
                return (Expression.Constant(Enum.ToObject(property.Type, fieldValue)), property);
            }

            if (property.Type == typeof(Guid))
            {
                return (Expression.Constant(Guid.Parse(fieldValue.ToString())), property);
            }

            ConstantExpression value = fieldValue == null ? Expression.Constant(null, property.Type) : Expression.Constant(Convert.ChangeType(fieldValue, property.Type));

            return (value, property);
        }

        private Expression GreaterThanOrEqualPredicate(ParameterExpression parameter, string fieldName, object fieldValue)
        {
            return this.CreatePredicate(parameter, fieldName, fieldValue, Expression.GreaterThanOrEqual);
        }

        private Expression GreaterThanPredicate(ParameterExpression parameter, string fieldName, object fieldValue)
        {
            return this.CreatePredicate(parameter, fieldName, fieldValue, Expression.GreaterThan);
        }

        private Expression InPredicate(ParameterExpression parameter, string fieldName, List<object> fieldValues)
        {
            MethodInfo containsMethod = typeof(List<object>)
                .GetMethod("Contains", new[] { typeof(object) });
            MemberExpression property = this.GetProperty(parameter, fieldName);
            ConstantExpression values = null;
            if (property.Type.BaseType == typeof(System.Enum))
            {
                values = Expression.Constant(fieldValues.Select(fieldValue => Convert.ChangeType(Enum.ToObject(property.Type, fieldValue), property.Type)).ToList());
            }
            else
            {
                values = Expression.Constant(fieldValues.Select(fieldValue => Convert.ChangeType(fieldValue, property.Type)).ToList());
            }

            return Expression.Call(values, containsMethod, Expression.Convert(property, typeof(object)));
        }

        private Expression LessThanOrEqualPredicate(ParameterExpression parameter, string fieldName, object fieldValue)
        {
            return this.CreatePredicate(parameter, fieldName, fieldValue, Expression.LessThanOrEqual);
        }

        private Expression LessThanPredicate(ParameterExpression parameter, string fieldName, object fieldValue)
        {
            return this.CreatePredicate(parameter, fieldName, fieldValue, Expression.LessThan);
        }

        private Expression StartsWithPredicate(ParameterExpression parameter, string fieldName, object? fieldValue)
        {
            MethodInfo? containsMethod = typeof(string).GetMethod("StartsWith", new[] { typeof(string) });
            Expression property = this.GetPropertyEnsureIsString(parameter, fieldName);

            ConstantExpression value = Expression.Constant(Convert.ToString(fieldValue));

            return Expression.Call(property, containsMethod, value);
        }
    }
}
