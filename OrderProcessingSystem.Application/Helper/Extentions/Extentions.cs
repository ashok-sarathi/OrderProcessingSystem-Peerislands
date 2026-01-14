using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;

namespace OrderProcessingSystem.Application.Helper.Extentions
{
    [ExcludeFromCodeCoverage]
    public static class LinqExtensions
    {
        public static IQueryable<T> WhereIf<T>(this IQueryable<T> source,
            bool condition, Expression<Func<T, bool>> predicate)
        {
            return condition ? source.Where(predicate) : source;
        }

        public static IEnumerable<T> WhereIf<T>(this IEnumerable<T> source,
            bool condition, Func<T, bool> predicate)
        {
            return condition ? source.Where(predicate) : source;
        }
    }
}
