using BusinessObjects.Models;
using DAO.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAO.Implements
{
    public class UnitOfWork : IUnitOfWork
    {
        private NashStoreDbContext _context;

        public UnitOfWork(NashStoreDbContext context)
        {
            _context = context; 
        }

        public Task CommitAsync()
        {
            return _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
