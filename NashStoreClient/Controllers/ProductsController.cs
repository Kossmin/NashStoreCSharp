using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BusinessObjects.Models;
using NashStoreClient.DataAccess;

namespace NashStoreClient.Controllers
{
    public class ProductsController : Controller
    {
        private IProductData _productData;
        public ProductsController(IProductData productData)
        {
            _productData = productData;
        }

        // GET: Products
        public async Task<IActionResult> Index(int pageIndex = 1)
        {
            var productsList = await _productData.GetProducts(pageIndex);
            return View(productsList);
        }

        // GET: Products/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _productData.GetProductById(id.Value);

            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }
    }
}
