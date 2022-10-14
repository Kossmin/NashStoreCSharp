using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BusinessObjects.Models;
using DAO.Interfaces;
using DTO.Models;

namespace NashStoreAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IProductRepository _context;

        public ProductsController(IProductRepository context)
        {
            _context = context;
        }

        // GET: api/Products
        [HttpGet]
        public async Task<ActionResult<ViewListModel<ViewProductModel>>> GetAllProducts([FromQuery] int pageIndex)
        {
            try
            {
                var productsData = await _context.PagingAsync(_context.GetAll(),pageIndex);

                List<ViewProductModel> products = new List<ViewProductModel>();
                foreach (var item in productsData.ModelDatas)
                {
                    var categoryName = item.Category.Name;
                    item.Category = null;
                    var product = item;
                    products.Add(new ViewProductModel
                    {
                        CategoryName = categoryName,
                        Product = product,
                    });
                }
                return new ViewListModel<ViewProductModel> { ModelDatas = products, MaxPage = 0, PageIndex = pageIndex};
            }
            catch (IndexOutOfRangeException ex)
            {
                return BadRequest("Can't find this page");
            }
        }

        [HttpGet("available")]
        public async Task<ActionResult<ViewListModel<ViewProductModel>>> GetAvailableProducts([FromQuery] int pageIndex)
        {
            try
            {
                var productsData = await _context.PagingAsync(_context.GetMany(p => p.IsDeleted == false), pageIndex);
                List<ViewProductModel> products = new List<ViewProductModel>();
                foreach (var item in productsData.ModelDatas)
                {
                    var categoryName = item.Category.Name;
                    item.Category = null;
                    var product = item;
                    products.Add(new ViewProductModel
                    {
                        CategoryName = categoryName,
                        Product = product,
                    });
                }
                return new ViewListModel<ViewProductModel> { ModelDatas = products, PageIndex = productsData.PageIndex, MaxPage = productsData.MaxPage};
            }
            catch (IndexOutOfRangeException ex)
            {
                return BadRequest("Can't find this page");
            }
        }

        [HttpGet("unavailable")]
        public async Task<ActionResult<ViewListModel<ViewProductModel>>> GetUnAvailableProducts([FromQuery] int pageIndex)
        {
            try
            {
                var productsData = await _context.PagingAsync(_context.GetMany(p => p.IsDeleted == true), pageIndex);
                List<ViewProductModel> products = new List<ViewProductModel>();
                foreach (var item in productsData.ModelDatas)
                {
                    var categoryName = item.Category.Name;
                    item.Category = null;
                    var product = item;
                    products.Add(new ViewProductModel
                    {
                        CategoryName = categoryName,
                        Product = product,
                    });
                }
                return new ViewListModel<ViewProductModel> { ModelDatas = products, MaxPage = productsData.MaxPage, PageIndex = productsData.PageIndex };
            }
            catch (IndexOutOfRangeException ex)
            {
                return BadRequest("Can't find this page");
            }
        }

        [HttpPost("search")]
        public async Task<ActionResult<ViewListModel<ViewProductModel>>> GetProductByName([FromBody]RequestSearchProductModel requestModel){
            try
            {
                var responseData = new ViewListModel<Product>();
                if (string.IsNullOrEmpty(requestModel.ProductName) && requestModel.CategoryId <= 0)
                {
                    return NotFound();
                }
                else if (string.IsNullOrEmpty(requestModel.ProductName))
                {
                    responseData = await _context.PagingAsync(_context.GetMany(x => x.CategoryID == requestModel.CategoryId), requestModel.PageIndex); 
                }
                else if (requestModel.CategoryId == null)
                {
                    responseData = await _context.PagingAsync(_context.GetMany(x => x.Name.ToUpper().Contains(requestModel.ProductName.ToUpper())), requestModel.PageIndex);
                }
                else
                {
                    responseData = await _context.PagingAsync(_context.GetMany(x => x.Name.ToUpper().Contains(requestModel.ProductName.ToUpper()) && x.CategoryID == requestModel.CategoryId), requestModel.PageIndex);
                }


                List<ViewProductModel> products = new List<ViewProductModel>();
                foreach (var item in responseData.ModelDatas)
                {
                    var categoryName = item.Category.Name;
                    item.Category = null;
                    var product = item;
                    products.Add(new ViewProductModel
                    {
                        CategoryName = categoryName,
                        Product = product,
                    });
                }
                return new ViewListModel<ViewProductModel> { ModelDatas = products, MaxPage = responseData.MaxPage, PageIndex = responseData.PageIndex };
            }
            catch (IndexOutOfRangeException ex)
            {
                return BadRequest("Can't find this page");
            }   
        }

        // GET: api/Products/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Product>> GetProduct([FromRoute]int id)
        {
            var product = await _context.GetByAsync(p => p.ID == id);

            if (product == null)
            {
                return NotFound();
            }

            return product;
        }

        //// PUT: api/Products/5
        //// To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        //[HttpPut("{id}")]
        //public async Task<IActionResult> PutProduct(int id, Product product)
        //{
        //    if (id != product.ID)
        //    {
        //        return BadRequest();
        //    }

        //    _context.Entry(product).State = EntityState.Modified;

        //    try
        //    {
        //        await _context.SaveChangesAsync();
        //    }
        //    catch (DbUpdateConcurrencyException)
        //    {
        //        if (!ProductExists(id))
        //        {
        //            return NotFound();
        //        }
        //        else
        //        {
        //            throw;
        //        }
        //    }

        //    return NoContent();
        //}

        //// POST: api/Products
        //// To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        //[HttpPost]
        //public async Task<ActionResult<Product>> PostProduct(Product product)
        //{
        //    _context.Products.Add(product);
        //    await _context.SaveChangesAsync();

        //    return CreatedAtAction("GetProduct", new { id = product.ID }, product);
        //}

        //// DELETE: api/Products/5
        //[HttpDelete("{id}")]
        //public async Task<IActionResult> DeleteProduct(int id)
        //{
        //    var product = await _context.Products.FindAsync(id);
        //    if (product == null)
        //    {
        //        return NotFound();
        //    }

        //    _context.Products.Remove(product);
        //    await _context.SaveChangesAsync();

        //    return NoContent();
        //}

        //private bool ProductExists(int id)
        //{
        //    return _context.Products.Any(e => e.ID == id);
        //}
    }
}
