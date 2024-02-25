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
    
    public IActionResult Edit(int? id)
    {
        if (id is null or 0)
        {
            return NotFound();
        }

        Product? productFromDb = _unitOfWork.ProductRepository.Get(u => u.Id == id);
        if(productFromDb==null) return NotFound();
        return View(productFromDb);
    }
    [HttpPost]
    public IActionResult Edit(Product obj)
    {
        if (!ModelState.IsValid) return View();
        _unitOfWork.ProductRepository.Update(obj);
        _unitOfWork.Save();
        return RedirectToAction("Index");
    }
    public IActionResult Delete(int? id)
    {
        if (id is null or 0)
        {
            return NotFound();
        }

        Product? productFromDb = _unitOfWork.ProductRepository.Get(u => u.Id == id);
        if(productFromDb==null) return NotFound();
        return View(productFromDb);
    }
    [HttpPost]
    public IActionResult Delete(Product obj)
    {
        _unitOfWork.ProductRepository.Remove(obj);
        _unitOfWork.Save();
        return RedirectToAction("Index");
    }
}