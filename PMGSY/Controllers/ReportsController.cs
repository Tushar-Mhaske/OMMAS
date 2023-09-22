using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PMGSY.BAL.Reports;
using PMGSY.Models;
using PMGSY.Models.Report;
using PMGSY.Common;
using PMGSY.Extensions;
using PMGSY.Models.Report.Ledger;

using PMGSY.Models.Report.ProgramFund;
using PMGSY.Models.Common;
using PMGSY.Models.Report.RegisterOfWorks;
using PMGSY.Models.Report.Account;
using System.Text;

using System.Globalization;
using PMGSY.DAL.Reports;
using System.Text.RegularExpressions;


namespace PMGSY.Controllers
{
    [RequiredAuthentication]
    [RequiredAuthorization]
    public partial class ReportsController : Controller
    {
        private IReportBAL objReportBAL = new ReportBAL();
        private CommonFunctions commomFuncObj = new CommonFunctions();


        public ReportsController()
        {
            PMGSYSession.Current.ModuleName = "Reports";
        }


        public ActionResult Index()
        {
            return View();
        }

        #region Cashbook

        //[Audit]
        public ActionResult CashBook()
        {
            CBSingleModel cbSingle = new CBSingleModel();

            //change done by Vikram on 01-Jan-2014
            if (PMGSYSession.Current.AccMonth != 0)
            {
                cbSingle.Month = PMGSYSession.Current.AccMonth;
                cbSingle.Year = PMGSYSession.Current.AccYear;
            }
            else
            {
                //end of change
                cbSingle.Month = Convert.ToInt16(DateTime.Now.Month);
                cbSingle.Year = Convert.ToInt16(DateTime.Now.Year);
            }
            ViewBag.ddlMonth = commomFuncObj.PopulateMonths(DateTime.Now.Month);
            ViewBag.ddlYear = commomFuncObj.PopulateYears(DateTime.Now.Year);

            //Added by abhishek kamble 3-oct-2013 start 
            if (PMGSYSession.Current.LevelId == 4 || PMGSYSession.Current.LevelId == 6)
            {
                //populate SRRDA
                List<SelectListItem> lstSRRDA = new List<SelectListItem>();
                lstSRRDA = commomFuncObj.PopulateNodalAgencies();
                ViewBag.SRRDA = lstSRRDA;

                //Populate DPIU
                List<SelectListItem> lstDPIU = new List<SelectListItem>();
                 lstDPIU.Insert(0, new SelectListItem { Text = "Select DPIU", Value = "0" });
                //lstDPIU.Insert(0, new SelectListItem { Text = "All DPIU", Value = "0", Selected = true });
               // lstDPIU.Insert(0, new SelectListItem { Text = "Select DPIU", Value = "-1" });
                ViewBag.DPIU = lstDPIU;
            }
            //Added by abhishek kamble 3-oct-2013 end 
            return View("CashBook", cbSingle);
        }

        [HttpPost]
        [Audit]
        public ActionResult CashBook(CBSingleModel cbSingle)
        {
            //Added by abhishek kamble 3-oct-2013 start 
            string SRRDA_DPIU = cbSingle.SRRDA_DPIU;
            int DPIU_NdCode = cbSingle.DPIU;
            //string 

            if (PMGSYSession.Current.LevelId == 4 || PMGSYSession.Current.LevelId == 6)
            {
                //populate SRRDA
                CommonFunctions objCommonFunction = new CommonFunctions();
                List<SelectListItem> lstSRRDA = new List<SelectListItem>();
                lstSRRDA = objCommonFunction.PopulateNodalAgencies();
                ViewBag.SRRDA = lstSRRDA;
               
                //Populate DPIU
                TransactionParams objTransParam = new TransactionParams();

                int AdminNdCode = cbSingle.SRRDA;
                objTransParam.ADMIN_ND_CODE = AdminNdCode;

                List<SelectListItem> lstDPIU = commomFuncObj.PopulateDPIU(objTransParam);
                lstDPIU.RemoveAt(0);
                lstDPIU.Insert(0, new SelectListItem { Text = "Select DPIU", Value = "0" });
                var selectedDPIU = lstDPIU.Where(m => m.Value == cbSingle.DPIU.ToString()).First();
                selectedDPIU.Selected = true;
                ViewBag.DPIU = lstDPIU;
            }
            //added by Abhishek kamble 3-oct-2013 end

            ReportFilter objParam = new ReportFilter();
            objParam.Month = cbSingle.Month;
            objParam.Year = cbSingle.Year;
            objParam.Selection = cbSingle.SRRDA_DPIU;
            objParam.Dpiu = cbSingle.DPIU;

            ViewBag.ddlMonth = commomFuncObj.PopulateMonths(objParam.Month);
            ViewBag.ddlYear = commomFuncObj.PopulateYears(objParam.Year);
            
            //added by Abhishek kamble 3-oct-2013 start

            //Login Mord/SRRDA
            if (PMGSYSession.Current.LevelId == 4 || PMGSYSession.Current.LevelId == 6)
            {
                if (cbSingle.SRRDA_DPIU == "S")
                {
                    objParam.AdminNdCode = cbSingle.SRRDA;
                }
                else if(cbSingle.SRRDA_DPIU == "D") {
                        objParam.AdminNdCode = cbSingle.DPIU;
                }
            }
            else { //login DPIU                                         
                objParam.AdminNdCode = PMGSYSession.Current.AdminNdCode;            
            }

            //added by Abhishek kamble 3-oct-2013 end
            
            objParam.FundType = PMGSYSession.Current.FundType;
            cbSingle = objReportBAL.GetSingleCB(objParam);

            cbSingle.SRRDA_DPIU = SRRDA_DPIU;

            if ((SRRDA_DPIU == "D" || SRRDA_DPIU == "S") && (DPIU_NdCode == 0))
            {
                cbSingle.DistrictDepartment = "-";
            }
            else
            {
                cbSingle.DPIU = DPIU_NdCode;
            }
            return View("CashBook", cbSingle);
        }

        [Audit]
        public ActionResult SingleCashBook(String id1, String id2)
        {
            return PartialView("");
        }

        [Audit]
        public ActionResult ReceiptCashBook(String id1, String id2)
        {
            CBReceiptModel cbReceiptModel = new CBReceiptModel();
            ReportFilter objParam = new ReportFilter();
            objParam.Month = Convert.ToInt16(id1);
            objParam.Year = Convert.ToInt16(id2);
            objParam.AdminNdCode = PMGSYSession.Current.AdminNdCode;
            objParam.FundType = PMGSYSession.Current.FundType;
            objParam.LevelId = PMGSYSession.Current.LevelId;

            cbReceiptModel = objReportBAL.ReceiptCashBook(objParam);
            //ReceiptCashBook(ReportFilter objParam);            
            return PartialView("CBReceipt", cbReceiptModel);
        }

        [Audit]
        public ActionResult PaymentCashBook(String id1, String id2)
        {
            CBPaymentModel cbPaymentModel = new CBPaymentModel();
            ReportFilter objParam = new ReportFilter();
            objParam.Month = Convert.ToInt16(id1);
            objParam.Year = Convert.ToInt16(id2);
            objParam.AdminNdCode = PMGSYSession.Current.AdminNdCode;
            objParam.FundType = PMGSYSession.Current.FundType;
            objParam.LevelId = PMGSYSession.Current.LevelId;

            cbPaymentModel = objReportBAL.PaymentCashBook(objParam);
            return PartialView("CBPayment", cbPaymentModel);
        }


        //Populate DPIU
        [Audit]
        public JsonResult PopulateDPIUForCashBook(string id)
        {
            try
            {
                TransactionParams objParam = new TransactionParams();
                CommonFunctions objCommonFunction = new CommonFunctions();

                int AdminNdCode = Convert.ToInt32(id);
                objParam.ADMIN_ND_CODE = AdminNdCode;


                if (AdminNdCode == 0)
                {
                    List<SelectListItem> lstDpiu = new List<SelectListItem>();
                    lstDpiu.Insert(0, new SelectListItem { Text = "Select DPIU", Value = "0" });
                    return Json(lstDpiu, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    List<SelectListItem> lstDPIU = objCommonFunction.PopulateDPIU(objParam);
                    lstDPIU.RemoveAt(0);
                    lstDPIU.Insert(0, new SelectListItem { Text = "Select DPIU", Value = "0" });
                    //lstDPIU.Insert(0, new SelectListItem { Text = "All DPIU", Value = "0", Selected = true });
                    //lstDPIU.Insert(0, new SelectListItem { Text = "Select DPIU", Value = "-1" });

                    return Json(new SelectList(lstDPIU, "Value", "Text"), JsonRequestBehavior.AllowGet);
                }
            }
            catch
            {
                return Json(false);
            }
        }


        #endregion cashbook
        
        #region Contractor Ledger
        [Audit]
        public ActionResult ContrctorLedger()
        {

            int adminNDCode = PMGSYSession.Current.AdminNdCode;
            ContractorLedger contractorLedger = new ContractorLedger();
            contractorLedger.FundType = PMGSYSession.Current.FundType;
            contractorLedger.LevelId = PMGSYSession.Current.LevelId;
            if (contractorLedger.LevelId == 5 && PMGSYSession.Current.ParentNDCode.HasValue)
            {

                contractorLedger.SrrdaNdCode = Convert.ToInt32(PMGSYSession.Current.ParentNDCode);
                contractorLedger.PiuNdCode = PMGSYSession.Current.AdminNdCode;

            }
            else
            {
                contractorLedger.SrrdaNdCode = Convert.ToInt32(PMGSYSession.Current.AdminNdCode);
                contractorLedger.PiuList = commomFuncObj.PopulateDPIUOfSRRDA(contractorLedger.SrrdaNdCode);
                contractorLedger.PiuList.Insert(0, new SelectListItem { Text = "Select DPIU", Value = "0", Selected = true });
            }


            return View(contractorLedger);
        }
        #endregion

        #region ledger

        /// <summary>
        /// action to return the ledger page
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Audit]
        public ActionResult Ledger(LedgerModel ledgerModel)
        {
            try
            {
                //Added By Abhishek kamble 3-jan-2013

                if (ledgerModel.HEAD != null)
                {
                    ledgerModel.SelectedHead =Convert.ToInt32(ledgerModel.HEAD);
                }

                //Commented by Abhishek kamble 3-jan-2014
                //LedgerModel ledgerModel = new LedgerModel();

                //change done by Vikram on 01-Jan-2014
                if (PMGSYSession.Current.AccMonth != 0)
                {
                    ledgerModel.MONTH_LIST = commomFuncObj.PopulateMonths(PMGSYSession.Current.AccMonth);
                    ledgerModel.YEAR_LIST = commomFuncObj.PopulateYears(PMGSYSession.Current.AccYear);
                    ledgerModel.MONTH = PMGSYSession.Current.AccMonth;
                    ledgerModel.YEAR = PMGSYSession.Current.AccYear;
                }
                else
                {
                    //end of change
                    ledgerModel.MONTH_LIST = commomFuncObj.PopulateMonths(DateTime.Now.Month);
                    ledgerModel.YEAR_LIST = commomFuncObj.PopulateYears(DateTime.Now.Year);

                    //Commented by Abhishek kamble 3-jan-2014
                    //ledgerModel.MONTH = Convert.ToInt16(DateTime.Now.Month);
                    //ledgerModel.YEAR = Convert.ToInt16(DateTime.Now.Year);
                }
                ledgerModel.PIU_LIST = commomFuncObj.PopulateDPIUOfSRRDA(PMGSYSession.Current.AdminNdCode);
                ledgerModel.PIU_LIST.Insert(0, (new SelectListItem { Text = "All DPIU", Value = "0", Selected = true }));

                //Added by abhishek kamble 4-oct-2013 start 

                List<SelectListItem>lstDPIU=new List<SelectListItem>();
                lstDPIU.Insert(0,new SelectListItem{Text="Select DPIU",Value="0"});
                ViewBag.DPIULevel = lstDPIU;

                if (PMGSYSession.Current.LevelId == 4 || PMGSYSession.Current.LevelId == 6)
                {
                    //populate SRRDA
                    List<SelectListItem> lstSRRDA = new List<SelectListItem>();
                    lstSRRDA = commomFuncObj.PopulateNodalAgencies();
                    ViewBag.SRRDA = lstSRRDA;
                }

                List<short> opLevel = new List<short>();
                String SRRDA_DPIU = null;
                // Modified by Abhishek kamble 4-oct-2013
                if (PMGSYSession.Current.LevelId == 5)
                {
                    opLevel.Add(1);
                    opLevel.Add(2);
                    SRRDA_DPIU = "D";
                }
                else if (PMGSYSession.Current.LevelId == 4 || PMGSYSession.Current.LevelId == 6)
                {
                    opLevel.Add(2);
                    opLevel.Add(3);
                    SRRDA_DPIU = "S";
                }
                ledgerModel.HEAD_LIST = objReportBAL.GetLedgerHeadList("C", PMGSYSession.Current.FundType, opLevel,SRRDA_DPIU);

                // Modified by Abhishek kamble 7-oct-2013

                ledgerModel.SRRDA = PMGSYSession.Current.AdminNdCode;

                if (PMGSYSession.Current.LevelId == 6)
                {
                    ledgerModel.levelId = 4;//SRRDA
                }
                else {
                    ledgerModel.levelId = PMGSYSession.Current.LevelId;
                }
                return View("CreditDebitLedger", ledgerModel);
            }
            catch (Exception ex)
            {
                throw new Exception("error occured");
            }
        }

        //action to get the list of head as per credit /debit
        [Audit]
        public JsonResult GetLedgerHeadList(String id)
        {
            List<short> opLevel = new List<short>();
            try
            {
                //modified by abhishek kamble 7-oct-2013
                String[] parameters;
                String SRRDA_DPIU=null;
                if (PMGSYSession.Current.LevelId == 4 || PMGSYSession.Current.LevelId ==6)
                {
                    parameters = id.Split('$');
                    id = parameters[0];
                    SRRDA_DPIU = parameters[1];      
                }

                if (id == "C" || id == "D")
                {
                    // Modified by Abhishek kamble 7-oct-2013
                    if (PMGSYSession.Current.LevelId == 5)
                    {      //forDPIU
                        opLevel.Add(1);
                        opLevel.Add(2);
                    }
                    else if (SRRDA_DPIU=="S")
                    {    //For SRRDA                       
                        opLevel.Add(2);
                        opLevel.Add(3);
                    }
                    else if (SRRDA_DPIU == "D")
                    {   //forDPIU
                        opLevel.Add(1);
                        opLevel.Add(2);                    
                    }
                    return Json(objReportBAL.GetLedgerHeadList(id, PMGSYSession.Current.FundType, opLevel,SRRDA_DPIU ));
                }
                else
                {
                    throw new Exception("Invalid parameter");
                }
            }
            catch (Exception ex)
            {
                throw new Exception("error while getting head list");
            }
        }

        /// <summary>
        /// action to return the actionresult 
        /// </summary>
        /// <param name="id">month ,year and credit or debit</param>
        /// <returns></returns>                           
        [Audit]
        public ActionResult GetCreditDebitLedger(String id)
        {
            try
            {
                if (!String.IsNullOrEmpty(id))
                {
                    string[] parameters = id.Split('$');
                    ledgerListModel lstObject = new ledgerListModel();
                    List<LedgerAmountModel> creditDebitModel = new List<LedgerAmountModel>();
                    ReportFilter objParam = new ReportFilter();
                    objParam.Month = Convert.ToInt16(parameters[0]);
                    objParam.Year = Convert.ToInt16(parameters[1]);
                    objParam.CreditDebit = parameters[2];
                    objParam.Head = Convert.ToInt16(parameters[3]);
                    objParam.LowerAdminNdCode = Convert.ToInt16(parameters[4]);
                    objParam.RoadStatus = Convert.ToChar(parameters[5]);

                    //Modified by abhishek kamble 7-oct-2013 start 
                    int AdminNdCode = 0;
                    short LevelId = 0;
                    string SRRDA_DPIU = null;
                    lstObject.MonthName = parameters[8];
                    lstObject.Year = parameters[1];
                    if (PMGSYSession.Current.LevelId == 4||PMGSYSession.Current.LevelId == 6)
                    {
                        AdminNdCode = Convert.ToInt32(parameters[6]);
                        SRRDA_DPIU = parameters[7];

                        if (SRRDA_DPIU == "S")
                        {
                            LevelId = 4;
                        }
                        else if(SRRDA_DPIU=="D"){
                            LevelId = 5;
                        }
                    }
                    else {
                        AdminNdCode = PMGSYSession.Current.AdminNdCode;
                        LevelId = PMGSYSession.Current.LevelId;
                    }
                    objParam.AdminNdCode = AdminNdCode;
                    objParam.FundType = PMGSYSession.Current.FundType;
                    objParam.LevelId = LevelId;
                    objParam.Selection = SRRDA_DPIU;

                    //Modified by abhishek kamble 7-oct-2013 end 
                    creditDebitModel = objReportBAL.GetCreditDebitModel(objParam);
                    lstObject.ListLedger = creditDebitModel;
                    lstObject.OPENING_BALANCE = creditDebitModel.Where(x => x.OPENING_BALANCE != String.Empty).First().OPENING_BALANCE;
                    lstObject.CR_DR = objParam.CreditDebit;

                    if (creditDebitModel.Where(m => m.ReportNumber !=null).Any())
                    {
                        lstObject.ReportNumber = creditDebitModel.Where(m => m.ReportNumber != null).FirstOrDefault().ReportNumber;
                    }
                    if (creditDebitModel.Where(m => m.ReportName != null).Any())
                    {
                        lstObject.ReportName = creditDebitModel.Where(m => m.ReportName != null).FirstOrDefault().ReportName;
                    }
                    if (creditDebitModel.Where(m => m.ReporPara != null).Any())
                    {
                        lstObject.ReporPara = creditDebitModel.Where(m => m.ReporPara != null).FirstOrDefault().ReporPara;
                    }
                    if (creditDebitModel.Where(m => m.FundType != null).Any())
                    {
                        lstObject.FundType = creditDebitModel.Where(m => m.FundType != null).FirstOrDefault().FundType;
                    }
                    if (objParam.LowerAdminNdCode != -1 && objParam.LowerAdminNdCode!=0) //Modified By Ashish Markande 18/10/2013
                    {
                        lstObject.DPIUName = creditDebitModel.Where(m => m.DPIUName != null).FirstOrDefault().DPIUName;
                    }
                    LedgerModel info = objReportBAL.GetForContextData(objParam);
                    lstObject.StateDepartment = info.StateDepartment;
                    lstObject.DistrictDepartment = info.DistrictDepartment;
                    
                    //added by abhishek kamble 9-dec-2013
                    lstObject.currentMonthName = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(objParam.Month);
                    lstObject.previousMonthName = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(objParam.Month ==1?12:(objParam.Month - 1));
                    
                    return View("CreditDebitDataView", lstObject);
                }
                else
                {
                    throw new Exception("Error While geting credit debit ledger.");

                }

            }
            catch (Exception ex)
            {
                string errorMsg = string.Format("Errors: {0}", ex.Message);
                Response.TrySkipIisCustomErrors = true;
                Response.StatusCode = 500;
                Response.Write(errorMsg);
                return Json(string.Empty);
            }

        }

        //Populate DPIU
        [Audit]
        public JsonResult PopulateDPIUOfSRRDA(string id)
        {
            try
            {   
                CommonFunctions objCommonFunction = new CommonFunctions();
                int AdminNdCode = Convert.ToInt32(id);
                List<SelectListItem> lstDPIU = objCommonFunction.PopulateDPIUOfSRRDA(AdminNdCode);
              //  lstDPIU.RemoveAt(0);
               lstDPIU.Insert(0, new SelectListItem { Text = "All DPIU", Value = "0" });           
                return Json(new SelectList(lstDPIU, "Value", "Text"), JsonRequestBehavior.AllowGet);                
            }
            catch
            {
                return Json(false);
            }
        }


        #endregion

        #region MonthlyAccount

        [Audit]
        public ActionResult MonthlyAccount()
        {
            MonthlyAccountModel monthlyAccountModel = new MonthlyAccountModel();

            //Added By Abhishek kamble 30-jan-2013 start            
            Int16 Month=0;
            Int16 Year = 0;
            Int16 IsStateSelected = 0;
            Int16 ADMIN_ND_CODE = 0;
            Int16 PARENT_ND_CODE = 0;

            if (!String.IsNullOrEmpty(Request.Params["Month"]))
            {
                Month = Convert.ToInt16(Request.Params["Month"]);
            }
            if (!String.IsNullOrEmpty(Request.Params["Month"]))
            {
                Year = Convert.ToInt16(Request.Params["Year"]);
            }
            if (!String.IsNullOrEmpty(Request.Params["IsStateSelected"]))
            {
                IsStateSelected = Convert.ToInt16(Request.Params["IsStateSelected"]);
            }
            if (!String.IsNullOrEmpty(Request.Params["ADMIN_ND_CODE"]))
            {
                ADMIN_ND_CODE = Convert.ToInt16(Request.Params["ADMIN_ND_CODE"]);
            }
            if (!String.IsNullOrEmpty(Request.Params["PARENT_ND_CODE"]))
            {
                PARENT_ND_CODE = Convert.ToInt16(Request.Params["PARENT_ND_CODE"]);
            }

            if (ADMIN_ND_CODE>0)
            {
                 if (IsStateSelected == 1)
                 {
                     monthlyAccountModel.monthlyStateSrrdaDpiu = "DPIU";
                     monthlyAccountModel.Dpiu = ADMIN_ND_CODE;
                     monthlyAccountModel.State = PARENT_ND_CODE;
                 }
                 else {
                     monthlyAccountModel.State = ADMIN_ND_CODE;
                 }
            }
            //Added By Abhishek kamble 30-jan-2013 end





            //Added by Vikram on 07 Jan 2014

            if (Month != 0)
            {
                monthlyAccountModel.Month = Month;
            }
            else
            {

                if (PMGSYSession.Current.AccMonth != 0)
                {
                    monthlyAccountModel.Month = PMGSYSession.Current.AccMonth;
                }
                else
                {
                    monthlyAccountModel.Month = Convert.ToInt16(DateTime.Now.Month);
                }
            }

            if (Year != 0)
            {
                monthlyAccountModel.Year = Year;
            }
            else
            {
                if (PMGSYSession.Current.AccYear != 0)
                {
                    monthlyAccountModel.Year = PMGSYSession.Current.AccYear;
                }
                else
                {
                    monthlyAccountModel.Year = Convert.ToInt16(DateTime.Now.Year);
                }
            }

            //end of change
            ViewBag.ddlMonth = commomFuncObj.PopulateMonths(DateTime.Now.Month);
            ViewBag.ddlYear = commomFuncObj.PopulateYears(DateTime.Now.Year);

            //populate State SRRDA                          
            IReportDAL objReportDAL = new ReportDAL();
            ViewBag.ddlState = objReportDAL.PopulateStateSRRDA();

            if (PMGSYSession.Current.LevelId == 4)
            {
                monthlyAccountModel.State = Convert.ToInt16(PMGSYSession.Current.AdminNdCode);
            }

            //populate DPIU
            List<SelectListItem> lstDpiu = new List<SelectListItem>();
            lstDpiu.Insert(0, (new SelectListItem { Text = "Select DPIU", Value = "0" }));
            ViewBag.ddlDPIU = lstDpiu;

            List<SelectListItem> lstCreditDebit = new List<SelectListItem>();
            lstCreditDebit.Insert(0, (new SelectListItem { Text = "Select Type", Value = "0" }));
            lstCreditDebit.Insert(1, (new SelectListItem { Text = "Credit", Value = "C" }));
            lstCreditDebit.Insert(2, (new SelectListItem { Text = "Debit", Value = "D" }));
            ViewBag.ddlCreditDebit = lstCreditDebit;
            ViewBag.Level = PMGSYSession.Current.LevelId == 4 ? "State" : "District";
            return View(monthlyAccountModel);
        }

        //Populate DPIU
        [Audit]
        public JsonResult PopulateDPIU(string id)
        {
            try
            {
                TransactionParams objParam = new TransactionParams();
                CommonFunctions objCommonFunction = new CommonFunctions();

                int AdminNdCode = Convert.ToInt32(id);
                objParam.ADMIN_ND_CODE = AdminNdCode;


                if (AdminNdCode == 0)
                {
                    List<SelectListItem> lstDpiu = new List<SelectListItem>();
                    lstDpiu.Insert(0, new SelectListItem { Text = "All DPIU", Value = "0" });
                    return Json(lstDpiu, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    List<SelectListItem> lstDPIU = objCommonFunction.PopulateDPIU(objParam);
                    lstDPIU.RemoveAt(0);
                    lstDPIU.Insert(0, new SelectListItem { Text = "All DPIU", Value = "0" });

                    return Json(new SelectList(lstDPIU, "Value", "Text"), JsonRequestBehavior.AllowGet);
                }
            }
            catch
            {
                return Json(false);
            }
        }

        [HttpPost]
        [Audit]
        [ValidateAntiForgeryToken]
        public ActionResult MonthlyAccount(MonthlyAccountModel monthlyAccountModel)
        {
            PMGSYEntities dbcontext = new PMGSYEntities();
            try
            {
                MonthlyAccountModel objMonthlyAcconutModel = new MonthlyAccountModel();
                ViewBag.ddlMonth = commomFuncObj.PopulateMonths(monthlyAccountModel.Month);
                ViewBag.ddlYear = commomFuncObj.PopulateYears(monthlyAccountModel.Year);
                ViewBag.Level = PMGSYSession.Current.LevelId == 4 ? "State" : "District";


                //Added By Abhishek 10-9-2013
                int ReportlevelID = 0;
                string ReportType = string.Empty;

                //populate State SRRDA                   
                monthlyAccountModel.lstMonthlyAccountSelf = null;
                monthlyAccountModel.lstMonthlyAccountAllPIU = null;

                IReportDAL objReportDAL = new ReportDAL();
                ViewBag.ddlState = objReportDAL.PopulateStateSRRDA();
                monthlyAccountModel.State = monthlyAccountModel.State;

                //populate DPIU                
                List<SelectListItem> lstDpiu = new List<SelectListItem>();

                TransactionParams objTranParam = new TransactionParams();
                CommonFunctions objCommonFunction = new CommonFunctions();

                int AdminNdCode = monthlyAccountModel.State;
                objTranParam.ADMIN_ND_CODE = AdminNdCode;

                if (AdminNdCode == 0)
                {
                    lstDpiu = new List<SelectListItem>();
                    lstDpiu.Insert(0, new SelectListItem { Text = "All DPIU", Value = "0" });
                    ViewBag.ddlDPIU = lstDpiu;
                }
                else
                {
                    List<SelectListItem> lstDPIU = objCommonFunction.PopulateDPIU(objTranParam);
                    lstDPIU.RemoveAt(0);
                    lstDPIU.Insert(0, new SelectListItem { Text = "All DPIU", Value = "0" });

                    ViewBag.ddlDPIU = new SelectList(lstDPIU, "Value", "Text", monthlyAccountModel.Dpiu);
                }

                ReportFilter objParam = new ReportFilter();

                //objParam.AdminNdCode = PMGSYSession.Current.AdminNdCode;

                //DPIU login
                if (PMGSYSession.Current.LevelId == 5)
                {
                    objParam.AdminNdCode = PMGSYSession.Current.AdminNdCode;
                } //SRRDA login
                else if (PMGSYSession.Current.LevelId == 4)
                {
                    if (monthlyAccountModel.monthlyStateSrrdaDpiu == "SRRDA" || monthlyAccountModel.monthlyStateSrrdaDpiu == "STATE")
                    {
                        objParam.AdminNdCode = monthlyAccountModel.State;
                    }
                    else if (monthlyAccountModel.monthlyStateSrrdaDpiu == "DPIU")
                    {
                        if (monthlyAccountModel.Dpiu == 0)
                        {
                            objParam.AdminNdCode = monthlyAccountModel.State;
                        }
                        else
                        {
                            objParam.AdminNdCode = monthlyAccountModel.Dpiu;
                        }
                    }
                } //MORD
                else if (PMGSYSession.Current.LevelId == 6)
                {

                    if (monthlyAccountModel.monthlyStateSrrdaDpiu == "STATE" || monthlyAccountModel.monthlyStateSrrdaDpiu == "SRRDA")
                    {
                        objParam.AdminNdCode = monthlyAccountModel.State;
                    }
                    else if (monthlyAccountModel.monthlyStateSrrdaDpiu == "DPIU")
                    {

                        if (monthlyAccountModel.Dpiu == 0)
                        {
                            objParam.AdminNdCode = monthlyAccountModel.State;
                        }
                        else
                        {
                            objParam.AdminNdCode = monthlyAccountModel.Dpiu;
                        }
                    }
                }

                objParam.CreditDebit = monthlyAccountModel.CreditDebit;
                objParam.FundType = PMGSYSession.Current.FundType;
                objParam.Month = monthlyAccountModel.Month;
                objParam.Year = monthlyAccountModel.Year;
                objParam.State = monthlyAccountModel.State;
                objParam.Dpiu = monthlyAccountModel.Dpiu;
                objParam.monthlyStateSrrdaDpiu = monthlyAccountModel.monthlyStateSrrdaDpiu;


                //cal USP_RPT_SHOW_MONTHLY_ACCOUNT_SELF_Result   

                //DPIU login
                if (PMGSYSession.Current.LevelId == 5)
                {
                    ReportlevelID = 5;
                    ReportType = "A";
                    //cal USP_RPT_SHOW_MONTHLY_ACCOUNT_SELF_Result
                    monthlyAccountModel.lstMonthlyAccountSelf = objReportBAL.GetMonthlyAccountList(objParam);
                    monthlyAccountModel.TotalOpeningAmount = monthlyAccountModel.lstMonthlyAccountSelf.Sum(m => m.OB_AMT);
                    monthlyAccountModel.TotalCreditDebit = monthlyAccountModel.lstMonthlyAccountSelf.Sum(m => m.MONTHLY_AMT);

                } //SRRDA Login
                else if (PMGSYSession.Current.LevelId == 4)
                {
                    if (monthlyAccountModel.monthlyStateSrrdaDpiu == "STATE")
                    {
                        ReportlevelID = 6;
                        ReportType = "S";
                        //cal USP_RPT_SHOW_MONTHLY_ACCOUNT_SELF_Result
                        monthlyAccountModel.lstMonthlyAccountSelf = objReportBAL.GetMonthlyAccountList(objParam);
                        monthlyAccountModel.TotalOpeningAmount = monthlyAccountModel.lstMonthlyAccountSelf.Sum(m => m.OB_AMT);
                        monthlyAccountModel.TotalCreditDebit = monthlyAccountModel.lstMonthlyAccountSelf.Sum(m => m.MONTHLY_AMT);

                        //cal USP_RPT_SHOW_MONTHLY_ACCOUNT_ALLPIU_Result
                        monthlyAccountModel.lstMonthlyAccountAllPIU = objReportBAL.GetMonthlyAccountForAllPIU(objParam);
                        monthlyAccountModel.TotalOpeningAmountForPIU = monthlyAccountModel.lstMonthlyAccountAllPIU.Sum(m => m.OB_AMT);
                        monthlyAccountModel.TotalCreditDebitAmountForPIU = monthlyAccountModel.lstMonthlyAccountAllPIU.Sum(m => m.MONTHLY_AMT);




                    }
                    else if (monthlyAccountModel.monthlyStateSrrdaDpiu == "SRRDA")
                    {
                        ReportlevelID = 4;
                        ReportType = "O";
                        //cal USP_RPT_SHOW_MONTHLY_ACCOUNT_SELF_Result
                        monthlyAccountModel.lstMonthlyAccountSelf = objReportBAL.GetMonthlyAccountList(objParam);
                        monthlyAccountModel.TotalOpeningAmount = monthlyAccountModel.lstMonthlyAccountSelf.Sum(m => m.OB_AMT);
                        monthlyAccountModel.TotalCreditDebit = monthlyAccountModel.lstMonthlyAccountSelf.Sum(m => m.MONTHLY_AMT);
                    }
                    else if (monthlyAccountModel.monthlyStateSrrdaDpiu == "DPIU")
                    {
                        ReportlevelID = 5;
                        ReportType = "A";
                        //cal USP_RPT_SHOW_MONTHLY_ACCOUNT_ALLPIU_Result

                        if (monthlyAccountModel.Dpiu == 0)
                        {
                            monthlyAccountModel.lstMonthlyAccountAllPIU = objReportBAL.GetMonthlyAccountForAllPIU(objParam);
                            monthlyAccountModel.TotalOpeningAmountForPIU = monthlyAccountModel.lstMonthlyAccountAllPIU.Sum(m => m.OB_AMT);
                            monthlyAccountModel.TotalCreditDebitAmountForPIU = monthlyAccountModel.lstMonthlyAccountAllPIU.Sum(m => m.MONTHLY_AMT);
                        }
                        else
                        {
                            monthlyAccountModel.lstMonthlyAccountSelf = objReportBAL.GetMonthlyAccountList(objParam);
                            monthlyAccountModel.TotalOpeningAmount = monthlyAccountModel.lstMonthlyAccountSelf.Sum(m => m.OB_AMT);
                            monthlyAccountModel.TotalCreditDebit = monthlyAccountModel.lstMonthlyAccountSelf.Sum(m => m.MONTHLY_AMT);
                        }

                    }
                }
                else if (PMGSYSession.Current.LevelId == 6)
                {
                    ReportlevelID = 6;
                    if (monthlyAccountModel.monthlyStateSrrdaDpiu == "STATE")
                    {
                        // ReportlevelID = 6;
                        ReportType = "S";
                        //cal USP_RPT_SHOW_MONTHLY_ACCOUNT_SELF_Result
                        monthlyAccountModel.lstMonthlyAccountSelf = objReportBAL.GetMonthlyAccountList(objParam);
                        monthlyAccountModel.TotalOpeningAmount = monthlyAccountModel.lstMonthlyAccountSelf.Sum(m => m.OB_AMT);
                        monthlyAccountModel.TotalCreditDebit = monthlyAccountModel.lstMonthlyAccountSelf.Sum(m => m.MONTHLY_AMT);

                        //cal USP_RPT_SHOW_MONTHLY_ACCOUNT_ALLPIU_Result
                        monthlyAccountModel.lstMonthlyAccountAllPIU = objReportBAL.GetMonthlyAccountForAllPIU(objParam);
                        monthlyAccountModel.TotalOpeningAmountForPIU = monthlyAccountModel.lstMonthlyAccountAllPIU.Sum(m => m.OB_AMT);
                        monthlyAccountModel.TotalCreditDebitAmountForPIU = monthlyAccountModel.lstMonthlyAccountAllPIU.Sum(m => m.MONTHLY_AMT);

                        //state name
                        monthlyAccountModel.StateName = dbcontext.MASTER_STATE.Where(m => m.MAST_STATE_CODE == dbcontext.ADMIN_DEPARTMENT.Where(a => a.ADMIN_ND_CODE == AdminNdCode).Select(s => s.MAST_STATE_CODE).FirstOrDefault()).Select(b => b.MAST_STATE_NAME).FirstOrDefault();

                    }
                    else if (monthlyAccountModel.monthlyStateSrrdaDpiu == "SRRDA")
                    {
                        // ReportlevelID = 4;
                        ReportType = "O";

                        //cal USP_RPT_SHOW_MONTHLY_ACCOUNT_SELF_Result
                        monthlyAccountModel.lstMonthlyAccountSelf = objReportBAL.GetMonthlyAccountList(objParam);
                        monthlyAccountModel.TotalOpeningAmount = monthlyAccountModel.lstMonthlyAccountSelf.Sum(m => m.OB_AMT);
                        monthlyAccountModel.TotalCreditDebit = monthlyAccountModel.lstMonthlyAccountSelf.Sum(m => m.MONTHLY_AMT);

                        //state name
                        monthlyAccountModel.StateName = dbcontext.MASTER_STATE.Where(m => m.MAST_STATE_CODE == dbcontext.ADMIN_DEPARTMENT.Where(a => a.ADMIN_ND_CODE == AdminNdCode).Select(s => s.MAST_STATE_CODE).FirstOrDefault()).Select(b => b.MAST_STATE_NAME).FirstOrDefault();

                    }
                    else if (monthlyAccountModel.monthlyStateSrrdaDpiu == "DPIU")
                    {
                        //ReportlevelID = 5;
                        ReportType = "A";

                        if (monthlyAccountModel.Dpiu == 0)
                        {
                            //cal USP_RPT_SHOW_MONTHLY_ACCOUNT_ALLPIU_Result
                            monthlyAccountModel.lstMonthlyAccountAllPIU = objReportBAL.GetMonthlyAccountForAllPIU(objParam);
                            monthlyAccountModel.TotalOpeningAmountForPIU = monthlyAccountModel.lstMonthlyAccountAllPIU.Sum(m => m.OB_AMT);
                            monthlyAccountModel.TotalCreditDebitAmountForPIU = monthlyAccountModel.lstMonthlyAccountAllPIU.Sum(m => m.MONTHLY_AMT);
                        }
                        else
                        {
                            //cal USP_RPT_SHOW_MONTHLY_ACCOUNT_SELF_Result
                            monthlyAccountModel.lstMonthlyAccountSelf = objReportBAL.GetMonthlyAccountList(objParam);
                            monthlyAccountModel.TotalOpeningAmount = monthlyAccountModel.lstMonthlyAccountSelf.Sum(m => m.OB_AMT);
                            monthlyAccountModel.TotalCreditDebit = monthlyAccountModel.lstMonthlyAccountSelf.Sum(m => m.MONTHLY_AMT);
                        }
                        //state name
                        monthlyAccountModel.StateName = dbcontext.MASTER_STATE.Where(m => m.MAST_STATE_CODE == dbcontext.ADMIN_DEPARTMENT.Where(a => a.ADMIN_ND_CODE == AdminNdCode).Select(s => s.MAST_STATE_CODE).FirstOrDefault()).Select(b => b.MAST_STATE_NAME).FirstOrDefault();
                    }
                }

                objMonthlyAcconutModel.NodalAgency = objReportBAL.GetNodalAgency(PMGSYSession.Current.AdminNdCode);
                objMonthlyAcconutModel.Month = monthlyAccountModel.Month;
                objMonthlyAcconutModel.Year = monthlyAccountModel.Year;
                List<SelectListItem> lstCreditDebit = new List<SelectListItem>();
                lstCreditDebit.Insert(0, (new SelectListItem { Text = "Select Type", Value = "0" }));
                lstCreditDebit.Insert(1, (new SelectListItem { Text = "Credit", Value = "C" }));
                lstCreditDebit.Insert(2, (new SelectListItem { Text = "Debit", Value = "D" }));
                var selected = lstCreditDebit.Where(x => x.Value == monthlyAccountModel.CreditDebit).First();
                selected.Selected = true;
                ViewBag.ddlCreditDebit = lstCreditDebit;

                //Set Report Header  
                //CommonFunctions objCommonFunction = new CommonFunctions();
                monthlyAccountModel.NodalAgency = objReportBAL.GetNodalAgency(objParam.AdminNdCode);
                var ReportHeader = dbcontext.SP_ACC_Get_Report_Header_Information("Reports", "MonthlyAccount", PMGSYSession.Current.FundType, ReportlevelID, ReportType).ToList<SP_ACC_Get_Report_Header_Information_Result>().FirstOrDefault();

                if (ReportHeader == null)
                {
                    monthlyAccountModel.FundTypeName = objCommonFunction.GetFundName(PMGSYSession.Current.FundType); ;
                    monthlyAccountModel.ReportName = String.Empty;
                    monthlyAccountModel.ReportParagraphName = String.Empty;
                    monthlyAccountModel.ReportFormNumber = String.Empty;
                }
                else
                {
                    monthlyAccountModel.FundTypeName = objCommonFunction.GetFundName(PMGSYSession.Current.FundType); ;
                    monthlyAccountModel.ReportName = ReportHeader.REPORT_NAME;
                    monthlyAccountModel.ReportParagraphName = ReportHeader.REPORT_PARAGRAPH_NAME;
                    monthlyAccountModel.ReportFormNumber = ReportHeader.REPORT_FORM_NO;
                }
                return View(monthlyAccountModel);
            }
            catch
            {
                return View("MonthlyAccount", new MonthlyAccountModel());
            }
            finally
            {
                if (dbcontext != null)
                {
                    dbcontext.Dispose();
                }
            }
        }

        #endregion
        
        #region Transfer Entry Order
        [Audit]
        public ActionResult TransferEntryOrder()
        {
            try
            {
                RptTransferEntryOrder teo = new RptTransferEntryOrder();

                if (PMGSYSession.Current.AccMonth != 0)
                {
                    teo.Month = PMGSYSession.Current.AccMonth;
                    teo.Year = PMGSYSession.Current.AccYear;
                }
                else
                {
                    teo.Month = (short)DateTime.Now.Month;
                    teo.Year = (short)DateTime.Now.Year;
                }
                
                //teo.Month = (short)DateTime.Now.Month;
                teo.MonthList = commomFuncObj.PopulateMonths(teo.Month);

                

                //teo.Year = (short)DateTime.Now.Year;
                teo.YearList = commomFuncObj.PopulateYears(teo.Year);
                teo.Dpiu = 0;
                teo.DpiuList = commomFuncObj.PopulateDPIUOfSRRDA(PMGSYSession.Current.AdminNdCode);
                teo.AdminNDCode = PMGSYSession.Current.AdminNdCode;
                teo.FundType = PMGSYSession.Current.FundType;

                if (PMGSYSession.Current.LevelId == 4)
                {
                    teo.isSRRDA = true;
                }
                else
                {
                    teo.isSRRDA = false;
                }
                return View(teo);

            }
            catch (Exception ex)
            {
                throw new Exception("error occured");
            }

        }

        [HttpPost]
        [Audit]
        public JsonResult GetTransferEntryOrderJson(RptTransferEntryOrder teo)
        {
            try
            {
                RptTrnasferEntryOrderList rptTeoList = new RptTrnasferEntryOrderList();
                ReportFilter objParam = new ReportFilter();
                string piuSrrdaName = String.Empty;
                string srrdaName = String.Empty;
                string fundName = string.Empty;

                //added by abhishek kamble 6-dec-2013
                if (teo.isSRRDA)
                {
                    objParam.AdminNdCode = PMGSYSession.Current.AdminNdCode;
                }
                else {

                    if (PMGSYSession.Current.LevelId == 5)
                    {
                        objParam.AdminNdCode=PMGSYSession.Current.AdminNdCode;
                    }
                    else {
                        objParam.AdminNdCode = teo.Dpiu;
                    }
                }

                objParam.FundType = PMGSYSession.Current.FundType;

                objParam.LevelId = PMGSYSession.Current.LevelId;

                LedgerModel info = objReportBAL.GetForContextData(objParam);

                // ledgerModel.DistrictDepartment = info.DistrictDepartment;

                //ledgerModel.StateDepartment = info.StateDepartment;
                if (teo.isSRRDA)
                {
                    teo.AdminNDCode = PMGSYSession.Current.AdminNdCode;
                    piuSrrdaName = "<table><tr><td style='color:green;font-weight:bold;'>Name of SRRDA :</td><td style='color:green'>" + "<span class='ui-state-default' style='background:none;border:none'>" + info.StateDepartment + "</span>" + "</td></tr></table>";
                }
                else
                {
                    //Modified by Abhishek kamble 11-oct-2013
                    if (PMGSYSession.Current.LevelId == 5)
                    {
                        teo.AdminNDCode = objParam.AdminNdCode;
                        //srrdaName = "<table><tr><td style='color:green'>Name of SRRDA :</td><td style='color:green'>" + info.StateDepartment + "</td></tr></table>";
                    }
                    else
                    {
                        teo.AdminNDCode = teo.Dpiu;
                      
                    }
                    srrdaName = "<table><tr><td style='color:green;font-weight:bold;'>Name of SRRDA :</td><td style='color:green'>" + "<span class='ui-state-default' style='background:none;border:none'>" + info.StateDepartment + "</span>" + "</td></tr></table>";
                    piuSrrdaName = "<table><tr><td style='color:green;font-weight:bold;'>Name of DPIU :</td><td style='color:green'>" + "<span class='ui-state-default' style='background:none;border:none'>" + info.DistrictDepartment + "</span>" + "</td></tr></table>";
                }


                teo.FundType = PMGSYSession.Current.FundType;
                if (teo.FundType == "A")
                    fundName = "PMGSY  ADMINISTRATIVE FUND";
                else if (teo.FundType == "P")
                    fundName = "PMGSY  PROGRAMME FUND";
                else if (teo.FundType == "M")
                    fundName = "PMGSY  MAINTENANCE FUND";

                rptTeoList = objReportBAL.GetTransferEntryOrderListBAL(teo, objParam);

                var jsonData = new
                {
                    total = 1,
                    page = 1,
                    records = rptTeoList.ListTeo.Count,
                    rows = rptTeoList.ListTeo,
                    header = new
                    {
                        formNumber = rptTeoList.FormNumber,
                        pageHeader1 = fundName,
                        pageHeader2 = rptTeoList.ReportName,
                        pageHeader3 = rptTeoList.Paragraph,
                        piusrrdaName = piuSrrdaName,
                        srrdaName=srrdaName,
                        monthYear = "<span class='ui-corner-all' style='font-weight:bold;'> Month-Year: </span>" + "<span class='ui-state-default' style='background:none;border:none'>" + CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(teo.Month) + "-" + teo.Year + "</span>"
                    },
                    footer = new
                    {
                        debitAmt = rptTeoList.DebitAmt,
                        creditAmt = rptTeoList.DebitAmt
                    }

                };
                return Json(jsonData, JsonRequestBehavior.AllowGet);


            }
            catch (Exception ex)
            {
                throw new Exception("error occured");
            }

        }
        #endregion
        
        #region Balance Sheet (Accounts)

        /// <summary>
        /// action to return the ledger page
        /// </summary>
        /// <returns></returns>
        /// 
        [Audit]        
        public ActionResult BalanceSheet()
        {
            try
            {
                BalanceSheet balanceSheet = new BalanceSheet();

                // ReportFilter objParam = new ReportFilter();

                //new change done by Vikram
                CommonFunctions objCommon = new CommonFunctions();
                balanceSheet.Month = (short)DateTime.Now.Month;
                balanceSheet.MonthList = commomFuncObj.PopulateMonths(DateTime.Now.Month);
                balanceSheet.Year = (short)DateTime.Now.Year;
                balanceSheet.YearList = commomFuncObj.PopulateYears(DateTime.Now.Year);
                if (PMGSYSession.Current.LevelId == 4)
                {
                    balanceSheet.ReportLevel = 'O';
                    // new change done by Vikram
                    List<SelectListItem> lstDPIU = objCommon.PopulateDPIUOfSRRDA(PMGSYSession.Current.AdminNdCode);
                    lstDPIU.RemoveAt(0);
                    lstDPIU.Insert(0, new SelectListItem { Text = "All DPIU", Value = "0" });
                    balanceSheet.DPIUList = lstDPIU;
                }
                else if (PMGSYSession.Current.LevelId == 5)
                    balanceSheet.ReportLevel = 'A';


                #region NEW_CHANGE_VIKRAM

                if (PMGSYSession.Current.LevelId == 6)
                {
                    balanceSheet.NodalAgencyList = commomFuncObj.PopulateNodalAgencies();
                }


                #endregion

                return View("BalanceSheet", balanceSheet);

            }
            catch (Exception ex)
            {
                throw new Exception("error occured");
            }

        }
        
        //[HttpPost]
        //public ActionResult GetBalanceSheet(BalanceSheet balanceSheet)
        //{
        //    try
        //    {

        //        if (ModelState.IsValid)
        //        {
        //            if (PMGSYSession.Current.LevelId == 5)
        //                balanceSheet.ReportLevel = 'O';

        //            ReportFilter objParam = new ReportFilter();

        //            objParam.AdminNdCode = PMGSYSession.Current.AdminNdCode;

        //            objParam.FundType = PMGSYSession.Current.FundType;

        //            objParam.LevelId = PMGSYSession.Current.LevelId;
        //            BalanceSheetList balanceSheetList = objReportBAL.GetBalanceSheetBAL(balanceSheet, objParam);

        //            if (balanceSheetList == null)
        //            {
        //                return PartialView("MaintenanceFundBalanceSheetList", new BalanceSheetList());
        //            }

        //            return PartialView("MaintenanceFundBalanceSheetList", balanceSheetList);
        //        }
        //        return PartialView("MaintenanceFundBalanceSheetList", new BalanceSheetList());
        //    }
        //    catch
        //    {
        //        return PartialView("MaintenanceFundBalanceSheetList", new BalanceSheetList());
        //    }

        //}

        [HttpPost]
        [Audit]     
        public ActionResult GetBalanceSheet(BalanceSheet balanceSheet)
        {
            ReportDAL objDAL = new ReportDAL();
            try
            {

                if (ModelState.IsValid)
                {
                    if (PMGSYSession.Current.LevelId == 5)
                    {
                        balanceSheet.ReportLevel = 'O';
                    }

                    ReportFilter objParam = new ReportFilter();

                    //new change done on 06-09-2013 in absence of vss

                    if (PMGSYSession.Current.LevelId == 6)
                    {
                        switch (balanceSheet.ReportLevel)
                        {
                            case 'O':
                                objParam.AdminNdCode = balanceSheet.StateAdminCode;
                                objParam.LevelId = 4;
                                break;
                            case 'A':
                                objParam.LevelId = 4;
                                if (balanceSheet.AdminCode == 0)
                                {
                                    objParam.AdminNdCode = balanceSheet.StateAdminCode;
                                }
                                else
                                {
                                    objParam.AdminNdCode = balanceSheet.AdminCode.Value;
                                }
                                break;
                            case 'S':
                                objParam.LevelId = 4;
                                objParam.AdminNdCode = balanceSheet.StateAdminCode;
                                break;
                            default:
                                break;
                        }
                    }

                    //end of change


                    //new change done by Vikram
                    else if (balanceSheet.ReportLevel == 'A')
                    {
                        if (balanceSheet.AdminCode == 0 && PMGSYSession.Current.LevelId != 6)
                        {
                            objParam.AdminNdCode = PMGSYSession.Current.AdminNdCode;
                        }
                        else if (PMGSYSession.Current.LevelId != 6)
                        {
                            objParam.AdminNdCode = balanceSheet.AdminCode.Value;
                        }
                    }
                    else
                    {
                        //new change done on 05-09-2013
                        if (balanceSheet.AdminCode == 0)
                        {
                            objParam.AdminNdCode = PMGSYSession.Current.AdminNdCode;
                        }
                        else if (PMGSYSession.Current.LevelId == 4)
                        {
                            if (balanceSheet.ReportLevel == 'O')
                                objParam.AdminNdCode = PMGSYSession.Current.AdminNdCode;
                            else
                                objParam.AdminNdCode = balanceSheet.AdminCode.Value;
                        }
                        //end of change
                    }

                    if (PMGSYSession.Current.LevelId == 5)
                    {
                        objParam.AdminNdCode = PMGSYSession.Current.AdminNdCode;
                    }

                    objParam.FundType = PMGSYSession.Current.FundType;


                    //new change done in absence of vss
                    if (PMGSYSession.Current.LevelId != 6)
                    {
                        objParam.LevelId = PMGSYSession.Current.LevelId;
                    }
                    //objParam.LevelId = PMGSYSession.Current.LevelId;

                    //end of change


                    BalanceSheetList balanceSheetList = objReportBAL.GetBalanceSheetBAL(balanceSheet, objParam);
                    if (PMGSYSession.Current.LevelId == 5 || PMGSYSession.Current.LevelId == 4)
                    {
                        balanceSheetList.NodalAgency = objReportBAL.GetDepartmentName(Convert.ToInt32(PMGSYSession.Current.ParentNDCode)); 
                    }
                    else
                    {
                        balanceSheetList.NodalAgency = objReportBAL.GetDepartmentName(objParam.AdminNdCode);
                    }
                    //added by abhishek kamble 5-dec-2013  
                                      
                    ViewBag.DPIU = balanceSheet.showDPIUName;
                    if (balanceSheet.IsMonthlyYearly==2)
                    {
                        ViewBag.Year = balanceSheet.showMonthName + "-" + balanceSheet.Year;
                        balanceSheetList.IsMonthlyYearly = 2;
                    }
                    else {
                        ViewBag.Year = (balanceSheet.Year) + "-" + ((balanceSheet.Year)+1);
                        balanceSheetList.IsMonthlyYearly = 1;
                    }

                    if (PMGSYSession.Current.LevelId == 6)
                    {
                        ViewBag.State = objDAL.GetStateName(balanceSheet.StateAdminCode);
                        balanceSheetList.NodalAgency = objDAL.GetNodalAgency(balanceSheet.StateAdminCode);
                    }
                    else {
                        ViewBag.State = objDAL.GetStateName(objParam.AdminNdCode);
                    }

                    if (balanceSheetList == null)
                    {
                        return PartialView("MaintenanceFundBalanceSheetList", new BalanceSheetList());
                    }
                    //added by abhishek kamble 20-dec-2013
                    balanceSheetList.balanceSheet = balanceSheet;
                    return PartialView("MaintenanceFundBalanceSheetList", balanceSheetList);
                }
                return PartialView("MaintenanceFundBalanceSheetList", new BalanceSheetList());
            }
            catch
            {
                return PartialView("MaintenanceFundBalanceSheetList", new BalanceSheetList());
            }

        }

        [HttpPost]
        [Audit]
        public JsonResult GetBalanceSheetJSon(BalanceSheet balanceSheet)
        {
            try
            {

                if (ModelState.IsValid)
                {
                    if (PMGSYSession.Current.LevelId == 5)
                        balanceSheet.ReportLevel = 'A';

                    ReportFilter objParam = new ReportFilter();

                    objParam.AdminNdCode = PMGSYSession.Current.AdminNdCode;

                    objParam.FundType = PMGSYSession.Current.FundType;

                    objParam.LevelId = PMGSYSession.Current.LevelId;
                    BalanceSheetList balanceSheetList = objReportBAL.GetBalanceSheetBAL(balanceSheet, objParam);
                    balanceSheetList.NodalAgency = PMGSYSession.Current.DepartmentName;
                    ViewBag.State = PMGSYSession.Current.StateName;
                    ViewBag.Year = balanceSheet.Year;

                    // return Json(balanceSheetList.ListBalanceSheet);
                    var jsonData = new
                    {
                        total = 1,
                        page = 1,
                        records = balanceSheetList.ListBalanceSheet.Count,
                        rows = balanceSheetList.ListBalanceSheet

                    };
                    return Json(jsonData, JsonRequestBehavior.AllowGet);
                }
                return Json("flag", JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json("flag", JsonRequestBehavior.AllowGet);
            }

        }

        [HttpPost]
        [Audit]
        public JsonResult GetBalanceSheetHeaderJSon(BalanceSheet balanceSheet)
        {
            try
            {

                if (ModelState.IsValid)
                {
                    if (PMGSYSession.Current.LevelId == 5)
                        balanceSheet.ReportLevel = 'A';

                    ReportFilter objParam = new ReportFilter();

                    objParam.AdminNdCode = PMGSYSession.Current.AdminNdCode;

                    objParam.FundType = PMGSYSession.Current.FundType;

                    objParam.LevelId = PMGSYSession.Current.LevelId;
                    BalanceSheetList balanceSheetList = objReportBAL.GetBalanceSheetBAL(balanceSheet, objParam);

                    // return Json(balanceSheetList.ListBalanceSheet);
                    var jsonData = new
                    {
                        total = 1,
                        page = 1,
                        records = balanceSheetList.ListBalanceSheet.Count,
                        rows = balanceSheetList.ListBalanceSheet,
                        header = new
                        {
                            FundType = balanceSheetList.FundType,
                            ReportHeader = balanceSheetList.ReportHeader,
                            Section = balanceSheetList.Section,
                            SelectionHeader = balanceSheetList.SelectionHeader,
                            ReportFormNumber = balanceSheetList.ReportFormNumber
                        }

                    };
                    return Json(jsonData, JsonRequestBehavior.AllowGet);
                }
                return Json("flag", JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json("flag", JsonRequestBehavior.AllowGet);
            }

        }

        [Audit]
        public ActionResult ProgramFundBalanceSheet()
        {
            try
            {
                ProgramFundBalanceSheet programFundBalanceSheet = new ProgramFundBalanceSheet();

                // ReportFilter objParam = new ReportFilter();

                programFundBalanceSheet.MONTH_LIST = commomFuncObj.PopulateMonths(DateTime.Now.Month);

                programFundBalanceSheet.YEAR_LIST = commomFuncObj.PopulateYears(DateTime.Now.Year);
                programFundBalanceSheet.RptType = 2;



                return View("ProgramFundBalanceSheet", programFundBalanceSheet);

            }
            catch (Exception ex)
            {
                throw new Exception("error occured");
            }

        }

        //[HttpPost]
        //public ActionResult GetProgramFundBalanceSheet(ProgramFundBalanceSheet programFundBalanceSheet)
        //{
        //    try
        //    {

        //        if (ModelState.IsValid)
        //        {
        //            ReportFilter objParam = new ReportFilter();

        //            objParam.AdminNdCode = PMGSYSession.Current.AdminNdCode;

        //            objParam.FundType = PMGSYSession.Current.FundType;

        //            objParam.LevelId = PMGSYSession.Current.LevelId;

        //            ProgramFundBalanceSheetList programFundBalanceSheetList = objReportBAL.GetProgramFundBalanceSheetBAL(programFundBalanceSheet, objParam);

        //            programFundBalanceSheetList.Year = programFundBalanceSheet.YEAR.ToString();
        //            programFundBalanceSheetList.MONTH = System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(programFundBalanceSheet.MONTH);

        //            if (programFundBalanceSheetList == null)
        //            {
        //                return PartialView("ProgramFundBalanceSheetList", new ProgramFundBalanceSheetList());
        //            }

        //            return PartialView("ProgramFundBalanceSheetList", programFundBalanceSheetList);
        //        }
        //        return PartialView("ProgramFundBalanceSheetList", new ProgramFundBalanceSheetList());
        //    }
        //    catch
        //    {
        //        return PartialView("ProgramFundBalanceSheetList", new ProgramFundBalanceSheetList());
        //    }

        //}

        //public ActionResult AdminFundBalanceSheet()
        //{
        //    try
        //    {
        //        AdminFundBalanceSheet adminFundBalanceSheet = new AdminFundBalanceSheet();

        //        // ReportFilter objParam = new ReportFilter();

        //        adminFundBalanceSheet.MONTH_LIST = commomFuncObj.PopulateMonths(DateTime.Now.Month);

        //        adminFundBalanceSheet.YEAR_LIST = commomFuncObj.PopulateYears(DateTime.Now.Year);
        //        adminFundBalanceSheet.RptType = 5;



        //        return View("AdminFundBalanceSheet", adminFundBalanceSheet);

        //    }
        //    catch (Exception ex)
        //    {
        //        throw new Exception("error occured");
        //    }

        //}

        //[HttpPost]
        //public ActionResult GetAdminFundBalanceSheet(AdminFundBalanceSheet adminFundBalanceSheet)
        //{
        //    try
        //    {

        //        if (ModelState.IsValid)
        //        {
        //            ReportFilter objParam = new ReportFilter();

        //            objParam.AdminNdCode = PMGSYSession.Current.AdminNdCode;

        //            objParam.FundType = PMGSYSession.Current.FundType;

        //            objParam.LevelId = PMGSYSession.Current.LevelId;

        //            AdminFundBalanceSheetList adminFundBalanceSheetList = objReportBAL.GetAdminFundBalanceSheetBAL(adminFundBalanceSheet, objParam);

        //            adminFundBalanceSheetList.Year = adminFundBalanceSheet.YEAR.ToString();
        //            adminFundBalanceSheetList.MONTH = System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(adminFundBalanceSheet.MONTH);

        //            if (adminFundBalanceSheetList == null)
        //            {
        //                return PartialView("AdminFundBalanceSheetList", new AdminFundBalanceSheetList());
        //            }

        //            return PartialView("AdminFundBalanceSheetList", adminFundBalanceSheetList);
        //        }
        //        return PartialView("AdminFundBalanceSheetList", new AdminFundBalanceSheetList());
        //    }
        //    catch
        //    {
        //        return PartialView("AdminFundBalanceSheetList", new AdminFundBalanceSheetList());
        //    }

        //}

        [Audit]
        [OutputCache(NoStore=true,Duration=0,VaryByParam="*")]
        public ActionResult MaintenanceFundBalanceSheet()
        {
            try
            {
                /* MaintenanceFundBalanceSheet maintenanceFundBalanceSheet = new MaintenanceFundBalanceSheet();

                // ReportFilter objParam = new ReportFilter();

                 maintenanceFundBalanceSheet.MONTH_LIST = commomFuncObj.PopulateMonths(DateTime.Now.Month);

                 maintenanceFundBalanceSheet.YEAR_LIST = commomFuncObj.PopulateYears(DateTime.Now.Year);
                 maintenanceFundBalanceSheet.RptType = 4;

                 //List<short> opLevel = new List<short>();

                 //if (PMGSYSession.Current.LevelId == 5)
                 //{
                 //    opLevel.Add(1);
                 //    opLevel.Add(2);
                 //}
                 //else if (PMGSYSession.Current.LevelId == 4)
                 //{
                 //    opLevel.Add(2);
                 //    opLevel.Add(3);
                 //}

              
                 //objParam.AdminNdCode = PMGSYSession.Current.AdminNdCode;

                 //objParam.FundType = PMGSYSession.Current.FundType;

                 //objParam.LevelId = PMGSYSession.Current.LevelId;

                 //LedgerModel info = objReportBAL.GetForContextData(objParam);

                 //ledgerModel.DistrictDepartment = info.DistrictDepartment;

                 //ledgerModel.StateDepartment = info.StateDepartment;

                 //ledgerModel.ReportAnnex = "PMGSY/IA/F-9";

                 return View("MaintenanceFundBalanceSheet", maintenanceFundBalanceSheet);*/
                try
                {
                    BalanceSheet balanceSheet = new BalanceSheet();

                    //Added By Abhishek kamble 30-jan-2013 start            
                    Int16 Month = 0;
                    Int16 Year = 0;
                    Int16 IsStateSelected = 0;
                    Int16 ADMIN_ND_CODE = 0;
                    Int16 PARENT_ND_CODE = 0;
                    String BalSheetType = string.Empty;


                    if (!String.IsNullOrEmpty(Request.Params["Month"]))
                    {
                        Month = Convert.ToInt16(Request.Params["Month"]);
                    }
                    if (!String.IsNullOrEmpty(Request.Params["Month"]))
                    {
                        Year = Convert.ToInt16(Request.Params["Year"]);
                    }
                    if (!String.IsNullOrEmpty(Request.Params["IsStateSelected"]))
                    {
                        IsStateSelected = Convert.ToInt16(Request.Params["IsStateSelected"]);
                    }
                    if (!String.IsNullOrEmpty(Request.Params["ADMIN_ND_CODE"]))
                    {
                        ADMIN_ND_CODE = Convert.ToInt16(Request.Params["ADMIN_ND_CODE"]);
                    }
                    if (!String.IsNullOrEmpty(Request.Params["PARENT_ND_CODE"]))
                    {
                        PARENT_ND_CODE = Convert.ToInt16(Request.Params["PARENT_ND_CODE"]);
                    }
                    if (!String.IsNullOrEmpty(Request.Params["BalSheetType"]))
                    {
                        BalSheetType =Request.Params["BalSheetType"];
                    }

                    if (ADMIN_ND_CODE > 0)
                    {
                        if (IsStateSelected == 0 && BalSheetType == "A")
                        {
                            balanceSheet.ReportLevel= 'A';
                            balanceSheet.StateAdminCode = ADMIN_ND_CODE;
                            balanceSheet.AdminCode= 0;
                        }
                        else if (IsStateSelected == 1 && BalSheetType == "A")
                        {
                            balanceSheet.ReportLevel = 'A';
                            balanceSheet.StateAdminCode = PARENT_ND_CODE;
                            balanceSheet.AdminCode = ADMIN_ND_CODE;
                        }
                        else if (BalSheetType == "O")
                        {
                            balanceSheet.ReportLevel = 'O';
                            //balanceSheet.AdminCode = ADMIN_ND_CODE;
                            balanceSheet.StateAdminCode = ADMIN_ND_CODE;
                        }
                        else if (BalSheetType == "S")
                        {
                            balanceSheet.ReportLevel = 'S';
                            //balanceSheet.AdminCode = ADMIN_ND_CODE;
                            balanceSheet.StateAdminCode = ADMIN_ND_CODE;
                        }
                    }

                    //Added By Abhishek kamble 30-jan-2013 end                    

                    // ReportFilter objParam = new ReportFilter();
                    //new change done by Vikram
                    CommonFunctions objCommon = new CommonFunctions();

                    //Added by Vikram on 07 Jan 2014

                    //if (PMGSYSession.Current.AccMonth != 0)
                    //{
                    //    balanceSheet.Month = PMGSYSession.Current.AccMonth;
                    //    balanceSheet.Year = PMGSYSession.Current.AccYear;
                    //}
                    //else
                    //{
                    //    balanceSheet.Month = Convert.ToInt16(DateTime.Now.Month);
                    //    balanceSheet.Year = Convert.ToInt16(DateTime.Now.Year);
                    //}

                    if (Month != 0)
                    {
                        balanceSheet.Month = Month;
                    }
                    else
                    {

                        if (PMGSYSession.Current.AccMonth != 0)
                        {
                            balanceSheet.Month = PMGSYSession.Current.AccMonth;
                        }
                        else
                        {
                            balanceSheet.Month = Convert.ToInt16(DateTime.Now.Month);
                        }
                    }

                    if (Year != 0)
                    {
                        balanceSheet.Year = Year;
                    }
                    else
                    {
                        if (PMGSYSession.Current.AccYear != 0)
                        {
                            balanceSheet.Year = PMGSYSession.Current.AccYear;
                        }
                        else
                        {
                            balanceSheet.Year = Convert.ToInt16(DateTime.Now.Year);
                        }
                    }

                   
                    //balanceSheet.Month = (short)DateTime.Now.Month;
                    balanceSheet.MonthList = commomFuncObj.PopulateMonths(balanceSheet.Month);
                    //balanceSheet.Year = (short)DateTime.Now.Year;
                    balanceSheet.YearList = commomFuncObj.PopulateYears(balanceSheet.Year);
                    if (PMGSYSession.Current.LevelId == 4)
                    {
                        balanceSheet.ReportLevel = 'O';
                        // new change done by Vikram
                        List<SelectListItem> lstDPIU = objCommon.PopulateDPIUOfSRRDA(PMGSYSession.Current.AdminNdCode);
                        
                        //commented by abhishek kamble 6-dec-2013
                        //lstDPIU.RemoveAt(0);                   
                        lstDPIU.Insert(0, new SelectListItem { Text = "All DPIU", Value = "0" });
                        balanceSheet.DPIUList = lstDPIU;

                    }
                    else if (PMGSYSession.Current.LevelId == 5)
                        balanceSheet.ReportLevel = 'A';

                    #region NEW_CHANGE_VIKRAM

                    if (PMGSYSession.Current.LevelId == 6)
                    {
                        balanceSheet.NodalAgencyList = commomFuncObj.PopulateNodalAgencies();
                    }


                    #endregion



                    return View("MaintenanceFundBalanceSheet", balanceSheet);

                }
                catch (Exception ex)
                {
                    throw new Exception("error occured");
                }

            }
            catch (Exception ex)
            {
                throw new Exception("error occured");
            }

        }

        //[HttpPost]
        //public ActionResult GetMaintenanceFundBalanceSheet(MaintenanceFundBalanceSheet maintenanceFundBalanceSheet)
        //{
        //    try
        //    {

        //        if (ModelState.IsValid)
        //        {
        //            ReportFilter objParam = new ReportFilter();

        //            objParam.AdminNdCode = PMGSYSession.Current.AdminNdCode;

        //            objParam.FundType = PMGSYSession.Current.FundType;

        //            objParam.LevelId = PMGSYSession.Current.LevelId;

        //            MaintenanceFundBalanceSheetList maintenanceFundBalanceSheetList = objReportBAL.GetMaintenanceFundBalanceSheetBAL(maintenanceFundBalanceSheet, objParam);

        //            maintenanceFundBalanceSheetList.Year = maintenanceFundBalanceSheet.YEAR.ToString();
        //            maintenanceFundBalanceSheetList.MONTH = System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(maintenanceFundBalanceSheet.MONTH);

        //            if (maintenanceFundBalanceSheetList == null)
        //            {
        //                return PartialView("MaintenanceFundBalanceSheetList", new MaintenanceFundBalanceSheetList());
        //            }

        //            return PartialView("MaintenanceFundBalanceSheetList", maintenanceFundBalanceSheetList);
        //        }
        //        return PartialView("MaintenanceFundBalanceSheetList", new MaintenanceFundBalanceSheetList());
        //    }
        //    catch
        //    {
        //        return PartialView("MaintenanceFundBalanceSheetList", new MaintenanceFundBalanceSheetList());
        //    }

        //}

        #endregion Balance Sheet (MF)

        #region Register of Works

        [Audit]
        public ActionResult RegisterOfWorks()
        {
            var dbContext = new PMGSYEntities();
            try
            {
                TransactionParams objparams = new TransactionParams();
                RegisterOfWorksModel registerOfWorksModel = new RegisterOfWorksModel();

                objparams.STATE_CODE = Convert.ToInt16(PMGSYSession.Current.StateCode);
                objparams.DISTRICT_CODE = PMGSYSession.Current.DistrictCode;
                objparams.FUND_TYPE = PMGSYSession.Current.FundType;
                objparams.LVL_ID = PMGSYSession.Current.LevelId;
                objparams.ADMIN_ND_CODE = PMGSYSession.Current.AdminNdCode;

                // --- Filters
                if (objparams.LVL_ID == 4)    //State
                {
                    List<SelectListItem> lstSRRDA = commomFuncObj.PopulateNodalAgencies();
                    //var selected = lstSRRDA.Where(m => m.Value == PMGSYSession.Current.AdminNdCode.ToString()).First();
                    //selected.Selected = true;
                    ViewBag.SRRDA = lstSRRDA;
                    registerOfWorksModel.DEPARTMENT_LIST = commomFuncObj.PopulateDPIU(objparams);
                    registerOfWorksModel.DEPARTMENT_LIST.RemoveAt(0);                                                   //Remove first select value & set first PIU as selected
                    objparams.ADMIN_ND_CODE = Convert.ToInt32(registerOfWorksModel.DEPARTMENT_LIST.ElementAt(0).Value);

                    objparams.DISTRICT_CODE = Convert.ToInt32(dbContext.ADMIN_DEPARTMENT.Where(c => c.ADMIN_ND_CODE == objparams.ADMIN_ND_CODE).Select(c => c.MAST_DISTRICT_CODE).FirstOrDefault());

                    registerOfWorksModel.CONTRACTOR_LIST = commomFuncObj.PopulateContractorSupplier(objparams);
                }
                else if (objparams.LVL_ID == 5)  //District
                {
                    registerOfWorksModel.ADMIN_ND_CODE = objparams.ADMIN_ND_CODE;
                    objparams.DISTRICT_CODE = Convert.ToInt32(dbContext.ADMIN_DEPARTMENT.Where(c => c.ADMIN_ND_CODE == objparams.ADMIN_ND_CODE).Select(c => c.MAST_DISTRICT_CODE).FirstOrDefault());
                    registerOfWorksModel.CONTRACTOR_LIST = commomFuncObj.PopulateContractorSupplier(objparams);
                }
                //Added By Ashish Markande on 8/10/2013
                else if (objparams.LVL_ID == 6)
                {
                    List<SelectListItem> lstSRRDA = commomFuncObj.PopulateNodalAgencies();
                    ViewBag.SRRDA = lstSRRDA;
                    registerOfWorksModel.DEPARTMENT_LIST = commomFuncObj.PopulateDPIU(objparams);
                    //registerOfWorksModel.DEPARTMENT_LIST.RemoveAt(0);                                                  
                    objparams.ADMIN_ND_CODE = Convert.ToInt32(registerOfWorksModel.DEPARTMENT_LIST.ElementAt(0).Value);

                    objparams.DISTRICT_CODE = Convert.ToInt32(dbContext.ADMIN_DEPARTMENT.Where(c => c.ADMIN_ND_CODE == objparams.ADMIN_ND_CODE).Select(c => c.MAST_DISTRICT_CODE).FirstOrDefault());

                    registerOfWorksModel.CONTRACTOR_LIST = commomFuncObj.PopulateContractorSupplier(objparams);

                }

                List<SelectListItem> lstAgreement = new SelectList(new List<SelectListItem>().AsEnumerable<SelectListItem>(), "Value", "Text").ToList();
                lstAgreement.Insert(0, (new SelectListItem { Text = "Select Agreement", Value = "0", Selected = true }));
                registerOfWorksModel.AGREEEMENT_LIST = lstAgreement;

                return View(registerOfWorksModel);
            }
            catch (Exception)
            {
                return null;
            }
            finally
            {
                dbContext.Dispose();
            }
        }


        [Audit]
        public JsonResult PopulateAgreement(string id1, string id2)
        {
            var dbContext = new PMGSYEntities();
            try
            {
                TransactionParams objparams = new TransactionParams();
                objparams.MAST_CONT_ID = Convert.ToInt16(id1.Trim());
                objparams.ADMIN_ND_CODE = Convert.ToInt16(id2.Trim());
                objparams.DISTRICT_CODE = Convert.ToInt32(dbContext.ADMIN_DEPARTMENT.Where(c => c.ADMIN_ND_CODE == objparams.ADMIN_ND_CODE).Select(c => c.MAST_DISTRICT_CODE).FirstOrDefault());
                return Json(commomFuncObj.PopulateAgreement(objparams));
            }
            catch (Exception ex)
            {
                string errorMsg = string.Format("Errors: {0}", ex.Message);
                Response.TrySkipIisCustomErrors = true;
                Response.StatusCode = 500;
                Response.Write(errorMsg);
                return Json(string.Empty);
            }
            finally
            {
                dbContext.Dispose();
            }
        }


        [Audit]
        public JsonResult PopulateContractorSupplier(string id)
        {
            var dbContext = new PMGSYEntities();
            try
            {
                TransactionParams objparams = new TransactionParams();
                objparams.ADMIN_ND_CODE = Convert.ToInt16(id.Trim());
                objparams.DISTRICT_CODE = Convert.ToInt32(dbContext.ADMIN_DEPARTMENT.Where(c => c.ADMIN_ND_CODE == objparams.ADMIN_ND_CODE).Select(c => c.MAST_DISTRICT_CODE).FirstOrDefault());
                List<SelectListItem> lstContractor = commomFuncObj.PopulateContractorSupplier(objparams);
                lstContractor.RemoveAt(0);
                lstContractor.Insert(0, new SelectListItem {Text="Select Contractor",Value="0"});
                return Json(lstContractor);
            }
            catch (Exception ex)
            {
                string errorMsg = string.Format("Errors: {0}", ex.Message);
                Response.TrySkipIisCustomErrors = true;
                Response.StatusCode = 500;
                Response.Write(errorMsg);
                return Json(string.Empty);
            }
            finally
            {
                dbContext.Dispose();
            }
        }



        [Audit]
        public ActionResult RegisterOfWorksHeaderPartial(string id)
        {
            try
            {
                String[] urlSplitParams = id.Split('$');

                TransactionParams objparams = new TransactionParams();
                RegisterOfWorksModel registerOfWorksModel = new RegisterOfWorksModel();

                objparams.STATE_CODE = Convert.ToInt16(PMGSYSession.Current.StateCode);
                objparams.FUND_TYPE = PMGSYSession.Current.FundType;
                objparams.LVL_ID = PMGSYSession.Current.LevelId;

                objparams.ADMIN_ND_CODE = Convert.ToInt32(urlSplitParams[0]);
                objparams.MAST_CONT_ID = Convert.ToInt32(urlSplitParams[1]);
                objparams.AGREEMENT_CODE = Convert.ToInt32(urlSplitParams[2]);

                registerOfWorksModel = objReportBAL.RegisterOfWorksBAL(objparams);

                return View(registerOfWorksModel);
            }
            catch (Exception)
            {
                return Json(new { Success = false, ErrorMessage = "An Error Occurred While Processing Your Request." });
            }
        }


        [HttpPost]
        [Audit]
        public ActionResult RegisterOfWorksHeaderGrid(FormCollection formCollection)
        {

            int totalRecords;
            try
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
                int distCode = Convert.ToInt32(formCollection["districtCode"]);
                var jsonData = new
                {
                    rows = objReportBAL.RegisterOfWorksHeaderGridBAL(Convert.ToInt32(formCollection["page"]) - 1, Convert.ToInt32(formCollection["rows"]),
                                                            formCollection["sidx"], formCollection["sord"], out totalRecords,
                                                            Convert.ToInt32(formCollection["ADMIN_ND_CODE"]), Convert.ToInt32(formCollection["MAST_CON_ID"]),
                                                            Convert.ToInt32(formCollection["TEND_AGREEMENT_CODE"])),
                    total = totalRecords <= Convert.ToInt32(formCollection["rows"]) ? 1 : totalRecords / Convert.ToInt32(formCollection["rows"]) + 1,
                    page = Convert.ToInt32(formCollection["page"]),
                    records = totalRecords
                };
                return Json(jsonData);
            }
            catch (Exception)
            {
                return Json(String.Empty);
            }
        }

        #endregion

        #region Schedule
        /// Auther      : Anand Singh
        /// Controller  : Reports
        /// Action      : AccountFilter
        /// Description : Return view for Common filter which is used in all the report of accounting module.
        /// Date        : 17-09-2013
        public PartialViewResult AccountFilter(AccountFilterModel filterViewModel)
        {
            int RptLevel = 0;
            int agencyCode = 0;

            //AccountFilterModel filterViewModel = new AccountFilterModel();

            ReportFilter objParam = new ReportFilter
            {
                //Modified by abhishek kamble 20-dec-2013
                //Month = Convert.ToInt16(DateTime.Now.Month),
                //Year = Convert.ToInt16(DateTime.Now.Year),
               
                Month = filterViewModel.Month,
                Year = filterViewModel.Year,
                
                AdminNdCode = PMGSYSession.Current.AdminNdCode,
                FundType = PMGSYSession.Current.FundType,
                LevelId = PMGSYSession.Current.LevelId
            };

            //Added by abhishek kamble 20-dec-2013 start
            
            if (PMGSYSession.Current.AccMonth != 0)
            {
                objParam.Month = PMGSYSession.Current.AccMonth;
            }
            else if (filterViewModel.Month == 0)
            {
                objParam.Month = Convert.ToInt16(DateTime.Now.Month);
            }

            if (PMGSYSession.Current.AccYear != 0)
            {
                objParam.Year = PMGSYSession.Current.AccYear;
            }
            else if (filterViewModel.Year == 0)
            {
                objParam.Year = Convert.ToInt16(DateTime.Now.Year);
            }
            

            if (filterViewModel.ReportLevel != 0)
            {
                RptLevel = filterViewModel.ReportLevel;            
            }

           

            if (PMGSYSession.Current.LevelId == 4)
            {
                agencyCode = PMGSYSession.Current.AdminNdCode;
            }
            else if (filterViewModel.Agency != 0)
            {
                agencyCode = filterViewModel.Agency;
            }
            //Added by abhishek kamble 20-dec-2013 end

            //Added by abhishek kamble 23-dec-2013 end
            if (filterViewModel.ReportLevel == 0)
            {
                RptLevel = 1;
            }

            PMGSY.Models.Report.Account.AccountFilterModel filterModel = new Models.Report.Account.AccountFilterModel
            {
                //Modified by abhishek kamble 20-dec-2013
                Month = objParam.Month,
                ListMonth = commomFuncObj.PopulateMonths(objParam.Month),
                ListYear = commomFuncObj.PopulateYears(objParam.Year),
                Year = objParam.Year,
                
                LevelId = objParam.LevelId,

                //Modified by abhishek kamble 20-dec-2013
                //Piu = objParam.AdminNdCode,                              
                Piu = filterViewModel.Piu,                              
                //Agency = 0,
                Agency = agencyCode,                
                //ReportLevel = 4,
                ReportLevel = RptLevel,
                //ReportType = 2
                ReportType = filterViewModel.ReportType
            };
            if (filterModel.LevelId == 4)
            {
                filterModel.ListPiu = commomFuncObj.PopulateDPIUOfSRRDA(objParam.AdminNdCode);
                filterModel.ListPiu.Insert(0, (new SelectListItem { Text = "All DPIU", Value = "0", Selected = true }));
                filterModel.ListAgency = commomFuncObj.PopulateNodalAgencies(PMGSYSession.Current.StateCode);
            }
            else if (filterModel.LevelId == 6)
            {
                //filterModel.ListPiu = commomFuncObj.PopulateDPIUOfSRRDA();  
                filterModel.ListPiu = new List<SelectListItem>();
                filterModel.ListPiu.Insert(0, (new SelectListItem { Text = "All DPIU", Value = "0", Selected = true }));
                filterModel.ListAgency = commomFuncObj.PopulateNodalAgencies();
            }


            return PartialView(filterModel);

        }
        /// Auther      : Anand Singh
        /// Controller  : Reports
        /// Action      : GetDPIUOfSRRDA
        /// Description : Return all the DPIU for SRRDA Agency Level in the form of JSon data.
        /// Date        : 17-09-2013
        [Audit]
        public JsonResult GetDPIUOfSRRDA(int ndcode)
        {
            return Json(commomFuncObj.PopulateDPIUOfSRRDA(ndcode), JsonRequestBehavior.AllowGet);
        }
        /// Auther      : Anand Singh
        /// Controller  : Reports
        /// Action      : Schedule
        /// Description : Return view for the Schedulue Repayable at PIU, SRRDA and MoRD level
        /// Date        : 17-09-2013

        [Audit]
        public ActionResult Schedule(BalanceSheet BalanceSheetViewModel)
        {
            ReportFilter objParam = new ReportFilter
            {
                //Modified by abhisheke kamble 23-dec-2013
                //Month = Convert.ToInt16(DateTime.Now.Month),
                //Year = Convert.ToInt16(DateTime.Now.Year),
                AdminNdCode = PMGSYSession.Current.AdminNdCode,
                FundType = PMGSYSession.Current.FundType,
                LevelId = PMGSYSession.Current.LevelId

            };

            PMGSY.Models.Report.Account.ScheduleModel scheduleModel = new Models.Report.Account.ScheduleModel
            {
                Header = "SCHEDULES FORMING PART OF BALANCE SHEET OF PIU",
                Paragraph1 = "(Referred to in Paragraphs 14.4.4 and 14.4.7 of the Manual",
                Paragraph2 = "Schedule for Programme Fund Received by PIU",
                FormNumber = "PMGSY/SCH/F-52A",
                LevelId = objParam.LevelId



            };
            if (PMGSYSession.Current.FundType.Equals("P"))
            {
                scheduleModel.FundType = "PMGSY PROGRAMME FUND";
            }
            else if (PMGSYSession.Current.FundType.Equals("A"))
            {
                scheduleModel.FundType = "PMGSY ADMINISTRATIVE FUND";
            }
            else if (PMGSYSession.Current.FundType.Equals("M"))
            {
                scheduleModel.FundType = "PMGSY MAITENANCE FUND";
            }
            if (scheduleModel.LevelId == 5)
            {
                scheduleModel.PiuName = PMGSYSession.Current.DepartmentName;

            }

            //Added by Vikram to Populate the Month and Year if they are in session
            if (PMGSYSession.Current.AccMonth != 0)
            {
                scheduleModel.Month = PMGSYSession.Current.AccMonth;
                scheduleModel.Year = PMGSYSession.Current.AccYear;
            }
            else
            {
                //end of change
                //Added by Abhishek kamble 20-dec-2013  start
                //scheduleModel.Month = BalanceSheetViewModel.Month;

                if (Convert.ToInt16(BalanceSheetViewModel.SelectedMonth) == 0)
                {
                    scheduleModel.Month = Convert.ToInt16(System.DateTime.Now.Month);
                }
                else {
                    scheduleModel.Month = Convert.ToInt16(BalanceSheetViewModel.SelectedMonth);
                }

                //scheduleModel.Year = BalanceSheetViewModel.Year;
                if (Convert.ToInt16(BalanceSheetViewModel.SelectedYear)==0)
                {
                    scheduleModel.Year = Convert.ToInt16(System.DateTime.Now.Year);
                }
                else {
                    scheduleModel.Year = Convert.ToInt16(BalanceSheetViewModel.SelectedYear);
                }
                scheduleModel.ReportType = BalanceSheetViewModel.IsMonthlyYearly;
            }

            if (BalanceSheetViewModel.AdminCode != null)
            {
                scheduleModel.Piu = BalanceSheetViewModel.AdminCode.Value;
            }
            if (BalanceSheetViewModel.StateAdminCode != null)
            {
                scheduleModel.State = BalanceSheetViewModel.StateAdminCode;
            }

            if (BalanceSheetViewModel.ReportLevel == 'S')//State
            {
                scheduleModel.ReportLevel = 4;
            }
            else if (BalanceSheetViewModel.ReportLevel == 'O')//SRRDA
            {
                scheduleModel.ReportLevel = 1;
            }
            else if (BalanceSheetViewModel.ReportLevel == 'A')//All DPIU
            {
                scheduleModel.ReportLevel = 2;
            }
            //Added by Abhishek kamble 20-dec-2013  end

            return View(scheduleModel);
        }

        /// Auther      : Anand Singh
        /// Controller  : Reports
        /// Action      : GetSchedule
        /// Description : Return json data for schedule repayable.
        /// Date        : 17-09-2013        
        [HttpPost]
        [Audit]
        public JsonResult GetSchedule(int month, int year, Int64 ndcode, int rlevel, int allpiu)
        {



            var reportMaster = commomFuncObj.GetReportId(rlevel, PMGSYSession.Current.FundType, 141);//OMMAS_DEV-141 - Trainning -180
            ACC_RPT_REPORT_PROPERTY[] reportHeader = reportMaster.ACC_RPT_REPORT_PROPERTY.ToArray<ACC_RPT_REPORT_PROPERTY>();
            object[] arrObject = new object[] { reportMaster.RPT_ID, ndcode, month, year, allpiu };

           ReportBAL.ReportSPBAL<PMGSY.Models.Report.Account.Schedule> objdal = new ReportBAL.ReportSPBAL<PMGSY.Models.Report.Account.Schedule>();
            List<PMGSY.Models.Report.Account.Schedule> ListSchedule = objdal.GetReportBAL(2, arrObject);

            var jsonData = new
            {
                rows = ListSchedule,
                total = 1,
                page = 1,
                records = ListSchedule.Count,
                footerData = new
                {
                    totCurrAmt = ListSchedule.Sum(m => m.CURRENT_AMT),
                    totPrevAmt = ListSchedule.Sum(m => m.PREVIOUS_AMT)
                },
                reportHeader = new
                {
                    formNumbr = reportHeader[0].PROP_VALUE,
                    scheduleNo = reportHeader[1].PROP_VALUE,
                    fundType = reportHeader[2].PROP_VALUE,
                    heading = reportHeader[3].PROP_VALUE,
                    referance = reportHeader[4].PROP_VALUE
                }
            };

            return Json(jsonData);
        }

        //String parameter, String hash, String key
        [Audit]
        public ActionResult ScheduleIncidental(BalanceSheet BalanceSheetViewModel)
        {
            ReportFilter objParam = new ReportFilter
            {
                //Modified by abhisheke kamble 24-dec-2013
                //Month = Convert.ToInt16(DateTime.Now.Month),
                //Year = Convert.ToInt16(DateTime.Now.Year),
                AdminNdCode = PMGSYSession.Current.AdminNdCode,
                FundType = PMGSYSession.Current.FundType,
                LevelId = PMGSYSession.Current.LevelId

            };

            //BalanceSheet BalanceSheetViewModel = new BalanceSheet();

            //new Change by Abhishek kamble 26-dec-2013 start
            //Dictionary<string, string> decryptedParameters = null;
            //decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { parameter, hash, key });

            //BalanceSheet BalanceSheetViewModel = new BalanceSheet();
            //BalanceSheetViewModel.Month = Convert.ToInt16(decryptedParameters["Month"]);
            //BalanceSheetViewModel.Year = Convert.ToInt16(decryptedParameters["Year"]);
            //BalanceSheetViewModel.IsMonthlyYearly = Convert.ToInt32(decryptedParameters["IsMonthlyYearly"]);
            //BalanceSheetViewModel.ReportLevel = Convert.ToChar(decryptedParameters["ReportLevel"]);
            //BalanceSheetViewModel.AdminCode = Convert.ToInt32(decryptedParameters["AdminCode"]);
            //BalanceSheetViewModel.StateAdminCode = Convert.ToInt32(decryptedParameters["StateAdminCode"]);
            //BalanceSheetViewModel.HeadCode = Convert.ToInt32(decryptedParameters["HeadCode"]);
            //BalanceSheetViewModel.HeadName = decryptedParameters["HeadName"];
            //new Change by Abhishek kamble 26-dec-2013 End

            PMGSY.Models.Report.Account.ScheduleModel scheduleModel = new Models.Report.Account.ScheduleModel
            {
                LevelId = objParam.LevelId
            };
            if (scheduleModel.LevelId == 5)
            {
                scheduleModel.PiuName = PMGSYSession.Current.DepartmentName;

            }

            //Added by Abhishek kamble 20-dec-2013  start
            //scheduleModel.Month = BalanceSheetViewModel.Month;
            //new change done by Vikram on 07 Jan 2014
            if (PMGSYSession.Current.AccMonth != 0)
            {
                scheduleModel.Month = PMGSYSession.Current.AccMonth;
                scheduleModel.Year = PMGSYSession.Current.AccYear;
            }
            else
                //end of change
            {
                if (Convert.ToInt16(BalanceSheetViewModel.SelectedMonth) == 0)
                {
                    scheduleModel.Month =Convert.ToInt16(System.DateTime.Now.Month);
                    scheduleModel.Year =Convert.ToInt16(System.DateTime.Now.Year);
                }
                else
                {
                    scheduleModel.Month = Convert.ToInt16(BalanceSheetViewModel.SelectedMonth);
                    scheduleModel.Year = Convert.ToInt16(BalanceSheetViewModel.SelectedYear);
                }
            }
            scheduleModel.ReportType = BalanceSheetViewModel.IsMonthlyYearly;

            if (BalanceSheetViewModel.AdminCode != null)
            {
                scheduleModel.Piu = BalanceSheetViewModel.AdminCode.Value;
            }
            if (BalanceSheetViewModel.StateAdminCode != null)
            {
                scheduleModel.State = BalanceSheetViewModel.StateAdminCode;
            }

            if (BalanceSheetViewModel.ReportLevel == 'S')//State
            {
                scheduleModel.ReportLevel = 4;
            }
            else if (BalanceSheetViewModel.ReportLevel == 'O')//SRRDA
            {
                scheduleModel.ReportLevel = 1;
            }
            else if (BalanceSheetViewModel.ReportLevel == 'A')//All DPIU
            {
                scheduleModel.ReportLevel = 2;
            }
            //Added by Abhishek kamble 20-dec-2013  end

            return View(scheduleModel);
        }
        [HttpPost]
        [Audit]
        public ActionResult ValidateParameter(string month, string year, string ndcode, string rlevel, string allpiu)
        {
            StringBuilder message = new StringBuilder();
            message = message.Append("<ul>");

            if (Regex.IsMatch(rlevel, "^([0-9]+)$"))
            {
                if (Convert.ToInt16(rlevel) > 4 || Convert.ToInt16(rlevel) < 0)
                {
                    //return Json(new { success = false, message = "<ul><li>Invalid report level.<ul><li>" });
                    message = message.Append("<li>Invalid report level.</li>");
                }
            }
            else
            {
                message = message.Append("<li>Invalid report level.</li>");
                //return Json(new { success = false, message = "<ul><li>Invalid report level.<ul><li>" });
            }

            //Modified by abhishek kamble 2-dec-2013
            if (Regex.IsMatch(month, "^([0-9]+)$"))
            {
                if (Convert.ToInt16(month) > 12 || Convert.ToInt16(month) < 0)
                {
                    //  return Json(new { success = false, message = "<ul><li>Invalid month.<ul><li>" });
                    message = message.Append("<li>Invalid month.</li>");
                }
            }
            else
            {
                //return Json(new { success = false, message = "<ul><li>Invalid month.<ul><li>" });
                message = message.Append("<li>Invalid month.</li>");
            }
            

            if (Regex.IsMatch(year, "^([0-9]+)$"))
            {
                if (Convert.ToInt16(year) > 2099 || Convert.ToInt16(year) < 1990)
                {
                    //return Json(new { success = false, message = "<ul><li>Invalid year.<ul><li>" });
                    message = message.Append("<li>Invalid year.</li>");
                }
            }
            else
            {
                //return Json(new { success = false, message = "<ul><li>Invalid year.<ul><li>" });
                message = message.Append("<li>Invalid year.</li>");
            }

            //modified by abhishek kamble 24-dec-2013
            if (ndcode != null)
            {
                if (Regex.IsMatch(ndcode, "^([0-9]+)$"))
                {
                    if (Convert.ToInt64(ndcode) > Int64.MaxValue || Convert.ToInt64(ndcode) < 0)
                    {
                        //return Json(new { success = false, message = "<ul><li>Invalid agency.<ul><li>" });
                        message = message.Append("<li>Invalid agency.</li>");
                    }
                }
                else
                {
                    message = message.Append("<li>Invalid agency.</li>");
                    //return Json(new { success = false, message = "<ul><li>Invalid agency.<ul><li>" });
                }
            }

            message = message.Append("</ul>");

            if (message.Length == 9)
            {
                return Json(new { success = true });
            }
            else
            {
                return Json(new { success = false, message = message.ToString()});
            }

            

        }

        /// Auther      : Anand Singh
        /// Controller  : Reports
        /// Action      : GetSchedule
        /// Description : Return json data for "Schedule of Incidental Funds/Misc Income".
        /// Date        : 18-09-2013 
        [HttpPost]
        [Audit]
        public JsonResult GetScheduleIncidental(int month, int year, int? ndcode, int rlevel, int allpiu)
        {
            //Commented By Abhishek kamble 16-Apr-2014
            //if((PMGSYSession.Current.LevelId==4) &&(rlevel==4))
            //{
            //    rlevel = 1;
            //}

            if (PMGSYSession.Current.LevelId == 5)
            {
                ndcode = PMGSYSession.Current.AdminNdCode;
            }

            var reportMaster = commomFuncObj.GetReportId(rlevel, PMGSYSession.Current.FundType, 146);//OMMAS_DEV-146 - Trainning -198

            //Modified By Abhishek kamble 10-Apr-2014 start
            ACC_RPT_REPORT_PROPERTY[] reportHeader = new ACC_RPT_REPORT_PROPERTY[4];
            string FormNumbr = "-";
            string ScheduleNo = "-";
            string FundType = "-";
            string Heading = "-";
            string Referance = "-";

            if (reportMaster != null)
            {
                reportHeader = reportMaster.ACC_RPT_REPORT_PROPERTY.ToArray<ACC_RPT_REPORT_PROPERTY>();
                FormNumbr = reportHeader[0].PROP_VALUE;
                ScheduleNo = reportHeader[1].PROP_VALUE;
                FundType = reportHeader[2].PROP_VALUE;
                Heading = reportHeader[3].PROP_VALUE;
                Referance = reportHeader[4].PROP_VALUE;
            }
            //Modified By Abhishek kamble 10-Apr-2014 end
            //ACC_RPT_REPORT_PROPERTY[] reportHeader = reportMaster.ACC_RPT_REPORT_PROPERTY.ToArray<ACC_RPT_REPORT_PROPERTY>();
            object[] arrObject = new object[] { reportMaster == null ? 0 : reportMaster.RPT_ID, ndcode, month, year, allpiu };

           ReportBAL.ReportSPBAL<PMGSY.Models.Report.Account.Schedule> objdal = new ReportBAL.ReportSPBAL<PMGSY.Models.Report.Account.Schedule>();
            List<PMGSY.Models.Report.Account.Schedule> ListSchedule = objdal.GetReportBAL(2, arrObject);

            var jsonData = new
            {
                rows = ListSchedule,
                total = 1,
                page = 1,
                records = ListSchedule.Count,
                footerData = new
                {
                    totCurrAmt = ListSchedule.Sum(m => m.CURRENT_AMT),
                    totPrevAmt = ListSchedule.Sum(m => m.PREVIOUS_AMT)
                },
                reportHeader = new
                {
                    formNumbr = FormNumbr,
                    scheduleNo = ScheduleNo,
                    fundType = FundType,
                    heading = Heading,
                    referance = Referance
                }
            };

            return Json(jsonData);
        }
        #endregion

        #region ScheduleUtilization

        [Audit]
        public ActionResult ScheduleUtilization()
        {
            ScheduleModel objSheduleModel = new ScheduleModel();
            ViewBag.ScheduleName = "Bank Authorisation Utilisation and Reconciliation Statement";
            objSheduleModel.LevelId = PMGSYSession.Current.LevelId;
            if (objSheduleModel.LevelId == 5)
            {
                ViewBag.PiuName = objReportBAL.GetNodalAgency(Convert.ToInt32(PMGSYSession.Current.ParentNDCode));
            }
            return View(objSheduleModel);
        }

        [Audit]
        public JsonResult GetUtilizationDetails(int? month, int? year, int? ndcode, int? rlevel, int? allpiu)
        {
            //added by abhishek kamble 12-dec-2013
            if ((PMGSYSession.Current.LevelId == 4) && (rlevel == 4))
            {
                rlevel = 1;
            }

            var reportMaster = commomFuncObj.GetReportId(rlevel, PMGSYSession.Current.FundType, 155);
            
            //Added By Abhishek kamble 15-jan-2014
            string formNumber = "-";
            string scheduleNo = "-";
            string reportHeading = "-";
            string fundType = "-";
            string refference = "-";
            if (reportMaster !=null)
            {
                ACC_RPT_REPORT_PROPERTY[] reportHeader = reportMaster.ACC_RPT_REPORT_PROPERTY.ToArray<ACC_RPT_REPORT_PROPERTY>();
                formNumber = reportHeader[0].PROP_VALUE;
                scheduleNo = reportHeader[1].PROP_VALUE == null ? "-" : reportHeader[1].PROP_VALUE;
                reportHeading = reportHeader[2].PROP_VALUE;
                fundType = reportHeader[3].PROP_VALUE;
                refference= reportHeader[4].PROP_VALUE;
            }
            
            object[] arrObject = new object[] { PMGSYSession.Current.FundType, month, year,ndcode, allpiu };

            ReportBAL.ReportSPBAL<ScheduleUtilization> objSchedule = new ReportBAL.ReportSPBAL<ScheduleUtilization>();
            List<ScheduleUtilization> lstSchedule=  objSchedule.GetReportBAL(14, arrObject);
            var jsonData = new
            {
                rows = lstSchedule,
                total = 0,
                page = 0,
                records = lstSchedule.Count,
                reportHeader = new
                {
                    formNumber = formNumber,
                    scheduleNo = scheduleNo,
                    reportHeading = reportHeading,
                    fundType = fundType,
                    refference = refference
                }
            };
            return Json(jsonData);

        }

        #endregion ScheduleUtilization

        #region RemittenceStatement

        [Audit]
        public ActionResult ScheduleBankRemittence()
        {
            ScheduleModel objScheduleModel = new ScheduleModel();
            objScheduleModel.LevelId = PMGSYSession.Current.LevelId;
            ViewBag.ScheduleName = "Bank Remittances Reconciliation Statement";
            if (objScheduleModel.LevelId == 5)
            {
                ViewBag.PiuName = objReportBAL.GetNodalAgency(Convert.ToInt32(PMGSYSession.Current.ParentNDCode));
            }
            return View(objScheduleModel);
        }

        [Audit]
        public ActionResult GetBankRemittenceDetails(int? Month, int? Year, int? ndCode, int? rlevel, int? allPiu)
        {

            //added by abhishek kamble 12-dec-2013
            if ((PMGSYSession.Current.LevelId == 4) && (rlevel == 4))
            {
                rlevel = 1;
            }
            var reportMaster = commomFuncObj.GetReportId(rlevel, PMGSYSession.Current.FundType, 156);
            //Added By Abhishek kamble 15-jan-2014
            string formNumber = "-";
            string scheduleNo = "-";
            string reportHeading = "-";
            string fundType = "-";
            string refference = "-";
            if (reportMaster != null)
            {
                ACC_RPT_REPORT_PROPERTY[] reportHeader = reportMaster.ACC_RPT_REPORT_PROPERTY.ToArray<ACC_RPT_REPORT_PROPERTY>();
                formNumber = reportHeader[0].PROP_VALUE;
                scheduleNo = reportHeader[1].PROP_VALUE == null ? "-" : reportHeader[1].PROP_VALUE;
                fundType = reportHeader[2].PROP_VALUE;
                reportHeading = reportHeader[3].PROP_VALUE;
                refference = reportHeader[4].PROP_VALUE;
            }

            object[] arrObject = new object[] { PMGSYSession.Current.FundType, Month, Year, ndCode, allPiu };
            ReportBAL.ReportSPBAL<ScheduleReconciliation> objReconciliation = new ReportBAL.ReportSPBAL<ScheduleReconciliation>();
            List<ScheduleReconciliation> lstReconciliation = objReconciliation.GetReportBAL(15,arrObject);

            var jsonData = new
            {
                rows = lstReconciliation,
                total = 0,
                page = 0,
                records = lstReconciliation.Count,
                reportHeader = new
                { 
                    formNumber = formNumber,
                    scheduleNo = scheduleNo,
                    reportHeading = reportHeading,
                    fundType = fundType,
                    refference = refference
                }
            };
            return Json(jsonData);
        }

        #endregion RemittenceStatement

        #region RUNNING_ACCOUNT

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [Audit]
        public ActionResult RunningAccount()
        {
            try
            {
                CommonFunctions objCommon = new CommonFunctions();
                RunningAccountViewModel model = new RunningAccountViewModel();

                ReportDAL objReportDAL = new ReportDAL();
                List<SelectListItem> lstDefault = new List<SelectListItem>();
                lstDefault.Insert(0, new SelectListItem { Value = "0", Text = "Select" });

                //Commented By Abhishek kamble 14-feb-2014
                //model.ddlYear = new SelectList(objCommon.PopulateFinancialYear(false,false),"Value","Text").ToList();
                model.ddlYear = new SelectList(objReportDAL.PopulateYears(PMGSYSession.Current.AdminNdCode,PMGSYSession.Current.LevelId), "Value", "Text").ToList();

                model.ddlMonth = lstDefault;
                //model.ddlDPIU = objCommon.PopulateDPIUOfSRRDA(PMGSYSession.Current.AdminNdCode);
                List<SelectListItem> lstPIU = objCommon.PopulateDPIUOfSRRDA(PMGSYSession.Current.AdminNdCode);
                //All PIU Option Added By Abhisek kamble 30-June-2014 To call All PIU SP
                lstPIU.Insert(0, new SelectListItem { Text = "All PIU", Value = "0" });
                model.ddlDPIU = lstPIU;

                return View(model);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        [HttpPost]
        [Audit]
        public ActionResult RunningAccountList(int? page, int? rows, string sidx, string sord,FormCollection formCollection)
        {
            int totalRecords = 0;
            try
            {
                if (ModelState.IsValid)
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
                    RunningAccountViewModel model = new RunningAccountViewModel();
                    model.Balance = Request.Params["Balance"];
                    model.Month = Convert.ToInt32(Request.Params["Month"]);
                    model.Year = Convert.ToInt32(Request.Params["Year"]);

                    if (PMGSYSession.Current.LevelId == 4)
                    {
                        if (Request.Params["ReportType"] == "S")
                        {
                            model.AdminCode = PMGSYSession.Current.AdminNdCode;
                        }
                        else if (Request.Params["ReportType"] == "D")
                        {
                            model.AdminCode = Convert.ToInt32(Request.Params["DPIU"]);
                        }
                    }
                    else if(PMGSYSession.Current.LevelId == 5)
                    {
                        model.AdminCode = PMGSYSession.Current.AdminNdCode;
                    }
                    var jsonData = new
                    {
                        rows = objReportBAL.GetRunningAccountListBAL(Convert.ToInt32(page) - 1, Convert.ToInt32(rows), sidx, sord, out totalRecords, model, Request.Params["ReportType"]),
                        total = totalRecords <= Convert.ToInt32(rows) ? 1 : totalRecords / Convert.ToInt32(rows) + 1,
                        page = Convert.ToInt32(page),
                        records = totalRecords,
                    };
                    return Json(jsonData, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return null;
                }
            }
            catch (Exception)
            {
                return null;
            }
        }

        [HttpPost]
        [Audit]
        [ValidateAntiForgeryToken]
        public ActionResult GetRunningAccountHeader(RunningAccountViewModel postModel)
        {
            commomFuncObj = new CommonFunctions();
            TransactionParams objParam = new TransactionParams();
            PMGSYEntities dbContext = new PMGSYEntities();
            ReportDAL objReportDAL = new ReportDAL();
            
            RunningAccountViewModel model = new RunningAccountViewModel();
            try
            {
                if (ModelState.IsValid)
                {
                    if (PMGSYSession.Current.LevelId == 5)
                    {
                        model.ReportType = "A";
                    }
                    else if (PMGSYSession.Current.LevelId == 4)
                    {
                        model.ReportType = "O";
                    }


                    objParam.ADMIN_ND_CODE = PMGSYSession.Current.AdminNdCode;
                    model.NodalAgency = objReportBAL.GetNodalAgency(objParam.ADMIN_ND_CODE);
                    var ReportHeader = dbContext.SP_ACC_Get_Report_Header_Information("Reports", "MonthlyAccount", PMGSYSession.Current.FundType, PMGSYSession.Current.LevelId, model.ReportType).ToList<SP_ACC_Get_Report_Header_Information_Result>().FirstOrDefault();

                    if (ReportHeader == null)
                    {
                        model.FundTypeName = commomFuncObj.GetFundName(PMGSYSession.Current.FundType); ;
                        model.ReportName = String.Empty;
                        model.ReportParagraphName = String.Empty;
                        model.ReportFormNumber = String.Empty;
                    }
                    else
                    {
                        model.FundTypeName = commomFuncObj.GetFundName(PMGSYSession.Current.FundType); ;
                        model.ReportName = ReportHeader.REPORT_NAME;
                        model.ReportParagraphName = ReportHeader.REPORT_PARAGRAPH_NAME;
                        model.ReportFormNumber = ReportHeader.REPORT_FORM_NO;
                    }
                    if (PMGSYSession.Current.LevelId == 4)
                    {
                        if (postModel.ReportType == "S")
                        {
                            model.SRRDADPIU = "SRRDA";
                        }
                        else
                        {
                            //If Added By Abhishek kamble to show label all PIU 30-June-2014
                            if (postModel.DPIUCode == 0)
                            {
                                model.DPIUName = "All PIU";
                            }
                            else
                            {
                                model.DPIUName = dbContext.ADMIN_DEPARTMENT.Where(m => m.ADMIN_ND_CODE == postModel.DPIUCode).Select(m => m.ADMIN_ND_NAME).FirstOrDefault();
                            }

                        }
                    }

                    model.NodalAgency = PMGSYSession.Current.DepartmentName;
                    List<SelectListItem> lstDefault = new List<SelectListItem>();
                    lstDefault.Insert(0, new SelectListItem { Value = "0", Text = "Select Month" });
                    //model.ddlYear = commomFuncObj.PopulateYears(true);
                    
                    //Modified By Abhishek kamble 147-feb-2014 start
                    //model.ddlYear = new SelectList(commomFuncObj.PopulateFinancialYear(false, false), "Value", "Text").ToList();
                    //model.ddlMonth = commomFuncObj.PopulateRunningMonthsByYear(postModel.Year,postModel.AdminCode,PMGSYSession.Current.LevelId);//NdCode and LvlId Parameter added by Abhishek 14-feb-2014

                    if (postModel.ReportType == "S")
                    {
                        model.ddlYear = new SelectList(objReportDAL.PopulateYears(PMGSYSession.Current.AdminNdCode, PMGSYSession.Current.LevelId), "Value", "Text").ToList();
                        //model.ddlMonth = commomFuncObj.PopulateRunningMonthsByYear(postModel.Year, PMGSYSession.Current.AdminNdCode, PMGSYSession.Current.LevelId);//NdCode and LvlId Parameter added by Abhishek 14-feb-2014                    

                        List<SelectListItem> lstMonth= new List<SelectListItem>();
                        lstMonth.Insert(0, new SelectListItem { Value = "0", Text = "Select" });
                        model.ddlMonth = lstMonth;


                    }
                    else if (postModel.ReportType == "D")
                    {

                        //Populate SRRDA Year if all PIU Is selected change by Abhisek kamble 30-June-2014 

                        if (postModel.DPIUCode == 0)
                        {
                            model.ddlYear = new SelectList(objReportDAL.PopulateYears(PMGSYSession.Current.AdminNdCode, 4), "Value", "Text").ToList();
                        }
                        else
                        {
                            model.ddlYear = new SelectList(objReportDAL.PopulateYears(postModel.DPIUCode, 5), "Value", "Text").ToList();
                        }

                        //model.ddlMonth = commomFuncObj.PopulateRunningMonthsByYear(postModel.Year, postModel.DPIUCode, 5);//NdCode and LvlId Parameter added by Abhishek 14-feb-2014                                        
                        List<SelectListItem> lstMonth = new List<SelectListItem>();
                        lstMonth.Insert(0, new SelectListItem { Value = "0", Text = "Select" });
                        model.ddlMonth = lstMonth;

                    }
                    else {
                        model.ddlYear = new SelectList(objReportDAL.PopulateYears(PMGSYSession.Current.AdminNdCode,PMGSYSession.Current.LevelId), "Value", "Text").ToList();
                        
                        List<SelectListItem> lstMonth = new List<SelectListItem>();
                        lstMonth.Insert(0, new SelectListItem { Value = "0", Text = "Select" });
                        model.ddlMonth = lstMonth;
                    }
                    //Modified By Abhishek kamble 147-feb-2014 end
                   
                    model.Month = postModel.Month;
                    model.MonthName = dbContext.MASTER_MONTH.Where(m => m.MAST_MONTH_CODE == postModel.Month).Select(m => m.MAST_MONTH_FULL_NAME).FirstOrDefault();
                    model.Year = postModel.Year;
                    model.BalanceName = (postModel.Balance == "C" ? "Credit" : "Debit");
                   // model.ddlDPIU = commomFuncObj.PopulateDPIUOfSRRDA(PMGSYSession.Current.AdminNdCode);

                    List<SelectListItem> lstPIU = commomFuncObj.PopulateDPIUOfSRRDA(PMGSYSession.Current.AdminNdCode);
                    //All PIU Option Added By Abhisek kamble 30-June-2014 To call All PIU SP
                    lstPIU.Insert(0, new SelectListItem { Text = "All PIU", Value = "0" });
                    model.ddlDPIU = lstPIU;


                    if (model.Month == 1)
                    {
                        model.PreviousMonthName = "December";
                    }
                    else
                    {
                        model.PreviousMonthName = dbContext.MASTER_MONTH.Where(m => m.MAST_MONTH_CODE == (postModel.Month - 1)).Select(m => m.MAST_MONTH_FULL_NAME).FirstOrDefault(); ;
                    }
                    return View("RunningAccount", model);
                }
                return View("RunningAccount", model);
            }
            catch (Exception)
            {
                return PartialView();
            }
        }


        #endregion

        #region REGISTER_HEADS

        [Audit]
        public ActionResult ViewRegisterFilter()
        {
            return View();
        }

        [Audit]
        public ActionResult ViewStatutoryDeduction()
        {
            RegisterViewModel model = new RegisterViewModel();
            commomFuncObj = new CommonFunctions();
            model.HeadCategoryId = 4;
            try
            {
                List<SelectListItem> lstDefault = new List<SelectListItem>();
                lstDefault = new SelectList(commomFuncObj.PopulateFinancialYear(true, false), "Value", "Text").ToList();
                model.lstFundingAgency = commomFuncObj.PopulateFundingAgency(false);
                model.lstMonths = commomFuncObj.PopulateMonths(true);
                model.lstYears = commomFuncObj.PopulateYears(true);
                model.lstFinancialYears = lstDefault;
                model.lstHeads = commomFuncObj.PopulateReportHeads(model.HeadCategoryId);
                model.lstPIU = commomFuncObj.PopulateDPIUOfSRRDA(PMGSYSession.Current.AdminNdCode);
                model.ReportTitle = "Register of Statutory Deduction";

                //Added by Vikram on 07 Jan 2014
                if (PMGSYSession.Current.AccMonth != 0)
                {
                    model.Month = PMGSYSession.Current.AccMonth;
                    model.Year = PMGSYSession.Current.AccYear;
                }
                else
                {
                    model.Month = DateTime.Now.Month;
                    model.Year = DateTime.Now.Year;
                }
                //end of change

                if (PMGSYSession.Current.LevelId == 6)
                {
                    model.lstSRRDA = commomFuncObj.PopulateNodalAgencies();
                }
                else if (PMGSYSession.Current.LevelId == 4)
                {
                    model.lstSRRDA = commomFuncObj.PopulateNodalAgencies(PMGSYSession.Current.StateCode);
                    model.SRRDACode = PMGSYSession.Current.AdminNdCode;
                }
                return View("ViewRegisterFilter", model);
            }
            catch (Exception)
            {
                return View("ViewRegisterFilter", model);
            }
        }

        [Audit]
        public ActionResult ViewRegisterDeposits()
        {
            RegisterViewModel model = new RegisterViewModel();
            commomFuncObj = new CommonFunctions();
            model.HeadCategoryId = 5;
            try
            {
                List<SelectListItem> lstDefault = new List<SelectListItem>();
                lstDefault = new SelectList(commomFuncObj.PopulateFinancialYear(true, false), "Value", "Text").ToList();
                model.lstFundingAgency = commomFuncObj.PopulateFundingAgency(false);
                model.lstMonths = commomFuncObj.PopulateMonths(true);
                model.lstYears = commomFuncObj.PopulateYears(true);
                model.lstHeads = commomFuncObj.PopulateReportHeads(model.HeadCategoryId);
                model.lstFinancialYears = lstDefault;
                model.lstPIU = commomFuncObj.PopulateDPIUOfSRRDA(PMGSYSession.Current.AdminNdCode);
                model.ReportTitle = "Register of Deposits";
                
                //Added by Vikram on 07 Jan 2014
                if (PMGSYSession.Current.AccMonth != 0)
                {
                    model.Month = PMGSYSession.Current.AccMonth;
                    model.Year = PMGSYSession.Current.AccYear;
                }
                else
                {
                    model.Month = DateTime.Now.Month;
                    model.Year = DateTime.Now.Year;
                }
                //end of change

                if (PMGSYSession.Current.LevelId == 6)
                {
                    model.lstSRRDA = commomFuncObj.PopulateNodalAgencies();
                }
                else if (PMGSYSession.Current.LevelId == 4)
                {
                    model.lstSRRDA = commomFuncObj.PopulateNodalAgencies(PMGSYSession.Current.StateCode);
                    model.SRRDACode = PMGSYSession.Current.AdminNdCode;
                }
                return View("ViewRegisterFilter", model);
            }
            catch (Exception)
            {
                return View("ViewRegisterFilter", model);
            }
        }

        [Audit]
        public ActionResult ViewMiscAdvances()
        {
            RegisterViewModel model = new RegisterViewModel();
            commomFuncObj = new CommonFunctions();
            model.HeadCategoryId = 6;
            try
            {
                List<SelectListItem> lstDefault = new List<SelectListItem>();
                lstDefault = new SelectList(commomFuncObj.PopulateFinancialYear(true, false), "Value", "Text").ToList();
                model.lstFundingAgency = commomFuncObj.PopulateFundingAgency(false);
                model.lstMonths = commomFuncObj.PopulateMonths(true);
                model.lstYears = commomFuncObj.PopulateYears(true);
                model.lstFinancialYears = lstDefault;
                model.lstHeads = commomFuncObj.PopulateReportHeads(model.HeadCategoryId);
                model.lstPIU = commomFuncObj.PopulateDPIUOfSRRDA(PMGSYSession.Current.AdminNdCode);
                model.ReportTitle = "Register of Miscellaneous Advances";

                //Added by Vikram on 07 Jan 2014
                if (PMGSYSession.Current.AccMonth != 0)
                {
                    model.Month = PMGSYSession.Current.AccMonth;
                    model.Year = PMGSYSession.Current.AccYear;
                }
                else
                {
                    model.Month = DateTime.Now.Month;
                    model.Year = DateTime.Now.Year;
                }
                //end of change

                if (PMGSYSession.Current.LevelId == 6)
                {
                    model.lstSRRDA = commomFuncObj.PopulateNodalAgencies();
                }
                else if (PMGSYSession.Current.LevelId == 4)
                {
                    model.lstSRRDA = commomFuncObj.PopulateNodalAgencies(PMGSYSession.Current.StateCode);
                    model.SRRDACode = PMGSYSession.Current.AdminNdCode;
                }
                return View("ViewRegisterFilter", model);
            }
            catch (Exception)
            {
                return View("ViewRegisterFilter", model);
            }
        }

        //public ActionResult ListRegisterDetails(int? page, int? rows, string sidx, string sord)
       // [Audit]
        public ActionResult ListRegisterDetails(FormCollection frmCollection)
        {     
            int totalRecords = 0;
            string page = frmCollection["page"];
            string rows = "999999";//frmCollection["rows"];
            string sidx = frmCollection["sidx"];
            string sord = frmCollection["sord"];
            PMGSYEntities db = new PMGSYEntities();
            try
            {
                //Adde By Abhishek kamble 29-Apr-2014 start
                using (CommonFunctions commonFunction = new CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new GridParams(Convert.ToInt32(frmCollection["page"]), Convert.ToInt32(frmCollection["rows"]), frmCollection["sidx"], frmCollection["sord"], Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                    {
                        return null;
                    }
                }
                //Adde By Abhishek kamble 29-Apr-2014 end
                RegisterViewModel model = new RegisterViewModel();
                switch (PMGSYSession.Current.LevelId)
                {
                    case 4:             
                        model.HeadId = Convert.ToInt32(Request.Params["HeadId"]);                        
                        model.FundingAgencyCode = Convert.ToInt32(Request.Params["FundingAgencyCode"]);

                        if (Request.Params["ReportType"] == "S")
                        {
                            //model.AdminCode = Convert.ToInt32(Request.Params["SRRDACode"]);
                            model.AdminCode = PMGSYSession.Current.AdminNdCode;
                            model.LevelId = PMGSYSession.Current.LevelId;
                        }
                        else if (Request.Params["ReportType"] == "D")
                        {
                            model.AdminCode = Convert.ToInt32(Request.Params["DPIUCode"]);
                            model.LevelId = 5;

                        }
                        model.Month = Convert.ToInt32(Request.Params["Month"]);
                        model.Year = Convert.ToInt32(Request.Params["Year"]);
                        if (Request.Params["Duration"] == "Y")
                        {
                            model.Year = Convert.ToInt32(Request.Params["FinancialYear"]);
                        }
                        model.StateCode = PMGSYSession.Current.StateCode;
                        break;
                    case 5:
                        model.AdminCode = PMGSYSession.Current.AdminNdCode;
                        model.FundingAgencyCode = Convert.ToInt32(Request.Params["FundingAgencyCode"]);
                        model.Month = Convert.ToInt32(Request.Params["Month"]);
                        model.Year = Convert.ToInt32(Request.Params["Year"]);
                        //model.Month = Convert.ToInt32(frmCollection["Month"]);
                        //model.Year = Convert.ToInt32(frmCollection["Year"]);
                        model.HeadId = Convert.ToInt32(Request.Params["HeadId"]);
                        model.StateCode = PMGSYSession.Current.StateCode;
                        if (Request.Params["DurationType"] == "Y")
                        {
                            model.Year = Convert.ToInt32(Request.Params["FinancialYear"]);
                        }
                        model.LevelId = PMGSYSession.Current.LevelId;
                        break;
                    case 6:
                        model.HeadId = Convert.ToInt32(Request.Params["HeadId"]);
                        model.FundingAgencyCode = Convert.ToInt32(Request.Params["FundingAgencyCode"]);
                        if (!string.IsNullOrEmpty(Request.Params["DPIUCode"]))
                        {
                            if (Convert.ToInt32(Request.Params["DPIUCode"]) != 0)
                            {
                                model.AdminCode = Convert.ToInt32(Request.Params["DPIUCode"]);
                            }
                            else
                            {
                                model.AdminCode = Convert.ToInt32(Request.Params["SRRDACode"]);
                            }
                        }

                        model.Month = Convert.ToInt32(Request.Params["Month"]);
                        model.Year = Convert.ToInt32(Request.Params["Year"]);
                        if (Request.Params["Duration"] == "Y")
                        {
                            model.Year = Convert.ToInt32(Request.Params["FinancialYear"]);
                        }
                        model.StateCode = db.ADMIN_DEPARTMENT.Where(m => m.ADMIN_ND_CODE == model.AdminCode).Select(m => m.MAST_STATE_CODE).FirstOrDefault();
                        model.LevelId = 5;
                        break;
                    default:
                        break;
                }
                model.Collaboration = db.ADMIN_DEPARTMENT.Where(m => m.ADMIN_ND_CODE == model.AdminCode).Select(m => m.MAST_AGENCY_CODE).FirstOrDefault();
                var jsonData = new
                {
                    rows = objReportBAL.GetRegisterListBAL(Convert.ToInt32(page) - 1, Convert.ToInt32(rows), sidx, sord, out totalRecords, model),
                    total = totalRecords <= Convert.ToInt32(rows) ? 1 : totalRecords / Convert.ToInt32(rows) + 1,
                    page = Convert.ToInt32(page),
                    records = totalRecords,
                };
                return Json(jsonData, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return null;
            }
        }

       
        [HttpPost]
        [Audit]
        [ValidateAntiForgeryToken]
        public ActionResult GetPostRegisterDetails(RegisterViewModel postModel)
        {
            commomFuncObj = new CommonFunctions();
            TransactionParams objParam = new TransactionParams();
            PMGSYEntities dbContext = new PMGSYEntities();
            RegisterViewModel model = new RegisterViewModel();
            string action = string.Empty;
            try
            {
                if (ModelState.IsValid)
                {
                    if (PMGSYSession.Current.LevelId == 5)
                    {
                        model.ReportType = "A";
                    }
                    else if (PMGSYSession.Current.LevelId == 4)
                    {
                        model.ReportType = "O";
                    }

                    switch (postModel.HeadCategoryId)
                    {
                        case 4:
                            model.ReportTitle = "Register of Statutory Deduction";
                            action = "ViewStatutoryDeduction";
                            break;
                        case 5:
                            model.ReportTitle = "Register of Deposits";
                            action = "ViewRegisterDeposits";
                            break;
                        case 6:
                            model.ReportTitle = "Register of Miscellaneous Advances";
                            action = "ViewMiscAdvances";
                            break;
                        default:
                            break;
                    }

                    var ReportHeader = dbContext.SP_ACC_Get_Report_Header_Information("Reports", action, PMGSYSession.Current.FundType, PMGSYSession.Current.LevelId, "O").ToList<SP_ACC_Get_Report_Header_Information_Result>().FirstOrDefault();

                    if (ReportHeader == null)
                    {
                        model.FundTypeName = commomFuncObj.GetFundName(PMGSYSession.Current.FundType); ;
                        model.ReportName = String.Empty;
                        model.ReportParagraphName = String.Empty;
                        model.ReportFormNumber = String.Empty;
                    }
                    else
                    {
                        model.FundTypeName = commomFuncObj.GetFundName(PMGSYSession.Current.FundType); ;
                        model.ReportName = ReportHeader.REPORT_NAME;
                        model.ReportParagraphName = ReportHeader.REPORT_PARAGRAPH_NAME;
                        model.ReportFormNumber = ReportHeader.REPORT_FORM_NO;
                    }
                    if (PMGSYSession.Current.LevelId != 6)
                    {
                        objParam.ADMIN_ND_CODE = PMGSYSession.Current.AdminNdCode;
                        model.NodalAgency = objReportBAL.GetNodalAgency(objParam.ADMIN_ND_CODE);
                    }
                    else
                    {
                        objParam.ADMIN_ND_CODE = postModel.SRRDACode;
                        model.NodalAgency = objReportBAL.GetNodalAgency(objParam.ADMIN_ND_CODE);
                    }

                    if (PMGSYSession.Current.LevelId == 4)
                    {
                        if (postModel.ReportType == "S")
                        {
                            model.SRRDADPIU = "SRRDA";
                        }
                        else
                        {
                            model.lstPIU = commomFuncObj.PopulateDPIUOfSRRDA(PMGSYSession.Current.AdminNdCode);
                            model.DPIUName = dbContext.ADMIN_DEPARTMENT.Where(m => m.ADMIN_ND_CODE == postModel.DPIUCode).Select(m => m.ADMIN_ND_NAME).FirstOrDefault();
                        }
                    }
                    else if (PMGSYSession.Current.LevelId == 6)
                    {
                        if (postModel.DPIUCode == 0)
                        {
                            model.DPIUName = "All DPIU";
                        }
                        else
                        {
                            model.DPIUName = dbContext.ADMIN_DEPARTMENT.Where(m => m.ADMIN_ND_CODE == postModel.DPIUCode).Select(m => m.ADMIN_ND_NAME).FirstOrDefault();
                            model.DPIUCode = postModel.DPIUCode;
                        }
                    }


                    if (PMGSYSession.Current.LevelId != 6)
                    {
                        model.NodalAgency = PMGSYSession.Current.DepartmentName;
                    }
                    List<SelectListItem> lstDefault = new List<SelectListItem>();
                    lstDefault.Insert(0, new SelectListItem { Value = "0", Text = "Select Month" });
                    model.lstYears = commomFuncObj.PopulateYears(true);
                    model.lstMonths = commomFuncObj.PopulateMonths(postModel.Year);
                    if (postModel.DurationType == "M")
                    {
                        model.Month = postModel.Month;
                        model.MonthName = dbContext.MASTER_MONTH.Where(m => m.MAST_MONTH_CODE == postModel.Month).Select(m => m.MAST_MONTH_FULL_NAME).FirstOrDefault();
                        model.Year = postModel.Year;
                    }
                    else
                    {
                        model.FinancialYear = postModel.FinancialYear;
                    }
                    model.lstHeads = commomFuncObj.PopulateReportHeads(postModel.HeadCategoryId);
                    model.lstFundingAgency = commomFuncObj.PopulateFundingAgency(false);
                    model.LevelId = PMGSYSession.Current.LevelId;
                    model.HeadName = dbContext.ACC_MASTER_HEAD.Where(m => m.HEAD_ID == postModel.HeadId).Select(m => m.HEAD_NAME).FirstOrDefault();
                    if (PMGSYSession.Current.LevelId == 4)
                    {
                        model.lstSRRDA = commomFuncObj.PopulateNodalAgencies(PMGSYSession.Current.StateCode);
                    }
                    else
                    {
                        model.lstSRRDA = commomFuncObj.PopulateNodalAgencies();
                    }
                    lstDefault = new SelectList(commomFuncObj.PopulateFinancialYear(true, false), "Value", "Text").ToList();
                    if (PMGSYSession.Current.LevelId == 4)
                    {
                        model.SRRDACode = PMGSYSession.Current.AdminNdCode;
                    }
                    else
                    {
                        model.SRRDACode = postModel.SRRDACode;
                        model.StateName = dbContext.MASTER_STATE.Where(m => m.MAST_STATE_CODE == dbContext.ADMIN_DEPARTMENT.Where(a => a.ADMIN_ND_CODE == postModel.SRRDACode).Select(a => a.MAST_STATE_CODE).FirstOrDefault()).Select(m => m.MAST_STATE_NAME).FirstOrDefault();
                    }
                    model.DPIUCode = postModel.DPIUCode;
                    model.lstFinancialYears = lstDefault;
                    if (PMGSYSession.Current.LevelId == 6)
                    {
                        model.lstPIU = commomFuncObj.PopulateDPIUOfSRRDA(model.SRRDACode);
                        model.lstPIU.Insert(0, new SelectListItem { Value = "0",Text = "All DPIU"});
                    }
                    else
                    {
                        model.lstPIU = commomFuncObj.PopulateDPIUOfSRRDA(PMGSYSession.Current.AdminNdCode);
                    }
                    
                    return View("ViewRegisterFilter", model);
                }
                else
                {
                    List<SelectListItem> lstDefault = new List<SelectListItem>();
                    lstDefault = new SelectList(commomFuncObj.PopulateFinancialYear(true, false), "Value", "Text").ToList();
                    model.lstFundingAgency = commomFuncObj.PopulateFundingAgency(false);
                    model.lstMonths = commomFuncObj.PopulateMonths(true);
                    model.lstYears = commomFuncObj.PopulateYears(true);
                    model.lstHeads = commomFuncObj.PopulateReportHeads(model.HeadCategoryId);
                    model.lstFinancialYears = lstDefault;
                    model.lstPIU = commomFuncObj.PopulateDPIUOfSRRDA(PMGSYSession.Current.AdminNdCode);
                    //model.ReportTitle = "Register of Deposits";
                    model.lstHeads = commomFuncObj.PopulateReportHeads(postModel.HeadCategoryId);
                    if (PMGSYSession.Current.LevelId == 6)
                    {
                        model.lstSRRDA = commomFuncObj.PopulateNodalAgencies();
                    }
                    else if (PMGSYSession.Current.LevelId == 4)
                    {
                        model.lstSRRDA = commomFuncObj.PopulateNodalAgencies(PMGSYSession.Current.StateCode);
                        model.SRRDACode = PMGSYSession.Current.AdminNdCode;
                    }
                    return View("ViewRegisterFilter", model);
                }
            }
            catch (Exception)
            {
                return View("ViewRegisterFilter", model);
            }
        }

        [Audit]
        public ActionResult ImprestSettlementDetails()
        {
            ImprestSettlementViewModel model = new ImprestSettlementViewModel();
            commomFuncObj = new CommonFunctions();
            try
            {
                model.lstFinancialYears = new SelectList(commomFuncObj.PopulateFinancialYear(true, false).ToList(),"Value","Text").ToList();
                if (PMGSYSession.Current.LevelId == 4)
                {
                    model.SrrdaAdminCode = PMGSYSession.Current.AdminNdCode;
                    model.lstSrrda = commomFuncObj.PopulateNodalAgencies(PMGSYSession.Current.StateCode);
                    model.lstDpiu = commomFuncObj.PopulateDPIUOfSRRDA(PMGSYSession.Current.AdminNdCode);
                }
                else
                {
                    model.lstSrrda = commomFuncObj.PopulateNodalAgencies();
                    model.lstDpiu.Insert(0, new SelectListItem { Value="0",Text = "Select DPIU"});
                }
                model.LevelId = PMGSYSession.Current.LevelId;
                return View(model);
            }
            catch (Exception)
            {
                return View(model);
            }
        }

        [HttpPost]
        [Audit]
        public ActionResult ListImprestSettlements(int? page, int? rows, string sidx, string sord)
        {
            int totalRecords = 0;
            try
            {
                //Added By Abhishek kamble 30-Apr-2014 Start
                using (CommonFunctions commonFunction = new CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new GridParams(page, rows, sidx, sord, Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                    {
                        return null;
                    }
                }
                //Added By Abhishek kamble 30-Apr-2014 end

                if (ModelState.IsValid)
                {
                    ImprestSettlementViewModel model = new ImprestSettlementViewModel();
                    
                    if (!String.IsNullOrEmpty(Request.Params["Year"]))
                    {
                        model.FinancialYear = Convert.ToInt16(Request.Params["Year"]);
                    }
                    
                    if (!String.IsNullOrEmpty(Request.Params["AdminCode"]))
                    {
                        model.AdminCode = Convert.ToInt32(Request.Params["AdminCode"]);
                    }

                    if (PMGSYSession.Current.LevelId == 5)
                    {
                        model.AdminCode = PMGSYSession.Current.AdminNdCode;
                    }

                    var jsonData = new
                    {
                        rows = objReportBAL.ListImprestSettlementsBAL(Convert.ToInt32(page) - 1, Convert.ToInt32(rows), sidx, sord, out totalRecords, model),
                        total = totalRecords <= Convert.ToInt32(rows) ? 1 : totalRecords / Convert.ToInt32(rows) + 1,
                        page = Convert.ToInt32(page),
                        records = totalRecords,
                    };
                    return Json(jsonData, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return null;
                }
            }
            catch (Exception)
            {
                return null;
            }
        }

        [HttpPost]
        public ActionResult GetImprestHeaderInfo(ImprestSettlementViewModel _postModel)
        {
            try
            {
                PMGSYEntities db = new PMGSYEntities();
                ImprestSettlementViewModel model = new ImprestSettlementViewModel();
                if (ModelState.IsValid)
                {

                    if (PMGSYSession.Current.LevelId == 5)
                    {
                        model.ReportLevel = "D";
                        model.StateName = db.ADMIN_DEPARTMENT.Where(m => m.ADMIN_ND_CODE == PMGSYSession.Current.AdminNdCode).Select(m => m.MASTER_STATE.MAST_STATE_NAME).FirstOrDefault();
                        model.DPIUName = db.ADMIN_DEPARTMENT.Where(m => m.ADMIN_ND_CODE == PMGSYSession.Current.AdminNdCode).Select(m => m.ADMIN_ND_NAME).FirstOrDefault();
                        model.FinancialYear = _postModel.FinancialYear;
                        model.ReportFormNumber = "";
                        model.FundTypeName = (PMGSYSession.Current.FundType == "P" ? "PMGSY PROGRAMME FUND" : (PMGSYSession.Current.FundType == "A" ? "PMGSY ADMINISTRTATIVE FUND" : "PMGSY MAINTENANCE FUND"));
                        model.ReportParagraphName = "";
                        model.ReportName = "IMPREST REGISTER";
                    }
                    else
                    {

                        if (_postModel.ReportLevel == "D")
                        {
                            model.ReportLevel = "D";
                            model.StateName = db.ADMIN_DEPARTMENT.Where(m => m.ADMIN_ND_CODE == _postModel.DpiuAdminCode).Select(m => m.MASTER_STATE.MAST_STATE_NAME).FirstOrDefault();
                            model.DPIUName = db.ADMIN_DEPARTMENT.Where(m => m.ADMIN_ND_CODE == _postModel.DpiuAdminCode).Select(m => m.ADMIN_ND_NAME).FirstOrDefault();
                            model.FinancialYear = _postModel.FinancialYear;
                            model.ReportFormNumber = "";
                            model.FundTypeName = (PMGSYSession.Current.FundType == "P" ? "PMGSY PROGRAMME FUND" : (PMGSYSession.Current.FundType == "A" ? "PMGSY ADMINISTRTATIVE FUND" : "PMGSY MAINTENANCE FUND"));
                            model.ReportParagraphName = "";
                            model.ReportName = "IMPREST REGISTER";
                        }
                        else
                        {
                            model.ReportLevel = "S";
                            model.StateName = db.ADMIN_DEPARTMENT.Where(m => m.ADMIN_ND_CODE == _postModel.SrrdaAdminCode).Select(m => m.MASTER_STATE.MAST_STATE_NAME).FirstOrDefault();
                            model.DPIUName = "";
                            model.FinancialYear = _postModel.FinancialYear;
                            model.ReportFormNumber = "";
                            model.FundTypeName = (PMGSYSession.Current.FundType == "P" ? "PMGSY PROGRAMME FUND" : (PMGSYSession.Current.FundType == "A" ? "PMGSY ADMINISTRTATIVE FUND" : "PMGSY MAINTENANCE FUND"));
                            model.ReportParagraphName = "";
                            model.ReportName = "IMPREST REGISTER";
                            model.NodalAgency = db.ADMIN_DEPARTMENT.Where(m => m.ADMIN_ND_CODE == _postModel.SrrdaAdminCode).Select(m => m.ADMIN_ND_NAME).FirstOrDefault().ToString();
                        }
                    }
                }
                return PartialView(model);
            }
            catch (Exception)
            {
                return PartialView();
            }
        }


        #endregion

        #region SchedulefundReconcilation

        [Audit]
        public ActionResult FundReconciliationView()
        {
            ScheduleModel objScheduleModel = new ScheduleModel();
            objScheduleModel.LevelId = PMGSYSession.Current.LevelId;
            ViewBag.ScheduleName = "Fund Reconciliation Between SRRDA & PIU(State)";
            if (objScheduleModel.LevelId == 5)
            {
                ViewBag.PiuName = objReportBAL.GetNodalAgency(Convert.ToInt32(PMGSYSession.Current.ParentNDCode));
            }
            return View(objScheduleModel);
          
        }
        [Audit]
        public ActionResult GetFundReconciliationDetails(int? srrdaNDCode, int? month, int? year, int? rlevel)
        {
            //added by abhishek kamble 12-dec-2013
            if ((PMGSYSession.Current.LevelId == 4) && (rlevel == 4))
            {
                rlevel = 1;
            } 

            var reportMaster = commomFuncObj.GetReportId(rlevel, PMGSYSession.Current.FundType, 153);

              //Added By Abhishek kamble 15-jan-2014
            string formNumber = "-";
            string scheduleNo = "-";
            string reportHeading = "-";
            string fundType = "-";
            string refference = "-";
            if (reportMaster != null)
            {
                ACC_RPT_REPORT_PROPERTY[] reportHeader = reportMaster.ACC_RPT_REPORT_PROPERTY.ToArray<ACC_RPT_REPORT_PROPERTY>();
                formNumber = reportHeader[0].PROP_VALUE;
                scheduleNo = reportHeader[1].PROP_VALUE == null ? "-" : reportHeader[1].PROP_VALUE;
                reportHeading = reportHeader[2].PROP_VALUE;
                fundType = reportHeader[3].PROP_VALUE;
                refference = reportHeader[4].PROP_VALUE;
            }
            object[] arrObject = new object[] { PMGSYSession.Current.FundType, month, year, srrdaNDCode};
            ReportBAL.ReportSPBAL<ScheduleFundReconciliation> objFundReconciliation = new ReportBAL.ReportSPBAL<ScheduleFundReconciliation>();
            List<ScheduleFundReconciliation> lstFundReconciliation = objFundReconciliation.GetReportBAL(18, arrObject);

            //var  reconcileData = lstFundReconciliation.Select(details => new
            //{

            //    cell = new 
            //    {
            //        details.Sno,
            //        details.Particulars,
            //        details.IsAmt,
            //        details.InnerAmt,
            //        details.OuterAmt
            //    }

            //}).ToArray();

            var jsonData = new
            {
                rows = lstFundReconciliation,
                total = 0,
                page = 0,
                records = lstFundReconciliation.Count,
                reportHeader = new
                {
                    formNumber = formNumber,
                    scheduleNo = scheduleNo,
                    reportHeading = reportHeading,
                    fundType = fundType,
                    refference = refference
                }
            };
            return Json(jsonData);
        }


        #endregion ScheduleFundReconciliation

        #region ScheduleCurrentLiabilities

        [Audit]
        public ActionResult CurrLiabilitiesView(BalanceSheet BalanceSheetViewModel)
        {
            ScheduleModel objScheduleModel = new ScheduleModel();
            objScheduleModel.LevelId = PMGSYSession.Current.LevelId;
            ViewBag.ScheduleName = "Schedule Of Current Liabilities";
            if (objScheduleModel.LevelId == 5)
            {
                ViewBag.PiuName = objReportBAL.GetNodalAgency(Convert.ToInt32(PMGSYSession.Current.ParentNDCode));
            }

            if (PMGSYSession.Current.AccYear != 0)
            {
                objScheduleModel.Month = PMGSYSession.Current.AccMonth;
                objScheduleModel.Year = PMGSYSession.Current.AccYear;
            }
            else
            {
                //Added by Abhishek kamble 20-dec-2013  start
                if (Convert.ToInt16(BalanceSheetViewModel.SelectedMonth) == 0)
                {
                    objScheduleModel.Month =Convert.ToInt16(System.DateTime.Now.Month);
                    objScheduleModel.Year = Convert.ToInt16(System.DateTime.Now.Year);
                }
                else {
                    objScheduleModel.Month = Convert.ToInt16(BalanceSheetViewModel.SelectedMonth);
                    objScheduleModel.Year = Convert.ToInt16(BalanceSheetViewModel.SelectedYear);
                }                
            }
                objScheduleModel.ReportType = BalanceSheetViewModel.IsMonthlyYearly;

                if (BalanceSheetViewModel.AdminCode != null)
                {
                    objScheduleModel.Piu = BalanceSheetViewModel.AdminCode.Value;
                }
                if (BalanceSheetViewModel.StateAdminCode != null)
                {
                    objScheduleModel.State = BalanceSheetViewModel.StateAdminCode;
                }

                if (BalanceSheetViewModel.ReportLevel == 'S')//State
                {
                    objScheduleModel.ReportLevel = 4;
                }
                else if (BalanceSheetViewModel.ReportLevel == 'O')//SRRDA
                {
                    objScheduleModel.ReportLevel = 1;
                }
                else if (BalanceSheetViewModel.ReportLevel == 'A')//All DPIU
                {
                    objScheduleModel.ReportLevel = 2;
                }
                //Added by Abhishek kamble 20-dec-2013  end
            return View(objScheduleModel);
        }

        [Audit]
        public ActionResult GetCurrentLibDetails(int month, int year, Int64 ndcode, int rlevel, int allpiu)
        {
            var reportMaster = commomFuncObj.GetReportId(rlevel, PMGSYSession.Current.FundType, 148);//OMMAS_DEV-148 - Trainning -187
            ACC_RPT_REPORT_PROPERTY[] reportHeader = reportMaster.ACC_RPT_REPORT_PROPERTY.ToArray<ACC_RPT_REPORT_PROPERTY>();
            object[] arrObject = new object[] { reportMaster.RPT_ID, ndcode, month, year, allpiu };

            ReportBAL.ReportSPBAL<ScheduleCurrentLiabilities> objdal = new ReportBAL.ReportSPBAL<ScheduleCurrentLiabilities>();
            List<ScheduleCurrentLiabilities> ListSchedule = objdal.GetReportBAL(19, arrObject);

            var jsonData = new
            {
                rows = ListSchedule,
                total = 1,
                page = 1,
                records = ListSchedule.Count,
                footerData = new
                {   //Modified by abhishek kamble 2-dec-2013
                    totCurrAmt = ListSchedule.Where(m=>m.ITEM_ID!=5).Sum(m => m.CURRENT_AMT),
                    totPrevAmt = ListSchedule.Where(m=>m.ITEM_ID!=5).Sum(m => m.PREVIOUS_AMT)
                },
                reportHeader = new
                {
                    formNumbr = reportHeader[0].PROP_VALUE,
                    scheduleNo = reportHeader[1].PROP_VALUE,
                    fundType = reportHeader[2].PROP_VALUE,
                    heading = reportHeader[3].PROP_VALUE,
                    referance = reportHeader[4].PROP_VALUE
                }
            };

            return Json(jsonData);
        }

        #endregion ScheduleCurrentLiabilities

        #region ScheduleFundTransferred

        [Audit]
        public ActionResult FundTransferredView(BalanceSheet BalanceSheetViewModel)
        {
            ScheduleModel objScheduleModel = new ScheduleModel();
            objScheduleModel.LevelId = PMGSYSession.Current.LevelId;
            ViewBag.ScheduleName = "Schedule of Fund Received";
            if (objScheduleModel.LevelId == 5)
            {
                ViewBag.PiuName = objReportBAL.GetNodalAgency(Convert.ToInt32(PMGSYSession.Current.ParentNDCode));
            }
            
            //Added by Abhishek kamble 20-dec-2013  start
            //objScheduleModel.Month = BalanceSheetViewModel.Month;
            //new code added by Vikram on 06 Jan 2014
            if (PMGSYSession.Current.AccMonth != 0)
            {
                objScheduleModel.Month = PMGSYSession.Current.AccMonth;
                //objScheduleModel.Year = BalanceSheetViewModel.Year;
                objScheduleModel.Year = PMGSYSession.Current.AccYear;
            }
            else
            {
                //end of change
                objScheduleModel.Month = Convert.ToInt16(BalanceSheetViewModel.SelectedMonth);
                //objScheduleModel.Year = BalanceSheetViewModel.Year;
                objScheduleModel.Year = Convert.ToInt16(BalanceSheetViewModel.SelectedYear);
            }



            objScheduleModel.ReportType = BalanceSheetViewModel.IsMonthlyYearly;

            if (BalanceSheetViewModel.AdminCode != null)
            {
                objScheduleModel.Piu = BalanceSheetViewModel.AdminCode.Value;
            }
            if (BalanceSheetViewModel.StateAdminCode != null)
            {
                objScheduleModel.State = BalanceSheetViewModel.StateAdminCode;
            }

            if (BalanceSheetViewModel.ReportLevel == 'S')//State
            {
                objScheduleModel.ReportLevel = 4;
            }
            else if (BalanceSheetViewModel.ReportLevel == 'O')//SRRDA
            {
                objScheduleModel.ReportLevel = 1;
            }
            else if (BalanceSheetViewModel.ReportLevel == 'A')//All DPIU
            {
                objScheduleModel.ReportLevel = 2;
            }
            //Added by Abhishek kamble 20-dec-2013  end

            return View(objScheduleModel);
        }

        [Audit]
        public ActionResult GetFundTransferredList(int month, int year, int ndcode, int rlevel, int allpiu)
        {
            var reportMaster = commomFuncObj.GetReportId(rlevel, PMGSYSession.Current.FundType, 147); //OMMAS_DEV-147 - Trainning -217
            ACC_RPT_REPORT_PROPERTY[] reportHeader = reportMaster.ACC_RPT_REPORT_PROPERTY.ToArray<ACC_RPT_REPORT_PROPERTY>();
            object[] arrObject = new object[] { reportMaster.RPT_ID, ndcode, month, year, allpiu };

            ReportBAL.ReportSPBAL<ScheduleFundTransferred> objdal = new ReportBAL.ReportSPBAL<ScheduleFundTransferred>();
            List<ScheduleFundTransferred> ListSchedule = objdal.GetReportBAL(20, arrObject);

            // new change done by Vikram to display 0 in place of null value

            ListSchedule = (from item in ListSchedule
                            select new ScheduleFundTransferred
                            {
                                CURRENT_AMT = item.CURRENT_AMT == null ? (Decimal)0.00 : item.CURRENT_AMT,
                                GROUP_ID = item.GROUP_ID,
                                ITEM_HEADING = item.ITEM_HEADING,
                                ITEM_ID = item.ITEM_ID,
                                LINK = item.LINK,
                                PREVIOUS_AMT = item.PREVIOUS_AMT == null ? (Decimal)0.00 : item.PREVIOUS_AMT,
                                SORT_ORDER = item.SORT_ORDER
                            }).ToList();

            //end of change

            var jsonData = new
            {
                rows = ListSchedule,
                total = 1,
                page = 1,
                records = ListSchedule.Count,
                footerData = new
                {
                    //Modified by Abhishek kamble 13-jan-2014
                    //totCurrAmt = ListSchedule.Sum(m => m.CURRENT_AMT),
                    //totPrevAmt = ListSchedule.Sum(m => m.PREVIOUS_AMT)
                    totCurrAmt = ListSchedule.Where(m => m.ITEM_HEADING == "<b> Balance </b>" || m.ITEM_HEADING == "<b> Closing Balance </b>").Sum(m => m.CURRENT_AMT),
                    totPrevAmt = ListSchedule.Where(m => m.ITEM_HEADING == "<b> Balance </b>" || m.ITEM_HEADING == "<b> Closing Balance </b>").Sum(m => m.PREVIOUS_AMT)
                },
                reportHeader = new
                {
                    formNumbr = reportHeader[0].PROP_VALUE,
                    scheduleNo = reportHeader[1].PROP_VALUE,
                    heading = reportHeader[2].PROP_VALUE,
                    fundType = reportHeader[3].PROP_VALUE,
                    referance = reportHeader[4].PROP_VALUE
                }
            };

            return Json(jsonData);
        }


        #endregion ScheduleFundTransferred

        #region Schedule Of Current Assets

        public PartialViewResult ScheduleCurrentAssetsFilter(AccountFilterModel filterViewModel)
        {
            //Added By abhishek kamble 21-dec-2013
            int RptLevel = 0;
            int agencyCode = 0;

            ReportFilter objParam = new ReportFilter
            {
                //Modified by abhishek kamble 20-dec-2013
                //Month = Convert.ToInt16(DateTime.Now.Month),
                //Year = Convert.ToInt16(DateTime.Now.Year),   
                Month = filterViewModel.Month,
                Year = filterViewModel.Year,

                AdminNdCode = PMGSYSession.Current.AdminNdCode,
                FundType = PMGSYSession.Current.FundType,
                LevelId = PMGSYSession.Current.LevelId
            };


            //if(PMGSYSession.Current.LevelId==5)
            //{
            //       objParam.AdminNdCode = PMGSYSession.Current.ParentNDCode,
            //}

            //Added by abhishek kamble 20-dec-2013 start

            if (PMGSYSession.Current.AccMonth != 0)
            {
                objParam.Month = PMGSYSession.Current.AccMonth;
            }
            else if (filterViewModel.Month == 0)
            {
                objParam.Month = Convert.ToInt16(DateTime.Now.Month);
            }

            if (PMGSYSession.Current.AccYear != 0)
            {
                objParam.Year = PMGSYSession.Current.AccYear;
            }
            else if (filterViewModel.Year == 0)
            {
                objParam.Year = Convert.ToInt16(DateTime.Now.Year);
            }
            if (filterViewModel.ReportLevel != 0)
            {
                RptLevel = filterViewModel.ReportLevel;
            }

            if (PMGSYSession.Current.LevelId == 4)
            {
                agencyCode = PMGSYSession.Current.AdminNdCode;
            }
            else if (filterViewModel.Agency != 0)
            {
                agencyCode = filterViewModel.Agency;
            }
            //Added by abhishek kamble 20-dec-2013 end

            PMGSY.Models.Report.Account.AccountFilterModel filterModel = new Models.Report.Account.AccountFilterModel
            {
                //Month = objParam.Month,
                //ListMonth = commomFuncObj.PopulateMonths(objParam.Month),
                //ListYear = commomFuncObj.PopulateYears(objParam.Year),
                //Year = objParam.Year,
                //Piu = objParam.AdminNdCode,
                //LevelId = objParam.LevelId,
                //Agency = 0,
                //ReportLevel = 4,
                //ReportType = 2 ,

                //Modified by abhishek kamble 20-dec-2013
                //Month = objParam.Month,
                ListMonth = commomFuncObj.PopulateMonths(objParam.Month),
                ListYear = commomFuncObj.PopulateYears(objParam.Year),
                //Year = objParam.Year,
                
                LevelId = objParam.LevelId,

                //Modified by abhishek kamble 20-dec-2013
                //Piu = objParam.AdminNdCode,                              
                Piu = filterViewModel.Piu,                              
                //Agency = 0,
                Agency = agencyCode,                
                //ReportLevel = 4,
                ReportLevel = RptLevel,
                //ReportType = 2
                ReportType = filterViewModel.ReportType
            };

            if (filterModel.LevelId == 4)
            {
                filterModel.ListPiu = commomFuncObj.PopulateDPIUOfSRRDA(objParam.AdminNdCode);
                filterModel.ListPiu.Insert(0, (new SelectListItem { Text = "All DPIU", Value = "0", Selected = true }));
                filterModel.ListAgency = commomFuncObj.PopulateNodalAgencies(PMGSYSession.Current.StateCode);
            }
            else if (filterModel.LevelId == 6)
            {
                //filterModel.ListPiu = commomFuncObj.PopulateDPIUOfSRRDA();

                filterModel.ListPiu = new List<SelectListItem>();
                filterModel.ListPiu.Insert(0, (new SelectListItem { Text = "All DPIU", Value = "0", Selected = true }));
                filterModel.ListAgency = commomFuncObj.PopulateNodalAgencies();

            }
            return PartialView(filterModel);

        }

        [Audit]
        public ActionResult ScheduleCurrentAssets(BalanceSheet BalanceSheetViewModel)
        {
            ScheduleModel objScheduleModel = new ScheduleModel();
            objScheduleModel.LevelId = PMGSYSession.Current.LevelId;

            ViewBag.ScheduleName = "Schedule Of Current Assets";

            if (objScheduleModel.LevelId == 5)
            {
                ViewBag.PiuName = objReportBAL.GetNodalAgency(Convert.ToInt32(PMGSYSession.Current.ParentNDCode));
            }

            //Added by Abhishek kamble 21-dec-2013  start

            if (PMGSYSession.Current.AccMonth != 0)
            {
                objScheduleModel.Month = PMGSYSession.Current.AccMonth;
                objScheduleModel.Year = PMGSYSession.Current.AccYear;
            }
            else
            {
                //objScheduleModel.Month = BalanceSheetViewModel.Month;
                objScheduleModel.Month = Convert.ToInt16(BalanceSheetViewModel.SelectedMonth);
                //objScheduleModel.Year = BalanceSheetViewModel.Year;
                objScheduleModel.Year = Convert.ToInt16(BalanceSheetViewModel.SelectedYear);
            }
            objScheduleModel.ReportType = BalanceSheetViewModel.IsMonthlyYearly;

            if (BalanceSheetViewModel.AdminCode != null)
            {
                objScheduleModel.Piu = BalanceSheetViewModel.AdminCode.Value;
            }
            if (BalanceSheetViewModel.StateAdminCode != null)
            {
                objScheduleModel.State = BalanceSheetViewModel.StateAdminCode;
            }

            if (BalanceSheetViewModel.ReportLevel == 'S')//State
            {
                objScheduleModel.ReportLevel = 4;
            }
            else if (BalanceSheetViewModel.ReportLevel == 'O')//SRRDA
            {
                objScheduleModel.ReportLevel = 1;
            }
            else if (BalanceSheetViewModel.ReportLevel == 'A')//All DPIU
            {
                objScheduleModel.ReportLevel = 2;
            }
            //Added by Abhishek kamble 21-dec-2013  end

            return View(objScheduleModel);
        }

        public ActionResult GetCurrentAssetsDetails(int? Month, int? Year, int? ndCode, int? rlevel, int? allPiu, int? duration)
        {
            try
            {
                string flag = string.Empty;


                if (duration == 1)//1- annually 2- Monthly
                {
                    Month = 0;
                }

                if (rlevel == 4)
                {
                    flag = "A";
                }
                else if (rlevel == 1)
                {
                    flag = "S";
                }
                else if (rlevel == 2)
                {
                    flag = "D";
                }

                //get report header from table
                var reportMaster = commomFuncObj.GetReportId(rlevel, PMGSYSession.Current.FundType, 152); //OMMAS_DEV-152 - Trainning -191

                //Modified By Abhishek kamble 21-Apr-2014 start
                ACC_RPT_REPORT_PROPERTY[] reportHeader = new ACC_RPT_REPORT_PROPERTY[4];
                string FormNumber = "-";
                string ScheduleNo = "-";
                string FundType = "-";
                string Heading = "-";
                string Referance = "-";

                if (reportMaster != null)
                {
                    reportHeader = reportMaster.ACC_RPT_REPORT_PROPERTY.ToArray<ACC_RPT_REPORT_PROPERTY>();
                    FormNumber = reportHeader[0].PROP_VALUE;
                    ScheduleNo = reportHeader[1].PROP_VALUE;
                    FundType = reportHeader[2].PROP_VALUE;
                    Heading = reportHeader[3].PROP_VALUE;
                    Referance = reportHeader[4].PROP_VALUE;
                }



                //get report data from SP
                object[] arrObject = new object[] { PMGSYSession.Current.FundType, Month, Year, ndCode, allPiu, flag };
                ReportBAL.ReportSPBAL<ScheduleCurrentAssets> objReconciliation = new ReportBAL.ReportSPBAL<ScheduleCurrentAssets>();
                List<ScheduleCurrentAssets> lstCurrentAssets = objReconciliation.GetReportBAL(16, arrObject);


                foreach (var item in lstCurrentAssets)
                {
                    if (item.AmountFlag == "N")
                    {
                        item.Amount = null;
                    }

                    if (item.AmountFlag == "Y" && item.Amount == null)
                    {
                        item.Amount = 0;
                    }
                }

                var jsonData = new
                {
                    rows = lstCurrentAssets,
                    total = 0,
                    page = 0,
                    records = lstCurrentAssets.Count,
                    //Modified By Abhishek kamble 21-Apr-2014 start
                    reportHeader = new
                    {
                        formNumber = FormNumber,
                        scheduleNo = ScheduleNo,
                        fundType = FundType,
                        reportHeading = Heading,
                        refference = Referance
                    }
                };
                return Json(jsonData);
            }
            catch
            {
                return null;
            }
        }

        #endregion Shedule Of Current Assets

        #region Schedule Of Durable Assets

        [Audit]
        public ActionResult ScheduleDurableAssets(BalanceSheet BalanceSheetViewModel)
        {
            ScheduleModel objScheduleModel = new ScheduleModel();
            objScheduleModel.LevelId = PMGSYSession.Current.LevelId;

            ViewBag.ScheduleName = "Schedule Of Durable Assets";

            if (objScheduleModel.LevelId == 5)
            {
                ViewBag.PiuName = objReportBAL.GetNodalAgency(Convert.ToInt32(PMGSYSession.Current.ParentNDCode));
            }

            //Added by Vikram on 08 Jan 2014
            if (PMGSYSession.Current.AccMonth != 0)
            {
                objScheduleModel.Month = PMGSYSession.Current.AccMonth;
                objScheduleModel.Year = PMGSYSession.Current.AccYear;
            }
            else
            {
                //end of change
                //Added by Abhishek kamble 20-dec-2013  start
                //objScheduleModel.Month = BalanceSheetViewModel.Month;
                objScheduleModel.Month = Convert.ToInt16(BalanceSheetViewModel.SelectedMonth);
                //objScheduleModel.Year = BalanceSheetViewModel.Year;
                objScheduleModel.Year = Convert.ToInt16(BalanceSheetViewModel.SelectedYear);
            }
            objScheduleModel.ReportType = BalanceSheetViewModel.IsMonthlyYearly;

            if (BalanceSheetViewModel.AdminCode != null)
            {
                objScheduleModel.Piu = BalanceSheetViewModel.AdminCode.Value;
            }
            if (BalanceSheetViewModel.StateAdminCode != null)
            {
                objScheduleModel.State = BalanceSheetViewModel.StateAdminCode;
            }

            if (BalanceSheetViewModel.ReportLevel == 'S')//State
            {
                objScheduleModel.ReportLevel = 4;
            }
            else if (BalanceSheetViewModel.ReportLevel == 'O')//SRRDA
            {
                objScheduleModel.ReportLevel = 1;
            }
            else if (BalanceSheetViewModel.ReportLevel == 'A')//All DPIU
            {
                objScheduleModel.ReportLevel = 2;
            }
            //Added by Abhishek kamble 20-dec-2013  end

            return View(objScheduleModel);
        }

        public JsonResult GetDurableAssetsDetails(int? Month, int? Year, int? ndCode, int? rlevel, int? allPiu, int? duration)
        {
            try
            {
                string flag = string.Empty;


                if (duration == 1)//1- annually 2- Monthly
                {
                    Month = 0;
                }

                if (rlevel == 4)
                {
                    flag = "A";
                }
                else if (rlevel == 1)
                {
                    flag = "S";
                }
                else if (rlevel == 2)
                {
                    flag = "D";
                }

                //get report header from table
                var reportMaster = commomFuncObj.GetReportId(rlevel, PMGSYSession.Current.FundType, 154); //154 is Menu id for durable assets menu.

                ACC_RPT_REPORT_PROPERTY[] reportHeader = reportMaster.ACC_RPT_REPORT_PROPERTY.ToArray<ACC_RPT_REPORT_PROPERTY>();

                //get report data from SP
                object[] arrObject = new object[] { PMGSYSession.Current.FundType, Month, Year, ndCode, allPiu, flag };
                ReportBAL.ReportSPBAL<ScheduleDurableAssets> objReconciliation = new ReportBAL.ReportSPBAL<ScheduleDurableAssets>();
                List<ScheduleDurableAssets> lstDurableAssets = objReconciliation.GetReportBAL(17, arrObject);


                //Added By Abhishek kamble 11-Mar-2014 start

                lstDurableAssets = lstDurableAssets.OrderBy(o => o.Centarl_State).ToList<ScheduleDurableAssets>();

                //Added By Abhishek kamble 11-Mar-2014 end

                Array data = lstDurableAssets.Select(assetDetails => new
                {
                    cell = new[]{
                        
                            assetDetails.Centarl_State.ToString(),
                            assetDetails.HEAD_NAME,
                            assetDetails.OBAmt.ToString(),
                            assetDetails.MonthlyAmt.ToString(),
                            assetDetails.TotAmt.ToString(),
                            String.Empty,
                            assetDetails.TotAmt.ToString(),
                        }
                }).ToArray();


                //return lstAccountDetails.Select(itemDetails => new
                //{
                //    cell = new[] 
                //{
                //    itemDetails.HEAD_CODE == null?string.Empty:itemDetails.HEAD_CODE.ToString(),
                //    //(itemDetails.HEAD_CODE == "11.01")||(itemDetails.HEAD_CODE == "11.02")?(itemDetails.HEAD_NAME == null?string.Empty:itemDetails.HEAD_NAME.ToString())+"Status:"+(itemDetails.HEAD_COMP_PROGRESS== null?string.Empty:itemDetails.HEAD_COMP_PROGRESS.ToString())+"Agency("+(itemDetails.IMS_COLLABORATION == null?string.Empty:itemDetails.IMS_COLLABORATION.ToString())+")":itemDetails.HEAD_NAME.ToString(),
                //    itemDetails.HEAD_NAME == null?string.Empty:itemDetails.HEAD_NAME.ToString(),
                //    itemDetails.HEAD_COMP_PROGRESS== null?string.Empty:itemDetails.HEAD_COMP_PROGRESS.ToString(),
                //    itemDetails.IMS_COLLABORATION == null?string.Empty:itemDetails.IMS_COLLABORATION.ToString(),
                //    itemDetails.OB_BALANCE_AMT == null?string.Empty:itemDetails.OB_BALANCE_AMT.ToString(),
                //    itemDetails.MONTHLY_BALANCE_AMT== null?string.Empty:itemDetails.MONTHLY_BALANCE_AMT.ToString(),
                //    (itemDetails.MONTHLY_BALANCE_AMT + itemDetails.OB_BALANCE_AMT) == 0?"0.0":(itemDetails.MONTHLY_BALANCE_AMT + itemDetails.OB_BALANCE_AMT).ToString()
                //}
                //}).ToArray();


                var jsonData = new
                {
                    rows = data,
                    total = 0,
                    page = 0,
                    records = lstDurableAssets.Count,
                    reportHeader = new
                    {
                        formNumber = reportHeader[0].PROP_VALUE,
                        scheduleNo = reportHeader[1].PROP_VALUE,
                        fundType = reportHeader[2].PROP_VALUE,
                        reportHeading = reportHeader[3].PROP_VALUE,
                        refference = reportHeader[4].PROP_VALUE
                    }
                };
                return Json(jsonData);
            }
            catch
            {
                return null;
            }
        }

        #endregion Shedule Of Durable Assets

        #region Income and Expenditure

            /// <summary>
            /// Shows Income And Expenditure Report Header and Grid
            /// </summary>
            /// <returns></returns>
        [Audit]
        public ActionResult IncomeAndExpenditureDetails()
            {
                IncomeAndExpenditureModel incomeAndExpenditureModel = new IncomeAndExpenditureModel();
                incomeAndExpenditureModel.LevelId = PMGSYSession.Current.LevelId;

                ViewBag.ScheduleName = "Income and Expenditure";

                
                if (incomeAndExpenditureModel.LevelId == 5)
                {
                    ViewBag.PiuName = objReportBAL.GetNodalAgency(Convert.ToInt32(PMGSYSession.Current.ParentNDCode));
                }
                return View(incomeAndExpenditureModel);        
            }

            /// <summary>
            ///  Display Report Form
            /// </summary>
            /// <returns></returns>
            public PartialViewResult ReportForm()
            {
                ReportFilter objParam = new ReportFilter
                {
                    Month = PMGSYSession.Current.AccMonth != 0 ? PMGSYSession.Current.AccMonth : Convert.ToInt16(DateTime.Now.Month),
                    Year = PMGSYSession.Current.AccYear != 0 ? PMGSYSession.Current.AccYear : Convert.ToInt16(DateTime.Now.Year),
                    AdminNdCode = PMGSYSession.Current.AdminNdCode,
                    FundType = PMGSYSession.Current.FundType,
                    LevelId = PMGSYSession.Current.LevelId
                };

                PMGSY.Models.Report.ReportFormModel reportModel = new Models.Report.ReportFormModel
                {
                    Month = objParam.Month,
                    ListMonth = commomFuncObj.PopulateMonths(objParam.Month),
                    ListYear = commomFuncObj.PopulateYears(objParam.Year),
                    Year = objParam.Year,
                    Piu = objParam.AdminNdCode,
                    LevelId = objParam.LevelId,
                    Agency = 0,
                    ReportLevel = 4,
                    ReportType = 2
                };
                if (reportModel.LevelId == 4)
                {
                    reportModel.ListPiu = commomFuncObj.PopulateDPIUOfSRRDA(objParam.AdminNdCode);
                    reportModel.ListPiu.Insert(0, (new SelectListItem { Text = "All DPIU", Value = "0", Selected = true }));
                    reportModel.ListAgency = commomFuncObj.PopulateNodalAgencies(PMGSYSession.Current.StateCode);
                }
                else if (reportModel.LevelId == 6)
                {
                    reportModel.ListPiu = new List<SelectListItem>();
                    reportModel.ListPiu.Insert(0, (new SelectListItem { Text = "All DPIU", Value = "0", Selected = true }));
                    reportModel.ListAgency = commomFuncObj.PopulateNodalAgencies();

                }
                return PartialView(reportModel);
            }
             
            /// <summary>
            /// Get Data to display On Income And Expenditure Grid.
            /// </summary>
            /// <param name="Month"></param>
            /// <param name="Year"></param>
            /// <param name="ndCode"></param>
            /// <param name="rlevel"></param>
            /// <param name="allPiu"></param>
            /// <param name="duration"></param>
            /// <returns></returns>
            [Audit]
            public ActionResult ListIncomeAndExpenditureDetails(int? Month, int? Year, int? ndCode, int? rlevel, int? allPiu, int? duration)
            {
                try
                {
                    string flag = string.Empty;
                    long totalRecords;
                    int LevelId=0;
                    string ReportType=string.Empty;

                    PMGSYEntities dbContext = new PMGSYEntities();

                    if (duration == 1)//1- annually 2- Monthly
                    {
                        Month = 0;
                    }

                    //get report header from table
                    var lstHeader = new SP_ACC_Get_Report_Header_Information_Result();

                    if (rlevel == 4)
                    {
                        LevelId = 4;
                        ReportType = "A";
                    }
                    else if (rlevel == 1)
                    {
                        LevelId = 4;
                        ReportType = "S";
                    }else if(rlevel==2)
                    {
                        LevelId = 5;
                        ReportType = "O";
                    }

                    lstHeader = dbContext.SP_ACC_Get_Report_Header_Information("Reports", "IncomeAndExpenditureDetails", PMGSYSession.Current.FundType,LevelId,ReportType).FirstOrDefault();

                    string ReportName = String.Empty;
                    string ReportNumber = String.Empty;
                    string ReportPara = String.Empty;
                    string FundName = String.Empty;

                    if (lstHeader != null)
                    {
                        ReportName = lstHeader.REPORT_NAME;
                        ReportNumber = lstHeader.REPORT_FORM_NO;
                        ReportPara = lstHeader.REPORT_PARAGRAPH_NAME;
                        FundName = commomFuncObj.GetFundName(PMGSYSession.Current.FundType);
                    }

                    //get report data from SP
                    var jsonData = new
                    {
                        rows = objReportBAL.ListIncomeAndExpenditureDetails(Month,Year,ndCode,rlevel,allPiu,duration,out totalRecords),
                        total = 0,
                        page = 0,
                        records = totalRecords,
                        reportHeader = new
                        {
                            formNumber = ReportNumber,
                            fundType = FundName,
                            reportHeading = ReportName,
                            refference = ReportPara
                        }
                    };
                    return Json(jsonData);
                }
                catch
                {
                    return null;
                }
            }

        #endregion Income and Expenditure

        #region COMMON_METHODS

        //[HttpPost]
        //public JsonResult PopulateDPIU(string id)
        //{
        //    int adminCode = 0;
        //    List<SelectListItem> lstDPIU = new List<SelectListItem>();
        //    try
        //    {
        //        if (int.TryParse(id, out adminCode))
        //        {
        //            lstDPIU = commomFuncObj.PopulateDPIUOfSRRDA(adminCode);
        //            lstDPIU.RemoveAt(0);
        //            lstDPIU.Insert(0, new SelectListItem { Text = "All DPIU", Value = "0" });
        //            return Json(lstDPIU);
        //        }
        //        else
        //        {
        //            return null;
        //        }
        //    }
        //    catch (Exception)
        //    {
        //        return null;
        //    }
        //}

            [HttpPost]
            [Audit]
        public JsonResult PopulateYears()
        {
            List<SelectListItem> lstYears = new List<SelectListItem>();
            try
            {
                if (PMGSYSession.Current.AccYear != 0)
                {
                    return Json(commomFuncObj.PopulateYears(PMGSYSession.Current.AccYear));
                }
                else {
                    return Json(commomFuncObj.PopulateYears(System.DateTime.Now.Year));
                }
            }
            catch (Exception)
            {
                return null;
            }
        }

        [HttpPost]
        [Audit] 
        public JsonResult PopulateFinancialYears()
        {
            List<SelectListItem> lstYears = new List<SelectListItem>();
            try
            {
                SelectList lstYear = commomFuncObj.PopulateFinancialYear(true);

                if (PMGSYSession.Current.AccYear != 0)
                {
                    lstYear = new SelectList(lstYear, "Value","Text", PMGSYSession.Current.AccYear);
                    //lstYears = new SelectListItem();
                }
                return Json(lstYear);
                //return Json(commomFuncObj.PopulateFinancialYear(true));

            }
            catch (Exception)
            {
                return null;
            }
        }

        [HttpPost]
        [Audit]
        public ActionResult PopulateRunningMonthsByYear(string id)
        {
            try
            {
                string[] Parameters = id.Split('$');
                int year = Convert.ToInt32(Parameters[0]);

                if (PMGSYSession.Current.LevelId == 5)
                {
                    return Json(commomFuncObj.PopulateRunningMonthsByYear(year, PMGSYSession.Current.AdminNdCode,PMGSYSession.Current.LevelId));//Parameter Modified by Abhishek 14-feb-2014
                }
                else
                {   
                    int AdminNdCode = 0;
                    int LevelId = 0;

                    if (Parameters[1] == "S")
                    {
                        AdminNdCode = PMGSYSession.Current.AdminNdCode;
                        LevelId = PMGSYSession.Current.LevelId;
                    }
                    else
                    {
                        //Populate SRRDA Year for All PIU new change by Abhishek kamble 30-June-2014
                        if (Convert.ToInt32(Parameters[2]) == 0)
                        {
                            AdminNdCode = PMGSYSession.Current.AdminNdCode;
                            LevelId = PMGSYSession.Current.LevelId;
                        }
                        else
                        {
                            AdminNdCode = Convert.ToInt32(Parameters[2]);
                            LevelId = 5;
                        }
                    }
                    commomFuncObj = new CommonFunctions();
                    return Json(commomFuncObj.PopulateRunningMonthsByYear(year, AdminNdCode, LevelId));//Parameter Modified by Abhishek 14-feb-2014
                }
            }
            catch (Exception)
            {
                return null;
            }
        }

        //New Method Added By Abhishek kamble to Puplate Years for State/ DPIU
        [HttpPost]
        [Audit]
        public ActionResult PopulateRunningAccYear(string id)
        {
            try                                             
            {                  

                ReportDAL objReportDAL = new ReportDAL();
               
                string[] Parameters = id.Split('$');

                //int year = Convert.ToInt32(Parameters[0]);
                int AdminNdCode = 0;
                int LevelId = 0;

                if (Parameters[0] == "S")
                {
                    AdminNdCode = PMGSYSession.Current.AdminNdCode;
                    LevelId = PMGSYSession.Current.LevelId;
                }
                else
                {
                    //Populate SRRDA Year for All PIU new change by Abhishek kamble 30-June-2014
                    if (Convert.ToInt32(Parameters[1]) == 0)
                    {
                        AdminNdCode = PMGSYSession.Current.AdminNdCode;
                        LevelId = PMGSYSession.Current.LevelId;
                    }
                    else
                    {
                        AdminNdCode = Convert.ToInt32(Parameters[1]);
                        LevelId = 5;
                    }
                } 
                return Json(objReportDAL.PopulateYears(AdminNdCode,LevelId));
                //new SelectList(objReportDAL.PopulateYears(PMGSYSession.Current.AdminNdCode, PMGSYSession.Current.LevelId), "Value", "Text").ToList();

            }
            catch (Exception)
            {
                return null;
            }
        }


        #endregion

        #region UPDATE_SESSION

        /// <summary>
        /// for updatin the Session month and year
        /// </summary>
        /// <returns>json success = true/false</returns>
        public JsonResult UpdateAccountSession()
        {
            try
            {
                if (!String.IsNullOrEmpty(Request.Params["Month"]))
                {
                    PMGSYSession.Current.AccMonth = Convert.ToInt16(Request.Params["Month"]);
                }

                if (!String.IsNullOrEmpty(Request.Params["Year"]))
                {
                    PMGSYSession.Current.AccYear = Convert.ToInt16(Request.Params["Year"]);
                }

                return Json(new { Success = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(new { Success = false }, JsonRequestBehavior.AllowGet);
            }
        }

        #endregion

        public PartialViewResult ShowBalanceSheetSchedules()
        {
           
            return PartialView();
        }

    }


}
