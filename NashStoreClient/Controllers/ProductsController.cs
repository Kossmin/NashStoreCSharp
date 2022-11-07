using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BusinessObjects.Models;
using NashStoreClient.DataAccess;
using DTO.Models;
using Microsoft.AspNetCore.Authorization;
using NashPhaseOne.DTO.Models.Product;
using System.Security.Claims;
using Refit;

namespace NashStoreClient.Controllers
{
    //[AllowAnonymous]
    public class ProductsController : Controller
    {
        private IData _data;
        public ProductsController(IData data)
        {
            _data = data;
        }

        // GET: Products
        public async Task<IActionResult> Index([FromQuery]int pageIndex = 1)
        {
            var productsList = await _data.GetProductsAsync(pageIndex);
            var categoryList = await _data.GetCategoriesAsync();
            ViewData["CategoryId"] = new SelectList(categoryList, "Id", "Name");
            return View(productsList);
        }

        public async Task<IActionResult> Search(string searchName, string searchType,[FromQuery] int pageIndex = 1)
        {
            var productsList = new ViewListDTO<ProductDTO>();
            try
            {
                if (string.IsNullOrEmpty(searchName) && string.IsNullOrEmpty(searchType))
                {
                    return RedirectToAction("Index", new { pageIndex = 1 });
                }
                else if (string.IsNullOrEmpty(searchName))
                {
                    productsList = await _data.SearchingAsync(new RequestSearchProductDTO { CategoryId = int.Parse(searchType), ProductName = "", PageIndex = pageIndex });
                }
                else if (string.IsNullOrEmpty(searchType))
                {
                    productsList = await _data.SearchingAsync(new RequestSearchProductDTO { CategoryId = 0, ProductName = searchName, PageIndex = pageIndex });
                }
                else
                {
                    productsList = await _data.SearchingAsync(new RequestSearchProductDTO { CategoryId = int.Parse(searchType), ProductName = searchName, PageIndex = pageIndex });
                }
            }
            catch (ApiException e)
            {
                var errorList = await e.GetContentAsAsync<Dictionary<string, string>>();
                TempData["Error"] = errorList.First().Value;
                return RedirectToAction("Index", "Products", new { pageIndex = 1 });
            }

            var categoryList = await _data.GetCategoriesAsync();
            ViewData["CategoryId"] = new SelectList(categoryList, "Id", "Name", searchType);
            ViewData["SearchName"] = searchName;
            ViewData["SearchType"] = searchType;
            return View(productsList);
        }

        // GET: Products/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            try
            {
                var product = await _data.GetProductByIdAsync(id.Value);
                var cateName = product.CategoryName;
                if (product == null)
                {
                    return NotFound();
                }

                if (User.Identity.IsAuthenticated)
                {
                    var userId = User.Claims.FirstOrDefault(u => u.Type == "userid").Value;
                    ViewData["userid"] = userId;
                }
                else
                {
                    ViewData["userid"] = null;
                }

                var ratingList = await _data.GetRatingAsync(id.Value);
                double avgRating;
                if (ratingList.Count() == 0)
                {
                    avgRating = 0;
                }
                else
                {
                    avgRating = ratingList.Average(x => (int)x.Star);
                }
                ViewData["ratingList"] = ratingList;
                ViewData["avgRating"] =Math.Round(avgRating,1);
                return View(product);
            }
            catch (ApiException e)
            {
                TempData["Error"] = e.Content;
                return RedirectToAction("Index");
            }
        }
    }
}
