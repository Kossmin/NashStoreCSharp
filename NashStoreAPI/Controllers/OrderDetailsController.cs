using DAO.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NashPhaseOne.DAO.Interfaces;
using NashPhaseOne.DTO.Models.Order;
using NashPhaseOne.BusinessObjects.Models;
using Microsoft.AspNetCore.Authorization;
using NashPhaseOne.API.Filters;

namespace NashPhaseOne.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [TypeFilter(typeof(CustomAuthorizeFilter))]
    public class OrderDetailsController : ControllerBase
    {
        private readonly IOrderDetailRepository _orderDetailRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IProductRepository _productRepository;
        private readonly IOrderRepository _orderRepository;

        public OrderDetailsController(IOrderRepository orderRepository, IProductRepository productRepository, IUnitOfWork unitOfWork, IOrderDetailRepository orderDetailRepository)
        {
            _orderRepository = orderRepository;
            _productRepository = productRepository;
            _unitOfWork = unitOfWork;
            _orderDetailRepository = orderDetailRepository;
        }

        [HttpPatch("update")]
        [Authorize]
        public async Task<ActionResult> Update([FromBody]OrderDetailDTO orderDetail)
        {
            var currentOrderDetail  = await _orderDetailRepository.GetByAsync(or => or.Id == orderDetail.Id);
            var currentProduct = await _productRepository.GetByAsync(or => or.Id == currentOrderDetail.ProductId);
            if(currentOrderDetail == null)
            {
                return BadRequest(new { message = "You are trying to update an invalid order" });
            }else if(currentProduct.Quantity < orderDetail.Quantity)
            {
                return BadRequest(new { message = "Your input is larger than the quantity of the product" });
            }
            else
            {
                currentOrderDetail.Quantity = orderDetail.Quantity;
                await _orderDetailRepository.UpdateAsync(currentOrderDetail);
            }
            await _unitOfWork.CommitAsync();
            return Ok();
        }

        [HttpDelete]
        [Authorize]
        public async Task<ActionResult> Delete(int id)
        {
            var orderDetail = await _orderDetailRepository.GetByAsync(o => o.Id == id);
            if(orderDetail == null)
            {
                return BadRequest(new { message = "Cannot find your order detail" });
            }
            var order = await _orderRepository.GetByAsync(o => o.Id == orderDetail.OrderId);
            
            if(order.Status != NashPhaseOne.BusinessObjects.Models.OrderStatus.Ordering)
            {
                return BadRequest(new { message = "This order is already paid can't be deleted" });
            }
            else
            {
                await _orderDetailRepository.DeleteAsync(orderDetail);
            }
            await _unitOfWork.CommitAsync();
            return Ok();
        }
    }
}
