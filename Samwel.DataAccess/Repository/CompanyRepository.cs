using Samwel.DataAccess.Data;
using Samwel.DataAccess.Repository.IRepository;
using Samwel.Models;

namespace Samwel.DataAccess.Repository
{
    public class CompanyRepository : Repository<Company>, ICompanyRepository
    {
        private AppDbContext _db;
        public CompanyRepository(AppDbContext context)
            : base(context)
        {
            _db = context;
        }
        public void Update(Company company)
        {
           _db.Companies.Update(company); 
        }

    }
}
