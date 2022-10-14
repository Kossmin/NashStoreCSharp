using BusinessObjects.Models;
using DAO.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAO.Implements
{
    internal class CategoryRepository : Repository<Category>, ICategoryRepository
    {
        public CategoryRepository(NashStoreDbContext nashStoreDbContext) : base(nashStoreDbContext)
        {
        }
    }
}
