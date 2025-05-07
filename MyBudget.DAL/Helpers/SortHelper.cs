using System.Linq.Dynamic.Core;
using System.Reflection;

namespace MyBudget.DAL.Helpers;

public class SortHelper<T> : ISortHelper<T>
{
    public IQueryable<T> ApplySort(IQueryable<T> entities, string? orderByQueryString)
    {
        if (string.IsNullOrWhiteSpace(orderByQueryString))
            return entities;

        var orderParams = orderByQueryString.Trim().Split(',');
        var propertyInfos = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);

        var orderQuery = "";

        foreach (var param in orderParams)
        {
            if (string.IsNullOrWhiteSpace(param)) continue;

            var propertyFromQueryName = param.Split(" ")[0];
            var objectProperty = propertyInfos.FirstOrDefault(p =>
                string.Equals(p.Name, propertyFromQueryName, StringComparison.InvariantCultureIgnoreCase));

            if (objectProperty is null) continue;

            var direction = param.EndsWith(" desc") ? "descending" : "ascending";
            orderQuery += $"{objectProperty.Name} {direction}, ";
        }

        orderQuery = orderQuery.TrimEnd(',', ' ');

        return string.IsNullOrWhiteSpace(orderQuery)
            ? entities
            : entities.OrderBy(orderQuery);
    }
}