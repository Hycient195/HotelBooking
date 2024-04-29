using HotelBooking.Core.Models;
using PagedList;
using System.Linq.Expressions;

namespace HotelBooking.Core.IReposotory
{
    public interface IGenericRepository<T> where T : class
    {
        Task<T> Get(Expression<Func<T, bool>> expression, List<string> includes = null);

        Task<IList<T>> GetAll(Expression<Func<T, bool>> expression = null, Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null, List<string> includes = null);

        IPagedList<T> GetByPage(PagingRequestParams requestParams, List<string> includes = null);
        Task Insert(T entity);

        Task InsertRange(IEnumerable<T> entities);
        
        Task Delete(int id);

        void DeleteRange(IEnumerable<T> ids);

        void Update(T entity);
    }
}
