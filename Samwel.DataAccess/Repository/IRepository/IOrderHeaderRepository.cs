using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using Samwel.Models;

namespace Samwel.DataAccess.Repository.IRepository
{
    public interface IOrderHeaderRepository : IRepository<OrderHeader>
    {
        void Update(OrderHeader OrderHeader);

        void UpdateStatus
            (int id, string orderStatus, string? PaymentStatus = null);
        void UpdateSripePaymentID
            (int id, string sessionId, string PaymentIntentId);
    }
}
