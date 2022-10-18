using DAO.Interfaces;
using DTO.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NashPhaseOne.DAO.Interfaces;
using NashPhaseOne.DTO.Models.Rating;

namespace NashStoreAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RatingsController : ControllerBase
    {
        private readonly IRatingRepository _ratingRepository;
        private readonly IOrderDetailRepository _orderdetailRepository;
        private readonly IOrderRepository _orderRepository;

        public RatingsController(IRatingRepository ratingRepository, IOrderDetailRepository orderDetailRepository, IOrderRepository orderRepository)
        {
            _ratingRepository = ratingRepository;
            _orderdetailRepository = orderDetailRepository;
            _orderRepository = orderRepository;
        }

        [HttpPost]
        public async Task<IActionResult> Create(RatingDTO model)
        {
            var userOrder = await _orderRepository.GetByAsync(o => o.UserId == model.CustomerId);
            var ifUserByThisProduct = userOrder?.OrderDetails.FirstOrDefault(od => od.ProductId == model.ProductId) != null;
            if (ifUserByThisProduct) 
            { 
                await _ratingRepository.SaveAsync(new BusinessObjects.Models.Rating { ProductId = model.ProductId, UserId = model.CustomerId, Comment = model.Comment, Star = model.Star });
            }
            else
            {
                return BadRequest(new { message = "You can't review the product that you haven't buy" });
            }
            return NoContent();
        }
    }
}
