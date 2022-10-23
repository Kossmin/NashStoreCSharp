using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NashPhaseOne.DTO.Models;
using NashPhaseOne.DTO.Models.Order;
using NashStoreClient.DataAccess;

namespace NashPhaseOne.Client.Controllers
{
    public class OrdersController : Controller
    {
        private IData _data;

        public OrdersController(IData data)
        {
            _data = data;
        }

        public async Task<ActionResult> Cart()
        {
            var userId = User.Claims.FirstOrDefault(u => u.Type == "userid").Value;
            var token = User.Claims.FirstOrDefault(u => u.Type == "token").Value;
            var cartDto = await _data.GetCartAsync(new IdString { Id = userId}, token);
            if(cartDto.OrderDetails.Count() == 0)
            {
                ViewData["cartDto"] = null;
            }
            else
            {
                ViewData["cartDto"] = cartDto;
            }
            return View(cartDto);
        }

        public async Task<ActionResult> CancelOrder(int id)
        {
            var token = User.Claims.FirstOrDefault(u => u.Type == "token").Value;

            try
            {
                await _data.CancelOrderAsync(new IdString { Id = id.ToString() }, token);
            }
            catch (Refit.ApiException)
            {

            }

            return RedirectToAction("PaidOrders");
        }

        public async Task<ActionResult> CanceledOrders()
        {
            var userId = User.Claims.FirstOrDefault(u => u.Type == "userid").Value;
            var token = User.Claims.FirstOrDefault(u => u.Type == "token").Value;
            var orders = await _data.GetCanceledOrdersAsync(new IdString { Id = userId}, token);
            if (orders.Count() == 0)
            {
                ViewData["cartDto"] = null;
            }
            else
            {
                ViewData["cartDto"] = orders;
            }
            return View(orders);
        }

        public async Task<ActionResult> PaidOrders()
        {
            var token = User.Claims.FirstOrDefault(u => u.Type == "token").Value;
            var userId = User.Claims.FirstOrDefault(u => u.Type == "userid").Value;
            var orders = await _data.GetPaidOrdersAsync(new IdString { Id = userId }, token);
            if (orders.Count() == 0)
            {
                ViewData["cartDto"] = null;
            }
            else
            {
                ViewData["cartDto"] = orders;
            }
            return View(orders);
        }

        public async Task<ActionResult> DeliveringOrders()
        {
            var userId = User.Claims.FirstOrDefault(u => u.Type == "userid").Value;
            var token = User.Claims.FirstOrDefault(u => u.Type == "token").Value;
            var orders = await _data.GetDeliveringOrdersAsync(new IdString { Id = userId }, token);
            if (orders.Count() == 0)
            {
                ViewData["cartDto"] = null;
            }
            else
            {
                ViewData["cartDto"] = orders;
            }
            return View(orders);
        }

        [Authorize]
        public async Task<ActionResult> Order([Bind("UserId, ProductId, Quantity, UnitPrice")]OrderDTO order)
        {
            var token = User.Claims.FirstOrDefault(u => u.Type == "token").Value;
            await _data.CreateOrderAsync(order, token);
            TempData["Message"] = "Add to cart success";
            return RedirectToAction("Index", "Products");
        }

        public async Task<ActionResult> Checkout()
        {
            var userId = User.Claims.FirstOrDefault(u => u.Type == "userid").Value;
            try
            {
                await _data.CheckoutAsync(new IdString { Id = userId });
            }
            catch (Refit.ApiException e)
            {
                var errorList = await e.GetContentAsAsync<Dictionary<string, string>>();
                
                TempData["Error"] = errorList?.First().Value;
                
            }
            return RedirectToAction("Index", "Products");
        }

        [HttpPost]
        public async Task<ActionResult> Edit(string OrderDetailId, int Quantity)
        {
            try
            {
                var newOrderDetail = new OrderDetailDTO { Id = int.Parse(OrderDetailId), Quantity = Quantity, Price = 0, Product = new DTO.Models.Product.ProductDetailDTO() };
                await _data.UpdateOrderDetailAsync(newOrderDetail);
            }catch(Refit.ApiException e)
            {
                var errorList = await e.GetContentAsAsync<Dictionary<string, string>>();
                TempData["Error"] = errorList?.First().Value; ;
            }
            TempData["Message"] = "Edit success";
            return RedirectToAction("Cart");
        }

        public async Task<ActionResult> DeleteOrderDetail(int id)
        {
            await _data.DeleteOrderDetailAsync(id);
            TempData["Message"] = "Remove success";
            return RedirectToAction("Cart");
        }
    }
}
