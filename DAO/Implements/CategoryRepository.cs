using DAO.Interfaces;
using Microsoft.EntityFrameworkCore;
using NashPhaseOne.BusinessObjects.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DAO.Implements
{
    public class CategoryRepository : Repository<Category>, ICategoryRepository
    {
        public CategoryRepository(NashStoreDbContext nashStoreDbContext) : base(nashStoreDbContext)
        {
        }

        public override async Task<Category> GetByAsync(Expression<Func<Category, bool>> expression)
        {
            return await _nashStoreDbContext.Categories.Include(x => x.Products).FirstOrDefaultAsync(expression);
        }
    }
}
