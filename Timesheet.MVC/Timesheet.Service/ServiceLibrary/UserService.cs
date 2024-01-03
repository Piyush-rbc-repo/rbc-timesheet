using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Timesheet.Service.ServiceLibrary
{
   public  class UserService :Timesheet.Service.ServiceInterface.IUserService
    {
       
         private Timesheet.Database.DataInterface.IUserData userdate = new Timesheet.Database.DataService.UserData();

         public string UserRolesGet(string userName)
           {
             return  userdate.UserRolesGet(userName);
           }


    }
}
