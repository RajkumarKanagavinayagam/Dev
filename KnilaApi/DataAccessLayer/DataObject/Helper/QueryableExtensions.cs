using System.Linq.Expressions;

namespace KnilaApi.DataAccessLayer.DataObject.Helper
{
    public static class QueryableExtensions
    {
        public static IQueryable<T> ApplySorting<T>(this IQueryable<T> query, string sortColumn, bool ascending)
        {
            if (string.IsNullOrEmpty(sortColumn))
            {
                return query;
            }

            var parameter = Expression.Parameter(typeof(T), "p");
            var property = Expression.Property(parameter, sortColumn);
            var orderByExpression = Expression.Lambda(property, parameter);

            string methodName = ascending ? "OrderBy" : "OrderByDescending";

            var resultExpression = Expression.Call(
                typeof(Queryable),
                methodName,
                new Type[] { typeof(T), property.Type },
                query.Expression,
                Expression.Quote(orderByExpression)
            );

            return query.Provider.CreateQuery<T>(resultExpression);
        }
    }
}
