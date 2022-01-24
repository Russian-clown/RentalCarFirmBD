using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CarRental.Models;

namespace CarRental.Controllers
{
    public class TrailersController : Controller
    {
        private readonly AppDbContext _context;

        public TrailersController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Trailers
        public async Task<IActionResult> Index()
        {
            var appDbContext = _context.Trailers.Include(t => t.Colors);
            return View(await appDbContext.ToListAsync());
        }

        // GET: Trailers/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var trailer = await _context.Trailers
                .Include(t => t.Colors)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (trailer == null)
            {
                return NotFound();
            }

            return View(trailer);
        }

        // GET: Trailers/Create
        public IActionResult Create()
        {
            ViewData["Colors"] = _context.Colors.ToList();
            return View();
        }

        // POST: Trailers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,LicensePlate,Carrying,Price")] Trailer trailer, int[] colors)
        {
            if (ModelState.IsValid && colors.Length > 0)
            {
                foreach (var c in _context.Colors.Where(c => colors.Contains(c.Id)))
                    trailer.Colors.Add(c);
                _context.Add(trailer);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["Colors"] = _context.Colors.ToList();
            return View(trailer);
        }

        // GET: Trailers/Edit/5
        public IActionResult Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var trailer = _context.Trailers.Include(t=>t.Colors).FirstOrDefault(t=>t.Id == id);
            if (trailer == null)
            {
                return NotFound();
            }
            ViewData["Colors"] = _context.Colors.ToList();
            return View(trailer);
        }

        // POST: Trailers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,LicensePlate,Carrying")] Trailer trailer, int[] colors)
        {
            if (id != trailer.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    Trailer tr = _context.Trailers.Include(t => t.Colors).FirstOrDefault(t => t.Id == id);
                    tr.Colors.Clear();
                    foreach (var c in _context.Colors.Where(c => colors.Contains(c.Id)))
                        tr.Colors.Add(c);
                    _context.Update(tr);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TrailerExists(trailer.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["Colors"] = _context.Colors.ToList();
            return View(trailer);
        }

        // GET: Trailers/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var trailer = await _context.Trailers
                .Include(t => t.Colors)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (trailer == null)
            {
                return NotFound();
            }

            return View(trailer);
        }

        // POST: Trailers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var trailer = await _context.Trailers.FindAsync(id);
            _context.Trailers.Remove(trailer);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TrailerExists(int id)
        {
            return _context.Trailers.Any(e => e.Id == id);
        }
    }
}
