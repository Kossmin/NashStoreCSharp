using BusinessObjects.Models;
using DAO.Implements;
using Microsoft.EntityFrameworkCore;
using NashPhaseOne.DAO.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace NashPhaseOne.DAO.Implements
{
    public class OrderRepository : Repository<Order>, IOrderRepository
    {
        public OrderRepository(NashStoreDbContext nashStoreDbContext) : base(nashStoreDbContext)
        {
        }

        public virtual Task<Order> GetByAsync(Expression<Func<Order, bool>> expression)
        {
            return _nashStoreDbContext.Set<Order>().Include(o => o.OrderDetails).FirstOrDefaultAsync(expression);
        }

        public virtual IQueryable<Order> GetMany(Expression<Func<Order, bool>> expression)
        {
            return _nashStoreDbContext.Set<Order>().Include(o => o.OrderDetails).Where(expression);
        }
    }
}
