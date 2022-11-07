using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NashPhaseOne.BusinessObjects.Models;
using DTO.Models;
using DTO.Models.Category;
using DAO.Interfaces;
using AutoMapper;
using NashPhaseOne.DTO.Models;
using Microsoft.AspNetCore.Authorization;
using NashPhaseOne.API.Filters;

namespace NashStoreAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly IProductRepository _productRepository;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public CategoriesController(ICategoryRepository categoryRepository, IMapper mapper, IUnitOfWork unitOfWork, IProductRepository productRepository)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _productRepository = productRepository;
            _categoryRepository = categoryRepository;
        }

        // GET: api/Categories
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CategoryDTO>>> GetCategories()
        {
            var cateResponse = await _categoryRepository.GetAll().Where(x=> !x.IsDeleted).ToListAsync();
            var categories = _mapper.Map<List<CategoryDTO>>(cateResponse);
            
            return categories;
        }

        [HttpGet("available")]
        [Authorize(Roles = "Admin")]
        [TypeFilter(typeof(CustomAuthorizeFilter))]
        public async Task<ActionResult<IEnumerable<CategoryDTO>>> GetAllCategories()
        {
            var cateResponse = await _categoryRepository.GetAll().ToListAsync();
            var categories = _mapper.Map<List<CategoryDTO>>(cateResponse);

            return categories;
        }

        [HttpPut("toggle")]
        [Authorize(Roles = "Admin")]
        [TypeFilter(typeof(CustomAuthorizeFilter))]
        public async Task<ActionResult> ToggleStatus([FromBody] IdString id)
        {
            if (string.IsNullOrEmpty(id.Id))
            {
                return BadRequest(new { message = "Can't find" });
            }
            var cate = await _categoryRepository.GetByAsync(c => c.Id == int.Parse(id.Id));
            cate.IsDeleted = !cate.IsDeleted;
            if (cate.IsDeleted)
            {
                cate.Products.ForEach(x =>
                {
                    x.IsDeleted = true;
                });
            }
            await _categoryRepository.UpdateAsync(cate);
            await _unitOfWork.CommitAsync();
            return Ok();
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [TypeFilter(typeof(CustomAuthorizeFilter))]
        public async Task<ActionResult> Add([FromBody] CategoryDTO category)
        {
            try
            {
                var cate = _mapper.Map<Category>(category);
                await _categoryRepository.SaveAsync(cate);
                await _unitOfWork.CommitAsync();
                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest(new { message = e.Message });
            }
        }

        [HttpPatch]
        [Authorize(Roles = "Admin")]
        [TypeFilter(typeof(CustomAuthorizeFilter))]
        public async Task<ActionResult> Update([FromBody] CategoryDTO category)
        {
            try
            {
                var cate = _mapper.Map<Category>(category);
                await _categoryRepository.UpdateAsync(cate);
                await _unitOfWork.CommitAsync();
                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest(new { message = e.Message });
            }
        }
    }
}
