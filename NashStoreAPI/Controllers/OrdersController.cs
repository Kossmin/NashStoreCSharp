using BusinessObjects.Models;
using DAO.Interfaces;
using DTO.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NashPhaseOne.DAO.Interfaces;
using NashPhaseOne.DTO.Models.Order;

namespace NashPhaseOne.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class OrdersController : ControllerBase
    {
        private IOrderRepository _orderRepository;
        private IOrderDetailRepository _orderDetailRepository;
        private IUnitOfWork _unitOfWork;

        public OrdersController(IUnitOfWork unitOfWork, IOrderDetailRepository orderDetailRepository, IOrderRepository orderRepository)
        {
            _unitOfWork = unitOfWork;
            _orderRepository = orderRepository;
            _orderDetailRepository = orderDetailRepository;
        }

        [HttpGet]
        public async Task<ViewListDTO<Order>> GetAll()
        {
            return await _orderRepository.PagingAsync(_orderRepository.GetAll());
        }

        [HttpPost]
        public async Task<ActionResult> Create(OrderDTO order)
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
            return NoContent();
        }
    }
}
