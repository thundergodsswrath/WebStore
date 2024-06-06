using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Store.DataAccess.Repository.IRepository;
using Store.Models;
using Store.Models.ViewModels;

namespace Store.Areas.Customer.Controllers;

[Authorize]
[Area("Customer")]
public class CartController : Controller
{
    private readonly IUnitOfWork _unitOfWork;
    public ShoppingCartVM ShoppingCartVM { get; set; }

    public CartController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public IActionResult Index()
    {
        var claimsIdentity = (ClaimsIdentity)User.Identity;
        var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

        ShoppingCartVM = new()
        {
            ShoppingCartList = _unitOfWork.ShoppingCartRepository.GetAll(u => u.ApplicationUserId == userId,
                includeProperties: "Product"),
                OrderHeader = new()
        };

        foreach (var cart in ShoppingCartVM.ShoppingCartList)
        {
            cart.Price = GetPriceBasedOnQuantity(cart);
            ShoppingCartVM.OrderHeader.OrderTotal += cart.Price * cart.Count;
        }

        return View(ShoppingCartVM);
    }

    public IActionResult Plus(int cartId)
    {
        var cartFromDb = _unitOfWork.ShoppingCartRepository.Get(u => u.Id == cartId);
        cartFromDb.Count++;
        _unitOfWork.ShoppingCartRepository.Update(cartFromDb);
        _unitOfWork.Save();
        return RedirectToAction(nameof(Index));
    }

    public IActionResult Minus(int cartId)
    {
        var cartFromDb = _unitOfWork.ShoppingCartRepository.Get(u => u.Id == cartId);
        if (cartFromDb.Count <= 1)
        {
            _unitOfWork.ShoppingCartRepository.Remove(cartFromDb);
        }
        else
        {
            cartFromDb.Count--;
            _unitOfWork.ShoppingCartRepository.Update(cartFromDb);
        }

        _unitOfWork.Save();

        return RedirectToAction(nameof(Index));
    }

    public IActionResult Delete(int cartId)
    {
        var cartFromDb = _unitOfWork.ShoppingCartRepository.Get(u => u.Id == cartId);
        _unitOfWork.ShoppingCartRepository.Remove(cartFromDb);
        _unitOfWork.Save();
        return RedirectToAction(nameof(Index));
    }

    public IActionResult Summary()
    {
        var claimsIdentity = (ClaimsIdentity)User.Identity;
        var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

        ShoppingCartVM = new()
        {
            ShoppingCartList = _unitOfWork.ShoppingCartRepository.GetAll(u => u.ApplicationUserId == userId,
                includeProperties: "Product"),
            OrderHeader = new()
        };

        ShoppingCartVM.OrderHeader.ApplicationUser = _unitOfWork.ApplicationUserRepository.Get(u => u.Id == userId);

        ShoppingCartVM.OrderHeader.Name = ShoppingCartVM.OrderHeader.ApplicationUser.Name;
        ShoppingCartVM.OrderHeader.PhoneNumber = ShoppingCartVM.OrderHeader.ApplicationUser.PhoneNumber;
        ShoppingCartVM.OrderHeader.StreetAddress = ShoppingCartVM.OrderHeader.ApplicationUser.StreetAddress;
        ShoppingCartVM.OrderHeader.City = ShoppingCartVM.OrderHeader.ApplicationUser.City;
        ShoppingCartVM.OrderHeader.State = ShoppingCartVM.OrderHeader.ApplicationUser.State;
        ShoppingCartVM.OrderHeader.PostalCode = ShoppingCartVM.OrderHeader.ApplicationUser.PostalCode;

        foreach (var cart in ShoppingCartVM.ShoppingCartList)
        {
            cart.Price = GetPriceBasedOnQuantity(cart);
            ShoppingCartVM.OrderHeader.OrderTotal += cart.Price * cart.Count;
        }

        return View(ShoppingCartVM);
    }

    private double GetPriceBasedOnQuantity(ShoppingCart shoppingCart)
    {
        const int maxCopiesQuantityFor1To3CopiesPrice = 3;
        return shoppingCart.Count <= maxCopiesQuantityFor1To3CopiesPrice
            ? shoppingCart.Product.Price
            : shoppingCart.Product.PriceFor4Copies;
    }
}