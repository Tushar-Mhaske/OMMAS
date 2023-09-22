using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PMGSY.Areas.QMSSRSReports.Models;
using PMGSY.Extensions;
using PMGSY.Common;
using PMGSY.Models;
using PMGSY.Controllers;
using System.IO;
using System.Configuration;
using PMGSY.Areas.Feedback.Models;
using PMGSY.Areas.MPR.Models;
namespace PMGSY.Areas.Feedback.Controllers
{
    [RequiredAuthentication]
    [RequiredAuthorization]
    public class FeedbackController : Controller
    {
        //
        // GET: /Feedback/Feedback/

        public ActionResult Index()
        {
            return View();
        }

        #region Feedback Details
        [HttpGet]
        public ActionResult FeedbackLayout()
        {
            QualityReportsViewModel QM = new QualityReportsViewModel();
            CommonFunctions comm = new CommonFunctions();
            try
            {

                QM.State = PMGSYSession.Current.StateCode;
                QM.StateList = comm.PopulateStates(true);
                QM.StateList.RemoveAt(0);
                QM.StateList.Insert(0, (new SelectListItem { Text = "All", Value = "0", Selected = true }));



                return View(QM);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return null;
            }
            finally
            {
                if (comm != null)
                {
                    comm.Dispose();
                }
            }
        }


        [HttpPost]
        public ActionResult FeedbackReport(QualityReportsViewModel QM)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (PMGSYSession.Current.StateName != null)
                    {
                        QM.StateName = PMGSYSession.Current.StateName;
                    }
                    else
                    {
                        QM.StateName = QM.State == 0 ? "All States" : QM.StateName;
                    }
                    return View(QM);
                }
                else
                {
                    string message = ModelState.Values.Where(x => x.Errors.Count == 1).FirstOrDefault().Errors[0].ErrorMessage;
                    return null;
                }
            }
            catch
            {
                return null;
            }
        }
        #endregion Feedback Details

        #region Feedback Statistics
        [HttpGet]
        public ActionResult FeedbackStatLayout()
        {
            QualityReportsViewModel QM = new QualityReportsViewModel();
            CommonFunctions comm = new CommonFunctions();
            try
            {

                QM.State = PMGSYSession.Current.StateCode;
                QM.StateList = comm.PopulateStates(true);
                QM.StateList.RemoveAt(0);
                QM.StateList.Insert(0, (new SelectListItem { Text = "All", Value = "0", Selected = true }));



                return View(QM);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return null;
            }
            finally
            {
                if (comm != null)
                {
                    comm.Dispose();
                }
            }
        }


        [HttpPost]
        public ActionResult FeedbackStatReport(QualityReportsViewModel QM)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (PMGSYSession.Current.StateName != null)
                    {
                        QM.StateName = PMGSYSession.Current.StateName;
                    }
                    else
                    {
                        QM.StateName = QM.State == 0 ? "All States" : QM.StateName;
                    }
                    return View(QM);
                }
                else
                {
                    string message = ModelState.Values.Where(x => x.Errors.Count == 1).FirstOrDefault().Errors[0].ErrorMessage;
                    return null;
                }
            }
            catch
            {
                return null;
            }
        }


        #endregion

        #region Eligible Vs Cleared Vs Connect
        public ActionResult GetHabCoverageOne()
        {
            try
            {
                if (ModelState.IsValid)
                {

                    return View();
                }
                else
                {
                    return null;
                }
            }
            catch
            {
                return null;
            }
        }

        #endregion

        #region Eligible Vs Connected
        public ActionResult GetHabCoverageTwo()
        {
            try
            {
                if (ModelState.IsValid)
                {

                    return View();
                }
                else
                {
                    return null;
                }
            }
            catch
            {
                return null;
            }
        }

        #endregion


        //1.Length Constructed
        [HttpGet]
        public ActionResult LengthClearedOne()
        {
            FeedbackModel feedBack = new FeedbackModel();
            CommonFunctions comm = new CommonFunctions();
            try
            {
                feedBack.Year = DateTime.Now.Year;
                feedBack.YearList = comm.PopulateYears(DateTime.Now.Year);

                feedBack.Month = DateTime.Now.Month;
                feedBack.MonthList = comm.PopulateMonths(DateTime.Now.Month);

                return View(feedBack);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return null;
            }
            finally
            {
                if (comm != null)
                {
                    comm.Dispose();
                }
            }
        }

        public ActionResult LengthClearedOneReport(FeedbackModel feedBack)
        {
            try
            {
                if (ModelState.IsValid)
                {

                    return View(feedBack);
                }
                else
                {
                    string message = ModelState.Values.Where(x => x.Errors.Count == 1).FirstOrDefault().Errors[0].ErrorMessage;
                    return null;
                }
            }
            catch
            {
                return null;
            }

        }




        //2.Length Cleared Vs Constructed
        [HttpGet]
        public ActionResult LengthClearedTwo()
        {
            FeedbackModel feedBack = new FeedbackModel();
            CommonFunctions comm = new CommonFunctions();
            try
            {
                feedBack.Year = DateTime.Now.Year;
                feedBack.YearList = comm.PopulateYears(DateTime.Now.Year);

                feedBack.Month = DateTime.Now.Month;
                feedBack.MonthList = comm.PopulateMonths(DateTime.Now.Month);

                return View(feedBack);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return null;
            }
            finally
            {
                if (comm != null)
                {
                    comm.Dispose();
                }
            }
        }

        public ActionResult LengthClearedTwoReport(FeedbackModel feedBack)
        {
            try
            {
                if (ModelState.IsValid)
                {

                    return View(feedBack);
                }
                else
                {
                    string message = ModelState.Values.Where(x => x.Errors.Count == 1).FirstOrDefault().Errors[0].ErrorMessage;
                    return null;
                }
            }
            catch
            {
                return null;
            }

        }





        //3.New Connectivity
        [HttpGet]
        public ActionResult LengthClearedThree()
        {
            FeedbackModel feedBack = new FeedbackModel();
            CommonFunctions comm = new CommonFunctions();
            try
            {
                feedBack.Year = DateTime.Now.Year;
                feedBack.YearList = comm.PopulateYears(DateTime.Now.Year);

                feedBack.Month = DateTime.Now.Month;
                feedBack.MonthList = comm.PopulateMonths(DateTime.Now.Month);

                return View(feedBack);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return null;
            }
            finally
            {
                if (comm != null)
                {
                    comm.Dispose();
                }
            }
        }

        public ActionResult LengthClearedThreeReport(FeedbackModel feedBack)
        {
            try
            {
                if (ModelState.IsValid)
                {

                    return View(feedBack);
                }
                else
                {
                    string message = ModelState.Values.Where(x => x.Errors.Count == 1).FirstOrDefault().Errors[0].ErrorMessage;
                    return null;
                }
            }
            catch
            {
                return null;
            }

        }







        //4.Upgradation
        [HttpGet]
        public ActionResult LengthClearedFour()
        {
            FeedbackModel feedBack = new FeedbackModel();
            CommonFunctions comm = new CommonFunctions();
            try
            {
                feedBack.Year = DateTime.Now.Year;
                feedBack.YearList = comm.PopulateYears(DateTime.Now.Year);

                feedBack.Month = DateTime.Now.Month;
                feedBack.MonthList = comm.PopulateMonths(DateTime.Now.Month);

                return View(feedBack);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return null;
            }
            finally
            {
                if (comm != null)
                {
                    comm.Dispose();
                }
            }
        }

        public ActionResult LengthClearedFourReport(FeedbackModel feedBack)
        {
            try
            {
                if (ModelState.IsValid)
                {

                    return View(feedBack);
                }
                else
                {
                    string message = ModelState.Values.Where(x => x.Errors.Count == 1).FirstOrDefault().Errors[0].ErrorMessage;
                    return null;
                }
            }
            catch
            {
                return null;
            }

        }


        //5.Length Cleared
        [HttpGet]
        public ActionResult LengthClearedFive()
        {

            FeedbackModel feedBack = new FeedbackModel();
            CommonFunctions comm = new CommonFunctions();
            try
            {
                feedBack.Year = DateTime.Now.Year;
                feedBack.YearList = comm.PopulateYears(DateTime.Now.Year);

                feedBack.Month = DateTime.Now.Month;
                feedBack.MonthList = comm.PopulateMonths(DateTime.Now.Month);

                return View(feedBack);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return null;
            }
            finally
            {
                if (comm != null)
                {
                    comm.Dispose();
                }
            }
        }

        public ActionResult LengthClearedFiveReport(FeedbackModel feedBack)
        {
            try
            {
                if (ModelState.IsValid)
                {

                    return View(feedBack);
                }
                else
                {
                    string message = ModelState.Values.Where(x => x.Errors.Count == 1).FirstOrDefault().Errors[0].ErrorMessage;
                    return null;
                }
            }
            catch
            {
                return null;
            }

        }



        // Hab Coverage New Report


        [HttpGet]
        public ActionResult HabCoverageNewLayout()
        {

            //FeedbackModel feedBack = new FeedbackModel();
            CommonFunctions comm = new CommonFunctions();
            HabitationClusterModel objHabitationClusterModel = new HabitationClusterModel();

            try
            {
                objHabitationClusterModel.Year = DateTime.Now.Year;
                objHabitationClusterModel.YearList = comm.PopulateYears(DateTime.Now.Year);

                objHabitationClusterModel.Month = DateTime.Now.Month;
                objHabitationClusterModel.MonthList = comm.PopulateMonths(DateTime.Now.Month);

                var lstStateList = objHabitationClusterModel.StateList;
                lstStateList.RemoveAt(0);
                lstStateList.Insert(0, new SelectListItem { Value = "0", Text = "All States" });

                return View(objHabitationClusterModel);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return null;
            }
            finally
            {
                if (comm != null)
                {
                    comm.Dispose();
                }
            }
        }

        public ActionResult HabCoverageNewReport(HabitationClusterModel objHabitationClusterModel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (objHabitationClusterModel.StateCode > 0)
                    {
                        objHabitationClusterModel.LevelCode = 2;
                    }
                    if (objHabitationClusterModel.StateCode == 0)
                    {
                        objHabitationClusterModel.LevelCode = 1;
                    }
                    objHabitationClusterModel.LevelCode = objHabitationClusterModel.Roadwise == true ? 3 : objHabitationClusterModel.LevelCode;
                    return View(objHabitationClusterModel);
                }
                else
                {
                    string message = ModelState.Values.Where(x => x.Errors.Count == 1).FirstOrDefault().Errors[0].ErrorMessage;
                    return null;
                }
            }
            catch
            {
                return null;
            }

        }

        // End of Hab Coverahe New Report





        //Hanitations Cleared and Connected
        public ActionResult HabsClearedConnectedReport()
        {
            try
            {
                ViewBag.Level = 1;
                ViewBag.State = 0;
                ViewBag.District = 0;
                ViewBag.Block = 0;

                return View();
            }
            catch
            {
                return null;
            }
        }



        //Habitation Cluster

        [HttpGet]
        public ActionResult HabitationClusterReportLayout()
        {
            HabitationClusterModel objHabitationClu = new HabitationClusterModel();
            return View(objHabitationClu);
        }

        [HttpPost]
        public ActionResult HabitationClusterReport(HabitationClusterModel objHabitationClu)
        {
            try
            {
                if (ModelState.ContainsKey("Month"))
                {
                    ModelState["Month"].Errors.Clear();

                }
                if (ModelState.ContainsKey("Year"))
                {
                    ModelState["Year"].Errors.Clear();

                }
                if (ModelState.IsValid)
                {
                    objHabitationClu.Mast_State_Code = objHabitationClu.StateCode > 0 ? objHabitationClu.StateCode : objHabitationClu.Mast_State_Code;
                    objHabitationClu.Mast_District_Code = objHabitationClu.DistrictCode > 0 ? objHabitationClu.DistrictCode : objHabitationClu.Mast_District_Code;
                    objHabitationClu.Mast_Block_Code = objHabitationClu.BlockCode > 0 ? objHabitationClu.BlockCode : objHabitationClu.Mast_Block_Code;

                    return View(objHabitationClu);
                }
                else
                {
                    return View("HabitationClusterReport", objHabitationClu);
                }
            }
            catch
            {
                return View(objHabitationClu);
            }

        }
        //End  Habitation Cluster

        #region MPR1 NEW REPORT

        [HttpPost]
        public JsonResult PopulateAgencies(int stateCode)
        {
            try
            {
                CommonFunctions comm = new CommonFunctions();
                //int stateCode = Convert.ToInt32(Request.Params["stateCode"]);

                return Json(comm.PopulateAgencies(stateCode, true));
            }
            catch
            {
                return Json(new { string.Empty });
            }
        }

        public ActionResult MPR1NewLayout()
        {
            MPR1Model mpr1 = new MPR1Model();
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

            mpr1.SchemeList = new List<SelectListItem>();
            mpr1.SchemeList.Add(new SelectListItem { Value = "0", Text = "All" });
            mpr1.SchemeList.Add(new SelectListItem { Value = "1", Text = "PMGSY1" });
            mpr1.SchemeList.Add(new SelectListItem { Value = "2", Text = "PMGSY2" });
            mpr1.SchemeList.Add(new SelectListItem { Value = "3", Text = "RCPLWE" });
            mpr1.SchemeList.Add(new SelectListItem { Value = "4", Text = "PMGSY3" });
            mpr1.Scheme = 0;

            mpr1.AgencyList = new List<SelectListItem>();
            mpr1.AgencyList.Add(new SelectListItem { Value = "0", Text = "All Agencies" });

            mpr1.AgencyList = commonFunctions.PopulateAgencies(mpr1.StateCode, true);

            // mpr1.Agency = dbContext.ADMIN_DEPARTMENT.Where(m => m.ADMIN_ND_CODE == PMGSYSession.Current.AdminNdCode).Select(m => m.MAST_AGENCY_CODE).FirstOrDefault();//PMGSYSession.Current.AdminNdCode;
            //  mpr1.AgencyName = PMGSYSession.Current.DepartmentName.Trim();


            #region Filtering Logic
            mpr1.StateName = PMGSYSession.Current.StateCode == 0 ? "0" : PMGSYSession.Current.StateName.Trim();
            mpr1.Mast_State_Code = PMGSYSession.Current.StateCode;

            mpr1.StateList = commonFunctions.PopulateStates(true);
            mpr1.StateCode = PMGSYSession.Current.StateCode == 0 ? 0 : PMGSYSession.Current.StateCode;
            mpr1.StateList.Find(x => x.Value == mpr1.StateCode.ToString()).Selected = true;
            mpr1.StateList.Find(x => x.Value == "0").Text = "All States";


            #endregion

            //if (PMGSYSession.Current.Language.Contains('-'))
            //{
            //    mpr1.localizedValue = PMGSYSession.Current.Language.Substring(0, PMGSYSession.Current.Language.IndexOf('-'));
            //}
            //else
            //{
            //    mpr1.localizedValue = PMGSYSession.Current.Language;
            //}
            mpr1.localizedValue = PMGSYSession.Current.Language;
            mpr1.localizedValue = "en";

            return View(mpr1);
        }



        public ActionResult MPR1NewReport(MPR1Model mpr1)
        {
            CommonFunctions commonFunctions = new CommonFunctions();
            //MPRModel mprModel = new MPRModel();
            //FetchCookieData fetchCookie = new FetchCookieData();
            try
            {
                if (ModelState.IsValid)
                {

                    mpr1.Mast_State_Code = mpr1.StateCode > 0 ? mpr1.StateCode : mpr1.Mast_State_Code;
                    mpr1.StateName = mpr1.StateCode == 0 ? "0" : mpr1.StateName.Trim();

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

                    //mpr1.Month = System.DateTime.Now.Month;
                    //mpr1.Year = System.DateTime.Now.Year;

                    mpr1.CollabList = commonFunctions.PopulateFundingAgency(true);
                    return View("MPR1NewReport", mpr1);
                }
            }
            catch
            {
                return View(mpr1);
            }
            //return View();
        }
        #endregion

        #region MPR1 NEW IAP REPORT

        public ActionResult MPR1NewIAPLayout()
        {
            MPR1Model mpr1 = new MPR1Model();
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

            mpr1.SchemeList = new List<SelectListItem>();
            mpr1.SchemeList.Add(new SelectListItem { Value = "0", Text = "Both" });
            mpr1.SchemeList.Add(new SelectListItem { Value = "1", Text = "PMGSY1" });
            mpr1.SchemeList.Add(new SelectListItem { Value = "2", Text = "PMGSY2" });
            mpr1.Scheme = 0;

            mpr1.AgencyList = new List<SelectListItem>();
            mpr1.AgencyList.Add(new SelectListItem { Value = "0", Text = "All Agencies" });

            mpr1.AgencyList = commonFunctions.PopulateAgencies(mpr1.StateCode, true);

            // mpr1.Agency = dbContext.ADMIN_DEPARTMENT.Where(m => m.ADMIN_ND_CODE == PMGSYSession.Current.AdminNdCode).Select(m => m.MAST_AGENCY_CODE).FirstOrDefault();//PMGSYSession.Current.AdminNdCode;
            //  mpr1.AgencyName = PMGSYSession.Current.DepartmentName.Trim();


            #region Filtering Logic
            mpr1.StateName = PMGSYSession.Current.StateCode == 0 ? "0" : PMGSYSession.Current.StateName.Trim();
            mpr1.Mast_State_Code = PMGSYSession.Current.StateCode;

            mpr1.StateList = commonFunctions.PopulateStates(true);
            mpr1.StateCode = PMGSYSession.Current.StateCode == 0 ? 0 : PMGSYSession.Current.StateCode;
            mpr1.StateList.Find(x => x.Value == mpr1.StateCode.ToString()).Selected = true;
            mpr1.StateList.Find(x => x.Value == "0").Text = "All States";


            #endregion

            //if (PMGSYSession.Current.Language.Contains('-'))
            //{
            //    mpr1.localizedValue = PMGSYSession.Current.Language.Substring(0, PMGSYSession.Current.Language.IndexOf('-'));
            //}
            //else
            //{
            //    mpr1.localizedValue = PMGSYSession.Current.Language;
            //}
            mpr1.localizedValue = PMGSYSession.Current.Language;
            mpr1.localizedValue = "en";

            return View(mpr1);
        }

        public ActionResult MPR1NewIAPReport(MPR1Model mpr1)
        {
            CommonFunctions commonFunctions = new CommonFunctions();
            //MPRModel mprModel = new MPRModel();
            //FetchCookieData fetchCookie = new FetchCookieData();
            try
            {
                if (ModelState.IsValid)
                {

                    mpr1.Mast_State_Code = mpr1.StateCode > 0 ? mpr1.StateCode : mpr1.Mast_State_Code;
                    mpr1.StateName = mpr1.StateCode == 0 ? "0" : mpr1.StateName.Trim();

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

                    //mpr1.Month = System.DateTime.Now.Month;
                    //mpr1.Year = System.DateTime.Now.Year;

                    mpr1.CollabList = commonFunctions.PopulateFundingAgency(true);
                    return View("MPR1NewIAPReport", mpr1);
                }
            }
            catch
            {
                return View(mpr1);
            }
            //return View();
        }
        #endregion

        #region CUPL Priority Report
        //[HttpGet]
        //public ActionResult CUPLPriorityLayout()
        //{
        //    CommonFunctions comm = new CommonFunctions();
        //    try
        //    {
        //        CUPLPriorityViewModel model = new CUPLPriorityViewModel();
        //        if (PMGSYSession.Current.StateCode > 0)
        //        {
        //            model.stateCode = PMGSYSession.Current.StateCode;
        //            model.mastStateCode = PMGSYSession.Current.StateCode;
        //            model.stateName = PMGSYSession.Current.StateName;

        //            List<SelectListItem> lstState = new List<SelectListItem>();
        //            //lstState.Insert(0, new SelectListItem { Value = Convert.ToString(model.stateCode), Text = Convert.ToString(model.stateName) });
        //            model.stateList = new SelectList(lstState, "Value", "Text").ToList();

        //            if (PMGSYSession.Current.DistrictCode > 0)
        //            {
        //                model.districtCode = PMGSYSession.Current.DistrictCode;
        //                model.mastDistrictCode = PMGSYSession.Current.DistrictCode;
        //                model.districtName = PMGSYSession.Current.DistrictName;

        //                List<SelectListItem> lstDist = new List<SelectListItem>();
        //                lstDist.Insert(0, new SelectListItem { Value = Convert.ToString(model.districtCode), Text = Convert.ToString(model.districtName) });
        //                model.districtList = new SelectList(lstDist, "Value", "Text").ToList();

        //                List<SelectListItem> lstBlock = new List<SelectListItem>();
        //                model.blockList = comm.PopulateBlocks(model.districtCode, true);
        //                model.blockList.RemoveAt(0);
        //                model.blockList.Insert(0, new SelectListItem { Value = "0", Text = "All Blocks" });
        //            }
        //            else
        //            {
        //                model.districtList = comm.PopulateDistrict(model.stateCode, true);
        //                model.districtList.RemoveAt(0);
        //                model.districtList.Insert(0, new SelectListItem { Value = "-1", Text = "Select District" });

        //                List<SelectListItem> lstBlock = new List<SelectListItem>();
        //                lstBlock.Insert(0, new SelectListItem { Value = "0", Text = "All Blocks" });
        //                model.blockList = new SelectList(lstBlock, "Value", "Text").ToList();
        //            }
        //        }
        //        #region
        //        else
        //        {
        //            model.stateList = comm.PopulateStates(true);
        //            model.stateList.RemoveAt(0);
        //            model.stateList.Insert(0, new SelectListItem { Value = "-1", Text = "Select State" });

        //            List<SelectListItem> lstDistricts = new List<SelectListItem>();
        //            lstDistricts.Insert(0, new SelectListItem { Value = "-1", Text = "Select District" });
        //            model.districtList = new SelectList(lstDistricts, "Value", "Text").ToList();

        //            List<SelectListItem> lstBlock = new List<SelectListItem>();
        //            lstBlock.Insert(0, new SelectListItem { Value = "0", Text = "All Blocks" });
        //            model.blockList = new SelectList(lstBlock, "Value", "Text").ToList();
        //        }
        //        #endregion
        //        model.pCategoryList = new List<SelectListItem>();
        //        model.pCategoryList.Insert(0, new SelectListItem { Value = "0", Text = "All" });
        //        model.pCategoryList.Insert(0, new SelectListItem { Value = "1", Text = "Priority I" });
        //        model.pCategoryList.Insert(0, new SelectListItem { Value = "2", Text = "Priority II" });
        //        model.pCategoryList.Insert(0, new SelectListItem { Value = "3", Text = "Priority III" });
        //        return View(model);
        //    }
        //    catch
        //    {
        //        return null;
        //    }
        //    finally
        //    {
        //        comm.Dispose();
        //    }
        //}

        //[HttpPost]
        //public ActionResult CUPLPriorityReport(CUPLPriorityViewModel model)
        //{
        //    CommonFunctions comm = new CommonFunctions();
        //    try
        //    {
        //        if (ModelState.IsValid)
        //        {
        //            model.mastStateCode = model.stateCode > 0 ? model.stateCode : model.mastStateCode;
        //            model.stateName = model.stateCode == 0 ? "All" : model.stateName.Trim();

        //            model.mastDistrictCode = model.districtCode > 0 ? model.districtCode : model.mastDistrictCode;
        //            model.districtName = model.districtCode == 0 ? "All" : model.districtName.Trim();

        //            return View(model);
        //        }
        //        else
        //        {
        //            return null;
        //        }
        //    }
        //    catch
        //    {
        //        return null;
        //    }
        //    finally
        //    {

        //    }
        //}
        #endregion

        #region Cluster Report
        [HttpGet]
        public ActionResult ClusterReportLayout()
        {
            CommonFunctions comm = new CommonFunctions();
            try
            {
                ClusterViewModel model = new ClusterViewModel();
                if (PMGSYSession.Current.StateCode > 0)
                {
                    model.stateCode = PMGSYSession.Current.StateCode;
                    //model.mastStateCode = PMGSYSession.Current.StateCode;
                    //model.stateName = PMGSYSession.Current.StateName;

                    List<SelectListItem> lstState = new List<SelectListItem>();
                    lstState.Insert(0, new SelectListItem { Value = Convert.ToString(model.stateCode), Text = Convert.ToString(PMGSYSession.Current.StateName) });
                    model.stateList = new SelectList(lstState, "Value", "Text").ToList();

                    model.districtList = comm.PopulateDistrict(PMGSYSession.Current.StateCode, true);
                    model.districtList.Find(x => x.Value == "-1").Value = "0";
                }
                #region
                else
                {
                    model.stateList = comm.PopulateStates(true);
                    model.stateList.Find(x => x.Value == "0").Value = "-1";

                    //List<SelectListItem> lstDistricts = new List<SelectListItem>();
                    //lstDistricts.Insert(0, new SelectListItem { Value = "-1", Text = "Select District" });

                    List<SelectListItem> lstDistricts = new List<SelectListItem>();
                    lstDistricts.Insert(0, new SelectListItem { Value = "-1", Text = "Select District" });
                    model.districtList = new SelectList(lstDistricts, "Value", "Text").ToList();
                }
                #endregion
                //model.pCategoryList = new List<SelectListItem>();
                //model.pCategoryList.Insert(0, new SelectListItem { Value = "0", Text = "All" });
                //model.pCategoryList.Insert(0, new SelectListItem { Value = "1", Text = "Priority I" });
                //model.pCategoryList.Insert(0, new SelectListItem { Value = "2", Text = "Priority II" });
                //model.pCategoryList.Insert(0, new SelectListItem { Value = "3", Text = "Priority III" });

                

                return View(model);
            }
            catch
            {
                return null;
            }
            finally
            {
                comm.Dispose();
            }
        }

        [HttpPost]
        public ActionResult ClusterReport(ClusterViewModel model)
        {
            CommonFunctions comm = new CommonFunctions();
            try
            {
                if (ModelState.IsValid)
                {
                    //model.mastStateCode = model.stateCode > 0 ? model.stateCode : model.mastStateCode;
                    //model.stateName = model.stateCode == 0 ? "All" : model.stateName.Trim();

                    //model.mastDistrictCode = model.districtCode > 0 ? model.districtCode : model.mastDistrictCode;
                    //model.districtName = model.districtCode == 0 ? "All" : model.districtName.Trim();

                    model.level = 1;


                    return View(model);
                }
                else
                {
                    return null;
                }
            }
            catch
            {
                return null;
            }
            finally
            {

            }
        }
        #endregion

        #region MPR1 REPORT
        public ActionResult MPR1Layout()
        {
            MPR1Model mpr1 = new MPR1Model();
            CommonFunctions commonFunctions = new CommonFunctions();

            mpr1.YearList = commonFunctions.PopulateYears(System.DateTime.Now.Year, false);
            mpr1.MonthList = commonFunctions.PopulateMonths(System.DateTime.Now.Month, false);

            mpr1.AgencyList = commonFunctions.PopulateAgencies(mpr1.StateCode, true);
            mpr1.Agency = Convert.ToInt32(mpr1.AgencyList.Where(m => m.Selected == true).Select(m => m.Value).LastOrDefault());
            //mpr1.AgencyList.RemoveAt(0);

            mpr1.YearList.RemoveAt(0);
            mpr1.MonthList.RemoveAt(0);

            mpr1.Month = System.DateTime.Now.Month;
            mpr1.Year = System.DateTime.Now.Year;

            mpr1.CollabList = commonFunctions.PopulateFundingAgency(true);
            mpr1.CollabList.Find(x => x.Value == "-1").Text = "All Collaborations";
            mpr1.CollabList.Find(x => x.Value == "-1").Value = "0";
            //mprModel.RouteList = commonFunctions.PopulateRoute();

            #region Filtering Logic
            mpr1.StateName = PMGSYSession.Current.StateCode == 0 ? "All States" : PMGSYSession.Current.StateName.Trim();
            mpr1.DistName = PMGSYSession.Current.DistrictCode == 0 ? "All Districts" : PMGSYSession.Current.DistrictName.Trim();
            //mpr1.BlockName = PMGSYSession.Current.BlockCode == 0 ? "0" : fetchCookie.BlockName.Trim();
            mpr1.Mast_State_Code = PMGSYSession.Current.StateCode;
            mpr1.Mast_District_Code = PMGSYSession.Current.DistrictCode;
            mpr1.Mast_Block_Code = 0;
            mpr1.LevelCode = PMGSYSession.Current.DistrictCode > 0 ? 2 : 1;
            mpr1.StateList = commonFunctions.PopulateStates(true);
            mpr1.StateCode = PMGSYSession.Current.StateCode == 0 ? 0 : PMGSYSession.Current.StateCode;
            mpr1.StateList.Find(x => x.Value == mpr1.StateCode.ToString()).Selected = true;
            mpr1.StateList.Find(x => x.Value == "0").Text = "All States";

            mpr1.DistrictList = new List<SelectListItem>();
            if (mpr1.StateCode == 0)
            {
                mpr1.DistrictList.Insert(0, (new SelectListItem { Text = "All Districts", Value = "0", Selected = true }));
            }
            else
            {
                mpr1.DistrictList = commonFunctions.PopulateDistrict(mpr1.StateCode, true);
                mpr1.DistrictList.Find(x => x.Value == "-1").Value = "0";
                mpr1.DistrictCode = PMGSYSession.Current.DistrictCode == 0 ? 0 : PMGSYSession.Current.DistrictCode;
                mpr1.DistrictList.Find(x => x.Value == mpr1.DistrictCode.ToString()).Selected = true;

            }
            mpr1.BlockList = new List<SelectListItem>();
            if (mpr1.DistrictCode == 0)
            {
                mpr1.BlockList.Insert(0, (new SelectListItem { Text = "All Blocks", Value = "0", Selected = true }));
            }
            else
            {
                mpr1.BlockList = commonFunctions.PopulateBlocks(mpr1.DistrictCode, true);
                mpr1.BlockCode = 0;
                mpr1.BlockList.Find(x => x.Value == mpr1.BlockCode.ToString()).Selected = true;
            }
            #endregion

            mpr1.SchemeList = new List<SelectListItem>();
            mpr1.SchemeList.Add(new SelectListItem { Value = "0", Text = "Both" });
            mpr1.SchemeList.Add(new SelectListItem { Value = "1", Text = "PMGSY1" });
            mpr1.SchemeList.Add(new SelectListItem { Value = "2", Text = "PMGSY2" });
            mpr1.Scheme = 0;

            //if (fetchCookie.Language.Contains('-'))
            //{
            //    mpr1.localizedValue = fetchCookie.Language.Substring(0, fetchCookie.Language.IndexOf('-'));
            //}
            //else
            //{
            //    mpr1.localizedValue = fetchCookie.Language;
            //}

            ViewBag.ColumnList = new SelectList(Columns.GetColumnsList(), "Id", "Name", "Category");
            return View(mpr1);
        }

        [HttpGet]
        public ActionResult GetColumns()
        {
            return Json(Columns.GetColumnsList(), JsonRequestBehavior.AllowGet);
        }

        public ActionResult MPR1Report(MPR1Model mpr1)
        {
            //MPRModel mprModel = new MPRModel();
            //FetchCookieData fetchCookie = new FetchCookieData();
            try
            {
                ModelState.Remove("localizedValue");
                if (ModelState.IsValid)
                {
                    mpr1.LevelCode = mpr1.DistrictCode > 0 ? 2 : mpr1.StateCode > 0 ? 2 : 1;
                    mpr1.Mast_State_Code = mpr1.StateCode > 0 ? mpr1.StateCode : mpr1.Mast_State_Code;
                    mpr1.Mast_District_Code = mpr1.DistrictCode > 0 ? mpr1.DistrictCode : mpr1.Mast_District_Code;


                    mpr1.StateName = mpr1.StateCode == 0 ? "0" : mpr1.StateName.Trim();
                    mpr1.DistName = mpr1.DistrictCode == 0 ? "0" : mpr1.DistName.Trim();
                    mpr1.BlockName = mpr1.BlockCode == 0 ? "0" : mpr1.BlockName.Trim();

                    return View(mpr1);
                }
                else
                {
                    string message = ModelState.Values.Where(x => x.Errors.Count == 1).FirstOrDefault().Errors[0].ErrorMessage;
                    return null;
                }
            }
            catch
            {
                return View(mpr1);
            }
            //return View();
        }
        #endregion

        #region Category wise feedback report

        public ActionResult CategorywisefeedbackreportLayout()
        {
            //QMCompletedWorkModel model = new QMCompletedWorkModel();
            CategorywiseFeedbackModel model = new CategorywiseFeedbackModel();
            try
            {
                string firstdate = "01/" + DateTime.Now.Month.ToString() + "/" + DateTime.Now.Year.ToString();
                model.FROM_DATE = Convert.ToDateTime(firstdate.Trim()).ToString("dd/MM/yyyy");
                model.TO_DATE = DateTime.Now.ToString("dd/MM/yyyy");
                return View(model);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "CategorywisefeedbackreportLayout()");
                return null;
            }
        }

        [HttpPost]
        public ActionResult Categorywisefeedbackreport(CategorywiseFeedbackModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    return View(model);
                }
                else
                {
                    string message = ModelState.Values.Where(x => x.Errors.Count == 1).FirstOrDefault().Errors[0].ErrorMessage;
                    return null;
                }
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "Categorywisefeedbackreport()");
                return null;
            }
        }
        #endregion


        #region Aspirational District Habitation Report

        public ActionResult AspirationalDistrictHabsLayout()
        {
            MPR1Model mpr1 = new MPR1Model();
            CommonFunctions commonFunctions = new CommonFunctions();
            PMGSYEntities dbContext = new PMGSYEntities();

            mpr1.YearList = commonFunctions.PopulateYears(System.DateTime.Now.Year, false);
            mpr1.MonthList = commonFunctions.PopulateMonths(System.DateTime.Now.Month, false);

            mpr1.YearList.RemoveAt(0);
            mpr1.MonthList.RemoveAt(0);

            mpr1.Month = System.DateTime.Now.Month;
            mpr1.Year = System.DateTime.Now.Year;

            #region Filtering Logic
            mpr1.StateName = PMGSYSession.Current.StateCode == 0 ? "0" : PMGSYSession.Current.StateName.Trim();
            mpr1.Mast_State_Code = PMGSYSession.Current.StateCode;

            mpr1.StateList = commonFunctions.PopulateStates(true);
            mpr1.StateCode = PMGSYSession.Current.StateCode == 0 ? 0 : PMGSYSession.Current.StateCode;
            mpr1.StateList.Find(x => x.Value == mpr1.StateCode.ToString()).Selected = true;
            mpr1.StateList.Find(x => x.Value == "0").Text = "All States";


            #endregion


            mpr1.localizedValue = PMGSYSession.Current.Language;
            mpr1.localizedValue = "en";

            return View(mpr1);
        }

        public ActionResult AspirationalDistrictHabsReport(MPR1Model mpr1)
        {
            CommonFunctions commonFunctions = new CommonFunctions();
            //MPRModel mprModel = new MPRModel();
            //FetchCookieData fetchCookie = new FetchCookieData();
            try
            {
                ModelState.Remove("Collab");
                ModelState.Remove("Agency");
                ModelState.Remove("localizedValue");
                ModelState.Remove("DistrictCode");
                ModelState.Remove("BlockCode");

                if (ModelState.IsValid)
                {

                    mpr1.Mast_State_Code = mpr1.StateCode > 0 ? mpr1.StateCode : mpr1.Mast_State_Code;
                    mpr1.StateName = mpr1.StateCode == 0 ? "All States" : new PMGSYEntities().MASTER_STATE.Where(x => x.MAST_STATE_CODE == mpr1.StateCode).First().MAST_STATE_NAME;

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

                    return View("AspirationalDistrictHabsLayout", mpr1);
                }
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "Feedback.AspirationalDistrictHabsReport");
                return View(mpr1);
            }
            //return View();
        }



        public ActionResult AspirationalDistrictPhysicalLayout()
        {
            MPR1Model mpr1 = new MPR1Model();
            CommonFunctions commonFunctions = new CommonFunctions();
            PMGSYEntities dbContext = new PMGSYEntities();

            mpr1.YearList = commonFunctions.PopulateYears(System.DateTime.Now.Year, false);
            mpr1.MonthList = commonFunctions.PopulateMonths(System.DateTime.Now.Month, false);

            mpr1.YearList.RemoveAt(0);
            mpr1.MonthList.RemoveAt(0);

            mpr1.Month = System.DateTime.Now.Month;
            mpr1.Year = System.DateTime.Now.Year;

            #region Filtering Logic
            mpr1.StateName = PMGSYSession.Current.StateCode == 0 ? "0" : PMGSYSession.Current.StateName.Trim();
            mpr1.Mast_State_Code = PMGSYSession.Current.StateCode;

            mpr1.StateList = commonFunctions.PopulateStates(true);
            mpr1.StateCode = PMGSYSession.Current.StateCode == 0 ? 0 : PMGSYSession.Current.StateCode;
            mpr1.StateList.Find(x => x.Value == mpr1.StateCode.ToString()).Selected = true;
            mpr1.StateList.Find(x => x.Value == "0").Text = "All States";


            #endregion


            mpr1.localizedValue = PMGSYSession.Current.Language;
            mpr1.localizedValue = "en";

            return View(mpr1);
        }

        public ActionResult AspirationalDistrictPhysicalReport(MPR1Model mpr1)
        {
            CommonFunctions commonFunctions = new CommonFunctions();
            //MPRModel mprModel = new MPRModel();
            //FetchCookieData fetchCookie = new FetchCookieData();
            try
            {
                ModelState.Remove("Collab");
                ModelState.Remove("Agency");
                ModelState.Remove("localizedValue");
                ModelState.Remove("DistrictCode");
                ModelState.Remove("BlockCode");

                if (ModelState.IsValid)
                {

                    mpr1.Mast_State_Code = mpr1.StateCode > 0 ? mpr1.StateCode : mpr1.Mast_State_Code;
                    mpr1.StateName = mpr1.StateCode == 0 ? "All States" : new PMGSYEntities().MASTER_STATE.Where(x => x.MAST_STATE_CODE == mpr1.StateCode).First().MAST_STATE_NAME;

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

                    return View("AspirationalDistrictHabsLayout", mpr1);
                }
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "Feedback.AspirationalDistrictHabsReport");
                return View(mpr1);
            }
            //return View();
        }

        #endregion


        //AREA->FEEDBACK->FEEDBACK
        #region LGD MIS DATA report

        public ActionResult LDGLayout()
        {
            //QMCompletedWorkModel model = new QMCompletedWorkModel();
            CategorywiseFeedbackModel model = new CategorywiseFeedbackModel();
            try
            {
                string firstdate = "01/" + DateTime.Now.Month.ToString() + "/" + DateTime.Now.Year.ToString();
                model.FROM_DATE = Convert.ToDateTime(firstdate.Trim()).ToString("dd/MM/yyyy");

                return View(model);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "CategorywisefeedbackreportLayout()");
                return null;
            }
        }

        [HttpPost]
        public ActionResult LDGreport(CategorywiseFeedbackModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    // Convert.ToDateTime(model.FROM_DATE).ToString("yyyy-MM-dd");

                    return View(model);
                }
                else
                {
                    string message = ModelState.Values.Where(x => x.Errors.Count == 1).FirstOrDefault().Errors[0].ErrorMessage;
                    return null;
                }
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "Categorywisefeedbackreport()");
                return null;
            }
        }
        #endregion





        #region State Brief Details
        [HttpGet]
        public ActionResult StateLayout()
        {
            QualityReportsViewModel QM = new QualityReportsViewModel();
            CommonFunctions comm = new CommonFunctions();
            try
            {

                QM.State = PMGSYSession.Current.StateCode;
                QM.StateList = comm.PopulateStates(true);
                QM.StateList.RemoveAt(0);
                QM.StateList.Insert(0, (new SelectListItem { Text = "Select State", Value = "0", Selected = true }));



                return View(QM);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return null;
            }
            finally
            {
                if (comm != null)
                {
                    comm.Dispose();
                }
            }
        }


        [HttpPost]
        public ActionResult StateReport(QualityReportsViewModel QM)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (PMGSYSession.Current.StateName != null)
                    {
                        QM.StateName = PMGSYSession.Current.StateName;
                    }
                    else
                    {
                        QM.StateName = QM.State == 0 ? "All States" : QM.StateName;
                    }
                    return View(QM);
                }
                else
                {
                    string message = ModelState.Values.Where(x => x.Errors.Count == 1).FirstOrDefault().Errors[0].ErrorMessage;
                    return null;
                }
            }
            catch
            {
                return null;
            }
        }
        #endregion State Brief Details



        #region District Brief Report
        [HttpGet]
        public ActionResult DistrictBriefLayout()
        {
            CommonFunctions comm = new CommonFunctions();
            try
            {
                ClusterViewModel model = new ClusterViewModel();
                if (PMGSYSession.Current.StateCode > 0)
                {
                    model.stateCode = PMGSYSession.Current.StateCode;
                    //model.mastStateCode = PMGSYSession.Current.StateCode;
                    //model.stateName = PMGSYSession.Current.StateName;

                    List<SelectListItem> lstState = new List<SelectListItem>();
                    lstState.Insert(0, new SelectListItem { Value = Convert.ToString(model.stateCode), Text = Convert.ToString(PMGSYSession.Current.StateName) });
                    model.stateList = new SelectList(lstState, "Value", "Text").ToList();

                    model.districtList = comm.PopulateDistrict(PMGSYSession.Current.StateCode, true);
                    model.districtList.Find(x => x.Value == "-1").Value = "0";
                }
                #region
                else
                {
                    model.stateList = comm.PopulateStates(true);
                    model.stateList.Find(x => x.Value == "0").Value = "-1";

                    //List<SelectListItem> lstDistricts = new List<SelectListItem>();
                    //lstDistricts.Insert(0, new SelectListItem { Value = "-1", Text = "Select District" });

                    List<SelectListItem> lstDistricts = new List<SelectListItem>();
                    lstDistricts.Insert(0, new SelectListItem { Value = "-1", Text = "Select District" });
                    model.districtList = new SelectList(lstDistricts, "Value", "Text").ToList();
                }
                #endregion
                //model.pCategoryList = new List<SelectListItem>();
                //model.pCategoryList.Insert(0, new SelectListItem { Value = "0", Text = "All" });
                //model.pCategoryList.Insert(0, new SelectListItem { Value = "1", Text = "Priority I" });
                //model.pCategoryList.Insert(0, new SelectListItem { Value = "2", Text = "Priority II" });
                //model.pCategoryList.Insert(0, new SelectListItem { Value = "3", Text = "Priority III" });



                return View(model);
            }
            catch
            {
                return null;
            }
            finally
            {
                comm.Dispose();
            }
        }


        public ActionResult DistrictDetails(FormCollection frmCollection)
        {
            CommonFunctions objCommonFunctions = new CommonFunctions();
            List<SelectListItem> list = objCommonFunctions.PopulateDistrict(Convert.ToInt32(frmCollection["StateCode"]), false);
         //   list.Find(x => x.Value == "-1").Value = "0";
            return Json(list, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public ActionResult DistrictBriefReport(ClusterViewModel model)
        {
            CommonFunctions comm = new CommonFunctions();
            try
            {
                if (ModelState.IsValid)
                {
                    //model.mastStateCode = model.stateCode > 0 ? model.stateCode : model.mastStateCode;
                    //model.stateName = model.stateCode == 0 ? "All" : model.stateName.Trim();

                    //model.mastDistrictCode = model.districtCode > 0 ? model.districtCode : model.mastDistrictCode;
                    //model.districtName = model.districtCode == 0 ? "All" : model.districtName.Trim();

                    model.level = 1;


                    return View(model);
                }
                else
                {
                    return null;
                }
            }
            catch
            {
                return null;
            }
            finally
            {

            }
        }
        #endregion




        #region New State Brief Details
        [HttpGet]
        public ActionResult NewStateLayout()
        {
            QualityReportsViewModel QM = new QualityReportsViewModel();
            CommonFunctions comm = new CommonFunctions();
            try
            {

                QM.State = PMGSYSession.Current.StateCode;
                QM.StateList = comm.PopulateStates(true);
                QM.StateList.RemoveAt(0);
                QM.StateList.Insert(0, (new SelectListItem { Text = "Select State", Value = "0", Selected = true }));



                return View(QM);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return null;
            }
            finally
            {
                if (comm != null)
                {
                    comm.Dispose();
                }
            }
        }


        [HttpPost]
        public ActionResult NewStateReport(QualityReportsViewModel QM)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (PMGSYSession.Current.StateName != null)
                    {
                        QM.StateName = PMGSYSession.Current.StateName;
                    }
                    else
                    {
                        QM.StateName = QM.State == 0 ? "All States" : QM.StateName;
                    }
                    return View(QM);
                }
                else
                {
                    string message = ModelState.Values.Where(x => x.Errors.Count == 1).FirstOrDefault().Errors[0].ErrorMessage;
                    return null;
                }
            }
            catch
            {
                return null;
            }
        }
        #endregion State Brief Details

    }

    public class Columns
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Category { get; set; }
        public int Ancestor { get; set; }
        public int Parent { get; set; }
        public int Child { get; set; }

        public static List<Columns> GetColumnsList()
        {
            return new List<Columns>{

            //new Car{Id=0, Name="", Category="Swedish Cars", Parent=1, Child=0},
            //new Car{Id=1, Name="Volvo", Category="Swedish Cars",Parent=1,  Child=1},
            //new Car{Id=2, Name="Saab", Category="Swedish Cars",Parent=1,  Child=1},
            //new Car{Id=0, Name="", Category="German Cars", Parent=2, Child=0},
            //new Car{Id=3, Name="Mercedes-Benz", Category="German Cars",Parent=2,  Child=2},
            //new Car{Id=4, Name="Audi", Category="German Cars",Parent=2,  Child=2},
            //new Car{Id=5, Name="BMW", Category="German Cars",Parent=2,  Child=2},

            new Columns{Id="0", Name="", Category="Clearance", Ancestor=1,  Parent=1, Child=0},
            
            new Columns{Id="0", Name="", Category="New Connectivity", Ancestor=0,  Parent=1, Child=0},
            new Columns{Id="01", Name="Numbers", Category="New Connectivity",Ancestor=0,  Parent=1,  Child=1},
            new Columns{Id="02", Name="Length", Category="New Connectivity",Ancestor=0,  Parent=1,  Child=1},
            new Columns{Id="03", Name="Cost", Category="New Connectivity",Ancestor=0,  Parent=1,  Child=1},
            new Columns{Id="04", Name="1000+", Category="New Connectivity",Ancestor=0,  Parent=1,  Child=1},
            new Columns{Id="05", Name="999-500", Category="New Connectivity",Ancestor=0,  Parent=1,  Child=1},
            new Columns{Id="06", Name="499-250(Eligible)", Category="New Connectivity",Ancestor=0,  Parent=1,  Child=1},
            new Columns{Id="07", Name="< 250(Incidental)", Category="New Connectivity",Ancestor=0,  Parent=1,  Child=1},

            new Columns{Id="0", Name="", Category="Upgradation", Ancestor=0,  Parent=2, Child=0},
            new Columns{Id="08", Name="Numbers", Category="Upgradation",Ancestor=0,  Parent=2,  Child=2},
            new Columns{Id="09", Name="Length", Category="Upgradation",Ancestor=0,  Parent=2,  Child=2},
            new Columns{Id="10", Name="Cost", Category="Upgradation",Ancestor=0,  Parent=2,  Child=2},

            new Columns{Id="0", Name="", Category="Total", Ancestor=0,  Parent=3, Child=0},
            new Columns{Id="11", Name="LSB Numbers", Category="Total",Ancestor=0,  Parent=3,  Child=3},
            new Columns{Id="12", Name="Road Numbers", Category="Total",Ancestor=0,  Parent=3,  Child=3},
            new Columns{Id="13", Name="Road Length", Category="Total",Ancestor=0,  Parent=3,  Child=3},
            new Columns{Id="14", Name="Cost", Category="Total",Ancestor=0,  Parent=3,  Child=3},
            
            new Columns{Id="15", Name="Funds Released", Category="FundsReleased", Ancestor=0,  Parent=1, Child=1},

            new Columns{Id="0", Name="", Category="Progress During Month", Ancestor=2,  Parent=1, Child=0},
            
            new Columns{Id="0", Name="", Category="New Connectivity ", Ancestor=0,  Parent=1, Child=0},
            new Columns{Id="16", Name="Numbers", Category="New Connectivity",Ancestor=0,  Parent=1,  Child=1},
            new Columns{Id="17", Name="Length", Category="New Connectivity",Ancestor=0,  Parent=1,  Child=1},
            new Columns{Id="18", Name="Expenditure", Category="New Connectivity",Ancestor=0,  Parent=1,  Child=1},
            new Columns{Id="19", Name="1000+", Category="New Connectivity",Ancestor=0,  Parent=1,  Child=1},
            new Columns{Id="20", Name="999-500", Category="New Connectivity",Ancestor=0,  Parent=1,  Child=1},
            new Columns{Id="21", Name="499-250(Eligible)", Category="New Connectivity",Ancestor=0,  Parent=1,  Child=1},
            new Columns{Id="22", Name="< 250(Incidental)", Category="New Connectivity",Ancestor=0,  Parent=1,  Child=1},
            
            new Columns{Id="0", Name="", Category="Upgradation ", Ancestor=0,  Parent=2, Child=0},
            new Columns{Id="23", Name="Numbers", Category="Upgradation",Ancestor=0,  Parent=2,  Child=2},
            new Columns{Id="24", Name="Length", Category="Upgradation",Ancestor=0,  Parent=2,  Child=2},
            new Columns{Id="25", Name="Expenditure", Category="Upgradation",Ancestor=0,  Parent=2,  Child=2},

            new Columns{Id="0", Name="", Category="Total ", Ancestor=0,  Parent=3, Child=0},
            new Columns{Id="26", Name="Numbers", Category="Total",Ancestor=0,  Parent=3,  Child=3},
            new Columns{Id="27", Name="Length", Category="Total",Ancestor=0,  Parent=3,  Child=3},
            new Columns{Id="28", Name="Expenditure", Category="Total",Ancestor=0,  Parent=3,  Child=3},

            new Columns{Id="0", Name="", Category="Cumulative Progress", Ancestor=3,  Parent=1, Child=0},
            
            new Columns{Id="0", Name="", Category=" New Connectivity", Ancestor=0,  Parent=1, Child=0},
            new Columns{Id="29", Name="Numbers", Category="New Connectivity",Ancestor=0,  Parent=1,  Child=1},
            new Columns{Id="30", Name="Length", Category="New Connectivity",Ancestor=0,  Parent=1,  Child=1},
            new Columns{Id="31", Name="Expenditure", Category="New Connectivity",Ancestor=0,  Parent=1,  Child=1},
            new Columns{Id="32", Name="1000+", Category="New Connectivity",Ancestor=0,  Parent=1,  Child=1},
            new Columns{Id="33", Name="999-500", Category="New Connectivity",Ancestor=0,  Parent=1,  Child=1},
            new Columns{Id="34", Name="499-250(Eligible)", Category="New Connectivity",Ancestor=0,  Parent=1,  Child=1},
            new Columns{Id="35", Name="< 250(Incidental)", Category="New Connectivity",Ancestor=0,  Parent=1,  Child=1},
            
            new Columns{Id="0", Name="", Category=" Upgradation", Ancestor=0,  Parent=2, Child=0},
            new Columns{Id="36", Name="Numbers", Category="Upgradation",Ancestor=0,  Parent=3,  Child=3},
            new Columns{Id="37", Name="Length", Category="Upgradation",Ancestor=0,  Parent=3,  Child=3},
            new Columns{Id="38", Name="Expenditure", Category="Upgradation",Ancestor=0,  Parent=3,  Child=3},

            new Columns{Id="0", Name="", Category=" Total", Ancestor=0,  Parent=3, Child=0},
            new Columns{Id="39", Name="LSB Numbers", Category="Total",Ancestor=0,  Parent=3,  Child=3},
            new Columns{Id="40", Name="Road Numbers", Category="Total",Ancestor=0,  Parent=3,  Child=3},
            new Columns{Id="41", Name="Road Length", Category="Total",Ancestor=0,  Parent=3,  Child=3},
            new Columns{Id="42", Name="Expenditure", Category="Total",Ancestor=0,  Parent=3,  Child=3},
            };
        }
    }
}
