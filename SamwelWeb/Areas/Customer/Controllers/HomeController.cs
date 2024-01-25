using System.Diagnostics;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Samwel.DataAccess.Repository.IRepository;
using Samwel.Models;
using Samwel.Utility;

namespace SamwelWeb.Areas.Customer.Controllers
{
    
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
		private readonly IUnitOfWork _productRepository;
		public HomeController(ILogger<HomeController> logger , IUnitOfWork productRepository)
        {
            _logger = logger;
            _productRepository = productRepository;
        }

        public IActionResult Index()
        {
            
            
			var products = _productRepository.Product.GetAll(null, "Category,productImages");
			return View(products);
        }
        
        public IActionResult Details(int Id)
        {
			var product = _productRepository.Product.Get(x=>x.Id == Id , "Category,productImages");

            return View(new ShoppingCart
            {
                Product = product!
                , ProductId = Id
            });  
        }
        [HttpPost]
        [Authorize] // tell it must be Authorize user 
        public IActionResult Details(ShoppingCart shoppingCart)
        {
            var Claim = (ClaimsIdentity)User.Identity!;
            var userId = Claim.FindFirst(ClaimTypes.NameIdentifier)!.Value;

            shoppingCart.AppUserId = userId;

            // shoppingCart.Id = 0;
            var Cart = _productRepository.
                ShoppingCart.Get(x => x.AppUserId == userId &&
                x.ProductId == shoppingCart.ProductId);
            if (Cart == null)
            {
                 shoppingCart.Id = 0;
                _productRepository.ShoppingCart.Add(shoppingCart);
                _productRepository.Save();
                HttpContext.Session.SetInt32(SD.SessionCart, _productRepository.
                ShoppingCart.GetAll(x => x.AppUserId == userId)!.Count());
            }
            else
            {
                Cart.Count = shoppingCart.Count; 
                _productRepository.ShoppingCart.Update(Cart);
                _productRepository.Save();
            }
            TempData["Hello"] = $"Cart is Updated";

            return RedirectToAction("Index");   
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}