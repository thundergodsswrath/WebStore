using Microsoft.AspNetCore.Mvc;
using Store.DataAccess.Repository.IRepository;
using Store.Models;

namespace Store.Areas.Admin.Controllers;
[Area("Admin")]
public class ProductController(IUnitOfWork unitOfWork) : Controller
{
    private IUnitOfWork _unitOfWork { get; set; } = unitOfWork;

    public IActionResult Index()
    {
        List<Product> objProductsList = _unitOfWork.ProductRepository.GetAll().ToList();
        return View(objProductsList);
    }

    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    public IActionResult Create(Product obj)
    {
        if (!ModelState.IsValid) return View();
        _unitOfWork.ProductRepository.Create(obj);
        _unitOfWork.Save();
        return RedirectToAction("Index");
    }
    
    public IActionResult Edit()
    {
        return View();
    }
    
    public IActionResult Delete()
    {
        return View();
    }
}