using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Store.DataAccess.Repository.IRepository;
using Store.Models;
using Store.Models.ViewModels;
using Store.Utility;

namespace Store.Areas.Admin.Controllers;
[Area("Admin")]
[Authorize(Roles = StaticDetails.RoleAdmin)]
public class ProductController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment) : Controller
{
    private IUnitOfWork _unitOfWork { get; } = unitOfWork;
    private readonly IWebHostEnvironment _webHostEnvironment = webHostEnvironment;

    public IActionResult Index()
    {
        List<Product> objProductsList = _unitOfWork.ProductRepository.GetAll(includeProperties:"Category").ToList();
        return View(objProductsList);
    }

    public IActionResult Upsert(int? id)
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
        if (id is not (null or 0))
        {
            productVM.Product = _unitOfWork.ProductRepository.Get(u => u.Id == id);
        }
        return View(productVM);
    }

    [HttpPost]
    public IActionResult Upsert(ProductVM obj, IFormFile? file)
    {
        if (ModelState.IsValid)
        {
            string wwwRootPath = _webHostEnvironment.WebRootPath;
            if (file is not null)
            {
                string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                string productPath = Path.Combine(wwwRootPath, @"images\product");

                if (!string.IsNullOrEmpty(obj.Product.ImageUrl))
                {
                    var oldImage = Path.Combine(wwwRootPath, obj.Product.ImageUrl.TrimStart('\\'));
                    if (System.IO.File.Exists(oldImage))
                    {
                        System.IO.File.Delete(oldImage);
                    }
                }
                
                using (var fileStream = new FileStream(Path.Combine(productPath, fileName), FileMode.Create))
                {
                    file.CopyTo(fileStream);
                }

                obj.Product.ImageUrl = @"\images\product\" + fileName;
            }

            if (obj.Product.Id == 0)
            {
                _unitOfWork.ProductRepository.Create(obj.Product);
            }
            else
            {
                _unitOfWork.ProductRepository.Update(obj.Product);
            }
            
            _unitOfWork.Save();
            return RedirectToAction("Index");
        }
        else
        {
            obj.CategoryList = _unitOfWork.CategoryRepository
                .GetAll().Select(u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString()
                });
            return View(obj);
        }
    } 

    [HttpGet]
    public IActionResult GetAll()
    {
        List<Product> objProductsList = _unitOfWork.ProductRepository.GetAll(includeProperties: "Category").ToList();
        return Json(new { data = objProductsList });
    }
    [HttpDelete]
    public IActionResult Delete(int? id)
    {
        var product = _unitOfWork.ProductRepository.Get(u => u.Id == id);
        if (product is null)
        {
            return Json(new { success = false, message = "Error while deleting" });
        }
        
        var oldImage = Path.Combine(_webHostEnvironment.WebRootPath, product.ImageUrl.TrimStart('\\'));
        if (System.IO.File.Exists(oldImage))
        {
            System.IO.File.Delete(oldImage);
        }
        
        _unitOfWork.ProductRepository.Remove(product);
        _unitOfWork.Save();
        
        return Json(new { success = true, message = "Delete successful!" });
    }
}