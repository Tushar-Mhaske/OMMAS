using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PMGSY.Models;
using PMGSY.Controllers;
using PMGSY.Areas.PMIS.Models;
using PMGSY.Common;
using PMGSY.Extensions;
using PMGSY.Areas.PMIS.DAL;
using System.Runtime.InteropServices;
using System.Data.Entity.Infrastructure;
using System.IO;
using System.Configuration;
using System.Text;

namespace PMGSY.Areas.PMIS.Controllers
{

    public class PMISController : Controller
    {

        #region PMIS List Road
        public ActionResult PMISRoadLayout()
        {
            ListPMISRoadDetailsViewModel model = new ListPMISRoadDetailsViewModel();
            CommonFunctions comm = new CommonFunctions();
            try
            {
                model.StateList = new List<SelectListItem>();
                model.StateList.Insert(0, new SelectListItem() { Text = PMGSYSession.Current.StateName, Value = PMGSYSession.Current.StateCode.ToString() });
                model.DistrictList = new List<SelectListItem>();
                if (PMGSYSession.Current.DistrictCode == 0)
                {
                    model.DistrictList = comm.PopulateDistrictForSRRDA(PMGSYSession.Current.StateCode);
                    // model.DistrictList.Find(x => x.Value == "0").Text = "Select District";
                }
                else if (PMGSYSession.Current.DistrictCode > 0)
                {

                    model.DistrictList.Insert(0, new SelectListItem() { Text = PMGSYSession.Current.DistrictName, Value = PMGSYSession.Current.DistrictCode.ToString() });
                }

                model.BlockList = new List<SelectListItem>();
                if (PMGSYSession.Current.DistrictCode == 0)
                {
                    model.BlockList.Insert(0, (new SelectListItem { Text = "All Blocks", Value = "0", Selected = true }));
                }
                else
                {
                    model.BlockList = comm.PopulateBlocks((PMGSYSession.Current.DistrictCode), true);

                    model.BlockList.Find(x => x.Value == "-1").Value = "0";
                    model.BlockList.Find(x => x.Value == model.BlockCode.ToString()).Selected = true;
                }
                model.Sanction_Year_List = comm.PopulateFinancialYear(true, true).ToList();
                model.Sanction_Year_List.RemoveAt(0);

                model.BatchList = comm.PopulateBatch();
                model.BatchList.RemoveAt(0);
                model.BatchList.Insert(0, new SelectListItem { Value = "0", Text = "All Batches" });

                return View(model);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "PMISController.PMISRoadLayout()");
                return null;
            }
        }

        [HttpPost]
        public JsonResult PopulateBlocks()
        {
            try
            {
                CommonFunctions objCommonFunctions = new CommonFunctions();
                int DistCode = Convert.ToInt32(Request.Params["DistrictCode"]);

                List<SelectListItem> lstBlock = new List<SelectListItem>();
                lstBlock = objCommonFunctions.PopulateBlocks(DistCode, true);
                lstBlock.RemoveAt(0);
                lstBlock.Insert(0, new SelectListItem { Value = "0", Text = "All Blocks" });

                return Json(lstBlock);
            }
            catch
            {
                return Json(new { string.Empty });
            }
        }

        [HttpPost]
        public ActionResult PMISRoadList(int? page, int? rows, string sidx, string sord)
        {
            long totalRecords; int state = 0; int district = 0; int block = 0; int sanction_year = 0; int batch = 0;
            try
            {
                if (!string.IsNullOrEmpty(Request.Params["state"]))
                {
                    state = Convert.ToInt32(Request.Params["state"]);
                }

                if (!string.IsNullOrEmpty(Request.Params["district"]))
                {
                    district = Convert.ToInt32(Request.Params["district"]);

                }
                if (!string.IsNullOrEmpty(Request.Params["block"]))
                {
                    block = Convert.ToInt32(Request.Params["block"]);

                }
                if (!string.IsNullOrEmpty(Request.Params["sanction_year"]))
                {
                    sanction_year = Convert.ToInt32(Request.Params["sanction_year"]);

                }
                if (!string.IsNullOrEmpty(Request.Params["batch"]))
                {
                    batch = Convert.ToInt32(Request.Params["batch"]);

                }

                PMISDAL dalobj = new PMISDAL();
                var jsonData = new
                {
                    rows = dalobj.PMISRoadListDAL(state, district, block, sanction_year, batch, Convert.ToInt32(Request.Params["page"]) - 1, Convert.ToInt32(Request.Params["rows"]), Request.Params["sidx"], Request.Params["sord"], out totalRecords),
                    total = totalRecords <= Convert.ToInt32(Request.Params["rows"]) ? 1 : totalRecords / Convert.ToInt32(Request.Params["rows"]) + 1,
                    page = Convert.ToInt32(Request.Params["page"]),
                    records = totalRecords
                };
                return Json(jsonData, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "PMISController.PMISRoadList()");
                return null;
            }
        }
       
        
        #endregion

        #region Add/Update/View Project PLAN
        [HttpGet]
        [Audit]
        public ViewResult AddPMISRoadProjectPlan(String parameter, String hash, String key)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            PMISDAL objDAL = new PMISDAL();
            AddPlanPMISViewModel pmisViewModel = new AddPlanPMISViewModel();
            Dictionary<string, string> decryptedParameters = null;
            ViewBag.operation = "A";
            //int roadCode = Convert.ToInt32(RoadCode);    

            try
            {
                decryptedParameters = URLEncrypt.DecryptParameters1(new string[] { parameter, hash, key });
                int IMS_PR_ROAD_CODE = 0; string StateShare = String.Empty; string MordShare = String.Empty; string TotalSanctionedCost = String.Empty;
                decimal stateShare = 0; decimal mordShare = 0; decimal totalSancCost = 0;
                if (decryptedParameters.Count() > 0)
                {
                    IMS_PR_ROAD_CODE = Convert.ToInt32(decryptedParameters["RoadCode"].ToString());
                    stateShare = Math.Round(Convert.ToDecimal(decryptedParameters["StateShare"]), 2);
                    StateShare = stateShare.ToString();
                    mordShare = Math.Round(Convert.ToDecimal(decryptedParameters["MordShare"]), 2);
                    MordShare = mordShare.ToString();
                    totalSancCost = Math.Round(Convert.ToDecimal(decryptedParameters["TotalSanctionedDate"]), 2);
                    TotalSanctionedCost = totalSancCost.ToString();
                }
                else
                {
                    return null;
                }
                if (IMS_PR_ROAD_CODE != 0)
                {


                    pmisViewModel.IMS_PR_ROAD_CODE = IMS_PR_ROAD_CODE;

                    pmisViewModel.IMS_ROAD_NAME = dbContext.IMS_SANCTIONED_PROJECTS.Where(x => x.IMS_PR_ROAD_CODE == pmisViewModel.IMS_PR_ROAD_CODE).Select(x => x.IMS_ROAD_NAME).FirstOrDefault();
                    var year = dbContext.IMS_SANCTIONED_PROJECTS.Where(x => x.IMS_PR_ROAD_CODE == IMS_PR_ROAD_CODE).Select(x => x.IMS_YEAR).FirstOrDefault();
                    pmisViewModel.IMS_YEAR = year.ToString() + "-" + (year + 1).ToString().Substring(2, 2);
                    pmisViewModel.IMS_PAV_LENGTH = Math.Round(dbContext.IMS_SANCTIONED_PROJECTS.Where(x => x.IMS_PR_ROAD_CODE == pmisViewModel.IMS_PR_ROAD_CODE).Select(x => x.IMS_PAV_LENGTH).FirstOrDefault(), 2);
                    pmisViewModel.IMS_PACKAGE_ID = dbContext.IMS_SANCTIONED_PROJECTS.Where(x => x.IMS_PR_ROAD_CODE == pmisViewModel.IMS_PR_ROAD_CODE).Select(x => x.IMS_PACKAGE_ID).FirstOrDefault();
                    //        pmisViewModel.AGREEMENT_VALUE = dbContext.TEND_AGREEMENT_DETAIL.Where(x => x.IMS_PR_ROAD_CODE == pmisViewModel.IMS_PR_ROAD_CODE && x.TEND_AGREEMENT_STATUS != "W").Select(x => x.TEND_AGREEMENT_AMOUNT).FirstOrDefault();
                    var Agreement_Id = dbContext.TEND_AGREEMENT_DETAIL.Where(x => x.IMS_PR_ROAD_CODE == IMS_PR_ROAD_CODE && x.TEND_AGREEMENT_STATUS != "W").Max(x => x.TEND_AGREEMENT_ID);

                    pmisViewModel.AGREEMENT_VALUE = dbContext.TEND_AGREEMENT_DETAIL.Where(x => x.IMS_PR_ROAD_CODE == pmisViewModel.IMS_PR_ROAD_CODE && x.TEND_AGREEMENT_ID == Agreement_Id).Select(x => x.TEND_AGREEMENT_AMOUNT + x.GST_AMT_MAINT_AGREEMENT_DLP).FirstOrDefault();
                  
                    pmisViewModel.StateShare = StateShare;
                    pmisViewModel.MordShare = MordShare;
                    pmisViewModel.TotalSanctionedCost = TotalSanctionedCost;

                    pmisViewModel.StateName = PMGSYSession.Current.StateName;
                    pmisViewModel.DistrictName = PMGSYSession.Current.DistrictName;

                    // Changes by Saurabh for getting record order wise

                    pmisViewModel.Activity_Desc_List = dbContext.PMIS_ACTIVITY_MASTER.Where(x => x.ROAD_TYPE == "R").OrderBy(x => x.SORT_ORDER).Select(x => x.ACTIVITY_DESC).ToArray();
                    pmisViewModel.Activity_Unit_List = dbContext.PMIS_ACTIVITY_MASTER.Where(x => x.ROAD_TYPE == "R").OrderBy(x => x.SORT_ORDER).Select(x => x.ACTIVITY_UNIT).ToArray();
                    pmisViewModel.QUANTITY_APPL = dbContext.PMIS_ACTIVITY_MASTER.Where(x => x.ROAD_TYPE == "R").OrderBy(x => x.SORT_ORDER).Select(x => x.QUANTITY_APPL).ToArray();
                    pmisViewModel.AGRCOST_APPL = dbContext.PMIS_ACTIVITY_MASTER.Where(x => x.ROAD_TYPE == "R").OrderBy(x => x.SORT_ORDER).Select(x => x.AGRCOST_APPL).ToArray();
                    pmisViewModel.PLANNED_START_DATE_APPL = dbContext.PMIS_ACTIVITY_MASTER.Where(x => x.ROAD_TYPE == "R").OrderBy(x => x.SORT_ORDER).Select(x => x.PLANNED_START_DATE_APPL).ToArray();
                    pmisViewModel.PLANNED_DURATION_APPL = dbContext.PMIS_ACTIVITY_MASTER.Where(x => x.ROAD_TYPE == "R").OrderBy(x => x.SORT_ORDER).Select(x => x.PLANNED_DURATION_APPL).ToArray();
                    pmisViewModel.PLANNED_COMPLETION_DATE_APPL = dbContext.PMIS_ACTIVITY_MASTER.Where(x => x.ROAD_TYPE == "R").OrderBy(x => x.SORT_ORDER).Select(x => x.PLANNED_COMPLETION_DATE_APPL).ToArray();


                    //pmisViewModel.Activity_Desc_List = (from x in dbContext.PMIS_ACTIVITY_MASTER where x.ROAD_TYPE == "R" select x.ACTIVITY_DESC).ToArray();
                    //pmisViewModel.Activity_Unit_List = (from x in dbContext.PMIS_ACTIVITY_MASTER where x.ROAD_TYPE == "R" select x.ACTIVITY_UNIT).ToArray();
                    //pmisViewModel.QUANTITY_APPL = (from x in dbContext.PMIS_ACTIVITY_MASTER select x.QUANTITY_APPL).ToArray();
                    //pmisViewModel.AGRCOST_APPL = (from x in dbContext.PMIS_ACTIVITY_MASTER select x.AGRCOST_APPL).ToArray();
                    //pmisViewModel.PLANNED_START_DATE_APPL = (from x in dbContext.PMIS_ACTIVITY_MASTER select x.PLANNED_START_DATE_APPL).ToArray();
                    //pmisViewModel.PLANNED_DURATION_APPL = (from x in dbContext.PMIS_ACTIVITY_MASTER select x.PLANNED_DURATION_APPL).ToArray();
                    //pmisViewModel.PLANNED_COMPLETION_DATE_APPL = (from x in dbContext.PMIS_ACTIVITY_MASTER select x.PLANNED_COMPLETION_DATE_APPL).ToArray();

                    // Changes added by Saurabh on 08-06-2023 
                    var TechnologyList = string.Join(", ", dbContext.IMS_PROPOSAL_TECH.Where(x => x.IMS_PR_ROAD_CODE == pmisViewModel.IMS_PR_ROAD_CODE).Select(x => x.MASTER_TECHNOLOGY.MAST_TECH_NAME).ToList());
                    ViewBag.TechName = TechnologyList;
                    // Changes Ended by Saurabh

                    ViewData["ActivityList"] = pmisViewModel.Activity_Desc_List;
                }
                return View("~/Areas/PMIS/Views/PMIS/AddPMISRoadProjectPlan.cshtml", pmisViewModel);

            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "PMIS.AddPMISRoadProjectPlan()");
                return null;
            }
        }

        [HttpPost]
        [Audit]
        public ActionResult SaveRoadProjectPlan(IEnumerable<AddPlanPMISViewModel> formData)
        {
            PMISDAL objDAL = new PMISDAL();
            PMGSYEntities dbContext = new PMGSYEntities();
            try
            {
                ViewBag.operation = "A";
                if (ModelState.IsValid)
                {
                    decimal TotalAgreementCost = 0;
                    decimal AgreementValue = 0;
                    int roadCode = 0;
                    foreach (var item in formData)
                    {
                        if (item.AGREEMENT_COST != null)
                        {
                            TotalAgreementCost += Convert.ToDecimal(item.AGREEMENT_COST);
                        }

                        if (item.IMS_PR_ROAD_CODE > 0)
                        {
                            roadCode = item.IMS_PR_ROAD_CODE;
                        }

                        // Changes added by Saurabh on 08-06-2023 for FDR Development 

                        #region FDR Development 

                        if ((dbContext.IMS_PROPOSAL_TECH.Where(x => x.IMS_PR_ROAD_CODE == roadCode && x.MAST_TECH_CODE == 64).Any()))
                        {
                            if ((item.PLANNED_START_DATE == null || item.PLANNED_COMPLETION_DATE == null || item.QUANTITY == null || item.QUANTITY == 0 || item.AGREEMENT_COST == 0 || item.AGREEMENT_COST == null) && item.ACTIVITY_DESC == "FDR Stabilized Base")
                            {
                                return Json(new { Success = false, ErrorMessage = "Enter start and end dates, and Quantity,Agreement cost for FDR Stabilized Base due to Cement Stabilization Technology used on this road." });
                            }
                            if ((item.PLANNED_START_DATE == null || item.PLANNED_COMPLETION_DATE == null || item.QUANTITY == null || item.QUANTITY == 0 || item.AGREEMENT_COST == 0 || item.AGREEMENT_COST == null) && item.ACTIVITY_DESC == "Crack Relief Layer (SAMI)")
                            {
                                return Json(new { Success = false, ErrorMessage = "Enter start and end dates, and Quantity,Agreement cost for Crack Relief Layer (SAMI) due to Cement Stabilization Technology used on this road." });
                            }
                        }
                        #endregion

                        // Changes By Saurabh

                        decimal SanLength = Math.Round(dbContext.IMS_SANCTIONED_PROJECTS.Where(x => x.IMS_PR_ROAD_CODE == item.IMS_PR_ROAD_CODE).Select(x => x.IMS_PAV_LENGTH).FirstOrDefault(),2);
                     //   decimal SanLength = dbContext.IMS_SANCTIONED_PROJECTS.Where(x => x.IMS_PR_ROAD_CODE == item.IMS_PR_ROAD_CODE).Select(x => x.IMS_PAV_LENGTH).FirstOrDefault();

                        if (item.QUANTITY != null && item.QUANTITY > SanLength && item.ACTIVITY_DESC != "CD Works" && item.ACTIVITY_DESC != "Longitudinal Drains" && item.ACTIVITY_DESC != "Protection Works" && item.ACTIVITY_DESC != "Road Furniture")
                        {                    
                            return Json(new { Success = false, ErrorMessage = "For each individual activity, Quantity entered should not be more than sanctioned length." });
                        }
                        if (item.QUANTITY == null && item.AGREEMENT_COST > 0 && item.ACTIVITY_DESC != "Miscellaneous" && item.ACTIVITY_DESC != "GST")
                        {
                            return Json(new { Success = false, ErrorMessage = "Agreement cost cannot be entered for activities for which quantity has not been entered." });
                        }
                        if (item.PLANNED_START_DATE != null && item.PLANNED_COMPLETION_DATE != null && item.PLANNED_START_DATE > item.PLANNED_COMPLETION_DATE)
                        {
                            return Json(new { Success = false, ErrorMessage = "Planned Completion date cannot be before the planned start date of an activity." });
                        }
                        if ((item.PLANNED_START_DATE == null || item.PLANNED_COMPLETION_DATE == null) && item.QUANTITY != null)
                        {
                            return Json(new { Success = false, ErrorMessage = "Planned Start and End Date should be entered for activities for which quantity has been entered." });
                        }
                        if ((item.PLANNED_START_DATE == null || item.PLANNED_COMPLETION_DATE == null) && item.ACTIVITY_DESC == "Field Lab")
                        {
                            return Json(new { Success = false, ErrorMessage = "Field Lab Start and End Date should be entered." });
                        }
                    }
                    //       AgreementValue = dbContext.TEND_AGREEMENT_DETAIL.Where(x => x.IMS_PR_ROAD_CODE == roadCode && x.TEND_AGREEMENT_STATUS != "W").Select(x => x.TEND_AGREEMENT_AMOUNT).FirstOrDefault();
                    var Agreement_Id = dbContext.TEND_AGREEMENT_DETAIL.Where(x => x.IMS_PR_ROAD_CODE == roadCode && x.TEND_AGREEMENT_STATUS != "W").Max(x => x.TEND_AGREEMENT_ID);

                    AgreementValue = dbContext.TEND_AGREEMENT_DETAIL.Where(x => x.IMS_PR_ROAD_CODE == roadCode && x.TEND_AGREEMENT_ID == Agreement_Id).Select(x => x.TEND_AGREEMENT_AMOUNT + x.GST_AMT_MAINT_AGREEMENT_DLP).FirstOrDefault();
                    if (TotalAgreementCost != AgreementValue)
                    {
                        return Json(new { Success = false, ErrorMessage = "The Sum of Agreement cost of all activities should match the Agreement Value for this road" });
                    }

                    string Status = objDAL.SaveRoadProjectPlanDAL(formData);
                    if (Status == string.Empty)
                        return Json(new { Success = true });
                    else
                        return Json(new { Success = false, ErrorMessage = Status });
                }
                else
                {
                    return Json(new { Success = false, ErrorMessage = new CommonFunctions().FormatErrorMessage(ModelState) });
                }
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "SaveRoadProjectPlan(AddPlanPMISViewModel formData))");
                throw ex;
            }
        }
        [HttpGet]
        [Audit]
        public ActionResult UpdatePMISRoadProjectPlanLayout(String parameter, String hash, String key)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            PMISDAL objlDAL = new PMISDAL();
            CommonFunctions objCommonFuntion = new CommonFunctions();
            Dictionary<string, string> decryptedParameters = null;
            string status = string.Empty;

            try
            {
                decryptedParameters = URLEncrypt.DecryptParameters1(new string[] { parameter, hash, key });
                int IMS_PR_ROAD_CODE = 0; string StateShare = String.Empty; string MordShare = String.Empty; string TotalSanctionedCost = String.Empty;
                decimal stateShare = 0; decimal mordShare = 0; decimal totalSancCost = 0;
                if (decryptedParameters.Count() > 0)
                {
                    IMS_PR_ROAD_CODE = Convert.ToInt32(decryptedParameters["RoadCode"].ToString());
                    stateShare = Math.Round(Convert.ToDecimal(decryptedParameters["StateShare"]), 2);
                    StateShare = stateShare.ToString();
                    mordShare = Math.Round(Convert.ToDecimal(decryptedParameters["MordShare"]), 2);
                    MordShare = mordShare.ToString();
                    totalSancCost = Math.Round(Convert.ToDecimal(decryptedParameters["TotalSanctionedDate"]), 2);
                    TotalSanctionedCost = totalSancCost.ToString();
                }
                else
                {
                    return Json(new { success = false, errorMessage = "Invalid Road Code." });
                }
                ViewBag.operation = "U";

                if (!dbContext.PMIS_PLAN_MASTER.Any(m => m.IMS_PR_ROAD_CODE == IMS_PR_ROAD_CODE))
                {
                    return Json(new { Success = false, ErrorMessage = "Plan Not Found" }, JsonRequestBehavior.AllowGet);
                }
                List<AddPlanPMISViewModel> objlist = new List<AddPlanPMISViewModel>();

                var latestRecord = dbContext.PMIS_PLAN_DETAILS.Where(x => x.IMS_PR_ROAD_CODE == IMS_PR_ROAD_CODE & x.IS_LATEST == "Y").ToList();
                if (latestRecord != null)
                {
                    ViewBag.IMS_PR_ROAD_CODE = IMS_PR_ROAD_CODE;
                    ViewBag.IMS_ROAD_NAME = dbContext.IMS_SANCTIONED_PROJECTS.Where(x => x.IMS_PR_ROAD_CODE == IMS_PR_ROAD_CODE).Select(x => x.IMS_ROAD_NAME).FirstOrDefault();
                    var year = dbContext.IMS_SANCTIONED_PROJECTS.Where(x => x.IMS_PR_ROAD_CODE == IMS_PR_ROAD_CODE).Select(x => x.IMS_YEAR).FirstOrDefault();
                    ViewBag.IMS_YEAR = year.ToString() + "-" + (year + 1).ToString().Substring(2, 2);
                    ViewBag.IMS_PAV_LENGTH = Math.Round(dbContext.IMS_SANCTIONED_PROJECTS.Where(x => x.IMS_PR_ROAD_CODE == IMS_PR_ROAD_CODE).Select(x => x.IMS_PAV_LENGTH).FirstOrDefault(), 2);
                    ViewBag.IMS_PACKAGE_ID = dbContext.IMS_SANCTIONED_PROJECTS.Where(x => x.IMS_PR_ROAD_CODE == IMS_PR_ROAD_CODE).Select(x => x.IMS_PACKAGE_ID).FirstOrDefault();
                    // Changes done on 25-02-2022 by Srishti Tyagi
                    //ViewBag.AGREEMENT_VALUE = dbContext.TEND_AGREEMENT_DETAIL.Where(x => x.IMS_PR_ROAD_CODE == IMS_PR_ROAD_CODE).Select(x => x.TEND_AGREEMENT_AMOUNT).FirstOrDefault();
                    var Agreement_Id = dbContext.TEND_AGREEMENT_DETAIL.Where(x => x.IMS_PR_ROAD_CODE == IMS_PR_ROAD_CODE && x.TEND_AGREEMENT_STATUS != "W").Max(x => x.TEND_AGREEMENT_ID);
             //       ViewBag.AGREEMENT_VALUE = dbContext.TEND_AGREEMENT_DETAIL.Where(x => x.IMS_PR_ROAD_CODE == IMS_PR_ROAD_CODE && x.TEND_AGREEMENT_ID == Agreement_Id).Select(x => x.TEND_AGREEMENT_AMOUNT).FirstOrDefault();

                    ViewBag.AGREEMENT_VALUE = dbContext.TEND_AGREEMENT_DETAIL.Where(x => x.IMS_PR_ROAD_CODE == IMS_PR_ROAD_CODE && x.TEND_AGREEMENT_ID == Agreement_Id).Select(x => x.TEND_AGREEMENT_AMOUNT + x.GST_AMT_MAINT_AGREEMENT_DLP).FirstOrDefault();

                    ViewBag.StateName = PMGSYSession.Current.StateName;
                    ViewBag.DistrictName = PMGSYSession.Current.DistrictName;
                    ViewBag.StateShare = StateShare;
                    ViewBag.MordShare = MordShare;
                    ViewBag.TotalSanctionedCost = TotalSanctionedCost;
                    ViewBag.BASELINE_NO = (from x in dbContext.PMIS_PLAN_MASTER where x.IMS_PR_ROAD_CODE == IMS_PR_ROAD_CODE && x.IS_LATEST == "Y" select x.BASELINE_NO).FirstOrDefault();
                    DateTime StartDate = ViewBag.TotalPlannedStartDate = dbContext.PMIS_PLAN_DETAILS.Where(x => x.IMS_PR_ROAD_CODE == IMS_PR_ROAD_CODE & x.IS_LATEST == "Y").Select(x => x.PLANNED_START_DATE).Min();
                    DateTime EndDate = ViewBag.TotalPlannedCompletion = dbContext.PMIS_PLAN_DETAILS.Where(x => x.IMS_PR_ROAD_CODE == IMS_PR_ROAD_CODE & x.IS_LATEST == "Y").Select(x => x.PLANNED_COMPLETION_DATE).Max();
                    ViewBag.TotalPlannedDuration = (EndDate - StartDate).Days;
                    Decimal? TotalCost = 0;

                    foreach (var item in latestRecord)
                    {
                        AddPlanPMISViewModel objPmis = new AddPlanPMISViewModel();

                        objPmis.ACTIVITY_DESC = dbContext.PMIS_ACTIVITY_MASTER.Where(y => y.ACTIVITY_ID == item.ACTIVITY_ID).Select(c => c.ACTIVITY_DESC).FirstOrDefault();
                        objPmis.ACTIVITY_UNIT = dbContext.PMIS_ACTIVITY_MASTER.Where(y => y.ACTIVITY_ID == item.ACTIVITY_ID).Select(c => c.ACTIVITY_UNIT).FirstOrDefault();
                        objPmis.SORT_ORDER = dbContext.PMIS_ACTIVITY_MASTER.Where(y => y.ACTIVITY_ID == item.ACTIVITY_ID).Select(x => x.SORT_ORDER).FirstOrDefault();
                        // above line change
                        objPmis.QUANTITY = item.QUANTITY;
                        objPmis.AGREEMENT_COST = Math.Round(Convert.ToDecimal(item.AGREEMENT_COST), 2);
                        objPmis.PLANNED_START_DATE = item.PLANNED_START_DATE;
                        objPmis.PLANNED_DURATION = item.PLANNED_DURATION;
                        objPmis.PLANNED_COMPLETION_DATE = item.PLANNED_COMPLETION_DATE;

                        objPmis.QUANTITY_APPL_U = dbContext.PMIS_ACTIVITY_MASTER.Where(y => y.ACTIVITY_ID == item.ACTIVITY_ID).Select(c => c.QUANTITY_APPL).FirstOrDefault();
                        objPmis.AGRCOST_APPL_U = dbContext.PMIS_ACTIVITY_MASTER.Where(y => y.ACTIVITY_ID == item.ACTIVITY_ID).Select(c => c.AGRCOST_APPL).FirstOrDefault();
                        objPmis.PLANNED_START_DATE_APPL_U = dbContext.PMIS_ACTIVITY_MASTER.Where(y => y.ACTIVITY_ID == item.ACTIVITY_ID).Select(c => c.PLANNED_START_DATE_APPL).FirstOrDefault();
                        objPmis.PLANNED_DURATION_APPL_U = dbContext.PMIS_ACTIVITY_MASTER.Where(y => y.ACTIVITY_ID == item.ACTIVITY_ID).Select(c => c.PLANNED_DURATION_APPL).FirstOrDefault();
                        objPmis.PLANNED_COMPLETION_DATE_APPL_U = dbContext.PMIS_ACTIVITY_MASTER.Where(y => y.ACTIVITY_ID == item.ACTIVITY_ID).Select(c => c.PLANNED_COMPLETION_DATE_APPL).FirstOrDefault();

                        objlist.Add(objPmis);
                    }

                    if (!(dbContext.PMIS_PLAN_DETAILS.Any(s => s.IMS_PR_ROAD_CODE == IMS_PR_ROAD_CODE && s.ACTIVITY_ID == 41) && dbContext.PMIS_PLAN_DETAILS.Any(s => s.IMS_PR_ROAD_CODE == IMS_PR_ROAD_CODE && s.ACTIVITY_ID == 42)))
                    {
                        AddPlanPMISViewModel objPmis1 = new AddPlanPMISViewModel();
                        objPmis1.ACTIVITY_DESC = dbContext.PMIS_ACTIVITY_MASTER.Where(y => y.ACTIVITY_ID == 41).Select(c => c.ACTIVITY_DESC).FirstOrDefault();
                        objPmis1.ACTIVITY_UNIT = dbContext.PMIS_ACTIVITY_MASTER.Where(y => y.ACTIVITY_ID == 41).Select(c => c.ACTIVITY_UNIT).FirstOrDefault();
                        objPmis1.SORT_ORDER = dbContext.PMIS_ACTIVITY_MASTER.Where(y => y.ACTIVITY_ID == 41).Select(x => x.SORT_ORDER).FirstOrDefault();
                        // above line change
                        objPmis1.QUANTITY_APPL_U = dbContext.PMIS_ACTIVITY_MASTER.Where(y => y.ACTIVITY_ID == 41).Select(c => c.QUANTITY_APPL).FirstOrDefault();
                        objPmis1.AGRCOST_APPL_U = dbContext.PMIS_ACTIVITY_MASTER.Where(y => y.ACTIVITY_ID == 41).Select(c => c.AGRCOST_APPL).FirstOrDefault();
                        objPmis1.PLANNED_START_DATE_APPL_U = dbContext.PMIS_ACTIVITY_MASTER.Where(y => y.ACTIVITY_ID == 41).Select(c => c.PLANNED_START_DATE_APPL).FirstOrDefault();
                        objPmis1.PLANNED_DURATION_APPL_U = dbContext.PMIS_ACTIVITY_MASTER.Where(y => y.ACTIVITY_ID == 41).Select(c => c.PLANNED_DURATION_APPL).FirstOrDefault();
                        objPmis1.PLANNED_COMPLETION_DATE_APPL_U = dbContext.PMIS_ACTIVITY_MASTER.Where(y => y.ACTIVITY_ID == 41).Select(c => c.PLANNED_COMPLETION_DATE_APPL).FirstOrDefault();
                        objlist.Add(objPmis1);
                        AddPlanPMISViewModel objPmis2 = new AddPlanPMISViewModel();
                        objPmis2.ACTIVITY_DESC = dbContext.PMIS_ACTIVITY_MASTER.Where(y => y.ACTIVITY_ID == 42).Select(c => c.ACTIVITY_DESC).FirstOrDefault();
                        objPmis2.ACTIVITY_UNIT = dbContext.PMIS_ACTIVITY_MASTER.Where(y => y.ACTIVITY_ID == 42).Select(c => c.ACTIVITY_UNIT).FirstOrDefault();
                        objPmis2.SORT_ORDER = dbContext.PMIS_ACTIVITY_MASTER.Where(y => y.ACTIVITY_ID == 42).Select(x => x.SORT_ORDER).FirstOrDefault();
                        // above line change
                        objPmis2.QUANTITY_APPL_U = dbContext.PMIS_ACTIVITY_MASTER.Where(y => y.ACTIVITY_ID == 42).Select(c => c.QUANTITY_APPL).FirstOrDefault();
                        objPmis2.AGRCOST_APPL_U = dbContext.PMIS_ACTIVITY_MASTER.Where(y => y.ACTIVITY_ID == 42).Select(c => c.AGRCOST_APPL).FirstOrDefault();
                        objPmis2.PLANNED_START_DATE_APPL_U = dbContext.PMIS_ACTIVITY_MASTER.Where(y => y.ACTIVITY_ID == 42).Select(c => c.PLANNED_START_DATE_APPL).FirstOrDefault();
                        objPmis2.PLANNED_DURATION_APPL_U = dbContext.PMIS_ACTIVITY_MASTER.Where(y => y.ACTIVITY_ID == 42).Select(c => c.PLANNED_DURATION_APPL).FirstOrDefault();
                        objPmis2.PLANNED_COMPLETION_DATE_APPL_U = dbContext.PMIS_ACTIVITY_MASTER.Where(y => y.ACTIVITY_ID == 42).Select(c => c.PLANNED_COMPLETION_DATE_APPL).FirstOrDefault();
                        objlist.Add(objPmis2);
                    }
                    objlist = objlist.OrderBy(x => x.SORT_ORDER).ToList();

                    foreach (var model in objlist)
                    {
                        if (model.AGREEMENT_COST != null)
                        {
                            TotalCost = TotalCost + model.AGREEMENT_COST;
                        }
                    }
                    ViewBag.TotalAgreementCost = TotalCost;
                    // Changes added by Saurabh on 08-06-2023 for FDR Development
                    var TechnologyList = string.Join(", ", dbContext.IMS_PROPOSAL_TECH.Where(x => x.IMS_PR_ROAD_CODE == IMS_PR_ROAD_CODE).Select(x => x.MASTER_TECHNOLOGY.MAST_TECH_NAME).ToList());
                    ViewBag.TechName = TechnologyList;
                    // Changes ended by Saurabh 

                }
                else
                {
                    return Json(new { Success = false, ErrorMessage = "Plan Details Not Found" }, JsonRequestBehavior.AllowGet);
                }

                return View("~/Areas/PMIS/Views/PMIS/UpdatePMISRoadProjectPlanLayout.cshtml", objlist);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "UpdatePMISRoadProjectPlanLayout()");
                return null;
            }
        }

        [HttpPost]
        [Audit]
        public ActionResult UpdatePMISRoadProjectPlan(IEnumerable<AddPlanPMISViewModel> formData)
        {
            PMISDAL objDAL = new PMISDAL();
            PMGSYEntities dbContext = new PMGSYEntities();
            try
            {
                ViewBag.Operation = "U";
                if (formData != null)
                {
                    decimal TotalAgreementCost = 0;
                    decimal AgreementValue = 0;
                    int roadCode = 0;
                    foreach (var item in formData)
                    {
                        if (item.AGREEMENT_COST != null)
                        {
                            TotalAgreementCost += Convert.ToDecimal(item.AGREEMENT_COST);
                        }

                        if (item.IMS_PR_ROAD_CODE > 0)
                        {
                            roadCode = item.IMS_PR_ROAD_CODE;
                        }

                        // Changes added by Saurabh on 08-06-2023 for new FDR Development

                        #region New FDR Development
                        if ((dbContext.IMS_PROPOSAL_TECH.Where(x => x.IMS_PR_ROAD_CODE == roadCode && x.MAST_TECH_CODE == 64).Any()))
                        {
                            if ((item.PLANNED_START_DATE == null || item.PLANNED_COMPLETION_DATE == null || item.QUANTITY == null || item.QUANTITY == 0 || item.AGREEMENT_COST == 0 || item.AGREEMENT_COST == null) && item.ACTIVITY_DESC == "FDR Stabilized Base")
                            {
                                return Json(new { Success = false, ErrorMessage = "Enter start and end dates, and Quantity,Agreement cost for FDR Stabilized Base due to Cement Stabilization Technology used on this road." });
                            }
                            if ((item.PLANNED_START_DATE == null || item.PLANNED_COMPLETION_DATE == null || item.QUANTITY == null || item.QUANTITY == 0 || item.AGREEMENT_COST == 0 || item.AGREEMENT_COST == null) && item.ACTIVITY_DESC == "Crack Relief Layer (SAMI)")
                            {
                                return Json(new { Success = false, ErrorMessage = "Enter start and end dates, and Quantity,Agreement cost for Crack Relief Layer (SAMI) due to Cement Stabilization Technology used on this road." });
                            }
                        }
                        #endregion

                        // Changes by Saurabh ended here..

                        //   decimal SanLength = Math.Round(dbContext.IMS_SANCTIONED_PROJECTS.Where(x => x.IMS_PR_ROAD_CODE == item.IMS_PR_ROAD_CODE).Select(x => x.IMS_PAV_LENGTH).FirstOrDefault(), 2);

                        decimal SanLength = Math.Round(dbContext.IMS_SANCTIONED_PROJECTS.Where(x => x.IMS_PR_ROAD_CODE == item.IMS_PR_ROAD_CODE).Select(x => x.IMS_PAV_LENGTH).FirstOrDefault(),2);
                        if (item.QUANTITY != null && item.QUANTITY > SanLength && item.ACTIVITY_DESC != "CD Works" && item.ACTIVITY_DESC != "Longitudinal Drains" && item.ACTIVITY_DESC != "Protection Works" && item.ACTIVITY_DESC != "Road Furniture")
                        {
                            return Json(new { Success = false, ErrorMessage = "For each individual activity, Quantity entered should not be more than sanctioned length." });
                        }
                        if (item.QUANTITY == null && item.AGREEMENT_COST > 0 && item.ACTIVITY_DESC != "Miscellaneous" && item.ACTIVITY_DESC != "GST")
                        {
                            return Json(new { Success = false, ErrorMessage = "Agreement cost cannot be entered for activities for which quantity has not been entered." });
                        }
                        if (item.PLANNED_START_DATE != null && item.PLANNED_COMPLETION_DATE != null && item.PLANNED_START_DATE > item.PLANNED_COMPLETION_DATE)
                        {
                            return Json(new { Success = false, ErrorMessage = "Planned Completion date cannot be before the planned start date of an activity." });
                        }
                        if ((item.PLANNED_START_DATE == null || item.PLANNED_COMPLETION_DATE == null) && item.QUANTITY != null)
                        {
                            return Json(new { Success = false, ErrorMessage = "Planned Start and End Date should be entered for activities for which quantity has been entered." });
                        }
                        if ((item.PLANNED_START_DATE == null || item.PLANNED_COMPLETION_DATE == null) && item.ACTIVITY_DESC == "Field Lab")
                        {
                            return Json(new { Success = false, ErrorMessage = "Field Lab Start and End Date should be entered." });
                        }
                    }
                    //     AgreementValue = dbContext.TEND_AGREEMENT_DETAIL.Where(x => x.IMS_PR_ROAD_CODE == roadCode && x.TEND_AGREEMENT_STATUS != "W").Select(x => x.TEND_AGREEMENT_AMOUNT).FirstOrDefault();

                    var Agreement_Id = dbContext.TEND_AGREEMENT_DETAIL.Where(x => x.IMS_PR_ROAD_CODE == roadCode && x.TEND_AGREEMENT_STATUS != "W").Max(x => x.TEND_AGREEMENT_ID);

                    AgreementValue = dbContext.TEND_AGREEMENT_DETAIL.Where(x => x.IMS_PR_ROAD_CODE == roadCode && x.TEND_AGREEMENT_ID == Agreement_Id).Select(x => x.TEND_AGREEMENT_AMOUNT + x.GST_AMT_MAINT_AGREEMENT_DLP).FirstOrDefault();
                    if (TotalAgreementCost != AgreementValue)
                    {
                        return Json(new { Success = false, ErrorMessage = "The Sum of Agreement cost of all activities should match the Agreement Value for this road" });
                    }
                    string Status = objDAL.UpdatePMISRoadProjectPlanDAL(formData);
                    if (Status == string.Empty)
                        return Json(new { Success = true });
                    else
                        return Json(new { Success = false, ErrorMessage = Status });
                }
                else
                {
                    return Json(new { Success = false, ErrorMessage = "Update Unsuccessful" });
                }

            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "UpdatePMISRoadProjectPlan(IEnumerable<AddPlanPMISViewModel> formData)");
                throw ex;
            }
        }

        [HttpGet]
        [Audit]
        public ActionResult ViewPMISRoadProjectPlanLayout(String parameter, String hash, String key)
        {
            string status = string.Empty;
            PMISDAL objDAL = new PMISDAL();
            PMGSYEntities dbContext = new PMGSYEntities();
            Dictionary<string, string> decryptedParameters = null;
            try
            {
                decryptedParameters = URLEncrypt.DecryptParameters1(new string[] { parameter, hash, key });
                int IMS_PR_ROAD_CODE = 0; string StateShare = String.Empty; string MordShare = String.Empty; string TotalSanctionedCost = String.Empty;
                decimal stateShare = 0; decimal mordShare = 0; decimal totalSancCost = 0;
                if (decryptedParameters.Count() > 0)
                {

                    IMS_PR_ROAD_CODE = Convert.ToInt32(decryptedParameters["RoadCode"].ToString());
                    stateShare = Math.Round(Convert.ToDecimal(decryptedParameters["StateShare"]), 2);
                    StateShare = stateShare.ToString();
                    mordShare = Math.Round(Convert.ToDecimal(decryptedParameters["MordShare"]), 2);
                    MordShare = mordShare.ToString();
                    totalSancCost = Math.Round(Convert.ToDecimal(decryptedParameters["TotalSanctionedDate"]), 2);
                    TotalSanctionedCost = totalSancCost.ToString();
                }
                else
                {
                    return Json(new { success = false, errorMessage = "Invalid Road Code." });
                }


                List<AddPlanPMISViewModel> objlist = new List<AddPlanPMISViewModel>();
                decimal? SumAgreementCost = 0;
                var viewlatest = dbContext.PMIS_PLAN_DETAILS.Where(x => x.IMS_PR_ROAD_CODE == IMS_PR_ROAD_CODE & x.IS_LATEST == "Y").ToList();
                if (viewlatest != null)
                {

                    foreach (var item in viewlatest)
                    {
                        AddPlanPMISViewModel objPmis = new AddPlanPMISViewModel();
                        objPmis.IMS_PR_ROAD_CODE = IMS_PR_ROAD_CODE;
                        objPmis.StateName = PMGSYSession.Current.StateName;
                        objPmis.ACTIVITY_DESC = dbContext.PMIS_ACTIVITY_MASTER.Where(y => y.ACTIVITY_ID == item.ACTIVITY_ID).Select(c => c.ACTIVITY_DESC).FirstOrDefault();
                        objPmis.ACTIVITY_UNIT = dbContext.PMIS_ACTIVITY_MASTER.Where(y => y.ACTIVITY_ID == item.ACTIVITY_ID).Select(c => c.ACTIVITY_UNIT).FirstOrDefault();
                        objPmis.QUANTITY = item.QUANTITY;
                        objPmis.AGREEMENT_COST = item.AGREEMENT_COST;
                        objPmis.PLANNED_START_DATE = item.PLANNED_START_DATE;
                        objPmis.View_Planned_Start_Date = item.PLANNED_START_DATE == null ? " " : item.PLANNED_START_DATE.Value.ToShortDateString(); //.Value.ToShortDateString();
                        objPmis.PLANNED_DURATION = item.PLANNED_DURATION;
                        objPmis.View_Planned_Completion_Date = item.PLANNED_COMPLETION_DATE == null ? " " : item.PLANNED_COMPLETION_DATE.Value.ToShortDateString();
                        objPmis.PLANNED_COMPLETION_DATE = item.PLANNED_COMPLETION_DATE;
                        if (objPmis.AGREEMENT_COST != null)
                            SumAgreementCost = SumAgreementCost + objPmis.AGREEMENT_COST;

                        objlist.Add(objPmis);
                    }

                    ViewBag.RoadName = dbContext.IMS_SANCTIONED_PROJECTS.Where(x => x.IMS_PR_ROAD_CODE == IMS_PR_ROAD_CODE).Select(x => x.IMS_ROAD_NAME).FirstOrDefault();
                    //ViewBag.SanctionedYear = dbContext.IMS_SANCTIONED_PROJECTS.Where(x => x.IMS_PR_ROAD_CODE == IMS_PR_ROAD_CODE).Select(x => x.IMS_YEAR).FirstOrDefault();
                    var year = dbContext.IMS_SANCTIONED_PROJECTS.Where(x => x.IMS_PR_ROAD_CODE == IMS_PR_ROAD_CODE).Select(x => x.IMS_YEAR).FirstOrDefault();
                    ViewBag.SanctionedYear = year.ToString() + "-" + (year + 1).ToString().Substring(2, 2);
                    ViewBag.SanctionedLength = Math.Round(dbContext.IMS_SANCTIONED_PROJECTS.Where(x => x.IMS_PR_ROAD_CODE == IMS_PR_ROAD_CODE).Select(x => x.IMS_PAV_LENGTH).FirstOrDefault(), 2);
                    ViewBag.PackageNo = dbContext.IMS_SANCTIONED_PROJECTS.Where(x => x.IMS_PR_ROAD_CODE == IMS_PR_ROAD_CODE).Select(x => x.IMS_PACKAGE_ID).FirstOrDefault();
                    //   ViewBag.AgreementValue = dbContext.TEND_AGREEMENT_DETAIL.Where(x => x.IMS_PR_ROAD_CODE == IMS_PR_ROAD_CODE && x.TEND_AGREEMENT_STATUS != "W").Select(x => x.TEND_AGREEMENT_AMOUNT).FirstOrDefault();

                    var Agreement_Id = dbContext.TEND_AGREEMENT_DETAIL.Where(x => x.IMS_PR_ROAD_CODE == IMS_PR_ROAD_CODE && x.TEND_AGREEMENT_STATUS != "W").Max(x => x.TEND_AGREEMENT_ID);

                    //  ViewBag.AgreementValue = dbContext.TEND_AGREEMENT_DETAIL.Where(x => x.IMS_PR_ROAD_CODE == IMS_PR_ROAD_CODE && x.TEND_AGREEMENT_STATUS != "W").Select(x => x.TEND_AGREEMENT_AMOUNT + x.GST_AMT_MAINT_AGREEMENT_DLP).FirstOrDefault();

                    ViewBag.AgreementValue = dbContext.TEND_AGREEMENT_DETAIL.Where(x => x.IMS_PR_ROAD_CODE == IMS_PR_ROAD_CODE && x.TEND_AGREEMENT_ID == Agreement_Id).Select(x => x.TEND_AGREEMENT_AMOUNT + x.GST_AMT_MAINT_AGREEMENT_DLP).FirstOrDefault();
                    ViewBag.RoadPlanVersions = dbContext.PMIS_PLAN_MASTER.Where(x => x.IMS_PR_ROAD_CODE == IMS_PR_ROAD_CODE).Select(x => x.BASELINE_NO).ToList();
                    ViewBag.CurrentRoadPlaneVersion = dbContext.PMIS_PLAN_MASTER.Where(x => x.IMS_PR_ROAD_CODE == IMS_PR_ROAD_CODE & x.IS_LATEST == "Y").Select(x => x.BASELINE_NO).FirstOrDefault();
                    ViewBag.StateShare = StateShare;
                    ViewBag.MordShare = MordShare;
                    ViewBag.TotalSanctionedCost = TotalSanctionedCost;

                    ViewBag.TotalAgreementCost = SumAgreementCost;
                    DateTime StartDate = ViewBag.TotalPlannedStartDate = dbContext.PMIS_PLAN_DETAILS.Where(x => x.IMS_PR_ROAD_CODE == IMS_PR_ROAD_CODE & x.IS_LATEST == "Y").Select(x => x.PLANNED_START_DATE).Min();
                    DateTime EndDate = ViewBag.TotalPlannedCompletion = dbContext.PMIS_PLAN_DETAILS.Where(x => x.IMS_PR_ROAD_CODE == IMS_PR_ROAD_CODE & x.IS_LATEST == "Y").Select(x => x.PLANNED_COMPLETION_DATE).Max();
                    ViewBag.TotalPlannedDuration = (EndDate - StartDate).Days;

                    // Changes added by Saurabh on 08-06-2023 for new FDR Development
                    var TechnologyList = string.Join(", ", dbContext.IMS_PROPOSAL_TECH.Where(x => x.IMS_PR_ROAD_CODE == IMS_PR_ROAD_CODE).Select(x => x.MASTER_TECHNOLOGY.MAST_TECH_NAME).ToList());
                    ViewBag.TechName = TechnologyList;
                    // Changes by Saurabh Ended Here
                }
                else
                {
                    return Json(new { Success = false, ErrorMessage = "Plan Details Not Found" }, JsonRequestBehavior.AllowGet);
                }

                return PartialView("~/Areas/PMIS/Views/PMIS/ViewPMISRoadProjectPlanLayout.cshtml", objlist);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "ViewPMISRoadProjectPlanLayout()");
                return null;
            }
        }

        [HttpGet]
        [Audit]
        public ActionResult ViewPMISRoadProjectPlan(String RoadName, String baseline)
        {
            string status = string.Empty;
            PMISDAL objDAL = new PMISDAL();
            PMGSYEntities dbContext = new PMGSYEntities();
            try
            {
                int RoadCode = 0;
                int BaseLine = 0;
                if (RoadName != null)
                {
                    RoadCode = dbContext.IMS_SANCTIONED_PROJECTS.Where(x => x.IMS_ROAD_NAME == RoadName).Select(x => x.IMS_PR_ROAD_CODE).FirstOrDefault();
                    BaseLine = Convert.ToInt32(baseline);


                    List<AddPlanPMISViewModel> objlist = new List<AddPlanPMISViewModel>();
                    decimal? SumAgreementCost = 0;
                    var viewlatest = dbContext.PMIS_PLAN_DETAILS.Where(x => x.IMS_PR_ROAD_CODE == RoadCode & x.BASELINE_NO == BaseLine).ToList();
                    if (viewlatest != null)
                    {

                        foreach (var item in viewlatest)
                        {
                            AddPlanPMISViewModel objPmis = new AddPlanPMISViewModel();
                            objPmis.IMS_PR_ROAD_CODE = RoadCode;
                            objPmis.StateName = PMGSYSession.Current.StateName;
                            objPmis.ACTIVITY_DESC = dbContext.PMIS_ACTIVITY_MASTER.Where(y => y.ACTIVITY_ID == item.ACTIVITY_ID).Select(c => c.ACTIVITY_DESC).FirstOrDefault();
                            objPmis.ACTIVITY_UNIT = dbContext.PMIS_ACTIVITY_MASTER.Where(y => y.ACTIVITY_ID == item.ACTIVITY_ID).Select(c => c.ACTIVITY_UNIT).FirstOrDefault();
                            objPmis.QUANTITY = item.QUANTITY;
                            objPmis.AGREEMENT_COST = item.AGREEMENT_COST;
                            objPmis.PLANNED_START_DATE = item.PLANNED_START_DATE;
                            objPmis.View_Planned_Start_Date = item.PLANNED_START_DATE == null ? " " : item.PLANNED_START_DATE.Value.ToShortDateString();
                            objPmis.PLANNED_DURATION = item.PLANNED_DURATION;
                            objPmis.View_Planned_Completion_Date = item.PLANNED_COMPLETION_DATE == null ? " " : item.PLANNED_COMPLETION_DATE.Value.ToShortDateString();
                            objPmis.PLANNED_COMPLETION_DATE = item.PLANNED_COMPLETION_DATE;


                            objlist.Add(objPmis);
                        }

                        ViewBag.RoadName = dbContext.IMS_SANCTIONED_PROJECTS.Where(x => x.IMS_PR_ROAD_CODE == RoadCode).Select(x => x.IMS_ROAD_NAME).FirstOrDefault();
                        //ViewBag.SanctionedYear = dbContext.IMS_SANCTIONED_PROJECTS.Where(x => x.IMS_PR_ROAD_CODE == RoadCode).Select(x => x.IMS_YEAR).FirstOrDefault();
                        var year = dbContext.IMS_SANCTIONED_PROJECTS.Where(x => x.IMS_PR_ROAD_CODE == RoadCode).Select(x => x.IMS_YEAR).FirstOrDefault();
                        ViewBag.SanctionedYear = year.ToString() + "-" + (year + 1).ToString().Substring(2, 2);
                        ViewBag.SanctionedLength = dbContext.IMS_SANCTIONED_PROJECTS.Where(x => x.IMS_PR_ROAD_CODE == RoadCode).Select(x => x.IMS_PAV_LENGTH).FirstOrDefault();
                        ViewBag.PackageNo = dbContext.IMS_SANCTIONED_PROJECTS.Where(x => x.IMS_PR_ROAD_CODE == RoadCode).Select(x => x.IMS_PACKAGE_ID).FirstOrDefault();

                        //    ViewBag.AgreementValue = dbContext.TEND_AGREEMENT_DETAIL.Where(x => x.IMS_PR_ROAD_CODE == RoadCode).Select(x => x.TEND_AGREEMENT_AMOUNT).FirstOrDefault();
                        var Agreement_Id = dbContext.TEND_AGREEMENT_DETAIL.Where(x => x.IMS_PR_ROAD_CODE == RoadCode && x.TEND_AGREEMENT_STATUS != "W").Max(x => x.TEND_AGREEMENT_ID);

                        ViewBag.AgreementValue = dbContext.TEND_AGREEMENT_DETAIL.Where(x => x.IMS_PR_ROAD_CODE == RoadCode && x.TEND_AGREEMENT_ID == Agreement_Id).Select(x => x.TEND_AGREEMENT_AMOUNT + x.GST_AMT_MAINT_AGREEMENT_DLP).FirstOrDefault();

                        ViewBag.RoadPlanVersions = dbContext.PMIS_PLAN_MASTER.Where(x => x.IMS_PR_ROAD_CODE == RoadCode).Select(x => x.BASELINE_NO).ToList();
                        ViewBag.CurrentRoadPlaneVersion = BaseLine;


                        ViewBag.TotalAgreementCost = SumAgreementCost;
                        DateTime StartDate = ViewBag.TotalPlannedStartDate = dbContext.PMIS_PLAN_DETAILS.Where(x => x.IMS_PR_ROAD_CODE == RoadCode & x.IS_LATEST == "Y").Select(x => x.PLANNED_START_DATE).Min();
                        DateTime EndDate = ViewBag.TotalPlannedCompletion = dbContext.PMIS_PLAN_DETAILS.Where(x => x.IMS_PR_ROAD_CODE == RoadCode & x.IS_LATEST == "Y").Select(x => x.PLANNED_COMPLETION_DATE).Max();
                        ViewBag.TotalPlannedDuration = (EndDate - StartDate).Days;

                        // Changes added by Saurabh on 08-06-2023 for new FDR Development
                        var TechnologyList = string.Join(", ", dbContext.IMS_PROPOSAL_TECH.Where(x => x.IMS_PR_ROAD_CODE == RoadCode).Select(x => x.MASTER_TECHNOLOGY.MAST_TECH_NAME).ToList());
                        ViewBag.TechName = TechnologyList;
                        // Changes by Saurabh Ended Here
                    }
                    else
                    {
                        return Json(new { Success = false, ErrorMessage = "Plan Details Not Found" }, JsonRequestBehavior.AllowGet);
                    }

                    return PartialView("~/Areas/PMIS/Views/PMIS/ViewPMISRoadProjectPlanLayout.cshtml", objlist);
                }
                else
                {
                    return Json(new { success = false, errorMessage = "Invalid Road Code." });
                }
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "ViewPMISRoadProjectPlanLayout()");
                return null;
            }
        }

        #endregion

        #region Delete PMIS Project PLAN

        [HttpPost]
        [Audit]
        public JsonResult DeletePmisRoadProjectPlan(String parameter, String hash, String key)
        {
            string status = string.Empty;
            PMISDAL objDAL = new PMISDAL();
            PMGSYEntities dbContext = new PMGSYEntities();
            Dictionary<string, string> decryptedParameters = null;
            try
            {
                decryptedParameters = URLEncrypt.DecryptParameters1(new string[] { parameter, hash, key });
                int IMS_PR_ROAD_CODE = 0;
                int PLAN_ID = 0;
                if (decryptedParameters.Count() > 0)
                {

                    IMS_PR_ROAD_CODE = Convert.ToInt32(decryptedParameters["RoadCode"].ToString());
                    PLAN_ID = Convert.ToInt32(decryptedParameters["PlanID"].ToString());

                }
                else
                {
                    return Json(new { success = false, errorMessage = "Invalid Road Code." });
                }

                status = objDAL.DeleteRoadProjectPlanDAL(IMS_PR_ROAD_CODE, PLAN_ID);
                if (status == string.Empty)
                {
                    return Json(new { success = true });
                }
                else
                {
                    return Json(new { success = false, errorMessage = status });
                }

            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "DeletePmisRoadProjectPlan()");
                return Json(new { success = false, errorMessage = status });
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

        #region Revise PMIS Project PLAN

        [HttpPost]
        [Audit]
        public ActionResult RevisePmisRoadProjectPlan(String parameter, String hash, String key)
        {
            string status = string.Empty;
            PMISDAL objDAL = new PMISDAL();
            PMGSYEntities dbContext = new PMGSYEntities();
            Dictionary<string, string> decryptedParameters = null;
            try
            {
                decryptedParameters = URLEncrypt.DecryptParameters1(new string[] { parameter, hash, key });
                int IMS_PR_ROAD_CODE = 0;
                if (decryptedParameters.Count() > 0)
                {

                    IMS_PR_ROAD_CODE = Convert.ToInt32(decryptedParameters["RoadCode"].ToString());

                }
                else
                {
                    return Json(new { success = false, errorMessage = "Invalid Road Code." });
                    //return null;
                }

                status = objDAL.ReviseRoadProjectPlanDAL(IMS_PR_ROAD_CODE);
                if (status == string.Empty)
                {
                    PMISController controllerobj = new PMISController();
                    // var updateobj = controllerobj.UpdatePMISRoadProjectPlanLayout(parameter, hash, key);
                    return Json(new { success = true });
                    //return null;
                }
                else
                {
                    //return null;
                    return Json(new { success = false, errorMessage = status });
                }
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "RevisePmisRoadProjectPlan()");
                return Json(new { success = false, errorMessage = status });
                //return null;
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

        #region Finalize PMIS Project PLAN

        [HttpPost]
        [Audit]
        public JsonResult FinalizePmisRoadProjectPlan(String parameter, String hash, String key)
        {
            string status = string.Empty;
            PMISDAL objDAL = new PMISDAL();
            PMGSYEntities dbContext = new PMGSYEntities();
            Dictionary<string, string> decryptedParameters = null;
            try
            {
                decryptedParameters = URLEncrypt.DecryptParameters1(new string[] { parameter, hash, key });
                int IMS_PR_ROAD_CODE = 0;
                if (decryptedParameters.Count() > 0)
                {

                    IMS_PR_ROAD_CODE = Convert.ToInt32(decryptedParameters["RoadCode"].ToString());

                }
                else
                {
                    return Json(new { success = false, errorMessage = "Invalid Road Code." });
                }

                status = objDAL.FinalizeRoadProjectPlanDAL(IMS_PR_ROAD_CODE);
                if (status == string.Empty)
                {
                    return Json(new { success = true });
                }
                else
                {
                    return Json(new { success = false, errorMessage = status });
                }
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "FinalizePmisRoadProjectPlan()");
                return Json(new { success = false, errorMessage = status });
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

        #region Add Actuals
        [HttpGet]
        [Audit]
        public ViewResult AddActuals(String parameter, String hash, String key)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            PMISDAL objDAL = new PMISDAL();
            Dictionary<string, string> decryptedParameters = null;

            // Added on 28-03-2022 by Srishti Tyagi
            string AprilMonthStartDay = ConfigurationManager.AppSettings["APRIL_MONTH_START_DAY"];   //1
            int AprilMonthStartDayValue = Convert.ToInt16(AprilMonthStartDay);

            string AprilMonthEndDay = ConfigurationManager.AppSettings["APRIL_MONTH_END_DAY"];   //5
            int AprilMonthEndDayValue = Convert.ToInt16(AprilMonthEndDay);

            string AprilMonth = ConfigurationManager.AppSettings["APRIL_MONTH"];  //4
            int AprilMonthValue = Convert.ToInt16(AprilMonth);

            DateTime FinanDate = DateTime.Now;
            int FinancialYear = FinanDate.Month >= 4 ? FinanDate.Year : FinanDate.Year;

            DateTime Conditional_Date_Value = new DateTime(FinancialYear, 3, 31, 00, 00, 00);
            DateTime CondEntryDate = new DateTime();
            Nullable<DateTime> tempDate = new DateTime();
            // Changes end here  

            try
            {
                

                decryptedParameters = URLEncrypt.DecryptParameters1(new string[] { parameter, hash, key });
                List<AddActualsViewModel> objlist = new List<AddActualsViewModel>();
                decimal? SumAgreementCost = 0;
                int IMS_PR_ROAD_CODE = 0;
                string StateShare = String.Empty; string MordShare = String.Empty; string TotalSanctionedCost = String.Empty; string AggStartDate = String.Empty; string AggEndDate = String.Empty;
                decimal stateShare = 0; decimal mordShare = 0; decimal totalSancCost = 0;

               

                if (decryptedParameters.Count() > 0)
                {
                   

                    IMS_PR_ROAD_CODE = Convert.ToInt32(decryptedParameters["RoadCode"].ToString());
                    stateShare = Math.Round(Convert.ToDecimal(decryptedParameters["StateShare"]), 2);
                    StateShare = stateShare.ToString();
                    mordShare = Math.Round(Convert.ToDecimal(decryptedParameters["MordShare"]), 2);
                    MordShare = mordShare.ToString();
                    totalSancCost = Math.Round(Convert.ToDecimal(decryptedParameters["TotalSanctionedDate"]), 2);
                    TotalSanctionedCost = totalSancCost.ToString();

                  
                    //StateShare = decryptedParameters["StateShare"].ToString();
                    //MordShare = decryptedParameters["MordShare"].ToString();
                    //TotalSanctionedCost = decryptedParameters["TotalSanctionedDate"].ToString();
                    //AggStartDate = decryptedParameters["AggStartDate"].ToString();
                    //AggEndDate = decryptedParameters["AggEndDate"].ToString();
                }
                else
                {
                   
                    return null;
                }
                if (IMS_PR_ROAD_CODE != 0)
                {
                    //if (dbContext.PMIS_PROGRESS_DETAILS.Where(x => x.IMS_PR_ROAD_CODE == IMS_PR_ROAD_CODE).Any())
                    //{
                    //    var latest_date = dbContext.PMIS_PROGRESS_DETAILS.Where(x => x.IMS_PR_ROAD_CODE == IMS_PR_ROAD_CODE).Select(x => x.ENTRY_DATE).Max();
                    //    var progress_Details = dbContext.PMIS_PROGRESS_DETAILS.Where(x => x.IMS_PR_ROAD_CODE == IMS_PR_ROAD_CODE & x.ENTRY_DATE == latest_date).ToList();
                    //    if (progress_Details != null)
                    //    { 

                    //    }
                    //}
                    //else
                    //{
                 


                    var progress_details_entry = dbContext.PMIS_PROGRESS_DETAILS.Where(x => x.IMS_PR_ROAD_CODE == IMS_PR_ROAD_CODE).Any();
                    var viewlatest = dbContext.PMIS_PLAN_DETAILS.Where(x => x.IMS_PR_ROAD_CODE == IMS_PR_ROAD_CODE & x.IS_LATEST == "Y").ToList();
                    if (viewlatest != null)
                    {
                      

                        foreach (var item in viewlatest)
                        {
                            AddActualsViewModel ActualsViewModelobj = new AddActualsViewModel();

                            

                            ActualsViewModelobj.ACTIVITY_DESC = dbContext.PMIS_ACTIVITY_MASTER.Where(y => y.ACTIVITY_ID == item.ACTIVITY_ID).Select(c => c.ACTIVITY_DESC).FirstOrDefault();
                            ActualsViewModelobj.ACTIVITY_UNIT = dbContext.PMIS_ACTIVITY_MASTER.Where(y => y.ACTIVITY_ID == item.ACTIVITY_ID).Select(c => c.ACTIVITY_UNIT).FirstOrDefault();
                            ActualsViewModelobj.QUANTITY = item.QUANTITY;
                            ActualsViewModelobj.AGREEMENT_COST = item.AGREEMENT_COST;
                            ActualsViewModelobj.PLANNED_START_DATE = item.PLANNED_START_DATE; //.Value.ToShortDateString();

                            if (progress_details_entry)
                            {
                            

                                var latest_date = //DateTime.Now;
                                    dbContext.PMIS_PROGRESS_DETAILS.Where(x => x.IMS_PR_ROAD_CODE == IMS_PR_ROAD_CODE).Select(x => x.ENTRY_DATE).Max();
                                var Date_Only = latest_date.Date;
                                var add_date = Date_Only.AddDays(1);
                                var progress_Details = dbContext.PMIS_PROGRESS_DETAILS.Where(x => x.IMS_PR_ROAD_CODE == IMS_PR_ROAD_CODE & x.ENTRY_DATE >= Date_Only & x.ENTRY_DATE < add_date & x.PLAN_ID == item.PLAN_ID & x.ACTIVITY_ID == item.ACTIVITY_ID & x.IS_LATEST == "Y").FirstOrDefault();
                                var progress_Master = dbContext.PMIS_PROGRESS_MASTER.Where(x => x.IMS_PR_ROAD_CODE == IMS_PR_ROAD_CODE & x.IS_LATEST == "Y").FirstOrDefault();
                                if (progress_Details != null && progress_Master != null)
                                {
                                    if (progress_Details.ENTRY_DATE != null)
                                    {
                                        tempDate = Convert.ToDateTime(progress_Details.ENTRY_DATE);

                                        CondEntryDate = Convert.ToDateTime((tempDate.Value.Day <= AprilMonthEndDayValue && tempDate.Value.Month == AprilMonthValue)
                                          ? Conditional_Date_Value : tempDate);
                                    }

                                    ActualsViewModelobj.ACTUAL_QUANTITY = progress_Details.PGS_QUANTITY;
                                    //var Act_StartDate = (progress_Details.ACTUAL_START_DATE ?? default(DateTime)).ToShortDateString();
                                    //var Act_FinshedDate = (progress_Details.ACTUAL_END_DATE ?? default(DateTime)).ToShortDateString();
                                    ActualsViewModelobj.STARTED_DATE = progress_Details.ACTUAL_START_DATE ?? null;  // default(DateTime);
                                    ActualsViewModelobj.FINISHED_DATE = progress_Details.ACTUAL_END_DATE ?? null;  // default(DateTime);
                                    TempData["CompletedRoadLength"] = progress_Master.COMPLETION_LENGTH;
                                    // Added on 30-03-2022 by Srishti Tyagi 
                                    //TempData["Date_of_progress_entry"] = progress_Details.ENTRY_DATE != null ? DateTime.Now : progress_Details.ENTRY_DATE;
                                    
                                    
                                    //TempData["Date_of_progress_entry"] = progress_Details.ENTRY_DATE != null ?
                                    //    (DateTime.Now.Day >= AprilMonthStartDayValue && DateTime.Now.Day <= AprilMonthEndDayValue && DateTime.Now.Month == AprilMonthValue)
                                    //    ? Conditional_Date_Value : DateTime.Now//progress_Details.ENTRY_DATE
                                    //    : DateTime.Now;

                                    //TempData["Date_of_progress_entry"] = CondEntryDate;
                                    TempData["Date_of_progress_entry"] = DateTime.Now;
                                    ActualsViewModelobj.ProjectStatus = Convert.ToChar(progress_Master.PROJECT_STATUS_);
                                    ViewBag.ProjectStatus = Convert.ToChar(progress_Master.PROJECT_STATUS_);


                                    //if (ActualsViewModelobj.STARTED_DATE == DateTime.MinValue || ActualsViewModelobj.FINISHED_DATE == DateTime.MinValue)
                                    //{
                                    //    ActualsViewModelobj.STARTED_DATE = DateTime.Now;
                                    //    ActualsViewModelobj.FINISHED_DATE = DateTime.Now;
                                    //}
                                }
                            }
                            //ActualsViewModel.STARTED = item.PLANNED_START_DATE == null ? "" : DateTime.Today.ToShortDateString();
                            //ActualsViewModel.FINISHED = item.PLANNED_START_DATE == null ? "" : DateTime.Today.ToShortDateString();
                            //if (item.PLANNED_START_DATE != null && !progress_details_entry)
                            //    {
                            //        ActualsViewModelobj.STARTED_DATE = DateTime.Now; // item.PLANNED_START_DATE == null ? null :
                            //        ActualsViewModelobj.FINISHED_DATE = DateTime.Now; //item.PLANNED_START_DATE == null ? null:
                            //    }

                           

                            ActualsViewModelobj.PLANNED_COMPLETION_DATE = item.PLANNED_COMPLETION_DATE;
                            ActualsViewModelobj.QUANTITY_APPL = dbContext.PMIS_ACTIVITY_MASTER.Where(y => y.ACTIVITY_ID == item.ACTIVITY_ID).Select(c => c.QUANTITY_APPL).FirstOrDefault();
                            ActualsViewModelobj.AGRCOST_APPL = dbContext.PMIS_ACTIVITY_MASTER.Where(y => y.ACTIVITY_ID == item.ACTIVITY_ID).Select(c => c.AGRCOST_APPL).FirstOrDefault();
                            ActualsViewModelobj.PLANNED_START_DATE_APPL = dbContext.PMIS_ACTIVITY_MASTER.Where(y => y.ACTIVITY_ID == item.ACTIVITY_ID).Select(c => c.PLANNED_START_DATE_APPL).FirstOrDefault();
                            //ActualsViewModelobj.PLANNED_DURATION_APPL = dbContext.PMIS_ACTIVITY_MASTER.Where(y => y.ACTIVITY_ID == item.ACTIVITY_ID).Select(c => c.PLANNED_DURATION_APPL).FirstOrDefault();
                            ActualsViewModelobj.PLANNED_COMPLETION_DATE_APPL = dbContext.PMIS_ACTIVITY_MASTER.Where(y => y.ACTIVITY_ID == item.ACTIVITY_ID).Select(c => c.PLANNED_COMPLETION_DATE_APPL).FirstOrDefault();
                            if (ActualsViewModelobj.AGREEMENT_COST != null)
                                SumAgreementCost = SumAgreementCost + ActualsViewModelobj.AGREEMENT_COST;
                            objlist.Add(ActualsViewModelobj);
                        }

                      


                        ViewBag.IMS_PR_ROAD_CODE = IMS_PR_ROAD_CODE;
                        ViewBag.RoadName = dbContext.IMS_SANCTIONED_PROJECTS.Where(x => x.IMS_PR_ROAD_CODE == IMS_PR_ROAD_CODE).Select(x => x.IMS_ROAD_NAME).FirstOrDefault();
                        //ViewBag.SanctionedYear = dbContext.IMS_SANCTIONED_PROJECTS.Where(x => x.IMS_PR_ROAD_CODE == IMS_PR_ROAD_CODE).Select(x => x.IMS_YEAR).FirstOrDefault();
                        var year = dbContext.IMS_SANCTIONED_PROJECTS.Where(x => x.IMS_PR_ROAD_CODE == IMS_PR_ROAD_CODE).Select(x => x.IMS_YEAR).FirstOrDefault();
                        ViewBag.SanctionedYear = year.ToString() + "-" + (year + 1).ToString().Substring(2, 2);
                        ViewBag.SanctionedLength = Math.Round(dbContext.IMS_SANCTIONED_PROJECTS.Where(x => x.IMS_PR_ROAD_CODE == IMS_PR_ROAD_CODE).Select(x => x.IMS_PAV_LENGTH).FirstOrDefault(),2);
                        ViewBag.PackageNo = dbContext.IMS_SANCTIONED_PROJECTS.Where(x => x.IMS_PR_ROAD_CODE == IMS_PR_ROAD_CODE).Select(x => x.IMS_PACKAGE_ID).FirstOrDefault();

                        //     ViewBag.AgreementValue = dbContext.TEND_AGREEMENT_DETAIL.Where(x => x.IMS_PR_ROAD_CODE == IMS_PR_ROAD_CODE && x.TEND_AGREEMENT_STATUS != "W").Select(x => x.TEND_AGREEMENT_AMOUNT).FirstOrDefault();
                        var Agreement_Id = dbContext.TEND_AGREEMENT_DETAIL.Where(x => x.IMS_PR_ROAD_CODE == IMS_PR_ROAD_CODE && x.TEND_AGREEMENT_STATUS != "W").Max(x => x.TEND_AGREEMENT_ID);

                        ViewBag.AgreementValue = dbContext.TEND_AGREEMENT_DETAIL.Where(x => x.IMS_PR_ROAD_CODE == IMS_PR_ROAD_CODE && x.TEND_AGREEMENT_ID == Agreement_Id).Select(x => x.TEND_AGREEMENT_AMOUNT + x.GST_AMT_MAINT_AGREEMENT_DLP).FirstOrDefault();

                        ViewBag.RoadPlanVersions = dbContext.PMIS_PLAN_MASTER.Where(x => x.IMS_PR_ROAD_CODE == IMS_PR_ROAD_CODE).Select(x => x.BASELINE_NO).ToList();
                        ViewBag.CurrentRoadPlaneVersion = dbContext.PMIS_PLAN_MASTER.Where(x => x.IMS_PR_ROAD_CODE == IMS_PR_ROAD_CODE & x.IS_LATEST == "Y").Select(x => x.BASELINE_NO).FirstOrDefault();
                        ViewBag.ProgressRecordedOn = DateTime.Today.ToShortDateString();
                        ViewBag.TotalAgreementCost = SumAgreementCost;
                        ViewBag.StateShare = StateShare;
                        ViewBag.MordShare = MordShare;
                        ViewBag.TotalSanctionedCost = TotalSanctionedCost;
                        ViewBag.TotalCompLength = dbContext.EXEC_ROADS_MONTHLY_STATUS.Where(x => x.IMS_PR_ROAD_CODE == IMS_PR_ROAD_CODE).Max(x => x.EXEC_COMPLETED);
                        DateTime StartDate = ViewBag.TotalPlannedStartDate = dbContext.PMIS_PLAN_DETAILS.Where(x => x.IMS_PR_ROAD_CODE == IMS_PR_ROAD_CODE & x.IS_LATEST == "Y").Select(x => x.PLANNED_START_DATE).Min();
                        DateTime EndDate = ViewBag.TotalPlannedCompletion = dbContext.PMIS_PLAN_DETAILS.Where(x => x.IMS_PR_ROAD_CODE == IMS_PR_ROAD_CODE & x.IS_LATEST == "Y").Select(x => x.PLANNED_COMPLETION_DATE).Max();

                        // Below Part is Changed by Saurabh on 08-06-2023 for FDR Development
                        var TechnologyList = string.Join(", ", dbContext.IMS_PROPOSAL_TECH.Where(x => x.IMS_PR_ROAD_CODE == IMS_PR_ROAD_CODE).Select(x => x.MASTER_TECHNOLOGY.MAST_TECH_NAME).ToList());
                        ViewBag.TechName = TechnologyList;

                        // Below Part is Changed by Saurabh on 08-06-2023 for FDR Development
                        if (!progress_details_entry)
                        {
                            TempData["CompletedRoadLength"] = null;
                            TempData["IsUpdate"] = null;
                            TempData["Date_of_progress_entry"] = DateTime.Now;
                        }
                        else
                        {
                            TempData["IsUpdate"] = 0;
                        }
                        // Changes Ended Here...
                    }
                    else
                    {
                       

                        return null;
                    }

                }
                return View("~/Areas/PMIS/Views/PMIS/AddActuals.cshtml", objlist);

            }
            catch (Exception ex)
            {


                ErrorLog.LogError(ex, "PMIS.AddActuals()");
                return null;
            }
        }


        [HttpPost]
        [Audit]
        public ActionResult SaveActuals(IEnumerable<AddActualsViewModel> formData)
        {
            PMISDAL objDAL = new PMISDAL();
            PMGSYEntities dbContext = new PMGSYEntities();
            char Project_Status = 'P';
            string IsFieldLab = string.Empty;  // change
            try
            {
                foreach (var item in formData)
                {
                    Project_Status = (item.ProjectStatus == 'C' ? 'C' : 'P');
                    string Proj_Status = Convert.ToString(Project_Status);
                    IsFieldLab = Convert.ToString(item.ProjectStatus);           // change

                    decimal? QtyLimit = item.QUANTITY == null ? 0 : (item.QUANTITY * 110) / 100;
                    if (item.ACTUAL_QUANTITY != null && item.ACTUAL_QUANTITY > QtyLimit)
                    {
                        if (item.QUANTITY == null && QtyLimit == 0)
                        {
                            return Json(new { Success = false, ErrorMessage = "Quantity executed as on date cannot be entered for the activities for which planned quantity has not been entered" });
                        }
                        return Json(new { Success = false, ErrorMessage = "Quantity executed as on date should not be more than 10% of the planned quantity" });
                    }

                    //Saurabh Changes added on 08-06-2023 for new FDR Technology Detail
                    #region Validation added for new FDR Technology Detail

                    if ((dbContext.IMS_PROPOSAL_TECH.Where(x => x.IMS_PR_ROAD_CODE == item.IMS_PR_ROAD_CODE && x.MAST_TECH_CODE == 64).Any()))
                    {
                        if ((item.STARTED_DATE == null || item.ACTUAL_QUANTITY == null) && item.ACTIVITY_DESC == "FDR Stabilized Base")  // item.FINISHED_DATE == null ||
                        {
                            return Json(new { Success = false, ErrorMessage = "Actual start date and Quantity executed is required for(FDR Stabilized Base) road projects utilizing Cement stabilization technology." });
                        }
                        else
                        {
                            if ((item.STARTED_DATE != null || item.ACTUAL_QUANTITY != null) && item.ACTIVITY_DESC == "FDR Stabilized Base")
                            {
                                if ((dbContext.PMIS_FDR_STABILIZE_CHAINAGE_WISE_DETAIL.Where(x => x.IMS_PR_ROAD_CODE == item.IMS_PR_ROAD_CODE && x.IS_LATEST == "Y").Any()))
                                {
                                    var entryDate = dbContext.PMIS_FDR_STABILIZE_CHAINAGE_WISE_DETAIL.Where(x => x.IMS_PR_ROAD_CODE == item.IMS_PR_ROAD_CODE && x.IS_LATEST == "Y").Select(x => x.ENTRY_DATE).FirstOrDefault();
                                    var currentDate = DateTime.Now;
                                    if (entryDate.Day != currentDate.Day || entryDate.Month != currentDate.Month || entryDate.Year != currentDate.Year)
                                    {
                                        return Json(new { Success = false, ErrorMessage = "Construction of FDR Stabilize base is Manadatory to fill for Current Date before progress enter for Activity FDR Stabilize Base." });
                                    }
                                }
                                else
                                {
                                    return Json(new { Success = false, ErrorMessage = "Construction of FDR Stabilize base is Manadatory to fill before progress enter for Activity FDR Stabilize Base." });
                                }
                            }
                        }


                        if ((item.STARTED_DATE == null || item.ACTUAL_QUANTITY == null) && item.ACTIVITY_DESC == "Crack Relief Layer (SAMI)")   // item.FINISHED_DATE == null ||
                        {
                            return Json(new { Success = false, ErrorMessage = "Actual start date and Quantity executed is required for(Crack Relief Layer (SAMI)) road projects utilizing Cement stabilization technology." });
                        }
                        else
                        {
                            if ((item.STARTED_DATE != null || item.ACTUAL_QUANTITY != null) && item.ACTIVITY_DESC == "Crack Relief Layer (SAMI)")
                            {
                                if ((dbContext.PMIS_FDR_STABILIZE_CHAINAGE_WISE_DETAIL.Where(x => x.IMS_PR_ROAD_CODE == item.IMS_PR_ROAD_CODE && x.IS_LATEST == "Y").Any()))
                                {
                                    var entryDate = dbContext.PMIS_FDR_STABILIZE_CHAINAGE_WISE_DETAIL.Where(x => x.IMS_PR_ROAD_CODE == item.IMS_PR_ROAD_CODE && x.IS_LATEST == "Y").Select(x => x.ENTRY_DATE).FirstOrDefault();
                                    var currentDate = DateTime.Now;
                                    if (entryDate.Day != currentDate.Day || entryDate.Month != currentDate.Month || entryDate.Year != currentDate.Year)
                                    {
                                        return Json(new { Success = false, ErrorMessage = "Construction of FDR Stabilize base is Manadatory to fill for Current Date before progress enter for Activity Crack Relief Layer (SAMI)." });
                                    }
                                }
                                else
                                {
                                    return Json(new { Success = false, ErrorMessage = "Construction of FDR Stabilize base is Manadatory to fill before progress enter for Activity Crack Relief Layer (SAMI)." });
                                }
                            }
                        }
                    }

                    #endregion
                    // Saurabh Changes Ended..

                    

                    decimal? TotalCompLength = dbContext.EXEC_ROADS_MONTHLY_STATUS.Where(x => x.IMS_PR_ROAD_CODE == item.IMS_PR_ROAD_CODE).Max(x => x.EXEC_COMPLETED);

                    // Changes Made by Saurabh Jojare based on new requirement on PMIS 21-02-2022
                    var ActivityValues = dbContext.PMIS_PROGRESS_DETAILS.Where(x => x.IMS_PR_ROAD_CODE == item.IMS_PR_ROAD_CODE && x.IS_LATEST=="Y").ToList();

                    foreach (var ActivityData in ActivityValues)
                    {
                        var ActivityName = dbContext.PMIS_ACTIVITY_MASTER.Where(x => x.ACTIVITY_ID == ActivityData.ACTIVITY_ID).Select(x => x.ACTIVITY_DESC).FirstOrDefault();

                        if (item.ACTIVITY_DESC == ActivityName && item.ACTUAL_QUANTITY < ActivityData.PGS_QUANTITY)
                        {
                            return Json(new { Success = false, ErrorMessage = ActivityName + " executed quantity cannot be less than previous reported executed quantity" });

                        }

                    }
                    if (item.ACTIVITY_DESC.Equals("Surface Course (OGPC/MSS/CC)") && (item.ACTUAL_QUANTITY < item.CompletedRoadLength))
                    {

                        return Json(new { Success = false, ErrorMessage = "Surface Course Length Cannot be less than Completed Length" });

                    }

                    // Changes made by Saurabh Ended Here...
                    if ((item.ACTIVITY_DESC != null || item.ACTIVITY_DESC != ""))
                    {

                        if (item.ACTIVITY_DESC != "Miscellaneous")
                        {
                            if (item.ACTIVITY_DESC == "Field Lab" && IsFieldLab != "L" && IsFieldLab != "F" && IsFieldLab != "A")   // change
                            {
                                if (item.STARTED_DATE == null || item.FINISHED_DATE == null)
                                {
                                    return Json(new { Success = false, ErrorMessage = "Actual Start Date and Actual Completion date must be Entered for " + item.ACTIVITY_DESC });
                                }

                            }
                        }
                    }

                    //decimal SanLength = Math.Round(dbContext.IMS_SANCTIONED_PROJECTS.Where(x => x.IMS_PR_ROAD_CODE == item.IMS_PR_ROAD_CODE).Select(x => x.IMS_PAV_LENGTH).FirstOrDefault(), 2);
                    //decimal SanLengthMax = (SanLength * 110) / 100;
                    //decimal MinSanLength = (SanLength * 90) / 100;

                    decimal SanLength = Math.Round(dbContext.IMS_SANCTIONED_PROJECTS.Where(x => x.IMS_PR_ROAD_CODE == item.IMS_PR_ROAD_CODE).Select(x => x.IMS_PAV_LENGTH).FirstOrDefault(), 2);

                    decimal changedLength = dbContext.IMS_PROGRESS_LENGTH_COMPLETION.Where(m => m.IMS_PR_ROAD_CODE == item.IMS_PR_ROAD_CODE).Any() ? dbContext.IMS_PROGRESS_LENGTH_COMPLETION.Where(m => m.IMS_PR_ROAD_CODE == item.IMS_PR_ROAD_CODE && m.IMS_IS_MRD_APPROVED == "Y").Select(m => m.IMS_CHANGED_LENGTH).FirstOrDefault() : 0;
                    decimal changed_SanctionedLength = changedLength <= 0 ? SanLength : changedLength;

                    decimal SanLengthMax = (changed_SanctionedLength * 110) / 100;
                    decimal MinSanLength = (changed_SanctionedLength * 90) / 100;

                    if (item.CompletedRoadLength != null && item.CompletedRoadLength > SanLengthMax)
                    {
                        return Json(new { Success = false, ErrorMessage = "Completed length should not be more than 10% of sanctioned length." });
                    }

                    if (Proj_Status == "C")
                    {
                        if (item.CompletedRoadLength != null && item.CompletedRoadLength < MinSanLength)
                        {
                            return Json(new { Success = false, ErrorMessage = "Completed length should not be less than 10% of sanctioned length." });
                        }
                    }
                    
                    if (TotalCompLength != null && item.CompletedRoadLength != null && item.CompletedRoadLength < TotalCompLength)
                    {
                        return Json(new { Success = false, ErrorMessage = "Completed length should not be less than total completed length as on date." });
                    }
                    if (item.STARTED_DATE != null && item.FINISHED_DATE != null && item.STARTED_DATE > item.FINISHED_DATE)
                    {
                        return Json(new { Success = false, ErrorMessage = "Actual Completion date of an activity cannot be before actual start date ." });
                    }
                    if (item.STARTED_DATE == null && item.ACTUAL_QUANTITY != null)
                    {
                        return Json(new { Success = false, ErrorMessage = "Actual start date is mandatory for the activities for which actual quantity has been entered." });
                    }
                }
                ViewBag.operation = "A";
                string Status = objDAL.SaveActualsDAL(formData);
                if (Status == string.Empty)
                    return Json(new { Success = true });
                else
                    return Json(new { Success = false, ErrorMessage = Status });

            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "SaveActuals(AddActualsViewModel formData))");
                throw ex;
            }
        }

        [HttpPost]
        [Audit]
        public ActionResult SubmitActuals(IEnumerable<AddActualsViewModel> formData)
        {
            PMISDAL objDAL = new PMISDAL();
            PMGSYEntities dbContext = new PMGSYEntities();
            char Project_Status = 'P';
            string IsFieldLab = string.Empty;     // change
            try
            {
                foreach (var item in formData)
                {
                    // changes made by Saurabh Jojare on 21-02-2022
                    Project_Status = (item.ProjectStatus == 'C' ? 'C' : 'P');
                    string Proj_Status = Convert.ToString(Project_Status);
                    IsFieldLab = Convert.ToString(item.ProjectStatus);         // change 
                    var ActivityValues = dbContext.PMIS_PROGRESS_DETAILS.Where(x => x.IMS_PR_ROAD_CODE == item.IMS_PR_ROAD_CODE && x.IS_LATEST == "Y").ToList();

                    foreach (var ActivityData in ActivityValues)
                    {
                        var ActivityName = dbContext.PMIS_ACTIVITY_MASTER.Where(x => x.ACTIVITY_ID == ActivityData.ACTIVITY_ID).Select(x => x.ACTIVITY_DESC).FirstOrDefault();

                        if (item.ACTIVITY_DESC == ActivityName && item.ACTUAL_QUANTITY < ActivityData.PGS_QUANTITY)
                        {
                            return Json(new { Success = false, ErrorMessage = ActivityName + " executed quantity cannot be less than previous reported executed quantity" });

                        }
                    }
                    if (item.ACTIVITY_DESC.Equals("Surface Course (OGPC/MSS/CC)") && (item.ACTUAL_QUANTITY < item.CompletedRoadLength))
                    {
                        return Json(new { Success = false, ErrorMessage = "Surface Course Length Cannot be less than Completed Length" });
                    }

                    // Changes made by Saurabh Ended Here...
                    if ((item.ACTIVITY_DESC != null || item.ACTIVITY_DESC != ""))
                    {

                        if (item.ACTIVITY_DESC != "Miscellaneous")
                        {
                            if (item.ACTIVITY_DESC == "Field Lab" && IsFieldLab != "L" && IsFieldLab != "F" && IsFieldLab != "A")  // change
                            {
                                if (item.STARTED_DATE == null || item.FINISHED_DATE == null)
                                {
                                    return Json(new { Success = false, ErrorMessage = "Actual Start Date and Actual Completion date must be Entered for " + item.ACTIVITY_DESC });
                                }

                            }
                        }
                    }


                    decimal? QtyLimit = item.QUANTITY == null ? 0 : (item.QUANTITY * 110) / 100;

                    if (item.ACTUAL_QUANTITY != null && item.ACTUAL_QUANTITY > QtyLimit)
                    {
                        if (item.QUANTITY == null && QtyLimit == 0)
                        {
                            return Json(new { Success = false, ErrorMessage = "Quantity executed as on date cannot be entered for the activities for which planned quantity has not been entered" });
                        }
                        return Json(new { Success = false, ErrorMessage = "Quantity executed as on date should not be more than 10% of the planned quantity" });
                    }

                    // Changes Added by Saurabh for FDR Technology Detail on 08-06-2023

                    #region Validation added for new FDR Technology Detail

                    if ((dbContext.IMS_PROPOSAL_TECH.Where(x => x.IMS_PR_ROAD_CODE == item.IMS_PR_ROAD_CODE && x.MAST_TECH_CODE == 64).Any()))
                    {
                        if ((item.STARTED_DATE == null || item.ACTUAL_QUANTITY == null) && item.ACTIVITY_DESC == "FDR Stabilized Base")  // item.FINISHED_DATE == null ||
                        {
                            return Json(new { Success = false, ErrorMessage = "Actual start date and Quantity executed is required for(FDR Stabilized Base) road projects utilizing Cement stabilization technology." });
                        }
                        else
                        {
                            if ((item.STARTED_DATE != null || item.ACTUAL_QUANTITY != null) && item.ACTIVITY_DESC == "FDR Stabilized Base")
                            {
                                if ((dbContext.PMIS_FDR_STABILIZE_CHAINAGE_WISE_DETAIL.Where(x => x.IMS_PR_ROAD_CODE == item.IMS_PR_ROAD_CODE && x.IS_LATEST == "Y").Any()))
                                {
                                    var entryDate = dbContext.PMIS_FDR_STABILIZE_CHAINAGE_WISE_DETAIL.Where(x => x.IMS_PR_ROAD_CODE == item.IMS_PR_ROAD_CODE && x.IS_LATEST == "Y").Select(x => x.ENTRY_DATE).FirstOrDefault();
                                    var currentDate = DateTime.Now;
                                    if (entryDate.Day != currentDate.Day || entryDate.Month != currentDate.Month || entryDate.Year != currentDate.Year)
                                    {
                                        return Json(new { Success = false, ErrorMessage = "Construction of FDR Stabilize base is Manadatory to fill for Current Date before progress enter for Activity FDR Stabilize Base." });
                                    }
                                }
                                else
                                {
                                    return Json(new { Success = false, ErrorMessage = "Construction of FDR Stabilize base is Manadatory to fill before progress enter for Activity FDR Stabilize Base." });
                                }
                            }

                        }
                        if ((item.STARTED_DATE == null || item.ACTUAL_QUANTITY == null) && item.ACTIVITY_DESC == "Crack Relief Layer (SAMI)")   // item.FINISHED_DATE == null ||
                        {
                            return Json(new { Success = false, ErrorMessage = "Actual start date and Quantity executed is required for(Crack Relief Layer (SAMI)) road projects utilizing Cement stabilization technology." });
                        }
                        else
                        {
                            if ((item.STARTED_DATE != null || item.ACTUAL_QUANTITY != null) && item.ACTIVITY_DESC == "Crack Relief Layer (SAMI)")
                            {
                                if ((dbContext.PMIS_FDR_STABILIZE_CHAINAGE_WISE_DETAIL.Where(x => x.IMS_PR_ROAD_CODE == item.IMS_PR_ROAD_CODE && x.IS_LATEST == "Y").Any()))
                                {
                                    var entryDate = dbContext.PMIS_FDR_STABILIZE_CHAINAGE_WISE_DETAIL.Where(x => x.IMS_PR_ROAD_CODE == item.IMS_PR_ROAD_CODE && x.IS_LATEST == "Y").Select(x => x.ENTRY_DATE).FirstOrDefault();
                                    var currentDate = DateTime.Now;
                                    if (entryDate.Day != currentDate.Day || entryDate.Month != currentDate.Month || entryDate.Year != currentDate.Year)
                                    {
                                        return Json(new { Success = false, ErrorMessage = "Construction of FDR Stabilize base is Manadatory to fill for Current Date before progress enter for Activity Crack Relief Layer (SAMI)." });
                                    }
                                }
                                else
                                {
                                    return Json(new { Success = false, ErrorMessage = "Construction of FDR Stabilize base is Manadatory to fill before progress enter for Activity Crack Relief Layer (SAMI)." });
                                }
                            }
                        }
                    }

                    #endregion

                    // Changes Ended Here

                    decimal SanLength = Math.Round(dbContext.IMS_SANCTIONED_PROJECTS.Where(x => x.IMS_PR_ROAD_CODE == item.IMS_PR_ROAD_CODE).Select(x => x.IMS_PAV_LENGTH).FirstOrDefault(), 2);
                    decimal changedLength = dbContext.IMS_PROGRESS_LENGTH_COMPLETION.Where(m => m.IMS_PR_ROAD_CODE == item.IMS_PR_ROAD_CODE).Any() ? dbContext.IMS_PROGRESS_LENGTH_COMPLETION.Where(m => m.IMS_PR_ROAD_CODE == item.IMS_PR_ROAD_CODE && m.IMS_IS_MRD_APPROVED == "Y").Select(m => m.IMS_CHANGED_LENGTH).FirstOrDefault() : 0;
                    decimal changed_SanctionedLength = changedLength <= 0 ? SanLength : changedLength;
                    decimal SanLengthMax = (changed_SanctionedLength * 110) / 100;
                    decimal MinSanLength = (changed_SanctionedLength * 90) / 100;

                    decimal? TotalCompLength = dbContext.EXEC_ROADS_MONTHLY_STATUS.Where(x => x.IMS_PR_ROAD_CODE == item.IMS_PR_ROAD_CODE).Max(x => x.EXEC_COMPLETED);

                    if (item.CompletedRoadLength != null && item.CompletedRoadLength > SanLengthMax)
                    {
                        return Json(new { Success = false, ErrorMessage = "Completed length should not be more than 10% of sanctioned length." });
                    }

                    if (Proj_Status == "C")
                    {
                        if (item.CompletedRoadLength != null && item.CompletedRoadLength < MinSanLength)
                        {
                            return Json(new { Success = false, ErrorMessage = "Completed length should not be less than 10% of sanctioned length." });
                        }
                    }

                    if (TotalCompLength != null && item.CompletedRoadLength != null && item.CompletedRoadLength < TotalCompLength)
                    {
                        return Json(new { Success = false, ErrorMessage = "Completed length should not be less than total completed length as on date." });
                    }

                    if (item.STARTED_DATE != null && item.FINISHED_DATE != null && item.STARTED_DATE > item.FINISHED_DATE)
                    {
                        return Json(new { Success = false, ErrorMessage = "Actual Completion date of an activity cannot be before actual start date ." });
                    }
                    if (item.STARTED_DATE == null && item.ACTUAL_QUANTITY != null)
                    {
                        return Json(new { Success = false, ErrorMessage = "Actual start date is mandatory for the activities for which actual quantity has been entered." });
                    }

                }
                string Status = objDAL.SubmitActualsDAL(formData);
                if (Status == string.Empty)
                    return Json(new { Success = true });
                else
                    return Json(new { Success = false, ErrorMessage = Status });

            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "SubmitActuals(AddActualsViewModel formData))");
                throw ex;
            }
        }
        #endregion


        #region Add Chainage Wise Details
        [HttpGet]
        [Audit]
        public ViewResult AddChainage(String parameter, String hash, String key)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            PMISDAL objDAL = new PMISDAL();
            Dictionary<string, string> decryptedParameters = null;
            try
            {
                int rowlength = 0;
                decryptedParameters = URLEncrypt.DecryptParameters1(new string[] { parameter, hash, key });
                AddChainageViewModel objaddchainage = new AddChainageViewModel();



                if (decryptedParameters.Count() > 0)
                {
                    objaddchainage.IMS_PR_ROAD_CODE = Convert.ToInt32(decryptedParameters["RoadCode"].ToString());
                    if (objaddchainage.IMS_PR_ROAD_CODE != 0)
                    {
                        decimal SanctionLength = 0.0m;
                        var SanctionLengthobj = dbContext.IMS_SANCTIONED_PROJECTS.Where(x => x.IMS_PR_ROAD_CODE == objaddchainage.IMS_PR_ROAD_CODE).FirstOrDefault();
                        if (SanctionLengthobj.IMS_PROPOSAL_TYPE == "P")
                            SanctionLength = SanctionLengthobj.IMS_PAV_LENGTH;
                        else
                            SanctionLength = SanctionLengthobj.IMS_BRIDGE_LENGTH ?? default(decimal);

                        rowlength = Convert.ToInt32(Decimal.Ceiling(SanctionLength));

                        if (rowlength >= 0)
                        {
                            objaddchainage.SanctionedLength = Convert.ToInt32(rowlength);
                        }
                        else
                        {
                            return null;
                        }

                    }
                    else
                    {
                        return null;
                    }

                    if (dbContext.PMIS_CHAINAGEWISE_COMPLETION_DETAILS.Where(x => x.IMS_PR_ROAD_CODE == objaddchainage.IMS_PR_ROAD_CODE).Any())
                    {
                        var chainagewisedetails = dbContext.PMIS_CHAINAGEWISE_COMPLETION_DETAILS.Where(x => x.IMS_PR_ROAD_CODE == objaddchainage.IMS_PR_ROAD_CODE && x.IS_VALID == "Y").ToList();
                        objaddchainage.earthworklist = new List<SelectListItem>();
                        objaddchainage.subgradelist = new List<SelectListItem>();
                        objaddchainage.granularsubbaselist = new List<SelectListItem>();
                        objaddchainage.wbmgrading2list = new List<SelectListItem>();
                        objaddchainage.wbmgrading3list = new List<SelectListItem>();
                        objaddchainage.wetmixmacadamlist = new List<SelectListItem>();
                        objaddchainage.bituminousmacadamlist = new List<SelectListItem>();
                        objaddchainage.surfacecourselist = new List<SelectListItem>();
                        var chainageentrydate = chainagewisedetails.Select(x => x.ENTRY_DATE).FirstOrDefault();
                        objaddchainage.Date_of_Chainage_entry = chainageentrydate;
                        foreach (var item in chainagewisedetails)
                        {
                            objaddchainage.earthworklist.Add(new SelectListItem { Value = item.ACT_EARTHWORK });
                            objaddchainage.subgradelist.Add(new SelectListItem { Value = item.ACT_SUBGRADE });
                            objaddchainage.granularsubbaselist.Add(new SelectListItem { Value = item.ACT_GRSUBBASE });
                            objaddchainage.wbmgrading2list.Add(new SelectListItem { Value = item.ACT_WBM_GRADING2 });
                            objaddchainage.wbmgrading3list.Add(new SelectListItem { Value = item.ACT_WBM_GRADING3 });
                            objaddchainage.wetmixmacadamlist.Add(new SelectListItem { Value = item.ACT_WETMIX_MACADAM });
                            objaddchainage.bituminousmacadamlist.Add(new SelectListItem { Value = item.ACT_BIT_MACADAM });
                            objaddchainage.surfacecourselist.Add(new SelectListItem { Value = item.ACT_SURFACE_COURSE });
                        }


                    }
                    else
                    {
                        objaddchainage.earthworklist = new List<SelectListItem>();
                        objaddchainage.subgradelist = new List<SelectListItem>();
                        objaddchainage.granularsubbaselist = new List<SelectListItem>();
                        objaddchainage.wbmgrading2list = new List<SelectListItem>();
                        objaddchainage.wbmgrading3list = new List<SelectListItem>();
                        objaddchainage.wetmixmacadamlist = new List<SelectListItem>();
                        objaddchainage.bituminousmacadamlist = new List<SelectListItem>();
                        objaddchainage.surfacecourselist = new List<SelectListItem>();

                        for (int i = 0; i <= rowlength; i++)
                        {
                            objaddchainage.earthworklist.Add(new SelectListItem { Value = null });
                            objaddchainage.subgradelist.Add(new SelectListItem { Value = null });
                            objaddchainage.granularsubbaselist.Add(new SelectListItem { Value = null });
                            objaddchainage.wbmgrading2list.Add(new SelectListItem { Value = null });
                            objaddchainage.wbmgrading3list.Add(new SelectListItem { Value = null });
                            objaddchainage.wetmixmacadamlist.Add(new SelectListItem { Value = null });
                            objaddchainage.bituminousmacadamlist.Add(new SelectListItem { Value = null });
                            objaddchainage.surfacecourselist.Add(new SelectListItem { Value = null });
                        }
                    }


                }
                return View("~/Areas/PMIS/Views/PMIS/AddChainage.cshtml", objaddchainage);

            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "PMIS.AddChainage()");
                return null;
            }
        }

        [HttpPost]
        [Audit]
        public JsonResult AddChainage(FormCollection formData)
        {
            PMISDAL objDAL = new PMISDAL();
            PMGSYEntities dbContext = new PMGSYEntities();
            try
            {
                string Status = objDAL.SubmitChainageDetailsDAL(formData);
                if (Status == string.Empty)
                    return Json(new { Success = true, Message = "Chainage wise details submitted successfully." });
                else
                    return Json(new { Success = false, Message = "Error occured while processing your request." });
            }
            catch (DbUpdateException ex)
            {
                ErrorLog.LogError(ex, "SubmitChainageDetails(DbUpdateException ex).DAL");
                return Json(new { Success = false, Message = "An Error occured while processing your request." });
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "SubmitChainageDetails(AddChainageViewModel formData))");
                throw ex;
            }
        }

        #endregion


        /// <summary>
        /// PMIS FDR Chainage-wise Detail region Added by Saurabh on 08-06-2023.
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="hash"></param>
        /// <param name="key"></param>
        /// <returns></returns>

        #region PMIS FDR Chainage-wise Detail 

        [HttpGet]
        [Audit]
        public ViewResult AddFDRStabilize(String parameter, String hash, String key)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            PMISDAL objDAL = new PMISDAL();
            int rowlength = 0;
            Dictionary<string, string> decryptedParameters = null;
            try
            {
                decryptedParameters = URLEncrypt.DecryptParameters1(new string[] { parameter, hash, key });

                AddFDRStabilizeModel objaddStabilize = new AddFDRStabilizeModel();
                //List<AddFDRStabilizeModel> fDRStabilizeModelList = new List<AddFDRStabilizeModel>();
                decimal SanctionLength = 0.0m;
                if (decryptedParameters.Count() > 0)
                {
                    objaddStabilize.IMS_PR_ROAD_CODE = Convert.ToInt32(decryptedParameters["RoadCode"].ToString());
                    if (objaddStabilize.IMS_PR_ROAD_CODE != 0)
                    {
                        var SanctionData = dbContext.IMS_SANCTIONED_PROJECTS.Where(x => x.IMS_PR_ROAD_CODE == objaddStabilize.IMS_PR_ROAD_CODE).FirstOrDefault();
                        var StateName = dbContext.MASTER_STATE.Where(x => x.MAST_STATE_CODE == SanctionData.MAST_STATE_CODE).Select(x => x.MAST_STATE_NAME).FirstOrDefault();
                        var DistrictName = dbContext.MASTER_DISTRICT.Where(x => x.MAST_DISTRICT_CODE == SanctionData.MAST_DISTRICT_CODE).Select(x => x.MAST_DISTRICT_NAME).FirstOrDefault();
                        var BlockName = dbContext.MASTER_BLOCK.Where(x => x.MAST_BLOCK_CODE == SanctionData.MAST_BLOCK_CODE).Select(x => x.MAST_BLOCK_NAME).FirstOrDefault();
                        ViewBag.PackageName = SanctionData.IMS_PACKAGE_ID;
                        ViewBag.SanctionYear = SanctionData.IMS_YEAR;
                        ViewBag.SanctionDate = ((SanctionData.IMS_SANCTIONED_DATE).Value).ToShortDateString();
                        ViewBag.Batch = SanctionData.IMS_BATCH;
                        ViewBag.SanctionLength = SanctionData.IMS_PAV_LENGTH;
                        ViewBag.RoadName = SanctionData.IMS_ROAD_NAME;
                        ViewBag.StateName = StateName;
                        ViewBag.DistrictName = DistrictName;
                        ViewBag.BlockName = BlockName;
                        ViewBag.RoadCode = objaddStabilize.IMS_PR_ROAD_CODE;
                        TempData["Date_of_FDR_entry"] = DateTime.Now;
                        var TechnologyList = string.Join(", ", dbContext.IMS_PROPOSAL_TECH.Where(x => x.IMS_PR_ROAD_CODE == objaddStabilize.IMS_PR_ROAD_CODE).Select(x => x.MASTER_TECHNOLOGY.MAST_TECH_NAME).ToList());
                        ViewBag.TechName = TechnologyList;



                        var SanctionLengthobj = dbContext.IMS_SANCTIONED_PROJECTS.Where(x => x.IMS_PR_ROAD_CODE == objaddStabilize.IMS_PR_ROAD_CODE).FirstOrDefault();
                        if (SanctionLengthobj.IMS_PROPOSAL_TYPE == "P")
                            SanctionLength = SanctionLengthobj.IMS_PAV_LENGTH;
                        else
                            SanctionLength = SanctionLengthobj.IMS_BRIDGE_LENGTH ?? default(decimal);

                        rowlength = Convert.ToInt32(Decimal.Ceiling(SanctionLength));
                        objaddStabilize.ROW_LENGTH = rowlength;
                        if (rowlength >= 0)
                        {
                            objaddStabilize.Sanction_length = SanctionLength;
                        }
                        else
                        {
                            return null;
                        }
                    }
                    else
                    {
                        return null;
                    }
                    if (dbContext.PMIS_FDR_STABILIZE_CHAINAGE_WISE_DETAIL.Where(x => x.IMS_PR_ROAD_CODE == objaddStabilize.IMS_PR_ROAD_CODE).Any())
                    {
                        var chainagewisedetails = dbContext.PMIS_FDR_STABILIZE_CHAINAGE_WISE_DETAIL.Where(x => x.IMS_PR_ROAD_CODE == objaddStabilize.IMS_PR_ROAD_CODE && x.IS_LATEST == "Y").ToList(); // IS_VALID
                        objaddStabilize.START_CHAINAGE_1 = new List<SelectListItem>();
                        objaddStabilize.END_CHAINAGE_1 = new List<SelectListItem>();
                        objaddStabilize.START_CHAINAGE_2 = new List<SelectListItem>();
                        objaddStabilize.END_CHAINAGE_2 = new List<SelectListItem>();
                        // objaddStabilize.Chainage_Date_FirstChainage = new List<SelectListItem>();
                        // objaddStabilize.Chainage_Date_SecondChainage = new List<SelectListItem>();
                        var chainageentrydate = chainagewisedetails.Select(x => x.ENTRY_DATE).FirstOrDefault();
                        objaddStabilize.Entry_Date = chainageentrydate;
                        foreach (var item in chainagewisedetails)
                        {
                            //objaddStabilize.START_CHAINAGE_1.Add(new SelectListItem { Value = item.CHAINAGE_FROM.ToString() });
                            //objaddStabilize.END_CHAINAGE_1.Add(new SelectListItem { Value = item.CHAINAGE_TO.ToString() });
                            //objaddStabilize.Chainage_Date_FirstChainage.Add(new SelectListItem { Value = item.ENTRY_DATE.ToString() });
                            //objaddStabilize.START_CHAINAGE_2.Add(new SelectListItem { Value = item.CHAINAGE_FROM.ToString() });
                            //objaddStabilize.END_CHAINAGE_2.Add(new SelectListItem { Value = item.CHAINAGE_TO.ToString() });
                            //objaddStabilize.Chainage_Date_SecondChainage.Add(new SelectListItem { Value = item.ENTRY_DATE.ToString() });


                        }
                        //for(int i=0;i< chainagewisedetails.Count; i++)
                        //{
                        //    AddFDRStabilizeListModel obj = new AddFDRStabilizeListModel();
                        //    obj.START_CHAINAGE_1 = chainagewisedetails[i].F_START_CHAINAGE;
                        //    obj.END_CHAINAGE_1 = chainagewisedetails[i].F_END_CHAINAGE;
                        //    obj.Chainage_Date_FirstChainage= chainagewisedetails[i].F_CHAINAGE_DATE.ToString();
                        //    obj.START_CHAINAGE_2= chainagewisedetails[i].S_START_CHAINAGE;
                        //    obj.END_CHAINAGE_2= chainagewisedetails[i].S_END_CHAINAGE;
                        //    obj.Chainage_Date_SecondChainage = chainagewisedetails[i].S_CHAINAGE_DATE.ToString();


                        //    objaddStabilize.AddFDRStabilizeListModelObj.Add(obj);
                        //}
                        List<AddFDRStabilizeListModel> AddFDRStabilizeListModelObjList = new List<AddFDRStabilizeListModel>();
                        foreach (var item in chainagewisedetails)
                        {
                            AddFDRStabilizeListModelObjList.Add(new AddFDRStabilizeListModel
                            {
                                CHAINAGE_FROM = item.CHAINAGE_FROM,
                                CHAINAGE_TO = item.CHAINAGE_TO,
                                START_CHAINAGE_1 = item.F_START_CHAINAGE,
                                END_CHAINAGE_1 = item.F_END_CHAINAGE,
                                Chainage_Date_FirstChainage = item.F_CHAINAGE_DATE.ToString().Split(' ')[0],
                                START_CHAINAGE_2 = item.S_START_CHAINAGE,
                                END_CHAINAGE_2 = item.S_END_CHAINAGE,
                                Chainage_Date_SecondChainage = item.S_CHAINAGE_DATE.ToString().Split(' ')[0]
                            });
                            objaddStabilize.AddFDRStabilizeListModelObj = AddFDRStabilizeListModelObjList;

                        }


                        objaddStabilize.IS_SUBMIT = "Y";
                        TempData["IsSubmit"] = 0;
                    }
                    else
                    {
                        objaddStabilize.START_CHAINAGE_1 = new List<SelectListItem>();
                        objaddStabilize.END_CHAINAGE_1 = new List<SelectListItem>();
                        objaddStabilize.START_CHAINAGE_2 = new List<SelectListItem>();
                        objaddStabilize.END_CHAINAGE_2 = new List<SelectListItem>();
                        //  objaddStabilize.Chainage_Date_FirstChainage = new List<SelectListItem>();
                        //  objaddStabilize.Chainage_Date_SecondChainage = new List<SelectListItem>();

                        for (int i = 0; i <= rowlength; i++)
                        {
                            //objaddStabilize.START_CHAINAGE_1.Add(new SelectListItem { Value = null });
                            //objaddStabilize.END_CHAINAGE_1.Add(new SelectListItem { Value = null });
                            //objaddStabilize.START_CHAINAGE_2.Add(new SelectListItem { Value = null });
                            //objaddStabilize.END_CHAINAGE_2.Add(new SelectListItem { Value = null });
                            //objaddStabilize.Chainage_Date_FirstChainage.Add(new SelectListItem { Value = null });
                            //objaddStabilize.Chainage_Date_SecondChainage.Add(new SelectListItem { Value = null });

                            objaddStabilize.START_CHAINAGE_1.Add(new SelectListItem { Value = null });
                            objaddStabilize.END_CHAINAGE_1.Add(new SelectListItem { Value = null });
                            objaddStabilize.START_CHAINAGE_2.Add(new SelectListItem { Value = null });
                            objaddStabilize.END_CHAINAGE_2.Add(new SelectListItem { Value = null });
                            // objaddStabilize.Chainage_Date_FirstChainage.Add(new SelectListItem { Value = null });
                            // objaddStabilize.Chainage_Date_SecondChainage.Add(new SelectListItem { Value = null });



                        }
                        TempData["IsSubmit"] = null;

                        objaddStabilize.IS_SUBMIT = "N";
                    }
                }
                return View("~/Areas/PMIS/Views/PMIS/AddFDRStabilize.cshtml", objaddStabilize);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "PMIS.AddFDRStabilize()");
                return null;
            }
        }

        [HttpPost]
        [Audit]
        public ActionResult SubmitFDRDetail(AddFDRStabilizeModel formData)   //    FormCollection
        {
            PMISDAL objDAL = new PMISDAL();
            PMGSYEntities dbContext = new PMGSYEntities();
            try
            {
                int i = 0;
                int count = 0;
                //foreach (var item in formData)
                //{
                i++;
                int roadcode = formData.IMS_PR_ROAD_CODE;// item.IMS_PR_ROAD_CODE;


                var SanctionDate = dbContext.IMS_SANCTIONED_PROJECTS.Where(x => x.IMS_PR_ROAD_CODE == roadcode).Select(x => x.IMS_SANCTIONED_DATE).FirstOrDefault();

                #region

                //if ((item.CHAINAGE_FROM == null) && (item.CHAINAGE_TO == null) && (item.CHAINAGE_DATE == null))
                //{
                //    count++;
                //    if (count == 5)
                //    {
                //        return Json(new { Success = false, ErrorMessage = "At Least one Start and End Chainage, Chainage Date is required."});
                //    }
                //}
                //else
                //{
                //    if ((item.CHAINAGE_FROM != null) && (item.CHAINAGE_TO == null || item.CHAINAGE_DATE == null))
                //    {
                //        return Json(new { Success = false, ErrorMessage = "End Chainage and Chainage Date required when Chainage From is entered for Sr No " + i });
                //    }
                //    else if ((item.CHAINAGE_TO != null) && (item.CHAINAGE_FROM == null || item.CHAINAGE_DATE == null))
                //    {
                //        return Json(new { Success = false, ErrorMessage = "Start Chainage and Chainage Date required when Chainage To is entered for Sr No " + i });
                //    }
                //    else if ((item.CHAINAGE_DATE != null) && (item.CHAINAGE_FROM == null || item.CHAINAGE_TO == null))
                //    {
                //        return Json(new { Success = false, ErrorMessage = "Start and End Chainage required when Chainage Date entered for Sr No " + i });
                //    }
                //}

                //if ((item.CHAINAGE_FROM != null) && (item.CHAINAGE_TO != null) && (item.CHAINAGE_DATE != null))
                //{

                //    try
                //    {
                //        if (item.CHAINAGE_DATE.Value.Day < SanctionDate.Value.Day && item.CHAINAGE_DATE.Value.Month == SanctionDate.Value.Month && item.CHAINAGE_DATE.Value.Year == SanctionDate.Value.Year)
                //        {
                //            return Json(new { Success = false, ErrorMessage = "Date Entered for Chainage should not be less than Sanction Date for Sr no. " + i });
                //        }

                //        if (item.CHAINAGE_TO <= item.CHAINAGE_FROM)
                //        {
                //            return Json(new { Success = false, ErrorMessage = "Start chainage must be less than end chainage From for Sr no. " + i });
                //        }

                //        //if (item.CHAINAGE_DATE < SanctionDate)
                //        //{
                //        //    return Json(new { Success = false, ErrorMessage = "Date Entered for Chainage should not be less than Sanction Date for Sr no. " + i });
                //        //}

                //    }
                //    catch (Exception ex)
                //    {
                //        ErrorLog.LogError(ex, "PMIS.SubmitFDRDetail()");
                //        return Json(new { Success = false, ErrorMessage = "Date Entered for Chainage not in Correct Format." });
                //    }

                //    var SanctionLength = dbContext.IMS_SANCTIONED_PROJECTS.Where(x => x.IMS_PR_ROAD_CODE == roadcode).Select(x => x.IMS_PAV_LENGTH).FirstOrDefault();
                //    if (item.CHAINAGE_FROM > SanctionLength || item.CHAINAGE_TO > SanctionLength)
                //    {
                //        return Json(new { Success = false, ErrorMessage = "Start and End Chainage Should not be greater than Sanction Length of Road for Sr No " + i });
                //    }
                //}


                // }

                #endregion


                List<string> errorslist = new List<string>();
                StringBuilder strErrorMessage = new StringBuilder("");
                // bool isRowAllDetailsFilled = true;
                int nullCount = 0;

                for (int t = 0; t < formData.ROW_LENGTH; t++)
                {
                    if (formData.AddFDRStabilizeListModelObj[t].START_CHAINAGE_1 == null &&
                        formData.AddFDRStabilizeListModelObj[t].END_CHAINAGE_1 == null && formData.AddFDRStabilizeListModelObj[t].Chainage_Date_FirstChainage == null)
                    {
                        nullCount++;
                    }
                    else
                    {

                        if (formData.AddFDRStabilizeListModelObj[t].START_CHAINAGE_1 == null)
                        {
                            string error = "Please enter the start chainage for " + t + "-" + (t + 0.5) + " km range";
                            errorslist.Add(error);
                        }
                        if (formData.AddFDRStabilizeListModelObj[t].END_CHAINAGE_1 == null)
                        {
                            string error = "Please enter the end chainage for " + t + "-" + (t + 0.5) + " km range";
                            errorslist.Add(error);
                        }
                        if (formData.AddFDRStabilizeListModelObj[t].Chainage_Date_FirstChainage == null)
                        {
                            string error = "Please select the date for chainage " + (t + 0.5) + "-" + (t + 1) + " km range";
                            errorslist.Add(error);
                        }

                        if (formData.AddFDRStabilizeListModelObj[t].Chainage_Date_FirstChainage != null)
                        {
                            string tempDate = formData.AddFDRStabilizeListModelObj[t].Chainage_Date_FirstChainage;
                            DateTime fromDateValue;
                            var formats = new[] { "dd/MM/yyyy" };
                            if (!DateTime.TryParseExact(tempDate, formats, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out fromDateValue))
                            {
                                string error = "Invalid First Chainage date entered for " + (t) + "-" + (t + 0.5) + " km.";
                                errorslist.Add(error);
                            }
                            else if (Convert.ToDateTime(formData.AddFDRStabilizeListModelObj[t].Chainage_Date_FirstChainage) < SanctionDate)
                            {
                                string error = "First Chainage date can't be less than sanction date for " + (t) + "-" + (t + 0.5) + " km.";
                                errorslist.Add(error);
                            }
                        }


                        if (formData.AddFDRStabilizeListModelObj[t].START_CHAINAGE_1 != null)
                        {
                            if (!(formData.AddFDRStabilizeListModelObj[t].START_CHAINAGE_1 >= t && formData.AddFDRStabilizeListModelObj[t].START_CHAINAGE_1 <= Convert.ToDecimal(t + 0.5)))
                            {
                                string error = "Please enter First Start chainage within range of " + (t) + "-" + (t + 0.5) + " km.";
                                errorslist.Add(error);
                            }
                            if (formData.AddFDRStabilizeListModelObj[t].START_CHAINAGE_1 > formData.Sanction_length)
                            {
                                string error = "First Start chainage should be less than sanction length for " + (t) + "-" + (t + 0.5) + " km. range";
                                errorslist.Add(error);
                            }
                        }
                        if (formData.AddFDRStabilizeListModelObj[t].END_CHAINAGE_1 != null)
                        {
                            if (!(formData.AddFDRStabilizeListModelObj[t].END_CHAINAGE_1 >= t && formData.AddFDRStabilizeListModelObj[t].END_CHAINAGE_1 <= Convert.ToDecimal(t + 0.5)))
                            {
                                string error = "Please enter First End chainage within range of " + (t) + "-" + (t + 0.5) + " km.";
                                errorslist.Add(error);
                            }
                            if (formData.AddFDRStabilizeListModelObj[t].END_CHAINAGE_1 > formData.Sanction_length)
                            {
                                string error = "First End chainage should be less than sanction length for " + (t) + "-" + (t + 0.5) + " km. range";
                                errorslist.Add(error);
                            }
                        }

                        if (formData.AddFDRStabilizeListModelObj[t].START_CHAINAGE_1 != null && formData.AddFDRStabilizeListModelObj[t].END_CHAINAGE_1 != null)
                        {
                            if (formData.AddFDRStabilizeListModelObj[t].START_CHAINAGE_1 >= formData.AddFDRStabilizeListModelObj[t].END_CHAINAGE_1)
                            {
                                string error = "Start chainage should be less than end chainage for " + (t) + "-" + (t + 0.5) + " km range";
                                errorslist.Add(error);
                            }
                        }

                    }

                }



                for (int t = 0; t < formData.ROW_LENGTH; t++)
                {
                    if (formData.AddFDRStabilizeListModelObj[t].START_CHAINAGE_2 == null &&
                        formData.AddFDRStabilizeListModelObj[t].END_CHAINAGE_2 == null && formData.AddFDRStabilizeListModelObj[t].Chainage_Date_SecondChainage == null)
                    {
                        nullCount++;
                    }
                    else
                    {
                        if (formData.AddFDRStabilizeListModelObj[t].START_CHAINAGE_2 == null)
                        {
                            string error = "Please enter the start chainage for " + (t + 0.5) + "-" + (t + 1) + " km range";
                            errorslist.Add(error);
                        }
                        if (formData.AddFDRStabilizeListModelObj[t].END_CHAINAGE_2 == null)
                        {
                            string error = "Please enter the end chainage for " + (t + 0.5) + "-" + (t + 1) + " km range";
                            errorslist.Add(error);
                        }

                        if (formData.AddFDRStabilizeListModelObj[t].Chainage_Date_SecondChainage == null)
                        {
                            string error = "Please select the date for chainage " + (t + 0.5) + "-" + (t + 1) + " km range";
                            errorslist.Add(error);
                        }

                        if (formData.AddFDRStabilizeListModelObj[t].Chainage_Date_SecondChainage != null)
                        {

                            string tempDate = formData.AddFDRStabilizeListModelObj[t].Chainage_Date_SecondChainage;
                            DateTime fromDateValue;
                            var formats = new[] { "dd/MM/yyyy" };
                            if (!DateTime.TryParseExact(tempDate, formats, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out fromDateValue))
                            {
                                string error = "Invalid Second Chainage date entered for " + (t + 0.5) + "-" + (t + 1) + " km.";
                                errorslist.Add(error);
                            }
                            else if (Convert.ToDateTime(formData.AddFDRStabilizeListModelObj[t].Chainage_Date_SecondChainage) < SanctionDate)
                            {
                                string error = "First Chainage date can't be less than sanction date for " + (t + 0.5) + "-" + (t + 1) + " km.";
                                errorslist.Add(error);
                            }
                        }


                        if (formData.AddFDRStabilizeListModelObj[t].START_CHAINAGE_2 != null)
                        {
                            if (!(formData.AddFDRStabilizeListModelObj[t].START_CHAINAGE_2 >= Convert.ToDecimal(t + 0.5) && formData.AddFDRStabilizeListModelObj[t].START_CHAINAGE_2 <= Convert.ToDecimal(t + 1)))
                            {
                                string error = "Please enter Second Start chainage within range of " + (t + 0.5) + "-" + (t + 1) + " km.";
                                errorslist.Add(error);
                            }
                            if (formData.AddFDRStabilizeListModelObj[t].START_CHAINAGE_2 > formData.Sanction_length)
                            {
                                string error = "Second Start chainage should be less than sanction length for " + (t) + "-" + (t + 0.5) + " km. range";
                                errorslist.Add(error);
                            }
                        }
                        if (formData.AddFDRStabilizeListModelObj[t].END_CHAINAGE_2 != null)
                        {
                            if (!(formData.AddFDRStabilizeListModelObj[t].END_CHAINAGE_2 >= Convert.ToDecimal(t + 0.5) && formData.AddFDRStabilizeListModelObj[t].END_CHAINAGE_2 <= Convert.ToDecimal(t + 1)))
                            {
                                string error = "Please enter Second End chainage within range of " + (t + 0.5) + "-" + (t + 1) + " km.";
                                errorslist.Add(error);
                            }
                            if (formData.AddFDRStabilizeListModelObj[t].END_CHAINAGE_2 > formData.Sanction_length)
                            {
                                string error = "Second End chainage should be less than sanction length for " + (t) + "-" + (t + 0.5) + " km. range";
                                errorslist.Add(error);
                            }
                        }

                        if (formData.AddFDRStabilizeListModelObj[t].START_CHAINAGE_2 != null && formData.AddFDRStabilizeListModelObj[t].END_CHAINAGE_2 != null)
                        {
                            if (formData.AddFDRStabilizeListModelObj[t].START_CHAINAGE_2 >= formData.AddFDRStabilizeListModelObj[t].END_CHAINAGE_2)
                            {
                                string error = "Start chainage should be less than end chainage for " + (t + 0.5) + "-" + (t + 1) + " km range";
                                errorslist.Add(error);
                            }
                        }
                    }

                }



                if (nullCount == formData.ROW_LENGTH * 2)
                {
                    string error = "Please enter the details atleast for one row";
                    errorslist.Add(error);
                    return Json(new { Success = false, data = strErrorMessage.ToString(), ErrorMessage = "Please enter the details atleast for one row." });
                }
                if (errorslist.Count > 0)
                {
                    int e = 0;
                    foreach (var ierror in errorslist)
                    {
                        strErrorMessage.Append("<li style='margin-left:10px; list-style-type: square'> ");
                        strErrorMessage.Append("Error-" + ++e + " " + ierror);
                        strErrorMessage.Append("<br />");
                        strErrorMessage.Append("</li>");
                    }
                    return Json(new { Success = false, ErrorListgenerated = true, data = strErrorMessage.ToString(), ErrorMessage = "Please correct the errors shown in error summary" });
                }

                if (ModelState.IsValid)
                {
                    string Status = objDAL.SubmitFDRChainageDAL(formData);
                    if (Status == string.Empty)
                        return Json(new { Success = true, ErrorMessage = "FDR Stabilize base Chainage-Wise Detail added Successfully." });
                    else
                        return Json(new { Success = false, ErrorMessage = Status });
                }
                else
                {
                    return Json(new { Success = false, ErrorMessage = new CommonFunctions().FormatErrorMessage(ModelState) });
                }



            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "PMIS.SubmitFDRDetail()");
                return null;
            }
            finally
            {
                dbContext.Dispose();
            }
        }




        [HttpPost]
        [Audit]
        public ActionResult UpdateFDRDetail(AddFDRStabilizeModel formData)   //    FormCollection
        {
            PMISDAL objDAL = new PMISDAL();
            PMGSYEntities dbContext = new PMGSYEntities();
            try
            {
                int i = 0;
                int count = 0;
                //foreach (var item in formData)
                //{
                i++;
                int roadcode = formData.IMS_PR_ROAD_CODE;// item.IMS_PR_ROAD_CODE;


                var SanctionDate = dbContext.IMS_SANCTIONED_PROJECTS.Where(x => x.IMS_PR_ROAD_CODE == roadcode).Select(x => x.IMS_SANCTIONED_DATE).FirstOrDefault();


                List<string> errorslist = new List<string>();
                StringBuilder strErrorMessage = new StringBuilder("");
                // bool isRowAllDetailsFilled = true;
                int nullCount = 0;
                for (int t = 0; t < formData.ROW_LENGTH; t++)
                {
                    if (formData.AddFDRStabilizeListModelObj[t].START_CHAINAGE_1 == null &&
                        formData.AddFDRStabilizeListModelObj[t].END_CHAINAGE_1 == null && formData.AddFDRStabilizeListModelObj[t].Chainage_Date_FirstChainage == null)
                    {
                        nullCount++;
                    }
                    else
                    {

                        if (formData.AddFDRStabilizeListModelObj[t].START_CHAINAGE_1 == null)
                        {
                            string error = "Please enter the start chainage for " + t + "-" + (t + 0.5) + " km range";
                            errorslist.Add(error);
                        }
                        if (formData.AddFDRStabilizeListModelObj[t].END_CHAINAGE_1 == null)
                        {
                            string error = "Please enter the end chainage for " + t + "-" + (t + 0.5) + " km range";
                            errorslist.Add(error);
                        }
                        if (formData.AddFDRStabilizeListModelObj[t].Chainage_Date_FirstChainage == null)
                        {
                            string error = "Please select the date for chainage " + (t + 0.5) + "-" + (t + 1) + " km range";
                            errorslist.Add(error);
                        }

                        if (formData.AddFDRStabilizeListModelObj[t].Chainage_Date_FirstChainage != null)
                        {
                            string tempDate = formData.AddFDRStabilizeListModelObj[t].Chainage_Date_FirstChainage;
                            DateTime fromDateValue;
                            var formats = new[] { "dd/MM/yyyy" };
                            if (!DateTime.TryParseExact(tempDate, formats, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out fromDateValue))
                            {
                                string error = "Invalid First Chainage date entered for " + (t) + "-" + (t + 0.5) + " km.";
                                errorslist.Add(error);
                            }
                            else if (Convert.ToDateTime(formData.AddFDRStabilizeListModelObj[t].Chainage_Date_FirstChainage) < SanctionDate)
                            {
                                string error = "First Chainage date can't be less than sanction date for " + (t) + "-" + (t + 0.5) + " km.";
                                errorslist.Add(error);
                            }
                        }


                        if (formData.AddFDRStabilizeListModelObj[t].START_CHAINAGE_1 != null)
                        {
                            if (!(formData.AddFDRStabilizeListModelObj[t].START_CHAINAGE_1 >= t && formData.AddFDRStabilizeListModelObj[t].START_CHAINAGE_1 <= Convert.ToDecimal(t + 0.5)))
                            {
                                string error = "Please enter First Start chainage within range of " + (t) + "-" + (t + 0.5) + " km.";
                                errorslist.Add(error);
                            }
                            if (formData.AddFDRStabilizeListModelObj[t].START_CHAINAGE_1 > formData.Sanction_length)
                            {
                                string error = "First Start chainage should be less than sanction length for " + (t) + "-" + (t + 0.5) + " km. range";
                                errorslist.Add(error);
                            }
                        }
                        if (formData.AddFDRStabilizeListModelObj[t].END_CHAINAGE_1 != null)
                        {
                            if (!(formData.AddFDRStabilizeListModelObj[t].END_CHAINAGE_1 >= t && formData.AddFDRStabilizeListModelObj[t].END_CHAINAGE_1 <= Convert.ToDecimal(t + 0.5)))
                            {
                                string error = "Please enter First End chainage within range of " + (t) + "-" + (t + 0.5) + " km.";
                                errorslist.Add(error);
                            }
                            if (formData.AddFDRStabilizeListModelObj[t].END_CHAINAGE_1 > formData.Sanction_length)
                            {
                                string error = "First end chainage should be less than sanction length for " + (t) + "-" + (t + 0.5) + " km. range";
                                errorslist.Add(error);
                            }
                        }

                        if (formData.AddFDRStabilizeListModelObj[t].START_CHAINAGE_1 != null && formData.AddFDRStabilizeListModelObj[t].END_CHAINAGE_1 != null)
                        {
                            if (formData.AddFDRStabilizeListModelObj[t].START_CHAINAGE_1 >= formData.AddFDRStabilizeListModelObj[t].END_CHAINAGE_1)
                            {
                                string error = "Start chainage should be less than end chainage for " + (t) + "-" + (t + 0.5) + " km range";
                                errorslist.Add(error);
                            }
                        }

                    }

                }



                for (int t = 0; t < formData.ROW_LENGTH; t++)
                {
                    if (formData.AddFDRStabilizeListModelObj[t].START_CHAINAGE_2 == null &&
                        formData.AddFDRStabilizeListModelObj[t].END_CHAINAGE_2 == null && formData.AddFDRStabilizeListModelObj[t].Chainage_Date_SecondChainage == null)
                    {
                        nullCount++;
                    }
                    else
                    {


                        if (formData.AddFDRStabilizeListModelObj[t].START_CHAINAGE_2 == null)
                        {
                            string error = "Please enter the start chainage for " + (t + 0.5) + "-" + (t + 1) + " km range";
                            errorslist.Add(error);
                        }
                        if (formData.AddFDRStabilizeListModelObj[t].END_CHAINAGE_2 == null)
                        {
                            string error = "Please enter the end chainage for " + (t + 0.5) + "-" + (t + 1) + " km range";
                            errorslist.Add(error);
                        }

                        if (formData.AddFDRStabilizeListModelObj[t].Chainage_Date_SecondChainage == null)
                        {
                            string error = "Please select the date for chainage " + (t + 0.5) + "-" + (t + 1) + " km range";
                            errorslist.Add(error);
                        }

                        if (formData.AddFDRStabilizeListModelObj[t].Chainage_Date_SecondChainage != null)
                        {

                            string tempDate = formData.AddFDRStabilizeListModelObj[t].Chainage_Date_SecondChainage;
                            DateTime fromDateValue;
                            var formats = new[] { "dd/MM/yyyy" };
                            if (!DateTime.TryParseExact(tempDate, formats, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out fromDateValue))
                            {
                                string error = "Invalid Second Chainage date entered for " + (t + 0.5) + "-" + (t + 1) + " km.";
                                errorslist.Add(error);
                            }
                            else if (Convert.ToDateTime(formData.AddFDRStabilizeListModelObj[t].Chainage_Date_SecondChainage) < SanctionDate)
                            {
                                string error = "First Chainage date can't be less than sanction date for " + (t + 0.5) + "-" + (t + 1) + " km.";
                                errorslist.Add(error);
                            }
                        }


                        if (formData.AddFDRStabilizeListModelObj[t].START_CHAINAGE_2 != null)
                        {
                            if (!(formData.AddFDRStabilizeListModelObj[t].START_CHAINAGE_2 >= Convert.ToDecimal(t + 0.5) && formData.AddFDRStabilizeListModelObj[t].START_CHAINAGE_2 <= Convert.ToDecimal(t + 1)))
                            {
                                string error = "Please enter Second Start chainage within range of " + (t + 0.5) + "-" + (t + 1) + " km.";
                                errorslist.Add(error);
                            }
                            if (formData.AddFDRStabilizeListModelObj[t].START_CHAINAGE_2 > formData.Sanction_length)
                            {
                                string error = "Second Start chainage should be less than sanction length for " + (t) + "-" + (t + 0.5) + " km. range";
                                errorslist.Add(error);
                            }
                        }
                        if (formData.AddFDRStabilizeListModelObj[t].END_CHAINAGE_2 != null)
                        {
                            if (!(formData.AddFDRStabilizeListModelObj[t].END_CHAINAGE_2 >= Convert.ToDecimal(t + 0.5) && formData.AddFDRStabilizeListModelObj[t].END_CHAINAGE_2 <= Convert.ToDecimal(t + 1)))
                            {
                                string error = "Please enter Second End chainage within range of " + (t + 0.5) + "-" + (t + 1) + " km.";
                                errorslist.Add(error);
                            }
                            if (formData.AddFDRStabilizeListModelObj[t].END_CHAINAGE_2 > formData.Sanction_length)
                            {
                                string error = "Second end chainage should be less than sanction length for " + (t) + "-" + (t + 0.5) + " km. range";
                                errorslist.Add(error);
                            }
                        }

                        if (formData.AddFDRStabilizeListModelObj[t].START_CHAINAGE_2 != null && formData.AddFDRStabilizeListModelObj[t].END_CHAINAGE_2 != null)
                        {
                            if (formData.AddFDRStabilizeListModelObj[t].START_CHAINAGE_2 >= formData.AddFDRStabilizeListModelObj[t].END_CHAINAGE_2)
                            {
                                string error = "Start chainage should be less than end chainage for " + (t + 0.5) + "-" + (t + 1) + " km range";
                                errorslist.Add(error);
                            }

                        }
                    }

                }



                if (nullCount == formData.ROW_LENGTH * 2)
                {
                    string error = "Please enter the details atleast for one row";
                    errorslist.Add(error);
                    return Json(new { Success = false, data = strErrorMessage.ToString(), ErrorMessage = "Please enter the details atleast for one row." });
                }
                if (errorslist.Count > 0)
                {
                    int e = 0;
                    foreach (var ierror in errorslist)
                    {
                        strErrorMessage.Append("<li style='margin-left:10px; list-style-type: square'> ");
                        strErrorMessage.Append("Error-" + ++e + " " + ierror);
                        strErrorMessage.Append("<br />");
                        strErrorMessage.Append("</li>");
                    }
                    return Json(new { Success = false, ErrorListgenerated = true, data = strErrorMessage.ToString(), ErrorMessage = "Please correct the errors shown in error summary" });
                }

                if (ModelState.IsValid)
                {
                    string Status = objDAL.UpdateFDRChainageDAL(formData);
                    if (Status == string.Empty)
                        return Json(new { Success = true, ErrorMessage = "FDR Stabilize base Chainage-Wise Detail added Successfully." });
                    else
                        return Json(new { Success = false, ErrorMessage = Status });
                }
                else
                {
                    return Json(new { Success = false, ErrorMessage = new CommonFunctions().FormatErrorMessage(ModelState) });
                }



            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "PMIS.SubmitFDRDetail()");
                return null;
            }
            finally
            {
                dbContext.Dispose();
            }
        }


        [HttpPost]
        [Audit]
        public ActionResult UpdateFDRDetail1(AddFDRStabilizeModel formData)
        {
            PMISDAL objDAL = new PMISDAL();
            PMGSYEntities dbContext = new PMGSYEntities();
            try
            {
                int i = 0;
                int count = 0;
                //foreach (var item in formData)
                //{
                //    i++;
                //    int roadcode = item.IMS_PR_ROAD_CODE;
                //    var SanctionDate = dbContext.IMS_SANCTIONED_PROJECTS.Where(x => x.IMS_PR_ROAD_CODE == roadcode).Select(x => x.IMS_SANCTIONED_DATE).FirstOrDefault();


                //    //if ((item.CHAINAGE_FROM == null) && (item.CHAINAGE_TO == null) && (item.CHAINAGE_DATE == null))
                //    //{
                //    //    count++;
                //    //    if(count==5)
                //    //    {
                //    //        return Json(new { Success = false, ErrorMessage = "At Least one Start and End Chainage, Chainage Date is required." });
                //    //    }

                //    //}
                //    //else
                //    //{
                //    //    if ((item.CHAINAGE_FROM != null) && (item.CHAINAGE_TO == null || item.CHAINAGE_DATE == null))
                //    //    {
                //    //        return Json(new { Success = false, ErrorMessage = "End Chainage and Chainage Date required when Chainage From is entered for Sr No " + i });
                //    //    }
                //    //    else if ((item.CHAINAGE_TO != null) && (item.CHAINAGE_FROM == null || item.CHAINAGE_DATE == null))
                //    //    {
                //    //        return Json(new { Success = false, ErrorMessage = "Start Chainage and Chainage Date required when Chainage To is entered for Sr No " + i });
                //    //    }
                //    //    else if ((item.CHAINAGE_DATE != null) && (item.CHAINAGE_FROM == null || item.CHAINAGE_TO == null))
                //    //    {
                //    //        return Json(new { Success = false, ErrorMessage = "Start and End Chainage required when Chainage Date entered for Sr No " + i });
                //    //    }
                //    //}

                //    //if ((item.CHAINAGE_FROM != null) && (item.CHAINAGE_TO != null) && (item.CHAINAGE_DATE != null))
                //    //{

                //    //    try
                //    //    {
                //    //        if (item.CHAINAGE_DATE.Value.Day < SanctionDate.Value.Day && item.CHAINAGE_DATE.Value.Month == SanctionDate.Value.Month && item.CHAINAGE_DATE.Value.Year == SanctionDate.Value.Year)
                //    //        {
                //    //            return Json(new { Success = false, ErrorMessage = "Date Entered for Chainage should not be less than Sanction Date for Sr no. " + i });
                //    //        }

                //    //        if (item.CHAINAGE_TO <= item.CHAINAGE_FROM)
                //    //        {
                //    //            return Json(new { Success = false, ErrorMessage = "Start chainage must be less than end chainage From for Sr no. " + i });
                //    //        }
                //    //    }
                //    //    catch (Exception ex)
                //    //    {
                //    //        ErrorLog.LogError(ex, "PMIS.SubmitFDRDetail()");
                //    //        return Json(new { Success = false, ErrorMessage = "Date Entered for Chainage not in Correct Format." });
                //    //    }

                //    //    var SanctionLength = dbContext.IMS_SANCTIONED_PROJECTS.Where(x => x.IMS_PR_ROAD_CODE == roadcode).Select(x => x.IMS_PAV_LENGTH).FirstOrDefault();
                //    //    if (item.CHAINAGE_FROM > SanctionLength || item.CHAINAGE_TO > SanctionLength)
                //    //    {
                //    //        return Json(new { Success = false, ErrorMessage = "Start and End Chainage Should not be greater than Sanction Length of Road for Sr No " + i });
                //    //    }
                //    //}


                //}
                string Status = objDAL.UpdateFDRChainageDAL1(formData);
                if (Status == string.Empty)
                    return Json(new { Success = true, ErrorMessage = "FDR Stabilize base Chainage-Wise Detail Updated Successfully." });
                else
                    return Json(new { Success = false, ErrorMessage = Status });
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "PMIS.SubmitFDRDetail()");
                return null;
            }
            finally
            {
                dbContext.Dispose();
            }
            return null;
        }


        [HttpPost]
        public ActionResult PMISFdrList(int? page, int? rows, string sidx, string sord)
        {
            int RoadCode = 0; long totalRecords;
            try
            {
                if (!string.IsNullOrEmpty(Request.Params["IMS_PR_ROAD_CODE"]))
                {
                    RoadCode = Convert.ToInt32(Request.Params["IMS_PR_ROAD_CODE"]);
                }

                PMISDAL dalobj = new PMISDAL();
                var jsonData = new
                {
                    rows = dalobj.PMISFDRStabListDAL(RoadCode, Convert.ToInt32(Request.Params["page"]) - 1, Convert.ToInt32(Request.Params["rows"]), Request.Params["sidx"], Request.Params["sord"], out totalRecords),
                    total = totalRecords <= Convert.ToInt32(Request.Params["rows"]) ? 1 : totalRecords / Convert.ToInt32(Request.Params["rows"]) + 1,
                    page = Convert.ToInt32(Request.Params["page"]),
                    records = totalRecords
                };
                return Json(jsonData, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "PMISController.PMISFdrList()");
                return null;
            }
        }


        #endregion


        #region Added By Hrishikesh To Add "Trail Strech For FDR" --05-06-2023 ---start

        [HttpGet]
        [Audit]
        public ActionResult AddTrailStrechForFDR(String parameter, String hash, String key)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            CommonFunctions commObj = new CommonFunctions();
            AddTrailStrechForFDRModel addTrailStrechForFDRModelObj = new AddTrailStrechForFDRModel();
            List<SelectListItem> addititveIdList = new List<SelectListItem>();
            List<SelectListItem> jmfIdList = new List<SelectListItem>();
            Dictionary<string, string> decryptedParameters = null;
            try
            {

                decryptedParameters = URLEncrypt.DecryptParameters1(new string[] { parameter, hash, key });
                if (decryptedParameters.Count > 0)
                {
                    var roadCode = Convert.ToInt32(decryptedParameters["RoadCode"].ToString().Trim());
                    //var SanctionedObj = dbContext.IMS_SANCTIONED_PROJECTS.Where(x => x.IMS_PR_ROAD_CODE == roadCode).Select(y => new { y.IMS_SANCTIONED_DATE, y.MAST_BLOCK_CODE,y.IMS_YEAR,y.IMS_ROAD_NAME }).FirstOrDefault();
                    var SanctionedObj = (from ISP in dbContext.IMS_SANCTIONED_PROJECTS
                                         where ISP.IMS_PR_ROAD_CODE == roadCode
                                         select new
                                         {
                                             ISP.IMS_SANCTIONED_DATE,
                                             ISP.MAST_BLOCK_CODE,
                                             ISP.IMS_YEAR,
                                             ISP.IMS_ROAD_NAME
                                         }).FirstOrDefault();
                    ViewBag.IMS_PR_ROAD_CODE = roadCode;
                    ViewBag.StateName = PMGSYSession.Current.StateName;
                    ViewBag.District = PMGSYSession.Current.DistrictName;
                    ViewBag.Block = SanctionedObj.MAST_BLOCK_CODE != null ? dbContext.MASTER_BLOCK.Where(x => x.MAST_BLOCK_CODE == SanctionedObj.MAST_BLOCK_CODE).Select(y => y.MAST_BLOCK_NAME).FirstOrDefault() : "-";
                    ViewBag.PackageName = decryptedParameters["PackageName"].ToString();
                    ViewBag.SanctionedYear = SanctionedObj.IMS_YEAR != null ? dbContext.MASTER_YEAR.Where(x => x.MAST_YEAR_CODE == SanctionedObj.IMS_YEAR).Select(y => y.MAST_YEAR_TEXT).FirstOrDefault() : "-";
                    ViewBag.SanctionedDate = SanctionedObj.IMS_SANCTIONED_DATE.ToString() != null ? string.Format("{0:d}", SanctionedObj.IMS_SANCTIONED_DATE) : "";
                    ViewBag.Batch = decryptedParameters["BatchName"];
                    ViewBag.Length = decryptedParameters["Length"];
                    ViewBag.RoadName = SanctionedObj.IMS_ROAD_NAME != null ? SanctionedObj.IMS_ROAD_NAME : "";

                    var TechnologyList = string.Join(", ", dbContext.IMS_PROPOSAL_TECH.Where(x => x.IMS_PR_ROAD_CODE == roadCode).Select(x => x.MASTER_TECHNOLOGY.MAST_TECH_NAME).ToList());
                    ViewBag.TechName = TechnologyList;

                    //model values
                    var masterAdditibeObjList = dbContext.MASTER_ADDITIVE.Select(x => new { x.ADDITIVE_ID, x.ADDITIVE_NAME }).ToList();
                    for (var i = 0; i < masterAdditibeObjList.Count; i++)
                    {
                        addititveIdList.Add(new SelectListItem { Text = masterAdditibeObjList[i].ADDITIVE_NAME, Value = masterAdditibeObjList[i].ADDITIVE_ID.ToString() });
                    }

                    //addititveIdList.Insert(0, (new SelectListItem { Text = "All Options", Value = "0", Selected = true }));

                    addTrailStrechForFDRModelObj.ADDITIVE_ID_LIST = addititveIdList;

                    var masterJMFObjList = dbContext.MASTER_JMF.Select(x => new { x.JMF_ID, x.JMF_PREP_NAME }).ToList();
                    jmfIdList.Add(new SelectListItem { Text = "Select", Value = "0" });
                    for (var i = 0; i < masterJMFObjList.Count; i++)
                    {
                        jmfIdList.Add(new SelectListItem { Text = masterJMFObjList[i].JMF_PREP_NAME, Value = masterJMFObjList[i].JMF_ID.ToString() });
                    }

                    addTrailStrechForFDRModelObj.JMF_ID_LIST = jmfIdList;

                    addTrailStrechForFDRModelObj.IMS_PR_ROAD_CODE = roadCode;

                    var isFDRFilled = dbContext.PMIS_TRIAL_STRETCH_FDR_DETAIL.Any(x => x.IMS_PR_ROAD_CODE == roadCode);


                    //for update/edit or View
                    if (isFDRFilled)
                    {
                        var addTrailStrechForFDRObj = dbContext.PMIS_TRIAL_STRETCH_FDR_DETAIL.Where(x => x.IMS_PR_ROAD_CODE == roadCode).FirstOrDefault();
                        addTrailStrechForFDRModelObj.IS_Finalized = dbContext.PMIS_TRIAL_STRETCH_FDR_DETAIL.Any(x => x.IMS_PR_ROAD_CODE == roadCode && x.IS_FINALIZED == "Y") ? "Y" : "N";
                        //ViewBag.isFDRFilled = "Y";
                        addTrailStrechForFDRModelObj.IS_FDR_FILLED = "Y";
                        //var addTrailStrechForFDRObj = dbContext.PMIS_TRIAL_STRETCH_FDR_DETAIL.Where(x => x.IMS_PR_ROAD_CODE == roadCode).FirstOrDefault();

                        var additiveIds = dbContext.PMIS_ADDITIVE_USED_DETAIL.Where(x => x.TRIAL_STRETCH_ID == addTrailStrechForFDRObj.TRIAL_STRETCH_ID).Select(y => y.ADDITIVE_ID).ToList();
                        //ViewBag.Additive_List = string.Join(", ", dbContext.MASTER_ADDITIVE.Where(x => x.TRIAL_STRETCH_ID == addTrailStrechForFDRObj.TRIAL_STRETCH_ID).ToList());
                        var Additive_List = string.Empty;
                        for (var i = 0; i < additiveIds.Count; i++)
                        {
                            int additiveid = additiveIds[i];
                            if (i == 0)
                            {
                                Additive_List = Additive_List + dbContext.MASTER_ADDITIVE.Where(x => x.ADDITIVE_ID == additiveid).Select(y => y.ADDITIVE_NAME).FirstOrDefault();
                            }
                            else
                                Additive_List = Additive_List + "," + dbContext.MASTER_ADDITIVE.Where(x => x.ADDITIVE_ID == additiveid).Select(y => y.ADDITIVE_NAME).FirstOrDefault();
                        }
                        ViewBag.Additive_List = Additive_List;
                        ViewBag.JMF_List = dbContext.MASTER_JMF.Where(x => x.JMF_ID == addTrailStrechForFDRObj.JMF_ID).Select(y => y.JMF_PREP_NAME).FirstOrDefault();

                        //addTrailStrechForFDRModelObj.ADDITIVE_ID_LIST = dbContext.PMIS_ADDITIVE_USED_DETAIL.Where(x=>x.TRIAL_STRETCH_ID== addTrailStrechForFDRObj.TRIAL_STRETCH_ID).ToList();
                        addTrailStrechForFDRModelObj.SELECTED_ADDITIVES_DB = string.Join(",", dbContext.PMIS_ADDITIVE_USED_DETAIL.Where(x => x.IMS_PR_ROAD_CODE == roadCode).Select(x => x.ADDITIVE_ID).ToList());
                        /*addTrailStrechForFDRModelObj.SELECTED_ADDITIVES_DB= string.Join(", ", from ma in dbContext.MASTER_ADDITIVE
                                                                                              join paud in dbContext.PMIS_ADDITIVE_USED_DETAIL on ma.ADDITIVE_ID equals paud.ADDITIVE_ID
                                                                                              where paud.IMS_PR_ROAD_CODE == roadCode
                                                                                              select ma.ADDITIVE_NAME );*/

                        addTrailStrechForFDRModelObj.JMF_ID = addTrailStrechForFDRObj.JMF_ID.ToString();
                        addTrailStrechForFDRModelObj.IS_ADDITIVE_CONT_PERC_ML = addTrailStrechForFDRObj.ADDITIVE_CONTENT_UNIT.ToString().Trim();
                        addTrailStrechForFDRModelObj.ADDITIVE_CONT_UNIT = addTrailStrechForFDRObj.ADDITIVE_CONTENT_UNIT.ToString().Trim();
                        addTrailStrechForFDRModelObj.ADDITIVE_CONT_ML_CUM = addTrailStrechForFDRObj.ADDITIVE_CONTENT_UNIT.ToString().Trim() == "ml/cum" ? addTrailStrechForFDRObj.ML_CUM_ADDITIVE_CONTENT.ToString() : "";
                        addTrailStrechForFDRModelObj.PERC_ADDITIVE_CONT = addTrailStrechForFDRObj.ADDITIVE_CONTENT_UNIT.ToString().Trim() == "%" ? addTrailStrechForFDRObj.PERC_ADDITIVE_CONTENT.ToString() : "";
                        addTrailStrechForFDRModelObj.PERC_CEMENT_CONT = addTrailStrechForFDRObj.PERC_CEMENT_CONTENT;
                        addTrailStrechForFDRModelObj.CEMENT_TYPE = addTrailStrechForFDRObj.CEMENT_TYPE == null ? "" : addTrailStrechForFDRObj.CEMENT_TYPE;
                        addTrailStrechForFDRModelObj.CEMENT_GRADE = addTrailStrechForFDRObj.CEMENT_GRADE == null ? "" : addTrailStrechForFDRObj.CEMENT_GRADE;
                        addTrailStrechForFDRModelObj.AVG_THICK_CRUST = addTrailStrechForFDRObj.AVG_THICK_CRUST.ToString() == null ? "" : addTrailStrechForFDRObj.AVG_THICK_CRUST.ToString();
                        addTrailStrechForFDRModelObj.IS_GRAD_MATERIAL_SPEC_LIMIT = addTrailStrechForFDRObj.IS_GRAD_MATERIAL_SPEC_LIMIT == null ? "" : addTrailStrechForFDRObj.IS_GRAD_MATERIAL_SPEC_LIMIT.ToString().Trim();
                        addTrailStrechForFDRModelObj.PERC_PLASTICITY_RECLAM_SOIL = addTrailStrechForFDRObj.PERC_PLASTICITY_RECLAM_SOIL.ToString() == null ? "" : addTrailStrechForFDRObj.PERC_PLASTICITY_RECLAM_SOIL.ToString();
                        addTrailStrechForFDRModelObj.PERC_INDEX_SUBGRADE_SOIL = addTrailStrechForFDRObj.PERC_PLASTICITY_SUBGRADE_SOIL.ToString() == null ? "" : addTrailStrechForFDRObj.PERC_PLASTICITY_SUBGRADE_SOIL.ToString();
                        addTrailStrechForFDRModelObj.IS_AVG_UCS_7D_28D = addTrailStrechForFDRObj.IS_AVG_UCS_7D_28D == null ? "" : addTrailStrechForFDRObj.IS_AVG_UCS_7D_28D.ToString().Trim();
                        addTrailStrechForFDRModelObj.AVG_UCS_7D = addTrailStrechForFDRObj.AVG_UCS_7DAY == null ? "" : addTrailStrechForFDRObj.AVG_UCS_7DAY.ToString().Trim();
                        addTrailStrechForFDRModelObj.AVG_UCS_28D = addTrailStrechForFDRObj.AVG_UCS_28DAY == null ? "" : addTrailStrechForFDRObj.AVG_UCS_28DAY.ToString().Trim();
                        addTrailStrechForFDRModelObj.IS_UCS_TEST_CUBE_CYLINDER = addTrailStrechForFDRObj.IS_UCS_CUBE_OR_CYLINDER == null ? "" : addTrailStrechForFDRObj.IS_UCS_CUBE_OR_CYLINDER.ToString().Trim();
                        addTrailStrechForFDRModelObj.UCS_TEST_CUBE = addTrailStrechForFDRObj.TEST_UCS_CUBE == null ? "" : addTrailStrechForFDRObj.TEST_UCS_CUBE.ToString().Trim();
                        addTrailStrechForFDRModelObj.UCS_TEST_CYLINDER = addTrailStrechForFDRObj.TEST_UCS_CYLINDER == null ? "" : addTrailStrechForFDRObj.TEST_UCS_CYLINDER.ToString().Trim();
                        addTrailStrechForFDRModelObj.OMC_MIX = addTrailStrechForFDRObj.OMC_MIX.ToString() == null ? "" : addTrailStrechForFDRObj.OMC_MIX.ToString().Trim();
                        addTrailStrechForFDRModelObj.MDD_MIX = addTrailStrechForFDRObj.MDD_MIX.ToString().Trim() == null ? "" : addTrailStrechForFDRObj.MDD_MIX.ToString().Trim();
                        addTrailStrechForFDRModelObj.TEST_RESULT_FILE_NAME = addTrailStrechForFDRObj.TEST_RESULT_UCS_FILE_NAME == null ? "" : addTrailStrechForFDRObj.TEST_RESULT_UCS_FILE_NAME.ToString().Trim();
                        addTrailStrechForFDRModelObj.TEST_RESULT_FILE_PATH = addTrailStrechForFDRObj.TEST_RESULT_UCS_FILE_PATH == null ? "" : addTrailStrechForFDRObj.TEST_RESULT_UCS_FILE_PATH.ToString().Trim();

                        addTrailStrechForFDRModelObj.IS_TS_UCS_STRENGTH_CUBE_CYLINDER = addTrailStrechForFDRObj.IS_TS_UCS_CUBE_CYLINDRICAL == null ? "" : addTrailStrechForFDRObj.IS_TS_UCS_CUBE_CYLINDRICAL.ToString().Trim();
                        addTrailStrechForFDRModelObj.UCS_STRENGTH_CUBE = addTrailStrechForFDRObj.TS_TEST_UCS_CUBE == null ? "" : addTrailStrechForFDRObj.TS_TEST_UCS_CUBE.ToString().Trim();
                        addTrailStrechForFDRModelObj.UCS_STRENGTH_CYLINDER = addTrailStrechForFDRObj.TS_TEST_UCS_CYLINDRICAL == null ? "" : addTrailStrechForFDRObj.TS_TEST_UCS_CYLINDRICAL.ToString().Trim();
                        addTrailStrechForFDRModelObj.IS_TS_UCS_7D_28D = addTrailStrechForFDRObj.IS_TS_UCS_7D_28D == null ? "" : addTrailStrechForFDRObj.IS_TS_UCS_7D_28D.ToString().Trim();
                        addTrailStrechForFDRModelObj.TS_UCS_28D = addTrailStrechForFDRObj.TS_UCS_28D == null ? "" : addTrailStrechForFDRObj.TS_UCS_28D.ToString().Trim();
                        addTrailStrechForFDRModelObj.TS_UCS_7D = addTrailStrechForFDRObj.TS_UCS_7D == null ? "" : addTrailStrechForFDRObj.TS_UCS_7D.ToString().Trim();
                        addTrailStrechForFDRModelObj.TS_CORE_TRIAL_STRETCH_FILE_NAME = addTrailStrechForFDRObj.TS_TEST_RESULT_FILE_NAME == null ? "" : addTrailStrechForFDRObj.TS_TEST_RESULT_FILE_NAME.ToString().Trim();
                        addTrailStrechForFDRModelObj.TS_CORE_TRIAL_STRETCH_FILE_PATH = addTrailStrechForFDRObj.TS_TEST_RESULT_FILE_PATH == null ? "" : addTrailStrechForFDRObj.TS_TEST_RESULT_FILE_PATH.ToString().Trim();
                        addTrailStrechForFDRModelObj.TS_RESD_STRENGTH_WETT = addTrailStrechForFDRObj.TS_RESD_STRENGTH_WETTING == null ? "" : addTrailStrechForFDRObj.TS_RESD_STRENGTH_WETTING.ToString().Trim();
                        addTrailStrechForFDRModelObj.TS_RESD_STRENGTH_WETT_FILE_NAME = addTrailStrechForFDRObj.TS_RESD_STRENGTH_FILE_NAME == null ? "" : addTrailStrechForFDRObj.TS_RESD_STRENGTH_FILE_NAME.ToString().Trim();
                        addTrailStrechForFDRModelObj.TS_RESD_STRENGTH_WETT_FILE_PATH = addTrailStrechForFDRObj.TS_RESD_STRENGTH_FILE_PATH == null ? "" : addTrailStrechForFDRObj.TS_RESD_STRENGTH_FILE_PATH.ToString().Trim();

                        addTrailStrechForFDRModelObj.CRACK_RELIEF_LAYER = addTrailStrechForFDRObj.CRACK_RELIEF_LAYER == null ? "" : addTrailStrechForFDRObj.CRACK_RELIEF_LAYER.ToString().Trim();
                        addTrailStrechForFDRModelObj.OTHER_CRACK_LAYER = addTrailStrechForFDRObj.CRACK_RELIEF_LAYER.ToString().Trim() == "Other" ? addTrailStrechForFDRObj.OTHER_CRACK_RELIEF_LAYER.ToString().Trim() : "";
                        addTrailStrechForFDRModelObj.STRETCH_LENGTH = addTrailStrechForFDRObj.STRETCH_LENGTH.ToString() == null ? "" : addTrailStrechForFDRObj.STRETCH_LENGTH.ToString().Trim();
                        addTrailStrechForFDRModelObj.STRETCH_CONSTR_DATE = commObj.GetDateTimeToString(addTrailStrechForFDRObj.STRETCH_CONSTR_DATE) == null ? " " : commObj.GetDateTimeToString(addTrailStrechForFDRObj.STRETCH_CONSTR_DATE);

                        if (addTrailStrechForFDRModelObj.IS_FDR_FILLED == "Y" && addTrailStrechForFDRModelObj.IS_Finalized == "Y")
                        {
                            return View("~/Areas/PMIS/Views/PMIS/AddTrailStrechForFDR.cshtml", addTrailStrechForFDRModelObj);
                        }
                        else
                        {
                            // EditTrailStrechForFDR
                            return View("~/Areas/PMIS/Views/PMIS/EditTrailStrechForFDR.cshtml", addTrailStrechForFDRModelObj);
                        }
                    }
                    else
                    {
                        //ViewBag.isFDRFilled = "N";
                        addTrailStrechForFDRModelObj.IS_FDR_FILLED = "N";
                        return View("~/Areas/PMIS/Views/PMIS/AddTrailStrechForFDR.cshtml", addTrailStrechForFDRModelObj);
                    }

                }
                else
                {
                    return null;
                }
                return View("~/Areas/PMIS/Views/PMIS/AddTrailStrechForFDR.cshtml", addTrailStrechForFDRModelObj);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "AddTrailStrechForFDR()");
                return null;
            }

        }

        [RequiredAuthentication]
        public JsonResult viewFdrFileTestResult(string id, string timestamp)
        {
            PMGSYEntities dbContext = new PMGSYEntities();

            try
            {
                //var splitarr = id.Split('$');

                long tStamp = Convert.ToInt64(timestamp);
                int roadCode = Convert.ToInt32(id);
                //string path = splitarr[1];
                var model = dbContext.PMIS_TRIAL_STRETCH_FDR_DETAIL.Where(s => s.IMS_PR_ROAD_CODE == roadCode).FirstOrDefault();
                string uploaded_path = string.Empty;

                /*                //D:\OMMASII\PMIS\UPLOADS\2023\6  --Example
                                *//* var month = model.JMF_FILE_PATH.ToString().Trim().Split('\\'); *//*
                                var month = model.JMF_FILE_PATH.ToString().Trim().Split('\\')[Convert.ToInt32(model.JMF_FILE_PATH.ToString().Trim().Split('\\').Length) - 1];
                                var year = model.JMF_FILE_PATH.ToString().Trim().Split('\\')[Convert.ToInt32(model.JMF_FILE_PATH.ToString().Trim().Split('\\').Length) - 2];*/

                //chnage on 19-06-2023
                if (model.TS_TEST_RESULT_FILE_NAME == "")
                {
                    return Json(new { success = false, Message = "File not Exist" }, JsonRequestBehavior.AllowGet);
                }
                //D:\OMMASII\PMIS\UPLOADS\2023\6  --Example
                var month = model.TS_TEST_RESULT_FILE_PATH.ToString().Trim().Split('\\')[Convert.ToInt32(model.TS_TEST_RESULT_FILE_PATH.ToString().Trim().Split('\\').Length) - 1];
                var year = model.TS_TEST_RESULT_FILE_PATH.ToString().Trim().Split('\\')[Convert.ToInt32(model.TS_TEST_RESULT_FILE_PATH.ToString().Trim().Split('\\').Length) - 2];

                string VirtualDirectoryPath = ConfigurationManager.AppSettings["PMIS_TRIAL_STRECH_FOR_FDR_UPLOAD_VIRTUAL_PATH"].ToString() + "\\" + year + "\\" + month;
                string VirtualDirectoryfullPath = string.Empty;
                var fileName = model.TS_TEST_RESULT_FILE_NAME;

                uploaded_path = ConfigurationManager.AppSettings["PMIS_TRIAL_STRECH_FOR_FDR_UPLOAD_PATH"].ToString() + "\\" + year + "\\" + month;
                VirtualDirectoryfullPath = Path.Combine(VirtualDirectoryPath, fileName.ToString()).ToString().Replace(@"\\", @"//").Replace(@"\", @"/");
                string physicalFullPath = Path.Combine(uploaded_path, fileName);

                FileInfo file = new FileInfo(physicalFullPath);
                if (file.Exists)
                {
                    return Json(new { success = true, Message = VirtualDirectoryfullPath }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { success = false, Message = "File not found" }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "PMIS.viewFdrFileTestResult()");
                return Json(new { success = false, Message = "Some problem occured. Please try after sometime." }, JsonRequestBehavior.AllowGet);
            }
            finally
            {
                dbContext.Dispose();
            }

        }

        [RequiredAuthentication]
        public JsonResult viewFdrFileJMF(string id, string timestamp)
        {
            PMGSYEntities dbContext = new PMGSYEntities();

            try
            {
                //var splitarr = id.Split('$');
                long tStamp = Convert.ToInt64(timestamp);
                int roadCode = Convert.ToInt32(id);
                //string path = splitarr[1];
                var model = dbContext.PMIS_TRIAL_STRETCH_FDR_DETAIL.Where(s => s.IMS_PR_ROAD_CODE == roadCode).FirstOrDefault();
                string uploaded_path = string.Empty;

                if (model.TS_RESD_STRENGTH_FILE_NAME == "")
                {
                    return Json(new { success = false, Message = "File not Exist" }, JsonRequestBehavior.AllowGet);
                }
                //D:\OMMASII\PMIS\UPLOADS\2023\6  --Example
                /* var month = model.JMF_FILE_PATH.ToString().Trim().Split('\\'); */
                var month = model.TS_RESD_STRENGTH_FILE_PATH.ToString().Trim().Split('\\')[Convert.ToInt32(model.TS_RESD_STRENGTH_FILE_PATH.ToString().Trim().Split('\\').Length) - 1];
                var year = model.TS_RESD_STRENGTH_FILE_PATH.ToString().Trim().Split('\\')[Convert.ToInt32(model.TS_RESD_STRENGTH_FILE_PATH.ToString().Trim().Split('\\').Length) - 2];

                string VirtualDirectoryPath = ConfigurationManager.AppSettings["PMIS_TRIAL_STRECH_FOR_FDR_UPLOAD_VIRTUAL_PATH"].ToString() + "\\" + year + "\\" + month;
                string VirtualDirectoryfullPath = string.Empty;
                var fileName = model.TS_RESD_STRENGTH_FILE_NAME;

                uploaded_path = ConfigurationManager.AppSettings["PMIS_TRIAL_STRECH_FOR_FDR_UPLOAD_PATH"].ToString() + "\\" + year + "\\" + month;
                VirtualDirectoryfullPath = Path.Combine(VirtualDirectoryPath, fileName.ToString()).ToString().Replace(@"\\", @"//").Replace(@"\", @"/");

                //var getTimestamp = new Date().getTime();
                //// VirtualDirectoryfullPath = "localhost:port/getpdf?your_param=xxx&timestamp=" + getTimestamp;
                //VirtualDirectoryfullPath = VirtualDirectoryfullPath+ getTimestamp

                string physicalFullPath = Path.Combine(uploaded_path, fileName);

                FileInfo file = new FileInfo(physicalFullPath);
                if (file.Exists)
                {
                    return Json(new { success = true, Message = VirtualDirectoryfullPath }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { success = false, Message = "File not found" }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "PMIS.viewFdrFileJMF()");
                return Json(new { success = false, Message = "Some problem occured. Please try after sometime." }, JsonRequestBehavior.AllowGet);
            }
            finally
            {
                dbContext.Dispose();
            }

        }

        // viewFdrFileMixDesign
        [RequiredAuthentication]
        public JsonResult viewFdrFileMixDesign(string id, string timestamp)
        {
            PMGSYEntities dbContext = new PMGSYEntities();

            try
            {
                //var splitarr = id.Split('$');
                int roadCode = Convert.ToInt32(id);
                long tStamp = Convert.ToInt64(timestamp);
                //string path = splitarr[1];
                var model = dbContext.PMIS_TRIAL_STRETCH_FDR_DETAIL.Where(s => s.IMS_PR_ROAD_CODE == roadCode).FirstOrDefault();
                string uploaded_path = string.Empty;

                if (model.TEST_RESULT_UCS_FILE_NAME == "")
                {
                    return Json(new { success = false, Message = "File not Exist" }, JsonRequestBehavior.AllowGet);
                }
                //D:\OMMASII\PMIS\UPLOADS\2023\6  --Example
                /* var month = model.JMF_FILE_PATH.ToString().Trim().Split('\\'); */
                var month = model.TEST_RESULT_UCS_FILE_PATH.ToString().Trim().Split('\\')[Convert.ToInt32(model.TEST_RESULT_UCS_FILE_PATH.ToString().Trim().Split('\\').Length) - 1];
                var year = model.TEST_RESULT_UCS_FILE_PATH.ToString().Trim().Split('\\')[Convert.ToInt32(model.TEST_RESULT_UCS_FILE_PATH.ToString().Trim().Split('\\').Length) - 2];

                string VirtualDirectoryPath = ConfigurationManager.AppSettings["PMIS_TRIAL_STRECH_FOR_FDR_UPLOAD_VIRTUAL_PATH"].ToString() + "\\" + year + "\\" + month;
                string VirtualDirectoryfullPath = string.Empty;
                var fileName = model.TEST_RESULT_UCS_FILE_NAME;

                uploaded_path = ConfigurationManager.AppSettings["PMIS_TRIAL_STRECH_FOR_FDR_UPLOAD_PATH"].ToString() + "\\" + year + "\\" + month;
                VirtualDirectoryfullPath = Path.Combine(VirtualDirectoryPath, fileName.ToString()).ToString().Replace(@"\\", @"//").Replace(@"\", @"/");

                string physicalFullPath = Path.Combine(uploaded_path, fileName);

                FileInfo file = new FileInfo(physicalFullPath);
                if (file.Exists)
                {
                    return Json(new { success = true, Message = VirtualDirectoryfullPath }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { success = false, Message = "File not found" }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "PMIS.viewFdrFileJMF()");
                return Json(new { success = false, Message = "Some problem occured. Please try after sometime." }, JsonRequestBehavior.AllowGet);
            }
            finally
            {
                dbContext.Dispose();
            }

        }

        [Audit]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SaveAddTrailStrechForFDR(FormCollection formCollection)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            PMISDAL objDAL = new PMISDAL();
            try
            {
                HttpPostedFileBase CoreTakenTrialfile1 = Request.Files[1];
                HttpPostedFileBase ResidualStrenTrialfile2 = Request.Files[2];
                HttpPostedFileBase TestResultMixfile3 = Request.Files[0];

                var multiselectlistArr = formCollection["ADDITIVE_ID_List_Arr"].ToString().Trim().Split('$'); //chek if empty or undefined 

                if (TestResultMixfile3.FileName == "")
                {
                    return Json(new { Success = false, Result = "-1", message = "Please Select file to Upload for Test Result Report of UCS pdf " });
                }
                if (CoreTakenTrialfile1.FileName == "")
                {
                    return Json(new { Success = false, Result = "-1", message = "Please Select file to Upload for Test result of core taken from trail section." });
                }
                if (ResidualStrenTrialfile2.FileName == "")
                {
                    return Json(new { Success = false, Result = "-1", message = "Please Select file to Upload for Residual Strength after 12 cycle wetting and drying pdf report." });
                }

                int fileSizeLimit = Convert.ToInt32(ConfigurationManager.AppSettings["PMIS_TRIAL_STRECH_FOR_FDR_FILE_SIZE"]) * 1024 * 1024;
                if (CoreTakenTrialfile1.ContentLength > fileSizeLimit || ResidualStrenTrialfile2.ContentLength > fileSizeLimit || TestResultMixfile3.ContentLength > fileSizeLimit)
                {
                    return Json(new { Success = false, Result = "-10", message = "File size should be less than 4mb" });
                }

                var IsAdditive = formCollection["IS_ADDITIVE_CONT_PERC_ML"].ToString().Trim();
                if (IsAdditive == "%")
                {
                    var PercAdditiveValue = formCollection["PERC_ADDITIVE_CONT"].ToString().Trim();
                    if (PercAdditiveValue == null || PercAdditiveValue == "")
                    {
                        return Json(new { Success = false, Result = "-1", message = "Please Enter Percent Additive Content." });
                    }
                }
                else
                {
                    var mlCumAdditiveValue = formCollection["ADDITIVE_CONT_ML_CUM"].ToString().Trim();
                    if (mlCumAdditiveValue == null || mlCumAdditiveValue == "")
                    {
                        return Json(new { Success = false, Result = "-1", message = "Please Enter Additive Content in ml/cum." });
                    }
                }

                var IsAvgUCS = formCollection["IS_AVG_UCS_7D_28D"].ToString().Trim();
                if (IsAvgUCS == "7D")
                {
                    var avgUCS7D = formCollection["AVG_UCS_7D"].ToString().Trim();
                    if (avgUCS7D == null || avgUCS7D == "")
                    {
                        return Json(new { Success = false, Result = "-1", message = "Please Enter Average UCS Strength(MPa) 7-Days." });
                    }
                }
                else
                {
                    var avgUCS28D = formCollection["AVG_UCS_28D"].ToString().Trim();
                    if (avgUCS28D == null || avgUCS28D == "")
                    {
                        return Json(new { Success = false, Result = "-1", message = "Please Enter Average UCS Strength(MPa) 28-Days." });
                    }
                }
                var IsUCSCube_Cylinder = formCollection["IS_UCS_TEST_CUBE_CYLINDER"].ToString().Trim();
                if (IsUCSCube_Cylinder == "Cube")
                {
                    var UCSCubeValue = formCollection["UCS_TEST_CUBE"].ToString().Trim();
                    if (UCSCubeValue == null || UCSCubeValue == "")
                    {
                        return Json(new { Success = false, Result = "-1", message = "Please Enter UCS Test Conducted on Cube Sample." });
                    }
                }
                else
                {
                    var UCSCylinderValue = formCollection["UCS_TEST_CYLINDER"].ToString().Trim();
                    if (UCSCylinderValue == null || UCSCylinderValue == "")
                    {
                        return Json(new { Success = false, Result = "-1", message = "Please Enter UCS Test Conducted on Cylindrical Sample." });
                    }
                }

                // Trial Stretch Validation Start Here
                var IsTSUCSCube_Cylinder = formCollection["IS_TS_UCS_STRENGTH_CUBE_CYLINDER"].ToString().Trim();
                if (IsTSUCSCube_Cylinder == "Cube")
                {
                    var tsUCSCubeValue = formCollection["UCS_STRENGTH_CUBE"].ToString().Trim();
                    if (tsUCSCubeValue == null || tsUCSCubeValue == "")
                    {
                        return Json(new { Success = false, Result = "-1", message = "Please Enter Details of Trial Stretch UCS Test Conducted on Cube." });
                    }
                }
                else
                {
                    var tsUCSCylinderValue = formCollection["UCS_STRENGTH_CYLINDER"].ToString().Trim();
                    if (tsUCSCylinderValue == null || tsUCSCylinderValue == "")
                    {
                        return Json(new { Success = false, Result = "-1", message = "Please Enter Details of Trial Stretch UCS Test Conducted on Cylindrical." });
                    }
                }

                var IsTSAvgUCS = formCollection["IS_TS_UCS_7D_28D"].ToString().Trim();
                if (IsTSAvgUCS == "7D")
                {
                    var TSavgUCS7D = formCollection["TS_UCS_7D"].ToString().Trim();
                    if (TSavgUCS7D == null || TSavgUCS7D == "")
                    {
                        return Json(new { Success = false, Result = "-1", message = "Please Enter Details of Trial Stretch UCS Value of 7-Days." });
                    }
                }
                else
                {
                    var TSavgUCS28D = formCollection["TS_UCS_28D"].ToString().Trim();
                    if (TSavgUCS28D == null || TSavgUCS28D == "")
                    {
                        return Json(new { Success = false, Result = "-1", message = "Please Enter Details of Trial Stretch UCS Value of 28-Days." });
                    }
                }
                var valueCrackRelief = formCollection["CRACK_RELIEF_LAYER"].ToString().Trim();
                if (valueCrackRelief == "Other")
                {
                    var valueOtherCrackRelief = formCollection["OTHER_CRACK_LAYER"].ToString().Trim();
                    if (valueOtherCrackRelief == null || valueOtherCrackRelief == "")
                    {
                        return Json(new { Success = false, Result = "-1", message = "Please Enter Other Crack Relief Layer." });
                    }
                }


                // Trial Stretch Validation End Here

                //if (file1.FileName != "" && formCollection["UCS_7DAY_VALUE"].ToString().Trim() == "")
                //{
                //    return Json(new { Success = false, Result = "-3", message = "Enter UCS value of 7 days(MPa) as per cores" });
                //}
                //if (file2.FileName != "" && formCollection["UCS_28DAY_VALUE"].ToString().Trim() == "")
                //{
                //    return Json(new { Success = false, Result = "-4", message = "Enter residual st after 12 cycle wetting and drying ( MPa) JMF PDF report" });
                //}

                int roadCode = Convert.ToInt32(formCollection["IMS_PR_ROAD_CODE"].ToString().Trim());
                var imsSanctionedObj = dbContext.IMS_SANCTIONED_PROJECTS.Where(x => x.IMS_PR_ROAD_CODE == roadCode).Select(y => new { y.IMS_PAV_LENGTH, y.IMS_SANCTIONED_DATE }).FirstOrDefault();
                //var strlen = Convert.ToDecimal(formCollection["STRETCH_LENGTH"].ToString().Trim());
                //var pavlen = imsSanctionedObj.IMS_PAV_LENGTH;
                if (Convert.ToDecimal(formCollection["STRETCH_LENGTH"].ToString().Trim()) > imsSanctionedObj.IMS_PAV_LENGTH)
                {
                    return Json(new { Success = false, Result = "-5", message = "Length of trial strech in km should not be greater than sanctioned length" });
                }

                var datefromTbl = imsSanctionedObj.IMS_SANCTIONED_DATE;
                var datefromView = formCollection["STRETCH_CONSTR_DATE"];
                DateTime dateFromView;
                bool isDateFromViewValid = DateTime.TryParse(datefromView, out dateFromView);
                if (isDateFromViewValid && dateFromView < datefromTbl.Value)
                {
                    return Json(new { Success = false, Result = "-6", message = "Date of contruction of trial strech can not be lesser than sanctioned date" });
                }

                if (formCollection["TS_UCS_7D"] == "" && formCollection["TS_UCS_28D"] == "")
                {
                    return Json(new { Success = false, Result = "-7", message = "Please enter at least one UCS value ( 7 days(MPa) or 28 days(MPa) ) " });
                }
                if (multiselectlistArr.Contains(""))
                {
                    return Json(new { Success = false, Result = "-8", message = "Please Select Additive used" });
                }
                if (formCollection["JMF_ID"] == "")
                {
                    return Json(new { Success = false, Result = "-9", message = "Please Select JMF Prepared" });
                }





                var message = string.Empty;
                bool flag = false;
                flag = objDAL.SaveAddTrailStrechForFDRDAL(formCollection, CoreTakenTrialfile1, ResidualStrenTrialfile2, TestResultMixfile3, ref message);

                if (flag)
                {
                    return Json(new { Success = true, Result = "-11", message = message }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { Success = false, Result = "-12", message = message }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "SaveAddTrailStrechForFDR()");
                return null;
            }

        }

        [HttpPost]
        public ActionResult DeleteTrialStrechFDR(string RoadCode/*, String hash, String key*/)
        {
            string status = string.Empty;
            PMISDAL objDAL = new PMISDAL();
            PMGSYEntities dbContext = new PMGSYEntities();
            Dictionary<string, string> decryptedParameters = null;

            try
            {
                //decryptedParameters = URLEncrypt.DecryptParameters1(new string[] { roadCode, hash, key });
                int IMS_PR_ROAD_CODE = 0;
                /*                if (decryptedParameters.Count() > 0)
                                {
                                    IMS_PR_ROAD_CODE = Convert.ToInt32(decryptedParameters["RoadCode"].ToString());
                                }
                                else
                                {
                                    return Json(new { success = false, errorMessage = "Invalid Road Code." });
                                }*/
                IMS_PR_ROAD_CODE = Convert.ToInt32(RoadCode);

                status = objDAL.DeleteTrialStrechFdrDAL(IMS_PR_ROAD_CODE);
                if (status == string.Empty)
                {
                    return Json(new { success = true });
                }
                else
                {
                    return Json(new { success = false, errorMessage = status });
                }
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "DeleteTrialStrechFDR()");
                return Json(new { success = false, errorMessage = status });
            }
            finally
            {
                if (dbContext != null)
                {
                    dbContext.Dispose();
                }
            }


            //return false;
        }

        [HttpPost]
        public ActionResult FinalizeTrailStrechFDR(string RoadCode)
        {
            string status = string.Empty;
            PMISDAL objDAL = new PMISDAL();
            PMGSYEntities dbContext = new PMGSYEntities();
            try
            {
                int IMS_PR_ROAD_CODE = 0;

                IMS_PR_ROAD_CODE = Convert.ToInt32(RoadCode);

                status = objDAL.FinalizeTrialStrechFdrDAL(IMS_PR_ROAD_CODE);
                if (status == string.Empty)
                {
                    return Json(new { success = true });
                }
                else
                {
                    return Json(new { success = false, errorMessage = status });
                }
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "FinalizeTrailStrechFDR()");
                return Json(new { success = false, errorMessage = status });
            }
            finally
            {
                if (dbContext != null)
                {
                    dbContext.Dispose();
                }
            }
        }

        [Audit]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateTrialStrechFDRDetails(FormCollection formCollection)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            PMISDAL objDAL = new PMISDAL();
            try
            {
                HttpPostedFileBase file1 = Request.Files[1];
                HttpPostedFileBase file2 = Request.Files[2];
                HttpPostedFileBase TestResultMixfile3 = Request.Files[0];

                var multiselectlistArr = formCollection["ADDITIVE_ID_List_Arr"].ToString().Trim().Split('$'); //chek if empty or undefined 

                if ((file1 != null && file1.ContentLength > 0 && Path.GetExtension(file1.FileName).ToLower() != ".pdf") || (file2 != null && file2.ContentLength > 0 && Path.GetExtension(file2.FileName).ToLower() != ".pdf") || (TestResultMixfile3 != null && TestResultMixfile3.ContentLength > 0 && Path.GetExtension(TestResultMixfile3.FileName).ToLower() != ".pdf"))
                {
                    return Json(new { Success = false, Result = "-33", message = "File is invalid" });
                }


                var IsAdditive = formCollection["IS_ADDITIVE_CONT_PERC_ML"].ToString().Trim();
                if (IsAdditive == "%")
                {
                    var PercAdditiveValue = formCollection["PERC_ADDITIVE_CONT"].ToString().Trim();
                    if (PercAdditiveValue == null || PercAdditiveValue == "")
                    {
                        return Json(new { Success = false, Result = "-1", message = "Please Enter Percent Additive Content." });
                    }
                }
                else
                {
                    var mlCumAdditiveValue = formCollection["ADDITIVE_CONT_ML_CUM"].ToString().Trim();
                    if (mlCumAdditiveValue == null || mlCumAdditiveValue == "")
                    {
                        return Json(new { Success = false, Result = "-1", message = "Please Enter Additive Content in ml/cum." });
                    }
                }

                var IsAvgUCS = formCollection["IS_AVG_UCS_7D_28D"].ToString().Trim();
                if (IsAvgUCS == "7D")
                {
                    var avgUCS7D = formCollection["AVG_UCS_7D"].ToString().Trim();
                    if (avgUCS7D == null || avgUCS7D == "")
                    {
                        return Json(new { Success = false, Result = "-1", message = "Please Enter Average UCS Strength(MPa) 7-Days." });
                    }
                }
                else
                {
                    var avgUCS28D = formCollection["AVG_UCS_28D"].ToString().Trim();
                    if (avgUCS28D == null || avgUCS28D == "")
                    {
                        return Json(new { Success = false, Result = "-1", message = "Please Enter Average UCS Strength(MPa) 28-Days." });
                    }
                }
                var IsUCSCube_Cylinder = formCollection["IS_UCS_TEST_CUBE_CYLINDER"].ToString().Trim();
                if (IsUCSCube_Cylinder == "Cube")
                {
                    var UCSCubeValue = formCollection["UCS_TEST_CUBE"].ToString().Trim();
                    if (UCSCubeValue == null || UCSCubeValue == "")
                    {
                        return Json(new { Success = false, Result = "-1", message = "Please Enter UCS Test Conducted on Cube Sample." });
                    }
                }
                else
                {
                    var UCSCylinderValue = formCollection["UCS_TEST_CYLINDER"].ToString().Trim();
                    if (UCSCylinderValue == null || UCSCylinderValue == "")
                    {
                        return Json(new { Success = false, Result = "-1", message = "Please Enter UCS Test Conducted on Cylindrical Sample." });
                    }
                }

                // Trial Stretch Validation Start Here

                var IsTSUCSCube_Cylinder = formCollection["IS_TS_UCS_STRENGTH_CUBE_CYLINDER"].ToString().Trim();
                if (IsTSUCSCube_Cylinder == "Cube")
                {
                    var tsUCSCubeValue = formCollection["UCS_STRENGTH_CUBE"].ToString().Trim();
                    if (tsUCSCubeValue == null || tsUCSCubeValue == "")
                    {
                        return Json(new { Success = false, Result = "-1", message = "Please Enter Details of Trial Stretch UCS Test Conducted on Cube." });
                    }
                }
                else
                {
                    var tsUCSCylinderValue = formCollection["UCS_STRENGTH_CYLINDER"].ToString().Trim();
                    if (tsUCSCylinderValue == null || tsUCSCylinderValue == "")
                    {
                        return Json(new { Success = false, Result = "-1", message = "Please Enter Details of Trial Stretch UCS Test Conducted on Cylindrical." });
                    }
                }

                var IsTSAvgUCS = formCollection["IS_TS_UCS_7D_28D"].ToString().Trim();
                if (IsTSAvgUCS == "7D")
                {
                    var TSavgUCS7D = formCollection["TS_UCS_7D"].ToString().Trim();
                    if (TSavgUCS7D == null || TSavgUCS7D == "")
                    {
                        return Json(new { Success = false, Result = "-1", message = "Please Enter Details of Trial Stretch UCS Value of 7-Days." });
                    }
                }
                else
                {
                    var TSavgUCS28D = formCollection["TS_UCS_28D"].ToString().Trim();
                    if (TSavgUCS28D == null || TSavgUCS28D == "")
                    {
                        return Json(new { Success = false, Result = "-1", message = "Please Enter Details of Trial Stretch UCS Value of 28-Days." });
                    }
                }
                var valueCrackRelief = formCollection["CRACK_RELIEF_LAYER"].ToString().Trim();
                if (valueCrackRelief == "Other")
                {
                    var valueOtherCrackRelief = formCollection["OTHER_CRACK_LAYER"].ToString().Trim();
                    if (valueOtherCrackRelief == null || valueOtherCrackRelief == "")
                    {
                        return Json(new { Success = false, Result = "-1", message = "Please Enter Other Crack Relief Layer." });
                    }
                }

                // Trial Stretch Validation End Here


                int roadCode = Convert.ToInt32(formCollection["IMS_PR_ROAD_CODE"].ToString().Trim());
                var imsSanctionedObj = dbContext.IMS_SANCTIONED_PROJECTS.Where(x => x.IMS_PR_ROAD_CODE == roadCode).Select(y => new { y.IMS_PAV_LENGTH, y.IMS_SANCTIONED_DATE }).FirstOrDefault();
                if (Convert.ToDecimal(formCollection["STRETCH_LENGTH"].ToString().Trim()) > imsSanctionedObj.IMS_PAV_LENGTH)
                {
                    return Json(new { Success = false, Result = "-5", message = "Length of trial strech in km should not be greated that sanctioned length" });
                }

                var datefromTbl = imsSanctionedObj.IMS_SANCTIONED_DATE;
                var datefromView = formCollection["STRETCH_CONSTR_DATE"];
                DateTime dateFromView;
                bool isDateFromViewValid = DateTime.TryParse(datefromView, out dateFromView);
                if (isDateFromViewValid && dateFromView < datefromTbl.Value)
                {
                    return Json(new { Success = false, Result = "-6", message = "Date of contruction of trial strech can not be lesser than sanctioned date" });
                }

                if (multiselectlistArr.Contains(""))
                {
                    return Json(new { Success = false, Result = "-8", message = "Please Select Additive used" });
                }
                if (formCollection["JMF_ID"] == "")
                {
                    return Json(new { Success = false, Result = "-9", message = "Please Select JMF Prepared" });
                }

                int fileSizeLimit = Convert.ToInt32(ConfigurationManager.AppSettings["PMIS_TRIAL_STRECH_FOR_FDR_FILE_SIZE"]) * 1024 * 1024;
                if (file1.ContentLength > fileSizeLimit || file2.ContentLength > fileSizeLimit || TestResultMixfile3.ContentLength > fileSizeLimit)
                {
                    return Json(new { Success = false, Result = "-10", message = "File size should be less than 4mb" });
                }

                var message = string.Empty;
                bool flag = false;
                flag = objDAL.UpdateTrialStrechFDRDetailsDAL(formCollection, file1, file2, TestResultMixfile3, ref message);

                if (flag)
                {
                    return Json(new { Success = true, Result = "-11", message = message }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { Success = false, Result = "-12", message = message }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "UpdateTrialStrechFDRDetails()");
                return null;
            }
        }

        #endregion


        #region PMIS Reports


        public ActionResult PMISRoadLayoutCharts()
        {
            ListPMISRoadDetailsViewModel model = new ListPMISRoadDetailsViewModel();
            CommonFunctions comm = new CommonFunctions();
            try
            {
                model.StateList = new List<SelectListItem>();
                model.StateList.Insert(0, new SelectListItem() { Text = PMGSYSession.Current.StateName, Value = PMGSYSession.Current.StateCode.ToString() });
                model.DistrictList = new List<SelectListItem>();
                if (PMGSYSession.Current.DistrictCode == 0)
                {
                    model.DistrictList = comm.PopulateDistrictForSRRDA(PMGSYSession.Current.StateCode);
                    // model.DistrictList.Find(x => x.Value == "0").Text = "Select District";
                }
                else if (PMGSYSession.Current.DistrictCode > 0)
                {

                    model.DistrictList.Insert(0, new SelectListItem() { Text = PMGSYSession.Current.DistrictName, Value = PMGSYSession.Current.DistrictCode.ToString() });
                }

                model.BlockList = new List<SelectListItem>();
                if (PMGSYSession.Current.DistrictCode == 0)
                {
                    model.BlockList.Insert(0, (new SelectListItem { Text = "All Blocks", Value = "0", Selected = true }));
                }
                else
                {
                    model.BlockList = comm.PopulateBlocks((PMGSYSession.Current.DistrictCode), true);

                    model.BlockList.Find(x => x.Value == "-1").Value = "0";
                    model.BlockList.Find(x => x.Value == model.BlockCode.ToString()).Selected = true;
                }
                model.Sanction_Year_List = comm.PopulateFinancialYear(true, true).ToList();
                model.Sanction_Year_List.RemoveAt(0);

                model.BatchList = comm.PopulateBatch();
                model.BatchList.RemoveAt(0);
                model.BatchList.Insert(0, new SelectListItem { Value = "0", Text = "All Batches" });

                return View(model);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "PMISController.PMISRoadLayoutCharts()");
                return null;
            }
        }



        [HttpPost]
        public ActionResult PMISRoadListCharts(int? page, int? rows, string sidx, string sord)
        {
            long totalRecords; int state = 0; int district = 0; int block = 0; int sanction_year = 0; int batch = 0;
            try
            {
                if (!string.IsNullOrEmpty(Request.Params["state"]))
                {
                    state = Convert.ToInt32(Request.Params["state"]);
                }

                if (!string.IsNullOrEmpty(Request.Params["district"]))
                {
                    district = Convert.ToInt32(Request.Params["district"]);

                }
                if (!string.IsNullOrEmpty(Request.Params["block"]))
                {
                    block = Convert.ToInt32(Request.Params["block"]);

                }
                if (!string.IsNullOrEmpty(Request.Params["sanction_year"]))
                {
                    sanction_year = Convert.ToInt32(Request.Params["sanction_year"]);

                }
                if (!string.IsNullOrEmpty(Request.Params["batch"]))
                {
                    batch = Convert.ToInt32(Request.Params["batch"]);

                }

                PMISDAL dalobj = new PMISDAL();
                var jsonData = new
                {
                    rows = dalobj.PMISRoadListDALCharts(state, district, block, sanction_year, batch, Convert.ToInt32(Request.Params["page"]) - 1, Convert.ToInt32(Request.Params["rows"]), Request.Params["sidx"], Request.Params["sord"], out totalRecords),
                    total = totalRecords <= Convert.ToInt32(Request.Params["rows"]) ? 1 : totalRecords / Convert.ToInt32(Request.Params["rows"]) + 1,
                    page = Convert.ToInt32(Request.Params["page"]),
                    records = totalRecords
                };
                return Json(jsonData, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "PMISController.PMISRoadListCharts()");
                return null;
            }
        }





        //[HttpPost]
        //public JsonResult GetDataForGanttChart(int RoadCode, int BaseLine)
        //{

        //    GanttChartResponseModel response = new GanttChartResponseModel();
        //    PMISDAL dalobj = new PMISDAL();

        //    response = dalobj.GetDataForGanttChartDAL(RoadCode, BaseLine);

        //    return Json(response, JsonRequestBehavior.AllowGet);
        //}



        [HttpPost]
        public JsonResult GetDataForGanttChart(String parameter, String hash, String key)
        {
            string status = string.Empty;
            PMISDAL objDAL = new PMISDAL();
            PMGSYEntities dbContext = new PMGSYEntities();
            Dictionary<string, string> decryptedParameters = null;
            try
            {
                decryptedParameters = URLEncrypt.DecryptParameters1(new string[] { parameter, hash, key });
                int RoadCode = 0;
                int PlanID = 0;
                if (decryptedParameters.Count() > 0)
                {

                    RoadCode = Convert.ToInt32(decryptedParameters["IMSPRRoadCode"].ToString());
                    PlanID = Convert.ToInt32(decryptedParameters["PlanID"].ToString());

                }
                else
                {
                    return Json(new { success = false, errorMessage = "Invalid Road Code." });
                }

                GanttChartResponseModel response = new GanttChartResponseModel();
                PMISDAL dalobj = new PMISDAL();

                response = dalobj.GetDataForGanttChartDAL(RoadCode, 1);
                //  response = dalobj.GetDataForGanttChartDAL(RoadCode, PlanID);

                return Json(response, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "PMISController.GetDataForGanttChart()");
                return Json(new { success = false, errorMessage = status });
            }
            finally
            {
                if (dbContext != null)
                {
                    dbContext.Dispose();
                }
            }
        }

        //[HttpGet]
        //[Audit]
        //public ActionResult ViewPMISRoadProjectPlanLayout(String parameter, String hash, String key)
        //{

        //}



        //public double MilliTimeStamp(DateTime TheDate)
        //{
        //    try
        //    {
        //        //response = GetDataForGanttChartDAL(RoadCode, BaseLine);

        //        DateTime d1 = new DateTime(1970, 1, 1);
        //        DateTime d2 = TheDate.ToUniversalTime();
        //        TimeSpan ts = new TimeSpan(d2.Ticks - d1.Ticks);
        //        return ts.TotalMilliseconds;
        //    }
        //    catch (Exception)
        //    {

        //        throw;
        //    }
        //}




        #endregion

        #region

        //#region PMIS List Bridge
        //public ActionResult PMISBridgeLayout()
        //{
        //    ListPMISBridgeDetailsViewModel model = new ListPMISBridgeDetailsViewModel();
        //    CommonFunctions comm = new CommonFunctions();
        //    try
        //    {
        //        model.StateList = new List<SelectListItem>();
        //        model.StateList.Insert(0, new SelectListItem() { Text = PMGSYSession.Current.StateName, Value = PMGSYSession.Current.StateCode.ToString() });
        //        model.DistrictList = new List<SelectListItem>();
        //        if (PMGSYSession.Current.DistrictCode == 0)
        //        {
        //            model.DistrictList = comm.PopulateDistrictForSRRDA(PMGSYSession.Current.StateCode);
        //            // model.DistrictList.Find(x => x.Value == "0").Text = "Select District";
        //        }
        //        else if (PMGSYSession.Current.DistrictCode > 0)
        //        {

        //            model.DistrictList.Insert(0, new SelectListItem() { Text = PMGSYSession.Current.DistrictName, Value = PMGSYSession.Current.DistrictCode.ToString() });
        //        }

        //        model.BlockList = new List<SelectListItem>();
        //        if (PMGSYSession.Current.DistrictCode == 0)
        //        {
        //            model.BlockList.Insert(0, (new SelectListItem { Text = "All Blocks", Value = "0", Selected = true }));
        //        }
        //        else
        //        {
        //            model.BlockList = comm.PopulateBlocks((PMGSYSession.Current.DistrictCode), true);

        //            model.BlockList.Find(x => x.Value == "-1").Value = "0";
        //            model.BlockList.Find(x => x.Value == model.BlockCode.ToString()).Selected = true;
        //        }
        //        model.Sanction_Year_List = comm.PopulateFinancialYear(true, true).ToList();
        //        model.Sanction_Year_List.RemoveAt(0);

        //        model.BatchList = comm.PopulateBatch();
        //        model.BatchList.RemoveAt(0);
        //        model.BatchList.Insert(0, new SelectListItem { Value = "0", Text = "All Batches" });

        //        return View(model);
        //    }
        //    catch (Exception ex)
        //    {
        //        ErrorLog.LogError(ex, "PMISController.PMISBridgeLayout()");
        //        return null;
        //    }
        //}

        //[HttpPost]
        //public ActionResult PMISBridgeList(int? page, int? rows, string sidx, string sord)
        //{
        //    long totalRecords; int state = 0; int district = 0; int block = 0; int sanction_year = 0; int batch = 0;
        //    try
        //    {
        //        if (!string.IsNullOrEmpty(Request.Params["state"]))
        //        {
        //            state = Convert.ToInt32(Request.Params["state"]);
        //        }

        //        if (!string.IsNullOrEmpty(Request.Params["district"]))
        //        {
        //            district = Convert.ToInt32(Request.Params["district"]);

        //        }
        //        if (!string.IsNullOrEmpty(Request.Params["block"]))
        //        {
        //            block = Convert.ToInt32(Request.Params["block"]);

        //        }
        //        if (!string.IsNullOrEmpty(Request.Params["sanction_year"]))
        //        {
        //            sanction_year = Convert.ToInt32(Request.Params["sanction_year"]);

        //        }
        //        if (!string.IsNullOrEmpty(Request.Params["batch"]))
        //        {
        //            batch = Convert.ToInt32(Request.Params["batch"]);

        //        }

        //        PMISDAL dalobj = new PMISDAL();
        //        var jsonData = new
        //        {
        //            rows = dalobj.PMISBridgeListDAL(state, district, block, sanction_year, batch, Convert.ToInt32(Request.Params["page"]) - 1, Convert.ToInt32(Request.Params["rows"]), Request.Params["sidx"], Request.Params["sord"], out totalRecords),
        //            total = totalRecords <= Convert.ToInt32(Request.Params["rows"]) ? 1 : totalRecords / Convert.ToInt32(Request.Params["rows"]) + 1,
        //            page = Convert.ToInt32(Request.Params["page"]),
        //            records = totalRecords
        //        };
        //        return Json(jsonData, JsonRequestBehavior.AllowGet);

        //    }
        //    catch (Exception ex)
        //    {
        //        ErrorLog.LogError(ex, "PMISController.PMISBridgeList()");
        //        return null;
        //    }
        //}
        //#endregion

        //#region Add/Update/View Project PLAN for Bridge
        //[HttpGet]
        //[Audit]
        //public ViewResult AddPMISBridgeProjectPlan(String parameter, String hash, String key)
        //{
        //    PMGSYEntities dbContext = new PMGSYEntities();
        //    PMISDAL objDAL = new PMISDAL();
        //    AddPlanPMISViewModelBridge pmisViewModel = new AddPlanPMISViewModelBridge();
        //    Dictionary<string, string> decryptedParameters = null;
        //    ViewBag.operation = "A";
        //    //int roadCode = Convert.ToInt32(RoadCode);    

        //    try
        //    {
        //        decryptedParameters = URLEncrypt.DecryptParameters1(new string[] { parameter, hash, key });
        //        int IMS_PR_ROAD_CODE = 0; string StateShare = String.Empty; string MordShare = String.Empty; string TotalSanctionedCost = String.Empty;
        //        decimal stateShare = 0; decimal mordShare = 0; decimal totalSancCost = 0;
        //        if (decryptedParameters.Count() > 0)
        //        {
        //            IMS_PR_ROAD_CODE = Convert.ToInt32(decryptedParameters["RoadCode"].ToString());
        //            stateShare = Math.Round(Convert.ToDecimal(decryptedParameters["StateShare"]), 2);
        //            StateShare = stateShare.ToString();
        //            mordShare = Math.Round(Convert.ToDecimal(decryptedParameters["MordShare"]), 2);
        //            MordShare = mordShare.ToString();
        //            totalSancCost = Math.Round(Convert.ToDecimal(decryptedParameters["TotalSanctionedDate"]), 2);
        //            TotalSanctionedCost = totalSancCost.ToString();
        //        }
        //        else
        //        {
        //            return null;
        //        }
        //        if (IMS_PR_ROAD_CODE != 0)
        //        {


        //            pmisViewModel.IMS_PR_ROAD_CODE = IMS_PR_ROAD_CODE;

        //            //pmisViewModel.IMS_ROAD_NAME = dbContext.IMS_SANCTIONED_PROJECTS.Where(x => x.IMS_PR_ROAD_CODE == pmisViewModel.IMS_PR_ROAD_CODE).Select(x => x.IMS_ROAD_NAME).FirstOrDefault();
        //            pmisViewModel.IMS_BRIDGE_NAME = dbContext.IMS_SANCTIONED_PROJECTS.Where(x => x.IMS_PR_ROAD_CODE == pmisViewModel.IMS_PR_ROAD_CODE).Select(x => x.IMS_BRIDGE_NAME).FirstOrDefault();
        //            var year = dbContext.IMS_SANCTIONED_PROJECTS.Where(x => x.IMS_PR_ROAD_CODE == IMS_PR_ROAD_CODE).Select(x => x.IMS_YEAR).FirstOrDefault();
        //            pmisViewModel.IMS_YEAR = year.ToString() + "-" + (year + 1).ToString().Substring(2, 2);
        //            //pmisViewModel.IMS_PAV_LENGTH = Math.Round(dbContext.IMS_SANCTIONED_PROJECTS.Where(x => x.IMS_PR_ROAD_CODE == pmisViewModel.IMS_PR_ROAD_CODE).Select(x => x.IMS_PAV_LENGTH).FirstOrDefault(), 2);
        //            pmisViewModel.IMS_BRIDGE_LENGTH = Math.Round(Convert.ToDecimal(dbContext.IMS_SANCTIONED_PROJECTS.Where(x => x.IMS_PR_ROAD_CODE == pmisViewModel.IMS_PR_ROAD_CODE).Select(x => x.IMS_BRIDGE_LENGTH).FirstOrDefault()), 2);
        //            pmisViewModel.IMS_PACKAGE_ID = dbContext.IMS_SANCTIONED_PROJECTS.Where(x => x.IMS_PR_ROAD_CODE == pmisViewModel.IMS_PR_ROAD_CODE).Select(x => x.IMS_PACKAGE_ID).FirstOrDefault();
        //            pmisViewModel.AGREEMENT_VALUE = dbContext.TEND_AGREEMENT_DETAIL.Where(x => x.IMS_PR_ROAD_CODE == pmisViewModel.IMS_PR_ROAD_CODE).Select(x => x.TEND_AGREEMENT_AMOUNT).FirstOrDefault();
        //            pmisViewModel.StateShare = StateShare;
        //            pmisViewModel.MordShare = MordShare;
        //            pmisViewModel.TotalSanctionedCost = TotalSanctionedCost;

        //            pmisViewModel.StateName = PMGSYSession.Current.StateName;
        //            pmisViewModel.DistrictName = PMGSYSession.Current.DistrictName;
        //            pmisViewModel.Activity_Desc_List = (from x in dbContext.PMIS_ACTIVITY_MASTER where x.ROAD_TYPE == "L" select x.ACTIVITY_DESC).ToArray();
        //            pmisViewModel.Activity_Unit_List = (from x in dbContext.PMIS_ACTIVITY_MASTER where x.ROAD_TYPE == "L" select x.ACTIVITY_UNIT).ToArray();
        //            pmisViewModel.QUANTITY_APPL = (from x in dbContext.PMIS_ACTIVITY_MASTER where x.ROAD_TYPE == "L" select x.QUANTITY_APPL).ToArray();
        //            pmisViewModel.AGRCOST_APPL = (from x in dbContext.PMIS_ACTIVITY_MASTER where x.ROAD_TYPE == "L" select x.AGRCOST_APPL).ToArray();
        //            pmisViewModel.PLANNED_START_DATE_APPL = (from x in dbContext.PMIS_ACTIVITY_MASTER where x.ROAD_TYPE == "L" select x.PLANNED_START_DATE_APPL).ToArray();
        //            pmisViewModel.PLANNED_DURATION_APPL = (from x in dbContext.PMIS_ACTIVITY_MASTER where x.ROAD_TYPE == "L" select x.PLANNED_DURATION_APPL).ToArray();
        //            pmisViewModel.PLANNED_COMPLETION_DATE_APPL = (from x in dbContext.PMIS_ACTIVITY_MASTER where x.ROAD_TYPE == "L" select x.PLANNED_COMPLETION_DATE_APPL).ToArray();

        //            ViewData["ActivityList"] = pmisViewModel.Activity_Desc_List;
        //        }
        //        return View("~/Areas/PMIS/Views/PMIS/AddPMISBridgeProjectPlan.cshtml", pmisViewModel);

        //    }
        //    catch (Exception ex)
        //    {
        //        ErrorLog.LogError(ex, "PMIS.AddPMISBridgeProjectPlan()");
        //        return null;
        //    }
        //}

        //[HttpPost]
        //[Audit]
        //public ActionResult SaveBridgeProjectPlan(IEnumerable<AddPlanPMISViewModelBridge> formData)
        //{
        //    PMISDAL objDAL = new PMISDAL();
        //    PMGSYEntities dbContext = new PMGSYEntities();
        //    try
        //    {
        //        ViewBag.operation = "A";
        //        if (ModelState.IsValid)
        //        {
        //            decimal TotalAgreementCost = 0;
        //            decimal AgreementValue = 0;
        //            int roadCode = 0;
        //            foreach (var item in formData)
        //            {
        //                if (item.AGREEMENT_COST != null)
        //                {
        //                    TotalAgreementCost += Convert.ToDecimal(item.AGREEMENT_COST);
        //                }

        //                if (item.IMS_PR_ROAD_CODE > 0)
        //                {
        //                    roadCode = item.IMS_PR_ROAD_CODE;
        //                }
        //                //decimal SanLength = dbContext.IMS_SANCTIONED_PROJECTS.Where(x => x.IMS_PR_ROAD_CODE == item.IMS_PR_ROAD_CODE).Select(x => x.IMS_PAV_LENGTH).FirstOrDefault();
        //                decimal SanLength = Convert.ToDecimal(dbContext.IMS_SANCTIONED_PROJECTS.Where(x => x.IMS_PR_ROAD_CODE == item.IMS_PR_ROAD_CODE).Select(x => x.IMS_BRIDGE_LENGTH).FirstOrDefault());

        //                if ((item.ACTIVITY_DESC != null || item.ACTIVITY_DESC != ""))
        //                {

        //                    if (item.ACTIVITY_DESC != "Miscellaneous")
        //                    {
        //                        if (item.ACTIVITY_DESC == "Resource Mobilization" || item.ACTIVITY_DESC == "Field Lab")
        //                        {
        //                            if (item.PLANNED_START_DATE == null || item.PLANNED_COMPLETION_DATE == null)
        //                            {
        //                                return Json(new { Success = false, ErrorMessage = "Planned Start Date and Planned End date must be Entered for " + item.ACTIVITY_DESC });
        //                            }

        //                        }
        //                        else if ((item.QUANTITY != 0 && item.QUANTITY != null) && (item.AGREEMENT_COST != 0 && item.AGREEMENT_COST != null))
        //                        {

        //                        }
        //                        else if ((item.QUANTITY != 0 && item.QUANTITY != null) && item.AGREEMENT_COST == 0)
        //                        {
        //                            return Json(new { Success = false, ErrorMessage = "Agreement Cost cannot be Zero for which Quantity entered is greater than zero" });
        //                        }
        //                        else if (item.QUANTITY == 0 && item.AGREEMENT_COST == 0)
        //                        {

        //                        }
        //                        else if ((item.QUANTITY == null) || (item.AGREEMENT_COST == null || item.PLANNED_START_DATE == null || item.PLANNED_COMPLETION_DATE == null))
        //                        {
        //                            return Json(new { Success = false, ErrorMessage = "All the * fields are mandatory" });
        //                        }

        //                    }
        //                    else if (item.AGREEMENT_COST == null)
        //                    {
        //                        return Json(new { Success = false, ErrorMessage = "All the * fields are mandatory" });
        //                    }
        //                    else
        //                    {

        //                    }
        //                }
        //                //if (item.QUANTITY != null && item.QUANTITY > SanLength && item.ACTIVITY_DESC != "Well Sinking" && item.ACTIVITY_DESC != "Pile Foundation" && item.ACTIVITY_DESC != "Bearings" && item.ACTIVITY_DESC != "Railings" && item.ACTIVITY_DESC != "Road Logo and other Furnitures")
        //                //{
        //                //    return Json(new { Success = false, ErrorMessage = "For each individual activity, Quantity entered should not be more than sanctioned length." });
        //                //}

        //                if (item.QUANTITY == null && item.AGREEMENT_COST > 0 && item.ACTIVITY_DESC != "Miscellaneous")
        //                {
        //                    return Json(new { Success = false, ErrorMessage = "Agreement cost cannot be entered for activities for which quantity has not been entered." });
        //                }
        //                if (item.PLANNED_START_DATE != null && item.PLANNED_COMPLETION_DATE != null && item.PLANNED_START_DATE > item.PLANNED_COMPLETION_DATE)
        //                {
        //                    return Json(new { Success = false, ErrorMessage = "Planned Completion date cannot be before the planned start date of an activity." });
        //                }
        //                if ((item.PLANNED_START_DATE == null || item.PLANNED_COMPLETION_DATE == null) && (item.QUANTITY > 0))
        //                {
        //                    return Json(new { Success = false, ErrorMessage = "Planned Start and End Date should be entered for activities for which quantity has been entered." });
        //                }
        //                //if ((item.PLANNED_START_DATE == null || item.PLANNED_COMPLETION_DATE == null) && item.ACTIVITY_DESC == "Field Lab")
        //                //{
        //                //    return Json(new { Success = false, ErrorMessage = "Field Lab Start and End Date should be entered." });
        //                //}
        //            }
        //            AgreementValue = dbContext.TEND_AGREEMENT_DETAIL.Where(x => x.IMS_PR_ROAD_CODE == roadCode).Select(x => x.TEND_AGREEMENT_AMOUNT).FirstOrDefault();
        //            if (TotalAgreementCost != AgreementValue)
        //            {
        //                return Json(new { Success = false, ErrorMessage = "The Sum of Agreement cost of all activities should match the Agreement Value for this road" });
        //            }

        //            string Status = objDAL.SaveBridgeProjectPlanDAL(formData);
        //            if (Status == string.Empty)
        //                return Json(new { Success = true });
        //            else
        //                return Json(new { Success = false, ErrorMessage = Status });
        //        }
        //        else
        //        {
        //            return Json(new { Success = false, ErrorMessage = new CommonFunctions().FormatErrorMessage(ModelState) });
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        ErrorLog.LogError(ex, "SaveBridgeProjectPlan(AddPlanPMISViewModelBridge formData))");
        //        throw ex;
        //    }
        //}

        //[HttpGet]
        //[Audit]
        //public ActionResult UpdatePMISBridgeProjectPlanLayout(String parameter, String hash, String key)
        //{
        //    PMGSYEntities dbContext = new PMGSYEntities();
        //    PMISDAL objlDAL = new PMISDAL();
        //    CommonFunctions objCommonFuntion = new CommonFunctions();
        //    Dictionary<string, string> decryptedParameters = null;
        //    string status = string.Empty;

        //    try
        //    {
        //        decryptedParameters = URLEncrypt.DecryptParameters1(new string[] { parameter, hash, key });
        //        int IMS_PR_ROAD_CODE = 0; string StateShare = String.Empty; string MordShare = String.Empty; string TotalSanctionedCost = String.Empty;
        //        decimal stateShare = 0; decimal mordShare = 0; decimal totalSancCost = 0;
        //        if (decryptedParameters.Count() > 0)
        //        {
        //            IMS_PR_ROAD_CODE = Convert.ToInt32(decryptedParameters["RoadCode"].ToString());
        //            stateShare = Math.Round(Convert.ToDecimal(decryptedParameters["StateShare"]), 2);
        //            StateShare = stateShare.ToString();
        //            mordShare = Math.Round(Convert.ToDecimal(decryptedParameters["MordShare"]), 2);
        //            MordShare = mordShare.ToString();
        //            totalSancCost = Math.Round(Convert.ToDecimal(decryptedParameters["TotalSanctionedDate"]), 2);
        //            TotalSanctionedCost = totalSancCost.ToString();
        //        }
        //        else
        //        {
        //            return Json(new { success = false, errorMessage = "Invalid Road Code." });
        //        }
        //        ViewBag.operation = "U";

        //        if (!dbContext.PMIS_PLAN_MASTER.Any(m => m.IMS_PR_ROAD_CODE == IMS_PR_ROAD_CODE))
        //        {
        //            return Json(new { Success = false, ErrorMessage = "Plan Not Found" }, JsonRequestBehavior.AllowGet);
        //        }
        //        List<AddPlanPMISViewModelBridge> objlist = new List<AddPlanPMISViewModelBridge>();

        //        var latestRecord = dbContext.PMIS_PLAN_DETAILS.Where(x => x.IMS_PR_ROAD_CODE == IMS_PR_ROAD_CODE & x.IS_LATEST == "Y").ToList();
        //        if (latestRecord != null)
        //        {
        //            ViewBag.IMS_PR_ROAD_CODE = IMS_PR_ROAD_CODE;
        //            //ViewBag.IMS_ROAD_NAME = dbContext.IMS_SANCTIONED_PROJECTS.Where(x => x.IMS_PR_ROAD_CODE == IMS_PR_ROAD_CODE).Select(x => x.IMS_ROAD_NAME).FirstOrDefault();
        //            ViewBag.IMS_BRIDGE_NAME = dbContext.IMS_SANCTIONED_PROJECTS.Where(x => x.IMS_PR_ROAD_CODE == IMS_PR_ROAD_CODE).Select(x => x.IMS_BRIDGE_NAME).FirstOrDefault();
        //            var year = dbContext.IMS_SANCTIONED_PROJECTS.Where(x => x.IMS_PR_ROAD_CODE == IMS_PR_ROAD_CODE).Select(x => x.IMS_YEAR).FirstOrDefault();
        //            ViewBag.IMS_YEAR = year.ToString() + "-" + (year + 1).ToString().Substring(2, 2);
        //            //ViewBag.IMS_PAV_LENGTH = Math.Round(dbContext.IMS_SANCTIONED_PROJECTS.Where(x => x.IMS_PR_ROAD_CODE == IMS_PR_ROAD_CODE).Select(x => x.IMS_PAV_LENGTH).FirstOrDefault(), 2);
        //            ViewBag.IMS_BRIDGE_LENGTH = Math.Round(Convert.ToDecimal(dbContext.IMS_SANCTIONED_PROJECTS.Where(x => x.IMS_PR_ROAD_CODE == IMS_PR_ROAD_CODE).Select(x => x.IMS_BRIDGE_LENGTH).FirstOrDefault()), 2);
        //            ViewBag.IMS_PACKAGE_ID = dbContext.IMS_SANCTIONED_PROJECTS.Where(x => x.IMS_PR_ROAD_CODE == IMS_PR_ROAD_CODE).Select(x => x.IMS_PACKAGE_ID).FirstOrDefault();
        //            ViewBag.AGREEMENT_VALUE = dbContext.TEND_AGREEMENT_DETAIL.Where(x => x.IMS_PR_ROAD_CODE == IMS_PR_ROAD_CODE).Select(x => x.TEND_AGREEMENT_AMOUNT).FirstOrDefault();
        //            ViewBag.StateName = PMGSYSession.Current.StateName;
        //            ViewBag.DistrictName = PMGSYSession.Current.DistrictName;
        //            ViewBag.StateShare = StateShare;
        //            ViewBag.MordShare = MordShare;
        //            ViewBag.TotalSanctionedCost = TotalSanctionedCost;
        //            ViewBag.BASELINE_NO = (from x in dbContext.PMIS_PLAN_MASTER where x.IMS_PR_ROAD_CODE == IMS_PR_ROAD_CODE && x.IS_LATEST == "Y" select x.BASELINE_NO).FirstOrDefault();
        //            DateTime StartDate = ViewBag.TotalPlannedStartDate = dbContext.PMIS_PLAN_DETAILS.Where(x => x.IMS_PR_ROAD_CODE == IMS_PR_ROAD_CODE & x.IS_LATEST == "Y").Select(x => x.PLANNED_START_DATE).Min();
        //            DateTime EndDate = ViewBag.TotalPlannedCompletion = dbContext.PMIS_PLAN_DETAILS.Where(x => x.IMS_PR_ROAD_CODE == IMS_PR_ROAD_CODE & x.IS_LATEST == "Y").Select(x => x.PLANNED_COMPLETION_DATE).Max();
        //            ViewBag.TotalPlannedDuration = (EndDate - StartDate).Days;
        //            Decimal? TotalCost = 0;

        //            foreach (var item in latestRecord)
        //            {
        //                AddPlanPMISViewModelBridge objPmis = new AddPlanPMISViewModelBridge();

        //                //objPmis.ACTIVITY_DESC = dbContext.PMIS_ACTIVITY_MASTER.Where(y => y.ACTIVITY_ID == item.ACTIVITY_ID).Select(c => c.ACTIVITY_DESC).FirstOrDefault();
        //                objPmis.ACTIVITY_DESC = dbContext.PMIS_ACTIVITY_MASTER.Where(y => y.ACTIVITY_ID == item.ACTIVITY_ID && y.ROAD_TYPE == "L").Select(c => c.ACTIVITY_DESC).FirstOrDefault();
        //                //objPmis.ACTIVITY_UNIT = dbContext.PMIS_ACTIVITY_MASTER.Where(y => y.ACTIVITY_ID == item.ACTIVITY_ID).Select(c => c.ACTIVITY_UNIT).FirstOrDefault();
        //                objPmis.ACTIVITY_UNIT = dbContext.PMIS_ACTIVITY_MASTER.Where(y => y.ACTIVITY_ID == item.ACTIVITY_ID && y.ROAD_TYPE == "L").Select(c => c.ACTIVITY_UNIT).FirstOrDefault();
        //                objPmis.QUANTITY = item.QUANTITY;
        //                objPmis.AGREEMENT_COST = Math.Round(Convert.ToDecimal(item.AGREEMENT_COST), 2);
        //                objPmis.PLANNED_START_DATE = item.PLANNED_START_DATE;
        //                objPmis.PLANNED_DURATION = item.PLANNED_DURATION;
        //                objPmis.PLANNED_COMPLETION_DATE = item.PLANNED_COMPLETION_DATE;

        //                //objPmis.QUANTITY_APPL_U = dbContext.PMIS_ACTIVITY_MASTER.Where(y => y.ACTIVITY_ID == item.ACTIVITY_ID).Select(c => c.QUANTITY_APPL).FirstOrDefault();
        //                objPmis.QUANTITY_APPL_U = dbContext.PMIS_ACTIVITY_MASTER.Where(y => y.ACTIVITY_ID == item.ACTIVITY_ID && y.ROAD_TYPE == "L").Select(c => c.QUANTITY_APPL).FirstOrDefault();
        //                //objPmis.AGRCOST_APPL_U = dbContext.PMIS_ACTIVITY_MASTER.Where(y => y.ACTIVITY_ID == item.ACTIVITY_ID).Select(c => c.AGRCOST_APPL).FirstOrDefault();
        //                objPmis.AGRCOST_APPL_U = dbContext.PMIS_ACTIVITY_MASTER.Where(y => y.ACTIVITY_ID == item.ACTIVITY_ID && y.ROAD_TYPE == "L").Select(c => c.AGRCOST_APPL).FirstOrDefault();
        //                //objPmis.PLANNED_START_DATE_APPL_U = dbContext.PMIS_ACTIVITY_MASTER.Where(y => y.ACTIVITY_ID == item.ACTIVITY_ID).Select(c => c.PLANNED_START_DATE_APPL).FirstOrDefault();
        //                objPmis.PLANNED_START_DATE_APPL_U = dbContext.PMIS_ACTIVITY_MASTER.Where(y => y.ACTIVITY_ID == item.ACTIVITY_ID && y.ROAD_TYPE == "L").Select(c => c.PLANNED_START_DATE_APPL).FirstOrDefault();
        //                //objPmis.PLANNED_DURATION_APPL_U = dbContext.PMIS_ACTIVITY_MASTER.Where(y => y.ACTIVITY_ID == item.ACTIVITY_ID).Select(c => c.PLANNED_DURATION_APPL).FirstOrDefault();
        //                objPmis.PLANNED_DURATION_APPL_U = dbContext.PMIS_ACTIVITY_MASTER.Where(y => y.ACTIVITY_ID == item.ACTIVITY_ID && y.ROAD_TYPE == "L").Select(c => c.PLANNED_DURATION_APPL).FirstOrDefault();
        //                //objPmis.PLANNED_COMPLETION_DATE_APPL_U = dbContext.PMIS_ACTIVITY_MASTER.Where(y => y.ACTIVITY_ID == item.ACTIVITY_ID).Select(c => c.PLANNED_COMPLETION_DATE_APPL).FirstOrDefault();
        //                objPmis.PLANNED_COMPLETION_DATE_APPL_U = dbContext.PMIS_ACTIVITY_MASTER.Where(y => y.ACTIVITY_ID == item.ACTIVITY_ID && y.ROAD_TYPE == "L").Select(c => c.PLANNED_COMPLETION_DATE_APPL).FirstOrDefault();

        //                objlist.Add(objPmis);
        //            }
        //            foreach (var model in objlist)
        //            {
        //                if (model.AGREEMENT_COST != null)
        //                {
        //                    TotalCost = TotalCost + model.AGREEMENT_COST;
        //                }
        //            }
        //            ViewBag.TotalAgreementCost = TotalCost;

        //        }
        //        else
        //        {
        //            return Json(new { Success = false, ErrorMessage = "Plan Details Not Found" }, JsonRequestBehavior.AllowGet);
        //        }

        //        return View("~/Areas/PMIS/Views/PMIS/UpdatePMISBridgeProjectPlanLayout.cshtml", objlist);
        //    }
        //    catch (Exception ex)
        //    {
        //        ErrorLog.LogError(ex, "UpdatePMISBridgeProjectPlanLayout()");
        //        return null;
        //    }
        //}

        //[HttpPost]
        //[Audit]
        //public ActionResult UpdatePMISBridgeProjectPlan(IEnumerable<AddPlanPMISViewModelBridge> formData)
        //{
        //    PMISDAL objDAL = new PMISDAL();
        //    PMGSYEntities dbContext = new PMGSYEntities();
        //    try
        //    {
        //        ViewBag.Operation = "U";
        //        if (formData != null)
        //        {
        //            decimal TotalAgreementCost = 0;
        //            decimal AgreementValue = 0;
        //            int roadCode = 0;
        //            foreach (var item in formData)
        //            {
        //                if (item.AGREEMENT_COST != null)
        //                {
        //                    TotalAgreementCost += Convert.ToDecimal(item.AGREEMENT_COST);
        //                }

        //                if (item.IMS_PR_ROAD_CODE > 0)
        //                {
        //                    roadCode = item.IMS_PR_ROAD_CODE;
        //                }
        //                //decimal SanLength = dbContext.IMS_SANCTIONED_PROJECTS.Where(x => x.IMS_PR_ROAD_CODE == item.IMS_PR_ROAD_CODE).Select(x => x.IMS_PAV_LENGTH).FirstOrDefault();
        //                decimal SanLength = Convert.ToDecimal(dbContext.IMS_SANCTIONED_PROJECTS.Where(x => x.IMS_PR_ROAD_CODE == item.IMS_PR_ROAD_CODE).Select(x => x.IMS_BRIDGE_LENGTH).FirstOrDefault());
        //                if ((item.ACTIVITY_DESC != null || item.ACTIVITY_DESC != ""))
        //                {

        //                    if (item.ACTIVITY_DESC != "Miscellaneous")
        //                    {
        //                        if (item.ACTIVITY_DESC == "Resource Mobilization" || item.ACTIVITY_DESC == "Field Lab")
        //                        {
        //                            if (item.PLANNED_START_DATE == null || item.PLANNED_COMPLETION_DATE == null)
        //                            {
        //                                return Json(new { Success = false, ErrorMessage = "Planned Start Date and Planned End date must be Entered for " + item.ACTIVITY_DESC });
        //                            }

        //                        }
        //                        else if ((item.QUANTITY != 0 && item.QUANTITY != null) && (item.AGREEMENT_COST != 0 && item.AGREEMENT_COST != null))
        //                        {

        //                        }
        //                        else if ((item.QUANTITY != 0 && item.QUANTITY != null) && item.AGREEMENT_COST == 0)
        //                        {
        //                            return Json(new { Success = false, ErrorMessage = "Agreement Cost cannot be Zero for which Quantity entered is greater than zero" });
        //                        }
        //                        else if (item.QUANTITY == 0 && item.AGREEMENT_COST == 0)
        //                        {

        //                        }
        //                        else if ((item.QUANTITY == null) || (item.AGREEMENT_COST == null || item.PLANNED_START_DATE == null || item.PLANNED_COMPLETION_DATE == null))
        //                        {
        //                            return Json(new { Success = false, ErrorMessage = "All the * fields are mandatory" });
        //                        }

        //                    }
        //                    else if (item.AGREEMENT_COST == null)
        //                    {
        //                        return Json(new { Success = false, ErrorMessage = "All the * fields are mandatory" });
        //                    }
        //                    else
        //                    {

        //                    }
        //                }
        //                //if (item.QUANTITY != null && item.QUANTITY > SanLength && item.ACTIVITY_DESC != "Well Sinking" && item.ACTIVITY_DESC != "Pile Foundation" && item.ACTIVITY_DESC != "Bearings" && item.ACTIVITY_DESC != "Railings" && item.ACTIVITY_DESC != "Road Logo and other Furnitures")
        //                //{
        //                //    return Json(new { Success = false, ErrorMessage = "For each individual activity, Quantity entered should not be more than sanctioned length." });
        //                //}

        //                if (item.QUANTITY == null && item.AGREEMENT_COST > 0 && item.ACTIVITY_DESC != "Miscellaneous")
        //                {
        //                    return Json(new { Success = false, ErrorMessage = "Agreement cost cannot be entered for activities for which quantity has not been entered." });
        //                }
        //                if (item.PLANNED_START_DATE != null && item.PLANNED_COMPLETION_DATE != null && item.PLANNED_START_DATE > item.PLANNED_COMPLETION_DATE)
        //                {
        //                    return Json(new { Success = false, ErrorMessage = "Planned Completion date cannot be before the planned start date of an activity." });
        //                }
        //                if ((item.PLANNED_START_DATE == null || item.PLANNED_COMPLETION_DATE == null) && (item.QUANTITY > 0))
        //                {
        //                    return Json(new { Success = false, ErrorMessage = "Planned Start and End Date should be entered for activities for which quantity has been entered." });
        //                }
        //                //if ((item.PLANNED_START_DATE == null || item.PLANNED_COMPLETION_DATE == null) && item.ACTIVITY_DESC == "Field Lab")
        //                //{
        //                //    return Json(new { Success = false, ErrorMessage = "Field Lab Start and End Date should be entered." });
        //                //}
        //            }
        //            AgreementValue = dbContext.TEND_AGREEMENT_DETAIL.Where(x => x.IMS_PR_ROAD_CODE == roadCode).Select(x => x.TEND_AGREEMENT_AMOUNT).FirstOrDefault();
        //            if (TotalAgreementCost != AgreementValue)
        //            {
        //                return Json(new { Success = false, ErrorMessage = "The Sum of Agreement cost of all activities should match the Agreement Value for this road" });
        //            }
        //            string Status = objDAL.UpdatePMISBridgeProjectPlanDAL(formData);
        //            if (Status == string.Empty)
        //                return Json(new { Success = true });
        //            else
        //                return Json(new { Success = false, ErrorMessage = Status });
        //        }
        //        else
        //        {
        //            return Json(new { Success = false, ErrorMessage = "Update Unsuccessful" });
        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        ErrorLog.LogError(ex, "UpdatePMISBridgeProjectPlan(IEnumerable<AddPlanPMISViewModelBridge> formData)");
        //        throw ex;
        //    }
        //}

        //[HttpGet]
        //[Audit]
        //public ActionResult ViewPMISBridgeProjectPlanLayout(String parameter, String hash, String key)
        //{
        //    string status = string.Empty;
        //    PMISDAL objDAL = new PMISDAL();
        //    PMGSYEntities dbContext = new PMGSYEntities();
        //    Dictionary<string, string> decryptedParameters = null;
        //    try
        //    {
        //        decryptedParameters = URLEncrypt.DecryptParameters1(new string[] { parameter, hash, key });
        //        int IMS_PR_ROAD_CODE = 0; string StateShare = String.Empty; string MordShare = String.Empty; string TotalSanctionedCost = String.Empty;
        //        decimal stateShare = 0; decimal mordShare = 0; decimal totalSancCost = 0;
        //        if (decryptedParameters.Count() > 0)
        //        {

        //            IMS_PR_ROAD_CODE = Convert.ToInt32(decryptedParameters["RoadCode"].ToString());
        //            stateShare = Math.Round(Convert.ToDecimal(decryptedParameters["StateShare"]), 2);
        //            StateShare = stateShare.ToString();
        //            mordShare = Math.Round(Convert.ToDecimal(decryptedParameters["MordShare"]), 2);
        //            MordShare = mordShare.ToString();
        //            totalSancCost = Math.Round(Convert.ToDecimal(decryptedParameters["TotalSanctionedDate"]), 2);
        //            TotalSanctionedCost = totalSancCost.ToString();
        //        }
        //        else
        //        {
        //            return Json(new { success = false, errorMessage = "Invalid Road Code." });
        //        }


        //        List<AddPlanPMISViewModelBridge> objlist = new List<AddPlanPMISViewModelBridge>();
        //        decimal? SumAgreementCost = 0;
        //        var viewlatest = dbContext.PMIS_PLAN_DETAILS.Where(x => x.IMS_PR_ROAD_CODE == IMS_PR_ROAD_CODE & x.IS_LATEST == "Y").ToList();
        //        if (viewlatest != null)
        //        {

        //            foreach (var item in viewlatest)
        //            {
        //                AddPlanPMISViewModelBridge objPmis = new AddPlanPMISViewModelBridge();
        //                objPmis.IMS_PR_ROAD_CODE = IMS_PR_ROAD_CODE;
        //                objPmis.StateName = PMGSYSession.Current.StateName;
        //                //objPmis.ACTIVITY_DESC = dbContext.PMIS_ACTIVITY_MASTER.Where(y => y.ACTIVITY_ID == item.ACTIVITY_ID).Select(c => c.ACTIVITY_DESC).FirstOrDefault();
        //                objPmis.ACTIVITY_DESC = dbContext.PMIS_ACTIVITY_MASTER.Where(y => y.ACTIVITY_ID == item.ACTIVITY_ID && y.ROAD_TYPE == "L").Select(c => c.ACTIVITY_DESC).FirstOrDefault();
        //                //objPmis.ACTIVITY_UNIT = dbContext.PMIS_ACTIVITY_MASTER.Where(y => y.ACTIVITY_ID == item.ACTIVITY_ID).Select(c => c.ACTIVITY_UNIT).FirstOrDefault();
        //                objPmis.ACTIVITY_UNIT = dbContext.PMIS_ACTIVITY_MASTER.Where(y => y.ACTIVITY_ID == item.ACTIVITY_ID && y.ROAD_TYPE == "L").Select(c => c.ACTIVITY_UNIT).FirstOrDefault();
        //                objPmis.QUANTITY = item.QUANTITY;
        //                objPmis.AGREEMENT_COST = item.AGREEMENT_COST;
        //                objPmis.PLANNED_START_DATE = item.PLANNED_START_DATE;
        //                objPmis.View_Planned_Start_Date = item.PLANNED_START_DATE == null ? " " : item.PLANNED_START_DATE.Value.ToShortDateString(); //.Value.ToShortDateString();
        //                objPmis.PLANNED_DURATION = item.PLANNED_DURATION;
        //                objPmis.View_Planned_Completion_Date = item.PLANNED_COMPLETION_DATE == null ? " " : item.PLANNED_COMPLETION_DATE.Value.ToShortDateString();
        //                objPmis.PLANNED_COMPLETION_DATE = item.PLANNED_COMPLETION_DATE;
        //                if (objPmis.AGREEMENT_COST != null)
        //                    SumAgreementCost = SumAgreementCost + objPmis.AGREEMENT_COST;

        //                objlist.Add(objPmis);
        //            }

        //            //ViewBag.RoadName = dbContext.IMS_SANCTIONED_PROJECTS.Where(x => x.IMS_PR_ROAD_CODE == IMS_PR_ROAD_CODE).Select(x => x.IMS_ROAD_NAME).FirstOrDefault();
        //            ViewBag.BridgeName = dbContext.IMS_SANCTIONED_PROJECTS.Where(x => x.IMS_PR_ROAD_CODE == IMS_PR_ROAD_CODE).Select(x => x.IMS_BRIDGE_NAME).FirstOrDefault();
        //            //ViewBag.SanctionedYear = dbContext.IMS_SANCTIONED_PROJECTS.Where(x => x.IMS_PR_ROAD_CODE == IMS_PR_ROAD_CODE).Select(x => x.IMS_YEAR).FirstOrDefault();
        //            var year = dbContext.IMS_SANCTIONED_PROJECTS.Where(x => x.IMS_PR_ROAD_CODE == IMS_PR_ROAD_CODE).Select(x => x.IMS_YEAR).FirstOrDefault();
        //            ViewBag.SanctionedYear = year.ToString() + "-" + (year + 1).ToString().Substring(2, 2);
        //            //ViewBag.SanctionedLength = Math.Round(dbContext.IMS_SANCTIONED_PROJECTS.Where(x => x.IMS_PR_ROAD_CODE == IMS_PR_ROAD_CODE).Select(x => x.IMS_PAV_LENGTH).FirstOrDefault(), 2);
        //            ViewBag.SanctionedLength = Math.Round(Convert.ToDecimal(dbContext.IMS_SANCTIONED_PROJECTS.Where(x => x.IMS_PR_ROAD_CODE == IMS_PR_ROAD_CODE).Select(x => x.IMS_BRIDGE_LENGTH).FirstOrDefault()), 2);
        //            ViewBag.PackageNo = dbContext.IMS_SANCTIONED_PROJECTS.Where(x => x.IMS_PR_ROAD_CODE == IMS_PR_ROAD_CODE).Select(x => x.IMS_PACKAGE_ID).FirstOrDefault();
        //            ViewBag.AgreementValue = dbContext.TEND_AGREEMENT_DETAIL.Where(x => x.IMS_PR_ROAD_CODE == IMS_PR_ROAD_CODE).Select(x => x.TEND_AGREEMENT_AMOUNT).FirstOrDefault();
        //            ViewBag.RoadPlanVersions = dbContext.PMIS_PLAN_MASTER.Where(x => x.IMS_PR_ROAD_CODE == IMS_PR_ROAD_CODE).Select(x => x.BASELINE_NO).ToList();
        //            ViewBag.CurrentRoadPlaneVersion = dbContext.PMIS_PLAN_MASTER.Where(x => x.IMS_PR_ROAD_CODE == IMS_PR_ROAD_CODE & x.IS_LATEST == "Y").Select(x => x.BASELINE_NO).FirstOrDefault();
        //            ViewBag.StateShare = StateShare;
        //            ViewBag.MordShare = MordShare;
        //            ViewBag.TotalSanctionedCost = TotalSanctionedCost;

        //            ViewBag.TotalAgreementCost = SumAgreementCost;
        //            DateTime StartDate = ViewBag.TotalPlannedStartDate = dbContext.PMIS_PLAN_DETAILS.Where(x => x.IMS_PR_ROAD_CODE == IMS_PR_ROAD_CODE & x.IS_LATEST == "Y").Select(x => x.PLANNED_START_DATE).Min();
        //            DateTime EndDate = ViewBag.TotalPlannedCompletion = dbContext.PMIS_PLAN_DETAILS.Where(x => x.IMS_PR_ROAD_CODE == IMS_PR_ROAD_CODE & x.IS_LATEST == "Y").Select(x => x.PLANNED_COMPLETION_DATE).Max();
        //            ViewBag.TotalPlannedDuration = (EndDate - StartDate).Days;
        //        }
        //        else
        //        {
        //            return Json(new { Success = false, ErrorMessage = "Plan Details Not Found" }, JsonRequestBehavior.AllowGet);
        //        }

        //        return PartialView("~/Areas/PMIS/Views/PMIS/ViewPMISBridgeProjectPlanLayout.cshtml", objlist);
        //    }
        //    catch (Exception ex)
        //    {
        //        ErrorLog.LogError(ex, "ViewPMISBridgeProjectPlanLayout()");
        //        return null;
        //    }
        //}

        //[HttpGet]
        //[Audit]
        //public ActionResult ViewPMISBridgeProjectPlan(String BridgeName, String StateShare, String MordShare, String TotalSanctionedCost, String baseline)
        //{
        //    string status = string.Empty;
        //    PMISDAL objDAL = new PMISDAL();
        //    PMGSYEntities dbContext = new PMGSYEntities();
        //    try
        //    {
        //        int RoadCode = 0;
        //        int BaseLine = 0;
        //        if (BridgeName != null)
        //        {
        //            //RoadCode = dbContext.IMS_SANCTIONED_PROJECTS.Where(x => x.IMS_ROAD_NAME == RoadName).Select(x => x.IMS_PR_ROAD_CODE).FirstOrDefault();
        //            RoadCode = dbContext.IMS_SANCTIONED_PROJECTS.Where(x => x.IMS_BRIDGE_NAME == BridgeName).Select(x => x.IMS_PR_ROAD_CODE).FirstOrDefault();
        //            BaseLine = Convert.ToInt32(baseline);


        //            List<AddPlanPMISViewModelBridge> objlist = new List<AddPlanPMISViewModelBridge>();
        //            decimal? SumAgreementCost = 0;
        //            var viewlatest = dbContext.PMIS_PLAN_DETAILS.Where(x => x.IMS_PR_ROAD_CODE == RoadCode & x.BASELINE_NO == BaseLine).ToList();
        //            if (viewlatest != null)
        //            {

        //                foreach (var item in viewlatest)
        //                {
        //                    AddPlanPMISViewModelBridge objPmis = new AddPlanPMISViewModelBridge();
        //                    objPmis.IMS_PR_ROAD_CODE = RoadCode;
        //                    objPmis.StateName = PMGSYSession.Current.StateName;
        //                    //objPmis.ACTIVITY_DESC = dbContext.PMIS_ACTIVITY_MASTER.Where(y => y.ACTIVITY_ID == item.ACTIVITY_ID).Select(c => c.ACTIVITY_DESC).FirstOrDefault();
        //                    objPmis.ACTIVITY_DESC = dbContext.PMIS_ACTIVITY_MASTER.Where(y => y.ACTIVITY_ID == item.ACTIVITY_ID && y.ROAD_TYPE == "L").Select(c => c.ACTIVITY_DESC).FirstOrDefault();
        //                    //objPmis.ACTIVITY_UNIT = dbContext.PMIS_ACTIVITY_MASTER.Where(y => y.ACTIVITY_ID == item.ACTIVITY_ID).Select(c => c.ACTIVITY_UNIT).FirstOrDefault();
        //                    objPmis.ACTIVITY_UNIT = dbContext.PMIS_ACTIVITY_MASTER.Where(y => y.ACTIVITY_ID == item.ACTIVITY_ID && y.ROAD_TYPE == "L").Select(c => c.ACTIVITY_UNIT).FirstOrDefault();
        //                    objPmis.QUANTITY = item.QUANTITY;
        //                    objPmis.AGREEMENT_COST = item.AGREEMENT_COST;
        //                    objPmis.PLANNED_START_DATE = item.PLANNED_START_DATE;
        //                    objPmis.View_Planned_Start_Date = item.PLANNED_START_DATE == null ? " " : item.PLANNED_START_DATE.Value.ToShortDateString();
        //                    objPmis.PLANNED_DURATION = item.PLANNED_DURATION;
        //                    objPmis.View_Planned_Completion_Date = item.PLANNED_COMPLETION_DATE == null ? " " : item.PLANNED_COMPLETION_DATE.Value.ToShortDateString();
        //                    objPmis.PLANNED_COMPLETION_DATE = item.PLANNED_COMPLETION_DATE;
        //                    if (objPmis.AGREEMENT_COST != null)
        //                        SumAgreementCost = SumAgreementCost + objPmis.AGREEMENT_COST;

        //                    objlist.Add(objPmis);
        //                }

        //                //ViewBag.RoadName = dbContext.IMS_SANCTIONED_PROJECTS.Where(x => x.IMS_PR_ROAD_CODE == RoadCode).Select(x => x.IMS_ROAD_NAME).FirstOrDefault();
        //                ViewBag.BridgeName = dbContext.IMS_SANCTIONED_PROJECTS.Where(x => x.IMS_PR_ROAD_CODE == RoadCode).Select(x => x.IMS_BRIDGE_NAME).FirstOrDefault();
        //                //ViewBag.SanctionedYear = dbContext.IMS_SANCTIONED_PROJECTS.Where(x => x.IMS_PR_ROAD_CODE == RoadCode).Select(x => x.IMS_YEAR).FirstOrDefault();
        //                var year = dbContext.IMS_SANCTIONED_PROJECTS.Where(x => x.IMS_PR_ROAD_CODE == RoadCode).Select(x => x.IMS_YEAR).FirstOrDefault();
        //                ViewBag.SanctionedYear = year.ToString() + "-" + (year + 1).ToString().Substring(2, 2);
        //                //ViewBag.SanctionedLength = dbContext.IMS_SANCTIONED_PROJECTS.Where(x => x.IMS_PR_ROAD_CODE == RoadCode).Select(x => x.IMS_PAV_LENGTH).FirstOrDefault();
        //                ViewBag.SanctionedLength = Math.Round(Convert.ToDecimal(dbContext.IMS_SANCTIONED_PROJECTS.Where(x => x.IMS_PR_ROAD_CODE == RoadCode).Select(x => x.IMS_BRIDGE_LENGTH).FirstOrDefault()), 2);
        //                ViewBag.PackageNo = dbContext.IMS_SANCTIONED_PROJECTS.Where(x => x.IMS_PR_ROAD_CODE == RoadCode).Select(x => x.IMS_PACKAGE_ID).FirstOrDefault();
        //                ViewBag.AgreementValue = dbContext.TEND_AGREEMENT_DETAIL.Where(x => x.IMS_PR_ROAD_CODE == RoadCode).Select(x => x.TEND_AGREEMENT_AMOUNT).FirstOrDefault();
        //                ViewBag.RoadPlanVersions = dbContext.PMIS_PLAN_MASTER.Where(x => x.IMS_PR_ROAD_CODE == RoadCode).Select(x => x.BASELINE_NO).ToList();
        //                ViewBag.CurrentRoadPlaneVersion = BaseLine;
        //                ViewBag.StateShare = StateShare;
        //                ViewBag.MordShare = MordShare;
        //                ViewBag.TotalSanctionedCost = TotalSanctionedCost;

        //                ViewBag.TotalAgreementCost = SumAgreementCost;
        //                DateTime StartDate = ViewBag.TotalPlannedStartDate = dbContext.PMIS_PLAN_DETAILS.Where(x => x.IMS_PR_ROAD_CODE == RoadCode & x.IS_LATEST == "Y").Select(x => x.PLANNED_START_DATE).Min();
        //                DateTime EndDate = ViewBag.TotalPlannedCompletion = dbContext.PMIS_PLAN_DETAILS.Where(x => x.IMS_PR_ROAD_CODE == RoadCode & x.IS_LATEST == "Y").Select(x => x.PLANNED_COMPLETION_DATE).Max();
        //                ViewBag.TotalPlannedDuration = (EndDate - StartDate).Days;
        //            }
        //            else
        //            {
        //                return Json(new { Success = false, ErrorMessage = "Plan Details Not Found" }, JsonRequestBehavior.AllowGet);
        //            }

        //            return PartialView("~/Areas/PMIS/Views/PMIS/ViewPMISBridgeProjectPlanLayout.cshtml", objlist);
        //        }
        //        else
        //        {
        //            return Json(new { success = false, errorMessage = "Invalid Road Code." });
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        ErrorLog.LogError(ex, "ViewPMISBridgeProjectPlanLayout()");
        //        return null;
        //    }
        //}

        //#endregion

        //#region Delete PMIS Project PLAN Bridge

        //[HttpPost]
        //[Audit]
        //public JsonResult DeletePmisBridgeProjectPlan(String parameter, String hash, String key)
        //{
        //    string status = string.Empty;
        //    PMISDAL objDAL = new PMISDAL();
        //    PMGSYEntities dbContext = new PMGSYEntities();
        //    Dictionary<string, string> decryptedParameters = null;
        //    try
        //    {
        //        decryptedParameters = URLEncrypt.DecryptParameters1(new string[] { parameter, hash, key });
        //        int IMS_PR_ROAD_CODE = 0;
        //        int PLAN_ID = 0;
        //        if (decryptedParameters.Count() > 0)
        //        {

        //            IMS_PR_ROAD_CODE = Convert.ToInt32(decryptedParameters["RoadCode"].ToString());
        //            PLAN_ID = Convert.ToInt32(decryptedParameters["PlanID"].ToString());

        //        }
        //        else
        //        {
        //            return Json(new { success = false, errorMessage = "Invalid Road Code." });
        //        }

        //        status = objDAL.DeleteBridgeProjectPlanDAL(IMS_PR_ROAD_CODE, PLAN_ID);
        //        if (status == string.Empty)
        //        {
        //            return Json(new { success = true });
        //        }
        //        else
        //        {
        //            return Json(new { success = false, errorMessage = status });
        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        ErrorLog.LogError(ex, "DeletePmisBridgeProjectPlan()");
        //        return Json(new { success = false, errorMessage = status });
        //    }
        //    finally
        //    {
        //        if (dbContext != null)
        //        {
        //            dbContext.Dispose();
        //        }
        //    }
        //}
        //#endregion

        //#region Finalize PMIS Project PLAN Bridge

        //[HttpPost]
        //[Audit]
        //public JsonResult FinalizePmisBridgeProjectPlan(String parameter, String hash, String key)
        //{
        //    string status = string.Empty;
        //    PMISDAL objDAL = new PMISDAL();
        //    PMGSYEntities dbContext = new PMGSYEntities();
        //    Dictionary<string, string> decryptedParameters = null;
        //    try
        //    {
        //        decryptedParameters = URLEncrypt.DecryptParameters1(new string[] { parameter, hash, key });
        //        int IMS_PR_ROAD_CODE = 0;
        //        if (decryptedParameters.Count() > 0)
        //        {

        //            IMS_PR_ROAD_CODE = Convert.ToInt32(decryptedParameters["RoadCode"].ToString());

        //        }
        //        else
        //        {
        //            return Json(new { success = false, errorMessage = "Invalid Road Code." });
        //        }

        //        status = objDAL.FinalizeBridgeProjectPlanDAL(IMS_PR_ROAD_CODE);
        //        if (status == string.Empty)
        //        {
        //            return Json(new { success = true });
        //        }
        //        else
        //        {
        //            return Json(new { success = false, errorMessage = status });
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        ErrorLog.LogError(ex, "FinalizePmisBridgeProjectPlan()");
        //        return Json(new { success = false, errorMessage = status });
        //    }
        //    finally
        //    {
        //        if (dbContext != null)
        //        {
        //            dbContext.Dispose();
        //        }
        //    }
        //}
        //#endregion

        //#region Revise PMIS Project PLAN Bridge

        //[HttpPost]
        //[Audit]
        //public ActionResult RevisePmisBridgeProjectPlan(String parameter, String hash, String key)
        //{
        //    string status = string.Empty;
        //    PMISDAL objDAL = new PMISDAL();
        //    PMGSYEntities dbContext = new PMGSYEntities();
        //    Dictionary<string, string> decryptedParameters = null;
        //    try
        //    {
        //        decryptedParameters = URLEncrypt.DecryptParameters1(new string[] { parameter, hash, key });
        //        int IMS_PR_ROAD_CODE = 0;
        //        if (decryptedParameters.Count() > 0)
        //        {

        //            IMS_PR_ROAD_CODE = Convert.ToInt32(decryptedParameters["RoadCode"].ToString());

        //        }
        //        else
        //        {
        //            return Json(new { success = false, errorMessage = "Invalid Road Code." });
        //            //return null;
        //        }

        //        status = objDAL.ReviseBridgeProjectPlanDAL(IMS_PR_ROAD_CODE);
        //        if (status == string.Empty)
        //        {
        //            PMISController controllerobj = new PMISController();
        //            // var updateobj = controllerobj.UpdatePMISRoadProjectPlanLayout(parameter, hash, key);
        //            return Json(new { success = true });
        //            //return null;
        //        }
        //        else
        //        {
        //            //return null;
        //            return Json(new { success = false, errorMessage = status });
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        ErrorLog.LogError(ex, "RevisePmisBridgeProjectPlan()");
        //        return Json(new { success = false, errorMessage = status });
        //        //return null;
        //    }
        //    finally
        //    {
        //        if (dbContext != null)
        //        {
        //            dbContext.Dispose();
        //        }
        //    }
        //}
        //#endregion

        #endregion

        #region PMIS Data Correction

        public ActionResult PMISDataCorrection()
        {
            PMISDataCorrectionModel model = new PMISDataCorrectionModel();
            CommonFunctions comm = new CommonFunctions();
            try
            {
                model.StateList = new List<SelectListItem>();
                model.StateList = comm.PopulateStates(true);
                model.StateList.RemoveAt(0);
                model.StateList.Insert(0, new SelectListItem { Text = "Select State", Value = "-1" });

                model.DistrictList = new List<SelectListItem>();
                model.DistrictList.Insert(0, new SelectListItem { Text = "All Districts", Value = "0" });

                model.BlockList = new List<SelectListItem>();
                model.BlockList.Insert(0, new SelectListItem { Text = "All Blocks", Value = "0" });

                model.Sanction_Year_List = comm.PopulateFinancialYear(true, true).ToList();
                model.Sanction_Year_List.RemoveAt(0);

                model.BatchList = comm.PopulateBatch();
                model.BatchList.RemoveAt(0);
                model.BatchList.Insert(0, new SelectListItem { Text = "All Batches", Value = "0" });


                model.ListTypeList = new List<SelectListItem>();
                model.ListTypeList.Insert(0, new SelectListItem { Text = "Road", Value = "R" });
                model.ListTypeList.Insert(1, new SelectListItem { Text = "Bridge", Value = "B" });

                return View("PMISDataCorrection", model);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "PMISController.PMISDataCorrection()");
                return null;
            }
        }

        [HttpPost]
        public JsonResult PopulateDistrictForPMISDataDeletion()
        {
            PMISDataCorrectionModel model = new PMISDataCorrectionModel();
            CommonFunctions objCommonFunctions = new CommonFunctions();
            try
            {
                int StateCode = Convert.ToInt32(Request.Params["StateCode"]);

                model.DistrictList = new List<SelectListItem>();
                model.DistrictList = objCommonFunctions.PopulateDistrictForSRRDA(StateCode);
                model.DistrictList.RemoveAt(0);
                model.DistrictList.Insert(0, new SelectListItem { Selected = true, Value = "0", Text = "All Districts" });

                model.BlockList = new List<SelectListItem>();
                model.BlockList.Insert(0, new SelectListItem { Text = "All Blocks", Value = "0" });

                return Json(model, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "PMISController.PopulateDistrictForPMISDataDeletion()");
                return Json(new { success = false, errorMessage = "Error while processing your request" });
            }
        }
                
        [HttpPost]
        public ActionResult PMISDataCorrectionList(int? page, int? rows, string sidx, string sord)
        {
            long totalRecords; int state = 0; int district = 0; int block = 0; int sanction_year = 0; int batch = 0; string listType = null;
            try
            {
                if (!string.IsNullOrEmpty(Request.Params["state"]))
                {
                    state = Convert.ToInt32(Request.Params["state"]);
                }

                if (!string.IsNullOrEmpty(Request.Params["district"]))
                {
                    district = Convert.ToInt32(Request.Params["district"]);

                }
                if (!string.IsNullOrEmpty(Request.Params["block"]))
                {
                    block = Convert.ToInt32(Request.Params["block"]);

                }
                if (!string.IsNullOrEmpty(Request.Params["sanction_year"]))
                {
                    sanction_year = Convert.ToInt32(Request.Params["sanction_year"]);

                }
                if (!string.IsNullOrEmpty(Request.Params["batch"]))
                {
                    batch = Convert.ToInt32(Request.Params["batch"]);
                }

                if (!string.IsNullOrEmpty(Request.Params["listType"]))
                {
                    listType = Convert.ToString(Request.Params["listType"]);
                }

                IPMISDAL dalobj = new PMISDAL();
                var jsonData = new
                {
                    rows = dalobj.PMISDataCorrectionList(state, district, block, sanction_year, batch, listType, Convert.ToInt32(Request.Params["page"]) - 1, Convert.ToInt32(Request.Params["rows"]), Request.Params["sidx"], Request.Params["sord"], out totalRecords),
                    total = totalRecords <= Convert.ToInt32(Request.Params["rows"]) ? 1 : totalRecords / Convert.ToInt32(Request.Params["rows"]) + 1,
                    page = Convert.ToInt32(Request.Params["page"]),
                    records = totalRecords
                };
                return Json(jsonData, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "PMISController.PMISDataCorrectionList()");
                return null;
            }
        }

        [HttpPost]
        public ActionResult GetPlanDetailsToEdit()
        {

            IPMISDAL objDAL = new PMISDAL();
            int RoadCode = 0;

            try
            {
                if (!string.IsNullOrEmpty(Request.Params["RoadCode"]))
                {
                    RoadCode = Convert.ToInt32(Request.Params["RoadCode"]);
                }

                UpdateCompletionDateLengthModel roadmodel = objDAL.GetPlanDetailsToEdit(RoadCode);
                if (roadmodel == null)
                {
                    return Json(new { success = false, errorMessage = "No Data Found" }, JsonRequestBehavior.AllowGet);

                }
                return View("GetPlanDetailsToEdit", roadmodel);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "PMISController.UpdateDataDeletionRoadDetails()");
                return Json(new { success = false, errorMessage = "Error Occured while proceesing your request" }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult UpdatePlanCompletionDetails()
        {

            int planid = 0, baselineno = 0;
            decimal completionLength = 0;
            DateTime? completionDate = null;
            string status = string.Empty, listType = null;

            try
            {
                if (!string.IsNullOrEmpty(Request.Params["listType"]))
                {
                    listType = Convert.ToString(Request.Params["listType"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["planid"]))
                {
                    planid = Convert.ToInt32(Request.Params["planid"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["baselineno"]))
                {
                    baselineno = Convert.ToInt32(Request.Params["baselineno"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["completionlength"]))
                {
                    completionLength = Convert.ToDecimal(Request.Params["completionlength"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["completiondate"]))
                {
                    completionDate = Convert.ToDateTime(Request.Params["completiondate"]);
                }

                IPMISDAL objDAL = new PMISDAL();
                status = objDAL.UpdatePlanCompletionDetails(listType, planid, baselineno, completionLength, completionDate);
                if (status == string.Empty)
                {
                    return Json(new { success = true }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { success = false, errorMessage = status }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "PMISController.UpdatePlanCompletionDetails()");
                return Json(new { success = status, errorMessage = "Error Occured while proceesing your request" }, JsonRequestBehavior.AllowGet);
            }

        }
                       
        [HttpPost]
        [Audit]
        public JsonResult DataDeleteProgressPlanList(String RoadCode, int? page, int? rows, string sidx, string sord)
        {
            long totalRecords;

            string status = string.Empty;
            IPMISDAL objDAL = new PMISDAL();
            PMGSYEntities dbContext = new PMGSYEntities();
            try
            {
                int roadCode = 0;
                roadCode = Convert.ToInt32(Request.Params["RoadCode"].ToString());
                if (roadCode <= 0)
                {
                    return Json(new { success = false, errorMessage = "Invalid Road Code." });
                }
                else
                {
                    var jsonData = new
                    {
                        rows = objDAL.PMISDataDeleteProgressPlanList(roadCode, Convert.ToInt32(Request.Params["page"]) - 1, Convert.ToInt32(Request.Params["rows"]), Request.Params["sidx"], Request.Params["sord"], out totalRecords),
                        total = totalRecords <= Convert.ToInt32(Request.Params["rows"]) ? 1 : totalRecords / Convert.ToInt32(Request.Params["rows"]) + 1,
                        page = Convert.ToInt32(Request.Params["page"]),
                        records = totalRecords
                    };
                    return Json(jsonData, JsonRequestBehavior.AllowGet);

                }
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "PMISController.DataDeleteProgressPlanList()");
                return null;
            }

        }
                       
        [HttpPost]
        [Audit]
        public JsonResult DataDeletePlan(int planId)
        {
            string status = string.Empty;
            IPMISDAL objDAL = new PMISDAL();
            try
            {
                int PLAN_ID = 0;
                if (planId > 0)
                {
                    PLAN_ID = Convert.ToInt32(Request.Params["planId"].ToString());
                }
                else
                {
                    return Json(new { success = false, errorMessage = "Invalid Plan !!" });
                }

                status = objDAL.DataDeletePlan(PLAN_ID);
                if (status == string.Empty)
                {
                    return Json(new { success = true });
                }
                else
                {
                    return Json(new { success = false, errorMessage = status });
                }
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "PMISController.DataDeletePlan()");
                return Json(new { success = false, errorMessage = status });
            }
        }
                      
        [HttpPost]
        [Audit]
        public JsonResult DataDeleteProgress(int planId)
        {
            string status = string.Empty;
            IPMISDAL objDAL = new PMISDAL();

            try
            {
                int PLAN_ID = 0;
                if (planId > 0)
                {
                    PLAN_ID = Convert.ToInt32(Request.Params["planId"].ToString());
                }
                else
                {
                    return Json(new { success = false, errorMessage = "Invalid Plan !!" });
                }

                status = objDAL.DataDeleteProgress(PLAN_ID);
                if (status == string.Empty)
                {
                    return Json(new { success = true });
                }
                else
                {
                    return Json(new { success = false, errorMessage = status });
                }
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "PMISController.DataDeleteProgress()");
                return Json(new { success = false, errorMessage = status });
            }
        }

        #endregion


        #region  PMIS BRIDGE 

    

        #region PMIS List Bridge
        public ActionResult PMISBridgeLayout()
        {
            ListPMISBridgeDetailsViewModel model = new ListPMISBridgeDetailsViewModel();
            CommonFunctions comm = new CommonFunctions();
            try
            {
                model.StateList = new List<SelectListItem>();
                model.StateList.Insert(0, new SelectListItem() { Text = PMGSYSession.Current.StateName, Value = PMGSYSession.Current.StateCode.ToString() });
                model.DistrictList = new List<SelectListItem>();
                if (PMGSYSession.Current.DistrictCode == 0)
                {
                    model.DistrictList = comm.PopulateDistrictForSRRDA(PMGSYSession.Current.StateCode);
                    // model.DistrictList.Find(x => x.Value == "0").Text = "Select District";
                }
                else if (PMGSYSession.Current.DistrictCode > 0)
                {

                    model.DistrictList.Insert(0, new SelectListItem() { Text = PMGSYSession.Current.DistrictName, Value = PMGSYSession.Current.DistrictCode.ToString() });
                }

                model.BlockList = new List<SelectListItem>();
                if (PMGSYSession.Current.DistrictCode == 0)
                {
                    model.BlockList.Insert(0, (new SelectListItem { Text = "All Blocks", Value = "0", Selected = true }));
                }
                else
                {
                    model.BlockList = comm.PopulateBlocks((PMGSYSession.Current.DistrictCode), true);

                    model.BlockList.Find(x => x.Value == "-1").Value = "0";
                    model.BlockList.Find(x => x.Value == model.BlockCode.ToString()).Selected = true;
                }
                model.Sanction_Year_List = comm.PopulateFinancialYear(true, true).ToList();
                model.Sanction_Year_List.RemoveAt(0);

                model.BatchList = comm.PopulateBatch();
                model.BatchList.RemoveAt(0);
                model.BatchList.Insert(0, new SelectListItem { Value = "0", Text = "All Batches" });

                return View(model);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "PMISController.PMISBridgeLayout()");
                return null;
            }
        }

        [HttpPost]
        public ActionResult PMISBridgeList(int? page, int? rows, string sidx, string sord)
        {
            long totalRecords; int state = 0; int district = 0; int block = 0; int sanction_year = 0; int batch = 0;
            try
            {
                if (!string.IsNullOrEmpty(Request.Params["state"]))
                {
                    state = Convert.ToInt32(Request.Params["state"]);
                }

                if (!string.IsNullOrEmpty(Request.Params["district"]))
                {
                    district = Convert.ToInt32(Request.Params["district"]);

                }
                if (!string.IsNullOrEmpty(Request.Params["block"]))
                {
                    block = Convert.ToInt32(Request.Params["block"]);

                }
                if (!string.IsNullOrEmpty(Request.Params["sanction_year"]))
                {
                    sanction_year = Convert.ToInt32(Request.Params["sanction_year"]);

                }
                if (!string.IsNullOrEmpty(Request.Params["batch"]))
                {
                    batch = Convert.ToInt32(Request.Params["batch"]);

                }

                PMISDAL dalobj = new PMISDAL();
                var jsonData = new
                {
                    rows = dalobj.PMISBridgeListDAL(state, district, block, sanction_year, batch, Convert.ToInt32(Request.Params["page"]) - 1, Convert.ToInt32(Request.Params["rows"]), Request.Params["sidx"], Request.Params["sord"], out totalRecords),
                    total = totalRecords <= Convert.ToInt32(Request.Params["rows"]) ? 1 : totalRecords / Convert.ToInt32(Request.Params["rows"]) + 1,
                    page = Convert.ToInt32(Request.Params["page"]),
                    records = totalRecords
                };
                return Json(jsonData, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "PMISController.PMISBridgeList()");
                return null;
            }
        }
        #endregion

        #region Add/Update/View Project PLAN for Bridge
        [HttpGet]
        [Audit]
        public ViewResult AddPMISBridgeProjectPlan(String parameter, String hash, String key)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            PMISDAL objDAL = new PMISDAL();
            AddPlanPMISViewModelBridge pmisViewModel = new AddPlanPMISViewModelBridge();
            Dictionary<string, string> decryptedParameters = null;
            ViewBag.operation = "A";
            //int roadCode = Convert.ToInt32(RoadCode);    

            try
            {
                decryptedParameters = URLEncrypt.DecryptParameters1(new string[] { parameter, hash, key });
                int IMS_PR_ROAD_CODE = 0; string StateShare = String.Empty; string MordShare = String.Empty; string TotalSanctionedCost = String.Empty;
                decimal stateShare = 0; decimal mordShare = 0; decimal totalSancCost = 0;
                if (decryptedParameters.Count() > 0)
                {
                    IMS_PR_ROAD_CODE = Convert.ToInt32(decryptedParameters["RoadCode"].ToString());
                    stateShare = Math.Round(Convert.ToDecimal(decryptedParameters["StateShare"]), 2);
                    StateShare = stateShare.ToString();
                    mordShare = Math.Round(Convert.ToDecimal(decryptedParameters["MordShare"]), 2);
                    MordShare = mordShare.ToString();
                    totalSancCost = Math.Round(Convert.ToDecimal(decryptedParameters["TotalSanctionedDate"]), 2);
                    TotalSanctionedCost = totalSancCost.ToString();
                }
                else
                {
                    return null;
                }
                if (IMS_PR_ROAD_CODE != 0)
                {


                    pmisViewModel.IMS_PR_ROAD_CODE = IMS_PR_ROAD_CODE;

                    //pmisViewModel.IMS_ROAD_NAME = dbContext.IMS_SANCTIONED_PROJECTS.Where(x => x.IMS_PR_ROAD_CODE == pmisViewModel.IMS_PR_ROAD_CODE).Select(x => x.IMS_ROAD_NAME).FirstOrDefault();
                    pmisViewModel.IMS_BRIDGE_NAME = dbContext.IMS_SANCTIONED_PROJECTS.Where(x => x.IMS_PR_ROAD_CODE == pmisViewModel.IMS_PR_ROAD_CODE).Select(x => x.IMS_BRIDGE_NAME).FirstOrDefault();
                    var year = dbContext.IMS_SANCTIONED_PROJECTS.Where(x => x.IMS_PR_ROAD_CODE == IMS_PR_ROAD_CODE).Select(x => x.IMS_YEAR).FirstOrDefault();
                    pmisViewModel.IMS_YEAR = year.ToString() + "-" + (year + 1).ToString().Substring(2, 2);
                    //pmisViewModel.IMS_PAV_LENGTH = Math.Round(dbContext.IMS_SANCTIONED_PROJECTS.Where(x => x.IMS_PR_ROAD_CODE == pmisViewModel.IMS_PR_ROAD_CODE).Select(x => x.IMS_PAV_LENGTH).FirstOrDefault(), 2);
                    pmisViewModel.IMS_BRIDGE_LENGTH = Math.Round(Convert.ToDecimal(dbContext.IMS_SANCTIONED_PROJECTS.Where(x => x.IMS_PR_ROAD_CODE == pmisViewModel.IMS_PR_ROAD_CODE).Select(x => x.IMS_BRIDGE_LENGTH).FirstOrDefault()), 2);
                    pmisViewModel.IMS_PACKAGE_ID = dbContext.IMS_SANCTIONED_PROJECTS.Where(x => x.IMS_PR_ROAD_CODE == pmisViewModel.IMS_PR_ROAD_CODE).Select(x => x.IMS_PACKAGE_ID).FirstOrDefault();
                    var Agreement_Id = dbContext.TEND_AGREEMENT_DETAIL.Where(x => x.IMS_PR_ROAD_CODE == IMS_PR_ROAD_CODE && x.TEND_AGREEMENT_STATUS != "W").Max(x => x.TEND_AGREEMENT_ID);

                    pmisViewModel.AGREEMENT_VALUE = dbContext.TEND_AGREEMENT_DETAIL.Where(x => x.IMS_PR_ROAD_CODE == pmisViewModel.IMS_PR_ROAD_CODE && x.TEND_AGREEMENT_ID == Agreement_Id).Select(x => x.TEND_AGREEMENT_AMOUNT + x.GST_AMT_MAINT_AGREEMENT_DLP).FirstOrDefault();


                    pmisViewModel.StateShare = StateShare;
                    pmisViewModel.MordShare = MordShare;
                    pmisViewModel.TotalSanctionedCost = TotalSanctionedCost;

                    pmisViewModel.StateName = PMGSYSession.Current.StateName;
                    pmisViewModel.DistrictName = PMGSYSession.Current.DistrictName;
                    pmisViewModel.Activity_Desc_List = (from x in dbContext.PMIS_ACTIVITY_MASTER where x.ROAD_TYPE == "L" select x.ACTIVITY_DESC).ToArray();
                    pmisViewModel.Activity_Unit_List = (from x in dbContext.PMIS_ACTIVITY_MASTER where x.ROAD_TYPE == "L" select x.ACTIVITY_UNIT).ToArray();
                    pmisViewModel.QUANTITY_APPL = (from x in dbContext.PMIS_ACTIVITY_MASTER where x.ROAD_TYPE == "L" select x.QUANTITY_APPL).ToArray();
                    pmisViewModel.AGRCOST_APPL = (from x in dbContext.PMIS_ACTIVITY_MASTER where x.ROAD_TYPE == "L" select x.AGRCOST_APPL).ToArray();
                    pmisViewModel.PLANNED_START_DATE_APPL = (from x in dbContext.PMIS_ACTIVITY_MASTER where x.ROAD_TYPE == "L" select x.PLANNED_START_DATE_APPL).ToArray();
                    pmisViewModel.PLANNED_DURATION_APPL = (from x in dbContext.PMIS_ACTIVITY_MASTER where x.ROAD_TYPE == "L" select x.PLANNED_DURATION_APPL).ToArray();
                    pmisViewModel.PLANNED_COMPLETION_DATE_APPL = (from x in dbContext.PMIS_ACTIVITY_MASTER where x.ROAD_TYPE == "L" select x.PLANNED_COMPLETION_DATE_APPL).ToArray();

                    ViewData["ActivityList"] = pmisViewModel.Activity_Desc_List;
                }
                return View("~/Areas/PMIS/Views/PMIS/AddPMISBridgeProjectPlan.cshtml", pmisViewModel);

            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "PMIS.AddPMISBridgeProjectPlan()");
                return null;
            }
        }

        [HttpPost]
        [Audit]
        public ActionResult SaveBridgeProjectPlan(IEnumerable<AddPlanPMISViewModelBridge> formData)
        {
            PMISDAL objDAL = new PMISDAL();
            PMGSYEntities dbContext = new PMGSYEntities();
            try
            {
                ViewBag.operation = "A";
                if (ModelState.IsValid)
                {
                    decimal TotalAgreementCost = 0;
                    decimal AgreementValue = 0;
                    int roadCode = 0;
                    foreach (var item in formData)
                    {
                        if (item.AGREEMENT_COST != null)
                        {
                            TotalAgreementCost += Convert.ToDecimal(item.AGREEMENT_COST);
                        }

                        if (item.IMS_PR_ROAD_CODE > 0)
                        {
                            roadCode = item.IMS_PR_ROAD_CODE;
                        }
                        //decimal SanLength = dbContext.IMS_SANCTIONED_PROJECTS.Where(x => x.IMS_PR_ROAD_CODE == item.IMS_PR_ROAD_CODE).Select(x => x.IMS_PAV_LENGTH).FirstOrDefault();
                        decimal SanLength = Convert.ToDecimal(dbContext.IMS_SANCTIONED_PROJECTS.Where(x => x.IMS_PR_ROAD_CODE == item.IMS_PR_ROAD_CODE).Select(x => x.IMS_BRIDGE_LENGTH).FirstOrDefault());

                        //if (item.QUANTITY != null && item.QUANTITY > SanLength)
                        //{
                        //    return Json(new { Success = false, ErrorMessage = "For each individual activity, Quantity entered should not be more than sanctioned length." });
                        //}
                        if ((item.ACTIVITY_DESC != null || item.ACTIVITY_DESC != ""))
                        {

                            if (item.ACTIVITY_DESC != "Miscellaneous")
                            {
                                if (item.ACTIVITY_DESC == "Resource Mobilization" || item.ACTIVITY_DESC == "Field Lab")
                                {
                                    if (item.PLANNED_START_DATE == null || item.PLANNED_COMPLETION_DATE == null)
                                    {
                                        return Json(new { Success = false, ErrorMessage = "Planned Start Date and Planned End date must be Entered for " + item.ACTIVITY_DESC });
                                    }

                                }

                                else if ((item.QUANTITY != 0 && item.QUANTITY != null) && item.AGREEMENT_COST == 0)
                                {
                                    return Json(new { Success = false, ErrorMessage = "Agreement Cost cannot be Zero for which Quantity entered is greater than zero" });
                                }

                            }

                        }


                        if (item.QUANTITY == null && item.AGREEMENT_COST > 0 && item.ACTIVITY_DESC != "Miscellaneous")
                        {
                            return Json(new { Success = false, ErrorMessage = "Agreement cost cannot be entered for activities for which quantity has not been entered." });
                        }
                        if (item.PLANNED_START_DATE != null && item.PLANNED_COMPLETION_DATE != null && item.PLANNED_START_DATE > item.PLANNED_COMPLETION_DATE)
                        {
                            return Json(new { Success = false, ErrorMessage = "Planned Completion date cannot be before the planned start date of an activity." });
                        }
                        if ((item.PLANNED_START_DATE == null || item.PLANNED_COMPLETION_DATE == null) && (item.QUANTITY > 0))
                        {
                            return Json(new { Success = false, ErrorMessage = "Planned Start and End Date should be entered for activities for which quantity has been entered." });
                        }

                    }
                    var Agreement_Id = dbContext.TEND_AGREEMENT_DETAIL.Where(x => x.IMS_PR_ROAD_CODE == roadCode && x.TEND_AGREEMENT_STATUS != "W").Max(x => x.TEND_AGREEMENT_ID);
                    AgreementValue = dbContext.TEND_AGREEMENT_DETAIL.Where(x => x.IMS_PR_ROAD_CODE == roadCode && x.TEND_AGREEMENT_ID == Agreement_Id).Select(x => x.TEND_AGREEMENT_AMOUNT + x.GST_AMT_MAINT_AGREEMENT_DLP).FirstOrDefault();
                    if (TotalAgreementCost != AgreementValue)
                    {
                        return Json(new { Success = false, ErrorMessage = "The Sum of Agreement cost of all activities should match the Agreement Value for this road" });
                    }

                    string Status = objDAL.SaveBridgeProjectPlanDAL(formData);
                    if (Status == string.Empty)
                        return Json(new { Success = true });
                    else
                        return Json(new { Success = false, ErrorMessage = Status });
                }
                else
                {
                    return Json(new { Success = false, ErrorMessage = new CommonFunctions().FormatErrorMessage(ModelState) });
                }
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "SaveBridgeProjectPlan(AddPlanPMISViewModelBridge formData))");
                throw ex;
            }
        }

        [HttpGet]
        [Audit]
        public ActionResult UpdatePMISBridgeProjectPlanLayout(String parameter, String hash, String key)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            PMISDAL objlDAL = new PMISDAL();
            CommonFunctions objCommonFuntion = new CommonFunctions();
            Dictionary<string, string> decryptedParameters = null;
            string status = string.Empty;

            try
            {
                decryptedParameters = URLEncrypt.DecryptParameters1(new string[] { parameter, hash, key });
                int IMS_PR_ROAD_CODE = 0; string StateShare = String.Empty; string MordShare = String.Empty; string TotalSanctionedCost = String.Empty;
                decimal stateShare = 0; decimal mordShare = 0; decimal totalSancCost = 0; string ReviseOperationStatus = string.Empty;
                if (decryptedParameters.Count() > 0)
                {
                    IMS_PR_ROAD_CODE = Convert.ToInt32(decryptedParameters["RoadCode"].ToString());
                    stateShare = Math.Round(Convert.ToDecimal(decryptedParameters["StateShare"]), 2);
                    StateShare = stateShare.ToString();
                    mordShare = Math.Round(Convert.ToDecimal(decryptedParameters["MordShare"]), 2);
                    MordShare = mordShare.ToString();
                    totalSancCost = Math.Round(Convert.ToDecimal(decryptedParameters["TotalSanctionedDate"]), 2);
                    if (decryptedParameters.ContainsKey("ReviseStatus"))
                    {
                        ReviseOperationStatus = decryptedParameters["ReviseStatus"];
                    }
                    TotalSanctionedCost = totalSancCost.ToString();
                }
                else
                {
                    return Json(new { success = false, errorMessage = "Invalid Road Code." });
                }


                ViewBag.operation = "U";



                if (!dbContext.PMIS_PLAN_MASTER.Any(m => m.IMS_PR_ROAD_CODE == IMS_PR_ROAD_CODE))
                {
                    return Json(new { Success = false, ErrorMessage = "Plan Not Found" }, JsonRequestBehavior.AllowGet);
                }
                List<AddPlanPMISViewModelBridge> objlist = new List<AddPlanPMISViewModelBridge>();

                var latestRecord = dbContext.PMIS_PLAN_DETAILS.Where(x => x.IMS_PR_ROAD_CODE == IMS_PR_ROAD_CODE & x.IS_LATEST == "Y").ToList();
                if (latestRecord != null)
                {
                    ViewBag.IMS_PR_ROAD_CODE = IMS_PR_ROAD_CODE;
                    //ViewBag.IMS_ROAD_NAME = dbContext.IMS_SANCTIONED_PROJECTS.Where(x => x.IMS_PR_ROAD_CODE == IMS_PR_ROAD_CODE).Select(x => x.IMS_ROAD_NAME).FirstOrDefault();
                    ViewBag.IMS_BRIDGE_NAME = dbContext.IMS_SANCTIONED_PROJECTS.Where(x => x.IMS_PR_ROAD_CODE == IMS_PR_ROAD_CODE).Select(x => x.IMS_BRIDGE_NAME).FirstOrDefault();
                    var year = dbContext.IMS_SANCTIONED_PROJECTS.Where(x => x.IMS_PR_ROAD_CODE == IMS_PR_ROAD_CODE).Select(x => x.IMS_YEAR).FirstOrDefault();
                    ViewBag.IMS_YEAR = year.ToString() + "-" + (year + 1).ToString().Substring(2, 2);
                    //ViewBag.IMS_PAV_LENGTH = Math.Round(dbContext.IMS_SANCTIONED_PROJECTS.Where(x => x.IMS_PR_ROAD_CODE == IMS_PR_ROAD_CODE).Select(x => x.IMS_PAV_LENGTH).FirstOrDefault(), 2);
                    ViewBag.IMS_BRIDGE_LENGTH = Math.Round(Convert.ToDecimal(dbContext.IMS_SANCTIONED_PROJECTS.Where(x => x.IMS_PR_ROAD_CODE == IMS_PR_ROAD_CODE).Select(x => x.IMS_BRIDGE_LENGTH).FirstOrDefault()), 2);
                    ViewBag.IMS_PACKAGE_ID = dbContext.IMS_SANCTIONED_PROJECTS.Where(x => x.IMS_PR_ROAD_CODE == IMS_PR_ROAD_CODE).Select(x => x.IMS_PACKAGE_ID).FirstOrDefault();
                    var Agreement_Id = dbContext.TEND_AGREEMENT_DETAIL.Where(x => x.IMS_PR_ROAD_CODE == IMS_PR_ROAD_CODE && x.TEND_AGREEMENT_STATUS != "W").Max(x => x.TEND_AGREEMENT_ID);
                    ViewBag.AGREEMENT_VALUE = dbContext.TEND_AGREEMENT_DETAIL.Where(x => x.IMS_PR_ROAD_CODE == IMS_PR_ROAD_CODE && x.TEND_AGREEMENT_ID == Agreement_Id).Select(x => x.TEND_AGREEMENT_AMOUNT + x.GST_AMT_MAINT_AGREEMENT_DLP).FirstOrDefault();
                    ViewBag.StateName = PMGSYSession.Current.StateName;
                    ViewBag.DistrictName = PMGSYSession.Current.DistrictName;
                    ViewBag.StateShare = StateShare;
                    ViewBag.MordShare = MordShare;
                    ViewBag.TotalSanctionedCost = TotalSanctionedCost;
                    ViewBag.BASELINE_NO = (from x in dbContext.PMIS_PLAN_MASTER where x.IMS_PR_ROAD_CODE == IMS_PR_ROAD_CODE && x.IS_LATEST == "Y" select x.BASELINE_NO).FirstOrDefault();
                    DateTime StartDate = ViewBag.TotalPlannedStartDate = dbContext.PMIS_PLAN_DETAILS.Where(x => x.IMS_PR_ROAD_CODE == IMS_PR_ROAD_CODE & x.IS_LATEST == "Y").Select(x => x.PLANNED_START_DATE).Min();
                    DateTime EndDate = ViewBag.TotalPlannedCompletion = dbContext.PMIS_PLAN_DETAILS.Where(x => x.IMS_PR_ROAD_CODE == IMS_PR_ROAD_CODE & x.IS_LATEST == "Y").Select(x => x.PLANNED_COMPLETION_DATE).Max();
                    ViewBag.TotalPlannedDuration = (EndDate - StartDate).Days;
                    Decimal? TotalCost = 0;

                    foreach (var item in latestRecord)
                    {
                        AddPlanPMISViewModelBridge objPmis = new AddPlanPMISViewModelBridge();

                        //objPmis.ACTIVITY_DESC = dbContext.PMIS_ACTIVITY_MASTER.Where(y => y.ACTIVITY_ID == item.ACTIVITY_ID).Select(c => c.ACTIVITY_DESC).FirstOrDefault();
                        objPmis.ACTIVITY_DESC = dbContext.PMIS_ACTIVITY_MASTER.Where(y => y.ACTIVITY_ID == item.ACTIVITY_ID && y.ROAD_TYPE == "L").Select(c => c.ACTIVITY_DESC).FirstOrDefault();
                        //objPmis.ACTIVITY_UNIT = dbContext.PMIS_ACTIVITY_MASTER.Where(y => y.ACTIVITY_ID == item.ACTIVITY_ID).Select(c => c.ACTIVITY_UNIT).FirstOrDefault();
                        objPmis.ACTIVITY_UNIT = dbContext.PMIS_ACTIVITY_MASTER.Where(y => y.ACTIVITY_ID == item.ACTIVITY_ID && y.ROAD_TYPE == "L").Select(c => c.ACTIVITY_UNIT).FirstOrDefault();
                        objPmis.QUANTITY = item.QUANTITY;
                        objPmis.AGREEMENT_COST = Math.Round(Convert.ToDecimal(item.AGREEMENT_COST), 2);
                        objPmis.PLANNED_START_DATE = item.PLANNED_START_DATE; // item.PLANNED_START_DATE.ToString().Split(' ')[0];
                        objPmis.PLANNED_DURATION = item.PLANNED_DURATION;
                        objPmis.PLANNED_COMPLETION_DATE = item.PLANNED_COMPLETION_DATE;

                        //objPmis.QUANTITY_APPL_U = dbContext.PMIS_ACTIVITY_MASTER.Where(y => y.ACTIVITY_ID == item.ACTIVITY_ID).Select(c => c.QUANTITY_APPL).FirstOrDefault();
                        objPmis.QUANTITY_APPL_U = dbContext.PMIS_ACTIVITY_MASTER.Where(y => y.ACTIVITY_ID == item.ACTIVITY_ID && y.ROAD_TYPE == "L").Select(c => c.QUANTITY_APPL).FirstOrDefault();
                        //objPmis.AGRCOST_APPL_U = dbContext.PMIS_ACTIVITY_MASTER.Where(y => y.ACTIVITY_ID == item.ACTIVITY_ID).Select(c => c.AGRCOST_APPL).FirstOrDefault();
                        objPmis.AGRCOST_APPL_U = dbContext.PMIS_ACTIVITY_MASTER.Where(y => y.ACTIVITY_ID == item.ACTIVITY_ID && y.ROAD_TYPE == "L").Select(c => c.AGRCOST_APPL).FirstOrDefault();
                        //objPmis.PLANNED_START_DATE_APPL_U = dbContext.PMIS_ACTIVITY_MASTER.Where(y => y.ACTIVITY_ID == item.ACTIVITY_ID).Select(c => c.PLANNED_START_DATE_APPL).FirstOrDefault();
                        objPmis.PLANNED_START_DATE_APPL_U = dbContext.PMIS_ACTIVITY_MASTER.Where(y => y.ACTIVITY_ID == item.ACTIVITY_ID && y.ROAD_TYPE == "L").Select(c => c.PLANNED_START_DATE_APPL).FirstOrDefault();
                        //objPmis.PLANNED_DURATION_APPL_U = dbContext.PMIS_ACTIVITY_MASTER.Where(y => y.ACTIVITY_ID == item.ACTIVITY_ID).Select(c => c.PLANNED_DURATION_APPL).FirstOrDefault();
                        objPmis.PLANNED_DURATION_APPL_U = dbContext.PMIS_ACTIVITY_MASTER.Where(y => y.ACTIVITY_ID == item.ACTIVITY_ID && y.ROAD_TYPE == "L").Select(c => c.PLANNED_DURATION_APPL).FirstOrDefault();
                        //objPmis.PLANNED_COMPLETION_DATE_APPL_U = dbContext.PMIS_ACTIVITY_MASTER.Where(y => y.ACTIVITY_ID == item.ACTIVITY_ID).Select(c => c.PLANNED_COMPLETION_DATE_APPL).FirstOrDefault();
                        objPmis.PLANNED_COMPLETION_DATE_APPL_U = dbContext.PMIS_ACTIVITY_MASTER.Where(y => y.ACTIVITY_ID == item.ACTIVITY_ID && y.ROAD_TYPE == "L").Select(c => c.PLANNED_COMPLETION_DATE_APPL).FirstOrDefault();
                        if (ReviseOperationStatus != null)
                        {
                            objPmis.ReviseStatus = ReviseOperationStatus;
                        }
                        else
                        {
                            objPmis.ReviseStatus = "U";
                        }
                        objlist.Add(objPmis);
                    }
                    foreach (var model in objlist)
                    {
                        if (model.AGREEMENT_COST != null)
                        {
                            TotalCost = TotalCost + model.AGREEMENT_COST;
                        }
                    }
                    ViewBag.TotalAgreementCost = TotalCost;

                }
                else
                {
                    return Json(new { Success = false, ErrorMessage = "Plan Details Not Found" }, JsonRequestBehavior.AllowGet);
                }

                return View("~/Areas/PMIS/Views/PMIS/UpdatePMISBridgeProjectPlanLayout.cshtml", objlist);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "UpdatePMISBridgeProjectPlanLayout()");
                return null;
            }
        }

        [HttpPost]
        [Audit]
        public ActionResult UpdatePMISBridgeProjectPlan(IEnumerable<AddPlanPMISViewModelBridge> formData)
        {
            PMISDAL objDAL = new PMISDAL();
            PMGSYEntities dbContext = new PMGSYEntities();
            try
            {
                ViewBag.Operation = "U";
                if (formData != null)
                {
                    decimal TotalAgreementCost = 0;
                    decimal AgreementValue = 0;
                    int roadCode = 0;
                    foreach (var item in formData)
                    {
                        if (item.AGREEMENT_COST != null)
                        {
                            TotalAgreementCost += Convert.ToDecimal(item.AGREEMENT_COST);
                        }

                        if (item.IMS_PR_ROAD_CODE > 0)
                        {
                            roadCode = item.IMS_PR_ROAD_CODE;
                        }
                        //decimal SanLength = dbContext.IMS_SANCTIONED_PROJECTS.Where(x => x.IMS_PR_ROAD_CODE == item.IMS_PR_ROAD_CODE).Select(x => x.IMS_PAV_LENGTH).FirstOrDefault();
                        decimal SanLength = Convert.ToDecimal(dbContext.IMS_SANCTIONED_PROJECTS.Where(x => x.IMS_PR_ROAD_CODE == item.IMS_PR_ROAD_CODE).Select(x => x.IMS_BRIDGE_LENGTH).FirstOrDefault());
                        //if (item.QUANTITY != null && item.QUANTITY >= SanLength)
                        //{
                        //    return Json(new { Success = false, ErrorMessage = "For each individual activity, Quantity entered should not be more than sanctioned length." });
                        //}
                        if ((item.ACTIVITY_DESC != null || item.ACTIVITY_DESC != ""))
                        {

                            if (item.ACTIVITY_DESC != "Miscellaneous")
                            {
                                if (item.ACTIVITY_DESC == "Resource Mobilization" || item.ACTIVITY_DESC == "Field Lab")
                                {
                                    if (item.PLANNED_START_DATE == null || item.PLANNED_COMPLETION_DATE == null)
                                    {
                                        return Json(new { Success = false, ErrorMessage = "Planned Start Date and Planned End date must be Entered for " + item.ACTIVITY_DESC });
                                    }

                                }

                                else if ((item.QUANTITY != 0 && item.QUANTITY != null) && item.AGREEMENT_COST == 0)
                                {
                                    return Json(new { Success = false, ErrorMessage = "Agreement Cost cannot be Zero for which Quantity entered is greater than zero" });
                                }

                            }

                        }


                        if (item.QUANTITY == null && item.AGREEMENT_COST > 0 && item.ACTIVITY_DESC != "Miscellaneous")
                        {
                            return Json(new { Success = false, ErrorMessage = "Agreement cost cannot be entered for activities for which quantity has not been entered." });
                        }
                        if (item.PLANNED_START_DATE != null && item.PLANNED_COMPLETION_DATE != null && item.PLANNED_START_DATE > item.PLANNED_COMPLETION_DATE)
                        {
                            return Json(new { Success = false, ErrorMessage = "Planned Completion date cannot be before the planned start date of an activity." });
                        }
                        if ((item.PLANNED_START_DATE == null || item.PLANNED_COMPLETION_DATE == null) && (item.QUANTITY > 0))
                        {
                            return Json(new { Success = false, ErrorMessage = "Planned Start and End Date should be entered for activities for which quantity has been entered." });
                        }

                    }
                    var Agreement_Id = dbContext.TEND_AGREEMENT_DETAIL.Where(x => x.IMS_PR_ROAD_CODE == roadCode && x.TEND_AGREEMENT_STATUS != "W").Max(x => x.TEND_AGREEMENT_ID);

                    AgreementValue = dbContext.TEND_AGREEMENT_DETAIL.Where(x => x.IMS_PR_ROAD_CODE == roadCode && x.TEND_AGREEMENT_ID == Agreement_Id).Select(x => x.TEND_AGREEMENT_AMOUNT + x.GST_AMT_MAINT_AGREEMENT_DLP).FirstOrDefault();
                    if (TotalAgreementCost != AgreementValue)
                    {
                        return Json(new { Success = false, ErrorMessage = "The Sum of Agreement cost of all activities should match the Agreement Value for this road" });
                    }

                    string Status = objDAL.UpdatePMISBridgeProjectPlanDAL(formData);
                    if (Status == string.Empty)
                        return Json(new { Success = true });
                    else
                        return Json(new { Success = false, ErrorMessage = Status });
                }
                else
                {
                    return Json(new { Success = false, ErrorMessage = "Update Unsuccessful" });
                }

            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "UpdatePMISBridgeProjectPlan(IEnumerable<AddPlanPMISViewModelBridge> formData)");
                throw ex;
            }
        }

        [HttpGet]
        [Audit]
        public ActionResult ViewPMISBridgeProjectPlanLayout(String parameter, String hash, String key)
        {
            string status = string.Empty;
            PMISDAL objDAL = new PMISDAL();
            PMGSYEntities dbContext = new PMGSYEntities();
            Dictionary<string, string> decryptedParameters = null;
            try
            {
                decryptedParameters = URLEncrypt.DecryptParameters1(new string[] { parameter, hash, key });
                int IMS_PR_ROAD_CODE = 0; string StateShare = String.Empty; string MordShare = String.Empty; string TotalSanctionedCost = String.Empty;
                decimal stateShare = 0; decimal mordShare = 0; decimal totalSancCost = 0;
                if (decryptedParameters.Count() > 0)
                {

                    IMS_PR_ROAD_CODE = Convert.ToInt32(decryptedParameters["RoadCode"].ToString());
                    stateShare = Math.Round(Convert.ToDecimal(decryptedParameters["StateShare"]), 2);
                    StateShare = stateShare.ToString();
                    mordShare = Math.Round(Convert.ToDecimal(decryptedParameters["MordShare"]), 2);
                    MordShare = mordShare.ToString();
                    totalSancCost = Math.Round(Convert.ToDecimal(decryptedParameters["TotalSanctionedDate"]), 2);
                    TotalSanctionedCost = totalSancCost.ToString();
                }
                else
                {
                    return Json(new { success = false, errorMessage = "Invalid Road Code." });
                }


                List<AddPlanPMISViewModelBridge> objlist = new List<AddPlanPMISViewModelBridge>();
                decimal? SumAgreementCost = 0;
                var viewlatest = dbContext.PMIS_PLAN_DETAILS.Where(x => x.IMS_PR_ROAD_CODE == IMS_PR_ROAD_CODE & x.IS_LATEST == "Y").ToList();
                if (viewlatest != null)
                {

                    foreach (var item in viewlatest)
                    {
                        AddPlanPMISViewModelBridge objPmis = new AddPlanPMISViewModelBridge();
                        objPmis.IMS_PR_ROAD_CODE = IMS_PR_ROAD_CODE;
                        objPmis.StateName = PMGSYSession.Current.StateName;
                        //objPmis.ACTIVITY_DESC = dbContext.PMIS_ACTIVITY_MASTER.Where(y => y.ACTIVITY_ID == item.ACTIVITY_ID).Select(c => c.ACTIVITY_DESC).FirstOrDefault();
                        objPmis.ACTIVITY_DESC = dbContext.PMIS_ACTIVITY_MASTER.Where(y => y.ACTIVITY_ID == item.ACTIVITY_ID && y.ROAD_TYPE == "L").Select(c => c.ACTIVITY_DESC).FirstOrDefault();
                        //objPmis.ACTIVITY_UNIT = dbContext.PMIS_ACTIVITY_MASTER.Where(y => y.ACTIVITY_ID == item.ACTIVITY_ID).Select(c => c.ACTIVITY_UNIT).FirstOrDefault();
                        objPmis.ACTIVITY_UNIT = dbContext.PMIS_ACTIVITY_MASTER.Where(y => y.ACTIVITY_ID == item.ACTIVITY_ID && y.ROAD_TYPE == "L").Select(c => c.ACTIVITY_UNIT).FirstOrDefault();
                        objPmis.QUANTITY = item.QUANTITY;
                        objPmis.AGREEMENT_COST = item.AGREEMENT_COST;
                        objPmis.PLANNED_START_DATE = item.PLANNED_START_DATE;
                        objPmis.View_Planned_Start_Date = item.PLANNED_START_DATE == null ? " " : item.PLANNED_START_DATE.Value.ToShortDateString(); //.Value.ToShortDateString();
                        objPmis.PLANNED_DURATION = item.PLANNED_DURATION;
                        objPmis.View_Planned_Completion_Date = item.PLANNED_COMPLETION_DATE == null ? " " : item.PLANNED_COMPLETION_DATE.Value.ToShortDateString();
                        objPmis.PLANNED_COMPLETION_DATE = item.PLANNED_COMPLETION_DATE;
                        if (objPmis.AGREEMENT_COST != null)
                            SumAgreementCost = SumAgreementCost + objPmis.AGREEMENT_COST;

                        objlist.Add(objPmis);
                    }

                    //ViewBag.RoadName = dbContext.IMS_SANCTIONED_PROJECTS.Where(x => x.IMS_PR_ROAD_CODE == IMS_PR_ROAD_CODE).Select(x => x.IMS_ROAD_NAME).FirstOrDefault();
                    ViewBag.BridgeName = dbContext.IMS_SANCTIONED_PROJECTS.Where(x => x.IMS_PR_ROAD_CODE == IMS_PR_ROAD_CODE).Select(x => x.IMS_BRIDGE_NAME).FirstOrDefault();
                    //ViewBag.SanctionedYear = dbContext.IMS_SANCTIONED_PROJECTS.Where(x => x.IMS_PR_ROAD_CODE == IMS_PR_ROAD_CODE).Select(x => x.IMS_YEAR).FirstOrDefault();
                    var year = dbContext.IMS_SANCTIONED_PROJECTS.Where(x => x.IMS_PR_ROAD_CODE == IMS_PR_ROAD_CODE).Select(x => x.IMS_YEAR).FirstOrDefault();
                    ViewBag.SanctionedYear = year.ToString() + "-" + (year + 1).ToString().Substring(2, 2);
                    //ViewBag.SanctionedLength = Math.Round(dbContext.IMS_SANCTIONED_PROJECTS.Where(x => x.IMS_PR_ROAD_CODE == IMS_PR_ROAD_CODE).Select(x => x.IMS_PAV_LENGTH).FirstOrDefault(), 2);
                    ViewBag.SanctionedLength = Math.Round(Convert.ToDecimal(dbContext.IMS_SANCTIONED_PROJECTS.Where(x => x.IMS_PR_ROAD_CODE == IMS_PR_ROAD_CODE).Select(x => x.IMS_BRIDGE_LENGTH).FirstOrDefault()), 2);
                    ViewBag.PackageNo = dbContext.IMS_SANCTIONED_PROJECTS.Where(x => x.IMS_PR_ROAD_CODE == IMS_PR_ROAD_CODE).Select(x => x.IMS_PACKAGE_ID).FirstOrDefault();
                    var Agreement_Id = dbContext.TEND_AGREEMENT_DETAIL.Where(x => x.IMS_PR_ROAD_CODE == IMS_PR_ROAD_CODE && x.TEND_AGREEMENT_STATUS != "W").Max(x => x.TEND_AGREEMENT_ID);
                    ViewBag.AgreementValue = dbContext.TEND_AGREEMENT_DETAIL.Where(x => x.IMS_PR_ROAD_CODE == IMS_PR_ROAD_CODE && x.TEND_AGREEMENT_ID == Agreement_Id).Select(x => x.TEND_AGREEMENT_AMOUNT + x.GST_AMT_MAINT_AGREEMENT_DLP).FirstOrDefault();
                    ViewBag.RoadPlanVersions = dbContext.PMIS_PLAN_MASTER.Where(x => x.IMS_PR_ROAD_CODE == IMS_PR_ROAD_CODE).Select(x => x.BASELINE_NO).ToList();
                    ViewBag.CurrentRoadPlaneVersion = dbContext.PMIS_PLAN_MASTER.Where(x => x.IMS_PR_ROAD_CODE == IMS_PR_ROAD_CODE & x.IS_LATEST == "Y").Select(x => x.BASELINE_NO).FirstOrDefault();
                    ViewBag.StateShare = StateShare;
                    ViewBag.MordShare = MordShare;
                    ViewBag.TotalSanctionedCost = TotalSanctionedCost;

                    ViewBag.TotalAgreementCost = SumAgreementCost;
                    DateTime StartDate = ViewBag.TotalPlannedStartDate = dbContext.PMIS_PLAN_DETAILS.Where(x => x.IMS_PR_ROAD_CODE == IMS_PR_ROAD_CODE & x.IS_LATEST == "Y").Select(x => x.PLANNED_START_DATE).Min();
                    DateTime EndDate = ViewBag.TotalPlannedCompletion = dbContext.PMIS_PLAN_DETAILS.Where(x => x.IMS_PR_ROAD_CODE == IMS_PR_ROAD_CODE & x.IS_LATEST == "Y").Select(x => x.PLANNED_COMPLETION_DATE).Max();
                    ViewBag.TotalPlannedDuration = (EndDate - StartDate).Days;
                }
                else
                {
                    return Json(new { Success = false, ErrorMessage = "Plan Details Not Found" }, JsonRequestBehavior.AllowGet);
                }

                return PartialView("~/Areas/PMIS/Views/PMIS/ViewPMISBridgeProjectPlanLayout.cshtml", objlist);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "ViewPMISBridgeProjectPlanLayout()");
                return null;
            }
        }

        [HttpGet]
        [Audit]
        public ActionResult ViewPMISBridgeProjectPlan(String BridgeName, String StateShare, String MordShare, String TotalSanctionedCost, String baseline)
        {
            string status = string.Empty;
            PMISDAL objDAL = new PMISDAL();
            PMGSYEntities dbContext = new PMGSYEntities();
            try
            {
                int RoadCode = 0;
                int BaseLine = 0;
                if (BridgeName != null)
                {
                    //RoadCode = dbContext.IMS_SANCTIONED_PROJECTS.Where(x => x.IMS_ROAD_NAME == RoadName).Select(x => x.IMS_PR_ROAD_CODE).FirstOrDefault();
                    RoadCode = dbContext.IMS_SANCTIONED_PROJECTS.Where(x => x.IMS_BRIDGE_NAME == BridgeName).Select(x => x.IMS_PR_ROAD_CODE).FirstOrDefault();
                    BaseLine = Convert.ToInt32(baseline);


                    List<AddPlanPMISViewModelBridge> objlist = new List<AddPlanPMISViewModelBridge>();
                    decimal? SumAgreementCost = 0;
                    var viewlatest = dbContext.PMIS_PLAN_DETAILS.Where(x => x.IMS_PR_ROAD_CODE == RoadCode & x.BASELINE_NO == BaseLine).ToList();
                    if (viewlatest != null)
                    {

                        foreach (var item in viewlatest)
                        {
                            AddPlanPMISViewModelBridge objPmis = new AddPlanPMISViewModelBridge();
                            objPmis.IMS_PR_ROAD_CODE = RoadCode;
                            objPmis.StateName = PMGSYSession.Current.StateName;
                            //objPmis.ACTIVITY_DESC = dbContext.PMIS_ACTIVITY_MASTER.Where(y => y.ACTIVITY_ID == item.ACTIVITY_ID).Select(c => c.ACTIVITY_DESC).FirstOrDefault();
                            objPmis.ACTIVITY_DESC = dbContext.PMIS_ACTIVITY_MASTER.Where(y => y.ACTIVITY_ID == item.ACTIVITY_ID && y.ROAD_TYPE == "L").Select(c => c.ACTIVITY_DESC).FirstOrDefault();
                            //objPmis.ACTIVITY_UNIT = dbContext.PMIS_ACTIVITY_MASTER.Where(y => y.ACTIVITY_ID == item.ACTIVITY_ID).Select(c => c.ACTIVITY_UNIT).FirstOrDefault();
                            objPmis.ACTIVITY_UNIT = dbContext.PMIS_ACTIVITY_MASTER.Where(y => y.ACTIVITY_ID == item.ACTIVITY_ID && y.ROAD_TYPE == "L").Select(c => c.ACTIVITY_UNIT).FirstOrDefault();
                            objPmis.QUANTITY = item.QUANTITY;
                            objPmis.AGREEMENT_COST = item.AGREEMENT_COST;
                            objPmis.PLANNED_START_DATE = item.PLANNED_START_DATE;
                            objPmis.View_Planned_Start_Date = item.PLANNED_START_DATE == null ? " " : item.PLANNED_START_DATE.Value.ToShortDateString();
                            objPmis.PLANNED_DURATION = item.PLANNED_DURATION;
                            objPmis.View_Planned_Completion_Date = item.PLANNED_COMPLETION_DATE == null ? " " : item.PLANNED_COMPLETION_DATE.Value.ToShortDateString();
                            objPmis.PLANNED_COMPLETION_DATE = item.PLANNED_COMPLETION_DATE;
                            if (objPmis.AGREEMENT_COST != null)
                                SumAgreementCost = SumAgreementCost + objPmis.AGREEMENT_COST;

                            objlist.Add(objPmis);
                        }

                        //ViewBag.RoadName = dbContext.IMS_SANCTIONED_PROJECTS.Where(x => x.IMS_PR_ROAD_CODE == RoadCode).Select(x => x.IMS_ROAD_NAME).FirstOrDefault();
                        ViewBag.BridgeName = dbContext.IMS_SANCTIONED_PROJECTS.Where(x => x.IMS_PR_ROAD_CODE == RoadCode).Select(x => x.IMS_BRIDGE_NAME).FirstOrDefault();
                        //ViewBag.SanctionedYear = dbContext.IMS_SANCTIONED_PROJECTS.Where(x => x.IMS_PR_ROAD_CODE == RoadCode).Select(x => x.IMS_YEAR).FirstOrDefault();
                        var year = dbContext.IMS_SANCTIONED_PROJECTS.Where(x => x.IMS_PR_ROAD_CODE == RoadCode).Select(x => x.IMS_YEAR).FirstOrDefault();
                        ViewBag.SanctionedYear = year.ToString() + "-" + (year + 1).ToString().Substring(2, 2);
                        //ViewBag.SanctionedLength = dbContext.IMS_SANCTIONED_PROJECTS.Where(x => x.IMS_PR_ROAD_CODE == RoadCode).Select(x => x.IMS_PAV_LENGTH).FirstOrDefault();
                        ViewBag.SanctionedLength = Math.Round(Convert.ToDecimal(dbContext.IMS_SANCTIONED_PROJECTS.Where(x => x.IMS_PR_ROAD_CODE == RoadCode).Select(x => x.IMS_BRIDGE_LENGTH).FirstOrDefault()), 2);
                        ViewBag.PackageNo = dbContext.IMS_SANCTIONED_PROJECTS.Where(x => x.IMS_PR_ROAD_CODE == RoadCode).Select(x => x.IMS_PACKAGE_ID).FirstOrDefault();
                        var Agreement_Id = dbContext.TEND_AGREEMENT_DETAIL.Where(x => x.IMS_PR_ROAD_CODE == RoadCode && x.TEND_AGREEMENT_STATUS != "W").Max(x => x.TEND_AGREEMENT_ID);

                        ViewBag.AgreementValue = dbContext.TEND_AGREEMENT_DETAIL.Where(x => x.IMS_PR_ROAD_CODE == RoadCode && x.TEND_AGREEMENT_ID == Agreement_Id).Select(x => x.TEND_AGREEMENT_AMOUNT + x.GST_AMT_MAINT_AGREEMENT_DLP).FirstOrDefault();
                        ViewBag.RoadPlanVersions = dbContext.PMIS_PLAN_MASTER.Where(x => x.IMS_PR_ROAD_CODE == RoadCode).Select(x => x.BASELINE_NO).ToList();
                        ViewBag.CurrentRoadPlaneVersion = BaseLine;
                        ViewBag.StateShare = StateShare;
                        ViewBag.MordShare = MordShare;
                        ViewBag.TotalSanctionedCost = TotalSanctionedCost;

                        ViewBag.TotalAgreementCost = SumAgreementCost;
                        DateTime StartDate = ViewBag.TotalPlannedStartDate = dbContext.PMIS_PLAN_DETAILS.Where(x => x.IMS_PR_ROAD_CODE == RoadCode & x.IS_LATEST == "Y").Select(x => x.PLANNED_START_DATE).Min();
                        DateTime EndDate = ViewBag.TotalPlannedCompletion = dbContext.PMIS_PLAN_DETAILS.Where(x => x.IMS_PR_ROAD_CODE == RoadCode & x.IS_LATEST == "Y").Select(x => x.PLANNED_COMPLETION_DATE).Max();
                        ViewBag.TotalPlannedDuration = (EndDate - StartDate).Days;
                    }
                    else
                    {
                        return Json(new { Success = false, ErrorMessage = "Plan Details Not Found" }, JsonRequestBehavior.AllowGet);
                    }

                    return PartialView("~/Areas/PMIS/Views/PMIS/ViewPMISBridgeProjectPlanLayout.cshtml", objlist);
                }
                else
                {
                    return Json(new { success = false, errorMessage = "Invalid Road Code." });
                }
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "ViewPMISBridgeProjectPlanLayout()");
                return null;
            }
        }

        #endregion

        #region Delete PMIS Project PLAN Bridge

        [HttpPost]
        [Audit]
        public JsonResult DeletePmisBridgeProjectPlan(String parameter, String hash, String key)
        {
            string status = string.Empty;
            PMISDAL objDAL = new PMISDAL();
            PMGSYEntities dbContext = new PMGSYEntities();
            Dictionary<string, string> decryptedParameters = null;
            try
            {
                decryptedParameters = URLEncrypt.DecryptParameters1(new string[] { parameter, hash, key });
                int IMS_PR_ROAD_CODE = 0;
                int PLAN_ID = 0;
                if (decryptedParameters.Count() > 0)
                {

                    IMS_PR_ROAD_CODE = Convert.ToInt32(decryptedParameters["RoadCode"].ToString());
                    PLAN_ID = Convert.ToInt32(decryptedParameters["PlanID"].ToString());

                }
                else
                {
                    return Json(new { success = false, errorMessage = "Invalid Road Code." });
                }

                status = objDAL.DeleteBridgeProjectPlanDAL(IMS_PR_ROAD_CODE, PLAN_ID);
                if (status == string.Empty)
                {
                    return Json(new { success = true });
                }
                else
                {
                    return Json(new { success = false, errorMessage = status });
                }

            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "DeletePmisBridgeProjectPlan()");
                return Json(new { success = false, errorMessage = status });
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

        #region Finalize PMIS Project PLAN Bridge

        [HttpPost]
        [Audit]
        public JsonResult FinalizePmisBridgeProjectPlan(String parameter, String hash, String key)
        {
            string status = string.Empty;
            PMISDAL objDAL = new PMISDAL();
            PMGSYEntities dbContext = new PMGSYEntities();
            Dictionary<string, string> decryptedParameters = null;
            try
            {
                decryptedParameters = URLEncrypt.DecryptParameters1(new string[] { parameter, hash, key });
                int IMS_PR_ROAD_CODE = 0;
                if (decryptedParameters.Count() > 0)
                {

                    IMS_PR_ROAD_CODE = Convert.ToInt32(decryptedParameters["RoadCode"].ToString());

                }
                else
                {
                    return Json(new { success = false, errorMessage = "Invalid Road Code." });
                }

                status = objDAL.FinalizeBridgeProjectPlanDAL(IMS_PR_ROAD_CODE);
                if (status == string.Empty)
                {
                    return Json(new { success = true });
                }
                else
                {
                    return Json(new { success = false, errorMessage = status });
                }
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "FinalizePmisBridgeProjectPlan()");
                return Json(new { success = false, errorMessage = status });
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

        #region Revise PMIS Project PLAN Bridge

        [HttpPost]
        [Audit]
        public ActionResult RevisePmisBridgeProjectPlan(String parameter, String hash, String key)
        {
            string status = string.Empty;
            PMISDAL objDAL = new PMISDAL();
            PMGSYEntities dbContext = new PMGSYEntities();
            Dictionary<string, string> decryptedParameters = null;
            try
            {
                decryptedParameters = URLEncrypt.DecryptParameters1(new string[] { parameter, hash, key });
                int IMS_PR_ROAD_CODE = 0;
                if (decryptedParameters.Count() > 0)
                {

                    IMS_PR_ROAD_CODE = Convert.ToInt32(decryptedParameters["RoadCode"].ToString());

                }
                else
                {
                    return Json(new { success = false, errorMessage = "Invalid Road Code." });
                    //return null;
                }

                status = objDAL.ReviseBridgeProjectPlanDAL(IMS_PR_ROAD_CODE);
                if (status == string.Empty)
                {
                    PMISController controllerobj = new PMISController();
                    // var updateobj = controllerobj.UpdatePMISRoadProjectPlanLayout(parameter, hash, key);
                    return Json(new { success = true });
                    //return null;
                }
                else
                {
                    //return null;
                    return Json(new { success = false, errorMessage = status });
                }
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "RevisePmisBridgeProjectPlan()");
                return Json(new { success = false, errorMessage = status });
                //return null;
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

        #region Add Bridge Actuals
        [HttpGet]
        [Audit]
        public ViewResult AddBridgeActuals(String parameter, String hash, String key)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            PMISDAL objDAL = new PMISDAL();
            Dictionary<string, string> decryptedParameters = null;

            string AprilMonthStartDay = ConfigurationManager.AppSettings["APRIL_MONTH_START_DAY"];   //1
            int AprilMonthStartDayValue = Convert.ToInt16(AprilMonthStartDay);

            string AprilMonthEndDay = ConfigurationManager.AppSettings["APRIL_MONTH_END_DAY"];   //5
            int AprilMonthEndDayValue = Convert.ToInt16(AprilMonthEndDay);

            string AprilMonth = ConfigurationManager.AppSettings["APRIL_MONTH"];  //4
            int AprilMonthValue = Convert.ToInt16(AprilMonth);

            DateTime FinanDate = DateTime.Now;
            int FinancialYear = FinanDate.Month >= 4 ? FinanDate.Year : FinanDate.Year;

            DateTime Conditional_Date_Value = new DateTime(FinancialYear, 3, 31, 00, 00, 00);
            DateTime CondEntryDate = new DateTime();
            Nullable<DateTime> tempDate = new DateTime();

            try
            {
               

                decryptedParameters = URLEncrypt.DecryptParameters1(new string[] { parameter, hash, key });
                List<AddActualsViewModel> objlist = new List<AddActualsViewModel>();
                decimal? SumAgreementCost = 0;
                int IMS_PR_ROAD_CODE = 0;
                string StateShare = String.Empty; string MordShare = String.Empty; string TotalSanctionedCost = String.Empty; string AggStartDate = String.Empty; string AggEndDate = String.Empty;
                decimal stateShare = 0; decimal mordShare = 0; decimal totalSancCost = 0;
                if (decryptedParameters.Count() > 0)
                {
                    IMS_PR_ROAD_CODE = Convert.ToInt32(decryptedParameters["RoadCode"].ToString());
                    stateShare = Math.Round(Convert.ToDecimal(decryptedParameters["StateShare"]), 2);
                    StateShare = stateShare.ToString();
                    mordShare = Math.Round(Convert.ToDecimal(decryptedParameters["MordShare"]), 2);
                    MordShare = mordShare.ToString();
                    totalSancCost = Math.Round(Convert.ToDecimal(decryptedParameters["TotalSanctionedDate"]), 2);
                    TotalSanctionedCost = totalSancCost.ToString();
                    //StateShare = decryptedParameters["StateShare"].ToString();
                    //MordShare = decryptedParameters["MordShare"].ToString();
                    //TotalSanctionedCost = decryptedParameters["TotalSanctionedDate"].ToString();
                    //AggStartDate = decryptedParameters["AggStartDate"].ToString();
                    //AggEndDate = decryptedParameters["AggEndDate"].ToString();
                }
                else
                {
                    return null;
                }
                if (IMS_PR_ROAD_CODE != 0)
                {
                    //if (dbContext.PMIS_PROGRESS_DETAILS.Where(x => x.IMS_PR_ROAD_CODE == IMS_PR_ROAD_CODE).Any())
                    //{
                    //    var latest_date = dbContext.PMIS_PROGRESS_DETAILS.Where(x => x.IMS_PR_ROAD_CODE == IMS_PR_ROAD_CODE).Select(x => x.ENTRY_DATE).Max();
                    //    var progress_Details = dbContext.PMIS_PROGRESS_DETAILS.Where(x => x.IMS_PR_ROAD_CODE == IMS_PR_ROAD_CODE & x.ENTRY_DATE == latest_date).ToList();
                    //    if (progress_Details != null)
                    //    { 

                    //    }
                    //}
                    //else
                    //{
                    var progress_details_entry = dbContext.PMIS_PROGRESS_DETAILS.Where(x => x.IMS_PR_ROAD_CODE == IMS_PR_ROAD_CODE).Any();
                    var viewlatest = dbContext.PMIS_PLAN_DETAILS.Where(x => x.IMS_PR_ROAD_CODE == IMS_PR_ROAD_CODE & x.IS_LATEST == "Y").ToList();
                    if (viewlatest != null)
                    {


                        foreach (var item in viewlatest)
                        {
                            AddActualsViewModel ActualsViewModelobj = new AddActualsViewModel();


                            ActualsViewModelobj.ACTIVITY_DESC = dbContext.PMIS_ACTIVITY_MASTER.Where(y => y.ACTIVITY_ID == item.ACTIVITY_ID).Select(c => c.ACTIVITY_DESC).FirstOrDefault();
                            ActualsViewModelobj.ACTIVITY_UNIT = dbContext.PMIS_ACTIVITY_MASTER.Where(y => y.ACTIVITY_ID == item.ACTIVITY_ID).Select(c => c.ACTIVITY_UNIT).FirstOrDefault();
                            ActualsViewModelobj.QUANTITY = item.QUANTITY;
                            ActualsViewModelobj.AGREEMENT_COST = item.AGREEMENT_COST;
                            ActualsViewModelobj.PLANNED_START_DATE = item.PLANNED_START_DATE; //.Value.ToShortDateString();

                            if (progress_details_entry)
                            {
                               

                                var latest_date = dbContext.PMIS_PROGRESS_DETAILS.Where(x => x.IMS_PR_ROAD_CODE == IMS_PR_ROAD_CODE).Select(x => x.ENTRY_DATE).Max();
                                var Date_Only = latest_date.Date;
                                var add_date = Date_Only.AddDays(1);
                                var progress_Details = dbContext.PMIS_PROGRESS_DETAILS.Where(x => x.IMS_PR_ROAD_CODE == IMS_PR_ROAD_CODE & x.ENTRY_DATE >= Date_Only & x.ENTRY_DATE < add_date & x.PLAN_ID == item.PLAN_ID & x.ACTIVITY_ID == item.ACTIVITY_ID & x.IS_LATEST == "Y").FirstOrDefault();
                                var progress_Master = dbContext.PMIS_PROGRESS_MASTER.Where(x => x.IMS_PR_ROAD_CODE == IMS_PR_ROAD_CODE & x.IS_LATEST == "Y").FirstOrDefault();
                                if (progress_Details != null && progress_Master != null)
                                {
                                    if (progress_Details.ENTRY_DATE != null)
                                    {
                                        tempDate = Convert.ToDateTime(progress_Details.ENTRY_DATE);

                                        CondEntryDate = Convert.ToDateTime((tempDate.Value.Day <= AprilMonthEndDayValue && tempDate.Value.Month == AprilMonthValue)
                                          ? Conditional_Date_Value : tempDate);
                                    }

                                    ActualsViewModelobj.ACTUAL_QUANTITY = progress_Details.PGS_QUANTITY;
                                    //var Act_StartDate = (progress_Details.ACTUAL_START_DATE ?? default(DateTime)).ToShortDateString();
                                    //var Act_FinshedDate = (progress_Details.ACTUAL_END_DATE ?? default(DateTime)).ToShortDateString();
                                    ActualsViewModelobj.STARTED_DATE = progress_Details.ACTUAL_START_DATE ?? null;  // default(DateTime);
                                    ActualsViewModelobj.FINISHED_DATE = progress_Details.ACTUAL_END_DATE ?? null;  // default(DateTime);

                                    TempData["CompletedRoadLength"] = progress_Master.COMPLETION_LENGTH;

                                    //TempData["Date_of_progress_entry"] = progress_Details.ENTRY_DATE != null ? DateTime.Now : progress_Details.ENTRY_DATE;

                                    // TempData["Date_of_progress_entry"] = CondEntryDate;

                                    TempData["Date_of_progress_entry"] = DateTime.Now;

                                    ActualsViewModelobj.ProjectStatus = Convert.ToChar(progress_Master.PROJECT_STATUS_);
                                    ViewBag.ProjectStatus = Convert.ToChar(progress_Master.PROJECT_STATUS_);
                                    //if (ActualsViewModelobj.STARTED_DATE == DateTime.MinValue || ActualsViewModelobj.FINISHED_DATE == DateTime.MinValue)
                                    //{
                                    //    ActualsViewModelobj.STARTED_DATE = DateTime.Now;
                                    //    ActualsViewModelobj.FINISHED_DATE = DateTime.Now;
                                    //}
                                }
                            }
                            //ActualsViewModel.STARTED = item.PLANNED_START_DATE == null ? "" : DateTime.Today.ToShortDateString();
                            //ActualsViewModel.FINISHED = item.PLANNED_START_DATE == null ? "" : DateTime.Today.ToShortDateString();
                            //if (item.PLANNED_START_DATE != null && !progress_details_entry)
                            //    {
                            //        ActualsViewModelobj.STARTED_DATE = DateTime.Now; // item.PLANNED_START_DATE == null ? null :
                            //        ActualsViewModelobj.FINISHED_DATE = DateTime.Now; //item.PLANNED_START_DATE == null ? null:
                            //    }

                            ActualsViewModelobj.PLANNED_COMPLETION_DATE = item.PLANNED_COMPLETION_DATE;
                            ActualsViewModelobj.QUANTITY_APPL = dbContext.PMIS_ACTIVITY_MASTER.Where(y => y.ACTIVITY_ID == item.ACTIVITY_ID).Select(c => c.QUANTITY_APPL).FirstOrDefault();
                            ActualsViewModelobj.AGRCOST_APPL = dbContext.PMIS_ACTIVITY_MASTER.Where(y => y.ACTIVITY_ID == item.ACTIVITY_ID).Select(c => c.AGRCOST_APPL).FirstOrDefault();
                            ActualsViewModelobj.PLANNED_START_DATE_APPL = dbContext.PMIS_ACTIVITY_MASTER.Where(y => y.ACTIVITY_ID == item.ACTIVITY_ID).Select(c => c.PLANNED_START_DATE_APPL).FirstOrDefault();
                            //ActualsViewModelobj.PLANNED_DURATION_APPL = dbContext.PMIS_ACTIVITY_MASTER.Where(y => y.ACTIVITY_ID == item.ACTIVITY_ID).Select(c => c.PLANNED_DURATION_APPL).FirstOrDefault();
                            ActualsViewModelobj.PLANNED_COMPLETION_DATE_APPL = dbContext.PMIS_ACTIVITY_MASTER.Where(y => y.ACTIVITY_ID == item.ACTIVITY_ID).Select(c => c.PLANNED_COMPLETION_DATE_APPL).FirstOrDefault();
                            if (ActualsViewModelobj.AGREEMENT_COST != null)
                                SumAgreementCost = SumAgreementCost + ActualsViewModelobj.AGREEMENT_COST;
                            objlist.Add(ActualsViewModelobj);
                        }
                        ViewBag.IMS_PR_ROAD_CODE = IMS_PR_ROAD_CODE;
                        ViewBag.RoadName = dbContext.IMS_SANCTIONED_PROJECTS.Where(x => x.IMS_PR_ROAD_CODE == IMS_PR_ROAD_CODE).Select(x => x.IMS_ROAD_NAME).FirstOrDefault();
                        //ViewBag.SanctionedYear = dbContext.IMS_SANCTIONED_PROJECTS.Where(x => x.IMS_PR_ROAD_CODE == IMS_PR_ROAD_CODE).Select(x => x.IMS_YEAR).FirstOrDefault();
                        var year = dbContext.IMS_SANCTIONED_PROJECTS.Where(x => x.IMS_PR_ROAD_CODE == IMS_PR_ROAD_CODE).Select(x => x.IMS_YEAR).FirstOrDefault();
                        ViewBag.SanctionedYear = year.ToString() + "-" + (year + 1).ToString().Substring(2, 2);
                        ViewBag.SanctionedLength = dbContext.IMS_SANCTIONED_PROJECTS.Where(x => x.IMS_PR_ROAD_CODE == IMS_PR_ROAD_CODE).Select(x => x.IMS_PAV_LENGTH).FirstOrDefault();
                        ViewBag.PackageNo = dbContext.IMS_SANCTIONED_PROJECTS.Where(x => x.IMS_PR_ROAD_CODE == IMS_PR_ROAD_CODE).Select(x => x.IMS_PACKAGE_ID).FirstOrDefault();
                        var Agreement_Id = dbContext.TEND_AGREEMENT_DETAIL.Where(x => x.IMS_PR_ROAD_CODE == IMS_PR_ROAD_CODE && x.TEND_AGREEMENT_STATUS != "W").Max(x => x.TEND_AGREEMENT_ID);

                        ViewBag.AgreementValue = dbContext.TEND_AGREEMENT_DETAIL.Where(x => x.IMS_PR_ROAD_CODE == IMS_PR_ROAD_CODE && x.TEND_AGREEMENT_ID == Agreement_Id).Select(x => x.TEND_AGREEMENT_AMOUNT + x.GST_AMT_MAINT_AGREEMENT_DLP).FirstOrDefault();
                        ViewBag.RoadPlanVersions = dbContext.PMIS_PLAN_MASTER.Where(x => x.IMS_PR_ROAD_CODE == IMS_PR_ROAD_CODE).Select(x => x.BASELINE_NO).ToList();
                        ViewBag.CurrentRoadPlaneVersion = dbContext.PMIS_PLAN_MASTER.Where(x => x.IMS_PR_ROAD_CODE == IMS_PR_ROAD_CODE & x.IS_LATEST == "Y").Select(x => x.BASELINE_NO).FirstOrDefault();
                        ViewBag.ProgressRecordedOn = DateTime.Today.ToShortDateString();
                        ViewBag.TotalAgreementCost = SumAgreementCost;
                        ViewBag.StateShare = StateShare;
                        ViewBag.MordShare = MordShare;
                        ViewBag.TotalSanctionedCost = TotalSanctionedCost;
                        ViewBag.TotalCompLength = dbContext.EXEC_LSB_MONTHLY_STATUS.Where(x => x.IMS_PR_ROAD_CODE == IMS_PR_ROAD_CODE).Max(x => x.EXEC_BRIDGE_COMPLETED);
                        DateTime StartDate = ViewBag.TotalPlannedStartDate = dbContext.PMIS_PLAN_DETAILS.Where(x => x.IMS_PR_ROAD_CODE == IMS_PR_ROAD_CODE & x.IS_LATEST == "Y").Select(x => x.PLANNED_START_DATE).Min();
                        DateTime EndDate = ViewBag.TotalPlannedCompletion = dbContext.PMIS_PLAN_DETAILS.Where(x => x.IMS_PR_ROAD_CODE == IMS_PR_ROAD_CODE & x.IS_LATEST == "Y").Select(x => x.PLANNED_COMPLETION_DATE).Max();
                        if (!progress_details_entry)
                        {
                            TempData["CompletedRoadLength"] = null;
                            TempData["Date_of_progress_entry"] = DateTime.Now;
                        }
                    }
                    else
                    {
                        return null;
                    }

                }
                return View("~/Areas/PMIS/Views/PMIS/AddBridgeActuals.cshtml", objlist);

            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "PMIS.AddBridgeActuals()");
                return null;
            }

        }

        [HttpPost]
        [Audit]
        public ActionResult SubmitBridgeActuals(IEnumerable<AddActualsViewModel> formData)
        {
            PMISDAL objDAL = new PMISDAL();
            PMGSYEntities dbContext = new PMGSYEntities();

            try
            {
                // changes made by Saurabh Jojare on 28-12-2021
                foreach (var item in formData)
                {

                    var ActivityValues = dbContext.PMIS_PROGRESS_DETAILS.Where(x => x.IMS_PR_ROAD_CODE == item.IMS_PR_ROAD_CODE && x.IS_LATEST == "Y").ToList();

                    foreach (var ActivityData in ActivityValues)
                    {
                        var ActivityName = dbContext.PMIS_ACTIVITY_MASTER.Where(x => x.ACTIVITY_ID == ActivityData.ACTIVITY_ID && x.ROAD_TYPE == "L").Select(x => x.ACTIVITY_DESC).FirstOrDefault();

                        if (item.ACTIVITY_DESC == ActivityName && item.ACTUAL_QUANTITY < ActivityData.PGS_QUANTITY)
                        {
                            return Json(new { Success = false, ErrorMessage = ActivityName + " executed quantity cannot be less than previous reported executed quantity" });

                        }   //if closes here
                    }    // inner for each closes here




                    decimal? QtyLimit = item.QUANTITY == null ? 0 : (item.QUANTITY * 110) / 100;

                    if ((item.ACTIVITY_DESC != null || item.ACTIVITY_DESC != ""))
                    {

                        if (item.ACTIVITY_DESC != "Miscellaneous")
                        {
                            if (item.ACTIVITY_DESC == "Resource Mobilization" || item.ACTIVITY_DESC == "Field Lab")
                            {
                                if (item.STARTED_DATE == null || item.FINISHED_DATE == null)
                                {
                                    return Json(new { Success = false, ErrorMessage = "Actual Start Date and Actual Completion date must be Entered for " + item.ACTIVITY_DESC });
                                }

                            }
                        }
                    }

                    if (item.ACTUAL_QUANTITY != null && item.ACTUAL_QUANTITY > QtyLimit)
                    {
                        if (item.QUANTITY == null && QtyLimit == 0)
                        {
                            return Json(new { Success = false, ErrorMessage = "Quantity executed as on date cannot be entered for the activities for which planned quantity has not been entered" });
                        }
                        return Json(new { Success = false, ErrorMessage = "Quantity executed as on date should not be more than 10% of the planned quantity" });
                    }
                    decimal SanLength = dbContext.IMS_SANCTIONED_PROJECTS.Where(x => x.IMS_PR_ROAD_CODE == item.IMS_PR_ROAD_CODE).Select(x => x.IMS_PAV_LENGTH).FirstOrDefault();
                    SanLength = (SanLength * 110) / 100;
                    decimal? TotalCompLength = dbContext.EXEC_LSB_MONTHLY_STATUS.Where(x => x.IMS_PR_ROAD_CODE == item.IMS_PR_ROAD_CODE).Max(x => x.EXEC_BRIDGE_COMPLETED);

                    //if (item.CompletedRoadLength != 0 && item.CompletedRoadLength > SanLength)
                    //{
                    //    return Json(new { Success = false, ErrorMessage = "Completed length should not be more than 10% of sanctioned length." });
                    //}
                    if (TotalCompLength != null && item.CompletedRoadLength != 0 && item.CompletedRoadLength < TotalCompLength)
                    {
                        return Json(new { Success = false, ErrorMessage = "Completed length should not be less than total completed length as on date." });
                    }
                    if (item.STARTED_DATE != null && item.FINISHED_DATE != null && item.STARTED_DATE > item.FINISHED_DATE)
                    {
                        return Json(new { Success = false, ErrorMessage = "Actual Completion date of an activity cannot be before actual start date ." });
                    }
                    if (item.STARTED_DATE == null && item.ACTUAL_QUANTITY != null)
                    {
                        return Json(new { Success = false, ErrorMessage = "Actual start date is mandatory for the activities for which actual quantity has been entered." });
                    }

                }        // outer for each closed here
                string Status = objDAL.SubmitBridgeActualsDAL(formData);
                if (Status == string.Empty)
                    return Json(new { Success = true });
                else
                    return Json(new { Success = false, ErrorMessage = Status });

            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "SubmitBridgeActuals(AddActualsViewModel formData))");
                throw ex;
            }

        }

        [HttpPost]
        [Audit]
        public ActionResult SaveBridgeActuals(IEnumerable<AddActualsViewModel> formData)
        {
            PMISDAL objDAL = new PMISDAL();
            PMGSYEntities dbContext = new PMGSYEntities();
            try
            {
                foreach (var item in formData)
                {
                    decimal? QtyLimit = item.QUANTITY == null ? 0 : (item.QUANTITY * 110) / 100;
                    if (item.ACTUAL_QUANTITY != null && item.ACTUAL_QUANTITY > QtyLimit)
                    {
                        if (item.QUANTITY == null && QtyLimit == 0)
                        {
                            return Json(new { Success = false, ErrorMessage = "Quantity executed as on date cannot be entered for the activities for which planned quantity has not been entered" });
                        }
                        return Json(new { Success = false, ErrorMessage = "Quantity executed as on date should not be more than 10% of the planned quantity" });
                    }
                    decimal SanLength = dbContext.IMS_SANCTIONED_PROJECTS.Where(x => x.IMS_PR_ROAD_CODE == item.IMS_PR_ROAD_CODE).Select(x => x.IMS_PAV_LENGTH).FirstOrDefault();
                    SanLength = (SanLength * 110) / 100;
                    decimal? TotalCompLength = dbContext.EXEC_LSB_MONTHLY_STATUS.Where(x => x.IMS_PR_ROAD_CODE == item.IMS_PR_ROAD_CODE).Max(x => x.EXEC_BRIDGE_COMPLETED);

                    // Changes Made by Saurabh Jojare based on new requirement on PMIS 28-12-2021
                    var ActivityValues = dbContext.PMIS_PROGRESS_DETAILS.Where(x => x.IMS_PR_ROAD_CODE == item.IMS_PR_ROAD_CODE && x.IS_LATEST == "Y").ToList();

                    foreach (var ActivityData in ActivityValues)
                    {
                        var ActivityName = dbContext.PMIS_ACTIVITY_MASTER.Where(x => x.ACTIVITY_ID == ActivityData.ACTIVITY_ID && x.ROAD_TYPE == "L").Select(x => x.ACTIVITY_DESC).FirstOrDefault();

                        if (item.ACTIVITY_DESC == ActivityName && item.ACTUAL_QUANTITY < ActivityData.PGS_QUANTITY)
                        {
                            return Json(new { Success = false, ErrorMessage = ActivityName + " executed quantity cannot be less than previous reported executed quantity" });

                        }

                    }
                    // added on 24-25-2022 below
                    if ((item.ACTIVITY_DESC != null || item.ACTIVITY_DESC != ""))
                    {

                        if (item.ACTIVITY_DESC != "Miscellaneous")
                        {
                            if (item.ACTIVITY_DESC == "Resource Mobilization" || item.ACTIVITY_DESC == "Field Lab")
                            {
                                if (item.STARTED_DATE == null || item.FINISHED_DATE == null)
                                {
                                    return Json(new { Success = false, ErrorMessage = "Actual Start Date and Actual Completion date must be Entered for " + item.ACTIVITY_DESC });
                                }

                            }
                        }
                    }

                    //if (item.CompletedRoadLength != 0 && item.CompletedRoadLength > SanLength)
                    //{
                    //    return Json(new { Success = false, ErrorMessage = "Completed length should not be more than 10% of sanctioned length." });
                    //}
                    if (TotalCompLength != null && item.CompletedRoadLength != 0 && item.CompletedRoadLength < TotalCompLength)
                    {
                        return Json(new { Success = false, ErrorMessage = "Completed length should not be less than total completed length as on date." });
                    }
                    if (item.STARTED_DATE != null && item.FINISHED_DATE != null && item.STARTED_DATE > item.FINISHED_DATE)
                    {
                        return Json(new { Success = false, ErrorMessage = "Actual Completion date of an activity cannot be before actual start date ." });
                    }
                    if (item.STARTED_DATE == null && item.ACTUAL_QUANTITY != null)
                    {
                        return Json(new { Success = false, ErrorMessage = "Actual start date is mandatory for the activities for which actual quantity has been entered." });
                    }
                }
                ViewBag.operation = "A";
                string Status = objDAL.SaveBridgeActualsDAL(formData);
                if (Status == string.Empty)
                    return Json(new { Success = true });
                else
                    return Json(new { Success = false, ErrorMessage = Status });



            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "SaveBridgeActuals(AddActualsViewModel formData))");
                throw ex;
            }
        }

        #endregion

        
        #endregion


    }
}
