using PMGSY.Areas.PMIS.Models;
using PMGSY.Common;
using PMGSY.Extensions;
using PMGSY.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.SqlServer;
using System.Data.Entity.Validation;
//using System.Data.Entity.Core.Objects.SqlClient;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Transactions;
using System.Web;
using System.Web.Mvc;

namespace PMGSY.Areas.PMIS.DAL
{
    public class PMISDAL : IPMISDAL
    {
        PMGSYEntities dbContext = null;



        #region PMIS BRIDGE DAL 

        public Array PMISBridgeListDAL(int state, int district, int block, int sanction_year, int batch, int? page, int? rows, string sidx, string sord, out long totalRecords)
        {

            try
            {
                dbContext = new PMGSYEntities();

                var PMISBridgeList = dbContext.USP_PMS_BRIDGE_LIST(state, district, block, sanction_year, batch).ToList<USP_PMS_BRIDGE_LIST_Result>();

                var resultList = new List<PMISBridgeDAL>();
                string ReviseStatus = "R";
                foreach (var item in PMISBridgeList)
                {
                    var RoadCode = dbContext.PMIS_PLAN_MASTER.Where(x => x.IMS_PR_ROAD_CODE == item.PrRoadCode).Select(c => c.IMS_PR_ROAD_CODE).FirstOrDefault();
                    var RevisePlanRoadCode = dbContext.PMIS_PLAN_MASTER.Where(x => x.IMS_PR_ROAD_CODE == item.PrRoadCode & x.IS_LATEST == "Y" & x.IS_FINALISED == "Y").Select(c => c.IMS_PR_ROAD_CODE).FirstOrDefault();
                    var PlanId = dbContext.PMIS_PLAN_MASTER.Where(x => x.IMS_PR_ROAD_CODE == item.PrRoadCode && x.IS_LATEST == "Y").Select(c => c.PLAN_ID).FirstOrDefault();
                    var FinalizeStatus = dbContext.PMIS_PLAN_MASTER.Where(x => x.PLAN_ID == PlanId && x.IMS_PR_ROAD_CODE == item.PrRoadCode && x.IS_LATEST == "Y").Select(c => c.IS_FINALISED).FirstOrDefault();
                    // var FinalizeRoadCode = dbContext.PMIS_PLAN_MASTER.Where(z => z.IMS_PR_ROAD_CODE == item.PrRoadCode & z.IS_LATEST == "Y" & z.IS_FINALISED == "N").Select(y => y.IMS_PR_ROAD_CODE).FirstOrDefault();

                    // added by rohit borse on 19-05-2023
                    bool IsBridge_ProgressEntered = false;
                    bool IsBridge_QCREntered_andFinalize = false;

                    if (PMGSYSession.Current.PMGSYScheme == 4 && item.ProposalType.Trim().Equals("L"))
                    {
                        IsBridge_ProgressEntered = dbContext.EXEC_LSB_MONTHLY_STATUS.Where(r => r.IMS_PR_ROAD_CODE == item.PrRoadCode).Any() ? true : false;

                        if (IsBridge_ProgressEntered)
                        {
                            IsBridge_QCREntered_andFinalize = dbContext.QM_QCR_DETAILS.Where(s => s.IMS_PR_ROAD_CODE == item.PrRoadCode).Any()
                                                    ? (dbContext.QM_QCR_DETAILS.Where(s => s.IMS_PR_ROAD_CODE == item.PrRoadCode && (s.IS_FINALIZE == null || s.IS_FINALIZE.Trim().Equals("N"))).Count() > 0 ? true : false)
                                                    : true;
                        }
                    }

                    resultList.Add(new PMISBridgeDAL
                    {
                        PlanId = PlanId,
                        StateName = item.MAST_STATE_NAME,
                        DistrictName = item.MAST_DISTRICT_NAME,
                        BlockName = item.BlockName,
                        PackageName = item.PackageId,
                        SanctionYear = (item.SanctionYear).ToString() + "-" + (item.SanctionYear + 1).ToString().Substring(2, 2),
                        SanctionDate = ((item.IMS_SANCTIONED_DATE).Value).ToShortDateString(),
                        BatchName = item.MAST_BATCH_NAME,
                        SanctionLength = (item.SanctionLength).ToString(),
                        AgreementNo = item.TEND_AGREEMENT_NUMBER,
                        AgreementCost = item.TEND_AGREEMENT_AMOUNT.ToString(),
                        MordShare = (item.Mord_Share ?? default(decimal)).ToString(),
                        StateShare = item.State_share.ToString(),
                        TotalSanctionedCost = item.TOTAL_COST.ToString(),
                        AgreementStartDate = (item.TEND_AGREEMENT_START_DATE == null) ? "NULL" : ((item.TEND_AGREEMENT_START_DATE).Value).ToShortDateString(),
                        AgreementEndDate = (item.TEND_AGREEMENT_END_DATE == null) ? "NULL" : ((item.TEND_AGREEMENT_END_DATE).Value).ToShortDateString(),
                        LSBName = item.LSBName,
                        IMS_PR_RoadCode = item.PrRoadCode.ToString(),
                        IsPlanAvaliable = RoadCode == 0 ? item.PrRoadCode.ToString() + "$" : RoadCode.ToString(),
                        IsFinalize = RoadCode == 0 ? "-" : RevisePlanRoadCode != 0 ? RevisePlanRoadCode.ToString() + "$" : item.PrRoadCode.ToString(),
                        IsRevisePlan = RevisePlanRoadCode == 0 ? " " : RevisePlanRoadCode.ToString(),
                        IsActualsAvaliable = RoadCode == 0 ? "" : RoadCode.ToString(),
                        ProgressStatus = item.ProgressStatus,
                        ActualLock = item.ProgressStatus,
                        IsFinalized = FinalizeStatus,

                        // added by rohit borse on 19-05-2023
                        Is_bridgeProgress_Entered = IsBridge_ProgressEntered,
                        Is_bridgeQCR_Entered_andFinalize = IsBridge_QCREntered_andFinalize,
                    });
                }

                totalRecords = PMISBridgeList.Count();

                return resultList.Select(BridgeDetails => new
                {
                    cell = new[] {
                            BridgeDetails.StateName,
                            BridgeDetails.DistrictName,
                            BridgeDetails.BlockName,
                            BridgeDetails.PackageName,
                            BridgeDetails.SanctionYear,
                            BridgeDetails.SanctionDate,
                            BridgeDetails.BatchName,
                            BridgeDetails.SanctionLength,
                            BridgeDetails.AgreementNo,
                            BridgeDetails.AgreementCost,
                            BridgeDetails.MordShare,
                            BridgeDetails.StateShare,
                            BridgeDetails.TotalSanctionedCost,
                            BridgeDetails.LSBName,

                            // RoadDetails.ActualLock == "C" ? "<center><table><tr><td  style='border:none'><span class='ui-icon ui-icon-locked ui-align-center ' title='Add Project Plan';'></span></td></tr></table></center>"
                        //:RoadDetails.IsPlanAvaliable.EndsWith("$") ? "<center><table><tr><td  style='border:none'><span class='ui-icon ui-icon-circle-plus ' title='Add Project Plan' onClick =AddProjectPlan('"+ URLEncrypt.EncryptParameters1(new string[]{"RoadCode =" + RoadDetails.IMS_PR_RoadCode.ToString().Trim(),"StateShare =" + RoadDetails.StateShare.ToString().Trim(),"MordShare =" + RoadDetails.MordShare.ToString().Trim(),"TotalSanctionedDate =" + RoadDetails.TotalSanctionedCost.ToString().Trim()})+"');></span></td></tr></table></center>"//RoadDetails.IsPlanAvaliable 
                        //: PMGSYSession.Current.RoleCode==36 ? "<center><table><tr><td  style='border:none;cursor:pointer'><span class='ui-icon ui-icon-pencil' title='Edit Project Plan' onClick =EditProjectPlan('"+ URLEncrypt.EncryptParameters1(new string[]{"RoadCode =" + RoadDetails.IsPlanAvaliable.ToString().Trim(),"StateShare =" + RoadDetails.StateShare.ToString().Trim(),"MordShare =" + RoadDetails.MordShare.ToString().Trim(),"TotalSanctionedDate =" + RoadDetails.TotalSanctionedCost.ToString().Trim()})+"');></span></td><td style='border:none;cursor:pointer'><span class='ui-icon ui-icon-search' title='View Project Plan' onClick ='ViewProjectPlan(\"" + URLEncrypt.EncryptParameters1(new string[]{"RoadCode =" + RoadDetails.IsPlanAvaliable.ToString().Trim(),"StateShare =" + RoadDetails.StateShare.ToString().Trim(),"MordShare =" + RoadDetails.MordShare.ToString().Trim(),"TotalSanctionedDate =" + RoadDetails.TotalSanctionedCost.ToString().Trim()}) + "\");'></span></td><td style='border:none;cursor:pointer'></td></tr></table></center>"
                        //:RoadDetails.IsFinalize.Contains("$") ? "<center><table><tr><td style='border:none;cursor:pointer'><span class='ui-icon ui-icon-search' title='View Project Plan' onClick =ViewProjectPlan('"+ URLEncrypt.EncryptParameters1(new string[]{"RoadCode =" + RoadDetails.IsPlanAvaliable.ToString().Trim(),"StateShare =" + RoadDetails.StateShare.ToString().Trim(),"MordShare =" + RoadDetails.MordShare.ToString().Trim(),"TotalSanctionedDate =" + RoadDetails.TotalSanctionedCost.ToString().Trim()})+"');></span></td></tr></table></center>"
                        //:"<center><table><tr><td  style='border:none;cursor:pointer'><span class='ui-icon ui-icon-pencil' title='Edit Project Plan' onClick =EditProjectPlan('"+ URLEncrypt.EncryptParameters1(new string[]{"RoadCode =" + RoadDetails.IsPlanAvaliable.ToString().Trim(),"StateShare =" + RoadDetails.StateShare.ToString().Trim(),"MordShare =" + RoadDetails.MordShare.ToString().Trim(),"TotalSanctionedDate =" + RoadDetails.TotalSanctionedCost.ToString().Trim()})+"');></span></td><td style='border:none;cursor:pointer'><span class='ui-icon ui-icon-search' title='View Project Plan' onClick ='ViewProjectPlan(\"" + URLEncrypt.EncryptParameters1(new string[]{"RoadCode =" + RoadDetails.IsPlanAvaliable.ToString().Trim(),"StateShare =" + RoadDetails.StateShare.ToString().Trim(),"MordShare =" + RoadDetails.MordShare.ToString().Trim(),"TotalSanctionedDate =" + RoadDetails.TotalSanctionedCost.ToString().Trim()}) + "\");'></span></td><td style='border:none;cursor:pointer'><span class='ui-icon ui-icon-trash' title='Delete Project Plan' onClick ='DeleteProjectPlan(\"" + URLEncrypt.EncryptParameters1(new string[]{"RoadCode =" + RoadDetails.IsPlanAvaliable.ToString().Trim(), "PlanID =" +RoadDetails.PlanId.ToString().Trim()}) + "\");'></span></td></tr></table></center>",//URLEncrypt.EncryptParameters1(new string[]{"RoadCode =" + RoadDetails.IsPlanAvaliable.ToString().Trim()}),
  
                        
                        BridgeDetails.ActualLock == "C" ? "<center><table><tr><td  style='border:none'><span class='ui-icon ui-icon-locked ui-align-center ' title='Add Project Plan';'></span></td></tr></table></center>"
                        :BridgeDetails.IsPlanAvaliable.EndsWith("$") ? "<center><table><tr><td  style='border:none'><span class='ui-icon ui-icon-circle-plus ' title='Add Project Plan' onClick =AddProjectPlan('"+ URLEncrypt.EncryptParameters1(new string[]{"RoadCode =" + BridgeDetails.IMS_PR_RoadCode.ToString().Trim(),"StateShare =" + BridgeDetails.StateShare.ToString().Trim(),"MordShare =" + BridgeDetails.MordShare.ToString().Trim(),"TotalSanctionedDate =" + BridgeDetails.TotalSanctionedCost.ToString().Trim()})+"');></span></td></tr></table></center>"//RoadDetails.IsPlanAvaliable 
                            : PMGSYSession.Current.RoleCode==36 ? "<center><table><tr><td  style='border:none;cursor:pointer'><span class='ui-icon ui-icon-pencil' title='Edit Project Plan' onClick =EditProjectPlan('"+ URLEncrypt.EncryptParameters1(new string[]{"RoadCode =" + BridgeDetails.IsPlanAvaliable.ToString().Trim(),"StateShare =" + BridgeDetails.StateShare.ToString().Trim(),"MordShare =" + BridgeDetails.MordShare.ToString().Trim(),"TotalSanctionedDate =" + BridgeDetails.TotalSanctionedCost.ToString().Trim()})+"');></span></td><td style='border:none;cursor:pointer'><span class='ui-icon ui-icon-search' title='View Project Plan' onClick ='ViewProjectPlan(\"" + URLEncrypt.EncryptParameters1(new string[]{"RoadCode =" + BridgeDetails.IsPlanAvaliable.ToString().Trim(),"StateShare =" + BridgeDetails.StateShare.ToString().Trim(),"MordShare =" + BridgeDetails.MordShare.ToString().Trim(),"TotalSanctionedDate =" + BridgeDetails.TotalSanctionedCost.ToString().Trim()}) + "\");'></span></td><td style='border:none;cursor:pointer'></td></tr></table></center>"
                            :BridgeDetails.IsFinalize.Contains("$") ? "<center><table><tr><td style='border:none;cursor:pointer'><span class='ui-icon ui-icon-search' title='View Project Plan' onClick =ViewProjectPlan('"+ URLEncrypt.EncryptParameters1(new string[]{"RoadCode =" + BridgeDetails.IsPlanAvaliable.ToString().Trim(),"StateShare =" + BridgeDetails.StateShare.ToString().Trim(),"MordShare =" + BridgeDetails.MordShare.ToString().Trim(),"TotalSanctionedDate =" + BridgeDetails.TotalSanctionedCost.ToString().Trim()})+"');></span></td></tr></table></center>"
                            :"<center><table><tr><td  style='border:none;cursor:pointer'><span class='ui-icon ui-icon-pencil' title='Edit Project Plan' onClick =EditProjectPlan('"+ URLEncrypt.EncryptParameters1(new string[]{"RoadCode =" + BridgeDetails.IsPlanAvaliable.ToString().Trim(),"StateShare =" + BridgeDetails.StateShare.ToString().Trim(),"MordShare =" + BridgeDetails.MordShare.ToString().Trim(),"TotalSanctionedDate =" + BridgeDetails.TotalSanctionedCost.ToString().Trim()})+"');></span></td><td style='border:none;cursor:pointer'><span class='ui-icon ui-icon-search' title='View Project Plan' onClick ='ViewProjectPlan(\"" + URLEncrypt.EncryptParameters1(new string[]{"RoadCode =" + BridgeDetails.IsPlanAvaliable.ToString().Trim(),"StateShare =" + BridgeDetails.StateShare.ToString().Trim(),"MordShare =" + BridgeDetails.MordShare.ToString().Trim(),"TotalSanctionedDate =" + BridgeDetails.TotalSanctionedCost.ToString().Trim()}) + "\");'></span></td><td style='border:none;cursor:pointer'><span class='ui-icon ui-icon-trash' title='Delete Project Plan' onClick ='DeleteProjectPlan(\"" + URLEncrypt.EncryptParameters1(new string[]{"RoadCode =" + BridgeDetails.IsPlanAvaliable.ToString().Trim(), "PlanID =" +BridgeDetails.PlanId.ToString().Trim()}) + "\");'></span></td></tr></table></center>",//URLEncrypt.EncryptParameters1(new string[]{"RoadCode =" + RoadDetails.IsPlanAvaliable.ToString().Trim()}),
                        

                            BridgeDetails.ActualLock == "C" ? "<center><table><tr><td  style='border:none'><span class='ui-icon ui-icon-locked ui-align-center ' title='Add Project Plan';'></span></td></tr></table></center>"
                            :BridgeDetails.IsFinalize == "-" ? "-" :  BridgeDetails.IsFinalize.Contains("$") ? "<center><table><tr><td  style='border:none'><span class='ui-icon  	ui-icon-locked ' title='Finalize Project Plan';'></span></td></tr></table></center>" :
                            "<center><table><tr><td  style='border:none'><span class='ui-icon  	ui-icon-unlocked ' title='Finalize Project Plan' onClick ='FinalizeProjectPlan(\"" + URLEncrypt.EncryptParameters1(new string[]{"RoadCode =" + BridgeDetails.IsFinalize.ToString().Trim()})  + "\");'></span></td></tr></table></center>",

                             BridgeDetails.ActualLock == "C" ? "<center><table><tr><td  style='border:none'><span class='ui-icon ui-icon-locked ui-align-center ' title='Add Project Plan';'></span></td></tr></table></center>"
                             :BridgeDetails.IsRevisePlan == " " ? "-" : "<a href='#' title='Click here to Revise Plan Details'  onClick=RevisePlanDetails('"+URLEncrypt.EncryptParameters1(new string[]{"RoadCode =" + BridgeDetails.IsRevisePlan.ToString().Trim(),"StateShare =" + BridgeDetails.StateShare.ToString().Trim(),"MordShare =" + BridgeDetails.MordShare.ToString().Trim(), "ReviseStatus =" +ReviseStatus.ToString().Trim(),"TotalSanctionedDate =" + BridgeDetails.TotalSanctionedCost.ToString().Trim()})+"'); return false;'>Revise Plan</a>",
                        //    BridgeDetails.IsActualsAvaliable == ""? "-": "<center><table><tr><td  style='border:none'><span class='ui-icon ui-icon-circle-plus ' title='Add Actuals' onClick =AddActuals('"+URLEncrypt.EncryptParameters1(new string[]{"RoadCode =" + BridgeDetails.IsActualsAvaliable.ToString().Trim(),"StateShare =" + BridgeDetails.StateShare.ToString().Trim(),"MordShare =" + BridgeDetails.MordShare.ToString().Trim(),"TotalSanctionedDate =" + BridgeDetails.TotalSanctionedCost.ToString().Trim()})+"');></span></td></tr></table></center>",
                         //   BridgeDetails.IsActualsAvaliable == ""? "-": "<center><table><tr><td  style='border:none'><span class='ui-icon ui-icon-circle-plus ' title='Add Chainage wise Details' onClick =AddChainage('"+URLEncrypt.EncryptParameters1(new string[]{"RoadCode =" + BridgeDetails.IsActualsAvaliable.ToString().Trim()})+"');></span></td></tr></table></center>",
                         //   BridgeDetails.ProgressStatus=="C"?"-": "<center><table><tr><td  style='border:none'><span class='ui-icon ui-icon-locked ui-align-center ' title='Add Actuals' onClick =AddActuals('"+URLEncrypt.EncryptParameters1(new string[]{"RoadCode =" + BridgeDetails.IsActualsAvaliable.ToString().Trim(),"StateShare =" + BridgeDetails.StateShare.ToString().Trim(),"MordShare =" + BridgeDetails.MordShare.ToString().Trim(),"TotalSanctionedDate =" + BridgeDetails.TotalSanctionedCost.ToString().Trim()})+"');></span></td></tr></table></center>"
                        //class='ui-icon ui-icon-locked ui-align-center'

                        // OLD Conditions commented on 19-05-2023 by rohit borse  
                        // changes by saurabh
                        //(BridgeDetails.ActualLock == "C" )? "<center><table><tr><td  style='border:none'><span class='ui-icon ui-icon-locked ui-align-center ' title='Add Actuals';'> </span></td></tr></table></center>"
                        //:(BridgeDetails.IsPlanAvaliable.EndsWith("$") || BridgeDetails.IsFinalized == "N") ? "-": "<center><table><tr><td  style='border:none'><span class='ui-icon ui-icon-circle-plus ' title='Add Actuals' onClick =AddActuals('"+URLEncrypt.EncryptParameters1(new string[]{"RoadCode =" + BridgeDetails.IsActualsAvaliable.ToString().Trim(),"StateShare =" + BridgeDetails.StateShare.ToString().Trim(),"MordShare =" + BridgeDetails.MordShare.ToString().Trim(),"TotalSanctionedDate =" + BridgeDetails.TotalSanctionedCost.ToString().Trim()})+"');></span></td></tr></table></center>",

                        
                        //------Start  NEW Condition added with Existing on 19-05-2023 by rohit borse
                        ( (BridgeDetails.Is_bridgeProgress_Entered == true && BridgeDetails.Is_bridgeQCR_Entered_andFinalize == true )
                            ? "<span class='ui-icon ui-icon-locked ui-align-center' title='Work freeze due to QCR not uploaded or finalize'  onclick='QcrNotUploaded_WorkFreeze();' > </span>"

                            :   (BridgeDetails.ActualLock == "C" )? "<center><table><tr><td  style='border:none'><span class='ui-icon ui-icon-locked ui-align-center ' title='Add Actuals';'> </span></td></tr></table></center>"
                                :(BridgeDetails.IsPlanAvaliable.EndsWith("$") || BridgeDetails.IsFinalized == "N") ? "-": "<center><table><tr><td  style='border:none'><span class='ui-icon ui-icon-circle-plus ' title='Add Actuals' onClick =AddActuals('"+URLEncrypt.EncryptParameters1(new string[]{"RoadCode =" + BridgeDetails.IsActualsAvaliable.ToString().Trim(),"StateShare =" + BridgeDetails.StateShare.ToString().Trim(),"MordShare =" + BridgeDetails.MordShare.ToString().Trim(),"TotalSanctionedDate =" + BridgeDetails.TotalSanctionedCost.ToString().Trim()})+"');></span></td></tr></table></center>"
                            ),
                        //------ End


          //   BridgeDetails.IsActualsAvaliable == ""? "-": "<center><table><tr><td  style='border:none'><span class='ui-icon ui-icon-circle-plus ' title='Add Chainage wise Details' onClick =AddChainage('"+URLEncrypt.EncryptParameters1(new string[]{"RoadCode =" + BridgeDetails.IsActualsAvaliable.ToString().Trim()})+"');></span></td></tr></table></center>",
  
// changes ended here...

                        }
                }).ToArray();

            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "PMISBridgeListDAL().DAL");
                totalRecords = 0;
                return null;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        public string SubmitBridgeActualsDAL(IEnumerable<AddActualsViewModel> planData)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
           
            try
            {
                // Added on 28-03-2022 by Srishti Tyagi
                string AprilMonthStartDay = ConfigurationManager.AppSettings["APRIL_MONTH_START_DAY"];   //1
                int AprilMonthStartDayValue = Convert.ToInt16(AprilMonthStartDay);

                string AprilMonthEndDay = ConfigurationManager.AppSettings["APRIL_MONTH_END_DAY"];   //15
                int AprilMonthEndDayValue = Convert.ToInt16(AprilMonthEndDay);

                string AprilMonth = ConfigurationManager.AppSettings["APRIL_MONTH"];  //4
                int AprilMonthValue = Convert.ToInt16(AprilMonth);

                DateTime FinanDate = DateTime.Now;                         // change by saurabh
                int FinancialYear = FinanDate.Month >= 4 ? FinanDate.Year : FinanDate.Year;
                DateTime Conditional_Date_Value = new DateTime(FinancialYear, 3, 31, 00, 00, 00);
                int CondFinanYear = FinancialYear;
                using (TransactionScope ts = new TransactionScope())
                {
                    PMIS_PROGRESS_MASTER ProgressMaster = new PMIS_PROGRESS_MASTER();
                    int RoadCode = 0;  //planData.ElementAt(0).IMS_PR_ROAD_CODE;
                    char Project_Status = 'P';
                    Nullable<DateTime> Entry_Date = new DateTime();
                    // execution model
                    String ProjectStatus = String.Empty;
                    DateTime? ProgressDate = null;
                    Decimal ComplLength = 0;

                    foreach (var plan in planData)
                    {
                        RoadCode = plan.IMS_PR_ROAD_CODE;
                        Project_Status = (plan.ProjectStatus == 'C' ? 'C' : 'P');
                        Entry_Date = plan.Date_of_progress_entry;
                        // Execution model mapping
                        ProjectStatus = Convert.ToString(plan.ProjectStatus);
                        ProgressDate = plan.Date_of_progress_entry;
                        ComplLength = plan.CompletedRoadLength;

                        if (plan.Date_of_progress_entry != null)
                        {
                            //Entry_Date = (plan.Date_of_progress_entry.Value.Day >= AprilMonthStartDayValue
                            //    && plan.Date_of_progress_entry.Value.Day <= AprilMonthEndDayValue
                            //    && plan.Date_of_progress_entry.Value.Month == AprilMonthValue) ? Conditional_Date_Value : plan.Date_of_progress_entry;

                            Entry_Date = (plan.Date_of_progress_entry.Value.Day >= AprilMonthStartDayValue
                                && plan.Date_of_progress_entry.Value.Day <= AprilMonthEndDayValue
                                && plan.Date_of_progress_entry.Value.Month == AprilMonthValue) ? Conditional_Date_Value : DateTime.Now;
                        }
                        else
                        {
                            return ("Date of progress entry is mandatory to Select.");
                        }

                        if (FinanDate.Month != Entry_Date.Value.Month || FinanDate.Day != Entry_Date.Value.Day || FinanDate.Year != Entry_Date.Value.Year)
                        {
                            return ("Progress can be entered in Current Date of Current Financial Year");
                        }

                        //if (FinanDate.Month == AprilMonthValue && FinanDate.Day > AprilMonthEndDayValue)   // CHANGE
                        //{
                        //    if (Entry_Date.Value.Year * 12 + Entry_Date.Value.Month <= CondFinanYear * 12 + 3)
                        //    {
                        //        return ("Progress can be entered in Current Date of Current Financial Year");
                        //    }
                        //}

                        if (ProjectStatus != "P" && ProjectStatus != "W" && ProjectStatus != "H")
                        {
                            if (ComplLength == 0)  // && ProjectStatusValid != "W" && ProjectStatusValid != "H"
                            {
                                return ("Completion Length is mandatory to fill.");
                            }
                        }
                        //if (ComplLength == 0)  //&& ProjectStatusValid != "W" && ProjectStatusValid != "H"
                        //{
                        //    return ("Completion Length is mandatory to fill.");
                        //}
                        if (ProjectStatus == "0")
                        {
                            return ("Project Status is mandatory to fill.");
                        }
                        if (plan.Date_of_progress_entry == null)
                        {
                            return ("Date of progress entry is mandatory to fill.");
                        }
                        if (RoadCode > 0)
                            break;
                    }

                    var latest_Master = dbContext.PMIS_PROGRESS_MASTER.Where(x => x.IMS_PR_ROAD_CODE == RoadCode & x.IS_LATEST == "Y").FirstOrDefault();
                    if (latest_Master != null)
                    {
                        latest_Master.IS_LATEST = "N";
                        dbContext.Entry(latest_Master).State = EntityState.Modified;
                        dbContext.SaveChanges();

                    }
                    foreach (var plan in planData)
                    {
                        RoadCode = plan.IMS_PR_ROAD_CODE;
                        if (RoadCode != 0)
                        {
                            ProgressMaster.PROGRESS_MASTER_ID = dbContext.PMIS_PROGRESS_MASTER.Any() ? dbContext.PMIS_PROGRESS_MASTER.Max(s => s.PROGRESS_MASTER_ID) + 1 : 1;
                            ProgressMaster.PLAN_ID = dbContext.PMIS_PLAN_MASTER.Where(x => x.IMS_PR_ROAD_CODE == RoadCode && x.IS_LATEST == "Y").Select(x => x.PLAN_ID).FirstOrDefault();
                            ProgressMaster.IMS_PR_ROAD_CODE = RoadCode;
                            ProgressMaster.COMPLETION_LENGTH = plan.CompletedRoadLength;
                            ProgressMaster.PROJECT_STATUS_ = Convert.ToString(plan.ProjectStatus);
                            ProgressMaster.USERID = PMGSYSession.Current.UserId;
                            ProgressMaster.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                            ProgressMaster.IS_LATEST = "Y";
                            ProgressMaster.BASELINE_NO = dbContext.PMIS_PLAN_MASTER.Where(x => x.IMS_PR_ROAD_CODE == RoadCode && x.IS_LATEST == "Y").Select(x => x.BASELINE_NO).FirstOrDefault() ?? default(int);
                            //ProgressMaster.ENTRY_DATE = plan.Date_of_progress_entry ?? default(DateTime);
                            ProgressMaster.ENTRY_DATE = Convert.ToDateTime(Entry_Date);
                            ProgressMaster.REMARKS = String.IsNullOrEmpty(plan.Remarks) ? null : plan.Remarks;
                            dbContext.PMIS_PROGRESS_MASTER.Add(ProgressMaster);
                            dbContext.SaveChanges();
                        }
                        break;
                    }
                    // added 1st progress to PMIS_progress_master

                    var Progress_id = dbContext.PMIS_PROGRESS_DETAILS.Any() ? dbContext.PMIS_PROGRESS_DETAILS.Max(s => s.PROGRESS_ID) + 1 : 1;
                    var latest_record = dbContext.PMIS_PROGRESS_DETAILS.Where(x => x.IMS_PR_ROAD_CODE == RoadCode && x.IS_LATEST == "Y").ToList();
                    if (latest_record != null)
                    {
                        foreach (var ls in latest_record)
                        {
                            ls.IS_LATEST = "N";
                            dbContext.Entry(ls).State = EntityState.Modified;
                            dbContext.SaveChanges();
                        }
                    }
                    foreach (var obj in planData)
                    {
                        if (RoadCode != 0)
                        {
                            PMIS_PROGRESS_DETAILS ProgressDetails = new PMIS_PROGRESS_DETAILS();
                            ProgressDetails.PROGRESS_ID = Progress_id;
                            ProgressDetails.PROGRESS_MASTER_ID = ProgressMaster.PROGRESS_MASTER_ID;
                            ProgressDetails.PLAN_ID = ProgressMaster.PLAN_ID;
                            ProgressDetails.IMS_PR_ROAD_CODE = ProgressMaster.IMS_PR_ROAD_CODE;
                            ProgressDetails.ACTIVITY_ID = dbContext.PMIS_ACTIVITY_MASTER.Where(x => x.ACTIVITY_DESC == obj.ACTIVITY_DESC && x.ROAD_TYPE == "L").Select(x => x.ACTIVITY_ID).FirstOrDefault();
                            ProgressDetails.PGS_QUANTITY = obj.ACTUAL_QUANTITY;
                            ProgressDetails.ACTUAL_START_DATE = obj.STARTED_DATE;
                            ProgressDetails.ACTUAL_END_DATE = obj.FINISHED_DATE;
                            ProgressDetails.IS_LATEST = "Y";
                            //ProgressDetails.ENTRY_DATE = obj.Date_of_progress_entry ?? default(DateTime);
                            ProgressDetails.ENTRY_DATE = Convert.ToDateTime(Entry_Date);
                            ProgressDetails.BASELINE_NO = dbContext.PMIS_PLAN_MASTER.Where(x => x.IMS_PR_ROAD_CODE == RoadCode && x.IS_LATEST == "Y").Select(x => x.BASELINE_NO).FirstOrDefault() ?? default(int);
                            ProgressDetails.USERID = PMGSYSession.Current.UserId;
                            ProgressDetails.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];

                            dbContext.PMIS_PROGRESS_DETAILS.Add(ProgressDetails);
                            Progress_id++;
                        }
                    }
                    dbContext.SaveChanges();
                    // Progress Added to PMIS_PROGRESS_DETAILS on 29-12-2021

                    //      prepWorkLen = 0, earthWorkLen = 0, subBasePrep = 0, baseCourseLen = 0, surCourseLen = 0, cdWorksLen = 0, miscLen = 0;
                    //      List<int> ActivityId = dbContext.PMIS_ACTIVITY_MASTER.Select(x => x.ACTIVITY_ID).ToList();
                    int mONTHtOCheck = Entry_Date.Value.Month;
                    int YearToCheck = Entry_Date.Value.Year;
                    if (!(dbContext.EXEC_LSB_MONTHLY_STATUS.Where(x => x.IMS_PR_ROAD_CODE == RoadCode && x.EXEC_PROG_MONTH == mONTHtOCheck && x.EXEC_PROG_YEAR == YearToCheck).Any()))
                    {
                        EXEC_LSB_MONTHLY_STATUS monthlyStatus = new EXEC_LSB_MONTHLY_STATUS();
                        monthlyStatus.IMS_PR_ROAD_CODE = RoadCode;
                        //monthlyStatus.EXEC_PROG_MONTH = ProgressDate.Value.Month;
                        //monthlyStatus.EXEC_PROG_YEAR = ProgressDate.Value.Year;

                        monthlyStatus.EXEC_PROG_MONTH = mONTHtOCheck;
                        monthlyStatus.EXEC_PROG_YEAR = YearToCheck;

                        if (ProjectStatus == "C" || ProjectStatus == "P" || ProjectStatus == "A" || ProjectStatus == "F" || ProjectStatus == "L")
                        {
                            monthlyStatus.EXEC_ISCOMPLETED = ProjectStatus;
                        }
                        else
                        {
                            monthlyStatus.EXEC_ISCOMPLETED = "P";
                        }
                       // monthlyStatus.EXEC_COMPLETION_DATE = ProjectStatus == "C" ? ProgressDate : null;
                        monthlyStatus.EXEC_COMPLETION_DATE = ProjectStatus == "C" ? Entry_Date : null;

                        monthlyStatus.USERID = PMGSYSession.Current.UserId;
                        monthlyStatus.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];

                        monthlyStatus.EXEC_RAFT = 0;
                        monthlyStatus.EXEC_FLOOR_PROTECTION = 0;
                        monthlyStatus.EXEC_SINKING = 0;
                        monthlyStatus.EXEC_BOTTOM_PLUGGING = 0;
                        monthlyStatus.EXEC_TOP_PLUGGING = 0;
                        monthlyStatus.EXEC_WELL_CAP = 0;
                        monthlyStatus.EXEC_PIER_SHAFT = 0;
                        monthlyStatus.EXEC_PIER_CAP = 0;
                        monthlyStatus.EXEC_BEARINGS = 0;
                        monthlyStatus.EXEC_DECK_SLAB = 0;
                        monthlyStatus.EXEC_WEARING_COAT = 0;
                        monthlyStatus.EXEC_POSTS_RAILING = 0;
                        monthlyStatus.EXEC_APP_ROAD_WORK = 0;
                        monthlyStatus.EXEC_APP_CD_WORKS = 0;
                        monthlyStatus.EXEC_APP_COMPLETED = 0;
                        monthlyStatus.EXEC_BRIDGE_COMPLETED = ComplLength;

                        dbContext.EXEC_LSB_MONTHLY_STATUS.Add(monthlyStatus);
                        dbContext.SaveChanges();
                    }
                    else
                    {
                        EXEC_LSB_MONTHLY_STATUS monthlyStatus = new EXEC_LSB_MONTHLY_STATUS();
                        monthlyStatus.IMS_PR_ROAD_CODE = RoadCode;
                        //monthlyStatus.EXEC_PROG_MONTH = ProgressDate.Value.Month;
                        //monthlyStatus.EXEC_PROG_YEAR = ProgressDate.Value.Year;

                        monthlyStatus.EXEC_PROG_MONTH = mONTHtOCheck;
                        monthlyStatus.EXEC_PROG_YEAR = YearToCheck;

                        if (ProjectStatus == "C" || ProjectStatus == "P" || ProjectStatus == "A" || ProjectStatus == "F" || ProjectStatus == "L")
                        {
                            monthlyStatus.EXEC_ISCOMPLETED = ProjectStatus;
                        }
                        else
                        {
                            monthlyStatus.EXEC_ISCOMPLETED = "P";
                        }
                       // monthlyStatus.EXEC_COMPLETION_DATE = ProjectStatus == "C" ? ProgressDate : null;
                        monthlyStatus.EXEC_COMPLETION_DATE = ProjectStatus == "C" ? Entry_Date : null;

                        monthlyStatus.USERID = PMGSYSession.Current.UserId;
                        monthlyStatus.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];

                        monthlyStatus.EXEC_RAFT = 0;
                        monthlyStatus.EXEC_FLOOR_PROTECTION = 0;
                        monthlyStatus.EXEC_SINKING = 0;
                        monthlyStatus.EXEC_BOTTOM_PLUGGING = 0;
                        monthlyStatus.EXEC_TOP_PLUGGING = 0;
                        monthlyStatus.EXEC_WELL_CAP = 0;
                        monthlyStatus.EXEC_PIER_SHAFT = 0;
                        monthlyStatus.EXEC_PIER_CAP = 0;
                        monthlyStatus.EXEC_BEARINGS = 0;
                        monthlyStatus.EXEC_DECK_SLAB = 0;
                        monthlyStatus.EXEC_WEARING_COAT = 0;
                        monthlyStatus.EXEC_POSTS_RAILING = 0;
                        monthlyStatus.EXEC_APP_ROAD_WORK = 0;
                        monthlyStatus.EXEC_APP_CD_WORKS = 0;
                        monthlyStatus.EXEC_APP_COMPLETED = 0;
                        monthlyStatus.EXEC_BRIDGE_COMPLETED = ComplLength;

                        dbContext.Entry(monthlyStatus).State = EntityState.Modified;
                        dbContext.SaveChanges();
                    }
                    IMS_SANCTIONED_PROJECTS imsMaster = dbContext.IMS_SANCTIONED_PROJECTS.Find(RoadCode);
                    if (imsMaster != null)
                    {
                        imsMaster.IMS_ISCOMPLETED = Project_Status.ToString();
                        imsMaster.IMS_ENTRY_DATE_PHYSICAL = Entry_Date;
                        imsMaster.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                        imsMaster.USERID = PMGSYSession.Current.UserId;
                        dbContext.SaveChanges();
                    }
                    ts.Complete();
                    // LSB progress Required to be mapped....





                }   // transacton Scope ends here....
                    //  return null;
                return string.Empty;
            }   // try block closed here
            catch (DbEntityValidationException e)
            {
                foreach (var eve in e.EntityValidationErrors)
                {
                    using (System.IO.StreamWriter sw = System.IO.File.AppendText(System.IO.Path.Combine("D:\\ErrorLog" + System.DateTime.Now.Date.ToString("dd_MM_yyyy") + ".txt", "")))
                    {
                        sw.WriteLine("Date :" + DateTime.Now.ToString());
                        sw.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                        eve.Entry.Entity.GetType().Name, eve.Entry.State);
                        sw.WriteLine("---------------------------------------------------------------------------------------");
                        sw.Close();
                    }

                    foreach (var ve in eve.ValidationErrors)
                    {
                        using (System.IO.StreamWriter sw = System.IO.File.AppendText(System.IO.Path.Combine("D:\\ErrorLog" + System.DateTime.Now.Date.ToString("dd_MM_yyyy") + ".txt", "")))
                        {
                            sw.WriteLine("Date :" + DateTime.Now.ToString());
                            sw.WriteLine("- Property: \"{0}\", Error: \"{1}\"",
                            ve.PropertyName, ve.ErrorMessage);
                            sw.WriteLine("---------------------------------------------------------------------------------------");
                            sw.Close();
                        }
                    }
                }
                throw;
            }   //  1st catch
            catch (DbUpdateException ex)
            {
                ErrorLog.LogError(ex, "SubmitBridgeActualsDAL(DbUpdateException ex).DAL");
                return ("An Error Occurred While Processing Your Request.");
            }   // 2nd catch
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "SubmitBridgeActualsDAL().DAL");
                return ("Error Occurred While Processing Request.");
            }    // 3rd catch
            finally
            {
                dbContext.Dispose();
            }       // finally ends here

        }   // DAL Method closed here.

        public string SaveBridgeActualsDAL(IEnumerable<AddActualsViewModel> planData)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            try
            {
                // Added on 28-03-2022 by Srishti Tyagi
                string AprilMonthStartDay = ConfigurationManager.AppSettings["APRIL_MONTH_START_DAY"];   //1
                int AprilMonthStartDayValue = Convert.ToInt16(AprilMonthStartDay);

                string AprilMonthEndDay = ConfigurationManager.AppSettings["APRIL_MONTH_END_DAY"];   //15
                int AprilMonthEndDayValue = Convert.ToInt16(AprilMonthEndDay);

                string AprilMonth = ConfigurationManager.AppSettings["APRIL_MONTH"];  //4
                int AprilMonthValue = Convert.ToInt16(AprilMonth);

                DateTime FinanDate = DateTime.Now;                         // change by saurabh
                int FinancialYear = FinanDate.Month >= 4 ? FinanDate.Year : FinanDate.Year;
                DateTime Conditional_Date_Value = new DateTime(FinancialYear, 3, 31, 00, 00, 00);
                int CondFinanYear = FinancialYear;

                using (TransactionScope ts = new TransactionScope())
                {

                    int RoadCode = 0;  //planData.ElementAt(0).IMS_PR_ROAD_CODE;
                    char Project_Status = 'P';
                    Nullable<DateTime> Entry_Date = new DateTime();

                    // execution model
                    String ProjectStatus = String.Empty;
                    DateTime? ProgressDate = null;
                    Decimal ComplLength = 0;

                    foreach (var plan in planData)
                    {
                        RoadCode = plan.IMS_PR_ROAD_CODE;
                        Project_Status = (plan.ProjectStatus == 'C' ? 'C' : 'P');
                        Entry_Date = plan.Date_of_progress_entry;
                        // Execution model mapping
                        ProjectStatus = Convert.ToString(plan.ProjectStatus);
                        ProgressDate = plan.Date_of_progress_entry;
                        ComplLength = plan.CompletedRoadLength;

                        if (plan.Date_of_progress_entry != null)
                        {
                            Entry_Date = (plan.Date_of_progress_entry.Value.Day >= AprilMonthStartDayValue
                                && plan.Date_of_progress_entry.Value.Day <= AprilMonthEndDayValue
                                && plan.Date_of_progress_entry.Value.Month == AprilMonthValue) ? Conditional_Date_Value : DateTime.Now;
                        }
                        else
                        {
                            return ("Date of progress entry is mandatory to Select.");
                        }

                        if (FinanDate.Month != Entry_Date.Value.Month || FinanDate.Day != Entry_Date.Value.Day || FinanDate.Year != Entry_Date.Value.Year)
                        {
                            return ("Progress can be entered in Current Date of Current Financial Year");
                        }

                        //if (FinanDate.Month == AprilMonthValue && FinanDate.Day > AprilMonthEndDayValue)   // CHANGE
                        //{
                        //    if (Entry_Date.Value.Year * 12 + Entry_Date.Value.Month <= CondFinanYear * 12 + 3)
                        //    {
                        //        return ("Progress can be entered in Current Date of Current Financial Year");
                        //    }
                        //}

                        if (ProjectStatus != "P" && ProjectStatus != "W" && ProjectStatus != "H")
                        {
                            if (ComplLength == 0)  // && ProjectStatusValid != "W" && ProjectStatusValid != "H"
                            {
                                return ("Completion Length is mandatory to fill.");
                            }
                        }
                       

                        if (ProjectStatus == "0")
                        {
                            return ("Project Status is mandatory to fill.");
                        }
                        if (plan.Date_of_progress_entry == null)
                        {
                            return ("Date of progress entry is mandatory to fill.");
                        }
                        if (RoadCode > 0)
                            break;
                    }   // 1st for that sets project status.
                    //foreach (var dateValue in planData)
                    //{
                    //    if (dateValue.Date_of_progress_entry == null)
                    //    {
                    //        return ("Date of progress entry is mandatory to fill.");
                    //    }
                    //}  // 2nd for loop, it checks whether progress date is filled or not.

                    // if starts here
                    if (dbContext.PMIS_PROGRESS_MASTER.Where(x => x.IMS_PR_ROAD_CODE == RoadCode).Any())
                    {
                        foreach (var plan in planData)
                        {
                            RoadCode = plan.IMS_PR_ROAD_CODE;
                            var latest_Master = dbContext.PMIS_PROGRESS_MASTER.Where(x => x.IMS_PR_ROAD_CODE == RoadCode & x.IS_LATEST == "Y").FirstOrDefault();
                            if (latest_Master != null && RoadCode != 0)
                            {
                                latest_Master.COMPLETION_LENGTH = plan.CompletedRoadLength;
                                latest_Master.PROJECT_STATUS_ = Convert.ToString(plan.ProjectStatus);
                                latest_Master.USERID = PMGSYSession.Current.UserId;
                                latest_Master.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                                latest_Master.IS_LATEST = "Y";
                                //latest_Master.ENTRY_DATE = plan.Date_of_progress_entry ?? DateTime.Now;
                                latest_Master.ENTRY_DATE = Convert.ToDateTime(Entry_Date);
                                latest_Master.REMARKS = String.IsNullOrEmpty(plan.Remarks) ? null : plan.Remarks;
                                dbContext.Entry(latest_Master).State = EntityState.Modified;
                                dbContext.SaveChanges();
                            }
                            break;
                        }


                        foreach (var obj in planData)
                        {
                            var activity_ID = dbContext.PMIS_ACTIVITY_MASTER.Where(x => x.ACTIVITY_DESC == obj.ACTIVITY_DESC && x.ROAD_TYPE == "L").Select(x => x.ACTIVITY_ID).FirstOrDefault();  // changes by saurabh on 18-11-2022
                            var latest_record = dbContext.PMIS_PROGRESS_DETAILS.Where(x => x.IMS_PR_ROAD_CODE == RoadCode && x.ACTIVITY_ID == activity_ID && x.IS_LATEST == "Y").FirstOrDefault(); // changes by saurabh on 18-11-2022

                            if (RoadCode != 0 && latest_record != null)
                            {
                                latest_record.PGS_QUANTITY = obj.ACTUAL_QUANTITY;
                                latest_record.ACTUAL_START_DATE = obj.STARTED_DATE;
                                latest_record.ACTUAL_END_DATE = obj.FINISHED_DATE;
                                latest_record.IS_LATEST = "Y";
                                //latest_record.ENTRY_DATE = obj.Date_of_progress_entry ?? DateTime.Now;
                                latest_record.ENTRY_DATE = Convert.ToDateTime(Entry_Date);
                                latest_record.USERID = PMGSYSession.Current.UserId;
                                latest_record.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];

                                dbContext.Entry(latest_record).State = EntityState.Modified;

                            }

                        }
                        dbContext.SaveChanges();
                    } // if closed here 
                    else
                    {
                        var latest_Master_new = dbContext.PMIS_PROGRESS_MASTER.Where(x => x.IMS_PR_ROAD_CODE == RoadCode & x.IS_LATEST == "Y").FirstOrDefault();
                        if (latest_Master_new != null)
                        {
                            latest_Master_new.IS_LATEST = "N";
                            dbContext.Entry(latest_Master_new).State = EntityState.Modified;
                            dbContext.SaveChanges();

                        }
                        PMIS_PROGRESS_MASTER ProgressMaster = new PMIS_PROGRESS_MASTER();
                        foreach (var plan in planData)
                        {
                            RoadCode = plan.IMS_PR_ROAD_CODE;
                            if (RoadCode != 0)
                            {
                                ProgressMaster.PROGRESS_MASTER_ID = dbContext.PMIS_PROGRESS_MASTER.Any() ? dbContext.PMIS_PROGRESS_MASTER.Max(s => s.PROGRESS_MASTER_ID) + 1 : 1;
                                ProgressMaster.PLAN_ID = dbContext.PMIS_PLAN_MASTER.Where(x => x.IMS_PR_ROAD_CODE == RoadCode && x.IS_LATEST == "Y").Select(x => x.PLAN_ID).FirstOrDefault();
                                ProgressMaster.IMS_PR_ROAD_CODE = RoadCode;
                                ProgressMaster.COMPLETION_LENGTH = plan.CompletedRoadLength;
                                ProgressMaster.PROJECT_STATUS_ = Convert.ToString(plan.ProjectStatus);
                                ProgressMaster.USERID = PMGSYSession.Current.UserId;
                                ProgressMaster.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                                ProgressMaster.IS_LATEST = "Y";
                                ProgressMaster.BASELINE_NO = dbContext.PMIS_PLAN_MASTER.Where(x => x.IMS_PR_ROAD_CODE == RoadCode && x.IS_LATEST == "Y").Select(x => x.BASELINE_NO).FirstOrDefault() ?? default(int);
                                //ProgressMaster.ENTRY_DATE = plan.Date_of_progress_entry ?? DateTime.Now;
                                ProgressMaster.ENTRY_DATE = Convert.ToDateTime(Entry_Date);
                                dbContext.PMIS_PROGRESS_MASTER.Add(ProgressMaster);
                                dbContext.SaveChanges();
                            }
                            break;
                        }
                        var Progress_id = dbContext.PMIS_PROGRESS_DETAILS.Any() ? dbContext.PMIS_PROGRESS_DETAILS.Max(s => s.PROGRESS_ID) + 1 : 1;
                        var latest_record_new = dbContext.PMIS_PROGRESS_DETAILS.Where(x => x.IMS_PR_ROAD_CODE == RoadCode && x.IS_LATEST == "Y").ToList();
                        if (latest_record_new != null)
                        {
                            foreach (var ls in latest_record_new)
                            {
                                ls.IS_LATEST = "N";
                                dbContext.Entry(ls).State = EntityState.Modified;
                                dbContext.SaveChanges();
                            }
                        }
                        foreach (var obj in planData)
                        {
                            if (RoadCode != 0)
                            {
                                PMIS_PROGRESS_DETAILS ProgressDetails = new PMIS_PROGRESS_DETAILS();
                                ProgressDetails.PROGRESS_ID = Progress_id;
                                ProgressDetails.PROGRESS_MASTER_ID = ProgressMaster.PROGRESS_MASTER_ID;
                                ProgressDetails.PLAN_ID = ProgressMaster.PLAN_ID;
                                ProgressDetails.IMS_PR_ROAD_CODE = ProgressMaster.IMS_PR_ROAD_CODE;
                                ProgressDetails.ACTIVITY_ID = dbContext.PMIS_ACTIVITY_MASTER.Where(x => x.ACTIVITY_DESC == obj.ACTIVITY_DESC && x.ROAD_TYPE == "L").Select(x => x.ACTIVITY_ID).FirstOrDefault();
                                ProgressDetails.PGS_QUANTITY = obj.ACTUAL_QUANTITY;
                                ProgressDetails.ACTUAL_START_DATE = obj.STARTED_DATE;
                                ProgressDetails.ACTUAL_END_DATE = obj.FINISHED_DATE;
                                ProgressDetails.IS_LATEST = "Y";
                                //ProgressDetails.ENTRY_DATE = obj.Date_of_progress_entry ?? DateTime.Now;
                                ProgressDetails.ENTRY_DATE = Convert.ToDateTime(Entry_Date);
                                ProgressDetails.BASELINE_NO = dbContext.PMIS_PLAN_MASTER.Where(x => x.IMS_PR_ROAD_CODE == RoadCode && x.IS_LATEST == "Y").Select(x => x.BASELINE_NO).FirstOrDefault() ?? default(int);
                                ProgressDetails.USERID = PMGSYSession.Current.UserId;
                                ProgressDetails.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];

                                dbContext.PMIS_PROGRESS_DETAILS.Add(ProgressDetails);
                                Progress_id++;
                            }
                        }
                        dbContext.SaveChanges();
                    } // else loop  closed here
                    int MonthToCheck = Entry_Date.Value.Month;
                    int YearToCheck = Entry_Date.Value.Year;
                    if (dbContext.EXEC_LSB_MONTHLY_STATUS.Where(x => x.IMS_PR_ROAD_CODE == RoadCode && x.EXEC_PROG_MONTH == MonthToCheck && x.EXEC_PROG_YEAR == YearToCheck).Any())
                    {
                        EXEC_LSB_MONTHLY_STATUS monthlyStatus = new EXEC_LSB_MONTHLY_STATUS();
                        monthlyStatus.IMS_PR_ROAD_CODE = RoadCode;
                        //monthlyStatus.EXEC_PROG_MONTH = ProgressDate.Value.Month;
                        //monthlyStatus.EXEC_PROG_YEAR = ProgressDate.Value.Year;

                        monthlyStatus.EXEC_PROG_MONTH = MonthToCheck;
                        monthlyStatus.EXEC_PROG_YEAR = YearToCheck;

                        if (ProjectStatus == "C" || ProjectStatus == "P" || ProjectStatus == "A" || ProjectStatus == "F" || ProjectStatus == "L")
                        {
                            monthlyStatus.EXEC_ISCOMPLETED = ProjectStatus;
                        }
                        else
                        {
                            monthlyStatus.EXEC_ISCOMPLETED = "P";
                        }
                        //monthlyStatus.EXEC_COMPLETION_DATE = ProjectStatus == "C" ? ProgressDate : null;
                        monthlyStatus.EXEC_COMPLETION_DATE = ProjectStatus == "C" ? Entry_Date : null;
                        monthlyStatus.USERID = PMGSYSession.Current.UserId;
                        monthlyStatus.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];

                        monthlyStatus.EXEC_RAFT = 0;
                        monthlyStatus.EXEC_FLOOR_PROTECTION = 0;
                        monthlyStatus.EXEC_SINKING = 0;
                        monthlyStatus.EXEC_BOTTOM_PLUGGING = 0;
                        monthlyStatus.EXEC_TOP_PLUGGING = 0;
                        monthlyStatus.EXEC_WELL_CAP = 0;
                        monthlyStatus.EXEC_PIER_SHAFT = 0;
                        monthlyStatus.EXEC_PIER_CAP = 0;
                        monthlyStatus.EXEC_BEARINGS = 0;
                        monthlyStatus.EXEC_DECK_SLAB = 0;
                        monthlyStatus.EXEC_WEARING_COAT = 0;
                        monthlyStatus.EXEC_POSTS_RAILING = 0;
                        monthlyStatus.EXEC_APP_ROAD_WORK = 0;
                        monthlyStatus.EXEC_APP_CD_WORKS = 0;
                        monthlyStatus.EXEC_APP_COMPLETED = 0;
                        monthlyStatus.EXEC_BRIDGE_COMPLETED = ComplLength;

                        dbContext.Entry(monthlyStatus).State = EntityState.Modified;
                        dbContext.SaveChanges();
                    }
                    else
                    {
                        EXEC_LSB_MONTHLY_STATUS monthlyStatus = new EXEC_LSB_MONTHLY_STATUS();
                        monthlyStatus.IMS_PR_ROAD_CODE = RoadCode;
                        //monthlyStatus.EXEC_PROG_MONTH = ProgressDate.Value.Month;
                        //monthlyStatus.EXEC_PROG_YEAR = ProgressDate.Value.Year;

                        monthlyStatus.EXEC_PROG_MONTH = MonthToCheck;
                        monthlyStatus.EXEC_PROG_YEAR = YearToCheck;

                        if (ProjectStatus == "C" || ProjectStatus == "P" || ProjectStatus == "A" || ProjectStatus == "F" || ProjectStatus == "L")
                        {
                            monthlyStatus.EXEC_ISCOMPLETED = ProjectStatus;
                        }
                        else
                        {
                            monthlyStatus.EXEC_ISCOMPLETED = "P";
                        }
                        //monthlyStatus.EXEC_COMPLETION_DATE = ProjectStatus == "C" ? ProgressDate : null;
                        monthlyStatus.EXEC_COMPLETION_DATE = ProjectStatus == "C" ? Entry_Date : null;
                        monthlyStatus.USERID = PMGSYSession.Current.UserId;
                        monthlyStatus.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];

                        monthlyStatus.EXEC_RAFT = 0;
                        monthlyStatus.EXEC_FLOOR_PROTECTION = 0;
                        monthlyStatus.EXEC_SINKING = 0;
                        monthlyStatus.EXEC_BOTTOM_PLUGGING = 0;
                        monthlyStatus.EXEC_TOP_PLUGGING = 0;
                        monthlyStatus.EXEC_WELL_CAP = 0;
                        monthlyStatus.EXEC_PIER_SHAFT = 0;
                        monthlyStatus.EXEC_PIER_CAP = 0;
                        monthlyStatus.EXEC_BEARINGS = 0;
                        monthlyStatus.EXEC_DECK_SLAB = 0;
                        monthlyStatus.EXEC_WEARING_COAT = 0;
                        monthlyStatus.EXEC_POSTS_RAILING = 0;
                        monthlyStatus.EXEC_APP_ROAD_WORK = 0;
                        monthlyStatus.EXEC_APP_CD_WORKS = 0;
                        monthlyStatus.EXEC_APP_COMPLETED = 0;
                        monthlyStatus.EXEC_BRIDGE_COMPLETED = ComplLength;

                        dbContext.EXEC_LSB_MONTHLY_STATUS.Add(monthlyStatus);
                        dbContext.SaveChanges();
                    }
                    IMS_SANCTIONED_PROJECTS imsMaster = dbContext.IMS_SANCTIONED_PROJECTS.Find(RoadCode);
                    if (imsMaster != null)
                    {
                        imsMaster.IMS_ISCOMPLETED = Project_Status.ToString();
                        imsMaster.IMS_ENTRY_DATE_PHYSICAL = Entry_Date;
                        imsMaster.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                        imsMaster.USERID = PMGSYSession.Current.UserId;
                        dbContext.SaveChanges();
                    }
                    ts.Complete();

                }
                return string.Empty;  // return statment for whole method
            } // try block ends here....
            catch (DbEntityValidationException e)
            {
                foreach (var eve in e.EntityValidationErrors)
                {
                    using (System.IO.StreamWriter sw = System.IO.File.AppendText(System.IO.Path.Combine("D:\\ErrorLog" + System.DateTime.Now.Date.ToString("dd_MM_yyyy") + ".txt", "")))
                    {
                        sw.WriteLine("Date :" + DateTime.Now.ToString());
                        sw.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                        eve.Entry.Entity.GetType().Name, eve.Entry.State);
                        sw.WriteLine("---------------------------------------------------------------------------------------");
                        sw.Close();
                    }

                    foreach (var ve in eve.ValidationErrors)
                    {
                        using (System.IO.StreamWriter sw = System.IO.File.AppendText(System.IO.Path.Combine("D:\\ErrorLog" + System.DateTime.Now.Date.ToString("dd_MM_yyyy") + ".txt", "")))
                        {
                            sw.WriteLine("Date :" + DateTime.Now.ToString());
                            sw.WriteLine("- Property: \"{0}\", Error: \"{1}\"",
                            ve.PropertyName, ve.ErrorMessage);
                            sw.WriteLine("---------------------------------------------------------------------------------------");
                            sw.Close();
                        }
                    }
                }
                throw;
            }
            catch (DbUpdateException ex)
            {
                ErrorLog.LogError(ex, "SaveBridgeActualsDAL(DbUpdateException ex).DAL");
                return ("An Error Occurred While Processing Your Request.");
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "SaveBridgeActualsDAL().DAL");
                return ("Error Occurred While Processing Request.");
            }
            finally
            {
                dbContext.Dispose();
            }

        }   // method closing  

        public string SaveBridgeProjectPlanDAL(IEnumerable<AddPlanPMISViewModelBridge> planData)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            try
            {
                using (TransactionScope ts = new TransactionScope())
                {
                    PMIS_PLAN_MASTER planMaster = new PMIS_PLAN_MASTER();
                    int RoadCode = 0;  //planData.ElementAt(0).IMS_PR_ROAD_CODE;
                    foreach (var plan in planData)
                    {
                        RoadCode = plan.IMS_PR_ROAD_CODE;
                        if (RoadCode > 0)
                            break;
                        else
                            continue;
                    }

                    planMaster.PLAN_ID = dbContext.PMIS_PLAN_MASTER.Any() ? dbContext.PMIS_PLAN_MASTER.Max(s => s.PLAN_ID) + 1 : 1;
                    planMaster.IMS_PR_ROAD_CODE = RoadCode; // dbContext.IMS_SANCTIONED_PROJECTS.Where(x => x.IMS_ROAD_NAME == RoadName).Select(x => x.IMS_PR_ROAD_CODE).FirstOrDefault();
                    planMaster.PLAN_CREATION_DATE = DateTime.Now;
                    planMaster.USERID = PMGSYSession.Current.UserId;
                    planMaster.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                    planMaster.IS_LATEST = "Y";
                    planMaster.IS_FINALISED = "N";
                    planMaster.BASELINE_NO = 1;
                    dbContext.PMIS_PLAN_MASTER.Add(planMaster);
                    dbContext.SaveChanges();

                    long k = dbContext.PMIS_PLAN_DETAILS.Any() ? dbContext.PMIS_PLAN_DETAILS.Max(s => s.DETAILED_ID) : 0;

                    foreach (var obj in planData)
                    {
                        PMIS_PLAN_DETAILS planDetails = new PMIS_PLAN_DETAILS();
                        planDetails.DETAILED_ID = ++k;      //dbContext.PMIS_PLAN_DETAILS.Any() ? dbContext.PMIS_PLAN_DETAILS.Max(s => s.DETAILED_ID) + 1 : 1;
                        planDetails.PLAN_ID = planMaster.PLAN_ID;
                        planDetails.IMS_PR_ROAD_CODE = planMaster.IMS_PR_ROAD_CODE;
                        //string activity = obj.ACTIVITY_DESC.Replace("\n"," ").Trim();
                        planDetails.ACTIVITY_ID = dbContext.PMIS_ACTIVITY_MASTER.Where(x => x.ACTIVITY_DESC == obj.ACTIVITY_DESC && x.ROAD_TYPE == "L").Select(x => x.ACTIVITY_ID).FirstOrDefault();
                        planDetails.QUANTITY = obj.QUANTITY;
                        planDetails.AGREEMENT_COST = obj.AGREEMENT_COST;
                        planDetails.PLANNED_START_DATE = obj.PLANNED_START_DATE;//== null ? "-" : obj.PLANNED_START_DATE.Value.To;
                        planDetails.PLANNED_DURATION = obj.PLANNED_DURATION;
                        planDetails.PLANNED_COMPLETION_DATE = planDetails.PLANNED_START_DATE != null ? obj.PLANNED_COMPLETION_DATE : null;
                        planDetails.IS_LATEST = "Y";
                        planDetails.BASELINE_NO = 1;
                        planDetails.USERID = PMGSYSession.Current.UserId;
                        planDetails.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                        dbContext.PMIS_PLAN_DETAILS.Add(planDetails);

                        dbContext.SaveChanges();
                    }
                    ts.Complete();
                    PMISLog("PMIS", ConfigurationManager.AppSettings["PMISLog"].ToString(), "Transaction Completed", "");
                }
                return string.Empty;
            }

            catch (DbUpdateException ex)
            {
                ErrorLog.LogError(ex, "SaveBridgeProjectPlanDAL(DbUpdateException ex).DAL");
                return ("An Error Occurred While Processing Your Request.");
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "SaveRoadProjectPlanDAL().DAL");
                return ("Error Occurred While Processing Request.");
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        public string DeleteBridgeProjectPlanDAL(int IMS_PR_ROAD_CODE, int PLAN_ID)
        {
            dbContext = new PMGSYEntities();
            try
            {
                if (dbContext.PMIS_PROGRESS_MASTER.Where(x => x.PLAN_ID == PLAN_ID && x.IMS_PR_ROAD_CODE == IMS_PR_ROAD_CODE).Any())
                {
                    return "This plan cannot be deleted as it contains actual progress details";
                }

                using (TransactionScope ts = new TransactionScope())
                {

                    if (dbContext.PMIS_PLAN_DETAILS.Any(m => m.IMS_PR_ROAD_CODE == IMS_PR_ROAD_CODE))
                    {
                        List<PMIS_PLAN_DETAILS> lstnewPlanDetails = dbContext.PMIS_PLAN_DETAILS.Where(m => m.IMS_PR_ROAD_CODE == IMS_PR_ROAD_CODE && m.IS_LATEST == "Y").ToList();
                        foreach (var item in lstnewPlanDetails)
                        {
                            dbContext.PMIS_PLAN_DETAILS.Remove(item);
                        }
                    }
                    //Added on 1 Feb 2021 to resolve DbUpdateException issue.
                    if (dbContext.PMIS_PROGRESS_DETAILS.Any(m => m.IMS_PR_ROAD_CODE == IMS_PR_ROAD_CODE))
                    {
                        List<PMIS_PROGRESS_DETAILS> listProgressDetails = dbContext.PMIS_PROGRESS_DETAILS.Where(m => m.IMS_PR_ROAD_CODE == IMS_PR_ROAD_CODE).ToList();
                        foreach (var item in listProgressDetails)
                        {
                            dbContext.PMIS_PROGRESS_DETAILS.Remove(item);
                        }
                    }
                    if (dbContext.PMIS_CHAINAGEWISE_COMPLETION_DETAILS.Any(m => m.IMS_PR_ROAD_CODE == IMS_PR_ROAD_CODE))
                    {
                        List<PMIS_CHAINAGEWISE_COMPLETION_DETAILS> listChainageDetails = dbContext.PMIS_CHAINAGEWISE_COMPLETION_DETAILS.Where(m => m.IMS_PR_ROAD_CODE == IMS_PR_ROAD_CODE).ToList();
                        foreach (var item in listChainageDetails)
                        {
                            dbContext.PMIS_CHAINAGEWISE_COMPLETION_DETAILS.Remove(item);
                        }
                    }
                    PMIS_PROGRESS_MASTER progressMaster = dbContext.PMIS_PROGRESS_MASTER.Where(z => z.IMS_PR_ROAD_CODE == IMS_PR_ROAD_CODE).FirstOrDefault();
                    if (progressMaster != null)
                    {
                        //dbContext.PMIS_PROGRESS_MASTER.Remove(progressMaster);
                        dbContext.Entry(progressMaster).State = EntityState.Deleted;
                    }

                    //End of changes
                    PMIS_PLAN_MASTER planMaster = dbContext.PMIS_PLAN_MASTER.Where(z => z.IMS_PR_ROAD_CODE == IMS_PR_ROAD_CODE & z.IS_LATEST == "Y").FirstOrDefault();
                    if (planMaster != null)
                    {
                        //dbContext.PMIS_PLAN_MASTER.Remove(planMaster);
                        dbContext.Entry(planMaster).State = EntityState.Deleted;
                    }

                    dbContext.SaveChanges();
                    PMIS_PLAN_MASTER oldmasterplan = dbContext.PMIS_PLAN_MASTER.Where(v => v.IMS_PR_ROAD_CODE == IMS_PR_ROAD_CODE).OrderByDescending(x => x.BASELINE_NO).FirstOrDefault();
                    if (oldmasterplan != null)
                    {
                        oldmasterplan.IS_LATEST = "Y";
                        dbContext.Entry(oldmasterplan).State = EntityState.Modified;
                        int? Baseline_No = oldmasterplan.BASELINE_NO;
                        List<PMIS_PLAN_DETAILS> lstoldPlanDetails = dbContext.PMIS_PLAN_DETAILS.Where(m => m.IMS_PR_ROAD_CODE == IMS_PR_ROAD_CODE & m.BASELINE_NO == Baseline_No).ToList();
                        foreach (var item in lstoldPlanDetails)
                        {
                            item.IS_LATEST = "Y";
                            dbContext.Entry(item).State = EntityState.Modified;
                        }
                    }
                    dbContext.SaveChanges();
                    ts.Complete();
                    return string.Empty;
                }
            }
            catch (DbUpdateException ex)
            {
                ErrorLog.LogError(ex, "DeleteBridgeProjectPlanDAL(DbUpdateException ex).DAL");
                return ("An Update Error Occurred While Processing Your Request.");
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "DeleteBridgeProjectPlanDAL().DAL");
                return ("Error Occurred While Processing Request.");
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        public string ReviseBridgeProjectPlanDAL(int IMS_PR_ROAD_CODE)
        {
            try
            {
                dbContext = new PMGSYEntities();
                using (TransactionScope ts = new TransactionScope())
                {

                    // if (dbContext.PMIS_PLAN_DETAILS.Any(m => m.IMS_PR_ROAD_CODE == IMS_PR_ROAD_CODE))
                    // {
                    PMIS_PLAN_MASTER RevisePlanMaster = dbContext.PMIS_PLAN_MASTER.Where(z => z.IMS_PR_ROAD_CODE == IMS_PR_ROAD_CODE & z.IS_FINALISED == "Y" & z.IS_LATEST == "Y").FirstOrDefault();

                    int planid = dbContext.PMIS_PLAN_MASTER.Max(s => s.PLAN_ID) + 1;
                    int baseLine = RevisePlanMaster.BASELINE_NO == null ? 0 : (int)(RevisePlanMaster.BASELINE_NO + 1);

                    if (RevisePlanMaster != null)
                    {
                        //For new Revise Entry
                        dbContext.PMIS_PLAN_MASTER.Add(new PMIS_PLAN_MASTER
                        {
                            PLAN_ID = planid,
                            IMS_PR_ROAD_CODE = RevisePlanMaster.IMS_PR_ROAD_CODE,
                            PLAN_CREATION_DATE = DateTime.Now,
                            USERID = PMGSYSession.Current.UserId,
                            IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"],
                            IS_LATEST = "Y",
                            BASELINE_NO = baseLine, // RevisePlanMaster.BASELINE_NO + 1,
                            IS_FINALISED = "N"
                        });

                        // For old 
                        RevisePlanMaster.IS_LATEST = "N";
                        dbContext.Entry(RevisePlanMaster).State = EntityState.Modified;
                        dbContext.SaveChanges();
                        //   }
                        // var adapter = (IObjectContextAdapter)dbContext;
                        //var objectContext = adapter.ObjectContext;
                        //objectContext.CommandTimeout = 0;
                        long count = dbContext.PMIS_PLAN_DETAILS.Max(s => s.DETAILED_ID) + 1;
                        List<PMIS_PLAN_DETAILS> lstPlanDetails = dbContext.PMIS_PLAN_DETAILS.Where(m => m.IMS_PR_ROAD_CODE == IMS_PR_ROAD_CODE && m.BASELINE_NO == RevisePlanMaster.BASELINE_NO && m.PLAN_ID == RevisePlanMaster.PLAN_ID).ToList();
                        // PMIS_PLAN_MASTER newplanmaster = dbContext.PMIS_PLAN_MASTER.Where(m => m.IMS_PR_ROAD_CODE == IMS_PR_ROAD_CODE & m.IS_LATEST == "Y").FirstOrDefault();
                        foreach (var item in lstPlanDetails)
                        {
                            //New record
                            PMIS_PLAN_DETAILS plandetails = new PMIS_PLAN_DETAILS();

                            plandetails.DETAILED_ID = count;
                            plandetails.PLAN_ID = planid;
                            plandetails.PLANNED_COMPLETION_DATE = item.PLANNED_COMPLETION_DATE;
                            plandetails.PLANNED_DURATION = item.PLANNED_DURATION;
                            plandetails.PLANNED_START_DATE = item.PLANNED_START_DATE;
                            plandetails.QUANTITY = item.QUANTITY;
                            plandetails.USERID = item.USERID;
                            plandetails.IPADD = item.IPADD;
                            plandetails.IS_LATEST = item.IS_LATEST;
                            plandetails.IMS_PR_ROAD_CODE = item.IMS_PR_ROAD_CODE;
                            plandetails.BASELINE_NO = baseLine; // item.BASELINE_NO + 1;
                            plandetails.AGREEMENT_COST = item.AGREEMENT_COST;
                            plandetails.ACTIVITY_ID = item.ACTIVITY_ID;


                            dbContext.PMIS_PLAN_DETAILS.Add(plandetails);
                            count++;

                            //Old record update
                            item.IS_LATEST = "N";
                            dbContext.Entry(item).State = EntityState.Modified;
                            dbContext.SaveChanges();
                        }
                        //using (TransactionScope ts = new TransactionScope())
                        //{
                        //dbContext.SaveChanges();
                        ts.Complete();

                    }

                    // }
                    return string.Empty;
                }

                return ("No Plans against this Road");
            }
            catch (DbUpdateException ex)
            {
                ErrorLog.LogError(ex, "ReviseRoadProjectPlanDAL(DbUpdateException ex).DAL");
                return ("An Update Error Occurred While Processing Your Request.");
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "ReviseRoadProjectPlanDAL().DAL");
                return ("Error Occurred While Processing Request.");
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        public string FinalizeBridgeProjectPlanDAL(int IMS_PR_ROAD_CODE)
        {
            try
            {
                dbContext = new PMGSYEntities();

                PMIS_PLAN_MASTER FinalizePlanMaster = dbContext.PMIS_PLAN_MASTER.Where(z => z.IMS_PR_ROAD_CODE == IMS_PR_ROAD_CODE & z.IS_LATEST == "Y").FirstOrDefault();
                //PMIS_PLAN_DETAILS planDetails = dbContext.PMIS_PLAN_DETAILS.Where(z => z.IMS_PR_ROAD_CODE == IMS_PR_ROAD_CODE & z.IS_LATEST == "Y" && z.ACTIVITY_ID == 1).FirstOrDefault();
                PMIS_PLAN_DETAILS planDetails = dbContext.PMIS_PLAN_DETAILS.Where(z => z.IMS_PR_ROAD_CODE == IMS_PR_ROAD_CODE & z.IS_LATEST == "Y" && z.ACTIVITY_ID == 18).FirstOrDefault();

                if (FinalizePlanMaster != null)
                {
                    if (planDetails.PLANNED_START_DATE != null && planDetails.PLANNED_COMPLETION_DATE != null)
                    {
                        FinalizePlanMaster.IS_FINALISED = "Y";
                        dbContext.Entry(FinalizePlanMaster).State = EntityState.Modified;
                        dbContext.SaveChanges();
                        return string.Empty;
                    }
                    else
                    {
                        return "Plan of a project cannot be finalized if the planned start date and planned end date of Field lab is not entered";
                    }
                }
                else
                {
                    return "No Plans against this Road ";
                }

            }
            catch (DbUpdateException ex)
            {
                ErrorLog.LogError(ex, "FinalizBridgeProjectPlanDAL(DbUpdateException ex).DAL");
                return ("An Update Error Occurred While Processing Your Request.");
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "FinalizBridgeProjectPlanDAL().DAL");
                return ("Error Occurred While Processing Request.");
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        public string UpdatePMISBridgeProjectPlanDAL(IEnumerable<AddPlanPMISViewModelBridge> planData)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            PMIS_PLAN_MASTER planMaster;
            try
            {
                using (TransactionScope ts = new TransactionScope())
                {

                    int RoadCode = 0;  //planData.ElementAt(0).IMS_PR_ROAD_CODE;
                    foreach (var plan in planData)
                    {
                        RoadCode = plan.IMS_PR_ROAD_CODE;
                        if (RoadCode > 0)
                            break;
                        else
                            continue;
                    }
                    //PMIS_PLAN_MASTER planMaster = dbContext.PMIS_PLAN_MASTER.Where(x => x.IMS_PR_ROAD_CODE == RoadCode && x.IS_LATEST == "Y" && x.IS_FINALISED != "Y").FirstOrDefault();
                    if (PMGSYSession.Current.RoleCode == 36)
                    {
                        planMaster = dbContext.PMIS_PLAN_MASTER.Where(x => x.IMS_PR_ROAD_CODE == RoadCode && x.IS_LATEST == "Y" && (x.IS_FINALISED != "Y" || x.IS_FINALISED == "Y")).FirstOrDefault();

                    }
                    else
                    {
                        planMaster = dbContext.PMIS_PLAN_MASTER.Where(x => x.IMS_PR_ROAD_CODE == RoadCode && x.IS_LATEST == "Y" && x.IS_FINALISED != "Y").FirstOrDefault();
                    }
                    planMaster.USERID = PMGSYSession.Current.UserId;
                    planMaster.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                    planMaster.IS_LATEST = "Y";
                    //planMaster.IS_FINALISED = "N";
                    dbContext.Entry(planMaster).State = EntityState.Modified;
                    dbContext.SaveChanges();

                    foreach (var obj in planData)
                    {
                        int ActivityId = dbContext.PMIS_ACTIVITY_MASTER.Where(x => x.ACTIVITY_DESC == obj.ACTIVITY_DESC && x.ROAD_TYPE == "L").Select(x => x.ACTIVITY_ID).FirstOrDefault();
                        PMIS_PLAN_DETAILS planDetails = dbContext.PMIS_PLAN_DETAILS.Where(x => x.IMS_PR_ROAD_CODE == RoadCode && x.IS_LATEST == "Y" && x.PLAN_ID == planMaster.PLAN_ID && x.ACTIVITY_ID == ActivityId).FirstOrDefault();
                        planDetails.IMS_PR_ROAD_CODE = planMaster.IMS_PR_ROAD_CODE;
                        planDetails.QUANTITY = obj.QUANTITY;
                        planDetails.AGREEMENT_COST = obj.AGREEMENT_COST;
                        planDetails.PLANNED_START_DATE = obj.PLANNED_START_DATE;
                        planDetails.PLANNED_DURATION = obj.PLANNED_DURATION;
                        planDetails.PLANNED_COMPLETION_DATE = obj.PLANNED_COMPLETION_DATE;
                        planDetails.IS_LATEST = "Y";
                        //planDetails.BASELINE_NO = 1;
                        planDetails.USERID = PMGSYSession.Current.UserId;
                        planDetails.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];

                        dbContext.Entry(planMaster).State = EntityState.Modified;
                        dbContext.SaveChanges();
                    }
                    ts.Complete();
                }
                return string.Empty;
            }

            catch (DbUpdateException ex)
            {
                ErrorLog.LogError(ex, "UpdatePMISBridgeProjectPlanDAL(DbUpdateException ex).DAL");
                return ("An Error Occurred While Processing Your Request.");
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "UpdatePMISBridgeProjectPlanDAL.DAL");
                return ("Error Occurred While Processing Request.");
            }
            finally
            {
                dbContext.Dispose();
            }
        }


        #endregion


        public Array PMISRoadListDAL(int state, int district, int block, int sanction_year, int batch, int? page, int? rows, string sidx, string sord, out long totalRecords)
        {

            try
            {
                // vikky latest
                dbContext = new PMGSYEntities();

                var PMISRoadList = dbContext.USP_PMS_ROAD_LIST(state, district, block, sanction_year, batch).ToList<USP_PMS_ROAD_LIST_Result>();

                //Added By Rohit Borse on 26-08-2023 for GPS VTS Freeze/Unfreeze Validation
                bool isVTSWorkUnfreeze = false;

                bool isVTSLastDateExceed = false;
                DateTime VTS_ENTRY_LASTDATE = Convert.ToDateTime(ConfigurationManager.AppSettings["VTS_ENTRY_LASTDATE"].ToString());

                // To Get Server DateTime
                DateTime utcTime = DateTime.UtcNow;
                TimeZoneInfo tzi = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");
                DateTime todaysDate = TimeZoneInfo.ConvertTimeFromUtc(utcTime, tzi); // convert from utc to local

                isVTSLastDateExceed = (VTS_ENTRY_LASTDATE > todaysDate) ? false : true;
                                                

                var resultList = new List<PMISRoadDAL>();

                foreach (var item in PMISRoadList)
                {
                    var RoadCode = dbContext.PMIS_PLAN_MASTER.Where(x => x.IMS_PR_ROAD_CODE == item.PrRoadCode).Select(c => c.IMS_PR_ROAD_CODE).FirstOrDefault();
                    var RevisePlanRoadCode = dbContext.PMIS_PLAN_MASTER.Where(x => x.IMS_PR_ROAD_CODE == item.PrRoadCode & x.IS_LATEST == "Y" & x.IS_FINALISED == "Y").Select(c => c.IMS_PR_ROAD_CODE).FirstOrDefault();
                    var PlanId = dbContext.PMIS_PLAN_MASTER.Where(x => x.IMS_PR_ROAD_CODE == item.PrRoadCode && x.IS_LATEST == "Y").Select(c => c.PLAN_ID).FirstOrDefault();
                    //    var Progress_Status = dbContext.PMIS_PROGRESS_MASTER.Where(x => x.PLAN_ID == PlanId).Select(c => c.PROJECT_STATUS_).FirstOrDefault();
                    var FinalizeStatus = dbContext.PMIS_PLAN_MASTER.Where(x => x.PLAN_ID == PlanId && x.IMS_PR_ROAD_CODE == item.PrRoadCode && x.IS_LATEST == "Y").Select(c => c.IS_FINALISED).FirstOrDefault();

                    bool IsRoad_ProgressEntered = false;
                    bool IsRoad_QCREntered_andFinalize = false;
                    bool IsWorkFreeze_RoadCode_Available = false;
                    bool Is_GPSVTS_Installed_OnWork = false;

                    // Changes added by Saurabh on 08-06-2023 for FDR Development

                    var technologyName = string.Join(", ", dbContext.IMS_PROPOSAL_TECH.Where(x => x.IMS_PR_ROAD_CODE == item.PrRoadCode).Select(x => x.MASTER_TECHNOLOGY.MAST_TECH_NAME).ToList());

                    bool isFDRFilled = dbContext.PMIS_TRIAL_STRETCH_FDR_DETAIL.Any(x => x.IMS_PR_ROAD_CODE == item.PrRoadCode);
                    bool IsFDRtech = false;
                    if ((dbContext.IMS_PROPOSAL_TECH.Where(x => x.IMS_PR_ROAD_CODE == item.PrRoadCode && x.MAST_TECH_CODE == 64).Any()))
                    {
                        IsFDRtech = true;
                    }
                    else
                    {
                        IsFDRtech = false;
                    }
                    var planMasterFinalized = dbContext.PMIS_PLAN_MASTER.Where(x => x.IMS_PR_ROAD_CODE == item.PrRoadCode & x.IS_LATEST == "Y" & x.IS_FINALISED == "Y").FirstOrDefault();
                    bool isFDR_SAMI_UPLOADED = false;
                    if (planMasterFinalized != null)
                    {
                        if (dbContext.PMIS_PLAN_DETAILS.Any(s => s.PLAN_ID == planMasterFinalized.PLAN_ID && s.IMS_PR_ROAD_CODE == RoadCode && s.ACTIVITY_ID == 41 && s.IS_LATEST == "Y") &&
                            dbContext.PMIS_PLAN_DETAILS.Any(s => s.PLAN_ID == planMasterFinalized.PLAN_ID && s.IMS_PR_ROAD_CODE == RoadCode && s.ACTIVITY_ID == 42 && s.IS_LATEST == "Y"))
                        {
                            isFDR_SAMI_UPLOADED = true;
                        }
                    }
                    bool isTrialStretchUploadedFinalized = false;
                    if (dbContext.PMIS_TRIAL_STRETCH_FDR_DETAIL.Any(s => s.IMS_PR_ROAD_CODE == RoadCode && s.IS_FINALIZED == "Y"))
                    {
                        isTrialStretchUploadedFinalized = true;

                    }

                    // Changes Ended Here..


                    

                    if (PMGSYSession.Current.PMGSYScheme == 4 && item.ProposalType.Trim().Equals("P"))
                    {
                        //added by rohit borse on 15-09-2023 for GPS VTS state relaxation to enter physical progress
                        string GPSVTS_StatesRelaxed = System.Configuration.ConfigurationManager.AppSettings["GPS_VTS_STATE_RELAXATION"];
                        var isCurrentStateRelxed = false;

                        if (!string.IsNullOrEmpty(GPSVTS_StatesRelaxed))
                        {
                            string[] GPSVTS_StatesRelaxedList = GPSVTS_StatesRelaxed.Split(',');
                            isCurrentStateRelxed = GPSVTS_StatesRelaxedList.Contains(PMGSYSession.Current.StateCode.ToString()) ? true : false;                            
                        }

                        if (isCurrentStateRelxed)
                        {
                            IsRoad_ProgressEntered = true;
                            IsRoad_QCREntered_andFinalize = true;
                            isVTSWorkUnfreeze = true;
                        }
                        else
                        {
                            IsRoad_ProgressEntered = dbContext.EXEC_ROADS_MONTHLY_STATUS.Where(r => r.IMS_PR_ROAD_CODE == item.PrRoadCode).Any() ? true : false;

                            if (IsRoad_ProgressEntered)
                            {
                                IsRoad_QCREntered_andFinalize = dbContext.QM_QCR_DETAILS.Where(s => s.IMS_PR_ROAD_CODE == item.PrRoadCode).Any()
                                                    ? (dbContext.QM_QCR_DETAILS.Where(s => s.IMS_PR_ROAD_CODE == item.PrRoadCode && (s.IS_FINALIZE == null || s.IS_FINALIZE.Trim().Equals("N"))).Count() > 0 ? false : true)
                                                    : false;

                                // Added for All Provided works need to freeze on 07-06-2023
                                //IsWorkFreeze_RoadCode_Available = dbContext.WORKS_FREEZE_VTS.Where(s => s.IMS_PR_ROAD_CODE == item.PrRoadCode).Where(x => x.Is_Active == true).Count() > 0 ? true : false;

                                // added after gps_vts screen deployed to check if freeze work has vts installed or not and if vts installed then freeze work should be unlock
                                //Is_GPSVTS_Installed_OnWork = dbContext.VTS_ROADWISE_GPS_AVAILABILITY.Where(x => x.IMS_PR_ROAD_CODE == item.PrRoadCode && x.GPS_INSTALLED.ToUpper() == "Y").Count() > 0 ? true : false;


                                //Added By Rohit Borse on 26-08-2023 for GPS VTS Freeze/Unfreeze Validation
                                // if last date exceeded then check that work is unfreeze by director login and vts pdf uploaded & finalize then only allow
                                // else vts last date (31-Aug-2023) not exceeded then check if VTS pdf uploaded and finalize then only allow
                                if (isVTSLastDateExceed)
                                {
                                    if (dbContext.VTS_UNFREEZE_WORKS.Where(s => s.IMS_PR_ROAD_CODE == item.PrRoadCode).Any())
                                    {

                                        isVTSWorkUnfreeze = (from unf in dbContext.VTS_UNFREEZE_WORKS                                                         
                                                             where unf.IMS_PR_ROAD_CODE == item.PrRoadCode
                                                             && unf.IS_UNFREEZED == "Y"                                                         
                                                             select new { unf.IMS_PR_ROAD_CODE }).Any();
                                    }
                                }
                                else
                                {
                                    isVTSWorkUnfreeze = true;
                                }


                            }

                        }

                    }


                    // var FinalizeRoadCode = dbContext.PMIS_PLAN_MASTER.Where(z => z.IMS_PR_ROAD_CODE == item.PrRoadCode & z.IS_LATEST == "Y" & z.IS_FINALISED == "N").Select(y => y.IMS_PR_ROAD_CODE).FirstOrDefault();
                    resultList.Add(new PMISRoadDAL
                    {
                        PlanId = PlanId,
                        StateName = item.MAST_STATE_NAME,
                        DistrictName = item.MAST_DISTRICT_NAME,
                        BlockName = item.BlockName,
                        PackageName = item.PackageId,
                        SanctionYear = (item.SanctionYear).ToString() + "-" + (item.SanctionYear + 1).ToString().Substring(2, 2),
                        SanctionDate = ((item.IMS_SANCTIONED_DATE).Value).ToShortDateString(),
                        BatchName = item.MAST_BATCH_NAME,
                        SanctionLength = (item.SanctionLength).ToString(),
                        AgreementNo = item.TEND_AGREEMENT_NUMBER,
                        AgreementCost = item.TEND_AGREEMENT_AMOUNT.ToString(),
                        MordShare = (item.Mord_Share ?? default(decimal)).ToString(),
                        StateShare = item.State_share.ToString(),
                        TotalSanctionedCost = item.TOTAL_COST.ToString(),
                        AgreementStartDate = (item.TEND_AGREEMENT_START_DATE == null) ? "NULL" : ((item.TEND_AGREEMENT_START_DATE).Value).ToShortDateString(),
                        AgreementEndDate = (item.TEND_AGREEMENT_END_DATE == null) ? "NULL" : ((item.TEND_AGREEMENT_END_DATE).Value).ToShortDateString(),
                        RoadName = item.RoadName,

                        //Change By Hrishikesh PMIS To Add Technology Name in Grid -12-06-2023
                        TECHNOLOGY_NAME = technologyName,

                        IMS_PR_RoadCode = item.PrRoadCode.ToString(),
                        IsPlanAvaliable = RoadCode == 0 ? item.PrRoadCode.ToString() + "$" : RoadCode.ToString(),
                        IsFinalize = RoadCode == 0 ? "-" : RevisePlanRoadCode != 0 ? RevisePlanRoadCode.ToString() + "$" : item.PrRoadCode.ToString(),
                        IsRevisePlan = RevisePlanRoadCode == 0 ? " " : RevisePlanRoadCode.ToString(),
                        IsActualsAvaliable = RoadCode == 0 ? "" : RoadCode.ToString(),
                        ActualLock = item.ProgressStatus,
                        ProgressStatus = item.ProgressStatus,
                        IsFinalized = FinalizeStatus,

                        // added on 19-05-2023 by rohit borse to decide wheather work will be freeze or not
                        Is_roadProgress_Entered = IsRoad_ProgressEntered,    
                        Is_roadQCR_Entered_andFinalize = IsRoad_QCREntered_andFinalize,
                        IsWork_Freeze_RoadCodeAvailable = IsWorkFreeze_RoadCode_Available,
                        IsGPSVTS_Installed_OnWork = Is_GPSVTS_Installed_OnWork,
                        IsVTSWorkUnfreeze = isVTSWorkUnfreeze,  //Added By Rohit Borse on 26-08-2023 for GPS VTS Freeze/Unfreeze Validation


                        // Changes Added by Saurabh on 08-06-2023 for FDR Development
                        IsFdrTechUsed = IsFDRtech,
                        isFDR_SAMI_UPLOADED = isFDR_SAMI_UPLOADED,
                        isTrialStretchUploadedFinalized = isTrialStretchUploadedFinalized,
                        IsFDRFilled = isFDRFilled,
                    });
                }

                totalRecords = PMISRoadList.Count();

                return resultList.Select(RoadDetails => new
                {
                    cell = new[] {    
                        RoadDetails.StateName,
                        RoadDetails.DistrictName,   
                        RoadDetails.BlockName,      
                        RoadDetails.PackageName,    
                        RoadDetails.SanctionYear,
                        RoadDetails.SanctionDate,
                        RoadDetails.BatchName,      
                        RoadDetails.SanctionLength, 
                        RoadDetails.AgreementNo,    
                        RoadDetails.AgreementCost, 
                        RoadDetails.MordShare,
                        RoadDetails.StateShare,
                        RoadDetails.TotalSanctionedCost,
                        RoadDetails.RoadName,
                         RoadDetails.TECHNOLOGY_NAME,  // Changes Added by Saurabh 
                   
                          RoadDetails.ActualLock == "C" ? "<center><table><tr><td  style='border:none'><span class='ui-icon ui-icon-locked ui-align-center ' title='Add Project Plan';'></span></td></tr></table></center>"
                        :RoadDetails.IsPlanAvaliable.EndsWith("$") ? "<center><table><tr><td  style='border:none'><span class='ui-icon ui-icon-circle-plus ' title='Add Project Plan' onClick =AddProjectPlan('"+ URLEncrypt.EncryptParameters1(new string[]{"RoadCode =" + RoadDetails.IMS_PR_RoadCode.ToString().Trim(),"StateShare =" + RoadDetails.StateShare.ToString().Trim(),"MordShare =" + RoadDetails.MordShare.ToString().Trim(),"TotalSanctionedDate =" + RoadDetails.TotalSanctionedCost.ToString().Trim()})+"');></span></td></tr></table></center>"//RoadDetails.IsPlanAvaliable 
                        : PMGSYSession.Current.RoleCode==36 ? "<center><table><tr><td  style='border:none;cursor:pointer'><span class='ui-icon ui-icon-pencil' title='Edit Project Plan' onClick =EditProjectPlan('"+ URLEncrypt.EncryptParameters1(new string[]{"RoadCode =" + RoadDetails.IsPlanAvaliable.ToString().Trim(),"StateShare =" + RoadDetails.StateShare.ToString().Trim(),"MordShare =" + RoadDetails.MordShare.ToString().Trim(),"TotalSanctionedDate =" + RoadDetails.TotalSanctionedCost.ToString().Trim()})+"');></span></td><td style='border:none;cursor:pointer'><span class='ui-icon ui-icon-search' title='View Project Plan' onClick ='ViewProjectPlan(\"" + URLEncrypt.EncryptParameters1(new string[]{"RoadCode =" + RoadDetails.IsPlanAvaliable.ToString().Trim(),"StateShare =" + RoadDetails.StateShare.ToString().Trim(),"MordShare =" + RoadDetails.MordShare.ToString().Trim(),"TotalSanctionedDate =" + RoadDetails.TotalSanctionedCost.ToString().Trim()}) + "\");'></span></td><td style='border:none;cursor:pointer'></td></tr></table></center>" 
                        :RoadDetails.IsFinalize.Contains("$") ? "<center><table><tr><td style='border:none;cursor:pointer'><span class='ui-icon ui-icon-search' title='View Project Plan' onClick =ViewProjectPlan('"+ URLEncrypt.EncryptParameters1(new string[]{"RoadCode =" + RoadDetails.IsPlanAvaliable.ToString().Trim(),"StateShare =" + RoadDetails.StateShare.ToString().Trim(),"MordShare =" + RoadDetails.MordShare.ToString().Trim(),"TotalSanctionedDate =" + RoadDetails.TotalSanctionedCost.ToString().Trim()})+"');></span></td></tr></table></center>"
                        :"<center><table><tr><td  style='border:none;cursor:pointer'><span class='ui-icon ui-icon-pencil' title='Edit Project Plan' onClick =EditProjectPlan('"+ URLEncrypt.EncryptParameters1(new string[]{"RoadCode =" + RoadDetails.IsPlanAvaliable.ToString().Trim(),"StateShare =" + RoadDetails.StateShare.ToString().Trim(),"MordShare =" + RoadDetails.MordShare.ToString().Trim(),"TotalSanctionedDate =" + RoadDetails.TotalSanctionedCost.ToString().Trim()})+"');></span></td><td style='border:none;cursor:pointer'><span class='ui-icon ui-icon-search' title='View Project Plan' onClick ='ViewProjectPlan(\"" + URLEncrypt.EncryptParameters1(new string[]{"RoadCode =" + RoadDetails.IsPlanAvaliable.ToString().Trim(),"StateShare =" + RoadDetails.StateShare.ToString().Trim(),"MordShare =" + RoadDetails.MordShare.ToString().Trim(),"TotalSanctionedDate =" + RoadDetails.TotalSanctionedCost.ToString().Trim()}) + "\");'></span></td><td style='border:none;cursor:pointer'><span class='ui-icon ui-icon-trash' title='Delete Project Plan' onClick ='DeleteProjectPlan(\"" + URLEncrypt.EncryptParameters1(new string[]{"RoadCode =" + RoadDetails.IsPlanAvaliable.ToString().Trim(), "PlanID =" +RoadDetails.PlanId.ToString().Trim()}) + "\");'></span></td></tr></table></center>",//URLEncrypt.EncryptParameters1(new string[]{"RoadCode =" + RoadDetails.IsPlanAvaliable.ToString().Trim()}),

                        RoadDetails.ActualLock == "C" ? "<center><table><tr><td  style='border:none'><span class='ui-icon ui-icon-locked ui-align-center ' title='Finalize Project Plan';'></span></td></tr></table></center>"
                        :RoadDetails.IsFinalize == "-" ? "-" :  RoadDetails.IsFinalize.Contains("$") ? "<center><table><tr><td  style='border:none'><span class='ui-icon ui-icon-locked ui-align-center ' title='Finalize Project Plan';'></span></td></tr></table></center>" :
                        "<center><table><tr><td  style='border:none'><span class='ui-icon ui-icon-unlocked ' title='Finalize Project Plan' onClick ='FinalizeProjectPlan(\"" + URLEncrypt.EncryptParameters1(new string[]{"RoadCode =" + RoadDetails.IsFinalize.ToString().Trim()})  + "\");'></span></td></tr></table></center>",
                         
                        // Revise Plan
                         RoadDetails.ActualLock == "C" ? "<center><table><tr><td  style='border:none'><span class='ui-icon ui-icon-locked ui-align-center ' title='Finalize Project Plan';'></span></td></tr></table></center>"
                        :RoadDetails.IsRevisePlan == " " ? "-" : "<a href='#' title='Click here to Revise Plan Details'  onClick=RevisePlanDetails('"+URLEncrypt.EncryptParameters1(new string[]{"RoadCode =" + RoadDetails.IsRevisePlan.ToString().Trim(),"StateShare =" + RoadDetails.StateShare.ToString().Trim(),"MordShare =" + RoadDetails.MordShare.ToString().Trim(),"TotalSanctionedDate =" + RoadDetails.TotalSanctionedCost.ToString().Trim()})+"'); return false;'>Revise Plan</a>",
                   

                         // Below Trial Stretch Detail Changes Done
                         RoadDetails.ActualLock == "C" ? "<center><table><tr><td  style='border:none'><span class='ui-icon ui-icon-locked ui-align-center ' title='Add Trail Stretch for FDR' > </span></td></tr></table></center>":
                         ((RoadDetails.IsFdrTechUsed != false || RoadDetails.IsFdrTechUsed == false) && (RoadDetails.IsPlanAvaliable.EndsWith("$") || RoadDetails.IsFinalized == "N")) ? "-"
                         :(RoadDetails.IsFdrTechUsed == false) ? "-":

                           (
                                                                     (RoadDetails.IsFdrTechUsed==true && RoadDetails.IsFDRFilled==true && RoadDetails.isTrialStretchUploadedFinalized==false)? "<center><table><tr><td  style='border:none'><span class='ui-icon ui-icon-pencil' title='Edit Trail Strech For FDR' onClick =AddTrailStrechForFDR('"+URLEncrypt.EncryptParameters1(new string[]{"RoadCode =" + RoadDetails.IsActualsAvaliable.ToString().Trim(),"PackageName="+RoadDetails.PackageName.ToString().Trim(),"BatchName="+RoadDetails.BatchName.ToString().Trim(),"Length="+RoadDetails.SanctionLength.ToString().Trim()}) +"');></span></td></tr></table></center>"
                                                                     :(RoadDetails.IsFdrTechUsed==true && RoadDetails.isTrialStretchUploadedFinalized==true)? "<center><table><tr><td  style='border:none'><span class='ui-icon ui-icon-search' title='View Trail Strech For FDR' onClick =AddTrailStrechForFDR('"+URLEncrypt.EncryptParameters1(new string[]{"RoadCode =" + RoadDetails.IsActualsAvaliable.ToString().Trim(),"PackageName="+RoadDetails.PackageName.ToString().Trim(),"BatchName="+RoadDetails.BatchName.ToString().Trim(),"Length="+RoadDetails.SanctionLength.ToString().Trim()}) +"');></span></td></tr></table></center>"
                                                                     :(RoadDetails.IsFdrTechUsed==true && RoadDetails.isFDR_SAMI_UPLOADED==true)?
                                                                               "<center><table><tr><td  style='border:none'><span class='ui-icon ui-icon-circle-plus ' title='Add Trail Stretch for FDR' onClick =AddTrailStrechForFDR('"+URLEncrypt.EncryptParameters1(new string[]{"RoadCode =" + RoadDetails.IsActualsAvaliable.ToString().Trim(),"PackageName="+RoadDetails.PackageName.ToString().Trim(),"BatchName="+RoadDetails.BatchName.ToString().Trim(),"Length="+RoadDetails.SanctionLength.ToString().Trim()}) +"');></span></td></tr></table></center>"
                                                                                :"<center><table><tr><td  style='border:none'><span class='ui-icon ui-icon-circle-plus ' onClick='FdrSAMITechNotUploadedAlert();return false;'   title='Add FDR Stabilized Base' > </span></td></tr></table></center>"
                           ),


                        //------Start  NEW Condition added with Existing on 19-05-2023 by rohit borse
                          //( (RoadDetails.Is_roadProgress_Entered == true && RoadDetails.Is_roadQCR_Entered_andFinalize == false )
                          //  ? "<span class='ui-icon ui-icon-locked ui-align-center' title='Work freeze due to QCR not uploaded or finalize' onclick='QcrNotUploaded_WorkFreeze();' > </span>"
                          //  : (RoadDetails.IsWork_Freeze_RoadCodeAvailable == true)
                          //      ? "<span class='ui-icon ui-icon-locked ui-align-center' title='Work freeze' onclick='GPSNotInstalled_WorksFreeze();' > </span>"

                          //      :   (RoadDetails.ActualLock == "C")? "<center><table><tr><td  style='border:none'><span class='ui-icon ui-icon-locked ui-align-center ' title='Add Actuals' > </span></td></tr></table></center>"
                          //          :(RoadDetails.IsPlanAvaliable.EndsWith("$") || RoadDetails.IsFinalized == "N") ? "-": "<center><table><tr><td  style='border:none'><span class='ui-icon ui-icon-circle-plus ' title='Add Actuals' onClick =AddActuals('"+URLEncrypt.EncryptParameters1(new string[]{"RoadCode =" + RoadDetails.IsActualsAvaliable.ToString().Trim(),"StateShare =" + RoadDetails.StateShare.ToString().Trim(),"MordShare =" + RoadDetails.MordShare.ToString().Trim(),"TotalSanctionedDate =" + RoadDetails.TotalSanctionedCost.ToString().Trim()})+"');></span></td></tr></table></center>"
                          //  ),
                        //------ End

                           // Add Actuals Changes Done
                          ( (RoadDetails.Is_roadProgress_Entered == true && RoadDetails.Is_roadQCR_Entered_andFinalize == false )
                            ? "<span class='ui-icon ui-icon-locked ui-align-center' title='Work freeze due to QCR not uploaded or finalize' onclick='QcrNotUploaded_WorkFreeze();' > </span>"
                            //Added By Rohit Borse on 26-08-2023 for GPS VTS Freeze/Unfreeze Validation
                            //: (RoadDetails.IsWork_Freeze_RoadCodeAvailable == true && RoadDetails.IsGPSVTS_Installed_OnWork == false)   // work freeze active and vts not installed then freeze work
                              : (RoadDetails.Is_roadProgress_Entered == true && RoadDetails.IsVTSWorkUnfreeze == false)
                                ? "<span class='ui-icon ui-icon-locked ui-align-center' title='Work freeze' onclick='GPSNotInstalled_WorksFreeze();' > </span>"

                                :   (RoadDetails.ActualLock == "C")? "<center><table><tr><td  style='border:none'><span class='ui-icon ui-icon-locked ui-align-center ' title='Add Actuals' > </span></td></tr></table></center>"
                                    :(RoadDetails.IsPlanAvaliable.EndsWith("$") || RoadDetails.IsFinalized == "N") ? "-" :
                                                               (
                                                                     RoadDetails.IsFdrTechUsed==true?
                                                                             (RoadDetails.isFDR_SAMI_UPLOADED==true?
                                                                                "<center><table><tr><td  style='border:none'><span class='ui-icon ui-icon-circle-plus ' title='Add Actuals' onClick =AddActuals('"+URLEncrypt.EncryptParameters1(new string[]{"RoadCode =" + RoadDetails.IsActualsAvaliable.ToString().Trim(),"StateShare =" + RoadDetails.StateShare.ToString().Trim(),"MordShare =" + RoadDetails.MordShare.ToString().Trim(),"TotalSanctionedDate =" + RoadDetails.TotalSanctionedCost.ToString().Trim()})+"');></span></td></tr></table></center>"
                                                                                :"<center><table><tr><td  style='border:none'><span class='ui-icon ui-icon-circle-plus ' onClick='FdrSAMITechNotUploadedAlert();return false;'   title='Add FDR Stabilized Base' > </span></td></tr></table></center>")
                                                                            :    "<center><table><tr><td  style='border:none'><span class='ui-icon ui-icon-circle-plus ' title='Add Actuals' onClick =AddActuals('"+URLEncrypt.EncryptParameters1(new string[]{"RoadCode =" + RoadDetails.IsActualsAvaliable.ToString().Trim(),"StateShare =" + RoadDetails.StateShare.ToString().Trim(),"MordShare =" + RoadDetails.MordShare.ToString().Trim(),"TotalSanctionedDate =" + RoadDetails.TotalSanctionedCost.ToString().Trim()})+"');></span></td></tr></table></center>"

                                                                )
                            ),

                           // Add FDR Chainage-Wise Detail Changes Done
                           RoadDetails.ActualLock == "C" ? "<center><table><tr><td  style='border:none'><span class='ui-icon ui-icon-locked ui-align-center ' title='Add FDR Stabilized Base' > </span></td></tr></table></center>":
                         (RoadDetails.IsPlanAvaliable.EndsWith("$") || RoadDetails.IsFinalized == "N") ? "-":
                                                                 (
                                                                     RoadDetails.IsFdrTechUsed==true?
                                                                             (RoadDetails.isFDR_SAMI_UPLOADED==true?
                                                                                    (RoadDetails.isTrialStretchUploadedFinalized==true?
                                                                                                    "<center><table><tr><td  style='border:none'><span class='ui-icon ui-icon-circle-plus ' title='Add FDR Stabilized Base' onClick =AddFDRDetail('"+URLEncrypt.EncryptParameters1(new string[]{"RoadCode =" + RoadDetails.IsActualsAvaliable.ToString().Trim(),"StateShare =" + RoadDetails.StateShare.ToString().Trim(),"MordShare =" + RoadDetails.MordShare.ToString().Trim(),"TotalSanctionedDate =" + RoadDetails.TotalSanctionedCost.ToString().Trim()})+"');></span></td></tr></table></center>"
                                                                                                    :"<center><table><tr><td  style='border:none'><span class='ui-icon ui-icon-circle-plus ' onClick='TrialStretchNotFinalizedAlert();return false;'   title='Add FDR Stabilized Base' > </span></td></tr></table></center>"
                                                                                    )

                                                                                       :"<center><table><tr><td  style='border:none'><span class='ui-icon ui-icon-circle-plus ' onClick='FdrSAMITechNotUploadedAlert();return false;'   title='Add FDR Stabilized Base' > </span></td></tr></table></center>")
                                                                            :   "-"
                                                                ),

                       // Add Chainage-Wise Detail Changes Done
                        (RoadDetails.ActualLock == "C")? "<center><table><tr><td  style='border:none'><span class='ui-icon ui-icon-locked ui-align-center ' title='Add Chainage wise Details' > </span></td></tr></table></center>"
                        :(RoadDetails.IsActualsAvaliable == "" || RoadDetails.IsFinalized== "N")? "-":
                                                                (
                                                                     RoadDetails.IsFdrTechUsed==true?
                                                                             (RoadDetails.isFDR_SAMI_UPLOADED==true?
                                                                               "<center><table><tr><td  style='border:none'><span class='ui-icon ui-icon-circle-plus ' title='Add Chainage wise Details' onClick =AddChainage('"+URLEncrypt.EncryptParameters1(new string[]{"RoadCode =" + RoadDetails.IsActualsAvaliable.ToString().Trim()})+"');></span></td></tr></table></center>"
                                                                                :"<center><table><tr><td  style='border:none'><span class='ui-icon ui-icon-circle-plus ' onClick='FdrSAMITechNotUploadedAlert();return false;'   title='Add FDR Stabilized Base' > </span></td></tr></table></center>")
                                                                            :   "<center><table><tr><td  style='border:none'><span class='ui-icon ui-icon-circle-plus ' title='Add Chainage wise Details' onClick =AddChainage('"+URLEncrypt.EncryptParameters1(new string[]{"RoadCode =" + RoadDetails.IsActualsAvaliable.ToString().Trim()})+"');></span></td></tr></table></center>"

                                                                ),


                    }
                }).ToArray();

            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "PMISRoadListDAL().DAL");
                totalRecords = 0;
                return null;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        public string SaveRoadProjectPlanDAL(IEnumerable<AddPlanPMISViewModel> planData)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            try
            {
                using (TransactionScope ts = new TransactionScope())
                {
                    PMIS_PLAN_MASTER planMaster = new PMIS_PLAN_MASTER();
                    int RoadCode = 0;  //planData.ElementAt(0).IMS_PR_ROAD_CODE;
                    foreach (var plan in planData)
                    {
                        RoadCode = plan.IMS_PR_ROAD_CODE;
                        if (RoadCode > 0)
                            break;
                        else
                            continue;
                    }

                    planMaster.PLAN_ID = dbContext.PMIS_PLAN_MASTER.Any() ? dbContext.PMIS_PLAN_MASTER.Max(s => s.PLAN_ID) + 1 : 1;
                    planMaster.IMS_PR_ROAD_CODE = RoadCode; // dbContext.IMS_SANCTIONED_PROJECTS.Where(x => x.IMS_ROAD_NAME == RoadName).Select(x => x.IMS_PR_ROAD_CODE).FirstOrDefault();
                    planMaster.PLAN_CREATION_DATE = DateTime.Now;
                    planMaster.USERID = PMGSYSession.Current.UserId;
                    planMaster.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                    planMaster.IS_LATEST = "Y";
                    planMaster.IS_FINALISED = "N";
                    planMaster.BASELINE_NO = 1;
                    dbContext.PMIS_PLAN_MASTER.Add(planMaster);
                    dbContext.SaveChanges();

                    long k = dbContext.PMIS_PLAN_DETAILS.Any() ? dbContext.PMIS_PLAN_DETAILS.Max(s => s.DETAILED_ID) : 0;

                    foreach (var obj in planData)
                    {
                        PMIS_PLAN_DETAILS planDetails = new PMIS_PLAN_DETAILS();
                        planDetails.DETAILED_ID = ++k;      //dbContext.PMIS_PLAN_DETAILS.Any() ? dbContext.PMIS_PLAN_DETAILS.Max(s => s.DETAILED_ID) + 1 : 1;
                        planDetails.PLAN_ID = planMaster.PLAN_ID;
                        planDetails.IMS_PR_ROAD_CODE = planMaster.IMS_PR_ROAD_CODE;
                        //string activity = obj.ACTIVITY_DESC.Replace("\n"," ").Trim();
                        planDetails.ACTIVITY_ID = dbContext.PMIS_ACTIVITY_MASTER.Where(x => x.ACTIVITY_DESC == obj.ACTIVITY_DESC).Select(x => x.ACTIVITY_ID).FirstOrDefault();
                        planDetails.QUANTITY = obj.QUANTITY;
                        planDetails.AGREEMENT_COST = obj.AGREEMENT_COST;
                        planDetails.PLANNED_START_DATE = obj.PLANNED_START_DATE;//== null ? "-" : obj.PLANNED_START_DATE.Value.To;
                        planDetails.PLANNED_DURATION = obj.PLANNED_DURATION;
                        planDetails.PLANNED_COMPLETION_DATE = planDetails.PLANNED_START_DATE != null ? obj.PLANNED_COMPLETION_DATE : null;
                        planDetails.IS_LATEST = "Y";
                        planDetails.BASELINE_NO = 1;
                        planDetails.USERID = PMGSYSession.Current.UserId;
                        planDetails.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                        dbContext.PMIS_PLAN_DETAILS.Add(planDetails);

                        dbContext.SaveChanges();
                    }
                    ts.Complete();
                    PMISLog("PMIS", ConfigurationManager.AppSettings["PMISLog"].ToString(), "Transaction Completed", "");
                }
                return string.Empty;
            }

            catch (DbUpdateException ex)
            {
                ErrorLog.LogError(ex, "SaveRoadProjectPlanDAL(DbUpdateException ex).DAL");
                return ("An Error Occurred While Processing Your Request.");
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "SaveRoadProjectPlanDAL().DAL");
                return ("Error Occurred While Processing Request.");
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        public string DeleteRoadProjectPlanDAL(int IMS_PR_ROAD_CODE, int PLAN_ID)
        {
            dbContext = new PMGSYEntities();
            try
            {
                if (dbContext.PMIS_PROGRESS_MASTER.Where(x => x.PLAN_ID == PLAN_ID && x.IMS_PR_ROAD_CODE == IMS_PR_ROAD_CODE).Any())
                {
                    return "This plan cannot be deleted as it contains actual progress details";
                }

                using (TransactionScope ts = new TransactionScope())
                {

                    if (dbContext.PMIS_PLAN_DETAILS.Any(m => m.IMS_PR_ROAD_CODE == IMS_PR_ROAD_CODE))
                    {
                        List<PMIS_PLAN_DETAILS> lstnewPlanDetails = dbContext.PMIS_PLAN_DETAILS.Where(m => m.IMS_PR_ROAD_CODE == IMS_PR_ROAD_CODE && m.IS_LATEST == "Y").ToList();
                        foreach (var item in lstnewPlanDetails)
                        {
                            dbContext.PMIS_PLAN_DETAILS.Remove(item);
                        }
                    }
                    //Added on 1 Feb 2021 to resolve DbUpdateException issue.
                    if (dbContext.PMIS_PROGRESS_DETAILS.Any(m => m.IMS_PR_ROAD_CODE == IMS_PR_ROAD_CODE))
                    {
                        List<PMIS_PROGRESS_DETAILS> listProgressDetails = dbContext.PMIS_PROGRESS_DETAILS.Where(m => m.IMS_PR_ROAD_CODE == IMS_PR_ROAD_CODE).ToList();
                        foreach (var item in listProgressDetails)
                        {
                            dbContext.PMIS_PROGRESS_DETAILS.Remove(item);
                        }
                    }
                    if (dbContext.PMIS_CHAINAGEWISE_COMPLETION_DETAILS.Any(m => m.IMS_PR_ROAD_CODE == IMS_PR_ROAD_CODE))
                    {
                        List<PMIS_CHAINAGEWISE_COMPLETION_DETAILS> listChainageDetails = dbContext.PMIS_CHAINAGEWISE_COMPLETION_DETAILS.Where(m => m.IMS_PR_ROAD_CODE == IMS_PR_ROAD_CODE).ToList();
                        foreach (var item in listChainageDetails)
                        {
                            dbContext.PMIS_CHAINAGEWISE_COMPLETION_DETAILS.Remove(item);
                        }
                    }
                    PMIS_PROGRESS_MASTER progressMaster = dbContext.PMIS_PROGRESS_MASTER.Where(z => z.IMS_PR_ROAD_CODE == IMS_PR_ROAD_CODE).FirstOrDefault();
                    if (progressMaster != null)
                    {
                        //dbContext.PMIS_PROGRESS_MASTER.Remove(progressMaster);
                        dbContext.Entry(progressMaster).State = EntityState.Deleted;
                    }

                    //End of changes
                    PMIS_PLAN_MASTER planMaster = dbContext.PMIS_PLAN_MASTER.Where(z => z.IMS_PR_ROAD_CODE == IMS_PR_ROAD_CODE & z.IS_LATEST == "Y").FirstOrDefault();
                    if (planMaster != null)
                    {
                        //dbContext.PMIS_PLAN_MASTER.Remove(planMaster);
                        dbContext.Entry(planMaster).State = EntityState.Deleted;
                    }

                    dbContext.SaveChanges();
                    PMIS_PLAN_MASTER oldmasterplan = dbContext.PMIS_PLAN_MASTER.Where(v => v.IMS_PR_ROAD_CODE == IMS_PR_ROAD_CODE).OrderByDescending(x => x.BASELINE_NO).FirstOrDefault();
                    if (oldmasterplan != null)
                    {
                        oldmasterplan.IS_LATEST = "Y";
                        dbContext.Entry(oldmasterplan).State = System.Data.Entity.EntityState.Modified;
                        int? Baseline_No = oldmasterplan.BASELINE_NO;
                        List<PMIS_PLAN_DETAILS> lstoldPlanDetails = dbContext.PMIS_PLAN_DETAILS.Where(m => m.IMS_PR_ROAD_CODE == IMS_PR_ROAD_CODE & m.BASELINE_NO == Baseline_No).ToList();
                        foreach (var item in lstoldPlanDetails)
                        {
                            item.IS_LATEST = "Y";
                            dbContext.Entry(item).State = System.Data.Entity.EntityState.Modified;
                        }
                    }
                    dbContext.SaveChanges();
                    ts.Complete();
                    return string.Empty;
                }
            }
            catch (DbUpdateException ex)
            {
                ErrorLog.LogError(ex, "DeleteRoadProjectPlanDAL(DbUpdateException ex).DAL");
                return ("An Update Error Occurred While Processing Your Request.");
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "DeleteRoadProjectPlanDAL().DAL");
                return ("Error Occurred While Processing Request.");
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        public string ReviseRoadProjectPlanDAL(int IMS_PR_ROAD_CODE)
        {
            try
            {
                dbContext = new PMGSYEntities();
                using (TransactionScope ts = new TransactionScope())
                {

                    // if (dbContext.PMIS_PLAN_DETAILS.Any(m => m.IMS_PR_ROAD_CODE == IMS_PR_ROAD_CODE))
                    // {
                    PMIS_PLAN_MASTER RevisePlanMaster = dbContext.PMIS_PLAN_MASTER.Where(z => z.IMS_PR_ROAD_CODE == IMS_PR_ROAD_CODE & z.IS_FINALISED == "Y" & z.IS_LATEST == "Y").FirstOrDefault();

                    int planid = dbContext.PMIS_PLAN_MASTER.Max(s => s.PLAN_ID) + 1;
                    int baseLine = RevisePlanMaster.BASELINE_NO == null ? 0 : (int)(RevisePlanMaster.BASELINE_NO + 1);

                    if (RevisePlanMaster != null)
                    {
                        //For new Revise Entry
                        dbContext.PMIS_PLAN_MASTER.Add(new PMIS_PLAN_MASTER
                        {
                            PLAN_ID = planid,
                            IMS_PR_ROAD_CODE = RevisePlanMaster.IMS_PR_ROAD_CODE,
                            PLAN_CREATION_DATE = DateTime.Now,
                            USERID = PMGSYSession.Current.UserId,
                            IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"],
                            IS_LATEST = "Y",
                            BASELINE_NO = baseLine, // RevisePlanMaster.BASELINE_NO + 1,
                            IS_FINALISED = "N"
                        });

                        // For old 
                        RevisePlanMaster.IS_LATEST = "N";
                        dbContext.Entry(RevisePlanMaster).State = System.Data.Entity.EntityState.Modified;
                        dbContext.SaveChanges();
                        //   }
                        // var adapter = (IObjectContextAdapter)dbContext;
                        //var objectContext = adapter.ObjectContext;
                        //objectContext.CommandTimeout = 0;
                        long count = dbContext.PMIS_PLAN_DETAILS.Max(s => s.DETAILED_ID) + 1;
                        List<PMIS_PLAN_DETAILS> lstPlanDetails = dbContext.PMIS_PLAN_DETAILS.Where(m => m.IMS_PR_ROAD_CODE == IMS_PR_ROAD_CODE && m.BASELINE_NO == RevisePlanMaster.BASELINE_NO && m.PLAN_ID == RevisePlanMaster.PLAN_ID).ToList();
                        // PMIS_PLAN_MASTER newplanmaster = dbContext.PMIS_PLAN_MASTER.Where(m => m.IMS_PR_ROAD_CODE == IMS_PR_ROAD_CODE & m.IS_LATEST == "Y").FirstOrDefault();
                        foreach (var item in lstPlanDetails)
                        {
                            //New record
                            PMIS_PLAN_DETAILS plandetails = new PMIS_PLAN_DETAILS();

                            plandetails.DETAILED_ID = count;
                            plandetails.PLAN_ID = planid;
                            plandetails.PLANNED_COMPLETION_DATE = item.PLANNED_COMPLETION_DATE;
                            plandetails.PLANNED_DURATION = item.PLANNED_DURATION;
                            plandetails.PLANNED_START_DATE = item.PLANNED_START_DATE;
                            plandetails.QUANTITY = item.QUANTITY;
                            plandetails.USERID = item.USERID;
                            plandetails.IPADD = item.IPADD;
                            plandetails.IS_LATEST = item.IS_LATEST;
                            plandetails.IMS_PR_ROAD_CODE = item.IMS_PR_ROAD_CODE;
                            plandetails.BASELINE_NO = baseLine; // item.BASELINE_NO + 1;
                            plandetails.AGREEMENT_COST = item.AGREEMENT_COST;
                            plandetails.ACTIVITY_ID = item.ACTIVITY_ID;


                            dbContext.PMIS_PLAN_DETAILS.Add(plandetails);
                            count++;

                            //Old record update
                            item.IS_LATEST = "N";
                            dbContext.Entry(item).State = System.Data.Entity.EntityState.Modified;
                            dbContext.SaveChanges();
                        }
                        //using (TransactionScope ts = new TransactionScope())
                        //{
                        //dbContext.SaveChanges();
                        ts.Complete();

                    }

                    // }
                    return string.Empty;
                }

                return ("No Plans against this Road");
            }
            catch (DbUpdateException ex)
            {
                ErrorLog.LogError(ex, "ReviseRoadProjectPlanDAL(DbUpdateException ex).DAL");
                return ("An Update Error Occurred While Processing Your Request.");
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "ReviseRoadProjectPlanDAL().DAL");
                return ("Error Occurred While Processing Request.");
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        public string FinalizeRoadProjectPlanDAL(int IMS_PR_ROAD_CODE)
        {
            try
            {
                dbContext = new PMGSYEntities();

                PMIS_PLAN_MASTER FinalizePlanMaster = dbContext.PMIS_PLAN_MASTER.Where(z => z.IMS_PR_ROAD_CODE == IMS_PR_ROAD_CODE & z.IS_LATEST == "Y").FirstOrDefault();
                PMIS_PLAN_DETAILS planDetails = dbContext.PMIS_PLAN_DETAILS.Where(z => z.IMS_PR_ROAD_CODE == IMS_PR_ROAD_CODE & z.IS_LATEST == "Y" && z.ACTIVITY_ID == 1).FirstOrDefault();

                // below code added by Saurabh on 19-06-2023
                bool isFDR_SAMI_UPLOADED = false;
                if (FinalizePlanMaster != null)
                {
                    if ((dbContext.IMS_PROPOSAL_TECH.Where(x => x.IMS_PR_ROAD_CODE == IMS_PR_ROAD_CODE && x.MAST_TECH_CODE == 64).Any()))
                    {
                        if (dbContext.PMIS_PLAN_DETAILS.Any(s => s.PLAN_ID == FinalizePlanMaster.PLAN_ID && s.IMS_PR_ROAD_CODE == IMS_PR_ROAD_CODE && s.ACTIVITY_ID == 41 && s.IS_LATEST == "Y") &&
                        dbContext.PMIS_PLAN_DETAILS.Any(s => s.PLAN_ID == FinalizePlanMaster.PLAN_ID && s.IMS_PR_ROAD_CODE == IMS_PR_ROAD_CODE && s.ACTIVITY_ID == 42 && s.IS_LATEST == "Y"))
                        {
                            isFDR_SAMI_UPLOADED = true;
                        }
                        else
                        {
                            return "Please enter the details for FDR Stabilized Base and Crack Relief Layer activity in Plan.";
                        }
                    }
                }
                // above code added by Saurabh Ends Here.

                if (FinalizePlanMaster != null)
                {
                    if (planDetails.PLANNED_START_DATE != null && planDetails.PLANNED_COMPLETION_DATE != null)
                    {
                        FinalizePlanMaster.IS_FINALISED = "Y";
                        dbContext.Entry(FinalizePlanMaster).State = System.Data.Entity.EntityState.Modified;
                        dbContext.SaveChanges();
                        return string.Empty;
                    }
                    else
                    {
                        return "Plan of a project cannot be finalized if the planned start date and planned end date of Field lab is not entered";
                    }
                }
                else
                {
                    return "No Plans against this Road ";
                }

            }
            catch (DbUpdateException ex)
            {
                ErrorLog.LogError(ex, "FinalizeRoadProjectPlanDAL(DbUpdateException ex).DAL");
                return ("An Update Error Occurred While Processing Your Request.");
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "FinalizeRoadProjectPlanDAL().DAL");
                return ("Error Occurred While Processing Request.");
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        public string UpdatePMISRoadProjectPlanDAL(IEnumerable<AddPlanPMISViewModel> planData)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            PMIS_PLAN_MASTER planMaster;
            try
            {
                using (TransactionScope ts = new TransactionScope())
                {

                    int RoadCode = 0;  //planData.ElementAt(0).IMS_PR_ROAD_CODE;
                    foreach (var plan in planData)
                    {
                        RoadCode = plan.IMS_PR_ROAD_CODE;
                        if (RoadCode > 0)
                            break;
                        else
                            continue;
                    }
                    //PMIS_PLAN_MASTER planMaster = dbContext.PMIS_PLAN_MASTER.Where(x => x.IMS_PR_ROAD_CODE == RoadCode && x.IS_LATEST == "Y" && x.IS_FINALISED != "Y").FirstOrDefault();
                    if (PMGSYSession.Current.RoleCode == 36)
                    {
                        planMaster = dbContext.PMIS_PLAN_MASTER.Where(x => x.IMS_PR_ROAD_CODE == RoadCode && x.IS_LATEST == "Y" && (x.IS_FINALISED != "Y" || x.IS_FINALISED == "Y")).FirstOrDefault();

                    }
                    else
                    {
                        planMaster = dbContext.PMIS_PLAN_MASTER.Where(x => x.IMS_PR_ROAD_CODE == RoadCode && x.IS_LATEST == "Y" && x.IS_FINALISED != "Y").FirstOrDefault();
                    }
                    planMaster.USERID = PMGSYSession.Current.UserId;
                    planMaster.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                    planMaster.IS_LATEST = "Y";
                    //planMaster.IS_FINALISED = "N";
                    dbContext.Entry(planMaster).State = System.Data.Entity.EntityState.Modified;
                    dbContext.SaveChanges();

                    foreach (var obj in planData)
                    {
                        int ActivityId = dbContext.PMIS_ACTIVITY_MASTER.Where(x => x.ACTIVITY_DESC == obj.ACTIVITY_DESC).Select(x => x.ACTIVITY_ID).FirstOrDefault();
                        PMIS_PLAN_DETAILS planDetails = dbContext.PMIS_PLAN_DETAILS.Where(x => x.IMS_PR_ROAD_CODE == RoadCode && x.IS_LATEST == "Y" && x.PLAN_ID == planMaster.PLAN_ID && x.ACTIVITY_ID == ActivityId).FirstOrDefault();

                        if (planDetails != null)
                        {
                            planDetails.IMS_PR_ROAD_CODE = planMaster.IMS_PR_ROAD_CODE;
                            planDetails.QUANTITY = obj.QUANTITY;
                            planDetails.AGREEMENT_COST = obj.AGREEMENT_COST;
                            planDetails.PLANNED_START_DATE = obj.PLANNED_START_DATE;
                            planDetails.PLANNED_DURATION = obj.PLANNED_DURATION;
                            planDetails.PLANNED_COMPLETION_DATE = obj.PLANNED_COMPLETION_DATE;
                            planDetails.IS_LATEST = "Y";
                            //planDetails.BASELINE_NO = 1;
                            planDetails.USERID = PMGSYSession.Current.UserId;
                            planDetails.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                            dbContext.Entry(planMaster).State = System.Data.Entity.EntityState.Modified;
                            dbContext.SaveChanges();
                        }
                        else
                        {
                            PMIS_PLAN_DETAILS planDetails1 = new PMIS_PLAN_DETAILS();

                            int ActivityId1 = dbContext.PMIS_ACTIVITY_MASTER.Where(x => x.ACTIVITY_DESC == obj.ACTIVITY_DESC).Select(x => x.ACTIVITY_ID).FirstOrDefault();
                            planDetails1.DETAILED_ID = dbContext.PMIS_PLAN_DETAILS.Any() ? (from t in dbContext.PMIS_PLAN_DETAILS select t.DETAILED_ID).Max() + 1 : 1;
                            planDetails1.PLAN_ID = planMaster.PLAN_ID;
                            planDetails1.IMS_PR_ROAD_CODE = planMaster.IMS_PR_ROAD_CODE;
                            planDetails1.ACTIVITY_ID = ActivityId1;
                            planDetails1.QUANTITY = obj.QUANTITY;
                            planDetails1.AGREEMENT_COST = obj.AGREEMENT_COST;
                            planDetails1.PLANNED_START_DATE = obj.PLANNED_START_DATE;
                            planDetails1.PLANNED_DURATION = obj.PLANNED_DURATION;
                            planDetails1.PLANNED_COMPLETION_DATE = obj.PLANNED_COMPLETION_DATE;
                            planDetails1.IS_LATEST = "Y";
                            planDetails1.BASELINE_NO = Convert.ToInt32(planMaster.BASELINE_NO);
                            planDetails1.USERID = PMGSYSession.Current.UserId;
                            planDetails1.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];

                            dbContext.PMIS_PLAN_DETAILS.Add(planDetails1);
                            dbContext.SaveChanges();
                        }

                        //planDetails.IMS_PR_ROAD_CODE = planMaster.IMS_PR_ROAD_CODE;
                        //planDetails.QUANTITY = obj.QUANTITY;
                        //planDetails.AGREEMENT_COST = obj.AGREEMENT_COST;
                        //planDetails.PLANNED_START_DATE = obj.PLANNED_START_DATE;
                        //planDetails.PLANNED_DURATION = obj.PLANNED_DURATION;
                        //planDetails.PLANNED_COMPLETION_DATE = obj.PLANNED_COMPLETION_DATE;
                        //planDetails.IS_LATEST = "Y";
                        ////planDetails.BASELINE_NO = 1;
                        //planDetails.USERID = PMGSYSession.Current.UserId;
                        //planDetails.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];

                        //dbContext.Entry(planMaster).State = System.Data.Entity.EntityState.Modified;
                        //dbContext.SaveChanges();
                    }
                    ts.Complete();
                }
                return string.Empty;
            }

            catch (DbUpdateException ex)
            {
                ErrorLog.LogError(ex, "UpdatePMISRoadProjectPlanDAL(DbUpdateException ex).DAL");
                return ("An Error Occurred While Processing Your Request.");
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "UpdatePMISRoadProjectPlanDAL.DAL");
                return ("Error Occurred While Processing Request.");
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        public string SaveActualsDAL(IEnumerable<AddActualsViewModel> planData)
        {
            PMGSYEntities dbContext = new PMGSYEntities();

            // Added on 28-03-2022 by Srishti Tyagi
            string AprilMonthStartDay = ConfigurationManager.AppSettings["APRIL_MONTH_START_DAY"];   //1
            int AprilMonthStartDayValue = Convert.ToInt16(AprilMonthStartDay);

            string AprilMonthEndDay = ConfigurationManager.AppSettings["APRIL_MONTH_END_DAY"];   //15
            int AprilMonthEndDayValue = Convert.ToInt16(AprilMonthEndDay);

            string AprilMonth = ConfigurationManager.AppSettings["APRIL_MONTH"];  //4
            int AprilMonthValue = Convert.ToInt16(AprilMonth);

            DateTime FinanDate = DateTime.Now;                         // change by saurabh
            //int FinancialYear = FinanDate.Month >= 4 ? FinanDate.Year : FinanDate.Year - 1;   // change by saurabh
            //DateTime Conditional_Date_Value = new DateTime(FinancialYear +1, 3, 31, 00, 00, 00);     // change by saurabh


            int FinancialYear = FinanDate.Month >= 4 ? FinanDate.Year : FinanDate.Year;
            DateTime Conditional_Date_Value = new DateTime(FinancialYear, 3, 31, 00, 00, 00);
            var CondFinanYear = FinancialYear ; // CHANGE

            // Changes end here 

            try
            {
                using (TransactionScope ts = new TransactionScope())
                {

                    int RoadCode = 0;  //planData.ElementAt(0).IMS_PR_ROAD_CODE;
                    char Project_Status = 'P';
                    Nullable<DateTime> Entry_Date = new DateTime();
                    String ProjectStatusValid = String.Empty;
                    Decimal ComplLengthValid = 0;

                    foreach (var plan in planData)
                    {
                        RoadCode = plan.IMS_PR_ROAD_CODE;
                        Project_Status = (plan.ProjectStatus == 'C' ? 'C' : 'P');
                        ProjectStatusValid = Convert.ToString(plan.ProjectStatus);
                        ComplLengthValid = plan.CompletedRoadLength;
                       // plan.Date_of_progress_entry = new DateTime(2023, 3, 31, 00, 00, 00);   // Commentable Change
                        if (plan.Date_of_progress_entry != null)
                        {
                            //Entry_Date = (plan.Date_of_progress_entry.Value.Day >= AprilMonthStartDayValue
                            //    && plan.Date_of_progress_entry.Value.Day <= AprilMonthEndDayValue
                            //    && plan.Date_of_progress_entry.Value.Month == AprilMonthValue) ? Conditional_Date_Value : plan.Date_of_progress_entry;

                            Entry_Date = (plan.Date_of_progress_entry.Value.Day >= AprilMonthStartDayValue
                               && plan.Date_of_progress_entry.Value.Day <= AprilMonthEndDayValue
                               && plan.Date_of_progress_entry.Value.Month == AprilMonthValue) ? Conditional_Date_Value : DateTime.Now ;

                        }
                        else 
                        {
                            return ("Date of progress entry is mandatory to Select.");
                        }
                        
                    
                        if (ProjectStatusValid == "0")
                        {
                            return ("Project Status is mandatory to Select.");
                        }
                        if (ProjectStatusValid != "P" && ProjectStatusValid != "W" && ProjectStatusValid != "H" && ProjectStatusValid != "L" && ProjectStatusValid != "F" && ProjectStatusValid != "A")
                        {
                            if (ComplLengthValid == 0)
                            {
                                return ("Completion Length is mandatory to fill.");
                            }
                        }
                        if (Entry_Date == null)
                        {
                            return ("Date of progress entry is mandatory to Select.");
                        }
                        //if (plan.Date_of_progress_entry.Value.Year * 12 + plan.Date_of_progress_entry.Value.Month <= FinancialYear * 12 + 3)   // Change by Saurabh
                        //{
                        //    return ("Progress can be entered in Current Financial Year");
                        //}

                        //if (FinanDate.Month == AprilMonthValue && FinanDate.Day > AprilMonthEndDayValue)   // CHANGE
                        //{
                        //    if (Entry_Date.Value.Year * 12 + Entry_Date.Value.Month <= CondFinanYear * 12 + 3)
                        //    {
                        //        return ("Progress can be entered in Current Date of Current Financial Year");
                        //    }
                        //}

                        if (FinanDate.Month != Entry_Date.Value.Month || FinanDate.Day != Entry_Date.Value.Day || FinanDate.Year != Entry_Date.Value.Year)
                        {
                            return ("Progress can be entered in Current Date of Current Financial Year");
                        }


                        if (RoadCode > 0)
                            break;
                    }
                    //foreach (var dateValue in planData)
                    //{
                    //    if (dateValue.Date_of_progress_entry == null)
                    //    {
                    //        return ("Date of progress entry is mandatory to fill.");
                    //    }
                    //    if (dateValue.Date_of_progress_entry.Value.Year * 12 + dateValue.Date_of_progress_entry.Value.Month <= 2022 * 12 + 3)
                    //    {
                    //        return ("Progress can be entered in Current Financial Year");
                    //    }

                    //}
                    if (dbContext.PMIS_PROGRESS_MASTER.Where(x => x.IMS_PR_ROAD_CODE == RoadCode).Any())
                    {
                        foreach (var plan in planData)
                        {
                            RoadCode = plan.IMS_PR_ROAD_CODE;
                            var latest_Master = dbContext.PMIS_PROGRESS_MASTER.Where(x => x.IMS_PR_ROAD_CODE == RoadCode & x.IS_LATEST == "Y").FirstOrDefault();
                            if (latest_Master != null && RoadCode != 0)
                            {
                                latest_Master.COMPLETION_LENGTH = plan.CompletedRoadLength;
                                latest_Master.PROJECT_STATUS_ = Convert.ToString(plan.ProjectStatus);
                                latest_Master.USERID = PMGSYSession.Current.UserId;
                                latest_Master.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                                latest_Master.IS_LATEST = "Y";
                                // latest_Master.ENTRY_DATE = plan.Date_of_progress_entry ?? DateTime.Now;
                                //latest_Master.ENTRY_DATE = (plan.Date_of_progress_entry.Value.Day >= AprilMonthStartDayValue
                                // && plan.Date_of_progress_entry.Value.Day <= AprilMonthEndDayValue
                                // && plan.Date_of_progress_entry.Value.Month == AprilMonthValue)
                                // ? Conditional_Date_Value : plan.Date_of_progress_entry ?? default(DateTime);

                                latest_Master.ENTRY_DATE = Convert.ToDateTime(Entry_Date);  // change by saurabh

                                latest_Master.REMARKS = String.IsNullOrEmpty(plan.Remarks) ? null : plan.Remarks;
                                dbContext.Entry(latest_Master).State = System.Data.Entity.EntityState.Modified;
                                dbContext.SaveChanges();
                            }
                            break;
                        }


                        foreach (var obj in planData)
                        {
                            var activity_ID = dbContext.PMIS_ACTIVITY_MASTER.Where(x => x.ACTIVITY_DESC == obj.ACTIVITY_DESC).Select(x => x.ACTIVITY_ID).FirstOrDefault();

                            var latest_record = dbContext.PMIS_PROGRESS_DETAILS.Where(x => x.IMS_PR_ROAD_CODE == RoadCode && x.ACTIVITY_ID == activity_ID && x.IS_LATEST == "Y").FirstOrDefault();

                            if (RoadCode != 0 && latest_record != null)
                            {
                                latest_record.PGS_QUANTITY = obj.ACTUAL_QUANTITY;
                                latest_record.ACTUAL_START_DATE = obj.STARTED_DATE;
                                latest_record.ACTUAL_END_DATE = obj.FINISHED_DATE;
                                latest_record.IS_LATEST = "Y";
                                // latest_record.ENTRY_DATE = obj.Date_of_progress_entry ?? DateTime.Now;
                                //latest_record.ENTRY_DATE = (DateTime.Now.Day >= AprilMonthStartDayValue
                                //    && DateTime.Now.Day <= AprilMonthEndDayValue && DateTime.Now.Month == AprilMonthValue)
                                //    ? Conditional_Date_Value : obj.Date_of_progress_entry ?? DateTime.Now;

                                latest_record.ENTRY_DATE = Convert.ToDateTime(Entry_Date);  // change by saurabh

                                latest_record.USERID = PMGSYSession.Current.UserId;
                                latest_record.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];

                                dbContext.Entry(latest_record).State = System.Data.Entity.EntityState.Modified;

                            }

                        }
                        dbContext.SaveChanges();
                    }
                    else
                    {
                        var latest_Master_new = dbContext.PMIS_PROGRESS_MASTER.Where(x => x.IMS_PR_ROAD_CODE == RoadCode & x.IS_LATEST == "Y").FirstOrDefault();
                        if (latest_Master_new != null)
                        {
                            latest_Master_new.IS_LATEST = "N";
                            dbContext.Entry(latest_Master_new).State = System.Data.Entity.EntityState.Modified;
                            dbContext.SaveChanges();

                        }
                        PMIS_PROGRESS_MASTER ProgressMaster = new PMIS_PROGRESS_MASTER();
                        foreach (var plan in planData)
                        {
                            RoadCode = plan.IMS_PR_ROAD_CODE;
                            if (RoadCode != 0)
                            {
                                ProgressMaster.PROGRESS_MASTER_ID = dbContext.PMIS_PROGRESS_MASTER.Any() ? dbContext.PMIS_PROGRESS_MASTER.Max(s => s.PROGRESS_MASTER_ID) + 1 : 1;
                                ProgressMaster.PLAN_ID = dbContext.PMIS_PLAN_MASTER.Where(x => x.IMS_PR_ROAD_CODE == RoadCode && x.IS_LATEST == "Y").Select(x => x.PLAN_ID).FirstOrDefault();
                                ProgressMaster.IMS_PR_ROAD_CODE = RoadCode;
                                ProgressMaster.COMPLETION_LENGTH = plan.CompletedRoadLength;
                                ProgressMaster.PROJECT_STATUS_ = Convert.ToString(plan.ProjectStatus);
                                ProgressMaster.USERID = PMGSYSession.Current.UserId;
                                ProgressMaster.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                                ProgressMaster.IS_LATEST = "Y";
                                ProgressMaster.BASELINE_NO = dbContext.PMIS_PLAN_MASTER.Where(x => x.IMS_PR_ROAD_CODE == RoadCode && x.IS_LATEST == "Y").Select(x => x.BASELINE_NO).FirstOrDefault() ?? default(int);
                                //ProgressMaster.ENTRY_DATE = plan.Date_of_progress_entry ?? DateTime.Now;
                                //ProgressMaster.ENTRY_DATE = (plan.Date_of_progress_entry.Value.Day >= AprilMonthStartDayValue
                                //&& plan.Date_of_progress_entry.Value.Day <= AprilMonthEndDayValue
                                //&& plan.Date_of_progress_entry.Value.Month == AprilMonthValue)
                                //? Conditional_Date_Value : plan.Date_of_progress_entry ?? default(DateTime);

                                ProgressMaster.ENTRY_DATE = Convert.ToDateTime(Entry_Date); // changes by saurabh

                                dbContext.PMIS_PROGRESS_MASTER.Add(ProgressMaster);
                                dbContext.SaveChanges();
                            }
                            break;
                        }
                        var Progress_id = dbContext.PMIS_PROGRESS_DETAILS.Any() ? dbContext.PMIS_PROGRESS_DETAILS.Max(s => s.PROGRESS_ID) + 1 : 1;
                        var latest_record_new = dbContext.PMIS_PROGRESS_DETAILS.Where(x => x.IMS_PR_ROAD_CODE == RoadCode && x.IS_LATEST == "Y").ToList();
                        if (latest_record_new != null)
                        {
                            foreach (var ls in latest_record_new)
                            {
                                ls.IS_LATEST = "N";
                                dbContext.Entry(ls).State = System.Data.Entity.EntityState.Modified;
                                dbContext.SaveChanges();
                            }
                        }
                        foreach (var obj in planData)
                        {
                            if (RoadCode != 0)
                            {
                                PMIS_PROGRESS_DETAILS ProgressDetails = new PMIS_PROGRESS_DETAILS();
                                ProgressDetails.PROGRESS_ID = Progress_id;
                                ProgressDetails.PROGRESS_MASTER_ID = ProgressMaster.PROGRESS_MASTER_ID;
                                ProgressDetails.PLAN_ID = ProgressMaster.PLAN_ID;
                                ProgressDetails.IMS_PR_ROAD_CODE = ProgressMaster.IMS_PR_ROAD_CODE;
                                ProgressDetails.ACTIVITY_ID = dbContext.PMIS_ACTIVITY_MASTER.Where(x => x.ACTIVITY_DESC == obj.ACTIVITY_DESC).Select(x => x.ACTIVITY_ID).FirstOrDefault();
                                ProgressDetails.PGS_QUANTITY = obj.ACTUAL_QUANTITY;
                                ProgressDetails.ACTUAL_START_DATE = obj.STARTED_DATE;
                                ProgressDetails.ACTUAL_END_DATE = obj.FINISHED_DATE;
                                ProgressDetails.IS_LATEST = "Y";
                                //ProgressDetails.ENTRY_DATE = obj.Date_of_progress_entry ?? DateTime.Now;
                                //ProgressDetails.ENTRY_DATE = (obj.Date_of_progress_entry.Value.Day >= AprilMonthStartDayValue
                                //&& obj.Date_of_progress_entry.Value.Day <= AprilMonthEndDayValue
                                //&& obj.Date_of_progress_entry.Value.Month == AprilMonthValue)
                                //? Conditional_Date_Value : obj.Date_of_progress_entry ?? default(DateTime);

                                ProgressDetails.ENTRY_DATE = Convert.ToDateTime(Entry_Date); // changes by saurabh

                                ProgressDetails.BASELINE_NO = dbContext.PMIS_PLAN_MASTER.Where(x => x.IMS_PR_ROAD_CODE == RoadCode && x.IS_LATEST == "Y").Select(x => x.BASELINE_NO).FirstOrDefault() ?? default(int);
                                ProgressDetails.USERID = PMGSYSession.Current.UserId;
                                ProgressDetails.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];

                                dbContext.PMIS_PROGRESS_DETAILS.Add(ProgressDetails);
                                Progress_id++;
                            }
                        }
                        dbContext.SaveChanges();
                    }
                    //Added on 6 July 2021
                    String ProjectStatus = String.Empty;
                    DateTime? ProgressDate = null;
                    Decimal ComplLength = 0, prepWorkLen = 0, earthWorkLen = 0, subBasePrep = 0, baseCourseLen = 0, surCourseLen = 0, cdWorksLen = 0, miscLen = 0;
                    List<int> ActivityId = dbContext.PMIS_ACTIVITY_MASTER.Select(x => x.ACTIVITY_ID).ToList();


                    foreach (var item in planData)
                    {
                        if (RoadCode > 0)
                        {
                            ProjectStatus = Convert.ToString(item.ProjectStatus);
                           // ProgressDate = item.Date_of_progress_entry;
                            ProgressDate = Convert.ToDateTime(Entry_Date); // changes by saurabh
                            ComplLength = item.CompletedRoadLength;
                            if (item.ACTIVITY_DESC.Equals("Preparatory Works"))
                            {
                                prepWorkLen = item.QUANTITY ?? default(decimal);
                            }
                            if (item.ACTIVITY_DESC.Equals("Earthwork"))
                            {
                                earthWorkLen = item.QUANTITY ?? default(decimal);
                            }
                            if (item.ACTIVITY_DESC.Equals("Granular Sub Base"))
                            {
                                subBasePrep = item.QUANTITY ?? default(decimal);
                            }
                            if (item.ACTIVITY_DESC.Equals("WBM Grading 2"))
                            {
                                baseCourseLen = item.QUANTITY ?? default(decimal);
                            }
                            if (item.ACTIVITY_DESC.Equals("Surface Course"))
                            {
                                surCourseLen = item.QUANTITY ?? default(decimal);
                            }
                            if (item.ACTIVITY_DESC.Equals("CD Works"))
                            {
                                cdWorksLen = item.QUANTITY ?? default(decimal);
                            }
                            if (item.ACTIVITY_DESC.Equals("Miscellaneous"))
                            {
                                miscLen = item.QUANTITY ?? default(decimal);
                            }
                        }
                    }
                    //var monthToCheck = (ProgressDate.Value.Day >= AprilMonthStartDayValue
                    //           && ProgressDate.Value.Day <= AprilMonthEndDayValue
                    //           && ProgressDate.Value.Month == AprilMonthValue) ? Conditional_Date_Value.Month : ProgressDate.Value.Month;

                    var monthToCheck = Convert.ToDateTime(Entry_Date).Month; // changes by saurabh
                    var YearCheck = Convert.ToDateTime(Entry_Date).Year; // changes by saurabh

                    if (dbContext.EXEC_ROADS_MONTHLY_STATUS.Where(x => x.IMS_PR_ROAD_CODE == RoadCode && x.EXEC_PROG_MONTH == monthToCheck && x.EXEC_PROG_YEAR == YearCheck).Any())    // change by saurabh
                    {
                        EXEC_ROADS_MONTHLY_STATUS monthlyStatus = new EXEC_ROADS_MONTHLY_STATUS();
                        monthlyStatus.IMS_PR_ROAD_CODE = RoadCode;
                        //monthlyStatus.EXEC_PROG_MONTH = (ProgressDate.Value.Month == AprilMonthValue)
                        //    ? Conditional_Date_Value.Month : ProgressDate.Value.Month;

                        //monthlyStatus.EXEC_PROG_MONTH = (ProgressDate.Value.Day >= AprilMonthStartDayValue
                        //       && ProgressDate.Value.Day <= AprilMonthEndDayValue
                        //       && ProgressDate.Value.Month == AprilMonthValue) ? Conditional_Date_Value.Month : ProgressDate.Value.Month;
                        //monthlyStatus.EXEC_PROG_YEAR = ProgressDate.Value.Year;

                        monthlyStatus.EXEC_PROG_MONTH = monthToCheck; // changes by saurabh
                        monthlyStatus.EXEC_PROG_YEAR = YearCheck;   // changes by saurabh

                        if (ProjectStatus == "C" || ProjectStatus == "P" || ProjectStatus == "A" || ProjectStatus == "F" || ProjectStatus == "L")
                        {
                            monthlyStatus.EXEC_ISCOMPLETED = ProjectStatus;
                        }
                        else
                        {
                            monthlyStatus.EXEC_ISCOMPLETED = "P";
                        }
                       // monthlyStatus.EXEC_COMPLETION_DATE = ProjectStatus == "C" ? ProgressDate : null;
                        monthlyStatus.EXEC_COMPLETION_DATE = ProjectStatus == "C" ? Entry_Date : null;  // changes by saurabh

                        monthlyStatus.USERID = PMGSYSession.Current.UserId;
                        monthlyStatus.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];

                        monthlyStatus.EXEC_PREPARATORY_WORK = prepWorkLen;
                        monthlyStatus.EXEC_EARTHWORK_SUBGRADE = earthWorkLen;
                        monthlyStatus.EXEC_SUBBASE_PREPRATION = subBasePrep;
                        monthlyStatus.EXEC_BASE_COURSE = baseCourseLen;
                        monthlyStatus.EXEC_SURFACE_COURSE = surCourseLen;
                        monthlyStatus.EXEC_SIGNS_STONES = 0;
                        monthlyStatus.EXEC_CD_WORKS = cdWorksLen;
                        monthlyStatus.EXEC_LSB_WORKS = 0;
                        monthlyStatus.EXEC_MISCELANEOUS = miscLen;
                        monthlyStatus.EXEC_COMPLETED = ComplLength;
                        dbContext.Entry(monthlyStatus).State = System.Data.Entity.EntityState.Modified;

                        // dbContext.Entry(monthlyStatus).State = System.Data.Entity.EntityState.Modified;
                        dbContext.SaveChanges();
                    }
                    else
                    {
                        EXEC_ROADS_MONTHLY_STATUS monthlyStatus = new EXEC_ROADS_MONTHLY_STATUS();
                        monthlyStatus.IMS_PR_ROAD_CODE = RoadCode;
                        //monthlyStatus.EXEC_PROG_MONTH = (ProgressDate.Value.Month == AprilMonthValue)
                        //    ? Conditional_Date_Value.Month : ProgressDate.Value.Month;
                        //monthlyStatus.EXEC_PROG_MONTH = (ProgressDate.Value.Day >= AprilMonthStartDayValue
                        //       && ProgressDate.Value.Day <= AprilMonthEndDayValue
                        //       && ProgressDate.Value.Month == AprilMonthValue) ? Conditional_Date_Value.Month : ProgressDate.Value.Month;
                        //monthlyStatus.EXEC_PROG_YEAR = ProgressDate.Value.Year;

                        monthlyStatus.EXEC_PROG_MONTH = monthToCheck; // changes by saurabh
                        monthlyStatus.EXEC_PROG_YEAR = YearCheck;   // changes by saurabh

                        if (ProjectStatus == "C" || ProjectStatus == "P" || ProjectStatus == "A" || ProjectStatus == "F" || ProjectStatus == "L")
                        {
                            monthlyStatus.EXEC_ISCOMPLETED = ProjectStatus;
                        }
                        else
                        {
                            monthlyStatus.EXEC_ISCOMPLETED = "P";
                        }
                        //monthlyStatus.EXEC_COMPLETION_DATE = ProjectStatus == "C" ? ProgressDate : null;
                        monthlyStatus.EXEC_COMPLETION_DATE = ProjectStatus == "C" ? Entry_Date : null;  //changes by saurabh

                        monthlyStatus.USERID = PMGSYSession.Current.UserId;
                        monthlyStatus.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];

                        monthlyStatus.EXEC_PREPARATORY_WORK = prepWorkLen;
                        monthlyStatus.EXEC_EARTHWORK_SUBGRADE = earthWorkLen;
                        monthlyStatus.EXEC_SUBBASE_PREPRATION = subBasePrep;
                        monthlyStatus.EXEC_BASE_COURSE = baseCourseLen;
                        monthlyStatus.EXEC_SURFACE_COURSE = surCourseLen;
                        monthlyStatus.EXEC_SIGNS_STONES = 0;
                        monthlyStatus.EXEC_CD_WORKS = cdWorksLen;
                        monthlyStatus.EXEC_LSB_WORKS = 0;
                        monthlyStatus.EXEC_MISCELANEOUS = miscLen;
                        monthlyStatus.EXEC_COMPLETED = ComplLength;

                        dbContext.EXEC_ROADS_MONTHLY_STATUS.Add(monthlyStatus);
                        dbContext.SaveChanges();
                    }
                    IMS_SANCTIONED_PROJECTS imsMaster = dbContext.IMS_SANCTIONED_PROJECTS.Find(RoadCode);
                    if (imsMaster != null)
                    {
                        imsMaster.IMS_ISCOMPLETED = Project_Status.ToString();
                        imsMaster.IMS_ENTRY_DATE_PHYSICAL = Entry_Date;
                        imsMaster.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                        imsMaster.USERID = PMGSYSession.Current.UserId;
                        dbContext.SaveChanges();
                    }
                    ts.Complete();
                    //  return null;
                }
                return string.Empty;
            }
            catch (DbEntityValidationException e)
            {
                foreach (var eve in e.EntityValidationErrors)
                {
                    using (System.IO.StreamWriter sw = System.IO.File.AppendText(System.IO.Path.Combine("D:\\ErrorLog" + System.DateTime.Now.Date.ToString("dd_MM_yyyy") + ".txt", "")))
                    {
                        sw.WriteLine("Date :" + DateTime.Now.ToString());
                        sw.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                        eve.Entry.Entity.GetType().Name, eve.Entry.State);
                        sw.WriteLine("---------------------------------------------------------------------------------------");
                        sw.Close();
                    }

                    foreach (var ve in eve.ValidationErrors)
                    {
                        using (System.IO.StreamWriter sw = System.IO.File.AppendText(System.IO.Path.Combine("D:\\ErrorLog" + System.DateTime.Now.Date.ToString("dd_MM_yyyy") + ".txt", "")))
                        {
                            sw.WriteLine("Date :" + DateTime.Now.ToString());
                            sw.WriteLine("- Property: \"{0}\", Error: \"{1}\"",
                            ve.PropertyName, ve.ErrorMessage);
                            sw.WriteLine("---------------------------------------------------------------------------------------");
                            sw.Close();
                        }
                    }
                }
                throw;
            }
            catch (DbUpdateException ex)
            {
                ErrorLog.LogError(ex, "SaveActualsDAL(DbUpdateException ex).DAL");
                return ("An Error Occurred While Processing Your Request.");
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "SaveActualsDAL().DAL");
                return ("Error Occurred While Processing Request.");
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        public string SubmitActualsDAL(IEnumerable<AddActualsViewModel> planData)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            try
            {
                using (TransactionScope ts = new TransactionScope())
                {
                    PMIS_PROGRESS_MASTER ProgressMaster = new PMIS_PROGRESS_MASTER();
                    int RoadCode = 0;  //planData.ElementAt(0).IMS_PR_ROAD_CODE;
                    char Project_Status = 'P';
                  
                    Nullable<DateTime> Entry_Date = new DateTime();
                    String ProjectStatusValid = String.Empty;
                    Decimal ComplLengthValid = 0;
                    // Added on 28-03-2022 by Srishti Tyagi
                    string AprilMonthStartDay = ConfigurationManager.AppSettings["APRIL_MONTH_START_DAY"];   //1
                    int AprilMonthStartDayValue = Convert.ToInt16(AprilMonthStartDay);

                    string AprilMonthEndDay = ConfigurationManager.AppSettings["APRIL_MONTH_END_DAY"];   //15
                    int AprilMonthEndDayValue = Convert.ToInt16(AprilMonthEndDay);

                    string AprilMonth = ConfigurationManager.AppSettings["APRIL_MONTH"];  //4
                    int AprilMonthValue = Convert.ToInt16(AprilMonth);
                    DateTime FinanDate = DateTime.Now;                                               // change by saurabh
                    //int FinancialYear = FinanDate.Month >= 4 ? FinanDate.Year : FinanDate.Year - 1;  // change by saurabh
                    //DateTime Conditional_Date_Value = new DateTime(FinancialYear + 1, 3, 31, 00, 00, 00);         // change by saurabh

                    int FinancialYear = FinanDate.Month >= 4 ? FinanDate.Year : FinanDate.Year;
                    DateTime Conditional_Date_Value = new DateTime(FinancialYear, 3, 31, 00, 00, 00);
                    var CondFinanYear = FinancialYear ; // CHANGE

                    //int conditionalMonth = (DateTime.Now.Month == AprilMonthValue && DateTime.Now.Day <= AprilMonthDayValue) ? (DateTime.Now.Month - 1) : DateTime.Now.Month;
                    // Changes end here 

                    foreach (var plan in planData)
                    {
                        
                        RoadCode = plan.IMS_PR_ROAD_CODE;
                        Project_Status = (plan.ProjectStatus == 'C' ? 'C' : 'P');
                        ProjectStatusValid = Convert.ToString(plan.ProjectStatus);
                        ComplLengthValid = plan.CompletedRoadLength;
                        // Added on 29-03-2022 by Srishti Tyagi
                        //Entry_Date = plan.Date_of_progress_entry;
                        //plan.Date_of_progress_entry = new DateTime(2023, 4, 14, 00, 00, 00);   // Commentable change by saurabh
                        if (plan.Date_of_progress_entry != null)
                        {
                            //Entry_Date = (plan.Date_of_progress_entry.Value.Day >= AprilMonthStartDayValue
                            //    && plan.Date_of_progress_entry.Value.Day <= AprilMonthEndDayValue
                            //    && plan.Date_of_progress_entry.Value.Month == AprilMonthValue) ? Conditional_Date_Value : plan.Date_of_progress_entry;


                            Entry_Date = (plan.Date_of_progress_entry.Value.Day >= AprilMonthStartDayValue
                               && plan.Date_of_progress_entry.Value.Day <= AprilMonthEndDayValue
                               && plan.Date_of_progress_entry.Value.Month == AprilMonthValue) ? Conditional_Date_Value : DateTime.Now;

                            //Nullable<DateTime> CurrentDate = new DateTime(2023, 4, 14, 00, 00, 00);
                            //Nullable<DateTime> LimitDate = new DateTime(2023, 4, 13, 00, 00, 00);
                            //if (CurrentDate > LimitDate)
                            //{
                            //    if (Entry_Date < CurrentDate)
                            //    {
                            //        return ("Date of progress entry is mandatory to fill.");
                            //    }
                            //}
                        }
                        else 
                        {
                            return ("Date of progress entry is mandatory to fill.");
                        }
                                              
                        if (ProjectStatusValid == "0")
                        {
                            return ("Project Status is mandatory to fill.");
                        }
                        if (ProjectStatusValid != "P" && ProjectStatusValid != "W" && ProjectStatusValid != "H" && ProjectStatusValid != "L" && ProjectStatusValid != "F" && ProjectStatusValid != "A")
                        {
                            if (ComplLengthValid == 0)
                            {
                                return ("Completion Length is mandatory to fill.");
                            }
                        }                                             
                        if (Entry_Date == null)
                        {
                            return ("Date of progress entry is mandatory to fill.");
                        }

                        if (FinanDate.Month != Entry_Date.Value.Month || FinanDate.Day != Entry_Date.Value.Day || FinanDate.Year != Entry_Date.Value.Year)
                        {
                            return ("Progress can be entered in Current Date of Current Financial Year");
                        }

                        //if (FinanDate.Month != AprilMonthValue && FinanDate.Day > AprilMonthEndDayValue)
                        //{
                        //    if (Entry_Date.Value.Year * 12 + Entry_Date.Value.Month <= CondFinanYear * 12 + 3)
                        //    {
                        //        return ("Progress can be entered in Current Date of Current Financial Year");
                        //    }
                        //}
                        //if (plan.Date_of_progress_entry.Value.Year * 12 + plan.Date_of_progress_entry.Value.Month <= FinancialYear * 12 + 3)   // change
                        //{
                        //    return ("Progress can be entered in Current Financial Year");
                        //}

                        if (RoadCode > 0)
                            break;
                    }
                    var latest_Master = dbContext.PMIS_PROGRESS_MASTER.Where(x => x.IMS_PR_ROAD_CODE == RoadCode & x.IS_LATEST == "Y").FirstOrDefault();
                    if (latest_Master != null)
                    {
                        latest_Master.IS_LATEST = "N";
                        dbContext.Entry(latest_Master).State = System.Data.Entity.EntityState.Modified;
                        dbContext.SaveChanges();

                    }
                    foreach (var plan in planData)
                    {
                        RoadCode = plan.IMS_PR_ROAD_CODE;
                        if (RoadCode != 0)
                        {
                            ProgressMaster.PROGRESS_MASTER_ID = dbContext.PMIS_PROGRESS_MASTER.Any() ? dbContext.PMIS_PROGRESS_MASTER.Max(s => s.PROGRESS_MASTER_ID) + 1 : 1;
                            ProgressMaster.PLAN_ID = dbContext.PMIS_PLAN_MASTER.Where(x => x.IMS_PR_ROAD_CODE == RoadCode && x.IS_LATEST == "Y").Select(x => x.PLAN_ID).FirstOrDefault();
                            ProgressMaster.IMS_PR_ROAD_CODE = RoadCode;
                            ProgressMaster.COMPLETION_LENGTH = plan.CompletedRoadLength;
                            ProgressMaster.PROJECT_STATUS_ = Convert.ToString(plan.ProjectStatus);
                            ProgressMaster.USERID = PMGSYSession.Current.UserId;
                            ProgressMaster.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                            ProgressMaster.IS_LATEST = "Y";
                            ProgressMaster.BASELINE_NO = dbContext.PMIS_PLAN_MASTER.Where(x => x.IMS_PR_ROAD_CODE == RoadCode && x.IS_LATEST == "Y").Select(x => x.BASELINE_NO).FirstOrDefault() ?? default(int);

                            // Added on 29-03-2022 by Srishti Tyagi
                            //ProgressMaster.ENTRY_DATE = plan.Date_of_progress_entry ?? default(DateTime);

                            //ProgressMaster.ENTRY_DATE = (plan.Date_of_progress_entry.Value.Day >= AprilMonthStartDayValue
                            //    && plan.Date_of_progress_entry.Value.Day <= AprilMonthEndDayValue
                            //    && plan.Date_of_progress_entry.Value.Month == AprilMonthValue)
                            //    ? Conditional_Date_Value : plan.Date_of_progress_entry ?? default(DateTime);

                            ProgressMaster.ENTRY_DATE = Convert.ToDateTime(Entry_Date);       // change by saurabh
                            ProgressMaster.REMARKS = String.IsNullOrEmpty(plan.Remarks) ? null : plan.Remarks;
                            dbContext.PMIS_PROGRESS_MASTER.Add(ProgressMaster);
                            dbContext.SaveChanges();
                        }
                        break;
                    }
                    var Progress_id = dbContext.PMIS_PROGRESS_DETAILS.Any() ? dbContext.PMIS_PROGRESS_DETAILS.Max(s => s.PROGRESS_ID) + 1 : 1;
                    var latest_record = dbContext.PMIS_PROGRESS_DETAILS.Where(x => x.IMS_PR_ROAD_CODE == RoadCode && x.IS_LATEST == "Y").ToList();
                    if (latest_record != null)
                    {
                        foreach (var ls in latest_record)
                        {
                            ls.IS_LATEST = "N";
                            dbContext.Entry(ls).State = System.Data.Entity.EntityState.Modified;
                            dbContext.SaveChanges();
                        }
                    }
                    foreach (var obj in planData)
                    {
                        if (RoadCode != 0)
                        {
                            PMIS_PROGRESS_DETAILS ProgressDetails = new PMIS_PROGRESS_DETAILS();
                            ProgressDetails.PROGRESS_ID = Progress_id;
                            ProgressDetails.PROGRESS_MASTER_ID = ProgressMaster.PROGRESS_MASTER_ID;
                            ProgressDetails.PLAN_ID = ProgressMaster.PLAN_ID;
                            ProgressDetails.IMS_PR_ROAD_CODE = ProgressMaster.IMS_PR_ROAD_CODE;
                            ProgressDetails.ACTIVITY_ID = dbContext.PMIS_ACTIVITY_MASTER.Where(x => x.ACTIVITY_DESC == obj.ACTIVITY_DESC).Select(x => x.ACTIVITY_ID).FirstOrDefault();
                            ProgressDetails.PGS_QUANTITY = obj.ACTUAL_QUANTITY;
                            ProgressDetails.ACTUAL_START_DATE = obj.STARTED_DATE;
                            ProgressDetails.ACTUAL_END_DATE = obj.FINISHED_DATE;
                            ProgressDetails.IS_LATEST = "Y";
                            // Added on 29-03-2022 by Srishti Tyagi
                            //ProgressDetails.ENTRY_DATE = obj.Date_of_progress_entry ?? default(DateTime);

                            //ProgressDetails.ENTRY_DATE = (obj.Date_of_progress_entry.Value.Day >= AprilMonthStartDayValue
                            //    && obj.Date_of_progress_entry.Value.Day <= AprilMonthEndDayValue
                            //    && obj.Date_of_progress_entry.Value.Month == AprilMonthValue)
                            //    ? Conditional_Date_Value : obj.Date_of_progress_entry ?? default(DateTime);

                            ProgressDetails.ENTRY_DATE = Convert.ToDateTime(Entry_Date);   // change by saurabh

                            ProgressDetails.BASELINE_NO = dbContext.PMIS_PLAN_MASTER.Where(x => x.IMS_PR_ROAD_CODE == RoadCode && x.IS_LATEST == "Y").Select(x => x.BASELINE_NO).FirstOrDefault() ?? default(int);
                            ProgressDetails.USERID = PMGSYSession.Current.UserId;
                            ProgressDetails.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];

                            dbContext.PMIS_PROGRESS_DETAILS.Add(ProgressDetails);
                            Progress_id++;
                        }
                    }
                    dbContext.SaveChanges();
                    //Added on 6 July 2021
                    String ProjectStatus = String.Empty;
                    DateTime? ProgressDate = null;
                    Decimal ComplLength = 0, prepWorkLen = 0, earthWorkLen = 0, subBasePrep = 0, baseCourseLen = 0, surCourseLen = 0, cdWorksLen = 0, miscLen = 0;
                    List<int> ActivityId = dbContext.PMIS_ACTIVITY_MASTER.Select(x => x.ACTIVITY_ID).ToList();


                    foreach (var item in planData)
                    {
                        if (RoadCode > 0)
                        {
                            ProjectStatus = Convert.ToString(item.ProjectStatus);
                            //ProgressDate = item.Date_of_progress_entry; 
                            ProgressDate = Convert.ToDateTime(Entry_Date); // changes by saurabh
                            ComplLength = item.CompletedRoadLength;
                            if (item.ACTIVITY_DESC.Equals("Preparatory Works"))
                            {
                                prepWorkLen = item.QUANTITY ?? default(decimal);
                            }
                            if (item.ACTIVITY_DESC.Equals("Earthwork"))
                            {
                                earthWorkLen = item.QUANTITY ?? default(decimal);
                            }
                            if (item.ACTIVITY_DESC.Equals("Granular Sub Base"))
                            {
                                subBasePrep = item.QUANTITY ?? default(decimal);
                            }
                            if (item.ACTIVITY_DESC.Equals("WBM Grading 2"))
                            {
                                baseCourseLen = item.QUANTITY ?? default(decimal);
                            }
                            if (item.ACTIVITY_DESC.Equals("Surface Course"))
                            {
                                surCourseLen = item.QUANTITY ?? default(decimal);
                            }
                            if (item.ACTIVITY_DESC.Equals("CD Works"))
                            {
                                cdWorksLen = item.QUANTITY ?? default(decimal);
                            }
                            if (item.ACTIVITY_DESC.Equals("Miscellaneous"))
                            {
                                miscLen = item.QUANTITY ?? default(decimal);
                            }
                        }
                    }
                    //var monthToCheck = (ProgressDate.Value.Day >= AprilMonthStartDayValue
                    //           && ProgressDate.Value.Day <= AprilMonthEndDayValue
                    //           && ProgressDate.Value.Month == AprilMonthValue) ? Conditional_Date_Value.Month : ProgressDate.Value.Month;

                    var monthToCheck = Convert.ToDateTime(Entry_Date).Month;    // change by saurabh
                    var yearToCheck = Convert.ToDateTime(Entry_Date).Year;       // change by saurabh

                    if (!(dbContext.EXEC_ROADS_MONTHLY_STATUS.Where(x => x.IMS_PR_ROAD_CODE == RoadCode && x.EXEC_PROG_MONTH == monthToCheck && x.EXEC_PROG_YEAR == yearToCheck).Any()))     // change
                    {
                        EXEC_ROADS_MONTHLY_STATUS monthlyStatus = new EXEC_ROADS_MONTHLY_STATUS();
                        monthlyStatus.IMS_PR_ROAD_CODE = RoadCode;
                        // Added on 29-03-2022 by Srishti Tyagi

                        

                        //monthlyStatus.EXEC_PROG_MONTH = (ProgressDate.Value.Day >= AprilMonthStartDayValue
                        //        && ProgressDate.Value.Day <= AprilMonthEndDayValue
                        //        && ProgressDate.Value.Month == AprilMonthValue) ? Conditional_Date_Value.Month : ProgressDate.Value.Month;
                        //monthlyStatus.EXEC_PROG_YEAR = ProgressDate.Value.Year;

                        monthlyStatus.EXEC_PROG_MONTH = monthToCheck;  // change by saurabh
                        monthlyStatus.EXEC_PROG_YEAR = yearToCheck;    // change by saurabh

                        if (ProjectStatus == "C" || ProjectStatus == "P" || ProjectStatus == "A" || ProjectStatus == "F" || ProjectStatus == "L")
                        {
                            monthlyStatus.EXEC_ISCOMPLETED = ProjectStatus;
                        }
                        else
                        {
                            monthlyStatus.EXEC_ISCOMPLETED = "P";
                        }
                        // monthlyStatus.EXEC_COMPLETION_DATE = ProjectStatus == "C" ? ProgressDate : null;  
                        monthlyStatus.EXEC_COMPLETION_DATE = ProjectStatus == "C" ? Entry_Date : null;   // change by saurabh

                        monthlyStatus.USERID = PMGSYSession.Current.UserId;
                        monthlyStatus.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];

                        monthlyStatus.EXEC_PREPARATORY_WORK = prepWorkLen;
                        monthlyStatus.EXEC_EARTHWORK_SUBGRADE = earthWorkLen;
                        monthlyStatus.EXEC_SUBBASE_PREPRATION = subBasePrep;
                        monthlyStatus.EXEC_BASE_COURSE = baseCourseLen;
                        monthlyStatus.EXEC_SURFACE_COURSE = surCourseLen;
                        monthlyStatus.EXEC_SIGNS_STONES = 0;
                        monthlyStatus.EXEC_CD_WORKS = cdWorksLen;
                        monthlyStatus.EXEC_LSB_WORKS = 0;
                        monthlyStatus.EXEC_MISCELANEOUS = miscLen;
                        monthlyStatus.EXEC_COMPLETED = ComplLength;

                        dbContext.EXEC_ROADS_MONTHLY_STATUS.Add(monthlyStatus);
                        dbContext.SaveChanges();
                    }
                    else
                    {
                        EXEC_ROADS_MONTHLY_STATUS monthlyStatus = new EXEC_ROADS_MONTHLY_STATUS();
                        monthlyStatus.IMS_PR_ROAD_CODE = RoadCode;
                        // Added on 29-03-2022 by Srishti Tyagi
                        //monthlyStatus.EXEC_PROG_MONTH = ProgressDate.Value.Month;

                        //monthlyStatus.EXEC_PROG_MONTH = (ProgressDate.Value.Day >= AprilMonthStartDayValue
                        //        && ProgressDate.Value.Day <= AprilMonthEndDayValue
                        //        && ProgressDate.Value.Month == AprilMonthValue) ? Conditional_Date_Value.Month : ProgressDate.Value.Month;
                        //monthlyStatus.EXEC_PROG_YEAR = ProgressDate.Value.Year;

                        monthlyStatus.EXEC_PROG_MONTH = monthToCheck;    // change by saurabh
                        monthlyStatus.EXEC_PROG_YEAR = yearToCheck;      // change by saurabh

                        if (ProjectStatus == "C" || ProjectStatus == "P" || ProjectStatus == "A" || ProjectStatus == "F" || ProjectStatus == "L")
                        {
                            monthlyStatus.EXEC_ISCOMPLETED = ProjectStatus;
                        }
                        else
                        {
                            monthlyStatus.EXEC_ISCOMPLETED = "P";
                        }
                        // monthlyStatus.EXEC_COMPLETION_DATE = ProjectStatus == "C" ? ProgressDate : null;
                        monthlyStatus.EXEC_COMPLETION_DATE = ProjectStatus == "C" ? Entry_Date : null;       // change by saurabh

                        monthlyStatus.USERID = PMGSYSession.Current.UserId;
                        monthlyStatus.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];

                        monthlyStatus.EXEC_PREPARATORY_WORK = prepWorkLen;
                        monthlyStatus.EXEC_EARTHWORK_SUBGRADE = earthWorkLen;
                        monthlyStatus.EXEC_SUBBASE_PREPRATION = subBasePrep;
                        monthlyStatus.EXEC_BASE_COURSE = baseCourseLen;
                        monthlyStatus.EXEC_SURFACE_COURSE = surCourseLen;
                        monthlyStatus.EXEC_SIGNS_STONES = 0;
                        monthlyStatus.EXEC_CD_WORKS = cdWorksLen;
                        monthlyStatus.EXEC_LSB_WORKS = 0;
                        monthlyStatus.EXEC_MISCELANEOUS = miscLen;
                        monthlyStatus.EXEC_COMPLETED = ComplLength;

                        dbContext.Entry(monthlyStatus).State = System.Data.Entity.EntityState.Modified;
                        dbContext.SaveChanges();
                    }
                    IMS_SANCTIONED_PROJECTS imsMaster = dbContext.IMS_SANCTIONED_PROJECTS.Find(RoadCode);
                    if (imsMaster != null)
                    {
                        imsMaster.IMS_ISCOMPLETED = Project_Status.ToString();
                        imsMaster.IMS_ENTRY_DATE_PHYSICAL = Entry_Date;
                        imsMaster.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                        imsMaster.USERID = PMGSYSession.Current.UserId;
                        dbContext.SaveChanges();
                    }
                    ts.Complete();
                    //  return null;
                }
                return string.Empty;
            }
            catch (DbEntityValidationException e)
            {
                foreach (var eve in e.EntityValidationErrors)
                {
                    using (System.IO.StreamWriter sw = System.IO.File.AppendText(System.IO.Path.Combine("D:\\ErrorLog" + System.DateTime.Now.Date.ToString("dd_MM_yyyy") + ".txt", "")))
                    {
                        sw.WriteLine("Date :" + DateTime.Now.ToString());
                        sw.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                        eve.Entry.Entity.GetType().Name, eve.Entry.State);
                        sw.WriteLine("---------------------------------------------------------------------------------------");
                        sw.Close();
                    }

                    foreach (var ve in eve.ValidationErrors)
                    {
                        using (System.IO.StreamWriter sw = System.IO.File.AppendText(System.IO.Path.Combine("D:\\ErrorLog" + System.DateTime.Now.Date.ToString("dd_MM_yyyy") + ".txt", "")))
                        {
                            sw.WriteLine("Date :" + DateTime.Now.ToString());
                            sw.WriteLine("- Property: \"{0}\", Error: \"{1}\"",
                            ve.PropertyName, ve.ErrorMessage);
                            sw.WriteLine("---------------------------------------------------------------------------------------");
                            sw.Close();
                        }
                    }
                }
                throw;
            }
            catch (DbUpdateException ex)
            {
                ErrorLog.LogError(ex, "SubmitActualsDAL(DbUpdateException ex).DAL");
                return ("An Error Occurred While Processing Your Request.");
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "SubmitActualsDAL().DAL");
                return ("Error Occurred While Processing Request.");
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        public string SubmitChainageDetailsDAL(FormCollection formData)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            try
            {
                using (TransactionScope ts = new TransactionScope())
                {
                    DateTime chainageEntryDate;
                    int RoadCode = Convert.ToInt32(formData["IMS_PR_ROAD_CODE"]);
                    if (dbContext.PMIS_CHAINAGEWISE_COMPLETION_DETAILS.Where(x => x.IMS_PR_ROAD_CODE == RoadCode).Any())
                    {
                        var chainagedetails = dbContext.PMIS_CHAINAGEWISE_COMPLETION_DETAILS.Where(x => x.IMS_PR_ROAD_CODE == RoadCode).ToList();
                        if (chainagedetails != null)
                        {
                            for (int i = 0; i < Convert.ToInt32(formData["SanctionedLength"]); i++)
                            {
                                var chainagerecord = dbContext.PMIS_CHAINAGEWISE_COMPLETION_DETAILS.Where(x => x.IMS_PR_ROAD_CODE == RoadCode && x.CHAINAGE_FROM == i).FirstOrDefault();

                                chainagerecord.ACT_EARTHWORK = formData["earthworklist[" + i + "]"];
                                chainagerecord.ACT_SUBGRADE = formData["subgradelist[" + i + "]"];
                                chainagerecord.ACT_GRSUBBASE = formData["granularsubbaselist[" + i + "]"];
                                chainagerecord.ACT_WBM_GRADING2 = formData["wbmgrading2list[" + i + "]"];
                                chainagerecord.ACT_WBM_GRADING3 = formData["wbmgrading3list[" + i + "]"];
                                chainagerecord.ACT_WETMIX_MACADAM = formData["wetmixmacadamlist[" + i + "]"];
                                chainagerecord.ACT_BIT_MACADAM = formData["bituminousmacadamlist[" + i + "]"];
                                chainagerecord.ACT_SURFACE_COURSE = formData["surfacecourselist[" + i + "]"];
                                chainagerecord.IS_VALID = "Y";
                                chainagerecord.USERID = PMGSYSession.Current.UserId;
                                chainagerecord.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                                DateTime.TryParse(formData["Date_of_Chainage_entry"], out chainageEntryDate);
                                chainagerecord.COMPL_MONTH = chainageEntryDate.Month;
                                chainagerecord.COMPL_YEAR = chainageEntryDate.Year;
                                chainagerecord.ENTRY_DATE = chainageEntryDate;
                                dbContext.Entry(chainagerecord).State = System.Data.Entity.EntityState.Modified;
                            }
                            dbContext.SaveChanges();
                        }
                    }
                    else
                    {
                        var compl_id = dbContext.PMIS_CHAINAGEWISE_COMPLETION_DETAILS.Any() ? dbContext.PMIS_CHAINAGEWISE_COMPLETION_DETAILS.Max(s => s.COMPL_ID) + 1 : 1;
                        for (int i = 0; i < Convert.ToInt32(formData["SanctionedLength"]); i++)
                        {

                            PMIS_CHAINAGEWISE_COMPLETION_DETAILS ChainageDetails = new PMIS_CHAINAGEWISE_COMPLETION_DETAILS();
                            ChainageDetails.COMPL_ID = compl_id;
                            ChainageDetails.CHAINAGE_FROM = i;
                            ChainageDetails.CHAINAGE_TO = i + 1;
                            ChainageDetails.ACT_EARTHWORK = formData["earthworklist[" + i + "]"];
                            ChainageDetails.ACT_SUBGRADE = formData["subgradelist[" + i + "]"];
                            ChainageDetails.ACT_GRSUBBASE = formData["granularsubbaselist[" + i + "]"];
                            ChainageDetails.ACT_WBM_GRADING2 = formData["wbmgrading2list[" + i + "]"];
                            ChainageDetails.ACT_WBM_GRADING3 = formData["wbmgrading3list[" + i + "]"];
                            ChainageDetails.ACT_WETMIX_MACADAM = formData["wetmixmacadamlist[" + i + "]"];
                            ChainageDetails.ACT_BIT_MACADAM = formData["bituminousmacadamlist[" + i + "]"];
                            ChainageDetails.ACT_SURFACE_COURSE = formData["surfacecourselist[" + i + "]"];
                            ChainageDetails.PLAN_ID = dbContext.PMIS_PLAN_MASTER.Where(x => x.IMS_PR_ROAD_CODE == RoadCode).Select(x => x.PLAN_ID).FirstOrDefault();
                            ChainageDetails.BASELINE_NO = dbContext.PMIS_PLAN_MASTER.Where(x => x.IMS_PR_ROAD_CODE == RoadCode).Select(x => x.BASELINE_NO).FirstOrDefault() ?? default(int);
                            ChainageDetails.IS_VALID = "Y";
                            ChainageDetails.USERID = PMGSYSession.Current.UserId;
                            ChainageDetails.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                            ChainageDetails.IMS_PR_ROAD_CODE = RoadCode;
                            DateTime.TryParse(formData["Date_of_Chainage_entry"], out chainageEntryDate);
                            ChainageDetails.COMPL_MONTH = chainageEntryDate.Month;
                            ChainageDetails.COMPL_YEAR = chainageEntryDate.Year;
                            ChainageDetails.ENTRY_DATE = chainageEntryDate;
                            dbContext.PMIS_CHAINAGEWISE_COMPLETION_DETAILS.Add(ChainageDetails);
                            compl_id++;
                        }
                        dbContext.SaveChanges();
                    }
                    ts.Complete();
                    //  return null;
                }
                return string.Empty;
            }
            catch (DbEntityValidationException e)
            {
                foreach (var eve in e.EntityValidationErrors)
                {
                    using (System.IO.StreamWriter sw = System.IO.File.AppendText(System.IO.Path.Combine("D:\\ErrorLog" + System.DateTime.Now.Date.ToString("dd_MM_yyyy") + ".txt", "")))
                    {
                        sw.WriteLine("Date :" + DateTime.Now.ToString());
                        sw.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                        eve.Entry.Entity.GetType().Name, eve.Entry.State);
                        sw.WriteLine("---------------------------------------------------------------------------------------");
                        sw.Close();
                    }

                    foreach (var ve in eve.ValidationErrors)
                    {
                        using (System.IO.StreamWriter sw = System.IO.File.AppendText(System.IO.Path.Combine("D:\\ErrorLog" + System.DateTime.Now.Date.ToString("dd_MM_yyyy") + ".txt", "")))
                        {
                            sw.WriteLine("Date :" + DateTime.Now.ToString());
                            sw.WriteLine("- Property: \"{0}\", Error: \"{1}\"",
                            ve.PropertyName, ve.ErrorMessage);
                            sw.WriteLine("---------------------------------------------------------------------------------------");
                            sw.Close();
                        }
                    }
                }
                throw;
            }
            catch (DbUpdateException ex)
            {
                ErrorLog.LogError(ex, "SubmitChainageDetailsDAL(DbUpdateException ex).DAL");
                return ("An Error Occurred While Processing Your Request.");
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "SubmitChainageDetailsDAL().DAL");
                return ("Error Occurred While Processing Request.");
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        public void PMISLog(string module, string logPath, string message, string fileName)
        {
            try
            {
                if (!Directory.Exists(logPath))
                {
                    Directory.CreateDirectory(logPath);
                }
                using (System.IO.StreamWriter sw = System.IO.File.AppendText(System.IO.Path.Combine(logPath + "\\PMIS" + module + "Log_" + System.DateTime.Now.Date.ToString("dd_MM_yyyy") + ".txt", "")))
                {
                    sw.WriteLine("---------------------------------------------------------------------------------------");
                    sw.WriteLine("Date : " + DateTime.Now.ToString());
                    sw.WriteLine("FileName : " + fileName);
                    sw.WriteLine("status : " + message);
                    sw.WriteLine("---------------------------------------------------------------------------------------");
                    sw.Close();
                }
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "PMIS.PMISLog");
            }
        }

        public Array PMISRoadListDALCharts(int state, int district, int block, int sanction_year, int batch, int? page, int? rows, string sidx, string sord, out long totalRecords)
        {

            try
            {
                dbContext = new PMGSYEntities();

                var PMISRoadList1 = dbContext.USP_PMS_ROAD_LIST(state, district, block, sanction_year, batch).ToList<USP_PMS_ROAD_LIST_Result>();

                var roadCodeList = dbContext.PMIS_PLAN_MASTER.Where(m => m.IS_FINALISED == "Y").Select(m => m.IMS_PR_ROAD_CODE).ToList();

                var PMISRoadList = PMISRoadList1.Where(m => roadCodeList.Contains(m.PrRoadCode)).ToList();

                // 

                var resultList = new List<PMISRoadDALDetails>();

                foreach (var item in PMISRoadList)
                {
                    var RoadCode = dbContext.PMIS_PLAN_MASTER.Where(x => x.IMS_PR_ROAD_CODE == item.PrRoadCode).Select(c => c.IMS_PR_ROAD_CODE).FirstOrDefault();
                    var RevisePlanRoadCode = dbContext.PMIS_PLAN_MASTER.Where(x => x.IMS_PR_ROAD_CODE == item.PrRoadCode & x.IS_LATEST == "Y" & x.IS_FINALISED == "Y").Select(c => c.IMS_PR_ROAD_CODE).FirstOrDefault();
                    var PLAN_ID = dbContext.PMIS_PLAN_MASTER.Where(x => x.IMS_PR_ROAD_CODE == item.PrRoadCode).Select(m => m.PLAN_ID).FirstOrDefault();
                    //  var PLAN_ID = dbContext.PMIS_PLAN_MASTER.Where(x => x.IMS_PR_ROAD_CODE == item.PrRoadCode).Max(m => m.PLAN_ID);
                    // var FinalizeRoadCode = dbContext.PMIS_PLAN_MASTER.Where(z => z.IMS_PR_ROAD_CODE == item.PrRoadCode & z.IS_LATEST == "Y" & z.IS_FINALISED == "N").Select(y => y.IMS_PR_ROAD_CODE).FirstOrDefault();
                    resultList.Add(new PMISRoadDALDetails
                    {
                        StateName = item.MAST_STATE_NAME,
                        DistrictName = item.MAST_DISTRICT_NAME,
                        BlockName = item.BlockName,
                        PackageName = item.PackageId,
                        SanctionYear = (item.SanctionYear).ToString() + "-" + (item.SanctionYear + 1).ToString().Substring(2, 2),
                        SanctionDate = ((item.IMS_SANCTIONED_DATE).Value).ToShortDateString(),
                        BatchName = item.MAST_BATCH_NAME,
                        SanctionLength = (item.SanctionLength).ToString(),
                        AgreementNo = item.TEND_AGREEMENT_NUMBER,
                        AgreementCost = item.TEND_AGREEMENT_AMOUNT.ToString(),
                        MordShare = (item.Mord_Share ?? default(decimal)).ToString(),
                        StateShare = item.State_share.ToString(),
                        TotalSanctionedCost = item.TOTAL_COST.ToString(),
                        AgreementStartDate = (item.TEND_AGREEMENT_START_DATE == null) ? "NULL" : ((item.TEND_AGREEMENT_START_DATE).Value).ToShortDateString(),
                        AgreementEndDate = (item.TEND_AGREEMENT_END_DATE == null) ? "NULL" : ((item.TEND_AGREEMENT_END_DATE).Value).ToShortDateString(),
                        RoadName = item.RoadName,
                        IMS_PR_RoadCode = item.PrRoadCode.ToString(),
                        IsPlanAvaliable = RoadCode == 0 ? item.PrRoadCode.ToString() + "$" : RoadCode.ToString(),
                        IsFinalize = RoadCode == 0 ? "-" : RevisePlanRoadCode != 0 ? RevisePlanRoadCode.ToString() + "$" : item.PrRoadCode.ToString(),
                        IsRevisePlan = RevisePlanRoadCode == 0 ? " " : RevisePlanRoadCode.ToString(),
                        IsActualsAvaliable = RoadCode == 0 ? "" : RoadCode.ToString(),
                        PlanId = PLAN_ID.ToString()
                    });
                }

                totalRecords = PMISRoadList.Count();

                return resultList.Select(RoadDetails => new
                {
                    cell = new[] {    
                        RoadDetails.StateName,
                        RoadDetails.DistrictName,   
                        RoadDetails.BlockName,      
                        RoadDetails.PackageName,    
                        RoadDetails.SanctionYear,
                        RoadDetails.SanctionDate,
                        RoadDetails.BatchName,      
                        RoadDetails.SanctionLength, 
                        RoadDetails.AgreementNo,    
                        RoadDetails.AgreementCost, 
                        RoadDetails.MordShare,
                        RoadDetails.StateShare,
                        RoadDetails.TotalSanctionedCost,
                        RoadDetails.RoadName,
                        "<a href='#' class='ui-icon ui-icon-zoomin ui-align-center' onclick='ViewChart(\"" + URLEncrypt.EncryptParameters1(new string[] { "IMSPRRoadCode =" + RoadDetails.IMS_PR_RoadCode.ToString(),"PlanID =" + RoadDetails.PlanId})+ "\"); return false;'></a>" ,


                        RoadDetails.IsPlanAvaliable.EndsWith("$") ? "<center><table><tr><td  style='border:none'><span class='ui-icon ui-icon-circle-plus ' title='Add Project Plan' onClick =AddProjectPlan('"+ URLEncrypt.EncryptParameters1(new string[]{"RoadCode =" + RoadDetails.IMS_PR_RoadCode.ToString().Trim(),"StateShare =" + RoadDetails.StateShare.ToString().Trim(),"MordShare =" + RoadDetails.MordShare.ToString().Trim(),"TotalSanctionedDate =" + RoadDetails.TotalSanctionedCost.ToString().Trim()})+"');></span></td></tr></table></center>"//RoadDetails.IsPlanAvaliable 
                        :  RoadDetails.IsFinalize.Contains("$") ? "<center><table><tr><td style='border:none;cursor:pointer'><span class='ui-icon ui-icon-search' title='View Project Plan' onClick =ViewProjectPlan('"+ URLEncrypt.EncryptParameters1(new string[]{"RoadCode =" + RoadDetails.IsPlanAvaliable.ToString().Trim(),"StateShare =" + RoadDetails.StateShare.ToString().Trim(),"MordShare =" + RoadDetails.MordShare.ToString().Trim(),"TotalSanctionedDate =" + RoadDetails.TotalSanctionedCost.ToString().Trim()})+"');></span></td></tr></table></center>"
                        :"<center><table><tr><td  style='border:none;cursor:pointer'><span class='ui-icon ui-icon-pencil' title='Edit Project Plan' onClick =EditProjectPlan('"+ URLEncrypt.EncryptParameters1(new string[]{"RoadCode =" + RoadDetails.IsPlanAvaliable.ToString().Trim(),"StateShare =" + RoadDetails.StateShare.ToString().Trim(),"MordShare =" + RoadDetails.MordShare.ToString().Trim(),"TotalSanctionedDate =" + RoadDetails.TotalSanctionedCost.ToString().Trim()})+"');></span></td><td style='border:none;cursor:pointer'><span class='ui-icon ui-icon-search' title='View Project Plan' onClick ='ViewProjectPlan(\"" + URLEncrypt.EncryptParameters1(new string[]{"RoadCode =" + RoadDetails.IsPlanAvaliable.ToString().Trim()}) + "\");'></span></td><td style='border:none;cursor:pointer'><span class='ui-icon ui-icon-trash' title='Delete Project Plan' onClick ='DeleteProjectPlan(\"" + URLEncrypt.EncryptParameters1(new string[]{"RoadCode =" + RoadDetails.IsPlanAvaliable.ToString().Trim()}) + "\");'></span></td></tr></table></center>",//URLEncrypt.EncryptParameters1(new string[]{"RoadCode =" + RoadDetails.IsPlanAvaliable.ToString().Trim()}),
                        RoadDetails.IsFinalize == "-" ? "-" :  RoadDetails.IsFinalize.Contains("$") ? "<center><table><tr><td  style='border:none'><span class='ui-icon  	ui-icon-locked ' title='Finalize Project Plan';'></span></td></tr></table></center>" :
                        "<center><table><tr><td  style='border:none'><span class='ui-icon  	ui-icon-unlocked ' title='Finalize Project Plan' onClick ='FinalizeProjectPlan(\"" + URLEncrypt.EncryptParameters1(new string[]{"RoadCode =" + RoadDetails.IsFinalize.ToString().Trim()})  + "\");'></span></td></tr></table></center>",
                        RoadDetails.IsRevisePlan == " " ? "-" : "<a href='#' title='Click here to Revise Plan Details'  onClick=RevisePlanDetails('"+URLEncrypt.EncryptParameters1(new string[]{"RoadCode =" + RoadDetails.IsRevisePlan.ToString().Trim(),"StateShare =" + RoadDetails.StateShare.ToString().Trim(),"MordShare =" + RoadDetails.MordShare.ToString().Trim(),"TotalSanctionedDate =" + RoadDetails.TotalSanctionedCost.ToString().Trim()})+"'); return false;'>Revise Plan</a>",
                        RoadDetails.IsActualsAvaliable == ""? "-": "<center><table><tr><td  style='border:none'><span class='ui-icon ui-icon-circle-plus ' title='Add Actuals' onClick =AddActuals('"+URLEncrypt.EncryptParameters1(new string[]{"RoadCode =" + RoadDetails.IsActualsAvaliable.ToString().Trim(),"StateShare =" + RoadDetails.StateShare.ToString().Trim(),"MordShare =" + RoadDetails.MordShare.ToString().Trim(),"TotalSanctionedDate =" + RoadDetails.TotalSanctionedCost.ToString().Trim()})+"');></span></td></tr></table></center>",
                        RoadDetails.IsActualsAvaliable == ""? "-": "<center><table><tr><td  style='border:none'><span class='ui-icon ui-icon-circle-plus ' title='Add Chainage wise Details' onClick =AddChainage('"+URLEncrypt.EncryptParameters1(new string[]{"RoadCode =" + RoadDetails.IsActualsAvaliable.ToString().Trim()})+"');></span></td></tr></table></center>"

                        
                    }
                }).ToArray();

            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "PMISRoadListDAL().DAL");
                totalRecords = 0;
                return null;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        public GanttChartResponseModel GetDataForGanttChartDAL(int RoadCode, int BaseLine)
        {
            try
            {
                GanttChartResponseModel response = new GanttChartResponseModel();

                PMGSYEntities dbContext = new PMGSYEntities();
                var result = dbContext.USP_RPT_PMIS_TARGET_AND_PROGRESS_DETAILS(RoadCode, BaseLine).ToList();
                var workDetails = dbContext.USP_PMIS_WORK_DETAILS(RoadCode).ToList();

                foreach (var item in result)
                {
                    if (item.PLANNED_START_DATE != null)
                    {
                        GanttChartData obj = new GanttChartData();

                        foreach (var work in workDetails)
                        {
                            obj.State = work.MAST_STATE_NAME;
                            obj.BlockName = work.MAST_BLOCK_NAME;
                            obj.SanctionYear = work.SANCTION_YEAR;
                            obj.SanctionCost = work.TENDER_AMOUNT;
                            obj.SanctionDate = work.SANCTIONED_DATE.HasValue ? work.SANCTIONED_DATE.Value.ToShortDateString() : "No Date Available";
                            obj.AgreementCost = work.AGREEMENT_AMOUNT;
                            obj.ImsBatch = work.IMS_BATCH;
                            obj.PiuName = work.PIU_NAME;
                        }
                        obj.name = item.ACTIVITY_DESC;
                        obj.start = item.PLANNED_START_DATE.HasValue ? item.PLANNED_START_DATE.Value.Subtract(new DateTime(1970, 1, 1)).TotalMilliseconds : 0;
                        obj.end = item.PLANNED_COMPLETION_DATE.HasValue ? item.PLANNED_COMPLETION_DATE.Value.Subtract(new DateTime(1970, 1, 1)).TotalMilliseconds : 0;
                        obj.District = item.MAST_DISTRICT_NAME;
                        obj.RoadCode = item.IMS_PR_ROAD_CODE;
                        obj.RoadName = item.IMS_Road_Name;
                        obj.SanctionedLength = item.IMS_PAV_LENGTH;
                        obj.PackageNo = item.IMS_PACkage_ID;

                        CompletedClass completed = new CompletedClass();
                        completed.amount = item.PROGRESS.HasValue ? ((item.PROGRESS.Value != 0) ? item.PROGRESS.Value / 100 : 0) : 0;
                        completed.fill = "#4982B3";

                        //  obj.completed = item.PROGRESS.HasValue? ((item.PROGRESS.Value!=0)? item.PROGRESS.Value/100:0) : 0;
                        obj.completed = completed;

                        obj.color = "#95CEFF";

                        response.ListForChart.Add(obj);
                    }
                }

                return response;
            }
            catch (Exception ex)
            {

                throw;
            }
        }


        #region PMIS Data Correction
               
        public Array PMISDataCorrectionList(int state, int district, int block, int sanction_year, int batch, string listType, int? page, int? rows, string sidx, string sord, out long totalRecords)
        {
            try
            {
                dbContext = new PMGSYEntities();
                if (listType.Equals("R"))
                {
                    var PMISRoadList = (from prd in dbContext.USP_PMS_ROAD_LIST(state, district, block, sanction_year, batch)
                                        join plmrd in dbContext.PMIS_PLAN_MASTER on prd.PrRoadCode equals plmrd.IMS_PR_ROAD_CODE
                                        select new
                                        {
                                            prd.MAST_STATE_NAME,
                                            prd.MAST_DISTRICT_NAME,
                                            prd.BlockName,
                                            prd.PackageId,
                                            prd.SanctionYear,
                                            prd.IMS_SANCTIONED_DATE,
                                            prd.MAST_BATCH_NAME,
                                            prd.SanctionLength,
                                            prd.TEND_AGREEMENT_NUMBER,
                                            prd.TEND_AGREEMENT_AMOUNT,
                                            prd.Mord_Share,
                                            prd.State_share,
                                            prd.TOTAL_COST,
                                            prd.TEND_AGREEMENT_START_DATE,
                                            prd.TEND_AGREEMENT_END_DATE,
                                            prd.RoadName,
                                            prd.PrRoadCode
                                        }).Distinct().ToList();

                    var resultList = new List<PMISDataCorrectionRoadDAL>();

                    foreach (var item in PMISRoadList)
                    {                       
                        var latestPlanDetails = dbContext.PMIS_PLAN_MASTER.Where(a => a.IMS_PR_ROAD_CODE == item.PrRoadCode).Where(a => a.IS_LATEST == "Y").Select(x => new { x.PLAN_ID, x.BASELINE_NO }).First();
                        var checkedPlanID = (from pm in dbContext.PMIS_PROGRESS_MASTER
                                             where pm.PLAN_ID == latestPlanDetails.PLAN_ID && pm.BASELINE_NO == latestPlanDetails.BASELINE_NO && pm.IS_LATEST == "Y" && pm.PROJECT_STATUS_ == "C"
                                             select new { pm.PLAN_ID }).Any();

                        resultList.Add(new PMISDataCorrectionRoadDAL
                        {
                            StateName = item.MAST_STATE_NAME,
                            DistrictName = item.MAST_DISTRICT_NAME,
                            BlockName = item.BlockName,
                            PackageName = item.PackageId,
                            SanctionYear = (item.SanctionYear).ToString() + "-" + (item.SanctionYear + 1).ToString().Substring(2, 2),
                            SanctionDate = ((item.IMS_SANCTIONED_DATE).Value).ToShortDateString(),
                            BatchName = item.MAST_BATCH_NAME,
                            SanctionLength = (item.SanctionLength).ToString(),
                            AgreementNo = item.TEND_AGREEMENT_NUMBER,
                            AgreementCost = item.TEND_AGREEMENT_AMOUNT.ToString(),
                            MordShare = (item.Mord_Share ?? default(decimal)).ToString(),
                            StateShare = item.State_share.ToString(),
                            TotalSanctionedCost = item.TOTAL_COST.ToString(),
                            AgreementStartDate = (item.TEND_AGREEMENT_START_DATE == null) ? "NULL" : ((item.TEND_AGREEMENT_START_DATE).Value).ToShortDateString(),
                            AgreementEndDate = (item.TEND_AGREEMENT_END_DATE == null) ? "NULL" : ((item.TEND_AGREEMENT_END_DATE).Value).ToShortDateString(),
                            RoadName = item.RoadName,
                            IMS_PR_RoadCode = item.PrRoadCode.ToString(),
                            planCheckedRoadCode = (checkedPlanID == false) ? null : item.PrRoadCode.ToString()
                        });
                    }

                    totalRecords = PMISRoadList.Count();

                    return resultList.Select(RoadDetails => new
                    {
                        cell = new[] {    
                        RoadDetails.StateName,
                        RoadDetails.DistrictName,   
                        RoadDetails.BlockName,      
                        RoadDetails.PackageName,    
                        RoadDetails.SanctionYear,
                        RoadDetails.SanctionDate,
                        RoadDetails.BatchName,      
                        RoadDetails.SanctionLength, 
                        RoadDetails.AgreementNo,    
                        RoadDetails.AgreementCost, 
                        RoadDetails.MordShare,
                        RoadDetails.StateShare,
                        RoadDetails.TotalSanctionedCost,
                        RoadDetails.AgreementStartDate,
                        RoadDetails.AgreementEndDate,
                        RoadDetails.RoadName,
                        RoadDetails.IMS_PR_RoadCode == null ? "<center><table><tr><td style='border:none;cursor:pointer'><span class='ui-icon ui-icon-minusthick' title='Plans and Progress Not Available'></span></td></tr></table></center>" :"<center><table><tr><td style='border:none;cursor:pointer'><span class='ui-icon ui-icon-trash' title='Delete Project Plan Progress' onClick ='DeleteProjectProgressPlan(\"" + RoadDetails.IMS_PR_RoadCode.ToString().Trim() + "\");'></span></td></tr></table></center>",
                        RoadDetails.planCheckedRoadCode == null ? "<center><table><tr><td style='border:none;cursor:pointer'><span class='ui-icon ui-icon-minusthick' title='Update Completion Length & Date Details'></span></td></tr></table></center>" :"<center><table><tr><td style='border:none;cursor:pointer'><span class='ui-icon ui-icon-pencil' title='Update Completion Length and Date Details' onClick ='UpdateCompletionLengthDateDetails(\"" + RoadDetails.IMS_PR_RoadCode.ToString().Trim() + "\");'></span></td></tr></table></center>"
                        }
                    }).ToArray();
                }
                else
                {                   
                    var PMISBridgeList = (from pbg in dbContext.USP_PMS_BRIDGE_LIST(state, district, block, sanction_year, batch)
                                          join plmrd in dbContext.PMIS_PLAN_MASTER on pbg.PrRoadCode equals plmrd.IMS_PR_ROAD_CODE
                                          select new
                                          {
                                              pbg.MAST_STATE_NAME,
                                              pbg.MAST_DISTRICT_NAME,
                                              pbg.BlockName,
                                              pbg.PackageId,
                                              pbg.SanctionYear,
                                              pbg.IMS_SANCTIONED_DATE,
                                              pbg.MAST_BATCH_NAME,
                                              pbg.SanctionLength,
                                              pbg.TEND_AGREEMENT_NUMBER,
                                              pbg.TEND_AGREEMENT_AMOUNT,
                                              pbg.Mord_Share,
                                              pbg.State_share,
                                              pbg.TOTAL_COST,
                                              pbg.TEND_AGREEMENT_START_DATE,
                                              pbg.TEND_AGREEMENT_END_DATE,
                                              pbg.LSBName,
                                              pbg.PrRoadCode,
                                          }).Distinct().ToList();

                    var resultList = new List<PMISDataCorrectionBridgeDAL>();

                    foreach (var item in PMISBridgeList)
                    {                        
                        var latestPlanDetails = dbContext.PMIS_PLAN_MASTER.Where(a => a.IMS_PR_ROAD_CODE == item.PrRoadCode).Where(a => a.IS_LATEST == "y").Select(x => new { x.PLAN_ID, x.BASELINE_NO }).First();
                        var checkedPlanID = (from pm in dbContext.PMIS_PROGRESS_MASTER
                                             where pm.PLAN_ID == latestPlanDetails.PLAN_ID && pm.BASELINE_NO == latestPlanDetails.BASELINE_NO && pm.IS_LATEST == "Y" && pm.PROJECT_STATUS_ == "C"
                                             select new { pm.PLAN_ID }).Any();

                        resultList.Add(new PMISDataCorrectionBridgeDAL
                        {
                            StateName = item.MAST_STATE_NAME,
                            DistrictName = item.MAST_DISTRICT_NAME,
                            BlockName = item.BlockName,
                            PackageName = item.PackageId,
                            SanctionYear = (item.SanctionYear).ToString() + "-" + (item.SanctionYear + 1).ToString().Substring(2, 2),
                            SanctionDate = ((item.IMS_SANCTIONED_DATE).Value).ToShortDateString(),
                            BatchName = item.MAST_BATCH_NAME,
                            SanctionLength = (item.SanctionLength).ToString(),
                            AgreementNo = item.TEND_AGREEMENT_NUMBER,
                            AgreementCost = item.TEND_AGREEMENT_AMOUNT.ToString(),
                            MordShare = (item.Mord_Share ?? default(decimal)).ToString(),
                            StateShare = item.State_share.ToString(),
                            TotalSanctionedCost = item.TOTAL_COST.ToString(),
                            AgreementStartDate = (item.TEND_AGREEMENT_START_DATE == null) ? "NULL" : ((item.TEND_AGREEMENT_START_DATE).Value).ToShortDateString(),
                            AgreementEndDate = (item.TEND_AGREEMENT_END_DATE == null) ? "NULL" : ((item.TEND_AGREEMENT_END_DATE).Value).ToShortDateString(),
                            LSBName = item.LSBName,
                            IMS_PR_RoadCode = item.PrRoadCode.ToString(),
                            planCheckedRoadCode = checkedPlanID == false ? null : item.PrRoadCode.ToString()
                        });
                    }

                    totalRecords = PMISBridgeList.Count();

                    return resultList.Select(BridgeDetails => new
                    {
                        cell = new[] {    
                            BridgeDetails.StateName,
                            BridgeDetails.DistrictName,   
                            BridgeDetails.BlockName,      
                            BridgeDetails.PackageName,    
                            BridgeDetails.SanctionYear,
                            BridgeDetails.SanctionDate,
                            BridgeDetails.BatchName,      
                            BridgeDetails.SanctionLength, 
                            BridgeDetails.AgreementNo,    
                            BridgeDetails.AgreementCost, 
                            BridgeDetails.MordShare,
                            BridgeDetails.StateShare,
                            BridgeDetails.TotalSanctionedCost,
                            BridgeDetails.AgreementStartDate,
                            BridgeDetails.AgreementEndDate,
                            BridgeDetails.LSBName,
                            BridgeDetails.IMS_PR_RoadCode == null ? "<center><table><tr><td style='border:none;cursor:pointer'><span class='ui-icon ui-icon-minusthick' title='Plans and Progress Not Available'></span></td></tr></table></center>" :"<center><table><tr><td style='border:none;cursor:pointer'><span class='ui-icon ui-icon-trash' title='Delete Project Plans Progress' onClick ='DeleteProjectProgressPlan(\"" + BridgeDetails.IMS_PR_RoadCode.ToString().Trim()+ "\");'></span></td></tr></table></center>",                          
                            BridgeDetails.planCheckedRoadCode = "<center><table><tr><td style='border:none;cursor:pointer'><span class='ui-icon ui-icon-minusthick' title='Update Completion Length & Date Details'></span></td></tr></table></center>"
                           
                           //// Bridge activities not yet mapped for update
                           // BridgeDetails.planCheckedRoadCode == null ? "<center><table><tr><td style='border:none;cursor:pointer'><span class='ui-icon ui-icon-minusthick' title='Update Completion Length & Date Details'></span></td></tr></table></center>" :"<center><table><tr><td style='border:none;cursor:pointer'><span class='ui-icon ui-icon-pencil' title='Update Completion Length and Date Details' onClick ='UpdateCompletionLengthDateDetails(\"" + BridgeDetails.IMS_PR_RoadCode.ToString().Trim() + "\");'></span></td></tr></table></center>"  
                        }
                    }).ToArray();
                }
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "PMISDAL.PMISDataCorrectionList()");
                totalRecords = 0;
                return null;
            }
            finally
            {
                dbContext.Dispose();
            }
        }
               
        public Array PMISDataDeleteProgressPlanList(int roadCode, int? page, int? rows, string sidx, string sord, out long totalRecords)
        {
            try
            {
                dbContext = new PMGSYEntities();

                var query = (from pln in dbContext.PMIS_PLAN_MASTER
                             where pln.IMS_PR_ROAD_CODE == roadCode
                             select new
                             {
                                 pln.PLAN_ID,
                                 pln.IMS_PR_ROAD_CODE,
                                 pln.BASELINE_NO,
                                 DeletePlanId = dbContext.PMIS_PROGRESS_MASTER.Where(w => w.PLAN_ID == pln.PLAN_ID).Any() ? true : false,
                                 DeleteProgressId = dbContext.PMIS_PROGRESS_MASTER.Where(s => s.PLAN_ID == pln.PLAN_ID).Any() ? true : false
                             });

                totalRecords = query.Count();
                                
                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "desc")
                    {
                        switch (sidx)
                        {
                            case "BASELINE_NO":
                                query = query.OrderByDescending(x => x.BASELINE_NO).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                            default:
                                break;
                        }
                    }
                }

                var result = query.Select(item => new
                {
                    item.PLAN_ID,
                    item.IMS_PR_ROAD_CODE,
                    BASELINE = "Baseline " + SqlFunctions.StringConvert((double?)item.BASELINE_NO).Trim(),
                    item.DeletePlanId,
                    item.DeleteProgressId
                }).ToArray();

                return result;
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "PMISDAL.PMISDataDeleteProgressPlanList()");
                totalRecords = 0;
                return null;
            }
            finally
            {
                if (dbContext != null)
                {
                    dbContext.Dispose();
                }
            }
        }
              
        public string DataDeletePlan(int PLAN_ID)
        {
            dbContext = new PMGSYEntities();
            try
            {
                if (dbContext.PMIS_PLAN_MASTER.Where(a => a.PLAN_ID == PLAN_ID && a.IS_LATEST == "N").Any())
                {
                    return "This plan cannot be deleted first Delete Latest Plan";
                }
                if (dbContext.PMIS_PROGRESS_MASTER.Where(x => x.PLAN_ID == PLAN_ID).Any())
                {
                    return "This plan cannot be deleted as it contains actual progress details";
                }

                using (TransactionScope ts = new TransactionScope())
                {                    
                    List<PMIS_CHAINAGEWISE_COMPLETION_DETAILS> listChainageDetails = dbContext.PMIS_CHAINAGEWISE_COMPLETION_DETAILS.Where(m => m.PLAN_ID == PLAN_ID).ToList();
                    foreach (var item in listChainageDetails)
                    {
                        dbContext.PMIS_CHAINAGEWISE_COMPLETION_DETAILS.Remove(item);
                    }

                    List<PMIS_PLAN_DETAILS> lstnewPlanDetails = dbContext.PMIS_PLAN_DETAILS.Where(m => m.PLAN_ID == PLAN_ID && m.IS_LATEST == "Y").ToList();
                    foreach (var item in lstnewPlanDetails)
                    {
                        dbContext.PMIS_PLAN_DETAILS.Remove(item);
                    }
                                        
                    PMIS_PLAN_MASTER planMaster = dbContext.PMIS_PLAN_MASTER.Where(z => z.PLAN_ID == PLAN_ID & z.IS_LATEST == "Y").FirstOrDefault();
                    if (planMaster != null)
                    {
                       dbContext.Entry(planMaster).State = EntityState.Deleted;
                    }

                    int? roadCode = dbContext.PMIS_PLAN_MASTER.Where(w => w.PLAN_ID == PLAN_ID).Select(s => s.IMS_PR_ROAD_CODE).FirstOrDefault();

                    dbContext.SaveChanges();

                    PMIS_PLAN_MASTER oldmasterplan = dbContext.PMIS_PLAN_MASTER.Where(v => v.IMS_PR_ROAD_CODE == roadCode).OrderByDescending(x => x.BASELINE_NO).FirstOrDefault();
                    if (oldmasterplan != null)
                    {
                        oldmasterplan.IS_LATEST = "Y";
                        dbContext.Entry(oldmasterplan).State = System.Data.Entity.EntityState.Modified;

                        int? Baseline_No = oldmasterplan.BASELINE_NO;
                                              
                        List<PMIS_PLAN_DETAILS> lstoldPlanDetails = dbContext.PMIS_PLAN_DETAILS.Where(m => m.IMS_PR_ROAD_CODE == roadCode & m.BASELINE_NO == Baseline_No).ToList();
                        foreach (var item in lstoldPlanDetails)
                        {
                            item.IS_LATEST = "Y";
                            dbContext.Entry(item).State = System.Data.Entity.EntityState.Modified;
                        }

                        List<PMIS_PROGRESS_MASTER> lstoldProgressMaster = dbContext.PMIS_PROGRESS_MASTER.Where(m => m.IMS_PR_ROAD_CODE == roadCode & m.BASELINE_NO == Baseline_No).ToList();
                        foreach (var item in lstoldProgressMaster)
                        {
                            item.IS_LATEST = "Y";
                            dbContext.Entry(item).State = System.Data.Entity.EntityState.Modified;
                        }

                        List<PMIS_PROGRESS_DETAILS> lstoldProgressDetail = dbContext.PMIS_PROGRESS_DETAILS.Where(m => m.IMS_PR_ROAD_CODE == roadCode & m.BASELINE_NO == Baseline_No).ToList();
                        foreach (var item in lstoldProgressDetail)
                        {
                            item.IS_LATEST = "Y";
                            dbContext.Entry(item).State = System.Data.Entity.EntityState.Modified;
                        }
                    }
                    dbContext.SaveChanges();
                    ts.Complete();
                    return string.Empty;
                }
            }
            catch (DbUpdateException sx)
            {
                ErrorLog.LogError(sx, "DataDeletePlan().DAL");
                return ("An Update Error Occurred While Processing Your Request");
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "DataDeletePlan().DAL");
                return ("Error Occurred While Processing Request.");
            }
            finally
            {
                dbContext.Dispose();
            }
        }
                
        public string DataDeleteProgress(int PLAN_ID)
        {
            dbContext = new PMGSYEntities();
            try
            {
                if (dbContext.PMIS_PLAN_MASTER.Where(a => a.PLAN_ID == PLAN_ID && a.IS_LATEST == "N").Any())
                {
                    return "This progress cannot be deleted first delete latest progress";
                }

                using (TransactionScope ts = new TransactionScope())
                {
                    List<PMIS_PROGRESS_DETAILS> listProgressDetails = dbContext.PMIS_PROGRESS_DETAILS.Where(m => m.PLAN_ID == PLAN_ID).ToList();
                    foreach (var item in listProgressDetails)
                    {
                        dbContext.PMIS_PROGRESS_DETAILS.Remove(item);
                    }

                    List<PMIS_PROGRESS_MASTER> listprogressMaster = dbContext.PMIS_PROGRESS_MASTER.Where(z => z.PLAN_ID == PLAN_ID).ToList();
                    foreach (var item in listprogressMaster)
                    {                        
                        dbContext.Entry(item).State = EntityState.Deleted;
                    }
                    dbContext.SaveChanges();
                    ts.Complete();
                    return string.Empty;
                }
            }
            catch (DbUpdateException sx)
            {
                ErrorLog.LogError(sx, "DataDeleteProgress().DAL");
                return ("An Update Error Occurred While Processing Your Request.");
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "DataDeleteProgress().DAL");
                return ("Error Occurred While Processing Request.");
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        public UpdateCompletionDateLengthModel GetPlanDetailsToEdit(int RoadCode)
        {
            dbContext = new PMGSYEntities();
            UpdateCompletionDateLengthModel model = new UpdateCompletionDateLengthModel();

            try
            {
                var updateRoadDetails = dbContext.IMS_SANCTIONED_PROJECTS.Where(v => v.IMS_PR_ROAD_CODE == RoadCode).FirstOrDefault();
                var stateName = dbContext.MASTER_STATE.Where(b => b.MAST_STATE_CODE == updateRoadDetails.MAST_STATE_CODE).Select(b => b.MAST_STATE_NAME).First();
                var districtName = dbContext.MASTER_DISTRICT.Where(c => c.MAST_DISTRICT_CODE == updateRoadDetails.MAST_DISTRICT_CODE).Select(c => c.MAST_DISTRICT_NAME).First();
                var blockName = dbContext.MASTER_BLOCK.Where(d => d.MAST_BLOCK_CODE == updateRoadDetails.MAST_BLOCK_CODE).Select(d => d.MAST_BLOCK_NAME).First();
                string sanctionYear = (updateRoadDetails.IMS_YEAR).ToString() + "-" + (updateRoadDetails.IMS_YEAR + 1).ToString().Substring(2, 2);
                string roadName = updateRoadDetails.IMS_ROAD_NAME.ToString();

                var latestplanDetail = dbContext.PMIS_PLAN_MASTER.Where(a => a.IMS_PR_ROAD_CODE == RoadCode).Where(a => a.IS_LATEST == "Y").Select(x => new { x.PLAN_ID, x.BASELINE_NO }).First();

                var CompletionDetails = (from pgm in dbContext.PMIS_PROGRESS_MASTER
                                         where pgm.PLAN_ID == latestplanDetail.PLAN_ID && pgm.BASELINE_NO == latestplanDetail.BASELINE_NO && pgm.IS_LATEST == "Y" && pgm.PROJECT_STATUS_ == "C"
                                         select new
                                         {
                                             pgm.PLAN_ID,
                                             pgm.BASELINE_NO,
                                             pgm.ENTRY_DATE,
                                             pgm.COMPLETION_LENGTH
                                         }).OrderByDescending(a => a.ENTRY_DATE).First();

                if (CompletionDetails.PLAN_ID > 0)
                {
                    var planCompletionDate = CompletionDetails.ENTRY_DATE == null ? string.Format("{0:dd/MM/yyyy}") : (CompletionDetails.ENTRY_DATE).ToShortDateString();
                    var planLength = CompletionDetails.COMPLETION_LENGTH < 0 ? Convert.ToDecimal("0.000") : Convert.ToDecimal(CompletionDetails.COMPLETION_LENGTH);

                    model.StateName = stateName.ToString();
                    model.DistrictName = districtName.ToString();
                    model.BlockName = blockName.ToString();
                    model.SanctionYear = sanctionYear;
                    model.RoadName = roadName;
                    model.PlanId = CompletionDetails.PLAN_ID;
                    model.BaselineNo = CompletionDetails.BASELINE_NO;
                    model.CompletedLength = planLength;
                    model.CompletedDate = planCompletionDate;
                    return model;
                }
                return null;
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "PMISDAL.GetPlanDetailsToEdit()");
                return null;
            }
            finally
            {
                if (dbContext != null)
                {
                    dbContext.Dispose();
                }
            }
        }


        public string UpdatePlanCompletionDetails(string listType, int planid, int baselineno, decimal completionLength, DateTime? completionDate)
        {
            dbContext = new PMGSYEntities();
            try
            {
                using (TransactionScope ts = new TransactionScope())
                {
                    DateTime completiondate = Convert.ToDateTime(completionDate);
                    PMIS_PROGRESS_MASTER oldProgressMaster = dbContext.PMIS_PROGRESS_MASTER.Where(v => v.PLAN_ID == planid).Where(v => v.BASELINE_NO == baselineno).Where(v => v.IS_LATEST == "Y").Where(v => v.PROJECT_STATUS_ == "C").OrderByDescending(v => v.ENTRY_DATE).First();
                    int roadcode = oldProgressMaster.IMS_PR_ROAD_CODE;

                    if (oldProgressMaster != null)
                    {
                        if (listType.Equals("R"))
                        {
                            PMIS_PROGRESS_DETAILS oldProgressDetail = dbContext.PMIS_PROGRESS_DETAILS.Where(v => v.PLAN_ID == planid).Where(v => v.BASELINE_NO == baselineno).Where(v => v.ACTIVITY_ID == 10).Where(v => v.IS_LATEST == "Y").OrderByDescending(v => v.ENTRY_DATE).First();

                            if (oldProgressDetail != null)
                            {
                                oldProgressDetail.ENTRY_DATE = completiondate;
                                dbContext.Entry(oldProgressDetail).State = System.Data.Entity.EntityState.Modified;
                            }

                            oldProgressMaster.COMPLETION_LENGTH = completionLength;
                            oldProgressMaster.ENTRY_DATE = completiondate;
                            dbContext.Entry(oldProgressMaster).State = System.Data.Entity.EntityState.Modified;

                            EXEC_ROADS_MONTHLY_STATUS roadMonthlyStatus = dbContext.EXEC_ROADS_MONTHLY_STATUS.Where(v => v.IMS_PR_ROAD_CODE == roadcode).Where(v => v.EXEC_ISCOMPLETED == "C").OrderByDescending(v => v.EXEC_COMPLETION_DATE).First();
                            if (roadMonthlyStatus != null)
                            {
                                roadMonthlyStatus.EXEC_COMPLETION_DATE = completiondate;
                                roadMonthlyStatus.EXEC_COMPLETED = completionLength;
                                dbContext.Entry(roadMonthlyStatus).State = System.Data.Entity.EntityState.Modified;
                            }

                            IMS_SANCTIONED_PROJECTS sanctionedModel = dbContext.IMS_SANCTIONED_PROJECTS.Where(v => v.IMS_PR_ROAD_CODE == roadcode).Where(v => v.IMS_ISCOMPLETED == "C").First();
                            if (sanctionedModel != null)
                            {
                                sanctionedModel.IMS_ENTRY_DATE_PHYSICAL = completiondate;
                                dbContext.Entry(sanctionedModel).State = System.Data.Entity.EntityState.Modified;
                            }
                        }
                        else
                        {
                            PMIS_PROGRESS_DETAILS oldProgressDetail = dbContext.PMIS_PROGRESS_DETAILS.Where(v => v.PLAN_ID == planid).Where(v => v.BASELINE_NO == baselineno).Where(v => v.IS_LATEST == "Y").OrderByDescending(v => v.ENTRY_DATE).First();

                            if (oldProgressDetail != null)
                            {
                                oldProgressDetail.ENTRY_DATE = completiondate;
                                dbContext.Entry(oldProgressDetail).State = System.Data.Entity.EntityState.Modified;
                            }

                            oldProgressMaster.COMPLETION_LENGTH = completionLength;
                            oldProgressMaster.ENTRY_DATE = completiondate;
                            dbContext.Entry(oldProgressMaster).State = System.Data.Entity.EntityState.Modified;

                            EXEC_LSB_MONTHLY_STATUS bridgeMonthlyStatus = dbContext.EXEC_LSB_MONTHLY_STATUS.Where(v => v.IMS_PR_ROAD_CODE == roadcode).Where(v => v.EXEC_ISCOMPLETED == "C").OrderByDescending(v => v.EXEC_COMPLETION_DATE).First();
                            if (bridgeMonthlyStatus != null)
                            {
                                bridgeMonthlyStatus.EXEC_COMPLETION_DATE = completiondate;
                                dbContext.Entry(bridgeMonthlyStatus).State = System.Data.Entity.EntityState.Modified;
                            }

                            IMS_SANCTIONED_PROJECTS sanctionedModel = dbContext.IMS_SANCTIONED_PROJECTS.Where(v => v.IMS_PR_ROAD_CODE == roadcode).Where(v => v.IMS_ISCOMPLETED == "C").First();
                            if (sanctionedModel != null)
                            {
                                sanctionedModel.IMS_ENTRY_DATE_PHYSICAL = completiondate;
                                dbContext.Entry(sanctionedModel).State = System.Data.Entity.EntityState.Modified;
                            }
                        }
                    }
                    dbContext.SaveChanges();
                    ts.Complete();
                    return string.Empty;
                }

            }
            catch (DbUpdateException sx)
            {
                ErrorLog.LogError(sx, "DataDeletePlan().DAL");
                return ("Update Error Occurred While Processing Your Request");
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "PMISDAL.UpdatePlanCompletionDetails()");
                return ("Error Occurred While Processing Your Request");
            }
            finally
            {
                if (dbContext != null)
                {
                    dbContext.Dispose();
                }
            }
        }

        #endregion

        #region  Bridge
        //public string UpdatePMISBridgeProjectPlanDAL(IEnumerable<AddPlanPMISViewModelBridge> planData)
        //{
        //    PMGSYEntities dbContext = new PMGSYEntities();
        //    PMIS_PLAN_MASTER planMaster;
        //    try
        //    {
        //        using (TransactionScope ts = new TransactionScope())
        //        {

        //            int RoadCode = 0;  //planData.ElementAt(0).IMS_PR_ROAD_CODE;
        //            foreach (var plan in planData)
        //            {
        //                RoadCode = plan.IMS_PR_ROAD_CODE;
        //                if (RoadCode > 0)
        //                    break;
        //                else
        //                    continue;
        //            }
        //            //PMIS_PLAN_MASTER planMaster = dbContext.PMIS_PLAN_MASTER.Where(x => x.IMS_PR_ROAD_CODE == RoadCode && x.IS_LATEST == "Y" && x.IS_FINALISED != "Y").FirstOrDefault();
        //            if (PMGSYSession.Current.RoleCode == 36)
        //            {
        //                planMaster = dbContext.PMIS_PLAN_MASTER.Where(x => x.IMS_PR_ROAD_CODE == RoadCode && x.IS_LATEST == "Y" && (x.IS_FINALISED != "Y" || x.IS_FINALISED == "Y")).FirstOrDefault();

        //            }
        //            else
        //            {
        //                planMaster = dbContext.PMIS_PLAN_MASTER.Where(x => x.IMS_PR_ROAD_CODE == RoadCode && x.IS_LATEST == "Y" && x.IS_FINALISED != "Y").FirstOrDefault();
        //            }
        //            planMaster.USERID = PMGSYSession.Current.UserId;
        //            planMaster.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
        //            planMaster.IS_LATEST = "Y";
        //            //planMaster.IS_FINALISED = "N";
        //            dbContext.Entry(planMaster).State = System.Data.Entity.EntityState.Modified;
        //            dbContext.SaveChanges();

        //            foreach (var obj in planData)
        //            {
        //                int ActivityId = dbContext.PMIS_ACTIVITY_MASTER.Where(x => x.ACTIVITY_DESC == obj.ACTIVITY_DESC && x.ROAD_TYPE == "L").Select(x => x.ACTIVITY_ID).FirstOrDefault();
        //                PMIS_PLAN_DETAILS planDetails = dbContext.PMIS_PLAN_DETAILS.Where(x => x.IMS_PR_ROAD_CODE == RoadCode && x.IS_LATEST == "Y" && x.PLAN_ID == planMaster.PLAN_ID && x.ACTIVITY_ID == ActivityId).FirstOrDefault();
        //                planDetails.IMS_PR_ROAD_CODE = planMaster.IMS_PR_ROAD_CODE;
        //                planDetails.QUANTITY = obj.QUANTITY;
        //                planDetails.AGREEMENT_COST = obj.AGREEMENT_COST;
        //                planDetails.PLANNED_START_DATE = obj.PLANNED_START_DATE;
        //                planDetails.PLANNED_DURATION = obj.PLANNED_DURATION;
        //                planDetails.PLANNED_COMPLETION_DATE = obj.PLANNED_COMPLETION_DATE;
        //                planDetails.IS_LATEST = "Y";
        //                //planDetails.BASELINE_NO = 1;
        //                planDetails.USERID = PMGSYSession.Current.UserId;
        //                planDetails.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];

        //                dbContext.Entry(planMaster).State = System.Data.Entity.EntityState.Modified;
        //                dbContext.SaveChanges();
        //            }
        //            ts.Complete();
        //        }
        //        return string.Empty;
        //    }

        //    catch (DbUpdateException ex)
        //    {
        //        ErrorLog.LogError(ex, "UpdatePMISBridgeProjectPlanDAL(DbUpdateException ex).DAL");
        //        return ("An Error Occurred While Processing Your Request.");
        //    }
        //    catch (Exception ex)
        //    {
        //        ErrorLog.LogError(ex, "UpdatePMISBridgeProjectPlanDAL.DAL");
        //        return ("Error Occurred While Processing Request.");
        //    }
        //    finally
        //    {
        //        dbContext.Dispose();
        //    }
        //}

        //public string FinalizeBridgeProjectPlanDAL(int IMS_PR_ROAD_CODE)
        //{
        //    try
        //    {
        //        dbContext = new PMGSYEntities();

        //        PMIS_PLAN_MASTER FinalizePlanMaster = dbContext.PMIS_PLAN_MASTER.Where(z => z.IMS_PR_ROAD_CODE == IMS_PR_ROAD_CODE & z.IS_LATEST == "Y").FirstOrDefault();
        //        //PMIS_PLAN_DETAILS planDetails = dbContext.PMIS_PLAN_DETAILS.Where(z => z.IMS_PR_ROAD_CODE == IMS_PR_ROAD_CODE & z.IS_LATEST == "Y" && z.ACTIVITY_ID == 1).FirstOrDefault();
        //        PMIS_PLAN_DETAILS planDetails = dbContext.PMIS_PLAN_DETAILS.Where(z => z.IMS_PR_ROAD_CODE == IMS_PR_ROAD_CODE & z.IS_LATEST == "Y" && z.ACTIVITY_ID == 18).FirstOrDefault();

        //        if (FinalizePlanMaster != null)
        //        {
        //            if (planDetails.PLANNED_START_DATE != null && planDetails.PLANNED_COMPLETION_DATE != null)
        //            {
        //                FinalizePlanMaster.IS_FINALISED = "Y";
        //                dbContext.Entry(FinalizePlanMaster).State = System.Data.Entity.EntityState.Modified;
        //                dbContext.SaveChanges();
        //                return string.Empty;
        //            }
        //            else
        //            {
        //                return "Plan of a project cannot be finalized if the planned start date and planned end date of Field lab is not entered";
        //            }
        //        }
        //        else
        //        {
        //            return "No Plans against this Road ";
        //        }

        //    }
        //    catch (DbUpdateException ex)
        //    {
        //        ErrorLog.LogError(ex, "FinalizBridgeProjectPlanDAL(DbUpdateException ex).DAL");
        //        return ("An Update Error Occurred While Processing Your Request.");
        //    }
        //    catch (Exception ex)
        //    {
        //        ErrorLog.LogError(ex, "FinalizBridgeProjectPlanDAL().DAL");
        //        return ("Error Occurred While Processing Request.");
        //    }
        //    finally
        //    {
        //        dbContext.Dispose();
        //    }
        //}

        //public string ReviseBridgeProjectPlanDAL(int IMS_PR_ROAD_CODE)
        //{
        //    try
        //    {
        //        dbContext = new PMGSYEntities();
        //        using (TransactionScope ts = new TransactionScope())
        //        {

        //            // if (dbContext.PMIS_PLAN_DETAILS.Any(m => m.IMS_PR_ROAD_CODE == IMS_PR_ROAD_CODE))
        //            // {
        //            PMIS_PLAN_MASTER RevisePlanMaster = dbContext.PMIS_PLAN_MASTER.Where(z => z.IMS_PR_ROAD_CODE == IMS_PR_ROAD_CODE & z.IS_FINALISED == "Y" & z.IS_LATEST == "Y").FirstOrDefault();

        //            int planid = dbContext.PMIS_PLAN_MASTER.Max(s => s.PLAN_ID) + 1;
        //            int baseLine = RevisePlanMaster.BASELINE_NO == null ? 0 : (int)(RevisePlanMaster.BASELINE_NO + 1);

        //            if (RevisePlanMaster != null)
        //            {
        //                //For new Revise Entry
        //                dbContext.PMIS_PLAN_MASTER.Add(new PMIS_PLAN_MASTER
        //                {
        //                    PLAN_ID = planid,
        //                    IMS_PR_ROAD_CODE = RevisePlanMaster.IMS_PR_ROAD_CODE,
        //                    PLAN_CREATION_DATE = DateTime.Now,
        //                    USERID = PMGSYSession.Current.UserId,
        //                    IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"],
        //                    IS_LATEST = "Y",
        //                    BASELINE_NO = baseLine, // RevisePlanMaster.BASELINE_NO + 1,
        //                    IS_FINALISED = "N"
        //                });

        //                // For old 
        //                RevisePlanMaster.IS_LATEST = "N";
        //                dbContext.Entry(RevisePlanMaster).State = System.Data.Entity.EntityState.Modified;
        //                dbContext.SaveChanges();
        //                //   }
        //                // var adapter = (IObjectContextAdapter)dbContext;
        //                //var objectContext = adapter.ObjectContext;
        //                //objectContext.CommandTimeout = 0;
        //                long count = dbContext.PMIS_PLAN_DETAILS.Max(s => s.DETAILED_ID) + 1;
        //                List<PMIS_PLAN_DETAILS> lstPlanDetails = dbContext.PMIS_PLAN_DETAILS.Where(m => m.IMS_PR_ROAD_CODE == IMS_PR_ROAD_CODE && m.BASELINE_NO == RevisePlanMaster.BASELINE_NO && m.PLAN_ID == RevisePlanMaster.PLAN_ID).ToList();
        //                // PMIS_PLAN_MASTER newplanmaster = dbContext.PMIS_PLAN_MASTER.Where(m => m.IMS_PR_ROAD_CODE == IMS_PR_ROAD_CODE & m.IS_LATEST == "Y").FirstOrDefault();
        //                foreach (var item in lstPlanDetails)
        //                {
        //                    //New record
        //                    PMIS_PLAN_DETAILS plandetails = new PMIS_PLAN_DETAILS();

        //                    plandetails.DETAILED_ID = count;
        //                    plandetails.PLAN_ID = planid;
        //                    plandetails.PLANNED_COMPLETION_DATE = item.PLANNED_COMPLETION_DATE;
        //                    plandetails.PLANNED_DURATION = item.PLANNED_DURATION;
        //                    plandetails.PLANNED_START_DATE = item.PLANNED_START_DATE;
        //                    plandetails.QUANTITY = item.QUANTITY;
        //                    plandetails.USERID = item.USERID;
        //                    plandetails.IPADD = item.IPADD;
        //                    plandetails.IS_LATEST = item.IS_LATEST;
        //                    plandetails.IMS_PR_ROAD_CODE = item.IMS_PR_ROAD_CODE;
        //                    plandetails.BASELINE_NO = baseLine; // item.BASELINE_NO + 1;
        //                    plandetails.AGREEMENT_COST = item.AGREEMENT_COST;
        //                    plandetails.ACTIVITY_ID = item.ACTIVITY_ID;


        //                    dbContext.PMIS_PLAN_DETAILS.Add(plandetails);
        //                    count++;

        //                    //Old record update
        //                    item.IS_LATEST = "N";
        //                    dbContext.Entry(item).State = System.Data.Entity.EntityState.Modified;
        //                    dbContext.SaveChanges();
        //                }
        //                //using (TransactionScope ts = new TransactionScope())
        //                //{
        //                //dbContext.SaveChanges();
        //                ts.Complete();

        //            }

        //            // }
        //            return string.Empty;
        //        }

        //        return ("No Plans against this Road");
        //    }
        //    catch (DbUpdateException ex)
        //    {
        //        ErrorLog.LogError(ex, "ReviseRoadProjectPlanDAL(DbUpdateException ex).DAL");
        //        return ("An Update Error Occurred While Processing Your Request.");
        //    }
        //    catch (Exception ex)
        //    {
        //        ErrorLog.LogError(ex, "ReviseRoadProjectPlanDAL().DAL");
        //        return ("Error Occurred While Processing Request.");
        //    }
        //    finally
        //    {
        //        dbContext.Dispose();
        //    }
        //}

        //public string DeleteBridgeProjectPlanDAL(int IMS_PR_ROAD_CODE, int PLAN_ID)
        //{
        //    dbContext = new PMGSYEntities();
        //    try
        //    {
        //        if (dbContext.PMIS_PROGRESS_MASTER.Where(x => x.PLAN_ID == PLAN_ID && x.IMS_PR_ROAD_CODE == IMS_PR_ROAD_CODE).Any())
        //        {
        //            return "This plan cannot be deleted as it contains actual progress details";
        //        }

        //        using (TransactionScope ts = new TransactionScope())
        //        {

        //            if (dbContext.PMIS_PLAN_DETAILS.Any(m => m.IMS_PR_ROAD_CODE == IMS_PR_ROAD_CODE))
        //            {
        //                List<PMIS_PLAN_DETAILS> lstnewPlanDetails = dbContext.PMIS_PLAN_DETAILS.Where(m => m.IMS_PR_ROAD_CODE == IMS_PR_ROAD_CODE && m.IS_LATEST == "Y").ToList();
        //                foreach (var item in lstnewPlanDetails)
        //                {
        //                    dbContext.PMIS_PLAN_DETAILS.Remove(item);
        //                }
        //            }
        //            //Added on 1 Feb 2021 to resolve DbUpdateException issue.
        //            if (dbContext.PMIS_PROGRESS_DETAILS.Any(m => m.IMS_PR_ROAD_CODE == IMS_PR_ROAD_CODE))
        //            {
        //                List<PMIS_PROGRESS_DETAILS> listProgressDetails = dbContext.PMIS_PROGRESS_DETAILS.Where(m => m.IMS_PR_ROAD_CODE == IMS_PR_ROAD_CODE).ToList();
        //                foreach (var item in listProgressDetails)
        //                {
        //                    dbContext.PMIS_PROGRESS_DETAILS.Remove(item);
        //                }
        //            }
        //            if (dbContext.PMIS_CHAINAGEWISE_COMPLETION_DETAILS.Any(m => m.IMS_PR_ROAD_CODE == IMS_PR_ROAD_CODE))
        //            {
        //                List<PMIS_CHAINAGEWISE_COMPLETION_DETAILS> listChainageDetails = dbContext.PMIS_CHAINAGEWISE_COMPLETION_DETAILS.Where(m => m.IMS_PR_ROAD_CODE == IMS_PR_ROAD_CODE).ToList();
        //                foreach (var item in listChainageDetails)
        //                {
        //                    dbContext.PMIS_CHAINAGEWISE_COMPLETION_DETAILS.Remove(item);
        //                }
        //            }
        //            PMIS_PROGRESS_MASTER progressMaster = dbContext.PMIS_PROGRESS_MASTER.Where(z => z.IMS_PR_ROAD_CODE == IMS_PR_ROAD_CODE).FirstOrDefault();
        //            if (progressMaster != null)
        //            {
        //                //dbContext.PMIS_PROGRESS_MASTER.Remove(progressMaster);
        //                dbContext.Entry(progressMaster).State = EntityState.Deleted;
        //            }

        //            //End of changes
        //            PMIS_PLAN_MASTER planMaster = dbContext.PMIS_PLAN_MASTER.Where(z => z.IMS_PR_ROAD_CODE == IMS_PR_ROAD_CODE & z.IS_LATEST == "Y").FirstOrDefault();
        //            if (planMaster != null)
        //            {
        //                //dbContext.PMIS_PLAN_MASTER.Remove(planMaster);
        //                dbContext.Entry(planMaster).State = EntityState.Deleted;
        //            }

        //            dbContext.SaveChanges();
        //            PMIS_PLAN_MASTER oldmasterplan = dbContext.PMIS_PLAN_MASTER.Where(v => v.IMS_PR_ROAD_CODE == IMS_PR_ROAD_CODE).OrderByDescending(x => x.BASELINE_NO).FirstOrDefault();
        //            if (oldmasterplan != null)
        //            {
        //                oldmasterplan.IS_LATEST = "Y";
        //                dbContext.Entry(oldmasterplan).State = System.Data.Entity.EntityState.Modified;
        //                int? Baseline_No = oldmasterplan.BASELINE_NO;
        //                List<PMIS_PLAN_DETAILS> lstoldPlanDetails = dbContext.PMIS_PLAN_DETAILS.Where(m => m.IMS_PR_ROAD_CODE == IMS_PR_ROAD_CODE & m.BASELINE_NO == Baseline_No).ToList();
        //                foreach (var item in lstoldPlanDetails)
        //                {
        //                    item.IS_LATEST = "Y";
        //                    dbContext.Entry(item).State = System.Data.Entity.EntityState.Modified;
        //                }
        //            }
        //            dbContext.SaveChanges();
        //            ts.Complete();
        //            return string.Empty;
        //        }
        //    }
        //    catch (DbUpdateException ex)
        //    {
        //        ErrorLog.LogError(ex, "DeleteBridgeProjectPlanDAL(DbUpdateException ex).DAL");
        //        return ("An Update Error Occurred While Processing Your Request.");
        //    }
        //    catch (Exception ex)
        //    {
        //        ErrorLog.LogError(ex, "DeleteBridgeProjectPlanDAL().DAL");
        //        return ("Error Occurred While Processing Request.");
        //    }
        //    finally
        //    {
        //        dbContext.Dispose();
        //    }
        //}

        //public string SaveBridgeProjectPlanDAL(IEnumerable<AddPlanPMISViewModelBridge> planData)
        //{
        //    PMGSYEntities dbContext = new PMGSYEntities();
        //    try
        //    {
        //        using (TransactionScope ts = new TransactionScope())
        //        {
        //            PMIS_PLAN_MASTER planMaster = new PMIS_PLAN_MASTER();
        //            int RoadCode = 0;  //planData.ElementAt(0).IMS_PR_ROAD_CODE;
        //            foreach (var plan in planData)
        //            {
        //                RoadCode = plan.IMS_PR_ROAD_CODE;
        //                if (RoadCode > 0)
        //                    break;
        //                else
        //                    continue;
        //            }

        //            planMaster.PLAN_ID = dbContext.PMIS_PLAN_MASTER.Any() ? dbContext.PMIS_PLAN_MASTER.Max(s => s.PLAN_ID) + 1 : 1;
        //            planMaster.IMS_PR_ROAD_CODE = RoadCode; // dbContext.IMS_SANCTIONED_PROJECTS.Where(x => x.IMS_ROAD_NAME == RoadName).Select(x => x.IMS_PR_ROAD_CODE).FirstOrDefault();
        //            planMaster.PLAN_CREATION_DATE = DateTime.Now;
        //            planMaster.USERID = PMGSYSession.Current.UserId;
        //            planMaster.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
        //            planMaster.IS_LATEST = "Y";
        //            planMaster.IS_FINALISED = "N";
        //            planMaster.BASELINE_NO = 1;
        //            dbContext.PMIS_PLAN_MASTER.Add(planMaster);
        //            dbContext.SaveChanges();

        //            long k = dbContext.PMIS_PLAN_DETAILS.Any() ? dbContext.PMIS_PLAN_DETAILS.Max(s => s.DETAILED_ID) : 0;

        //            foreach (var obj in planData)
        //            {
        //                PMIS_PLAN_DETAILS planDetails = new PMIS_PLAN_DETAILS();
        //                planDetails.DETAILED_ID = ++k;      //dbContext.PMIS_PLAN_DETAILS.Any() ? dbContext.PMIS_PLAN_DETAILS.Max(s => s.DETAILED_ID) + 1 : 1;
        //                planDetails.PLAN_ID = planMaster.PLAN_ID;
        //                planDetails.IMS_PR_ROAD_CODE = planMaster.IMS_PR_ROAD_CODE;
        //                //string activity = obj.ACTIVITY_DESC.Replace("\n"," ").Trim();
        //                planDetails.ACTIVITY_ID = dbContext.PMIS_ACTIVITY_MASTER.Where(x => x.ACTIVITY_DESC == obj.ACTIVITY_DESC && x.ROAD_TYPE == "L").Select(x => x.ACTIVITY_ID).FirstOrDefault();
        //                planDetails.QUANTITY = obj.QUANTITY;
        //                planDetails.AGREEMENT_COST = obj.AGREEMENT_COST;
        //                planDetails.PLANNED_START_DATE = obj.PLANNED_START_DATE;//== null ? "-" : obj.PLANNED_START_DATE.Value.To;
        //                planDetails.PLANNED_DURATION = obj.PLANNED_DURATION;
        //                planDetails.PLANNED_COMPLETION_DATE = planDetails.PLANNED_START_DATE != null ? obj.PLANNED_COMPLETION_DATE : null;
        //                planDetails.IS_LATEST = "Y";
        //                planDetails.BASELINE_NO = 1;
        //                planDetails.USERID = PMGSYSession.Current.UserId;
        //                planDetails.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
        //                dbContext.PMIS_PLAN_DETAILS.Add(planDetails);

        //                dbContext.SaveChanges();
        //            }
        //            ts.Complete();
        //            PMISLog("PMIS", ConfigurationManager.AppSettings["PMISLog"].ToString(), "Transaction Completed", "");
        //        }
        //        return string.Empty;
        //    }

        //    catch (DbUpdateException ex)
        //    {
        //        ErrorLog.LogError(ex, "SaveBridgeProjectPlanDAL(DbUpdateException ex).DAL");
        //        return ("An Error Occurred While Processing Your Request.");
        //    }
        //    catch (Exception ex)
        //    {
        //        ErrorLog.LogError(ex, "SaveRoadProjectPlanDAL().DAL");
        //        return ("Error Occurred While Processing Request.");
        //    }
        //    finally
        //    {
        //        dbContext.Dispose();
        //    }
        //}

        //public Array PMISBridgeListDAL(int state, int district, int block, int sanction_year, int batch, int? page, int? rows, string sidx, string sord, out long totalRecords)
        //{

        //    try
        //    {
        //        dbContext = new PMGSYEntities();

        //        var PMISBridgeList = dbContext.USP_PMS_BRIDGE_LIST(state, district, block, sanction_year, batch).ToList<USP_PMS_BRIDGE_LIST_Result>();

        //        var resultList = new List<PMISBridgeDAL>();

        //        foreach (var item in PMISBridgeList)
        //        {
        //            var RoadCode = dbContext.PMIS_PLAN_MASTER.Where(x => x.IMS_PR_ROAD_CODE == item.PrRoadCode).Select(c => c.IMS_PR_ROAD_CODE).FirstOrDefault();
        //            var RevisePlanRoadCode = dbContext.PMIS_PLAN_MASTER.Where(x => x.IMS_PR_ROAD_CODE == item.PrRoadCode & x.IS_LATEST == "Y" & x.IS_FINALISED == "Y").Select(c => c.IMS_PR_ROAD_CODE).FirstOrDefault();
        //            var PlanId = dbContext.PMIS_PLAN_MASTER.Where(x => x.IMS_PR_ROAD_CODE == item.PrRoadCode && x.IS_LATEST == "Y").Select(c => c.PLAN_ID).FirstOrDefault();

        //            // var FinalizeRoadCode = dbContext.PMIS_PLAN_MASTER.Where(z => z.IMS_PR_ROAD_CODE == item.PrRoadCode & z.IS_LATEST == "Y" & z.IS_FINALISED == "N").Select(y => y.IMS_PR_ROAD_CODE).FirstOrDefault();
        //            resultList.Add(new PMISBridgeDAL
        //            {
        //                PlanId = PlanId,
        //                StateName = item.MAST_STATE_NAME,
        //                DistrictName = item.MAST_DISTRICT_NAME,
        //                BlockName = item.BlockName,
        //                PackageName = item.PackageId,
        //                SanctionYear = (item.SanctionYear).ToString() + "-" + (item.SanctionYear + 1).ToString().Substring(2, 2),
        //                SanctionDate = ((item.IMS_SANCTIONED_DATE).Value).ToShortDateString(),
        //                BatchName = item.MAST_BATCH_NAME,
        //                SanctionLength = (item.SanctionLength).ToString(),
        //                AgreementNo = item.TEND_AGREEMENT_NUMBER,
        //                AgreementCost = item.TEND_AGREEMENT_AMOUNT.ToString(),
        //                MordShare = (item.Mord_Share ?? default(decimal)).ToString(),
        //                StateShare = item.State_share.ToString(),
        //                TotalSanctionedCost = item.TOTAL_COST.ToString(),
        //                AgreementStartDate = (item.TEND_AGREEMENT_START_DATE == null) ? "NULL" : ((item.TEND_AGREEMENT_START_DATE).Value).ToShortDateString(),
        //                AgreementEndDate = (item.TEND_AGREEMENT_END_DATE == null) ? "NULL" : ((item.TEND_AGREEMENT_END_DATE).Value).ToShortDateString(),
        //                LSBName = item.LSBName,
        //                IMS_PR_RoadCode = item.PrRoadCode.ToString(),
        //                IsPlanAvaliable = RoadCode == 0 ? item.PrRoadCode.ToString() + "$" : RoadCode.ToString(),
        //                IsFinalize = RoadCode == 0 ? "-" : RevisePlanRoadCode != 0 ? RevisePlanRoadCode.ToString() + "$" : item.PrRoadCode.ToString(),
        //                IsRevisePlan = RevisePlanRoadCode == 0 ? " " : RevisePlanRoadCode.ToString(),
        //                IsActualsAvaliable = RoadCode == 0 ? "" : RoadCode.ToString(),
        //            });
        //        }

        //        totalRecords = PMISBridgeList.Count();

        //        return resultList.Select(BridgeDetails => new
        //        {
        //            cell = new[] {
        //                    BridgeDetails.StateName,
        //                    BridgeDetails.DistrictName,
        //                    BridgeDetails.BlockName,
        //                    BridgeDetails.PackageName,
        //                    BridgeDetails.SanctionYear,
        //                    BridgeDetails.SanctionDate,
        //                    BridgeDetails.BatchName,
        //                    BridgeDetails.SanctionLength,
        //                    BridgeDetails.AgreementNo,
        //                    BridgeDetails.AgreementCost,
        //                    BridgeDetails.MordShare,
        //                    BridgeDetails.StateShare,
        //                    BridgeDetails.TotalSanctionedCost,
        //                    BridgeDetails.LSBName,
        //                    //RoadDetails.IsPlanAvaliable.EndsWith("$") ? "<center><table><tr><td  style='border:none'><span class='ui-icon ui-icon-circle-plus ' title='Add Project Plan' onClick =AddProjectPlan('"+ URLEncrypt.EncryptParameters1(new string[]{"RoadCode =" + RoadDetails.IMS_PR_RoadCode.ToString().Trim(),"StateShare =" + RoadDetails.StateShare.ToString().Trim(),"MordShare =" + RoadDetails.MordShare.ToString().Trim(),"TotalSanctionedDate =" + RoadDetails.TotalSanctionedCost.ToString().Trim()})+"');></span></td></tr></table></center>"//RoadDetails.IsPlanAvaliable 
        //                    //:  RoadDetails.IsFinalize.Contains("$") ? "<center><table><tr><td style='border:none;cursor:pointer'><span class='ui-icon ui-icon-search' title='View Project Plan' onClick =ViewProjectPlan('"+ URLEncrypt.EncryptParameters1(new string[]{"RoadCode =" + RoadDetails.IsPlanAvaliable.ToString().Trim(),"StateShare =" + RoadDetails.StateShare.ToString().Trim(),"MordShare =" + RoadDetails.MordShare.ToString().Trim(),"TotalSanctionedDate =" + RoadDetails.TotalSanctionedCost.ToString().Trim()})+"');></span></td></tr></table></center>"
        //                    //:"<center><table><tr><td  style='border:none;cursor:pointer'><span class='ui-icon ui-icon-pencil' title='Edit Project Plan' onClick =EditProjectPlan('"+ URLEncrypt.EncryptParameters1(new string[]{"RoadCode =" + RoadDetails.IsPlanAvaliable.ToString().Trim(),"StateShare =" + RoadDetails.StateShare.ToString().Trim(),"MordShare =" + RoadDetails.MordShare.ToString().Trim(),"TotalSanctionedDate =" + RoadDetails.TotalSanctionedCost.ToString().Trim()})+"');></span></td><td style='border:none;cursor:pointer'><span class='ui-icon ui-icon-search' title='View Project Plan' onClick ='ViewProjectPlan(\"" + URLEncrypt.EncryptParameters1(new string[]{"RoadCode =" + RoadDetails.IsPlanAvaliable.ToString().Trim(),"StateShare =" + RoadDetails.StateShare.ToString().Trim(),"MordShare =" + RoadDetails.MordShare.ToString().Trim(),"TotalSanctionedDate =" + RoadDetails.TotalSanctionedCost.ToString().Trim()}) + "\");'></span></td><td style='border:none;cursor:pointer'><span class='ui-icon ui-icon-trash' title='Delete Project Plan' onClick ='DeleteProjectPlan(\"" + URLEncrypt.EncryptParameters1(new string[]{"RoadCode =" + RoadDetails.IsPlanAvaliable.ToString().Trim(), "PlanID =" +RoadDetails.PlanId.ToString().Trim()}) + "\");'></span></td></tr></table></center>",//URLEncrypt.EncryptParameters1(new string[]{"RoadCode =" + RoadDetails.IsPlanAvaliable.ToString().Trim()}),
        //                    BridgeDetails.IsPlanAvaliable.EndsWith("$") ? "<center><table><tr><td  style='border:none'><span class='ui-icon ui-icon-circle-plus ' title='Add Project Plan' onClick =AddProjectPlan('"+ URLEncrypt.EncryptParameters1(new string[]{"RoadCode =" + BridgeDetails.IMS_PR_RoadCode.ToString().Trim(),"StateShare =" + BridgeDetails.StateShare.ToString().Trim(),"MordShare =" + BridgeDetails.MordShare.ToString().Trim(),"TotalSanctionedDate =" + BridgeDetails.TotalSanctionedCost.ToString().Trim()})+"');></span></td></tr></table></center>"//RoadDetails.IsPlanAvaliable 
        //                    : PMGSYSession.Current.RoleCode==36 ? "<center><table><tr><td  style='border:none;cursor:pointer'><span class='ui-icon ui-icon-pencil' title='Edit Project Plan' onClick =EditProjectPlan('"+ URLEncrypt.EncryptParameters1(new string[]{"RoadCode =" + BridgeDetails.IsPlanAvaliable.ToString().Trim(),"StateShare =" + BridgeDetails.StateShare.ToString().Trim(),"MordShare =" + BridgeDetails.MordShare.ToString().Trim(),"TotalSanctionedDate =" + BridgeDetails.TotalSanctionedCost.ToString().Trim()})+"');></span></td><td style='border:none;cursor:pointer'><span class='ui-icon ui-icon-search' title='View Project Plan' onClick ='ViewProjectPlan(\"" + URLEncrypt.EncryptParameters1(new string[]{"RoadCode =" + BridgeDetails.IsPlanAvaliable.ToString().Trim(),"StateShare =" + BridgeDetails.StateShare.ToString().Trim(),"MordShare =" + BridgeDetails.MordShare.ToString().Trim(),"TotalSanctionedDate =" + BridgeDetails.TotalSanctionedCost.ToString().Trim()}) + "\");'></span></td><td style='border:none;cursor:pointer'></td></tr></table></center>"
        //                    :BridgeDetails.IsFinalize.Contains("$") ? "<center><table><tr><td style='border:none;cursor:pointer'><span class='ui-icon ui-icon-search' title='View Project Plan' onClick =ViewProjectPlan('"+ URLEncrypt.EncryptParameters1(new string[]{"RoadCode =" + BridgeDetails.IsPlanAvaliable.ToString().Trim(),"StateShare =" + BridgeDetails.StateShare.ToString().Trim(),"MordShare =" + BridgeDetails.MordShare.ToString().Trim(),"TotalSanctionedDate =" + BridgeDetails.TotalSanctionedCost.ToString().Trim()})+"');></span></td></tr></table></center>"
        //                    :"<center><table><tr><td  style='border:none;cursor:pointer'><span class='ui-icon ui-icon-pencil' title='Edit Project Plan' onClick =EditProjectPlan('"+ URLEncrypt.EncryptParameters1(new string[]{"RoadCode =" + BridgeDetails.IsPlanAvaliable.ToString().Trim(),"StateShare =" + BridgeDetails.StateShare.ToString().Trim(),"MordShare =" + BridgeDetails.MordShare.ToString().Trim(),"TotalSanctionedDate =" + BridgeDetails.TotalSanctionedCost.ToString().Trim()})+"');></span></td><td style='border:none;cursor:pointer'><span class='ui-icon ui-icon-search' title='View Project Plan' onClick ='ViewProjectPlan(\"" + URLEncrypt.EncryptParameters1(new string[]{"RoadCode =" + BridgeDetails.IsPlanAvaliable.ToString().Trim(),"StateShare =" + BridgeDetails.StateShare.ToString().Trim(),"MordShare =" + BridgeDetails.MordShare.ToString().Trim(),"TotalSanctionedDate =" + BridgeDetails.TotalSanctionedCost.ToString().Trim()}) + "\");'></span></td><td style='border:none;cursor:pointer'><span class='ui-icon ui-icon-trash' title='Delete Project Plan' onClick ='DeleteProjectPlan(\"" + URLEncrypt.EncryptParameters1(new string[]{"RoadCode =" + BridgeDetails.IsPlanAvaliable.ToString().Trim(), "PlanID =" +BridgeDetails.PlanId.ToString().Trim()}) + "\");'></span></td></tr></table></center>",//URLEncrypt.EncryptParameters1(new string[]{"RoadCode =" + RoadDetails.IsPlanAvaliable.ToString().Trim()}),

        //                    BridgeDetails.IsFinalize == "-" ? "-" :  BridgeDetails.IsFinalize.Contains("$") ? "<center><table><tr><td  style='border:none'><span class='ui-icon  	ui-icon-locked ' title='Finalize Project Plan';'></span></td></tr></table></center>" :
        //                    "<center><table><tr><td  style='border:none'><span class='ui-icon  	ui-icon-unlocked ' title='Finalize Project Plan' onClick ='FinalizeProjectPlan(\"" + URLEncrypt.EncryptParameters1(new string[]{"RoadCode =" + BridgeDetails.IsFinalize.ToString().Trim()})  + "\");'></span></td></tr></table></center>",
        //                    BridgeDetails.IsRevisePlan == " " ? "-" : "<a href='#' title='Click here to Revise Plan Details'  onClick=RevisePlanDetails('"+URLEncrypt.EncryptParameters1(new string[]{"RoadCode =" + BridgeDetails.IsRevisePlan.ToString().Trim(),"StateShare =" + BridgeDetails.StateShare.ToString().Trim(),"MordShare =" + BridgeDetails.MordShare.ToString().Trim(),"TotalSanctionedDate =" + BridgeDetails.TotalSanctionedCost.ToString().Trim()})+"'); return false;'>Revise Plan</a>",
        //                    BridgeDetails.IsActualsAvaliable == ""? "-": "<center><table><tr><td  style='border:none'><span class='ui-icon ui-icon-circle-plus ' title='Add Actuals' onClick =AddActuals1('"+URLEncrypt.EncryptParameters1(new string[]{"RoadCode =" + BridgeDetails.IsActualsAvaliable.ToString().Trim(),"StateShare =" + BridgeDetails.StateShare.ToString().Trim(),"MordShare =" + BridgeDetails.MordShare.ToString().Trim(),"TotalSanctionedDate =" + BridgeDetails.TotalSanctionedCost.ToString().Trim()})+"');></span></td></tr></table></center>",
        //                    BridgeDetails.IsActualsAvaliable == ""? "-": "<center><table><tr><td  style='border:none'><span class='ui-icon ui-icon-circle-plus ' title='Add Chainage wise Details' onClick =AddChainage1('"+URLEncrypt.EncryptParameters1(new string[]{"RoadCode =" + BridgeDetails.IsActualsAvaliable.ToString().Trim()})+"');></span></td></tr></table></center>"


        //                }
        //        }).ToArray();

        //    }
        //    catch (Exception ex)
        //    {
        //        ErrorLog.LogError(ex, "PMISBridgeListDAL().DAL");
        //        totalRecords = 0;
        //        return null;
        //    }
        //    finally
        //    {
        //        dbContext.Dispose();
        //    }
        //}

        #endregion


        #region  FDR Stabilize Data Chainage-Wise Detail 

        public string SubmitFDRChainageDAL(AddFDRStabilizeModel ChainageModel)   // IEnumerable<AddFDRStabilizeModel>  FormCollection
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            try
            {
                //int RoadCode = 0;
                using (TransactionScope ts = new TransactionScope())
                {
                    int RoadCode = Convert.ToInt32(ChainageModel.IMS_PR_ROAD_CODE);
                    if (dbContext.PMIS_FDR_STABILIZE_CHAINAGE_WISE_DETAIL.Where(x => x.IMS_PR_ROAD_CODE == RoadCode).Any())
                    {
                        var chainagedetails = dbContext.PMIS_FDR_STABILIZE_CHAINAGE_WISE_DETAIL.Where(x => x.IMS_PR_ROAD_CODE == RoadCode).ToList();
                        if (chainagedetails != null)
                        {
                            //for (int i = 0; i < Convert.ToInt32(ChainageModel["Sanction_length"]); i++)
                            //{
                            //    var chainagerecord = dbContext.PMIS_FDR_STABILIZE_CHAINAGE_WISE_DETAIL.Where(x => x.IMS_PR_ROAD_CODE == RoadCode && x.CHAINAGE_FROM == i).FirstOrDefault();

                            //    //chainagerecord.F_START_CHAINAGE = chainagedetails["START_CHAINAGE_1[" + i + "]"];
                            //    //chainagerecord.ACT_SUBGRADE = chainagedetails["subgradelist[" + i + "]"];
                            //    //chainagerecord.IS_VALID = "Y";
                            //    //chainagerecord.USERID = PMGSYSession.Current.UserId;
                            //    //chainagerecord.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                            //    //chainagerecord.ENTRY_DATE = chainageEntryDate;
                            //    dbContext.Entry(chainagerecord).State = System.Data.Entity.EntityState.Modified;
                            //}
                            dbContext.SaveChanges();
                        }
                    }
                    else
                    {
                        var compl_id = dbContext.PMIS_FDR_STABILIZE_CHAINAGE_WISE_DETAIL.Any() ? dbContext.PMIS_FDR_STABILIZE_CHAINAGE_WISE_DETAIL.Max(s => s.ID) + 1 : 1;
                        for (int i = 0; i < ChainageModel.ROW_LENGTH; i++)
                        {

                            PMIS_FDR_STABILIZE_CHAINAGE_WISE_DETAIL ChainageDetails = new PMIS_FDR_STABILIZE_CHAINAGE_WISE_DETAIL();

                            ChainageDetails.ID = compl_id;
                            ChainageDetails.CHAINAGE_FROM = i;
                            ChainageDetails.CHAINAGE_TO = i + 1;  // formData["wbmgrading2list[" + i + "]"];
                            ChainageDetails.F_START_CHAINAGE = ChainageModel.AddFDRStabilizeListModelObj[i].START_CHAINAGE_1;
                            ChainageDetails.F_END_CHAINAGE = ChainageModel.AddFDRStabilizeListModelObj[i].END_CHAINAGE_1;
                            if (ChainageModel.AddFDRStabilizeListModelObj[i].Chainage_Date_FirstChainage != null)
                            {
                                ChainageDetails.F_CHAINAGE_DATE = Convert.ToDateTime(ChainageModel.AddFDRStabilizeListModelObj[i].Chainage_Date_FirstChainage);
                            }
                            else
                            {
                                ChainageDetails.F_CHAINAGE_DATE = null;
                            }
                            // ChainageDetails.F_CHAINAGE_DATE = Convert.ToDateTime(ChainageModel.Chainage_Date_FirstChainage[i].Value);
                            ChainageDetails.S_START_CHAINAGE = ChainageModel.AddFDRStabilizeListModelObj[i].START_CHAINAGE_2;
                            ChainageDetails.S_END_CHAINAGE = ChainageModel.AddFDRStabilizeListModelObj[i].END_CHAINAGE_2;
                            if (ChainageModel.AddFDRStabilizeListModelObj[i].Chainage_Date_SecondChainage != null)
                            {
                                ChainageDetails.S_CHAINAGE_DATE = Convert.ToDateTime(ChainageModel.AddFDRStabilizeListModelObj[i].Chainage_Date_SecondChainage);
                            }
                            else
                            {
                                ChainageDetails.S_CHAINAGE_DATE = null;
                            }

                            ChainageDetails.IS_LATEST = "Y";
                            ChainageDetails.USERID = PMGSYSession.Current.UserId;
                            ChainageDetails.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                            ChainageDetails.IMS_PR_ROAD_CODE = RoadCode;
                            ChainageDetails.ENTRY_DATE = DateTime.Now;
                            dbContext.PMIS_FDR_STABILIZE_CHAINAGE_WISE_DETAIL.Add(ChainageDetails);
                            compl_id++;
                            dbContext.SaveChanges();
                        }

                    }
                    ts.Complete();
                    //  return null;
                }


                //using (TransactionScope ts = new TransactionScope())
                //{
                //    var CurrentDate = DateTime.Now.ToString("dd/MM/yyyy");
                //    var RoadCode = 0;//Convert.ToInt32(ChainageModel.Select(x => x.IMS_PR_ROAD_CODE).FirstOrDefault());
                //    var latest_Master = dbContext.PMIS_FDR_STABILIZE_CHAINAGE_WISE_DETAIL.Where(x => x.IMS_PR_ROAD_CODE == RoadCode & x.IS_LATEST == "Y").ToList();
                //    if (latest_Master != null)
                //    {
                //        foreach (var lstInsertFDR in latest_Master)
                //        {
                //            lstInsertFDR.IS_LATEST = "N";
                //            dbContext.Entry(lstInsertFDR).State = System.Data.Entity.EntityState.Modified;
                //            dbContext.SaveChanges();
                //        }
                //    }
                //    foreach (var item in ChainageModel)
                //    {                  
                //        PMIS_FDR_STABILIZE_CHAINAGE_WISE_DETAIL OBJ = new PMIS_FDR_STABILIZE_CHAINAGE_WISE_DETAIL();
                //        OBJ.ID = dbContext.PMIS_FDR_STABILIZE_CHAINAGE_WISE_DETAIL.Any() ? dbContext.PMIS_FDR_STABILIZE_CHAINAGE_WISE_DETAIL.Max(s => s.ID) + 1 : 1;
                //        OBJ.IMS_PR_ROAD_CODE = 0;// item.IMS_PR_ROAD_CODE;
                //        //OBJ.CHAINAGE_FROM = item.CHAINAGE_FROM;
                //        //OBJ.CHAINAGE_TO = item.CHAINAGE_TO;
                //        //OBJ.DATE = item.CHAINAGE_DATE;
                //        OBJ.ENTRY_DATE = Convert.ToDateTime(CurrentDate.Split(' ')[0]);
                //        OBJ.IS_LATEST = "Y";
                //        OBJ.USERID = PMGSYSession.Current.UserId;
                //        OBJ.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                //        dbContext.PMIS_FDR_STABILIZE_CHAINAGE_WISE_DETAIL.Add(OBJ);
                //        dbContext.SaveChanges();
                //    }
                //    ts.Complete();
                //}
                return string.Empty;
            }
            catch (DbEntityValidationException e)
            {
                foreach (var eve in e.EntityValidationErrors)
                {
                    using (System.IO.StreamWriter sw = System.IO.File.AppendText(System.IO.Path.Combine("D:\\ErrorLog" + System.DateTime.Now.Date.ToString("dd_MM_yyyy") + ".txt", "")))
                    {
                        sw.WriteLine("Date :" + DateTime.Now.ToString());
                        sw.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                        eve.Entry.Entity.GetType().Name, eve.Entry.State);
                        sw.WriteLine("---------------------------------------------------------------------------------------");
                        sw.Close();
                    }

                    foreach (var ve in eve.ValidationErrors)
                    {
                        using (System.IO.StreamWriter sw = System.IO.File.AppendText(System.IO.Path.Combine("D:\\ErrorLog" + System.DateTime.Now.Date.ToString("dd_MM_yyyy") + ".txt", "")))
                        {
                            sw.WriteLine("Date :" + DateTime.Now.ToString());
                            sw.WriteLine("- Property: \"{0}\", Error: \"{1}\"",
                            ve.PropertyName, ve.ErrorMessage);
                            sw.WriteLine("---------------------------------------------------------------------------------------");
                            sw.Close();
                        }
                    }
                }
                return ("An Error Occurred While Processing Your Request.");
                //throw;
            }
            catch (DbUpdateException ex)
            {
                ErrorLog.LogError(ex, "SubmitFDRChainageDAL(DbUpdateException ex).DAL");
                return ("An Error Occurred While Processing Your Request.");
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "SubmitFDRChainageDAL().DAL");
                return ("Error Occurred While Processing Request.");
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        public string UpdateFDRChainageDAL(AddFDRStabilizeModel ChainageModel)   // IEnumerable<AddFDRStabilizeModel>  FormCollection
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            try
            {
                //int RoadCode = 0;

                using (TransactionScope ts = new TransactionScope())
                {
                    int RoadCode = Convert.ToInt32(ChainageModel.IMS_PR_ROAD_CODE);
                    if (dbContext.PMIS_FDR_STABILIZE_CHAINAGE_WISE_DETAIL.Where(x => x.IMS_PR_ROAD_CODE == RoadCode).Any())
                    {
                        var model = dbContext.PMIS_FDR_STABILIZE_CHAINAGE_WISE_DETAIL.Where(x => x.IMS_PR_ROAD_CODE == RoadCode && x.IS_LATEST == "Y").FirstOrDefault();
                        if (model.ENTRY_DATE.Date == DateTime.Now.Date)
                        {
                            var chainagedetails = dbContext.PMIS_FDR_STABILIZE_CHAINAGE_WISE_DETAIL.Where(x => x.IMS_PR_ROAD_CODE == RoadCode).ToList();
                            if (chainagedetails != null)
                            {
                                for (int i = 0; i < Convert.ToInt32(ChainageModel.Sanction_length); i++)
                                {
                                    var ChainageDetails = dbContext.PMIS_FDR_STABILIZE_CHAINAGE_WISE_DETAIL.Where(x => x.IMS_PR_ROAD_CODE == RoadCode && x.CHAINAGE_FROM == i).FirstOrDefault();

                                    ChainageDetails.CHAINAGE_FROM = i;
                                    ChainageDetails.CHAINAGE_TO = i + 1;  // formData["wbmgrading2list[" + i + "]"];
                                    ChainageDetails.F_START_CHAINAGE = ChainageModel.AddFDRStabilizeListModelObj[i].START_CHAINAGE_1;
                                    ChainageDetails.F_END_CHAINAGE = ChainageModel.AddFDRStabilizeListModelObj[i].END_CHAINAGE_1;
                                    if (ChainageModel.AddFDRStabilizeListModelObj[i].Chainage_Date_FirstChainage != null)
                                    {
                                        ChainageDetails.F_CHAINAGE_DATE = Convert.ToDateTime(ChainageModel.AddFDRStabilizeListModelObj[i].Chainage_Date_FirstChainage);
                                    }
                                    else
                                    {
                                        ChainageDetails.F_CHAINAGE_DATE = null;
                                    }
                                    // ChainageDetails.F_CHAINAGE_DATE = Convert.ToDateTime(ChainageModel.Chainage_Date_FirstChainage[i].Value);
                                    ChainageDetails.S_START_CHAINAGE = ChainageModel.AddFDRStabilizeListModelObj[i].START_CHAINAGE_2;
                                    ChainageDetails.S_END_CHAINAGE = ChainageModel.AddFDRStabilizeListModelObj[i].END_CHAINAGE_2;
                                    if (ChainageModel.AddFDRStabilizeListModelObj[i].Chainage_Date_SecondChainage != null)
                                    {
                                        ChainageDetails.S_CHAINAGE_DATE = Convert.ToDateTime(ChainageModel.AddFDRStabilizeListModelObj[i].Chainage_Date_SecondChainage);
                                    }
                                    else
                                    {
                                        ChainageDetails.S_CHAINAGE_DATE = null;
                                    }

                                    ChainageDetails.IS_LATEST = "Y";
                                    dbContext.Entry(ChainageDetails).State = System.Data.Entity.EntityState.Modified;
                                }
                                dbContext.SaveChanges();
                            }
                        }
                        else
                        {
                            var ChainageDetails = dbContext.PMIS_FDR_STABILIZE_CHAINAGE_WISE_DETAIL.Where(x => x.IMS_PR_ROAD_CODE == RoadCode).ToList();

                            foreach (var item in ChainageDetails)
                            {
                                var ChainageDetailsUpdate = dbContext.PMIS_FDR_STABILIZE_CHAINAGE_WISE_DETAIL.Where(x => x.IMS_PR_ROAD_CODE == RoadCode && x.ID == item.ID).FirstOrDefault();

                                ChainageDetailsUpdate.IS_LATEST = "N";

                                dbContext.Entry(ChainageDetailsUpdate).State = System.Data.Entity.EntityState.Modified;
                                dbContext.SaveChanges();
                            }
                            var compl_id = dbContext.PMIS_FDR_STABILIZE_CHAINAGE_WISE_DETAIL.Any() ? dbContext.PMIS_FDR_STABILIZE_CHAINAGE_WISE_DETAIL.Max(s => s.ID) + 1 : 1;
                            for (int i = 0; i < ChainageModel.ROW_LENGTH; i++)
                            {

                                PMIS_FDR_STABILIZE_CHAINAGE_WISE_DETAIL ChainageDetailsInsert = new PMIS_FDR_STABILIZE_CHAINAGE_WISE_DETAIL();

                                ChainageDetailsInsert.ID = compl_id;
                                ChainageDetailsInsert.CHAINAGE_FROM = i;
                                ChainageDetailsInsert.CHAINAGE_TO = i + 1;  // formData["wbmgrading2list[" + i + "]"];
                                ChainageDetailsInsert.F_START_CHAINAGE = ChainageModel.AddFDRStabilizeListModelObj[i].START_CHAINAGE_1;
                                ChainageDetailsInsert.F_END_CHAINAGE = ChainageModel.AddFDRStabilizeListModelObj[i].END_CHAINAGE_1;
                                if (ChainageModel.AddFDRStabilizeListModelObj[i].Chainage_Date_FirstChainage != null)
                                {
                                    ChainageDetailsInsert.F_CHAINAGE_DATE = Convert.ToDateTime(ChainageModel.AddFDRStabilizeListModelObj[i].Chainage_Date_FirstChainage);
                                }
                                else
                                {
                                    ChainageDetailsInsert.F_CHAINAGE_DATE = null;
                                }
                                // ChainageDetailsInsert.F_CHAINAGE_DATE = Convert.ToDateTime(ChainageModel.Chainage_Date_FirstChainage[i].Value);
                                ChainageDetailsInsert.S_START_CHAINAGE = ChainageModel.AddFDRStabilizeListModelObj[i].START_CHAINAGE_2;
                                ChainageDetailsInsert.S_END_CHAINAGE = ChainageModel.AddFDRStabilizeListModelObj[i].END_CHAINAGE_2;
                                if (ChainageModel.AddFDRStabilizeListModelObj[i].Chainage_Date_SecondChainage != null)
                                {
                                    ChainageDetailsInsert.S_CHAINAGE_DATE = Convert.ToDateTime(ChainageModel.AddFDRStabilizeListModelObj[i].Chainage_Date_SecondChainage);
                                }
                                else
                                {
                                    ChainageDetailsInsert.S_CHAINAGE_DATE = null;
                                }

                                ChainageDetailsInsert.IS_LATEST = "Y";
                                ChainageDetailsInsert.USERID = PMGSYSession.Current.UserId;
                                ChainageDetailsInsert.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                                ChainageDetailsInsert.IMS_PR_ROAD_CODE = RoadCode;
                                ChainageDetailsInsert.ENTRY_DATE = DateTime.Now;
                                dbContext.PMIS_FDR_STABILIZE_CHAINAGE_WISE_DETAIL.Add(ChainageDetailsInsert);
                                compl_id++;
                                dbContext.SaveChanges();
                            }

                        }

                    }
                    else
                    {

                        var compl_id = dbContext.PMIS_FDR_STABILIZE_CHAINAGE_WISE_DETAIL.Any() ? dbContext.PMIS_FDR_STABILIZE_CHAINAGE_WISE_DETAIL.Max(s => s.ID) + 1 : 1;
                        for (int i = 0; i < Convert.ToInt32(ChainageModel.Sanction_length); i++)
                        {

                            PMIS_FDR_STABILIZE_CHAINAGE_WISE_DETAIL ChainageDetails = new PMIS_FDR_STABILIZE_CHAINAGE_WISE_DETAIL();

                            ChainageDetails.ID = compl_id;
                            ChainageDetails.CHAINAGE_FROM = i;
                            ChainageDetails.CHAINAGE_TO = i + 1;  // formData["wbmgrading2list[" + i + "]"];
                            ChainageDetails.F_START_CHAINAGE = ChainageModel.AddFDRStabilizeListModelObj[i].START_CHAINAGE_1;
                            ChainageDetails.F_END_CHAINAGE = ChainageModel.AddFDRStabilizeListModelObj[i].END_CHAINAGE_1;
                            if (ChainageModel.AddFDRStabilizeListModelObj[i].Chainage_Date_FirstChainage != null)
                            {
                                ChainageDetails.F_CHAINAGE_DATE = Convert.ToDateTime(ChainageModel.AddFDRStabilizeListModelObj[i].Chainage_Date_FirstChainage);
                            }
                            else
                            {
                                ChainageDetails.F_CHAINAGE_DATE = null;
                            }
                            // ChainageDetails.F_CHAINAGE_DATE = Convert.ToDateTime(ChainageModel.Chainage_Date_FirstChainage[i].Value);
                            ChainageDetails.S_START_CHAINAGE = ChainageModel.AddFDRStabilizeListModelObj[i].START_CHAINAGE_2;
                            ChainageDetails.S_END_CHAINAGE = ChainageModel.AddFDRStabilizeListModelObj[i].END_CHAINAGE_2;
                            if (ChainageModel.AddFDRStabilizeListModelObj[i].Chainage_Date_SecondChainage != null)
                            {
                                ChainageDetails.S_CHAINAGE_DATE = Convert.ToDateTime(ChainageModel.AddFDRStabilizeListModelObj[i].Chainage_Date_SecondChainage);
                            }
                            else
                            {
                                ChainageDetails.S_CHAINAGE_DATE = null;
                            }

                            ChainageDetails.IS_LATEST = "Y";
                            ChainageDetails.USERID = PMGSYSession.Current.UserId;
                            ChainageDetails.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                            ChainageDetails.IMS_PR_ROAD_CODE = RoadCode;
                            ChainageDetails.ENTRY_DATE = DateTime.Now;
                            dbContext.PMIS_FDR_STABILIZE_CHAINAGE_WISE_DETAIL.Add(ChainageDetails);
                            compl_id++;
                            dbContext.SaveChanges();
                        }



                    }
                    ts.Complete();
                    //  return null;
                }


                //using (TransactionScope ts = new TransactionScope())
                //{
                //    var CurrentDate = DateTime.Now.ToString("dd/MM/yyyy");
                //    var RoadCode = 0;//Convert.ToInt32(ChainageModel.Select(x => x.IMS_PR_ROAD_CODE).FirstOrDefault());
                //    var latest_Master = dbContext.PMIS_FDR_STABILIZE_CHAINAGE_WISE_DETAIL.Where(x => x.IMS_PR_ROAD_CODE == RoadCode & x.IS_LATEST == "Y").ToList();
                //    if (latest_Master != null)
                //    {
                //        foreach (var lstInsertFDR in latest_Master)
                //        {
                //            lstInsertFDR.IS_LATEST = "N";
                //            dbContext.Entry(lstInsertFDR).State = System.Data.Entity.EntityState.Modified;
                //            dbContext.SaveChanges();
                //        }
                //    }
                //    foreach (var item in ChainageModel)
                //    {                  
                //        PMIS_FDR_STABILIZE_CHAINAGE_WISE_DETAIL OBJ = new PMIS_FDR_STABILIZE_CHAINAGE_WISE_DETAIL();
                //        OBJ.ID = dbContext.PMIS_FDR_STABILIZE_CHAINAGE_WISE_DETAIL.Any() ? dbContext.PMIS_FDR_STABILIZE_CHAINAGE_WISE_DETAIL.Max(s => s.ID) + 1 : 1;
                //        OBJ.IMS_PR_ROAD_CODE = 0;// item.IMS_PR_ROAD_CODE;
                //        //OBJ.CHAINAGE_FROM = item.CHAINAGE_FROM;
                //        //OBJ.CHAINAGE_TO = item.CHAINAGE_TO;
                //        //OBJ.DATE = item.CHAINAGE_DATE;
                //        OBJ.ENTRY_DATE = Convert.ToDateTime(CurrentDate.Split(' ')[0]);
                //        OBJ.IS_LATEST = "Y";
                //        OBJ.USERID = PMGSYSession.Current.UserId;
                //        OBJ.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                //        dbContext.PMIS_FDR_STABILIZE_CHAINAGE_WISE_DETAIL.Add(OBJ);
                //        dbContext.SaveChanges();
                //    }
                //    ts.Complete();
                //}
                return string.Empty;
            }
            catch (DbEntityValidationException e)
            {
                foreach (var eve in e.EntityValidationErrors)
                {
                    using (System.IO.StreamWriter sw = System.IO.File.AppendText(System.IO.Path.Combine("D:\\ErrorLog" + System.DateTime.Now.Date.ToString("dd_MM_yyyy") + ".txt", "")))
                    {
                        sw.WriteLine("Date :" + DateTime.Now.ToString());
                        sw.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                        eve.Entry.Entity.GetType().Name, eve.Entry.State);
                        sw.WriteLine("---------------------------------------------------------------------------------------");
                        sw.Close();
                    }

                    foreach (var ve in eve.ValidationErrors)
                    {
                        using (System.IO.StreamWriter sw = System.IO.File.AppendText(System.IO.Path.Combine("D:\\ErrorLog" + System.DateTime.Now.Date.ToString("dd_MM_yyyy") + ".txt", "")))
                        {
                            sw.WriteLine("Date :" + DateTime.Now.ToString());
                            sw.WriteLine("- Property: \"{0}\", Error: \"{1}\"",
                            ve.PropertyName, ve.ErrorMessage);
                            sw.WriteLine("---------------------------------------------------------------------------------------");
                            sw.Close();
                        }
                    }
                }
                return ("An Error Occurred While Processing Your Request.");
                //throw;
            }
            catch (DbUpdateException ex)
            {
                ErrorLog.LogError(ex, "SubmitFDRChainageDAL(DbUpdateException ex).DAL");
                return ("An Error Occurred While Processing Your Request.");
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "SubmitFDRChainageDAL().DAL");
                return ("Error Occurred While Processing Request.");
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        public string UpdateFDRChainageDAL1(AddFDRStabilizeModel ChainageModel)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            try
            {
                using (TransactionScope ts = new TransactionScope())
                {
                    var CurrentDate = DateTime.Now.ToString("dd/MM/yyyy");
                    //var RoadCode = Convert.ToInt32(ChainageModel.Select(x => x.IMS_PR_ROAD_CODE).FirstOrDefault());


                    //    if (dbContext.PMIS_FDR_STABILIZE_CHAINAGE_WISE_DETAIL.Where(x => x.IMS_PR_ROAD_CODE == RoadCode).Any())
                    //    {
                    //        int i = 0;

                    //        foreach (var obj in ChainageModel )
                    //        {                        
                    //            var latest_Master = dbContext.PMIS_FDR_STABILIZE_CHAINAGE_WISE_DETAIL.Where(x => x.IMS_PR_ROAD_CODE == RoadCode & x.IS_LATEST == "Y").ToList();
                    //            if (latest_Master != null && RoadCode != 0)
                    //            {
                    //                int j = 0;
                    //                foreach (var itemData in latest_Master)
                    //                {
                    //                        if (i == j)
                    //                        {
                    //                            //itemData.CHAINAGE_FROM = obj.CHAINAGE_FROM;
                    //                            //itemData.CHAINAGE_TO = obj.CHAINAGE_TO;
                    //                            //itemData.DATE = obj.CHAINAGE_DATE;
                    //                            itemData.ENTRY_DATE = Convert.ToDateTime(CurrentDate.Split(' ')[0]);
                    //                            itemData.IS_LATEST = "Y";
                    //                            itemData.USERID = PMGSYSession.Current.UserId;
                    //                            itemData.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                    //                            dbContext.Entry(itemData).State = System.Data.Entity.EntityState.Modified;
                    //                        }
                    //                      j++;
                    //                }                                                             
                    //            }
                    //             i++;
                    //        }
                    //         dbContext.SaveChanges();                         
                    //    }
                    //    else
                    //    {
                    //        var latest_Master = dbContext.PMIS_FDR_STABILIZE_CHAINAGE_WISE_DETAIL.Where(x => x.IMS_PR_ROAD_CODE == RoadCode & x.IS_LATEST == "Y").ToList();
                    //        if (latest_Master != null)
                    //        {
                    //            foreach (var lstInsertFDR in latest_Master)
                    //            {
                    //                lstInsertFDR.IS_LATEST = "N";
                    //                dbContext.Entry(lstInsertFDR).State = System.Data.Entity.EntityState.Modified;
                    //                dbContext.SaveChanges();
                    //            }
                    //        }
                    //        foreach (var item in ChainageModel)
                    //        {
                    //            PMIS_FDR_STABILIZE_CHAINAGE_WISE_DETAIL OBJ = new PMIS_FDR_STABILIZE_CHAINAGE_WISE_DETAIL();
                    //            OBJ.ID = dbContext.PMIS_FDR_STABILIZE_CHAINAGE_WISE_DETAIL.Any() ? dbContext.PMIS_FDR_STABILIZE_CHAINAGE_WISE_DETAIL.Max(s => s.ID) + 1 : 1;
                    //            OBJ.IMS_PR_ROAD_CODE = item.IMS_PR_ROAD_CODE;
                    //            //OBJ.CHAINAGE_FROM = item.CHAINAGE_FROM;
                    //            //OBJ.CHAINAGE_TO = item.CHAINAGE_TO;
                    //            //OBJ.DATE = item.CHAINAGE_DATE;
                    //            OBJ.ENTRY_DATE = Convert.ToDateTime(CurrentDate.Split(' ')[0]);
                    //            OBJ.IS_LATEST = "Y";
                    //            OBJ.USERID = PMGSYSession.Current.UserId;
                    //            OBJ.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                    //            dbContext.PMIS_FDR_STABILIZE_CHAINAGE_WISE_DETAIL.Add(OBJ);
                    //            dbContext.SaveChanges();
                    //        }
                    //    }

                    ts.Complete();
                }

                return string.Empty;
            }
            catch (DbEntityValidationException e)
            {
                foreach (var eve in e.EntityValidationErrors)
                {
                    using (System.IO.StreamWriter sw = System.IO.File.AppendText(System.IO.Path.Combine("D:\\ErrorLog" + System.DateTime.Now.Date.ToString("dd_MM_yyyy") + ".txt", "")))
                    {
                        sw.WriteLine("Date :" + DateTime.Now.ToString());
                        sw.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                        eve.Entry.Entity.GetType().Name, eve.Entry.State);
                        sw.WriteLine("---------------------------------------------------------------------------------------");
                        sw.Close();
                    }

                    foreach (var ve in eve.ValidationErrors)
                    {
                        using (System.IO.StreamWriter sw = System.IO.File.AppendText(System.IO.Path.Combine("D:\\ErrorLog" + System.DateTime.Now.Date.ToString("dd_MM_yyyy") + ".txt", "")))
                        {
                            sw.WriteLine("Date :" + DateTime.Now.ToString());
                            sw.WriteLine("- Property: \"{0}\", Error: \"{1}\"",
                            ve.PropertyName, ve.ErrorMessage);
                            sw.WriteLine("---------------------------------------------------------------------------------------");
                            sw.Close();
                        }
                    }
                }
                return ("An Error Occurred While Processing Your Request.");
                //throw;
            }
            catch (DbUpdateException ex)
            {
                ErrorLog.LogError(ex, "SubmitFDRChainageDAL(DbUpdateException ex).DAL");
                return ("An Error Occurred While Processing Your Request.");
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "SubmitFDRChainageDAL().DAL");
                return ("Error Occurred While Processing Request.");
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        public Array PMISFDRStabListDAL(int RoadCode, int? page, int? rows, string sidx, string sord, out long totalRecords)
        {

            try
            {
                dbContext = new PMGSYEntities();
                int ImsRoadCode = Convert.ToInt32(RoadCode);
                //long totalrecords ;
                var Stabilizelstdata = dbContext.PMIS_FDR_STABILIZE_CHAINAGE_WISE_DETAIL.Where(x => x.IMS_PR_ROAD_CODE == ImsRoadCode).OrderByDescending(x => x.ENTRY_DATE).ToList();
                totalRecords = Stabilizelstdata.Count;
                List<AddFDRStabilizeModel> lstFDRStabilize = new List<AddFDRStabilizeModel>();

                foreach (var RemoveItemNull in Stabilizelstdata)
                {
                    //if (RemoveItemNull.CHAINAGE_FROM == null && RemoveItemNull.CHAINAGE_TO == null && RemoveItemNull.DATE == null)
                    //{
                    //    //Stabilizelstdata.Remove(RemoveItemNull);
                    //    //continue;
                    //}
                    //else
                    //{
                    //    AddFDRStabilizeModel obj = new AddFDRStabilizeModel();
                    //    obj.IMS_PR_ROAD_CODE = RemoveItemNull.IMS_PR_ROAD_CODE;
                    //    //obj.CHAINAGE_FROM = RemoveItemNull.CHAINAGE_FROM;
                    //    //obj.CHAINAGE_TO = RemoveItemNull.CHAINAGE_TO;
                    //    //obj.CHAINAGE_DATE = RemoveItemNull.DATE;
                    //    obj.Entry_Date = RemoveItemNull.ENTRY_DATE;
                    //    lstFDRStabilize.Add(obj);
                    //}
                }
                //foreach (var item in Stabilizelstdata)
                //{

                //}
                return lstFDRStabilize.Select(RoadDetails => new
                {
                    cell = new[] {
                        RoadDetails.IMS_PR_ROAD_CODE.ToString(),
                        RoadDetails.Entry_Date.ToString().Split(' ')[0],
                        //RoadDetails.CHAINAGE_FROM.ToString(),
                        //RoadDetails.CHAINAGE_TO.ToString(),
                        //RoadDetails.CHAINAGE_DATE.ToString().Split(' ')[0]
                    }
                }).ToArray();
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "PMISFDRStabListDAL().DAL");
                totalRecords = 0;
                return null;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        #endregion

        #region Added By Hrishikesh To Add "Trail Strech For FDR" --05-06-2023 ---start

        #region Commented "Working" SaveAddTrailStrechForFDRDAL() , DeleteTrialStrechFdrDAL() 
        /*        public bool SaveAddTrailStrechForFDRDAL(FormCollection formCollection, HttpPostedFileBase file1, HttpPostedFileBase file2, ref string message)
                {
                    dbContext = new PMGSYEntities();
                    CommonFunctions commObj = new CommonFunctions();
                    string fileName1 = string.Empty;
                    string fileName2 = string.Empty;
                    string uploaded_File1 = string.Empty;
                    string uploaded_File2 = string.Empty;
                    var multiselectlistArr = formCollection["ADDITIVE_ID_List_Arr"].ToString().Trim().Split('$'); //chek if empty or undefined 
                    var jmfId = Convert.ToInt32(formCollection["JMF_ID"].ToString().Trim());


                    try
                    {
                        if ((file1.FileName != "" && file1.ContentLength != 0) || (file2.FileName != "" && file2.ContentLength != 0))
                        {
                            int fileSizeLimit = Convert.ToInt32(ConfigurationManager.AppSettings["PMIS_TRIAL_STRECH_FOR_FDR_FILE_SIZE"]) * 1024 * 1024; //byte to kb to Mb
                            if ((file1.ContentLength < fileSizeLimit) || (file2.ContentLength < fileSizeLimit))
                            {
                                if ((file1 != null && file1.ContentLength > 0 && Path.GetExtension(file1.FileName).ToLower() == ".pdf") || (file2 != null && file2.ContentLength > 0 && Path.GetExtension(file2.FileName).ToLower() == ".pdf"))
                                {
                                    var trialStrechId = dbContext.PMIS_TRIAL_STRETCH_FDR_DETAIL.Max(cp => (Int32?)cp.TRIAL_STRETCH_ID) == null ? 1 : (Int32)dbContext.PMIS_TRIAL_STRETCH_FDR_DETAIL.Max(cp => (Int32?)cp.TRIAL_STRETCH_ID) + 1;
                                    //var trialStrechId = dbContext.PMIS_TRIAL_STRETCH_FDR_DETAIL.Max(x => x.TRIAL_STRETCH_ID);

                                    if (file1 != null && file1.ContentLength > 0 && Path.GetExtension(file1.FileName).ToLower() == ".pdf")
                                    {
                                        fileName1 = trialStrechId + "_PMIS_TestResult_TrailSection_" + System.DateTime.Now.ToString("dd_MM_yyyy_HHmmss") + ".pdf"; //add month and year 
                                    }
                                    if (file2 != null && file2.ContentLength > 0 && Path.GetExtension(file2.FileName).ToLower() == ".pdf")
                                    {
                                        fileName2 = trialStrechId + "_PMIS_JMF_Report_" + System.DateTime.Now.ToString("dd_MM_yyyy_HHmmss") + ".pdf"; //add month and year 
                                    }


                                    using (var scope = new TransactionScope())
                                    {
                                        String Uploaded_Path = ConfigurationManager.AppSettings["PMIS_TRIAL_STRECH_FOR_FDR_UPLOAD_PATH"] + "\\" + DateTime.Now.Year + "\\" + DateTime.Now.Month;
                                        if (!Directory.Exists(Uploaded_Path))
                                        {
                                            Directory.CreateDirectory(Uploaded_Path);
                                        }

                                        if (file1 != null && file1.ContentLength > 0 && Path.GetExtension(file1.FileName).ToLower() == ".pdf")
                                        {
                                            uploaded_File1 = Path.Combine(Uploaded_Path, fileName1);
                                            file1.SaveAs(uploaded_File1);
                                        }
                                        if (file2 != null && file2.ContentLength > 0 && Path.GetExtension(file2.FileName).ToLower() == ".pdf")
                                        {
                                            uploaded_File2 = Path.Combine(Uploaded_Path, fileName2);
                                            file2.SaveAs(uploaded_File2);
                                        }

                                        *//*uploaded_File = Path.Combine(Uploaded_Path, fileName);
                                          file1.SaveAs(uploaded_File);*//*

                                        //Save Data Operation --start
                                        var trialStrechIdPK = dbContext.PMIS_TRIAL_STRETCH_FDR_DETAIL.Max(cp => (Int32?)cp.TRIAL_STRETCH_ID) == null ? 1 : (Int32)dbContext.PMIS_TRIAL_STRETCH_FDR_DETAIL.Max(cp => (Int32?)cp.TRIAL_STRETCH_ID) + 1;

                                        PMIS_TRIAL_STRETCH_FDR_DETAIL pmisTrailStrechFdrDetailsObj = new PMIS_TRIAL_STRETCH_FDR_DETAIL();

                                        pmisTrailStrechFdrDetailsObj.TRIAL_STRETCH_ID = trialStrechId;
                                        pmisTrailStrechFdrDetailsObj.IMS_PR_ROAD_CODE = Convert.ToInt32(formCollection["IMS_PR_ROAD_CODE"].ToString().Trim());
                                        pmisTrailStrechFdrDetailsObj.JMF_ID = dbContext.MASTER_JMF.Where(x => x.JMF_ID == jmfId).Select(y => y.JMF_ID).FirstOrDefault();// ----------------have to fetcj from master_jmf tbl
                                        pmisTrailStrechFdrDetailsObj.PERC_CEMENT_CONT = Convert.ToDecimal(formCollection["PERC_CEMENT_CONT"].ToString().Trim());
                                        pmisTrailStrechFdrDetailsObj.PERC_ADDITIVE_CONT = Convert.ToDecimal(formCollection["PERC_ADDITIVE_CONT"].ToString().Trim());
                                        pmisTrailStrechFdrDetailsObj.CEMENT_TYPE = formCollection["CEMENT_TYPE"].ToString().Trim();
                                        pmisTrailStrechFdrDetailsObj.CEMENT_GRADE = formCollection["CEMENT_GRADE"].ToString().Trim();
                                        pmisTrailStrechFdrDetailsObj.AVG_THICK_CRUST = Convert.ToDecimal(formCollection["AVG_THICK_CRUST"].ToString().Trim());
                                        pmisTrailStrechFdrDetailsObj.PERC_PLASTICITY_RECLAM_SOIL = Convert.ToDecimal(formCollection["PERC_PLASTICITY_RECLAM_SOIL"].ToString().Trim());
                                        pmisTrailStrechFdrDetailsObj.IS_GRAD_MATERIAL_SPEC_LIMIT = formCollection["IS_GRAD_MATERIAL_SPEC_LIMIT"].ToString().Trim();
                                        pmisTrailStrechFdrDetailsObj.CRACK_RELIEF_LAYER = formCollection["CRACK_RELIEF_LAYER"].ToString().Trim();
                                        pmisTrailStrechFdrDetailsObj.STRETCH_LENGTH = Convert.ToDecimal(formCollection["STRETCH_LENGTH"].ToString().Trim());
                                        pmisTrailStrechFdrDetailsObj.STRETCH_CONSTR_DATE = commObj.GetStringToDateTime(formCollection["STRETCH_CONSTR_DATE"].ToString().Trim());
                                        pmisTrailStrechFdrDetailsObj.UCS_TEST_CONDUCTED = formCollection["UCS_TEST_CONDUCTED"].ToString().Trim();
                                        pmisTrailStrechFdrDetailsObj.UCS_7DAY_VALUE = formCollection["UCS_7DAY_VALUE"].ToString().Trim();   //--nullable val 
                                        pmisTrailStrechFdrDetailsObj.UCS_28DAY_VALUE = formCollection["UCS_28DAY_VALUE"].ToString().Trim();     //--nullable val 

                                        if (file1 != null && file1.ContentLength > 0 && Path.GetExtension(file1.FileName).ToLower() == ".pdf")
                                        {
                                            pmisTrailStrechFdrDetailsObj.TEST_RESULT_FILE_NAME = fileName1 == "" ? "" : fileName1;  //--nullable val 
                                            pmisTrailStrechFdrDetailsObj.TEST_RESULT_FILE_PATH = Uploaded_Path == "" ? "" : Uploaded_Path;  //--nullable val 
                                        }
                                        else
                                        {
                                            pmisTrailStrechFdrDetailsObj.TEST_RESULT_FILE_NAME = "";  //--nullable val 
                                            pmisTrailStrechFdrDetailsObj.TEST_RESULT_FILE_PATH = "";  //--nullable val
                                        }
                                        if (file2 != null && file2.ContentLength > 0 && Path.GetExtension(file2.FileName).ToLower() == ".pdf")
                                        {
                                            pmisTrailStrechFdrDetailsObj.JMF_FILE_NAME = fileName2 == "" ? "" : fileName2;  //--nullable val 
                                            pmisTrailStrechFdrDetailsObj.JMF_FILE_PATH = Uploaded_Path == "" ? "" : Uploaded_Path;  //--nullable val 
                                        }
                                        else
                                        {

                                            pmisTrailStrechFdrDetailsObj.JMF_FILE_NAME = "";  //--nullable val 
                                            pmisTrailStrechFdrDetailsObj.JMF_FILE_PATH = "";  //--nullable val 
                                        }
                                        pmisTrailStrechFdrDetailsObj.PERC_INDEX_SUBGRADE_SOIL = Convert.ToDecimal(formCollection["PERC_INDEX_SUBGRADE_SOIL"].ToString().Trim());

                                        if (formCollection["SAVE_FINALIZE"].ToString().Trim() == "S" || formCollection["IS_FDR_FILLED"].ToString().Trim() == "")
                                        {
                                            pmisTrailStrechFdrDetailsObj.IS_FINALIZED = "N";
                                        }
                                        else if (formCollection["SAVE_FINALIZE"].ToString().Trim() == "SF")
                                        {
                                            pmisTrailStrechFdrDetailsObj.IS_FINALIZED = "Y";
                                        }
                                        else
                                        {
                                            pmisTrailStrechFdrDetailsObj.IS_FINALIZED = "N";
                                        }
                                        pmisTrailStrechFdrDetailsObj.USERID = PMGSYSession.Current.UserId;  //--nullable val 
                                        pmisTrailStrechFdrDetailsObj.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];    //--nullable val 

                                        dbContext.PMIS_TRIAL_STRETCH_FDR_DETAIL.Add(pmisTrailStrechFdrDetailsObj);
                                        dbContext.SaveChanges();

                                        PMIS_ADDITIVE_USED_DETAIL pmisAdditiveUsedDetailsObj = null;
                                        for (var i = 0; i < multiselectlistArr.Length; i++)
                                        {
                                            pmisAdditiveUsedDetailsObj = new PMIS_ADDITIVE_USED_DETAIL();
                                            var additiveId = Convert.ToInt32(multiselectlistArr[i]);
                                            //var additiveDetailsId = dbContext.PMIS_ADDITIVE_USED_DETAIL.Max(cp => (Int32?)cp.ADDITIVE_DETAIL_ID) == null ? 1 : (Int32)dbContext.PMIS_ADDITIVE_USED_DETAIL.Max(cp => (Int32?)cp.ADDITIVE_DETAIL_ID) + 1;
                                            //pmisAdditiveUsedDetailsObj.ADDITIVE_DETAIL_ID = additiveDetailsId;
                                            pmisAdditiveUsedDetailsObj.ADDITIVE_DETAIL_ID = dbContext.PMIS_ADDITIVE_USED_DETAIL.Max(cp => (Int32?)cp.ADDITIVE_DETAIL_ID) == null ? 1 : (Int32)dbContext.PMIS_ADDITIVE_USED_DETAIL.Max(cp => (Int32?)cp.ADDITIVE_DETAIL_ID) + 1;
                                            pmisAdditiveUsedDetailsObj.TRIAL_STRETCH_ID = trialStrechIdPK;
                                            //pmisAdditiveUsedDetailsObj.ADDITIVE_ID = 1; //---------will get from MASTER_ADDITIVE table
                                            pmisAdditiveUsedDetailsObj.ADDITIVE_ID = dbContext.MASTER_ADDITIVE.Where(x => x.ADDITIVE_ID == additiveId).Select(y => y.ADDITIVE_ID).FirstOrDefault(); //---------will get from MASTER_ADDITIVE table
                                            pmisAdditiveUsedDetailsObj.IMS_PR_ROAD_CODE = Convert.ToInt32(formCollection["IMS_PR_ROAD_CODE"].ToString().Trim());
                                            pmisAdditiveUsedDetailsObj.USERID = PMGSYSession.Current.UserId;  //--nullable val 
                                            pmisAdditiveUsedDetailsObj.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];  //--nullable val 

                                            dbContext.PMIS_ADDITIVE_USED_DETAIL.Add(pmisAdditiveUsedDetailsObj);
                                            dbContext.SaveChanges();
                                        }

                                        dbContext.SaveChanges();

                                        scope.Complete();
                                        message = "Trial Strech FDR Details Saved Successfully.";
                                        return true;
                                    }
                                }
                                else
                                {
                                    message = "File is invalid";
                                    return false;
                                }
                            }
                            else
                            {
                                message = "File Size Should Be Less Than 4Mb";
                                return false;
                            }
                        }
                        else
                        {
                            message = "Please select file";
                            return false;
                        }

                    }
                    catch (DbEntityValidationException e)
                    {
                        foreach (var eve in e.EntityValidationErrors)
                        {
                            using (System.IO.StreamWriter sw = System.IO.File.AppendText(System.IO.Path.Combine("D:\\ErrorLog" + System.DateTime.Now.Date.ToString("dd_MM_yyyy") + ".txt", "")))
                            {
                                sw.WriteLine("Date :" + DateTime.Now.ToString());
                                sw.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                                eve.Entry.Entity.GetType().Name, eve.Entry.State);
                                sw.WriteLine("---------------------------------------------------------------------------------------");
                                sw.Close();
                            }

                            foreach (var ve in eve.ValidationErrors)
                            {
                                using (System.IO.StreamWriter sw = System.IO.File.AppendText(System.IO.Path.Combine("D:\\ErrorLog" + System.DateTime.Now.Date.ToString("dd_MM_yyyy") + ".txt", "")))
                                {
                                    sw.WriteLine("Date :" + DateTime.Now.ToString());
                                    sw.WriteLine("- Property: \"{0}\", Error: \"{1}\"",
                                    ve.PropertyName, ve.ErrorMessage);
                                    sw.WriteLine("---------------------------------------------------------------------------------------");
                                    sw.Close();
                                }
                            }
                        }
                        throw;
                    }
                    catch (DbUpdateException ex)
                    {
                        ErrorLog.LogError(ex, "SaveAddTrailStrechForFDRDAL(DbUpdateException ex).DAL");
                        return false;
                    }
                    catch (Exception ex)
                    {
                        ErrorLog.LogError(ex, "PMISDAL.SaveAddTrailStrechForFDRDAL()");
                        message = "Some Problem Occured Please Try After Some Time";
                        return false;
                    }
                    finally
                    {
                        file1.InputStream.Flush();
                        file1.InputStream.Close();

                        file2.InputStream.Flush();
                        file2.InputStream.Close();

                        if (dbContext != null)
                        {
                            dbContext.Dispose();
                        }

                    }

                }

                public string DeleteTrialStrechFdrDAL(int IMS_PR_ROAD_CODE)
                {
                    dbContext = new PMGSYEntities();
                    string uploaded_path = string.Empty;

                    try
                    {
                        using (var scope = new TransactionScope())
                        {
                            PMIS_TRIAL_STRETCH_FDR_DETAIL model = dbContext.PMIS_TRIAL_STRETCH_FDR_DETAIL.Where(x => x.IMS_PR_ROAD_CODE == IMS_PR_ROAD_CODE).FirstOrDefault();
                            uploaded_path = ConfigurationManager.AppSettings["PMIS_TRIAL_STRECH_FOR_FDR_UPLOAD_PATH"].ToString() + "\\" + model.STRETCH_CONSTR_DATE.Year + "\\" + model.STRETCH_CONSTR_DATE.Month;

                            if (model.TEST_RESULT_FILE_NAME != "" || model.TEST_RESULT_FILE_NAME != null)
                            {
                                var fullpath1 = Path.Combine(uploaded_path, model.TEST_RESULT_FILE_NAME);
                                FileInfo file_QM1 = new FileInfo(fullpath1);
                                if (file_QM1.Exists)
                                {
                                    file_QM1.Delete();
                                }
                            }
                            if (model.JMF_FILE_NAME != "" || model.JMF_FILE_NAME != null)
                            {
                                var fullpath2 = Path.Combine(uploaded_path, model.JMF_FILE_NAME);
                                FileInfo file_QM2 = new FileInfo(fullpath2);
                                if (file_QM2.Exists)
                                {
                                    file_QM2.Delete();
                                }

                            }

                            List<PMIS_ADDITIVE_USED_DETAIL> PMISAdditiveUsedDetailsObj = dbContext.PMIS_ADDITIVE_USED_DETAIL.Where(x => x.IMS_PR_ROAD_CODE == IMS_PR_ROAD_CODE).ToList();
                            if (PMISAdditiveUsedDetailsObj.Count > 0)
                            {
                                foreach (var item in PMISAdditiveUsedDetailsObj)
                                {
                                    dbContext.PMIS_ADDITIVE_USED_DETAIL.Remove(item);
                                }
                            }
                            dbContext.SaveChanges();

                            PMIS_TRIAL_STRETCH_FDR_DETAIL pmisTrialStrchFDRDetails = dbContext.PMIS_TRIAL_STRETCH_FDR_DETAIL.Where(x => x.IMS_PR_ROAD_CODE == IMS_PR_ROAD_CODE).FirstOrDefault();
                            if (pmisTrialStrchFDRDetails != null)
                            {
                                //dbContext.PMIS_TRIAL_STRETCH_FDR_DETAIL.Remove(pmisTrialStrchFDRDetails);
                                dbContext.Entry(pmisTrialStrchFDRDetails).State = EntityState.Deleted;
                            }
                            dbContext.SaveChanges();

                            scope.Complete();
                            return string.Empty;
                        }
                    }
                    catch (DbUpdateException ex)
                    {
                        ErrorLog.LogError(ex, "DeleteTrialStrechFdrDAL(DbUpdateException ex).DAL");
                        return ("An Update Error Occurred While Processing Your Request.");
                    }
                    catch (Exception ex)
                    {
                        ErrorLog.LogError(ex, "DeleteTrialStrechFdrDAL().DAL");
                        return ("Error Occurred While Processing Request.");
                    }
                    finally
                    {
                        dbContext.Dispose();
                    }

                }*/

        #endregion

        public bool SaveAddTrailStrechForFDRDAL(FormCollection formCollection, HttpPostedFileBase file1, HttpPostedFileBase file2, HttpPostedFileBase file3, ref string message)
        {
            dbContext = new PMGSYEntities();
            CommonFunctions commObj = new CommonFunctions();
            string fileName1 = string.Empty;
            string fileName2 = string.Empty;
            string fileName3 = string.Empty;
            string uploaded_File1 = string.Empty;
            string uploaded_File2 = string.Empty;
            string uploaded_File3 = string.Empty;
            var multiselectlistArr = formCollection["ADDITIVE_ID_List_Arr"].ToString().Trim().Split('$'); //chek if empty or undefined 
            var jmfId = Convert.ToInt32(formCollection["JMF_ID"].ToString().Trim());

            int savedInPmisTrialStrechFdrDetails;
            int savedPmisAdditiveUsedDetails;

            try
            {
                if ((file1.FileName != "" && file1.ContentLength != 0) || (file2.FileName != "" && file2.ContentLength != 0) || ((file3.FileName != "" && file3.ContentLength != 0)))
                {
                    int fileSizeLimit = Convert.ToInt32(ConfigurationManager.AppSettings["PMIS_TRIAL_STRECH_FOR_FDR_FILE_SIZE"]) * 1024 * 1024; //byte to kb to Mb
                    if ((file1.ContentLength < fileSizeLimit) || (file2.ContentLength < fileSizeLimit) || (file3.ContentLength < fileSizeLimit))
                    {
                        if ((file1 != null && file1.ContentLength > 0 && Path.GetExtension(file1.FileName).ToLower() == ".pdf") || (file2 != null && file2.ContentLength > 0 && Path.GetExtension(file2.FileName).ToLower() == ".pdf") || (file3 != null && file3.ContentLength > 0 && Path.GetExtension(file3.FileName).ToLower() == ".pdf"))
                        {
                            var trialStrechId = dbContext.PMIS_TRIAL_STRETCH_FDR_DETAIL.Max(cp => (Int32?)cp.TRIAL_STRETCH_ID) == null ? 1 : (Int32)dbContext.PMIS_TRIAL_STRETCH_FDR_DETAIL.Max(cp => (Int32?)cp.TRIAL_STRETCH_ID) + 1;
                            //var trialStrechId = dbContext.PMIS_TRIAL_STRETCH_FDR_DETAIL.Max(x => x.TRIAL_STRETCH_ID);

                            if (file1 != null && file1.ContentLength > 0 && Path.GetExtension(file1.FileName).ToLower() == ".pdf")
                            {
                                fileName1 = trialStrechId + "_PMIS_TestResult_TrailSection_" + System.DateTime.Now.ToString("dd_MM_yyyy_HHmmss") + ".pdf"; //add month and year 
                            }
                            if (file2 != null && file2.ContentLength > 0 && Path.GetExtension(file2.FileName).ToLower() == ".pdf")
                            {
                                fileName2 = trialStrechId + "_PMIS_JMF_Report_" + System.DateTime.Now.ToString("dd_MM_yyyy_HHmmss") + ".pdf"; //add month and year 
                            }
                            if (file3 != null && file3.ContentLength > 0 && Path.GetExtension(file3.FileName).ToLower() == ".pdf")
                            {
                                fileName3 = trialStrechId + "_PMIS_TestResult_MixDesign_" + System.DateTime.Now.ToString("dd_MM_yyyy_HHmmss") + ".pdf"; //add month and year 
                            }


                            using (var scope = new TransactionScope())
                            {
                                String Uploaded_Path = ConfigurationManager.AppSettings["PMIS_TRIAL_STRECH_FOR_FDR_UPLOAD_PATH"] + "\\" + DateTime.Now.Year + "\\" + DateTime.Now.Month;

                                //Save Data Operation --start
                                var trialStrechIdPK = dbContext.PMIS_TRIAL_STRETCH_FDR_DETAIL.Max(cp => (Int32?)cp.TRIAL_STRETCH_ID) == null ? 1 : (Int32)dbContext.PMIS_TRIAL_STRETCH_FDR_DETAIL.Max(cp => (Int32?)cp.TRIAL_STRETCH_ID) + 1;

                                PMIS_TRIAL_STRETCH_FDR_DETAIL pmisTrailStrechFdrDetailsObj = new PMIS_TRIAL_STRETCH_FDR_DETAIL();

                                pmisTrailStrechFdrDetailsObj.TRIAL_STRETCH_ID = trialStrechId;
                                pmisTrailStrechFdrDetailsObj.IMS_PR_ROAD_CODE = Convert.ToInt32(formCollection["IMS_PR_ROAD_CODE"].ToString().Trim());
                                pmisTrailStrechFdrDetailsObj.JMF_ID = dbContext.MASTER_JMF.Where(x => x.JMF_ID == jmfId).Select(y => y.JMF_ID).FirstOrDefault();// ----------------have to fetcj from master_jmf tbl

                                pmisTrailStrechFdrDetailsObj.ADDITIVE_CONTENT_UNIT = formCollection["IS_ADDITIVE_CONT_PERC_ML"].ToString().Trim();

                                if (pmisTrailStrechFdrDetailsObj.ADDITIVE_CONTENT_UNIT == "%")
                                {
                                    pmisTrailStrechFdrDetailsObj.PERC_ADDITIVE_CONTENT = Convert.ToDecimal(formCollection["PERC_ADDITIVE_CONT"].ToString().Trim());

                                }
                                else
                                {
                                    pmisTrailStrechFdrDetailsObj.ML_CUM_ADDITIVE_CONTENT = formCollection["ADDITIVE_CONT_ML_CUM"].ToString().Trim();
                                }

                                pmisTrailStrechFdrDetailsObj.PERC_CEMENT_CONTENT = Convert.ToDecimal(formCollection["PERC_CEMENT_CONT"].ToString().Trim());
                                pmisTrailStrechFdrDetailsObj.CEMENT_TYPE = formCollection["CEMENT_TYPE"].ToString().Trim();
                                pmisTrailStrechFdrDetailsObj.CEMENT_GRADE = formCollection["CEMENT_GRADE"].ToString().Trim();
                                pmisTrailStrechFdrDetailsObj.AVG_THICK_CRUST = Convert.ToDecimal(formCollection["AVG_THICK_CRUST"].ToString().Trim());
                                pmisTrailStrechFdrDetailsObj.PERC_PLASTICITY_RECLAM_SOIL = Convert.ToDecimal(formCollection["PERC_PLASTICITY_RECLAM_SOIL"].ToString().Trim());
                                pmisTrailStrechFdrDetailsObj.PERC_PLASTICITY_SUBGRADE_SOIL = Convert.ToDecimal(formCollection["PERC_INDEX_SUBGRADE_SOIL"].ToString().Trim());
                                pmisTrailStrechFdrDetailsObj.IS_GRAD_MATERIAL_SPEC_LIMIT = formCollection["IS_GRAD_MATERIAL_SPEC_LIMIT"].ToString().Trim();
                                pmisTrailStrechFdrDetailsObj.IS_AVG_UCS_7D_28D = formCollection["IS_AVG_UCS_7D_28D"].ToString().Trim();

                                if (pmisTrailStrechFdrDetailsObj.IS_AVG_UCS_7D_28D == "7D")
                                {
                                    pmisTrailStrechFdrDetailsObj.AVG_UCS_7DAY = formCollection["AVG_UCS_7D"].ToString().Trim();
                                }
                                else
                                {
                                    pmisTrailStrechFdrDetailsObj.AVG_UCS_28DAY = formCollection["AVG_UCS_28D"].ToString().Trim();
                                }

                                pmisTrailStrechFdrDetailsObj.IS_UCS_CUBE_OR_CYLINDER = formCollection["IS_UCS_TEST_CUBE_CYLINDER"].ToString().Trim();

                                if (pmisTrailStrechFdrDetailsObj.IS_UCS_CUBE_OR_CYLINDER == "Cube")
                                {
                                    pmisTrailStrechFdrDetailsObj.TEST_UCS_CUBE = formCollection["UCS_TEST_CUBE"].ToString().Trim();
                                }
                                else
                                {
                                    pmisTrailStrechFdrDetailsObj.TEST_UCS_CYLINDER = formCollection["UCS_TEST_CYLINDER"].ToString().Trim();
                                }

                                pmisTrailStrechFdrDetailsObj.MDD_MIX = Convert.ToDecimal(formCollection["MDD_MIX"].ToString().Trim());
                                pmisTrailStrechFdrDetailsObj.OMC_MIX = Convert.ToDecimal(formCollection["OMC_MIX"].ToString().Trim());
                                pmisTrailStrechFdrDetailsObj.IS_TS_UCS_CUBE_CYLINDRICAL = formCollection["IS_TS_UCS_STRENGTH_CUBE_CYLINDER"].ToString().Trim();

                                if (pmisTrailStrechFdrDetailsObj.IS_TS_UCS_CUBE_CYLINDRICAL == "Cube")
                                {
                                    pmisTrailStrechFdrDetailsObj.TS_TEST_UCS_CUBE = formCollection["UCS_STRENGTH_CUBE"].ToString().Trim();
                                }
                                else
                                {
                                    pmisTrailStrechFdrDetailsObj.TS_TEST_UCS_CYLINDRICAL = formCollection["UCS_STRENGTH_CYLINDER"].ToString().Trim();
                                }

                                pmisTrailStrechFdrDetailsObj.IS_TS_UCS_7D_28D = formCollection["IS_TS_UCS_7D_28D"].ToString().Trim();

                                if (pmisTrailStrechFdrDetailsObj.IS_TS_UCS_7D_28D == "7D")
                                {
                                    pmisTrailStrechFdrDetailsObj.TS_UCS_7D = formCollection["TS_UCS_7D"].ToString().Trim();
                                }
                                else
                                {
                                    pmisTrailStrechFdrDetailsObj.TS_UCS_28D = formCollection["TS_UCS_28D"].ToString().Trim();
                                }

                                pmisTrailStrechFdrDetailsObj.TS_RESD_STRENGTH_WETTING = formCollection["TS_RESD_STRENGTH_WETT"].ToString().Trim();
                                pmisTrailStrechFdrDetailsObj.CRACK_RELIEF_LAYER = formCollection["CRACK_RELIEF_LAYER"].ToString().Trim();

                                if (pmisTrailStrechFdrDetailsObj.CRACK_RELIEF_LAYER == "Other")
                                {
                                    pmisTrailStrechFdrDetailsObj.OTHER_CRACK_RELIEF_LAYER = formCollection["OTHER_CRACK_LAYER"].ToString().Trim();
                                }

                                pmisTrailStrechFdrDetailsObj.STRETCH_LENGTH = Convert.ToDecimal(formCollection["STRETCH_LENGTH"].ToString().Trim());
                                pmisTrailStrechFdrDetailsObj.STRETCH_CONSTR_DATE = commObj.GetStringToDateTime(formCollection["STRETCH_CONSTR_DATE"].ToString().Trim());

                                if (file1 != null && file1.ContentLength > 0 && Path.GetExtension(file1.FileName).ToLower() == ".pdf")
                                {
                                    pmisTrailStrechFdrDetailsObj.TS_TEST_RESULT_FILE_NAME = fileName1 == "" ? "" : fileName1;  //--nullable val 
                                    pmisTrailStrechFdrDetailsObj.TS_TEST_RESULT_FILE_PATH = Uploaded_Path == "" ? "" : Uploaded_Path;  //--nullable val 
                                }
                                else
                                {
                                    pmisTrailStrechFdrDetailsObj.TS_TEST_RESULT_FILE_NAME = "";  //--nullable val 
                                    pmisTrailStrechFdrDetailsObj.TS_TEST_RESULT_FILE_PATH = "";  //--nullable val
                                }
                                if (file2 != null && file2.ContentLength > 0 && Path.GetExtension(file2.FileName).ToLower() == ".pdf")
                                {
                                    pmisTrailStrechFdrDetailsObj.TS_RESD_STRENGTH_FILE_NAME = fileName2 == "" ? "" : fileName2;  //--nullable val 
                                    pmisTrailStrechFdrDetailsObj.TS_RESD_STRENGTH_FILE_PATH = Uploaded_Path == "" ? "" : Uploaded_Path;  //--nullable val 
                                }
                                else
                                {
                                    pmisTrailStrechFdrDetailsObj.TS_RESD_STRENGTH_FILE_NAME = "";  //--nullable val 
                                    pmisTrailStrechFdrDetailsObj.TS_RESD_STRENGTH_FILE_PATH = "";  //--nullable val 
                                }

                                if (file3 != null && file2.ContentLength > 0 && Path.GetExtension(file3.FileName).ToLower() == ".pdf")
                                {
                                    pmisTrailStrechFdrDetailsObj.TEST_RESULT_UCS_FILE_NAME = fileName3 == "" ? "" : fileName3;  //--nullable val 
                                    pmisTrailStrechFdrDetailsObj.TEST_RESULT_UCS_FILE_PATH = Uploaded_Path == "" ? "" : Uploaded_Path;  //--nullable val 
                                }
                                else
                                {
                                    pmisTrailStrechFdrDetailsObj.TEST_RESULT_UCS_FILE_NAME = "";  //--nullable val 
                                    pmisTrailStrechFdrDetailsObj.TEST_RESULT_UCS_FILE_PATH = "";  //--nullable val 
                                }

                                if (formCollection["SAVE_FINALIZE"].ToString().Trim() == "S" || formCollection["IS_FDR_FILLED"].ToString().Trim() == "")
                                {
                                    pmisTrailStrechFdrDetailsObj.IS_FINALIZED = "N";
                                }
                                else if (formCollection["SAVE_FINALIZE"].ToString().Trim() == "SF")
                                {
                                    pmisTrailStrechFdrDetailsObj.IS_FINALIZED = "Y";
                                }
                                else
                                {
                                    pmisTrailStrechFdrDetailsObj.IS_FINALIZED = "N";
                                }
                                pmisTrailStrechFdrDetailsObj.USERID = PMGSYSession.Current.UserId;  //--nullable val 
                                pmisTrailStrechFdrDetailsObj.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];    //--nullable val 

                                dbContext.PMIS_TRIAL_STRETCH_FDR_DETAIL.Add(pmisTrailStrechFdrDetailsObj);
                                savedInPmisTrialStrechFdrDetails = dbContext.SaveChanges();

                                PMIS_ADDITIVE_USED_DETAIL pmisAdditiveUsedDetailsObj = null;
                                for (var i = 0; i < multiselectlistArr.Length; i++)
                                {
                                    pmisAdditiveUsedDetailsObj = new PMIS_ADDITIVE_USED_DETAIL();
                                    var additiveId = Convert.ToInt32(multiselectlistArr[i]);
                                    //var additiveDetailsId = dbContext.PMIS_ADDITIVE_USED_DETAIL.Max(cp => (Int32?)cp.ADDITIVE_DETAIL_ID) == null ? 1 : (Int32)dbContext.PMIS_ADDITIVE_USED_DETAIL.Max(cp => (Int32?)cp.ADDITIVE_DETAIL_ID) + 1;
                                    //pmisAdditiveUsedDetailsObj.ADDITIVE_DETAIL_ID = additiveDetailsId;
                                    pmisAdditiveUsedDetailsObj.ADDITIVE_DETAIL_ID = dbContext.PMIS_ADDITIVE_USED_DETAIL.Max(cp => (Int32?)cp.ADDITIVE_DETAIL_ID) == null ? 1 : (Int32)dbContext.PMIS_ADDITIVE_USED_DETAIL.Max(cp => (Int32?)cp.ADDITIVE_DETAIL_ID) + 1;
                                    pmisAdditiveUsedDetailsObj.TRIAL_STRETCH_ID = trialStrechIdPK;
                                    //pmisAdditiveUsedDetailsObj.ADDITIVE_ID = 1; //---------will get from MASTER_ADDITIVE table
                                    pmisAdditiveUsedDetailsObj.ADDITIVE_ID = dbContext.MASTER_ADDITIVE.Where(x => x.ADDITIVE_ID == additiveId).Select(y => y.ADDITIVE_ID).FirstOrDefault(); //---------will get from MASTER_ADDITIVE table
                                    pmisAdditiveUsedDetailsObj.IMS_PR_ROAD_CODE = Convert.ToInt32(formCollection["IMS_PR_ROAD_CODE"].ToString().Trim());
                                    pmisAdditiveUsedDetailsObj.USERID = PMGSYSession.Current.UserId;  //--nullable val 
                                    pmisAdditiveUsedDetailsObj.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];  //--nullable val 

                                    dbContext.PMIS_ADDITIVE_USED_DETAIL.Add(pmisAdditiveUsedDetailsObj);
                                    dbContext.SaveChanges();
                                }

                                //savedPmisAdditiveUsedDetails = dbContext.SaveChanges();

                                if (savedInPmisTrialStrechFdrDetails > 0)
                                {
                                    if (!Directory.Exists(Uploaded_Path))
                                    {
                                        Directory.CreateDirectory(Uploaded_Path);
                                    }

                                    if (file1 != null && file1.ContentLength > 0 && Path.GetExtension(file1.FileName).ToLower() == ".pdf")
                                    {
                                        uploaded_File1 = Path.Combine(Uploaded_Path, fileName1);
                                        file1.SaveAs(uploaded_File1);
                                    }
                                    if (file2 != null && file2.ContentLength > 0 && Path.GetExtension(file2.FileName).ToLower() == ".pdf")
                                    {
                                        uploaded_File2 = Path.Combine(Uploaded_Path, fileName2);
                                        file2.SaveAs(uploaded_File2);
                                    }
                                    if (file3 != null && file3.ContentLength > 0 && Path.GetExtension(file3.FileName).ToLower() == ".pdf")
                                    {
                                        uploaded_File3 = Path.Combine(Uploaded_Path, fileName3);
                                        file3.SaveAs(uploaded_File3);
                                    }

                                    scope.Complete();
                                    message = "Trial Strech FDR Details Saved Successfully.";
                                    return true;
                                }

                                scope.Dispose();
                                message = "The problem occurs while saving data..";
                                return false;
                            }
                        }
                        else
                        {
                            message = "File is invalid";
                            return false;
                        }
                    }
                    else
                    {
                        message = "File Size Should Be Less Than 4Mb";
                        return false;
                    }
                }
                else
                {
                    message = "Please select file";
                    return false;
                }

            }
            catch (DbEntityValidationException e)
            {
                foreach (var eve in e.EntityValidationErrors)
                {
                    using (System.IO.StreamWriter sw = System.IO.File.AppendText(System.IO.Path.Combine("D:\\ErrorLog" + System.DateTime.Now.Date.ToString("dd_MM_yyyy") + ".txt", "")))
                    {
                        sw.WriteLine("Date :" + DateTime.Now.ToString());
                        sw.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                        eve.Entry.Entity.GetType().Name, eve.Entry.State);
                        sw.WriteLine("---------------------------------------------------------------------------------------");
                        sw.Close();
                    }

                    foreach (var ve in eve.ValidationErrors)
                    {
                        using (System.IO.StreamWriter sw = System.IO.File.AppendText(System.IO.Path.Combine("D:\\ErrorLog" + System.DateTime.Now.Date.ToString("dd_MM_yyyy") + ".txt", "")))
                        {
                            sw.WriteLine("Date :" + DateTime.Now.ToString());
                            sw.WriteLine("- Property: \"{0}\", Error: \"{1}\"",
                            ve.PropertyName, ve.ErrorMessage);
                            sw.WriteLine("---------------------------------------------------------------------------------------");
                            sw.Close();
                        }
                    }
                }
                throw;
            }
            catch (DbUpdateException ex)
            {
                ErrorLog.LogError(ex, "SaveAddTrailStrechForFDRDAL(DbUpdateException ex).DAL");
                return false;
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "PMISDAL.SaveAddTrailStrechForFDRDAL()");
                message = "Some Problems Occurred. Please Try After Some Time.";
                return false;
            }
            finally
            {
                file1.InputStream.Flush();
                file1.InputStream.Close();

                file2.InputStream.Flush();
                file2.InputStream.Close();

                file3.InputStream.Flush();
                file3.InputStream.Close();

                if (dbContext != null)
                {
                    dbContext.Dispose();
                }

            }

        }
        public string DeleteTrialStrechFdrDAL(int IMS_PR_ROAD_CODE)
        {
            dbContext = new PMGSYEntities();
            string uploaded_path = string.Empty;

            try
            {
                using (var scope = new TransactionScope())
                {
                    PMIS_TRIAL_STRETCH_FDR_DETAIL model = dbContext.PMIS_TRIAL_STRETCH_FDR_DETAIL.Where(x => x.IMS_PR_ROAD_CODE == IMS_PR_ROAD_CODE).FirstOrDefault();
                    uploaded_path = ConfigurationManager.AppSettings["PMIS_TRIAL_STRECH_FOR_FDR_UPLOAD_PATH"].ToString() + "\\" + model.STRETCH_CONSTR_DATE.Year + "\\" + model.STRETCH_CONSTR_DATE.Month;

                    /*if (model.TEST_RESULT_FILE_NAME != "" || model.TEST_RESULT_FILE_NAME != null)
                    {
                        var fullpath1 = Path.Combine(uploaded_path, model.TEST_RESULT_FILE_NAME);
                        FileInfo file_QM1 = new FileInfo(fullpath1);
                        if (file_QM1.Exists)
                        {
                            file_QM1.Delete();
                        }
                    }
                    if (model.JMF_FILE_NAME != "" || model.JMF_FILE_NAME != null)
                    {
                        var fullpath2 = Path.Combine(uploaded_path, model.JMF_FILE_NAME);
                        FileInfo file_QM2 = new FileInfo(fullpath2);
                        if (file_QM2.Exists)
                        {
                            file_QM2.Delete();
                        }

                    }*/

                    List<PMIS_ADDITIVE_USED_DETAIL> PMISAdditiveUsedDetailsObj = dbContext.PMIS_ADDITIVE_USED_DETAIL.Where(x => x.IMS_PR_ROAD_CODE == IMS_PR_ROAD_CODE).ToList();
                    if (PMISAdditiveUsedDetailsObj.Count > 0)
                    {
                        foreach (var item in PMISAdditiveUsedDetailsObj)
                        {
                            dbContext.PMIS_ADDITIVE_USED_DETAIL.Remove(item);
                        }
                    }
                    int saveChnagesAdditiveUsed = dbContext.SaveChanges();

                    PMIS_TRIAL_STRETCH_FDR_DETAIL pmisTrialStrchFDRDetails = dbContext.PMIS_TRIAL_STRETCH_FDR_DETAIL.Where(x => x.IMS_PR_ROAD_CODE == IMS_PR_ROAD_CODE).FirstOrDefault();
                    if (pmisTrialStrchFDRDetails != null)
                    {
                        //dbContext.PMIS_TRIAL_STRETCH_FDR_DETAIL.Remove(pmisTrialStrchFDRDetails);
                        dbContext.Entry(pmisTrialStrchFDRDetails).State = EntityState.Deleted;
                    }
                    int saveChangesTrialStrechFDR = dbContext.SaveChanges();

                    if (saveChnagesAdditiveUsed > 0 && saveChangesTrialStrechFDR > 0)
                    {
                        //if (model.TEST_RESULT_FILE_NAME != null)
                        if (model.TEST_RESULT_UCS_FILE_NAME != "" || model.TEST_RESULT_UCS_FILE_NAME != null)
                        {
                            var fullpath1 = Path.Combine(uploaded_path, model.TEST_RESULT_UCS_FILE_NAME);
                            FileInfo file_QM1 = new FileInfo(fullpath1);
                            if (file_QM1.Exists)
                            {
                                file_QM1.Delete();
                            }
                        }
                        //if (model.JMF_FILE_NAME != null)
                        if (model.TS_RESD_STRENGTH_FILE_NAME != "" || model.TS_RESD_STRENGTH_FILE_NAME != null)
                        {
                            var fullpath2 = Path.Combine(uploaded_path, model.TS_RESD_STRENGTH_FILE_NAME);
                            FileInfo file_QM2 = new FileInfo(fullpath2);
                            if (file_QM2.Exists)
                            {
                                file_QM2.Delete();
                            }
                        }
                        if (model.TS_TEST_RESULT_FILE_NAME != "" || model.TS_TEST_RESULT_FILE_NAME != null)
                        {
                            var fullpath3 = Path.Combine(uploaded_path, model.TS_TEST_RESULT_FILE_NAME);
                            FileInfo file_QM3 = new FileInfo(fullpath3);
                            if (file_QM3.Exists)
                            {
                                file_QM3.Delete();
                            }
                        }
                        scope.Complete();
                        return string.Empty;
                    }

                    /* scope.Complete();
                     return string.Empty;*/
                    scope.Dispose();
                    return ("The problem occurred while deleting data.");
                }
            }
            catch (DbUpdateException ex)
            {
                ErrorLog.LogError(ex, "DeleteTrialStrechFdrDAL(DbUpdateException ex).DAL");
                return ("An Update Error Occurred While Processing Your Request.");
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "DeleteTrialStrechFdrDAL().DAL");
                return ("Error Occurred While Processing Request.");
            }
            finally
            {
                dbContext.Dispose();
            }

        }
        public string FinalizeTrialStrechFdrDAL(int IMS_PR_ROAD_CODE)
        {
            dbContext = new PMGSYEntities();
            //string uploaded_path = string.Empty;

            try
            {
                using (var scope = new TransactionScope())
                {
                    PMIS_TRIAL_STRETCH_FDR_DETAIL pmisTrialStrchFDRDetails = dbContext.PMIS_TRIAL_STRETCH_FDR_DETAIL.Where(x => x.IMS_PR_ROAD_CODE == IMS_PR_ROAD_CODE).FirstOrDefault();
                    if (pmisTrialStrchFDRDetails != null)
                    {
                        pmisTrialStrchFDRDetails.IS_FINALIZED = "Y";
                        dbContext.PMIS_TRIAL_STRETCH_FDR_DETAIL.Attach(pmisTrialStrchFDRDetails);
                        dbContext.Entry(pmisTrialStrchFDRDetails).Property(x => x.IS_FINALIZED).IsModified = true;
                    }
                    dbContext.SaveChanges();
                    scope.Complete();
                    return string.Empty;
                }

            }
            catch (DbUpdateException ex)
            {
                ErrorLog.LogError(ex, "DeleteTrialStrechFdrDAL(DbUpdateException ex).DAL");
                return ("An Update Error Occurred While Processing Your Request.");
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "DeleteTrialStrechFdrDAL().DAL");
                return ("Error Occurred While Processing Request.");
            }
            finally
            {
                dbContext.Dispose();
            }

        }
        public bool UpdateTrialStrechFDRDetailsDAL(FormCollection formCollection, HttpPostedFileBase file1, HttpPostedFileBase file2, HttpPostedFileBase TestResultMixfile3, ref string message)
        {

            dbContext = new PMGSYEntities();
            CommonFunctions commObj = new CommonFunctions();
            string uploaded_path = string.Empty;
            PMIS_TRIAL_STRETCH_FDR_DETAIL pmisTrialStrechFdrDetailObj = new PMIS_TRIAL_STRETCH_FDR_DETAIL();

            int roadCode = Convert.ToInt32(formCollection["IMS_PR_ROAD_CODE"].Trim());
            string fileName1 = string.Empty;
            string fileName2 = string.Empty;
            string fileName3 = string.Empty;
            string MixDesignfile3 = string.Empty;
            string uploaded_File1 = string.Empty;
            string uploaded_File2 = string.Empty;
            string uploaded_File3 = string.Empty;
            string Uploaded_Path1 = string.Empty;
            string Uploaded_Path2 = string.Empty;
            string Uploaded_Path3 = string.Empty;
            var multiselectlistArr = formCollection["ADDITIVE_ID_List_Arr"].ToString().Trim().Split('$'); //check if empty or undefined 
            var jmfId = Convert.ToInt32(formCollection["JMF_ID"].ToString().Trim());
            try
            {
                /*
                1-if file exist delete it 
                2- save new files 
                3- save other data 
                */
                PMIS_TRIAL_STRETCH_FDR_DETAIL model1 = dbContext.PMIS_TRIAL_STRETCH_FDR_DETAIL.Where(x => x.IMS_PR_ROAD_CODE == roadCode).FirstOrDefault();

                using (var scope = new TransactionScope())
                {
                    PMIS_TRIAL_STRETCH_FDR_DETAIL model = dbContext.PMIS_TRIAL_STRETCH_FDR_DETAIL.Where(x => x.IMS_PR_ROAD_CODE == roadCode).FirstOrDefault();

                    var TS_TEST_RESULT_FILE_NAME = model.TS_TEST_RESULT_FILE_NAME;
                    var TS_RESD_JMF_FILE_NAME = model.TS_RESD_STRENGTH_FILE_NAME;
                    var MixDesign_FileName = model.TEST_RESULT_UCS_FILE_NAME;

                    //chnage on 19-06-2023
                    var month = string.Empty;
                    var year = string.Empty;
                    if (model.TEST_RESULT_UCS_FILE_NAME != "" && (TestResultMixfile3 != null && TestResultMixfile3.ContentLength > 0 && Path.GetExtension(TestResultMixfile3.FileName).ToLower() == ".pdf"))
                    {
                        //D:\OMMASII\PMIS\UPLOADS\2023\6  --Example
                        month = model.TEST_RESULT_UCS_FILE_PATH.ToString().Trim().Split('\\')[Convert.ToInt32(model.TEST_RESULT_UCS_FILE_PATH.ToString().Trim().Split('\\').Length) - 1];
                        year = model.TEST_RESULT_UCS_FILE_PATH.ToString().Trim().Split('\\')[Convert.ToInt32(model.TEST_RESULT_UCS_FILE_PATH.ToString().Trim().Split('\\').Length) - 2];
                    }
                    if ((model.TS_RESD_STRENGTH_FILE_NAME != null || model.TS_RESD_STRENGTH_FILE_NAME != "") && (file2 != null && file2.ContentLength > 0 && Path.GetExtension(file2.FileName).ToLower() == ".pdf"))
                    {
                        month = model.TS_RESD_STRENGTH_FILE_PATH.ToString().Trim().Split('\\')[Convert.ToInt32(model.TS_RESD_STRENGTH_FILE_PATH.ToString().Trim().Split('\\').Length) - 1];
                        year = model.TS_RESD_STRENGTH_FILE_PATH.ToString().Trim().Split('\\')[Convert.ToInt32(model.TS_RESD_STRENGTH_FILE_PATH.ToString().Trim().Split('\\').Length) - 2];
                    }
                    if ((model.TS_TEST_RESULT_FILE_NAME != null || model.TS_TEST_RESULT_FILE_NAME != "") && (file1 != null && file1.ContentLength > 0 && Path.GetExtension(file1.FileName).ToLower() == ".pdf"))
                    {
                        //D:\OMMASII\PMIS\UPLOADS\2023\6  --Example
                        month = model.TS_TEST_RESULT_FILE_PATH.ToString().Trim().Split('\\')[Convert.ToInt32(model.TS_TEST_RESULT_FILE_PATH.ToString().Trim().Split('\\').Length) - 1];
                        year = model.TS_TEST_RESULT_FILE_PATH.ToString().Trim().Split('\\')[Convert.ToInt32(model.TS_TEST_RESULT_FILE_PATH.ToString().Trim().Split('\\').Length) - 2];
                    }

                    uploaded_path = ConfigurationManager.AppSettings["PMIS_TRIAL_STRECH_FOR_FDR_UPLOAD_PATH"].ToString() + "\\" + year + "\\" + month;

                    //If file come from view and also present in DB then Delete it -- i.e Update File 
                    if ((file1 != null && file1.ContentLength > 0 && Path.GetExtension(file1.FileName).ToLower() == ".pdf") || (file2 != null && file2.ContentLength > 0 && Path.GetExtension(file2.FileName).ToLower() == ".pdf") || (TestResultMixfile3 != null && TestResultMixfile3.ContentLength > 0 && Path.GetExtension(TestResultMixfile3.FileName).ToLower() == ".pdf"))
                    {
                        #region Files Delete Operation
                        if ((file1 != null && file1.ContentLength > 0 && Path.GetExtension(file1.FileName).ToLower() == ".pdf"))
                        {
                            var fullpath1 = Path.Combine(uploaded_path, model.TS_TEST_RESULT_FILE_NAME);
                            FileInfo file_QM1 = new FileInfo(fullpath1);
                            if (file_QM1.Exists)
                            {
                                file_QM1.Delete();
                            }
                        }
                        if ((file2 != null && file2.ContentLength > 0 && Path.GetExtension(file2.FileName).ToLower() == ".pdf"))
                        {
                            var fullpath2 = Path.Combine(uploaded_path, model.TS_RESD_STRENGTH_FILE_NAME);
                            FileInfo file_QM2 = new FileInfo(fullpath2);
                            if (file_QM2.Exists)
                            {
                                file_QM2.Delete();
                            }
                        }
                        if ((TestResultMixfile3 != null && TestResultMixfile3.ContentLength > 0 && Path.GetExtension(TestResultMixfile3.FileName).ToLower() == ".pdf"))
                        {
                            var fullpath3 = Path.Combine(uploaded_path, model.TEST_RESULT_UCS_FILE_NAME);
                            FileInfo file_QM3 = new FileInfo(fullpath3);
                            if (file_QM3.Exists)
                            {
                                file_QM3.Delete();
                            }
                        }
                        #endregion

                        //file Delete opration end

                        #region Save Or Update/Replace Files Oparation
                        /*
                         1-If File is Alredy Present I.e Uploaded Then save new Updated file with that "old name and old path"
                         2-If File Is new Fresh file then save that with new current yr,month
                        */
                        int fileSizeLimit = Convert.ToInt32(ConfigurationManager.AppSettings["PMIS_TRIAL_STRECH_FOR_FDR_FILE_SIZE"]) * 1024 * 1024; //byte to kb to Mb
                        if ((file1.ContentLength < fileSizeLimit) || (file2.ContentLength < fileSizeLimit) || (TestResultMixfile3.ContentLength < fileSizeLimit))
                        {

                            //Check if Alredy Present I.e Uploaded Then save new Updated file with that "old name and old path"
                            //var trialStrechId = dbContext.PMIS_TRIAL_STRETCH_FDR_DETAIL.Max(cp => (Int32?)cp.TRIAL_STRETCH_ID) == null ? 1 : (Int32)dbContext.PMIS_TRIAL_STRETCH_FDR_DETAIL.Max(cp => (Int32?)cp.TRIAL_STRETCH_ID) + 1;
                            var trialStrechId = model.TRIAL_STRETCH_ID;
                            if ((file1 != null && file1.ContentLength > 0 && Path.GetExtension(file1.FileName).ToLower() == ".pdf"))
                            {
                                //If File is Alredy Uploded save with "same name and path"
                                if (TS_TEST_RESULT_FILE_NAME != "")
                                {
                                    fileName1 = model.TS_TEST_RESULT_FILE_NAME;
                                    Uploaded_Path1 = ConfigurationManager.AppSettings["PMIS_TRIAL_STRECH_FOR_FDR_UPLOAD_PATH"] + "\\" + year + "\\" + month;

                                    if (!Directory.Exists(Uploaded_Path1))
                                    {
                                        Directory.CreateDirectory(Uploaded_Path1);
                                    }

                                }//If File Fresh new, save with "new file name and Current yr and month"
                                else
                                {
                                    fileName1 = trialStrechId + "_PMIS_TestResult_TrailSection_" + System.DateTime.Now.ToString("dd_MM_yyyy_HHmmss") + ".pdf"; //add month and year 
                                    Uploaded_Path1 = ConfigurationManager.AppSettings["PMIS_TRIAL_STRECH_FOR_FDR_UPLOAD_PATH"] + "\\" + DateTime.Now.Year + "\\" + DateTime.Now.Month;

                                    if (!Directory.Exists(Uploaded_Path1))
                                    {
                                        Directory.CreateDirectory(Uploaded_Path1);
                                    }
                                }
                            }


                            if ((file2 != null && file2.ContentLength > 0 && Path.GetExtension(file2.FileName).ToLower() == ".pdf"))
                            {
                                //If File is Alredy Uploded save with "same name and path"
                                if (TS_RESD_JMF_FILE_NAME != "")
                                {
                                    fileName2 = model.TS_RESD_STRENGTH_FILE_NAME;
                                    Uploaded_Path2 = ConfigurationManager.AppSettings["PMIS_TRIAL_STRECH_FOR_FDR_UPLOAD_PATH"] + "\\" + year + "\\" + month;
                                    if (!Directory.Exists(Uploaded_Path2))
                                    {
                                        Directory.CreateDirectory(Uploaded_Path2);
                                    }
                                }//If File Fresh new, save with "new file name and Current yr and month"
                                else
                                {
                                    fileName2 = trialStrechId + "_PMIS_JMF_Report_" + System.DateTime.Now.ToString("dd_MM_yyyy_HHmmss") + ".pdf"; //add month and year 
                                    Uploaded_Path2 = ConfigurationManager.AppSettings["PMIS_TRIAL_STRECH_FOR_FDR_UPLOAD_PATH"] + "\\" + DateTime.Now.Year + "\\" + DateTime.Now.Month;

                                    if (!Directory.Exists(Uploaded_Path2))
                                    {
                                        Directory.CreateDirectory(Uploaded_Path2);
                                    }
                                }
                            }


                            if ((TestResultMixfile3 != null && TestResultMixfile3.ContentLength > 0 && Path.GetExtension(TestResultMixfile3.FileName).ToLower() == ".pdf"))
                            {
                                //If File is Alredy Uploded save with "same name and path"
                                if (MixDesign_FileName != "")
                                {
                                    fileName3 = model.TEST_RESULT_UCS_FILE_NAME;
                                    Uploaded_Path3 = ConfigurationManager.AppSettings["PMIS_TRIAL_STRECH_FOR_FDR_UPLOAD_PATH"] + "\\" + year + "\\" + month;
                                    if (!Directory.Exists(Uploaded_Path3))
                                    {
                                        Directory.CreateDirectory(Uploaded_Path3);
                                    }
                                }//If File Fresh new, save with "new file name and Current yr and month"
                                else
                                {
                                    fileName3 = trialStrechId + "_PMIS_TestResult_MixDesign_" + System.DateTime.Now.ToString("dd_MM_yyyy_HHmmss") + ".pdf"; //add month and year 
                                    Uploaded_Path3 = ConfigurationManager.AppSettings["PMIS_TRIAL_STRECH_FOR_FDR_UPLOAD_PATH"] + "\\" + DateTime.Now.Year + "\\" + DateTime.Now.Month;

                                    if (!Directory.Exists(Uploaded_Path3))
                                    {
                                        Directory.CreateDirectory(Uploaded_Path3);
                                    }
                                }
                            }


                            /*-------For Test Result File------- */
                            //for updating file 
                            if ((file1 != null && file1.ContentLength > 0 && Path.GetExtension(file1.FileName).ToLower() == ".pdf"))
                            {
                                //If File Alredy Uploaded but Again come for Updation
                                if ((file1 != null && file1.ContentLength > 0 && Path.GetExtension(file1.FileName).ToLower() == ".pdf") && (TS_TEST_RESULT_FILE_NAME != ""))
                                {
                                    uploaded_File1 = Path.Combine(Uploaded_Path1, fileName1);
                                    file1.SaveAs(uploaded_File1);
                                }
                                //Save New Fresh File
                                else
                                {
                                    uploaded_File1 = Path.Combine(Uploaded_Path1, fileName1);
                                    file1.SaveAs(uploaded_File1);
                                }
                            }

                            /*-------For Test JMF File-------*/
                            //for updating file 
                            if ((file2 != null && file2.ContentLength > 0 && Path.GetExtension(file2.FileName).ToLower() == ".pdf"))
                            {
                                //If File Alredy Uploaded but Again come for Updation
                                if ((file2 != null && file2.ContentLength > 0 && Path.GetExtension(file2.FileName).ToLower() == ".pdf") && (TS_RESD_JMF_FILE_NAME != ""))
                                {
                                    uploaded_File2 = Path.Combine(Uploaded_Path2, fileName2);
                                    file2.SaveAs(uploaded_File2);
                                }
                                //Save New Fresh File
                                else
                                {
                                    uploaded_File2 = Path.Combine(Uploaded_Path2, fileName2);
                                    file2.SaveAs(uploaded_File2);
                                }
                            }

                            if ((TestResultMixfile3 != null && TestResultMixfile3.ContentLength > 0 && Path.GetExtension(TestResultMixfile3.FileName).ToLower() == ".pdf"))
                            {
                                //If File Alredy Uploaded but Again come for Updation
                                if ((TestResultMixfile3 != null && TestResultMixfile3.ContentLength > 0 && Path.GetExtension(TestResultMixfile3.FileName).ToLower() == ".pdf") && (MixDesign_FileName != ""))
                                {
                                    uploaded_File3 = Path.Combine(Uploaded_Path3, fileName3);
                                    TestResultMixfile3.SaveAs(uploaded_File3);
                                }
                                //Save New Fresh File
                                else
                                {
                                    uploaded_File3 = Path.Combine(Uploaded_Path3, fileName3);
                                    TestResultMixfile3.SaveAs(uploaded_File3);
                                }
                            }

                        }  // if close
                        else
                        {
                            message = "File Size Should Be Less Than 4Mb";
                            return false;
                        }
                        #endregion
                        //file save opration end

                        #region Update Opration in DB

                        // 1- update major tbl
                        PMIS_TRIAL_STRETCH_FDR_DETAIL pmisTrailStrechFdrDetailsObj = dbContext.PMIS_TRIAL_STRETCH_FDR_DETAIL.Where(x => x.IMS_PR_ROAD_CODE == roadCode).FirstOrDefault();

                        if (pmisTrailStrechFdrDetailsObj != null)
                        {
                            //PMIS_TRIAL_STRETCH_FDR_DETAIL pmisTrailStrechFdrDetailsObj = new PMIS_TRIAL_STRETCH_FDR_DETAIL();

                            //pmisTrailStrechFdrDetailsObj.TRIAL_STRETCH_ID = pmisTrailStrechFdrObj.TRIAL_STRETCH_ID;
                            //pmisTrailStrechFdrDetailsObj.IMS_PR_ROAD_CODE = pmisTrailStrechFdrDetailsObj.IMS_PR_ROAD_CODE;
                            pmisTrailStrechFdrDetailsObj.PERC_CEMENT_CONTENT = Convert.ToDecimal(formCollection["PERC_CEMENT_CONT"].ToString().Trim());
                            pmisTrailStrechFdrDetailsObj.ADDITIVE_CONTENT_UNIT = formCollection["IS_ADDITIVE_CONT_PERC_ML"].ToString().Trim();

                            if (pmisTrailStrechFdrDetailsObj.ADDITIVE_CONTENT_UNIT == "%")
                            {
                                pmisTrailStrechFdrDetailsObj.PERC_ADDITIVE_CONTENT = Convert.ToDecimal(formCollection["PERC_ADDITIVE_CONT"].ToString().Trim());
                                pmisTrailStrechFdrDetailsObj.ML_CUM_ADDITIVE_CONTENT = null;
                            }
                            else
                            {
                                pmisTrailStrechFdrDetailsObj.PERC_ADDITIVE_CONTENT = null;
                                pmisTrailStrechFdrDetailsObj.ML_CUM_ADDITIVE_CONTENT = formCollection["ADDITIVE_CONT_ML_CUM"].ToString().Trim();
                            }

                            pmisTrailStrechFdrDetailsObj.CEMENT_TYPE = formCollection["CEMENT_TYPE"].ToString().Trim();
                            pmisTrailStrechFdrDetailsObj.CEMENT_GRADE = formCollection["CEMENT_GRADE"].ToString().Trim();
                            pmisTrailStrechFdrDetailsObj.AVG_THICK_CRUST = Convert.ToDecimal(formCollection["AVG_THICK_CRUST"].ToString().Trim());
                            pmisTrailStrechFdrDetailsObj.PERC_PLASTICITY_RECLAM_SOIL = Convert.ToDecimal(formCollection["PERC_PLASTICITY_RECLAM_SOIL"].ToString().Trim());
                            pmisTrailStrechFdrDetailsObj.PERC_PLASTICITY_SUBGRADE_SOIL = Convert.ToDecimal(formCollection["PERC_INDEX_SUBGRADE_SOIL"].ToString().Trim());
                            pmisTrailStrechFdrDetailsObj.IS_GRAD_MATERIAL_SPEC_LIMIT = formCollection["IS_GRAD_MATERIAL_SPEC_LIMIT"].ToString().Trim();
                            pmisTrailStrechFdrDetailsObj.IS_AVG_UCS_7D_28D = formCollection["IS_AVG_UCS_7D_28D"].ToString().Trim();

                            if (pmisTrailStrechFdrDetailsObj.IS_AVG_UCS_7D_28D == "7D")
                            {
                                pmisTrailStrechFdrDetailsObj.AVG_UCS_7DAY = formCollection["AVG_UCS_7D"].ToString().Trim();
                                pmisTrailStrechFdrDetailsObj.AVG_UCS_28DAY = null;
                            }
                            else
                            {
                                pmisTrailStrechFdrDetailsObj.AVG_UCS_7DAY = null;
                                pmisTrailStrechFdrDetailsObj.AVG_UCS_28DAY = formCollection["AVG_UCS_28D"].ToString().Trim();
                            }

                            pmisTrailStrechFdrDetailsObj.IS_UCS_CUBE_OR_CYLINDER = formCollection["IS_UCS_TEST_CUBE_CYLINDER"].ToString().Trim();

                            if (pmisTrailStrechFdrDetailsObj.IS_UCS_CUBE_OR_CYLINDER == "Cube")
                            {
                                pmisTrailStrechFdrDetailsObj.TEST_UCS_CUBE = formCollection["UCS_TEST_CUBE"].ToString().Trim();
                                pmisTrailStrechFdrDetailsObj.TEST_UCS_CYLINDER = null;
                            }
                            else
                            {
                                pmisTrailStrechFdrDetailsObj.TEST_UCS_CUBE = null;
                                pmisTrailStrechFdrDetailsObj.TEST_UCS_CYLINDER = formCollection["UCS_TEST_CYLINDER"].ToString().Trim();
                            }

                            pmisTrailStrechFdrDetailsObj.MDD_MIX = Convert.ToDecimal(formCollection["MDD_MIX"].ToString().Trim());
                            pmisTrailStrechFdrDetailsObj.OMC_MIX = Convert.ToDecimal(formCollection["OMC_MIX"].ToString().Trim());
                            pmisTrailStrechFdrDetailsObj.IS_TS_UCS_CUBE_CYLINDRICAL = formCollection["IS_TS_UCS_STRENGTH_CUBE_CYLINDER"].ToString().Trim();

                            if (pmisTrailStrechFdrDetailsObj.IS_TS_UCS_CUBE_CYLINDRICAL == "Cube")
                            {
                                pmisTrailStrechFdrDetailsObj.TS_TEST_UCS_CUBE = formCollection["UCS_STRENGTH_CUBE"].ToString().Trim();
                                pmisTrailStrechFdrDetailsObj.TS_TEST_UCS_CYLINDRICAL = null;
                            }
                            else
                            {
                                pmisTrailStrechFdrDetailsObj.TS_TEST_UCS_CUBE = null;
                                pmisTrailStrechFdrDetailsObj.TS_TEST_UCS_CYLINDRICAL = formCollection["UCS_STRENGTH_CYLINDER"].ToString().Trim();
                            }

                            pmisTrailStrechFdrDetailsObj.IS_TS_UCS_7D_28D = formCollection["IS_TS_UCS_7D_28D"].ToString().Trim();

                            if (pmisTrailStrechFdrDetailsObj.IS_TS_UCS_7D_28D == "7D")
                            {
                                pmisTrailStrechFdrDetailsObj.TS_UCS_7D = formCollection["TS_UCS_7D"].ToString().Trim();
                                pmisTrailStrechFdrDetailsObj.TS_UCS_28D = null;
                            }
                            else
                            {
                                pmisTrailStrechFdrDetailsObj.TS_UCS_7D = null;
                                pmisTrailStrechFdrDetailsObj.TS_UCS_28D = formCollection["TS_UCS_28D"].ToString().Trim();
                            }

                            pmisTrailStrechFdrDetailsObj.TS_RESD_STRENGTH_WETTING = formCollection["TS_RESD_STRENGTH_WETT"].ToString().Trim();
                            pmisTrailStrechFdrDetailsObj.CRACK_RELIEF_LAYER = formCollection["CRACK_RELIEF_LAYER"].ToString().Trim();

                            if (pmisTrailStrechFdrDetailsObj.CRACK_RELIEF_LAYER == "Other")
                            {
                                pmisTrailStrechFdrDetailsObj.OTHER_CRACK_RELIEF_LAYER = formCollection["OTHER_CRACK_LAYER"].ToString().Trim();
                            }
                            else
                            {
                                pmisTrailStrechFdrDetailsObj.OTHER_CRACK_RELIEF_LAYER = null;
                            }

                            pmisTrailStrechFdrDetailsObj.STRETCH_LENGTH = Convert.ToDecimal(formCollection["STRETCH_LENGTH"].ToString().Trim());
                            pmisTrailStrechFdrDetailsObj.STRETCH_CONSTR_DATE = commObj.GetStringToDateTime(formCollection["STRETCH_CONSTR_DATE"].ToString().Trim());

                            //if File Come for Updation then only update Path and File name
                            if ((file1 != null && file1.ContentLength > 0 && Path.GetExtension(file1.FileName).ToLower() == ".pdf"))
                            {
                                //pmisTrailStrechFdrDetailsObj.TS_TEST_RESULT_FILE_NAME = fileName1 == "" ? "" : fileName1;
                                //pmisTrailStrechFdrDetailsObj.TS_TEST_RESULT_FILE_PATH = TS_TEST_RESULT_FILE_NAME != null ? Uploaded_Path1 : Uploaded_Path2;  
                                //pmisTrailStrechFdrDetailsObj.TS_TEST_RESULT_FILE_PATH = Uploaded_Path1 != "" ? Uploaded_Path1 : Uploaded_Path2;
                            }
                            //if File Come for Updation then only update Path and File name
                            if ((file2 != null && file2.ContentLength > 0 && Path.GetExtension(file2.FileName).ToLower() == ".pdf"))
                            {
                                // TS_RESD_STRENGTH_FILE_NAME
                                //pmisTrailStrechFdrDetailsObj.TS_RESD_STRENGTH_FILE_NAME = fileName2 == "" ? "" : fileName2;  //--nullable val 
                                //TS_RESD_JMF_FILE_NAME = Uploaded_Path1 != "" ? Uploaded_Path1 : Uploaded_Path2;   //--nullable val 
                            }
                            if ((TestResultMixfile3 != null && TestResultMixfile3.ContentLength > 0 && Path.GetExtension(TestResultMixfile3.FileName).ToLower() == ".pdf"))
                            {
                                //pmisTrailStrechFdrDetailsObj.TEST_RESULT_UCS_FILE_NAME = fileName3 == "" ? "" : fileName3;  //--nullable val 
                                //MixDesign_FileName = Uploaded_Path3 != "" ? Uploaded_Path3 : Uploaded_Path3;   //--nullable val 
                            }

                            pmisTrailStrechFdrDetailsObj.USERID = PMGSYSession.Current.UserId;  //--nullable val 
                            pmisTrailStrechFdrDetailsObj.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];    //--nullable val 

                            dbContext.Entry(pmisTrailStrechFdrDetailsObj).State = System.Data.Entity.EntityState.Modified;
                            dbContext.SaveChanges();
                        }

                        //****************** 2 - update minor tbl ******************
                        //1st delet previos entryes
                        List<PMIS_ADDITIVE_USED_DETAIL> PMISAdditiveUsedDetailsObj = dbContext.PMIS_ADDITIVE_USED_DETAIL.Where(x => x.IMS_PR_ROAD_CODE == roadCode).ToList();
                        if (PMISAdditiveUsedDetailsObj.Count > 0)
                        {
                            foreach (var item in PMISAdditiveUsedDetailsObj)
                            {
                                dbContext.PMIS_ADDITIVE_USED_DETAIL.Remove(item);
                            }
                        }
                        dbContext.SaveChanges();

                        PMIS_ADDITIVE_USED_DETAIL pmisAdditiveUsedDetailsObjNew = null;
                        for (var i = 0; i < multiselectlistArr.Length; i++)
                        {
                            pmisAdditiveUsedDetailsObjNew = new PMIS_ADDITIVE_USED_DETAIL();
                            var additiveId = Convert.ToInt32(multiselectlistArr[i]);
                            pmisAdditiveUsedDetailsObjNew.ADDITIVE_DETAIL_ID = dbContext.PMIS_ADDITIVE_USED_DETAIL.Max(cp => (Int32?)cp.ADDITIVE_DETAIL_ID) == null ? 1 : (Int32)dbContext.PMIS_ADDITIVE_USED_DETAIL.Max(cp => (Int32?)cp.ADDITIVE_DETAIL_ID) + 1;
                            pmisAdditiveUsedDetailsObjNew.TRIAL_STRETCH_ID = pmisTrailStrechFdrDetailsObj.TRIAL_STRETCH_ID;
                            pmisAdditiveUsedDetailsObjNew.ADDITIVE_ID = dbContext.MASTER_ADDITIVE.Where(x => x.ADDITIVE_ID == additiveId).Select(y => y.ADDITIVE_ID).FirstOrDefault(); //---------will get from MASTER_ADDITIVE table
                            pmisAdditiveUsedDetailsObjNew.IMS_PR_ROAD_CODE = Convert.ToInt32(formCollection["IMS_PR_ROAD_CODE"].ToString().Trim());
                            pmisAdditiveUsedDetailsObjNew.USERID = PMGSYSession.Current.UserId;  //--nullable val 
                            pmisAdditiveUsedDetailsObjNew.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];  //--nullable val 

                            dbContext.PMIS_ADDITIVE_USED_DETAIL.Add(pmisAdditiveUsedDetailsObjNew);
                            dbContext.SaveChanges();
                        }

                        dbContext.SaveChanges();

                        scope.Complete();
                        message = "Trial Strech For FDR Is Updated Sucessfully";
                        return true;

                        #endregion
                    }
                    else
                    {
                        #region Update Opration in DB
                        string Item = string.Empty;
                        // 1- update major tbl
                        PMIS_TRIAL_STRETCH_FDR_DETAIL pmisTrailStrechFdrDetailsObj = dbContext.PMIS_TRIAL_STRETCH_FDR_DETAIL.Where(x => x.IMS_PR_ROAD_CODE == roadCode).FirstOrDefault();
                        if (pmisTrailStrechFdrDetailsObj != null)
                        {
                            //pmisTrailStrechFdrDetailsObj.TRIAL_STRETCH_ID = pmisTrailStrechFdrObj.TRIAL_STRETCH_ID;

                            pmisTrailStrechFdrDetailsObj.JMF_ID = dbContext.MASTER_JMF.Where(x => x.JMF_ID == jmfId).Select(y => y.JMF_ID).FirstOrDefault();// ----------------have to fetcj from master_jmf tbl
                            pmisTrailStrechFdrDetailsObj.PERC_CEMENT_CONTENT = Convert.ToDecimal(formCollection["PERC_CEMENT_CONT"].ToString().Trim());
                            pmisTrailStrechFdrDetailsObj.ADDITIVE_CONTENT_UNIT = formCollection["IS_ADDITIVE_CONT_PERC_ML"].ToString().Trim();

                            if (pmisTrailStrechFdrDetailsObj.ADDITIVE_CONTENT_UNIT == "%")
                            {
                                pmisTrailStrechFdrDetailsObj.PERC_ADDITIVE_CONTENT = Convert.ToDecimal(formCollection["PERC_ADDITIVE_CONT"].ToString().Trim());
                                pmisTrailStrechFdrDetailsObj.ML_CUM_ADDITIVE_CONTENT = null;
                            }
                            else
                            {
                                pmisTrailStrechFdrDetailsObj.PERC_ADDITIVE_CONTENT = null;
                                pmisTrailStrechFdrDetailsObj.ML_CUM_ADDITIVE_CONTENT = formCollection["ADDITIVE_CONT_ML_CUM"].ToString().Trim();
                            }

                            pmisTrailStrechFdrDetailsObj.CEMENT_TYPE = formCollection["CEMENT_TYPE"].ToString().Trim();
                            pmisTrailStrechFdrDetailsObj.CEMENT_GRADE = formCollection["CEMENT_GRADE"].ToString().Trim();
                            pmisTrailStrechFdrDetailsObj.AVG_THICK_CRUST = Convert.ToDecimal(formCollection["AVG_THICK_CRUST"].ToString().Trim());
                            pmisTrailStrechFdrDetailsObj.PERC_PLASTICITY_RECLAM_SOIL = Convert.ToDecimal(formCollection["PERC_PLASTICITY_RECLAM_SOIL"].ToString().Trim());
                            pmisTrailStrechFdrDetailsObj.PERC_PLASTICITY_SUBGRADE_SOIL = Convert.ToDecimal(formCollection["PERC_INDEX_SUBGRADE_SOIL"].ToString().Trim());
                            pmisTrailStrechFdrDetailsObj.IS_GRAD_MATERIAL_SPEC_LIMIT = formCollection["IS_GRAD_MATERIAL_SPEC_LIMIT"].ToString().Trim();
                            pmisTrailStrechFdrDetailsObj.IS_AVG_UCS_7D_28D = formCollection["IS_AVG_UCS_7D_28D"].ToString().Trim();

                            if (pmisTrailStrechFdrDetailsObj.IS_AVG_UCS_7D_28D == "7D")
                            {
                                pmisTrailStrechFdrDetailsObj.AVG_UCS_7DAY = formCollection["AVG_UCS_7D"].ToString().Trim();
                                pmisTrailStrechFdrDetailsObj.AVG_UCS_28DAY = null;
                            }
                            else
                            {
                                pmisTrailStrechFdrDetailsObj.AVG_UCS_7DAY = null;
                                pmisTrailStrechFdrDetailsObj.AVG_UCS_28DAY = formCollection["AVG_UCS_28D"].ToString().Trim();
                            }
                            pmisTrailStrechFdrDetailsObj.IS_UCS_CUBE_OR_CYLINDER = formCollection["IS_UCS_TEST_CUBE_CYLINDER"].ToString().Trim();

                            if (pmisTrailStrechFdrDetailsObj.IS_UCS_CUBE_OR_CYLINDER == "Cube")
                            {
                                pmisTrailStrechFdrDetailsObj.TEST_UCS_CUBE = formCollection["UCS_TEST_CUBE"].ToString().Trim();
                                pmisTrailStrechFdrDetailsObj.TEST_UCS_CYLINDER = null;
                            }
                            else
                            {
                                pmisTrailStrechFdrDetailsObj.TEST_UCS_CUBE = null;
                                pmisTrailStrechFdrDetailsObj.TEST_UCS_CYLINDER = formCollection["UCS_TEST_CYLINDER"].ToString().Trim();
                            }

                            pmisTrailStrechFdrDetailsObj.MDD_MIX = Convert.ToDecimal(formCollection["MDD_MIX"].ToString().Trim());
                            pmisTrailStrechFdrDetailsObj.OMC_MIX = Convert.ToDecimal(formCollection["OMC_MIX"].ToString().Trim());
                            pmisTrailStrechFdrDetailsObj.IS_TS_UCS_CUBE_CYLINDRICAL = formCollection["IS_TS_UCS_STRENGTH_CUBE_CYLINDER"].ToString().Trim();

                            if (pmisTrailStrechFdrDetailsObj.IS_TS_UCS_CUBE_CYLINDRICAL == "Cube")
                            {
                                pmisTrailStrechFdrDetailsObj.TS_TEST_UCS_CUBE = formCollection["UCS_STRENGTH_CUBE"].ToString().Trim();
                                pmisTrailStrechFdrDetailsObj.TS_TEST_UCS_CYLINDRICAL = null;
                            }
                            else
                            {
                                pmisTrailStrechFdrDetailsObj.TS_TEST_UCS_CUBE = null;
                                pmisTrailStrechFdrDetailsObj.TS_TEST_UCS_CYLINDRICAL = formCollection["UCS_STRENGTH_CYLINDER"].ToString().Trim();
                            }

                            pmisTrailStrechFdrDetailsObj.IS_TS_UCS_7D_28D = formCollection["IS_TS_UCS_7D_28D"].ToString().Trim();

                            if (pmisTrailStrechFdrDetailsObj.IS_TS_UCS_7D_28D == "7D")
                            {
                                pmisTrailStrechFdrDetailsObj.TS_UCS_7D = formCollection["TS_UCS_7D"].ToString().Trim();
                                pmisTrailStrechFdrDetailsObj.TS_UCS_28D = null;
                            }
                            else
                            {
                                pmisTrailStrechFdrDetailsObj.TS_UCS_7D = null;
                                pmisTrailStrechFdrDetailsObj.TS_UCS_28D = formCollection["TS_UCS_28D"].ToString().Trim();
                            }

                            pmisTrailStrechFdrDetailsObj.TS_RESD_STRENGTH_WETTING = formCollection["TS_RESD_STRENGTH_WETT"].ToString().Trim();
                            pmisTrailStrechFdrDetailsObj.CRACK_RELIEF_LAYER = formCollection["CRACK_RELIEF_LAYER"].ToString().Trim();

                            if (pmisTrailStrechFdrDetailsObj.CRACK_RELIEF_LAYER == "Other")
                            {
                                pmisTrailStrechFdrDetailsObj.OTHER_CRACK_RELIEF_LAYER = formCollection["OTHER_CRACK_LAYER"].ToString().Trim();
                            }
                            else
                            {
                                pmisTrailStrechFdrDetailsObj.OTHER_CRACK_RELIEF_LAYER = null;
                            }

                            pmisTrailStrechFdrDetailsObj.STRETCH_LENGTH = Convert.ToDecimal(formCollection["STRETCH_LENGTH"].ToString().Trim());
                            pmisTrailStrechFdrDetailsObj.STRETCH_CONSTR_DATE = commObj.GetStringToDateTime(formCollection["STRETCH_CONSTR_DATE"].ToString().Trim());

                            pmisTrailStrechFdrDetailsObj.USERID = PMGSYSession.Current.UserId;  //--nullable val 
                            pmisTrailStrechFdrDetailsObj.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];    //--nullable val 

                            dbContext.Entry(pmisTrailStrechFdrDetailsObj).State = System.Data.Entity.EntityState.Modified;
                            dbContext.SaveChanges();
                        }

                        //****************** 2 - update minor tbl ******************
                        //1st delete previos entryes
                        List<PMIS_ADDITIVE_USED_DETAIL> PMISAdditiveUsedDetailsObj = dbContext.PMIS_ADDITIVE_USED_DETAIL.Where(x => x.IMS_PR_ROAD_CODE == roadCode).ToList();
                        if (PMISAdditiveUsedDetailsObj.Count > 0)
                        {
                            foreach (var item in PMISAdditiveUsedDetailsObj)
                            {
                                dbContext.PMIS_ADDITIVE_USED_DETAIL.Remove(item);
                            }
                        }
                        dbContext.SaveChanges();

                        PMIS_ADDITIVE_USED_DETAIL pmisAdditiveUsedDetailsObjNew = null;
                        for (var i = 0; i < multiselectlistArr.Length; i++)
                        {
                            pmisAdditiveUsedDetailsObjNew = new PMIS_ADDITIVE_USED_DETAIL();
                            var additiveId = Convert.ToInt32(multiselectlistArr[i]);
                            pmisAdditiveUsedDetailsObjNew.ADDITIVE_DETAIL_ID = dbContext.PMIS_ADDITIVE_USED_DETAIL.Max(cp => (Int32?)cp.ADDITIVE_DETAIL_ID) == null ? 1 : (Int32)dbContext.PMIS_ADDITIVE_USED_DETAIL.Max(cp => (Int32?)cp.ADDITIVE_DETAIL_ID) + 1;
                            pmisAdditiveUsedDetailsObjNew.TRIAL_STRETCH_ID = pmisTrailStrechFdrDetailsObj.TRIAL_STRETCH_ID;
                            pmisAdditiveUsedDetailsObjNew.ADDITIVE_ID = dbContext.MASTER_ADDITIVE.Where(x => x.ADDITIVE_ID == additiveId).Select(y => y.ADDITIVE_ID).FirstOrDefault(); //---------will get from MASTER_ADDITIVE table
                            pmisAdditiveUsedDetailsObjNew.IMS_PR_ROAD_CODE = Convert.ToInt32(formCollection["IMS_PR_ROAD_CODE"].ToString().Trim());
                            pmisAdditiveUsedDetailsObjNew.USERID = PMGSYSession.Current.UserId;  //--nullable val 
                            pmisAdditiveUsedDetailsObjNew.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];  //--nullable val 

                            dbContext.PMIS_ADDITIVE_USED_DETAIL.Add(pmisAdditiveUsedDetailsObjNew);
                            dbContext.SaveChanges();
                        }
                        dbContext.SaveChanges();

                        scope.Complete();
                        message = "Trial Strech For FDR Is Updated Sucessfully";
                        return true;

                        #endregion
                    }
                }   //scope end

            }
            catch (DbEntityValidationException e)
            {
                foreach (var eve in e.EntityValidationErrors)
                {
                    using (System.IO.StreamWriter sw = System.IO.File.AppendText(System.IO.Path.Combine("D:\\ErrorLog" + System.DateTime.Now.Date.ToString("dd_MM_yyyy") + ".txt", "")))
                    {
                        sw.WriteLine("Date :" + DateTime.Now.ToString());
                        sw.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                        eve.Entry.Entity.GetType().Name, eve.Entry.State);
                        sw.WriteLine("---------------------------------------------------------------------------------------");
                        sw.Close();
                    }

                    foreach (var ve in eve.ValidationErrors)
                    {
                        using (System.IO.StreamWriter sw = System.IO.File.AppendText(System.IO.Path.Combine("D:\\ErrorLog" + System.DateTime.Now.Date.ToString("dd_MM_yyyy") + ".txt", "")))
                        {
                            sw.WriteLine("Date :" + DateTime.Now.ToString());
                            sw.WriteLine("- Property: \"{0}\", Error: \"{1}\"",
                            ve.PropertyName, ve.ErrorMessage);
                            sw.WriteLine("---------------------------------------------------------------------------------------");
                            sw.Close();
                        }
                    }
                }
                throw;
            }
            catch (DbUpdateException ex)
            {
                ErrorLog.LogError(ex, "UpdateTrialStrechFDRDetailsDAL(DbUpdateException ex).DAL");
                if (file1.ContentLength > 0)
                {
                    file1.InputStream.Flush();
                    file1.InputStream.Close();
                }

                if (file2.ContentLength > 0)
                {
                    file2.InputStream.Flush();
                    file2.InputStream.Close();
                }

                if (TestResultMixfile3.ContentLength > 0)
                {
                    TestResultMixfile3.InputStream.Flush();
                    TestResultMixfile3.InputStream.Close();
                }
                message = "Error Occurred while processing request, please try after some time.";
                return false;
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "UpdateTrialStrechFDRDetailsDAL");
                if (file1.ContentLength > 0)
                {
                    file1.InputStream.Flush();
                    file1.InputStream.Close();
                }

                if (file2.ContentLength > 0)
                {
                    file2.InputStream.Flush();
                    file2.InputStream.Close();
                }

                if (TestResultMixfile3.ContentLength > 0)
                {
                    TestResultMixfile3.InputStream.Flush();
                    TestResultMixfile3.InputStream.Close();
                }

                message = "Error Occurred while processing request, please try after some time.";
                return false;
            }
            finally
            {
                if (dbContext != null)
                {
                    dbContext.Dispose();
                }
            }
            //return true;
        }

        #endregion

    }

    public interface IPMISDAL
    {
        Array PMISRoadListDAL(int state, int district, int block, int sanction_year, int batch, int? page, int? rows, string sidx, string sord, out long totalRecords);
        string SaveRoadProjectPlanDAL(IEnumerable<AddPlanPMISViewModel> planData);      
        string DeleteRoadProjectPlanDAL(int IMS_PR_ROAD_CODE, int PLAN_ID);       
        string ReviseRoadProjectPlanDAL(int IMS_PR_ROAD_CODE);  
        string FinalizeRoadProjectPlanDAL(int IMS_PR_ROAD_CODE); 
        string UpdatePMISRoadProjectPlanDAL(IEnumerable<AddPlanPMISViewModel> planData);  
        string SaveActualsDAL(IEnumerable<AddActualsViewModel> planData);
        string SubmitActualsDAL(IEnumerable<AddActualsViewModel> planData);
        string SubmitChainageDetailsDAL(FormCollection formData);
        void PMISLog(string module, string logPath, string message, string fileName);

        //   Array PMISRoadListDAL(int state, int district, int block, int sanction_year, int batch, int? page, int? rows, string sidx, string sord, out long totalRecords);

        #region PMIS Data Correction

        Array PMISDataCorrectionList(int state, int district, int block, int sanction_year, int batch, string listType, int? page, int? rows, string sidx, string sord, out long totalRecords);
        Array PMISDataDeleteProgressPlanList(int roadCode, int? page, int? rows, string sidx, string sord, out long totalRecords);
        string DataDeletePlan(int PLAN_ID);
        string DataDeleteProgress(int PLAN_ID);
        UpdateCompletionDateLengthModel GetPlanDetailsToEdit(int RoadCode);
        string UpdatePlanCompletionDetails(string listType, int planid, int baselineno, decimal completionLength, DateTime? completionDate);

        #endregion


        #region PMIS BRIDGE DAL INTERFACE

        Array PMISBridgeListDAL(int state, int district, int block, int sanction_year, int batch, int? page, int? rows, string sidx, string sord, out long totalRecords);
        string UpdatePMISBridgeProjectPlanDAL(IEnumerable<AddPlanPMISViewModelBridge> planData);
        string ReviseBridgeProjectPlanDAL(int IMS_PR_ROAD_CODE);
        string FinalizeBridgeProjectPlanDAL(int IMS_PR_ROAD_CODE);
        string DeleteBridgeProjectPlanDAL(int IMS_PR_ROAD_CODE, int PLAN_ID);
        string SaveBridgeProjectPlanDAL(IEnumerable<AddPlanPMISViewModelBridge> planData);
        string SubmitBridgeActualsDAL(IEnumerable<AddActualsViewModel> planData);    // new IDAL for Bridge
        string SaveBridgeActualsDAL(IEnumerable<AddActualsViewModel> planData);   // for update actuals

        #endregion


        #region FDR Stabilize Detail

        string SubmitFDRChainageDAL(AddFDRStabilizeModel ChainageModel);  // FormCollection

        string UpdateFDRChainageDAL(AddFDRStabilizeModel ChainageModel); //

        Array PMISFDRStabListDAL(int RoadCode, int? page, int? rows, string sidx, string sord, out long totalRecords);

        #endregion

        #region Added By Hrishikesh To Add "Trail Strech For FDR" --05-06-2023 ---start
        bool SaveAddTrailStrechForFDRDAL(FormCollection formCollection, HttpPostedFileBase file1, HttpPostedFileBase file2, HttpPostedFileBase file3, ref string message);
        string DeleteTrialStrechFdrDAL(int IMS_PR_ROAD_CODE);
        string FinalizeTrialStrechFdrDAL(int IMS_PR_ROAD_CODE);
        bool UpdateTrialStrechFDRDetailsDAL(FormCollection formCollection, HttpPostedFileBase file1, HttpPostedFileBase file2, HttpPostedFileBase TestResultMixfile3, ref string message);
        #endregion


    }
}
