using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Samwel.DataAccess.Repository;
using Samwel.DataAccess.Repository.IRepository;
using Samwel.Models;
using Samwel.Models.ViewModels;
using Samwel.Utility;
using Stripe;

namespace SamwelWeb.Areas.Admin.Controllers
{
	[Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class ProductController : Controller
    {
        private readonly IUnitOfWork _productRepository;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public ProductController(IUnitOfWork repository, IWebHostEnvironment webHostEnvironment)
        {
            _productRepository = repository;
            _webHostEnvironment = webHostEnvironment;
        }
        public IActionResult Index()
        {
           //var products = _productRepository.Product.GetAll("Category");
            
            return View();
        }
		public IActionResult Upsert(int ? Id)
		{
          
            IEnumerable<SelectListItem> CategoryList
                    = _productRepository.Category
                    .GetAll(null).Select(x => new SelectListItem
                    {
                         Text = x.Name,
                          Value = x.CategoryId.ToString(),
                    });
 
            ProductViewModel model = new ProductViewModel
            {
                Product = new Samwel.Models.Product(),
                SelectLists = CategoryList  
            };
            if (Id == 0 || Id == null)
                return View(model);
            else
            {
                model.Product = _productRepository.Product
                    .Get(x => x.Id == Id  , include: "productImages")!;
                return View(model);
            }

        }
		[HttpPost]
		public IActionResult Upsert(ProductViewModel model , List<IFormFile> ? files)
		{

            var print = model.Product.Id == 0 ? "Create" : "Update";
            if (!ModelState.IsValid)
            {
                IEnumerable<SelectListItem> CategoryList
               = _productRepository.Category
               .GetAll(null).Select(x => new SelectListItem
               {
                   Text = x.Name,
                   Value = x.CategoryId.ToString(),
               });


                model.SelectLists = CategoryList;
                
                return View(model);
            }
            if (model.Product.Id == 0)
            {
                model.Product.ImageUrl = ""; 
                _productRepository.Product.Add(model.Product);

            }
            else
            {
                _productRepository.Product.Update(model.Product);
                _productRepository.Save();
            }
            string wwwroot = _webHostEnvironment.WebRootPath;
            if (files != null)
            {
                foreach (IFormFile file in files)
                {
                    string filename = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                    string productpath = @"Images\Proucts\product-"+model.Product.Id;
                    string finalpath = Path.Combine(wwwroot, productpath);

                    if (!Directory.Exists(finalpath))
                    {
                        Directory.CreateDirectory(finalpath);   
                    }

                    using FileStream stream = new FileStream(Path.Combine(finalpath, filename), FileMode.Create);
                    file.CopyTo(stream);

                    ProductImage productImage = new ProductImage()
                    {
                        ImageUrl = @"\" + productpath + @"\" +filename 
                        , ProductId = model.Product.Id  
                    };
                    if (model.Product.productImages == null)
                        model.Product.productImages = new List<ProductImage>();

                    model.Product.productImages.Add(productImage);  
                }
                model.Product.ImageUrl = model.Product.productImages.First().ImageUrl;
                _productRepository.Product.Update(model.Product);
                _productRepository.Save();
            }
           
            TempData["Hello"] = $"We {print} {model.Product.Title} Book";

         
            return RedirectToAction("Index");
		}

        public IActionResult DeleteImage (int Id)
        {
            var image = _productRepository.ProductImage.Get(x => x.Id == Id);
            if (image != null)
            {
                if (!string.IsNullOrEmpty(image.ImageUrl))
                {
                    // delete 
                    var old = Path.Combine(_webHostEnvironment.WebRootPath, image.ImageUrl.TrimStart('\\'));
                    if (System.IO.File.Exists(old))
                    {
                        System.IO.File.Delete(old);
                    }
                }
                _productRepository.ProductImage.Remove(image);
                _productRepository.Save();

                TempData["Hello"] = $"We Delete This Image from this  Book";

            }
            return RedirectToAction(nameof(Upsert), new { Id = image.ProductId });

        }



        #region API CALLS

        [HttpGet]
        public IActionResult GetAll ()
        {
            var products = _productRepository.Product.GetAll(null,"Category");
            return
                Json(new {data = products});
        }
       
        [HttpDelete]
		public IActionResult Delete(int ? id )
		{
			var product = _productRepository.
						 Product.Get(c => c.Id == id);
            if (product == null)
            {
				return
			  Json(new { success = false , message="Error While Deleting" });
			}

            string productpath = @"Images\Proucts\product-" +id;
            string finalpath = Path.Combine(_webHostEnvironment.WebRootPath, productpath);

            if (Directory.Exists(finalpath))
            {
                string [] filepath = Directory.GetFiles(finalpath);
                foreach (string file in filepath) 
                {
                   System.IO.File.Delete(file);    
                }
                Directory.Delete(finalpath);
            }

            _productRepository.Product.Remove(product);
            _productRepository.Save();
			return
				Json(new { success = true, message = "Product was deleted" });

		}
		#endregion
	}
}
