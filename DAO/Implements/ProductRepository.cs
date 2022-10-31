using BusinessObjects.Models;
using DAO.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DAO.Implements
{
    public class ProductRepository : Repository<Product>, IProductRepository
    {
        public ProductRepository(NashStoreDbContext nashStoreDbContext) : base(nashStoreDbContext)
        {
        }

        public override Task<Product> GetByAsync(Expression<Func<Product, bool>> expression)
        {
            return _nashStoreDbContext.Products.Include(p => p.Category).FirstOrDefaultAsync(expression);
        }

        public override IQueryable<Product> GetMany(Expression<Func<Product, bool>> expression)
        {
            return _nashStoreDbContext.Products.Include(p => p.Category).Where(expression);
        }

        public override IQueryable<Product> GetAll()
        {
            return _nashStoreDbContext.Products.Include(x=> x.Category);
        }

        public override async Task UpdateAsync(Product entity)
        {
            var product = _nashStoreDbContext.Products.AsNoTracking().FirstOrDefault(x=> x.Id == entity.Id);
            if (entity.ImgUrls.Count() == 0)
            {
                entity.ImgUrls = product.ImgUrls;
            }
            if(entity.Version == product.Version)
            {
                entity.Version++;
                _nashStoreDbContext.Products.Update(entity);
            }
            else
            {
                throw new TaskCanceledException("There is a change during the update process");
            }
        }
    }
}
