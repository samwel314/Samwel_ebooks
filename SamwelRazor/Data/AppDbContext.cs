using Microsoft.EntityFrameworkCore;
using SamwelRazor.Models;

namespace SamwelRazor.Data
{
	public class AppDbContext : DbContext
	{
		public DbSet<Category> Categories { get; set; }

		public AppDbContext(DbContextOptions dbContext) :
			base(dbContext)
		{

		}


	}

}
