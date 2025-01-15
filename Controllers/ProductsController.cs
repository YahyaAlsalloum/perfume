using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using perfume.Data;
using perfume.Models;

namespace perfume.Controllers
{
    public class ProductsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHost;
        private readonly UserManager<ApplicationUser> _userManager;

        public ProductsController(ApplicationDbContext context, IWebHostEnvironment webHost, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _webHost = webHost;
            _userManager = userManager;
        }

        // GET: Products
        public async Task<IActionResult> Index()
        {
            return _context.Product != null ?
                          View(await _context.Product.ToListAsync()) :
                          Problem("Entity set 'ApplicationDbContext.Product'  is null.");


        }

        // GET : search product 
        public async Task<IActionResult> Search(string searchPhrase)
        {
            return _context.Product != null ?
                        View("Index", await _context.Product.Where(j => EF.Functions.Like(j.Name, $"%{searchPhrase}%")).ToListAsync()) :
                        Problem("Entity set 'ApplicationDbContext.Product'  is null.");

        }
        //GET : Products in admin page
        public async Task<IActionResult> IndexAdmin()
        {
            return _context.Product != null ?
                        View(await _context.Product.ToListAsync()) :
                        Problem("Entity set 'ApplicationDbContext.Product'  is null.");

            if (_context.Product != null)
            {
                View(await _context.Product.ToListAsync());
            }
            else
            {
                Problem("Entity set 'ApplicationDbContext.Product'  is null.");
            }
        }

        public async Task<IActionResult> AddToFavorite( string imageUrl, string name)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized("You must be logged in to add items to your cart.");

            var favorite = new Favorite
            {
                Name = name,
                Image = imageUrl,
            };
            _context.Add(favorite);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
            [Authorize]
        public async Task<IActionResult> AddToCart(int productId)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized("You must be logged in to add items to your cart.");

            // Check if product exists
            var product = await _context.Products.FindAsync(productId);
            if (product == null) return NotFound("Product not found.");

            // Check if a temporary cart order already exists
            var cartOrder = await _context.Order
                .FirstOrDefaultAsync(o => o.UserId == user.Id && o.Status == "Cart");

            // If not, create a new cart order
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

            // Add product to OrderProduct table with the temporary order
            var orderProduct = new OrderProduct
            {
                ProductId = productId,
                OrderId = cartOrder.Id
            };

            _context.OrderProducts.Add(orderProduct);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index", "Products");
        }


        // GET: Products/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Product == null)
            {
                return NotFound();
            }

            var product = await _context.Product
                .FirstOrDefaultAsync(m => m.Id == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // GET: Products/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Products/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create( Product product , IFormFile imageFile)
        {
            string uploadFolder = Path.Combine(_webHost.WebRootPath, "images");
            if (imageFile == null || imageFile.Length == 0)
            {
                // Handle the case where no file is uploaded
                ViewBag.Message = "Please select a file for the image.";
                return View(product);
            }
            if (!Directory.Exists(uploadFolder))
            {
                Directory.CreateDirectory(uploadFolder);
            }

            string fileName = Path.GetFileName(imageFile.FileName);
            string filePath = Path.Combine(uploadFolder, fileName);

            using (FileStream stream = new FileStream(filePath, FileMode.Create))
            {
                await imageFile.CopyToAsync(stream);
            }
            string relativePath = Path.Combine("images", Path.GetFileName(imageFile.FileName)).Replace('\\', '/');

            product.ImageURL = relativePath;
            _context.Add(product);
            await _context.SaveChangesAsync();

            ViewBag.Message = fileName + " uploaded successfully";
            return View(product);
        }

        // GET: Products/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Product == null)
            {
                return NotFound();
            }

            var product = await _context.Product.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }

        // POST: Products/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Brand,Price,WithDiscount,Description,Stock,ImageURL")] Product product)
        {
            if (id != product.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(product);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductExists(product.Id))
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
            return View(product);
        }

        // GET: Products/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Product == null)
            {
                return NotFound();
            }

            var product = await _context.Product
                .FirstOrDefaultAsync(m => m.Id == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Product == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Product'  is null.");
            }
            var product = await _context.Product.FindAsync(id);
            if (product != null)
            {
                _context.Product.Remove(product);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProductExists(int id)
        {
          return (_context.Product?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
