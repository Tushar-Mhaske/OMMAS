/*-----------------------------------------------------------------------------------
 * Project Name: OMMAS-II
 * File Name: AccountReportController.js
 * Created By : Ashish Markande
 * Creation Date: 24/07/2013
 * Purpose: For displaying Search form and details of Reports.
 *-----------------------------------------------------------------------------------
 * */

using PMGSY.DAL.AccountsReports;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PMGSY.Models.Report;
using PMGSY.Extensions;
using PMGSY.Common;
using PMGSY.Models.AccountReports;
using PMGSY.BAL.AccountReports;
using PMGSY.Models.Common;
using PMGSY.DAL.Payment;
using PMGSY.Models.AccountsReports;

using PMGSY.Models.Report.Account;
using PMGSY.Models;
namespace PMGSY.Controllers
{
    [RequiredAuthentication]
    [RequiredAuthorization]
    public partial class AccountsReportsController : Controller
    {
        IAccountReportsDAL objAccountDAL = new AccountReportsDAL();
        IAccountReportsBAL objAccountBAL = new AccountReportsBAL();
        int outParam = 0;
          
        public AccountsReportsController()
        {
            PMGSYSession.Current.ModuleName = "Reports";
        }

        #region AnnualAccount
        /// <summary>
        /// Action is used to load Annual Account search form.
        /// </summary>
        /// <returns></returns>
            [Audit]
        public ActionResult AnnualAccountSearch()
        {
            AnnualAccount objAnnualAccount = new AnnualAccount();
            CommonFunctions objCommFunc = new CommonFunctions();
            if (PMGSYSession.Current.UserId == 0)
            {
                Response.Redirect("/Login/SessionExpire");
            }

            List<SelectListItem> lstSRRDA = new List<SelectListItem>();
            lstSRRDA = objAccountDAL.PopulateSRRDA();
            if (PMGSYSession.Current.LevelId == 4)
            {
                var selected = lstSRRDA.Where(x => x.Value == PMGSYSession.Current.AdminNdCode.ToString()).First();
                selected.Selected = true;
            }

            ViewBag.ddlState = lstSRRDA;

            //ViewBag.State =objAccountDAL.PopulateSRRDA();
            //objAnnualAccount.State =PMGSYSession.Current.StateCode.ToString();
            
            
            
            //lstSRRDA.Insert(0,new SelectListItem{Text="Select SRRDA",Value="0"});

           // ViewBag.SRRDA=lstSRRDA;

            List<SelectListItem> lstDPIU=new List<SelectListItem>();
            lstDPIU.Insert(0,new SelectListItem{Text="All DPIU",Value="0"});
            ViewBag.DPIU = lstDPIU;
            ViewBag.Year = objAccountDAL.getAllYears();
            ViewBag.BalanceType = objAccountDAL.getBalanceType();
            return View(objAnnualAccount);
        }

        //[HttpPost]
        //public JsonResult PopulateSRRDA(string id)
        //{
        //    int StateCode = Convert.ToInt32(id);
        //    return Json(objAccountDAL.PopulateSRRDA(StateCode), JsonRequestBehavior.AllowGet);
        //}

        [HttpPost]
        [Audit]
               public JsonResult PopulateDPIU(string id)
        {
            CommonFunctions objCommFunc=new CommonFunctions();
            string[] param = id.Split('$');
             
            List<SelectListItem> lstDPIU = new List<SelectListItem>();
            if (id != string.Empty)
            {
                int DPIUCode = Convert.ToInt32(param[0]);
                lstDPIU = objCommFunc.PopulateDPIUOfSRRDA(DPIUCode);
            }
            
            if(lstDPIU.Count==1)
            {            
                lstDPIU.RemoveAt(0);
            }

            if (param[1] == "A")
            {
                lstDPIU.Insert(0, new SelectListItem { Text = "All DPIU", Value = "0" });
            }
            else
            {
                lstDPIU.Insert(0, new SelectListItem { Text = "Select DPIU", Value = "0" });
            }
            return Json(lstDPIU, JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// Action is used to load Annual Account list view based on search criteria.
        /// </summary>
        /// <param name="objAnnualAccount">Model object contains Year and BalanceType as a parameter which are passed from search form.</param>
        /// <returns></returns>
        [HttpPost]
        [Audit]      
        public ActionResult AnnualAccountDetails(AnnualAccount objAnnualAccount)
        {
            if (PMGSYSession.Current.UserId == 0)
            {
                Response.Redirect("/Login/SessionExpire");
            }

            ReportFilter objParam = new ReportFilter();         
            ViewBag.Year = (objAnnualAccount.Year)+"-"+(objAnnualAccount.Year+1);
            if (PMGSYSession.Current.LevelId == 5 || PMGSYSession.Current.LevelId == 4)
            {
                ViewBag.State = PMGSYSession.Current.StateName;
            }
            if(PMGSYSession.Current.LevelId == 6)
            {
                ViewBag.State = objAccountDAL.StateName(Convert.ToInt32(objAnnualAccount.State));
            }
            objParam.LevelId = PMGSYSession.Current.LevelId;
            if (objAnnualAccount.Selection == "S" || objAnnualAccount.Selection=="R")
            {
                objParam.AdminNdCode = Convert.ToInt32(objAnnualAccount.State);
            }
            else if(objAnnualAccount.Selection=="D")
            {
                if (objAnnualAccount.DPIU == "0")
                {
                    objParam.AdminNdCode = Convert.ToInt32(objAnnualAccount.State);
                    objParam.DPIUSelection = objAnnualAccount.DPIU;
                }
                else
                {
                    objParam.AdminNdCode = Convert.ToInt32(objAnnualAccount.State);
                    objParam.LowerAdminNdCode = Convert.ToInt32(objAnnualAccount.DPIU);
                    objParam.DPIUSelection = objAnnualAccount.DPIU;
                }
            }
            objParam.FundType = PMGSYSession.Current.FundType;
            objParam.CreditDebit = objAnnualAccount.CreditDebit;
            objParam.Year = objAnnualAccount.Year;
            objParam.Selection = objAnnualAccount.Selection;
            //if (PMGSYSession.Current.LevelId == 6)
            //{
            //    objParam.AdminNdCode = Convert.ToInt32(objAnnualAccount.State);
            //}
            //else
            //{
            //    objParam.AdminNdCode = PMGSYSession.Current.AdminNdCode;
            //}
            if (PMGSYSession.Current.LevelId == 4 || PMGSYSession.Current.LevelId == 6)
            {
                objParam.LowerAdminNdCode = Convert.ToInt32(objAnnualAccount.DPIU);
            }
            else
            {
                objParam.LowerAdminNdCode = PMGSYSession.Current.AdminNdCode;
            }
            objAnnualAccount = objAccountBAL.GetAnnualAccountList(objParam);            
            return PartialView("AnnualAccountDetails",objAnnualAccount);

        }

        

        #endregion AnnualAccount
        
        #region BillDetails

        /// <summary>
        /// Action is used to load the search form.
        /// </summary>
        /// <returns></returns>
          [Audit]
        public ActionResult DisplayBillDetails()
        {
            if (PMGSYSession.Current.UserId == 0)
            {
                Response.Redirect("/Login/SessionExpire");
            }
            
            CommonFunctions objCommon = new CommonFunctions();
            AccountBillViewModel accModel = new AccountBillViewModel();           
            TransactionParams objParam = new TransactionParams();

            //Added by Vikram on 07 Jan 2014
            if (PMGSYSession.Current.AccMonth != 0)
            {
                objParam.MONTH = PMGSYSession.Current.AccMonth;
                objParam.YEAR = PMGSYSession.Current.AccYear;
            }
            else
            {
                objParam.MONTH = Convert.ToInt16(DateTime.Now.Month);
                objParam.YEAR = Convert.ToInt16(DateTime.Now.Year);
            }

            objParam.ADMIN_ND_CODE = PMGSYSession.Current.AdminNdCode;
            
            //ViewData["DPIU"] = new SelectList(objCommon.PopulateDPIU(objParam), "Value", "Text");          

            //Modified for all piu change by abhishek kamble 21-June-2014

            List<SelectListItem> lstPIU = objCommon.PopulateDPIU(objParam);
            lstPIU.RemoveAt(0);
            lstPIU.Insert(0, new SelectListItem { Text = "All PIU", Value = "0" });
              
            ViewData["DPIU"] = new SelectList(lstPIU, "Value", "Text");          

            
            //ViewData["Months"] =new SelectList(objCommon.PopulateMonths(objParam.MONTH),"Value","Text",objParam.MONTH);

            //ViewData["Year"] = new SelectList(objCommon.PopulateYears(objParam.YEAR), "Value", "Text",objParam.YEAR);
            
            accModel.MonthList = objCommon.PopulateMonths(objParam.MONTH); 
            accModel.YearList = objCommon.PopulateYears(objParam.YEAR);

            //commented by koustubh nakate on 26/09/2013 for populating years
            //  ViewData["Year"] = new SelectList(objAccountDAL.getAllYears(), "Value", "Text");
            //Added by Vikram on 07 Jan 2014
            if (PMGSYSession.Current.AccMonth != 0)
            {
                accModel.Month = PMGSYSession.Current.AccMonth;
                accModel.Year = PMGSYSession.Current.AccYear;
            }
            else
            {
                accModel.Month = Convert.ToInt16(DateTime.Now.Month);
                accModel.Year = Convert.ToInt16(DateTime.Now.Year);
            }
            accModel.ddlBillType = accModel.GetBillType();
            return View("DisplayBillDetails", accModel);
        }

        /// <summary>
        /// Action is used to load bill details list view based on search parameter.
        /// </summary>
        /// <param name="accModel">Model object contains parameter which are selected from search form.</param>
        /// <returns></returns>
        [HttpPost]
       [Audit]
        public ActionResult ShowBillDetails(AccountBillViewModel accModel)
        {                                     
            //modified by abhishek kamble 12-nov-2013
            if (ModelState.IsValid)
            {
                accModel.levelId = PMGSYSession.Current.LevelId;
                accModel.FundType = PMGSYSession.Current.FundType;
                accModel = objAccountBAL.getBillDetails(accModel);                
            }
            return PartialView("ShowBillDetails", accModel);
        }

        /// <summary>
        /// Action is used to load transaction details list view.
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
          [Audit]
        public ActionResult TransactionDetailsView(string param)
        {
            string[] parameters = param.Split(',');

            AccountBillViewModel model = new AccountBillViewModel();
            model.BillId = Convert.ToInt64(parameters[0]);
            model.BillType = parameters[1];
            return View("TransactionDetailsView", model);
        }
        /// <summary>
        ///  Method is used to load transaction details grid based on search parameter.
        /// </summary>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <param name="sidx"></param>
        /// <param name="sord"></param>
        /// <returns></returns>
          [Audit]
        public ActionResult GetTransactionDetails(int? page, int? rows, string sidx, string sord)
        {  
            String searchParameters = String.Empty;
            long totalRecords;      
            string billType = string.Empty;
            string billId = string.Empty;
            string sidxNew=sidx.Replace("asc,", "").Trim();
            using (CommonFunctions commonFunction = new CommonFunctions())
            {
                if (!commonFunction.ValidateGridParameters(new GridParams(page, rows, sidxNew, sord, Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                {
                    return null;
                }
            }           

            if (!string.IsNullOrEmpty(Request.Params["BillId"]))
            {
                billId = Request.Params["BillId"];
            }
           
            var jsonData = new
            {
                rows = objAccountBAL.lstTransactionDetails(billId, Convert.ToInt32(page) - 1, Convert.ToInt32(rows), sidx, sord, out totalRecords),
                total =0,
                page = Convert.ToInt32(page),
                records = totalRecords
            };

            return Json(jsonData);
        }

        //addition by Koustubh Nakate on 26/09/2013 to populate years

        [HttpPost]
        [Audit]
        public JsonResult GetYears()
        {
            try
            {
                CommonFunctions objCommon = new CommonFunctions();

                List<SelectListItem> yearList = new List<SelectListItem>();

                if (PMGSYSession.Current.AccYear != 0)
                {
                    return Json(objCommon.PopulateYears(PMGSYSession.Current.AccYear));
                }
                else
                {
                    return Json(objCommon.PopulateYears(DateTime.Now.Year));
                    //yearList = objCommon.PopulateYears(DateTime.Now.Year);
                }
                //return Json(yearList);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return Json(false);
            }
        }//end function GetYears


        [HttpPost]
        [Audit]
        public JsonResult GetFullYears()
        {
            try
            {
                if (PMGSYSession.Current.AccYear != 0)
                {
                    SelectList lstYears= new SelectList(objAccountDAL.getAllYears(), "Value", "Text", PMGSYSession.Current.AccYear);
                    return Json(lstYears);
                }
                else {
                    return Json(new SelectList(objAccountDAL.getAllYears(), "Value", "Text"));
                }
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return Json(false);
            }
        }//end function GetYears

        #endregion BillDEtails

        #region ReportHeader

        //public ActionResult ReportHeader()
        //{
        //    ReportHeader rptHeader =objAccountBAL.GetReportheader() ;
        //    ViewBag.FormNo = rptHeader.FormNo;
        //    ViewBag.ReportName = rptHeader.ReportName;
        //    ViewBag.RptParaName = rptHeader.ReportParaGraphName;

        //    return View("ReportHeader");
       

        //}
        

        #endregion ReportHeader

        #region ContractorLedger

        /// <summary>
        /// Method is used to load search page.
        /// </summary>
        /// <returns></returns>
          [Audit]
        public ActionResult ContrctorLedger()
        {

            int adminNDCode = PMGSYSession.Current.AdminNdCode;
            ContractorLedgerModel contractorLedger = new ContractorLedgerModel();
            CommonFunctions objCommanFunc = new CommonFunctions();
            TransactionParams objParam = new TransactionParams();

            objParam.DISTRICT_CODE =Convert.ToInt32(objAccountDAL.GetDistrictCode(adminNDCode));

           List<SelectListItem> lstContractorSupplier = objCommanFunc.PopulateContractorSupplier(objParam);

           lstContractorSupplier.RemoveAt(0);
           lstContractorSupplier.Insert(0,new SelectListItem {Text="Select Contractor",Value="0" });
           ViewData["Contractor"] = lstContractorSupplier;
            // contractorLedger.LevelId = PMGSYSession.Current.LevelId;
            if (contractorLedger.LevelId == 5 && PMGSYSession.Current.ParentNDCode.HasValue)
            {

                contractorLedger.SRRDACode = Convert.ToInt32(PMGSYSession.Current.ParentNDCode);
                contractorLedger.PIUCode = PMGSYSession.Current.AdminNdCode;

            }
            else
            {
                contractorLedger.SRRDACode = Convert.ToInt32(PMGSYSession.Current.AdminNdCode);
                contractorLedger.ddlPIU = objCommanFunc.PopulateDPIUOfSRRDA(contractorLedger.SRRDACode);
                contractorLedger.ddlPIU.Insert(0, new SelectListItem { Text = "Select DPIU", Value = "0", Selected = true });
            }


            return View(contractorLedger);
        }

        /// <summary>
        /// Method is used to populate contractor based on PIU code passed to it.
        /// </summary>
        /// <param name="PIUCode"></param>
        /// <returns></returns>

        [HttpPost]
        [Audit]
        public JsonResult PopulateContractor(string PIUCode)
        {
            TransactionParams objParam = new TransactionParams();
            ContractorLedgerModel contractorLedger = new ContractorLedgerModel();
            CommonFunctions objCommanFunc = new CommonFunctions();
            objParam.FUND_TYPE = PMGSYSession.Current.FundType;
            // objParam.STATE_CODE = PMGSYSession.Current.StateCode;
            if (PMGSYSession.Current.LevelId == 4)
            {
                objParam.DISTRICT_CODE =Convert.ToInt32(objAccountDAL.GetDistrictCode(Convert.ToInt32(PIUCode)));
            }
            else if(PMGSYSession.Current.LevelId==5)
            {
                objParam.DISTRICT_CODE = Convert.ToInt32(objAccountDAL.GetDistrictCode(PMGSYSession.Current.AdminNdCode));
            }
            objParam.LVL_ID = PMGSYSession.Current.LevelId;

            // contractorLedger.ddlContractor = objCommanFunc.PopulateContractorSupplier(objParam);
            List<SelectListItem> lstContractorSupplier = objCommanFunc.PopulateContractorSupplier(objParam);

            lstContractorSupplier.RemoveAt(0);
            lstContractorSupplier.Insert(0, new SelectListItem { Text = "Select Contractor", Value = "0" });
            return Json(new SelectList(lstContractorSupplier, "Value", "Text"), JsonRequestBehavior.AllowGet);

            //return Json(new SelectList(objCommanFunc.PopulateContractorSupplier(objParam), "Value", "Text"), JsonRequestBehavior.AllowGet);

        }
        
        /// <summary>
        /// Method is used to populate Aggrement based on parameters passed to it.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        [Audit]
        public JsonResult PopulateAggrement(string id)
        {
            string[] param = id.Split(',');

            int contractorId =Convert.ToInt32(param[0]);
            int PIUCode = 0;
            if (PMGSYSession.Current.LevelId == 4)
            {
                 PIUCode = Convert.ToInt32(param[1]);
            }
            TransactionParams objParam = new TransactionParams();
            ContractorLedgerModel contractorLedger = new ContractorLedgerModel();
            CommonFunctions objCommanFunc = new CommonFunctions();
            objParam.FUND_TYPE = PMGSYSession.Current.FundType;
            if (PMGSYSession.Current.LevelId == 4)
            {
                objParam.DISTRICT_CODE = Convert.ToInt32(objAccountDAL.GetDistrictCode(PIUCode));
            }
            else if(PMGSYSession.Current.LevelId==5)
            {
                objParam.DISTRICT_CODE =Convert.ToInt32(objAccountDAL.GetDistrictCode(PMGSYSession.Current.AdminNdCode));
            }
            objParam.MAST_CONT_ID = contractorId;
            objParam.LVL_ID = PMGSYSession.Current.LevelId;

            // contractorLedger.ddlContractor = objCommanFunc.PopulateContractorSupplier(objParam);
            return Json(new SelectList(objCommanFunc.PopulateAgreement(objParam), "Value", "Text"), JsonRequestBehavior.AllowGet);

        }

          [Audit]
        public ActionResult ContractorLedgerView(ContractorLedgerModel contractorLedger)
        {
            contractorLedger.FundType = PMGSYSession.Current.FundType;
            contractorLedger.LevelId = PMGSYSession.Current.LevelId;
            contractorLedger = objAccountBAL.GerReportHeader(contractorLedger);
            return View("ContractorLedgerView",contractorLedger);
        }

          [Audit]
          public ActionResult GetContLedgerDetails(int? page, int? rows, string sidx, string sord)
        {
            if (PMGSYSession.Current.UserId == 0)
            {
                Response.Redirect("/Login/SessionExpire");
            }

            String searchParameters = String.Empty;
            long totalRecords;
            int PIUCode =0;
            int AggrementCode = 0;
            int ContrCode =0;
           
            //using (CommonFunctions commonFunction = new CommonFunctions())
            //{
            //    if (!commonFunction.ValidateGridParameters(new GridParams(page, rows, sidx, sord, Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
            //    {
            //        return null;
            //    }
            //}

            if (PMGSYSession.Current.LevelId == 5)
            {
                PIUCode = PMGSYSession.Current.AdminNdCode;
            }
            if (PMGSYSession.Current.LevelId == 4)
            {
                if (!string.IsNullOrEmpty(Request.Params["PIUCode"]))
                {
                    PIUCode = Convert.ToInt32(Request.Params["PIUCode"]);
                }
            }
            if (!string.IsNullOrEmpty(Request.Params["AggrementCode"]))
            {
                AggrementCode =Convert.ToInt32(Request.Params["AggrementCode"]);
            }
            if (!string.IsNullOrEmpty(Request.Params["ContrCode"]))
            {
                ContrCode =Convert.ToInt32(Request.Params["ContrCode"]);
            }

            var jsonData = new
            {
                rows =objAccountBAL.lstContLedgerDetails(PIUCode,ContrCode,AggrementCode,Convert.ToInt32(page)-1,Convert.ToInt32(rows),sidx,sord,out totalRecords),
                total = 0,
                page = Convert.ToInt32(page),
                records = totalRecords
            };

            return Json(jsonData);
        }


        /// <summary>
        /// Method is to populate ledger details based on search parameters.
        /// </summary>
        /// <param name="contractorLedger"></param>
        /// <returns></returns>
        [HttpPost]
        [Audit]       
        public ActionResult ContractorLedgerDetails(ContractorLedgerModel contractorLedger)
        {
            contractorLedger.LevelId = PMGSYSession.Current.LevelId;
            contractorLedger.FundType = PMGSYSession.Current.FundType;

         
            contractorLedger = objAccountBAL.getLedgerDetails(contractorLedger);
            return View("ContractorLedgerDetails", contractorLedger);
        }
       


        #endregion ContractorLedger

        #region Chequebook details

        /// <summary>
        /// ChequeBookDetails() action is used to display Cheque book details search form.
        /// </summary>
        /// <returns></returns>
        [Audit]
        public ActionResult ChequeBookDetails()
        {

            try
            {
                //if (PMGSYSession.Current.UserId == 0)
                //{
                //    Response.Redirect("/Login/SessionExpire");
                //}

                CommonFunctions objCommonFunction = new CommonFunctions();
                IPaymentDAL objPaymentDal = new PaymentDAL();

                //populate DPIU
                TransactionParams objParam = new TransactionParams();

                //Added by Vikram on 07 Jan 2014
                if (PMGSYSession.Current.AccMonth != 0)
                {
                    objParam.MONTH = PMGSYSession.Current.AccMonth;
                    objParam.YEAR = PMGSYSession.Current.AccYear;
                }
                else
                {
                    objParam.MONTH = Convert.ToInt16(DateTime.Now.Month);
                    objParam.YEAR = Convert.ToInt16(DateTime.Now.Year);
                }
                //end of change


                objParam.ADMIN_ND_CODE = PMGSYSession.Current.AdminNdCode;
                ViewData["DPIU"] = new SelectList(objCommonFunction.PopulateDPIU(objParam), "Value", "Text");

                //populate Month
                //ViewData["Month"] = new SelectList(objCommonFunction.PopulateMonths(objParam.MONTH), "Value", "Text");
                ViewData["Month"] = objCommonFunction.PopulateMonths(objParam.MONTH);

                //populate Year
                ViewData["Year"] = new SelectList(objCommonFunction.PopulateYears(objParam.YEAR), "Value", "Text");

                if (PMGSYSession.Current.LevelId == 6||PMGSYSession.Current.LevelId==4)
                {
                    //Populate SRRDA
                    ViewData["SRRDA"] = new SelectList(objCommonFunction.PopulateNodalAgencies(), "Value", "Text");
                }
                //populate CheckBook Series
                if (PMGSYSession.Current.LevelId == 5) //DPIU
                {
                    ViewData["ChequebookSeries"] = new SelectList(objPaymentDal.GetchequebookSeries(PMGSYSession.Current.AdminNdCode, PMGSYSession.Current.FundType, PMGSYSession.Current.LevelId), "Value", "Text");
                }
                else if (PMGSYSession.Current.LevelId == 4 ||PMGSYSession.Current.LevelId==6)  //SRRDA and MORD
                {
                    ViewData["ChequebookSeries"] = new SelectList(objPaymentDal.GetchequebookSeries(0, PMGSYSession.Current.FundType, PMGSYSession.Current.LevelId), "Value", "Text");
                }
                CheckBookDetailsViewFilterModel chequebookDetailsViewModel = new CheckBookDetailsViewFilterModel();
                chequebookDetailsViewModel.LevelId = PMGSYSession.Current.LevelId;
                //chequebookDetailsViewModel.Month = (Int16)DateTime.Now.Month;
                //chequebookDetailsViewModel.Year = (Int16)DateTime.Now.Year;

                //Added by Vikram on 07 Jan 2014
                if (PMGSYSession.Current.AccMonth != 0)
                {
                    chequebookDetailsViewModel.Month = PMGSYSession.Current.AccMonth;
                    chequebookDetailsViewModel.Year = PMGSYSession.Current.AccYear;
                }
                else
                {
                    chequebookDetailsViewModel.Month = Convert.ToInt16(DateTime.Now.Month);
                    chequebookDetailsViewModel.Year = Convert.ToInt16(DateTime.Now.Year);
                }
                //end of change


                return View("ChequeBookDetails", chequebookDetailsViewModel);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);              
          
                return View("ChequeBookDetails", new CheckBookDetailsViewFilterModel());
            }
        }

        /// <summary>
        /// ShowChequebookDetails() action is used to display cheque book details based on search criteria.
        /// </summary>
        /// <param name="checkbookDetailsViewFilterModel">
        ///if Monthwise search: contains selected Month and year 
        ///if Chequebook wise search: contains selected cheque book series
        /// </param>
        /// <returns></returns>
        [HttpPost]
        [Audit]
        public ActionResult ShowChequebookDetails(CheckBookDetailsViewFilterModel checkbookDetailsViewFilterModel)
        {
            //if (PMGSYSession.Current.UserId == 0)
            //{
            //    Response.Redirect("/Login/SessionExpire");
            //}

            //if (PMGSYSession.Current == null)
            //{
            //    Response.Redirect("/Login/Login");
            //}
            //AccountReportsBAL objBAL = new AccountReportsBAL();


            //set Report Hearder
            //ReportHeader objReportHeader = new ReportHeader();

            if (ModelState.IsValid)
            {

                checkbookDetailsViewFilterModel = objAccountBAL.getCheckBookDetails(checkbookDetailsViewFilterModel);
                return PartialView("ShowChequebookDetails", checkbookDetailsViewFilterModel);
            }
            else
            {
                return Json(new { success = false, message = new CommonFunctions().FormatErrorMessage(ModelState) });
                //return PartialView("ShowChequebookDetails", checkbookDetailsViewFilterModel);
            }
        }


        /// <summary>
        /// Populate Chequebook Details on DPIU drop down change event based on selected DPIU
        /// </summary>
        /// <param name="paramAdminNdCode"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult GetChequebookSeriesByAdminNdCode(string paramAdminNdCode)
        {
            try
            {
                //if (PMGSYSession.Current.UserId == 0)
                //{
                //    Response.Redirect("/Login/SessionExpire");
                //}
                IPaymentDAL objPaymentDal = new PaymentDAL();
                int adminNdCode = Convert.ToInt32(paramAdminNdCode);
                return Json(new SelectList(objPaymentDal.GetchequebookSeries(adminNdCode, PMGSYSession.Current.FundType, 5), "Value", "Text"));
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);              
          
                return Json(false);
            }
        }

        #endregion Chequebook details

        #region authorised Signatory Details
          [Audit]
        public ActionResult ListAuthorizedSignatoryDetails()
        {
            try
            {
                //if (PMGSYSession.Current.UserId == 0)
                //{
                //    Response.Redirect("/Login/SessionExpire");
                //}
                return View("ListAuthorizedSignatoryDetails", new AuthorisedSignatoryViewModel());
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);              
          
                return null;
            }
        }

        /// <summary>
        /// ShowAuthSignatoryDetails() action is used to display Authorised signatory details based on search criteria.
        /// </summary>
        /// <param name="authSignatoryViewModel">
        /// </param>
        /// <returns></returns>
        [HttpPost]
        [Audit]
   
        public ActionResult ShowAuthSignatoryDetails(AuthorisedSignatoryViewModel authSignatoryViewModel)
        {
            //if (PMGSYSession.Current.UserId == 0)
            //{
            //    Response.Redirect("/Login/SessionExpire");
            //}

            authSignatoryViewModel = objAccountBAL.getAuthSignatoryDetails(authSignatoryViewModel);
            return PartialView("ShowAuthSignatoryDetails", authSignatoryViewModel);
        }

        #endregion

        #region Asset Register details

        /// <summary>
        /// AssetRegisterDetails() action is used to display Asset Register details search form.
        /// </summary>
        /// <returns></returns>
          [Audit]
        public ActionResult AssetRegisterDetails()
        {
            try
            {
                CommonFunctions objCommonFunction = new CommonFunctions();
                IPaymentDAL objPaymentDal = new PaymentDAL();
                
                //populate DPIU
                TransactionParams objParam = new TransactionParams();
                objParam.ADMIN_ND_CODE = PMGSYSession.Current.AdminNdCode;
                ViewData["DPIU"] = new SelectList(objCommonFunction.PopulateDPIU(objParam), "Value", "Text");

                //populate Month
                ViewData["Month"] = new SelectList(objCommonFunction.PopulateMonths(true), "Value", "Text");

                //populate Year
                ViewData["Year"] = new SelectList(objCommonFunction.PopulateYears(true), "Value", "Text");

                //populate Fund-Central/State/All

                ViewData["Fund"] = new SelectList(objAccountDAL.getFundForStateCentral(), "Value", "Text");

                AssetRegisterViewModel assetRegisterViewModel = new AssetRegisterViewModel();

                if (PMGSYSession.Current.AccMonth != 0)
                {
                    assetRegisterViewModel.Month = PMGSYSession.Current.AccMonth;
                    assetRegisterViewModel.Year = PMGSYSession.Current.AccYear;
                }
                else
                {
                    assetRegisterViewModel.Month = Convert.ToInt16(DateTime.Now.Month);
                    assetRegisterViewModel.Year = Convert.ToInt16(DateTime.Now.Year);
                }

                return View("AssetRegisterDetails", assetRegisterViewModel);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);              
          
                return View("AssetRegisterDetails", new AssetRegisterViewModel());
            }
        }

        /// <summary>
        /// ShowChequebookDetails() action is used to display cheque book details based on search criteria.
        /// </summary>
        /// <param name="checkbookDetailsViewFilterModel">
        ///if Monthwise search: contains selected Month and year 
        ///if Chequebook wise search: contains selected cheque book series
        /// </param>
        /// <returns></returns>
        [HttpPost]
        [Audit]
   
        public ActionResult ShowAssetRegisterDetails(AssetRegisterViewModel assetRegisterViewModel)
        {
            if (ModelState.IsValid)
            {

                assetRegisterViewModel = objAccountBAL.getAssetRegisterDetails(assetRegisterViewModel);
                return PartialView("ShowAssetRegisterDetails", assetRegisterViewModel);
            }
            else
            {
                return PartialView("ShowAssetRegisterDetails", assetRegisterViewModel);
            }
        }

        #endregion Asset Register details

        #region FundTransfer

        [Audit]
        public ActionResult FundTransferView()
        {           
            //CommonFunctions objCommFunc = new CommonFunctions();
            //ViewBag.Year = objCommFunc.PopulateYears(true);
            //ViewBag.Month = objCommFunc.PopulateMonths(true);
            //ViewBag.FundSelection = objAccountBAL.PopulateFund();

            //List<SelectListItem> lstSRRDA=new List<SelectListItem>();

            //lstSRRDA=objAccountDAL.PopulateSRRDA();
            //if (PMGSYSession.Current.LevelId == 4)
            //{
            //    var selected=lstSRRDA.Where(m=>m.Value==PMGSYSession.Current.AdminNdCode.ToString()).First();
            //    selected.Selected=true;
            //}
      
            // ViewBag.SRRDA = lstSRRDA;
           
            //List<SelectListItem> lstDPIU = new List<SelectListItem>();
            //lstDPIU.Insert(0, new SelectListItem { Text = "All DPIU", Value = "0" });
            //ViewBag.DPIU = lstDPIU;
            string ScreenName = "FundTransfer";
            CommanViewFunction(ScreenName);
            //FundTransferViewModel model = new FundTransferViewModel();
            //if (PMGSYSession.Current.AccMonth != 0)
            //{
            //    model.Month = PMGSYSession.Current.AccMonth.ToString();
            //    model.Year = PMGSYSession.Current.AccYear.ToString();
            //}
            //else
            //{
            //    model.Month = DateTime.Now.Month.ToString();
            //    model.Year = DateTime.Now.Year.ToString();
            //}


            //return View(model);
            return View();
        }

        [Audit]
        public ActionResult FundTransferDetails(FundTransferViewModel objFundTransfer)
        {
            ReportFilter objParam = new ReportFilter();
            if (ModelState.IsValid)
            {

                if (PMGSYSession.Current.LevelId == 5)
                {
                    objParam.AdminNdCode = Convert.ToInt32(PMGSYSession.Current.ParentNDCode);
                    objParam.LowerAdminNdCode = PMGSYSession.Current.AdminNdCode;
                }
                else
                {
                    objParam.AdminNdCode = Convert.ToInt32(objFundTransfer.StateCode);
                    objParam.LowerAdminNdCode = Convert.ToInt32(objFundTransfer.DPIUCode);
                }
                objParam.Head = Convert.ToInt16(objFundTransfer.HeadCode);
                objParam.Month = Convert.ToInt16(objFundTransfer.Month);
                objParam.Year = Convert.ToInt16(objFundTransfer.Year);

                objParam.HeadName = objFundTransfer.HeadName;
                


                ViewBag.YearName = objFundTransfer.YearName;
                ViewBag.MonthName = objFundTransfer.MonthName;
                ViewBag.HeadName = objFundTransfer.HeadName;

                
                //modified by abhishek kamble 12-dec-2013
                if(PMGSYSession.Current.LevelId==5)
                {
                    PMGSY.DAL.Reports.IReportDAL objReportDAL = new PMGSY.DAL.Reports.ReportDAL();                    
                    ViewBag.StateName = objReportDAL.GetDepartmentName(objParam.AdminNdCode);                         
                    ViewBag.DPIUName = PMGSYSession.Current.DepartmentName;
                }
                else{
                    ViewBag.StateName = objFundTransfer.StateName;
                    ViewBag.DPIUName = objFundTransfer.DPIUName;
                }

                objFundTransfer = objAccountBAL.GetFundTransferDetails(objParam);

                return View(objFundTransfer);
            }
            else
            {
                return Json(new { success = false, message = new CommonFunctions().FormatErrorMessage(ModelState)});
            }

        }

        #endregion FundTransfer

        #region AbstractFundTransfer

        [Audit]
        public ActionResult AbstractFundView()
        {
            //CommonFunctions objCommFunc = new CommonFunctions();
            //ViewBag.Year = objCommFunc.PopulateFinancialYear(true);
            //ViewBag.Month = objCommFunc.PopulateMonths(true);
            //ViewBag.FundSelection = objAccountBAL.PopulateFund();

            //List<SelectListItem> lstSRRDA = new List<SelectListItem>();

            //lstSRRDA = objAccountDAL.PopulateSRRDA();
            //if (PMGSYSession.Current.LevelId == 4)
            //{
            //    var selected = lstSRRDA.Where(m => m.Value == PMGSYSession.Current.AdminNdCode.ToString()).First();
            //    selected.Selected = true;
            //}

            //ViewBag.SRRDA = lstSRRDA;

            //List<SelectListItem> lstDPIU = new List<SelectListItem>();
            //lstDPIU.Insert(0, new SelectListItem { Text = "All DPIU", Value = "0" });
            //ViewBag.DPIU = lstDPIU;
            string ScreenName = "AbstractFund";
            CommanViewFunction(ScreenName);
            return View();
        }

        [Audit]
        public ActionResult AbstractFundDetails(AbstractFundTransferredViewModel objAbstractFund)
        {
            if (ModelState.IsValid)
            {
                objAbstractFund = objAccountBAL.GetReportHeaderInfo(objAbstractFund);

                return View("AbstractFundDetails", objAbstractFund);
            }
            else
            {
                return Json(new { success = false, message = new CommonFunctions().FormatErrorMessage(ModelState) });
            }
        }



        [Audit]
        public ActionResult GetAbstractFundDetails(int? page, int? rows, string sidx, string sord)
        {
            if (PMGSYSession.Current.UserId == 0)
            {
                Response.Redirect("/Login/SessionExpire");
            }

            String searchParameters = String.Empty;
            long totalRecords;
            Int16  Year= 0;
            Int16 Head = 0;
            int AdminNdCode = 0;
            int LoweAdminNdCode = 0;

            using (CommonFunctions commonFunction = new CommonFunctions())
            {
                if (!commonFunction.ValidateGridParameters(new GridParams(page, rows, sidx, sord, Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                {
                    return null;
                }
            }

            if (!string.IsNullOrEmpty(Request.Params["Year"]))
            {
                Year = Convert.ToInt16(Request.Params["Year"]);
            }
            if (!string.IsNullOrEmpty(Request.Params["Head"]))
            {
                Head = Convert.ToInt16(Request.Params["Head"]);
            }
            if (!string.IsNullOrEmpty(Request.Params["State"]))
            {
                AdminNdCode = Convert.ToInt16(Request.Params["State"]);
            }
            if (!string.IsNullOrEmpty(Request.Params["DPIU"]))
            {
                LoweAdminNdCode = Convert.ToInt16(Request.Params["DPIU"]);
            }

            //if (PMGSYSession.Current.LevelId == 5)
            //{
            //    PIUCode = PMGSYSession.Current.AdminNdCode;
            //}
            //if (PMGSYSession.Current.LevelId == 4)
            //{
            //    if (!string.IsNullOrEmpty(Request.Params["PIUCode"]))
            //    {
            //        PIUCode = Convert.ToInt32(Request.Params["PIUCode"]);
            //    }
            //}
            //if (!string.IsNullOrEmpty(Request.Params["AggrementCode"]))
            //{
            //    AggrementCode = Convert.ToInt32(Request.Params["AggrementCode"]);
            //}
            //if (!string.IsNullOrEmpty(Request.Params["ContrCode"]))
            //{
            //    ContrCode = Convert.ToInt32(Request.Params["ContrCode"]);
            //}

            var jsonData = new
            {
                rows = objAccountBAL.GetAbstractFundDetails(Year,Head,AdminNdCode,LoweAdminNdCode, Convert.ToInt32(page) - 1, Convert.ToInt32(rows), sidx, sord, out totalRecords),
                total = 0,
                page = Convert.ToInt32(page),
                records = totalRecords
            };

            return Json(jsonData);
        }

        //public ActionResult AbstractFundDetails(AbstractFundTransferredViewModel objAbstractFund)
        //{
        //    ReportFilter objParam = new ReportFilter();
        //    //AbstractFundTransferredViewModel objAbstractFund = new AbstractFundTransferredViewModel();

        //    objParam.Year =Convert.ToInt16(objAbstractFund.Year);
        //    objParam.Head =Convert.ToInt16(objAbstractFund.Head);
        //    objParam.AdminNdCode =Convert.ToInt32(objAbstractFund.State);
        //    objParam.LowerAdminNdCode =Convert.ToInt32(objAbstractFund.DPIU);

        //    objAbstractFund = objAccountBAL.AbstractFundDetails(objParam);

        //    return View(objAbstractFund);

        //}

        #endregion AbstractFundTransfer

        #region Abstract Bank Authorization
     
        /// <summary>
        /// Display Search page of Abstract Bank Details
        /// </summary>
        /// <returns></returns>          
        [Audit]
        public ActionResult AbstractBankAuthorization()
        {
            string name = "AbstractBankAuthorization";
            CommanViewFunction(name);
            return View();
        }

        /// <summary>
        /// Display Report Header
        /// </summary>
        /// <param name="abstractBankAuthViewModel"></param>
        /// <returns></returns>
        [HttpPost]
        [Audit]
      
        public ActionResult AbstractBankAuthDetails(AbstractBankAuthViewModel abstractBankAuthViewModel)
        {
            return PartialView("AbstractBankAuthDetails", objAccountBAL.AbstractBankAuthDetails(abstractBankAuthViewModel));
        }

        /// <summary>
        /// List Abstract Bank Authorization Details
        /// </summary>
        /// <param name="formCollection"></param>
        /// <returns></returns>
        [Audit]
        public ActionResult ListAbstractBankAuthDetails(FormCollection formCollection)
        {
            short year=0;
            int StateSRRDA = 0;
            int DPIU = 0;
            long totalRecords;

            if (!string.IsNullOrEmpty(Request.Params["YEAR"]))
            {
                year = Convert.ToInt16(Request.Params["YEAR"]);
            }

            if (!string.IsNullOrEmpty(Request.Params["STATE_SRRDA"]))
            {
                StateSRRDA = Convert.ToInt32(Request.Params["STATE_SRRDA"]);
            }

            if (!string.IsNullOrEmpty(Request.Params["DPIU"]))
            {
                DPIU = Convert.ToInt32(Request.Params["DPIU"]);
            }

            var jsonData = new
            {
                rows=objAccountBAL.ListAbstractBankAuthDetails(year,StateSRRDA,DPIU,Convert.ToInt32(formCollection["page"])-1,Convert.ToInt32(formCollection["rows"]),formCollection["sidx"],formCollection["sord"],out totalRecords),
                total=0,
                page= Convert.ToInt32(formCollection["page"]),
                records=totalRecords
            };                    
            return Json(jsonData,JsonRequestBehavior.AllowGet);
        }       

        #endregion 

        #region BankAuthrization

        [Audit]
        public ActionResult BankAuthrizationView()
        {
            string ScreenName = "BankAuthrization";
            CommanViewFunction(ScreenName);
            return View();
        }

        [Audit]
        public ActionResult BankAuthrizationDetails(BankAuthrizationViewModel objBankAuthrization)
        {
            ReportFilter objParam = new ReportFilter();

           
            ViewBag.Month = objBankAuthrization.MonthName;
            ViewBag.Year = objBankAuthrization.YearName;

            if (PMGSYSession.Current.LevelId == 5)
            {
                objParam.AdminNdCode =Convert.ToInt32(PMGSYSession.Current.ParentNDCode);
                objParam.LowerAdminNdCode = PMGSYSession.Current.AdminNdCode;
            }
            else
            {
                objParam.AdminNdCode = Convert.ToInt32(objBankAuthrization.State);
                objParam.LowerAdminNdCode = Convert.ToInt32(objBankAuthrization.DPIU);
            }
            objParam.Month = Convert.ToInt16(objBankAuthrization.Month);
            objParam.Year = Convert.ToInt16(objBankAuthrization.Year);

            
            objBankAuthrization = objAccountBAL.GetAuthrizationDetails(objParam);
            if (objBankAuthrization.SRRDAName != null)
            {
                ViewBag.SRRDA = objBankAuthrization.SRRDAName;
            }
            else
            {
                ViewBag.SRRDA = "";
            }
            if (objBankAuthrization.DPIUName != null)
            {
                ViewBag.DPIU = objBankAuthrization.DPIUName;
            }
            else
            {
                ViewBag.DPIU = "";
            }
            return View(objBankAuthrization);
        }

        #endregion BankAuthrization

        #region FunctionForFundTransferAndBankAuthrization

        private void CommanViewFunction(string Name)
        {
            CommonFunctions objCommFunc = new CommonFunctions();
            //Added by Vikram 
            short month = 0;
            short year = 0;
            if (PMGSYSession.Current.AccMonth != 0)
            {
                month = PMGSYSession.Current.AccMonth;
                year = PMGSYSession.Current.AccYear;
            }
            else
            {
                month = Convert.ToInt16(DateTime.Now.Month);
                year = Convert.ToInt16(DateTime.Now.Year);
            }


            if (Name == "AbstractBankAuthorization" || Name == "AbstractFund")
            {       
                ViewBag.Year = objCommFunc.PopulateFinancialYear(true);
            }
            else {
                ViewBag.Year = objCommFunc.PopulateYears(year);
            }
            
            ViewBag.Month = objCommFunc.PopulateMonths(month);
            if (Name != "BankAuthrization")
            {
                ViewBag.FundSelection = objAccountBAL.PopulateFund();
            }

            List<SelectListItem> lstSRRDA = new List<SelectListItem>();

            lstSRRDA = objCommFunc.PopulateNodalAgencies();
            lstSRRDA.Insert(0, new SelectListItem { Text = "Select State", Value = "0" });

            if (PMGSYSession.Current.LevelId == 4)
            {
                var selected = lstSRRDA.Where(m => m.Value == PMGSYSession.Current.AdminNdCode.ToString()).First();
                selected.Selected = true;
            }

            ViewBag.SRRDA = lstSRRDA;

            List<SelectListItem> lstDPIU = new List<SelectListItem>();            
            //Modified by abhishek kamble 4-dec-2013
            //lstDPIU.Insert(0, new SelectListItem { Text = "Select DPIU", Value = "0" });
            lstDPIU.Insert(0, new SelectListItem { Text = "All DPIU", Value = "0" });
            ViewBag.DPIU = lstDPIU;
           // return string.Empty;

            //new change done by Vikram on 10 Jan 2014 for selecting the month and year from session
            ViewBag.SelectedMonth = month;
            ViewBag.SelectedYear = year;

            //end of change


        }
       
        #endregion FunctionForFundTransferAndBankAuthrization

        #region ScheduleRoad

        private void ScheduleView()
        {
            short month = 0;
            short year = 0;
            CommonFunctions objCommanFunc=new CommonFunctions();
            List<SelectListItem> lstFundAgency = objAccountBAL.lstFundingAgency();
            lstFundAgency.Insert(0, new SelectListItem { Text = "All Agency", Value = "0" });
            ViewBag.FumdingAgency = lstFundAgency;
           // ViewBag.Head = objAccountBAL.PopulateHead(28);
            //Added by Vikram 
            if (PMGSYSession.Current.AccMonth != 0)
            {
                month = PMGSYSession.Current.AccMonth;
                year = PMGSYSession.Current.AccYear;
            }
            else 
            {
                month = Convert.ToInt16(DateTime.Now.Month);
                year = Convert.ToInt16(DateTime.Now.Year);
            }
            //end of change


            ViewBag.Month = objCommanFunc.PopulateMonths(month);
            ViewBag.Year = objCommanFunc.PopulateYears(year);
            List<SelectListItem> lstSRRDA = new List<SelectListItem>();
            lstSRRDA= objCommanFunc.PopulateNodalAgencies();
            if (PMGSYSession.Current.LevelId == 4)
            {
                var selected = lstSRRDA.Where(m => m.Value == PMGSYSession.Current.AdminNdCode.ToString()).First();
                selected.Selected = true;
            }
            ViewBag.State = lstSRRDA;
            List<SelectListItem> lstDPIU = new List<SelectListItem>();
            lstDPIU.Insert(0, new SelectListItem { Text = "Select DPIU", Value = "0" });
            ViewBag.DPIU = lstDPIU;
           // return View();
        }

        [Audit]
        public ActionResult ScheduleDetails(ScheduleModel objScheduleModel)
        {
            ReportFilter objParam = new ReportFilter();
            objParam.Head =Convert.ToInt16(objScheduleModel.HeadCode);
            if (PMGSYSession.Current.LevelId == 5)
            {
                objParam.AdminNdCode = Convert.ToInt32(PMGSYSession.Current.ParentNDCode);
                objParam.LowerAdminNdCode = PMGSYSession.Current.AdminNdCode;
                objParam.LevelId = PMGSYSession.Current.LevelId;
            }
            else
            {
                objParam.AdminNdCode = Convert.ToInt32(objScheduleModel.State);
                objParam.LowerAdminNdCode = Convert.ToInt32(objScheduleModel.Piu);
                objParam.LevelId = 4;
            }
            objParam.AgencyCode = objScheduleModel.FundingAgency.ToString();
            objParam.Month = objScheduleModel.Month;
            objParam.Year = objScheduleModel.Year;
            objParam.Selection = objScheduleModel.ScheduleName;
            ViewBag.YearName = objScheduleModel.YearName;
            ViewBag.MonthName = objScheduleModel.MonthName;
            ViewBag.AgencyName = objScheduleModel.AgencyName;
            ViewBag.HeadName = objScheduleModel.HeadName;

            objScheduleModel = objAccountBAL.GetScheduleList(objParam);
            ViewBag.StateName = objScheduleModel.StateName;
            ViewBag.DPIUName = objScheduleModel.PiuName;
            return View("ScheduleDetails",objScheduleModel);
        }

        //public ActionResult GetScheduleDetails(int? page, int? rows, string sidx, string sord)
        //{
        //    String searchParameters = String.Empty;
        //    long totalRecords;
        //    int? adminNdCode = 0;
        //    int? month = 0;
        //    int? year = 0;
        //    Int16? head = 0;
        //    string fundingAgenCode = string.Empty;



        //    if (!string.IsNullOrEmpty(Request.Params["AdminNdCode"]))
        //    {
        //        adminNdCode = Convert.ToInt32(Request.Params["AdminNdCode"]);
        //    }
        //    if (!string.IsNullOrEmpty(Request.Params["Month"]))
        //    {
        //        month = Convert.ToInt32(Request.Params["Month"]);
        //    }

        //    if (!string.IsNullOrEmpty(Request.Params["Year"]))
        //    {
        //        year = Convert.ToInt32(Request.Params["Year"]);
        //    }

        //    if (!string.IsNullOrEmpty(Request.Params["Head"]))
        //    {
        //        head = Convert.ToInt16(Request.Params["Head"]);
        //    }

        //    if (!string.IsNullOrEmpty(Request.Params["Agency"]))
        //    {
        //        fundingAgenCode = Request.Params["Agency"];
        //    }

        //    var jsonData = new
        //    {
        //        rows = objAccountBAL.ListScheduleDetails(adminNdCode,month,year,head,fundingAgenCode,Convert.ToInt32(page) - 1, Convert.ToInt32(rows), sidx, sord, out totalRecords),
        //        total = 0,
        //        page = Convert.ToInt32(page),
        //        records = totalRecords
        //    };

        //    return Json(jsonData);
            
        //}

        [Audit]
        public ActionResult ScheduleNewRoad(BalanceSheet BalanceSheetViewModel)
        {
            ScheduleModel objModel = new ScheduleModel();
            CommonFunctions objCommon = new CommonFunctions();
            int[] headCode = { 28,427 };
            List<SelectListItem> lstHead = new List<SelectListItem>();
            lstHead = objAccountBAL.PopulateHead(headCode);
            lstHead.Insert(0, new SelectListItem { Text = "Select Head", Value = "0" });
            ViewBag.Head = lstHead;
            ViewBag.Schedule = "Schedule of New Road Constructed";
            objModel.ScheduleName = "ScheduleNewRoad";
           
            
            ScheduleView();

            //new change done by Vikram on 06 Jan 2014

            if (PMGSYSession.Current.AccMonth != 0)
            {
                objModel.Month = PMGSYSession.Current.AccMonth;
                objModel.Year = PMGSYSession.Current.AccYear;
            }
            else
            {
                //objModel.Month = Convert.ToInt16(DateTime.Now.Month);                
                //objModel.Year = Convert.ToInt16(DateTime.Now.Year);
                objModel.Month = Convert.ToInt16(BalanceSheetViewModel.SelectedMonth);
                objModel.Year = Convert.ToInt16(BalanceSheetViewModel.SelectedYear);
            }
            //ViewBag.Month = objCommon.PopulateMonths(objModel.Month);
            //ViewBag.Year = objCommon.PopulateYears(objModel.Year);
            //end of change     

            //Added by Abhishek kamble 6-jan-2014 Start            
            if (BalanceSheetViewModel.AdminCode != null)
            {
                objModel.Piu = BalanceSheetViewModel.AdminCode.Value;
            }
            if (BalanceSheetViewModel.StateAdminCode != null)
            {
                objModel.State = BalanceSheetViewModel.StateAdminCode;
            }
            objModel.ReportName = "N";//Schedule of Other Expenditure on Road
            //Added by Abhishek kamble 24-dec-2013  End


            return View("ScheduleView",objModel);
        }

        [Audit]
        public ActionResult ScheduleUpgradedRoad(BalanceSheet BalanceSheetViewModel)
        {
            ScheduleModel objModel = new ScheduleModel();
            int[] headCode = { 29,385,386,428 };
            List<SelectListItem> lstHead = new List<SelectListItem>();
            lstHead = objAccountBAL.PopulateHead(headCode);
            lstHead.Insert(0, new SelectListItem { Text = "Select Head", Value = "0" });
            ViewBag.Head = lstHead;
            ViewBag.Schedule = "Schedule of Upgradation of Existing Road";
            objModel.ScheduleName = "ScheduleUpgradedRoad";
            

            //new change done by Vikram on 06 Jan 2014

            if (PMGSYSession.Current.AccMonth != 0)
            {
                objModel.Month = PMGSYSession.Current.AccMonth;
                objModel.Year = PMGSYSession.Current.AccYear;
            }
            else
            {
                //objModel.Month = Convert.ToInt16(DateTime.Now.Month);
                //objModel.Year = Convert.ToInt16(DateTime.Now.Year);
                objModel.Month = Convert.ToInt16(BalanceSheetViewModel.SelectedMonth);
                objModel.Year = Convert.ToInt16(BalanceSheetViewModel.SelectedYear);
            }

            //end of change
            ScheduleView();

            //Added by Abhishek kamble 6-jan-2014 Start            
            if (BalanceSheetViewModel.AdminCode != null)
            {
                objModel.Piu = BalanceSheetViewModel.AdminCode.Value;
            }
            if (BalanceSheetViewModel.StateAdminCode != null)
            {
                objModel.State = BalanceSheetViewModel.StateAdminCode;
            }
            objModel.ReportName = "U";//Schedule of Other Expenditure on Road
            //Added by Abhishek kamble 24-dec-2013  End
            return View("ScheduleView",objModel);
        }

        [Audit]
        public ActionResult ScheduleOtherExpenditure(BalanceSheet BalanceSheetViewModel)
        {
            ScheduleModel objModel = new ScheduleModel();
            int[] headCode = { 30,31 };
            List<SelectListItem> lstHead = new List<SelectListItem>();
            lstHead = objAccountBAL.PopulateHead(headCode);
            lstHead.Insert(0, new SelectListItem {Text="Select Head",Value="0" });
            ViewBag.Head = lstHead;
            ViewBag.Schedule = "Schedule of Other Expenditure on Roads";
            objModel.ScheduleName = "ScheduleOtherExpenditure";
            ScheduleView();

            //Added by Abhishek kamble 24-dec-2013  Start

            if (BalanceSheetViewModel.Month != 0)
            {
                objModel.Month = BalanceSheetViewModel.Month;
            }
                //new change done by Vikram on 07 Jan 2014
            else if (PMGSYSession.Current.AccMonth != 0)
            {
                objModel.Month = PMGSYSession.Current.AccMonth;
            }
                //end of change
            else
            {
                objModel.Month = Convert.ToInt16(System.DateTime.Now.Month);
            }

            if (BalanceSheetViewModel.Year != 0)
            {
                objModel.Year = BalanceSheetViewModel.Year;
            }
            //new change done by Vikram on 07 Jan 2014
            else if (PMGSYSession.Current.AccYear != 0)
            {
                objModel.Year = PMGSYSession.Current.AccYear;
            }
            //end of change
            else {
                objModel.Year =Convert.ToInt16(System.DateTime.Now.Year);
            }
            if (BalanceSheetViewModel.AdminCode != null)
            {
                objModel.Piu = BalanceSheetViewModel.AdminCode.Value;
            }
            if (BalanceSheetViewModel.StateAdminCode != null)
            {
                objModel.State = BalanceSheetViewModel.StateAdminCode;
            }
            if (BalanceSheetViewModel.HeadName != null)
            {
                foreach (var item in lstHead)
                {
                    if (BalanceSheetViewModel.HeadName.Trim().Equals(item.Text.Trim()))
                    {
                        objModel.HeadCode = Convert.ToInt32(item.Value);
                        //lstHead.Select(m => m.Selected);
                        item.Selected = true;
                        break;
                    }
                }
            }
            else {
                objModel.HeadCode = 0;
            }

            objModel.ReportName = "O";//Schedule of Other Expenditure on Road
            //Added by Abhishek kamble 24-dec-2013  End

            return View("ScheduleView",objModel);
        }
        #endregion ScheduleRoad


        #region Master Sheet

        /// <summary>
        /// Master Sheet for PIU Details
        /// Liabilities & Assets details maintained for each PIU.
        /// </summary>
        /// <returns></returns>
        [Audit]
        public ActionResult MasterSheet()
        {
            MasterSheetViewModel masterSheetModel = new MasterSheetViewModel();
            masterSheetModel.Year = Request.Params["year"] == null ? DateTime.Now.Year - 1 : Convert.ToInt32(Request.Params["year"]);
            masterSheetModel.YEARS_LIST = objAccountDAL.getAllYears();
            return View(masterSheetModel);
          
        }
        [HttpPost]
        [Audit]
        public ActionResult MasterSheetDetails()
        {

            PMGSYEntities dbContext = null;
            try
            {
                MasterSheetViewModel masterSheetModel = new MasterSheetViewModel();

                //Get all Records in List
                masterSheetModel.LIST_MASTER_SHEET = objAccountBAL.MasterSheetBAL(Convert.ToInt32(Request.Params["year"] == null ? masterSheetModel.Year.ToString() : Request.Params["year"]));

                //Prepare Distinct Head Code List & ND Code List with Nd Names
                if (masterSheetModel.LIST_MASTER_SHEET != null)
                {
                    masterSheetModel.LIST_DISTINCT_HEAD_CODE = masterSheetModel.LIST_MASTER_SHEET.Select(c => c.HEAD_CODE).Distinct().ToList<string>();
                    var lstNdNames = (from c in masterSheetModel.LIST_MASTER_SHEET
                                      where c.ADMIN_ND_CODE != 0 && c.MAST_ND_TYPE.Equals("D")                          //for only DPIUs
                                      select new { ADMIN_ND_CODE = c.ADMIN_ND_CODE, ADMIN_ND_NAME = c.ADMIN_ND_NAME }
                                    ).Distinct().ToList();

                    masterSheetModel.LIST_DISTINCT_ND_CODE = new Dictionary<int, string>();
                    foreach (var item in lstNdNames)
                    {
                        masterSheetModel.LIST_DISTINCT_ND_CODE.Add(item.ADMIN_ND_CODE, item.ADMIN_ND_NAME);
                    }
                }


                //Prepare Nodal Departments wise Totals for Liabilities & Assets
                //Also keep Grand Totals of Liabilities & Assets
                decimal liabilitiesTotal = 0;
                decimal assetsTotal = 0;
                masterSheetModel.LiabilitiesGrandTotal = 0;
                masterSheetModel.AssetsGrandTotal = 0;
                masterSheetModel.LiabilitiesTotalWithDepartments = new Dictionary<int, decimal>();
                masterSheetModel.AssetsTotalWithDepartments = new Dictionary<int, decimal>();
                foreach (var distinctNdCodeItem in masterSheetModel.LIST_DISTINCT_ND_CODE)
                {
                    liabilitiesTotal = 0;
                    assetsTotal = 0;
                    foreach (var item in masterSheetModel.LIST_MASTER_SHEET)
                    {
                        if (distinctNdCodeItem.Key == item.ADMIN_ND_CODE && item.MAST_ND_TYPE.Equals("D"))     //Totals for DPIUs only
                        {
                            if (item.CREDIT_DEBIT.Equals("C"))
                            {
                                liabilitiesTotal = liabilitiesTotal + Convert.ToDecimal(item.MONTHLY_BALANCE_AMT);
                                masterSheetModel.LiabilitiesGrandTotal = masterSheetModel.LiabilitiesGrandTotal + Convert.ToDecimal(item.MONTHLY_BALANCE_AMT);
                            }
                            else if (item.CREDIT_DEBIT.Equals("D"))
                            {
                                assetsTotal = assetsTotal + Convert.ToDecimal(item.MONTHLY_BALANCE_AMT);
                                masterSheetModel.AssetsGrandTotal = masterSheetModel.AssetsGrandTotal + Convert.ToDecimal(item.MONTHLY_BALANCE_AMT);
                            }
                        }
                    }

                    masterSheetModel.LiabilitiesTotalWithDepartments.Add(distinctNdCodeItem.Key, liabilitiesTotal);
                    masterSheetModel.AssetsTotalWithDepartments.Add(distinctNdCodeItem.Key, assetsTotal);
                }


                //Prepare Headwise Total of Departments
                decimal headwiseTotal = 0.00M;
                masterSheetModel.SRRDAGrandTotal = 0.00M;
                masterSheetModel.HeadWiseTotalOfDepartments = new Dictionary<string, decimal>();
                foreach (var distinctHeadCodeItem in masterSheetModel.LIST_DISTINCT_HEAD_CODE)
                {
                    headwiseTotal = 0;
                    foreach (var item in masterSheetModel.LIST_MASTER_SHEET)
                    {
                        if (distinctHeadCodeItem.Equals(item.HEAD_CODE) && item.MAST_ND_TYPE.Equals("D"))     //    Totals for DPIUs only
                        {
                            headwiseTotal = headwiseTotal + Convert.ToDecimal(item.MONTHLY_BALANCE_AMT);
                        }
                        else if (distinctHeadCodeItem.Equals(item.HEAD_CODE) && item.MAST_ND_TYPE.Equals("S"))  //  Total for SRRDAs
                        {
                            masterSheetModel.SRRDAGrandTotal = masterSheetModel.SRRDAGrandTotal + Convert.ToDecimal(item.MONTHLY_BALANCE_AMT);
                        }
                    }

                    masterSheetModel.HeadWiseTotalOfDepartments.Add(distinctHeadCodeItem, headwiseTotal);
                }

                //Added By Abhishek kamble 15-nov-2013
                //Set Report Header
                dbContext = new PMGSYEntities();
                CommonFunctions commomFuncObj = new CommonFunctions();

                var rptHeader = dbContext.SP_ACC_Get_Report_Header_Information("AccountsReports", "MasterSheet", PMGSYSession.Current.FundType, PMGSYSession.Current.LevelId, "A").ToList<SP_ACC_Get_Report_Header_Information_Result>().FirstOrDefault();

                if (rptHeader == null)
                {
                    masterSheetModel.ReportFormNumber = String.Empty;
                    masterSheetModel.ReportName = String.Empty;
                    masterSheetModel.ReportParagraphName = String.Empty;
                    masterSheetModel.FundTypeName = String.Empty;
                }
                else
                {
                    masterSheetModel.ReportFormNumber = rptHeader.REPORT_FORM_NO;
                    masterSheetModel.ReportName = rptHeader.REPORT_NAME;
                    masterSheetModel.ReportParagraphName = rptHeader.REPORT_PARAGRAPH_NAME;
                    masterSheetModel.FundTypeName = commomFuncObj.GetFundName(PMGSYSession.Current.FundType);
                }

                return View(masterSheetModel);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return View(new MasterSheetViewModel());
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

        #region Final Bill Payment

        public ActionResult FinalBillPayment()
        {

            FinalBillPaymentModel finalBillPaymentModel = new FinalBillPaymentModel();
            CommonFunctions objCommonFunction=new CommonFunctions();
            IAccountReportsDAL objAccountReportDAL=new AccountReportsDAL();
            List<SelectListItem> lstFundingAgency = new List<SelectListItem>();
            finalBillPaymentModel.lstStates = objCommonFunction.PopulateStates(true);
            finalBillPaymentModel.lstAgency = objAccountReportDAL.PopulateAgency(0);
                                                                                                        
            lstFundingAgency = objCommonFunction.PopulateFundingAgency(true);
            lstFundingAgency.RemoveAt(0);
            lstFundingAgency.Insert(0, new SelectListItem { Text = "All Funding Agency", Value = "0" });
            finalBillPaymentModel.lstFundingAgency = lstFundingAgency;

            if (PMGSYSession.Current.LevelId == 4)
            {
                finalBillPaymentModel.StateCode = PMGSYSession.Current.StateCode;
            }

            return View(finalBillPaymentModel);
        }

        public ActionResult FinalBillPaymentDetails(FinalBillPaymentModel finalBillPaymentModel)
        {
            return PartialView("FinalBillPaymentDetails", objAccountBAL.FinalBillPaymentHeaderInformation(finalBillPaymentModel));
        }

        public ActionResult ListFinalBillPaymentCompletedDetails(FormCollection formCollection)
        {                       

            //Adde By Abhishek kamble 29-Apr-2014 start
            using (CommonFunctions commonFunction = new CommonFunctions())
            {
                if (!commonFunction.ValidateGridParameters(new GridParams(Convert.ToInt32(formCollection["page"]), Convert.ToInt32(formCollection["rows"]), formCollection["sidx"], formCollection["sord"], Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                {
                    return null;
                }
            }
            //Adde By Abhishek kamble 29-Apr-2014 end

            int StateCode= 0;
            int AgencyCode = 0;
            int FundingAgencyCode = 0;
            long totalRecords;

            if (!string.IsNullOrEmpty(Request.Params["State"]))
            {
                StateCode = Convert.ToInt32(Request.Params["State"]);
            }

            if (!string.IsNullOrEmpty(Request.Params["Agency"]))
            {
                AgencyCode = Convert.ToInt32(Request.Params["Agency"]);
            }

            if (!string.IsNullOrEmpty(Request.Params["FundingAgency"]))
            {
                FundingAgencyCode = Convert.ToInt32(Request.Params["FundingAgency"]);
            }

            var jsonData = new
            {
                rows = objAccountBAL.ListFinalBillPaymentCompletedDetails(StateCode,AgencyCode,FundingAgencyCode, Convert.ToInt32(formCollection["page"]) - 1, Convert.ToInt32(formCollection["rows"]), formCollection["sidx"], formCollection["sord"], out totalRecords),
                total = 0,
                page = Convert.ToInt32(formCollection["page"]),
                records = totalRecords
            };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }       

        
        public ActionResult ListFinalBillPaymentPenddingDetails(FormCollection formCollection)
        {
            long totalRecords;

            //Adde By Abhishek kamble 29-Apr-2014 start
            using (CommonFunctions commonFunction = new CommonFunctions())
            {
                if (!commonFunction.ValidateGridParameters(new GridParams(Convert.ToInt32(formCollection["page"]), Convert.ToInt32(formCollection["rows"]), formCollection["sidx"], formCollection["sord"], Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                {
                    return null;
                }
            }
            //Adde By Abhishek kamble 29-Apr-2014 end

            //if (!string.IsNullOrEmpty(Request.Params["StatParameterse"]))
            //{
            //    parameters=Request.Params["Parameters"].Split('$');
            //}
            
            String []parameters=Request.Params["Parameters"].Split('$');

            int stateCode = Convert.ToInt32(parameters[0]);
            int AgencyCode = Convert.ToInt32(parameters[1]);
            int FundingAgencyCode = Convert.ToInt32(parameters[2]);
            int Year = Convert.ToInt32(parameters[3]);
            
            var jsonData = new
            {
                rows = objAccountBAL.ListFinalBillPaymentPendingDetails(stateCode,AgencyCode,FundingAgencyCode,Year, Convert.ToInt32(formCollection["page"]) - 1, Convert.ToInt32(formCollection["rows"]), formCollection["sidx"], formCollection["sord"], out totalRecords),
                total = 0,
                page = Convert.ToInt32(formCollection["page"]),
                records = totalRecords
            };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }       
       
        [HttpPost]
        public JsonResult PopulateAgency(string id)
        {
            if (!int.TryParse(id, out outParam))
            {
                return Json(false);
            }
            IAccountReportsDAL objAccountReportDAL = new AccountReportsDAL();
            int stateCode=Convert.ToInt32(id);

            return Json(objAccountReportDAL.PopulateAgency(stateCode),JsonRequestBehavior.AllowGet);
            
        }

        #endregion Final Bill Payment


        #region State Account Monitoring                 

        public ActionResult StateAccountMonitoring()
        {
            StateAccountMonitoringViewModel stateAccountMonitoringModel = new StateAccountMonitoringViewModel();
            CommonFunctions objCommonFun = new CommonFunctions();

            List<SelectListItem> lstStates = objCommonFun.PopulateStates(false).OrderBy(o => o.Text).ToList();

            lstStates.Insert(0, new SelectListItem { Text = "All State", Value = "0" });        

            stateAccountMonitoringModel.lstStates = lstStates;
            stateAccountMonitoringModel.lstAgency = objAccountDAL.PopulateAgencyByStateCode(0).ToList();
            stateAccountMonitoringModel.lstFundType = objAccountDAL.PopulateFundType();

            stateAccountMonitoringModel.FundType = PMGSYSession.Current.FundType;

            if (PMGSYSession.Current.LevelId == 4)
            {
                stateAccountMonitoringModel.StateCode = PMGSYSession.Current.StateCode;
            }
            return View(stateAccountMonitoringModel);
        }

        [HttpPost]
        public ActionResult StateAccountMonitoringDetails(StateAccountMonitoringViewModel stateAccountMonitoringModel)
        {
            return PartialView("StateAccountMonitoringDetails",objAccountBAL.StateAccMonitoringHeaderInformation(stateAccountMonitoringModel));        
        }

        public ActionResult ListStateAccountMonitoringDetails(FormCollection formCollection)
        {    
            //Adde By Abhishek kamble 29-Apr-2014 start
            using (CommonFunctions commonFunction = new CommonFunctions())
            {
                if (!commonFunction.ValidateGridParameters(new GridParams(Convert.ToInt32(formCollection["page"]), Convert.ToInt32(formCollection["rows"]), formCollection["sidx"], formCollection["sord"], Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                {
                    return null;
                }
            }
            //Adde By Abhishek kamble 29-Apr-2014 end

            long totalRecords;
            int stateCode = 0;
            int agencyCode = 0;
            string fundType = String.Empty;
            if (!string.IsNullOrEmpty(Request.Params["State"]))
            {
                stateCode = Convert.ToInt32(Request.Params["State"]);
            }
            if (!string.IsNullOrEmpty(Request.Params["Agency"]))
            {
                agencyCode = Convert.ToInt32(Request.Params["Agency"]);
            }
            if (!string.IsNullOrEmpty(Request.Params["FundType"]))
            {
                fundType = Request.Params["FundType"];
            }

            var jsonData = new { 
                rows=objAccountBAL.ListStateAccountMonitoringDetails(stateCode,agencyCode,fundType,Convert.ToInt32(formCollection["page"])-1,Convert.ToInt32(formCollection["rows"]),formCollection["sidx"],formCollection["sord"],out totalRecords),
                total=0,
                page=Convert.ToInt32(formCollection["page"]),
                records=totalRecords
            };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }


        public JsonResult PopulateAgencyByStateCode(string id)
        {
            if (!int.TryParse(id, out outParam))
            {
                return Json(false);
            }                                                                                          
            int stateCode = Convert.ToInt32(id);                                                       
            return Json(objAccountDAL.PopulateAgencyByStateCode(stateCode),JsonRequestBehavior.AllowGet);
        }

        #endregion State Account Monitoring
    }
}
