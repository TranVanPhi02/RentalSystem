using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using RentalSystem.Models;

namespace RentalSystem.Controllers;

public class HomeController : Controller
{
    public IActionResult Index()
    {
        return RedirectToAction("Home", "Customer");
    }
}
