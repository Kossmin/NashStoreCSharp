using AutoMapper;
using NashPhaseOne.BusinessObjects.Models;
using DAO.Interfaces;
using DTO.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NashPhaseOne.DAO.Interfaces;
using NashPhaseOne.DTO.Models;
using NashPhaseOne.DTO.Models.Order;
using NashPhaseOne.API.Filters;
using NashPhaseOne.DTO.Models.Statistic;

namespace NashPhaseOne.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private IOrderRepository _orderRepository;
        private IUnitOfWork _unitOfWork;
        private IMapper _mapper;
        private IProductRepository _productRepository;

        public OrdersController(IProductRepository productRepository, IMapper mapper, IUnitOfWork unitOfWork, IOrderRepository orderRepository)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _orderRepository = orderRepository;
            _productRepository = productRepository;
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        [TypeFilter(typeof(CustomAuthorizeFilter))]
        public async Task<ActionResult<ViewListDTO<AdminGetOrderDTO>>> GetAll([FromQuery]int pageIndex = 1)
        {
            var orders = await _orderRepository.PagingAsync(_orderRepository.GetMany(x => x.Status != OrderStatus.Ordering).OrderBy(x => x.Status != OrderStatus.Pending).ThenBy(x=>x.OrderDate), pageIndex);
            if (orders == null)
            {
                return new ViewListDTO<AdminGetOrderDTO>();
            }
            var castOrders = _mapper.Map<List<AdminGetOrderDTO>>(orders.ModelDatas);
            var result = new ViewListDTO<AdminGetOrderDTO> { ModelDatas = castOrders, MaxPage= orders.MaxPage, PageIndex = orders.PageIndex};
            return result;
        }

        [HttpGet("getbystatus")]
        [Authorize(Roles = "Admin")]
        [TypeFilter(typeof(CustomAuthorizeFilter))]
        public async Task<ActionResult<ViewListDTO<AdminGetOrderDTO>>> GetByStatus([FromQuery]int status, int pageIndex = 1)
        {
            var orders = await _orderRepository.PagingAsync(_orderRepository.GetAll().Where(x => x.Status == (OrderStatus)status), pageIndex);
            if(orders == null)
            {
                return new ViewListDTO<AdminGetOrderDTO>();
            }
            var castOrders = _mapper.Map<List<AdminGetOrderDTO>>(orders.ModelDatas);
            var result = new ViewListDTO<AdminGetOrderDTO> { ModelDatas = castOrders, MaxPage = orders.MaxPage, PageIndex = orders.PageIndex };
            return result;
        }

        [HttpGet("getorderstatistic")]
        [Authorize(Roles = "Admin")]
        [TypeFilter(typeof(CustomAuthorizeFilter))]
        public async Task<List<OrderStatisticDTO>> GetOrderStatistics()
        {
            var query = _orderRepository.GetMany(x => x.Status != OrderStatus.Ordering);
            if(query == null)
            {
                return null;
            }
            var orders = query.ToList();
            var paidOrders = orders.Where(x => x.Status == OrderStatus.Done);
            var delivering = orders.Where(x => x.Status == OrderStatus.Delivering);
            var delivered = orders.Where(x => x.Status == OrderStatus.Delivered);
            var pending = orders.Where(x => x.Status == OrderStatus.Pending);
            var canceled = orders.Where(x => x.Status == OrderStatus.Canceled);
            return new List<OrderStatisticDTO> {
                new OrderStatisticDTO { Type = OrderStatus.Done.ToString(), Amount = paidOrders.Count() },
                new OrderStatisticDTO { Type = OrderStatus.Delivering.ToString(), Amount = delivering.Count() },
                new OrderStatisticDTO { Type = OrderStatus.Delivered.ToString(), Amount = delivered.Count() },
                new OrderStatisticDTO { Type = OrderStatus.Pending.ToString(), Amount = pending.Count() },
                new OrderStatisticDTO { Type = OrderStatus.Canceled.ToString(), Amount = canceled.Count()},};
        }

        [HttpGet("getmonthlystatistic")]
        [Authorize(Roles = "Admin")]
        [TypeFilter(typeof(CustomAuthorizeFilter))]
        public async Task<MonthlyIncomeStatisticDTO> GetMonthlyIncomeStatistics()
        {
            var orders = _orderRepository.GetMany(x => x.Status != OrderStatus.Ordering && x.OrderDate.Year == DateTime.Now.Year);
            if(orders == null)
            {
                return null;
            }
            var months = new MonthlyIncomeStatisticDTO { MonthlyIncome = new Dictionary<int, decimal> { 
                { 1, 0 },
                { 2, 0 },
                { 3, 0 },
                { 4, 0 },
                { 5, 0 },
                { 6, 0 },
                { 7, 0 },
                { 8, 0 },
                { 9, 0 },
                { 10, 0 },
                { 11, 0 },
                { 12, 0},} 
            };
            foreach (var item in orders)
            {
                var monthIncome = months.MonthlyIncome.FirstOrDefault(x => x.Key == item.OrderDate.Month).Value;
                foreach (var orderDetail in item.OrderDetails)
                {
                    monthIncome += orderDetail.Price * orderDetail.Quantity;
                }
                months.MonthlyIncome[item.OrderDate.Month] = monthIncome;
            }
            return months;
        }

        [HttpPost("cart")]
        [Authorize]
        [TypeFilter(typeof(CustomAuthorizeFilter))]
        public async Task<ListOrderDetailsDTO> GetCart([FromBody]IdString userId)
        {
            var order = await _orderRepository.GetByAsync(o => o.Status == OrderStatus.Ordering && o.UserId == userId.Id);

            if(order != null)
            {
                var mappedOrder = _mapper.Map<ListOrderDetailsDTO>(order);
                return mappedOrder;
            }
            else
            {
                return new ListOrderDetailsDTO();
            }
        }

        [HttpPatch("confirm")]
        [Authorize(Roles = "Admin")]
        [TypeFilter(typeof(CustomAuthorizeFilter))]
        public async Task<ActionResult> Confirm([FromBody] IdString id)
        {
            var order = await _orderRepository.GetByAsync(o => o.Id == int.Parse(id.Id) && o.Status == OrderStatus.Pending);
            if (order == null)
            {
                return BadRequest(new { message = "Can't find this order" });
            }
            else
            {
                order.Status = OrderStatus.Delivering;
                await _orderRepository.UpdateAsync(order);
            }
            await _unitOfWork.CommitAsync();
            return Ok();
        }

        [HttpPut("cancel")]
        [Authorize]
        [TypeFilter(typeof(CustomAuthorizeFilter))]
        public async Task<ActionResult> CancelOrder([FromBody] IdString id)
        {
            var order = await _orderRepository.GetByAsync(o => o.Id == int.Parse(id.Id) && o.Status == OrderStatus.Pending);

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

                    try
                    {
                        await _productRepository.UpdateAsync(prod);
                    }
                    catch (TaskCanceledException e)
                    {

                        return BadRequest(new {message = e.Message});
                    }
                }
                await _unitOfWork.CommitAsync();
                return Ok();
            }
        }

        [HttpPost("canceled")]
        [Authorize]
        [TypeFilter(typeof(CustomAuthorizeFilter))]
        public async Task<List<ListOrderDetailsDTO>> GetCanceledOrder([FromBody] IdString userId)
        {
            var order = _orderRepository.GetMany(o => o.Status == OrderStatus.Canceled && o.UserId == userId.Id)?.Include(x => x.OrderDetails).ThenInclude(x => x.Product).ToList();

            if (order != null)
            {
                var mappedOrder = _mapper.Map<List<ListOrderDetailsDTO>>(order);
                return mappedOrder;
            }
            else
            {
                return new List<ListOrderDetailsDTO>();
            }
        }

        [HttpPost("pending")]
        [Authorize]
        [TypeFilter(typeof(CustomAuthorizeFilter))]
        public async Task<List<ListOrderDetailsDTO>> GetPendingOrder([FromBody] IdString userId)
        {
            var order = _orderRepository.GetMany(o => o.Status == OrderStatus.Pending && o.UserId == userId.Id)?.Include(x => x.OrderDetails).ThenInclude(x => x.Product).ToList();

            if (order != null)
            {
                var mappedOrder = _mapper.Map<List<ListOrderDetailsDTO>>(order);
                return mappedOrder;
            }
            else
            {
                return new List<ListOrderDetailsDTO>();
            }
        }

        [HttpPost("done")]
        [Authorize]
        [TypeFilter(typeof(CustomAuthorizeFilter))]
        public async Task<List<ListOrderDetailsDTO>> GetDoneOrder([FromBody] IdString userId)
        {
            var order = _orderRepository.GetMany(o => o.Status == OrderStatus.Done && o.UserId == userId.Id)?.Include(x=>x.OrderDetails).ThenInclude(x=>x.Product).ToList();

            if (order != null)
            {
                var mappedOrder = _mapper.Map<List<ListOrderDetailsDTO>>(order);
                return mappedOrder;
            }
            else
            {
                return new List<ListOrderDetailsDTO>();
            }
        }

        [HttpPost("delivering")]
        [Authorize]
        [TypeFilter(typeof(CustomAuthorizeFilter))]
        public async Task<List<ListOrderDetailsDTO>> GetDeliveringOrder([FromBody] IdString userId)
        {
            var order = _orderRepository.GetMany(o => o.Status == OrderStatus.Delivering && o.UserId == userId.Id)?.Include(x => x.OrderDetails).ThenInclude(x => x.Product).ToList();

            if (order != null)
            {
                var mappedOrder = _mapper.Map<List<ListOrderDetailsDTO>>(order);
                return mappedOrder;
            }
            else
            {
                return new List<ListOrderDetailsDTO>();
            }
        }

        [HttpPost("checkout")]
        [Authorize(Roles = "Customer")]
        [TypeFilter(typeof(CustomAuthorizeFilter))]
        public async Task<ActionResult> Checkout([FromBody]IdString userId)
        {
            var order = await _orderRepository.GetByAsync(o => o.Status == OrderStatus.Ordering && o.UserId == userId.Id);

            if(order == null)
            {
                return BadRequest("Can't find");
            }

            var orderDetails = order.OrderDetails;

            foreach (var orderDetail in orderDetails)
            {
                var prod = await _productRepository.GetByAsync(p => p.Id == orderDetail.ProductId && !p.IsDeleted);
                if(prod is null)
                {
                    return BadRequest(new { message = $"{orderDetail.Product.Name} is not available" });
                }
                if(prod.Quantity < orderDetail.Quantity)
                {
                    return BadRequest(new { message = "This item don't have enough quantity for you!" });
                }
                prod.Quantity -= orderDetail.Quantity;
                if(prod.Quantity == 0)
                {
                    prod.IsDeleted = true;
                }
                try
                {
                    await _productRepository.UpdateAsync(prod);

                }
                catch (Exception e)
                {

                    return BadRequest(new { message = e.Message });
                }            
            }

            order.Status = OrderStatus.Pending;
            order.OrderDate = DateTime.UtcNow;
            await _orderRepository.UpdateAsync(order);
            await _unitOfWork.CommitAsync();
            return Ok();
        }

        [HttpPost("create")]
        [Authorize(Roles = "Customer")]
        [TypeFilter(typeof(CustomAuthorizeFilter))]
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
