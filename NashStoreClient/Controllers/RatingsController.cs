using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using NashStoreClient.DataAccess;
using DTO.Models;
using NashPhaseOne.DTO.Models.Rating;
using Microsoft.AspNetCore.Authorization;

namespace NashStoreClient.Controllers
{
    public class RatingsController : Controller
    {
        private IData _data;

        public RatingsController(IData data)
        {
            _data = data;
        }

        //// GET: Ratings
        //public async Task<IActionResult> Index()
        //{
        //    var nashStoreDbContext = _context.Ratings.Include(r => r.Product).Include(r => r.User);
        //    return View(await nashStoreDbContext.ToListAsync());
        //}

        //// GET: Ratings/Details/5
        //public async Task<IActionResult> Details(string id)
        //{
        //    if (id == null || _context.Ratings == null)
        //    {
        //        return NotFound();
        //    }

        //    var rating = await _context.Ratings
        //        .Include(r => r.Product)
        //        .Include(r => r.User)
        //        .FirstOrDefaultAsync(m => m.UserID == id);
        //    if (rating == null)
        //    {
        //        return NotFound();
        //    }

        //    return View(rating);
        //}

        // POST: Ratings/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CustomerId,ProductId,Star,Comment")] RatingDTO rating)
        {
            if (ModelState.IsValid)
            {
                var token = User.Claims.FirstOrDefault(u => u.Type == "token").Value;
                try
                {
                    var response = await _data.CreateRatingAsync(rating, token);
                }
                catch (Refit.ApiException e)
                { 
                    var errorList = await e.GetContentAsAsync<Dictionary<string, string>>();
                    if(errorList != null)
                    {
                        TempData["Error"] = errorList.First().Value;
                        return RedirectToAction("Details", "Products", new { id = rating.ProductId });
                    }
                    else
                    {
                        TempData["Message"] = "Comment success";
                    }
                }
            }
            return RedirectToAction("Index", "Products", new { pageIndex = 1 });
        }

        //// GET: Ratings/Edit/5
        //public async Task<IActionResult> Edit(string id)
        //{
        //    if (id == null || _context.Ratings == null)
        //    {
        //        return NotFound();
        //    }

        //    var rating = await _context.Ratings.FindAsync(id);
        //    if (rating == null)
        //    {
        //        return NotFound();
        //    }
        //    ViewData["ProductID"] = new SelectList(_context.Products, "ID", "Description", rating.ProductID);
        //    ViewData["UserID"] = new SelectList(_context.Users, "Id", "Id", rating.UserID);
        //    return View(rating);
        //}

        //// POST: Ratings/Edit/5
        //// To protect from overposting attacks, enable the specific properties you want to bind to.
        //// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Edit(string id, [Bind("UserID,ProductID,Star")] Rating rating)
        //{
        //    if (id != rating.UserID)
        //    {
        //        return NotFound();
        //    }

        //    if (ModelState.IsValid)
        //    {
        //        try
        //        {
        //            _context.Update(rating);
        //            await _context.SaveChangesAsync();
        //        }
        //        catch (DbUpdateConcurrencyException)
        //        {
        //            if (!RatingExists(rating.UserID))
        //            {
        //                return NotFound();
        //            }
        //            else
        //            {
        //                throw;
        //            }
        //        }
        //        return RedirectToAction(nameof(Index));
        //    }
        //    ViewData["ProductID"] = new SelectList(_context.Products, "ID", "Description", rating.ProductID);
        //    ViewData["UserID"] = new SelectList(_context.Users, "Id", "Id", rating.UserID);
        //    return View(rating);
        //}

        //// GET: Ratings/Delete/5
        //public async Task<IActionResult> Delete(string id)
        //{
        //    if (id == null || _context.Ratings == null)
        //    {
        //        return NotFound();
        //    }

        //    var rating = await _context.Ratings
        //        .Include(r => r.Product)
        //        .Include(r => r.User)
        //        .FirstOrDefaultAsync(m => m.UserID == id);
        //    if (rating == null)
        //    {
        //        return NotFound();
        //    }

        //    return View(rating);
        //}

        //// POST: Ratings/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> DeleteConfirmed(string id)
        //{
        //    if (_context.Ratings == null)
        //    {
        //        return Problem("Entity set 'NashStoreDbContext.Ratings'  is null.");
        //    }
        //    var rating = await _context.Ratings.FindAsync(id);
        //    if (rating != null)
        //    {
        //        _context.Ratings.Remove(rating);
        //    }

        //    await _context.SaveChangesAsync();
        //    return RedirectToAction(nameof(Index));
        //}

        //private bool RatingExists(string id)
        //{
        //  return _context.Ratings.Any(e => e.UserID == id);
        //}
    }
}
