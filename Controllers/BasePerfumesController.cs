using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using perfume.Data;
using perfume.Models;

namespace perfume.Controllers
{
    public class BasePerfumesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public BasePerfumesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: BasePerfumes
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.BasePerfume.Include(b => b.User);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: BasePerfumes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.BasePerfume == null)
            {
                return NotFound();
            }

            var basePerfume = await _context.BasePerfume
                .Include(b => b.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (basePerfume == null)
            {
                return NotFound();
            }

            return View(basePerfume);
        }

        // GET: BasePerfumes/Create
        public IActionResult Create()
        {
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id");
            return View();
        }

        // POST: BasePerfumes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,UserId,Size,SelectedBases")] BasePerfume basePerfume)
        {
            if (ModelState.IsValid)
            {
                _context.Add(basePerfume);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", basePerfume.UserId);
            return View(basePerfume);
        }

        // GET: BasePerfumes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.BasePerfume == null)
            {
                return NotFound();
            }

            var basePerfume = await _context.BasePerfume.FindAsync(id);
            if (basePerfume == null)
            {
                return NotFound();
            }
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", basePerfume.UserId);
            return View(basePerfume);
        }

        // POST: BasePerfumes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,UserId,Size,SelectedBases")] BasePerfume basePerfume)
        {
            if (id != basePerfume.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(basePerfume);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BasePerfumeExists(basePerfume.Id))
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
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", basePerfume.UserId);
            return View(basePerfume);
        }

        // GET: BasePerfumes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.BasePerfume == null)
            {
                return NotFound();
            }

            var basePerfume = await _context.BasePerfume
                .Include(b => b.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (basePerfume == null)
            {
                return NotFound();
            }

            return View(basePerfume);
        }

        // POST: BasePerfumes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.BasePerfume == null)
            {
                return Problem("Entity set 'ApplicationDbContext.BasePerfume'  is null.");
            }
            var basePerfume = await _context.BasePerfume.FindAsync(id);
            if (basePerfume != null)
            {
                _context.BasePerfume.Remove(basePerfume);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BasePerfumeExists(int id)
        {
          return (_context.BasePerfume?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
