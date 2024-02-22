using Microsoft.AspNetCore.Mvc;
using Store.DataAccess.Data;
using Store.DataAccess.Repository.IRepository;
using Store.Models;

namespace Store.Controllers;

public class CategoryController : Controller
{
    private readonly IUnitOfWork _unitOfWork;

    public CategoryController(IUnitOfWork db)
    {
        _unitOfWork = db;
    }

    public IActionResult Index()
    {
        List<Category> objCategoryList = _unitOfWork.CategoryRepository.GetAll().ToList();
        return View(objCategoryList);
    }

    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    public IActionResult Create(Category obj)
    {
        if (ModelState.IsValid)
        {
            _unitOfWork.CategoryRepository.Create(obj);
            _unitOfWork.Save();
            TempData["success"] = "Category created successfully!";
            return RedirectToAction("Index");
        }

        return View();
    }

    public IActionResult Edit(int? id)
    {
        if (id is null or 0)
        {
            return NotFound();
        }

        Category? categoryFromDb = _unitOfWork.CategoryRepository.Get(u => u.Id == id);

        if (categoryFromDb == null)
        {
            return NotFound();
        }

        return View(categoryFromDb);
    }

    [HttpPost]
    public IActionResult Edit(Category obj)
    {
        if (ModelState.IsValid)
        {
            _unitOfWork.CategoryRepository.Update(obj);
            _unitOfWork.Save();
            TempData["success"] = "Category updated successfully!";
            return RedirectToAction("Index");
        }

        return View();
    }

    public IActionResult Delete(int? id)
    {
        if (id is null or 0)
        {
            return NotFound();
        }

        Category? categoryFromDb = _unitOfWork.CategoryRepository.Get(u=> u.Id == id);

        if (categoryFromDb == null)
        {
            return NotFound();
        }

        return View(categoryFromDb);
    }

    [HttpPost]
    public IActionResult Delete(Category obj)
    {
        _unitOfWork.CategoryRepository.Remove(obj);

        _unitOfWork.Save();
        TempData["success"] = "Category deleted successfully!";
        return RedirectToAction("Index");
    }
}