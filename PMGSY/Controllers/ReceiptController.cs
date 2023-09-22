using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PMGSY.Common;
using PMGSY.Models.Common;
using PMGSY.BAL.Receipt;
using PMGSY.Models.Receipts;
using PMGSY.Models;
using System.Data.Entity.Validation;
using PMGSY.Extensions;

namespace PMGSY.Controllers
{
    [RequiredAuthentication]
    [RequiredAuthorization]
    public class ReceiptController : Controller
    {
        private PMGSYEntities db = new PMGSYEntities();
        private IReceiptBAL receiptBAL = new ReceiptBAL();
        private CommonFunctions commomFuncObj = new CommonFunctions();

        ///Commented temperory for payuthaldwani issue
        //[GenericAccountValidationFilter(InputParameter = new string[] { "BankDetails", "ChequeBookDetails", "OpeningBalanceDetails", "AuthSign"})]
        public ActionResult ListReceipt()
        {
            Int32 Month = DateTime.Now.Month;
            Int32 Year = DateTime.Now.Year;
            if (Request.Params["Month"] != null)
            {
                Month = Convert.ToInt32(Request.Params["Month"]);
                Year = Convert.ToInt32(Request.Params["Year"]);
            }
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
            
            ViewBag.ddlMonth = commomFuncObj.PopulateMonths(Month);
            ViewBag.ddlYear = commomFuncObj.PopulateYears(Year);
            TransactionParams objMaster = new TransactionParams();
            objMaster.BILL_TYPE = "R";
            objMaster.FUND_TYPE = PMGSYSession.Current.FundType;
            objMaster.LVL_ID = PMGSYSession.Current.LevelId;
            objMaster.TXN_ID = 0;

            ViewBag.ddlMasterTrans = commomFuncObj.PopulateTransactions(objMaster);

            return View("ListReceipt");
        }

        [HttpPost]
        [Audit]
        public ActionResult GetReceiptList(FormCollection homeFormCollection)
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

            var jsonData = new
            {
                rows = receiptBAL.ReceiptList(objFilter, out totalRecords),
                total = totalRecords <= objFilter.rows ? 1 : totalRecords / objFilter.rows + 1,
                page = objFilter.page + 1,
                records = totalRecords
            };
            return Json(jsonData);

        }

        [Audit]
        public ActionResult ReceiptMasterList(FormCollection frmCollect)
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
            long totalRecords;
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
            var jsonData = new
            {
                rows = receiptBAL.ReceiptMasterList(objFilter, out totalRecords),
                total = totalRecords <= objFilter.rows ? 1 : totalRecords / objFilter.rows + 1,
                page = objFilter.page,
                records = totalRecords
            };
            return Json(jsonData);
        }



        /// <summary>
        /// action to get the receipt details list 
        /// </summary>
        /// <param name="frmCollect"></param>
        /// <returns></returns>
        [Audit]
        public ActionResult ReceiptDetailsList(FormCollection frmCollect)
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
            decimal totalAmount = 0;
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
            ACC_BILL_MASTER acc_bill_master = new ACC_BILL_MASTER();
            acc_bill_master = db.ACC_BILL_MASTER.Where(m => m.BILL_ID == objFilter.BillId).FirstOrDefault();
            String mulTxn = db.ACC_SCREEN_DESIGN_PARAM_MASTER.Where(m => m.TXN_ID == acc_bill_master.TXN_ID).Select(m => m.MTXN_REQ).FirstOrDefault();

            //Added by Abhishek kamble 1-jan-2014
            //string IsMultiTranAllowed =mulTxn;
            
            if (db.ACC_BILL_DETAILS.Where(m => m.BILL_ID == acc_bill_master.BILL_ID).Any() && mulTxn == "N")
            {
                mulTxn = "N";
            }
            else
            {
                mulTxn = "Y";
            }
            decimal grossAmount = acc_bill_master.GROSS_AMOUNT;
            objFilter.page = Convert.ToInt32(frmCollect["page"]) - 1;
            objFilter.rows = Convert.ToInt32(frmCollect["rows"]);
            objFilter.sidx = frmCollect["sidx"].ToString();
            objFilter.sord = frmCollect["sord"].ToString();

            
            var jsonData = new
            {
                rows = receiptBAL.ReceiptDetailsList(objFilter, out totalRecords, out totalAmount),
                total = totalRecords <= objFilter.rows ? 1 : totalRecords / objFilter.rows + 1,
                page = objFilter.page,
                records = totalRecords,//Modified by Abhishek kamble 1-jan-2014
                userdata = new { DPIU = "<label style='color:#125806'>Total Receipt(In Rs.)</label>", Amount = "<label style='color:#0a8900'>" + totalAmount.ToString("#0.00") + "</label>", Narration = "<label style='color:#125806'>Remained(In Rs): </label><label style='color:#981b1e'>" + (grossAmount - totalAmount).ToString("#0.00") + "</label>", TotalAmount = totalAmount.ToString("#0.00"), isMulTxn = mulTxn, ReceiptGrossAmount = acc_bill_master.GROSS_AMOUNT }//, IsMultiTranAllowed = IsMultiTranAllowed, ReceiptGrossAmount = acc_bill_master.GROSS_AMOUNT
            };
            return Json(jsonData);
        }

        [Audit]
        public ActionResult AddEditReceipt(String parameter, String hash, String key)
        {
            ViewBag.Month = Request.Params["Month"];
            ViewBag.Year = Request.Params["Year"];
            if (parameter == null)
            {
                ViewBag.BILL_ID = 0;
            }
            else
            {
                string[] strParameters = URLEncrypt.DecryptParameters(new string[] { parameter, hash, key });
                Int64 billId = Convert.ToInt64(strParameters[0]);
                ViewBag.BILL_ID = URLEncrypt.EncryptParameters(new string[] { billId.ToString().Trim() });
                ACC_BILL_MASTER acc_bill_master = new ACC_BILL_MASTER();
                acc_bill_master = db.ACC_BILL_MASTER.Where(m => m.BILL_ID == billId).FirstOrDefault();
                ViewBag.IsFinalize = acc_bill_master.BILL_FINALIZED;
                String mulTxn = db.ACC_SCREEN_DESIGN_PARAM_MASTER.Where(m => m.TXN_ID == acc_bill_master.TXN_ID).Select(m => m.MTXN_REQ).FirstOrDefault();
                if (db.ACC_BILL_DETAILS.Where(m => m.BILL_ID == acc_bill_master.BILL_ID).Any() && mulTxn == "N")
                {
                    ViewBag.IsMTxn = "N";
                }
                else
                {
                    ViewBag.IsMTxn = "Y";
                }

                //Added By Abhishek Kamble 1-jan-2013
                ViewBag.IsMultiTranAllowed = mulTxn;
                ViewBag.ReceiptGrossAmount = acc_bill_master.GROSS_AMOUNT;
                
            }
            return View();
        }

        [Audit]
        public ActionResult ReceiptMaster(String parameter, String hash, String key)
        {
            Int16 Month = Convert.ToInt16(Request.Params["Month"]);
            Int16 Year = Convert.ToInt16(Request.Params["Year"]);

            BillMasterViewModel billMaster = new BillMasterViewModel();
            TransactionParams objMaster = new TransactionParams();
            objMaster.BILL_TYPE = "R";
            objMaster.FUND_TYPE = PMGSYSession.Current.FundType;
            objMaster.LVL_ID = PMGSYSession.Current.LevelId;
            objMaster.TXN_ID = 0;
            if (parameter == null)
            {
                objMaster.OP_MODE = "A";
            }
            else
            {
                objMaster.OP_MODE = "E";
            }
            objMaster.OP_MODE = "A";
            ViewBag.ddlMasterTrans = commomFuncObj.PopulateTransactions(objMaster);
            if (parameter == null)
            {
                billMaster.BILL_DATE = String.Empty;
                ViewBag.ddlMonth = commomFuncObj.PopulateMonths(DateTime.Now.Month);
                ViewBag.ddlYear = commomFuncObj.PopulateYears(DateTime.Now.Year);
                billMaster.BILL_MONTH = Convert.ToInt16(Month);
                billMaster.BILL_YEAR = Convert.ToInt16(Year);
            }
            else
            {
                string[] strParameters = URLEncrypt.DecryptParameters(new string[] { parameter, hash, key });
                Int64 billId = Convert.ToInt64(strParameters[0]);
                ViewBag.BILL_ID = URLEncrypt.EncryptParameters(new string[] { billId.ToString().Trim() });
                ACC_BILL_MASTER acc_bill_master = new ACC_BILL_MASTER();
                acc_bill_master = db.ACC_BILL_MASTER.Find(billId);
                ViewBag.ddlMonth = commomFuncObj.PopulateMonths(acc_bill_master.BILL_MONTH);
                ViewBag.ddlYear = commomFuncObj.PopulateYears(acc_bill_master.BILL_YEAR);
                billMaster = CloneObject(acc_bill_master);
            }

            billMaster.CURRENT_DATE = DateTime.Now.ToString("dd/MM/yyyy");
            return PartialView(billMaster);
        }

        [Audit]
        public ActionResult ReceiptDetails(String parameter, String hash, String key)
        {
            String[] strParameters = URLEncrypt.DecryptParameters(new string[] { parameter, hash, key });
            BillDetailsViewModel billDetailsViewModel = new BillDetailsViewModel();
            Int64 billId = 0;
            Int16 parentId = 0;
            String TransId = null;
            billId = Convert.ToInt64(strParameters[0].Split('$')[0]);
            ACC_BILL_MASTER acc_bill_master = new ACC_BILL_MASTER();
            acc_bill_master = db.ACC_BILL_MASTER.Where(m => m.BILL_ID == billId).FirstOrDefault();
            ViewBag.BILL_ID = URLEncrypt.EncryptParameters(new String[] { billId.ToString().Trim()});
            if (strParameters[0].Contains('$'))
            {
                parentId = acc_bill_master.TXN_ID;
                Int16 TransNo = Convert.ToInt16(strParameters[0].Split('$')[1]);
                billDetailsViewModel = receiptBAL.GetReceiptDetailByTransNo(billId, TransNo);
                TransId = URLEncrypt.EncryptParameters(new string[] { billId.ToString().Trim() + "$" + TransNo.ToString().Trim() });

                TransactionParams objMaster = new TransactionParams();
                objMaster.BILL_TYPE = acc_bill_master.BILL_TYPE;
                objMaster.FUND_TYPE = acc_bill_master.FUND_TYPE;
                objMaster.LVL_ID = acc_bill_master.LVL_ID;
                objMaster.TXN_ID = parentId;
                objMaster.ADMIN_ND_CODE = acc_bill_master.ADMIN_ND_CODE;
                objMaster.CASH_CHQ = acc_bill_master.CHQ_EPAY;
                if (billDetailsViewModel.MAST_CON_ID == null)
                {
                    objMaster.MAST_CONT_ID = 0;
                }
                else
                {
                    objMaster.MAST_CONT_ID = Convert.ToInt32(billDetailsViewModel.MAST_CON_ID);
                }
                objMaster.BILL_ID = billId;
                objMaster.STATE_CODE = PMGSYSession.Current.StateCode;
                objMaster.DISTRICT_CODE = PMGSYSession.Current.DistrictCode;
                objMaster.OP_MODE = "E";
                ViewBag.ddlSubTrans = commomFuncObj.PopulateTransactions(objMaster);
                Int64 pBillId = 0;
                if (db.ACC_MAP_IMPREST_PAYMENT_SETTLEMENT_DETAILS.Where(m => m.S_BIll_ID == billId).Any())
                {
                    pBillId = db.ACC_MAP_IMPREST_PAYMENT_SETTLEMENT_DETAILS.Where(m => m.S_BIll_ID == billId).Select(m => m.P_BILL_ID).FirstOrDefault();
                }

                ViewBag.ddlUnSettledVouchers = receiptBAL.GetUnSettledVouchers(acc_bill_master.BILL_MONTH, acc_bill_master.BILL_YEAR, pBillId);
                ViewBag.ddlContractor = commomFuncObj.PopulateContractorSupplier(objMaster);
                ViewBag.ddlAgreement = commomFuncObj.PopulateAgreement(objMaster);
                //ViewBag.ddlDPIU = commomFuncObj.PopulateDPIU(objMaster);//commented by Vikram on 08 Jan 2014
                if (billDetailsViewModel.TXN_ID == "1210")
                {
                    ViewBag.ddlDPIU = commomFuncObj.PopulateNodalAgencies(PMGSYSession.Current.StateCode);
                }
                else
                {
                    ViewBag.ddlDPIU = commomFuncObj.PopulateDPIU(objMaster);
                }
                // ViewBag.BILL_ID = strParameters;
            }
            else
            {
                //billId = Convert.ToInt64(strParameters[0]);
                parentId = db.ACC_BILL_MASTER.Where(m => m.BILL_ID == billId).Select(m => m.TXN_ID).FirstOrDefault();
                // Check for Edit (For Listing Screen to Add/Edit Master Receipt)
                if (db.ACC_BILL_DETAILS.Where(m => m.BILL_ID == billId).Count() > 0)
                {
                    TransId = "Y"; //Populate Receipt Details
                }
                else
                {
                    TransId = "N"; //Do not Populate Receipt Details
                }
                TransactionParams objMaster = new TransactionParams();
                objMaster.BILL_TYPE = "R";
                objMaster.FUND_TYPE = PMGSYSession.Current.FundType;
                objMaster.LVL_ID = PMGSYSession.Current.LevelId;
                objMaster.TXN_ID = parentId;
                objMaster.ADMIN_ND_CODE = PMGSYSession.Current.AdminNdCode; // Take it from session
                objMaster.MAST_CONT_ID = 0; // For initial condition
                objMaster.BILL_ID = billId;
                objMaster.STATE_CODE = PMGSYSession.Current.StateCode;
                objMaster.DISTRICT_CODE = PMGSYSession.Current.DistrictCode;
                objMaster.CASH_CHQ = acc_bill_master.CHQ_EPAY;
                objMaster.OP_MODE = "A";
                ViewBag.ddlSubTrans = commomFuncObj.PopulateTransactions(objMaster);
                ViewBag.ddlUnSettledVouchers = receiptBAL.GetUnSettledVouchers(acc_bill_master.BILL_MONTH, acc_bill_master.BILL_YEAR, 0);
                ViewBag.ddlContractor = commomFuncObj.PopulateContractorSupplier(objMaster);
                ViewBag.ddlAgreement = commomFuncObj.PopulateAgreement(objMaster);
                ViewBag.ddlDPIU = commomFuncObj.PopulateDPIU(objMaster);
                //ViewBag.BILL_ID = strParameters;
            }




            ViewBag.IsTrans = TransId;

            return PartialView("ReceiptDetails", billDetailsViewModel);
        }

        [HttpPost]
        [Audit]
        [ValidateAntiForgeryToken]
        public ActionResult AddReceiptMaster(BillMasterViewModel billMasterViewModel)
        {
            String ValidationSummary = String.Empty;

            commomFuncObj = new CommonFunctions();

            ViewBag.ddlMonth = commomFuncObj.PopulateMonths(billMasterViewModel.BILL_MONTH);
            ViewBag.ddlYear = commomFuncObj.PopulateYears(billMasterViewModel.BILL_YEAR);
            if (ModelState.IsValid)
            {
                billMasterViewModel.FUND_TYPE = PMGSYSession.Current.FundType;
                billMasterViewModel.LVL_ID = PMGSYSession.Current.LevelId;
                billMasterViewModel.ADMIN_ND_CODE = PMGSYSession.Current.AdminNdCode;
                billMasterViewModel.BILL_ID = db.ACC_BILL_MASTER.Max(m => m.BILL_ID) + 1;
                ACC_BILL_MASTER acc_bill_master = CloneModel(billMasterViewModel);
                try
                {
                    Int16 monthToClose = billMasterViewModel.BILL_YEAR;

                    string result = string.Empty;
                    String errMessage = String.Empty;

                    result = commomFuncObj.MonthlyClosingValidation(billMasterViewModel.BILL_MONTH, billMasterViewModel.BILL_YEAR, PMGSYSession.Current.FundType, PMGSYSession.Current.LevelId, PMGSYSession.Current.AdminNdCode,ref errMessage);
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
                        db.ACC_BILL_MASTER.Add(acc_bill_master);

                        int fiscalYear = 0;
                        if (acc_bill_master.BILL_MONTH <= 3)
                        {
                            fiscalYear = (acc_bill_master.BILL_YEAR - 1);
                        }
                        else
                        {
                            fiscalYear = acc_bill_master.BILL_YEAR;
                        }


                        ACC_VOUCHER_NUMBER_MASTER oldVoucherNumberModel = new ACC_VOUCHER_NUMBER_MASTER();
                        oldVoucherNumberModel = db.ACC_VOUCHER_NUMBER_MASTER.Where(x => x.ADMIN_ND_CODE == acc_bill_master.ADMIN_ND_CODE && x.FUND_TYPE == acc_bill_master.FUND_TYPE && x.BILL_TYPE == acc_bill_master.BILL_TYPE && x.FISCAL_YEAR == fiscalYear).FirstOrDefault();
                        ACC_VOUCHER_NUMBER_MASTER newMvoucherNumberModel = new ACC_VOUCHER_NUMBER_MASTER();

                        newMvoucherNumberModel.ADMIN_ND_CODE = acc_bill_master.ADMIN_ND_CODE;
                        newMvoucherNumberModel.FUND_TYPE = acc_bill_master.FUND_TYPE;
                        newMvoucherNumberModel.BILL_TYPE = acc_bill_master.BILL_TYPE;
                        newMvoucherNumberModel.FISCAL_YEAR = fiscalYear;
                        newMvoucherNumberModel.SLNO = oldVoucherNumberModel.SLNO + 1;

                        db.Entry(oldVoucherNumberModel).CurrentValues.SetValues(newMvoucherNumberModel);
                        db.SaveChanges();

                        //Added by Abhishek Kamble 1-jan-2014
                        String IsMultiTranAllowed = db.ACC_SCREEN_DESIGN_PARAM_MASTER.Where(m => m.TXN_ID == acc_bill_master.TXN_ID).Select(m => m.MTXN_REQ).FirstOrDefault();

                        return this.Json(new { success = true, message = URLEncrypt.EncryptParameters(new string[] { acc_bill_master.BILL_ID.ToString().Trim() }),IsMultiTranAllowed = IsMultiTranAllowed, ReceiptGrossAmount = acc_bill_master.GROSS_AMOUNT });
                    }
                    else
                    {
                        return this.Json(new { success = false, message = ValidationSummary });
                    }
                }
                catch (DbEntityValidationException ex)
                {
                    Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                    var errorMessages = ex.EntityValidationErrors.SelectMany(x => x.ValidationErrors).Select(x => x.ErrorMessage);
                    var fullErrorMessage = string.Join("; ", errorMessages);
                    var exceptionMessage = string.Concat(ex.Message, " The validation errors are: ", fullErrorMessage);
                    throw new DbEntityValidationException(exceptionMessage, ex.EntityValidationErrors);
                }
            }
            else
            {
                TransactionParams objMaster = new TransactionParams();
                objMaster.BILL_TYPE = "R";
                objMaster.FUND_TYPE = PMGSYSession.Current.FundType;
                objMaster.LVL_ID = PMGSYSession.Current.LevelId;
                objMaster.TXN_ID = 0;
                objMaster.OP_MODE = "A";
                ViewBag.ddlMasterTrans = commomFuncObj.PopulateTransactions(objMaster);
            }
            return PartialView("ReceiptMaster", billMasterViewModel);
        }

        [HttpPost]
        [Audit]
        [ValidateAntiForgeryToken]
        public ActionResult EditReceiptMaster(BillMasterViewModel billMasterViewModel, String parameter, String hash, String key)
        {
            String ValidationSummary = String.Empty;
            string[] strParameters = URLEncrypt.DecryptParameters(new string[] { parameter, hash, key });
            ViewBag.ddlMonth = commomFuncObj.PopulateMonths(billMasterViewModel.BILL_MONTH);
            ViewBag.ddlYear = commomFuncObj.PopulateYears(billMasterViewModel.BILL_YEAR);
            ViewBag.BILL_ID = URLEncrypt.EncryptParameters(new string[] { Convert.ToInt64(strParameters[0]).ToString().Trim() });
            ModelState.Remove("BILL_NO");
            if (ModelState.IsValid)
            {
                billMasterViewModel.BILL_ID = Convert.ToInt64(strParameters[0]);
                ACC_BILL_MASTER old_acc_bill_master = db.ACC_BILL_MASTER.Find(billMasterViewModel.BILL_ID);
                billMasterViewModel.FUND_TYPE = old_acc_bill_master.FUND_TYPE;
                billMasterViewModel.LVL_ID = old_acc_bill_master.LVL_ID;
                billMasterViewModel.ADMIN_ND_CODE = old_acc_bill_master.ADMIN_ND_CODE;
                ACC_BILL_MASTER acc_bill_master = CloneModel(billMasterViewModel);
                try
                {
                    ValidationSummary = receiptBAL.ValidateEditReceiptMaster(billMasterViewModel);
                    if (ValidationSummary == String.Empty)
                    {
                        db.Entry(old_acc_bill_master).CurrentValues.SetValues(acc_bill_master);
                        db.SaveChanges();
                        //Added by Abhishek Kamble 1-jan-2014
                        String IsMultiTranAllowed = db.ACC_SCREEN_DESIGN_PARAM_MASTER.Where(m => m.TXN_ID == acc_bill_master.TXN_ID).Select(m => m.MTXN_REQ).FirstOrDefault();

                        return this.Json(new { success = true, message = URLEncrypt.EncryptParameters(new string[] { acc_bill_master.BILL_ID.ToString().Trim() }), IsMultiTranAllowed = IsMultiTranAllowed, ReceiptGrossAmount = acc_bill_master.GROSS_AMOUNT });
                    }
                    else
                    {
                        return this.Json(new { success = false, message = ValidationSummary });
                    }
                }
                catch (DbEntityValidationException ex)
                {
                    Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                    var errorMessages = ex.EntityValidationErrors.SelectMany(x => x.ValidationErrors).Select(x => x.ErrorMessage);
                    var fullErrorMessage = string.Join("; ", errorMessages);
                    var exceptionMessage = string.Concat(ex.Message, " The validation errors are: ", fullErrorMessage);
                    throw new DbEntityValidationException(exceptionMessage, ex.EntityValidationErrors);
                }

                //return PartialView("AddEditChequeBook",new ChequeBookViewModel());
            }
            else
            {
                TransactionParams objMaster = new TransactionParams();
                objMaster.BILL_TYPE = "R";
                objMaster.FUND_TYPE = PMGSYSession.Current.FundType;
                objMaster.LVL_ID = PMGSYSession.Current.LevelId;
                objMaster.TXN_ID = 0;
                objMaster.OP_MODE = "A";
                ViewBag.ddlMasterTrans = commomFuncObj.PopulateTransactions(objMaster);
            }

            return PartialView("ReceiptMaster", billMasterViewModel);
        }

        [HttpPost]
        [Audit]
        [ValidateAntiForgeryToken]
        public ActionResult AddReceiptDetails(BillDetailsViewModel billDetailsViewModel, String parameter, String hash, String key)
        {
            String ValidationSummary = String.Empty;
            string[] strParameters = URLEncrypt.DecryptParameters(new string[] { parameter, hash, key });
            if (strParameters.Length == 0)
            {
                return this.Json(new { success = false, message = "Error while Adding Receipt Details" });
            }
            else
            {
                billDetailsViewModel.BILL_ID = Convert.ToInt64(strParameters[0]);
            }
            ACC_BILL_MASTER acc_bill_master = db.ACC_BILL_MASTER.Where(m => m.BILL_ID == billDetailsViewModel.BILL_ID).FirstOrDefault();
            TransactionParams objMaster = new TransactionParams();
            objMaster.BILL_TYPE = "R";
            objMaster.FUND_TYPE = PMGSYSession.Current.FundType;
            objMaster.LVL_ID = PMGSYSession.Current.LevelId;
            objMaster.TXN_ID = acc_bill_master.TXN_ID;
            objMaster.ADMIN_ND_CODE = PMGSYSession.Current.AdminNdCode; // Take it from session
            objMaster.MAST_CONT_ID = 0; // For initial condition 
            objMaster.OP_MODE = "A";
            objMaster.CASH_CHQ = acc_bill_master.CHQ_EPAY;
            ViewBag.ddlSubTrans = commomFuncObj.PopulateTransactions(objMaster);

            ViewBag.ddlContractor = commomFuncObj.PopulateContractorSupplier(objMaster); //new SelectListItem { Text = "--Select--", Value="0", Selected=true};
            ViewBag.ddlAgreement = commomFuncObj.PopulateAgreement(objMaster);//new SelectListItem { Text = "--Select--", Value = "0", Selected = true };
            ViewBag.ddlDPIU = commomFuncObj.PopulateDPIU(objMaster); //new SelectListItem { Text = "--Select--", Value = "0", Selected = true };
            billDetailsViewModel.EncryptedBillId = URLEncrypt.EncryptParameters(new string[] { billDetailsViewModel.BILL_ID.ToString().Trim() });
            if (ModelState.IsValid)
            {
                try
                {
                    ValidationSummary = receiptBAL.ValidateAddReceiptDetails(billDetailsViewModel, "A");
                    if (ValidationSummary == String.Empty)
                    {
                        // to check if correct entry as is operational and is required after porting flag
                        string correctionStatus = commomFuncObj.ValidateHeadForCorrection(Convert.ToInt16(billDetailsViewModel.TXN_ID.Split('$')[0]), 0, "A");
                        if (correctionStatus != "1")
                        {
                            return this.Json(new { success = false, message = "Invalid Transaction Type" });
                        }

                        string success = receiptBAL.AddReceiptDetails(billDetailsViewModel);
                        //db.ACC_BILL_DETAILS.Add(acc_bill_master);
                        //db.SaveChanges();
                        if (success == String.Empty)
                        {
                            return this.Json(new { success = true, message = URLEncrypt.EncryptParameters(new string[] { billDetailsViewModel.BILL_ID.ToString().Trim() }), encBillId = billDetailsViewModel.EncryptedBillId });
                            //return PartialView("AddEditReceiptDetails", billDetailsViewModel);
                        }
                        else
                        {
                            return this.Json(new { success = false, message = URLEncrypt.EncryptParameters(new string[] { billDetailsViewModel.BILL_ID.ToString().Trim() }) });
                        }
                    }
                    else
                    {
                        return this.Json(new { success = false, message = ValidationSummary });
                    }
                }
                catch (DbEntityValidationException ex)
                {
                    Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                    var errorMessages = ex.EntityValidationErrors.SelectMany(x => x.ValidationErrors).Select(x => x.ErrorMessage);
                    var fullErrorMessage = string.Join("; ", errorMessages);
                    var exceptionMessage = string.Concat(ex.Message, " The validation errors are: ", fullErrorMessage);
                    throw new DbEntityValidationException(exceptionMessage, ex.EntityValidationErrors);
                }

                //return PartialView("AddEditChequeBook",new ChequeBookViewModel());
            }

            return View("ReceiptDetails", billDetailsViewModel);

        }

        [HttpPost]
        [Audit]
        [ValidateAntiForgeryToken]
        public ActionResult EditReceiptDetails(BillDetailsViewModel billDetailsViewModel, String parameter, String hash, String key)
        {
            String ValidationSummary = String.Empty;

            string[] strParameters = URLEncrypt.DecryptParameters(new string[] { parameter, hash, key });
            billDetailsViewModel.BILL_ID = Convert.ToInt64(strParameters[0].Split('$')[0]);
            billDetailsViewModel.TXN_NO = Convert.ToInt16(strParameters[0].Split('$')[1]);
            try
            {
                ACC_BILL_MASTER acc_bill_master = db.ACC_BILL_MASTER.Where(m => m.BILL_ID == billDetailsViewModel.BILL_ID).FirstOrDefault();
                TransactionParams objMaster = new TransactionParams();
                objMaster.BILL_TYPE = "R";
                objMaster.FUND_TYPE = PMGSYSession.Current.FundType;
                objMaster.LVL_ID = PMGSYSession.Current.LevelId;
                objMaster.TXN_ID = acc_bill_master.TXN_ID;
                objMaster.ADMIN_ND_CODE = PMGSYSession.Current.AdminNdCode; // Take it from session
                objMaster.MAST_CONT_ID = 0; // For initial condition 
                objMaster.CASH_CHQ = acc_bill_master.CHQ_EPAY;
                objMaster.OP_MODE = "E";
                ViewBag.ddlSubTrans = commomFuncObj.PopulateTransactions(objMaster);

                ViewBag.ddlContractor = commomFuncObj.PopulateContractorSupplier(objMaster); //new SelectListItem { Text = "--Select--", Value="0", Selected=true};
                ViewBag.ddlAgreement = commomFuncObj.PopulateAgreement(objMaster);//new SelectListItem { Text = "--Select--", Value = "0", Selected = true };
                ViewBag.ddlDPIU = commomFuncObj.PopulateDPIU(objMaster); //new SelectListItem { Text = "--Select--", Value = "0", Selected = true };
                ValidationSummary = receiptBAL.ValidateAddReceiptDetails(billDetailsViewModel, "E");
                if (ModelState.IsValid)
                {
                    if (ValidationSummary == String.Empty)
                    {
                        // to check if correct entry as is operational and is required after porting flag
                        string correctionStatus = commomFuncObj.ValidateHeadForCorrection(Convert.ToInt16(billDetailsViewModel.TXN_ID.Split('$')[0]), 0, "E");
                        if (correctionStatus == "0")
                        {
                            return this.Json(new { success = false, message = "Edit Opearion is not allowed for this Transaction Type." });
                        }

                        string success = receiptBAL.EditReceiptDetails(billDetailsViewModel);

                        if (success == String.Empty)
                        {
                            return this.Json(new { success = true, message = URLEncrypt.EncryptParameters(new string[] { billDetailsViewModel.BILL_ID.ToString().Trim() }) });
                        }
                        else
                        {
                            return this.Json(new { success = false, message = URLEncrypt.EncryptParameters(new string[] { billDetailsViewModel.BILL_ID.ToString().Trim() }) });
                        }
                    }
                    else
                    {
                        return this.Json(new { success = false, message = ValidationSummary });
                    }
                }
            }
            catch (DbEntityValidationException ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                var errorMessages = ex.EntityValidationErrors.SelectMany(x => x.ValidationErrors).Select(x => x.ErrorMessage);
                var fullErrorMessage = string.Join("; ", errorMessages);
                var exceptionMessage = string.Concat(ex.Message, " The validation errors are: ", fullErrorMessage);
                throw new DbEntityValidationException(exceptionMessage, ex.EntityValidationErrors);
            }

            //return PartialView("AddEditChequeBook",new ChequeBookViewModel());

            return View("ReceiptDetails", billDetailsViewModel);
        }

        [Audit]
        public ActionResult DeleteReceiptDetails(String parameter, String hash, String key)
        {
            string[] strParameters = URLEncrypt.DecryptParameters(new string[] { parameter, hash, key });
            string status = receiptBAL.DeleteReceiptDetails(Convert.ToInt64(strParameters[0].Split('$')[0]), Convert.ToInt16(strParameters[0].Split('$')[1]));
            if (status == String.Empty)
            {
                return this.Json(new { success = true, message = String.Empty });
            }
            else
            {
                return this.Json(new { success = false, message = status });
            }
        }

        [Audit]
        public ActionResult FinalizeReceipt(String parameter, String hash, String key)
        {
            string[] strParameters = URLEncrypt.DecryptParameters(new string[] { parameter, hash, key });

            // to check if correct entry as is operational and is required after porting flag
            string correctionStatus = commomFuncObj.ValidateHeadForCorrection(0, Convert.ToInt64(strParameters[0]), "F");
            if (correctionStatus != "1")
            {
                return this.Json(new { success = false, message = "Receipt cant be finalized as on of its sub transaction has invalid transaction type" });
            }



            string status = receiptBAL.FinalizeReceipt(Convert.ToInt64(strParameters[0]));
            if (status == String.Empty)
            {
                return this.Json(new { success = true, message = URLEncrypt.EncryptParameters(new string[] { strParameters[0] }) });
            }
            else
            {
                return this.Json(new { success = false, message = status });
            }
        }

        [Audit]
        public ActionResult DeleteReceipt(String parameter, String hash, String key)
        {
            string[] strParameters = URLEncrypt.DecryptParameters(new string[] { parameter, hash, key });
            string status = receiptBAL.DeleteReceipt(Convert.ToInt64(strParameters[0]));
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
        public JsonResult GetDetailsDesignMode(String id)
        {

            TransactionParams objParam = new TransactionParams();
            objParam.TXN_ID = Convert.ToInt16(id.Split('$')[0]);
            ACC_SCREEN_DESIGN_PARAM_DETAILS objDesignParam = new ACC_SCREEN_DESIGN_PARAM_DETAILS();
            objDesignParam = commomFuncObj.getDetailsDesignParam(objParam);
            if (objDesignParam == null)
            {
                return Json(new { CON_REQ = "N", AGREEMENT_REQ = "N", PIU_REQ = "N", SUP_REQ = "N", ROAD_REQ = "N" });
            }
            else
            {
                return Json(new { CON_REQ = objDesignParam.CON_REQ, AGREEMENT_REQ = objDesignParam.AGREEMENT_REQ, PIU_REQ = objDesignParam.PIU_REQ, SUP_REQ = objDesignParam.SUPPLIER_REQ, ROAD_REQ = objDesignParam.ROAD_REQ });
            }
        }

        [HttpPost]
        [Audit]
        public JsonResult GetMasterDesignParams(String id)
        {

            TransactionParams objParam = new TransactionParams();
            objParam.TXN_ID = Convert.ToInt16(id.Split('$')[0]);
            ACC_SCREEN_DESIGN_PARAM_MASTER objDesignParam = new ACC_SCREEN_DESIGN_PARAM_MASTER();
            objDesignParam = commomFuncObj.getMasterDesignParam(objParam);
            if (objDesignParam == null)
            {
                return Json(new { CASH_CHQ = "N", AGREEMENT_REQ = "N", CON_REQ = "N", SUP_REQ = "N", MTXN_REQ = "N" });
            }
            else
            {
                return Json(new { CASH_CHQ = objDesignParam.CASH_CHQ, AGREEMENT_REQ = objDesignParam.MAST_AGREEMENT_REQ, CON_REQ = objDesignParam.MAST_CON_REQ, SUP_REQ = objDesignParam.MAST_SUPPLIER_REQ, MTXN_REQ = objDesignParam.MTXN_REQ });
            }
        }

        [HttpPost]
        [Audit]
        public JsonResult GetMasterDesignParamsByBillId(String parameter, String hash, String key)
        {
            string[] strParameters = URLEncrypt.DecryptParameters(new string[] { parameter, hash, key });
            Int64 billId = Convert.ToInt64(strParameters[0]);
            return GetMasterDesignParams(db.ACC_BILL_MASTER.Where(m => m.BILL_ID == billId).Select(m => m.TXN_ID).FirstOrDefault().ToString());
        }

        [HttpPost]
        [Audit]
        public JsonResult getDetailsTransDetails(String parameter, String hash, String key)
        {
            string[] strParameters = URLEncrypt.DecryptParameters(new string[] { parameter, hash, key });
            Int64 billId = Convert.ToInt64(strParameters[0]);
            Int64 count = db.ACC_BILL_DETAILS.Where(m => m.BILL_ID == billId && m.CREDIT_DEBIT == "D").Count();
            ACC_BILL_MASTER acc_bill_master = new ACC_BILL_MASTER();
            acc_bill_master = (from item in db.ACC_BILL_MASTER
                               where item.BILL_ID == billId
                               select item).FirstOrDefault();
            String transId = acc_bill_master.TXN_ID.ToString().Trim() + "$" + acc_bill_master.CHQ_EPAY.ToString().Trim();
            return Json(new { ReceiptCount = count, TransId = transId });
        }

        [Audit]
        public JsonResult GetTransactionDetails(String parameter, String hash, String key)
        {
            string[] strParameters = URLEncrypt.DecryptParameters(new string[] { parameter, hash, key });
            Int64 billId = Convert.ToInt64(strParameters[0]);
            ACC_BILL_DETAILS acc_bill_details = new ACC_BILL_DETAILS();
            Int32? deptCode = db.ACC_BILL_DETAILS.Where(m => m.BILL_ID == billId).Select(m => m.ADMIN_ND_CODE).FirstOrDefault();
            Int32? AggCode = db.ACC_BILL_DETAILS.Where(m => m.BILL_ID == billId).Select(m => m.IMS_AGREEMENT_CODE).FirstOrDefault();
            Int32? ConCode = db.ACC_BILL_DETAILS.Where(m => m.BILL_ID == billId).Select(m => m.MAST_CON_ID).FirstOrDefault();
            return Json(new { Con = ConCode, Agg = AggCode, Dept = deptCode });
        }

        [Audit]
        public BillMasterViewModel CloneObject(ACC_BILL_MASTER acc_bill_master)
        {
            commomFuncObj = new CommonFunctions();
            BillMasterViewModel billMasterViewModel = new BillMasterViewModel();
            billMasterViewModel.BILL_ID = acc_bill_master.BILL_ID;
            billMasterViewModel.BILL_NO = acc_bill_master.BILL_NO;
            billMasterViewModel.BILL_DATE = commomFuncObj.GetDateTimeToString(acc_bill_master.BILL_DATE);
            billMasterViewModel.BILL_MONTH = (byte)acc_bill_master.BILL_DATE.Month;
            billMasterViewModel.BILL_YEAR = (short)acc_bill_master.BILL_DATE.Year;
            billMasterViewModel.CHQ_NO = acc_bill_master.CHQ_NO;
            billMasterViewModel.CHQ_DATE = acc_bill_master.CHQ_DATE == null ? "" : Convert.ToDateTime(acc_bill_master.CHQ_DATE).ToString("dd/MM/yyyy");
            billMasterViewModel.CHQ_AMOUNT = acc_bill_master.CHQ_AMOUNT;
            billMasterViewModel.CASH_AMOUNT = acc_bill_master.CASH_AMOUNT;
            billMasterViewModel.GROSS_AMOUNT = acc_bill_master.GROSS_AMOUNT;
            billMasterViewModel.CHQ_EPAY = acc_bill_master.CHQ_EPAY;
            billMasterViewModel.BILL_FINALIZED = acc_bill_master.BILL_FINALIZED;
            billMasterViewModel.ADMIN_ND_CODE = acc_bill_master.ADMIN_ND_CODE;
            billMasterViewModel.FUND_TYPE = acc_bill_master.FUND_TYPE;
            billMasterViewModel.LVL_ID = acc_bill_master.LVL_ID;
            billMasterViewModel.BILL_TYPE = acc_bill_master.BILL_TYPE;
            billMasterViewModel.TXN_ID = acc_bill_master.TXN_ID.ToString().Trim() + "$" + db.ACC_MASTER_TXN.Where(m => m.TXN_ID == acc_bill_master.TXN_ID).Select(m => m.CASH_CHQ).FirstOrDefault().Trim();
            return billMasterViewModel;
        }

        [Audit]
        public ACC_BILL_MASTER CloneModel(BillMasterViewModel billMasterViewModel)
        {
            commomFuncObj = new CommonFunctions();
            ACC_BILL_MASTER acc_bill_master = new ACC_BILL_MASTER();
            acc_bill_master.BILL_ID = billMasterViewModel.BILL_ID;
            acc_bill_master.BILL_NO = billMasterViewModel.BILL_NO;
            acc_bill_master.BILL_DATE = commomFuncObj.GetStringToDateTime(billMasterViewModel.BILL_DATE);
            acc_bill_master.BILL_MONTH = billMasterViewModel.BILL_MONTH;
            acc_bill_master.BILL_YEAR = billMasterViewModel.BILL_YEAR;
            acc_bill_master.TXN_ID = Convert.ToInt16(billMasterViewModel.TXN_ID.Split('$')[0]);
            acc_bill_master.CHQ_EPAY = billMasterViewModel.CHQ_EPAY;
            acc_bill_master.GROSS_AMOUNT = Convert.ToDecimal(billMasterViewModel.GROSS_AMOUNT);
            if (acc_bill_master.CHQ_EPAY == "Q")
            {
                acc_bill_master.CHQ_NO = billMasterViewModel.CHQ_NO;
                acc_bill_master.CHQ_DATE = commomFuncObj.GetStringToDateTime(billMasterViewModel.CHQ_DATE);
                acc_bill_master.CHQ_AMOUNT = acc_bill_master.GROSS_AMOUNT;
            }
            else if (acc_bill_master.CHQ_EPAY == "C")
            {
                acc_bill_master.CHQ_NO = null;
                acc_bill_master.CHQ_DATE = null;
                acc_bill_master.CASH_AMOUNT = acc_bill_master.GROSS_AMOUNT;
                //acc_bill_master.CHQ_EPAY = null; // chq_epay accepting E or Q only.
            }

            acc_bill_master.TEO_TRANSFER_TYPE = null;
            acc_bill_master.BILL_FINALIZED = "N";
            acc_bill_master.ADMIN_ND_CODE = billMasterViewModel.ADMIN_ND_CODE;
            acc_bill_master.FUND_TYPE = billMasterViewModel.FUND_TYPE;
            acc_bill_master.LVL_ID = (byte)billMasterViewModel.LVL_ID;
            acc_bill_master.BILL_TYPE = "R";

            //added by abhishek kamble 28-nov-2013
            acc_bill_master.USERID = PMGSYSession.Current.UserId;
            acc_bill_master.IPADD= Request.ServerVariables["REMOTE_ADDR"];
            
            return acc_bill_master;
        }

        [Audit]
        public JsonResult PopulateAgreement(String id)
        {
            TransactionParams objparams = new TransactionParams();
            try
            {

            objparams.DISTRICT_CODE = PMGSYSession.Current.DistrictCode;
            objparams.MAST_CONT_ID = Convert.ToInt32(id.Trim());
            return Json(commomFuncObj.PopulateAgreement(objparams));
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "Receipt.PopulateAgreement");
                return null;
            }
        }

        [HttpPost]
        [Audit]
        public String GetContractorName(String id)
        {
            try
            {
                TransactionParams objParam = new TransactionParams();
                objParam.MAST_CONT_ID = Convert.ToInt32(id);
                //return commomFuncObj.GetContractorSupplierName(objParam);

                if (objParam.MAST_CONT_ID == -1)
                {
                    return "No Contractor/Supplier";

                }
                else
                {
                    return commomFuncObj.GetContractorSupplierName(objParam);

                }
            }
            catch (Exception ex)
            {
                //Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                ErrorLog.LogError(ex, "Receipt.GetContractorName");
                return String.Empty;
            }
        }

        [HttpPost]       
        public String GetNarration(String parameter, String hash, String key)
        {
            String TransID = Request.Params["transId"].ToString();
            String[] strParameters = URLEncrypt.DecryptParameters(new string[] { parameter, hash, key });
            return commomFuncObj.GetTransNarration(Convert.ToInt16(TransID), strParameters[0], "R");
        }

       
        public String GetImprestAmount(String parameter, String hash, String key)
        {
            String[] strParameters = URLEncrypt.DecryptParameters(new string[] { parameter, hash, key });
            return receiptBAL.GetImprestAmount(Convert.ToInt64(strParameters[0].Split('$')[0]), Convert.ToInt16(strParameters[0].Split('$')[1]));
        }

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

        public JsonResult GetPIU(String parameter, String hash, String key)
        {
            try
            {
                CommonFunctions objCommon = new CommonFunctions();
                TransactionParams objParam = new TransactionParams();
                if (!String.IsNullOrEmpty(parameter) && !String.IsNullOrEmpty(hash) && !String.IsNullOrEmpty(key))
                {

                    if (!String.IsNullOrEmpty(parameter) && !String.IsNullOrEmpty(hash) && !String.IsNullOrEmpty(key))
                    {
                        String[] urlParams = URLEncrypt.DecryptParameters(new String[] { parameter, hash, key });
                        if (urlParams.Length >= 1)
                        {

                            objParam.BILL_ID = Convert.ToInt64(urlParams[0]);
                        }
                    }
                    else
                    {
                        throw new Exception("Error while getting  payment details..");
                    }
                }

                List<SelectListItem> lstPIU = new List<SelectListItem>();
                using (PMGSYEntities db = new PMGSYEntities())
                {   
                    int? adminCode = db.ACC_BILL_DETAILS.Where(m=>m.BILL_ID == objParam.BILL_ID).Select(m=>m.ADMIN_ND_CODE).FirstOrDefault();
                    var list = db.ADMIN_DEPARTMENT.Where(m => m.MAST_ND_TYPE == "D" && m.MAST_PARENT_ND_CODE == PMGSYSession.Current.AdminNdCode).ToList();
                    foreach (var item in list)
                    {
                        if (item.ADMIN_ND_CODE == adminCode)
                        {
                            lstPIU.Add(new SelectListItem { Value = item.ADMIN_ND_CODE.ToString(), Text = item.ADMIN_ND_NAME, Selected = true });
                        }
                        else
                        {
                            lstPIU.Add(new SelectListItem { Value = item.ADMIN_ND_CODE.ToString(), Text = item.ADMIN_ND_NAME, });
                        }
                    }
                    lstPIU.Insert(0, new SelectListItem { Value = "0",Text = "Select PIU"});
                }

                return Json(lstPIU);
            }
            catch (Exception)
            {
                return null;
            }
            
        }

        #region ISVALID_TRANSACTION

        /// <summary>
        /// Checks whether the master transaction cash_cheque flag matches with the screen design parameters or not
        /// </summary>
        /// <param name="billId">master transaction id</param>
        /// <returns>true/false</returns>
        public ActionResult IsValidTransaction(String parameter, String hash, String key)
        {
            string message = string.Empty;
            bool status = true;
            try
            {
                if (!String.IsNullOrEmpty(parameter) && !String.IsNullOrEmpty(hash) && !String.IsNullOrEmpty(key))
                {
                    String[] urlParams = URLEncrypt.DecryptParameters(new String[] { parameter, hash, key });
                    if (urlParams.Length >= 1)
                    {
                        long billId = Convert.ToInt64(urlParams[0]);
                        
                        status = commomFuncObj.IsValidTransaction(billId,ref message);
                    }
                }
                return Json(new { success = status , message = message});
            }
            catch (Exception)
            {
                return Json(new { success = false});
            }
        }

        #endregion



        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}


