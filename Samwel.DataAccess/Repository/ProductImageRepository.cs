using Samwel.DataAccess.Data;
using Samwel.DataAccess.Repository.IRepository;
using Samwel.Models;

namespace Samwel.DataAccess.Repository
{
    public class ProductImageRepository : Repository<ProductImage>, IProductImageRepository
    {
        private AppDbContext _db;
        public ProductImageRepository(AppDbContext context)
            : base(context)
        {
            _db = context;
        }


        public void Update(ProductImage ProductImage)
        {
            _db.ProductImages.Update(ProductImage);
        }
    }
}
