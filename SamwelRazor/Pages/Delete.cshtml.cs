using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SamwelRazor.Data;
using SamwelRazor.Models;

namespace SamwelRazor.Pages
{
    public class DeleteModel : PageModel
    {
		[BindProperty]
		public Category Category { get; set; }
		private AppDbContext _context;
		public DeleteModel(AppDbContext context)
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
		public IActionResult OnPost( int  CategoryId)
		{
			var Category = _context.Categories.	
				SingleOrDefault(c => c.CategoryId == CategoryId);
			if (Category == null)
				return NotFound();
			_context.Categories.Remove(Category);
			_context.SaveChanges();
			TempData["Hello"] = $"We Delete {Category.Name} Category";

			return RedirectToPage("Index");
		}
	}
}
