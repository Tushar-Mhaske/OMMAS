using PMGSY.Areas.ContractorGrievances.DAL;
using PMGSY.Areas.ContractorGrievances.Models;
using PMGSY.Common;
using PMGSY.Controllers;
using PMGSY.DAL.Master;
using PMGSY.Extensions;
using PMGSY.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PMGSY.Areas.ContractorGrievances.Controllers
{
    public class ContractorGrievancesController : Controller
    {
        // GET: /ContractorGrievances/ContractorGrievances/
        PMGSYEntities dbContext = new PMGSYEntities();

        #region Contractor Profile
        [HttpGet]
        public ActionResult GetProfileLayout()
        {
            ContractorGrievancesViewModel model = new ContractorGrievancesViewModel();
            try
            {
                ContractorGrievancesDAL objDAL = new ContractorGrievancesDAL();
                model = objDAL.GetProfileDetails();
                return View(model);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "ContractorGrievances.GetProfileLayout()");
                return null;
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SaveContractorBankDetails(ContractorGrievancesViewModel contractorBankDetails)
        {
            string message = String.Empty;
            bool status = false;
            try
            {
                ContractorGrievancesDAL objDAL = new ContractorGrievancesDAL();
                MasterDAL masterDAL = new MasterDAL();
                if (ModelState.IsValid)
                {
                    if (!masterDAL.ValidatePFMSBankDetailsDAL(contractorBankDetails.BANK_NAME, contractorBankDetails.IFSC_CODE))
                    {
                        return Json(new { success = false, message = "Invalid Ifsc Code entered." }, JsonRequestBehavior.AllowGet);
                    }
                    if (objDAL.SaveContractorBankDetails(contractorBankDetails, ref message))
                    {
                        message = message == string.Empty ? "Bank details saved successfully." : message;
                        status = true;
                    }
                    else
                    {
                        message = message == string.Empty ? "Bank details not saved. " : message;
                    }
                }

                else
                {
                    return View("GetProfileLayout", contractorBankDetails);
                }
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                message = "Bank details not saved.";
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
        }

        #endregion

        #region Register Grievance
        [HttpGet]
        public ActionResult GetRegisterGrievanceLayout()
        {
            RegisterGrievanceViewModel model = new RegisterGrievanceViewModel();
            CommonFunctions comm = new CommonFunctions();
            try
            {
                model.StateList = new List<SelectListItem>();
                model.StateList = comm.PopulateStates(false);
                model.StateList.Find(x => x.Value == "0").Text = "All States";

                model.DistrictList = new List<SelectListItem>();
                model.DistrictList.Insert(0, (new SelectListItem { Text = "All Districts", Value = "0", Selected = true }));
                model.Agreement_Year_List = comm.PopulateFinancialYear(true, true).ToList();
                //model.Agreement_Year_List.RemoveAt(0);

                return View(model);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "ContractorGrievances.GetRegisterGrievanceLayout()");
                return null;
            }
        }

        [HttpGet]
        public ActionResult PopulateDistrictListContractor()
        {
            try
            {
                CommonFunctions comm = new CommonFunctions();
                var DistrictList = comm.PopulateDistrict(Convert.ToInt32(Request.Params["StateCode"]));
                DistrictList.RemoveAll(x => x.Value == "0");
                DistrictList.Insert(0, new SelectListItem { Text = "All District", Value = "0", Selected = true });
                return Json(DistrictList, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "ContractorGrievances/PopulateDistrictListContractor");
                return null;
            }
        }

        [HttpPost]
        public ActionResult GetContractorAgreementList(int? page, int? rows, string sidx, string sord)
        {
            long totalRecords; int state = 0; int district = 0; int agreement_year = 0;
            try
            {
                if (!string.IsNullOrEmpty(Request.Params["state"]))
                {
                    state = Convert.ToInt32(Request.Params["state"]);
                }

                if (!string.IsNullOrEmpty(Request.Params["district"]))
                {
                    district = Convert.ToInt32(Request.Params["district"]);

                }
                if (!string.IsNullOrEmpty(Request.Params["agreement_year"]))
                {
                    agreement_year = Convert.ToInt32(Request.Params["agreement_year"]);

                }

                ContractorGrievancesDAL dalObj = new ContractorGrievancesDAL();

                using (CommonFunctions commonFunction = new CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new GridParams(Convert.ToInt32(Request.Params["page"]), Convert.ToInt32(Request.Params["rows"]), Request.Params["sidx"], Request.Params["sord"], Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                    {
                        return null;
                    }
                }
                var jsonData = new
                {
                    rows = dalObj.AgreementListDAL(state, district, agreement_year, Convert.ToInt32(Request.Params["page"]) - 1, Convert.ToInt32(Request.Params["rows"]), Request.Params["sidx"], Request.Params["sord"], out totalRecords),
                    total = totalRecords <= Convert.ToInt32(Request.Params["rows"]) ? 1 : totalRecords / Convert.ToInt32(Request.Params["rows"]) + 1,
                    page = Convert.ToInt32(Request.Params["page"]),
                    records = totalRecords
                };
                return Json(jsonData, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "ContractorGrievancesController.GetContractorAgreementList()");
                return null;
            }
        }

        [HttpGet]
        [Audit]
        public ActionResult RegisterGrievanceForm(String parameter, String hash, String key)
        {
            ContractorGrievancesDAL objDAL = new ContractorGrievancesDAL();
            AgreementDetailsModel agViewModel = new AgreementDetailsModel();
            Dictionary<string, string> decryptedParameters = null;

            int TEND_AGREEMENT_CODE = 0; int RoadCode = 0;
            String StateName = String.Empty; String DistrictName = String.Empty;
            try
            {
                if (!string.IsNullOrEmpty(Request.Params["agreementCode"]))
                {
                    String[] urlParams = Request.Params["agreementCode"].Split('$');
                    TEND_AGREEMENT_CODE = Convert.ToInt32(urlParams[0]);
                    ViewBag.operation = urlParams[1].ToString();
                }
                else
                {
                    decryptedParameters = URLEncrypt.DecryptParameters1(new string[] { parameter, hash, key });

                    if (decryptedParameters.Count() > 0)
                    {
                        TEND_AGREEMENT_CODE = Convert.ToInt32(decryptedParameters["AgreementCode"].ToString());
                        RoadCode = Convert.ToInt32(decryptedParameters["RoadCode"].ToString());
                        StateName = decryptedParameters["State"].ToString();
                        DistrictName = decryptedParameters["District"].ToString();
                        ViewBag.operation = "A"; //Add Grievances
                    }
                    else
                    {
                        return null;
                    }
                }
                if (TEND_AGREEMENT_CODE != 0)
                {
                    agViewModel.Agreement_Code = TEND_AGREEMENT_CODE;
                    agViewModel.Agreement_Number = dbContext.TEND_AGREEMENT_MASTER.Where(x => x.TEND_AGREEMENT_CODE == agViewModel.Agreement_Code).Select(x => x.TEND_AGREEMENT_NUMBER).FirstOrDefault();
                    agViewModel.Agreement_Date = dbContext.TEND_AGREEMENT_MASTER.Where(x => x.TEND_AGREEMENT_CODE == agViewModel.Agreement_Code).Select(x => x.TEND_DATE_OF_AGREEMENT).FirstOrDefault();

                    agViewModel.State = StateName;                                                                //dbContext.MASTER_STATE.Where(x => x.MAST_STATE_CODE == agViewModel.StateCode).Select(x => x.MAST_STATE_NAME).FirstOrDefault();
                    agViewModel.District = DistrictName;                                                        // dbContext.MASTER_DISTRICT.Where(x => x.MAST_DISTRICT_CODE == agViewModel.DistrictCode).Select(x => x.MAST_DISTRICT_NAME).FirstOrDefault();

                    agViewModel.Grievance_Type_List = new List<SelectListItem>();
                    agViewModel.Grievance_Type_List.Add(new SelectListItem { Value = "0", Text = "Select Grievance Type" });
                    agViewModel.Grievance_Type_List.Add(new SelectListItem { Value = "1", Text = "Progress" });
                    agViewModel.Grievance_Type_List.Add(new SelectListItem { Value = "2", Text = "Finance" });
                    agViewModel.Grievance_Type_List.Add(new SelectListItem { Value = "3", Text = "Quality" });
                    agViewModel.Grievance_Type = 0;
                    agViewModel.Grievance_SubType_List = new List<SelectListItem>();
                    agViewModel.Grievance_SubType_List.Add(new SelectListItem { Value = "0", Text = "Select a Sub-Type" });
                    agViewModel.Grievance_SubType = 0;
                    agViewModel.IMS_PR_ROAD_CODE = RoadCode;

                }

                return View("~/Areas/ContractorGrievances/Views/ContractorGrievances/RegisterGrievanceForm.cshtml", agViewModel);

            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, " ContractorGrievances.RegisterGrievanceForm()");
                return null;
            }
        }

        [HttpPost]
        [Audit]
        public ActionResult SaveContractorGrievance(AgreementDetailsModel formData)
        {
            ContractorGrievancesDAL objDAL = new ContractorGrievancesDAL();
            PMGSYEntities dbContext = new PMGSYEntities();
            string filePath = string.Empty;
            bool isFileSaved = false;
            string referenceNo = null;
            HttpPostedFileBase FileBase = null;
            bool uploadStatus = false;
            try
            {
                if (ModelState.IsValid)
                {
                    #region File Saving Code
                    if (Request.Files.AllKeys.Count() <= 0)
                    {
                        return Json(new { success = false, ErrorMessage = "Please select a file" });
                    }
                    HttpFileCollectionBase FilesList = Request.Files;
                    if (Request.Files.Count > 0)
                    {
                        for (int i = 0; i < Request.Files.Count; i++)
                        {
                            FileBase = Request.Files[i];
                            var filename = FileBase.FileName;
                            uploadStatus = objDAL.SaveGrievanceFileDAL(filename, FileBase, out isFileSaved);

                            if (uploadStatus == false && isFileSaved == false)
                            {
                                break;
                            }
                        }
                    }
                    if (uploadStatus == false && isFileSaved == false)
                    {
                        return Json(new { success = false, ErrorMessage = "Error in Saving uploaded file " });
                    }
                    #endregion

                    string Status = objDAL.SaveContractorGrievanceDAL(formData, FilesList, out referenceNo);
                    if (Status == string.Empty)
                        return Json(new { Success = true, ReferenceNo = referenceNo });
                    else
                        return Json(new { Success = false, ErrorMessage = Status });
                }
                else
                {
                    return Json(new { Success = false, ErrorMessage = new CommonFunctions().FormatErrorMessage(ModelState) });
                }
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "SaveContractorGrievance(AgreementDetailsModel formData))");
                throw ex;
            }
        }

        [HttpGet]
        public ActionResult PopulateGrievanceSubTypeList()
        {
            dbContext = new PMGSYEntities();
            List<SelectListItem> SubTypeList = null;
            try
            {
                if (!string.IsNullOrEmpty(Request.Params["typeCode"]))
                {
                    string grievanceType = Convert.ToInt32(Request.Params["typeCode"]) == 1 ? "P" : Convert.ToInt32(Request.Params["typeCode"]) == 2 ? "F" : Convert.ToInt32(Request.Params["typeCode"]) == 3 ? "Q" : "";
                    SubTypeList = new SelectList(dbContext.MASTER_CONTRACTOR_GRIEVANCE.Where(x => x.GRIEVANCE_TYPE == grievanceType).OrderBy(x => x.GRIEVANCE_SUBTYPE), "GRIEVANCE_ID", "GRIEVANCE_SUBTYPE", 0).ToList();
                    SubTypeList.RemoveAll(x => x.Value == "0");
                    SubTypeList.Insert(0, new SelectListItem { Text = "Select a Sub-Type", Value = "0", Selected = true });
                    return Json(SubTypeList, JsonRequestBehavior.AllowGet);
                }
                return Json(SubTypeList, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "ContractorGrievances/PopulateGrievanceSubTypeList");
                return null;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        #endregion

        #region Track Grievance
        [HttpPost]
        public ActionResult GetContractorGrievanceList(int? page, int? rows, string sidx, string sord)
        {
            long totalRecords; int roadCode = 0;
            try
            {
                if (!string.IsNullOrEmpty(Request.Params["roadCode"]))
                {
                    roadCode = Convert.ToInt32(Request.Params["roadCode"]);
                }
                ContractorGrievancesDAL dalObj = new ContractorGrievancesDAL();

                using (CommonFunctions commonFunction = new CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new GridParams(Convert.ToInt32(Request.Params["page"]), Convert.ToInt32(Request.Params["rows"]), Request.Params["sidx"], Request.Params["sord"], Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                    {
                        return null;
                    }
                }
                var jsonData = new
                {
                    rows = dalObj.GrievanceListDAL(roadCode, Convert.ToInt32(Request.Params["page"]) - 1, Convert.ToInt32(Request.Params["rows"]), Request.Params["sidx"], Request.Params["sord"], out totalRecords),
                    total = totalRecords <= Convert.ToInt32(Request.Params["rows"]) ? 1 : totalRecords / Convert.ToInt32(Request.Params["rows"]) + 1,
                    page = Convert.ToInt32(Request.Params["page"]),
                    records = totalRecords
                };
                return Json(jsonData, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "ContractorGrievancesController.GetContractorGrievanceList()");
                return null;
            }
        }

        [HttpGet]
        [Audit]
        public ActionResult ViewEditGrievanceForm(String parameter, String hash, String key)
        {
            ContractorGrievancesDAL objDAL = new ContractorGrievancesDAL();
            AgreementDetailsModel agViewModel = new AgreementDetailsModel();
            Dictionary<string, string> decryptedParameters = null;
            CONTRACTOR_GRIEVANCE_DETAILS GrievanceRecord = new CONTRACTOR_GRIEVANCE_DETAILS();

            int RoadCode = 0; int DetailID = 0;
            try
            {
                decryptedParameters = URLEncrypt.DecryptParameters1(new string[] { parameter, hash, key });

                if (decryptedParameters.Count() > 0)
                {
                    RoadCode = Convert.ToInt32(decryptedParameters["RoadCode"].ToString());
                    DetailID = Convert.ToInt32(decryptedParameters["DetailId"].ToString());
                    ViewBag.operation = decryptedParameters["Operation"].ToString();
                }
                else
                {
                    return null;
                }

                if (DetailID != 0 && RoadCode != 0)
                {
                    if (ViewBag.operation == "E")
                    {
                        GrievanceRecord = dbContext.CONTRACTOR_GRIEVANCE_DETAILS.Where(x => x.DETAIL_ID == DetailID && x.IMS_PR_ROAD_CODE == RoadCode && x.GRIEVANCE_BY_USERID == PMGSYSession.Current.UserId && x.IS_FINALIZED == "N" && x.IS_CLOSED == "N").FirstOrDefault();
                    }
                    else
                    {
                        GrievanceRecord = dbContext.CONTRACTOR_GRIEVANCE_DETAILS.Where(x => x.DETAIL_ID == DetailID && x.IMS_PR_ROAD_CODE == RoadCode && x.GRIEVANCE_BY_USERID == PMGSYSession.Current.UserId && x.IS_FINALIZED == "Y").FirstOrDefault();
                    }
                    agViewModel.Detail_Id = DetailID;
                    agViewModel.Feedback_Complaint = GrievanceRecord.FEEDBACK_COMPLAINT == "C" ? true : false;
                    agViewModel.Grievance_Type_List = new List<SelectListItem>();
                    agViewModel.Grievance_Type_List.Add(new SelectListItem { Value = "0", Text = "Select Grievance Type" });
                    agViewModel.Grievance_Type_List.Add(new SelectListItem { Value = "1", Text = "Progress" });
                    agViewModel.Grievance_Type_List.Add(new SelectListItem { Value = "2", Text = "Finance" });
                    agViewModel.Grievance_Type_List.Add(new SelectListItem { Value = "3", Text = "Quality" });
                    agViewModel.Grievance_Type = GrievanceRecord.GRIEVANCE_TYPE == "P" ? 1 : GrievanceRecord.GRIEVANCE_TYPE == "F" ? 2 : GrievanceRecord.GRIEVANCE_TYPE == "Q" ? 3 : 0;

                    agViewModel.Grievance_SubType_List = new List<SelectListItem>();
                    agViewModel.Grievance_SubType_List = new SelectList(dbContext.MASTER_CONTRACTOR_GRIEVANCE.Where(x => x.GRIEVANCE_TYPE == GrievanceRecord.GRIEVANCE_TYPE).OrderBy(x => x.GRIEVANCE_SUBTYPE), "GRIEVANCE_ID", "GRIEVANCE_SUBTYPE", 0).ToList();
                    //agViewModel.Grievance_SubType_List.Add(new SelectListItem { Value = "0", Text = "Select a Sub-Type" });
                    agViewModel.Grievance_SubType = GrievanceRecord.GRIEVANCE_ID;

                    agViewModel.IMS_PR_ROAD_CODE = RoadCode;
                    agViewModel.Grievance_Description = GrievanceRecord.GRIEVANCE_DESC;
                }

                return View("~/Areas/ContractorGrievances/Views/ContractorGrievances/RegisterGrievanceForm.cshtml", agViewModel);

            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, " ContractorGrievances.ViewEditGrievanceForm()");
                return null;
            }
        }

        [HttpPost]
        [Audit]
        public ActionResult EditContractorGrievance(AgreementDetailsModel formData)
        {
            ContractorGrievancesDAL objDAL = new ContractorGrievancesDAL();
            dbContext = new PMGSYEntities();
            string filePath = string.Empty;
            bool isFileSaved = false;
            string referenceNo = null;
            HttpPostedFileBase FileBase = null;
            bool uploadStatus = false;
            try
            {
                if (ModelState.IsValid)
                {
                    #region File Saving Code
                    HttpFileCollectionBase FilesList = Request.Files;
                    if (Request.Files.Count > 0)
                    {
                        for (int i = 0; i < Request.Files.Count; i++)
                        {
                            FileBase = Request.Files[i];
                            var filename = FileBase.FileName;
                            uploadStatus = objDAL.SaveGrievanceFileDAL(filename, FileBase, out isFileSaved);

                            if (uploadStatus == false && isFileSaved == false)
                            {
                                break;
                            }
                        }
                    }
                    if (uploadStatus == false && isFileSaved == false)
                    {
                        return Json(new { success = false, ErrorMessage = "Error in Saving uploaded file " });
                    }
                    #endregion

                    string Status = objDAL.EditContractorGrievanceDAL(formData, FilesList, out referenceNo);
                    if (Status == string.Empty)
                        return Json(new { Success = true, ReferenceNo = referenceNo });
                    else
                        return Json(new { Success = false, ErrorMessage = Status });
                }
                else
                {
                    return Json(new { Success = false, ErrorMessage = new CommonFunctions().FormatErrorMessage(ModelState) });
                }
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "EditContractorGrievance(AgreementDetailsModel formData))");
                throw ex;
            }
        }

        [HttpPost]
        public ActionResult FinalizeContractorGrievance(String parameter, String hash, String key)
        {
            dbContext = new PMGSYEntities();
            ContractorGrievancesDAL objDAL = new ContractorGrievancesDAL();
            AgreementDetailsModel agViewModel = new AgreementDetailsModel();
            Dictionary<string, string> decryptedParameters = null;
            string message = "";
            bool status = false;
            int DetailId = 0; int roadCode = 0;

            try
            {
                decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { parameter, hash, key });

                if (decryptedParameters.Count > 0)
                {
                    DetailId = Convert.ToInt32(decryptedParameters["DetailId"]);

                    if (objDAL.FinalizeContractorGrievanceDAL(DetailId, out roadCode))
                    {
                        status = true;

                        message = "Grievance details finalized successfully.";

                        return Json(new { success = status, message = message, roadCode = roadCode });
                    }
                    else
                    {
                        message = "Grievance details cannot be finalized.";
                        return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    message = "Grievance details cannot be finalized.";
                    return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                message = "Grievance details cannot be finalized.";
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public ActionResult DownloadGrievanceFile(String parameter, String hash, String key)
        {
            try
            {
                dbContext = new PMGSYEntities();
                string FileName = string.Empty;
                int detailId = 0;
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
                        detailId = Convert.ToInt32(urlSplitParams[1]);
                    }
                }
                FileExtension = Path.GetExtension(FileName).ToLower();

                var record = (from con in dbContext.CONTRACTOR_GRIEVANCE_FILE_DETAILS
                              where con.DETAIL_ID == detailId
                              select con).First();

                string VirtualDirectoryUrl = string.Empty;
                string PhysicalPath = string.Empty;
                if (!String.IsNullOrEmpty(record.FILENAME))
                {
                    VirtualDirectoryUrl = ConfigurationManager.AppSettings["ContractorGrievanceFilePath_Virtual_Dir_Path"];
                    PhysicalPath = ConfigurationManager.AppSettings["ContractorGrievanceFilePath"];
                }
                else
                {
                    VirtualDirectoryUrl = null;
                    PhysicalPath = null;
                }

                FullFileLogicalPath = Path.Combine(VirtualDirectoryUrl, FileName);
                FullfilePhysicalPath = Path.Combine(PhysicalPath, FileName);


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
                        case ".doc":
                        case ".docx":
                            type = "Application/msword";
                            break;
                        case ".jpg":
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
            catch
            {
                return Json(new { Success = "false", ErrorMessage = "Error While Processing Your Request." }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult GetGrievanceFilesList(int? page, int? rows, string sidx, string sord)
        {
            long totalRecords; int detailId = 0; string uploadedBy = String.Empty;
            try
            {
                if (!string.IsNullOrEmpty(Request.Params["detailId"]))
                {
                    detailId = Convert.ToInt32(Request.Params["detailId"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["uploadedBy"]))
                {
                    uploadedBy = Request.Params["uploadedBy"];
                }
                ContractorGrievancesDAL dalObj = new ContractorGrievancesDAL();
                var jsonData = new
                {
                    rows = dalObj.FilesListDAL(detailId, uploadedBy, Convert.ToInt32(Request.Params["page"]) - 1, Convert.ToInt32(Request.Params["rows"]), Request.Params["sidx"], Request.Params["sord"], out totalRecords),
                    total = totalRecords <= Convert.ToInt32(Request.Params["rows"]) ? 1 : totalRecords / Convert.ToInt32(Request.Params["rows"]) + 1,
                    page = Convert.ToInt32(Request.Params["page"]),
                    records = totalRecords
                };
                return Json(jsonData, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "ContractorGrievancesController.GetGrievanceFilesList()");
                return null;
            }
        }

        #endregion

        #region Resolve Grievance By SRRDA and PIU

        [HttpGet]
        public ActionResult TrackGrievanceLayout()
        {
            RegisterGrievanceViewModel model = new RegisterGrievanceViewModel();
            CommonFunctions comm = new CommonFunctions();
            try
            {
                if (PMGSYSession.Current.StateCode > 0)
                {
                    model.StateList = new List<SelectListItem>();
                    model.StateList.Insert(0, (new SelectListItem { Text = PMGSYSession.Current.StateName, Value = Convert.ToString(PMGSYSession.Current.StateCode), Selected = true }));
                    model.DistrictList = new List<SelectListItem>();
                    if (PMGSYSession.Current.DistrictCode > 0)
                    {
                        model.DistrictList.Insert(0, new SelectListItem() { Text = PMGSYSession.Current.DistrictName, Value = PMGSYSession.Current.DistrictCode.ToString() });
                    }
                    else
                    {
                        model.DistrictList = comm.PopulateDistrict(PMGSYSession.Current.StateCode, true);
                        model.DistrictList.Find(x => x.Value == "-1").Value = "0";
                    }
                }
                else
                {
                    model.StateList = new List<SelectListItem>();
                    model.StateList = comm.PopulateStates(false);
                    model.StateList.Find(x => x.Value == "0").Text = "All States";
                    model.DistrictList = new List<SelectListItem>();
                    model.DistrictList.Insert(0, (new SelectListItem { Text = "All Districts", Value = "0", Selected = true }));
                }

                model.Agreement_Year_List = comm.PopulateFinancialYear(true, true).ToList();
                //model.Agreement_Year_List.RemoveAt(0);

                return View(model);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "ContractorGrievances.TrackGrievanceLayout()");
                return null;
            }
        }

        [HttpPost]
        public ActionResult GetGrievanceListForSrrdaPiu(int? page, int? rows, string sidx, string sord)
        {
            long totalRecords; int state = 0; int district = 0; int package_year = 0; string login = String.Empty;
            try
            {
                if (!string.IsNullOrEmpty(Request.Params["state"]))
                {
                    state = 0;// Convert.ToInt32(Request.Params["state"]);
                }

                if (!string.IsNullOrEmpty(Request.Params["district"]))
                {
                    district = 0;// Convert.ToInt32(Request.Params["district"]);

                }
                if (!string.IsNullOrEmpty(Request.Params["package_year"]))
                {
                    package_year = 0;// Convert.ToInt32(Request.Params["agreement_year"]);

                }
                if (!string.IsNullOrEmpty(Request.Params["package_year"]))
                {
                    login = Request.Params["login"];

                }
                ContractorGrievancesDAL dalObj = new ContractorGrievancesDAL();
                using (CommonFunctions commonFunction = new CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new GridParams(Convert.ToInt32(Request.Params["page"]), Convert.ToInt32(Request.Params["rows"]), Request.Params["sidx"], Request.Params["sord"], Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                    {
                        return null;
                    }
                }
                if (login == "srrda")
                {
                    var jsonData = new
                    {
                        rows = dalObj.PopulateGrievanceListSrrdaDAL(state, district, package_year, Convert.ToInt32(Request.Params["page"]) - 1, Convert.ToInt32(Request.Params["rows"]), Request.Params["sidx"], Request.Params["sord"], out totalRecords),
                        total = totalRecords <= Convert.ToInt32(Request.Params["rows"]) ? 1 : totalRecords / Convert.ToInt32(Request.Params["rows"]) + 1,
                        page = Convert.ToInt32(Request.Params["page"]),
                        records = totalRecords
                    };
                    return Json(jsonData, JsonRequestBehavior.AllowGet);
                }
                if (login == "piu")
                {
                    var jsonData = new
                    {
                        rows = dalObj.PopulateGrievanceListPiuDAL(state, district, package_year, Convert.ToInt32(Request.Params["page"]) - 1, Convert.ToInt32(Request.Params["rows"]), Request.Params["sidx"], Request.Params["sord"], out totalRecords),
                        total = totalRecords <= Convert.ToInt32(Request.Params["rows"]) ? 1 : totalRecords / Convert.ToInt32(Request.Params["rows"]) + 1,
                        page = Convert.ToInt32(Request.Params["page"]),
                        records = totalRecords
                    };
                    return Json(jsonData, JsonRequestBehavior.AllowGet);
                }
                return Json(string.Empty);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "ContractorGrievancesController.GetGrievanceListForSRRDA()");
                return null;
            }
        }

        [HttpGet]
        [Audit]
        public ActionResult ForwardGrievanceToPiuForm(String parameter, String hash, String key)
        {
            ContractorGrievancesDAL objDAL = new ContractorGrievancesDAL();
            TrackingDetailsModel viewModel = new TrackingDetailsModel();
            Dictionary<string, string> decryptedParameters = null;
            int detailId = 0; string form = String.Empty;
            ViewBag.Operation = "SRRDA";
            try
            {
                if (!String.IsNullOrEmpty(parameter) && !String.IsNullOrEmpty(hash) && !String.IsNullOrEmpty(key))
                {
                    decryptedParameters = URLEncrypt.DecryptParameters1(new string[] { parameter, hash, key });

                    if (decryptedParameters.Count() > 0)
                    {
                        detailId = Convert.ToInt32(decryptedParameters["DetailId"]);
                        form = decryptedParameters["Form"];
                    }
                    else
                    {
                        return null;
                    }
                }
                if (detailId != 0)
                {
                    viewModel.DETAIL_ID = detailId;
                    viewModel.SUBMITTED_ON = dbContext.CONTRACTOR_GRIEVANCE_DETAILS.Where(x => x.DETAIL_ID == detailId).Select(x => x.FINALIZATION_DATE).FirstOrDefault();
                    viewModel.Contractor_Remarks = dbContext.CONTRACTOR_GRIEVANCE_DETAILS.Where(x => x.DETAIL_ID == detailId).Select(x => x.GRIEVANCE_DESC).FirstOrDefault();
                    String Grievance_Category = dbContext.CONTRACTOR_GRIEVANCE_DETAILS.Where(x => x.DETAIL_ID == detailId).Select(x => x.GRIEVANCE_TYPE).FirstOrDefault();
                    viewModel.Contractor_Grievance_Category = Grievance_Category == "P" ? "Progress" : Grievance_Category == "F" ? "Finance" : Grievance_Category == "Q" ? "Quality" : "Others";
                    int GrievanceSubTypeId = dbContext.CONTRACTOR_GRIEVANCE_DETAILS.Where(x => x.DETAIL_ID == detailId).Select(x => x.GRIEVANCE_ID).FirstOrDefault();
                    viewModel.Contractor_Grievance_SubCat = dbContext.MASTER_CONTRACTOR_GRIEVANCE.Where(x => x.GRIEVANCE_ID == GrievanceSubTypeId).Select(x => x.GRIEVANCE_SUBTYPE).FirstOrDefault();
                    viewModel.FORWARD_DATE = dbContext.CONTRACTOR_GRIEVANCE_TRACKING_DETAILS.Where(x => x.DETAIL_ID == detailId).Select(x => x.FORWARD_DATE).FirstOrDefault();
                    if (form == "srrdaRead")
                    {
                        ViewBag.Form = form;
                        viewModel.SRRDA_REMARKS = dbContext.CONTRACTOR_GRIEVANCE_TRACKING_DETAILS.Where(x => x.DETAIL_ID == detailId).Select(x => x.SRRDA_REMARKS).FirstOrDefault();
                    }
                    else
                    {
                        ViewBag.Form = form;
                    }
                }

                return View("~/Areas/ContractorGrievances/Views/ContractorGrievances/ForwardGrievanceToPiuForm.cshtml", viewModel);

            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, " ContractorGrievances.ForwardGrievanceToPiuForm()");
                return null;
            }
        }

        [HttpPost]
        [Audit]
        public ActionResult SaveGrievanceTrackingBySrrda(TrackingDetailsModel formData)
        {
            ContractorGrievancesDAL objDAL = new ContractorGrievancesDAL();
            PMGSYEntities dbContext = new PMGSYEntities();
            string filePath = string.Empty;
            bool isFileSaved = false;
            string referenceNo = null;
            HttpPostedFileBase FileBase = null;
            bool uploadStatus = false;
            try
            {
                if (ModelState.IsValid)
                {
                    #region File Saving Code
                    if (Request.Files.AllKeys.Count() <= 0)
                    {
                        return Json(new { success = false, ErrorMessage = "Please select a file" });
                    }
                    HttpFileCollectionBase FilesList = Request.Files;
                    if (Request.Files.Count > 0)
                    {
                        for (int i = 0; i < Request.Files.Count; i++)
                        {
                            FileBase = Request.Files[i];
                            var filename = FileBase.FileName;
                            uploadStatus = objDAL.SaveGrievanceFileDAL(filename, FileBase, out isFileSaved);

                            if (uploadStatus == false && isFileSaved == false)
                            {
                                break;
                            }
                        }
                    }
                    if (uploadStatus == false && isFileSaved == false)
                    {
                        return Json(new { success = false, ErrorMessage = "Error in Saving uploaded file " });
                    }
                    #endregion

                    string Status = objDAL.SaveGrievanceTrackingDAL(formData, FilesList);
                    if (Status == string.Empty)
                        return Json(new { Success = true, ReferenceNo = referenceNo });
                    else
                        return Json(new { Success = false, ErrorMessage = Status });
                }
                else
                {
                    return Json(new { Success = false, ErrorMessage = new CommonFunctions().FormatErrorMessage(ModelState) });
                }
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "SaveGrievanceTracking(TrackingDetailsModel formData)");
                throw ex;
            }
        }

        [HttpGet]
        [Audit]
        public ActionResult PiuActionOnGrievanceForm(String parameter, String hash, String key)
        {
            ContractorGrievancesDAL objDAL = new ContractorGrievancesDAL();
            TrackingDetailsModel viewModel = new TrackingDetailsModel();
            Dictionary<string, string> decryptedParameters = null;
            int detailId = 0; string form = String.Empty;
            ViewBag.Operation = "PIU";
            try
            {
                if (!String.IsNullOrEmpty(parameter) && !String.IsNullOrEmpty(hash) && !String.IsNullOrEmpty(key))
                {
                    decryptedParameters = URLEncrypt.DecryptParameters1(new string[] { parameter, hash, key });

                    if (decryptedParameters.Count() > 0)
                    {
                        detailId = Convert.ToInt32(decryptedParameters["DetailId"]);
                        form = decryptedParameters["Form"];
                    }
                    else
                    {
                        return null;
                    }
                }
                if (detailId != 0)
                {
                    viewModel.DETAIL_ID = detailId;
                    viewModel.Contractor_Remarks = dbContext.CONTRACTOR_GRIEVANCE_DETAILS.Where(x => x.DETAIL_ID == detailId).Select(x => x.GRIEVANCE_DESC).FirstOrDefault();
                    viewModel.SUBMITTED_ON = dbContext.CONTRACTOR_GRIEVANCE_DETAILS.Where(x => x.DETAIL_ID == detailId).Select(x => x.FINALIZATION_DATE).FirstOrDefault();
                    String Grievance_Category = dbContext.CONTRACTOR_GRIEVANCE_DETAILS.Where(x => x.DETAIL_ID == detailId).Select(x => x.GRIEVANCE_TYPE).FirstOrDefault();
                    viewModel.Contractor_Grievance_Category = Grievance_Category == "P" ? "Progress" : Grievance_Category == "F" ? "Finance" : Grievance_Category == "Q" ? "Quality" : "Others";

                    int GrievanceSubTypeId = dbContext.CONTRACTOR_GRIEVANCE_DETAILS.Where(x => x.DETAIL_ID == detailId).Select(x => x.GRIEVANCE_ID).FirstOrDefault();
                    viewModel.Contractor_Grievance_SubCat = dbContext.MASTER_CONTRACTOR_GRIEVANCE.Where(x => x.GRIEVANCE_ID == GrievanceSubTypeId).Select(x => x.GRIEVANCE_SUBTYPE).FirstOrDefault();

                    viewModel.SRRDA_REMARKS = dbContext.CONTRACTOR_GRIEVANCE_TRACKING_DETAILS.Where(x => x.DETAIL_ID == detailId).Select(x => x.SRRDA_REMARKS).FirstOrDefault();
                    viewModel.FORWARD_DATE = dbContext.CONTRACTOR_GRIEVANCE_TRACKING_DETAILS.Where(x => x.DETAIL_ID == detailId).Select(x => x.FORWARD_DATE).FirstOrDefault();
                    viewModel.IS_LATEST = dbContext.CONTRACTOR_GRIEVANCE_TRACKING_DETAILS.Where(x => x.DETAIL_ID == detailId).Select(x => x.IS_LATEST).FirstOrDefault() == "Y" ? "Yes" : "No";
                    viewModel.PIU_FINALIZATION_DATE = dbContext.CONTRACTOR_GRIEVANCE_TRACKING_DETAILS.Where(x => x.DETAIL_ID == detailId).Select(x => x.PIU_FINALIZATION_DATE).FirstOrDefault();
                    if (form == "piuRead")
                    {
                        ViewBag.Form = form;
                        viewModel.PIU_REMARKS = dbContext.CONTRACTOR_GRIEVANCE_TRACKING_DETAILS.Where(x => x.DETAIL_ID == detailId).Select(x => x.PIU_REMARKS).FirstOrDefault();
                    }
                    else
                    {
                        ViewBag.Form = form;
                    }
                }

                return View("~/Areas/ContractorGrievances/Views/ContractorGrievances/ForwardGrievanceToPiuForm.cshtml", viewModel);

            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, " ContractorGrievances.PiuActionOnGrievanceForm()");
                return null;
            }
        }

        [HttpPost]
        [Audit]
        public ActionResult SavePIUactionOnGrievance(TrackingDetailsModel formData)
        {
            ContractorGrievancesDAL objDAL = new ContractorGrievancesDAL();
            PMGSYEntities dbContext = new PMGSYEntities();
            string filePath = string.Empty;
            bool isFileSaved = false;
            string referenceNo = null;
            HttpPostedFileBase FileBase = null;
            bool uploadStatus = false;
            try
            {
                if (ModelState.IsValid)
                {
                    #region File Saving Code
                    if (Request.Files.AllKeys.Count() <= 0)
                    {
                        return Json(new { success = false, ErrorMessage = "Please select a file" });
                    }
                    HttpFileCollectionBase FilesList = Request.Files;
                    if (Request.Files.Count > 0)
                    {
                        for (int i = 0; i < Request.Files.Count; i++)
                        {
                            FileBase = Request.Files[i];
                            var filename = FileBase.FileName;
                            uploadStatus = objDAL.SaveGrievanceFileDAL(filename, FileBase, out isFileSaved);

                            if (uploadStatus == false && isFileSaved == false)
                            {
                                break;
                            }
                        }
                    }
                    if (uploadStatus == false && isFileSaved == false)
                    {
                        return Json(new { success = false, ErrorMessage = "Error in Saving uploaded file " });
                    }
                    #endregion

                    string Status = objDAL.SavePIUactionOnGrievanceDAL(formData, FilesList);
                    if (Status == string.Empty)
                        return Json(new { Success = true, ReferenceNo = referenceNo });
                    else
                        return Json(new { Success = false, ErrorMessage = Status });
                }
                else
                {
                    return Json(new { Success = false, ErrorMessage = new CommonFunctions().FormatErrorMessage(ModelState) });
                }
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "SavePIUactionOnGrievance(TrackingDetailsModel formData)");
                throw ex;
            }
        }

        [HttpPost]
        public ActionResult FinalizeGrievanceTrackingByPIU(String parameter, String hash, String key)
        {
            dbContext = new PMGSYEntities();
            ContractorGrievancesDAL objDAL = new ContractorGrievancesDAL();

            Dictionary<string, string> decryptedParameters = null;
            string message = "";
            bool status = false;
            int DetailId = 0; int roadCode = 0;

            try
            {
                decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { parameter, hash, key });

                if (decryptedParameters.Count > 0)
                {
                    DetailId = Convert.ToInt32(decryptedParameters["DetailId"]);

                    if (objDAL.FinalizeGrievanceTrackingDAL(DetailId))
                    {
                        status = true;

                        message = "Grievance tracking details finalized successfully.";

                        return Json(new { success = status, message = message });
                    }
                    else
                    {
                        message = "Grievance tracking details cannot be finalized.";
                        return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    message = "Grievance tracking details cannot be finalized.";
                    return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                message = "Grievance tracking details cannot be finalized.";
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult CloseGrievance(String parameter, String hash, String key)
        {
            dbContext = new PMGSYEntities();
            ContractorGrievancesDAL objDAL = new ContractorGrievancesDAL();

            Dictionary<string, string> decryptedParameters = null;
            string message = "";
            bool status = false;
            int DetailId = 0; int roadCode = 0;

            try
            {
                decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { parameter, hash, key });

                if (decryptedParameters.Count > 0)
                {
                    DetailId = Convert.ToInt32(decryptedParameters["DetailId"]);

                    if (objDAL.CloseGrievanceDAL(DetailId))
                    {
                        status = true;

                        message = "Grievance closed successfully.";

                        return Json(new { success = status, message = message });
                    }
                    else
                    {
                        message = "Grievance cannot be closed.Take Action on Grievance before closing it";
                        return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    message = "Grievance cannot be closed.";
                    return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                message = "Grievance cannot be closed.";
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion
    }
}
        