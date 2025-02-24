using Microsoft.AspNetCore.Mvc;

namespace RentalSystem.Controllers.Customer
{
    public class CustomerController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
