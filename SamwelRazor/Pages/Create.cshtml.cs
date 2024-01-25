using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SamwelRazor.Data;
using SamwelRazor.Models;

namespace SamwelRazor.Pages
{
    public class CreateModel : PageModel
    {
        public Category Category { get; set; }
        private AppDbContext _context;
        public CreateModel (AppDbContext context )
        {
            _context = context; 
           // Category = new Category (); 
        }
        public void OnGet()
        {
        }
        public IActionResult OnPost(Category Category)
        {
            if (Category.Name == Category.DisplayOrder.ToString())
            {
                ModelState.AddModelError("Name", "Can Not Insert Categore and Display Oeder With Same Value");
                return Page();
            }
            if (!ModelState.IsValid)
                return Page();
            _context.Categories.Add(Category);
            _context.SaveChanges();
            TempData["Hello"] = $"We Create {Category.Name} Category";
            return RedirectToPage("Index");
        }
       
    }
}
