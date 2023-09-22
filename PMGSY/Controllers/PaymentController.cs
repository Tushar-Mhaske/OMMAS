using PMGSY.Common;
using PMGSY.Models.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PMGSY.Extensions;
using PMGSY.Models;
using PMGSY.BAL.Payment;
using System.Data.Entity.Validation;
using Newtonsoft.Json;
using PMGSY.Models.PaymentModel;
using PMGSY.Models.Payment;

using RazorPDF;
using System.IO;
using iTextSharp.text.pdf;
using iTextSharp.text;
using System.Net;
using iTextSharp.text.html.simpleparser;
using System.Web.UI;
using System.Text.RegularExpressions;
using iTextSharp.text.xml;
using System.Configuration;
using Mvc.Mailer;

using PMGSY.BAL.DigSign;
using PMGSY.DigSignDocs;
using PMGSY.Models.DigSign;

using iTextSharp.text.pdf.draw;
using PMGSY.DAL.PFMS;
using PMGSY.DAL.Payment;
using PMGSY.Areas.REAT.DAL;

using System.Xml;
using System.Xml.Linq;
using System;
using System.IO;
using System.Collections;
using System.Net.Mail;

namespace PMGSY.Controllers
{
    // [RequiredAuthentication]
    //[RequiredAuthorization]
    public class PaymentController : Controller
    {
        CommonFunctions common = null;
        IPaymentBAL objPaymentBAL = null;

        public PaymentController()
        {
            common = new CommonFunctions();
            objPaymentBAL = new PaymentBAL();
        }

        [Audit]
        //Commented temperory for payuthaldwani issue
        [GenericAccountValidationFilter(InputParameter = new string[] { "BankDetails", "ChequeBookDetails", "OpeningBalanceDetails", "AuthSign" })]
        public ActionResult GetPaymentList(String id)
        {
            TransactionParams objparams = new TransactionParams();
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
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                string errorMsg = string.Format("Errors: {0}", ex.Message);
                Response.TrySkipIisCustomErrors = true;
                Response.StatusCode = 500;
                Response.Write(errorMsg);
                return Json(string.Empty);
            }

        }

        /// <summary>
        /// this is the  page action on which the master and details forms are dynamically rendered while adding the new master details
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Audit]
        public ActionResult MasterDetailsPayment(String id)
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

        }

        /// <summary>
        /// this is the action master and details forms are dynamically rendered while editing  master details
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="hash"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        [HttpGet]
        [Audit]
        public ActionResult MasterDetailsPaymentEdit(String parameter, String hash, String key)
        {
            TransactionParams objparams = new TransactionParams();

            Int64 Bill_Id = 0;

            if (!String.IsNullOrEmpty(parameter) && !String.IsNullOrEmpty(hash) && !String.IsNullOrEmpty(key))
            {
                String[] urlParams = URLEncrypt.DecryptParameters(new String[] { parameter, hash, key });
                if (urlParams.Length >= 1)
                {

                    Bill_Id = Convert.ToInt64(urlParams[0]);

                }
            }
            else
            {
                throw new Exception("Error while getting master payment details..");
            }

            ACC_BILL_MASTER masterDetails = new ACC_BILL_MASTER();
            masterDetails = objPaymentBAL.GetMasterPaymentDetails(Bill_Id);

            if (masterDetails.CHQ_EPAY == "A")//Advice no change for 8Apr2015
            {
                masterDetails.CHQ_EPAY = "Q";
            }

            ViewBag.month = masterDetails.BILL_MONTH;
            ViewBag.year = masterDetails.BILL_YEAR;
            ViewBag.operationType = "E";

            ViewBag.Bill_id = URLEncrypt.EncryptParameters(new string[] { Bill_Id.ToString() });

            ACC_SCREEN_DESIGN_PARAM_MASTER obj = new ACC_SCREEN_DESIGN_PARAM_MASTER();

            objparams.TXN_ID = masterDetails.TXN_ID;
            obj = common.getMasterDesignParam(objparams);

            if (obj.DED_REQ == "B")
            {

                ViewBag.DeductionScreenRequired = (masterDetails.CASH_AMOUNT != 0 && (masterDetails.CHQ_EPAY == "Q" || masterDetails.CHQ_EPAY == "E")) ? true : masterDetails.CHQ_EPAY == "C" ? true : false;

                //If Condition Added By Abhishek kamble 16-Apr-2014 start
                if ((objparams.TXN_ID == 47) || (objparams.TXN_ID == 737))
                {
                    ViewBag.PaymentScreenRequired = true;
                }
                else
                {
                    ViewBag.PaymentScreenRequired = masterDetails.CHQ_AMOUNT != 0 && (masterDetails.CHQ_EPAY == "Q" || masterDetails.CHQ_EPAY == "E") ? true : false;
                }
            }
            else
            {
                //If Condition Added By Abhishek kamble 16-Apr-2014 start
                if (objparams.TXN_ID == 47)
                {
                    ViewBag.PaymentScreenRequired = true;
                }
                else
                {
                    ViewBag.PaymentScreenRequired = (masterDetails.CHQ_AMOUNT != 0 && (masterDetails.CHQ_EPAY == "Q" || masterDetails.CHQ_EPAY == "E")) || (masterDetails.CHQ_EPAY == "C") ? true : false;
                }


                ViewBag.DeductionScreenRequired = masterDetails.CASH_AMOUNT != 0 && (masterDetails.CHQ_EPAY == "Q" || masterDetails.CHQ_EPAY == "E") ? true : false;
            }

            return View("MasterDetailsPayment");
        }


        /// <summary>
        /// function to populater the supplier agreement
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

                            objparams.BILL_ID = Convert.ToInt64(urlParams[0]);
                        }
                    }
                    else
                    {
                        throw new Exception("Error while getting  payment details..");
                    }
                }


                //get the master transaction details
                ACC_BILL_MASTER masterDetails = new ACC_BILL_MASTER();
                masterDetails = objPaymentBAL.GetMasterPaymentDetails(objparams.BILL_ID);

                objparams.BILL_TYPE = masterDetails.BILL_TYPE;
                objparams.FUND_TYPE = masterDetails.FUND_TYPE;
                objparams.LVL_ID = masterDetails.LVL_ID;
                objparams.ADMIN_ND_CODE = masterDetails.ADMIN_ND_CODE;
                objparams.MAST_CONT_ID = masterDetails.MAST_CON_ID.HasValue ? masterDetails.MAST_CON_ID.Value : 0;
                objparams.DISTRICT_CODE = PMGSYSession.Current.DistrictCode;
                objparams.TXN_ID = masterDetails.TXN_ID;

                return Json(common.PopulateAgreement(objparams));
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
        /// Action to get the head details
        /// </summary>
        /// <param name="id"> transaction id</param>
        /// <returns></returns>
        [Audit]
        public JsonResult GetHeadDetails(String parameter, String hash, String key, string id)
        {
            try
            {
                if (!String.IsNullOrEmpty(id))
                {
                    Int16 transId = Convert.ToInt16(id.Split('$')[0]);

                    String billId = String.Empty;

                    String billType = "P";

                    if (!String.IsNullOrEmpty(parameter) && !String.IsNullOrEmpty(hash) && !String.IsNullOrEmpty(key))
                    {

                        if (!String.IsNullOrEmpty(parameter) && !String.IsNullOrEmpty(hash) && !String.IsNullOrEmpty(key))
                        {
                            String[] urlParams = URLEncrypt.DecryptParameters(new String[] { parameter, hash, key });
                            if (urlParams.Length >= 1)
                            {

                                billId = urlParams[0];
                            }
                        }
                        else
                        {
                            throw new Exception("Error while getting head details..");
                        }
                    }


                    common = new CommonFunctions();
                    return (Json(common.GetTransNarration(transId, billId, billType)));
                }
                else
                {
                    throw new Exception("Error while getting head details..");
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
        /// function t o get the piu
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="hash"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        [Audit]
        public JsonResult GetPIU(String parameter, String hash, String key)
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
                            if (urlParams[0].Contains("$"))
                            {
                                objparams.BILL_ID = Convert.ToInt64(urlParams[0].Split('$')[0]);
                                objparams.TXN_NO = Convert.ToInt16(urlParams[0].Split('$')[1]);
                            }
                            else
                            {
                                objparams.BILL_ID = Convert.ToInt64(urlParams[0]);
                                objparams.TXN_NO = 1;
                            }
                        }
                    }
                    else
                    {
                        throw new Exception("Error while getting  payment details..");
                    }
                }


                //get the master transaction details
                ACC_BILL_MASTER masterDetails = new ACC_BILL_MASTER();
                masterDetails = objPaymentBAL.GetMasterPaymentDetails(objparams.BILL_ID);

                objparams.BILL_TYPE = masterDetails.BILL_TYPE;
                objparams.FUND_TYPE = masterDetails.FUND_TYPE;
                objparams.LVL_ID = masterDetails.LVL_ID;
                objparams.ADMIN_ND_CODE = masterDetails.ADMIN_ND_CODE;
                objparams.MAST_CONT_ID = masterDetails.MAST_CON_ID.HasValue ? masterDetails.MAST_CON_ID.Value : 0;
                objparams.DISTRICT_CODE = PMGSYSession.Current.DistrictCode;
                objparams.STATE_CODE = PMGSYSession.Current.StateCode;
                objparams.TXN_ID = masterDetails.TXN_ID;

                //new change done by Vikram on 08 Jan 2014
                PMGSYEntities db = new PMGSYEntities();
                int? adminCode = 0;
                if (masterDetails.BILL_TYPE == "P")
                {
                    adminCode = db.ACC_BILL_DETAILS.Where(m => m.BILL_ID == objparams.BILL_ID && m.TXN_NO == objparams.TXN_NO).Select(m => m.ADMIN_ND_CODE).FirstOrDefault();
                }
                else
                {
                    adminCode = db.ACC_BILL_DETAILS.Where(m => m.BILL_ID == objparams.BILL_ID).Select(m => m.ADMIN_ND_CODE).FirstOrDefault();
                }
                List<SelectListItem> lstPIU = new List<SelectListItem>();
                var list = db.ADMIN_DEPARTMENT.Where(m => m.MAST_STATE_CODE == PMGSYSession.Current.StateCode && m.MAST_ND_TYPE == "S").ToList();
                foreach (var item in list)
                {
                    if (item.ADMIN_ND_CODE == adminCode)
                    {
                        lstPIU.Add(new SelectListItem { Value = item.ADMIN_ND_CODE.ToString(), Text = item.ADMIN_ND_NAME, Selected = true });
                    }
                    else
                    {
                        lstPIU.Add(new SelectListItem { Value = item.ADMIN_ND_CODE.ToString(), Text = item.ADMIN_ND_NAME });
                    }
                }
                lstPIU.Insert(0, new SelectListItem { Value = "0", Text = "--Select--" });
                //end of change


                //return Json(common.PopulateDPIU(objparams));
                return Json(lstPIU);
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
        ///  this action is used to render the payment and deduction forms of transactions 
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="hash"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        [HttpGet]
        [Audit]
        public ActionResult PartialTransaction(String parameter, String hash, String key)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            try
            {

                TransactionParams objparams = new TransactionParams();
                PaymentDetailsModel model = new PaymentDetailsModel();
                var CHQ_AMOUNT = Request.QueryString["CHQ_AMOUNT"];


                if (!String.IsNullOrEmpty(parameter) && !String.IsNullOrEmpty(hash) && !String.IsNullOrEmpty(key))
                {

                    if (!String.IsNullOrEmpty(parameter) && !String.IsNullOrEmpty(hash) && !String.IsNullOrEmpty(key))
                    {
                        String[] urlParams = URLEncrypt.DecryptParameters(new String[] { parameter, hash, key });
                        if (urlParams.Length >= 1)
                        {

                            objparams.BILL_ID = Convert.ToInt64(urlParams[0]);
                        }
                    }
                    else
                    {
                        throw new Exception("Error while getting master payment details..");
                    }
                }

                List<SelectListItem> roadlist = new List<SelectListItem>();
                roadlist.Add(new SelectListItem { Selected = true, Text = "--Select--", Value = "" });


                //get the master transaction details
                ACC_BILL_MASTER masterDetails = new ACC_BILL_MASTER();
                masterDetails = objPaymentBAL.GetMasterPaymentDetails(objparams.BILL_ID);

                //if Added By Abhishek kamble for Advice No 6Apr2015 start
                if (masterDetails.CHQ_EPAY == "A")
                {
                    masterDetails.CHQ_EPAY = "Q";
                }
                //Added By Abhishek kamble for Advice No 6Apr2015 end

                objparams.BILL_TYPE = masterDetails.BILL_TYPE;
                objparams.FUND_TYPE = masterDetails.FUND_TYPE;
                objparams.LVL_ID = masterDetails.LVL_ID;
                objparams.ADMIN_ND_CODE = masterDetails.ADMIN_ND_CODE;
                objparams.MAST_CONT_ID = masterDetails.MAST_CON_ID.HasValue ? masterDetails.MAST_CON_ID.Value : 0;
                objparams.DISTRICT_CODE = PMGSYSession.Current.DistrictCode;
                objparams.TXN_ID = masterDetails.TXN_ID;

                model.CONTRACTOR_ID = masterDetails.MAST_CON_ID.HasValue ? masterDetails.MAST_CON_ID.Value : 0;


                objparams.OP_MODE = "A";

                List<SelectListItem> subHeadList = new List<SelectListItem>();

                subHeadList = common.PopulateTransactions(objparams);

                model.HeadId_P = new List<SelectListItem>(subHeadList);


                //for remittance payment set the payment transaction same as remittance office set in master payment
                if (masterDetails.TXN_ID != 462)
                {
                    if (masterDetails.REMIT_TYPE != null)
                    {
                        foreach (var item in subHeadList)
                        {
                            if (item.Value != string.Empty)
                            {
                                //get the head id 
                                int head = Convert.ToInt32(item.Value.Split('$')[0]);

                                //get the order of it from master txn table 
                                int orderOfTxn = dbContext.ACC_MASTER_TXN.Where(x => x.TXN_ID == head).Select(x => x.TXN_ORDER).FirstOrDefault();

                                //check if order matches or remove from subtransaction list
                                if (orderOfTxn != masterDetails.REMIT_TYPE)
                                {
                                    model.HeadId_P.Remove(item);
                                }
                            }
                        }
                    }
                }

                subHeadList.Clear();

                subHeadList = null;

                model.AGREEMENT_S = roadlist;

                model.AGREEMENT_C = roadlist;

                model.MAST_DPIU_CODEList = roadlist;

                model.IMS_PR_ROAD_CODEList = roadlist;

                //agreement for deduction
                ViewBag.DeductionAgreementRequired = objparams.MAST_CONT_ID == 0 ? false : true;

                model.AGREEMENT_DED = common.PopulateAgreement(objparams);

                objparams.BILL_TYPE = "D";

                model.HeadId_D = common.PopulateTransactions(objparams);

                roadlist.Clear();

                roadlist.Add(new SelectListItem { Selected = true, Text = "--Select--", Value = "0" });

                model.mast_CON_ID_CON1 = roadlist;

                model.IMS_SANCTION_YEAR_List = roadlist;

                model.IMS_SANCTION_PACKAGE_List = roadlist;


                List<SelectListItem> List = new List<SelectListItem>();
                List.Add(new SelectListItem { Selected = false, Text = "Yes", Value = "true" });
                List.Add(new SelectListItem { Selected = true, Text = "No", Value = "false" });
                
                model.final_pay = List;

                #region if for master payment multiple transsaction is not allowed

                ACC_SCREEN_DESIGN_PARAM_MASTER obj = new ACC_SCREEN_DESIGN_PARAM_MASTER();
                obj = common.getMasterDesignParam(objparams);

                bool disablePayHead = false;

                if (obj.MTXN_REQ == "N")
                {
                    if (!dbContext.ACC_BILL_DETAILS.Where(d => d.BILL_ID == masterDetails.BILL_ID).Any())
                    {
                        disablePayHead = false;
                    }
                    else
                    {
                        Int16? txn = dbContext.ACC_BILL_DETAILS.Where(d => d.BILL_ID == masterDetails.BILL_ID).Select(c => c.TXN_ID).First();
                        string CheCash = dbContext.ACC_MASTER_TXN.Where(m => m.TXN_ID == txn).Select(m => m.CASH_CHQ).FirstOrDefault().Trim();
                        model.HEAD_ID_P = txn + "$" + CheCash;
                        disablePayHead = true;
                    }
                }

                #endregion

                ViewBag.disablePaymentHead = disablePayHead;

                ViewBag.Bill_ID = URLEncrypt.EncryptParameters(new string[] { objparams.BILL_ID.ToString() });

                ViewBag.BillFinalized = masterDetails.BILL_FINALIZED;

                if (obj.DED_REQ == "B")
                {

                    ViewBag.DeductionScreenRequired = (masterDetails.CASH_AMOUNT != 0 && (masterDetails.CHQ_EPAY == "Q" || masterDetails.CHQ_EPAY == "E")) ? true : masterDetails.CHQ_EPAY == "C" ? true : false;
                    //ViewBag.DeductionScreenRequired = ((masterDetails.CHQ_EPAY == "Q" || masterDetails.CHQ_EPAY == "E")) ? true : masterDetails.CHQ_EPAY == "C" ? true : false;


                    //if (((objparams.TXN_ID == 47) || (objparams.TXN_ID == 737) || (objparams.TXN_ID == 1484)) && (masterDetails.CHQ_EPAY == "Q"))

                    //Avinash :Condition for RCPLWE
                    if (((objparams.TXN_ID == 47) || (objparams.TXN_ID == 737) || (objparams.TXN_ID == 1484) || (objparams.TXN_ID == 1788)) && (masterDetails.CHQ_EPAY == "Q"))
                    {
                        long BillId = Convert.ToInt64(objparams.BILL_ID);

                        if (dbContext.ACC_BILL_DETAILS.Where(m => m.BILL_ID == BillId).Any())
                        {
                            if (dbContext.ACC_BILL_MASTER.Where(m => m.BILL_ID == BillId && m.CHQ_AMOUNT == 0).Any())
                            {
                                ViewBag.PaymentScreenRequired = false;
                            }
                            else
                            {
                                ViewBag.PaymentScreenRequired = true;
                            }
                        }
                        else
                        {
                            ViewBag.PaymentScreenRequired = true;
                        }
                    }//47-Contractor Work Payment 72-Advances To Contractor 86-Refund of deposite to contractor 777-Payment of Adv To Contractor 787-Refund of Deposite to contracor supplier
                    ///Added TXN_ID=1484 by SAMMED A. PATIL to display Payment screen for Contractors Payment PMGSY2
                    //else if (((objparams.TXN_ID == 47) || (objparams.TXN_ID == 72) || (objparams.TXN_ID == 86) || (objparams.TXN_ID == 737) || (objparams.TXN_ID == 777) || (objparams.TXN_ID == 314) || (objparams.TXN_ID == 787) || (objparams.TXN_ID == 415) || (objparams.TXN_ID == 1484)) && (masterDetails.CHQ_EPAY == "C"))
                    else if (((objparams.TXN_ID == 47) || (objparams.TXN_ID == 72) || (objparams.TXN_ID == 86) || (objparams.TXN_ID == 1326) || (objparams.TXN_ID == 1484) || (objparams.TXN_ID == 1788) || (objparams.TXN_ID == 1974) || (objparams.TXN_ID == 737) || (objparams.TXN_ID == 777) || (objparams.TXN_ID == 314) || (objparams.TXN_ID == 787) || (objparams.TXN_ID == 415) || (objparams.TXN_ID == 1484)) && (masterDetails.CHQ_EPAY == "C"))
                    {
                        ViewBag.PaymentScreenRequired = true;
                    }
                    else
                    {
                        //Below Line is commented on 03-12-2021 to enable Add Payment Details Screen
                        //ViewBag.PaymentScreenRequired = masterDetails.CHQ_AMOUNT != 0 && (masterDetails.CHQ_EPAY == "Q" || masterDetails.CHQ_EPAY == "E") ? true : false;

                        //Below Line is Added on 03-12-2021 to enable Add Payment Details Screen
                        ViewBag.PaymentScreenRequired = (masterDetails.CHQ_EPAY == "Q" || masterDetails.CHQ_EPAY == "E") ? true : false;
                    }
                }
                else
                {

                    if (((objparams.TXN_ID == 47) || (objparams.TXN_ID == 737) || (objparams.TXN_ID == 1484)) && (masterDetails.CHQ_EPAY == "Q"))
                    {
                        long BillId = Convert.ToInt64(objparams.BILL_ID);

                        if (dbContext.ACC_BILL_DETAILS.Where(m => m.BILL_ID == BillId).Any())
                        {
                            ViewBag.PaymentScreenRequired = false;
                        }
                        else
                        {
                            ViewBag.PaymentScreenRequired = true;
                        }
                    }
                    else
                    {
                        //Below Line is commented on 03-12-2021 to enable Add Payment Details Screen
                        //ViewBag.PaymentScreenRequired = (masterDetails.CHQ_AMOUNT != 0 && (masterDetails.CHQ_EPAY == "Q" || masterDetails.CHQ_EPAY == "E")) || (masterDetails.CHQ_EPAY == "C") ? true : false;

                        //Below Line is Added on 03-12-2021 to enable Add Payment Details Screen
                        ViewBag.PaymentScreenRequired = (masterDetails.CHQ_EPAY == "Q" || masterDetails.CHQ_EPAY == "E") || masterDetails.CHQ_EPAY == "C" ? true : false;
                    }
                    ViewBag.DeductionScreenRequired = masterDetails.CASH_AMOUNT != 0 && (masterDetails.CHQ_EPAY == "Q" || masterDetails.CHQ_EPAY == "E") ? true : false;
                    //ViewBag.DeductionScreenRequired = (masterDetails.CHQ_EPAY == "Q" || masterDetails.CHQ_EPAY == "E") ? true : false;
                }

                ViewBag.disableDeductionHead = false;


                //Added By Abhishek Kamble 1-jan-2013
                ViewBag.IsMultiTranAllowed = obj.MTXN_REQ;
                ViewBag.ReceiptGrossAmount = masterDetails.GROSS_AMOUNT;
                ViewBag.ChequeAmount = masterDetails.CHQ_AMOUNT;
                ViewBag.CashAmount = masterDetails.CASH_AMOUNT;
                ViewBag.urlparams = URLEncrypt.EncryptParameters(new string[] { objparams.BILL_ID.ToString() });

                //Added by Abhishek kamble 24-June-2014 to show lable cash for cash payment such 'payment to nodal agency'
                ViewBag.ChqEpayFlag = masterDetails.CHQ_EPAY;
                ViewBag.TxnIdFlag = masterDetails.TXN_ID;
                //Added By Bhushan
                ViewBag.CHQ_AMOUNT = CHQ_AMOUNT;

                return View(model);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return Json(string.Empty);
            }
            finally
            {

                dbContext.Dispose();
            }

        }

        /// <summary>
        /// function to populate the roads based on the agreement selected
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        [Audit]
        public JsonResult PopulateRoad(string id)
        {

            try
            {
                // new change done by Vikram on 30-08-2013
                string[] param = id.Split('$'); //change
                TransactionParams objparams = new TransactionParams();
                objparams.AGREEMENT_CODE = Convert.ToInt32(param[0]);
                if (param.Count() > 1)
                {
                    objparams.HEAD_ID = Convert.ToInt16(param[1]); //change
                }

                if (!String.IsNullOrEmpty(param[3]) && param[3] != "undefined")
                {
                    objparams.MAST_CONT_ID = Convert.ToInt32(param[3]);
                }

                if (!String.IsNullOrEmpty(Request.Params["AGREEMENT_NUMBER"]))
                {
                    objparams.AGREEMENT_NUMBER = Request.Params["AGREEMENT_NUMBER"];
                }

                return Json(common.PopulateRoad(objparams));
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return Json(string.Empty);

                /* string errorMsg = string.Format("Errors: {0}", ex.Message);
                 Response.TrySkipIisCustomErrors = true;
                 Response.StatusCode = 500;
                 Response.Write(errorMsg);*/

            }

        }

        #region QCR Record Status
        [HttpPost]
        [Audit]
        public ActionResult GetQCRRecordStatus(String parameter, String hash, String key, string roadCode)
        {
            try
            {
                
                PMGSYEntities dbContext = new PMGSYEntities();
                string status = null;
                string prop_type = null;
                string exec_compl = null;
                string QCRRecordStatus=null;
                long? BILL_ID = null;
                int? Txn_Id = null;

                if (!String.IsNullOrEmpty(parameter) && !String.IsNullOrEmpty(hash) && !String.IsNullOrEmpty(key))
                {

                    if (!String.IsNullOrEmpty(parameter) && !String.IsNullOrEmpty(hash) && !String.IsNullOrEmpty(key))
                    {
                        String[] urlParams = URLEncrypt.DecryptParameters(new String[] { parameter, hash, key });
                        if (urlParams.Length >= 1)
                        {

                            BILL_ID = Convert.ToInt64(urlParams[0]);
                        }
                    }
                    else
                    {
                        throw new Exception("Error while getting master payment details..");
                    }
                }

                if (BILL_ID != null)
                {
                    Txn_Id = dbContext.ACC_BILL_MASTER.Where(x => x.BILL_ID == BILL_ID).Select(x => x.TXN_ID).FirstOrDefault();
                }

                //if ((!(roadCode == null || roadCode == string.Empty || roadCode=="0")) && Txn_Id != 72 && Txn_Id != 86 && Txn_Id != 105 && Txn_Id != 109)
                if ((!(roadCode == null || roadCode == string.Empty || roadCode=="0")) && Txn_Id != 72 && Txn_Id != 86 && Txn_Id != 105 && Txn_Id != 109 && Txn_Id != 1488 && !(PMGSYSession.Current.UserId == 1315 && PMGSYSession.Current.StateCode == 5 && DateTime.Now.Date <= new CommonFunctions().GetStringToDateTime("22/10/2022")) )
                {
                    int ims_pr_road_Code = Convert.ToInt32(roadCode);
                    prop_type = dbContext.IMS_SANCTIONED_PROJECTS.Where(x => x.IMS_PR_ROAD_CODE == ims_pr_road_Code).Select(x => x.IMS_PROPOSAL_TYPE).FirstOrDefault();

                    //var record = dbContext.EXEC_ROADS_MONTHLY_STATUS.Where(x => x.IMS_PR_ROAD_CODE == ims_pr_road_Code).OrderByDescending(x => x.EXEC_PROG_YEAR).ThenByDescending(x => x.EXEC_PROG_MONTH).FirstOrDefault();
                    if (prop_type == "P" || prop_type == "L")
                    {
                        exec_compl =
                        prop_type == "P"
                        ?
                        dbContext.EXEC_ROADS_MONTHLY_STATUS.Where(x => x.IMS_PR_ROAD_CODE == ims_pr_road_Code).OrderByDescending(x => x.EXEC_PROG_YEAR).ThenByDescending(x => x.EXEC_PROG_MONTH).Select(x => x.EXEC_ISCOMPLETED).Any() ? dbContext.EXEC_ROADS_MONTHLY_STATUS.Where(x => x.IMS_PR_ROAD_CODE == ims_pr_road_Code).OrderByDescending(x => x.EXEC_PROG_YEAR).ThenByDescending(x => x.EXEC_PROG_MONTH).Select(x => x.EXEC_ISCOMPLETED).FirstOrDefault() : null
                        :
                        dbContext.EXEC_LSB_MONTHLY_STATUS.Where(x => x.IMS_PR_ROAD_CODE == ims_pr_road_Code).OrderByDescending(x => x.EXEC_PROG_YEAR).ThenByDescending(x => x.EXEC_PROG_MONTH).Select(x => x.EXEC_ISCOMPLETED).Any() ? dbContext.EXEC_LSB_MONTHLY_STATUS.Where(x => x.IMS_PR_ROAD_CODE == ims_pr_road_Code).OrderByDescending(x => x.EXEC_PROG_YEAR).ThenByDescending(x => x.EXEC_PROG_MONTH).Select(x => x.EXEC_ISCOMPLETED).FirstOrDefault() : null;

                        if (exec_compl != "C")
                        {
                            //QM_QCR_DETAILS data = new QM_QCR_DETAILS();
                            QCRRecordStatus = dbContext.QM_QCR_DETAILS.Where(x => x.IMS_PR_ROAD_CODE == ims_pr_road_Code && x.IS_FINALIZE == "Y").Any() ? "Y" : "N";
                        }
                        if (QCRRecordStatus == "N")
                        {
                            return Json(new { status = false });
                        }
                    }


                    //exec_compl =
                    //    prop_type == "P"
                    //    ?
                    //    dbContext.EXEC_ROADS_MONTHLY_STATUS.Where(x => x.IMS_PR_ROAD_CODE == ims_pr_road_Code).OrderByDescending(x => x.EXEC_PROG_YEAR).ThenByDescending(x => x.EXEC_PROG_MONTH).Select(x => x.EXEC_ISCOMPLETED).Any() ? dbContext.EXEC_ROADS_MONTHLY_STATUS.Where(x => x.IMS_PR_ROAD_CODE == ims_pr_road_Code).OrderByDescending(x => x.EXEC_PROG_YEAR).ThenByDescending(x => x.EXEC_PROG_MONTH).Select(x => x.EXEC_ISCOMPLETED).FirstOrDefault() : null
                    //    :
                    //    dbContext.EXEC_LSB_MONTHLY_STATUS.Where(x => x.IMS_PR_ROAD_CODE == ims_pr_road_Code).OrderByDescending(x => x.EXEC_PROG_YEAR).ThenByDescending(x => x.EXEC_PROG_MONTH).Select(x => x.EXEC_ISCOMPLETED).Any() ? dbContext.EXEC_LSB_MONTHLY_STATUS.Where(x => x.IMS_PR_ROAD_CODE == ims_pr_road_Code).OrderByDescending(x => x.EXEC_PROG_YEAR).ThenByDescending(x => x.EXEC_PROG_MONTH).Select(x => x.EXEC_ISCOMPLETED).FirstOrDefault():null;

                    //    if (exec_compl != "C")
                    //    {
                    //        //QM_QCR_DETAILS data = new QM_QCR_DETAILS();
                    //        QCRRecordStatus = dbContext.QM_QCR_DETAILS.Where(x => x.IMS_PR_ROAD_CODE == ims_pr_road_Code && x.IS_FINALIZE=="Y").Any() ? "Y":"N";
                    //    }
                    //    if (QCRRecordStatus=="N")
                    //    {
                    //        return Json(new { status=false});
                    //    }
                }
                else
                {
                    return Json(string.Empty);
                }

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
            return null;
        }

        #endregion

        #region VTS Validation Status
        [HttpPost]
        [Audit]
        public ActionResult GetVTSValidationStatus(String parameter, String hash, String key, string roadCode)
        {
            PMGSYEntities dbContext = new PMGSYEntities();

            try
            {
                int ims_pr_road_Code = Convert.ToInt32(roadCode);
               // int ims_pr_road_Code = 231450;// 230663;// 230662;


                int roadScheme = dbContext.IMS_SANCTIONED_PROJECTS.Where(x => x.IMS_PR_ROAD_CODE == ims_pr_road_Code && x.IMS_PROPOSAL_TYPE == "P").Select(x => x.MAST_PMGSY_SCHEME).FirstOrDefault();

                if (roadScheme == 4)//Check Only if Road Exists under PMGSY-III scheme
                {


                    //added by rohit borse on 15-09-2023 for GPS VTS state relaxation to enter financial progress
                    string GPSVTS_StatesRelaxed = System.Configuration.ConfigurationManager.AppSettings["GPS_VTS_STATE_RELAXATION"];
                    if (!string.IsNullOrEmpty(GPSVTS_StatesRelaxed))
                    {
                        string[] GPSVTS_StatesRelaxedList = GPSVTS_StatesRelaxed.Split(',');
                        if (GPSVTS_StatesRelaxedList.Contains(PMGSYSession.Current.StateCode.ToString()))
                        {
                            return Json(new { status = true });
                        }
                    }


                    bool isVTSLastDateExceed = false;
                    DateTime VTS_ENTRY_LASTDATE = Convert.ToDateTime(ConfigurationManager.AppSettings["VTS_ENTRY_LASTDATE"].ToString());

                    // To Get Server DateTime
                    DateTime utcTime = DateTime.UtcNow;
                    TimeZoneInfo tzi = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");
                    DateTime todaysDate = TimeZoneInfo.ConvertTimeFromUtc(utcTime, tzi); // convert from utc to local

                    bool isPaymentEntryAllowed = false;
                    //int ims_pr_road_Code = Convert.ToInt32(roadCode);
                    //int ims_pr_road_Code = 231379;

                    bool isRoadUnfreeze = false;

                    isVTSLastDateExceed = (todaysDate > VTS_ENTRY_LASTDATE) ? true : false;

                    //using (PMGSYEntities dbContext = new PMGSYEntities())
                    {
                        int maxYear = dbContext.EXEC_ROADS_MONTHLY_STATUS.Where(r => r.IMS_PR_ROAD_CODE == ims_pr_road_Code).Any() ? dbContext.EXEC_ROADS_MONTHLY_STATUS.Where(r => r.IMS_PR_ROAD_CODE == ims_pr_road_Code).Max(x => x.EXEC_PROG_YEAR) : 0;
                        int maxMonth = maxYear != 0 ? dbContext.EXEC_ROADS_MONTHLY_STATUS.Where(r => r.IMS_PR_ROAD_CODE == ims_pr_road_Code && r.EXEC_PROG_YEAR == maxYear).Max(x => x.EXEC_PROG_MONTH) : 0;


                        //Check Selected Road Exists under PMGSY-III or not
                        bool isPmgsyIIIProgressRoad = dbContext.IMS_SANCTIONED_PROJECTS.Where(x => x.IMS_PR_ROAD_CODE == ims_pr_road_Code && x.MAST_PMGSY_SCHEME == 4 && x.IMS_PROPOSAL_TYPE == "P").Any()
                                                      ?
                                                            dbContext.EXEC_ROADS_MONTHLY_STATUS.Where(r => r.IMS_PR_ROAD_CODE == ims_pr_road_Code).Any()
                                                            ?
                                                                 dbContext.EXEC_ROADS_MONTHLY_STATUS.Where(r => r.IMS_PR_ROAD_CODE == ims_pr_road_Code && r.EXEC_PROG_YEAR == maxYear && r.EXEC_PROG_MONTH == maxMonth && r.EXEC_ISCOMPLETED == "C").Any() ? false : true
                                                            :
                                                                false
                                                      : false;
                        if (isPmgsyIIIProgressRoad)
                        {

                            if (isVTSLastDateExceed) //After VTS_ENTRY_LASTDATE , Check Condition for unfreeze Road having VTS file uploaded or not
                            {
                                // isPaymentEntryAllowed = dbContext.
                                isRoadUnfreeze = dbContext.VTS_UNFREEZE_WORKS.Where(x => x.IMS_PR_ROAD_CODE == ims_pr_road_Code && x.IS_UNFREEZED.Equals("Y")).Any() ? true : false;
                                if (isRoadUnfreeze)
                                {
                                    // As per disccusion with pankaj sir on date 30-08-2023 if road unfreezed then directly give provision to add payment

                                    //payment will be processed
                                    //isPaymentEntryAllowed = dbContext.VTS_GPS_FILES_DETAILS.Where(x => x.IMS_PR_ROAD_CODE == ims_pr_road_Code).Any()
                                    //                     ?
                                    //                         dbContext.VTS_ROADWISE_GPS_AVAILABILITY.Where(x => x.IMS_PR_ROAD_CODE == ims_pr_road_Code && x.IS_PDF_FINALIZED.Equals("Y")).Any() ? true : false
                                    //                     :
                                    //                           false;

                                    //return Json(new { status = isPaymentEntryAllowed });
                                    return Json(new { status = true });
                                }
                                else
                                {
                                    //payment will not be processed
                                    return Json(new { status = false });
                                }

                            }
                            else  //Before VTS_ENTRY_LASTDATE , Check condition for VTS file uploaded or not
                            {
                                // As per disccusion with pankaj sir on date 30-08-2023 if road unfreezed then directly give provision to add payment

                                //isPaymentEntryAllowed = dbContext.VTS_GPS_FILES_DETAILS.Where(x => x.IMS_PR_ROAD_CODE == ims_pr_road_Code).Any()
                                //                        ?
                                //                            dbContext.VTS_ROADWISE_GPS_AVAILABILITY.Where(x => x.IMS_PR_ROAD_CODE == ims_pr_road_Code && x.IS_PDF_FINALIZED.Equals("Y")).Any() ? true : false
                                //                        :
                                //                              false;

                                //return Json(new { status = isPaymentEntryAllowed });
                                return Json(new { status = true });
                            }
                        }
                        else
                        {
                            if (dbContext.IMS_SANCTIONED_PROJECTS.Where(x => x.IMS_PR_ROAD_CODE == ims_pr_road_Code && x.IMS_PROPOSAL_TYPE == "L").Any())
                            {
                                return Json(new { status = true });
                            }
                            else
                            {
                                if (dbContext.EXEC_ROADS_MONTHLY_STATUS.Where(r => r.IMS_PR_ROAD_CODE == ims_pr_road_Code).Any())
                                {

                                    return Json(new { status = true });

                                }
                                else
                                {
                                    return Json(new { status = false });

                                }
                            }



                        }



                    }
                }
                else
                {
                    return Json(new { status = true });
                }


            }
            catch (Exception ex)
            {

                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                string errorMsg = string.Format("Errors: {0}", ex.Message);
                Response.TrySkipIisCustomErrors = true;
                Response.StatusCode = 500;
                Response.Write(errorMsg);
                return Json(new { status = false });
            }
            finally
            {
                dbContext.Dispose();
            }

        }
        #endregion


        /// <summary>
        /// function to display addition form of master payment
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Audit]
        public ActionResult AddEditMasterPayment(String id)
        {

            PaymentDAL paymentDAL = new PaymentDAL();

            TransactionParams objparams = new TransactionParams();
            PaymentMasterModel model = new PaymentMasterModel();
            CommonFunctions objCommon = new CommonFunctions();

            //string strVoucherNumber = objCommon.GetPaymentReceiptNumber(905, "P", "P", 6, 2016);
            try
            {
                PMGSYEntities dbContext = null;
                dbContext = new PMGSYEntities();

                string moduleType = "R";

                //comment on : 12 oct 2021 to enable Payment for Reat Module
                //string moduleType = "D";
                //REAT_OMMAS_PAYMENT_SUCCESS objModuleType = dbContext.REAT_OMMAS_PAYMENT_SUCCESS.FirstOrDefault(s => s.ADMIN_ND_CODE == PMGSYSession.Current.AdminNdCode);
                //REAT_OMMAS_PAYMENT_SUCCESS objModuleType = dbContext.REAT_OMMAS_PAYMENT_SUCCESS.FirstOrDefault(s => s.ADMIN_ND_CODE == PMGSYSession.Current.AdminNdCode && s.FUND_TYPE == PMGSYSession.Current.FundType);
                //if (objModuleType != null)
                //{
                //    moduleType = "R";
                //}

                #region PFMS Validations
                //Code Commented on 09-12-2021
                //PaymentValidationViewModel pfmsVal = paymentDAL.ValidatePFMSPaymentDetails(moduleType);
                //model.IsAgencyMapped = pfmsVal.IsAgencyMapped;
                //model.IsSRRDABankDetailsFinalized = pfmsVal.IsSRRDABankDetailsFinalized;
                //model.IsDSCEnrollmentFinalized = pfmsVal.IsDSCEnrollmentFinalized;
                //model.IsEmailAvailable = pfmsVal.IsEmailAvailable;
                //model.IsPaymentSuccess = pfmsVal.IsPaymentSuccess;
                #endregion

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

                }
                else
                {
                    throw new Exception("Error While loading the page..");
                }

                objparams.OP_MODE = "A";
                List<SelectListItem> transactionType = common.PopulateTransactions(objparams);
                model.txn_ID1 = transactionType;

                List<SelectListItem> monthList = common.PopulateMonths(month);
                model.BILL_MONTH_List = monthList;

                List<SelectListItem> yearList = common.PopulateYears(year);
                model.BILL_YEAR_List = yearList;

                List<SelectListItem> CHQ_Book_ID = new List<SelectListItem>();
                CHQ_Book_ID.Add(new SelectListItem { Text = "--Select--", Value = "0", Selected = true });
                model.CHQ_Book_ID_List = CHQ_Book_ID;

                List<SelectListItem> ContractorList = common.PopulateContractorSupplier(objparams);
                model.mast_CON_ID_C1 = ContractorList;

                objparams.MAST_CON_SUP_FLAG = "S";

                List<SelectListItem> SupplierList = common.PopulateContractorSupplier(objparams);
                model.mast_CON_ID_S1 = SupplierList;

                List<SelectListItem> RemitanceDepartment = common.PopulateContractorSupplier(objparams);
                RemitanceDepartment = common.PopulateRemittanceDepartment();
                model.dept_ID1 = RemitanceDepartment;

                model.CHQ_EPAY = "Q";

                //Commented By Abhishek kamble to set BILL_DATE in js file

                //model.BILL_DATE = objCommon.GetDateTimeToString(DateTime.Now);
                //model.CHALAN_DATE = model.BILL_DATE;
                //model.CHQ_DATE = model.BILL_DATE;

                List<SelectListItem> chequeNumbers = new List<SelectListItem>();
                //initialy send -1 as cheque book id

                //  String[] availableCheques =  objPaymentBAL.GetAllAvailableChequesArray(-1, objparams.ADMIN_ND_CODE, PMGSYSession.Current.FundType);

                // ViewBag.availableCheques = availableCheques;

                ViewBag.LevelID = PMGSYSession.Current.LevelId;
                ViewBag.moduleType = moduleType;
                model.CURRENT_DATE = DateTime.Now.ToString("dd/MM/yyyy");

                List<SelectListItem> CONC_Account_ID = new List<SelectListItem>();
                CONC_Account_ID.Add(new SelectListItem { Text = "--Select Account--", Value = "0", Selected = true });
                model.CONC_Account_ID1 = CONC_Account_ID;


                return View(model);

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
        /// action method for displaying the master details while editing from list page through the masterdetailsPayment.js 
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="hash"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        [HttpGet]
        [Audit]
        public ActionResult EditMasterPayment(String parameter, String hash, String key)
        {
            PaymentDAL paymentDAL = new PaymentDAL();

            Int64 Bill_Id = 0;
            CommonFunctions objCommon = new CommonFunctions();

            if (!String.IsNullOrEmpty(parameter) && !String.IsNullOrEmpty(hash) && !String.IsNullOrEmpty(key))
            {
                String[] urlParams = URLEncrypt.DecryptParameters(new String[] { parameter, hash, key });
                if (urlParams.Length >= 1)
                {

                    Bill_Id = Convert.ToInt64(urlParams[0]);

                }
            }
            else
            {
                throw new Exception("Error while getting master payment details..");
            }

            //get the master details
            ACC_BILL_MASTER masterDetails = new ACC_BILL_MASTER();
            masterDetails = objPaymentBAL.GetMasterPaymentDetails(Bill_Id);

            TransactionParams objparams = new TransactionParams();
            objparams.TXN_ID = masterDetails.TXN_ID;
            PaymentMasterModel model = new PaymentMasterModel();
            try
            {
                #region PFMS Validations

                PMGSYEntities dbContext = null;
                dbContext = new PMGSYEntities();
                // bool isPFMSFinalized = true;
                string moduleType = "D";
                REAT_OMMAS_PAYMENT_SUCCESS objModuleType = dbContext.REAT_OMMAS_PAYMENT_SUCCESS.FirstOrDefault(s => s.ADMIN_ND_CODE == PMGSYSession.Current.AdminNdCode);
                if (objModuleType != null)
                {
                    moduleType = "R";

                }

                //Below Code Commented on 13-12-2021
                //PaymentValidationViewModel pfmsVal = paymentDAL.ValidatePFMSPaymentDetails(moduleType);
                //model.IsAgencyMapped = pfmsVal.IsAgencyMapped;
                //model.IsSRRDABankDetailsFinalized = pfmsVal.IsSRRDABankDetailsFinalized;
                //model.IsDSCEnrollmentFinalized = pfmsVal.IsDSCEnrollmentFinalized;
                //model.IsEmailAvailable = pfmsVal.IsEmailAvailable;
                #endregion

                objparams.BILL_TYPE = "P";
                objparams.FUND_TYPE = PMGSYSession.Current.FundType;
                objparams.LVL_ID = PMGSYSession.Current.LevelId;
                objparams.ADMIN_ND_CODE = PMGSYSession.Current.AdminNdCode;
                objparams.DISTRICT_CODE = PMGSYSession.Current.DistrictCode;

                if (masterDetails.CHQ_EPAY == "Q")
                {
                    model.CHQ_Book_ID_List = objPaymentBAL.GetchequebookSeries(PMGSYSession.Current.AdminNdCode, PMGSYSession.Current.FundType, PMGSYSession.Current.LevelId);
                    model.CHQ_Book_ID = masterDetails.CHQ_Book_ID;
                }
                else
                {

                    List<SelectListItem> CHQ_Book_ID = new List<SelectListItem>();
                    CHQ_Book_ID.Add(new SelectListItem { Text = "--Select--", Value = "0", Selected = true });
                    model.CHQ_Book_ID_List = CHQ_Book_ID;
                }

                objparams.OP_MODE = "E";
                List<SelectListItem> transactionType = common.PopulateTransactions(objparams);
                model.txn_ID1 = transactionType;


                model.CHQ_EPAY = masterDetails.CHQ_EPAY;

                List<SelectListItem> monthList = common.PopulateMonths(masterDetails.BILL_MONTH);
                model.BILL_MONTH_List = monthList;
                model.BILL_MONTH = masterDetails.BILL_MONTH;

                List<SelectListItem> yearList = common.PopulateYears(masterDetails.BILL_YEAR);
                model.BILL_YEAR = masterDetails.BILL_YEAR;
                model.BILL_YEAR_List = yearList;

                List<SelectListItem> ContractorList = common.PopulateContractorSupplier(objparams);
                model.mast_CON_ID_C1 = ContractorList;

                objparams.MAST_CON_SUP_FLAG = "S";
                List<SelectListItem> SupplierList = common.PopulateContractorSupplier(objparams);
                model.mast_CON_ID_S1 = SupplierList;

                model.CHALAN_NO = masterDetails.CHALAN_NO;

                if (masterDetails.CHALAN_DATE.HasValue)
                {
                    model.CHALAN_DATE = common.GetDateTimeToString(masterDetails.CHALAN_DATE.Value);
                }

                List<SelectListItem> conAccountDetails = new List<SelectListItem>();
                conAccountDetails.Add(new SelectListItem { Text = "---Select Account---", Value = "0" });

                MASTER_CONTRACTOR_BANK objBANK = dbContext.MASTER_CONTRACTOR_BANK.FirstOrDefault(s => s.MAST_CON_ID == masterDetails.MAST_CON_ID && s.MAST_ACCOUNT_ID == masterDetails.CON_ACCOUNT_ID);
                //if (objModuleType != null)
                //{
                //  moduleType = "R";

                // }

                // conAccountDetails = objPaymentBAL.GetContratorBankAccNoAndIFSCcode(Convert.ToInt32(masterDetails.MAST_CON_ID), PMGSYSession.Current.FundType, masterDetails.TXN_ID, ref isPFMSFinalized, true, true);

                if (PMGSYSession.Current.FundType == "A" || PMGSYSession.Current.FundType == "P" || PMGSYSession.Current.FundType == "M")
                {
                    conAccountDetails.Add(new SelectListItem { Text = "-", Value = "-", Selected = true });

                    model.CONC_Account_ID1 = conAccountDetails;
                }
                else
                {

                    conAccountDetails.Add(new SelectListItem { Text = objBANK.MAST_BANK_NAME + ":" + objBANK.MAST_IFSC_CODE + ":" + objBANK.MAST_ACCOUNT_NUMBER, Value = objBANK.MAST_ACCOUNT_ID.ToString(), Selected = true });

                    model.CONC_Account_ID1 = conAccountDetails;
                }


                List<SelectListItem> RemitanceDepartment = common.PopulateContractorSupplier(objparams);
                RemitanceDepartment = common.PopulateRemittanceDepartment();
                model.dept_ID1 = RemitanceDepartment;

                // ViewBag.availableCheques = objPaymentBAL.GetAllAvailableChequesArray(-1, objparams.ADMIN_ND_CODE, PMGSYSession.Current.FundType);

                ViewBag.operationType = "E";

                ViewBag.Bill_id = URLEncrypt.EncryptParameters(new string[] { Bill_Id.ToString() });

                ViewBag.BillFinalized = masterDetails.BILL_FINALIZED;

                ViewBag.LevelID = PMGSYSession.Current.LevelId;

                return View("AddEditMasterPayment", model);

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
        /// function to return screen design params for selected transaction type
        /// </summary>
        /// <returns></returns>
        [Audit]
        public JsonResult GetMasterDesignParams(string id)
        {
            ACC_SCREEN_DESIGN_PARAM_MASTER parametes = new ACC_SCREEN_DESIGN_PARAM_MASTER();
            TransactionParams objparams = new TransactionParams();
            PaymentDAL paymentDAL = new PaymentDAL();
            PaymentValidationViewModel pfmsVal = null;
            try
            {
                objparams.TXN_ID = Convert.ToInt16(id.Split('$')[0]);

                string bill_date = Request.Params["billDate"];//Added on 01-07-2022

                //Below conditions added on 14-06-2023
                string isChqChecked = string.IsNullOrEmpty(Request.Params["isChqChecked"]) ? string.Empty : Request.Params["isChqChecked"];//Added on 15-02-2023
                string isAdvChecked = string.IsNullOrEmpty(Request.Params["isAdvChecked"]) ? string.Empty : Request.Params["isAdvChecked"];//Added on 16-02-2023
                bool blockConSupplDropdown = (string.IsNullOrEmpty(Request.Params["isChqChecked"]) && string.IsNullOrEmpty(Request.Params["isAdvChecked"]))
                    ?
                       false
                    : (isChqChecked == "Y" || isAdvChecked == "Y") && (objparams.TXN_ID == 472 || objparams.TXN_ID == 390) && PMGSYSession.Current.FundType == "A" ? true : false;


                parametes = common.getMasterDesignParam(objparams);
                //string strVoucherNumber = objCommon.GetPaymentReceiptNumber(905, "P", "P", 6, 2016);

                #region PFMS Validations

                /* Below Code added on 12-10-2021 and previous code commented , to enable only REAT module payments */
                string moduleType = "R";
                pfmsVal = new PaymentValidationViewModel();

                //if (PMGSYSession.Current.ParentNDCode == 1 || PMGSYSession.Current.ParentNDCode == 1058 || PMGSYSession.Current.ParentNDCode == 1068 || PMGSYSession.Current.ParentNDCode == 1069 || PMGSYSession.Current.ParentNDCode == 1071)
                //{
                //    pfmsVal.IsAgencyMapped = true;
                //    pfmsVal.IsSRRDABankDetailsFinalized = true;
                //    pfmsVal.IsDSCEnrollmentFinalized = true;
                //    pfmsVal.IsEmailAvailable = true;
                //    pfmsVal.IsPaymentSuccess = true;
                //}

                //Below Line Commented on 09-12-2021
                //pfmsVal = paymentDAL.ValidatePFMSPaymentDetails(moduleType);

                //Below Line Added on 09-12-2021
                pfmsVal = paymentDAL.ValidatePFMSPaymentDetails(moduleType, objparams.TXN_ID);

                //if (id.Split('$').Count() > 2 && (Convert.ToInt32(id.Split('$')[2]) + (Convert.ToInt32(id.Split('$')[3]) * 12) >= (8 + (2018 * 12))))
                //{
                //    PMGSYEntities dbContext = null;
                //    dbContext = new PMGSYEntities();

                //    string moduleType = "D";
                //    REAT_OMMAS_PAYMENT_SUCCESS objModuleType = dbContext.REAT_OMMAS_PAYMENT_SUCCESS.FirstOrDefault(s => s.ADMIN_ND_CODE == PMGSYSession.Current.AdminNdCode);
                //    if (objModuleType != null)
                //    {
                //        moduleType = "R";
                //    }

                //    pfmsVal = paymentDAL.ValidatePFMSPaymentDetails(moduleType);

                //}
                //else
                //{
                //    pfmsVal = new PaymentValidationViewModel();
                //    if (PMGSYSession.Current.ParentNDCode == 1 || PMGSYSession.Current.ParentNDCode == 1058 || PMGSYSession.Current.ParentNDCode == 1068 || PMGSYSession.Current.ParentNDCode == 1069 || PMGSYSession.Current.ParentNDCode == 1071)
                //    {
                //        pfmsVal.IsAgencyMapped = true;
                //        pfmsVal.IsSRRDABankDetailsFinalized = true;
                //        pfmsVal.IsDSCEnrollmentFinalized = true;
                //        pfmsVal.IsEmailAvailable = true;
                //        pfmsVal.IsPaymentSuccess = true;
                //    }
                //    else
                //    {
                //        PMGSYEntities dbContext = null;
                //        dbContext = new PMGSYEntities();

                //        string moduleType = "D";
                //        REAT_OMMAS_PAYMENT_SUCCESS objModuleType = dbContext.REAT_OMMAS_PAYMENT_SUCCESS.FirstOrDefault(s => s.ADMIN_ND_CODE == PMGSYSession.Current.AdminNdCode);
                //        if (objModuleType != null)
                //        {
                //            moduleType = "R";
                //        }

                //        pfmsVal = paymentDAL.ValidatePFMSPaymentDetails(moduleType);

                //    }
                //}
                #endregion

                #region Security Deposit and Holding Account Bank Validation
                PMGSYEntities dbContext = new PMGSYEntities();
                if (objparams.TXN_ID == 3185 || objparams.TXN_ID == 3187 || objparams.TXN_ID == 3173)
                {
                    int isHoldBankValid = dbContext.ACC_BANK_DETAILS.Where(x => x.ADMIN_ND_CODE == PMGSYSession.Current.ParentNDCode && x.FUND_TYPE == PMGSYSession.Current.FundType && x.MAST_STATE_CODE == PMGSYSession.Current.StateCode && x.ACCOUNT_TYPE == "H" && x.BANK_ACC_STATUS == true).Any()
                                          ?
                                              dbContext.ACC_BANK_DETAILS.Where(x => x.ADMIN_ND_CODE == PMGSYSession.Current.ParentNDCode && x.FUND_TYPE == PMGSYSession.Current.FundType && x.MAST_STATE_CODE == PMGSYSession.Current.StateCode && x.ACCOUNT_TYPE == "H" && x.BANK_ACC_STATUS == true && x.BANK_ACC_NO != null).Any()
                                               ?
                                                        dbContext.ACC_BANK_DETAILS.Where(x => x.ADMIN_ND_CODE == PMGSYSession.Current.ParentNDCode && x.FUND_TYPE == PMGSYSession.Current.FundType && x.MAST_STATE_CODE == PMGSYSession.Current.StateCode && x.ACCOUNT_TYPE == "H" && x.BANK_ACC_STATUS == true && x.MAST_IFSC_CODE != null).Any()
                                                    ?
                                                        1
                                                    :
                                                        -3//("Holding Bank IFSC Code is not available.")
                                               :
                                                    -2//("Holding Bank Account Number is not available.")
                                          : -1;//("Holding Bank Account details is not available.")


                    int isSecDepBankValid = dbContext.ACC_BANK_DETAILS.Where(x => x.ADMIN_ND_CODE == PMGSYSession.Current.ParentNDCode && x.FUND_TYPE == PMGSYSession.Current.FundType && x.MAST_STATE_CODE == PMGSYSession.Current.StateCode && x.ACCOUNT_TYPE == "D" && x.BANK_ACC_STATUS == true).Any()
                                          ?
                                              dbContext.ACC_BANK_DETAILS.Where(x => x.ADMIN_ND_CODE == PMGSYSession.Current.ParentNDCode && x.FUND_TYPE == PMGSYSession.Current.FundType && x.MAST_STATE_CODE == PMGSYSession.Current.StateCode && x.ACCOUNT_TYPE == "D" && x.BANK_ACC_STATUS == true && x.BANK_ACC_NO != null).Any()
                                               ?
                                                        dbContext.ACC_BANK_DETAILS.Where(x => x.ADMIN_ND_CODE == PMGSYSession.Current.ParentNDCode && x.FUND_TYPE == PMGSYSession.Current.FundType && x.MAST_STATE_CODE == PMGSYSession.Current.StateCode && x.ACCOUNT_TYPE == "D" && x.BANK_ACC_STATUS == true && x.MAST_IFSC_CODE != null).Any()
                                                    ?
                                                        1
                                                    :
                                                        -5//("Security Deposit Bank IFSC Code is not available.")
                                               :
                                                    -6//("Security Deposit Bank Account Number is not available.")
                                          : -4;//("Security Deposit Bank Account details is not available.")

                    if (isHoldBankValid < 0 && objparams.TXN_ID == 3173)
                    {
                        switch (isHoldBankValid)
                        {
                            case -1:
                                return Json(-1);
                            //break;
                            case -2:
                                return Json(-2);
                            //break;
                            case -3:
                                return Json(-3);
                            //break;
                            default:
                                break;
                        }
                    }
                    else if (isSecDepBankValid < 0)
                    {
                        switch (isSecDepBankValid)
                        {
                            case -4:
                                return Json(-4);
                            // break;
                            case -5:
                                return Json(-5);
                            // break;
                            case -6:
                                return Json(-6);
                            // break;
                            default:
                                break;
                        }
                    }
                }

                #endregion

                #region Region To Read ByPassStateChkTrans xml file

                XDocument doc_xml = XDocument.Load(System.Web.HttpContext.Current.Server.MapPath("~/ByPassStateChkTrans.xml"));
                List<int> StateCode = new List<int>();
                foreach (XElement element in doc_xml.Descendants("stateList").Descendants("statCode"))
                {
                    StateCode.Add(Convert.ToInt32(element.Value));
                }
                //xml Code end

                string ChqBackDateUpto = ConfigurationManager.AppSettings["CHQ_PAYMENT_BACK_DATE_UPTO"].ToString();
                string chqAllowUpto = ConfigurationManager.AppSettings["CHQ_PAYMENT_ALLOW_UPTO"].ToString();
                #endregion

                #region Region To read xml for By pass deduction screen
                //XDocument xml_Ded_Doc = XDocument.Load(System.Web.HttpContext.Current.Server.MapPath("~/ByPassDeduction.xml"));
                //string bypass_ded = "N";
                //foreach (XElement element in xml_Ded_Doc.Descendants("stateList").Descendants("ByPassDed").Descendants("statCode"))
                //{
                //    if (Convert.ToInt32(element.Value) == PMGSYSession.Current.StateCode)
                //    {
                //        bypass_ded = "Y";
                //        break;
                //    }
                //}
                #endregion

                //Added condition to allow Backdate Cheque Payment for state

                //if (parametes != null && PMGSYSession.Current.FundType == "A" && new CommonFunctions().GetStringToDateTime(bill_date) < new CommonFunctions().GetStringToDateTime(ChqBackDateUpto) && DateTime.Now.Date < new CommonFunctions().GetStringToDateTime(chqAllowUpto) && StateCode.Contains(PMGSYSession.Current.StateCode))
                if (
                    (!(bill_date == null || bill_date == "")) &&
                        (
                            (parametes != null && PMGSYSession.Current.FundType == "A" && new CommonFunctions().GetStringToDateTime(bill_date) <= new CommonFunctions().GetStringToDateTime(ChqBackDateUpto) && DateTime.Now.Date < new CommonFunctions().GetStringToDateTime(chqAllowUpto) && StateCode.Contains(PMGSYSession.Current.StateCode))
                         )
                     ||
                         (parametes != null && PMGSYSession.Current.FundType == "P" && DateTime.Now.Date <= new CommonFunctions().GetStringToDateTime("22/10/2022") && PMGSYSession.Current.StateCode == 5 && PMGSYSession.Current.UserId == 1315)
                     )

                {
                    return Json(new
                    {

                        BankAuthRequired = parametes.BAR_REQ.Trim(),
                        CashOrCheque = parametes.CASH_CHQ.Trim(),
                        DedRequired = parametes.DED_REQ.Trim(),
                        //EpayRequired = parametes.EPAY_REQ.Trim(),
                        EpayRequired = (PMGSYSession.Current.UserId == 1315 && PMGSYSession.Current.FundType == "P") ? "N" : parametes.EPAY_REQ.Trim(),//Modified on 21-09-2022
                        MultipleTransRequired = parametes.MTXN_REQ.Trim(),
                        RemittanceRequired = "N",
                        //ContractorRequired = "N",
                        ContractorRequired = PMGSYSession.Current.FundType == "P" ? parametes.MAST_CON_REQ : "N",
                        //AgreementRequired = "N",
                        AgreementRequired = PMGSYSession.Current.FundType == "P" ? parametes.MAST_AGREEMENT_REQ : "N",
                        //SupplierRequired = "N",
                        SupplierRequired = PMGSYSession.Current.FundType == "P" ? parametes.MAST_SUPPLIER_REQ : "N",

                        AdviceNoRequired = (PMGSYSession.Current.LevelId == 5 ? parametes.ADVANCES_REQ : "N"),


                        IsAgencyMapped = pfmsVal.IsAgencyMapped,
                        IsSRRDABankDetailsFinalized = pfmsVal.IsSRRDABankDetailsFinalized,
                        IsDSCEnrollmentFinalized = pfmsVal.IsDSCEnrollmentFinalized,
                        IsEmailAvailable = pfmsVal.IsEmailAvailable,
                        IsPaymentSuccess = pfmsVal.IsPaymentSuccess
                        //IsDedBypass = "N" // Added on 28-01-2023 to bypass deduction based on state
                    });
                }
                else if (parametes != null)
                {
                    return Json(new
                    {

                        BankAuthRequired = parametes.BAR_REQ.Trim(),
                        CashOrCheque = parametes.CASH_CHQ.Trim(),
                        DedRequired = parametes.DED_REQ.Trim(),
                        EpayRequired = parametes.EPAY_REQ.Trim(),
                        MultipleTransRequired = parametes.MTXN_REQ.Trim(),
                        RemittanceRequired = parametes.REM_REQ.Trim(),
                        //ContractorRequired = parametes.MAST_CON_REQ,//commented on 14-06-2023
                        ContractorRequired = blockConSupplDropdown ? "N" : parametes.MAST_CON_REQ,//Added on 14-06-2023
                        AgreementRequired = parametes.MAST_AGREEMENT_REQ,
                        SupplierRequired = parametes.MAST_SUPPLIER_REQ,
                        AdviceNoRequired = (PMGSYSession.Current.LevelId == 5 ? parametes.ADVANCES_REQ : "N"),


                        IsAgencyMapped = pfmsVal.IsAgencyMapped,
                        IsSRRDABankDetailsFinalized = pfmsVal.IsSRRDABankDetailsFinalized,
                        IsDSCEnrollmentFinalized = pfmsVal.IsDSCEnrollmentFinalized,
                        IsEmailAvailable = pfmsVal.IsEmailAvailable,
                        IsPaymentSuccess = pfmsVal.IsPaymentSuccess
                        //IsDedBypass = bypass_ded // Added on 28-01-2023 to bypass deduction based on state
                    });
                }
                else
                {
                    return Json(string.Empty);
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


        [Audit]
        public JsonResult GenerateVoucherNo(string id)
        {
            try
            {

                string strVoucherNumber = common.GetPaymentReceiptNumber(PMGSYSession.Current.AdminNdCode, PMGSYSession.Current.FundType, id.Split('$')[0].ToString().Trim(), Convert.ToInt16(id.Split('$')[1]), Convert.ToInt16(id.Split('$')[2]));


                if (strVoucherNumber != string.Empty)
                {
                    return Json(new
                    {
                        strVoucherNumber = strVoucherNumber.Split('$')[0].Trim(),
                        strVoucherCnt = strVoucherNumber.Split('$')[1].Trim(),
                    });
                }
                else
                {
                    return Json(string.Empty);
                }
            }
            catch (Exception ex)
            {

                return Json(string.Empty);
            }


        }

        /// <summary>
        /// function to get the details design params
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Audit]
        public JsonResult getDetailsDesignParam(string id)
        {
            ACC_SCREEN_DESIGN_PARAM_DETAILS_CUSTOM parametes = new ACC_SCREEN_DESIGN_PARAM_DETAILS_CUSTOM();
            TransactionParams objparams = new TransactionParams();
            long billId = 0;
            bool isSupplier = false;
            try
            {
                objparams.TXN_ID = Convert.ToInt16(id.Split('$')[0]);
                if (!(string.IsNullOrEmpty(Request.Params["billId"])) && (objparams.TXN_ID == 1489 || objparams.TXN_ID == 1490))
                {
                    ///Changes for Supplier Payment Building Proposal
                    String[] urlParams = URLEncrypt.DecryptParameters(new String[] { Request.Params["billId"].Split('/')[0], Request.Params["billId"].Split('/')[1], Request.Params["billId"].Split('/')[2] });

                    billId = Convert.ToInt64(urlParams[0]);
                    isSupplier = common.getContractorSupplierbyBillId(billId);
                }
                parametes = common.getDetailsDesignParamForPayment(objparams);

                if (parametes != null)
                {
                    return Json(new
                    {

                        SupplierRequired = parametes.SUPPLIER_REQ.Trim(),
                        RoadRequired = (billId > 0 && isSupplier == true && (objparams.TXN_ID == 1489 || objparams.TXN_ID == 1490)) ? "N" : parametes.ROAD_REQ.Trim(),
                        PiuRequired = parametes.PIU_REQ.Trim(),
                        ContractorRequired = parametes.CON_REQ.Trim(),
                        AgreementRequired = parametes.AGREEMENT_REQ.Trim(),
                        YearRequired = parametes.YEAR_REQ.Trim(),
                        PackageRequired = parametes.PKG_REQ.Trim(),
                        MasterTxnId = parametes.MASTER_TXN_ID,
                        ShowContractor = parametes.SHOW_CON_AT_TRANSACTION
                    });
                }
                else
                {
                    return Json(string.Empty);
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
        /// function to calculate the closing balance for the payment
        /// </summary>
        /// <returns></returns>
        [Audit]
        public JsonResult GetClosingBalanceForPayment(String id)
        {
            TransactionParams objparams = new TransactionParams();

            objparams.ADMIN_ND_CODE = PMGSYSession.Current.AdminNdCode;

            objparams.FUND_TYPE = PMGSYSession.Current.FundType;

            objparams.MONTH = Convert.ToInt16(id.Split('$')[0]);

            objparams.YEAR = Convert.ToInt16(id.Split('$')[1]);

            objparams.LVL_ID = PMGSYSession.Current.LevelId;



            //UDF_ACC_GEN_GET_BA_CASH_CLOSING_BALANCES_Result balance = new UDF_ACC_GEN_GET_BA_CASH_CLOSING_BALANCES_Result();

            //balance = objPaymentBAL.GetCloSingBalanceForPayment(objparams);

            UDF_ACC_GEN_GET_BA_CASH_CLOSING_BALANCES_COMPOSITE_Result balanceComposite = new UDF_ACC_GEN_GET_BA_CASH_CLOSING_BALANCES_COMPOSITE_Result();

            balanceComposite = objPaymentBAL.GetCloSingBalanceForPayment(objparams);

            return Json(new
            {
                Cash = balanceComposite.cash,
                BankAuth = balanceComposite.bank_auth,
                BankAuthHold = balanceComposite.Bank_Auth_Holding,
                BankAuthSec = balanceComposite.Bank_Auth_Security

            });

        }

        /// <summary>
        /// function to populate master details in grid on landing page
        /// </summary>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <param name="sidx"></param>
        /// <param name="sord"></param>
        /// <returns></returns>
        [Audit]
        public JsonResult GetMasterPaymentListJson(int? page, int? rows, string sidx, string sord, String parameter, String hash, String key)
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

                if (!String.IsNullOrEmpty(parameter) && !String.IsNullOrEmpty(hash) && !String.IsNullOrEmpty(key))
                {
                    Int64 master_Bill_Id = 0;

                    if (!String.IsNullOrEmpty(parameter) && !String.IsNullOrEmpty(hash) && !String.IsNullOrEmpty(key))
                    {
                        String[] urlParams = URLEncrypt.DecryptParameters(new String[] { parameter, hash, key });
                        if (urlParams.Length >= 1)
                        {
                            String[] urlSplitParams = urlParams[0].Split('$');
                            master_Bill_Id = Convert.ToInt64(urlSplitParams[0]);
                            objFilter.BillId = master_Bill_Id;
                        }


                    }
                    else
                    {
                        throw new Exception("Error While deleteing master payment details..");
                    }
                }


                long totalRecords;
                objFilter.Month = Convert.ToInt16(Request.Params["months"]);
                objFilter.Year = Convert.ToInt16(Request.Params["year"]);

                if (objFilter.FilterMode.Equals("Search"))
                {
                    objFilter.FromDate = Request.Params["fromDate"].ToString() == String.Empty ? null : Request.Params["fromDate"].ToString();
                    objFilter.ToDate = Request.Params["toDate"].ToString() == String.Empty ? null : Request.Params["toDate"].ToString();
                    objFilter.TransId = Request.Params["transType"] == String.Empty ? Convert.ToInt16(0) : Convert.ToInt16(Request.Params["transType"].Trim().Split('$')[0]);
                    objFilter.ChequeEpayNumber = Request.Params["Chq_Epay"].ToString() == String.Empty ? null : Request.Params["Chq_Epay"].ToString();
                }
                objFilter.page = Convert.ToInt32(page) - 1;
                objFilter.rows = Convert.ToInt32(rows);
                objFilter.sidx = sidx.ToString();
                objFilter.sord = sord.ToString();

                objFilter.Bill_type = "P";

                objFilter.AdminNdCode = PMGSYSession.Current.AdminNdCode;
                objFilter.FundType = PMGSYSession.Current.FundType;
                objFilter.LevelId = PMGSYSession.Current.LevelId;

                /*  
        objFilter.AdminNdCode = 11;
        objFilter.FundType = PMGSYSession.Current.FundType;
        objFilter.LevelId = 5; */

                var jsonData = new
                {
                    rows = objPaymentBAL.ListMasterPaymentDetails(objFilter, out totalRecords),
                    total = totalRecords <= objFilter.rows ? 1 : totalRecords / objFilter.rows + 1,
                    page = page,
                    records = totalRecords
                };
                return Json(jsonData);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "PaymentController.GetMasterPaymentListJson()");
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                string errorMsg = string.Format("Errors: {0}", ex.Message);
                Response.TrySkipIisCustomErrors = true;
                Response.StatusCode = 500;
                Response.Write(errorMsg);
                return Json(string.Empty);
            }

        }

        /// <summary>
        /// function called when master payment gree is to be populated on dataentry page
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
        public JsonResult ListMasterPaymentDetailsForDataEntry(int? page, int? rows, string sidx, string sord, String parameter, String hash, String key, string id)
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

                if (!String.IsNullOrEmpty(parameter) && !String.IsNullOrEmpty(hash) && !String.IsNullOrEmpty(key))
                {


                    if (!String.IsNullOrEmpty(parameter) && !String.IsNullOrEmpty(hash) && !String.IsNullOrEmpty(key))
                    {
                        String[] urlParams = URLEncrypt.DecryptParameters(new String[] { parameter, hash, key });
                        if (urlParams.Length >= 1)
                        {

                            objFilter.BillId = Convert.ToInt64(urlParams[0]);

                        }
                    }
                    else
                    {
                        throw new Exception("Error While getting master payment details..");
                    }
                }

                long totalRecords;
                long _MasterTxnID;
                objFilter.Month = Convert.ToInt16(Request.Params["months"]);
                objFilter.Year = Convert.ToInt16(Request.Params["year"]);
                objFilter.page = Convert.ToInt32(page) - 1;
                objFilter.rows = Convert.ToInt32(rows);
                objFilter.sidx = sidx.ToString();
                objFilter.sord = sord.ToString();

                objFilter.Bill_type = "P";

                objFilter.AdminNdCode = PMGSYSession.Current.AdminNdCode;
                objFilter.FundType = PMGSYSession.Current.FundType;
                objFilter.LevelId = PMGSYSession.Current.LevelId;

                /*
                objFilter.AdminNdCode = 11;
                objFilter.FundType = PMGSYSession.Current.FundType;
                objFilter.LevelId = 5; */

                var jsonData = new
                {
                    rows = objPaymentBAL.ListMasterPaymentDetailsForDataEntry(objFilter, out totalRecords, out _MasterTxnID),
                    total = totalRecords <= objFilter.rows ? 1 : totalRecords / objFilter.rows + 1,
                    page = page,
                    records = totalRecords,
                    userdata = new { TxnId = _MasterTxnID }
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
        /// function to get the agreement details
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Audit]
        public JsonResult PopulateAgreement(string id)
        {

            try
            {
                TransactionParams objparams = new TransactionParams();
                objparams.MAST_CONT_ID = Convert.ToInt16(id.Trim());
                return Json(common.PopulateAgreement(objparams));
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
        /// function to populate contractor
        /// </summary>
        /// <returns></returns>
        [Audit]
        public JsonResult GetContractor()
        {

            try
            {
                TransactionParams objparams = new TransactionParams();
                objparams.STATE_CODE = PMGSYSession.Current.StateCode;
                objparams.DISTRICT_CODE = PMGSYSession.Current.DistrictCode;
                objparams.MAST_CON_SUP_FLAG = "C";
                return Json(common.PopulateContractorSupplier(objparams));
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
        /// function to get the aggreements of the contractor
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Audit]
        public JsonResult GetAgreementForContractor(String id)
        {

            try
            {
                int contractorId = 0;

                contractorId = Convert.ToInt32(id);

                TransactionParams objparams = new TransactionParams();

                objparams.BILL_TYPE = "P";
                objparams.FUND_TYPE = PMGSYSession.Current.FundType;
                objparams.LVL_ID = PMGSYSession.Current.LevelId;
                objparams.ADMIN_ND_CODE = PMGSYSession.Current.AdminNdCode;
                objparams.MAST_CONT_ID = contractorId;
                objparams.DISTRICT_CODE = PMGSYSession.Current.DistrictCode;

                return Json(common.PopulateAgreement(objparams));

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
        /// function to get the contractor or supplier name
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Audit]
        public String GetContractorSupplierName(string id)
        {
            string name = string.Empty;


            try
            {

                TransactionParams objparams = new TransactionParams();
                int CintractorId = Convert.ToInt32(id.Split('$')[0]);

                objparams.TXN_ID = Convert.ToInt16(id.Split('$')[1]);
                if (!String.IsNullOrEmpty(id))
                {
                    //CintractorId = Convert.ToInt32(id.Trim());
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
                    name = common.GetContractorSupplierName(objparams);
                    return name;
                }

            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                string errorMsg = string.Format("Errors: {0}", ex.Message);
                Response.TrySkipIisCustomErrors = true;
                Response.StatusCode = 500;
                Response.Write(errorMsg);
                return string.Empty;
            }


        }

        /// <summary>
        /// Get Contrator Bank Account Number IFSC Code
        /// </summary>
        /// <returns></returns>
        //public JsonResult GetContratorBankNameAccNoAndIFSCcode(string id)
        //{
        //    bool isAdvicePayment = false;
        //    bool isChqPaymentAllowed = false;
        //    CommonFunctions comm = new CommonFunctions();
        //    string PfmsDate = "01/08/2018";
        //    try
        //    {
        //        ///PFMS Validations
        //        bool isPFMSFinalized = false;

        //        int ContractorId = 0;
        //        objPaymentBAL = new PaymentBAL();

        //        String[] urlParams = id.Split('$');
        //        id = urlParams[0].ToString();

        //        if (!String.IsNullOrEmpty(id))
        //        {
        //            ContractorId = Convert.ToInt32(id.Trim());
        //        }
        //        else
        //        {
        //            return null;
        //        }
        //        //MF Advice Payment
        //        if (urlParams.Count() == 5)
        //        {
        //            if (!string.IsNullOrEmpty(urlParams[4]))
        //            {
        //                isAdvicePayment = urlParams[4] == "true" ? true : false;
        //            }
        //        }
        //        //Chq Payment before 01AUG2019
        //        if (urlParams.Count() == 7)
        //        {
        //            if (!string.IsNullOrEmpty(urlParams[5]) && !string.IsNullOrEmpty(urlParams[6]) && PMGSYSession.Current.FundType == "P")
        //            {
        //                if (comm.GetStringToDateTime(urlParams[6].Trim().Replace('-', '/')) < comm.GetStringToDateTime(PfmsDate.Trim()) && urlParams[5].Trim() == "true")
        //                {
        //                    isChqPaymentAllowed = true;
        //                }
        //                else
        //                {
        //                    isChqPaymentAllowed = false;
        //                }
        //            }
        //        }

        //       // MASTER_CONTRACTOR_BANK contractorBankDetails = objPaymentBAL.GetContratorBankAccNoAndIFSCcode(ContractorId, urlParams[1].ToString().Trim(), Convert.ToInt32(urlParams[2]), ref isPFMSFinalized, isAdvicePayment, isChqPaymentAllowed);

        //        //if (PMGSYSession.Current.FundType == "P")
        //        //{
        //        //    if (!isPFMSFinalized)
        //        //    {
        //        //        return Json(new { Success = false, message = "Contractor not finalized by PFMS" }, JsonRequestBehavior.AllowGet);
        //        //    }
        //        //}

        //        if (contractorBankDetails != null)
        //        {
        //            string BankAccNumber = string.Empty;
        //            string BankIFSCCode = string.Empty;
        //            string BankName = string.Empty;
        //            int accountId = 0;
        //            //if (contractorBankDetails.MAST_ACCOUNT_NUMBER.Equals(string.Empty) || contractorBankDetails.MAST_ACCOUNT_NUMBER.Equals("0"))
        //            if (string.IsNullOrEmpty(contractorBankDetails.MAST_ACCOUNT_NUMBER))
        //            {
        //                return Json(new { Success = false }, JsonRequestBehavior.AllowGet);
        //            }
        //            else
        //            {
        //                BankAccNumber = contractorBankDetails.MAST_ACCOUNT_NUMBER;
        //                BankIFSCCode = contractorBankDetails.MAST_IFSC_CODE;
        //                BankName = contractorBankDetails.MAST_BANK_NAME;
        //                accountId = contractorBankDetails.MAST_ACCOUNT_ID;
        //                return Json(new { Success = true, BankAccNumber = BankAccNumber, BankIFSCCode = BankIFSCCode, BankName = BankName, BankAccountId = accountId }, JsonRequestBehavior.AllowGet);
        //            }

        //        }
        //        else
        //        {
        //            return Json(new { Success = false }, JsonRequestBehavior.AllowGet);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        //Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
        //        ErrorLog.LogError(ex, "Payment.GetContratorBankNameAccNoAndIFSCcode()");
        //        return null;
        //    }

        //}


        public JsonResult GetContratorBankNameAccNoAndIFSCcode(string id)
        {
            bool isAdvicePayment = false;
            bool isChqPaymentAllowed = false;
            CommonFunctions comm = new CommonFunctions();
            string PfmsDate = "01/08/2018";
            try
            {
                ///PFMS Validations
                bool isPFMSFinalized = false;

                int ContractorId = 0;
                objPaymentBAL = new PaymentBAL();

                String[] urlParams = id.Split('$');
                id = urlParams[0].ToString();

                if (!String.IsNullOrEmpty(id))
                {
                    ContractorId = Convert.ToInt32(id.Trim());
                }
                else
                {
                    return null;
                }
                //MF Advice Payment
                if (urlParams.Count() == 5)
                {
                    if (!string.IsNullOrEmpty(urlParams[4]))
                    {
                        isAdvicePayment = urlParams[4] == "true" ? true : false;
                    }
                }
                //Chq Payment before 01AUG2019
                if (urlParams.Count() == 7)
                {
                    //if ((!string.IsNullOrEmpty(urlParams[5]) && !string.IsNullOrEmpty(urlParams[6]) && PMGSYSession.Current.FundType == "P") || (PMGSYSession.Current.FundType == "M"))  // --- Punjab Change
                    if (!string.IsNullOrEmpty(urlParams[5]) && !string.IsNullOrEmpty(urlParams[6]) && PMGSYSession.Current.FundType == "P")
                    {
                        if (comm.GetStringToDateTime(urlParams[6].Trim().Replace('-', '/')) < comm.GetStringToDateTime(PfmsDate.Trim()) && urlParams[5].Trim() == "true")
                        {
                            isChqPaymentAllowed = true;
                        }
                        else
                        {
                            isChqPaymentAllowed = false;
                        }
                    }
                }

                if (urlParams.Count() == 6)
                {

                    if (!string.IsNullOrEmpty(urlParams[4]))
                    {
                        isAdvicePayment = urlParams[4] == "true" ? true : false;
                    }

                    if (!string.IsNullOrEmpty(urlParams[5]))
                    {
                        isChqPaymentAllowed = urlParams[5] == "true" ? true : false;
                    }
                }

                // MASTER_CONTRACTOR_BANK contractorBankDetails = objPaymentBAL.GetContratorBankAccNoAndIFSCcode(ContractorId, urlParams[1].ToString().Trim(), Convert.ToInt32(urlParams[2]), ref isPFMSFinalized, isAdvicePayment, isChqPaymentAllowed);

                //if (PMGSYSession.Current.FundType == "P")
                //{
                //    if (!isPFMSFinalized)
                //    {
                //        return Json(new { Success = false, message = "Contractor not finalized by PFMS" }, JsonRequestBehavior.AllowGet);
                //    }
                //}
                return Json(objPaymentBAL.GetContratorBankAccNoAndIFSCcode(ContractorId, urlParams[1].ToString().Trim(), Convert.ToInt32(urlParams[2]), ref isPFMSFinalized, isAdvicePayment, isChqPaymentAllowed));

            }
            catch (Exception ex)
            {
                //Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                ErrorLog.LogError(ex, "Payment.GetContratorBankNameAccNoAndIFSCcode()");
                return null;
            }

        }

        /// <summary>
        /// action to get the remittance department 
        /// </summary>
        [Audit]
        public string GetRemDepartmentName(string id)
        {

            int dept_Id = 0;
            try
            {
                if (!String.IsNullOrEmpty(id))
                {
                    dept_Id = Convert.ToInt32(id.Trim());
                    return common.GetRemDepartmentName(dept_Id);
                }
                else
                {
                    throw new Exception("Error while getting the remittance department name...");
                }
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                string errorMsg = string.Format("Errors: {0}", ex.Message);
                Response.TrySkipIisCustomErrors = true;
                Response.StatusCode = 500;
                Response.Write(errorMsg);
                return string.Empty;

            }


        }

        /// <summary>
        /// function to delete the master payment details
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="hash"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        [Audit]
        public JsonResult DeletetMasterPaymentDetails(String parameter, String hash, String key)
        {
            Int64 master_Bill_Id = 0;
            int result = 0;
            if (!String.IsNullOrEmpty(parameter) && !String.IsNullOrEmpty(hash) && !String.IsNullOrEmpty(key))
            {
                String[] urlParams = URLEncrypt.DecryptParameters(new String[] { parameter, hash, key });
                if (urlParams.Length >= 1)
                {

                    master_Bill_Id = Convert.ToInt64(urlParams[0]);

                }

                result = objPaymentBAL.DeleteMasterPaymentDetails(master_Bill_Id);

                return Json(new { result = result });

            }
            else
            {
                throw new Exception("Error While deleteing master payment details..");
            }

        }

        /// <summary>
        /// action to add master details
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Audit]
        [ValidateAntiForgeryToken]
        public ActionResult AddPaymentMasterDetails(PaymentMasterModel model)
        {
            DateTime pfmsStartDt = new DateTime(2018, 08, 02);
            PaymentDAL payDAL = new PaymentDAL();
            try
            {
                ///PFMS Validations 
                //if (PMGSYSession.Current.FundType == "P" && model.CHQ_EPAY != "E" && (Convert.ToInt32(model.TXN_ID.Split('$')[0]) == 47 || Convert.ToInt32(model.TXN_ID.Split('$')[0]) == 72 || Convert.ToInt32(model.TXN_ID.Split('$')[0]) == 86
                //    || Convert.ToInt32(model.TXN_ID.Split('$')[0]) == 1484 || Convert.ToInt32(model.TXN_ID.Split('$')[0]) == 1788))
                //{
                //    return Json(new { message = "Payment mode should be Epayment for selected transaction type" }, JsonRequestBehavior.AllowGet);
                //}

                //added by Avinash
                #region
                if (PMGSYSession.Current.FundType == "P" && (model.CHQ_EPAY != "E" && model.CHQ_EPAY != "C" && model.CHQ_EPAY != "Q") && (Convert.ToInt32(model.TXN_ID.Split('$')[0]) == 47 || Convert.ToInt32(model.TXN_ID.Split('$')[0]) == 72 || Convert.ToInt32(model.TXN_ID.Split('$')[0]) == 86 || Convert.ToInt32(model.TXN_ID.Split('$')[0]) == 1484 || Convert.ToInt32(model.TXN_ID.Split('$')[0]) == 1788 || Convert.ToInt32(model.TXN_ID.Split('$')[0]) == 1974))//PMGSY3
                {

                    return Json(new { message = "Payment mode should be Epayment for selected transaction type" }, JsonRequestBehavior.AllowGet);
                }

                //added by Avinash
                int month = Convert.ToInt16(model.BILL_MONTH);
                int voucherDateMonth = Convert.ToInt16(model.BILL_DATE.Split('/')[1]);
                if (voucherDateMonth != month)
                {
                    return Json(new { message = "Voucher Date must be within selected month and year" }, JsonRequestBehavior.AllowGet);
                }

                //added by Avinash
                if (PMGSYSession.Current.FundType == "P" && (new CommonFunctions().GetStringToDateTime(model.BILL_DATE) >= pfmsStartDt) && (model.CHQ_EPAY == "Q" && model.CHQ_AMOUNT > 0) && (Convert.ToInt32(model.TXN_ID.Split('$')[0]) == 47 || Convert.ToInt32(model.TXN_ID.Split('$')[0]) == 1484 || Convert.ToInt32(model.TXN_ID.Split('$')[0]) == 1788 || Convert.ToInt32(model.TXN_ID.Split('$')[0]) == 1974 || Convert.ToInt32(model.TXN_ID.Split('$')[0]) == 3117))//PMGSY3
                {
                    //return Json(new { message = "Payment mode should be Epayment for selected transaction type Or Cheque Amount should be 0" }, JsonRequestBehavior.AllowGet);

                    //Relaxation for cheque payment to bihar state 
                    if (!(PMGSYSession.Current.StateCode == 5 && DateTime.Now.Date <= new CommonFunctions().GetStringToDateTime("17/10/2022") && PMGSYSession.Current.UserId == 1315))
                    {
                        return Json(new { message = "Payment mode should be Epayment for selected transaction type Or Cheque Amount should be 0" }, JsonRequestBehavior.AllowGet);

                    }
                }

                //Disable check payment for Fund Type='A' , Code Added on 20-12-2021 Uncomment when asked  
                //if (PMGSYSession.Current.FundType == "A" && (new CommonFunctions().GetStringToDateTime(model.BILL_DATE) >= new CommonFunctions().GetStringToDateTime("01/01/2022")) && (model.CHQ_EPAY == "Q" && model.CHQ_AMOUNT > 0) && Convert.ToInt32(model.TXN_ID.Split('$')[0]) != 462)

                //Modify Below Condition on 20-01-2022
                //if (PMGSYSession.Current.FundType == "A" && (new CommonFunctions().GetStringToDateTime(model.BILL_DATE) >= new CommonFunctions().GetStringToDateTime("01/01/2022")) && (model.CHQ_EPAY == "Q" && model.CHQ_AMOUNT > 0) &&  ((Convert.ToInt32(model.TXN_ID.Split('$')[0]) != 462 && PMGSYSession.Current.LevelId==5 )|| (Convert.ToInt32(model.TXN_ID.Split('$')[0]) != 384 && PMGSYSession.Current.LevelId==4)) )

                #region Region to read ByPassStateChkTrans XML File

                XDocument doc_xml = XDocument.Load(System.Web.HttpContext.Current.Server.MapPath("~/ByPassStateChkTrans.xml"));
                List<int> StateCode = new List<int>();
                foreach (XElement element in doc_xml.Descendants("stateList").Descendants("statCode"))
                {
                    StateCode.Add(Convert.ToInt32(element.Value));
                }
                //xml Code end
                string ChqBackDateUpto = ConfigurationManager.AppSettings["CHQ_PAYMENT_BACK_DATE_UPTO"].ToString();
                string chqAllowUpto = ConfigurationManager.AppSettings["CHQ_PAYMENT_ALLOW_UPTO"].ToString();
                #endregion

                if (PMGSYSession.Current.FundType == "A" && (new CommonFunctions().GetStringToDateTime(model.BILL_DATE) >= new CommonFunctions().GetStringToDateTime("01/01/2022")) && (model.CHQ_EPAY == "Q" && model.CHQ_AMOUNT > 0) && ((Convert.ToInt32(model.TXN_ID.Split('$')[0]) != 462 && Convert.ToInt32(model.TXN_ID.Split('$')[0]) != 472 && PMGSYSession.Current.LevelId == 5) || (Convert.ToInt32(model.TXN_ID.Split('$')[0]) != 384 && Convert.ToInt32(model.TXN_ID.Split('$')[0]) != 390 && Convert.ToInt32(model.TXN_ID.Split('$')[0]) != 635 && PMGSYSession.Current.LevelId == 4)))
                {

                    //if ((Convert.ToInt32(model.TXN_ID.Split('$')[0]) != 469) && (Convert.ToInt32(model.TXN_ID.Split('$')[0]) != 3078))
                    //{
                    //    return Json(new { message = "Payment mode should be Epayment for selected transaction type" }, JsonRequestBehavior.AllowGet);

                    //}

                    //Allow Check Payment for the Tamilnadu state upto 05-01-2023


                    if ((Convert.ToInt32(model.TXN_ID.Split('$')[0]) != 469) && (Convert.ToInt32(model.TXN_ID.Split('$')[0]) != 3078))
                    {
                        if (!(PMGSYSession.Current.FundType == "A" && new CommonFunctions().GetStringToDateTime(model.BILL_DATE) <= new CommonFunctions().GetStringToDateTime(ChqBackDateUpto) && DateTime.Now.Date <= new CommonFunctions().GetStringToDateTime(chqAllowUpto) && StateCode.Contains(PMGSYSession.Current.StateCode) && PMGSYSession.Current.LevelId == 4))
                        {
                            return Json(new { message = "Payment mode should be Epayment for selected transaction type" }, JsonRequestBehavior.AllowGet);
                        }

                    }

                }
                else if (PMGSYSession.Current.FundType == "A" && (model.CHQ_EPAY == "Q" && model.CHQ_AMOUNT > 0) && ((Convert.ToInt32(model.TXN_ID.Split('$')[0]) != 462 && Convert.ToInt32(model.TXN_ID.Split('$')[0]) != 472 && PMGSYSession.Current.LevelId == 5) || (Convert.ToInt32(model.TXN_ID.Split('$')[0]) != 384 && Convert.ToInt32(model.TXN_ID.Split('$')[0]) != 390 && Convert.ToInt32(model.TXN_ID.Split('$')[0]) != 635 && PMGSYSession.Current.LevelId == 4)))
                {
                    //Allow Check Payment Before December 2022 for the state . uncomment if required
                    //if (PMGSYSession.Current.FundType == "A" && DateTime.Now.Date <= new CommonFunctions().GetStringToDateTime("11/07/2022") && new CommonFunctions().GetStringToDateTime(model.BILL_DATE) < new CommonFunctions().GetStringToDateTime("01/01/2022"))

                    //Allow Check Payment for the Tamilnadu state upto 05-01-2023
                    if (!(PMGSYSession.Current.FundType == "A" && new CommonFunctions().GetStringToDateTime(model.BILL_DATE) <= new CommonFunctions().GetStringToDateTime(ChqBackDateUpto) && DateTime.Now.Date <= new CommonFunctions().GetStringToDateTime(chqAllowUpto) && StateCode.Contains(PMGSYSession.Current.StateCode) && PMGSYSession.Current.LevelId == 4))
                    {
                        return Json(new { message = "Payment mode should be Epayment for selected transaction type" }, JsonRequestBehavior.AllowGet);
                    }

                }
                #endregion

                #region Confirmation is pending

                if (PMGSYSession.Current.LevelId == 5)
                {
                    //bool status = objPaymentBAL.ValidateifMonthAcknowledged(PMGSYSession.Current.ParentNDCode, PMGSYSession.Current.AdminNdCode, PMGSYSession.Current.FundType, Convert.ToInt16(model.BILL_MONTH), Convert.ToInt16(model.BILL_YEAR));
                    //if (status == false)
                    //{
                    //    return Json(new { Success = false, Bill_ID = -9 });
                    //}
                }

                #endregion

                CommonFunctions comm = new CommonFunctions();
                string PfmsDate = "01/08/2018";
                bool isChqPaymentAllowed = false;

                if (PMGSYSession.Current.FundType == "M")  // --- Punjab Change
                {
                    isChqPaymentAllowed = true;
                    //if ((comm.GetStringToDateTime(model.BILL_DATE.Trim().Replace('-', '/')) < comm.GetStringToDateTime(PfmsDate.Trim())))
                    //{
                    //    isChqPaymentAllowed = true;
                    //}
                    //else
                    //{
                    //    isChqPaymentAllowed = false;
                    //}
                }



                #region Maintenance Fund Contractor Bank Validations
                // if (PMGSYSession.Current.FundType.Equals("M") && model.MAST_CON_ID_C.HasValue && model.MAST_CON_ID_C.Value > 0 && model.CHQ_EPAY.Trim() != "A")//MF Advice Payment
                if (PMGSYSession.Current.FundType.Equals("M") && model.MAST_CON_ID_C.HasValue && model.MAST_CON_ID_C.Value > 0 && model.CHQ_EPAY.Trim() != "A" && isChqPaymentAllowed == false)//MF Advice Payment  --- Punjab Change
                {
                    bool isPFMSFinalized = false;

                    isPFMSFinalized = payDAL.IsPFMSFinalized(model.MAST_CON_ID_C.Value, model.conAccountId);
                    if (isPFMSFinalized == false)
                    {
                        return Json(new { Success = false, Bill_ID = -5 });
                    }
                    if (model.MAST_CON_ID_C.HasValue && model.MAST_CON_ID_C.Value > 0)
                    {
                        bool status = objPaymentBAL.ValidateContractorStatus(model.MAST_CON_ID_C.Value, Convert.ToInt32(model.TXN_ID.Split('$')[0]));
                        if (status == false)
                        {
                            return Json(new { Success = false, Bill_ID = -5 });
                        }
                    }

                    if (model.MAST_CON_ID_S.HasValue && model.MAST_CON_ID_S.Value > 0)
                    {
                        bool status = objPaymentBAL.ValidateContractorStatus(model.MAST_CON_ID_S.Value, Convert.ToInt32(model.TXN_ID.Split('$')[0]));
                        if (status == false)
                        {
                            return Json(new { Success = false, Bill_ID = -5 });
                        }
                    }
                }
                #endregion

                if (model.CHQ_EPAY == "E")
                {
                    bool status = objPaymentBAL.ValidateDPIUEpaymentBAL(PMGSYSession.Current.AdminNdCode);
                    if (status == false)
                    {
                        return Json(new { Success = false, Bill_ID = -111 });
                    }
                }

                if (model.CHQ_EPAY == "R")
                {
                    bool status = objPaymentBAL.ValidateDPIUEremittenceBAL(PMGSYSession.Current.AdminNdCode);
                    if (status == false)
                    {
                        return Json(new { Success = false, Bill_ID = -222 });
                    }
                }

                if (model.CHQ_EPAY == "E")
                {
                    if (PMGSYSession.Current.FundType.Equals("P") || PMGSYSession.Current.FundType.Equals("M"))
                    {
                        ///PFMS Validations 
                        if (PMGSYSession.Current.FundType == "P")
                        {
                            bool isPFMSFinalized = false;

                            //Below condition is commented on 02-12-2022
                            //isPFMSFinalized = payDAL.IsPFMSFinalized(model.MAST_CON_ID_C.Value, model.conAccountId);

                            //Below condition is Added on 02-12-2022
                            if (model.MAST_CON_ID_C.HasValue && model.MAST_CON_ID_C.Value > 0)
                            {
                                isPFMSFinalized = payDAL.IsPFMSFinalized(model.MAST_CON_ID_C.Value, model.conAccountId);
                            }
                            else if (model.MAST_CON_ID_S.HasValue && model.MAST_CON_ID_S.Value > 0)
                            {
                                isPFMSFinalized = payDAL.IsPFMSFinalized(model.MAST_CON_ID_S.Value, model.conAccountId);

                            }

                            //if (isPFMSFinalized == false) //Commeneted on 21-04-2023
                            if (isPFMSFinalized == false && Convert.ToInt32(model.TXN_ID.Split('$')[0]) != 3185) //Added on 21-04-2023 to bypass va;lidation for txn_id=3185
                            {
                                return Json(new { Success = false, Bill_ID = -5 });
                            }
                        }
                        if (model.MAST_CON_ID_C.HasValue && model.MAST_CON_ID_C.Value > 0)
                        {
                            bool status = objPaymentBAL.ValidateContractorStatus(model.MAST_CON_ID_C.Value, Convert.ToInt32(model.TXN_ID.Split('$')[0]));
                            if (status == false)
                            {
                                return Json(new { Success = false, Bill_ID = -5 });
                            }
                        }

                        if (model.MAST_CON_ID_S.HasValue && model.MAST_CON_ID_S.Value > 0)
                        {
                            bool status = objPaymentBAL.ValidateContractorStatus(model.MAST_CON_ID_S.Value, Convert.ToInt32(model.TXN_ID.Split('$')[0]));
                            if (status == false)
                            {
                                return Json(new { Success = false, Bill_ID = -5 });
                            }
                        }
                    }
                    else if (PMGSYSession.Current.FundType.Equals("A"))
                    {
                        //if (model.MAST_CON_ID_S.HasValue)
                        //{
                        //    bool status = objPaymentBAL.ValidateContractorStatus(model.MAST_CON_ID_S.Value, Convert.ToInt32(model.TXN_ID.Split('$')[0]));
                        //    if (status == false)
                        //    {
                        //        return Json(new { Success = false, Bill_ID = -5 });
                        //    }
                        //}
                        string con_sup_id = (model.MAST_CON_ID_S != 0) ? "MAST_CON_ID_S" : (model.MAST_CON_ID_C != 0) ? "MAST_CON_ID_C" : "";

                        switch (con_sup_id)
                        {
                            case "MAST_CON_ID_S":
                                if (model.MAST_CON_ID_S.HasValue)
                                {
                                    bool status1 = objPaymentBAL.ValidateContractorStatus(model.MAST_CON_ID_S.Value, Convert.ToInt32(model.TXN_ID.Split('$')[0]));
                                    if (status1 == false)
                                    {
                                        return Json(new { Success = false, Bill_ID = -5 });
                                    }
                                }
                                break;

                            case "MAST_CON_ID_C":
                                if (model.MAST_CON_ID_C.HasValue)
                                {
                                    bool status2 = objPaymentBAL.ValidateContractorStatus(model.MAST_CON_ID_C.Value, Convert.ToInt32(model.TXN_ID.Split('$')[0]));
                                    if (status2 == false)
                                    {
                                        return Json(new { Success = false, Bill_ID = -5 });
                                    }
                                }
                                break;

                            default:
                                break;
                        }
                    }

                }





                //Added By Abhishek kamble for Advice No 6Apr2015 start
                bool IsAdvicePayment = false;
                if (model.CHQ_EPAY == "A")
                {
                    if ((model.CHQ_NO.Contains("\\")) || (model.CHQ_NO.Contains("/")))
                    {
                        ModelState.AddModelError("CHQ_NO", @"Advice number contains invalid character / or \");
                    }
                    IsAdvicePayment = true;
                    model.CHQ_EPAY = "Q";
                }
                //Added By Abhishek kamble for Advice No 6Apr2015 end



                Int64 Bill_ID = 0;
                TransactionParams objparams = new TransactionParams();
                CommonFunctions objCommon = new CommonFunctions();
                ACC_SCREEN_DESIGN_PARAM_MASTER obj = new ACC_SCREEN_DESIGN_PARAM_MASTER();
                String message = String.Empty;//new change done by Vikram on 01-Jan-2014


                model.ND_CODE = PMGSYSession.Current.AdminNdCode;

                model.FUND_TYPE = PMGSYSession.Current.FundType;

                model.LVL_ID = PMGSYSession.Current.LevelId;

                objparams.DISTRICT_CODE = PMGSYSession.Current.DistrictCode;

                objparams.BILL_TYPE = "P";

                objparams.FUND_TYPE = PMGSYSession.Current.FundType;

                objparams.MONTH = Convert.ToInt16(model.BILL_MONTH);

                objparams.YEAR = Convert.ToInt16(model.BILL_YEAR);

                objparams.LVL_ID = PMGSYSession.Current.LevelId;

                objparams.ADMIN_ND_CODE = PMGSYSession.Current.AdminNdCode;

                #region validation based on master design params

                //Added BY Abhishek kamble 15-Apr-2014
                //|| (model.TXN_ID == "47$Q") || (model.TXN_ID == "737$Q")
                if ((model.TXN_ID == "137$Q") || (model.TXN_ID == "834$Q") || (model.TXN_ID == "469$Q"))
                {
                    if (ModelState.ContainsKey("CHQ_DATE"))
                        ModelState["CHQ_DATE"].Errors.Clear();
                    ModelState["CONC_Account_ID"].Errors.Clear();
                    ModelState["conAccountId"].Errors.Clear();

                    // ModelState["CONC_Account_ID1"].Errors.Clear();

                    // ModelState["conAccountID"].Errors.Clear();
                }
                else if ((model.TXN_ID == "47$Q") || (model.TXN_ID == "737$Q") || (model.TXN_ID == "1484$Q") || (model.TXN_ID == "1974$Q"))//PMGSY3
                {
                    if (model.CHQ_AMOUNT == 0)
                    {
                        if (ModelState.ContainsKey("CHQ_DATE"))
                            ModelState["CHQ_DATE"].Errors.Clear();
                    }
                }

                if (ModelState.ContainsKey("CONC_Account_ID"))
                    ModelState["CONC_Account_ID"].Errors.Clear();
                if (ModelState.ContainsKey("conAccountId"))
                    ModelState["conAccountId"].Errors.Clear();

                var errors = ModelState.Where(n => n.Value.Errors.Count > 0).ToList();

                //if model is valid by view model validations
                if (ModelState.IsValid)
                {

                    if (model.BILL_MONTH != 0 && model.BILL_MONTH < 13 && model.BILL_YEAR != 0)
                    {
                        string monthlyClosingStatus = string.Empty;
                        String errMessage = String.Empty;
                        monthlyClosingStatus = objCommon.MonthlyClosingValidation(model.BILL_MONTH, model.BILL_YEAR, objparams.FUND_TYPE, model.LVL_ID, model.ND_CODE, ref errMessage);

                        if (monthlyClosingStatus.Equals("-111"))
                        {
                            ModelState.AddModelError("BILL_MONTH", errMessage);
                        }
                        if (monthlyClosingStatus.Equals("-222"))
                        {
                            ModelState.AddModelError("BILL_MONTH", "Month is already closed.");
                        }
                    }

                    if (model.CHQ_EPAY != "Q" && model.CHQ_EPAY != "E" && model.CHQ_EPAY != "C" && model.CHQ_EPAY != "R")
                    {
                        ModelState.AddModelError("CHQ_EPAY", "Invalid Mode of Transaction");
                    }

                    //if Remittance set the  model.CHQ_EPAY  as epayment and  Model.IS_EREMIT to true to indicate eremitance
                    if (model.CHQ_EPAY == "R")
                    {
                        model.CHQ_EPAY = "E";
                        model.IS_EREMIT = true;
                    }

                    //check if opening balance entry is finalized
                    /*  bool oBFinalized = common.CheckIfOBFinalized(objparams);

                      if (!oBFinalized)
                      {
                          return Json(new { Success = false, Bill_ID = -4 });
                      }
                      */
                    //bill date should be greater than opening balance date
                    DateTime? openingBalanceDate = common.GetOBDate(objparams);

                    if (openingBalanceDate.HasValue)
                    {
                        if (common.GetStringToDateTime(model.BILL_DATE) < openingBalanceDate.Value)
                        {
                            ModelState.AddModelError("BILL_DATE", "Voucher Date must be greater than or equal to opening balance date");
                        }
                    }

                    //if Epayment: Bill Date and epay date must be equal to current date 
                    if (model.CHQ_EPAY.Trim().Equals("E"))
                    {
                        DateTime bill_Date = common.GetStringToDateTime(model.BILL_DATE);
                        DateTime chq_Date = common.GetStringToDateTime(model.CHQ_DATE);

                        if (bill_Date != DateTime.Now.Date)
                        {
                            ModelState.AddModelError("BILL_DATE", "Voucher Date must be equal to todays date ");
                        }

                        if (chq_Date != DateTime.Now.Date)
                        {
                            ModelState.AddModelError("CHQ_DATE", "Epayment Date must be equal to todays date ");
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


                        obj = common.getMasterDesignParam(objparams);

                        //Below code added on 14-06-2023
                        if (PMGSYSession.Current.FundType == "A" && (objparams.TXN_ID == 472 || objparams.TXN_ID == 390) && !model.CHQ_EPAY.Trim().Equals("E"))
                        {
                            obj.MAST_CON_REQ = "N";
                        }

                        //if (PMGSYSession.Current.FundType == "A" && DateTime.Now.Date <= new CommonFunctions().GetStringToDateTime("11/07/2022") && new CommonFunctions().GetStringToDateTime(model.BILL_DATE) < new CommonFunctions().GetStringToDateTime("01/01/2022"))
                        if ((PMGSYSession.Current.FundType == "A" && new CommonFunctions().GetStringToDateTime(model.BILL_DATE) <= new CommonFunctions().GetStringToDateTime(ChqBackDateUpto) && DateTime.Now.Date <= new CommonFunctions().GetStringToDateTime(chqAllowUpto) && StateCode.Contains(PMGSYSession.Current.StateCode))
                            ||
                            (PMGSYSession.Current.FundType == "P" && DateTime.Now.Date <= new CommonFunctions().GetStringToDateTime("22/10/2022") && PMGSYSession.Current.StateCode == 5 && PMGSYSession.Current.UserId == 1315)
                           )
                        {
                            //obj.MAST_CON_REQ = "N";
                            obj.MAST_CON_REQ = PMGSYSession.Current.FundType == "P" ? obj.MAST_CON_REQ : "N";
                            //obj.MAST_SUPPLIER_REQ = "N";
                            obj.MAST_SUPPLIER_REQ = PMGSYSession.Current.FundType == "P" ? obj.MAST_SUPPLIER_REQ : "N";
                            //obj.MAST_AGREEMENT_REQ = "N";
                            obj.MAST_AGREEMENT_REQ = PMGSYSession.Current.FundType == "P" ? obj.MAST_AGREEMENT_REQ : "N";

                            obj.REM_REQ = "N";

                        }

                        if (obj.MAST_CON_REQ.Trim() == "Y")
                        {
                            model.MAST_CON_REQ = true;

                            if (model.MAST_CON_ID_C == null || model.MAST_CON_ID_C == 0)
                            {
                                ModelState.AddModelError("MAST_CON_ID_C", "Company Name is Required");
                            }
                            if (model.PAYEE_NAME_C == null || model.PAYEE_NAME_C == string.Empty)
                            {
                                ModelState.AddModelError("PAYEE_NAME_C", "Payee Name is Required");
                            }

                        }
                        else
                        {

                            if (ModelState.ContainsKey("MAST_CON_ID_C"))
                                ModelState["MAST_CON_ID_C"].Errors.Clear();
                            if (ModelState.ContainsKey("PAYEE_NAME_C"))
                                ModelState["PAYEE_NAME_C"].Errors.Clear();
                        }


                        if (obj.MAST_SUPPLIER_REQ.Trim() == "Y")
                        {
                            model.MAST_SUPPLIER_REQ = true;
                            if (model.MAST_CON_ID_S == null)
                            {
                                ModelState.AddModelError("MAST_CON_ID_S", "Company Name is Required");
                            }
                            //if (model.PAYEE_NAME_S == null || model.PAYEE_NAME_S == string.Empty)
                            //{
                            //    ModelState.AddModelError("PAYEE_NAME_S", "Payee Name is Required");
                            //}
                        }
                        else
                        {
                            if (ModelState.ContainsKey("MAST_CON_ID_S"))
                                ModelState["MAST_CON_ID_S"].Errors.Clear();
                            if (ModelState.ContainsKey("PAYEE_NAME_S"))
                                ModelState["PAYEE_NAME_S"].Errors.Clear();

                        }

                        if (obj.REM_REQ.Trim() == "Y")
                        {
                            model.REM_REQ = true;

                            if (model.DEPT_ID == 0)
                            {
                                ModelState.AddModelError("DEPT_ID", "Department Name is Required");
                            }
                            if (model.CHALAN_DATE != string.Empty && model.CHALAN_DATE != null)
                            {
                                DateTime chalanDate = common.GetStringToDateTime(model.CHALAN_DATE);


                                if (chalanDate > DateTime.Now.Date)
                                {
                                    ModelState.AddModelError("CHALAN_DATE", "Chalan Date must be less than or equal to today's date");
                                }


                            }
                            /*  if (model.CHALAN_NO == null || model.CHALAN_NO == string.Empty)
                             {
                                 ModelState.AddModelError("CHALAN_NO", "Chalan Number is Required");
                             }*/

                        }
                        else
                        {
                            if (ModelState.ContainsKey("DEPT_ID"))
                                ModelState["DEPT_ID"].Errors.Clear();
                            if (ModelState.ContainsKey("CHALAN_DATE"))
                                ModelState["CHALAN_DATE"].Errors.Clear();
                            if (ModelState.ContainsKey("CHALAN_NO"))
                                ModelState["CHALAN_NO"].Errors.Clear();
                        }

                        if (obj.CASH_CHQ.Trim() == "C" && model.CHQ_EPAY == "C")
                        {
                            model.CASH_REQ = true;
                            if (model.CASH_AMOUNT == null)
                            {
                                ModelState.AddModelError("CASH_AMOUNT", "Cash Amount is Required");
                            }

                        }
                        else
                        {
                            if (ModelState.ContainsKey("CASH_AMOUNT"))
                                ModelState["CASH_AMOUNT"].Errors.Clear();
                        }

                        if (obj.CASH_CHQ.Trim() == "Q" && model.CHQ_EPAY == "Q")
                        {
                            model.CHQ_REQ = true;

                            //If Condition Added By Abhishek kamble 14-Apr-2014
                            //&& (objparams.TXN_ID != 47) && (objparams.TXN_ID != 737)
                            if ((objparams.TXN_ID != 137) && (objparams.TXN_ID != 834) && (objparams.TXN_ID != 469) && (model.CHQ_AMOUNT != 0))
                            {
                                if (model.CHQ_DATE == null || model.CHQ_DATE == string.Empty)
                                {
                                    ModelState.AddModelError("CHQ_DATE", "Cheque/Epay Date is Required");
                                }
                            }
                            if (model.CHQ_AMOUNT == null)
                            {
                                ModelState.AddModelError("CHQ_AMOUNT", "Amount is Required");
                            }

                            //If Condition Added By Abhishek kamble 14-Apr-2014
                            //&& (objparams.TXN_ID != 47) && (objparams.TXN_ID != 737)
                            if ((objparams.TXN_ID != 137) && (objparams.TXN_ID != 834) && (objparams.TXN_ID != 469) && (model.CHQ_AMOUNT != 0))
                            {
                                if (model.CHQ_NO == null)
                                {
                                    ModelState.AddModelError("CHQ_NO", "Cheque Number is Required");
                                }
                            }

                            //check for valid cheque book number only if DPIU
                            if (PMGSYSession.Current.LevelId == 5)
                            {
                                if (model.CHQ_Book_ID == null)
                                {
                                    ModelState.AddModelError("CHQ_Book_ID", "Cheque Series is Required");
                                }
                                else
                                {
                                    //If Condition Added By Abhishek kamble 14-Apr-2014
                                    //&& (objparams.TXN_ID != 47) && (objparams.TXN_ID != 737)
                                    if ((objparams.TXN_ID != 137) && (objparams.TXN_ID != 834) && (objparams.TXN_ID != 469) && (model.CHQ_AMOUNT != 0))
                                    {
                                        //If Condition Added By Abhishek kamble 6-Apr-2015 for Advice No
                                        if (IsAdvicePayment)
                                        {

                                            //if (objPaymentBAL.ValidateAdviceNoExist(model.CHQ_NO, 0))
                                            //{
                                            //    ModelState.AddModelError("CHQ_NO", "Advice No is already exist.");
                                            //    model.CHQ_EPAY = "A";
                                            //}
                                        }
                                        else
                                        {
                                            String[] availableCheques = objPaymentBAL.GetAllAvailableChequesArray(model.CHQ_Book_ID.Value, PMGSYSession.Current.AdminNdCode, PMGSYSession.Current.FundType);

                                            int pos = Array.IndexOf(availableCheques, model.CHQ_NO);
                                            if (pos == -1)
                                            {
                                                ModelState.AddModelError("CHQ_NO", "Invalid Cheque Number");
                                                //}
                                            }
                                        }
                                    }
                                }

                            }
                            //at srrda LEVEL CHECK IF cheque series is selected
                            if (PMGSYSession.Current.LevelId == 4 && model.CHQ_Book_ID != 0)
                            {
                                //If Condition Added By Abhishek kamble 6-Apr-2015 for Advice No
                                if (IsAdvicePayment)
                                {

                                    //if (objPaymentBAL.ValidateAdviceNoExist(model.CHQ_NO, Bill_ID))
                                    //{
                                    //    ModelState.AddModelError("CHQ_NO", "Advice No is already exist.");
                                    //    model.CHQ_EPAY = "A";
                                    //}
                                }
                                else
                                {
                                    //if cheque book id is selected check if entered cheque is in selected series
                                    if (model.CHQ_Book_ID != 0)
                                    {
                                        String[] availableCheques = objPaymentBAL.GetAllAvailableChequesArray(model.CHQ_Book_ID.Value, PMGSYSession.Current.AdminNdCode, PMGSYSession.Current.FundType);

                                        int pos = Array.IndexOf(availableCheques, model.CHQ_NO);
                                        if (pos == -1)
                                        {
                                            //If Condition Added By Abhishek kamble 14-Apr-2014
                                            //&& (objparams.TXN_ID != 47) && (objparams.TXN_ID != 737) && (objparams.TXN_ID != 47) && (objparams.TXN_ID != 737)
                                            if ((objparams.TXN_ID != 137) && (objparams.TXN_ID != 834) && (objparams.TXN_ID != 469) && (model.CHQ_AMOUNT != 0))
                                            {
                                                ModelState.AddModelError("CHQ_NO", "Invalid Cheque Number");
                                            }
                                        }
                                    }
                                }
                            }

                            //Validation commented by Abhisehk kamble 6 Nov 2014 because 'This Cheque is already issued' validation not required at SRRDA level

                            //if (PMGSYSession.Current.LevelId == 4 && model.CHQ_Book_ID == 0)
                            //{
                            //    //check if cheque already entered
                            //    bool chequeAllreadyIssued = false;

                            //    chequeAllreadyIssued = objPaymentBAL.IschequeIssued(model.CHQ_NO, "A", 0);

                            //    if (chequeAllreadyIssued)
                            //    {
                            //        //If Condition Added By Abhishek kamble 14-Apr-2014
                            //        //&& (objparams.TXN_ID != 47) && (objparams.TXN_ID != 737)
                            //        if ((objparams.TXN_ID != 137) && (objparams.TXN_ID != 834) && (objparams.TXN_ID != 469) && (model.CHQ_AMOUNT != 0))
                            //        {
                            //            ModelState.AddModelError("CHQ_NO", "This Cheque is already issued");
                            //        }
                            //    }

                            //}

                        }
                        else
                        {
                            if (ModelState.ContainsKey("CHQ_DATE"))
                                ModelState["CHQ_DATE"].Errors.Clear();
                            if (ModelState.ContainsKey("CHQ_AMOUNT"))
                                ModelState["CHQ_AMOUNT"].Errors.Clear();
                            if (ModelState.ContainsKey("CHQ_NO"))
                                ModelState["CHQ_NO"].Errors.Clear();

                        }

                        if (obj.DED_REQ.Trim() == "Y")
                        {
                            model.DED_REQ = true;
                            if (model.DEDUCTION_AMOUNT == null)
                            {
                                ModelState.AddModelError("DEDUCTION_AMOUNT", "Deduction Amount is Required");

                            }

                        }
                        else
                        {
                            if (ModelState.ContainsKey("DEDUCTION_AMOUNT"))
                                ModelState["DEDUCTION_AMOUNT"].Errors.Clear();
                        }

                        //if payment is only for deduction 
                        if (obj.DED_REQ.Trim() == "B" && model.CHQ_EPAY == "C")
                        {
                            model.DED_REQ = true;
                            if (model.DEDUCTION_AMOUNT == null)
                            {
                                ModelState.AddModelError("DEDUCTION_AMOUNT", "Deduction Amount is Required");
                            }
                        }
                        else
                        {
                            if (ModelState.ContainsKey("DEDUCTION_AMOUNT"))
                                ModelState["DEDUCTION_AMOUNT"].Errors.Clear();
                        }

                        if (obj.EPAY_REQ == "Y" && model.CHQ_EPAY == "E")
                        {
                            model.EPAY_REQ = true;

                            if (model.EPAY_NO == null || model.EPAY_NO == string.Empty)
                            {
                                ModelState.AddModelError("EPAY_NO", "Epayment Number is Required");
                            }
                        }
                        else
                        {
                            if (ModelState.ContainsKey("EPAY_REQ"))
                                ModelState["EPAY_REQ"].Errors.Clear();
                        }

                    }

                    if (obj.DED_REQ == "B" && model.CHQ_EPAY == "C")
                    {
                        //do not validate the amount as its only deduction

                        if (model.DEDUCTION_AMOUNT != null && model.DEDUCTION_AMOUNT == 0)
                        {
                            ModelState.AddModelError("DEDUCTION_AMOUNT", "Deduction Amount should be greater than zero");
                        }

                    }
                    else
                    {
                        //if model is still valid then only get the payment balnces to validate cash/cheque amount 
                        if (ModelState.IsValid)
                        {

                            //Commented on 05-05-2023
                            //UDF_ACC_GEN_GET_BA_CASH_CLOSING_BALANCES_Result balance = new UDF_ACC_GEN_GET_BA_CASH_CLOSING_BALANCES_Result();
                            //Added on 05-05-2023
                            UDF_ACC_GEN_GET_BA_CASH_CLOSING_BALANCES_COMPOSITE_Result balance = new UDF_ACC_GEN_GET_BA_CASH_CLOSING_BALANCES_COMPOSITE_Result();

                            balance = objPaymentBAL.GetCloSingBalanceForPayment(objparams);

                            if (model.CHQ_EPAY == "Q" || model.CHQ_EPAY == "E")
                            {
                                //Cheque amount should be less than equal to Bank authorization balance amount for cheque payment 



                                if (model.CHQ_AMOUNT != null && model.CHQ_AMOUNT != 0)
                                {
                                    //Below condition added on 05-05-2023
                                    if (objparams.TXN_ID == 3185 || objparams.TXN_ID == 3173)
                                    {
                                        if (Convert.ToDecimal(balance.Bank_Auth_Holding) < Convert.ToDecimal(model.CHQ_AMOUNT))
                                        {
                                            ModelState.AddModelError("CHQ_AMOUNT", "Amount  must be less than or equal to Holding Account Bank Authorization balance amount");
                                        }
                                    }
                                    else if (objparams.TXN_ID == 3187)
                                    {
                                        if (Convert.ToDecimal(balance.Bank_Auth_Security) < Convert.ToDecimal(model.CHQ_AMOUNT))
                                        {
                                            ModelState.AddModelError("CHQ_AMOUNT", "Amount  must be less than or equal to Security Deposit Account Bank Authorization balance amount");
                                        }
                                    }
                                    else
                                    {
                                        if (Convert.ToDecimal(balance.bank_auth) < Convert.ToDecimal(model.CHQ_AMOUNT))
                                        {
                                            ModelState.AddModelError("CHQ_AMOUNT", "Amount  must be less than or equal to Bank Authorization balance amount");
                                        }

                                    }

                                    if (model.CHQ_AMOUNT == 0)
                                    {
                                        ModelState.AddModelError("CHQ_AMOUNT", "Amount should be greater than zero");
                                    }

                                }

                                if (model.CHQ_AMOUNT != null && model.CHQ_AMOUNT != null && model.DEDUCTION_AMOUNT != 0 && model.DEDUCTION_AMOUNT != null)
                                {
                                    //if (model.CHQ_AMOUNT < model.DEDUCTION_AMOUNT)
                                    //{

                                    //    ModelState.AddModelError("DEDUCTION_AMOUNT", "Deduction Amount should be less than amount");
                                    //}


                                    if (model.CHQ_AMOUNT == 0)
                                    {
                                        ModelState.AddModelError("CHQ_AMOUNT", "Amount should be greater than zero");
                                    }
                                }

                            }

                            if (model.CHQ_EPAY == "C" && obj.DED_REQ != "B")
                            {
                                //cash amount   should be less than equal to balance amount for cash payment 
                                if (model.CASH_AMOUNT != null && model.CASH_AMOUNT != 0)
                                {
                                    if (Convert.ToDecimal(balance.cash) < Convert.ToDecimal(model.CASH_AMOUNT))
                                    {
                                        ModelState.AddModelError("CASH_AMOUNT", "Amount must be less than or equal to Cash Balance Amount");
                                    }
                                }
                            }

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
                        model.PAYEE_NAME = model.PAYEE_NAME_C;
                        model.CON_ID = model.MAST_CON_ID_C;
                    }
                    if (obj.MAST_SUPPLIER_REQ.Trim() == "Y")
                    {
                        model.PAYEE_NAME = model.PAYEE_NAME_S;
                        model.CON_ID = model.MAST_CON_ID_S;
                    }
                    if (obj.REM_REQ.Trim() == "Y")
                    {
                        model.PAYEE_NAME = model.PAYEE_NAME_R;
                        model.REMIT_TYPE = model.DEPT_ID;
                        // model.MAST_CON_ID = model.DEPT_ID;

                    }
                    if (obj.REM_REQ.Trim() == "N")
                    {
                        model.CHALAN_DATE = null;
                        model.CHALAN_NO = null;
                        // model.MAST_CON_ID = model.DEPT_ID;
                    }
                    else
                    {
                        model.PAYEE_NAME = model.PAYEE_NAME;
                    }

                    //check for cheque/cash
                    if (obj.CASH_CHQ.Trim() == "C")
                    {
                        model.CHQ_AMOUNT = 0;
                        model.CHQ_DATE = null;
                        model.CHQ_NO = null;
                    }
                    if (obj.CASH_CHQ.Trim() == "Q")
                    {
                        model.CASH_AMOUNT = 0;

                    }


                    if (obj.DED_REQ.Trim() == "Y" || obj.DED_REQ.Trim() == "B")
                    {
                        //Cheque transaction with deduction put deduction in cash amount
                        if (obj.CASH_CHQ.Trim() == "Q")
                        {
                            model.CASH_AMOUNT = model.DEDUCTION_AMOUNT;
                        }
                    }

                    decimal GrossAmount = 0;

                    if (obj.DED_REQ.Trim() == "B" && model.CHQ_EPAY == "C")
                    {

                        model.CASH_AMOUNT = model.DEDUCTION_AMOUNT;
                        model.CHQ_AMOUNT = 0;
                        GrossAmount = model.CASH_AMOUNT.Value;
                    }
                    else if (obj.DED_REQ.Trim() != "B" && model.CHQ_EPAY == "C")
                    {
                        model.DEDUCTION_AMOUNT = 0;
                        model.CHQ_AMOUNT = 0;
                    }
                    else if (obj.DED_REQ.Trim() == "N" && (model.CHQ_EPAY == "Q"))
                    {
                        model.DEDUCTION_AMOUNT = 0;
                        model.CASH_AMOUNT = 0;

                    }


                    //get the month from date

                    model.BILL_MONTH = Convert.ToInt16(model.BILL_DATE.Split('/')[1]);
                    model.BILL_YEAR = Convert.ToInt16(model.BILL_DATE.Split('/')[2]);

                    //calculate gross amount 



                    if (model.CHQ_AMOUNT != null)
                    {
                        if (model.CHQ_AMOUNT != 0)
                        {
                            if (model.DEDUCTION_AMOUNT != null)
                            {
                                GrossAmount = model.CHQ_AMOUNT.Value + model.DEDUCTION_AMOUNT.Value;
                            }
                            else
                            {
                                GrossAmount = model.CHQ_AMOUNT.Value;
                            }
                        }
                    }

                    if (obj.DED_REQ.Trim() != "B" && model.CHQ_EPAY == "C")
                    {
                        if (model.CASH_AMOUNT != null)
                        {
                            if (model.CASH_AMOUNT != 0)
                            {
                                if (model.DEDUCTION_AMOUNT != null)
                                {
                                    if (obj.DED_REQ.Trim() == "B" && model.CHQ_EPAY == "C")
                                    {
                                        GrossAmount = model.CASH_AMOUNT.Value;
                                    }
                                    else
                                    {
                                        GrossAmount = model.CASH_AMOUNT.Value + model.DEDUCTION_AMOUNT.Value;
                                    }

                                }
                                else
                                {
                                    GrossAmount = model.CASH_AMOUNT.Value;
                                }
                            }
                        }
                    }

                    if (model.CHQ_EPAY.Trim() == "E")
                    {
                        model.CHQ_NO = model.EPAY_NO;

                    }

                    model.GROSS_AMOUNT = GrossAmount;

                    //Added By Abhishek kamble for Advice No 6Apr2015 start                    
                    //if (IsAdvicePayment)
                    //{
                    //    //model.CHALAN_NO = "A";
                    //    model.CHQ_EPAY = "A";
                    //}
                    //Added By Abhishek kamble for Advice No 6Apr2015 end  
                    Bill_ID = objPaymentBAL.AddEditMasterPaymentDetails(model, "A", Bill_ID, IsAdvicePayment);


                    int Master_Head = Convert.ToInt32(model.TXN_ID.Split('$')[0]);

                    bool deductionRequired = model.DEDUCTION_AMOUNT == 0 ? false : true;

                    bool PaymentRequired = model.CHQ_AMOUNT == 0 ? false : true;

                    ModelState.Clear();

                    string encrptedBill_Id = string.Empty;

                    //if epayment number allready exist (may happen due to simultanious operation)
                    if (Bill_ID == -2)
                    {
                        return Json(new { Success = false, Bill_ID = -2, Master_Head = Master_Head, deductionRequired = deductionRequired, PaymentRequired = PaymentRequired });

                    }
                    else if (Bill_ID == -3)
                    {
                        return Json(new { Success = false, Bill_ID = -3, Master_Head = Master_Head, deductionRequired = deductionRequired, PaymentRequired = PaymentRequired });

                    }
                    else if (Bill_ID == -123)
                    {
                        return Json(new { Success = false, Bill_ID = -123 });
                    }
                    //else if (Bill_ID == -999)//Added By Abhishek kamble 9Mar2015 for to check chq book issue date validation
                    //{
                    //    return Json(new { Success = false, Bill_ID = -999,Message="Voucher date : "+model.BILL_DATE+" should be greater than or euqal to Cheque book issue date."});
                    //}
                    else
                    {
                        encrptedBill_Id = URLEncrypt.EncryptParameters(new string[] { Bill_ID.ToString() });
                        //return Json(new { Success = true, Bill_ID = encrptedBill_Id, Master_Head = Master_Head, deductionRequired = deductionRequired, PaymentRequired = PaymentRequired });
                        return Json(new { Success = true, Bill_ID = encrptedBill_Id, Master_Head = Master_Head, deductionRequired = deductionRequired, PaymentRequired = PaymentRequired, CHQ_AMOUNT = model.CHQ_AMOUNT });

                    }


                    #endregion valid model
                }
                else
                {
                    #region prepare view when model is invalid
                    int txnid = objparams.TXN_ID;

                    objparams.TXN_ID = 0;

                    objparams.OP_MODE = "A";

                    List<SelectListItem> CONC_Account_ID = new List<SelectListItem>();
                    CONC_Account_ID.Add(new SelectListItem { Text = "--Select Account--", Value = "0", Selected = true });
                    model.CONC_Account_ID1 = CONC_Account_ID;

                    List<SelectListItem> transactionType = common.PopulateTransactions(objparams);
                    model.TXN_ID = model.TXN_ID;
                    model.txn_ID1 = transactionType;

                    List<SelectListItem> monthList = common.PopulateMonths(model.BILL_MONTH);
                    model.BILL_MONTH = model.BILL_MONTH;
                    model.BILL_MONTH_List = monthList;

                    List<SelectListItem> yearList = common.PopulateYears(model.BILL_YEAR);
                    model.BILL_YEAR = model.BILL_YEAR;
                    model.BILL_YEAR_List = yearList;

                    model.CHQ_Book_ID_List = objPaymentBAL.GetchequebookSeries(PMGSYSession.Current.AdminNdCode, PMGSYSession.Current.FundType, PMGSYSession.Current.LevelId);
                    model.CHQ_Book_ID = model.CHQ_Book_ID;

                    List<SelectListItem> ContractorList = common.PopulateContractorSupplier(objparams);
                    model.mast_CON_ID_C1 = ContractorList;

                    objparams.MAST_CON_SUP_FLAG = "S";
                    List<SelectListItem> SupplierList = common.PopulateContractorSupplier(objparams);
                    model.mast_CON_ID_S1 = SupplierList;

                    List<SelectListItem> RemitanceDepartment = common.PopulateContractorSupplier(objparams);
                    RemitanceDepartment = common.PopulateRemittanceDepartment();
                    model.dept_ID1 = RemitanceDepartment;

                    model.CHQ_EPAY = model.CHQ_EPAY;

                    //if epayment with remittance its eremittance lease selected remittance, used to show correct radio button selected on error
                    ViewBag.SELECTED_CHQ_EPAY_ON_ERROR = model.CHQ_EPAY == "E" && model.IS_EREMIT ? "R" : model.CHQ_EPAY;

                    //ViewBag.availableCheques = objPaymentBAL.GetAllAvailableChequesArray(-1, objparams.ADMIN_ND_CODE, PMGSYSession.Current.FundType);

                    ViewBag.LevelID = PMGSYSession.Current.LevelId;

                    //Added by abhi 6Apr2015 for Advice No start
                    if (IsAdvicePayment)
                    {
                        model.CHQ_EPAY = "A";
                        ViewBag.SELECTED_CHQ_EPAY_ON_ERROR = "A";
                    }
                    //Added by abhi 6Apr2015 for Advice No end

                    return View("AddEditMasterPayment", model);
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
        /// funtion to edit the master payment details
        /// </summary>
        /// <param name="model"></param>
        /// <param name="parameter"></param>
        /// <param name="hash"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        [HttpPost]
        [Audit]
        [ValidateAntiForgeryToken]
        public ActionResult EditMasterPaymentDetails(PaymentMasterModel model, String parameter, String hash, String key)
        {
            try
            {

                TransactionParams objparams = new TransactionParams();
                CommonFunctions objCommon = new CommonFunctions();
                objparams.BILL_TYPE = "P";

                objparams.ADMIN_ND_CODE = PMGSYSession.Current.AdminNdCode;

                objparams.DISTRICT_CODE = PMGSYSession.Current.DistrictCode;

                objparams.FUND_TYPE = PMGSYSession.Current.FundType;

                objparams.MONTH = Convert.ToInt16(model.BILL_MONTH);

                objparams.YEAR = Convert.ToInt16(model.BILL_YEAR);

                objparams.LVL_ID = PMGSYSession.Current.LevelId;

                //change done by Vikram

                DateTime billDate = objCommon.GetStringToDateTime(model.BILL_DATE);
                if (billDate != null)
                {
                    model.BILL_MONTH = (Int16)billDate.Month;
                    model.BILL_YEAR = (Int16)billDate.Year;
                }

                //end of change

                ACC_SCREEN_DESIGN_PARAM_MASTER obj = new ACC_SCREEN_DESIGN_PARAM_MASTER();

                Int64 Bill_ID = 0;

                if (!String.IsNullOrEmpty(parameter) && !String.IsNullOrEmpty(hash) && !String.IsNullOrEmpty(key))
                {
                    String[] urlParams = URLEncrypt.DecryptParameters(new String[] { parameter, hash, key });
                    if (urlParams.Length >= 1)
                    {

                        Bill_ID = Convert.ToInt64(urlParams[0]);
                    }
                }

                #region server validation

                //for the validation based on master design params we have to set properties of elemetns 
                //we require txnid so validate it first 

                //check if opening balance entry is finalized
                /*   bool oBFinalized = common.CheckIfOBFinalized(objparams);

                   if (!oBFinalized)
                   {
                       return Json(new { Success = false, Bill_ID = -4 });
                   }
                */
                //bill date should be greater than opening balance date
                DateTime? openingBalanceDate = common.GetOBDate(objparams);

                if (openingBalanceDate.HasValue)
                {
                    if (common.GetStringToDateTime(model.BILL_DATE) < openingBalanceDate.Value)
                    {
                        ModelState.AddModelError("BILL_DATE", "Voucher Date must be greater than or equal to opening balance date");
                    }
                }

                if (model.BILL_MONTH != 0 && model.BILL_MONTH < 13 && model.BILL_YEAR != 0)
                {
                    string monthlyClosingStatus = string.Empty;

                    String errMessage = string.Empty;
                    monthlyClosingStatus = objCommon.MonthlyClosingValidation(model.BILL_MONTH, model.BILL_YEAR, objparams.FUND_TYPE, objparams.LVL_ID, PMGSYSession.Current.AdminNdCode, ref errMessage);

                    if (monthlyClosingStatus.Equals("-111"))
                    {
                        ModelState.AddModelError("BILL_MONTH", errMessage);
                    }
                    if (monthlyClosingStatus.Equals("-222"))
                    {
                        ModelState.AddModelError("BILL_MONTH", "Month is already closed.");
                    }
                }

                //Added By Abhishek kamble for Advice No 6Apr2015 start
                bool IsAdvicePayment = false;
                if (model.CHQ_EPAY == "A")
                {
                    IsAdvicePayment = true;
                    model.CHQ_EPAY = "Q";
                }
                //Added By Abhishek kamble for Advice No 6Apr2015 end

                if (model.CHQ_EPAY != "Q" && model.CHQ_EPAY != "E" && model.CHQ_EPAY != "C" && model.CHQ_EPAY != "R")
                {
                    ModelState.AddModelError("CHQ_EPAY", "Invalid Mode of Transaction");
                }

                //if Remittance set the  model.CHQ_EPAY  as epayment and  Model.IS_EREMIT to true to indicate eremitance
                if (model.CHQ_EPAY == "R")
                {
                    model.CHQ_EPAY = "E";
                    model.IS_EREMIT = true;
                }



                ACC_BILL_MASTER masterDetails = new ACC_BILL_MASTER();
                masterDetails = objPaymentBAL.GetMasterPaymentDetails(Bill_ID);

                objparams.TXN_ID = Convert.ToInt16(masterDetails.TXN_ID);

                obj = common.getMasterDesignParam(objparams);

                if (obj.MAST_CON_REQ.Trim() == "Y")
                {
                    model.MAST_CON_REQ = true;

                    if (model.MAST_CON_ID_C == null || model.MAST_CON_ID_C == 0)
                    {
                        ModelState.AddModelError("MAST_CON_ID_C", "Company Name is Required");
                    }
                    if (model.PAYEE_NAME_C == null || model.PAYEE_NAME_C == string.Empty)
                    {
                        ModelState.AddModelError("PAYEE_NAME_C", "Payee Name is Required");
                    }
                    if (model.CONC_Account_ID == null | model.CONC_Account_ID == 0)
                    {
                        ModelState.AddModelError("CONC_Account_ID", "Select Contractor Account details");
                    }

                }
                else
                {

                    if (ModelState.ContainsKey("MAST_CON_ID_C"))
                        ModelState["MAST_CON_ID_C"].Errors.Clear();
                    if (ModelState.ContainsKey("PAYEE_NAME_C"))
                        ModelState["PAYEE_NAME_C"].Errors.Clear();
                }


                if (obj.MAST_SUPPLIER_REQ.Trim() == "Y")
                {
                    model.MAST_SUPPLIER_REQ = true;
                    if (model.MAST_CON_ID_S == null)
                    {
                        ModelState.AddModelError("MAST_CON_ID_S", "Company Name is Required");
                    }
                    if (model.PAYEE_NAME_S == null || model.PAYEE_NAME_S == string.Empty)
                    {
                        ModelState.AddModelError("PAYEE_NAME_S", "Payee Name is Required");
                    }
                }
                else
                {
                    if (ModelState.ContainsKey("MAST_CON_ID_S"))
                        ModelState["MAST_CON_ID_S"].Errors.Clear();
                    if (ModelState.ContainsKey("PAYEE_NAME_S"))
                        ModelState["PAYEE_NAME_S"].Errors.Clear();

                }

                if (obj.REM_REQ.Trim() == "Y")
                {
                    model.REM_REQ = true;

                    if (model.DEPT_ID == null)
                    {
                        ModelState.AddModelError("DEPT_ID", "Department Name is Required");
                    }
                    //if (model.CHALAN_DATE == null || model.CHALAN_DATE == string.Empty)
                    //{
                    //    ModelState.AddModelError("CHALAN_DATE", "Chalan Date is Required");
                    //}
                    //if (model.CHALAN_NO == null || model.CHALAN_NO == string.Empty)
                    //{
                    //    ModelState.AddModelError("CHALAN_NO", "Chalan Number is Required");
                    //}

                }
                else
                {
                    if (ModelState.ContainsKey("DEPT_ID"))
                        ModelState["DEPT_ID"].Errors.Clear();
                    if (ModelState.ContainsKey("CHALAN_DATE"))
                        ModelState["CHALAN_DATE"].Errors.Clear();
                    if (ModelState.ContainsKey("CHALAN_NO"))
                        ModelState["CHALAN_NO"].Errors.Clear();


                }

                if (model.CHQ_EPAY != "Q" && model.CHQ_EPAY != "E" && model.CHQ_EPAY != "C")
                {
                    ModelState.AddModelError("CHQ_EPAY", "Invalid Mode of Transaction");
                }


                //if Epayment: Bill Date and epay date must be equal to current date 
                if (model.CHQ_EPAY.Trim().Equals("E"))
                {
                    DateTime bill_Date = common.GetStringToDateTime(model.BILL_DATE);
                    DateTime chq_Date = common.GetStringToDateTime(model.CHQ_DATE);

                    if (bill_Date != DateTime.Now.Date)
                    {
                        ModelState.AddModelError("BILL_DATE", "Voucher Date must be equal to todays date ");
                    }

                    if (chq_Date != DateTime.Now.Date)
                    {
                        ModelState.AddModelError("CHQ_DATE", "Epayment Date must be equal to todays date ");
                    }

                }


                if (obj.CASH_CHQ.Trim() == "C" && model.CHQ_EPAY == "C")
                {
                    model.CASH_REQ = true;
                    if (model.CASH_AMOUNT == null)
                    {
                        ModelState.AddModelError("CASH_AMOUNT", "Cash Amount is Required");
                    }

                }
                else
                {
                    if (ModelState.ContainsKey("CASH_AMOUNT"))
                        ModelState["CASH_AMOUNT"].Errors.Clear();

                }

                if (obj.CASH_CHQ.Trim() == "Q" && model.CHQ_EPAY == "Q")
                {
                    model.CHQ_REQ = true;

                    //If Condition Added By Abhishek kamble 14-Apr-2014
                    //&& (objparams.TXN_ID != 47) && (objparams.TXN_ID != 737)
                    if ((objparams.TXN_ID != 137) && (objparams.TXN_ID != 834) && (objparams.TXN_ID != 469) && (model.CHQ_AMOUNT != 0))
                    {
                        if (model.CHQ_DATE == null || model.CHQ_DATE == string.Empty)
                        {
                            ModelState.AddModelError("CHQ_DATE", "Cheque/Epay Date is Required");
                        }
                    }
                    if (model.CHQ_AMOUNT == null)
                    {
                        ModelState.AddModelError("CHQ_AMOUNT", "Cheque/Epay Amount is Required");
                    }
                    //If Condition Added By Abhishek kamble 14-Apr-2014
                    //&& (objparams.TXN_ID != 47) && (objparams.TXN_ID != 737)
                    if ((objparams.TXN_ID != 137) && (objparams.TXN_ID != 834) && (objparams.TXN_ID != 469) && (model.CHQ_AMOUNT != 0))
                    {
                        if (model.CHQ_NO == null)
                        {
                            ModelState.AddModelError("CHQ_NO", "Cheque Number is Required");
                        }
                    }

                    //check for valid cheque book number only if DPIU
                    if (PMGSYSession.Current.LevelId == 5)
                    {

                        //If Condition Added By Abhishek kamble 6-Apr-2015 for Advice No
                        if (IsAdvicePayment)
                        {

                            //if (objPaymentBAL.ValidateAdviceNoExist(model.CHQ_NO, Bill_ID))
                            //{
                            //    ModelState.AddModelError("CHQ_NO", "Advice No is already exist.");
                            //    model.CHQ_EPAY = "A";
                            //}
                        }
                        else
                        {

                            if (model.CHQ_Book_ID == null)
                            {
                                ModelState.AddModelError("CHQ_Book_ID", "Cheque Series is Required");
                            }
                            else
                            {
                                int chq_id = 0;
                                if (model.CHQ_Book_ID.HasValue)
                                {
                                    chq_id = model.CHQ_Book_ID.Value;
                                }


                                //If Condition Added By Abhishek kamble 14-Apr-2014
                                //&& (objparams.TXN_ID != 47) && (objparams.TXN_ID != 737)
                                if ((objparams.TXN_ID != 137) && (objparams.TXN_ID != 834) && (objparams.TXN_ID != 469) && (model.CHQ_AMOUNT != 0))
                                {
                                    String[] availableCheques = objPaymentBAL.GetAllAvailableChequesArray(chq_id, PMGSYSession.Current.AdminNdCode, PMGSYSession.Current.FundType, "E", Bill_ID);
                                    Array.Resize(ref availableCheques, (availableCheques.Length + 1));

                                    if (Array.IndexOf(availableCheques, masterDetails.CHQ_NO) < 0)
                                    {
                                        availableCheques[availableCheques.Length - 1] = masterDetails.CHQ_NO;
                                    }
                                    int pos = Array.IndexOf(availableCheques, model.CHQ_NO);
                                    if (pos == -1)
                                    {
                                        ModelState.AddModelError("CHQ_NO", "Invalid Cheque Number");
                                    }
                                    else
                                    {
                                        string data = objPaymentBAL.GetChequeBookSeriesBasedOnCheque(PMGSYSession.Current.AdminNdCode, PMGSYSession.Current.FundType, Convert.ToInt32(model.CHQ_NO));

                                        int chqBook_id = Convert.ToInt32(data.Split('$')[1]);
                                        if (chq_id != chqBook_id)
                                        {
                                            ModelState.AddModelError("CHQ_NO", "Invalid Cheque Number");
                                        }

                                    }
                                }
                            }
                        }
                    }

                    //at srrda LEVEL  check if cheque book is selected 

                    if (PMGSYSession.Current.LevelId == 4 && model.CHQ_Book_ID != 0)
                    {

                        //If Condition Added By Abhishek kamble 6-Apr-2015 for Advice No
                        if (IsAdvicePayment)
                        {

                            //if (objPaymentBAL.ValidateAdviceNoExist(model.CHQ_NO, Bill_ID))
                            //{
                            //    ModelState.AddModelError("CHQ_NO", "Advice No is already exist.");
                            //    model.CHQ_EPAY = "A";
                            //}
                        }
                        else
                        {

                            //if cheque book id is selected check if entered cheque is in selected series

                            //If Condition Added By Abhishek kamble 14-Apr-2014
                            //&& (objparams.TXN_ID != 47) && (objparams.TXN_ID != 737)
                            if ((objparams.TXN_ID != 137) && (objparams.TXN_ID != 834) && (objparams.TXN_ID != 469) && (model.CHQ_AMOUNT != 0))
                            {
                                //String[] availableCheques = objPaymentBAL.GetAllAvailableChequesArray(model.CHQ_Book_ID.Value, PMGSYSession.Current.AdminNdCode, PMGSYSession.Current.FundType);

                                String[] availableCheques = objPaymentBAL.GetAllAvailableChequesArray(model.CHQ_Book_ID.Value, PMGSYSession.Current.AdminNdCode, PMGSYSession.Current.FundType, "E", Bill_ID);


                                int pos = Array.IndexOf(availableCheques, model.CHQ_NO);
                                if (pos == -1)
                                {
                                    //If Condition Added By Abhishek kamble 14-Apr-2014
                                    //&& (objparams.TXN_ID != 47) && (objparams.TXN_ID != 737)
                                    if ((objparams.TXN_ID != 137) && (objparams.TXN_ID != 834) && (objparams.TXN_ID != 469) && (model.CHQ_AMOUNT != 0))
                                    {
                                        ModelState.AddModelError("CHQ_NO", "Invalid Cheque Number");
                                    }
                                }
                                else
                                {

                                    //If Condition Added By Abhishek kamble 14-Apr-2014
                                    //&& (objparams.TXN_ID != 47) && (objparams.TXN_ID != 737)
                                    if ((objparams.TXN_ID != 137) && (objparams.TXN_ID != 834) && (objparams.TXN_ID != 469))
                                    {
                                        if ((objparams.TXN_ID == 47) || (objparams.TXN_ID == 737))
                                        {
                                            if (model.CHQ_AMOUNT != 0)
                                            {
                                                string data = objPaymentBAL.GetChequeBookSeriesBasedOnCheque(PMGSYSession.Current.AdminNdCode, PMGSYSession.Current.FundType, Convert.ToInt32(model.CHQ_NO));
                                                int chqBook_id = Convert.ToInt32(data.Split('$')[1]);
                                                if (model.CHQ_Book_ID != chqBook_id)
                                                {
                                                    ModelState.AddModelError("CHQ_NO", "Invalid Cheque Number");
                                                }
                                            }
                                        }
                                        else
                                        {
                                            string data = objPaymentBAL.GetChequeBookSeriesBasedOnCheque(PMGSYSession.Current.AdminNdCode, PMGSYSession.Current.FundType, Convert.ToInt32(model.CHQ_NO));
                                            int chqBook_id = Convert.ToInt32(data.Split('$')[1]);
                                            if (model.CHQ_Book_ID != chqBook_id)
                                            {
                                                ModelState.AddModelError("CHQ_NO", "Invalid Cheque Number");
                                            }
                                        }

                                    }



                                }//end of else
                            }
                        }
                    }

                    //@SRRDA chequ book not selected 
                    //Validation commented by Abhisehk kamble 6 Nov 2014 because 'This Cheque is already issued' validation not required at SRRDA level
                    //if (PMGSYSession.Current.LevelId == 4 && model.CHQ_Book_ID == 0)
                    //{
                    //    //If Condition Added By Abhishek kamble 14-Apr-2014
                    //    //&& (objparams.TXN_ID != 47) && (objparams.TXN_ID != 737)
                    //    if ((objparams.TXN_ID != 137) && (objparams.TXN_ID != 834) && (objparams.TXN_ID != 469) && (model.CHQ_AMOUNT != 0))
                    //    {
                    //        //check if chequ already entered
                    //        bool chequeAllreadyIssued = false;


                    //        chequeAllreadyIssued = objPaymentBAL.IschequeIssued(model.CHQ_NO, "E", Bill_ID);

                    //        if (chequeAllreadyIssued)
                    //        {
                    //            //If Condition Added By Abhishek kamble 14-Apr-2014
                    //            //&& (objparams.TXN_ID != 47) && (objparams.TXN_ID != 737)
                    //            if ((objparams.TXN_ID != 137) && (objparams.TXN_ID != 834) && (objparams.TXN_ID != 469) && (model.CHQ_AMOUNT != 0))
                    //            {
                    //                ModelState.AddModelError("CHQ_NO", "This Cheque is already issued");
                    //            }
                    //        }
                    //    }
                    //}

                }
                else
                {
                    if (ModelState.ContainsKey("CHQ_DATE"))
                        ModelState["CHQ_DATE"].Errors.Clear();
                    if (ModelState.ContainsKey("CHQ_AMOUNT"))
                        ModelState["CHQ_AMOUNT"].Errors.Clear();
                    if (ModelState.ContainsKey("CHQ_NO"))
                        ModelState["CHQ_NO"].Errors.Clear();
                    if (ModelState.ContainsKey("CHQ_Book_ID"))
                        ModelState["CHQ_Book_ID"].Errors.Clear();

                }

                if (obj.DED_REQ.Trim() == "Y")
                {
                    model.DED_REQ = true;
                    if (model.DEDUCTION_AMOUNT == null)
                    {
                        ModelState.AddModelError("DEDUCTION_AMOUNT", "Deduction Amount is Required");
                    }
                }
                else
                {
                    if (ModelState.ContainsKey("DEDUCTION_AMOUNT"))
                        ModelState["DEDUCTION_AMOUNT"].Errors.Clear();
                }

                //if payment is only for deduction 
                if (obj.DED_REQ.Trim() == "B" && model.CHQ_EPAY == "C")
                {
                    model.DED_REQ = true;
                    if (model.DEDUCTION_AMOUNT == null)
                    {
                        ModelState.AddModelError("DEDUCTION_AMOUNT", "Deduction Amount is Required");
                    }
                }
                else
                {
                    if (ModelState.ContainsKey("DEDUCTION_AMOUNT"))
                        ModelState["DEDUCTION_AMOUNT"].Errors.Clear();
                }


                if (obj.EPAY_REQ == "Y" && model.CHQ_EPAY == "E")
                {
                    model.EPAY_REQ = true;

                    if (model.EPAY_NO == null || model.EPAY_NO == string.Empty)
                    {
                        ModelState.AddModelError("EPAY_NO", "Epayment Number is Required");
                    }
                }
                else
                {
                    if (ModelState.ContainsKey("EPAY_REQ"))
                        ModelState["EPAY_REQ"].Errors.Clear();
                }



                if (obj.DED_REQ == "B" && model.CHQ_EPAY == "C")
                {
                    //if transaction is of cash and deduction required status is B its deduction only payment no need to check for amount balances
                }
                else
                {
                    //Commented on 05-05-2023
                    //UDF_ACC_GEN_GET_BA_CASH_CLOSING_BALANCES_Result balance = new UDF_ACC_GEN_GET_BA_CASH_CLOSING_BALANCES_Result();

                    //Added on 05-05-2023
                    UDF_ACC_GEN_GET_BA_CASH_CLOSING_BALANCES_COMPOSITE_Result balance = new UDF_ACC_GEN_GET_BA_CASH_CLOSING_BALANCES_COMPOSITE_Result();

                    balance = objPaymentBAL.GetCloSingBalanceForPayment(objparams);

                    //Cheque amount should be less than equal to (Bank authorization -old chq amount) for cheque payment 
                    if (model.CHQ_EPAY == "E" || model.CHQ_EPAY == "Q")
                    {
                        if (model.CHQ_AMOUNT != null && model.CHQ_AMOUNT != 0)
                        {

                            //Below Code commented on 05-05-2023
                            //Decimal TotalAmount = balance.bank_auth + masterDetails.CHQ_AMOUNT;
                            Decimal TotalAmount = 0;

                            //Below Code Added on 05-05-2023
                            if (objparams.TXN_ID == 3185 || objparams.TXN_ID == 3173)
                            {
                                TotalAmount = balance.Bank_Auth_Holding + masterDetails.CHQ_AMOUNT;
                            }
                            else if (objparams.TXN_ID == 3187)
                            {
                                TotalAmount = balance.Bank_Auth_Security + masterDetails.CHQ_AMOUNT;
                            }
                            else
                            {
                                TotalAmount = balance.bank_auth + masterDetails.CHQ_AMOUNT;
                            }

                            if (masterDetails.CHQ_AMOUNT >= model.CHQ_AMOUNT.Value)
                            {
                                //its deduction only dont do anything 
                            }
                            else
                            {
                                if (TotalAmount < model.CHQ_AMOUNT.Value)
                                {
                                    if (objparams.TXN_ID == 3185 || objparams.TXN_ID == 3173)
                                    {
                                        ModelState.AddModelError("AMOUNT_Q", "Amount must be less than or equal to Holding Account Bank Authorization balance amount");

                                    }
                                    else if (objparams.TXN_ID == 3187)
                                    {
                                        ModelState.AddModelError("AMOUNT_Q", "Amount must be less than or equal to Security Deposit Account Bank Authorization balance amount");

                                    }
                                    else
                                    {
                                        ModelState.AddModelError("AMOUNT_Q", "Amount must be less than or equal to Bank Authorization balance amount");

                                    }
                                }
                                else if (TotalAmount >= model.CHQ_AMOUNT.Value)
                                {
                                }
                                else
                                {
                                    if (objparams.TXN_ID == 3185 || objparams.TXN_ID == 3173)
                                    {
                                        ModelState.AddModelError("AMOUNT_Q", "Amount must be less than or equal to Holding Account Bank Authorization balance amount");

                                    }
                                    else if (objparams.TXN_ID == 3187)
                                    {
                                        ModelState.AddModelError("AMOUNT_Q", "Amount must be less than or equal to Security Deposit Account Bank Authorization balance amount");

                                    }
                                    else
                                    {
                                        ModelState.AddModelError("AMOUNT_Q", "Amount must be less than or equal to Bank Authorization balance amount");

                                    }
                                    //ModelState.AddModelError("CHQ_AMOUNT", "Amount must be less than or equal to Bank Authorization balance amount");
                                }

                            }
                        }

                    }

                    if (obj.DED_REQ != "B" && model.CHQ_EPAY == "C")
                    {
                        //cash amount   should be less than equal to difrence amount for cash payment 
                        if (model.CASH_AMOUNT != null && model.CASH_AMOUNT != 0)
                        {

                            Decimal TotalAmount = balance.cash + masterDetails.CASH_AMOUNT;

                            if (masterDetails.CASH_AMOUNT >= model.CASH_AMOUNT.Value)
                            {
                            }
                            else
                            {
                                if (TotalAmount < model.CASH_AMOUNT.Value)
                                {
                                    ModelState.AddModelError("CASH_AMOUNT", "Amount must be less than or equal to Cash balance amount");
                                }
                                else if (TotalAmount >= model.CASH_AMOUNT.Value)
                                {
                                }
                                else
                                {
                                    ModelState.AddModelError("CASH_AMOUNT", "Amount must be less than or equal to Cash balance amount");
                                }

                            }

                        }
                    }

                }

                //in case of edit we dont have to validate the bill number as we are getting it from old entry.
                if (ModelState.ContainsKey("BILL_NO"))
                    ModelState["BILL_NO"].Errors.Clear();

                #endregion

                //Added BY Abhishek kamble 15-Apr-2014
                //|| (model.TXN_ID == "47$Q") && (model.TXN_ID == "737$Q")
                if ((model.TXN_ID == "137$Q") || (model.TXN_ID == "834$Q") || (model.TXN_ID == "469$Q"))
                {

                    if (ModelState.ContainsKey("CHQ_DATE"))
                        ModelState["CHQ_DATE"].Errors.Clear();
                }
                else if ((model.TXN_ID == "47$Q") || (model.TXN_ID == "737$Q"))
                {
                    if (model.CHQ_AMOUNT == 0)
                    {
                        if (ModelState.ContainsKey("CHQ_DATE"))
                            ModelState["CHQ_DATE"].Errors.Clear();
                    }
                }

                if (ModelState.IsValid)
                {

                    #region valid model
                    model.ND_CODE = PMGSYSession.Current.AdminNdCode;
                    model.FUND_TYPE = PMGSYSession.Current.FundType;
                    model.LVL_ID = PMGSYSession.Current.LevelId;

                    if (obj.MAST_CON_REQ.Trim() == "Y")
                    {
                        model.PAYEE_NAME = model.PAYEE_NAME_C;
                        model.CON_ID = model.MAST_CON_ID_C;
                    }
                    if (obj.MAST_SUPPLIER_REQ.Trim() == "Y")
                    {
                        model.PAYEE_NAME = model.PAYEE_NAME_S;
                        model.CON_ID = model.MAST_CON_ID_S;
                    }
                    if (obj.REM_REQ.Trim() == "Y")
                    {
                        model.PAYEE_NAME = model.PAYEE_NAME_R;
                        model.REMIT_TYPE = model.DEPT_ID;
                        // model.MAST_CON_ID = model.DEPT_ID;

                    }
                    else if (obj.MAST_CON_REQ.Trim() != "Y" && obj.MAST_SUPPLIER_REQ.Trim() != "Y" && obj.REM_REQ.Trim() != "Y")
                    {
                        model.PAYEE_NAME = model.PAYEE_NAME;
                    }

                    //check for cheque/cash
                    if (obj.CASH_CHQ.Trim() == "C")
                    {
                        model.CHQ_AMOUNT = 0;
                        model.CHQ_DATE = null;
                        model.CHQ_NO = null;
                    }
                    if (obj.CASH_CHQ.Trim() == "Q")
                    {
                        model.CASH_AMOUNT = 0;

                    }
                    if (model.CHQ_EPAY == "C")
                    {
                        model.CHQ_AMOUNT = 0;
                    }

                    if (model.CHQ_EPAY == "Q" || model.CHQ_EPAY == "E")
                    {
                        model.CASH_AMOUNT = 0;
                    }

                    if (obj.DED_REQ.Trim() == "N")
                    {
                        model.DEDUCTION_AMOUNT = 0;
                    }
                    else
                    {
                        //Cheque transaction with deduction put deduction in cash amount
                        if (obj.CASH_CHQ.Trim() == "Q")
                        {
                            model.CASH_AMOUNT = model.DEDUCTION_AMOUNT;
                        }
                    }


                    decimal GrossAmount = 0;

                    if (obj.DED_REQ.Trim() == "B" && model.CHQ_EPAY == "C")
                    {

                        model.CASH_AMOUNT = model.DEDUCTION_AMOUNT;
                        model.CHQ_AMOUNT = 0;
                        GrossAmount = model.CASH_AMOUNT.Value;
                    }
                    else if (obj.DED_REQ.Trim() != "B" && model.CHQ_EPAY == "C")
                    {
                        model.DEDUCTION_AMOUNT = 0;
                        model.CHQ_AMOUNT = 0;
                    }
                    else if (obj.DED_REQ.Trim() == "N" && (model.CHQ_EPAY == "Q"))
                    {
                        model.DEDUCTION_AMOUNT = 0;
                        model.CASH_AMOUNT = 0;

                    }

                    //calculate gross amount 

                    if (model.CHQ_AMOUNT != null)
                    {
                        if (model.CHQ_AMOUNT != 0)
                        {
                            if (model.DEDUCTION_AMOUNT != null)
                            {
                                GrossAmount = model.CHQ_AMOUNT.Value + model.DEDUCTION_AMOUNT.Value;
                            }
                            else
                            {
                                GrossAmount = model.CHQ_AMOUNT.Value;
                            }
                        }
                    }

                    if (obj.DED_REQ.Trim() != "B" && model.CHQ_EPAY == "C")
                    {
                        if (model.CASH_AMOUNT != null)
                        {
                            if (model.CASH_AMOUNT != 0)
                            {
                                if (model.DEDUCTION_AMOUNT != null)
                                {
                                    if (obj.DED_REQ.Trim() == "B" && model.CHQ_EPAY == "C")
                                    {
                                        GrossAmount = model.CASH_AMOUNT.Value;
                                    }
                                    else
                                    {
                                        GrossAmount = model.CASH_AMOUNT.Value + model.DEDUCTION_AMOUNT.Value;
                                    }

                                }
                                else
                                {
                                    GrossAmount = model.CASH_AMOUNT.Value;
                                }
                            }
                        }
                    }


                    if (model.CHQ_EPAY.Trim().Equals("E"))
                    {
                        model.CHQ_NO = model.EPAY_NO;
                    }


                    model.GROSS_AMOUNT = GrossAmount;

                    //Added by abhi 7Apr2015 for Advice No start
                    //if (IsAdvicePayment)
                    //{
                    //    model.CHQ_EPAY = "A";                        
                    //}
                    //Added by abhi 7Apr2015 for Advice No end

                    Bill_ID = objPaymentBAL.AddEditMasterPaymentDetails(model, "E", Bill_ID, IsAdvicePayment);
                    bool Success = false;

                    string encrptedBill_Id = String.Empty;


                    //-1 if allrady finalized -2 if epaymnumber already exist
                    //&& Bill_ID!=-999 added by abhishek kamble 10Mar2015 for Cheque issue date and voucher date comp validation.
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
                        //else if (Bill_ID == -999)   //added by abhishek kamble 10Mar2015 for Cheque issue date and voucher date comp validation.
                        //{
                        //    encrptedBill_Id = "-999";
                        //}
                    }
                    int Master_Head = Convert.ToInt32(model.TXN_ID.Split('$')[0]);

                    bool deductionRequired = model.DEDUCTION_AMOUNT == 0 ? false : true;

                    bool PaymentRequired = model.CHQ_AMOUNT == 0 ? false : true;

                    ModelState.Clear();

                    return Json(new { Success = Success, Bill_ID = encrptedBill_Id, Master_Head = Master_Head, deductionRequired = deductionRequired, PaymentRequired = PaymentRequired, VoucherDate = model.BILL_DATE });

                    #endregion
                }
                else
                {
                    #region prepare view on error

                    if (masterDetails.CHQ_EPAY == "Q")
                    {
                        model.CHQ_Book_ID_List = objPaymentBAL.GetchequebookSeries(PMGSYSession.Current.AdminNdCode, PMGSYSession.Current.FundType, PMGSYSession.Current.LevelId);
                        model.CHQ_Book_ID = masterDetails.CHQ_Book_ID;
                    }
                    else
                    {

                        List<SelectListItem> CHQ_Book_ID = new List<SelectListItem>();
                        CHQ_Book_ID.Add(new SelectListItem { Text = "--Select--", Value = "0", Selected = true });
                        model.CHQ_Book_ID_List = CHQ_Book_ID;
                    }

                    objparams.TXN_ID = 0;

                    objparams.OP_MODE = "A";

                    List<SelectListItem> transactionType = common.PopulateTransactions(objparams);
                    model.TXN_ID = model.TXN_ID;
                    model.txn_ID1 = transactionType;

                    model.CHQ_EPAY = masterDetails.CHQ_EPAY;

                    //if epayment with remittance its eremittance lease selected remittance, used to show correct radio button selected on error
                    ViewBag.SELECTED_CHQ_EPAY_ON_ERROR = model.CHQ_EPAY == "E" && model.IS_EREMIT ? "R" : model.CHQ_EPAY;

                    List<SelectListItem> monthList = common.PopulateMonths(masterDetails.BILL_MONTH);
                    model.BILL_MONTH_List = monthList;
                    model.BILL_MONTH = masterDetails.BILL_MONTH;

                    List<SelectListItem> yearList = common.PopulateYears(masterDetails.BILL_YEAR);
                    model.BILL_YEAR = masterDetails.BILL_YEAR;
                    model.BILL_YEAR_List = yearList;

                    List<SelectListItem> ContractorList = common.PopulateContractorSupplier(objparams);
                    //ViewData["MAST_CON_ID_C"] = ContractorList;
                    model.mast_CON_ID_C1 = ContractorList;

                    objparams.MAST_CON_SUP_FLAG = "S";
                    List<SelectListItem> SupplierList = common.PopulateContractorSupplier(objparams);
                    // ViewData["MAST_CON_ID_S"] = SupplierList;
                    model.mast_CON_ID_S1 = SupplierList;

                    List<SelectListItem> RemitanceDepartment = common.PopulateContractorSupplier(objparams);
                    RemitanceDepartment = common.PopulateRemittanceDepartment();
                    model.dept_ID1 = RemitanceDepartment;

                    ViewBag.operationType = "E";

                    ViewBag.Bill_id = URLEncrypt.EncryptParameters(new string[] { Bill_ID.ToString() });

                    ViewBag.LevelID = PMGSYSession.Current.LevelId;

                    //Added by abhi 6Apr2015 for Advice No start
                    if (IsAdvicePayment)
                    {
                        model.CHQ_EPAY = "A";
                        ViewBag.SELECTED_CHQ_EPAY_ON_ERROR = "A";
                    }
                    //Added by abhi 6Apr2015 for Advice No end
                    List<SelectListItem> conAccountDetails = new List<SelectListItem>();
                    conAccountDetails.Add(new SelectListItem { Text = "---Select Account---", Value = "0" });
                    PMGSYEntities dbContext = new PMGSYEntities();
                    //  MASTER_CONTRACTOR_BANK objBANK = dbContext.MASTER_CONTRACTOR_BANK.FirstOrDefault(s => s.MAST_CON_ID == masterDetails.MAST_CON_ID && s.MAST_ACCOUNT_ID == masterDetails.CON_ACCOUNT_ID);

                    // conAccountDetails = objPaymentBAL.GetContratorBankAccNoAndIFSCcode(Convert.ToInt32(masterDetails.MAST_CON_ID), PMGSYSession.Current.FundType, masterDetails.TXN_ID, ref isPFMSFinalized, true, true);
                    // conAccountDetails.Add(new SelectListItem { Text = objBANK.MAST_BANK_NAME + ":" + objBANK.MAST_IFSC_CODE + ":" + objBANK.MAST_ACCOUNT_NUMBER, Value = objBANK.MAST_ACCOUNT_ID.ToString(), Selected = true });

                    model.CONC_Account_ID1 = conAccountDetails;

                    return View("AddEditMasterPayment", model);
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


        [HttpPost]
        public ActionResult GetChequeBookIssueDate(string id)
        {
            try
            {
                objPaymentBAL = new PaymentBAL();
                if (String.IsNullOrEmpty(id))
                {
                    return Json(new { IssueDate = String.Empty });
                }
                else
                {
                    // Int64 chqBookId = Convert.ToInt64(id);

                    return Json(new { IssueDate = objPaymentBAL.GetChequeBookIssueDate(Convert.ToInt64(id)) });
                }
            }
            catch (Exception)
            {
                throw new Exception("Error While getting Cheque book details..");
            }
        }

        /// <summary>
        /// post  action to save the payment transaction details
        /// </summary>
        /// <param name="model">transaction model to save</param>
        /// <param name="parameter"></param>
        /// <param name="hash"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        [HttpPost]
        [Audit]
        [ValidateAntiForgeryToken]
        public ActionResult AddPaymentTransactionDetails(PaymentDetailsModel model, String parameter, String hash, String key)
        {
            PaymentDAL paymentDAL = new PaymentDAL();
            PMGSYEntities dbContext = new PMGSYEntities();
            ACC_SCREEN_DESIGN_PARAM_DETAILS_CUSTOM parametes = new ACC_SCREEN_DESIGN_PARAM_DETAILS_CUSTOM();
            TransactionParams objparams = new TransactionParams();
            CommonFunctions objCommon = new CommonFunctions();
            Int64 Bill_ID = 0;
            try
            {

              


                //Added by Abhishek kamble 14Aug2015 start

                if (PMGSYSession.Current.FundType == "M")
                {
                    model.FINAL_PAYMENT = null;
                }

                //Added by Abhishek kamble 14Aug2015 end

                if (!String.IsNullOrEmpty(parameter) && !String.IsNullOrEmpty(hash) && !String.IsNullOrEmpty(key))
                {
                    String[] urlParams = URLEncrypt.DecryptParameters(new String[] { parameter, hash, key });
                    if (urlParams.Length >= 1)
                    {

                        Bill_ID = Convert.ToInt64(urlParams[0]);
                    }
                }

                if (model.HEAD_ID_P != null)
                {
                    //if (Convert.ToInt32(model.HEAD_ID_P.Split('$')[0]) == 48 || Convert.ToInt32(model.HEAD_ID_P.Split('$')[0]) == 49)
                    ///Changed by SAMMED A. PATIL on 20FEB2018 for RCPLWE upgradation check as per account head 
                    if (Convert.ToInt32(model.HEAD_ID_P.Split('$')[0]) == 48 || Convert.ToInt32(model.HEAD_ID_P.Split('$')[0]) == 49 || Convert.ToInt32(model.HEAD_ID_P.Split('$')[0]) == 1789 || Convert.ToInt32(model.HEAD_ID_P.Split('$')[0]) == 1790)
                    {
                        if (model.IMS_PR_ROAD_CODE != null)
                        {
                            if (!ValidateRoad(Convert.ToInt32(model.HEAD_ID_P.Split('$')[0]), model.IMS_PR_ROAD_CODE.Value, PMGSYSession.Current.FundType))
                            {
                                return Json(new { Success = false, status = "-555", message = "Select Upgrade/New road as per account head." });
                            }
                        }
                    }
                    //New validation to validate PMGSY scheme Roads based on Head. start 1 July 2021
                    if (PMGSYSession.Current.FundType == "P" && model.IMS_PR_ROAD_CODE != null && model.IMS_PR_ROAD_CODE != 0)
                    {
                        if (dbContext.VERIFY_ROAD_AMOUNT.Where(x => x.IMS_PR_ROAD_CODE == model.IMS_PR_ROAD_CODE && x.TO_VERIFY == "Y").Any())
                        {

                            //var expAmt = dbContext.UDF_GET_ROAD_EXP_AMOUNT(model.IMS_PR_ROAD_CODE.Value).FirstOrDefault();
                            var expAmt = dbContext.UDF_GET_ROAD_EXP_AMOUNT(model.IMS_PR_ROAD_CODE.Value).FirstOrDefault();
                            var sancAmt = dbContext.UDF_ROAD_SANCTION_WORK_COST(model.IMS_PR_ROAD_CODE.Value).Select(x => x.TOTAL_COST).FirstOrDefault();
                            if (expAmt.Amt > sancAmt)
                            {
                                return Json(new { Success = false, status = "-990", message = "Expenditure amount can not be greater than sanctioned cost." });
                            }
                        }
                    }
                    //validation end

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

                //Added on 14-09-2022 to Verify QCR details against Road
                ActionResult result = GetQCRRecordStatus(parameter, hash, key, model.IMS_PR_ROAD_CODE.ToString());

                if ((!(((JsonResult)result).Data.Equals("")) &&  ((JsonResult)result).Data.Equals("status").Equals(false)))
                {
                    return Json(new { Success = false, status = "-111", message = "QCR details not uploaded against selected road .Hence Payment cannot be done." });
                }

                //Added on 26-08-2023 to Verify VTS validation against Road

                #region VTS validation
                ActionResult isVTSValidate = GetVTSValidationStatus(parameter, hash, key, model.IMS_PR_ROAD_CODE.ToString());


                if (!((JsonResult)isVTSValidate).Data.Equals("status").Equals(false))
                {
                    return Json(new { Success = false, status = "-222", message = "Work freeze due to GPS/VTS Analysis not uploaded." });
                }
                #endregion

                //get the contractor details from master details 
                ACC_BILL_MASTER masterDetails = new ACC_BILL_MASTER();

                masterDetails = objPaymentBAL.GetMasterPaymentDetails(Bill_ID);

                //modified by Abhishek kamble for Advice No 8Apr2015 start
                if (masterDetails.CHQ_EPAY == "A")
                {
                    masterDetails.CHQ_EPAY = "Q";
                }
                //modified by Abhishek kamble for Advice No 8Apr2015 end



                if (PMGSYSession.Current.FundType == "P")
                {
                    if (!(masterDetails.TXN_ID == 86))
                    {
                        if (model.IMS_PR_ROAD_CODE != null && model.IMS_AGREEMENT_CODE_C != null)
                        {
                            if (model.IMS_PR_ROAD_CODE > 0 && model.IMS_AGREEMENT_CODE_C > 0)
                            {
                                IMS_SANCTIONED_PROJECTS masterModel = dbContext.IMS_SANCTIONED_PROJECTS.Where(m => m.IMS_PR_ROAD_CODE == model.IMS_PR_ROAD_CODE).FirstOrDefault();

                                if (masterModel.IMS_ISCOMPLETED.Equals("C") || masterModel.IMS_ISCOMPLETED.Equals("X"))
                                {// Allow
                                }
                                else
                                {
                                    var EstablishmentDate = dbContext.UDF_QM_LAB_EST_DATE(masterModel.IMS_PACKAGE_ID, model.IMS_AGREEMENT_CODE_C).FirstOrDefault();
                                    if (EstablishmentDate == null)
                                    {
                                        return Json(new { Success = false, status = "-777", message = "Lab Establishment Details are not available, Payment can not be made." });
                                    }

                                }
                            }
                        }
                    }
                }


                ACC_SCREEN_DESIGN_PARAM_MASTER obj = new ACC_SCREEN_DESIGN_PARAM_MASTER();
                bool disablePayHead = false;
                bool disblehead = false;

                #region if for master payment multiple transsaction is not allowed

                TransactionParams objparams1 = new TransactionParams();

                objparams1.TXN_ID = masterDetails.TXN_ID;

                obj = common.getMasterDesignParam(objparams1);


                if (obj.MTXN_REQ == "N")
                {
                    if (!dbContext.ACC_BILL_DETAILS.Where(d => d.BILL_ID == masterDetails.BILL_ID).Any())
                    {
                        disablePayHead = false;
                    }
                    else
                    {
                        Int16? txn = dbContext.ACC_BILL_DETAILS.Where(d => d.BILL_ID == masterDetails.BILL_ID).Select(c => c.TXN_ID).First();
                        string CheCash = dbContext.ACC_MASTER_TXN.Where(m => m.TXN_ID == txn).Select(m => m.CASH_CHQ).FirstOrDefault().Trim();
                        model.HEAD_ID_P = txn + "$" + CheCash;
                        disablePayHead = true;
                    }
                }
                //to disable on error in javascript file
                ViewBag.disablePaymentHead = disablePayHead;

                disblehead = disablePayHead;

                #endregion

                if (model.HEAD_ID_P != null && model.HEAD_ID_P != String.Empty)
                {
                    objparams.TXN_ID = Convert.ToInt16(model.HEAD_ID_P.Split('$')[0]);

                    parametes = common.getDetailsDesignParamForPayment(objparams);


                    // to check if correct entry as is operational and is required after porting flag
                    string correctionStatus = objCommon.ValidateHeadForCorrection(objparams.TXN_ID, 0, "A");
                    if (correctionStatus != "1")
                    {
                        ModelState.AddModelError("HEAD_ID_P", "Sub Transaction Type (Payment) is invalid.");
                    }

                }
                else
                {

                    ModelState.AddModelError("HEAD_ID_P", "Invalid Transaction Type(payment)");

                }

                // added on 12-01-2022 by Srishti Tyagi
                // there are few roads which cannot add payment details
                #region Validation for payment for restricted roads

                //Xml code start
                AddXmlFile payment;
                List<AddXmlFile> road = new List<AddXmlFile>();

                if (System.IO.File.Exists(System.Web.HttpContext.Current.Server.MapPath("~/RestrictedRoadCodeForPayment.xml")))
                { // Create a file to write to   
                    XDocument doc = XDocument.Load(System.Web.HttpContext.Current.Server.MapPath("~/RestrictedRoadCodeForPayment.xml"));

                    foreach (XElement element in doc.Descendants("Road")
                        .Descendants("RoadCodeValidation"))
                    {
                        payment = new AddXmlFile();
                        payment.RoadCode = element.Element("Key").Value;
                        payment.Value = Convert.ToInt32(element.Element("Value").Value);
                        road.Add(payment);
                    }
                }
                //xml Code end

                foreach (AddXmlFile item in road)
                {
                    if (model.IMS_PR_ROAD_CODE == item.Value)
                    {
                        return Json(new { Success = false, status = "-9", message = "Transaction cannot be made on selected road." });
                    }
                }

                #endregion

                #region Validation for payment of roads within 6 months of completion
                string paymentValidationDt = ConfigurationManager.AppSettings["PaymentValidationDate"].ToString();
                ///Commented as per instructions from Anita Mam on 07JAN2019
                if (PMGSYSession.Current.FundType == "P" && model.IMS_PR_ROAD_CODE != null && model.IMS_PR_ROAD_CODE != 0 && masterDetails.BILL_TYPE == "P" && masterDetails.TXN_ID != 86
                    //&& PMGSYSession.Current.StateCode != 16 && PMGSYSession.Current.StateCode != 15
                    && (masterDetails.BILL_DATE >= common.GetStringToDateTime(paymentValidationDt))
                    )
                {
                    //Below code is commented on 12-10-2022 to Provide relaxation to brcpwddarbhanga piu.
                    //string[] param = null;
                    //if (!paymentDAL.CompletionDateValidation1(model.IMS_PR_ROAD_CODE.Value, masterDetails.BILL_DATE, ref param))
                    //{
                    //    int diff = Convert.ToInt32((masterDetails.BILL_DATE - objCommon.GetStringToDateTime(param[1])).TotalDays);
                    //    return Json(new { Success = false, status = "-9", message = "Transaction cannot be made on selected " + param[0] + " as completion date [" + param[1] + "] is more than 6 months (180 days). [Difference in days=" + diff + "]" });
                    //}

                    //Below condition is Added on 12-10-2022 to Provide relaxation to brcpwddarbhanga piu.
                    if (!(PMGSYSession.Current.UserId == 1315 && PMGSYSession.Current.StateCode == 5 && DateTime.Now.Date <= new CommonFunctions().GetStringToDateTime("22/10/2022")))
                    {
                        string[] param = null;
                        if (!paymentDAL.CompletionDateValidation1(model.IMS_PR_ROAD_CODE.Value, masterDetails.BILL_DATE, ref param))
                        {
                            int diff = Convert.ToInt32((masterDetails.BILL_DATE - objCommon.GetStringToDateTime(param[1])).TotalDays);
                            return Json(new { Success = false, status = "-9", message = "Transaction cannot be made on selected " + param[0] + " as completion date [" + param[1] + "] is more than 6 months (180 days). [Difference in days=" + diff + "]" });
                        }
                    }
                    
                }

                #endregion

                //if no other model errors are there then only check for dynamic validation
                if (ModelState.IsValid)
                {
                    #region serverside validation for payment

                    //for Remittance Of Statutory Deductions/State Government the contractor is required in bill details table
                    if (parametes.SHOW_CON_AT_TRANSACTION)
                    {
                        if (model.MAST_CON_ID_CON == null || model.MAST_CON_ID_CON == 0)
                        {
                            ModelState.AddModelError("MAST_CON_ID_CON", "Company Name (Contractor) is Required");
                        }

                    }
                    else
                    {
                        if (ModelState.ContainsKey("MAST_CON_ID_CON"))
                            ModelState["MAST_CON_ID_CON"].Errors.Clear();
                    }

                    if (parametes.ROAD_REQ.Trim() == "Y")
                    {

                        if (model.HEAD_ID_P.Equals("90$Q"))
                        {
                            if (ModelState.ContainsKey("IMS_PR_ROAD_CODE"))
                                ModelState["IMS_PR_ROAD_CODE"].Errors.Clear();

                        }
                        else
                        {
                            ///Changes for Supplier Payment Building Proposal
                            if (model.HEAD_ID_P.Equals("1489$Q") || model.HEAD_ID_P.Equals("1490$Q"))
                            {
                                bool flg = common.getContractorSupplierbyBillId(Bill_ID);
                                if (flg)
                                {
                                    model.ROAD_REQ = "N";
                                    parametes.ROAD_REQ = "N";
                                    if (ModelState.ContainsKey("IMS_PR_ROAD_CODE"))
                                        ModelState["IMS_PR_ROAD_CODE"].Errors.Clear();
                                }
                                else
                                {
                                    if (model.IMS_PR_ROAD_CODE == null || model.IMS_PR_ROAD_CODE == 0)
                                    {
                                        ModelState.AddModelError("IMS_PR_ROAD_CODE", "Road is Required");
                                    }
                                }
                            }
                            else if ((parametes.CON_REQ.Trim().Equals("Y")) && (parametes.AGREEMENT_REQ.Trim().Equals("Y")) && (parametes.ROAD_REQ.Trim().Equals("Y")) && (masterDetails.TXN_ID == 86))
                            {
                                string proposalType = (dbContext.TEND_AGREEMENT_MASTER.Where(c => c.MAST_CON_ID == masterDetails.MAST_CON_ID && c.TEND_AGREEMENT_CODE == model.IMS_AGREEMENT_CODE_C).Select(c => c.TEND_AGREEMENT_TYPE).First());
                                if (!(proposalType.Equals("D") || (proposalType.Equals("S") && masterDetails.TXN_ID == 86)))
                                {
                                    if (model.IMS_PR_ROAD_CODE == null || model.IMS_PR_ROAD_CODE == 0)
                                    {
                                        ModelState.AddModelError("IMS_PR_ROAD_CODE", "Road is Required");
                                    }
                                }
                            }
                            else
                            {
                                if (model.IMS_PR_ROAD_CODE == null || model.IMS_PR_ROAD_CODE == 0)
                                {
                                    ModelState.AddModelError("IMS_PR_ROAD_CODE", "Road is Required");
                                }
                            }

                        }

                    }
                    else
                    {
                        if (ModelState.ContainsKey("IMS_PR_ROAD_CODE"))
                            ModelState["IMS_PR_ROAD_CODE"].Errors.Clear();
                    }

                    if (parametes.PIU_REQ.Trim() == "Y")
                    {
                        //commented by Vikram
                        //if (model.PIU_REQ == null || model.PIU_REQ == String.Empty)
                        //{
                        //    ModelState.AddModelError("PIU_REQ", "PIU is Required");
                        //}
                        if (model.MAST_DPIU_CODE == null || model.MAST_DPIU_CODE == 0)
                        {
                            ModelState.AddModelError("MAST_DPIU_CODE", "Implementing Agency / Department Selection is required.");
                        }
                    }
                    else
                    {
                        if (ModelState.ContainsKey("MAST_DPIU_CODE"))
                            ModelState["MAST_DPIU_CODE"].Errors.Clear();
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

                        if (model.IMS_AGREEMENT_CODE_S == null || model.IMS_AGREEMENT_CODE_S == 0)
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
                        ModelState.AddModelError("AMOUNT_Q", " Amount is Required");
                    }
                    else if (model.AMOUNT_Q == 0)
                    {
                        //If Condition Added By Abhishek kamble 16-Apr-2014
                        //if ((objparams1.TXN_ID != 47) && (objparams1.TXN_ID != 737) && (objparams1.TXN_ID != 72) && (objparams1.TXN_ID != 86) && (objparams1.TXN_ID != 777) && (objparams1.TXN_ID != 787) && (objparams1.TXN_ID != 415) && (objparams1.TXN_ID != 314) && (objparams1.TXN_ID != 1484)  )

                        // Changes made by Anita - to remove unnecessary zero entries in bill_details table
                        //Zero amount is allowed only to Contractor work payment
                        if (PMGSYSession.Current.FundType.Equals("P"))
                        {
                            if ((objparams1.TXN_ID != 47) && (objparams1.TXN_ID != 737) && (objparams1.TXN_ID != 1484) && (objparams1.TXN_ID != 1788) && (objparams1.TXN_ID != 1974))//PMGSY3
                            {
                                ModelState.AddModelError("AMOUNT_Q", " Amount must be greater than 0");
                            }
                        }
                    }
                    else if (model.AMOUNT_C != null)
                    {
                        //if (model.AMOUNT_Q < model.AMOUNT_C)
                        //{
                        //    ModelState.AddModelError("AMOUNT_Q", " Amount must be greater than cash amount");
                        //}
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

                    amountModel = objPaymentBAL.CalculatePaymentAmounts(Bill_ID);

                    //cash only transaction Amount Q contains the cash amount
                    if (obj.DED_REQ != "B" && masterDetails.CHQ_EPAY == "C")
                    {

                        //cash amount   should be less than equal to difrence amount for cash payment 
                        if (model.AMOUNT_Q != null && model.AMOUNT_Q != 0)
                        {
                            if (Convert.ToDecimal(amountModel.DiffCachAmount) < Convert.ToDecimal(model.AMOUNT_Q))
                            {
                                ModelState.AddModelError("AMOUNT_Q", " Amount must be less than or equal to Difference To Be Entered for cash Amount");
                            }
                        }

                    }
                    else
                    {
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

                    if (masterDetails.BILL_FINALIZED == "Y")
                    {
                        return Json(new { Success = false, status = "-1" });
                    }

                    //Validation to check Head 55 added by Abhishek kamble 26-Aug-2014 start

                    //**Commented to allow head 55 entry, as per mail received on march 23 2017 by arun with subject -State Administrative Expenses Head to be unlocked in OMMAS 

                    //if ((PMGSYSession.Current.FundType == "A") && (PMGSYSession.Current.LevelId == 5) && (masterDetails.TXN_ID == 415))
                    //{
                    //    int? statusCode = dbContext.USP_ACC_VERIFY_ADMIN_FUND_HEAD_55("P", masterDetails.TXN_ID, objparams.TXN_ID, "D", masterDetails.BILL_DATE).FirstOrDefault();

                    //    if (statusCode == 0)
                    //    {
                    //        return Json(new { Success = false, status = "-55" });
                    //    }

                    //}

                    //**Commented to allow head 55 entry, as per mail received on march 23 2017 by arun with subject -State Administrative Expenses Head to be unlocked in OMMAS 


                    //Validation to check Head 55 added by Abhishek kamble 26-Aug-2014 end  

                    if (parametes.ROAD_REQ.Trim() == "Y")
                    {
                        model.IMS_PR_ROAD_CODE = model.IMS_PR_ROAD_CODE;
                        if (model.IMS_PR_ROAD_CODE == 0)
                        {
                            model.FINAL_PAYMENT = null;
                        }
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

                    if (parametes.AGREEMENT_REQ.Trim() == "Y" && parametes.SUPPLIER_REQ.Trim() == "Y")
                    {
                        model.IMS_AGREEMENT_CODE = model.IMS_AGREEMENT_CODE_S;
                    }

                    // if cheque amount is 0 setdeduction agreement code as agreement code
                    if (model.AMOUNT_Q == 0)
                    {

                        //model.IMS_AGREEMENT_CODE = model.IMS_AGREEMENT_CODE_DED;
                        model.IMS_AGREEMENT_CODE = model.IMS_AGREEMENT_CODE_C;
                    }
                    else
                    {
                        model.IMS_AGREEMENT_CODE_DED = model.IMS_AGREEMENT_CODE;
                    }

                    //for the transaction details admin_nd_code is mast_PIU_Code
                    model.ND_CODE = model.MAST_DPIU_CODE;

                    model.MAST_DISTRICT_CODE = PMGSYSession.Current.DistrictCode;


                    model.CASH_CHQ = masterDetails.CHQ_EPAY;


                    if (parametes.SHOW_CON_AT_TRANSACTION)
                    {
                        model.CON_ID = model.MAST_CON_ID_CON;

                    }
                    else
                    {
                        //for details master contractor id get it from master table (used for opening balance) 
                        model.CON_ID = masterDetails.MAST_CON_ID;
                    }


                    Boolean value = objPaymentBAL.AddEditTransactionDeductionPaymentDetails(model, "T", Bill_ID, "A", 0);

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

                    ViewBag.BillFinalized = masterDetails.BILL_FINALIZED;
                    objparams.BILL_ID = Bill_ID;
                    objparams.BILL_TYPE = masterDetails.BILL_TYPE;
                    objparams.FUND_TYPE = masterDetails.FUND_TYPE;
                    objparams.LVL_ID = masterDetails.LVL_ID;
                    objparams.ADMIN_ND_CODE = masterDetails.ADMIN_ND_CODE;
                    objparams.MAST_CONT_ID = masterDetails.MAST_CON_ID.HasValue ? masterDetails.MAST_CON_ID.Value : 0;
                    objparams.DISTRICT_CODE = PMGSYSession.Current.DistrictCode;
                    objparams.TXN_ID = masterDetails.TXN_ID;
                    objparams.STATE_CODE = Convert.ToInt16(PMGSYSession.Current.StateCode);
                    objparams.DISTRICT_CODE = Convert.ToInt16(PMGSYSession.Current.DistrictCode);
                    objparams.SANC_YEAR = Convert.ToInt16(model.SANCTION_YEAR);
                    objparams.OP_MODE = "A";
                    List<SelectListItem> generalList = new List<SelectListItem>();
                    generalList.Add(new SelectListItem { Selected = true, Text = "--Select--", Value = "" });

                    List<SelectListItem> subHeadList = new List<SelectListItem>();

                    subHeadList = common.PopulateTransactions(objparams);

                    model.HeadId_P = new List<SelectListItem>(subHeadList);

                    //for remittance payment set the payment transaction same as remittance office set in master payment

                    if (masterDetails.REMIT_TYPE != null)
                    {

                        foreach (var item in subHeadList)
                        {
                            if (item.Value != string.Empty)
                            {
                                //get the head id 
                                int head = Convert.ToInt32(item.Value.Split('$')[0]);

                                //get the order of it from master txn table 
                                int orderOfTxn = dbContext.ACC_MASTER_TXN.Where(x => x.TXN_ID == head).Select(x => x.TXN_ORDER).FirstOrDefault();

                                //check if order matches or remove from subtransaction list
                                if (orderOfTxn != masterDetails.REMIT_TYPE)
                                {
                                    model.HeadId_P.Remove(item);
                                }
                            }
                        }
                    }

                    subHeadList.Clear();

                    subHeadList = null;

                    if (parametes.SHOW_CON_AT_TRANSACTION)
                    {
                        objparams.MAST_CON_SUP_FLAG = "C";
                        model.mast_CON_ID_CON1 = common.PopulateContractorSupplier(objparams);

                    }
                    else
                    {
                        model.mast_CON_ID_CON1 = generalList;
                    }


                    if (parametes.ROAD_REQ.Trim() == "Y")
                    {
                        model.IMS_PR_ROAD_CODEList = common.PopulateRoad(objparams);
                    }
                    else
                    {
                        model.IMS_PR_ROAD_CODEList = generalList;
                    }

                    if (parametes.PIU_REQ.Trim() == "Y")
                    {
                        model.MAST_DPIU_CODEList = common.PopulateDPIU(objparams);
                    }
                    else
                    {
                        model.MAST_DPIU_CODEList = generalList;
                    }

                    if (parametes.AGREEMENT_REQ.Trim() == "Y" && parametes.CON_REQ.Trim() == "Y")
                    {
                        model.AGREEMENT_C = common.PopulateAgreement(objparams);
                    }
                    else
                    {
                        model.AGREEMENT_C = generalList;

                    }

                    if (parametes.AGREEMENT_REQ.Trim() == "Y" && parametes.SUPPLIER_REQ.Trim() == "Y")
                    {
                        model.AGREEMENT_S = common.PopulateAgreement(objparams);
                    }
                    else
                    {
                        model.AGREEMENT_S = generalList;
                    }

                    if (parametes.YEAR_REQ.Trim() == "Y")
                    {
                        model.IMS_SANCTION_YEAR_List = common.PopulateSancYear(objparams);
                    }
                    else
                    {
                        generalList.Clear();
                        generalList.Add(new SelectListItem { Selected = true, Text = "--Select--", Value = "0" });
                        model.IMS_SANCTION_YEAR_List = generalList;
                    }
                    if (parametes.PKG_REQ.Trim() == "Y")
                    {
                        model.IMS_SANCTION_PACKAGE_List = common.PopulatePackage(objparams);
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
                        model.AGREEMENT_DED = common.PopulateAgreement(objparams);
                    }
                    else
                    {
                        model.AGREEMENT_DED = generalList;
                    }

                    objparams.BILL_TYPE = "D";

                    model.HeadId_D = common.PopulateTransactions(objparams);

                    if (parametes.AGREEMENT_REQ.Trim() == "Y" && parametes.CON_REQ.Trim() == "Y")
                    {
                        model.IMS_AGREEMENT_CODE = model.IMS_AGREEMENT_CODE_C;
                    }

                    if (parametes.AGREEMENT_REQ.Trim() == "Y" && parametes.SUPPLIER_REQ.Trim() == "Y")
                    {
                        model.IMS_AGREEMENT_CODE = model.IMS_AGREEMENT_CODE_S;
                    }

                    objparams.AGREEMENT_CODE = model.IMS_AGREEMENT_CODE.HasValue ? model.IMS_AGREEMENT_CODE.Value : 0;

                    List<SelectListItem> List = new List<SelectListItem>();

                    List.Add(new SelectListItem { Text = "Yes", Value = "true" });
                    List.Add(new SelectListItem { Selected = true, Text = "No", Value = "false" });

                    model.final_pay = List;

                    ViewBag.disablePaymentHead = disablePayHead;

                    if (obj.DED_REQ == "B")
                    {

                        ViewBag.DeductionScreenRequired = (masterDetails.CASH_AMOUNT != 0 && (masterDetails.CHQ_EPAY == "Q" || masterDetails.CHQ_EPAY == "E")) ? true : masterDetails.CHQ_EPAY == "C" ? true : false;
                        ViewBag.PaymentScreenRequired = masterDetails.CHQ_AMOUNT != 0 && (masterDetails.CHQ_EPAY == "Q" || masterDetails.CHQ_EPAY == "E") ? true : false;
                    }
                    else
                    {
                        ViewBag.PaymentScreenRequired = (masterDetails.CHQ_AMOUNT != 0 && (masterDetails.CHQ_EPAY == "Q" || masterDetails.CHQ_EPAY == "E")) || (masterDetails.CHQ_EPAY == "C") ? true : false;
                        ViewBag.DeductionScreenRequired = masterDetails.CASH_AMOUNT != 0 && (masterDetails.CHQ_EPAY == "Q" || masterDetails.CHQ_EPAY == "E") ? true : false;
                    }

                    ViewBag.Bill_ID = URLEncrypt.EncryptParameters(new string[] { objparams.BILL_ID.ToString() });

                    return View("PartialTransaction", model);

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
        /// action to save the deduction details
        /// </summary>
        /// <param name="model"> transaction model to save</param>
        /// <param name="parameter"></param>
        /// <param name="hash"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        [HttpPost]
        [Audit]
        [ValidateAntiForgeryToken]
        public ActionResult AddDeductionTransactionDetails(PaymentDetailsModel model, String parameter, String hash, String key)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            CommonFunctions objCommon = new CommonFunctions();
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

                        Bill_ID = Convert.ToInt64(urlParams[0]);
                    }
                }



                //get the contractor details from master details 
                ACC_BILL_MASTER masterDetails = new ACC_BILL_MASTER();
                masterDetails = objPaymentBAL.GetMasterPaymentDetails(Bill_ID);

                if (masterDetails.CHQ_EPAY == "A")//Advice no change for 8Apr2015
                {
                    masterDetails.CHQ_EPAY = "Q";
                }

                #region if for master payment multiple transsaction is not allowed

                ACC_SCREEN_DESIGN_PARAM_MASTER obj = new ACC_SCREEN_DESIGN_PARAM_MASTER();

                TransactionParams objparams1 = new TransactionParams();

                objparams1.TXN_ID = masterDetails.TXN_ID;

                obj = common.getMasterDesignParam(objparams1);

                bool disablePayHead = false;

                if (obj.MTXN_REQ == "N")
                {
                    if (!dbContext.ACC_BILL_DETAILS.Where(d => d.BILL_ID == masterDetails.BILL_ID).Any())
                    {
                        disablePayHead = false;
                    }
                    else
                    {
                        Int16? txn = dbContext.ACC_BILL_DETAILS.Where(d => d.BILL_ID == masterDetails.BILL_ID).Select(c => c.TXN_ID).First();
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

                    if (model.HEAD_ID_D == null || model.HEAD_ID_D == String.Empty)
                    {
                        ModelState.AddModelError("HEAD_ID_D", "Sub Transaction Type (Deduction) is required");
                    }

                    else
                    {
                        // to check if correct entry as is operational and is required after porting flag
                        string correctionStatus = objCommon.ValidateHeadForCorrection(Convert.ToInt16(model.HEAD_ID_D.Split('$')[0]), 0, "A");

                        if (correctionStatus != "1")
                        {
                            ModelState.AddModelError("HEAD_ID_D", "Sub Transaction Type (Deduction) is invalid");
                        }
                        else
                        {

                            if (ModelState.ContainsKey("HEAD_ID_D"))
                                ModelState["HEAD_ID_D"].Errors.Clear();
                        }
                    }

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



                    int ContractorSupplierRequired = masterDetails.MAST_CON_ID.HasValue ? masterDetails.MAST_CON_ID.Value : 0;
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

                        amountModel = objPaymentBAL.CalculatePaymentAmounts(Bill_ID);

                        if (Convert.ToDecimal(amountModel.DiffDedAmount) < Convert.ToDecimal(model.AMOUNT_D))
                        {
                            ModelState.AddModelError("AMOUNT_D", "Deduction Amount must be less than or equal to Difference To Be Entered for Deduction Amount");
                        }

                    }


                }
                #endregion

                if (ModelState.IsValid)
                {

                    if (masterDetails.BILL_FINALIZED == "Y")
                    {
                        return Json(new { Success = false, status = "-1" });
                    }

                    //for the transaction details admin_nd_code is mast_PIU_Code
                    model.ND_CODE = model.MAST_DPIU_CODE;

                    model.MAST_DISTRICT_CODE = PMGSYSession.Current.DistrictCode;

                    model.CON_ID = masterDetails.MAST_CON_ID;


                    Boolean value = objPaymentBAL.AddEditTransactionDeductionPaymentDetails(model, "D", Bill_ID, "A", 0);

                    ModelState.Clear();

                    return Json(new { Success = value });

                }
                else
                {
                    ViewBag.BillFinalized = masterDetails.BILL_FINALIZED;
                    objparams.BILL_ID = Bill_ID;
                    objparams.BILL_TYPE = masterDetails.BILL_TYPE;
                    objparams.FUND_TYPE = masterDetails.FUND_TYPE;
                    objparams.LVL_ID = masterDetails.LVL_ID;
                    objparams.ADMIN_ND_CODE = masterDetails.ADMIN_ND_CODE;
                    objparams.MAST_CONT_ID = masterDetails.MAST_CON_ID.HasValue ? masterDetails.MAST_CON_ID.Value : 0;
                    objparams.DISTRICT_CODE = PMGSYSession.Current.DistrictCode;
                    objparams.TXN_ID = masterDetails.TXN_ID;
                    objparams.STATE_CODE = Convert.ToInt16(PMGSYSession.Current.StateCode);
                    objparams.DISTRICT_CODE = Convert.ToInt16(PMGSYSession.Current.DistrictCode);
                    objparams.SANC_YEAR = Convert.ToInt16(model.SANCTION_YEAR);
                    objparams.OP_MODE = "A";
                    model.AGREEMENT_S = generalList;

                    model.AGREEMENT_C = generalList;

                    model.AGREEMENT_DED = common.PopulateAgreement(objparams);

                    model.IMS_PR_ROAD_CODEList = generalList;

                    model.MAST_DPIU_CODEList = generalList;

                    model.mast_CON_ID_CON1 = generalList;

                    generalList.Clear();
                    generalList.Add(new SelectListItem { Selected = true, Text = "--Select--", Value = "0" });

                    model.IMS_SANCTION_YEAR_List = generalList;

                    model.IMS_SANCTION_PACKAGE_List = generalList;

                    List<SelectListItem> subHeadList = new List<SelectListItem>();

                    subHeadList = common.PopulateTransactions(objparams);

                    model.HeadId_P = new List<SelectListItem>(subHeadList);

                    //for remittance payment set the payment transaction same as remittance office set in master payment

                    if (masterDetails.REMIT_TYPE != null)
                    {

                        foreach (var item in subHeadList)
                        {
                            if (item.Value != string.Empty)
                            {
                                //get the head id 
                                int head = Convert.ToInt32(item.Value.Split('$')[0]);

                                //get the order of it from master txn table 
                                int orderOfTxn = dbContext.ACC_MASTER_TXN.Where(x => x.TXN_ID == head).Select(x => x.TXN_ORDER).FirstOrDefault();

                                //check if order matches or remove from subtransaction list
                                if (orderOfTxn != masterDetails.REMIT_TYPE)
                                {
                                    model.HeadId_P.Remove(item);
                                }
                            }
                        }
                    }

                    subHeadList.Clear();

                    subHeadList = null;

                    objparams.BILL_TYPE = "D";

                    model.HeadId_D = common.PopulateTransactions(objparams);

                    objparams.AGREEMENT_CODE = model.IMS_AGREEMENT_CODE.HasValue ? model.IMS_AGREEMENT_CODE.Value : 0;

                    List<SelectListItem> List = new List<SelectListItem>();

                    List.Add(new SelectListItem { Text = "Yes", Value = "true" });
                    List.Add(new SelectListItem { Selected = true, Text = "No", Value = "false" });

                    model.final_pay = List;

                    ViewBag.disablePaymentHead = disablePayHead;
                    //agreement for deduction
                    ViewBag.DeductionAgreementRequired = objparams.MAST_CONT_ID == 0 ? false : true;

                    if (obj.DED_REQ == "B")
                    {

                        ViewBag.DeductionScreenRequired = (masterDetails.CASH_AMOUNT != 0 && (masterDetails.CHQ_EPAY == "Q" || masterDetails.CHQ_EPAY == "E")) ? true : masterDetails.CHQ_EPAY == "C" ? true : false;
                        ViewBag.PaymentScreenRequired = masterDetails.CHQ_AMOUNT != 0 && (masterDetails.CHQ_EPAY == "Q" || masterDetails.CHQ_EPAY == "E") ? true : false;
                    }
                    else
                    {
                        ViewBag.PaymentScreenRequired = (masterDetails.CHQ_AMOUNT != 0 && (masterDetails.CHQ_EPAY == "Q" || masterDetails.CHQ_EPAY == "E")) || (masterDetails.CHQ_EPAY == "C") ? true : false;
                        ViewBag.DeductionScreenRequired = masterDetails.CASH_AMOUNT != 0 && (masterDetails.CHQ_EPAY == "Q" || masterDetails.CHQ_EPAY == "E") ? true : false;
                    }

                    ViewBag.Bill_ID = URLEncrypt.EncryptParameters(new string[] { objparams.BILL_ID.ToString() });

                    return View("PartialTransaction", model);
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
        /// action to edit the transaction master 
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="hash"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        [HttpGet]
        [Audit]
        public ActionResult EditPaymentMasterDetails(String parameter, String hash, String key)
        {
            Int64 Bill_Id = 0;
            PMGSYEntities dbContext = new PMGSYEntities();
            TransactionParams objparams = new TransactionParams();
            PaymentMasterModel model = new PaymentMasterModel();

            string moduleType = "D";


            try
            {
                REAT_OMMAS_PAYMENT_SUCCESS objModuleType = dbContext.REAT_OMMAS_PAYMENT_SUCCESS.FirstOrDefault(s => s.ADMIN_ND_CODE == PMGSYSession.Current.AdminNdCode);
                if (objModuleType != null)
                {
                    moduleType = "R";

                }

                List<SelectListItem> conAccountDetails = new List<SelectListItem>();
                conAccountDetails.Add(new SelectListItem { Text = "---Select Account---", Value = "0" });

                if (!String.IsNullOrEmpty(parameter) && !String.IsNullOrEmpty(hash) && !String.IsNullOrEmpty(key))
                {

                    if (!String.IsNullOrEmpty(parameter) && !String.IsNullOrEmpty(hash) && !String.IsNullOrEmpty(key))
                    {
                        String[] urlParams = URLEncrypt.DecryptParameters(new String[] { parameter, hash, key });
                        if (urlParams.Length >= 1)
                        {
                            Bill_Id = Convert.ToInt64(urlParams[0]);
                        }
                    }
                    else
                    {
                        throw new Exception("Error while getting master payment details..");
                    }
                }

                ACC_BILL_MASTER masterDetails = new ACC_BILL_MASTER();
                masterDetails = objPaymentBAL.GetMasterPaymentDetails(Bill_Id);

                objparams.BILL_TYPE = masterDetails.BILL_TYPE;
                objparams.FUND_TYPE = masterDetails.FUND_TYPE;
                objparams.LVL_ID = masterDetails.LVL_ID;
                objparams.ADMIN_ND_CODE = masterDetails.ADMIN_ND_CODE;
                objparams.DISTRICT_CODE = PMGSYSession.Current.DistrictCode;
                //objparams.DISTRICT_CODE = 508;



                List<SelectListItem> transactionType = common.PopulateTransactions(objparams);
                model.TXN_ID = masterDetails.TXN_ID + "$" + dbContext.ACC_MASTER_TXN.Where(m => m.TXN_ID == masterDetails.TXN_ID).Select(m => m.CASH_CHQ).FirstOrDefault().Trim();
                model.txn_ID1 = transactionType;

                objparams.TXN_ID = Convert.ToInt16(model.TXN_ID.Split('$')[0]);

                //if (objparams.TXN_ID == 472 || objparams.TXN_ID == 415)
                //{
                //    objparams.MAST_CONT_ID = Convert.ToInt32(masterDetails.ADMIN_NO_OFFICER_CODE);
                //}
                //else
                //{
                    objparams.MAST_CONT_ID = Convert.ToInt32(masterDetails.MAST_CON_ID);
                //}
                List<SelectListItem> monthList = common.PopulateMonths(masterDetails.BILL_MONTH);
                model.BILL_MONTH_List = monthList;
                model.BILL_MONTH = masterDetails.BILL_MONTH;

                List<SelectListItem> yearList = common.PopulateYears(masterDetails.BILL_YEAR);
                model.BILL_YEAR = masterDetails.BILL_YEAR;
                model.BILL_YEAR_List = yearList;

                List<SelectListItem> ContractorList = common.PopulateContractorSupplier(objparams);
                //if (objparams.TXN_ID == 472 || objparams.TXN_ID == 415)
                //{
                //    model.MAST_CON_ID_C = masterDetails.ADMIN_NO_OFFICER_CODE;
                //}
                //else
                //{
                    model.MAST_CON_ID_C = masterDetails.MAST_CON_ID;
                //}

                model.mast_CON_ID_C1 = ContractorList;

                objparams.MAST_CON_SUP_FLAG = "S";
                List<SelectListItem> SupplierList = common.PopulateContractorSupplier(objparams);
                //if (objparams.TXN_ID == 472 || objparams.TXN_ID == 415)
                //{
                //    model.MAST_CON_ID_S = masterDetails.ADMIN_NO_OFFICER_CODE;

                //}
                //else
                //{
                    model.MAST_CON_ID_S = masterDetails.MAST_CON_ID;
                //}

                model.mast_CON_ID_S1 = SupplierList;



                List<SelectListItem> RemitanceDepartment = new List<SelectListItem>();
                RemitanceDepartment = common.PopulateRemittanceDepartment();

                model.DEPT_ID = masterDetails.REMIT_TYPE.HasValue ? masterDetails.REMIT_TYPE.Value : Convert.ToByte(0);
                model.dept_ID1 = RemitanceDepartment;

                model.BILL_DATE = common.GetDateTimeToString(masterDetails.BILL_DATE);

                model.BILL_NO = masterDetails.BILL_NO;

                model.DEDUCTION_AMOUNT = masterDetails.CASH_AMOUNT;

                model.CHQ_AMOUNT = masterDetails.CHQ_AMOUNT;

                model.CASH_AMOUNT = masterDetails.CASH_AMOUNT;

                if (masterDetails.CHQ_DATE.HasValue)
                {
                    model.CHQ_DATE = common.GetDateTimeToString(masterDetails.CHQ_DATE.Value);
                }
                else
                {
                    model.CHQ_DATE = null;
                }

                MASTER_CONTRACTOR_BANK objBANK = dbContext.MASTER_CONTRACTOR_BANK.FirstOrDefault(s => s.MAST_CON_ID == masterDetails.MAST_CON_ID && s.MAST_ACCOUNT_ID == masterDetails.CON_ACCOUNT_ID);
                //if (objModuleType != null)
                //{
                //  moduleType = "R";

                // }

                // conAccountDetails = objPaymentBAL.GetContratorBankAccNoAndIFSCcode(Convert.ToInt32(masterDetails.MAST_CON_ID), PMGSYSession.Current.FundType, masterDetails.TXN_ID, ref isPFMSFinalized, true, true);
                //if (objBANK != null)
                //{
                //    conAccountDetails.Add(new SelectListItem { Text = objBANK.MAST_BANK_NAME + ":" + objBANK.MAST_IFSC_CODE + ":" + objBANK.MAST_ACCOUNT_NUMBER, Value = objBANK.MAST_ACCOUNT_ID.ToString(), Selected = true });

                //}
                conAccountDetails.Add(new SelectListItem { Text = objBANK.MAST_BANK_NAME + ":" + objBANK.MAST_IFSC_CODE + ":" + objBANK.MAST_ACCOUNT_NUMBER, Value = objBANK.MAST_ACCOUNT_ID.ToString(), Selected = true });

                model.CONC_Account_ID1 = conAccountDetails;

                //if Added By Abhishek kamble for Advice No 6Apr2015 start
                //if (masterDetails.CHQ_EPAY == "A")
                //{
                //    model.CHQ_EPAY = "Q";
                //}
                //else
                //{
                //    model.CHQ_EPAY = masterDetails.CHQ_EPAY;
                //}
                //Added By Abhishek kamble for Advice No 6Apr2015 end


                if (masterDetails.CHALAN_DATE.HasValue)
                {
                    model.CHALAN_DATE = common.GetDateTimeToString(masterDetails.CHALAN_DATE.Value);
                }
                else
                {
                    model.CHALAN_DATE = null;
                }

                model.CHALAN_NO = masterDetails.CHALAN_NO;

                //get payee name based on transaction 
                ACC_SCREEN_DESIGN_PARAM_MASTER obj = new ACC_SCREEN_DESIGN_PARAM_MASTER();


                obj = common.getMasterDesignParam(objparams);

                if (obj.MAST_CON_REQ.Trim() == "Y" && obj.REM_REQ.Trim() == "N")
                {
                    model.PAYEE_NAME_C = masterDetails.PAYEE_NAME;
                }
                else
                {
                    objparams.MAST_CONT_ID = masterDetails.MAST_CON_ID.HasValue ? masterDetails.MAST_CON_ID.Value : 0;
                    model.PAYEE_NAME_C = common.GetContractorSupplierName(objparams);
                }
                if (obj.MAST_SUPPLIER_REQ.Trim() == "Y")
                {
                    model.PAYEE_NAME_S = masterDetails.PAYEE_NAME;

                }
                if (obj.REM_REQ.Trim() == "Y")
                {
                    model.PAYEE_NAME_R = masterDetails.PAYEE_NAME;

                }
                else if (obj.MAST_CON_REQ.Trim() != "Y" && obj.MAST_SUPPLIER_REQ.Trim() != "Y" && obj.REM_REQ.Trim() != "Y")
                {
                    model.PAYEE_NAME = masterDetails.PAYEE_NAME;
                }

                if (masterDetails.CHQ_EPAY == "Q")
                {
                    model.CHQ_Book_ID_List = objPaymentBAL.GetchequebookSeries(PMGSYSession.Current.AdminNdCode, PMGSYSession.Current.FundType, PMGSYSession.Current.LevelId);
                    model.CHQ_Book_ID = masterDetails.CHQ_Book_ID;
                }
                else
                {
                    List<SelectListItem> CHQ_Book_ID = new List<SelectListItem>();
                    CHQ_Book_ID.Add(new SelectListItem { Text = "--Select--", Value = "0", Selected = true });
                    model.CHQ_Book_ID_List = CHQ_Book_ID;
                }

                model.CHQ_NO = masterDetails.CHQ_NO;

                if (masterDetails.CHQ_EPAY.Trim() == "E")
                {
                    model.EPAY_NO = masterDetails.CHQ_NO;
                }

                //if epayment with remittance its eremittnace
                if (masterDetails.CHQ_EPAY.Trim() == "E" && masterDetails.REMIT_TYPE != null)
                {
                    model.IS_EREMIT = true;
                }

                ViewBag.operationType = "E";

                ViewBag.Bill_id = URLEncrypt.EncryptParameters(new string[] { Bill_Id.ToString() });

                ViewBag.LevelID = PMGSYSession.Current.LevelId;

                //added by abhishek kamble 10Mar2015 for Cheque issue date disp.
                if (model.CHQ_Book_ID != null && model.CHQ_Book_ID != 0)
                {
                    ViewBag.ChequeIssueDate = objPaymentBAL.GetChequeBookIssueDate(Convert.ToInt64(model.CHQ_Book_ID));
                }

                //if Added By Abhishek kamble for Advice No 6Apr2015 start
                if (masterDetails.CHQ_EPAY == "A")
                {
                    model.CHQ_EPAY = "A";
                }
                else
                {
                    model.CHQ_EPAY = masterDetails.CHQ_EPAY;
                }
                //Added By Abhishek kamble for Advice No 6Apr2015 end
                return View("AddEditMasterPayment", model);
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
        /// function to get the cheque book series
        /// </summary>
        /// <returns></returns>
        [Audit]
        public JsonResult GetchequebookSeries()
        {
            try
            {
                return Json(objPaymentBAL.GetchequebookSeries(PMGSYSession.Current.AdminNdCode, PMGSYSession.Current.FundType, PMGSYSession.Current.LevelId));
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                string errorMsg = string.Format("Errors: {0}", ex.Message);
                Response.TrySkipIisCustomErrors = true;
                Response.StatusCode = 500;
                Response.Write(errorMsg);
                return Json(new String[] { });
            }
        }

        /// <summary>
        /// Purpose :   Get the list of available cheque
        /// Called  :   By change event of chequbook series  in AddEditMasterPayment.js
        /// Author  :  Amol Jadhav
        /// Created :  29/04/2013
        /// </summary>
        /// <param name="id1">random number</param>
        /// <returns>Json</returns>

        [Audit]
        public JsonResult GetAllAvailableCheques(String id)
        {

            int chequebookid = 0;
            int adminNdCode = PMGSYSession.Current.AdminNdCode;
            Int64 billID = 0;
            /* adminNdCode = 11;*/

            string[] availableCheques = null;

            //added by Koustubh Nakate on 20/09/2013 
            string operation = "A";

            if (!string.IsNullOrEmpty(Request.Params["opeartion"]))
            {
                operation = Request.Params["opeartion"].ToString();
            }

            if (!string.IsNullOrEmpty(Request.Params["encryptedBillID"]))
            {
                String[] urlParams = URLEncrypt.DecryptParameters(new String[] { Request.Params["encryptedBillID"].ToString().Split('/')[0], Request.Params["encryptedBillID"].ToString().Split('/')[1], Request.Params["encryptedBillID"].ToString().Split('/')[2] });
                if (urlParams.Length >= 1)
                {
                    billID = Convert.ToInt64(urlParams[0]);
                }
            }

            try
            {
                chequebookid = Convert.ToInt32(id);

                availableCheques = objPaymentBAL.GetAllAvailableChequesArray(chequebookid, adminNdCode, PMGSYSession.Current.FundType, operation, billID);
                //old
                //return Json(availableCheques);

                //
                JsonResult result = Json(availableCheques);
                result.MaxJsonLength = Int32.MaxValue;
                return result;
            }

            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                string errorMsg = string.Format("Errors: {0}", ex.Message);
                Response.TrySkipIisCustomErrors = true;
                Response.StatusCode = 500;
                Response.Write(errorMsg);
                return Json(new String[] { });
            }
        }

        /// <summary>
        /// action to list the master payment details
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="hash"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        /* public ActionResult GetPaymentDetailList(String parameter, String hash, String key)
         {
             Int64 master_Bill_Id = 0;
            
             if (!String.IsNullOrEmpty(parameter) && !String.IsNullOrEmpty(hash) && !String.IsNullOrEmpty(key))
             {
                 String[] urlParams = URLEncrypt.DecryptParameters(new String[] { parameter, hash, key });
                 if (urlParams.Length >= 1)
                 {
                     String[] urlSplitParams = urlParams[0].Split('$');
                     master_Bill_Id = Convert.ToInt64(urlSplitParams[0]);
                                      

                 }
             }
             return View();

         }*/


        /// <summary>
        /// function to get the subtransaction based on the master head 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Audit]
        public JsonResult GetSubtransaction(string id)
        {

            TransactionParams objparams = new TransactionParams();
            objparams.BILL_TYPE = "P";
            objparams.FUND_TYPE = PMGSYSession.Current.FundType;
            objparams.LVL_ID = PMGSYSession.Current.LevelId;
            objparams.ADMIN_ND_CODE = PMGSYSession.Current.AdminNdCode;

            /*
            TransactionParams objparams = new TransactionParams();
            objparams.BILL_TYPE = "P";
            objparams.FUND_TYPE = PMGSYSession.Current.FundType;
            objparams.LVL_ID = 5;
            objparams.ADMIN_ND_CODE = 11;
             */

            List<SelectListItem> Subtransactions = null;
            try
            {
                if (!String.IsNullOrEmpty(id))
                {

                    objparams.TXN_ID = Convert.ToInt16(id.Split('$')[0]);
                    objparams.BILL_TYPE = id.Split('$')[1].Trim();

                }
                else
                {
                    throw new Exception("Error while getting subtransaction details.");
                }

                Subtransactions = common.PopulateTransactions(objparams);


                return Json(Subtransactions);
            }

            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                string errorMsg = string.Format("Errors: {0}", ex.Message);
                Response.TrySkipIisCustomErrors = true;
                Response.StatusCode = 500;
                Response.Write(errorMsg);
                return Json(new String[] { });
            }

        }

        /// <summary>
        /// action to get the payment details of transactions 
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="hash"></param>
        /// <param name="key"></param>
        [HttpPost]
        public ActionResult GetPaymentDetailList(int? page, int? rows, string sidx, string sord, String parameter, String hash, String key)
        {
            var header = Request.Headers;
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

                        objFilter.BillId = Convert.ToInt64(urlParams[0]);


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
                    rows = objPaymentBAL.ListPaymentDeductionDetailsForDataEntry(objFilter, out totalRecords),
                    total = totalRecords <= objFilter.rows ? 1 : totalRecords / objFilter.rows + 1,
                    page = page,
                    records = totalRecords
                };

                return Json(jsonData, JsonRequestBehavior.AllowGet);
                //return Json(jsonData);
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
        /// function to get the list of deduction entries of the transaction 
        /// </summary>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <param name="sidx"></param>
        /// <param name="sord"></param>
        /// <param name="parameter">encrypted params </param>
        /// <param name="hash"></param>
        /// <param name="key"> key to decrypt</param>
        /// <returns></returns>
        [HttpPost]
        [Audit]
        public JsonResult GetPaymentDeductionDetailList(int? page, int? rows, string sidx, string sord, String parameter, String hash, String key)
        {

            PaymentFilterModel objFilter = new PaymentFilterModel();
            try
            {
                if (!String.IsNullOrEmpty(parameter) && !String.IsNullOrEmpty(hash) && !String.IsNullOrEmpty(key))
                {
                    String[] urlParams = URLEncrypt.DecryptParameters(new String[] { parameter, hash, key });
                    if (urlParams.Length >= 1)
                    {

                        objFilter.BillId = Convert.ToInt64(urlParams[0]);


                    }
                }
                else
                {
                    throw new Exception("Error while getting deduction payment transaction list");
                }

                long totalRecords;

                objFilter.page = Convert.ToInt32(page) - 1;
                objFilter.rows = Convert.ToInt32(rows);
                objFilter.sidx = sidx.ToString();
                objFilter.sord = sord.ToString();
                objFilter.Bill_type = "P";
                objFilter.FundType = PMGSYSession.Current.FundType;
                objFilter.Deduction_Payment = "D";
                objFilter.AdminNdCode = PMGSYSession.Current.AdminNdCode;
                objFilter.LevelId = PMGSYSession.Current.LevelId;

                /*
                               objFilter.AdminNdCode = 11;
                               objFilter.LevelId = 5;
               */
                var jsonData = new
                {
                    rows = objPaymentBAL.ListPaymentDeductionDetailsForDataEntry(objFilter, out totalRecords),
                    total = totalRecords <= objFilter.rows ? 1 : totalRecords / objFilter.rows + 1,
                    page = page,
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
        /// function to delete the transaction (payment or deduction)
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="hash"></param>
        /// <param name="key"></param>
        /// <returns> json values indicating result of operation ,transaction type (payment or deduction) & encrypted billid </returns>
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

                        value = objPaymentBAL.DeleteTransactionPaymentDetails(master_Bill_Id, tranNumber, paymentDeduction);


                        return Json(new
                        {
                            Success = value,
                            TransactionType = paymentDeduction == "D" ? "D" : "P",
                            master_Bill_Id = URLEncrypt.EncryptParameters(new string[] { master_Bill_Id.ToString() })
                        });

                    }
                    else
                    {
                        throw new Exception("Error while getting payment transaction to delete");
                    }
                }
                else
                {
                    throw new Exception("Error while getting payment transaction to delete");
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
        /// Get action to edit the transaction details 
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="hash"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        [HttpGet]
        [Audit]
        public ActionResult EditTransactionDetails(String parameter, String hash, String key)
        {

            ACC_SCREEN_DESIGN_PARAM_DETAILS_CUSTOM parametes = new ACC_SCREEN_DESIGN_PARAM_DETAILS_CUSTOM();
            Int16 tranNumber = 0;
            String paymentDeduction = string.Empty;
            PMGSYEntities dbContext = new PMGSYEntities();
            TransactionParams objparams = new TransactionParams();
            PaymentDetailsModel model = new PaymentDetailsModel();
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
                ACC_BILL_MASTER masterDetails = new ACC_BILL_MASTER();
                masterDetails = objPaymentBAL.GetMasterPaymentDetails(objparams.BILL_ID);

                if (masterDetails.CHQ_EPAY == "A")    //Added By Abhishek for Advice No 8Apr2015
                {
                    masterDetails.CHQ_EPAY = "Q";
                }

                //get the transaction Details
                ACC_BILL_DETAILS transactionDetails = new ACC_BILL_DETAILS();
                transactionDetails = objPaymentBAL.GetTransactionPaymentDetails(objparams.BILL_ID, tranNumber, paymentDeduction);


                objparams.BILL_TYPE = masterDetails.BILL_TYPE;
                objparams.FUND_TYPE = masterDetails.FUND_TYPE;
                objparams.LVL_ID = masterDetails.LVL_ID;
                objparams.MAST_CONT_ID = masterDetails.MAST_CON_ID.HasValue ? masterDetails.MAST_CON_ID.Value : 0;
                objparams.DISTRICT_CODE = PMGSYSession.Current.DistrictCode;
                objparams.TXN_ID = masterDetails.TXN_ID;
                objparams.STATE_CODE = Convert.ToInt16(PMGSYSession.Current.StateCode);
                objparams.DISTRICT_CODE = Convert.ToInt16(PMGSYSession.Current.DistrictCode);
                objparams.SANC_YEAR = Convert.ToInt16(model.SANCTION_YEAR);
                objparams.OP_MODE = "E";
                objparams.HEAD_ID = transactionDetails.HEAD_ID;
                objparams.AGREEMENT_CODE = transactionDetails.IMS_AGREEMENT_CODE.HasValue ? transactionDetails.IMS_AGREEMENT_CODE.Value : 0;
                List<SelectListItem> subHeadList = new List<SelectListItem>();

                subHeadList = common.PopulateTransactions(objparams);

                model.CONTRACTOR_ID = masterDetails.MAST_CON_ID.HasValue ? masterDetails.MAST_CON_ID.Value : 0;

                model.HeadId_P = new List<SelectListItem>(subHeadList);

                //for remittance payment set the payment transaction same as remittance office set in master payment
                if (masterDetails.TXN_ID != 462)
                {
                    if (masterDetails.REMIT_TYPE != null)
                    {
                        foreach (var item in subHeadList)
                        {
                            if (item.Value != string.Empty)
                            {
                                //get the head id 
                                int head = Convert.ToInt32(item.Value.Split('$')[0]);

                                //get the order of it from master txn table 
                                int orderOfTxn = dbContext.ACC_MASTER_TXN.Where(x => x.TXN_ID == head).Select(x => x.TXN_ORDER).FirstOrDefault();

                                //check if order matches or remove from subtransaction list
                                if (orderOfTxn != masterDetails.REMIT_TYPE)
                                {
                                    model.HeadId_P.Remove(item);
                                }
                            }
                        }
                    }
                }

                subHeadList.Clear();

                subHeadList = null;

                //new change done by Vikram for populating Road according to the Maintenance Agreement and Contractor

                if (PMGSYSession.Current.FundType == "M")
                {
                    //Old
                    //objparams.AGREEMENT_NUMBER = dbContext.MANE_IMS_CONTRACT.Where(m => m.MANE_PR_CONTRACT_CODE == objparams.AGREEMENT_CODE).Select(m => m.MANE_AGREEMENT_NUMBER).FirstOrDefault();
                    //Modified By Abhishek kamble to get Agrement No using IMS_CONTRACTOR_ID 
                    objparams.AGREEMENT_NUMBER = dbContext.MANE_IMS_CONTRACT.Where(m => m.MANE_CONTRACT_ID == objparams.AGREEMENT_CODE).Select(m => m.MANE_AGREEMENT_NUMBER).FirstOrDefault();
                }

                //agreement for deduction

                model.AGREEMENT_DED = common.PopulateAgreement(objparams);

                //find out for deduction
                objparams.BILL_TYPE = "D";

                model.HeadId_D = common.PopulateTransactions(objparams);

                objparams.AGREEMENT_CODE = transactionDetails.IMS_AGREEMENT_CODE.HasValue ? transactionDetails.IMS_AGREEMENT_CODE.Value : 0;

                if (transactionDetails.IMS_PR_ROAD_CODE == null)
                {
                    model.IMS_PR_ROAD_CODE = 0;
                }
                else
                {
                    model.IMS_PR_ROAD_CODE = transactionDetails.IMS_PR_ROAD_CODE;
                }
                if (paymentDeduction != "D")
                {

                    objparams.TXN_ID = Convert.ToInt16(transactionDetails.TXN_ID);

                    parametes = common.getDetailsDesignParamForPayment(objparams);



                    if (parametes.PIU_REQ.Trim() == "Y")
                    {
                        model.MAST_DPIU_CODEList = common.PopulateDPIU(objparams);
                    }
                    else
                    {
                        model.MAST_DPIU_CODEList = generalList;
                    }

                    if (parametes.AGREEMENT_REQ.Trim() == "Y" && parametes.CON_REQ.Trim() == "Y")
                    {

                        //for remittnaces get  agrrements  
                        if (parametes.SHOW_CON_AT_TRANSACTION)
                        {
                            objparams.MAST_CONT_ID = transactionDetails.MAST_CON_ID.HasValue ? transactionDetails.MAST_CON_ID.Value : 0;
                        }

                        model.AGREEMENT_C = common.PopulateAgreement(objparams);
                    }
                    else
                    {
                        model.AGREEMENT_C = generalList;

                    }

                    if (parametes.AGREEMENT_REQ.Trim() == "Y" && parametes.SUPPLIER_REQ.Trim() == "Y")
                    {
                        model.AGREEMENT_S = common.PopulateAgreement(objparams);
                    }
                    else
                    {
                        model.AGREEMENT_S = generalList;
                    }

                    if (parametes.YEAR_REQ.Trim() == "Y")
                    {

                        model.SANCTION_YEAR = common.getSancYearFromRoad(model.IMS_PR_ROAD_CODE.HasValue ? model.IMS_PR_ROAD_CODE.Value : 0);

                        model.IMS_SANCTION_YEAR_List = common.PopulateSancYear(objparams);
                    }
                    else
                    {
                        generalList.Clear();
                        generalList.Add(new SelectListItem { Selected = true, Text = "--Select--", Value = "0" });
                        model.IMS_SANCTION_YEAR_List = generalList;
                    }
                    if (parametes.PKG_REQ.Trim() == "Y")
                    {
                        model.SANCTION_PACKAGE = common.getPackageFromRoad(model.IMS_PR_ROAD_CODE.HasValue ? model.IMS_PR_ROAD_CODE.Value : 0);
                        objparams.SANC_YEAR = Convert.ToInt16(model.SANCTION_YEAR);
                        model.IMS_SANCTION_PACKAGE_List = common.PopulatePackage(objparams);
                    }
                    else
                    {
                        generalList.Clear();
                        generalList.Add(new SelectListItem { Selected = true, Text = "--Select--", Value = "0" });
                        model.IMS_SANCTION_PACKAGE_List = generalList;
                    }
                    if (parametes.ROAD_REQ.Trim() == "Y")
                    {
                        objparams.PACKAGE_ID = model.SANCTION_PACKAGE;
                        model.IMS_PR_ROAD_CODEList = common.PopulateRoad(objparams);
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
                    model.AGREEMENT_S = generalList;
                    model.AGREEMENT_S = generalList;
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
                    model.IMS_AGREEMENT_CODE_S = transactionDetails.IMS_AGREEMENT_CODE;
                    if (transactionDetails.IMS_PR_ROAD_CODE == null)
                    {
                        model.IMS_PR_ROAD_CODE = 0;
                    }
                    else
                    {
                        model.IMS_PR_ROAD_CODE = transactionDetails.IMS_PR_ROAD_CODE;
                    }
                    model.MAST_DPIU_CODE = transactionDetails.ADMIN_ND_CODE;
                    model.HEAD_ID_P = transactionDetails.TXN_ID + "$" + dbContext.ACC_MASTER_TXN.Where(m => m.TXN_ID == transactionDetails.TXN_ID).Select(m => m.CASH_CHQ).FirstOrDefault().Trim();
                    model.AMOUNT_Q = transactionDetails.AMOUNT;
                    model.FINAL_PAYMENT = transactionDetails.FINAL_PAYMENT;
                    model.NARRATION_P = transactionDetails.NARRATION;

                    if (masterDetails.CASH_AMOUNT != 0)
                    {
                        ACC_BILL_DETAILS cashTransactionDetails = new ACC_BILL_DETAILS();
                        cashTransactionDetails = objPaymentBAL.GetTransactionPaymentDetails(objparams.BILL_ID, Convert.ToInt16(tranNumber + 1), paymentDeduction);

                        if (cashTransactionDetails != null)
                        {
                            model.AMOUNT_C = cashTransactionDetails.AMOUNT;
                        }
                    }
                }

                if (!(transactionDetails.IMS_PR_ROAD_CODE == null))
                {
                    string proposalType = (dbContext.IMS_SANCTIONED_PROJECTS.Where(c => c.IMS_PR_ROAD_CODE == transactionDetails.IMS_PR_ROAD_CODE).Select(c => c.IMS_PROPOSAL_TYPE).First());

                    bool physicalCompletion = false;

                    if (proposalType.Equals("P"))
                    {
                        if (dbContext.EXEC_ROADS_MONTHLY_STATUS.Where(c => c.IMS_PR_ROAD_CODE == transactionDetails.IMS_PR_ROAD_CODE && c.EXEC_ISCOMPLETED == "C").Any())
                        {
                            physicalCompletion = true;
                        }
                    }
                    else if (proposalType.Equals("L"))
                    {
                        if (dbContext.EXEC_LSB_MONTHLY_STATUS.Where(c => c.IMS_PR_ROAD_CODE == transactionDetails.IMS_PR_ROAD_CODE && c.EXEC_ISCOMPLETED == "C").Any())
                        {
                            physicalCompletion = true;
                        }
                    }
                    else
                    {
                        physicalCompletion = true;
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
                            if (physicalCompletion)
                            {
                                List.Add(new SelectListItem { Text = "Yes", Value = "true" });
                            }
                            List.Add(new SelectListItem { Selected = true, Text = "No", Value = "false" });
                        }

                    }
                    else
                    {
                        if (physicalCompletion)
                        {
                            List.Add(new SelectListItem { Text = "Yes", Value = "true" });
                        }
                        List.Add(new SelectListItem { Text = "No", Value = "false" });
                    }
                    model.final_pay = List;
                }
                else
                {
                    List<SelectListItem> List = new List<SelectListItem>();
                    List.Add(new SelectListItem { Text = "Select", Value = "false" });
                    model.final_pay = List;
                }

                ViewBag.urlparams = URLEncrypt.EncryptParameters(new string[] { objparams.BILL_ID.ToString() + "$" + tranNumber + "$" + paymentDeduction });

                ViewBag.BillFinalized = masterDetails.BILL_FINALIZED;

                ViewBag.Bill_ID = URLEncrypt.EncryptParameters(new string[] { objparams.BILL_ID.ToString() });

                #region if for master payment multiple transsaction is not allowed

                ACC_SCREEN_DESIGN_PARAM_MASTER obj = new ACC_SCREEN_DESIGN_PARAM_MASTER();

                //get master txn id
                objparams.TXN_ID = masterDetails.TXN_ID;

                obj = common.getMasterDesignParam(objparams);

                bool disablePayHead = false;

                if (obj.MTXN_REQ == "N")
                {
                    if (!dbContext.ACC_BILL_DETAILS.Where(d => d.BILL_ID == masterDetails.BILL_ID).Any())
                    {
                        disablePayHead = false;
                    }
                    else
                    {
                        Int16? txn = dbContext.ACC_BILL_DETAILS.Where(d => d.BILL_ID == masterDetails.BILL_ID).Select(c => c.TXN_ID).First();
                        string CheCash = dbContext.ACC_MASTER_TXN.Where(m => m.TXN_ID == txn).Select(m => m.CASH_CHQ).FirstOrDefault().Trim();
                        model.HEAD_ID_P = txn + "$" + CheCash;
                        disablePayHead = true;
                    }
                }
                //to disable on error
                ViewBag.disablePaymentHead = disablePayHead;

                if (parametes.SHOW_CON_AT_TRANSACTION)
                {
                    objparams.MAST_CON_SUP_FLAG = "C";
                    model.mast_CON_ID_CON1 = common.PopulateContractorSupplier(objparams);
                    model.MAST_CON_ID_CON = transactionDetails.MAST_CON_ID;
                }
                else
                {
                    model.mast_CON_ID_CON1 = generalList;
                }



                if (obj.DED_REQ == "B")
                {

                    ViewBag.DeductionScreenRequired = (masterDetails.CASH_AMOUNT != 0 && (masterDetails.CHQ_EPAY == "Q" || masterDetails.CHQ_EPAY == "E")) ? true : masterDetails.CHQ_EPAY == "C" ? true : false;
                    //If condition Added By Abhishek kamble 16-Apr-2014
                    if ((objparams.TXN_ID == 47) || (objparams.TXN_ID == 737))
                    {
                        ViewBag.PaymentScreenRequired = true;
                    }
                    //If condition Added By Abhishek kamble 20-June-2014  payment screen for deduction only transaction
                    else if (((objparams.TXN_ID == 72) || (objparams.TXN_ID == 314) || (objparams.TXN_ID == 415) || (objparams.TXN_ID == 1484) || (objparams.TXN_ID == 86) || (objparams.TXN_ID == 777) || (objparams.TXN_ID == 787)) && (masterDetails.CHQ_EPAY == "C"))
                    {
                        ViewBag.PaymentScreenRequired = true;
                    }
                    else
                    {
                        ViewBag.PaymentScreenRequired = masterDetails.CHQ_AMOUNT != 0 && (masterDetails.CHQ_EPAY == "Q" || masterDetails.CHQ_EPAY == "E") ? true : false;
                    }


                }
                else
                {
                    //If condition Added By Abhishek kamble 16-Apr-2014
                    if ((objparams.TXN_ID == 47) || (objparams.TXN_ID == 737))
                    {
                        ViewBag.PaymentScreenRequired = true;
                    }
                    else
                    {
                        ViewBag.PaymentScreenRequired = (masterDetails.CHQ_AMOUNT != 0 && (masterDetails.CHQ_EPAY == "Q" || masterDetails.CHQ_EPAY == "E")) || (masterDetails.CHQ_EPAY == "C") ? true : false;
                    }
                    ViewBag.DeductionScreenRequired = masterDetails.CASH_AMOUNT != 0 && (masterDetails.CHQ_EPAY == "Q" || masterDetails.CHQ_EPAY == "E") ? true : false;
                }

                #endregion

                //added by Koustubh Nakate on 24/09/2013 to disable subtransaction on edit mode
                if (paymentDeduction.Equals("P"))
                {
                    ViewBag.disablePaymentHead = true;
                }
                else if (paymentDeduction.Equals("D"))
                {
                    ViewBag.disableDeductionHead = true;
                }
                else if (paymentDeduction.Equals("Q"))
                {
                    ViewBag.disablePaymentHead = true;
                }
                //else
                //{
                //    ViewBag.disablePaymentHead = true;
                //    ViewBag.disableDeductionHead = true;
                //}
                return View("PartialTransaction", model);


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
        /// post action to edit the transaction details
        /// </summary>
        /// <param name="model"></param>
        /// <param name="parameter"></param>
        /// <param name="hash"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        [HttpPost]
        [Audit]
        [ValidateAntiForgeryToken]
        public ActionResult PostEditTransactionDetails(PaymentDetailsModel model, String parameter, String hash, String key)
        {
            PaymentDAL paymentDAL = new PaymentDAL();
            Int16 tranNumber = 0;
            String paymentDeduction = string.Empty;
            PMGSYEntities dbContext = new PMGSYEntities();
            TransactionParams objparams = new TransactionParams();
            ACC_SCREEN_DESIGN_PARAM_DETAILS_CUSTOM parametes = new ACC_SCREEN_DESIGN_PARAM_DETAILS_CUSTOM();
            CommonFunctions objCommon = new CommonFunctions();
            try
            {
                if (PMGSYSession.Current.FundType == "P")
                {
                    if (model.IMS_PR_ROAD_CODE != null && model.IMS_AGREEMENT_CODE_C != null)
                    {
                        if (model.IMS_PR_ROAD_CODE > 0 && model.IMS_AGREEMENT_CODE_C > 0)
                        {
                            IMS_SANCTIONED_PROJECTS masterModel = dbContext.IMS_SANCTIONED_PROJECTS.Where(m => m.IMS_PR_ROAD_CODE == model.IMS_PR_ROAD_CODE).FirstOrDefault();

                            if (masterModel.IMS_ISCOMPLETED.Equals("C") || masterModel.IMS_ISCOMPLETED.Equals("X"))
                            {// Allow
                            }
                            else
                            {
                                var EstablishmentDate = dbContext.UDF_QM_LAB_EST_DATE(masterModel.IMS_PACKAGE_ID, model.IMS_AGREEMENT_CODE_C).FirstOrDefault();
                                if (EstablishmentDate == null)
                                {
                                    return Json(new { Success = false, status = "-777", message = "Lab Establishment Details are not available, Payment can not be made." });
                                }

                            }
                        }
                    }
                }




                if (PMGSYSession.Current.FundType == "M")
                {
                    model.FINAL_PAYMENT = null;
                }

                //Added by Abhishek kamble 14Aug2015 end


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

                if (model.HEAD_ID_P != null)
                {
                    //if (Convert.ToInt32(model.HEAD_ID_P.Split('$')[0]) == 48 || Convert.ToInt32(model.HEAD_ID_P.Split('$')[0]) == 49)
                    ///Changed by SAMMED A. PATIL on 20FEB2018 for RCPLWE upgradation check as per account head 
                    if (Convert.ToInt32(model.HEAD_ID_P.Split('$')[0]) == 48 || Convert.ToInt32(model.HEAD_ID_P.Split('$')[0]) == 49 || Convert.ToInt32(model.HEAD_ID_P.Split('$')[0]) == 1789 || Convert.ToInt32(model.HEAD_ID_P.Split('$')[0]) == 1790)
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


                //get the master transaction details
                ACC_BILL_MASTER masterDetails = new ACC_BILL_MASTER();
                masterDetails = objPaymentBAL.GetMasterPaymentDetails(objparams.BILL_ID);

                if (masterDetails.CHQ_EPAY == "A")//Adivice no change 8Apr2015
                {
                    masterDetails.CHQ_EPAY = "Q";
                }

                //get the transaction Details for cheque entry
                ACC_BILL_DETAILS transactionDetails = new ACC_BILL_DETAILS();
                transactionDetails = objPaymentBAL.GetTransactionPaymentDetails(objparams.BILL_ID, tranNumber, paymentDeduction);

                List<SelectListItem> generalList = new List<SelectListItem>();
                generalList.Add(new SelectListItem { Selected = true, Text = "--Select--", Value = "" });

                #region if for master payment multiple transsaction is not allowed

                ACC_SCREEN_DESIGN_PARAM_MASTER obj = new ACC_SCREEN_DESIGN_PARAM_MASTER();

                objparams.TXN_ID = masterDetails.TXN_ID;

                obj = common.getMasterDesignParam(objparams);

                bool disablePayHead = false;

                if (obj.MTXN_REQ == "N")
                {
                    if (!dbContext.ACC_BILL_DETAILS.Where(d => d.BILL_ID == masterDetails.BILL_ID).Any())
                    {
                        disablePayHead = false;
                    }
                    else
                    {
                        Int16? txn = dbContext.ACC_BILL_DETAILS.Where(d => d.BILL_ID == masterDetails.BILL_ID).Select(c => c.TXN_ID).First();
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

                #region Validation for payment of roads within 6 months of completion
                string paymentValidationDt = ConfigurationManager.AppSettings["PaymentValidationDate"].ToString();
                ///Commented as per instructions from Anita Mam on 07JAN2019
                if (PMGSYSession.Current.FundType == "P" && model.IMS_PR_ROAD_CODE != null && model.IMS_PR_ROAD_CODE != 0 && masterDetails.BILL_TYPE == "P" && masterDetails.TXN_ID != 86
                    //&& PMGSYSession.Current.StateCode != 16 && PMGSYSession.Current.StateCode != 15
                    && (masterDetails.BILL_DATE >= common.GetStringToDateTime(paymentValidationDt))
                    )
                {
                    //bool flg = paymentDAL.CompletionDateValidation(model.IMS_PR_ROAD_CODE.Value, masterDetails.BILL_DATE);
                    string[] param = null;
                    if (!paymentDAL.CompletionDateValidation1(model.IMS_PR_ROAD_CODE.Value, masterDetails.BILL_DATE, ref param))
                    {
                        int diff = Convert.ToInt32((masterDetails.BILL_DATE - objCommon.GetStringToDateTime(param[1])).TotalDays);
                        return Json(new { Success = false, status = "-9", message = "Transaction cannot be made on selected " + param[0] + " as completion date [" + param[1] + "] is more than 6 months (180 days). [Difference in days=" + diff + "]" });
                    }
                }
                #endregion

                //if no other model errors are there then only check for dynamic validation
                if (ModelState.IsValid)
                {
                    if (!paymentDeduction.Equals("D"))
                    {
                        #region serverside validation for payment

                        objparams.TXN_ID = Convert.ToInt16(model.HEAD_ID_P.Split('$')[0]);

                        if (model.HEAD_ID_P == null || model.HEAD_ID_P == String.Empty)
                        {
                            ModelState.AddModelError("HEAD_ID_P", "Sub Transaction Type (Payment) is Required");
                        }
                        else
                        {
                            // to check if correct entry as is operational and is required after porting flag
                            string correctionStatus = objCommon.ValidateHeadForCorrection(objparams.TXN_ID, 0, "E");
                            if (correctionStatus != "1")
                            {
                                ModelState.AddModelError("HEAD_ID_P", "Sub Transaction Type (Payment) is invalid");
                            }
                            else
                            {

                                if (ModelState.ContainsKey("HEAD_ID_P"))
                                    ModelState["HEAD_ID_P"].Errors.Clear();
                            }
                        }


                        parametes = common.getDetailsDesignParamForPayment(objparams);

                        if (parametes.SHOW_CON_AT_TRANSACTION)
                        {
                            model.CON_ID = model.MAST_CON_ID_CON;

                        }
                        else
                        {
                            //for details master contractor id get it from master table (used for opening balance) 
                            model.CON_ID = masterDetails.MAST_CON_ID;
                        }

                        if (parametes.ROAD_REQ.Trim() == "Y")
                        {

                            if (!(model.HEAD_ID_P.Equals("90$Q")))
                            {
                                //if (model.IMS_PR_ROAD_CODE == null || model.IMS_PR_ROAD_CODE == 0)
                                //{
                                //    ModelState.AddModelError("IMS_PR_ROAD_CODE", "Road is Required");
                                //}
                                ///Changes for Supplier Payment Building Proposal
                                if (model.HEAD_ID_P.Equals("1489$Q") || model.HEAD_ID_P.Equals("1490$Q"))
                                {
                                    bool flg = common.getContractorSupplierbyBillId(objparams.BILL_ID);
                                    if (flg)
                                    {
                                        model.ROAD_REQ = "N";
                                        parametes.ROAD_REQ = "N";
                                        if (ModelState.ContainsKey("IMS_PR_ROAD_CODE"))
                                            ModelState["IMS_PR_ROAD_CODE"].Errors.Clear();
                                    }
                                    else
                                    {
                                        if (model.IMS_PR_ROAD_CODE == null || model.IMS_PR_ROAD_CODE == 0)
                                        {
                                            ModelState.AddModelError("IMS_PR_ROAD_CODE", "Road is Required");
                                        }
                                    }
                                }
                                else if ((parametes.CON_REQ.Trim().Equals("Y")) && (parametes.AGREEMENT_REQ.Trim().Equals("Y")) && (parametes.ROAD_REQ.Trim().Equals("Y")) && (masterDetails.TXN_ID == 86))
                                {
                                    string proposalType = (dbContext.TEND_AGREEMENT_MASTER.Where(c => c.MAST_CON_ID == masterDetails.MAST_CON_ID && c.TEND_AGREEMENT_CODE == model.IMS_AGREEMENT_CODE_C).Select(c => c.TEND_AGREEMENT_TYPE).First());
                                    if (!(proposalType.Equals("D")))
                                    {
                                        if (model.IMS_PR_ROAD_CODE == null || model.IMS_PR_ROAD_CODE == 0)
                                        {
                                            ModelState.AddModelError("IMS_PR_ROAD_CODE", "Road is Required");
                                        }
                                    }
                                }
                                else
                                {
                                    if (model.IMS_PR_ROAD_CODE == null || model.IMS_PR_ROAD_CODE == 0)
                                    {
                                        ModelState.AddModelError("IMS_PR_ROAD_CODE", "Road is Required");
                                    }
                                }

                            }
                        }
                        else
                        {
                            if (ModelState.ContainsKey("IMS_PR_ROAD_CODE"))
                                ModelState["IMS_PR_ROAD_CODE"].Errors.Clear();
                        }

                        if (parametes.PIU_REQ.Trim() == "Y")
                        {
                            //commented by Vikram
                            //if (model.PIU_REQ == null || model.PIU_REQ == String.Empty)
                            //{
                            //    ModelState.AddModelError("PIU_REQ", "PIU is Required");
                            //}
                            if (model.MAST_DPIU_CODE == null)
                            {
                                ModelState.AddModelError("PIU_REQ", "PIU is Required");
                            }
                        }
                        else
                        {
                            if (ModelState.ContainsKey("PIU_REQ"))
                                ModelState["PIU_REQ"].Errors.Clear();
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

                            if (model.IMS_AGREEMENT_CODE_S == null || model.IMS_AGREEMENT_CODE_S == 0)
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





                        if (model.AMOUNT_Q == null)
                        {
                            ModelState.AddModelError("AMOUNT_Q", "Amount is Required");
                        }
                        else if (model.AMOUNT_Q == 0)
                        {
                            //If Condition Added By Abhishek kamble 16-Apr-2014
                            //if ((masterDetails.TXN_ID != 47) && (masterDetails.TXN_ID != 737) && (masterDetails.TXN_ID != 72) && (masterDetails.TXN_ID != 86) && (masterDetails.TXN_ID != 777) && (masterDetails.TXN_ID != 787))
                            //{

                            if ((masterDetails.TXN_ID != 47) && (masterDetails.TXN_ID != 1484) && (masterDetails.TXN_ID != 737) && (masterDetails.TXN_ID != 1788) && (masterDetails.TXN_ID != 1974))//PMGSY3
                            {
                                ModelState.AddModelError("AMOUNT_Q", " Amount must be greater than 0");
                            }
                        }
                        else if (model.AMOUNT_C != null)
                        {
                            //if (model.AMOUNT_Q < model.AMOUNT_C)
                            //{
                            //    ModelState.AddModelError("AMOUNT_Q", " Amount must be greater than cash amount");
                            //}

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



                        //validation for checking the asset amount is less than total payment amount against same head
                        if (objPaymentBAL.CheckForAssetPaymentValidation(transactionDetails.BILL_ID, transactionDetails.HEAD_ID, transactionDetails.TXN_NO, model.AMOUNT_Q.Value) == "0")
                        {
                            ModelState.AddModelError("AMOUNT_Q", "total Amount for selected transaction cant be less than Total amount of which asset details has been entered. ");
                        }


                        AmountCalculationModel amountModel = new AmountCalculationModel();

                        amountModel = objPaymentBAL.CalculatePaymentAmounts(objparams.BILL_ID);

                        //cash only transaction Amount Q contains the cash amount
                        if (obj.DED_REQ != "B" && masterDetails.CHQ_EPAY == "C")
                        {

                            //get the details for cash amount from bill details 
                            ACC_BILL_DETAILS transactionDetailsCash = new ACC_BILL_DETAILS();
                            transactionDetailsCash = objPaymentBAL.GetTransactionPaymentDetails(objparams.BILL_ID, Convert.ToInt16(tranNumber + 1), paymentDeduction);

                            if (transactionDetailsCash != null)
                            {
                                Decimal TotalAmount = amountModel.DiffCachAmount + transactionDetailsCash.AMOUNT;

                                if (transactionDetailsCash.AMOUNT >= model.AMOUNT_Q.Value)
                                {
                                }
                                else
                                {
                                    if (TotalAmount < model.AMOUNT_Q.Value)
                                    {
                                        ModelState.AddModelError("AMOUNT_Q", "Invalid  Amount.its greater than remaining cash amount to enter");
                                    }
                                    else if (TotalAmount >= model.AMOUNT_Q.Value)
                                    {
                                    }
                                    else
                                    {
                                        ModelState.AddModelError("AMOUNT_Q", "Invalid  Amount.its greater than remaining cash amount to enter");
                                    }

                                }
                            }

                        }
                        else
                        {

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
                                ACC_BILL_DETAILS transactionDetailsCash = new ACC_BILL_DETAILS();
                                transactionDetailsCash = objPaymentBAL.GetTransactionPaymentDetails(objparams.BILL_ID, Convert.ToInt16(tranNumber + 1), paymentDeduction);

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

                        if (model.HEAD_ID_D == null || model.HEAD_ID_D == String.Empty)
                        {
                            ModelState.AddModelError("HEAD_ID_D", "Sub Transaction Type (Deduction)");
                        }

                        else
                        {
                            // to check if correct entry as is operational and is required after porting flag
                            string correctionStatus = objCommon.ValidateHeadForCorrection(Convert.ToInt16(model.HEAD_ID_D.Split('$')[0]), 0, "E");

                            if (correctionStatus != "1")
                            {
                                ModelState.AddModelError("HEAD_ID_D", "Sub Transaction Type (Deduction) is Required");
                            }
                            else
                            {
                                if (ModelState.ContainsKey("HEAD_ID_D"))
                                    ModelState["HEAD_ID_D"].Errors.Clear();
                            }
                        }


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



                        int ContractorSupplierRequired = masterDetails.MAST_CON_ID.HasValue ? masterDetails.MAST_CON_ID.Value : 0;
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

                            amountModel = objPaymentBAL.CalculatePaymentAmounts(objparams.BILL_ID);

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
                                    ModelState.AddModelError("AMOUNT_Q", "Invalid Deduction Amount.its greater than remaining Deduction amount to enter");
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

                    if (masterDetails.BILL_FINALIZED == "Y")
                    {

                        return Json(new { Success = false, Bill_ID = "-1", disblehead = disblehead, head = model.HEAD_ID_P });

                    }


                    if (!paymentDeduction.Equals("D"))
                    {
                        objparams.TXN_ID = Convert.ToInt16(model.HEAD_ID_P.Split('$')[0]);

                        parametes = common.getDetailsDesignParamForPayment(objparams);

                        if (parametes.SHOW_CON_AT_TRANSACTION)
                        {
                            model.CON_ID = model.MAST_CON_ID_CON;

                        }
                        else
                        {
                            //for details master contractor id get it from master table (used for opening balance) 
                            model.CON_ID = masterDetails.MAST_CON_ID;
                        }


                        /* if (parametes.SUPPLIER_REQ.Trim() == "Y" || parametes.CON_REQ.Trim() == "Y")
                         {
                             model.MAST_CON_ID = masterDetails.MAST_CON_ID;
                         }
                         else
                         {
                             model.MAST_CON_ID = null;
                         }*/


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

                        if (parametes.AGREEMENT_REQ.Trim() == "Y" && parametes.SUPPLIER_REQ.Trim() == "Y")
                        {
                            model.IMS_AGREEMENT_CODE = model.IMS_AGREEMENT_CODE_S;
                        }

                    }
                    else
                    {
                        model.IMS_AGREEMENT_CODE = model.IMS_AGREEMENT_CODE_DED;
                        model.CON_ID = masterDetails.MAST_CON_ID;
                    }

                    bool result = objPaymentBAL.AddEditTransactionDeductionPaymentDetails(model, paymentDeduction.Equals("D") ? "D" : "T", objparams.BILL_ID, "E", tranNumber);

                    String encrptedBill_Id = URLEncrypt.EncryptParameters(new string[] { objparams.BILL_ID.ToString() });

                    ModelState.Clear();

                    return Json(new { Success = result, Bill_ID = encrptedBill_Id, disblehead = disblehead, head = model.HEAD_ID_P });
                    #endregion  valid model
                }
                else
                {
                    #region model error

                    objparams.BILL_TYPE = masterDetails.BILL_TYPE;
                    objparams.FUND_TYPE = masterDetails.FUND_TYPE;
                    objparams.LVL_ID = masterDetails.LVL_ID;
                    objparams.ADMIN_ND_CODE = masterDetails.ADMIN_ND_CODE;
                    objparams.MAST_CONT_ID = masterDetails.MAST_CON_ID.HasValue ? masterDetails.MAST_CON_ID.Value : 0;
                    objparams.DISTRICT_CODE = PMGSYSession.Current.DistrictCode;
                    objparams.TXN_ID = masterDetails.TXN_ID;
                    objparams.STATE_CODE = Convert.ToInt16(PMGSYSession.Current.StateCode);
                    objparams.DISTRICT_CODE = Convert.ToInt16(PMGSYSession.Current.DistrictCode);
                    objparams.SANC_YEAR = Convert.ToInt16(model.SANCTION_YEAR);
                    objparams.OP_MODE = "E";
                    //objparams.DISTRICT_CODE = 508;

                    model.HEAD_ID_P = transactionDetails.TXN_ID + "$" + dbContext.ACC_MASTER_TXN.Where(m => m.TXN_ID == transactionDetails.TXN_ID).Select(m => m.CASH_CHQ).FirstOrDefault().Trim();

                    List<SelectListItem> subHeadList = new List<SelectListItem>();

                    subHeadList = common.PopulateTransactions(objparams);

                    model.HeadId_P = new List<SelectListItem>(subHeadList);

                    //for remittance payment set the payment transaction same as remittance office set in master payment

                    if (masterDetails.REMIT_TYPE != null)
                    {

                        foreach (var item in subHeadList)
                        {
                            if (item.Value != string.Empty)
                            {
                                //get the head id 
                                int head = Convert.ToInt32(item.Value.Split('$')[0]);

                                //get the order of it from master txn table 
                                int orderOfTxn = dbContext.ACC_MASTER_TXN.Where(x => x.TXN_ID == head).Select(x => x.TXN_ORDER).FirstOrDefault();

                                //check if order matches or remove from subtransaction list
                                if (orderOfTxn != masterDetails.REMIT_TYPE)
                                {
                                    model.HeadId_P.Remove(item);
                                }
                            }
                        }
                    }

                    subHeadList.Clear();

                    subHeadList = null;
                    //find out for deduction
                    objparams.BILL_TYPE = "D";
                    model.HEAD_ID_D = transactionDetails.TXN_ID + "$" + dbContext.ACC_MASTER_TXN.Where(m => m.TXN_ID == transactionDetails.TXN_ID).Select(m => m.CASH_CHQ).FirstOrDefault().Trim();
                    model.HeadId_D = common.PopulateTransactions(objparams);

                    model.IMS_AGREEMENT_CODE_S = transactionDetails.IMS_AGREEMENT_CODE;
                    model.IMS_AGREEMENT_CODE_C = transactionDetails.IMS_AGREEMENT_CODE;
                    model.IMS_PR_ROAD_CODE = transactionDetails.IMS_PR_ROAD_CODE;
                    objparams.AGREEMENT_CODE = transactionDetails.IMS_AGREEMENT_CODE.HasValue ? transactionDetails.IMS_AGREEMENT_CODE.Value : 0;

                    model.FINAL_PAYMENT = transactionDetails.FINAL_PAYMENT;

                    model.AGREEMENT_DED = common.PopulateAgreement(objparams);


                    //if deduction payment
                    if (paymentDeduction.Equals("D"))
                    {

                        model.AGREEMENT_S = generalList;
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
                        if (parametes.SHOW_CON_AT_TRANSACTION)
                        {
                            objparams.MAST_CON_SUP_FLAG = "C";
                            model.mast_CON_ID_CON1 = common.PopulateContractorSupplier(objparams);

                        }
                        else
                        {
                            model.mast_CON_ID_CON1 = generalList;
                        }

                        if (parametes.ROAD_REQ.Trim() == "Y")
                        {
                            model.IMS_PR_ROAD_CODEList = common.PopulateRoad(objparams);
                        }
                        else
                        {
                            model.IMS_PR_ROAD_CODEList = generalList;
                        }

                        if (parametes.PIU_REQ.Trim() == "Y")
                        {
                            model.MAST_DPIU_CODEList = common.PopulateDPIU(objparams);
                        }
                        else
                        {
                            model.MAST_DPIU_CODEList = generalList;
                        }

                        if (parametes.AGREEMENT_REQ.Trim() == "Y" && parametes.CON_REQ.Trim() == "Y")
                        {
                            model.AGREEMENT_C = common.PopulateAgreement(objparams);
                        }
                        else
                        {
                            model.AGREEMENT_C = generalList;

                        }

                        if (parametes.AGREEMENT_REQ.Trim() == "Y" && parametes.SUPPLIER_REQ.Trim() == "Y")
                        {
                            model.AGREEMENT_S = common.PopulateAgreement(objparams);
                        }
                        else
                        {
                            model.AGREEMENT_S = generalList;
                        }

                        if (parametes.YEAR_REQ.Trim() == "Y")
                        {

                            model.IMS_SANCTION_YEAR_List = common.PopulateSancYear(objparams);
                        }
                        else
                        {
                            generalList.Clear();
                            generalList.Add(new SelectListItem { Selected = true, Text = "--Select--", Value = "0" });
                            model.IMS_SANCTION_YEAR_List = generalList;
                        }
                        if (parametes.PKG_REQ.Trim() == "Y")
                        {
                            model.IMS_SANCTION_PACKAGE_List = common.PopulatePackage(objparams);
                        }
                        else
                        {
                            generalList.Clear();
                            generalList.Add(new SelectListItem { Selected = true, Text = "--Select--", Value = "0" });
                            model.IMS_SANCTION_PACKAGE_List = generalList;
                        }


                    }

                    if (obj.DED_REQ == "B")
                    {

                        ViewBag.DeductionScreenRequired = (masterDetails.CASH_AMOUNT != 0 && (masterDetails.CHQ_EPAY == "Q" || masterDetails.CHQ_EPAY == "E")) ? true : masterDetails.CHQ_EPAY == "C" ? true : false;
                        ViewBag.PaymentScreenRequired = masterDetails.CHQ_AMOUNT != 0 && (masterDetails.CHQ_EPAY == "Q" || masterDetails.CHQ_EPAY == "E") ? true : false;
                    }
                    else
                    {
                        ViewBag.PaymentScreenRequired = (masterDetails.CHQ_AMOUNT != 0 && (masterDetails.CHQ_EPAY == "Q" || masterDetails.CHQ_EPAY == "E")) || (masterDetails.CHQ_EPAY == "C") ? true : false;
                        ViewBag.DeductionScreenRequired = masterDetails.CASH_AMOUNT != 0 && (masterDetails.CHQ_EPAY == "Q" || masterDetails.CHQ_EPAY == "E") ? true : false;
                    }

                    ViewBag.urlparams = URLEncrypt.EncryptParameters(new string[] { objparams.BILL_ID.ToString() + "$" + tranNumber + "$" + paymentDeduction });

                    ViewBag.BillFinalized = masterDetails.BILL_FINALIZED;

                    ViewBag.Bill_ID = URLEncrypt.EncryptParameters(new string[] { objparams.BILL_ID.ToString() });

                    List<SelectListItem> List = new List<SelectListItem>();

                    List.Add(new SelectListItem { Text = "Yes", Value = "true" });
                    List.Add(new SelectListItem { Selected = true, Text = "No", Value = "false" });

                    model.final_pay = List;

                    return View("PartialTransaction", model);

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
        /// function to get letest amount to be entered,allready entered ,total amount to be entered 
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="hash"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        // [Audit]
        [HttpPost]
        public JsonResult GetAmountBalanceDetails(String parameter, String hash, String key)
        {
            Int64 BILL_ID = 0;

            try
            {
                if (!String.IsNullOrEmpty(parameter) && !String.IsNullOrEmpty(hash) && !String.IsNullOrEmpty(key))
                {
                    String[] urlParams = URLEncrypt.DecryptParameters(new String[] { parameter, hash, key });
                    if (urlParams.Length >= 1)
                    {

                        BILL_ID = Convert.ToInt64(urlParams[0]);

                    }

                    AmountCalculationModel amountModel = new AmountCalculationModel();

                    amountModel = objPaymentBAL.CalculatePaymentAmounts(BILL_ID);

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
                        CashPayment = amountModel.CashPayment,
                        TransactionId = amountModel.TransactionId,
                        IsDetailsEntered = amountModel.IsDetailsEntered,


                    });

                }
                else
                {
                    throw new Exception("Error while getting payment details..");
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
        /// Action to return the selected agreement number for the payment transaction 
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

                        return agreementNumber = objPaymentBAL.GetAgreemntNumberForVoucher(BILL_ID);

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
        /// action to finalize the voucher details
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="hash"></param>
        /// <param name="key"></param>
        /// <param name="id"> bool value to finalize or not</param>
        /// <returns> 1 for success ,-1 for invalid voucher</returns>
        [HttpPost]
        [Audit]
        public JsonResult FinalizeVoucher(String parameter, String hash, String key, string id)
        {
            Int64 BILL_ID = 0;
            bool FinalPayment = false;
            try
            {
                if (!String.IsNullOrEmpty(parameter) && !String.IsNullOrEmpty(hash) && !String.IsNullOrEmpty(key))
                {
                    String[] urlParams = URLEncrypt.DecryptParameters(new String[] { parameter, hash, key });
                    if (urlParams.Length >= 1)
                    {
                        String[] urlSplitParams = urlParams[0].Split('$');
                        BILL_ID = Convert.ToInt64(urlSplitParams[0]);
                        FinalPayment = Convert.ToBoolean(id);
                        bool isEpayVoucher = false;
                        bool isEremVoucher = false;
                        string voucherCodeAndStatus = String.Empty;

                        int result = objPaymentBAL.FinalizeVoucher(BILL_ID, FinalPayment);
                        //if finalization is complete then only 
                        if (result == 1)
                        {
                            //create the status so that if voucher is of type epayment;it will  be used to show the epay order
                            ACC_BILL_MASTER masterDetails = new ACC_BILL_MASTER();
                            masterDetails = objPaymentBAL.GetMasterPaymentDetails(BILL_ID);
                            isEpayVoucher = (masterDetails.CHQ_EPAY.Trim() == "E" && masterDetails.REMIT_TYPE == null) ? true : false;
                            isEremVoucher = (masterDetails.CHQ_EPAY.Trim() == "E" && masterDetails.REMIT_TYPE != null) ? true : false;
                            voucherCodeAndStatus = URLEncrypt.EncryptParameters(new string[] { BILL_ID.ToString() + "$N$E$Y" + "$" + ((masterDetails.CON_ACCOUNT_ID != null) ? masterDetails.CON_ACCOUNT_ID.Value : 0) });//PFMS 
                        }
                        return Json(new { Success = result, voucherCodeAndStatus = voucherCodeAndStatus, isEpayVoucher = isEpayVoucher, isEremVoucher = isEremVoucher });
                    }
                    else
                    {
                        throw new Exception("Error While finalizing the voucher.. ");
                    }

                }
                else
                {
                    throw new Exception("Error While finalizing the voucher.. ");
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

        /// <summary>
        /// action to get the dropdown options based on road selected
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="hash"></param>
        /// <param name="key"></param>
        /// <param name="id"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        [Audit]
        public JsonResult GetFinalPaymentDetails(String parameter, String hash, String key, string id)
        {
            Int64 BILL_ID = 0;
            Int32 subTxnID = 0;
            try
            {
                Int32 roadID = 0;

                //if else condition added by Abhishek kamble to get TxnID
                if (id.Split('$').Count() > 1)
                {
                    roadID = Convert.ToInt32(id.Split('$')[0]);

                    subTxnID = Convert.ToInt32(id.Split('$')[1]);
                }
                else
                {
                    roadID = Convert.ToInt32(id.Trim());
                }

                if (!String.IsNullOrEmpty(parameter) && !String.IsNullOrEmpty(hash) && !String.IsNullOrEmpty(key))
                {
                    String[] urlParams = URLEncrypt.DecryptParameters(new String[] { parameter, hash, key });
                    if (urlParams.Length >= 1)
                    {
                        String[] urlSplitParams = urlParams[0].Split('$');

                        BILL_ID = Convert.ToInt64(urlSplitParams[0]);

                        return Json(objPaymentBAL.GetFinalPaymentDetails(BILL_ID, roadID, subTxnID));

                    }
                    else
                    {
                        throw new Exception("Error While getting payment details.. ");
                    }
                }
                else
                {
                    throw new Exception("Error While getting payment details.. ");
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
        /// function to get the Epay number
        /// </summary>
        /// <param name="id">month and year</param>
        /// <returns></returns>
        [Audit]
        public JsonResult GetEpayNumber(String id)
        {
            try
            {
                Int16 month = 0;
                Int16 year = 0;

                month = Convert.ToInt16(id.Split('$')[0]);
                year = Convert.ToInt16(id.Split('$')[1]);

                return Json(objPaymentBAL.GetEpayNumber(month, year, PMGSYSession.Current.StateCode, PMGSYSession.Current.AdminNdCode));


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
        /// function to get eremittance number
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Audit]
        public JsonResult GetEremittanceNumber(String id)
        {
            try
            {
                Int16 month = 0;
                Int16 year = 0;

                month = Convert.ToInt16(id.Split('$')[0]);
                year = Convert.ToInt16(id.Split('$')[1]);

                return Json(objPaymentBAL.GetEremittanceNumber(month, year, PMGSYSession.Current.StateCode, PMGSYSession.Current.AdminNdCode));


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
        /// fucntion to populater the saction year
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Audit]
        public JsonResult populateSactionYear(String id)
        {
            try
            {
                //new change done by Vikram on 30-08-2013

                string[] param = id.Split('$');

                TransactionParams objparams = new TransactionParams();

                objparams.HEAD_ID = Convert.ToInt16(param[0]);

                objparams.STATE_CODE = Convert.ToInt16(PMGSYSession.Current.StateCode);

                objparams.DISTRICT_CODE = Convert.ToInt16(PMGSYSession.Current.DistrictCode);

                return Json(common.PopulateSancYear(objparams));


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
        /// function to populate saction packages
        /// </summary>
        /// <returns></returns>
        [Audit]
        public JsonResult PopulateSactionPackage(String id)
        {
            try
            {
                //new change done by Vikram on 30-08-2013

                string[] param = id.Split('$');

                TransactionParams objparams = new TransactionParams();

                objparams.STATE_CODE = Convert.ToInt16(PMGSYSession.Current.StateCode);

                objparams.DISTRICT_CODE = Convert.ToInt16(PMGSYSession.Current.DistrictCode);

                objparams.SANC_YEAR = Convert.ToInt16(param[0]);

                objparams.HEAD_ID = Convert.ToInt16(param[1]);  //new change

                return Json(common.PopulatePackage(objparams));


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
        /// function to populate road by package and year
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Audit]
        public JsonResult PopulateRoadbyPackageYear(string id)
        {

            try
            {
                //new change done by Vikram on 30-08-2013
                string[] param = id.Split('$');
                //end of change

                TransactionParams objparams = new TransactionParams();
                //objparams.PACKAGE_ID = id;
                objparams.PACKAGE_ID = param[0];
                objparams.HEAD_ID = Convert.ToInt16(param[1]);
                return Json(common.PopulateRoad(objparams));
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

        //GetVoucherPayment() added by vikram 28-8-2013
        [HttpPost]
        [Audit]
        public JsonResult GetVoucherPayment(String parameter, String hash, String key)
        {
            String[] decryptedParameters = null;
            Int64 billId = 0;
            string paymentType = string.Empty;
            objPaymentBAL = new PaymentBAL();
            decimal? paymentValue = 0;
            try
            {
                decryptedParameters = URLEncrypt.DecryptParameters(new string[] { parameter, hash, key });
                String[] param = decryptedParameters[0].Split('$');
                billId = Convert.ToInt64(param[0]);
                paymentValue = objPaymentBAL.GetVoucherPayment(billId, out paymentType);
                return Json(new { Payment = paymentValue, PaymentType = paymentType });
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return Json(string.Empty);
            }
        }

        #region Cheque Renewal

        [HttpPost]
        [Audit]
        public JsonResult ListChequeDetailsForRenewalByCheque(int? page, int? rows, string sidx, string sord, String id)
        {
            int chequeNumber = 0;
            try
            {

                chequeNumber = Convert.ToInt32(id);

                PaymentFilterModel objFilter = new PaymentFilterModel();
                long totalRecords;
                Int64 BILL_ID = 0;

                objFilter.page = Convert.ToInt32(page) - 1;
                objFilter.rows = Convert.ToInt32(rows);
                objFilter.sidx = sidx.ToString();
                objFilter.sord = sord.ToString();
                objFilter.Bill_type = "P";
                objFilter.AdminNdCode = PMGSYSession.Current.AdminNdCode;
                objFilter.FundType = PMGSYSession.Current.FundType;
                objFilter.LevelId = PMGSYSession.Current.LevelId;

                objFilter.BillId = chequeNumber;

                var jsonData = new
                {
                    rows = objPaymentBAL.ListChequeDetailsForRenewalbyCheque(objFilter, out totalRecords, out BILL_ID),
                    total = totalRecords <= objFilter.rows ? 1 : totalRecords / objFilter.rows + 1,
                    page = page,
                    records = totalRecords,
                    userdata = new { billId = URLEncrypt.EncryptParameters(new string[] { BILL_ID.ToString() }) }
                };
                return Json(jsonData);
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



        /// <summary>
        /// action to display from for cheque Renewal
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="hash"></param>
        /// <param name="key"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Audit]
        public ActionResult GetChequeRenew(String parameter, String hash, String key, string id)
        {
            ChequeRenewModel model = new ChequeRenewModel();

            Int64 BILL_ID = 0;
            String Chq_No = String.Empty;
            try
            {

                if (!String.IsNullOrEmpty(parameter) && !String.IsNullOrEmpty(hash) && !String.IsNullOrEmpty(key))
                {
                    String[] urlParams = URLEncrypt.DecryptParameters(new String[] { parameter, hash, key });
                    if (urlParams.Length >= 1)
                    {
                        String[] urlSplitParams = urlParams[0].Split('$');

                        BILL_ID = Convert.ToInt64(urlSplitParams[0]);

                        Chq_No = urlSplitParams[1].ToString();

                        ViewBag.Chq_No = Chq_No;

                        ViewBag.str_bill_id = URLEncrypt.EncryptParameters(new string[] { BILL_ID.ToString() + "$" + Chq_No });

                        ViewBag.LevelID = PMGSYSession.Current.LevelId;

                        //Added By Abhishek kamble 22-jan-2014 start
                        objPaymentBAL = new PaymentBAL();
                        common = new CommonFunctions();
                        ACC_BILL_MASTER billMasterModel = objPaymentBAL.GetMasterPaymentDetails(BILL_ID);
                        if (billMasterModel != null)
                        {
                            ViewBag.ChequeAmount = billMasterModel.CHQ_AMOUNT;
                            ViewBag.VoucherNumber = billMasterModel.BILL_NO;
                            ViewBag.VoucherDate = common.GetDateTimeToString(billMasterModel.BILL_DATE);
                        }
                        else
                        {
                            ViewBag.ChequeAmount = "-";
                            ViewBag.VoucherNumber = "-";
                            ViewBag.VoucherDate = "-";
                        }
                        //Added By Abhishek kamble 22-jan-2014 end                        

                        // ViewBag.availableCheques = objPaymentBAL.GetAllAvailableChequesArray(-1, PMGSYSession.Current.AdminNdCode, PMGSYSession.Current.FundType);
                        model.CHQ_CANCEL_RENEW = "R";

                        List<SelectListItem> CHQ_Book_ID = new List<SelectListItem>();
                        CHQ_Book_ID.Add(new SelectListItem { Text = "--Select--", Value = "0", Selected = true });
                        model.CHQ_Book_ID_List = CHQ_Book_ID;
                        //model.CURRENT_DATE = DateTime.Now.ToString("dd/MM/yyyy");

                        if (billMasterModel.CHQ_EPAY == "A")//Added By Abhishek for Advice No 6Apr2015
                        {
                            model.CHQ_EPAY = "A";
                        }
                        return View("ChequeRenew", model);
                    }
                    else
                    {
                        throw new Exception("Error While getting Cheque details.. ");
                    }
                }
                else
                {
                    List<SelectListItem> genral = new List<SelectListItem>();
                    genral.Add(new SelectListItem { Selected = true, Text = "--Select--", Value = "" });
                    ViewBag.str_bill_id = String.Empty;
                    model.CHQ_CANCEL_RENEW = "R";
                    model.CHQ_Book_ID_List = genral;
                    return View("ChequeRenew");
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

        /// <summary>
        /// function to get all finalized cheques
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public JsonResult GetAllFinalizedCheques(string id)
        {
            try
            {
                string chequeSeries = String.Empty;
                int chequebookID = 0;
                chequebookID = Convert.ToInt32(id);
                return Json(objPaymentBAL.GetAllFinalizedCheques(chequebookID, PMGSYSession.Current.AdminNdCode, PMGSYSession.Current.FundType, chequeSeries));

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


        /// <summary>
        /// action to renew cheques
        /// </summary>
        /// <param name="model"></param>
        /// <param name="parameter"></param>
        /// <param name="hash"></param>
        /// <param name="key"></param>
        /// <param name="id"></param>
        /// <returns>
        ///  json result of the status 
        /// </returns>
        [HttpPost]
        [Audit]
        [ValidateAntiForgeryToken]
        public ActionResult RenewCheque(ChequeRenewModel model, String parameter, String hash, String key, string id)
        {
            CommonFunctions objCommon = new CommonFunctions();
            try
            {
                Int64 BILL_ID = 0;
                String Chq_No = String.Empty;
                string result = string.Empty;


                if (!String.IsNullOrEmpty(parameter) && !String.IsNullOrEmpty(hash) && !String.IsNullOrEmpty(key))
                {
                    String[] urlParams = URLEncrypt.DecryptParameters(new String[] { parameter, hash, key });
                    if (urlParams.Length >= 1)
                    {
                        String[] urlSplitParams = urlParams[0].Split('$');

                        BILL_ID = Convert.ToInt64(urlSplitParams[0]);
                        Chq_No = urlSplitParams[1];

                        #region Server Side Validation


                        if (model.CHQ_DATE == null || model.CHQ_DATE == string.Empty)
                        {
                            ModelState.AddModelError("CHQ_DATE", "Cheque/Epay/Advice Date is Required");
                        }

                        if (model.CHQ_NO == null)
                        {
                            ModelState.AddModelError("CHQ_NO", "Cheque/Advice Number is Required");
                        }

                        //check for valid cheque book number only if DPIU
                        if (PMGSYSession.Current.LevelId == 5)
                        {
                            //If Condition Added By Abhishek kamble 6-Apr-2015 for Advice No
                            if (model.CHQ_EPAY == "A")
                            {
                                //if (objPaymentBAL.ValidateAdviceNoExist(model.CHQ_NO, 0))
                                //{
                                //    ModelState.AddModelError("CHQ_NO", "Advice No is already exist.");
                                //    //model.CHQ_EPAY = "A";
                                //}
                            }
                            else
                            {
                                if (model.CHQ_Book_ID == null || model.CHQ_Book_ID == 0)
                                {
                                    ModelState.AddModelError("CHQ_Book_ID", "Cheque series is Required");
                                }
                                else
                                {
                                    //old 
                                    //String[] availableCheques = objPaymentBAL.GetAllAvailableChequesArray(-1, PMGSYSession.Current.AdminNdCode, PMGSYSession.Current.FundType);
                                    //CHQ_Book_ID Modified by Abhihsek kamble 3Dec2014
                                    String[] availableCheques = objPaymentBAL.GetAllAvailableChequesArray(model.CHQ_Book_ID.Value, PMGSYSession.Current.AdminNdCode, PMGSYSession.Current.FundType);

                                    int pos = Array.IndexOf(availableCheques, model.CHQ_NO);
                                    if (pos == -1)
                                    {
                                        ModelState.AddModelError("CHQ_NO", "Invalid Cheque Number");
                                    }

                                }
                            }
                        }
                        else if (PMGSYSession.Current.LevelId == 4 && model.CHQ_Book_ID != 0)
                        {//@SRRDA level & check book id is selected

                            //old
                            //String[] availableCheques = objPaymentBAL.GetAllAvailableChequesArray(-1, PMGSYSession.Current.AdminNdCode, PMGSYSession.Current.FundType);
                            //CHQ_Book_ID Modified by Abhihsek kamble 3Dec2014                            
                            String[] availableCheques = objPaymentBAL.GetAllAvailableChequesArray(model.CHQ_Book_ID.Value, PMGSYSession.Current.AdminNdCode, PMGSYSession.Current.FundType);
                            int pos = Array.IndexOf(availableCheques, model.CHQ_NO);
                            if (pos == -1)
                            {
                                ModelState.AddModelError("CHQ_NO", "Invalid Cheque Number");
                            }

                        }
                        else if (PMGSYSession.Current.LevelId == 4 && model.CHQ_Book_ID == 0)
                        {


                            //@SRRDA level check if chequ already entered
                            bool chequeAllreadyIssued = false;
                            chequeAllreadyIssued = objPaymentBAL.IschequeIssued(model.CHQ_NO, "A", 0);

                            if (chequeAllreadyIssued)
                            {
                                ModelState.AddModelError("CHQ_NO", "This Cheque is already issued");
                            }
                        }


                        #endregion

                        if (objCommon.GetStringToDateTime(model.CHQ_DATE) > objCommon.GetStringToDateTime(model.BILL_DATE))
                        {
                            ModelState.AddModelError("CHQ_DATE", "Voucher Date should be greater than or equal to Cheque Date");
                        }

                        if (ModelState.IsValid)
                        {
                            #region Monthly Closing Validation
                            if (objCommon.GetStringToDateTime(model.BILL_DATE).Month != 0 && objCommon.GetStringToDateTime(model.BILL_DATE).Month < 13 && objCommon.GetStringToDateTime(model.BILL_DATE).Year != 0)
                            {
                                string monthlyClosingStatus = string.Empty;
                                String errMessage = String.Empty;
                                monthlyClosingStatus = objCommon.MonthlyClosingValidation((short)objCommon.GetStringToDateTime(model.BILL_DATE).Month, (short)objCommon.GetStringToDateTime(model.BILL_DATE).Year, PMGSYSession.Current.FundType, PMGSYSession.Current.LevelId, PMGSYSession.Current.AdminNdCode, ref errMessage);

                                if (monthlyClosingStatus.Equals("-111"))
                                {
                                    ModelState.AddModelError("BILL_MONTH", errMessage);
                                }
                                if (monthlyClosingStatus.Equals("-222"))
                                {
                                    ModelState.AddModelError("BILL_MONTH", "Month is already closed.");
                                    errMessage = "Month is already closed.";
                                }
                                if (!(monthlyClosingStatus.Equals("1")))
                                {
                                    return Json(new
                                    {
                                        Success = false,
                                        Result = monthlyClosingStatus == "-111" ? "-11" : "-22",
                                        message = errMessage
                                    });
                                }
                            }
                            #endregion

                            result = objPaymentBAL.RenewCheque(BILL_ID, model);

                            return Json(new
                            {
                                Success = result.Equals("1") ? true : false,
                                Result = result,
                                message = result == "-999" ? "Cheque cannot be cancelled after " + ConfigurationManager.AppSettings["ChequeCancelRenewMonths"] + " months of cheque issue date." : ""
                            });
                        }
                        else
                        {

                            ViewBag.str_bill_id = URLEncrypt.EncryptParameters(new string[] { BILL_ID.ToString() + "$" + Chq_No });
                            ViewBag.Chq_No = Chq_No;
                            model.CHQ_Book_ID_List = objPaymentBAL.GetchequebookSeries(PMGSYSession.Current.AdminNdCode, PMGSYSession.Current.FundType, PMGSYSession.Current.LevelId);
                            model.CHQ_Book_ID = model.CHQ_Book_ID;

                            //added by abhishek kamble 10Mar2015 for Cheque issue date disp.
                            if (model.CHQ_Book_ID != null && model.CHQ_Book_ID != 0)
                            {
                                ViewBag.ChequeIssueDate = objPaymentBAL.GetChequeBookIssueDate(Convert.ToInt64(model.CHQ_Book_ID));
                            }

                            return View("ChequeRenew", model);

                        }
                    }
                    else
                    {
                        throw new Exception("Error While getting Cheque details.. ");
                    }
                }
                else
                {
                    throw new Exception("Error While getting Cheque details.. ");
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


        /// <summary>
        /// action to return cheque cancellation form
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public PartialViewResult CancelCheque()
        {
            ChequeCancellModel model = new ChequeCancellModel();

            //Below line is commented on 18-01-2022
            //model.CANCEL_FUND_TYPE_List = objPaymentBAL.populateFundTypeForCancellation(PMGSYSession.Current.FundType);
            //Below line is Added on 18-01-2022
            model.CANCEL_FUND_TYPE_List = objPaymentBAL.populateFundTypeForCancellation(PMGSYSession.Current.FundType, PMGSYSession.Current.LevelId);

            model.CURRENT_DATE = DateTime.Now.ToString("dd/MM/yyyy");

            return PartialView("ChequeCancelPartial", model);

        }

        [HttpPost]
        [Audit]
        [ValidateAntiForgeryToken]
        public ActionResult CancelCheque(ChequeCancellModel model, String parameter, String hash, String key, string id)
        {
            CommonFunctions objCommon = new CommonFunctions();
            try
            {
                Int64 BILL_ID = 0;
                String Chq_No = String.Empty;
                string result = string.Empty;


                if (!String.IsNullOrEmpty(parameter) && !String.IsNullOrEmpty(hash) && !String.IsNullOrEmpty(key))
                {
                    String[] urlParams = URLEncrypt.DecryptParameters(new String[] { parameter, hash, key });
                    if (urlParams.Length >= 1)
                    {
                        String[] urlSplitParams = urlParams[0].Split('$');

                        BILL_ID = Convert.ToInt64(urlSplitParams[0]);


                        if (ModelState.IsValid)
                        {
                            #region Monthly Closing Validation
                            if (objCommon.GetStringToDateTime(model.CHEQUE_CANCEL_DATE).Month != 0 && objCommon.GetStringToDateTime(model.CHEQUE_CANCEL_DATE).Month < 13 && objCommon.GetStringToDateTime(model.CHEQUE_CANCEL_DATE).Year != 0)
                            {
                                string monthlyClosingStatus = string.Empty;
                                String errMessage = String.Empty;
                                monthlyClosingStatus = objCommon.MonthlyClosingValidation((short)objCommon.GetStringToDateTime(model.CHEQUE_CANCEL_DATE).Month, (short)objCommon.GetStringToDateTime(model.CHEQUE_CANCEL_DATE).Year, PMGSYSession.Current.FundType, PMGSYSession.Current.LevelId, PMGSYSession.Current.AdminNdCode, ref errMessage);

                                if (monthlyClosingStatus.Equals("-111"))
                                {
                                    ModelState.AddModelError("BILL_MONTH", errMessage);
                                }
                                if (monthlyClosingStatus.Equals("-222"))
                                {
                                    ModelState.AddModelError("BILL_MONTH", "Month is already closed.");
                                    errMessage = "Month is already closed.";
                                }
                                if (monthlyClosingStatus.Equals("-111") || monthlyClosingStatus.Equals("-222"))
                                {
                                    return Json(new
                                    {
                                        Success = false,
                                        Result = monthlyClosingStatus == "-111" ? "-11" : "-22",
                                        message = errMessage
                                    });
                                }
                            }
                            #endregion

                            result = objPaymentBAL.CancelCheque(BILL_ID, model);
                            return Json(new
                            {
                                Success = result.Equals("1") ? true : false,
                                Result = result,
                                message = result == "-999" ? "Cheque cannot be cancelled after " + ConfigurationManager.AppSettings["ChequeCancelRenewMonths"] + " months of cheque issue date." : ""
                            });
                        }
                        else
                        {
                            //Below line is commented on 18-01-2022
                            //model.CANCEL_FUND_TYPE_List = objPaymentBAL.populateFundTypeForCancellation(PMGSYSession.Current.FundType);
                            //Below line is Added on 18-01-2022
                            model.CANCEL_FUND_TYPE_List = objPaymentBAL.populateFundTypeForCancellation(PMGSYSession.Current.FundType, PMGSYSession.Current.LevelId);
                            return View("ChequeCancelPartial", model);

                        }
                    }
                    else
                    {
                        throw new Exception("Error While getting Cheque details.. ");
                    }
                }
                else
                {
                    throw new Exception("Error While getting Cheque details.. ");
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



        #endregion

        #region EPAYMENT

        /// <summary>
        /// function to get epayment details
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="hash"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        [HttpPost]
        [Audit]
        public JsonResult GetEpaymentOrderDetails(String parameter, String hash, String key)
        {
            Int64 bill_id = 0;
            String FinalizedByAuthSig = string.Empty;
            try
            {
                if (!String.IsNullOrEmpty(parameter) && !String.IsNullOrEmpty(hash) && !String.IsNullOrEmpty(key))
                {
                    String[] urlParams = URLEncrypt.DecryptParameters(new String[] { parameter, hash, key });
                    if (urlParams.Length >= 1)
                    {
                        String[] urlSplitParams = urlParams[0].Split('$');

                        bill_id = Convert.ToInt64(urlSplitParams[0]);

                        FinalizedByAuthSig = urlSplitParams[1];

                        EpaymentOrderModel model = new EpaymentOrderModel();
                        DigSignBAL objBAL = new DigSignBAL();
                        RegisterDSCModel authModel = new RegisterDSCModel();
                        authModel = objBAL.GetDetailsToRegisterDSC();


                        model.BillID = bill_id;

                        //if not finalized by  auth sig show details of epayment 
                        if (FinalizedByAuthSig == "N")
                        {

                            model = objPaymentBAL.GetEpaymentDetails(bill_id);
                            model.AuthorisedSignName = authModel.NameAsPerCertificate;
                            model.AuthorisedSignMobile = authModel.Mobile;
                            return Json(new
                            {

                                Success = true,
                                EmailRecepient = model.EmailRecepient,
                                DPIUName = model.EpayDPIU + " ( District - " + model.DPIUName + " )",
                                STATEName = model.STATEName,
                                EmailDate = model.EmailDate,
                                Bankaddress = model.Bankaddress,
                                BankAcNumber = model.BankAcNumber,
                                EpayNumber = model.EpayNumber,
                                EpayDate = model.EpayDate,
                                EpayState = model.EpayState,
                                EpayDPIU = model.EpayDPIU,
                                EpayVNumber = model.EpayVNumber,
                                EpayVDate = model.EpayVDate,
                                EpayVPackages = model.EpayVPackages,
                                EpayConName = model.EpayConName,
                                EpayConAcNum = model.EpayConAcNum,
                                EpayConBankName = model.EpayConBankName,
                                EpayConBankIFSCCode = model.EpayConBankIFSCCode,
                                EpayAmount = model.EpayAmount,
                                EpayAmountInWord = model.EpayAmountInWord,
                                ShowPassword = "Y",
                                EpayContLegalHeirName = model.EpayContLegalHeirName,//added by Abhishek kamble 29-May-2014
                                FinalizedByAuthSig = FinalizedByAuthSig,
                                AuthSignName = model.AuthorisedSignName,
                                AuthSignPhoneNumber = model.AuthorisedSignMobile
                            });
                        }
                        // if finalized by  auth sig show details from mail sent i.e ACC_EPAY_MAIL_MASTER
                        else if (FinalizedByAuthSig == "Y")
                        {
                            model = objPaymentBAL.GetEpaymentDetailsFinalizedByAuthSig(bill_id);
                            model.AuthorisedSignName = authModel.NameAsPerCertificate;
                            model.AuthorisedSignMobile = authModel.Mobile;

                            return Json(new
                            {

                                Success = true,
                                EmailRecepient = model.EmailRecepient,
                                DPIUName = model.EpayDPIU + " ( District - " + model.DPIUName + " )", //model.DPIUName,
                                STATEName = model.STATEName,
                                EmailDate = model.EmailDate,
                                Bankaddress = model.Bankaddress,
                                BankAcNumber = model.BankAcNumber,
                                EpayNumber = model.EpayNumber,
                                EpayDate = model.EpayDate,
                                EpayState = model.EpayState,
                                EpayDPIU = model.EpayDPIU,
                                EpayVNumber = model.EpayVNumber,
                                EpayVDate = model.EpayVDate,
                                EpayVPackages = model.EpayVPackages,
                                EpayConName = model.EpayConName,
                                EpayConAcNum = model.EpayConAcNum,
                                EpayConBankName = model.EpayConBankName,
                                EpayConBankIFSCCode = model.EpayConBankIFSCCode,
                                EpayAmount = model.EpayAmount,
                                EpayAmountInWord = model.EpayAmountInWord,
                                ShowPassword = "N",
                                EpayContLegalHeirName = model.EpayContLegalHeirName,//added by Abhishek kamble 29-May-2014
                                FinalizedByAuthSig = FinalizedByAuthSig,
                                AuthSignName = model.AuthorisedSignName,
                                AuthSignPhoneNumber = model.AuthorisedSignMobile
                            });

                        }
                        else
                        {

                            throw new Exception("Error while getting epayment details");
                        }

                    }
                    else
                    {
                        throw new Exception("Error While getting Epayment order Details.. ");
                    }
                }
                else
                {
                    throw new Exception("Error While getting order Details.. ");
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





        [HttpPost]
        [Audit]
        public ActionResult GetEpaymentDetailsForSigning(String parameter, String hash, String key)
        {
            PaymentDAL paymentDAL = new PaymentDAL();
            Int64 bill_id = 0;
            String FinalizedByAuthSig = string.Empty;
            int conAccountId = 0;
            try
            {
                if (!String.IsNullOrEmpty(parameter) && !String.IsNullOrEmpty(hash) && !String.IsNullOrEmpty(key))
                {
                    String[] urlParams = URLEncrypt.DecryptParameters(new String[] { parameter, hash, key });
                    if (urlParams.Length >= 1)
                    {
                        String[] urlSplitParams = urlParams[0].Split('$');

                        bill_id = Convert.ToInt64(urlSplitParams[0]);
                        FinalizedByAuthSig = urlSplitParams[1];
                        string strEpayErem = urlSplitParams[2].ToString().Trim();
                        string BillFinalized = urlSplitParams[3].ToString().Trim();

                        //Below line added on 21-04-2023
                        PMGSYEntities dbContext = new PMGSYEntities();
                        ViewBag.TxnId = dbContext.ACC_BILL_MASTER.Where(x => x.BILL_ID == bill_id).Select(x => x.TXN_ID).FirstOrDefault();

                        if (urlSplitParams.Length > 4)
                        {
                            conAccountId = Convert.ToInt32(urlSplitParams[4]);
                        }
                        DigSignBAL objBAL = new DigSignBAL();
                        RegisterDSCModel authModel = new RegisterDSCModel();
                        authModel = objBAL.GetDetailsToRegisterDSC();


                        EpaymentOrderModel model = new EpaymentOrderModel();
                        model.BillID = bill_id;

                        string fileName = string.Empty;
                        //if not finalized by  auth sig show details of epayment 
                        if (FinalizedByAuthSig == "N")
                        {

                            model = (conAccountId == 0) ? paymentDAL.GetEpaymentDetails(bill_id) : paymentDAL.GetEpaymentDetails1(bill_id, conAccountId);
                            model.AuthorisedSignName = authModel.NameAsPerCertificate;
                            model.AuthorisedSignMobile = authModel.Mobile;

                            if (BillFinalized.Equals("Y"))
                            {
                                if (PMGSYSession.Current.RoleCode == 26)
                                {
                                    model.ShowPassword = "Y";
                                    fileName = "PMGSY_" + model.StateCode + model.DPIUCode + "_Epayment_" + model.EpayNumber.Replace('/', '-') + "_" + DateTime.Now.ToString
                                        ("ddMMyyyy") + "_" + DateTime.Now.ToString("HHmmss") + "_" + bill_id + ".pdf";
                                    model.PdfFileName = fileName;
                                    model.EmailDate = string.Empty;
                                }
                                else //if (PMGSYSession.Current.RoleCode == 21)
                                {
                                    model.ShowPassword = "N";
                                    model.PdfFileName = string.Empty;
                                    model.EmailDate = string.Empty;
                                }
                            }
                            else
                            {
                                model.ShowPassword = "N";
                                model.PdfFileName = string.Empty;
                                model.EmailDate = string.Empty;
                            }

                            return View("SignEpayment", model);


                        }
                        // if finalized by  auth sig show details from mail sent i.e ACC_EPAY_MAIL_MASTER
                        else if (FinalizedByAuthSig == "Y")
                        {
                            //model = objPaymentBAL.GetEpaymentDetailsFinalizedByAuthSig(bill_id);
                            model = (conAccountId == 0) ? paymentDAL.GetEpaymentDetails(bill_id) : paymentDAL.GetEpaymentDetails1(bill_id, conAccountId);
                            model.AuthorisedSignName = authModel.NameAsPerCertificate;
                            model.AuthorisedSignMobile = authModel.Mobile;
                            model.ShowPassword = "N";

                            if (PMGSYSession.Current.RoleCode == 26)
                            {
                                fileName = "PMGSY_" + model.StateCode + model.DPIUCode + "_Epayment_" + model.EpayNumber.Replace('/', '-') + "_" + DateTime.Now.ToString("ddMMyyyy") + "_" + DateTime.Now.ToString("HHmmss") + "_" + bill_id + ".pdf";
                                model.PdfFileName = fileName;
                            }
                            else
                            {
                                model.ShowPassword = "N";
                                model.PdfFileName = string.Empty;

                            }
                            return View("SignEpayment", model);

                        }
                        else
                        {

                            throw new Exception("Error while getting epayment details");
                        }

                    }
                    else
                    {
                        throw new Exception("Error While getting Epayment order Details.. ");
                    }
                }
                else
                {
                    throw new Exception("Error While getting order Details.. ");
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

        [HttpPost]
        [Audit]
        public ActionResult SignEpaymentPDF(String parameter, String hash, String key)
        {
            Int64 bill_id = 0;
            String FinalizedByAuthSig = string.Empty;
            PaymentDAL objDAL = new PaymentDAL();
            int accountId = 0;
            try
            {
                if (!String.IsNullOrEmpty(parameter) && !String.IsNullOrEmpty(hash) && !String.IsNullOrEmpty(key))
                {
                    String[] urlParams = URLEncrypt.DecryptParameters(new String[] { parameter, hash, key });
                    if (urlParams.Length >= 1)
                    {
                        String[] urlSplitParams = urlParams[0].Split('$');

                        bill_id = Convert.ToInt64(urlSplitParams[0]);
                        FinalizedByAuthSig = urlSplitParams[1];
                        string strEpayErem = urlSplitParams[2].ToString().Trim();
                        string BillFinalized = urlSplitParams[3].ToString().Trim();
                        EpaymentOrderModel model = new EpaymentOrderModel();
                        //model = objPaymentBAL.GetEpaymentDetails(bill_id);
                        ///Bank Details Accepted by PFMS 
                        accountId = objDAL.GetAccountIdByBillId(bill_id);
                        model = accountId > 0 ? objDAL.GetEpaymentDetails1(bill_id, accountId) : objPaymentBAL.GetEpaymentDetails(bill_id);

                        SignPDFModel smodel = new SignPDFModel();
                        smodel.BillID = bill_id;
                        smodel.PdfFileName = "PMGSY_" + model.StateCode + model.DPIUCode + "_Epayment_" + model.EpayNumber.Replace('/', '-') + "_" + DateTime.Now.ToString("ddMMyyyy") + "_" + DateTime.Now.ToString("HHmmss") + "_" + bill_id + ".pdf";
                        smodel.PiuName = model.DPIUName;

                        #region Security Deposit and Holding account region
                        PMGSYEntities dbContext = new PMGSYEntities();
                        int txnId = dbContext.ACC_BILL_MASTER.Where(x => x.BILL_ID == bill_id).Any() ? dbContext.ACC_BILL_MASTER.Where(x => x.BILL_ID == bill_id).Select(x => x.TXN_ID).First() : 0;
                        if (txnId == 3187)
                        {
                            return View("SignEpaymentPDFHolding", smodel);
                        }

                        #endregion


                        return View("SignEpaymentPDF", smodel);
                    }
                    else
                    {
                        throw new Exception("Error While getting Details.. ");
                    }
                }
                else
                {
                    throw new Exception("Error While getting Details.. ");
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

        //Below method added on 10-05-2023
        [HttpPost]
        [Audit]
        public ActionResult SignEpaymentPDFHolding(String parameter, String hash, String key)
        {
            Int64 bill_id = 0;
            String FinalizedByAuthSig = string.Empty;
            PaymentDAL objDAL = new PaymentDAL();
            int accountId = 0;
            try
            {
                if (!String.IsNullOrEmpty(parameter) && !String.IsNullOrEmpty(hash) && !String.IsNullOrEmpty(key))
                {
                    String[] urlParams = URLEncrypt.DecryptParameters(new String[] { parameter, hash, key });
                    if (urlParams.Length >= 1)
                    {
                        String[] urlSplitParams = urlParams[0].Split('$');

                        bill_id = Convert.ToInt64(urlSplitParams[0]);
                        FinalizedByAuthSig = urlSplitParams[1];
                        string strEpayErem = urlSplitParams[2].ToString().Trim();
                        string BillFinalized = urlSplitParams[3].ToString().Trim();
                        EpaymentOrderModel model = new EpaymentOrderModel();
                        //model = objPaymentBAL.GetEpaymentDetails(bill_id);
                        ///Bank Details Accepted by PFMS 
                        accountId = objDAL.GetAccountIdByBillId(bill_id);
                        model = accountId > 0 ? objDAL.GetEpaymentDetails1(bill_id, accountId) : objPaymentBAL.GetEpaymentDetails(bill_id);

                        SignPDFModel smodel = new SignPDFModel();
                        smodel.BillID = bill_id;
                        smodel.PdfFileName = "PMGSY_" + model.StateCode + model.DPIUCode + "_Epayment_" + model.EpayNumber.Replace('/', '-') + "_" + DateTime.Now.ToString("ddMMyyyy") + "_" + DateTime.Now.ToString("HHmmss") + "_" + bill_id + ".pdf";
                        smodel.PiuName = model.DPIUName;

                        return View("SignEpaymentPDFHolding", smodel);
                    }
                    else
                    {
                        throw new Exception("Error While getting Details.. ");
                    }
                }
                else
                {
                    throw new Exception("Error While getting Details.. ");
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

        [HttpPost]
        [Audit]
        public ActionResult SignERemtPDF(String parameter, String hash, String key)
        {
            Int64 bill_id = 0;
            String FinalizedByAuthSig = string.Empty;
            try
            {
                if (!String.IsNullOrEmpty(parameter) && !String.IsNullOrEmpty(hash) && !String.IsNullOrEmpty(key))
                {
                    String[] urlParams = URLEncrypt.DecryptParameters(new String[] { parameter, hash, key });
                    if (urlParams.Length >= 1)
                    {
                        String[] urlSplitParams = urlParams[0].Split('$');

                        bill_id = Convert.ToInt64(urlSplitParams[0]);
                        FinalizedByAuthSig = urlSplitParams[1];
                        string strEpayErem = urlSplitParams[2].ToString().Trim();
                        string BillFinalized = urlSplitParams[3].ToString().Trim();
                        EremittnaceOrderModel model = new EremittnaceOrderModel();
                        model = objPaymentBAL.GetEremittanceDetails(bill_id);

                        SignPDFModel smodel = new SignPDFModel();
                        smodel.BillID = bill_id;
                        smodel.PdfFileName = "PMGSY_" + model.StateCode + model.DPIUCode + "_ERemit_" + model.EpayNumber.Replace('/', '-') + "_" + DateTime.Now.ToString("ddMMyyyy") + "_" + DateTime.Now.ToString("HHmmss") + "_" + bill_id + ".pdf";
                        smodel.PiuName = model.DPIUName;
                        return View("SignERemPDF", smodel);
                    }
                    else
                    {
                        throw new Exception("Error While getting Details.. ");
                    }
                }
                else
                {
                    throw new Exception("Error While getting Details.. ");
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


        [HttpPost]
        [Audit]
        public ActionResult GetERemDetailsForSigning(String parameter, String hash, String key)
        {
            Int64 bill_id = 0;
            String FinalizedByAuthSig = string.Empty;
            try
            {
                if (!String.IsNullOrEmpty(parameter) && !String.IsNullOrEmpty(hash) && !String.IsNullOrEmpty(key))
                {
                    String[] urlParams = URLEncrypt.DecryptParameters(new String[] { parameter, hash, key });
                    if (urlParams.Length >= 1)
                    {
                        String[] urlSplitParams = urlParams[0].Split('$');

                        bill_id = Convert.ToInt64(urlSplitParams[0]);
                        FinalizedByAuthSig = urlSplitParams[1];
                        string strEpayErem = urlSplitParams[2].ToString().Trim();
                        string BillFinalized = urlSplitParams[3].ToString().Trim();

                        DigSignBAL objBAL = new DigSignBAL();
                        RegisterDSCModel authModel = new RegisterDSCModel();
                        authModel = objBAL.GetDetailsToRegisterDSC();


                        EremittnaceOrderModel model = new EremittnaceOrderModel();


                        string fileName = string.Empty;
                        //if not finalized by  auth sig show details of epayment 
                        if (FinalizedByAuthSig == "N")
                        {

                            model = objPaymentBAL.GetEremittanceDetails(bill_id);
                            model.BillID = bill_id;
                            model.AuthorisedSignName = authModel.NameAsPerCertificate;
                            model.AuthorisedSignMobile = authModel.Mobile;
                            if (BillFinalized.Equals("Y"))
                            {
                                if (PMGSYSession.Current.RoleCode == 26)
                                {
                                    model.ShowPassword = "Y";
                                    fileName = "PMGSY_" + model.StateCode + model.DPIUCode + "_ERemit_" + model.EpayNumber.Replace('/', '-') + "_" + DateTime.Now.ToString("ddMMyyyy") + "_" + DateTime.Now.ToString("HHmmss") + "_" + bill_id + ".pdf";
                                    model.PdfFileName = fileName;
                                    model.EmailDate = string.Empty;
                                }
                                else //if (PMGSYSession.Current.RoleCode == 21)
                                {
                                    model.ShowPassword = "N";
                                    model.PdfFileName = string.Empty;
                                    model.EmailDate = string.Empty;
                                }
                            }
                            else
                            {
                                model.ShowPassword = "N";
                                model.PdfFileName = string.Empty;
                                model.EmailDate = string.Empty;
                            }
                            return View("SignERem", model);
                        }
                        // if finalized by  auth sig show details from mail sent i.e ACC_EPAY_MAIL_MASTER
                        else if (FinalizedByAuthSig == "Y")
                        {
                            model = objPaymentBAL.GetEremittanceDetails(bill_id);
                            model.BillID = bill_id;
                            model.AuthorisedSignName = authModel.NameAsPerCertificate;
                            model.AuthorisedSignMobile = authModel.Mobile;
                            model.ShowPassword = "N";
                            if (PMGSYSession.Current.RoleCode == 26)
                            {
                                fileName = "PMGSY_" + model.StateCode + model.DPIUCode + "_ERemit_" + model.EpayNumber.Replace('/', '-') + "_" + DateTime.Now.ToString("ddMMyyyy") + "_" + DateTime.Now.ToString("HHmmss") + "_" + bill_id + ".pdf";
                                model.PdfFileName = fileName;
                            }
                            else
                            {
                                model.ShowPassword = "N";
                                model.PdfFileName = string.Empty;

                            }
                            return View("SignERem", model);

                        }
                        else
                        {

                            throw new Exception("Error while getting epayment details");
                        }

                    }
                    else
                    {
                        throw new Exception("Error While getting Epayment order Details.. ");
                    }
                }
                else
                {
                    throw new Exception("Error While getting order Details.. ");
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
        /// action to diaplay the eremitance order 
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="hash"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        [HttpPost]
        [Audit]
        public ActionResult GetERemOrderDetails(String parameter, String hash, String key)
        {
            Int64 bill_id = 0;
            String FinalizedByAuthSig = string.Empty;
            try
            {
                if (!String.IsNullOrEmpty(parameter) && !String.IsNullOrEmpty(hash) && !String.IsNullOrEmpty(key))
                {
                    String[] urlParams = URLEncrypt.DecryptParameters(new String[] { parameter, hash, key });
                    if (urlParams.Length >= 1)
                    {
                        String[] urlSplitParams = urlParams[0].Split('$');

                        bill_id = Convert.ToInt64(urlSplitParams[0]);

                        FinalizedByAuthSig = urlSplitParams[1];

                        EremittnaceOrderModel model = new EremittnaceOrderModel();
                        DigSignBAL objBAL = new DigSignBAL();
                        RegisterDSCModel authModel = new RegisterDSCModel();
                        authModel = objBAL.GetDetailsToRegisterDSC();

                        //if not finalized by  auth sig show details of eremitance 
                        if (FinalizedByAuthSig == "N")
                        {

                            model = objPaymentBAL.GetEremittanceDetails(bill_id);
                            model.ShowPasswordRow = "Y";
                            model.AuthorisedSignName = authModel.NameAsPerCertificate;
                            model.AuthorisedSignMobile = authModel.Mobile;
                            return View("EremittanceOrder", model);

                        }
                        // if finalized by  auth sig show details from mail sent i.e ACC_EPAY_MAIL_MASTER
                        else if (FinalizedByAuthSig == "Y")
                        {
                            model = objPaymentBAL.GetEremittanceDetailsFinalizedByAuthSig(bill_id);
                            model.ShowPasswordRow = "N";
                            model.AuthorisedSignName = authModel.NameAsPerCertificate;
                            model.AuthorisedSignMobile = authModel.Mobile;
                            return View("EremittanceOrder", model);

                        }
                        else
                        {

                            throw new Exception("Error while getting epayment details");
                        }

                    }
                    else
                    {
                        throw new Exception("Error While getting Epayment order Details.. ");
                    }
                }
                else
                {
                    throw new Exception("Error While getting order Details.. ");
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
        /// function to show epayment list at Authorized signatory page
        /// </summary>
        /// <returns></returns>
        [Audit]
        public ActionResult GetEpayList(String parameter, String hash, String key)
        {
            ReatDAL reatDAL = new ReatDAL();
            PFMSDAL1 pfmsDAL = new PFMSDAL1();
            PMGSYEntities dbContext = new PMGSYEntities();

            TransactionParams objparams = new TransactionParams();
            ViewBag.ModuleType = "D";

            try
            {
                DigSignBAL objBAL = new DigSignBAL();
                RegisterDSCModel modelDSC = new RegisterDSCModel();
                modelDSC = objBAL.GetDetailsToRegisterDSC();
                if (modelDSC.NodalOfficerCode == 0)
                {
                    modelDSC.IsAlreadyRegistered = 0;
                }
                else
                {
                    if (objBAL.IsAlreadyRegistered(modelDSC.NodalOfficerCode))
                    {
                        modelDSC.IsAlreadyRegistered = 1;

                    }
                    else
                    {
                        modelDSC.IsAlreadyRegistered = 2;
                    }
                }

                if (modelDSC.IsAlreadyRegistered == 1)
                {


                    ListModel model = new ListModel();
                    // model.billid = 0;

                    objparams.BILL_TYPE = "P";
                    objparams.FUND_TYPE = PMGSYSession.Current.FundType;
                    objparams.LVL_ID = PMGSYSession.Current.LevelId;
                    objparams.ADMIN_ND_CODE = PMGSYSession.Current.AdminNdCode;
                    objparams.DISTRICT_CODE = PMGSYSession.Current.DistrictCode;

                    List<SelectListItem> monthList = common.PopulateMonths(DateTime.Now.Month);
                    ViewData["months"] = monthList;

                    List<SelectListItem> yearList = common.PopulateYears(DateTime.Now.Year);
                    ViewData["year"] = yearList;

                    List<SelectListItem> transactionType = common.PopulateTransactions(objparams);
                    ViewData["TXN_ID"] = transactionType;



                    ViewBag.SessionSalt = Session["SessionSalt"].ToString();
                    ViewBag.IsAlreadyRegistered = "1";

                    //ViewBag.IsPaymentEnabled = pfmsDAL.ValidateSamplePayment();
                    ViewBag.IsPaymentEnabled = false;

                    ViewBag.IsReatPaymentEnabled = reatDAL.ValidateSamplePayment();

                    return View("EpayList");
                }
                else
                {
                    ViewBag.IsAlreadyRegistered = modelDSC.IsAlreadyRegistered.ToString();
                    return View("EpayList");
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

        //Below method added on 10-05-2023
        [Audit]
        public ActionResult GetEPayListHolding(String parameter, String hash, String key)
        {
            ReatDAL reatDAL = new ReatDAL();
            PFMSDAL1 pfmsDAL = new PFMSDAL1();
            PMGSYEntities dbContext = new PMGSYEntities();

            TransactionParams objparams = new TransactionParams();

            ViewBag.ModuleType = "R";

            //ViewBag.ChkHoldSecDepRadio = "D";

            #region Security Deposit and Holding

            string PdfKey = "";
            PdfKey = Request.Params["pdfKey"];
            String[] urlSplitParams = PdfKey.Split('$');
            Int64 bill_ID = Convert.ToInt64(urlSplitParams[0]);

            int txnId = dbContext.ACC_BILL_MASTER.Where(x => x.BILL_ID == bill_ID).Any() ? dbContext.ACC_BILL_MASTER.Where(x => x.BILL_ID == bill_ID).Select(x => x.TXN_ID).First() : 0;
            if (txnId != 3187)
            {
                ViewBag.ChkHoldSecDepRadio = "D";
            }

            #endregion


            try
            {
                DigSignBAL objBAL = new DigSignBAL();
                RegisterDSCModel modelDSC = new RegisterDSCModel();
                modelDSC = objBAL.GetDetailsToRegisterDSC();
                if (modelDSC.NodalOfficerCode == 0)
                {
                    modelDSC.IsAlreadyRegistered = 0;
                }
                else
                {
                    if (objBAL.IsAlreadyRegistered(modelDSC.NodalOfficerCode))
                    {
                        modelDSC.IsAlreadyRegistered = 1;

                    }
                    else
                    {
                        modelDSC.IsAlreadyRegistered = 2;
                    }
                }

                if (modelDSC.IsAlreadyRegistered == 1)
                {


                    ListModel model = new ListModel();
                    // model.billid = 0;

                    objparams.BILL_TYPE = "P";
                    objparams.FUND_TYPE = PMGSYSession.Current.FundType;
                    objparams.LVL_ID = PMGSYSession.Current.LevelId;
                    objparams.ADMIN_ND_CODE = PMGSYSession.Current.AdminNdCode;
                    objparams.DISTRICT_CODE = PMGSYSession.Current.DistrictCode;

                    List<SelectListItem> monthList = common.PopulateMonths(DateTime.Now.Month);
                    ViewData["months"] = monthList;

                    List<SelectListItem> yearList = common.PopulateYears(DateTime.Now.Year);
                    ViewData["year"] = yearList;

                    List<SelectListItem> transactionType = common.PopulateTransactions(objparams);
                    ViewData["TXN_ID"] = transactionType;



                    ViewBag.SessionSalt = Session["SessionSalt"].ToString();
                    ViewBag.IsAlreadyRegistered = "1";

                    //ViewBag.IsPaymentEnabled = pfmsDAL.ValidateSamplePayment();
                    ViewBag.IsPaymentEnabled = false;

                    ViewBag.IsReatPaymentEnabled = reatDAL.ValidateSamplePayment();

                    return View("EpayList");
                }
                else
                {
                    ViewBag.IsAlreadyRegistered = modelDSC.IsAlreadyRegistered.ToString();
                    return View("EpayList");
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

        [Audit]
        public ActionResult GetEpayListREAT(String parameter, String hash, String key)
        {
            ReatDAL reatDAL = new ReatDAL();
            PFMSDAL1 pfmsDAL = new PFMSDAL1();
            PMGSYEntities dbContext = new PMGSYEntities();

            TransactionParams objparams = new TransactionParams();
            ViewBag.ModuleType = "R";
            try
            {
                DigSignBAL objBAL = new DigSignBAL();
                RegisterDSCModel modelDSC = new RegisterDSCModel();
                modelDSC = objBAL.GetDetailsToRegisterDSC();
                //Below Condition modified on 26-11-2021
               // if ((modelDSC.NodalOfficerCode == 0) || (modelDSC.IsValidXmlDscRegisteredREAT == false) || (modelDSC.DscDeleteEnabled == true))
                if ((modelDSC.NodalOfficerCode == 0))
                {
                    modelDSC.IsAlreadyRegistered = 0;
                }
                else
                {
                    if (objBAL.IsAlreadyRegistered(modelDSC.NodalOfficerCode))
                    {
                        modelDSC.IsAlreadyRegistered = 1;
                        //modelDSC.IsAlreadyRegistered = modelDSC.IsValidXmlDscRegisteredREAT == false ? 0 : 1;
                        if (modelDSC.DSCDeregCheck == false)
                        {
                            modelDSC.IsAlreadyRegistered = 0;
                        }
                    }
                    else
                    {
                        modelDSC.IsAlreadyRegistered = 2;
                    }
                    
                }

                if (modelDSC.IsAlreadyRegistered == 1)
                {


                    ListModel model = new ListModel();
                    // model.billid = 0;

                    objparams.BILL_TYPE = "P";
                    objparams.FUND_TYPE = PMGSYSession.Current.FundType;
                    objparams.LVL_ID = PMGSYSession.Current.LevelId;
                    objparams.ADMIN_ND_CODE = PMGSYSession.Current.AdminNdCode;
                    objparams.DISTRICT_CODE = PMGSYSession.Current.DistrictCode;

                    List<SelectListItem> monthList = common.PopulateMonths(DateTime.Now.Month);
                    ViewData["months"] = monthList;

                    List<SelectListItem> yearList = common.PopulateYears(DateTime.Now.Year);
                    ViewData["year"] = yearList;

                    List<SelectListItem> transactionType = common.PopulateTransactions(objparams);
                    ViewData["TXN_ID"] = transactionType;



                    ViewBag.SessionSalt = Session["SessionSalt"].ToString();
                    ViewBag.IsAlreadyRegistered = "1";

                    //ViewBag.IsPaymentEnabled = pfmsDAL.ValidateSamplePayment();
                    ViewBag.IsPaymentEnabled = false;

                    ViewBag.IsReatPaymentEnabled = reatDAL.ValidateSamplePayment();

                    return View("EpayList");
                }
                else
                {
                    ViewBag.IsAlreadyRegistered = modelDSC.IsAlreadyRegistered.ToString();
                    return View("EpayList");
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
        /// function to populate the epayment grid
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
        public JsonResult GetEPaymentListJson(int? page, int? rows, string sidx, string sord)
        {
            try
            {
                PaymentFilterModel objFilter = new PaymentFilterModel();
                objFilter.FilterMode = Request.Params["mode"].Trim();

                string TransactionType = string.Empty;

                //Below line commented on 21-04-2023
                //if (Request.Params["payType"] != "E" && Request.Params["payType"] != "R")
                //Below line added on 21-04-2023
                if (Request.Params["payType"] != "E" && Request.Params["payType"] != "R" && Request.Params["payType"] != "D" && Request.Params["payType"] != "H")
                {
                    throw new Exception("Invalid Parameters");
                }
                else
                {
                    TransactionType = Request.Params["payType"];
                }

                long totalRecords;
                objFilter.Month = Convert.ToInt16(Request.Params["months"]);
                objFilter.Year = Convert.ToInt16(Request.Params["year"]);

                if (objFilter.FilterMode.Equals("Search"))
                {
                    objFilter.FromDate = Request.Params["fromDate"].ToString() == String.Empty ? null : Request.Params["fromDate"].ToString();
                    objFilter.ToDate = Request.Params["toDate"].ToString() == String.Empty ? null : Request.Params["toDate"].ToString();
                    objFilter.TransId = Request.Params["transType"] == String.Empty ? Convert.ToInt16(0) : Convert.ToInt16(Request.Params["transType"].Trim().Split('$')[0]);

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
                    rows = objPaymentBAL.ListEPaymentDetails(objFilter, TransactionType, out totalRecords, Request.Params["moduleType"].ToString()),
                    total = totalRecords <= objFilter.rows ? 1 : totalRecords / objFilter.rows + 1,
                    page = page,
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
        /// New Common Method Created for Generate PDF File For Epayment
        /// </summary>
        /// <param name="epaymodel"></param>
        /// <returns></returns>
        public String CreateEpaymentPDFFile(EpaymentOrderModel epaymodel, long bill_ID, string fileName)
        {
            try
            {
                ConvertHTMLToPDF("", fileName, epaymodel);
                return "1";
            }
            catch (Exception ex)
            {
                return "0";
            }
        }
        public String CreateERemPDFFile(EremittnaceOrderModel epaymodel, long bill_ID, string fileName)
        {
            try
            {
                ConvertHTMLToPDFErem("", fileName, epaymodel);
                return "1";
            }
            catch (Exception ex)
            {
                return "0";
            }
        }


        [HttpPost]
        public ActionResult GetPdf()
        {
            using (StreamWriter sw = System.IO.File.AppendText(ConfigurationManager.AppSettings["DigSignErrorLogPath"].ToString() + "PdfSignerAppletErrorLog_" + System.DateTime.Now.Date.ToString("dd_MM_yyyy") + ".txt"))
            {
                sw.WriteLine(" in GetpDF :" + DateTime.Now.ToString() + " UserName -" + PMGSYSession.Current.UserName);
                sw.Close();
            }


            Int64 bill_ID;
            DigSignBAL objBAL = new DigSignBAL();

            int accountId = 0;
            PaymentDAL objDAL = new PaymentDAL();

            try
            {
                RegistrationData model = new RegistrationData();
                model.PdfKey = Request.Params["pdfKey"];
                model.CertificateBase64 = string.IsNullOrEmpty(Request.Params["certificate"]) ? string.Empty : Request.Params["certificate"].Trim();
                model.CertificateChainBase64 = string.IsNullOrEmpty(Request.Params["certificateChain"]) ? string.Empty : Request.Params["certificateChain"].Trim();

                if (string.IsNullOrEmpty(model.CertificateBase64) || string.IsNullOrEmpty(model.CertificateChainBase64))
                {
                    using (StreamWriter sw = System.IO.File.AppendText(ConfigurationManager.AppSettings["DigSignErrorLogPath"].ToString() + "PdfSignerAppletErrorLog_" + System.DateTime.Now.Date.ToString("dd_MM_yyyy") + ".txt"))
                    {
                        sw.WriteLine("Certificate details from client to server are null or empty." + DateTime.Now.ToString() + " UserName -" +
                            PMGSYSession.Current.UserName);
                        sw.Close();
                    }

                    return Json(new { message = "Certificate details from client to server are null or empty." });
                }

                RegisterDSCModel regModel = new RegisterDSCModel();
                regModel = objBAL.GetDetailsToRegisterDSC();

                //verify certificate equality with Authorized Signatory's registered Certificate
                string verificationOfRegCertResult = objBAL.VerifyRegisteredCertificate(model.CertificateBase64, model.CertificateChainBase64, regModel.NodalOfficerCode);

                if (!verificationOfRegCertResult.Equals(string.Empty))
                {

                    using (StreamWriter sw = System.IO.File.AppendText(ConfigurationManager.AppSettings["DigSignErrorLogPath"].ToString() + "PdfSignerAppletErrorLog_" + System.DateTime.Now.Date.ToString("dd_MM_yyyy") + ".txt"))
                    {
                        sw.WriteLine("verificationOfRegCertResult is NULL or empty" + DateTime.Now.ToString() + DateTime.Now.ToString() + " UserName -" +
                            PMGSYSession.Current.UserName);
                        sw.Close();
                    }

                    return Json(new { status = "error", message = verificationOfRegCertResult, fileBase64String = string.Empty, encHashOfBase64Str = string.Empty },
                        JsonRequestBehavior.AllowGet);
                }

                // verify Certificate, if suucessful, then only store it in DB else flag error message
                string verificationResult = objBAL.VerifyCertificate(model.CertificateBase64, model.CertificateChainBase64, regModel.NameAsPerCertificate);

                if (verificationResult.Equals(string.Empty))
                {

                    using (StreamWriter sw = System.IO.File.AppendText(ConfigurationManager.AppSettings["DigSignErrorLogPath"].ToString() + "PdfSignerAppletErrorLog_" + System.DateTime.Now.Date.ToString("dd_MM_yyyy") + ".txt"))
                    {
                        sw.WriteLine("verificationResult success " + DateTime.Now.ToString() + " UserName:- " + PMGSYSession.Current.UserName);
                        sw.Close();
                    }

                    String[] urlSplitParams = model.PdfKey.Split('$');
                    bill_ID = Convert.ToInt64(urlSplitParams[0]);
                    string fileName = urlSplitParams[1].ToString();
                    string Path = string.Empty;
                    EpaymentOrderModel epaymodel = new EpaymentOrderModel();
                    //epaymodel = objPaymentBAL.GetEpaymentDetails(bill_ID);

                    ///Bank Details Accepted by PFMS
                    accountId = objDAL.GetAccountIdByBillId(bill_ID);
                    epaymodel = accountId > 0 ? objDAL.GetEpaymentDetails1(bill_ID, accountId) : objPaymentBAL.GetEpaymentDetails(bill_ID);

                    #region Region to Check Security Deposit and Holding Account Bank Availability
                    //Below conditions are used to check availability of security deposit bank details and holding account bank details for txnId = 3185

                    //PMGSYEntities dbContext = new PMGSYEntities();
                    //int txn_id = dbContext.ACC_BILL_MASTER.Where(x => x.BILL_ID == bill_ID).Any() ? dbContext.ACC_BILL_MASTER.Where(x => x.BILL_ID == bill_ID).Select(x => x.TXN_ID).FirstOrDefault() : 0;

                    //if ((txn_id == 3185) && ((epaymodel.EpayConAcNum == string.Empty) || (epaymodel.EpayConBankName == string.Empty) || (epaymodel.EpayConBankIFSCCode == string.Empty )))
                    //{
                    //    return Json(new { status = "error", message = "Security Deposit bank account number or name or IFSC code  details is not available ." }, JsonRequestBehavior.AllowGet);
                    //}
                    //else if ((txn_id == 3185) && ((epaymodel.BankAcNumber == string.Empty )|| (epaymodel.BankIFSCCode==string.Empty)))
                    //{
                    //    return Json(new { status = "error", message = "Holding account bank account number or name or IFSC code details is not available ." }, JsonRequestBehavior.AllowGet);

                    //}
                    #endregion

                    string strFlag = CreateEpaymentPDFFile(epaymodel, bill_ID, fileName);
                    string passwords = string.Empty;
                    if (!(epaymodel.SrrdaPassword == null))
                    {
                        passwords = epaymodel.SrrdaPassword.ToString();
                    }
                    if (!(epaymodel.BankPdfPassword == null))
                    {
                        passwords = passwords + "," + epaymodel.BankPdfPassword.ToString();
                    }

                    using (StreamWriter sw = System.IO.File.AppendText(ConfigurationManager.AppSettings["DigSignErrorLogPath"].ToString() + "PdfSignerAppletErrorLog_" + System.DateTime.Now.Date.ToString("dd_MM_yyyy") + ".txt"))
                    {
                        sw.WriteLine("strFlag" + strFlag + "  " + DateTime.Now.ToString() + " UserName -" +
                            PMGSYSession.Current.UserName);
                        sw.Close();
                    }

                    if (strFlag.Equals("1"))
                    {
                        Path = ConfigurationManager.AppSettings["PdfFilePath"].ToString() + fileName;
                    }
                    else
                    {
                        using (StreamWriter sw = System.IO.File.AppendText(ConfigurationManager.AppSettings["DigSignErrorLogPath"].ToString() + "PdfSignerAppletErrorLog_" + System.DateTime.Now.Date.ToString("dd_MM_yyyy") + ".txt"))
                        {
                            sw.WriteLine("verificationResult is  NULL or empty" + DateTime.Now.ToString() + " UserName -" +
                            PMGSYSession.Current.UserName);
                            sw.Close();
                        }

                        return Json(new { status = "error", message = verificationResult, fileBase64String = string.Empty, encHashOfBase64Str = string.Empty },
                            JsonRequestBehavior.AllowGet);
                    }
                    if (System.IO.File.Exists(Path))
                    {
                        var fileByteArr = System.IO.File.ReadAllBytes(Path);
                        string fileBase64Str = Convert.ToBase64String(fileByteArr);

                        string encParam = PMGSY.Common.Cryptography.AESEncrypt(passwords);
                        using (StreamWriter sw = System.IO.File.AppendText(ConfigurationManager.AppSettings["DigSignErrorLogPath"].ToString() + "PdfSignerAppletErrorLog_" + System.DateTime.Now.Date.ToString("dd_MM_yyyy") + ".txt"))
                        {
                            sw.WriteLine("encParam" + encParam + "  " + DateTime.Now.ToString() + " UserName -" +
                           PMGSYSession.Current.UserName);
                            sw.Close();
                        }

                        return Json(new
                        {
                            status = "success",
                            password = encParam,
                            message = string.Empty,
                            fileBase64String = fileBase64Str,
                            encHashOfBase64String =
                                string.Empty
                        }, JsonRequestBehavior.AllowGet);

                        // return Json(new { status = "error", message = verificationOfRegCertResult, fileBase64String = string.Empty, encHashOfBase64Str = string.Empty },
                        //JsonRequestBehavior.AllowGet);

                    }
                    else
                    {
                        return Json(new { status = "error", message = verificationResult, fileBase64String = string.Empty, encHashOfBase64Str = string.Empty },
                            JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    return Json(new { status = "error", message = verificationResult, fileBase64String = string.Empty, encHashOfBase64Str = string.Empty },
                        JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                using (StreamWriter sw = System.IO.File.AppendText(ConfigurationManager.AppSettings["DigSignErrorLogPath"].ToString() + "PdfSignerAppletErrorLog_" + System.DateTime.Now.Date.ToString("dd_MM_yyyy") + ".txt"))
                {
                    sw.WriteLine("Date :" + DateTime.Now.ToString() + " UserName -" + PMGSYSession.Current.UserName);
                    sw.WriteLine("Method : " + "Get Pdf()");
                    sw.WriteLine("Exception Message : " + ex.Message);
                    sw.WriteLine("Exception : " + ex.StackTrace);
                    sw.WriteLine("____________________________________________________");
                    sw.Close();
                }
                return Json(new { status = "error", message = "Error occurred while accessing pdf file.", fileBase64String = string.Empty, encHashOfBase64Str = string.Empty },
                    JsonRequestBehavior.AllowGet);

            }

        }

        [HttpPost]
        public ActionResult SavePdf()
        {
            using (StreamWriter sw = System.IO.File.AppendText(ConfigurationManager.AppSettings["DigSignErrorLogPath"].ToString() + "PdfSignerAppletErrorLog_" + System.DateTime.Now.Date.ToString("dd_MM_yyyy") + ".txt"))
            {
                sw.WriteLine(" in SavePdf :" + DateTime.Now.ToString() + " UserName -" + PMGSYSession.Current.UserName);
                sw.Close();
            }


            DigSignBAL objBAL = new DigSignBAL();
            RegistrationData model = new RegistrationData();
            model.PdfKey = Request.Params["pdfKey"];
            String[] urlSplitParams = model.PdfKey.Split('$');
            Int64 bill_ID = Convert.ToInt64(urlSplitParams[0]);
            string fileName = urlSplitParams[1].ToString();

            int accountId = 0;
            PaymentDAL objDAL = new PaymentDAL();

            try
            {

                Request.Files[0].SaveAs(Path.Combine(ConfigurationManager.AppSettings["PdfFilePath"].ToString() + "S_" + fileName));
                string result = objPaymentBAL.InsertEpaymentMailDetails(bill_ID, "S_" + fileName);
                if (result.Equals("1"))
                {

                    try
                    {

                        string strHeaderPath = "";
                        string ErrorMessage = string.Empty;
                        EpaymentOrderModel epaymodel = new EpaymentOrderModel();
                        //epaymodel = objPaymentBAL.GetEpaymentDetails(bill_ID);
                        ///Bank Details Accepted by PFMS
                        accountId = objDAL.GetAccountIdByBillId(bill_ID);
                        epaymodel = accountId > 0 ? objDAL.GetEpaymentDetails1(bill_ID, accountId) : objPaymentBAL.GetEpaymentDetails(bill_ID);

                        // Commented by Srishti on 13-03-2023
                        //MvcMailMessage ms = objPaymentBAL.EpayOrderMail(epaymodel, Path.Combine(ConfigurationManager.AppSettings["PdfFilePath"].ToString() + "S_" + fileName), strHeaderPath, ref ErrorMessage);
                        // To uncomment file on the live environment 
                        //ms.Send();//Comment on 13-12-2021

                        // Added by Srishti on 13-03-2023
                        ServicePointManager.ServerCertificateValidationCallback += (s, ce, ca, p) => true;
                        ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                        MailMessage ms = objPaymentBAL.EpayOrderMail(epaymodel, Path.Combine(ConfigurationManager.AppSettings["PdfFilePath"].ToString() + "S_" + fileName), strHeaderPath, ref ErrorMessage);

                        SmtpClient client = new SmtpClient();

                        string e_EuthHost = ConfigurationManager.AppSettings["e_EuthHost"];
                        string e_EuthPort = ConfigurationManager.AppSettings["e_EuthPort"];
                        string e_EuthMailUserName = ConfigurationManager.AppSettings["e_EuthMailUserName"];
                        string e_EuthMailPassword = ConfigurationManager.AppSettings["e_EuthMailPassword"];

                        client.Host = e_EuthHost;
                        client.Port = Convert.ToInt32(e_EuthPort);
                        client.UseDefaultCredentials = false;
                        client.EnableSsl = true; // Change to true
                        client.Credentials = new NetworkCredential(e_EuthMailUserName, e_EuthMailPassword);
                        client.DeliveryMethod = SmtpDeliveryMethod.Network;
                        client.Send(ms);

                    }
                    catch (Exception ex)
                    {
                        string mailDeleteResult = objPaymentBAL.DeleteMailDetails(bill_ID);
                        System.IO.File.Delete(ConfigurationManager.AppSettings["PdfFilePath"].ToString() + fileName);
                        using (StreamWriter sw = System.IO.File.AppendText(ConfigurationManager.AppSettings["DigSignErrorLogPath"].ToString() + "PdfSignerAppletErrorLog_" + System.DateTime.Now.Date.ToString("dd_MM_yyyy") + ".txt"))
                        {
                            sw.WriteLine("Date :" + DateTime.Now.ToString() + " UserName -" +
                                    PMGSYSession.Current.UserName);
                            sw.WriteLine("Method : " + "SavePdf()");
                            sw.WriteLine("Exception Message : " + ex.Message);
                            sw.WriteLine("Exception : " + ex.StackTrace);
                            sw.WriteLine("____________________________________________________");
                            sw.Close();
                        }
                        return Json(new { status = "error", message = "Error while sending E-Mail ." });

                    }
                }

                System.IO.File.Delete(Path.Combine(ConfigurationManager.AppSettings["PdfFilePath"].ToString() + fileName));
                return Json(new { status = "success", message = "Document signed successfully." });
            }
            catch (Exception ex)
            {

                using (StreamWriter sw = System.IO.File.AppendText(ConfigurationManager.AppSettings["DigSignErrorLogPath"].ToString() + "PdfSignerAppletErrorLog_" + System.DateTime.Now.Date.ToString("dd_MM_yyyy") + ".txt"))
                {
                    sw.WriteLine("Date :" + DateTime.Now.ToString() + " UserName -" +
                            PMGSYSession.Current.UserName);
                    sw.WriteLine("Method : " + "SavePdf()");
                    sw.WriteLine("Exception Message : " + ex.Message);
                    sw.WriteLine("Exception : " + ex.StackTrace);
                    sw.WriteLine("____________________________________________________");
                    sw.Close();
                }
                return Json(new { message = "Error occurred while saving signed Document." });
            }
        }

        [HttpPost]
        public ActionResult GetEremPdf()
        {
            Int64 bill_ID;
            DigSignBAL objBAL = new DigSignBAL();
            try
            {
                RegistrationData model = new RegistrationData();
                model.PdfKey = Request.Params["pdfKey"];
                model.CertificateBase64 = string.IsNullOrEmpty(Request.Params["certificate"]) ? string.Empty : Request.Params["certificate"].Trim();
                model.CertificateChainBase64 = string.IsNullOrEmpty(Request.Params["certificateChain"]) ? string.Empty : Request.Params["certificateChain"].Trim();
                // string verificationResult = string.Empty;

                if (string.IsNullOrEmpty(model.CertificateBase64) || string.IsNullOrEmpty(model.CertificateChainBase64))
                {
                    return Json(new { message = "Certificate details from client to server are null or empty." });
                }

                RegisterDSCModel regModel = new RegisterDSCModel();
                regModel = objBAL.GetDetailsToRegisterDSC();

                //verify certificate equality with Authorized Signatory's registered Certificate
                string verificationOfRegCertResult = objBAL.VerifyRegisteredCertificate(model.CertificateBase64, model.CertificateChainBase64, regModel.NodalOfficerCode);

                if (!verificationOfRegCertResult.Equals(string.Empty))
                {
                    return Json(new { status = "error", message = verificationOfRegCertResult, fileBase64String = string.Empty, encHashOfBase64Str = string.Empty },
                        JsonRequestBehavior.AllowGet);
                }

                // verify Certificate, if suucessful, then only store it in DB else flag error message
                string verificationResult = objBAL.VerifyCertificate(model.CertificateBase64, model.CertificateChainBase64, regModel.NameAsPerCertificate);

                if (verificationResult.Equals(string.Empty))
                {
                    String[] urlSplitParams = model.PdfKey.Split('$');
                    bill_ID = Convert.ToInt64(urlSplitParams[0]);
                    string fileName = urlSplitParams[1].ToString();
                    string Path = string.Empty;
                    EremittnaceOrderModel eremmodel = new EremittnaceOrderModel();
                    eremmodel = objPaymentBAL.GetEremittanceDetails(bill_ID);
                    string strFlag = CreateERemPDFFile(eremmodel, bill_ID, fileName);
                    // string passwords = eremmodel.BankPdfPassword.ToString() + "," + eremmodel.BankPdfPassword;
                    string passwords = string.Empty;

                    if (strFlag.Equals("1"))
                    {
                        Path = ConfigurationManager.AppSettings["PdfFilePath"].ToString() + fileName;
                    }
                    else
                    {
                        return Json(new { status = "error", message = verificationResult, fileBase64String = string.Empty, encHashOfBase64Str = string.Empty },
                        JsonRequestBehavior.AllowGet);
                    }

                    if (!(eremmodel.SrrdaPassword == null))
                    {
                        passwords = eremmodel.SrrdaPassword.ToString();
                    }
                    if (!(eremmodel.BankPdfPassword == null))
                    {
                        passwords = passwords + "," + eremmodel.BankPdfPassword.ToString();
                    }

                    if (System.IO.File.Exists(Path))
                    {

                        var fileByteArr = System.IO.File.ReadAllBytes(Path);
                        string fileBase64Str = Convert.ToBase64String(fileByteArr);

                        string encParam = PMGSY.Common.Cryptography.AESEncrypt(passwords);
                        //string decParam = PMGSY.Common.Cryptography.AESDecrypt(encParam);
                        return Json(new { status = "success", password = encParam, message = string.Empty, fileBase64String = fileBase64Str, encHashOfBase64String = string.Empty }, JsonRequestBehavior.AllowGet);

                    }
                    else
                    {
                        return Json(new { status = "error", message = verificationResult, fileBase64String = string.Empty, encHashOfBase64Str = string.Empty }, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    return Json(new { status = "error", message = verificationResult, fileBase64String = string.Empty, encHashOfBase64Str = string.Empty }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                using (StreamWriter sw = System.IO.File.AppendText(ConfigurationManager.AppSettings["DigSignErrorLogPath"].ToString() + "PdfSignerAppletErrorLog_" + System.DateTime.Now.Date.ToString("dd_MM_yyyy") + ".txt"))
                {
                    sw.WriteLine("Date :" + DateTime.Now.ToString());
                    sw.WriteLine("Method : " + "SignPdf()");
                    sw.WriteLine("Exception Message : " + ex.Message);
                    sw.WriteLine("Exception : " + ex.StackTrace);
                    sw.WriteLine("____________________________________________________");
                    sw.Close();
                }
                return Json(new { status = "error", message = "Error occurred while accessing pdf file.", fileBase64String = string.Empty, encHashOfBase64Str = string.Empty }, JsonRequestBehavior.AllowGet);

            }

        }

        [HttpPost]
        public ActionResult SaveEremPdf()
        {
            DigSignBAL objBAL = new DigSignBAL();
            RegistrationData model = new RegistrationData();
            model.PdfKey = Request.Params["pdfKey"];
            String[] urlSplitParams = model.PdfKey.Split('$');
            Int64 bill_ID = Convert.ToInt64(urlSplitParams[0]);
            string fileName = urlSplitParams[1].ToString();
            EremittnaceOrderModel epaymodel = new EremittnaceOrderModel();
            try
            {
                Request.Files[0].SaveAs(Path.Combine(ConfigurationManager.AppSettings["PdfFilePath"].ToString() + "S_" + fileName));
                string result = objPaymentBAL.InsertEremittanceMailDetails(bill_ID, "S_" + fileName);
                if (result.Equals("1"))
                {
                    try
                    {
                        string strHeaderPath = Server.MapPath("~/Content/images/Header-e-gov-6.png");
                        string ErrorMessage = string.Empty;
                        epaymodel = objPaymentBAL.GetEremittanceDetails(bill_ID);
                        // Commented by Srishti on 13-03-2023
                        //objPaymentBAL.SendMailForEremOrder(epaymodel, ConfigurationManager.AppSettings["PdfFilePath"].ToString() + "S_" + fileName, strHeaderPath, ref 
                        //ErrorMessage).Send();

                        // Added by Srishti on 13-03-2023
                        ServicePointManager.ServerCertificateValidationCallback += (s, ce, ca, p) => true;
                        ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                        MailMessage ms = objPaymentBAL.SendMailForEremOrder(epaymodel, ConfigurationManager.AppSettings["PdfFilePath"].ToString() + "S_" + fileName, strHeaderPath, ref ErrorMessage);

                        SmtpClient client = new SmtpClient();

                        string e_EuthHost = ConfigurationManager.AppSettings["e_EuthHost"];
                        string e_EuthPort = ConfigurationManager.AppSettings["e_EuthPort"];
                        string e_EuthMailUserName = ConfigurationManager.AppSettings["e_EuthMailUserName"];
                        string e_EuthMailPassword = ConfigurationManager.AppSettings["e_EuthMailPassword"];

                        client.Host = e_EuthHost;
                        client.Port = Convert.ToInt32(e_EuthPort);
                        client.UseDefaultCredentials = false;
                        client.EnableSsl = true; // Change to true
                        client.Credentials = new NetworkCredential(e_EuthMailUserName, e_EuthMailPassword);
                        client.Send(ms);

                    }
                    catch (Exception ex)
                    {
                        string mailDeleteResult = objPaymentBAL.DeleteMailDetails(bill_ID);

                        // UNCOMMENT
                        //System.IO.File.Delete(ConfigurationManager.AppSettings["PdfFilePath"].ToString() + "S_" +  fileName);
                        //return Json(new { status = "error", message = "Error while sending E-Mail ." });
                        System.IO.File.Delete(ConfigurationManager.AppSettings["PdfFilePath"].ToString() + fileName);
                        return Json(new { status = "error", message = "Error while sending E-Mail ." });

                    }
                }
                System.IO.File.Delete(Path.Combine(ConfigurationManager.AppSettings["PdfFilePath"].ToString() + fileName));
                return Json(new { status = "success", message = "Document signed successfully." });
            }
            catch (Exception ex)
            {

                using (StreamWriter sw = System.IO.File.AppendText(ConfigurationManager.AppSettings["DigSignErrorLogPath"].ToString() + "PdfSignerAppletErrorLog_" + System.DateTime.Now.Date.ToString("dd_MM_yyyy") + ".txt"))
                {
                    sw.WriteLine("Date :" + DateTime.Now.ToString());
                    sw.WriteLine("Method : " + "SignPdf()");
                    sw.WriteLine("Exception Message : " + ex.Message);
                    sw.WriteLine("Exception : " + ex.StackTrace);
                    sw.WriteLine("____________________________________________________");
                    sw.Close();
                }
                return Json(new { message = "Error occurred while saving signed Document." });
            }
        }


        /// <summary>
        /// function to unlock voucher
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="hash"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        [Audit]
        public JsonResult UnlockEpayment(String parameter, String hash, String key)
        {

            Int64 bill_ID = 0;

            if (!String.IsNullOrEmpty(parameter) && !String.IsNullOrEmpty(hash) && !String.IsNullOrEmpty(key))
            {
                String[] urlParams = URLEncrypt.DecryptParameters(new String[] { parameter, hash, key });
                if (urlParams.Length >= 1)
                {
                    String[] urlSplitParams = urlParams[0].Split('$');

                    bill_ID = Convert.ToInt64(urlSplitParams[0]);

                    string result = objPaymentBAL.UnlockEpayment(bill_ID);

                    return Json(new
                    {
                        Success = result
                    });

                }
                else
                {
                    throw new Exception("Error While getting Epaymant details.. ");
                }
            }
            else
            {
                throw new Exception("Error While getting Epaymant details.. ");
            }

        }


        [Audit]
        protected void ConvertHTMLToPDF(string HTMLCode, String fileName, EpaymentOrderModel Model)
        {
            using (StreamWriter sw = System.IO.File.AppendText(ConfigurationManager.AppSettings["DigSignErrorLogPath"].ToString() + "PdfSignerAppletErrorLog_" + System.DateTime.Now.Date.ToString("dd_MM_yyyy") + ".txt"))
            {
                sw.WriteLine(" in ConvertHTMLToPDF :" + DateTime.Now.ToString() + " UserName:- " + PMGSYSession.Current.UserName);
                sw.Close();
            }

            DigSignBAL objBAL = new DigSignBAL();
            RegisterDSCModel regsignmodel = new RegisterDSCModel();
            regsignmodel = objBAL.GetDetailsToRegisterDSC();

            //Below Code Added on 21-04-2023
            PMGSYEntities dbContext = new PMGSYEntities();
            int txnId = dbContext.ACC_BILL_MASTER.Where(x => x.BILL_ID == Model.BillID).Select(x => x.TXN_ID).FirstOrDefault();

            //Create PDF document 
            Document doc = new Document(PageSize.A4);
            PdfWriter writer = PdfWriter.GetInstance(doc, new FileStream(ConfigurationManager.AppSettings["PdfFilePath"].ToString() + fileName, FileMode.Create));
            doc.Open();

            string str = Server.MapPath("~/Content/images/pmgsy-logo.png");
            Image jpg = Image.GetInstance(str);
            jpg.SetAbsolutePosition(30, 770);

            jpg.ScaleToFit(60f, 60f);
            doc.Add(jpg);


            iTextSharp.text.Font fontBoldU = new iTextSharp.text.Font(iTextSharp.text.FontFactory.GetFont("calibri", 12, iTextSharp.text.Font.UNDERLINE));
            iTextSharp.text.Font fontBold12 = new iTextSharp.text.Font(iTextSharp.text.FontFactory.GetFont("calibri", 12, iTextSharp.text.Font.BOLD));
            iTextSharp.text.Font fontBold = new iTextSharp.text.Font(iTextSharp.text.FontFactory.GetFont("calibri", 12, iTextSharp.text.Font.BOLD));
            //iTextSharp.text.Font fontBold11 = new iTextSharp.text.Font(iTextSharp.text.FontFactory.GetFont("calibri", 11, iTextSharp.text.Font.BOLD));
            iTextSharp.text.Font fontNormal11 = new iTextSharp.text.Font(iTextSharp.text.FontFactory.GetFont("calibri", 11, iTextSharp.text.Font.NORMAL));
            iTextSharp.text.Font fontBold9 = new iTextSharp.text.Font(iTextSharp.text.FontFactory.GetFont("calibri", 9, iTextSharp.text.Font.BOLD));
            iTextSharp.text.Font fontNormal8 = new iTextSharp.text.Font(iTextSharp.text.FontFactory.GetFont("calibri", 8, iTextSharp.text.Font.NORMAL));
            iTextSharp.text.Font fontBold8 = new iTextSharp.text.Font(iTextSharp.text.FontFactory.GetFont("calibri", 8, iTextSharp.text.Font.BOLD));
            iTextSharp.text.Font fontBold10 = new iTextSharp.text.Font(iTextSharp.text.FontFactory.GetFont("calibri", 10, iTextSharp.text.Font.BOLD));
            string strFund = string.Empty;
            if (PMGSY.Extensions.PMGSYSession.Current.FundType.Equals("A"))
            {
                strFund = "Administrative Expenses Fund";
            }
            else if (PMGSY.Extensions.PMGSYSession.Current.FundType.Equals("P"))
            {
                strFund = "Programme Fund";
            }
            else if (PMGSY.Extensions.PMGSYSession.Current.FundType.Equals("M"))
            {
                strFund = "Maintenance Fund";
            }


            PdfContentByte canvas = writer.DirectContent;

            ColumnText.ShowTextAligned(canvas, Element.ALIGN_CENTER, new Phrase("Pradhan Mantri Gram Sadak Yojana", fontBold12), 300, 790, 0);
            doc.Add(new Phrase(Environment.NewLine));

            canvas.SaveState();
            canvas.SetLineWidth(1.0f);   // Make a bit thicker than 1.0 default
            canvas.SetColorStroke(iTextSharp.text.Color.BLACK); // 1 = black, 0 = white
            canvas.MoveTo(20, 770);
            canvas.LineTo(570, 770);
            canvas.Stroke();
            canvas.RestoreState();

            ColumnText.ShowTextAligned(canvas, Element.ALIGN_CENTER, new Phrase("E-payment Details", fontBoldU), 300, 750, 0);
            ColumnText.ShowTextAligned(canvas, Element.ALIGN_CENTER, new Phrase(strFund, fontNormal11), 300, 730, 0);

            //if (txnId == 3185 || txnId == 3187)
            //{
            //    //doc.Add(new Phrase(Environment.NewLine));
            //    //ColumnText.ShowTextAligned(canvas, Element.ALIGN_LEFT, new Phrase("State", fontBold8), 300, 730, 0);
            //    ColumnText.ShowTextAligned(canvas, Element.ALIGN_LEFT, new Phrase("State : "+Model.EpayState, fontBold8), 300, 730, 0);
            //    //doc.Add(new Phrase(Environment.NewLine));
            //    //ColumnText.ShowTextAligned(canvas, Element.ALIGN_RIGHT, new Phrase("DPIU", fontBold8), 300, 730, 0);
            //    //ColumnText.ShowTextAligned(canvas, Element.ALIGN_LEFT, new Phrase("DPIU : "+Model.EpayDPIU + " ( - " + Model.DPIUName + " ) ", fontBold8), 300, 730, 0);
            //}


            PdfPTable table = new PdfPTable(2);
            table.TotalWidth = 260f;
            table.LockedWidth = true;
            float[] tablet1widths = new float[] { 80f, 180f };
            table.SetWidths(tablet1widths);

            //Below Condition Commented on 21-04-2023
            //PdfPCell cell = new PdfPCell(new Phrase("Sender Information", fontBold9));
            //Below Condition Added on 21-04-2023
            PdfPCell cell = new PdfPCell();
            if (txnId == 3185 || txnId == 3187)
            {
                cell = new PdfPCell(new Phrase("Debit Information", fontBold9));

            }
            else
            {
                cell = new PdfPCell(new Phrase("Sender Information", fontBold9));

            }
            cell.Colspan = 2;
            cell.HorizontalAlignment = 0;
            cell.BorderWidthBottom = 1f;
            cell.BorderWidthLeft = 0;
            cell.BorderWidthTop = 0;
            cell.BorderWidthRight = 0;

            table.AddCell(cell);
            table.DefaultCell.Border = Rectangle.NO_BORDER;

            table.AddCell(new Phrase("State", fontBold8));
            table.AddCell(new Phrase(Model.EpayState, fontNormal8));

            table.AddCell(new Phrase("DPIU", fontBold8));
            table.AddCell(new Phrase(Model.EpayDPIU + " ( - " + Model.DPIUName + " ) ", fontNormal8));


            table.AddCell(new Phrase("Bank Details", fontBold8));
            table.AddCell(new Phrase(Model.Bankaddress, fontNormal8));

            table.AddCell(new Phrase("Account Number", fontBold8));
            table.AddCell(new Phrase(Model.BankAcNumber, fontNormal8));

            //Below Code Added on 15-03-2023
            if (txnId == 3185 || txnId == 3187)
            {
                table.AddCell(new Phrase("IFSC Code", fontBold8));
                table.AddCell(new Phrase(Model.BankIFSCCode, fontNormal8));
            }

            PdfPTable t2 = new PdfPTable(1);
            t2.TotalWidth = 260f;
            PdfPCell cell2 = new PdfPCell();
            cell2.AddElement(table);
            cell2.BorderWidthBottom = 1f;
            cell2.BorderWidthLeft = 1f;
            cell2.BorderWidthTop = 1f;
            cell2.BorderWidthRight = 1f;
            t2.AddCell(cell2);

            t2.WriteSelectedRows(0, -1, doc.Left + 5, doc.Top - 90, writer.DirectContent);

            table = new PdfPTable(2);
            table.TotalWidth = 260f;
            table.LockedWidth = true;
            float[] tablet2widths = new float[] { 70f, 190f };
            table.SetWidths(tablet2widths);
            //Below line commented on 21-04-2023
            //cell = new PdfPCell(new Phrase("Contractor/Supplier Information", fontBold9));
            //Below line Added on 21-04-2023
            if (txnId == 3185 || txnId == 3187)
            {
                cell = new PdfPCell(new Phrase("Credit Information", fontBold9));
            }
            else
            {
                cell = new PdfPCell(new Phrase("Contractor/Supplier Information", fontBold9));
            }
            cell.Colspan = 2;
            cell.HorizontalAlignment = 0;
            cell.HorizontalAlignment = 0;
            cell.BorderWidthBottom = 1f;
            cell.BorderWidthLeft = 0;
            cell.BorderWidthTop = 0;
            cell.BorderWidthRight = 0;

            table.AddCell(cell);
            table.DefaultCell.Border = Rectangle.NO_BORDER;

            table.AddCell(new Phrase("Name", fontBold8));
            table.AddCell(new Phrase(Model.EpayConName, fontNormal8));

            table.AddCell(new Phrase("Bank Name", fontBold8));
            table.AddCell(new Phrase(Model.EpayConBankName, fontNormal8));

            table.AddCell(new Phrase("Account Number", fontBold8));
            table.AddCell(new Phrase(Model.EpayConAcNum, fontNormal8));

            table.AddCell(new Phrase("IFSC Code", fontBold8));
            table.AddCell(new Phrase(Model.EpayConBankIFSCCode, fontNormal8));


            PdfPTable t3 = new PdfPTable(1);
            t3.TotalWidth = 260f;
            PdfPCell cell3 = new PdfPCell();
            cell3.AddElement(table);
            cell3.BorderWidthBottom = 1f;
            cell3.BorderWidthLeft = 1f;
            cell3.BorderWidthTop = 1f;
            cell3.BorderWidthRight = 1f;
            t3.AddCell(cell3);
            t3.WriteSelectedRows(0, -1, doc.Left + 270, doc.Top - 90, writer.DirectContent);

            table = new PdfPTable(4);
            table.TotalWidth = 530f;
            table.LockedWidth = true;
            table.DefaultCell.Border = Rectangle.NO_BORDER;
            float[] tablet3widths = new float[] { 80f, 180f, 80f, 180f };
            table.SetWidths(tablet3widths);

            cell = new PdfPCell(new Phrase("Epayment Information", fontBold9));
            cell.Colspan = 4;
            // cell.Border = 0;
            cell.BorderWidthBottom = 1f;
            cell.BorderWidthLeft = 0;
            cell.BorderWidthTop = 0;
            cell.BorderWidthRight = 0;
            cell.HorizontalAlignment = 1;
            table.AddCell(cell);


            table.AddCell(new Phrase("Date", fontBold8));
            table.AddCell(new Phrase(Model.EmailDate, fontNormal8));

            table.AddCell(new Phrase("Number", fontBold8));
            table.AddCell(new Phrase(Model.EpayNumber, fontNormal8));


            table.AddCell(new Phrase("Voucher Number", fontBold8));
            table.AddCell(new Phrase(Model.EpayVNumber, fontNormal8));

            table.AddCell(new Phrase("Voucher Date", fontBold8));
            table.AddCell(new Phrase(Model.EpayVDate, fontNormal8));

            if (txnId != 3185)//Added on 21-04-2023
            {
                cell = new PdfPCell(new Phrase("Package(s)", fontBold8));
                cell.Colspan = 1;
                cell.Border = 0;
                table.AddCell(cell);

                cell = new PdfPCell(new Phrase(Model.EpayVPackages, fontNormal8));
                cell.Colspan = 3;
                cell.Border = 0;
                table.AddCell(cell);

                cell = new PdfPCell(new Phrase("Agreement Number", fontBold8));
                cell.Colspan = 1;
                cell.Border = 0;
                table.AddCell(cell);

                cell = new PdfPCell(new Phrase(Model.AgreementNumber, fontNormal8));
                cell.Colspan = 3;
                cell.Border = 0;
                table.AddCell(cell);
            }

            cell = new PdfPCell(new Phrase("Net Amount (in Rs.)", fontBold8));
            cell.Colspan = 1;
            cell.Border = 0;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(Model.EpayAmount, fontNormal8));
            cell.Colspan = 3;
            cell.Border = 0;
            table.AddCell(cell);

            Phrase phraseAmount = new Phrase();
            phraseAmount.Add(new Chunk("Net Amount (in words): ", fontBold8));
            phraseAmount.Add(new Chunk(Model.EpayAmountInWord, fontNormal8));

            cell = new PdfPCell(new Phrase(phraseAmount));
            cell.Colspan = 4;
            cell.Border = 0;
            table.AddCell(cell);

            //cell = new PdfPCell(new Phrase(Model.EpayAmountInWord, fontNormal8));
            //cell.Colspan = 2;
            //cell.Border = 0;
            //table.AddCell(cell);


            PdfPTable t1 = new PdfPTable(1);
            t1.TotalWidth = 530f;
            PdfPCell cell1 = new PdfPCell();
            cell1.AddElement(table);
            cell1.BorderWidthBottom = 1f;
            cell1.BorderWidthLeft = 1f;
            cell1.BorderWidthTop = 1f;
            cell1.BorderWidthRight = 1f;
            t1.AddCell(cell1);


            // table.WriteSelectedRows(0, -1, doc.Left + 5, doc.Top - 210, writer.DirectContent);
            t1.WriteSelectedRows(0, -1, doc.Left + 5, doc.Top - 190, writer.DirectContent);
            canvas.SaveState();
            canvas.SetLineWidth(1f);
            //canvas.Rectangle(90, 35, 240f, 120f);
            canvas.Rectangle(300, 55, 240f, 100f);
            canvas.Stroke();
            canvas.RestoreState();





            Phrase strPhraseSignature = new Phrase("Signature of the person responsible for signing e-Payment", fontNormal8);
            ColumnText.ShowTextAligned(canvas, Element.ALIGN_LEFT, strPhraseSignature, 305, 90, 0);

            Phrase strPhraseName = new Phrase("Signed By: " + regsignmodel.NameAsPerCertificate, fontNormal8);
            ColumnText.ShowTextAligned(canvas, Element.ALIGN_LEFT, strPhraseName, 305, 75, 0);

            Phrase strPhraseDesg = new Phrase("Contact Number: " + regsignmodel.Mobile.ToString(), fontNormal8);
            ColumnText.ShowTextAligned(canvas, Element.ALIGN_LEFT, strPhraseDesg, 305, 60, 0);

            if (Model.IsNewResend.Equals("R"))
            {
                Phrase strNote = new Phrase("Note: This is Regenerated E-Payment. Original E-Payment date:" + Model.OrignalEpayDate + " Regenerated E-Payment date:" +
                    Model.EmailDate, fontBold10);
                ColumnText.ShowTextAligned(canvas, Element.ALIGN_LEFT, strNote, 20, 30, 0);
            }

            Phrase strPhrasePage = new Phrase("E-Payment details: Page 1 of 1 ", fontNormal11);
            ColumnText.ShowTextAligned(canvas, Element.ALIGN_LEFT, strPhrasePage, 20, 10, 0);

            strPhrasePage = new Phrase("Generated On:" + DateTime.Now.ToString("dd/MM/yyyy HH:mm tt", System.Globalization.CultureInfo.InvariantCulture), fontNormal11);
            ColumnText.ShowTextAligned(canvas, Element.ALIGN_LEFT, strPhrasePage, 200, 10, 0);


            strPhrasePage = new Phrase("NRRDA, All rights reserved.", fontNormal11);
            ColumnText.ShowTextAligned(canvas, Element.ALIGN_LEFT, strPhrasePage, 430, 10, 0);

            canvas.SaveState();
            canvas.SetLineWidth(1.0f);   // Make a bit thicker than 1.0 default
            canvas.SetColorStroke(iTextSharp.text.Color.BLACK); // 1 = black, 0 = white
            canvas.MoveTo(20, 22);
            canvas.LineTo(570, 22);
            canvas.Stroke();
            canvas.RestoreState();



            using (StreamWriter sw = System.IO.File.AppendText(ConfigurationManager.AppSettings["DigSignErrorLogPath"].ToString() + "PdfSignerAppletErrorLog_" + System.DateTime.Now.Date.ToString("dd_MM_yyyy") + ".txt"))
            {
                sw.WriteLine(" Out of ConvertHTMLToPDF :" + DateTime.Now.ToString() + " UserName:- " + PMGSYSession.Current.UserName);
                sw.Close();
            }
            doc.Close();


        }

        [Audit]
        protected void ConvertHTMLToPDFErem(string HTMLCode, String fileName, EremittnaceOrderModel Model)
        {

            DigSignBAL objBAL = new DigSignBAL();
            RegisterDSCModel regsignmodel = new RegisterDSCModel();
            regsignmodel = objBAL.GetDetailsToRegisterDSC();

            //Create PDF document 
            Document doc = new Document(PageSize.A4);
            PdfWriter writer = PdfWriter.GetInstance(doc, new FileStream(ConfigurationManager.AppSettings["PdfFilePath"].ToString() + fileName, FileMode.Create));
            doc.Open();

            iTextSharp.text.Font fontBoldU = new iTextSharp.text.Font(iTextSharp.text.FontFactory.GetFont("calibri", 12, iTextSharp.text.Font.UNDERLINE));
            iTextSharp.text.Font fontBold12 = new iTextSharp.text.Font(iTextSharp.text.FontFactory.GetFont("calibri", 12, iTextSharp.text.Font.BOLD));
            iTextSharp.text.Font fontBold = new iTextSharp.text.Font(iTextSharp.text.FontFactory.GetFont("calibri", 12, iTextSharp.text.Font.BOLD));
            //iTextSharp.text.Font fontBold11 = new iTextSharp.text.Font(iTextSharp.text.FontFactory.GetFont("calibri", 11, iTextSharp.text.Font.BOLD));
            iTextSharp.text.Font fontNormal11 = new iTextSharp.text.Font(iTextSharp.text.FontFactory.GetFont("calibri", 11, iTextSharp.text.Font.NORMAL));
            iTextSharp.text.Font fontBold9 = new iTextSharp.text.Font(iTextSharp.text.FontFactory.GetFont("calibri", 9, iTextSharp.text.Font.BOLD));
            iTextSharp.text.Font fontNormal8 = new iTextSharp.text.Font(iTextSharp.text.FontFactory.GetFont("calibri", 8, iTextSharp.text.Font.NORMAL));
            iTextSharp.text.Font fontBold8 = new iTextSharp.text.Font(iTextSharp.text.FontFactory.GetFont("calibri", 8, iTextSharp.text.Font.BOLD));
            iTextSharp.text.Font fontBold10 = new iTextSharp.text.Font(iTextSharp.text.FontFactory.GetFont("calibri", 10, iTextSharp.text.Font.BOLD));


            PdfContentByte canvas = writer.DirectContent;
            PdfPTable table = new PdfPTable(2);
            table.TotalWidth = 260f;
            table.LockedWidth = true;
            float[] tablet1widths = new float[] { 80f, 180f };
            table.SetWidths(tablet1widths);
            PdfPCell cell = new PdfPCell(new Phrase("Sender Information", fontBold9));
            cell.Colspan = 2;
            cell.HorizontalAlignment = 0;
            cell.BorderWidthBottom = 1f;
            cell.BorderWidthLeft = 0;
            cell.BorderWidthTop = 0;
            cell.BorderWidthRight = 0;

            table.AddCell(cell);
            table.DefaultCell.Border = Rectangle.NO_BORDER;

            table.AddCell(new Phrase("State", fontBold8));
            table.AddCell(new Phrase(Model.EpayState, fontNormal8));

            table.AddCell(new Phrase("DPIU", fontBold8));
            table.AddCell(new Phrase(Model.EpayDPIU + " ( District - " + Model.DPIUName + " ) ", fontNormal8));


            table.AddCell(new Phrase("Bank Details", fontBold8));
            table.AddCell(new Phrase(Model.Bankaddress, fontNormal8));

            table.AddCell(new Phrase("Account Number", fontBold8));
            table.AddCell(new Phrase(Model.BankAcNumber, fontNormal8));

            PdfPTable t2 = new PdfPTable(1);
            t2.TotalWidth = 260f;
            PdfPCell cell2 = new PdfPCell();
            cell2.AddElement(table);
            cell2.BorderWidthBottom = 1f;
            cell2.BorderWidthLeft = 1f;
            cell2.BorderWidthTop = 1f;
            cell2.BorderWidthRight = 1f;
            t2.AddCell(cell2);

            t2.WriteSelectedRows(0, -1, doc.Left + 5, doc.Top - 90, writer.DirectContent);

            table = new PdfPTable(2);
            table.TotalWidth = 260f;
            table.LockedWidth = true;
            float[] tablet2widths = new float[] { 70f, 190f };
            table.SetWidths(tablet2widths);
            cell = new PdfPCell(new Phrase("Department information", fontBold9));
            cell.Colspan = 2;
            cell.HorizontalAlignment = 0;
            cell.HorizontalAlignment = 0;
            cell.BorderWidthBottom = 1f;
            cell.BorderWidthLeft = 0;
            cell.BorderWidthTop = 0;
            cell.BorderWidthRight = 0;

            table.AddCell(cell);
            table.DefaultCell.Border = Rectangle.NO_BORDER;

            table.AddCell(new Phrase("Name", fontBold8));
            table.AddCell(new Phrase(Model.DepartmentNameFull, fontNormal8));

            //table.AddCell(new Phrase("Bank Account Number", fontBold8));
            //table.AddCell(new Phrase(Model.DepartmentAcNum, fontNormal8));

            table.AddCell(new Phrase("TAN Number", fontBold8));
            table.AddCell(new Phrase(@Model.DPIUTANNumber, fontNormal8));

            PdfPTable t3 = new PdfPTable(1);
            t3.TotalWidth = 260f;
            PdfPCell cell3 = new PdfPCell();
            cell3.AddElement(table);
            cell3.BorderWidthBottom = 1f;
            cell3.BorderWidthLeft = 1f;
            cell3.BorderWidthTop = 1f;
            cell3.BorderWidthRight = 1f;
            t3.AddCell(cell3);

            t3.WriteSelectedRows(0, -1, doc.Left + 270, doc.Top - 90, writer.DirectContent);

            table = new PdfPTable(4);
            table.TotalWidth = 530f;
            table.LockedWidth = true;
            table.DefaultCell.Border = Rectangle.NO_BORDER;
            float[] tablet3widths = new float[] { 80f, 180f, 80f, 180f };
            table.SetWidths(tablet3widths);

            cell = new PdfPCell(new Phrase("E-Remittance Information", fontBold9));
            cell.Colspan = 4;
            cell.BorderWidthBottom = 1f;
            cell.BorderWidthLeft = 0;
            cell.BorderWidthTop = 0;
            cell.BorderWidthRight = 0;
            cell.HorizontalAlignment = 1;
            table.AddCell(cell);

            table.AddCell(new Phrase("Date", fontBold8));
            table.AddCell(new Phrase(Model.EmailDate, fontNormal8));

            table.AddCell(new Phrase("Number", fontBold8));
            table.AddCell(new Phrase(Model.EpayNumber, fontNormal8));

            table.AddCell(new Phrase("Voucher Number", fontBold8));
            table.AddCell(new Phrase(Model.EpayVNumber, fontNormal8));

            table.AddCell(new Phrase("Voucher Date", fontBold8));
            table.AddCell(new Phrase(Model.EpayVDate, fontNormal8));

            cell = new PdfPCell(new Phrase("Net Amount (in Rs.)", fontBold8));
            cell.Colspan = 1;
            cell.Border = 0;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(Model.TotalAmount.ToString(), fontNormal8));
            cell.Colspan = 3;
            cell.Border = 0;
            table.AddCell(cell);

            Phrase phraseAmount = new Phrase();
            phraseAmount.Add(new Chunk("Net Amount (in words): ", fontBold8));
            phraseAmount.Add(new Chunk(Model.EpayTotalAmountInWord.Trim(), fontNormal8));

            cell = new PdfPCell(new Phrase(phraseAmount));
            cell.Colspan = 4;
            cell.Border = 0;
            table.AddCell(cell);


            PdfPTable t1 = new PdfPTable(1);
            t1.TotalWidth = 530f;
            PdfPCell cell1 = new PdfPCell();
            cell1.AddElement(table);
            cell1.BorderWidthBottom = 1f;
            cell1.BorderWidthLeft = 1f;
            cell1.BorderWidthTop = 1f;
            cell1.BorderWidthRight = 1f;
            t1.AddCell(cell1);

            t1.WriteSelectedRows(0, -1, doc.Left + 5, doc.Top - 180, writer.DirectContent);

            PdfPTable table2 = new PdfPTable(5);
            table2.TotalWidth = 530f;
            float[] widths2 = new float[] { 20f, 200f, 70f, 140f, 100f };
            table2.SetWidths(widths2);
            table2.WidthPercentage = 100f;
            table2.SpacingAfter = 10f;

            cell = new PdfPCell(new Phrase("Contractor information", fontBold9));
            cell.Colspan = 5;
            table2.AddCell(cell);

            table2.AddCell(new Phrase("Sr.No.", fontBold8));
            table2.AddCell(new Phrase("Name/Company Name", fontBold8));
            table2.AddCell(new Phrase("Contractor PAN No.", fontBold8));
            table2.AddCell(new Phrase("Agreement No.", fontBold8));
            table2.AddCell(new Phrase("Amount (in Rs.)", fontBold8));


            int j = 0;
            AddPageHeaderFooter(canvas, doc, "R", 1);
            int pageno = 0;
            for (int i = 0; i < Model.ContractorList.Count; i++)
            {
                if (i == 20)
                {
                    if (pageno == 0)
                    {
                        table2.WriteSelectedRows(0, -1, doc.Left + 5, doc.Top - 260, writer.DirectContent);
                        table2.FlushContent();
                    }
                    pageno = pageno + 1;
                    doc.NewPage();
                    j = 0;
                }
                else if (i > 20)
                {
                    if (j == 29)
                    {
                        doc.NewPage();
                        j = 0;
                        pageno = pageno + 1;
                        AddPageHeaderFooter(canvas, doc, "R", pageno);

                        table2.WriteSelectedRows(0, -1, doc.Left + 5, doc.Top - 90, writer.DirectContent);
                        table2.FlushContent();
                    }
                    else
                    {
                        j = j + 1;
                    }


                }
                table2.AddCell(new Phrase((i + 1).ToString(), fontNormal8));
                table2.AddCell(new Phrase(Model.ContractorList[i].EpayConName, fontNormal8));
                table2.AddCell(new Phrase(Model.ContractorList[i].EpayConPanNumber, fontNormal8));
                table2.AddCell(new Phrase(Model.ContractorList[i].EpayConAggreement, fontNormal8));
                PdfPCell amtcell = new PdfPCell(new Phrase(Model.ContractorList[i].EpayAmount, fontNormal8));
                amtcell.HorizontalAlignment = 2;
                table2.AddCell(amtcell);
            }

            if (pageno > 0)
            {
                doc.NewPage();
                pageno = pageno + 1;
                AddPageHeaderFooter(canvas, doc, "R", pageno);

                PdfPCell newAmtCell = new PdfPCell(new Phrase(Model.TotalAmount.ToString(), fontBold8));
                newAmtCell.Colspan = 5;
                newAmtCell.HorizontalAlignment = 2; //0=Left, 1=Centre, 2=Right
                table2.AddCell(newAmtCell);

                PdfPCell newAmountInWordCell = new PdfPCell(new Phrase(Model.EpayTotalAmountInWord, fontBold8));
                newAmountInWordCell.Colspan = 5;
                newAmountInWordCell.HorizontalAlignment = 2;
                table2.AddCell(newAmountInWordCell);

                table2.WriteSelectedRows(0, -1, doc.Left + 5, doc.Top - 90, writer.DirectContent);
            }
            else
            {
                pageno = pageno + 1;
                AddPageHeaderFooter(canvas, doc, "R", pageno);

                PdfPCell newAmtCell = new PdfPCell(new Phrase(Model.TotalAmount.ToString(), fontBold8));
                newAmtCell.Colspan = 5;
                newAmtCell.HorizontalAlignment = 2;
                table2.AddCell(newAmtCell);

                PdfPCell newAmountInWordCell = new PdfPCell(new Phrase(Model.EpayTotalAmountInWord, fontBold8));
                newAmountInWordCell.Colspan = 5;
                newAmountInWordCell.HorizontalAlignment = 2;
                table2.AddCell(newAmountInWordCell);

                table2.WriteSelectedRows(0, -1, doc.Left + 5, doc.Top - 260, writer.DirectContent);
            }

            canvas.SaveState();
            canvas.SetLineWidth(1f);

            canvas.Rectangle(300, 55, 240f, 100f);
            canvas.Stroke();
            canvas.RestoreState();


            if (Model.IsNewResend.Equals("R"))
            {
                Phrase strNote = new Phrase("Note: This is Regenerated E-Remittance.Original E-Remittance date:" + Model.OrignalEremiDate + "Regenerated E-Remittance date:" +
                    Model.EmailDate, fontBold9);
                ColumnText.ShowTextAligned(canvas, Element.ALIGN_LEFT, strNote, 20, 30, 0);
            }

            Phrase strPhraseSignature = new Phrase("Signature of the person responsible for signing e-Remittance", fontNormal8);
            ColumnText.ShowTextAligned(canvas, Element.ALIGN_LEFT, strPhraseSignature, 305, 90, 0);

            Phrase strPhraseName = new Phrase("Signed By: " + regsignmodel.NameAsPerCertificate, fontNormal8);
            ColumnText.ShowTextAligned(canvas, Element.ALIGN_LEFT, strPhraseName, 305, 75, 0);

            Phrase strPhraseDesg = new Phrase("Contact Number: " + regsignmodel.Mobile.ToString(), fontNormal8);
            ColumnText.ShowTextAligned(canvas, Element.ALIGN_LEFT, strPhraseDesg, 306, 60, 0);

            doc.Close();
        }


        protected void AddPageHeaderFooter(PdfContentByte canvas, Document doc, string epayType, int PageNo)
        {

            string strFund = string.Empty;
            if (PMGSY.Extensions.PMGSYSession.Current.FundType.Equals("A"))
            {
                strFund = "Administrative Expenses Fund";
            }
            else if (PMGSY.Extensions.PMGSYSession.Current.FundType.Equals("P"))
            {
                strFund = "Programme Fund";
            }
            else if (PMGSY.Extensions.PMGSYSession.Current.FundType.Equals("M"))
            {
                strFund = "Maintenance Fund";
            }

            iTextSharp.text.Font fontBoldU = new iTextSharp.text.Font(iTextSharp.text.FontFactory.GetFont("calibri", 12, iTextSharp.text.Font.UNDERLINE));
            iTextSharp.text.Font fontBold12 = new iTextSharp.text.Font(iTextSharp.text.FontFactory.GetFont("calibri", 12, iTextSharp.text.Font.BOLD));
            iTextSharp.text.Font fontNormal11 = new iTextSharp.text.Font(iTextSharp.text.FontFactory.GetFont("calibri", 11, iTextSharp.text.Font.NORMAL));
            iTextSharp.text.Font fontNormal9 = new iTextSharp.text.Font(iTextSharp.text.FontFactory.GetFont("calibri", 9, iTextSharp.text.Font.NORMAL));

            string str = Server.MapPath("~/Content/images/pmgsy-logo.png");
            Image jpg = Image.GetInstance(str);
            jpg.SetAbsolutePosition(30, 770);

            jpg.ScaleToFit(60f, 60f);
            doc.Add(jpg);

            ColumnText.ShowTextAligned(canvas, Element.ALIGN_CENTER, new Phrase("Pradhan Mantri Gram Sadak Yojana", fontBold12), 300, 790, 0);
            doc.Add(new Phrase(Environment.NewLine));

            canvas.SaveState();
            canvas.SetLineWidth(1.0f);
            canvas.SetColorStroke(iTextSharp.text.Color.BLACK);
            canvas.MoveTo(20, 770);
            canvas.LineTo(570, 770);
            canvas.Stroke();
            canvas.RestoreState();
            if (epayType.Equals("R"))
            {
                ColumnText.ShowTextAligned(canvas, Element.ALIGN_CENTER, new Phrase("E-Remittance Details", fontBoldU), 300, 750, 0);
                ColumnText.ShowTextAligned(canvas, Element.ALIGN_CENTER, new Phrase(strFund, fontNormal11), 300, 730, 0);
            }
            else if (epayType.Equals("E"))
            {
                ColumnText.ShowTextAligned(canvas, Element.ALIGN_CENTER, new Phrase("E-Payment Details", fontBoldU), 300, 750, 0);
                ColumnText.ShowTextAligned(canvas, Element.ALIGN_CENTER, new Phrase(strFund, fontNormal11), 300, 730, 0);
            }


            Phrase strPhrasePage = null;
            if (epayType.Equals("E"))
            {
                strPhrasePage = new Phrase("E-Payment details: Page " + PageNo.ToString(), fontNormal9);
            }
            else if (epayType.Equals("R"))
            {
                strPhrasePage = new Phrase("E-Remittance details: Page " + PageNo.ToString(), fontNormal9);
            }

            ColumnText.ShowTextAligned(canvas, Element.ALIGN_LEFT, strPhrasePage, 20, 10, 0);
            strPhrasePage = new Phrase("Generated On:" + DateTime.Now.ToString("dd/MM/yyyy HH:mm tt", System.Globalization.CultureInfo.InvariantCulture), fontNormal9);
            ColumnText.ShowTextAligned(canvas, Element.ALIGN_LEFT, strPhrasePage, 200, 10, 0);


            strPhrasePage = new Phrase("NRRDA, All rights reserved.", fontNormal9);
            ColumnText.ShowTextAligned(canvas, Element.ALIGN_LEFT, strPhrasePage, 430, 10, 0);

            canvas.SaveState();
            canvas.SetLineWidth(1.0f);
            canvas.SetColorStroke(iTextSharp.text.Color.BLACK);
            canvas.MoveTo(20, 22);
            canvas.LineTo(570, 22);
            canvas.Stroke();
            canvas.RestoreState();

        }



        #endregion

        #region Reject / Resend Epayment


        /// <summary>
        /// function to show epayment list at Authorized signatory page  for Reject / Resend Epayment
        /// </summary>
        /// <returns></returns>
        [Audit]
        public ActionResult GetEpayListForRejectResend()
        {
            TransactionParams objparams = new TransactionParams();
            RejectResendModel rejectResendModel = new RejectResendModel();
            try
            {
                if (PMGSYSession.Current.FundType == "A")
                {
                    //populate SRRDA
                    List<SelectListItem> lstSRRDA = new List<SelectListItem>();
                    lstSRRDA = common.PopulateSRRDA();
                    // ViewBag.SRRDA = lstSRRDA;
                    rejectResendModel.SRRDA_LIST = lstSRRDA;

                }
                List<SelectListItem> lstDPIU = common.PopulateDPIUOfSRRDA(PMGSYSession.Current.AdminNdCode);
                if (lstDPIU != null && lstDPIU.Count() != 0)
                {
                    lstDPIU.Insert(0, new SelectListItem { Text = "Select DPIU", Value = "0" });
                }
                rejectResendModel.DPIU_LIST = lstDPIU;
                rejectResendModel.BILL_MONTH_List = common.PopulateMonths(DateTime.Now.Month);
                rejectResendModel.BILL_YEAR_List = common.PopulateYears(DateTime.Now.Year);
                ViewBag.SessionSalt = Session["SessionSalt"].ToString();
                return View("GetEpayListForRejectResend", rejectResendModel);
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

        [HttpPost]
        public JsonResult GetEpaymentRejectResendList(FormCollection formCollection)
        {
            try
            {
                var srrda = formCollection["srrda"];
                PaymentFilterModel objFilter = new PaymentFilterModel();
                String TransactionType = String.Empty;

                if ((Request.Params["payType"] != "E") && (Request.Params["payType"] != "R"))
                {
                    throw new Exception("Invalid Mode of Transaction");
                }
                else
                {
                    TransactionType = Request.Params["payType"];
                }

                if (String.IsNullOrEmpty(Request.Params["dpiu"]))
                {
                    throw new Exception("Please select DPIU");
                }

                long totalRecords;
                objFilter.Month = Convert.ToInt16(Request.Params["month"]);
                objFilter.Year = Convert.ToInt16(Request.Params["year"]);

                objFilter.page = Convert.ToInt32(formCollection["page"]) - 1;
                objFilter.rows = Convert.ToInt32(formCollection["rows"]);
                objFilter.sidx = formCollection["sidx"];
                objFilter.sord = formCollection["sord"];

                objFilter.Bill_type = "P";
                //objFilter.AdminNdCode = Convert.ToInt32(Request.Params["dpiu"]);
                objFilter.AdminNdCode = (srrda == null || srrda == "0") ? Convert.ToInt32(Request.Params["dpiu"]) : Convert.ToInt32(Request.Params["srrda"]);
                objFilter.FundType = PMGSYSession.Current.FundType;
                //objFilter.LevelId = PMGSYSession.Current.LevelId;
                objFilter.LevelId = (srrda == null || srrda == "0") ? Convert.ToInt16(5) : Convert.ToInt16(4);

                var jsonData = new
                {
                    rows = objPaymentBAL.GetEpaymentRejectResendList(objFilter, TransactionType, out totalRecords),
                    total = totalRecords <= objFilter.rows ? 1 : totalRecords / objFilter.rows + 1,
                    page = Convert.ToInt32(formCollection["page"]),
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

        public ActionResult RejectResendForm(String id)
        {
            PaymentDAL paymentDAL = new PaymentDAL();

            RejectResendFormModel model = new RejectResendFormModel();
            var level = Request.Params["dpiu"] != "0" ? "5" : Request.Params["srrda"] != "0" ? "4" : "";
            List<SelectListItem> lstHeadID = new List<SelectListItem>();
            PaymentDAL objDAL = new PaymentDAL();

            lstHeadID = objDAL.populateFundTypeForCancellation(PMGSYSession.Current.FundType, Convert.ToInt16(level));
            model.PopulateHeadId = lstHeadID;

            model.Encrypted_BIllID_EpayID = Request.Params["EncId"];
            model.currentDate = System.DateTime.Now.ToString("dd/MM/yyyy");
            model.IsEpayErremi = Request.Params["EpayEremi"];

            model.CancelResend = Request.Params["CancelResend"];

            //PFMS Validations
            //if (model.IsEpayErremi == "E")
            //{
            //    return Json(new { success = false, message = "Cannot Reject/Resend Epayments" }, JsonRequestBehavior.AllowGet);
            //}

            //Get BIll_Id
            String[] encryptedParameters = model.Encrypted_BIllID_EpayID.Split('/');
            Dictionary<String, String> DecryptedParameter = URLEncrypt.DecryptParameters1(new String[] { encryptedParameters[0], encryptedParameters[1], encryptedParameters[2] });
            long Bill_ID = Convert.ToInt64(DecryptedParameter["BILL_ID"].ToString());

            model.BillDate = DecryptedParameter["BILL_DATE"].ToString().Replace('.', '/');

            model.EncBillID = URLEncrypt.EncryptParameters(new string[] { Bill_ID.ToString() });

            //Below Code is Commented on 05-01-2022
            //if (PMGSYSession.Current.FundType == "P" && model.CancelResend.Equals("C"))
            //{
            //    model.isPaymentRejected = paymentDAL.IsRejectedByPFMSDAL(Bill_ID);
            //}
            //Below Code is Addded on 05-01-2022
            if ((PMGSYSession.Current.FundType == "P" || PMGSYSession.Current.FundType == "A") && model.CancelResend.Equals("C"))
            {
                model.isPaymentRejected = paymentDAL.IsRejectedByPFMSDAL(Bill_ID);
            }
            //int i = DateTime.Compare(new CommonFunctions().GetStringToDateTime(model.BillDate), new DateTime(2018, 08, 02));
            //model.isPFMSPayment = i >= 0 ? true : false;
            return PartialView(model);
        }

        [HttpPost]
        public JsonResult SaveRejectResendDetails(RejectResendFormModel model)
        {

            try
            {
                //Validation Clear for Resened
                if (model.CancelResend.Equals("R"))
                {
                    if (ModelState.ContainsKey("HeadId"))
                        ModelState["HeadId"].Errors.Clear();
                }

                //Get BIll_Id Epay ID
                String[] encryptedParameters = model.Encrypted_BIllID_EpayID.Split('/');
                Dictionary<String, String> DecryptedParameter = URLEncrypt.DecryptParameters1(new String[] { encryptedParameters[0], encryptedParameters[1], encryptedParameters[2] });

                long Bill_ID = Convert.ToInt64(DecryptedParameter["BILL_ID"].ToString());
                int Epay_ID = Convert.ToInt32(DecryptedParameter["EPAY_ID"].ToString());

                if (ModelState.IsValid)
                {
                    //Date validation for cancel Epayment start

                    if (model.CancelResend.Equals("C"))
                    {
                        CommonFunctions obj = new CommonFunctions();

                        if (String.IsNullOrEmpty(model.BillDate))
                        {
                            return Json(new { status = false, message = "Invalid Bill Date" });
                        }

                        DateTime BillDate = obj.GetStringToDateTime(model.BillDate).Date;
                        DateTime CancelDate = obj.GetStringToDateTime(model.ResendDate).Date;

                        if (DateTime.Compare(CancelDate, BillDate) < 0)
                        {
                            return Json(new { status = false, message = "Cancel Date should be greate than epayment voucher Date." });
                        }
                        if (DateTime.Compare(CancelDate, DateTime.Now.Date) > 0)
                        {
                            return Json(new { status = false, message = "Cancel Date should be less than or equal to Today's Date." });
                        }
                    }

                    //Date validation for cancel Epayment end

                    String message = String.Empty;
                    CommonFunctions objCommon = new CommonFunctions();
                    //Below Condition is commneted on 05-01-2022
                    //if (PMGSYSession.Current.FundType == "P" && model.CancelResend.Equals("C"))

                    //Below Condition is modified on 05-01-2022
                    if ((PMGSYSession.Current.FundType == "P" || PMGSYSession.Current.FundType == "A") && model.CancelResend.Equals("C"))
                    {
                        //model.isPaymentRejected = paymentDAL.IsRejectedByPFMSDAL(Bill_ID);
                        if (model.isPaymentRejected)
                        {
                            model.UploadFileName = "NA";
                            model.Epay_ID = Epay_ID;
                            if ((CancelEpaymentEremDetails(model, Bill_ID, ref message)))
                            {
                                //return Json(new { status = false, message = (message == String.Empty ? "Error Occured while cancelling." : message) });
                                return Json(new { status = true, message = "Epayment Cancelled Successfully." });
                            }
                            else
                            {// Added on 14 Aug 2019 in case of false result retured.
                                return Json(new { status = false, message = "Epayment is not Cancelled." });
                            }
                        }
                    }

                    if (Request.Files.Count > 0)
                    {
                        //Validate File
                        if (!ValidateFile(ref message))
                        {
                            return Json(new { status = false, message = message });
                        }

                        //validate Epay Date                                                                                           
                        if (!(model.ResendDate.Equals(objCommon.GetDateTimeToString(System.DateTime.Now))) && model.CancelResend == "R")//&& model.CancelResend=="R"
                        {
                            return Json(new { status = false, message = (model.CancelResend.Equals("R") ? "Resend Date must be equal to current date." : "Cancellation Date must be equal to current date.") });
                        }

                        //Save File                                                                                                  
                        HttpPostedFileBase file = Request.Files[0];
                        String NonEpayFileName = String.Empty;
                        //Generate 20 Digit Random Number
                        Random random = new Random();
                        String RanNo = "";
                        for (int i = 1; i < 21; i++)
                        {
                            RanNo += random.Next(0, 9).ToString();
                        }
                        NonEpayFileName = RanNo + "_" + DateTime.Now.ToString("ddMMyyyy") + "_" + DateTime.Now.ToString("HHmmss") + Path.GetExtension(file.FileName);
                        file.SaveAs(Path.Combine(ConfigurationManager.AppSettings["NON_EPAYMENT_CERTIFICATE_FILE_UPLOAD"], NonEpayFileName));    //Uploaded File Save

                        //get Bill_Epay_ID
                        if (String.IsNullOrEmpty(model.Encrypted_BIllID_EpayID))
                        {
                            return Json(new { status = false, message = "Invalid Data." });
                        }
                        else
                        {
                            string NonEpayFilePath = ConfigurationManager.AppSettings["NON_EPAYMENT_CERTIFICATE_FILE_UPLOAD"].ToString() + NonEpayFileName;

                            model.UploadFileName = NonEpayFileName;
                            model.Epay_ID = Epay_ID;
                            if (System.IO.File.Exists(NonEpayFilePath))
                            {
                                if (model.CancelResend == "R")
                                {
                                    if (model.IsEpayErremi == "E")
                                    {
                                        //ResendEpaymentDetails(model, Bill_ID);

                                        //if (!(ResendEpaymentDetails(model, Bill_ID)))
                                        //{
                                        //    return Json(new { status = false, message = "Error Occured while creating PDF file" });
                                        //}

                                        objPaymentBAL.InsertResendEpaymentDetails(model, Bill_ID);
                                    }
                                    else
                                    {

                                        objPaymentBAL.InsertResendEpaymentDetails(model, Bill_ID);
                                        //ResendEremittanceDetails(model, Bill_ID);

                                        //if (!(ResendEremittanceDetails(model, Bill_ID)))
                                        //{
                                        //    return Json(new { status = false, message = "Error Occured while creating PDF file" });
                                        //}
                                    }
                                }
                                else
                                {
                                    //Cancel EpayEremittance Code  
                                    // String message = String.Empty;

                                    if (!(CancelEpaymentEremDetails(model, Bill_ID, ref message)))
                                    {
                                        return Json(new { status = false, message = (message == String.Empty ? "Error Occured while cancelling." : message) });
                                    }
                                }
                                return Json(new { status = true, message = (model.CancelResend == "R" ? (model.IsEpayErremi == "E" ? "E-Payment Details Resend Successfully." : "E-Remittance Details Resend Successfully.") : (model.IsEpayErremi == "E" ? "Epayment Cancelled Successfully." : "Eremittance Cancelled Successfully.")) });
                            }
                            else
                            {
                                return Json(new { status = false, message = "Non Epayment/Eremittance Certificate is Not uploaded." });
                            }
                        }
                    }
                    else
                    {
                        return Json(new { status = false, message = "Please select File" });
                    }
                }
                else
                {
                    return Json(new { status = false, message = "Invalid Data." });
                }
            }
            catch (Exception)
            {
                return Json(new { status = false, message = "An Error occued while proccessing your request." });
            }
        }


        //public bool ResendEremittanceDetails(RejectResendFormModel model, long Bill_ID)
        //{
        //    //Epay Resend            
        //    EremittnaceOrderModel epaymodel = new EremittnaceOrderModel();
        //    objPaymentBAL = new PaymentBAL();
        //    epaymodel = objPaymentBAL.GetEremittanceDetails(Bill_ID);
        //    //Set Epay Date As Current Date.            
        //    epaymodel.OrignalEremiDate = epaymodel.EpayDate;
        //    epaymodel.EmailDate = model.ResendDate;
        //    epaymodel.EpayDate = model.ResendDate;
        //    epaymodel.EmailSubject = "[Duplicate] " + epaymodel.EmailSubject;

        //    epaymodel.IsNewResend = "R";

        //    //Create Epayment PDF File
        //    String fileName = CreateEremittancePDFFile(epaymodel, Bill_ID);
        //    string EpayPDFPath = ConfigurationManager.AppSettings["PdfFilePath"].ToString() + fileName;
        //    model.EpayPDFFileName = fileName;

        //    if (System.IO.File.Exists(EpayPDFPath))//Epay PDF file created or not
        //    {
        //        String ErrorMessage = String.Empty;
        //        try
        //        {
        //            //save epayment mail details Here                         
        //            objPaymentBAL.InsertResendEpaymentDetails(model, Bill_ID);

        //            string strHeaderPath = Server.MapPath("~/Content/images/Header-e-gov-6.png");
        //            //send mail 
        //            //epaymodel.PdfFileName = fileName;                    
        //            objPaymentBAL.SendMailForEremOrder(epaymodel, ConfigurationManager.AppSettings["PdfFilePath"].ToString() + "S_" + fileName, strHeaderPath, ref ErrorMessage).Send();
        //            System.IO.File.Delete(EpayPDFPath);
        //            return true;
        //        }
        //        catch (Exception ex)
        //        {
        //            Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
        //            // code to remove the email details from the email master and email details
        //            string mailDeleteResult = objPaymentBAL.DeleteMailDetails(Bill_ID);
        //            //remove the secured pdf file created becouse mail has not been sent                   
        //            System.IO.File.Delete(Path.Combine(ConfigurationManager.AppSettings["NON_EPAYMENT_CERTIFICATE_FILE_UPLOAD"].ToString(), model.UploadFileName));
        //            System.IO.File.Delete(EpayPDFPath);
        //            System.IO.File.Delete(ConfigurationManager.AppSettings["PdfFilePath"].ToString() + "S_" + fileName);

        //            throw new Exception("Error while Resending E-Remittance.");
        //        }

        //        //delete the unsecured file based on which secured file is created
        //    }
        //    else
        //    {
        //        return false;
        //    }
        //}

        //public bool ResendEpaymentDetails(RejectResendFormModel model, long Bill_ID)
        //{
        //    //Epay Resend            
        //    EpaymentOrderModel epaymodel = new EpaymentOrderModel();
        //    objPaymentBAL = new PaymentBAL();
        //    epaymodel = objPaymentBAL.GetEpaymentDetails(Bill_ID);

        //    //Set Epay Date As Current Date.            
        //    epaymodel.OrignalEpayDate = epaymodel.EpayDate;
        //    epaymodel.EmailDate = model.ResendDate;
        //    epaymodel.EpayDate = model.ResendDate;

        //    epaymodel.EmailSubject = "[Duplicate] " + epaymodel.EmailSubject;
        //    epaymodel.IsNewResend = "R";

        //    //Create Epayment PDF File
        //    String fileName = CreateEpaymentPDFFile(epaymodel, Bill_ID,"");
        //    string EpayPDFPath = ConfigurationManager.AppSettings["PdfFilePath"].ToString() + fileName;
        //    model.EpayPDFFileName = fileName;

        //    if (System.IO.File.Exists(EpayPDFPath))//Epay PDF file created or not
        //    {
        //        String ErrorMessage = String.Empty;
        //        try
        //        {
        //            //save epayment mail details Here                         
        //            objPaymentBAL.InsertResendEpaymentDetails(model, Bill_ID);

        //            string strHeaderPath = Server.MapPath("~/Content/images/Header-e-gov-6.png");
        //            //send mail 
        //            //epaymodel.PdfFileName = fileName;
        //            objPaymentBAL.EpayOrderMail(epaymodel, ConfigurationManager.AppSettings["PdfFilePath"].ToString() + "S_" + fileName, strHeaderPath, ref ErrorMessage).Send();
        //            System.IO.File.Delete(EpayPDFPath);
        //            return true;
        //        }
        //        catch (Exception ex)
        //        {
        //            Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
        //            // code to remove the email details from the email master and email details
        //            string mailDeleteResult = objPaymentBAL.DeleteMailDetails(Bill_ID);

        //            //remove the secured pdf file created becouse mail has not been sent
        //            System.IO.File.Delete(Path.Combine(ConfigurationManager.AppSettings["NON_EPAYMENT_CERTIFICATE_FILE_UPLOAD"].ToString(), model.UploadFileName)); //Delete uploaded file
        //            System.IO.File.Delete(EpayPDFPath);// delete insecure pdf file
        //            System.IO.File.Delete(ConfigurationManager.AppSettings["PdfFilePath"].ToString() + "S_" + fileName); //delete secure pdf file

        //            throw new Exception("Error while Resending Epayment.");
        //        }
        //    }
        //    else
        //    {
        //        return false;
        //    }
        //}


        public bool CancelEpaymentEremDetails(RejectResendFormModel model, long Bill_ID, ref string message)
        {
            objPaymentBAL = new PaymentBAL();

            try
            {
                return objPaymentBAL.CancelEpaymentEremDetails(model, Bill_ID, ref message);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);

                System.IO.File.Delete(Path.Combine(ConfigurationManager.AppSettings["NON_EPAYMENT_CERTIFICATE_FILE_UPLOAD"].ToString(), model.UploadFileName)); //Delete uploaded file

                throw new Exception("Error while cancelling Epayment.");
            }
        }


        //validation for File Type
        public bool ValidateFile(ref string message)
        {
            CommonFunctions objCommonFunc = new CommonFunctions();
            HttpPostedFileBase file = Request.Files[0];
            string fileExtension = Path.GetExtension(file.FileName);
            //Array of File Types to Validate 
            String[] fileTypes = new String[] { "jpeg" };

            long FileSize = 2097152; // 2 Mb =2097152 bytes

            switch (fileExtension.Trim().ToLower())
            {
                case ".pdf":
                    if (!(objCommonFunc.ValidateIsPdf(ConfigurationManager.AppSettings["PdfFilePath"], Request)))
                    {
                        message = "File Type is Not Allowed.";
                        return false;
                    }
                    else
                    {
                        //File Length Validation          
                        if (!(objCommonFunc.ValidateFileLength(ConfigurationManager.AppSettings["PdfFilePath"], Request, FileSize)))
                        {
                            message = "Maximum size of an File must not be greater than 2 MB";
                            return false;
                        }
                        else
                        {
                            return true;
                        }
                        //File Type Validation                      
                    }
                //break; 
                case ".jpg":
                case ".jpeg":
                    if (!(objCommonFunc.IsValidImageFile(ConfigurationManager.AppSettings["PdfFilePath"], Request, fileTypes)))
                    {
                        message = "File Type is Not Allowed.";
                        return false;
                    }
                    else
                    {
                        //File Length Validation          
                        if (!(objCommonFunc.ValidateFileLength(ConfigurationManager.AppSettings["PdfFilePath"], Request, FileSize)))
                        {
                            message = "Maximum size of an File must not be greater than 2 MB";
                            return false;
                        }
                        else
                        {
                            return true;
                        }
                        //File Type Validation                        
                    }
                default:
                    message = "File Type is Not Allowed.";
                    return false;
            }
            //return false;
        }

        #endregion Reject / Resend Epayment

        // new methods added by Vikram in 29-08-2013
        [HttpPost]
        [Audit]
        public JsonResult ValidateDPIUEpayment()
        {
            bool status = false;
            try
            {
                objPaymentBAL = new PaymentBAL();
                status = objPaymentBAL.ValidateDPIUEpaymentBAL(PMGSYSession.Current.AdminNdCode);
                if (status == true)
                {
                    return Json(new { success = true });
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


        [HttpPost]
        [Audit]
        public JsonResult ValidateAuthSignRegistration()
        {
            bool status = false;
            DigSignBAL objBAL = new DigSignBAL();
            RegisterDSCModel model = new RegisterDSCModel();
            model = objBAL.GetDetailsToRegisterDSC();
            if (model.NodalOfficerCode == 0)
            {
                model.IsAlreadyRegistered = 0;
            }
            else
            {
                if (objBAL.IsAlreadyRegistered(model.NodalOfficerCode))
                {
                    model.IsAlreadyRegistered = 1; //Certificate Details are available in Ommas 

                }
                else
                {
                    model.IsAlreadyRegistered = 2; //Certificate Details are not available in Ommas 
                }
            }
           
            if ((model.IsAlreadyRegistered == 2) || (model.IsAlreadyRegistered == 0))
            {

                return Json(new { success = false });
            }
            else
            {

                if (model.IsValidXmlDscRegisteredREAT == true) // Dsc registered in REAT Module 
                {
                    if (model.DSCDeregCheck == false)
                    {
                        return Json(new { success = false });  // DSC deregistration request is processed at REAT 
                    }
                    else
                    {
                        return Json(new { success = true });
                    }
                }
                else
                {
                    return Json(new { success = false });
                };

                
            }

        }


        [HttpPost]
        [Audit]
        public JsonResult ValidateDPIUEremittence()
        {
            bool status = false;
            try
            {
                objPaymentBAL = new PaymentBAL();
                status = objPaymentBAL.ValidateDPIUEremittenceBAL(PMGSYSession.Current.AdminNdCode);
                if (status == true)
                {
                    return Json(new { success = true });
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


        //new method added by Vikram 
        [Audit]
        public bool ValidateRoad(int headId, int proposalCode, string fundType)
        {
            string upgradeConnectFlag = string.Empty;
            common = new CommonFunctions();
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
                        ///Changed by SAMMED A. PATIL on 20FEB2018 check for RCPLWE Scheme Transaction Id 1789,1790
                        case 1789:
                            upgradeConnectFlag = "N";
                            break;
                        case 1790:
                            upgradeConnectFlag = "U";
                            break;

                        default:
                            upgradeConnectFlag = "A";
                            break;
                    }

                    bool status = common.ValidateRoad(proposalCode, upgradeConnectFlag);
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

        //Added By Abhishek kamble 2 Sep 2014
        public bool ValidateRoadForPMGSYScheme(short txnID, int RoadCode)
        {

            try
            {
                CommonFunctions commonFuncObj = new CommonFunctions();
                bool status = commonFuncObj.ValidateRoadForPMGSYScheme(txnID, RoadCode);
                return status;

            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return false;
            }
        }

        public JsonResult ValidateFinalPaymentTransaction(String parameter, String hash, String key)
        {
            PMGSYEntities db = new PMGSYEntities();
            try
            {
                Dictionary<int, int> lstTransactions = new Dictionary<int, int>();
                lstTransactions.Add(1, 105);
                String[] decryptedParameters = null;
                Int64 billId = 0;
                decryptedParameters = URLEncrypt.DecryptParameters(new string[] { parameter, hash, key });
                String[] param = decryptedParameters[0].Split('$');
                billId = Convert.ToInt64(param[0]);
                int txnId = db.ACC_BILL_MASTER.Where(m => m.BILL_ID == billId).Select(m => m.TXN_ID).FirstOrDefault();
                if (lstTransactions.Any(m => m.Value == txnId))
                {
                    return Json(new { Disable = true });
                }
                else
                {
                    return Json(new { Disable = false });
                }

            }
            catch (Exception ex)
            {
                return Json(new { Disable = false });
            }
        }


        [Audit]
        public JsonResult GetPayeeSupplierDetails(string id)
        {
            CommonFunctions objCommon = new CommonFunctions();

            TransactionParams objparams = new TransactionParams();
            objparams.TXN_ID = Convert.ToInt16(id.Split('$')[0]);

            //Below code Added on 17-11-2021
            try
            {
                return Json(common.PopulateContractorSupplier(objparams));
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "Payment.GetPayeeSupplierDetails()");
                return Json(new String[] { });
            }

            //Below code commented on 17-11-2021
            //try
            //{
            //    if (objparams.TXN_ID == 472 || objparams.TXN_ID == 415 || objparams.TXN_ID == 455)
            //    {
            //        objparams.BILL_TYPE = "P";
            //        objparams.FUND_TYPE = PMGSYSession.Current.FundType;
            //        objparams.LVL_ID = PMGSYSession.Current.LevelId;
            //        objparams.ADMIN_ND_CODE = PMGSYSession.Current.AdminNdCode;
            //        objparams.DISTRICT_CODE = PMGSYSession.Current.DistrictCode;
            //        objparams.STATE_CODE = PMGSYSession.Current.StateCode;
            //        objparams.MAST_CON_SUP_FLAG = "S";

            //        return Json(common.PopulateContractorSupplier(objparams));
            //    }
            //    else
            //    {
            //        return Json(new String[] { });
            //    }
            //}
            //catch (Exception ex)
            //{
            //    Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
            //    string errorMsg = string.Format("Errors: {0}", ex.Message);
            //    Response.TrySkipIisCustomErrors = true;
            //    Response.StatusCode = 500;
            //    Response.Write(errorMsg);
            //    return Json(new String[] { });
            //}
        }


        [HttpPost]
        public ActionResult GetContractorBankGuranteeDetails(string id)
        {
            try
            {
                PMGSYEntities dbContext = null;
                String[] urlParams = id.Split('$');
                int agreementCode = Convert.ToInt32(urlParams[0].ToString());
                int txnId = Convert.ToInt32(urlParams[2].ToString());




                dbContext = new PMGSYEntities();
                //                decimal CHQ_AMOUNT = Convert.ToDecimal(Request.QueryString["CHQ_AMOUNT"]);

                ArrayList ByPassAgrCodeList = new ArrayList();
                if (System.IO.File.Exists(System.Web.HttpContext.Current.Server.MapPath("~/BypassBankGuaranteeCodes.xml")))
                {
                    XDocument xmlDoc = XDocument.Load(System.Web.HttpContext.Current.Server.MapPath("~/BypassBankGuaranteeCodes.xml"));

                    if (xmlDoc != null)
                    {
                        foreach (XElement element in
                        xmlDoc.Descendants("Agreements").Descendants("ByPassAgreement"))
                        {
                            //var agreementCode = element.Element("Value").Value;
                            var TendAgreementCode = element.Element("Value").Value;
                            ByPassAgrCodeList.Add(TendAgreementCode);
                        }
                    }
                }





                if (PMGSYSession.Current.FundType.Equals("P") && (PMGSYSession.Current.LevelId == 5))
                {
                    //provide relaxation to userId 1315, Added on 12-10-2022
                    if (PMGSYSession.Current.UserId == 1315 && PMGSYSession.Current.StateCode == 5 && DateTime.Now.Date <= new CommonFunctions().GetStringToDateTime("22/10/2022"))
                    {
                        return Json(new { Success = true }, JsonRequestBehavior.AllowGet);
                    }

                    //if (CHQ_AMOUNT > 0)
                    //{
                    if (!(txnId == 113 || txnId == 109 || txnId == 105 || txnId == 120 || txnId == 86 || txnId == 118 || txnId == 134 || txnId == 137 || txnId == 1488 || txnId == 1546 || txnId == 1553 || txnId == 1661 || txnId == 2000 || txnId == 42 || txnId == 1485 || txnId == 1814 || txnId == 1997 || txnId == 3185 || txnId == 3187 || txnId==3117))//txn_id=3185 added on 21-04-2023 to bypass validation, txn_id=3117 added on 07-08-2023
                    {
                        var it = dbContext.USP_ACC_VALIDATE_BANK_GURANTEE_DETAILS(PMGSYSession.Current.AdminNdCode, PMGSYSession.Current.FundType, agreementCode);
                        int flag = 0;
                        double bankGuranteeAmount = 0;
                        foreach (var item in it)
                        {

                            if (item.VALIDATION_ID == 1)
                            {
                                if (item.ADVANCE_PAYMENT == 0) // if no advance payment made on the selected agreement
                                {
                                    flag = 1;  //Added on 23 March 2021 by Aditi to include Bank Guarantee Validation for PMGSY 3 and below line is commented so that execution goes to Validation 2 and 3
                                               //return Json(new { Success = true }, JsonRequestBehavior.AllowGet); // exclude validation 
                                }
                                else
                                {
                                    ///Added by SAMMED A. PATIL on 05APR2018 for payupbaghpat issue for bank guarantee.
                                    if (item.amount == 0)
                                    {
                                        flag = 1; //Added on 23 March 2021 by Aditi to include Bank Guarantee Validation for PMGSY 3 and below line is commented so that execution goes to Validation 2 and 3
                                                  // return Json(new { Success = true }, JsonRequestBehavior.AllowGet); // exclude validation 
                                    }
                                    else
                                    {
                                        flag = 1;
                                    }
                                }
                            }
                            if (flag == 1)
                            {
                                if (item.VALIDATION_ID == 2)
                                {
                                    if ((item.TEND_BG_EXPIRY_DATE == null) || (item.TEND_BG_EXPIRY_DATE.ToString() == string.Empty))
                                    {
                                        return Json(new { Success = false, ErrorMessage = "Contractor Bank Gurantee Details are not available for selected agreement" }, JsonRequestBehavior.AllowGet);
                                    }

                                    if (item.TEND_BG_EXPIRY_DATE < DateTime.Now && !ByPassAgrCodeList.Contains(agreementCode.ToString()))
                                    {
                                        return Json(new { Success = false, ErrorMessage = "Bank Gurantee date for selected contractor agreement is expired" }, JsonRequestBehavior.AllowGet);
                                    }
                                    else
                                    {
                                        bankGuranteeAmount = Convert.ToDouble(item.amount);
                                    }
                                }

                                //if (item.VALIDATION_ID == 3)
                                //{
                                //    if (!(Convert.ToDouble(item.amount) <= bankGuranteeAmount))
                                //    {
                                //        return Json(new { Success = false, ErrorMessage = "Bank Gurantee amount is less than than advance payment amount" }, JsonRequestBehavior.AllowGet);
                                //    }
                                //}

                                if (txnId == 72)
                                {
                                    if (item.VALIDATION_ID == 3)
                                    {
                                        if (!(Convert.ToDouble(item.amount) <= bankGuranteeAmount))
                                        {
                                            return Json(new { Success = false, ErrorMessage = "Bank Gurantee amount is less than than advance payment amount" }, JsonRequestBehavior.AllowGet);
                                        }
                                    }
                                }
                            }
                        }
                    }
                    return Json(new { Success = true }, JsonRequestBehavior.AllowGet);

                    //}
                    //else
                    //{
                    //    return Json(new { Success = true }, JsonRequestBehavior.AllowGet);
                    //}
                }
            }
            catch (Exception ex)
            {
                return Json(new { Success = false, ErrorMessage = "An error occured while getting contractor bank guarantee details." }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { Success = true }, JsonRequestBehavior.AllowGet);

        }


        public ActionResult VerifyDPRAgreement(string id)
        {
            try
            {

                PMGSYEntities dbContext = new PMGSYEntities();
                // PMGSYEntities dbContext = null;
                String[] urlParams = id.Split('$');
                int contractorCode = Convert.ToInt32(urlParams[0].ToString());
                int agreementCode = Convert.ToInt32(urlParams[1].ToString());
                string fundtype = urlParams[2].ToString();
                int TXNId = Convert.ToInt32(urlParams[3]);

                if (fundtype.Equals("P"))
                {
                    string proposalType = (dbContext.TEND_AGREEMENT_MASTER.Where(c => c.MAST_CON_ID == contractorCode && c.TEND_AGREEMENT_CODE == agreementCode).Select(c => c.TEND_AGREEMENT_TYPE).First());
                    if (proposalType.Equals("D") || (proposalType.Equals("S") && TXNId == 86))///Disable Road selection for TXN Refund of Deposits to Contractor / Supplier
                    {
                        return Json(new { Success = true }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json(new { Success = false }, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    return Json(new { Success = false }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (DbEntityValidationException ex)
            {
                // Retrieve the error messages as a list of strings.
                var errorMessages = ex.EntityValidationErrors
                        .SelectMany(x => x.ValidationErrors)
                        .Select(x => x.ErrorMessage);

                // Join the list to a single string.
                var fullErrorMessage = string.Join("; ", errorMessages);

                // Combine the original exception message with the new one.
                var exceptionMessage = string.Concat(ex.Message, " The validation errors are: ", fullErrorMessage);

                // Throw a new DbEntityValidationException with the improved exception message.
                throw new DbEntityValidationException(exceptionMessage, ex.EntityValidationErrors);
            }

        }

        #region PFMS Execution Payment Validation Configuration

        [HttpGet]
        public ActionResult PaymentValidationLayout()
        {
            try
            {
                return View();
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "Payment.PaymentValidationLayout()");
                return null;
            }
        }

        [HttpGet]
        public ActionResult SearchPaymentValidationLayout()
        {
            ExecPaymentValidationViewModel model = new ExecPaymentValidationViewModel();
            CommonFunctions comm = new CommonFunctions();
            TransactionParams objParam = new TransactionParams();
            try
            {
                model.lstSRRDA = new List<SelectListItem>();
                model.lstSRRDA = comm.PopulateNodalAgencies();

                //Populate DPIU
                objParam.ADMIN_ND_CODE = PMGSYSession.Current.AdminNdCode;

                model.lstDPIU = comm.PopulateDPIU(objParam);
                model.lstDPIU.RemoveAt(0);
                //model.lstDPIU.Insert(0, new SelectListItem { Text = "Select DPIU", Value = "0" });
                model.lstDPIU.Insert(0, new SelectListItem { Text = "All DPIU", Value = "0" });

                model.srrda_Dpiu = "S";
                model.roadSelection = "0";

                //model.lstYear = comm.PopulateYears(true);
                //model.lstRoadCode = new List<SelectListItem>();
                //model.lstRoadCode.Insert(0, new SelectListItem() { Text = "Select Road", Value = "0" });

                return View(model);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "Payment.SearchPaymentValidationLayout()");
                return null;
            }
        }

        [HttpGet]
        public ActionResult ExecPaymentValidationLayout()
        {
            ExecPaymentValidationViewModel model = new ExecPaymentValidationViewModel();
            CommonFunctions comm = new CommonFunctions();
            TransactionParams objParam = new TransactionParams();
            try
            {
                model.lstSRRDA = new List<SelectListItem>();
                model.lstSRRDA = comm.PopulateNodalAgencies();

                //Populate DPIU
                objParam.ADMIN_ND_CODE = PMGSYSession.Current.AdminNdCode;

                model.lstDPIU = comm.PopulateDPIU(objParam);
                model.lstDPIU.RemoveAt(0);
                //model.lstDPIU.Insert(0, new SelectListItem { Text = "Select DPIU", Value = "0" });
                model.lstDPIU.Insert(0, new SelectListItem { Text = "All DPIU", Value = "0" });

                model.srrda_Dpiu = "S";
                model.roadSelection = "0";

                model.lstYear = comm.PopulateYears(true);
                model.lstRoadCode = new List<SelectListItem>();
                model.lstRoadCode.Insert(0, new SelectListItem() { Text = "Select Road", Value = "0" });

                return View(model);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "Payment.ExecPaymentValidationLayout()");
                return null;
            }
        }

        [HttpGet]
        public ActionResult GetValidationDetails(int? page, int? rows, string sidx, string sord)
        {
            int adminNdCode = 0;
            string frmDt = string.Empty, toDt = string.Empty;

            int totalRecords = 0;
            PaymentDAL paymentDAL = new PaymentDAL();

            try
            {

                using (CommonFunctions commonFunction = new CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new GridParams(page, rows, sidx, sord, Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                    {
                        return null;
                    }
                }

                adminNdCode = Convert.ToInt32(Request.Params["AdminNdCode"]);
                frmDt = Convert.ToString(Request.Params["frmDt"]);
                toDt = Convert.ToString(Request.Params["toDt"]);

                var jsonData = new
                {
                    rows = paymentDAL.GetValidationDetailsDAL(adminNdCode, frmDt, toDt, Convert.ToInt32(page) - 1, Convert.ToInt32(rows), sidx, sord, out totalRecords),
                    total = totalRecords <= Convert.ToInt32(rows) ? 1 : totalRecords / Convert.ToInt32(rows) + 1,
                    page = Convert.ToInt32(page),
                    records = totalRecords,
                };

                return Json(jsonData, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "Payment.GetValidationDetails()");
                return null;
            }
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
                    lstDpiu.Insert(0, new SelectListItem { Text = "All DPIU", Value = "0" });
                    return Json(lstDpiu, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    List<SelectListItem> lstDPIU = objCommonFunction.PopulateDPIU(objParam);
                    lstDPIU.RemoveAt(0);
                    //lstDPIU.Insert(0, new SelectListItem { Text = "Select DPIU", Value = "0" });
                    lstDPIU.Insert(0, new SelectListItem { Text = "All DPIU", Value = "0" });

                    return Json(new SelectList(lstDPIU, "Value", "Text"), JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "Payment.PopulateDPIUForCashBook()");
                return Json(false);
            }
        }

        [HttpGet]
        public ActionResult PopulateRoads(int adminNdCode, string srrdaDpiu, int year)
        {
            CommonFunctions objCommonFunctions = new CommonFunctions();
            PaymentDAL paymentDAL = new PaymentDAL();
            try
            {
                List<SelectListItem> list = paymentDAL.PopulateRoadsDAL(adminNdCode, srrdaDpiu, year);
                //list.Find(x => x.Value == "-1").Value = "0";
                return Json(list, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "Payment.PopulateRoads()");
                return null;
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddValidationDetails(ExecPaymentValidationViewModel model)
        {
            string message = string.Empty;
            bool flag = false;
            PaymentDAL paymentDAL = new PaymentDAL();
            try
            {
                if (model.roadSelection == "0")
                {
                    ModelState.Remove("Year");
                    ModelState.Remove("roadCode");
                }
                if (ModelState.IsValid)
                {
                    flag = paymentDAL.AddValidationDetailsDAL(model, ref message);

                    return Json(new { status = flag, message = message });
                }
                else
                {
                    message = ModelState.Values.Where(x => x.Errors.Count == 1).FirstOrDefault().Errors[0].ErrorMessage;
                    return Json(new { status = flag, message = message });
                }
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "Payment.AddValidationDetails");
                return Json(new { status = flag, message = message });
            }
        }
        #endregion



        #region Transfer Deduction Amount to Holding account vikky
        [Audit]
        public ActionResult TransferDeductionAmtToHoldingAcc(String id)
        {
            #region Region to read HoldingSecurityUAT XML File
            try
            {
                int AllowUATflag = 0;
                AllowUATflag = HoldingSecDepUATAllowStatus();
                if (AllowUATflag == 0)
                {
                    return View("HoldingSecurityModuleUATView");
                }
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "PaymentController.TransferDeductionAmtToHoldingAcc()");

            }

            #endregion

            TransactionParams objparams = new TransactionParams();
            try
            {
                PMGSYEntities dbContext = new PMGSYEntities();
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


                //    List<SelectListItem> deductionTypeList = new SelectList(dbContext.REAT_MATSER_DEDUCTION_TYPE.Where(s=>s.IS_ACTIVE==true), "HEAD_ID", "HEAD_DESC",HEA).ToList();

                SelectListItem item = new SelectListItem();
                List<SelectListItem> deductionTypeList = new List<SelectListItem>();


                var list = (from c in dbContext.REAT_MATSER_DEDUCTION_TYPE

                            where c.IS_ACTIVE == true
                            select new
                            {
                                Text = c.HEAD_DESC,
                                Value = c.HEAD_DESC
                            }).OrderBy(c => c.Value).Distinct().ToList();
                foreach (var data in list)
                {
                    if (data.Text != "MSC" && data.Text != "OTH")
                    {
                        item = new SelectListItem();
                        item.Text = data.Text;
                        item.Value = data.Value.ToString();
                        deductionTypeList.Add(item);

                    }

                }

                deductionTypeList.Insert(0, (new SelectListItem { Text = "Select All", Value = "0" }));



                List<SelectListItem> voucherStatusList = new List<SelectListItem>();
                SelectListItem item1 = new SelectListItem();
                item1.Text = "Pending Voucher";
                item1.Value = "P";
                voucherStatusList.Add(item1);
                SelectListItem item2 = new SelectListItem();
                item2.Text = "Generated Voucher";
                item2.Value = "G";
                voucherStatusList.Add(item2);

                ViewData["voucherStatusList"] = voucherStatusList;
                ViewData["DeductionTypeList"] = deductionTypeList;
                ViewData["months"] = monthList;
                ViewData["year"] = yearList;

                ViewBag.levelID = PMGSYSession.Current.LevelId;
                List<SelectListItem> transactionType = common.PopulateTransactions(objparams);
                ViewData["TXN_ID"] = transactionType;
                return View();
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

        [Audit]
        public JsonResult GetTransferDeductionAmtList(int? page, int? rows, string sidx, string sord)
        {
            try
            {
                List<string> voucherGeneratedList;
                objPaymentBAL = new PaymentBAL();
                PaymentFilterModel objFilter = new PaymentFilterModel();
                objFilter.FilterMode = Request.Params["mode"].Trim();

                string TransactionType = string.Empty;

                if (Request.Params["payType"] != "E" && Request.Params["payType"] != "R")
                {
                    throw new Exception("Invalid Parameters");
                }
                else
                {
                    TransactionType = Request.Params["payType"];
                }

                long totalRecords;
                //objFilter.Month = Convert.ToInt16(Request.Params["months"]);
                //objFilter.Year = Convert.ToInt16(Request.Params["year"]);
                objFilter.deductionType = (Request.Params["DeductionType"]).ToString();
                if (objFilter.FilterMode.Equals("Search"))
                {
                    objFilter.FromDate = Request.Params["fromDate"].ToString() == String.Empty ? null : Request.Params["fromDate"].ToString();
                    objFilter.ToDate = Request.Params["toDate"].ToString() == String.Empty ? null : Request.Params["toDate"].ToString();
                    objFilter.TransId = Request.Params["transType"] == String.Empty ? Convert.ToInt16(0) : Convert.ToInt16(Request.Params["transType"].Trim().Split('$')[0]);

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
                    rows = objPaymentBAL.GetTransferDeductionAmtListBAL(objFilter, TransactionType, out totalRecords, out voucherGeneratedList),
                    total = totalRecords <= objFilter.rows ? 1 : totalRecords / objFilter.rows + 1,
                    page = page,
                    records = totalRecords,
                    userdata = new { ids = voucherGeneratedList.ToArray<string>() }
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



        public ActionResult IsAssignVouchersCorrect(string[] submitarray)
        {
            try
            {
                PMGSYEntities dbContext = new PMGSYEntities();
                bool status = true;
                string firstSelectedRecord = submitarray[0].Split('_')[3].ToString();
                for (int i = 0; i < submitarray.Length; i++)
                {
                    if (!(submitarray[i].Split('_')[3].ToString().Equals(firstSelectedRecord)))
                    {
                        return Json(new { success = false, Msg = "Please select records of same Deduction Type" });
                    }
                    long snaBillId = Convert.ToInt64(submitarray[i].Split('_')[0]);
                    short snaBillTxnNo = Convert.ToInt16(submitarray[i].Split('_')[1]);

                    if (dbContext.ACC_BILL_SNA_HOLDING_MAPPING.Any(s => s.SNA_BILL_ID == snaBillId && s.SNA_BILL_TXN_NO == snaBillTxnNo))
                    {
                        string voucherName = dbContext.ACC_BILL_MASTER.Where(s => s.BILL_ID == snaBillId).Select(m => m.BILL_NO).FirstOrDefault().ToString();
                        return Json(new { success = false, Msg = "The vouchers are already generated for the selected record " + voucherName });
                    }
                }

                return Json(new { success = status, Msg = "Please proceed" });
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "QualityMonitoringController.AssignTechExpert");
                return Json(new { success = false }, JsonRequestBehavior.DenyGet);
            }
        }


        [HttpPost]
        public ActionResult TransferDeductionAmtDilogueBox(string[] submitArray)
        {
            PaymentMasterModel paymentMasterModel = new PaymentMasterModel();
            CommonFunctions objCommon = new CommonFunctions();
            ACC_BILL_MASTER masterDetails = new ACC_BILL_MASTER();
            // masterDetails = objPaymentBAL.GetMasterPaymentDetails(billId);
            paymentMasterModel.TextConcat = "";
            for (int i = 0; i < submitArray.Length; i++)
            {

                paymentMasterModel.TextConcat += submitArray[i];
                if (submitArray.Length > i + 1)
                {
                    paymentMasterModel.TextConcat += "*";
                }
            }




            //string [] submitArray= paymentMasterModel.TextConcat.Split('&');
            decimal deductionAmtSum = 0;
            for (int i = 0; i < submitArray.Length; i++)
            {

                deductionAmtSum = deductionAmtSum + Convert.ToDecimal(submitArray[i].Split('_')[2].Replace('A', '.'));

            }
            // paymentMasterModel.BILL_ID = billId;
            //paymentMasterModel.PAYEE_NAME = masterDetails.PAYEE_NAME;
            paymentMasterModel.DEDUCTION_AMOUNT = deductionAmtSum;
            paymentMasterModel.BILL_DATE = objCommon.GetDateTimeToString(DateTime.Now);

            return PartialView("TransferDeductionAmtDilogueBoxView", paymentMasterModel);
        }
        [HttpPost]
        public JsonResult SubmitTransferDeductionAmountToHoldingAcc(string formData)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            CommonFunctions objCommon = new CommonFunctions();
            TransactionParams objparams = new TransactionParams();
            PaymentMasterModel model = new PaymentMasterModel();
            PaymentBAL objBAL = new PaymentBAL();
            string otherInfo = formData.Split('@')[0];
            string billInfo = formData.Split('@')[1];
            string[] billIdArray = billInfo.Split('*');
            try
            {
                string billNo = otherInfo.Split('$')[0];
                DateTime billDate = Convert.ToDateTime(otherInfo.Split('$')[1]);
                string chq_No = otherInfo.Split('$')[2];
                decimal deductionAmt = Convert.ToDecimal(otherInfo.Split('$')[3]);

                for (int i = 0; i < billIdArray.Length; i++)
                {
                    long snaBillId = Convert.ToInt64(billIdArray[i].Split('_')[0]);
                    int snaBillTxnNo = Convert.ToInt32(billIdArray[i].Split('_')[1]);
                    if (dbContext.ACC_BILL_SNA_HOLDING_MAPPING.Any(s => s.SNA_BILL_ID == snaBillId && s.SNA_BILL_TXN_NO == snaBillTxnNo))
                    {
                        string voucherName = dbContext.ACC_BILL_MASTER.Where(s => s.BILL_ID == snaBillId).Select(m => m.BILL_NO).FirstOrDefault().ToString();
                        return Json(new { Success = false, message = "The vouchers are already generated for the selected record " + voucherName });
                    }
                }





                var month = billDate.Month;
                var year = billDate.Year;
                // return Json(new { Success = false, message = "Testing " });
                bool result;
                String errMessage = String.Empty;
                if (month != 0 && month < 13 && year != 0)
                {
                    string monthlyClosingStatus = string.Empty;

                    monthlyClosingStatus = objCommon.MonthlyClosingValidation(Convert.ToInt16(month), Convert.ToInt16(year), PMGSYSession.Current.FundType, PMGSYSession.Current.LevelId, PMGSYSession.Current.AdminNdCode, ref errMessage);

                    if (monthlyClosingStatus.Equals("-111"))
                    {
                        ModelState.AddModelError("BILL_MONTH", errMessage);
                    }
                    if (monthlyClosingStatus.Equals("-222"))
                    {
                        return Json(new { Success = false, message = "Month is already closed" });
                    }
                }
                if (ModelState.IsValid)
                {

                    result = objBAL.SubmitTransferDeductionAmountBAL(billIdArray, billNo, billDate, chq_No, deductionAmt);
                }
                else
                {
                    result = false;
                    return Json(new { Success = false, message = errMessage });
                }

                if (result)
                    return Json(new { Success = true, message = "Voucher Generated successfully." });
                else
                    return Json(new { Success = false, message = "Voucher is not Generated" });
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



        [RequiredAuthentication]
        public JsonResult GetTransferDeductionAmtGeneratedVoucherList(int? page, int? rows, string sidx, string sord)
        {
            try
            {

                objPaymentBAL = new PaymentBAL();
                PaymentFilterModel objFilter = new PaymentFilterModel();
                objFilter.FilterMode = Request.Params["mode"].Trim();

                string TransactionType = string.Empty;

                if (Request.Params["payType"] != "E" && Request.Params["payType"] != "R")
                {
                    throw new Exception("Invalid Parameters");
                }
                else
                {
                    TransactionType = Request.Params["payType"];
                }

                long totalRecords;
                objFilter.Month = Convert.ToInt16(Request.Params["months"]);
                objFilter.Year = Convert.ToInt16(Request.Params["year"]);
                objFilter.deductionType = (Request.Params["DeductionType"]).ToString();
                if (objFilter.FilterMode.Equals("Search"))
                {
                    objFilter.FromDate = Request.Params["fromDate"].ToString() == String.Empty ? null : Request.Params["fromDate"].ToString();
                    objFilter.ToDate = Request.Params["toDate"].ToString() == String.Empty ? null : Request.Params["toDate"].ToString();
                    objFilter.TransId = Request.Params["transType"] == String.Empty ? Convert.ToInt16(0) : Convert.ToInt16(Request.Params["transType"].Trim().Split('$')[0]);

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
                    rows = objPaymentBAL.GetTransferDeductionAmtGeneratedVoucherListBAL(objFilter, TransactionType, out totalRecords),
                    total = totalRecords <= objFilter.rows ? 1 : totalRecords / objFilter.rows + 1,
                    page = page,
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




        [Audit]
        public ActionResult PaymentWiseHoldingSecurityDepositeAccInfo(String parameter, String hash, String key)
        {
            ReatDAL reatDAL = new ReatDAL();
            PFMSDAL1 pfmsDAL = new PFMSDAL1();
            PMGSYEntities dbContext = new PMGSYEntities();

            TransactionParams objparams = new TransactionParams();
            ViewBag.ModuleType = "R";
            ViewBag.ChkHoldSecDepRadio = "H";
            try
            {
                DigSignBAL objBAL = new DigSignBAL();
                RegisterDSCModel modelDSC = new RegisterDSCModel();
                modelDSC = objBAL.GetDetailsToRegisterDSC();
                //Below Condition modified on 26-11-2021
                // if ((modelDSC.NodalOfficerCode == 0) || (modelDSC.IsValidXmlDscRegisteredREAT == false) || (modelDSC.DscDeleteEnabled == true))
                if ((modelDSC.NodalOfficerCode == 0))
                {
                    modelDSC.IsAlreadyRegistered = 0;
                }
                else
                {
                    if (objBAL.IsAlreadyRegistered(modelDSC.NodalOfficerCode))
                    {
                        modelDSC.IsAlreadyRegistered = 1;
                        //modelDSC.IsAlreadyRegistered = modelDSC.IsValidXmlDscRegisteredREAT == false ? 0 : 1;
                        if (modelDSC.DSCDeregCheck == false)
                        {
                            modelDSC.IsAlreadyRegistered = 0;
                        }
                    }
                    else
                    {
                        modelDSC.IsAlreadyRegistered = 2;
                    }

                }

                if (modelDSC.IsAlreadyRegistered == 1)
                {


                    ListModel model = new ListModel();
                    // model.billid = 0;

                    objparams.BILL_TYPE = "P";
                    objparams.FUND_TYPE = PMGSYSession.Current.FundType;
                    objparams.LVL_ID = PMGSYSession.Current.LevelId;
                    objparams.ADMIN_ND_CODE = PMGSYSession.Current.AdminNdCode;
                    objparams.DISTRICT_CODE = PMGSYSession.Current.DistrictCode;

                    List<SelectListItem> monthList = common.PopulateMonths(DateTime.Now.Month);
                    ViewData["months"] = monthList;

                    List<SelectListItem> yearList = common.PopulateYears(DateTime.Now.Year);
                    ViewData["year"] = yearList;

                    List<SelectListItem> transactionType = common.PopulateTransactions(objparams);
                    ViewData["TXN_ID"] = transactionType;



                    ViewBag.SessionSalt = Session["SessionSalt"].ToString();
                    ViewBag.IsAlreadyRegistered = "1";

                    //ViewBag.IsPaymentEnabled = pfmsDAL.ValidateSamplePayment();
                    ViewBag.IsPaymentEnabled = false;

                    ViewBag.IsReatPaymentEnabled = reatDAL.ValidateSamplePayment();

                    //return View("PaymentWiseHoldingSecurityDepositeAccInfo");
                    return View("EpayList");
                }
                else
                {
                    ViewBag.IsAlreadyRegistered = modelDSC.IsAlreadyRegistered.ToString();
                    //return View("PaymentWiseHoldingSecurityDepositeAccInfo");
                    return View("EpayList");
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



        [Audit]
        public JsonResult GetSecondLevelSuccessEPaymentListJson(int? page, int? rows, string sidx, string sord)
        {
            try
            {
                PaymentFilterModel objFilter = new PaymentFilterModel();
                objFilter.FilterMode = Request.Params["mode"].Trim();

                string TransactionType = string.Empty;

                if (Request.Params["payType"] != "E" && Request.Params["payType"] != "R" && Request.Params["payType"] != "D" && Request.Params["payType"] != "H")
                {
                    throw new Exception("Invalid Parameters");
                }
                else
                {
                    TransactionType = Request.Params["payType"];
                }

                long totalRecords;
                objFilter.Month = Convert.ToInt16(Request.Params["months"]);
                objFilter.Year = Convert.ToInt16(Request.Params["year"]);

                if (objFilter.FilterMode.Equals("Search"))
                {
                    objFilter.FromDate = Request.Params["fromDate"].ToString() == String.Empty ? null : Request.Params["fromDate"].ToString();
                    objFilter.ToDate = Request.Params["toDate"].ToString() == String.Empty ? null : Request.Params["toDate"].ToString();
                    objFilter.TransId = Request.Params["transType"] == String.Empty ? Convert.ToInt16(0) : Convert.ToInt16(Request.Params["transType"].Trim().Split('$')[0]);

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
                    rows = objPaymentBAL.ListSecondLevelSuccessEPaymentDetails(objFilter, TransactionType, out totalRecords, Request.Params["moduleType"].ToString()),
                    total = totalRecords <= objFilter.rows ? 1 : totalRecords / objFilter.rows + 1,
                    page = page,
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

        #endregion


        #region Security Deposit opening Balance Entry
        //Added By Hrishikesh--
        [HttpGet]
        [Audit]
        public ActionResult GetSecurityDepositACCDetails()
        {
            #region Region to read HoldingSecurityUAT XML File
            try
            {
                int AllowUATflag = 0;
                AllowUATflag = HoldingSecDepUATAllowStatus();
                if (AllowUATflag == 0)
                {
                    return View("HoldingSecurityModuleUATView");
                }
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "PaymentController.GetSecurityDepositACCDetails()");

            }

            #endregion

            PaymentDAL paymentDAL = new PaymentDAL();

            TransactionParams objparams = new TransactionParams();
            CommonFunctions objCommon = new CommonFunctions();
            //PaymentMasterModel model = new PaymentMasterModel();
            SecurityDepositAccOpeningBalanceEntryModel model = new SecurityDepositAccOpeningBalanceEntryModel();

            objparams.BILL_TYPE = "P";
            objparams.FUND_TYPE = PMGSYSession.Current.FundType;
            objparams.LVL_ID = PMGSYSession.Current.LevelId;
            objparams.ADMIN_ND_CODE = PMGSYSession.Current.AdminNdCode;
            objparams.DISTRICT_CODE = 0;
            objparams.STATE_CODE = PMGSYSession.Current.StateCode;

            try
            {
                model.BILL_DATE = DateTime.Now.ToString("dd/MM/yyyy");
                /*
                                List<SelectListItem> ContractorList = common.PopulateContractorSupplier(objparams);
                                model.mast_CON_ID_C1 = ContractorList;
                */


                List<SelectListItem> ContractorList = common.PopulateContractorSupplierNew(objparams);
                model.mast_CON_ID_C1 = ContractorList;

                List<SelectListItem> CONC_Account_ID = new List<SelectListItem>();
                CONC_Account_ID.Add(new SelectListItem { Text = "--Select Account--", Value = "0", Selected = true });
                model.CONC_Account_ID1 = CONC_Account_ID;

                /*                List<SelectListItem> SupplierList = common.PopulateContractorSupplier(objparams);
                                model.mast_CON_ID_S1 = SupplierList;*/

                model.CHQ_EPAY = "E";
                model.BILL_MONTH = DateTime.Now.Month;
                model.BILL_YEAR = DateTime.Now.Year; ;
                return View(model);
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


        }//end GetSecurityDepositACCDetails()

        //added by hrishikesh 
        [HttpPost]
        [Audit]
        [ValidateAntiForgeryToken]
        public ActionResult AddSecurityDepositAccOpeningBalance(SecurityDepositAccOpeningBalanceEntryModel securitydepositaccopeningbalanceentry)
        {
            /*
                        if (!ModelState.IsValid)
                        {
                            //ViewBag.ErrorMsg = "Validations Are Remaining";
                            return Json(new { message = "Validation Error" }, JsonRequestBehavior.AllowGet);
                        }
                        else
                        {*/

            try
            {

                PaymentDAL payDAL = new PaymentDAL();
                if (PMGSYSession.Current.FundType == "P")
                {
                    bool isPFMSFinalized = false;

                    // Added by Srishti on 18-05-2023
                    PMGSYEntities dbContext = new PMGSYEntities();
                    int adminNdCode = PMGSYSession.Current.AdminNdCode;
                    bool isAccountPresent = dbContext.ACC_BANK_DETAILS.Where(x => x.ADMIN_ND_CODE == adminNdCode && x.FUND_TYPE == "P" && x.ACCOUNT_TYPE == "D").Any();
                    if (!isAccountPresent)
                        return Json(new { Success = false, Result = -1, message = "Please add Security Deposit Account bank details." });


                    //Below condition is Added on 02-12-2022
                    if (securitydepositaccopeningbalanceentry.MAST_CON_ID_C.HasValue && securitydepositaccopeningbalanceentry.MAST_CON_ID_C.Value > 0)
                    {
                        int? conAccId = securitydepositaccopeningbalanceentry.CONC_Account_ID;
                        isPFMSFinalized = payDAL.IsPFMSFinalized(securitydepositaccopeningbalanceentry.MAST_CON_ID_C.Value, (int)conAccId);

                    }

                    //if (isPFMSFinalized == false && Convert.ToInt32(securitydepositaccopeningbalanceentry.TXN_ID.Split('$')[0]) != 3185) //Added on 21-04-2023 to bypass va;lidation for txn_id=3185
                    if (isPFMSFinalized == false) //Added on 21-04-2023 to bypass va;lidation for txn_id=3185
                    {
                        return Json(new { Success = false, Bill_ID = -5 });
                    }
                }
                if (securitydepositaccopeningbalanceentry.MAST_CON_ID_C.HasValue && securitydepositaccopeningbalanceentry.MAST_CON_ID_C.Value > 0)
                {
                    bool status = objPaymentBAL.ValidateContractorStatus(securitydepositaccopeningbalanceentry.MAST_CON_ID_C.Value, Convert.ToInt32(3199));
                    if (status == false)
                    {
                        return Json(new { Success = false, Bill_ID = -5 });
                    }
                }


                var output = objPaymentBAL.AddSecurityDepositAccOpeningBalanceBAL(securitydepositaccopeningbalanceentry);
                if (output == 1)
                {
                    return Json(new { Success = true });
                }
                else if (output == -1)
                {
                    return Json(new { Success = false, Result = -1, message = "Security Deposit Account Opening Balance Entry Already Exist" });
                }
                else
                {
                    return Json(new { success = false });
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
            //}
        }

        //added by hrishikesh
        [HttpPost]
        [Audit]
        //public JsonResult GetSecurityDepositAccOpeningBalanceJson(int page, int rows, string sidx, string sord)
        public JsonResult GetSecurityDepositAccOpeningBalanceJson(FormCollection formCollection)
        {
            try
            {
                objPaymentBAL = new PaymentBAL();
                SecurityDepositAccOpeningBalanceEntryModel securitydepositaccopeningbalanceentryobj = new SecurityDepositAccOpeningBalanceEntryModel();

                using (CommonFunctions commonFunction = new CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new GridParams(Convert.ToInt32(formCollection["page"]), Convert.ToInt32(formCollection["rows"]), formCollection["sidx"], formCollection["sord"], Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                    {
                        return null;
                    }
                }

                int month = Convert.ToInt32(Request.Params["month"]);
                int year = Convert.ToInt32(Request.Params["year"]);
                long totalRecords;

                var jsonData = new
                {
                    rows = objPaymentBAL.GetSecurityDepositAccOpeningBalanceJsonBAL(Convert.ToInt32(formCollection["page"]) - 1, Convert.ToInt32(formCollection["rows"]), formCollection["sidx"], formCollection["sord"], formCollection["filters"], month, year, out totalRecords),
                    total = totalRecords <= Convert.ToInt32(formCollection["rows"]) ? 1 : (totalRecords % Convert.ToInt32(formCollection["rows"]) == 0 ? totalRecords / Convert.ToInt32(formCollection["rows"]) : (totalRecords / Convert.ToInt32(formCollection["rows"]) + 1)),
                    page = Convert.ToInt32(formCollection["page"]),
                    records = totalRecords
                };
                return Json(jsonData);

            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "PaymentController.GetSecurityDepositAccOpeningBalanceJson()");
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                string errorMsg = string.Format("Errors: {0}", ex.Message);
                Response.TrySkipIisCustomErrors = true;
                Response.StatusCode = 500;
                Response.Write(errorMsg);
                return Json(string.Empty);

            }
            //return Json(new { });
        }


        #endregion
        //Added on 12-05-2023
        public int HoldingSecDepUATAllowStatus()
        {
            try
            {

                XDocument doc_xml = XDocument.Load(System.Web.HttpContext.Current.Server.MapPath("~/HoldingSecurityUAT.xml"));
                List<HoldingSecurityUATModel> UATStateModelList = new List<HoldingSecurityUATModel>();
                foreach (XElement element in doc_xml.Descendants("stateList").Descendants("stateUAT"))
                {
                    HoldingSecurityUATModel UATStateModel = new HoldingSecurityUATModel();
                    UATStateModel.StateCode = Convert.ToInt32(element.Descendants("statCode").FirstOrDefault().Value);
                    UATStateModel.AdminNdCode = Convert.ToInt32(element.Descendants("adminNDCode").FirstOrDefault().Value);

                    UATStateModelList.Add(UATStateModel);

                }
                //xml Code end
                int AllowUATflag = 0;

                foreach (HoldingSecurityUATModel item in UATStateModelList)
                {
                    if (item.AdminNdCode == PMGSYSession.Current.AdminNdCode && item.StateCode == PMGSYSession.Current.StateCode)
                    {
                        AllowUATflag = 1;
                        break;
                    }
                }
                return AllowUATflag;

            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "PaymentController.HoldSecDepUATAllowStatus()");
                throw;
            }
        }


        #region Holding and SDA Transaction

        [Audit]
        public ActionResult GetMonthWiseHoldingLayout(HoldingSDALayoutModel model)
        {

            try
            {
                CommonFunctions common = new CommonFunctions();
                TransactionParams objparams = new TransactionParams();
                int SRRDAcode = Convert.ToInt32(PMGSYSession.Current.ParentNDCode);

                List<SelectListItem> monthList = common.PopulateMonths(DateTime.Now.Month);
                model.MONTH_LIST = monthList;

                List<SelectListItem> yearList = common.PopulateYears(DateTime.Now.Year);
                model.YEAR_LIST = yearList;

                List<SelectListItem> lstDPIU = common.PopulateDPIUOfSRRDA(SRRDAcode);
                model.Dpiu_List = lstDPIU;

                List<SelectListItem> lstAccType = new List<SelectListItem>();
                lstAccType.Insert(0, new SelectListItem { Text = "Select Account Type", Value = "0" });
                lstAccType.Insert(1, new SelectListItem { Text = "Saving", Value = "1" });
                lstAccType.Insert(2, new SelectListItem { Text = "Holding", Value = "2" });
                //lstAccType.Insert(3, new SelectListItem { Text = "Security Deposit Account", Value = "3" });
                model.ACCOUNT_TYPE_LIST = lstAccType;

                return View(model);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "PaymentController.GetMonthWiseHoldingLayout()");
                return null;
            }
        }


        [HttpPost]
        [Audit]
        public ActionResult AutomateHoldingSecurity(string[] submitarray, string id)
        {
            bool status = true;
            long Bill_id = 0;
            int month, year = 0;
            string TxnModel = id;
            string errMessage = string.Empty;
            string message = string.Empty;
            PaymentDAL ObjDAL = new PaymentDAL();
            PMGSYEntities dbContext = new PMGSYEntities();
            CommonFunctions objCommon = new CommonFunctions();
            try
            {
                string[] arr = id.Split('$');
                month = Convert.ToInt32(arr[0]);
                year = Convert.ToInt32(arr[1]);

                string monthlyClosingStatus = string.Empty;

                monthlyClosingStatus = objCommon.MonthlyClosingValidation(Convert.ToInt16(month), Convert.ToInt16(year), PMGSYSession.Current.FundType, PMGSYSession.Current.LevelId, Convert.ToInt16(PMGSYSession.Current.ParentNDCode), ref errMessage);
                if (monthlyClosingStatus.Equals("-222"))
                {
                    status = false;
                    return Json(new { Success = status, message = "Month is already closed at SRRDA" });
                }

                monthlyClosingStatus = objCommon.MonthlyClosingValidation(Convert.ToInt16(month), Convert.ToInt16(year), PMGSYSession.Current.FundType, PMGSYSession.Current.LevelId, PMGSYSession.Current.AdminNdCode, ref errMessage);
                if (monthlyClosingStatus.Equals("-222"))
                {
                    return Json(new { Success = false, message = "Month is already closed at PIU", JsonRequestBehavior.AllowGet });
                }

                long[] billIds = null;
                billIds = new long[submitarray.Length];
                int i = 0;


                if (submitarray != null)
                {
                    foreach (var item in submitarray)
                    {
                        Bill_id = Convert.ToInt64(item);
                        billIds[i] = Bill_id;
                        i++;

                        string voucherName = string.Empty;
                        if (dbContext.ACC_HOLDING_SECURITY_AUTO_ENTRIES.Any(s => s.BILL_ID == Bill_id))
                        {
                            voucherName = dbContext.ACC_BILL_MASTER.Where(s => s.BILL_ID == Bill_id).Select(m => m.BILL_NO).FirstOrDefault().ToString();
                            status = false;
                            return Json(new { Success = status, message = "The Vouchers are already generated for the selected record " + voucherName });
                        }
                    }  // for loop close
                }
                string SuccessStatus = ObjDAL.AutoGenerateTxnDAL(billIds, TxnModel);
                if (SuccessStatus == "-111")
                {
                    status = true;
                    message = "Voucher Generated Successfully.";
                    return Json(new { Success = status, message = message });
                }
                else
                {
                    status = false;
                    message = "Failed to Generated Voucher at SRRDA Level.";
                    return Json(new { Success = status, message = message });
                }
            }
            catch (Exception ex)
            {
                status = false;
                ErrorLog.LogError(ex, "PaymentController.AutomateHoldingSecurity()");
                message = "Error Occured while Processing Request.";
                return Json(new { Success = status, message = message });
            }
            finally
            {
                dbContext.Dispose();
            }
        }


        [RequiredAuthentication]
        public JsonResult GetSDAandHoldingList(int? page, int? rows, string sidx, string sord)
        {
            int month = 0, year = 0;
            int AdminNDCode = 0;
            int tXnid = 0;
            long totalRecords = 0;
            CommonFunctions common = new CommonFunctions();
            PaymentDAL ObjDAL = new PaymentDAL();
            try
            {
                if (!common.ValidateGridParameters(new GridParams(page, rows, sidx, sord, Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                {
                    return null;
                }
                if (!string.IsNullOrEmpty(Request.Params["month"]))
                {
                    month = Convert.ToInt32(Request.Params["month"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["year"]))
                {
                    year = Convert.ToInt32(Request.Params["year"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["DPIUCode"]))
                {
                    AdminNDCode = Convert.ToInt32(Request.Params["DPIUCode"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["TxnID"]))
                {
                    tXnid = Convert.ToInt32(Request.Params["TxnID"]);
                }

                List<String> SelectedIdList = new List<String>();

                var jsonData = new
                {
                    rows = ObjDAL.GetSDAandHoldingListDal(month, year, AdminNDCode, tXnid, Convert.ToInt32(page) - 1, Convert.ToInt32(rows), sidx, sord, out totalRecords, out SelectedIdList),
                    total = totalRecords <= Convert.ToInt32(rows) ? 1 : totalRecords / Convert.ToInt32(rows) + 1,
                    page = Convert.ToInt32(page),
                    records = totalRecords,
                    userdata = new { ids = SelectedIdList.ToArray<string>(), billID = tXnid }
                };
                return Json(jsonData, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "PaymentController.GetSDAandHoldingList()");
                return null;
            }
        }

        #endregion

    }
}
