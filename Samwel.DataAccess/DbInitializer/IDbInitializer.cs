using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Samwel.DataAccess.DbInitializer
{
    public interface IDbInitializer
    {
        void Initialize();// To Create First Admin User and Roles 
    }
}
