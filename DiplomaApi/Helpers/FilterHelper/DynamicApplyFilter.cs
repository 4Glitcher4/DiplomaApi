using System.Linq;
using System.Linq.Expressions;

namespace DiplomaApi.Helpers.FilterHelper
{
    public static class DynamicApplyFilter
    {
        public static IQueryable<T> ApplySort<T>(this IQueryable<T> queryable, string sort)
        {
            if (!string.IsNullOrEmpty(sort))
            {
                var sorts = sort.Split(',')
                    .Select(s => s.Trim().Split(' '))
                    .Select(s => new
                    {
                        Field = s[0],
                        Direction = s[1].ToLower() == "asc" ? nameof(Enumerable.OrderBy) : nameof(Enumerable.OrderByDescending)
                    })
                    .ToList();

                var parameter = Expression.Parameter(typeof(T), "x");
                var orderByExp = queryable.Expression;

                foreach (var sortItem in sorts)
                {
                    var property = Expression.Property(parameter, sortItem.Field);
                    orderByExp = Expression.Call(
                        typeof(Queryable),
                        sortItem.Direction,
                        new[] { typeof(T), property.Type },
                        orderByExp,
                        Expression.Lambda(property, parameter));
                }

                queryable = queryable.Provider.CreateQuery<T>(orderByExp);
                return queryable;
            }
            return queryable;
        }

        //public static int Count<T>(this IQueryable<T> queryable)
        //{
        //    var countExpression = Expression.Call(
        //           typeof(Queryable),
        //           nameof(Enumerable.Count),
        //           new[] { typeof(T) },
        //           queryable.Expression);


        //    var countLambda = Expression.Lambda<Func<int>>(countExpression);
        //    return queryable.Provider.Execute<int>(countExpression);
        //}

        public static IQueryable<T> ApplyFilter<T>(this IQueryable<T> queryable, List<FieldFilter> filters)
        {
            foreach (var filter in filters)
            {
                var field = filter.Field;
                var value = filter.Value;

                // Создаем выражение для фильтрации
                var parameter = Expression.Parameter(typeof(T), "x");
                var property = Expression.Property(parameter, field);
                var constant = Expression.Constant(value, value.GetType());
                BinaryExpression comparison;

                if (filter.Operator == "gt")
                {
                    comparison = Expression.GreaterThan(property, constant);
                }
                else if (filter.Operator == "ls")
                {
                    comparison = Expression.LessThan(property, constant);
                }
                else
                {
                    continue;
                }

                var lambda = Expression.Lambda(comparison, parameter);
                var whereCallExpression = Expression.Call(
                    typeof(Queryable),
                    nameof(Enumerable.Where),
                    new[] { typeof(T) },
                    queryable.Expression,
                    lambda);

                queryable = queryable.Provider.CreateQuery<T>(whereCallExpression);
            }

            return queryable;
        }

        public static IQueryable<T> Skip<T>(this IQueryable<T> queryable, string skip)
        {
            if (!string.IsNullOrEmpty(skip))
            {
                return queryable.Provider.CreateQuery<T>(
                Expression.Call(
                        typeof(Queryable),
                        nameof(Enumerable.Skip),
                        new[] { typeof(T) },
                        queryable.Expression,
                        Expression.Constant(int.Parse(skip))));

            }

            return queryable;
        }

        public static IQueryable<T> Take<T>(this IQueryable<T> queryable, string take)
        {
            if (!string.IsNullOrEmpty(take))
            {
                return queryable.Provider.CreateQuery<T>(
                Expression.Call(
                        typeof(Queryable),
                        nameof(Enumerable.Take),
                        new[] { typeof(T) },
                        queryable.Expression,
                        Expression.Constant(int.Parse(take))));
            }

            return queryable;
        }
    }

    public class FieldFilter
    {
        public string Field { get; set; }
        public string Operator { get; set; }
        public object Value { get; set; }
    }
}
