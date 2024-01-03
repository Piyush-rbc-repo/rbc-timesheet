using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Timesheet.Database.DataInterface;

namespace Timesheet.Database.DataService
{
    public class UserData : IUserData
    {
      public string UserRolesGet(string userName)
        {
            SqlParameter user_Name = new SqlParameter("userName", userName);

            DataSet ds = SqlHelper.ExecuteDataset(CommandType.StoredProcedure, "uspUserRolesGet", new SqlParameter[]{
            user_Name
            });
            if (ds != null && ds.Tables[0].Rows.Count == 0)
            {
                return string.Empty;
            }
            else
            {
                return ds.Tables[0].Rows[0][0].ToString();
            }


        }
    }
}
