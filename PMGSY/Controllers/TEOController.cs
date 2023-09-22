using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PMGSY.Models.TransferEntryOrder;
using PMGSY.Extensions;
using PMGSY.Common;
using PMGSY.Models.Common;
using PMGSY.BAL.TransferEntryOrder;
using PMGSY.Models.Receipts;
using PMGSY.Models;
using PMGSY.DAL.TransferEntryOrder;
using System.Web.Script.Serialization;

namespace PMGSY.Controllers
{
    [RequiredAuthentication]
    [RequiredAuthorization]
    public class TEOController : Controller
    {
        private ITransferEntryOrderBAL teoBAL = null;
        private CommonFunctions commonFuncObj = null;

        private static bool _populateActiveInactiveDistricts = false;

        //added by koustubh nakate on 26/08/2013 for bill type TEO 
        private const string BILLTYPE = "J";

        Dictionary<string, string> lstTransactionIDs = new Dictionary<string, string>();

        public TEOController()
        {
            if (!string.IsNullOrEmpty(PMGSYSession.Current.FundType))
            {

                if (PMGSYSession.Current.FundType.Equals("P"))
                {

                    lstTransactionIDs = new Dictionary<string, string>() { 
                                                                           //{"TXN_ID0",""}, 
                                                                           
                                                                           {"TXN_ID1","163"}, 
                            
                                                                           {"TXN_ID2","164"}, 
                                                                           
                                                                           {"TXN_ID3","165"}, 
                                                                           {"TXN_ID4","1187"},
                                                                           {"TXN_ID5","1550"},//Added By Abhishek kamble 7Apr2015 -Balance Transfer between the road heads of same piu  
                                                                           {"TXN_ID6","1664"}  ,
                                                                           {"TXN_ID7","1665"} 
                                                                         };
                }
                else if (PMGSYSession.Current.FundType.Equals("M"))
                {

                    lstTransactionIDs = new Dictionary<string, string>() { 
                                                                          // {"TXN_ID0",""}, 
                                                                           {"TXN_ID1","1192"}, 
                            
                                                                           { "TXN_ID2","1193"}, 
                                                                           
                                                                           {"TXN_ID3","1194"}, 
                                                                           {"TXN_ID4","1195"} };
                }
            }
        }

        [Audit]
        [GenericAccountValidationFilter(InputParameter = new string[] { "BankDetails", "ChequeBookDetails", "OpeningBalanceDetails", "AuthSign" })]
        public ActionResult TEOList()
        {
            Int32 Month = DateTime.Now.Month;
            Int32 Year = DateTime.Now.Year;
            if (Request.Params["Month"] != null)
            {
                Month = Convert.ToInt32(Request.Params["Month"]);
                Year = Convert.ToInt32(Request.Params["Year"]);
            }
            //new change done by Vikram on 01-Jan-2014
            else if (PMGSYSession.Current.AccMonth != 0)
            {
                Month = Convert.ToInt32(PMGSYSession.Current.AccMonth);
                Year = Convert.ToInt32(PMGSYSession.Current.AccYear);
            }
            else
            {
                PMGSYSession.Current.AccMonth = Convert.ToInt16(DateTime.Now.Month);
                PMGSYSession.Current.AccYear = Convert.ToInt16(DateTime.Now.Year);
            }
            //end of change
            commonFuncObj = new CommonFunctions();
            ViewBag.ddlMonth = commonFuncObj.PopulateMonths(Month);
            ViewBag.ddlYear = commonFuncObj.PopulateYears(Year);
            TransactionParams objMaster = new TransactionParams();
            objMaster.BILL_TYPE = "J";
            objMaster.FUND_TYPE = PMGSYSession.Current.FundType;
            objMaster.LVL_ID = PMGSYSession.Current.LevelId;

            //changes by Koustubh Nakate on 26/08/2013 for removing some transactions 
            List<SelectListItem> lstTransacrion = commonFuncObj.PopulateTransactions(objMaster);

            //lstTransacrion.RemoveRange(4, 4);
            if (PMGSYSession.Current.FundType != "A")
            {
                lstTransacrion = (from TXN in lstTransacrion
                                  where !lstTransactionIDs.Any(ID => ID.Value == TXN.Value)
                                  select TXN).ToList();
            }


            ViewBag.ddlMasterTrans = lstTransacrion;//commonFuncObj.PopulateTransactions(objMaster);
            ViewBag.ImprestLevel = PMGSYSession.Current.LevelId;
            ViewBag.ImprestFundType = PMGSYSession.Current.FundType;
            return View("TEOList");
        }

        [HttpPost]
        [Audit]
        public ActionResult GetTEOList(FormCollection homeFormCollection)
        {
            //Adde By Abhishek kamble 30-Apr-2014 start
            using (CommonFunctions commonFunction = new CommonFunctions())
            {
                if (!commonFunction.ValidateGridParameters(new GridParams(Convert.ToInt32(homeFormCollection["page"]), Convert.ToInt32(homeFormCollection["rows"]), homeFormCollection["sidx"], homeFormCollection["sord"], Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                {
                    return null;
                }
            }
            //Adde By Abhishek kamble 30-Apr-2014 end
            teoBAL = new TransferEntryOrderBAL();
            ReceiptFilterModel objFilter = new ReceiptFilterModel();
            long totalRecords;
            objFilter.Month = Convert.ToInt16(Request.Params["month"]);
            objFilter.Year = Convert.ToInt16(Request.Params["year"]);
            objFilter.FromDate = Request.Params["fromDate"].ToString();
            objFilter.ToDate = Request.Params["toDate"].ToString();
            objFilter.TransId = String.IsNullOrEmpty(Request.Params["transType"]) ? (short)0 : Convert.ToInt16(Request.Params["transType"].Split('$')[0]);
            objFilter.page = Convert.ToInt32(homeFormCollection["page"]) - 1;
            objFilter.rows = Convert.ToInt32(homeFormCollection["rows"]);
            objFilter.sidx = homeFormCollection["sidx"].ToString();
            objFilter.sord = homeFormCollection["sord"].ToString();
            objFilter.FilterMode = homeFormCollection["mode"].ToString();
            objFilter.AdminNdCode = PMGSYSession.Current.AdminNdCode;
            objFilter.FundType = PMGSYSession.Current.FundType;
            objFilter.LevelId = PMGSYSession.Current.LevelId;
            objFilter.BillType = "J";

            var jsonData = new
            {
                rows = teoBAL.TEOList(objFilter, out totalRecords),
                total = totalRecords <= objFilter.rows ? 1 : totalRecords / objFilter.rows + 1,
                page = objFilter.page + 1,
                records = totalRecords
            };
            return Json(jsonData);

        }

        [Audit]
        public ActionResult TEOEntry(String parameter, String hash, String key)
        {
            ViewBag.ImprestEntry = false;

            //Avinash_Start
            if (!string.IsNullOrEmpty(Request.Params["EncryptedEAuthID"]))   //EAuthorization Link at Home Page --Alerts
            {
                string EncryptedEAuthID = Request.Params["EncryptedEAuthID"];
                Int32 Month = DateTime.Now.Month;
                Int32 Year = DateTime.Now.Year;
                if (Request.Params["Month"] != null)
                {
                    Month = Convert.ToInt32(Request.Params["Month"]);
                    Year = Convert.ToInt32(Request.Params["Year"]);
                }
                //new change done by Vikram on 01-Jan-2014
                else if (PMGSYSession.Current.AccMonth != 0)
                {
                    Month = Convert.ToInt32(PMGSYSession.Current.AccMonth);
                    Year = Convert.ToInt32(PMGSYSession.Current.AccYear);
                }
                else
                {
                    PMGSYSession.Current.AccMonth = Convert.ToInt16(DateTime.Now.Month);
                    PMGSYSession.Current.AccYear = Convert.ToInt16(DateTime.Now.Year);
                }

                ViewBag.Month = Month;
                ViewBag.Year = Year;
                commonFuncObj = new CommonFunctions();
                ViewBag.ddlMonth = commonFuncObj.PopulateMonths(Month);
                ViewBag.ddlYear = commonFuncObj.PopulateYears(Year);
                ViewBag.EncryptedEAuthID = EncryptedEAuthID;
                ViewBag.BILL_ID = 0;

                return View();
                //Avinash_End

            }
            else  //Other than EAuthorization
            {
                if (Request.Params["Month"] != null)
                {
                    ViewBag.Month = Convert.ToInt32(Request.Params["Month"]);
                    ViewBag.Year = Convert.ToInt32(Request.Params["Year"]);
                    //change by amol jadhav to identify if imprest entry
                    if (Request.Params["ImprestEntry"] != null)
                    {
                        ViewBag.ImprestEntry = Convert.ToBoolean(Request.Params["ImprestEntry"]);
                    }
                }
                if (parameter == null)
                {
                    ViewBag.BILL_ID = 0;
                }
                else
                {
                    teoBAL = new TransferEntryOrderBAL();
                    string[] strParameters = URLEncrypt.DecryptParameters(new string[] { parameter, hash, key });
                    Int64 billId = Convert.ToInt64(strParameters[0]);
                    ViewBag.BILL_ID = URLEncrypt.EncryptParameters(new string[] { billId.ToString().Trim() });
                    ViewBag.IsFinalize = teoBAL.IsTEOFinalized(billId);
                    ViewBag.IsMulTxn = teoBAL.IsMultipleTransactionRequired(billId);
                    ViewBag.GetMasterTxnID = teoBAL.GetMasterTXNId(billId);
                }
                return View();
            }


        }


        /// <summary>
        /// Action for list page of the imprest settlement
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="hash"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        [GenericAccountValidationFilter(InputParameter = new string[] { "BankDetails", "ChequeBookDetails", "OpeningBalanceDetails", "AuthSign" })]
        [HttpGet]
        [Audit]
        public ActionResult TEOImprest(String parameter, String hash, String key)
        {
            Int16 Month = 0;
            Int16 Year = 0;
            if (Request.Params["Month"] == null)
            {
                //new change done by Vikram on 01-Jan-2014
                if (PMGSYSession.Current.AccMonth != 0)
                {
                    Month = Convert.ToInt16(PMGSYSession.Current.AccMonth);
                    Year = Convert.ToInt16(PMGSYSession.Current.AccYear);
                }
                else
                //end of change
                {
                    Month = Convert.ToInt16(DateTime.Now.Month);
                    Year = Convert.ToInt16(DateTime.Now.Year);
                }
            }
            else
            {
                Month = Convert.ToInt16(Request.Params["Month"]);
                Year = Convert.ToInt16(Request.Params["Year"]);
            }
            commonFuncObj = new CommonFunctions();
            TeoMasterModel teoMasterModel = new TeoMasterModel();
            teoBAL = new TransferEntryOrderBAL();
            //new change done by Vikram on 1-10-2013
            // to show default bill date as the server date
            //teoMasterModel.BILL_DATE = commonFuncObj.GetDateTimeToString(DateTime.Now);
            //end of change
            if (parameter == null)
            {
                teoMasterModel.BILL_MONTH = Convert.ToInt16(Month);
                teoMasterModel.BILL_YEAR = Convert.ToInt16(Year);
                ViewBag.ddlMonth = commonFuncObj.PopulateMonths(Month);
                ViewBag.ddlYear = commonFuncObj.PopulateYears(Year);
            }
            else
            {
                //string[] strParameters = URLEncrypt.DecryptParameters(new string[] { parameter, hash, key });
                //Int64 billId = Convert.ToInt64(strParameters[0]);
                //ViewBag.BILL_ID = URLEncrypt.EncryptParameters(new string[] { billId.ToString().Trim() });
                //ACC_BILL_MASTER acc_bill_master = new ACC_BILL_MASTER();
                //teoMasterModel = teoBAL.GetTEOMaster(billId);
                //ViewBag.ddlMonth = commonFuncObj.PopulateMonths(Month);
                //ViewBag.ddlYear = commonFuncObj.PopulateYears(Year);                
            }
            //new change done by Vikram on 24 Dec 2013
            teoMasterModel.CURRENT_DATE = DateTime.Now.ToString("dd/MM/yyyy");
            //end of change
            return View(teoMasterModel);
        }



        [Audit]
        public ActionResult TEOMaster(String parameter, String hash, String key)
        {

            //Avinash_Start
            TransactionParams objMaster = new TransactionParams();
            objMaster.BILL_TYPE = "J";
            objMaster.FUND_TYPE = PMGSYSession.Current.FundType;
            objMaster.LVL_ID = PMGSYSession.Current.LevelId;
            objMaster.TXN_ID = 0;
            commonFuncObj = new CommonFunctions();
            //Avinash_End


            //Avinash_Start
            if (!string.IsNullOrEmpty(Request.Params["EncryptedEAuthID"]))
            {
                TeoMasterModel teoMasterModel = new TeoMasterModel();
                PMGSYEntities dbContext = new PMGSYEntities();
                ACC_NOTIFICATION_DETAILS AccDetails = new ACC_NOTIFICATION_DETAILS();
                Int64 DETAIL_ID = 0;
                string EncryptedEAuthID = Request.Params["EncryptedEAuthID"];
                ACC_EAUTH_MASTER objAccMaster = new ACC_EAUTH_MASTER();
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
                        DETAIL_ID = Convert.ToInt64(urlSplitParams[0]);
                    }
                    AccDetails = dbContext.ACC_NOTIFICATION_DETAILS.Where(x => x.DETAIL_ID == DETAIL_ID).FirstOrDefault();

                }

                if (AccDetails != null)
                {
                    objAccMaster = dbContext.ACC_EAUTH_MASTER.Where(x => x.EAUTH_ID == AccDetails.INITIATION_BILL_ID && (x.EAUTH_STATUS == "A" || x.EAUTH_STATUS == "P")).FirstOrDefault();
                    if (objAccMaster != null)
                    {
                        //Gross Amount Should be In Rs...Not In Lakh
                        teoMasterModel.GROSS_AMOUNT = Convert.ToDecimal(objAccMaster.TOTAL_AUTH_APPROVED) * 100000;
                    }
                }


                Int16 Month = Convert.ToInt16(Request.Params["Month"]);
                Int16 Year = Convert.ToInt16(Request.Params["Year"]);



                teoMasterModel.BILL_MONTH = Convert.ToInt16(Month);
                teoMasterModel.BILL_YEAR = Convert.ToInt16(Year);
                ViewBag.ddlMonth = commonFuncObj.PopulateMonths(Month);
                ViewBag.ddlYear = commonFuncObj.PopulateYears(Year);

                List<SelectListItem> lstTransacrion = commonFuncObj.PopulateTransactions(objMaster);

                if (PMGSYSession.Current.FundType != "A")
                {
                    lstTransacrion = (from TXN in lstTransacrion
                                      where !lstTransactionIDs.Any(ID => ID.Value == TXN.Value)
                                      select TXN).ToList();
                }






                ViewBag.ddlMasterTrans = lstTransacrion;
                List<SelectListItem> lstOneItem = new List<SelectListItem>();
                lstOneItem.Insert(0, (new SelectListItem { Text = "Select SubTransaction", Value = "0", Selected = true }));
                ViewBag.ddlSubTrans = lstOneItem;
                teoMasterModel.BILL_ID = 0;
                teoMasterModel.CURRENT_DATE = DateTime.Now.ToString("dd/MM/yyyy");
                teoMasterModel.EncryptedEAuthID = EncryptedEAuthID;
                teoMasterModel.Module = "e-Auth";
                return PartialView("TEOAddMaster", teoMasterModel);

                //Avinash_End


            }
            else
            {
                Int16 Month = Convert.ToInt16(Request.Params["Month"]);
                Int16 Year = Convert.ToInt16(Request.Params["Year"]);

                TeoMasterModel teoMasterModel = new TeoMasterModel();
                teoBAL = new TransferEntryOrderBAL();


                //new change done by Vikram on 1-10-2013
                // to show default bill date as the server date
                //teoMasterModel.BILL_DATE = commonFuncObj.GetDateTimeToString(DateTime.Now);
                //end of change
                if (parameter == null)
                {
                    teoMasterModel.BILL_MONTH = Convert.ToInt16(Month);
                    teoMasterModel.BILL_YEAR = Convert.ToInt16(Year);
                    ViewBag.ddlMonth = commonFuncObj.PopulateMonths(Month);
                    ViewBag.ddlYear = commonFuncObj.PopulateYears(Year);

                    //changes by Koustubh Nakate on 26/08/2013 for removing some transactions 
                    List<SelectListItem> lstTransacrion = commonFuncObj.PopulateTransactions(objMaster);

                    // lstTransacrion.RemoveRange(4, 4);
                    //new change
                    if (PMGSYSession.Current.FundType != "A")
                    {
                        lstTransacrion = (from TXN in lstTransacrion
                                          where !lstTransactionIDs.Any(ID => ID.Value == TXN.Value)
                                          select TXN).ToList();
                    }


                    ViewBag.ddlMasterTrans = lstTransacrion;//commonFuncObj.PopulateTransactions(objMaster);
                    List<SelectListItem> lstOneItem = new List<SelectListItem>();
                    lstOneItem.Insert(0, (new SelectListItem { Text = "Select SubTransaction", Value = "0", Selected = true }));
                    ViewBag.ddlSubTrans = lstOneItem;
                    teoMasterModel.BILL_ID = 0;
                }
                else
                {
                    string[] strParameters = URLEncrypt.DecryptParameters(new string[] { parameter, hash, key });
                    Int64 billId = Convert.ToInt64(strParameters[0]);
                    ViewBag.BILL_ID = URLEncrypt.EncryptParameters(new string[] { billId.ToString().Trim() });
                    ACC_BILL_MASTER acc_bill_master = new ACC_BILL_MASTER();
                    teoMasterModel = teoBAL.GetTEOMaster(billId);
                    if (teoMasterModel.TXN_ID == 172 || teoMasterModel.TXN_ID == 402 || teoMasterModel.TXN_ID == 480 || teoMasterModel.TXN_ID == 864)
                    {
                        objMaster.BILL_TYPE = "I";
                    }

                    //changes by Koustubh Nakate on 26/08/2013 for removing some transactions 
                    List<SelectListItem> lstTransacrion = commonFuncObj.PopulateTransactions(objMaster);

                    // lstTransacrion.RemoveRange(4, 4);

                    lstTransacrion = (from TXN in lstTransacrion
                                      where !lstTransactionIDs.Any(ID => ID.Value == TXN.Value)
                                      select TXN).ToList();


                    ViewBag.ddlMasterTrans = lstTransacrion;//commonFuncObj.PopulateTransactions(objMaster);
                    ViewBag.ddlMonth = commonFuncObj.PopulateMonths(Month);
                    ViewBag.ddlYear = commonFuncObj.PopulateYears(Year);
                    objMaster.TXN_ID = teoMasterModel.TXN_ID;
                    ViewBag.ddlSubTrans = commonFuncObj.PopulateTransactions(objMaster);
                }
                teoMasterModel.CURRENT_DATE = DateTime.Now.ToString("dd/MM/yyyy");
                // teoMasterModel.BILL_ID = billId;
                return PartialView("TEOAddMaster", teoMasterModel);
            }

        }


        /// <summary>
        /// action to display the credit debit views  
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="hash"></param>
        /// <param name="key"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        [Audit]
        public ActionResult TEODetails(String parameter, String hash, String key, String id)
        {
            teoBAL = new TransferEntryOrderBAL();
            TransferEntryOrderDAL teoDAL = new TransferEntryOrderDAL();
            commonFuncObj = new CommonFunctions();
            TransactionParams objMaster = new TransactionParams();
            TeoDetailsModel teoDetailsModel = new TeoDetailsModel();

            String[] strParameters = URLEncrypt.DecryptParameters(new string[] { parameter, hash, key });

            Int64 billId = 0;
            String TransId = null;
            Int16 TransNo = 0;

            if (strParameters[0].Contains('$'))
            {
                billId = Convert.ToInt64(strParameters[0].Split('$')[0]);
                TransNo = Convert.ToInt16(strParameters[0].Split('$')[1]);
                teoDetailsModel = teoBAL.GetTEODetailByTransNo(billId, TransNo);
                TransId = URLEncrypt.EncryptParameters(new string[] { billId.ToString().Trim() + "$" + TransNo.ToString().Trim() });

                objMaster.ADMIN_ND_CODE = teoDetailsModel.ADMIN_ND_CODE == null ? PMGSYSession.Current.AdminNdCode : Convert.ToInt32(teoDetailsModel.ADMIN_ND_CODE);
                objMaster.MAST_CONT_ID = teoDetailsModel.MAST_CON_ID == null ? 0 : Convert.ToInt32(teoDetailsModel.MAST_CON_ID);
                objMaster.DISTRICT_CODE = teoDetailsModel.MAST_DISTRICT_CODE == null ? PMGSYSession.Current.DistrictCode : Convert.ToInt32(teoDetailsModel.MAST_DISTRICT_CODE);
                objMaster.AGREEMENT_CODE = teoDetailsModel.IMS_AGREEMENT_CODE == null ? 0 : Convert.ToInt32(teoDetailsModel.IMS_AGREEMENT_CODE);
                objMaster.HEAD_ID = teoDetailsModel.HEAD_ID;
                //If condition 'old - objMaster.AGREEMENT_CODE == 0' modified by Abhishek kamble 17Nov2014 
                if (teoDetailsModel.IMS_PR_ROAD_CODE != null && objMaster.AGREEMENT_CODE != 0)
                {
                    objMaster.SANC_YEAR = commonFuncObj.getSancYearFromRoad(Convert.ToInt32(teoDetailsModel.IMS_PR_ROAD_CODE));
                    objMaster.PACKAGE_ID = commonFuncObj.getPackageFromRoad(Convert.ToInt32(teoDetailsModel.IMS_PR_ROAD_CODE));
                }
            }
            else
            {
                billId = Convert.ToInt64(strParameters[0]);
                objMaster.ADMIN_ND_CODE = PMGSYSession.Current.AdminNdCode;
                objMaster.MAST_CONT_ID = 0;
                objMaster.DISTRICT_CODE = PMGSYSession.Current.DistrictCode;

            }
            ViewBag.TransId = TransId;
            Int16 parentId = teoBAL.GetMasterTransId(billId);
            ViewBag.BILL_ID = URLEncrypt.EncryptParameters(new string[] { billId.ToString().Trim() });
            ViewBag.parentTxnId = parentId;//Added on 13-10-2022

            if (id.Trim().ToLower().Equals("c"))
            {
                ViewBag.TEODetailTitle = "Credit Details";
                ViewBag.SaveTEODetails = "btnSaveTEOCreditDetails";
                ViewBag.EditTEODetails = "btnEditTEOCreditDetails";
                ViewBag.CancelTEODetails = "btnCancelTEOCreditDetails";
                ViewBag.TransIdC = TransId;
            }
            else
            {
                ViewBag.TEODetailTitle = "Debit Details";
                ViewBag.SaveTEODetails = "btnSaveTEODebitDetails";
                ViewBag.EditTEODetails = "btnEditTEODebitDetails";
                ViewBag.CancelTEODetails = "btnCancelTEODebitDetails";
                ViewBag.TransIdD = TransId;
            }

            //change is for multiple transaction
            #region New Requirement 06/07/2013

            ACC_BILL_DETAILS acc_bill_details_trans = new ACC_BILL_DETAILS();
            if (id.Trim() == "C")
            {
                //2)acc_bill_details_trans = teoBAL.GetBillDetails(billId, "D"); // 1 )dbContext.ACC_BILL_DETAILS.Where(m => m.BILL_ID == teoDetailsModel.BILL_ID && m.CREDIT_DEBIT == "D").FirstOrDefault();
                acc_bill_details_trans = teoBAL.GetBillDetails(billId, "D"); //change by amol
            }
            else
            {
                //2) acc_bill_details_trans = teoBAL.GetBillDetails(billId, "C"); //1)dbContext.ACC_BILL_DETAILS.Where(m => m.BILL_ID == teoDetailsModel.BILL_ID && m.CREDIT_DEBIT == "C").FirstOrDefault();
                acc_bill_details_trans = teoBAL.GetBillDetails(billId, "C");//change by amol
            }



            ACC_TEO_SCREEN_TXN_VALIDATIONS validationParams = new ACC_TEO_SCREEN_TXN_VALIDATIONS();

            validationParams = commonFuncObj.GetTEOValidationParams(0, parentId);

            #endregion

            ACC_TEO_SCREEN_DESIGN_PARAM_DETAILS designParams = new ACC_TEO_SCREEN_DESIGN_PARAM_DETAILS();
            designParams = commonFuncObj.getTEODesignParamDetails(0, parentId);

            if (designParams == null)
            {
                designParams = teoDAL.setDefualtDesignTransParam();
            }

            objMaster.BILL_TYPE = "J";
            objMaster.FUND_TYPE = PMGSYSession.Current.FundType;
            objMaster.LVL_ID = PMGSYSession.Current.LevelId;
            objMaster.TXN_ID = parentId;
            objMaster.BILL_ID = billId;
            objMaster.STATE_CODE = PMGSYSession.Current.StateCode;

            objMaster.CREDIT_DEBIT = id.Trim();

            if (designParams.CON_REQ == "Y")
            {
                objMaster.MAST_CON_SUP_FLAG = "C";
            }
            else if (designParams.SUPP_REQ == "Y")
            {
                objMaster.MAST_CON_SUP_FLAG = "S";
            }

            if (designParams.DISTRICT_REQ == "Y")
            {
                if (validationParams.IS_DISTRICT_REPEAT == "Y" && acc_bill_details_trans != null)
                {
                    ViewBag.ddlDistrict = commonFuncObj.PopulateDistrictforTOB(objMaster.STATE_CODE, false, Convert.ToInt32(acc_bill_details_trans.MAST_DISTRICT_CODE));
                }
                else
                {
                    ViewBag.ddlDistrict = commonFuncObj.PopulateDistrictforTOB(objMaster.STATE_CODE);
                }
            }
            else
            {
                ViewBag.ddlDistrict = new SelectList(new List<SelectListItem>().AsEnumerable<SelectListItem>(), "Value", "Text");
            }
            if (designParams.DPIU_REQ == "Y")
            {
                if (validationParams.IS_DPIU_REPEAT == "Y" && acc_bill_details_trans != null)
                {
                    Int32 tmpDistrict = objMaster.DISTRICT_CODE;
                    Int32 tmpAdminCode = objMaster.ADMIN_ND_CODE;
                    if (validationParams.IS_DPIU_REPEAT == "Y" && acc_bill_details_trans != null)
                    {
                        objMaster.DISTRICT_CODE = PMGSYSession.Current.LevelId == 5 ? PMGSYSession.Current.DistrictCode : Convert.ToInt32(acc_bill_details_trans.MAST_DISTRICT_CODE);
                        objMaster.ADMIN_ND_CODE = Convert.ToInt32(acc_bill_details_trans.ADMIN_ND_CODE);
                        ViewBag.ddlDPIU = commonFuncObj.PopulateDPIU(objMaster);
                    }
                    else
                    {
                        objMaster.DISTRICT_CODE = PMGSYSession.Current.LevelId == 5 ? PMGSYSession.Current.DistrictCode : Convert.ToInt32(acc_bill_details_trans.MAST_DISTRICT_CODE);
                        ViewBag.ddlDPIU = commonFuncObj.PopulateDPIU(objMaster);

                    }
                    objMaster.ADMIN_ND_CODE = tmpAdminCode;
                    objMaster.DISTRICT_CODE = tmpDistrict;
                }
                else if (TransNo == 0)
                {
                    List<SelectListItem> lstOneItem = new List<SelectListItem>();
                    lstOneItem.Insert(0, (new SelectListItem { Text = "Select Department", Value = "0", Selected = true }));
                    ViewBag.ddlDPIU = lstOneItem;
                }
                else
                {
                    ViewBag.ddlDPIU = commonFuncObj.PopulateDPIU(objMaster);
                }

                //code by amol jadhav 
                //if district is same but  dpiu is not repeted 
                if (validationParams.IS_DISTRICT_REPEAT == "Y" && validationParams.IS_DPIU_REPEAT == "N" && acc_bill_details_trans != null)
                {
                    objMaster.DISTRICT_CODE = PMGSYSession.Current.LevelId == 5 ? PMGSYSession.Current.DistrictCode : Convert.ToInt32(acc_bill_details_trans.MAST_DISTRICT_CODE);
                    objMaster.ADMIN_ND_CODE = Convert.ToInt32(acc_bill_details_trans.ADMIN_ND_CODE);
                    ViewBag.ddlDPIU = commonFuncObj.PopulateDPIU(objMaster);
                }
            }
            else
            {
                ViewBag.ddlDPIU = new SelectList(new List<SelectListItem>().AsEnumerable<SelectListItem>(), "Value", "Text");
            }

            if (designParams.CON_REQ == "Y" || designParams.SUPP_REQ == "Y")
            {
                if (acc_bill_details_trans != null)
                {
                    objMaster.MAST_CONT_ID = Convert.ToInt32(acc_bill_details_trans.MAST_CON_ID);
                    ViewBag.ddlContractor = commonFuncObj.PopulateContractorSupplier(objMaster);
                    // objMaster.MAST_CONT_ID = 0;
                }
                else
                {
                    ViewBag.ddlContractor = commonFuncObj.PopulateContractorSupplier(objMaster);
                }
            }
            else
            {
                ViewBag.ddlContractor = new SelectList(new List<SelectListItem>().AsEnumerable<SelectListItem>(), "Value", "Text");
            }

            if (designParams.AFREEMENT_REQ == "Y")
            {
                if (acc_bill_details_trans != null)
                {
                    Int32 tmpAggrementCode = objMaster.AGREEMENT_CODE;
                    Int32 tmpDistrict = objMaster.DISTRICT_CODE;
                    Int32 tmpContID = objMaster.MAST_CONT_ID;
                    objMaster.DISTRICT_CODE = PMGSYSession.Current.LevelId == 5 ? PMGSYSession.Current.DistrictCode : Convert.ToInt32(acc_bill_details_trans.MAST_DISTRICT_CODE);
                    objMaster.MAST_CONT_ID = Convert.ToInt32(acc_bill_details_trans.MAST_CON_ID);
                    objMaster.AGREEMENT_CODE = Convert.ToInt32(acc_bill_details_trans.IMS_AGREEMENT_CODE);
                    ViewBag.ddlAgreement = commonFuncObj.PopulateAgreement(objMaster);
                    objMaster.AGREEMENT_CODE = tmpAggrementCode;
                    objMaster.MAST_CONT_ID = tmpContID;
                    objMaster.DISTRICT_CODE = tmpDistrict;
                }
                else if (TransNo == 0)
                {
                    List<SelectListItem> lstOneItem = new List<SelectListItem>();
                    lstOneItem.Insert(0, (new SelectListItem { Text = "Select Agreement", Value = "0", Selected = true }));
                    ViewBag.ddlAgreement = lstOneItem;
                }
                else
                {
                    ViewBag.ddlAgreement = commonFuncObj.PopulateAgreement(objMaster);
                }
            }
            else
            {
                ViewBag.ddlAgreement = new SelectList(new List<SelectListItem>().AsEnumerable<SelectListItem>(), "Value", "Text");
            }

            if (designParams.SAN_REQ == "Y")
            {
                ViewBag.ddlSancYear = commonFuncObj.PopulateSancYear(objMaster);
            }
            else
            {
                ViewBag.ddlSancYear = new SelectList(new List<SelectListItem>().AsEnumerable<SelectListItem>(), "Value", "Text");
            }


            //Added by Abhishek kamble to get Agreement No to populate road for MF start

            if (PMGSYSession.Current.FundType == "M")
            {
                ACC_BILL_DETAILS acc_bill_detailsC = new ACC_BILL_DETAILS();
                ACC_BILL_DETAILS acc_bill_detailsD = new ACC_BILL_DETAILS();
                acc_bill_detailsC = teoBAL.GetBillDetails(billId, "C");
                acc_bill_detailsD = teoBAL.GetBillDetails(billId, "D");

                int? _ImsAgreementCode;
                //  int _MastConID;

                if (acc_bill_detailsC != null)
                {
                    _ImsAgreementCode = teoDetailsModel.IMS_AGREEMENT_CODE == null ? acc_bill_detailsC.IMS_AGREEMENT_CODE : teoDetailsModel.IMS_AGREEMENT_CODE;
                    objMaster.AGREEMENT_NUMBER = teoBAL.GetAgreementNumberForMF(_ImsAgreementCode);
                }
                else if (acc_bill_detailsD != null)
                {
                    _ImsAgreementCode = teoDetailsModel.IMS_AGREEMENT_CODE == null ? acc_bill_detailsD.IMS_AGREEMENT_CODE : teoDetailsModel.IMS_AGREEMENT_CODE;
                    objMaster.AGREEMENT_NUMBER = teoBAL.GetAgreementNumberForMF(_ImsAgreementCode);
                }

                //Get Contractor ID and Sanc Year
                if (objMaster.MAST_CONT_ID == 0 && (acc_bill_detailsC != null || acc_bill_detailsD != null))
                {
                    //short _txnId = (id.Trim() == "C") ? (short)2 : (short)1;
                    //TeoDetailsModel _teoDetailsViewModel = teoBAL.GetTEODetailByTransNo(billId, _txnId);
                    if (acc_bill_detailsC != null)
                    {
                        //objMaster.MAST_CONT_ID = (teoDetailsModel.MAST_CON_ID == null || teoDetailsModel.MAST_CON_ID == 0) ? (acc_bill_detailsC.MAST_CON_ID.Value) : (teoDetailsModel.MAST_CON_ID.Value);
                        objMaster.MAST_CONT_ID = (teoDetailsModel.MAST_CON_ID == null || teoDetailsModel.MAST_CON_ID == 0) ? 0 : (teoDetailsModel.MAST_CON_ID.Value);


                        int? _IMSPrRoadCodeC = (teoDetailsModel.IMS_PR_ROAD_CODE == null || teoDetailsModel.IMS_PR_ROAD_CODE == 0) ? (acc_bill_detailsC.IMS_PR_ROAD_CODE) : (teoDetailsModel.IMS_PR_ROAD_CODE);

                        if (objMaster.SANC_YEAR == 0)
                        {
                            if (_IMSPrRoadCodeC != null)
                                objMaster.SANC_YEAR = commonFuncObj.getSancYearFromRoad(Convert.ToInt32(_IMSPrRoadCodeC));
                        }
                    }
                    else
                    {

                        try
                        {
                            objMaster.MAST_CONT_ID = (teoDetailsModel.MAST_CON_ID == null || teoDetailsModel.MAST_CON_ID == 0) ? (acc_bill_detailsD.MAST_CON_ID.Value) : (teoDetailsModel.MAST_CON_ID.Value);
                        }
                        catch
                        {
                            objMaster.MAST_CONT_ID = 0;
                        }


                        int? _IMSPrRoadCodeD = (teoDetailsModel.IMS_PR_ROAD_CODE == null || teoDetailsModel.IMS_PR_ROAD_CODE == 0) ? (acc_bill_detailsD.IMS_PR_ROAD_CODE) : (teoDetailsModel.IMS_PR_ROAD_CODE);

                        if (objMaster.SANC_YEAR == 0)
                        {
                            if (_IMSPrRoadCodeD != null)
                                objMaster.SANC_YEAR = commonFuncObj.getSancYearFromRoad(Convert.ToInt32(_IMSPrRoadCodeD));
                        }
                    }
                }
            }

            //Added by Abhishek kamble to get Agreement No to populate road for MF end

            //designParams.ROAD_REQ condition necessary bcoz it will populate unnecessarily 
            if (designParams.ROAD_REQ == "Y" && validationParams.IS_ROAD_REPEAT == "Y" && acc_bill_details_trans != null)
            {
                Int32 tmpAggCode = objMaster.AGREEMENT_CODE;
                objMaster.AGREEMENT_CODE = Convert.ToInt32(acc_bill_details_trans.IMS_AGREEMENT_CODE);
                objMaster.ROAD_CODE = Convert.ToInt32(acc_bill_details_trans.IMS_PR_ROAD_CODE);
                ViewBag.ddlRoad = commonFuncObj.PopulateRoad(objMaster);
                objMaster.AGREEMENT_CODE = tmpAggCode;
            }
            else if (designParams.ROAD_REQ == "Y" && validationParams.IS_AGREEMENT_REPEAT == "Y" && acc_bill_details_trans != null && designParams.AFREEMENT_REQ == "Y")
            {
                Int32 tmpAggCode = objMaster.AGREEMENT_CODE;
                objMaster.AGREEMENT_CODE = Convert.ToInt32(acc_bill_details_trans.IMS_AGREEMENT_CODE);
                objMaster.ROAD_CODE = 0;
                ViewBag.ddlRoad = commonFuncObj.PopulateRoad(objMaster);
                objMaster.AGREEMENT_CODE = tmpAggCode;
            }
            else if (designParams.ROAD_REQ == "Y" && acc_bill_details_trans != null)
            {
                Int32 tmpAggCode = objMaster.AGREEMENT_CODE;
                objMaster.AGREEMENT_CODE = Convert.ToInt32(acc_bill_details_trans.IMS_AGREEMENT_CODE);
                objMaster.ROAD_CODE = 0;
                ViewBag.ddlRoad = commonFuncObj.PopulateRoad(objMaster);
                objMaster.AGREEMENT_CODE = tmpAggCode;
            }
            else if (designParams.ROAD_REQ == "Y")
            {
                ViewBag.ddlRoad = commonFuncObj.PopulateRoad(objMaster);
            }
            else
            {
                ViewBag.ddlRoad = new SelectList(new List<SelectListItem>().AsEnumerable<SelectListItem>(), "Value", "Text");
            }

            if (designParams.PKG_REQ == "Y")
            {
                if (TransNo == 0)
                {
                    List<SelectListItem> lstOneItem = new List<SelectListItem>();
                    lstOneItem.Insert(0, (new SelectListItem { Text = "Select Package", Value = "0", Selected = true }));
                    ViewBag.ddlPackage = lstOneItem;
                }
                else
                {
                    ViewBag.ddlPackage = commonFuncObj.PopulatePackage(objMaster);
                }
            }
            else
            {
                ViewBag.ddlPackage = new SelectList(new List<SelectListItem>().AsEnumerable<SelectListItem>(), "Value", "Text");
            }
            if (validationParams.IS_HEAD_REPEAT == "Y" && acc_bill_details_trans != null)
            {
                objMaster.HEAD_ID = acc_bill_details_trans.HEAD_ID;
                ViewBag.ddlHead = commonFuncObj.PopulateHead(objMaster);
                objMaster.HEAD_ID = teoDetailsModel.HEAD_ID;
            }
            else
            {
                ViewBag.ddlHead = commonFuncObj.PopulateHead(objMaster);
            }
            if (teoDetailsModel.HEAD_ID != 0)
            {
                ACC_TEO_SCREEN_DESIGN_HEADWISE_DETAILS headParams = new ACC_TEO_SCREEN_DESIGN_HEADWISE_DETAILS();
                headParams = commonFuncObj.GetHeadwiseDesignParams(objMaster);
                if (headParams == null)
                {
                    headParams = teoDAL.setDefualtDesignHeadParam();
                }
                if (headParams.CON_REQ == "Y")
                {
                    ViewBag.ddlHeadContractor = commonFuncObj.PopulateContractorSupplier(objMaster);
                }
                else
                {
                    ViewBag.ddlHeadContractor = new SelectList(new List<SelectListItem>().AsEnumerable<SelectListItem>(), "Value", "Text");
                }
                if (headParams.AGREEMENT_REQ == "Y")
                {
                    ViewBag.ddlHeadAgreement = commonFuncObj.PopulateAgreement(objMaster);
                }
                else
                {
                    ViewBag.ddlHeadAgreement = new SelectList(new List<SelectListItem>().AsEnumerable<SelectListItem>(), "Value", "Text");
                }
                if (headParams.SANC_YEAR_REQ == "Y")
                {
                    ViewBag.ddlHeadSancYear = commonFuncObj.PopulateSancYear(objMaster);
                }
                else
                {
                    ViewBag.ddlHeadSancYear = new SelectList(new List<SelectListItem>().AsEnumerable<SelectListItem>(), "Value", "Text");
                }
                if (headParams.ROAD_REQ == "Y")
                {
                    ViewBag.ddlHeadRoad = commonFuncObj.PopulateRoad(objMaster);
                }
                else
                {
                    ViewBag.ddlHeadRoad = new SelectList(new List<SelectListItem>().AsEnumerable<SelectListItem>(), "Value", "Text");
                }
                if (headParams.PKG_REQ == "Y")
                {
                    ViewBag.ddlHeadPackage = commonFuncObj.PopulatePackage(objMaster);
                }
                else
                {
                    ViewBag.ddlHeadPackage = new SelectList(new List<SelectListItem>().AsEnumerable<SelectListItem>(), "Value", "Text");
                }
            }
            else
            {
                ACC_TEO_SCREEN_DESIGN_HEADWISE_DETAILS headParams = new ACC_TEO_SCREEN_DESIGN_HEADWISE_DETAILS();
                if (acc_bill_details_trans != null)
                {
                    objMaster.HEAD_ID = acc_bill_details_trans.HEAD_ID;
                }
                headParams = commonFuncObj.GetHeadwiseDesignParams(objMaster);
                objMaster.HEAD_ID = teoDetailsModel.HEAD_ID;
                if (headParams == null)
                {
                    headParams = teoDAL.setDefualtDesignHeadParam();
                }

                if (acc_bill_details_trans != null && headParams.CON_REQ == "Y")
                {
                    objMaster.MAST_CONT_ID = Convert.ToInt32(acc_bill_details_trans.MAST_CON_ID);
                    ViewBag.ddlHeadContractor = commonFuncObj.PopulateContractorSupplier(objMaster);
                    objMaster.MAST_CONT_ID = 0;
                }
                else
                {
                    ViewBag.ddlHeadContractor = new SelectList(new List<SelectListItem>().AsEnumerable<SelectListItem>(), "Value", "Text");
                }

                if (acc_bill_details_trans != null && headParams.AGREEMENT_REQ == "Y")
                {
                    Int32 tmpAggrementCode = objMaster.AGREEMENT_CODE;
                    Int32 tmpDistrict = objMaster.DISTRICT_CODE;
                    Int32 tmpContID = objMaster.MAST_CONT_ID;

                    objMaster.DISTRICT_CODE = PMGSYSession.Current.LevelId == 5 ? PMGSYSession.Current.DistrictCode : Convert.ToInt32(acc_bill_details_trans.MAST_DISTRICT_CODE); ;
                    objMaster.MAST_CONT_ID = Convert.ToInt32(acc_bill_details_trans.MAST_CON_ID);
                    objMaster.AGREEMENT_CODE = Convert.ToInt32(acc_bill_details_trans.IMS_AGREEMENT_CODE);
                    ViewBag.ddlHeadAgreement = commonFuncObj.PopulateAgreement(objMaster);
                    objMaster.AGREEMENT_CODE = tmpAggrementCode;
                    objMaster.MAST_CONT_ID = tmpContID;
                    objMaster.DISTRICT_CODE = tmpDistrict;
                }
                else
                {
                    ViewBag.ddlHeadAgreement = new SelectList(new List<SelectListItem>().AsEnumerable<SelectListItem>(), "Value", "Text");
                }

                //if (validationParams.SANC_YEAR_REPEAT == "Y" && acc_bill_details_trans != null && headParams.SANC_YEAR_REQ == "Y")
                //{
                //    Int32 tmpDistrictCode = objMaster.DISTRICT_CODE;
                //    Int16 tmpSancYear = objMaster.SANC_YEAR;
                //    if (teoDetailsModel.IMS_PR_ROAD_CODE != null && acc_bill_details_trans.IMS_AGREEMENT_CODE == null)
                //    {
                //        objMaster.DISTRICT_CODE = Convert.ToInt32(acc_bill_details_trans.MAST_DISTRICT_CODE);
                //        objMaster.SANC_YEAR = commonFuncObj.getSancYearFromRoad(Convert.ToInt32(acc_bill_details_trans.IMS_PR_ROAD_CODE));                        
                //    }
                //    ViewBag.ddlHeadSancYear = commonFuncObj.PopulateSancYear(objMaster);
                //    objMaster.DISTRICT_CODE = tmpDistrictCode;
                //    objMaster.SANC_YEAR = tmpSancYear;
                //}
                //else
                //{
                ViewBag.ddlHeadSancYear = new SelectList(new List<SelectListItem>().AsEnumerable<SelectListItem>(), "Value", "Text");
                //}

                //if (validationParams.PKG_REQ_REPEAT == "Y" && acc_bill_details_trans != null && headParams.PKG_REQ == "Y")
                //{
                //    Int32 tmpDistrictCode = objMaster.DISTRICT_CODE;
                //    Int32 tmpAdminCode = objMaster.ADMIN_ND_CODE;
                //    Int16 tmpSancYear = objMaster.SANC_YEAR;
                //    String tmpPackageId = objMaster.PACKAGE_ID;

                //    if (teoDetailsModel.IMS_PR_ROAD_CODE != null && acc_bill_details_trans.IMS_AGREEMENT_CODE == null)
                //    {
                //        objMaster.DISTRICT_CODE = Convert.ToInt32(acc_bill_details_trans.MAST_DISTRICT_CODE);
                //        objMaster.SANC_YEAR = commonFuncObj.getSancYearFromRoad(Convert.ToInt32(acc_bill_details_trans.IMS_PR_ROAD_CODE));
                //        objMaster.ADMIN_ND_CODE = Convert.ToInt32(acc_bill_details_trans.ADMIN_ND_CODE);
                //        objMaster.PACKAGE_ID = commonFuncObj.getPackageFromRoad(Convert.ToInt32(acc_bill_details_trans.IMS_PR_ROAD_CODE));
                //    }
                //    ViewBag.ddlHeadSancYear = commonFuncObj.PopulatePackage(objMaster);
                //    objMaster.DISTRICT_CODE = tmpDistrictCode;
                //    objMaster.SANC_YEAR = tmpSancYear;
                //    objMaster.ADMIN_ND_CODE = tmpAdminCode;
                //    objMaster.PACKAGE_ID = tmpPackageId;
                //}
                //else
                //{
                ViewBag.ddlHeadPackage = new SelectList(new List<SelectListItem>().AsEnumerable<SelectListItem>(), "Value", "Text");
                //}

                if (validationParams.IS_ROAD_REPEAT == "Y" && acc_bill_details_trans != null && headParams.ROAD_REQ == "Y")
                {
                    Int32 tmpAggCode = objMaster.AGREEMENT_CODE;
                    objMaster.AGREEMENT_CODE = Convert.ToInt32(acc_bill_details_trans.IMS_AGREEMENT_CODE);
                    objMaster.ROAD_CODE = Convert.ToInt32(acc_bill_details_trans.IMS_PR_ROAD_CODE);
                    ViewBag.ddlHeadRoad = commonFuncObj.PopulateRoad(objMaster);
                    objMaster.AGREEMENT_CODE = tmpAggCode;
                }
                else if (headParams.ROAD_REQ == "Y" && validationParams.IS_AGREEMENT_REPEAT == "Y" && acc_bill_details_trans != null && headParams.AGREEMENT_REQ == "Y")
                {
                    Int32 tmpAggCode = objMaster.AGREEMENT_CODE;
                    objMaster.AGREEMENT_CODE = Convert.ToInt32(acc_bill_details_trans.IMS_AGREEMENT_CODE);
                    objMaster.ROAD_CODE = 0;
                    ViewBag.ddlHeadRoad = commonFuncObj.PopulateRoad(objMaster);
                    objMaster.AGREEMENT_CODE = tmpAggCode;
                }
                //else if (headParams.ROAD_REQ == "Y" && acc_bill_details_trans != null)
                //{
                //    Int32 tmpAggCode = objMaster.AGREEMENT_CODE;
                //    objMaster.AGREEMENT_CODE = Convert.ToInt32(acc_bill_details_trans.IMS_AGREEMENT_CODE);
                //    objMaster.ROAD_CODE = 0;
                //    ViewBag.ddlHeadRoad = commonFuncObj.PopulateRoad(objMaster);
                //    objMaster.AGREEMENT_CODE = tmpAggCode;
                //}
                else
                {
                    ViewBag.ddlHeadRoad = new SelectList(new List<SelectListItem>().AsEnumerable<SelectListItem>(), "Value", "Text");
                }


                if (acc_bill_details_trans != null && validationParams.IS_HEAD_REPEAT == "Y")
                {
                    teoDetailsModel.HEAD_ID = acc_bill_details_trans.HEAD_ID;
                }
            }
            ViewBag.CreditDebit = id.Trim();

            if (teoDetailsModel.MAST_CON_ID_TRANS == null && teoDetailsModel.MAST_CON_ID != null)
            {
                teoDetailsModel.MAST_CON_ID_TRANS = teoDetailsModel.MAST_CON_ID;
            }
            return PartialView("TEOAddDetails", teoDetailsModel);
        }



        ////public ActionResult TEODetails(String parameter, String hash, String key, String id)
        ////{
        ////    teoBAL = new TransferEntryOrderBAL();
        ////    TransferEntryOrderDAL teoDAL = new TransferEntryOrderDAL();
        ////    commonFuncObj = new CommonFunctions();
        ////    TransactionParams objMaster = new TransactionParams();
        ////    TeoDetailsModel teoDetailsModel = new TeoDetailsModel();

        ////    String[] strParameters = URLEncrypt.DecryptParameters(new string[] { parameter, hash, key });

        ////    Int64 billId = 0;
        ////    String TransId = null;
        ////    Int16 TransNo = 0;

        ////    if (strParameters[0].Contains('$'))
        ////    {
        ////        billId = Convert.ToInt64(strParameters[0].Split('$')[0]);
        ////        TransNo = Convert.ToInt16(strParameters[0].Split('$')[1]);
        ////        teoDetailsModel = teoBAL.GetTEODetailByTransNo(billId, TransNo);
        ////        TransId = URLEncrypt.EncryptParameters(new string[] { billId.ToString().Trim() + "$" + TransNo.ToString().Trim() });

        ////        objMaster.ADMIN_ND_CODE = teoDetailsModel.ADMIN_ND_CODE == null ? PMGSYSession.Current.AdminNdCode : Convert.ToInt32(teoDetailsModel.ADMIN_ND_CODE);
        ////        objMaster.MAST_CONT_ID = teoDetailsModel.MAST_CON_ID == null ? 0 : Convert.ToInt32(teoDetailsModel.MAST_CON_ID);
        ////        objMaster.DISTRICT_CODE = teoDetailsModel.MAST_DISTRICT_CODE == null ? PMGSYSession.Current.DistrictCode : Convert.ToInt32(teoDetailsModel.MAST_DISTRICT_CODE);
        ////        objMaster.AGREEMENT_CODE = teoDetailsModel.IMS_AGREEMENT_CODE == null ? 0 : Convert.ToInt32(teoDetailsModel.IMS_AGREEMENT_CODE);
        ////        objMaster.HEAD_ID = teoDetailsModel.HEAD_ID;
        ////        //If condition 'old - objMaster.AGREEMENT_CODE == 0' modified by Abhishek kamble 17Nov2014 
        ////        if (teoDetailsModel.IMS_PR_ROAD_CODE != null && objMaster.AGREEMENT_CODE != 0)
        ////        {
        ////            objMaster.SANC_YEAR = commonFuncObj.getSancYearFromRoad(Convert.ToInt32(teoDetailsModel.IMS_PR_ROAD_CODE));
        ////            objMaster.PACKAGE_ID = commonFuncObj.getPackageFromRoad(Convert.ToInt32(teoDetailsModel.IMS_PR_ROAD_CODE));
        ////        }
        ////    }
        ////    else
        ////    {
        ////        billId = Convert.ToInt64(strParameters[0]);
        ////        objMaster.ADMIN_ND_CODE = PMGSYSession.Current.AdminNdCode;
        ////        objMaster.MAST_CONT_ID = 0;
        ////        objMaster.DISTRICT_CODE = PMGSYSession.Current.DistrictCode;

        ////    }
        ////    ViewBag.TransId = TransId;
        ////    Int16 parentId = teoBAL.GetMasterTransId(billId);
        ////    ViewBag.BILL_ID = URLEncrypt.EncryptParameters(new string[] { billId.ToString().Trim() });
        ////    if (id.Trim().ToLower().Equals("c"))
        ////    {
        ////        ViewBag.TEODetailTitle = "Credit Details";
        ////        ViewBag.SaveTEODetails = "btnSaveTEOCreditDetails";
        ////        ViewBag.EditTEODetails = "btnEditTEOCreditDetails";
        ////        ViewBag.CancelTEODetails = "btnCancelTEOCreditDetails";
        ////        ViewBag.TransIdC = TransId;
        ////    }
        ////    else
        ////    {
        ////        ViewBag.TEODetailTitle = "Debit Details";
        ////        ViewBag.SaveTEODetails = "btnSaveTEODebitDetails";
        ////        ViewBag.EditTEODetails = "btnEditTEODebitDetails";
        ////        ViewBag.CancelTEODetails = "btnCancelTEODebitDetails";
        ////        ViewBag.TransIdD = TransId;
        ////    }

        ////    //change is for multiple transaction
        ////    #region New Requirement 06/07/2013

        ////    ACC_BILL_DETAILS acc_bill_details_trans = new ACC_BILL_DETAILS();
        ////    if (id.Trim() == "C")
        ////    {
        ////        //2)acc_bill_details_trans = teoBAL.GetBillDetails(billId, "D"); // 1 )dbContext.ACC_BILL_DETAILS.Where(m => m.BILL_ID == teoDetailsModel.BILL_ID && m.CREDIT_DEBIT == "D").FirstOrDefault();
        ////        acc_bill_details_trans = teoBAL.GetBillDetails(billId, "D"); //change by amol
        ////    }
        ////    else
        ////    {
        ////        //2) acc_bill_details_trans = teoBAL.GetBillDetails(billId, "C"); //1)dbContext.ACC_BILL_DETAILS.Where(m => m.BILL_ID == teoDetailsModel.BILL_ID && m.CREDIT_DEBIT == "C").FirstOrDefault();
        ////        acc_bill_details_trans = teoBAL.GetBillDetails(billId, "C");//change by amol
        ////    }



        ////    ACC_TEO_SCREEN_TXN_VALIDATIONS validationParams = new ACC_TEO_SCREEN_TXN_VALIDATIONS();

        ////    validationParams = commonFuncObj.GetTEOValidationParams(0, parentId);

        ////    #endregion

        ////    ACC_TEO_SCREEN_DESIGN_PARAM_DETAILS designParams = new ACC_TEO_SCREEN_DESIGN_PARAM_DETAILS();
        ////    designParams = commonFuncObj.getTEODesignParamDetails(0, parentId);

        ////    if (designParams == null)
        ////    {
        ////        designParams = teoDAL.setDefualtDesignTransParam();
        ////    }

        ////    objMaster.BILL_TYPE = "J";
        ////    objMaster.FUND_TYPE = PMGSYSession.Current.FundType;
        ////    objMaster.LVL_ID = PMGSYSession.Current.LevelId;
        ////    objMaster.TXN_ID = parentId;
        ////    objMaster.BILL_ID = billId;
        ////    objMaster.STATE_CODE = PMGSYSession.Current.StateCode;

        ////    objMaster.CREDIT_DEBIT = id.Trim();

        ////    if (designParams.CON_REQ == "Y")
        ////    {
        ////        objMaster.MAST_CON_SUP_FLAG = "C";
        ////    }
        ////    else if (designParams.SUPP_REQ == "Y")
        ////    {
        ////        objMaster.MAST_CON_SUP_FLAG = "S";
        ////    }

        ////    if (designParams.DISTRICT_REQ == "Y")
        ////    {
        ////        if (validationParams.IS_DISTRICT_REPEAT == "Y" && acc_bill_details_trans != null)
        ////        {
        ////            ViewBag.ddlDistrict = commonFuncObj.PopulateDistrictforTOB(objMaster.STATE_CODE, false, Convert.ToInt32(acc_bill_details_trans.MAST_DISTRICT_CODE));
        ////        }
        ////        else
        ////        {
        ////            ViewBag.ddlDistrict = commonFuncObj.PopulateDistrictforTOB(objMaster.STATE_CODE);
        ////        }
        ////    }
        ////    else
        ////    {
        ////        ViewBag.ddlDistrict = new SelectList(new List<SelectListItem>().AsEnumerable<SelectListItem>(), "Value", "Text");
        ////    }
        ////    if (designParams.DPIU_REQ == "Y")
        ////    {
        ////        if (validationParams.IS_DPIU_REPEAT == "Y" && acc_bill_details_trans != null)
        ////        {
        ////            Int32 tmpDistrict = objMaster.DISTRICT_CODE;
        ////            Int32 tmpAdminCode = objMaster.ADMIN_ND_CODE;
        ////            if (validationParams.IS_DPIU_REPEAT == "Y" && acc_bill_details_trans != null)
        ////            {
        ////                objMaster.DISTRICT_CODE = PMGSYSession.Current.LevelId == 5 ? PMGSYSession.Current.DistrictCode : Convert.ToInt32(acc_bill_details_trans.MAST_DISTRICT_CODE);
        ////                objMaster.ADMIN_ND_CODE = Convert.ToInt32(acc_bill_details_trans.ADMIN_ND_CODE);
        ////                ViewBag.ddlDPIU = commonFuncObj.PopulateDPIU(objMaster);
        ////            }
        ////            else
        ////            {
        ////                objMaster.DISTRICT_CODE = PMGSYSession.Current.LevelId == 5 ? PMGSYSession.Current.DistrictCode : Convert.ToInt32(acc_bill_details_trans.MAST_DISTRICT_CODE);
        ////                ViewBag.ddlDPIU = commonFuncObj.PopulateDPIU(objMaster);

        ////            }
        ////            objMaster.ADMIN_ND_CODE = tmpAdminCode;
        ////            objMaster.DISTRICT_CODE = tmpDistrict;
        ////        }
        ////        else if (TransNo == 0)
        ////        {
        ////            List<SelectListItem> lstOneItem = new List<SelectListItem>();
        ////            lstOneItem.Insert(0, (new SelectListItem { Text = "Select Department", Value = "0", Selected = true }));
        ////            ViewBag.ddlDPIU = lstOneItem;
        ////        }
        ////        else
        ////        {
        ////            ViewBag.ddlDPIU = commonFuncObj.PopulateDPIU(objMaster);
        ////        }

        ////        //code by amol jadhav 
        ////        //if district is same but  dpiu is not repeted 
        ////        if (validationParams.IS_DISTRICT_REPEAT == "Y" && validationParams.IS_DPIU_REPEAT == "N" && acc_bill_details_trans != null)
        ////        {
        ////            objMaster.DISTRICT_CODE = PMGSYSession.Current.LevelId == 5 ? PMGSYSession.Current.DistrictCode : Convert.ToInt32(acc_bill_details_trans.MAST_DISTRICT_CODE);
        ////            objMaster.ADMIN_ND_CODE = Convert.ToInt32(acc_bill_details_trans.ADMIN_ND_CODE);
        ////            ViewBag.ddlDPIU = commonFuncObj.PopulateDPIU(objMaster);
        ////        }
        ////    }
        ////    else
        ////    {
        ////        ViewBag.ddlDPIU = new SelectList(new List<SelectListItem>().AsEnumerable<SelectListItem>(), "Value", "Text");
        ////    }

        ////    if (designParams.CON_REQ == "Y" || designParams.SUPP_REQ == "Y")
        ////    {
        ////        if (acc_bill_details_trans != null)
        ////        {
        ////            objMaster.MAST_CONT_ID = Convert.ToInt32(acc_bill_details_trans.MAST_CON_ID);
        ////            ViewBag.ddlContractor = commonFuncObj.PopulateContractorSupplier(objMaster);
        ////           // objMaster.MAST_CONT_ID = 0;
        ////        }
        ////        else
        ////        {
        ////            ViewBag.ddlContractor = commonFuncObj.PopulateContractorSupplier(objMaster);
        ////        }
        ////    }
        ////    else
        ////    {
        ////        ViewBag.ddlContractor = new SelectList(new List<SelectListItem>().AsEnumerable<SelectListItem>(), "Value", "Text");
        ////    }

        ////    if (designParams.AFREEMENT_REQ == "Y")
        ////    {
        ////        if (acc_bill_details_trans != null)
        ////        {
        ////            Int32 tmpAggrementCode = objMaster.AGREEMENT_CODE;
        ////            Int32 tmpDistrict = objMaster.DISTRICT_CODE;
        ////            Int32 tmpContID = objMaster.MAST_CONT_ID;
        ////            objMaster.DISTRICT_CODE = PMGSYSession.Current.LevelId == 5 ? PMGSYSession.Current.DistrictCode : Convert.ToInt32(acc_bill_details_trans.MAST_DISTRICT_CODE);
        ////            objMaster.MAST_CONT_ID = Convert.ToInt32(acc_bill_details_trans.MAST_CON_ID);
        ////            objMaster.AGREEMENT_CODE = Convert.ToInt32(acc_bill_details_trans.IMS_AGREEMENT_CODE);
        ////            ViewBag.ddlAgreement = commonFuncObj.PopulateAgreement(objMaster);
        ////            objMaster.AGREEMENT_CODE = tmpAggrementCode;
        ////            objMaster.MAST_CONT_ID = tmpContID;
        ////            objMaster.DISTRICT_CODE = tmpDistrict;
        ////        }
        ////        else if (TransNo == 0)
        ////        {
        ////            List<SelectListItem> lstOneItem = new List<SelectListItem>();
        ////            lstOneItem.Insert(0, (new SelectListItem { Text = "Select Agreement", Value = "0", Selected = true }));
        ////            ViewBag.ddlAgreement = lstOneItem;
        ////        }
        ////        else
        ////        {
        ////            ViewBag.ddlAgreement = commonFuncObj.PopulateAgreement(objMaster);
        ////        }
        ////    }
        ////    else
        ////    {
        ////        ViewBag.ddlAgreement = new SelectList(new List<SelectListItem>().AsEnumerable<SelectListItem>(), "Value", "Text");
        ////    }

        ////    if (designParams.SAN_REQ == "Y")
        ////    {
        ////        ViewBag.ddlSancYear = commonFuncObj.PopulateSancYear(objMaster);
        ////    }
        ////    else
        ////    {
        ////        ViewBag.ddlSancYear = new SelectList(new List<SelectListItem>().AsEnumerable<SelectListItem>(), "Value", "Text");
        ////    }


        ////    //Added by Abhishek kamble to get Agreement No to populate road for MF start

        ////    if (PMGSYSession.Current.FundType == "M")
        ////    {
        ////        ACC_BILL_DETAILS acc_bill_detailsC = new ACC_BILL_DETAILS();
        ////        ACC_BILL_DETAILS acc_bill_detailsD = new ACC_BILL_DETAILS();
        ////        acc_bill_detailsC = teoBAL.GetBillDetails(billId, "C");
        ////        acc_bill_detailsD = teoBAL.GetBillDetails(billId, "D");

        ////        int? _ImsAgreementCode;
        ////      //  int _MastConID;

        ////        if (acc_bill_detailsC != null)       
        ////        {
        ////            _ImsAgreementCode = teoDetailsModel.IMS_AGREEMENT_CODE == null ? acc_bill_detailsC.IMS_AGREEMENT_CODE : teoDetailsModel.IMS_AGREEMENT_CODE;
        ////            objMaster.AGREEMENT_NUMBER = teoBAL.GetAgreementNumberForMF(_ImsAgreementCode);
        ////        }
        ////        else if (acc_bill_detailsD != null)
        ////        {
        ////            _ImsAgreementCode = teoDetailsModel.IMS_AGREEMENT_CODE == null ? acc_bill_detailsD.IMS_AGREEMENT_CODE : teoDetailsModel.IMS_AGREEMENT_CODE;
        ////            objMaster.AGREEMENT_NUMBER = teoBAL.GetAgreementNumberForMF(_ImsAgreementCode);
        ////        }

        ////        //Get Contractor ID and Sanc Year
        ////        if (objMaster.MAST_CONT_ID == 0 && (acc_bill_detailsC != null || acc_bill_detailsD!=null))
        ////        {
        ////            //short _txnId = (id.Trim() == "C") ? (short)2 : (short)1;
        ////            //TeoDetailsModel _teoDetailsViewModel = teoBAL.GetTEODetailByTransNo(billId, _txnId);
        ////            if (acc_bill_detailsC != null)
        ////            {
        ////                //objMaster.MAST_CONT_ID = (teoDetailsModel.MAST_CON_ID == null || teoDetailsModel.MAST_CON_ID == 0) ? (acc_bill_detailsC.MAST_CON_ID.Value) : (teoDetailsModel.MAST_CON_ID.Value);
        ////                objMaster.MAST_CONT_ID = (teoDetailsModel.MAST_CON_ID == null || teoDetailsModel.MAST_CON_ID == 0) ? 0 : (teoDetailsModel.MAST_CON_ID.Value);


        ////                int? _IMSPrRoadCodeC = (teoDetailsModel.IMS_PR_ROAD_CODE == null || teoDetailsModel.IMS_PR_ROAD_CODE == 0) ? (acc_bill_detailsC.IMS_PR_ROAD_CODE) : (teoDetailsModel.IMS_PR_ROAD_CODE);

        ////                if (objMaster.SANC_YEAR == 0)
        ////                {
        ////                    if (_IMSPrRoadCodeC != null)
        ////                        objMaster.SANC_YEAR = commonFuncObj.getSancYearFromRoad(Convert.ToInt32(_IMSPrRoadCodeC));
        ////                }
        ////            }
        ////            else {

        ////                objMaster.MAST_CONT_ID = (teoDetailsModel.MAST_CON_ID == null || teoDetailsModel.MAST_CON_ID == 0) ? (acc_bill_detailsD.MAST_CON_ID.Value) : (teoDetailsModel.MAST_CON_ID.Value);

        ////                int? _IMSPrRoadCodeD = (teoDetailsModel.IMS_PR_ROAD_CODE == null || teoDetailsModel.IMS_PR_ROAD_CODE == 0) ? (acc_bill_detailsD.IMS_PR_ROAD_CODE) : (teoDetailsModel.IMS_PR_ROAD_CODE);

        ////                if (objMaster.SANC_YEAR == 0)
        ////                {
        ////                    if (_IMSPrRoadCodeD != null)
        ////                        objMaster.SANC_YEAR = commonFuncObj.getSancYearFromRoad(Convert.ToInt32(_IMSPrRoadCodeD));
        ////                }
        ////            }
        ////        }
        ////    }

        ////    //Added by Abhishek kamble to get Agreement No to populate road for MF end

        ////    //designParams.ROAD_REQ condition necessary bcoz it will populate unnecessarily 
        ////    if (designParams.ROAD_REQ == "Y" && validationParams.IS_ROAD_REPEAT == "Y" && acc_bill_details_trans != null)
        ////    {
        ////        Int32 tmpAggCode = objMaster.AGREEMENT_CODE;
        ////        objMaster.AGREEMENT_CODE = Convert.ToInt32(acc_bill_details_trans.IMS_AGREEMENT_CODE);
        ////        objMaster.ROAD_CODE = Convert.ToInt32(acc_bill_details_trans.IMS_PR_ROAD_CODE);
        ////        ViewBag.ddlRoad = commonFuncObj.PopulateRoad(objMaster);
        ////        objMaster.AGREEMENT_CODE = tmpAggCode;
        ////    }
        ////    else if (designParams.ROAD_REQ == "Y" && validationParams.IS_AGREEMENT_REPEAT == "Y" && acc_bill_details_trans != null && designParams.AFREEMENT_REQ == "Y")
        ////    {
        ////        Int32 tmpAggCode = objMaster.AGREEMENT_CODE;
        ////        objMaster.AGREEMENT_CODE = Convert.ToInt32(acc_bill_details_trans.IMS_AGREEMENT_CODE);
        ////        objMaster.ROAD_CODE = 0;
        ////        ViewBag.ddlRoad = commonFuncObj.PopulateRoad(objMaster);
        ////        objMaster.AGREEMENT_CODE = tmpAggCode;
        ////    }
        ////    else if (designParams.ROAD_REQ == "Y" && acc_bill_details_trans != null)
        ////    {
        ////        Int32 tmpAggCode = objMaster.AGREEMENT_CODE;
        ////        objMaster.AGREEMENT_CODE = Convert.ToInt32(acc_bill_details_trans.IMS_AGREEMENT_CODE);
        ////        objMaster.ROAD_CODE = 0;
        ////        ViewBag.ddlRoad = commonFuncObj.PopulateRoad(objMaster);
        ////        objMaster.AGREEMENT_CODE = tmpAggCode;
        ////    }
        ////    else if (designParams.ROAD_REQ == "Y")
        ////    {
        ////        ViewBag.ddlRoad = commonFuncObj.PopulateRoad(objMaster);
        ////    }
        ////    else
        ////    {
        ////        ViewBag.ddlRoad = new SelectList(new List<SelectListItem>().AsEnumerable<SelectListItem>(), "Value", "Text");
        ////    }

        ////    if (designParams.PKG_REQ == "Y")
        ////    {
        ////        if (TransNo == 0)
        ////        {
        ////            List<SelectListItem> lstOneItem = new List<SelectListItem>();
        ////            lstOneItem.Insert(0, (new SelectListItem { Text = "Select Package", Value = "0", Selected = true }));
        ////            ViewBag.ddlPackage = lstOneItem;
        ////        }
        ////        else
        ////        {
        ////            ViewBag.ddlPackage = commonFuncObj.PopulatePackage(objMaster);
        ////        }
        ////    }
        ////    else
        ////    {
        ////        ViewBag.ddlPackage = new SelectList(new List<SelectListItem>().AsEnumerable<SelectListItem>(), "Value", "Text");
        ////    }
        ////    if (validationParams.IS_HEAD_REPEAT == "Y" && acc_bill_details_trans != null)
        ////    {
        ////        objMaster.HEAD_ID = acc_bill_details_trans.HEAD_ID;
        ////        ViewBag.ddlHead = commonFuncObj.PopulateHead(objMaster);
        ////        objMaster.HEAD_ID = teoDetailsModel.HEAD_ID;
        ////    }
        ////    else
        ////    {
        ////        ViewBag.ddlHead = commonFuncObj.PopulateHead(objMaster);
        ////    }
        ////    if (teoDetailsModel.HEAD_ID != 0)
        ////    {
        ////        ACC_TEO_SCREEN_DESIGN_HEADWISE_DETAILS headParams = new ACC_TEO_SCREEN_DESIGN_HEADWISE_DETAILS();
        ////        headParams = commonFuncObj.GetHeadwiseDesignParams(objMaster);
        ////        if (headParams == null)
        ////        {
        ////            headParams = teoDAL.setDefualtDesignHeadParam();
        ////        }
        ////        if (headParams.CON_REQ == "Y")
        ////        {
        ////            ViewBag.ddlHeadContractor = commonFuncObj.PopulateContractorSupplier(objMaster);
        ////        }
        ////        else
        ////        {
        ////            ViewBag.ddlHeadContractor = new SelectList(new List<SelectListItem>().AsEnumerable<SelectListItem>(), "Value", "Text");
        ////        }
        ////        if (headParams.AGREEMENT_REQ == "Y")
        ////        {
        ////            ViewBag.ddlHeadAgreement = commonFuncObj.PopulateAgreement(objMaster);
        ////        }
        ////        else
        ////        {
        ////            ViewBag.ddlHeadAgreement = new SelectList(new List<SelectListItem>().AsEnumerable<SelectListItem>(), "Value", "Text");
        ////        }
        ////        if (headParams.SANC_YEAR_REQ == "Y")
        ////        {
        ////            ViewBag.ddlHeadSancYear = commonFuncObj.PopulateSancYear(objMaster);
        ////        }
        ////        else
        ////        {
        ////            ViewBag.ddlHeadSancYear = new SelectList(new List<SelectListItem>().AsEnumerable<SelectListItem>(), "Value", "Text");
        ////        }
        ////        if (headParams.ROAD_REQ == "Y")
        ////        {
        ////            ViewBag.ddlHeadRoad = commonFuncObj.PopulateRoad(objMaster);
        ////        }
        ////        else
        ////        {
        ////            ViewBag.ddlHeadRoad = new SelectList(new List<SelectListItem>().AsEnumerable<SelectListItem>(), "Value", "Text");
        ////        }
        ////        if (headParams.PKG_REQ == "Y")
        ////        {
        ////            ViewBag.ddlHeadPackage = commonFuncObj.PopulatePackage(objMaster);
        ////        }
        ////        else
        ////        {
        ////            ViewBag.ddlHeadPackage = new SelectList(new List<SelectListItem>().AsEnumerable<SelectListItem>(), "Value", "Text");
        ////        }
        ////    }
        ////    else
        ////    {
        ////        ACC_TEO_SCREEN_DESIGN_HEADWISE_DETAILS headParams = new ACC_TEO_SCREEN_DESIGN_HEADWISE_DETAILS();
        ////        if (acc_bill_details_trans != null)
        ////        {
        ////            objMaster.HEAD_ID = acc_bill_details_trans.HEAD_ID;
        ////        }
        ////        headParams = commonFuncObj.GetHeadwiseDesignParams(objMaster);
        ////        objMaster.HEAD_ID = teoDetailsModel.HEAD_ID;
        ////        if (headParams == null)
        ////        {
        ////            headParams = teoDAL.setDefualtDesignHeadParam();
        ////        }

        ////        if (acc_bill_details_trans != null && headParams.CON_REQ == "Y")
        ////        {
        ////            objMaster.MAST_CONT_ID = Convert.ToInt32(acc_bill_details_trans.MAST_CON_ID);
        ////            ViewBag.ddlHeadContractor = commonFuncObj.PopulateContractorSupplier(objMaster);
        ////            objMaster.MAST_CONT_ID = 0;
        ////        }
        ////        else
        ////        {
        ////            ViewBag.ddlHeadContractor = new SelectList(new List<SelectListItem>().AsEnumerable<SelectListItem>(), "Value", "Text");
        ////        }

        ////        if (acc_bill_details_trans != null && headParams.AGREEMENT_REQ == "Y")
        ////        {
        ////            Int32 tmpAggrementCode = objMaster.AGREEMENT_CODE;
        ////            Int32 tmpDistrict = objMaster.DISTRICT_CODE;
        ////            Int32 tmpContID = objMaster.MAST_CONT_ID;

        ////            objMaster.DISTRICT_CODE = PMGSYSession.Current.LevelId == 5 ? PMGSYSession.Current.DistrictCode : Convert.ToInt32(acc_bill_details_trans.MAST_DISTRICT_CODE); ;
        ////            objMaster.MAST_CONT_ID = Convert.ToInt32(acc_bill_details_trans.MAST_CON_ID);
        ////            objMaster.AGREEMENT_CODE = Convert.ToInt32(acc_bill_details_trans.IMS_AGREEMENT_CODE);
        ////            ViewBag.ddlHeadAgreement = commonFuncObj.PopulateAgreement(objMaster);
        ////            objMaster.AGREEMENT_CODE = tmpAggrementCode;
        ////            objMaster.MAST_CONT_ID = tmpContID;
        ////            objMaster.DISTRICT_CODE = tmpDistrict;
        ////        }
        ////        else
        ////        {
        ////            ViewBag.ddlHeadAgreement = new SelectList(new List<SelectListItem>().AsEnumerable<SelectListItem>(), "Value", "Text");
        ////        }

        ////        //if (validationParams.SANC_YEAR_REPEAT == "Y" && acc_bill_details_trans != null && headParams.SANC_YEAR_REQ == "Y")
        ////        //{
        ////        //    Int32 tmpDistrictCode = objMaster.DISTRICT_CODE;
        ////        //    Int16 tmpSancYear = objMaster.SANC_YEAR;
        ////        //    if (teoDetailsModel.IMS_PR_ROAD_CODE != null && acc_bill_details_trans.IMS_AGREEMENT_CODE == null)
        ////        //    {
        ////        //        objMaster.DISTRICT_CODE = Convert.ToInt32(acc_bill_details_trans.MAST_DISTRICT_CODE);
        ////        //        objMaster.SANC_YEAR = commonFuncObj.getSancYearFromRoad(Convert.ToInt32(acc_bill_details_trans.IMS_PR_ROAD_CODE));                        
        ////        //    }
        ////        //    ViewBag.ddlHeadSancYear = commonFuncObj.PopulateSancYear(objMaster);
        ////        //    objMaster.DISTRICT_CODE = tmpDistrictCode;
        ////        //    objMaster.SANC_YEAR = tmpSancYear;
        ////        //}
        ////        //else
        ////        //{
        ////        ViewBag.ddlHeadSancYear = new SelectList(new List<SelectListItem>().AsEnumerable<SelectListItem>(), "Value", "Text");
        ////        //}

        ////        //if (validationParams.PKG_REQ_REPEAT == "Y" && acc_bill_details_trans != null && headParams.PKG_REQ == "Y")
        ////        //{
        ////        //    Int32 tmpDistrictCode = objMaster.DISTRICT_CODE;
        ////        //    Int32 tmpAdminCode = objMaster.ADMIN_ND_CODE;
        ////        //    Int16 tmpSancYear = objMaster.SANC_YEAR;
        ////        //    String tmpPackageId = objMaster.PACKAGE_ID;

        ////        //    if (teoDetailsModel.IMS_PR_ROAD_CODE != null && acc_bill_details_trans.IMS_AGREEMENT_CODE == null)
        ////        //    {
        ////        //        objMaster.DISTRICT_CODE = Convert.ToInt32(acc_bill_details_trans.MAST_DISTRICT_CODE);
        ////        //        objMaster.SANC_YEAR = commonFuncObj.getSancYearFromRoad(Convert.ToInt32(acc_bill_details_trans.IMS_PR_ROAD_CODE));
        ////        //        objMaster.ADMIN_ND_CODE = Convert.ToInt32(acc_bill_details_trans.ADMIN_ND_CODE);
        ////        //        objMaster.PACKAGE_ID = commonFuncObj.getPackageFromRoad(Convert.ToInt32(acc_bill_details_trans.IMS_PR_ROAD_CODE));
        ////        //    }
        ////        //    ViewBag.ddlHeadSancYear = commonFuncObj.PopulatePackage(objMaster);
        ////        //    objMaster.DISTRICT_CODE = tmpDistrictCode;
        ////        //    objMaster.SANC_YEAR = tmpSancYear;
        ////        //    objMaster.ADMIN_ND_CODE = tmpAdminCode;
        ////        //    objMaster.PACKAGE_ID = tmpPackageId;
        ////        //}
        ////        //else
        ////        //{
        ////        ViewBag.ddlHeadPackage = new SelectList(new List<SelectListItem>().AsEnumerable<SelectListItem>(), "Value", "Text");
        ////        //}

        ////        if (validationParams.IS_ROAD_REPEAT == "Y" && acc_bill_details_trans != null && headParams.ROAD_REQ == "Y")
        ////        {
        ////            Int32 tmpAggCode = objMaster.AGREEMENT_CODE;
        ////            objMaster.AGREEMENT_CODE = Convert.ToInt32(acc_bill_details_trans.IMS_AGREEMENT_CODE);
        ////            objMaster.ROAD_CODE = Convert.ToInt32(acc_bill_details_trans.IMS_PR_ROAD_CODE);
        ////            ViewBag.ddlHeadRoad = commonFuncObj.PopulateRoad(objMaster);
        ////            objMaster.AGREEMENT_CODE = tmpAggCode;
        ////        }
        ////        else if (headParams.ROAD_REQ == "Y" && validationParams.IS_AGREEMENT_REPEAT == "Y" && acc_bill_details_trans != null && headParams.AGREEMENT_REQ == "Y")
        ////        {
        ////            Int32 tmpAggCode = objMaster.AGREEMENT_CODE;
        ////            objMaster.AGREEMENT_CODE = Convert.ToInt32(acc_bill_details_trans.IMS_AGREEMENT_CODE);
        ////            objMaster.ROAD_CODE = 0;
        ////            ViewBag.ddlHeadRoad = commonFuncObj.PopulateRoad(objMaster);
        ////            objMaster.AGREEMENT_CODE = tmpAggCode;
        ////        }
        ////        //else if (headParams.ROAD_REQ == "Y" && acc_bill_details_trans != null)
        ////        //{
        ////        //    Int32 tmpAggCode = objMaster.AGREEMENT_CODE;
        ////        //    objMaster.AGREEMENT_CODE = Convert.ToInt32(acc_bill_details_trans.IMS_AGREEMENT_CODE);
        ////        //    objMaster.ROAD_CODE = 0;
        ////        //    ViewBag.ddlHeadRoad = commonFuncObj.PopulateRoad(objMaster);
        ////        //    objMaster.AGREEMENT_CODE = tmpAggCode;
        ////        //}
        ////        else
        ////        {
        ////            ViewBag.ddlHeadRoad = new SelectList(new List<SelectListItem>().AsEnumerable<SelectListItem>(), "Value", "Text");
        ////        }


        ////        if (acc_bill_details_trans != null && validationParams.IS_HEAD_REPEAT == "Y")
        ////        {
        ////            teoDetailsModel.HEAD_ID = acc_bill_details_trans.HEAD_ID;
        ////        }
        ////    }
        ////    ViewBag.CreditDebit = id.Trim();

        ////    if (teoDetailsModel.MAST_CON_ID_TRANS == null && teoDetailsModel.MAST_CON_ID != null)
        ////    {
        ////        teoDetailsModel.MAST_CON_ID_TRANS = teoDetailsModel.MAST_CON_ID;
        ////    }
        ////    return PartialView("TEOAddDetails", teoDetailsModel);
        ////}

        //public ActionResult TEOImprestDetails(String parameter, String hash, String key, String id)
        //{
        //    teoBAL = new TransferEntryOrderBAL();
        //    TransferEntryOrderDAL teoDAL = new TransferEntryOrderDAL();
        //    commonFuncObj = new CommonFunctions();
        //    TransactionParams objMaster = new TransactionParams();
        //    TeoDetailsModel teoDetailsModel = new TeoDetailsModel();

        //    String[] strParameters = URLEncrypt.DecryptParameters(new string[] { parameter, hash, key });

        //    Int64 PbillId = 0;
        //    Int64 SbillId = 0;
        //    String TransId = null;
        //    Int16 TransNo = 0;

        //    if (strParameters[0].Contains('$'))
        //    {
        //        PbillId = Convert.ToInt64(strParameters[0].Split('$')[0]);
        //        SbillId = Convert.ToInt64(strParameters[0].Split('$')[1]);
        //        teoDetailsModel = teoBAL.GetTEODetailByTransNo(PbillId, TransNo);
        //        //TransId = URLEncrypt.EncryptParameters(new string[] { billId.ToString().Trim() + "$" + TransNo.ToString().Trim() });

        //        objMaster.ADMIN_ND_CODE = teoDetailsModel.ADMIN_ND_CODE == null ? PMGSYSession.Current.AdminNdCode : Convert.ToInt32(teoDetailsModel.ADMIN_ND_CODE);
        //        objMaster.MAST_CONT_ID = teoDetailsModel.MAST_CON_ID == null ? 0 : Convert.ToInt32(teoDetailsModel.MAST_CON_ID);
        //        objMaster.DISTRICT_CODE = teoDetailsModel.MAST_DISTRICT_CODE == null ? PMGSYSession.Current.DistrictCode : Convert.ToInt32(teoDetailsModel.MAST_DISTRICT_CODE);
        //        objMaster.AGREEMENT_CODE = teoDetailsModel.IMS_AGREEMENT_CODE == null ? 0 : Convert.ToInt32(teoDetailsModel.IMS_AGREEMENT_CODE);
        //        objMaster.HEAD_ID = teoDetailsModel.HEAD_ID;
        //        if (teoDetailsModel.IMS_PR_ROAD_CODE != null && objMaster.AGREEMENT_CODE == 0)
        //        {
        //            objMaster.SANC_YEAR = commonFuncObj.getSancYearFromRoad(Convert.ToInt32(teoDetailsModel.IMS_PR_ROAD_CODE));
        //            objMaster.PACKAGE_ID = commonFuncObj.getPackageFromRoad(Convert.ToInt32(teoDetailsModel.IMS_PR_ROAD_CODE));
        //        }
        //    }
        //    else
        //    {
        //        billId = Convert.ToInt64(strParameters[0]);
        //        //// Check for Edit (For Listing Screen to Add/Edit Master Receipt)
        //        //if (db.ACC_BILL_DETAILS.Where(m => m.BILL_ID == billId).Count() > 0)
        //        //{
        //        //    TransId = "Y"; //Populate Receipt Details
        //        //}
        //        //else
        //        //{
        //        //    TransId = "N"; //Do not Populate Receipt Details
        //        //}

        //        objMaster.ADMIN_ND_CODE = PMGSYSession.Current.AdminNdCode;
        //        objMaster.MAST_CONT_ID = 0;
        //        objMaster.DISTRICT_CODE = PMGSYSession.Current.DistrictCode;

        //    }
        //    ViewBag.TransId = TransId;
        //    Int16 parentId = teoBAL.GetMasterTransId(billId);
        //    ViewBag.BILL_ID = URLEncrypt.EncryptParameters(new string[] { billId.ToString().Trim() });
        //    if (id.Trim().ToLower().Equals("c"))
        //    {
        //        ViewBag.TEODetailTitle = "Credit Details";
        //        ViewBag.SaveTEODetails = "btnSaveTEOCreditDetails";
        //        ViewBag.EditTEODetails = "btnEditTEOCreditDetails";
        //        ViewBag.CancelTEODetails = "btnCancelTEOCreditDetails";
        //    }
        //    else
        //    {
        //        ViewBag.TEODetailTitle = "Debit Details";
        //        ViewBag.SaveTEODetails = "btnSaveTEODebitDetails";
        //        ViewBag.EditTEODetails = "btnEditTEODebitDetails";
        //        ViewBag.CancelTEODetails = "btnCancelTEODebitDetails";
        //    }

        //    ACC_TEO_SCREEN_DESIGN_PARAM_DETAILS designParams = new ACC_TEO_SCREEN_DESIGN_PARAM_DETAILS();
        //    designParams = commonFuncObj.getTEODesignParamDetails(0, parentId);
        //    if (designParams == null)
        //    {
        //        designParams = teoDAL.setDefualtDesignTransParam();
        //    }
        //    objMaster.BILL_TYPE = "J";
        //    objMaster.FUND_TYPE = PMGSYSession.Current.FundType;
        //    objMaster.LVL_ID = PMGSYSession.Current.LevelId;
        //    objMaster.TXN_ID = parentId;
        //    objMaster.BILL_ID = billId;
        //    objMaster.STATE_CODE = PMGSYSession.Current.StateCode;

        //    objMaster.CREDIT_DEBIT = id.Trim();

        //    if (designParams.CON_REQ == "Y")
        //    {
        //        objMaster.MAST_CON_SUP_FLAG = "C";
        //    }
        //    else if (designParams.SUPP_REQ == "Y")
        //    {
        //        objMaster.MAST_CON_SUP_FLAG = "S";
        //    }

        //    if (designParams.DISTRICT_REQ == "Y")
        //    {
        //        ViewBag.ddlDistrict = commonFuncObj.PopulateDistrict(objMaster.STATE_CODE);
        //    }
        //    else
        //    {
        //        ViewBag.ddlDistrict = new SelectList(new List<SelectListItem>().AsEnumerable<SelectListItem>(), "Value", "Text");
        //    }
        //    if (designParams.DPIU_REQ == "Y")
        //    {
        //        if (TransNo == 0)
        //        {
        //            List<SelectListItem> lstOneItem = new List<SelectListItem>();
        //            lstOneItem.Insert(0, (new SelectListItem { Text = "Select Department", Value = "0", Selected = true }));
        //            ViewBag.ddlDPIU = lstOneItem;
        //        }
        //        else
        //        {
        //            ViewBag.ddlDPIU = commonFuncObj.PopulateDPIU(objMaster);
        //        }
        //    }
        //    else
        //    {
        //        ViewBag.ddlDPIU = new SelectList(new List<SelectListItem>().AsEnumerable<SelectListItem>(), "Value", "Text");
        //    }

        //    if (designParams.CON_REQ == "Y" || designParams.SUPP_REQ == "Y")
        //    {
        //        ViewBag.ddlContractor = commonFuncObj.PopulateContractorSupplier(objMaster);
        //    }
        //    else
        //    {
        //        ViewBag.ddlContractor = new SelectList(new List<SelectListItem>().AsEnumerable<SelectListItem>(), "Value", "Text");
        //    }

        //    if (designParams.AFREEMENT_REQ == "Y")
        //    {
        //        if (TransNo == 0)
        //        {
        //            List<SelectListItem> lstOneItem = new List<SelectListItem>();
        //            lstOneItem.Insert(0, (new SelectListItem { Text = "Select Agreement", Value = "0", Selected = true }));
        //            ViewBag.ddlAgreement = lstOneItem;
        //        }
        //        else
        //        {
        //            ViewBag.ddlAgreement = commonFuncObj.PopulateAgreement(objMaster);
        //        }
        //    }
        //    else
        //    {
        //        ViewBag.ddlAgreement = new SelectList(new List<SelectListItem>().AsEnumerable<SelectListItem>(), "Value", "Text");
        //    }

        //    if (designParams.SAN_REQ == "Y")
        //    {
        //        ViewBag.ddlSancYear = commonFuncObj.PopulateSancYear(objMaster);
        //    }
        //    else
        //    {
        //        ViewBag.ddlSancYear = new SelectList(new List<SelectListItem>().AsEnumerable<SelectListItem>(), "Value", "Text");
        //    }

        //    if (designParams.ROAD_REQ == "Y")
        //    {
        //        if (TransNo == 0)
        //        {
        //            List<SelectListItem> lstOneItem = new List<SelectListItem>();
        //            lstOneItem.Insert(0, (new SelectListItem { Text = "Select Road", Value = "0", Selected = true }));
        //            ViewBag.ddlRoad = lstOneItem;
        //        }
        //        else
        //        {
        //            ViewBag.ddlRoad = commonFuncObj.PopulateRoad(objMaster);
        //        }
        //    }
        //    else
        //    {
        //        ViewBag.ddlRoad = new SelectList(new List<SelectListItem>().AsEnumerable<SelectListItem>(), "Value", "Text");
        //    }

        //    if (designParams.PKG_REQ == "Y")
        //    {
        //        if (TransNo == 0)
        //        {
        //            List<SelectListItem> lstOneItem = new List<SelectListItem>();
        //            lstOneItem.Insert(0, (new SelectListItem { Text = "Select Package", Value = "0", Selected = true }));
        //            ViewBag.ddlPackage = lstOneItem;
        //        }
        //        else
        //        {
        //            ViewBag.ddlPackage = commonFuncObj.PopulatePackage(objMaster);
        //        }
        //    }
        //    else
        //    {
        //        ViewBag.ddlPackage = new SelectList(new List<SelectListItem>().AsEnumerable<SelectListItem>(), "Value", "Text");
        //    }

        //    ViewBag.ddlHead = commonFuncObj.PopulateHead(objMaster);
        //    if (teoDetailsModel.HEAD_ID != 0)
        //    {
        //        ACC_TEO_SCREEN_DESIGN_HEADWISE_DETAILS headParams = new ACC_TEO_SCREEN_DESIGN_HEADWISE_DETAILS();
        //        headParams = commonFuncObj.GetHeadwiseDesignParams(objMaster);
        //        if (headParams == null)
        //        {
        //            headParams = teoDAL.setDefualtDesignHeadParam();
        //        }
        //        if (headParams.CON_REQ == "Y")
        //        {
        //            ViewBag.ddlHeadContractor = commonFuncObj.PopulateContractorSupplier(objMaster);
        //        }
        //        else
        //        {
        //            ViewBag.ddlHeadContractor = new SelectList(new List<SelectListItem>().AsEnumerable<SelectListItem>(), "Value", "Text");
        //        }
        //        if (headParams.AGREEMENT_REQ == "Y")
        //        {
        //            ViewBag.ddlHeadAgreement = commonFuncObj.PopulateAgreement(objMaster);
        //        }
        //        else
        //        {
        //            ViewBag.ddlHeadAgreement = new SelectList(new List<SelectListItem>().AsEnumerable<SelectListItem>(), "Value", "Text");
        //        }
        //        if (headParams.SANC_YEAR_REQ == "Y")
        //        {
        //            ViewBag.ddlHeadSancYear = commonFuncObj.PopulateSancYear(objMaster);
        //        }
        //        else
        //        {
        //            ViewBag.ddlHeadSancYear = new SelectList(new List<SelectListItem>().AsEnumerable<SelectListItem>(), "Value", "Text");
        //        }
        //        if (headParams.ROAD_REQ == "Y")
        //        {
        //            ViewBag.ddlHeadRoad = commonFuncObj.PopulateRoad(objMaster);
        //        }
        //        else
        //        {
        //            ViewBag.ddlHeadRoad = new SelectList(new List<SelectListItem>().AsEnumerable<SelectListItem>(), "Value", "Text");
        //        }
        //        if (headParams.PKG_REQ == "Y")
        //        {
        //            ViewBag.ddlHeadPackage = commonFuncObj.PopulatePackage(objMaster);
        //        }
        //        else
        //        {
        //            ViewBag.ddlHeadPackage = new SelectList(new List<SelectListItem>().AsEnumerable<SelectListItem>(), "Value", "Text");
        //        }
        //    }
        //    else
        //    {
        //        ViewBag.ddlHeadContractor = new SelectList(new List<SelectListItem>().AsEnumerable<SelectListItem>(), "Value", "Text");
        //        ViewBag.ddlHeadAgreement = new SelectList(new List<SelectListItem>().AsEnumerable<SelectListItem>(), "Value", "Text");
        //        ViewBag.ddlHeadSancYear = new SelectList(new List<SelectListItem>().AsEnumerable<SelectListItem>(), "Value", "Text");
        //        ViewBag.ddlHeadRoad = new SelectList(new List<SelectListItem>().AsEnumerable<SelectListItem>(), "Value", "Text");
        //        ViewBag.ddlHeadPackage = new SelectList(new List<SelectListItem>().AsEnumerable<SelectListItem>(), "Value", "Text");
        //    }
        //    ViewBag.CreditDebit = id.Trim();

        //    return PartialView("TEOAddDetails", teoDetailsModel);
        //}

        [HttpPost]
        [Audit]
        [ValidateAntiForgeryToken]
        public ActionResult AddTEOMaster(TeoMasterModel teoMasterModel)
        {
            String ValidationSummary = String.Empty;
            commonFuncObj = new CommonFunctions();
            teoBAL = new TransferEntryOrderBAL();
            ViewBag.ddlMonth = commonFuncObj.PopulateMonths(teoMasterModel.BILL_MONTH);
            ViewBag.ddlYear = commonFuncObj.PopulateYears(teoMasterModel.BILL_YEAR);

            var errors = ModelState.Values.SelectMany(v => v.Errors);

            if (ModelState.IsValid)
            {
                //Avinash_Start

                PMGSYEntities dbContext = new PMGSYEntities();
                string EncryptedEAuthID = Request.Params["EncryptedEAuthID"];
                if (!string.IsNullOrEmpty(EncryptedEAuthID))
                {
                    teoMasterModel.EncryptedEAuthID = EncryptedEAuthID;

                    if (teoMasterModel.GROSS_AMOUNT == 0)
                    {
                        return this.Json(new { success = false, message = "Entry Can't be Made as Amount is Zero" });
                    }

                    Int64 EAuthID = 0;
                    if (!String.IsNullOrEmpty(teoMasterModel.EncryptedEAuthID))
                    {
                        String[] splitID = EncryptedEAuthID.Split('/');
                        string parameter = splitID[0];
                        string hash = splitID[1];
                        string key = splitID[2];

                        if (!String.IsNullOrEmpty(parameter) && !String.IsNullOrEmpty(hash) && !String.IsNullOrEmpty(key))
                        {
                            String[] urlParams = URLEncrypt.DecryptParameters(new String[] { parameter, hash, key });
                            if (urlParams.Length >= 1)
                            {
                                String[] urlSplitParams = urlParams[0].Split('$');
                                EAuthID = Convert.ToInt64(urlSplitParams[0]);
                            }


                            Int64 Eauthid = dbContext.ACC_NOTIFICATION_DETAILS.Where(x => x.DETAIL_ID == EAuthID).Select(x => x.INITIATION_BILL_ID).FirstOrDefault();
                            Int32 eAuthID = Convert.ToInt32(Eauthid);
                            decimal? MasterAuthAmt = dbContext.ACC_EAUTH_MASTER.Where(x => x.EAUTH_ID == eAuthID).Select(x => x.TOTAL_AUTH_APPROVED).FirstOrDefault();
                            decimal? newMasterAuthAmt = MasterAuthAmt * 100000;

                            if (newMasterAuthAmt != teoMasterModel.GROSS_AMOUNT)
                            {
                                return this.Json(new { success = false, message = "Amount should be equal to Total Authorization Approved" });

                            }




                        }
                        else
                        {
                            return this.Json(new { success = false, message = "Error in Processing,Please try Again" });


                        }


                    }


                }
                //Avinash_End






                teoMasterModel.FUND_TYPE = PMGSYSession.Current.FundType;
                teoMasterModel.LVL_ID = PMGSYSession.Current.LevelId;
                teoMasterModel.ADMIN_ND_CODE = PMGSYSession.Current.AdminNdCode;

                string result = string.Empty;
                String message = String.Empty;
                result = commonFuncObj.MonthlyClosingValidation(teoMasterModel.BILL_MONTH, teoMasterModel.BILL_YEAR, PMGSYSession.Current.FundType, PMGSYSession.Current.LevelId, PMGSYSession.Current.AdminNdCode, ref message);
                if (result == "-111")
                {
                    return this.Json(new { success = false, message = message });
                }
                else if ((result == "-222"))
                {
                    return this.Json(new { success = false, message = "Month is already closed,Please revoke the month and try again." });
                }

                //new change done by Vikram on 13 Jan 2014
                bool status = true;
                if (teoMasterModel.TXN_ID == 169)
                {
                    status = teoBAL.ValidateTransaction(teoMasterModel.TXN_ID, teoMasterModel.BILL_DATE, ref message);
                    if (status == false)
                    {
                        return this.Json(new { success = false, message = message });
                    }
                }

                //end of change


                if (ValidationSummary == String.Empty)
                {
                    Int64 billId = teoBAL.AddTEOMaster(teoMasterModel);
                    //Avinash_Start
                    if (billId == 0)
                    {
                        return this.Json(new { success = false, message = "Transfer Entry Order has Already been filled" });

                    }
                    else
                    {
                        return this.Json(new { success = true, message = URLEncrypt.EncryptParameters(new string[] { billId.ToString().Trim() }), _MasterTxnID = teoMasterModel.TXN_ID });
                    }
                    //Avinash_End

                }
                else
                {
                    return this.Json(new { success = false, message = ValidationSummary });
                }

            }
            else
            {
                TransactionParams objMaster = new TransactionParams();
                objMaster.BILL_TYPE = "J";
                objMaster.FUND_TYPE = PMGSYSession.Current.FundType;
                objMaster.LVL_ID = PMGSYSession.Current.LevelId;
                objMaster.TXN_ID = 0;

                //changes by Koustubh Nakate on 26/08/2013 for removing some transactions 
                List<SelectListItem> lstTransacrion = commonFuncObj.PopulateTransactions(objMaster);

                //lstTransacrion.RemoveRange(4, 4);

                lstTransacrion = (from TXN in lstTransacrion
                                  where !lstTransactionIDs.Any(ID => ID.Value == TXN.Value)
                                  select TXN).ToList();


                ViewBag.ddlMasterTrans = lstTransacrion;//commonFuncObj.PopulateTransactions(objMaster);
                objMaster.TXN_ID = teoMasterModel.TXN_ID;
                ViewBag.ddlSubTrans = commonFuncObj.PopulateTransactions(objMaster);//lstOneItem;
            }
            return PartialView("TEOAddMaster", teoMasterModel);
        }

        /// <summary>
        /// action to save the imprest entry 
        /// </summary>
        /// <param name="teoMasterModel"></param>
        /// <param name="parameter"></param>
        /// <param name="hash"></param>
        /// <param name="key"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        [Audit]
        [ValidateAntiForgeryToken]
        public ActionResult AddImprestMaster(TeoMasterModel teoMasterModel, String parameter, String hash, String key, String id)
        {
            String ValidationSummary = String.Empty;
            commonFuncObj = new CommonFunctions();
            teoBAL = new TransferEntryOrderBAL();
            ViewBag.ddlMonth = commonFuncObj.PopulateMonths(teoMasterModel.BILL_MONTH);
            ViewBag.ddlYear = commonFuncObj.PopulateYears(teoMasterModel.BILL_YEAR);
            //new change done by Vikram as the validation for imprest settlement was wrong in terms of opening balance
            if (!string.IsNullOrEmpty(id))
            {
                teoMasterModel.TXN_NO = Convert.ToInt16(id);
            }

            //end of change


            //ModelState.Remove("GROSS_AMOUNT");
            if (ModelState.IsValid)
            {
                teoMasterModel.FUND_TYPE = PMGSYSession.Current.FundType;
                teoMasterModel.LVL_ID = PMGSYSession.Current.LevelId;
                teoMasterModel.ADMIN_ND_CODE = PMGSYSession.Current.AdminNdCode;
                teoMasterModel.TXN_NO = Convert.ToInt16(id);
                string[] strParameters = URLEncrypt.DecryptParameters(new string[] { parameter, hash, key });
                teoMasterModel.PBILL_ID = Convert.ToInt64(strParameters[0]);

                //validation for date date of imprest should be greater than imprest payment/ ob date
                if (!commonFuncObj.isValidImprestSttlementDate(teoMasterModel.PBILL_ID, teoMasterModel.BILL_DATE))
                {
                    return this.Json(new { success = false, message = "TEO  Date should be greater than date of imprest payment or opening balance date" });
                }

                string result = string.Empty;
                String errMessage = String.Empty;
                result = commonFuncObj.MonthlyClosingValidation(teoMasterModel.BILL_MONTH, teoMasterModel.BILL_YEAR, PMGSYSession.Current.FundType, PMGSYSession.Current.LevelId, PMGSYSession.Current.AdminNdCode, ref errMessage);
                if (result == "-111")
                {
                    return this.Json(new { success = false, message = errMessage });
                }
                else if ((result == "-222"))
                {
                    return this.Json(new { success = false, message = "Month is already closed,Please revoke the month and try again." });
                }


                if (ValidationSummary == String.Empty)
                {
                    Int64 billId = teoBAL.AddImprestMaster(teoMasterModel);
                    return this.Json(new { success = true, message = URLEncrypt.EncryptParameters(new string[] { billId.ToString().Trim() }) });
                }
                else
                {
                    return this.Json(new { success = false, message = ValidationSummary });
                }

            }
            return View("TEOImprest", teoMasterModel);
        }

        [HttpPost]
        [Audit]
        [ValidateAntiForgeryToken]
        public ActionResult EditTEOMaster(TeoMasterModel teoMasterModel, String parameter, String hash, String key)
        {

            String ValidationSummary = String.Empty;
            teoBAL = new TransferEntryOrderBAL();
            commonFuncObj = new CommonFunctions();
            string[] strParameters = URLEncrypt.DecryptParameters(new string[] { parameter, hash, key });
            ViewBag.ddlMonth = commonFuncObj.PopulateMonths(teoMasterModel.BILL_MONTH);
            ViewBag.ddlYear = commonFuncObj.PopulateYears(teoMasterModel.BILL_YEAR);
            ViewBag.BILL_ID = URLEncrypt.EncryptParameters(new string[] { Convert.ToInt64(strParameters[0]).ToString().Trim() });
            ModelState.Remove("BILL_NO");

            //added by Koustuibh Nakate on 15/10/2013 
            if (teoMasterModel.PBILL_ID == null || teoMasterModel.PBILL_ID == 0)
            {
                ModelState.Remove("PBILL_ID");
            }

            if (ModelState.IsValid)
            {
                //added by Koustubh Nakate to check gross amount for imprest settelement
                if (teoMasterModel.PBILL_ID != null && teoMasterModel.PBILL_ID > 0)
                {
                    if (!teoBAL.ValidateGrossAmount(teoMasterModel, ref ValidationSummary))
                    {
                        return Json(new { success = false, message = ValidationSummary });
                    }
                }

                //Added By Abhishek kamble 4-Mar-2014
                //validation for date date of imprest should be greater than imprest payment/ ob date
                if (!(string.IsNullOrEmpty(teoMasterModel.TEObillId)))
                {

                    string[] urlParam = teoMasterModel.TEObillId.Split('/');
                    Int64 pBIllID = 0;
                    //string[] Parameters =String.Empty;

                    if ((urlParam.Count() == 3))
                    {
                        String[] strEncParameters = URLEncrypt.DecryptParameters(new string[] { urlParam[0], urlParam[1], urlParam[2] });
                        pBIllID = Convert.ToInt64(strEncParameters[0]);
                    }
                    else
                    {
                        urlParam = teoMasterModel.TEObillId.Split('_');
                        pBIllID = Convert.ToInt64(urlParam[0]);
                    }

                    if (!commonFuncObj.isValidImprestSttlementDate(pBIllID, teoMasterModel.BILL_DATE))
                    {
                        return this.Json(new { success = false, message = "TEO  Date should be greater than date of imprest payment or opening balance date" });
                    }
                }
                string result = string.Empty;
                String errMessage = string.Empty;
                result = commonFuncObj.MonthlyClosingValidation(teoMasterModel.BILL_MONTH, teoMasterModel.BILL_YEAR, PMGSYSession.Current.FundType, PMGSYSession.Current.LevelId, PMGSYSession.Current.AdminNdCode, ref errMessage);
                if (result == "-111")
                {
                    return this.Json(new { success = false, message = errMessage });
                }
                else if ((result == "-222"))
                {
                    return this.Json(new { success = false, message = "Month is already closed,Please revoke the month and try again." });
                }

                teoMasterModel.BILL_ID = Convert.ToInt64(strParameters[0]);
                ValidationSummary = teoBAL.ValidateEditTEOMaster(teoMasterModel);
                if (ValidationSummary == String.Empty)
                {
                    string status = teoBAL.EditTEOMaster(teoMasterModel);
                    if (status == "")
                    {
                        return this.Json(new { success = true, message = URLEncrypt.EncryptParameters(new string[] { teoMasterModel.BILL_ID.ToString().Trim() }) });
                    }
                    else
                    {
                        return this.Json(new { success = false, message = "Error while processing your request" });
                    }
                }
                else
                {
                    return this.Json(new { success = false, message = ValidationSummary });
                }
            }
            else
            {
                TransactionParams objMaster = new TransactionParams();
                objMaster.BILL_TYPE = "R";
                objMaster.FUND_TYPE = PMGSYSession.Current.FundType;
                objMaster.LVL_ID = PMGSYSession.Current.LevelId;
                objMaster.TXN_ID = 0;
                ViewBag.ddlMasterTrans = commonFuncObj.PopulateTransactions(objMaster);
                //List<SelectListItem> lstOneItem = new List<SelectListItem>();
                //lstOneItem.Insert(0, (new SelectListItem { Text = "Select SubTransaction", Value = "0", Selected = true }));
                objMaster.TXN_ID = teoMasterModel.TXN_ID;
                ViewBag.ddlSubTrans = commonFuncObj.PopulateTransactions(objMaster);//lstOneItem;
            }

            return PartialView("TEOAddMaster", teoMasterModel);
        }

        [Audit]
        public ActionResult DeleteTEOMaster(String parameter, String hash, String key)
        {
            teoBAL = new TransferEntryOrderBAL();
            string[] strParameters = URLEncrypt.DecryptParameters(new string[] { parameter, hash, key });
            string status = teoBAL.DeleteTEO(Convert.ToInt64(strParameters[0]));
            if (status == String.Empty)
            {
                return this.Json(new { success = true, message = String.Empty });
            }
            else
            {
                return this.Json(new { success = true, message = status });
            }
        }

        [Audit]
        public ActionResult TEOMasterList(FormCollection frmCollect)
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
            ReceiptFilterModel objFilter = new ReceiptFilterModel();
            teoBAL = new TransferEntryOrderBAL();
            String[] parameters = Request.Params["masterId"].ToString().Split('/');
            String[] strParameters = URLEncrypt.DecryptParameters(new string[] { parameters[0], parameters[1], parameters[2] });
            if (strParameters.Length > 0)
            {
                objFilter.BillId = Convert.ToInt64(strParameters[0]);
            }
            else
            {
                throw new Exception("Error While Getting Master Data");
            }

            objFilter.page = Convert.ToInt32(frmCollect["page"]) - 1;
            objFilter.rows = Convert.ToInt32(frmCollect["rows"]);
            objFilter.sidx = frmCollect["sidx"].ToString();
            objFilter.sord = frmCollect["sord"].ToString();

            var rowData = teoBAL.TEOMasterList(objFilter);

            var jsonData = new
            {
                rows = rowData,
                total = 1,
                page = objFilter.page,
                records = 1
            };
            return Json(jsonData);
        }


        /// <summary>
        /// action to return the master list of the imprest
        /// </summary>
        /// <param name="frmCollect"></param>
        /// <returns></returns>

        [Audit]
        public ActionResult ImprestMasterList(FormCollection frmCollect)
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
            ReceiptFilterModel objFilter = new ReceiptFilterModel();
            teoBAL = new TransferEntryOrderBAL();
            long totalRecords = 0;

            objFilter.page = Convert.ToInt32(frmCollect["page"]) - 1;
            objFilter.rows = Convert.ToInt32(frmCollect["rows"]);
            objFilter.sidx = frmCollect["sidx"].ToString();
            objFilter.sord = frmCollect["sord"].ToString();
            objFilter.Month = Convert.ToInt16(frmCollect["month"]);
            objFilter.Year = Convert.ToInt16(frmCollect["year"]);
            objFilter.AdminNdCode = PMGSYSession.Current.AdminNdCode;
            objFilter.LevelId = PMGSYSession.Current.LevelId;
            objFilter.FundType = PMGSYSession.Current.FundType;
            if (objFilter.LevelId == 5)
            {
                objFilter.TransId = objFilter.FundType == "P" ? Convert.ToInt16(118) : objFilter.FundType == "A" ? Convert.ToInt16(472) : Convert.ToInt16(808);
            }
            else if (objFilter.LevelId == 4)
            {
                objFilter.TransId = objFilter.FundType == "A" ? Convert.ToInt16(390) : Convert.ToInt16(0);
            }

            objFilter.HeadId = objFilter.FundType == "P" ? Convert.ToInt16(46) : objFilter.FundType == "A" ? Convert.ToInt16(82) : Convert.ToInt16(328);

            var jsonData = new
            {
                rows = teoBAL.ImprestMasterList(objFilter, out totalRecords),
                total = totalRecords <= objFilter.rows ? 1 : totalRecords / objFilter.rows + 1,
                page = objFilter.page + 1,
                records = totalRecords
            };
            return Json(jsonData);

        }

        /// <summary>
        /// action to list the settlement master list against the inprest entry
        /// </summary>
        /// <param name="frmCollect"></param>
        /// <returns></returns>
        [Audit]
        public ActionResult ImprestSettlementMasterList(FormCollection frmCollect)
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
            ReceiptFilterModel objFilter = new ReceiptFilterModel();
            teoBAL = new TransferEntryOrderBAL();
            long totalRecords = 0;

            objFilter.page = Convert.ToInt32(frmCollect["page"]) - 1;
            objFilter.rows = Convert.ToInt32(frmCollect["rows"]);
            objFilter.sidx = frmCollect["sidx"].ToString();
            objFilter.sord = frmCollect["sord"].ToString();
            objFilter.Month = Convert.ToInt16(frmCollect["month"]);
            objFilter.Year = Convert.ToInt16(frmCollect["year"]);
            objFilter.AdminNdCode = PMGSYSession.Current.AdminNdCode;
            objFilter.LevelId = PMGSYSession.Current.LevelId;
            objFilter.FundType = PMGSYSession.Current.FundType;

            string[] paramAckBillValues = Request.Params["imprestCode"].Split('/');

            String[] urlParams = URLEncrypt.DecryptParameters(new String[] { paramAckBillValues[0], paramAckBillValues[1], paramAckBillValues[2] });

            objFilter.BillId = Convert.ToInt64(urlParams[0].Split('$')[0]);
            objFilter.TransId = Convert.ToInt16(urlParams[0].Split('$')[1]);
            var jsonData = new
            {
                rows = teoBAL.ImprestSettlementMasterList(objFilter, out totalRecords),
                total = totalRecords <= objFilter.rows ? 1 : totalRecords / objFilter.rows + 1,
                page = objFilter.page + 1,
                records = totalRecords
            };
            return Json(jsonData);

        }

        /// <summary>
        /// Action to display the teo Details List
        /// </summary>
        /// <param name="frmCollect"></param>
        /// <returns></returns>
        [Audit]
        public ActionResult TEODetailsList(FormCollection frmCollect)
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
            ReceiptFilterModel objFilter = new ReceiptFilterModel();
            teoBAL = new TransferEntryOrderBAL();
            long totalRecords = 0;
            decimal cTotalAmount = 0;
            decimal dTotalAmount = 0;
            decimal GrossAmount = 0;
            //Added By Abhishek kamble 10-dec-2013
            decimal creditRemainingAmt = 0;
            decimal debitRemainingAmt = 0;

            string[] parameters = Request.Params["masterId"].ToString().Split('/');
            string[] strParameters = URLEncrypt.DecryptParameters(new string[] { parameters[0], parameters[1], parameters[2] });
            if (strParameters.Length > 0)
            {
                objFilter.BillId = Convert.ToInt64(strParameters[0]);
            }
            else
            {
                throw new Exception("Error While Getting Master Data");
            }

            objFilter.page = Convert.ToInt32(frmCollect["page"]) - 1;
            objFilter.rows = Convert.ToInt32(frmCollect["rows"]);
            objFilter.sidx = frmCollect["sidx"].ToString();
            objFilter.sord = frmCollect["sord"].ToString();

            String finFlag = "N";
            Array lstTEODetails = teoBAL.TEODetailsList(objFilter, out totalRecords, out cTotalAmount, out dTotalAmount, out GrossAmount);
            if (GrossAmount == cTotalAmount && GrossAmount == dTotalAmount)
            {
                finFlag = "Y";
            }

            //Added By Abhishek kamble 10-dec-2013
            creditRemainingAmt = GrossAmount - cTotalAmount;
            debitRemainingAmt = GrossAmount - dTotalAmount;

            //get if multiple transaction is allowed
            ACC_TEO_SCREEN_DESIGN_PARAM_DETAILS designParams = new ACC_TEO_SCREEN_DESIGN_PARAM_DETAILS();
            commonFuncObj = new CommonFunctions();
            designParams = commonFuncObj.getTEODesignParamDetails(objFilter.BillId, 0);

            var jsonData = new
            {
                rows = lstTEODetails,
                total = totalRecords <= objFilter.rows ? 1 : totalRecords / objFilter.rows + 1,
                page = objFilter.page + 1,
                records = totalRecords,  //modified By Abhishek kamble 10-dec-2013
                userdata = new { DPIU = "<div style='color:green'><b>Total Amount:</b></div>", CAmount = "<div style='color:green'>" + cTotalAmount.ToString("#0.00") + "</div>", DAmount = "<div style='color:green'>" + dTotalAmount.ToString("#0.00") + "</div>", isFinalize = finFlag, multipleTrans = designParams.MTXN_REQ, creditRemainingAmt = creditRemainingAmt, debitRemainingAmt = debitRemainingAmt, GrossAmount = GrossAmount }
            };
            return Json(jsonData);
        }

        [HttpPost]
        [Audit]
        public ActionResult AddCreditTEODetails(TeoDetailsModel teoDetailsModel, String parameter, String hash, String key)
        {
            TransferEntryOrderDAL objDAL = new TransferEntryOrderDAL();
            teoBAL = new TransferEntryOrderBAL();
            String ValidationSummary = String.Empty;
            CommonFunctions commomFuncObj = new CommonFunctions();
            Int16 transId = 0;
            string[] strParameters = URLEncrypt.DecryptParameters(new string[] { parameter, hash, key });

            //Below 2 lines are added on 13-10-2022
            teoDetailsModel.IMS_PR_ROAD_CODE_Head = teoDetailsModel.IMS_PR_ROAD_CODE_Head == -1 ? null : teoDetailsModel.IMS_PR_ROAD_CODE_Head;
            teoDetailsModel.IMS_AGREEMENT_CODE = teoDetailsModel.IMS_AGREEMENT_CODE == -1 ? null : teoDetailsModel.IMS_AGREEMENT_CODE;
            teoDetailsModel.MAST_CON_ID_TRANS = teoDetailsModel.MAST_CON_ID_TRANS == -1 ? null : teoDetailsModel.MAST_CON_ID_TRANS;


            if (strParameters.Length == 0)
            {
                return this.Json(new { success = false, message = "Error while Adding TEO Credit Details" });
            }
            else
            {
                teoDetailsModel.BILL_ID = Convert.ToInt64(strParameters[0]);
            }

            //Changed by SAMMED A. PATIL on 04DEC2018 to skip road validation for head 11.01 for Normal TEO's
            int txnId = objDAL.GetMasterTXNId(teoDetailsModel.BILL_ID);

            //new change done By Vikram 

            //if (teoDetailsModel.HEAD_ID == 29 || teoDetailsModel.HEAD_ID == 28)
            if (teoDetailsModel.HEAD_ID == 29 || (teoDetailsModel.HEAD_ID == 28 && txnId != 166) || teoDetailsModel.HEAD_ID == 427 || teoDetailsModel.HEAD_ID == 428)
            ///Changed by SAMMED A. PATIL on 20FEB2018 for RCPLWE connectivity check as per account head 
            {
                Int32 roadCode = 0;
                string errorKey = string.Empty;
                if (teoDetailsModel.IMS_PR_ROAD_CODE != 0 && teoDetailsModel.IMS_PR_ROAD_CODE != null)
                {
                    roadCode = teoDetailsModel.IMS_PR_ROAD_CODE.Value;
                    errorKey = "IMS_PR_ROAD_CODE";
                }
                else if (teoDetailsModel.IMS_PR_ROAD_CODE_Head != 0 && teoDetailsModel.IMS_PR_ROAD_CODE_Head != null)
                {
                    roadCode = teoDetailsModel.IMS_PR_ROAD_CODE_Head.Value;
                    errorKey = "IMS_PR_ROAD_CODE_Head";
                }

                //if(teoDetailsModel)
                if (ValidateRoad(teoDetailsModel.HEAD_ID, roadCode, PMGSYSession.Current.FundType) == false)
                {
                    //errorKey = "HEAD_ID";
                    ModelState.AddModelError(errorKey, "The selected head is not valid for this road.");
                    return Json(new { success = false, status = "-555", message = "Select Upgrade/New road as per account head." });
                }
            }
            //end of change

            //New validation to validate PMGSY scheme Roads based on Head. start 2 Sep 2014
            if (PMGSYSession.Current.FundType == "P" && (teoDetailsModel.IMS_PR_ROAD_CODE != null && teoDetailsModel.IMS_PR_ROAD_CODE != 0) || (teoDetailsModel.IMS_PR_ROAD_CODE_Head != null && teoDetailsModel.IMS_PR_ROAD_CODE_Head != 0))
            {

                int? RoadCode = (teoDetailsModel.IMS_PR_ROAD_CODE == null || teoDetailsModel.IMS_PR_ROAD_CODE == 0) ? teoDetailsModel.IMS_PR_ROAD_CODE_Head : teoDetailsModel.IMS_PR_ROAD_CODE;


                if (!ValidateRoadForPMGSYScheme(teoDetailsModel.HEAD_ID, RoadCode.Value))
                {
                    return Json(new { success = false, message = "Road can not be Selected, Since it is in different Scheme." });
                }
            }
            //New validation to validate PMGSY scheme Roads based on Head. end

            List<ModelErrorList> lstModelErrorList = new List<ModelErrorList>();

            lstModelErrorList = teoBAL.GetAddTEODetailsModelErrors(teoDetailsModel, "C");
            if (lstModelErrorList.Count != 0)
            {
                foreach (ModelErrorList item in lstModelErrorList)
                {
                    ModelState.AddModelError(item.Key, item.Value);
                }

            }



            if (ModelState.IsValid)
            {
                //ValidationSummary = receiptBAL.ValidateAddReceiptDetails(billDetailsViewModel);
                if (ValidationSummary == String.Empty)
                {
                    //Validate head 55 added By Abhishek kamble 27-Aug-2014 start

                    //int status=ValidateHead55(teoDetailsModel.ACC_BILL_MASTER.DA  

                    //**Commented to allow head 55 entry, as per mail received on march 23 2017 by arun with subject -State Administrative Expenses Head to be unlocked in OMMAS 

                    //if (PMGSYSession.Current.LevelId == 5 && PMGSYSession.Current.FundType == "A")
                    //{
                    //    int? status = ValidateHead55ForTEO(teoDetailsModel.BILL_ID, teoDetailsModel.HEAD_ID, "C");
                    //    if (status == 0)
                    //    {
                    //        return this.Json(new { success = false, message = "Entry is not allowed since Head 55 is valid upto March-2014." });
                    //    }
                    //}

                    ////**Commented to allow head 55 entry, as per mail received on march 23 2017 by arun with subject -State Administrative Expenses Head to be unlocked in OMMAS 

                    //Validate head 55 added By Abhishek kamble 27-Aug-2014 end

                    string success = teoBAL.AddCreditTEODetails(teoDetailsModel, out transId);
                    ViewBag.TRANS_ID = transId;

                    // to check if correct entry as is operational and is required after porting flag
                    string correctionStatus = commomFuncObj.ValidateHeadForCorrection(Convert.ToInt16(teoDetailsModel.TXN_ID), 0, "A");
                    if (correctionStatus != "1")
                    {
                        return this.Json(new { success = false, message = "Invalid Transaction Type" });
                    }

                    if (success == String.Empty)
                    {
                        return this.Json(new { success = true, message = URLEncrypt.EncryptParameters(new string[] { teoDetailsModel.BILL_ID.ToString().Trim() }) });
                    }
                    else
                    {
                        return this.Json(new { success = false, message = URLEncrypt.EncryptParameters(new string[] { teoDetailsModel.BILL_ID.ToString().Trim() }) });
                    }
                }
                else
                {
                    return this.Json(new { success = false, message = ValidationSummary });
                }
            }
            else
            {

                ViewBag.BILL_ID = URLEncrypt.EncryptParameters(new string[] { teoDetailsModel.BILL_ID.ToString().Trim() });
                ViewBag.TransId = null;
                ViewBag.TEODetailTitle = "Credit Details";
                ViewBag.SaveTEODetails = "btnSaveTEOCreditDetails";
                ViewBag.EditTEODetails = "btnEditTEOCreditDetails";
                ViewBag.CancelTEODetails = "btnCancelTEOCreditDetails";

                TransactionParams objMaster = new TransactionParams();
                TransferEntryOrderDAL teoDAL = new TransferEntryOrderDAL();
                commonFuncObj = new CommonFunctions();
                objMaster.ADMIN_ND_CODE = teoDetailsModel.ADMIN_ND_CODE == null ? PMGSYSession.Current.AdminNdCode : Convert.ToInt32(teoDetailsModel.ADMIN_ND_CODE);
                objMaster.MAST_CONT_ID = teoDetailsModel.MAST_CON_ID == null ? 0 : Convert.ToInt32(teoDetailsModel.MAST_CON_ID);
                objMaster.DISTRICT_CODE = teoDetailsModel.MAST_DISTRICT_CODE == null ? PMGSYSession.Current.DistrictCode : Convert.ToInt32(teoDetailsModel.MAST_DISTRICT_CODE);
                objMaster.AGREEMENT_CODE = teoDetailsModel.IMS_AGREEMENT_CODE == null ? 0 : Convert.ToInt32(teoDetailsModel.IMS_AGREEMENT_CODE);
                objMaster.SANC_YEAR = teoDetailsModel.SANC_YEAR == null ? (Int16)0 : Convert.ToInt16(teoDetailsModel.SANC_YEAR);
                objMaster.PACKAGE_ID = teoDetailsModel.IMS_PACKAGE_ID == null ? "0" : teoDetailsModel.IMS_PACKAGE_ID;
                objMaster.HEAD_ID = teoDetailsModel.HEAD_ID;

                if (teoDetailsModel.SANC_YEAR != 0)
                {
                    objMaster.SANC_YEAR = commonFuncObj.getSancYearFromRoad(Convert.ToInt32(teoDetailsModel.IMS_PR_ROAD_CODE));
                    objMaster.PACKAGE_ID = commonFuncObj.getPackageFromRoad(Convert.ToInt32(teoDetailsModel.IMS_PR_ROAD_CODE));
                }


                //Added by Abhishek kamble to get Agreement No to populate road for MF start
                if (teoDetailsModel != null && PMGSYSession.Current.FundType == "M")
                {
                    objMaster.AGREEMENT_NUMBER = teoBAL.GetAgreementNumberForMF(teoDetailsModel.IMS_AGREEMENT_CODE);
                }
                //Added by Abhishek kamble to get Agreement No to populate road for MF end


                Int16 parentId = teoBAL.GetMasterTransId(teoDetailsModel.BILL_ID);
                ACC_BILL_DETAILS acc_bill_details_trans = new ACC_BILL_DETAILS();
                ViewBag.parentTxnId = parentId;//Added on 13-10-2022

                acc_bill_details_trans = teoBAL.GetBillDetails(teoDetailsModel.BILL_ID, "D"); //dbContext.ACC_BILL_DETAILS.Where(m => m.BILL_ID == teoDetailsModel.BILL_ID && m.CREDIT_DEBIT == "D").FirstOrDefault();


                ACC_TEO_SCREEN_TXN_VALIDATIONS validationParams = new ACC_TEO_SCREEN_TXN_VALIDATIONS();

                validationParams = commonFuncObj.GetTEOValidationParams(0, parentId);


                ACC_TEO_SCREEN_DESIGN_PARAM_DETAILS designParams = new ACC_TEO_SCREEN_DESIGN_PARAM_DETAILS();
                designParams = commonFuncObj.getTEODesignParamDetails(0, parentId);

                if (designParams == null)
                {
                    designParams = teoDAL.setDefualtDesignTransParam();
                }

                objMaster.BILL_TYPE = "J";
                objMaster.FUND_TYPE = PMGSYSession.Current.FundType;
                objMaster.LVL_ID = PMGSYSession.Current.LevelId;
                objMaster.TXN_ID = parentId;
                objMaster.BILL_ID = teoDetailsModel.BILL_ID;
                objMaster.STATE_CODE = PMGSYSession.Current.StateCode;

                objMaster.CREDIT_DEBIT = "C";

                if (designParams.CON_REQ == "Y")
                {
                    objMaster.MAST_CON_SUP_FLAG = "C";
                }
                else if (designParams.SUPP_REQ == "Y")
                {
                    objMaster.MAST_CON_SUP_FLAG = "S";
                }

                if (designParams.DISTRICT_REQ == "Y")
                {
                    if (validationParams.IS_DISTRICT_REPEAT == "Y" && acc_bill_details_trans != null)
                    {
                        ViewBag.ddlDistrict = commonFuncObj.PopulateDistrictforTOB(objMaster.STATE_CODE, false, Convert.ToInt32(acc_bill_details_trans.MAST_DISTRICT_CODE));
                    }
                    else
                    {
                        ViewBag.ddlDistrict = commonFuncObj.PopulateDistrictforTOB(objMaster.STATE_CODE);
                    }
                }
                else
                {
                    ViewBag.ddlDistrict = new SelectList(new List<SelectListItem>().AsEnumerable<SelectListItem>(), "Value", "Text");
                }
                if (designParams.DPIU_REQ == "Y")
                {
                    if (validationParams.IS_DPIU_REPEAT == "Y" && acc_bill_details_trans != null)
                    {
                        Int32 tmpDistrict = objMaster.DISTRICT_CODE;
                        Int32 tmpAdminCode = objMaster.ADMIN_ND_CODE;
                        if (validationParams.IS_DPIU_REPEAT == "Y" && acc_bill_details_trans != null)
                        {
                            objMaster.DISTRICT_CODE = PMGSYSession.Current.LevelId == 5 ? PMGSYSession.Current.DistrictCode : Convert.ToInt32(acc_bill_details_trans.MAST_DISTRICT_CODE);
                            objMaster.ADMIN_ND_CODE = Convert.ToInt32(acc_bill_details_trans.ADMIN_ND_CODE);
                            ViewBag.ddlDPIU = commonFuncObj.PopulateDPIU(objMaster);
                        }
                        else
                        {
                            objMaster.DISTRICT_CODE = PMGSYSession.Current.LevelId == 5 ? PMGSYSession.Current.DistrictCode : Convert.ToInt32(acc_bill_details_trans.MAST_DISTRICT_CODE);
                            ViewBag.ddlDPIU = commonFuncObj.PopulateDPIU(objMaster);

                        }
                        objMaster.ADMIN_ND_CODE = tmpAdminCode;
                        objMaster.DISTRICT_CODE = tmpDistrict;
                    }
                    else
                    {
                        ViewBag.ddlDPIU = commonFuncObj.PopulateDPIU(objMaster);
                    }
                }
                else
                {
                    ViewBag.ddlDPIU = new SelectList(new List<SelectListItem>().AsEnumerable<SelectListItem>(), "Value", "Text");
                }

                if (designParams.CON_REQ == "Y" || designParams.SUPP_REQ == "Y")
                {
                    if (acc_bill_details_trans != null)
                    {
                        objMaster.MAST_CONT_ID = Convert.ToInt32(acc_bill_details_trans.MAST_CON_ID);
                        ViewBag.ddlContractor = commonFuncObj.PopulateContractorSupplier(objMaster);
                        objMaster.MAST_CONT_ID = 0;
                    }
                    else
                    {
                        ViewBag.ddlContractor = commonFuncObj.PopulateContractorSupplier(objMaster);
                    }
                }
                else
                {
                    ViewBag.ddlContractor = new SelectList(new List<SelectListItem>().AsEnumerable<SelectListItem>(), "Value", "Text");
                }

                if (designParams.AFREEMENT_REQ == "Y")
                {
                    if (acc_bill_details_trans != null)
                    {
                        Int32 tmpAggrementCode = objMaster.AGREEMENT_CODE;
                        Int32 tmpDistrict = objMaster.DISTRICT_CODE;
                        Int32 tmpContID = objMaster.MAST_CONT_ID;
                        objMaster.DISTRICT_CODE = PMGSYSession.Current.LevelId == 5 ? PMGSYSession.Current.DistrictCode : Convert.ToInt32(acc_bill_details_trans.MAST_DISTRICT_CODE);
                        objMaster.MAST_CONT_ID = Convert.ToInt32(acc_bill_details_trans.MAST_CON_ID);
                        objMaster.AGREEMENT_CODE = Convert.ToInt32(acc_bill_details_trans.IMS_AGREEMENT_CODE);
                        ViewBag.ddlAgreement = commonFuncObj.PopulateAgreement(objMaster);
                        objMaster.AGREEMENT_CODE = tmpAggrementCode;
                        objMaster.MAST_CONT_ID = tmpContID;
                        objMaster.DISTRICT_CODE = tmpDistrict;
                    }
                    //commented by koustubh Nakate on 05/09/2013 for agreement not populated when modelstate not valid
                    //else if (teoDetailsModel.TXN_NO == 0)
                    //{
                    //    List<SelectListItem> lstOneItem = new List<SelectListItem>();
                    //    lstOneItem.Insert(0, (new SelectListItem { Text = "Select Agreement", Value = "0", Selected = true }));
                    //    ViewBag.ddlAgreement = lstOneItem;
                    //}
                    else
                    {
                        ViewBag.ddlAgreement = commonFuncObj.PopulateAgreement(objMaster);
                    }
                }
                else
                {
                    ViewBag.ddlAgreement = new SelectList(new List<SelectListItem>().AsEnumerable<SelectListItem>(), "Value", "Text");
                }

                if (designParams.SAN_REQ == "Y")
                {
                    ViewBag.ddlSancYear = commonFuncObj.PopulateSancYear(objMaster);
                }
                else
                {
                    ViewBag.ddlSancYear = new SelectList(new List<SelectListItem>().AsEnumerable<SelectListItem>(), "Value", "Text");
                }

                //designParams.ROAD_REQ condition necessary bcoz it will populate unnecessarily 
                if (designParams.ROAD_REQ == "Y" && validationParams.IS_ROAD_REPEAT == "Y" && acc_bill_details_trans != null)
                {
                    Int32 tmpAggCode = objMaster.AGREEMENT_CODE;
                    objMaster.AGREEMENT_CODE = Convert.ToInt32(acc_bill_details_trans.IMS_AGREEMENT_CODE);
                    objMaster.ROAD_CODE = Convert.ToInt32(acc_bill_details_trans.IMS_PR_ROAD_CODE);
                    ViewBag.ddlRoad = commonFuncObj.PopulateRoad(objMaster);
                    objMaster.AGREEMENT_CODE = tmpAggCode;
                }
                else if (validationParams.IS_AGREEMENT_REPEAT == "Y" && acc_bill_details_trans != null && designParams.AFREEMENT_REQ == "Y")
                {
                    Int32 tmpAggCode = objMaster.AGREEMENT_CODE;
                    objMaster.AGREEMENT_CODE = Convert.ToInt32(acc_bill_details_trans.IMS_AGREEMENT_CODE);
                    objMaster.ROAD_CODE = 0;
                    ViewBag.ddlRoad = commonFuncObj.PopulateRoad(objMaster);
                    objMaster.AGREEMENT_CODE = tmpAggCode;
                }
                else if (designParams.ROAD_REQ == "Y" && acc_bill_details_trans != null)
                {
                    Int32 tmpAggCode = objMaster.AGREEMENT_CODE;
                    objMaster.AGREEMENT_CODE = Convert.ToInt32(acc_bill_details_trans.IMS_AGREEMENT_CODE);
                    objMaster.ROAD_CODE = 0;
                    ViewBag.ddlRoad = commonFuncObj.PopulateRoad(objMaster);
                    objMaster.AGREEMENT_CODE = tmpAggCode;
                }
                else
                {
                    //new change done by Vikram -- for showing the road dropdown when the entry is the first entry
                    if (teoDetailsModel.IMS_PR_ROAD_CODE != 0 && teoDetailsModel.IMS_PR_ROAD_CODE != null)
                    {
                        Int32 tmpAggCode = objMaster.AGREEMENT_CODE;
                        ViewBag.ddlRoad = commonFuncObj.PopulateRoad(objMaster);
                        objMaster.ROAD_CODE = 0;
                        ViewBag.ddlRoad = commonFuncObj.PopulateRoad(objMaster);
                    }
                    else
                    {
                        ViewBag.ddlRoad = new SelectList(new List<SelectListItem>().AsEnumerable<SelectListItem>(), "Value", "Text");
                    }
                }

                if (designParams.PKG_REQ == "Y")
                {
                    if (teoDetailsModel.TXN_NO == 0)
                    {
                        List<SelectListItem> lstOneItem = new List<SelectListItem>();
                        lstOneItem.Insert(0, (new SelectListItem { Text = "Select Package", Value = "0", Selected = true }));
                        ViewBag.ddlPackage = lstOneItem;
                    }
                    else
                    {
                        ViewBag.ddlPackage = commonFuncObj.PopulatePackage(objMaster);
                    }
                }
                else
                {
                    ViewBag.ddlPackage = new SelectList(new List<SelectListItem>().AsEnumerable<SelectListItem>(), "Value", "Text");
                }
                if (validationParams.IS_HEAD_REPEAT == "Y" && acc_bill_details_trans != null)
                {
                    objMaster.HEAD_ID = acc_bill_details_trans.HEAD_ID;
                    ViewBag.ddlHead = commonFuncObj.PopulateHead(objMaster);
                    objMaster.HEAD_ID = teoDetailsModel.HEAD_ID;
                }
                else
                {
                    ViewBag.ddlHead = commonFuncObj.PopulateHead(objMaster);
                }

                ACC_TEO_SCREEN_DESIGN_HEADWISE_DETAILS headParams = new ACC_TEO_SCREEN_DESIGN_HEADWISE_DETAILS();
                if (acc_bill_details_trans != null)
                {
                    objMaster.HEAD_ID = acc_bill_details_trans.HEAD_ID;
                }
                headParams = commonFuncObj.GetHeadwiseDesignParams(objMaster);
                objMaster.HEAD_ID = teoDetailsModel.HEAD_ID;
                if (headParams == null)
                {
                    headParams = teoDAL.setDefualtDesignHeadParam();
                }

                if (acc_bill_details_trans != null && headParams.CON_REQ == "Y")
                {
                    objMaster.MAST_CONT_ID = Convert.ToInt32(acc_bill_details_trans.MAST_CON_ID);
                    ViewBag.ddlHeadContractor = commonFuncObj.PopulateContractorSupplier(objMaster);
                    objMaster.MAST_CONT_ID = 0;
                }
                else
                {
                    ViewBag.ddlHeadContractor = new SelectList(new List<SelectListItem>().AsEnumerable<SelectListItem>(), "Value", "Text");
                }

                if (acc_bill_details_trans != null && headParams.AGREEMENT_REQ == "Y")
                {
                    Int32 tmpAggrementCode = objMaster.AGREEMENT_CODE;
                    Int32 tmpDistrict = objMaster.DISTRICT_CODE;
                    Int32 tmpContID = objMaster.MAST_CONT_ID;

                    objMaster.DISTRICT_CODE = PMGSYSession.Current.LevelId == 5 ? PMGSYSession.Current.DistrictCode : Convert.ToInt32(acc_bill_details_trans.MAST_DISTRICT_CODE); ;
                    objMaster.MAST_CONT_ID = Convert.ToInt32(acc_bill_details_trans.MAST_CON_ID);
                    objMaster.AGREEMENT_CODE = Convert.ToInt32(acc_bill_details_trans.IMS_AGREEMENT_CODE);
                    //ViewBag.ddlHeadAgreement = commonFuncObj.PopulateAgreement(objMaster);
                    JavaScriptSerializer json_serializer = new JavaScriptSerializer();
                    string id = objMaster.DISTRICT_CODE + "$" + objMaster.AGREEMENT_CODE;
                    //List<SelectListItem> AgrWithNoOption = json_serializer.Deserialize<List<SelectListItem>>(json_serializer.Serialize(PopulateAgreementWithNoOption(id).Data));
                    ViewBag.ddlHeadAgreement = parentId == 3100 ? json_serializer.Deserialize<List<SelectListItem>>(json_serializer.Serialize(PopulateAgreementWithNoOption(id).Data)) : commonFuncObj.PopulateAgreement(objMaster);

                    objMaster.AGREEMENT_CODE = tmpAggrementCode;
                    objMaster.MAST_CONT_ID = tmpContID;
                    objMaster.DISTRICT_CODE = tmpDistrict;
                }
                else
                {
                    ViewBag.ddlHeadAgreement = new SelectList(new List<SelectListItem>().AsEnumerable<SelectListItem>(), "Value", "Text");
                }


                ViewBag.ddlHeadSancYear = new SelectList(new List<SelectListItem>().AsEnumerable<SelectListItem>(), "Value", "Text");

                ViewBag.ddlHeadPackage = new SelectList(new List<SelectListItem>().AsEnumerable<SelectListItem>(), "Value", "Text");

                if (validationParams.IS_ROAD_REPEAT == "Y" && acc_bill_details_trans != null && headParams.ROAD_REQ == "Y")
                {
                    Int32 tmpAggCode = objMaster.AGREEMENT_CODE;
                    objMaster.AGREEMENT_CODE = Convert.ToInt32(acc_bill_details_trans.IMS_AGREEMENT_CODE);
                    objMaster.ROAD_CODE = Convert.ToInt32(acc_bill_details_trans.IMS_PR_ROAD_CODE);
                    ViewBag.ddlHeadRoad = commonFuncObj.PopulateRoad(objMaster);
                    objMaster.AGREEMENT_CODE = tmpAggCode;
                }
                else if (headParams.ROAD_REQ == "Y" && validationParams.IS_AGREEMENT_REPEAT == "Y" && acc_bill_details_trans != null && headParams.AGREEMENT_REQ == "Y")
                {
                    Int32 tmpAggCode = objMaster.AGREEMENT_CODE;
                    objMaster.AGREEMENT_CODE = Convert.ToInt32(acc_bill_details_trans.IMS_AGREEMENT_CODE);
                    objMaster.ROAD_CODE = 0;
                    ViewBag.ddlHeadRoad = commonFuncObj.PopulateRoad(objMaster);
                    objMaster.AGREEMENT_CODE = tmpAggCode;
                }
                else
                {
                    ViewBag.ddlHeadRoad = new SelectList(new List<SelectListItem>().AsEnumerable<SelectListItem>(), "Value", "Text");
                }


                if (acc_bill_details_trans != null && validationParams.IS_HEAD_REPEAT == "Y")
                {
                    teoDetailsModel.HEAD_ID = acc_bill_details_trans.HEAD_ID;
                }

            }
            ViewBag.CreditDebit = "C";
            return View("TEOAddDetails", teoDetailsModel);

        }


        public int? ValidateHead55ForTEO(long BIllID, short detailsTxnID, string CreaditDebit)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            try
            {
                ACC_BILL_MASTER billMaster = dbContext.ACC_BILL_MASTER.Where(m => m.BILL_ID == BIllID).FirstOrDefault();
                if (billMaster.TXN_ID == 479)
                {
                    return dbContext.USP_ACC_VERIFY_ADMIN_FUND_HEAD_55("J", billMaster.TXN_ID, detailsTxnID, CreaditDebit, billMaster.BILL_DATE).FirstOrDefault();
                }
                else
                {
                    return 1;
                }
            }
            catch (Exception)
            {
                return 0;
            }
            finally
            {
                if (dbContext != null)
                {
                    dbContext.Dispose();
                }

            }
        }

        [HttpPost]
        [Audit]
        [ValidateAntiForgeryToken]
        public ActionResult EditCreditTEODetails(TeoDetailsModel teoDetailsModel, String parameter, String hash, String key)
        {
            teoBAL = new TransferEntryOrderBAL();
            String ValidationSummary = String.Empty;
            List<ModelErrorList> lstModelErrorList = new List<ModelErrorList>();
            CommonFunctions commomFuncObj = new CommonFunctions();
            string[] strParameters = URLEncrypt.DecryptParameters(new string[] { parameter, hash, key });
            teoDetailsModel.BILL_ID = Convert.ToInt64(strParameters[0].Split('$')[0]);
            teoDetailsModel.TXN_NO = Convert.ToInt16(strParameters[0].Split('$')[1]);

            ViewBag.BILL_ID = URLEncrypt.EncryptParameters(new string[] { teoDetailsModel.BILL_ID.ToString().Trim() });
            ViewBag.TransId = URLEncrypt.EncryptParameters(new string[] { teoDetailsModel.BILL_ID.ToString().Trim() + "$" + teoDetailsModel.TXN_NO.ToString().Trim() }); ;
            ViewBag.CreditDebit = "C";

            lstModelErrorList = teoBAL.GetAddTEODetailsModelErrors(teoDetailsModel, "C");
            if (lstModelErrorList.Count != 0)
            {
                foreach (ModelErrorList item in lstModelErrorList)
                {
                    ModelState.AddModelError(item.Key, item.Value);
                }

            }

            //new change done By Vikram 

            //if (teoDetailsModel.HEAD_ID == 29 || teoDetailsModel.HEAD_ID == 28)
            if (teoDetailsModel.HEAD_ID == 29 || teoDetailsModel.HEAD_ID == 28 || teoDetailsModel.HEAD_ID == 427 || teoDetailsModel.HEAD_ID == 428)
            ///Changed by SAMMED A. PATIL on 20FEB2018 for RCPLWE connectivity check as per account head 
            {
                if (ValidateRoad(teoDetailsModel.HEAD_ID, teoDetailsModel.IMS_PR_ROAD_CODE.Value, PMGSYSession.Current.FundType) == false)
                {
                    ModelState.AddModelError("IMS_PR_ROAD_CODE", "The selected head is not valid for this road.");
                    return Json(new { success = false, status = "-555", message = "Select Upgrade/New road as per account head." });
                }
            }

            //end of change

            //New validation to validate PMGSY scheme Roads based on Head. start 2 Sep 2014
            if (PMGSYSession.Current.FundType == "P" && teoDetailsModel.IMS_PR_ROAD_CODE != null && teoDetailsModel.IMS_PR_ROAD_CODE != 0)
            {
                if (!ValidateRoadForPMGSYScheme(teoDetailsModel.HEAD_ID, teoDetailsModel.IMS_PR_ROAD_CODE.Value))
                {
                    return Json(new { success = false, message = "Road can not be Selected, Since it is in different Scheme." });
                }
            }
            //New validation to validate PMGSY scheme Roads based on Head. end


            if (ModelState.IsValid)
            {
                //ValidationSummary = receiptBAL.ValidateAddReceiptDetails(billDetailsViewModel);
                if (ValidationSummary == String.Empty)
                {

                    //int status=ValidateHead55(teoDetailsModel.ACC_BILL_MASTER.DA                     


                    //**Commented to allow head 55 entry, as per mail received on march 23 2017 by arun with subject -State Administrative Expenses Head to be unlocked in OMMAS 
                    //if (PMGSYSession.Current.LevelId == 5 && PMGSYSession.Current.FundType == "A")
                    //{
                    //    int? status = ValidateHead55ForTEO(teoDetailsModel.BILL_ID, teoDetailsModel.HEAD_ID, "C");
                    //    if (status == 0)
                    //    {
                    //        return this.Json(new { success = false, message = "Entry is not allowed since Head 55 is valid upto March-2014." });
                    //    }
                    //}
                    //**Commented to allow head 55 entry, as per mail received on march 23 2017 by arun with subject -State Administrative Expenses Head to be unlocked in OMMAS 


                    //Validate head 55 added By Abhishek kamble 27-Aug-2014 end

                    // to check if correct entry as is operational and is required after porting flag
                    string correctionStatus = commomFuncObj.ValidateHeadForCorrection(Convert.ToInt16(teoDetailsModel.TXN_ID), 0, "E");
                    if (correctionStatus == "0")
                    {
                        return this.Json(new { success = false, message = "Invalid Transaction Type.Please delete this entry and make new entry." });
                    }

                    string success = teoBAL.EditCreditTEODetails(teoDetailsModel);

                    if (success == String.Empty)
                    {
                        return this.Json(new { success = true, message = URLEncrypt.EncryptParameters(new string[] { teoDetailsModel.BILL_ID.ToString().Trim() }) });
                    }
                    else
                    {
                        return this.Json(new { success = false, message = URLEncrypt.EncryptParameters(new string[] { teoDetailsModel.BILL_ID.ToString().Trim() }) });
                    }
                }
                else
                {
                    return this.Json(new { success = false, message = ValidationSummary });
                }

            }
            else
            {
                ViewBag.TEODetailTitle = "Credit Details";
                ViewBag.SaveTEODetails = "btnSaveTEOCreditDetails";
                ViewBag.EditTEODetails = "btnEditTEOCreditDetails";
                ViewBag.CancelTEODetails = "btnCancelTEOCreditDetails";
                TransactionParams objMaster = new TransactionParams();
                TransferEntryOrderDAL teoDAL = new TransferEntryOrderDAL();
                teoBAL = new TransferEntryOrderBAL();
                commonFuncObj = new CommonFunctions();
                TeoDetailsModel oldTeoDetailsModel = new TeoDetailsModel();
                oldTeoDetailsModel = teoBAL.GetTEODetailByTransNo(teoDetailsModel.BILL_ID, teoDetailsModel.TXN_NO);

                objMaster.ADMIN_ND_CODE = oldTeoDetailsModel.ADMIN_ND_CODE == null ? PMGSYSession.Current.AdminNdCode : Convert.ToInt32(oldTeoDetailsModel.ADMIN_ND_CODE);
                objMaster.MAST_CONT_ID = oldTeoDetailsModel.MAST_CON_ID == null ? 0 : Convert.ToInt32(teoDetailsModel.MAST_CON_ID);
                objMaster.DISTRICT_CODE = oldTeoDetailsModel.MAST_DISTRICT_CODE == null ? PMGSYSession.Current.DistrictCode : Convert.ToInt32(oldTeoDetailsModel.MAST_DISTRICT_CODE);
                objMaster.AGREEMENT_CODE = oldTeoDetailsModel.IMS_AGREEMENT_CODE == null ? 0 : Convert.ToInt32(oldTeoDetailsModel.IMS_AGREEMENT_CODE);
                objMaster.SANC_YEAR = oldTeoDetailsModel.SANC_YEAR == null ? (Int16)0 : Convert.ToInt16(oldTeoDetailsModel.SANC_YEAR);
                objMaster.PACKAGE_ID = oldTeoDetailsModel.IMS_PACKAGE_ID == null ? "0" : oldTeoDetailsModel.IMS_PACKAGE_ID;
                objMaster.HEAD_ID = oldTeoDetailsModel.HEAD_ID;

                if (objMaster.SANC_YEAR != 0)
                {
                    objMaster.SANC_YEAR = commonFuncObj.getSancYearFromRoad(Convert.ToInt32(oldTeoDetailsModel.IMS_PR_ROAD_CODE));
                    objMaster.PACKAGE_ID = commonFuncObj.getPackageFromRoad(Convert.ToInt32(oldTeoDetailsModel.IMS_PR_ROAD_CODE));
                }


                //Added by Abhishek kamble to get Agreement No to populate road for MF start
                if (teoDetailsModel.IMS_AGREEMENT_CODE != null && PMGSYSession.Current.FundType == "M")
                {
                    objMaster.AGREEMENT_NUMBER = teoBAL.GetAgreementNumberForMF(teoDetailsModel.IMS_AGREEMENT_CODE);
                }
                //Added by Abhishek kamble to get Agreement No to populate road for MF end

                ACC_TEO_SCREEN_DESIGN_PARAM_DETAILS designParams = new ACC_TEO_SCREEN_DESIGN_PARAM_DETAILS();
                Int16 parentId = teoBAL.GetMasterTransId(oldTeoDetailsModel.BILL_ID);
                designParams = commonFuncObj.getTEODesignParamDetails(0, parentId);
                if (designParams == null)
                {
                    designParams = teoDAL.setDefualtDesignTransParam();
                }
                objMaster.BILL_TYPE = "J";
                objMaster.FUND_TYPE = PMGSYSession.Current.FundType;
                objMaster.LVL_ID = PMGSYSession.Current.LevelId;
                objMaster.TXN_ID = parentId;
                objMaster.BILL_ID = oldTeoDetailsModel.BILL_ID;
                objMaster.STATE_CODE = PMGSYSession.Current.StateCode;

                objMaster.CREDIT_DEBIT = "C";
                List<SelectListItem> lstOneItem = new List<SelectListItem>();

                if (designParams.CON_REQ == "Y")
                {
                    objMaster.MAST_CON_SUP_FLAG = "C";
                }
                else if (designParams.SUPP_REQ == "Y")
                {
                    objMaster.MAST_CON_SUP_FLAG = "S";
                }

                if (designParams.DISTRICT_REQ == "Y")
                {
                    ViewBag.ddlDistrict = commonFuncObj.PopulateDistrictforTOB(objMaster.STATE_CODE);
                }
                else
                {
                    ViewBag.ddlDistrict = new SelectList(new List<SelectListItem>().AsEnumerable<SelectListItem>(), "Value", "Text");
                }
                if (designParams.DPIU_REQ == "Y")
                {
                    ViewBag.ddlDPIU = commonFuncObj.PopulateDPIU(objMaster);
                }
                else
                {
                    ViewBag.ddlDPIU = new SelectList(new List<SelectListItem>().AsEnumerable<SelectListItem>(), "Value", "Text");
                }

                if (designParams.CON_REQ == "Y" || designParams.SUPP_REQ == "Y")
                {
                    ViewBag.ddlContractor = commonFuncObj.PopulateContractorSupplier(objMaster);
                }
                else
                {
                    ViewBag.ddlContractor = new SelectList(new List<SelectListItem>().AsEnumerable<SelectListItem>(), "Value", "Text");
                }

                if (designParams.AFREEMENT_REQ == "Y")
                {
                    ViewBag.ddlAgreement = commonFuncObj.PopulateAgreement(objMaster);
                }
                else
                {
                    ViewBag.ddlAgreement = new SelectList(new List<SelectListItem>().AsEnumerable<SelectListItem>(), "Value", "Text");
                }

                if (designParams.SAN_REQ == "Y")
                {
                    ViewBag.ddlSancYear = commonFuncObj.PopulateSancYear(objMaster);
                }
                else
                {
                    ViewBag.ddlSancYear = new SelectList(new List<SelectListItem>().AsEnumerable<SelectListItem>(), "Value", "Text");
                }

                if (designParams.ROAD_REQ == "Y")
                {
                    ViewBag.ddlRoad = commonFuncObj.PopulateRoad(objMaster);
                }
                else
                {
                    ViewBag.ddlRoad = new SelectList(new List<SelectListItem>().AsEnumerable<SelectListItem>(), "Value", "Text");
                }

                if (designParams.PKG_REQ == "Y")
                {
                    ViewBag.ddlPackage = commonFuncObj.PopulatePackage(objMaster);
                }
                else
                {
                    ViewBag.ddlPackage = new SelectList(new List<SelectListItem>().AsEnumerable<SelectListItem>(), "Value", "Text");
                }

                ViewBag.ddlHead = commonFuncObj.PopulateHead(objMaster);
                if (teoDetailsModel.HEAD_ID != 0)
                {
                    ACC_TEO_SCREEN_DESIGN_HEADWISE_DETAILS headParams = new ACC_TEO_SCREEN_DESIGN_HEADWISE_DETAILS();
                    headParams = commonFuncObj.GetHeadwiseDesignParams(objMaster);
                    if (headParams == null)
                    {
                        headParams = teoDAL.setDefualtDesignHeadParam();
                    }
                    if (headParams.CON_REQ == "Y")
                    {
                        ViewBag.ddlHeadContractor = commonFuncObj.PopulateContractorSupplier(objMaster);
                    }
                    else
                    {
                        ViewBag.ddlHeadContractor = new SelectList(new List<SelectListItem>().AsEnumerable<SelectListItem>(), "Value", "Text");
                    }
                    if (headParams.AGREEMENT_REQ == "Y")
                    {
                        ViewBag.ddlHeadAgreement = commonFuncObj.PopulateAgreement(objMaster);
                    }
                    else
                    {
                        ViewBag.ddlHeadAgreement = new SelectList(new List<SelectListItem>().AsEnumerable<SelectListItem>(), "Value", "Text");
                    }
                    if (headParams.SANC_YEAR_REQ == "Y")
                    {
                        ViewBag.ddlHeadSancYear = commonFuncObj.PopulateSancYear(objMaster);
                    }
                    else
                    {
                        ViewBag.ddlHeadSancYear = new SelectList(new List<SelectListItem>().AsEnumerable<SelectListItem>(), "Value", "Text");
                    }
                    if (headParams.ROAD_REQ == "Y")
                    {
                        ViewBag.ddlHeadRoad = commonFuncObj.PopulateRoad(objMaster);
                    }
                    else
                    {
                        ViewBag.ddlHeadRoad = new SelectList(new List<SelectListItem>().AsEnumerable<SelectListItem>(), "Value", "Text");
                    }
                    if (headParams.PKG_REQ == "Y")
                    {
                        ViewBag.ddlHeadPackage = commonFuncObj.PopulatePackage(objMaster);
                    }
                    else
                    {
                        ViewBag.ddlHeadPackage = new SelectList(new List<SelectListItem>().AsEnumerable<SelectListItem>(), "Value", "Text");
                    }
                }
                else
                {
                    ViewBag.ddlHeadContractor = new SelectList(new List<SelectListItem>().AsEnumerable<SelectListItem>(), "Value", "Text");
                    ViewBag.ddlHeadAgreement = new SelectList(new List<SelectListItem>().AsEnumerable<SelectListItem>(), "Value", "Text");
                    ViewBag.ddlHeadSancYear = new SelectList(new List<SelectListItem>().AsEnumerable<SelectListItem>(), "Value", "Text");
                    ViewBag.ddlHeadRoad = new SelectList(new List<SelectListItem>().AsEnumerable<SelectListItem>(), "Value", "Text");
                    ViewBag.ddlHeadPackage = new SelectList(new List<SelectListItem>().AsEnumerable<SelectListItem>(), "Value", "Text");
                }
            }
            return View("TEOAddDetails", teoDetailsModel);
        }

        [HttpPost]
        [Audit]
        [ValidateAntiForgeryToken]
        public ActionResult EditDebitTEODetails(TeoDetailsModel teoDetailsModel, String parameter, String hash, String key)
        {
            teoBAL = new TransferEntryOrderBAL();
            List<ModelErrorList> lstModelErrorList = new List<ModelErrorList>();
            String ValidationSummary = String.Empty;
            CommonFunctions commomFuncObj = new CommonFunctions();
            string[] strParameters = URLEncrypt.DecryptParameters(new string[] { parameter, hash, key });
            teoDetailsModel.BILL_ID = Convert.ToInt64(strParameters[0].Split('$')[0]);
            teoDetailsModel.TXN_NO = Convert.ToInt16(strParameters[0].Split('$')[1]);

            ViewBag.BILL_ID = URLEncrypt.EncryptParameters(new string[] { teoDetailsModel.BILL_ID.ToString().Trim() });
            ViewBag.TransId = URLEncrypt.EncryptParameters(new string[] { teoDetailsModel.BILL_ID.ToString().Trim() + "$" + teoDetailsModel.TXN_NO.ToString().Trim() }); ;
            ViewBag.CreditDebit = "D";

            //added by Koustubh Nakate on 30/09/2013 
            if (teoDetailsModel.IMS_PR_ROAD_CODE_Head != 0 && teoDetailsModel.IMS_PR_ROAD_CODE_Head != null && (teoDetailsModel.IMS_PR_ROAD_CODE == 0 || teoDetailsModel.IMS_PR_ROAD_CODE == null))
            {
                teoDetailsModel.IMS_PR_ROAD_CODE = teoDetailsModel.IMS_PR_ROAD_CODE_Head;
            }

            //new change done By Vikram 

            //if (teoDetailsModel.HEAD_ID == 29 || teoDetailsModel.HEAD_ID == 28)
            if (teoDetailsModel.HEAD_ID == 29 || teoDetailsModel.HEAD_ID == 28 || teoDetailsModel.HEAD_ID == 427 || teoDetailsModel.HEAD_ID == 428)
            ///Changed by SAMMED A. PATIL on 20FEB2018 for RCPLWE connectivity check as per account head 
            {
                if (teoDetailsModel.IMS_PR_ROAD_CODE.HasValue)
                {
                    if (ValidateRoad(teoDetailsModel.HEAD_ID, teoDetailsModel.IMS_PR_ROAD_CODE.Value, PMGSYSession.Current.FundType) == false)
                    {
                        ModelState.AddModelError("IMS_PR_ROAD_CODE", "The selected head is not valid for this road.");
                        return Json(new { success = false, status = "-555", message = "Select Upgrade/New road as per account head." });
                    }
                }
            }

            //end of change

            //New validation to validate PMGSY scheme Roads based on Head. start 2 Sep 2014
            if (PMGSYSession.Current.FundType == "P" && teoDetailsModel.IMS_PR_ROAD_CODE != null && teoDetailsModel.IMS_PR_ROAD_CODE != 0)
            {
                if (!ValidateRoadForPMGSYScheme(teoDetailsModel.HEAD_ID, teoDetailsModel.IMS_PR_ROAD_CODE.Value))
                {
                    return Json(new { success = false, message = "Road can not be Selected, Since it is in different Scheme." });
                }
            }
            //New validation to validate PMGSY scheme Roads based on Head. end

            lstModelErrorList = teoBAL.GetAddTEODetailsModelErrors(teoDetailsModel, "D");
            if (lstModelErrorList.Count != 0)
            {
                foreach (ModelErrorList item in lstModelErrorList)
                {
                    ModelState.AddModelError(item.Key, item.Value);
                }

            }
            if (ModelState.IsValid)
            {
                //ValidationSummary = receiptBAL.ValidateAddReceiptDetails(billDetailsViewModel);
                if (ValidationSummary == String.Empty)
                {

                    //**Commented to allow head 55 entry, as per mail received on march 23 2017 by arun with subject -State Administrative Expenses Head to be unlocked in OMMAS 

                    //if (PMGSYSession.Current.LevelId == 5 && PMGSYSession.Current.FundType == "A")
                    //{
                    //   // int? status = ValidateHead55ForTEO(teoDetailsModel.BILL_ID, teoDetailsModel.HEAD_ID, "D");
                    //    int? status = 1;
                    //    if (status == 0)
                    //    {
                    //        return this.Json(new { success = false, message = "Entry is not allowed since Head 55 is valid upto March-2014." });
                    //    }
                    //}
                    //**Commented to allow head 55 entry


                    //Validate head 55 added By Abhishek kamble 27-Aug-2014 end
                    // to check if correct entry as is operational and is required after porting flag
                    string correctionStatus = commomFuncObj.ValidateHeadForCorrection(Convert.ToInt16(teoDetailsModel.TXN_ID), 0, "E");
                    if (correctionStatus == "0")
                    {
                        return this.Json(new { success = false, message = "Invalid Transaction Type.Please delete this entry and make new entry." });
                    }


                    string success = teoBAL.EditDebitTEODetails(teoDetailsModel);

                    if (success == String.Empty)
                    {
                        return this.Json(new { success = true, message = URLEncrypt.EncryptParameters(new string[] { teoDetailsModel.BILL_ID.ToString().Trim() }) });
                    }
                    else
                    {
                        return this.Json(new { success = false, message = URLEncrypt.EncryptParameters(new string[] { teoDetailsModel.BILL_ID.ToString().Trim() }) });
                    }
                }
                else
                {
                    return this.Json(new { success = false, message = ValidationSummary });
                }
            }
            else
            {
                ViewBag.TEODetailTitle = "Debit Details";
                ViewBag.SaveTEODetails = "btnSaveTEODebitDetails";
                ViewBag.EditTEODetails = "btnEditTEODebitDetails";
                ViewBag.CancelTEODetails = "btnCancelTEODebitDetails";
                TransactionParams objMaster = new TransactionParams();
                TransferEntryOrderDAL teoDAL = new TransferEntryOrderDAL();
                teoBAL = new TransferEntryOrderBAL();
                commonFuncObj = new CommonFunctions();
                TeoDetailsModel oldTeoDetailsModel = new TeoDetailsModel();
                oldTeoDetailsModel = teoBAL.GetTEODetailByTransNo(teoDetailsModel.BILL_ID, teoDetailsModel.TXN_NO);

                objMaster.ADMIN_ND_CODE = oldTeoDetailsModel.ADMIN_ND_CODE == null ? PMGSYSession.Current.AdminNdCode : Convert.ToInt32(oldTeoDetailsModel.ADMIN_ND_CODE);
                objMaster.MAST_CONT_ID = oldTeoDetailsModel.MAST_CON_ID == null ? 0 : Convert.ToInt32(teoDetailsModel.MAST_CON_ID);
                objMaster.DISTRICT_CODE = oldTeoDetailsModel.MAST_DISTRICT_CODE == null ? PMGSYSession.Current.DistrictCode : Convert.ToInt32(oldTeoDetailsModel.MAST_DISTRICT_CODE);
                objMaster.AGREEMENT_CODE = oldTeoDetailsModel.IMS_AGREEMENT_CODE == null ? 0 : Convert.ToInt32(oldTeoDetailsModel.IMS_AGREEMENT_CODE);
                objMaster.SANC_YEAR = oldTeoDetailsModel.SANC_YEAR == null ? (Int16)0 : Convert.ToInt16(oldTeoDetailsModel.SANC_YEAR);
                objMaster.PACKAGE_ID = oldTeoDetailsModel.IMS_PACKAGE_ID == null ? "0" : oldTeoDetailsModel.IMS_PACKAGE_ID;
                objMaster.HEAD_ID = oldTeoDetailsModel.HEAD_ID;

                if (objMaster.SANC_YEAR != 0)
                {
                    objMaster.SANC_YEAR = commonFuncObj.getSancYearFromRoad(Convert.ToInt32(oldTeoDetailsModel.IMS_PR_ROAD_CODE));
                    objMaster.PACKAGE_ID = commonFuncObj.getPackageFromRoad(Convert.ToInt32(oldTeoDetailsModel.IMS_PR_ROAD_CODE));
                }


                //Added by Abhishek kamble to get Agreement No to populate road for MF start
                if (teoDetailsModel.IMS_AGREEMENT_CODE != null && PMGSYSession.Current.FundType == "M")
                {
                    objMaster.AGREEMENT_NUMBER = teoBAL.GetAgreementNumberForMF(teoDetailsModel.IMS_AGREEMENT_CODE);
                }
                //Added by Abhishek kamble to get Agreement No to populate road for MF end

                ACC_TEO_SCREEN_DESIGN_PARAM_DETAILS designParams = new ACC_TEO_SCREEN_DESIGN_PARAM_DETAILS();
                Int16 parentId = teoBAL.GetMasterTransId(oldTeoDetailsModel.BILL_ID);
                designParams = commonFuncObj.getTEODesignParamDetails(0, parentId);
                if (designParams == null)
                {
                    designParams = teoDAL.setDefualtDesignTransParam();
                }
                objMaster.BILL_TYPE = "J";
                objMaster.FUND_TYPE = PMGSYSession.Current.FundType;
                objMaster.LVL_ID = PMGSYSession.Current.LevelId;
                objMaster.TXN_ID = parentId;
                objMaster.BILL_ID = oldTeoDetailsModel.BILL_ID;
                objMaster.STATE_CODE = PMGSYSession.Current.StateCode;

                objMaster.CREDIT_DEBIT = "D";
                List<SelectListItem> lstOneItem = new List<SelectListItem>();

                if (designParams.CON_REQ == "Y")
                {
                    objMaster.MAST_CON_SUP_FLAG = "C";
                }
                else if (designParams.SUPP_REQ == "Y")
                {
                    objMaster.MAST_CON_SUP_FLAG = "S";
                }

                if (designParams.DISTRICT_REQ == "Y")
                {
                    ViewBag.ddlDistrict = commonFuncObj.PopulateDistrictforTOB(objMaster.STATE_CODE);
                }
                else
                {
                    ViewBag.ddlDistrict = new SelectList(new List<SelectListItem>().AsEnumerable<SelectListItem>(), "Value", "Text");
                }
                if (designParams.DPIU_REQ == "Y")
                {
                    ViewBag.ddlDPIU = commonFuncObj.PopulateDPIU(objMaster);
                }
                else
                {
                    ViewBag.ddlDPIU = new SelectList(new List<SelectListItem>().AsEnumerable<SelectListItem>(), "Value", "Text");
                }

                if (designParams.CON_REQ == "Y" || designParams.SUPP_REQ == "Y")
                {
                    ViewBag.ddlContractor = commonFuncObj.PopulateContractorSupplier(objMaster);
                }
                else
                {
                    ViewBag.ddlContractor = new SelectList(new List<SelectListItem>().AsEnumerable<SelectListItem>(), "Value", "Text");
                }

                if (designParams.AFREEMENT_REQ == "Y")
                {
                    ViewBag.ddlAgreement = commonFuncObj.PopulateAgreement(objMaster);
                }
                else
                {
                    ViewBag.ddlAgreement = new SelectList(new List<SelectListItem>().AsEnumerable<SelectListItem>(), "Value", "Text");
                }

                if (designParams.SAN_REQ == "Y")
                {
                    ViewBag.ddlSancYear = commonFuncObj.PopulateSancYear(objMaster);
                }
                else
                {
                    ViewBag.ddlSancYear = new SelectList(new List<SelectListItem>().AsEnumerable<SelectListItem>(), "Value", "Text");
                }

                if (designParams.ROAD_REQ == "Y")
                {
                    ViewBag.ddlRoad = commonFuncObj.PopulateRoad(objMaster);
                }
                else
                {
                    ViewBag.ddlRoad = new SelectList(new List<SelectListItem>().AsEnumerable<SelectListItem>(), "Value", "Text");
                }

                if (designParams.PKG_REQ == "Y")
                {
                    ViewBag.ddlPackage = commonFuncObj.PopulatePackage(objMaster);
                }
                else
                {
                    ViewBag.ddlPackage = new SelectList(new List<SelectListItem>().AsEnumerable<SelectListItem>(), "Value", "Text");
                }

                ViewBag.ddlHead = commonFuncObj.PopulateHead(objMaster);
                if (teoDetailsModel.HEAD_ID != 0)
                {
                    ACC_TEO_SCREEN_DESIGN_HEADWISE_DETAILS headParams = new ACC_TEO_SCREEN_DESIGN_HEADWISE_DETAILS();
                    headParams = commonFuncObj.GetHeadwiseDesignParams(objMaster);
                    if (headParams == null)
                    {
                        headParams = teoDAL.setDefualtDesignHeadParam();
                    }
                    if (headParams.CON_REQ == "Y")
                    {
                        ViewBag.ddlHeadContractor = commonFuncObj.PopulateContractorSupplier(objMaster);
                    }
                    else
                    {
                        ViewBag.ddlHeadContractor = new SelectList(new List<SelectListItem>().AsEnumerable<SelectListItem>(), "Value", "Text");
                    }
                    if (headParams.AGREEMENT_REQ == "Y")
                    {
                        ViewBag.ddlHeadAgreement = commonFuncObj.PopulateAgreement(objMaster);
                    }
                    else
                    {
                        ViewBag.ddlHeadAgreement = new SelectList(new List<SelectListItem>().AsEnumerable<SelectListItem>(), "Value", "Text");
                    }
                    if (headParams.SANC_YEAR_REQ == "Y")
                    {
                        ViewBag.ddlHeadSancYear = commonFuncObj.PopulateSancYear(objMaster);
                    }
                    else
                    {
                        ViewBag.ddlHeadSancYear = new SelectList(new List<SelectListItem>().AsEnumerable<SelectListItem>(), "Value", "Text");
                    }
                    if (headParams.ROAD_REQ == "Y")
                    {
                        ViewBag.ddlHeadRoad = commonFuncObj.PopulateRoad(objMaster);
                    }
                    else
                    {
                        ViewBag.ddlHeadRoad = new SelectList(new List<SelectListItem>().AsEnumerable<SelectListItem>(), "Value", "Text");
                    }
                    if (headParams.PKG_REQ == "Y")
                    {
                        ViewBag.ddlHeadPackage = commonFuncObj.PopulatePackage(objMaster);
                    }
                    else
                    {
                        ViewBag.ddlHeadPackage = new SelectList(new List<SelectListItem>().AsEnumerable<SelectListItem>(), "Value", "Text");
                    }
                }
                else
                {
                    ViewBag.ddlHeadContractor = new SelectList(new List<SelectListItem>().AsEnumerable<SelectListItem>(), "Value", "Text");
                    ViewBag.ddlHeadAgreement = new SelectList(new List<SelectListItem>().AsEnumerable<SelectListItem>(), "Value", "Text");
                    ViewBag.ddlHeadSancYear = new SelectList(new List<SelectListItem>().AsEnumerable<SelectListItem>(), "Value", "Text");
                    ViewBag.ddlHeadRoad = new SelectList(new List<SelectListItem>().AsEnumerable<SelectListItem>(), "Value", "Text");
                    ViewBag.ddlHeadPackage = new SelectList(new List<SelectListItem>().AsEnumerable<SelectListItem>(), "Value", "Text");
                }
            }
            return View("TEOAddDetails", teoDetailsModel);
        }

        [Audit]
        public ActionResult DeleteTEODetails(String parameter, String hash, String key)
        {
            teoBAL = new TransferEntryOrderBAL();
            string[] strParameters = URLEncrypt.DecryptParameters(new string[] { parameter, hash, key });
            string status = teoBAL.DeleteTEODetails(Convert.ToInt64(strParameters[0].Split('$')[0]), Convert.ToInt16(strParameters[0].Split('$')[1]));
            if (status == String.Empty)
            {
                return this.Json(new { success = true, message = String.Empty });
            }
            else
            {
                return this.Json(new { success = true, message = status });
            }
        }

        [Audit]
        public ActionResult FinalizeTEO(String parameter, String hash, String key)
        {
            teoBAL = new TransferEntryOrderBAL();
            string[] strParameters = URLEncrypt.DecryptParameters(new string[] { parameter, hash, key });
            CommonFunctions commomFuncObj = new CommonFunctions();
            // to check if correct entry as is operational and is required after porting flag
            string correctionStatus = commomFuncObj.ValidateHeadForCorrection(0, Convert.ToInt64(strParameters[0]), "F");
            if (correctionStatus == "0")
            {
                return this.Json(new { success = false, message = "Can not finalize the entry as it contains one or more invalid transaction type." });
            }

            string status = teoBAL.FinalizeTEO(Convert.ToInt64(strParameters[0]));
            if (status == String.Empty)
            {
                return this.Json(new { success = true, message = URLEncrypt.EncryptParameters(new string[] { strParameters[0] }) });
            }
            else
            {
                return this.Json(new { success = false, message = status });
            }
        }

        [HttpPost]
        [Audit]
        [ValidateAntiForgeryToken]
        public ActionResult AddDebitTEODetails(TeoDetailsModel teoDetailsModel, String parameter, String hash, String key)
        {
            TransferEntryOrderDAL objDAL = new TransferEntryOrderDAL();
            teoBAL = new TransferEntryOrderBAL();
            String ValidationSummary = String.Empty;
            Int16 transId = 0;
            CommonFunctions commomFuncObj = new CommonFunctions();
            string[] strParameters = URLEncrypt.DecryptParameters(new string[] { parameter, hash, key });


            //Below 2 lines are added on 06-10-2022
            teoDetailsModel.IMS_PR_ROAD_CODE_Head = teoDetailsModel.IMS_PR_ROAD_CODE_Head == -1 ? null : teoDetailsModel.IMS_PR_ROAD_CODE_Head;
            teoDetailsModel.IMS_AGREEMENT_CODE = teoDetailsModel.IMS_AGREEMENT_CODE == -1 ? null : teoDetailsModel.IMS_AGREEMENT_CODE;
            teoDetailsModel.MAST_CON_ID_TRANS = teoDetailsModel.MAST_CON_ID_TRANS == -1 ? null : teoDetailsModel.MAST_CON_ID_TRANS;


            if (strParameters.Length == 0)
            {
                return this.Json(new { success = false, message = "Error while Adding TEO Debit Details" });
            }
            else
            {
                teoDetailsModel.BILL_ID = Convert.ToInt64(strParameters[0]);
            }

            //Changed by SAMMED A. PATIL on 04DEC2018 to skip road validation for head 11.01 for Normal TEO's
            int txnId = objDAL.GetMasterTXNId(teoDetailsModel.BILL_ID);

            //new change done By Vikram 

            //if (teoDetailsModel.HEAD_ID == 29 || teoDetailsModel.HEAD_ID == 28)
            if (teoDetailsModel.HEAD_ID == 29 || (teoDetailsModel.HEAD_ID == 28 && txnId != 166) || teoDetailsModel.HEAD_ID == 427 || teoDetailsModel.HEAD_ID == 428)
            ///Changed by SAMMED A. PATIL on 20FEB2018 for RCPLWE connectivity check as per account head 
            {
                Int32 roadCode = 0;
                string errorKey = string.Empty;
                if (teoDetailsModel.IMS_PR_ROAD_CODE != 0 && teoDetailsModel.IMS_PR_ROAD_CODE != null)
                {
                    roadCode = teoDetailsModel.IMS_PR_ROAD_CODE.Value;
                    errorKey = "IMS_PR_ROAD_CODE";
                }
                else if (teoDetailsModel.IMS_PR_ROAD_CODE_Head != 0 && teoDetailsModel.IMS_PR_ROAD_CODE_Head != null)
                {
                    roadCode = teoDetailsModel.IMS_PR_ROAD_CODE_Head.Value;
                    errorKey = "IMS_PR_ROAD_CODE_Head";
                }

                if (ValidateRoad(teoDetailsModel.HEAD_ID, roadCode, PMGSYSession.Current.FundType) == false)
                {
                    ModelState.AddModelError(errorKey, "The selected head is not valid for this road.");
                    return Json(new { success = false, status = "-555", message = "Select Upgrade/New road as per account head." });
                }
            }

            //end of change

            //New validation to validate PMGSY scheme Roads based on Head. start 2 Sep 2014
            if (PMGSYSession.Current.FundType == "P" && (teoDetailsModel.IMS_PR_ROAD_CODE != null && teoDetailsModel.IMS_PR_ROAD_CODE != 0) || ((teoDetailsModel.IMS_PR_ROAD_CODE_Head != null && teoDetailsModel.IMS_PR_ROAD_CODE_Head != 0)))
            {
                int? RoadCode = (teoDetailsModel.IMS_PR_ROAD_CODE == null || teoDetailsModel.IMS_PR_ROAD_CODE == 0) ? teoDetailsModel.IMS_PR_ROAD_CODE_Head : teoDetailsModel.IMS_PR_ROAD_CODE;

                if (!ValidateRoadForPMGSYScheme(teoDetailsModel.HEAD_ID, RoadCode.Value))
                {
                    return Json(new { success = false, message = "Road can not be Selected, Since it is in different Scheme." });
                }
            }
            //New validation to validate PMGSY scheme Roads based on Head. end

            ///Commented code in below region by SAMMED A. PATIL on 23FEB2018 for change of sequence in controls issue
            #region
            ////added by Koustubh Nakate on 30/09/2013 
            //if (teoDetailsModel.IMS_PR_ROAD_CODE_Head != 0 && teoDetailsModel.IMS_PR_ROAD_CODE_Head != null && (teoDetailsModel.IMS_PR_ROAD_CODE == 0 || teoDetailsModel.IMS_PR_ROAD_CODE == null))
            //{
            //    teoDetailsModel.IMS_PR_ROAD_CODE = teoDetailsModel.IMS_PR_ROAD_CODE_Head;
            //}
            #endregion

            List<ModelErrorList> lstModelErrorList = new List<ModelErrorList>();
            lstModelErrorList = teoBAL.GetAddTEODetailsModelErrors(teoDetailsModel, "D");
            if (lstModelErrorList.Count != 0)
            {
                foreach (ModelErrorList item in lstModelErrorList)
                {
                    ModelState.AddModelError(item.Key, item.Value);
                }

            }

            if (ModelState.IsValid)
            {
                //ValidationSummary = receiptBAL.ValidateAddReceiptDetails(billDetailsViewModel);
                if (ValidationSummary == String.Empty)
                {

                    //int status=ValidateHead55(teoDetailsModel.ACC_BILL_MASTER.DA                     

                    ////**Commented to allow head 55 entry, as per mail received on march 23 2017 by arun with subject -State Administrative Expenses Head to be unlocked in OMMAS 

                    //if (PMGSYSession.Current.LevelId == 5 && PMGSYSession.Current.FundType == "A")
                    //{
                    //    int? status = ValidateHead55ForTEO(teoDetailsModel.BILL_ID, teoDetailsModel.HEAD_ID, "D");
                    //    if (status == 0)
                    //    {
                    //        return this.Json(new { success = false, message = "Entry is not allowed since Head 55 is valid upto March-2014." });
                    //    }
                    //}

                    ////**Commented to allow head 55 entry, as per mail received on march 23 2017 by arun with subject -State Administrative Expenses Head to be unlocked in OMMAS 

                    //Validate head 55 added By Abhishek kamble 27-Aug-2014 end

                    // to check if correct entry as is operational and is required after porting flag
                    string correctionStatus = commomFuncObj.ValidateHeadForCorrection(Convert.ToInt16(teoDetailsModel.TXN_ID), 0, "A");
                    if (correctionStatus == "0")
                    {
                        return this.Json(new { success = false, message = "Invalid Transaction Type.Please delete this entry and make new entry." });
                    }

                    string success = teoBAL.AddDebitTEODetails(teoDetailsModel, out transId);
                    ViewBag.TRANS_ID = transId;

                    if (success == String.Empty)
                    {
                        return this.Json(new { success = true, message = URLEncrypt.EncryptParameters(new string[] { teoDetailsModel.BILL_ID.ToString().Trim() }) });
                    }
                    else
                    {
                        return this.Json(new { success = false, message = URLEncrypt.EncryptParameters(new string[] { teoDetailsModel.BILL_ID.ToString().Trim() }) });
                    }
                }
                else
                {
                    return this.Json(new { success = false, message = ValidationSummary });
                }
            }
            else
            {
                ViewBag.BILL_ID = URLEncrypt.EncryptParameters(new string[] { teoDetailsModel.BILL_ID.ToString().Trim() });
                ViewBag.TransId = null;
                ViewBag.TEODetailTitle = "Debit Details";
                ViewBag.SaveTEODetails = "btnSaveTEODebitDetails";
                ViewBag.EditTEODetails = "btnEditTEODebitDetails";
                ViewBag.CancelTEODetails = "btnCancelTEODebitDetails";

                TransactionParams objMaster = new TransactionParams();
                TransferEntryOrderDAL teoDAL = new TransferEntryOrderDAL();
                commonFuncObj = new CommonFunctions();
                objMaster.ADMIN_ND_CODE = teoDetailsModel.ADMIN_ND_CODE == null ? PMGSYSession.Current.AdminNdCode : Convert.ToInt32(teoDetailsModel.ADMIN_ND_CODE);
                objMaster.MAST_CONT_ID = teoDetailsModel.MAST_CON_ID == null ? 0 : Convert.ToInt32(teoDetailsModel.MAST_CON_ID);
                objMaster.DISTRICT_CODE = teoDetailsModel.MAST_DISTRICT_CODE == null ? PMGSYSession.Current.DistrictCode : Convert.ToInt32(teoDetailsModel.MAST_DISTRICT_CODE);
                objMaster.AGREEMENT_CODE = teoDetailsModel.IMS_AGREEMENT_CODE == null ? 0 : Convert.ToInt32(teoDetailsModel.IMS_AGREEMENT_CODE);
                objMaster.SANC_YEAR = teoDetailsModel.SANC_YEAR == null ? (Int16)0 : Convert.ToInt16(teoDetailsModel.SANC_YEAR);
                objMaster.PACKAGE_ID = teoDetailsModel.IMS_PACKAGE_ID == null ? "0" : teoDetailsModel.IMS_PACKAGE_ID;
                objMaster.HEAD_ID = teoDetailsModel.HEAD_ID;

                Int16 parentId = teoBAL.GetMasterTransId(teoDetailsModel.BILL_ID);
                ViewBag.parentTxnId = parentId;//Added on 13-10-2022
                ACC_BILL_DETAILS acc_bill_details_trans = new ACC_BILL_DETAILS();

                acc_bill_details_trans = teoBAL.GetBillDetails(teoDetailsModel.BILL_ID, "C"); //dbContext.ACC_BILL_DETAILS.Where(m => m.BILL_ID == teoDetailsModel.BILL_ID && m.CREDIT_DEBIT == "D").FirstOrDefault();




                ACC_TEO_SCREEN_TXN_VALIDATIONS validationParams = new ACC_TEO_SCREEN_TXN_VALIDATIONS();

                validationParams = commonFuncObj.GetTEOValidationParams(0, parentId);


                ACC_TEO_SCREEN_DESIGN_PARAM_DETAILS designParams = new ACC_TEO_SCREEN_DESIGN_PARAM_DETAILS();
                designParams = commonFuncObj.getTEODesignParamDetails(0, parentId);

                if (designParams == null)
                {
                    designParams = teoDAL.setDefualtDesignTransParam();
                }

                objMaster.BILL_TYPE = "J";
                objMaster.FUND_TYPE = PMGSYSession.Current.FundType;
                objMaster.LVL_ID = PMGSYSession.Current.LevelId;
                objMaster.TXN_ID = parentId;
                objMaster.BILL_ID = teoDetailsModel.BILL_ID;
                objMaster.STATE_CODE = PMGSYSession.Current.StateCode;

                objMaster.CREDIT_DEBIT = "D";

                if (designParams.CON_REQ == "Y")
                {
                    objMaster.MAST_CON_SUP_FLAG = "C";
                }
                else if (designParams.SUPP_REQ == "Y")
                {
                    objMaster.MAST_CON_SUP_FLAG = "S";
                }

                if (designParams.DISTRICT_REQ == "Y")
                {
                    if (validationParams.IS_DISTRICT_REPEAT == "Y" && acc_bill_details_trans != null)
                    {
                        ViewBag.ddlDistrict = commonFuncObj.PopulateDistrictforTOB(objMaster.STATE_CODE, false, Convert.ToInt32(acc_bill_details_trans.MAST_DISTRICT_CODE));
                    }
                    else
                    {
                        ViewBag.ddlDistrict = commonFuncObj.PopulateDistrictforTOB(objMaster.STATE_CODE);
                    }
                }
                else
                {
                    ViewBag.ddlDistrict = new SelectList(new List<SelectListItem>().AsEnumerable<SelectListItem>(), "Value", "Text");
                }
                if (designParams.DPIU_REQ == "Y")
                {
                    if (validationParams.IS_DPIU_REPEAT == "Y" && acc_bill_details_trans != null)
                    {
                        Int32 tmpDistrict = objMaster.DISTRICT_CODE;
                        Int32 tmpAdminCode = objMaster.ADMIN_ND_CODE;
                        if (validationParams.IS_DPIU_REPEAT == "Y" && acc_bill_details_trans != null)
                        {
                            objMaster.DISTRICT_CODE = PMGSYSession.Current.LevelId == 5 ? PMGSYSession.Current.DistrictCode : Convert.ToInt32(acc_bill_details_trans.MAST_DISTRICT_CODE);
                            objMaster.ADMIN_ND_CODE = Convert.ToInt32(acc_bill_details_trans.ADMIN_ND_CODE);
                            ViewBag.ddlDPIU = commonFuncObj.PopulateDPIU(objMaster);
                        }
                        else
                        {
                            objMaster.DISTRICT_CODE = PMGSYSession.Current.LevelId == 5 ? PMGSYSession.Current.DistrictCode : Convert.ToInt32(acc_bill_details_trans.MAST_DISTRICT_CODE);
                            ViewBag.ddlDPIU = commonFuncObj.PopulateDPIU(objMaster);

                        }
                        objMaster.ADMIN_ND_CODE = tmpAdminCode;
                        objMaster.DISTRICT_CODE = tmpDistrict;
                    }
                    else
                    {
                        ViewBag.ddlDPIU = commonFuncObj.PopulateDPIU(objMaster);
                    }
                }
                else
                {
                    ViewBag.ddlDPIU = new SelectList(new List<SelectListItem>().AsEnumerable<SelectListItem>(), "Value", "Text");
                }

                if (designParams.CON_REQ == "Y" || designParams.SUPP_REQ == "Y")
                {
                    if (acc_bill_details_trans != null)
                    {
                        objMaster.MAST_CONT_ID = Convert.ToInt32(acc_bill_details_trans.MAST_CON_ID);
                        ViewBag.ddlContractor = commonFuncObj.PopulateContractorSupplier(objMaster);
                        objMaster.MAST_CONT_ID = 0;
                    }
                    else
                    {
                        ViewBag.ddlContractor = commonFuncObj.PopulateContractorSupplier(objMaster);
                    }
                }
                else
                {
                    ViewBag.ddlContractor = new SelectList(new List<SelectListItem>().AsEnumerable<SelectListItem>(), "Value", "Text");
                }

                if (designParams.AFREEMENT_REQ == "Y")
                {
                    if (acc_bill_details_trans != null)
                    {
                        Int32 tmpAggrementCode = objMaster.AGREEMENT_CODE;
                        Int32 tmpDistrict = objMaster.DISTRICT_CODE;
                        Int32 tmpContID = objMaster.MAST_CONT_ID;
                        objMaster.DISTRICT_CODE = PMGSYSession.Current.LevelId == 5 ? PMGSYSession.Current.DistrictCode : Convert.ToInt32(acc_bill_details_trans.MAST_DISTRICT_CODE);
                        objMaster.MAST_CONT_ID = Convert.ToInt32(acc_bill_details_trans.MAST_CON_ID);
                        objMaster.AGREEMENT_CODE = Convert.ToInt32(acc_bill_details_trans.IMS_AGREEMENT_CODE);
                        ViewBag.ddlAgreement = commonFuncObj.PopulateAgreement(objMaster);
                        objMaster.AGREEMENT_CODE = tmpAggrementCode;
                        objMaster.MAST_CONT_ID = tmpContID;
                        objMaster.DISTRICT_CODE = tmpDistrict;
                    }
                    //commented by koustubh Nakate on 05/09/2013 for agreement not populated when modelstate not valid
                    //else if (teoDetailsModel.TXN_NO == 0)
                    //{
                    //    List<SelectListItem> lstOneItem = new List<SelectListItem>();
                    //    lstOneItem.Insert(0, (new SelectListItem { Text = "Select Agreement", Value = "0", Selected = true }));
                    //    ViewBag.ddlAgreement = lstOneItem;
                    //}
                    else
                    {
                        ViewBag.ddlAgreement = commonFuncObj.PopulateAgreement(objMaster);
                    }
                }
                else
                {
                    ViewBag.ddlAgreement = new SelectList(new List<SelectListItem>().AsEnumerable<SelectListItem>(), "Value", "Text");
                }

                if (designParams.SAN_REQ == "Y")
                {
                    ViewBag.ddlSancYear = commonFuncObj.PopulateSancYear(objMaster);
                }
                else
                {
                    ViewBag.ddlSancYear = new SelectList(new List<SelectListItem>().AsEnumerable<SelectListItem>(), "Value", "Text");
                }

                //Added by Abhishek kamble to get Agreement No to populate road for MF start
                if (teoDetailsModel.IMS_AGREEMENT_CODE != null && PMGSYSession.Current.FundType == "M")
                {
                    objMaster.AGREEMENT_NUMBER = teoBAL.GetAgreementNumberForMF(teoDetailsModel.IMS_AGREEMENT_CODE);
                    objMaster.MAST_CONT_ID = teoDetailsModel.MAST_CON_ID.Value;
                }
                //Added by Abhishek kamble to get Agreement No to populate road for MF end

                //designParams.ROAD_REQ condition necessary bcoz it will populate unnecessarily 
                if (designParams.ROAD_REQ == "Y" && validationParams.IS_ROAD_REPEAT == "Y" && acc_bill_details_trans != null)
                {
                    Int32 tmpAggCode = objMaster.AGREEMENT_CODE;
                    objMaster.AGREEMENT_CODE = Convert.ToInt32(acc_bill_details_trans.IMS_AGREEMENT_CODE);
                    objMaster.ROAD_CODE = Convert.ToInt32(acc_bill_details_trans.IMS_PR_ROAD_CODE);
                    ViewBag.ddlRoad = commonFuncObj.PopulateRoad(objMaster);
                    objMaster.AGREEMENT_CODE = tmpAggCode;
                }
                else if (validationParams.IS_AGREEMENT_REPEAT == "Y" && acc_bill_details_trans != null && designParams.AFREEMENT_REQ == "Y")
                {
                    Int32 tmpAggCode = objMaster.AGREEMENT_CODE;
                    objMaster.AGREEMENT_CODE = Convert.ToInt32(acc_bill_details_trans.IMS_AGREEMENT_CODE);
                    objMaster.ROAD_CODE = 0;
                    ViewBag.ddlRoad = commonFuncObj.PopulateRoad(objMaster);
                    objMaster.AGREEMENT_CODE = tmpAggCode;
                }
                else if (designParams.ROAD_REQ == "Y" && acc_bill_details_trans != null)
                {
                    Int32 tmpAggCode = objMaster.AGREEMENT_CODE;
                    objMaster.AGREEMENT_CODE = Convert.ToInt32(acc_bill_details_trans.IMS_AGREEMENT_CODE);
                    objMaster.ROAD_CODE = 0;
                    ViewBag.ddlRoad = commonFuncObj.PopulateRoad(objMaster);
                    objMaster.AGREEMENT_CODE = tmpAggCode;
                }
                else
                {
                    //ViewBag.ddlRoad = new SelectList(new List<SelectListItem>().AsEnumerable<SelectListItem>(), "Value", "Text");
                    //new change done by Vikram -- for showing the road dropdown when the entry is the first entry
                    if (teoDetailsModel.IMS_PR_ROAD_CODE != 0 && teoDetailsModel.IMS_PR_ROAD_CODE != null)
                    {
                        Int32 tmpAggCode = objMaster.AGREEMENT_CODE;
                        ViewBag.ddlRoad = commonFuncObj.PopulateRoad(objMaster);
                        objMaster.ROAD_CODE = 0;
                        ViewBag.ddlRoad = commonFuncObj.PopulateRoad(objMaster);
                    }
                    else
                    {
                        ViewBag.ddlRoad = new SelectList(new List<SelectListItem>().AsEnumerable<SelectListItem>(), "Value", "Text");
                    }
                }

                if (designParams.PKG_REQ == "Y")
                {
                    if (teoDetailsModel.TXN_NO == 0)
                    {
                        List<SelectListItem> lstOneItem = new List<SelectListItem>();
                        lstOneItem.Insert(0, (new SelectListItem { Text = "Select Package", Value = "0", Selected = true }));
                        ViewBag.ddlPackage = lstOneItem;
                    }
                    else
                    {
                        ViewBag.ddlPackage = commonFuncObj.PopulatePackage(objMaster);
                    }
                }
                else
                {
                    ViewBag.ddlPackage = new SelectList(new List<SelectListItem>().AsEnumerable<SelectListItem>(), "Value", "Text");
                }
                if (validationParams.IS_HEAD_REPEAT == "Y" && acc_bill_details_trans != null)
                {
                    objMaster.HEAD_ID = acc_bill_details_trans.HEAD_ID;
                    ViewBag.ddlHead = commonFuncObj.PopulateHead(objMaster);
                    objMaster.HEAD_ID = teoDetailsModel.HEAD_ID;
                }
                else
                {
                    ViewBag.ddlHead = commonFuncObj.PopulateHead(objMaster);
                }

                ACC_TEO_SCREEN_DESIGN_HEADWISE_DETAILS headParams = new ACC_TEO_SCREEN_DESIGN_HEADWISE_DETAILS();
                if (acc_bill_details_trans != null)
                {
                    objMaster.HEAD_ID = acc_bill_details_trans.HEAD_ID;
                }
                headParams = commonFuncObj.GetHeadwiseDesignParams(objMaster);
                objMaster.HEAD_ID = teoDetailsModel.HEAD_ID;
                if (headParams == null)
                {
                    headParams = teoDAL.setDefualtDesignHeadParam();
                }

                if (acc_bill_details_trans != null && headParams.CON_REQ == "Y")
                {
                    objMaster.MAST_CONT_ID = Convert.ToInt32(acc_bill_details_trans.MAST_CON_ID);
                    ViewBag.ddlHeadContractor = commonFuncObj.PopulateContractorSupplier(objMaster);
                    objMaster.MAST_CONT_ID = 0;
                }
                else
                {
                    ViewBag.ddlHeadContractor = new SelectList(new List<SelectListItem>().AsEnumerable<SelectListItem>(), "Value", "Text");
                }

                if (acc_bill_details_trans != null && headParams.AGREEMENT_REQ == "Y")
                {
                    Int32 tmpAggrementCode = objMaster.AGREEMENT_CODE;
                    Int32 tmpDistrict = objMaster.DISTRICT_CODE;
                    Int32 tmpContID = objMaster.MAST_CONT_ID;

                    objMaster.DISTRICT_CODE = PMGSYSession.Current.LevelId == 5 ? PMGSYSession.Current.DistrictCode : Convert.ToInt32(acc_bill_details_trans.MAST_DISTRICT_CODE); ;
                    objMaster.MAST_CONT_ID = Convert.ToInt32(acc_bill_details_trans.MAST_CON_ID);
                    objMaster.AGREEMENT_CODE = Convert.ToInt32(acc_bill_details_trans.IMS_AGREEMENT_CODE);
                    //ViewBag.ddlHeadAgreement = commonFuncObj.PopulateAgreement(objMaster);
                    JavaScriptSerializer json_serializer = new JavaScriptSerializer();
                    string id = objMaster.DISTRICT_CODE + "$" + objMaster.AGREEMENT_CODE;
                    //List<SelectListItem> AgrWithNoOption = json_serializer.Deserialize<List<SelectListItem>>(json_serializer.Serialize(PopulateAgreementWithNoOption(id).Data));
                    ViewBag.ddlHeadAgreement = parentId == 3100 ? json_serializer.Deserialize<List<SelectListItem>>(json_serializer.Serialize(PopulateAgreementWithNoOption(id).Data)) : commonFuncObj.PopulateAgreement(objMaster);

                    objMaster.AGREEMENT_CODE = tmpAggrementCode;
                    objMaster.MAST_CONT_ID = tmpContID;
                    objMaster.DISTRICT_CODE = tmpDistrict;
                }
                else
                {
                    ViewBag.ddlHeadAgreement = new SelectList(new List<SelectListItem>().AsEnumerable<SelectListItem>(), "Value", "Text");
                }


                ViewBag.ddlHeadSancYear = new SelectList(new List<SelectListItem>().AsEnumerable<SelectListItem>(), "Value", "Text");

                ViewBag.ddlHeadPackage = new SelectList(new List<SelectListItem>().AsEnumerable<SelectListItem>(), "Value", "Text");

                if (validationParams.IS_ROAD_REPEAT == "Y" && acc_bill_details_trans != null && headParams.ROAD_REQ == "Y")
                {
                    Int32 tmpAggCode = objMaster.AGREEMENT_CODE;
                    objMaster.AGREEMENT_CODE = Convert.ToInt32(acc_bill_details_trans.IMS_AGREEMENT_CODE);
                    objMaster.ROAD_CODE = Convert.ToInt32(acc_bill_details_trans.IMS_PR_ROAD_CODE);
                    ViewBag.ddlHeadRoad = commonFuncObj.PopulateRoad(objMaster);
                    objMaster.AGREEMENT_CODE = tmpAggCode;
                }
                else if (headParams.ROAD_REQ == "Y" && validationParams.IS_AGREEMENT_REPEAT == "Y" && acc_bill_details_trans != null && headParams.AGREEMENT_REQ == "Y")
                {
                    Int32 tmpAggCode = objMaster.AGREEMENT_CODE;
                    objMaster.AGREEMENT_CODE = Convert.ToInt32(acc_bill_details_trans.IMS_AGREEMENT_CODE);
                    objMaster.ROAD_CODE = 0;
                    ViewBag.ddlHeadRoad = commonFuncObj.PopulateRoad(objMaster);
                    objMaster.AGREEMENT_CODE = tmpAggCode;
                }
                else
                {
                    ViewBag.ddlHeadRoad = new SelectList(new List<SelectListItem>().AsEnumerable<SelectListItem>(), "Value", "Text");
                }


                if (acc_bill_details_trans != null && validationParams.IS_HEAD_REPEAT == "Y")
                {
                    teoDetailsModel.HEAD_ID = acc_bill_details_trans.HEAD_ID;
                }

                #region old code
                /*
                ACC_TEO_SCREEN_DESIGN_PARAM_DETAILS designParams = new ACC_TEO_SCREEN_DESIGN_PARAM_DETAILS();
                Int16 parentId = teoBAL.GetMasterTransId(teoDetailsModel.BILL_ID);
                designParams = commonFuncObj.getTEODesignParamDetails(0,parentId);
                if (designParams == null)
                {
                    designParams = teoDAL.setDefualtDesignTransParam();
                }
                objMaster.BILL_TYPE = "J";
                objMaster.FUND_TYPE = PMGSYSession.Current.FundType;
                objMaster.LVL_ID = PMGSYSession.Current.LevelId;
                objMaster.TXN_ID = parentId;
                objMaster.BILL_ID = teoDetailsModel.BILL_ID;
                objMaster.STATE_CODE = PMGSYSession.Current.StateCode;

                objMaster.CREDIT_DEBIT = "D";
                List<SelectListItem> lstOneItem = new List<SelectListItem>();

                if (designParams.CON_REQ == "Y")
                {
                    objMaster.MAST_CON_SUP_FLAG = "C";
                }
                else if (designParams.SUPP_REQ == "Y")
                {
                    objMaster.MAST_CON_SUP_FLAG = "S";
                }

                if (designParams.DISTRICT_REQ == "Y")
                {
                    ViewBag.ddlDistrict = commonFuncObj.PopulateDistrict(objMaster.STATE_CODE);
                }
                else
                {
                    ViewBag.ddlDistrict = new SelectList(new List<SelectListItem>().AsEnumerable<SelectListItem>(), "Value", "Text");
                }
                if (designParams.DPIU_REQ == "Y")
                {
                    ViewBag.ddlDPIU = commonFuncObj.PopulateDPIU(objMaster);
                }
                else
                {
                    ViewBag.ddlDPIU = new SelectList(new List<SelectListItem>().AsEnumerable<SelectListItem>(), "Value", "Text");
                }

                if (designParams.CON_REQ == "Y" || designParams.SUPP_REQ == "Y")
                {
                    ViewBag.ddlContractor = commonFuncObj.PopulateContractorSupplier(objMaster);
                }
                else
                {
                    ViewBag.ddlContractor = new SelectList(new List<SelectListItem>().AsEnumerable<SelectListItem>(), "Value", "Text");
                }

                if (designParams.AFREEMENT_REQ == "Y")
                {
                    ViewBag.ddlAgreement = commonFuncObj.PopulateAgreement(objMaster);
                }
                else
                {
                    ViewBag.ddlAgreement = new SelectList(new List<SelectListItem>().AsEnumerable<SelectListItem>(), "Value", "Text");
                }

                if (designParams.SAN_REQ == "Y")
                {
                    ViewBag.ddlSancYear = commonFuncObj.PopulateSancYear(objMaster);
                }
                else
                {
                    ViewBag.ddlSancYear = new SelectList(new List<SelectListItem>().AsEnumerable<SelectListItem>(), "Value", "Text");
                }

                if (designParams.ROAD_REQ == "Y")
                {
                    ViewBag.ddlRoad = commonFuncObj.PopulateRoad(objMaster);
                }
                else
                {
                    ViewBag.ddlRoad = new SelectList(new List<SelectListItem>().AsEnumerable<SelectListItem>(), "Value", "Text");
                }

                if (designParams.PKG_REQ == "Y")
                {
                    ViewBag.ddlPackage = commonFuncObj.PopulatePackage(objMaster);
                }
                else
                {
                    ViewBag.ddlPackage = new SelectList(new List<SelectListItem>().AsEnumerable<SelectListItem>(), "Value", "Text");
                }

                ViewBag.ddlHead = commonFuncObj.PopulateHead(objMaster);
                if (teoDetailsModel.HEAD_ID != 0)
                {
                    ACC_TEO_SCREEN_DESIGN_HEADWISE_DETAILS headParams = new ACC_TEO_SCREEN_DESIGN_HEADWISE_DETAILS();
                    headParams = commonFuncObj.GetHeadwiseDesignParams(objMaster);
                    if (headParams == null)
                    {
                        headParams = teoDAL.setDefualtDesignHeadParam();
                    }
                    if (headParams.CON_REQ == "Y")
                    {
                        ViewBag.ddlHeadContractor = commonFuncObj.PopulateContractorSupplier(objMaster);
                    }
                    else
                    {
                        ViewBag.ddlHeadContractor = new SelectList(new List<SelectListItem>().AsEnumerable<SelectListItem>(), "Value", "Text");
                    }
                    if (headParams.AGREEMENT_REQ == "Y")
                    {
                        ViewBag.ddlHeadAgreement = commonFuncObj.PopulateAgreement(objMaster);
                    }
                    else
                    {
                        ViewBag.ddlHeadAgreement = new SelectList(new List<SelectListItem>().AsEnumerable<SelectListItem>(), "Value", "Text");
                    }
                    if (headParams.SANC_YEAR_REQ == "Y")
                    {
                        ViewBag.ddlHeadSancYear = commonFuncObj.PopulateSancYear(objMaster);
                    }
                    else
                    {
                        ViewBag.ddlHeadSancYear = new SelectList(new List<SelectListItem>().AsEnumerable<SelectListItem>(), "Value", "Text");
                    }
                    if (headParams.ROAD_REQ == "Y")
                    {
                        ViewBag.ddlHeadRoad = commonFuncObj.PopulateRoad(objMaster);
                    }
                    else
                    {
                        ViewBag.ddlHeadRoad = new SelectList(new List<SelectListItem>().AsEnumerable<SelectListItem>(), "Value", "Text");
                    }
                    if (headParams.PKG_REQ == "Y")
                    {
                        ViewBag.ddlHeadPackage = commonFuncObj.PopulatePackage(objMaster);
                    }
                    else
                    {
                        ViewBag.ddlHeadPackage = new SelectList(new List<SelectListItem>().AsEnumerable<SelectListItem>(), "Value", "Text");
                    }
                }
                else
                {
                    ViewBag.ddlHeadContractor = new SelectList(new List<SelectListItem>().AsEnumerable<SelectListItem>(), "Value", "Text");
                    ViewBag.ddlHeadAgreement = new SelectList(new List<SelectListItem>().AsEnumerable<SelectListItem>(), "Value", "Text");
                    ViewBag.ddlHeadSancYear = new SelectList(new List<SelectListItem>().AsEnumerable<SelectListItem>(), "Value", "Text");
                    ViewBag.ddlHeadRoad = new SelectList(new List<SelectListItem>().AsEnumerable<SelectListItem>(), "Value", "Text");
                    ViewBag.ddlHeadPackage = new SelectList(new List<SelectListItem>().AsEnumerable<SelectListItem>(), "Value", "Text");
                }
                 */
                #endregion
                ViewBag.CreditDebit = "D";
                teoDetailsModel.HEAD_ID = 0;
                return View("TEOAddDetails", teoDetailsModel);
            }
        }

        [Audit]
        public JsonResult GetHeadwiseDesignParams(String parameter, String hash, String key, String id)
        {
            ACC_TEO_SCREEN_DESIGN_HEADWISE_DETAILS designParams = new ACC_TEO_SCREEN_DESIGN_HEADWISE_DETAILS();
            TransactionParams objParam = new TransactionParams();
            String[] strParameters = URLEncrypt.DecryptParameters(new string[] { parameter, hash, key });
            objParam.BILL_ID = Convert.ToInt64(strParameters[0]);
            objParam.HEAD_ID = Convert.ToInt16(id);
            commonFuncObj = new CommonFunctions();
            designParams = commonFuncObj.GetHeadwiseDesignParams(objParam);
            if (designParams == null)
            {
                return Json(new { CON_REQ = "N", AGREEMENT_REQ = "N", SANCYEAR_REQ = "N", PKG_REQ = "N", ROAD_REQ = "N" });
            }
            else
            {
                return Json(new { CON_REQ = designParams.CON_REQ, AGREEMENT_REQ = designParams.AGREEMENT_REQ, SANCYEAR_REQ = designParams.SANC_YEAR_REQ, PKG_REQ = designParams.PKG_REQ, ROAD_REQ = designParams.ROAD_REQ });
            }
        }

        [Audit]
        public JsonResult GetTransDesignParams(String parameter, String hash, String key)
        {
            ACC_TEO_SCREEN_DESIGN_PARAM_DETAILS designParams = new ACC_TEO_SCREEN_DESIGN_PARAM_DETAILS();
            TransactionParams objParam = new TransactionParams();
            String[] strParameters = URLEncrypt.DecryptParameters(new string[] { parameter, hash, key });
            Int64 billId = Convert.ToInt64(strParameters[0]);
            commonFuncObj = new CommonFunctions();
            designParams = commonFuncObj.getTEODesignParamDetails(billId, 0);
            if (designParams == null)
            {
                return Json(new { DIST_REQ = "N", DPIU_REQ = "N", CON_REQ = "N", SUP_REQ = "N", AGREEMENT_REQ = "N", SANCYEAR_REQ = "N", PKG_REQ = "N", ROAD_REQ = "N" });
            }
            else
            {
                return Json(new { DIST_REQ = designParams.DISTRICT_REQ, DPIU_REQ = designParams.DPIU_REQ, CON_REQ = designParams.CON_REQ, SUP_REQ = designParams.SUPP_REQ, AGREEMENT_REQ = designParams.AFREEMENT_REQ, SANCYEAR_REQ = designParams.SAN_REQ, PKG_REQ = designParams.PKG_REQ, ROAD_REQ = designParams.ROAD_REQ });
            }
        }

        [Audit]
        public JsonResult GetTEOValidationParams(String parameter, String hash, String key)
        {
            ACC_TEO_SCREEN_TXN_VALIDATIONS designParams = new ACC_TEO_SCREEN_TXN_VALIDATIONS();
            TransactionParams objParam = new TransactionParams();
            String[] strParameters = URLEncrypt.DecryptParameters(new string[] { parameter, hash, key });
            Int64 billId = Convert.ToInt64(strParameters[0]);
            commonFuncObj = new CommonFunctions();
            designParams = commonFuncObj.GetTEOValidationParams(billId, 0);
            if (designParams == null)
            {
                return Json(new { CHead = "0", DHead = "0", District = "N", DPIU = "N", Contractor = "N", Supplier = "N", Agreement = "N", Road = "N", Head = "N" });
            }
            else
            {
                //if (_populateActiveInactiveDistricts)//Added by Abhishek 6jan2016 for head 20.01
                //{
                //    return Json(new { CHead = designParams.C_CREDIT_HEAD, DHead = designParams.C_DEBIT_HEAD, District ="Y", DPIU = "Y", Contractor = designParams.IS_CON_REPEAT, Supplier = designParams.IS_SUP_REPEAT, Agreement = designParams.IS_AGREEMENT_REPEAT, Road = designParams.IS_ROAD_REPEAT, Head = designParams.IS_HEAD_REPEAT });
                //}
                //else
                //{
                //    return Json(new { CHead = designParams.C_CREDIT_HEAD, DHead = designParams.C_DEBIT_HEAD, District = designParams.IS_DISTRICT_REPEAT, DPIU = designParams.IS_DPIU_REPEAT, Contractor = designParams.IS_CON_REPEAT, Supplier = designParams.IS_SUP_REPEAT, Agreement = designParams.IS_AGREEMENT_REPEAT, Road = designParams.IS_ROAD_REPEAT, Head = designParams.IS_HEAD_REPEAT });
                //}
                return Json(new { CHead = designParams.C_CREDIT_HEAD, DHead = designParams.C_DEBIT_HEAD, District = designParams.IS_DISTRICT_REPEAT, DPIU = designParams.IS_DPIU_REPEAT, Contractor = designParams.IS_CON_REPEAT, Supplier = designParams.IS_SUP_REPEAT, Agreement = designParams.IS_AGREEMENT_REPEAT, Road = designParams.IS_ROAD_REPEAT, Head = designParams.IS_HEAD_REPEAT });

            }
        }

        [Audit]
        public JsonResult PopulateDPIU(String id)
        {
            try
            {
                TransactionParams objparams = new TransactionParams();
                TransferEntryOrderDAL objDAL = new TransferEntryOrderDAL();
                String[] urlParam;
                int count = 0;
                commonFuncObj = new CommonFunctions();
                if (id != null && id != "null") // new change done by Vikram as in TOB id is null.
                {
                    //objparams.DISTRICT_CODE = Convert.ToInt32(id.Trim());
                    //Added By Abhishek kamble 5-june-2014
                    urlParam = id.Split('$');
                    objparams.DISTRICT_CODE = Convert.ToInt32(urlParam[0]);
                    count = urlParam.Count();
                }
                if (count == 2)
                {
                    return Json(commonFuncObj.PopulateDPIU(objparams));
                }
                else
                {
                    return Json(objDAL.PopulateDPIUForTOB(objparams));
                }
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return Json(null);
            }
        }

        [Audit]
        public JsonResult PopulateContractor(String id)
        {
            try
            {
                TransactionParams objparams = new TransactionParams();
                commonFuncObj = new CommonFunctions();
                objparams.STATE_CODE = PMGSYSession.Current.StateCode;
                //if (id == null)       //modified by Abhishek for Adjustment on Road Heads and advances 12Mar2015
                if (id == null || id == "null")
                {
                    objparams.DISTRICT_CODE = PMGSYSession.Current.DistrictCode;
                }
                else
                {
                    objparams.DISTRICT_CODE = Convert.ToInt32(id);
                }
                objparams.MAST_CON_SUP_FLAG = "C";
                return Json(commonFuncObj.PopulateContractorSupplier(objparams));
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return Json(null);
            }

        }

        public JsonResult PopulateContractorWithNoOption(String id)
        {
            try
            {
                TransactionParams objparams = new TransactionParams();
                commonFuncObj = new CommonFunctions();
                objparams.STATE_CODE = PMGSYSession.Current.StateCode;
                //if (id == null)       //modified by Abhishek for Adjustment on Road Heads and advances 12Mar2015
                if (id == null || id == "null")
                {
                    objparams.DISTRICT_CODE = PMGSYSession.Current.DistrictCode;
                }
                else
                {
                    objparams.DISTRICT_CODE = Convert.ToInt32(id);
                }
                objparams.MAST_CON_SUP_FLAG = "C";

                List<SelectListItem> contractorList = commonFuncObj.PopulateContractorSupplier(objparams);
                List<SelectListItem> contractorListWithNoOption = new List<SelectListItem>();

                for (int i = 0; i < contractorList.Count(); i++)
                {
                    if (i == 0)
                    {
                        contractorListWithNoOption.Add(contractorList[i]);
                        contractorListWithNoOption.Add(new SelectListItem { Text = "No Contractor", Value = "-1" });
                    }
                    else
                    {
                        contractorListWithNoOption.Add(contractorList[i]);

                    }
                }

                return Json(contractorListWithNoOption);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return Json(null);
            }

        }

        [Audit]
        public JsonResult PopulateAgreement(String id)
        {
            try
            {
                TransactionParams objparams = new TransactionParams();
                commonFuncObj = new CommonFunctions();
                if (id != null)// new change done by Vikram as in TOB id is null.
                {
                    //objparams.DISTRICT_CODE = id.Split('$')[0] == "0" ? PMGSYSession.Current.DistrictCode : Convert.ToInt32(id.Split('$')[0].Trim());
                    //modified by Abhishek for Adjustment on Road Heads and advances 12Mar2015
                    objparams.DISTRICT_CODE = (id.Split('$')[0] == "0" || id.Split('$')[0] == null || id.Split('$')[0] == "null") ? PMGSYSession.Current.DistrictCode : Convert.ToInt32(id.Split('$')[0].Trim());
                    if (id.Split('$')[1] != "null")
                    {
                        objparams.MAST_CONT_ID = Convert.ToInt32(id.Split('$')[1].Trim());
                    }
                }
                return Json(commonFuncObj.PopulateAgreement(objparams));
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return Json(null);
            }

        }

        [Audit]
        public JsonResult PopulateAgreementWithNoOption(String id)
        {
            try
            {
                TransactionParams objparams = new TransactionParams();
                commonFuncObj = new CommonFunctions();
                if (id != null)// new change done by Vikram as in TOB id is null.
                {
                    //objparams.DISTRICT_CODE = id.Split('$')[0] == "0" ? PMGSYSession.Current.DistrictCode : Convert.ToInt32(id.Split('$')[0].Trim());
                    //modified by Abhishek for Adjustment on Road Heads and advances 12Mar2015
                    objparams.DISTRICT_CODE = (id.Split('$')[0] == "0" || id.Split('$')[0] == null || id.Split('$')[0] == "null") ? PMGSYSession.Current.DistrictCode : Convert.ToInt32(id.Split('$')[0].Trim());
                    if (id.Split('$')[1] != "null")
                    {
                        objparams.MAST_CONT_ID = Convert.ToInt32(id.Split('$')[1].Trim());
                    }
                }
                List<SelectListItem> agreementListWithNoOptions = new List<SelectListItem>();

                if (id.Split('$')[1] != "null" && objparams.MAST_CONT_ID == -1)
                {
                    //objparams.AGREEMENT_NUMBER = Request.Params["AGREEMENT_NUMBER"];
                    agreementListWithNoOptions.Add(new SelectListItem { Text = "--select--", Value = "0", Selected = true });
                    agreementListWithNoOptions.Add(new SelectListItem { Text = "No Agreement", Value = "-1" });
                }

                //List<SelectListItem> agreementList = commonFuncObj.PopulateAgreement(objparams);
                //List<SelectListItem> agreementListWithNoOptions = new List<SelectListItem>();

                //for (int i = 0; i < agreementList.Count(); i++)
                //{
                //    if (i == 0)
                //    {
                //        agreementListWithNoOptions.Add(agreementList[i]);
                //        agreementListWithNoOptions.Add(new SelectListItem { Text = "No Agreement", Value = "-1" });
                //    }
                //    else
                //    {
                //        agreementListWithNoOptions.Add(agreementList[i]);

                //    }
                //}

                return Json(agreementListWithNoOptions);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return Json(null);
            }

        }

        [Audit]
        public JsonResult PopulateRoad(String id)
        {
            try
            {
                TransactionParams objparams = new TransactionParams();
                commonFuncObj = new CommonFunctions();
                objparams.AGREEMENT_CODE = Convert.ToInt32(id.Split('$')[0].Trim());
                if (id.Split('$')[1] != null && id.Split('$')[2] != "null" && id.Split('$')[2] != "undefined")
                {
                    objparams.PACKAGE_ID = id.Split('$')[1].ToString().Trim();
                }

                if (id.Split('$')[2] != null)
                {
                    objparams.SANC_YEAR = Convert.ToInt16(id.Split('$')[2].Trim());
                }

                if (id.Split('$').Count() > 3 && id.Split('$')[3] != "undefined" && id.Split('$')[3] != "NaN" && id.Split('$')[3] != "null")
                {
                    objparams.MAST_CONT_ID = Convert.ToInt32(id.Split('$')[3].Trim());
                }

                if (id.Split('$').Count() > 4 && id.Split('$')[4] != "undefined")
                {
                    objparams.HEAD_ID = Convert.ToInt16(id.Split('$')[4].Trim());
                }

                if (!String.IsNullOrEmpty(Request.Params["AGREEMENT_NUMBER"]))
                {
                    objparams.AGREEMENT_NUMBER = Request.Params["AGREEMENT_NUMBER"];
                }


                return Json(commonFuncObj.PopulateRoad(objparams));
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return Json(false);
            }
        }

        [Audit]
        public JsonResult PopulateRoadWithNoOption(String id)
        {
            try
            {
                //TransactionParams objparams = new TransactionParams();
                commonFuncObj = new CommonFunctions();
                int AGREEMENT_CODE = Convert.ToInt32(id.Split('$')[0].Trim());
                List<SelectListItem> roadsWithNoOption = new List<SelectListItem>();

                if (!String.IsNullOrEmpty(Request.Params["AGREEMENT_NUMBER"]) && AGREEMENT_CODE == -1)
                {
                    //objparams.AGREEMENT_NUMBER = Request.Params["AGREEMENT_NUMBER"];
                    roadsWithNoOption.Add(new SelectListItem { Text = "No Road", Value = "-1", Selected = true });
                }


                return Json(roadsWithNoOption);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return Json(false);
            }
        }

        [Audit]
        public JsonResult PopulateSancYear()
        {
            try
            {
                TransactionParams objparams = new TransactionParams();
                commonFuncObj = new CommonFunctions();
                objparams.STATE_CODE = PMGSYSession.Current.StateCode;
                objparams.DISTRICT_CODE = PMGSYSession.Current.DistrictCode;
                return Json(commonFuncObj.PopulateSancYear(objparams));
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return Json(null);
            }
        }

        [Audit]
        //public JsonResult PopulatePackage(String id)
        //{
        //    try
        //    {
        //        TransactionParams objparams = new TransactionParams();
        //        commonFuncObj = new CommonFunctions();
        //        objparams.STATE_CODE = PMGSYSession.Current.StateCode;
        //        objparams.DISTRICT_CODE = PMGSYSession.Current.DistrictCode;
        //        if (id != null && id != "null")
        //        {
        //            objparams.SANC_YEAR = Convert.ToInt16(id);
        //        }
        //        return Json(commonFuncObj.PopulatePackage(objparams));
        //    }
        //    catch (Exception ex)
        //    {
        //        Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
        //        return Json(null);
        //    }

        //}
        public JsonResult PopulatePackage(String id)
        {
            try
            {

                //if (id.Split('$')[2] != null)
                //{
                //    objparams.SANC_YEAR = Convert.ToInt16(id.Split('$')[2].Trim());
                //}
                TransactionParams objparams = new TransactionParams();
                commonFuncObj = new CommonFunctions();

                if (id.Split('$').Count() > 0)
                {

                    objparams.STATE_CODE = PMGSYSession.Current.StateCode;
                    if (id.Split('$').Count() == 1)
                    {
                        if (Convert.ToInt32(PMGSYSession.Current.DistrictCode) != null)
                        {
                            objparams.DISTRICT_CODE = PMGSYSession.Current.DistrictCode;
                        }
                    }
                    else
                    {
                        objparams.DISTRICT_CODE = Convert.ToInt32(id.Split('$')[1].Trim());
                    }

                    if (Convert.ToInt16(id.Split('$')[0].Trim()) != null)
                    {
                        objparams.SANC_YEAR = Convert.ToInt16(id.Split('$')[0].Trim());
                    }
                }
                else
                {

                    objparams.STATE_CODE = PMGSYSession.Current.StateCode;
                    objparams.DISTRICT_CODE = PMGSYSession.Current.DistrictCode;


                    if (id != null && id != "null")
                    {
                        objparams.SANC_YEAR = Convert.ToInt16(id);
                    }

                }






                return Json(commonFuncObj.PopulatePackage(objparams));
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return Json(null);
            }

        }

        [Audit]
        public JsonResult PopulateSubTransaction(String id)
        {
            try
            {
                TransactionParams objparams = new TransactionParams();
                commonFuncObj = new CommonFunctions();
                objparams.BILL_TYPE = "J";
                objparams.FUND_TYPE = PMGSYSession.Current.FundType;
                objparams.LVL_ID = PMGSYSession.Current.LevelId;
                objparams.TXN_ID = Convert.ToInt16(id);
                return Json(commonFuncObj.PopulateTransactions(objparams));
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return null;
            }

        }

        //Added By Abhishek kamble to populate District drop down   1 Oct 2014 start

        [Audit]
        public JsonResult PopulateDistricts(String id)
        {
            try
            {
                commonFuncObj = new CommonFunctions();
                if (id != null && id != String.Empty)
                {
                    //old
                    //return Json(commonFuncObj.PopulateDistrict(Convert.ToInt32(id), false));
                    return Json(commonFuncObj.PopulateDistrict(Convert.ToInt32(id), false, 0, false, _populateActiveInactiveDistricts));         //_populateActiveInactiveDistricts - Added by Abhishek kamble 5Jan2015 for head 21.01 for asset and lib for state AP           
                }
                else
                {
                    return Json(commonFuncObj.PopulateDistrict(0, false));
                }
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return Json(null);
            }
        }

        //Added By Abhishek kamble to populate District drop down   1 Oct 2014 end

        [HttpPost]
        [Audit]
        public String GetNarration(String id)
        {
            commonFuncObj = new CommonFunctions();
            String TransID = id.ToString();
            //String[] strParameters = URLEncrypt.DecryptParameters(new string[] { parameter, hash, key });
            return commonFuncObj.GetTransNarration(Convert.ToInt16(TransID), null, "T");
        }

        [HttpPost]
        [Audit]
        public String IsFinalPayment(String id)
        {
            teoBAL = new TransferEntryOrderBAL();
            string billId = string.Empty;
            if (!String.IsNullOrEmpty(Request.Params["BillID"]))
            {
                billId = Request.Params["BillID"];
            }
            return teoBAL.IsFinalPayment(id, billId);
        }

        //added by Koustubh Nakate on 30/09/2013 to check head is already exist or not for given BILL ID  
        /// <summary>
        /// to check head already exist for billId
        /// </summary>
        /// <returns></returns>
        [Audit]
        public ActionResult CheckHeadAlreadyExist()
        {

            bool exist = false;
            string creditDebit = string.Empty;
            Int16 headID = 0;
            Int64 billID = 0;
            string[] paramaters = null;
            try
            {
                teoBAL = new TransferEntryOrderBAL();
                if (Request.Params["BillID"] == null || Request.Params["HeadID"] == null || Request.Params["CreditDebit"] == null)
                {
                    return Json(new { exist = true }, JsonRequestBehavior.AllowGet);
                }

                paramaters = Request.Params["BillID"].ToString().Split('/');

                string[] strParameters = URLEncrypt.DecryptParameters(new string[] { paramaters[0], paramaters[1], paramaters[2] });

                billID = Convert.ToInt64(strParameters[0]);
                headID = Convert.ToInt16(Request.Params["HeadID"].ToString());
                creditDebit = Request.Params["CreditDebit"].ToString().Trim();

                exist = teoBAL.CheckHeadAlreadyExistBAL(billID, headID, creditDebit);

                return Json(new { exist = exist }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return Json(new { exist = exist }, JsonRequestBehavior.AllowGet);
            }
        }

        /* region added by Koustubh Nakate on 26/08/2013 for transfer of balances   */

        #region Transfer Of Balances

        [Audit]
        public ActionResult TransferOfBalancesList()
        {
            Int32 Month = DateTime.Now.Month;
            Int32 Year = DateTime.Now.Year;
            if (Request.Params["Month"] != null)
            {
                Month = Convert.ToInt32(Request.Params["Month"]);
                Year = Convert.ToInt32(Request.Params["Year"]);
            }
            //new change done by Vikram on 01-Jan-2014
            else if (PMGSYSession.Current.AccMonth != 0)
            {
                Month = Convert.ToInt32(PMGSYSession.Current.AccMonth);
                Year = Convert.ToInt32(PMGSYSession.Current.AccYear);
            }
            //end of change
            commonFuncObj = new CommonFunctions();
            ViewBag.ddlMonth = commonFuncObj.PopulateMonths(Month);
            ViewBag.ddlYear = commonFuncObj.PopulateYears(Year);
            TransactionParams objMaster = new TransactionParams();
            objMaster.BILL_TYPE = BILLTYPE;
            objMaster.FUND_TYPE = PMGSYSession.Current.FundType;
            objMaster.LVL_ID = PMGSYSession.Current.LevelId;

            List<SelectListItem> lstTransacrion = commonFuncObj.PopulateTransactions(objMaster);

            //  lstTransacrion.RemoveRange(1, 3);

            lstTransacrion = (from TXN in lstTransacrion
                              where lstTransactionIDs.Any(ID => ID.Value == TXN.Value || TXN.Value == string.Empty)
                              select TXN).ToList();


            ViewBag.ddlMasterTrans = lstTransacrion;

            ViewBag.ImprestLevel = objMaster.LVL_ID;
            ViewBag.ImprestFundType = objMaster.FUND_TYPE;
            return View("TransferOfBalancesList");
        }


        [HttpPost]
        [Audit]
        public ActionResult GetTEOListforTransferOfBalances(FormCollection homeFormCollection)
        {
            //Adde By Abhishek kamble 30-Apr-2014 start
            using (CommonFunctions commonFunction = new CommonFunctions())
            {
                if (!commonFunction.ValidateGridParameters(new GridParams(Convert.ToInt32(homeFormCollection["page"]), Convert.ToInt32(homeFormCollection["rows"]), homeFormCollection["sidx"], homeFormCollection["sord"], Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                {
                    return null;
                }
            }
            //Adde By Abhishek kamble 30-Apr-2014 end
            teoBAL = new TransferEntryOrderBAL();
            ReceiptFilterModel objFilter = new ReceiptFilterModel();
            long totalRecords;
            objFilter.Month = Convert.ToInt16(Request.Params["month"]);
            objFilter.Year = Convert.ToInt16(Request.Params["year"]);
            objFilter.FromDate = Request.Params["fromDate"].ToString();
            objFilter.ToDate = Request.Params["toDate"].ToString();
            objFilter.TransId = String.IsNullOrEmpty(Request.Params["transType"]) ? (short)0 : Convert.ToInt16(Request.Params["transType"].Split('$')[0]);
            objFilter.page = Convert.ToInt32(homeFormCollection["page"]) - 1;
            objFilter.rows = Convert.ToInt32(homeFormCollection["rows"]);
            objFilter.sidx = homeFormCollection["sidx"].ToString();
            objFilter.sord = homeFormCollection["sord"].ToString();
            objFilter.FilterMode = homeFormCollection["mode"].ToString();
            objFilter.AdminNdCode = PMGSYSession.Current.AdminNdCode;
            objFilter.FundType = PMGSYSession.Current.FundType;
            objFilter.LevelId = PMGSYSession.Current.LevelId;
            objFilter.BillType = BILLTYPE;

            var jsonData = new
            {
                rows = teoBAL.TEOList(objFilter, out totalRecords, true),
                total = totalRecords <= Convert.ToInt32(objFilter.rows) ? 1 : (totalRecords % Convert.ToInt32(objFilter.rows) == 0 ? totalRecords / Convert.ToInt32(objFilter.rows) : totalRecords / Convert.ToInt32(objFilter.rows) + 1),
                page = objFilter.page + 1,
                records = totalRecords
            };
            return Json(jsonData);

        }

        [Audit]
        public ActionResult TOBEntry(String parameter, String hash, String key)
        {
            ViewBag.ImprestEntry = false;

            if (Request.Params["Month"] != null)
            {
                ViewBag.Month = Convert.ToInt32(Request.Params["Month"]);
                ViewBag.Year = Convert.ToInt32(Request.Params["Year"]);
                //change by amol jadhav to identify if imprest entry
                if (Request.Params["ImprestEntry"] != null)
                {
                    ViewBag.ImprestEntry = Convert.ToBoolean(Request.Params["ImprestEntry"]);
                }
            }
            if (parameter == null)
            {
                ViewBag.BILL_ID = 0;
            }
            else
            {
                teoBAL = new TransferEntryOrderBAL();
                string[] strParameters = URLEncrypt.DecryptParameters(new string[] { parameter, hash, key });
                Int64 billId = Convert.ToInt64(strParameters[0]);
                ViewBag.BILL_ID = URLEncrypt.EncryptParameters(new string[] { billId.ToString().Trim() });
                ViewBag.IsFinalize = teoBAL.IsTEOFinalized(billId);
                ViewBag.IsMulTxn = teoBAL.IsMultipleTransactionRequired(billId);
            }
            return View("TOBEntry");
        }

        [Audit]
        public ActionResult TEOMasterForTOB(String parameter, String hash, String key)
        {
            Int16 Month = Convert.ToInt16(Request.Params["Month"]);
            Int16 Year = Convert.ToInt16(Request.Params["Year"]);

            TeoMasterModel teoMasterModel = new TeoMasterModel();
            teoBAL = new TransferEntryOrderBAL();
            commonFuncObj = new CommonFunctions();
            TransactionParams objMaster = new TransactionParams();
            objMaster.BILL_TYPE = BILLTYPE;
            objMaster.FUND_TYPE = PMGSYSession.Current.FundType;
            objMaster.LVL_ID = PMGSYSession.Current.LevelId;
            objMaster.TXN_ID = 0;

            if (parameter == null)// for add new TEO
            {
                teoMasterModel.BILL_MONTH = Convert.ToInt16(Month);
                teoMasterModel.BILL_YEAR = Convert.ToInt16(Year);
                ViewBag.ddlMonth = commonFuncObj.PopulateMonths(Month);
                ViewBag.ddlYear = commonFuncObj.PopulateYears(Year);

                List<SelectListItem> lstTransacrion = commonFuncObj.PopulateTransactions(objMaster);
                //lstTransacrion.RemoveRange(1, 3);

                lstTransacrion = (from TXN in lstTransacrion
                                  where lstTransactionIDs.Any(ID => ID.Value == TXN.Value || TXN.Value == string.Empty)
                                  select TXN).ToList();


                ViewBag.ddlMasterTrans = lstTransacrion;//commonFuncObj.PopulateTransactions(objMaster);
                List<SelectListItem> lstOneItem = new List<SelectListItem>();
                lstOneItem.Insert(0, (new SelectListItem { Text = "Select SubTransaction", Value = "0", Selected = true }));
                ViewBag.ddlSubTrans = lstOneItem;
            }
            else//for edit TEO
            {

                string[] strParameters = URLEncrypt.DecryptParameters(new string[] { parameter, hash, key });
                Int64 billId = Convert.ToInt64(strParameters[0]);
                ViewBag.BILL_ID = URLEncrypt.EncryptParameters(new string[] { billId.ToString().Trim() });
                ACC_BILL_MASTER acc_bill_master = new ACC_BILL_MASTER();
                teoMasterModel = teoBAL.GetTEOMaster(billId);
                if (teoMasterModel.TXN_ID == 172 || teoMasterModel.TXN_ID == 402 || teoMasterModel.TXN_ID == 480 || teoMasterModel.TXN_ID == 864)
                {
                    objMaster.BILL_TYPE = "I";
                }

                List<SelectListItem> lstTransacrion = commonFuncObj.PopulateTransactions(objMaster);

                // lstTransacrion.RemoveRange(1, 3);

                lstTransacrion = (from TXN in lstTransacrion
                                  where lstTransactionIDs.Any(ID => ID.Value == TXN.Value || TXN.Value == string.Empty)
                                  select TXN).ToList();


                ViewBag.ddlMasterTrans = lstTransacrion;//commonFuncObj.PopulateTransactions(objMaster);
                ViewBag.ddlMonth = commonFuncObj.PopulateMonths(Month);
                ViewBag.ddlYear = commonFuncObj.PopulateYears(Year);
                objMaster.TXN_ID = teoMasterModel.TXN_ID;
                ViewBag.ddlSubTrans = commonFuncObj.PopulateTransactions(objMaster);
            }

            //new change done by Abhishek kamble on 21 jan 2014
            teoMasterModel.CURRENT_DATE = DateTime.Now.ToString("dd/MM/yyyy");
            //end Change

            return PartialView("TEOAddMasterForTOB", teoMasterModel);
        }


        [HttpPost]
        [Audit]
        [ValidateAntiForgeryToken]
        public ActionResult AddTEOMasterForTOB(TeoMasterModel teoMasterModel)
        {
            String ValidationSummary = String.Empty;
            commonFuncObj = new CommonFunctions();
            teoBAL = new TransferEntryOrderBAL();


            short levelID = PMGSYSession.Current.LevelId;
            int adminNDCode = PMGSYSession.Current.AdminNdCode;
            string fundType = PMGSYSession.Current.FundType;

            ViewBag.ddlMonth = commonFuncObj.PopulateMonths(teoMasterModel.BILL_MONTH);
            ViewBag.ddlYear = commonFuncObj.PopulateYears(teoMasterModel.BILL_YEAR);

            if (ModelState.IsValid)
            {
                teoMasterModel.FUND_TYPE = fundType;
                teoMasterModel.LVL_ID = levelID;
                teoMasterModel.ADMIN_ND_CODE = adminNDCode;

                string result = string.Empty;
                String message = String.Empty;
                result = commonFuncObj.MonthlyClosingValidation(teoMasterModel.BILL_MONTH, teoMasterModel.BILL_YEAR, PMGSYSession.Current.FundType, PMGSYSession.Current.LevelId, PMGSYSession.Current.AdminNdCode, ref message);
                if (result == "-111")
                {
                    return this.Json(new { success = false, message = message });
                }
                else if ((result == "-222"))
                {
                    return this.Json(new { success = false, message = "Month is already closed,Please revoke the month and try again." });
                }

                if (ValidationSummary == String.Empty)
                {
                    Int64 billId = teoBAL.AddTEOMaster(teoMasterModel);
                    return this.Json(new { success = true, message = URLEncrypt.EncryptParameters(new string[] { billId.ToString().Trim() }) });
                }
                else
                {
                    return this.Json(new { success = false, message = ValidationSummary });
                }

            }
            else
            {
                TransactionParams objMaster = new TransactionParams();
                objMaster.BILL_TYPE = BILLTYPE;
                objMaster.FUND_TYPE = fundType;
                objMaster.LVL_ID = levelID;

                List<SelectListItem> lstTransacrion = commonFuncObj.PopulateTransactions(objMaster);

                // lstTransacrion.RemoveRange(1, 3);

                lstTransacrion = (from TXN in lstTransacrion
                                  where lstTransactionIDs.Any(ID => ID.Value == TXN.Value || TXN.Value == string.Empty)
                                  select TXN).ToList();

                ViewBag.ddlMasterTrans = lstTransacrion; //commonFuncObj.PopulateTransactions(objMaster);
                objMaster.TXN_ID = teoMasterModel.TXN_ID;
                ViewBag.ddlSubTrans = commonFuncObj.PopulateTransactions(objMaster);//lstOneItem;
            }
            return PartialView("TEOAddMasterForTOB", teoMasterModel);
        }


        [Audit]
        public ActionResult TEODetailsForTOB(String parameter, String hash, String key)
        {
            teoBAL = new TransferEntryOrderBAL();
            TransferEntryOrderDAL teoDAL = new TransferEntryOrderDAL();
            commonFuncObj = new CommonFunctions();
            TransactionParams objMaster = new TransactionParams();

            TeoDetailsModel teoDetailsModel = new TeoDetailsModel();

            String[] strParameters = URLEncrypt.DecryptParameters(new string[] { parameter, hash, key });

            Int64 billId = 0;
            String TransId = null;
            Int16 TransNo = 0;

            if (strParameters[0].Contains('$'))
            {
                billId = Convert.ToInt64(strParameters[0].Split('$')[0]);
                TransNo = Convert.ToInt16(strParameters[0].Split('$')[1]);
                teoDetailsModel = teoBAL.GetTEODetailByTransNo(billId, TransNo);
                TransId = URLEncrypt.EncryptParameters(new string[] { billId.ToString().Trim() + "$" + TransNo.ToString().Trim() });

                objMaster.ADMIN_ND_CODE = teoDetailsModel.ADMIN_ND_CODE == null ? PMGSYSession.Current.AdminNdCode : Convert.ToInt32(teoDetailsModel.ADMIN_ND_CODE);
                objMaster.MAST_CONT_ID = teoDetailsModel.MAST_CON_ID == null ? 0 : Convert.ToInt32(teoDetailsModel.MAST_CON_ID);
                objMaster.DISTRICT_CODE = teoDetailsModel.MAST_DISTRICT_CODE == null ? PMGSYSession.Current.DistrictCode : Convert.ToInt32(teoDetailsModel.MAST_DISTRICT_CODE);
                objMaster.AGREEMENT_CODE = teoDetailsModel.IMS_AGREEMENT_CODE == null ? 0 : Convert.ToInt32(teoDetailsModel.IMS_AGREEMENT_CODE);
                objMaster.HEAD_ID = teoDetailsModel.HEAD_ID;
                if (teoDetailsModel.IMS_PR_ROAD_CODE != null && objMaster.AGREEMENT_CODE == 0)
                {
                    objMaster.SANC_YEAR = commonFuncObj.getSancYearFromRoad(Convert.ToInt32(teoDetailsModel.IMS_PR_ROAD_CODE));
                    objMaster.PACKAGE_ID = commonFuncObj.getPackageFromRoad(Convert.ToInt32(teoDetailsModel.IMS_PR_ROAD_CODE));
                }
            }
            else
            {
                billId = Convert.ToInt64(strParameters[0]);
                objMaster.ADMIN_ND_CODE = PMGSYSession.Current.AdminNdCode;
                objMaster.MAST_CONT_ID = 0;
                objMaster.DISTRICT_CODE = PMGSYSession.Current.DistrictCode;

            }
            ViewBag.TransId = TransId;
            Int16 parentId = teoBAL.GetMasterTransId(billId);
            ViewBag.BILL_ID = URLEncrypt.EncryptParameters(new string[] { billId.ToString().Trim() });

            //added by Koustubh Nakate on 17/10/2013 to get txn Id on view page

            ViewBag.ParentTxnId = parentId;


            //Added By Abhishek kamble to populate Inactive Dist for TOB start 10oct2014
            int[] _parentTxnId = { 164, 165, 1194, 1195 };
            bool _populateInactiveDistrictsForC = false;

            if (_parentTxnId.Contains(parentId) && PMGSYSession.Current.StateCode == 2)
            {
                _populateInactiveDistrictsForC = true;
            }
            //Added By Abhishek kamble to populate Inactive Dist for TOB end 10oct2014

            //Added by Abhishek kamble 5Jan2015 for head 21.01 for asset and lib for state AP  start
            int[] _parentTxnIdForHeadTwentyPointOne = { 1664, 1665 };
            if (_parentTxnIdForHeadTwentyPointOne.Contains(parentId) && PMGSYSession.Current.StateCode == 2)
            {
                _populateActiveInactiveDistricts = true;
            }
            else
            {
                _populateActiveInactiveDistricts = false;
            }
            //Added by Abhishek kamble 5Jan2015 for head 21.01 for asset and lib for state AP  end

            //change is for multiple transaction
            #region New Requirement 06/07/2013

            // ACC_BILL_DETAILS acc_bill_details_trans = new ACC_BILL_DETAILS();

            //if (id.Trim() == "C")
            //{
            //    //2)acc_bill_details_trans = teoBAL.GetBillDetails(billId, "D"); // 1 )dbContext.ACC_BILL_DETAILS.Where(m => m.BILL_ID == teoDetailsModel.BILL_ID && m.CREDIT_DEBIT == "D").FirstOrDefault();
            //    acc_bill_details_trans = teoBAL.GetBillDetails(billId, "D"); //change by amol
            //}
            //else
            //{
            //    //2) acc_bill_details_trans = teoBAL.GetBillDetails(billId, "C"); //1)dbContext.ACC_BILL_DETAILS.Where(m => m.BILL_ID == teoDetailsModel.BILL_ID && m.CREDIT_DEBIT == "C").FirstOrDefault();
            //    acc_bill_details_trans = teoBAL.GetBillDetails(billId, "C");//change by amol
            //}

            ACC_TEO_SCREEN_TXN_VALIDATIONS validationParams = new ACC_TEO_SCREEN_TXN_VALIDATIONS();

            validationParams = commonFuncObj.GetTEOValidationParams(0, parentId);

            #endregion

            ACC_TEO_SCREEN_DESIGN_PARAM_DETAILS designParams = new ACC_TEO_SCREEN_DESIGN_PARAM_DETAILS();
            designParams = commonFuncObj.getTEODesignParamDetails(0, parentId);

            if (designParams == null)
            {
                designParams = teoDAL.setDefualtDesignTransParam();
            }

            objMaster.BILL_TYPE = BILLTYPE;
            objMaster.FUND_TYPE = PMGSYSession.Current.FundType;
            objMaster.LVL_ID = PMGSYSession.Current.LevelId;
            objMaster.TXN_ID = parentId;
            objMaster.BILL_ID = billId;
            objMaster.STATE_CODE = PMGSYSession.Current.StateCode;

            // objMaster.CREDIT_DEBIT = id.Trim();

            if (designParams.CON_REQ.Equals("Y"))
            {
                objMaster.MAST_CON_SUP_FLAG = "C";
            }
            else if (designParams.SUPP_REQ.Equals("Y"))
            {
                objMaster.MAST_CON_SUP_FLAG = "S";
            }

            if (designParams.DISTRICT_REQ == "Y")
            {

                //Avinash.07/01/2019...populate all Active/InActive District in Case of Andhra Pradesh..StateCode=2..Discuss with Mam
                if (objMaster.STATE_CODE == 2)
                {

                    int[] TxnId = { 163, 164, 165, 1187, 1550, 1664, 1664 };
                    if (TxnId.Contains(parentId) && PMGSYSession.Current.StateCode == 2)
                    {

                        ViewBag.ddlDistrictC = commonFuncObj.PopulateDistrict(objMaster.STATE_CODE, false, 0, true, true);
                        ViewBag.ddlDistrictD = commonFuncObj.PopulateDistrict(objMaster.STATE_CODE, false, 0, true, true);
                    }
                }
                else
                {
                    if (validationParams.IS_DISTRICT_REPEAT == "Y")
                    {
                        //old modified by Abhishek kamble 10oct2014
                        //ViewBag.ddlDistrict = commonFuncObj.PopulateDistrict(objMaster.STATE_CODE, false);
                        if (_populateInactiveDistrictsForC)
                        {
                            ViewBag.ddlDistrictC = commonFuncObj.PopulateDistrict(objMaster.STATE_CODE, false, 0, true);//Added by Abhishek kamble 5Jan2015 for head 21.01 for asset and lib for state AP 
                            ViewBag.ddlDistrictD = commonFuncObj.PopulateDistrict(objMaster.STATE_CODE, false);
                        }
                        else
                        {
                            ViewBag.ddlDistrictC = commonFuncObj.PopulateDistrict(objMaster.STATE_CODE, false, 0, false, _populateActiveInactiveDistricts);//_populateActiveInactiveDistricts - Added by Abhishek kamble 5Jan2015 for head 21.01 for asset and lib for state AP
                            ViewBag.ddlDistrictD = commonFuncObj.PopulateDistrict(objMaster.STATE_CODE, false, 0, false, _populateActiveInactiveDistricts);//_populateActiveInactiveDistricts - Added by Abhishek kamble 5Jan2015 for head 21.01 for asset and lib for state AP 
                        }
                    }
                    else
                    {
                        //old modified by Abhishek kamble 10oct2014      
                        // ViewBag.ddlDistrict = commonFuncObj.PopulateDistrict(objMaster.STATE_CODE);
                        if (_populateInactiveDistrictsForC)
                        {
                            ViewBag.ddlDistrictC = commonFuncObj.PopulateDistrict(objMaster.STATE_CODE, false, 0, true);
                            ViewBag.ddlDistrictD = commonFuncObj.PopulateDistrict(objMaster.STATE_CODE);
                        }
                        else
                        {
                            ViewBag.ddlDistrictC = commonFuncObj.PopulateDistrict(objMaster.STATE_CODE, false, 0, true, _populateActiveInactiveDistricts);//_populateActiveInactiveDistricts - Added by Abhishek kamble 5Jan2015 for head 21.01 for asset and lib for state AP 
                            ViewBag.ddlDistrictD = commonFuncObj.PopulateDistrict(objMaster.STATE_CODE, false, 0, true, _populateActiveInactiveDistricts);//_populateActiveInactiveDistricts- Added by Abhishek kamble 5Jan2015 for head 21.01 for asset and lib for state AP 
                        }
                    }

                }
            }
            else
            {
                ViewBag.ddlDistrict = new SelectList(new List<SelectListItem>().AsEnumerable<SelectListItem>(), "Value", "Text");
            }

            if (designParams.DPIU_REQ == "Y")
            {
                if (validationParams.IS_DPIU_REPEAT == "Y" && TransNo == 0)
                {
                    List<SelectListItem> lstOneItem = new List<SelectListItem>();
                    lstOneItem.Insert(0, (new SelectListItem { Text = "Select Department", Value = "0", Selected = true }));
                    ViewBag.ddlDPIU = lstOneItem;
                    ViewBag.ddlDPIU_D = lstOneItem;
                }
                else
                {
                    ViewBag.ddlDPIU = commonFuncObj.PopulateDPIU(objMaster);
                    ViewBag.ddlDPIU_D = commonFuncObj.PopulateDPIU(objMaster);
                }
            }
            else
            {
                ViewBag.ddlDPIU = new SelectList(new List<SelectListItem>().AsEnumerable<SelectListItem>(), "Value", "Text");
                ViewBag.ddlDPIU_D = new SelectList(new List<SelectListItem>().AsEnumerable<SelectListItem>(), "Value", "Text");
            }

            if (designParams.CON_REQ == "Y" || designParams.SUPP_REQ == "Y")
            {
                ViewBag.ddlContractor = commonFuncObj.PopulateContractorSupplier(objMaster);
            }
            else
            {
                ViewBag.ddlContractor = new SelectList(new List<SelectListItem>().AsEnumerable<SelectListItem>(), "Value", "Text");
            }

            if (designParams.AFREEMENT_REQ == "Y")
            {
                //if (acc_bill_details_trans != null)
                //{
                //    Int32 tmpAggrementCode = objMaster.AGREEMENT_CODE;
                //    Int32 tmpDistrict = objMaster.DISTRICT_CODE;
                //    Int32 tmpContID = objMaster.MAST_CONT_ID;
                //    objMaster.DISTRICT_CODE = PMGSYSession.Current.LevelId == 5 ? PMGSYSession.Current.DistrictCode : Convert.ToInt32(acc_bill_details_trans.MAST_DISTRICT_CODE);
                //    objMaster.MAST_CONT_ID = Convert.ToInt32(acc_bill_details_trans.MAST_CON_ID);
                //    objMaster.AGREEMENT_CODE = Convert.ToInt32(acc_bill_details_trans.IMS_AGREEMENT_CODE);
                //    ViewBag.ddlAgreement = commonFuncObj.PopulateAgreement(objMaster);
                //    objMaster.AGREEMENT_CODE = tmpAggrementCode;
                //    objMaster.MAST_CONT_ID = tmpContID;
                //    objMaster.DISTRICT_CODE = tmpDistrict;
                //}
                if (TransNo == 0)
                {
                    List<SelectListItem> lstOneItem = new List<SelectListItem>();
                    lstOneItem.Insert(0, (new SelectListItem { Text = "Select Agreement", Value = "0", Selected = true }));
                    ViewBag.ddlAgreement = lstOneItem;
                }
                else
                {
                    ViewBag.ddlAgreement = commonFuncObj.PopulateAgreement(objMaster);
                }
            }
            else
            {
                ViewBag.ddlAgreement = new SelectList(new List<SelectListItem>().AsEnumerable<SelectListItem>(), "Value", "Text");
            }

            if (designParams.SAN_REQ == "Y")
            {
                ViewBag.ddlSancYear = commonFuncObj.PopulateSancYear(objMaster);
            }
            else
            {
                ViewBag.ddlSancYear = new SelectList(new List<SelectListItem>().AsEnumerable<SelectListItem>(), "Value", "Text");
            }

            //designParams.ROAD_REQ condition necessary bcoz it will populate unnecessarily 
            //if (designParams.ROAD_REQ == "Y" && validationParams.IS_ROAD_REPEAT == "Y" )
            //{
            //    Int32 tmpAggCode = objMaster.AGREEMENT_CODE;
            //    objMaster.AGREEMENT_CODE = Convert.ToInt32(acc_bill_details_trans.IMS_AGREEMENT_CODE);
            //    objMaster.ROAD_CODE = Convert.ToInt32(acc_bill_details_trans.IMS_PR_ROAD_CODE);
            //    ViewBag.ddlRoad = commonFuncObj.PopulateRoad(objMaster);
            //    objMaster.AGREEMENT_CODE = tmpAggCode;
            //}
            //else if (designParams.ROAD_REQ == "Y" && validationParams.IS_AGREEMENT_REPEAT == "Y" && acc_bill_details_trans != null && designParams.AFREEMENT_REQ == "Y")
            //{
            //    Int32 tmpAggCode = objMaster.AGREEMENT_CODE;
            //    objMaster.AGREEMENT_CODE = Convert.ToInt32(acc_bill_details_trans.IMS_AGREEMENT_CODE);
            //    objMaster.ROAD_CODE = 0;
            //    ViewBag.ddlRoad = commonFuncObj.PopulateRoad(objMaster);
            //    objMaster.AGREEMENT_CODE = tmpAggCode;
            //}
            //else if (designParams.ROAD_REQ == "Y" && acc_bill_details_trans != null)
            //{
            //    Int32 tmpAggCode = objMaster.AGREEMENT_CODE;
            //    objMaster.AGREEMENT_CODE = Convert.ToInt32(acc_bill_details_trans.IMS_AGREEMENT_CODE);
            //    objMaster.ROAD_CODE = 0;
            //    ViewBag.ddlRoad = commonFuncObj.PopulateRoad(objMaster);
            //    objMaster.AGREEMENT_CODE = tmpAggCode;
            //}
            if (designParams.ROAD_REQ == "Y")
            {
                ViewBag.ddlRoad = commonFuncObj.PopulateRoad(objMaster);
            }
            else
            {
                ViewBag.ddlRoad = new SelectList(new List<SelectListItem>().AsEnumerable<SelectListItem>(), "Value", "Text");
            }

            if (designParams.PKG_REQ == "Y")
            {
                if (TransNo == 0)
                {
                    List<SelectListItem> lstOneItem = new List<SelectListItem>();
                    lstOneItem.Insert(0, (new SelectListItem { Text = "Select Package", Value = "0", Selected = true }));
                    ViewBag.ddlPackage = lstOneItem;
                }
                else
                {
                    ViewBag.ddlPackage = commonFuncObj.PopulatePackage(objMaster);
                }
            }
            else
            {
                ViewBag.ddlPackage = new SelectList(new List<SelectListItem>().AsEnumerable<SelectListItem>(), "Value", "Text");
            }

            if (validationParams.IS_HEAD_REPEAT == "Y")
            {
                objMaster.CREDIT_DEBIT = "C";
                ViewBag.ddlHead_C = commonFuncObj.PopulateHead(objMaster);

                objMaster.CREDIT_DEBIT = "D";
                ViewBag.ddlHead_D = commonFuncObj.PopulateHead(objMaster);
            }
            else
            {
                objMaster.CREDIT_DEBIT = "C";
                ViewBag.ddlHead_C = commonFuncObj.PopulateHead(objMaster);

                objMaster.CREDIT_DEBIT = "D";
                ViewBag.ddlHead_D = commonFuncObj.PopulateHead(objMaster);

                //ViewBag.ddlHead = commonFuncObj.PopulateHead(objMaster);
            }

            if (teoDetailsModel.HEAD_ID != 0)
            {
                ACC_TEO_SCREEN_DESIGN_HEADWISE_DETAILS headParams = new ACC_TEO_SCREEN_DESIGN_HEADWISE_DETAILS();
                headParams = commonFuncObj.GetHeadwiseDesignParams(objMaster);
                if (headParams == null)
                {
                    headParams = teoDAL.setDefualtDesignHeadParam();
                }
                if (headParams.CON_REQ == "Y")
                {
                    ViewBag.ddlHeadContractor = commonFuncObj.PopulateContractorSupplier(objMaster);
                }
                else
                {
                    ViewBag.ddlHeadContractor = new SelectList(new List<SelectListItem>().AsEnumerable<SelectListItem>(), "Value", "Text");
                }
                if (headParams.AGREEMENT_REQ == "Y")
                {
                    ViewBag.ddlHeadAgreement = commonFuncObj.PopulateAgreement(objMaster);
                }
                else
                {
                    ViewBag.ddlHeadAgreement = new SelectList(new List<SelectListItem>().AsEnumerable<SelectListItem>(), "Value", "Text");
                }
                if (headParams.SANC_YEAR_REQ == "Y")
                {
                    ViewBag.ddlHeadSancYear = commonFuncObj.PopulateSancYear(objMaster);
                }
                else
                {
                    ViewBag.ddlHeadSancYear = new SelectList(new List<SelectListItem>().AsEnumerable<SelectListItem>(), "Value", "Text");
                }
                if (headParams.ROAD_REQ == "Y")
                {
                    ViewBag.ddlHeadRoad = commonFuncObj.PopulateRoad(objMaster);
                }
                else
                {
                    ViewBag.ddlHeadRoad = new SelectList(new List<SelectListItem>().AsEnumerable<SelectListItem>(), "Value", "Text");
                }
                if (headParams.PKG_REQ == "Y")
                {
                    ViewBag.ddlHeadPackage = commonFuncObj.PopulatePackage(objMaster);
                }
                else
                {
                    ViewBag.ddlHeadPackage = new SelectList(new List<SelectListItem>().AsEnumerable<SelectListItem>(), "Value", "Text");
                }
            }
            else
            {
                ACC_TEO_SCREEN_DESIGN_HEADWISE_DETAILS headParams = new ACC_TEO_SCREEN_DESIGN_HEADWISE_DETAILS();
                //if (acc_bill_details_trans != null)
                //{
                //    objMaster.HEAD_ID = acc_bill_details_trans.HEAD_ID;
                //}
                headParams = commonFuncObj.GetHeadwiseDesignParams(objMaster);
                objMaster.HEAD_ID = teoDetailsModel.HEAD_ID;
                if (headParams == null)
                {
                    headParams = teoDAL.setDefualtDesignHeadParam();
                }

                if (headParams.CON_REQ == "Y")
                {
                    // objMaster.MAST_CONT_ID = Convert.ToInt32(acc_bill_details_trans.MAST_CON_ID);
                    ViewBag.ddlHeadContractor = commonFuncObj.PopulateContractorSupplier(objMaster);
                    objMaster.MAST_CONT_ID = 0;
                }
                else
                {
                    ViewBag.ddlHeadContractor = new SelectList(new List<SelectListItem>().AsEnumerable<SelectListItem>(), "Value", "Text");
                }

                if (headParams.AGREEMENT_REQ == "Y")
                {
                    Int32 tmpAggrementCode = objMaster.AGREEMENT_CODE;
                    Int32 tmpDistrict = objMaster.DISTRICT_CODE;
                    Int32 tmpContID = objMaster.MAST_CONT_ID;

                    objMaster.DISTRICT_CODE = PMGSYSession.Current.LevelId == 5 ? PMGSYSession.Current.DistrictCode : Convert.ToInt32(teoDetailsModel.MAST_DISTRICT_CODE); ;
                    objMaster.MAST_CONT_ID = Convert.ToInt32(teoDetailsModel.MAST_CON_ID);
                    objMaster.AGREEMENT_CODE = Convert.ToInt32(teoDetailsModel.IMS_AGREEMENT_CODE);
                    ViewBag.ddlHeadAgreement = commonFuncObj.PopulateAgreement(objMaster);
                    objMaster.AGREEMENT_CODE = tmpAggrementCode;
                    objMaster.MAST_CONT_ID = tmpContID;
                    objMaster.DISTRICT_CODE = tmpDistrict;
                }
                else
                {
                    ViewBag.ddlHeadAgreement = new SelectList(new List<SelectListItem>().AsEnumerable<SelectListItem>(), "Value", "Text");
                }


                ViewBag.ddlHeadSancYear = new SelectList(new List<SelectListItem>().AsEnumerable<SelectListItem>(), "Value", "Text");

                ViewBag.ddlHeadPackage = new SelectList(new List<SelectListItem>().AsEnumerable<SelectListItem>(), "Value", "Text");

                //if (validationParams.IS_ROAD_REPEAT == "Y" && acc_bill_details_trans != null && headParams.ROAD_REQ == "Y")
                //{
                //    Int32 tmpAggCode = objMaster.AGREEMENT_CODE;
                //    objMaster.AGREEMENT_CODE = Convert.ToInt32(acc_bill_details_trans.IMS_AGREEMENT_CODE);
                //    objMaster.ROAD_CODE = Convert.ToInt32(acc_bill_details_trans.IMS_PR_ROAD_CODE);
                //    ViewBag.ddlHeadRoad = commonFuncObj.PopulateRoad(objMaster);
                //    objMaster.AGREEMENT_CODE = tmpAggCode;
                //}
                //else if (headParams.ROAD_REQ == "Y" && validationParams.IS_AGREEMENT_REPEAT == "Y" && acc_bill_details_trans != null && headParams.AGREEMENT_REQ == "Y")
                //{
                //    Int32 tmpAggCode = objMaster.AGREEMENT_CODE;
                //    objMaster.AGREEMENT_CODE = Convert.ToInt32(acc_bill_details_trans.IMS_AGREEMENT_CODE);
                //    objMaster.ROAD_CODE = 0;
                //    ViewBag.ddlHeadRoad = commonFuncObj.PopulateRoad(objMaster);
                //    objMaster.AGREEMENT_CODE = tmpAggCode;
                //}

                //else
                //{
                //ViewBag.ddlHeadRoad = new SelectList(new List<SelectListItem>().AsEnumerable<SelectListItem>(), "Value", "Text");
                //}

                ViewBag.ddlHeadRoad = new SelectList(new List<SelectListItem>().AsEnumerable<SelectListItem>(), "Value", "Text");

            }
            //ViewBag.CreditDebit = id.Trim();

            if (teoDetailsModel.MAST_CON_ID_TRANS == null && teoDetailsModel.MAST_CON_ID != null)
            {
                teoDetailsModel.MAST_CON_ID_TRANS = teoDetailsModel.MAST_CON_ID;
            }


            TeoDetailsModelForTOB teoDetailsModelForTOB = new TeoDetailsModelForTOB()
            {
                BILL_ID = teoDetailsModel.BILL_ID,
                TXN_NO = teoDetailsModel.TXN_NO,
                TXN_ID = teoDetailsModel.TXN_ID,
                HEAD_ID_C = teoDetailsModel.HEAD_ID,
                HEAD_ID_D = teoDetailsModel.HEAD_ID,
                AMOUNT_C = teoDetailsModel.AMOUNT,
                AMOUNT_D = teoDetailsModel.AMOUNT,
                NARRATION_C = teoDetailsModel.NARRATION,
                NARRATION_D = teoDetailsModel.NARRATION,
                ADMIN_ND_CODE_C = teoDetailsModel.ADMIN_ND_CODE,
                ADMIN_ND_CODE_D = teoDetailsModel.ADMIN_ND_CODE,
                MAST_CON_ID_C = teoDetailsModel.MAST_CON_ID,
                MAST_CON_ID_D = teoDetailsModel.MAST_CON_ID,
                MAST_CON_ID_TRANS_C = teoDetailsModel.MAST_CON_ID_TRANS,
                MAST_CON_ID_TRANS_D = teoDetailsModel.MAST_CON_ID_TRANS,
                IMS_PR_ROAD_CODE_C = teoDetailsModel.IMS_PR_ROAD_CODE,
                IMS_PR_ROAD_CODE_D = teoDetailsModel.IMS_PR_ROAD_CODE,
                IMS_AGREEMENT_CODE_C = teoDetailsModel.IMS_AGREEMENT_CODE,
                IMS_AGREEMENT_CODE_D = teoDetailsModel.IMS_AGREEMENT_CODE,
                MAST_DISTRICT_CODE_C = teoDetailsModel.MAST_DISTRICT_CODE,
                MAST_DISTRICT_CODE_D = teoDetailsModel.MAST_DISTRICT_CODE,
                IMS_PACKAGE_ID_C = teoDetailsModel.IMS_PACKAGE_ID,
                IMS_PACKAGE_ID_D = teoDetailsModel.IMS_PACKAGE_ID,
                SANC_YEAR_C = teoDetailsModel.SANC_YEAR,
                SANC_YEAR_D = teoDetailsModel.SANC_YEAR,
                FINAL_PAYMENT_C = teoDetailsModel.FINAL_PAYMENT,
                FINAL_PAYMENT_D = teoDetailsModel.FINAL_PAYMENT
            };

            //Added By Abhishek kamble to populate State drop down   1 Oct 2014
            ViewBag.ddlState = commonFuncObj.PopulateStates(true);
            teoDetailsModelForTOB.MAST_STATE_CODE_C = PMGSYSession.Current.StateCode;
            return PartialView("TEOAddDetailsForTOB", teoDetailsModelForTOB);
        }


        [HttpPost]
        [Audit]
        [ValidateAntiForgeryToken]
        public ActionResult AddCreditDebitTEODetailsForTOB(TeoDetailsModelForTOB teoDetailsModelForTOB, String parameter, String hash, String key)
        {
            teoBAL = new TransferEntryOrderBAL();
            TransferEntryOrderDAL teoDAL = new TransferEntryOrderDAL();
            String ValidationSummary = String.Empty;
            CommonFunctions commomFuncObj = new CommonFunctions();
            Int16 transId = 0;
            string[] strParameters = URLEncrypt.DecryptParameters(new string[] { parameter, hash, key });
            if (strParameters.Length == 0)
            {
                return this.Json(new { success = false, message = "Error while adding TEO Credit and Debit Details" });
            }
            else
            {
                teoDetailsModelForTOB.BILL_ID = Convert.ToInt64(strParameters[0]);
            }

            if (teoDetailsModelForTOB.HEAD_ID_C == 29 || teoDetailsModelForTOB.HEAD_ID_C == 28)
            {
                Int32 roadCode = 0;
                string errorKey = string.Empty;
                if (teoDetailsModelForTOB.IMS_PR_ROAD_CODE_C != 0 && teoDetailsModelForTOB.IMS_PR_ROAD_CODE_C != null)
                {
                    roadCode = teoDetailsModelForTOB.IMS_PR_ROAD_CODE_C.Value;
                    errorKey = "IMS_PR_ROAD_CODE";
                }

                //remove validation for txn 1550-Balance Transfer between the road heads of same piu  by Abhishek 7Apr2015 if condition is added
                if (teoDetailsModelForTOB._ParentTxnID != 1550)
                {
                    if (ValidateRoad(teoDetailsModelForTOB.HEAD_ID_C, roadCode, PMGSYSession.Current.FundType) == false)
                    {
                        ModelState.AddModelError(errorKey, "The selected head is not valid for this road.");
                        return Json(new { success = false, status = "-555", message = "Select Upgrade/New road as per account head in Credit side." });
                    }
                }
            }

            if (teoDetailsModelForTOB.HEAD_ID_D == 29 || teoDetailsModelForTOB.HEAD_ID_D == 28)
            {
                Int32 roadCode = 0;
                string errorKey = string.Empty;
                if (teoDetailsModelForTOB.IMS_PR_ROAD_CODE_D != 0 && teoDetailsModelForTOB.IMS_PR_ROAD_CODE_D != null)
                {
                    roadCode = teoDetailsModelForTOB.IMS_PR_ROAD_CODE_D.Value;
                    errorKey = "IMS_PR_ROAD_CODE";
                }

                //remove validation for txn 1550-Balance Transfer between the road heads of same piu  by Abhishek 7Apr2015 if condition is added
                if (teoDetailsModelForTOB._ParentTxnID != 1550)
                {
                    if (ValidateRoad(teoDetailsModelForTOB.HEAD_ID_D, roadCode, PMGSYSession.Current.FundType) == false)
                    {
                        ModelState.AddModelError(errorKey, "The selected head is not valid for this road.");
                        return Json(new { success = false, status = "-555", message = "Select Upgrade/New road as per account head in Debit side." });
                    }
                }
            }

            //New validation to validate PMGSY scheme Roads based on Head. start 18 Sep 2014
            if (PMGSYSession.Current.FundType == "P" && teoDetailsModelForTOB.IMS_PR_ROAD_CODE_C != null && teoDetailsModelForTOB.IMS_PR_ROAD_CODE_C != 0)
            {
                if (!ValidateRoadForPMGSYScheme(teoDetailsModelForTOB.HEAD_ID_C, teoDetailsModelForTOB.IMS_PR_ROAD_CODE_C.Value))
                {
                    return Json(new { success = false, message = "Road can not be Selected, Since it is in different Scheme." });
                }
            }
            if (PMGSYSession.Current.FundType == "P" && teoDetailsModelForTOB.IMS_PR_ROAD_CODE_D != null && teoDetailsModelForTOB.IMS_PR_ROAD_CODE_D != 0)
            {
                if (!ValidateRoadForPMGSYScheme(teoDetailsModelForTOB.HEAD_ID_D, teoDetailsModelForTOB.IMS_PR_ROAD_CODE_D.Value))
                {
                    return Json(new { success = false, message = "Road can not be Selected, Since it is in different Scheme." });
                }
            }
            //New validation to validate PMGSY scheme Roads based on Head. end


            List<ModelErrorList> lstModelErrorList = new List<ModelErrorList>();
            lstModelErrorList = teoDAL.GetAddTEODetailsModelErrorsForTOB(teoDetailsModelForTOB);
            if (lstModelErrorList.Count != 0)
            {
                foreach (ModelErrorList item in lstModelErrorList)
                {
                    ModelState.AddModelError(item.Key, item.Value);
                }

            }


            //remove validation errors for state transfer start 9 oct 2014

            int[] _parentTxnId = { 164, 165, 1194, 1195 };
            if (!(_parentTxnId.Contains(teoDetailsModelForTOB._ParentTxnID)))
            {
                if (ModelState.ContainsKey("MAST_STATE_CODE_C"))
                    ModelState["MAST_STATE_CODE_C"].Errors.Clear();

                if (ModelState.ContainsKey("MAST_STATE_CODE_D"))
                    ModelState["MAST_STATE_CODE_D"].Errors.Clear();
            }

            //remove validation errors for state transfer start

            if (ModelState.IsValid)
            {
                //ValidationSummary = receiptBAL.ValidateAddReceiptDetails(billDetailsViewModel);
                if (ValidationSummary == String.Empty)
                {
                    string success = teoBAL.AddCreditDebitTEODetailsforTOB(teoDetailsModelForTOB, out transId);
                    ViewBag.TRANS_ID = transId;

                    // to check if correct entry as is operational and is required after porting flag
                    string correctionStatus = commomFuncObj.ValidateHeadForCorrection(Convert.ToInt16(teoDetailsModelForTOB.TXN_ID), 0, "A");
                    if (correctionStatus != "1")
                    {
                        return this.Json(new { success = false, message = "Invalid Transaction Type" });
                    }

                    if (success == String.Empty)
                    {
                        return this.Json(new { success = true, message = URLEncrypt.EncryptParameters(new string[] { teoDetailsModelForTOB.BILL_ID.ToString().Trim() }) });
                    }
                    else
                    {
                        return this.Json(new { success = false, message = URLEncrypt.EncryptParameters(new string[] { teoDetailsModelForTOB.BILL_ID.ToString().Trim() }) });
                    }
                }
                else
                {
                    return this.Json(new { success = false, message = ValidationSummary });
                }
            }
            else
            {

                ViewBag.BILL_ID = URLEncrypt.EncryptParameters(new string[] { teoDetailsModelForTOB.BILL_ID.ToString().Trim() });
                ViewBag.TransId = null;

                TransactionParams objMaster = new TransactionParams();

                commonFuncObj = new CommonFunctions();
                objMaster.ADMIN_ND_CODE = teoDetailsModelForTOB.ADMIN_ND_CODE_C == null ? PMGSYSession.Current.AdminNdCode : Convert.ToInt32(teoDetailsModelForTOB.ADMIN_ND_CODE_C);
                objMaster.MAST_CONT_ID = teoDetailsModelForTOB.MAST_CON_ID_C == null ? 0 : Convert.ToInt32(teoDetailsModelForTOB.MAST_CON_ID_C);
                objMaster.DISTRICT_CODE = teoDetailsModelForTOB.MAST_DISTRICT_CODE_C == null ? PMGSYSession.Current.DistrictCode : Convert.ToInt32(teoDetailsModelForTOB.MAST_DISTRICT_CODE_C);
                objMaster.AGREEMENT_CODE = teoDetailsModelForTOB.IMS_AGREEMENT_CODE_C == null ? 0 : Convert.ToInt32(teoDetailsModelForTOB.IMS_AGREEMENT_CODE_C);
                objMaster.SANC_YEAR = teoDetailsModelForTOB.SANC_YEAR_C == null ? (Int16)0 : Convert.ToInt16(teoDetailsModelForTOB.SANC_YEAR_C);
                objMaster.PACKAGE_ID = teoDetailsModelForTOB.IMS_PACKAGE_ID_C == null ? "0" : teoDetailsModelForTOB.IMS_PACKAGE_ID_C;
                objMaster.HEAD_ID = teoDetailsModelForTOB.HEAD_ID_C;

                if (teoDetailsModelForTOB.SANC_YEAR_C != 0)
                {
                    objMaster.SANC_YEAR = commonFuncObj.getSancYearFromRoad(Convert.ToInt32(teoDetailsModelForTOB.IMS_PR_ROAD_CODE_C));
                    objMaster.PACKAGE_ID = commonFuncObj.getPackageFromRoad(Convert.ToInt32(teoDetailsModelForTOB.IMS_PR_ROAD_CODE_C));
                }
                Int16 parentId = teoBAL.GetMasterTransId(teoDetailsModelForTOB.BILL_ID);
                //ACC_BILL_DETAILS acc_bill_details_trans = new ACC_BILL_DETAILS();

                //acc_bill_details_trans = teoBAL.GetBillDetails(teoDetailsModel.BILL_ID, "D"); //dbContext.ACC_BILL_DETAILS.Where(m => m.BILL_ID == teoDetailsModel.BILL_ID && m.CREDIT_DEBIT == "D").FirstOrDefault();


                ACC_TEO_SCREEN_TXN_VALIDATIONS validationParams = new ACC_TEO_SCREEN_TXN_VALIDATIONS();

                validationParams = commonFuncObj.GetTEOValidationParams(0, parentId);


                ACC_TEO_SCREEN_DESIGN_PARAM_DETAILS designParams = new ACC_TEO_SCREEN_DESIGN_PARAM_DETAILS();
                designParams = commonFuncObj.getTEODesignParamDetails(0, parentId);

                if (designParams == null)
                {
                    designParams = teoDAL.setDefualtDesignTransParam();
                }

                objMaster.BILL_TYPE = BILLTYPE;
                objMaster.FUND_TYPE = PMGSYSession.Current.FundType;
                objMaster.LVL_ID = PMGSYSession.Current.LevelId;
                objMaster.TXN_ID = parentId;
                objMaster.BILL_ID = teoDetailsModelForTOB.BILL_ID;
                objMaster.STATE_CODE = PMGSYSession.Current.StateCode;


                //added by Koustubh Nakate on 17/10/2013 to get txn Id on view page

                ViewBag.ParentTxnId = parentId;

                //Added By Abhishek kamble to populate Inactive Dist for TOB start 10oct2014                
                bool _populateInactiveDistrictsForC = false;

                if (_parentTxnId.Contains(parentId) && PMGSYSession.Current.StateCode == 2)
                {
                    _populateInactiveDistrictsForC = true;
                }
                //Added By Abhishek kamble to populate Inactive Dist for TOB end 10oct2014

                objMaster.CREDIT_DEBIT = "C";

                if (designParams.CON_REQ == "Y")
                {
                    objMaster.MAST_CON_SUP_FLAG = "C";
                }
                else if (designParams.SUPP_REQ == "Y")
                {
                    objMaster.MAST_CON_SUP_FLAG = "S";
                }

                if (designParams.DISTRICT_REQ == "Y")
                {
                    if (validationParams.IS_DISTRICT_REPEAT == "Y")
                    {
                        //old modified by Abhishek kamble 10oct2014                        
                        //ViewBag.ddlDistrict = commonFuncObj.PopulateDistrict(objMaster.STATE_CODE, false, Convert.ToInt32(teoDetailsModelForTOB.MAST_DISTRICT_CODE_C));

                        if (_populateInactiveDistrictsForC)
                        {
                            ViewBag.ddlDistrictC = commonFuncObj.PopulateDistrict(objMaster.STATE_CODE, false, Convert.ToInt32(teoDetailsModelForTOB.MAST_DISTRICT_CODE_C), true);
                            ViewBag.ddlDistrictD = commonFuncObj.PopulateDistrict((teoDetailsModelForTOB.MAST_STATE_CODE_D), false, Convert.ToInt32(teoDetailsModelForTOB.MAST_DISTRICT_CODE_D));
                        }
                        else
                        {
                            ViewBag.ddlDistrictC = commonFuncObj.PopulateDistrict(objMaster.STATE_CODE, false, Convert.ToInt32(teoDetailsModelForTOB.MAST_DISTRICT_CODE_C));
                            ViewBag.ddlDistrictD = commonFuncObj.PopulateDistrict(objMaster.STATE_CODE, false, Convert.ToInt32(teoDetailsModelForTOB.MAST_DISTRICT_CODE_C));
                        }


                    }
                    else
                    {
                        //old   modified by Abhishek kamble 10oct2014  
                        //ViewBag.ddlDistrict = commonFuncObj.PopulateDistrict(objMaster.STATE_CODE);

                        if (_populateInactiveDistrictsForC)
                        {
                            ViewBag.ddlDistrictC = commonFuncObj.PopulateDistrict(objMaster.STATE_CODE, false, 0, true);
                            ViewBag.ddlDistrictD = commonFuncObj.PopulateDistrict(teoDetailsModelForTOB.MAST_STATE_CODE_D);
                        }
                        else
                        {
                            ViewBag.ddlDistrictC = commonFuncObj.PopulateDistrict(objMaster.STATE_CODE);
                            ViewBag.ddlDistrictD = commonFuncObj.PopulateDistrict(objMaster.STATE_CODE);
                        }

                    }
                }
                else
                {
                    //old      modified by Abhishek kamble 10oct2014  
                    //ViewBag.ddlDistrict = new SelectList(new List<SelectListItem>().AsEnumerable<SelectListItem>(), "Value", "Text");
                    ViewBag.ddlDistrictC = new SelectList(new List<SelectListItem>().AsEnumerable<SelectListItem>(), "Value", "Text");
                    ViewBag.ddlDistrictD = new SelectList(new List<SelectListItem>().AsEnumerable<SelectListItem>(), "Value", "Text");
                }

                if (designParams.DPIU_REQ == "Y")
                {
                    if (validationParams.IS_DPIU_REPEAT == "Y")
                    {
                        Int32 tmpDistrict = objMaster.DISTRICT_CODE;
                        Int32 tmpAdminCode = objMaster.ADMIN_ND_CODE;
                        if (validationParams.IS_DPIU_REPEAT == "Y")
                        {
                            objMaster.DISTRICT_CODE = PMGSYSession.Current.LevelId == 5 ? PMGSYSession.Current.DistrictCode : Convert.ToInt32(teoDetailsModelForTOB.MAST_DISTRICT_CODE_C);
                            objMaster.ADMIN_ND_CODE = Convert.ToInt32(teoDetailsModelForTOB.ADMIN_ND_CODE_C);
                            ViewBag.ddlDPIU = commonFuncObj.PopulateDPIU(objMaster);
                            ViewBag.ddlDPIU_D = commonFuncObj.PopulateDPIU(objMaster);
                        }
                        else
                        {
                            objMaster.DISTRICT_CODE = PMGSYSession.Current.LevelId == 5 ? PMGSYSession.Current.DistrictCode : Convert.ToInt32(teoDetailsModelForTOB.MAST_DISTRICT_CODE_C);
                            ViewBag.ddlDPIU = commonFuncObj.PopulateDPIU(objMaster);
                            ViewBag.ddlDPIU_D = commonFuncObj.PopulateDPIU(objMaster);
                        }
                        objMaster.ADMIN_ND_CODE = tmpAdminCode;
                        objMaster.DISTRICT_CODE = tmpDistrict;
                    }
                    else
                    {
                        ViewBag.ddlDPIU = commonFuncObj.PopulateDPIU(objMaster);

                        objMaster.DISTRICT_CODE = teoDetailsModelForTOB.MAST_DISTRICT_CODE_D == null ? PMGSYSession.Current.DistrictCode : Convert.ToInt32(teoDetailsModelForTOB.MAST_DISTRICT_CODE_D);
                        ViewBag.ddlDPIU_D = commonFuncObj.PopulateDPIU(objMaster);
                    }
                }
                else
                {
                    ViewBag.ddlDPIU = new SelectList(new List<SelectListItem>().AsEnumerable<SelectListItem>(), "Value", "Text");

                    ViewBag.ddlDPIU_D = new SelectList(new List<SelectListItem>().AsEnumerable<SelectListItem>(), "Value", "Text");
                }

                if (designParams.CON_REQ == "Y" || designParams.SUPP_REQ == "Y")
                {
                    ViewBag.ddlContractor = commonFuncObj.PopulateContractorSupplier(objMaster);
                }
                else
                {
                    ViewBag.ddlContractor = new SelectList(new List<SelectListItem>().AsEnumerable<SelectListItem>(), "Value", "Text");
                }


                if (designParams.AFREEMENT_REQ == "Y")
                {
                    //if (acc_bill_details_trans != null)
                    //{
                    //    Int32 tmpAggrementCode = objMaster.AGREEMENT_CODE;
                    //    Int32 tmpDistrict = objMaster.DISTRICT_CODE;
                    //    Int32 tmpContID = objMaster.MAST_CONT_ID;
                    //    objMaster.DISTRICT_CODE = PMGSYSession.Current.LevelId == 5 ? PMGSYSession.Current.DistrictCode : Convert.ToInt32(acc_bill_details_trans.MAST_DISTRICT_CODE);
                    //    objMaster.MAST_CONT_ID = Convert.ToInt32(acc_bill_details_trans.MAST_CON_ID);
                    //    objMaster.AGREEMENT_CODE = Convert.ToInt32(acc_bill_details_trans.IMS_AGREEMENT_CODE);
                    //    ViewBag.ddlAgreement = commonFuncObj.PopulateAgreement(objMaster);
                    //    objMaster.AGREEMENT_CODE = tmpAggrementCode;
                    //    objMaster.MAST_CONT_ID = tmpContID;
                    //    objMaster.DISTRICT_CODE = tmpDistrict;
                    //}

                    //check condition it is write or not
                    //if (teoDetailsModelForTOB.TXN_NO == 0)
                    //{
                    //    List<SelectListItem> lstOneItem = new List<SelectListItem>();
                    //    lstOneItem.Insert(0, (new SelectListItem { Text = "Select Agreement", Value = "0", Selected = true }));
                    //    ViewBag.ddlAgreement = lstOneItem;
                    //}
                    //else
                    //{

                    //}

                    ViewBag.ddlAgreement = commonFuncObj.PopulateAgreement(objMaster);

                }
                else
                {
                    ViewBag.ddlAgreement = new SelectList(new List<SelectListItem>().AsEnumerable<SelectListItem>(), "Value", "Text");
                }

                if (designParams.SAN_REQ == "Y")
                {
                    ViewBag.ddlSancYear = commonFuncObj.PopulateSancYear(objMaster);
                }
                else
                {
                    ViewBag.ddlSancYear = new SelectList(new List<SelectListItem>().AsEnumerable<SelectListItem>(), "Value", "Text");
                }

                //designParams.ROAD_REQ condition necessary bcoz it will populate unnecessarily 
                if (designParams.ROAD_REQ == "Y" && validationParams.IS_ROAD_REPEAT == "Y")
                {
                    Int32 tmpAggCode = objMaster.AGREEMENT_CODE;
                    objMaster.AGREEMENT_CODE = Convert.ToInt32(teoDetailsModelForTOB.IMS_AGREEMENT_CODE_C);
                    objMaster.ROAD_CODE = Convert.ToInt32(teoDetailsModelForTOB.IMS_PR_ROAD_CODE_C);
                    ViewBag.ddlRoad = commonFuncObj.PopulateRoad(objMaster);
                    objMaster.AGREEMENT_CODE = tmpAggCode;
                }
                else if (validationParams.IS_AGREEMENT_REPEAT == "Y" && designParams.AFREEMENT_REQ == "Y")
                {
                    Int32 tmpAggCode = objMaster.AGREEMENT_CODE;
                    objMaster.AGREEMENT_CODE = Convert.ToInt32(teoDetailsModelForTOB.IMS_AGREEMENT_CODE_C);
                    objMaster.ROAD_CODE = 0;
                    ViewBag.ddlRoad = commonFuncObj.PopulateRoad(objMaster);
                    objMaster.AGREEMENT_CODE = tmpAggCode;
                }
                else if (designParams.ROAD_REQ == "Y")
                {
                    Int32 tmpAggCode = objMaster.AGREEMENT_CODE;
                    objMaster.AGREEMENT_CODE = Convert.ToInt32(teoDetailsModelForTOB.IMS_AGREEMENT_CODE_C);
                    objMaster.ROAD_CODE = 0;
                    ViewBag.ddlRoad = commonFuncObj.PopulateRoad(objMaster);
                    objMaster.AGREEMENT_CODE = tmpAggCode;
                }
                else
                {
                    ViewBag.ddlRoad = new SelectList(new List<SelectListItem>().AsEnumerable<SelectListItem>(), "Value", "Text");
                }


                if (designParams.PKG_REQ == "Y")
                {
                    if (teoDetailsModelForTOB.TXN_NO == 0)
                    {
                        List<SelectListItem> lstOneItem = new List<SelectListItem>();
                        lstOneItem.Insert(0, (new SelectListItem { Text = "Select Package", Value = "0", Selected = true }));
                        ViewBag.ddlPackage = lstOneItem;
                    }
                    else
                    {
                        ViewBag.ddlPackage = commonFuncObj.PopulatePackage(objMaster);
                    }
                }
                else
                {
                    ViewBag.ddlPackage = new SelectList(new List<SelectListItem>().AsEnumerable<SelectListItem>(), "Value", "Text");
                }


                if (validationParams.IS_HEAD_REPEAT == "Y")
                {
                    objMaster.HEAD_ID = teoDetailsModelForTOB.HEAD_ID_C;
                    ViewBag.ddlHead_C = commonFuncObj.PopulateHead(objMaster);

                    objMaster.CREDIT_DEBIT = "D";
                    objMaster.HEAD_ID = teoDetailsModelForTOB.HEAD_ID_D;
                    ViewBag.ddlHead_D = commonFuncObj.PopulateHead(objMaster);

                    //objMaster.HEAD_ID = teoDetailsModel.HEAD_ID;
                }
                else
                {
                    objMaster.HEAD_ID = teoDetailsModelForTOB.HEAD_ID_C;
                    ViewBag.ddlHead_C = commonFuncObj.PopulateHead(objMaster);

                    objMaster.CREDIT_DEBIT = "D";
                    objMaster.HEAD_ID = teoDetailsModelForTOB.HEAD_ID_D;
                    ViewBag.ddlHead_D = commonFuncObj.PopulateHead(objMaster);

                    // ViewBag.ddlHead = commonFuncObj.PopulateHead(objMaster);
                }

                ACC_TEO_SCREEN_DESIGN_HEADWISE_DETAILS headParams = new ACC_TEO_SCREEN_DESIGN_HEADWISE_DETAILS();
                //if (acc_bill_details_trans != null)
                //{
                //    objMaster.HEAD_ID = acc_bill_details_trans.HEAD_ID;
                //}

                objMaster.HEAD_ID = teoDetailsModelForTOB.HEAD_ID_C;
                headParams = commonFuncObj.GetHeadwiseDesignParams(objMaster);

                if (headParams == null)
                {
                    headParams = teoDAL.setDefualtDesignHeadParam();
                }

                if (headParams.CON_REQ == "Y")
                {
                    objMaster.MAST_CONT_ID = Convert.ToInt32(teoDetailsModelForTOB.MAST_CON_ID_C);
                    ViewBag.ddlHeadContractor = commonFuncObj.PopulateContractorSupplier(objMaster);
                    objMaster.MAST_CONT_ID = 0;
                }
                else
                {
                    ViewBag.ddlHeadContractor = new SelectList(new List<SelectListItem>().AsEnumerable<SelectListItem>(), "Value", "Text");
                }

                if (headParams.AGREEMENT_REQ == "Y")
                {
                    Int32 tmpAggrementCode = objMaster.AGREEMENT_CODE;
                    Int32 tmpDistrict = objMaster.DISTRICT_CODE;
                    Int32 tmpContID = objMaster.MAST_CONT_ID;

                    objMaster.DISTRICT_CODE = PMGSYSession.Current.LevelId == 5 ? PMGSYSession.Current.DistrictCode : Convert.ToInt32(teoDetailsModelForTOB.MAST_DISTRICT_CODE_C);
                    objMaster.MAST_CONT_ID = Convert.ToInt32(teoDetailsModelForTOB.MAST_CON_ID_C);
                    objMaster.AGREEMENT_CODE = Convert.ToInt32(teoDetailsModelForTOB.IMS_AGREEMENT_CODE_C);
                    ViewBag.ddlHeadAgreement = commonFuncObj.PopulateAgreement(objMaster);
                    objMaster.AGREEMENT_CODE = tmpAggrementCode;
                    objMaster.MAST_CONT_ID = tmpContID;
                    objMaster.DISTRICT_CODE = tmpDistrict;
                }
                else
                {
                    ViewBag.ddlHeadAgreement = new SelectList(new List<SelectListItem>().AsEnumerable<SelectListItem>(), "Value", "Text");
                }


                ViewBag.ddlHeadSancYear = new SelectList(new List<SelectListItem>().AsEnumerable<SelectListItem>(), "Value", "Text");

                ViewBag.ddlHeadPackage = new SelectList(new List<SelectListItem>().AsEnumerable<SelectListItem>(), "Value", "Text");

                if (validationParams.IS_ROAD_REPEAT == "Y" && headParams.ROAD_REQ == "Y")
                {
                    Int32 tmpAggCode = objMaster.AGREEMENT_CODE;
                    objMaster.AGREEMENT_CODE = Convert.ToInt32(teoDetailsModelForTOB.IMS_AGREEMENT_CODE_C);
                    objMaster.ROAD_CODE = Convert.ToInt32(teoDetailsModelForTOB.IMS_PR_ROAD_CODE_C);
                    ViewBag.ddlHeadRoad = commonFuncObj.PopulateRoad(objMaster);
                    objMaster.AGREEMENT_CODE = tmpAggCode;
                }
                else if (headParams.ROAD_REQ == "Y" && validationParams.IS_AGREEMENT_REPEAT == "Y" && headParams.AGREEMENT_REQ == "Y")
                {
                    Int32 tmpAggCode = objMaster.AGREEMENT_CODE;
                    objMaster.AGREEMENT_CODE = Convert.ToInt32(teoDetailsModelForTOB.IMS_AGREEMENT_CODE_C);
                    objMaster.ROAD_CODE = 0;
                    ViewBag.ddlHeadRoad = commonFuncObj.PopulateRoad(objMaster);
                    objMaster.AGREEMENT_CODE = tmpAggCode;
                }
                else
                {
                    ViewBag.ddlHeadRoad = new SelectList(new List<SelectListItem>().AsEnumerable<SelectListItem>(), "Value", "Text");
                }


                //if (acc_bill_details_trans != null && validationParams.IS_HEAD_REPEAT == "Y")
                //{
                //    teoDetailsModel.HEAD_ID = acc_bill_details_trans.HEAD_ID;
                //}

                //Added By Abhishek kamble to populate State drop down   1 Oct 2014
                ViewBag.ddlState = commonFuncObj.PopulateStates(true);
                //teoDetailsModelForTOB.MAST_STATE_CODE_C = PMGSYSession.Current.StateCode;
            }
            // ViewBag.CreditDebit = "C";
            return View("TEOAddDetailsForTOB", teoDetailsModelForTOB);

        }


        /// <summary>
        /// Action to get TEO details list 
        /// </summary>
        /// <param name="frmCollect"></param>
        /// <returns></returns>
        [Audit]
        public ActionResult TEODetailsListForTOB(FormCollection frmCollect)
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
            ReceiptFilterModel objFilter = new ReceiptFilterModel();
            teoBAL = new TransferEntryOrderBAL();
            long totalRecords = 0;
            decimal cTotalAmount = 0;
            decimal dTotalAmount = 0;
            decimal GrossAmount = 0;
            string[] parameters = Request.Params["masterId"].ToString().Split('/');
            string[] strParameters = URLEncrypt.DecryptParameters(new string[] { parameters[0], parameters[1], parameters[2] });
            if (strParameters.Length > 0)
            {
                objFilter.BillId = Convert.ToInt64(strParameters[0]);
            }
            else
            {
                throw new Exception("Error While Getting Master Data");
            }

            objFilter.page = Convert.ToInt32(frmCollect["page"]) - 1;
            objFilter.rows = Convert.ToInt32(frmCollect["rows"]);
            objFilter.sidx = frmCollect["sidx"].ToString();
            objFilter.sord = frmCollect["sord"].ToString();

            String finFlag = "N";
            Array lstTEODetails = teoBAL.TEODetailsList(objFilter, out totalRecords, out cTotalAmount, out dTotalAmount, out GrossAmount, true);
            if (GrossAmount == cTotalAmount && GrossAmount == dTotalAmount)
            {
                finFlag = "Y";
            }

            //get if multiple transaction is allowed
            ACC_TEO_SCREEN_DESIGN_PARAM_DETAILS designParams = new ACC_TEO_SCREEN_DESIGN_PARAM_DETAILS();
            commonFuncObj = new CommonFunctions();
            designParams = commonFuncObj.getTEODesignParamDetails(objFilter.BillId, 0);


            var jsonData = new
            {
                rows = lstTEODetails,
                total = totalRecords <= Convert.ToInt32(objFilter.rows) ? 1 : (totalRecords % Convert.ToInt32(objFilter.rows) == 0 ? totalRecords / Convert.ToInt32(objFilter.rows) : totalRecords / Convert.ToInt32(objFilter.rows) + 1),//totalRecords <= objFilter.rows ? 1 : totalRecords / objFilter.rows + 1,
                page = objFilter.page + 1,
                records = totalRecords,
                userdata = new { DPIU = "<div style='color:green'><b>Total Amount:</b></div>", CAmount = "<div style='color:green'>" + cTotalAmount.ToString("#0.00") + "</div>", DAmount = "<div style='color:green'>" + dTotalAmount.ToString("#0.00") + "</div>", isFinalize = finFlag, multipleTrans = designParams.MTXN_REQ, _TotalCAmount = cTotalAmount.ToString("#0.00") }
            };
            return Json(jsonData);
        }


        [HttpPost]
        [Audit]
        public ActionResult DeleteTEODetailsForTOB(String parameter, String hash, String key)
        {

            bool status = false;
            string message = string.Empty;
            Dictionary<string, string> decryptedParameters = null;
            teoBAL = new TransferEntryOrderBAL();
            try
            {
                decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { parameter, hash, key });

                if (decryptedParameters.Count() > 0)
                {

                    if (teoBAL.DeleteTEODetailsForTOB(Convert.ToInt64(decryptedParameters["BILL_ID"].ToString()), Convert.ToInt16(decryptedParameters["TXN_NO"].ToString()), ref message))
                    {
                        message = "TEO details deleted successfully.";
                        status = true;
                    }
                    else
                    {
                        message = message == string.Empty ? "TEO details not deleted successfully." : message;
                    }

                    return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
                }

                message = "TEO details not deleted successfully.";
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);

                message = "TEO details not deleted successfully.";
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
        }


        //added by Koustubh Nakate on 14/10/2013 to check transaction is already exist or not for given BILL ID  
        /// <summary>
        /// to check transaction already exist for billId
        /// </summary>
        /// <returns></returns>
        [Audit]
        public ActionResult CheckForTransactionAlreadyExist()
        {

            bool exist = false;

            Int64 billID = 0;
            Int32 DistrictC = 0;
            Int32 PIUC = 0;
            Int32 DistrictD = 0;
            Int32 PIUD = 0;
            Int32? ContractorC = 0;

            //Added By Abhishek kamble 13Oct2014 to set State of Debit Side
            Int32? StateD = 0;

            string[] paramaters = null;
            try
            {
                teoBAL = new TransferEntryOrderBAL();
                if (Request.Params["BillID"] == null)
                {
                    return Json(new { exist = false }, JsonRequestBehavior.AllowGet);
                }

                paramaters = Request.Params["BillID"].ToString().Split('/');

                string[] strParameters = URLEncrypt.DecryptParameters(new string[] { paramaters[0], paramaters[1], paramaters[2] });

                billID = Convert.ToInt64(strParameters[0]);

                exist = teoBAL.CheckForTransactionAlreadyExistBAL(billID, ref DistrictC, ref PIUC, ref DistrictD, ref PIUD, ref ContractorC, ref StateD);

                return Json(new { exist = exist, DistrictC = DistrictC, PIUC = PIUC, DistrictD = DistrictD, PIUD = PIUD, ContractorC = ContractorC, StateD = StateD }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return Json(new { exist = exist }, JsonRequestBehavior.AllowGet);
            }
        }

        //added by Koustubh Nakate on 17/10/2013 to check district has been shifted or not  
        /// <summary>
        /// to check district has been shifted or not  
        /// </summary>
        /// <returns></returns>
        [Audit]
        public ActionResult CheckIsDistrictShifted()
        {

            bool exist = false;

            Int32 DistrictC = 0;

            Int32 DistrictD = 0;

            string[] paramaters = null;
            try
            {
                teoBAL = new TransferEntryOrderBAL();
                if (Request.Params["DistrictC"] == null || Request.Params["DistrictD"] == null)
                {
                    return Json(new { exist = exist }, JsonRequestBehavior.AllowGet);
                }


                DistrictC = Convert.ToInt32(Request.Params["DistrictC"]);
                DistrictD = Convert.ToInt32(Request.Params["DistrictD"]);

                exist = teoBAL.CheckIsDistrictShiftedBAL(DistrictC, DistrictD);

                return Json(new { exist = exist }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return Json(new { exist = exist }, JsonRequestBehavior.AllowGet);
            }
        }


        #endregion Transfer Of Balances

        #region NEW_CHANGE_TOB_AUTO_ENTRY
        /// <summary>
        /// adds the transaction into the associated dpiu account as selected in the TOB
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="hash"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        [Audit]
        public ActionResult AddAutoEntryTOB(String parameter, String hash, String key)
        {
            try
            {
                teoBAL = new TransferEntryOrderBAL();
                string[] urlParameters = URLEncrypt.DecryptParameters(new string[] { parameter, hash, key });
                bool status = teoBAL.AddAutoEntryTOB(Convert.ToInt64(urlParameters[0]));
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

        #endregion

        //new method added by Vikram 

        /// <summary>
        /// validates whether the selected month matches with the account head selected.
        /// </summary>
        /// <param name="headId"></param>
        /// <param name="proposalCode"></param>
        /// <param name="fundType"></param>
        /// <returns></returns>
        [Audit]
        public bool ValidateRoad(int headId, int proposalCode, string fundType)
        {
            string upgradeConnectFlag = string.Empty;
            teoBAL = new TransferEntryOrderBAL();
            try
            {
                if (fundType == "P")
                {
                    switch (headId)
                    {
                        case 28:
                            upgradeConnectFlag = "N";
                            break;
                        case 29:
                            upgradeConnectFlag = "U";
                            break;
                        ///Changed by SAMMED A. PATIL on 20FEB2018 check for RCPLWE Scheme Transaction Id 1789,1790
                        case 427:
                            upgradeConnectFlag = "N";
                            break;
                        case 428:
                            upgradeConnectFlag = "U";
                            break;
                        default:
                            upgradeConnectFlag = "A";
                            break;
                    }

                    bool status = teoBAL.ValidateRoad(proposalCode, upgradeConnectFlag);
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
        public bool ValidateRoadForPMGSYScheme(short headID, int RoadCode)
        {

            try
            {
                commonFuncObj = new CommonFunctions();
                bool status = commonFuncObj.ValidateRoadForPMGSYScheme(headID, RoadCode, false);
                return status;

            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return false;
            }
        }

        /// <summary>
        /// validates whether the selected piu has closed the month in which the transaction is proceeding.
        /// </summary>
        /// <param name="headId"></param>
        /// <param name="proposalCode"></param>
        /// <param name="fundType"></param>
        /// <returns></returns>
        public JsonResult ValidateDPIUMonths(String id)
        {
            teoBAL = new TransferEntryOrderBAL();
            try
            {
                if (!string.IsNullOrEmpty(id))
                {
                    string[] parameters = id.Split('@');
                    bool status = teoBAL.ValidateDPIUMonths(Convert.ToInt32(parameters[0]), parameters[1]);
                    if (status == true)
                    {
                        return Json(new { IsValid = true, Message = "" });
                    }
                    else
                    {
                        return Json(new { IsValid = false, Message = "" });
                    }
                }
                return Json(new { IsValid = false, Message = "Error occurred while processing your request." });
            }
            catch (Exception)
            {
                return Json(new { IsValid = false, Message = "Error occurred while processing your request." });
            }
        }


        #region MAPPING_OLD_DATA_PAYMENT

        /// <summary>
        /// returns the list containing the old data imprest payments
        /// </summary>
        /// <returns></returns>
        [Audit]
        public ActionResult OldImprestPayments()
        {
            Int32 Month = 0;
            Int32 Year = 0;
            try
            {
                //new change done by Vikram on 1-Jan-2014
                if (PMGSYSession.Current.AccMonth != 0)
                {
                    Month = PMGSYSession.Current.AccMonth;
                    Year = PMGSYSession.Current.AccYear;
                }
                //end of change
                else
                {
                    Month = DateTime.Now.Month;
                    Year = DateTime.Now.Year;
                }
                commonFuncObj = new CommonFunctions();
                ViewBag.ddlMonth = commonFuncObj.PopulateMonths(Month);
                ViewBag.ddlYear = commonFuncObj.PopulateYears(Year);
                return View();
            }
            catch (Exception)
            {
                return View();
            }

        }

        /// <summary>
        /// populates the list of old data imprest payments
        /// </summary>
        /// <param name="frmCollect">collection containing the page,rows , sort index and sort order</param>
        /// <returns></returns>
        [HttpPost]
        [Audit]
        public ActionResult OldImprestPaymentList(FormCollection frmCollect)
        {
            try
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
                ReceiptFilterModel objFilter = new ReceiptFilterModel();
                long totalRecords = 0;
                teoBAL = new TransferEntryOrderBAL();
                objFilter.page = Convert.ToInt32(frmCollect["page"]) - 1;
                objFilter.rows = Convert.ToInt32(frmCollect["rows"]);
                objFilter.sidx = frmCollect["sidx"].ToString();
                objFilter.sord = frmCollect["sord"].ToString();
                if (!string.IsNullOrEmpty(frmCollect["month"]))
                {
                    objFilter.Month = Convert.ToInt16(frmCollect["month"]);
                }

                if (!string.IsNullOrEmpty(frmCollect["year"]))
                {
                    objFilter.Year = Convert.ToInt16(frmCollect["year"]);
                }

                var jsonData = new
                {
                    rows = teoBAL.OldImprestPaymentListBAL(objFilter, out totalRecords),
                    total = totalRecords <= objFilter.rows ? 1 : totalRecords / objFilter.rows + 1,
                    page = objFilter.page + 1,
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

        /// <summary>
        /// returns the view containing the listing of settled TEO's and Reciepts 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Audit]
        public ActionResult MapTEODetails(String id)
        {
            ImprestSettlementViewModel model = new ImprestSettlementViewModel();
            String[] encryptedParameters;
            Dictionary<string, string> decryptedParameters = new Dictionary<string, string>();
            try
            {
                if (!string.IsNullOrEmpty(id))
                {
                    encryptedParameters = id.Split('/');
                    decryptedParameters = (URLEncrypt.DecryptParameters1(new string[] { encryptedParameters[0], encryptedParameters[1], encryptedParameters[2] }));
                    model.P_BILL_ID = Convert.ToInt32(decryptedParameters["BILL_ID"]);
                }

                if (!string.IsNullOrEmpty(Request.Params["Month"]))
                {
                    model.Month = Convert.ToInt16(Request.Params["Month"]);
                }

                if (!string.IsNullOrEmpty(Request.Params["Year"]))
                {
                    model.Year = Convert.ToInt16(Request.Params["Year"]);
                }

                commonFuncObj = new CommonFunctions();
                ViewBag.ddlMonth = commonFuncObj.PopulateMonths(model.Month);
                ViewBag.ddlYear = commonFuncObj.PopulateYears(model.Year);

                return View(model);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return View();
            }
        }

        /// <summary>
        /// returns the list of setteled TEO and Reciepts to map
        /// </summary>
        /// <param name="frmCollection"></param>
        /// <returns></returns>
        [HttpPost]
        [Audit]
        public ActionResult ListSettledTEORecieptDetails(FormCollection frmCollection)
        {
            try
            {
                //Adde By Abhishek kamble 30-Apr-2014 start
                using (CommonFunctions commonFunction = new CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new GridParams(Convert.ToInt32(frmCollection["page"]), Convert.ToInt32(frmCollection["rows"]), frmCollection["sidx"], frmCollection["sord"], Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                    {
                        return null;
                    }
                }
                //Adde By Abhishek kamble 30-Apr-2014 end
                ReceiptFilterModel objFilter = new ReceiptFilterModel();
                long totalRecords = 0;
                teoBAL = new TransferEntryOrderBAL();
                objFilter.page = Convert.ToInt32(frmCollection["page"]) - 1;
                objFilter.rows = Convert.ToInt32(frmCollection["rows"]);
                objFilter.sidx = frmCollection["sidx"].ToString();
                objFilter.sord = frmCollection["sord"].ToString();
                if (!string.IsNullOrEmpty(frmCollection["month"]))
                {
                    objFilter.Month = Convert.ToInt16(frmCollection["month"]);
                }

                if (!string.IsNullOrEmpty(frmCollection["year"]))
                {
                    objFilter.Year = Convert.ToInt16(frmCollection["year"]);
                }

                var jsonData = new
                {
                    rows = teoBAL.ListSettledTEORecieptDetailsBAL(objFilter, out totalRecords),
                    total = totalRecords <= objFilter.rows ? 1 : totalRecords / objFilter.rows + 1,
                    page = objFilter.page + 1,
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

        /// <summary>
        /// function for validating the payment total amount with the selected TEO or Reciept vouchers
        /// </summary>
        /// <returns></returns>
        [Audit]
        public ActionResult ValidatePaymentAmount()
        {
            teoBAL = new TransferEntryOrderBAL();
            string message = string.Empty;
            String[] s_billIds = null;
            int P_BILL_ID = 0;
            try
            {
                if (!String.IsNullOrEmpty(Request.Params["S_BILL_ID[]"]))
                {
                    s_billIds = Request.Params["S_BILL_ID[]"].Split(',');
                }
                else
                {
                    return Json(new { IsValid = false, Message = "Please select settlement vouchers" });
                }

                if (!String.IsNullOrEmpty(Request.Params["P_BILL_ID"]))
                {
                    P_BILL_ID = Convert.ToInt32(Request.Params["P_BILL_ID"]);
                }
                else
                {
                    return Json(new { IsValid = false, Message = "Please select payment vouchers" });
                }

                bool isValid = teoBAL.ValidatePaymentAmountBAL(s_billIds, P_BILL_ID, out message);

                return Json(new { IsValid = isValid, Message = message });
            }
            catch (Exception)
            {
                return Json(new { IsValid = false });
            }
        }

        /// <summary>
        /// for validating whether the Payment has been made and finalized on Department expenditure on works
        /// </summary>
        /// <returns></returns>
        public ActionResult ValidateTransaction(String id)
        {
            try
            {
                bool status = true;
                teoBAL = new TransferEntryOrderBAL();
                int txnId = 0;
                if (!String.IsNullOrEmpty(id))
                {
                    txnId = Convert.ToInt32(id);
                }

                if (txnId == 169)
                {
                    //status = teoBAL.ValidateTransaction(txnId);
                }

                if (status == true)
                {
                    return Json(new { success = status, message = "" });
                }
                else
                {
                    return Json(new { success = status, message = "Payment has not been made or finalized to made this adjustment." });
                }

            }
            catch (Exception)
            {
                return Json(new { success = false, message = "Error occurred while processing the request." });
            }
        }


        #endregion



    }
}
