using Store.Models;

namespace Store.DataAccess.Repository.IRepository;

public interface IShoppingCartRepository : IRepository<ShoppingCart>
{
    void Update(ShoppingCart obj);
}