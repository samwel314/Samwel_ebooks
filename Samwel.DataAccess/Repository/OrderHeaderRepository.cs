using Samwel.DataAccess.Data;
using Samwel.DataAccess.Repository.IRepository;
using Samwel.Models;

namespace Samwel.DataAccess.Repository
{
    public class OrderHeaderRepository : Repository<OrderHeader>, IOrderHeaderRepository
    {
        private AppDbContext _db;
        public OrderHeaderRepository(AppDbContext context)
            : base(context)
        {
            _db = context;
        }


        public void Update(OrderHeader OrderHeader)
        {
            _db.OrderHeaders.Update(OrderHeader);
        }

		public void UpdateSripePaymentID(int id, string sessionId, string PaymentIntentId)
		{
			var Order = _db.OrderHeaders.FirstOrDefault(x => x.Id == id);
			if (Order != null)
			{
				if (!string.IsNullOrEmpty(sessionId))
				{
					Order.SessionId = sessionId;
				}
				if (!string.IsNullOrEmpty(PaymentIntentId))
				{
					Order.PaymentIntendId = PaymentIntentId;
					Order.PaymentDate = DateTime.Now;	
				}
			}
		}

		public void UpdateStatus(int id, string orderStatus, string? PaymentStatus = null)
		{
			var Order = _db.OrderHeaders.FirstOrDefault(x => x.Id == id);
			if (Order != null)
			{
                Order.OrderStatus = orderStatus;    
                if (!string.IsNullOrEmpty(PaymentStatus)) 
                {
                    Order.PaymentStatus = PaymentStatus;
                }
			}
		}
	}
}
