using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Timesheet.Modal;
using Timesheet.Database.DataInterface;
using System.Data.SqlClient;
using System.Data;
namespace Timesheet.Database.DataService
{
   public class DataMaster : IDataMaster
    {
        public List<LookupMasterModal> GetAll()
        {
            return GetMasters(null);
        }
        public List<LookupMasterModal> GetById(int id)
        {
            return GetMasters(id);
        }
        public CommoSaveResult Add(LookupMasterModal modal)
        {
            return AddUpdateMaster(modal);
        }
        public CommoSaveResult Update(LookupMasterModal modal)
        {
            return AddUpdateMaster(modal);

        }
        public List<LookupMasterModal> Delete(int id)
        {
            return new List<LookupMasterModal>();
        }


   

       private List<LookupMasterModal> GetMasters(int? parentId)
       {

           SqlParameter ps_Mode = new SqlParameter("ps_Mode", "R");
            SqlParameter pn_Error = new SqlParameter("pb_Error", SqlDbType.Bit);
            pn_Error.Direction = ParameterDirection.Output;
            SqlParameter ps_Msg = new SqlParameter("ps_Msg", SqlDbType.Bit);
            ps_Msg.Direction = ParameterDirection.Output;
           
           SqlParameter pn_Id= new SqlParameter("pn_Id", parentId);
            
            System.Data.DataSet ds = SqlHelper.ExecuteDataset( CommandType.StoredProcedure, Timesheet.Common.Constants.spGetAllMaster, new SqlParameter[]{
           ps_Mode ,pn_Error ,ps_Msg ,pn_Id
           });
            if (ds != null)
            {
                if (ds.Tables.Count > 0)
                {

                    return ds.Tables[0].Rows.Cast<DataRow>().Select(x => new LookupMasterModal()
                    {
                        n_Id = (int)x["id"],
                        n_ParentId =x.Field<int?>("ParentId"),
                        n_RefId = x.Field<int?>("RefId"),
                        s_MasterCode = x["MasterCode"].ToString(),
                        s_MasterName = x["MasterName"].ToString(),
                        value1 = x["value1"].ToString(),
                        value2 = x["value2"].ToString(),
                        value3 = x["value3"].ToString(),
                        s_value4 = x["value4"].ToString(),
                        s_value5 = x["value5"].ToString(),
                        s_value6 = x["value6"].ToString(),
                        CRTypeName = x["CRTypeName"].ToString(),// CrtypeName recieved from database
                        /*TaskName = x["TaskName"].ToString(),//TaskName recieved from database*/
                        ParentName = x["ReferenceName"].ToString(),
                        b_IsActive = x.Field<bool>("IsActive"),


                    }).ToList();




                }


                return new List<LookupMasterModal>();
            }
            else
            {
                return new List<LookupMasterModal>();
            }

       }


       private CommoSaveResult AddUpdateMaster(LookupMasterModal modal)
       {
           SqlParameter pn_Id = new SqlParameter("pn_Id", modal.n_Id);
           pn_Id.Direction = ParameterDirection.InputOutput;
           SqlParameter pn_ParentId = new SqlParameter("pn_ParentId", modal.n_ParentId);
           SqlParameter pn_RefId = new SqlParameter("pn_RefId", modal.n_RefId);
           SqlParameter ps_MasterCode = new SqlParameter("ps_MasterCode", modal.s_MasterCode);
           SqlParameter ps_MasterName = new SqlParameter("ps_MasterName", modal.s_MasterName);
           SqlParameter ps_value1 = new SqlParameter("ps_value1", modal.value1);
           SqlParameter ps_value2 = new SqlParameter("ps_value2", modal.value2);
           SqlParameter ps_value3 = new SqlParameter("ps_value3", modal.value3);
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
