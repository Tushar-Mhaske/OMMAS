using PMGSY.BAL.RevokeClosing;
using PMGSY.Common;
using PMGSY.Extensions;
using PMGSY.Models.RevokeClosing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PMGSY.Controllers
{
    [RequiredAuthentication]
    [RequiredAuthorization]
    public class RevokeClosingController : Controller
    {
        //
        // GET: /RevokeClosing/

        private RevokeClosingBAL objRevokeClosingBAL = new RevokeClosingBAL();
        private CommonFunctions commomFuncObj = new CommonFunctions();

        [Audit]
        public ActionResult RevokeClosing()
        {



            RevokeClosingModel revokeClosingModel = new RevokeClosingModel();
            revokeClosingModel.ToMonth = Convert.ToInt16(DateTime.Now.Month);
            revokeClosingModel.ToYear = Convert.ToInt16(DateTime.Now.Year);
            ViewBag.ddlMonth = commomFuncObj.PopulateMonths();
            ViewBag.ddlYear = commomFuncObj.PopulateYears();
            ViewBag.ToMonth = commomFuncObj.PopulateMonths(DateTime.Now.Month);
            ViewBag.ToYear = commomFuncObj.PopulateYears(DateTime.Now.Year); 
            revokeClosingModel.AdminNdCode = PMGSYSession.Current.AdminNdCode;
            revokeClosingModel.LevelID = PMGSYSession.Current.LevelId;
            revokeClosingModel.FundType = PMGSYSession.Current.FundType == null ? "P" : PMGSYSession.Current.FundType;
            
            if (PMGSYSession.Current.FundType == null)
            {
                List<SelectListItem>lstFund=commomFuncObj.PopulateFundTypes(false);
                lstFund.Insert(0,new SelectListItem{Text="Select Fund",Value="0"});
                ViewBag.ddlFund = lstFund;
            }

            revokeClosingModel.MonthClosed = objRevokeClosingBAL.GetLastMonthClosed(revokeClosingModel.AdminNdCode, revokeClosingModel.FundType, revokeClosingModel.LevelID);

            //Added Abhishek kamble for monthly cloasing of DPIU at SRRDA level 28-Aug-2014
            if (PMGSYSession.Current.LevelId == 4 || PMGSYSession.Current.LevelId == 6)//accmord finance login
            {
                List<SelectListItem> lstMonths=commomFuncObj.PopulateDPIUOfSRRDA(PMGSYSession.Current.AdminNdCode);
                lstMonths.Insert(0,new SelectListItem{Text="Select DPIU",Value="0"});
                revokeClosingModel.DPIU_LIST = lstMonths;
            }

            if (PMGSYSession.Current.LevelId == 4 || PMGSYSession.Current.LevelId == 6)//accmord finance login
            {                
                //populate SRRDA
                List<SelectListItem> lstSRRDA = new List<SelectListItem>();
                lstSRRDA = commomFuncObj.PopulateNodalAgencies();
                // ViewBag.SRRDA = lstSRRDA;
                lstSRRDA.Insert(0, new SelectListItem { Text = "Select SRRDA", Value = "0" });
                revokeClosingModel.SRRDA_LIST = lstSRRDA;
            }

            return View(revokeClosingModel);
        }

        #region Old Revoking COde
        //[HttpPost]
        //[Audit]
        //public ActionResult IsValidRevoke(FormCollection frmCollect)
        //{

        //    //parameter added for Revoke DPIU at SRRDA level by abhishek 11Sep2014 start.
        //    int AdminNdCode = 0;
        //    short levelId = 0;
        //    String OwnDPIUFlag = String.Empty;


        //    if (PMGSYSession.Current.LevelId == 4)
        //    {

        //        if (!(String.IsNullOrEmpty(frmCollect["OwnDPIUFlag"])) && !(String.IsNullOrEmpty(frmCollect["DPIU_CODE"])))
        //        {
        //            if (frmCollect["OwnDPIUFlag"] == "O")
        //            {
        //                AdminNdCode = PMGSYSession.Current.AdminNdCode;
        //                levelId = PMGSYSession.Current.LevelId;
        //            }
        //            else
        //            {
        //                AdminNdCode = Convert.ToInt32(frmCollect["DPIU_CODE"]);
        //                levelId = 5;
        //            }
        //        }
        //        else
        //        {
        //            return this.Json(new { success = false, message = "Please select Own/Lower (DPIU) " });
        //        }
        //    }
        //    else if (PMGSYSession.Current.LevelId == 6)
        //    {

        //        if (!(String.IsNullOrEmpty(frmCollect["OwnDPIUFlag"])) && !(String.IsNullOrEmpty(frmCollect["DPIU_CODE"])))
        //        {
        //            if (frmCollect["OwnDPIUFlag"] == "O")
        //            {
        //                AdminNdCode = Convert.ToInt32(frmCollect["SRRDA_CODE"]);
        //                levelId = 4;
        //            }
        //            else
        //            {
        //                AdminNdCode = Convert.ToInt32(frmCollect["DPIU_CODE"]);
        //                levelId = 5;
        //            }
        //        }
        //        else
        //        {
        //            return this.Json(new { success = false, message = "Please select Own/Lower (DPIU) " });
        //        }
        //    }
        //    else
        //    {
        //        AdminNdCode = PMGSYSession.Current.AdminNdCode;
        //        levelId = PMGSYSession.Current.LevelId;
        //    }
        //    //parameter added for Revoke DPIU at SRRDA level by abhishek 11Sep2014 end.
        //    RevokeClosingModel rcModel = new RevokeClosingModel();

        //    //old  admin code and level ID modified for revoke month 
        //    //rcModel.AdminNdCode = PMGSYSession.Current.AdminNdCode;
        //    rcModel.AdminNdCode = AdminNdCode;
        //    //rcModel.LevelID = PMGSYSession.Current.LevelId;
        //    rcModel.LevelID = levelId;

        //    rcModel.FundType = PMGSYSession.Current.FundType == null ? frmCollect["FundType"] : PMGSYSession.Current.FundType;

        //    rcModel.Month = Convert.ToInt16(frmCollect["Month"]);
        //    rcModel.Year = Convert.ToInt16(frmCollect["Year"]);

        //    String Status = objRevokeClosingBAL.GetRevokeStatus(rcModel);

        //    //Added By Abhishek kamble 5Jan2016 for Definalize and delete operation restriction start

        //    if (PMGSYSession.Current.LevelId != 6 && Status == "")//Finance Role
        //    {
        //        //if voucher is not in current month
        //        if (rcModel.Month != DateTime.Now.Month && rcModel.Year != DateTime.Now.Year)
        //        {
        //            int CurrentMonthMaxAllowedDays = 10;//Default
        //            if (!String.IsNullOrEmpty(System.Configuration.ConfigurationManager.AppSettings["ACCOUNT_REVOKE_CURRENT_MONTH_MAX_DAYS"]))
        //            {
        //                CurrentMonthMaxAllowedDays = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["ACCOUNT_REVOKE_CURRENT_MONTH_MAX_DAYS"]);
        //            }

        //            DateTime minAllowedDate = new DateTime(DateTime.Now.Date.AddMonths(-1).Year, DateTime.Now.Date.AddMonths(-1).Month, 1);
        //            DateTime maxAllowedDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, CurrentMonthMaxAllowedDays);

        //            //if (voucherBillDate >= minAllowedDate && voucherBillDate <= maxAllowedDate)
        //            if (!(rcModel.Month >= minAllowedDate.Month && rcModel.Year >= minAllowedDate.Year && DateTime.Now <= maxAllowedDate))
        //            {
        //                return this.Json(new { success = false, message = "You can not Revoke month because Revoke month time period is now elapsed" });
        //            }
        //        }
        //    }
        //    //Added By Abhishek kamble 5Jan2016 for Definalize and delete operation restriction end

        //    if (Status == "")
        //    {
        //        return this.Json(new { success = true, message = "" });
        //    }
        //    else
        //    {
        //        return this.Json(new { success = false, message = Status });
        //    }
        //}

        //[HttpPost]
        //[Audit]
        //public ActionResult RevokeClosing(FormCollection frmCollect)
        //{
        //    //parameter added for Revoke DPIU at SRRDA level by abhishek 11Sep2014 start.
        //    int AdminNdCode = 0;
        //    short levelId = 0;
        //    String OwnDPIUFlag = String.Empty;

        //    int SRRDAAdminNdCode = 0;

        //    RevokeClosingModel rcModel = new RevokeClosingModel();

        //    if (PMGSYSession.Current.LevelId == 4)
        //    {

        //        if (!(String.IsNullOrEmpty(frmCollect["OwnDPIUFlag"])) && !(String.IsNullOrEmpty(frmCollect["DPIU_CODE"])))
        //        {
        //            if (frmCollect["OwnDPIUFlag"] == "O")
        //            {
        //                AdminNdCode = PMGSYSession.Current.AdminNdCode;
        //                levelId = PMGSYSession.Current.LevelId;
        //            }
        //            else
        //            {
        //                AdminNdCode = Convert.ToInt32(frmCollect["DPIU_CODE"]);
        //                levelId = 5;
        //                // rcModel.DPIU_CODE = AdminNdCode;
        //            }
        //            SRRDAAdminNdCode = PMGSYSession.Current.AdminNdCode;
        //        }
        //        else
        //        {
        //            return this.Json(new { success = false, message = "Please select Own/Lower (DPIU) " });
        //        }
        //    }
        //    else if (PMGSYSession.Current.LevelId == 6)//accmorduser
        //    {

        //        if (!(String.IsNullOrEmpty(frmCollect["OwnDPIUFlag"])) && !(String.IsNullOrEmpty(frmCollect["DPIU_CODE"])))
        //        {
        //            if (frmCollect["OwnDPIUFlag"] == "O")
        //            {
        //                AdminNdCode = Convert.ToInt32(frmCollect["SRRDA_CODE"]);
        //                levelId = 4;
        //            }
        //            else
        //            {
        //                AdminNdCode = Convert.ToInt32(frmCollect["DPIU_CODE"]);
        //                levelId = 5;
        //                // rcModel.DPIU_CODE = AdminNdCode;
        //            }
        //            SRRDAAdminNdCode = Convert.ToInt32(frmCollect["SRRDA_CODE"]);
        //        }
        //        else
        //        {
        //            return this.Json(new { success = false, message = "Please select Own/Lower (DPIU) " });
        //        }
        //    }
        //    else
        //    {
        //        AdminNdCode = PMGSYSession.Current.AdminNdCode;
        //        levelId = PMGSYSession.Current.LevelId;
        //    }
        //    //parameter added for Revoke DPIU at SRRDA level by abhishek 11Sep2014 end.


        //    //rcModel.AdminNdCode = PMGSYSession.Current.AdminNdCode;
        //    rcModel.AdminNdCode = AdminNdCode;
        //    rcModel.FundType = PMGSYSession.Current.FundType == null ? frmCollect["FundType"] : PMGSYSession.Current.FundType;
        //    //rcModel.LevelID = PMGSYSession.Current.LevelId;
        //    rcModel.LevelID = levelId;
        //    rcModel.Month = Convert.ToInt16(frmCollect["Month"]);
        //    rcModel.Year = Convert.ToInt16(frmCollect["Year"]);

        //    rcModel.Remark = frmCollect["Remark"];

        //    String Status = objRevokeClosingBAL.RevokeClosing(rcModel);
        //    if (Status == "")
        //    {
        //        RevokeClosingModel revokeClosingModel = new RevokeClosingModel();
        //        revokeClosingModel.Month = Convert.ToInt16(DateTime.Now.Month);
        //        revokeClosingModel.Year = Convert.ToInt16(DateTime.Now.Year);
        //        ViewBag.ddlMonth = commomFuncObj.PopulateMonths(DateTime.Now.Month);
        //        ViewBag.ddlYear = commomFuncObj.PopulateYears(DateTime.Now.Year);
        //        //revokeClosingModel.AdminNdCode = PMGSYSession.Current.AdminNdCode;
        //        revokeClosingModel.AdminNdCode = AdminNdCode;
        //        revokeClosingModel.FundType = PMGSYSession.Current.FundType == null ? frmCollect["FundType"] : PMGSYSession.Current.FundType;
        //        //revokeClosingModel.LevelID = PMGSYSession.Current.LevelId;
        //        revokeClosingModel.LevelID = levelId;
        //        revokeClosingModel.MonthClosed = objRevokeClosingBAL.GetLastMonthClosed(revokeClosingModel.AdminNdCode, revokeClosingModel.FundType, revokeClosingModel.LevelID);


        //        //Added Abhishek kamble for monthly cloasing of DPIU at SRRDA level 28-Aug-2014
        //        if (PMGSYSession.Current.LevelId == 4 || PMGSYSession.Current.LevelId == 6)//accmord
        //        {

        //            //populate SRRDA
        //            if (PMGSYSession.Current.LevelId == 6)
        //            {
        //                List<SelectListItem> lstSRRDA = new List<SelectListItem>();
        //                lstSRRDA = commomFuncObj.PopulateNodalAgencies();
        //                // ViewBag.SRRDA = lstSRRDA;
        //                revokeClosingModel.SRRDA_LIST = lstSRRDA;
        //            }

        //            // revokeClosingModel.DPIU_LIST = commomFuncObj.PopulateDPIUOfSRRDA(PMGSYSession.Current.AdminNdCode);
        //            List<SelectListItem> lstMonths = commomFuncObj.PopulateDPIUOfSRRDA(SRRDAAdminNdCode);
        //            lstMonths.Insert(0, new SelectListItem { Text = "Select DPIU", Value = "0" });
        //            revokeClosingModel.DPIU_LIST = lstMonths;

        //            if (frmCollect["OwnDPIUFlag"] == "D")
        //            {
        //                revokeClosingModel.DPIU_CODE = AdminNdCode;
        //                revokeClosingModel.OwnDPIUFlag = "D";
        //            }
        //            else
        //            {
        //                revokeClosingModel.OwnDPIUFlag = "O";
        //            }

        //            if (PMGSYSession.Current.FundType == null)
        //            {
        //                List<SelectListItem> lstFund = commomFuncObj.PopulateFundTypes(false);
        //                lstFund.Insert(0, new SelectListItem { Text = "Select Fund", Value = "0" });
        //                ViewBag.ddlFund = lstFund;
        //            }
        //        }

        //        return View(revokeClosingModel);
        //    }
        //    else
        //    {
        //        return this.Json(new { success = false, message = Status });
        //    }
        //} 
        #endregion

        //Revoke Closing 
        [HttpPost]
        public JsonResult RevokeClosing(RevokeClosingModel model)
        {

            if (PMGSYSession.Current.LevelId == 4)
            {
                if (model.OwnDPIUFlag.Equals("O"))
                {
                    return Json(new { success = false, message = "User does not have permission to revoke month of SRRDA"});
                }

                DateTime currentDate = DateTime.Now.Date;
                int currentDay = currentDate.Day;
                int currentMonth = currentDate.Month;
                int currentYear = currentDate.Year;

                int prevMonth ; 
                int prevYear ;

                //currentDay = 10;
                
                if (currentMonth == 1)
                {
                    prevMonth =  12 ;
                    prevYear =  currentYear - 1;
                }
                else
                {
                    prevMonth = currentMonth - 1;
                    prevYear = currentYear ;
                }
                ///Changes by SAMMED A. PATIL on 28FEB2018 for provision to revoke current month
                #region
                //if (!((model.StartMonth + (model.StartYear * 12)) == (prevMonth + (prevYear * 12))))
                //{
                //    return Json(new { success = false, message = "SRRDA can revoke only previous month  " });
                //}
                //else
                //{
                //    if (currentDay > 10)
                //    {
                //        return Json(new { success = false, message = "SRRDA can revoke accounts only upto 10th day of a month" });
                //    }
                //}
                #endregion
            }



            
            String status = objRevokeClosingBAL.RevokeClosing(model);
            if (status.Equals("Success") && model.durationFlag.Equals("M"))
            {
                return Json(new { success = true , message = "Account Revoked successfully for selected Month." });
            }
            else
            {
                if(status.Equals("Success") && model.durationFlag.Equals("P"))
	            {
                    return Json(new { success = true, message = "Account Revoked successfully for selected period." });
	            }
                else
                {
                    return Json(new { success = false, message = status });
                }
            }
            
            
        }

        /// <summary>
        /// To get month closed details. added By Abhishek kamble 11Sep2014
        /// </summary>
        /// <param name="adminNdCode"></param>
        /// <param name="levelID"></param>
        /// <returns></returns>
        [HttpPost]
        [Audit]
        public JsonResult GetMonthClosedDetails(int? adminNdCodeDPIU, string OwnDPIUFlag, String FundType = null, string SRRDANdCode="0")
        {
            try
            {
                short levelID = 0;
                int? AdminNDCode = 0;

                if (PMGSYSession.Current.LevelId == 4 || PMGSYSession.Current.LevelId==6)//21- accmorduser
                {
                    if (!String.IsNullOrEmpty(OwnDPIUFlag))
                    {
                        if (OwnDPIUFlag == "O")
                        {

                            if (PMGSYSession.Current.LevelId == 4|| PMGSYSession.Current.LevelId == 6)
                            {
                                levelID =4;
                                AdminNDCode = Convert.ToInt32(SRRDANdCode);
                            }
                            else 
                            {
                                levelID = PMGSYSession.Current.LevelId;
                                AdminNDCode = PMGSYSession.Current.AdminNdCode;
                            }
                        }
                        else if (OwnDPIUFlag == "D")
                        { 
                            levelID = 5;
                            if (AdminNDCode == null)
                            {
                                return Json(new { status = false, message = "Please select DPIU" });                                                                           
                            }
                            else
                            {
                                AdminNDCode = adminNdCodeDPIU;
                            }
                        }
                    }
                    else {
                        return Json(new { status = false, message = "Please select Own / DPIU" });                                                                           
                    }

                }
                else {
                    levelID = PMGSYSession.Current.LevelId;
                    AdminNDCode = PMGSYSession.Current.AdminNdCode;
                }


                String closedMonth = objRevokeClosingBAL.GetLastMonthClosed(AdminNDCode.Value, (PMGSYSession.Current.FundType == null ? FundType : PMGSYSession.Current.FundType), levelID);
                return Json(new { status = true, message = closedMonth });                                                       
            }
            catch (Exception)
            {     
                return Json(new { status = false, message = "An error occured while getting month closed details." });                                                       
            }
        }

        //Populate DPIU
        [Audit]
        public JsonResult PopulateDPIU(string id)
        {
            try
            {
                PMGSY.Models.Common.TransactionParams objParam = new PMGSY.Models.Common.TransactionParams();
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

                    return Json(new SelectList(lstDPIU, "Value", "Text"), JsonRequestBehavior.AllowGet);
                }
            }
            catch
            {
                return Json(false);
            }
        }

        #region Finalize BalanceSheet

        [Audit]
        public ActionResult FinalizeBalanceSheet()
        {
            FinalizeBalanceSheetModel model = new FinalizeBalanceSheetModel();
            int? ObYear = objRevokeClosingBAL.GetOpeningBalanceYear();

            if (ObYear != null)
            {
                //ViewBag.lstYear = commomFuncObj.PopulateFinancialYear(true, false);                
                ViewBag.lstYear = objRevokeClosingBAL.PopulateFinancialYear(ObYear.Value);
            }
            else {
                List<SelectListItem> lstYear=new List<SelectListItem>();
                lstYear.Add(new SelectListItem{Text="Select Year",Value="0"});
                ViewBag.lstYear = new SelectList(lstYear, "Value", "Text");
            }

            model.FinalizedYear = GetFinalizedBalanceSheetDetails();
            //Set Last Finalized Balance sheet Details.

            return View(model);        
        }

        [HttpPost]
        [Audit]
        public ActionResult FinalizeBalanceSheet(FinalizeBalanceSheetModel model)
        {
            string message = String.Empty;

            if (ModelState.IsValid)
            {

                if (objRevokeClosingBAL.FinalizeBalanceSheet(model, ref message))
                {
                    return Json(new { status = true, message = "Balance Sheet Finalize Successfully." });
                }
                else
                {
                    return Json(new { status = true, message=message });
                }
            }
            else {
                return Json(new { status = false, message = "Please select Year." });
            }
        }


        public String GetFinalizedBalanceSheetDetails()
        {
           return objRevokeClosingBAL.GetFinalizedBalanceSheetDetails(); 
        }

        #endregion Finalize BalanceSheet


        #region Definalize BalanceSheet

        /// <summary>
        /// Return Definalize Balance Sheet View
        /// </summary>
        /// <returns></returns>
        public ActionResult DefinalizeBalanceSheetView()
        {               
            DefinalizeBalanceSheetModel definalizeBalSheetModel = new DefinalizeBalanceSheetModel();            

            //populate Agency.
            definalizeBalSheetModel.lstAgency = commomFuncObj.PopulateNodalAgencies();
            
            //populate Fund Type
            List<SelectListItem> lstFundType = commomFuncObj.PopulateFundTypes(false);
            lstFundType.Insert(0,new SelectListItem{Text="Select Fund Type",Value="0"});
            definalizeBalSheetModel.lstFundType = lstFundType;
         
            //populate Year 
            //definalizeBalSheetModel.lstYear = commomFuncObj.PopulateYears();
            List<SelectListItem> lstYear=new List<SelectListItem>();
            lstYear.Insert(0, new SelectListItem { Text="Select Year",Value="0"});
            definalizeBalSheetModel.lstYear = lstYear;

            return View(definalizeBalSheetModel);
        }


        public ActionResult DefinalizeBalanceSheet(DefinalizeBalanceSheetModel model)
        {
            //String message = String.Empty;
            try
            {

                if (ModelState.IsValid)
                {

                    if (objRevokeClosingBAL.DefinalizeBalanceSheet(model))
                    {
                        return Json(new { status = true, message = "Balance Sheet is Definalize Successfully." });                    
                    }
                    else {
                        return Json(new { status = false, message = "Balance Sheet is not Definalize." });                    
                    }                    
                }
                else {
                    return Json(new { status = false, message = "Balance Sheet is not Definalize." });
                }
                
            }
            catch (Exception)
            {
                return Json(new { status = false, message = "Balance Sheet is not Definalize." });
            }
        }

        [HttpPost]
        public ActionResult GetDefinalizeBalSheetYear()
        {
            try
            {    
                int adminNdCode=Convert.ToInt32(Request.Params["AdminNdCode"]);
                String FundType = Request.Params["FundType"];      

                return Json(objRevokeClosingBAL.GetDefinalizeBalSheetYear(adminNdCode,FundType)); 
            }
            catch (Exception)
            {
                return null;
            }
        }

        #endregion Definalize BalanceSheet

    }
}
