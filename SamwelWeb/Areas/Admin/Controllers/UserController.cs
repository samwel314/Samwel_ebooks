
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Samwel.DataAccess.Data;
using Samwel.DataAccess.Repository;
using Samwel.DataAccess.Repository.IRepository;
using Samwel.Models;
using Samwel.Models.ViewModels;
using Samwel.Utility;
using System.Collections.Immutable;
using System.Linq.Expressions;
namespace SamwelWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
   [Authorize(Roles = SD.Role_Admin)]
    public class UserController : Controller
    {
        private readonly AppDbContext _UserRepository;
        public UserController(AppDbContext repository)
        {
            _UserRepository = repository;
        }
        public IActionResult Index()
        {
            return View();
        }



        #region API CALLS

        [HttpGet]
        public IActionResult GetAll()
        {
            var users = _UserRepository.AppUsers.Include(u=>u.Company).ToList(); 
            var userroles = _UserRepository.UserRoles.ToList();
            var roles = _UserRepository.Roles.ToList();
            foreach (var user in users) 
            {
                string roleId; 
                var haverole = userroles.FirstOrDefault(x => x.UserId == user.Id);
                if (haverole != null)
                {
                    roleId = haverole.RoleId;
                    user.RoleName = roles.FirstOrDefault(c => c.Id == roleId).Name;
                }

                if (user.Company == null)
                {
                    user.Company = new Company
                    {
                        Name = "Not Comapny "
                    };
                }

            }
            return
                Json(new { data = users });
        }

        [HttpPost]
        public IActionResult LockUnLock(string ? id)
        {
   
            var user = _UserRepository.AppUsers.
                FirstOrDefault(x => x.Id == id);
            if (user == null)
            {
                return Json(new { success = false, message = "Error while Lock or UnLock " }); 
            }
            if (user
                .LockoutEnd!=null && user.LockoutEnd>DateTime.Now)
            {
                user
                .LockoutEnd = DateTime.Now; 
            }
            else
            {
                user
                .LockoutEnd = DateTime.Now.AddYears(1000); 
            }
            _UserRepository.SaveChanges (); 
            return
                Json(new { success = true, message = "Ok .. " });

        }
        #endregion



        public IActionResult Manageuser (string ? id)
        {
            if (id == null)
                return NotFound(); 
           var user =  _UserRepository.AppUsers.First(x => x.Id == id); 
           var roles = _UserRepository.Roles.ToList();
           var roleid = _UserRepository.UserRoles.FirstOrDefault(x=>x.UserId == id).RoleId;

            var data = new ManageuserVM
            {
                Name = user.Name,
                RolesList = roles.Select(x => new SelectListItem
                {
                    Text = x.Name,
                    Value = x.Id
                })
                ,
                CompanyList = _UserRepository
                .Companies.Select(c => new SelectListItem
                {
                    Text = c.Name,
                    Value = c.Id.ToString()
                })
                ,
                Role = _UserRepository.Roles.First(x => x.Id == roleid).Name 
            };
            return View(data); 
        }
        [HttpPost]
        public IActionResult Manageuser(ManageuserVM manageuser)
        {
            var user = _UserRepository.AppUsers.First(x => x.Id == manageuser.Id);
            user.Name = manageuser.Name;
            var userrole =
                _UserRepository.UserRoles.First(x => x.UserId == manageuser.Id);
             _UserRepository.Remove(userrole);
            _UserRepository.SaveChanges();
            _UserRepository.UserRoles.Add
                (new Microsoft.AspNetCore.Identity.IdentityUserRole<string>()
                {
                    UserId = manageuser.Id,
                    RoleId = manageuser.Role
                }); 
            if  (manageuser.Company != 0) 
            {
                user.CompanyId = manageuser.Company; 
            }
            else
            {
                user.CompanyId = null;
            }
   
            _UserRepository.SaveChanges(); 
            return RedirectToAction("Index");
        }
    }
}
