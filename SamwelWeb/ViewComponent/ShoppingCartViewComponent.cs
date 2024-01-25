using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Samwel.DataAccess.Repository;
using Samwel.DataAccess.Repository.IRepository;
using Samwel.Utility;

namespace SamwelWeb.ViewComponent
{
    public class ShoppingCartViewComponent : Microsoft.AspNetCore.Mvc.ViewComponent
    {
        private readonly IUnitOfWork _Repository;
        public ShoppingCartViewComponent(IUnitOfWork repository)
        {
            _Repository = repository;
           
        }
        public async Task <IViewComponentResult> InvokeAsync()
        {
            var Claim = (ClaimsIdentity)User.Identity!;
            var Check = Claim.FindFirst(ClaimTypes.NameIdentifier);
            if (Check != null)
            {
                if (HttpContext.Session.GetInt32(SD.SessionCart)== null)
                {
                    HttpContext.Session.SetInt32(SD.SessionCart, _Repository.
                   ShoppingCart.GetAll(x => x.AppUserId == Check.Value)!.Count());
  
                }
             
                return View(HttpContext.Session.GetInt32(SD.SessionCart)); 
            }
            else
            {
                HttpContext.Session.Clear();
                return View(0);
            }

        }
    }
}
