using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BusinessObjects.Models;
using DTO.Models;
using DTO.Models.Category;
using DAO.Interfaces;
using AutoMapper;

namespace NashStoreAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly IMapper _mapper;

        public CategoriesController(ICategoryRepository categoryRepository, IMapper mapper)
        {
            _mapper = mapper;
            _categoryRepository = categoryRepository;
        }

        // GET: api/Categories
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CategoryDTO>>> GetCategories()
        {
            var cateResponse = await _categoryRepository.GetAll().ToListAsync();
            var categories = _mapper.Map<List<CategoryDTO>>(cateResponse);
            
            return categories;
        }

        //// GET: api/Categories/5
        //[HttpGet("{id}")]
        //public async Task<ActionResult<Category>> GetCategory(int id)
        //{
        //    var category = await _categoryRepository.Categories.FindAsync(id);

        //    if (category == null)
        //    {
        //        return NotFound();
        //    }

        //    return category;
        //}

        //// PUT: api/Categories/5
        //// To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        //[HttpPut("{id}")]
        //public async Task<IActionResult> PutCategory(int id, Category category)
        //{
        //    if (id != category.Id)
        //    {
        //        return BadRequest();
        //    }

        //    _categoryRepository.Entry(category).State = EntityState.Modified;

        //    try
        //    {
        //        await _categoryRepository.SaveChangesAsync();
        //    }
        //    catch (DbUpdateConcurrencyException)
        //    {
        //        if (!CategoryExists(id))
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

        //// POST: api/Categories
        //// To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        //[HttpPost]
        //public async Task<ActionResult<Category>> PostCategory(Category category)
        //{
        //    _categoryRepository.Categories.Add(category);
        //    await _categoryRepository.SaveChangesAsync();

        //    return CreatedAtAction("GetCategory", new { id = category.Id }, category);
        //}

        //// DELETE: api/Categories/5
        //[HttpDelete("{id}")]
        //public async Task<IActionResult> DeleteCategory(int id)
        //{
        //    var category = await _categoryRepository.Categories.FindAsync(id);
        //    if (category == null)
        //    {
        //        return NotFound();
        //    }

        //    _categoryRepository.Categories.Remove(category);
        //    await _categoryRepository.SaveChangesAsync();

        //    return NoContent();
        //}

        //private bool CategoryExists(int id)
        //{
        //    return _categoryRepository.Categories.Any(e => e.Id == id);
        //}
    }
}
