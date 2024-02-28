using Store.DataAccess.Data;
using Store.DataAccess.Repository.IRepository;
using Store.Models;

namespace Store.DataAccess.Repository;

public class ProductRepository : Repository<Product>, IProductRepository
{
    private ApplicationDbContext _db;

    public ProductRepository(ApplicationDbContext db) : base(db)
    {
        _db = db;
    }

    public void Update(Product obj)
    {
        var objFromDb = _db.Products.FirstOrDefault(u => u.Id == obj.Id);
        if (objFromDb is not null)
        {
            objFromDb.Title = obj.Title;
            objFromDb.Description = obj.Description;
            objFromDb.Developer = obj.Developer;
            objFromDb.Price = obj.Price;
            objFromDb.PriceFor4Copies = obj.PriceFor4Copies;
            objFromDb.ListPrice = obj.ListPrice;
            objFromDb.CategoryId = obj.CategoryId;
            if (obj.ImageUrl is not null)
            {
                objFromDb.ImageUrl = obj.ImageUrl;
            }
        }
    }
}