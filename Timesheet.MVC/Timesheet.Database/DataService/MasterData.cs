using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Timesheet.Database.DataInterface;
using Timesheet.Modal;
using System.Data;
using System.Data.SqlClient;
namespace Timesheet.Database.DataService
{
    public class MasterData : IMasterData
    {

        public CommoSaveResult InsertUpdateMaster(LookupMasterModal modal)
        {
            SqlParameter pn_Id = new SqlParameter("pn_Id",modal.n_Id);
            pn_Id.Direction = ParameterDirection.InputOutput;
            SqlParameter pn_ParentId = new SqlParameter("pn_ParentId", modal.n_ParentId);
            SqlParameter pn_RefId = new SqlParameter("pn_RefId", modal.n_RefId);
            SqlParameter ps_MasterCode = new SqlParameter("ps_MasterCode", modal.s_MasterCode);
            SqlParameter ps_MasterName = new SqlParameter("ps_MasterName", modal.s_MasterName);
            SqlParameter ps_value1 = new SqlParameter("ps_value1", modal.s_value1);
            SqlParameter ps_value2 = new SqlParameter("ps_value2", modal.s_value2);
            SqlParameter ps_value3 = new SqlParameter("ps_value3", modal.s_value3);
            SqlParameter ps_value4 = new SqlParameter("ps_value4", modal.s_value4);
            SqlParameter ps_value5 = new SqlParameter("ps_value5", modal.s_value5);
            SqlParameter ps_value6 = new SqlParameter("ps_value6", modal.s_value6);
               
            SqlParameter ps_Mode = new SqlParameter("ps_Mode", modal.Oper);
            SqlParameter pn_MakerId = new SqlParameter("pn_MakerId", modal.MakerId);
            SqlParameter pb_Active = new SqlParameter("pb_Active", modal.b_IsActive);
            SqlParameter pn_Error = new SqlParameter("pb_Error", SqlDbType.Bit);
            pn_Error.Direction = ParameterDirection.Output;
            SqlParameter ps_Msg = new SqlParameter("ps_Msg", SqlDbType.VarChar, 100);
            ps_Msg.Direction = ParameterDirection.Output;

            SqlHelper.ExecuteDataset(CommandType.StoredProcedure, Common.Constants.spGetAllMaster, new SqlParameter[] { 
            pn_Id,pn_ParentId,pn_RefId,ps_MasterCode,ps_MasterName,ps_value1,ps_value2,ps_value3,ps_value4,ps_value5,ps_value6,
            ps_Mode,pb_Active,pn_Error,ps_Msg
            });

            return new CommoSaveResult()
            {
                pn_Error = Convert.ToBoolean(pn_Error.Value),
                ps_Msg = ps_Msg.Value.ToString(),
                pn_RecordId = int.Parse(pn_Id.Value.ToString())
            };


            
        }
    }
}
