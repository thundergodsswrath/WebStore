using Store.DataAccess.Data;
using Store.DataAccess.Repository.IRepository;

namespace Store.DataAccess.Repository;

public class UnitOfWork : IUnitOfWork
{
    private ApplicationDbContext _db;

    public UnitOfWork(ApplicationDbContext db)
    {
        _db = db;
        CategoryRepository = new CategoryRepository(_db);
    }
    
    public ICategoryRepository CategoryRepository { get; private set; }
    public void Save()
    {
        _db.SaveChanges();
    }
}