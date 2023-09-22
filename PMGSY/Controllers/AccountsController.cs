using PMGSY.Models.Accounts;
using PMGSY.Models.Receipts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PMGSY.BAL.Accounts;
using PMGSY.Extensions;
using PMGSY.Common;
using PMGSY.Models.Common;
using PMGSY.Models;
using System.Configuration;
using System.IO;

namespace PMGSY.Controllers
{
    [RequiredAuthentication]
    //[RequiredAuthorization]
    public class AccountsController : Controller
    {
        IAccountsBAL objAccountsBAL = null;

          [Audit]
        public ActionResult Index()
        {
            return View();
        }

        //action to redirect to generic validation page in case of failure of the accounting validation
        [Audit]
        public ViewResult GenericAccountValidation(string id)
        {
            AccountValidationModel model = new AccountValidationModel();
            try
            {
                using (System.IO.StreamWriter sw = System.IO.File.AppendText(System.IO.Path.Combine("D:\\ErrorLog\\OMMASErrorLog_" + System.DateTime.Now.Date.ToString("dd_MM_yyyy") + ".txt", "")))
                {
                    sw.WriteLine("Date :" + DateTime.Now.ToString());
                    sw.WriteLine("Method : " + "Accounts.GenericAccountValidation Method test");
                    sw.WriteLine("---------------------------------------------------------------------------------------");
                    sw.Close();
                }
                if (!string.IsNullOrEmpty(id))
                {
                    model.BankDetailsEntered = Convert.ToBoolean(id.Split('$')[0]);
                    model.ChequeBookDetailsEntered = Convert.ToBoolean(id.Split('$')[1]);
                    model.OpeningBalanceFinalized = Convert.ToBoolean(id.Split('$')[2]);
                    model.AuthSign = Convert.ToBoolean(id.Split('$')[3]);
                    model.SrrdaOBEntered = Convert.ToBoolean(id.Split('$')[4]);
                }
                return View(model);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "Accounts.GenericAccountValidation()");
                return null;
            }
        }
         [Audit]
        public ActionResult AccountDashBoard(DashBoardModel model)
        {

            CommonFunctions common = new CommonFunctions();
            TransactionParams objparams = new TransactionParams();

            objparams.FUND_TYPE = PMGSYSession.Current.FundType;
            objparams.LVL_ID = PMGSYSession.Current.LevelId;
            objparams.ADMIN_ND_CODE = PMGSYSession.Current.AdminNdCode;
            objparams.DISTRICT_CODE = PMGSYSession.Current.DistrictCode;

            ViewBag.ActionToRedirect = TempData["roleDefaultPage"];
            ViewBag.LevelId = PMGSYSession.Current.LevelId;

            List<SelectListItem> monthList = common.PopulateMonths(DateTime.Now.Month);
            model.MONTH_LIST = monthList;

            List<SelectListItem> yearList = common.PopulateYears(DateTime.Now.Year);
            model.YEAR_LIST = yearList;

            List<SelectListItem> DPIUList = common.PopulateDPIU(objparams);
            DPIUList.Remove(DPIUList.Where(x => x.Value == "0").First());

            ViewBag.EncryptedProgram = URLEncrypt.EncryptParameters(new string[] { "P" });
            ViewBag.EncryptedAdmin = URLEncrypt.EncryptParameters(new string[] { "A" });
            ViewBag.EncryptedMainten = URLEncrypt.EncryptParameters(new string[] { "M" });

            if (PMGSYSession.Current.LevelId == 4)
            {

                // DPIUList.Insert(0, (new SelectListItem { Text = "All Departments", Value = "0", Selected = true }));
            }

            model.DPIU_LIST = DPIUList;

             //Added By Abhishek kamble 28-Feb-2014 to Populate SRRDA
            IAccountsBAL objAccountBAL = new AccountsBAL();

            if (PMGSYSession.Current.LevelId == 6 )
            {   
                List<SelectListItem> lstSRRDA = new List<SelectListItem>();
                lstSRRDA = objAccountBAL.PopulateSRRDA(0);
                model.SRRDA_LIST = lstSRRDA;
            }
            else if(PMGSYSession.Current.LevelId == 4)
            {
                List<SelectListItem> lstSRRDA = new List<SelectListItem>();
                lstSRRDA = objAccountBAL.PopulateSRRDA(PMGSYSession.Current.AdminNdCode);
                model.SRRDA_LIST = lstSRRDA;                
            }

            return View(model);
        }

        [HttpPost]
        [Audit]      
        public ActionResult GetPFAuthorizationList(FormCollection frmCollect)
        {
            //Adde By Abhishek kamble 30-Apr-2014 start
            using (CommonFunctions commonFunction = new CommonFunctions())
            {
                if (!commonFunction.ValidateGridParameters(new GridParams(Convert.ToInt32(frmCollect["page"]), Convert.ToInt32(frmCollect["rows"]), frmCollect["sidx"], frmCollect["sord"], Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                {
                    return null;
                }
            }
            //Adde By Abhishek kamble 30-Apr-2014 end

            objAccountsBAL = new AccountsBAL();
            AccountsFilterModel objFilter = new AccountsFilterModel();
            long totalRecords;
            objFilter.page = Convert.ToInt32(frmCollect["page"]) - 1;
            objFilter.rows = Convert.ToInt32(frmCollect["rows"]);
            objFilter.sidx = frmCollect["sidx"].ToString();
            objFilter.sord = frmCollect["sord"].ToString();
            objFilter.AdminNdCode = PMGSYSession.Current.AdminNdCode;
            objFilter.FundType = PMGSYSession.Current.FundType==null?PMGSYSession.Current.FundType:PMGSYSession.Current.FundType.Trim(); //"P"; //changes by koustubh nakate on 22/07/2013 for specific fund type
            
            objFilter.LevelId = PMGSYSession.Current.LevelId;

            var jsonData = new
            {
                rows = objAccountsBAL.GetPFAuthorizationList(objFilter, out totalRecords),
                total = totalRecords <= objFilter.rows ? 1 : totalRecords / objFilter.rows + 1,
                page = objFilter.page + 1,
                records = totalRecords
            };
            return Json(jsonData);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <param name="sidx"></param>
        /// <param name="sord"></param>
        [HttpPost]
        [Audit]
      
        public ActionResult GetAssetliabilityList(int? page, int? rows, string sidx, string sord)
        {
            //Adde By Abhishek kamble 30-Apr-2014 start  
            using (CommonFunctions commonFunction = new CommonFunctions())
            {
                if (!commonFunction.ValidateGridParameters(new GridParams(page, rows, sidx, sord, Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                {
                    return null;
                }
            }
            //Adde By Abhishek kamble 30-Apr-2014 end

            objAccountsBAL = new AccountsBAL();
            AccountsFilterModel objFilter = new AccountsFilterModel();
            long totalRecords;
            objFilter.page = Convert.ToInt32(page) - 1;
            objFilter.rows = Convert.ToInt32(rows);
            objFilter.sidx = sidx.ToString();
            objFilter.sord = sord.ToString();
            //objFilter.AdminNdCode = PMGSYSession.Current.AdminNdCode;
            //AdminNdCode changes done by Abhishek kamble 28-Feb-2014
            if (PMGSYSession.Current.LevelId == 6)
            {
                objFilter.AdminNdCode = Convert.ToInt32(Request.Params["adminNdCode"]);
            }
            else
            {
                objFilter.AdminNdCode = PMGSYSession.Current.AdminNdCode;
            }
            objFilter.AssetOrliability = Request.Params["assetOrliability"];
            objFilter.FundType = Request.Params["fundType"];
            //objFilter.LevelId = Convert.ToInt16(Request.Params["level"]);
            //LevelId changes done by Abhishek kamble 3-March-2014            
            if (Request.Params["ownLower"] == "L" && (PMGSYSession.Current.LevelId == 6) || (PMGSYSession.Current.LevelId == 5))
            {
                objFilter.LevelId = 5;//DPIU
            }
            else {
                objFilter.LevelId = Convert.ToInt16(Request.Params["level"]);
            }
            objFilter.month = Convert.ToInt16(Request.Params["month"]);
            objFilter.year = Convert.ToInt16(Request.Params["year"]);
            objFilter.lowercode = Convert.ToInt16(Request.Params["lowercode"]);
            objFilter.ownLower = Request.Params["ownLower"];

            objFilter.rptId = Convert.ToInt16(Request.Params["rptId"]);
            objFilter.selectioncode = (Request.Params["selectioncode"]);
            var jsonData = new
            {
                rows = objAccountsBAL.GetAssetliabilityList(objFilter, out totalRecords),
                total = totalRecords <= objFilter.rows ? 1 : totalRecords / objFilter.rows + 1,
                page = objFilter.page + 1,
                records = totalRecords
            };
            return Json(jsonData);

        }


        /// <summary>
        /// Action method to return the authorization received list
        /// </summary>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <param name="sidx"></param>
        /// <param name="sord"></param>
        /// <returns></returns>
        [HttpPost]
        [Audit]
      
        public ActionResult GetAuthorizationReceivedList(int? page, int? rows, string sidx, string sord)
        {
            //Adde By Abhishek kamble 30-Apr-2014 start  
            using (CommonFunctions commonFunction = new CommonFunctions())
            {
                if (!commonFunction.ValidateGridParameters(new GridParams(page, rows, sidx, sord, Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                {
                    return null;
                }
            }
            //Adde By Abhishek kamble 30-Apr-2014 end
            objAccountsBAL = new AccountsBAL();
            AccountsFilterModel objFilter = new AccountsFilterModel();
            long totalRecords;
            objFilter.page = Convert.ToInt32(page) - 1;
            objFilter.rows = Convert.ToInt32(rows);
            objFilter.sidx = sidx.ToString();
            objFilter.sord = sord.ToString();

            //AdminNdCode changes done by Abhishek kamble 28-Feb-2014
            if (PMGSYSession.Current.LevelId == 6)
            {
                objFilter.AdminNdCode = Convert.ToInt32(Request.Params["adminNdCode"]);
            }
            else {
                objFilter.AdminNdCode = PMGSYSession.Current.AdminNdCode;
            }
            objFilter.FundType = Request.Params["fundType"];
            objFilter.month = Convert.ToInt16(Request.Params["month"]);
            objFilter.year = Convert.ToInt16(Request.Params["year"]);
            //objFilter.LevelId = Convert.ToInt16(Request.Params["level"]);
            //LevelId changes done by Abhishek kamble 3-March-2014            
            if (Request.Params["ownLower"] == "L" && (PMGSYSession.Current.LevelId == 6) || (PMGSYSession.Current.LevelId == 5))
            {
                objFilter.LevelId = 5;//DPIU
            }
            else
            {
                objFilter.LevelId = Convert.ToInt16(Request.Params["level"]);
            }
            objFilter.lowercode = Convert.ToInt16(Request.Params["lowercode"]);
            objFilter.ownLower = Request.Params["ownLower"];

            var jsonData = new
            {
                rows = objAccountsBAL.GetAuthorizationReceivedList(objFilter, out totalRecords),
                total = totalRecords <= objFilter.rows ? 1 : totalRecords / objFilter.rows + 1,
                page = objFilter.page + 1,
                records = totalRecords
            };
            return Json(jsonData);

        }

        /// <summary>
        /// action to get lettest transaction details
        /// </summary>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <param name="sidx"></param>
        /// <param name="sord"></param>
        /// <returns></returns>
        [HttpPost]
        [Audit]
      
        public ActionResult GetLettestTransactionsList(int? page, int? rows, string sidx, string sord)
        {
            //Adde By Abhishek kamble 30-Apr-2014 start  
            using (CommonFunctions commonFunction = new CommonFunctions())
            {
                if (!commonFunction.ValidateGridParameters(new GridParams(page, rows, sidx, sord, Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                {
                    return null;
                }
            }
            //Adde By Abhishek kamble 30-Apr-2014 end

            objAccountsBAL = new AccountsBAL();
            AccountsFilterModel objFilter = new AccountsFilterModel();
            long totalRecords;
            objFilter.page = Convert.ToInt32(page) - 1;
            objFilter.rows = Convert.ToInt32(rows);
            objFilter.sidx = sidx.ToString();
            objFilter.sord = sord.ToString();
            objFilter.AdminNdCode = PMGSYSession.Current.AdminNdCode;
            objFilter.FundType = PMGSYSession.Current.FundType;
            objFilter.LevelId = PMGSYSession.Current.LevelId;
           

            var jsonData = new
            {
                rows = objAccountsBAL.GetLettestTransactionsList(objFilter, out totalRecords),
                total = totalRecords <= objFilter.rows ? 1 : totalRecords / objFilter.rows + 1,
                page = objFilter.page + 1,
                records = totalRecords
            };
            return Json(jsonData);
        
        }


        /// <summary>
        /// Action to get the list of summary for dashbord
        /// </summary>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <param name="sidx"></param>
        /// <param name="sord"></param>
        /// <returns></returns>
        [HttpPost]
        [Audit]
       
        public ActionResult GetSummaryList(int? page, int? rows, string sidx, string sord)
        {
            //Adde By Abhishek kamble 30-Apr-2014 start  
            using (CommonFunctions commonFunction = new CommonFunctions())
            {
                if (!commonFunction.ValidateGridParameters(new GridParams(page, rows, sidx, sord, Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                {
                    return null;
                }
            }
            //Adde By Abhishek kamble 30-Apr-2014 end
            objAccountsBAL = new AccountsBAL();
            AccountsFilterModel objFilter = new AccountsFilterModel();
            long totalRecords;
            objFilter.page = Convert.ToInt32(page) - 1;
            objFilter.rows = Convert.ToInt32(rows);
            objFilter.sidx = sidx.ToString();
            objFilter.sord = sord.ToString();

            //AdminNdCode changes done by Abhishek kamble 28-Feb-2014
            //objFilter.AdminNdCode = PMGSYSession.Current.AdminNdCode;
            if (PMGSYSession.Current.LevelId == 6)
            {
                objFilter.AdminNdCode = Convert.ToInt32(Request.Params["adminNdCode"]);
            }
            else
            {
                objFilter.AdminNdCode = PMGSYSession.Current.AdminNdCode;
            }

            objFilter.FundType = Request.Params["fundType"];
            objFilter.month = Convert.ToInt16(Request.Params["month"]);
            objFilter.year = Convert.ToInt16(Request.Params["year"]);
            //objFilter.LevelId = Convert.ToInt16(Request.Params["level"]);
            //LevelId changes done by Abhishek kamble 3-March-2014            
            if (Request.Params["ownLower"] == "L" && (PMGSYSession.Current.LevelId == 6) || (PMGSYSession.Current.LevelId == 5))
            {
                objFilter.LevelId = 5;//DPIU
            }
            else
            {
                objFilter.LevelId = Convert.ToInt16(Request.Params["level"]);
            }
            objFilter.lowercode = Convert.ToInt16(Request.Params["lowercode"]);
            objFilter.ownLower = Request.Params["ownLower"];

            var jsonData = new
            {
                rows = objAccountsBAL.GetSummaryList(objFilter, out totalRecords),
                total = totalRecords <= objFilter.rows ? 1 : totalRecords / objFilter.rows + 1,
                page = objFilter.page + 1,
                records = totalRecords
            };
            return Json(jsonData);

        }




        /// <summary>
        /// function to get the chart data
        /// </summary>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <param name="sidx"></param>
        /// <param name="sord"></param>
        /// <returns></returns>
          [Audit]
        public JsonResult GetAssetliabilityChart()
        {
            objAccountsBAL = new AccountsBAL();
            AccountsFilterModel objFilter = new AccountsFilterModel();


            //AdminNdCode changes done by Abhishek kamble 28-Feb-2014
            //objFilter.AdminNdCode = PMGSYSession.Current.AdminNdCode;            
            if (PMGSYSession.Current.LevelId == 6)
            {
                objFilter.AdminNdCode = Convert.ToInt32(Request.Params["adminNdCode"]);
            }
            else
            {
                objFilter.AdminNdCode = PMGSYSession.Current.AdminNdCode;
            }
            
            objFilter.AssetOrliability = Request.Params["assetOrliability"];
            objFilter.FundType = Request.Params["fundType"];
            //objFilter.LevelId = Convert.ToInt16(Request.Params["level"]);
            //LevelId changes done by Abhishek kamble 3-March-2014            
            if (Request.Params["ownLower"] == "L" && (PMGSYSession.Current.LevelId == 6) || (PMGSYSession.Current.LevelId == 5))
            {
                objFilter.LevelId = 5;//DPIU
            }
            else
            {
                objFilter.LevelId = Convert.ToInt16(Request.Params["level"]);
            }
            objFilter.month = Convert.ToInt16(Request.Params["month"]);
            objFilter.year = Convert.ToInt16(Request.Params["year"]);
            objFilter.lowercode = Convert.ToInt16(Request.Params["lowercode"]);
            objFilter.ownLower = Request.Params["ownLower"];
            objFilter.rptId = Convert.ToInt16(Request.Params["rptId"]);
            objFilter.selectioncode = (Request.Params["selectioncode"]);

            List<SP_ACC_DB_ASSET_LIABILITIES_MAJOR_DETAILS_Result> List = objAccountsBAL.GetAssetliabilityChart(objFilter);


            List<charModel> lstChart = new List<charModel>();

            decimal totalAmt = 0;

            totalAmt = List.Sum(x => x.amount.HasValue ? x.amount.Value : 0);

            foreach (var p in List)
            {
                charModel chart = new charModel();
                chart.x = p.ITEM_HEADING;
                chart.z = (p.amount).ToString();
                chart.y = p.amount != 0 && totalAmt != 0 && p.amount.HasValue ? Math.Round((p.amount.Value * 100 / totalAmt), 2).ToString() : "0";
                chart.id = Convert.ToInt32(p.ITEM_ID);
                chart.fundType = objFilter.FundType;
                chart.AssetOrLia = objFilter.AssetOrliability;
                // chart.headCode = p.ITEM_HEADING.Split('$')[0];
                lstChart.Add(chart);
            }
            return new JsonResult
            {
                Data = lstChart
            };


        }


        /// <summary>
        /// Controller Method to get the asset liability details based on master head id
        /// </summary>
          [Audit]
        public JsonResult GetAssetliabilityDetailsChart()
        {
            objAccountsBAL = new AccountsBAL();
            AccountsFilterModel objFilter = new AccountsFilterModel();

            objFilter.AdminNdCode = PMGSYSession.Current.AdminNdCode;
            objFilter.AssetOrliability = Request.Params["assetOrliability"];
            objFilter.FundType = Request.Params["fundType"];
            objFilter.LevelId = Convert.ToInt16(Request.Params["level"]);
            objFilter.month = Convert.ToInt16(Request.Params["month"]);
            objFilter.year = Convert.ToInt16(Request.Params["year"]);
            objFilter.lowercode = Convert.ToInt16(Request.Params["lowercode"]);
            objFilter.ownLower = Request.Params["ownLower"];
            objFilter.rptId = Convert.ToInt16(Request.Params["rptId"]);
            objFilter.selectioncode = (Request.Params["selectioncode"]);
            objFilter.masterHead = Convert.ToInt16(Request.Params["mainHeadId"]);
            List<SP_ACC_DB_ASSET_LIABILITIES_MINOR_DETAILS_Result> List = objAccountsBAL.GetAssetliabilityDetailsChart(objFilter);

            List<charModel> lstChart = new List<charModel>();



            //  decimal totalAmt = 0; totalAmt = List.Sum(x => x.amount);

            foreach (var p in List)
            {
                charModel chart = new charModel();
                chart.x = p.HEAD_NAME;
                chart.y = (p.AMOUNT).ToString();
                chart.id = p.ITEM_ID;
                chart.fundType = objFilter.FundType;
                chart.AssetOrLia = objFilter.AssetOrliability;
                lstChart.Add(chart);
            }
            return new JsonResult
            {
                Data = lstChart
            };


        }


        /// <summary>
        /// this action used to get fundtypeselection screen
        /// </summary>
        /// <returns>FundTypeSelection view</returns>
          [Audit]
        public ActionResult FundTypeSelection()
        {

            ViewBag.EncryptedProgramme = URLEncrypt.EncryptParameters(new string[] { "P" });
            ViewBag.EncryptedAdministrative = URLEncrypt.EncryptParameters(new string[] { "A" });
            ViewBag.EncryptedMaintenance = URLEncrypt.EncryptParameters(new string[] { "M" });

            return View("FundTypeSelection"); 
        }



        /// <summary>
        /// this action is used to get Latest DPIU transaction Details added by koustubh nakate on 20/08/2013
        /// </summary>
        /// <returns>Latest DPIU transaction Details List</returns>


        [HttpPost]
        [Audit]
     
        public ActionResult GetLatestDPIUTransactionsDetailsList(int? page, int? rows, string sidx, string sord)
        {

            long totalRecords;
          
            try
            {
                objAccountsBAL = new AccountsBAL();
                using (CommonFunctions commonFunction = new CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new GridParams(page, rows, sidx, sord, Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                    {
                        return null;
                    }
                }

              
                var jsonData = new
                {
                    rows = objAccountsBAL.GetLatestDPIUTransactionsDetailsListBAL(Convert.ToInt32(page) - 1, Convert.ToInt32(rows), sidx, sord, out totalRecords),
                    total = totalRecords <= Convert.ToInt32(rows) ? 1 : (totalRecords % Convert.ToInt32(rows) == 0 ? totalRecords / Convert.ToInt32(rows) : totalRecords / Convert.ToInt32(rows) + 1),
                    page = Convert.ToInt32(page),
                    records = totalRecords
                };
                return Json(jsonData);

            }
            catch(Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);              
                return null;
            }

        }



        /// <summary>
        /// this action is used to get DPIU autherization issued Details added by koustubh nakate on 23/08/2013
        /// </summary>
        /// <returns>Autherization Issued Details List</returns>


        [HttpPost]
        [Audit]      
        public ActionResult GetDPIUAutherizationIssuedDetailsList(int? page, int? rows, string sidx, string sord)
        {

            long totalRecords;

            try
            {
                objAccountsBAL = new AccountsBAL();
                using (CommonFunctions commonFunction = new CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new GridParams(page, rows, sidx, sord, Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                    {
                        return null;
                    }
                }


                var jsonData = new
                {
                    rows = objAccountsBAL.GetDPIUAutherizationIssuedDetailsListBAL(Convert.ToInt32(page) - 1, Convert.ToInt32(rows), sidx, sord, out totalRecords),
                    total = totalRecords <= Convert.ToInt32(rows) ? 1 : (totalRecords % Convert.ToInt32(rows) == 0 ? totalRecords / Convert.ToInt32(rows) : totalRecords / Convert.ToInt32(rows) + 1),
                    page = Convert.ToInt32(page),
                    records = totalRecords
                };
                return Json(jsonData);

            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);              
                 return null;
            }

        }

        #region Accounting ATR  [Created By Pradip Patil(17-05-2017))]

        [HttpGet]
        public ViewResult AccountingATRLayout()
        {
            CommonFunctions commomObj = new CommonFunctions();
            AccountingATRModel model = new AccountingATRModel();
            try
            {
                model.lstStates = commomObj.PopulateStates();
                model.lstAgency = new List<SelectListItem>() { new SelectListItem { Text = "Select Agency", Value = "0", Selected = true } };
                model.lstYear = commomObj.PopulateFinancialYear().ToList<SelectListItem>();
                return View(model);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "AccountingATRLayout()");
                return null;

            }
        }


        [HttpPost]
        public JsonResult GetAgencies(String State)
        {
            try
            {
                var Agencies = new CommonFunctions().PopulateAgencies(Convert.ToInt32(State));
                Agencies.Find(x => x.Value == "-1").Value = "0";
                return Json(Agencies);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "GetAgencies()");
                return null;
            }
        }
        [HttpGet]
        public String GetBalanceSheet(AccountingATRModel model)
        {
            PMGSYEntities dbContext = null;
            try
            {
                Microsoft.Reporting.WebForms.ReportViewer rview = new Microsoft.Reporting.WebForms.ReportViewer();
                rview.ServerReport.ReportServerUrl = new Uri(System.Web.Configuration.WebConfigurationManager.AppSettings["MvcReportViewer.ReportServerUrl"]);
                System.Collections.Generic.List<Microsoft.Reporting.WebForms.ReportParameter> paramList = new System.Collections.Generic.List<Microsoft.Reporting.WebForms.ReportParameter>();
                dbContext = new PMGSYEntities();
                Int32 AdminNdCode = dbContext.ADMIN_DEPARTMENT.Where(s => s.MAST_AGENCY_CODE == model.Agency && s.MAST_STATE_CODE == model.StateCode && s.MAST_ND_TYPE == "S").Select(s => s.ADMIN_ND_CODE).FirstOrDefault();

                paramList.Add(new Microsoft.Reporting.WebForms.ReportParameter("PrmSRRDANDCode", AdminNdCode.ToString()));
                paramList.Add(new Microsoft.Reporting.WebForms.ReportParameter("PINTRptID", "9"));
                paramList.Add(new Microsoft.Reporting.WebForms.ReportParameter("PINTAdminNDCode", AdminNdCode.ToString()));
                paramList.Add(new Microsoft.Reporting.WebForms.ReportParameter("PINTAccMonth", "3"));
                paramList.Add(new Microsoft.Reporting.WebForms.ReportParameter("PINTAccYear", model.Year.ToString()));
                paramList.Add(new Microsoft.Reporting.WebForms.ReportParameter("PINTisAllPIU", "0"));
                paramList.Add(new Microsoft.Reporting.WebForms.ReportParameter("PCHARFUNDTYPE", PMGSYSession.Current.FundType));
                paramList.Add(new Microsoft.Reporting.WebForms.ReportParameter("PCHARRType", "S"));
                paramList.Add(new Microsoft.Reporting.WebForms.ReportParameter("PINTLVL", "4"));
                paramList.Add(new Microsoft.Reporting.WebForms.ReportParameter("LocalizationValue", "en"));

                Microsoft.Reporting.WebForms.IReportServerCredentials irsc = new CustomReportCredentials(System.Web.Configuration.WebConfigurationManager.AppSettings["MvcReportViewer.Username"], System.Web.Configuration.WebConfigurationManager.AppSettings["MvcReportViewer.Password"]);
                rview.ServerReport.ReportServerCredentials = irsc;
                rview.ServerReport.ReportPath = "/PMGSYCitizen/BalanceSheet";
                rview.ServerReport.SetParameters(paramList);
                rview.AsyncRendering = true;
                string mimeType, encoding, extension, deviceInfo;
                string[] streamids;
                Microsoft.Reporting.WebForms.Warning[] warnings;
                string format = "PDF"; //Desired format goes here (PDF, Excel, or Image)
                deviceInfo = "<DeviceInfo>" + "<SimplePageHeaders>True</SimplePageHeaders>" + "</DeviceInfo>";
                byte[] bytes = rview.ServerReport.Render(format, deviceInfo, out mimeType, out encoding, out extension, out streamids, out warnings);

                //String DirectoryPath = model.PMGSYScheme == 1 ? ConfigurationManager.AppSettings["DROP_ORDER_PDF_PMGSYI"].ToString() : ConfigurationManager.AppSettings["DROP_ORDER_PDF_PMGSYII"].ToString();
                //if (!Directory.Exists(DirectoryPath))
                //    Directory.CreateDirectory(DirectoryPath);
                //  System.IO.File.WriteAllBytes(filePath, bytes);
                //  return true;
                var cd = new System.Net.Mime.ContentDisposition
                {
                    Inline = true
                };
                Response.AppendHeader("Content-Disposition", cd.ToString());
                // return File(bytes, System.Net.Mime.MediaTypeNames.Application.Pdf);
                return Convert.ToBase64String(bytes);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "GetBalanceSheet()");
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

        [HttpGet]
        public ViewResult ObserVationView()
        {
            ObservationModel model = new ObservationModel();
            model.NonConforfance = "N";
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult SaveObservationDetails(ObservationModel model)
        {
            Int32 state = 0; Int32 agency = 0; Int32 year = 0;
            if (!(PMGSYSession.Current.RoleCode == 2))
            {
                state = Convert.ToInt32(Request.Params["state"].ToString());
                agency = Convert.ToInt32(Request.Params["agency"].ToString());
                year = Convert.ToInt32(Request.Params["year"].ToString());
            }
            String message = "Error occured while processing your request";
            Boolean status = false;
            try
            {
                objAccountsBAL = new AccountsBAL();

                status = objAccountsBAL.SaveObservationDetailsBAL(model, state, agency, year, out message);
                if (status)
                    return Json(new { success = status, message = message });
                return Json(new { success = status, message = message });
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "SaveObservationDetails()"); ;
                return Json(new { success = status, message = message });
            }

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult GetObservationList(int? page, int? rows, String sidx, String sord)
        {
            objAccountsBAL = new AccountsBAL();
            long totalrecords;
            try
            {
                var JsonData = new
                {
                    rows = objAccountsBAL.GetObservationListBAL(page - 1, rows, sidx, sord, out totalrecords),
                    total = totalrecords <= Convert.ToInt32(rows) ? 1 : (totalrecords % Convert.ToInt32(rows) == 0 ? totalrecords / Convert.ToInt32(rows) : totalrecords / Convert.ToInt32(rows) + 1),
                    page = Convert.ToInt32(page),
                    records = totalrecords,
                };
                return Json(JsonData);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "GetObservationList()");
                return null;
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult GetDetailObservationList(String id, int? page, int? rows, String sidx, String sord)
        {
            objAccountsBAL = new AccountsBAL();

            long totalrecords;
            try
            {
                var JsonData = new
                {
                    rows = objAccountsBAL.GetDetailObservationListBAL(page - 1, rows, sidx, sord, out totalrecords, Convert.ToInt32(id)),
                    total = totalrecords <= Convert.ToInt32(rows) ? 1 : (totalrecords % Convert.ToInt32(rows) == 0 ? totalrecords / Convert.ToInt32(rows) : totalrecords / Convert.ToInt32(rows) + 1),
                    page = Convert.ToInt32(page),
                    records = totalrecords,
                    IsSRRADA = PMGSYSession.Current.RoleCode == 2 ? "Y" : "N",
                };
                return Json(JsonData);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "GetDetailObservationList()");
                return null;
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult GetObservationReplyList(String id, int? page, int? rows, String sidx, String sord)
        {
            objAccountsBAL = new AccountsBAL();

            long totalrecords;
            try
            {
                var JsonData = new
                {
                    rows = objAccountsBAL.GetObservationReplyListBAL(page - 1, rows, sidx, sord, out totalrecords, Convert.ToInt32(id)),
                    total = totalrecords <= Convert.ToInt32(rows) ? 1 : (totalrecords % Convert.ToInt32(rows) == 0 ? totalrecords / Convert.ToInt32(rows) : totalrecords / Convert.ToInt32(rows) + 1),
                    page = Convert.ToInt32(page),
                    records = totalrecords,
                    IsSRRADA = PMGSYSession.Current.RoleCode == 2 ? "Y" : "N",
                };
                return Json(JsonData);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "GetDetailObservationList()");
                return null;
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult UploadATRFile(HttpPostedFileBase FileATR, String MasterObId)
        {
            objAccountsBAL = new AccountsBAL();
            try
            {
                Boolean Status = false;
                CommonFunctions commonFunction = new CommonFunctions();
                int masterObervId = 0;
                //String [] encparam = MasterObId.Split('/');
                //Dictionary<String, String> DecryptedParams = URLEncrypt.DecryptParameters1(encparam);

                String Temppath = ConfigurationManager.AppSettings["ATR_UPLOAD_TEMP"].ToString();
                if (!Directory.Exists(Temppath))
                    Directory.CreateDirectory(Temppath);  //if Directory not created creat it;


                if (Request.Files.Count == 0)
                {
                    return Json(new { success = false, file = false, message = "No file selected. Please select file" });
                }
                else
                {
                    HttpPostedFileBase postedATRFile = FileATR;
                    int maxSize = Convert.ToInt32(ConfigurationManager.AppSettings["ATR_FILE_SIZE"]);// 1024 * 1024 * 4;
                    if (!commonFunction.ValidateIsPdf(Temppath, Request))
                    {
                        return Json(new { success = false, file = false, message = "Invalid file. Please upload only pdf files." });
                    }
                    else if (!(postedATRFile.ContentType == "application/pdf"))
                    {
                        return Json(new { success = false, file = false, message = "Invalid file. Please upload only pdf files." });
                    }
                    else if (postedATRFile.FileName.Substring(postedATRFile.FileName.LastIndexOf('.') + 1).Trim().ToLower() != "pdf")
                    {
                        return Json(new { success = false, file = false, message = "Invalid file. Please upload only pdf files." });
                    }

                    if (postedATRFile.ContentLength > maxSize)
                    {
                        return Json(new { success = false, file = false, message = "File size should be less than or equal to 4 MB." });
                    }

                    if (MasterObId != null)
                    {
                        String MastId = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(MasterObId)).Split(':')[3];
                        masterObervId = Convert.ToInt32(MastId);
                        Status = objAccountsBAL.UploadATRFileBAL(postedATRFile, masterObervId);
                    }

                    if (Status)
                        return Json(new { success = Status, message = "File uploaded successfully." });
                    return Json(new { success = Status, message = "Error occured while processing your request." });
                }
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "UploadATRFile()");
                return Json(new { message = "Error occured while processing your request", success = false });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult GetATRFileList(String id, int? page, int? rows, String sidx, String sord)
        {
            objAccountsBAL = new AccountsBAL();
            long totalrecords;
            String filterValues;
            try
            {
                var JsonData = new
                {
                    rows = objAccountsBAL.GetATRFileListBAL(page - 1, rows, sidx, sord, out totalrecords, id, out filterValues),
                    total = totalrecords <= Convert.ToInt32(rows) ? 1 : (totalrecords % Convert.ToInt32(rows) == 0 ? totalrecords / Convert.ToInt32(rows) : totalrecords / Convert.ToInt32(rows) + 1),
                    page = Convert.ToInt32(page),
                    records = totalrecords,
                    FilterValues = filterValues,  //not in use temp
                    IsSRRADA = PMGSYSession.Current.RoleCode == 2 ? "Y" : "N",
                };
                return Json(JsonData);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "GetDetailObservationList()");
                return null;
            }
        }

        [HttpGet]
        public FilePathResult GetATRFile(string id)
        {
            try
            {
                String FileName = String.Empty;
                string[] encParam = id.Split('/');
                Dictionary<String, String> decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { encParam[0], encParam[1], encParam[2] });
                if (decryptedParameters.Count > 0)
                {
                    FileName = decryptedParameters["ATRFile"];
                }
                String Path = ConfigurationManager.AppSettings["ATR_UPLOAD_MAIN"].ToString();

                var cd = new System.Net.Mime.ContentDisposition { FileName = FileName, Inline = false };

                Response.AppendHeader("Content-Disposition", cd.ToString());

                return File(Path + FileName, System.Net.Mime.MediaTypeNames.Application.Pdf);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "GetBankGuarantee()");
                return null;
            }

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult DeleteObservation(String urlparameter)
        {
            objAccountsBAL = new AccountsBAL();
            try
            {
                String message = "Error Occured while processing your request";
                Boolean Status = false;
                int masterObjId = 0;
                int observationId = 0;
                Dictionary<String, String> decryptedParam = URLEncrypt.DecryptParameters1(urlparameter.Split('/'));
                if (decryptedParam.Count > 0)
                {
                    observationId = Convert.ToInt32(decryptedParam["ObservationId"].ToString());
                    masterObjId = Convert.ToInt32(decryptedParam["masterObId"].ToString());
                    Status = objAccountsBAL.DeleteObservationBAL(observationId, masterObjId, out message);
                }
                return Json(new { success = Status, message = message }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "DeleteObservation()");
                return Json(new { success = false, message = "Error occured while processing your request." });
            }
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult DeleteATRFIle(String parameter, String hash, String key)
        {
            objAccountsBAL = new AccountsBAL();
            try
            {
                String message = "Error Occured while processing your request";
                Boolean Status = false;
                String FileName;
                int UploadId = 0;
                Dictionary<String, String> decryptedParam = URLEncrypt.DecryptParameters1(new String[] { parameter, hash, key });
                if (decryptedParam.Count > 0)
                {
                    FileName = decryptedParam["FileName"].ToString();
                    UploadId = Convert.ToInt32(decryptedParam["UploadId"].ToString());
                    Status = objAccountsBAL.DeleteATRFIleBAL(FileName, UploadId, out message);
                }
                return Json(new { success = Status, message = message }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "DeleteATRFIle()");
                return Json(new { success = false, message = "Error occured while processing your request." });
            }
        }

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public JsonResult DeleteChildObservation(String urlparameter)
        //{
        //    objAccountsBAL = new AccountsBAL();
        //    try
        //    {
        //        String message = "Error Occured while processing your request";
        //        Boolean Status = false;
        //        int masterObjId = 0;
        //        int observationId = 0;
        //        Dictionary<String, String> decryptedParam = URLEncrypt.DecryptParameters1(urlparameter.Split('/'));
        //        if (decryptedParam.Count > 0)
        //        {
        //            observationId = Convert.ToInt32(decryptedParam["ObservationId"].ToString());
        //            masterObjId = Convert.ToInt32(decryptedParam["masterObId"].ToString());
        //            Status = objAccountsBAL.DeleteChildObservationBAL(observationId, masterObjId, out message);
        //        }
        //        return Json(new { success = Status, message = message }, JsonRequestBehavior.AllowGet);
        //    }
        //    catch (Exception ex)
        //    {
        //        ErrorLog.LogError(ex, "DeleteChildObservation()");
        //        return Json(new { success = false, message = "Error occured while processing your request." });
        //    }


        //}
        #endregion    

    }
}
