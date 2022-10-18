using Microsoft.AspNetCore.Mvc;
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

        public IActionResult Index()
        {
            return View();
        }

        public async Task<ActionResult> Order([Bind("UserId, ProductId, Quantity")]OrderDTO order)
        {
            _data.CreateOrder(order);
            return View();
        }
    }
}
