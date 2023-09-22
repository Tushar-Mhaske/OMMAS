using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PMGSY.BAL.OB;
using PMGSY.Common;
using PMGSY.Models.OB;
using PMGSY.Models;
using PMGSY.Extensions;
using PMGSY.Models.Receipts;
using PMGSY.Models.Common;

namespace PMGSY.Controllers
{
    [RequiredAuthentication]
    [RequiredAuthorization]
    public class OBController : Controller
    {
        private OpeningBalanceBAL objBAL = null;
        private CommonFunctions commonFuncObj = null;

        [Audit]
        //[GenericAccountValidationFilter(InputParameter = new string[] { "BankDetails", "AuthSign" })]
        [GenericAccountValidationFilter(InputParameter = new string[] { "BankDetails", "AuthSign", "SrrdaOBEntered" })]
        public ActionResult OpeningBalance()
        {
            objBAL = new OpeningBalanceBAL();
            TransactionParams objParam = new TransactionParams();
            objParam.ADMIN_ND_CODE = PMGSYSession.Current.AdminNdCode;
            objParam.STATE_CODE = PMGSYSession.Current.StateCode;
            objParam.DISTRICT_CODE = PMGSYSession.Current.DistrictCode;
            objParam.FUND_TYPE = PMGSYSession.Current.FundType;
            objParam.BILL_TYPE = "O";
            objParam.LVL_ID = PMGSYSession.Current.LevelId;
            ViewBag.IsAccountEntry = objBAL.GetAccountStatus(objParam);
            ViewBag.FinalizeStatus = objBAL.GetOBStatus(objParam);
            ViewBag.FundType = PMGSYSession.Current.FundType;
            return View();
        }

        [HttpPost]
        [Audit]
        public ActionResult GetOBMasterList(FormCollection homeFormCollection)
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
            objBAL = new OpeningBalanceBAL();
            ReceiptFilterModel objFilter = new ReceiptFilterModel();
            long totalRecords;
            objFilter.page = Convert.ToInt32(homeFormCollection["page"]) - 1;
            objFilter.rows = Convert.ToInt32(homeFormCollection["rows"]);
            objFilter.sidx = homeFormCollection["sidx"].ToString();
            objFilter.sord = homeFormCollection["sord"].ToString();
            objFilter.AdminNdCode = PMGSYSession.Current.AdminNdCode;
            objFilter.FundType = PMGSYSession.Current.FundType;
            objFilter.LevelId = PMGSYSession.Current.LevelId;
            objFilter.BillType = "O";

            var jsonData = new
            {
                rows = objBAL.GetOBMasterList(objFilter, out totalRecords),
                total = totalRecords <= objFilter.rows ? 1 : totalRecords / objFilter.rows + 1,
                page = objFilter.page + 1,
                records = totalRecords
            };
            return Json(jsonData);
        }

        [Audit]
        public ActionResult GetOBDetailsList(FormCollection frmCollect)
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
            objBAL = new OpeningBalanceBAL();
            long totalRecords = 0;
            Decimal GrossAmount = 0;
            Decimal AssetTotal = 0;
            Decimal LibTotal = 0;
            String Finalized = "N";
            string[] parameters = Request.Params["masterId"].ToString().Split('/');
            string[] strParameters = URLEncrypt.DecryptParameters(new string[] { parameters[0], parameters[1], parameters[2] });
            //Added by abhishek kamble 10-dec-2013
            Decimal AssetRemainingAmount = 0;
            Decimal LibRemainingAmount = 0;

            if (strParameters.Length > 0)
            {
                if (!strParameters[0].Contains("_"))
                {
                    String OBMasterIds = objBAL.GetOBMasterIds(Convert.ToInt64(strParameters[0].Trim()));
                    objFilter.AssetBillId = Convert.ToInt64(OBMasterIds.Split('$')[0]);
                    objFilter.LibBillId = Convert.ToInt64(OBMasterIds.Split('$')[1]);
                }
                else// if (strParameters[0].Contains("_"))
                {
                    objFilter.AssetBillId = Convert.ToInt64(strParameters[0].Split('_')[0]);
                    objFilter.LibBillId = Convert.ToInt64(strParameters[0].Split('_')[1]);
                }
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
            Array lstTEODetails = objBAL.GetOBDetailsList(objFilter, out totalRecords, out AssetTotal, out LibTotal, out GrossAmount, out Finalized);
            if ((GrossAmount == AssetTotal) && (GrossAmount == LibTotal) && (Finalized == "N"))
            {
                finFlag = "Y";
            }

            AssetRemainingAmount = GrossAmount - AssetTotal;
            LibRemainingAmount= GrossAmount - LibTotal;

            var jsonData = new
            {

                page = objFilter.page + 1,
                records = totalRecords,
                rows = lstTEODetails,
                total = 0,
                //modified by abhishek kamble 10-dec-2013
                //userdata = new { DPIU = "<label style='color:#125806'>Total Amount(In Rs.)</label>", AssetAmount = "<label style='color:#0a8900'>" + AssetTotal.ToString("#0.00") + "</label>", LibAmount = "<label style='color:#125806'>" + LibTotal.ToString("#0.00") + "</label><label style='color:#981b1e'>", isFinalize = finFlag, grossAmount = GrossAmount }
                userdata = new { DPIU = "<label style='color:#125806'>Total Amount(In Rs.)</label>", AssetAmount = "<label style='color:#0a8900'>" + AssetTotal.ToString("#0.00") + "</label>", LibAmount = "<label style='color:#125806'>" + LibTotal.ToString("#0.00") + "</label><label style='color:#981b1e'>", isFinalize = finFlag, grossAmount = GrossAmount,AssetRemainingAmount=AssetRemainingAmount,LibRemainingAmount=LibRemainingAmount }

            };
            return Json(jsonData);
        }

        [Audit]
        public ActionResult AddOBMaster(String parameter, String hash, String key)
        {
            OBMasterModel obMasterModel = new OBMasterModel();
            objBAL = new OpeningBalanceBAL();

            if (parameter != null)
            {
                string[] strParameters = URLEncrypt.DecryptParameters(new string[] { parameter, hash, key });
                Int64 billIdAsset = Convert.ToInt64(strParameters[0].Split('$')[0]);
                Int64 billIdLib = Convert.ToInt64(strParameters[0].Split('$')[1]);
                ViewBag.BILL_ID = URLEncrypt.EncryptParameters(new string[] { billIdAsset.ToString().Trim() + "$" + billIdLib.ToString().Trim() });
                obMasterModel = objBAL.GetOBMasterById(billIdAsset + "$" + billIdLib);
            }
            return PartialView("AddOBMaster", obMasterModel);
        }

        [HttpPost]
        [Audit]
        [ValidateAntiForgeryToken]
        public ActionResult AddOBMaster(OBMasterModel obMasterModel)
        {
            String ValidationSummary = String.Empty;
            commonFuncObj = new CommonFunctions();
            objBAL = new OpeningBalanceBAL();
            CommonFunctions objCommon = new CommonFunctions();

            obMasterModel.FUND_TYPE = PMGSYSession.Current.FundType;
            obMasterModel.LVL_ID = PMGSYSession.Current.LevelId;
            obMasterModel.ADMIN_ND_CODE = PMGSYSession.Current.AdminNdCode;

            if (ModelState.IsValid)
            {
                PMGSYEntities db = new PMGSYEntities();
                int? srrdaCode = db.ADMIN_DEPARTMENT.Where(m=>m.ADMIN_ND_CODE == PMGSYSession.Current.AdminNdCode).Select(m=>m.MAST_PARENT_ND_CODE).FirstOrDefault();
                if (srrdaCode != null)
                {
                    if (db.ACC_BILL_MASTER.Where(m => m.ADMIN_ND_CODE == srrdaCode && m.FUND_TYPE == PMGSYSession.Current.FundType && m.BILL_TYPE == "O" && m.BILL_FINALIZED == "Y").Select(m => m.BILL_DATE).FirstOrDefault() > objCommon.GetStringToDateTime(obMasterModel.ASSET_BILL_DATE))
                    {
                        return this.Json(new { success = false, message = "OB Date must be greater than or equal to SRRDA OB Date." });
                    }
                }

                //added by koustubh nakate on 18/07/2013 for checking previous month is closed

                 int? parent_nd_code = db.ADMIN_DEPARTMENT.Where(m => m.ADMIN_ND_CODE == PMGSYSession.Current.AdminNdCode).Select(m => m.MAST_PARENT_ND_CODE).FirstOrDefault();
                 if (parent_nd_code != 1131)
                 {
                     if (objCommon.GetStringToDateTime(obMasterModel.ASSET_BILL_DATE).Month != 0 && objCommon.GetStringToDateTime(obMasterModel.ASSET_BILL_DATE).Month < 13 && objCommon.GetStringToDateTime(obMasterModel.ASSET_BILL_DATE).Year != 0)
                     {
                         string monthlyClosingStatus = string.Empty;

                         String message = String.Empty;
                         monthlyClosingStatus = objCommon.MonthlyClosingValidation((Int16)objCommon.GetStringToDateTime(obMasterModel.ASSET_BILL_DATE).Month, (Int16)objCommon.GetStringToDateTime(obMasterModel.ASSET_BILL_DATE).Year, obMasterModel.FUND_TYPE, obMasterModel.LVL_ID, obMasterModel.ADMIN_ND_CODE, ref message);

                         if (monthlyClosingStatus.Equals("-111"))
                         {
                             ModelState.AddModelError("BILL_MONTH", message);
                         }
                         if (monthlyClosingStatus.Equals("-222"))
                         {
                             ModelState.AddModelError("BILL_MONTH", "Month is already closed.");
                         }

                     }

                     //end added by koustubh nakate on 18/07/2013 for checking previous month is closed
                 }
            }

            if (ModelState.IsValid)
            {

                obMasterModel.CHQ_EPAY = "O";
                obMasterModel.BILL_TYPE = "O";
                obMasterModel.BILL_FINALIZED = "N";

                //ValidationSummary = chequeBookBAL.ValidateAddEditChequeBookDetails(chequeBookViewModel);
                if (ValidationSummary == String.Empty)
                {
                    Int64 billId = objBAL.AddOBMaster(obMasterModel);
                    return this.Json(new { success = true, message = URLEncrypt.EncryptParameters(new string[] { billId.ToString().Trim() }) });
                }
                else
                {
                    return this.Json(new { success = false, message = ValidationSummary });
                }

            }
            return PartialView("AddOBMaster", obMasterModel);
        }

        [HttpPost]
        [Audit]
        [ValidateAntiForgeryToken]
        public ActionResult EditOBMaster(OBMasterModel obMasterModel, String parameter, String hash, String key)
        {
            String ValidationSummary = String.Empty;
            objBAL = new OpeningBalanceBAL();
            commonFuncObj = new CommonFunctions();
            string[] strParameters = URLEncrypt.DecryptParameters(new string[] { parameter, hash, key });
            Int64 billIdAsset = Convert.ToInt64(strParameters[0].Split('$')[0]);
            Int64 billIdLib = Convert.ToInt64(strParameters[0].Split('$')[1]);
            ViewBag.BILL_ID = URLEncrypt.EncryptParameters(new string[] { billIdAsset.ToString().Trim() + "$" + billIdLib.ToString().Trim() });

            if (ModelState.IsValid)
            {
                obMasterModel.ASSET_BILL_ID = billIdAsset;
                obMasterModel.LIB_BILL_ID = billIdLib;
                //ValidationSummary = objBAL.ValidateEditTEOMaster(obMasterModel);

                PMGSYEntities db = new PMGSYEntities();
                int? srrdaCode = db.ADMIN_DEPARTMENT.Where(m => m.ADMIN_ND_CODE == PMGSYSession.Current.AdminNdCode).Select(m => m.MAST_PARENT_ND_CODE).FirstOrDefault();
                if (srrdaCode != null)
                {
                    if (db.ACC_BILL_MASTER.Where(m => m.ADMIN_ND_CODE == srrdaCode && m.FUND_TYPE == PMGSYSession.Current.FundType && m.BILL_TYPE == "O" && m.BILL_FINALIZED == "Y").Select(m => m.BILL_DATE).FirstOrDefault() > commonFuncObj.GetStringToDateTime(obMasterModel.ASSET_BILL_DATE))
                    {
                        return this.Json(new { success = false, message = "OB Date must be greater than or equal to SRRDA OB Date." });
                    }
                }
                
                
                if (ValidationSummary == String.Empty)
                {
                    string status = objBAL.EditOBMaster(obMasterModel);
                    if (status == "")
                    {
                        return this.Json(new { success = true, message = URLEncrypt.EncryptParameters(new string[] { billIdAsset.ToString().Trim() + "$" + billIdLib.ToString().Trim() }) });
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
            return PartialView("AddOBMaster", obMasterModel);
        }

        [Audit]
        public ActionResult DeleteOBMaster(String parameter, String hash, String key)
        {
            objBAL = new OpeningBalanceBAL();
            string[] strParameters = URLEncrypt.DecryptParameters(new string[] { parameter, hash, key });
            Int64 billIdAsset = Convert.ToInt64(strParameters[0].Split('$')[0]);
            Int64 billIdLib = Convert.ToInt64(strParameters[0].Split('$')[1]);
            string status = objBAL.DeleteOBMaster(billIdAsset + "$" + billIdLib);
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
        public ActionResult AddOBDetails(String parameter, String hash, String key, String id)
        {
            String[] strParameters = URLEncrypt.DecryptParameters(new string[] { parameter, hash, key });
            objBAL = new OpeningBalanceBAL();
            commonFuncObj = new CommonFunctions();
            Int64 billId = 0;
            PMGSYEntities dbContext = new PMGSYEntities();
            OBDetailsModel obDetailsModel = new OBDetailsModel();
            if (id == null)
            {
                billId = Convert.ToInt64(strParameters[0].Split('$')[0]);
                ViewBag.BillId = URLEncrypt.EncryptParameters(new string[] { billId.ToString().Trim() });
            }
            else
            {
                if (id == "1")
                {
                    billId = Convert.ToInt64(strParameters[0].Split('_')[0]);
                }
                else
                {
                    billId = Convert.ToInt64(strParameters[0].Split('_')[1]);
                }
                ViewBag.BillId = URLEncrypt.EncryptParameters(new string[] { strParameters[0].ToString().Trim() });
            }

            String TransId = null;
            ACC_BILL_MASTER acc_bill_master = new ACC_BILL_MASTER();
            acc_bill_master = objBAL.GetOBMaster(billId);
            //new change done by Vikram for getting the Head id of this transaction
            ACC_BILL_DETAILS billDetails = dbContext.ACC_BILL_DETAILS.Where(m => m.BILL_ID == billId).FirstOrDefault();
            //end of transaction
            if (acc_bill_master.BILL_NO == "1")
            {
                ViewBag.AssetLib = "A";
            }
            else
            {
                ViewBag.AssetLib = "L";
            }
            TransactionParams objMaster = new TransactionParams();
            objMaster.BILL_TYPE = acc_bill_master.BILL_TYPE;
            objMaster.FUND_TYPE = acc_bill_master.FUND_TYPE;
            objMaster.LVL_ID = acc_bill_master.LVL_ID;
            objMaster.BILL_NO = acc_bill_master.BILL_NO;
            objMaster.ADMIN_ND_CODE = acc_bill_master.ADMIN_ND_CODE;
            objMaster.TXN_ID = 0;
            objMaster.MAST_CONT_ID = 0;
            objMaster.BILL_ID = billId;
            objMaster.STATE_CODE = PMGSYSession.Current.StateCode;
            objMaster.DISTRICT_CODE = PMGSYSession.Current.DistrictCode;
            if (id == null) { objMaster.OP_MODE = "E"; } else { objMaster.OP_MODE = "A"; }
            ViewBag.ddlTrans = commonFuncObj.PopulateOBTransaction(objMaster);

            if (id == null)
            {
                Int16 TransNo = Convert.ToInt16(strParameters[0].Split('$')[1]);
                obDetailsModel = objBAL.GetOBDetailsByTransId(billId, TransNo);
                obDetailsModel.HDN_TXN_NO = TransNo.ToString();
                TransId = URLEncrypt.EncryptParameters(new string[] { billId.ToString().Trim() + "$" + TransNo.ToString().Trim() });
                objMaster.TXN_ID = obDetailsModel.TXN_ID;
                objMaster.OP_MODE = "E";
                objMaster.HEAD_ID = obDetailsModel.HEAD_ID;
                objMaster.MAST_CONT_ID = obDetailsModel.MAST_CON_ID == null ? 0 : Convert.ToInt32(obDetailsModel.MAST_CON_ID);
                objMaster.AGREEMENT_CODE = obDetailsModel.IMS_AGREEMENT_CODE == null ? 0 : Convert.ToInt32(obDetailsModel.IMS_AGREEMENT_CODE);
                if (obDetailsModel.IMS_PR_ROAD_CODE != null && objMaster.AGREEMENT_CODE == 0)
                {
                    objMaster.SANC_YEAR = commonFuncObj.getSancYearFromRoad(Convert.ToInt32(obDetailsModel.IMS_PR_ROAD_CODE));
                    objMaster.PACKAGE_ID = commonFuncObj.getPackageFromRoad(Convert.ToInt32(obDetailsModel.IMS_PR_ROAD_CODE));
                }

                ViewBag.ddlSubTrans = commonFuncObj.PopulateOBTransaction(objMaster);
                if (obDetailsModel.MAST_CON_ID != null)
                {
                    objMaster.MAST_CONT_ID = Convert.ToInt32(obDetailsModel.MAST_CON_ID);
                }

                //new change done by Vikram for populating Road according to the Maintenance Agreement and Contractor

                if (PMGSYSession.Current.FundType == "M")
                {
                    //Old
                    //objMaster.AGREEMENT_NUMBER = dbContext.MANE_IMS_CONTRACT.Where(m => m.MANE_PR_CONTRACT_CODE == objMaster.AGREEMENT_CODE).Select(m => m.MANE_AGREEMENT_NUMBER).FirstOrDefault();
                    //Modified By Abhishek kamble to get Agr Number using MANE_CONTRACTOR_ID 17Nov2014
                    objMaster.AGREEMENT_NUMBER = dbContext.MANE_IMS_CONTRACT.Where(m => m.MANE_CONTRACT_ID == objMaster.AGREEMENT_CODE).Select(m => m.MANE_AGREEMENT_NUMBER).FirstOrDefault();
                }

                ACC_SCREEN_DESIGN_PARAM_DETAILS designParams = new ACC_SCREEN_DESIGN_PARAM_DETAILS();
                objMaster.TXN_ID = obDetailsModel.SUB_TXN_ID;
                designParams = commonFuncObj.getDetailsDesignParam(objMaster);

                if (designParams != null && designParams.CON_REQ == "Y")
                {
                    ViewBag.ddlContractor = commonFuncObj.PopulateContractorSupplier(objMaster);
                }
                else
                {
                    ViewBag.ddlContractor = new SelectList(new List<SelectListItem>().AsEnumerable<SelectListItem>(), "Value", "Text");
                }
                if (designParams != null && designParams.AGREEMENT_REQ == "Y")
                {
                    ViewBag.ddlAgreement = commonFuncObj.PopulateAgreement(objMaster);
                }
                else
                {
                    ViewBag.ddlAgreement = new SelectList(new List<SelectListItem>().AsEnumerable<SelectListItem>(), "Value", "Text");
                }
                if (designParams != null && designParams.PIU_REQ == "Y")
                {
                    ViewBag.ddlDPIU = commonFuncObj.PopulateDPIU(objMaster);
                }
                else
                {
                    ViewBag.ddlDPIU = new SelectList(new List<SelectListItem>().AsEnumerable<SelectListItem>(), "Value", "Text");
                }

                if (designParams != null && designParams.YEAR_REQ == "Y")
                {
                    ViewBag.ddlSancYear = commonFuncObj.PopulateSancYear(objMaster);
                }
                else
                {
                    ViewBag.ddlSancYear = new SelectList(new List<SelectListItem>().AsEnumerable<SelectListItem>(), "Value", "Text");
                }

                if (designParams != null && designParams.PKG_REQ == "Y")
                {
                    ViewBag.ddlPackage = commonFuncObj.PopulatePackage(objMaster);
                }
                else
                {
                    ViewBag.ddlPackage = new SelectList(new List<SelectListItem>().AsEnumerable<SelectListItem>(), "Value", "Text");
                }

                if (designParams != null && designParams.ROAD_REQ == "Y")
                {
                    ViewBag.ddlRoad = commonFuncObj.PopulateRoad(objMaster);
                }
                else
                {
                    ViewBag.ddlRoad = new SelectList(new List<SelectListItem>().AsEnumerable<SelectListItem>(), "Value", "Text");
                }
            }
            else
            {
                List<SelectListItem> lstSubTrans = new List<SelectListItem>();
                lstSubTrans.Insert(0, (new SelectListItem { Text = "Select Sub Transaction", Value = "0", Selected = true }));
                ViewBag.ddlSubTrans = lstSubTrans;
                ViewBag.ddlContractor = new SelectList(new List<SelectListItem>().AsEnumerable<SelectListItem>(), "Value", "Text");
                ViewBag.ddlAgreement = new SelectList(new List<SelectListItem>().AsEnumerable<SelectListItem>(), "Value", "Text");
                ViewBag.ddlDPIU = new SelectList(new List<SelectListItem>().AsEnumerable<SelectListItem>(), "Value", "Text");
                ViewBag.ddlPackage = new SelectList(new List<SelectListItem>().AsEnumerable<SelectListItem>(), "Value", "Text");
                ViewBag.ddlSancYear = new SelectList(new List<SelectListItem>().AsEnumerable<SelectListItem>(), "Value", "Text");
                ViewBag.ddlRoad = new SelectList(new List<SelectListItem>().AsEnumerable<SelectListItem>(), "Value", "Text");
            }

            ViewBag.IsTrans = TransId;


            return PartialView("AddOBDetails", obDetailsModel);
        }

        public ActionResult DeleteOBDetails(String parameter, String hash, String key)
        {
            objBAL = new OpeningBalanceBAL();
            string[] strParameters = URLEncrypt.DecryptParameters(new string[] { parameter, hash, key });
            Int64 billId = Convert.ToInt64(strParameters[0].Split('$')[0]);
            Int16 transNo = Convert.ToInt16(strParameters[0].Split('$')[1]);
            string status = objBAL.DeleteOBDetails(billId, transNo);
            if (status == String.Empty)
            {
                return this.Json(new { success = true, message = String.Empty });
            }
            else
            {
                return this.Json(new { success = true, message = status });
            }
        }

        [HttpPost]
        [Audit]
        [ValidateAntiForgeryToken]
        public ActionResult AddOBDetails(OBDetailsModel obDetailsModel, String parameter, String hash, String key)
        {
            commonFuncObj = new CommonFunctions();
            objBAL = new OpeningBalanceBAL();
            String ValidationSummary = String.Empty;
            String[] strParameters = URLEncrypt.DecryptParameters(new string[] { parameter, hash, key });
            if (strParameters.Length == 0)
            {
                return this.Json(new { success = false, message = "Error while Adding Receipt Details" });
            }
            else
            {
                if (strParameters[0].Contains('$'))
                {
                    obDetailsModel.TXN_NO = Convert.ToInt16(strParameters[0].Split('$')[1]);
                    obDetailsModel.BILL_ID = Convert.ToInt64(strParameters[0].Split('$')[0]);
                    ViewBag.BillId = URLEncrypt.EncryptParameters(new string[] { obDetailsModel.BILL_ID.ToString().Trim() });
                    ViewBag.IsTrans = URLEncrypt.EncryptParameters(new string[] { obDetailsModel.BILL_ID.ToString().Trim() + "$" + obDetailsModel.TXN_NO.ToString().Trim() });
                }
                else if (strParameters[0].Contains('_'))
                {
                    if (obDetailsModel.CREDIT_DEBIT.Trim() == "C")
                    {
                        obDetailsModel.BILL_ID = Convert.ToInt64(strParameters[0].Split('_')[1]);
                    }
                    else
                    {
                        obDetailsModel.BILL_ID = Convert.ToInt64(strParameters[0].Split('_')[0]);
                    }
                    ViewBag.BillId = URLEncrypt.EncryptParameters(new string[] { strParameters[0].ToString().Trim() });
                    ViewBag.IsTrans = null;
                }//else condition Added By Abhishek kamble (To Solved Fatal Error )22-Apr-2014
                else {
                    obDetailsModel.BILL_ID = Convert.ToInt64(strParameters[0]);
                    ViewBag.BillId = URLEncrypt.EncryptParameters(new string[] { strParameters[0]});
                    ViewBag.IsTrans = null;

                }
            }

            //new change done by Vikram on 19-10-2013
            //validation for road -- whether the road selected match with the sub transaction selected
            if (obDetailsModel.SUB_TXN_ID == 549 || obDetailsModel.SUB_TXN_ID == 550 || obDetailsModel.SUB_TXN_ID == 541 || obDetailsModel.SUB_TXN_ID == 542)
            {
                if (obDetailsModel.IMS_PR_ROAD_CODE != null)
                {
                    if (!ValidateRoad(obDetailsModel.SUB_TXN_ID, obDetailsModel.IMS_PR_ROAD_CODE.Value, PMGSYSession.Current.FundType))
                    {
                        return Json(new { success = false, status = "-555", message = "Select Upgrade/New road as per account head." });
                    }
                }
            }
            //end of change

            //New validation to validate PMGSY scheme Roads based on Head. start 2 Sep 2014
            if (PMGSYSession.Current.FundType == "P" && obDetailsModel.IMS_PR_ROAD_CODE != null && obDetailsModel.IMS_PR_ROAD_CODE!=0)
            {
                    if (!ValidateRoadForPMGSYScheme(obDetailsModel.SUB_TXN_ID, obDetailsModel.IMS_PR_ROAD_CODE.Value))
                    {
                        return Json(new  { success = false, message = "Road can not be Selected, Since it is in different Scheme." });
                    }  
            }
            //New validation to validate PMGSY scheme Roads based on Head. end


            ACC_BILL_MASTER acc_bill_master = new ACC_BILL_MASTER();
            acc_bill_master = objBAL.GetOBMaster(obDetailsModel.BILL_ID);
            if (acc_bill_master.BILL_NO == "1")
            {
                ViewBag.AssetLib = "A";
            }
            else
            {
                ViewBag.AssetLib = "L";
            }
            TransactionParams objMaster = new TransactionParams();
            objMaster.BILL_TYPE = "O";
            objMaster.FUND_TYPE = PMGSYSession.Current.FundType;
            objMaster.LVL_ID = PMGSYSession.Current.LevelId;
            objMaster.DISTRICT_CODE = PMGSYSession.Current.DistrictCode;
            objMaster.STATE_CODE = PMGSYSession.Current.StateCode;
            objMaster.TXN_ID = 0;
            objMaster.ADMIN_ND_CODE = PMGSYSession.Current.AdminNdCode;
            objMaster.MAST_CONT_ID = 0;
            objMaster.BILL_NO = acc_bill_master.BILL_NO;
            ViewBag.ddlTrans = commonFuncObj.PopulateOBTransaction(objMaster);
            objMaster.TXN_ID = obDetailsModel.TXN_ID;
            ViewBag.ddlSubTrans = commonFuncObj.PopulateOBTransaction(objMaster);
            objMaster.TXN_ID = obDetailsModel.SUB_TXN_ID;
            ACC_SCREEN_DESIGN_PARAM_DETAILS designParams = new ACC_SCREEN_DESIGN_PARAM_DETAILS();
            designParams = commonFuncObj.getDetailsDesignParam(objMaster);

            if (designParams != null && designParams.CON_REQ == "Y")
            {
                ViewBag.ddlContractor = commonFuncObj.PopulateContractorSupplier(objMaster);
                objMaster.MAST_CONT_ID = Convert.ToInt32(obDetailsModel.MAST_CON_ID);
            }
            else
            {
                ViewBag.ddlContractor = new SelectList(new List<SelectListItem>().AsEnumerable<SelectListItem>(), "Value", "Text");
            }
            if (designParams != null && designParams.AGREEMENT_REQ == "Y")
            {
                ViewBag.ddlAgreement = commonFuncObj.PopulateAgreement(objMaster);
            }
            else
            {
                ViewBag.ddlAgreement = new SelectList(new List<SelectListItem>().AsEnumerable<SelectListItem>(), "Value", "Text");
            }
            if (designParams != null && designParams.PIU_REQ == "Y")
            {
                ViewBag.ddlDPIU = commonFuncObj.PopulateDPIU(objMaster);
            }
            else
            {
                ViewBag.ddlDPIU = new SelectList(new List<SelectListItem>().AsEnumerable<SelectListItem>(), "Value", "Text");
            }

            if (designParams != null && designParams.YEAR_REQ == "Y")
            {
                ViewBag.ddlSancYear = commonFuncObj.PopulateSancYear(objMaster);
            }
            else
            {
                ViewBag.ddlSancYear = new SelectList(new List<SelectListItem>().AsEnumerable<SelectListItem>(), "Value", "Text");
            }

            if (designParams != null && designParams.PKG_REQ == "Y")
            {
                ViewBag.ddlPackage = commonFuncObj.PopulatePackage(objMaster);
            }
            else
            {
                //ViewBag.ddlPackage = new SelectList(new List<SelectListItem>().AsEnumerable<SelectListItem>(), "Value", "Text");
                List<SelectListItem> lstRoads = new List<SelectListItem>();
                lstRoads.Add(new SelectListItem { Value = "0", Text = "Select Road Name" });
                ViewBag.ddlPackage = lstRoads;
            }

            if (designParams != null && designParams.ROAD_REQ == "Y")
            {
                ViewBag.ddlRoad = commonFuncObj.PopulateRoad(objMaster);
            }
            else
            {
                //ViewBag.ddlRoad = new SelectList(new List<SelectListItem>().AsEnumerable<SelectListItem>(), "Value", "Text");
                List<SelectListItem> lstRoads = new List<SelectListItem>();
                lstRoads.Add(new SelectListItem { Value = "0", Text = "Select Package" });
                ViewBag.ddlRoad = lstRoads;
            }

            if (ModelState.IsValid)
            {
                ValidationSummary = objBAL.ValidateOBDetails(obDetailsModel);
                if (ValidationSummary == String.Empty)
                {
                    string success = String.Empty;
                    if (obDetailsModel.TXN_NO != 0)
                    {
                        // to check if correct entry as is operational and is required after porting flag
                        string correctionStatus = commonFuncObj.ValidateHeadForCorrection(Convert.ToInt16(obDetailsModel.SUB_TXN_ID), 0, "E");

                        if (correctionStatus != "1")
                        {
                            return this.Json(new { success = false, message = "Invalid transaction type.Delete the entry and make new entry. " });
                        }

                        success = objBAL.EditOBDetails(obDetailsModel);
                    }
                    else
                    {
                        // to check if correct entry as is operational and is required after porting flag
                        string correctionStatus = commonFuncObj.ValidateHeadForCorrection(Convert.ToInt16(obDetailsModel.SUB_TXN_ID), 0, "A");

                        if (correctionStatus != "1")
                        {
                            return this.Json(new { success = false, message = "Invalid transaction type " });
                        }
                        success = objBAL.AddOBDetails(obDetailsModel);
                    }

                    if (success == String.Empty)
                    {
                        return this.Json(new { success = true, message = URLEncrypt.EncryptParameters(new string[] { obDetailsModel.BILL_ID.ToString().Trim() }) });
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

            return View("AddOBDetails", obDetailsModel);
        }

        [Audit]
        public JsonResult PopulateSubTransaction(String id)
        {
            TransactionParams objparams = new TransactionParams();
            commonFuncObj = new CommonFunctions();
            objparams.BILL_TYPE = "O";
            objparams.FUND_TYPE = PMGSYSession.Current.FundType;
            objparams.LVL_ID = PMGSYSession.Current.LevelId;
            objparams.TXN_ID = Convert.ToInt16(id.Split('$')[0]);
            objparams.BILL_NO = id.Split('$')[1].Trim() == "A" ? "1" : "2";
            objparams.OP_MODE = id.Split('$')[2].Trim();
            return Json(commonFuncObj.PopulateOBTransaction(objparams));
        }

        [Audit]
        public JsonResult GetDetailsDesignParams(String id)
        {
            commonFuncObj = new CommonFunctions();
            TransactionParams objParam = new TransactionParams();
            objParam.TXN_ID = Convert.ToInt16(id.Trim());
            ACC_SCREEN_DESIGN_PARAM_DETAILS designParams = new ACC_SCREEN_DESIGN_PARAM_DETAILS();
            designParams = commonFuncObj.getDetailsDesignParam(objParam);
            if (designParams == null)
            {
                return Json(new { CON_REQ = "N", AGREEMENT_REQ = "N", PIU_REQ = "N", SUP_REQ = "N", ROAD_REQ = "N", SANC_YEAR = "N", PKG_REQ = "N" });
            }
            else
            {
                return Json(new { CON_REQ = designParams.CON_REQ, AGREEMENT_REQ = designParams.AGREEMENT_REQ, PIU_REQ = designParams.PIU_REQ, SUP_REQ = designParams.SUPPLIER_REQ, ROAD_REQ = designParams.ROAD_REQ, SANC_YEAR = designParams.YEAR_REQ, PKG_REQ = designParams.PKG_REQ });
            }
        }

        [Audit]
        public JsonResult PopulateDPIU()
        {
            TransactionParams objparams = new TransactionParams();
            commonFuncObj = new CommonFunctions();
            objparams.DISTRICT_CODE = PMGSYSession.Current.DistrictCode;
            objparams.STATE_CODE = PMGSYSession.Current.StateCode;
            objparams.ADMIN_ND_CODE = PMGSYSession.Current.AdminNdCode;

            return Json(commonFuncObj.PopulateDPIU(objparams));
        }

        [Audit]
        public JsonResult PopulateContractor(String id)
        {
            TransactionParams objparams = new TransactionParams();
            commonFuncObj = new CommonFunctions();
            objparams.DISTRICT_CODE = PMGSYSession.Current.DistrictCode;
            objparams.STATE_CODE = PMGSYSession.Current.StateCode;
            objparams.MAST_CON_SUP_FLAG = id.Trim();

            return Json(commonFuncObj.PopulateContractorSupplier(objparams));
        }

        [Audit]
        public JsonResult PopulateAgreement(String id)
        {
            TransactionParams objparams = new TransactionParams();
            commonFuncObj = new CommonFunctions();
            objparams.DISTRICT_CODE = PMGSYSession.Current.DistrictCode;
            objparams.STATE_CODE = PMGSYSession.Current.StateCode;
            objparams.MAST_CONT_ID = Convert.ToInt32(id.Trim());

            return Json(commonFuncObj.PopulateAgreement(objparams));
        }

        [Audit]
        public JsonResult PopulateRoad(String id)
        {
            TransactionParams objparams = new TransactionParams();
            commonFuncObj = new CommonFunctions();
            string[] param = id.Split('$'); //change
            objparams.AGREEMENT_CODE = Convert.ToInt32(param[0]);

            //new change done by Vikram as per the road should be populated according to the fund type and the associated table

            if (!String.IsNullOrEmpty(param[2]) && param[2] != "undefined")
            {
                objparams.MAST_CONT_ID = Convert.ToInt32(param[2]);
            }

            if (!String.IsNullOrEmpty(Request.Params["AGREEMENT_NUMBER"]))
            {
                objparams.AGREEMENT_NUMBER = Request.Params["AGREEMENT_NUMBER"];
            }

            //end of change


            return Json(commonFuncObj.PopulateRoad(objparams));
        }

        [Audit]
        public JsonResult FinalizeOB(String parameter, String hash, String key)
        {
            objBAL = new OpeningBalanceBAL();
            commonFuncObj = new CommonFunctions();
            string[] strParameters = URLEncrypt.DecryptParameters(new string[] { parameter, hash, key });
            if (strParameters != null)
            {
                //sString OBMasterIds = objBAL.GetOBMasterIds(Convert.ToInt64(strParameters[0].Trim()));

                // to check if correct entry as is operational and is required after porting flag
                string correctionStatusAsset = commonFuncObj.ValidateHeadForCorrection(0, Convert.ToInt64(strParameters[0].Split('_')[0]), "F");

                if (correctionStatusAsset != "1")
                {
                    return this.Json(new { success = false, message = "Cant finalize the entry as Asset details contains invalid transaction type" });
                }

                // to check if correct entry as is operational and is required after porting flag
                string correctionStatusLia = commonFuncObj.ValidateHeadForCorrection(0, Convert.ToInt64(strParameters[0].Split('_')[1]), "F");

                if (correctionStatusLia != "1")
                {
                    return this.Json(new { success = false, message = "Cant finalize the entry as Liability details contains invalid transaction type" });
                }

                string status = objBAL.FinalizeOB(Convert.ToInt64(strParameters[0].Split('_')[0]), Convert.ToInt64(strParameters[0].Split('_')[1]));
                if (status == String.Empty)
                {
                    return this.Json(new { success = true, message = URLEncrypt.EncryptParameters(new string[] { strParameters[0] }) });
                }
                else
                {
                    return this.Json(new { success = false, message = status });
                }
            }
            else
            {
                return this.Json(new { success = false, message = "Error while Processing your request" });
            }
        }

        [Audit]
        public JsonResult GetAssetLiabilityDetails()
        {
            objBAL = new OpeningBalanceBAL();
            TransactionParams objFilter = new TransactionParams();
            objFilter.ADMIN_ND_CODE = PMGSYSession.Current.AdminNdCode;
            objFilter.FUND_TYPE = PMGSYSession.Current.FundType;
            objFilter.LVL_ID = PMGSYSession.Current.LevelId;
            objFilter.BILL_TYPE = "O";
            JsonCollection jsonCol = objBAL.GetAssetLiabilityDetails(objFilter);
            return Json(new { success = true, asset = jsonCol.assetList, liability = jsonCol.libList });
        }

        [HttpPost]
        [Audit]
        public String GetNarration(String parameter, String hash, String key)
        {
            commonFuncObj = new CommonFunctions();
            String TransID = Request.Params["transId"].ToString();
            String[] strParameters = URLEncrypt.DecryptParameters(new string[] { parameter, hash, key });
            return commonFuncObj.GetTransNarration(Convert.ToInt16(TransID), strParameters[0], "O");
        }

        //new method added by Vikram 
        public bool ValidateRoad(short headId, int proposalCode, string fundType)
        {
            string upgradeConnectFlag = string.Empty;
            commonFuncObj = new CommonFunctions();
            try
            {
                if (fundType == "P")
                {
                    switch (headId)
                    {
                        case 549:
                            upgradeConnectFlag = "N";
                            break;
                        case 550:
                            upgradeConnectFlag = "U";
                            break;
                        case 541:
                            upgradeConnectFlag = "N";
                            break;
                        case 542:
                            upgradeConnectFlag = "U";
                            break;
                        default:
                            break;
                    }

                    bool status = commonFuncObj.ValidateRoad(proposalCode, upgradeConnectFlag);
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
                commonFuncObj = new CommonFunctions();
                bool status = commonFuncObj.ValidateRoadForPMGSYScheme(txnID, RoadCode);
                return status;
                
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return false;
            }
        }

        /// <summary>
        /// returns the success result whether the final payment or not
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="hash"></param>
        /// <param name="key"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public JsonResult IsFinalPayment(String parameter, String hash, String key,String id)
        {
            bool status = false;
            objBAL = new OpeningBalanceBAL();
            try
            {
                if (!String.IsNullOrEmpty(parameter) && !String.IsNullOrEmpty(hash) && !String.IsNullOrEmpty(key))
                {
                    String[] decryptedParameters = URLEncrypt.DecryptParameters(new String[] { parameter,hash,key});
                    if (decryptedParameters[0].Contains('_'))
                    {
                        decryptedParameters = decryptedParameters[0].Split('_');
                    }
                    int billId = Convert.ToInt32(decryptedParameters[0]);
                    int imsRoadCode = Convert.ToInt32(id);
                    status = objBAL.IsFinalPayment(billId,imsRoadCode);
                }
                return Json(new { success = status});
            }
            catch (Exception)
            {
                return Json(new { success = false});
            }
        }

    }
}
