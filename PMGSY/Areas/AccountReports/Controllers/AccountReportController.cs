using PMGSY.Areas.AccountReports.Models;
using PMGSY.Common;
using PMGSY.Extensions;
using PMGSY.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PMGSY.Areas.AccountReports.Controllers
{
    public partial class AccountController : Controller
    {
        //
        // GET: /AccountReports/AccountReport/

        //public ActionResult Index()
        //{
        //    return View();
        //}


        //public ActionResult MasterSheetLayout()
        //{
        //    CommonFunctions cm = new CommonFunctions();
        //    MasterSheetModel masterSheetModel = new MasterSheetModel();
        //    masterSheetModel.Year = Request.Params["year"] == null ? DateTime.Now.Year - 1 : Convert.ToInt32(Request.Params["year"]);
        //    masterSheetModel.YEAR_LIST = cm.PopulateFinancialYear(true);

        //    return View(masterSheetModel);
        //}

       // public ActionResult MasterSheetReport(MasterSheetModel mastersheet)
       // {

       //     //PMGSY.Common.FetchCookieData fetchCookieData = new PMGSY.Common.FetchCookieData();
           
       //     PMGSY.Models.PMGSYEntities dbcontext = new PMGSY.Models.PMGSYEntities();
           
       //     mastersheet.LvlId = 2;
       //     mastersheet.StateCode = PMGSYSession.Current.StateCode;
       //     mastersheet.Agency = dbcontext.ADMIN_DEPARTMENT.Where(c => c.ADMIN_ND_CODE == PMGSYSession.Current.AdminNdCode).Select(c => c.MAST_AGENCY_CODE).First();
       //     mastersheet.StateName = PMGSYSession.Current.StateName;
       //     mastersheet.Year = Convert.ToInt32(Request.Params["year"]);
       //     mastersheet.FundType = PMGSYSession.Current.FundType;
       //     return View(mastersheet);

       // }


       // public ActionResult AuthorizedSignatoryLayout()
       // {
       //     try
       //     {
       //         //if (PMGSYSession.Current.UserId == 0)
       //         //{
       //         //    Response.Redirect("/Login/SessionExpire");
       //         //}
       //         return View("AuthorizedSignatoryLayout", new AuthorizedSignatoryModel());
       //     }
       //     catch (Exception ex)
       //     {
       //         Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);

       //         return null;
       //     }
       // }


       // public ActionResult AuthorizedSignatoryReport( AuthorizedSignatoryModel authSignatoryViewModel)
       // {
       //     //if (PMGSYSession.Current.UserId == 0)
       //     //{
       //     //    Response.Redirect("/Login/SessionExpire");
       //     //}
         
       //     int AdminNDCode = 0;
       //     int LevelID = 0;

       //     //if DPIU Dropdown is selected then AdminNDCode variable stores selected dropdown ADMIN_ND_CODE value 
       //     if (authSignatoryViewModel.DPIU > 0) //DPIU
       //     {
       //         AdminNDCode = authSignatoryViewModel.DPIU;
       //         LevelID = 5;
       //     }
       //     else//SRRDA
       //     { //else we get ADMIN_ND_CODE from session
       //         AdminNDCode = PMGSYSession.Current.AdminNdCode;
       //         LevelID = 4;
       //     }
       //     authSignatoryViewModel.DPIU = AdminNDCode;
       //     authSignatoryViewModel.LevelID = LevelID;


       //     return View(authSignatoryViewModel);
       //     //return PartialView("AuthorizedSignatoryReport", authSignatoryViewModel);
        
       // }

        //public ActionResult FundTransferLayout()
        //{
            
        //    string ScreenName = "FundTransfer";
        //    CommanViewFunction(ScreenName);
            
        //    return View();
        //}
        //private void CommanViewFunction(string Name)
        //{
        //    CommonFunctions objCommFunc = new CommonFunctions();
           
        //    short month = 0;
        //    short year = 0;
        //    if (PMGSYSession.Current.AccMonth != 0)
        //    {
        //        month = PMGSYSession.Current.AccMonth;
        //        year = PMGSYSession.Current.AccYear;
        //    }
        //    else
        //    {
        //        month = Convert.ToInt16(DateTime.Now.Month);
        //        year = Convert.ToInt16(DateTime.Now.Year);
        //    }


        //    if (Name == "AbstractBankAuthorization" || Name == "AbstractFund")
        //    {
        //        ViewBag.Year = objCommFunc.PopulateFinancialYear(true);
        //    }
        //    else
        //    {
        //        ViewBag.Year = objCommFunc.PopulateYears(year);
        //    }

        //    ViewBag.Month = objCommFunc.PopulateMonths(month);
        //    if (Name != "BankAuthrization")
        //    {
        //        ViewBag.FundSelection = PopulateFund();
        //    }

        //    List<SelectListItem> lstSRRDA = new List<SelectListItem>();

        //    lstSRRDA = objCommFunc.PopulateNodalAgencies();
        //    lstSRRDA.Insert(0, new SelectListItem { Text = "Select State", Value = "0" });

        //    if (PMGSYSession.Current.LevelId == 4)
        //    {
        //        var selected = lstSRRDA.Where(m => m.Value == PMGSYSession.Current.AdminNdCode.ToString()).First();
        //        selected.Selected = true;
        //    }

        //    ViewBag.SRRDA = lstSRRDA;

        //    List<SelectListItem> lstDPIU = new List<SelectListItem>();
    
        //    lstDPIU.Insert(0, new SelectListItem { Text = "All DPIU", Value = "0" });
        //    ViewBag.DPIU = lstDPIU;
         
        //    ViewBag.SelectedMonth = month;
        //    ViewBag.SelectedYear = year;

        //}

        //public ActionResult FundTransferReport(FundTransferViewModel objFundTransfer)
        //{
        //    PMGSY.Models.Report.ReportFilter objParam = new PMGSY.Models.Report.ReportFilter();
        //    if (ModelState.IsValid)
        //    {

        //        if (PMGSYSession.Current.LevelId == 5)
        //        {
        //            objParam.AdminNdCode = Convert.ToInt32(PMGSYSession.Current.ParentNDCode);
        //            objParam.LowerAdminNdCode = PMGSYSession.Current.AdminNdCode;
        //        }
        //        else
        //        {
        //            objParam.AdminNdCode = Convert.ToInt32(objFundTransfer.StateCode);
        //            objParam.LowerAdminNdCode = Convert.ToInt32(objFundTransfer.DPIUCode);
        //        }
        //        objParam.Head = Convert.ToInt16(objFundTransfer.HeadCode);
        //        objParam.Month = Convert.ToInt16(objFundTransfer.Month);
        //        objParam.Year = Convert.ToInt16(objFundTransfer.Year);

        //        objParam.HeadName = objFundTransfer.HeadName;

        //        objFundTransfer.StateCode = objParam.AdminNdCode.ToString();
        //        objFundTransfer.DPIUCode = objParam.LowerAdminNdCode.ToString();

        //        ViewBag.HeadName = objFundTransfer.HeadName;

        //        //if (PMGSYSession.Current.LevelId == 5)
        //        //{
        //        //    PMGSY.DAL.Reports.IReportDAL objReportDAL = new PMGSY.DAL.Reports.ReportDAL();
        //        //    ViewBag.StateName = objReportDAL.GetDepartmentName(objParam.AdminNdCode);
        //        //    ViewBag.DPIUName = PMGSYSession.Current.DepartmentName;
        //        //}
        //        //else
        //        //{
                    
        //        //}

        //        //objFundTransfer = GetFundTransferDetails(objParam);

        //        return View(objFundTransfer);
        //    }
        //    else
        //    {
        //        return Json(new { success = false, message = new CommonFunctions().FormatErrorMessage(ModelState) });
        //    }

        //}


        //public List<SelectListItem> PopulateFund()
        //{
        //    PMGSY.Models.PMGSYEntities dbContext = new PMGSY.Models.PMGSYEntities();
        //    List<SelectListItem> lstFund = new List<SelectListItem>();
        //    try
        //    {


        //        var lstFundType = dbContext.USP_ACC_RPT_Get_HEAD_for_Bank_Authorisation(PMGSYSession.Current.FundType).ToList();

        //        foreach (var item in lstFundType)
        //        {
        //            lstFund.Add(new SelectListItem { Value = item.HEAD_ID.ToString(), Text = item.NAME.ToString().Trim() });
        //        }

        //        lstFund.Insert(0, new SelectListItem { Value = "0", Text = "Select Fund" });
        //        return lstFund;
        //    }
        //    catch (Exception ex)
        //    {
               
        //        return lstFund;

        //    }
        //    finally
        //    {
        //        if (dbContext != null)
        //        {
        //            dbContext.Dispose();
        //        }
        //    }
        //}




        //public ActionResult AbstractFundLayout()
        //{
        //    string ScreenName = "AbstractFund";
        //    CommanViewFunction(ScreenName);
        //    return View();
        //}


        //public ActionResult AbstractFundReport(AbstractFundTransferredViewModel objAbstractFund)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        return View("AbstractFundReport", objAbstractFund);
        //    }
        //    else
        //    {
        //        return Json(new { success = false, message = new CommonFunctions().FormatErrorMessage(ModelState) });
        //    }
        //}


        //public ActionResult BankAuthrizationLayout()
        //{
        //    string ScreenName = "BankAuthrization";
        //    CommanViewFunction(ScreenName);
        //    return View();
        //}
        //public ActionResult BankAuthrizationReport(BankAuthrizationViewModel objBankAuthrization)
        //{
        //    PMGSY.Models.Report.ReportFilter objParam = new PMGSY.Models.Report.ReportFilter();


        //    ViewBag.Month = objBankAuthrization.MonthName;
        //    ViewBag.Year = objBankAuthrization.YearName;

        //    if (PMGSYSession.Current.LevelId == 5)
        //    {
        //        objParam.AdminNdCode = Convert.ToInt32(PMGSYSession.Current.ParentNDCode);
        //        objParam.LowerAdminNdCode = PMGSYSession.Current.AdminNdCode;
        //    }
        //    else
        //    {
        //        objParam.AdminNdCode = Convert.ToInt32(objBankAuthrization.State);
        //        objParam.LowerAdminNdCode = Convert.ToInt32(objBankAuthrization.DPIU);
        //    }
        //    objParam.Month = Convert.ToInt16(objBankAuthrization.Month);
        //    objParam.Year = Convert.ToInt16(objBankAuthrization.Year);
        //    objParam.State = objParam.AdminNdCode;
        //    objParam.Dpiu = objParam.LowerAdminNdCode;

        //    return View(objBankAuthrization);
        //}

        //public ActionResult AbstractBankAuthorizationLayout()
        //{
        //    string name = "AbstractBankAuthorization";
        //    CommanViewFunction(name);
        //    return View();
        //}

        //public ActionResult AbstractBankAuthReport(AbstractBankAuthViewModel abstractBankAuthViewModel)
        //{
        //    return View("AbstractBankAuthReport", abstractBankAuthViewModel);
        //    //return PartialView("AbstractBankAuthReport", objAccountBAL.AbstractBankAuthDetails(abstractBankAuthViewModel));
        //}

        public ActionResult ViewRegisterDepositsLayout()
        {
            CommonFunctions commomFuncObj = new CommonFunctions();
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
        [HttpPost]
        public ActionResult ListRegisterReport(int Month, int Year, int HeadId, string ReportType, int DPIUCode, int FundingAgencyCode)
        {
            //int totalRecords = 0;
            //string page = frmCollection["page"];
            //string rows = "999999";//frmCollection["rows"];
            //string sidx = frmCollection["sidx"];
            //string sord = frmCollection["sord"];
            RegisterViewModel model = new RegisterViewModel();
            PMGSY.Models.PMGSYEntities db = new PMGSY.Models.PMGSYEntities();
            try
            {
                //Adde By Abhishek kamble 29-Apr-2014 start
                //using (CommonFunctions commonFunction = new CommonFunctions())
                //{
                //    if (!commonFunction.ValidateGridParameters(new GridParams(Convert.ToInt32(frmCollection["page"]), Convert.ToInt32(frmCollection["rows"]), frmCollection["sidx"], frmCollection["sord"], Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                //    {
                //        return null;
                //    }
                //}
                //Adde By Abhishek kamble 29-Apr-2014 end
                //RegisterViewModel model = new RegisterViewModel();
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
                        model.LevelId = PMGSYSession.Current.LevelId;
                        break;
                    default:
                        break;
                }
                model.Collaboration = db.ADMIN_DEPARTMENT.Where(m => m.ADMIN_ND_CODE == model.AdminCode).Select(m => m.MAST_AGENCY_CODE).FirstOrDefault();

                return View();
            }
            catch (Exception)
            {
                return null;
            }
        }

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
                    model.ReportType = Request.Params["ReportType"];

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
                       if(model.Balance=="C")
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



    }

}
