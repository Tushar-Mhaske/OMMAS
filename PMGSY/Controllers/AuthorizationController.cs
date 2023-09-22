using PMGSY.Extensions;
using PMGSY.Models.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PMGSY.BAL.Authorization;
using PMGSY.Models.Common;
using PMGSY.Common;
using PMGSY.Models;
using System.Data.Entity.Validation;

namespace PMGSY.Controllers
{
    [RequiredAuthentication]
    [RequiredAuthorization]
    public partial class AuthorizationController : Controller
    {
        IAuthorizationBAL objAuthBAL = null;
        CommonFunctions objCommon = null;
        string message = string.Empty;

        public AuthorizationController()
        {
            objAuthBAL = new AuthorizationBAL();
        }
         [Audit]
        public ActionResult ListAuthRequestDetails()
        {
            ListAutorizationRequestModel listAuthRequestModel = new ListAutorizationRequestModel();
            objCommon = new CommonFunctions();
            TransactionParams objParam = new TransactionParams();
            objParam.ADMIN_ND_CODE = PMGSYSession.Current.AdminNdCode;
            ViewBag.DPIU = objCommon.PopulateDPIU(objParam);

            listAuthRequestModel.AUTH_MONTH = Convert.ToInt16( DateTime.Now.Month);
            ViewBag.ddlMonth = objCommon.PopulateMonths();
            listAuthRequestModel.AUTH_MONTH_LIST = objCommon.PopulateMonths();

            listAuthRequestModel.AUTH_YEAR = Convert.ToInt16(DateTime.Now.Year);
            ViewBag.ddlYear = objCommon.PopulateYears(true);
            listAuthRequestModel.AUTH_YEAR_LIST = objCommon.PopulateYears(true);

            return View(listAuthRequestModel);
        }      

        [HttpPost]
         [Audit]
        public ActionResult ListAuthorizationRequest(FormCollection frmCollect)
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
            AuthorizationFilter objFilter = new AuthorizationFilter();
            long totalRecords;
            objFilter.Month = Convert.ToInt16(Request.Params["month"]);
            objFilter.Year = Convert.ToInt16(Request.Params["year"]);
            objFilter.AdminNdCode = Convert.ToInt32(Request.Params["dpiu"]);
            objFilter.page = Convert.ToInt32(frmCollect["page"]) - 1;
            objFilter.rows = Convert.ToInt32(frmCollect["rows"]);
            objFilter.sidx = frmCollect["sidx"].ToString();
            objFilter.sord = frmCollect["sord"].ToString();
            objFilter.FundType = PMGSYSession.Current.FundType;
            objFilter.LevelId = PMGSYSession.Current.LevelId;

            var jsonData = new
            {
                rows = objAuthBAL.AuthorizationRequestList(objFilter, out totalRecords),
                total = totalRecords <= objFilter.rows ? 1 : totalRecords / objFilter.rows + 1,
                page = objFilter.page + 1,
                records = totalRecords
            };
            return Json(jsonData);
        }

        [HttpPost]
         [Audit]
         [ValidateAntiForgeryToken]
        public ActionResult AddRequestTrackingDetails(ListAutorizationRequestModel authRequestModel)
        {
            
            if(ModelState.IsValid)
            {
                string status = objAuthBAL.AddRequestTrackingDetails(authRequestModel);
                if (status == String.Empty)
                {
                    return this.Json(new { success = true, message = authRequestModel.AUTH_STATUS });
                }
                else
                {
                    return this.Json(new { success = false, message = status });
                }
            }
            else
            {
                return View("ListAuthRequestDetails",authRequestModel);
            }
        }
         [Audit]
        public ActionResult AddReceiptPaymentDetails(String parameter, String hash, String key)
        {
            String[] strParameters = URLEncrypt.DecryptParameters(new string[] { parameter, hash, key });
            Int64 AuthId = Convert.ToInt64(strParameters[0].Split('_')[0]);
            Int64 BillId = Convert.ToInt64(strParameters[0].Split('_')[1]);
            ACC_AUTH_REQUEST_MASTER request_master = new ACC_AUTH_REQUEST_MASTER();
            request_master = objAuthBAL.GetAuthorizationRequestMaster(AuthId);
            ViewBag.CurrentStatus = request_master.CURRENT_AUTH_STATUS;
            if (BillId == 0 && request_master.CURRENT_AUTH_STATUS != "R")
            {
                ViewBag.BillId = null;
            }
            else
            {
                ViewBag.BillId = URLEncrypt.EncryptParameters(new string[] { BillId.ToString() });
            }
            ViewBag.AuthId = URLEncrypt.EncryptParameters(new string[] { AuthId.ToString() });
            return View();
        }
         [Audit]
        public ActionResult ViewReceiptPaymentDetails(String parameter, String hash, String key)
        {
            String[] strParameters = URLEncrypt.DecryptParameters(new string[] { parameter, hash, key });
            Int64 AuthId = Convert.ToInt64(strParameters[0].Split('$')[0]);
            Int64 BillId = Convert.ToInt64(strParameters[0].Split('$')[1]);
            ACC_AUTH_REQUEST_MASTER request_master = new ACC_AUTH_REQUEST_MASTER();
            request_master = objAuthBAL.GetAuthorizationRequestMaster(AuthId);
            ViewBag.CurrentStatus = request_master.CURRENT_AUTH_STATUS;
            ViewBag.AuthId = URLEncrypt.EncryptParameters(new string[] { AuthId.ToString() });
            return View();
        }
            [Audit]
        public ActionResult AddReceiptDetails(String parameter, String hash, String key)
        {
            ReceiptDetailsModel receiptModel = new ReceiptDetailsModel();
            objCommon = new CommonFunctions();
           if(parameter != null)
            {
                String[] strParameters = URLEncrypt.DecryptParameters(new string[] { parameter, hash, key });
                Int64 BillId = Convert.ToInt64(strParameters[0].Split('$')[0]);
                ACC_BILL_MASTER acc_bill_master = new ACC_BILL_MASTER();
                acc_bill_master = objAuthBAL.GetBillMaster(BillId);
                receiptModel.BILL_NO = acc_bill_master.BILL_NO;
                receiptModel.BILL_DATE = objCommon.GetDateTimeToString(acc_bill_master.BILL_DATE);
            }
            return View(receiptModel);
        }

        [HttpPost]
        [Audit]
         [ValidateAntiForgeryToken]
        public ActionResult AddReceiptDetails(ReceiptDetailsModel receiptModel, String parameter, String hash, String key)
        {
            if (ModelState.IsValid)
            {
                String[] strParameters = URLEncrypt.DecryptParameters(new string[] { parameter, hash, key });
                receiptModel.AUTH_ID = Convert.ToInt64(strParameters[0].Split('$')[0]);
                objCommon = new CommonFunctions();

                //added by koustubh nakate on 22/07/2013 to check previous month is closed or not
                if (objCommon.GetStringToDateTime(receiptModel.BILL_DATE).Month != 0 && objCommon.GetStringToDateTime(receiptModel.BILL_DATE).Month < 13 && objCommon.GetStringToDateTime(receiptModel.BILL_DATE).Year != 0)
                {
                    string monthlyClosingStatus = string.Empty;

                    String errMessage = String.Empty;

                    monthlyClosingStatus = objCommon.MonthlyClosingValidation((Int16)objCommon.GetStringToDateTime(receiptModel.BILL_DATE).Month, (Int16)objCommon.GetStringToDateTime(receiptModel.BILL_DATE).Year, PMGSYSession.Current.FundType, PMGSYSession.Current.LevelId, PMGSYSession.Current.AdminNdCode,ref errMessage);
                    
                    if (monthlyClosingStatus.Equals("-111"))
                    {
                        ModelState.AddModelError("BILL_DATE", errMessage);
                    }
                    if (monthlyClosingStatus.Equals("-222"))
                    {
                        ModelState.AddModelError("BILL_DATE", "Month is already closed.");
                    }
                }


                String status = objAuthBAL.AddReceiptDetails(receiptModel, ref message);
                if (status != String.Empty)
                {
                    return this.Json(new { success = true, billid = URLEncrypt.EncryptParameters(new string[] { status }), message = "Receipt Details Added." });
                }
                else
                {
                    message = message == string.Empty ? "Error while processing your request." : message;

                    return this.Json(new { success = false, message = message });
                }

            }
            else
            {
                receiptModel.BILL_NO = null;
            }
            return PartialView("AddReceiptDetails", receiptModel);            
        }
   [Audit]
        public ActionResult AddPaymentDetails()
        {
            PaymentDetailsModel paymentModel = new PaymentDetailsModel();
            //paymentModel.PHDN_AUTH_ID = "1";
            return View(paymentModel);
        }
        
        [HttpPost]
        [Audit]
         [ValidateAntiForgeryToken]
        public ActionResult AddPaymentDetails(PaymentDetailsModel paymentModel, String parameter, String hash, String key)
        {
            if (ModelState.IsValid)
            {
                String[] strParameters = URLEncrypt.DecryptParameters(new string[] { parameter, hash, key });
                paymentModel.AUTH_ID = Convert.ToInt64(strParameters[0].Split('$')[0]);
                objCommon = new CommonFunctions();

                //added by koustubh nakate on 22/07/2013 to check previous month is closed or not
                if (objCommon.GetStringToDateTime(paymentModel.BILL_DATE).Month != 0 && objCommon.GetStringToDateTime(paymentModel.BILL_DATE).Month < 13 && objCommon.GetStringToDateTime(paymentModel.BILL_DATE).Year != 0)
                {
                    string monthlyClosingStatus = string.Empty;
                    String message = String.Empty;
                    monthlyClosingStatus = objCommon.MonthlyClosingValidation((Int16)objCommon.GetStringToDateTime(paymentModel.BILL_DATE).Month, (Int16)objCommon.GetStringToDateTime(paymentModel.BILL_DATE).Year, PMGSYSession.Current.FundType, PMGSYSession.Current.LevelId, PMGSYSession.Current.AdminNdCode,ref message);

                    if (monthlyClosingStatus.Equals("-111"))
                    {
                        ModelState.AddModelError("BILL_DATE", message);
                    }
                    if (monthlyClosingStatus.Equals("-222"))
                    {
                        ModelState.AddModelError("BILL_DATE", "Month is already closed.");
                    }
                }


                String status = objAuthBAL.AddPaymentDetails(paymentModel);
                if (status == String.Empty)
                {
                    return this.Json(new { success = true, message = "" });
                }
                else
                {
                    return this.Json(new { success = true, message = "Error while processing your request" });
                }

            }
            return PartialView("AddPaymentDetails", paymentModel); 
        }


        /// <summary>
        /// action to return view of  Authorization request list page
        /// </summary>
        /// <returns></returns>
        [Audit]
        [GenericAccountValidationFilter(InputParameter = new string[] { "BankDetails", "ChequeBookDetails", "OpeningBalanceDetails", "AuthSign" })]
        public ActionResult GetAuthorizationRequest(String id)
        {
            TransactionParams objparams = new TransactionParams();
            objCommon = new CommonFunctions();
            try
            {

                objparams.BILL_TYPE = "P";
                objparams.FUND_TYPE = PMGSYSession.Current.FundType;
                objparams.LVL_ID = PMGSYSession.Current.LevelId;
                objparams.ADMIN_ND_CODE = PMGSYSession.Current.AdminNdCode;
                objparams.DISTRICT_CODE = PMGSYSession.Current.DistrictCode;

                if (!String.IsNullOrEmpty(id))
                {

                    List<SelectListItem> monthList = objCommon.PopulateMonths(Convert.ToInt16(id.Split('$')[0]));
                    ViewData["months"] = monthList;

                    List<SelectListItem> yearList = objCommon.PopulateYears(Convert.ToInt16(id.Split('$')[1]));
                    ViewData["year"] = yearList;

                }
                else
                {
                    //new change done by Vikram on 01-Jan-2014
                    if (PMGSYSession.Current.AccMonth != 0)
                    {
                        List<SelectListItem> monthList = objCommon.PopulateMonths(PMGSYSession.Current.AccMonth);
                        ViewData["months"] = monthList;

                        List<SelectListItem> yearList = objCommon.PopulateYears(PMGSYSession.Current.AccYear);
                        ViewData["year"] = yearList;
                    }
                    else
                    {
                        //end of change
                        List<SelectListItem> monthList = objCommon.PopulateMonths(DateTime.Now.Month);
                        ViewData["months"] = monthList;

                        List<SelectListItem> yearList = objCommon.PopulateYears(DateTime.Now.Year);
                        ViewData["year"] = yearList;
                    }

                }
                List<SelectListItem> transactionType = objCommon.PopulateAuthorizationTransaction(objparams);
                ViewData["TXN_ID"] = transactionType;

                return View("AuthorizationListMaster");
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                string errorMsg = string.Format("Errors: {0}", ex.Message);
                Response.TrySkipIisCustomErrors = true;
                Response.StatusCode = 500;
                Response.Write(errorMsg);
                return Json(string.Empty);
            }
        }


        /// <summary>
        /// action to get the authorization request details
        /// </summary>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <param name="sidx"></param>
        /// <param name="sord"></param>
        /// <returns></returns>
           [Audit]
        public JsonResult AuthorizationRequestList(int? page, int? rows, string sidx, string sord)
        {
            try
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

                PaymentFilterModel objFilter = new PaymentFilterModel();
                objFilter.FilterMode = Request.Params["mode"].Trim();


                long totalRecords;
                objFilter.Month = Convert.ToInt16(Request.Params["months"]);
                objFilter.Year = Convert.ToInt16(Request.Params["year"]);

                if (objFilter.FilterMode.Equals("Search"))
                {
                    objFilter.FromDate = Request.Params["fromDate"].ToString() == String.Empty ? null : Request.Params["fromDate"].ToString();
                    objFilter.ToDate = Request.Params["toDate"].ToString() == String.Empty ? null : Request.Params["toDate"].ToString();
                    objFilter.TransId = Request.Params["transType"] == String.Empty ? Convert.ToInt16(0) : Convert.ToInt16(Request.Params["transType"].Trim().Split('$')[0]);
                    objFilter.AuthorizationStatus = Request.Params["AUTH_STATUS"].ToString() == String.Empty ? String.Empty : Request.Params["AUTH_STATUS"].ToString();
                }
                objFilter.page = Convert.ToInt32(page) - 1;
                objFilter.rows = Convert.ToInt32(rows);
                objFilter.sidx = sidx.ToString();
                objFilter.sord = sord.ToString();

                objFilter.Bill_type = "P";

                objFilter.AdminNdCode = PMGSYSession.Current.AdminNdCode;
                objFilter.FundType = PMGSYSession.Current.FundType;
                objFilter.LevelId = PMGSYSession.Current.LevelId;


                var jsonData = new
                {
                    rows = objAuthBAL.ListAuthorizationRequestDetails(objFilter, out totalRecords),
                    total = totalRecords <= objFilter.rows ? 1 : totalRecords / objFilter.rows + 1,
                    page = objFilter.page,
                    records = totalRecords
                };
                return Json(jsonData);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                string errorMsg = string.Format("Errors: {0}", ex.Message);
                Response.TrySkipIisCustomErrors = true;
                Response.StatusCode = 500;
                Response.Write(errorMsg);
                return Json(string.Empty);
            }

        }



        /// <summary>
        /// action to get the authorization request details
        /// </summary>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <param name="sidx"></param>
        /// <param name="sord"></param>
        /// <returns></returns>
        [HttpPost]
        [Audit]
        public JsonResult ListAuthorizationRequestForDataEntry(int? page, int? rows, string sidx, string sord, String parameter, String hash, String key)
        {
            try
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
                Int64 Auth_Id = 0;
                long totalRecords = 0;
                PaymentFilterModel objFilter = new PaymentFilterModel();

                objFilter.page = Convert.ToInt32(page) - 1;
                objFilter.rows = Convert.ToInt32(rows);
                objFilter.sidx = sidx.ToString();
                objFilter.sord = sord.ToString();
                objFilter.Bill_type = "P";
                objFilter.AdminNdCode = PMGSYSession.Current.AdminNdCode;
                objFilter.FundType = PMGSYSession.Current.FundType;
                objFilter.LevelId = PMGSYSession.Current.LevelId;

                if (!String.IsNullOrEmpty(parameter) && !String.IsNullOrEmpty(hash) && !String.IsNullOrEmpty(key))
                {
                    String[] urlParams = URLEncrypt.DecryptParameters(new String[] { parameter, hash, key });
                    if (urlParams.Length >= 1)
                    {
                        String[] urlSplitParams = urlParams[0].Split('$');
                        Auth_Id = Convert.ToInt64(urlSplitParams[0]);

                        objFilter.BillId = Auth_Id;

                        var jsonData = new
                        {
                            rows = objAuthBAL.ListAuthorizationMasterDetails(objFilter, out totalRecords),
                            total = totalRecords <= objFilter.rows ? 1 : totalRecords / objFilter.rows + 1,
                            page = objFilter.page,
                            records = totalRecords
                        };
                        return Json(jsonData);

                    }
                    else
                    {
                        throw new Exception("Error while getting master payment details");
                    }
                }
                else
                {
                    var jsonData = new
                    {
                        rows = 0,
                        total = 0,
                        page = objFilter.page,
                        records = totalRecords
                    };
                    return Json(jsonData);
                }

            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                string errorMsg = string.Format("Errors: {0}", ex.Message);
                Response.TrySkipIisCustomErrors = true;
                Response.StatusCode = 500;
                Response.Write(errorMsg);
                return Json(string.Empty);
            }

        }

        /// <summary>
        /// Get action menthos to get the Authorization request Master data entry page
        /// </summary>
        /// <returns></returns>

        [Audit]
        public ActionResult GetAddMasterAuthorization(String id)
        {


            TransactionParams objparams = new TransactionParams();
            AuthorizationRequestMasterModel model = new AuthorizationRequestMasterModel();
            CommonFunctions objCommon = new CommonFunctions();
            try
            {
                objparams.BILL_TYPE = "P";
                objparams.FUND_TYPE = PMGSYSession.Current.FundType;
                objparams.LVL_ID = PMGSYSession.Current.LevelId;
                objparams.ADMIN_ND_CODE = PMGSYSession.Current.AdminNdCode;
                objparams.DISTRICT_CODE = PMGSYSession.Current.DistrictCode;


                int month = 0;
                int year = 0;

                if (!String.IsNullOrEmpty(id))
                {
                    model.AUTH_MONTH = Convert.ToInt16(id.Split('$')[0]);

                    model.AUTH_YEAR = Convert.ToInt16(id.Split('$')[1]);

                    ViewBag.operationType = id.Split('$')[2];

                }
                else
                {
                    throw new Exception("Error While loading the page..");
                }

                objparams.OP_MODE = "A";
                List<SelectListItem> transactionType = objCommon.PopulateAuthorizationTransaction(objparams);
                model.TXN_ID_LIST = transactionType;

                List<SelectListItem> monthList = objCommon.PopulateMonths(month);
                model.AUTH_MONTH_LIST = monthList;

                List<SelectListItem> yearList = objCommon.PopulateYears(year);
                model.AUTH_YEAR_LIST = yearList;


                List<SelectListItem> ContractorList = objCommon.PopulateContractorSupplier(objparams);
                model.MAST_CON_ID_C1 = ContractorList;

                model.AUTH_NO = String.Empty;

                //Modified by Abhishek kamble 21-jan-2014 
                //model.AUTH_DATE = objCommon.GetDateTimeToString(DateTime.Now);
                model.AUTH_DATE = null;

                ViewBag.LevelID = PMGSYSession.Current.LevelId;

                return View("AuthorizationMaster", model);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                string errorMsg = string.Format("Errors: {0}", ex.Message);
                Response.TrySkipIisCustomErrors = true;
                Response.StatusCode = 500;
                Response.Write(errorMsg);
                return Json(string.Empty);
            }
        }

        /// <summary>
        /// action called from authorizationListMaster.js for adding new authorization 
        /// </summary>
        /// <returns> action returns the view  page for intermediate page which will call the view page to add master authorization details</returns>
        [HttpGet]
        [Audit]
        public ActionResult GetAuthMasterAndTransDetails(String id)
        {
            try
            {
                int month = 0;
                int year = 0;
                string operationType = string.Empty;

                if (!String.IsNullOrEmpty(id))
                {
                    month = Convert.ToInt32(id.Split('$')[0]);
                    year = Convert.ToInt16(id.Split('$')[1]);
                    operationType = id.Split('$')[2].ToString().Trim();
                }
                else
                {
                    throw new Exception("Error While loading the page..");
                }

                ViewBag.month = month;
                ViewBag.year = year;
                ViewBag.operationType = operationType;
                ViewBag.Bill_id = 0;
                ViewBag.Bill_finalize = false;
                ViewBag.DeductionScreenRequired = true;
                ViewBag.PaymentScreenRequired = true;

                return View("AuthMasterAndTransDetails");
            }

            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                string errorMsg = string.Format("Errors: {0}", ex.Message);
                Response.TrySkipIisCustomErrors = true;
                Response.StatusCode = 500;
                Response.Write(errorMsg);
                return Json(string.Empty);
            }
        }


        /// <summary>
        /// post page to add the master data entry page 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Audit]
        public ActionResult PostAddMasterAuthorization(AuthorizationRequestMasterModel model)
        {

            try
            {
                Int64 Auth_Id = 0;
                TransactionParams objparams = new TransactionParams();
                ACC_SCREEN_DESIGN_PARAM_MASTER obj = new ACC_SCREEN_DESIGN_PARAM_MASTER();
                objCommon = new CommonFunctions();


                model.ADMIN_ND_CODE = PMGSYSession.Current.AdminNdCode;
                model.FUND_TYPE = PMGSYSession.Current.FundType;
                model.LVL_ID = PMGSYSession.Current.LevelId;
                objparams.DISTRICT_CODE = PMGSYSession.Current.DistrictCode;
                objparams.BILL_TYPE = "P";
                objparams.FUND_TYPE = PMGSYSession.Current.FundType;
                objparams.LVL_ID = PMGSYSession.Current.LevelId;

                #region validation based on master design params

                //if model is valid by view model validations
                if (ModelState.IsValid)
                {

                    //check if opening balance entry is finalized
                 /*   bool oBFinalized = objCommon.CheckIfOBFinalized(objparams);

                    if (!oBFinalized)
                    {
                        return Json(new { Success = false, Bill_ID = -4 });
                    }
                 */   
                    DateTime? openingBalanceDate = objCommon.GetOBDate(objparams);
                    if (openingBalanceDate.HasValue)
                    {
                        if (objCommon.GetStringToDateTime(model.AUTH_DATE) < openingBalanceDate.Value)
                        {
                            ModelState.AddModelError("AUTH_DATE", "Authorization Date must be greater than or equal to opening balance date");
                        }
                    }

                    if (model.AUTH_MONTH != 0 && model.AUTH_MONTH < 13 && model.AUTH_YEAR != 0)
                    {
                        string monthlyClosingStatus = string.Empty;

                        string errMessage = string.Empty;

                        monthlyClosingStatus = objCommon.MonthlyClosingValidation(model.AUTH_MONTH, model.AUTH_YEAR, objparams.FUND_TYPE, model.LVL_ID, model.ADMIN_ND_CODE,ref errMessage);

                        if (monthlyClosingStatus.Equals("-111"))
                        {
                            ModelState.AddModelError("AUTH_MONTH", errMessage);
                        }
                        if (monthlyClosingStatus.Equals("-222"))
                        {
                            ModelState.AddModelError("AUTH_MONTH", "Month is already closed.");
                        }
                    }

                    //for the validation based on master design params we have to set properties of elemetns 
                    //we require txnid so validate it first 
                    if (model.TXN_ID == null)
                    {
                        ModelState.AddModelError(model.TXN_ID, "Trnasaction type is required.");
                    }
                    else
                    {
                        objparams.TXN_ID = Convert.ToInt16(model.TXN_ID.Split('$')[0]);

                        obj = objCommon.getMasterDesignParam(objparams);


                        if (obj.MAST_CON_REQ.Trim() == "Y")
                        {

                            if (model.MAST_CON_ID_C == null || model.MAST_CON_ID_C == 0)
                            {
                                ModelState.AddModelError("MAST_CON_ID_C", "Company Name is Required");
                            }
                            if (model.PAYEE_NAME == null || model.PAYEE_NAME == string.Empty)
                            {
                                ModelState.AddModelError("PAYEE_NAME_C", "Payee Name is Required");
                            }

                        }
                        else
                        {

                            if (ModelState.ContainsKey("MAST_CON_ID_C"))
                                ModelState["MAST_CON_ID_C"].Errors.Clear();
                            if (ModelState.ContainsKey("PAYEE_NAME"))
                                ModelState["PAYEE_NAME"].Errors.Clear();
                        }


                        if (obj.DED_REQ.Trim() == "Y")
                        {

                            if (model.CASH_AMOUNT.ToString() == "")
                            {
                                ModelState.AddModelError("CASH_AMOUNT", "Cash Amount is Required");
                            }
                        }
                        else
                        {
                            if (ModelState.ContainsKey("CASH_AMOUNT"))
                                ModelState["CASH_AMOUNT"].Errors.Clear();
                        }


                        if (model.CHEQUE_AMOUNT.ToString() == string.Empty || model.CHEQUE_AMOUNT == 0)
                        {

                            ModelState.AddModelError("CHEQUE_AMOUNT", "Bank Authorization Request Amount is Required");

                        }

                    }

                }

                #endregion

                if (ModelState.IsValid)
                {
                    #region valid model

                    //get payee name based on transaction 

                    if (obj.MAST_CON_REQ.Trim() == "Y")
                    {
                        model.PAYEE_NAME = model.PAYEE_NAME;
                        model.MAST_CON_ID = model.MAST_CON_ID_C.HasValue ? model.MAST_CON_ID_C.Value : 0;
                    }

                    decimal GrossAmount = 0;

                    if (model.CASH_AMOUNT != 0)
                    {
                        GrossAmount = model.CHEQUE_AMOUNT + model.CASH_AMOUNT;
                    }
                    else
                    {
                        GrossAmount = model.CHEQUE_AMOUNT;
                    }
                    model.GROSS_AMOUNT = GrossAmount;

                    Auth_Id = objAuthBAL.AddEditMasterAuthorizationDetails(model, "A", Auth_Id);


                    int Master_Head = Convert.ToInt32(model.TXN_ID.Split('$')[0]);

                    bool deductionRequired = model.CASH_AMOUNT == 0 ? false : true;

                    bool PaymentRequired = model.CHEQUE_AMOUNT == 0 ? false : true;

                    ModelState.Clear();

                    string encrptedBill_Id = string.Empty;

                    //if Authorization number allready exist (may happen due to simultanious operation)
                    if (Auth_Id == -2)
                    {
                        return Json(new { Success = false, Auth_Id = -2, Master_Head = Master_Head, deductionRequired = deductionRequired, PaymentRequired = PaymentRequired });

                    }else if(Auth_Id == -3) //bank details does not found
                    {
                        return Json(new { Success = false, Auth_Id = -3, Master_Head = Master_Head, deductionRequired = deductionRequired, PaymentRequired = PaymentRequired });

                    }
                    else
                    {
                        encrptedBill_Id = URLEncrypt.EncryptParameters(new string[] { Auth_Id.ToString() });
                        return Json(new { Success = true, Auth_Id = encrptedBill_Id, Master_Head = Master_Head, deductionRequired = deductionRequired, PaymentRequired = PaymentRequired });
                    }


                    #endregion valid model
                }
                else
                {
                    #region prepare view when model is invalid

                    int txnid = objparams.TXN_ID;

                    objparams.TXN_ID = 0;

                    objparams.OP_MODE = "A";
                    List<SelectListItem> transactionType = objCommon.PopulateAuthorizationTransaction(objparams);
                    model.TXN_ID_LIST = transactionType;

                    List<SelectListItem> monthList = objCommon.PopulateMonths(model.AUTH_MONTH);
                    model.AUTH_MONTH_LIST = monthList;

                    List<SelectListItem> yearList = objCommon.PopulateYears(model.AUTH_YEAR);
                    model.AUTH_YEAR_LIST = yearList;


                    List<SelectListItem> ContractorList = objCommon.PopulateContractorSupplier(objparams);
                    model.MAST_CON_ID_C1 = ContractorList;


                    ViewBag.LevelID = PMGSYSession.Current.LevelId;

                    return View("AuthorizationMaster", model);

                    #endregion

                }

            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                string errorMsg = string.Format("Errors: {0}", ex.Message);
                Response.TrySkipIisCustomErrors = true;
                Response.StatusCode = 500;
                Response.Write(errorMsg);
                return Json(string.Empty);
            }

        }



        /// <summary>
        /// post action to edit the authorization details  
        /// </summary>
        /// <param name="model"></param>
        /// <param name="parameter"></param>
        /// <param name="hash"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        [HttpPost]
        [Audit]
        public ActionResult PostEditAuthorizationMasterDetails(AuthorizationRequestMasterModel model, String parameter, String hash, String key)
        {
            try
            {

                TransactionParams objparams = new TransactionParams();

                objparams.DISTRICT_CODE = PMGSYSession.Current.DistrictCode;
                objparams.BILL_TYPE = "P";
                objparams.FUND_TYPE = PMGSYSession.Current.FundType;
                objparams.LVL_ID = PMGSYSession.Current.LevelId;

                ACC_SCREEN_DESIGN_PARAM_MASTER obj = new ACC_SCREEN_DESIGN_PARAM_MASTER();
                ACC_AUTH_REQUEST_MASTER masterDetails = new ACC_AUTH_REQUEST_MASTER();
                Int64 Bill_ID = 0;

                objCommon = new CommonFunctions();


                if (!String.IsNullOrEmpty(parameter) && !String.IsNullOrEmpty(hash) && !String.IsNullOrEmpty(key))
                {
                    String[] urlParams = URLEncrypt.DecryptParameters(new String[] { parameter, hash, key });
                    if (urlParams.Length >= 1)
                    {
                        String[] urlSplitParams = urlParams[0].Split('$');
                        Bill_ID = Convert.ToInt64(urlSplitParams[0]);
                    }
                }

                #region server validation

              /*  //check if opening balance entry is finalized
                bool oBFinalized = objCommon.CheckIfOBFinalized(objparams);

                if (!oBFinalized)
                {
                    return Json(new { Success = false, Bill_ID = -4 });
                }
                */

                DateTime? openingBalanceDate = objCommon.GetOBDate(objparams);
                if (openingBalanceDate.HasValue)
                {
                    if (objCommon.GetStringToDateTime(model.AUTH_DATE) < openingBalanceDate.Value)
                    {
                        ModelState.AddModelError("AUTH_DATE", "Authorization Date must be greater than or equal to opening balance date");
                    }
                }

                if (model.AUTH_MONTH != 0 && model.AUTH_MONTH < 13 && model.AUTH_YEAR != 0)
                {
                    string monthlyClosingStatus = string.Empty;

                    String errMessage = String.Empty;

                    monthlyClosingStatus = objCommon.MonthlyClosingValidation(model.AUTH_MONTH, model.AUTH_YEAR, objparams.FUND_TYPE, model.LVL_ID, model.ADMIN_ND_CODE,ref errMessage);

                    if (monthlyClosingStatus.Equals("-111"))
                    {
                        ModelState.AddModelError("AUTH_MONTH", errMessage);
                    }
                    if (monthlyClosingStatus.Equals("-222"))
                    {
                        ModelState.AddModelError("AUTH_MONTH", "Month is already closed.");
                    }
                }


                //for the validation based on master design params we have to set properties of elemetns 
                //we require txnid so validate it first 

                if (model.TXN_ID == null)
                {
                    ModelState.AddModelError(model.TXN_ID, "Trnasaction type is required.");
                }
                else
                {

                    masterDetails = objAuthBAL.GetMasterAuthorizationDetails(Bill_ID);

                    objparams.TXN_ID = Convert.ToInt16(masterDetails.TXN_ID);

                    obj = objCommon.getMasterDesignParam(objparams);




                    if (obj.MAST_CON_REQ.Trim() == "Y")
                    {


                        if (model.MAST_CON_ID_C == null || model.MAST_CON_ID_C == 0)
                        {
                            ModelState.AddModelError("MAST_CON_ID_C", "Company Name is Required");
                        }
                        if (model.PAYEE_NAME == null || model.PAYEE_NAME == string.Empty)
                        {
                            ModelState.AddModelError("PAYEE_NAME_C", "Payee Name is Required");
                        }

                    }
                    else
                    {

                        if (ModelState.ContainsKey("MAST_CON_ID_C"))
                            ModelState["MAST_CON_ID_C"].Errors.Clear();
                        if (ModelState.ContainsKey("PAYEE_NAME_C"))
                            ModelState["PAYEE_NAME"].Errors.Clear();
                    }


                    //in case of edit we dont have to validate the bill number as we are getting it from old entry.
                    if (ModelState.ContainsKey("AUTH_NO"))
                        ModelState["AUTH_NO"].Errors.Clear();
                }
                #endregion

                if (ModelState.IsValid)
                {

                    #region valid model
                    model.ADMIN_ND_CODE = PMGSYSession.Current.AdminNdCode;
                    model.FUND_TYPE = PMGSYSession.Current.FundType;
                    model.LVL_ID = PMGSYSession.Current.LevelId;



                    if (obj.MAST_CON_REQ.Trim() == "Y")
                    {
                        model.PAYEE_NAME = model.PAYEE_NAME;
                        model.MAST_CON_ID = model.MAST_CON_ID_C.HasValue ? model.MAST_CON_ID_C.Value : 0;
                    }

                    decimal GrossAmount = 0;

                    if (model.CASH_AMOUNT != 0)
                    {
                        GrossAmount = model.CHEQUE_AMOUNT + model.CASH_AMOUNT;
                    }
                    else
                    {
                        GrossAmount = model.CHEQUE_AMOUNT;
                    }

                    model.GROSS_AMOUNT = GrossAmount;

                    Bill_ID = objAuthBAL.AddEditMasterAuthorizationDetails(model, "E", Bill_ID);
                    bool Success = false;

                    string encrptedBill_Id = String.Empty;


                    //-1 if allrady finalized -2 if epaymnumber already exist
                    if (Bill_ID != -1 && Bill_ID != -2 && Bill_ID != -3)
                    {
                        Success = true;
                        encrptedBill_Id = URLEncrypt.EncryptParameters(new string[] { Bill_ID.ToString() });
                    }
                    else
                    {
                        Success = false;
                        if (Bill_ID == -1)
                        {
                            encrptedBill_Id = "-1";
                        }
                        else if (Bill_ID == -2)
                        {
                            encrptedBill_Id = "-2";
                        }
                        else if (Bill_ID == -3)
                        {
                            encrptedBill_Id = "-3";
                        }
                    }
                    int Master_Head = Convert.ToInt32(model.TXN_ID.Split('$')[0]);

                    bool deductionRequired = model.CASH_AMOUNT == 0 ? false : true;

                    bool PaymentRequired = model.CHEQUE_AMOUNT == 0 ? false : true;

                    ModelState.Clear();

                    return Json(new { Success = Success, Bill_ID = encrptedBill_Id, Master_Head = Master_Head, deductionRequired = deductionRequired, PaymentRequired = PaymentRequired });

                    #endregion
                }
                else
                {
                    #region prepare view on error
                    //string errorval = string.Empty;
                    //foreach (ModelState modelState in ViewData.ModelState.Values)
                    //{
                    //    foreach (ModelError error in modelState.Errors)
                    //    {
                    //        errorval = error.ErrorMessage;
                    //    }
                    //}

                    objparams.BILL_TYPE = "P";
                    objparams.FUND_TYPE = PMGSYSession.Current.FundType;
                    objparams.LVL_ID = PMGSYSession.Current.LevelId;
                    objparams.ADMIN_ND_CODE = PMGSYSession.Current.AdminNdCode;
                    objparams.DISTRICT_CODE = PMGSYSession.Current.DistrictCode;


                    List<SelectListItem> transactionType = objCommon.PopulateAuthorizationTransaction(objparams);
                    // ViewData["TXN_ID"] = transactionType;
                    model.TXN_ID_LIST = transactionType;


                    List<SelectListItem> monthList = objCommon.PopulateMonths(masterDetails.AUTH_MONTH);
                    model.AUTH_MONTH_LIST = monthList;
                    model.AUTH_MONTH = masterDetails.AUTH_MONTH;

                    List<SelectListItem> yearList = objCommon.PopulateYears(masterDetails.AUTH_YEAR);
                    model.AUTH_YEAR = masterDetails.AUTH_YEAR;
                    model.AUTH_YEAR_LIST = yearList;

                    List<SelectListItem> ContractorList = objCommon.PopulateContractorSupplier(objparams);
                    //ViewData["MAST_CON_ID_C"] = ContractorList;
                    model.MAST_CON_ID_C1 = ContractorList;

                    ViewBag.operationType = "E";

                    ViewBag.Bill_id = URLEncrypt.EncryptParameters(new string[] { Bill_ID.ToString() });

                    ViewBag.LevelID = PMGSYSession.Current.LevelId;

                    return View("AuthorizationMaster", model);
                    #endregion

                }

            }

            catch (DbEntityValidationException ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);

                string exMessege = string.Empty;
                foreach (var eve in ex.EntityValidationErrors)
                {
                    foreach (var ve in eve.ValidationErrors)
                    {
                        exMessege += ve.ErrorMessage + "<br> ";
                    }
                }

                string errorMsg = string.Format("Errors: {0}", exMessege);
                Response.TrySkipIisCustomErrors = true;
                Response.StatusCode = 500;
                Response.Write(errorMsg);
                return Json(string.Empty);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);

                string errorMsg = string.Format("Errors: {0}", ex.Message);
                Response.TrySkipIisCustomErrors = true;
                Response.StatusCode = 500;
                Response.Write(errorMsg);
                return Json(string.Empty);
            }

        }



        /// <summary>
        /// action to get authorization number
        /// </summary>
        /// <returns></returns>
        [Audit]
        public JsonResult GetAuthorizationNumber(string id)
        {
            try
            {
                Int16 month = 0;
                Int16 year = 0;

                month = Convert.ToInt16(id.Split('$')[0]);
                year = Convert.ToInt16(id.Split('$')[1]);

                return Json(objAuthBAL.GetAuthorizationNumber(month, year, PMGSYSession.Current.StateCode, PMGSYSession.Current.AdminNdCode));


            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                string errorMsg = string.Format("Errors: {0}", ex.Message);
                Response.TrySkipIisCustomErrors = true;
                Response.StatusCode = 500;
                Response.Write(errorMsg);
                return Json(string.Empty);
            }

        }


        /// <summary>
        /// function to get authorization balance
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="hash"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        [Audit]
        public JsonResult GetAuthorizationAmountBalance(String parameter, String hash, String key)
        {
            Int64 Auth_Id = 0;

            try
            {
                if (!String.IsNullOrEmpty(parameter) && !String.IsNullOrEmpty(hash) && !String.IsNullOrEmpty(key))
                {
                    String[] urlParams = URLEncrypt.DecryptParameters(new String[] { parameter, hash, key });
                    if (urlParams.Length >= 1)
                    {
                        String[] urlSplitParams = urlParams[0].Split('$');
                        Auth_Id = Convert.ToInt64(urlSplitParams[0]);

                    }

                    AmountCalculationModel amountModel = new AmountCalculationModel();

                    amountModel = objAuthBAL.GetAuthorizationAmountBalance(Auth_Id);

                    return Json(new
                    {
                        Success = true,
                        //TotalAmtToEnterChqAmount = Convert.ToDecimal(amountModel.TotalAmtToEnterChqAmount).ToString("#0.00", System.Globalization.CultureInfo.CreateSpecificCulture("hi-IN"),

                        TotalAmtToEnterChqAmount = amountModel.TotalAmtToEnterChqAmount,
                        TotalAmtToEnterCachAmount = amountModel.TotalAmtToEnterCachAmount,
                        TotalAmtToEnterDedAmount = amountModel.TotalAmtToEnterDedAmount,
                        TotalAmtToEnterGrossAmount = amountModel.TotalAmtToEnterGrossAmount,

                        TotalAmtEnteredChqAmount = amountModel.TotalAmtEnteredChqAmount,
                        TotalAmtEnteredCachAmount = amountModel.TotalAmtEnteredCachAmount,
                        TotalAmtEnteredDedAmount = amountModel.TotalAmtEnteredDedAmount,
                        TotalAmtEnteredGrossAmount = amountModel.TotalAmtEnteredGrossAmount,

                        DiffChqAmount = amountModel.DiffChqAmount,
                        DiffCachAmount = amountModel.DiffCachAmount,
                        DiffDedAmount = amountModel.DiffDedAmount,
                        DiffGrossAmount = amountModel.DiffGrossAmount,
                        VoucherFinalized = amountModel.VoucherFinalized,
                        CashPayment = amountModel.CashPayment

                    });

                }
                else
                {
                    throw new Exception("Error while getting  authorization balance details..");
                }

            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                string errorMsg = string.Format("Errors: {0}", ex.Message);
                Response.TrySkipIisCustomErrors = true;
                Response.StatusCode = 500;
                Response.Write(errorMsg);
                return Json(string.Empty);
            }
        }

        /// <summary>
        /// this action returns the authorization transaction for with facility to add amount and deduction amount
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="hash"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        [Audit]
        public ActionResult GetAuthDetailsEntryForm(String parameter, String hash, String key)
        {

            try
            {
                PMGSYEntities dbContext = new PMGSYEntities();
                TransactionParams objparams = new TransactionParams();
                objCommon = new CommonFunctions();

                if (!String.IsNullOrEmpty(parameter) && !String.IsNullOrEmpty(hash) && !String.IsNullOrEmpty(key))
                {

                    if (!String.IsNullOrEmpty(parameter) && !String.IsNullOrEmpty(hash) && !String.IsNullOrEmpty(key))
                    {
                        String[] urlParams = URLEncrypt.DecryptParameters(new String[] { parameter, hash, key });
                        if (urlParams.Length >= 1)
                        {
                            String[] urlSplitParams = urlParams[0].Split('$');
                            objparams.BILL_ID = Convert.ToInt64(urlSplitParams[0]);
                        }
                    }
                    else
                    {
                        throw new Exception("Error while getting Master Authorization Request details..");
                    }
                }

                List<SelectListItem> roadlist = new List<SelectListItem>();
                roadlist.Add(new SelectListItem { Selected = true, Text = "--Select--", Value = "" });


                //get the master transaction details
                ACC_AUTH_REQUEST_MASTER masterDetails = new ACC_AUTH_REQUEST_MASTER();
                masterDetails = objAuthBAL.GetMasterAuthorizationDetails(objparams.BILL_ID);


                objparams.FUND_TYPE = masterDetails.FUND_TYPE;
                objparams.LVL_ID = masterDetails.LVL_ID;
                objparams.ADMIN_ND_CODE = masterDetails.ADMIN_ND_CODE;
                objparams.MAST_CONT_ID = masterDetails.MAST_CON_ID == null ? 0 : masterDetails.MAST_CON_ID.Value;
                objparams.DISTRICT_CODE = PMGSYSession.Current.DistrictCode;
                objparams.TXN_ID = masterDetails.TXN_ID;
                objparams.BILL_TYPE = "P";
                objparams.OP_MODE = "A";


                AuthorizationRequestDetailsModel model = new AuthorizationRequestDetailsModel();

                model.CONTRACTOR_ID = masterDetails.MAST_CON_ID == null ? 0 : masterDetails.MAST_CON_ID.Value;

                model.HeadId_P = objCommon.PopulateTransactions(objparams);

                model.AGREEMENT_C = roadlist;

                model.MAST_DPIU_CODEList = roadlist;

                model.IMS_PR_ROAD_CODEList = roadlist;

                //agreement for deduction
                ViewBag.DeductionAgreementRequired = objparams.MAST_CONT_ID == 0 ? false : true;

                if (objparams.MAST_CONT_ID != 0)
                {
                    model.AGREEMENT_DED = objCommon.PopulateAgreement(objparams);
                }
                else
                {
                    model.AGREEMENT_DED = roadlist;
                }

                objparams.BILL_TYPE = "D";

                model.HeadId_D = objCommon.PopulateTransactions(objparams);

                roadlist.Clear();

                roadlist.Add(new SelectListItem { Selected = true, Text = "--Select--", Value = "0" });

                model.IMS_SANCTION_YEAR_List = roadlist;

                model.IMS_SANCTION_PACKAGE_List = roadlist;


                List<SelectListItem> List = new List<SelectListItem>();

                List.Add(new SelectListItem { Text = "Yes", Value = "true" });
                List.Add(new SelectListItem { Selected = true, Text = "No", Value = "false" });

                model.final_pay = List;

                #region if for master payment multiple transsaction is not allowed

                ACC_SCREEN_DESIGN_PARAM_MASTER obj = new ACC_SCREEN_DESIGN_PARAM_MASTER();
                obj = objCommon.getMasterDesignParam(objparams);

                bool disablePayHead = false;

                if (obj.MTXN_REQ == "N")
                {
                    if (!dbContext.ACC_AUTH_REQUEST_DETAILS.Where(d => d.AUTH_ID == masterDetails.AUTH_ID).Any())
                    {
                        disablePayHead = false;
                    }
                    else
                    {
                        Int16 txn = dbContext.ACC_AUTH_REQUEST_DETAILS.Where(d => d.AUTH_ID == masterDetails.AUTH_ID).Select(c => c.TXN_ID).First();
                        string CheCash = dbContext.ACC_MASTER_TXN.Where(m => m.TXN_ID == txn).Select(m => m.CASH_CHQ).FirstOrDefault().Trim();
                        model.HEAD_ID_P = txn + "$" + CheCash;
                        disablePayHead = true;
                    }
                }

                #endregion

                ViewBag.disablePaymentHead = disablePayHead;

                ViewBag.Bill_ID = URLEncrypt.EncryptParameters(new string[] { objparams.BILL_ID.ToString() });

                ViewBag.BillFinalized = masterDetails.AUTH_FINALIZED;

                ViewBag.PaymentScreenRequired = (masterDetails.CHQ_AMOUNT != 0) ? true : false;

                ViewBag.DeductionScreenRequired = (masterDetails.CASH_AMOUNT != 0) ? true : false;

                return View("AuthDetailsEntryForm", model);

            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                string errorMsg = string.Format("Errors: {0}", ex.Message);
                Response.TrySkipIisCustomErrors = true;
                Response.StatusCode = 500;
                Response.Write(errorMsg);
                return Json(string.Empty);
            }
        }




        /// <summary>
        /// function to delete authorization Request
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="hash"></param>
        /// <param name="key"></param>
        /// <returns> 1 :deletion complete ,-1 finalized entry</returns>
        [Audit]
        public JsonResult DeleteAuthorizationRequest(String parameter, String hash, String key)
        {
            Int64 Auth_Id = 0;
            try
            {
                if (!String.IsNullOrEmpty(parameter) && !String.IsNullOrEmpty(hash) && !String.IsNullOrEmpty(key))
                {
                    String[] urlParams = URLEncrypt.DecryptParameters(new String[] { parameter, hash, key });
                    if (urlParams.Length >= 1)
                    {
                        String[] urlSplitParams = urlParams[0].Split('$');
                        Auth_Id = Convert.ToInt64(urlSplitParams[0]);

                        String result = String.Empty;

                        result = objAuthBAL.DeleteAuthorizationRequest(Auth_Id);
                        if (result == "1")
                        {

                            return Json(new
                            {
                                result = 1
                            });
                        }
                        else
                        {
                            return Json(new
                            {
                                result = -1
                            });

                        }
                    }
                    else
                    {

                        throw new Exception("Error while getting master authorization details..");
                    }
                }
                else
                {
                    throw new Exception("Error while getting master authorization details..");
                }


            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                string errorMsg = string.Format("Errors: {0}", ex.Message);
                Response.TrySkipIisCustomErrors = true;
                Response.StatusCode = 500;
                Response.Write(errorMsg);
                return Json(string.Empty);
            }



        }

        /// <summary>
        /// function to edit authorization master details
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="hash"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        [Audit]
        public ActionResult GetEditAuthorizationRequest(String parameter, String hash, String key)
        {
            TransactionParams objparams = new TransactionParams();

            Int64 Bill_Id = 0;

            if (!String.IsNullOrEmpty(parameter) && !String.IsNullOrEmpty(hash) && !String.IsNullOrEmpty(key))
            {
                String[] urlParams = URLEncrypt.DecryptParameters(new String[] { parameter, hash, key });
                if (urlParams.Length >= 1)
                {
                    String[] urlSplitParams = urlParams[0].Split('$');
                    Bill_Id = Convert.ToInt64(urlSplitParams[0]);

                }
            }
            else
            {
                throw new Exception("Error while getting authorization request details..");
            }

            ACC_AUTH_REQUEST_MASTER masterDetails = new ACC_AUTH_REQUEST_MASTER();

            masterDetails = objAuthBAL.GetMasterAuthorizationDetails(Bill_Id);

            ViewBag.month = masterDetails.AUTH_MONTH;

            ViewBag.year = masterDetails.AUTH_YEAR;

            ViewBag.operationType = "E";

            ViewBag.Bill_id = URLEncrypt.EncryptParameters(new string[] { Bill_Id.ToString() });

            //ACC_SCREEN_DESIGN_PARAM_MASTER obj = new ACC_SCREEN_DESIGN_PARAM_MASTER();
            //objparams.TXN_ID = masterDetails.TXN_ID;
            //obj = objCommon.getMasterDesignParam(objparams);

            ViewBag.PaymentScreenRequired = masterDetails.CHQ_AMOUNT != 0 ? true : false;

            ViewBag.DeductionScreenRequired = masterDetails.CASH_AMOUNT != 0 ? true : false;

            return View("AuthMasterAndTransDetails");

        }

        /// <summary>
        /// function to finalize the authorization
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="hash"></param>
        /// <param name="key"></param>
        /// <returns>1 :suceessfull , -1 not all details entered</returns>
        [Audit]
        public JsonResult FinalizeAuthorization(String parameter, String hash, String key)
        {

            Int64 auth_Id = 0;
            try
            {
                if (!String.IsNullOrEmpty(parameter) && !String.IsNullOrEmpty(hash) && !String.IsNullOrEmpty(key))
                {
                    String[] urlParams = URLEncrypt.DecryptParameters(new String[] { parameter, hash, key });
                    if (urlParams.Length >= 1)
                    {
                        String[] urlSplitParams = urlParams[0].Split('$');
                        auth_Id = Convert.ToInt64(urlSplitParams[0]);

                        String result = String.Empty;

                        result = objAuthBAL.FinlizeAuthorization(auth_Id);
                        if (result == "1")
                        {

                            return Json(new
                            {
                                result = 1
                            });
                        }
                        else
                        {
                            return Json(new
                            {
                                result = -1
                            });

                        }
                    }
                    else
                    {

                        throw new Exception("Error while getting master authorization details..");
                    }
                }
                else
                {
                    throw new Exception("Error while getting master authorization details..");
                }


            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                string errorMsg = string.Format("Errors: {0}", ex.Message);
                Response.TrySkipIisCustomErrors = true;
                Response.StatusCode = 500;
                Response.Write(errorMsg);
                return Json(string.Empty);
            }
        }


        /// <summary>
        /// function to get contractors agreement
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="hash"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        [Audit]
        public JsonResult GetSupplierContractorAgreement(String parameter, String hash, String key)
        {
            TransactionParams objparams = new TransactionParams();
            try
            {


                if (!String.IsNullOrEmpty(parameter) && !String.IsNullOrEmpty(hash) && !String.IsNullOrEmpty(key))
                {

                    if (!String.IsNullOrEmpty(parameter) && !String.IsNullOrEmpty(hash) && !String.IsNullOrEmpty(key))
                    {
                        String[] urlParams = URLEncrypt.DecryptParameters(new String[] { parameter, hash, key });
                        if (urlParams.Length >= 1)
                        {
                            String[] urlSplitParams = urlParams[0].Split('$');
                            objparams.BILL_ID = Convert.ToInt64(urlSplitParams[0]);
                        }
                    }
                    else
                    {
                        throw new Exception("Error while getting  authorization details..");
                    }
                }


                //get the master transaction details
                ACC_AUTH_REQUEST_MASTER masterDetails = new ACC_AUTH_REQUEST_MASTER();
                masterDetails = objAuthBAL.GetMasterAuthorizationDetails(objparams.BILL_ID);

                objparams.BILL_TYPE = "P";
                objparams.FUND_TYPE = masterDetails.FUND_TYPE;
                objparams.LVL_ID = masterDetails.LVL_ID;
                objparams.ADMIN_ND_CODE = masterDetails.ADMIN_ND_CODE;
                objparams.MAST_CONT_ID = masterDetails.MAST_CON_ID == null ? 0 : masterDetails.MAST_CON_ID.Value;
                objparams.DISTRICT_CODE = PMGSYSession.Current.DistrictCode;
                objparams.TXN_ID = masterDetails.TXN_ID;

                objCommon = new CommonFunctions();
                return Json(objCommon.PopulateAgreement(objparams));
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                string errorMsg = string.Format("Errors: {0}", ex.Message);
                Response.TrySkipIisCustomErrors = true;
                Response.StatusCode = 500;
                Response.Write(errorMsg);
                return Json(string.Empty);
            }

        }


        /// <summary>
        /// action to return grid data for payment deduction details
        /// </summary>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <param name="sidx"></param>
        /// <param name="sord"></param>
        /// <param name="parameter"></param>
        /// <param name="hash"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        [Audit]
        public JsonResult GetPaymentDetailList(int? page, int? rows, string sidx, string sord, String parameter, String hash, String key)
        {

            PaymentFilterModel objFilter = new PaymentFilterModel();
            try
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
                if (!String.IsNullOrEmpty(parameter) && !String.IsNullOrEmpty(hash) && !String.IsNullOrEmpty(key))
                {
                    String[] urlParams = URLEncrypt.DecryptParameters(new String[] { parameter, hash, key });
                    if (urlParams.Length >= 1)
                    {
                        String[] urlSplitParams = urlParams[0].Split('$');
                        objFilter.BillId = Convert.ToInt64(urlSplitParams[0]);


                    }
                }
                else
                {
                    throw new Exception("Error while getting payment transaction list");
                }

                long totalRecords;

                objFilter.page = Convert.ToInt32(page) - 1;
                objFilter.rows = Convert.ToInt32(rows);
                objFilter.sidx = sidx.ToString();
                objFilter.sord = sord.ToString();
                objFilter.Bill_type = "P";
                objFilter.FundType = PMGSYSession.Current.FundType;
                objFilter.Deduction_Payment = "P";

                objFilter.AdminNdCode = PMGSYSession.Current.AdminNdCode;
                objFilter.FundType = PMGSYSession.Current.FundType;
                objFilter.LevelId = PMGSYSession.Current.LevelId;

                /* 
              objFilter.AdminNdCode = 11;
              objFilter.LevelId = 5; */

                var jsonData = new
                {
                    rows = objAuthBAL.ListAuthDeductionDetailsForDataEntry(objFilter, out totalRecords),
                    total = totalRecords <= objFilter.rows ? 1 : totalRecords / objFilter.rows + 1,
                    page = objFilter.page,
                    records = totalRecords
                };

                return Json(jsonData);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                string errorMsg = string.Format("Errors: {0}", ex.Message);
                Response.TrySkipIisCustomErrors = true;
                Response.StatusCode = 500;
                Response.Write(errorMsg);
                return Json(string.Empty);
            }

        }

        /// <summary>
        /// function to add payment authorization details
        /// </summary>
        /// <param name="model"></param>
        /// <param name="parameter"></param>
        /// <param name="hash"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        [HttpPost]
        [Audit]
         [ValidateAntiForgeryToken]
        public ActionResult AddPaymentTransactionDetails(AuthorizationRequestDetailsModel model, String parameter, String hash, String key)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            ACC_SCREEN_DESIGN_PARAM_DETAILS parametes = new ACC_SCREEN_DESIGN_PARAM_DETAILS();
            TransactionParams objparams = new TransactionParams();
            Int64 Bill_ID = 0;
            objCommon = new CommonFunctions();
            try
            {

                if (!String.IsNullOrEmpty(parameter) && !String.IsNullOrEmpty(hash) && !String.IsNullOrEmpty(key))
                {
                    String[] urlParams = URLEncrypt.DecryptParameters(new String[] { parameter, hash, key });
                    if (urlParams.Length >= 1)
                    {
                        String[] urlSplitParams = urlParams[0].Split('$');
                        Bill_ID = Convert.ToInt64(urlSplitParams[0]);
                    }
                }

                //new change done by Vikram on 20-Dec-2013
                if (model.HEAD_ID_P != null)
                {
                    if (Convert.ToInt32(model.HEAD_ID_P.Split('$')[0]) == 48 || Convert.ToInt32(model.HEAD_ID_P.Split('$')[0]) == 49)
                    {
                        if (model.IMS_PR_ROAD_CODE != null)
                        {
                            if (!ValidateRoad(Convert.ToInt32(model.HEAD_ID_P.Split('$')[0]), model.IMS_PR_ROAD_CODE.Value, PMGSYSession.Current.FundType))
                            {
                                return Json(new { Success = false, status = "-555", message = "Select Upgrade/New road as per account head." });
                            }
                        }
                    }


                    //New validation to validate PMGSY scheme Roads based on Head. start 2 Sep 2014
                    if (PMGSYSession.Current.FundType == "P" && model.IMS_PR_ROAD_CODE != null && model.IMS_PR_ROAD_CODE != 0)
                    {
                        if (!ValidateRoadForPMGSYScheme(Convert.ToInt16(model.HEAD_ID_P.Split('$')[0]), model.IMS_PR_ROAD_CODE.Value))
                        {
                            return Json(new { Success = false, status = "-999", message = "Road can not be Selected, Since it is in different Scheme." });
                        }
                    }
                    //New validation to validate PMGSY scheme Roads based on Head. end

                }
                //end of change


                //get the contractor details from master details 
                ACC_AUTH_REQUEST_MASTER masterDetails = new ACC_AUTH_REQUEST_MASTER();
                masterDetails = objAuthBAL.GetMasterAuthorizationDetails(Bill_ID);

                objparams.TXN_ID = Convert.ToInt16(model.HEAD_ID_P.Split('$')[0]);

                parametes = objCommon.getDetailsDesignParam(objparams);


                #region if for master payment multiple transsaction is not allowed

                ACC_SCREEN_DESIGN_PARAM_MASTER obj = new ACC_SCREEN_DESIGN_PARAM_MASTER();

                TransactionParams objparams1 = new TransactionParams();

                objparams1.TXN_ID = masterDetails.TXN_ID;

                obj = objCommon.getMasterDesignParam(objparams1);

                bool disablePayHead = false;

                if (obj.MTXN_REQ == "N")
                {
                    if (!dbContext.ACC_AUTH_REQUEST_DETAILS.Where(d => d.AUTH_ID == masterDetails.AUTH_ID).Any())
                    {
                        disablePayHead = false;
                    }
                    else
                    {
                        Int16? txn = dbContext.ACC_AUTH_REQUEST_DETAILS.Where(d => d.AUTH_ID == masterDetails.AUTH_ID).Select(c => c.TXN_ID).First();
                        string CheCash = dbContext.ACC_MASTER_TXN.Where(m => m.TXN_ID == txn).Select(m => m.CASH_CHQ).FirstOrDefault().Trim();
                        model.HEAD_ID_P = txn + "$" + CheCash;
                        disablePayHead = true;
                    }
                }
                //to disable on error in javascript file
                ViewBag.disablePaymentHead = disablePayHead;

                bool disblehead = false;

                disblehead = disablePayHead;

                #endregion


                //if no other model errors are there then only check for dynamic validation
                if (ModelState.IsValid)
                {
                    #region serverside validation for payment



                    if (parametes.ROAD_REQ.Trim() == "Y")
                    {

                        if (model.IMS_PR_ROAD_CODE == null || model.IMS_PR_ROAD_CODE == 0)
                        {
                            ModelState.AddModelError("IMS_PR_ROAD_CODE", "Road is Required");
                        }

                    }
                    else
                    {
                        if (ModelState.ContainsKey("IMS_PR_ROAD_CODE"))
                            ModelState["IMS_PR_ROAD_CODE"].Errors.Clear();
                    }



                    if (parametes.AGREEMENT_REQ.Trim() == "Y" && parametes.CON_REQ.Trim() == "Y")
                    {
                        if (model.IMS_AGREEMENT_CODE_C == null || model.IMS_AGREEMENT_CODE_C == 0)
                        {
                            ModelState.AddModelError("IMS_AGREEMENT_CODE_C", "Agreement Name is Required");
                        }
                    }
                    else
                    {

                        if (ModelState.ContainsKey("IMS_AGREEMENT_CODE_C"))
                            ModelState["IMS_AGREEMENT_CODE_C"].Errors.Clear();
                    }

                    if (model.NARRATION_P == null || model.NARRATION_P == String.Empty)
                    {
                        ModelState.AddModelError("NARRATION_P", "Narration is Required");
                    }

                    else
                    {
                        if (ModelState.ContainsKey("NARRATION_P"))
                            ModelState["NARRATION_P"].Errors.Clear();
                    }


                    if (model.HEAD_ID_P == null || model.HEAD_ID_P == String.Empty)
                    {
                        ModelState.AddModelError("HEAD_ID_P", "Sub Transaction Type (Payment) is Required");
                    }
                    else
                    {
                        if (ModelState.ContainsKey("HEAD_ID_P"))
                            ModelState["HEAD_ID_P"].Errors.Clear();
                    }


                    if (model.AMOUNT_Q == null)
                    {
                        ModelState.AddModelError("AMOUNT_Q", " Amount is Required");
                    }
                    else if (model.AMOUNT_Q == 0)
                    {
                        ModelState.AddModelError("AMOUNT_Q", " Amount must be greater than 0");
                    }
                    else if (model.AMOUNT_C != null)
                    {
                        if (model.AMOUNT_Q < model.AMOUNT_C)
                        {
                            ModelState.AddModelError("AMOUNT_Q", " Amount must be greater than cash amount");
                        }

                    }
                    else
                    {
                        if (ModelState.ContainsKey("AMOUNT_Q"))
                            ModelState["AMOUNT_Q"].Errors.Clear();
                    }


                    if (model.AMOUNT_C == null)
                    {
                        ModelState.AddModelError("AMOUNT_C", "Cash Amount is Required");
                    }

                    else
                    {
                        if (ModelState.ContainsKey("AMOUNT_C"))
                            ModelState["AMOUNT_C"].Errors.Clear();
                    }

                    AmountCalculationModel amountModel = new AmountCalculationModel();

                    amountModel = objAuthBAL.GetAuthorizationAmountBalance(Bill_ID);

                    //Cheque amount should be less than equal to diffrence amount for cheque payment 
                    if (model.AMOUNT_Q != null && model.AMOUNT_Q != 0)
                    {
                        if (Convert.ToDecimal(amountModel.DiffChqAmount) < Convert.ToDecimal(model.AMOUNT_Q))
                        {
                            ModelState.AddModelError("AMOUNT_Q", "Amount must be less than or equal to Difference To Be Entered for cheque Amount");
                        }

                    }

                    //cash amount   should be less than equal to difrence amount for cash payment 
                    if (model.AMOUNT_C != null && model.AMOUNT_C != 0)
                    {
                        if (Convert.ToDecimal(amountModel.DiffCachAmount) < Convert.ToDecimal(model.AMOUNT_C))
                        {
                            ModelState.AddModelError("AMOUNT_C", "Cash Amount must be less than or equal to Difference To Be Entered for cash Amount");
                        }
                    }


                    #region remove deduction related errors if any

                    if (ModelState.ContainsKey("HEAD_ID_D"))
                        ModelState["HEAD_ID_D"].Errors.Clear();

                    if (ModelState.ContainsKey("NARRATION_D"))
                        ModelState["NARRATION_D"].Errors.Clear();

                    if (ModelState.ContainsKey("IMS_AGREEMENT_CODE_DED"))
                        ModelState["IMS_AGREEMENT_CODE_DED"].Errors.Clear();

                    if (ModelState.ContainsKey("IMS_AGREEMENT_CODE_DED"))
                        ModelState["IMS_AGREEMENT_CODE_DED"].Errors.Clear();

                    if (ModelState.ContainsKey("AMOUNT_D"))
                        ModelState["AMOUNT_D"].Errors.Clear();

                    #endregion remove deduction related errors if any

                    #endregion

                }

                //save data if model is still valid 
                if (ModelState.IsValid)
                {

                    #region Model Success
                    //get the design params

                    if (masterDetails.AUTH_FINALIZED == "Y")
                    {
                        return Json(new { Success = false, status = "-1" });
                    }


                    if (parametes.SUPPLIER_REQ.Trim() == "Y" || parametes.CON_REQ.Trim() == "Y")
                    {
                        model.MAST_CON_ID = masterDetails.MAST_CON_ID;
                    }
                    else
                    {
                        model.MAST_CON_ID = null;
                    }
                    if (parametes.ROAD_REQ.Trim() == "Y")
                    {
                        model.IMS_PR_ROAD_CODE = model.IMS_PR_ROAD_CODE;
                    }
                    else
                    {
                        model.IMS_PR_ROAD_CODE = null;
                        model.FINAL_PAYMENT = null;
                    }

                    if (parametes.PIU_REQ.Trim() == "Y")
                    {
                        model.MAST_DPIU_CODE = model.MAST_DPIU_CODE;
                    }
                    else
                    {
                        model.MAST_DPIU_CODE = null;
                    }

                    if (parametes.AGREEMENT_REQ.Trim() == "Y" && parametes.CON_REQ.Trim() == "Y")
                    {
                        model.IMS_AGREEMENT_CODE = model.IMS_AGREEMENT_CODE_C;
                    }


                    model.CASH_CHQ = "Q";

                    //for details master contractor id get it from master table (used for opening balance) 
                    model.MAST_CON_ID = masterDetails.MAST_CON_ID;

                    Boolean value = objAuthBAL.AddEditTransactionDeductionPaymentDetails(model, "T", Bill_ID, "A", 0);

                    //if one transaction is successfully entered && multiple transaction is not allowed
                    if (value && obj.MTXN_REQ == "N")
                    {
                        disblehead = true;
                    }

                    return Json(new { Success = value, disblehead = disblehead, head = model.HEAD_ID_P });

                    #endregion
                }
                else
                {
                    #region Model Error

                    //get the master transaction details


                    ViewBag.BillFinalized = masterDetails.AUTH_FINALIZED;
                    objparams.BILL_ID = Bill_ID;
                    objparams.BILL_TYPE = "P";
                    objparams.FUND_TYPE = masterDetails.FUND_TYPE;
                    objparams.LVL_ID = masterDetails.LVL_ID;
                    objparams.ADMIN_ND_CODE = masterDetails.ADMIN_ND_CODE;
                    objparams.MAST_CONT_ID = masterDetails.MAST_CON_ID == null ? 0 : masterDetails.MAST_CON_ID.Value;
                    objparams.DISTRICT_CODE = PMGSYSession.Current.DistrictCode;
                    objparams.TXN_ID = masterDetails.TXN_ID;
                    objparams.STATE_CODE = Convert.ToInt16(PMGSYSession.Current.StateCode);
                    objparams.DISTRICT_CODE = Convert.ToInt16(PMGSYSession.Current.DistrictCode);
                    objparams.SANC_YEAR = Convert.ToInt16(model.IMS_SANCTION_YEAR);
                    objparams.OP_MODE = "A";
                    List<SelectListItem> generalList = new List<SelectListItem>();
                    generalList.Add(new SelectListItem { Selected = true, Text = "--Select--", Value = "" });

                    model.HeadId_P = objCommon.PopulateTransactions(objparams);


                    if (parametes.ROAD_REQ.Trim() == "Y")
                    {
                        model.IMS_PR_ROAD_CODEList = objCommon.PopulateRoad(objparams);
                    }
                    else
                    {
                        model.IMS_PR_ROAD_CODEList = generalList;
                    }

                    if (parametes.PIU_REQ.Trim() == "Y")
                    {
                        model.MAST_DPIU_CODEList = objCommon.PopulateDPIU(objparams);
                    }
                    else
                    {
                        model.MAST_DPIU_CODEList = generalList;
                    }

                    if (parametes.AGREEMENT_REQ.Trim() == "Y" && parametes.CON_REQ.Trim() == "Y")
                    {
                        model.AGREEMENT_C = objCommon.PopulateAgreement(objparams);
                    }
                    else
                    {
                        model.AGREEMENT_C = generalList;

                    }



                    if (parametes.YEAR_REQ.Trim() == "Y")
                    {
                        model.IMS_SANCTION_YEAR_List = objCommon.PopulateSancYear(objparams);
                    }
                    else
                    {
                        generalList.Clear();
                        generalList.Add(new SelectListItem { Selected = true, Text = "--Select--", Value = "0" });
                        model.IMS_SANCTION_YEAR_List = generalList;
                    }
                    if (parametes.PKG_REQ.Trim() == "Y")
                    {
                        model.IMS_SANCTION_PACKAGE_List = objCommon.PopulatePackage(objparams);
                    }
                    else
                    {
                        generalList.Clear();
                        generalList.Add(new SelectListItem { Selected = true, Text = "--Select--", Value = "0" });
                        model.IMS_SANCTION_PACKAGE_List = generalList;
                    }


                    //agreement for deduction
                    ViewBag.DeductionAgreementRequired = (objparams.MAST_CONT_ID == 0) ? false : true;

                    if (ViewBag.DeductionAgreementRequired)
                    {
                        model.AGREEMENT_DED = objCommon.PopulateAgreement(objparams);
                    }
                    else
                    {
                        model.AGREEMENT_DED = generalList;
                    }

                    objparams.BILL_TYPE = "D";

                    model.HeadId_D = objCommon.PopulateTransactions(objparams);

                    if (parametes.AGREEMENT_REQ.Trim() == "Y" && parametes.CON_REQ.Trim() == "Y")
                    {
                        model.IMS_AGREEMENT_CODE = model.IMS_AGREEMENT_CODE_C;
                    }



                    objparams.AGREEMENT_CODE = model.IMS_AGREEMENT_CODE.HasValue ? model.IMS_AGREEMENT_CODE.Value : 0;

                    List<SelectListItem> List = new List<SelectListItem>();

                    List.Add(new SelectListItem { Text = "Yes", Value = "true" });
                    List.Add(new SelectListItem { Selected = true, Text = "No", Value = "false" });

                    model.final_pay = List;

                    ViewBag.disablePaymentHead = disablePayHead;


                    ViewBag.PaymentScreenRequired = masterDetails.CHQ_AMOUNT != 0 ? true : false;
                    ViewBag.DeductionScreenRequired = masterDetails.CASH_AMOUNT != 0 ? true : false;


                    ViewBag.Bill_ID = URLEncrypt.EncryptParameters(new string[] { objparams.BILL_ID.ToString() });

                    return View("AuthDetailsEntryForm", model);

                    #endregion
                }

            }

            catch (DbEntityValidationException ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);

                string exMessege = string.Empty;

                foreach (var eve in ex.EntityValidationErrors)
                {

                    foreach (var ve in eve.ValidationErrors)
                    {

                        exMessege += ve.ErrorMessage + "<br> ";

                    }
                }

                string errorMsg = string.Format("Errors: {0}", exMessege);
                Response.TrySkipIisCustomErrors = true;
                Response.StatusCode = 500;
                Response.Write(errorMsg);
                return Json(string.Empty);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
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


        /// <summary>
        /// Action to Add the deduction transaction details of authorization
        /// </summary>
        /// <param name="model"></param>
        /// <param name="parameter"></param>
        /// <param name="hash"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        [HttpPost]
        [Audit]
         [ValidateAntiForgeryToken]
        public ActionResult AddDeductionTransactionDetails(AuthorizationRequestDetailsModel model, String parameter, String hash, String key)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            objCommon = new CommonFunctions();
            List<SelectListItem> generalList = new List<SelectListItem>();
            generalList.Add(new SelectListItem { Selected = true, Text = "--Select--", Value = "" });

            try
            {

                TransactionParams objparams = new TransactionParams();

                Int64 Bill_ID = 0;

                if (!String.IsNullOrEmpty(parameter) && !String.IsNullOrEmpty(hash) && !String.IsNullOrEmpty(key))
                {
                    String[] urlParams = URLEncrypt.DecryptParameters(new String[] { parameter, hash, key });
                    if (urlParams.Length >= 1)
                    {
                        String[] urlSplitParams = urlParams[0].Split('$');
                        Bill_ID = Convert.ToInt64(urlSplitParams[0]);
                    }
                }



                //get the contractor details from master details 
                ACC_AUTH_REQUEST_MASTER masterDetails = new ACC_AUTH_REQUEST_MASTER();
                masterDetails = objAuthBAL.GetMasterAuthorizationDetails(Bill_ID);

                #region if for master payment multiple transsaction is not allowed

                ACC_SCREEN_DESIGN_PARAM_MASTER obj = new ACC_SCREEN_DESIGN_PARAM_MASTER();

                TransactionParams objparams1 = new TransactionParams();

                objparams1.TXN_ID = masterDetails.TXN_ID;

                obj = objCommon.getMasterDesignParam(objparams1);

                bool disablePayHead = false;

                if (obj.MTXN_REQ == "N")
                {
                    if (!dbContext.ACC_AUTH_REQUEST_DETAILS.Where(d => d.AUTH_ID == masterDetails.AUTH_ID).Any())
                    {
                        disablePayHead = false;
                    }
                    else
                    {
                        Int16? txn = dbContext.ACC_AUTH_REQUEST_DETAILS.Where(d => d.AUTH_ID == masterDetails.AUTH_ID).Select(c => c.TXN_ID).First();
                        string CheCash = dbContext.ACC_MASTER_TXN.Where(m => m.TXN_ID == txn).Select(m => m.CASH_CHQ).FirstOrDefault().Trim();
                        model.HEAD_ID_P = txn + "$" + CheCash;
                        disablePayHead = true;
                    }
                }
                //to disable on error in javascript file
                ViewBag.disablePaymentHead = disablePayHead;

                bool disblehead = false;

                disblehead = disablePayHead;

                #endregion


                #region dynamic validations

                if (ModelState.IsValid)
                {

                    if (model.AMOUNT_D == null)
                    {
                        ModelState.AddModelError("AMOUNT_D", "Deduction Amount is Required");
                    }
                    else if (model.AMOUNT_D == 0)
                    {
                        ModelState.AddModelError("AMOUNT_D", "Deduction Amount must be greater than 0");
                    }
                    else
                    {
                        if (ModelState.ContainsKey("AMOUNT_D"))
                            ModelState["AMOUNT_D"].Errors.Clear();
                    }


                    if (model.NARRATION_D == null || model.NARRATION_D == String.Empty)
                    {
                        ModelState.AddModelError("NARRATION_D", "Narration is Required");
                    }

                    else
                    {
                        if (ModelState.ContainsKey("NARRATION_D"))
                            ModelState["NARRATION_D"].Errors.Clear();
                    }

                    if (model.HEAD_ID_D == null || model.HEAD_ID_D == String.Empty)
                    {
                        ModelState.AddModelError("HEAD_ID_D", "Sub Transaction Type (Deduction)");
                    }

                    else
                    {
                        if (ModelState.ContainsKey("HEAD_ID_D"))
                            ModelState["HEAD_ID_D"].Errors.Clear();
                    }

                    int ContractorSupplierRequired = masterDetails.MAST_CON_ID == null ? 0 : masterDetails.MAST_CON_ID.Value;
                    if (ContractorSupplierRequired != 0)
                    {
                        if (model.IMS_AGREEMENT_CODE_DED == 0)
                        {
                            ModelState.AddModelError("IMS_AGREEMENT_CODE_DED", "Agreement Name is Required");
                        }
                        else
                        {
                            if (ModelState.ContainsKey("IMS_AGREEMENT_CODE_DED"))
                                ModelState["IMS_AGREEMENT_CODE_DED"].Errors.Clear();
                        }
                    }
                    else
                    {
                        if (ModelState.ContainsKey("IMS_AGREEMENT_CODE_DED"))
                            ModelState["IMS_AGREEMENT_CODE_DED"].Errors.Clear();
                    }

                    if (model.AMOUNT_D != null && model.AMOUNT_D != 0)
                    {

                        AmountCalculationModel amountModel = new AmountCalculationModel();

                        amountModel = objAuthBAL.GetAuthorizationAmountBalance(Bill_ID);

                        if (Convert.ToDecimal(amountModel.DiffDedAmount) < Convert.ToDecimal(model.AMOUNT_D))
                        {
                            ModelState.AddModelError("AMOUNT_D", "Deduction Amount must be less than or equal to Difference To Be Entered for Deduction Amount");
                        }

                    }


                }
                #endregion

                if (ModelState.IsValid)
                {

                    if (masterDetails.AUTH_FINALIZED == "Y")
                    {
                        return Json(new { Success = false, status = "-1" });
                    }


                    model.MAST_CON_ID = masterDetails.MAST_CON_ID;


                    Boolean value = objAuthBAL.AddEditTransactionDeductionPaymentDetails(model, "D", Bill_ID, "A", 0);

                    ModelState.Clear();

                    return Json(new { Success = value });

                }
                else
                {
                    ViewBag.BillFinalized = masterDetails.AUTH_FINALIZED;
                    objparams.BILL_ID = Bill_ID;
                    objparams.BILL_TYPE = "P";
                    objparams.FUND_TYPE = masterDetails.FUND_TYPE;
                    objparams.LVL_ID = masterDetails.LVL_ID;
                    objparams.ADMIN_ND_CODE = masterDetails.ADMIN_ND_CODE;
                    objparams.MAST_CONT_ID = masterDetails.MAST_CON_ID == null ? 0 : masterDetails.MAST_CON_ID.Value; 
                    objparams.DISTRICT_CODE = PMGSYSession.Current.DistrictCode;
                    objparams.TXN_ID = masterDetails.TXN_ID;
                    objparams.STATE_CODE = Convert.ToInt16(PMGSYSession.Current.StateCode);
                    objparams.DISTRICT_CODE = Convert.ToInt16(PMGSYSession.Current.DistrictCode);
                    objparams.SANC_YEAR = Convert.ToInt16(model.IMS_SANCTION_YEAR);
                    objparams.OP_MODE = "A";


                    model.AGREEMENT_C = generalList;

                    model.AGREEMENT_DED = objCommon.PopulateAgreement(objparams);

                    model.IMS_PR_ROAD_CODEList = generalList;

                    model.MAST_DPIU_CODEList = generalList;

                    generalList.Clear();
                    generalList.Add(new SelectListItem { Selected = true, Text = "--Select--", Value = "0" });

                    model.IMS_SANCTION_YEAR_List = generalList;

                    model.IMS_SANCTION_PACKAGE_List = generalList;

                    model.HeadId_P = objCommon.PopulateTransactions(objparams);

                    objparams.BILL_TYPE = "D";

                    model.HeadId_D = objCommon.PopulateTransactions(objparams);

                    objparams.AGREEMENT_CODE = model.IMS_AGREEMENT_CODE.HasValue ? model.IMS_AGREEMENT_CODE.Value : 0;

                    List<SelectListItem> List = new List<SelectListItem>();

                    List.Add(new SelectListItem { Text = "Yes", Value = "true" });
                    List.Add(new SelectListItem { Selected = true, Text = "No", Value = "false" });

                    model.final_pay = List;

                    ViewBag.disablePaymentHead = disablePayHead;
                    //agreement for deduction
                    ViewBag.DeductionAgreementRequired = objparams.MAST_CONT_ID == 0 ? false : true;


                    ViewBag.PaymentScreenRequired = masterDetails.CHQ_AMOUNT != 0 ? true : false;
                    ViewBag.DeductionScreenRequired = masterDetails.CASH_AMOUNT != 0 ? true : false;

                    ViewBag.Bill_ID = URLEncrypt.EncryptParameters(new string[] { objparams.BILL_ID.ToString() });

                    return View("AuthDetailsEntryForm", model);
                }

            }

            catch (DbEntityValidationException ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);

                string exMessege = string.Empty;

                foreach (var eve in ex.EntityValidationErrors)
                {

                    foreach (var ve in eve.ValidationErrors)
                    {

                        exMessege += ve.ErrorMessage + "<br> ";

                    }
                }

                string errorMsg = string.Format("Errors: {0}", exMessege);
                Response.TrySkipIisCustomErrors = true;
                Response.StatusCode = 500;
                Response.Write(errorMsg);
                return Json(string.Empty);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
          

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


        /// <summary>
        /// function to delete the transaction payment and deduction details
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="hash"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        [HttpPost]
         [Audit]
        public JsonResult DeleteTransactionPaymentDetails(String parameter, String hash, String key)
        {
            Int64 master_Bill_Id = 0;
            Int16 tranNumber = 0;
            String paymentDeduction = string.Empty;
            Int32 value = 0;
            try
            {
                if (!String.IsNullOrEmpty(parameter) && !String.IsNullOrEmpty(hash) && !String.IsNullOrEmpty(key))
                {
                    String[] urlParams = URLEncrypt.DecryptParameters(new String[] { parameter, hash, key });
                    if (urlParams.Length >= 1)
                    {
                        String[] urlSplitParams = urlParams[0].Split('$');
                        master_Bill_Id = Convert.ToInt64(urlSplitParams[0]);
                        tranNumber = Convert.ToInt16(urlSplitParams[1]);
                        paymentDeduction = urlSplitParams[2].Trim();

                        value = objAuthBAL.DeleteAuthorizationTransDetails(master_Bill_Id, tranNumber, paymentDeduction);


                        return Json(new
                        {
                            Success = value,
                            TransactionType = paymentDeduction == "D" ? "D" : "P",
                            master_Bill_Id = URLEncrypt.EncryptParameters(new string[] { master_Bill_Id.ToString() })
                        });

                    }
                    else
                    {
                        throw new Exception("Error while getting  transaction to delete");
                    }
                }
                else
                {
                    throw new Exception("Error while getting  transaction to delete");
                }

            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
          
                string errorMsg = string.Format("Errors: {0}", ex.Message);
                Response.TrySkipIisCustomErrors = true;
                Response.StatusCode = 500;
                Response.Write(errorMsg);
                return Json(string.Empty);
            }

        }


        /// <summary>
        /// function to edit the authorization transaction details (payment and deduction)
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="hash"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        [HttpGet]
        [Audit]
         //[ValidateAntiForgeryToken]
        public ActionResult EditTransactionDetails(String parameter, String hash, String key)
        {

            ACC_SCREEN_DESIGN_PARAM_DETAILS parametes = new ACC_SCREEN_DESIGN_PARAM_DETAILS();
            Int16 tranNumber = 0;
            String paymentDeduction = string.Empty;
            objCommon = new CommonFunctions();
            PMGSYEntities dbContext = new PMGSYEntities();
            TransactionParams objparams = new TransactionParams();
            AuthorizationRequestDetailsModel model = new AuthorizationRequestDetailsModel();
            List<SelectListItem> generalList = new List<SelectListItem>();
            generalList.Add(new SelectListItem { Selected = true, Text = "--Select--", Value = "" });

            try
            {


                if (!String.IsNullOrEmpty(parameter) && !String.IsNullOrEmpty(hash) && !String.IsNullOrEmpty(key))
                {
                    String[] urlParams = URLEncrypt.DecryptParameters(new String[] { parameter, hash, key });
                    if (urlParams.Length >= 1)
                    {
                        String[] urlSplitParams = urlParams[0].Split('$');
                        objparams.BILL_ID = Convert.ToInt64(urlSplitParams[0]);
                        tranNumber = Convert.ToInt16(urlSplitParams[1]);
                        paymentDeduction = urlSplitParams[2].Trim();

                    }
                }
                else
                {
                    throw new Exception("Error while getting payment details..");
                }

                //get the master transaction details
                ACC_AUTH_REQUEST_MASTER masterDetails = new ACC_AUTH_REQUEST_MASTER();
                masterDetails = objAuthBAL.GetMasterAuthorizationDetails(objparams.BILL_ID);

                //get the transaction Details
                ACC_AUTH_REQUEST_DETAILS transactionDetails = new ACC_AUTH_REQUEST_DETAILS();
                transactionDetails = objAuthBAL.GetAuthTransactionDetails(objparams.BILL_ID, tranNumber);


                objparams.BILL_TYPE = "P";
                objparams.FUND_TYPE = masterDetails.FUND_TYPE;
                objparams.LVL_ID = masterDetails.LVL_ID;
                objparams.MAST_CONT_ID = masterDetails.MAST_CON_ID == null ? 0 : masterDetails.MAST_CON_ID.Value; 
                objparams.DISTRICT_CODE = PMGSYSession.Current.DistrictCode;
                objparams.TXN_ID = masterDetails.TXN_ID;
                objparams.STATE_CODE = Convert.ToInt16(PMGSYSession.Current.StateCode);
                objparams.DISTRICT_CODE = Convert.ToInt16(PMGSYSession.Current.DistrictCode);
                objparams.SANC_YEAR = Convert.ToInt16(model.IMS_SANCTION_YEAR);
                objparams.OP_MODE = "E";

                model.CONTRACTOR_ID = masterDetails.MAST_CON_ID.HasValue ? masterDetails.MAST_CON_ID.Value : 0;

                model.HeadId_P = objCommon.PopulateTransactions(objparams);

                //agreement for deduction

                model.AGREEMENT_DED = objCommon.PopulateAgreement(objparams);

                //find out for deduction
                objparams.BILL_TYPE = "D";

                model.HeadId_D = objCommon.PopulateTransactions(objparams);

                

                objparams.AGREEMENT_CODE = transactionDetails.IMS_AGREEMENT_CODE.HasValue ? transactionDetails.IMS_AGREEMENT_CODE.Value : 0;

                model.IMS_PR_ROAD_CODE = transactionDetails.IMS_PR_ROAD_CODE;

                //new change done by Vikram for populating Road according to the Maintenance Agreement and Contractor

                if (PMGSYSession.Current.FundType == "M")
                {
                    //old
                    //objparams.AGREEMENT_NUMBER = dbContext.MANE_IMS_CONTRACT.Where(m => m.MANE_PR_CONTRACT_CODE == objparams.AGREEMENT_CODE).Select(m => m.MANE_AGREEMENT_NUMBER).FirstOrDefault();
                    //Modified By Abhishek kamble to get Agr No using MANE_CONT_ID 17Nov2014 
                    objparams.AGREEMENT_NUMBER = dbContext.MANE_IMS_CONTRACT.Where(m => m.MANE_CONTRACT_ID == objparams.AGREEMENT_CODE).Select(m => m.MANE_AGREEMENT_NUMBER).FirstOrDefault();

                }

                if (paymentDeduction != "D")
                {

                    objparams.TXN_ID = Convert.ToInt16(transactionDetails.TXN_ID);

                    parametes = objCommon.getDetailsDesignParam(objparams);



                    if (parametes.PIU_REQ.Trim() == "Y")
                    {
                        model.MAST_DPIU_CODEList = objCommon.PopulateDPIU(objparams);
                    }
                    else
                    {
                        model.MAST_DPIU_CODEList = generalList;
                    }

                    if (parametes.AGREEMENT_REQ.Trim() == "Y" && parametes.CON_REQ.Trim() == "Y")
                    {
                        model.AGREEMENT_C = objCommon.PopulateAgreement(objparams);
                    }
                    else
                    {
                        model.AGREEMENT_C = generalList;

                    }



                    if (parametes.YEAR_REQ.Trim() == "Y")
                    {

                        model.IMS_SANCTION_YEAR = objCommon.getSancYearFromRoad(model.IMS_PR_ROAD_CODE.HasValue ? model.IMS_PR_ROAD_CODE.Value : 0);

                        model.IMS_SANCTION_YEAR_List = objCommon.PopulateSancYear(objparams);
                    }
                    else
                    {
                        generalList.Clear();
                        generalList.Add(new SelectListItem { Selected = true, Text = "--Select--", Value = "0" });
                        model.IMS_SANCTION_YEAR_List = generalList;
                    }
                    if (parametes.PKG_REQ.Trim() == "Y")
                    {
                        model.IMS_SANCTION_PACKAGE = objCommon.getPackageFromRoad(model.IMS_PR_ROAD_CODE.HasValue ? model.IMS_PR_ROAD_CODE.Value : 0);
                        objparams.SANC_YEAR = Convert.ToInt16(model.IMS_SANCTION_YEAR);
                        model.IMS_SANCTION_PACKAGE_List = objCommon.PopulatePackage(objparams);
                    }
                    else
                    {
                        generalList.Clear();
                        generalList.Add(new SelectListItem { Selected = true, Text = "--Select--", Value = "0" });
                        model.IMS_SANCTION_PACKAGE_List = generalList;
                    }
                    if (parametes.ROAD_REQ.Trim() == "Y")
                    {
                        objparams.PACKAGE_ID = model.IMS_SANCTION_PACKAGE;
                        model.IMS_PR_ROAD_CODEList = objCommon.PopulateRoad(objparams);
                    }
                    else
                    {
                        model.IMS_PR_ROAD_CODEList = generalList;
                    }

                }
                else
                {

                    model.IMS_PR_ROAD_CODEList = generalList;
                    model.MAST_DPIU_CODEList = generalList;
                    model.AGREEMENT_C = generalList;
                    generalList.Clear();
                    generalList.Add(new SelectListItem { Selected = true, Text = "--Select--", Value = "0" });
                    model.IMS_SANCTION_YEAR_List = generalList;
                    model.IMS_SANCTION_PACKAGE_List = generalList;

                }


                if (paymentDeduction.Equals("D"))
                {
                    model.HEAD_ID_D = transactionDetails.TXN_ID + "$" + dbContext.ACC_MASTER_TXN.Where(m => m.TXN_ID == transactionDetails.TXN_ID).Select(m => m.CASH_CHQ).FirstOrDefault().Trim();
                    model.NARRATION_D = transactionDetails.NARRATION;
                    model.AMOUNT_D = transactionDetails.AMOUNT;
                    model.IMS_AGREEMENT_CODE_DED = transactionDetails.IMS_AGREEMENT_CODE;
                }
                else
                {
                    model.IMS_AGREEMENT_CODE_C = transactionDetails.IMS_AGREEMENT_CODE;

                    model.IMS_PR_ROAD_CODE = transactionDetails.IMS_PR_ROAD_CODE;

                    model.HEAD_ID_P = transactionDetails.TXN_ID + "$" + dbContext.ACC_MASTER_TXN.Where(m => m.TXN_ID == transactionDetails.TXN_ID).Select(m => m.CASH_CHQ).FirstOrDefault().Trim();
                    model.AMOUNT_Q = transactionDetails.AMOUNT;
                    model.FINAL_PAYMENT = transactionDetails.FINAL_PAYMENT;
                    model.NARRATION_P = transactionDetails.NARRATION;

                    if (masterDetails.CASH_AMOUNT != 0)
                    {
                        ACC_AUTH_REQUEST_DETAILS cashTransactionDetails = new ACC_AUTH_REQUEST_DETAILS();
                        cashTransactionDetails = objAuthBAL.GetAuthTransactionDetails(objparams.BILL_ID, Convert.ToInt16(tranNumber + 1));

                        if (cashTransactionDetails != null)
                        {
                            model.AMOUNT_C = cashTransactionDetails.AMOUNT;
                        }
                    }
                }

                //check for final payment 
                List<SelectListItem> List = new List<SelectListItem>();
                if (model.FINAL_PAYMENT.HasValue)
                {
                    if (model.FINAL_PAYMENT.Value)
                    {
                        List.Add(new SelectListItem { Selected = true, Text = "Yes", Value = "true" });
                    }
                    else
                    {

                        List.Add(new SelectListItem { Text = "Yes", Value = "true" });
                        List.Add(new SelectListItem { Selected = true, Text = "No", Value = "false" });
                    }

                }
                else
                {

                    List.Add(new SelectListItem { Text = "Yes", Value = "true" });
                    List.Add(new SelectListItem { Selected = true, Text = "No", Value = "false" });
                }

                model.final_pay = List;


                ViewBag.urlparams = URLEncrypt.EncryptParameters(new string[] { objparams.BILL_ID.ToString() + "$" + tranNumber + "$" + paymentDeduction });

                ViewBag.BillFinalized = masterDetails.AUTH_FINALIZED;

                ViewBag.Bill_ID = URLEncrypt.EncryptParameters(new string[] { objparams.BILL_ID.ToString() });


                #region if for master payment multiple transsaction is not allowed

                ACC_SCREEN_DESIGN_PARAM_MASTER obj = new ACC_SCREEN_DESIGN_PARAM_MASTER();

                //get master txn id
                objparams.TXN_ID = masterDetails.TXN_ID;

                obj = objCommon.getMasterDesignParam(objparams);

                bool disablePayHead = false;

                if (obj.MTXN_REQ == "N")
                {
                    if (!dbContext.ACC_AUTH_REQUEST_DETAILS.Where(d => d.AUTH_ID == masterDetails.AUTH_ID).Any())
                    {
                        disablePayHead = false;
                    }
                    else
                    {
                        Int16? txn = dbContext.ACC_AUTH_REQUEST_DETAILS.Where(d => d.AUTH_ID == masterDetails.AUTH_ID).Select(c => c.TXN_ID).First();
                        string CheCash = dbContext.ACC_MASTER_TXN.Where(m => m.TXN_ID == txn).Select(m => m.CASH_CHQ).FirstOrDefault().Trim();
                        model.HEAD_ID_P = txn + "$" + CheCash;
                        disablePayHead = true;
                    }
                }
                //to disable on header
                ViewBag.disablePaymentHead = disablePayHead;
                ViewBag.PaymentScreenRequired = masterDetails.CHQ_AMOUNT != 0 ? true : false;
                ViewBag.DeductionScreenRequired = masterDetails.CASH_AMOUNT != 0 ? true : false;

                #endregion

                return View("AuthDetailsEntryForm", model);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
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


        /// <summary>
        /// action to edit the authorization transaction and deduction details
        /// </summary>
        /// <param name="model"></param>
        /// <param name="parameter"></param>
        /// <param name="hash"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        [HttpPost]
         [Audit]
        public ActionResult PostEditTransactionDetails(AuthorizationRequestDetailsModel model, String parameter, String hash, String key)
        {

            Int16 tranNumber = 0;
            String paymentDeduction = string.Empty;
            PMGSYEntities dbContext = new PMGSYEntities();
            TransactionParams objparams = new TransactionParams();
            ACC_SCREEN_DESIGN_PARAM_DETAILS parametes = new ACC_SCREEN_DESIGN_PARAM_DETAILS();
            objCommon = new CommonFunctions();
            try
            {

                if (!String.IsNullOrEmpty(parameter) && !String.IsNullOrEmpty(hash) && !String.IsNullOrEmpty(key))
                {
                    String[] urlParams = URLEncrypt.DecryptParameters(new String[] { parameter, hash, key });
                    if (urlParams.Length >= 1)
                    {
                        String[] urlSplitParams = urlParams[0].Split('$');
                        objparams.BILL_ID = Convert.ToInt64(urlSplitParams[0]);
                        tranNumber = Convert.ToInt16(urlSplitParams[1]);
                        paymentDeduction = urlSplitParams[2].Trim();

                    }
                }
                else
                {
                    throw new Exception("Error while getting payment details..");
                }

                //new change done by Vikram on 20-Dec-2013
                if (model.HEAD_ID_P != null)
                {
                    if (Convert.ToInt32(model.HEAD_ID_P.Split('$')[0]) == 48 || Convert.ToInt32(model.HEAD_ID_P.Split('$')[0]) == 49)
                    {
                        if (model.IMS_PR_ROAD_CODE != null)
                        {
                            if (!ValidateRoad(Convert.ToInt32(model.HEAD_ID_P.Split('$')[0]), model.IMS_PR_ROAD_CODE.Value, PMGSYSession.Current.FundType))
                            {
                                return Json(new { Success = false, status = "-555", message = "Select Upgrade/New road as per account head." });
                            }
                        }
                    }
                      
                    //New validation to validate PMGSY scheme Roads based on Head. start 2 Sep 2014
                    if (PMGSYSession.Current.FundType == "P" && model.IMS_PR_ROAD_CODE != null && model.IMS_PR_ROAD_CODE != 0)
                    {
                        if (!ValidateRoadForPMGSYScheme(Convert.ToInt16(model.HEAD_ID_P.Split('$')[0]), model.IMS_PR_ROAD_CODE.Value))
                        {
                            return Json(new { Success = false, status = "-999", message = "Road can not be Selected, Since it is in different Scheme." });
                        }
                    }
                    //New validation to validate PMGSY scheme Roads based on Head. end
                }
                //end of change


                //get the master transaction details
                ACC_AUTH_REQUEST_MASTER masterDetails = new ACC_AUTH_REQUEST_MASTER();
                masterDetails = objAuthBAL.GetMasterAuthorizationDetails(objparams.BILL_ID);


                //get the transaction Details for cheque entry
                ACC_AUTH_REQUEST_DETAILS transactionDetails = new ACC_AUTH_REQUEST_DETAILS();
                transactionDetails = objAuthBAL.GetAuthTransactionDetails(objparams.BILL_ID, tranNumber);

                List<SelectListItem> generalList = new List<SelectListItem>();
                generalList.Add(new SelectListItem { Selected = true, Text = "--Select--", Value = "" });

                #region if for master payment multiple transsaction is not allowed

                ACC_SCREEN_DESIGN_PARAM_MASTER obj = new ACC_SCREEN_DESIGN_PARAM_MASTER();

                objparams.TXN_ID = masterDetails.TXN_ID;

                obj = objCommon.getMasterDesignParam(objparams);

                bool disablePayHead = false;

                if (obj.MTXN_REQ == "N")
                {
                    if (!dbContext.ACC_AUTH_REQUEST_DETAILS.Where(d => d.AUTH_ID == masterDetails.AUTH_ID).Any())
                    {
                        disablePayHead = false;
                    }
                    else
                    {
                        Int16? txn = dbContext.ACC_AUTH_REQUEST_DETAILS.Where(d => d.AUTH_ID == masterDetails.AUTH_ID).Select(c => c.TXN_ID).First();
                        string CheCash = dbContext.ACC_MASTER_TXN.Where(m => m.TXN_ID == txn).Select(m => m.CASH_CHQ).FirstOrDefault().Trim();
                        model.HEAD_ID_P = txn + "$" + CheCash;
                        disablePayHead = true;
                    }
                }
                //to disable on error
                ViewBag.disablePaymentHead = disablePayHead;

                bool disblehead = false;

                disblehead = disablePayHead;

                #endregion


                //if no other model errors are there then only check for dynamic validation
                if (ModelState.IsValid)
                {
                    if (!paymentDeduction.Equals("D"))
                    {
                        #region serverside validation for payment

                        objparams.TXN_ID = Convert.ToInt16(model.HEAD_ID_P.Split('$')[0]);

                        parametes = objCommon.getDetailsDesignParam(objparams);

                        if (parametes.ROAD_REQ.Trim() == "Y")
                        {

                            if (model.IMS_PR_ROAD_CODE == null || model.IMS_PR_ROAD_CODE == 0)
                            {
                                ModelState.AddModelError("IMS_PR_ROAD_CODE", "Road is Required");
                            }

                        }
                        else
                        {
                            if (ModelState.ContainsKey("IMS_PR_ROAD_CODE"))
                                ModelState["IMS_PR_ROAD_CODE"].Errors.Clear();
                        }


                        if (parametes.AGREEMENT_REQ.Trim() == "Y" && parametes.CON_REQ.Trim() == "Y")
                        {
                            if (model.IMS_AGREEMENT_CODE_C == null || model.IMS_AGREEMENT_CODE_C == 0)
                            {
                                ModelState.AddModelError("IMS_AGREEMENT_CODE_C", "Agreement Name is Required");
                            }
                        }
                        else
                        {

                            if (ModelState.ContainsKey("IMS_AGREEMENT_CODE_C"))
                                ModelState["IMS_AGREEMENT_CODE_C"].Errors.Clear();
                        }

                        if (parametes.AGREEMENT_REQ.Trim() == "Y" && parametes.SUPPLIER_REQ.Trim() == "Y")
                        {

                            if (model.IMS_AGREEMENT_CODE_C == null || model.IMS_AGREEMENT_CODE_C == 0)
                            {
                                ModelState.AddModelError("IMS_AGREEMENT_CODE_S", "Agreement Name is Required");
                            }
                        }
                        else
                        {
                            if (ModelState.ContainsKey("IMS_AGREEMENT_CODE_S"))
                                ModelState["IMS_AGREEMENT_CODE_S"].Errors.Clear();

                        }

                        if (model.NARRATION_P == null || model.NARRATION_P == String.Empty)
                        {
                            ModelState.AddModelError("NARRATION_P", "Narration is Required");
                        }

                        else
                        {
                            if (ModelState.ContainsKey("NARRATION_P"))
                                ModelState["NARRATION_P"].Errors.Clear();
                        }


                        if (model.HEAD_ID_P == null || model.HEAD_ID_P == String.Empty)
                        {
                            ModelState.AddModelError("HEAD_ID_P", "Sub Transaction Type (Payment) is Required");
                        }
                        else
                        {
                            if (ModelState.ContainsKey("HEAD_ID_P"))
                                ModelState["HEAD_ID_P"].Errors.Clear();
                        }


                        if (model.AMOUNT_Q == null)
                        {
                            ModelState.AddModelError("AMOUNT_Q", "Amount is Required");
                        }
                        else if (model.AMOUNT_Q == 0)
                        {
                            ModelState.AddModelError("AMOUNT_Q", " Amount must be greater than 0");
                        }
                        else if (model.AMOUNT_C != null)
                        {
                            if (model.AMOUNT_Q < model.AMOUNT_C)
                            {
                                ModelState.AddModelError("AMOUNT_Q", " Amount must be greater than cash amount");
                            }

                        }
                        else
                        {
                            if (ModelState.ContainsKey("AMOUNT_Q"))
                                ModelState["AMOUNT_Q"].Errors.Clear();
                        }


                        if (model.AMOUNT_C == null)
                        {
                            ModelState.AddModelError("AMOUNT_C", "Cash Amount is Required");
                        }

                        else
                        {
                            if (ModelState.ContainsKey("AMOUNT_C"))
                                ModelState["AMOUNT_C"].Errors.Clear();
                        }

                        AmountCalculationModel amountModel = new AmountCalculationModel();

                        amountModel = objAuthBAL.GetAuthorizationAmountBalance(objparams.BILL_ID);

                        //Cheque amount should be less than equal to diffrence amount for cheque payment 

                        if (model.AMOUNT_Q != null && model.AMOUNT_Q != 0)
                        {

                            Decimal TotalAmount = amountModel.DiffChqAmount + transactionDetails.AMOUNT;

                            if (transactionDetails.AMOUNT >= model.AMOUNT_Q.Value)
                            {
                            }
                            else
                            {
                                if (TotalAmount < model.AMOUNT_Q.Value)
                                {
                                    ModelState.AddModelError("AMOUNT_Q", "Invalid  Amount.its greater than remaining cheque amount to enter");
                                }
                                else if (TotalAmount >= model.AMOUNT_Q.Value)
                                {
                                }
                                else
                                {
                                    ModelState.AddModelError("AMOUNT_Q", "Invalid  Amount.its greater than remaining cheque amount to enter");
                                }

                            }

                        }

                        //cash amount   should be less than equal to difrence amount for cash payment 
                        if (model.AMOUNT_C != null && model.AMOUNT_C != 0)
                        {
                            //get the details for cash amount from bill details 
                            ACC_AUTH_REQUEST_DETAILS transactionDetailsCash = new ACC_AUTH_REQUEST_DETAILS();
                            transactionDetailsCash = objAuthBAL.GetAuthTransactionDetails(objparams.BILL_ID, Convert.ToInt16(tranNumber + 1));

                            if (transactionDetailsCash != null)
                            {
                                Decimal TotalAmount = amountModel.DiffCachAmount + transactionDetailsCash.AMOUNT;

                                if (transactionDetailsCash.AMOUNT >= model.AMOUNT_Q.Value)
                                {
                                }
                                else
                                {
                                    if (TotalAmount < model.AMOUNT_C.Value)
                                    {
                                        ModelState.AddModelError("AMOUNT_C", "Invalid cash Amount.its greater than remaining cash amount to enter");
                                    }
                                    else if (TotalAmount >= model.AMOUNT_C.Value)
                                    {
                                    }
                                    else
                                    {
                                        ModelState.AddModelError("AMOUNT_C", "Invalid cash Amount.its greater than remaining cash amount to enter");
                                    }

                                }
                            }
                        }


                        #region remove deduction related errors if any

                        if (ModelState.ContainsKey("HEAD_ID_D"))
                            ModelState["HEAD_ID_D"].Errors.Clear();

                        if (ModelState.ContainsKey("NARRATION_D"))
                            ModelState["NARRATION_D"].Errors.Clear();

                        if (ModelState.ContainsKey("IMS_AGREEMENT_CODE_DED"))
                            ModelState["IMS_AGREEMENT_CODE_DED"].Errors.Clear();

                        if (ModelState.ContainsKey("IMS_AGREEMENT_CODE_DED"))
                            ModelState["IMS_AGREEMENT_CODE_DED"].Errors.Clear();

                        if (ModelState.ContainsKey("AMOUNT_D"))
                            ModelState["AMOUNT_D"].Errors.Clear();

                        #endregion remove deduction related errors if any

                        #endregion

                    }
                    else
                    {

                        #region server validation for deduction

                        if (model.AMOUNT_D == null)
                        {
                            ModelState.AddModelError("AMOUNT_D", "Deduction Amount is Required");
                        }
                        else if (model.AMOUNT_D == 0)
                        {
                            ModelState.AddModelError("AMOUNT_D", "Deduction Amount must be greater than 0");
                        }
                        else
                        {
                            if (ModelState.ContainsKey("AMOUNT_D"))
                                ModelState["AMOUNT_D"].Errors.Clear();
                        }

                        if (model.NARRATION_D == null || model.NARRATION_D == String.Empty)
                        {
                            ModelState.AddModelError("NARRATION_D", "Narration is Required");
                        }

                        else
                        {
                            if (ModelState.ContainsKey("NARRATION_D"))
                                ModelState["NARRATION_D"].Errors.Clear();
                        }

                        if (model.HEAD_ID_D == null || model.HEAD_ID_D == String.Empty)
                        {
                            ModelState.AddModelError("HEAD_ID_D", "Sub Transaction Type (Deduction)");
                        }

                        else
                        {
                            if (ModelState.ContainsKey("HEAD_ID_D"))
                                ModelState["HEAD_ID_D"].Errors.Clear();
                        }

                        int ContractorSupplierRequired = masterDetails.MAST_CON_ID == null ? 0 : masterDetails.MAST_CON_ID.Value; 
                        if (ContractorSupplierRequired != 0)
                        {
                            if (model.IMS_AGREEMENT_CODE_DED == 0)
                            {
                                ModelState.AddModelError("IMS_AGREEMENT_CODE_DED", "Agreement Name is Required");
                            }
                            else
                            {
                                if (ModelState.ContainsKey("IMS_AGREEMENT_CODE_DED"))
                                    ModelState["IMS_AGREEMENT_CODE_DED"].Errors.Clear();
                            }
                        }
                        else
                        {
                            if (ModelState.ContainsKey("IMS_AGREEMENT_CODE_DED"))
                                ModelState["IMS_AGREEMENT_CODE_DED"].Errors.Clear();
                        }

                        if (model.AMOUNT_D != null && model.AMOUNT_D != 0)
                        {

                            AmountCalculationModel amountModel = new AmountCalculationModel();

                            amountModel = objAuthBAL.GetAuthorizationAmountBalance(objparams.BILL_ID);

                            Decimal TotalAmount = amountModel.DiffDedAmount + transactionDetails.AMOUNT;

                            if (transactionDetails.AMOUNT >= model.AMOUNT_D.Value)
                            {
                            }
                            else
                            {
                                if (TotalAmount < model.AMOUNT_D.Value)
                                {
                                    ModelState.AddModelError("AMOUNT_D", "Invalid Deduction Amount.its greater than remaining Deduction amount to enter");
                                }
                                else if (TotalAmount >= model.AMOUNT_D.Value)
                                {
                                }
                                else
                                {
                                    ModelState.AddModelError("AMOUNT_D", "Invalid Deduction Amount.its greater than remaining Deduction amount to enter");
                                }

                            }
                        }

                        #endregion
                    }
                }

                if (model.IMS_PR_ROAD_CODE == 0)
                {
                    model.IMS_PR_ROAD_CODE = null;
                }

                if (ModelState.IsValid)
                {
                    #region valid model

                    if (masterDetails.AUTH_FINALIZED == "Y")
                    {

                        return Json(new { Success = false, Bill_ID = "-1", disblehead = disblehead, head = model.HEAD_ID_P });

                    }

                    if (!paymentDeduction.Equals("D"))
                    {
                        objparams.TXN_ID = Convert.ToInt16(model.HEAD_ID_P.Split('$')[0]);

                        parametes = objCommon.getDetailsDesignParam(objparams);


                        if (parametes.SUPPLIER_REQ.Trim() == "Y" || parametes.CON_REQ.Trim() == "Y")
                        {
                            model.MAST_CON_ID = masterDetails.MAST_CON_ID;
                        }
                        else
                        {
                            model.MAST_CON_ID = null;
                        }
                        if (parametes.ROAD_REQ.Trim() == "Y")
                        {
                            model.IMS_PR_ROAD_CODE = model.IMS_PR_ROAD_CODE;
                        }
                        else
                        {
                            model.IMS_PR_ROAD_CODE = null;
                            model.FINAL_PAYMENT = null;
                        }

                        if (parametes.PIU_REQ.Trim() == "Y")
                        {
                            model.MAST_DPIU_CODE = model.MAST_DPIU_CODE;
                        }
                        else
                        {
                            model.MAST_DPIU_CODE = null;
                        }

                        if (parametes.AGREEMENT_REQ.Trim() == "Y" && parametes.CON_REQ.Trim() == "Y")
                        {
                            model.IMS_AGREEMENT_CODE = model.IMS_AGREEMENT_CODE_C;
                        }
                    }
                    else
                    {
                        model.IMS_AGREEMENT_CODE = model.IMS_AGREEMENT_CODE_DED;
                        model.MAST_CON_ID = masterDetails.MAST_CON_ID;
                    }

                    bool result = objAuthBAL.AddEditTransactionDeductionPaymentDetails(model, paymentDeduction.Equals("D") ? "D" : "T", objparams.BILL_ID, "E", tranNumber);

                    String encrptedBill_Id = URLEncrypt.EncryptParameters(new string[] { objparams.BILL_ID.ToString() });

                    ModelState.Clear();

                    return Json(new { Success = result, Bill_ID = encrptedBill_Id, disblehead = disblehead, head = model.HEAD_ID_P });
                    #endregion  valid model
                }
                else
                {
                    #region model error

                    objparams.BILL_TYPE = "P";
                    objparams.FUND_TYPE = masterDetails.FUND_TYPE;
                    objparams.LVL_ID = masterDetails.LVL_ID;
                    objparams.ADMIN_ND_CODE = masterDetails.ADMIN_ND_CODE;
                    objparams.MAST_CONT_ID = masterDetails.MAST_CON_ID == null ? 0 : masterDetails.MAST_CON_ID.Value; 
                    objparams.DISTRICT_CODE = PMGSYSession.Current.DistrictCode;
                    objparams.TXN_ID = masterDetails.TXN_ID;
                    objparams.STATE_CODE = Convert.ToInt16(PMGSYSession.Current.StateCode);
                    objparams.DISTRICT_CODE = Convert.ToInt16(PMGSYSession.Current.DistrictCode);
                    objparams.SANC_YEAR = Convert.ToInt16(model.IMS_SANCTION_YEAR);
                    objparams.OP_MODE = "E";
                    //objparams.DISTRICT_CODE = 508;
                    model.HEAD_ID_P = transactionDetails.TXN_ID + "$" + dbContext.ACC_MASTER_TXN.Where(m => m.TXN_ID == transactionDetails.TXN_ID).Select(m => m.CASH_CHQ).FirstOrDefault().Trim();
                    model.HeadId_P = objCommon.PopulateTransactions(objparams);
                    //find out for deduction
                    objparams.BILL_TYPE = "D";
                    model.HEAD_ID_D = transactionDetails.TXN_ID + "$" + dbContext.ACC_MASTER_TXN.Where(m => m.TXN_ID == transactionDetails.TXN_ID).Select(m => m.CASH_CHQ).FirstOrDefault().Trim();
                    model.HeadId_D = objCommon.PopulateTransactions(objparams);
                    model.IMS_AGREEMENT_CODE_C = transactionDetails.IMS_AGREEMENT_CODE;
                    model.IMS_PR_ROAD_CODE = transactionDetails.IMS_PR_ROAD_CODE;
                    objparams.AGREEMENT_CODE = transactionDetails.IMS_AGREEMENT_CODE.HasValue ? transactionDetails.IMS_AGREEMENT_CODE.Value : 0;
                    model.FINAL_PAYMENT = transactionDetails.FINAL_PAYMENT;
                    model.AGREEMENT_DED = objCommon.PopulateAgreement(objparams);


                    //if deduction payment
                    if (paymentDeduction.Equals("D"))
                    {
                        model.AGREEMENT_C = generalList;
                        model.IMS_PR_ROAD_CODEList = generalList;
                        model.MAST_DPIU_CODEList = generalList;
                        generalList.Clear();
                        generalList.Add(new SelectListItem { Selected = true, Text = "--Select--", Value = "0" });
                        model.IMS_SANCTION_YEAR_List = generalList;
                        model.IMS_SANCTION_PACKAGE_List = generalList;

                    }
                    else
                    {

                        if (parametes.ROAD_REQ.Trim() == "Y")
                        {
                            model.IMS_PR_ROAD_CODEList = objCommon.PopulateRoad(objparams);
                        }
                        else
                        {
                            model.IMS_PR_ROAD_CODEList = generalList;
                        }

                        if (parametes.PIU_REQ.Trim() == "Y")
                        {
                            model.MAST_DPIU_CODEList = objCommon.PopulateDPIU(objparams);
                        }
                        else
                        {
                            model.MAST_DPIU_CODEList = generalList;
                        }

                        if (parametes.AGREEMENT_REQ.Trim() == "Y" && parametes.CON_REQ.Trim() == "Y")
                        {
                            model.AGREEMENT_C = objCommon.PopulateAgreement(objparams);
                        }
                        else
                        {
                            model.AGREEMENT_C = generalList;

                        }

                        if (parametes.YEAR_REQ.Trim() == "Y")
                        {

                            model.IMS_SANCTION_YEAR_List = objCommon.PopulateSancYear(objparams);
                        }
                        else
                        {
                            generalList.Clear();
                            generalList.Add(new SelectListItem { Selected = true, Text = "--Select--", Value = "0" });
                            model.IMS_SANCTION_YEAR_List = generalList;
                        }
                        if (parametes.PKG_REQ.Trim() == "Y")
                        {
                            model.IMS_SANCTION_PACKAGE_List = objCommon.PopulatePackage(objparams);
                        }
                        else
                        {
                            generalList.Clear();
                            generalList.Add(new SelectListItem { Selected = true, Text = "--Select--", Value = "0" });
                            model.IMS_SANCTION_PACKAGE_List = generalList;
                        }


                    }

                    ViewBag.PaymentScreenRequired = masterDetails.CHQ_AMOUNT != 0 ? true : false;

                    ViewBag.DeductionScreenRequired = masterDetails.CASH_AMOUNT != 0 ? true : false;

                    ViewBag.urlparams = URLEncrypt.EncryptParameters(new string[] { objparams.BILL_ID.ToString() + "$" + tranNumber + "$" + paymentDeduction });

                    ViewBag.BillFinalized = masterDetails.AUTH_FINALIZED;

                    ViewBag.Bill_ID = URLEncrypt.EncryptParameters(new string[] { objparams.BILL_ID.ToString() });

                    List<SelectListItem> List = new List<SelectListItem>();

                    List.Add(new SelectListItem { Text = "Yes", Value = "true" });
                    List.Add(new SelectListItem { Selected = true, Text = "No", Value = "false" });

                    model.final_pay = List;

                    return View("AuthDetailsEntryForm", model);

                    #endregion model error
                }


            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                string errorMsg = string.Format("Errors: {0}", ex.Message);
                Response.TrySkipIisCustomErrors = true;
                Response.StatusCode = 500;
                Response.Write(errorMsg);
                return Json(string.Empty);
            }



        }


        /// <summary>
        /// Action to return the selected agreement number for the authorization payment transaction 
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="hash"></param>
        /// <param name="key"></param>
        /// <returns> returns the agreement code for the transaction</returns>
           [Audit]
         public string GetSelectedAgreementForTransaction(String parameter, String hash, String key)
        {
            Int64 BILL_ID = 0;

            String agreementNumber = string.Empty;

            try
            {
                if (!String.IsNullOrEmpty(parameter) && !String.IsNullOrEmpty(hash) && !String.IsNullOrEmpty(key))
                {
                    String[] urlParams = URLEncrypt.DecryptParameters(new String[] { parameter, hash, key });
                    if (urlParams.Length >= 1)
                    {
                        String[] urlSplitParams = urlParams[0].Split('$');

                        BILL_ID = Convert.ToInt64(urlSplitParams[0]);

                        return agreementNumber = objAuthBAL.GetAgreemntNumberForAuthorization(BILL_ID);

                    }
                    else
                    {
                        throw new Exception("Error While getting the selected agreement.. ");
                    }
                }
                else
                {
                    throw new Exception("Error While getting the selected agreement.. ");
                }
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                string errorMsg = string.Format("Errors: {0}", ex.Message);
                Response.TrySkipIisCustomErrors = true;
                Response.StatusCode = 500;
                Response.Write(errorMsg);
                return (string.Empty);
            }

        }

        /// <summary>
        /// action to edit the master payment details
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="hash"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        [HttpGet]
        [Audit]
         public ActionResult EditAuthorizationMasterDetails(String parameter, String hash, String key)
        {
            Int64 Bill_Id = 0;
            PMGSYEntities dbContext = new PMGSYEntities();
            TransactionParams objparams = new TransactionParams();
            AuthorizationRequestMasterModel model = new AuthorizationRequestMasterModel();
            objCommon = new CommonFunctions();
            try
            {
                if (!String.IsNullOrEmpty(parameter) && !String.IsNullOrEmpty(hash) && !String.IsNullOrEmpty(key))
                {

                    if (!String.IsNullOrEmpty(parameter) && !String.IsNullOrEmpty(hash) && !String.IsNullOrEmpty(key))
                    {
                        String[] urlParams = URLEncrypt.DecryptParameters(new String[] { parameter, hash, key });
                        if (urlParams.Length >= 1)
                        {
                            String[] urlSplitParams = urlParams[0].Split('$');
                            Bill_Id = Convert.ToInt64(urlSplitParams[0]);

                        }
                    }
                    else
                    {
                        throw new Exception("Error while getting master payment details..");
                    }
                }

                ACC_AUTH_REQUEST_MASTER masterDetails = new ACC_AUTH_REQUEST_MASTER();
                masterDetails = objAuthBAL.GetMasterAuthorizationDetails(Bill_Id);


                objparams.BILL_TYPE = "P";
                objparams.FUND_TYPE = masterDetails.FUND_TYPE;
                objparams.LVL_ID = masterDetails.LVL_ID;
                objparams.ADMIN_ND_CODE = masterDetails.ADMIN_ND_CODE;
                objparams.DISTRICT_CODE = PMGSYSession.Current.DistrictCode;
                //objparams.DISTRICT_CODE = 508;


                List<SelectListItem> transactionType = objCommon.PopulateAuthorizationTransaction(objparams);
                model.TXN_ID = masterDetails.TXN_ID + "$" + dbContext.ACC_MASTER_TXN.Where(m => m.TXN_ID == masterDetails.TXN_ID).Select(m => m.CASH_CHQ).FirstOrDefault().Trim();
                model.TXN_ID_LIST = transactionType;

                List<SelectListItem> monthList = objCommon.PopulateMonths(masterDetails.AUTH_MONTH);
                model.AUTH_MONTH_LIST = monthList;
                model.AUTH_MONTH = masterDetails.AUTH_MONTH;

                List<SelectListItem> yearList = objCommon.PopulateYears(masterDetails.AUTH_YEAR);
                model.AUTH_YEAR = masterDetails.AUTH_YEAR;
                model.AUTH_YEAR_LIST = yearList;

                List<SelectListItem> ContractorList = objCommon.PopulateContractorSupplier(objparams);
                model.MAST_CON_ID_C = masterDetails.MAST_CON_ID;
                model.MAST_CON_ID_C1 = ContractorList;

                model.AUTH_DATE = objCommon.GetDateTimeToString(masterDetails.AUTH_DATE);

                model.AUTH_NO = masterDetails.AUTH_NO;

                model.CASH_AMOUNT = masterDetails.CASH_AMOUNT;

                model.CHEQUE_AMOUNT = masterDetails.CHQ_AMOUNT;

                ViewBag.operationType = "E";

                ViewBag.Bill_id = URLEncrypt.EncryptParameters(new string[] { Bill_Id.ToString() });

                ViewBag.LevelID = PMGSYSession.Current.LevelId;

                return View("AuthorizationMaster", model);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
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


        /// <summary>
        /// function to get the final authorization details
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="hash"></param>
        /// <param name="key"></param>
        /// <param name="id"></param>
        /// <returns></returns>
         [Audit]
         public JsonResult GetFinalAuthorizationDetails(String parameter, String hash, String key, string id)
        {
            Int64 BILL_ID = 0;
            try
            {
                Int32 roadID = Convert.ToInt32(id.Trim());

                if (!String.IsNullOrEmpty(parameter) && !String.IsNullOrEmpty(hash) && !String.IsNullOrEmpty(key))
                {
                    String[] urlParams = URLEncrypt.DecryptParameters(new String[] { parameter, hash, key });
                    if (urlParams.Length >= 1)
                    {
                        String[] urlSplitParams = urlParams[0].Split('$');

                        BILL_ID = Convert.ToInt64(urlSplitParams[0]);

                        return Json(objAuthBAL.GetFinalPaymentDetails(BILL_ID, roadID));

                    }
                    else
                    {
                        throw new Exception("Error While getting authorization details details.. ");
                    }
                }
                else
                {
                    throw new Exception("Error While getting authorization details.. ");
                }


            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                string errorMsg = string.Format("Errors: {0}", ex.Message);
                Response.TrySkipIisCustomErrors = true;
                Response.StatusCode = 500;
                Response.Write(errorMsg);
                return Json(string.Empty);
            }


        }


         //new method added by Vikram on 29-08-2013
          [Audit]
         public JsonResult ValidateDPIUBankAuthorization()
        {
            objAuthBAL = new AuthorizationBAL();
            try
            {
                bool status = objAuthBAL.ValidateDPIUBankAuthBAL(PMGSYSession.Current.AdminNdCode);
                if (status == true)
                {
                    return Json(new { success=true});
                }
                else
                {
                    return Json(new { success = false });
                }
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return Json(new { success = false });
            }
        }


          [Audit]
          public bool ValidateRoad(int headId, int proposalCode, string fundType)
          {
              string upgradeConnectFlag = string.Empty;
              objCommon = new CommonFunctions();
              try
              {
                  if (fundType == "P")
                  {
                      switch (headId)
                      {
                          case 48:
                              upgradeConnectFlag = "N";
                              break;
                          case 49:
                              upgradeConnectFlag = "U";
                              break;
                          default:
                              upgradeConnectFlag = "A";
                              break;
                      }

                      bool status = objCommon.ValidateRoad(proposalCode, upgradeConnectFlag);
                      return status;
                  }
                  else
                  {
                      return true;
                  }
              }
              catch (Exception ex)
              {
                  Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                  return false;
              }
          }


          //Added By Abhishek kamble 4 Sep 2014
          /// <summary>
          /// Validates whether selected road is valid for this PMGSY scheme I /PMGSY scheme II
          /// </summary>
          /// <param name="txnID"></param>
          /// <param name="RoadCode"></param>
          /// <returns></returns>
          public bool ValidateRoadForPMGSYScheme(short txnID, int RoadCode)
          {

              try
              {
                  CommonFunctions commonFuncObj = new CommonFunctions();
                  bool status = commonFuncObj.ValidateRoadForPMGSYScheme(txnID, RoadCode, true);
                  return status;

              }
              catch (Exception ex)
              {
                  Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                  return false;
              }
          }

    }

        

}
