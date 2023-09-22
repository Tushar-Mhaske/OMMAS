using PMGSY.BAL.AssetDetails;
using PMGSY.Common;
using PMGSY.DAL;
using PMGSY.Extensions;
using PMGSY.Models;
using PMGSY.Models.AssetDetails;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PMGSY.Controllers
{
    [RequiredAuthentication]
    [RequiredAuthorization]
    public class AssetsController : Controller
    {

        public AssetsController()
        {
            PMGSYSession.Current.ModuleName = "Asset Entry";
        }

        /// <summary>
        /// returns the view with the asset amount and other details list
        /// </summary>
        /// <returns></returns>
       [Audit]
        public ActionResult ListAssetDetails()
        {
            CommonFunctions objCommon = new CommonFunctions();
            AssetFilterViewModel assetModel = new AssetFilterViewModel();
            assetModel.ddlMonth = objCommon.PopulateMonths(DateTime.Now.Month);
            assetModel.ddlYear = objCommon.PopulateYears(DateTime.Now.Year).OrderByDescending(m => m.Value).ToList<SelectListItem>();
            assetModel.Month = Convert.ToInt16(DateTime.Now.Month);
            assetModel.Year = Convert.ToInt16(DateTime.Now.Year);
            return View("ListAssetDetails", assetModel);
        }

        /// <summary>
        /// returns the list of asset payments
        /// </summary>
        /// <param name="page">no. of pages</param>
        /// <param name="rows">no. of rows</param>
        /// <param name="sidx">sort index</param>
        /// <param name="sord">sort order</param>
        /// <returns></returns>
        [Audit]
        public JsonResult GetAssetPaymentDetails(int? page, int? rows, string sidx, string sord)
        {
            int BillId = Convert.ToInt32(Request["BillId"]);
            long totalRecords;
            short monthCode = Convert.ToInt16(Request["Month"]);
            short yearCode = Convert.ToInt16(Request["Year"]);
            string chequeNo = Request["ChequeNo"];
            string billNo = Request["BillNo"];

            IAssetDetailsBAL objAssetBAL = new AssetDetailsBAL();
            using (CommonFunctions commonFunction = new CommonFunctions())
            {
                if (!commonFunction.ValidateGridParameters(new GridParams(page, rows, sidx, sord, Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                {
                    return null;
                }
            }

            var jsonData = new
            {
                rows = objAssetBAL.GetAssetDetails(Convert.ToInt32(page) - 1, Convert.ToInt32(rows), sidx, sord, out totalRecords, monthCode, yearCode, chequeNo, billNo, PMGSYSession.Current.AdminNdCode, PMGSYSession.Current.FundType),
                total = totalRecords <= Convert.ToInt32(rows) ? 1 : totalRecords / Convert.ToInt32(rows) + 1,
                page = Convert.ToInt32(page),
                records = totalRecords
            };
            return Json(jsonData);
        }

        /// <summary>
        /// returns the partial view for saving the asset details
        /// </summary>
        /// <param name="hash">encrypted id</param>
        /// <param name="parameter">encrypted id</param>
        /// <param name="key">encrypted id</param>
        /// <returns></returns>
        [Audit]
        public ActionResult AddEditAssetDetails(string urlparameter)
        {
            String[] encryptedParameters = null;
            Dictionary<string, string> decryptedParameters = null;
            IAssetDetailsDAL objDAL = new AssetDetailsDAL();
            try
            {
                encryptedParameters = urlparameter.Split('/');
                decryptedParameters = URLEncrypt.DecryptParameters1(new string[] { encryptedParameters[0], encryptedParameters[1], encryptedParameters[2] });
                AssetDetailsViewModel assetModel = new AssetDetailsViewModel();
                assetModel.Bill_Id = Convert.ToInt64(decryptedParameters["BillId"]);
                assetModel.Head_Id = Convert.ToInt16(decryptedParameters["HeadId"]);
                assetModel.TotalAssetAmount = Convert.ToDecimal(decryptedParameters["AssetAmount"]);
                assetModel.HeadDesc = decryptedParameters["HeadDesc"];
                assetModel.VoucherDate = decryptedParameters["VoucherDate"];
                assetModel.VoucherNo = decryptedParameters["VoucherNo"];
                assetModel.Urlparameter = urlparameter;
                assetModel.TotalRemainingAmount = objDAL.GetRemainingAmount(assetModel.TotalAssetAmount, assetModel.Head_Id, assetModel.Bill_Id);
                assetModel.ddlAssets = objDAL.GetAssets();
                assetModel.Operation = "A";
                return PartialView("AddEditAssetDetails", assetModel);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);

                return PartialView("AddEditAssetDetails");
            }
        }

        /// <summary>
        /// returns the list of Asset Details for particular Head and Bill
        /// </summary>
        /// <param name="page">no. of pages</param>
        /// <param name="rows">no. of rows</param>
        /// <param name="sidx">sort column name</param>
        /// <param name="sord">sort order</param>
        /// <returns></returns>
        [Audit]
        public JsonResult GetAssetDetailsList(int? page, int? rows, string sidx, string sord)
        {
            int BillId = Convert.ToInt32(Request["BillId"]);
            int HeadId = Convert.ToInt32(Request["HeadId"]);
            long totalRecords;
            bool isFinalize = false;
            IAssetDetailsBAL objAssetBAL = new AssetDetailsBAL();
            using (CommonFunctions commonFunction = new CommonFunctions())
            {
                if (!commonFunction.ValidateGridParameters(new GridParams(page, rows, sidx, sord, Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                {
                    return null;
                }
            }

            var jsonData = new
            {
                rows = objAssetBAL.GetAssetDetailsList(Convert.ToInt32(page) - 1, Convert.ToInt32(rows), sidx, sord, out totalRecords, BillId, HeadId,ref isFinalize),
                total = totalRecords <= Convert.ToInt32(rows) ? 1 : totalRecords / Convert.ToInt32(rows) + 1,
                page = Convert.ToInt32(page),
                records = totalRecords,
                IsFinalize = isFinalize
            };
            return Json(jsonData);
        }

        /// <summary>
        /// saves the Asset details
        /// </summary>
        /// <param name="model">contains the Asset Details</param>
        /// <returns></returns>
        [HttpPost]
        [Audit]
        public ActionResult AddAssetDetails(AssetDetailsViewModel model)
        {
            IAssetDetailsBAL objBAL = new AssetDetailsBAL();
            string message = string.Empty;
            try
            {
                if (ModelState.IsValid)
                {
                    if (!objBAL.AddAssetDetails(model, ref message))
                    {
                        return Json(new { success = false, message = message });
                    }
                    else
                    {
                        return Json(new { success = true, message = message });
                    }
                }
                else
                {
                    return Json(new { success = false, message = new CommonFunctions().FormatErrorMessage(ModelState) });
                }
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return Json(new { success = false, message = "Error occurred while processing your request." });
            }
        }

        /// <summary>
        /// updates the Asset details
        /// </summary>
        /// <param name="model">contains the Asset Details</param>
        /// <returns></returns>
        [HttpPost]
        [Audit]
        public ActionResult EditAssetDetails(AssetDetailsViewModel model)
        {
            IAssetDetailsBAL objBAL = new AssetDetailsBAL();
            string message = string.Empty;
            try
            {
                if (ModelState.IsValid)
                {
                    if (!objBAL.EditAssetDetails(model, ref message))
                    {
                        return Json(new { success = false, message = message });
                    }
                    else
                    {
                        return Json(new { success = true, message = message });
                    }
                }
                else
                {
                    return Json(new { success = false, message = new CommonFunctions().FormatErrorMessage(ModelState) });
                }
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);

                return Json(new { success = false, message = "Error occurred while processing your request." });
            }
        }

        /// <summary>
        /// gets the details of particular asset
        /// </summary>
        /// <param name="urlparameter">encrypted id of asset</param>
        /// <returns></returns>
        [Audit]
        public ActionResult GetAssetInformation(string id)
        {
            Dictionary<string, string> decryptedParameters = null;
            IAssetDetailsBAL objBAL = new AssetDetailsBAL();
            IAssetDetailsDAL objDAL = new AssetDetailsDAL();
            String[] encryptedParameters = null;
            PMGSY.Models.PMGSYEntities db = new Models.PMGSYEntities();
            CommonFunctions objCommon = new CommonFunctions();
            try
            {
                encryptedParameters = id.Split('/');
                string[] param = encryptedParameters[2].Split('$');
                decryptedParameters = URLEncrypt.DecryptParameters1(new string[] { encryptedParameters[0], encryptedParameters[1], param[0] });
                AssetDetailsViewModel assetModel = new AssetDetailsViewModel();
                assetModel = objBAL.GetAssetInformation(Convert.ToInt32(decryptedParameters["DetailsId"]));
                assetModel.Bill_Id = Convert.ToInt64(decryptedParameters["BillId"]);
                assetModel.Head_Id = Convert.ToInt16(decryptedParameters["HeadId"]);
                ACC_BILL_MASTER billMaster = db.ACC_BILL_MASTER.Find(assetModel.Bill_Id);
                assetModel.VoucherNo = billMaster.BILL_NO;
                assetModel.VoucherDate = objCommon.GetDateTimeToString(billMaster.BILL_DATE);
                assetModel.HeadDesc = db.ACC_MASTER_HEAD.Where(m => m.HEAD_ID == assetModel.Head_Id).Select(m => m.HEAD_NAME).FirstOrDefault();
                if (decryptedParameters.Count() > 3)
                {
                    assetModel.HeadDesc = decryptedParameters["HeadDesc"];
                    assetModel.VoucherDate = decryptedParameters["VoucherDate"];
                    assetModel.VoucherNo = decryptedParameters["VoucherNo"];
                }
                assetModel.TotalAssetAmount = Convert.ToDecimal(param[1]);
                assetModel.TotalRemainingAmount = objDAL.GetRemainingAmount(assetModel.TotalAssetAmount, assetModel.Head_Id, assetModel.Bill_Id) + assetModel.Total_Amount;
                assetModel.ddlAssets = objDAL.GetAssets();
                assetModel.Operation = "E";
                //model.TotalRemainingAmount = objDAL.GetAvailableAmount();
                return PartialView("AddEditAssetDetails", assetModel);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return Json(new { success = false, message = "Error occurred while processing your request." });
            }
        }

        /// <summary>
        /// deletes the asset with particular details id
        /// </summary>
        /// <param name="urlparameter">encrypted details id</param>
        /// <returns></returns>
        [HttpPost]
        [Audit]
        public ActionResult DeleteAssetDetails(String parameter, String hash, String key)
        {
            Dictionary<string, string> decryptedParameters = null;
            string message = string.Empty;
            IAssetDetailsBAL objBAL = new AssetDetailsBAL();
            try
            {
                decryptedParameters = URLEncrypt.DecryptParameters1(new string[] { parameter, hash, key });
                if (objBAL.DeleteAssetDetails(Convert.ToInt32(decryptedParameters["DetailsId"]), ref message))
                {
                    return Json(new { success = true, message = message });
                }
                else
                {
                    return Json(new { success = false, message = message });
                }
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return Json(new { success = false, message = message });
            }
        }

        /// <summary>
        /// validates the total asset amount with the entered asset details amount
        /// </summary>
        /// <param name="model">contains the total asset and asset details amount </param>
        /// <returns></returns>
        [HttpPost]
        [Audit]
        public ActionResult CheckTotalAmount(AssetFilterViewModel model)
        {
            IAssetDetailsBAL objBAL = new AssetDetailsBAL();
            try
            {
                decimal? totalAmount = Convert.ToDecimal(Request.Params["totalAmount"]);
                bool status = false;//objBAL.CheckTotalAmount(model,totalAmount);
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

        /// <summary>
        /// deletes the entry from the asset payment details list
        /// </summary>
        /// <param name="parameter">encrypted id</param>
        /// <param name="hash">encrypted id</param>
        /// <param name="key">encrypted id</param>
        /// <returns></returns>
        [Audit]
        public ActionResult DeleteAssetPaymentDetails(String parameter, String hash, String key)
        {
            IAssetDetailsBAL objBAL = new AssetDetailsBAL();
            Dictionary<string, string> decryptedParameters = null;
            string message = string.Empty;
            try
            {
                decryptedParameters = URLEncrypt.DecryptParameters1(new string[] { parameter, hash, key });
                short headId = Convert.ToInt16(decryptedParameters["HeadId"]);
                long billId = Convert.ToInt64(decryptedParameters["BillId"]);
                bool status = objBAL.DeleteAssetPaymentDetails(headId, billId, ref message);
                if (status == true)
                {
                    return Json(new { success = true, message = message });
                }
                else
                {
                    return Json(new { success = false, message = message });
                }
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return Json(new { success = false, message = message });
            }
        }

        public ActionResult FinalizeAssetDetails(String id)
        {
            String message = string.Empty;
            try
            {
                if (!String.IsNullOrEmpty(id))
                {
                    IAssetDetailsBAL objBAL = new AssetDetailsBAL();
                    bool status = objBAL.FinalizeAssetDetails(id, ref message);
                    if (status == true)
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
                    return Json(new { success = false, message = message });
                }
                
            }
            catch (Exception)
            {
                return Json(new { success =false ,message = "Error occurred while finalizing the details."});
            }
        }

        public ActionResult DefinalizeAssetDetails(String parameter, String hash, String key)
        {
            Dictionary<string, string> decryptedParameters = null;
            string message = string.Empty;
            IAssetDetailsBAL objBAL = new AssetDetailsBAL();
            long billId = 0;
            short headId = 0;
            try
            {
                decryptedParameters = URLEncrypt.DecryptParameters1(new string[] { parameter, hash, key });
                billId = Convert.ToInt64(decryptedParameters["BillId"]);
                headId = Convert.ToInt16(decryptedParameters["HeadId"]);
                if (objBAL.DefinalizeAssetDetails(billId,headId, ref message))
                {
                    return Json(new { success = true, message = message });
                }
                else
                {
                    return Json(new { success = false, message = message });
                }

            }
            catch
            {
                return Json(new { success = false, message = "Error occurred while processing your request." });
            }
        }

    }
}
