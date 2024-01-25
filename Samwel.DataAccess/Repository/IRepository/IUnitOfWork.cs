using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Samwel.DataAccess.Repository.IRepository
{
    public interface IUnitOfWork
    {
        ICategoryRepository Category
        { get;}

        IProductRepository Product { get;}
        ICompanyRepository Company { get;}  

        IShoppingCartRepository ShoppingCart { get;}   
        
        IAppUserRepository AppUser { get;}    
        
        IOrderDetailsRepository OrderDetails { get;}    

        IOrderHeaderRepository OrderHeader { get;}  
            
        IProductImageRepository ProductImage { get;}
        void Save(); 
    }
}
