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
            var cartDto = await _data.GetCartAsync(new UserIdString { Id = userId});
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
                await _data.CheckoutAsync(new UserIdString { Id = userId });
            }
            catch (Refit.ApiException e)
            {
                var errorList = await e.GetContentAsAsync<Dictionary<string, string>>();
                if(errorList != null)
                {
                    TempData["Error"] = errorList?.First().Value;
                }
                else
                {
                    TempData["Message"] = "Checkout successly";
                }
            }
            return RedirectToAction("Index", "Products");
        }
    }
}
