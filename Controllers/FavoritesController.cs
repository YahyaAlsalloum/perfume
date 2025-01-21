using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using perfume.Data;
using perfume.Data.Migrations;
using perfume.Models;

namespace perfume.Controllers
{
    public class FavoritesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;


        public FavoritesController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Favorites
        public async Task<IActionResult> Index()
        {
              return _context.Favorite != null ? 
                          View(await _context.Favorite.ToListAsync()) :
                          Problem("Entity set 'ApplicationDbContext.Favorite'  is null.");
        }


        public async Task<IActionResult> payNaw(string Name)
        {
            // Get the current user
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return Unauthorized("You must be logged in to add items to your cart.");

            // Check if the product exists by name
            var product = await _context.Products.FirstOrDefaultAsync(p => p.Name == Name);
            if (product == null)
                return NotFound("Product not found.");

            // Check if a cart order already exists for the user
            var cartOrder = await _context.Order
                .FirstOrDefaultAsync(o => o.UserId == user.Id && o.Status == "Cart");

            // If no cart exists, create a new one
            if (cartOrder == null)
            {
                cartOrder = new Order
                {
                    UserId = user.Id,
                    Status = "Cart",
                    OrderDate = DateTime.Now
                };

                _context.Order.Add(cartOrder);
                await _context.SaveChangesAsync();
            }

            // Check if the product is already in the cart
            var existingOrderProduct = await _context.OrderProducts
                .FirstOrDefaultAsync(op => op.OrderId == cartOrder.Id && op.ProductId == product.Id);

            if (existingOrderProduct == null)
            {
                // Add the product to the OrderProducts table
                var orderProduct = new OrderProduct
                {
                    ProductId = product.Id,
                    OrderId = cartOrder.Id,
                    Quantity = 1 // Default quantity as 1
                };

                _context.OrderProducts.Add(orderProduct);
                var favorite = await _context.Favorite.FirstOrDefaultAsync(p => p.Name == Name);
                if (favorite != null)
                {
                    _context.Favorite.Remove(favorite);
                }
                await _context.SaveChangesAsync();
            }
            else
            {
                // If the product is already in the cart, update its quantity
                existingOrderProduct.Quantity += 1;
                _context.OrderProducts.Update(existingOrderProduct);
                await _context.SaveChangesAsync();
            }

            // Redirect to the Products Index page
            return RedirectToAction("Index", "Products");
        }


        // GET: Favorites/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Favorite == null)
            {
                return NotFound();
            }

            var favorite = await _context.Favorite
                .FirstOrDefaultAsync(m => m.Id == id);
            if (favorite == null)
            {
                return NotFound();
            }

            return View(favorite);
        }

        // GET: Favorites/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Favorites/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Image")] Favorite favorite)
        {
            if (ModelState.IsValid)
            {
                _context.Add(favorite);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(favorite);
        }

        // GET: Favorites/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Favorite == null)
            {
                return NotFound();
            }

            var favorite = await _context.Favorite.FindAsync(id);
            if (favorite == null)
            {
                return NotFound();
            }
            return View(favorite);
        }

        // POST: Favorites/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Favorite favorite)
        {
            if (id != favorite.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(favorite);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FavoriteExists(favorite.Id))
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
            return View(favorite);
        }

        // GET: Favorites/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Favorite == null)
            {
                return NotFound();
            }

            var favorite = await _context.Favorite
                .FirstOrDefaultAsync(m => m.Id == id);
            if (favorite == null)
            {
                return NotFound();
            }

            return View(favorite);
        }

        // POST: Favorites/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Favorite == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Favorite'  is null.");
            }
            var favorite = await _context.Favorite.FindAsync(id);
            if (favorite != null)
            {
                _context.Favorite.Remove(favorite);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool FavoriteExists(int id)
        {
          return (_context.Favorite?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
