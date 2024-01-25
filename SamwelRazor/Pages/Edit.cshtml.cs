using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SamwelRazor.Data;
using SamwelRazor.Models;

namespace SamwelRazor.Pages
{
    public class EditModel : PageModel
    {
        public  Category Category { get; set; }
        private AppDbContext _context;
        public EditModel(AppDbContext context)
        {
            _context = context;
            // Category = new Category (); 
        }
        public IActionResult OnGet(int CategoryId)
        {
            Category = _context.Categories
                .FirstOrDefault(c => c.CategoryId == CategoryId)!;
       
            if (Category == null)
            {
                return NotFound();  
            }
            return Page();  
        }

        public IActionResult OnPost(Category Category )
        {
            if (Category.Name == Category.DisplayOrder.ToString())
            {
                ModelState.AddModelError("Name", "Can Not Insert Categore and Display Oeder With Same Value");
                return Page();
            }
            if (!ModelState.IsValid)
                return Page();
            _context.Categories.Update(Category);
            _context.SaveChanges();
            TempData["Hello"] = $"We Edit {Category.Name} Category";
            return RedirectToPage("Index");
        }
    }
}
