using Microsoft.AspNetCore.Mvc;

namespace Store.Controllers;

public class CategoryController : Controller
{
    // GET
    public IActionResult Index()
    {
        return View();
    }
}