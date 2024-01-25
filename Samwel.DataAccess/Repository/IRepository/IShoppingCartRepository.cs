using Samwel.Models;

namespace Samwel.DataAccess.Repository.IRepository
{
    public interface IShoppingCartRepository : IRepository<ShoppingCart>
    {
        void Update(ShoppingCart  shoppingCart);

    }
}
