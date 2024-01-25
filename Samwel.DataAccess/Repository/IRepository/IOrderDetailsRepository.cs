using Samwel.Models;

namespace Samwel.DataAccess.Repository.IRepository
{
    public interface IOrderDetailsRepository : IRepository<OrderDetails>
    {
        void Update(OrderDetails OrderDetails);
    }
}
