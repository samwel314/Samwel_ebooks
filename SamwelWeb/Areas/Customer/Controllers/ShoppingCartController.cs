using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Samwel.DataAccess.Repository.IRepository;
using Samwel.Models;
using Samwel.Models.ViewModels;
using Samwel.Utility;
using Stripe.Checkout;
using Stripe.FinancialConnections;
using Session = Stripe.Checkout.Session;
using SessionCreateOptions = Stripe.Checkout.SessionCreateOptions;
using SessionService = Stripe.Checkout.SessionService;

namespace SamwelWeb.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Authorize]
  
    public class ShoppingCartController : Controller
    {
        private readonly IUnitOfWork _productRepository;
        public ShoppingCartController( IUnitOfWork productRepository)
        {
            _productRepository = productRepository;
        }
        public IActionResult Index()
        {
            var Claim = (ClaimsIdentity)User.Identity!;
            var userId = Claim.FindFirst(ClaimTypes.NameIdentifier)!.Value;

            var Cartvm = new ShoppingCartVM
            {
                shoppingCarts =
                _productRepository.ShoppingCart.GetAll(
                    x => x.AppUserId == userId , "Product"),
                OrderHeader = new OrderHeader()
            };
            IEnumerable<ProductImage> productImages = _productRepository.ProductImage.GetAll(null); 
           
            foreach(var cart in Cartvm.shoppingCarts)
            {
                cart.Product.productImages =
                    productImages.Where(c=>c.ProductId == cart.ProductId).ToList(); 
            }
            
            foreach (var item in Cartvm.shoppingCarts)
            {
                item.Price = GetPriceBasedOnQuantity(item);
                Cartvm.OrderHeader.OrderTotal += item.Price * item.Count;
            }
            return View(Cartvm);
        }
        public IActionResult Summary()
        {
            var Claim = (ClaimsIdentity)User.Identity!;
            var userId = Claim.FindFirst(ClaimTypes.NameIdentifier)!.Value;

            var Cartvm = new ShoppingCartVM
            {
                shoppingCarts =
                _productRepository.ShoppingCart.GetAll(
                    x => x.AppUserId == userId, "Product"),
                OrderHeader = new OrderHeader()
            };
            Cartvm.OrderHeader.AppUser =
                _productRepository.AppUser.Get(x=>x.Id ==  userId)!;
            
            Cartvm.OrderHeader.Name = Cartvm.OrderHeader.AppUser.Name;
            Cartvm.OrderHeader.PostalCode = Cartvm.OrderHeader.AppUser.PostalCode!;
            Cartvm.OrderHeader.PhoneNumber = Cartvm.OrderHeader.AppUser.PhoneNumber!;
            Cartvm.OrderHeader.State = Cartvm.OrderHeader.AppUser.State!;
            Cartvm.OrderHeader.StreetAddress = Cartvm.OrderHeader.AppUser.StreetAddress!;
            Cartvm.OrderHeader.City = Cartvm.OrderHeader.AppUser.City!;

            foreach (var item in Cartvm.shoppingCarts)
            {
                item.Price = GetPriceBasedOnQuantity(item);
                Cartvm.OrderHeader.OrderTotal += item.Price * item.Count;
            }
            return View(Cartvm);
        }

        [HttpPost]
		public IActionResult Summary(OrderHeader OrderHeader)
		{
			var Claim = (ClaimsIdentity)User.Identity!;
			var userId = Claim.FindFirst(ClaimTypes.NameIdentifier)!.Value;

            var Carts = _productRepository.ShoppingCart.GetAll(
                    x => x.AppUserId == userId, "Product");

			foreach (var item in Carts)
			{
				item.Price = GetPriceBasedOnQuantity(item);
				OrderHeader.OrderTotal += item.Price * item.Count;
			}
            OrderHeader.AppUserId = userId;
            OrderHeader.OrderDate = DateTime.Now;
            var CrunUser = 
                _productRepository.AppUser.Get(x => x.Id == userId);
            if (CrunUser!.CompanyId != null)
            {

				OrderHeader.OrderStatus = SD.StatusApproved;
				OrderHeader.PaymentStatus = SD.PaymentStatusDelayedPayment;
			}
            else
            {
                OrderHeader.OrderStatus = SD.StatusPending;
                OrderHeader.PaymentStatus = SD.PaymentStatusPending;
            }

			_productRepository.OrderHeader.Add(OrderHeader);
			_productRepository.Save();
            foreach (var item in Carts)
            {
                _productRepository
                    .OrderDetails.Add(new OrderDetails
                    {
                        Count = item.Count,
                        Price = item.Price,
                        OrderHeader = OrderHeader,
                        ProductId = item.Product.Id
                    });
				_productRepository.Save();

			}
			if (CrunUser!.CompanyId == null)
            {
                var Domain = Request.Scheme + "://" + Request.Host.Value;
             
                var options = new SessionCreateOptions
                {
                    SuccessUrl = Domain + $"/Customer/ShoppingCart/OrderConfirmation?id={OrderHeader.Id}",
                    CancelUrl = Domain + "/Customer/ShoppingCart/Index",
					Mode = "payment",
                    LineItems = new()
				};
                foreach (var item in Carts)
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
                        ,Quantity = item.Count
                    };
                    options.LineItems.Add(sessionlineItem);
                }

				var service = new SessionService();
				Session session =   service.Create(options);
                _productRepository
                    .OrderHeader.UpdateSripePaymentID(OrderHeader.Id, session.Id, session.PaymentIntentId);
			    _productRepository .Save();
                Response.Headers.Add("Location", session.Url);
                return new StatusCodeResult(303);
            }

            return RedirectToAction(nameof(OrderConfirmation),new { Id = OrderHeader.Id });
		}

        public IActionResult OrderConfirmation (int Id)
        {
            var OrderHeader = _productRepository.OrderHeader.Get(x => x.Id == Id, include: "AppUser")!;
            if (OrderHeader.PaymentStatus!=SD.PaymentStatusDelayedPayment )
            {
                var service = new SessionService();
                Session session = service.Get(OrderHeader.SessionId);
                if (session.PaymentStatus.ToLower() =="paid" ) 
                {
                    _productRepository.OrderHeader
                        .UpdateSripePaymentID(OrderHeader.Id, session.Id, session.PaymentIntentId);
                    _productRepository.
                        OrderHeader.UpdateStatus(Id, SD.StatusApproved, SD.PaymentStatusApproved);
                    _productRepository.Save(); 
                }
                HttpContext.Session.Clear();
            }
            List<ShoppingCart> carts = _productRepository
         .ShoppingCart.GetAll(U => U.AppUserId == OrderHeader.AppUserId).ToList();
            _productRepository.ShoppingCart.RemoveRange(carts);

            return View(Id);  
        }
		public IActionResult Plus(int Id)
        {
            var cart = _productRepository
                .ShoppingCart.Get(x => x.Id == Id);
            cart!.Count++;
            _productRepository.ShoppingCart.Update(cart);   
            _productRepository.Save();
    
            return RedirectToAction("Index");
        }
        
        public IActionResult Minus(int Id)
        {
            var cart = _productRepository
            .ShoppingCart.Get(x => x.Id == Id);
            if (cart!.Count!=1)
            {
                cart!.Count--;
                _productRepository.ShoppingCart.Update(cart);
                _productRepository.Save();
            }

            else
            {
                HttpContext.Session.SetInt32(SD.SessionCart, _productRepository.
   ShoppingCart.GetAll(x => x.AppUserId == cart.AppUserId)!.Count() - 1);
                _productRepository.ShoppingCart.Remove(cart!);
       
                _productRepository.Save();
            }

   
            return RedirectToAction("Index");
        }
        public IActionResult Remove(int Id)
        {
            var cart = _productRepository
            .ShoppingCart.Get(x => x.Id == Id);
            var uid = cart?.AppUserId;  
            HttpContext.Session.SetInt32(SD.SessionCart, _productRepository.
     ShoppingCart.GetAll(x => x.AppUserId == uid)!.Count() - 1);
            _productRepository.ShoppingCart.Remove(cart!);
        
            _productRepository.Save();
          
            return RedirectToAction("Index");
        }

       
        private double GetPriceBasedOnQuantity (ShoppingCart shoppingCart)
        {
            if (shoppingCart.Count <= 50)
            {
                return shoppingCart.Product.Price;
            }
            else
            {
                if (shoppingCart.Count <= 100)
                    return shoppingCart.Product.Price50;
                else
                    return shoppingCart.Product.Price100;
            }
        }
    }
}
