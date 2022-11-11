using AutoMapper;
using NashPhaseOne.BusinessObjects.Models;
using DAO.Interfaces;
using DTO.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
        private readonly IMapper _mapper;

        public RatingsController(IMapper mapper, IRatingRepository ratingRepository, IOrderRepository orderRepository, IUnitOfWork unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _ratingRepository = ratingRepository;
            _orderRepository = orderRepository;
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Create(RatingDTO model)
        {
            var userOrder = _orderRepository.GetMany(o => o.UserId == model.UserId && o.Status != OrderStatus.Ordering && o.Status != OrderStatus.Pending)?.ToList();
            if(userOrder == null)
            {
                return NotFound();
            }
            var userOrderDetails = new List<OrderDetail>();
            foreach (var item in userOrder)
            {
                userOrderDetails.AddRange(item.OrderDetails);
            }
            var ifUserByThisProduct = userOrderDetails.FirstOrDefault(od => od.ProductId == model.ProductId) != null;
            if (ifUserByThisProduct) 
            { 
                await _ratingRepository.SaveAsync(new NashPhaseOne.BusinessObjects.Models.Rating { ProductId = model.ProductId, UserId = model.UserId, Comment = model.Comment, Star = model.Star });
            }
            else
            {
                return BadRequest(new { message = "You can't review the product that you haven't buy" });
            }
            try
            {
                await _unitOfWork.CommitAsync();
            }catch(Exception e)
            {
                return BadRequest(new { message = "You have reviewed this product already" });
            }
            return Ok();
        }

        [HttpGet]
        public async Task<List<RatingDTO>> Get(int id)
        {
            var result = await _ratingRepository.GetMany(r => r.ProductId == id).ToListAsync();
            if(result.Count == 0)
            {
                return new List<RatingDTO>();
            }
            else
            {
                return _mapper.Map<List<RatingDTO>>(result);
            }
        }
    }
}
