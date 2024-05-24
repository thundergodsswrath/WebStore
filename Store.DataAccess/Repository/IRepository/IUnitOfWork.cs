namespace Store.DataAccess.Repository.IRepository;

public interface IUnitOfWork
{
    ICategoryRepository CategoryRepository { get; }
    IProductRepository ProductRepository { get; }
    IShoppingCartRepository ShoppingCartRepository { get; }

    void Save();
}