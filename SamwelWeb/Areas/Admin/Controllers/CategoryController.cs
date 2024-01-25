
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Samwel.DataAccess.Data;
using Samwel.DataAccess.Repository.IRepository;
using Samwel.Models;
using Samwel.Utility;
using System.Linq.Expressions;
namespace SamwelWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
   [Authorize(Roles = SD.Role_Admin)]
    public class CategoryController : Controller
    {
        private readonly IUnitOfWork _categoryRepository;
        public CategoryController(IUnitOfWork repository)
        {
            _categoryRepository = repository;
        }
        public IActionResult Index()
        {
            var Categories =
                _categoryRepository.Category.GetAll(null);
            return View(Categories);
        }

        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Create(Category category)
        {
            if (category.Name == category.DisplayOrder.ToString())
                ModelState.AddModelError("Name", "Can Not Insert Categore and Display Oeder With Same Value");

            if (!ModelState.IsValid)
                return View();
            _categoryRepository.Category.Add(category);
            TempData["Hello"] = $"We Create {category.Name} Category";
            return RedirectToAction("Index");
        }

        public IActionResult Edit(int? CategoryId)
        {
            if (CategoryId == null || CategoryId == 0)
            {
                return NotFound();
            }

            var Category =
                _categoryRepository
                .Category.Get(u => u.CategoryId == CategoryId);

            if (Category == null)
                return NotFound();
            return View(Category);
        }
        [HttpPost]
        public IActionResult Edit(Category category)
        {
            if (category.Name == category.DisplayOrder.ToString())
                ModelState.AddModelError("Name", "Can Not Insert Categore and Display Oeder With Same Value");

            if (!ModelState.IsValid)
                return View();
            _categoryRepository.Category.Update(category);
            _categoryRepository.Save();
            TempData["Hello"] = $"We Edit {category.Name} Category";

            return RedirectToAction("Index");
        }

        public IActionResult Delete(int? CategoryId)
        {
            if (CategoryId == null || CategoryId == 0)
            {
                return NotFound();
            }
            var Category = _categoryRepository.
                Category.Get(c => c.CategoryId == CategoryId);

            if (Category == null)
                return NotFound();
            return View(Category);
        }
        [HttpPost, ActionName("Delete")]
        public IActionResult DeletePost(int? CategoryId)
        {
            var Category = _categoryRepository.
                Category.Get(c => c.CategoryId == CategoryId);

            if (Category == null)
                return NotFound();
            _categoryRepository.Category.Remove(Category);
            //-><-||--||||

            TempData["Hello"] = $"We Delete {Category.Name} Category";

            return RedirectToAction("Index");
        }
    }
}
