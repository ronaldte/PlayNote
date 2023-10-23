using PlayNote.Entities;

namespace PlayNote.Services
{
    /// <summary>
    /// Extension method used in service layer.
    /// </summary>
    public static class ServiceExtensions
    {
        /// <summary>
        /// Paginates given <see cref="IQueryable{T}"/> and returns new one with objects for given page number and page size.
        /// </summary>
        /// <typeparam name="T">Type of object in collection.</typeparam>
        /// <param name="obj">Collection of objects.</param>
        /// <param name="pageNumber">Number of page to be selected.</param>
        /// <param name="pageSize">Size of the page to be selected.</param>
        /// <returns>New collection with object for given page number and page size.</returns>
        public static IQueryable<T> Paginate<T>(this IQueryable<T> obj, int pageNumber, int pageSize) where T : class, IBaseEntity
        {
            return obj.OrderBy(e => e.Id).Skip((pageNumber - 1) * pageSize).Take(pageSize);
        }
    }
}
