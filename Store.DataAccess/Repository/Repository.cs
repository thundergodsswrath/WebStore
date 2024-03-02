using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Store.DataAccess.Data;
using Store.DataAccess.Repository.IRepository;

namespace Store.DataAccess.Repository;

public class Repository<T> : IRepository<T> where T : class
{
    private readonly ApplicationDbContext _db;
    private DbSet<T> _dbSet;

    protected Repository(ApplicationDbContext db)
    {
        _db = db;
        _dbSet = _db.Set<T>();
    }
    
    public IEnumerable<T> GetAll(string? includeProperties = null)
    {
        IQueryable<T> query = _dbSet;
        if (!string.IsNullOrEmpty(includeProperties))
        {
            foreach (var prop in includeProperties.Split(new char[]{','}, StringSplitOptions.RemoveEmptyEntries))
            {
                query = query.Include(prop);
            }
        }
        return query.ToList();
    }

    public T Get(Expression<Func<T, bool>> filter, string? includeProperties = null)
    {
        IQueryable<T> query = _dbSet.Where(filter);
        if (!string.IsNullOrEmpty(includeProperties))
        {
            foreach (var prop in includeProperties.Split(new char[]{','}, StringSplitOptions.RemoveEmptyEntries))
            {
                query = query.Include(prop);
            }
        }
        return query.FirstOrDefault();
    }

    public void Create(T entity)
    {
        _dbSet.Add(entity);
    }

    public void Remove(T entity)
    {
        _dbSet.Remove(entity);
    }

    public void RemoveRange(IEnumerable<T> entities)
    {
        _dbSet.RemoveRange(entities);
    }
}