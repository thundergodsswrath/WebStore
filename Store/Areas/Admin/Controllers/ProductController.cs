using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Store.DataAccess.Repository.IRepository;
using Store.Models;
using Store.Models.ViewModels;

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
        ProductVM productVM = new()
        {
            CategoryList = _unitOfWork.CategoryRepository
                .GetAll().Select(u=> new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString()
                }),
            Product = new Product()
        };
        return View(productVM);
    }

    [HttpPost]
    public IActionResult Create(ProductVM obj)
    {
        obj.CategoryList = _unitOfWork.CategoryRepository
            .GetAll().Select(u => new SelectListItem
            {
                Text = u.Name,
                Value = u.Id.ToString()
            });
        if (!ModelState.IsValid) return View(obj);
        _unitOfWork.ProductRepository.Create(obj.Product);
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