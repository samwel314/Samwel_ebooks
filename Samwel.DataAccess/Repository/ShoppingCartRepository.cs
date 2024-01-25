using Samwel.DataAccess.Data;
using Samwel.DataAccess.Repository.IRepository;
using Samwel.Models;

namespace Samwel.DataAccess.Repository
{
    public class ShoppingCartRepository : Repository<ShoppingCart>, IShoppingCartRepository
    {
        private AppDbContext _db;
        public ShoppingCartRepository(AppDbContext context)
            : base(context)
        {
            _db = context;
        }


        public void Update(ShoppingCart shoppingCart)
        {
            _db.ShoppingCarts.Update(shoppingCart);
        }
    }
}
