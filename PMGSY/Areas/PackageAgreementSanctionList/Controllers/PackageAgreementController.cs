using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PMGSY.Areas.PackageAgreementSanctionList.Models;
using PMGSY.Common;
using PMGSY.Models;
using PMGSY.Extensions;
using PMGSY.Controllers;

namespace PMGSY.Areas.PackageAgreementSanctionList.Controllers
{
    [RequiredAuthentication]
    [RequiredAuthorization]
    public class PackageAgreementController : Controller
    {
        //
        // GET: /PackageAgreementSanctionList/PackageAgreement/

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult PackageAgreementLayout()
        {
            PackageAgreementViewModel packageAgreement = new PackageAgreementViewModel();
            CommonFunctions commonFunctions = new CommonFunctions();

            packageAgreement.StateName = PMGSYSession.Current.StateCode == 0 ? "0" : PMGSYSession.Current.StateName.Trim();

            packageAgreement.DistName = PMGSYSession.Current.DistrictCode == 0 ? "0" : PMGSYSession.Current.DistrictName.Trim();



            packageAgreement.Mast_State_Code = PMGSYSession.Current.StateCode;

            packageAgreement.Mast_District_Code = PMGSYSession.Current.DistrictCode;

            packageAgreement.DistrictCode = PMGSYSession.Current.DistrictCode;


            packageAgreement.StateList = commonFunctions.PopulateStates(true);


            packageAgreement.StateList.Find(x => x.Value == packageAgreement.StateCode.ToString()).Selected = true;

            packageAgreement.DistrictList = new List<SelectListItem>();
            if (packageAgreement.StateCode == 0)
            {
                packageAgreement.DistrictList.Insert(0, (new SelectListItem { Text = "All Districts", Value = "0", Selected = true }));
            }
            else
            {
                packageAgreement.DistrictList = commonFunctions.PopulateDistrict(packageAgreement.StateCode, true);
                packageAgreement.DistrictCode = PMGSYSession.Current.DistrictCode == 0 ? 0 : PMGSYSession.Current.DistrictCode;
                packageAgreement.DistrictList.Find(x => x.Value == packageAgreement.DistrictCode.ToString()).Selected = true;

            }
            packageAgreement.BlockList = new List<SelectListItem>();
            if (packageAgreement.DistrictCode == 0)
            {
                packageAgreement.BlockList.Insert(0, (new SelectListItem { Text = "All Blocks", Value = "0", Selected = true }));
            }
            else
            {
                packageAgreement.BlockList = commonFunctions.PopulateBlocks(packageAgreement.DistrictCode, true);

                packageAgreement.BlockList.Find(x => x.Value == "-1").Value = "0";
                packageAgreement.BlockList.Find(x => x.Value == packageAgreement.BlockCode.ToString()).Selected = true;
            }

            packageAgreement.Year = DateTime.Now.Year;
          
            packageAgreement.YearList = commonFunctions.PopulateFinancialYear(true,true).ToList();



            return View(packageAgreement);

        }

        /// <summary>
        /// Render data in Completed Roads Report
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult PackageAgreementReport(PackageAgreementViewModel packageAgreement)
        {

            try
            {
                if (ModelState.IsValid)
                {

                    packageAgreement.Mast_State_Code = packageAgreement.StateCode > 0 ? packageAgreement.StateCode : packageAgreement.Mast_State_Code;
                    packageAgreement.Mast_District_Code = packageAgreement.DistrictCode > 0 ? packageAgreement.DistrictCode : packageAgreement.Mast_District_Code;
                    packageAgreement.Mast_Block_Code = packageAgreement.BlockCode > 0 ? packageAgreement.BlockCode : packageAgreement.Mast_Block_Code;

                    return View(packageAgreement);
                }
                else
                {   string message = ModelState.Values.Where(x => x.Errors.Count == 1).FirstOrDefault().Errors[0].ErrorMessage;
                    return Json(string.Empty);
                
                }
            }
            catch
            {
                return View(packageAgreement);
            }

        }

        #region Common function
        public ActionResult DistrictDetails(FormCollection frmCollection)
        {
            CommonFunctions objCommonFunctions = new CommonFunctions();
            List<SelectListItem> list = objCommonFunctions.PopulateDistrict(Convert.ToInt32(frmCollection["StateCode"]), true);
            list.Find(x => x.Value == "-1").Value = "0";
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public ActionResult BlockDetails(FormCollection frmCollection)
        {
            CommonFunctions objCommonFunctions = new CommonFunctions();
            List<SelectListItem> list = objCommonFunctions.PopulateBlocks(Convert.ToInt32(frmCollection["DistrictCode"]), true);
            list.Find(x => x.Value == "-1").Value = "0";
            return Json(list, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Wrongly Awarded Works
        public ActionResult WronglyAwardedWorksLayout()
        {
            WronglyAwardedWorksViewModel model = new WronglyAwardedWorksViewModel();
            CommonFunctions comm = new CommonFunctions();
            try
            {
                model.lstState = comm.PopulateStates(true);
                model.lstState.Find(x => x.Value == "0").Value = "-1";
                model.lstDistrict = new List<SelectListItem>();
                model.lstDistrict.Insert(0, new SelectListItem { Text = "All Districts", Value = "0" });

                //model.lstYear = comm.PopulateYears(false);
                model.pmgsyScheme = PMGSYSession.Current.PMGSYScheme;
                return View(model);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "Areas.PackageAgreement.WronglyAwardedWorksLayout()");
                return null;
            }
        }

        public ActionResult WronglyAwardedWorksReport(WronglyAwardedWorksViewModel model)
        {
            try
            {
                return View(model);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "Areas.PackageAgreement.WronglyAwardedWorksReport()");
                return null;
            }
        }
        #endregion

        #region Emarg

        public ActionResult EmargLayout()
        {
            PackageAgreementViewModel packageAgreement = new PackageAgreementViewModel();
            CommonFunctions commonFunctions = new CommonFunctions();

            packageAgreement.StateName = PMGSYSession.Current.StateCode == 0 ? "0" : PMGSYSession.Current.StateName.Trim();

            packageAgreement.DistName = PMGSYSession.Current.DistrictCode == 0 ? "0" : PMGSYSession.Current.DistrictName.Trim();



            packageAgreement.Mast_State_Code = PMGSYSession.Current.StateCode;

            packageAgreement.Mast_District_Code = PMGSYSession.Current.DistrictCode;

            packageAgreement.DistrictCode = PMGSYSession.Current.DistrictCode;


            packageAgreement.StateList = commonFunctions.PopulateStates(true);


            packageAgreement.StateList.Find(x => x.Value == packageAgreement.StateCode.ToString()).Selected = true;

            packageAgreement.DistrictList = new List<SelectListItem>();
            if (packageAgreement.StateCode == 0 && PMGSYSession.Current.RoleCode != 2)
            {
                packageAgreement.DistrictList.Insert(0, (new SelectListItem { Text = "All Districts", Value = "0", Selected = true }));
            }
            else
            {
                packageAgreement.DistrictList = commonFunctions.PopulateDistrictForSRRDA(PMGSYSession.Current.StateCode, true); 
            //    packageAgreement.DistrictCode = PMGSYSession.Current.DistrictCode == 0 ? 0 : PMGSYSession.Current.DistrictCode;
             //   packageAgreement.DistrictList.Find(x => x.Value == packageAgreement.DistrictCode.ToString()).Selected = true;

            }
            packageAgreement.BlockList = new List<SelectListItem>();
            if (packageAgreement.DistrictCode == 0)
            {
                packageAgreement.BlockList.Insert(0, (new SelectListItem { Text = "All Blocks", Value = "0", Selected = true }));
            }
            else
            {
                packageAgreement.BlockList = commonFunctions.PopulateBlocks(packageAgreement.DistrictCode, true);

                packageAgreement.BlockList.Find(x => x.Value == "-1").Value = "0";
                packageAgreement.BlockList.Find(x => x.Value == packageAgreement.BlockCode.ToString()).Selected = true;
            }

            packageAgreement.Year = DateTime.Now.Year;

            packageAgreement.YearList = commonFunctions.PopulateFinancialYear(true, true).ToList();



            return View(packageAgreement);

        }

        /// <summary>
        /// Render data in Completed Roads Report
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult EmargReport(PackageAgreementViewModel packageAgreement)
        { 
            try
            {
                if (ModelState.IsValid)
                {   packageAgreement.Mast_State_Code = packageAgreement.StateCode > 0 ? packageAgreement.StateCode : packageAgreement.Mast_State_Code;
                    packageAgreement.Mast_District_Code = packageAgreement.DistrictCode > 0 ? packageAgreement.DistrictCode : packageAgreement.Mast_District_Code;
                    packageAgreement.Mast_Block_Code = packageAgreement.BlockCode > 0 ? packageAgreement.BlockCode : packageAgreement.Mast_Block_Code;
                    packageAgreement.YearString = packageAgreement.Year == 0 ? "0" : (packageAgreement.Year + "-" + (packageAgreement.Year + 1));
                    return View(packageAgreement);
                }
                else
                {
                    string message = ModelState.Values.Where(x => x.Errors.Count == 1).FirstOrDefault().Errors[0].ErrorMessage;
                    return Json(string.Empty);
                }
            }
            catch
            {
                return View(packageAgreement);
            }

        }

        #endregion Emarg

        #region Facility Details 

        public ActionResult FacilityLayout()
        {
            PackageAgreementViewModel packageAgreement = new PackageAgreementViewModel();
            CommonFunctions commonFunctions = new CommonFunctions();

            packageAgreement.StateName = PMGSYSession.Current.StateCode == 0 ? "0" : PMGSYSession.Current.StateName.Trim();

            packageAgreement.DistName = PMGSYSession.Current.DistrictCode == 0 ? "0" : PMGSYSession.Current.DistrictName.Trim();



            packageAgreement.Mast_State_Code = PMGSYSession.Current.StateCode;

            packageAgreement.Mast_District_Code = PMGSYSession.Current.DistrictCode;

            packageAgreement.DistrictCode = PMGSYSession.Current.DistrictCode;


            packageAgreement.StateList = commonFunctions.PopulateStates(true);


            packageAgreement.StateList.Find(x => x.Value == packageAgreement.StateCode.ToString()).Selected = true;

            packageAgreement.DistrictList = new List<SelectListItem>();
            if (packageAgreement.StateCode == 0 && PMGSYSession.Current.RoleCode!=2)
            {
                packageAgreement.DistrictList.Insert(0, (new SelectListItem { Text = "All Districts", Value = "0", Selected = true }));
            }
            else
            {
                packageAgreement.DistrictList = commonFunctions.PopulateDistrictForSRRDA(PMGSYSession.Current.StateCode, true);
           //     packageAgreement.DistrictCode = PMGSYSession.Current.DistrictCode == 0 ? 0 : PMGSYSession.Current.DistrictCode;
           //     packageAgreement.DistrictList.Find(x => x.Value == packageAgreement.DistrictCode.ToString()).Selected = true;

            }
            packageAgreement.BlockList = new List<SelectListItem>();
            if (packageAgreement.DistrictCode == 0)
            {
                packageAgreement.BlockList.Insert(0, (new SelectListItem { Text = "All Blocks", Value = "0", Selected = true }));
            }
            else
            {
                packageAgreement.BlockList = commonFunctions.PopulateBlocks(packageAgreement.DistrictCode, true);

                packageAgreement.BlockList.Find(x => x.Value == "-1").Value = "0";
                packageAgreement.BlockList.Find(x => x.Value == packageAgreement.BlockCode.ToString()).Selected = true;
            }

            packageAgreement.Year = DateTime.Now.Year;

            packageAgreement.YearList = commonFunctions.PopulateFinancialYear(true, true).ToList();



            return View(packageAgreement);

        }

        /// <summary>
        /// Render data in Completed Roads Report
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult FacilityReport(PackageAgreementViewModel packageAgreement)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    packageAgreement.Mast_State_Code = packageAgreement.StateCode > 0 ? packageAgreement.StateCode : packageAgreement.Mast_State_Code;
                    packageAgreement.Mast_District_Code = packageAgreement.DistrictCode > 0 ? packageAgreement.DistrictCode : packageAgreement.Mast_District_Code;
                    packageAgreement.Mast_Block_Code = packageAgreement.BlockCode > 0 ? packageAgreement.BlockCode : packageAgreement.Mast_Block_Code;
                    packageAgreement.YearString = packageAgreement.Year == 0 ? "0" : (packageAgreement.Year + "-" + (packageAgreement.Year + 1));
                    return View(packageAgreement);
                }
                else
                {
                    string message = ModelState.Values.Where(x => x.Errors.Count == 1).FirstOrDefault().Errors[0].ErrorMessage;
                    return Json(string.Empty);
                }
            }
            catch
            {
                return View(packageAgreement);
            }

        }
        #endregion


        #region AGR Chart
        public ActionResult AGRLayout()
        {
            AGRChart packageAgreement = new AGRChart();
            CommonFunctions commonFunctions = new CommonFunctions();

            packageAgreement.StateName = PMGSYSession.Current.StateCode == 0 ? "0" : PMGSYSession.Current.StateName.Trim();

            packageAgreement.DistName = PMGSYSession.Current.DistrictCode == 0 ? "0" : PMGSYSession.Current.DistrictName.Trim();

            packageAgreement.Mast_State_Code = PMGSYSession.Current.StateCode;

            packageAgreement.Mast_District_Code = PMGSYSession.Current.DistrictCode;

            packageAgreement.DistrictCode = PMGSYSession.Current.DistrictCode;

            packageAgreement.StateList = commonFunctions.PopulateStates(false);


            //    packageAgreement.StateList.Find(x => x.Value == packageAgreement.StateCode.ToString()).Selected = true;

            packageAgreement.DistrictList = new List<SelectListItem>();
            if (packageAgreement.StateCode == 0 && PMGSYSession.Current.RoleCode != (int)PMGSY.Common.CommonFunctions.PMGSYSessionRoleDetails.PIU)
            {
                packageAgreement.DistrictList.Insert(0, (new SelectListItem { Text = "All Districts", Value = "0", Selected = true }));
            }
            else
            {
                packageAgreement.DistrictList = commonFunctions.PopulateDistrictForSRRDA(PMGSYSession.Current.StateCode, true);

            }
            packageAgreement.BlockList = new List<SelectListItem>();
            if (packageAgreement.DistrictCode == 0)
            {
                packageAgreement.BlockList.Insert(0, (new SelectListItem { Text = "All Blocks", Value = "0", Selected = true }));
            }
            else
            {
                packageAgreement.BlockList = commonFunctions.PopulateBlocks(packageAgreement.DistrictCode, true);

                packageAgreement.BlockList.Find(x => x.Value == "-1").Value = "0";
                packageAgreement.BlockList.Find(x => x.Value == packageAgreement.BlockCode.ToString()).Selected = true;
            }

            packageAgreement.Year = DateTime.Now.Year;

            packageAgreement.YearList = commonFunctions.PopulateFinancialYear(true, true).ToList();

            return View(packageAgreement);

        }


        [HttpPost]
        [Audit]
        public ActionResult ChartAGR()
        {
            try
            {


                List<USP_AGR_CHART_Result> List = ChartAGRDetails(Convert.ToInt32(Request.Params["LevelCode"]), Convert.ToInt32(Request.Params["StateCode"]), Convert.ToInt32(Request.Params["DistrictCode"]), Convert.ToInt32(Request.Params["Pmgsy"]), Convert.ToChar(Request.Params["WorkType"]));

                List<AGRChartFields> lstChart = new List<AGRChartFields>();

                foreach (var p in List)
                {
                    AGRChartFields chart = new AGRChartFields();
                    // chart.Year = p.IMS_YEAR.ToString();
                    chart.SanctionWorks = p.WORKS_SANCTIONED.ToString(); //Total Sactioned Works
                    chart.AwardedWorks = p.WORKS_AGREEMENTED.ToString(); // Total Works Awarded
                    chart.UnawardedWorks = p.WORKS_NOT_AWARDED.ToString(); // Total Unawarded Works
                    chart.TerminatedWorks = p.WORKS_TEMINATED.ToString(); // Works Awarded & Terminated

                    chart.CompletedWorks = p.COMPLETED_WORKS.ToString(); // Completed Works
                    chart.ProgressWorks = p.PROGRESS_WORKS.ToString(); // Progress Works

                    chart.LessThanYear_1 = p.LESS_1_YR.ToString(); // Less Than Year 1 
                    chart.Year_1_To_2 = p.YR_1_2.ToString();// Between Year 1 and 2
                    chart.Year_2_To_3 = p.YR_2_3.ToString();// Between Year 2 and 3
                    chart.GreaterThanYear_3 = p.YR_3_GREAT.ToString();// Greater Than Year 3

                    lstChart.Add(chart);
                }

                return new JsonResult
                {
                    Data = lstChart
                };
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "DashboardController.WorksColumnChart()");
                return null;
            }
        }


        public List<USP_AGR_CHART_Result> ChartAGRDetails(int Level, int State, int District, int Pmgsy, char WorkType)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            try
            {
                var resultlist = dbContext.USP_AGR_CHART(Level, State, District, 0, "%").ToList<USP_AGR_CHART_Result>();
                return resultlist;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        #endregion

        #region Habitation , Facility and DRRP CSV Download Report


        public ActionResult HabLayout()
        {
            PackageAgreementViewModel packageAgreement = new PackageAgreementViewModel();
            CommonFunctions commonFunctions = new CommonFunctions();

            packageAgreement.StateName = PMGSYSession.Current.StateCode == 0 ? "0" : PMGSYSession.Current.StateName.Trim();

            packageAgreement.DistName = PMGSYSession.Current.DistrictCode == 0 ? "0" : PMGSYSession.Current.DistrictName.Trim();



            packageAgreement.Mast_State_Code = PMGSYSession.Current.StateCode;

            packageAgreement.Mast_District_Code = PMGSYSession.Current.DistrictCode;

            packageAgreement.DistrictCode = PMGSYSession.Current.DistrictCode;


            packageAgreement.StateList = commonFunctions.PopulateStates(true);


            packageAgreement.StateList.Find(x => x.Value == packageAgreement.StateCode.ToString()).Selected = true;

            packageAgreement.DistrictList = new List<SelectListItem>();
            if (packageAgreement.StateCode == 0 && PMGSYSession.Current.RoleCode != 2)
            {
                packageAgreement.DistrictList.Insert(0, (new SelectListItem { Text = "All Districts", Value = "0", Selected = true }));
            }
            else
            {
                packageAgreement.DistrictList = commonFunctions.PopulateDistrictForSRRDA(PMGSYSession.Current.StateCode, true);
                //     packageAgreement.DistrictCode = PMGSYSession.Current.DistrictCode == 0 ? 0 : PMGSYSession.Current.DistrictCode;
                //     packageAgreement.DistrictList.Find(x => x.Value == packageAgreement.DistrictCode.ToString()).Selected = true;

            }
            packageAgreement.BlockList = new List<SelectListItem>();
            if (packageAgreement.DistrictCode == 0)
            {
                packageAgreement.BlockList.Insert(0, (new SelectListItem { Text = "All Blocks", Value = "0", Selected = true }));
            }
            else
            {
                packageAgreement.BlockList = commonFunctions.PopulateBlocks(packageAgreement.DistrictCode, true);

                packageAgreement.BlockList.Find(x => x.Value == "-1").Value = "0";
                packageAgreement.BlockList.Find(x => x.Value == packageAgreement.BlockCode.ToString()).Selected = true;
            }

            packageAgreement.Year = DateTime.Now.Year;

            packageAgreement.YearList = commonFunctions.PopulateFinancialYear(true, true).ToList();



            return View(packageAgreement);

        }

        /// <summary>
        /// Render data in Completed Roads Report
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult HabReport(PackageAgreementViewModel packageAgreement)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    packageAgreement.Mast_State_Code = packageAgreement.StateCode > 0 ? packageAgreement.StateCode : packageAgreement.Mast_State_Code;
                    packageAgreement.Mast_District_Code = packageAgreement.DistrictCode > 0 ? packageAgreement.DistrictCode : packageAgreement.Mast_District_Code;
                    packageAgreement.Mast_Block_Code = packageAgreement.BlockCode > 0 ? packageAgreement.BlockCode : packageAgreement.Mast_Block_Code;
                    packageAgreement.YearString = packageAgreement.Year == 0 ? "0" : (packageAgreement.Year + "-" + (packageAgreement.Year + 1));
                    return View(packageAgreement);
                }
                else
                {
                    string message = ModelState.Values.Where(x => x.Errors.Count == 1).FirstOrDefault().Errors[0].ErrorMessage;
                    return Json(string.Empty);
                }
            }
            catch
            {
                return View(packageAgreement);
            }

        }




        public ActionResult DRRPLayout()
        {
            PackageAgreementViewModel packageAgreement = new PackageAgreementViewModel();
            CommonFunctions commonFunctions = new CommonFunctions();

            packageAgreement.StateName = PMGSYSession.Current.StateCode == 0 ? "0" : PMGSYSession.Current.StateName.Trim();

            packageAgreement.DistName = PMGSYSession.Current.DistrictCode == 0 ? "0" : PMGSYSession.Current.DistrictName.Trim();



            packageAgreement.Mast_State_Code = PMGSYSession.Current.StateCode;

            packageAgreement.Mast_District_Code = PMGSYSession.Current.DistrictCode;

            packageAgreement.DistrictCode = PMGSYSession.Current.DistrictCode;


            packageAgreement.StateList = commonFunctions.PopulateStates(true);


            packageAgreement.StateList.Find(x => x.Value == packageAgreement.StateCode.ToString()).Selected = true;

            packageAgreement.DistrictList = new List<SelectListItem>();
            if (packageAgreement.StateCode == 0 && PMGSYSession.Current.RoleCode != 2)
            {
                packageAgreement.DistrictList.Insert(0, (new SelectListItem { Text = "All Districts", Value = "0", Selected = true }));
            }
            else
            {
                packageAgreement.DistrictList = commonFunctions.PopulateDistrictForSRRDA(PMGSYSession.Current.StateCode, true);
                //     packageAgreement.DistrictCode = PMGSYSession.Current.DistrictCode == 0 ? 0 : PMGSYSession.Current.DistrictCode;
                //     packageAgreement.DistrictList.Find(x => x.Value == packageAgreement.DistrictCode.ToString()).Selected = true;

            }
            packageAgreement.BlockList = new List<SelectListItem>();
            if (packageAgreement.DistrictCode == 0)
            {
                packageAgreement.BlockList.Insert(0, (new SelectListItem { Text = "All Blocks", Value = "0", Selected = true }));
            }
            else
            {
                packageAgreement.BlockList = commonFunctions.PopulateBlocks(packageAgreement.DistrictCode, true);

                packageAgreement.BlockList.Find(x => x.Value == "-1").Value = "0";
                packageAgreement.BlockList.Find(x => x.Value == packageAgreement.BlockCode.ToString()).Selected = true;
            }

            packageAgreement.Year = DateTime.Now.Year;

            packageAgreement.YearList = commonFunctions.PopulateFinancialYear(true, true).ToList();



            return View(packageAgreement);

        }

        /// <summary>
        /// Render data in Completed Roads Report
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult DRRPReport(PackageAgreementViewModel packageAgreement)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    packageAgreement.Mast_State_Code = packageAgreement.StateCode > 0 ? packageAgreement.StateCode : packageAgreement.Mast_State_Code;
                    packageAgreement.Mast_District_Code = packageAgreement.DistrictCode > 0 ? packageAgreement.DistrictCode : packageAgreement.Mast_District_Code;
                    packageAgreement.Mast_Block_Code = packageAgreement.BlockCode > 0 ? packageAgreement.BlockCode : packageAgreement.Mast_Block_Code;
                    packageAgreement.YearString = packageAgreement.Year == 0 ? "0" : (packageAgreement.Year + "-" + (packageAgreement.Year + 1));
                    return View(packageAgreement);
                }
                else
                {
                    string message = ModelState.Values.Where(x => x.Errors.Count == 1).FirstOrDefault().Errors[0].ErrorMessage;
                    return Json(string.Empty);
                }
            }
            catch
            {
                return View(packageAgreement);
            }

        }



        public ActionResult FacilityDownloadLayout()
        {
            PackageAgreementViewModel packageAgreement = new PackageAgreementViewModel();
            CommonFunctions commonFunctions = new CommonFunctions();

            packageAgreement.StateName = PMGSYSession.Current.StateCode == 0 ? "0" : PMGSYSession.Current.StateName.Trim();

            packageAgreement.DistName = PMGSYSession.Current.DistrictCode == 0 ? "0" : PMGSYSession.Current.DistrictName.Trim();



            packageAgreement.Mast_State_Code = PMGSYSession.Current.StateCode;

            packageAgreement.Mast_District_Code = PMGSYSession.Current.DistrictCode;

            packageAgreement.DistrictCode = PMGSYSession.Current.DistrictCode;


            packageAgreement.StateList = commonFunctions.PopulateStates(true);


            packageAgreement.StateList.Find(x => x.Value == packageAgreement.StateCode.ToString()).Selected = true;

            packageAgreement.DistrictList = new List<SelectListItem>();
            if (packageAgreement.StateCode == 0 && PMGSYSession.Current.RoleCode != 2)
            {
                packageAgreement.DistrictList.Insert(0, (new SelectListItem { Text = "All Districts", Value = "0", Selected = true }));
            }
            else
            {
                packageAgreement.DistrictList = commonFunctions.PopulateDistrictForSRRDA(PMGSYSession.Current.StateCode, true);
                //     packageAgreement.DistrictCode = PMGSYSession.Current.DistrictCode == 0 ? 0 : PMGSYSession.Current.DistrictCode;
                //     packageAgreement.DistrictList.Find(x => x.Value == packageAgreement.DistrictCode.ToString()).Selected = true;

            }
            packageAgreement.BlockList = new List<SelectListItem>();
            if (packageAgreement.DistrictCode == 0)
            {
                packageAgreement.BlockList.Insert(0, (new SelectListItem { Text = "All Blocks", Value = "0", Selected = true }));
            }
            else
            {
                packageAgreement.BlockList = commonFunctions.PopulateBlocks(packageAgreement.DistrictCode, true);

                packageAgreement.BlockList.Find(x => x.Value == "-1").Value = "0";
                packageAgreement.BlockList.Find(x => x.Value == packageAgreement.BlockCode.ToString()).Selected = true;
            }

            packageAgreement.Year = DateTime.Now.Year;

            packageAgreement.YearList = commonFunctions.PopulateFinancialYear(true, true).ToList();



            return View(packageAgreement);

        }

        /// <summary>
        /// Render data in Completed Roads Report
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult FacilityDownloadReport(PackageAgreementViewModel packageAgreement)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    packageAgreement.Mast_State_Code = packageAgreement.StateCode > 0 ? packageAgreement.StateCode : packageAgreement.Mast_State_Code;
                    packageAgreement.Mast_District_Code = packageAgreement.DistrictCode > 0 ? packageAgreement.DistrictCode : packageAgreement.Mast_District_Code;
                    packageAgreement.Mast_Block_Code = packageAgreement.BlockCode > 0 ? packageAgreement.BlockCode : packageAgreement.Mast_Block_Code;
                    packageAgreement.YearString = packageAgreement.Year == 0 ? "0" : (packageAgreement.Year + "-" + (packageAgreement.Year + 1));
                    return View(packageAgreement);
                }
                else
                {
                    string message = ModelState.Values.Where(x => x.Errors.Count == 1).FirstOrDefault().Errors[0].ErrorMessage;
                    return Json(string.Empty);
                }
            }
            catch
            {
                return View(packageAgreement);
            }

        }
        #endregion

        #region CUCPL


        public ActionResult CucplRptLayout()
        {
            PackageAgreementViewModel packageAgreement = new PackageAgreementViewModel();
            CommonFunctions commonFunctions = new CommonFunctions();

            packageAgreement.StateName = PMGSYSession.Current.StateCode == 0 ? "0" : PMGSYSession.Current.StateName.Trim();
            packageAgreement.DistName = PMGSYSession.Current.DistrictCode == 0 ? "0" : PMGSYSession.Current.DistrictName.Trim();



            packageAgreement.Mast_State_Code = PMGSYSession.Current.StateCode;
            packageAgreement.Mast_District_Code = PMGSYSession.Current.DistrictCode;
            packageAgreement.DistrictCode = PMGSYSession.Current.DistrictCode;

            packageAgreement.StateList = commonFunctions.PopulateStates(true);
            packageAgreement.StateList.Find(x => x.Value == packageAgreement.StateCode.ToString()).Selected = true;

            packageAgreement.DistrictList = new List<SelectListItem>();
            if (packageAgreement.Mast_State_Code == 0 && PMGSYSession.Current.RoleCode != 2)
            {
                packageAgreement.DistrictList.Insert(0, (new SelectListItem { Text = "All Districts", Value = "0", Selected = true }));
            }
            else
            {
                packageAgreement.DistrictList = commonFunctions.PopulateDistrictForSRRDA(PMGSYSession.Current.StateCode, false);
                //     packageAgreement.DistrictCode = PMGSYSession.Current.DistrictCode == 0 ? 0 : PMGSYSession.Current.DistrictCode;
                //     packageAgreement.DistrictList.Find(x => x.Value == packageAgreement.DistrictCode.ToString()).Selected = true;

            }
            packageAgreement.BlockList = new List<SelectListItem>();
            if (packageAgreement.DistrictCode == 0)
            {
                packageAgreement.BlockList.Insert(0, (new SelectListItem { Text = "Select Blocks", Value = "0", Selected = true }));
            }
            else
            {
                packageAgreement.BlockList = commonFunctions.PopulateBlocks(packageAgreement.DistrictCode, false);

                //    packageAgreement.BlockList.Find(x => x.Value == "-1").Value = "0";
                packageAgreement.BlockList.Find(x => x.Value == packageAgreement.BlockCode.ToString()).Selected = true;
            }

            packageAgreement.Year = DateTime.Now.Year;

            packageAgreement.YearList = commonFunctions.PopulateFinancialYear(true, true).ToList();



            return View(packageAgreement);

        }

        /// <summary>
        /// Render data in Completed Roads Report
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult CucplRptReport(PackageAgreementViewModel packageAgreement)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    packageAgreement.Mast_State_Code = packageAgreement.StateCode > 0 ? packageAgreement.StateCode : packageAgreement.Mast_State_Code;
                    packageAgreement.Mast_District_Code = packageAgreement.DistrictCode > 0 ? packageAgreement.DistrictCode : packageAgreement.Mast_District_Code;
                    packageAgreement.Mast_Block_Code = packageAgreement.BlockCode > 0 ? packageAgreement.BlockCode : packageAgreement.Mast_Block_Code;
                    packageAgreement.YearString = packageAgreement.Year == 0 ? "0" : (packageAgreement.Year + "-" + (packageAgreement.Year + 1));
                    return View(packageAgreement);
                }
                else
                {
                    string message = ModelState.Values.Where(x => x.Errors.Count == 1).FirstOrDefault().Errors[0].ErrorMessage;
                    return Json(string.Empty);
                }
            }
            catch
            {
                return View(packageAgreement);
            }

        }


        #region Common function
        public ActionResult DistrictDetailsForCUCPL(FormCollection frmCollection)
        {
            CommonFunctions objCommonFunctions = new CommonFunctions();
            List<SelectListItem> list = objCommonFunctions.PopulateDistrict(Convert.ToInt32(frmCollection["StateCode"]), false);
            //list.Find(x => x.Value == "-1").Value = "0";
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public ActionResult BlockDetailsForCUCPL(FormCollection frmCollection)
        {
            CommonFunctions objCommonFunctions = new CommonFunctions();
            List<SelectListItem> list = objCommonFunctions.PopulateBlocks(Convert.ToInt32(frmCollection["DistrictCode"]), false);
            //list.Find(x => x.Value == "-1").Value = "0";
            return Json(list, JsonRequestBehavior.AllowGet);
        }
        #endregion
        #endregion

        #region RSA

        public ActionResult RSALayout()
        {
            RSALayout packageAgreement = new RSALayout();
            CommonFunctions commonFunctions = new CommonFunctions();

            packageAgreement.StateName = PMGSYSession.Current.StateCode == 0 ? "0" : PMGSYSession.Current.StateName.Trim();

            packageAgreement.DistName = PMGSYSession.Current.DistrictCode == 0 ? "0" : PMGSYSession.Current.DistrictName.Trim();
            packageAgreement.Mast_State_Code = PMGSYSession.Current.StateCode;
            packageAgreement.Mast_District_Code = PMGSYSession.Current.DistrictCode;
            packageAgreement.DistrictCode = PMGSYSession.Current.DistrictCode;
            packageAgreement.StateList = commonFunctions.PopulateStates(false);
            packageAgreement.StateList.Find(x => x.Value == packageAgreement.StateCode.ToString()).Selected = true;
            packageAgreement.DistrictList = new List<SelectListItem>();

            if (packageAgreement.StateCode == 0 && PMGSYSession.Current.RoleCode != 2)
            {
                packageAgreement.DistrictList.Insert(0, (new SelectListItem { Text = "All Districts", Value = "0", Selected = true }));
            }
            else
            {
                packageAgreement.DistrictList = commonFunctions.PopulateDistrictForSRRDA(PMGSYSession.Current.StateCode, true);
            }
            packageAgreement.BlockList = new List<SelectListItem>();
            if (packageAgreement.DistrictCode == 0)
            {
                packageAgreement.BlockList.Insert(0, (new SelectListItem { Text = "All Blocks", Value = "0", Selected = true }));
            }
            else
            {
                packageAgreement.BlockList = commonFunctions.PopulateBlocks(packageAgreement.DistrictCode, true);

                packageAgreement.BlockList.Find(x => x.Value == "-1").Value = "0";
                packageAgreement.BlockList.Find(x => x.Value == packageAgreement.BlockCode.ToString()).Selected = true;
            }

            packageAgreement.lstscheme = commonFunctions.PopulateScheme();

            packageAgreement.Year = 0;
            packageAgreement.YearList = commonFunctions.PopulateFinancialYear(true, true).ToList();
            return View(packageAgreement);

        }

        /// <summary>
        /// Render data in Completed Roads Report
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult RSAReport(RSALayout packageAgreement)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    packageAgreement.Mast_State_Code = packageAgreement.StateCode > 0 ? packageAgreement.StateCode : packageAgreement.Mast_State_Code;
                    packageAgreement.Mast_District_Code = packageAgreement.DistrictCode > 0 ? packageAgreement.DistrictCode : packageAgreement.Mast_District_Code;
                    packageAgreement.Mast_Block_Code = packageAgreement.BlockCode > 0 ? packageAgreement.BlockCode : packageAgreement.Mast_Block_Code;
                    packageAgreement.YearString = packageAgreement.Year == 0 ? "0" : (packageAgreement.Year + "-" + (packageAgreement.Year + 1));
                    return View(packageAgreement);
                }
                else
                {
                    string message = ModelState.Values.Where(x => x.Errors.Count == 1).FirstOrDefault().Errors[0].ErrorMessage;
                    return Json(string.Empty);
                }
            }
            catch
            {
                return View(packageAgreement);
            }

        }
        #endregion

        #region Unfinalized DRRP Roads

        public ActionResult UnfinalizedDRRPRoadsLayout()
        {
            UnfinalizedDRRPRoadsViewModel unfinalizedRoads = new UnfinalizedDRRPRoadsViewModel();
            CommonFunctions commonFunctions = new CommonFunctions();
            unfinalizedRoads.StateName = PMGSYSession.Current.StateCode == 0 ? "0" : PMGSYSession.Current.StateName.Trim();
            unfinalizedRoads.DistName = PMGSYSession.Current.DistrictCode == 0 ? "0" : PMGSYSession.Current.DistrictName.Trim();
            unfinalizedRoads.Mast_State_Code = PMGSYSession.Current.StateCode;
            unfinalizedRoads.Mast_District_Code = PMGSYSession.Current.DistrictCode;
            unfinalizedRoads.DistrictCode = PMGSYSession.Current.DistrictCode;
            unfinalizedRoads.StateList = commonFunctions.PopulateStates(true);
            unfinalizedRoads.StateList.Find(x => x.Value == unfinalizedRoads.StateCode.ToString()).Selected = true;
            unfinalizedRoads.DistrictList = new List<SelectListItem>();
            if (unfinalizedRoads.StateCode == 0 && PMGSYSession.Current.RoleCode != 2)
            {
                unfinalizedRoads.DistrictList.Insert(0, (new SelectListItem { Text = "All Districts", Value = "0", Selected = true }));
            }
            else
            {
                unfinalizedRoads.DistrictList = commonFunctions.PopulateDistrictForSRRDA(PMGSYSession.Current.StateCode, true);
            }
            unfinalizedRoads.BlockList = new List<SelectListItem>();
            if (unfinalizedRoads.DistrictCode == 0)
            {
                unfinalizedRoads.BlockList.Insert(0, (new SelectListItem { Text = "All Blocks", Value = "0", Selected = true }));
            }
            else
            {
                unfinalizedRoads.BlockList = commonFunctions.PopulateBlocks(unfinalizedRoads.DistrictCode, true);
                unfinalizedRoads.BlockList.Find(x => x.Value == "-1").Value = "0";
                unfinalizedRoads.BlockList.Find(x => x.Value == unfinalizedRoads.BlockCode.ToString()).Selected = true;
            }
            unfinalizedRoads.Year = DateTime.Now.Year;
            unfinalizedRoads.YearList = commonFunctions.PopulateFinancialYear(true, true).ToList();
            return View(unfinalizedRoads);
        }

        /// <summary>
        /// Render data in Completed Roads Report
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult UnfinalizedDRRPRoadsReport(UnfinalizedDRRPRoadsViewModel unfinalizedRoads)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    unfinalizedRoads.Mast_State_Code = unfinalizedRoads.StateCode > 0 ? unfinalizedRoads.StateCode : unfinalizedRoads.Mast_State_Code;
                    unfinalizedRoads.Mast_District_Code = unfinalizedRoads.DistrictCode > 0 ? unfinalizedRoads.DistrictCode : unfinalizedRoads.Mast_District_Code;
                    unfinalizedRoads.Mast_Block_Code = unfinalizedRoads.BlockCode > 0 ? unfinalizedRoads.BlockCode : unfinalizedRoads.Mast_Block_Code;
                    unfinalizedRoads.YearString = unfinalizedRoads.Year == 0 ? "0" : (unfinalizedRoads.Year + "-" + (unfinalizedRoads.Year + 1));
                    return View(unfinalizedRoads);
                }
                else
                {
                    string message = ModelState.Values.Where(x => x.Errors.Count == 1).FirstOrDefault().Errors[0].ErrorMessage;
                    return Json(string.Empty);
                }
            }
            catch
            {
                return View(unfinalizedRoads);
            }

        }


        #endregion

        #region ZeroPopulation
        public ActionResult ZeroPopulationHabitaionLayout()
        {
            PackageAgreementViewModel packageAgreement = new PackageAgreementViewModel();
            CommonFunctions commonFunctions = new CommonFunctions();

            packageAgreement.StateName = PMGSYSession.Current.StateCode == 0 ? "0" : PMGSYSession.Current.StateName.Trim();

            packageAgreement.DistName = PMGSYSession.Current.DistrictCode == 0 ? "0" : PMGSYSession.Current.DistrictName.Trim();



            packageAgreement.Mast_State_Code = PMGSYSession.Current.StateCode;

            packageAgreement.Mast_District_Code = PMGSYSession.Current.DistrictCode;

            packageAgreement.DistrictCode = PMGSYSession.Current.DistrictCode;


            packageAgreement.StateList = commonFunctions.PopulateStates(true);


            packageAgreement.StateList.Find(x => x.Value == packageAgreement.StateCode.ToString()).Selected = true;

            packageAgreement.DistrictList = new List<SelectListItem>();
            if (packageAgreement.StateCode == 0 && PMGSYSession.Current.RoleCode != 2)
            {
                packageAgreement.DistrictList.Insert(0, (new SelectListItem { Text = "All Districts", Value = "0", Selected = true }));
            }
            else
            {
                packageAgreement.DistrictList = commonFunctions.PopulateDistrictForSRRDA(PMGSYSession.Current.StateCode, true);
                //     packageAgreement.DistrictCode = PMGSYSession.Current.DistrictCode == 0 ? 0 : PMGSYSession.Current.DistrictCode;
                //     packageAgreement.DistrictList.Find(x => x.Value == packageAgreement.DistrictCode.ToString()).Selected = true;

            }
            packageAgreement.BlockList = new List<SelectListItem>();
            if (packageAgreement.DistrictCode == 0)
            {
                packageAgreement.BlockList.Insert(0, (new SelectListItem { Text = "All Blocks", Value = "0", Selected = true }));
            }
            else
            {
                packageAgreement.BlockList = commonFunctions.PopulateBlocks(packageAgreement.DistrictCode, true);

                packageAgreement.BlockList.Find(x => x.Value == "-1").Value = "0";
                packageAgreement.BlockList.Find(x => x.Value == packageAgreement.BlockCode.ToString()).Selected = true;
            }

            packageAgreement.Year = DateTime.Now.Year;

            packageAgreement.YearList = commonFunctions.PopulateFinancialYear(true, true).ToList();



            return View(packageAgreement);

        }

        /// <summary>
        /// Render data in Completed Roads Report
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult ZeroPopulationHabitaionReport(PackageAgreementViewModel packageAgreement)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    packageAgreement.Mast_State_Code = packageAgreement.StateCode > 0 ? packageAgreement.StateCode : packageAgreement.Mast_State_Code;
                    packageAgreement.Mast_District_Code = packageAgreement.DistrictCode > 0 ? packageAgreement.DistrictCode : packageAgreement.Mast_District_Code;
                    packageAgreement.Mast_Block_Code = packageAgreement.BlockCode > 0 ? packageAgreement.BlockCode : packageAgreement.Mast_Block_Code;
                    packageAgreement.YearString = packageAgreement.Year == 0 ? "0" : (packageAgreement.Year + "-" + (packageAgreement.Year + 1));
                    return View(packageAgreement);
                }
                else
                {
                    string message = ModelState.Values.Where(x => x.Errors.Count == 1).FirstOrDefault().Errors[0].ErrorMessage;
                    return Json(string.Empty);
                }
            }
            catch
            {
                return View(packageAgreement);
            }

        }

        #endregion

        #region Maintenance and Work Layout

        public ActionResult MaintenanceWorkAndLengthLayout()
        {
            MaintWorksAndLength allotmentDuties = new MaintWorksAndLength();
            CommonFunctions commonFunctions = new CommonFunctions();

            try
            {
                allotmentDuties.Year = DateTime.Now.Year;
                allotmentDuties.YearList = commonFunctions.PopulateYears(DateTime.Now.Year);

                allotmentDuties.MonthList = commonFunctions.PopulateMonths(DateTime.Now.Month);
                allotmentDuties.Month = DateTime.Now.Month;
            }
            catch (Exception ex)
            {
                return null;
            }

            return View(allotmentDuties);
        }
        public ActionResult MaintenanceWorkAndLengthReport(MaintWorksAndLength allotmentDuties)
        {

            try
            {
                if (ModelState.IsValid)
                {
                    return View("MaintenanceWorkAndLengthReport", allotmentDuties);
                }
                else
                {
                }
            }
            catch (Exception)
            {
                return null;
            }


            return View();
        }

        #endregion

        #region Gepnic Bid Tech ( Fin Open and Evaulation )

        public ActionResult GepnicBidLayout()
        {
            GepnicBidModel packageAgreement = new GepnicBidModel();
            CommonFunctions commonFunctions = new CommonFunctions();
            packageAgreement.StateName = PMGSYSession.Current.StateCode == 0 ? "0" : PMGSYSession.Current.StateName.Trim();
            packageAgreement.Mast_State_Code = PMGSYSession.Current.StateCode;
            packageAgreement.StateList = commonFunctions.PopulateStates(true);
            packageAgreement.StateList.Find(x => x.Value == packageAgreement.StateCode.ToString()).Selected = true;
            packageAgreement.BatchList = new List<SelectListItem>();
            packageAgreement.BatchList.Insert(0, new SelectListItem() { Value = "0", Text = "All" });
            packageAgreement.BatchList.Insert(1, new SelectListItem() { Value = "1", Text = "BATCH 1" });
            packageAgreement.BatchList.Insert(2, new SelectListItem() { Value = "2", Text = "BATCH 2" });
            packageAgreement.BatchList.Insert(3, new SelectListItem() { Value = "3", Text = "BATCH 3" });
            packageAgreement.BatchList.Insert(4, new SelectListItem() { Value = "4", Text = "BATCH 4" });
            packageAgreement.BatchList.Insert(5, new SelectListItem() { Value = "5", Text = "BATCH 5" });
            packageAgreement.BatchList.Insert(6, new SelectListItem() { Value = "6", Text = "BATCH 6" });
            packageAgreement.Year = DateTime.Now.Year;
            packageAgreement.YearList = commonFunctions.PopulateFinancialYear(true, true).ToList();
            packageAgreement.SchemeCode = 0;
            packageAgreement.SchemeList = new List<SelectListItem>();
            packageAgreement.SchemeList.Insert(0, new SelectListItem() { Value = "0", Text = "All", Selected = true });
            packageAgreement.SchemeList.Insert(1, new SelectListItem() { Value = "1", Text = "PMGSY I" });
            packageAgreement.SchemeList.Insert(2, new SelectListItem() { Value = "2", Text = "PMGSY II" });
            packageAgreement.SchemeList.Insert(3, new SelectListItem() { Value = "3", Text = "RCPLWE" });
            packageAgreement.SchemeList.Insert(4, new SelectListItem() { Value = "4", Text = "PMGSY III" });

            return View(packageAgreement);
        }

        /// <summary>
        /// Render data in Completed Roads Report
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult GepnicBidReport(GepnicBidModel packageAgreement)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    packageAgreement.Mast_State_Code = packageAgreement.StateCode > 0 ? packageAgreement.StateCode : packageAgreement.Mast_State_Code;
                    packageAgreement.YearString = packageAgreement.Year == 0 ? "0" : (packageAgreement.Year + "-" + (packageAgreement.Year + 1));
                    return View(packageAgreement);
                }
                else
                {
                    string message = ModelState.Values.Where(x => x.Errors.Count == 1).FirstOrDefault().Errors[0].ErrorMessage;
                    return Json(string.Empty);
                }
            }
            catch
            {
                return View(packageAgreement);
            }

        }

        #endregion 

        #region Agremment Reports

        /// <summary>
        /// Monthwise Agreement Details : Redering Layout (Filters) Action 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult MonthwiseAgreementLayout()
        {
            try
            {
                MonthwiseAgreement monthwiseAgreementDetails = new MonthwiseAgreement();
                CommonFunctions commonFunctions = new CommonFunctions();
                monthwiseAgreementDetails.StateName = PMGSYSession.Current.StateCode == 0 ? "0" : PMGSYSession.Current.StateName.Trim();
                monthwiseAgreementDetails.DistName = PMGSYSession.Current.DistrictCode == 0 ? "0" : PMGSYSession.Current.DistrictName.Trim();
                monthwiseAgreementDetails.Mast_State_Code = PMGSYSession.Current.StateCode;
                monthwiseAgreementDetails.Mast_District_Code = PMGSYSession.Current.DistrictCode;
                monthwiseAgreementDetails.DistrictCode = PMGSYSession.Current.DistrictCode;
                monthwiseAgreementDetails.StateList = commonFunctions.PopulateStates(false);
                monthwiseAgreementDetails.StateList.Find(x => x.Value == monthwiseAgreementDetails.StateCode.ToString()).Selected = true;
                monthwiseAgreementDetails.DistrictList = new List<SelectListItem>();
                monthwiseAgreementDetails.SchemeCode = 0;
                monthwiseAgreementDetails.SchemeList = new List<SelectListItem>();
                monthwiseAgreementDetails.SchemeList.Insert(0, new SelectListItem() { Value = "0", Text = "All", Selected = true });
                monthwiseAgreementDetails.SchemeList.Insert(1, new SelectListItem() { Value = "1", Text = "PMGSY I" });
                monthwiseAgreementDetails.SchemeList.Insert(2, new SelectListItem() { Value = "2", Text = "PMGSY II" });
                monthwiseAgreementDetails.SchemeList.Insert(3, new SelectListItem() { Value = "4", Text = "PMGSY III" });
                monthwiseAgreementDetails.SchemeList.Insert(4, new SelectListItem() { Value = "3", Text = "RCPLWE" });

                if (monthwiseAgreementDetails.StateCode == 0 && PMGSYSession.Current.RoleCode != 2)
                {
                    monthwiseAgreementDetails.DistrictList.Insert(0, (new SelectListItem { Text = "All Districts", Value = "0", Selected = true }));
                }
                else
                {
                    monthwiseAgreementDetails.DistrictList = commonFunctions.PopulateDistrictForSRRDA(PMGSYSession.Current.StateCode, true);
                }
                return View(monthwiseAgreementDetails);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "Areas.PackageAgreement.MonthwiseAgreementLayout()");
                return null;
            }
        }

        /// <summary>
        /// Monthwise Agreement Details :Passing Parameters to Server
        /// </summary>
        /// <param name="packageAgreement"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult MonthwiseAgreementReport(MonthwiseAgreement monthwiseAgreementDetails)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    monthwiseAgreementDetails.Mast_State_Code = monthwiseAgreementDetails.StateCode > 0 ? monthwiseAgreementDetails.StateCode : monthwiseAgreementDetails.Mast_State_Code;
                    monthwiseAgreementDetails.Mast_District_Code = monthwiseAgreementDetails.DistrictCode > 0 ? monthwiseAgreementDetails.DistrictCode : monthwiseAgreementDetails.Mast_District_Code;
                    return View(monthwiseAgreementDetails);
                }
                else
                {
                    string message = ModelState.Values.Where(x => x.Errors.Count == 1).FirstOrDefault().Errors[0].ErrorMessage;
                    return Json(string.Empty);
                }
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "Areas.PackageAgreement.MonthwiseAgreementReport()");
                return View(monthwiseAgreementDetails);
            }

        }






        /// <summary>
        /// Yearwise Agreement Details : Redering Layout (Filters) Action 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult YearwiseAgreementLayout()
        {
            try
            {
                MonthwiseAgreement monthwiseAgreementDetails = new MonthwiseAgreement();
                CommonFunctions commonFunctions = new CommonFunctions();
                monthwiseAgreementDetails.StateName = PMGSYSession.Current.StateCode == 0 ? "0" : PMGSYSession.Current.StateName.Trim();
                monthwiseAgreementDetails.DistName = PMGSYSession.Current.DistrictCode == 0 ? "0" : PMGSYSession.Current.DistrictName.Trim();
                monthwiseAgreementDetails.Mast_State_Code = PMGSYSession.Current.StateCode;
                monthwiseAgreementDetails.Mast_District_Code = PMGSYSession.Current.DistrictCode;
                monthwiseAgreementDetails.DistrictCode = PMGSYSession.Current.DistrictCode;
                monthwiseAgreementDetails.StateList = commonFunctions.PopulateStates(false);
                monthwiseAgreementDetails.StateList.Find(x => x.Value == monthwiseAgreementDetails.StateCode.ToString()).Selected = true;
                monthwiseAgreementDetails.DistrictList = new List<SelectListItem>();
                monthwiseAgreementDetails.SchemeCode = 0;
                monthwiseAgreementDetails.SchemeList = new List<SelectListItem>();
                monthwiseAgreementDetails.SchemeList.Insert(0, new SelectListItem() { Value = "0", Text = "All", Selected = true });
                monthwiseAgreementDetails.SchemeList.Insert(1, new SelectListItem() { Value = "1", Text = "PMGSY I" });
                monthwiseAgreementDetails.SchemeList.Insert(2, new SelectListItem() { Value = "2", Text = "PMGSY II" });
                monthwiseAgreementDetails.SchemeList.Insert(3, new SelectListItem() { Value = "4", Text = "PMGSY III" });
                monthwiseAgreementDetails.SchemeList.Insert(4, new SelectListItem() { Value = "3", Text = "RCPLWE" });

                if (monthwiseAgreementDetails.StateCode == 0 && PMGSYSession.Current.RoleCode != 2)
                {
                    monthwiseAgreementDetails.DistrictList.Insert(0, (new SelectListItem { Text = "All Districts", Value = "0", Selected = true }));
                }
                else
                {
                    monthwiseAgreementDetails.DistrictList = commonFunctions.PopulateDistrictForSRRDA(PMGSYSession.Current.StateCode, true);
                }
                return View(monthwiseAgreementDetails);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "Areas.PackageAgreement.YearwiseAgreementLayout()");
                return null;
            }
        }

        /// <summary>
        /// Yearwise Agreement Details :Passing Parameters to Server
        /// </summary>
        /// <param name="packageAgreement"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult YearwiseAgreementReport(MonthwiseAgreement monthwiseAgreementDetails)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    monthwiseAgreementDetails.Mast_State_Code = monthwiseAgreementDetails.StateCode > 0 ? monthwiseAgreementDetails.StateCode : monthwiseAgreementDetails.Mast_State_Code;
                    monthwiseAgreementDetails.Mast_District_Code = monthwiseAgreementDetails.DistrictCode > 0 ? monthwiseAgreementDetails.DistrictCode : monthwiseAgreementDetails.Mast_District_Code;
                    return View(monthwiseAgreementDetails);
                }
                else
                {
                    string message = ModelState.Values.Where(x => x.Errors.Count == 1).FirstOrDefault().Errors[0].ErrorMessage;
                    return Json(string.Empty);
                }
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "Areas.PackageAgreement.YearwiseAgreementReport()");
                return View(monthwiseAgreementDetails);
            }

        }

        #endregion


        #region Habitation Direct Indirect
        /// <summary>
        /// Render Hab Direct Indirect Report Filter Layout
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult HabDirectIndirectLayout()
        {
            try
            {
                PackageAgreementViewModel packageAgreement = new PackageAgreementViewModel();
                CommonFunctions commonFunctions = new CommonFunctions();
                packageAgreement.StateName = PMGSYSession.Current.StateCode == 0 ? "0" : PMGSYSession.Current.StateName.Trim();
                packageAgreement.DistName = PMGSYSession.Current.DistrictCode == 0 ? "0" : PMGSYSession.Current.DistrictName.Trim();
                packageAgreement.Mast_State_Code = PMGSYSession.Current.StateCode;
                packageAgreement.Mast_District_Code = PMGSYSession.Current.DistrictCode;
                packageAgreement.DistrictCode = PMGSYSession.Current.DistrictCode;
                packageAgreement.StateList = commonFunctions.PopulateStates(true);
                packageAgreement.StateList.Find(x => x.Value == packageAgreement.StateCode.ToString()).Selected = true;
                packageAgreement.DistrictList = new List<SelectListItem>();
                if (packageAgreement.StateCode == 0 && PMGSYSession.Current.RoleCode != 2)
                {
                    packageAgreement.DistrictList.Insert(0, (new SelectListItem { Text = "All Districts", Value = "0", Selected = true }));
                }
                else
                {
                    packageAgreement.DistrictList = commonFunctions.PopulateDistrictForSRRDA(PMGSYSession.Current.StateCode, true);
                }
                packageAgreement.BlockList = new List<SelectListItem>();
                if (packageAgreement.DistrictCode == 0)
                {
                    packageAgreement.BlockList.Insert(0, (new SelectListItem { Text = "All Blocks", Value = "0", Selected = true }));
                }
                else
                {
                    packageAgreement.BlockList = commonFunctions.PopulateBlocks(packageAgreement.DistrictCode, true);
                    packageAgreement.BlockList.Find(x => x.Value == "-1").Value = "0";
                    packageAgreement.BlockList.Find(x => x.Value == packageAgreement.BlockCode.ToString()).Selected = true;
                }
                packageAgreement.Year = DateTime.Now.Year;
                packageAgreement.YearList = commonFunctions.PopulateFinancialYear(true, true).ToList();
                return View(packageAgreement);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "Areas.PackageAgreement.HabDirectIndirectReport()");
                return null;
            }
        }

        /// <summary>
        /// Render data in Hab Direct Indirect Report
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult HabDirectIndirectReport(PackageAgreementViewModel packageAgreement)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    packageAgreement.Mast_State_Code = packageAgreement.StateCode > 0 ? packageAgreement.StateCode : packageAgreement.Mast_State_Code;
                    packageAgreement.Mast_District_Code = packageAgreement.DistrictCode > 0 ? packageAgreement.DistrictCode : packageAgreement.Mast_District_Code;
                    packageAgreement.Mast_Block_Code = packageAgreement.BlockCode > 0 ? packageAgreement.BlockCode : packageAgreement.Mast_Block_Code;
                    packageAgreement.YearString = packageAgreement.Year == 0 ? "0" : (packageAgreement.Year + "-" + (packageAgreement.Year + 1));
                    return View(packageAgreement);
                }
                else
                {
                    string message = ModelState.Values.Where(x => x.Errors.Count == 1).FirstOrDefault().Errors[0].ErrorMessage;
                    return Json(string.Empty);
                }
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "Areas.PackageAgreement.HabDirectIndirectReport()");
                return View(packageAgreement);
            }

        }

        #endregion Habitation Direct Indirect


        #region Issues


        public ActionResult IssuesLayout()
        {
            Issues packageAgreement = new Issues();
            CommonFunctions commonFunctions = new CommonFunctions();

            packageAgreement.StateName = PMGSYSession.Current.StateCode == 0 ? "0" : PMGSYSession.Current.StateName.Trim();

          
            packageAgreement.Mast_State_Code = PMGSYSession.Current.StateCode;



            packageAgreement.StateList = commonFunctions.PopulateStates(true);


            packageAgreement.StateList.Find(x => x.Value == packageAgreement.StateCode.ToString()).Selected = true;

            

            return View(packageAgreement);

        }

        /// <summary>
        /// Render data in Completed Roads Report
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult IssuesReport(Issues packageAgreement)
        {

            try
            {
                if (ModelState.IsValid)
                {

                    packageAgreement.Mast_State_Code = packageAgreement.StateCode > 0 ? packageAgreement.StateCode : packageAgreement.Mast_State_Code;
                  
                    
                    return View(packageAgreement);
                }
                else
                {
                    string message = ModelState.Values.Where(x => x.Errors.Count == 1).FirstOrDefault().Errors[0].ErrorMessage;
                    return Json(string.Empty);

                }
            }
            catch
            {
                return View(packageAgreement);
            }

        }
        #endregion
    }
}
