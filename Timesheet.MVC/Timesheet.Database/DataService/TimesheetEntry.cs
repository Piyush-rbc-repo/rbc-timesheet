using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Timesheet.Database.DataInterface;
using Timesheet.Modal;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Xml.Serialization;
using System.IO;
using System.Web;
using Timesheet.Common;
using System.Net.Mail;
namespace Timesheet.Database.DataService
{
    public class TimesheetEntry  : IDataTimesheetEntryService
    {
        public List<TimesheetModal> GetAll()
        {
            return new List<TimesheetModal>();
        }
        public List<TimesheetModal> GetById(int id)
        {
            return new List<TimesheetModal>();
        }
        public CommoSaveResult Add(TimesheetCreateModal modal)
        {
            return AddUpdate(modal,"I",true);
           
        }
        public CommoSaveResult Update(TimesheetCreateModal modal, string comm)
        {
            return AddUpdate(modal, comm,true);

        }
        public TimesheetSubmitValidationResult DeleteSubmit(string ids, bool Delete, bool submit, int userid,bool partialSubmit= false)
        {
            SqlParameter pn_MakerId = new SqlParameter("pn_MakerId", userid);
            SqlParameter pn_RecordIds = new SqlParameter("pn_RecordIds", ids);
            SqlParameter pb_ToDelete = new SqlParameter("pb_ToDelete", Delete);
            SqlParameter pb_ToSubmit = new SqlParameter("pb_ToSubmit", submit);
            SqlParameter pn_partialSubmit = new SqlParameter("pn_partialSubmit", partialSubmit);
            SqlParameter pn_Error = new SqlParameter("pn_Error", SqlDbType.Bit);
            pn_Error.Direction = ParameterDirection.Output;
            SqlParameter ps_Msg = new SqlParameter("ps_Msg", SqlDbType.Bit);
            ps_Msg.Direction = ParameterDirection.Output;

          DataSet ds=  SqlHelper.ExecuteDataset(CommandType.StoredProcedure,Timesheet.Common.Constants.spDeleteSubmit_Timesheet, new SqlParameter[] { 
            pn_MakerId,
            pn_RecordIds ,
            pb_ToDelete,
            pb_ToSubmit,
            pn_partialSubmit
            //pn_Error ,
            //ps_Msg
           
            });

          TimesheetSubmitValidationResult result = new TimesheetSubmitValidationResult();
            var ResultTablePos = 0;
            if(ds.Tables.Count>1)
            {
                ResultTablePos=1;
                result.timesheetSubmitValidationOutput = ds.Tables[0].Rows.Cast<DataRow>().ToList().Select(x => new TimesheetSubmitValidationResultModal()
                {
                    n_ResourceId = x.Field<int>("n_ResourceId"),
                    FullName = x.Field<string>("FullName"),
                    Date = x.Field<string>("Date"),
                    Cause = x.Field<string>("Cause")

                }).ToList();

            }
            result.SaveResult= ds.Tables[ResultTablePos].Rows.Cast<DataRow>().ToList().Select(x=>new CommoSaveResult()
            {
                pn_Error = x.Field<bool>("pn_Error"),
                ps_Msg = x.Field<string>("ps_Msg")
                
            }).FirstOrDefault();


            return result;

            
        }
        public List<SelectListModal> GetAllDropdown()
        {
            SqlParameter ps_Mode = new SqlParameter("ps_Mode", "R");
            SqlParameter pn_Error = new SqlParameter("pb_Error", SqlDbType.Bit);
            pn_Error.Direction = ParameterDirection.Output;
            SqlParameter ps_Msg = new SqlParameter("ps_Msg", SqlDbType.Bit);
            ps_Msg.Direction = ParameterDirection.Output;

            System.Data.DataSet ds = SqlHelper.ExecuteDataset( CommandType.StoredProcedure, Timesheet.Common.Constants.spGetAllMaster, new SqlParameter[]{
           ps_Mode ,pn_Error ,ps_Msg 
           });
            if(ds != null)
            {
                if(ds.Tables.Count>0)
                {
                   
                        return ds.Tables[0].Rows.Cast<DataRow>().Where(x=>Convert.ToBoolean(x["IsActive"].ToString())==true).Select(x=> new SelectListModal(){
                            MasterName = x["ParentName"].ToString(),
                            Text = x["MasterName"].ToString(),
                            Value = x["Id"].ToString(),
                            s_Value3 = x["Value3"].ToString(),
                            ParentId = x["ParentId"].ToString(),
                            RefId = x["RefId"].ToString(),
                            s_Value1= x["value1"].ToString(), // Added by Sasmita  for Resource Role, required in Invoice Excel
                            s_Value2 = x["value2"].ToString(), // Added by Sasmita  for Resource Per Day Cost, required in Invoice Excel
                            s_Value4 = x["value4"].ToString(), // Added by Sasmita  for Resource Per Day OLS Rate, required in Invoice Excel
                            s_Value5 = x["value5"].ToString(), // Added by Sasmita for Resource RBC Role, required in Invoice Excel
                            s_Value6 = x["value6"].ToString() // Added by Sasmita for Resource Type (Support / TnM / Other), required in Invoice Excel
                        }).ToList();

                   
                    

                }

            }
            
            throw new Exception("No data found for select list");

            
        }
        public List<TimesheetSearchResultModal> Search(string Fromdate, string ToDate, int? activityId, int? tasksId, int? projectId, bool? billable, int? resourceId, int? crNumberId, int? crProjectId  )
        {
            SqlParameter ps_Fomdate = new SqlParameter("ps_FromDate", Fromdate);
            SqlParameter ps_ToDate = new SqlParameter("ps_ToDate ", ToDate);    
            SqlParameter pn_activityId = new SqlParameter("pn_ActivityId", activityId);
            SqlParameter pn_tasksId = new SqlParameter("pn_TasksId", tasksId);   // Added By Piyush
            SqlParameter pn_projectId = new SqlParameter("pn_ProjectId", projectId);
            SqlParameter pn_billable = new SqlParameter("pn_Billable", billable);
            SqlParameter pn_resourceId = new SqlParameter("pn_Resource", resourceId);
            SqlParameter pn_crNumberId = new SqlParameter("pn_crNumberId", crNumberId);
            SqlParameter pn_crProjectId = new SqlParameter("pn_crProjectId", crProjectId);
            SqlParameter pn_Error = new SqlParameter("pb_Error", SqlDbType.Bit);
            pn_Error.Direction = ParameterDirection.Output;
            SqlParameter ps_Msg = new SqlParameter("ps_Msg", SqlDbType.Bit);
            ps_Msg.Direction = ParameterDirection.Output;


            DataSet ds = SqlHelper.ExecuteDataset(CommandType.StoredProcedure, Timesheet.Common.Constants.spSearchTimeSheet, new SqlParameter[]{
            ps_Fomdate,ps_ToDate,pn_activityId,pn_tasksId,pn_projectId,pn_billable,pn_resourceId,pn_crNumberId,pn_crProjectId
            ,pn_Error,ps_Msg
            });

            if (ds != null)
            {

                return ds.Tables[0].Rows.Cast<DataRow>().ToList().Select(x => new TimesheetSearchResultModal() { 
                n_id = x.Field<int>("n_Id"),
                ActivityDate = x.Field<string>("ActivityDate"),
                ResourceName = x.Field<string>("ResourceName"),
                ProjectName = x.Field<string>("ProjectName"),
                Activity = x.Field<string>("Activity"),
                Tasks = x.Field<string>("Tasks"),                    // Added By Piyush 
                Billable = x.Field<string>("Billable"),
                Comments = x.Field<string>("Comments"),
                CreatedOn = x.Field<DateTime>("CreatedOn"),
                CrNumber = x.Field<string>("CrNumber"),
                CrTypeName = x.Field<string>("CrTypeName"),           // Added by Piyush CRTypeName
                Efforts = x.Field<decimal>("Efforts"),
                Efforts_Days = x.Field<decimal>("Efforts_days"),
                Status = x.Field<string>("Status"),
                SubActivity = x.Field<string>("SubActivity"),
                n_status = x.Field<int>("n_status"),
                RejectionReason = x.Field<string>("RejectionReason"),
                VisibleToUser = x.Field<bool>("VisibleToUser"),
                Location = x.Field<string>("Location"),
                ActualEfforts = x.Field<decimal>("ActualEfforts")
                }).ToList();

            }
            else
                return new List<TimesheetSearchResultModal>();

            
        }

        private CommoSaveResult AddUpdate(TimesheetCreateModal modal, string mode , bool visibleToUser)
        {


               
                SqlParameter pd_ActivityDate = new SqlParameter("pd_ActivityDate", modal.Activitydate);
                SqlParameter pn_ResourceId = new SqlParameter("pn_ResourceId", modal.ResourceId);
                SqlParameter pn_ProjectId = new SqlParameter("pn_ProjectId", modal.ProjectId);
                SqlParameter pn_CrId = new SqlParameter("pn_CrId", modal.CrNumberId);
                SqlParameter pn_ActivityId = new SqlParameter("pn_ActivityId", modal.ActivityId);
                SqlParameter pn_TasksId = new SqlParameter("pn_TasksId", modal.TasksId);                   //Added by Piyush to send SubTasks Id
                SqlParameter ps_SubActivity = new SqlParameter("ps_SubActivity", modal.SubActivity);
                SqlParameter pn_Efforts = new SqlParameter("pn_Efforts", modal.Efforts);
                SqlParameter pn_ActualEfforts = new SqlParameter("pn_ActualEfforts", modal.Efforts);
                SqlParameter pn_IsBillable  = new SqlParameter("pn_IsBillable", modal.IsBillable);
                
                SqlParameter pb_IsSubmitted = new SqlParameter("pb_IsSubmitted", modal.IsSubmit);
                SqlParameter ps_Comments = new SqlParameter("ps_Comments", modal.Comments);
                SqlParameter pn_MakerId = new SqlParameter("pn_MakerId", modal.ResourceId);
                SqlParameter ps_Mode = new SqlParameter("ps_Mode", mode);
                SqlParameter ps_VisibleToUser = new SqlParameter("ps_VisibleToUser", visibleToUser);
                SqlParameter pn_Error = new SqlParameter("pn_Error", SqlDbType.Bit);
                pn_Error.Direction = ParameterDirection.Output;
                SqlParameter ps_Msg = new SqlParameter("ps_Msg", SqlDbType.VarChar, 100);
                ps_Msg.Direction = ParameterDirection.Output;
                SqlParameter pn_RecordId = new SqlParameter("pn_RecordId", SqlDbType.Int);
                pn_RecordId.Value = (modal.Id.HasValue ? modal.Id : null);
   
                pn_RecordId.Direction = ParameterDirection.InputOutput;
                SqlHelper.ExecuteDataset(CommandType.StoredProcedure, Common.Constants.spAddUpdate_TimesheetEntry, new SqlParameter[] { 
            pd_ActivityDate,
            pn_ResourceId,
            pn_ProjectId,
            pn_CrId,
            pn_ActivityId,
            pn_TasksId,
            ps_SubActivity,
            pn_Efforts,
            pn_ActualEfforts,
            pn_IsBillable,
            pb_IsSubmitted,
            ps_Comments,
            ps_Mode,
            ps_VisibleToUser,
            pn_MakerId,
            pn_Error,
            ps_Msg,
            pn_RecordId
            });

                return new CommoSaveResult()
                {
                    pn_Error = Convert.ToBoolean(pn_Error.Value),
                    ps_Msg = Convert.ToString(ps_Msg.Value),
                    pn_RecordId = int.Parse(pn_RecordId.Value.ToString())
                };
 


        }


        public List<TimesheetSearchResultModal> GetDashboard(ReportSearchModal modal)
        {
            //SqlParameter pn_status = new SqlParameter("pn_status", (modal.statusId == true ? 2 : 0));
            SqlParameter pn_resourceId = new SqlParameter("pn_resourceId", modal.ResourceId);
            SqlParameter pn_MonthId = new SqlParameter("pn_MonthId", modal.MonthId);
            SqlParameter pn_WeekId = new SqlParameter("pn_WeekId", modal.WeekId);
            SqlParameter pn_status = new SqlParameter("pn_status", (modal.statusId==true?2:0));
            DataSet ds = SqlHelper.ExecuteDataset(CommandType.StoredProcedure, Timesheet.Common.Constants.spDashboard, new SqlParameter[]{
            pn_resourceId ,pn_MonthId,pn_WeekId ,pn_status 
            });

            if (ds != null)
            {

                return ds.Tables[0].Rows.Cast<DataRow>().ToList().Select(x => new TimesheetSearchResultModal()
                {
                    n_id = x.Field<int>("n_Id"),
                    ActivityDate = x.Field<string>("ActivityDate"),
                    ResourceName = x.Field<string>("ResourceName"),
                    ProjectName = x.Field<string>("ProjectName"),
                    ActivityId = x.Field<int>("ActivityId"),
                    Activity = x.Field<string>("Activity"),
                    Billable = x.Field<string>("Billable"),
                    Comments = x.Field<string>("Comments"),
                    CreatedOn = x.Field<DateTime>("CreatedOn"),
                    CrNumber = x.Field<string>("CrNumber"),
                    Efforts = x.Field<decimal>("Efforts"),
                    Efforts_Days = x.Field<decimal>("Efforts_days"),
                    Status = x.Field<string>("Status"),
                    SubActivity = x.Field<string>("SubActivity"),
                    ProjectId = x.Field<int>("ProjectId"),
                    CrNumberId = x.Field<int?>("CrNumberId"),
                    ResoureId = x.Field<int>("ResoureId"),
                    n_status = x.Field<int?>("n_status"),
                    Approved_Cost = x.Field<decimal>("Cost"),
                    ManagerId = x.Field<int?>("ManagerId"), // modified to Nullable Int by Sasmita to avoid NULL exception while exporting data on Dashboard page
                    ActualEfforts=  x.Field<decimal>("ActualEfforts"),
                    VisibleToUser = x.Field<bool>("VisibleToUser"),
                    Location = x.Field<string>("Location"), //added by Sasmita to append Onshore/Offshore dynamically in excel filename on Dashboard page
                    CrLongName = x.Field<string>("CrLongName"), // added by Sasmita to display crLongName in Invoice Excel Report
                    OnsiteManagerName = x.Field<string>("OnsiteManagerName"), // added by Sasmita to display Onsite Manager Name in Invoice Excel Report
                    ProjectType = x.Field<string>("ProjectType") //Added by Sasmita to retrive Support projects
                }).ToList();

            }
            else
                return new List<TimesheetSearchResultModal>();

            
        }
        public ProjectDashboardModal GetProjectCrBasedDashboard(ProjectCrBasedSearchModal modal)
        {
            SqlParameter pn_CrId = new SqlParameter("pn_CrId", modal.CrId);
            SqlParameter pn_ProjectId = new SqlParameter("pn_ProjectId", modal.ProjectId);
            SqlParameter pn_month = new SqlParameter("pn_month", modal.month);
            

            DataSet ds = SqlHelper.ExecuteDataset(CommandType.StoredProcedure, Timesheet.Common.Constants.spProjectCrLevelDashboard, new SqlParameter[] { 
            
            pn_CrId,pn_ProjectId,pn_month
            });
            ProjectDashboardModal returnmodal = new ProjectDashboardModal();
            returnmodal.resourceCostModal = ds.Tables[0].Rows.Cast<DataRow>().ToList().Select(x=> new ResourceCostModal()
                {
                    FullName = x.Field<string>("FullName"),
                    Category = x.Field<string>("Category"),
                    CostPerHr = x.Field<decimal?>("CostPerHr"),
                    CostThisMonth = x.Field<decimal>("CostThisMonth"),
                    EarlierCost = x.Field<decimal>("EarlierCost"),
                    TotalHrs = x.Field<decimal>("TotalHrs"),
                    TotalCost = x.Field<decimal>("TotalCost"),
                    TotalHrsThisMonth = x.Field<decimal>("TotalHrsThisMonth"),
                    TotalBilledThisMonth = x.Field<decimal>("TotalBilledThisMonth"),
                    TotalUnBilledHrsThisMonth = x.Field<decimal>("TotalUnBilledHrsThisMonth"),
                    TotalHrsOtherMonths= x.Field<decimal>("TotalHrsOtherMonths"),
                    TotalBilledOtherMonths= x.Field<decimal>("TotalBilledOtherMonths"),
                    TotalUnBilledHrsOtherMonths= x.Field<decimal>("TotalUnBilledHrsOtherMonths"),
                    

                }).ToList();

            returnmodal.costModal= ds.Tables[1].Rows.Cast<DataRow>().ToList().Select(x=> new CostModal()
                {
                    Name=x.Field<string>("Name"),
                    SDLC= x.Field<decimal?>("SDLC"),
                    Dev= x.Field<decimal?>("Dev"),
                    QA= x.Field<decimal?>("QA")
                    
                }).FirstOrDefault();

            return returnmodal;



        }

        public CommoSaveResult ApproveReject(TimesheetApproverModal modal, int userid ,bool insertRecords=false)
        {
            SqlParameter ps_Ids = new SqlParameter("ps_Ids", modal.Ids);
            SqlParameter pb_Status = new SqlParameter("pb_Status", modal.IsApproved);
            SqlParameter ps_RejectionReason = new SqlParameter("ps_RejectionReason", modal.RejectionReason);
            SqlParameter pn_MakerId = new SqlParameter("pn_MakerId", userid);
            SqlParameter pn_insertRecords = new SqlParameter("pn_insertRecords", insertRecords);
            SqlParameter pn_Error = new SqlParameter("pb_Error", SqlDbType.Bit);
            pn_Error.Direction = ParameterDirection.Output;
            SqlParameter ps_Msg = new SqlParameter("ps_Msg", SqlDbType.VarChar, 100);
            ps_Msg.Direction = ParameterDirection.Output;

            SqlHelper.ExecuteDataset(CommandType.StoredProcedure, Common.Constants.spApproveRejectTimesheet, new SqlParameter[] { 
            ps_Ids,
            pb_Status,ps_RejectionReason,pn_MakerId,pn_insertRecords,pn_Error,ps_Msg
            });           

            return new CommoSaveResult()
            {
                pn_Error = Convert.ToBoolean(pn_Error.Value),
                ps_Msg = ps_Msg.Value.ToString()
                
            };

        }
        
        public CommoSaveResult UpdateRecordFromApproverDashboard(List<TimesheetSearchResultModal> modal, int userid)
        {
            SqlParameter px_XmlData = new SqlParameter("px_XmlData", modal.CreateXml() );
            SqlParameter pn_makerId		= new SqlParameter("pn_makerId",userid);
              SqlParameter pn_Error = new SqlParameter("pb_Error", SqlDbType.Bit);
            pn_Error.Direction = ParameterDirection.Output;
            SqlParameter ps_Msg = new SqlParameter("ps_Msg", SqlDbType.VarChar, 100);
            ps_Msg.Direction = ParameterDirection.Output;

             SqlHelper.ExecuteDataset(CommandType.StoredProcedure, Common.Constants.spUpdateTimeSheetDataByApprovar, new SqlParameter[] { 
                px_XmlData,pn_makerId,pn_Error,ps_Msg
            });
            
             return new CommoSaveResult()
            {
                pn_Error = Convert.ToBoolean(pn_Error.Value),
                ps_Msg = ps_Msg.Value.ToString()
                
            };
        }
        public TimesheetFileUploadResultModal UploadCsv(List<TimesheetFileUploadModal> modal, int Userid, bool overwriteExistsing)
        {

            SqlParameter pn_MakerId = new SqlParameter("pn_MakerId", Userid);
            SqlParameter px_Data = new SqlParameter("px_Data", modal.CreateXml());
            SqlParameter pb_withOverWrite = new SqlParameter("pb_withOverWrite",overwriteExistsing);
            SqlParameter pn_Error = new SqlParameter("pb_Error", SqlDbType.Bit);
            pn_Error.Direction = ParameterDirection.Output;
            SqlParameter ps_Msg = new SqlParameter("ps_Msg", SqlDbType.VarChar, 100);
            ps_Msg.Direction = ParameterDirection.Output;

            DataSet ds = SqlHelper.ExecuteDataset(CommandType.StoredProcedure, Timesheet.Common.Constants.spBulkSaveTimesheet, new SqlParameter[] { 
            
            pn_MakerId,px_Data,pb_withOverWrite,pn_Error,ps_Msg
            });
            TimesheetFileUploadResultModal returnmodal = new TimesheetFileUploadResultModal();
            if(ds!=null)
            {
                if(ds.Tables.Count>0)
                {
                    returnmodal.timesheetFileUploadModal = ds.Tables[0].Rows.Cast<DataRow>().ToList().Select(x => new TimesheetSearchResultModal()
                    {
                        n_id = x.Field<int>("N_Id"),
                        ActivityDate = x.Field<string>("ActivityDate"),
                        ResourceName = x.Field<string>("ResourceName"),
                        ProjectName = x.Field<string>("Project"),
                        CrNumber = x.Field<string>("CRNumber"),
                        Activity = x.Field<string>("Activity"),
                        SubActivity = x.Field<string>("SubActivity"),
                        Efforts = x.Field<decimal>("Efforts"),
                        Efforts_Days = x.Field<decimal>("Efforts_days"),
                        Billable = x.Field<string>("BillableFlag"),
                        Status = x.Field<string>("Status"),
                        n_status = x.Field<int>("n_Status")


                    }).ToList();

                }

            }
            
            

            returnmodal.result = new CommoSaveResult()
                {
                    pn_Error = Convert.ToBoolean(pn_Error.Value),
                    ps_Msg = ps_Msg.Value.ToString()
                };

            return returnmodal;

        }


        //Added by Sasmita | Extract DefaulterList functionality
        public List<TimesheetDefaulterListModal> GetDefaulterList(string Fromdate, string ToDate)
        {

            SqlParameter ps_Fomdate = new SqlParameter("ps_Fromdate", Fromdate);
            SqlParameter ps_ToDate = new SqlParameter("ps_Todate", ToDate);
        
            DataSet ds = SqlHelper.ExecuteDataset(CommandType.StoredProcedure, Timesheet.Common.Constants.spGetDefaulterList, new SqlParameter[]{
            ps_Fomdate ,ps_ToDate});

            if (ds != null)
            {

                return ds.Tables[0].Rows.Cast<DataRow>().ToList().Select(x => new TimesheetDefaulterListModal()
                {
                    ResourceID = x.Field<string>("ResourceID"),
                    ResourceName = x.Field<string>("ResourceName"),
                    MissingDate = x.Field<string>("MissingDate")
                   
                }).ToList();

            }
            else
                return new List<TimesheetDefaulterListModal>();


        }
    }
}
