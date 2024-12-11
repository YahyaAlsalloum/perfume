using Microsoft.AspNetCore.Mvc;

namespace perfume.Controllers
{
    public class AdminController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
