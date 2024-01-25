using Samwel.DataAccess.Data;
using Samwel.DataAccess.Repository.IRepository;
using Samwel.Models;

namespace Samwel.DataAccess.Repository
{
    public class OrderDetailsRepository : Repository<OrderDetails>, IOrderDetailsRepository
    {
        private AppDbContext _db;
        public OrderDetailsRepository(AppDbContext context)
            : base(context)
        {
            _db = context;
        }


        public void Update(OrderDetails OrderDetails)
        {
            _db.OrderDetails.Update(OrderDetails);
        }
    }
}
