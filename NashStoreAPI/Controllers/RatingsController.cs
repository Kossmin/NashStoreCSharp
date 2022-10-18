using DAO.Interfaces;
using DTO.Models;
using Microsoft.AspNetCore.Authorization;
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
        private readonly IOrderRepository _orderRepository;
        private readonly IUnitOfWork _unitOfWork;

        public RatingsController(IRatingRepository ratingRepository, IOrderRepository orderRepository, IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _ratingRepository = ratingRepository;
            _orderRepository = orderRepository;
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Create(RatingDTO model)
        {
            var userOrder = await _orderRepository.GetByAsync(o => o.UserId == model.CustomerId);
            var ifUserByThisProduct = userOrder?.OrderDetails.FirstOrDefault(od => od.ProductId == model.ProductId) != null;
            if (ifUserByThisProduct) 
            { 
                await _ratingRepository.SaveAsync(new BusinessObjects.Models.Rating { ProductId = model.ProductId, UserId = model.CustomerId, Comment = model.Comment, Star = BusinessObjects.Models.RatingStar.Neutral });
            }
            else
            {
                return BadRequest(new { message = "You can't review the product that you haven't buy" });
            }
            await _unitOfWork.CommitAsync();
            return Ok();
        }
    }
}
