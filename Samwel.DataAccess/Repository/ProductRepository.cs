using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Samwel.DataAccess.Data;
using Samwel.DataAccess.Repository.IRepository;
using Samwel.Models;

namespace Samwel.DataAccess.Repository
{
    public class ProductRepository : Repository<Product>, IProductRepository
    {

        private AppDbContext _db;
        public ProductRepository(AppDbContext context)
            : base(context)
        {
            _db = context;
        }
        

        public void Update(Product product)
        {
            var item = _db.Products.FirstOrDefault(p => p.Id == product.Id);
            if (item != null) 
            {
                item.Title = product.Title;
                item.Description = product.Description;
                item.CategoryId = product.CategoryId;
                item.ListPrice = product.ListPrice; 
                item.Price = product.Price;
                item.Price50 = product.Price50;
                item.Price100 = product.Price100; 
                item.Author = product.Author;
                item.ISBN = product.ISBN;
                item.productImages = product.productImages;
                if (product.ImageUrl != null)
                    item.ImageUrl = product.ImageUrl;
            }
        }

     
    }



}
