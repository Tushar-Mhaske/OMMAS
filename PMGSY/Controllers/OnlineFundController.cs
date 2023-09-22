using PMGSY.BAL.OnlineFundProcess;
using PMGSY.Common;
using PMGSY.DAL.OnlineFundProcess;
using PMGSY.Extensions;
using PMGSY.Models.OnlineFundRequest;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PMGSY.Controllers
{
    [RequiredAuthorization]
    [RequiredAuthentication]
    public class OnlineFundController : Controller
    {
        private CommonFunctions objCommon = new CommonFunctions();
        private IOnlineFundProcessBAL objProcessBAL = new OnlineFundProcessBAL();
        private OnlineFundProcessDAL objProcessDAL = new OnlineFundProcessDAL();

        /// <summary>
        /// lists all the requests added by SRRDA
        /// </summary>
        /// <returns></returns>
        public ActionResult ListOnlineFundRequests()
        {
            try
            {
                if (PMGSYSession.Current.RoleCode == 2)
                {
                    List<SelectListItem> lstFirstItem = new List<SelectListItem>();
                    lstFirstItem.Insert(0, new SelectListItem { Value = "", Text = "Select" });
                    FundRequestFilterModel model = new FundRequestFilterModel();
                    model.lstBatches = objCommon.PopulateBatch(true);
                    model.lstCollaborations = objCommon.PopulateFundingAgency(true);
                    if (PMGSYSession.Current.RoleCode == 2)
                    {
                        model.lstAgencies = objCommon.PopulateAgenciesByStateAndDepartmentwise(PMGSYSession.Current.StateCode, PMGSYSession.Current.AdminNdCode, false);
                    }
                    else
                    {
                        model.lstAgencies = objCommon.PopulateAgencies(PMGSYSession.Current.StateCode, false);
                    }
                    model.lstStates = objCommon.PopulateStates(true);
                    model.lstYears = new SelectList(objCommon.PopulateFinancialYear(true, true).ToList(), "Value", "Text").ToList();
                    model.State = PMGSYSession.Current.StateCode;
                    return View(model);
                }
                else
                {
                    return View("ObservationDetails");
                }
            }
            catch (Exception)
            {
                ViewBag.ErrorMessage = "Error occurred while processing your request.";
                return View(new FundRequestFilterModel());
            }
        }

        /// <summary>
        /// returns the array of online fund requests made by current selection
        /// </summary>
        /// <returns></returns>
        public ActionResult GetListOfOnlineFundRequests(FormCollection formCollection)
        {
            long totalRecords;

            try
            {
                if (!objCommon.ValidateGridParameters(new GridParams(Convert.ToInt32(formCollection["page"]), Convert.ToInt32(formCollection["rows"]), formCollection["sidx"], formCollection["sord"], Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                {
                    return null;
                }
                else
                {
                    int State = Convert.ToInt32(formCollection["StateCode"]);
                    int Year = Convert.ToInt32(formCollection["YearCode"]);
                    int Batch = Convert.ToInt32(formCollection["BatchCode"]);
                    int Collaboration = Convert.ToInt32(formCollection["CollaborationCode"]);
                    int Agency = Convert.ToInt32(formCollection["AgencyCode"]);
                    int Scheme = Convert.ToInt32(formCollection["PMGSYScheme"]);

                    var jsonData = new
                    {
                        rows = objProcessBAL.GetListOfOnlineFundRequestsBAL(Convert.ToInt32(formCollection["page"]) - 1, Convert.ToInt32(formCollection["rows"]), formCollection["sidx"], formCollection["sord"], out totalRecords, State,Year,Batch,Collaboration,Agency,Scheme),
                        total = totalRecords <= Convert.ToInt32(formCollection["rows"]) ? 1 : totalRecords / Convert.ToInt32(formCollection["rows"]) + 1,
                        page = Convert.ToInt32(formCollection["page"]),
                        records = totalRecords
                    };
                    return Json(jsonData);
                }
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// view for adding the online fund request.
        /// </summary>
        /// <returns></returns>
        public ActionResult AddOnlineFundRequest()
        {
            try
            {
                OnlineFundRequestViewModel model = new OnlineFundRequestViewModel();
                model.lstBatches = objCommon.PopulateBatch(false);
                model.lstAgencies = objCommon.PopulateAgenciesByStateAndDepartmentwise(PMGSYSession.Current.StateCode,PMGSYSession.Current.AdminNdCode,false);
                model.lstCollaborations = objCommon.PopulateFundingAgency(false);
                model.lstYears = new SelectList(objCommon.PopulateFinancialYear(true).ToList(),"Value","Text").ToList();
                model.Operation = "A";
                return PartialView(model);
            }
            catch (Exception)
            {
                ViewBag.ErrorMessage = "Error occurred while processing your request.";
                return PartialView(new OnlineFundRequestViewModel());
            }
        }

        /// <summary>
        /// saves the details of online fund request.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddOnlineFundRequest(OnlineFundRequestViewModel model)
        {
            string ErrorMessage = string.Empty;
            if (PMGSYSession.Current.StateCode != 0)
            {
                ModelState.Remove("ADMIN_ND_CODE");
            }
            try
            {
                if (ModelState.IsValid)
                {
                    if (objProcessBAL.AddOnlineFundRequestBAL(model,ref ErrorMessage))
                    {
                        return Json(new { Success = true });
                    }
                    else
                    {
                        return Json(new { Success = false , ErrorMessage = ErrorMessage });
                    }
                }
                else
                {
                    return Json(new { Success = false , ErrorMessage = objCommon.FormatErrorMessage(ModelState)});
                }
            }
            catch (Exception)
            {
                return Json(new { Success = false , ErrorMessage = "Error occurred while processing your request."});
            }
        }

        /// <summary>
        /// returns the details for updation
        /// </summary>
        /// <returns></returns>
        public ActionResult EditOnlineFundRequest(string parameter,string hash , string key)
        {
            string[] decryptedParameters = null;
            try
            {
                if (!String.IsNullOrEmpty(parameter) && !String.IsNullOrEmpty(hash) && !String.IsNullOrEmpty(key))
                {
                    decryptedParameters = URLEncrypt.DecryptParameters(new string[] { parameter, hash, key });
                }

                OnlineFundRequestViewModel model = objProcessBAL.GetOnlineFundRequestDetailsBAL(Convert.ToInt32(decryptedParameters[0]));
                if (model != null)
                {
                    model.lstBatches = objCommon.PopulateBatch(false);
                    model.lstAgencies = objCommon.PopulateAgenciesByStateAndDepartmentwise(PMGSYSession.Current.StateCode, PMGSYSession.Current.AdminNdCode, false);
                    model.lstCollaborations = objCommon.PopulateFundingAgency(false);
                    model.lstYears = objCommon.PopulateYears(false);
                    model.Operation = "E";
                    return PartialView("AddOnlineFundRequest", model);
                }
                else
                {
                    return null;
                }
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// updates the details of online fund request.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateOnlineFundRequest(OnlineFundRequestViewModel model)
        {
            string ErrorMessage = string.Empty;
            try
            {
                if (ModelState.IsValid)
                {
                    if (objProcessBAL.UpdateOnlineFundRequestBAL(model, ref ErrorMessage))
                    {
                        return Json(new { Success = true });
                    }
                    else
                    {
                        return Json(new { Success = false, ErrorMessage = ErrorMessage });
                    }
                }
                else
                {
                    return Json(new { Success = false, ErrorMessage = objCommon.FormatErrorMessage(ModelState) });
                }
            }
            catch (Exception)
            {
                return Json(new { Success = false, ErrorMessage = "Error occurred while processing your request." });
            }
        }

        /// <summary>
        /// deletes the entry of online fund request.
        /// </summary>
        /// <returns></returns>
        public ActionResult DeleteOnlineFundRequest(String parameter, String hash, String key)
        {
            string[] decryptedParameters = null;
            try
            {
                if (!String.IsNullOrEmpty(parameter) && !String.IsNullOrEmpty(hash) && !String.IsNullOrEmpty(key))
                {
                    decryptedParameters = URLEncrypt.DecryptParameters(new string[] { parameter , hash , key});
                }

                if (objProcessBAL.DeleteOnlineFundRequestBAL(Convert.ToInt32(decryptedParameters[0])))
                {
                    return Json(new { Success = true });
                }
                else
                {
                    return Json(new { Success = false , ErrorMessage = "Error occurred while processing your request." });
                }
            }
            catch (Exception)
            {
                return Json(new { Success = false , ErrorMessage = "Error occurred while processing your request."});
            }
        }

        /// <summary>
        /// returns the display of fund request.
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="hash"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public ActionResult ViewFundRequest(String parameter, String hash, String key)
        {
            string[] decryptedParameters = null;
            try
            {
                if (!String.IsNullOrEmpty(parameter) && !String.IsNullOrEmpty(hash) && !String.IsNullOrEmpty(key))
                {
                    decryptedParameters = URLEncrypt.DecryptParameters(new string[] { parameter, hash, key });
                }

                OnlineFundRequestViewModel model = objProcessBAL.GetOnlineFundRequestDetailsBAL(Convert.ToInt32(decryptedParameters[0]));
                TotalFundDetails fundDetails = objProcessDAL.GetTotalDetailsByRequestId(Convert.ToInt32(decryptedParameters[0]));
                ViewBag.FundDetails = fundDetails;
                return PartialView(model);
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// finalizes the request details
        /// </summary>
        /// <returns></returns>
        public ActionResult FinalizeRequestDetails(String parameter, String hash, String key)
        {
            string[] decryptedParameters = null;
            string message = string.Empty;
            try
            {
                if (!String.IsNullOrEmpty(parameter) && !String.IsNullOrEmpty(hash) && !String.IsNullOrEmpty(key))
                {
                    decryptedParameters = URLEncrypt.DecryptParameters(new string[] { parameter, hash, key });
                }

                if(objProcessBAL.FinalizeRequestDetailsBAL(Convert.ToInt32(decryptedParameters[0]),out message))
                {
                    return Json(new { Success = true });
                }
                else
                {
                    return Json(new { Success = false , ErrorMessage = message });
                }
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// views the proposal details
        /// </summary>
        /// <returns></returns>
        public ActionResult ViewProposalDetails(String parameter, String hash, String key)
        {
            string[] decryptedParameters = null;

            try
            {
                if (!String.IsNullOrEmpty(parameter) && !String.IsNullOrEmpty(hash) && !String.IsNullOrEmpty(key))
                {
                    decryptedParameters = URLEncrypt.DecryptParameters(new string[] { parameter, hash, key });
                    ViewBag.EncryptedParams = parameter + "/" + hash + "/" + key;
                    return PartialView();
                }
                else
                {
                    return null;
                }
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// returns the list of proposal details
        /// </summary>
        /// <returns></returns>
        public ActionResult GetProposalList(FormCollection formCollection)
        {
            long totalRecords;
            int requestId = 0;
            try
            {
                if (!objCommon.ValidateGridParameters(new GridParams(Convert.ToInt32(formCollection["page"]), Convert.ToInt32(formCollection["rows"]), formCollection["sidx"], formCollection["sord"], Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                {
                    return null;
                }
                else
                {
                    string EncryptedId = formCollection["EncryptedId"];
                    if(!String.IsNullOrEmpty(EncryptedId))
                    {
                        string[] decryptedParams = URLEncrypt.DecryptParameters(new string[]{ EncryptedId.Split('/')[0] , EncryptedId.Split('/')[1] , EncryptedId.Split('/')[2]});
                        requestId = Convert.ToInt32(decryptedParams[0]);
                    }

                    var jsonData = new
                    {
                        rows = objProcessBAL.GetProposalListBAL(Convert.ToInt32(formCollection["page"]) - 1, Convert.ToInt32(formCollection["rows"]), formCollection["sidx"], formCollection["sord"], out totalRecords, requestId),
                        total = totalRecords <= Convert.ToInt32(formCollection["rows"]) ? 1 : totalRecords / Convert.ToInt32(formCollection["rows"]) + 1,
                        page = Convert.ToInt32(formCollection["page"]),
                        records = totalRecords
                    };
                    return Json(jsonData);
                }
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// returns the view for uploading
        /// </summary>
        /// <returns></returns>
        public ActionResult UploadDetails(String parameter, String hash, String key)
        {
            try
            {
                DocumentUploadViewModel model = new DocumentUploadViewModel();
                model.EncryptedRequestId = parameter + "/" + hash + "/" + key;
                model.lstDocuments = objProcessDAL.GetDocumentListByRequestId(model.EncryptedRequestId);
                model.DocumentBefore = objProcessDAL.GetDocumentNeeded(model.EncryptedRequestId);
                return PartialView(model);
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// saves the uploaded documents
        /// </summary>
        /// <returns></returns>
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult UploadDocuments(FormCollection formCollection)
        {
            try
            {
                int count = Request.Files.Count;
                List<HttpPostedFileBase> lstFiles = new List<HttpPostedFileBase>();
                List<DocumentUploadViewModel> lstModels = new List<DocumentUploadViewModel>();
                List<SelectListItem> lstRequiredDocuments = objProcessDAL.GetDocumentListByRequestId(formCollection["EncryptedRequestId"]);
                foreach (var key in lstRequiredDocuments.Select(m=>m.Text.Replace(' ','_')).ToList())
                {

                    if (Request.Files.AllKeys.Count() == 0)
                    {
                        ModelState.AddModelError("","Please upload at least one file.");
                        return Json(new { Success = false, ErrorMessage = objCommon.FormatErrorMessage(ModelState) });
                    }
                    //if (!Request.Files.AllKeys.Contains(key))
                    //{
                    //    ModelState.AddModelError("", "Please upload " + key);
                    //}
                    //else
                    //{
                    //}
                    if (Request.Files.AllKeys.Contains(key))
                    {
                        lstFiles.Add(Request.Files[key]);
                        if (string.IsNullOrEmpty(formCollection["Remarks" + key]))
                        {
                            ModelState.AddModelError("", "Please enter remarks for " + key.Replace('_', ' '));
                        }
                    }

                    
                }

                if (!ModelState.IsValid)
                {
                    return Json(new { Success = false, ErrorMessage = objCommon.FormatErrorMessage(ModelState) });
                }
                else
                {
                    foreach (var key in lstRequiredDocuments.Select(m => m.Text.Replace(' ', '_')).ToList())
                    {
                        HttpPostedFileBase file = Request.Files[key];
                        if (file != null)
                        {
                            int documentId = Convert.ToInt32(formCollection["ddl" + key]);
                            string fileTypes = string.Empty;

                            bool fileExt = false;
                            bool isValidFile = false;
                            string fileId = string.Empty;
                            string filePath = string.Empty;
                            string fileSaveExt = string.Empty;
                            switch (documentId)
                            {
                                case 1:
                                    fileTypes = ConfigurationManager.AppSettings["OFP_FILE_UPLOAD_BALANCE_SHEET_FORMAT"];
                                    filePath = ConfigurationManager.AppSettings["OFP_FILE_UPLOAD_BALANCE_SHEET"];
                                    fileId = "Balance Sheet";
                                    break;
                                case 2:
                                    fileTypes = ConfigurationManager.AppSettings["OFP_FILE_UPLOAD_MPR_FORMAT"];
                                    filePath = ConfigurationManager.AppSettings["OFP_FILE_UPLOAD_MPR"];
                                    fileId = "MPR";
                                    break;
                                case 3:
                                    fileTypes = ConfigurationManager.AppSettings["OFP_FILE_UPLOAD_SANCTION_LETTER_FORMAT"];
                                    filePath = ConfigurationManager.AppSettings["OFP_FILE_UPLOAD_SANCTION_LETTER"];
                                    fileId = "Sanction Letter";
                                    break;
                                case 4:
                                    fileTypes = ConfigurationManager.AppSettings["OFP_FILE_UPLOAD_BANKER_CERTIFICATE_OB_FORMAT"];
                                    filePath = ConfigurationManager.AppSettings["OFP_FILE_UPLOAD_BANKER_CERTIFICATE_OB"];
                                    fileId = "Banker Certificate for opening Balance";
                                    break;
                                case 5:
                                    fileTypes = ConfigurationManager.AppSettings["OFP_FILE_UPLOAD_UTILIZATION_CERTIFICATE_FORMAT"];
                                    filePath = ConfigurationManager.AppSettings["OFP_FILE_UPLOAD_UTILIZATION_CERTIFICATE"];
                                    fileId = "Utilization Certificate";
                                    break;
                                case 6:
                                    fileTypes = ConfigurationManager.AppSettings["OFP_FILE_UPLOAD_BANK_RECONCILITATION_OB_FORMAT"];
                                    filePath = ConfigurationManager.AppSettings["OFP_FILE_UPLOAD_BANK_RECONCILITATION_OB"];
                                    fileId = "Bank Reconcilitation for opening balance";
                                    break;
                                case 7:
                                    fileTypes = ConfigurationManager.AppSettings["OFP_FILE_UPLOAD_BANK_RECONCILITATION_CB_FORMAT"];
                                    filePath = ConfigurationManager.AppSettings["OFP_FILE_UPLOAD_BANK_RECONCILITATION_CB"];
                                    fileId = "Banker Reconcilitation for closing balance";
                                    break;
                                case 8:
                                    fileTypes = ConfigurationManager.AppSettings["OFP_FILE_UPLOAD_BANKER_CERTIFICATE_CB_FORMAT"];
                                    filePath = ConfigurationManager.AppSettings["OFP_FILE_UPLOAD_BANKER_CERTIFICATE_CB"];
                                    fileId = "Banker Certificate for closing balance";
                                    break;
                                default:
                                    break;
                            }

                            string[] allowedFileTypes = fileTypes.Split('$');

                            foreach (var item in allowedFileTypes)
                            {
                                if (item == file.FileName.Split('.')[1])
                                {
                                    fileExt = true;
                                    fileSaveExt = item;
                                    break;
                                }
                            }
                            if (fileExt == true)
                            {
                                switch (fileSaveExt)
                                {
                                    case "pdf":
                                        isValidFile = objCommon.ValidateIsPdfNew(filePath, file);
                                        break;
                                    case "doc":
                                        isValidFile = objCommon.ValidateIsWordNew(filePath, file);
                                        break;
                                    case "docx":
                                        isValidFile = objCommon.ValidateIsWordNew(filePath, file);
                                        break;
                                    case "xls":
                                        isValidFile = objCommon.ValidateIsExcel(filePath, file, ".xls");
                                        break;
                                    case "xlsx":
                                        isValidFile = objCommon.ValidateIsExcel(filePath, file, ".xlsx");
                                        break;
                                }
                            }
                            else
                            {
                                ModelState.AddModelError("", "File type is not allowed for " + fileId);
                            }

                            if (ModelState.IsValid && isValidFile == true)
                            {
                                DocumentUploadViewModel model = new DocumentUploadViewModel();
                                model.EncryptedRequestId = formCollection["EncryptedRequestId"];
                                model.DOCUMENT_ID = Convert.ToInt32(formCollection["ddl" + key]);
                                model.fileInfo = Request.Files[key];
                                model.REMARKS = formCollection["Remarks" + key];
                                lstModels.Add(model);
                            }
                            else
                            {
                                return Json(new { Success = false, ErrorMessage = objCommon.FormatErrorMessage(ModelState) });
                            }
                        }
                    }
                    if (objProcessBAL.AddDocumentDetailsBAL(lstModels))
                    {
                        return Json(new { Success = true });
                    }
                    else
                    {
                        return Json(new { Success = false , ErrorMessage = "Error occurred while processing your request."});
                    }
                }
            }
            catch (Exception)
            {
                return Json(new { Success = false , ErrorMessage = "Error occurred while processing your request."});
            }
        }

        /// <summary>
        /// request processing by different users
        /// </summary>
        /// <returns></returns>
        public ActionResult AddObservationDetails(string parameter, string hash, string key)
        {
            RequestApprovalViewModel model = new RequestApprovalViewModel();
            try
            {
                model.REQUEST_ID = parameter + "/" + hash + "/" + key;
                model.FILE_NO = objProcessDAL.GetRequestFileNumber(model.REQUEST_ID);
                model.lstRequestTo = objProcessDAL.PopulateForwardRequestUsers(model.REQUEST_ID);
                ViewBag.IsObservationDone = objProcessDAL.IsObservationDone(model.REQUEST_ID);
                model.lstConditions = objProcessDAL.PopulateConditions();
                return PartialView(model);
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// saves the observation details
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult AddObservationDetails(FormCollection frmCollection)
        {
            bool fileExt = false;
            string fileSaveExt = string.Empty;
            bool isValidFile = false;
            string filePath = string.Empty;
            string fileName = string.Empty;

            try
            {
                if (PMGSYSession.Current.RoleCode == 52)
                {
                    ModelState.Remove("REQUEST_FORWADED_TO");
                }

                if (frmCollection["APPROVAL_STATUS"] == "R")
                {
                    if (Request.Files.AllKeys.Count() == 0)
                    {
                        return Json(new { Success = false, ErrorMessage = "Please upload Reject Letter." });
                    }
                    else
                    {
                        fileSaveExt = Request.Files["rejectLetter"].FileName.Split('.')[1];
                    }
                }

                RequestApprovalViewModel model = new RequestApprovalViewModel();
                model.APPROVAL_STATUS = frmCollection["APPROVAL_STATUS"];
                model.CONDITION_IMPOSED = frmCollection["CONDITION_IMPOSED"];
                model.ConditionCode = Convert.ToInt32(frmCollection["ConditionCode"]);
                model.FILE_NO = frmCollection["FILE_NO"];
                model.REMARKS = frmCollection["REMARKS"];
                model.REQUEST_FORWADED_TO = Convert.ToInt32(frmCollection["REQUEST_FORWADED_TO"]);
                model.REQUEST_ID = frmCollection["REQUEST_ID"];
                model.RejectLetterName = objProcessDAL.GetRejectLetterFileName(model.REQUEST_ID) + "." + fileSaveExt ;
               
                if (ModelState.IsValid)
                {
                    if (objProcessBAL.AddObservationDetailsBAL(model))
                    {
                        if (model.APPROVAL_STATUS == "R")
                        {
                            HttpPostedFileBase file = Request.Files["rejectLetter"];
                            string fileTypes = ConfigurationManager.AppSettings["OFP_FILE_UPLOAD_REJECT_LETTER_FORMAT"];

                            string[] allowedFileTypes = fileTypes.Split('$');

                            foreach (var item in allowedFileTypes)
                            {
                                if (item == file.FileName.Split('.')[1])
                                {
                                    fileExt = true;
                                    fileSaveExt = item;
                                    break;
                                }
                            }
                            if (fileExt == true)
                            {
                                switch (fileSaveExt)
                                {
                                    case "pdf":
                                        isValidFile = objCommon.ValidateIsPdf(filePath, Request);
                                        break;
                                    case "doc":
                                        isValidFile = objCommon.ValidateIsWord(filePath, Request);
                                        break;
                                    case "docx":
                                        isValidFile = objCommon.ValidateIsWord(filePath, Request);
                                        break;
                                }
                            }
                            else
                            {
                                ModelState.AddModelError("", "File type is not allowed");
                            }

                            filePath = ConfigurationManager.AppSettings["OFP_FILE_UPLOAD_REJECT_LETTER"];
                            fileName = objProcessDAL.GetRejectLetterFileName(model.REQUEST_ID);
                            file.SaveAs(Path.Combine(filePath, fileName + "." + file.FileName.Split('.')[1]));
                        }

                        return Json(new { Success = true });
                    }
                    else
                    {
                        return Json(new { Success = false, ErrorMessage = "Error occurred while processing your request." });
                    }
                }
                else
                {
                    return Json(new { Success = false, ErrorMessage = objCommon.FormatErrorMessage(ModelState) });
                }
            }
            catch (Exception)
            {
                return Json(new { Success = false , ErrorMessage = "Error occurred while processing your request."});
            }
        }

        /// <summary>
        /// returns the list of documents uploaded.
        /// </summary>
        /// <returns></returns>
        public ActionResult GetListofDocumentsUploaded(FormCollection formCollection)
        {
            long totalRecords;

            try
            {
                if (!objCommon.ValidateGridParameters(new GridParams(Convert.ToInt32(formCollection["page"]), Convert.ToInt32(formCollection["rows"]), formCollection["sidx"], formCollection["sord"], Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                {
                    return null;
                }
                else
                {
                    string[] encryptedCode = URLEncrypt.DecryptParameters(new string[] { formCollection["EncryptedRequestId"].Split('/')[0], formCollection["EncryptedRequestId"].Split('/')[1], formCollection["EncryptedRequestId"].Split('/')[2] });
                    int reuestCode = Convert.ToInt32(encryptedCode[0]);
                    var jsonData = new
                    {
                        rows = objProcessBAL.GetListOfDocumentsUploadedBAL(Convert.ToInt32(formCollection["page"]) - 1, Convert.ToInt32(formCollection["rows"]), formCollection["sidx"], formCollection["sord"], out totalRecords, reuestCode),
                        total = totalRecords <= Convert.ToInt32(formCollection["rows"]) ? 1 : totalRecords / Convert.ToInt32(formCollection["rows"]) + 1,
                        page = Convert.ToInt32(formCollection["page"]),
                        records = totalRecords
                    };
                    return Json(jsonData);
                }
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// returns the file details
        /// </summary>
        /// <returns></returns>
        public ActionResult DownloadFile(string parameter, string hash , string key)
        {
            string FileName = string.Empty;
            string FullFileLogicalPath = string.Empty;
            string FullfilePhysicalPath = string.Empty;
            string FileExtension = string.Empty;
            String[] urlParams = null;
            String[] urlSplitParams = null;
            try
            {
                if (!String.IsNullOrEmpty(parameter) && !String.IsNullOrEmpty(hash) && !String.IsNullOrEmpty(key))
                {
                    urlParams = URLEncrypt.DecryptParameters(new String[] { parameter, hash, key });
                    if (urlParams.Length >= 1)
                    {
                        urlSplitParams = urlParams[0].Split('$');
                        FileName = (urlSplitParams[2]);
                    }
                }

                FileExtension = Path.GetExtension(FileName).ToLower();

                switch (Convert.ToInt32(urlSplitParams[1]))
                {
                    case 1:
                        FullfilePhysicalPath = Path.Combine(ConfigurationManager.AppSettings["OFP_FILE_UPLOAD_BALANCE_SHEET"],FileName);
                        break;
                    case 2:
                        FullfilePhysicalPath = Path.Combine(ConfigurationManager.AppSettings["OFP_FILE_UPLOAD_MPR"],FileName);
                        break;
                    case 3:
                        FullfilePhysicalPath = Path.Combine(ConfigurationManager.AppSettings["OFP_FILE_UPLOAD_SANCTION_LETTER"],FileName);
                        break;
                    case 4:
                        FullfilePhysicalPath = Path.Combine(ConfigurationManager.AppSettings["OFP_FILE_UPLOAD_BANKER_CERTIFICATE_OB"],FileName);
                        break;
                    case 5:
                        FullfilePhysicalPath = Path.Combine(ConfigurationManager.AppSettings["OFP_FILE_UPLOAD_UTILIZATION_CERTIFICATE"],FileName);
                        break;
                    case 6:
                        FullfilePhysicalPath = Path.Combine(ConfigurationManager.AppSettings["OFP_FILE_UPLOAD_BANK_RECONCILITATION_OB"],FileName);
                        break;
                    case 7:
                        FullfilePhysicalPath = Path.Combine(ConfigurationManager.AppSettings["OFP_FILE_UPLOAD_BANKER_CERTIFICATE_CB"],FileName);
                        break;
                    case 8:
                        FullfilePhysicalPath = Path.Combine(ConfigurationManager.AppSettings["OFP_FILE_UPLOAD_BANKER_CERTIFICATE_CB"], FileName);
                        break;
                    default:
                        break;
                }

                string ext = Path.GetExtension(FileName);

                string type = string.Empty;

                if (ext != null)
                {
                    switch (ext.ToLower())
                    {
                        case ".pdf":
                            type = "Application/pdf";
                            break;
                        case ".doc":
                        case ".docx":
                            type = "Application/msword";
                            break;
                        case ".jpg":
                        case ".xls":
                            type = "Application/msexcel";
                            break;
                        case ".xlsx":
                            type = "Application/msexcel";
                            break;
                        case ".bmp":
                        case ".tiff":
                        case ".png":
                        case ".gif":
                        case ".jpeg":
                            type = "image/png";
                            break;
                        default:
                            type = "Application";
                            break;
                    }
                }

                if (System.IO.File.Exists(FullfilePhysicalPath))
                {
                    return File(FullfilePhysicalPath, type, DateTime.Now.ToShortDateString() + "_" + new Random().Next(1000000000) + FileExtension);
                }
                else
                {
                    return Json(new { Success = "false", ErrorMessage = "File Not Found." }, JsonRequestBehavior.AllowGet);
                }

            }
            catch (Exception)
            {
                return Json(new { Success = "false", ErrorMessage = "Error While Processing Your Request." }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// returns the file details
        /// </summary>
        /// <returns></returns>
        public ActionResult DownloadRejectLetterFile(string parameter, string hash, string key)
        {
            string FileName = string.Empty;
            string FullFileLogicalPath = string.Empty;
            string FullfilePhysicalPath = string.Empty;
            string FileExtension = string.Empty;
            String[] urlParams = null;
            String[] urlSplitParams = null;
            try
            {
                if (!String.IsNullOrEmpty(parameter) && !String.IsNullOrEmpty(hash) && !String.IsNullOrEmpty(key))
                {
                    urlParams = URLEncrypt.DecryptParameters(new String[] { parameter, hash, key });
                    if (urlParams.Length >= 1)
                    {
                        urlSplitParams = urlParams[0].Split('$');
                        FileName = (urlSplitParams[1]);
                    }
                }

                FileExtension = Path.GetExtension(FileName).ToLower();
               FullfilePhysicalPath = Path.Combine(ConfigurationManager.AppSettings["OFP_FILE_UPLOAD_REJECT_LETTER"], FileName);

                string ext = Path.GetExtension(FileName);

                string type = string.Empty;

                if (ext != null)
                {
                    switch (ext.ToLower())
                    {
                        case ".pdf":
                            type = "Application/pdf";
                            break;
                        case ".doc":
                        case ".docx":
                            type = "Application/msword";
                            break;
                        case ".jpg":
                        case ".xls":
                            type = "Application/msexcel";
                            break;
                        case ".xlsx":
                            type = "Application/msexcel";
                            break;
                        case ".bmp":
                        case ".tiff":
                        case ".png":
                        case ".gif":
                        case ".jpeg":
                            type = "image/png";
                            break;
                        default:
                            type = "Application";
                            break;
                    }
                }

                if (System.IO.File.Exists(FullfilePhysicalPath))
                {
                    return File(FullfilePhysicalPath, type, DateTime.Now.ToShortDateString() + "_" + new Random().Next(1000000000) + FileExtension);
                }
                else
                {
                    return Json(new { Success = "false", ErrorMessage = "File Not Found." }, JsonRequestBehavior.AllowGet);
                }

            }
            catch (Exception)
            {
                return Json(new { Success = "false", ErrorMessage = "Error While Processing Your Request." }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// returns the list of observations given.
        /// </summary>
        /// <returns></returns>
        public ActionResult GetListofObservationDetails(FormCollection formCollection)
        {
            long totalRecords;

            try
            {
                if (!objCommon.ValidateGridParameters(new GridParams(Convert.ToInt32(formCollection["page"]), Convert.ToInt32(formCollection["rows"]), formCollection["sidx"], formCollection["sord"], Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                {
                    return null;
                }
                else
                {
                    string[] encryptedCode = URLEncrypt.DecryptParameters(new string[] { formCollection["RequestId"].Split('/')[0], formCollection["RequestId"].Split('/')[1], formCollection["RequestId"].Split('/')[2] });
                    int reuestCode = Convert.ToInt32(encryptedCode[0]);
                    var jsonData = new
                    {
                        rows = objProcessBAL.GetListofObservationDetailsBAL(Convert.ToInt32(formCollection["page"]) - 1, Convert.ToInt32(formCollection["rows"]), formCollection["sidx"], formCollection["sord"], out totalRecords, reuestCode),
                        total = totalRecords <= Convert.ToInt32(formCollection["rows"]) ? 1 : totalRecords / Convert.ToInt32(formCollection["rows"]) + 1,
                        page = Convert.ToInt32(formCollection["page"]),
                        records = totalRecords
                    };
                    return Json(jsonData);
                }
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// returns the list of agencies according to the state
        /// </summary>
        /// <returns></returns>
        public ActionResult GetAgenciesByState(int stateCode)
        {
            try
            {
                return Json(objCommon.PopulateAgencies(stateCode, false));
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// returns the details according to the parameters
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult GetTotalDetailsOfState(string id)
        {
            TotalFundDetails fundDetails = new TotalFundDetails();

            try
            {
                if (!String.IsNullOrEmpty(id))
                {
                    int year = Convert.ToInt32(id.Split('$')[0]);
                    int batch = Convert.ToInt32(id.Split('$')[1]);
                    int collaboration = Convert.ToInt32(id.Split('$')[2]);
                    int scheme = Convert.ToInt32(id.Split('$')[3]);

                    if (objProcessDAL.GetTotalDetails(year, batch, collaboration, scheme, out fundDetails))
                    {
                        return Json(new { Success = true, ProposalDetails = fundDetails });
                    }
                    else
                    {
                        return Json(new { Success = false, ErrorMessage = "Error occurred while processing your request." });
                    }
                }
                else
                {
                    return Json(new { Success = false , ErrorMessage = "Error occurred while processing your request."});
                }
            }
            catch (Exception)
            {
                return Json(new { Success = false, ErrorMessage = "Error occurred while processing your request." });
            }
        }

        /// <summary>
        /// returns the list of request for which the action is required
        /// </summary>
        /// <param name="formCollection"></param>
        /// <returns></returns>
        public ActionResult GetActionRequiredRequestList(FormCollection formCollection)
        {
            long totalRecords;

            try
            {
                if (!objCommon.ValidateGridParameters(new GridParams(Convert.ToInt32(formCollection["page"]), Convert.ToInt32(formCollection["rows"]), formCollection["sidx"], formCollection["sord"], Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                {
                    return null;
                }
                else
                {
                    var jsonData = new
                    {
                        rows = objProcessBAL.GetActionRequiredRequestListBAL(Convert.ToInt32(formCollection["page"]) - 1, Convert.ToInt32(formCollection["rows"]), formCollection["sidx"], formCollection["sord"], out totalRecords),
                        total = totalRecords <= Convert.ToInt32(formCollection["rows"]) ? 1 : totalRecords / Convert.ToInt32(formCollection["rows"]) + 1,
                        page = Convert.ToInt32(formCollection["page"]),
                        records = totalRecords
                    };
                    return Json(jsonData);
                }
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// returns the list of in progress requests
        /// </summary>
        /// <param name="formCollection"></param>
        /// <returns></returns>
        public ActionResult GetInProgressRequestList(FormCollection formCollection)
        {
            long totalRecords;

            try
            {
                if (!objCommon.ValidateGridParameters(new GridParams(Convert.ToInt32(formCollection["page"]), Convert.ToInt32(formCollection["rows"]), formCollection["sidx"], formCollection["sord"], Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                {
                    return null;
                }
                else
                {
                    var jsonData = new
                    {
                        rows = objProcessBAL.GetInProgressRequestListBAL(Convert.ToInt32(formCollection["page"]) - 1, Convert.ToInt32(formCollection["rows"]), formCollection["sidx"], formCollection["sord"], out totalRecords),
                        total = totalRecords <= Convert.ToInt32(formCollection["rows"]) ? 1 : totalRecords / Convert.ToInt32(formCollection["rows"]) + 1,
                        page = Convert.ToInt32(formCollection["page"]),
                        records = totalRecords
                    };
                    return Json(jsonData);
                }
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// returns the list of completed requests
        /// </summary>
        /// <param name="formCollection"></param>
        /// <returns></returns>
        public ActionResult GetCompletedRequestList(FormCollection formCollection)
        {
            long totalRecords;

            try
            {
                if (!objCommon.ValidateGridParameters(new GridParams(Convert.ToInt32(formCollection["page"]), Convert.ToInt32(formCollection["rows"]), formCollection["sidx"], formCollection["sord"], Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                {
                    return null;
                }
                else
                {
                    var jsonData = new
                    {
                        rows = objProcessBAL.GetCompletedRequestListBAL(Convert.ToInt32(formCollection["page"]) - 1, Convert.ToInt32(formCollection["rows"]), formCollection["sidx"], formCollection["sord"], out totalRecords),
                        total = totalRecords <= Convert.ToInt32(formCollection["rows"]) ? 1 : totalRecords / Convert.ToInt32(formCollection["rows"]) + 1,
                        page = Convert.ToInt32(formCollection["page"]),
                        records = totalRecords
                    };
                    return Json(jsonData);
                }
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// returns the list of requests for adding UO No.
        /// </summary>
        /// <param name="formCollection"></param>
        /// <returns></returns>
        public ActionResult GetApprovedRequestList(FormCollection formCollection)
        {
            long totalRecords;

            try
            {
                if (!objCommon.ValidateGridParameters(new GridParams(Convert.ToInt32(formCollection["page"]), Convert.ToInt32(formCollection["rows"]), formCollection["sidx"], formCollection["sord"], Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                {
                    return null;
                }
                else
                {
                    var jsonData = new
                    {
                        rows = objProcessBAL.GetApprovedRequestListBAL(Convert.ToInt32(formCollection["page"]) - 1, Convert.ToInt32(formCollection["rows"]), formCollection["sidx"], formCollection["sord"], out totalRecords),
                        total = totalRecords <= Convert.ToInt32(formCollection["rows"]) ? 1 : totalRecords / Convert.ToInt32(formCollection["rows"]) + 1,
                        page = Convert.ToInt32(formCollection["page"]),
                        records = totalRecords
                    };
                    return Json(jsonData);
                }
            }
            catch (Exception)
            {
                return null;
            }
        }


        /// <summary>
        /// returns the view for showing the request details.
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="hash"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public ActionResult ViewRequestDetails(string parameter, string hash, string key)
        {
            string[] decryptedParameters = null;
            try
            {
                PMGSY.Models.PMGSYEntities db = new Models.PMGSYEntities();
                ViewBag.EncryptedId = parameter + "/" + hash + "/" + key;
                if (!String.IsNullOrEmpty(parameter) && !String.IsNullOrEmpty(hash) && !String.IsNullOrEmpty(key))
                {
                    decryptedParameters = URLEncrypt.DecryptParameters(new string[] { parameter, hash, key });
                }
                int requestCode = Convert.ToInt32(decryptedParameters[0]);
                PMGSY.Models.OFP_REQUEST_MASTER requestDetails = db.OFP_REQUEST_MASTER.Find(requestCode);
                ViewBag.State = requestDetails.MASTER_STATE.MAST_STATE_NAME;
                ViewBag.Agency = requestDetails.ADMIN_DEPARTMENT.ADMIN_ND_NAME;
                ViewBag.Installment = requestDetails.REQUEST_INSTALLMENT;
                ViewBag.Year = requestDetails.MASTER_YEAR.MAST_YEAR_TEXT;
                ViewBag.Batch = requestDetails.MASTER_BATCH.MAST_BATCH_NAME;
                ViewBag.Collaboration = requestDetails.MASTER_STREAMS.MAST_STREAM_NAME;
                return View();
            }
            catch (Exception)
            {
                return null;
            }
            
        }

        /// <summary>
        /// returns the view for request details
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="hash"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public ActionResult ViewObservationDetails(string parameter, string hash, string key)
        {
            string[] decryptedParameters = null;
            try
            {
                if (!String.IsNullOrEmpty(parameter) && !String.IsNullOrEmpty(hash) && !String.IsNullOrEmpty(key))
                {
                    decryptedParameters = URLEncrypt.DecryptParameters(new string[] { parameter, hash, key });
                }

                OnlineFundRequestViewModel model = objProcessBAL.GetOnlineFundRequestDetailsBAL(Convert.ToInt32(decryptedParameters[0]));
                TotalFundDetails fundDetails = objProcessDAL.GetTotalDetailsByRequestId(Convert.ToInt32(decryptedParameters[0]));
                ViewBag.FundDetails = fundDetails;
                return PartialView(model);
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// returns whether the condition is imposed on the srrda
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult IsConditionImposed(string id)
        {
            try
            {
                if (objProcessDAL.IsConditionImposedDAL(id))
                {
                    return Json(new { Success = true });
                }
                else 
                {
                    return Json(new { Success = false });
                }
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// returns the view for adding the reply of previously added condition
        /// </summary>
        /// <returns></returns>
        public ActionResult AddConditionReply(string parameter, string hash, string key)
        {
            string[] decryptedParameters = null;
            try
            {
                ConditionReplyViewModel model = new ConditionReplyViewModel();
                if (!String.IsNullOrEmpty(parameter) && !String.IsNullOrEmpty(hash) && !String.IsNullOrEmpty(key))
                {
                    decryptedParameters = URLEncrypt.DecryptParameters(new string[] { parameter, hash, key });
                }
                else
                {
                    return null;
                }
                model.REQUEST_ID = Convert.ToInt32(decryptedParameters[0]);
                return PartialView(model);
            }
            catch (Exception)
            {
                return null;
            }
        }
        
        /// <summary>
        /// saves the details of reply for the condition imposed
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult AddConditionReply(ConditionReplyViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (objProcessBAL.AddConditionReplyBAL(model))
                    {
                        return Json(new { Success = true});
                    }
                    else
                    {
                        return Json(new { Success = false ,  ErrorMessage = "Error occurred while processing your request." });
                    }
                }
                else
                {
                    return Json(new { Success = false , ErrorMessage = objCommon.FormatErrorMessage(ModelState)});
                }
            }
            catch (Exception)
            {
                return Json(new { Success = false, ErrorMessage = "Error occurred while processing your request." });
            }
        }

        /// <summary>
        /// returns the view for showing the request details 
        /// </summary>
        /// <returns></returns>
        public ActionResult ViewRequestStatusDetails()
        {
            try
            {
                ViewBag.ListRequests = objProcessDAL.GetRequestDetails();
                return PartialView();
            }
            catch (Exception)
            {
                return View();
            }
        }

        /// <summary>
        /// populates the batches according to the Year 
        /// </summary>
        /// <returns></returns>
        public ActionResult PopulateBatchByYear(int yearCode)
        {
            try
            {
                return Json(objProcessDAL.PopulateBatchByYear(yearCode));
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// regenerates the request details
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="hash"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public ActionResult RegenerateRequestDetails(string parameter, string hash, string key)
        {
            try
            {
                string[] decryptedParameters = null;
                if (!String.IsNullOrEmpty(parameter) && !String.IsNullOrEmpty(hash) && !String.IsNullOrEmpty(key))
                {
                    decryptedParameters = URLEncrypt.DecryptParameters(new string[] { parameter, hash, key });
                }

                if (objProcessDAL.RegenerateRequestDetails(Convert.ToInt32(decryptedParameters[0])))
                {
                    return Json(new { Success = true });
                }
                else
                {
                    return Json(new { Success = false, Message = "Error occurred while processing your request." });
                }
            }
            catch (Exception)
            {
                return Json(new { Success = false, Message = "Error occurred while processing your request." });
            }
        }

        /// <summary>
        /// returns the view for showing the complete request details
        /// </summary>
        /// <returns></returns>
        public ActionResult CompleteRequestDetails(string parameter, string hash, string key)
        {
            try
            {
                string[] decryptedParameters = null;
                if (!String.IsNullOrEmpty(parameter) && !String.IsNullOrEmpty(hash) && !String.IsNullOrEmpty(key))
                {
                    decryptedParameters = URLEncrypt.DecryptParameters(new string[] { parameter, hash, key });
                }
                ViewBag.RequestId = parameter + "/" + hash + "/" + key;
                ViewBag.IsReleaseNoGenerated = objProcessDAL.IsReleaseNoGenerated(Convert.ToInt32(decryptedParameters[0]));
                return View();
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// returns the view for adding the UO No and observation details
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="hash"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public ActionResult AddUODetails(string parameter, string hash, string key)
        {
            string[] decryptedParameters = null;
            OnlineFundRequestViewModel model = new OnlineFundRequestViewModel();
            try
            {
                if (!String.IsNullOrEmpty(parameter) && !String.IsNullOrEmpty(hash) && !String.IsNullOrEmpty(key))
                {
                    decryptedParameters = URLEncrypt.DecryptParameters(new string[] { parameter, hash, key });
                }
                else
                {
                    return null;
                }
                model = objProcessDAL.GetOnlineFundRequestDetailsDAL(Convert.ToInt32(decryptedParameters[0]));
                model.EncRequestId = parameter + "/" + hash + "/" + key;
                return PartialView(model);
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// saves the details of UO and observation
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult AddUODetails(OnlineFundRequestViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (objProcessBAL.AddUODetailsBAL(model))
                    {
                        return Json(new { Success = true });
                    }
                    else
                    {
                        return Json(new { Success = false, ErrorMessage = objCommon.FormatErrorMessage(ModelState) });
                    }
                }
                else
                {
                    return Json(new { Success = false, ErrorMessage = "Error occurred while processing your request." });
                }
            }
            catch (Exception)
            {
                return Json(new { Success = false , ErrorMessage = "Error occurred while processing your request." });
            }
        }

        /// <summary>
        /// delete the document details
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult DeleteFileDetails(string id)
        {
            try
            {
                String PhysicalPath = string.Empty;
                string[] decryptedParams = URLEncrypt.DecryptParameters(new string[] { id.Split('/')[0], id.Split('/')[1], id.Split('/')[2] });
                int requestDocumentId = 0;
                int documentId = 0;
                if (!String.IsNullOrEmpty(id))
                {
                    requestDocumentId = Convert.ToInt32(decryptedParams[0].Split('$')[0]);
                    documentId = Convert.ToInt32(decryptedParams[0].Split('$')[1]);
                }

                string FILE_NAME = decryptedParams[0].Split('$')[2];
                
                switch (documentId)
                {
                    case 1:
                        PhysicalPath = ConfigurationManager.AppSettings["OFP_FILE_UPLOAD_BALANCE_SHEET"];
                        break;
                    case 2:
                        PhysicalPath = ConfigurationManager.AppSettings["OFP_FILE_UPLOAD_MPR"];
                        break;
                    case 3:
                        PhysicalPath = ConfigurationManager.AppSettings["OFP_FILE_UPLOAD_SANCTION_LETTER"];
                        break;
                    case 4:
                        PhysicalPath = ConfigurationManager.AppSettings["OFP_FILE_UPLOAD_BANKER_CERTIFICATE_OB"];
                        break;
                    case 5:
                        PhysicalPath = ConfigurationManager.AppSettings["OFP_FILE_UPLOAD_UTILIZATION_CERTIFICATE"];
                        break;
                    case 6:
                        PhysicalPath = ConfigurationManager.AppSettings["OFP_FILE_UPLOAD_BANK_RECONCILITATION_OB"];
                        break;
                    case 7:
                        PhysicalPath = ConfigurationManager.AppSettings["OFP_FILE_UPLOAD_BANK_RECONCILITATION_CB"];
                        break;
                    case 8:
                        PhysicalPath = ConfigurationManager.AppSettings["OFP_FILE_UPLOAD_BANKER_CERTIFICATE_CB"];
                        break;
                    default:
                        break;
                }


                PhysicalPath = Path.Combine(PhysicalPath, FILE_NAME);

                if (!System.IO.File.Exists(PhysicalPath))
                {
                    //return Json(new { Success = false, ErrorMessage = "File Not Found." });
                }

                bool status = objProcessBAL.DeleteDocumentDetailsBAL(requestDocumentId);

                if (status == true)
                {
                    try
                    {
                        System.IO.File.Delete(PhysicalPath);
                    }
                    catch (Exception ex)
                    {
                        Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                        return Json(new { Success = true, ErrorMessage = ex.Message });
                    }
                    return Json(new { Success = true, ErrorMessage = "There is an error while processing your request." });
                }
                else
                {
                    return Json(new { Success = false, ErrorMessage = status });
                }
            }
            catch
            {
                return Json(new { Success = false, ErrorMessage = "There is an error while processing your request." });
            }
        }

        /// <summary>
        /// returns the list of condition imposed 
        /// </summary>
        /// <returns></returns>
        public ActionResult GetConditionImposedList(FormCollection formCollection)
        {
            long totalRecords;

            try
            {
                if (!objCommon.ValidateGridParameters(new GridParams(Convert.ToInt32(formCollection["page"]), Convert.ToInt32(formCollection["rows"]), formCollection["sidx"], formCollection["sord"], Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                {
                    return null;
                }
                else
                {
                    int requestCode = Convert.ToInt32(formCollection["RequestId"]);

                    var jsonData = new
                    {
                        rows = objProcessBAL.GetConditionImposedListBAL(Convert.ToInt32(formCollection["page"]) - 1, Convert.ToInt32(formCollection["rows"]), formCollection["sidx"], formCollection["sord"], requestCode, out totalRecords),
                        total = totalRecords <= Convert.ToInt32(formCollection["rows"]) ? 1 : totalRecords / Convert.ToInt32(formCollection["rows"]) + 1,
                        page = Convert.ToInt32(formCollection["page"]),
                        records = totalRecords
                    };
                    return Json(jsonData);
                }
            }
            catch (Exception)
            {
                return null;
            }
        }

    }
}


public class TotalFundDetails
{
    public int totalRoads { get; set; }
    public int totalBridges { get; set; }
    public decimal? totalPavementLength { get; set; }
    public decimal? totalBridgeLength { get; set; }
    public decimal? totalStateCost { get; set; }
    public decimal? totalMordCost { get; set; }
    public decimal? totalSanctionCost { get; set; }
    public decimal? totalMaintenanceCost { get; set; }
}
