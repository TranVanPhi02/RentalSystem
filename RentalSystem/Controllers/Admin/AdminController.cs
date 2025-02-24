using Microsoft.AspNetCore.Mvc;

namespace RentalSystem.Controllers.Admin
{
    public class AdminController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
