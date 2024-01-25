using System.Diagnostics;
using System.Security.Claims;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Samwel.DataAccess.Repository;
using Samwel.DataAccess.Repository.IRepository;
using Samwel.Models;
using Samwel.Models.ViewModels;
using Samwel.Utility;
using Stripe;
using Stripe.Checkout;
using Stripe.Issuing;

namespace SamwelWeb.Areas.Admin.Controllers
{
	[Area("Admin")]
	[Authorize]	
	public class OrderController : Controller
	{
		private readonly IUnitOfWork _orderRepository;
		[BindProperty]
		public OrderVM order { get; set; }	

        public OrderController(IUnitOfWork _orderRepository)
		{
			this._orderRepository = _orderRepository;
		}
		public IActionResult Index()
		{
			return View();
		}
		public IActionResult Details(int id)
		{
			OrderVM orderVM = new OrderVM()
			{
				OrderHeader = _orderRepository.OrderHeader.Get(x => x.Id == id , include: "AppUser")!,
				Details = _orderRepository.OrderDetails.GetAll(x => x.OrderHeaderId == id , include:"Product")	
			};
			return View(orderVM);
		}
        [Authorize(Roles = SD.Role_Admin + "," + SD.Role_Employee)]
        [HttpPost]
        public IActionResult StartProcessing ()
		{
			_orderRepository.OrderHeader.UpdateStatus(order.OrderHeader.Id, SD.StatusInProcess);
			_orderRepository.Save();
            TempData["Hello"] = $"We update Ordere ";

            return RedirectToAction(nameof(Details), new { id = order.OrderHeader.Id });

        }
        [Authorize(Roles = SD.Role_Admin + "," + SD.Role_Employee)]
        [HttpPost]
        public IActionResult ShipOrder()
        {
			var orderheader = 
				_orderRepository.OrderHeader.Get(c=>c.Id == order.OrderHeader.Id);
			orderheader!.Carrier = order.OrderHeader.Carrier;
            orderheader!.TrackingNumber = order.OrderHeader.TrackingNumber;
            orderheader!.ShippingDate = DateTime.Now;
			orderheader!.OrderStatus = SD.StatusShipped; 
			if (orderheader.PaymentStatus == SD.PaymentStatusDelayedPayment)
			{
				orderheader.PaymentDueDate = DateTime.Now.AddDays(30);
			}

            _orderRepository.OrderHeader.Update(orderheader);
            _orderRepository.Save();
            TempData["Hello"] = $"We Shipped Ordere ";

            return RedirectToAction(nameof(Details), new { id = order.OrderHeader.Id });

        }
        public IActionResult CancelOrder()
        {
            var orderheader =
                _orderRepository.OrderHeader.Get(c => c.Id == order.OrderHeader.Id);
         
			if (orderheader!.PaymentStatus == SD.PaymentStatusApproved)
			{
				var options = new RefundCreateOptions()
				{
					Reason = RefundReasons.RequestedByCustomer ,
					PaymentIntent = orderheader.PaymentIntendId
                };
				var service = new RefundService();

				Refund refund = service.Create(options);
				_orderRepository.OrderHeader.UpdateStatus
					(orderheader.Id, SD.StatusCancelled , SD.StatusRefunded) ;

			}
			else
			{
                _orderRepository.OrderHeader.UpdateStatus
                (orderheader.Id, SD.StatusCancelled);
            }

            _orderRepository.Save();
            TempData["Hello"] = $"We  Ordere Cancelled ";

            return RedirectToAction(nameof(Details), new { id = order.OrderHeader.Id });
        }

		[ActionName("Details")]
		[HttpPost]
		public IActionResult Details_PAY_NOW ()
		{
            order.OrderHeader = _orderRepository.OrderHeader.Get(x => x.Id == order.OrderHeader.Id, include: "AppUser")!;
            order.Details = _orderRepository.OrderDetails.GetAll(x => x.OrderHeaderId == order.OrderHeader.Id, include: "Product");
            var Domain = Request.Scheme+"://"+Request.Host.Value;
            var options = new SessionCreateOptions
            {
                SuccessUrl = Domain + $"/admin/order/PaymentConfirmation?id={order.OrderHeader.Id}",
                CancelUrl = Domain + $"/admin/order/details?id={order.OrderHeader.Id}" ,
                Mode = "payment",
                LineItems = new()
            };
            foreach (var item in order.Details)
            {
                var sessionlineItem = new SessionLineItemOptions
                {
                    PriceData = new()
                    {
                        UnitAmount = (long)item.Price * 100,
                        Currency = "usd",
                        ProductData = new()
                        {
                            Name = item.Product.Title
                        }
                    }
                    ,
                    Quantity = item.Count
                };
                options.LineItems.Add(sessionlineItem);
            }

            var service = new SessionService();
            Session session = service.Create(options);
            _orderRepository
                .OrderHeader.UpdateSripePaymentID(order.OrderHeader.Id, session.Id, session.PaymentIntentId);
            _orderRepository.Save();
            Response.Headers.Add("Location", session.Url);
            return new StatusCodeResult(303);

        }
        public IActionResult PaymentConfirmation(int id)
        {
            var OrderHeader = _orderRepository.OrderHeader.Get(x => x.Id == id)!;
            if (OrderHeader.PaymentStatus == SD.PaymentStatusDelayedPayment)
            {
                var service = new SessionService();
                Session session = service.Get(OrderHeader.SessionId);
                if (session.PaymentStatus.ToLower() == "paid")
                {
                    _orderRepository.OrderHeader
                        .UpdateSripePaymentID(OrderHeader.Id, session.Id, session.PaymentIntentId);
                    _orderRepository.
                        OrderHeader.UpdateStatus(id,OrderHeader.State, SD.PaymentStatusApproved);
                    _orderRepository.Save();
                }
                
            }

     
            return View(id);
        }
        [Authorize(Roles = SD.Role_Admin +","+SD.Role_Employee)]
		[HttpPost]
        public IActionResult UpdateOrderDetails()
        {
			var orderheader = _orderRepository.OrderHeader.
				Get(x=>x.Id == order.OrderHeader.Id);
			orderheader!.Name = order.OrderHeader.Name;
            orderheader.PhoneNumber = order.OrderHeader.PhoneNumber;
            orderheader.City = order.OrderHeader.City;
            orderheader.State = order.OrderHeader.State;
            orderheader.PostalCode = order.OrderHeader.PostalCode!;
            orderheader.StreetAddress = order.OrderHeader.StreetAddress;
			if (!string.IsNullOrEmpty(order.OrderHeader.Carrier))
                orderheader.Carrier = order.OrderHeader.Carrier;
            if (!string.IsNullOrEmpty(order.OrderHeader.TrackingNumber))
                orderheader.TrackingNumber = order.OrderHeader.TrackingNumber;
			_orderRepository.OrderHeader.Update(orderheader);
            TempData["Hello"] = $"We update Ordere ";

            _orderRepository.Save();
			return RedirectToAction(nameof(Details), new { id = orderheader.Id });
        }
        #region API CALLS
        public IActionResult GetAll(string status)
		{
			 IEnumerable<OrderHeader> OrderHeader;
            if(User.IsInRole(SD.Role_Admin) || User.IsInRole(SD.Role_Employee))
			{
                OrderHeader = _orderRepository.OrderHeader.GetAll(null, "AppUser");
            }
			else
			{
                var Claim = (ClaimsIdentity)User.Identity!;
                var userId = Claim.FindFirst(ClaimTypes.NameIdentifier)!.Value;
				OrderHeader = _orderRepository
					.OrderHeader.GetAll(u => u.AppUserId == userId, include: "AppUser");
            }

            switch (status)
			{
				case "pending":
					OrderHeader = OrderHeader.Where(x => x.PaymentStatus == SD.PaymentStatusDelayedPayment);
					break;
				case "approved":
					OrderHeader = OrderHeader.Where(x => x.OrderStatus == SD.StatusApproved);
					break;
				case "inprocess":
					OrderHeader = OrderHeader.Where(x => x.OrderStatus == SD.StatusInProcess);
					break;
				case "completed":
					OrderHeader = OrderHeader.Where(x => x.OrderStatus == SD.StatusShipped);
					break;
				default:
					break;
			}
			return
				Json(new { data = OrderHeader });
		}
		#endregion
	}
}
