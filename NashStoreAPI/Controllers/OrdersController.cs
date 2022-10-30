using AutoMapper;
using BusinessObjects.Models;
using DAO.Interfaces;
using DTO.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
        public async Task<ViewListDTO<Order>> GetAll([FromQuery]int pageIndex = 1)
        {
            return await _orderRepository.PagingAsync(_orderRepository.GetAll().Where(x=>x.Status != OrderStatus.Ordering), pageIndex);
        }

        [HttpGet("getorderstatistic")]
        public async Task<List<OrderStatisticDTO>> GetOrderStatistics()
        {
            var orders = await _orderRepository.GetAll().Where(x=>x.Status != OrderStatus.Ordering).ToListAsync();
            var paidOrders = orders.Where(x => x.Status == OrderStatus.Paid);
            var delivering = orders.Where(x => x.Status == OrderStatus.Delivering);
            var delivered = orders.Where(x => x.Status == OrderStatus.Delivered);
            var pending = orders.Where(x => x.Status == OrderStatus.Pending);
            var canceled = orders.Where(x => x.Status == OrderStatus.Canceled);
            return new List<OrderStatisticDTO> {
                new OrderStatisticDTO { Type = OrderStatus.Paid.ToString(), Amount = paidOrders.Count() },
                new OrderStatisticDTO { Type = OrderStatus.Delivering.ToString(), Amount = delivering.Count() },
                new OrderStatisticDTO { Type = OrderStatus.Delivered.ToString(), Amount = delivered.Count() },
                new OrderStatisticDTO { Type = OrderStatus.Pending.ToString(), Amount = pending.Count() },
                new OrderStatisticDTO { Type = OrderStatus.Canceled.ToString(), Amount = canceled.Count()},};
        }

        [HttpPost("cart")]
        [Authorize]
        public async Task<ListOrderDetailsDTO> GetCart([FromBody]IdString userId)
        {
            var order = await _orderRepository.GetByAsync(o => o.Status == OrderStatus.Ordering && o.UserId == userId.Id);

            if(order != null)
            {
                var mappedOrder = _mapper.Map<Order, ListOrderDetailsDTO>(order);
                return mappedOrder;
            }
            else
            {
                return new ListOrderDetailsDTO();
            }
        }

        [HttpPut("cancel")]
        [Authorize]
        public async Task<ActionResult> CancelOrders([FromBody] IdString id)
        {
            var order = await _orderRepository.GetByAsync(o => o.Id == int.Parse(id.Id));

            if (order == null)
            {
                return BadRequest(new { message = "Can't find this order" });
            }
            else
            {
                order.Status = OrderStatus.Canceled;
                await _orderRepository.UpdateAsync(order);

                var orderDetails = order.OrderDetails;

                foreach (var orderDetail in orderDetails)
                {
                    var prod = await _productRepository.GetByAsync(x => x.Id == orderDetail.ProductId);
                    prod.Quantity += orderDetail.Quantity;

                    await _productRepository.UpdateAsync(prod);
                }
                await _unitOfWork.CommitAsync();
                return Ok();
            }
        }

        [HttpPost("canceled")]
        [Authorize]
        public async Task<List<ListOrderDetailsDTO>> GetCanceledOrders([FromBody] IdString userId)
        {
            var order = _orderRepository.GetMany(o => o.Status == OrderStatus.Canceled && o.UserId == userId.Id).Include(x => x.OrderDetails).ThenInclude(x => x.Product).ToList();

            if (order != null)
            {
                var mappedOrder = _mapper.Map<List<Order>, List<ListOrderDetailsDTO>>(order);
                return mappedOrder;
            }
            else
            {
                return new List<ListOrderDetailsDTO>();
            }
        }

        [HttpPost("paid")]
        [Authorize]
        public async Task<List<ListOrderDetailsDTO>> GetPaidOrders([FromBody] IdString userId)
        {
            var order = _orderRepository.GetMany(o => o.Status == OrderStatus.Paid && o.UserId == userId.Id).Include(x=>x.OrderDetails).ThenInclude(x=>x.Product).ToList();

            if (order != null)
            {
                var mappedOrder = _mapper.Map<List<Order>, List<ListOrderDetailsDTO>>(order);
                return mappedOrder;
            }
            else
            {
                return new List<ListOrderDetailsDTO>();
            }
        }

        [HttpPost("delivering")]
        [Authorize]
        public async Task<List<ListOrderDetailsDTO>> GetDeliveringOrders([FromBody] IdString userId)
        {
            var order = _orderRepository.GetMany(o => o.Status == OrderStatus.Delivering && o.UserId == userId.Id).ToList();

            if (order != null)
            {
                var mappedOrder = _mapper.Map<List<Order>, List<ListOrderDetailsDTO>>(order);
                return mappedOrder;
            }
            else
            {
                return new List<ListOrderDetailsDTO>();
            }
        }

        [HttpPost("checkout")]
        //[AllowAnonymous]
        public async Task<ActionResult> Checkout([FromBody]IdString userId)
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
            order.OrderDate = DateTime.UtcNow;
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
