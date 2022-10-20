using AutoMapper;
using BusinessObjects.Models;
using DAO.Interfaces;
using DTO.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NashPhaseOne.DAO.Interfaces;
using NashPhaseOne.DTO.Models;
using NashPhaseOne.DTO.Models.Order;

namespace NashPhaseOne.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize]
    public class OrdersController : ControllerBase
    {
        private IOrderRepository _orderRepository;
        private IOrderDetailRepository _orderDetailRepository;
        private IUnitOfWork _unitOfWork;
        private IMapper _mapper;
        private IProductRepository _productRepository;

        public OrdersController(IProductRepository productRepository, IMapper mapper, IUnitOfWork unitOfWork, IOrderDetailRepository orderDetailRepository, IOrderRepository orderRepository)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _orderRepository = orderRepository;
            _orderDetailRepository = orderDetailRepository;
            _productRepository = productRepository;
        }

        [HttpGet]
        public async Task<ViewListDTO<Order>> GetAll()
        {
            return await _orderRepository.PagingAsync(_orderRepository.GetAll());
        }

        [HttpPost("cart")]
        [AllowAnonymous]
        public async Task<CartDTO> GetCart([FromBody]UserIdString userId)
        {
            var order = await _orderRepository.GetByAsync(o => o.Status == OrderStatus.Ordering && o.UserId == userId.Id);

            if(order != null)
            {
                var mappedOrder = _mapper.Map<Order, CartDTO>(order);
                return mappedOrder;
            }
            else
            {
                return new CartDTO();
            }
        }

        [HttpPost("checkout")]
        //[AllowAnonymous]
        public async Task<ActionResult> Checkout([FromBody]UserIdString userId)
        {
            var order = await _orderRepository.GetByAsync(o => o.Status == OrderStatus.Ordering && o.UserId == userId.Id);

            var orderDetails = order.OrderDetails;

            foreach (var orderDetail in orderDetails)
            {
                var prod = await _productRepository.GetByAsync(p => p.Id == orderDetail.ProductId);
                if(prod.Quantity < orderDetail.Quantity)
                {
                    return BadRequest(new { message = "This item don't have enough quantity for you!" });
                }
                prod.Quantity -= orderDetail.Quantity;
                if(prod.Quantity == 0)
                {
                    prod.IsDeleted = true;
                }
                await _productRepository.UpdateAsync(prod);
            }

            order.Status = OrderStatus.Paid;
            await _orderRepository.UpdateAsync(order);
            await _unitOfWork.CommitAsync();
            return Ok();
        }

        [HttpPost("create")]
        [Authorize]
        public async Task Create(OrderDTO order)
        {
            var availableOrder = await _orderRepository.GetByAsync(o => o.UserId == order.UserId && o.Status == BusinessObjects.Models.OrderStatus.Ordering);
            if(availableOrder == null)
            {
                Order newOrder = new Order { Status = OrderStatus.Ordering, UserId = order.UserId, OrderDetails = new List<OrderDetail>() };
                newOrder.OrderDetails.Add(new OrderDetail { ProductId = order.ProductId, Quantity = order.Quantity, Price = order.UnitPrice * order.Quantity });
                await _orderRepository.SaveAsync(newOrder);
            }
            else
            {
                var listOfOrderDetails = availableOrder.OrderDetails;
                var oldOrderDetail = listOfOrderDetails.FirstOrDefault(od => od.ProductId == order.ProductId);
                if(oldOrderDetail == null)
                {
                    listOfOrderDetails.Add(new OrderDetail { OrderId = availableOrder.Id, ProductId = order.ProductId, Quantity = order.Quantity, Price = order.UnitPrice * order.Quantity });
                }
                else
                {
                    oldOrderDetail.Quantity += order.Quantity;
                }
                await _orderRepository.UpdateAsync(availableOrder);
            }
            await _unitOfWork.CommitAsync();
        }
    }
}
