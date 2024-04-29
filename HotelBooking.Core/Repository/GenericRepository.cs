using HotelBooking.Data;
using HotelBooking.Core.Models;
using HotelBooking.Core.IReposotory;
using Microsoft.EntityFrameworkCore;
using PagedList;
using System.Linq.Expressions;

namespace HotelBooking.Core.Repository
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        private readonly DbContext _context;
        private readonly DbSet<T> _db;

        public GenericRepository(DbContext context)
        {
            _context = context;
            _db = _context.Set<T>();
        }

        public async Task Delete(int id)
        {
            var item = await _db.FindAsync(id);
            _db.Remove(item);
        }

        public void DeleteRange(IEnumerable<T> items)
        {
            _db.RemoveRange(items);
        }

        public async Task<T> Get(Expression<Func<T, bool>> expression, List<string> includes = null)
        {
            IQueryable<T> query = _db;
            if (includes != null)
            {
                foreach (var property in includes)
                {
                    query = query.Include(property);
                }
            }
            return await query.AsNoTracking().FirstOrDefaultAsync(expression);
        }

        public async Task<IList<T>> GetAll(Expression<Func<T, bool>> expression = null, Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null, List<string> includes = null)
        {
            IQueryable<T> query = _db;
            if (expression != null)
            {
                query = query.Where(expression);
            }

            if (includes != null)
            {
                foreach(var property in includes)
                {
                    query = query.Include(property);
                }
            }

            if (orderBy != null)
            {
                query = orderBy(query);
            }

            return await query.AsNoTracking().ToListAsync();
        }

        public IPagedList<T> GetByPage(PagingRequestParams requestParams, List<string> includes = null)
        {
            IQueryable<T> query = _db;
            if (includes != null)
            {
                foreach (var property in includes)
                {
                    query = query.Include(property);
                }
            }
            return query.AsNoTracking().ToPagedList(requestParams.PageNumber, requestParams.PageSize);
        }

        public async Task Insert(T entity)
        {
            await _db.AddAsync(entity);
        }

        public async Task InsertRange(IEnumerable<T> entities)
        {
            await _db.AddRangeAsync(entities);
        }

        public void Update(T entity)
        {
            _db.Attach(entity);
            _context.Entry(entity).State = EntityState.Modified;
        }
    }
}
