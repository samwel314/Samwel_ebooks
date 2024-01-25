using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Samwel.Models.ViewModels
{
    public class ManageuserVM
    {
        public string Id { get; set; }  

        public string Name { get; set; }

        public IEnumerable<SelectListItem> RolesList { get; set; }

        public IEnumerable<SelectListItem> CompanyList { get; set; }

        public int  Company { get; set; }
        public string Role { get; set; }
    }
}
