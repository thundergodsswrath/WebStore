using System.Diagnostics;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Store.DataAccess.Repository.IRepository;
using Store.Models;

namespace Store.Areas.Customer.Controllers;
[Area("Customer")]
public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IUnitOfWork _unitOfWork;

    public HomeController(ILogger<HomeController> logger, IUnitOfWork unitOfWork)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
    }

    public IActionResult Index()
    {
        IEnumerable<Product> productList = _unitOfWork.ProductRepository.GetAll(includeProperties: "Category");
        return View(productList);
    }
    
    public IActionResult Details(int productId)
    {
        ShoppingCart shoppingCart = new()
        {
            Product = _unitOfWork.ProductRepository.Get(u => u.Id == productId, includeProperties: "Category"),
            Count = 1,
            ProductId = productId
        };
        return View(shoppingCart);
    }
    
    [HttpPost]
    [Authorize]
    public IActionResult Details(ShoppingCart shoppingCart)
    {
        var claimsIdentity = (ClaimsIdentity)User.Identity;
        var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
        shoppingCart.ApplicationUserId = userId;

        ShoppingCart cartFromDb = _unitOfWork.ShoppingCartRepository.Get(u => u.ApplicationUserId == userId &&
                                                                              u.ProductId==shoppingCart.ProductId);
        if (cartFromDb is not null)
        {
            cartFromDb.Count += shoppingCart.Count;
            _unitOfWork.ShoppingCartRepository.Update(cartFromDb);
        }
        else
        {
            _unitOfWork.ShoppingCartRepository.Create(shoppingCart);
        }
        
        _unitOfWork.Save();
        
        return RedirectToAction(nameof(Index));
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}