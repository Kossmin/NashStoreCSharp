﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BusinessObjects.Models;
using DAO.Interfaces;
using DTO.Models;
using Microsoft.AspNetCore.Authorization;
using NashPhaseOne.DTO.Models.Product;
using AutoMapper;
using NashPhaseOne.API.BlobHelper;

namespace NashStoreAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IProductRepository _productRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IBlobService _blobService;
        private readonly string blobURL = "https://nashstoreimage.blob.core.windows.net/nashstoreimage/";

        public ProductsController(IMapper mapper, IProductRepository productRepository, IUnitOfWork unitOfWork, IBlobService blobService)
        {
            _blobService = blobService;
            _productRepository = productRepository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        // GET: api/Products
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ViewListDTO<ProductDTO>>> GetAllProducts([FromQuery] int pageIndex)
        {
            try
            {
                var productsData = await _productRepository.PagingAsync(_productRepository.GetAll().OrderBy(x => x.IsDeleted ? 1 : 0), pageIndex);

                var products = _mapper.Map<List<ProductDTO>>(productsData.ModelDatas);

                return Ok(new ViewListDTO<ProductDTO> { ModelDatas = products, MaxPage = productsData.MaxPage, PageIndex = pageIndex});
            }
            catch (IndexOutOfRangeException ex)
            {
                return BadRequest("Can't find this page");
            }
        }

        [HttpGet("available")]
        public async Task<ActionResult<ViewListDTO<ProductDTO>>> GetAvailableProducts([FromQuery] int pageIndex)
        {
            try
            {
                var productsData = await _productRepository.PagingAsync(_productRepository.GetMany(p => p.IsDeleted == false), pageIndex);

                var products = _mapper.Map<List<ProductDTO>>(productsData.ModelDatas);

                return Ok(new ViewListDTO<ProductDTO> { ModelDatas = products, MaxPage = productsData.MaxPage, PageIndex = pageIndex });
            }
            catch (IndexOutOfRangeException ex)
            {
                return BadRequest("Can't find this page");
            }
        }

        //[Authorize(Roles = "Admin")]
        [HttpGet("unavailable")]
        public async Task<ActionResult<ViewListDTO<ProductDTO>>> GetUnAvailableProducts([FromQuery] int pageIndex)
        {
            try
            {
                var productsData = await _productRepository.PagingAsync(_productRepository.GetMany(p => p.IsDeleted == true), pageIndex);
                var products = _mapper.Map<List<ProductDTO>>(productsData.ModelDatas);

                return Ok(new ViewListDTO<ProductDTO> { ModelDatas = products, MaxPage = productsData.MaxPage, PageIndex = pageIndex });
            }
            catch (IndexOutOfRangeException ex)
            {
                return BadRequest("Can't find this page");
            }
        }

        [HttpPost("search")]
        public async Task<ActionResult<ViewListDTO<ProductDTO>>> GetProductByName([FromBody]RequestSearchProductDTO requestModel)
        {
            try
            {
                IQueryable<Product> queries;
                if (string.IsNullOrEmpty(requestModel.ProductName) && requestModel.CategoryId <= 0)
                {
                    return NotFound();
                }
                else if (string.IsNullOrEmpty(requestModel.ProductName))
                {
                    queries = _productRepository.GetMany(x => x.CategoryId == requestModel.CategoryId && !x.IsDeleted); 
                }
                else if (requestModel.CategoryId == 0)
                {
                    queries = _productRepository.GetMany(x => x.Name.ToUpper().Contains(requestModel.ProductName.ToUpper()) && !x.IsDeleted);
                }
                else
                {
                    queries = _productRepository.GetMany(x => (x.Name.ToUpper().Contains(requestModel.ProductName.ToUpper()) || x.CategoryId == requestModel.CategoryId) && !x.IsDeleted);
                }
                var responseData = await _productRepository.PagingAsync(queries, requestModel.PageIndex);
                if(responseData == null)
                {
                    return BadRequest(new { message = "Can't find" });
                }
                var products = _mapper.Map<List<ProductDTO>>(responseData.ModelDatas);

                return Ok(new ViewListDTO<ProductDTO> { ModelDatas = products, MaxPage = responseData.MaxPage, PageIndex = requestModel.PageIndex });
            }
            catch (IndexOutOfRangeException ex)
            {
                return BadRequest("Can't find this page");
            }   
        }

        // GET: api/Products/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ProductDTO>> GetProduct([FromRoute]int id)
        {
            var product = await _productRepository.GetByAsync(p => p.Id == id);

            if (product == null)
            {
                return NotFound();
            }
            var convertData = _mapper.Map<ProductDTO>(product);

            return convertData;
        }

        [HttpPost("add")]
        public async Task<ActionResult> Add([FromForm]AdminProductDTO model)
        {
            var newProduct = _mapper.Map<Product>(model);
            newProduct.ImgUrls = new List<string>();
            await _productRepository.SaveAsync(newProduct);

            foreach (var item in model.Imgs)
            {
                await _blobService.UploadFileBlobAsync(item);
                newProduct.ImgUrls.Add(blobURL + item.FileName);
            }

            await _unitOfWork.CommitAsync();
            return Ok();
        }

        //[HttpGet("Temp")]
        //public async Task<ActionResult> AddImage()
        //{
        //    var productsData = await _context.PagingAsync(_context.GetMany(p => p.IsDeleted == false));
        //    List<TempProductDTO> products = new List<TempProductDTO>();
        //    foreach (var item in productsData.ModelDatas)
        //    {
        //        item.ImgUrls = new List<string>();
        //        item.ImgUrls.Add("https://cdn2.cellphones.com.vn/358x/media/catalog/product/i/p/iphone-11-xanh-la-1_1.jpg");
        //        item.ImgUrls.Add("https://cdn2.cellphones.com.vn/358x/media/catalog/product/4/8/4824327_cover_iphone-11-mint211_1.jpg");
        //        await _context.UpdateAsync(item);
        //    }
        //    await _unitOfWork.CommitAsync();
        //    return Ok();
        //}

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
