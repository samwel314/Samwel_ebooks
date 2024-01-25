using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SamwelRazor.Data;
using SamwelRazor.Models;

namespace SamwelRazor.Pages
{
	public class IndexModel : PageModel
	{
		private AppDbContext _context; 
		public IEnumerable<Category> categories; 
		public IndexModel( AppDbContext Context)
		{
			_context = Context; 
			categories = new List<Category>();
		}

		public void OnGet()
		{
			categories = _context.Categories.AsNoTracking().ToList();
		}
	}
}