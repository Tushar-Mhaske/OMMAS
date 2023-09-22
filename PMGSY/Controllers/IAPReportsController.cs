#region HEADER
/*
* Project Id:

 * Project Name:OMMAS-II

 * File Name: ExistingRoadsController.cs

 * Author : Abhishek Kamble (changes done by Vikram Nandanwar)

 * Creation Date :30/May/2013

 * Desc : This class is used as controller  to perform Save,Edit,Update,Delete and listing of Existing Roads screens.  
 */
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using PMGSY.BAL.IAPReports;
using PMGSY.Models;
using PMGSY.Models.IAPReports;
using System.Data.Entity;
using PMGSY.Common;
using PMGSY.Extensions;
using PMGSY.DAL.IAPReports;
using System.Data.Entity.Validation;


namespace PMGSY.Controllers
{
    [RequiredAuthentication]
    [RequiredAuthorization]
    [Audit]
    public class IAPReportsController : Controller
    {
        private PMGSYEntities db;


        public IAPReportsController()
        {
            PMGSYSession.Current.ModuleName = "Existing Roads";
        }

        private IIAPReportsBAL objIAPBAL = new IAPReportsBAL();
        string message = String.Empty;
        #region Habitation
     
        public ActionResult IAPDistrictHabitationDetails()
        {

            CommonFunctions objCommonFunctions = new CommonFunctions();
            try
            {
                IAPReportsViewModel iapReport = new IAPReportsViewModel();
                iapReport.MAST_STATE_CODE = PMGSYSession.Current.StateCode;
             
                iapReport.Type = "H";
                iapReport.MONTH = DateTime.Now.Month;
                iapReport.YEAR = DateTime.Now.Year;
                // qmAtr.MONITORS = qualityBAL.PopulateMonitorsBAL(PMGSYSession.Current.StateCode, DateTime.Now.Month, DateTime.Now.Year, PMGSYSession.Current.DistrictCode, "I");
                iapReport.MONTHS_LIST = objCommonFunctions.PopulateMonths(true).ToList();
                iapReport.YEARS_LIST = objCommonFunctions.PopulateFinancialYear(true, false).ToList();
                   
                return View("IAPDistrictHabitationDetails", iapReport);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return null;
            }
        }

        [HttpPost]       
        public ActionResult IAPDistrictHabitationReportList(FormCollection formCollection)
        {

            objIAPBAL = new IAPReportsBAL();
            int totalRecords;
            int stateCode = Convert.ToInt32(formCollection["StateCode"]);
            int year = Convert.ToInt32(formCollection["YEAR"]);
            int month = Convert.ToInt32(formCollection["MONTH"]);

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
                    rows = objIAPBAL.IAPDistrictHabitationDetailsBAL(stateCode, month, year, Convert.ToInt32(formCollection["page"]) - 1, Convert.ToInt32(formCollection["rows"]),
                                                             formCollection["sidx"], formCollection["sord"], out totalRecords),
                    total = totalRecords <= Convert.ToInt32(formCollection["rows"]) ? 1 : (totalRecords % Convert.ToInt32(formCollection["rows"]) == 0 ? totalRecords / Convert.ToInt32(formCollection["rows"]) : (totalRecords / Convert.ToInt32(formCollection["rows"]) + 1)),
                    page = Convert.ToInt32(formCollection["page"]),
                    records = totalRecords
                };
                var jsonResult = Json(jsonData, JsonRequestBehavior.AllowGet);
                jsonResult.MaxJsonLength = int.MaxValue;
                return jsonResult;
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return Json(String.Empty);
            }
        }
        #endregion

        #region Physical Progress

      
        public ActionResult IAPDistrictPhysicalProgressDetails()
        {

            CommonFunctions objCommonFunctions = new CommonFunctions();
            try
            {
                IAPReportsViewModel iapReport = new IAPReportsViewModel();
                iapReport.MAST_STATE_CODE = PMGSYSession.Current.StateCode;
             
                iapReport.Type = "P";
                iapReport.MONTH = DateTime.Now.Month;


                iapReport.YEAR = DateTime.Now.Year;

                // qmAtr.MONITORS = qualityBAL.PopulateMonitorsBAL(PMGSYSession.Current.StateCode, DateTime.Now.Month, DateTime.Now.Year, PMGSYSession.Current.DistrictCode, "I");
                iapReport.MONTHS_LIST = objCommonFunctions.PopulateMonths(true);

                iapReport.YEARS_LIST = objCommonFunctions.PopulateYears(true);

                return View(iapReport);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return null;
            }
        }

        [HttpPost]       
        public ActionResult IAPDistrictPhysicalProgressList(FormCollection formCollection)
        {
            objIAPBAL = new IAPReportsBAL();
            int totalRecords;
            int stateCode = Convert.ToInt32(formCollection["StateCode"]);
            int year = Convert.ToInt32(formCollection["YEAR"]);
            int month = Convert.ToInt32(formCollection["MONTH"]);

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
                    rows = objIAPBAL.IAPDistrictPhysicalProgressDetailsBAL(stateCode, month, year, Convert.ToInt32(formCollection["page"]) - 1, Convert.ToInt32(formCollection["rows"]),
                                                             formCollection["sidx"], formCollection["sord"], out totalRecords),
                    total = totalRecords <= Convert.ToInt32(formCollection["rows"]) ? 1 : (totalRecords % Convert.ToInt32(formCollection["rows"]) == 0 ? totalRecords / Convert.ToInt32(formCollection["rows"]) : (totalRecords / Convert.ToInt32(formCollection["rows"]) + 1)),
                    page = Convert.ToInt32(formCollection["page"]),
                    records = totalRecords
                };
                var jsonResult = Json(jsonData, JsonRequestBehavior.AllowGet);
                jsonResult.MaxJsonLength = int.MaxValue;
                return jsonResult;
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return Json(String.Empty);
            }
        }
     
        #endregion

        #region Financial
       
      
        public ActionResult IAPDistrictFinancialProgressDetails()
        {

            CommonFunctions objCommonFunctions = new CommonFunctions();
            try
            {
                IAPReportsViewModel iapReport = new IAPReportsViewModel();
                iapReport.MAST_STATE_CODE = PMGSYSession.Current.StateCode;
             
                iapReport.Type = "F";
                iapReport.MONTH = DateTime.Now.Month;


                iapReport.YEAR = DateTime.Now.Year;

                // qmAtr.MONITORS = qualityBAL.PopulateMonitorsBAL(PMGSYSession.Current.StateCode, DateTime.Now.Month, DateTime.Now.Year, PMGSYSession.Current.DistrictCode, "I");
                iapReport.MONTHS_LIST = objCommonFunctions.PopulateMonths(true);
                iapReport.YEARS_LIST = objCommonFunctions.PopulateYears(true);

                return View(iapReport);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return null;
            }
        }

        [HttpPost]      
        public ActionResult IAPDistrictFinancialProgressList(FormCollection formCollection)
        {
            objIAPBAL = new IAPReportsBAL();
            int totalRecords;
            int stateCode = Convert.ToInt32(formCollection["StateCode"]);
            int year = Convert.ToInt32(formCollection["YEAR"]);
            int month = Convert.ToInt32(formCollection["MONTH"]);

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
                    rows = objIAPBAL.IAPDistrictFinancialProgressDetailsBAL(stateCode, month, year, Convert.ToInt32(formCollection["page"]) - 1, Convert.ToInt32(formCollection["rows"]),
                                                             formCollection["sidx"], formCollection["sord"], out totalRecords),
                    total = totalRecords <= Convert.ToInt32(formCollection["rows"]) ? 1 : (totalRecords % Convert.ToInt32(formCollection["rows"]) == 0 ? totalRecords / Convert.ToInt32(formCollection["rows"]) : (totalRecords / Convert.ToInt32(formCollection["rows"]) + 1)),
                    page = Convert.ToInt32(formCollection["page"]),
                    records = totalRecords
                };
                var jsonResult = Json(jsonData, JsonRequestBehavior.AllowGet);
                jsonResult.MaxJsonLength = int.MaxValue;
                return jsonResult;
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return Json(String.Empty);
            }
        }
     

        #endregion

        #region Expenditure        
        public ActionResult IAPDistrictExpenditureDetails()
        {

            CommonFunctions objCommonFunctions = new CommonFunctions();
            try
            {
                IAPReportsViewModel iapReport = new IAPReportsViewModel();
                iapReport.MAST_STATE_CODE = PMGSYSession.Current.StateCode;
                iapReport.Type = "E";
                iapReport.YEAR = DateTime.Now.Year;
                iapReport.YEARS_LIST = objCommonFunctions.PopulateFinancialYear(true, false).ToList();

                return View(iapReport);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return null;
            }
        }

        [HttpPost]       
        public ActionResult IAPDistrictExpenditureList(FormCollection formCollection)
        {
            objIAPBAL = new IAPReportsBAL();
            int totalRecords;
            int stateCode = Convert.ToInt32(formCollection["StateCode"]);
            int year = Convert.ToInt32(formCollection["YEAR"]);
           // int month = Convert.ToInt32(formCollection["MONTH"]);

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
                    rows = objIAPBAL.IAPDistrictExpenditureDetailsBAL(stateCode, year, Convert.ToInt32(formCollection["page"]) - 1, Convert.ToInt32(formCollection["rows"]),
                                                             formCollection["sidx"], formCollection["sord"], out totalRecords),
                    total = totalRecords <= Convert.ToInt32(formCollection["rows"]) ? 1 : (totalRecords % Convert.ToInt32(formCollection["rows"]) == 0 ? totalRecords / Convert.ToInt32(formCollection["rows"]) : (totalRecords / Convert.ToInt32(formCollection["rows"]) + 1)),
                    page = Convert.ToInt32(formCollection["page"]),
                    records = totalRecords
                };
                var jsonResult = Json(jsonData, JsonRequestBehavior.AllowGet);
                jsonResult.MaxJsonLength = int.MaxValue;
                return jsonResult;
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return Json(String.Empty);
            }
        }
        #endregion
    }


}
