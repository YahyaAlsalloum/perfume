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
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;

namespace perfume.Controllers
{
    public class OrdersController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;


        public OrdersController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;

        }


        //this method update the quantity of product in the order "cart"
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditQuantity(int id, int quantity)
        {
            var orderProduct = _context.OrderProducts.Find(id);
            if (orderProduct != null)
            {
                orderProduct.Quantity = quantity;
                await _context.SaveChangesAsync();
                return RedirectToAction("Create");
            }
            return RedirectToAction("Create");
        }


        // GET: Orders
        public async Task<IActionResult> Index()
        {
            // Include the User and related OrderProducts with their associated Product
            var orders = await _context.Order
                .Include(o => o.User) // Include the User entity
                .Include(o => o.OrderProducts)
                    .ThenInclude(op => op.Product) // Include the Product entity
                .ToListAsync();

            return View(orders);
        }
        // GET: Orders/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Order == null)
            {
                return NotFound();
            }

            var order = await _context.Order
                .Include(o => o.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        // GET: Orders/Create
        [Authorize]
        public async Task<IActionResult> Create()
        {

            var user = await _userManager.GetUserAsync(User);
            var cartOrder = await _context.Order
                .FirstOrDefaultAsync(o => o.UserId == user.Id && o.Status == "Cart");

            if (cartOrder != null)
            {
                var selectedProducts = await _context.OrderProducts
                    .Where(op => op.OrderId == cartOrder.Id)
                    .Include(op => op.Product)
                    .ToListAsync();

                ViewData["cartProducts"] = selectedProducts;
            }

            //get the maked perfume:
            var makedPerfume = await _context.BasePerfume.Include(o => o.User).ToListAsync();

            ViewData["makedPerfume"] = makedPerfume;

            return View();
        }

        // POST: Orders/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Order order, List<OrderProduct> orderProducts, decimal TotalAmount)
        {
            // Get the user   
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Unauthorized("You must be logged in to create an order.");
            }

            order.UserId = user.Id;
            order.OrderDate = DateTime.Now;
            order.Status = "Processing";
            _context.Add(order);
            await _context.SaveChangesAsync();
            foreach (var product in orderProducts)
            {
                product.OrderId = order.Id;

                _context.Add(product);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction("Index", "Home");



            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", order.UserId);
            return View(order);
        }


        // GET: Orders/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Order == null)
            {
                return NotFound();
            }

            var order = await _context.Order.FindAsync(id);
            if (order == null)
            {
                return NotFound();
            }
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", order.UserId);
            return View(order);
        }

        // POST: Orders/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Order order)
        {
            if (id != order.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(order);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OrderExists(order.Id))
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
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", order.UserId);
            return View(order);
        }

        // GET: Orders/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Order == null)
            {
                return NotFound();
            }

            var order = await _context.Order
                .Include(o => o.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        // POST: Orders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Order == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Order'  is null.");
            }
            var order = await _context.Order.FindAsync(id);
            if (order != null)
            {
                _context.Order.Remove(order);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool OrderExists(int id)
        {
            return (_context.Order?.Any(e => e.Id == id)).GetValueOrDefault();
        }




    }
}