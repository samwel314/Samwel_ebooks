using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Samwel.DataAccess.Data;
using Samwel.Models;
using Samwel.Utility;

namespace Samwel.DataAccess.DbInitializer
{
    public class DbInitializer : IDbInitializer
    {
        UserManager<IdentityUser> _usermanager; 
        RoleManager<IdentityRole> _rolemanager;
        AppDbContext _db; 
        public DbInitializer(UserManager<IdentityUser> _usermanager
        , RoleManager<IdentityRole> _rolemanager,
        AppDbContext _db)
        {
            this._usermanager = _usermanager;   
            this._rolemanager = _rolemanager;   
            this._db = _db;
        }
        public void Initialize()
        {
            try
            {
                if (_db.Database.GetPendingMigrations().Count() > 0 )
                {
                    _db.Database.Migrate(); 
                }
            
            }
            catch(Exception e)
            {

            }

            if (!_rolemanager.RoleExistsAsync(SD.Role_Customer).GetAwaiter().GetResult())
            {
                _rolemanager.CreateAsync(new IdentityRole(SD.Role_Customer)).GetAwaiter().GetResult();
                _rolemanager.CreateAsync(new IdentityRole(SD.Role_Admin)).GetAwaiter().GetResult();
                _rolemanager.CreateAsync(new IdentityRole(SD.Role_Company)).GetAwaiter().GetResult();
                _rolemanager.CreateAsync(new IdentityRole(SD.Role_Employee)).GetAwaiter().GetResult();


                _usermanager.CreateAsync(new AppUser
                {
                    UserName = "zoka123@gmail.com",
                    Email = "zoka123@gmail.com",
                    Name = "marzouk rezq",
                    PhoneNumber = "1234567890",
                    StreetAddress = "almo3lmen 23",
                    City = "Assuit",
                    State = "active",
                    PostalCode = "1234567890",
                }, "*123Zoka").GetAwaiter().GetResult();

                AppUser appUser =
                    _db.AppUsers.FirstOrDefault(u => u.Email ==
                    "zoka123@gmail.com")!;
                _usermanager.AddToRoleAsync(appUser, SD.Role_Admin).GetAwaiter().GetResult();  
            }

            return; 
        }


    }
}
