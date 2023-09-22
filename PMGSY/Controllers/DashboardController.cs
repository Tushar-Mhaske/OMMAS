#region File Header
/*
        * Project Id    :
        * Project Name  :   OMMAS II
        * Name          :   DashboardController.cs        
        * Description   :   Main layout for Dashboard for WorldBank, Mord etc.
        *                   Financial Report, Status Monitoring, General Information, Balancesheet, Annual account, Cummulative Expenditure, Implementation Summary . 
        * Author        :   Shyam Yadav 
        * Creation Date :   18/Sep/2013    
 **/
#endregion

using PMGSY.BAL.Dashboard;
using PMGSY.Common;
using PMGSY.DAL.Dashboard;
using PMGSY.Extensions;
using PMGSY.Models;
using PMGSY.Models.Common;
using PMGSY.Models.Dashboard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PMGSY.Controllers
{
    [RequiredAuthentication]
    public class DashboardController : Controller
    {
        private IDashboardBAL dashboardBAL;

        /// <summary>
        /// Renders Main Layout for World Bank Dashboard
        /// </summary>
        /// <returns></returns>
        [Audit]
        public ActionResult DashboardLayout()
        {
            WBankDashboardLayoutViewModel wBankDashboardLayoutViewModel = new WBankDashboardLayoutViewModel();
            CommonFunctions objCommonFuntion = new CommonFunctions();
            List<SelectListItem> lstCollaborations = new List<SelectListItem>();
            try
            {
                wBankDashboardLayoutViewModel.STATE_ND_CODE = 0;
                wBankDashboardLayoutViewModel.PIU_ND_CODE = 0;
                wBankDashboardLayoutViewModel.FUND_TYPE_LIST = objCommonFuntion.PopulateFundTypes(false);

                if (PMGSYSession.Current.RoleCode == 45)     //For World Bank
                {
                    lstCollaborations.Insert(0, (new SelectListItem { Text = "World Bank (RRP-I)", Value = "3", Selected = true }));
                    lstCollaborations.Insert(1, (new SelectListItem { Text = "World Bank (RRP-II)", Value = "4" }));
                    wBankDashboardLayoutViewModel.COLLABORATIONS_LIST = lstCollaborations;
                }
                else //if (PMGSYSession.Current.RoleCode == 25)   
                {
                    wBankDashboardLayoutViewModel.COLLABORATIONS_LIST = objCommonFuntion.PopulateFundingAgency(true);
                }

                wBankDashboardLayoutViewModel.QM_TYPE = "I";
                wBankDashboardLayoutViewModel.QM_TYPE_LIST = objCommonFuntion.PopulateMonitorTypes();

                return View(wBankDashboardLayoutViewModel);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                throw ex;
            }
        }

        #region Financial Details

        /// <summary>
        /// Render Filters for World Bank Dashboard
        /// </summary>
        /// <returns></returns>
        [Audit]
        public ActionResult WBankDashboardFilters()
        {
            try
            {
                WBankDashboardFiltersViewModel wBankDashboardViewModel = new WBankDashboardFiltersViewModel();
                CommonFunctions objCommonFuntion = new CommonFunctions();
                TransactionParams objParam = new TransactionParams();

                objParam.DISTRICT_CODE = 0;

                wBankDashboardViewModel.STATE_DEPT_LIST = objCommonFuntion.PopulateNodalAgencies();
                //wBankDashboardViewModel.STATE_DEPT_LIST.RemoveAt(0);
                //wBankDashboardViewModel.STATE_DEPT_LIST.Insert(0, (new SelectListItem { Text = "All SRRDA", Value = "0", Selected = true }));

                wBankDashboardViewModel.DISTRICTS_LIST = objCommonFuntion.PopulateDistrictOfSRRDADept(0, true);


                wBankDashboardViewModel.DPIU_LIST = objCommonFuntion.PopulateDPIU(objParam);
                wBankDashboardViewModel.DPIU_LIST.RemoveAt(0);
                wBankDashboardViewModel.DPIU_LIST.Insert(0, (new SelectListItem { Text = "All PIU", Value = "0", Selected = true }));

                wBankDashboardViewModel.COLLABORATIONS_LIST = objCommonFuntion.PopulateFundingAgency(true);

                List<SelectListItem> lstDuration = new List<SelectListItem>();
                lstDuration.Insert(0, (new SelectListItem { Text = "Yearly", Value = "Y", Selected = true }));
                lstDuration.Insert(1, (new SelectListItem { Text = "Half Yearly", Value = "H" }));
                lstDuration.Insert(2, (new SelectListItem { Text = "Quarterly", Value = "Q" }));
                wBankDashboardViewModel.DURATION_TYPE_LIST = lstDuration;

                List<SelectListItem> lstYearDuration = new List<SelectListItem>();
                lstYearDuration.Insert(0, (new SelectListItem { Text = "Select Duration", Value = "0", Selected = true }));
                wBankDashboardViewModel.DURATION_LIST = lstYearDuration;

                wBankDashboardViewModel.YEAR = DateTime.Now.Year;
                wBankDashboardViewModel.YEARS_LIST = objCommonFuntion.PopulateFinancialYear(false, true).ToList();

                return View(wBankDashboardViewModel);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return null;
            }

        }


        ///// <summary>
        ///// Populate Districts according to selected State
        ///// </summary>
        ///// <param name="selectedState"></param>
        ///// <param name="isAllSelected"></param>
        ///// <returns></returns>
        //public ActionResult PopulateDistricts(int selectedValue)
        //{
        //    try
        //    {
        //        return Json(new CommonFunctions().PopulateDistrictOfSRRDADept(selectedValue,true));
        //    }
        //    catch (Exception)
        //    {
        //        return null;
        //    }
        //}


        ///// <summary>
        ///// Populate DPIUs according to selected Districts
        ///// </summary>
        ///// <param name="selectedValue"></param>
        ///// <param name="isAllSelected"></param>
        ///// <returns></returns>
        //public ActionResult PopulateDPIU(int selectedValue)
        //{
        //    try
        //    {
        //        List<SelectListItem> lstDPIUs = new List<SelectListItem>();
        //        TransactionParams objParam = new TransactionParams();
        //        objParam.DISTRICT_CODE = selectedValue;
        //        lstDPIUs = new CommonFunctions().PopulateDPIU(objParam);
        //        lstDPIUs.RemoveAt(0);
        //        lstDPIUs.Insert(0, (new SelectListItem { Text = "All PIU", Value = "0", Selected = true }));
        //        return Json(lstDPIUs);
        //    }
        //    catch (Exception)
        //    {
        //        return null;
        //    }
        //}


        ///// <summary>
        ///// Populate Financial Years Duration as per selected duration
        ///// If Yearly - then populate only select option
        ///// If Half Yearly - Populate Apr - Sept & Oct - Mar
        ///// If Quarterly - Populate Apr - June & July - Sep & Oct - Dec & Jan - Mar
        ///// </summary>
        ///// <param name="selectedValue"></param>
        ///// <param name="isAllSelected"></param>
        ///// <returns></returns>
        //public ActionResult PopulateFinancialDuration(string selectedValue)
        //{
        //    try
        //    {
        //        List<SelectListItem> lstFinancialYearDuration = new List<SelectListItem>();

        //        if (selectedValue.Equals("Y"))
        //        {
        //            lstFinancialYearDuration.Insert(0, (new SelectListItem { Text = "Select Year Duration", Value = "0", Selected = true }));
        //        }
        //        else if (selectedValue.Equals("H"))
        //        {
        //            lstFinancialYearDuration.Insert(0, (new SelectListItem { Text = "April - September", Value = "1", Selected = true }));
        //            lstFinancialYearDuration.Insert(1, (new SelectListItem { Text = "October - March", Value = "2" }));
        //        }
        //        else if (selectedValue.Equals("Q"))
        //        {
        //            lstFinancialYearDuration.Insert(0, (new SelectListItem { Text = "April - June", Value = "1", Selected = true }));
        //            lstFinancialYearDuration.Insert(1, (new SelectListItem { Text = "July - September", Value = "2" }));
        //            lstFinancialYearDuration.Insert(2, (new SelectListItem { Text = "October - December", Value = "3" }));
        //            lstFinancialYearDuration.Insert(3, (new SelectListItem { Text = "January - March", Value = "4" }));
        //        }

        //        return Json(lstFinancialYearDuration);
        //    }
        //    catch (Exception)
        //    {
        //        return null;
        //    }
        //}


        /// <summary>
        /// Lists Fund V/s Expenditure Details for all SRRDAs
        /// </summary>
        /// <param name="formCollection"></param>
        /// <returns></returns>
        [HttpPost]
        [Audit]
        public ActionResult ListFundVsExpenditureReport(FormCollection formCollection)
        {
            dashboardBAL = new DashboardBAL();
            int totalRecords;

            try
            {
                //Adde By Abhishek kamble 1-May-2014 start
                using (CommonFunctions commonFunction = new CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new GridParams(Convert.ToInt32(formCollection["page"]), Convert.ToInt32(formCollection["rows"]), formCollection["sidx"], formCollection["sord"], Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                    {
                        return null;
                    }
                }
                //Adde By Abhishek kamble 1-May-2014 end
                var jsonData = new
                {
                    rows = dashboardBAL.FundVsExpenditureBAL(Convert.ToInt32(formCollection["page"]) - 1, Convert.ToInt32(formCollection["rows"]),
                                                            formCollection["sidx"], formCollection["sord"], out totalRecords,
                                                            formCollection["fundType"]),
                    total = totalRecords <= Convert.ToInt32(formCollection["rows"]) ? 1 : (totalRecords % Convert.ToInt32(formCollection["rows"]) == 0 ? totalRecords / Convert.ToInt32(formCollection["rows"]) : (totalRecords / Convert.ToInt32(formCollection["rows"]) + 1)),
                    page = Convert.ToInt32(formCollection["page"]),
                    records = totalRecords
                };
                return Json(jsonData, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);

                return Json(String.Empty);
            }
        }


        /// <summary>
        /// Listing of Description & Expenditure Summary
        /// </summary>
        /// <param name="formCollection"></param>
        /// <returns></returns>
        [HttpPost]
        [Audit]
        public ActionResult ListExpenditureSummary(FormCollection formCollection)
        {
            dashboardBAL = new DashboardBAL();
            int totalRecords;

            try
            {
                //Adde By Abhishek kamble 1-May-2014 start
                using (CommonFunctions commonFunction = new CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new GridParams(Convert.ToInt32(formCollection["page"]), Convert.ToInt32(formCollection["rows"]), formCollection["sidx"], formCollection["sord"], Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                    {
                        return null;
                    }
                }
                //Adde By Abhishek kamble 1-May-2014 end
                var jsonData = new
                {
                    rows = dashboardBAL.ExpenditureSummaryBAL(Convert.ToInt32(formCollection["page"]) - 1, Convert.ToInt32(formCollection["rows"]),
                                                           formCollection["sidx"], formCollection["sord"], out totalRecords,
                                                           formCollection["fundType"], Convert.ToInt32(formCollection["stateNdCode"] == null ? "0" : formCollection["stateNdCode"]),
                                                           Convert.ToInt32(formCollection["piuNdCode"] == null ? "0" : formCollection["piuNdCode"]),
                                                           Convert.ToInt32(formCollection["fundingAgency"])),
                    total = totalRecords <= Convert.ToInt32(formCollection["rows"]) ? 1 : (totalRecords % Convert.ToInt32(formCollection["rows"]) == 0 ? totalRecords / Convert.ToInt32(formCollection["rows"]) : (totalRecords / Convert.ToInt32(formCollection["rows"]) + 1)),
                    page = Convert.ToInt32(formCollection["page"]),
                    records = totalRecords
                };
                return Json(jsonData, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return Json(String.Empty);
            }
        }


        /// <summary>
        /// Listing of Fundwise No. of PIUs, No. of PIUs Closed Accounts, Un Reconciled details
        /// </summary>
        /// <param name="formCollection"></param>
        /// <returns></returns>
        [HttpPost]
        [Audit]
        public ActionResult ListStatusMonitoringReport(FormCollection formCollection)
        {
            dashboardBAL = new DashboardBAL();
            int totalRecords;

            try
            {
                //Adde By Abhishek kamble 1-May-2014 start
                using (CommonFunctions commonFunction = new CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new GridParams(Convert.ToInt32(formCollection["page"]), Convert.ToInt32(formCollection["rows"]), formCollection["sidx"], formCollection["sord"], Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                    {
                        return null;
                    }
                }
                //Adde By Abhishek kamble 1-May-2014 end
                var jsonData = new
                {
                    rows = dashboardBAL.StatusMonitoringReportBAL(Convert.ToInt32(formCollection["page"]) - 1, Convert.ToInt32(formCollection["rows"]),
                                                            formCollection["sidx"], formCollection["sord"], out totalRecords,
                                                            formCollection["fundType"]),
                    total = totalRecords <= Convert.ToInt32(formCollection["rows"]) ? 1 : (totalRecords % Convert.ToInt32(formCollection["rows"]) == 0 ? totalRecords / Convert.ToInt32(formCollection["rows"]) : (totalRecords / Convert.ToInt32(formCollection["rows"]) + 1)),
                    page = Convert.ToInt32(formCollection["page"]),
                    records = totalRecords
                };
                return Json(jsonData, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return Json(String.Empty);
            }
        }


        /// <summary>
        /// Listing of PIU wise last month closed details
        /// </summary>
        /// <param name="formCollection"></param>
        /// <returns></returns>
        [HttpPost]
        [Audit]
        public ActionResult ListStatusMonitoringPIUReport(FormCollection formCollection)
        {
            dashboardBAL = new DashboardBAL();
            int totalRecords;

            try
            {
                //Adde By Abhishek kamble 1-May-2014 start
                using (CommonFunctions commonFunction = new CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new GridParams(Convert.ToInt32(formCollection["page"]), Convert.ToInt32(formCollection["rows"]), formCollection["sidx"], formCollection["sord"], Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                    {
                        return null;
                    }
                }
                //Adde By Abhishek kamble 1-May-2014 end
                var jsonData = new
                {
                    rows = dashboardBAL.StatusMonitoringPIUReportBAL(Convert.ToInt32(formCollection["page"]) - 1, Convert.ToInt32(formCollection["rows"]),
                                                           formCollection["sidx"], formCollection["sord"], out totalRecords,
                                                           Convert.ToInt32(formCollection["stateNdCode"]), formCollection["fundType"]),
                    total = totalRecords <= Convert.ToInt32(formCollection["rows"]) ? 1 : (totalRecords % Convert.ToInt32(formCollection["rows"]) == 0 ? totalRecords / Convert.ToInt32(formCollection["rows"]) : (totalRecords / Convert.ToInt32(formCollection["rows"]) + 1)),
                    page = Convert.ToInt32(formCollection["page"]),
                    records = totalRecords
                };
                return Json(jsonData, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return Json(String.Empty);
            }
        }



        /// <summary>
        /// returns List for Yearwise & Cumulative Expenditure Details
        /// </summary>
        /// <returns></returns>
        [Audit]
        public ActionResult ExpenditureTrend()
        {
            dashboardBAL = new DashboardBAL();

            try
            {
                List<USP_ACC_DB_Expn_Trend_Result> List = dashboardBAL.ExpenditureTrendBAL(Request.Params["fundType"], Convert.ToInt32(Request.Params["stateCode"]),
                                                                            Convert.ToInt32(Request.Params["piuCode"]), Convert.ToInt32(Request.Params["collaboration"]));

                List<YearwiseCumExpnChartModel> lstChartYearwise = new List<YearwiseCumExpnChartModel>();


                foreach (var p in List)
                {
                    YearwiseCumExpnChartModel chart = new YearwiseCumExpnChartModel();
                    chart.MYear = p.YEAR_ID.ToString();
                    chart.YearlyExpn = (p.Yearly_Expn).ToString();
                    chart.CumExpn = (p.Cum_Expn).ToString();
                    lstChartYearwise.Add(chart);
                }

                return new JsonResult
                {
                    Data = lstChartYearwise
                };
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return new JsonResult
                {
                    Data = string.Empty
                };
            }
        }


        #endregion

        #region Physical Details

        /// <summary>
        /// Layout for Physical ot Technical Details
        /// </summary>
        /// <returns></returns>
        [Audit]
        public ActionResult PhysicalLayout()
        {
            WBankDashboardLayoutViewModel wBankDashboardLayoutViewModel = new WBankDashboardLayoutViewModel();
            CommonFunctions objCommonFuntion = new CommonFunctions();
            List<SelectListItem> lstCollaborations = new List<SelectListItem>();
            try
            {
                wBankDashboardLayoutViewModel.STATE_ND_CODE = 0;
                wBankDashboardLayoutViewModel.PIU_ND_CODE = 0;
                wBankDashboardLayoutViewModel.FUND_TYPE_LIST = objCommonFuntion.PopulateFundTypes(false);

                if (PMGSYSession.Current.RoleCode == 25)     //For Mord populate all collaboraions
                {
                    wBankDashboardLayoutViewModel.COLLABORATIONS_LIST = objCommonFuntion.PopulateFundingAgency(true);
                }
                else if (PMGSYSession.Current.RoleCode == 45)   //For World Bank
                {

                    lstCollaborations.Insert(0, (new SelectListItem { Text = "World Bank (RRP-I)", Value = "3", Selected = true }));
                    lstCollaborations.Insert(1, (new SelectListItem { Text = "World Bank (RRP-II)", Value = "4" }));
                    wBankDashboardLayoutViewModel.COLLABORATIONS_LIST = lstCollaborations;
                }
                wBankDashboardLayoutViewModel.MAST_STATE_CODE_PHYSICAL = 0;
                wBankDashboardLayoutViewModel.MAST_DISTRICT_CODE_PHYSICAL = 0;
                wBankDashboardLayoutViewModel.PROPOSAL_TYPE = "P";
                return View(wBankDashboardLayoutViewModel);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                throw ex;
            }
        }


        /// <summary>
        /// Lising of Technical Details for all states
        /// </summary>
        /// <param name="formCollection"></param>
        /// <returns></returns>
        [HttpPost]
        [Audit]
        public ActionResult ListAllStatesTechnicalDetails(FormCollection formCollection)
        {
            dashboardBAL = new DashboardBAL();
            int totalRecords;

            try
            {
                //Adde By Abhishek kamble 1-May-2014 start
                using (CommonFunctions commonFunction = new CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new GridParams(Convert.ToInt32(formCollection["page"]), Convert.ToInt32(formCollection["rows"]), formCollection["sidx"], formCollection["sord"], Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                    {
                        return null;
                    }
                }
                //Adde By Abhishek kamble 1-May-2014 end
                var jsonData = new
                {
                    rows = dashboardBAL.AllStatesTechnicalDetailsBAL(Convert.ToInt32(formCollection["page"]) - 1, Convert.ToInt32(formCollection["rows"]),
                                                           formCollection["sidx"], formCollection["sord"], out totalRecords,
                                                           Convert.ToInt32(formCollection["fundingAgency"])),
                    total = totalRecords <= Convert.ToInt32(formCollection["rows"]) ? 1 : (totalRecords % Convert.ToInt32(formCollection["rows"]) == 0 ? totalRecords / Convert.ToInt32(formCollection["rows"]) : (totalRecords / Convert.ToInt32(formCollection["rows"]) + 1)),
                    page = Convert.ToInt32(formCollection["page"]),
                    records = totalRecords
                };
                return Json(jsonData, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return Json(String.Empty);
            }
        }


        /// <summary>
        /// Technical Details of all Districts under particular State
        /// </summary>
        /// <param name="formCollection"></param>
        /// <returns></returns>
        [HttpPost]
        [Audit]
        public ActionResult ListDistrictwiseTechnicalDetails(FormCollection formCollection)
        {
            dashboardBAL = new DashboardBAL();
            int totalRecords;

            try
            {
                //Adde By Abhishek kamble 1-May-2014 start
                using (CommonFunctions commonFunction = new CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new GridParams(Convert.ToInt32(formCollection["page"]), Convert.ToInt32(formCollection["rows"]), formCollection["sidx"], formCollection["sord"], Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                    {
                        return null;
                    }
                }
                //Adde By Abhishek kamble 1-May-2014 end
                var jsonData = new
                {
                    rows = dashboardBAL.DistrictwiseTechnicalDetailsBAL(Convert.ToInt32(formCollection["page"]) - 1, Convert.ToInt32(formCollection["rows"]),
                                                           formCollection["sidx"], formCollection["sord"], out totalRecords,
                                                           Convert.ToInt32(formCollection["stateCode"]), Convert.ToInt32(formCollection["fundingAgency"])),
                    total = totalRecords <= Convert.ToInt32(formCollection["rows"]) ? 1 : (totalRecords % Convert.ToInt32(formCollection["rows"]) == 0 ? totalRecords / Convert.ToInt32(formCollection["rows"]) : (totalRecords / Convert.ToInt32(formCollection["rows"]) + 1)),
                    page = Convert.ToInt32(formCollection["page"]),
                    records = totalRecords
                };
                return Json(jsonData, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return Json(String.Empty);
            }
        }


        /// <summary>
        /// Technical Details of all Blocks under particular District
        /// </summary>
        /// <param name="formCollection"></param>
        /// <returns></returns>
        [HttpPost]
        [Audit]
        public ActionResult ListBlockwiseTechnicalDetails(FormCollection formCollection)
        {
            dashboardBAL = new DashboardBAL();
            int totalRecords;

            try
            {
                //Adde By Abhishek kamble 1-May-2014 start
                using (CommonFunctions commonFunction = new CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new GridParams(Convert.ToInt32(formCollection["page"]), Convert.ToInt32(formCollection["rows"]), formCollection["sidx"], formCollection["sord"], Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                    {
                        return null;
                    }
                }
                //Adde By Abhishek kamble 1-May-2014 end
                var jsonData = new
                {
                    rows = dashboardBAL.BlockwiseTechnicalDetailsBAL(Convert.ToInt32(formCollection["page"]) - 1, Convert.ToInt32(formCollection["rows"]),
                                                           formCollection["sidx"], formCollection["sord"], out totalRecords,
                                                            Convert.ToInt32(formCollection["stateCode"]), Convert.ToInt32(formCollection["districtCode"]),
                                                           Convert.ToInt32(formCollection["fundingAgency"])),
                    total = totalRecords <= Convert.ToInt32(formCollection["rows"]) ? 1 : (totalRecords % Convert.ToInt32(formCollection["rows"]) == 0 ? totalRecords / Convert.ToInt32(formCollection["rows"]) : (totalRecords / Convert.ToInt32(formCollection["rows"]) + 1)),
                    page = Convert.ToInt32(formCollection["page"]),
                    records = totalRecords
                };
                return Json(jsonData, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return Json(String.Empty);
            }
        }


        ///// <summary>
        ///// Sanction Vs Awarded vs Completed vs Progress Works Details
        ///// </summary>
        ///// <returns></returns>
        //[HttpPost]
        //[Audit]
        //public ActionResult WorksColumnChart()
        //{
        //    dashboardBAL = new DashboardBAL();

        //    try
        //    {
        //        List<USP_TECH_DASH_R1_Result> List = dashboardBAL.WorksColumnChartBAL(Convert.ToInt32(Request.Params["stateCode"]),
        //                                            Convert.ToInt32(Request.Params["districtCode"]), Convert.ToInt32(Request.Params["fundingAgency"]));

        //        List<WorksColumnChartModel> lstChart = new List<WorksColumnChartModel>();


        //        foreach (var p in List)
        //        {
        //            WorksColumnChartModel chart = new WorksColumnChartModel();
        //            chart.Year = p.IMS_YEAR.ToString();
        //            chart.SanctionWorks = p.SANC_WORKS.ToString();
        //            chart.AwardedWorks = p.AWARDED_WORKS.ToString();
        //            chart.CompletedWorks = p.COMP_WORKS.ToString();
        //            chart.ProgWorks = p.PROG_WORKS.ToString();

        //            lstChart.Add(chart);
        //        }

        //        return new JsonResult
        //        {
        //            Data = lstChart
        //        };
        //    }
        //    catch (Exception ex)
        //    {
        //        Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
        //        return new JsonResult
        //        {
        //            Data = string.Empty
        //        };
        //    }
        //}




        ///// <summary>
        ///// Sanction Vs Awarded vs Completed vs Progress Length Details
        ///// </summary>
        ///// <returns></returns>
        //[HttpPost]
        //[Audit]
        //public ActionResult LengthColumnChart()
        //{
        //    dashboardBAL = new DashboardBAL();

        //    try
        //    {
        //        List<LengthColumnChartStoredProcModel> List = dashboardBAL.LengthColumnChartBAL(Convert.ToInt32(Request.Params["stateCode"]),
        //                                            Convert.ToInt32(Request.Params["districtCode"]), Convert.ToInt32(Request.Params["fundingAgency"]));

        //        List<LengthColumnChartModel> lstChart = new List<LengthColumnChartModel>();

        //        foreach (var p in List)
        //        {
        //            LengthColumnChartModel chart = new LengthColumnChartModel();
        //            if (Request.Params["proposalType"].Trim().Equals("P"))
        //            {
        //                chart.Year = p.IMS_YEAR.ToString();
        //                chart.SanctionRdLength = p.ROAD_LEN.ToString();
        //                chart.AwardedRdLength = p.AWARD_RD_LEN.ToString();
        //                chart.CompletedRdLength = p.COMP_RD_LEN.ToString();
        //                chart.ProgRdLength = p.PROG_RD_LEN.ToString();
        //            }
        //            else if (Request.Params["proposalType"].Trim().Equals("L"))
        //            {
        //                chart.Year = p.IMS_YEAR.ToString();
        //                chart.SanctionLSBLength = p.LSB_LEN.ToString();
        //                chart.AwardedLSBLength = p.AWARD_LSB_LEN.ToString();
        //                chart.CompletedLSBLength = p.COMP_LSB_LEN.ToString();
        //                chart.ProgLSBLength = p.PROG_LSB_LEN.ToString();
        //            }

        //            lstChart.Add(chart);
        //        }

        //        return new JsonResult
        //        {
        //            Data = lstChart
        //        };
        //    }
        //    catch (Exception ex)
        //    {
        //        Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
        //        return new JsonResult
        //        {
        //            Data = string.Empty
        //        };
        //    }
        //}


        ///// <summary>
        ///// Sanction Cost Vs Awarded cost vs Maintenance Expenditure Details
        ///// </summary>
        ///// <returns></returns>
        //[HttpPost]
        //[Audit]
        //public ActionResult CostColumnChart()
        //{
        //    dashboardBAL = new DashboardBAL();

        //    try
        //    {
        //        List<CostColumnChartStoredProcModel> List = dashboardBAL.CostColumnChartBAL(Convert.ToInt32(Request.Params["stateCode"]),
        //                                            Convert.ToInt32(Request.Params["districtCode"]), Convert.ToInt32(Request.Params["fundingAgency"]));

        //        List<CostColumnChartModel> lstChart = new List<CostColumnChartModel>();

        //        foreach (var p in List)
        //        {
        //            CostColumnChartModel chart = new CostColumnChartModel();
        //            chart.Year = p.IMS_YEAR.ToString();
        //            chart.SanctionTotal = (p.WORK_TOTAL + p.MAINT_TOTAL).ToString();
        //            chart.AwardedTotal = (p.AWARD_WORK_AMT + p.AWARD_MAINT_AMT).ToString();
        //            chart.Exp = p.WORK_EXP.ToString();

        //            lstChart.Add(chart);
        //        }

        //        return new JsonResult
        //        {
        //            Data = lstChart
        //        };
        //    }
        //    catch (Exception ex)
        //    {
        //        Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
        //        return new JsonResult
        //        {
        //            Data = string.Empty
        //        };
        //    }
        //}


        [HttpPost]
        public ActionResult WorkLenghtExpYearWiseStateWiseGrid(FormCollection formCollection)
        {
            dashboardBAL = new DashboardBAL();
            int totalRecords;
            bool isYearWise; 
            int year = 0;   
          
            try
            {
                if (!(String.IsNullOrEmpty(Request.Params["ImsYear"])))
                {
                    year = Convert.ToInt32(Request.Params["ImsYear"]);
                }

                if (!(String.IsNullOrEmpty(Request.Params["isYearWise"])))
                {
                    isYearWise = Convert.ToBoolean(Request.Params["isYearWise"]);
                }
                else
                {
                    return null;
                }
                using (CommonFunctions commonFunction = new CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new GridParams(Convert.ToInt32(formCollection["page"]), Convert.ToInt32(formCollection["rows"]), formCollection["sidx"], formCollection["sord"], Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                    {
                        return null;
                    }
                }
                
                var jsonData = new
                {
                    rows = dashboardBAL.WorkLenghtExpYearWiseStateWiseGrid(Convert.ToInt32(formCollection["page"]) - 1, Convert.ToInt32(formCollection["rows"]),
                                                           formCollection["sidx"], formCollection["sord"], out totalRecords,
                                                            Convert.ToInt32(formCollection["stateCode"]), Convert.ToInt32(formCollection["districtCode"]),
                                                           Convert.ToInt32(formCollection["fundingAgency"]),year,isYearWise),
                    total = totalRecords <= Convert.ToInt32(formCollection["rows"]) ? 1 : (totalRecords % Convert.ToInt32(formCollection["rows"]) == 0 ? totalRecords / Convert.ToInt32(formCollection["rows"]) : (totalRecords / Convert.ToInt32(formCollection["rows"]) + 1)),
                    page = Convert.ToInt32(formCollection["page"]),
                    records = totalRecords
                };
                return Json(jsonData, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return Json(String.Empty);
            }

                    
        }                                 

        /// <summary>
        /// Column Chart Year wise and State wise
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Audit]
        public ActionResult WorkLenghtExpYearWiseStateWiseColumnChart()
        {
            dashboardBAL = new DashboardBAL();
            try
            {
                bool isYearWise;

                int year = 0;

                if (!(String.IsNullOrEmpty(Request.Params["ImsYear"])))
                {
                    year = Convert.ToInt32(Request.Params["ImsYear"]);
                }

                if (!(String.IsNullOrEmpty(Request.Params["isYearWise"])))
                {
                    isYearWise = Convert.ToBoolean(Request.Params["isYearWise"]);
                }
                else {
                    return null;
                }
                List<USP_DSS_ACHIEVEMENT_REPORT_Result> list = dashboardBAL.WorksLengthExpYearWiseStateWiseColumnChartDAL(Convert.ToInt32(Request.Params["stateCode"]), Convert.ToInt32(Request.Params["districtCode"]), Convert.ToInt32(Request.Params["fundingAgency"]),year, isYearWise);

                return new JsonResult
                {
                    Data = list
                };
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return new JsonResult
                {
                    Data = string.Empty
                };
            }

        }

        #endregion

        #region Quality Details
        [Audit]
        public ActionResult QualityLayout()
        {
            WBankDashboardLayoutViewModel wBankDashboardViewModel = new WBankDashboardLayoutViewModel();
            wBankDashboardViewModel.MAST_STATE_CODE_QUALITY = 0;
            return View(wBankDashboardViewModel);
        }


        /// <summary>
        /// All States  NQM SQM wise Insp Count & Grading %
        /// </summary>
        /// <param name="formCollection"></param>
        /// <returns></returns>
        [HttpPost]
        [Audit]
        public ActionResult ListAllStatesInspectionDetails(FormCollection formCollection)
        {
            dashboardBAL = new DashboardBAL();
            int totalRecords;
            try
            {
                //Adde By Abhishek kamble 1-May-2014 start
                using (CommonFunctions commonFunction = new CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new GridParams(Convert.ToInt32(formCollection["page"]), Convert.ToInt32(formCollection["rows"]), formCollection["sidx"], formCollection["sord"], Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                    {
                        return null;
                    }
                }
                //Adde By Abhishek kamble 1-May-2014 end
                var jsonData = new
                {
                    rows = dashboardBAL.AllStatesInspectionDetailsBAL(Convert.ToInt32(formCollection["page"]) - 1, Convert.ToInt32(formCollection["rows"]),
                                                           formCollection["sidx"], formCollection["sord"], out totalRecords),
                    total = totalRecords <= Convert.ToInt32(formCollection["rows"]) ? 1 : (totalRecords % Convert.ToInt32(formCollection["rows"]) == 0 ? totalRecords / Convert.ToInt32(formCollection["rows"]) : (totalRecords / Convert.ToInt32(formCollection["rows"]) + 1)),
                    page = Convert.ToInt32(formCollection["page"]),
                    records = totalRecords
                };
                return Json(jsonData, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return Json(String.Empty);
            }
        }


        /// <summary>
        /// List Districtwise NQM/SQM Inspections & % Grading
        /// </summary>
        /// <param name="formCollection"></param>
        /// <returns></returns>
        [HttpPost]
        [Audit]
        public ActionResult ListDistrictwiseInspectionDetails(FormCollection formCollection)
        {
            dashboardBAL = new DashboardBAL();
            int totalRecords;
            try
            {
                //Adde By Abhishek kamble 1-May-2014 start
                using (CommonFunctions commonFunction = new CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new GridParams(Convert.ToInt32(formCollection["page"]), Convert.ToInt32(formCollection["rows"]), formCollection["sidx"], formCollection["sord"], Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                    {
                        return null;
                    }
                }
                //Adde By Abhishek kamble 1-May-2014 end
                var jsonData = new
                {
                    rows = dashboardBAL.DistrictwiseInspectionDetailsBAL(Convert.ToInt32(formCollection["page"]) - 1, Convert.ToInt32(formCollection["rows"]),
                                                           formCollection["sidx"], formCollection["sord"], out totalRecords,
                                                           Convert.ToInt32(formCollection["stateCode"])),
                    total = totalRecords <= Convert.ToInt32(formCollection["rows"]) ? 1 : (totalRecords % Convert.ToInt32(formCollection["rows"]) == 0 ? totalRecords / Convert.ToInt32(formCollection["rows"]) : (totalRecords / Convert.ToInt32(formCollection["rows"]) + 1)),
                    page = Convert.ToInt32(formCollection["page"]),
                    records = totalRecords
                };
                return Json(jsonData, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return Json(String.Empty);
            }
        }



        /// <summary>
        /// Monitorwise Completed & In Progress Roads Grading Details
        /// </summary>
        /// <param name="formCollection"></param>
        /// <returns></returns>
        [HttpPost]
        [Audit]
        public ActionResult ListMonitorwiseComplAndProgressDetails(FormCollection formCollection)
        {
            dashboardBAL = new DashboardBAL();
            int totalRecords;
            try
            {
                //Adde By Abhishek kamble 1-May-2014 start
                using (CommonFunctions commonFunction = new CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new GridParams(Convert.ToInt32(formCollection["page"]), Convert.ToInt32(formCollection["rows"]), formCollection["sidx"], formCollection["sord"], Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                    {
                        return null;
                    }
                }
                //Adde By Abhishek kamble 1-May-2014 end
                var jsonData = new
                {


                    rows = dashboardBAL.MonitorwiseComplAndProgressDetailsBAL(Convert.ToInt32(formCollection["page"]) - 1, Convert.ToInt32(formCollection["rows"]),
                                                           formCollection["sidx"], formCollection["sord"], out totalRecords,
                                                           Convert.ToInt32(formCollection["stateCode"]), formCollection["qmType"]),
                    total = totalRecords <= Convert.ToInt32(formCollection["rows"]) ? 1 : (totalRecords % Convert.ToInt32(formCollection["rows"]) == 0 ? totalRecords / Convert.ToInt32(formCollection["rows"]) : (totalRecords / Convert.ToInt32(formCollection["rows"]) + 1)),
                    page = Convert.ToInt32(formCollection["page"]),
                    records = totalRecords
                };
                return Json(jsonData, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return Json(String.Empty);
            }
        }


        /// <summary>
        /// All States Grading Pie Chart for percentage of S, RI, U
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Audit]
        public ActionResult GetAllStatesGradingPieChart()
        {
            dashboardBAL = new DashboardBAL();
            try
            {
                List<AllStatesQualityPieChartModel> lstChart = dashboardBAL.AllStatesGradingPieChartBAL(Convert.ToInt32(Request.Params["stateCode"]),
                                                                                            Request.Params["qmType"]);
                return new JsonResult
                {
                    Data = lstChart
                };
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return new JsonResult
                {
                    Data = string.Empty
                };
            }
        }

        /// <summary>
        /// Yearly % Grading for All States 
        /// as well as - In case of Level 4 report all Districts of particular State
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Audit]
        public ActionResult GetYearlyGradingLineChart()
        {
            dashboardBAL = new DashboardBAL();
            try
            {
                List<USP_QM_STATE_DISTRICT_DASH_S1_Result> List = dashboardBAL.YearlyGradingLineChartBAL(Convert.ToInt32(Request.Params["stateCode"]));

                List<YearlyGradingLineChartModel> lstChartYearwise = new List<YearlyGradingLineChartModel>();

                if (Request.Params["qmType"].Equals("I"))
                {
                    foreach (var p in List)
                    {
                        YearlyGradingLineChartModel chart = new YearlyGradingLineChartModel();
                        chart.Year = p.INSP_YEAR.ToString();
                        chart.SPercent = Convert.ToDecimal(p.I_GRADE_1_PER);
                        chart.SRIPercent = Convert.ToDecimal(p.I_GRADE_2_PER);
                        chart.UPercent = Convert.ToDecimal(p.I_GRADE_3_PER);
                        lstChartYearwise.Add(chart);
                    }
                }
                else if (Request.Params["qmType"].Equals("S"))
                {
                    foreach (var p in List)
                    {
                        YearlyGradingLineChartModel chart = new YearlyGradingLineChartModel();
                        chart.Year = p.INSP_YEAR.ToString();
                        chart.SPercent = Convert.ToDecimal(p.S_GRADE_1_PER);
                        chart.SRIPercent = Convert.ToDecimal(p.S_GRADE_2_PER);
                        chart.UPercent = Convert.ToDecimal(p.S_GRADE_3_PER);
                        lstChartYearwise.Add(chart);
                    }
                }

                return new JsonResult
                {
                    Data = lstChartYearwise
                };
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return new JsonResult
                {
                    Data = string.Empty
                };
            }
        }


        /// <summary>
        /// Monitorwise Grading % in inspected states
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Audit]
        public ActionResult GetMonitorsGradingColumnChart()
        {
            dashboardBAL = new DashboardBAL();
            try
            {
                int stateCode = Convert.ToInt32(Request.Params["stateCode"]);
                List<USP_QM_STATE_DISTRICT_DASH_S3_Result> List = dashboardBAL.MonitorsGradingColumnChartBAL(stateCode,
                                                Convert.ToInt32(Request.Params["monitorCode"]), Request.Params["qmType"]);

                List<MonitorsStatewiseGradingColumnChartModel> lstChart = new List<MonitorsStatewiseGradingColumnChartModel>();

                foreach (var p in List)
                {
                    MonitorsStatewiseGradingColumnChartModel chart = new MonitorsStatewiseGradingColumnChartModel();
                    if (Request.Params["qmType"].Trim().Equals("I"))
                    {
                        if (stateCode == 0)
                            chart.Name = p.MAST_STATE_NAME;
                        else
                            chart.Name = p.MAST_DISTRICT_NAME;

                        chart.SPercent = Convert.ToDecimal(p.I_GRADE_1_PER);
                        chart.SRIPercent = Convert.ToDecimal(p.I_GRADE_2_PER);
                        chart.UPercent = Convert.ToDecimal(p.I_GRADE_3_PER);

                    }
                    else if (Request.Params["qmType"].Trim().Equals("S"))
                    {
                        if (stateCode == 0)
                            chart.Name = p.MAST_STATE_NAME;
                        else
                            chart.Name = p.MAST_DISTRICT_NAME;

                        chart.SPercent = Convert.ToDecimal(p.S_GRADE_1_PER);
                        chart.SRIPercent = Convert.ToDecimal(p.S_GRADE_2_PER);
                        chart.UPercent = Convert.ToDecimal(p.S_GRADE_3_PER);
                    }

                    lstChart.Add(chart);
                }

                return new JsonResult
                {
                    Data = lstChart
                };
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return new JsonResult
                {
                    Data = string.Empty
                };
            }
        }

        #endregion

        #region NRRDA_DASHBOARDS

        /// <summary>
        /// layout view for NRRDA Dashboard
        /// </summary>
        /// <returns></returns>
        public ActionResult NRRDADashboardLayout()
        {
            try
            {
                DashboardDAL objDAL = new DashboardDAL();
                CommonFunctions objCommon = new CommonFunctions();
                NRRDADashboardFilterViewModel model = new NRRDADashboardFilterViewModel();
                ViewBag.STATE_CODES = objCommon.PopulateStates(false);
                model.lstYears = new SelectList(objCommon.PopulateFinancialYear(false, false).ToList(), "Value", "Text").ToList();
                model.lstCollaborations = objCommon.PopulateFundingAgency(true);
                model.lstAgency = objDAL.PopulateGovernmentAgencies();
                return View(model);
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// returns the data related to Proposals in form of chart
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GetTrendsOfProposalsSanctioned(FormCollection frmCollection)
        {
            dashboardBAL = new DashboardBAL();
            try
            {
                string stateList = string.Empty;
                int tillYear = 0;
                int agency = 0;
                int collaboration = 0;
                if (!String.IsNullOrEmpty(Request.Params["STATE_CODES"]))
                {
                    stateList = Request.Params["STATE_CODES"];
                }

                if (!String.IsNullOrEmpty(Request.Params["Year"]))
                {
                    tillYear = Convert.ToInt32(Request.Params["Year"]);
                }

                if (!String.IsNullOrEmpty(Request.Params["Agency"]))
                {
                    agency = Convert.ToInt32(Request.Params["Agency"]);
                }

                if (!String.IsNullOrEmpty(Request.Params["Collaboration"]))
                {
                    collaboration = Convert.ToInt32(Request.Params["Collaboration"]);
                }

                List<USP_TECH_TRENDS_CHART_PPT_Result> lstDetails = new List<USP_TECH_TRENDS_CHART_PPT_Result>();
                lstDetails = dashboardBAL.GetProposalTrendsDetailsBAL(stateList, tillYear, 1, agency, collaboration);
                List<Array> lstArray = new List<Array>();
                var lstStates = lstDetails.Select(m => m.LOCATION_NAME).Distinct().ToList();
                foreach (var item in lstStates)
                {
                    lstArray.Add(lstDetails.Where(m => m.LOCATION_NAME == item).Select(m => m.ROAD_LEN).ToArray());
                }
                var data = lstDetails.GroupBy(m => m.LOCATION_NAME, m => new { m.ROAD_LEN }).ToList();

                return Json
                (
                    new
                    {
                        LOCATION_NAME = lstDetails.Select(m => m.LOCATION_NAME).Distinct().ToList(),
                        YEARS = lstDetails.Select(m => m.MAST_YEAR_TEXT).Distinct().ToList(),
                        ROAD_LEN = lstArray,
                        LSB_LEN = lstDetails.Select(m => m.LSB_LEN).ToList()
                    }
                );


            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// returns the data related to Proposals in form of chart
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GetTrendsOfProposalsSanctionedDemo(FormCollection frmCollection)
        {
            DashboardDAL dashboardDAL = new DashboardDAL();
            try
            {
                string stateList = string.Empty;
                int tillYear = 0;
                int agency = 0;
                int collaboration = 0;
                if (!String.IsNullOrEmpty(Request.Params["STATE_CODES"]))
                {
                    stateList = Request.Params["STATE_CODES"];
                }

                if (!String.IsNullOrEmpty(Request.Params["Year"]))
                {
                    tillYear = Convert.ToInt32(Request.Params["Year"]);
                }

                if (!String.IsNullOrEmpty(Request.Params["Agency"]))
                {
                    agency = Convert.ToInt32(Request.Params["Agency"]);
                }

                if (!String.IsNullOrEmpty(Request.Params["Collaboration"]))
                {
                    collaboration = Convert.ToInt32(Request.Params["Collaboration"]);
                }

                List<USP_TECH_TRENDS_CHART_PPT_Result> lstDetails = new List<USP_TECH_TRENDS_CHART_PPT_Result>();
                lstDetails = dashboardDAL.GetProposalTrendsDetailsDAL(stateList, tillYear, 1, agency, collaboration);
                List<Array> lstArray = new List<Array>();
                var lstStates = lstDetails.Select(m => m.LOCATION_NAME).Distinct().ToList();
                foreach (var item in lstStates)
                {
                    lstArray.Add(lstDetails.Where(m => m.LOCATION_NAME == item).Select(m => m.ROAD_LEN).ToArray());
                }
                var data = lstDetails.GroupBy(m => m.LOCATION_NAME, m => new { m.ROAD_LEN }).ToList();

                return Json
                (
                    new
                    {
                        LOCATION_NAME = lstDetails.Select(m => m.LOCATION_NAME).Distinct().ToList(),
                        YEARS = lstDetails.Select(m => m.MAST_YEAR_TEXT).Distinct().ToList(),
                        ROAD_LEN = lstArray,
                        LSB_LEN = lstDetails.Select(m => m.LSB_LEN).ToList()
                    }
                );


            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// returns the data related to Proposals in form of chart
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GetTrendsOfProposalsCompleted(FormCollection frmCollection)
        {
            dashboardBAL = new DashboardBAL();
            try
            {
                string stateList = string.Empty;
                int tillYear = 0;
                int agency = 0;
                int collaboration = 0;
                if (!String.IsNullOrEmpty(Request.Params["STATE_CODES"]))
                {
                    stateList = Request.Params["STATE_CODES"];
                }

                if (!String.IsNullOrEmpty(Request.Params["Year"]))
                {
                    tillYear = Convert.ToInt32(Request.Params["Year"]);
                }

                if (!String.IsNullOrEmpty(Request.Params["Agency"]))
                {
                    agency = Convert.ToInt32(Request.Params["Agency"]);
                }

                if (!String.IsNullOrEmpty(Request.Params["Collaboration"]))
                {
                    collaboration = Convert.ToInt32(Request.Params["Collaboration"]);
                }

                List<USP_TECH_TRENDS_CHART_PPT_Result> lstDetails = new List<USP_TECH_TRENDS_CHART_PPT_Result>();
                lstDetails = dashboardBAL.GetProposalTrendsDetailsBAL(stateList, tillYear, 2, agency, collaboration);
                List<Array> lstArray = new List<Array>();
                var lstStates = lstDetails.Select(m => m.LOCATION_NAME).Distinct().ToList();
                foreach (var item in lstStates)
                {
                    lstArray.Add(lstDetails.Where(m => m.LOCATION_NAME == item).Select(m => m.ROAD_LEN).ToArray());
                }
                var data = lstDetails.GroupBy(m => m.LOCATION_NAME, m => new { m.ROAD_LEN }).ToList();

                return Json
                (
                    new
                    {
                        LOCATION_NAME = lstDetails.Select(m => m.LOCATION_NAME).Distinct().ToList(),
                        YEARS = lstDetails.Select(m => m.MAST_YEAR_TEXT).Distinct().ToList(),
                        ROAD_LEN = lstArray,
                        LSB_LEN = lstDetails.Select(m => m.LSB_LEN).ToList()
                    }
                );


            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// returns the data related to Proposals in form of chart
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GetTrendsOfProposalsExpenditure(FormCollection frmCollection)
        {
            dashboardBAL = new DashboardBAL();
            try
            {
                string stateList = string.Empty;
                int tillYear = 0;
                int agency = 0;
                int collaboration = 0;
                if (!String.IsNullOrEmpty(Request.Params["STATE_CODES"]))
                {
                    stateList = Request.Params["STATE_CODES"];
                }

                if (!String.IsNullOrEmpty(Request.Params["Year"]))
                {
                    tillYear = Convert.ToInt32(Request.Params["Year"]);
                }

                if (!String.IsNullOrEmpty(Request.Params["Agency"]))
                {
                    agency = Convert.ToInt32(Request.Params["Agency"]);
                }

                if (!String.IsNullOrEmpty(Request.Params["Collaboration"]))
                {
                    collaboration = Convert.ToInt32(Request.Params["Collaboration"]);
                }

                List<USP_TECH_TRENDS_CHART_PPT_Result> lstDetails = new List<USP_TECH_TRENDS_CHART_PPT_Result>();
                lstDetails = dashboardBAL.GetProposalTrendsDetailsBAL(stateList, tillYear, 3, agency, collaboration);
                List<Array> lstArray = new List<Array>();
                var lstStates = lstDetails.Select(m => m.LOCATION_NAME).Distinct().ToList();
                foreach (var item in lstStates)
                {
                    lstArray.Add(lstDetails.Where(m => m.LOCATION_NAME == item).Select(m => m.ROAD_LEN).ToArray());
                }
                var data = lstDetails.GroupBy(m => m.LOCATION_NAME, m => new { m.ROAD_LEN }).ToList();

                return Json
                (
                    new
                    {
                        LOCATION_NAME = lstDetails.Select(m => m.LOCATION_NAME).Distinct().ToList(),
                        YEARS = lstDetails.Select(m => m.MAST_YEAR_TEXT).Distinct().ToList(),
                        ROAD_LEN = lstArray,
                        LSB_LEN = lstDetails.Select(m => m.LSB_LEN).ToList()
                    }
                );


            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="frmCollection"></param>
        /// <returns></returns>
        public ActionResult GetTabularTrendValues(FormCollection frmCollection)
        {
            try
            {
                string stateList = string.Empty;
                int tillYear = 0;
                int reportType = 0;
                int agency = 0;
                int collaboration = 0;
                if (!String.IsNullOrEmpty(Request.Params["STATE_CODES"]))
                {
                    stateList = Request.Params["STATE_CODES"];
                }

                if (!String.IsNullOrEmpty(Request.Params["Year"]))
                {
                    tillYear = Convert.ToInt32(Request.Params["Year"]);
                }

                if (!String.IsNullOrEmpty(Request.Params["ReportType"]))
                {
                    reportType = Convert.ToInt32(Request.Params["ReportType"]);
                }
                List<USP_TECH_TRENDS_CHART_PPT_Result> lstDetails = new List<USP_TECH_TRENDS_CHART_PPT_Result>();

                lstDetails = dashboardBAL.GetProposalTrendsDetailsBAL(stateList, tillYear, reportType, agency, collaboration);

                ViewBag.TrendDetails = lstDetails;

                return PartialView();
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// returns the data of habitations 
        /// </summary>
        /// <param name="frmCollection"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult HabitationDetailsMPR(FormCollection frmCollection)
        {
            try
            {
                dashboardBAL = new DashboardBAL();
                string stateList = string.Empty;
                int tillYear = 0;
                int agency = 0;
                int collaboration = 0;
                if (!String.IsNullOrEmpty(Request.Params["STATE_CODES"]))
                {
                    stateList = Request.Params["STATE_CODES"];
                }

                if (!String.IsNullOrEmpty(Request.Params["Year"]))
                {
                    tillYear = Convert.ToInt32(Request.Params["Year"]);
                }

                if (!String.IsNullOrEmpty(Request.Params["Agency"]))
                {
                    agency = Convert.ToInt32(Request.Params["Agency"]);
                }

                if (!String.IsNullOrEmpty(Request.Params["Collaboration"]))
                {
                    collaboration = Convert.ToInt32(Request.Params["Collaboration"]);
                }

                List<USP_PROP_HAB_PPT_LIST_Result> lstHabDetails = new List<USP_PROP_HAB_PPT_LIST_Result>();
                lstHabDetails = dashboardBAL.GetHabitationDetailsMPRBAL(stateList, tillYear, DateTime.Now.Month, agency, collaboration);
                ViewBag.HabDetails = lstHabDetails;
                return PartialView();
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// returns the data of maintenance for the last 4 years.
        /// </summary>
        /// <param name="frmCollection"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult MaintenanceFundStatus(FormCollection frmCollection)
        {
            try
            {
                dashboardBAL = new DashboardBAL();
                string stateList = string.Empty;
                int tillYear = 0;
                int agency = 0;
                int collaboration = 0;
                if (!String.IsNullOrEmpty(Request.Params["STATE_CODES"]))
                {
                    stateList = Request.Params["STATE_CODES"];
                }

                if (!String.IsNullOrEmpty(Request.Params["Year"]))
                {
                    tillYear = Convert.ToInt32(Request.Params["Year"]);
                }

                if (!String.IsNullOrEmpty(Request.Params["Agency"]))
                {
                    agency = Convert.ToInt32(Request.Params["Agency"]);
                }

                if (!String.IsNullOrEmpty(Request.Params["Collaboration"]))
                {
                    collaboration = Convert.ToInt32(Request.Params["Collaboration"]);
                }

                List<USP_PROP_MAINT_PPT_LIST_Result> lstMaintenanceDetails = new List<USP_PROP_MAINT_PPT_LIST_Result>();
                lstMaintenanceDetails = dashboardBAL.GetMaintenanceDetailsMPRBAL(stateList, tillYear, DateTime.Now.Month, agency, collaboration);
                ViewBag.HabDetails = lstMaintenanceDetails;
                string curYear = DateTime.Now.Year.ToString()[2] + "" + DateTime.Now.Year.ToString()[3];
                ViewBag.CurrentYear = Convert.ToInt32(curYear);
                return PartialView();
            }
            catch (Exception)
            {
                return null;
            }
        }



        #endregion

    }
}
