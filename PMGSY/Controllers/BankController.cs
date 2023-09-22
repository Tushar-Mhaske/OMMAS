using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PMGSY.Models.Bank;
using PMGSY.Common;
using PMGSY.Models.Common;
using PMGSY.Extensions;
using PMGSY.Models.Receipts;
using PMGSY.BAL.Bank;
using PMGSY.DAL.Bank;
using System.Text.RegularExpressions;
using PMGSY.Models;
namespace PMGSY.Controllers
{
    [RequiredAuthentication]
    [RequiredAuthorization]
    public class BankController : Controller
    {
        private CommonFunctions commonFuncObj = null;
        private IBankBAL BankBALObj = null;
        private BankDAL BankDALObj = null;
        private string message = string.Empty;
        [Audit]
        public ActionResult BankReconciliation()
        {
            commonFuncObj = new CommonFunctions();
            ViewBag.ddlMonth = commonFuncObj.PopulateMonths(DateTime.Now.Month);
            ViewBag.ddlYear = commonFuncObj.PopulateYears(DateTime.Now.Year);
            TransactionParams objMaster = new TransactionParams();
            objMaster.ADMIN_ND_CODE = PMGSYSession.Current.AdminNdCode;
            objMaster.DISTRICT_CODE = 0;
            ViewBag.ddlDPIU = commonFuncObj.PopulateDPIU(objMaster);

            //Added By Abhishek kamble to populate DPIU for date wise.
            List<SelectListItem> lstDateWiseDPIU = commonFuncObj.PopulateDPIU(objMaster);
            lstDateWiseDPIU.RemoveAt(0);
            lstDateWiseDPIU.Insert(0, new SelectListItem { Text = "All DPIU", Value = "0" });
            ViewBag.ddlDateWiseDPIU = lstDateWiseDPIU;

            return View(new BankReconciliationModel());
        }
        [Audit]
        public JsonResult BankReconciliationList(Int32? page, Int32? rows, String sidx, String sord)
        {
            string chequeEpay = string.Empty;
            //Adde By Abhishek kamble 30-Apr-2014 start  
            using (CommonFunctions commonFunction = new CommonFunctions())
            {
                if (!commonFunction.ValidateGridParameters(new GridParams(page, rows, sidx, sord, Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                {
                    return null;
                }
            }

            //Adde By Abhishek kamble 30-Apr-2014 end


            String SRRDADpiu = Request.Params["SRRDADpiu"];

            BankBALObj = new BankBAL();
            BankFilterModel objFilter = new BankFilterModel();
            objFilter.AdminNdCode = SRRDADpiu.Equals("S") ? PMGSYSession.Current.AdminNdCode : Convert.ToInt32(Request.Params["DPIU"]);
            objFilter.Month = Convert.ToInt16(Request.Params["Month"]);
            objFilter.Year = Convert.ToInt16(Request.Params["Year"]);
            objFilter.page = Convert.ToInt32(page) - 1;
            objFilter.rows = Convert.ToInt32(rows);
            objFilter.sidx = sidx;
            objFilter.sord = sord;
            objFilter.FundType = PMGSYSession.Current.FundType;
            long totalRecords = 0;

            //Adde By Abhishek kamble 17 sep 2014
            objFilter.MonthDateWise = Request.Params["MonthDateWise"];
            objFilter.SearchBillDate = Request.Params["SearchBillDate"];

            chequeEpay = Request.Params["ChequeEpay"];

            objFilter.LevelID = SRRDADpiu.Equals("S") ? PMGSYSession.Current.LevelId : 5;

            var jsonData = (dynamic)null;
            if (string.IsNullOrEmpty(chequeEpay))
            {
                jsonData = new
                {
                    rows = BankBALObj.BankReconciliationList(objFilter, out totalRecords),
                    //total = totalRecords <= objFilter.rows ? 1 : totalRecords / objFilter.rows + 1,
                    //added by abhishek kamble 21-8-2013 total=0
                    total = 0,
                    page = objFilter.page + 1,
                    records = totalRecords
                };
            }
            else
            {
                BankDALObj = new BankDAL();
                jsonData = new
                {
                    rows = BankDALObj.BankReconciliationPFMSList(objFilter, out totalRecords),
                    //total = totalRecords <= objFilter.rows ? 1 : totalRecords / objFilter.rows + 1,
                    //added by abhishek kamble 21-8-2013 total=0
                    total = 0,
                    page = objFilter.page + 1,
                    records = totalRecords
                };
            }
            return Json(jsonData);
        }

        [HttpPost]
        [Audit]
        //  [ValidateInput(false)]
        public ActionResult postGridData()
        {
            try
            {
                BankBALObj = new BankBAL();
                String result = String.Empty;
                bool status = true;
                BankFilterModel objParam = new BankFilterModel();
                string szRemoteAddr = Request.UserHostAddress;
                string szXForwardedFor = Request.ServerVariables["X_FORWARDED_FOR"];
                objParam.FundType = PMGSYSession.Current.FundType;
                objParam.Month = Convert.ToInt16(Request.Params["month"]);
                objParam.Year = Convert.ToInt16(Request.Params["year"]);
                objParam.AdminNdCode = Request.Params["SRRDADpiu"].Equals("S") ? PMGSYSession.Current.AdminNdCode : Convert.ToInt32(Request.Params["dpiu"]);//Modified By Abhishek kamble for SRRDA cheque Rec.

                //Adde By Abhishek kamble 17 sep 2014
                objParam.MonthDateWise = Request.Params["MonthDateWise"];
                objParam.SearchBillDate = Request.Params["SearchBillDate"];

                objParam.LevelID = Request.Params["SRRDADpiu"].Equals("S") ? PMGSYSession.Current.LevelId : 5;

                if (Request.Params["jqGridHeaderDate"] == null || Request.Params["jqGridHeaderRemark"] == null || Request.Params["jqGridHeaderReconcile"] == null)
                {
                    message = "Error occurred while processing your request.";
                    return Json(new { success = false, message = message }, JsonRequestBehavior.AllowGet);
                }

                if (szXForwardedFor == null)
                {
                    objParam.ClientIP = szRemoteAddr == "::1" ? "127.0.0.1" : szRemoteAddr;
                }

                System.Web.Script.Serialization.JavaScriptSerializer js = new System.Web.Script.Serialization.JavaScriptSerializer();

                if (Request.Params["jqGridData"] != null && Request.Params["jqGridData"] != String.Empty)
                {
                    //string app = Request.Params["jqGridData"];

                    objParam.HeaderDate = Request.Params["jqGridHeaderDate"].ToString();
                    objParam.HeaderRemarks = Request.Params["jqGridHeaderRemark"].ToString();
                    objParam.HeaderReconcile = Request.Params["jqGridHeaderReconcile"].ToString();

                    //objParam.jqGridData = js.Deserialize<BankReconciliationModel[]>(Request.Params["jqGridData"]);                
                    objParam.jqGridData = new BankReconciliationModel[] { new BankReconciliationModel() };
                    objParam.BillID = Request.Params["jqGridData"].ToString();
                }
                else
                {
                    objParam.HeaderDate = Request.Params["jqGridHeaderDate"].ToString();
                    objParam.HeaderRemarks = Request.Params["jqGridHeaderRemark"].ToString();
                    objParam.HeaderReconcile = Request.Params["jqGridHeaderReconcile"].ToString();
                }

                result = BankBALObj.SaveBankReconciliedCheques(objParam, ref message);

                if (!string.IsNullOrEmpty(result))
                {
                    message = message == string.Empty ? string.Empty : message;
                    status = false;
                }
                else
                {
                    if (objParam.HeaderReconcile.ToLower() == "yes")
                    {
                        message = "Cheques Reconciled Successfully.";
                    }
                    else
                    {
                        message = "Cheques UnReconciled Successfully.";
                    }
                }

                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
                // return string.Empty;
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                message = "Error occurred while processing your request.";
                return Json(new { success = false, message = message }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// returns view for adding bank details
        /// </summary>
        /// <returns></returns>
        [Audit]
        public ActionResult AddBankDetails()
        {
            try
            {
                CommonFunctions objCommon = new CommonFunctions();
                BankDALObj = new BankDAL();
                BankDetailsViewModel bankModel = new BankDetailsViewModel();
                PMGSY.DAL.Master.MasterDAL masterDAL = new PMGSY.DAL.Master.MasterDAL();

                #region  new try

                //bankModel.lstBankNames = objCommon.PopulatePFMSBankNames();
                //bankModel.lstIfscCodes = new List<SelectListItem>();
                //bankModel.lstIfscCodes.Insert(0, new SelectListItem() { Text = "Select Ifsc Code", Value = "" });

                //bankModel.DISTRICT = objCommon.PopulateDistrict(PMGSYSession.Current.StateCode, false);
                //bankModel.MAST_STATE_CODE = PMGSYSession.Current.StateCode;
                //bankModel.FUND_TYPE = PMGSYSession.Current.FundType;

                //List<SelectListItem> items = new List<SelectListItem>();
                //items.Add(new SelectListItem() { Text = "Saving", Value = "S" });
                //items.Add(new SelectListItem() { Text = "Holding", Value = "H" });
                //items.Add(new SelectListItem() { Text = "Security Deposit Account (SDA)", Value = "D" });
                //items.Insert(0, (new SelectListItem { Text = "Select Account Type", Value = "A", Selected = true }));
                //bankModel.lstBankAccType = items;

                //if (PMGSYSession.Current.LevelId == 5)
                //{
                //    bankModel.ADMIN_ND_CODE = BankDALObj.GetParentAdminCode();
                //}
                //else
                //{
                //    bankModel.ADMIN_ND_CODE = PMGSYSession.Current.AdminNdCode;
                //}
                //bankModel.Operation = "A";
                //bankModel.OldCloseDate = BankDALObj.GetOldCloseDate();
                //@ViewBag.Status = "No active bank details are present.";
                //return PartialView("AddBankDetails", bankModel);

                #endregion

                // Commented by Srishti 
                int bankCode = 0; //BankDALObj.BankDetailsStatus();
                if (bankCode == 0)
                {
                    bankModel.lstBankNames = objCommon.PopulatePFMSBankNames();
                    bankModel.lstIfscCodes = new List<SelectListItem>();
                    bankModel.lstIfscCodes.Insert(0, new SelectListItem() { Text = "Select Ifsc Code", Value = "" });

                    bankModel.DISTRICT = objCommon.PopulateDistrict(PMGSYSession.Current.StateCode, false);
                    bankModel.MAST_STATE_CODE = PMGSYSession.Current.StateCode;
                    bankModel.FUND_TYPE = PMGSYSession.Current.FundType;

                    List<SelectListItem> items = new List<SelectListItem>();
                    items.Add(new SelectListItem() { Text = "Saving", Value = "S" });
                    items.Add(new SelectListItem() { Text = "Holding", Value = "H" });
                    items.Add(new SelectListItem() { Text = "Security Deposit Account (SDA)", Value = "D" });
                    items.Insert(0, (new SelectListItem { Text = "Select Account Type", Value = "A", Selected = true }));
                    bankModel.lstBankAccType = items;

                    if (PMGSYSession.Current.LevelId == 5)
                    {
                        bankModel.ADMIN_ND_CODE = BankDALObj.GetParentAdminCode();

                    }
                    else
                    {
                        bankModel.ADMIN_ND_CODE = PMGSYSession.Current.AdminNdCode;
                    }
                    bankModel.Operation = "A";
                    bankModel.OldCloseDate = BankDALObj.GetOldCloseDate();
                    @ViewBag.Status = "No active bank details are present.";
                    return PartialView("AddBankDetails", bankModel);
                }
                else
                {
                    bankModel = BankDALObj.GetBankDetails(bankCode);
                    bankModel.STATE = objCommon.PopulateStates();
                    bankModel.DISTRICT = objCommon.PopulateDistrict(PMGSYSession.Current.StateCode, false);
                    bankModel.MAST_STATE_CODE = PMGSYSession.Current.StateCode;
                    bankModel.FUND_TYPE = PMGSYSession.Current.FundType;

                    List<SelectListItem> items = new List<SelectListItem>();
                    items.Add(new SelectListItem() { Text = "Saving", Value = "S" });
                    items.Add(new SelectListItem() { Text = "Holding", Value = "H" });
                    items.Add(new SelectListItem() { Text = "Security Deposit Account (SDA)", Value = "D" });
                    items.Insert(0, (new SelectListItem { Text = "Select Account Type", Value = "A", Selected = true }));
                    bankModel.lstBankAccType = items;

                    bankModel.lstBankNames = objCommon.PopulatePFMSBankNames();
                    bankModel.lstIfscCodes = new List<SelectListItem>();
                    bankModel.lstIfscCodes.Insert(0, new SelectListItem() { Text = "Select Ifsc Code", Value = "" });

                    if (!masterDAL.ValidatePFMSBankDetailsDAL(bankModel.BANK_NAME, bankModel.MAST_IFSC_CODE))
                    {
                        //contractorBankDetails.pfmsErrorMessage = "Either Bank Name or Ifsc Code or both details entered does not match PFMS details, please select Bank Name & Ifsc Code as per PFMS";
                        bankModel.pfmsErrorMessage = "Bank/IFSC Code is not entered as per Master Data of Bank/IFSC Code, please select correct Bank/IFSC Code from given dropdowns.";
                    }

                    if (PMGSYSession.Current.LevelId == 5)
                    {
                        bankModel.ADMIN_ND_CODE = BankDALObj.GetParentAdminCode();
                    }
                    else
                    {
                        bankModel.ADMIN_ND_CODE = PMGSYSession.Current.AdminNdCode;
                    }
                    bankModel.OldCloseDate = BankDALObj.GetOldCloseDate();
                    bankModel.Operation = "E";
                    @ViewBag.Status = "";
                    return PartialView("AddBankDetails", bankModel);
                }
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return Json(new { success = false, message = "Error occurred while processing your request." });
            }
        }

        // Added new by Srishti
        [Audit]
        public ActionResult EditBankDetails(string idtemp)
        {
            try
            {
                CommonFunctions objCommon = new CommonFunctions();
                BankDALObj = new BankDAL();
                BankDetailsViewModel bankModel = new BankDetailsViewModel();
                PMGSY.DAL.Master.MasterDAL masterDAL = new PMGSY.DAL.Master.MasterDAL();
                Dictionary<string, string> decryptedParameters = null;

                #region  new try
                int bankCode = 0;

                if (idtemp != null)
                {
                    string[] encParam = idtemp.Split('/');
                    decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { encParam[0], encParam[1], encParam[2] });
                    if (decryptedParameters.Count > 0)
                    {
                        bankCode = Convert.ToInt32(decryptedParameters["BankCode"]);
                    }
                }

                //int BankCode = BankDALObj.BankDetailsStatus();

                bankModel = BankDALObj.GetBankDetails(bankCode);
                bankModel.STATE = objCommon.PopulateStates();
                bankModel.DISTRICT = objCommon.PopulateDistrict(PMGSYSession.Current.StateCode, false);
                bankModel.MAST_STATE_CODE = PMGSYSession.Current.StateCode;
                bankModel.FUND_TYPE = PMGSYSession.Current.FundType;

                bankModel.lstBankNames = objCommon.PopulatePFMSBankNames();
                bankModel.lstIfscCodes = new List<SelectListItem>();
                bankModel.lstIfscCodes.Insert(0, new SelectListItem() { Text = "Select Ifsc Code", Value = "" });

                //Added by Srishti 
                List<SelectListItem> items = new List<SelectListItem>();
                items.Add(new SelectListItem() { Text = "Saving", Value = "S" });
                items.Add(new SelectListItem() { Text = "Holding", Value = "H" });
                items.Add(new SelectListItem() { Text = "Security Deposit Account (SDA)", Value = "D" });
                items.Insert(0, (new SelectListItem { Text = "Select Account Type", Value = "A", Selected = true }));
                bankModel.lstBankAccType = items;

                if (!masterDAL.ValidatePFMSBankDetailsDAL(bankModel.BANK_NAME, bankModel.MAST_IFSC_CODE))
                {
                    bankModel.pfmsErrorMessage = "Bank/IFSC Code is not entered as per Master Data of Bank/IFSC Code, please select correct Bank/IFSC Code from given dropdowns.";
                }

                if (PMGSYSession.Current.LevelId == 5)
                {
                    bankModel.ADMIN_ND_CODE = BankDALObj.GetParentAdminCode();
                }
                else
                {
                    bankModel.ADMIN_ND_CODE = PMGSYSession.Current.AdminNdCode;
                }
                bankModel.OldCloseDate = BankDALObj.GetOldCloseDate();
                bankModel.Operation = "E";
                @ViewBag.Status = "";
                return PartialView("AddBankDetails", bankModel);
                #endregion

            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return Json(new { success = false, message = "Error occurred while processing your request." });
            }
        }

        /// <summary>
        /// list view of bank details 
        /// </summary>
        /// <returns></returns>
        [Audit]
        public ActionResult ListBankDetails()
        {
            try
            {
                return View("ListBankDetails");
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return null;
            }
        }

        /// <summary>
        /// save the Bank details information
        /// </summary>
        /// <param name="bankModel">model containing bank details</param>
        /// <returns></returns>
        [HttpPost]
        [Audit]
        [ValidateAntiForgeryToken]
        public ActionResult AddBankDetails(BankDetailsViewModel bankModel)
        {
            BankBALObj = new BankBAL();
            string message = string.Empty;
            CommonFunctions objCommon = new CommonFunctions();
            PMGSY.DAL.Master.MasterDAL masterDAL = new PMGSY.DAL.Master.MasterDAL();
            try
            {
                if (ModelState.IsValid)
                {
                    if (!masterDAL.ValidatePFMSBankDetailsDAL(bankModel.BANK_NAME, bankModel.MAST_IFSC_CODE))
                    {
                        return Json(new { success = false, message = "Invalid Ifsc Code entered." }, JsonRequestBehavior.AllowGet);
                    }
                    if ((BankBALObj.AddBankDetails(bankModel, ref message)))
                    {
                        return Json(new { success = true, message = message });
                    }
                    else
                    {
                        return Json(new { success = false, message = (message == string.Empty ? "Bank Details not added successfully." : message) });
                    }
                }
                else
                {
                    //bankModel.DISTRICT = objCommon.PopulateDistrict(PMGSYSession.Current.StateCode, false);
                    //return PartialView("AddBankDetails", bankModel);

                    string ermessage = ModelState.Values.Where(x => x.Errors.Count == 1).FirstOrDefault().Errors[0].ErrorMessage;
                    return Json(new { success = false, message = ermessage });
                }
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);

                return Json(new { success = false, message = "Error occurred while processing your request." });
            }
        }

        /// <summary>
        /// updates the bank details information
        /// </summary>
        /// <param name="bankModel">model containing updated Bank details information</param>
        /// <returns></returns>
        [HttpPost]
        [Audit]
        [ValidateAntiForgeryToken]
        public ActionResult EditBankDetails(BankDetailsViewModel bankModel)
        {
            BankBALObj = new BankBAL();
            CommonFunctions objCommon = new CommonFunctions();
            string message = string.Empty;
            PMGSY.DAL.Master.MasterDAL masterDAL = new PMGSY.DAL.Master.MasterDAL();
            try
            {
                if (ModelState.IsValid)
                {
                    if (!masterDAL.ValidatePFMSBankDetailsDAL(bankModel.BANK_NAME, bankModel.MAST_IFSC_CODE))
                    {
                        return Json(new { success = false, message = "Invalid Ifsc Code entered." }, JsonRequestBehavior.AllowGet);
                    }
                    if (BankBALObj.EditBankDetails(bankModel, ref message))
                    {
                        return Json(new { success = true, message = message });
                    }
                    else
                    {
                        return Json(new { success = false, message = (message == string.Empty ? "Bank details not updated successfully." : message) });
                    }
                }
                else
                {
                    bankModel.DISTRICT = objCommon.PopulateDistrict(PMGSYSession.Current.StateCode, false);
                    bankModel.lstBankNames = objCommon.PopulatePFMSBankNames();
                    //bankModel.lstIfscCodes = new List<SelectListItem>();
                    //bankModel.lstIfscCodes.Insert(0, new SelectListItem() { Text = "Select Ifsc Code", Value = "" });
                    return PartialView("AddBankDetails", bankModel);
                    //return Json(new { success = false, message = new CommonFunctions().FormatErrorMessage(ModelState) });
                }
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return Json(new { success = false, message = "Error occurred while processing the request." });
            }
        }

        /// <summary>
        /// returns the list of bank details
        /// </summary>
        /// <param name="page">no. of pages</param>
        /// <param name="rows">total rows</param>
        /// <param name="sidx">sort column name</param>
        /// <param name="sord">sort order</param>
        /// <returns></returns>
        [Audit]
        public ActionResult GetBankDetailsList(int? page, int? rows, string sidx, string sord)
        {
            BankBALObj = new BankBAL();
            long totalRecords = 0;
            int stateCode = 0;
            try
            {
                using (CommonFunctions commonFunction = new CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new GridParams(page, rows, sidx, sord, Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                    {
                        return null;
                    }
                }

                stateCode = PMGSYSession.Current.StateCode;

                var jsonData = new
                {
                    rows = BankBALObj.GetBankDetailsList(page - 1, Convert.ToInt32(rows), sidx, sord, out totalRecords, stateCode),
                    total = totalRecords <= Convert.ToInt32(rows) ? 1 : totalRecords / Convert.ToInt32(rows) + 1,
                    page = Convert.ToInt32(page),
                    records = totalRecords,
                };
                return Json(jsonData, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return null;
            }
        }

        [Audit]
        public ActionResult ShowBankDetails(String parameter, String hash, String key)
        {
            Dictionary<string, string> decryptedParameters = null;
            String[] encryptedParameters = null;
            BankDALObj = new BankDAL();
            try
            {
                decryptedParameters = URLEncrypt.DecryptParameters1(new string[] { parameter, hash, key });
                int bankCode = Convert.ToInt32(decryptedParameters["BankCode"]);
                BankDetailsViewModel bankModel = BankDALObj.GetBankDetails(bankCode);
                // Added by Srishti
                @ViewBag.AccountType = bankModel.BANK_ACC_TYPE == "S" ? "Saving" : bankModel.BANK_ACC_TYPE == "H" ? "Holding" : bankModel.BANK_ACC_TYPE == "D" ? "Security Deposit Account" : "--";
                bankModel.Operation = "V";
                @ViewBag.Status = "";
                return PartialView("AddBankDetails", bankModel);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return Json(new { success = false, message = "Error occurred while processing your request." });
            }
        }

        [Audit]
        public ActionResult DisplayDPIUDetails(int? page, int? rows, string sidx, string sord)
        {
            BankBALObj = new BankBAL();
            long totalRecords = 0;
            int adminCode = 0;
            try
            {
                using (CommonFunctions commonFunction = new CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new GridParams(page, rows, sidx, sord, Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                    {
                        return null;
                    }
                }

                adminCode = PMGSYSession.Current.AdminNdCode;

                var jsonData = new
                {
                    rows = BankBALObj.DisplayDPIUStatusList(page - 1, Convert.ToInt32(rows), sidx, sord, out totalRecords, adminCode),
                    total = totalRecords <= Convert.ToInt32(rows) ? 1 : totalRecords / Convert.ToInt32(rows) + 1,
                    page = Convert.ToInt32(page),
                    records = totalRecords,
                };
                return Json(jsonData, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return null;
            }
        }
        [Audit]
        public ActionResult DisplayDPIUStatusList()
        {
            AdminDepartmentViewModel adminModel = new AdminDepartmentViewModel();
            //adminModel.ADMIN_ND_CODE = PMGSYSession.Current.AdminNdCode;
            return View("DisplayDPIUStatusList", adminModel);
        }

        [HttpPost]
        [Audit]
        public JsonResult UpdateDPIUDetails(FormCollection formCollection)
        {
            try
            {
                BankBALObj = new BankBAL();
                commonFuncObj = new CommonFunctions();
                string message = string.Empty;
                int adminCode = Convert.ToInt32(formCollection["id"]);
                AdminDepartmentViewModel adminModel = new AdminDepartmentViewModel();
                Regex regex = new Regex(@"^(((0[1-9]|[12]\d|3[01])\/(0[13578]|1[02])\/((19|[2-9]\d)\d{2}))|((0[1-9]|[12]\d|30)\/(0[13456789]|1[012])\/((19|[2-9]\d)\d{2}))|((0[1-9]|1\d|2[0-8])\/02\/((19|[2-9]\d)\d{2}))|(29\/02\/((1[6-9]|[2-9]\d)(0[48]|[2468][048]|[13579][26])|((16|[2468][048]|[3579][26])00))))$");
                adminModel.ADMIN_ND_CODE = adminCode;
                var data = (formCollection["ADMIN_EPAY_ENABLE_DATE"]);

                if ((formCollection["ADMIN_EPAY_ENABLE_DATE"]) != null && (formCollection["ADMIN_EPAY_ENABLE_DATE"]) != string.Empty)
                {
                    if (regex.IsMatch(formCollection["ADMIN_EPAY_ENABLE_DATE"]))
                    {
                        adminModel.ADMIN_EPAY_ENABLE_DATE = formCollection["ADMIN_EPAY_ENABLE_DATE"];
                        if (commonFuncObj.GetStringToDateTime(adminModel.ADMIN_EPAY_ENABLE_DATE) > System.DateTime.Now)
                        {
                            return Json("Date should not be greater than todays date.");
                        }
                    }
                    else
                    {
                        return Json("Invalid Date, Please enter date in dd/mm/yyyy format.");
                    }
                }

                if ((formCollection["ADMIN_BANK_AUTH_ENABLED"]) != null && (formCollection["ADMIN_BANK_AUTH_ENABLED"]) != string.Empty)
                {
                    adminModel.ADMIN_BANK_AUTH_ENABLED = formCollection["ADMIN_BANK_AUTH_ENABLED"];
                }

                if ((formCollection["ADMIN_BA_ENABLE_DATE"]) != null && (formCollection["ADMIN_BA_ENABLE_DATE"]) != string.Empty)
                {
                    if (regex.IsMatch(formCollection["ADMIN_BA_ENABLE_DATE"]) || formCollection["ADMIN_BA_ENABLE_DATE"] == string.Empty)
                    {
                        adminModel.ADMIN_BA_ENABLE_DATE = formCollection["ADMIN_BA_ENABLE_DATE"];
                        if (commonFuncObj.GetStringToDateTime(adminModel.ADMIN_BA_ENABLE_DATE) > System.DateTime.Now)
                        {
                            return Json("Date should not be greater than todays date.");
                        }
                    }
                    else
                    {
                        return Json("Invalid Date, Please enter date in dd/mm/yyyy format.");
                    }
                }
                else
                {
                    adminModel.ADMIN_BANK_AUTH_ENABLED = "N";
                }
                if ((formCollection["ADMIN_EREMIT_ENABLED_DATE"]) != null && (formCollection["ADMIN_EREMIT_ENABLED_DATE"]) != string.Empty)
                {

                    if (regex.IsMatch(formCollection["ADMIN_EREMIT_ENABLED_DATE"]) || formCollection["ADMIN_EREMIT_ENABLED_DATE"] == string.Empty)
                    {
                        adminModel.ADMIN_EREMIT_ENABLED_DATE = formCollection["ADMIN_EREMIT_ENABLED_DATE"];
                        if (commonFuncObj.GetStringToDateTime(adminModel.ADMIN_EREMIT_ENABLED_DATE) > System.DateTime.Now)
                        {
                            return Json("Date should not be greater than todays date.");
                        }
                    }
                    else
                    {
                        return Json("Invalid Date, Please enter date in dd/mm/yyyy format.");
                    }
                }

                bool status = BankBALObj.UpdateDPIUDetailsBAL(adminModel, ref message);

                if (status)
                    return Json(true);
                else
                    return Json(message);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return Json("Error occurred while processing your request.");
            }

        }

        #region AUTHORIZE_SIGNATORY_KEY

        /// <summary>
        /// returns the view for the list of Authorized signatory
        /// </summary>
        /// <returns></returns>
        public ActionResult ViewAuthSignatory()
        {
            return View();
        }

        /// <summary>
        /// returns the list of Active Authorized signatory of particular state
        /// </summary>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <param name="sidx"></param>
        /// <param name="sord"></param>
        /// <returns></returns>
        [Audit]
        public ActionResult ListAuthorizedSignatories(int? page, int? rows, string sidx, string sord)
        {
            try
            {
                long totalRecords = 0;
                BankBALObj = new BankBAL();
                using (CommonFunctions commonFunction = new CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new GridParams(page, rows, sidx, sord, Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                    {
                        return null;
                    }
                }

                var jsonData = new
                {
                    rows = BankBALObj.DisplayAuthSignatoryList(page - 1, Convert.ToInt32(rows), sidx, sord, out totalRecords),
                    total = totalRecords,
                    page = page == null ? 0 : Convert.ToInt32(page),
                    records = totalRecords,
                };
                return Json(jsonData, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return Json(null);
            }
        }

        //Added By Abhishek kamble 26-Feb-2014

        /// <summary>
        /// Generates Random Epayment password for authorised signatory.
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GenerateKey()
        {
            try
            {
                BankBALObj = new BankBAL();

                string key = BankBALObj.GenerateKey();
                if (key == null)
                {
                    return Json(new { success = false, message = "An Error occured while proccessing your request." }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { success = true, key = key }, JsonRequestBehavior.AllowGet);
                }

            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return Json(new { success = false, message = "An Error occured while proccessing your request." }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// Save Authorised signatory Epayment Password and send an email to it
        /// </summary>
        /// <param name="formCollection"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult SaveAuthSigKey(FormCollection formCollection)
        {
            try
            {
                string sendEmailTo = String.Empty;
                int AdminNoOfficerCode = 0;
                string AuthSigKey = string.Empty;

                BankBALObj = new BankBAL();
                if (!(string.IsNullOrEmpty(formCollection["hdnAdminNoOfficerCode"])))
                {
                    String[] encryptedParameters = null;
                    Dictionary<string, string> decryptedParameters = null;
                    encryptedParameters = formCollection["hdnAdminNoOfficerCode"].Split('/');

                    if (encryptedParameters.Length == 3)
                    {
                        decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { encryptedParameters[0], encryptedParameters[1], encryptedParameters[2] });

                        AdminNoOfficerCode = Convert.ToInt32(decryptedParameters["ADMIN_NO_OFFICER_CODE"]);
                    }
                    else
                    {
                        return Json(new { success = false, message = "An Error occured while proccessing your request." }, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    return Json(new { success = false, message = "An Error occured while proccessing your request." }, JsonRequestBehavior.AllowGet);
                }

                if (!(string.IsNullOrEmpty(formCollection["hdnGeneratedKey"])))
                {
                    AuthSigKey = formCollection["hdnGeneratedKey"];
                }
                else
                {
                    return Json(new { success = false, message = "Please generate your key." }, JsonRequestBehavior.AllowGet);
                }

                if (BankBALObj.SaveAuthSigKey(AdminNoOfficerCode, AuthSigKey, ref sendEmailTo))
                {
                    return Json(new { success = true, message = "Generated Key is send at " + sendEmailTo }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { success = false, message = "Generated Key is not saved." }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return Json(new { success = false, message = "An Error occured while proccessing your request." }, JsonRequestBehavior.AllowGet);
            }
        }

        #endregion  AUTHORIZE_SIGNATORY_KEY

        #region SRRDA_PDF_KEY

        /// <summary>
        /// Display Pdf key generate form
        /// </summary>
        /// <returns></returns>
        public ActionResult GenerateSRRDAPdfKeyForm()
        {
            SRRDAPdfKeyViewModel SrrdaPdfKeyModel = new SRRDAPdfKeyViewModel();
            ADMIN_DEPARTMENT adminDeptModel = null;
            ACC_EPAY_MAIL_OTHER mailOtherModel = null;
            BankBALObj = new BankBAL();
            try
            {
                adminDeptModel = BankBALObj.GetDepartmentDetails();
                mailOtherModel = BankBALObj.GetEpayMailOtherDetails();

                if (mailOtherModel != null)
                {
                    SrrdaPdfKeyModel.EncryptedMailID = URLEncrypt.EncryptParameters1(new string[] { "MAIL_ID=" + mailOtherModel.MAIL_ID.ToString() });
                }
                SrrdaPdfKeyModel.EMAIL_CC = mailOtherModel == null ? adminDeptModel.ADMIN_ND_EMAIL : mailOtherModel.EMAIL_CC;

                SrrdaPdfKeyModel.PDF_OPEN_KEY = BankBALObj.GenerateKey();
                return View(SrrdaPdfKeyModel);
            }
            catch (Exception)
            {
                return View(SrrdaPdfKeyModel);
            }
        }

        /// <summary>
        /// Save generated SRRDA pdf key details and send mail
        /// </summary>
        /// <param name="PdfKeymodel"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult SaveSRRDAPdfKeyDetails(SRRDAPdfKeyViewModel PdfKeymodel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    int MailID = 0;
                    string PDFOpenKey = string.Empty;
                    BankBALObj = new BankBAL();

                    if (!(string.IsNullOrEmpty(PdfKeymodel.EncryptedMailID))) //Update Key details
                    {
                        String[] encryptedParameters = null;
                        Dictionary<string, string> decryptedParameters = null;
                        encryptedParameters = PdfKeymodel.EncryptedMailID.Split('/');

                        if (encryptedParameters.Length == 3)
                        {
                            decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { encryptedParameters[0], encryptedParameters[1], encryptedParameters[2] });
                            MailID = Convert.ToInt32(decryptedParameters["MAIL_ID"]);
                        }
                        else
                        {
                            return Json(new { success = false, message = "An Error occured while proccessing your request." }, JsonRequestBehavior.AllowGet);
                        }
                    }

                    if (BankBALObj.AddEditSRRDAPDFKeyDetails(PdfKeymodel, MailID, ref message))
                    {
                        return Json(new { success = true, message = message }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json(new { success = false, message = "Generated Key is not saved." }, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    return View("GenerateSRRDAPdfKeyForm", PdfKeymodel);
                }
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return Json(new { success = false, message = "An Error occured while proccessing your request." }, JsonRequestBehavior.AllowGet);
            }

        }

        #endregion SRRDA_PDF_KEY

        #region BankPDFKeyGeneration
        
        /// <summary>
        /// Filter view for Fund and Agency Selection
        /// </summary>
        /// <returns></returns>
        public ActionResult ViewBankPDFKeyGeneration()
        {
            BankPdfKeyViewModel bankPdfKeyViewModel = new BankPdfKeyViewModel();
            return View(bankPdfKeyViewModel);
        }

        /// <summary>
        /// Display Bank details list
        /// </summary>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <param name="sidx"></param>
        /// <param name="sord"></param>
        /// <returns></returns>
        public ActionResult DisplayBankDetailList(int? page, int? rows, string sidx, string sord)
        {
            try
            {
                long totalRecords = 0;
                BankBALObj = new BankBAL();
                int agencyCode = 0;
                String FundType = String.Empty;

                if (!(String.IsNullOrEmpty(Request.Params["AgencyCode"])) && !(String.IsNullOrEmpty(Request.Params["FundType"])))
                {
                    agencyCode = Convert.ToInt32(Request.Params["AgencyCode"]);
                    FundType = Request.Params["FundType"];
                }

                using (CommonFunctions commonFunction = new CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new GridParams(page, rows, sidx, sord, Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                    {
                        return null;
                    }
                }

                var jsonData = new
                {
                    rows = BankBALObj.DisplayBankDetailList(page - 1, Convert.ToInt32(rows), sidx, sord, out totalRecords, agencyCode, FundType),
                    total = totalRecords,
                    page = page == null ? 0 : Convert.ToInt32(page),
                    records = totalRecords,
                };
                return Json(jsonData, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return Json(null);
            }
        }

        /// <summary>
        /// Save Generated bank pdf open key and send email
        /// </summary>
        /// <param name="bankPdfKeyViewModel"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult SaveBankPDFOpenKey(BankPdfKeyViewModel bankPdfKeyViewModel)
        {
            try
            {
                string sendEmailTo = String.Empty;
                int BankCode = 0;
                string BankPDFKey = string.Empty;

                BankBALObj = new BankBAL();
                if (!(string.IsNullOrEmpty(bankPdfKeyViewModel.hdnBankCode)))
                {
                    String[] encryptedParameters = null;
                    Dictionary<string, string> decryptedParameters = null;
                    encryptedParameters = bankPdfKeyViewModel.hdnBankCode.Split('/');

                    if (encryptedParameters.Length == 3)
                    {
                        decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { encryptedParameters[0], encryptedParameters[1], encryptedParameters[2] });

                        BankCode = Convert.ToInt32(decryptedParameters["BankCode"]);
                    }
                    else
                    {
                        return Json(new { success = false, message = "An Error occured while proccessing your request." }, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    return Json(new { success = false, message = "An Error occured while proccessing your request." }, JsonRequestBehavior.AllowGet);
                }

                if (!(string.IsNullOrEmpty(bankPdfKeyViewModel.hdnGeneratedKey)))
                {
                    BankPDFKey = bankPdfKeyViewModel.hdnGeneratedKey;
                }
                else
                {
                    return Json(new { success = false, message = "Please generate your key." }, JsonRequestBehavior.AllowGet);
                }

                if (BankBALObj.SaveBankPDFOpenKey(BankCode, BankPDFKey, ref sendEmailTo, bankPdfKeyViewModel.StateAgencyName,ref message))
                {
                    return Json(new { success = true, message = "Generated Key is send at " + sendEmailTo }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { success = false, message = ( message==String.Empty?"Generated Key is not saved.":message) }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return Json(new { success = false, message = "An Error occured while proccessing your request." }, JsonRequestBehavior.AllowGet);
            }
        }


        #endregion BankPDFKeyGeneration
    }
}
