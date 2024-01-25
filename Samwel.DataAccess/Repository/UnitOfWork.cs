using Samwel.DataAccess.Data;
using Samwel.DataAccess.Repository.IRepository;
using Samwel.Models;

namespace Samwel.DataAccess.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private AppDbContext _db;
        public ICategoryRepository Category { get; private set; }

        public IProductRepository Product { get; private set; } 

        public ICompanyRepository Company { get; private set; }

        public IShoppingCartRepository ShoppingCart{get; private set;}

        public IAppUserRepository AppUser { get; private set;  }

        public IOrderHeaderRepository OrderHeader { get; private set; }

        public IOrderDetailsRepository OrderDetails { get; private set; }

        public IProductImageRepository ProductImage {get; private set;} 

        public UnitOfWork(AppDbContext context)
        {
            _db = context;
            Category = new CategoryRepository(_db);
            Product = new ProductRepository(_db);
            Company = new CompanyRepository(_db);
            ShoppingCart = new ShoppingCartRepository(_db); 
            AppUser = new AppUserRepository(_db);   
            OrderHeader = new OrderHeaderRepository(_db);
            OrderDetails = new OrderDetailsRepository(_db);
            ProductImage = new ProductImageRepository(_db); 
        }


        public void Save()
        {
            _db.SaveChanges();
        }
    }
}