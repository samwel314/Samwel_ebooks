using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Samwel.DataAccess.Repository;
using Samwel.DataAccess.Repository.IRepository;
using Samwel.Models;
using Samwel.Utility;

namespace SamwelWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class CompanyController : Controller
    {
        private readonly IUnitOfWork _companyRepository;
        public CompanyController(IUnitOfWork repository)
        {
            _companyRepository = repository;
        }
        public IActionResult Index()
        {
            var Categories =
                _companyRepository.Company.GetAll(null);
            return View(Categories);
        }

        public IActionResult Create()
        {
            return View();
        }
		[HttpPost]
		public IActionResult Create(Company company)
		{
		
			if (!ModelState.IsValid)
				return View();
			_companyRepository.Company.Add(company);
			TempData["Hello"] = $"We Create {company.Name} company";
			return RedirectToAction("Index");
		}

        public IActionResult Edit(int? Id)
        {
            if (Id == null || Id == 0)
            {
                return NotFound();
            }

            var Company =
                _companyRepository
                .Company.Get(u => u.Id == Id);

            if (Company == null)
                return NotFound();
            return View(Company);
        }
        [HttpPost]
        public IActionResult Edit(Company company)
        {
          

            if (!ModelState.IsValid)
                return View();
            _companyRepository.Company.Update(company);
            _companyRepository.Save();
            TempData["Hello"] = $"We Edit {company.Name} company";

            return RedirectToAction("Index");
        }


        public IActionResult Delete(int? Id)
        {
            if (Id == null || Id == 0)
            {
                return NotFound();
            }
            var Compay = _companyRepository.
                Company.Get(c => c.Id == Id);

            if (Compay == null)
                return NotFound();
            return View(Compay);
        }
        [HttpPost, ActionName("Delete")]
        public IActionResult DeletePost(int? Id)
        {
            var Compay = _companyRepository.
              Company.Get(c => c.Id == Id);

            if (Compay == null)
                return NotFound();
            _companyRepository.Company.Remove(Compay);

            TempData["Hello"] = $"We Delete {Compay.Name} Compay";

            return RedirectToAction("Index");
        }
    }
}
