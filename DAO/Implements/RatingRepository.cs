using BusinessObjects.Models;
using DAO.Interfaces;

namespace DAO.Implements
{
    public class RatingRepository : Repository<Rating>, IRatingRepository
    {
        public RatingRepository(NashStoreDbContext nashStoreDbContext) : base(nashStoreDbContext)
        {
        }
    }
}
