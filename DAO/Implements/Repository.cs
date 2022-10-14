using BusinessObjects.Models;
using DAO.Interfaces;
using DTO.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace DAO.Implements
{
    public class Repository<T> : IRepository<T> where T : class
    {
        protected NashStoreDbContext _nashStoreDbContext;

        public Repository(NashStoreDbContext nashStoreDbContext)
        {
            _nashStoreDbContext = nashStoreDbContext;
        }

        public Task DeleteAsync(T entity)
        {
            _nashStoreDbContext.Set<T>().Remove(entity);
            return Task.CompletedTask;
        }

        public virtual IQueryable<T> GetAll()
        {
            return _nashStoreDbContext.Set<T>().AsQueryable();
        }

        public virtual Task<T> GetByAsync(Expression<Func<T, bool>> expression)
        {
            return _nashStoreDbContext.Set<T>().FirstOrDefaultAsync(expression);
        }

        public virtual IQueryable<T> GetMany(Expression<Func<T, bool>> expression)
        {
            return _nashStoreDbContext.Set<T>().Where(expression);
        }

        public async Task<ViewListModel<T>> PagingAsync(IQueryable<T> records, int pageIndex, int pageSize)
        {
            var maxNumberOfPage = records.Count()/pageSize;
            if(records.Count() % pageSize > 0)
            {
                maxNumberOfPage++;
            }
            if(pageIndex > maxNumberOfPage)
            {
                throw new IndexOutOfRangeException();
            }
            var listResult = await records.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync();
            return new ViewListModel<T> { ModelDatas = listResult, MaxPage = maxNumberOfPage, PageIndex = pageIndex};
        }

        public async Task SaveAsync(T entity)
        {
            _nashStoreDbContext.Set<T>().AddAsync(entity);
        }

        public async Task UpdateAsync(T entity)
        {
            _nashStoreDbContext.Set<T>().Update(entity);
        }
    }
}
