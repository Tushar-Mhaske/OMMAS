using PMGSY.Areas.ProgressReport.DAL;
using PMGSY.Areas.ProgressReport.Models;
using PMGSY.Common;
using PMGSY.Controllers;
using PMGSY.Extensions;
using PMGSY.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PMGSY.Areas.ProgressReport.Controllers
{
    [RequiredAuthentication]
    [RequiredAuthorization]
    public class ProgressController : Controller
    {
        ProgressViewModel progressModel = null;
        CommonFunctions common = null;
        //
        // GET: /ProgressReport/Progress/
        public ProgressController()
        {
            common = new CommonFunctions();

        }

        #region Progress Photographs
        public ActionResult AppPhotoLayout()
        {

            progressModel = new ProgressViewModel
            {
                StateList = common.PopulateStates(true),
                DistrictList = common.PopulateDistrict(PMGSYSession.Current.StateCode, true),
                MonthList = common.PopulateMonths(true),
                YearList = common.PopulateYears(true),
                Level = PMGSYSession.Current.LevelId,
                PhotoType = 1,
                StateCode = PMGSYSession.Current.StateCode,
                DistrictCode = PMGSYSession.Current.DistrictCode
            };

            progressModel.StateList[0].Text = "All State";
            progressModel.DistrictList[0].Text = "All District";
            progressModel.DistrictList[0].Value = "0";
            return View(progressModel);
        }

        public ActionResult LabPhotoLayout()
        {
            progressModel = new ProgressViewModel
            {
                StateList = common.PopulateStates(true),
                DistrictList = common.PopulateDistrict(PMGSYSession.Current.StateCode),
                MonthList = common.PopulateMonths(true),
                YearList = common.PopulateYears(true),
                Level = PMGSYSession.Current.LevelId,
                PhotoType = 2,
                StateCode = PMGSYSession.Current.StateCode,
                DistrictCode = PMGSYSession.Current.DistrictCode
            };
            progressModel.StateList[0].Text = "All State";
            progressModel.DistrictList[0].Text = "All District";
            //progressModel.DistrictList.Find(x=>x.Value == "-1").Value = "0";
            return View(progressModel);
        }

        public ActionResult AppPhotoReport(ProgressViewModel progress)
        {

            return View(progress);
        }
        #endregion
        #region Common
        /// <summary>
        /// Get the List of Districts
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        //[Audit]
        public JsonResult GetDistricts()
        {
            try
            {
                CommonFunctions objCommonFunctions = new CommonFunctions();
                int MAST_STATE_CODE = Convert.ToInt32(Request.Params["MAST_STATE_CODE"]);
                if (PMGSYSession.Current.RoleCode == 25)
                {
                    List<SelectListItem> lst = objCommonFunctions.PopulateDistrict(MAST_STATE_CODE, true);
                    lst.Find(x => x.Value == "-1").Value = "0";
                    return Json(lst);
                }
                else if (PMGSYSession.Current.RoleCode == 3)
                {
                    return Json(objCommonFunctions.PopulateDistrictsOfTA(MAST_STATE_CODE, true));
                }
                else
                {
                    return Json(objCommonFunctions.PopulateDistrict(MAST_STATE_CODE, false));
                }
            }
            catch
            {
                return Json(new { string.Empty });
            }
        }

        #endregion

        #region Execution
        public ActionResult ExecutionDetails(string id)
        {
            try
            {
                ProgressDAL objDAL = new ProgressDAL();
                ExecutionMonitoringDetails model = new ExecutionMonitoringDetails();

                int proposalCode = Convert.ToInt32(id);

                ViewBag.FileDetails = objDAL.GetFileDetails(proposalCode);
                ViewBag.IsLatLongAvailable = objDAL.IsLatLongAvailable(proposalCode);
                ViewBag.IsMarkerAvailable = objDAL.GetMarkerDetails(proposalCode);
                model = objDAL.GetProposalDetails(proposalCode);
                model.ProposalCode = proposalCode;
                return View(model);
            }
            catch (Exception ex)
            {
                //using (System.IO.StreamWriter sw = System.IO.File.AppendText(System.IO.Path.Combine("D:\\ErrorLog\\OMMASErrorLog_" + System.DateTime.Now.Date.ToString("dd_MM_yyyy") + ".txt", "")))
                //{
                //    sw.WriteLine("Date :" + DateTime.Now.ToString());
                //    sw.WriteLine("Method : " + "ExecutionMonitoring.ExecutionDetails()");
                //    if (ex != null)
                //        sw.WriteLine("Exception : " + ex.ToString());
                //    if (ex.InnerException != null)
                //        sw.WriteLine("innerException : " + ex.InnerException.ToString());
                //    if (ex.InnerException.InnerException != null)
                //        sw.WriteLine("httpException : " + ex.InnerException.InnerException.ToString());
                //    sw.WriteLine("---------------------------------------------------------------------------------------");
                //    sw.Close();
                //}

                return null;
            }
        }

        public ActionResult GetLatLong()
        {
            ProgressDAL objDAL = new ProgressDAL();
            try
            {
                int proposalId = Convert.ToInt32(Request.Params["proposalId"]);
                return Json(new { Success = true, Message = objDAL.GetLatLongDAL(proposalId) }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                //using (System.IO.StreamWriter sw = System.IO.File.AppendText(System.IO.Path.Combine("D:\\ErrorLog\\OMMASErrorLog_" + System.DateTime.Now.Date.ToString("dd_MM_yyyy") + ".txt", "")))
                //{
                //    sw.WriteLine("Date :" + DateTime.Now.ToString());
                //    sw.WriteLine("Method : " + "ExecutionMonitoring.GetLatLong()");
                //    if (ex != null)
                //        sw.WriteLine("Exception : " + ex.ToString());
                //    if (ex.InnerException != null)
                //        sw.WriteLine("innerException : " + ex.InnerException.ToString());
                //    if (ex.InnerException.InnerException != null)
                //        sw.WriteLine("httpException : " + ex.InnerException.InnerException.ToString());
                //    sw.WriteLine("---------------------------------------------------------------------------------------");
                //    sw.Close();
                //}

                return null;
            }
        }

        /// <summary>
        /// returns the start, end longitude and latitude
        /// </summary>
        /// <returns></returns>
        public ActionResult GetStartEndLatLong()
        {
            ProgressDAL objDAL = new ProgressDAL();
            try
            {
                int obsId = Convert.ToInt32(Request.Params["obsId"]);
                return Json(new { Success = true, Message = objDAL.GetStartEndLatLong(obsId) }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return null;
            }
        }
        #endregion

        #region Execution
        public ActionResult LabDetails(string id)
        {
            try
            {
                ProgressDAL objDAL = new ProgressDAL();
                ExecutionMonitoringDetails model = new ExecutionMonitoringDetails();

                int labCode = Convert.ToInt32(id);

                ViewBag.FileDetails = objDAL.GetFileDetailsLAB(labCode);
                ViewBag.IsLatLongAvailable = objDAL.IsLatLongAvailableLAB(labCode);
                ViewBag.IsMarkerAvailable = objDAL.GetMarkerDetailsLAB(labCode);
                model = objDAL.GetLABDetails(labCode);
                model.LabCode = labCode;
                return View(model);
            }
            catch (Exception ex)
            {
                //using (System.IO.StreamWriter sw = System.IO.File.AppendText(System.IO.Path.Combine("D:\\ErrorLog\\OMMASErrorLog_" + System.DateTime.Now.Date.ToString("dd_MM_yyyy") + ".txt", "")))
                //{
                //    sw.WriteLine("Date :" + DateTime.Now.ToString());
                //    sw.WriteLine("Method : " + "ExecutionMonitoring.LabDetails()");
                //    if (ex != null)
                //        sw.WriteLine("Exception : " + ex.ToString());
                //    if (ex.InnerException != null)
                //        sw.WriteLine("innerException : " + ex.InnerException.ToString());
                //    if (ex.InnerException.InnerException != null)
                //        sw.WriteLine("httpException : " + ex.InnerException.InnerException.ToString());
                //    sw.WriteLine("---------------------------------------------------------------------------------------");
                //    sw.Close();
                //}

                return null;
            }
        }

        public ActionResult GetLatLongLAB()
        {
            ProgressDAL objDAL = new ProgressDAL();
            try
            {
                int labCode = Convert.ToInt32(Request.Params["labId"]);
                return Json(new { Success = true, Message = objDAL.GetLatLongLABDAL(labCode) }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                //using (System.IO.StreamWriter sw = System.IO.File.AppendText(System.IO.Path.Combine("D:\\ErrorLog\\OMMASErrorLog_" + System.DateTime.Now.Date.ToString("dd_MM_yyyy") + ".txt", "")))
                //{
                //    sw.WriteLine("Date :" + DateTime.Now.ToString());
                //    sw.WriteLine("Method : " + "ExecutionMonitoring.GetLatLong()");
                //    if (ex != null)
                //        sw.WriteLine("Exception : " + ex.ToString());
                //    if (ex.InnerException != null)
                //        sw.WriteLine("innerException : " + ex.InnerException.ToString());
                //    if (ex.InnerException.InnerException != null)
                //        sw.WriteLine("httpException : " + ex.InnerException.InnerException.ToString());
                //    sw.WriteLine("---------------------------------------------------------------------------------------");
                //    sw.Close();
                //}

                return null;
            }
        }
        #endregion

        #region Outcome Target and Achievement

        [HttpPost]
        public JsonResult PopulateDistricts(int stateCode)
        {
            try
            {
                CommonFunctions comm = new CommonFunctions();
                List<SelectListItem> lst = comm.PopulateDistrict(stateCode, true);
                lst.Find(x => x.Value == "-1").Value = "0";
                return Json(lst);
            }
            catch
            {
                return Json(new { string.Empty });
            }
        }

        [HttpPost]
        public JsonResult PopulateBlocks(int distCode)
        {
            try
            {
                CommonFunctions comm = new CommonFunctions();
                List<SelectListItem> lst = comm.PopulateBlocks(distCode, true);
                lst.Find(x => x.Value == "-1").Value = "0";
                return Json(lst);
            }
            catch
            {
                return Json(new { string.Empty });
            }
        }

        public ActionResult MPRTargetLayout()
        {
            MPRTargetModel mpr1 = new MPRTargetModel();
            CommonFunctions commonFunctions = new CommonFunctions();
            PMGSYEntities dbContext = new PMGSYEntities();

            mpr1.YearList = commonFunctions.PopulateYears(System.DateTime.Now.Year, false);
            mpr1.MonthList = commonFunctions.PopulateMonths(System.DateTime.Now.Month, false);

            mpr1.YearList.RemoveAt(0);
            mpr1.MonthList.RemoveAt(0);

            mpr1.Month = System.DateTime.Now.Month;
            mpr1.Year = System.DateTime.Now.Year;

            mpr1.CollabList = commonFunctions.PopulateFundingAgency(true);
            mpr1.CollabList.RemoveAt(0);
            mpr1.CollabList.Insert(0, new SelectListItem { Value = "0", Text = "All Collaborations" });

            #region Filtering Logic
            mpr1.StateName = PMGSYSession.Current.StateCode == 0 ? "0" : PMGSYSession.Current.StateName.Trim();
            mpr1.Mast_State_Code = PMGSYSession.Current.StateCode;

            mpr1.StateList = commonFunctions.PopulateStates(true);
            mpr1.StateCode = PMGSYSession.Current.StateCode == 0 ? 0 : PMGSYSession.Current.StateCode;
            mpr1.StateList.Find(x => x.Value == mpr1.StateCode.ToString()).Selected = true;
            mpr1.StateList.Find(x => x.Value == "0").Text = "All States";
            if (PMGSYSession.Current.StateCode == 0)
            {
                mpr1.DistrictList = new List<SelectListItem>();
                mpr1.DistrictList.Insert(0, new SelectListItem { Value = "0", Text = "All Districts" });
            }
            else
            {
                mpr1.DistrictList = commonFunctions.PopulateDistrict(PMGSYSession.Current.StateCode, true);
                mpr1.DistrictList.Find(x => x.Value == "-1").Value = "0";
            }

            mpr1.BlockList = new List<SelectListItem>();
            mpr1.BlockList.Insert(0, new SelectListItem { Value = "0", Text = "All Blocks" });
            #endregion

            mpr1.localizedValue = "en";

            return View(mpr1);
        }

        public ActionResult MPRTargetReport(MPRTargetModel mpr1)
        {
            CommonFunctions commonFunctions = new CommonFunctions();
            try
            {
                if (ModelState.IsValid)
                {
                    mpr1.Mast_State_Code = mpr1.StateCode > 0 ? mpr1.StateCode : mpr1.Mast_State_Code;
                    mpr1.StateName = mpr1.StateCode == 0 ? "All States" : PMGSYSession.Current.StateName.Trim();
                    //mpr1.DistName = mpr1.DistrictCode == 0 ? "0" : mpr1.DistName.Trim();
                    //mpr1.BlockName = mpr1.CollabCode == 0 ? "0" : mpr1.BlockName.Trim();
                    return View(mpr1);
                }
                else
                {
                    string message = "";
                    //    bool flag = false;
                    message = ModelState.Values.Where(x => x.Errors.Count == 1).FirstOrDefault().Errors[0].ErrorMessage;

                    mpr1.YearList = commonFunctions.PopulateYears(System.DateTime.Now.Year, false);
                    mpr1.MonthList = commonFunctions.PopulateMonths(System.DateTime.Now.Month, false);

                    mpr1.YearList.RemoveAt(0);
                    mpr1.MonthList.RemoveAt(0);

                    mpr1.Month = System.DateTime.Now.Month;
                    mpr1.Year = System.DateTime.Now.Year;

                    mpr1.CollabList = commonFunctions.PopulateFundingAgency(true);
                    return View("MPRTargetLayout", mpr1);
                }
            }
            catch
            {
                return View(mpr1);
            }
            //return View();
        }
        #endregion

        #region Monthly Achievement
        public ActionResult MonthlyTargetLayout()
        {
            MPRTargetModel mpr1 = new MPRTargetModel();
            CommonFunctions commonFunctions = new CommonFunctions();
            PMGSYEntities dbContext = new PMGSYEntities();

            mpr1.YearList = commonFunctions.PopulateFinancialYear(false, false).ToList();
            mpr1.MonthList = commonFunctions.PopulateMonths(System.DateTime.Now.Month, false);

            mpr1.YearList.RemoveAt(0);
            mpr1.MonthList.RemoveAt(0);

            mpr1.Month = System.DateTime.Now.Month;
            mpr1.Year = System.DateTime.Now.Year;

            mpr1.CollabList = commonFunctions.PopulateFundingAgency(true);
            mpr1.CollabList.RemoveAt(0);
            mpr1.CollabList.Insert(0, new SelectListItem { Value = "0", Text = "All Collaborations" });

            #region Filtering Logic
            mpr1.StateName = PMGSYSession.Current.StateCode == 0 ? "0" : PMGSYSession.Current.StateName.Trim();
            mpr1.Mast_State_Code = PMGSYSession.Current.StateCode;

            mpr1.StateList = commonFunctions.PopulateStates(true);
            mpr1.StateCode = PMGSYSession.Current.StateCode == 0 ? 0 : PMGSYSession.Current.StateCode;
            mpr1.StateList.Find(x => x.Value == mpr1.StateCode.ToString()).Selected = true;
            mpr1.StateList.Find(x => x.Value == "0").Text = "All States";
            if (PMGSYSession.Current.StateCode == 0)
            {
                mpr1.DistrictList = new List<SelectListItem>();
                mpr1.DistrictList.Insert(0, new SelectListItem { Value = "0", Text = "All Districts" });
            }
            else
            {
                mpr1.DistrictList = commonFunctions.PopulateDistrict(PMGSYSession.Current.StateCode, true);
                mpr1.DistrictList.Find(x => x.Value == "-1").Value = "0";
            }

            mpr1.BlockList = new List<SelectListItem>();
            mpr1.BlockList.Insert(0, new SelectListItem { Value = "0", Text = "All Blocks" });
            #endregion

            mpr1.localizedValue = "en";

            return View(mpr1);
        }

        public ActionResult MonthlyTargetReport(MPRTargetModel mpr1)
        {
            CommonFunctions commonFunctions = new CommonFunctions();
            try
            {
                if (ModelState.IsValid)
                {
                    mpr1.Mast_State_Code = mpr1.StateCode > 0 ? mpr1.StateCode : mpr1.Mast_State_Code;
                    mpr1.StateName = mpr1.StateCode == 0 ? "All States" : PMGSYSession.Current.StateName.Trim();
                    //mpr1.DistName = mpr1.DistrictCode == 0 ? "0" : mpr1.DistName.Trim();
                    //mpr1.BlockName = mpr1.CollabCode == 0 ? "0" : mpr1.BlockName.Trim();
                    return View(mpr1);
                }
                else
                {
                    string message = "";
                    //    bool flag = false;
                    message = ModelState.Values.Where(x => x.Errors.Count == 1).FirstOrDefault().Errors[0].ErrorMessage;

                    mpr1.YearList = commonFunctions.PopulateFinancialYear(false, false).ToList();
                    mpr1.MonthList = commonFunctions.PopulateMonths(System.DateTime.Now.Month, false);

                    mpr1.YearList.RemoveAt(0);
                    mpr1.MonthList.RemoveAt(0);

                    mpr1.Month = System.DateTime.Now.Month;
                    mpr1.Year = System.DateTime.Now.Year;

                    mpr1.CollabList = commonFunctions.PopulateFundingAgency(true);
                    return View("MPRTargetLayout", mpr1);
                }
            }
            catch
            {
                return View(mpr1);
            }
            //return View();
        }
        #endregion

        #region Achievement by State
        public ActionResult AchievementStateLayout()
        {
            MPRTargetModel mpr1 = new MPRTargetModel();
            CommonFunctions commonFunctions = new CommonFunctions();
            PMGSYEntities dbContext = new PMGSYEntities();

            mpr1.YearList = commonFunctions.PopulateFinancialYear(false, false).ToList();

            int count = mpr1.YearList.IndexOf(mpr1.YearList.Find(c => c.Value == "2015"));
            mpr1.YearList.RemoveRange(mpr1.YearList.IndexOf(mpr1.YearList.Find(c => c.Value == "2015")), mpr1.YearList.Count - count);
            if (DateTime.Now.Month <= 3)
            {
                mpr1.YearList.Remove(mpr1.YearList.Find(x => x.Value == DateTime.Now.Year.ToString()));
            }
            mpr1.MonthList = commonFunctions.PopulateMonths(System.DateTime.Now.Month, false);

            mpr1.YearList.RemoveAt(0);
            mpr1.MonthList.RemoveAt(0);

            mpr1.Month = System.DateTime.Now.Month;
            mpr1.Year = System.DateTime.Now.Year;

            mpr1.CollabList = commonFunctions.PopulateFundingAgency(true);
            mpr1.CollabList.RemoveAt(0);
            mpr1.CollabList.Insert(0, new SelectListItem { Value = "0", Text = "All Collaborations" });

            #region Filtering Logic
            mpr1.StateName = PMGSYSession.Current.StateCode == 0 ? "0" : PMGSYSession.Current.StateName.Trim();
            mpr1.Mast_State_Code = PMGSYSession.Current.StateCode;

            mpr1.StateList = commonFunctions.PopulateStates(true);
            mpr1.StateCode = PMGSYSession.Current.StateCode == 0 ? 0 : PMGSYSession.Current.StateCode;
            mpr1.StateList.Find(x => x.Value == mpr1.StateCode.ToString()).Selected = true;
            mpr1.StateList.Find(x => x.Value == "0").Text = "All States";
            if (PMGSYSession.Current.StateCode == 0)
            {
                mpr1.DistrictList = new List<SelectListItem>();
                mpr1.DistrictList.Insert(0, new SelectListItem { Value = "0", Text = "All Districts" });
            }
            else
            {
                mpr1.DistrictList = commonFunctions.PopulateDistrict(PMGSYSession.Current.StateCode, true);
                mpr1.DistrictList.Find(x => x.Value == "-1").Value = "0";
            }

            mpr1.BlockList = new List<SelectListItem>();
            mpr1.BlockList.Insert(0, new SelectListItem { Value = "0", Text = "All Blocks" });
            #endregion

            mpr1.localizedValue = "en";

            return View(mpr1);
        }

        public ActionResult AchievementStateReport(MPRTargetModel mpr1)
        {
            CommonFunctions commonFunctions = new CommonFunctions();
            try
            {
                if (ModelState.IsValid)
                {
                    mpr1.Mast_State_Code = mpr1.StateCode > 0 ? mpr1.StateCode : mpr1.Mast_State_Code;
                    mpr1.StateName = mpr1.StateCode == 0 ? "All States" : PMGSYSession.Current.StateName.Trim();
                    //mpr1.DistName = mpr1.DistrictCode == 0 ? "0" : mpr1.DistName.Trim();
                    //mpr1.BlockName = mpr1.CollabCode == 0 ? "0" : mpr1.BlockName.Trim();
                    return View(mpr1);
                }
                else
                {
                    string message = "";
                    //    bool flag = false;
                    message = ModelState.Values.Where(x => x.Errors.Count == 1).FirstOrDefault().Errors[0].ErrorMessage;

                    mpr1.YearList = commonFunctions.PopulateFinancialYear(false, false).ToList();
                    mpr1.MonthList = commonFunctions.PopulateMonths(System.DateTime.Now.Month, false);

                    mpr1.YearList.RemoveAt(0);
                    mpr1.MonthList.RemoveAt(0);

                    mpr1.Month = System.DateTime.Now.Month;
                    mpr1.Year = System.DateTime.Now.Year;

                    mpr1.CollabList = commonFunctions.PopulateFundingAgency(true);
                    return View("MPRTargetLayout", mpr1);
                }
            }
            catch
            {
                return View(mpr1);
            }
            //return View();
        }
        #endregion

        #region Target Achievement

        public ActionResult TargetAchievementLayout()
        {
            TargetAchievementViewModel model = new TargetAchievementViewModel();
            CommonFunctions comm = new CommonFunctions();
            try
            {
                model.fromDate = comm.GetDateTimeToString(DateTime.Now.AddDays(-1));
                model.toDate = comm.GetDateTimeToString(DateTime.Now.AddDays(-1));

                return View(model);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "Areas.ProgressReport.TargetAchievementLayout()");
                return null;
            }
        }

        public ActionResult TargetAchievementReport(TargetAchievementViewModel model)
        {
            CommonFunctions comm = new CommonFunctions();
            try
            {
                string[] arrfromdate = model.fromDate.Split('/');
                string[] arrtodate = model.toDate.Split('/');
                //if (comm.GetStringToDateTime(model.fromDate) > comm.GetStringToDateTime(model.toDate))
                if (arrfromdate[1] != arrtodate[1])
                {
                    return Json(new { success = false, message = "Please select dates for same month." });
                }
                if (comm.GetStringToDateTime(model.fromDate) > comm.GetStringToDateTime(model.toDate))
                {
                    return Json(new { success = false, message = "To Date should be greater than From Date." });
                }
                return View(model);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "Areas.ProgressReport.TargetAchievementReport()");
                return null;
            }
        }
        #endregion

        #region New Aspirational Lab Status Report 26 April 2018 Added By Rohit J.

        public ActionResult AddStatusLayout()
        {
            try
            {
                progressModel = new ProgressViewModel
                {
                    StateList = common.PopulateStates(true),
                    DistrictList = common.PopulateDistrict(PMGSYSession.Current.StateCode, true),
                    MonthList = common.PopulateMonths(true),
                    YearList = common.PopulateYears(true),
                    Level = PMGSYSession.Current.LevelId,
                    PhotoType = 1,
                    StateCode = PMGSYSession.Current.StateCode,
                    DistrictCode = PMGSYSession.Current.DistrictCode
                };
                progressModel.StateList[0].Text = "All State";
                progressModel.DistrictList[0].Text = "All District";
                progressModel.DistrictList[0].Value = "0";
                return View(progressModel);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "ProgressReport/Progress/AddStatusLayout()");
                return null;
            }

        }
        //

        public ActionResult AddStatusReport(ProgressViewModel progress)
        {
            try
            {
                return View(progress);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "ProgressReport/Progress/AddStatusReport()");
                return null;
            }
        }

        #endregion

        #region Physical Progress of Work
        /// <summary>
        /// Layout for Physical Progress Of Work Report
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult PhysicalProgressWorkLayout()
        {
            PhysicalProgressWorkModel physicalProgressWorkModel = new PhysicalProgressWorkModel();
            CommonFunctions commonFunctions = new CommonFunctions();
            try
            {

                physicalProgressWorkModel.StateName = PMGSYSession.Current.StateCode == 0 ? "0" : PMGSYSession.Current.StateName.Trim();
                physicalProgressWorkModel.DistName = PMGSYSession.Current.DistrictCode == 0 ? "0" : PMGSYSession.Current.DistrictName.Trim();
                physicalProgressWorkModel.BlockName = "0";//PMGSYSession.Current.BlockCode == 0 ? "0" : PMGSYSession.Current.BlockName.Trim();
                physicalProgressWorkModel.Mast_State_Code = PMGSYSession.Current.StateCode;
                physicalProgressWorkModel.Mast_District_Code = PMGSYSession.Current.DistrictCode;
                physicalProgressWorkModel.Mast_Block_Code = 0;//PMGSYSession.Current.BlockCode;
                physicalProgressWorkModel.LevelCode = PMGSYSession.Current.DistrictCode > 0 ? 2 : 1;

                if (PMGSYSession.Current.StateCode == 0)
                {
                    physicalProgressWorkModel.StateList = commonFunctions.PopulateStates(true);
                    physicalProgressWorkModel.StateList.Find(x => x.Value == "0").Value = "-1";
                    physicalProgressWorkModel.StateCode = PMGSYSession.Current.StateCode == 0 ? -1 : PMGSYSession.Current.StateCode;
                    physicalProgressWorkModel.StateList.Find(x => x.Value == physicalProgressWorkModel.StateCode.ToString()).Selected = true;
                }
                else
                {
                    physicalProgressWorkModel.StateList = new List<SelectListItem>();
                    physicalProgressWorkModel.StateList.Insert(0, (new SelectListItem { Text = PMGSYSession.Current.StateName, Value = PMGSYSession.Current.StateCode.ToString(), Selected = true }));
                }
                physicalProgressWorkModel.DistrictList = new List<SelectListItem>();
                if (physicalProgressWorkModel.StateCode == -1)
                {
                    physicalProgressWorkModel.DistrictList.Insert(0, (new SelectListItem { Text = "All Districts", Value = "0", Selected = true }));
                }
                else
                {
                    physicalProgressWorkModel.DistrictList = commonFunctions.PopulateDistrict(physicalProgressWorkModel.StateCode, true);
                    physicalProgressWorkModel.DistrictList.Find(x => x.Value == "-1").Value = "0";
                    physicalProgressWorkModel.DistrictCode = PMGSYSession.Current.DistrictCode == 0 ? 0 : PMGSYSession.Current.DistrictCode;
                    physicalProgressWorkModel.DistrictList.Find(x => x.Value == physicalProgressWorkModel.DistrictCode.ToString()).Selected = true;

                }
                physicalProgressWorkModel.BlockList = new List<SelectListItem>();
                if (physicalProgressWorkModel.DistrictCode == 0)
                {
                    physicalProgressWorkModel.BlockList.Insert(0, (new SelectListItem { Text = "All Blocks", Value = "0", Selected = true }));
                }
                else
                {
                    physicalProgressWorkModel.BlockList = commonFunctions.PopulateBlocks(physicalProgressWorkModel.DistrictCode, true);
                    physicalProgressWorkModel.BlockCode = 0;//PMGSYSession.Current.BlockCode == 0 ? 0 : PMGSYSession.Current.BlockCode;
                    physicalProgressWorkModel.BlockList.Find(x => x.Value == physicalProgressWorkModel.BlockCode.ToString()).Selected = true;
                }
                physicalProgressWorkModel.Year = DateTime.Now.Year;
                physicalProgressWorkModel.YearList = commonFunctions.PopulateYears(true);
                physicalProgressWorkModel.YearList.Find(x => x.Value == "0").Text = "All Years";
                physicalProgressWorkModel.BatchList = commonFunctions.PopulateBatch(true);
                physicalProgressWorkModel.FundingAgencyList = commonFunctions.PopulateFundingAgency(true);
                physicalProgressWorkModel.FundingAgencyList.Find(x => x.Value == "-1").Value = "0";

                physicalProgressWorkModel.localizedValue = "en";//PMGSYSession.Current.Language;

                return View(physicalProgressWorkModel);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "ProgressReport.PhysicalProgressWork()");
                return null;
            }
        }

        /// <summary>
        /// Render data in Physical Progress Of Work Report
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult PhysicalProgressWorkReport(PhysicalProgressWorkModel physicalProgressWorkModel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    // physicalProgressWorkModel.LevelCode = physicalProgressWorkModel.BlockCode > 0 ? 3 : physicalProgressWorkModel.DistrictCode > 0 ? 2 : 1;
                    physicalProgressWorkModel.LevelCode = physicalProgressWorkModel.RoadWise == true ? 4 : physicalProgressWorkModel.BlockCode > 0 ? 3 : physicalProgressWorkModel.DistrictCode > 0 ? 2 : 1;
                    physicalProgressWorkModel.Mast_State_Code = physicalProgressWorkModel.StateCode > 0 ? physicalProgressWorkModel.StateCode : physicalProgressWorkModel.Mast_State_Code;
                    physicalProgressWorkModel.Mast_District_Code = physicalProgressWorkModel.DistrictCode > 0 ? physicalProgressWorkModel.DistrictCode : physicalProgressWorkModel.Mast_District_Code;
                    physicalProgressWorkModel.Mast_Block_Code = physicalProgressWorkModel.BlockCode > 0 ? physicalProgressWorkModel.BlockCode : physicalProgressWorkModel.Mast_Block_Code;
                    return View(physicalProgressWorkModel);
                }
                else
                {
                    return View("PhysicalProgressWorkLayout", physicalProgressWorkModel);
                }
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "ProgressReport.PhysicalProgressWorkReport()");
                return View(physicalProgressWorkModel);
            }
        }

        public ActionResult BlockDetails(FormCollection frmCollection)
        {
            CommonFunctions objCommonFunctions = new CommonFunctions();
            List<SelectListItem> list = objCommonFunctions.PopulateBlocks(Convert.ToInt32(frmCollection["DistrictCode"]), true);
            list.Find(x => x.Value == "-1").Value = "0";
            return Json(list, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region PMGSY3 Progress report
        public ActionResult GetDistrictProgressView()
        {
            try
            {
                CommonFunctions commonFunctions = new CommonFunctions();
                DistricProgressViewModel viewmodel = new DistricProgressViewModel();
                viewmodel.StateName = PMGSYSession.Current.StateCode == 0 ? "0" : PMGSYSession.Current.StateName.Trim();
                viewmodel.DistName = PMGSYSession.Current.DistrictCode == 0 ? "0" : PMGSYSession.Current.DistrictName.Trim();
                viewmodel.BlockName = "0";//PMGSYSession.Current.BlockCode == 0 ? "0" : PMGSYSession.Current.BlockName.Trim();
                viewmodel.Mast_State_Code = PMGSYSession.Current.StateCode;
                viewmodel.Mast_District_Code = PMGSYSession.Current.DistrictCode;
                viewmodel.Mast_Block_Code = 0;//PMGSYSession.Current.BlockCode;
                viewmodel.LevelCode = PMGSYSession.Current.DistrictCode > 0 ? 2 : 1;

                if (PMGSYSession.Current.StateCode == 0)
                {
                    viewmodel.StateList = commonFunctions.PopulateStates(false);
                    //viewmodel.StateList.Find(x => x.Value == "0").Value = "-1";
                    //viewmodel.StateCode = PMGSYSession.Current.StateCode == 0 ? -1 : PMGSYSession.Current.StateCode;
                    //viewmodel.StateList.Find(x => x.Value == viewmodel.StateCode.ToString()).Selected = true;
                }
                else
                {
                    viewmodel.StateList = new List<SelectListItem>();
                    viewmodel.StateList.Insert(0, (new SelectListItem { Text = PMGSYSession.Current.StateName, Value = PMGSYSession.Current.StateCode.ToString(), Selected = true }));
                }
                viewmodel.DistrictList = new List<SelectListItem>();
                if (viewmodel.StateCode == -1)
                {
                    viewmodel.DistrictList.Insert(0, (new SelectListItem { Text = "All Districts", Value = "0", Selected = true }));
                }
                else
                {
                    viewmodel.DistrictList = commonFunctions.PopulateDistrict(PMGSYSession.Current.StateCode, true);
                    viewmodel.DistrictList.Find(x => x.Value == "-1").Value = "0";
                    viewmodel.DistrictCode = PMGSYSession.Current.DistrictCode == 0 ? 0 : PMGSYSession.Current.DistrictCode;
                    viewmodel.DistrictList.Find(x => x.Value == viewmodel.DistrictCode.ToString()).Selected = true;

                }

                if (PMGSYSession.Current.DistrictCode > 0)
                {
                    viewmodel.DistrictList.Clear();
                    viewmodel.DistrictList.Insert(0, new SelectListItem { Value = PMGSYSession.Current.DistrictCode.ToString(), Text = PMGSYSession.Current.DistrictName });
                }
                viewmodel.BlockList = new List<SelectListItem>();
                if (viewmodel.DistrictCode == 0)
                {
                    viewmodel.BlockList.Insert(0, (new SelectListItem { Text = "All Blocks", Value = "0", Selected = true }));
                }
                else
                {
                    viewmodel.BlockList = commonFunctions.PopulateBlocks(PMGSYSession.Current.DistrictCode, true);
                    viewmodel.BlockCode = 0;
                    viewmodel.BlockList.Find(x => x.Value == "-1").Value = "0";
                }
                return View(viewmodel);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "Areas.Progress.GetDistrictProgressView");
                return null;
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult GetDistrictProgressPost(DistricProgressViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    model.LevelCode = 1;
                    model.Mast_State_Code = model.StateCode;
                    model.Mast_District_Code = model.DistrictCode;
                    model.Mast_Block_Code = model.BlockCode;
                    return View(model);
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "ProgressReport/GetDistrictProgressPost");
                return null;
            }

        }

        #region Block Wise

        [HttpGet]
        public ActionResult GetBlockProgressView()
        {
            try
            {
                CommonFunctions commonFunctions = new CommonFunctions();
                DistricProgressViewModel viewmodel = new DistricProgressViewModel();
                viewmodel.StateName = PMGSYSession.Current.StateCode == 0 ? "0" : PMGSYSession.Current.StateName.Trim();
                viewmodel.DistName = PMGSYSession.Current.DistrictCode == 0 ? "0" : PMGSYSession.Current.DistrictName.Trim();
                viewmodel.BlockName = "0";//PMGSYSession.Current.BlockCode == 0 ? "0" : PMGSYSession.Current.BlockName.Trim();
                viewmodel.Mast_State_Code = PMGSYSession.Current.StateCode;
                viewmodel.Mast_District_Code = PMGSYSession.Current.DistrictCode;
                viewmodel.Mast_Block_Code = 0;//PMGSYSession.Current.BlockCode;
                viewmodel.LevelCode = PMGSYSession.Current.DistrictCode > 0 ? 2 : 1;

                if (PMGSYSession.Current.StateCode == 0)
                {
                    viewmodel.StateList = commonFunctions.PopulateStates(false);
                    //viewmodel.StateList.Find(x => x.Value == "0").Value = "-1";
                    //viewmodel.StateCode = PMGSYSession.Current.StateCode == 0 ? -1 : PMGSYSession.Current.StateCode;
                    //viewmodel.StateList.Find(x => x.Value == viewmodel.StateCode.ToString()).Selected = true;
                }
                else
                {
                    viewmodel.StateList = new List<SelectListItem>();
                    viewmodel.StateList.Insert(0, (new SelectListItem { Text = PMGSYSession.Current.StateName, Value = PMGSYSession.Current.StateCode.ToString(), Selected = true }));
                }
                viewmodel.DistrictList = new List<SelectListItem>();
                if (viewmodel.StateCode == -1)
                {
                    viewmodel.DistrictList.Insert(0, (new SelectListItem { Text = "All Districts", Value = "0", Selected = true }));
                }
                else
                {
                    viewmodel.DistrictList = commonFunctions.PopulateDistrict(PMGSYSession.Current.StateCode, true);
                    viewmodel.DistrictList.Find(x => x.Value == "-1").Value = "0";
                    viewmodel.DistrictCode = PMGSYSession.Current.DistrictCode == 0 ? 0 : PMGSYSession.Current.DistrictCode;
                    viewmodel.DistrictList.Find(x => x.Value == viewmodel.DistrictCode.ToString()).Selected = true;

                }

                if (PMGSYSession.Current.DistrictCode > 0)
                {
                    viewmodel.DistrictList.Clear();
                    viewmodel.DistrictList.Insert(0, new SelectListItem { Value = PMGSYSession.Current.DistrictCode.ToString(), Text = PMGSYSession.Current.DistrictName });
                }
                viewmodel.BlockList = new List<SelectListItem>();
                if (viewmodel.DistrictCode == 0)
                {
                    viewmodel.BlockList.Insert(0, (new SelectListItem { Text = "All Blocks", Value = "0", Selected = true }));
                }
                else
                {
                    viewmodel.BlockList = commonFunctions.PopulateBlocks(PMGSYSession.Current.DistrictCode, true);
                    viewmodel.BlockCode = 0;
                    viewmodel.BlockList.Find(x => x.Value == "-1").Value = "0";
                }
                return View(viewmodel);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "Areas.Progress.GetBlockProgressView");
                return null;
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult GetBlockProgressPost(DistricProgressViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    model.LevelCode = 1;
                    model.Mast_State_Code = model.StateCode;
                    model.Mast_District_Code = model.DistrictCode;
                    model.Mast_Block_Code = model.BlockCode;
                    return View(model);
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "ProgressReport/GetBlockProgressPost");
                return null;
            }

        }

        [HttpGet]
        public ActionResult GetCuplExclusionView()
        {
            try
            {
                CommonFunctions commonFunctions = new CommonFunctions();
                CuplExclusionModel viewModel = new CuplExclusionModel();

                viewModel.YearList = commonFunctions.PopulateYears(1900, true);

                if (PMGSYSession.Current.StateCode == 0)
                {
                    viewModel.StateList = commonFunctions.PopulateStates(false);
                }
                else
                {
                    viewModel.StateList = new List<SelectListItem>();
                    viewModel.StateList.Insert(0, (new SelectListItem { Text = PMGSYSession.Current.StateName, Value = PMGSYSession.Current.StateCode.ToString(), Selected = true }));
                }

                viewModel.BatchList = commonFunctions.PopulateBatch(true);

                return View(viewModel);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "Areas.Progress.GetCuplExclusionView");
                return null;
            }

        }

        [HttpPost]
        public ActionResult GetCuplExclusionPost(CuplExclusionModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    CommonFunctions commonFunctions = new CommonFunctions();

                    model.StateCode = model.StateCode;
                    model.Batch = model.Batch;
                    model.Year = model.Year;


                    if (PMGSYSession.Current.StateCode == 0)
                    {
                        model.StateList = commonFunctions.PopulateStates(false);
                    }
                    else
                    {
                        model.StateList = new List<SelectListItem>();
                        model.StateList.Insert(0, (new SelectListItem { Text = PMGSYSession.Current.StateName, Value = PMGSYSession.Current.StateCode.ToString(), Selected = true }));
                    }

                    foreach (var item in model.StateList)
                    {
                        if (item.Value.Equals(model.StateCode.ToString()))
                        {
                            model.StateName = item.Text;
                            break;
                        }
                    }

                    return View(model);
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "ProgressReport/GetCuplExclusionPost");
                return null;
            }

        }

        #endregion

        #endregion




        #region Work Details
        public ActionResult WorkDetailsLayout()
        {
            WorkDetails mpr1 = new WorkDetails();
            CommonFunctions commonFunctions = new CommonFunctions();
            PMGSYEntities dbContext = new PMGSYEntities();

            mpr1.YearList = commonFunctions.PopulateYears(true).ToList();
            mpr1.MonthList = commonFunctions.PopulateMonths(System.DateTime.Now.Month, false);

            mpr1.YearList.RemoveAt(0);
            mpr1.MonthList.RemoveAt(0);

            mpr1.Month = System.DateTime.Now.Month;
            mpr1.Year = System.DateTime.Now.Year;

            //mpr1.CollabList = commonFunctions.PopulateFundingAgency(true);
            //mpr1.CollabList.RemoveAt(0);
            //mpr1.CollabList.Insert(0, new SelectListItem { Value = "0", Text = "All Collaborations" });

            #region Filtering Logic
            mpr1.StateName = PMGSYSession.Current.StateCode == 0 ? "0" : PMGSYSession.Current.StateName.Trim();
            mpr1.Mast_State_Code = PMGSYSession.Current.StateCode;

            mpr1.StateList = commonFunctions.PopulateStates(true);
            mpr1.StateCode = PMGSYSession.Current.StateCode == 0 ? 0 : PMGSYSession.Current.StateCode;
            mpr1.StateList.Find(x => x.Value == mpr1.StateCode.ToString()).Selected = true;
            mpr1.StateList.Find(x => x.Value == "0").Text = "All States";


            mpr1.ProposalTypeList = new List<SelectListItem> { 
                                                            new SelectListItem{ Text = "All", Value ="%" , Selected = true }, 
                                                            new SelectListItem{ Text = "Road", Value ="P" }, 
                                                            new SelectListItem{ Text = "LSB", Value ="L" } 
                                                            //new SelectListItem{ Text = "RCPLWE", Value ="3" } 
                                                           
                                                            };

            mpr1.ConnectivityList = new List<SelectListItem> { 
                                                            new SelectListItem{ Text = "All", Value ="%" , Selected = true }, 
                                                            new SelectListItem{ Text = "New Connectivity", Value ="N" }, 
                                                            new SelectListItem{ Text = "Upgradation", Value ="U" } 
            };



            mpr1.SchemeList = new List<SelectListItem> { 
                                                            new SelectListItem{ Text = "All", Value ="0" , Selected = true }, 
                                                            new SelectListItem{ Text = "PMGSY I", Value ="1" }, 
                                                            new SelectListItem{ Text = "PMGSY II", Value ="2" },
                                                            new SelectListItem{ Text = "RCPLWE", Value ="3" },
                                                            new SelectListItem{ Text = "PMGSY III", Value ="4" } 
            };


           //if (PMGSYSession.Current.StateCode == 0)
           // {
           //     mpr1.DistrictList = new List<SelectListItem>();
           //     mpr1.DistrictList.Insert(0, new SelectListItem { Value = "0", Text = "All Districts" });
           // }
           // else
           // {
           //     mpr1.DistrictList = commonFunctions.PopulateDistrict(PMGSYSession.Current.StateCode, true);
           //     mpr1.DistrictList.Find(x => x.Value == "-1").Value = "0";
           // }

           // mpr1.BlockList = new List<SelectListItem>();
           // mpr1.BlockList.Insert(0, new SelectListItem { Value = "0", Text = "All Blocks" });
            #endregion

          

            return View(mpr1);
        }

        public ActionResult WorkDetailsReport(WorkDetails mpr1)
        {
            CommonFunctions commonFunctions = new CommonFunctions();
            try
            {
                if (ModelState.IsValid)
                {
                    mpr1.Mast_State_Code = mpr1.StateCode > 0 ? mpr1.StateCode : mpr1.Mast_State_Code;
                    mpr1.StateName = mpr1.StateCode == 0 ? "All States" : PMGSYSession.Current.StateName.Trim();
                    //mpr1.DistName = mpr1.DistrictCode == 0 ? "0" : mpr1.DistName.Trim();
                    //mpr1.BlockName = mpr1.CollabCode == 0 ? "0" : mpr1.BlockName.Trim();
                    return View(mpr1);
                }
                else
                {
                    string message = "";
                    //    bool flag = false;
                    message = ModelState.Values.Where(x => x.Errors.Count == 1).FirstOrDefault().Errors[0].ErrorMessage;

                    mpr1.YearList = commonFunctions.PopulateFinancialYear(false, false).ToList();
                    mpr1.MonthList = commonFunctions.PopulateMonths(System.DateTime.Now.Month, false);

                    mpr1.YearList.RemoveAt(0);
                    mpr1.MonthList.RemoveAt(0);

                    mpr1.Month = System.DateTime.Now.Month;
                    mpr1.Year = System.DateTime.Now.Year;

                    //mpr1.CollabList = commonFunctions.PopulateFundingAgency(true);
                    return View("WorkDetailsLayout", mpr1);
                }
            }
            catch
            {
                return View(mpr1);
            }
            //return View();
        }
        #endregion

        #region Monthly Review Reports.


        public ActionResult NationalSummary()
        {
            TargetAchievementViewModel model = new TargetAchievementViewModel();
            CommonFunctions comm = new CommonFunctions();
            try
            {
                model.fromDate = comm.GetDateTimeToString(DateTime.Now.AddDays(-1));
                model.toDate = comm.GetDateTimeToString(DateTime.Now.AddDays(-1));

                return View(model);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "Areas.ProgressReport.NationalSummary()");
                return null;
            }
        }

        public ActionResult NationalSummaryReport(TargetAchievementViewModel model)
        {
            CommonFunctions comm = new CommonFunctions();
            try
            {
                string[] arrfromdate = model.fromDate.Split('/');
                string[] arrtodate = model.toDate.Split('/');
                //if (comm.GetStringToDateTime(model.fromDate) > comm.GetStringToDateTime(model.toDate))
                if (arrfromdate[1] != arrtodate[1])
                {
                    return Json(new { success = false, message = "Please select dates for same month." });
                }
                if (comm.GetStringToDateTime(model.fromDate) > comm.GetStringToDateTime(model.toDate))
                {
                    return Json(new { success = false, message = "To Date should be greater than From Date." });
                }
                return View(model);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "Areas.ProgressReport.NationalSummaryReport()");
                return null;
            }
        }




        public ActionResult FormatTwo()
        {
            TargetAchievementViewModel model = new TargetAchievementViewModel();
            CommonFunctions comm = new CommonFunctions();
            try
            {
                model.fromDate = comm.GetDateTimeToString(DateTime.Now.AddDays(-1));
                model.toDate = comm.GetDateTimeToString(DateTime.Now.AddDays(-1));

                return View(model);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "Areas.ProgressReport.FormatTwo()");
                return null;
            }
        }

        public ActionResult FormatTwoReport(TargetAchievementViewModel model)
        {
            CommonFunctions comm = new CommonFunctions();
            try
            {
                string[] arrfromdate = model.fromDate.Split('/');
                string[] arrtodate = model.toDate.Split('/');
                //if (comm.GetStringToDateTime(model.fromDate) > comm.GetStringToDateTime(model.toDate))
                if (arrfromdate[1] != arrtodate[1])
                {
                    return Json(new { success = false, message = "Please select dates for same month." });
                }
                if (comm.GetStringToDateTime(model.fromDate) > comm.GetStringToDateTime(model.toDate))
                {
                    return Json(new { success = false, message = "To Date should be greater than From Date." });
                }
                return View(model);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "Areas.ProgressReport.FormatTwoReport()");
                return null;
            }
        }


        public ActionResult FormatThree()
        {
            TargetAchievementViewModel model = new TargetAchievementViewModel();
            CommonFunctions comm = new CommonFunctions();
            try
            {
                model.fromDate = comm.GetDateTimeToString(DateTime.Now.AddDays(-1));
                model.toDate = comm.GetDateTimeToString(DateTime.Now.AddDays(-1));

                return View(model);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "Areas.ProgressReport.FormatThree()");
                return null;
            }
        }

        public ActionResult FormatThreeReport(TargetAchievementViewModel model)
        {
            CommonFunctions comm = new CommonFunctions();
            try
            {
                string[] arrfromdate = model.fromDate.Split('/');
                string[] arrtodate = model.toDate.Split('/');
                //if (comm.GetStringToDateTime(model.fromDate) > comm.GetStringToDateTime(model.toDate))
                if (arrfromdate[1] != arrtodate[1])
                {
                    return Json(new { success = false, message = "Please select dates for same month." });
                }
                if (comm.GetStringToDateTime(model.fromDate) > comm.GetStringToDateTime(model.toDate))
                {
                    return Json(new { success = false, message = "To Date should be greater than From Date." });
                }
                return View(model);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "Areas.ProgressReport.FormatThreeReport()");
                return null;
            }
        }





        public ActionResult FormatFour()
        {
            TargetAchievementViewModel model = new TargetAchievementViewModel();
            CommonFunctions comm = new CommonFunctions();
            try
            {
                model.fromDate = comm.GetDateTimeToString(DateTime.Now.AddDays(-1));
                model.toDate = comm.GetDateTimeToString(DateTime.Now.AddDays(-1));

                return View(model);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "Areas.ProgressReport.FormatFour()");
                return null;
            }
        }

        public ActionResult FormatFourReport(TargetAchievementViewModel model)
        {
            CommonFunctions comm = new CommonFunctions();
            try
            {
                string[] arrfromdate = model.fromDate.Split('/');
                string[] arrtodate = model.toDate.Split('/');
                //if (comm.GetStringToDateTime(model.fromDate) > comm.GetStringToDateTime(model.toDate))
                if (arrfromdate[1] != arrtodate[1])
                {
                    return Json(new { success = false, message = "Please select dates for same month." });
                }
                if (comm.GetStringToDateTime(model.fromDate) > comm.GetStringToDateTime(model.toDate))
                {
                    return Json(new { success = false, message = "To Date should be greater than From Date." });
                }
                return View(model);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "Areas.ProgressReport.FormatFourReport()");
                return null;
            }
        }






        public ActionResult FormatFive()
        {
            TargetAchievementViewModel model = new TargetAchievementViewModel();
            CommonFunctions comm = new CommonFunctions();
            try
            {
                model.fromDate = comm.GetDateTimeToString(DateTime.Now.AddDays(-1));
                model.toDate = comm.GetDateTimeToString(DateTime.Now.AddDays(-1));

                return View(model);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "Areas.ProgressReport.FormatFive()");
                return null;
            }
        }

        public ActionResult FormatFiveReport(TargetAchievementViewModel model)
        {
            CommonFunctions comm = new CommonFunctions();
            try
            {
                string[] arrfromdate = model.fromDate.Split('/');
                string[] arrtodate = model.toDate.Split('/');
                //if (comm.GetStringToDateTime(model.fromDate) > comm.GetStringToDateTime(model.toDate))
                if (arrfromdate[1] != arrtodate[1])
                {
                    return Json(new { success = false, message = "Please select dates for same month." });
                }
                if (comm.GetStringToDateTime(model.fromDate) > comm.GetStringToDateTime(model.toDate))
                {
                    return Json(new { success = false, message = "To Date should be greater than From Date." });
                }
                return View(model);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "Areas.ProgressReport.FormatFiveReport()");
                return null;
            }
        }





        public ActionResult HabitationCoverageMPR20()
        {
            try
            {
                MonthYear my = new MonthYear();


                my.Month = System.DateTime.Now.Month;
                my.Year = System.DateTime.Now.Year;


                return View(my);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "ProgressReport.HabitationCoverageMPR20()");
                return View();
            }
        }

        // State-wise Details of Completed Works Pending Financial Closure as on 10-11-2020
        public ActionResult FormartThirteen()
        {
            try
            {
                MonthYear my = new MonthYear();


                my.Month = System.DateTime.Now.Month;
                my.Year = System.DateTime.Now.Year;


                return View(my);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "ProgressReport.FormartThirteen()");
                return View();
            }
        }

        public ActionResult FormatSix()
        {
            try
            {
                MonthYear my = new MonthYear();


                my.Month = System.DateTime.Now.Month;
                my.Year = System.DateTime.Now.Year;

                return View(my);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "ProgressReport.FormatSix()");
                return View();
            }
        }


        public ActionResult FormatNineteen()
        {
            try
            {
                MonthYear model = new MonthYear();


                model.Month = System.DateTime.Now.Month;
                model.Year = System.DateTime.Now.Year;

                return View(model);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "ProgressReport.FormatNineteen()");
                return View();
            }
        }

        public ActionResult FormatEighteen()
        {
            try
            {
                MonthYear model = new MonthYear();


                model.Month = System.DateTime.Now.Month;
                model.Year = System.DateTime.Now.Year;

                return View(model);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "ProgressReport.FormatEighteen()");
                return View();
            }
        }

        public ActionResult FormatFourteenBalanceSheet()
        {
            try
            {
                MonthYear my = new MonthYear();


                my.Month = System.DateTime.Now.Month;
                my.Year = System.DateTime.Now.Year;


                return View(my);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "ProgressReport.FormatFourteenBalanceSheet()");
                return View();
            }
        }

        public ActionResult FormatFifteen()
        {
            try
            {
                MonthYear my = new MonthYear();


                my.Month = System.DateTime.Now.Month;
                my.Year = System.DateTime.Now.Year;


                return View(my);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "ProgressReport.FormatFifteen()");
                return View();
            }
        }

        public ActionResult FormatTen()
        {
            try
            {
                MonthYear my = new MonthYear();


                my.Month = System.DateTime.Now.Month;
                my.Year = (System.DateTime.Now.Year)-1;

                return View(my);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "ProgressReport.FormatSix()");
                return View();
            }
        }

        public ActionResult Format17()
        {
            try
            {
                MonthYear my = new MonthYear();


                my.Month = System.DateTime.Now.Month;
                my.Year = System.DateTime.Now.Year;


                return View(my);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "ProgressReport.Format17()");
                return View();
            }
        }

        public ActionResult Format16()
        {
            try
            {
                MonthYear my = new MonthYear();


                my.Month = System.DateTime.Now.Month;
                my.Year = System.DateTime.Now.Year;


                return View(my);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "ProgressReport.Format16()");
                return View();
            }
        }


        //public ActionResult FormatSix()
        //{
        //    TargetAchievementViewModel model = new TargetAchievementViewModel();
        //    CommonFunctions comm = new CommonFunctions();
        //    try
        //    {
        //        model.fromDate = comm.GetDateTimeToString(DateTime.Now.AddDays(-1));
        //        model.toDate = comm.GetDateTimeToString(DateTime.Now.AddDays(-1));

        //        return View(model);
        //    }
        //    catch (Exception ex)
        //    {
        //        ErrorLog.LogError(ex, "Areas.ProgressReport.FormatSix()");
        //        return null;
        //    }
        //}

        //public ActionResult FormatSixReport(TargetAchievementViewModel model)
        //{
        //    CommonFunctions comm = new CommonFunctions();
        //    try
        //    {
        //        string[] arrfromdate = model.fromDate.Split('/');
        //        string[] arrtodate = model.toDate.Split('/');
        //        //if (comm.GetStringToDateTime(model.fromDate) > comm.GetStringToDateTime(model.toDate))
        //        if (arrfromdate[1] != arrtodate[1])
        //        {
        //            return Json(new { success = false, message = "Please select dates for same month." });
        //        }
        //        if (comm.GetStringToDateTime(model.fromDate) > comm.GetStringToDateTime(model.toDate))
        //        {
        //            return Json(new { success = false, message = "To Date should be greater than From Date." });
        //        }
        //        return View(model);
        //    }
        //    catch (Exception ex)
        //    {
        //        ErrorLog.LogError(ex, "Areas.ProgressReport.FormatSixReport()");
        //        return null;
        //    }
        //}



        //public ActionResult FormatSeven()
        //{
        //    TargetAchievementViewModel model = new TargetAchievementViewModel();
        //    CommonFunctions comm = new CommonFunctions();
        //    try
        //    {
        //        model.fromDate = comm.GetDateTimeToString(DateTime.Now.AddDays(-1));
        //        model.toDate = comm.GetDateTimeToString(DateTime.Now.AddDays(-1));

        //        return View(model);
        //    }
        //    catch (Exception ex)
        //    {
        //        ErrorLog.LogError(ex, "Areas.ProgressReport.FormatSeven()");
        //        return null;
        //    }
        //}

        //public ActionResult FormatSevenReport(TargetAchievementViewModel model)
        //{
        //    CommonFunctions comm = new CommonFunctions();
        //    try
        //    {
        //        string[] arrfromdate = model.fromDate.Split('/');
        //        string[] arrtodate = model.toDate.Split('/');
        //        //if (comm.GetStringToDateTime(model.fromDate) > comm.GetStringToDateTime(model.toDate))
        //        if (arrfromdate[1] != arrtodate[1])
        //        {
        //            return Json(new { success = false, message = "Please select dates for same month." });
        //        }
        //        if (comm.GetStringToDateTime(model.fromDate) > comm.GetStringToDateTime(model.toDate))
        //        {
        //            return Json(new { success = false, message = "To Date should be greater than From Date." });
        //        }
        //        return View(model);
        //    }
        //    catch (Exception ex)
        //    {
        //        ErrorLog.LogError(ex, "Areas.ProgressReport.FormatSevenReport()");
        //        return null;
        //    }
        //}



        //public ActionResult FormatEight()
        //{
        //    TargetAchievementViewModel model = new TargetAchievementViewModel();
        //    CommonFunctions comm = new CommonFunctions();
        //    try
        //    {
        //        model.fromDate = comm.GetDateTimeToString(DateTime.Now.AddDays(-1));
        //        model.toDate = comm.GetDateTimeToString(DateTime.Now.AddDays(-1));

        //        return View(model);
        //    }
        //    catch (Exception ex)
        //    {
        //        ErrorLog.LogError(ex, "Areas.ProgressReport.FormatEight()");
        //        return null;
        //    }
        //}

        //public ActionResult FormatEightReport(TargetAchievementViewModel model)
        //{
        //    CommonFunctions comm = new CommonFunctions();
        //    try
        //    {
        //        string[] arrfromdate = model.fromDate.Split('/');
        //        string[] arrtodate = model.toDate.Split('/');
        //        //if (comm.GetStringToDateTime(model.fromDate) > comm.GetStringToDateTime(model.toDate))
        //        if (arrfromdate[1] != arrtodate[1])
        //        {
        //            return Json(new { success = false, message = "Please select dates for same month." });
        //        }
        //        if (comm.GetStringToDateTime(model.fromDate) > comm.GetStringToDateTime(model.toDate))
        //        {
        //            return Json(new { success = false, message = "To Date should be greater than From Date." });
        //        }
        //        return View(model);
        //    }
        //    catch (Exception ex)
        //    {
        //        ErrorLog.LogError(ex, "Areas.ProgressReport.FormatEightReport()");
        //        return null;
        //    }
        //}



        //public ActionResult FormatNine()
        //{
        //    TargetAchievementViewModel model = new TargetAchievementViewModel();
        //    CommonFunctions comm = new CommonFunctions();
        //    try
        //    {
        //        model.fromDate = comm.GetDateTimeToString(DateTime.Now.AddDays(-1));
        //        model.toDate = comm.GetDateTimeToString(DateTime.Now.AddDays(-1));

        //        return View(model);
        //    }
        //    catch (Exception ex)
        //    {
        //        ErrorLog.LogError(ex, "Areas.ProgressReport.FormatNine()");
        //        return null;
        //    }
        //}

        //public ActionResult FormatNineReport(TargetAchievementViewModel model)
        //{
        //    CommonFunctions comm = new CommonFunctions();
        //    try
        //    {
        //        string[] arrfromdate = model.fromDate.Split('/');
        //        string[] arrtodate = model.toDate.Split('/');
        //        //if (comm.GetStringToDateTime(model.fromDate) > comm.GetStringToDateTime(model.toDate))
        //        if (arrfromdate[1] != arrtodate[1])
        //        {
        //            return Json(new { success = false, message = "Please select dates for same month." });
        //        }
        //        if (comm.GetStringToDateTime(model.fromDate) > comm.GetStringToDateTime(model.toDate))
        //        {
        //            return Json(new { success = false, message = "To Date should be greater than From Date." });
        //        }
        //        return View(model);
        //    }
        //    catch (Exception ex)
        //    {
        //        ErrorLog.LogError(ex, "Areas.ProgressReport.FormatNineReport()");
        //        return null;
        //    }
        //}




        //public ActionResult FormatTen()
        //{
        //    TargetAchievementViewModel model = new TargetAchievementViewModel();
        //    CommonFunctions comm = new CommonFunctions();
        //    try
        //    {
        //        model.fromDate = comm.GetDateTimeToString(DateTime.Now.AddDays(-1));
        //        model.toDate = comm.GetDateTimeToString(DateTime.Now.AddDays(-1));

        //        return View(model);
        //    }
        //    catch (Exception ex)
        //    {
        //        ErrorLog.LogError(ex, "Areas.ProgressReport.FormatTen()");
        //        return null;
        //    }
        //}

        //public ActionResult FormatTenReport(TargetAchievementViewModel model)
        //{
        //    CommonFunctions comm = new CommonFunctions();
        //    try
        //    {
        //        string[] arrfromdate = model.fromDate.Split('/');
        //        string[] arrtodate = model.toDate.Split('/');
        //        //if (comm.GetStringToDateTime(model.fromDate) > comm.GetStringToDateTime(model.toDate))
        //        if (arrfromdate[1] != arrtodate[1])
        //        {
        //            return Json(new { success = false, message = "Please select dates for same month." });
        //        }
        //        if (comm.GetStringToDateTime(model.fromDate) > comm.GetStringToDateTime(model.toDate))
        //        {
        //            return Json(new { success = false, message = "To Date should be greater than From Date." });
        //        }
        //        return View(model);
        //    }
        //    catch (Exception ex)
        //    {
        //        ErrorLog.LogError(ex, "Areas.ProgressReport.FormatTenReport()");
        //        return null;
        //    }
        //}



        public ActionResult FormatEightMaintenanceStatus()
        {
            try
            {
                MonthYear my = new MonthYear();

                my.Year = System.DateTime.Now.Year;

                return View(my);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "ProgressReport.FormatEightMaintenanceStatus()");
                return View();
            }
        }

        public ActionResult FormatNineMaintenanceStatus()
        {
            try
            {
                MonthYear my = new MonthYear();

                my.Year = System.DateTime.Now.Year;

                return View(my);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "ProgressReport.FormatNineMaintenanceStatus()");
                return View();
            }
        }

        //Mpr Booklet Report
        public ActionResult MPRBooklet()
        {
            try
            {
                ViewData["Month"] = DateTime.Now.Month.ToString();
                ViewData["Year"] = DateTime.Now.Year.ToString();
                ViewData["award"] = 0;
                return View();
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "ProgressReport.MPRBooklet()");
                return View();
            }
        }
        #endregion



        #region Dynamic Report At Admin Login
        public ActionResult DynamicLayout()
        {
            DynamicSQL model = new DynamicSQL();
            CommonFunctions comm = new CommonFunctions();
            try
            {
                //model.fromDate = comm.GetDateTimeToString(DateTime.Now.AddDays(-1));
              //  model.toDate = comm.GetDateTimeToString(DateTime.Now.AddDays(-1));

                return View(model);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "Areas.ProgressReport.DynamicLayout()");
                return null;
            }
        }

        public ActionResult DynamicReport(DynamicSQL model)
        {
            CommonFunctions comm = new CommonFunctions();
            try
            {
              
                return View(model);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "Areas.ProgressReport.DynamicReport()");
                return null;
            }
        }

        #endregion


        #region District Achievement
        public ActionResult AspirationalDistAchievement()
        {
            try
            {
                ProgressViewModel progressModel = new ProgressViewModel
                {
                    YearList = common.PopulateFinancialYear(true).ToList(),
                };
                if (DateTime.Now.Month < 4)
                {
                    progressModel.YearList.RemoveAt(1);
                }
                return View(progressModel);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "ProgressReport.AspirationalDistAchievement()");
                return View();
            }
        }

        public ActionResult AspirationalDistAchievementReport(ProgressViewModel model)
        {
            try
            {
                return View(model);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "ProgressReport.AspirationalDistAchievementReport()");
                return View();
            }
        }
        #endregion 

        #region Awarded and InProgress Roadworks
        /// <summary>
        /// Layout for Awarded and InProgress Roadworks Report
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult AwardedInProgressRoadworksLayout()
        {
            AwardedInProgressRoadworksViewModel roadModel = new AwardedInProgressRoadworksViewModel();
            CommonFunctions commonFunctions = new CommonFunctions();
            try
            {

                if (PMGSYSession.Current.StateCode == 0)
                {
                    roadModel.StateList = commonFunctions.PopulateStates(false);
                    roadModel.StateList.Find(x => x.Value == "0").Value = "-1";
                    roadModel.StateCode = PMGSYSession.Current.StateCode == 0 ? -1 : PMGSYSession.Current.StateCode;
                    roadModel.StateList.Find(x => x.Value == roadModel.StateCode.ToString()).Selected = true;
                }
                else
                {
                    roadModel.StateList = new List<SelectListItem>();
                    roadModel.StateList.Insert(0, (new SelectListItem { Text = PMGSYSession.Current.StateName, Value = PMGSYSession.Current.StateCode.ToString(), Selected = true }));
                }
                roadModel.SchemeCode = 0;
                roadModel.SchemeList = new List<SelectListItem>();
                roadModel.SchemeList.Insert(0, new SelectListItem() { Value = "0", Text = "All", Selected = true });
                roadModel.SchemeList.Insert(1, new SelectListItem() { Value = "1", Text = "PMGSY I" });
                roadModel.SchemeList.Insert(2, new SelectListItem() { Value = "2", Text = "PMGSY II" });
                roadModel.SchemeList.Insert(3, new SelectListItem() { Value = "3", Text = "RCPLWE" });
                roadModel.SchemeList.Insert(4, new SelectListItem() { Value = "4", Text = "PMGSY III" });
                roadModel.Level = roadModel.StateCode == -1 ? 1 : 2;
                roadModel.PhaseCode = "0";
                roadModel.PhaseList = new List<SelectListItem>();
                roadModel.PhaseList.Insert(0, new SelectListItem() { Value = "0", Text = "All", Selected = true });
                roadModel.PhaseList.Insert(1, new SelectListItem() { Value = "S1", Text = "Stage - 1" });
                roadModel.PhaseList.Insert(2, new SelectListItem() { Value = "S2", Text = "Stage - 2" });
                roadModel.PhaseList.Insert(3, new SelectListItem() { Value = "C", Text = "Completed" });

                return View(roadModel);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "ProgressReport.AwardedInProgressRoadworksLayout()");
                return null;
            }
        }

        [HttpPost]
        public ActionResult AwardedInProgressRoadworksReport(AwardedInProgressRoadworksViewModel roadModel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    roadModel.Level = roadModel.StateCode == -1 ? 1 : 2;
                    return View(roadModel);
                }
                else
                {
                    return View("AwardedInProgressRoadworksLayout", roadModel);
                }
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "ProgressReport.AwardedInProgressRoadworksReport()");
                return View(roadModel);
            }
        }
        #endregion

        #region PMIS reports.
        public ActionResult PMISProgressReportsLayout()
        {
            try
            {
                CommonFunctions commonFunctions = new CommonFunctions();
                PMISProgressReports viewmodel = new PMISProgressReports();
                viewmodel.StateName = PMGSYSession.Current.StateCode == 0 ? "0" : PMGSYSession.Current.StateName.Trim();
                viewmodel.DistName = PMGSYSession.Current.DistrictCode == 0 ? "0" : PMGSYSession.Current.DistrictName.Trim();
                viewmodel.Mast_State_Code = PMGSYSession.Current.StateCode;
                viewmodel.Mast_District_Code = PMGSYSession.Current.DistrictCode;
                viewmodel.LevelCode = PMGSYSession.Current.DistrictCode > 0 ? 2 : 1;

                if (PMGSYSession.Current.StateCode == 0)
                {
                    viewmodel.StateList = commonFunctions.PopulateStates(false);
                    //viewmodel.StateList.Find(x => x.Value == "0").Value = "-1";
                    //viewmodel.StateCode = PMGSYSession.Current.StateCode == 0 ? -1 : PMGSYSession.Current.StateCode;
                    //viewmodel.StateList.Find(x => x.Value == viewmodel.StateCode.ToString()).Selected = true;
                }
                else
                {
                    viewmodel.StateList = new List<SelectListItem>();
                    viewmodel.StateList.Insert(0, (new SelectListItem { Text = PMGSYSession.Current.StateName, Value = PMGSYSession.Current.StateCode.ToString(), Selected = true }));
                }
                viewmodel.DistrictList = new List<SelectListItem>();
                if (viewmodel.StateCode == -1)
                {
                    viewmodel.DistrictList.Insert(0, (new SelectListItem { Text = "All Districts", Value = "0", Selected = true }));
                }
                else
                {
                    viewmodel.DistrictList = commonFunctions.PopulateDistrict(PMGSYSession.Current.StateCode, true);
                    viewmodel.DistrictList.Find(x => x.Value == "-1").Value = "0";
                    viewmodel.DistrictCode = PMGSYSession.Current.DistrictCode == 0 ? 0 : PMGSYSession.Current.DistrictCode;
                    viewmodel.DistrictList.Find(x => x.Value == viewmodel.DistrictCode.ToString()).Selected = true;

                }

                if (PMGSYSession.Current.DistrictCode > 0)
                {
                    viewmodel.DistrictList.Clear();
                    viewmodel.DistrictList.Insert(0, new SelectListItem { Value = PMGSYSession.Current.DistrictCode.ToString(), Text = PMGSYSession.Current.DistrictName });
                }

                return View(viewmodel);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "Areas.Progress.PMISProgressReportsLayout");
                return null;
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult GetPMISProgressReports(PMISProgressReports model)
        {
            try
            {
                if (ModelState.IsValid)
                {

                    //model.LevelCode = model.LevelCode;
                    model.Mast_State_Code = model.StateCode;
                    if (model.Mast_State_Code == 0)
                    {
                        model.LevelCode = 1;
                    }
                    else
                    {
                        model.LevelCode = 3;
                    }
                    model.Mast_District_Code = model.DistrictCode;
                    return View(model);
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "ProgressReport/GetPMISProgressReports");
                return null;
            }
        }
        #endregion


    }
}
