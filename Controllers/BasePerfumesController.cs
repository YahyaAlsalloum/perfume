using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
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
        private readonly UserManager<ApplicationUser> _userManager;

        public BasePerfumesController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: BasePerfumes

        public async Task<IActionResult> Index()
        {
            var roses = new[]
            {
        new { Id = 2, Name = "Rose", img = "/images/base/rose.jpg" },
        new { Id = 3, Name = "Lavnder", img = "/images/base/lavender.jpg"},
        new { Id = 4, Name = "Orange Blossom", img = "/images/base/orange-blossom.jpg" },
        new { Id = 4, Name = "Jasmine", img = "/images/base/jasmine.jpg" },
        new { Id = 4, Name = "Honeysuckle", img = "/images/base/honeysuckle.jpg" },
    };

            var fruits = new[]
            {
        new { Id = 2, Name = "Orange", img = "/images/base/orange.jpg" },
        new { Id = 3, Name = "Lemon", img = "/images/base/lemon.jpg"},
        new { Id = 4, Name = "Lime", img = "/images/base/lime.jpg" },
        new { Id = 4, Name = "Pomegranate", img = "/images/base/pomegranate.jpg" },
        new { Id = 4, Name = "Apple", img = "/images/base/apple.jpg" },
    };

            var woods = new[]
            {
        new { Id = 2, Name = "Frankincense", img = "/images/base/frankincense.jpg" },
        new { Id = 3, Name = "Oud", img = "/images/base/oud.jpg"},
        new { Id = 4, Name = "Sandalwood", img = "/images/base/sandalwood.jpg" },
        new { Id = 4, Name = "Rosewood", img = "/images/base/rosewood.jpg" },
        new { Id = 4, Name = "Nerolin", img = "/images/base/nerolin.jpg" },
    };

            ViewData["Roses"] = roses;
            ViewData["Fruits"] = fruits;
            ViewData["Woods"] = woods;

            return View();
        }

        //Admin
        public async Task<IActionResult> AdminList()
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
/*            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id");
*/            return View();
        }

        // POST: BasePerfumes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(IEnumerable<string> selectedRoses, IEnumerable<string> selectedFruits, IEnumerable<string> selectedWoods, BasePerfume basePerfume)
        {
            // Get the user 
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Unauthorized("You must be logged in to create an order.");
            }
         
                var selectedBases = selectedRoses.Concat(selectedFruits).Concat(selectedWoods);
                basePerfume.SelectedBases = string.Join(",", selectedBases);
                basePerfume.UserId = user.Id;
            basePerfume.Status = "Cart";

                try
                {
                    _context.Add(basePerfume);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    // معالجة الاستثناء
                }
            
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
        public async Task<IActionResult> Edit(int id, [Bind("Id,UserId,Size,Quantity,SelectedBases")] BasePerfume basePerfume)
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
            return RedirectToAction("Create", "Orders");
        }

        private bool BasePerfumeExists(int id)
        {
          return (_context.BasePerfume?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
