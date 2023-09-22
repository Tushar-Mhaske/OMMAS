
#region HEADER
/*
* Project Id:

 * Project Name:OMMAS-II

 * File Name: AccountController.cs

 * Author : Abhishek Kamble

 * Creation Date : 26 ‎November ‎2014, ‏‎11:23:25 AM

 * Desc : This class is used as controller to perform Display layout and SSRS Account Report.
 */
#endregion

using PMGSY.Areas.AccountReports.Models;
using PMGSY.Common;
using PMGSY.Controllers;
using PMGSY.Extensions;
//using PMGSY.Models;
using PMGSY.Models.Common;
//using PMGSY.Models.Report;
//PMGSY.Areas.AccountReports.Models.CBSingleModel
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;

using PMGSY.DAL.AccountsReports;
using PMGSY.BAL.AccountReports;
//using PMGSY.Models.Report;
using PMGSY.Models;

using PMGSY.BAL.Reports;
using PMGSY.DAL.Payment;

namespace PMGSY.Areas.AccountReports.Controllers
{
    [RequiredAuthentication]
    [RequiredAuthorization]
    public partial class AccountController : Controller
    {
        private IReportBAL objReportBAL = new ReportBAL();

        IAccountReportsDAL objAccountDAL = new AccountReportsDAL();
        IAccountReportsBAL objAccountBAL = new AccountReportsBAL();
        //
        // GET: /AccountReports/Account/


        #region Index Method
        public ActionResult Index()
        {
            return View();
        }
        #endregion

        #region Balance Sheet
        public ActionResult BalanceSheetLayout()
        {
            BalanceSheet balanceSheet = new BalanceSheet();
            CommonFunctions commonFunctions = new CommonFunctions();

            balanceSheet.Month = (short)DateTime.Now.Month;
            balanceSheet.MonthList = commonFunctions.PopulateMonths(DateTime.Now.Month);
            balanceSheet.Year = (short)DateTime.Now.Year;
            balanceSheet.YearList = commonFunctions.PopulateYears(DateTime.Now.Year);

            //populate DPIU
            List<SelectListItem> lstDpiu = new List<SelectListItem>();
            lstDpiu.Insert(0, (new SelectListItem { Text = "Select DPIU", Value = "0" }));
            balanceSheet.DPIUList = lstDpiu;

            //populate SRRDA
            if (PMGSYSession.Current.LevelId == 6)
            {
                balanceSheet.NodalAgencyList = commonFunctions.PopulateNodalAgencies();
            }
            else if (PMGSYSession.Current.LevelId == 4)
            {
                balanceSheet.NodalAgencyList = commonFunctions.PopulateNodalAgencies(PMGSYSession.Current.StateCode);
                balanceSheet.StateAdminCode = PMGSYSession.Current.AdminNdCode;
            }
            else if (PMGSYSession.Current.LevelId == 5)
            {
                balanceSheet.NodalAgencyList = commonFunctions.PopulateNodalAgencies(PMGSYSession.Current.StateCode);
                balanceSheet.StateAdminCode = PMGSYSession.Current.ParentNDCode.Value;
            }
            return View(balanceSheet);
        }

        /// <summary>
        /// Render data in Bal Sheet Report
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult BalanceSheetReport(string id)
        {
            BalanceSheet balanceSheet = new BalanceSheet();
            try
            {
                string[] splitParams = id.Split('$');
                int LevelId = Convert.ToInt32(splitParams[0]);

                //State Admin Code
                balanceSheet.StateAdminCode = Convert.ToInt32(splitParams[2]);
                balanceSheet.Year = Convert.ToInt16(splitParams[5]);
                balanceSheet.FundType = PMGSYSession.Current.FundType;

                if (splitParams[7] == "Y")
                {
                    balanceSheet.Month = 0;
                }
                else
                {
                    balanceSheet.Month = Convert.ToInt16(splitParams[4]);
                }
                //All State 
                if (LevelId == 6) //All State
                {
                    //if (PMGSYSession.Current.FundType == "P")
                    //{
                    //    balanceSheet.PINTRptID = 9;
                    //}
                    //else if (PMGSYSession.Current.FundType == "A")
                    //{
                    //    balanceSheet.PINTRptID = 7;
                    //}
                    //else if (PMGSYSession.Current.FundType == "M")
                    //{
                    //    balanceSheet.PINTRptID = 1;
                    //}

                    if (splitParams[1] == "S")//State Report Id
                    {
                        if (PMGSYSession.Current.FundType == "P")
                        {
                            balanceSheet.PINTRptID = 9;
                        }
                        else if (PMGSYSession.Current.FundType == "A")
                        {
                            balanceSheet.PINTRptID = 7;
                        }
                        else if (PMGSYSession.Current.FundType == "M")
                        {
                            balanceSheet.PINTRptID = 1;
                        }
                    }
                    else if (splitParams[1] == "O")   //Srrda
                    {
                        if (PMGSYSession.Current.FundType == "P")
                        {
                            balanceSheet.PINTRptID = 2;
                        }
                        else if (PMGSYSession.Current.FundType == "A")
                        {
                            balanceSheet.PINTRptID = 5;
                        }
                        else if (PMGSYSession.Current.FundType == "M")
                        {
                            balanceSheet.PINTRptID = 4;
                        }
                    }
                    else if (splitParams[1] == "A")    //All PIU
                    {
                        if (PMGSYSession.Current.FundType == "P")
                        {
                            balanceSheet.PINTRptID = 8;
                        }
                        else if (PMGSYSession.Current.FundType == "A")
                        {
                            balanceSheet.PINTRptID = 6;
                        }
                        else if (PMGSYSession.Current.FundType == "M")
                        {
                            balanceSheet.PINTRptID = 3;
                        }
                    }

                    ////Is All PIU
                    //if ((splitParams[1] == "A") && (Convert.ToInt32(splitParams[3]) != 0))
                    //{
                    //    balanceSheet.IsAllPiu = 1;
                    //    balanceSheet.AdminCode = Convert.ToInt32(splitParams[3]);
                    //}
                    //else
                    //{
                    //    balanceSheet.IsAllPiu = 0;
                    //    balanceSheet.AdminCode = Convert.ToInt32(splitParams[2]);
                    //}

                    //Is All PIU
                    if ((splitParams[1] == "A") && (Convert.ToInt32(splitParams[3]) == 0))
                    {
                        balanceSheet.IsAllPiu = 1;
                        balanceSheet.AdminCode = Convert.ToInt32(splitParams[2]);
                    }
                    else
                    {
                        balanceSheet.IsAllPiu = 0;
                        balanceSheet.AdminCode = Convert.ToInt32(splitParams[3]);


                        if (balanceSheet.AdminCode == 0)
                        {
                            balanceSheet.AdminCode = Convert.ToInt32(splitParams[2]);
                        }
                        else
                        {
                            balanceSheet.AdminCode = Convert.ToInt32(splitParams[3]);
                        }
                    }

                    balanceSheet.ReportLevel = Convert.ToChar(splitParams[1]);
                    balanceSheet.levelId = 4;//
                }
                //SRRDA 
                if (LevelId == 4) //srrda
                {
                    //if (PMGSYSession.Current.FundType == "P")
                    //{
                    //    balanceSheet.PINTRptID = 2;
                    //}
                    //else if (PMGSYSession.Current.FundType == "A")
                    //{
                    //    balanceSheet.PINTRptID = 5;
                    //}
                    //else if (PMGSYSession.Current.FundType == "M")
                    //{
                    //    balanceSheet.PINTRptID = 4;
                    //}

                    if (splitParams[1] == "S")//State Report Id
                    {
                        if (PMGSYSession.Current.FundType == "P")
                        {
                            balanceSheet.PINTRptID = 9;
                        }
                        else if (PMGSYSession.Current.FundType == "A")
                        {
                            balanceSheet.PINTRptID = 7;
                        }
                        else if (PMGSYSession.Current.FundType == "M")
                        {
                            balanceSheet.PINTRptID = 1;
                        }
                    }
                    else if (splitParams[1] == "O")   //Srrda
                    {
                        if (PMGSYSession.Current.FundType == "P")
                        {
                            balanceSheet.PINTRptID = 2;
                        }
                        else if (PMGSYSession.Current.FundType == "A")
                        {
                            balanceSheet.PINTRptID = 5;
                        }
                        else if (PMGSYSession.Current.FundType == "M")
                        {
                            balanceSheet.PINTRptID = 4;
                        }
                    }
                    else if (splitParams[1] == "A")    //All PIU
                    {
                        if (PMGSYSession.Current.FundType == "P")
                        {
                            balanceSheet.PINTRptID = 8;
                        }
                        else if (PMGSYSession.Current.FundType == "A")
                        {
                            balanceSheet.PINTRptID = 6;
                        }
                        else if (PMGSYSession.Current.FundType == "M")
                        {
                            balanceSheet.PINTRptID = 3;
                        }
                    }
                    ////Is All PIU
                    //if ((splitParams[1] == "A") && (Convert.ToInt32(splitParams[3]) != 0))
                    //{
                    //    balanceSheet.IsAllPiu = 1;
                    //    balanceSheet.AdminCode = Convert.ToInt32(splitParams[3]);
                    //}
                    //else
                    //{
                    //    balanceSheet.IsAllPiu = 0;
                    //    balanceSheet.AdminCode = Convert.ToInt32(splitParams[2]);
                    //}

                    //Is All PIU
                    if ((splitParams[1] == "A") && (Convert.ToInt32(splitParams[3]) == 0))
                    {
                        balanceSheet.IsAllPiu = 1;
                        balanceSheet.AdminCode = Convert.ToInt32(splitParams[2]);
                    }
                    else
                    {
                        balanceSheet.IsAllPiu = 0;
                        balanceSheet.AdminCode = Convert.ToInt32(splitParams[3]);


                        if (balanceSheet.AdminCode == 0)
                        {
                            balanceSheet.AdminCode = Convert.ToInt32(splitParams[2]);
                        }
                        else
                        {
                            balanceSheet.AdminCode = Convert.ToInt32(splitParams[3]);
                        }
                    }

                    balanceSheet.ReportLevel = Convert.ToChar(splitParams[1]);
                    balanceSheet.levelId = 4;
                }

                //piu
                if (LevelId == 5) //srrda
                {
                    if (PMGSYSession.Current.FundType == "P")
                    {
                        balanceSheet.PINTRptID = 8;
                    }
                    else if (PMGSYSession.Current.FundType == "A")
                    {
                        balanceSheet.PINTRptID = 6;
                    }
                    else if (PMGSYSession.Current.FundType == "M")
                    {
                        balanceSheet.PINTRptID = 3;
                    }
                    balanceSheet.IsAllPiu = 0;
                    balanceSheet.ReportLevel = 'O';
                    balanceSheet.levelId = 5;
                    balanceSheet.AdminCode = Convert.ToInt32(splitParams[3]);
                }

                return View(balanceSheet);
            }
            catch
            {
                return View(balanceSheet);
            }

        }

        #endregion Balance Sheet

        #region Monthly Acount Report

        public ActionResult MonthlyAcountLayout()
        {
            MonthlyAccountModel monthlyAccountModel = new MonthlyAccountModel();
            CommonFunctions commonFunctions = new CommonFunctions();

            //populate Month 
            ViewBag.ddlMonth = commonFunctions.PopulateMonths(DateTime.Now.Month);
            //populate Year
            ViewBag.ddlYear = commonFunctions.PopulateYears(DateTime.Now.Year);
            //populate State SRRDA         

            if (PMGSYSession.Current.LevelId == 6)
            {
                monthlyAccountModel.lstStates = commonFunctions.PopulateNodalAgencies();
            }
            else if (PMGSYSession.Current.LevelId == 4)
            {
                monthlyAccountModel.lstStates = commonFunctions.PopulateNodalAgencies(PMGSYSession.Current.StateCode);
                monthlyAccountModel.State = PMGSYSession.Current.AdminNdCode;

            }
            else if (PMGSYSession.Current.LevelId == 5)
            {
                monthlyAccountModel.lstStates = commonFunctions.PopulateNodalAgencies(PMGSYSession.Current.StateCode);
                monthlyAccountModel.State = PMGSYSession.Current.ParentNDCode.Value;
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

            if (PMGSYSession.Current.LevelId == 5)
            {
                monthlyAccountModel.Dpiu = Convert.ToInt16(PMGSYSession.Current.AdminNdCode);
            }
            return View("MonthlyAcountLayout", monthlyAccountModel);

        }

        /// <summary>
        /// Render data in Monthly Account Report
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult MonthlyAcountLayoutReport(string id)
        {
            MonthlyAccountModel monthlyAccountModel = new MonthlyAccountModel();
            try
            {
                string[] splitParams = id.Split('$');
                int LevelId = Convert.ToInt32(splitParams[0]);
                string StateSrrdaPiu = splitParams[1];
                int Level = 1;
                //int AdminNdCode = Convert.ToInt32(splitParams[3]);

                //All State 
                if (LevelId == 6) //All State
                {
                    if (StateSrrdaPiu == "STATE")
                    {
                        Level = 1;
                        monthlyAccountModel.AdminNdCode = Convert.ToInt32(splitParams[2]);
                        monthlyAccountModel.ISSelf = 1;
                        monthlyAccountModel.AllPIU = 1;
                    }
                    else if (StateSrrdaPiu == "SRRDA")
                    {
                        Level = 4;
                        monthlyAccountModel.AdminNdCode = Convert.ToInt32(splitParams[2]);

                        monthlyAccountModel.ISSelf = 1;
                        monthlyAccountModel.AllPIU = 0;
                    }
                    else if (StateSrrdaPiu == "DPIU" && Convert.ToInt16(splitParams[3]) == 0)
                    {
                        Level = 5;
                        monthlyAccountModel.AdminNdCode = Convert.ToInt32(splitParams[2]);
                        monthlyAccountModel.ISSelf = 0;
                        monthlyAccountModel.AllPIU = 1;
                    }
                    else if (StateSrrdaPiu == "DPIU" && Convert.ToInt16(splitParams[3]) != 0)
                    {
                        Level = 5;
                        monthlyAccountModel.AdminNdCode = Convert.ToInt32(splitParams[3]);

                        monthlyAccountModel.ISSelf = 1;
                        monthlyAccountModel.AllPIU = 0;
                    }
                }

                //SRRDA
                if (LevelId == 4) //All State
                {
                    if (StateSrrdaPiu == "STATE")
                    {
                        Level = 1;
                        monthlyAccountModel.AdminNdCode = Convert.ToInt32(splitParams[2]);
                        monthlyAccountModel.ISSelf = 1;
                        monthlyAccountModel.AllPIU = 1;
                    }
                    if (StateSrrdaPiu == "SRRDA")
                    {
                        Level = 4;
                        monthlyAccountModel.AdminNdCode = Convert.ToInt32(splitParams[2]);

                        monthlyAccountModel.ISSelf = 1;
                        monthlyAccountModel.AllPIU = 0;
                    }
                    else if (StateSrrdaPiu == "DPIU" && Convert.ToInt16(splitParams[3]) == 0)
                    {
                        Level = 5;
                        monthlyAccountModel.AdminNdCode = Convert.ToInt32(splitParams[2]);
                        monthlyAccountModel.ISSelf = 0;
                        monthlyAccountModel.AllPIU = 1;
                    }
                    else if (StateSrrdaPiu == "DPIU" && Convert.ToInt16(splitParams[3]) != 0)
                    {
                        Level = 5;
                        monthlyAccountModel.AdminNdCode = Convert.ToInt32(splitParams[3]);
                        monthlyAccountModel.ISSelf = 1;
                        monthlyAccountModel.AllPIU = 0;
                    }
                }
                //PIU
                if (LevelId == 5) //All State
                {
                    Level = 5;
                    monthlyAccountModel.AdminNdCode = Convert.ToInt32(splitParams[3]);

                    monthlyAccountModel.ISSelf = 1;
                    monthlyAccountModel.AllPIU = 0;
                }

                monthlyAccountModel.levelId = Level;
                monthlyAccountModel.monthlyStateSrrdaDpiu = StateSrrdaPiu;

                monthlyAccountModel.State = Convert.ToInt32(splitParams[2]);
                monthlyAccountModel.Dpiu = Convert.ToInt16(splitParams[3]);

                monthlyAccountModel.Month = Convert.ToInt16(splitParams[4]);
                monthlyAccountModel.Year = Convert.ToInt16(splitParams[5]);
                monthlyAccountModel.FundType = PMGSYSession.Current.FundType;
                monthlyAccountModel.CreditDebit = splitParams[6];


                return View(monthlyAccountModel);
            }
            catch
            {
                return View(monthlyAccountModel);
            }

        }

        #endregion  Monthly Acount Report

        //Added By Abhishek kamble 2-July-2014
        #region Cashbook Report

        /// <summary>
        /// Display Cashbook Report Filter
        /// </summary>
        /// <returns></returns>
        public ActionResult CashBookLayout()
        {
            PMGSY.Areas.AccountReports.Models.CBSingleModel cbSingle = new CBSingleModel();
            CommonFunctions commomFuncObj = new CommonFunctions();

            if (PMGSYSession.Current.AccMonth != 0)
            {
                cbSingle.Month = PMGSYSession.Current.AccMonth;
                cbSingle.Year = PMGSYSession.Current.AccYear;
            }
            else
            {
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
                ViewBag.DPIU = lstDPIU;
            }
            //Added by abhishek kamble 3-oct-2013 end 
            return View(cbSingle);
        }


        [HttpPost]
        public ActionResult CashBookReport(CBSingleModel cbSingle)
        {
            //Added by abhishek kamble 3-oct-2013 start 
            CommonFunctions commomFuncObj = new CommonFunctions();
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
            PMGSY.Models.Report.ReportFilter objParam = new PMGSY.Models.Report.ReportFilter();
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
                else if (cbSingle.SRRDA_DPIU == "D")
                {
                    objParam.AdminNdCode = cbSingle.DPIU;
                }
            }
            else
            { //login DPIU                                         
                objParam.AdminNdCode = PMGSYSession.Current.AdminNdCode;
            }
            //added by Abhishek kamble 3-oct-2013 end

            objParam.FundType = PMGSYSession.Current.FundType;

            cbSingle.SRRDA_DPIU = SRRDA_DPIU;

            if ((SRRDA_DPIU == "D" || SRRDA_DPIU == "S") && (DPIU_NdCode == 0))
            {
                //cbSingle.DistrictDepartment = "-";
            }
            else
            {
                cbSingle.DPIU = DPIU_NdCode;
            }


            //Set ADMIN_ND_CODE
            cbSingle.ADMIN_ND_CODE = objParam.AdminNdCode;

            //set Level
            short LevelId = 0;
            if (objParam.Selection == "D" && objParam.Dpiu != 0)
            {
                LevelId = 5;
            }
            else
            {
                if (PMGSYSession.Current.LevelId == 6)
                {
                    LevelId = 4;
                }
                else
                {
                    LevelId = PMGSYSession.Current.LevelId;
                }
            }
            cbSingle.LvlId = LevelId;
            return View(cbSingle);
        }


        #endregion Cashbook Report

        #region ledger SSRS Report

        /// <summary>
        /// action to return the ledger Filter page
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Audit]
        public ActionResult LedgerLayout(LedgerModel ledgerModel)
        {
            CommonFunctions commomFuncObj = new CommonFunctions();
            try
            {
                if (ledgerModel.HEAD != null)
                {
                    ledgerModel.SelectedHead = Convert.ToInt32(ledgerModel.HEAD);
                }
                if (PMGSYSession.Current.AccMonth != 0)
                {
                    ledgerModel.MONTH_LIST = commomFuncObj.PopulateMonths(PMGSYSession.Current.AccMonth);
                    ledgerModel.YEAR_LIST = commomFuncObj.PopulateYears(PMGSYSession.Current.AccYear);
                    ledgerModel.MONTH = PMGSYSession.Current.AccMonth;
                    ledgerModel.YEAR = PMGSYSession.Current.AccYear;
                }
                else
                {
                    ledgerModel.MONTH_LIST = commomFuncObj.PopulateMonths(DateTime.Now.Month);
                    ledgerModel.YEAR_LIST = commomFuncObj.PopulateYears(DateTime.Now.Year);
                }
                ledgerModel.PIU_LIST = commomFuncObj.PopulateDPIUOfSRRDA(PMGSYSession.Current.AdminNdCode);
                ledgerModel.PIU_LIST.Insert(0, (new SelectListItem { Text = "All DPIU", Value = "0", Selected = true }));

                List<SelectListItem> lstDPIU = new List<SelectListItem>();
                lstDPIU.Insert(0, new SelectListItem { Text = "Select DPIU", Value = "0" });
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
                ledgerModel.HEAD_LIST = GetLedgerHeadList("C", PMGSYSession.Current.FundType, opLevel, SRRDA_DPIU);

                ledgerModel.SRRDA = PMGSYSession.Current.AdminNdCode;

                if (PMGSYSession.Current.LevelId == 6)
                {
                    ledgerModel.levelId = 4;//SRRDA
                }
                else
                {
                    ledgerModel.levelId = PMGSYSession.Current.LevelId;
                }
                return View(ledgerModel);
            }
            catch (Exception ex)
            {
                throw new Exception("error occured");
            }
        }

        //action to get the list of head as per credit /debit
        [Audit]
        [HttpPost]
        public JsonResult GetLedgerHeadList(String id)
        {
            List<short> opLevel = new List<short>();
            try
            {
                String[] parameters;
                String SRRDA_DPIU = null;
                if (PMGSYSession.Current.LevelId == 4 || PMGSYSession.Current.LevelId == 6)
                {
                    parameters = id.Split('$');
                    id = parameters[0];
                    SRRDA_DPIU = parameters[1];
                }

                if (id == "C" || id == "D")
                {
                    if (PMGSYSession.Current.LevelId == 5)
                    {      //forDPIU
                        opLevel.Add(1);
                        opLevel.Add(2);
                    }
                    else if (SRRDA_DPIU == "S")
                    {    //For SRRDA                       
                        opLevel.Add(2);
                        opLevel.Add(3);
                    }
                    else if (SRRDA_DPIU == "D")
                    {   //forDPIU
                        opLevel.Add(1);
                        opLevel.Add(2);
                    }
                    return Json(GetLedgerHeadList(id, PMGSYSession.Current.FundType, opLevel, SRRDA_DPIU));
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
        [HttpPost]
        public ActionResult LedgerReport(String id)
        {
            try
            {
                if (!String.IsNullOrEmpty(id))
                {
                    string[] parameters = id.Split('$');
                    int AdminNdCode = 0;
                    short LevelId = 0;
                    string SRRDA_DPIU = null;
                    short LowerAdminNdCode = Convert.ToInt16(parameters[4]);
                    int SRRDA_ND_CODE = 0;
                    string RoadStatus = parameters[5];
                    short DPIUNdCode = -1;

                    if (PMGSYSession.Current.LevelId == 4 || PMGSYSession.Current.LevelId == 6)
                    {
                        AdminNdCode = Convert.ToInt32(parameters[6]);
                        SRRDA_DPIU = parameters[7];

                        if (SRRDA_DPIU == "S")
                        {
                            LevelId = 4;
                            SRRDA_ND_CODE = Convert.ToInt32(parameters[6]);//to disp agency name only
                        }
                        else if (SRRDA_DPIU == "D")
                        {
                            LevelId = 5;
                            SRRDA_ND_CODE = PMGSYSession.Current.LevelId == 6 ? Convert.ToInt16(parameters[9]) : PMGSYSession.Current.AdminNdCode;//to disp agency name only
                            DPIUNdCode = Convert.ToInt16(AdminNdCode);
                        }
                    }
                    else
                    {
                        AdminNdCode = PMGSYSession.Current.AdminNdCode;
                        LevelId = PMGSYSession.Current.LevelId;
                        SRRDA_ND_CODE = PMGSYSession.Current.AdminNdCode;//to disp agency name only
                        SRRDA_DPIU = "D";
                    }

                    //set Parameters To Ledger Model.
                    LedgerModel ledgerModel = new LedgerModel();
                    ledgerModel.FundType = PMGSYSession.Current.FundType;//Fund Type
                    ledgerModel.AdminNdCode = AdminNdCode; //Admin ND Code
                    ledgerModel.MONTH = Convert.ToInt16(parameters[0]);//Month
                    ledgerModel.YEAR = Convert.ToInt16(parameters[1]); //Year
                    ledgerModel.levelId = LevelId; //LevelID
                    ledgerModel.HeadCode = Convert.ToInt16(parameters[3]); //Head Code
                    ledgerModel.CREDIT_DEBIT = parameters[2];
                    ledgerModel.SRRDA_DPIU = SRRDA_DPIU;    //S-SRRDA D-DPIU
                    ledgerModel.HeadDetails = parameters[8]; //head Desc details.

                    //DPIU nd code
                    ledgerModel.DPIU = LowerAdminNdCode;
                    ledgerModel.SRRDA = SRRDA_ND_CODE;    //SRRDA_ND_Code
                    ledgerModel.RoadStatus = RoadStatus; //Road Status C-Completed,P-Inprogress,L-Ledger Detais,D-DPIU details

                    if (ledgerModel.RoadStatus == "C")
                    {
                        ledgerModel.ReportType = "C";
                        ledgerModel.DPIU = Convert.ToInt16(AdminNdCode);
                    }
                    else if (ledgerModel.RoadStatus == "P")
                    {
                        ledgerModel.ReportType = "P";
                        ledgerModel.DPIU = Convert.ToInt16(AdminNdCode);
                    }
                    else if (LowerAdminNdCode == -1)
                    {
                        ledgerModel.ReportType = "L";
                        ledgerModel.DPIU = DPIUNdCode;

                    }//|| objParam.CreditDebit == "C" condition added to disp all piu option for head P-6,A-77,M-333 21-June-2014
                    else if ((ledgerModel.CREDIT_DEBIT == "D" || ledgerModel.CREDIT_DEBIT == "C") && LowerAdminNdCode != -1)
                    {
                        ledgerModel.ReportType = "D";
                        ledgerModel.DPIU = LowerAdminNdCode;
                    }
                    return View(ledgerModel);
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
                lstDPIU.Insert(0, new SelectListItem { Text = "All DPIU", Value = "0" });
                return Json(new SelectList(lstDPIU, "Value", "Text"), JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json(false);
            }
        }

        /// <summary>
        /// Action to get credit debit head list
        /// </summary>
        /// <param name="creditDebit"> credit or debit </param>
        /// <param name="fundType"></param>
        /// <param name="op_Level"></param>
        /// <returns></returns>
        public List<SelectListItem> GetLedgerHeadList(String creditDebit, String fundType, List<short> op_Level, String SRRDA_DPIU)
        {
            PMGSY.Models.PMGSYEntities dbContext = new PMGSY.Models.PMGSYEntities();
            try
            {
                List<SelectListItem> headList = new List<SelectListItem>();

                short LevelId = 0;

                if (SRRDA_DPIU == "S")
                {
                    LevelId = 4;
                }
                else if (SRRDA_DPIU == "D")
                {
                    LevelId = 5;
                }
                else
                {
                    LevelId = PMGSYSession.Current.LevelId;
                }

                List<PMGSY.Models.SP_ACC_LEDGER_GET_CREDIT_DEBIT_HEADS_Result> list = dbContext.SP_ACC_LEDGER_GET_CREDIT_DEBIT_HEADS(fundType, LevelId, creditDebit).ToList<PMGSY.Models.SP_ACC_LEDGER_GET_CREDIT_DEBIT_HEADS_Result>();

                SelectListItem head = null;
                foreach (PMGSY.Models.SP_ACC_LEDGER_GET_CREDIT_DEBIT_HEADS_Result result in list)
                {
                    head = new SelectListItem();
                    head.Selected = false;
                    head.Text = result.HEAD_NAME;
                    head.Value = result.HEAD_ID.ToString();

                    headList.Add(head);
                }

                headList.Insert(0, (new SelectListItem { Text = " -- Select Head -- ", Value = "0", Selected = true }));
                return headList;
            }
            catch (Exception Ex)
            {
                throw Ex;
            }
            finally
            {
                dbContext.Dispose();
            }
        }



        #endregion ledger SSRS Report

        #region Shedule Reports

        #region Fund Received Report

        public ActionResult FundReceivedLayout()
        {
            CommonFunctions commomFuncObj = new CommonFunctions();
            SheduleReportModel filterModel = new SheduleReportModel
            {
                Month = (short)DateTime.Now.Month,
                ListMonth = commomFuncObj.PopulateMonths(DateTime.Now.Month),
                ListYear = commomFuncObj.PopulateYears(DateTime.Now.Year),
                Year = (short)DateTime.Now.Year,
                LevelId = PMGSYSession.Current.LevelId,
                Piu = PMGSYSession.Current.AdminNdCode,
            };
            if (PMGSYSession.Current.LevelId == 4)
            {
                filterModel.ListPiu = commomFuncObj.PopulateDPIUOfSRRDA(PMGSYSession.Current.AdminNdCode);
                filterModel.ListPiu.Insert(0, (new SelectListItem { Text = "All DPIU", Value = "0", Selected = true }));
                List<SelectListItem> lst = commomFuncObj.PopulateNodalAgencies(PMGSYSession.Current.StateCode);
                filterModel.ListAgency = lst.Where(m => m.Value == PMGSYSession.Current.AdminNdCode.ToString()).ToList();
            }
            else if (PMGSYSession.Current.LevelId == 6)
            {
                filterModel.ListPiu = new List<SelectListItem>();
                filterModel.ListPiu.Insert(0, (new SelectListItem { Text = "All DPIU", Value = "0", Selected = true }));
                filterModel.ListAgency = commomFuncObj.PopulateNodalAgencies();
            }
            return View(filterModel);
        }

        [Audit]
        public ActionResult FundReceivedReport(short month, short year, int? ndcode, int rlevel, int allpiu, string srrda_dpiu, int? SrrdaNdCode)
        {
            CommonFunctions commomFuncObj = new CommonFunctions();
            SheduleReportModel sheduleReportModel = new SheduleReportModel();
            var reportMaster = commomFuncObj.GetReportId(rlevel, PMGSYSession.Current.FundType, 147); //OMMAS_DEV-147 - Trainning -217

            if (reportMaster != null)
            {
                sheduleReportModel.ReportID = reportMaster.RPT_ID;
            }
            else
            {
                sheduleReportModel.ReportID = 0;
            }
            sheduleReportModel.Agency = PMGSYSession.Current.LevelId == 5 ? PMGSYSession.Current.AdminNdCode : (ndcode == null ? 0 : ndcode.Value);
            sheduleReportModel.Month = month;
            sheduleReportModel.Year = year;
            sheduleReportModel.Piu = allpiu;
            sheduleReportModel.FundType = PMGSYSession.Current.FundType;
            sheduleReportModel.SRRDA_DPIU = srrda_dpiu;
            sheduleReportModel.SrrdaNdCode = PMGSYSession.Current.LevelId == 5 ? PMGSYSession.Current.ParentNDCode : SrrdaNdCode;
            return PartialView(sheduleReportModel);
        }

        #endregion Fund Received Report

        #region Deposit Repayable

        public ActionResult DepositRepayableLayout()
        {
            CommonFunctions commomFuncObj = new CommonFunctions();
            SheduleReportModel filterModel = new SheduleReportModel
            {
                Month = (short)DateTime.Now.Month,
                ListMonth = commomFuncObj.PopulateMonths(DateTime.Now.Month),
                ListYear = commomFuncObj.PopulateYears(DateTime.Now.Year),
                Year = (short)DateTime.Now.Year,
                LevelId = PMGSYSession.Current.LevelId,
                Piu = PMGSYSession.Current.AdminNdCode,
            };
            if (PMGSYSession.Current.LevelId == 4)
            {
                filterModel.ListPiu = commomFuncObj.PopulateDPIUOfSRRDA(PMGSYSession.Current.AdminNdCode);
                filterModel.ListPiu.Insert(0, (new SelectListItem { Text = "All DPIU", Value = "0", Selected = true }));
                List<SelectListItem> lst = commomFuncObj.PopulateNodalAgencies(PMGSYSession.Current.StateCode);
                filterModel.ListAgency = lst.Where(m => m.Value == PMGSYSession.Current.AdminNdCode.ToString()).ToList();
            }
            else if (PMGSYSession.Current.LevelId == 6)
            {
                filterModel.ListPiu = new List<SelectListItem>();
                filterModel.ListPiu.Insert(0, (new SelectListItem { Text = "All DPIU", Value = "0", Selected = true }));
                filterModel.ListAgency = commomFuncObj.PopulateNodalAgencies();
            }
            return View(filterModel);
        }
        public ActionResult DepositRepayableReport(short month, short year, int? ndcode, int rlevel, int allpiu, string srrda_dpiu, int? SrrdaNdCode)
        {
            CommonFunctions commomFuncObj = new CommonFunctions();
            SheduleReportModel sheduleReportModel = new SheduleReportModel();
            var reportMaster = commomFuncObj.GetReportId(rlevel, PMGSYSession.Current.FundType, 141);//OMMAS_DEV-141 - Trainning -180

            if (reportMaster != null)
            {
                sheduleReportModel.ReportID = reportMaster.RPT_ID;
            }
            else
            {
                sheduleReportModel.ReportID = 0;
            }
            sheduleReportModel.Agency = PMGSYSession.Current.LevelId == 5 ? PMGSYSession.Current.AdminNdCode : (ndcode == null ? 0 : ndcode.Value);
            sheduleReportModel.Month = month;
            sheduleReportModel.Year = year;
            sheduleReportModel.Piu = allpiu;
            sheduleReportModel.FundType = PMGSYSession.Current.FundType;
            sheduleReportModel.SRRDA_DPIU = srrda_dpiu;
            sheduleReportModel.SrrdaNdCode = PMGSYSession.Current.LevelId == 5 ? PMGSYSession.Current.ParentNDCode : SrrdaNdCode;
            return PartialView(sheduleReportModel);
        }

        #endregion Deposit Re-payable

        #region Incidental Fund

        public ActionResult IncidentalFundLayout()
        {
            CommonFunctions commomFuncObj = new CommonFunctions();
            SheduleReportModel filterModel = new SheduleReportModel
            {
                Month = (short)DateTime.Now.Month,
                ListMonth = commomFuncObj.PopulateMonths(DateTime.Now.Month),
                ListYear = commomFuncObj.PopulateYears(DateTime.Now.Year),
                Year = (short)DateTime.Now.Year,
                LevelId = PMGSYSession.Current.LevelId,
                Piu = PMGSYSession.Current.AdminNdCode,
            };
            if (PMGSYSession.Current.LevelId == 4)
            {
                filterModel.ListPiu = commomFuncObj.PopulateDPIUOfSRRDA(PMGSYSession.Current.AdminNdCode);
                filterModel.ListPiu.Insert(0, (new SelectListItem { Text = "All DPIU", Value = "0", Selected = true }));
                List<SelectListItem> lst = commomFuncObj.PopulateNodalAgencies(PMGSYSession.Current.StateCode);
                filterModel.ListAgency = lst.Where(m => m.Value == PMGSYSession.Current.AdminNdCode.ToString()).ToList();
            }
            else if (PMGSYSession.Current.LevelId == 6)
            {
                filterModel.ListPiu = new List<SelectListItem>();
                filterModel.ListPiu.Insert(0, (new SelectListItem { Text = "All DPIU", Value = "0", Selected = true }));
                filterModel.ListAgency = commomFuncObj.PopulateNodalAgencies();
            }
            return View(filterModel);
        }

        public ActionResult IncidentalFundReport(short month, short year, int? ndcode, int rlevel, int allpiu, string srrda_dpiu, int? SrrdaNdCode)
        {
            CommonFunctions commomFuncObj = new CommonFunctions();
            SheduleReportModel scheduleReportModel = new SheduleReportModel();
            var reportMaster = commomFuncObj.GetReportId(rlevel, PMGSYSession.Current.FundType, 146);//OMMAS_DEV-146 - Trainning -198                        

            PMGSY.Models.ACC_RPT_REPORT_PROPERTY[] reportHeader = new PMGSY.Models.ACC_RPT_REPORT_PROPERTY[4];

            if (reportMaster != null)
            {
                reportHeader = reportMaster.ACC_RPT_REPORT_PROPERTY.ToArray<PMGSY.Models.ACC_RPT_REPORT_PROPERTY>();
                scheduleReportModel.FormNumber = reportHeader[0].PROP_VALUE;
                scheduleReportModel.ScheduleNumber = reportHeader[1].PROP_VALUE;
                //FundType = reportHeader[2].PROP_VALUE;
                scheduleReportModel.ReportHeader = reportHeader[3].PROP_VALUE;
                scheduleReportModel.Refference = reportHeader[4].PROP_VALUE;
                scheduleReportModel.ReportID = reportMaster.RPT_ID;
            }

            scheduleReportModel.Agency = PMGSYSession.Current.LevelId == 5 ? PMGSYSession.Current.AdminNdCode : (ndcode == null ? 0 : ndcode.Value);
            scheduleReportModel.Month = month;
            scheduleReportModel.Year = year;
            scheduleReportModel.Piu = allpiu;
            scheduleReportModel.FundType = PMGSYSession.Current.FundType;
            scheduleReportModel.SRRDA_DPIU = srrda_dpiu;
            scheduleReportModel.SrrdaNdCode = PMGSYSession.Current.LevelId == 5 ? PMGSYSession.Current.ParentNDCode : SrrdaNdCode;
            return PartialView(scheduleReportModel);
        }

        #endregion Incidental Fund

        #region Current Liabilities

        public ActionResult CurrentLiabilitiesLayout()
        {
            CommonFunctions commomFuncObj = new CommonFunctions();
            SheduleReportModel filterModel = new SheduleReportModel
            {
                Month = (short)DateTime.Now.Month,
                ListMonth = commomFuncObj.PopulateMonths(DateTime.Now.Month),
                ListYear = commomFuncObj.PopulateYears(DateTime.Now.Year),
                Year = (short)DateTime.Now.Year,
                LevelId = PMGSYSession.Current.LevelId,
                Piu = PMGSYSession.Current.AdminNdCode,
            };
            if (PMGSYSession.Current.LevelId == 4)
            {
                filterModel.ListPiu = commomFuncObj.PopulateDPIUOfSRRDA(PMGSYSession.Current.AdminNdCode);
                filterModel.ListPiu.Insert(0, (new SelectListItem { Text = "All DPIU", Value = "0", Selected = true }));
                List<SelectListItem> lst = commomFuncObj.PopulateNodalAgencies(PMGSYSession.Current.StateCode);
                filterModel.ListAgency = lst.Where(m => m.Value == PMGSYSession.Current.AdminNdCode.ToString()).ToList();
            }
            else if (PMGSYSession.Current.LevelId == 6)
            {
                filterModel.ListPiu = new List<SelectListItem>();
                filterModel.ListPiu.Insert(0, (new SelectListItem { Text = "All DPIU", Value = "0", Selected = true }));
                filterModel.ListAgency = commomFuncObj.PopulateNodalAgencies();
            }
            return View(filterModel);
        }

        public ActionResult CurrentLiabilitiesReport(short month, short year, int? ndcode, int rlevel, int allpiu, string srrda_dpiu, int? SrrdaNdCode)
        {
            CommonFunctions commomFuncObj = new CommonFunctions();
            SheduleReportModel scheduleReportModel = new SheduleReportModel();
            var reportMaster = commomFuncObj.GetReportId(rlevel, PMGSYSession.Current.FundType, 148);//OMMAS_DEV-148 - Trainning -187

            PMGSY.Models.ACC_RPT_REPORT_PROPERTY[] reportHeader = new PMGSY.Models.ACC_RPT_REPORT_PROPERTY[4];

            if (reportMaster != null)
            {
                reportHeader = reportMaster.ACC_RPT_REPORT_PROPERTY.ToArray<PMGSY.Models.ACC_RPT_REPORT_PROPERTY>();
                scheduleReportModel.FormNumber = reportHeader[0].PROP_VALUE;
                scheduleReportModel.ScheduleNumber = reportHeader[1].PROP_VALUE;
                //FundType = reportHeader[2].PROP_VALUE;
                scheduleReportModel.ReportHeader = reportHeader[3].PROP_VALUE;
                scheduleReportModel.Refference = reportHeader[4].PROP_VALUE;
                scheduleReportModel.ReportID = reportMaster.RPT_ID;
            }

            scheduleReportModel.Agency = PMGSYSession.Current.LevelId == 5 ? PMGSYSession.Current.AdminNdCode : (ndcode == null ? 0 : ndcode.Value);
            scheduleReportModel.Month = month;
            scheduleReportModel.Year = year;
            scheduleReportModel.Piu = allpiu;
            scheduleReportModel.FundType = PMGSYSession.Current.FundType;
            scheduleReportModel.SRRDA_DPIU = srrda_dpiu;
            scheduleReportModel.SrrdaNdCode = PMGSYSession.Current.LevelId == 5 ? PMGSYSession.Current.ParentNDCode : SrrdaNdCode;
            return PartialView(scheduleReportModel);
        }

        #endregion Current Liabilities

        #region Current Assets

        public ActionResult CurrentAssetLayout()
        {
            CommonFunctions commomFuncObj = new CommonFunctions();
            SheduleReportModel filterModel = new SheduleReportModel
            {
                Month = (short)DateTime.Now.Month,
                ListMonth = commomFuncObj.PopulateMonths(DateTime.Now.Month),
                ListYear = commomFuncObj.PopulateYears(DateTime.Now.Year),
                Year = (short)DateTime.Now.Year,
                LevelId = PMGSYSession.Current.LevelId,
                Piu = PMGSYSession.Current.AdminNdCode,
            };
            if (PMGSYSession.Current.LevelId == 4)
            {
                filterModel.ListPiu = commomFuncObj.PopulateDPIUOfSRRDA(PMGSYSession.Current.AdminNdCode);
                filterModel.ListPiu.Insert(0, (new SelectListItem { Text = "All DPIU", Value = "0", Selected = true }));
                List<SelectListItem> lst = commomFuncObj.PopulateNodalAgencies(PMGSYSession.Current.StateCode);
                filterModel.ListAgency = lst.Where(m => m.Value == PMGSYSession.Current.AdminNdCode.ToString()).ToList();
            }
            else if (PMGSYSession.Current.LevelId == 6)
            {
                filterModel.ListPiu = new List<SelectListItem>();
                filterModel.ListPiu.Insert(0, (new SelectListItem { Text = "All DPIU", Value = "0", Selected = true }));
                filterModel.ListAgency = commomFuncObj.PopulateNodalAgencies();
            }
            return View(filterModel);
        }
        public ActionResult CurrentAssetReport(short month, short year, int? ndcode, int rlevel, int allpiu, int? duration)
        {
            CommonFunctions commomFuncObj = new CommonFunctions();
            SheduleReportModel sheduleReportModel = new SheduleReportModel();
            var reportMaster = commomFuncObj.GetReportId(rlevel, PMGSYSession.Current.FundType, 152); //OMMAS_DEV-152 - Trainning -191

            //Modified By Abhishek kamble 21-Apr-2014 start
            PMGSY.Models.ACC_RPT_REPORT_PROPERTY[] reportHeader = new PMGSY.Models.ACC_RPT_REPORT_PROPERTY[4];

            if (reportMaster != null)
            {
                reportHeader = reportMaster.ACC_RPT_REPORT_PROPERTY.ToArray<PMGSY.Models.ACC_RPT_REPORT_PROPERTY>();
                sheduleReportModel.FormNumber = reportHeader[0].PROP_VALUE;
                sheduleReportModel.ReportHeader = reportHeader[3].PROP_VALUE;
                sheduleReportModel.Refference = reportHeader[4].PROP_VALUE;
            }

            string flag = string.Empty;

            if (duration == 1)//1- annually 2- Monthly
            {
                month = 0;
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


            sheduleReportModel.Agency = PMGSYSession.Current.LevelId == 5 ? PMGSYSession.Current.ParentNDCode.Value : (ndcode == null ? 0 : ndcode.Value);
            sheduleReportModel.Month = month;
            sheduleReportModel.Year = year;
            sheduleReportModel.Piu = PMGSYSession.Current.LevelId == 5 ? PMGSYSession.Current.AdminNdCode : allpiu;
            sheduleReportModel.FundType = PMGSYSession.Current.FundType;
            sheduleReportModel.SRRDA_DPIU = flag;

            return PartialView(sheduleReportModel);
        }

        #endregion Current Assets

        #region Durable Assets

        public ActionResult DurableAssetLayout()
        {
            CommonFunctions commomFuncObj = new CommonFunctions();
            SheduleReportModel filterModel = new SheduleReportModel
            {
                Month = (short)DateTime.Now.Month,
                ListMonth = commomFuncObj.PopulateMonths(DateTime.Now.Month),
                ListYear = commomFuncObj.PopulateYears(DateTime.Now.Year),
                Year = (short)DateTime.Now.Year,
                LevelId = PMGSYSession.Current.LevelId,
                Piu = PMGSYSession.Current.AdminNdCode,
            };
            if (PMGSYSession.Current.LevelId == 4)
            {
                filterModel.ListPiu = commomFuncObj.PopulateDPIUOfSRRDA(PMGSYSession.Current.AdminNdCode);
                filterModel.ListPiu.Insert(0, (new SelectListItem { Text = "All DPIU", Value = "0", Selected = true }));
                List<SelectListItem> lst = commomFuncObj.PopulateNodalAgencies(PMGSYSession.Current.StateCode);
                filterModel.ListAgency = lst.Where(m => m.Value == PMGSYSession.Current.AdminNdCode.ToString()).ToList();
            }
            else if (PMGSYSession.Current.LevelId == 6)
            {
                filterModel.ListPiu = new List<SelectListItem>();
                filterModel.ListPiu.Insert(0, (new SelectListItem { Text = "All DPIU", Value = "0", Selected = true }));
                filterModel.ListAgency = commomFuncObj.PopulateNodalAgencies();
            }
            return View(filterModel);
        }
        public ActionResult DurableAssetReport(short month, short year, int? ndcode, int rlevel, int allpiu, int? duration)
        {
            CommonFunctions commomFuncObj = new CommonFunctions();
            SheduleReportModel sheduleReportModel = new SheduleReportModel();
            var reportMaster = commomFuncObj.GetReportId(rlevel, PMGSYSession.Current.FundType, 154); //154 is Menu id for durable assets menu.

            //Modified By Abhishek kamble 21-Apr-2014 start
            PMGSY.Models.ACC_RPT_REPORT_PROPERTY[] reportHeader = new PMGSY.Models.ACC_RPT_REPORT_PROPERTY[4];

            if (reportMaster != null)
            {
                reportHeader = reportMaster.ACC_RPT_REPORT_PROPERTY.ToArray<PMGSY.Models.ACC_RPT_REPORT_PROPERTY>();
                sheduleReportModel.FormNumber = reportHeader[0].PROP_VALUE;
                sheduleReportModel.ReportHeader = reportHeader[3].PROP_VALUE;
                sheduleReportModel.Refference = reportHeader[4].PROP_VALUE;
            }

            string flag = string.Empty;

            if (duration == 1)//1- annually 2- Monthly
            {
                month = 0;
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


            sheduleReportModel.Agency = PMGSYSession.Current.LevelId == 5 ? PMGSYSession.Current.ParentNDCode.Value : (ndcode == null ? 0 : ndcode.Value);
            sheduleReportModel.Month = month;
            sheduleReportModel.Year = year;
            sheduleReportModel.Piu = PMGSYSession.Current.LevelId == 5 ? PMGSYSession.Current.AdminNdCode : allpiu;
            sheduleReportModel.FundType = PMGSYSession.Current.FundType;
            sheduleReportModel.SRRDA_DPIU = flag;

            return PartialView(sheduleReportModel);
        }

        #endregion Durable Assets

        #region AbstractScheduleOfRoads

        public ActionResult AbstractSheduleRoadsLayout()
        {
            AbstractSheduleOfRoads absScheduleOfRoads = new AbstractSheduleOfRoads();

            return View(absScheduleOfRoads);
        }


        public ActionResult AbstractScheduleRoadDetails(AbstractSheduleOfRoads absScheduleOfRoad)
        {
            if (absScheduleOfRoad.MonthlyYearly == "Y")
            {
                absScheduleOfRoad.Month = 3;//March                
                absScheduleOfRoad.Year = absScheduleOfRoad.FinancialYear + 1;
            }
            return View(absScheduleOfRoad);
        }

        public List<SelectListItem> PopulateHead()
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            try
            {
                //String [] HeadCodeArr={"11.01","11.02","11.03","11.04","11.05","11.06","11.07"};
                //List<SelectListItem> lstHead = new List<SelectListItem>();
                return new SelectList(dbContext.ACC_MASTER_HEAD.Where(m => m.HEAD_CODE == "11.01" || m.HEAD_CODE == "11.02" || m.HEAD_CODE == "11.03" || m.HEAD_CODE == "11.04" || m.HEAD_CODE == "11.05" || m.HEAD_CODE == "11.06" || m.HEAD_CODE == "11.07" ), "HEAD_ID", "HEAD_NAME").ToList();
                //return lstHead;
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

        #endregion AbstractScheduleOfRoads

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
                return Json(new { success = false, message = message.ToString() });
            }



        }


        #endregion Shedule Reports

        #region ScheduleRoad

        public ActionResult ScheduleNewRoadLayout()
        {
            PMGSY.Areas.AccountReports.Models.ScheduleRoadModel objModel = new Models.ScheduleRoadModel();
            PMGSY.Common.CommonFunctions objCommon = new PMGSY.Common.CommonFunctions();
            int[] headCode = { 28, 427 };//Rohit J for RCPLWE
            List<SelectListItem> lstHead = new List<SelectListItem>();
            lstHead = PopulateHead(headCode);
            lstHead.Insert(0, new SelectListItem { Text = "Select Head", Value = "0" });
            ViewBag.Head = lstHead;
            ViewBag.Schedule = "Schedule of New Road Constructed";
            ScheduleView();
            objModel.Month = PMGSYSession.Current.AccMonth;
            objModel.Year = PMGSYSession.Current.AccYear;
            objModel.RoadReportType = "N";//Schedule of Other Expenditure on Road            
            return View("ScheduleRoadLayout", objModel);
        }

        public ActionResult ScheduleUpgradedRoadLayout()
        {
            PMGSY.Areas.AccountReports.Models.ScheduleRoadModel objModel = new Models.ScheduleRoadModel();
            PMGSY.Common.CommonFunctions objCommon = new PMGSY.Common.CommonFunctions();
            int[] headCode = { 29, 385, 386,428, 464,465 };

            //Added By Abhishek kamble 24-sep-2014 to populate PMGSY II heads Rohit J for RCPLWE
            List<SelectListItem> lstHead = new List<SelectListItem>();
            lstHead = PopulateHead(headCode);
            lstHead.Insert(0, new SelectListItem { Text = "Select Head", Value = "0" });
            ViewBag.Head = lstHead;
            ViewBag.Schedule = "Schedule of Upgradation of Existing Road";
            ScheduleView();
            objModel.Month = PMGSYSession.Current.AccMonth;
            objModel.Year = PMGSYSession.Current.AccYear;
            objModel.RoadReportType = "U";
            return View("ScheduleRoadLayout", objModel);
        }

        public ActionResult ScheduleOtherExpenditureLayout()
        {
            PMGSY.Areas.AccountReports.Models.ScheduleRoadModel objModel = new Models.ScheduleRoadModel();
            PMGSY.Common.CommonFunctions objCommon = new PMGSY.Common.CommonFunctions();
            int[] headCode = { 30, 31, 32, 33, 34, 387, 388, 409, 410, 412, 429, 430, 512, 513, 514, 515, 516, 517, 524, 525, 526, 527, 528, 529, 530, 531, 532, 533, 534, 535, 536, 537, 538, 539, 540, 541, 542, 543 };
            List<SelectListItem> lstHead = new List<SelectListItem>();
            lstHead = PopulateHead(headCode);
            lstHead.Insert(0, new SelectListItem { Text = "Select Head", Value = "0" });
            ViewBag.Head = lstHead;
            ViewBag.Schedule = "Schedule of Other Expenditure on Roads";
            ScheduleView();
            objModel.Month = PMGSYSession.Current.AccMonth;
            objModel.Year = PMGSYSession.Current.AccYear;
            objModel.RoadReportType = "E";//Schedule of Other Expenditure on Road
            return View("ScheduleRoadLayout", objModel);
        }

        private void ScheduleView()
        {
            short month = 0;
            short year = 0;
            CommonFunctions objCommanFunc = new CommonFunctions();
            List<SelectListItem> lstFundAgency = lstFundingAgency();
            lstFundAgency.Insert(0, new SelectListItem { Text = "All Agency", Value = "0" });
            ViewBag.FumdingAgency = lstFundAgency;
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
            ViewBag.Month = objCommanFunc.PopulateMonths(month);
            ViewBag.Year = objCommanFunc.PopulateYears(year);
            List<SelectListItem> lstSRRDA = new List<SelectListItem>();
            lstSRRDA = objCommanFunc.PopulateNodalAgencies();
            if (PMGSYSession.Current.LevelId == 4)
            {
                var selected = lstSRRDA.Where(m => m.Value == PMGSYSession.Current.AdminNdCode.ToString()).First();
                selected.Selected = true;
            }
            ViewBag.State = lstSRRDA;
            List<SelectListItem> lstDPIU = new List<SelectListItem>();
            lstDPIU.Insert(0, new SelectListItem { Text = "Select DPIU", Value = "0" });
            ViewBag.DPIU = lstDPIU;
        }

        public ActionResult ScheduleRoadDetails(PMGSY.Areas.AccountReports.Models.ScheduleRoadModel objScheduleModel)
        {
            int AdminNdCode;
            int LowerAdminNdCode;
            if (PMGSYSession.Current.LevelId == 5)
            {
                AdminNdCode = Convert.ToInt32(PMGSYSession.Current.ParentNDCode);
                LowerAdminNdCode = PMGSYSession.Current.AdminNdCode;
            }
            else
            {
                AdminNdCode = Convert.ToInt32(objScheduleModel.State);
                LowerAdminNdCode = Convert.ToInt32(objScheduleModel.Piu);
            }

            objScheduleModel.FundType = PMGSYSession.Current.FundType;
            objScheduleModel.Piu = LowerAdminNdCode;
            objScheduleModel.LevelId = 5;//DPIU      
            objScheduleModel.SRRDANdCode = AdminNdCode;

            return View("ScheduleRoadDetails", objScheduleModel);
        }

        public List<SelectListItem> lstFundingAgency()
        {
            PMGSY.Models.PMGSYEntities dbcontext = new PMGSY.Models.PMGSYEntities();
            try
            {
                return new SelectList(dbcontext.MASTER_FUNDING_AGENCY.ToList(), "MAST_FUNDING_AGENCY_CODE", "MAST_FUNDING_AGENCY_NAME").ToList();
            }
            catch (Exception ex)
            {
                return null;
            }
            finally
            {
                dbcontext.Dispose();
            }
        }


        public List<SelectListItem> PopulateHead(int[] headId)
        {
            PMGSY.Models.PMGSYEntities dbcontext = new PMGSY.Models.PMGSYEntities();
            try
            {
                var lstDetails = (from item in dbcontext.ACC_MASTER_HEAD
                                  where headId.Contains(item.HEAD_ID)
                                  select new
                                  {
                                      HEAD_CODE = item.HEAD_ID,
                                      HEAD_NAME = item.HEAD_NAME
                                  }).ToList();
                return new SelectList(lstDetails, "HEAD_CODE", "HEAD_NAME").ToList();
            }
            catch (Exception ex)
            {
                return null;
            }
            finally
            {
                dbcontext.Dispose();
            }
        }

        #endregion ScheduleRoad

        #region ScheduleOfRoadsMF

        /// <summary>
        /// ScheduleOfRoadsMFLayout() is used to show filter
        /// </summary>
        /// <returns></returns>
        public ActionResult ScheduleOfRoadsMFLayout()
        {
            try
            {
                PMGSY.Areas.AccountReports.Models.ScheduleOfRoadMFModel objModel = new Models.ScheduleOfRoadMFModel();
                PMGSY.Common.CommonFunctions objCommon = new PMGSY.Common.CommonFunctions();
                List<SelectListItem> lstHead = new List<SelectListItem>();
                lstHead = PopulateScheduleRoadHeadMF();
                ViewBag.Head = lstHead;
                ScheduleView();
                objModel.Month = PMGSYSession.Current.AccMonth;
                objModel.Year = PMGSYSession.Current.AccYear;
                return View("ScheduleOfRoadsMFLayout", objModel);
            }
            catch (Exception)
            {
                return View("ScheduleOfRoadsMFLayout", new Models.ScheduleOfRoadMFModel());
            }
        }

        /// <summary>
        ///  ScheduleOfRoadsMFReport() method display SSRS report based on selected search criteria.
        /// </summary>
        /// <param name="objScheduleModel"></param>
        /// <returns></returns>
        public ActionResult ScheduleOfRoadsMFReport(PMGSY.Areas.AccountReports.Models.ScheduleOfRoadMFModel objScheduleModel)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            try
            {
                int AdminNdCode;  //admin Nd code of SRRDA
                int LowerAdminNdCode;   //admin Nd code of DPIU
                if (PMGSYSession.Current.LevelId == 5)
                {
                    AdminNdCode = Convert.ToInt32(PMGSYSession.Current.ParentNDCode);
                    LowerAdminNdCode = PMGSYSession.Current.AdminNdCode;
                }
                else
                {
                    AdminNdCode = Convert.ToInt32(objScheduleModel.State);
                    LowerAdminNdCode = Convert.ToInt32(objScheduleModel.Piu);
                }

                objScheduleModel.FundType = PMGSYSession.Current.FundType;
                objScheduleModel.Piu = LowerAdminNdCode;
                objScheduleModel.LevelId = 5;//DPIU      
                objScheduleModel.ParentNdCode = AdminNdCode;

                var ReportHeaders = dbContext.SP_ACC_Get_Report_Header_Information("AccountReports/Account", "ScheduleOfRoadsMFLayout", PMGSYSession.Current.FundType, 5, "A").ToList();

                objScheduleModel.ReportName = ReportHeaders.Where(m => m.REPORT_CPWD_NO == objScheduleModel.HeadCode.ToString()).Select(s => s.REPORT_NAME).FirstOrDefault();
                objScheduleModel.ReportParagraphName = ReportHeaders.Where(m => m.REPORT_CPWD_NO == objScheduleModel.HeadCode.ToString()).Select(s => s.REPORT_PARAGRAPH_NAME).FirstOrDefault();
                objScheduleModel.ReportFormNo = ReportHeaders.Where(m => m.REPORT_CPWD_NO == objScheduleModel.HeadCode.ToString()).Select(s => s.REPORT_FORM_NO).FirstOrDefault();

                return View("ScheduleOfRoadsMFReport", objScheduleModel);
            }
            catch (Exception)
            {
                return View("ScheduleOfRoadsMFReport", objScheduleModel);
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        public List<SelectListItem> PopulateScheduleRoadHeadMF()
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            try
            {
                var HeadDetails = dbContext.ACC_MASTER_HEAD.Where(m => m.HEAD_CODE == "140" || m.HEAD_CODE == "141" || m.HEAD_CODE == "142").ToList();

                List<SelectListItem> lstHead = new List<SelectListItem>();
                foreach (var item in HeadDetails)
                {
                    lstHead.Add(new SelectListItem { Text = item.HEAD_CODE + " - " + item.HEAD_NAME, Value = item.HEAD_ID.ToString() });
                }

                lstHead.Insert(0, new SelectListItem { Text = "Select Head", Value = "0" });

                return lstHead;
                //return new SelectList(dbContext.ACC_MASTER_HEAD.Where(m => m.HEAD_CODE == "11.01" || m.HEAD_CODE == "11.02" || m.HEAD_CODE == "11.03" || m.HEAD_CODE == "11.04" || m.HEAD_CODE == "11.05" || m.HEAD_CODE == "11.06" || m.HEAD_CODE == "11.07"), "HEAD_ID", "HEAD_NAME").ToList();                
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


        #endregion ScheduleOfRoadsMF


        #region Population Methods
        /// <summary>
        /// used in report 1)Fund Received to populate DPIU.  2)Depasit Repayable
        /// </summary>
        /// <param name="ndcode"></param>
        /// <returns></returns>      
        /// 
        public JsonResult GetDPIUOfSRRDA(int ndcode)
        {
            CommonFunctions commomFuncObj = new CommonFunctions();
            return Json(commomFuncObj.PopulateDPIUOfSRRDA(ndcode), JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult PopulateYears()
        {
            List<SelectListItem> lstYears = new List<SelectListItem>();
            try
            {
                CommonFunctions commonFunctions = new CommonFunctions();

                return Json(commonFunctions.PopulateYears(System.DateTime.Now.Year));
            }
            catch (Exception)
            {
                return null;
            }
        }

        [HttpPost]
        public JsonResult PopulateFinancialYears()
        {
            try
            {
                CommonFunctions commonFunctions = new CommonFunctions();
                SelectList lstYears = commonFunctions.PopulateFinancialYear();
                return Json(lstYears);
            }
            catch (Exception)
            {
                return null;
            }
        }

        //Populate DPIU      
        public JsonResult PopulateDPIU(string id)
        {
            try
            {
                CommonFunctions objCommonFunction = new CommonFunctions();
                int AdminNdCode = Convert.ToInt32(id);

                if (AdminNdCode == 0)
                {
                    List<SelectListItem> lstDpiu = new List<SelectListItem>();
                    lstDpiu.Insert(0, new SelectListItem { Text = "All DPIU", Value = "0" });
                    return Json(lstDpiu, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    TransactionParams objParam = new TransactionParams();
                    objParam.ADMIN_ND_CODE = AdminNdCode;
                    objParam.DISTRICT_CODE = PMGSYSession.Current.DistrictCode;

                    List<SelectListItem> lstDPIU = objCommonFunction.PopulateDPIU(objParam);
                    lstDPIU.RemoveAt(0);
                    lstDPIU.Insert(0, new SelectListItem { Text = "All DPIU", Value = "0" });

                    if (PMGSYSession.Current.LevelId == 5)
                    {
                        lstDPIU = lstDPIU.Where(m => m.Value == PMGSYSession.Current.AdminNdCode.ToString()).ToList();

                    }

                    return Json(new SelectList(lstDPIU, "Value", "Text"), JsonRequestBehavior.AllowGet);
                }
            }
            catch
            {
                return Json(false);
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

        #region SSRS Reports

        #region FinancialProgressofWork
        //Added By Rohit A. Jadhav
        public ActionResult FinacialProgressLayout()
        {
            CommonFunctions commomFuncObj = new CommonFunctions();
            FinancialProgressofWork finapwork = new FinancialProgressofWork
            {
                Month = (short)DateTime.Now.Month,
                ListMonth = commomFuncObj.PopulateMonths(DateTime.Now.Month),

                ListYear = commomFuncObj.PopulateYears(DateTime.Now.Year),
                Year = (short)DateTime.Now.Year,

                LevelId = PMGSYSession.Current.LevelId,


            };
            // PIU Level
            if (PMGSYSession.Current.LevelId == 5)
            {
                List<SelectListItem> lst = commomFuncObj.PopulateNodalAgencies(PMGSYSession.Current.StateCode);
                finapwork.ListAgency = lst.Where(m => m.Value == PMGSYSession.Current.ParentNDCode.ToString()).ToList();
            }
            // SRRDA Level
            if (PMGSYSession.Current.LevelId == 4)
            {

                List<SelectListItem> lst = commomFuncObj.PopulateNodalAgencies(PMGSYSession.Current.StateCode);
                finapwork.ListAgency = lst.Where(m => m.Value == PMGSYSession.Current.AdminNdCode.ToString()).ToList();
            }
            // Mord Level
            else if (PMGSYSession.Current.LevelId == 6)
            {


                List<SelectListItem> items = commomFuncObj.PopulateNodalAgencies();

                items.Insert(0, (new SelectListItem { Text = "All", Value = "0" }));

                finapwork.ListAgency = items;

            }

            return View(finapwork);


        }


        public ActionResult FinacialProgressReport(short month, short year, int? ndcode, string StateName)
        {
            CommonFunctions commomFuncObj = new CommonFunctions();
            FinancialProgressofWork finapwork = new FinancialProgressofWork();
            finapwork.Name = StateName;
            finapwork.Month = month;
            finapwork.Year = year;
            finapwork.Agency = (ndcode == null ? 0 : ndcode.Value);

            return PartialView(finapwork);

        }
        #endregion

        #region AnnualAccount
        /// <summary>
        /// Action is used to load Annual Account search form.
        /// </summary>
        /// <returns></returns>
        [Audit]
        public ActionResult AnnualAccountLayout()
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

            List<SelectListItem> lstDPIU = new List<SelectListItem>();
            lstDPIU.Insert(0, new SelectListItem { Text = "All DPIU", Value = "0" });
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

        //[HttpPost]
        //[Audit]
        //public JsonResult PopulateDPIU(string id)
        //{
        //    CommonFunctions objCommFunc = new CommonFunctions();
        //    string[] param = id.Split('$');

        //    List<SelectListItem> lstDPIU = new List<SelectListItem>();
        //    if (id != string.Empty)
        //    {
        //        int DPIUCode = Convert.ToInt32(param[0]);
        //        lstDPIU = objCommFunc.PopulateDPIUOfSRRDA(DPIUCode);
        //    }

        //    if (lstDPIU.Count == 1)
        //    {
        //        lstDPIU.RemoveAt(0);
        //    }

        //    if (param[1] == "A")
        //    {
        //        lstDPIU.Insert(0, new SelectListItem { Text = "All DPIU", Value = "0" });
        //    }
        //    else
        //    {
        //        lstDPIU.Insert(0, new SelectListItem { Text = "Select DPIU", Value = "0" });
        //    }
        //    return Json(lstDPIU, JsonRequestBehavior.AllowGet);
        //}


        /// <summary>
        /// Action is used to load Annual Account list view based on search criteria.
        /// </summary>
        /// <param name="objAnnualAccount">Model object contains Year and BalanceType as a parameter which are passed from search form.</param>
        /// <returns></returns>
        [HttpPost]
        [Audit]
        public ActionResult AnnualAccountReport(AnnualAccount objAnnualAccount)
        {
            if (PMGSYSession.Current.UserId == 0)
            {
                Response.Redirect("/Login/SessionExpire");
            }

            PMGSY.Models.Report.ReportFilter objParam = new PMGSY.Models.Report.ReportFilter();
            ViewBag.Year = (objAnnualAccount.Year) + "-" + Convert.ToString(objAnnualAccount.Year + 1);
            if (PMGSYSession.Current.LevelId == 5 || PMGSYSession.Current.LevelId == 4)
            {
                ViewBag.State = PMGSYSession.Current.StateName;
            }
            if (PMGSYSession.Current.LevelId == 6)
            {
                ViewBag.State = objAccountDAL.StateName(Convert.ToInt32(objAnnualAccount.State));
            }
            objParam.LevelId = PMGSYSession.Current.LevelId;
            if (objAnnualAccount.Selection == "S" || objAnnualAccount.Selection == "R")
            {
                objParam.AdminNdCode = Convert.ToInt32(objAnnualAccount.State);

            }
            else if (objAnnualAccount.Selection == "D")
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

            if (PMGSYSession.Current.LevelId == 4 || PMGSYSession.Current.LevelId == 6)
            {
                objParam.LowerAdminNdCode = Convert.ToInt32(objAnnualAccount.DPIU);
            }
            else
            {
                objParam.LowerAdminNdCode = PMGSYSession.Current.AdminNdCode;
                objParam.AdminNdCode = PMGSYSession.Current.AdminNdCode;
            }

            ///objAnnualAccount = objAccountBAL.GetAnnualAccountList(objParam);

            #region

            PMGSYEntities dbContext = new PMGSYEntities();
            AnnualAccount account = new AnnualAccount();
            CommonFunctions commomFuncObj = new CommonFunctions();
            try
            {
                string rptType = string.Empty;
                string fundType = string.Empty;
                account.CreditDebit = objParam.CreditDebit;
                account.Year = objParam.Year;

                objAnnualAccount.AdminNdCode = objParam.AdminNdCode;
                objAnnualAccount.CreditDebit = objParam.CreditDebit;
                objAnnualAccount.FundType = objParam.FundType;
                objAnnualAccount.Year = objParam.Year;
                objAnnualAccount.Selection = objParam.Selection;
                objAnnualAccount.DpiuSelection = Convert.ToString(objAnnualAccount.DPIU) == null ? "-" : Convert.ToString(objAnnualAccount.DPIU);
                //objAnnualAccount.PIU = objAnnualAccount.DpiuSelection;

                if (PMGSYSession.Current.LevelId == 5)
                {
                    objAnnualAccount.StateName = PMGSYSession.Current.StateName;
                    objAnnualAccount.PIUName = PMGSYSession.Current.DepartmentName;
                    objAnnualAccount.NodalAgencyName = "-";
                }
                else
                {
                    objAnnualAccount.NodalAgencyName = objAnnualAccount.StateName.Substring(objAnnualAccount.StateName.IndexOf("(") + 1, objAnnualAccount.StateName.IndexOf(")") - objAnnualAccount.StateName.IndexOf("(") - 1);

                    objAnnualAccount.StateName = objAnnualAccount.StateName.Substring(0, objAnnualAccount.StateName.IndexOf("("));
                }
                int levelId = 0;
                if (PMGSYSession.Current.LevelId == 4)
                {
                    if (objParam.DPIUSelection != "0" && (objParam.DPIUSelection != null))
                    {
                        rptType = "A";
                    }
                    else
                    {
                        rptType = "O";
                    }
                }
                else if (PMGSYSession.Current.LevelId == 5)
                {
                    rptType = "A";
                }
                else if (objParam.Selection == "R")
                {
                    rptType = "O";
                }
                else if (objParam.Selection == "D" && objParam.DPIUSelection != "0")
                {
                    rptType = "A";
                }

                if (objParam.Selection == "S")
                {
                    //account.lstAnnualReport = dbContext.USP_RPT_SHOW_YEARLY_ACCOUNT_SELF(objParam.AdminNdCode, objParam.Year, objParam.FundType, objParam.CreditDebit).ToList<USP_RPT_SHOW_YEARLY_ACCOUNT_SELF_Result>();
                    //account.lstReportDPIU = dbContext.USP_RPT_SHOW_YEARLY_ACCOUNT_ALLPIU(objParam.AdminNdCode, objParam.Year, objParam.FundType, objParam.CreditDebit).ToList<USP_RPT_SHOW_YEARLY_ACCOUNT_ALLPIU_Result>();

                    //account.TotalCreditDebit = account.lstAnnualReport.Sum(m => m.OB_AMT);
                    //account.TotalOpeningAmount = account.lstAnnualReport.Sum(m => m.YEARLY_AMT);

                    //account.TotalCreditDebitDPIU = account.lstReportDPIU.Sum(m => m.OB_AMT);
                    //account.TotalOpeningAmountDPIU = account.lstReportDPIU.Sum(m => m.YEARLY_AMT);
                    levelId = 6;
                }
                else if (objParam.Selection == "R")
                {
                    //account.lstAnnualReport = dbContext.USP_RPT_SHOW_YEARLY_ACCOUNT_SELF(objParam.AdminNdCode, objParam.Year, objParam.FundType, objParam.CreditDebit).ToList<USP_RPT_SHOW_YEARLY_ACCOUNT_SELF_Result>();

                    //account.TotalCreditDebit = account.lstAnnualReport.Sum(m => m.OB_AMT);
                    //account.TotalOpeningAmount = account.lstAnnualReport.Sum(m => m.YEARLY_AMT);
                    levelId = 4;
                }
                if (objParam.Selection == "D" || PMGSYSession.Current.LevelId == 5)
                {
                    //if (objParam.DPIUSelection == "0")
                    //{

                    //    account.lstReportDPIU = dbContext.USP_RPT_SHOW_YEARLY_ACCOUNT_ALLPIU(objParam.AdminNdCode, objParam.Year, objParam.FundType, objParam.CreditDebit).ToList<USP_RPT_SHOW_YEARLY_ACCOUNT_ALLPIU_Result>();

                    //    account.TotalCreditDebit = account.lstReportDPIU.Sum(m => m.OB_AMT);
                    //    account.TotalOpeningAmount = account.lstReportDPIU.Sum(m => m.YEARLY_AMT);
                    //}
                    //else
                    //{
                    //    account.lstAnnualReport = dbContext.USP_RPT_SHOW_YEARLY_ACCOUNT_SELF(objParam.LowerAdminNdCode, objParam.Year, objParam.FundType, objParam.CreditDebit).ToList<USP_RPT_SHOW_YEARLY_ACCOUNT_SELF_Result>();

                    //    account.TotalCreditDebitDPIU = account.lstAnnualReport.Sum(m => m.OB_AMT);
                    //    account.TotalOpeningAmountDPIU = account.lstAnnualReport.Sum(m => m.YEARLY_AMT);
                    //}
                    if (Convert.ToInt32(objAnnualAccount.DPIU) > 0)
                    {
                        objAnnualAccount.AdminNdCode = Convert.ToInt32(objAnnualAccount.DPIU);
                        objAnnualAccount.PIUName = dbContext.ADMIN_DEPARTMENT.Where(m => m.ADMIN_ND_CODE == objAnnualAccount.AdminNdCode).Select(m => m.ADMIN_ND_NAME).FirstOrDefault();
                    }
                    levelId = 5;
                }
                var rptHeader = dbContext.SP_ACC_Get_Report_Header_Information("AccountReports/Account", "AnnualAccountLayout", PMGSYSession.Current.FundType, levelId, rptType).ToList<SP_ACC_Get_Report_Header_Information_Result>().FirstOrDefault();


                if (rptHeader == null)
                {
                    objAnnualAccount.FormNo = "-";
                    objAnnualAccount.ReportName = "-";
                    objAnnualAccount.ReportParaName = "-";
                    //objAnnualAccount.FundType = String.Empty;
                    objAnnualAccount.Reference = "-";
                    //objAnnualAccount. = string.Empty;
                    objAnnualAccount.FundTypeName = "-";
                    //objAnnualAccount.PIUName = "-";
                }
                else
                {
                    objAnnualAccount.FormNo = rptHeader.REPORT_FORM_NO;
                    objAnnualAccount.ReportName = rptHeader.REPORT_NAME;
                    objAnnualAccount.ReportParaName = rptHeader.REPORT_PARAGRAPH_NAME;
                    objAnnualAccount.FundTypeName = commomFuncObj.GetFundName(PMGSYSession.Current.FundType);
                }

                //if (objParam.LevelId == 6 || objParam.LevelId == 4 && objParam.Selection == "D" && objParam.DPIUSelection != "0")
                //{
                //    account.PIU = dbContext.ADMIN_DEPARTMENT.Where(m => m.ADMIN_ND_CODE == objParam.LowerAdminNdCode).Select(m => m.ADMIN_ND_NAME).FirstOrDefault();
                //    account.NodalAgency = dbContext.ADMIN_DEPARTMENT.Where(m => m.ADMIN_ND_CODE == objParam.AdminNdCode).Select(m => m.ADMIN_ND_NAME).FirstOrDefault();
                //}

                if (objParam.LevelId == 5)
                {
                    objAnnualAccount.PIUName = dbContext.ADMIN_DEPARTMENT.Where(m => m.ADMIN_ND_CODE == objParam.LowerAdminNdCode).Select(m => m.ADMIN_ND_NAME).FirstOrDefault();
                    objAnnualAccount.NodalAgencyName = dbContext.ADMIN_DEPARTMENT.Where(m => m.ADMIN_ND_CODE == (dbContext.ADMIN_DEPARTMENT.Where(p => p.ADMIN_ND_CODE == objParam.LowerAdminNdCode).Select(p => p.MAST_PARENT_ND_CODE).FirstOrDefault())).Select(m => m.ADMIN_ND_NAME).FirstOrDefault();
                    //account.PIU = dbContext.ADMIN_DEPARTMENT.Where(m => m.ADMIN_ND_CODE == objParam.LowerAdminNdCode).Select(m => m.ADMIN_ND_NAME).FirstOrDefault();
                    //account.NodalAgency = dbContext.ADMIN_DEPARTMENT.Where(m => m.ADMIN_ND_CODE == (dbContext.ADMIN_DEPARTMENT.Where(p => p.ADMIN_ND_CODE == objParam.LowerAdminNdCode).Select(p => p.MAST_PARENT_ND_CODE).FirstOrDefault())).Select(m => m.ADMIN_ND_NAME).FirstOrDefault();
                    objParam.Selection = "D";
                    objParam.DPIUSelection = Convert.ToString(objAnnualAccount.AdminNdCode);
                }
                else
                {
                    //objAnnualAccount.PIUName = "-";

                    //account.NodalAgency = dbContext.ADMIN_DEPARTMENT.Where(m => m.ADMIN_ND_CODE == objParam.AdminNdCode).Select(m => m.ADMIN_ND_NAME).FirstOrDefault();
                }

                objAnnualAccount.Selection = objParam.Selection == null ? "0" : objParam.Selection;
                objAnnualAccount.DpiuSelection = objParam.DPIUSelection == null ? "0" : objParam.DPIUSelection;
                ///return account;
                if (objAnnualAccount.PIUName == null)
                {

                    if (objParam.Selection == "D")
                    {
                        objAnnualAccount.PIUName = "All DPIU";
                    }
                    else
                    {
                        objAnnualAccount.PIUName = "-";
                    }
                }

            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                throw ex;
            }
            finally
            {
                if (dbContext != null)
                {
                    dbContext.Dispose();
                }
            }

            #endregion
            return PartialView(objAnnualAccount);

        }

        #endregion AnnualAccount

        #region Income and Expenditure

        /// <summary>
        /// Shows Income And Expenditure Report Header and Grid
        /// </summary>
        /// <returns></returns>
        [Audit]
        public ActionResult IncomeAndExpenditureLayout()
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
            CommonFunctions commomFuncObj = new CommonFunctions();
            PMGSY.Models.Report.ReportFilter objParam = new PMGSY.Models.Report.ReportFilter
            {
                Month = PMGSYSession.Current.AccMonth != 0 ? PMGSYSession.Current.AccMonth : Convert.ToInt16(DateTime.Now.Month),
                Year = PMGSYSession.Current.AccYear != 0 ? PMGSYSession.Current.AccYear : Convert.ToInt16(DateTime.Now.Year),
                AdminNdCode = PMGSYSession.Current.AdminNdCode,
                FundType = PMGSYSession.Current.FundType,
                LevelId = PMGSYSession.Current.LevelId
            };

            PMGSY.Areas.AccountReports.Models.ReportFormModel reportModel = new PMGSY.Areas.AccountReports.Models.ReportFormModel
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
        public ActionResult ListIncomeAndExpenditureReport(int? Month, int? Year, int? ndCode, int? rlevel, int? allPiu, int? duration, string dpiu, string nodalAgency)
        {
            CommonFunctions commomFuncObj = new CommonFunctions();
            try
            {
                string flag = string.Empty;
                long totalRecords;
                int LevelId = 0;
                string ReportType = string.Empty;

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
                }
                else if (rlevel == 2)
                {
                    LevelId = 5;
                    ReportType = "O";
                }

                lstHeader = dbContext.SP_ACC_Get_Report_Header_Information("AccountReports/Account", "IncomeAndExpenditureLayout", PMGSYSession.Current.FundType, LevelId, ReportType).FirstOrDefault();

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
                //var jsonData = new
                //{
                //    rows = objReportBAL.ListIncomeAndExpenditureDetails(Month, Year, ndCode, rlevel, allPiu, duration, out totalRecords),
                //    total = 0,
                //    page = 0,
                //    records = totalRecords,
                //    reportHeader = new
                //    {
                //        formNumber = ReportNumber,
                //        fundType = FundName,
                //        reportHeading = ReportName,
                //        refference = ReportPara
                //    }
                //};
                //return Json(jsonData);
                PMGSY.Areas.AccountReports.Models.ReportFormModel reportModel = new PMGSY.Areas.AccountReports.Models.ReportFormModel();

                reportModel.AdminNdCode = Convert.ToInt32(ndCode);
                reportModel.Month = Convert.ToInt16(Month);
                reportModel.Year = Convert.ToInt16(Year);
                reportModel.LevelId = Convert.ToInt32(rlevel);
                reportModel.isAllDPIU = Convert.ToInt32(allPiu);
                reportModel.FundType = PMGSYSession.Current.FundType;
                reportModel.FormName = ReportNumber;
                reportModel.ReportHeading = ReportName;
                reportModel.Paragraph = ReportPara;

                if (Convert.ToInt16(Month) > 0)
                {
                    string MonthName = commomFuncObj.getMonthText(Convert.ToInt16(Month));
                    reportModel.YearName = MonthName + "-" + Convert.ToString(reportModel.Year);
                }
                else
                {
                    reportModel.YearName = Convert.ToString(reportModel.Year) + "-" + Convert.ToString(reportModel.Year + 1);
                }

                if (dpiu != null && dpiu != string.Empty)
                {
                    reportModel.PIUName = dpiu.Trim();
                }
                else
                {
                    reportModel.PIUName = "-";
                }

                if (PMGSYSession.Current.LevelId == 5)
                {
                    reportModel.PIUName = PMGSYSession.Current.DepartmentName;
                    nodalAgency = objReportBAL.GetNodalAgency(Convert.ToInt32(PMGSYSession.Current.ParentNDCode));
                }

                if (nodalAgency != null && nodalAgency != string.Empty)
                {
                    reportModel.NodalAgencyName = nodalAgency.Trim();
                }
                else
                {
                    reportModel.NodalAgencyName = "-";
                }

                return View(reportModel);
            }
            catch
            {
                return null;
            }
        }

        #endregion Income and Expenditure

        #region Chequebook details

        /// <summary>
        /// ChequeBookDetails() action is used to display Cheque book details search form.
        /// </summary>
        /// <returns></returns>
        [Audit]
        public ActionResult ChequeBookLayout()
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

                if (PMGSYSession.Current.LevelId == 6 || PMGSYSession.Current.LevelId == 4)
                {
                    //Populate SRRDA
                    ViewData["SRRDA"] = new SelectList(objCommonFunction.PopulateNodalAgencies(), "Value", "Text");
                }
                //populate CheckBook Series
                if (PMGSYSession.Current.LevelId == 5) //DPIU
                {
                    ViewData["ChequebookSeries"] = new SelectList(objPaymentDal.GetchequebookSeries(PMGSYSession.Current.AdminNdCode, PMGSYSession.Current.FundType, PMGSYSession.Current.LevelId), "Value", "Text");
                }
                else if (PMGSYSession.Current.LevelId == 4 || PMGSYSession.Current.LevelId == 6)  //SRRDA and MORD
                {
                    ViewData["ChequebookSeries"] = new SelectList(objPaymentDal.GetchequebookSeries(0, PMGSYSession.Current.FundType, PMGSYSession.Current.LevelId), "Value", "Text");
                    //ViewData["ChequebookSeries"] = new SelectList(objPaymentDal.GetchequebookSeries(PMGSYSession.Current.AdminNdCode, PMGSYSession.Current.FundType, PMGSYSession.Current.LevelId), "Value", "Text");
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


                return View(chequebookDetailsViewModel);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);

                return View(new CheckBookDetailsViewFilterModel());
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
        public ActionResult ShowChequebookReport(CheckBookDetailsViewFilterModel checkbookDetailsViewFilterModel)
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

                //checkbookDetailsViewFilterModel = objAccountBAL.getCheckBookDetails(checkbookDetailsViewFilterModel);

                #region
                PMGSYEntities dbcontext = new PMGSYEntities();
                try
                {
                    //store ADMIN_ND_CODE
                    int AdminNDCode = 0;
                    int SrrdaNDCode = 0;


                    //if DPIU Dropdown is selected then AdminNDCode variable stores selected dropdown ADMIN_ND_CODE value 
                    //old code
                    //if (checkbookDetailsViewFilterModel.DPIU > 0)
                    //{
                    //    AdminNDCode = checkbookDetailsViewFilterModel.DPIU;
                    //}
                    //else
                    //{ //else we get ADMIN_ND_CODE from session
                    //    AdminNDCode = PMGSYSession.Current.AdminNdCode;
                    //}

                    //New Code for All PIU Option 15-July-2014 start                
                    //Mord Level
                    if (PMGSYSession.Current.LevelId == 6 && checkbookDetailsViewFilterModel.DPIU == 0)
                    {
                        if (checkbookDetailsViewFilterModel.CheckbookMonthYearWise.Equals("M"))
                        {
                            if (checkbookDetailsViewFilterModel.IsSRRDA_DPIU == "S")//SRRDA
                            {
                                AdminNDCode = checkbookDetailsViewFilterModel.SRRDA;
                            }
                            else//DPIU
                            {
                                AdminNDCode = 0; //All PIU Nd Code
                            }
                        }
                        else
                        {
                            AdminNDCode = checkbookDetailsViewFilterModel.SRRDA;
                        }
                        SrrdaNDCode = checkbookDetailsViewFilterModel.SRRDA;//SRRDA ND Code
                    }
                    else if (PMGSYSession.Current.LevelId == 6 && checkbookDetailsViewFilterModel.DPIU != 0)
                    {
                        AdminNDCode = checkbookDetailsViewFilterModel.DPIU;//PIU ND Code
                        SrrdaNDCode = checkbookDetailsViewFilterModel.SRRDA;//SRRDA ND Code
                    }//SRRDA Level
                    else if (PMGSYSession.Current.LevelId == 4 && checkbookDetailsViewFilterModel.DPIU == 0)
                    {
                        if (checkbookDetailsViewFilterModel.CheckbookMonthYearWise.Equals("M"))
                        {
                            if (checkbookDetailsViewFilterModel.IsSRRDA_DPIU == "S")//SRRDA
                            {
                                AdminNDCode = checkbookDetailsViewFilterModel.AdminNdCode;
                            }
                            else//DPIU
                            {
                                AdminNDCode = 0; //All PIU Nd Code
                            }
                        }
                        else
                        {
                            AdminNDCode = checkbookDetailsViewFilterModel.AdminNdCode;
                        }
                        SrrdaNDCode = PMGSYSession.Current.AdminNdCode;//SRRDA ND Code
                    }
                    else if (PMGSYSession.Current.LevelId == 4 && checkbookDetailsViewFilterModel.DPIU != 0)
                    {
                        AdminNDCode = checkbookDetailsViewFilterModel.DPIU;//PIU ND Code
                        SrrdaNDCode = PMGSYSession.Current.AdminNdCode;//SRRDA ND Code
                    }  //PIU Level
                    else if (PMGSYSession.Current.LevelId == 5)
                    {
                        AdminNDCode = PMGSYSession.Current.AdminNdCode;//PIU ND Code                
                        SrrdaNDCode = PMGSYSession.Current.ParentNDCode.Value;//SRRDA ND Code
                    }
                    //New Code for All PIU Option 15-July-2014 end 



                    //state Name
                    if (PMGSYSession.Current.LevelId == 6)
                    {
                        int ndcode = AdminNDCode == 0 ? SrrdaNDCode : AdminNDCode;

                        checkbookDetailsViewFilterModel.StateName = dbcontext.MASTER_STATE.Where(m => m.MAST_STATE_CODE == dbcontext.ADMIN_DEPARTMENT.Where(p => p.ADMIN_ND_CODE == ndcode).Select(a => a.MAST_STATE_CODE).FirstOrDefault()).Select(s => s.MAST_STATE_NAME).FirstOrDefault();
                    }
                    else
                    {
                        checkbookDetailsViewFilterModel.StateName = PMGSYSession.Current.StateName;
                    }
                    //PIU Name
                    //Modified By Abhishek for All PIU Option 15-July-2014 if Condition Added start                
                    if ((checkbookDetailsViewFilterModel.DPIU == 0) && (PMGSYSession.Current.LevelId == 6 || PMGSYSession.Current.LevelId == 4))
                    {
                        checkbookDetailsViewFilterModel.PIUName = "All DPIU";
                    }
                    else
                    {
                        checkbookDetailsViewFilterModel.PIUName = dbcontext.ADMIN_DEPARTMENT.Where(m => m.ADMIN_ND_CODE == AdminNDCode).Select(s => s.ADMIN_ND_NAME).FirstOrDefault();
                    }
                    //Modified By Abhishek for All PIU Option 15-July-2014 if Condition Added end

                    //Bank Name                
                    Boolean BankAccStatus = true;
                    checkbookDetailsViewFilterModel.BankName = dbcontext.ACC_BANK_DETAILS.Where(m => m.ADMIN_ND_CODE == (dbcontext.ADMIN_DEPARTMENT.Where(t => t.ADMIN_ND_CODE == (AdminNDCode == 0 ? SrrdaNDCode : AdminNDCode)).Select(w => w.MAST_PARENT_ND_CODE).FirstOrDefault()) && m.FUND_TYPE == PMGSYSession.Current.FundType && m.BANK_ACC_STATUS == BankAccStatus && m.ACCOUNT_TYPE == "S").Select(s => s.BANK_NAME).FirstOrDefault();

                    //Nodal Agency Name
                    if (PMGSYSession.Current.LevelId == 4)//SRRDA login we get nodal agency name directly from session
                    {
                        checkbookDetailsViewFilterModel.NodalAgencyName = PMGSYSession.Current.DepartmentName;
                    }
                    else
                    { //else find nodal agency name using selected ADMIN_ND_CODE from DPIU Dropdown and its Parent_Nd_Code

                        int ndcode = AdminNDCode == 0 ? SrrdaNDCode : AdminNDCode;
                        checkbookDetailsViewFilterModel.NodalAgencyName = dbcontext.ADMIN_DEPARTMENT.Where(m => m.ADMIN_ND_CODE == (dbcontext.ADMIN_DEPARTMENT.Where(w => w.ADMIN_ND_CODE == ndcode).Select(s => s.MAST_PARENT_ND_CODE).FirstOrDefault())).Select(t => t.ADMIN_ND_NAME).FirstOrDefault();
                    }

                    //call SP_ACC_RPT_PIU_CHEQUE_ISSUED_DETAILS
                    //checkbookDetailsViewFilterModel.lstCheckbookDetails = dbcontext.SP_ACC_RPT_PIU_CHEQUE_ISSUED_DETAILS(PMGSYSession.Current.FundType, AdminNDCode, checkbookDetailsViewFilterModel.Month, checkbookDetailsViewFilterModel.Year, checkbookDetailsViewFilterModel.CheckbookMonthYearWise, checkbookDetailsViewFilterModel.CheckbookSeries, SrrdaNDCode).ToList<SP_ACC_RPT_PIU_CHEQUE_ISSUED_DETAILS_Result>();

                    //find total no of records
                    //checkbookDetailsViewFilterModel.totalRecords = checkbookDetailsViewFilterModel.lstCheckbookDetails.Count();

                    //if ((checkbookDetailsViewFilterModel.CheckbookMonthYearWise == "M") && (checkbookDetailsViewFilterModel.Month > 0) && (checkbookDetailsViewFilterModel.Year > 0))
                    //{
                    //    //call SP_ACC_RPT_PIU_CHEQUE_ISSUED_ABSTRACT_Result
                    //    checkbookDetailsViewFilterModel.lstChequeIssuedAbstract = dbcontext.SP_ACC_RPT_PIU_CHEQUE_ISSUED_ABSTRACT(PMGSYSession.Current.FundType, AdminNDCode, checkbookDetailsViewFilterModel.Month, checkbookDetailsViewFilterModel.Year, SrrdaNDCode).ToList<SP_ACC_RPT_PIU_CHEQUE_ISSUED_ABSTRACT_Result>();

                    //    //call SP_ACC_RPT_PIU_CHEQUE_OUTSTANDING_DETAILS_Result
                    //    checkbookDetailsViewFilterModel.lstChequeOutstandingDetails = dbcontext.SP_ACC_RPT_PIU_CHEQUE_OUTSTANDING_DETAILS(PMGSYSession.Current.FundType, AdminNDCode, checkbookDetailsViewFilterModel.Month, checkbookDetailsViewFilterModel.Year, SrrdaNDCode).ToList<SP_ACC_RPT_PIU_CHEQUE_OUTSTANDING_DETAILS_Result>();
                    //}

                    //Set Report Header        
                    CommonFunctions objCommonFunction = new CommonFunctions();
                    int levelID = 0;

                    //Modified by abhishek kamble 10-oct-2013
                    if (PMGSYSession.Current.LevelId == 6)
                    {
                        levelID = 5;
                    }
                    else
                    {
                        levelID = PMGSYSession.Current.LevelId;
                    }

                    var ReportHeader = dbcontext.SP_ACC_Get_Report_Header_Information("AccountReports/Account", "ChequeBookLayout", PMGSYSession.Current.FundType, levelID, "O").ToList<SP_ACC_Get_Report_Header_Information_Result>().FirstOrDefault();

                    if (ReportHeader == null)
                    {
                        checkbookDetailsViewFilterModel.FundTypeName = objCommonFunction.GetFundName(PMGSYSession.Current.FundType);
                        checkbookDetailsViewFilterModel.ReportName = String.Empty;
                        checkbookDetailsViewFilterModel.ReportParagraphName = String.Empty;
                        checkbookDetailsViewFilterModel.ReportFormNumber = String.Empty;
                    }
                    else
                    {
                        checkbookDetailsViewFilterModel.FundTypeName = objCommonFunction.GetFundName(PMGSYSession.Current.FundType);
                        checkbookDetailsViewFilterModel.ReportName = checkbookDetailsViewFilterModel.IsSRRDA_DPIU == "S" ? "SRRDA-Wise Register of Cheques Issued" : ReportHeader.REPORT_NAME;
                        checkbookDetailsViewFilterModel.ReportParagraphName = ReportHeader.REPORT_PARAGRAPH_NAME;
                        checkbookDetailsViewFilterModel.ReportFormNumber = ReportHeader.REPORT_FORM_NO;
                    }

                    if (checkbookDetailsViewFilterModel.BankName == null)
                    {
                        checkbookDetailsViewFilterModel.BankName = "-";
                    }

                    checkbookDetailsViewFilterModel.AdminNdCode = AdminNDCode;
                    checkbookDetailsViewFilterModel.SRRDANdCode = SrrdaNDCode;

                    checkbookDetailsViewFilterModel.FundType = PMGSYSession.Current.FundType;

                    if (checkbookDetailsViewFilterModel.CheckbookSeries > 0)
                    {
                        checkbookDetailsViewFilterModel.YearName = checkbookDetailsViewFilterModel.CheckbookSeriesName;
                    }
                    else
                    {
                        checkbookDetailsViewFilterModel.YearName = checkbookDetailsViewFilterModel.MonthName + " - " + checkbookDetailsViewFilterModel.YearName;
                    }

                }
                catch (Exception ex)
                {
                    Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);

                    return null;
                }
                finally
                {
                    if (dbcontext != null)
                    {
                        dbcontext.Dispose();
                    }
                }
                #endregion

                return PartialView(checkbookDetailsViewFilterModel);
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
                short lvlId = 0;

                if (!(String.IsNullOrEmpty(Request.Params["IsSRRDA_DPIU"])))
                {
                    lvlId = Request.Params["IsSRRDA_DPIU"].Equals("S") ? (short)4 : (short)5;
                }

                adminNdCode = adminNdCode == 0 ? PMGSYSession.Current.AdminNdCode : adminNdCode;

                return Json(new SelectList(objPaymentDal.GetchequebookSeries(adminNdCode, PMGSYSession.Current.FundType, lvlId), "Value", "Text"));
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);

                return Json(false);
            }
        }

        #endregion Chequebook details

        #region BillDetails

        /// <summary>
        /// Action is used to load the search form.
        /// </summary>
        /// <returns></returns>
        [Audit]
        public ActionResult DisplayBillLayout()
        {
            try
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
                return View(accModel);
            }
            catch(Exception ex)
            {
                ErrorLog.LogError(ex, "AccountReport().AccounController.ShowBillReport");
                return null;
            }
        }

        /// <summary>
        /// Action is used to load bill details list view based on search parameter.
        /// </summary>
        /// <param name="accModel">Model object contains parameter which are selected from search form.</param>
        /// <returns></returns>
        [HttpPost]
        [Audit]
        public ActionResult ShowBillReport(AccountBillViewModel accModel)
        {
            //modified by abhishek kamble 12-nov-2013
            try
            {
                accModel.levelId = PMGSYSession.Current.LevelId;
                accModel.FundType = PMGSYSession.Current.FundType;
                //accModel = objAccountBAL.getBillDetails(accModel);

                #region

                PMGSYEntities dbContext = new PMGSYEntities();
                CommonFunctions objComm = new CommonFunctions();
                try
                {
                    if (ModelState.IsValid)
                    {

                        int AdminNDCode = 0;
                        int AllPIU = 0;

                        if (accModel.NodalAgency == "D")
                        {
                            AllPIU = accModel.DPIU == 0 ? 1 : 0;
                        }

                        if (accModel.NodalAgency == "D" && accModel.DPIU > 0)
                        {
                            accModel.levelId = 5;
                        }

                        //check srrda/DPIU
                        if (accModel.DPIU > 0)
                        {
                            AdminNDCode = accModel.DPIU;
                        }
                        else
                        {
                            AdminNDCode = PMGSYSession.Current.AdminNdCode;
                        }

                        DateTime? StartDate = null;
                        DateTime? EndDate = null;
                        if (accModel.StartDate != null && accModel.EndDate != null)
                        {
                            StartDate = objComm.GetStringToDateTime(accModel.StartDate);
                            EndDate = objComm.GetStringToDateTime(accModel.EndDate);
                        }

                        if (accModel.levelId == 5)
                        {
                            //accModel.DPIUBySRRDA = dbContext.ADMIN_DEPARTMENT.Where(m => m.ADMIN_ND_CODE == AdminNDCode).Select(m => m.ADMIN_ND_NAME).FirstOrDefault();
                            accModel.DPIUName = dbContext.ADMIN_DEPARTMENT.Where(m => m.ADMIN_ND_CODE == AdminNDCode).Select(m => m.ADMIN_ND_NAME).FirstOrDefault();
                            accModel.NodalAgency = dbContext.ADMIN_DEPARTMENT.Where(m => m.ADMIN_ND_CODE == (dbContext.ADMIN_DEPARTMENT.Where(p => p.ADMIN_ND_CODE == AdminNDCode).Select(p => p.MAST_PARENT_ND_CODE).FirstOrDefault())).Select(m => m.ADMIN_ND_NAME).FirstOrDefault();
                        }
                        else
                        {
                            accModel.NodalAgency = dbContext.ADMIN_DEPARTMENT.Where(m => m.ADMIN_ND_CODE == AdminNDCode).Select(m => m.ADMIN_ND_NAME).FirstOrDefault();
                            accModel.DPIUName = null;
                        }


                        //accModel.lstAccountBillDetails = dbContext.SP_ACC_RPT_DISPALY_Bill_DETAILS(PMGSYSession.Current.FundType, AdminNDCode, accModel.Month, accModel.Year, StartDate, EndDate, accModel.BillType, accModel.rType, AllPIU).ToList<SP_ACC_RPT_DISPALY_Bill_DETAILS_Result>();
                        //accModel.TotalRecords = accModel.lstAccountBillDetails.Count();
                        //return accModel;

                        if (accModel.StartDate == null || accModel.StartDate == string.Empty)
                        {
                            accModel.StartDate = "-";
                        }
                        else
                        {
                            accModel.StartDate = Convert.ToDateTime(accModel.StartDate).ToString("MM/dd/yyyy");
                        }
                        if (accModel.EndDate == null || accModel.EndDate == string.Empty)
                        {
                            accModel.EndDate = "-";
                        }
                        else
                        {
                            accModel.EndDate = Convert.ToDateTime(accModel.EndDate).ToString("MM/dd/yyyy");
                        }
                        accModel.AdminNdCode = AdminNDCode;
                        if (accModel.DPIUName == null)
                        {
                            accModel.DPIUName = "-";
                        }
                        if (accModel.Month > 0)
                        {
                            string MonthName = objComm.getMonthText(accModel.Month);
                            accModel.YearName = MonthName + "-" + accModel.Year;
                        }
                        else
                        {
                            accModel.YearName = accModel.Year + "-" + Convert.ToString(accModel.Year + 1);
                        }

                        if (PMGSYSession.Current.StateName != null && PMGSYSession.Current.StateName != string.Empty)
                        {
                            accModel.StateName = PMGSYSession.Current.StateName;
                        }
                        else
                        {
                            PMGSYSession.Current.StateName = "-";
                        }

                        accModel.Selection = accModel.rType == "M" ? "Monthly" : accModel.rType == "Y" ? "Yearly" : accModel.rType == "P" ? "Periodic" : string.Empty;
                        //accModel.sDate = accModel.StartDate;
                        //accModel.eDate = accModel.EndDate;
                        accModel.isAllPiu = AllPIU;
                        return PartialView(accModel);
                    }
                    return PartialView(accModel);
                }
                catch (Exception ex)
                {
                    Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                    return null;
                }
                finally
                {

                    if (dbContext != null)
                    {
                        dbContext.Dispose();
                    }
                }

                #endregion
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "AccountReport().AccounController.ShowBillReport");
                return null;
            }


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
            string sidxNew = sidx.Replace("asc,", "").Trim();
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
                total = 0,
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
                    SelectList lstYears = new SelectList(objAccountDAL.getAllYears(), "Value", "Text", PMGSYSession.Current.AccYear);
                    return Json(lstYears);
                }
                else
                {
                    return Json(new SelectList(objAccountDAL.getAllYears(), "Value", "Text"));
                }
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return Json(false);
            }
        }//end function GetYears

        #endregion BillDetails

        #region Remittence and Reconciliation

        [Audit]
        public ActionResult ScheduleBankRemittenceLayout()
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
        public ActionResult GetBankRemittenceReport(int? Month, int? Year, int? ndCode, int? rlevel, int? allPiu, string DPIU, string NodalAgency)
        {
            CommonFunctions commomFuncObj = new CommonFunctions();
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
            //List<ScheduleReconciliation> lstReconciliation = objReconciliation.GetReportBAL(15, arrObject);

            #region

            #endregion

            //var jsonData = new
            //{
            //    rows = lstReconciliation,
            //    total = 0,
            //    page = 0,
            //    records = lstReconciliation.Count,
            //    reportHeader = new
            //    {
            //        formNumber = formNumber,
            //        scheduleNo = scheduleNo,
            //        reportHeading = reportHeading,
            //        fundType = fundType,
            //        refference = refference
            //    }
            //};
            //return Json(jsonData);
            ScheduleModel objScheduleModel = new ScheduleModel();

            objScheduleModel.AdminNdCode = Convert.ToInt32(ndCode);
            objScheduleModel.Month = Convert.ToInt16(Month);
            objScheduleModel.Year = Convert.ToInt16(Year);
            objScheduleModel.FundType = PMGSYSession.Current.FundType;
            objScheduleModel.isallPiu = Convert.ToInt32(allPiu);
            objScheduleModel.FormNumber = formNumber;
            objScheduleModel.ReportName = reportHeading;
            objScheduleModel.Paragraph1 = refference;

            if (PMGSYSession.Current.LevelId == 5)
            {
                objScheduleModel.PiuName = PMGSYSession.Current.DepartmentName;
            }
            else
            {
                if ((PMGSYSession.Current.LevelId == 4) && (rlevel == 1))
                {
                    objScheduleModel.PiuName = "-";
                }
                else
                {
                    objScheduleModel.PiuName = DPIU;
                }
            }
            if (NodalAgency != null && NodalAgency != string.Empty)
            {
                objScheduleModel.NodalAgencyName = NodalAgency;
            }
            else
            {
                objScheduleModel.NodalAgencyName = objReportBAL.GetNodalAgency(Convert.ToInt32(PMGSYSession.Current.ParentNDCode));
            }
            if (Convert.ToInt16(Month) > 0)
            {
                string MonthName = commomFuncObj.getMonthText(Convert.ToInt16(Month));
                objScheduleModel.YearName = MonthName + "-" + objScheduleModel.Year;
            }
            else
            {
                objScheduleModel.YearName = objScheduleModel.Year + "-" + Convert.ToString(objScheduleModel.Year + 1);
            }

            return PartialView(objScheduleModel);
        }

        public PartialViewResult AccountFilter(AccountFilterModel filterViewModel)
        {
            CommonFunctions commomFuncObj = new CommonFunctions();

            int RptLevel = 0;
            int agencyCode = 0;

            //AccountFilterModel filterViewModel = new AccountFilterModel();

            PMGSY.Models.Report.ReportFilter objParam = new PMGSY.Models.Report.ReportFilter
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

            PMGSY.Models.Report.Account.AccountFilterModel filterModel = new PMGSY.Models.Report.Account.AccountFilterModel
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

        #endregion Remittence and Reconciliation

        #region Utilization and Reconciliation

        [Audit]
        public ActionResult ScheduleUtilizationLayout()
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
        public ActionResult GetUtilizationReport(int? month, int? year, int? ndcode, int? rlevel, int? allpiu, string DPIU, string NodalAgency)
        {
            CommonFunctions commomFuncObj = new CommonFunctions();
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
            if (reportMaster != null)
            {
                ACC_RPT_REPORT_PROPERTY[] reportHeader = reportMaster.ACC_RPT_REPORT_PROPERTY.ToArray<ACC_RPT_REPORT_PROPERTY>();
                formNumber = reportHeader[0].PROP_VALUE;
                scheduleNo = reportHeader[1].PROP_VALUE == null ? "-" : reportHeader[1].PROP_VALUE;
                reportHeading = reportHeader[2].PROP_VALUE;
                fundType = reportHeader[3].PROP_VALUE;
                refference = reportHeader[4].PROP_VALUE;
            }

            object[] arrObject = new object[] { PMGSYSession.Current.FundType, month, year, ndcode, allpiu };

            ReportBAL.ReportSPBAL<ScheduleUtilization> objSchedule = new ReportBAL.ReportSPBAL<ScheduleUtilization>();
            //List<ScheduleUtilization> lstSchedule = objSchedule.GetReportBAL(14, arrObject);
            //var jsonData = new
            //{
            //    rows = lstSchedule,
            //    total = 0,
            //    page = 0,
            //    records = lstSchedule.Count,
            //    reportHeader = new
            //    {
            //        formNumber = formNumber,
            //        scheduleNo = scheduleNo,
            //        reportHeading = reportHeading,
            //        fundType = fundType,
            //        refference = refference
            //    }
            //};
            //return Json(jsonData);

            ScheduleModel objScheduleModel = new ScheduleModel();

            objScheduleModel.AdminNdCode = Convert.ToInt32(ndcode);
            objScheduleModel.Month = Convert.ToInt16(month);
            objScheduleModel.Year = Convert.ToInt16(year);
            objScheduleModel.FundType = PMGSYSession.Current.FundType;
            objScheduleModel.isallPiu = Convert.ToInt32(allpiu);
            objScheduleModel.FormNumber = formNumber;
            objScheduleModel.ReportName = reportHeading;
            objScheduleModel.Paragraph1 = refference;

            if (PMGSYSession.Current.LevelId == 5)
            {
                objScheduleModel.PiuName = PMGSYSession.Current.DepartmentName;
            }
            else
            {
                if ((PMGSYSession.Current.LevelId == 4) && (rlevel == 1))
                {
                    objScheduleModel.PiuName = "-";
                }
                else
                {
                    objScheduleModel.PiuName = DPIU;
                }
            }
            if (NodalAgency != null && NodalAgency != string.Empty)
            {
                objScheduleModel.NodalAgencyName = NodalAgency;
            }
            else
            {
                objScheduleModel.NodalAgencyName = objReportBAL.GetNodalAgency(Convert.ToInt32(PMGSYSession.Current.ParentNDCode));
            }
            if (Convert.ToInt16(month) > 0)
            {
                string MonthName = commomFuncObj.getMonthText(Convert.ToInt16(month));
                objScheduleModel.YearName = MonthName + "-" + objScheduleModel.Year;
            }
            else
            {
                objScheduleModel.YearName = objScheduleModel.Year + "-" + Convert.ToString(objScheduleModel.Year + 1);
            }

            return PartialView(objScheduleModel);

        }



        #endregion

        #region Asset Register details

        /// <summary>
        /// AssetRegisterDetails() action is used to display Asset Register details search form.
        /// </summary>
        /// <returns></returns>
        [Audit]
        public ActionResult AssetRegisterLayout()
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

                return View(assetRegisterViewModel);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);

                return View(new AssetRegisterViewModel());
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

        public ActionResult AssetRegisterReport(AssetRegisterViewModel assetRegisterViewModel)
        {
            if (ModelState.IsValid)
            {

                //assetRegisterViewModel = objAccountBAL.getAssetRegisterDetails(assetRegisterViewModel);

                #region

                PMGSYEntities dbContext = new PMGSYEntities();
                try
                {
                    CommonFunctions objCommonFunction = new CommonFunctions();

                    //store ADMIN_ND_CODE
                    int AdminNDCode = 0;

                    //if DPIU Dropdown is selected then AdminNDCode variable stores selected dropdown ADMIN_ND_CODE value 
                    if (assetRegisterViewModel.DPIU > 0)
                    {
                        AdminNDCode = assetRegisterViewModel.DPIU;
                    }
                    else
                    { //else we get ADMIN_ND_CODE from session
                        AdminNDCode = PMGSYSession.Current.AdminNdCode;
                    }
                    if (assetRegisterViewModel.SRRDADPIU == "S")
                    {
                        assetRegisterViewModel.DPIUName = "-";
                    }
                    else
                    {
                        //PIU Name
                        assetRegisterViewModel.DPIUName = dbContext.ADMIN_DEPARTMENT.Where(m => m.ADMIN_ND_CODE == AdminNDCode).Select(s => s.ADMIN_ND_NAME).FirstOrDefault();
                    }
                    //Nodal Agency Name
                    if (PMGSYSession.Current.LevelId == 4)//SRRDA login we get nodal agency name directly from session
                    {
                        assetRegisterViewModel.NodalAgencyName = PMGSYSession.Current.DepartmentName;
                    }
                    else
                    { //else find nodal agency name using selected ADMIN_ND_CODE from DPIU Dropdown and its Parent_Nd_Code
                        assetRegisterViewModel.NodalAgencyName = dbContext.ADMIN_DEPARTMENT.Where(m => m.ADMIN_ND_CODE == (dbContext.ADMIN_DEPARTMENT.Where(w => w.ADMIN_ND_CODE == AdminNDCode).Select(s => s.MAST_PARENT_ND_CODE).FirstOrDefault())).Select(t => t.ADMIN_ND_NAME).FirstOrDefault();
                    }

                    System.DateTime? FromDate = null;
                    System.DateTime? ToDate = null;
                    if (assetRegisterViewModel.FromDate != null && assetRegisterViewModel.ToDate != null)
                    {
                        FromDate = objCommonFunction.GetStringToDateTime(assetRegisterViewModel.FromDate);
                        ToDate = objCommonFunction.GetStringToDateTime(assetRegisterViewModel.ToDate);
                    }

                    //set Month and Year
                    short? Month = null;
                    short? Year = null;
                    if (assetRegisterViewModel.Month != 0 && assetRegisterViewModel.Year != 0)
                    {
                        Month = assetRegisterViewModel.Month;
                        Year = assetRegisterViewModel.Year;
                    }



                    //Added By Abhishek kamble 17-feb-2014
                    if (assetRegisterViewModel.monthlyPeriodicFundWise == "P")
                    {
                        Month = null;
                        Year = null;
                    }

                    //call Stored procedure USP_ACC_RPT_REGISTER_DURABLE_ASSETS
                    assetRegisterViewModel.lstAssetRegisterDetails = dbContext.USP_ACC_RPT_REGISTER_DURABLE_ASSETS(AdminNDCode, assetRegisterViewModel.monthlyPeriodicFundWise, Month, Year, FromDate, ToDate, PMGSYSession.Current.FundType, assetRegisterViewModel.FundCentralState).ToList<USP_ACC_RPT_REGISTER_DURABLE_ASSETS_Result>();

                    if (assetRegisterViewModel.lstAssetRegisterDetails.Count > 0)
                    {
                        assetRegisterViewModel.TotalAmount = assetRegisterViewModel.lstAssetRegisterDetails.Sum(m => (Decimal?)m.TOTAL_AMOUNT);
                    }

                    //call  Function UDF_ACC_GET_ASSET_HEADS
                    assetRegisterViewModel.lstAssetRegisterClassificationDetails = dbContext.UDF_ACC_GET_ASSET_HEADS(PMGSYSession.Current.FundType, assetRegisterViewModel.FundCentralState).ToList<UDF_ACC_GET_ASSET_HEADS_Result>();

                    int counter = 1;
                    int totalRecords = assetRegisterViewModel.lstAssetRegisterClassificationDetails.Count();

                    foreach (var item in assetRegisterViewModel.lstAssetRegisterClassificationDetails)
                    {
                        if (counter < totalRecords)
                        {
                            assetRegisterViewModel.ClassificationCode += item.HEAD_CODE + ", ";
                        }
                        else
                        {
                            assetRegisterViewModel.ClassificationCode += item.HEAD_CODE;
                        }
                        counter++;
                    }

                    //Set Report Header        
                    var ReportHeader = new SP_ACC_Get_Report_Header_Information_Result();
                    ReportHeader = dbContext.SP_ACC_Get_Report_Header_Information(@"AccountReports/Account", "AssetRegisterLayout", PMGSYSession.Current.FundType, PMGSYSession.Current.LevelId, "O").ToList<SP_ACC_Get_Report_Header_Information_Result>().FirstOrDefault();

                    if (ReportHeader == null)
                    {
                        assetRegisterViewModel.FundTypeName = objCommonFunction.GetFundName(PMGSYSession.Current.FundType);
                        assetRegisterViewModel.ReportName = String.Empty;
                        assetRegisterViewModel.ReportParagraphName = String.Empty;
                        assetRegisterViewModel.ReportFormNumber = String.Empty;
                    }
                    else
                    {
                        assetRegisterViewModel.FundTypeName = objCommonFunction.GetFundName(PMGSYSession.Current.FundType);
                        assetRegisterViewModel.ReportName = ReportHeader.REPORT_NAME;
                        assetRegisterViewModel.ReportParagraphName = ReportHeader.REPORT_PARAGRAPH_NAME;
                        assetRegisterViewModel.ReportFormNumber = ReportHeader.REPORT_FORM_NO;
                    }

                    assetRegisterViewModel.AdminNdCode = AdminNDCode;
                    assetRegisterViewModel.FundType = PMGSYSession.Current.FundType;
                    //if (assetRegisterViewModel.Month > 0)
                    //{
                    //    string MonthName = objCommonFunction.getMonthText(assetRegisterViewModel.Month);
                    //    assetRegisterViewModel.YearName = MonthName + "-" + assetRegisterViewModel.Year;
                    //}
                    //else
                    //{
                    //    assetRegisterViewModel.YearName = assetRegisterViewModel.Year + "-" + assetRegisterViewModel.Year + 1;
                    //}

                    if (assetRegisterViewModel.monthlyPeriodicFundWise == "M")
                    {
                        string MonthName = objCommonFunction.getMonthText(assetRegisterViewModel.Month);
                        assetRegisterViewModel.YearName = MonthName + " - " + assetRegisterViewModel.Year;
                    }
                    else
                    {
                        assetRegisterViewModel.YearName = assetRegisterViewModel.FromDate + " - " + assetRegisterViewModel.ToDate;
                    }

                    if (assetRegisterViewModel.FromDate == null || assetRegisterViewModel.FromDate == string.Empty)
                    {
                        assetRegisterViewModel.StartDate = "-";
                    }
                    else
                    {
                        assetRegisterViewModel.StartDate = Convert.ToDateTime(assetRegisterViewModel.StartDate).ToString("MM/dd/yyyy");
                    }
                    if (assetRegisterViewModel.ToDate == null || assetRegisterViewModel.ToDate == string.Empty)
                    {
                        assetRegisterViewModel.EndDate = "-";
                    }
                    else
                    {
                        assetRegisterViewModel.EndDate = Convert.ToDateTime(assetRegisterViewModel.EndDate).ToString("MM/dd/yyyy");
                    }

                    return PartialView(assetRegisterViewModel);
                }
                catch (Exception ex)
                {
                    Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);

                    return PartialView(new AssetRegisterViewModel());
                }
                finally
                {
                    if (dbContext != null)
                    {
                        dbContext.Dispose();
                    }
                }

                #endregion

                return PartialView(assetRegisterViewModel);
            }
            else
            {
                return PartialView(assetRegisterViewModel);
            }
        }

        #endregion Asset Register details

        #region Transfer Entry Order
        [Audit]
        public ActionResult TransferEntryOrderLayout()
        {
            CommonFunctions commomFuncObj = new CommonFunctions();
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
        public ActionResult TransferEntryOrderReport(RptTransferEntryOrder teo)
        {
            CommonFunctions comm = new CommonFunctions();
            try
            {
                RptTrnasferEntryOrderList rptTeoList = new RptTrnasferEntryOrderList();
                PMGSY.Models.Report.ReportFilter objParam = new PMGSY.Models.Report.ReportFilter();
                string piuSrrdaName = String.Empty;
                string srrdaName = String.Empty;
                string fundName = string.Empty;

                //added by abhishek kamble 6-dec-2013
                if (teo.isSRRDA)
                {
                    objParam.AdminNdCode = PMGSYSession.Current.AdminNdCode;
                }
                else
                {

                    if (PMGSYSession.Current.LevelId == 5)
                    {
                        objParam.AdminNdCode = PMGSYSession.Current.AdminNdCode;
                    }
                    else
                    {
                        objParam.AdminNdCode = teo.Dpiu;
                    }
                }

                objParam.FundType = PMGSYSession.Current.FundType;

                objParam.LevelId = PMGSYSession.Current.LevelId;

                LedgerModel info = GetForContextData(objParam);

                // ledgerModel.DistrictDepartment = info.DistrictDepartment;

                //ledgerModel.StateDepartment = info.StateDepartment;
                if (teo.isSRRDA)
                {
                    teo.AdminNDCode = PMGSYSession.Current.AdminNdCode;
                    //piuSrrdaName = "<table><tr><td style='color:green;font-weight:bold;'>Name of SRRDA :</td><td style='color:green'>" + "<span class='ui-state-default' style='background:none;border:none'>" + info.StateDepartment + "</span>" + "</td></tr></table>";
                    srrdaName = info.StateDepartment;
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
                    //srrdaName = "<table><tr><td style='color:green;font-weight:bold;'>Name of SRRDA :</td><td style='color:green'>" + "<span class='ui-state-default' style='background:none;border:none'>" + info.StateDepartment + "</span>" + "</td></tr></table>";
                    //piuSrrdaName = "<table><tr><td style='color:green;font-weight:bold;'>Name of DPIU :</td><td style='color:green'>" + "<span class='ui-state-default' style='background:none;border:none'>" + info.DistrictDepartment + "</span>" + "</td></tr></table>";
                    srrdaName = info.StateDepartment;
                    piuSrrdaName = info.DistrictDepartment;
                }


                teo.FundType = PMGSYSession.Current.FundType;
                if (teo.FundType == "A")
                    fundName = "PMGSY  ADMINISTRATIVE FUND";
                else if (teo.FundType == "P")
                    fundName = "PMGSY  PROGRAMME FUND";
                else if (teo.FundType == "M")
                    fundName = "PMGSY  MAINTENANCE FUND";


                #region

                PMGSYEntities dbContext = new PMGSYEntities();
                //RptTrnasferEntryOrderList rptTeoList = new RptTrnasferEntryOrderList();
                rptTeoList.ListTeo = dbContext.SP_ACC_RPT_DISPLAY_TEO_DETAILS(teo.FundType, teo.AdminNDCode, teo.Month, teo.Year).ToList<SP_ACC_RPT_DISPLAY_TEO_DETAILS_Result>();
                var headerBalanceSheet = dbContext.SP_ACC_Get_Report_Header_Information("AccountReports/Account", "TransferEntryOrderLayout", objParam.FundType, objParam.LevelId, "O").ToList<SP_ACC_Get_Report_Header_Information_Result>().FirstOrDefault();
                if (headerBalanceSheet == null)
                {
                    rptTeoList.FormNumber = String.Empty;
                    rptTeoList.ReportName = String.Empty;
                    rptTeoList.Paragraph = String.Empty;
                }
                else
                {
                    rptTeoList.FormNumber = headerBalanceSheet.REPORT_FORM_NO;
                    rptTeoList.ReportName = headerBalanceSheet.REPORT_NAME;
                    rptTeoList.Paragraph = headerBalanceSheet.REPORT_PARAGRAPH_NAME;
                }

                rptTeoList.AdminNDCode = teo.AdminNDCode;
                rptTeoList.Month = teo.Month;
                rptTeoList.Year = teo.Year;
                rptTeoList.FundType = teo.FundType;
                if (srrdaName == string.Empty)
                {
                    rptTeoList.SrrdaName = "-";
                }
                else
                {
                    rptTeoList.SrrdaName = srrdaName;
                }
                rptTeoList.PIUName = piuSrrdaName == "" ? "-" : piuSrrdaName;

                if (rptTeoList.Month > 0)
                {
                    string MonthName = comm.getMonthText(Convert.ToInt16(rptTeoList.Month));
                    rptTeoList.YearName = MonthName + "-" + rptTeoList.Year;
                }
                else
                {
                    rptTeoList.YearName = rptTeoList.Year + "-" + Convert.ToString(rptTeoList.Year + 1);
                }

                return PartialView(rptTeoList);
                #endregion


            }
            catch (Exception ex)
            {
                throw new Exception("error occured");
                return PartialView(new RptTrnasferEntryOrderList());
            }

        }

        public LedgerModel GetForContextData(PMGSY.Models.Report.ReportFilter objParam)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            try
            {
                LedgerModel model = new LedgerModel();

                //Modified by Abhishek kamble 7-oct-2013
                if ((objParam.LevelId == 5) || (objParam.LowerAdminNdCode != -1))
                {
                    var department = (from distDepartment in dbContext.ADMIN_DEPARTMENT
                                      join stateDepartment in dbContext.ADMIN_DEPARTMENT
                                      on distDepartment.MAST_PARENT_ND_CODE equals stateDepartment.ADMIN_ND_CODE
                                      where distDepartment.ADMIN_ND_CODE == objParam.AdminNdCode
                                      select new { DistrictDepartment = distDepartment.ADMIN_ND_NAME, StateDepartment = stateDepartment.ADMIN_ND_NAME }).FirstOrDefault();

                    //Modified by Abhishek kamble 2-jan-2014
                    if (department != null)
                    {
                        model.DistrictDepartment = department.DistrictDepartment;
                        model.StateDepartment = department.StateDepartment;
                    }
                    if (objParam.Selection == "S")
                    {
                        model.DistrictDepartment = "-";
                    }
                }
                else
                {
                    model.StateDepartment = dbContext.ADMIN_DEPARTMENT.Where(m => m.ADMIN_ND_CODE == objParam.AdminNdCode).Select(m => m.ADMIN_ND_NAME).FirstOrDefault();
                    model.DistrictDepartment = "-";
                }

                return model;
            }
            catch (Exception Ex)
            {
                throw Ex;
            }
            finally
            {
                dbContext.Dispose();
            }
        }
        #endregion

        #region Fund Reconcilation

        [Audit]
        public ActionResult FundReconciliationLayout()
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
        public ActionResult FundReconciliationReport(int? srrdaNDCode, int? month, int? year, int? rlevel, string NodalAgency)
        {
            CommonFunctions commomFuncObj = new CommonFunctions();
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
            object[] arrObject = new object[] { PMGSYSession.Current.FundType, month, year, srrdaNDCode };
            ReportBAL.ReportSPBAL<ScheduleFundReconciliation> objFundReconciliation = new ReportBAL.ReportSPBAL<ScheduleFundReconciliation>();
            //List<ScheduleFundReconciliation> lstFundReconciliation = objFundReconciliation.GetReportBAL(18, arrObject);

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

            //var jsonData = new
            //{
            //    rows = lstFundReconciliation,
            //    total = 0,
            //    page = 0,
            //    records = lstFundReconciliation.Count,
            //    reportHeader = new
            //    {
            //        formNumber = formNumber,
            //        scheduleNo = scheduleNo,
            //        reportHeading = reportHeading,
            //        fundType = fundType,
            //        refference = refference
            //    }
            //};
            //return Json(jsonData);
            ScheduleModel objScheduleModel = new ScheduleModel();

            objScheduleModel.AdminNdCode = Convert.ToInt32(srrdaNDCode);
            objScheduleModel.Month = Convert.ToInt16(month);
            objScheduleModel.Year = Convert.ToInt16(year);
            objScheduleModel.FundType = PMGSYSession.Current.FundType;
            objScheduleModel.FormNumber = formNumber;
            objScheduleModel.ReportName = reportHeading;
            objScheduleModel.Paragraph1 = refference;
            //objScheduleModel.isallPiu = Convert.ToInt32(allpiu);

            //if (PMGSYSession.Current.LevelId == 5)
            //{
            //    objScheduleModel.PiuName = PMGSYSession.Current.DepartmentName;
            //}
            //else
            //{
            //    objScheduleModel.PiuName = DPIU;
            //}
            objScheduleModel.NodalAgencyName = NodalAgency;//objReportBAL.GetNodalAgency(Convert.ToInt32(PMGSYSession.Current.ParentNDCode));

            if (Convert.ToInt16(month) > 0)
            {
                string MonthName = commomFuncObj.getMonthText(Convert.ToInt16(month));
                objScheduleModel.YearName = MonthName + "-" + objScheduleModel.Year;
            }
            else
            {
                objScheduleModel.YearName = objScheduleModel.Year + "-" + Convert.ToString(objScheduleModel.Year + 1);
            }

            return PartialView(objScheduleModel);
        }


        #endregion Fund Reconciliation

        #region Master Sheet
        public ActionResult MasterSheetLayout()
        {
            CommonFunctions cm = new CommonFunctions();
            MasterSheetModel masterSheetModel = new MasterSheetModel();
            masterSheetModel.Year = Request.Params["year"] == null ? DateTime.Now.Year - 1 : Convert.ToInt32(Request.Params["year"]);
            masterSheetModel.YEAR_LIST = cm.PopulateFinancialYear(true);

            return View(masterSheetModel);
        }

        public ActionResult MasterSheetReport(MasterSheetModel mastersheet)
        {

            //PMGSY.Common.FetchCookieData fetchCookieData = new PMGSY.Common.FetchCookieData();

            PMGSY.Models.PMGSYEntities dbcontext = new PMGSY.Models.PMGSYEntities();

            mastersheet.LvlId = 2;
            mastersheet.StateCode = PMGSYSession.Current.StateCode;
            mastersheet.Agency = dbcontext.ADMIN_DEPARTMENT.Where(c => c.ADMIN_ND_CODE == PMGSYSession.Current.AdminNdCode).Select(c => c.MAST_AGENCY_CODE).First();
            mastersheet.StateName = PMGSYSession.Current.StateName;
            mastersheet.Year = Convert.ToInt32(Request.Params["year"]);
            mastersheet.FundType = PMGSYSession.Current.FundType;
            return View(mastersheet);

        }

        #endregion Master Sheet

        #region Authorized Signatory
        public ActionResult AuthorizedSignatoryLayout()
        {
            try
            {
                //if (PMGSYSession.Current.UserId == 0)
                //{
                //    Response.Redirect("/Login/SessionExpire");
                //}
                return View("AuthorizedSignatoryLayout", new AuthorizedSignatoryModel());
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);

                return null;
            }
        }


        public ActionResult AuthorizedSignatoryReport(AuthorizedSignatoryModel authSignatoryViewModel)
        {
            //if (PMGSYSession.Current.UserId == 0)
            //{
            //    Response.Redirect("/Login/SessionExpire");
            //}

            int AdminNDCode = 0;
            int LevelID = 0;

            //if DPIU Dropdown is selected then AdminNDCode variable stores selected dropdown ADMIN_ND_CODE value 
            if (authSignatoryViewModel.DPIU > 0) //DPIU
            {
                AdminNDCode = authSignatoryViewModel.DPIU;
                LevelID = 5;
            }
            else//SRRDA
            { //else we get ADMIN_ND_CODE from session
                AdminNDCode = PMGSYSession.Current.AdminNdCode;
                LevelID = 4;
            }
            authSignatoryViewModel.DPIU = AdminNDCode;
            authSignatoryViewModel.LevelID = LevelID;


            return View(authSignatoryViewModel);
            //return PartialView("AuthorizedSignatoryReport", authSignatoryViewModel);

        }
        #endregion

        #region Statutory Deduction , Deposites , Miscellaneous Advances
        // Added By Rohit A. Jadhav
        public ActionResult ViewStatutoryDeductionLayout()
        {
            RegisterViewModel model = new RegisterViewModel();
            CommonFunctions commomFuncObj = new CommonFunctions();
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

        public ActionResult ViewRegisterDeposits()
        {
            RegisterViewModel model = new RegisterViewModel();
            CommonFunctions commomFuncObj = new CommonFunctions();
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

                //Below line is commented on 15-03-2023
                model.lstPIU = commomFuncObj.PopulateDPIUOfSRRDA(PMGSYSession.Current.AdminNdCode);

                //Below condition is Added on 15-03-2023
                //if (PMGSYSession.Current.LevelId == 4)
                //{
                //    model.lstPIU = commomFuncObj.PopulateAllDPIUOfSRRDA(PMGSYSession.Current.AdminNdCode);
                //    model.lstPIU.Insert(0, (new SelectListItem { Text = "All DPIU", Value = "0", Selected = true }));
                //}
                //else
                //{
                //    model.lstPIU = commomFuncObj.PopulateDPIUOfSRRDA(PMGSYSession.Current.AdminNdCode);
                //}

                model.ReportTitle = "Register of Deposits";


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


        public ActionResult ViewMiscAdvancesLayout()
        {
            RegisterViewModel model = new RegisterViewModel();
            CommonFunctions commomFuncObj = new CommonFunctions();
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



        public ActionResult GetPostRegisterDetails(RegisterViewModel postModel)
        {
            CommonFunctions commomFuncObj = new CommonFunctions();
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
                        model.lstPIU.Insert(0, new SelectListItem { Value = "0", Text = "All DPIU" });
                    }
                    else
                    {
                        model.lstPIU = commomFuncObj.PopulateDPIUOfSRRDA(PMGSYSession.Current.AdminNdCode);
                    }

                    return View("GetPostRegisterDetails", model);
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
                    return View("GetPostRegisterDetails", model);
                }
            }
            catch (Exception)
            {
                return View("GetPostRegisterDetails", model);
            }
        }




        public ActionResult ListRegisterDetails(FormCollection frmCollection)
        {

            PMGSYEntities db = new PMGSYEntities();
            try
            {
                RegisterViewModel model = new RegisterViewModel();
                model.HeadName = Request.Params["HeadName"];
                model.HeadCategoryId = Convert.ToInt32(Request.Params["HeadCategoryId"]);
                switch (PMGSYSession.Current.LevelId)
                {
                    case 4:
                        model.HeadId = Convert.ToInt32(Request.Params["HeadId"]);
                        model.SRRDACode = PMGSYSession.Current.AdminNdCode;
                        model.FundingAgencyCode = Convert.ToInt32(Request.Params["FundingAgencyCode"]);

                        if (Request.Params["ReportType"] == "S")
                        {
                            //model.AdminCode = Convert.ToInt32(Request.Params["SRRDACode"]);
                            model.AdminCode = PMGSYSession.Current.AdminNdCode;
                            model.LevelId = PMGSYSession.Current.LevelId;
                        }
                        else if (Request.Params["ReportType"] == "D")
                        {
                            //model.AdminCode = Convert.ToInt32(Request.Params["DPIUCode"]);
                            //model.LevelId = 5;

                            if (Convert.ToInt32(Request.Params["DPIUCode"]) != 0)
                            {
                                model.AdminCode = Convert.ToInt32(Request.Params["DPIUCode"]);
                                model.LevelId = 5;
                            }
                            else
                            {
                                //Below code commented on 15-03-2023
                                model.AdminCode = Convert.ToInt32(Request.Params["SRRDACode"]);
                                model.LevelId = 4;

                                //Below code Added on 15-03-2023
                                //model.AdminCode = 0;
                                //model.LevelId = 5;
                            }
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
                        model.SRRDACode = PMGSYSession.Current.ParentNDCode.Value;

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
                        model.SRRDACode = Convert.ToInt32(Request.Params["SRRDACode"]);
                        if (!string.IsNullOrEmpty(Request.Params["DPIUCode"]))
                        {
                            if (Convert.ToInt32(Request.Params["DPIUCode"]) != 0)
                            {
                                model.AdminCode = Convert.ToInt32(Request.Params["DPIUCode"]);
                                model.LevelId = 5;
                            }
                            else
                            {
                                model.AdminCode = Convert.ToInt32(Request.Params["SRRDACode"]);
                                model.LevelId = 4;
                            }
                        }

                        model.Month = Convert.ToInt32(Request.Params["Month"]);
                        model.Year = Convert.ToInt32(Request.Params["Year"]);
                        if (Request.Params["Duration"] == "Y")
                        {
                            model.Year = Convert.ToInt32(Request.Params["FinancialYear"]);
                        }
                        model.StateCode = db.ADMIN_DEPARTMENT.Where(m => m.ADMIN_ND_CODE == model.AdminCode).Select(m => m.MAST_STATE_CODE).FirstOrDefault();

                        break;
                    default:
                        break;
                }
                model.Collaboration = db.ADMIN_DEPARTMENT.Where(m => m.ADMIN_ND_CODE == model.AdminCode).Select(m => m.MAST_AGENCY_CODE).FirstOrDefault();
                return View("GetPostRegisterDetails", model);

            }
            catch (Exception)
            {
                return null;
            }
        }



        #endregion

        #region Running Account

        public ActionResult RunningAccountLayout()
        {
            try
            {
                CommonFunctions objCommon = new CommonFunctions();
                RunningAccountViewModel model = new RunningAccountViewModel();

                //ReportDAL objReportDAL = new ReportDAL();
                List<SelectListItem> lstDefault = new List<SelectListItem>();
                lstDefault.Insert(0, new SelectListItem { Value = "0", Text = "Select" });


                //model.ddlYear = new SelectList(objCommon.PopulateFinancialYear(false,false),"Value","Text").ToList();
                model.ddlYear = new SelectList(PopulateYears(PMGSYSession.Current.AdminNdCode, PMGSYSession.Current.LevelId), "Value", "Text").ToList();

                model.ddlMonth = lstDefault;
                //model.ddlDPIU = objCommon.PopulateDPIUOfSRRDA(PMGSYSession.Current.AdminNdCode);
                List<SelectListItem> lstPIU = objCommon.PopulateDPIUOfSRRDA(PMGSYSession.Current.AdminNdCode);
                //All PIU Option To call All PIU SP
                lstPIU.Insert(0, new SelectListItem { Text = "All PIU", Value = "0" });
                model.ddlDPIU = lstPIU;

                return View(model);
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }


        //public ActionResult RunningAccountReport(RunningAccountViewModel postModel)
        //{
        //    CommonFunctions commomFuncObj = new CommonFunctions();
        //    PMGSY.Models.Common.TransactionParams objParam = new PMGSY.Models.Common.TransactionParams();
        //    PMGSYEntities dbContext = new PMGSYEntities();

        //    RunningAccountViewModel model = new RunningAccountViewModel();
        //    try
        //    {
        //        if (ModelState.IsValid)
        //        {
        //            if (PMGSYSession.Current.LevelId == 5)
        //            {
        //                model.ReportType = "A";
        //            }
        //            else if (PMGSYSession.Current.LevelId == 4)
        //            {
        //                model.ReportType = "O";
        //            }


        //            objParam.ADMIN_ND_CODE = PMGSYSession.Current.AdminNdCode;
        //            model.NodalAgency = objReportBAL.GetNodalAgency(objParam.ADMIN_ND_CODE);
        //            var ReportHeader = dbContext.SP_ACC_Get_Report_Header_Information("Reports", "MonthlyAccount", PMGSYSession.Current.FundType, PMGSYSession.Current.LevelId, model.ReportType).ToList<SP_ACC_Get_Report_Header_Information_Result>().FirstOrDefault();

        //            if (ReportHeader == null)
        //            {
        //                model.FundTypeName = commomFuncObj.GetFundName(PMGSYSession.Current.FundType); ;
        //                model.ReportName = String.Empty;
        //                model.ReportParagraphName = String.Empty;
        //                model.ReportFormNumber = String.Empty;
        //            }
        //            else
        //            {
        //                model.FundTypeName = commomFuncObj.GetFundName(PMGSYSession.Current.FundType); ;
        //                model.ReportName = ReportHeader.REPORT_NAME;
        //                model.ReportParagraphName = ReportHeader.REPORT_PARAGRAPH_NAME;
        //                model.ReportFormNumber = ReportHeader.REPORT_FORM_NO;
        //            }
        //            if (PMGSYSession.Current.LevelId == 4)
        //            {
        //                if (postModel.ReportType == "S")
        //                {
        //                    model.SRRDADPIU = "SRRDA";
        //                }
        //                else
        //                {
        //                    if (postModel.DPIUCode == 0)
        //                    {
        //                        model.DPIUName = "All PIU";
        //                    }
        //                    else
        //                    {
        //                        model.DPIUName = dbContext.ADMIN_DEPARTMENT.Where(m => m.ADMIN_ND_CODE == postModel.DPIUCode).Select(m => m.ADMIN_ND_NAME).FirstOrDefault();
        //                    }

        //                }
        //            }

        //            model.NodalAgency = PMGSYSession.Current.DepartmentName;
        //            List<SelectListItem> lstDefault = new List<SelectListItem>();
        //            lstDefault.Insert(0, new SelectListItem { Value = "0", Text = "Select Month" });

        //            if (postModel.ReportType == "S")
        //            {
        //                model.ddlYear = new SelectList(PopulateYears(PMGSYSession.Current.AdminNdCode, PMGSYSession.Current.LevelId), "Value", "Text").ToList();

        //                List<SelectListItem> lstMonth = new List<SelectListItem>();
        //                lstMonth.Insert(0, new SelectListItem { Value = "0", Text = "Select" });
        //                model.ddlMonth = lstMonth;


        //            }
        //            else if (postModel.ReportType == "D")
        //            {

        //                if (postModel.DPIUCode == 0)
        //                {
        //                    model.ddlYear = new SelectList(PopulateYears(PMGSYSession.Current.AdminNdCode, 4), "Value", "Text").ToList();
        //                }
        //                else
        //                {
        //                    model.ddlYear = new SelectList(PopulateYears(postModel.DPIUCode, 5), "Value", "Text").ToList();
        //                }

        //                List<SelectListItem> lstMonth = new List<SelectListItem>();
        //                lstMonth.Insert(0, new SelectListItem { Value = "0", Text = "Select" });
        //                model.ddlMonth = lstMonth;

        //            }
        //            else
        //            {
        //                model.ddlYear = new SelectList(PopulateYears(PMGSYSession.Current.AdminNdCode, PMGSYSession.Current.LevelId), "Value", "Text").ToList();

        //                List<SelectListItem> lstMonth = new List<SelectListItem>();
        //                lstMonth.Insert(0, new SelectListItem { Value = "0", Text = "Select" });
        //                model.ddlMonth = lstMonth;
        //            }

        //            model.Month = postModel.Month;
        //            model.MonthName = dbContext.MASTER_MONTH.Where(m => m.MAST_MONTH_CODE == postModel.Month).Select(m => m.MAST_MONTH_FULL_NAME).FirstOrDefault();
        //            model.Year = postModel.Year;
        //            model.BalanceName = (postModel.Balance == "C" ? "Credit" : "Debit");
        //            List<SelectListItem> lstPIU = commomFuncObj.PopulateDPIUOfSRRDA(PMGSYSession.Current.AdminNdCode);
        //            lstPIU.Insert(0, new SelectListItem { Text = "All PIU", Value = "0" });
        //            model.ddlDPIU = lstPIU;

        //            if (model.Month == 1)
        //            {
        //                model.PreviousMonthName = "December";
        //            }
        //            else
        //            {
        //                model.PreviousMonthName = dbContext.MASTER_MONTH.Where(m => m.MAST_MONTH_CODE == (postModel.Month - 1)).Select(m => m.MAST_MONTH_FULL_NAME).FirstOrDefault(); ;
        //            }
        //            return View("RunningAccount", model);
        //        }
        //        return View("RunningAccount", model);
        //    }
        //    catch (Exception)
        //    {
        //        return PartialView();
        //    }
        //}

        public ActionResult RunningAccountReport()
        {
            RunningAccountViewModel model = new RunningAccountViewModel();
            try
            {
                if (ModelState.IsValid)
                {
                    model.Balance = Request.Params["Balance"];
                    model.Month = Convert.ToInt32(Request.Params["Month"]);
                    model.Year = Convert.ToInt32(Request.Params["Year"]);

                    if (PMGSYSession.Current.LevelId == 5)
                    {
                        model.ReportType = "D";
                    }
                    else
                    {
                        model.ReportType = Request.Params["ReportType"];
                    }
                    if (PMGSYSession.Current.LevelId == 4)
                    {
                        model.SRRDANDCode = PMGSY.Extensions.PMGSYSession.Current.AdminNdCode;
                        if (Request.Params["ReportType"] == "S")
                        {
                            model.AdminCode = PMGSYSession.Current.AdminNdCode;
                        }
                        else if (Request.Params["ReportType"] == "D")
                        {
                            model.AdminCode = Convert.ToInt32(Request.Params["DPIU"]);
                        }
                    }
                    else if (PMGSYSession.Current.LevelId == 5)
                    {
                        model.AdminCode = PMGSYSession.Current.AdminNdCode;
                        model.SRRDANDCode = PMGSY.Extensions.PMGSYSession.Current.ParentNDCode.Value;
                    }


                    if (PMGSYSession.Current.LevelId != 5 && model.AdminCode == 0 && Request.Params["ReportType"] == "D")
                    {

                        if (model.Balance == "C")
                        {
                            model.SSRSReport = "AC";
                        }
                        else
                        {
                            model.SSRSReport = "AD";
                        }

                    }
                    else
                    {
                        if (model.Balance == "C")
                        {
                            model.SSRSReport = "SC";
                        }
                        else
                        {
                            model.SSRSReport = "SD";
                        }
                    }

                }

                else
                {
                    return null;
                }
                return View(model);
            }

            catch (Exception)
            {
                return null;
            }



        }

        public SelectList PopulateYears(int AdminNdCode, int LevelId)
        {

            try
            {
                PMGSYEntities dbContext = new PMGSYEntities();
                int? entryYear = 0;
                Int16? entryMonth = 0;
                var lstClosedMonths = (from item in dbContext.ACC_RPT_MONTHWISE_SUMMARY
                                       where item.ADMIN_ND_CODE == AdminNdCode &&
                                       item.FUND_TYPE == PMGSYSession.Current.FundType
                                       && item.LVL_ID == LevelId //added by abhishek kamble 14-feb-2014
                                       select new
                                       {
                                           MONTH = item.ACC_MONTH,
                                           YEAR = item.ACC_YEAR,
                                           ID = (item.ACC_MONTH + item.ACC_YEAR * 12)
                                       }).Distinct().ToList();

                if (lstClosedMonths != null)
                {
                    entryYear = lstClosedMonths.Max(m => (int?)m.YEAR);

                    if (entryYear != null)
                    {
                        entryMonth = lstClosedMonths.Where(m => m.YEAR == entryYear).Max(s => (short?)s.MONTH);

                        if (entryMonth == 12)
                        {
                            entryYear = entryYear + 1;
                        }

                    }
                }

                List<MASTER_YEAR> lstYears = new List<MASTER_YEAR>();
                if (entryYear != 0 && entryYear != null)
                {
                    lstYears = dbContext.MASTER_YEAR.Where(y => y.MAST_YEAR_CODE == entryYear).OrderByDescending(y => y.MAST_YEAR_CODE).ToList<MASTER_YEAR>();
                }
                else
                {
                    short? OBYear = dbContext.ACC_BILL_MASTER.Where(m => m.BILL_TYPE == "O" && m.LVL_ID == LevelId && m.FUND_TYPE == PMGSYSession.Current.FundType && m.ADMIN_ND_CODE == AdminNdCode).Select(s => s.BILL_YEAR).FirstOrDefault();

                    if (OBYear != null)
                    {
                        lstYears = dbContext.MASTER_YEAR.Where(y => y.MAST_YEAR_CODE == OBYear).OrderByDescending(y => y.MAST_YEAR_CODE).ToList<MASTER_YEAR>();
                    }
                    else
                    {
                        lstYears.Insert(0, new MASTER_YEAR() { MAST_YEAR_CODE = 0, MAST_YEAR_TEXT = "Select Year " });
                    }


                }
                return new SelectList(lstYears, "MAST_YEAR_CODE", "MAST_YEAR_TEXT", DateTime.Now.Year);
            }
            catch (Exception ex)
            {
                return null;
            }
            finally
            {
                PMGSYEntities dbContext = new PMGSYEntities();
                if (dbContext != null)
                {
                    dbContext.Dispose();
                }
            }
        }

        public ActionResult PopulateRunningMonthsByYear(string id)
        {
            CommonFunctions commomFuncObj = new CommonFunctions();
            try
            {
                string[] Parameters = id.Split('$');
                int year = Convert.ToInt32(Parameters[0]);

                if (PMGSYSession.Current.LevelId == 5)
                {
                    return Json(commomFuncObj.PopulateRunningMonthsByYear(year, PMGSYSession.Current.AdminNdCode, PMGSYSession.Current.LevelId));//Parameter Modified by Abhishek 14-feb-2014
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

        public ActionResult PopulateRunningAccYear(string id)
        {
            try
            {

                string[] Parameters = id.Split('$');

                int AdminNdCode = 0;
                int LevelId = 0;

                if (Parameters[0] == "S")
                {
                    AdminNdCode = PMGSYSession.Current.AdminNdCode;
                    LevelId = PMGSYSession.Current.LevelId;
                }
                else
                {

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
                return Json(PopulateYears(AdminNdCode, LevelId));


            }
            catch (Exception)
            {
                return null;
            }
        }




        #endregion Running Account

        #region Fund Transfer
        public ActionResult FundTransferLayout()
        {

            string ScreenName = "FundTransfer";
            CommanViewFunction(ScreenName);

            return View();
        }
        private void CommanViewFunction(string Name)
        {
            CommonFunctions objCommFunc = new CommonFunctions();

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
            else
            {
                ViewBag.Year = objCommFunc.PopulateYears(year);
            }

            ViewBag.Month = objCommFunc.PopulateMonths(month);
            if (Name != "BankAuthrization")
            {
                ViewBag.FundSelection = PopulateFund();
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

            lstDPIU.Insert(0, new SelectListItem { Text = "All DPIU", Value = "0" });
            ViewBag.DPIU = lstDPIU;

            ViewBag.SelectedMonth = month;
            ViewBag.SelectedYear = year;

        }

        public ActionResult FundTransferReport(FundTransferViewModel objFundTransfer)
        {
            PMGSY.Models.Report.ReportFilter objParam = new PMGSY.Models.Report.ReportFilter();
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

                objFundTransfer.StateCode = objParam.AdminNdCode.ToString();
                objFundTransfer.DPIUCode = objParam.LowerAdminNdCode.ToString();

                ViewBag.HeadName = objFundTransfer.HeadName;

                //if (PMGSYSession.Current.LevelId == 5)
                //{
                //    PMGSY.DAL.Reports.IReportDAL objReportDAL = new PMGSY.DAL.Reports.ReportDAL();
                //    ViewBag.StateName = objReportDAL.GetDepartmentName(objParam.AdminNdCode);
                //    ViewBag.DPIUName = PMGSYSession.Current.DepartmentName;
                //}
                //else
                //{

                //}

                //objFundTransfer = GetFundTransferDetails(objParam);

                return View(objFundTransfer);
            }
            else
            {
                return Json(new { success = false, message = new CommonFunctions().FormatErrorMessage(ModelState) });
            }

        }


        public List<SelectListItem> PopulateFund()
        {
            PMGSY.Models.PMGSYEntities dbContext = new PMGSY.Models.PMGSYEntities();
            List<SelectListItem> lstFund = new List<SelectListItem>();
            try
            {


                var lstFundType = dbContext.USP_ACC_RPT_Get_HEAD_for_Bank_Authorisation(PMGSYSession.Current.FundType).ToList();

                foreach (var item in lstFundType)
                {
                    lstFund.Add(new SelectListItem { Value = item.HEAD_ID.ToString(), Text = item.NAME.ToString().Trim() });
                }

                lstFund.Insert(0, new SelectListItem { Value = "0", Text = "Select Fund" });
                return lstFund;
            }
            catch (Exception ex)
            {

                return lstFund;

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

        #region Abstract Fund Transfer
        // Added By Rohit A. Jadhav
        public ActionResult AbstractFundLayout()
        {
            string ScreenName = "AbstractFund";
            CommanViewFunction(ScreenName);
            return View();
        }


        public ActionResult AbstractFundReport(AbstractFundTransferredViewModel objAbstractFund)
        {
            if (ModelState.IsValid)
            {
                return View("AbstractFundReport", objAbstractFund);
            }
            else
            {
                return Json(new { success = false, message = new CommonFunctions().FormatErrorMessage(ModelState) });
            }
        }

        #endregion Abstract Fund Transfer

        #region Bank Authorization
        public ActionResult BankAuthrizationLayout()
        {
            string ScreenName = "BankAuthrization";
            CommanViewFunction(ScreenName);
            return View();
        }
        public ActionResult BankAuthrizationReport(BankAuthrizationViewModel objBankAuthrization)
        {
            PMGSY.Models.Report.ReportFilter objParam = new PMGSY.Models.Report.ReportFilter();


            ViewBag.Month = objBankAuthrization.MonthName;
            ViewBag.Year = objBankAuthrization.YearName;

            if (PMGSYSession.Current.LevelId == 5)
            {
                objParam.AdminNdCode = Convert.ToInt32(PMGSYSession.Current.ParentNDCode);
                objParam.LowerAdminNdCode = PMGSYSession.Current.AdminNdCode;
            }
            else
            {
                objParam.AdminNdCode = Convert.ToInt32(objBankAuthrization.State);
                objParam.LowerAdminNdCode = Convert.ToInt32(objBankAuthrization.DPIU);
            }
            objParam.Month = Convert.ToInt16(objBankAuthrization.Month);
            objParam.Year = Convert.ToInt16(objBankAuthrization.Year);
            objParam.State = objParam.AdminNdCode;
            objParam.Dpiu = objParam.LowerAdminNdCode;

            return View(objBankAuthrization);
        }

        #endregion Bank Authorization

        #region Abstract Bank Authorization

        public ActionResult AbstractBankAuthorizationLayout()
        {
            string name = "AbstractBankAuthorization";
            CommanViewFunction(name);
            return View();
        }

        public ActionResult AbstractBankAuthReport(AbstractBankAuthViewModel abstractBankAuthViewModel)
        {
            return View("AbstractBankAuthReport", abstractBankAuthViewModel);
            //return PartialView("AbstractBankAuthReport", objAccountBAL.AbstractBankAuthDetails(abstractBankAuthViewModel));
        }


        #endregion Abstract Bank Authorization

        #region Reconciliation
        // Added By Rohit A Jadhav. 11 June 2014
        public ActionResult ReconciliationLayout()
        {
            //PMGSY.Common.FetchCookieData fetchCookieData = new PMGSY.Common.FetchCookieData();
            ReconciliationModel recon = new ReconciliationModel();
            CommonFunctions commonFunctions = new CommonFunctions();


            if (PMGSYSession.Current.LevelId == 5)
            {
                recon.StateCode = Convert.ToInt32(PMGSYSession.Current.ParentNDCode);
            }
            else
            {
                recon.StateCode = PMGSYSession.Current.AdminNdCode;
            }
            recon.StateName = PMGSYSession.Current.StateCode == 0 ? "0" : PMGSYSession.Current.StateName;

            recon.Year = DateTime.Now.Year;
            recon.listYear = commonFunctions.PopulateYears(DateTime.Now.Year);

            recon.Month = DateTime.Now.Month;
            recon.listMonth = commonFunctions.PopulateMonths(DateTime.Now.Month);

            // Added by Srishti on 18-05-2023
            //List<SelectListItem> items = new List<SelectListItem>();
            //items.Add(new SelectListItem() { Text = "SNA", Value = "S" });
            //items.Add(new SelectListItem() { Text = "Holding", Value = "H" });
            //items.Add(new SelectListItem() { Text = "Security Deposit Account (SDA)", Value = "D" });
            //items.Insert(0, (new SelectListItem { Text = "Select Account Type", Value = "A", Selected = true }));
            //recon.lstBankAccType = new List<SelectListItem>();
            //recon.lstBankAccType = items;


            //populate SRRDA

            if (PMGSYSession.Current.LevelId == 6)//level MORD
            {
                recon.listStates = commonFunctions.PopulateNodalAgencies();
            }
            else if (PMGSYSession.Current.LevelId == 4)//level SRRDA
            {
                //recon.listStates = commonFunctions.PopulateNodalAgencies(PMGSYSession.Current.StateCode);
                //recon.StateCode = PMGSYSession.Current.AdminNdCode;
                List<SelectListItem> lst = commonFunctions.PopulateNodalAgencies(PMGSYSession.Current.StateCode);
                recon.listStates = lst.Where(m => m.Value == PMGSYSession.Current.AdminNdCode.ToString()).ToList();
                recon.StateCode = PMGSYSession.Current.AdminNdCode;
            }
            else if (PMGSYSession.Current.LevelId == 5)//level SRRDA
            {
                //recon.listStates = commonFunctions.PopulateNodalAgencies(PMGSYSession.Current.StateCode);
                //recon.StateCode = PMGSYSession.Current.AdminNdCode;
                List<SelectListItem> lst = commonFunctions.PopulateNodalAgencies(Convert.ToInt32(PMGSYSession.Current.StateCode));
                recon.listStates = lst.Where(m => m.Value == Convert.ToInt32(PMGSYSession.Current.ParentNDCode).ToString()).ToList();
                recon.StateCode = Convert.ToInt32(PMGSYSession.Current.ParentNDCode);
            }

            //recon.listStates = commonFunctions.PopulateStates(false);

            //List<SelectListItem> lst = commonFunctions.PopulateFundType();
            //lst.RemoveAt(3);
            //recon.listFundType = lst;

            return View(recon);

        }
        public ActionResult ReconciliationReport(ReconciliationModel recon)
        {
            //PMGSY.Common.FetchCookieData fetchCookieData = new PMGSY.Common.FetchCookieData();

            try
            {

                if (ModelState.IsValid)
                {
                    if (PMGSYSession.Current.LevelId == 4)
                    {
                        recon.StateCode = PMGSYSession.Current.AdminNdCode;
                    }
                    return View(recon);
                }
                else
                {
                    return Json(new { success = false }, JsonRequestBehavior.AllowGet);
                }

            }
            catch
            {
                return View(recon);
            }


        }


        #endregion Reconciliation

        #endregion

        #region Register Of Work


        [Audit]
        public ActionResult RegisterOfWorksLayout()
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            CommonFunctions commomFuncObj = new CommonFunctions();

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
                else if (objparams.LVL_ID == 6)
                {
                    List<SelectListItem> lstSRRDA = commomFuncObj.PopulateNodalAgencies();
                    ViewBag.SRRDA = lstSRRDA;
                    registerOfWorksModel.DEPARTMENT_LIST = commomFuncObj.PopulateDPIU(objparams);
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
            PMGSYEntities dbContext = new PMGSYEntities();
            CommonFunctions commomFuncObj = new CommonFunctions();

            try
            {
                TransactionParams objparams = new TransactionParams();
                objparams.MAST_CONT_ID = Convert.ToInt32(id1.Trim());
                objparams.ADMIN_ND_CODE = Convert.ToInt32(id2.Trim());
                //objparams.MAST_CONT_ID = Convert.ToInt16(id1.Trim());
                //objparams.ADMIN_ND_CODE = Convert.ToInt16(id2.Trim());
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
            PMGSYEntities dbContext = new PMGSYEntities();
            CommonFunctions commomFuncObj = new CommonFunctions();
            try
            {
                TransactionParams objparams = new TransactionParams();
                objparams.ADMIN_ND_CODE = Convert.ToInt16(id.Trim());
                objparams.DISTRICT_CODE = Convert.ToInt32(dbContext.ADMIN_DEPARTMENT.Where(c => c.ADMIN_ND_CODE == objparams.ADMIN_ND_CODE).Select(c => c.MAST_DISTRICT_CODE).FirstOrDefault());
                List<SelectListItem> lstContractor = commomFuncObj.PopulateContractorSupplier(objparams);
                lstContractor.RemoveAt(0);
                lstContractor.Insert(0, new SelectListItem { Text = "Select Contractor", Value = "0" });
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
        [HttpPost]
        public ActionResult RegisterOfWorksReport(string id)
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

                registerOfWorksModel = RegisterOfWorksDetails(objparams);

                if (PMGSYSession.Current.LevelId != 5)
                {
                    registerOfWorksModel.StateDepartment = urlSplitParams[3];
                }

                registerOfWorksModel.ContractorName = Request.Params["ContratorName"];

                return View(registerOfWorksModel);
            }
            catch (Exception)
            {
                return Json(new { Success = false, ErrorMessage = "An Error Occurred While Processing Your Request." });
            }
        }

        public RegisterOfWorksModel RegisterOfWorksDetails(TransactionParams objparams)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            RegisterOfWorksModel registerOfWorksModel = new RegisterOfWorksModel();
            try
            {
                //Header Info
                SP_ACC_RPT_PF_WORK_REGISTER_HEADER_INFORMATION_Result result = dbContext.SP_ACC_RPT_PF_WORK_REGISTER_HEADER_INFORMATION(objparams.FUND_TYPE,
                                                                                        objparams.ADMIN_ND_CODE, objparams.MAST_CONT_ID, objparams.AGREEMENT_CODE).FirstOrDefault();

                if (result != null)
                {
                    if (PMGSYSession.Current.LevelId == 5)
                    {
                        registerOfWorksModel.StateDepartment = dbContext.ADMIN_DEPARTMENT.Where(m => m.ADMIN_ND_CODE == PMGSYSession.Current.ParentNDCode).Select(s => s.ADMIN_ND_NAME).FirstOrDefault();
                    }
                    registerOfWorksModel.DistrictDepartment = result.Piu_Name;
                    registerOfWorksModel.AGREEMENT_DATE = result.Agreement_Date;
                    registerOfWorksModel.AGREEMENT_AMOUNT = result.Amount;
                    registerOfWorksModel.AGREEMENT_NUMBER = result.Agreement_number;
                }
                registerOfWorksModel.ADMIN_ND_CODE = objparams.ADMIN_ND_CODE;
                registerOfWorksModel.MAST_CON_ID = objparams.MAST_CONT_ID;
                registerOfWorksModel.TEND_AGREEMENT_CODE = objparams.AGREEMENT_CODE;
                return registerOfWorksModel;
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

        #endregion Register Of Work

        #region Imprest Register
        [Audit]
        public ActionResult ImprestRegisterLayout()
        {
            ImprestSettlementViewModel model = new ImprestSettlementViewModel();
            CommonFunctions commomFuncObj = new CommonFunctions();
            try
            {
                model.lstFinancialYears = new SelectList(commomFuncObj.PopulateFinancialYear(true, false).ToList(), "Value", "Text").ToList();
                if (PMGSYSession.Current.LevelId == 4)
                {
                    model.SrrdaAdminCode = PMGSYSession.Current.AdminNdCode;
                    model.lstSrrda = commomFuncObj.PopulateNodalAgencies(PMGSYSession.Current.StateCode);
                    model.lstDpiu = commomFuncObj.PopulateDPIUOfSRRDA(PMGSYSession.Current.AdminNdCode);
                }
                else
                {
                    model.lstSrrda = commomFuncObj.PopulateNodalAgencies();
                    model.lstDpiu.Insert(0, new SelectListItem { Value = "0", Text = "Select DPIU" });
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
        public ActionResult ImprestRegisterReport(ImprestSettlementViewModel _postModel)
        {
            PMGSYEntities db = new PMGSYEntities();
            try
            {
                ImprestSettlementViewModel model = new ImprestSettlementViewModel();
                model.SrrdaAdminCode = _postModel.SrrdaAdminCode;
                model.DpiuAdminCode = PMGSYSession.Current.LevelId == 5 ? PMGSYSession.Current.AdminNdCode : _postModel.DpiuAdminCode;

                if (ModelState.IsValid)
                {
                    if (PMGSYSession.Current.LevelId == 5)
                    {
                        model.ReportLevel = "D";
                        model.NodalAgency = db.ADMIN_DEPARTMENT.Where(m => m.ADMIN_ND_CODE == PMGSYSession.Current.ParentNDCode).Select(m => m.ADMIN_ND_NAME).FirstOrDefault();
                        model.DPIUName = PMGSYSession.Current.DepartmentName;
                        model.FinancialYear = _postModel.FinancialYear;
                    }
                    else
                    {
                        if (_postModel.ReportLevel == "D")
                        {
                            model.ReportLevel = "D";
                            model.NodalAgency = _postModel.NodalAgency;
                            model.DPIUName = _postModel.DPIUName;
                            model.FinancialYear = _postModel.FinancialYear;
                        }
                        else
                        {
                            model.ReportLevel = "S";
                            model.NodalAgency = _postModel.NodalAgency;
                            model.DPIUName = "";
                            model.FinancialYear = _postModel.FinancialYear;
                        }
                    }
                }
                return PartialView(model);
            }
            catch (Exception)
            {
                return PartialView();
            }
            finally
            {
                db.Dispose();
            }
        }

        #endregion Imprest Register

        #region Utilization Certificate

        public ActionResult UtilizationCertificatesLayout()
        {
            CommonFunctions commonFunctions = new CommonFunctions();
            UtilizationCertificate ucDetails = new UtilizationCertificate();

            ucDetails.lstStates = commonFunctions.PopulateStates(false);
            ucDetails.lstStates.Insert(0, (new SelectListItem { Text = "Select State", Value = "0", Selected = true }));
            ucDetails.lstAgency = PopulateAgency(PMGSYSession.Current.StateCode);

            List<SelectListItem> lstYear = new List<SelectListItem>();
            // below code added by Saurabh Jojare on  30-12-2021
            ucDetails.lstYear = commonFunctions.PopulateYears();


        //    ucDetails.lstYear.Insert(0, (new SelectListItem { Text = "Select Year", Value = "0", Selected = true }));
         
            // lstYear.Add(new SelectListItem { Text = "Select", Value = "0" });

            //if (PMGSYSession.Current.LevelId == 4)
            //{
            //    PMGSYEntities dbContext = new PMGSYEntities();

                //var details = dbContext.UDF_ACC_GET_OB_CLOSING_DETAILS(PMGSYSession.Current.StateCode, 0, PMGSYSession.Current.AdminNdCode, PMGSYSession.Current.FundType).FirstOrDefault();

                //if (details != null)
                //{
                //    for (int startYear = details.OB_YEAR.Value; startYear <= details.CLOSE_YEAR; startYear++)
                //    {
                //        lstYear.Add(new SelectListItem { Value = startYear.ToString(), Text = startYear + "-" + (startYear + 1) });
                //    }
                //}
                // Above Code Commented By saurabh Jojare on 30-12-2021.

          //  }

           // ucDetails.lstYear = lstYear;
            return View(ucDetails);

        }
        [HttpPost]
        public JsonResult PopulateAgency(string id)
        {
            int stateCode = Convert.ToInt32(id);
            return Json(PopulateAgency(stateCode), JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult UCReport(UtilizationCertificate modelUC)
        {

            return View(modelUC);
        }



        public List<SelectListItem> PopulateAgency(int stateCode)
        {
            PMGSY.Models.PMGSYEntities dbcontext = new PMGSY.Models.PMGSYEntities();
            try
            {
                List<SelectListItem> lstAgency = new List<SelectListItem>();

                var lstData = (from ma in dbcontext.MASTER_AGENCY
                               join md in dbcontext.ADMIN_DEPARTMENT on ma.MAST_AGENCY_CODE equals md.MAST_AGENCY_CODE
                               where
                               md.MAST_ND_TYPE == "S"
                               &&
                               md.MAST_STATE_CODE == stateCode
                               //(stateCode == 0 ? 1 : md.MAST_STATE_CODE) == (stateCode == 0 ? 1 : stateCode)

                               select new
                               {
                                   AgencyName = md.ADMIN_ND_NAME,//ma.MAST_AGENCY_NAME,
                                   AgencyCode = md.ADMIN_ND_CODE,
                                   Selected = (ma.MAST_AGENCY_TYPE == "G" ? true : false)
                               }).OrderBy(c => c.AgencyName).ToList().Distinct();


                foreach (var item in lstData)
                {
                    lstAgency.Add(new SelectListItem { Text = item.AgencyName, Value = item.AgencyCode.ToString() });
                }
                lstAgency.Insert(0, new SelectListItem { Text = "Select Agency", Value = "0" });
                return lstAgency;
            }
            catch (Exception ex)
            {
                return null;
            }
            finally
            {
                if (dbcontext != null)
                {
                    dbcontext.Dispose();
                }
            }
        }


        //Start Added By Rohit 17 Feb 2016
        public ActionResult PopulateFinancialYearAccount(int state, int AdminNDCode)
        {
            PMGSY.Models.PMGSYEntities dbContext = new PMGSY.Models.PMGSYEntities();
            try
            {
                dbContext = new PMGSYEntities();

                List<SelectListItem> lstYears = new List<SelectListItem>();

                lstYears.Add(new SelectListItem { Value = "0", Text = "Select" });

                var details = dbContext.UDF_ACC_GET_OB_CLOSING_DETAILS(state, 0, AdminNDCode, PMGSYSession.Current.FundType).FirstOrDefault();

                if (details != null)
                {
                    for (int startYear = details.OB_YEAR.Value; startYear <= details.CLOSE_YEAR; startYear++)
                    {
                        lstYears.Add(new SelectListItem { Value = startYear.ToString(), Text = startYear + "-" + (startYear + 1) });
                    }
                }

                //  return new SelectList(lstYears, "Value", "Text");
                return Json(lstYears);
            }
            catch
            {
                return null;
            }
            finally
            {
                dbContext.Dispose();
            }
        }


        #endregion

        #region  Month Revoking Details 28 Aug 2017 Added By Rohit Jadhav



        public ActionResult MonthRevokeLayout()
        {
            CommonFunctions commomFuncObj = new CommonFunctions();
            MonthRevokeModel filterModel = new MonthRevokeModel
            {
                Month = (short)DateTime.Now.Month,
                ListMonth = commomFuncObj.PopulateMonths(DateTime.Now.Month),
                ListYear = commomFuncObj.PopulateYears(DateTime.Now.Year),
                Year = (short)DateTime.Now.Year,


                ToMonth = (short)DateTime.Now.Month,
                ListToMonth = commomFuncObj.PopulateMonths(DateTime.Now.Month),

                ListToYear = commomFuncObj.PopulateYears(DateTime.Now.Year),
                ToYear = (short)DateTime.Now.Year,

                LevelId = PMGSYSession.Current.LevelId,
                Piu = PMGSYSession.Current.AdminNdCode,
            };
            if (PMGSYSession.Current.LevelId == 4)
            {
                filterModel.ListPiu = commomFuncObj.PopulateAllDPIUOfSRRDA(PMGSYSession.Current.AdminNdCode);
                filterModel.ListPiu.Insert(0, (new SelectListItem { Text = "All DPIU", Value = "0", Selected = true }));
                List<SelectListItem> lst = commomFuncObj.PopulateNodalAgencies(PMGSYSession.Current.StateCode);
                filterModel.ListAgency = lst.Where(m => m.Value == PMGSYSession.Current.AdminNdCode.ToString()).ToList();
            }
            else if (PMGSYSession.Current.LevelId == 6)
            {// accmord user
                filterModel.ListPiu = new List<SelectListItem>();
                filterModel.ListPiu.Insert(0, (new SelectListItem { Text = "All DPIU", Value = "0", Selected = true }));
                filterModel.ListAgency = commomFuncObj.PopulateAllNodalAgencies();
            }
            return View(filterModel);
        }



        public ActionResult MonthRevokeReport(short month, short year, int? ndcode, int rlevel, int allpiu, int? duration, short ToMonth, short ToYear)
        {
            CommonFunctions commomFuncObj = new CommonFunctions();
            MonthRevokeModel sheduleReportModel = new MonthRevokeModel();
            PMGSY.Models.PMGSYEntities dbContext = new PMGSY.Models.PMGSYEntities();
            try
            {
                var reportMaster = commomFuncObj.GetReportId(rlevel, PMGSYSession.Current.FundType, 152); //OMMAS_DEV-152 - Trainning -191

                //Modified By Abhishek kamble 21-Apr-2014 start
                PMGSY.Models.ACC_RPT_REPORT_PROPERTY[] reportHeader = new PMGSY.Models.ACC_RPT_REPORT_PROPERTY[4];

                if (reportMaster != null)
                {
                    reportHeader = reportMaster.ACC_RPT_REPORT_PROPERTY.ToArray<PMGSY.Models.ACC_RPT_REPORT_PROPERTY>();
                    sheduleReportModel.FormNumber = reportHeader[0].PROP_VALUE;
                    sheduleReportModel.ReportHeader = reportHeader[3].PROP_VALUE;
                    sheduleReportModel.Refference = reportHeader[4].PROP_VALUE;
                }

                string flag = string.Empty;

                if (duration == 1)//1- annually 2- Monthly
                {
                    //  month = 0;
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

                sheduleReportModel.UserID = PMGSYSession.Current.UserId;
                sheduleReportModel.FundTye = PMGSYSession.Current.FundType;
                if (duration == 1)
                {//1- periodically  2- Monthly
                    sheduleReportModel.DurationFlag = "P";
                }
                else
                {
                    sheduleReportModel.DurationFlag = "M";
                }



                sheduleReportModel.Agency = PMGSYSession.Current.LevelId == 5 ? PMGSYSession.Current.ParentNDCode.Value : (ndcode == null ? 0 : ndcode.Value);
                sheduleReportModel.Month = month;
                sheduleReportModel.Year = year;
                sheduleReportModel.ToMonth = ToMonth;
                sheduleReportModel.ToYear = ToYear;


                sheduleReportModel.Piu = PMGSYSession.Current.LevelId == 5 ? PMGSYSession.Current.AdminNdCode : allpiu;
                sheduleReportModel.FundType = PMGSYSession.Current.FundType;
                sheduleReportModel.SRRDA_DPIU = flag;

                return PartialView(sheduleReportModel);
            }
            catch (Exception Ex)
            {
                throw Ex;
            }
            finally
            {
                dbContext.Dispose();
            }
        }


        /// 
        public JsonResult GetAllDPIUOfSRRDA(int ndcode)
        {
            CommonFunctions commomFuncObj = new CommonFunctions();
            return Json(commomFuncObj.PopulateAllDPIUOfSRRDA(ndcode), JsonRequestBehavior.AllowGet);
        }


        #endregion

        #region Fund Position Report
        [HttpGet]
        public ActionResult FundPositionReportLayout()
        {
            try
            {
                List<SelectListItem> yearList = new List<SelectListItem>();
                CommonFunctions cf = new CommonFunctions();
                FundPostionModel model = new FundPostionModel();
                model.YearList = cf.PopulateFinancialYear(false).ToList();

                return View(model);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "AccountReports.FundPositionReportLayout()");
                return null;
            }
        }

        [HttpPost]
        public ActionResult FundPostionReport(FundPostionModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    return View(model);
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "AccountReports.FundPostionReport()");
                return null;
            }
        }
        #endregion

        #region Location wise FPP Details



        public ActionResult FPPLayout()
        {
            FPPModel stateAccountMonitoringModel = new FPPModel();

            CommonFunctions commonFunctions = new CommonFunctions();

            List<SelectListItem> lstStates = commonFunctions.PopulateStates(false).ToList();

            stateAccountMonitoringModel.lstStates = lstStates;
            //    stateAccountMonitoringModel.StateCode = PMGSYSession.Current.StateCode;//  fetchCookieData.StateCode;

            stateAccountMonitoringModel.lstAgency = PopulateAgencyByStateCode(PMGSYSession.Current.StateCode);

            stateAccountMonitoringModel.ListYear = commonFunctions.PopulateAllYears(DateTime.Now.Year);
            stateAccountMonitoringModel.lstFundingAgency = commonFunctions.PopulateFundingAgency(true);

            stateAccountMonitoringModel.lstPeriod = PopulatePeriod();

            return View(stateAccountMonitoringModel);
        }

        public List<SelectListItem> PopulatePeriod()
        {
            List<SelectListItem> ProposalType = new List<SelectListItem>();
            SelectListItem item = new SelectListItem();
            item.Text = "Greater Than 2";
            item.Value = "24";
            item.Selected = true;
            ProposalType.Add(item);


            item = new SelectListItem();
            item.Text = "Greater Than 1";
            item.Value = "12";
            ProposalType.Add(item);

            item = new SelectListItem();
            item.Text = "Greater Than 6";
            item.Value = "6";
            ProposalType.Add(item);

            return ProposalType;
        }

        //public JsonResult PopulateAgencyByStateCode(string id)
        //{
        //    if (!int.TryParse(id, out outParam))
        //    {
        //        return Json(false);
        //    }
        //    int stateCode = Convert.ToInt32(id);
        //    return Json(objAccountDAL.PopulateAgencyByStateCode(stateCode), JsonRequestBehavior.AllowGet);
        //}


        /// <summary>
        /// Render data in State Acc Monitoring Report
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult FPPReport(FPPModel stateAccMonitoringModel)
        {
            FPPModel Model = new FPPModel();
            //PMGSY.Common.FetchCookieData fetchCookieData = new PMGSY.Common.FetchCookieData();

            try
            {
                if (ModelState.IsValid)
                {
                    //string[] splitParams = id.Split('$');

                    //stateAccountMonitoringModel.StateCode = stateAccMonitoringModel.StateCode;
                    //stateAccountMonitoringModel.Agency = Convert.ToInt32(stateAccMonitoringModel.Agency);
                    //stateAccountMonitoringModel.FundType = stateAccMonitoringModel.FundType;


                    Model.StateCode = stateAccMonitoringModel.StateCode;
                    Model.Agency = stateAccMonitoringModel.Agency;
                    Model.Year = stateAccMonitoringModel.Year;
                    Model.FundingAgencyCode = stateAccMonitoringModel.FundingAgencyCode;
                    Model.PeriodCode = stateAccMonitoringModel.PeriodCode;


                    //stateAccountMonitoringModel.StateSrrdaPiu = "S";
                    //stateAccountMonitoringModel.levelId = 4;

                    //stateAccountMonitoringModel.DisplayAgencyName = splitParams[5];
                    //stateAccountMonitoringModel.DisplayStateName = fetchCookieData.StateName;


                    //stateAccountMonitoringModel.AgencyName = stateAccMonitoringModel.AgencyName;
                    return View(Model);
                }
                else
                {
                    return Json(new { success = false }, JsonRequestBehavior.AllowGet);
                    //return View("StateAccountMonitoringLayout",stateAccountMonitoringModel);                
                }

            }
            catch
            {
                return View(Model);
            }

        }

        public List<SelectListItem> PopulateAgencyByStateCode(int stateCode)
        {
            PMGSY.Models.PMGSYEntities dbcontext = new PMGSY.Models.PMGSYEntities();

            try
            {
                List<SelectListItem> lstAgency = new SelectList(dbcontext.USP_ACC_DISPLAY_AGENCIES_DETAILS(stateCode), "MAST_AGENCY_CODE", "ADMIN_ND_NAME").OrderBy(o => o.Text).ToList();
                lstAgency.Insert(0, new SelectListItem { Text = "All Agency", Value = "0" });
                return lstAgency;

            }
            catch (Exception ex)
            {
                return null;
            }
            finally
            {
                if (dbcontext != null)
                {
                    dbcontext.Dispose();
                }
            }
        }


        [HttpPost]
        public JsonResult PopulateAgencyUsingStateCode(string id)
        {
            PMGSY.Models.PMGSYEntities dbcontext = new PMGSY.Models.PMGSYEntities();
            int stateCode = Convert.ToInt32(id);
            List<SelectListItem> lstAgency = new SelectList(dbcontext.USP_ACC_DISPLAY_AGENCIES_DETAILS(stateCode), "MAST_AGENCY_CODE", "ADMIN_ND_NAME").OrderBy(o => o.Text).ToList();
            lstAgency.Insert(0, new SelectListItem { Text = "All Agency", Value = "0" });
            return Json(lstAgency, JsonRequestBehavior.AllowGet);
        }



        #endregion

        #region ANNEXURE

        public ActionResult AnnLayout()
        {
            ANNEXURE sc = new ANNEXURE();

            CommonFunctions commonFunctions = new CommonFunctions();

            sc.lstscheme = commonFunctions.PopulateScheme();
            sc.ListYear = commonFunctions.PopulateFinancialYear(true, true).ToList();



            return View(sc);
        }

        public ActionResult AnnReport(ANNEXURE sc)
        {
            ANNEXURE Model = new ANNEXURE();
            try
            {
                if (ModelState.IsValid)
                {
                    Model.Year = sc.Year;
                    Model.schemeCode = sc.schemeCode;

                    // 2 for SSRDA and 3 for STA
                    if (PMGSYSession.Current.RoleCode == 2 || PMGSYSession.Current.RoleCode == 3)
                    {
                        Model.Districtwise = true;
                        Model.sessionStatecode = PMGSYSession.Current.StateCode;

                    }
                    else
                    {
                        Model.Districtwise = false;
                        // MORD Level
                    }

                    return View(Model);
                }
                else
                {
                    return Json(new { success = false }, JsonRequestBehavior.AllowGet);
                }

            }
            catch
            {
                return View(Model);
            }
        }
        #endregion


        #region Road Works Completed

        public ActionResult RoadWorksCompletedLayout()
        {
            RoadWiseCompleted packageAgreement = new RoadWiseCompleted();
            CommonFunctions commonFunctions = new CommonFunctions();


            packageAgreement.lstscheme = commonFunctions.PopulateScheme();

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
            packageAgreement.YearList = commonFunctions.PopulateFinancialYear(true, true).ToList();
            packageAgreement.YearList.RemoveAt(0);
            packageAgreement.YearList.Insert(0, new SelectListItem { Text = "Select Year", Value = "0" });
            return View(packageAgreement);
        }

        /// <summary>
        /// Render data in Completed Roads Report
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult RoadWorksCompletedReport(RoadWiseCompleted packageAgreement)
        {

            try
            {
                if (ModelState.IsValid)
                {
                    packageAgreement.schemeCode = packageAgreement.schemeCode;

                    // 2 for SSRDA and 3 for STA
                    if (PMGSYSession.Current.RoleCode == 2 || PMGSYSession.Current.RoleCode == 3)
                    {
                        packageAgreement.Districtwise = true;
                        packageAgreement.sessionStatecode = PMGSYSession.Current.StateCode;

                    }
                    else
                    {
                        packageAgreement.Districtwise = false;
                        // MORD Level

                    }
                    packageAgreement.Mast_State_Code = packageAgreement.StateCode > 0 ? packageAgreement.StateCode : packageAgreement.Mast_State_Code;
                    packageAgreement.Mast_District_Code = packageAgreement.DistrictCode > 0 ? packageAgreement.DistrictCode : packageAgreement.Mast_District_Code;
                    packageAgreement.Mast_Block_Code = packageAgreement.BlockCode > 0 ? packageAgreement.BlockCode : packageAgreement.Mast_Block_Code;

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
        #endregion

        #region ANNEXURE F

        public ActionResult AnnexureF()
        {
            ANNEXURE sc = new ANNEXURE();
            CommonFunctions commonFunctions = new CommonFunctions();
            sc.lstscheme = commonFunctions.PopulateScheme();
            sc.ListYear = commonFunctions.PopulateFinancialYear(true, true).ToList();
            return View(sc);
        }


        public ActionResult AnnexureF_Report(ANNEXURE sc)
        {
            ANNEXURE Model = new ANNEXURE();
            try
            {
                if (ModelState.IsValid)
                {
                    Model.Year = sc.Year;
                    Model.schemeCode = sc.schemeCode;

                    // 2 for SSRDA and 3 for STA
                    if (PMGSYSession.Current.RoleCode == 2 || PMGSYSession.Current.RoleCode == 3)
                    {
                        Model.Districtwise = true;
                        Model.sessionStatecode = PMGSYSession.Current.StateCode;
                    }
                    else
                    {
                        Model.Districtwise = false;
                        // MORD Level
                    }

                    return View(Model);
                }
                else
                {
                    return Json(new { success = false }, JsonRequestBehavior.AllowGet);
                }

            }
            catch
            {
                return View(Model);
            }
        }
        #endregion

        #region Cash Payment Report
        //CashPaymentreportActionMethod added by Abhinav Pathak 
        // module type added by sachin
        [Audit]
        public ActionResult DisplayPaymentLayoutView()
        {
            if (PMGSYSession.Current.UserId == 0)
            {
                Response.Redirect("/Login/SessionExpire");
            }

            CommonFunctions objCommon = new CommonFunctions();
            CashPaymentViewModel accModel = new CashPaymentViewModel();
            TransactionParams objParam = new TransactionParams();

            try
            {
                
                if (PMGSYSession.Current.LevelId == 4 || PMGSYSession.Current.LevelId == 6)                
                {
                    //populate SRRDA
                    List<SelectListItem> lstSRRDA = new List<SelectListItem>();
                    lstSRRDA = objCommon.PopulateNodalAgencies();

                    ViewBag.SRRDA = lstSRRDA;

                    //Populate DPIU
                    List<SelectListItem> lstDPIU = new List<SelectListItem>();
                    lstDPIU = objCommon.PopulateDPIUOfSRRDA(accModel.SRRDA);
                    lstDPIU.Insert(0, new SelectListItem { Text = "All PIU", Value = "0" });
                    ViewBag.DPIU = lstDPIU;
                }
                //Added by abhishek kamble 3-oct-2013 end
                accModel.ModuleType = new List<SelectListItem> {     new SelectListItem{ Text = "Reat Module", Value ="R" , Selected = true }, 
                                                            new SelectListItem{ Text = "DBT Module", Value ="D" }};

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

                //accModel.SRRDA = PMGSYSession.Current.AdminNdCode;
                List<SelectListItem> lstPIU = objCommon.PopulateDPIU(objParam);
                lstPIU.RemoveAt(0);
                lstPIU.Insert(0, new SelectListItem { Text = "All PIU", Value = "0" });

                ViewBag.DPIU = lstPIU;


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
                
                // commented due to fund type selection provision by screen on 14-07-2022
                //accModel.Fundtype = PMGSYSession.Current.FundType;
                return View(accModel);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "AccountReports.AccountController.DisplayPaymentLayoutView()");
                return null;
            }
        }

        // CashPayment report post action method
        [HttpPost]
        [Audit]
        public ActionResult ShowCashPaymentReport(CashPaymentViewModel accModel)
        {
            //modified .......

            accModel.levelId = PMGSYSession.Current.LevelId;

            PMGSYEntities dbContext = new PMGSYEntities();
            CommonFunctions objComm = new CommonFunctions();
            try
            {
                if (ModelState.IsValid)
                {

                    int AdminNDCode = accModel.AdminNdCode;
                    int AllPIU = 0;

                    if (accModel.NodalAgency == "D")
                    {
                        AllPIU = accModel.DPIU == 0 ? 1 : 0;
                    }

                    if (accModel.NodalAgency == "D" && accModel.DPIU > 0)
                    {
                        accModel.levelId = 5;
                    }

                    //check srrda/DPIU
                    if (accModel.DPIU > 0)
                    {
                        AdminNDCode = accModel.DPIU;
                    }
                    else
                    {
                        AdminNDCode = PMGSYSession.Current.AdminNdCode;
                    }

                    DateTime? StartDate = null;
                    DateTime? EndDate = null;
                    if (accModel.StartDate != null && accModel.EndDate != null)
                    {
                        StartDate = objComm.GetStringToDateTime(accModel.StartDate);
                        EndDate = objComm.GetStringToDateTime(accModel.EndDate);
                    }
                                        
                    if (PMGSYSession.Current.LevelId == 6)                    
                    {
                        accModel.AdminNdCode = accModel.SRRDA;
                    }
                    else
                    {
                        accModel.AdminNdCode = PMGSYSession.Current.ParentNDCode.Value;
                    }
                                        
                    if (accModel.levelId == 4 || accModel.levelId == 6)                    
                    {
                        //accModel.DPIUBySRRDA = dbContext.ADMIN_DEPARTMENT.Where(m => m.ADMIN_ND_CODE == AdminNDCode).Select(m => m.ADMIN_ND_NAME).FirstOrDefault();
                        accModel.DPIUName = dbContext.ADMIN_DEPARTMENT.Where(m => m.ADMIN_ND_CODE == accModel.DPIU).Select(m => m.ADMIN_ND_NAME).FirstOrDefault();
                        accModel.NodalAgency = dbContext.ADMIN_DEPARTMENT.Where(m => m.ADMIN_ND_CODE == (dbContext.ADMIN_DEPARTMENT.Where(p => p.ADMIN_ND_CODE == accModel.AdminNdCode).Select(p => p.MAST_PARENT_ND_CODE).FirstOrDefault())).Select(m => m.ADMIN_ND_NAME).FirstOrDefault();

                    }
                    else
                    {
                        accModel.NodalAgency = dbContext.ADMIN_DEPARTMENT.Where(m => m.ADMIN_ND_CODE == AdminNDCode).Select(m => m.ADMIN_ND_NAME).FirstOrDefault();
                        accModel.DPIUName = null;
                        accModel.DPIU = accModel.DPIU == 0 ? PMGSYSession.Current.AdminNdCode : accModel.DPIU;
                    }

                    if (accModel.DPIU > 0)
                    {
                        accModel.NodalAgency = accModel.DPIUName;
                    }

                    //else if (accModel.DPIU == 0)
                    //{
                    //    accModel.NodalAgency = "All DPIU";
                    //    accModel.DPIUName = "All DPIU";
                    //}

                    //accModel.lstAccountBillDetails = dbContext.SP_ACC_RPT_DISPALY_Bill_DETAILS(PMGSYSession.Current.FundType, AdminNDCode, accModel.Month, accModel.Year, StartDate, EndDate, accModel.BillType, accModel.rType, AllPIU).ToList<SP_ACC_RPT_DISPALY_Bill_DETAILS_Result>();
                    //accModel.TotalRecords = accModel.lstAccountBillDetails.Count();
                    //return accModel;

                    if (accModel.StartDate == null || accModel.StartDate == string.Empty)
                    {
                        //accModel.StartDate = "-";
                        accModel.StartDate = " ";
                    }
                    else
                    {
                        accModel.StartDate = Convert.ToDateTime(accModel.StartDate).ToString("MM/dd/yyyy");
                    }
                    if (accModel.EndDate == null || accModel.EndDate == string.Empty)
                    {
                        //accModel.EndDate = "-";
                        accModel.EndDate = " ";
                    }
                    else
                    {
                        accModel.EndDate = Convert.ToDateTime(accModel.EndDate).ToString("MM/dd/yyyy");
                    }

                    if (accModel.DPIUName == null)
                    {
                        accModel.DPIUName = "-";
                    }
                    if (accModel.Month > 0)
                    {
                        string MonthName = objComm.getMonthText(accModel.Month);
                        accModel.YearName = MonthName + "-" + accModel.Year;
                    }
                    else
                    {
                        accModel.YearName = accModel.Year + "-" + Convert.ToString(accModel.Year + 1);
                    }

                    if (PMGSYSession.Current.StateName != null && PMGSYSession.Current.StateName != string.Empty)
                    {
                        accModel.StateName = PMGSYSession.Current.StateName;
                    }
                    else
                    {
                        PMGSYSession.Current.StateName = "-";
                        accModel.StateName = "-";
                    }

                    accModel.Selection = accModel.rType == "M" ? "Monthly" : accModel.rType == "Y" ? "Yearly" : accModel.rType == "P" ? "Periodic" : string.Empty;
                    //accModel.sDate = accModel.StartDate;
                    //accModel.eDate = accModel.EndDate;
                    accModel.isAllPiu = AllPIU;
                    return PartialView(accModel);
                }
                return PartialView(accModel);
            }
            catch (Exception ex)
            {
                //Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                ErrorLog.LogError(ex, "AccountReports.Account.ShowCashPaymentReport()");
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
        
        
        #endregion

        #region PFMS MIS PAYMENT SUMMARY
        ///SUMMARY 
        ///This method returns the view with required data to populate dropdown at UI
        ///Method return view to generate PFMS payment report
        //added by abhinav pathak 0n 04-dec-2018

        [Audit]
        [HttpGet]
        public ActionResult DisplayMISPayment(string id)
        {
            CommonFunctions objCommon = null;
            PfmsMisPaymentModel accModel = null;
            TransactionParams objParam = null;
            PMGSYEntities dbContext = null;
            try
            {

                objCommon = new CommonFunctions();
                accModel = new PfmsMisPaymentModel();
                objParam = new TransactionParams();
                dbContext = new PMGSYEntities();

                if (id != null)
                {
                    accModel.StartDate = id.Split('$')[0];
                    accModel.StartDate = accModel.StartDate.Replace('-', '/');
                    accModel.EndDate = id.Split('$')[1];
                    accModel.EndDate = accModel.EndDate.Replace('-', '/');
                    accModel.isRedirected = true;
                }

                if (PMGSYSession.Current.UserId == 0)
                {
                    Response.Redirect("/Login/SessionExpire");
                }

                if (PMGSYSession.Current.LevelId == 4 || PMGSYSession.Current.LevelId == 6)
                {
                    //populate SRRDA
                    List<SelectListItem> lstSRRDA = new List<SelectListItem>();
                    lstSRRDA = objCommon.PopulateNodalAgencies();
                    if (lstSRRDA != null)
                    {
                        lstSRRDA.Insert(0, new SelectListItem() { Selected = true, Text = "All States", Value = "0" });
                        ViewBag.SRRDA = lstSRRDA;
                    }

                    //Populate DPIU
                    List<SelectListItem> lstDPIU = new List<SelectListItem>();
                    lstDPIU = objCommon.PopulateDPIUOfSRRDA(accModel.SRRDA);
                    if (lstDPIU != null)
                    {
                        lstDPIU.Insert(0, new SelectListItem() { Text = "All PIU", Value = "0" });
                        ViewBag.DPIU = lstDPIU;
                    }
                }

                if (PMGSYSession.Current.LevelId == 5)
                {

                    List<SelectListItem> lstSRRDA = new List<SelectListItem>();

                    lstSRRDA.Insert(0, new SelectListItem() { Selected = true, Text = PMGSYSession.Current.StateName, Value = PMGSYSession.Current.StateCode.ToString() });
                    ViewBag.SRRDA = lstSRRDA;


                    List<SelectListItem> lstDPIU = new List<SelectListItem>();
                    accModel.DPIU = PMGSYSession.Current.AdminNdCode;
                    accModel.DPIUName = dbContext.ADMIN_DEPARTMENT.Where(m => m.ADMIN_ND_CODE == accModel.DPIU).Select(m => m.ADMIN_ND_NAME).FirstOrDefault();
                    lstDPIU.Insert(0, new SelectListItem() { Selected = true, Text = accModel.DPIUName, Value = PMGSYSession.Current.DistrictCode.ToString() });
                    ViewBag.DPIU = lstDPIU;
                }


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

                //accModel.SRRDA = PMGSYSession.Current.AdminNdCode;
                //List<SelectListItem> lstPIU = objCommon.PopulateDPIU(objParam);
                //lstPIU.RemoveAt(0);
                //lstPIU.Insert(0, new SelectListItem { Text = "All PIU", Value = "0" });

                //ViewBag.DPIU = lstPIU;


                accModel.MonthList = objCommon.PopulateMonths(objParam.MONTH);
                accModel.YearList = objCommon.PopulateYears(objParam.YEAR);

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

                return View(accModel);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "AccountReports.AccountController.DisplayMISPayment()");
                return null;
            }
        }

        ///SUMMARY
        ///Method recieves the post request for genrating PFMS payment Report
        ///To bind the incoming data with the required parameter to generate report

        //post action method for PFMS payment report
        //added by abhinav pathak 0n 04-dec-2018
        [HttpPost]
        [Audit]
        public ActionResult ShowMISPaymentReport(PfmsMisPaymentModel accModel)
        {

            accModel.FundType = PMGSYSession.Current.FundType;
            accModel.levelId = PMGSYSession.Current.LevelId;
            PMGSYEntities dbContext = null;
            CommonFunctions objComm = null;
            try
            {
                dbContext = new PMGSYEntities();
                objComm = new CommonFunctions();

                if (ModelState.IsValid)
                {

                    int AdminNDCode = accModel.AdminNdCode;
                    int AllPIU = 0;

                    if (accModel.NodalAgency == "D")
                    {
                        AllPIU = accModel.DPIU == 0 ? 1 : 0;
                    }

                    if (accModel.NodalAgency == "D" && accModel.DPIU > 0)
                    {
                        accModel.levelId = 5;
                    }

                    //check srrda/DPIU
                    if (accModel.DPIU > 0)
                    {
                        AdminNDCode = accModel.DPIU;
                    }
                    else
                    {
                        AdminNDCode = PMGSYSession.Current.AdminNdCode;
                    }

                    if (PMGSYSession.Current.LevelId == 6)
                    {
                        accModel.AdminNdCode = accModel.SRRDA;
                    }
                    else
                    {
                        accModel.AdminNdCode = PMGSYSession.Current.ParentNDCode.Value;
                    }

                    if (accModel.levelId == 4 || accModel.levelId == 6)
                    {
                        accModel.DPIUName = dbContext.ADMIN_DEPARTMENT.Where(m => m.ADMIN_ND_CODE == accModel.DPIU).Select(m => m.ADMIN_ND_NAME).FirstOrDefault();
                        accModel.NodalAgency = dbContext.ADMIN_DEPARTMENT.Where(m => m.ADMIN_ND_CODE == (dbContext.ADMIN_DEPARTMENT.Where(p => p.ADMIN_ND_CODE == accModel.AdminNdCode).Select(p => p.MAST_PARENT_ND_CODE).FirstOrDefault())).Select(m => m.ADMIN_ND_NAME).FirstOrDefault();
                    }

                    if (accModel.levelId == 5)
                    {
                        accModel.DPIU = PMGSYSession.Current.AdminNdCode;
                        accModel.DPIUName = dbContext.ADMIN_DEPARTMENT.Where(m => m.ADMIN_ND_CODE == accModel.DPIU).Select(m => m.ADMIN_ND_NAME).FirstOrDefault();
                    }
                    else
                    {
                        accModel.NodalAgency = dbContext.ADMIN_DEPARTMENT.Where(m => m.ADMIN_ND_CODE == AdminNDCode).Select(m => m.ADMIN_ND_NAME).FirstOrDefault();
                        accModel.DPIUName = dbContext.ADMIN_DEPARTMENT.Where(m => m.ADMIN_ND_CODE == accModel.DPIU).Select(m => m.ADMIN_ND_NAME).FirstOrDefault();
                    }

                    if (accModel.DPIU > 0)
                    {
                        accModel.NodalAgency = accModel.DPIUName;
                    }

                    if (accModel.StartDate == null || accModel.StartDate == string.Empty)
                    {
                        accModel.StartDate = null;
                    }

                    if (accModel.EndDate == null || accModel.EndDate == string.Empty)
                    {
                        accModel.EndDate = null;
                    }

                    if (accModel.DPIUName == null)
                    {
                        accModel.DPIUName = "-";
                    }
                    if (accModel.Month > 0)
                    {
                        string MonthName = objComm.getMonthText(accModel.Month);
                        accModel.YearName = MonthName + "-" + accModel.Year;
                    }
                    else
                    {
                        accModel.YearName = accModel.Year + "-" + Convert.ToString(accModel.Year + 1);
                    }

                    if (PMGSYSession.Current.StateName != null && PMGSYSession.Current.StateName != string.Empty)
                    {
                        accModel.StateName = PMGSYSession.Current.StateName;
                    }
                    else
                    {
                        PMGSYSession.Current.StateName = "-";
                        //accModel.StateName = "-";
                    }

                    if (accModel.DPIU == 0)
                    {
                        accModel.DPIUName = "All DPIU";
                    }

                    if (accModel.AdminNdCode == 0)
                    {
                        accModel.StateName = "All States";
                    }

                    if (string.IsNullOrEmpty(accModel.StateName) || accModel.StateName.Equals("-"))
                    {
                        var stateCode = (from item in dbContext.ADMIN_DEPARTMENT
                                         where item.MAST_PARENT_ND_CODE == accModel.AdminNdCode
                                         select item.MAST_STATE_CODE).ToList();

                        int sc = Convert.ToInt32(stateCode.ElementAt(0));

                        accModel.StateName = (from item in dbContext.MASTER_STATE
                                              where item.MAST_STATE_CODE == sc
                                              select item.MAST_STATE_NAME).ToList().ElementAt(0);
                        //accModel.StateName = stateName.ElementAt(0);

                    }


                    accModel.Selection = accModel.rType == "M" ? "Monthly" : accModel.rType == "Y" ? "Yearly" : accModel.rType == "P" ? "Periodic" : string.Empty;
                    accModel.isAllPiu = AllPIU;
                    return PartialView(accModel);
                }
                return PartialView(accModel);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "AccountReports.Account.ShowMISPaymentReport");
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

        ///SUMMARY
        //Method to populate dpiu according to selected state
        //added by abhinav pathak on 04-DEC-2018
        public JsonResult PopulateDPIUForStates(string id)
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

        #endregion

        #region PFMS Daywise Payment Details
        [Audit]
        [HttpGet]
        public ActionResult DisplayPFMSPendingBills(string id)
        {

            CommonFunctions objCommon = null;
            PfmsPendingBills accModel = null;
            TransactionParams objParam = null;
            PMGSYEntities dbContext = null;
            try
            {
                objCommon = new CommonFunctions();
                accModel = new PfmsPendingBills();
                objParam = new TransactionParams();
                dbContext = new PMGSYEntities();

                if (id != null)
                {
                    accModel.StartDate = id.Split('$')[0];
                    accModel.StartDate = accModel.StartDate.Replace('-', '/');
                    accModel.EndDate = id.Split('$')[1];
                    accModel.EndDate = accModel.EndDate.Replace('-', '/');
                }

                if (PMGSYSession.Current.UserId == 0)
                {
                    Response.Redirect("/Login/SessionExpire");
                }

                if (PMGSYSession.Current.LevelId == 4 || PMGSYSession.Current.LevelId == 6)
                {
                    //populate SRRDA
                    List<SelectListItem> lstSRRDA = new List<SelectListItem>();
                    lstSRRDA = objCommon.PopulateNodalAgencies();
                    if (lstSRRDA != null)
                    {
                        lstSRRDA.Insert(0, new SelectListItem() { Selected = true, Text = "All States", Value = "0" });
                        ViewBag.SRRDA = lstSRRDA;
                    }

                    //Populate DPIU
                    List<SelectListItem> lstDPIU = new List<SelectListItem>();
                    lstDPIU = objCommon.PopulateDPIUOfSRRDA(accModel.SRRDA);
                    if (lstDPIU != null)
                    {
                        lstDPIU.Insert(0, new SelectListItem() { Text = "All PIU", Value = "0" });
                        ViewBag.DPIU = lstDPIU;
                    }
                }

                if (PMGSYSession.Current.LevelId == 5)
                {

                    List<SelectListItem> lstSRRDA = new List<SelectListItem>();

                    lstSRRDA.Insert(0, new SelectListItem() { Selected = true, Text = PMGSYSession.Current.StateName, Value = PMGSYSession.Current.StateCode.ToString() });
                    ViewBag.SRRDA = lstSRRDA;


                    List<SelectListItem> lstDPIU = new List<SelectListItem>();
                    accModel.DPIU = PMGSYSession.Current.AdminNdCode;
                    accModel.DPIUName = dbContext.ADMIN_DEPARTMENT.Where(m => m.ADMIN_ND_CODE == accModel.DPIU).Select(m => m.ADMIN_ND_NAME).FirstOrDefault();
                    lstDPIU.Insert(0, new SelectListItem() { Selected = true, Text = accModel.DPIUName, Value = PMGSYSession.Current.DistrictCode.ToString() });
                    ViewBag.DPIU = lstDPIU;
                }


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

                accModel.MonthList = objCommon.PopulateMonths(objParam.MONTH);
                accModel.YearList = objCommon.PopulateYears(objParam.YEAR);

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
                accModel.rType = "P";
                return View(accModel);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "AccountReports.AccountController.DisplayMISPayment()");
                return null;
            }
        }


        [HttpPost]
        [Audit]
        public ActionResult PFMSPendingBillsReport(PfmsPendingBills accModel)
        {

            accModel.FundType = PMGSYSession.Current.FundType;
            accModel.levelId = PMGSYSession.Current.LevelId;
            PMGSYEntities dbContext = null;
            CommonFunctions objComm = null;
            try
            {
                dbContext = new PMGSYEntities();
                objComm = new CommonFunctions();

                if (ModelState.IsValid)
                {

                    int AdminNDCode = accModel.AdminNdCode;
                    int AllPIU = 0;

                    if (accModel.NodalAgency == "D")
                    {
                        AllPIU = accModel.DPIU == 0 ? 1 : 0;
                    }

                    if (accModel.NodalAgency == "D" && accModel.DPIU > 0)
                    {
                        accModel.levelId = 5;
                    }

                    //check srrda/DPIU
                    if (accModel.DPIU > 0)
                    {
                        AdminNDCode = accModel.DPIU;
                    }
                    else
                    {
                        AdminNDCode = PMGSYSession.Current.AdminNdCode;
                    }

                    if (PMGSYSession.Current.LevelId == 6)
                    {
                        accModel.AdminNdCode = accModel.SRRDA;
                    }
                    else
                    {
                        accModel.AdminNdCode = PMGSYSession.Current.ParentNDCode.Value;
                    }

                    if (accModel.levelId == 4 || accModel.levelId == 6)
                    {
                        accModel.DPIUName = dbContext.ADMIN_DEPARTMENT.Where(m => m.ADMIN_ND_CODE == accModel.DPIU).Select(m => m.ADMIN_ND_NAME).FirstOrDefault();
                        accModel.NodalAgency = dbContext.ADMIN_DEPARTMENT.Where(m => m.ADMIN_ND_CODE == (dbContext.ADMIN_DEPARTMENT.Where(p => p.ADMIN_ND_CODE == accModel.AdminNdCode).Select(p => p.MAST_PARENT_ND_CODE).FirstOrDefault())).Select(m => m.ADMIN_ND_NAME).FirstOrDefault();
                    }

                    if (accModel.levelId == 5)
                    {
                        accModel.DPIU = PMGSYSession.Current.AdminNdCode;
                        accModel.DPIUName = dbContext.ADMIN_DEPARTMENT.Where(m => m.ADMIN_ND_CODE == accModel.DPIU).Select(m => m.ADMIN_ND_NAME).FirstOrDefault();
                    }
                    else
                    {
                        accModel.NodalAgency = dbContext.ADMIN_DEPARTMENT.Where(m => m.ADMIN_ND_CODE == AdminNDCode).Select(m => m.ADMIN_ND_NAME).FirstOrDefault();
                        accModel.DPIUName = dbContext.ADMIN_DEPARTMENT.Where(m => m.ADMIN_ND_CODE == accModel.DPIU).Select(m => m.ADMIN_ND_NAME).FirstOrDefault();
                    }

                    if (accModel.DPIU > 0)
                    {
                        accModel.NodalAgency = accModel.DPIUName;
                    }

                    if (accModel.StartDate == null || accModel.StartDate == string.Empty)
                    {
                        accModel.StartDate = null;
                    }

                    if (accModel.EndDate == null || accModel.EndDate == string.Empty)
                    {
                        accModel.EndDate = null;
                    }

                    if (accModel.DPIUName == null)
                    {
                        accModel.DPIUName = "-";
                    }
                    if (accModel.Month > 0)
                    {
                        string MonthName = objComm.getMonthText(accModel.Month);
                        accModel.YearName = MonthName + "-" + accModel.Year;
                    }
                    else
                    {
                        accModel.YearName = accModel.Year + "-" + Convert.ToString(accModel.Year + 1);
                    }

                    if (PMGSYSession.Current.StateName != null && PMGSYSession.Current.StateName != string.Empty)
                    {
                        accModel.StateName = PMGSYSession.Current.StateName;
                    }
                    else
                    {
                        PMGSYSession.Current.StateName = "-";
                        //accModel.StateName = "-";
                    }

                    if (accModel.DPIU == 0)
                    {
                        accModel.DPIUName = "All DPIU";
                    }

                    if (accModel.AdminNdCode == 0)
                    {
                        accModel.StateName = "All States";
                    }

                    if (string.IsNullOrEmpty(accModel.StateName) || accModel.StateName.Equals("-"))
                    {
                        var stateCode = (from item in dbContext.ADMIN_DEPARTMENT
                                         where item.MAST_PARENT_ND_CODE == accModel.AdminNdCode
                                         select item.MAST_STATE_CODE).ToList();

                        int sc = Convert.ToInt32(stateCode.ElementAt(0));

                        accModel.StateName = (from item in dbContext.MASTER_STATE
                                              where item.MAST_STATE_CODE == sc
                                              select item.MAST_STATE_NAME).ToList().ElementAt(0);
                        //accModel.StateName = stateName.ElementAt(0);

                    }


                    accModel.Selection = accModel.rType == "M" ? "Monthly" : accModel.rType == "Y" ? "Yearly" : accModel.rType == "P" ? "Periodic" : string.Empty;
                    accModel.isAllPiu = AllPIU;
                    return PartialView(accModel);
                }
                return PartialView(accModel);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "AccountReports.Account.ShowMISPaymentReport");
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

        #endregion


        //added by sachin
        #region REAT SSRS Report
        //ADDED BY SACHIN 30/7/2020
        // module type added by sachin
        [Audit]
        public ActionResult DisplayReatLayoutView()
        {
            if (PMGSYSession.Current.UserId == 0)
            {
                Response.Redirect("/Login/SessionExpire");
            }

            CommonFunctions objCommon = new CommonFunctions();
            CashPaymentViewModel accModel = new CashPaymentViewModel();
            TransactionParams objParam = new TransactionParams();

            try
            {
                if (PMGSYSession.Current.LevelId == 4 || PMGSYSession.Current.LevelId == 6)
                {
                    //populate SRRDA
                    List<SelectListItem> lstSRRDA = new List<SelectListItem>();
                    lstSRRDA = objCommon.PopulateNodalAgencies();

                    ViewBag.SRRDA = lstSRRDA;

                    //Populate DPIU
                    List<SelectListItem> lstDPIU = new List<SelectListItem>();
                    lstDPIU = objCommon.PopulateDPIUOfSRRDA(accModel.SRRDA);
                    lstDPIU.Insert(0, new SelectListItem { Text = "All PIU", Value = "0" });
                    ViewBag.DPIU = lstDPIU;
                }
                //Added by abhishek kamble 3-oct-2013 end
                accModel.ModuleType = new List<SelectListItem> {     new SelectListItem{ Text = "Reat Module", Value ="R" , Selected = true }, 
                                                            new SelectListItem{ Text = "DBT Module", Value ="D" }};

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

                //accModel.SRRDA = PMGSYSession.Current.AdminNdCode;
                List<SelectListItem> lstPIU = objCommon.PopulateDPIU(objParam);
                lstPIU.RemoveAt(0);
                lstPIU.Insert(0, new SelectListItem { Text = "All PIU", Value = "0" });

                ViewBag.DPIU = lstPIU;


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

                return View(accModel);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "AccountReports.AccountController.DisplayReatLayoutView()");
                return null;
            }
        }

        // CashPayment report post action method
        [HttpPost]
        [Audit]
        public ActionResult ShowReatReport(CashPaymentViewModel accModel)
        {
            //modified .......

            accModel.levelId = PMGSYSession.Current.LevelId;

            PMGSYEntities dbContext = new PMGSYEntities();
            CommonFunctions objComm = new CommonFunctions();
            try
            {
                if (ModelState.IsValid)
                {
                    int AdminNDCode = accModel.AdminNdCode;
                    int AllPIU = 0;

                    if (accModel.NodalAgency == "D")
                    {
                        AllPIU = accModel.DPIU == 0 ? 1 : 0;
                    }

                    if (accModel.NodalAgency == "D" && accModel.DPIU > 0)
                    {
                        accModel.levelId = 5;
                    }

                    //check srrda/DPIU
                    if (accModel.DPIU > 0)
                    {
                        AdminNDCode = accModel.DPIU;
                    }
                    else
                    {
                        AdminNDCode = PMGSYSession.Current.AdminNdCode;
                    }

                    DateTime? StartDate = null;
                    DateTime? EndDate = null;
                    if (accModel.StartDate != null && accModel.EndDate != null)
                    {
                        StartDate = objComm.GetStringToDateTime(accModel.StartDate);
                        EndDate = objComm.GetStringToDateTime(accModel.EndDate);
                    }
                    if (PMGSYSession.Current.LevelId == 6)
                    {
                        accModel.AdminNdCode = accModel.SRRDA;
                    }
                    else
                    {
                        accModel.AdminNdCode = PMGSYSession.Current.ParentNDCode.Value;
                    }

                    if (accModel.levelId == 4 || accModel.levelId == 6)
                    {
                        //accModel.DPIUBySRRDA = dbContext.ADMIN_DEPARTMENT.Where(m => m.ADMIN_ND_CODE == AdminNDCode).Select(m => m.ADMIN_ND_NAME).FirstOrDefault();
                        accModel.DPIUName = dbContext.ADMIN_DEPARTMENT.Where(m => m.ADMIN_ND_CODE == accModel.DPIU).Select(m => m.ADMIN_ND_NAME).FirstOrDefault();
                        accModel.NodalAgency = dbContext.ADMIN_DEPARTMENT.Where(m => m.ADMIN_ND_CODE == (dbContext.ADMIN_DEPARTMENT.Where(p => p.ADMIN_ND_CODE == accModel.AdminNdCode).Select(p => p.MAST_PARENT_ND_CODE).FirstOrDefault())).Select(m => m.ADMIN_ND_NAME).FirstOrDefault();

                    }
                    else
                    {
                        accModel.NodalAgency = dbContext.ADMIN_DEPARTMENT.Where(m => m.ADMIN_ND_CODE == AdminNDCode).Select(m => m.ADMIN_ND_NAME).FirstOrDefault();
                        accModel.DPIUName = null;
                        accModel.DPIU = accModel.DPIU == 0 ? PMGSYSession.Current.AdminNdCode : accModel.DPIU;
                    }

                    if (accModel.DPIU > 0)
                    {
                        accModel.NodalAgency = accModel.DPIUName;
                    }
                    //accModel.lstAccountBillDetails = dbContext.SP_ACC_RPT_DISPALY_Bill_DETAILS(PMGSYSession.Current.FundType, AdminNDCode, accModel.Month, accModel.Year, StartDate, EndDate, accModel.BillType, accModel.rType, AllPIU).ToList<SP_ACC_RPT_DISPALY_Bill_DETAILS_Result>();
                    //accModel.TotalRecords = accModel.lstAccountBillDetails.Count();
                    //return accModel;

                    if (accModel.StartDate == null || accModel.StartDate == string.Empty)
                    {
                        accModel.StartDate = "-";
                    }
                    else
                    {
                        accModel.StartDate = Convert.ToDateTime(accModel.StartDate).ToString("MM/dd/yyyy");
                    }
                    if (accModel.EndDate == null || accModel.EndDate == string.Empty)
                    {
                        accModel.EndDate = "-";
                    }
                    else
                    {
                        accModel.EndDate = Convert.ToDateTime(accModel.EndDate).ToString("MM/dd/yyyy");
                    }

                    if (accModel.DPIUName == null)
                    {
                        accModel.DPIUName = "-";
                    }
                    if (accModel.Month > 0)
                    {
                        string MonthName = objComm.getMonthText(accModel.Month);
                        accModel.YearName = MonthName + "-" + accModel.Year;
                    }
                    else
                    {
                        accModel.YearName = accModel.Year + "-" + Convert.ToString(accModel.Year + 1);
                    }


                    accModel.StateName =
                        // if (PMGSYSession.Current.StateName != null && PMGSYSession.Current.StateName != string.Empty)
                        // {
                        //    accModel.StateName = PMGSYSession.Current.StateName;
                        // }
                        // else
                        // {
                        //    PMGSYSession.Current.StateName = "-";
                        //     accModel.StateName = "-";
                        //  }

                    accModel.Selection = accModel.rType == "M" ? "Monthly" : accModel.rType == "Y" ? "Yearly" : accModel.rType == "P" ? "Periodic" : string.Empty;
                    //accModel.sDate = accModel.StartDate;
                    //accModel.eDate = accModel.EndDate;
                    accModel.isAllPiu = AllPIU;
                    return PartialView(accModel);
                }
                return PartialView(accModel);
            }
            catch (Exception ex)
            {
                //Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                ErrorLog.LogError(ex, "AccountReports.Account.ShowReatReport()");
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
        #endregion

        //added by sachin 30 july 2020
        #region Reat opening balance report
        [HttpGet]
        public ActionResult ReatOPLayout()
        {
            QualityReportsViewModel QM = new QualityReportsViewModel();
            CommonFunctions comm = new CommonFunctions();
            // Added By Sachin

            QM.AgencyList = comm.PopulateAgencies(PMGSYSession.Current.StateCode, false);
            try
            {
                QM.fundType = PMGSYSession.Current.FundType;
                QM.State = PMGSYSession.Current.StateCode;
                QM.StateName = PMGSYSession.Current.StateCode > 0 ? PMGSYSession.Current.StateName : "All States";
                QM.AgencyList = comm.PopulateAgencies(QM.State, false);
                if (PMGSYSession.Current.StateCode > 0)
                {
                    QM.StateList = new List<SelectListItem>();
                    QM.StateList.Insert(0, (new SelectListItem { Text = PMGSYSession.Current.StateName, Value = Convert.ToString(PMGSYSession.Current.StateCode), Selected = true }));
                }
                else
                {
                    QM.StateList = comm.PopulateStates(true);
                    QM.StateList.Find(x => x.Value == "0").Text = "All States";
                    QM.AgencyList.RemoveAt(0);
                    QM.AgencyList.Insert(0, (new SelectListItem { Text = "All Agency", Value = "0" }));
                }
                return View(QM);
            }
            catch (Exception ex)
            {
                //Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                ErrorLog.LogError(ex, "AccountReports.ReatOPLayout()");
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
        public JsonResult PopulateAgencies()
        {
            try
            {
                CommonFunctions objCommonFunctions = new CommonFunctions();
                int stateCode = Convert.ToInt32(Request.Params["stateCode"]);

                return Json(objCommonFunctions.PopulateAgencies(stateCode, false));
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "Account.PopulateAgencies()");
                return Json(new { string.Empty });
            }
        }





        [HttpPost]
        public ActionResult ReatOPLayoutReport(QualityReportsViewModel QM)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    //QM.qmType = PMGSYSession.Current.RoleCode == 8 ? "S" : "I";

                    QM.StateName = PMGSYSession.Current.StateCode > 0 ? PMGSYSession.Current.StateName : QM.StateName;
                    QM.fundType = PMGSYSession.Current.FundType;

                    return View(QM);
                }
                else
                {
                    string message = ModelState.Values.Where(x => x.Errors.Count == 1).FirstOrDefault().Errors[0].ErrorMessage;
                    return null;
                }
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "AccountReports.ReatOPLayoutReport()");
                return null;
            }
        }
        #endregion

        public ActionResult NewCashBookLayout()
        {
            NewCashbookModel newCashbook = new NewCashbookModel();
            CommonFunctions commomFuncObj = new CommonFunctions();

            try
            {
                if (PMGSYSession.Current.AccMonth != 0)
                {
                    newCashbook.Month = PMGSYSession.Current.AccMonth;
                    newCashbook.Year = PMGSYSession.Current.AccYear;
                }
                else
                {
                    newCashbook.Month = Convert.ToInt16(DateTime.Now.Month);
                    newCashbook.Year = Convert.ToInt16(DateTime.Now.Year);
                }

                ViewBag.ddlMonth = commomFuncObj.PopulateMonths(DateTime.Now.Month);
                ViewBag.ddlYear = commomFuncObj.PopulateYears(DateTime.Now.Year);


                if (PMGSYSession.Current.LevelId == 4 || PMGSYSession.Current.LevelId == 6)
                {
                    //populate SRRDA
                    List<SelectListItem> lstSRRDA = new List<SelectListItem>();
                    lstSRRDA = commomFuncObj.PopulateNodalAgencies();
                    ViewBag.SRRDA = lstSRRDA;

                    //Populate DPIU
                    List<SelectListItem> lstDPIU = new List<SelectListItem>();
                    lstDPIU.Insert(0, new SelectListItem { Text = "Select DPIU", Value = "0" });
                    ViewBag.DPIU = lstDPIU;
                }

                return View(newCashbook);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "NewCashBookLayout()");
                return null;
            }

        }

        [HttpPost]
        public ActionResult NewCashBookReport(NewCashbookModel newCashbook)
        {

            CommonFunctions commomFuncObj = new CommonFunctions();
            string SRRDA_DPIU = newCashbook.SRRDA_DPIU;
            int DPIU_NdCode = newCashbook.DPIU;

            if (PMGSYSession.Current.LevelId == 4 || PMGSYSession.Current.LevelId == 6)
            {
                //populate SRRDA
                CommonFunctions objCommonFunction = new CommonFunctions();
                List<SelectListItem> lstSRRDA = new List<SelectListItem>();
                lstSRRDA = objCommonFunction.PopulateNodalAgencies();
                ViewBag.SRRDA = lstSRRDA;

                //Populate DPIU
                TransactionParams objTransParam = new TransactionParams();

                int AdminNdCode = newCashbook.SRRDA;
                objTransParam.ADMIN_ND_CODE = AdminNdCode;

                List<SelectListItem> lstDPIU = commomFuncObj.PopulateDPIU(objTransParam);
                lstDPIU.RemoveAt(0);
                lstDPIU.Insert(0, new SelectListItem { Text = "Select DPIU", Value = "0" });
                var selectedDPIU = lstDPIU.Where(m => m.Value == newCashbook.DPIU.ToString()).First();
                selectedDPIU.Selected = true;
                ViewBag.DPIU = lstDPIU;
            }

            PMGSY.Models.Report.ReportFilter objParam = new PMGSY.Models.Report.ReportFilter();
            objParam.Month = newCashbook.Month;
            objParam.Year = newCashbook.Year;
            objParam.Selection = newCashbook.SRRDA_DPIU;
            objParam.Dpiu = newCashbook.DPIU;

            ViewBag.ddlMonth = commomFuncObj.PopulateMonths(objParam.Month);
            ViewBag.ddlYear = commomFuncObj.PopulateYears(objParam.Year);


            //Login Mord/SRRDA
            if (PMGSYSession.Current.LevelId == 4 || PMGSYSession.Current.LevelId == 6)
            {
                if (newCashbook.SRRDA_DPIU == "S")
                {
                    objParam.AdminNdCode = newCashbook.SRRDA;
                }
                else if (newCashbook.SRRDA_DPIU == "D")
                {
                    objParam.AdminNdCode = newCashbook.DPIU;
                }
            }
            else
            { //login DPIU                                         
                objParam.AdminNdCode = PMGSYSession.Current.AdminNdCode;
            }

            objParam.FundType = PMGSYSession.Current.FundType;

            newCashbook.SRRDA_DPIU = SRRDA_DPIU;

            if ((SRRDA_DPIU == "D" || SRRDA_DPIU == "S") && (DPIU_NdCode == 0))
            {
                //cbSingle.DistrictDepartment = "-";
            }
            else
            {
                newCashbook.DPIU = DPIU_NdCode;
            }


            //Set ADMIN_ND_CODE
            newCashbook.ADMIN_ND_CODE = objParam.AdminNdCode;

            //set Level
            short LevelId = 0;
            if (objParam.Selection == "D" && objParam.Dpiu != 0)
            {
                LevelId = 5;
            }
            else
            {
                if (PMGSYSession.Current.LevelId == 6)
                {
                    LevelId = 4;
                }
                else
                {
                    LevelId = PMGSYSession.Current.LevelId;
                }
            }
            newCashbook.LvlId = LevelId;
            return View(newCashbook);
        }
    }
}
