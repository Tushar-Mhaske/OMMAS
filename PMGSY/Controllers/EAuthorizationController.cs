#region References
using iTextSharp.text.pdf;
using Mvc.Mailer;
using PMGSY.BAL;
using PMGSY.BAL.EAuthorization;
using PMGSY.Common;
using PMGSY.DAL.EAuthorization;
using PMGSY.DAL.Payment;
using PMGSY.Extensions;
using PMGSY.Models;
using PMGSY.Models.Common;
using PMGSY.Models.EAuthorization;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity.Validation;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Transactions;
using System.Web;
using System.Web.Mvc;

#endregion

namespace PMGSY.Controllers
{
    public class EAuthorizationController : Controller
    {
        #region Properties
        CommonFunctions common = null;
        IEAuthorizationBAL objPaymentBAL = null;
        #endregion

        public EAuthorizationController()
        {
            common = new CommonFunctions();
            objPaymentBAL = new EAuthorizationBAL();
        }

        #region Methods

        public ActionResult Index()
        {
            return View();
        }


        public string GetSHA1HashData(string data)
        {
            //create new instance of md5
            SHA1 sha1 = SHA1.Create();

            //convert the input text to array of bytes
            byte[] hashData = sha1.ComputeHash(Encoding.Default.GetBytes(data));

            //create new instance of StringBuilder to save hashed data
            StringBuilder returnValue = new StringBuilder();

            //loop for each byte and add it to StringBuilder
            for (int i = 0; i < hashData.Length; i++)
            {
                returnValue.Append(hashData[i].ToString());
            }

            // return hexadecimal string
            return returnValue.ToString();
        }



        #region List EAuthorization Master 1st View

        [Audit]
        public ActionResult GetEAuthorizationList(string id)
        {
            TransactionParams objparams = new TransactionParams();

            string filePath = string.Empty;
            try
            {
                objparams.BILL_TYPE = "P";
                objparams.FUND_TYPE = PMGSYSession.Current.FundType;
                objparams.LVL_ID = PMGSYSession.Current.LevelId;
                objparams.ADMIN_ND_CODE = PMGSYSession.Current.AdminNdCode;
                objparams.DISTRICT_CODE = PMGSYSession.Current.DistrictCode;
                List<SelectListItem> monthList = new List<SelectListItem>();
                List<SelectListItem> yearList = new List<SelectListItem>();
                if (string.IsNullOrEmpty(id))
                {
                    if (PMGSYSession.Current.AccMonth == 0)
                    {
                        monthList = common.PopulateMonths(DateTime.Now.Month);
                        yearList = common.PopulateYears(DateTime.Now.Year);
                    }
                    else
                    {
                        monthList = common.PopulateMonths(PMGSYSession.Current.AccMonth);
                        yearList = common.PopulateYears(PMGSYSession.Current.AccYear);
                    }
                }
                else
                {
                    int month = 0;
                    int year = 0;
                    month = Convert.ToInt32(id.Split('$')[0]);
                    year = Convert.ToInt32(id.Split('$')[1]);
                    monthList = common.PopulateMonths(month);
                    yearList = common.PopulateYears(year);
                }
                ViewData["months"] = monthList;
                ViewData["year"] = yearList;
                ViewBag.levelID = PMGSYSession.Current.LevelId;
                List<SelectListItem> transactionType = common.PopulateTransactions(objparams);

                ViewData["TXN_ID"] = transactionType;
                return View();
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "EAuthorization.GetEAuthorizationList()");
                return null;
            }
        }
        #endregion

        #region List EAuthorization Master 1st View
        /// <summary>
        /// action to get the authorization request details
        /// </summary>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <param name="sidx"></param>
        /// <param name="sord"></param>
        /// <returns></returns>
        /// 

        [HttpPost]
        [Audit]
        public JsonResult EAuthorizationRequestListView(int? page, int? rows, string sidx, string sord)
        {
            EAuthorizationBAL objEAuthorizationBAL = null;
            try
            {
                objEAuthorizationBAL = new EAuthorizationBAL();
                using (CommonFunctions commonFunction = new CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new GridParams(page, rows, sidx, sord, Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                    {
                        return null;
                    }
                }
                EAuthorizationFilterModel objFilter = new EAuthorizationFilterModel();
                objFilter.FilterMode = Request.Params["mode"].Trim();
                long totalRecords;
                objFilter.Month = Convert.ToInt16(Request.Params["months"]);
                objFilter.Year = Convert.ToInt16(Request.Params["year"]);

                if (objFilter.FilterMode.Equals("Search"))
                {
                    objFilter.FromDate = Request.Params["fromDate"].ToString() == String.Empty ? null : Request.Params["fromDate"].ToString();
                    objFilter.ToDate = Request.Params["toDate"].ToString() == String.Empty ? null : Request.Params["toDate"].ToString();
                    objFilter.EAuthorizationStatus = Request.Params["AUTH_STATUS"].ToString() == String.Empty ? String.Empty : Request.Params["EAUTH_STATUS"].ToString();
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
                    rows = objEAuthorizationBAL.EAuthorizationRequestListView(objFilter, out totalRecords),
                    total = totalRecords <= objFilter.rows ? 1 : totalRecords / objFilter.rows + 1,
                    page = objFilter.page,
                    records = totalRecords
                };
                return Json(jsonData);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "EAuthorization.EAuthorizationRequestListView()");
                return null;
            }
        }
        #endregion

        /// <summary>
        ///This Method Returns Master EAuthroization Details before Transaction form Entry
        /// </summary>
        /// <param name="id"></param>
        /// <param name="parameter</param>
        /// <param name="hash"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        [HttpGet]
        [Audit]
        public ActionResult MasterEAuthorizationDetails(String id, String parameter, String hash, String key)
        {
            Int64 BILL_ID = 0;
            PMGSYEntities dbContext = null;
            try
            {
                dbContext = new PMGSYEntities();
                //Add EAuth Master Button First Time
                if (String.IsNullOrEmpty(parameter) && String.IsNullOrEmpty(hash) && String.IsNullOrEmpty(key))
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
                    ViewBag.FundType = PMGSYSession.Current.FundType;
                    return View();
                }//Edit case From 1st List
                else
                {
                    String[] urlParams = URLEncrypt.DecryptParameters(new String[] { parameter, hash, key });
                    if (urlParams.Length >= 1)
                    {
                        String[] urlSplitParams = urlParams[0].Split('$');
                        BILL_ID = Convert.ToInt64(urlSplitParams[0]);
                    }
                    if (!dbContext.ACC_EAUTH_DETAILS.Where(x => x.EAUTH_ID == BILL_ID).Any())
                    {
                        ViewBag.cntEntryInDetailsForFinalizedButton = 0;
                    }
                    else
                    {
                        int count = dbContext.ACC_EAUTH_DETAILS.Where(x => x.EAUTH_ID == BILL_ID).Count();
                        ViewBag.cntEntryInDetailsForFinalizedButton = count;
                    }
                    ViewBag.operationType = "E";
                    ViewBag.Bill_id = URLEncrypt.EncryptParameters(new string[] { BILL_ID.ToString().Trim() });
                    return View();
                }
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "EAuthorization.MasterEAuthorizationDetails()");
                return null;
            }
        }

        #region EAuthorization Master Entry Form


        /// <summary>
        ///Edit Authorization Details
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Audit]
        public ActionResult AddEditEAuthorizationDetails(String id)
        {
            TransactionParams objparams = null;
            EAuthorizationMasterModel model = null;
            CommonFunctions objCommon = null;
            EAuthorizationBAL objEAuthorizationBAL = null;
            try
            {
                objparams = new TransactionParams();
                model = new EAuthorizationMasterModel();
                objCommon = new CommonFunctions();
                objEAuthorizationBAL = new EAuthorizationBAL();
                objparams.BILL_TYPE = "P";
                objparams.FUND_TYPE = PMGSYSession.Current.FundType;
                objparams.LVL_ID = PMGSYSession.Current.LevelId;
                objparams.ADMIN_ND_CODE = PMGSYSession.Current.AdminNdCode;
                objparams.DISTRICT_CODE = PMGSYSession.Current.DistrictCode;
                objparams.STATE_CODE = PMGSYSession.Current.StateCode;
                int month = 0;
                int year = 0;
                if (!String.IsNullOrEmpty(id))
                {
                    model.BILL_MONTH = Convert.ToInt16(id.Split('$')[0]);
                    model.BILL_YEAR = Convert.ToInt16(id.Split('$')[1]);
                    model.EAUTHORIZATION_NO = objEAuthorizationBAL.GetAuthorizationNumber(model.BILL_MONTH, model.BILL_YEAR, objparams.STATE_CODE, objparams.ADMIN_ND_CODE);
                }
                else
                {
                    throw new Exception("Error While loading the page..");
                }
                objparams.OP_MODE = "A";
                List<SelectListItem> monthList = common.PopulateMonths(month);
                model.BILL_MONTH_List = monthList;
                List<SelectListItem> yearList = common.PopulateYears(year);
                model.BILL_YEAR_List = yearList;
                ViewBag.LevelID = PMGSYSession.Current.LevelId;
                model.CURRENT_DATE = DateTime.Now.ToString("dd/MM/yyyy");
                return View(model);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "EAuthorization.AddEditEAuthorizationDetails()");
                return null;
            }
        }

        #endregion

        #region GetEAuthorization Number on Year/Month Change
        /// <summary>
        /// action to get authorization number
        /// </summary>
        /// <returns></returns>
        /// 
        [HttpGet]
        [Audit]
        public JsonResult GetAuthorizationNumber(string id)
        {
            EAuthorizationBAL objEAuthorizationBAL = null;
            try
            {
                objEAuthorizationBAL = new EAuthorizationBAL();
                Int16 month = 0;
                Int16 year = 0;
                month = Convert.ToInt16(id.Split('$')[0]);
                year = Convert.ToInt16(id.Split('$')[1]);
                return Json(objEAuthorizationBAL.GetAuthorizationNumber(month, year, PMGSYSession.Current.StateCode, PMGSYSession.Current.AdminNdCode), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "EAuthorization.GetAuthorizationNumber()");
                return null;
            }

        }
        #endregion

        #region Get EAuthorization Detail Form
        /// <summary>
        /// Returs EAuthorization Transaction Detail Entry Form
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="hash"></param>
        /// <param name="key"></param>
        /// <returns></returns>

        [Audit]
        public ActionResult GetEAuthDetailsEntryForm(String parameter, String hash, String key)
        {
            CommonFunctions objCommon = null;
            PMGSYEntities dbContext = null;
            TransactionParams objparams = null;
            try
            {
                dbContext = new PMGSYEntities();
                objparams = new TransactionParams();
                objCommon = new CommonFunctions();
                objPaymentBAL = new EAuthorizationBAL();
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
                        throw new Exception("Error while getting Master E Authorization Request details..");
                    }
                }

                List<SelectListItem> roadlist = new List<SelectListItem>();
                roadlist.Add(new SelectListItem { Selected = true, Text = "--Select--", Value = "" });
                ACC_EAUTH_MASTER masterDetails = new ACC_EAUTH_MASTER();
                masterDetails = objPaymentBAL.GetMasterAuthorizationDetails(objparams.BILL_ID);
                objparams.FUND_TYPE = masterDetails.FUND_TYPE;
                objparams.LVL_ID = masterDetails.LVL_ID;
                objparams.ADMIN_ND_CODE = masterDetails.ADMIN_ND_CODE;
                objparams.DISTRICT_CODE = PMGSYSession.Current.DistrictCode;
                objparams.BILL_TYPE = "P";
                objparams.OP_MODE = "A";
                EAuthorizationRequestDetailsModel model = new EAuthorizationRequestDetailsModel();
                model.AGREEMENT_C = roadlist;
                model.MAST_DPIU_CODEList = roadlist;
                model.IMS_PR_ROAD_CODEList = roadlist;
                List<SelectListItem> ContractorList = objCommon.PopulateContractorSupplier(objparams);
                model.MAST_CON_ID_C1 = ContractorList;
                roadlist.Clear();
                roadlist.Add(new SelectListItem { Selected = true, Text = "--Select--", Value = "0" });
                model.IMS_SANCTION_YEAR_List = roadlist;
                model.IMS_SANCTION_PACKAGE_List = roadlist;
                List<SelectListItem> List = new List<SelectListItem>();
                List.Add(new SelectListItem { Selected = true, Text = "No", Value = "false" });
                List.Add(new SelectListItem { Text = "Yes", Value = "true" });
                model.final_pay = List;
                ViewBag.Bill_ID = URLEncrypt.EncryptParameters(new string[] { objparams.BILL_ID.ToString() });
                return View("TransactionDetailsEntryForm", model);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "EAuthorization.GetEAuthDetailsEntryForm()");
                return null;
            }
        }
        #endregion

        #region Adding Master Entry
        /// <summary>
        /// This methods Add EAuthrization Master Entry
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Audit]
        [ValidateAntiForgeryToken]
        public ActionResult AddEAuthorizationMasterDetails(EAuthorizationMasterModel model)
        {
            EAuthorizationBAL objEAuthorizationBAL = null;
            string encrptedAuth_Id = string.Empty;
            EAuthorizationMasterModel returnModel = null;
            try
            {
                common = new CommonFunctions();
                Int32 Auth_Id = 0;
                objEAuthorizationBAL = new EAuthorizationBAL();
                model.FundType = PMGSYSession.Current.FundType;
                model.LevelID = PMGSYSession.Current.LevelId;
                model.AdminNDCode = PMGSYSession.Current.AdminNdCode;
                model.UserID = PMGSYSession.Current.UserId;
                returnModel = new EAuthorizationMasterModel();
                model.Operation = "A";
                model.Auth_Id = Auth_Id;
                if (ModelState.IsValid)
                {
                    if (model.BILL_MONTH != 0 && model.BILL_MONTH < 13 && model.BILL_YEAR != 0)
                    {
                        string monthlyClosingStatus = string.Empty;
                        string errMessage = string.Empty;
                        monthlyClosingStatus = common.MonthlyClosingValidation(model.BILL_MONTH, model.BILL_YEAR, model.FundType, model.LevelID, model.AdminNDCode, ref errMessage);
                        if (monthlyClosingStatus.Equals("-111"))
                        {
                            return Json(new { success = false, ErrMessage = errMessage });
                        }
                        if (monthlyClosingStatus.Equals("-222"))
                        {
                            return Json(new { success = false, ErrMessage = "Month is already closed" });
                        }

                        //Date Validation
                        string EAuthDate = model.EAUTHORIZATION_DATE;
                        string[] SplitEAuthDate = EAuthDate.Split('/');
                        Int32 EAuthDay = Convert.ToInt32(SplitEAuthDate[0]); //Day
                        Int32 EAuthMonth = Convert.ToInt32(SplitEAuthDate[1]); //Month
                        Int32 EAuthYear = Convert.ToInt32(SplitEAuthDate[2]);//Year
                        DateTime date = DateTime.Now;
                        Int32 ToDay = date.Day;
                        Int32 ToMonth = date.Month;
                        Int32 ToYear = date.Year;
                        if (EAuthDay != ToDay)
                        {
                            return Json(new { success = false, Bill_ID = "-11" });
                        }
                        else if (EAuthMonth != ToMonth)
                        {
                            return Json(new { success = false, Bill_ID = "-11" });
                        }
                        else if (EAuthYear != ToYear)
                        {
                            return Json(new { success = false, Bill_ID = "-11" });
                        }

                        if (PMGSYSession.Current.FundType == "P")
                        {
                        }
                        else
                        {
                            return Json(new { success = false, Bill_ID = "-111" });

                        }
                    }

                    returnModel = objEAuthorizationBAL.AddEAuthorizationMasterDetails(model);
                    if (returnModel != null)
                    {
                        if (returnModel.status)
                        {
                            if (returnModel.Auth_Id != 0)
                            {
                                model.Auth_Id = returnModel.Auth_Id;
                                encrptedAuth_Id = URLEncrypt.EncryptParameters(new string[] { model.Auth_Id.ToString() });
                                return Json(new { success = true, Auth_Id = encrptedAuth_Id });
                            }
                            else
                            {
                                return Json(new { success = false, Bill_ID = "-1" });
                            }
                        }
                        else
                        {
                            return Json(new { success = false, Bill_ID = "-23", StatusMessage=returnModel.StatusMessage });
                        }
                        

                    }
                    else
                    {
                        return Json(new { success = false, Bill_ID = "-26" });
                    }

                   
                }
                else
                {
                    return Json(new { success = false, Bill_ID = "-2" });
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
                ErrorLog.LogError(ex, "EAuthorization.AddEAuthorizationMasterDetails()");
                return null;
            }


        }
        #endregion

        #region List After Adding Master Entry
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
        public JsonResult ListEAuthorizationRequestForDataEntry(int? page, int? rows, string sidx, string sord, String parameter, String hash, String key)
        {
            try
            {
                objPaymentBAL = new EAuthorizationBAL();
                using (CommonFunctions commonFunction = new CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new GridParams(page, rows, sidx, sord, Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                    {
                        return null;
                    }
                }

                Int64 Auth_Id = 0;
                long totalRecords = 0;
                EAuthorizationFilterModel objFilter = new EAuthorizationFilterModel();
                objFilter.page = Convert.ToInt32(page) - 1;
                objFilter.rows = Convert.ToInt32(rows);
                objFilter.sidx = sidx.ToString();
                objFilter.sord = sord.ToString();
                objFilter.Bill_type = "P";
                objFilter.AdminNdCode = PMGSYSession.Current.AdminNdCode;
                objFilter.FundType = PMGSYSession.Current.FundType;
                objFilter.LevelId = PMGSYSession.Current.LevelId;
                string Authid = Request.Params["EAUTH_ID"].ToString() == String.Empty ? String.Empty : Request.Params["EAUTH_ID"].ToString();
                String[] splitID = Authid.Split('/');
                parameter = splitID[0];
                hash = splitID[1];
                key = splitID[2];
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
                            rows = objPaymentBAL.ListEAuthorizationMasterDetails(objFilter, out totalRecords),
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
                ErrorLog.LogError(ex, "EAuthorization.ListEAuthorizationRequestForDataEntry()");
                return null;
            }

        }
        #endregion

        #region Add Transaction Details
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
        public ActionResult AddPaymentTransactionDetails(EAuthorizationRequestDetailsModel model, String parameter, String hash, String key)
        {
            PMGSYEntities dbContext = null;
            ACC_SCREEN_DESIGN_PARAM_DETAILS parametes = new ACC_SCREEN_DESIGN_PARAM_DETAILS();
            TransactionParams objparams = null;
            Int64 Bill_ID = 0;
            try
            {
                dbContext = new PMGSYEntities();
                common = new CommonFunctions();
                objPaymentBAL = new EAuthorizationBAL();
                objparams = new TransactionParams();
                if (!String.IsNullOrEmpty(parameter) && !String.IsNullOrEmpty(hash) && !String.IsNullOrEmpty(key))
                {
                    String[] urlParams = URLEncrypt.DecryptParameters(new String[] { parameter, hash, key });
                    if (urlParams.Length >= 1)
                    {
                        String[] urlSplitParams = urlParams[0].Split('$');
                        Bill_ID = Convert.ToInt64(urlSplitParams[0]);
                    }
                }


                ACC_EAUTH_MASTER masterDetails = new ACC_EAUTH_MASTER();
                masterDetails = objPaymentBAL.GetMasterAuthorizationDetails(Bill_ID);
                if (model.ALREADY_AUTHORISED_AMOUNT == null)
                {
                    return Json(new { Success = false, status = "-66" });
                }

                if (ModelState.IsValid)
                {
                    #region serverside validation for payment

                    if (masterDetails.EAUTH_STATUS == "Y")
                    {
                        return Json(new { Success = false, status = "-1" });
                    }
                    if (model.MAST_CON_ID_C == null || model.MAST_CON_ID_C == 0)
                    {
                        return Json(new { Success = false, status = "-2" });
                    }
                    if (model.IMS_AGREEMENT_CODE_C == null || model.IMS_AGREEMENT_CODE_C == 0)
                    {
                        return Json(new { Success = false, status = "-3" });
                    }
                    if (model.IMS_SANCTION_PACKAGE == "0")
                    {
                        return Json(new { Success = false, status = "-20" });
                    }

                    //Package NIL Check

                    //if (model.IMS_SANCTION_PACKAGE == "NIL")
                    //{
                    //    return Json(new { Success = false, status = "-45" });
                    //}


                    if (model.AMOUNT_Q == null)
                    {
                        return Json(new { Success = false, status = "-4" });
                    }
                    else if (model.AMOUNT_Q <= 0)
                    {
                        return Json(new { Success = false, status = "-5" });
                    }

                    decimal TotalAgreementAmt = Convert.ToDecimal(model.AGREEMENT_AMOUNT); //A(In Lakhs)
                    decimal alreadyAuthAmt = Convert.ToDecimal(model.ALREADY_AUTHORISED_AMOUNT); //B (In Lakhs)
                    decimal RequestAmount = Convert.ToDecimal(model.AMOUNT_Q); //C
                    decimal TotalCompareAmt = RequestAmount + alreadyAuthAmt;
                    if (TotalAgreementAmt >= TotalCompareAmt)
                    {
                    }
                    else
                    {
                        return Json(new { Success = false, status = "-7" });
                    }
                    int lgdStateCode = dbContext.OMMAS_LDG_STATE_MAPPING.Where(x => x.MAST_STATE_CODE == PMGSYSession.Current.StateCode).Select(x => x.MAST_STATE_LDG_CODE).FirstOrDefault();
                    Int32 ConID = Convert.ToInt32(model.MAST_CON_ID_C);
                    bool checkbankDetails = dbContext.PFMS_OMMAS_CONTRACTOR_MAPPING.Any(x => x.MAST_CON_ID == ConID && x.MAST_LGD_STATE_CODE == lgdStateCode && x.PFMS_CON_ID != null && x.STATUS == "A" && x.MASTER_CONTRACTOR.MASTER_CONTRACTOR_BANK.Where(z => z.MAST_CON_ID == ConID && z.MAST_LOCK_STATUS == "Y").Select(b => b.MAST_ACCOUNT_STATUS).FirstOrDefault() == "A");

                    if (!checkbankDetails)
                    {
                        return Json(new { Success = false, status = "-77" });
                    }
                    Int32 AggrementCode = Convert.ToInt32(model.IMS_AGREEMENT_CODE_C);
                    //if Package already Present Against same Agreement code
                    if (dbContext.ACC_EAUTH_DETAILS.Where(x => x.IMS_PACKAGE_ID == model.IMS_SANCTION_PACKAGE && x.IMS_AGREEMENT_CODE == AggrementCode && x.EAUTH_ID == Bill_ID).Any())
                    {
                        return Json(new { Success = false, status = "-88" });
                    }
                    model.AdminND_Code = PMGSYSession.Current.AdminNdCode;
                    Int32 EAuthID = objPaymentBAL.AddPaymentTransactionDetails(model, "T", Bill_ID, "A", 0);
                    if (EAuthID != 0)
                    {
                        return Json(new { Success = true, EAuthID = EAuthID });
                    }
                    else
                    {
                        return Json(new { Success = false, status = "-6" });
                    }

                    #endregion

                }
                else
                {
                    return Json(new { Success = false });
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
                ErrorLog.LogError(ex, "EAuthorization.AddPaymentTransactionDetails()");
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

        #region List Transaction Details on Adding
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
        [HttpGet]
        [Audit]
        public JsonResult GetPaymentDetailList(int? page, int? rows, string sidx, string sord, String parameter, String hash, String key)
        {
            EAuthorizationFilterModel objFilter = null;
            try
            {
                objPaymentBAL = new EAuthorizationBAL();
                objFilter = new EAuthorizationFilterModel();
                using (CommonFunctions commonFunction = new CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new GridParams(page, rows, sidx, sord, Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                    {
                        return null;
                    }
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


                var jsonData = new
                {
                    rows = objPaymentBAL.GetPaymentDetailList(objFilter, out totalRecords),
                    total = totalRecords <= objFilter.rows ? 1 : totalRecords / objFilter.rows + 1,
                    page = objFilter.page,
                    records = totalRecords
                };
                return Json(jsonData, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "EAuthorization.GetPaymentDetailList()");
                return null;
            }

        }
        #endregion

        #region Setting PayeeName
        /// <summary>
        /// function to get the contractor or supplier name
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>

        [Audit]
        public String SetPayeeName(string id)
        {
            string name = string.Empty;
            try
            {
                objPaymentBAL = new EAuthorizationBAL();
                TransactionParams objparams = new TransactionParams();
                int CintractorId = Convert.ToInt16(id);
                if (!String.IsNullOrEmpty(id))
                {
                    objparams.MAST_CONT_ID = CintractorId;
                }
                else
                {
                    throw new Exception("Exception while getting Contractor/Supplier name");
                }

                if (objparams.MAST_CONT_ID == -1)
                {
                    return string.Empty;
                }
                else
                {
                    name = objPaymentBAL.SetPayeeName(objparams);
                    return name;
                }
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "EAuthorization.SetPayeeName()");
                return null;
            }


        }




        #endregion

        #region Populate Packages

        /// <summary>
        /// function to get the Package
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// 
        [HttpPost]
        [Audit]
        public JsonResult PopulatePackage(string id)
        {
            TransactionParams objparams = null;
            PMGSYEntities dbcontext = null;
            try
            {
                dbcontext = new PMGSYEntities();
                objPaymentBAL = new EAuthorizationBAL();
                objparams = new TransactionParams();
                objparams.DISTRICT_CODE = PMGSYSession.Current.DistrictCode;
                objparams.ADMIN_ND_CODE = PMGSYSession.Current.AdminNdCode;
                string[] arr = id.Split('$');
                List<SelectListItem> lstTendDetails = new List<SelectListItem>();
                objparams.MAST_CONT_ID = Convert.ToInt32(arr[0]);
                objparams.AGREEMENT_CODE = Convert.ToInt32(arr[1]);
                var varAggrement = (from aggmast in dbcontext.TEND_AGREEMENT_DETAIL

                                    where
                                   aggmast.TEND_AGREEMENT_CODE == objparams.AGREEMENT_CODE
                                    select new
                                    {
                                        IMS_PACKAGE_ID = aggmast.IMS_SANCTIONED_PROJECTS.IMS_PACKAGE_ID
                                    }).Distinct().ToList();

                foreach (var item in varAggrement)
                {
                    lstTendDetails.Add(new SelectListItem { Text = item.IMS_PACKAGE_ID, Value = item.IMS_PACKAGE_ID });
                }

                lstTendDetails = lstTendDetails.OrderBy(m => m.Text).ToList();
                lstTendDetails.Insert(0, (new SelectListItem { Text = "Select Package", Value = "0" }));
                if (lstTendDetails != null)
                {
                    return Json(new SelectList(lstTendDetails, "Value", "Text"));
                }
                else
                {
                    return Json(new { success = false, response = "Error in Getting AggrementList." });
                }
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "EAuthorization.PopulatePackage()");
                return null;
            }

        }
        #endregion


        #region Populate Agreement
        /// <summary>
        /// Populate Agreement based on Selected Contractor
        /// </summary>
        /// <param name="MastConID"></param>
        /// <returns></returns>
        [HttpPost]
        [Audit]
        public JsonResult PopulateAgreement(int MastConID)
        {
            TransactionParams objparams = null;
            PMGSYEntities dbContext = new PMGSYEntities();
            try
            {
                objparams = new TransactionParams();
                objparams.DISTRICT_CODE = PMGSYSession.Current.DistrictCode;
                objparams.MAST_CONT_ID = MastConID;
                List<SelectListItem> lstAgreement = new List<SelectListItem>();
                var varAggrement = (from aggmast in dbContext.TEND_AGREEMENT_MASTER

                                    where
                                   aggmast.MAST_CON_ID == MastConID
                                    select new
                                    {

                                        TEND_AGREEMENT_NUMBER = aggmast.TEND_AGREEMENT_NUMBER,
                                        TEND_AGREEMENT_CODE = aggmast.TEND_AGREEMENT_CODE

                                    }).Distinct().ToList();
                foreach (var item in varAggrement)
                {
                    lstAgreement.Add(new SelectListItem { Text = item.TEND_AGREEMENT_NUMBER, Value = item.TEND_AGREEMENT_CODE.ToString() });
                }
                lstAgreement = lstAgreement.OrderBy(m => m.Text).ToList();
                lstAgreement.Insert(0, (new SelectListItem { Text = "Select Agreement", Value = "0" }));
                if (lstAgreement != null)
                {
                    return Json(new SelectList(lstAgreement, "Value", "Text"));
                }
                else
                {
                    return Json(new { success = false, response = "Error in Getting AggrementList." });
                }
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return Json(string.Empty);
            }

        }
        #endregion

        #region Edit EAuthorization Details

        /// <summary>
        /// action to Edit the EAuthorization Details
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="hash"></param>
        /// <param name="key"></param>
        /// <returns></returns>

        [Audit]
        public ActionResult EditEAuthorizationDetails(String parameter, String hash, String key)
        {
            Int64 Bill_Id = 0;
            PMGSYEntities dbContext = null;
            TransactionParams objparams = null;
            CommonFunctions objCommon = null;
            ACC_EAUTH_DETAILS obj = null;
            EAuthorizationRequestDetailsModel model = null;
            List<SelectListItem> lstTendDetails = new List<SelectListItem>();
            try
            {
                objPaymentBAL = new EAuthorizationBAL();
                objCommon = new CommonFunctions();
                obj = new ACC_EAUTH_DETAILS();
                model = new EAuthorizationRequestDetailsModel();
                objparams = new TransactionParams();
                dbContext = new PMGSYEntities();
                if (!String.IsNullOrEmpty(parameter) && !String.IsNullOrEmpty(hash) && !String.IsNullOrEmpty(key))
                {
                    if (!String.IsNullOrEmpty(parameter) && !String.IsNullOrEmpty(hash) && !String.IsNullOrEmpty(key))
                    {
                        String[] urlParams = URLEncrypt.DecryptParameters(new String[] { parameter, hash, key });
                        if (urlParams.Length >= 1)
                        {
                            String[] urlSplitParams = urlParams[0].Split('$');
                            objparams.BILL_ID = Convert.ToInt64(urlSplitParams[0]);
                            objparams.TXN_NO = Convert.ToInt16(urlSplitParams[1]);
                        }
                    }
                    else
                    {
                        throw new Exception("Error while getting master payment details..");
                    }
                    List<SelectListItem> roadlist = new List<SelectListItem>();
                    roadlist.Add(new SelectListItem { Selected = true, Text = "--Select--", Value = "" });
                    ACC_EAUTH_MASTER masterDetails = new ACC_EAUTH_MASTER();
                    masterDetails = objPaymentBAL.GetMasterAuthorizationDetails(objparams.BILL_ID);
                    objparams.FUND_TYPE = masterDetails.FUND_TYPE;
                    objparams.LVL_ID = masterDetails.LVL_ID;
                    objparams.ADMIN_ND_CODE = masterDetails.ADMIN_ND_CODE;
                    objparams.DISTRICT_CODE = PMGSYSession.Current.DistrictCode;
                    //objparams.TXN_ID = masterDetails.TXN_ID;
                    objparams.BILL_TYPE = "P";
                    objparams.OP_MODE = "A";
                    model.AGREEMENT_C = roadlist;
                    model.MAST_DPIU_CODEList = roadlist;
                    model.IMS_PR_ROAD_CODEList = roadlist;
                    List<SelectListItem> ContractorList = objCommon.PopulateContractorSupplier(objparams);
                    model.MAST_CON_ID_C1 = ContractorList;
                    roadlist.Clear();
                    roadlist.Add(new SelectListItem { Selected = true, Text = "--Select--", Value = "0" });
                    model.IMS_SANCTION_YEAR_List = roadlist;
                    model.IMS_SANCTION_PACKAGE_List = roadlist;
                    List<SelectListItem> List = new List<SelectListItem>();
                    List.Add(new SelectListItem { Selected = true, Text = "No", Value = "false" });
                    List.Add(new SelectListItem { Text = "Yes", Value = "true" });
                    model.final_pay = List;
                    obj = new ACC_EAUTH_DETAILS();
                    obj = dbContext.ACC_EAUTH_DETAILS.Where(x => x.EAUTH_ID == objparams.BILL_ID && x.EAUTH_TXN_NO == objparams.TXN_NO).FirstOrDefault();
                    model.MAST_CON_ID_C = obj.MAST_CON_ID;
                    model.TXN_NO = obj.EAUTH_TXN_NO;
                    objparams.MAST_CONT_ID = Convert.ToInt32(model.MAST_CON_ID_C);
                    model.IMS_AGREEMENT_CODE_C = obj.IMS_AGREEMENT_CODE;
                    model.AGREEMENT_AMOUNT = 0;
                    model.ALREADY_AUTHORISED_AMOUNT = 0;
                    model.AMOUNT_Q = obj.AMOUNT;
                    model.AGREEMENT_C = common.PopulateAgreement(objparams);
                    model.IMS_AGREEMENT_CODE_C = obj.IMS_AGREEMENT_CODE;
                    model.IsForUpdate = true;
                    model.PAYEE_NAME = objPaymentBAL.SetPayeeName(objparams);
                    model.IMS_SANCTION_PACKAGE = obj.IMS_PACKAGE_ID;
                    model.AGREEMENT_AMOUNT = obj.TEND_AGREEMENT_AMOUNT;
                    model.ALREADY_AUTHORISED_AMOUNT = obj.ALREADY_AUTH_AMOUNT;
                    if (obj.TEND_AGREEMENT_AMOUNT == null)
                    {
                        model.AGREEMENT_AMOUNT = 0;
                    }
                    else
                    {
                        model.AGREEMENT_AMOUNT = obj.TEND_AGREEMENT_AMOUNT;
                    }
                    //Get Expenditure Amount in Case of Edit..use Function to Get Exp Amt as we are not storing Exp Amount
                    decimal Expenditure_Amt = 0;
                    var result = dbContext.UDF_ACC_EAUTH_AGR_AUTH_AMT(PMGSYSession.Current.AdminNdCode, obj.MAST_CON_ID, obj.IMS_AGREEMENT_CODE, obj.IMS_PACKAGE_ID, PMGSYSession.Current.FundType);
                    if (result != null)
                    {
                        foreach (var item in result)
                        {
                            if (item.EXPN_AMOUNT == null)
                            {
                                Expenditure_Amt = 0;
                            }
                            else
                            {
                                Expenditure_Amt = Convert.ToDecimal(item.EXPN_AMOUNT);
                            }
                        }
                    }
                    else
                    {
                        Expenditure_Amt = 0;
                    }

                    model.EXPENDITURE_AMOUNT = Expenditure_Amt;
                    //Populate Package in Edit Mode
                    objparams.DISTRICT_CODE = PMGSYSession.Current.DistrictCode;
                    objparams.ADMIN_ND_CODE = PMGSYSession.Current.AdminNdCode;
                    objparams.AGREEMENT_CODE = Convert.ToInt32(model.IMS_AGREEMENT_CODE_C);
                    var varAggrement = (from aggmast in dbContext.TEND_AGREEMENT_DETAIL
                                        where
                                       aggmast.TEND_AGREEMENT_CODE == objparams.AGREEMENT_CODE
                                        select new
                                        {
                                            IMS_PACKAGE_ID = aggmast.IMS_SANCTIONED_PROJECTS.IMS_PACKAGE_ID
                                        }).Distinct().ToList();
                    foreach (var item in varAggrement)
                    {
                        lstTendDetails.Add(new SelectListItem { Text = item.IMS_PACKAGE_ID, Value = item.IMS_PACKAGE_ID });
                    }
                    lstTendDetails = lstTendDetails.OrderBy(m => m.Text).ToList();
                    lstTendDetails.Insert(0, (new SelectListItem { Text = "Select Package", Value = "0" }));
                    model.IMS_SANCTION_PACKAGE_List = lstTendDetails;
                    ViewBag.Bill_ID = URLEncrypt.EncryptParameters(new string[] { objparams.BILL_ID.ToString() });
                }
                return View("TransactionDetailsEntryForm", model);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "EAuthorization.EditEAuthorizationDetails()");
                return null;
            }
            finally
            {
                dbContext.Dispose();
            }
        }
        #endregion

        #region Update EAuthorization Details
        /// <summary>
        /// action to Update the EAuthorization Details
        /// </summary>
        /// <param name="model"></param>
        /// <param name="parameter"></param>
        /// <param name="hash"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        [HttpPost]
        [Audit]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateEAuthorizationDetails(EAuthorizationRequestDetailsModel model, String parameter, String hash, String key)
        {
            String paymentDeduction = string.Empty;
            PMGSYEntities dbContext = null;
            TransactionParams objparams = null;
            CommonFunctions objCommon = null;
            try
            {
                objCommon = new CommonFunctions();
                objPaymentBAL = new EAuthorizationBAL();
                dbContext = new PMGSYEntities();
                objparams = new TransactionParams();
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
                    return Json(new { Success = false, errMessage = "Error in Getting EAuthorization Details,Please try Again" });
                }

                model.EAUTH_ID = objparams.BILL_ID;
                if (model.EAUTH_ID == 0)
                {
                    return Json(new { Success = false, errMessage = "Error in Getting EAuthorization Details,Please try Again" });
                }
                else
                {
                    if (ModelState.IsValid)
                    {
                        if (PMGSYSession.Current.FundType == "P")
                        {
                        }
                        else
                        {
                            return Json(new { Success = false, errMessage = "e-Authorization request Entry can only be made in Programme fund" });
                        }


                        
                        if (model.MAST_CON_ID_C == null || model.IMS_AGREEMENT_CODE_C == 0)
                        {
                            return Json(new { Success = false, errMessage = "Contractor Name is Required" });
                        }
                        if (model.IMS_AGREEMENT_CODE_C == null || model.IMS_AGREEMENT_CODE_C == 0)
                        {
                            return Json(new { Success = false, errMessage = "Agreement Number is Required" });
                        }
                        if (model.IMS_SANCTION_PACKAGE == "0")
                        {
                            return Json(new { Success = false, errMessage = "Package is Required" });
                        }

                        //Package NIL check
                        //if (model.IMS_SANCTION_PACKAGE == "NIL")
                        //{
                        //    return Json(new { Success = false, errMessage = "Invalid Package,Please Select Another Package" });
                        //}
                        
                        if (model.AMOUNT_Q == null)
                        {
                            ModelState.AddModelError("AMOUNT_Q", "Amount is Required");
                            return Json(new { Success = false, errMessage = "EAuthorization Request Amount is Required" });
                        }
                        if (model.AMOUNT_Q == 0)
                        {
                            return Json(new { Success = false, errMessage = "Amount must be greater than 0" });
                        }
                        int lgdStateCode = dbContext.OMMAS_LDG_STATE_MAPPING.Where(x => x.MAST_STATE_CODE == PMGSYSession.Current.StateCode).Select(x => x.MAST_STATE_LDG_CODE).FirstOrDefault();
                        Int32 ConID = Convert.ToInt32(model.MAST_CON_ID_C);
                        bool checkbankDetails = dbContext.PFMS_OMMAS_CONTRACTOR_MAPPING.Any(x => x.MAST_CON_ID == ConID && x.MAST_LGD_STATE_CODE == lgdStateCode && x.PFMS_CON_ID != null && x.STATUS == "A" && x.MASTER_CONTRACTOR.MASTER_CONTRACTOR_BANK.Where(z => z.MAST_CON_ID == ConID && z.MAST_LOCK_STATUS == "Y").Select(b => b.MAST_ACCOUNT_STATUS).FirstOrDefault() == "A");
                        if (!checkbankDetails)
                        {
                            return Json(new { Success = false, errMessage = "Contractor bank details not present." });
                        }
                        EAuthorizationRequestDetailsModel responseModel = objPaymentBAL.UpdateEAuthorizationDetails(model);
                        if (responseModel != null)
                        {
                            if (responseModel.status)
                            {
                                return Json(new { Success = true });
                            }
                            else
                            {
                                return Json(new { Success = false, errMessage = responseModel.ErrMessage });
                            }
                        }
                        else
                        {
                            return Json(new { Success = false, errMessage = responseModel.ErrMessage });
                        }
                    }
                    else
                    {
                        return Json(new { Success = false, errMessage = "Unable to Update EAuthorization Details,Please try Again" });
                    }
                }

            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "EAuthorization.UpdateEAuthorizationDetails()");
                return null;
            }
        }

        #endregion

        #region Delete EAuthorization Details
        /// <summary>
        /// This Method Delets EAuthorization Detail
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="hash"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        [HttpPost]
        [Audit]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteEAuthorizationDetails(String parameter, String hash, String key)
        {
            PMGSYEntities dbContext = null;
            TransactionParams objparams = null;
            CommonFunctions objCommon = null;
            try
            {
                objCommon = new CommonFunctions();
                objPaymentBAL = new EAuthorizationBAL();
                dbContext = new PMGSYEntities();
                objparams = new TransactionParams();
                if (!String.IsNullOrEmpty(parameter) && !String.IsNullOrEmpty(hash) && !String.IsNullOrEmpty(key))
                {
                    String[] urlParams = URLEncrypt.DecryptParameters(new String[] { parameter, hash, key });
                    if (urlParams.Length >= 1)
                    {
                        String[] urlSplitParams = urlParams[0].Split('$');
                        objparams.BILL_ID = Convert.ToInt64(urlSplitParams[0]);
                        objparams.TXN_NO = Convert.ToInt16(urlSplitParams[1]);
                    }
                }
                else
                {
                    return Json(new { Success = false, errMessage = "Error in Getting EAuthorization Details,Please try Again" });
                }
                if (objparams.BILL_ID == 0)
                {
                    return Json(new { Success = false, errMessage = "Error in Getting EAuthorization Details,Please try Again" });
                }
                else
                {
                    EAuthorizationRequestDetailsModel responseModel = objPaymentBAL.DeleteEAuthorizationDetails(objparams.BILL_ID, objparams.TXN_NO);
                    if (responseModel == null)
                    {
                        return Json(new { Success = false, errMessage = "Unable to Delete EAuthorization Details,Please try Again" });
                    }
                    else
                    {
                        if (responseModel.status)
                        {
                            return Json(new { Success = true, Count = responseModel.count });
                        }
                        else
                        {
                            return Json(new { Success = false, errMessage = responseModel.ErrMessage });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "EAuthorization.DeleteEAuthorizationDetails()");
                return null;
            }
        }
        #endregion

        #region Finalize EAuthorization Details
        /// <summary>
        /// This Method Finalize EAuthorization Master Detail
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="hash"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        [HttpPost]
        [Audit]
        [ValidateAntiForgeryToken]
        public ActionResult FinalizeEAuthorizationDetails(String parameter, String hash, String key)
        {
            PMGSYEntities dbContext = null;
            TransactionParams objparams = null;
            CommonFunctions objCommon = null;
            try
            {
                objCommon = new CommonFunctions();
                objPaymentBAL = new EAuthorizationBAL();
                dbContext = new PMGSYEntities();
                objparams = new TransactionParams();
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
                    return Json(new { Success = false, errMessage = "Error in Getting EAuthorization Details,Please try Again" });
                }
                if (objparams.BILL_ID == 0)
                {
                    return Json(new { Success = false, errMessage = "Error in Getting EAuthorization Details,Please try Again" });
                }
                else
                {
                    EAuthorizationRequestDetailsModel responseModel = objPaymentBAL.FinalizeEAuthorizationDetails(objparams.BILL_ID);
                    if (responseModel == null)
                    {
                        return Json(new { Success = false, errMessage = "Unable to Finalize EAuthorization Details,Please try Again" });
                    }
                    else
                    {
                        if (responseModel.status)
                        {
                            return Json(new { Success = true });
                        }
                        else
                        {
                            return Json(new { Success = false, errMessage = responseModel.ErrMessage });

                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "EAuthorization.FinalizeEAuthorizationDetails()");
                return null;
            }

        }
        #endregion

        #region Delete EAuthorization Master
        /// <summary>
        /// This Method Deletes EAuthorization Master Detail
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="hash"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        [HttpPost]
        [Audit]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteEAuthorizationMaster(String parameter, String hash, String key)
        {
            PMGSYEntities dbContext = null;
            TransactionParams objparams = null;
            CommonFunctions objCommon = null;
            try
            {
                dbContext = new PMGSYEntities();
                objCommon = new CommonFunctions();
                objPaymentBAL = new EAuthorizationBAL();
                objparams = new TransactionParams();

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
                    return Json(new { Success = false, errMessage = "Error in Getting EAuthorization Details,Please try Again" });
                }
                if (objparams.BILL_ID == 0)
                {
                    return Json(new { Success = false, errMessage = "Error in Getting EAuthorization Details,Please try Again" });
                }
                else
                {
                    EAuthorizationRequestDetailsModel responseModel = objPaymentBAL.DeleteEAuthorizationMaster(objparams.BILL_ID);
                    if (responseModel == null)
                    {
                        return Json(new { Success = false, errMessage = "Unable to Delete EAuthorization Details,Please try Again" });
                    }
                    else
                    {
                        if (responseModel.status)
                        {
                            return Json(new { Success = true });
                        }
                        else
                        {
                            return Json(new { Success = false, errMessage = responseModel.ErrMessage });

                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "EAuthorization.DeleteEAuthorizationMaster()");
                return null;
            }
        }



        #endregion

        #region SRRDA Main View
        /// <summary>
        ///SRRDAeAuthorizationList
        /// </summary>
        /// <returns></returns>

        [Audit]
        public ActionResult SRRDAeAuthorizationList()
        {
            SRRDAeAuthorizationListModel Model = null;
            try
            {
                Model = new SRRDAeAuthorizationListModel();
                objPaymentBAL = new EAuthorizationBAL();
                common = new CommonFunctions();
                TransactionParams objParam = new TransactionParams();
                objParam.ADMIN_ND_CODE = PMGSYSession.Current.AdminNdCode;
                ViewBag.DPIU = objPaymentBAL.PopulateDPIUForSRRDA(objParam);
                ViewBag.ddlStatus = objPaymentBAL.PopulateSTATUSForSRRDA();
                Model.EAUTH_MONTH_LIST = objPaymentBAL.PopulateSTATUSForSRRDA();
                //Model.EAUTH_MONTH = Convert.ToInt16(DateTime.Now.Month);
                ViewBag.ddlMonth = common.PopulateMonths();
                Model.EAUTH_MONTH_LIST = common.PopulateMonths();
                //Model.EAUTH_YEAR = Convert.ToInt16(DateTime.Now.Year);
                ViewBag.ddlYear = common.PopulateYears(true);
                Model.EAUTH_YEAR_LIST = common.PopulateYears(true);
                return View(Model);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "EAuthorization.SRRDAeAuthorizationList()");
                return null;
            }
        }
        #endregion

        #region SRRDA Main View Data
        /// <summary>
        /// action to return EO grid data for Approval/Reject(Master Grid)
        /// </summary>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <param name="sidx"></param>
        /// <param name="sord"></param>
        /// <param name="parameter"></param>
        /// <param name="hash"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        [HttpPost]
        [Audit]
        public ActionResult SRRDAeAuthorizationRequestListData(int? page, int? rows, string sidx, string sord)
        {
            EAuthorizationFilterModel objFilter = null;
            try
            {
                objPaymentBAL = new EAuthorizationBAL();
                long totalRecords;
                objFilter = new EAuthorizationFilterModel();
                string loadGrid = Convert.ToString(Request.Params["Load"]);
                if (String.IsNullOrEmpty(loadGrid))   //View Button Click to Get EO List
                {
                    objFilter.Month = Convert.ToInt16(Request.Params["months"]);
                    objFilter.Year = Convert.ToInt16(Request.Params["year"]);
                    objFilter.AdminNdCode = Convert.ToInt32(Request.Params["AdminNDCode"]);
                    objFilter.StatusID = Convert.ToInt32(Request.Params["Status"]);
                    objFilter.LoadStr = loadGrid;
                    objFilter.page = Convert.ToInt32(page) - 1;
                    objFilter.rows = Convert.ToInt32(rows);
                    objFilter.sidx = sidx.ToString();
                    objFilter.sord = sord.ToString();
                    objFilter.FundType = PMGSYSession.Current.FundType;
                    objFilter.LevelId = PMGSYSession.Current.LevelId;
                }
                else   //On load 
                {

                    objFilter.ParentNdCode = Convert.ToInt32(PMGSYSession.Current.ParentNDCode);
                    objFilter.StateCode = PMGSYSession.Current.StateCode;
                    objFilter.page = Convert.ToInt32(page) - 1;
                    objFilter.rows = Convert.ToInt32(rows);
                    objFilter.sidx = sidx.ToString();
                    objFilter.sord = sord.ToString();
                    objFilter.LoadStr = loadGrid;
                    objFilter.FundType = PMGSYSession.Current.FundType;
                    objFilter.LevelId = PMGSYSession.Current.LevelId;
                }
                var jsonData = new
                {
                    rows = objPaymentBAL.SRRDAeAuthorizationRequestListData(objFilter, out totalRecords),
                    total = totalRecords <= objFilter.rows ? 1 : totalRecords / objFilter.rows + 1,
                    page = objFilter.page + 1,
                    records = totalRecords
                };

                return Json(jsonData);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "EAuthorization.SRRDAeAuthorizationRequestListData()");
                return null;
            }

        }
        #endregion

        #region SRRDA Details Grid
        /// <summary>
        /// action to return EO grid data for Approval/Reject(Detail Grid)
        /// </summary>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <param name="sidx"></param>
        /// <param name="sord"></param>
        /// <param name="parameter"></param>
        /// <param name="hash"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        [HttpPost]
        [Audit]
        public JsonResult GetSRRDAeAuthDetailListForApproval(int? page, int? rows, string sidx, string sord, String parameter, String hash, String key)
        {
            EAuthorizationFilterModel objFilter = null;
            try
            {
                objPaymentBAL = new EAuthorizationBAL();
                PMGSYEntities dbContext = new PMGSYEntities();
                objFilter = new EAuthorizationFilterModel();
                using (CommonFunctions commonFunction = new CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new GridParams(page, rows, sidx, sord, Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                    {
                        return null;
                    }
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

                string sEncId = URLEncrypt.EncryptParameters(new string[] { objFilter.BillId.ToString().Trim() });
                string status = dbContext.ACC_EAUTH_MASTER.Where(x => x.EAUTH_ID == objFilter.BillId).Select(x => x.EAUTH_STATUS).FirstOrDefault();
                string eAuthNo = dbContext.ACC_EAUTH_MASTER.Where(x => x.EAUTH_ID == objFilter.BillId).Select(x => x.EAUTH_NO).FirstOrDefault();
                int count = dbContext.ACC_EAUTH_DETAILS.Where(x => x.EAUTH_ID == objFilter.BillId && ((x.APPROVAL_STAUS == "A" || x.APPROVAL_STAUS == "R "))).Count();
                //if count >=1 that means:show continue Button....
                var jsonData = new
                {
                    rows = objPaymentBAL.GetSRRDAeAuthDetailListForApproval(objFilter, out totalRecords),
                    total = totalRecords <= objFilter.rows ? 1 : totalRecords / objFilter.rows + 1,
                    page = objFilter.page,
                    records = totalRecords,
                    hidEncId = sEncId,
                    Status = status,
                    EAUTHNO = eAuthNo,
                    Count = count
                };

                return Json(jsonData);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "EAuthorization.GetSRRDAeAuthDetailListForApproval()");
                return null;
            }
        }


        #endregion

        #region SRRDA proceed Approve Reject Details
        /// <summary>
        /// This Methods Saves EO Approval/Rejected 
        /// </summary>
        /// <param name="ApproveArr"></param>
        /// <param name="RejectArr"></param>
        /// <param name="EncryptedEAuthID"></param>
        /// <returns></returns>
        [HttpGet]
        [Audit]
        public ActionResult ProceedForApproveRejectDetails(string ApproveArr, string RejectArr, string EncryptedEAuthID)
        {
            EAuthorizationRequestDetailsModel model = null;
            string parameter = string.Empty;
            string hash = string.Empty;
            string key = string.Empty;
            SRRSEAuthorizationReportModel reportmodel = null;
            try
            {
                objPaymentBAL = new EAuthorizationBAL();
                model = new EAuthorizationRequestDetailsModel();
                reportmodel = new SRRSEAuthorizationReportModel();
                if (!String.IsNullOrEmpty(EncryptedEAuthID))
                {
                    String[] splitID = EncryptedEAuthID.Split('/');
                    parameter = splitID[0];
                    hash = splitID[1];
                    key = splitID[2];
                }
                Int64 EAuthID = 0;
                if (!String.IsNullOrEmpty(parameter) && !String.IsNullOrEmpty(hash) && !String.IsNullOrEmpty(key))
                {
                    String[] urlParams = URLEncrypt.DecryptParameters(new String[] { parameter, hash, key });
                    if (urlParams.Length >= 1)
                    {
                        String[] urlSplitParams = urlParams[0].Split('$');
                        EAuthID = Convert.ToInt64(urlSplitParams[0]);
                    }
                    model = objPaymentBAL.ProceedForApproveRejectDetails(ApproveArr, RejectArr, EAuthID);
                    if (model != null)
                    {
                        if (model.status)
                        {
                            return Json(new { success = true, ErrMessage = model.ErrMessage, EAUTH_NO = model.EAUTH_NO, ApprovalStatus = model.ApprovalStatus }, JsonRequestBehavior.AllowGet);
                        }
                        else
                        {
                            return Json(new { success = false, ErrMessage = model.ErrMessage }, JsonRequestBehavior.AllowGet);
                        }
                    }
                    else
                    {
                        return Json(new { success = false, ErrMessage = model.ErrMessage }, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    throw new Exception("Error while getting payment transaction list");
                }
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "EAuthorization.ProceedForApproveRejectDetails()");
                return null;
            }



        }


        #endregion

        /// <summary>
        /// This Methods Returns Total BAnk Euthorization Available..
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>

        [Audit]
        public JsonResult GetBankAuthorizationAvailable(String id)
        {
            TransactionParams objparams = new TransactionParams();
            objparams.ADMIN_ND_CODE = PMGSYSession.Current.AdminNdCode;
            objparams.FUND_TYPE = PMGSYSession.Current.FundType;
            objparams.MONTH = Convert.ToInt16(id.Split('$')[0]);
            objparams.YEAR = Convert.ToInt16(id.Split('$')[1]);
            objparams.LVL_ID = PMGSYSession.Current.LevelId;
            UDF_ACC_GEN_GET_BA_CASH_CLOSING_BALANCES_Result balance = new UDF_ACC_GEN_GET_BA_CASH_CLOSING_BALANCES_Result();
            balance = objPaymentBAL.GetBankAuthorizationAvailable(objparams);
            return Json(new
            {
                Cash = (balance.cash / 100000),
                BankAuth = (balance.bank_auth / 100000)
            });
        }

        #region SRRDA View Report
        /// <summary>
        /// This Methods Returns MVC e-Authorization Report
        /// </summary>
        /// <param name="EncryptedEAuthID"></param>
        /// <returns></returns>
        /// 
        [HttpPost]
        [Audit]
        public ActionResult GetSRRDAEAuthorizationReport(string EncryptedEAuthID)
        {
            string parameter = string.Empty;
            string hash = string.Empty;
            string key = string.Empty;
            PMGSYEntities dbcontext = null;
            SRRDAeAuthorizationListModel model = null;
            Int64 EAuthID = 0;
            try
            {
                model = new SRRDAeAuthorizationListModel();
                dbcontext = new PMGSYEntities();
                if (!String.IsNullOrEmpty(EncryptedEAuthID))
                {
                    String[] splitID = EncryptedEAuthID.Split('/');
                    parameter = splitID[0];
                    hash = splitID[1];
                    key = splitID[2];
                }

                if (!String.IsNullOrEmpty(parameter) && !String.IsNullOrEmpty(hash) && !String.IsNullOrEmpty(key))
                {
                    String[] urlParams = URLEncrypt.DecryptParameters(new String[] { parameter, hash, key });
                    if (urlParams.Length >= 1)
                    {
                        String[] urlSplitParams = urlParams[0].Split('$');
                        EAuthID = Convert.ToInt64(urlSplitParams[0]);
                        model.EAUTH_ID = EAuthID;
                        model.PIU_ADMIN_ND_CODE = dbcontext.ACC_EAUTH_MASTER.Where(x => x.EAUTH_ID == model.EAUTH_ID).Select(x => x.ADMIN_ND_CODE).FirstOrDefault();
                        model.EncryptedEAuthID = URLEncrypt.EncryptParameters(new string[] { EAuthID.ToString().Trim() });
                        model.StateCode = Convert.ToInt32(PMGSYSession.Current.StateCode);
                        model.FUND_TYPE = PMGSYSession.Current.FundType;
                        model.EO_ADMIN_ND_CODE = PMGSYSession.Current.AdminNdCode;
                    }
                }
                return View("SRRDAReportLayout", model);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "EAuthorization.GetSRRDAEAuthorizationReport()");
                return null;
            }
        }

        #endregion

        #region Check Send Mail Status
        /// <summary>
        /// This Method Returns Mail Send Status
        /// </summary>
        /// <param name="EncryptedEAuthID"></param>
        /// <returns></returns>
        /// 
        [HttpPost]
        [Audit]
        public ActionResult CheckSendMailStatus(string EncryptedEAuthID)
        {
            PMGSYEntities dbContext = null;
            string parameter = string.Empty;
            string hash = string.Empty;
            string key = string.Empty;
            Int64 EAuthID = 0;
            try
            {
                dbContext = new PMGSYEntities();
                if (!String.IsNullOrEmpty(EncryptedEAuthID))
                {
                    String[] splitID = EncryptedEAuthID.Split('/');
                    parameter = splitID[0];
                    hash = splitID[1];
                    key = splitID[2];
                }
                else
                {
                    return Json(new { Success = false });
                }

                if (!String.IsNullOrEmpty(parameter) && !String.IsNullOrEmpty(hash) && !String.IsNullOrEmpty(key))
                {
                    String[] urlParams = URLEncrypt.DecryptParameters(new String[] { parameter, hash, key });
                    if (urlParams.Length >= 1)
                    {
                        String[] urlSplitParams = urlParams[0].Split('$');
                        EAuthID = Convert.ToInt64(urlSplitParams[0]);
                        string status = dbContext.ACC_EAUTH_MASTER.Where(x => x.EAUTH_ID == EAuthID).Select(x => x.EAUTH_STATUS).FirstOrDefault();
                        if (!String.IsNullOrEmpty(status))
                        {
                            return Json(new { Success = true, status = status });
                        }
                        else
                        {
                            return Json(new { Success = false });
                        }
                    }
                    else
                    {
                        return Json(new { Success = false });
                    }
                }
                else
                {
                    return Json(new { Success = false });
                }
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "EAuthorization.CheckSendMailStatus()");
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

        #region Get EAuthorization Details After Finalization(In popUP) Not Used
        /// <summary>
        /// This Method Returns List of EAUthorization Details After Finalization
        /// </summary>
        /// <param name="EncryptedEAuthID"></param>
        /// <returns></returns>
        [HttpPost]
        [Audit]
        public ActionResult GetEAuthorizationDetailsViewAfterFinalize(string EncryptedEAuthID)
        {
            string parameter = string.Empty;
            string hash = string.Empty;
            string key = string.Empty;
            SRRDAeAuthorizationListModel model = null;
            PMGSYEntities dbContext = new PMGSYEntities();
            ACC_EAUTH_MASTER objMaster = null;
            Int64 EAuthID = 0;
            try
            {
                model = new SRRDAeAuthorizationListModel();
                objMaster = new ACC_EAUTH_MASTER();
                if (!String.IsNullOrEmpty(EncryptedEAuthID))
                {
                    String[] splitID = EncryptedEAuthID.Split('/');
                    parameter = splitID[0];
                    hash = splitID[1];
                    key = splitID[2];
                }
                if (!String.IsNullOrEmpty(parameter) && !String.IsNullOrEmpty(hash) && !String.IsNullOrEmpty(key))
                {
                    String[] urlParams = URLEncrypt.DecryptParameters(new String[] { parameter, hash, key });
                    if (urlParams.Length >= 1)
                    {
                        String[] urlSplitParams = urlParams[0].Split('$');
                        EAuthID = Convert.ToInt64(urlSplitParams[0]);
                        model.EAUTH_ID = EAuthID;
                    }
                    objMaster = dbContext.ACC_EAUTH_MASTER.Where(x => x.EAUTH_ID == model.EAUTH_ID).FirstOrDefault();
                    if (objMaster != null)
                    {
                        model.EAUTH_NO = objMaster.EAUTH_NO;
                        string EAUTH_DATE = GetDateTimeToString(objMaster.EAUTH_DATE);
                        model.EAUTH_DATE = EAUTH_DATE;
                        if (objMaster.FUND_TYPE == "P")
                        {
                            model.FUND_TYPE = "Programme Fund";
                        }
                        else if (objMaster.FUND_TYPE == "M")
                        {
                            model.FUND_TYPE = "Maintenance Fund";
                        }
                        else if (objMaster.FUND_TYPE == "A")
                        {
                            model.FUND_TYPE = "Administrative  Fund";
                        }
                        model.REQUEST_AMOUNT = Convert.ToString(objMaster.TOTAL_AUTH_AMOUNT_REQ);
                        model.TOTAL_APPROVE_AMOUNT = Convert.ToString(objMaster.TOTAL_AUTH_APPROVED);
                    }
                    else
                    {
                        model.EAUTH_NO = "-";
                        model.EAUTH_DATE = "-";
                        model.FUND_TYPE = "-";
                        model.REQUEST_AMOUNT = "-";
                        model.TOTAL_APPROVE_AMOUNT = "-";
                    }
                }
                return View("GetEAuthorizationDetailsViewAfterFinalize", model);

            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "EAuthorization.GetEAuthorizationDetailsViewAfterFinalize()");
                return null;
            }
        }
        #endregion

        #region SRRDA Send Mail

        /// <summary>
        /// This Method Sends EAuthorization Mail to Bank
        /// </summary>
        /// <param name="EncryptedEAuthID"></param>
        /// <returns></returns>
        /// 
        [HttpPost]
        [Audit]
        [ValidateAntiForgeryToken]
        public ActionResult SendEAuthorizationMail(string EncryptedEAuthID)
        {
            PMGSYEntities dbContext = null;
            SRRDAeAuthorizationListModel model = null;
            SRRSEAuthorizationReportModel reportmodel = null;
            EAuthorizationRequestDetailsModel objmodel = null;
            EAuthorizationBAL objBal = null;
            Int64 EAuthID = 0;
            string parameter = string.Empty;
            string hash = string.Empty;
            string key = string.Empty;
            ACC_EAUTH_MASTER objMaster = null;
            string file = string.Empty;
            string OutPutFile = string.Empty;
            string userPassword = string.Empty;
            string ownerPassword = string.Empty;
            ACC_BANK_DETAILS bakDetails = null;
            string BANK_EMAIL_TO = string.Empty;
            string BCC = string.Empty;
            string SRRDA_EMAIL = string.Empty;
            string SRRDA_PASSWORD = string.Empty;
            string BANK_PASSWORD = string.Empty;
            int _AdminNdCode = 0;
            int _ParendAdminNdCode = 0;
            try
            {
                model = new SRRDAeAuthorizationListModel();
                reportmodel = new SRRSEAuthorizationReportModel();
                dbContext = new PMGSYEntities();
                objBal = new EAuthorizationBAL();
                objmodel = new EAuthorizationRequestDetailsModel();
                bakDetails = new ACC_BANK_DETAILS();
                if (!String.IsNullOrEmpty(EncryptedEAuthID))
                {
                    String[] splitID = EncryptedEAuthID.Split('/');
                    parameter = splitID[0];
                    hash = splitID[1];
                    key = splitID[2];
                }

                if (!String.IsNullOrEmpty(parameter) && !String.IsNullOrEmpty(hash) && !String.IsNullOrEmpty(key))
                {
                    String[] urlParams = URLEncrypt.DecryptParameters(new String[] { parameter, hash, key });
                    if (urlParams.Length >= 1)
                    {
                        String[] urlSplitParams = urlParams[0].Split('$');
                        EAuthID = Convert.ToInt64(urlSplitParams[0]);
                        model.EAUTH_ID = EAuthID;
                    }
                }

                objMaster = dbContext.ACC_EAUTH_MASTER.Where(x => x.EAUTH_ID == model.EAUTH_ID).FirstOrDefault();
                if ((objMaster.EAUTH_STATUS == "A" || objMaster.EAUTH_STATUS == "P")) //If Mail Already Send...than Show Error Msg
                {
                    return Json(new { success = false, Message = "Email is Already Sent" });
                }

                bool connection = NetworkInterface.GetIsNetworkAvailable();
                if (connection)
                {
                    reportmodel.UserName = ConfigurationManager.AppSettings["MvcReportViewer.Username"]; //UserName for MVC Report Viewer
                    reportmodel.Password = ConfigurationManager.AppSettings["MvcReportViewer.Password"]; //Password for MVC Report Viewer
                    reportmodel.ReportServerUrl = ConfigurationManager.AppSettings["MvcReportViewer.ReportServerUrl"]; //MVC Report Viewer URL
                    reportmodel.FolderNameAndReportName = "/PMGSYCitizen/EAuthorization";  //e-Authorization Report URL
                    Int32 StateCode = Convert.ToInt32(PMGSYSession.Current.StateCode);
                    String FundType = PMGSYSession.Current.FundType;
                    model.PIU_ADMIN_ND_CODE = dbContext.ACC_EAUTH_MASTER.Where(x => x.EAUTH_ID == model.EAUTH_ID).Select(x => x.ADMIN_ND_CODE).FirstOrDefault();
                    model.FUND_TYPE = PMGSYSession.Current.FundType;
                    model.EO_ADMIN_ND_CODE = PMGSYSession.Current.AdminNdCode;

                    #region EMAIL ID AND Password
                    _AdminNdCode = PMGSYSession.Current.AdminNdCode;
                    _ParendAdminNdCode = PMGSYSession.Current.ParentNDCode.Value;
                    BCC = ConfigurationManager.AppSettings["EpayBCC"].ToString(); //omms.pmgsy@nic.in
                    int? parentNdVode = dbContext.ADMIN_DEPARTMENT.Where(b => b.ADMIN_ND_CODE == _AdminNdCode).Select(d => d.MAST_PARENT_ND_CODE).First();
                    if (parentNdVode.HasValue)
                    {
                        bakDetails = dbContext.ACC_BANK_DETAILS.Where(f => f.ADMIN_ND_CODE == parentNdVode.Value && f.FUND_TYPE == PMGSYSession.Current.FundType && f.BANK_ACC_STATUS == true && f.ACCOUNT_TYPE == "S").First();
                        BANK_EMAIL_TO = bakDetails.BANK_EMAIL;
                        BANK_PASSWORD = bakDetails.Bank_SEC_CODE;

                        if (dbContext.ACC_EPAY_MAIL_OTHER.Where(m => m.ADMIN_ND_CODE == _ParendAdminNdCode).Any())
                        {
                            SRRDA_EMAIL = dbContext.ACC_EPAY_MAIL_OTHER.Where(m => m.ADMIN_ND_CODE == _ParendAdminNdCode).Select(m => m.EMAIL_CC).FirstOrDefault();
                            SRRDA_PASSWORD = dbContext.ACC_EPAY_MAIL_OTHER.Where(m => m.ADMIN_ND_CODE == _ParendAdminNdCode).Select(m => m.PDF_OPEN_KEY).FirstOrDefault();
                        }
                        else
                        {
                            SRRDA_EMAIL = "";
                            SRRDA_PASSWORD = "";
                        }
                    }

                    #endregion

                    reportmodel.ReportParameter = new { EauthID = EAuthID, StateCode = StateCode, FundType = FundType, PiuAdminNDCode = model.PIU_ADMIN_ND_CODE, EoAdminNDCode = model.EO_ADMIN_ND_CODE };  //Parameters required for SSRS report.

                    if (reportmodel.ReportParameter != null)
                        reportmodel.QueryString = CreateQueryStringForSSRSReport(reportmodel.ReportParameter);

                    reportmodel.FileBytes = ConvertSSRReportToBytes(reportmodel); //Get Byte Array of Report
                    var filename = "e-AuthSample_" + EAuthID + ".pdf";
                    var EncryptedFileName = "e-Auth_" + EAuthID + ".pdf";
                    string e_AuthorizationSendMailFilePath = ConfigurationManager.AppSettings["e_AuthorizationSendMailFilePath"];
                    if (!Directory.Exists(e_AuthorizationSendMailFilePath))
                    {
                        Directory.CreateDirectory(e_AuthorizationSendMailFilePath);
                    }
                    
                    string e_AuthorizationSendMailEncryptedFilePath = ConfigurationManager.AppSettings["e_AuthorizationSendMailEncryptedFilePath"];
                    if (!Directory.Exists(e_AuthorizationSendMailEncryptedFilePath))
                    {
                        Directory.CreateDirectory(e_AuthorizationSendMailEncryptedFilePath);
                    }
                    file = e_AuthorizationSendMailFilePath + filename;   //File Path AND File Name
                    OutPutFile = e_AuthorizationSendMailEncryptedFilePath + EncryptedFileName; //Encrypted Path AND File Name
                    System.Diagnostics.Process proc = new System.Diagnostics.Process();
                    if (System.IO.File.Exists(file))
                    {
                        System.IO.File.Delete(file);  //if File already Exist...Delete that File AND Save new One
                        System.IO.File.WriteAllBytes(file, reportmodel.FileBytes);  //Saving New One
                        Stream input = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.Read);
                        Stream output = new FileStream(OutPutFile, FileMode.Create, FileAccess.Write, FileShare.None);
                        PdfReader reader = new PdfReader(input);
                        //PdfEncryptor.Encrypt(reader, output, true, userPassword, ownerPassword, PdfWriter.ALLOW_PRINTING);
                        PdfEncryptor.Encrypt(reader, output, true, BANK_PASSWORD, SRRDA_PASSWORD, PdfWriter.ALLOW_PRINTING);
                        reader.Close();
                        output.Close();
                        input.Close();

                        System.IO.File.Delete(file);
                    }
                    else
                    {
                        System.IO.File.WriteAllBytes(file, reportmodel.FileBytes);  //Saving File at Location 
                        Stream input = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.Read);
                        Stream output = new FileStream(OutPutFile, FileMode.Create, FileAccess.Write, FileShare.None);
                        PdfReader reader = new PdfReader(input);
                        PdfEncryptor.Encrypt(reader, output, true, BANK_PASSWORD, SRRDA_PASSWORD, PdfWriter.ALLOW_PRINTING);
                        reader.Close();
                        output.Close();
                        input.Close();
                        System.IO.File.Delete(file);
                    }

                    #region Sending Mail
                    Int32 stateCode = PMGSYSession.Current.StateCode;
                    string StateName = dbContext.MASTER_STATE.Where(x => x.MAST_STATE_CODE == stateCode).Select(x => x.MAST_STATE_NAME).FirstOrDefault();
                    objMaster = dbContext.ACC_EAUTH_MASTER.Where(x => x.EAUTH_ID == EAuthID).FirstOrDefault();
                    string DPIUName = dbContext.ADMIN_DEPARTMENT.Where(x => x.ADMIN_ND_CODE == objMaster.ADMIN_ND_CODE).Select(x => x.ADMIN_ND_NAME).FirstOrDefault();
                    Int32 SRRDA_Admin_ND_Code = PMGSYSession.Current.AdminNdCode;
                    string SRRDAName = dbContext.ADMIN_DEPARTMENT.Where(x => x.ADMIN_ND_CODE == SRRDA_Admin_ND_Code).Select(x => x.ADMIN_ND_NAME).FirstOrDefault();
                    Int32 adminNDCode = PMGSYSession.Current.AdminNdCode;
                    string Amt = Convert.ToString(objMaster.TOTAL_AUTH_APPROVED);
                    AmountToWord ObjamtToWord = new AmountToWord();
                    string AmountInWords = ObjamtToWord.RupeesToWord(Amt.ToString());
                    string ReplacedStr = AmountInWords.Replace("Only", "").Replace("  ", " ");
                    DateTime approvedDate = DateTime.Now;
                    string Approveddate = approvedDate.ToString("dd/MM/yyyy");
                    StringBuilder msgBody = new StringBuilder();
                    msgBody.Append("Dear Sir / Madam , <br/><br/>");
                    msgBody.Append("An e- Authorization is made by" + " " + StateName + "-" + SRRDAName + "to" + " " + DPIUName + "of Rs." + objMaster.TOTAL_AUTH_APPROVED + " " + "Lakhs." + "(" + ReplacedStr + " " + "Lakhs." + ")" + "Dated on" + " " + Approveddate + "." + "<br/><br/>");
                    msgBody.Append("<br/>");
                    msgBody.Append("Refer attached pdf document for details.<br/><br/>");
                    msgBody.Append("<br/><br/>");
                    msgBody.Append("With Regards,<br/>");
                    msgBody.Append("OMMAS Team.<br/><br/>");
                    msgBody.Append("<b>Note:</b>  This is a system generated mail. Please do not reply back to this email ID. If you have a query or need any clarification you may contact the concerned SRRDA<br/><br/>");
                    msgBody.Append("<b><u>CONFIDENTIALITY INFORMATION AND DISCLAIMER</u></b> <br/>");
                    msgBody.Append("This email message and its attachments may contain confidential, proprietary or legally privileged information and is intended solely for the use of the " +
                                   " individual or entity to whom it is addressed. If you have erroneously received this message, please delete it immediately and notify the sender. If " +
                                   " you are not the intended recipient of the email message you should not disseminate, distribute or copy this e-mail. E-mail transmission cannot be guaranteed " +
                                   " to be secure or error-free as information could be intercepted, corrupted, lost, destroyed, incomplete or contain viruses and the OMMAS team " +
                                   " accepts no liability for any damage caused by the limitations of the e-mail transmission.");

                    // Added by Srishti on 13-03-2023
                    ServicePointManager.ServerCertificateValidationCallback += (s, ce, ca, p) => true;
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

                    MailMessage msg = new MailMessage();
                    //msg.From = new MailAddress("gavinash@cdac.in");
                    msg.From = new MailAddress(ConfigurationManager.AppSettings["eAuthMailFrom"].ToString());

                    //To:Bank Mail
                    msg.To.Add(BANK_EMAIL_TO);
                    //msg.To.Add(ConfigurationManager.AppSettings["eAuthMailTo"].ToString());

                    //CC:SRRDA
                    msg.CC.Add(SRRDA_EMAIL);
                    //msg.CC.Add(ConfigurationManager.AppSettings["eAuthMailCC1"].ToString());
                    

                    msg.IsBodyHtml = true;
                    msg.Body = msgBody.ToString();
                    msg.Subject = "E-Authorization Details";
                    msg.Priority = MailPriority.High;

                    //MAIL CONFIGURATION
                    SmtpClient client = new SmtpClient();
                    //Local Credentials
                    string e_EuthHost = ConfigurationManager.AppSettings["e_EuthHost"];
                    string e_EuthPort = ConfigurationManager.AppSettings["e_EuthPort"];
                    string e_EuthMailUserName = ConfigurationManager.AppSettings["e_EuthMailUserName"];
                    string e_EuthMailPassword = ConfigurationManager.AppSettings["e_EuthMailPassword"];
                    client.Host = e_EuthHost;//"smtp.cdac.in";
                    client.Port = Convert.ToInt32(e_EuthPort);//587;
                    //client.UseDefaultCredentials = true; // Commented by Srishti
                    client.UseDefaultCredentials = false; // Added by Srishti on 14-03-2023
                    client.Credentials = new NetworkCredential(e_EuthMailUserName, e_EuthMailPassword);
                    client.EnableSsl = true;
                    if (System.IO.File.Exists(OutPutFile))
                    {
                        System.Net.Mail.Attachment attachment;
                        attachment = new System.Net.Mail.Attachment(OutPutFile);
                        msg.Attachments.Add(attachment);
                    }


                    try
                    {
                        client.Send(msg);
                    }
                    catch (Exception ex)
                    {

                        ErrorLog.LogError(ex, "EAuthorization.SendEAuthorizationMail()");
                        
                    }
                    msg.Dispose();
                    #endregion

                }
                else
                {
                    return Json(new { success = false, Message = "Network Connectivity is not Available,Please check Connectivity before Sending Mail" });

                }

                // return Json(new { success = true });


                #region Update Status After Sending Mail
                objMaster = dbContext.ACC_EAUTH_MASTER.Where(x => x.EAUTH_ID == EAuthID).FirstOrDefault();
                if (objMaster != null)
                {
                    int count = dbContext.ACC_EAUTH_DETAILS.Where(x => x.EAUTH_ID == EAuthID).Count();
                    int arrroval = dbContext.ACC_EAUTH_DETAILS.Where(x => x.APPROVAL_STAUS == "A" && x.EAUTH_ID == EAuthID).Count();
                    if (arrroval == 0)
                    {
                        objMaster.EAUTH_STATUS = "R";
                    }
                    else if (count == arrroval)
                    {
                        objMaster.EAUTH_STATUS = "A";
                    }
                    else
                    {
                        objMaster.EAUTH_STATUS = "P";
                    }
                    objMaster.APPOVAL_DATE_SRRDA = DateTime.Now;
                    dbContext.Entry(objMaster).State = System.Data.Entity.EntityState.Modified;
                    dbContext.SaveChanges();
                    #region Entry in ACC_NOTIFICATION_DETAILS

                    objmodel = objBal.SaveNotificationDetailsAfterSendingMail(EncryptedEAuthID);
                    if (objmodel != null)
                    {
                        if (objmodel.status)
                        {
                            return Json(new { success = true });
                        }
                        else
                        {
                            return Json(new { success = false, Message = "Failure in Sending Mail,Please try Again" });
                        }
                    }
                    else
                    {
                        return Json(new { success = false, Message = "Failure in Sending Mail,Please try Again" });
                    }

                    #endregion
                }
                else
                {
                    if (System.IO.File.Exists(file))
                    {
                        System.IO.File.Delete(file);
                    }
                    return Json(new { success = false, Message = "Failure in Sending Mail,Please try Again" });
                }

                #endregion
            }
            catch (Exception ex)
            {
                if (System.IO.File.Exists(file))
                {
                    System.IO.File.Delete(file);
                }
                ErrorLog.LogError(ex, "EAuthorization.SendEAuthorizationMail()");
                return null;
            }

        }

        #endregion

        public string CreateQueryStringForSSRSReport(object parameter)
        {
            try
            {
                string queryString = "&";
                PropertyInfo[] paramProperties = parameter.GetType().GetProperties();
                if (paramProperties.Length == 0)
                    return string.Empty;
                for (int i = 0; i < paramProperties.Length; i++)
                {
                    queryString += paramProperties[i].Name + "=" + paramProperties[i].GetValue(parameter) + "&";
                }

                queryString = queryString.TrimEnd('&');
                return queryString;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public byte[] ConvertSSRReportToBytes(SRRSEAuthorizationReportModel model)
        {
            try
            {
                string sub = model.ReportParameter.ToString();
                string queryString = model.QueryString;
                string strReportUser = model.UserName;
                string strReportUserPW = model.Password;
                string sTargetURL = model.ReportServerUrl + "?" + model.FolderNameAndReportName + "&rs:Command=Render&rs:format=PDF" + queryString;
                HttpWebRequest req =
                      (HttpWebRequest)WebRequest.Create(sTargetURL);
                req.PreAuthenticate = true;
                req.Credentials = new System.Net.NetworkCredential(
                    strReportUser,
                    strReportUserPW);
                HttpWebResponse HttpWResp = (HttpWebResponse)req.GetResponse();
                Stream fStream = HttpWResp.GetResponseStream();
                //Now turn around and send this as the response..
                byte[] fileBytes = ReadFully(fStream);
                return fileBytes;
                //return fStream;
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "EAuthorization.ConvertSSRReportToBytes()");
                return null;
            }
        }

        public static byte[] ReadFully(Stream input)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                input.CopyTo(ms);
                return ms.ToArray();
            }
        }

        public static DateTime GetStringToDateTime(string strDate)
        {
            string[] formats = { "dd/MM/yyyy" };
            DateTime newDate;
            bool boolDate = DateTime.TryParse(DateTime.ParseExact(strDate, formats[0], null).ToString(), out newDate);
            if (boolDate)
            {
                return newDate;
            }
            else
            {
                throw new Exception("Invalid Date. Error in Parsing");
            }
        }

        public String GetDateTimeToString(DateTime date)
        {
            return date.ToString("dd/MM/yyyy");
        }

        #region check Already Authorised Amount
        /// <summary>
        /// This Method Returns Agreement Amount And Already Authorised Amount
        /// </summary>
        /// <param name="PackageValue"></param>
        /// <returns></returns>
        [HttpGet]
        [Audit]
        public ActionResult CheckAlreadyAuthorisedAmount(string PackageValue)
        {
            PMGSYEntities dbContext = null;
            EAuthorizationAmountRequestModel model = null;
            EAuthorizationBAL objBal = null;
            try
            {
                model = new EAuthorizationAmountRequestModel();
                objBal = new EAuthorizationBAL();
                string[] arr = PackageValue.Split('$');
                dbContext = new PMGSYEntities();
                if (!String.IsNullOrEmpty(PackageValue))
                {
                    model.ADMIN_ND_CODE = PMGSYSession.Current.AdminNdCode;
                    model.MAST_CONT_ID = Convert.ToInt32(arr[0]);
                    model.AGREEMENT_CODE = Convert.ToInt32(arr[1]);
                    model.IMS_SANCTION_PACKAGE = arr[2];
                }
                else
                {
                    return Json(new { Success = false, errMessage = "Error occur while Getting Already Authorised Amount,Please try again" }, JsonRequestBehavior.AllowGet);
                }
                model = objBal.CheckAlreadyAuthorisedAmount(model);
                if (model != null)
                {
                    return Json(new { Success = true, AuthorisedAmount = model.AMOUNT_AUTHORIZED }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { Success = false, errMessage = "Error occur while Getting Already Authorised Amount,Please try again" }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "EAuthorization.CheckAlreadyAuthorisedAmount()");
                return null;
            }


        }

        #endregion

        #region Populate Agreement/Already Authorised Amount
        /// <summary>
        /// This Method Returns Agreement Amount And Already Authorised Amount
        /// </summary>
        /// <param name="strAmtValue"></param>
        /// <returns></returns>
        [HttpGet]
        [Audit]
        public ActionResult PopulateAgreementExpenditureAmount(string strAmtValue)
        {
            PMGSYEntities dbContext = null;
            try
            {
                string[] arr = strAmtValue.Split('$');
                dbContext = new PMGSYEntities();
                decimal Auth_Amount = 0;
                decimal tend_Aggrement_Amount = 0;
                decimal Expenditure_Amount = 0;
                Int32 AdminNDCode = PMGSYSession.Current.AdminNdCode;
                Int32 MAST_CONT_ID = Convert.ToInt32(arr[0]);
                Int32 AGREEMENT_CODE = Convert.ToInt32(arr[1]);
                string package = arr[2];
                string fundType = PMGSYSession.Current.FundType;

                //Function to Get Agreement Amount AND Already Authorised Amount
                var result = dbContext.UDF_ACC_EAUTH_AGR_AUTH_AMT(AdminNDCode, MAST_CONT_ID, AGREEMENT_CODE, package, fundType);
                if (result != null)
                {
                    foreach (var item in result)
                    {
                        if (item.AUTH_AMOUNT == null)
                        {
                            Auth_Amount = 0;
                        }
                        else
                        {
                            Auth_Amount = Convert.ToDecimal(item.AUTH_AMOUNT);
                        }
                        if (item.TEND_AGREEMENT_AMOUNT == null)
                        {
                            tend_Aggrement_Amount = 0;
                        }
                        else
                        {
                            tend_Aggrement_Amount = Convert.ToDecimal(item.TEND_AGREEMENT_AMOUNT);
                        }
                        if (item.EXPN_AMOUNT == null)
                        {
                            Expenditure_Amount = 0;
                        }
                        else
                        {
                            Expenditure_Amount = Convert.ToDecimal(item.EXPN_AMOUNT);
                        }
                    }
                    return Json(new { Success = true, Auth_Amount = Auth_Amount, tend_Aggrement_Amount = tend_Aggrement_Amount, Expenditure_Amount = Expenditure_Amount }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { Success = false, errMessage = "Error occur while Getting Amount Details,Please try again" }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "EAuthorization.PopulateAmountForTransaction()");
                return null;
            }

        }

        #endregion

        #region GetAddEAuthorizationLinkView
        [HttpGet]
        public ActionResult GetAddEAuthorizationLinkView(string ConIDAggrementPackageValue)
        {
            string MASTCONID = string.Empty;
            string AggrementCode = string.Empty;
            string Package = string.Empty;
            EAuthorizationAmountRequestModel model = null;
            EAuthorizationBAL objEAuthBAL = null;
            PMGSYEntities dbContext = null;

            try
            {
                model = new EAuthorizationAmountRequestModel();
                objEAuthBAL = new EAuthorizationBAL();
                dbContext = new PMGSYEntities();
                if (!String.IsNullOrEmpty(ConIDAggrementPackageValue))
                {
                    String[] splitID = ConIDAggrementPackageValue.Split('$');
                    MASTCONID = splitID[0];
                    AggrementCode = splitID[1];
                    Package = splitID[2];
                }

                if (!String.IsNullOrEmpty(MASTCONID) && !String.IsNullOrEmpty(AggrementCode) && !String.IsNullOrEmpty(Package))
                {
                    model.MAST_CONT_ID = Convert.ToInt32(MASTCONID);
                    model.AGREEMENT_CODE = Convert.ToInt32(AggrementCode);
                    model.IMS_SANCTION_PACKAGE = Package;

                    //Getting Agrement Amount on Partial View in Order to Validate already authorised Amount
                    //already authorised Amount < Agrement Amount
                    var result = dbContext.UDF_ACC_EAUTH_AGR_AUTH_AMT(PMGSYSession.Current.AdminNdCode, model.MAST_CONT_ID, model.AGREEMENT_CODE, model.IMS_SANCTION_PACKAGE, PMGSYSession.Current.FundType);
                    foreach (var item in result)
                    {
                        decimal tend_Aggrement_Amount = Convert.ToDecimal(item.TEND_AGREEMENT_AMOUNT);
                        model.AGREEMENT_AMOUNT = tend_Aggrement_Amount;
                    }
                }

                
                



                model = objEAuthBAL.GetAddEAuthorizationLinkView(model);
                return PartialView("GetAddEAuthorizationLinkView", model);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "EAuthorization.GetAddEAuthorizationLinkView()");
                return null;
            }

        }

        #endregion

        #region Add New AuthorizationEntry(Popup)
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
        public ActionResult AddNewAuthorizationEntry(EAuthorizationAmountRequestModel model, String parameter, String hash, String key)
        {
            Int64 EAuth_ID = 0;
            EAuthorizationBAL objEAuthorizationBAL = null;
            EAuthorizationAmountRequestModel objEAuthorizationAmountRequestModel = null;
            PMGSYEntities dbContext = null;
            try
            {
                dbContext = new PMGSYEntities();
                objEAuthorizationBAL = new EAuthorizationBAL();
                objEAuthorizationAmountRequestModel = new EAuthorizationAmountRequestModel();
                if (!String.IsNullOrEmpty(parameter) && !String.IsNullOrEmpty(hash) && !String.IsNullOrEmpty(key))
                {
                    String[] urlParams = URLEncrypt.DecryptParameters(new String[] { parameter, hash, key });
                    if (urlParams.Length >= 1)
                    {
                        String[] urlSplitParams = urlParams[0].Split('$');
                        EAuth_ID = Convert.ToInt64(urlSplitParams[0]);
                        model.EAuth_ID = EAuth_ID;
                    }
                }
                else
                {
                    return Json(new { Success = false, errMessage = "Error Occur while Adding new Authorization Details,please try Again " });
                }

                if (ModelState.IsValid)
                {
                    if (model.AUTHORIZATION_AMOUNT == 0)
                    {
                        return Json(new { Success = false, errMessage = "Authorization Amount Should be Greater than 0" });
                    }

                    //Authorization Amount should be less than Agreement Amount
                    var result = dbContext.UDF_ACC_EAUTH_AGR_AUTH_AMT(PMGSYSession.Current.AdminNdCode, model.MAST_CONT_ID, model.AGREEMENT_CODE, model.IMS_SANCTION_PACKAGE, PMGSYSession.Current.FundType);
                    foreach (var item in result)
                    {
                        decimal tend_Aggrement_Amount = Convert.ToDecimal(item.TEND_AGREEMENT_AMOUNT);
                        model.AGREEMENT_AMOUNT = tend_Aggrement_Amount;
                    }


                    if (model.AUTHORIZATION_AMOUNT > model.AGREEMENT_AMOUNT)
                    {
                        return Json(new { Success = false, errMessage = "Authorization Amount Should be less than Agreement Amount" });
                    }
                    
                    bool EAuthorizationStatus = objEAuthorizationBAL.AddNewAuthorizationEntry(model);
                    if (EAuthorizationStatus)
                    {
                        return Json(new { Success = true, AuthorisedAmount = model.AUTHORIZATION_AMOUNT });
                    }
                    else
                    {
                        return Json(new { Success = false, errMessage = "Error Occur while Adding new Authorization Details,please try Again " });
                    }
                }
                else
                {
                    return Json(new { Success = false, errMessage = "Error Occur while Adding new Authorization Details,please try Again " });
                }
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "EAuthorization.AddNewAuthorizationEntry()");
                return null;
            }


        }
        #endregion

        #endregion
    }
}
