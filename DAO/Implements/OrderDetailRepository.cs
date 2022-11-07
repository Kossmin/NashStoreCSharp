using NashPhaseOne.BusinessObjects.Models;
using DAO.Implements;
using NashPhaseOne.DAO.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NashPhaseOne.DAO.Implements
{
    public class OrderDetailRepository : Repository<OrderDetail>, IOrderDetailRepository
    {
        public OrderDetailRepository(NashStoreDbContext nashStoreDbContext) : base(nashStoreDbContext)
        {
        }
    }
}
