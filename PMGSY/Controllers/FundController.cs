#region HEADER
/*
* Project Id:

 * Project Name:OMMAS-II

 * File Name: FundController.cs

 * Author :  Vikram Nandanwar

 * Creation Date :05/June/2013

 * Desc : This class is used as controller  to perform Save,Edit,Update,Delete and listing of Fund Allocation and Release screens.  
 */
#endregion

using PMGSY.BAL.Fund;
using PMGSY.Common;
using PMGSY.DAL.FundDAL;
using PMGSY.Extensions;
using PMGSY.Models;
using PMGSY.Models.Fund;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace PMGSY.Controllers
{
    [RequiredAuthentication]
    [RequiredAuthorization]
    public class FundController : Controller
    {

        public FundController()
        {
            PMGSYSession.Current.ModuleName = "Fund Allocation and Release";
        }

        IFundBAL objBAL = new FundBAL();
        FundDAL objDAL = new FundDAL();
        PMGSYEntities db = new PMGSYEntities();
        string message = string.Empty;

        #region FUND_ALLOCATION

        [Audit]
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// List View of Fund Allocation
        /// </summary>
        /// <returns></returns>
        [Audit]
        public ActionResult ListFundAllocationDetails()
        {

            if (PMGSYSession.Current.RoleCode == 2)
            {
                FundAllocationFilterViewModel model = new FundAllocationFilterViewModel();
                model.State = PMGSYSession.Current.StateCode;
                ViewData["State"] = new SelectList(objDAL.GetAllStates(), "Value", "Text", model.State);
                ViewData["FundType"] = new SelectList(objDAL.GetFundType(), "Value", "Text");
                ViewData["Year"] = new SelectList(objDAL.GetAllYear(), "Value", "Text");
                ViewData["FundingAgency"] = new SelectList(objDAL.GetFundingAgency(), "Value", "Text");
                return View(model);
            }
            else
            {
                ViewData["State"] = new SelectList(objDAL.GetAllStates(), "Value", "Text");
                ViewData["FundType"] = new SelectList(objDAL.GetFundType(), "Value", "Text");
                ViewData["Year"] = new SelectList(objDAL.GetAllYear(), "Value", "Text");
                ViewData["FundingAgency"] = new SelectList(objDAL.GetFundingAgency(), "Value", "Text");
                return View(new FundAllocationFilterViewModel());
            }


        }

        /// <summary>
        /// Returns the list of Fund Allocation Details to view
        /// </summary>
        /// <param name="fundCollection"></param>
        /// <returns></returns>
        [HttpPost]
        [Audit]
        public ActionResult GetFundAllocationList(int? page, int? rows, string sidx, string sord)
        {
            int stateCode = 0;
            string fundType = string.Empty;
            int fundingAgencyCode = 0;
            int yearCode = 0;
            long totalRecords = 0;

            using (CommonFunctions commonFunction = new CommonFunctions())
            {
                if (!commonFunction.ValidateGridParameters(new GridParams(page, rows, sidx, sord, Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                {
                    return null;
                }
            }

            if (!string.IsNullOrEmpty(Request.Params["stateCode"]))
            {
                stateCode = Convert.ToInt32(Request.Params["stateCode"]);
            }

            if (!string.IsNullOrEmpty(Request.Params["fundType"]))
            {
                fundType = Request.Params["fundType"];
            }

            if (!string.IsNullOrEmpty(Request.Params["yearCode"]))
            {
                yearCode = Convert.ToInt32(Request.Params["yearCode"]);
            }

            if (!string.IsNullOrEmpty(Request.Params["fundingAgencyCode"]))
            {
                fundingAgencyCode = Convert.ToInt32(Request.Params["fundingAgencyCode"]);
            }

            var jsonData = new
            {
                rows = objBAL.GetFundAllocationList(stateCode, fundType, fundingAgencyCode, yearCode, Convert.ToInt32(page) - 1, Convert.ToInt32(rows), sidx, sord, out totalRecords),
                total = totalRecords <= Convert.ToInt32(rows) ? 1 : totalRecords / Convert.ToInt32(rows) + 1,
                page = Convert.ToInt32(page),
                records = totalRecords
            };

            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Returns the Add View of Fund Allocation
        /// </summary>
        /// <param name="stateCode"></param>
        /// <param name="fundType"></param>
        /// <returns></returns>
        [Audit]
        public ActionResult AddEditFundAllocation(int stateCode, string fundType)
        {
            FundAllocationViewModel model = new FundAllocationViewModel();
            try
            {
                if (PMGSYSession.Current.RoleCode == 25)
                {
                    model.releaseType = "C";
                }
                else
                {
                    model.releaseType = "S";
                    model.MAST_STATE_CODE = PMGSYSession.Current.StateCode;
                }

                ViewData["State"] = new SelectList(objDAL.GetAllStates(), "Value", "Text");
                ViewData["FundType"] = new SelectList(objDAL.GetFundType(true), "Value", "Text");
                ViewData["FundingAgency"] = new SelectList(objDAL.GetFundingAgency(), "Value", "Text");
                ViewData["ExecutingAgency"] = new SelectList(objDAL.GetExecutingAgencyByStateCode(stateCode), "Value", "Text").OrderBy(m=>m.Value);
                ViewData["Year"] = new SelectList(objDAL.GetAllYear(), "Value", "Text");
                return PartialView("AddEditFundAllocation", model);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
           
                return PartialView("AddEditFundAllocation", new FundAllocationViewModel());
            }
        }

        /// <summary>
        /// Save Fund Allocation Details
        /// </summary>
        /// <param name="fundModel"></param>
        /// <returns></returns>
        [HttpPost]
        [Audit]
        public ActionResult AddFundAllocation(FundAllocationViewModel fundModel)
        {
            bool status = false;
            try
            {
                if (ModelState.IsValid)
                {
                    if (objBAL.AddFundAllocation(fundModel, ref message))
                    {
                        message = message == string.Empty ? "Fund allocation details added successfully." : message;
                        status = true;
                        return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        message = message == string.Empty ? "Fund allocation details not added successfully." : message;
                        return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    string messages = string.Join("<br/>", ModelState.Values
                                             .SelectMany(x => x.Errors)
                                             .Select(x => x.ErrorMessage));

                    return Json(new { success = false, message = messages }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                message = message == string.Empty ? "Fund allocation details not added successfully." : message;
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// Delete Fund Allocation Details
        /// </summary>
        /// <param name="urlparameter"></param>
        /// <returns></returns>
        [HttpPost]
        [Audit]
        public ActionResult DeleteFundAllocation(String parameter, String hash, String key) //string urlparameter
        {
            Dictionary<string, string> decryptedParameters = null;
            int transactionCode = 0;
            int stateCode = 0;
            int adminCode = 0;
            int yearCode = 0;
            string fundType = string.Empty;
            int fundingAgencyCode = 0;
            try
            {
                decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { parameter, hash, key });
                transactionCode = Convert.ToInt32(decryptedParameters["TId"]);
                stateCode = Convert.ToInt32(decryptedParameters["StateId"]);
                adminCode = Convert.ToInt32(decryptedParameters["AdminCode"]);
                yearCode = Convert.ToInt32(decryptedParameters["YearCode"]);
                //releaseType = decryptedParameters["ReleaseType"];
                fundType = decryptedParameters["FundType"];
                fundingAgencyCode = Convert.ToInt32(decryptedParameters["FundingAgency"]);

                if (!(objBAL.DeleteFundAllocation(transactionCode, stateCode, adminCode, yearCode, fundType, fundingAgencyCode,ref message)))
                {
                    return Json(new { success = false, message = message }, JsonRequestBehavior.AllowGet);
                }

                return Json(new { success = true, message = "Fund allocation details deleted successfully" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
           
                return Json(new { success = false, message = "Fund allocation details can not be deleted" }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// Returns the details to edit
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Audit]
        public ActionResult EditFundAllocation(String parameter, String hash, String key)//string id
        {
            int transactionCode = 0;
            int stateCode = 0;
            int adminCode = 0;
            int yearCode = 0;
            int fundingAgencyCode = 0;
            string releaseType = string.Empty;
            string fundType = string.Empty;
            Dictionary<string, string> decryptedParameters = null;
            try
            {
                decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { parameter, hash, key });
                transactionCode = Convert.ToInt32(decryptedParameters["TId"]);
                stateCode = Convert.ToInt32(decryptedParameters["StateId"]);
                adminCode = Convert.ToInt32(decryptedParameters["AdminCode"]);
                yearCode = Convert.ToInt32(decryptedParameters["YearCode"]);
                fundType = decryptedParameters["FundType"];
                fundingAgencyCode = Convert.ToInt32(decryptedParameters["FundingAgency"]);

                FundAllocationViewModel model = objBAL.GetFundAllocationDetails(transactionCode, stateCode, adminCode, yearCode, fundType, fundingAgencyCode);
                if (model == null)
                {
                    return PartialView("AddEditFundAllocation", new FundAllocationViewModel());
                }
                ViewData["State"] = new SelectList(objDAL.GetAllStates(), "Value", "Text");
                ViewData["FundType"] = new SelectList(objDAL.GetFundType(true), "Value", "Text");
                ViewData["FundingAgency"] = new SelectList(objDAL.GetFundingAgency(), "Value", "Text");
                ViewData["ExecutingAgency"] = new SelectList(objDAL.GetExecutingAgencyByStateCode(model.MAST_STATE_CODE), "Value", "Text");
                ViewData["Year"] = new SelectList(objDAL.GetAllYear(), "Value", "Text");
                return PartialView("AddEditFundAllocation", model);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return PartialView("AddEditFundAllocation", new FundAllocationViewModel());
            }
        }

        /// <summary>
        /// Update the Edited Details
        /// </summary>
        /// <param name="fundModel"></param>
        /// <returns></returns>
        [HttpPost]
        [Audit]
        public ActionResult EditFundAllocation(FundAllocationViewModel fundModel)
        {
            bool status = false;
            try
            {
                if (ModelState.IsValid)
                {
                    if (objBAL.EditFundAllocation(fundModel, ref message))
                    {
                        message = message == string.Empty ? "Fund Allocation details updated successfully." : message;
                        status = true;
                        return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        message = message == string.Empty ? "Fund Allocation details not updated successfully." : message;
                        return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    string messages = string.Join("<br/>", ModelState.Values
                                            .SelectMany(x => x.Errors)
                                            .Select(x => x.ErrorMessage));

                    return Json(new { success = false, message = messages }, JsonRequestBehavior.AllowGet);
                }

            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                message = message == string.Empty ? "Fund Allocation details not updated successfully." : message;
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// Returns the total Allocation Amount,transaction no and available allocation
        /// </summary>
        /// <param name="stateCode"></param>
        /// <param name="yearCode"></param>
        /// <param name="fundType"></param>
        /// <param name="executingAgencyCode"></param>
        /// <param name="agencyCode"></param>
        /// <returns></returns>
        [HttpPost]
        [Audit]
        public JsonResult GetAllocationData()
        {
            int stateCode = 0;
            int yearCode = 0;
            string fundType = string.Empty;
            int executingAgencyCode = 0;
            int agencyCode = 0;
            int transactionNo = 0;
            double? totalAllocation = 0;
            double? remainingAllocation = 0;
            try
            {
                stateCode = Convert.ToInt32(Request.Params["stateCode"]);
                yearCode = Convert.ToInt32(Request.Params["yearCode"]);
                executingAgencyCode = Convert.ToInt32(Request.Params["executingAgencyCode"]);
                agencyCode = Convert.ToInt32(Request.Params["agencyCode"]);
                fundType = Request.Params["fundType"];
                transactionNo = db.MRD_FUND_ALLOCATION.Where(m => m.MAST_STATE_CODE == stateCode && m.ADMIN_NO_CODE == executingAgencyCode && m.MAST_YEAR == yearCode && m.MAST_FUND_TYPE == fundType && m.MAST_FUNDING_AGENCY_CODE == agencyCode).OrderByDescending(m=>m.MAST_TRANSACTION_NO).Select(m=>m.MAST_TRANSACTION_NO).FirstOrDefault();
                transactionNo = transactionNo + 1;
                totalAllocation = objDAL.GetFundAllocationTotal(stateCode, yearCode, fundType, executingAgencyCode, "C", agencyCode);
                remainingAllocation = objDAL.GetRemainingFundAllocationTotal(stateCode, yearCode, fundType, executingAgencyCode, agencyCode);
                return Json(new { success = true, transactionNo = transactionNo, totalAllocation = totalAllocation, remainingAllocation = remainingAllocation }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return Json(new { success = false, result = "" }, JsonRequestBehavior.AllowGet);
            }
            finally
            {
                db.Dispose();
            }
        }

        /// <summary>
        /// Fund Allocation Sanction Letter Upload View
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Audit]
        public ActionResult FileUpload(String hash, String parameter, String key)
        {
            int transactionCode = 0;
            int stateCode = 0;
            int adminCode = 0;
            int yearCode = 0;
            string fundType = string.Empty;
            int fundingAgencyCode = 0;
            string releaseType = string.Empty;
            Dictionary<string, string> decryptedParameters = null;
            try
            {
                decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { parameter, hash, key });

                transactionCode = Convert.ToInt32(decryptedParameters["TId"]);
                stateCode = Convert.ToInt32(decryptedParameters["StateId"]);
                adminCode = Convert.ToInt32(decryptedParameters["AdminCode"]);
                yearCode = Convert.ToInt32(decryptedParameters["YearCode"]);
                //releaseType = decryptedParameters["ReleaseType"];
                fundType = decryptedParameters["FundType"];
                fundingAgencyCode = Convert.ToInt32(decryptedParameters["FundingAgency"]);

                FundAllocationViewModel fundModel = objBAL.GetFundAllocationDetails(transactionCode, stateCode, adminCode, yearCode, fundType, fundingAgencyCode);
                if (fundModel.MAST_ALLOCATION_FILE != null)
                {
                    fundModel.NumberofFiles = 1;
                }
                else
                {
                    fundModel.NumberofFiles = 0;
                }
                if (fundModel == null)
                {
                    return PartialView("FileUpload", new FundAllocationViewModel());
                }
                return PartialView("FileUpload", fundModel);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return PartialView("FileUpload", new FundAllocationViewModel());
            }
        }

        /// <summary>
        /// Fund Release Sanction Letter Upload View
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Audit]
        public ActionResult FileUploadFundRelease(String parameter, String hash, String key)
        {
            int transactionCode = 0;
            int stateCode = 0;
            int adminCode = 0;
            int yearCode = 0;
            string releaseType = string.Empty;
            string fundType = string.Empty;
            int fundingAgencyCode = 0;

            Dictionary<string, string> decryptedParameters = null;
            try
            {
                decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { parameter, hash, key });
                transactionCode = Convert.ToInt32(decryptedParameters["TId"]);
                stateCode = Convert.ToInt32(decryptedParameters["StateId"]);
                adminCode = Convert.ToInt32(decryptedParameters["AdminCode"]);
                yearCode = Convert.ToInt32(decryptedParameters["YearCode"]);
                releaseType = decryptedParameters["ReleaseType"];
                fundType = decryptedParameters["FundType"];
                fundingAgencyCode = Convert.ToInt32(decryptedParameters["FundingAgency"]);

                FundReleaseViewModel model = objBAL.GetFundReleaseDetails(transactionCode, stateCode, adminCode, yearCode, releaseType, fundType, fundingAgencyCode);
                if (model.MAST_RELEASE_FILE != null)
                {
                    model.NumberofFiles = 1;
                }
                else
                {
                    model.NumberofFiles = 0;
                }


                if (model == null)
                {
                    return PartialView("FileUploadFundRelease", new FundReleaseViewModel());
                }
                ViewData["State"] = new SelectList(objDAL.GetAllStates(), "Value", "Text");
                ViewData["FundType"] = new SelectList(objDAL.GetFundType(), "Value", "Text");
                ViewData["FundingAgency"] = new SelectList(objDAL.GetFundingAgency(), "Value", "Text");
                ViewData["ExecutingAgency"] = new SelectList(objDAL.GetExecutingAgencyByStateCode(model.MAST_STATE_CODE), "Value", "Text");
                ViewData["Year"] = new SelectList(objDAL.GetAllYear(), "Value", "Text");
                return PartialView("FileUploadFundRelease", model);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return PartialView("FileUploadFundRelease", new FundReleaseViewModel());
            }
        }

        /// <summary>
        /// Upload the Sanction Letter for Fund Allocation
        /// </summary>
        /// <param name="fundModel"></param>
        /// <returns></returns>
        [HttpPost]
        [Audit]
        public ActionResult PdfFileUpload(FundAllocationViewModel fundModel)
        {
            CommonFunctions objCommonFunc = new CommonFunctions();
            if (!(objCommonFunc.ValidateIsPdf(ConfigurationManager.AppSettings["FUND_ALLOCATION_FILE_UPLOAD"], Request)))
            {
                fundModel.ErrorMessage = "File Type is Not Allowed.";
                return View("FileUpload", fundModel.ErrorMessage);
            }
            var fileData = fundModel;

            int roadCode = fundModel.transactionCount;//fundModel.MAST_TRANSACTION_NO;
            foreach (string file in Request.Files)
            {
                UploadPDFFile(Request, fileData, roadCode);
            }

            var serializer = new JavaScriptSerializer();
            serializer.MaxJsonLength = Int32.MaxValue;
            var result = new ContentResult
            {
                Content = "{\"files\":" + serializer.Serialize(fileData) + "}",
            };
            return result;
        }

        /// <summary>
        /// Upload the Sanction Letter for Fund Release
        /// </summary>
        /// <param name="fundModel"></param>
        /// <returns></returns>
        [HttpPost]
        [Audit]
        public ActionResult PdfFileUploadFundRelease(FundReleaseViewModel fundModel)
        {   
            CommonFunctions objCommonFunc = new CommonFunctions();
            if (!(objCommonFunc.ValidateIsPdf(ConfigurationManager.AppSettings["FUND_RELEASE_FILE_UPLOAD"], Request)))
            {
                fundModel.ErrorMessage = "File Type is Not Allowed.";
                return View("FileUploadFundRelease", fundModel.ErrorMessage);
            }
            var fileData = fundModel;

            int roadCode = fundModel.MAST_TRANSACTION_NO;
            foreach (string file in Request.Files)
            {
                UploadPDFFileFundRelease(Request, fileData, roadCode);
            }

            var serializer = new JavaScriptSerializer();
            serializer.MaxJsonLength = Int32.MaxValue;
            var result = new ContentResult
            {
                Content = "{\"files\":" + serializer.Serialize(fileData) + "}",
            };
            return result;
        }

        /// <summary>
        /// uploads the release transaction file
        /// </summary>
        /// <param name="request">request components</param>
        /// <param name="statuses">list of files along with other model details</param>
        /// <param name="releaseCode">transaction id</param>
        public void UploadPDFFileFundRelease(HttpRequestBase request, FundReleaseViewModel statuses, int releaseCode)
        {
            String StorageRoot = ConfigurationManager.AppSettings["FUND_RELEASE_FILE_UPLOAD"];

            for (int i = 0; i < request.Files.Count; i++)
            {
                HttpPostedFileBase file = request.Files[i];
                var fileId = releaseCode;
                var fileName = statuses.MAST_YEAR + "-" + releaseCode + "-" + statuses.MAST_FUND_TYPE + "-" + statuses.MAST_RELEASE_TYPE + Path.GetExtension(request.Files[i].FileName).ToString();
                var fullPath = Path.Combine(StorageRoot, fileName);

                statuses.url = fullPath;
                statuses.thumbnail_url = fullPath;
                statuses.name = fileName;
                statuses.type = file.ContentType;
                statuses.size = file.ContentLength;

                bool status = objBAL.AddFileUploadToTransactionRelease(statuses);
                if (status == true)
                {
                    //file.SaveAs(fullPath);
                    file.SaveAs(Path.Combine(ConfigurationManager.AppSettings["FUND_RELEASE_FILE_UPLOAD"], fileName));
                }
                else
                {
                    // show an error over here
                }
            }
        }

        /// <summary>
        /// uploads the Allocation transaction file 
        /// </summary>
        /// <param name="request">request component</param>
        /// <param name="statuses">list of files along with the allocation details</param>
        /// <param name="roadCode">id of transaction</param>
        public void UploadPDFFile(HttpRequestBase request, FundAllocationViewModel statuses, int roadCode)
        {
            String StorageRoot = ConfigurationManager.AppSettings["FUND_ALLOCATION_FILE_UPLOAD"];
            int MaxCount = 0;

            for (int i = 0; i < request.Files.Count; i++)
            {
                HttpPostedFileBase file = request.Files[i];
                var fileId = roadCode;
                var fileName = statuses.MAST_YEAR + "-" + roadCode + "-" + statuses.MAST_FUND_TYPE + Path.GetExtension(request.Files[i].FileName).ToString();
                var fullPath = Path.Combine(StorageRoot, fileName);

                statuses.url = fullPath;
                statuses.thumbnail_url = fullPath;
                statuses.name = fileName;
                statuses.type = file.ContentType;
                statuses.size = file.ContentLength;

                bool status = objBAL.AddFileUploadToTransaction(statuses);
                if (status == true)
                {
                    //file.SaveAs(fullPath);
                    file.SaveAs(Path.Combine(ConfigurationManager.AppSettings["FUND_ALLOCATION_FILE_UPLOAD"], fileName));
                }
                else
                {
                    // show an error over here
                }
            }
        }

        /// <summary>
        /// List the Uploaded files
        /// </summary>
        /// <param name="page">No. of pages of list</param>
        /// <param name="rows">No. of rows</param>
        /// <param name="sidx">sort index</param>
        /// <param name="sord">sort order</param>
        /// <returns></returns>
        [Audit]
        public ActionResult ListFiles(int? page, int? rows, string sidx, string sord)
        {
            try
            {
                //int PLAN_CN_ROAD_CODE = Convert.ToInt32(Request["PLAN_CN_ROAD_CODE"]);
                int stateCode = Convert.ToInt32(Request.Params["STATE_CODE"]);
                int adminCode = Convert.ToInt32(Request.Params["ADMIN_CODE"]);
                int agencyCode = Convert.ToInt32(Request.Params["FUNDING_AGENCY"]);
                int transactionCode = Convert.ToInt32(Request.Params["TRANSACTION"]);
                int yearCode = Convert.ToInt32(Request.Params["YEAR"]);
                string fundType = Request.Params["FUND_TYPE"];
                long totalRecords;
                var jsonData = new
                {
                    rows = objBAL.GetListFilesBAL(Convert.ToInt32(Request.Params["page"]) - 1, Convert.ToInt32(Request.Params["rows"]), Request.Params["sidx"], Request.Params["sord"], out totalRecords,stateCode,adminCode,fundType,agencyCode,yearCode,transactionCode),
                    total = totalRecords <= Convert.ToInt32(Request.Params["rows"]) ? 1 : totalRecords / Convert.ToInt32(Request.Params["rows"]) + 1,
                    page = Convert.ToInt32(Request.Params["page"]),
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
        /// For Downloading the Sanction Letter of Fund Allocation
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="hash"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        [HttpGet]
        [Audit]
        public ActionResult DownloadFundAllocationFile(String parameter, String hash, String key)
        {
            string FileName = string.Empty;
            string FullFileLogicalPath = string.Empty;
            string FullfilePhysicalPath = string.Empty;
            string FileExtension = string.Empty;

            if (!String.IsNullOrEmpty(parameter) && !String.IsNullOrEmpty(hash) && !String.IsNullOrEmpty(key))
            {
                String[] urlParams = URLEncrypt.DecryptParameters(new String[] { parameter, hash, key });
                if (urlParams.Length >= 1)
                {
                    String[] urlSplitParams = urlParams[0].Split('$');
                    FileName = (urlSplitParams[0]);

                }
            }


            FileExtension = Path.GetExtension(FileName).ToLower();

            if (FileExtension == ".pdf")
            {
                FullFileLogicalPath = Path.Combine(ConfigurationManager.AppSettings["FUND_ALLOCATION_FILE_UPLOAD_VIRTUAL_DIR_PATH"], FileName);
                FullfilePhysicalPath = Path.Combine(ConfigurationManager.AppSettings["FUND_ALLOCATION_FILE_UPLOAD"], FileName);
            }


            string name = Path.GetFileName(FileName);
            string ext = Path.GetExtension(FileName);

            string type = string.Empty;

            if (ext != null)
            {
                switch (ext.ToLower())
                {
                    case ".pdf":
                        type = "Application/pdf";
                        break;

                }
            }

            if (System.IO.File.Exists(FullfilePhysicalPath))
            {
                return File(FullfilePhysicalPath, type, DateTime.Now.ToShortDateString() + "_" + new Random().Next(1000000000) + FileExtension);
            }
            else
            {
                return Json(new { Success = "false" });
            }
        }

        /// <summary>
        /// deletes the file of allocation transaction
        /// </summary>
        /// <param name="parameter">encrypted id</param>
        /// <param name="hash">encrypted id</param>
        /// <param name="key">encrypted id</param>
        /// <returns></returns>
        [Audit]
        public ActionResult DeleteFundAllocationFile(String parameter, String hash, String key)
        {
            try
            {
                Dictionary<string, string> decryptedParameters = null;
                bool status = false ;
                if (!String.IsNullOrEmpty(parameter) && !String.IsNullOrEmpty(hash) && !String.IsNullOrEmpty(key))
                {
                    decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { parameter, hash, key });
                    string fileName = decryptedParameters["FileName"];
                    int stateCode = Convert.ToInt32(decryptedParameters["State"]);
                    string[] fileString = fileName.Split('.');
                    status = objDAL.DeleteAllocationFile(fileString[0],stateCode, ref message);
                }
                if (status == true)
                {
                    return Json(new { Success = true, ErrorMessage = "File Deleted Successfully." });
                }
                else
                {
                    return Json(new { Success = false, ErrorMessage = message });
                }

                
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return null;
            }
        }


        //public ActionResult CheckReleaseAmount()
        //{
        //    try
        //    {
        //        bool status = objBAL.CheckReleaseAmount();
        //        if (status == true)
        //        {
        //            return Json(new { success = true});
        //        }
        //        else
        //        {
        //            return Json(new { success = false });
        //        }
        //    }
        //    catch (Exception)
        //    {
        //        return Json(new { success = false });
        //    }
        //}


        #endregion

        #region FUND_RELEASE

        /// <summary>
        /// List View of Fund Release
        /// </summary>
        /// <returns></returns>
        [Audit]
        public ActionResult ListFundReleaseDetails()
        {
            if (PMGSYSession.Current.RoleCode == 2 || PMGSYSession.Current.RoleCode == 37)
            {
                FundReleaseFilterViewModel model = new FundReleaseFilterViewModel();
                model.State = PMGSYSession.Current.StateCode;
                ViewData["State"] = new SelectList(objDAL.GetAllStates(), "Value", "Text", model.State);
                ViewData["FundType"] = new SelectList(objDAL.GetFundType(), "Value", "Text");
                ViewData["Year"] = new SelectList(objDAL.GetAllYear(), "Value", "Text");
                ViewData["FundingAgency"] = new SelectList(objDAL.GetFundingAgency(), "Value", "Text");
                List<SelectListItem> lstReleaser = new List<SelectListItem>();
                //lstReleaser.Insert(0,new SelectListItem{Value = "0",Text = "All"});
                lstReleaser.Add(new SelectListItem{Value = "S",Text = "State"});
                lstReleaser.Add(new SelectListItem{Value = "C",Text = "MoRD"});
                ViewData["Releaser"] = lstReleaser;
                return View(model);
            }
            else
            {
                List<SelectListItem> lstReleaser = new List<SelectListItem>();
                //lstReleaser.Insert(0, new SelectListItem { Value = "0", Text = "All",Selected = true });
                lstReleaser.Add(new SelectListItem { Value = "S", Text = "State" });
                lstReleaser.Add(new SelectListItem { Value = "C", Text = "MoRD" });
                ViewData["Releaser"] = lstReleaser;
                ViewData["State"] = new SelectList(objDAL.GetAllStates(), "Value", "Text");
                ViewData["FundType"] = new SelectList(objDAL.GetFundType(), "Value", "Text");
                ViewData["Year"] = new SelectList(objDAL.GetAllYear(), "Value", "Text");
                ViewData["FundingAgency"] = new SelectList(objDAL.GetFundingAgency(), "Value", "Text");
                return View(new FundReleaseFilterViewModel());
            }

        }

        /// <summary>
        /// Returns the list of Fund Released for various year
        /// </summary>
        /// <param name="fundCollection"></param>
        /// <returns></returns>
        [HttpPost]
        [Audit]
        public ActionResult GetFundReleaseList(int? page, int? rows, string sidx, string sord)
        {
            int stateCode = 0;
            string fundType = string.Empty;
            int fundingAgencyCode = 0;
            int yearCode = 0;
            long totalRecords = 0;
            string releaser = string.Empty;

            using (CommonFunctions commonFunction = new CommonFunctions())
            {
                if (!commonFunction.ValidateGridParameters(new GridParams(page, rows, sidx, sord, Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                {
                    return null;
                }
            }

            if (!string.IsNullOrEmpty(Request.Params["stateCode"]))
            {
                stateCode = Convert.ToInt32(Request.Params["stateCode"]);
            }

            if (!string.IsNullOrEmpty(Request.Params["fundType"]))
            {
                fundType = Request.Params["fundType"];
            }

            if (!string.IsNullOrEmpty(Request.Params["yearCode"]))
            {
                yearCode = Convert.ToInt32(Request.Params["yearCode"]);
            }

            if (!string.IsNullOrEmpty(Request.Params["fundingAgencyCode"]))
            {
                fundingAgencyCode = Convert.ToInt32(Request.Params["fundingAgencyCode"]);
            }

            if (!string.IsNullOrEmpty(Request.Params["ReleaseBy"]))
            {
                releaser = Request.Params["ReleaseBy"];
            }

            var jsonData = new
            {
                rows = objBAL.GetFundReleaseList(stateCode, fundType, fundingAgencyCode, yearCode, releaser,Convert.ToInt32(page) - 1, Convert.ToInt32(rows), sidx, sord, out totalRecords),
                total = totalRecords <= Convert.ToInt32(rows) ? 1 : totalRecords / Convert.ToInt32(rows) + 1,
                page = Convert.ToInt32(page),
                records = totalRecords
            };

            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Returns the Add view of Fund Release
        /// </summary>
        /// <param name="stateCode"></param>
        /// <param name="fundType"></param>
        /// <returns></returns>
        [Audit]
        public ActionResult AddEditFundRelease(int stateCode, string fundType)
        {
            try
            {
                FundReleaseViewModel model = new FundReleaseViewModel();
                if (PMGSYSession.Current.RoleCode == 25)
                {
                    model.MAST_RELEASE_TYPE = "C";
                }
                else
                {
                    model.MAST_RELEASE_TYPE = "S";
                    model.MAST_STATE_CODE = PMGSYSession.Current.StateCode;
                }

                ViewData["State"] = new SelectList(objDAL.GetAllStates(), "Value", "Text");
                ViewData["FundType"] = new SelectList(objDAL.GetFundType(true), "Value", "Text");
                ViewData["FundingAgency"] = new SelectList(objDAL.GetFundingAgency(), "Value", "Text");
                ViewData["ExecutingAgency"] = new SelectList(objDAL.GetExecutingAgencyByStateCode(stateCode), "Value", "Text");
                ViewData["Year"] = new SelectList(objDAL.GetAllYear(), "Value", "Text");
                return PartialView("AddEditFundRelease", model);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return PartialView("AddEditFundRelease", new FundReleaseViewModel());
            }
        }

        /// <summary>
        /// Save the Fund Released Details
        /// </summary>
        /// <param name="fundModel"></param>
        /// <returns></returns>
        [HttpPost]
        [Audit]
        public ActionResult AddFundRelease(FundReleaseViewModel fundModel)
        {
            bool status = false;
            try
            {
                if (ModelState.IsValid)
                {
                    if (objBAL.AddFundRelease(fundModel, ref message))
                    {
                        message = message == string.Empty ? "Fund release details added successfully." : message;
                        status = true;
                        return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        message = message == string.Empty ? "Fund release details not added successfully." : message;
                        return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    status = false;
                    string messages = string.Join("; ", ModelState.Values
                                              .SelectMany(x => x.Errors)
                                              .Select(x => x.ErrorMessage));

                    return Json(new { success = status, message = messages }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                message = message == string.Empty ? "Fund release details not added successfully." : message;
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// returns the Fund Release Details to edit
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Audit]
        public ActionResult EditFundRelease(String parameter, String hash, String key) //string id
        {
            int transactionCode = 0;
            int stateCode = 0;
            int adminCode = 0;
            int yearCode = 0;
            string releaseType = string.Empty;
            string fundType = string.Empty;
            Dictionary<string, string> decryptedParameters = null;
            int fundingAgencyCode = 0;
            try
            {
                decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { parameter, hash, key });

                transactionCode = Convert.ToInt32(decryptedParameters["TId"]);
                stateCode = Convert.ToInt32(decryptedParameters["StateId"]);
                adminCode = Convert.ToInt32(decryptedParameters["AdminCode"]);
                yearCode = Convert.ToInt32(decryptedParameters["YearCode"]);
                releaseType = decryptedParameters["ReleaseType"];
                fundType = decryptedParameters["FundType"];
                fundingAgencyCode = Convert.ToInt32(decryptedParameters["FundingAgency"]);


                FundReleaseViewModel model = objBAL.GetFundReleaseDetails(transactionCode, stateCode, adminCode, yearCode, releaseType, fundType, fundingAgencyCode);
                model.transactionCountRelease = transactionCode;
                if (model == null)
                {
                    return PartialView("AddEditFundRelease", new FundReleaseViewModel());
                }
                ViewData["State"] = new SelectList(objDAL.GetAllStates(), "Value", "Text");
                ViewData["FundType"] = new SelectList(objDAL.GetFundType(true), "Value", "Text");
                ViewData["FundingAgency"] = new SelectList(objDAL.GetFundingAgency(), "Value", "Text");
                ViewData["ExecutingAgency"] = new SelectList(objDAL.GetExecutingAgencyByStateCode(model.MAST_STATE_CODE, model.ADMIN_NO_CODE), "Value", "Text", model.ADMIN_NO_CODE);
                ViewData["Year"] = new SelectList(objDAL.GetAllYear(), "Value", "Text");
                return PartialView("AddEditFundRelease", model);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return PartialView("AddEditFundRelease", new FundReleaseViewModel());
            }
        }

        /// <summary>
        /// Update the Edited Fund Release Details
        /// </summary>
        /// <param name="fundModel"></param>
        /// <returns></returns>
        [HttpPost]
        [Audit]
        public ActionResult EditFundRelease(FundReleaseViewModel fundModel)
        {
            bool status = false;
            try
            {
                if (ModelState.IsValid)
                {
                    if (objBAL.EditFundRelease(fundModel, ref message))
                    {
                        message = message == string.Empty ? "Fund Release details updated successfully." : message;
                        status = true;
                        return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        message = message == string.Empty ? "Fund Release details not updated successfully." : message;
                        return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    status = false;
                    string messages = string.Join("; ", ModelState.Values
                                              .SelectMany(x => x.Errors)
                                              .Select(x => x.ErrorMessage));

                    return Json(new { success = status, message = messages }, JsonRequestBehavior.AllowGet);
                    //return Json(new { success = status, message = "Fund Release details not updated successfully." }, JsonRequestBehavior.AllowGet);
                }

            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                message = message == string.Empty ? "Fund Release details not updated successfully." : message;
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// Populates SRRDA dropdown 
        /// </summary>
        /// <param name="stateCode"></param>
        /// <returns></returns>
        [HttpPost]
        [Audit]
        public JsonResult GetExecutingAgencyByState(int stateCode)
        {
            try
            {
                List<SelectListItem> lstState = objDAL.GetExecutingAgencyByStateCode(stateCode);
                lstState.Add(new SelectListItem { Value = "0", Text = "--Select Agency--" });
                return Json(new SelectList(lstState, "Value", "Text").OrderBy(m => m.Value), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return null;
            }
        }

        /// <summary>
        /// Delete Fund Release Details
        /// </summary>
        /// <param name="urlparameter"></param>
        /// <returns></returns>
        [HttpPost]
        [Audit]
        public ActionResult DeleteFundRelease(String parameter, String hash, String key)  //string urlparameter
        {
            int transactionCode = 0;
            int stateCode = 0;
            int adminCode = 0;
            int yearCode = 0;
            string releaseType = string.Empty;
            string fundType = string.Empty;
            int fundingAgencyCode = 0;
            Dictionary<string, string> decryptedParameters = null;
            try
            {
                decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { parameter, hash, key });

                decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { parameter, hash, key });
                transactionCode = Convert.ToInt32(decryptedParameters["TId"]);
                stateCode = Convert.ToInt32(decryptedParameters["StateId"]);
                adminCode = Convert.ToInt32(decryptedParameters["AdminCode"]);
                yearCode = Convert.ToInt32(decryptedParameters["YearCode"]);
                releaseType = decryptedParameters["ReleaseType"];
                fundType = decryptedParameters["FundType"];
                fundingAgencyCode = Convert.ToInt32(decryptedParameters["FundingAgency"]);


                if (!(objBAL.DeleteFundRelease(transactionCode, stateCode, adminCode, yearCode, releaseType, fundType, fundingAgencyCode)))
                {
                    return Json(new { success = false, message = "Fund release details not deleted successfully" }, JsonRequestBehavior.AllowGet);
                }

                return Json(new { success = true, message = "Fund release details deleted successfully" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return Json(new { success = false, message = "Fund release details can not be deleted" }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// Returns the total allocation amount
        /// </summary>
        /// <param name="stateCode"></param>
        /// <param name="yearCode"></param>
        /// <param name="fundType"></param>
        /// <param name="executingAgencyCode"></param>
        /// <param name="releaseType"></param>
        /// <param name="agencyCode"></param>
        /// <returns></returns>
        [HttpPost]
        [Audit]
        public JsonResult GetFundAllocationTotal()
        {
            try
            {
                int stateCode = Convert.ToInt32(Request.Params["stateCode"]);
                int yearCode = Convert.ToInt32(Request.Params["yearCode"]);
                string fundType = Request.Params["fundType"];
                int executingAgencyCode = Convert.ToInt32(Request.Params["executingAgencyCode"]);
                int agencyCode = Convert.ToInt32(Request.Params["agencyCode"]);
                string releaseType = Request.Params["releaseType"];
                double? total = objDAL.GetFundAllocationTotal(stateCode, yearCode, fundType, executingAgencyCode, releaseType, agencyCode);
                if (total == null)
                {
                    return Json(new { success = false }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { sucess = true, total = total }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return Json(new { success = false }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// returns the total release amount ,release no and available amount
        /// </summary>
        /// <param name="stateCode"></param>
        /// <param name="yearCode"></param>
        /// <param name="fundType"></param>
        /// <param name="executingAgencyCode"></param>
        /// <param name="agencyCode"></param>
        /// <param name="releaseType"></param>
        /// <returns></returns>
        [HttpPost]
        [Audit]
        public JsonResult GetReleaseData()
        {
            int transactionNo = 0;
            double? totalRelease = 0;
            double? remainingRelease = 0;
            decimal? totalAllocation = 0;
            try
            {
                int stateCode = Convert.ToInt32(Request.Params["stateCode"]);
                int yearCode = Convert.ToInt32(Request.Params["yearCode"]);
                string fundType = Request.Params["fundType"];
                int executingAgencyCode = Convert.ToInt32(Request.Params["executingAgencyCode"]);
                int agencyCode = Convert.ToInt32(Request.Params["agencyCode"]);
                string releaseType = Request.Params["releaseType"];
                transactionNo = db.MRD_FUND_RELEASE.Where(m => m.MAST_STATE_CODE == stateCode && m.ADMIN_NO_CODE == executingAgencyCode && m.MAST_YEAR == yearCode && m.MAST_FUND_TYPE == fundType && m.MAST_FUNDING_AGENCY_CODE == agencyCode && m.MAST_RELEASE_TYPE == releaseType).OrderByDescending(m=>m.MAST_TRANSACTION_NO).Select(m=>m.MAST_TRANSACTION_NO).FirstOrDefault();// .Select(m=>m.MAST_TRANSACTION_NO).OrderByDescending();
                transactionNo = transactionNo + 1;
                totalRelease = objDAL.GetFundReleaseTotal(stateCode, yearCode, fundType, executingAgencyCode, "C", agencyCode);
                remainingRelease = objDAL.GetFundAllocationTotal(stateCode, yearCode, fundType, executingAgencyCode, releaseType, agencyCode);
                totalAllocation = objDAL.GetTotalAllocation(stateCode,yearCode,fundType,executingAgencyCode,agencyCode);
                return Json(new { success = true, transactionNo = transactionNo, totalRelease = totalRelease, remainingRelease = remainingRelease,totalAllocation=totalAllocation }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return Json(new { success = false, result = "" }, JsonRequestBehavior.AllowGet);
            }
            finally
            {
                db.Dispose();
            }
        }

        /// <summary>
        /// calculates the Release amount 
        /// </summary>
        /// <param name="stateCode">id of state</param>
        /// <param name="yearCode">id of year</param>
        /// <param name="fundType">type of fund</param>
        /// <param name="executingAgencyCode">id of SRRDA</param>
        /// <param name="agencyCode">id of funding agency</param>
        /// <param name="releaseType">releaser type(s-state,c-mord)</param>
        /// <param name="releaseAmount">value of release amount</param>
        /// <returns></returns>
        [HttpPost]
        [Audit]
        public JsonResult GetRemainingReleaseAmount(int stateCode, int yearCode, string fundType, int executingAgencyCode, int agencyCode, string releaseType, double releaseAmount)
        {
            double? totalRelease = 0;
            double? totalAvailable = 0;
            try
            {
                totalRelease = objDAL.GetFundReleaseTotal(stateCode, yearCode, fundType, executingAgencyCode, releaseType, agencyCode);
                totalAvailable = objDAL.GetFundAllocationTotal(stateCode, yearCode, fundType, executingAgencyCode, releaseType, agencyCode);
                totalAvailable = totalAvailable - totalRelease;
                totalAvailable = totalAvailable - releaseAmount;
                return Json(new { success = true, totalRelease = totalRelease, totalAvailable = totalAvailable }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return Json(new { success = false });
            }

        }

        /// <summary>
        /// Download the Fund Release Sanction Letter
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="hash"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        [HttpGet]
        [Audit]
        public ActionResult DownloadFundReleaseFile(String parameter, String hash, String key)
        {
            string FileName = string.Empty;
            string FullFileLogicalPath = string.Empty;
            string FullfilePhysicalPath = string.Empty;
            string FileExtension = string.Empty;

            if (!String.IsNullOrEmpty(parameter) && !String.IsNullOrEmpty(hash) && !String.IsNullOrEmpty(key))
            {
                String[] urlParams = URLEncrypt.DecryptParameters(new String[] { parameter, hash, key });
                if (urlParams.Length >= 1)
                {
                    String[] urlSplitParams = urlParams[0].Split('$');
                    FileName = (urlSplitParams[0]);

                }
            }


            FileExtension = Path.GetExtension(FileName).ToLower();

            if (FileExtension == ".pdf")
            {
                FullFileLogicalPath = Path.Combine(ConfigurationManager.AppSettings["FUND_RELEASE_FILE_UPLOAD_VIRTUAL_DIR_PATH"], FileName);
                FullfilePhysicalPath = Path.Combine(ConfigurationManager.AppSettings["FUND_RELEASE_FILE_UPLOAD"], FileName);
            }


            string name = Path.GetFileName(FileName);
            string ext = Path.GetExtension(FileName);

            string type = string.Empty;

            if (ext != null)
            {
                switch (ext.ToLower())
                {
                    case ".pdf":
                        type = "Application/pdf";
                        break;

                }
            }

            if (System.IO.File.Exists(FullfilePhysicalPath))
            {
                return File(FullfilePhysicalPath, type, DateTime.Now.ToShortDateString() + "_" + new Random().Next(1000000000) + FileExtension);
            }
            else
            {
                return Json(new { Success = "false" });
            }
        }

        /// <summary>
        /// list of fund release files
        /// </summary>
        /// <param name="page">no. of pages</param>
        /// <param name="rows">no. of rows</param>
        /// <param name="sidx">sort column name</param>
        /// <param name="sord">sort order</param>
        /// <returns></returns>
        [Audit]
        public ActionResult ListFundReleaseFiles(int? page, int? rows, string sidx, string sord)
        {
            try
            {
                //int PLAN_CN_ROAD_CODE = Convert.ToInt32(Request["PLAN_CN_ROAD_CODE"]);
                int stateCode = Convert.ToInt32(Request.Params["STATE_CODE"]);
                int adminCode = Convert.ToInt32(Request.Params["ADMIN_CODE"]);
                int agencyCode = Convert.ToInt32(Request.Params["FUNDING_AGENCY"]);
                int transactionCode = Convert.ToInt32(Request.Params["TRANSACTION"]);
                int yearCode = Convert.ToInt32(Request.Params["YEAR"]);
                string fundType = Request.Params["FUND_TYPE"];
                string releaseType = Request.Params["RELEASE_TYPE"];
                long totalRecords;
                var jsonData = new
                {
                    rows = objBAL.GetListFundReleaseFilesBAL(Convert.ToInt32(Request.Params["page"]) - 1, Convert.ToInt32(Request.Params["rows"]), Request.Params["sidx"], Request.Params["sord"], out totalRecords, stateCode, adminCode, fundType, agencyCode, yearCode, transactionCode,releaseType),
                    total = totalRecords <= Convert.ToInt32(Request.Params["rows"]) ? 1 : totalRecords / Convert.ToInt32(Request.Params["rows"]) + 1,
                    page = Convert.ToInt32(Request.Params["page"]),
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
        /// deletes the fund release file
        /// </summary>
        /// <param name="parameter">encrypted id</param>
        /// <param name="hash">encrypted id</param>
        /// <param name="key">encrypted id</param>
        /// <returns></returns>
        [Audit]
        public ActionResult DeleteFundReleaseFile(String parameter, String hash, String key)
        {
            Dictionary<string,string> decryptedParameters = null;
            long totalrecords = 0;
            try
            {
                decryptedParameters = URLEncrypt.DecryptParameters1(new string[]{parameter,hash,key});
                string fileName = decryptedParameters["FileName"];
                string[] files = fileName.Split('.');
                int stateCode = Convert.ToInt32(decryptedParameters["State"]);
                bool status = objBAL.DeleteFundReleaseFile(files[0],stateCode,ref message);
                return Json(new { Success=true,message = message});
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return Json(new { Success = false, message = message });
            }
        }
        
        
        #endregion



    }
}
