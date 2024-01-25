using Samwel.Models;

namespace Samwel.DataAccess.Repository.IRepository
{
    public interface IProductImageRepository : IRepository<ProductImage>
    {
        void Update(ProductImage ProductImage);
    }
}
