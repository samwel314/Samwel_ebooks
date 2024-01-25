using Samwel.DataAccess.Data;
using Samwel.DataAccess.Repository.IRepository;
using Samwel.Models;

namespace Samwel.DataAccess.Repository
{
    public class AppUserRepository : Repository<AppUser>, IAppUserRepository
    {
        private AppDbContext _db;
        public AppUserRepository(AppDbContext context)
            : base(context)
        {
            _db = context;
        }
    }
}
