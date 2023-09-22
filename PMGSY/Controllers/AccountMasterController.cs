using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PMGSY.BAL.AccountMaster;
using PMGSY.Models.AccountMaster;
using PMGSY.Common;
using PMGSY.Models;

namespace PMGSY.Controllers
{
    [RequiredAuthentication]
    [RequiredAuthorization]
    public class AccountMasterController : Controller
    {
        IAccountMasterBAL objBAL = new AccountMasterBAL();

        //
        // GET: /AccountMaster/

        //public ActionResult Index()
        //{
        //    return View();
        //}

        #region Master Head

        public ActionResult MasterHeadView()
        {
            return View();
        }

        public ActionResult AddEditMasterHeadDetails(String id)
        {
            MasterHeadViewModel masterHeadViewModel = new MasterHeadViewModel();
            if (PMGSY.Extensions.PMGSYSession.Current.FundType != null)
            {
                masterHeadViewModel.FUND_TYPE = PMGSY.Extensions.PMGSYSession.Current.FundType;
            }

            if ( !String.IsNullOrEmpty(id) &&  id == "P")
            {
                masterHeadViewModel.IsParentHead = true;
                masterHeadViewModel.ParentSubHead = "P";                
            }
            else
            {
                masterHeadViewModel.IsParentHead = false;
                masterHeadViewModel.ParentSubHead = "S";
            }


            return View(masterHeadViewModel);
        }

        [HttpPost]
        public ActionResult AddMasterHeadDetails(MasterHeadViewModel masterHeadViewModel)
        {
            String message = String.Empty;
            try
            {

                if (masterHeadViewModel.ParentSubHead == "P")
                {
                    if (ModelState.ContainsKey("PARENT_HEAD_ID"))
                        ModelState["PARENT_HEAD_ID"].Errors.Clear();

                    if (ModelState.ContainsKey("CREDIT_DEBIT"))
                        ModelState["CREDIT_DEBIT"].Errors.Clear(); 
                }

                if (ModelState.IsValid)
                {
                    if ((objBAL.AddMasterHeadDetails(masterHeadViewModel, ref message)))
                    {
                        return Json(new { success = true, message = message });
                    }
                    else
                    {
                        return Json(new { success = false, message = (message == string.Empty ? "Head Details not added." : message) });
                    }
                }
                else
                {
                    if (PMGSY.Extensions.PMGSYSession.Current.FundType != null)
                    {
                        masterHeadViewModel.FUND_TYPE = PMGSY.Extensions.PMGSYSession.Current.FundType;
                    }

                    if (masterHeadViewModel.ParentSubHead=="P")
                    {
                        masterHeadViewModel.IsParentHead = true;
                        masterHeadViewModel.ParentSubHead = "P";
                    }
                    else
                    {
                        masterHeadViewModel.IsParentHead = false;
                        masterHeadViewModel.ParentSubHead = "S";
                    }

                    return PartialView("AddEditMasterHeadDetails", masterHeadViewModel);
                }
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return Json(new { success = false, message = "Error occurred while processing your request." });
            }

        }

        [HttpPost]
        public ActionResult EditMasterHeadDetails(MasterHeadViewModel masterHeadViewModel)
        {
            String message = String.Empty;
            try
            {

                if (masterHeadViewModel.ParentSubHead == "P")
                {
                    if (ModelState.ContainsKey("PARENT_HEAD_ID"))
                        ModelState["PARENT_HEAD_ID"].Errors.Clear();

                    if (ModelState.ContainsKey("CREDIT_DEBIT"))
                        ModelState["CREDIT_DEBIT"].Errors.Clear();
                }
                if (ModelState.IsValid)
                {
                    if ((objBAL.EditMasterHeadDetails(masterHeadViewModel, ref message)))
                    {
                        return Json(new { success = true, message = message });
                    }
                    else
                    {
                        return Json(new { success = false, message = (message == string.Empty ? "Head Details not updated." : message) });
                    }
                }
                else
                {
                    if (PMGSY.Extensions.PMGSYSession.Current.FundType != null)
                    {
                        masterHeadViewModel.FUND_TYPE = PMGSY.Extensions.PMGSYSession.Current.FundType;
                    }
                    if (masterHeadViewModel.ParentSubHead == "P")
                    {
                        masterHeadViewModel.IsParentHead = true;
                        masterHeadViewModel.ParentSubHead = "P";
                    }
                    else
                    {
                        masterHeadViewModel.IsParentHead = false;
                        masterHeadViewModel.ParentSubHead = "S";
                    }
                    return PartialView("AddEditMasterHeadDetails", masterHeadViewModel);
                }
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return Json(new { success = false, message = "Error occurred while processing your request." });
            }

        }

        [HttpPost]
        public ActionResult DeleteMasterHeadDetails(String Parameter, String Hash, String Key)
        {
            String message = String.Empty;
            try
            {
                short HeadId = 0;

                if (!String.IsNullOrEmpty(Parameter) && !String.IsNullOrEmpty(Hash) && !String.IsNullOrEmpty(Key))
                {
                    Dictionary<string, string> decryptedParameters = null;

                    decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { Parameter, Hash, Key });

                    if (decryptedParameters.Count > 0)
                    {
                        HeadId = Convert.ToInt16(decryptedParameters["HEAD_ID"]);
                    }

                    if (objBAL.DeleteMasterHeadDetails(HeadId, ref message))
                    {
                        return Json(new { success = true, message = message });
                    }
                    else
                    {
                        return Json(new { success = false, message = message });
                    }

                }
                else
                {
                    return Json(new { success = false, message = "An Error occured while processing your request." });
                }
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return Json(new { success = false, message = "Error occurred while processing your request." });
            }
        }

        public ActionResult GetMasterHeadDetails(String Parameter, String Hash, String Key)
        {
            String message = String.Empty;
            try
            {
                short HeadId = 0;

                if (!String.IsNullOrEmpty(Parameter) && !String.IsNullOrEmpty(Hash) && !String.IsNullOrEmpty(Key))
                {
                    Dictionary<string, string> decryptedParameters = null;

                    decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { Parameter, Hash, Key });

                    if (decryptedParameters.Count > 0)
                    {
                        HeadId = Convert.ToInt16(decryptedParameters["HEAD_ID"]);
                    }
                    return PartialView("AddEditMasterHeadDetails", objBAL.GetHeadDetails(HeadId));
                }
                else
                {
                    return PartialView("AddEditMasterHeadDetails", new MasterHeadViewModel());
                }
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return PartialView("AddEditMasterHeadDetails", new MasterHeadViewModel());
            }
        }

        /// <summary>
        /// List MasterHead Details
        /// </summary>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <param name="sidx"></param>
        /// <param name="sord"></param>
        /// <returns></returns>
        public ActionResult MasterHeadList(Int32? page, Int32? rows, String sidx, String sord)
        {
            using (CommonFunctions commonFunction = new CommonFunctions())
            {
                if (!commonFunction.ValidateGridParameters(new GridParams(page, rows, sidx, sord, Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                {
                    return null;
                }
            }

            String FundType = PMGSY.Extensions.PMGSYSession.Current.FundType == null ? Request.Params["Fund_Type"] : PMGSY.Extensions.PMGSYSession.Current.FundType;

            long totalRecords = 0;
            var jsonData = new
            {
                rows = objBAL.ListMasterHeadDetails(page, rows, sidx, sord, out totalRecords, FundType),
                total = 0,
                page = page + 1,
                records = totalRecords
            };
            return Json(jsonData);
        }


        [HttpPost]
        public JsonResult GetParentHeadCode(string id)
        {
            return Json(new { HeadCode = objBAL.GetParentHeadCode(Convert.ToInt16(id)) });
        }

        #endregion Master Head


        #region Master Transaction

        public ActionResult MasterTransactionView()
        {
            return View();
        }

        public ActionResult AddEditMasterTransactionDetails(String id)
        {
            MasterTransactionViewModel masterTransactionViewModel = new MasterTransactionViewModel();
            if (PMGSY.Extensions.PMGSYSession.Current.FundType != null)
            {
                masterTransactionViewModel.FUND_TYPE = PMGSY.Extensions.PMGSYSession.Current.FundType;
            }

            if (!String.IsNullOrEmpty(id) && id == "P")
            {
                masterTransactionViewModel.IsParentTransaction= true;
                masterTransactionViewModel.ParentSubTransaction = "P";
            }
            else
            {
                masterTransactionViewModel.IsParentTransaction = false;
                masterTransactionViewModel.ParentSubTransaction = "S";
            }


            return View(masterTransactionViewModel);
        }

        [HttpPost]
        public ActionResult AddMasterTransactionDetails(MasterTransactionViewModel masterTransactionViewModel)
        {
            String message = String.Empty;
            try
            {

                if (masterTransactionViewModel.ParentSubTransaction == "P")
                {
                    if (ModelState.ContainsKey("TXN_PARENT_ID"))
                        ModelState["TXN_PARENT_ID"].Errors.Clear();

                    if (ModelState.ContainsKey("TXN_NARRATION"))
                        ModelState["TXN_NARRATION"].Errors.Clear();
                }

                if (ModelState.IsValid)
                {
                    if ((objBAL.AddMasterTransactionDetails(masterTransactionViewModel, ref message)))
                    {
                        return Json(new { success = true, message = message });
                    }
                    else
                    {
                        return Json(new { success = false, message = (message == string.Empty ? "Transaction Details not added." : message) });
                    }
                }
                else
                {
                    if (PMGSY.Extensions.PMGSYSession.Current.FundType != null)
                    {
                        masterTransactionViewModel.FUND_TYPE = PMGSY.Extensions.PMGSYSession.Current.FundType;
                    }

                    if (masterTransactionViewModel.ParentSubTransaction == "P")
                    {
                        masterTransactionViewModel.IsParentTransaction = true;
                        masterTransactionViewModel.ParentSubTransaction = "P";
                    }
                    else
                    {
                        masterTransactionViewModel.IsParentTransaction = false;
                        masterTransactionViewModel.ParentSubTransaction = "S";
                    }

                    return PartialView("AddEditMasterTransactionDetails", masterTransactionViewModel);
                }
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return Json(new { success = false, message = "Error occurred while processing your request." });
            }

        }

        [HttpPost]
        public ActionResult EditMasterTransactionDetails(MasterTransactionViewModel masterTransactionViewModel)
        {
            String message = String.Empty;
            try
            {        
                if (masterTransactionViewModel.ParentSubTransaction == "P")
                {
                    if (ModelState.ContainsKey("TXN_PARENT_ID"))
                        ModelState["TXN_PARENT_ID"].Errors.Clear();

                    if (ModelState.ContainsKey("TXN_NARRATION"))
                        ModelState["TXN_NARRATION"].Errors.Clear();
                }

                if (ModelState.IsValid)
                {
                    if ((objBAL.EditMasterTransactionDetails(masterTransactionViewModel, ref message)))
                    {
                        return Json(new { success = true, message = message });
                    }
                    else
                    {
                        return Json(new { success = false, message = (message == string.Empty ? "Transaction Details not updated." : message) });
                    }
                }
                else
                {
                    if (PMGSY.Extensions.PMGSYSession.Current.FundType != null)
                    {
                        masterTransactionViewModel.FUND_TYPE = PMGSY.Extensions.PMGSYSession.Current.FundType;
                    }
                    if (masterTransactionViewModel.ParentSubTransaction == "P")
                    {
                        masterTransactionViewModel.IsParentTransaction = true;
                        masterTransactionViewModel.ParentSubTransaction = "P";
                    }
                    else
                    {
                        masterTransactionViewModel.IsParentTransaction = false;
                        masterTransactionViewModel.ParentSubTransaction = "S";
                    }
                    return PartialView("AddEditMasterTransactionDetails", masterTransactionViewModel);
                }
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return Json(new { success = false, message = "Error occurred while processing your request." });
            }

        }

        [HttpPost]
        public ActionResult DeleteMasterTransactionDetails(String Parameter, String Hash, String Key)
        {
            String message = String.Empty;
            try
            {
                short TxnId = 0;

                if (!String.IsNullOrEmpty(Parameter) && !String.IsNullOrEmpty(Hash) && !String.IsNullOrEmpty(Key))
                {
                    Dictionary<string, string> decryptedParameters = null;

                    decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { Parameter, Hash, Key });

                    if (decryptedParameters.Count > 0)
                    {
                        TxnId = Convert.ToInt16(decryptedParameters["TXN_ID"]);
                    }

                    if (objBAL.DeleteMasterTransactionDetails(TxnId, ref message))
                    {
                        return Json(new { success = true, message = message });
                    }
                    else
                    {
                        return Json(new { success = false, message = message });
                    }

                }
                else
                {
                    return Json(new { success = false, message = "An Error occured while processing your request." });
                }
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return Json(new { success = false, message = "Error occurred while processing your request." });
            }
        }

        public ActionResult GetMasterTransactionDetails(String Parameter, String Hash, String Key)
        {
            String message = String.Empty;
            try
            {
                short TxnId = 0;

                if (!String.IsNullOrEmpty(Parameter) && !String.IsNullOrEmpty(Hash) && !String.IsNullOrEmpty(Key))
                {
                    Dictionary<string, string> decryptedParameters = null;

                    decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { Parameter, Hash, Key });

                    if (decryptedParameters.Count > 0)
                    {
                        TxnId = Convert.ToInt16(decryptedParameters["TXN_ID"]);
                    }
                    return PartialView("AddEditMasterTransactionDetails", objBAL.GetTransactionDetails(TxnId));
                }
                else
                {
                    return PartialView("AddEditMasterTransactionDetails", new MasterTransactionViewModel());
                }
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return PartialView("AddEditMasterTransactionDetails", new MasterTransactionViewModel());
            }
        }

   
        public ActionResult MasterTransactionList(Int32? page, Int32? rows, String sidx, String sord)
        {
            using (CommonFunctions commonFunction = new CommonFunctions())
            {
                if (!commonFunction.ValidateGridParameters(new GridParams(page, rows, sidx, sord, Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                {
                    return null;
                }
            }

            bool IsSearch = false; 
            
            short ParentTxn = 0;
            int Level = 0;
            string CashCheque = String.Empty;
            string BillType = String.Empty;
            bool? IsOperational = null; 

            if (!(String.IsNullOrEmpty(Request.Params["IsSearch"])))
            {
                IsSearch = Convert.ToBoolean(Request.Params["IsSearch"]);
            }
            if (!(String.IsNullOrEmpty(Request.Params["ParentTxn"])))
            {
                ParentTxn = Convert.ToInt16(Request.Params["ParentTxn"]);
            }
            if (!(String.IsNullOrEmpty(Request.Params["Level"])))
            {
                Level = Convert.ToInt32(Request.Params["Level"]);
            }
            if (!(String.IsNullOrEmpty(Request.Params["CashCheque"])))
            {
                CashCheque = Request.Params["CashCheque"];
            }
            if (!(String.IsNullOrEmpty(Request.Params["BillType"])))
            {
                BillType = Request.Params["BillType"]; 
            }
            if (!(String.IsNullOrEmpty(Request.Params["IsOperational"])))
            {
                IsOperational = Convert.ToBoolean(Request.Params["IsOperational"]);
            }


            String FundType = PMGSY.Extensions.PMGSYSession.Current.FundType == null ? Request.Params["Fund_Type"] : PMGSY.Extensions.PMGSYSession.Current.FundType;

            long totalRecords = 0;
            var jsonData = new
            {
                rows = objBAL.ListMasterTransactionDetails(page, rows, sidx, sord, out totalRecords, FundType, IsSearch, ParentTxn, Level, CashCheque, BillType, IsOperational),
                total = 0,
                page = page + 1,
                records = totalRecords
            };
            return Json(jsonData);
        }

        [HttpPost]
        public JsonResult GetParentTransactionDetails(string id)
        {
            ACC_MASTER_TXN txnMaster = objBAL.GetParentTransactionDetails(Convert.ToInt16(id));
            int Level=0;
            String CashCheque=String.Empty;
            String BillType=String.Empty;
            if (txnMaster != null)
            {
                Level = txnMaster.OP_LVL_ID;
                CashCheque = txnMaster.CASH_CHQ;
                BillType = txnMaster.BILL_TYPE;
            }
            return Json(new { Level = Level,CashCheque=CashCheque,BillType=BillType });
        }
         
        [HttpGet]
        public ActionResult SearchTransactionDetailsView()
        {
            MasterTransactionViewModel masterTransactionModel = new MasterTransactionViewModel();
            if (PMGSY.Extensions.PMGSYSession.Current.FundType != null)
            {
                masterTransactionModel.FUND_TYPE = PMGSY.Extensions.PMGSYSession.Current.FundType;
            }
            return View(masterTransactionModel);
        }

        #endregion Master Transaction

    }
}
