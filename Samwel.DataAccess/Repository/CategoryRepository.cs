using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Samwel.DataAccess.Data;
using Samwel.DataAccess.Repository.IRepository;
using Samwel.Models;

namespace Samwel.DataAccess.Repository
{
    public class CategoryRepository : Repository<Category> , ICategoryRepository
    {
        private AppDbContext _db; 
        public CategoryRepository (AppDbContext context)
            : base (context)
        {
            _db = context;  
        }
  

        public void Update(Category category)
        {
           _db.Categories.Update(category);
        }
    }
}
