using Samwel.Models;

namespace Samwel.DataAccess.Repository.IRepository
{
    public interface ICompanyRepository : IRepository<Company>
    {
        void Update(Company Company);


    }
}
