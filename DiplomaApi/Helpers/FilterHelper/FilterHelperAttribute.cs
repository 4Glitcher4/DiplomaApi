using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Org.BouncyCastle.Asn1.X509.Qualified;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text.RegularExpressions;

namespace DiplomaApi.Helpers.FilterHelper
{
    public class FilterHelperAttribute<T> : ActionFilterAttribute
    {
        public override void OnActionExecuted(ActionExecutedContext context)
        {
            var filter = context.HttpContext.Request.Query["$filter"].ToString();
            var sort = context.HttpContext.Request.Query["$sort"].ToString();
            var take = context.HttpContext.Request.Query["$take"].ToString();
            var skip = context.HttpContext.Request.Query["$skip"].ToString();

            var filters = ParseFilters(filter);
            // Применяем фильтры к результату запроса
            (IQueryable result, int count) = ((IQueryable, int))ApplyFilters(context.Result as OkObjectResult, filters, skip, take, sort);

            // Заменяем результат запроса отфильтрованным результатом
            var executedResult = context.Result as ObjectResult;
            executedResult.Value = new { count = count, result = result };

        }

        private dynamic ParseFilters(string filter)
        {
            var regex = new Regex(@"(\w+)\s*(gt|ls)\s*([\d.-]+)");
            var matches = regex.Matches(filter);

            var filters = matches.Select(match => new FieldFilter
            {
                Field = match.Groups[1].Value,
                Operator = match.Groups[2].Value,
                Value = Convert.ChangeType(match.Groups[3].Value, typeof(T).GetProperty(match.Groups[1].Value).PropertyType) // Преобразуем значение в соответствующий тип
            }).ToList();

            return filters;
        }

        private (IQueryable result, int count) ApplyFilters(OkObjectResult result, List<FieldFilter> filters, string skip, string take, string sort)
        {
            var queryable = result.Value as IQueryable<T>;

            if (queryable != null)
            {
                var count = queryable.Count();
                // Применяем сортировку
                var results = queryable.ApplyFilter(filters)
                    .ApplySort(sort)
                    .Skip(skip)
                    .Take(take);
                
                return (results, count);
            }
            return (queryable, 0);
        }
    }
}
