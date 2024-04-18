using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Org.BouncyCastle.Asn1.X509.Qualified;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text.RegularExpressions;

namespace DiplomaApi.Helpers.FilterHelper
{
    public class FilterHelperAttribute : ActionFilterAttribute
    {
        private readonly Type _filterType;

        public FilterHelperAttribute(Type filterType)
        {
            _filterType = filterType;
        }

        public override void OnActionExecuted(ActionExecutedContext context)
        {
            var query = context.HttpContext.Request.Query["$filter"].ToString();

            if (!string.IsNullOrEmpty(query))
            {
                var filters = ParseFilters(query);

                // Применяем фильтры к результату запроса
                var result = ApplyFilters(context.Result as OkObjectResult, filters);

                // Заменяем результат запроса отфильтрованным результатом
                var executedResult = context.Result as ObjectResult;
                executedResult.Value = result;
            }
        }

        private dynamic ParseFilters(string query)
        {
            var regex = new Regex(@"(\w+)\s*(gt|ls)\s*([\d.-]+)");
            var matches = regex.Matches(query);

            var filters = matches.Select(match => new
            {
                Field = match.Groups[1].Value,
                Operator = match.Groups[2].Value,
                Value = Convert.ChangeType(match.Groups[3].Value, _filterType.GetProperty(match.Groups[1].Value).PropertyType) // Преобразуем значение в соответствующий тип
            }).ToList();

            return filters;
        }

        private IQueryable ApplyFilters(OkObjectResult result, dynamic filters)
        {
            var queryable = result.Value as IQueryable;

            if (queryable != null)
            {
                foreach (var filter in filters)
                {
                    var field = filter.Field;
                    var value = filter.Value;

                    // Создаем выражение для фильтрации
                    var parameter = Expression.Parameter(_filterType, "x");
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
                        new[] { _filterType },
                        queryable.Expression,
                        lambda);

                    queryable = queryable.Provider.CreateQuery(whereCallExpression);
                }

                return queryable;
            }
            return filters;
        }
    }
}
