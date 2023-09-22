using PMGSY.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PMGSY.Areas.EFORMArea.Model;
using PMGSY.Extensions;
using PMGSY.Models;
using PMGSY.Areas.EFORMArea.Dal;
using PMGSY.Common;
using ErrorLog = PMGSY.Areas.EFORMArea.Common.ErrorLog;
using GridParams = PMGSY.Areas.EFORMArea.Common.GridParams;
using CommonFunctions = PMGSY.Areas.EFORMArea.Common.CommonFunctions;
using System.Configuration;
using Syncfusion.Pdf.Parsing;
using System.IO;
using System.Text;
using System.Transactions;
using System.Data.Entity;
using PMGSY.Areas.EFORMArea.QMModels;
using PMGSY.Areas.EFORMArea.QM_ViewPdf_Model;
using PMGSY.Areas.EFORMArea.PiuBridgeModel;
using PMGSY.Areas.EFORMArea.QMBridgeViewPdfModel;
using PMGSY.Areas.EFORMArea.QMBridgeModel;
using PMGSY.Areas.EFORMArea.PiuBridgeViewPdfModel;
using PMGSY.Areas.EFORMArea.Models;
using PMGSY.Areas.EFORMArea.TestReportModel;

//ajit sir


using System.Net;

using System.Web.Script;
using System.Web.Script.Serialization;
using System.Web.Helpers;

using System.Security;
using PMGSY.Areas.EFORMArea.TestReportPreviewModel;
using Syncfusion.Pdf;

namespace PMGSY.Areas.EFORMArea.Controllers
{

    [RequiredAuthentication]
    [RequiredAuthorization]
    public class EFORMController : Controller
    {
        //
        // GET: /EFORMArea/EFORM/
        private string message = string.Empty;
        Dictionary<string, string> decryptedParameters = null;

        [RequiredAuthentication]
        [HttpGet]
        public ActionResult Index()
        {
            PMGSYEntities eformdbContext = new PMGSYEntities();
            PdfPageViewModel model = new PdfPageViewModel();
            CommonFunctions comm = new CommonFunctions();
            try
            {

                model.StateName = PMGSYSession.Current.StateName;
                model.DistrictName = PMGSYSession.Current.DistrictName;
                model.stateCode = PMGSYSession.Current.StateCode;
                model.districtCode = PMGSYSession.Current.DistrictCode;
                model.lstDistrict = new List<SelectListItem>();
                model.MonthCode = System.DateTime.Now.Month;
                model.YearCode = System.DateTime.Now.Year;
                if (PMGSYSession.Current.RoleCode == 7)   //SQM
                {
                    int userId = PMGSYSession.Current.UserId;
                    int admnQmCode = eformdbContext.ADMIN_QUALITY_MONITORS.Where(s => s.ADMIN_USER_ID == userId).Select(m => m.ADMIN_QM_CODE).FirstOrDefault();
                    if (eformdbContext.ADMIN_QUALITY_MONITORS_INTER_STATE.Any(s => s.ADMIN_QM_CODE == admnQmCode))
                    {
                        model.lstState = comm.PopulateStatesInterStateSQM(admnQmCode, true);
                        // model.lstDistrict.Insert(0, new SelectListItem() { Text = "Select District", Value = "-1" });
                        model.lstDistrict = comm.PopulateDistrict(PMGSYSession.Current.StateCode, false);
                        model.isInterstateSQM = "Y";
                    }
                    else
                    {
                        model.isInterstateSQM = "N";
                        model.lstDistrict = comm.PopulateDistrict(PMGSYSession.Current.StateCode, false);
                    }
                }
                else if (PMGSYSession.Current.RoleCode == 6 || PMGSYSession.Current.RoleCode == 9 || PMGSYSession.Current.RoleCode == 5)   //NQM  && //CQC
                {
                    model.lstState = comm.PopulateStates(true);
                    model.lstDistrict.Insert(0, new SelectListItem() { Text = "Select District", Value = "-1" });
                }
                else if (PMGSYSession.Current.RoleCode == 8 || PMGSYSession.Current.RoleCode == 48 || PMGSYSession.Current.RoleCode == 69)   //SQC  
                {
                    model.lstDistrict = comm.PopulateDistrict(PMGSYSession.Current.StateCode, false);
                }
                model.lstMonth = comm.PopulateMonths(true);
                model.Eform_Type = new List<SelectListItem> {
                                                            new SelectListItem{ Text = "All", Value ="O" , Selected = true },
                                                            new SelectListItem{ Text = "Road", Value ="P" },
                                                            new SelectListItem{ Text = "Bridge", Value ="L" } };



                model.lstYear = comm.PopulateYears(true);
                model.lstEformStatus = new List<SelectListItem>();
                model.lstEformStatus.Insert(0, new SelectListItem() { Text = "All", Value = "O", Selected = true });
                model.lstEformStatus.Add(new SelectListItem() { Text = "Pending", Value = "N" });
                model.lstEformStatus.Add(new SelectListItem() { Text = "Submitted", Value = "Y" });
                model.RoleCode = PMGSYSession.Current.RoleCode;
                return View(model);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "eformcontroller.Index()");
                return View("~/Areas/EFORMArea/Views/EFORM/PDF_ErrorPage");
            }
            finally
            {
                eformdbContext.Dispose();
            }
        }

        [HttpPost]
        public ActionResult PopulateDistrictsbyStateCode(int stateCode)
        {
            try
            {
                CommonFunctions objCommonFunctions = new CommonFunctions();
                List<SelectListItem> lstDist = new List<SelectListItem>();
                lstDist = objCommonFunctions.PopulateDistrict(stateCode, false);
                return Json(lstDist);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "eformcontroller.PopulateDistrictsbyStateCode()");
                return Json(new { string.Empty });
            }
        }



        #region PIU_demo

        [RequiredAuthentication]
        public JsonResult GetWorkList(int? page, int? rows, string sidx, string sord)
        {
            int districtCode = 0, month = 0, year = 0, stateCode = 0;
            string eformStatus = string.Empty;
            string proposalType = string.Empty;
            long totalRecords = 0;
            IPdfDataDAL objDAL = new PdfDataDAL();
            CommonFunctions commonFunction = new CommonFunctions();
            try
            {
                if (!commonFunction.ValidateGridParameters(new GridParams(page, rows, sidx, sord, Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
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
                if (!string.IsNullOrEmpty(Request.Params["stateCode"]))
                {
                    stateCode = Convert.ToInt32(Request.Params["stateCode"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["districtCode"]))
                {
                    districtCode = Convert.ToInt32(Request.Params["districtCode"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["eformStatus"]))
                {
                    eformStatus = Request.Params["eformStatus"];
                }
                if (!string.IsNullOrEmpty(Request.Params["eFormType"]))
                {
                    proposalType = Request.Params["eFormType"];
                }
                var jsonData = new
                {
                    rows = objDAL.GetWorkListDal(month, year, stateCode, districtCode, eformStatus, proposalType, Convert.ToInt32(page) - 1, Convert.ToInt32(rows), sidx, sord, out totalRecords),
                    total = totalRecords <= Convert.ToInt32(rows) ? 1 : totalRecords / Convert.ToInt32(rows) + 1,
                    page = Convert.ToInt32(page),
                    records = totalRecords,
                };
                return Json(jsonData, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "eformController.GetWorkList()");
                return null;
            }
        }

        [RequiredAuthentication]
        public FileResult GetDownloadedPDF(string encRoadCode)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            try
            {
                IPdfDataDAL objDAL = new PdfDataDAL();
                string decRoadCode = string.Empty;
                string qmName = string.Empty;
                string qmType = string.Empty;
                string qmMonth = string.Empty;
                string qmYear = string.Empty;
                string qmCode = string.Empty;
                string QmScheduleCode = string.Empty;
                string eformIDfromGrid = string.Empty;
                string workStatus = string.Empty;
                string proposaltype = string.Empty;
                string[] encParam = encRoadCode.Split('/');
                decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { encParam[0], encParam[1], encParam[2] });
                if (decryptedParameters.Count > 0)
                {
                    decRoadCode = decryptedParameters["imsRoadID"].Split('$')[0];
                    qmName = decryptedParameters["imsRoadID"].Split('$')[1];
                    qmType = decryptedParameters["imsRoadID"].Split('$')[2];
                    qmMonth = decryptedParameters["imsRoadID"].Split('$')[3];
                    qmYear = decryptedParameters["imsRoadID"].Split('$')[4];
                    qmCode = decryptedParameters["imsRoadID"].Split('$')[5];
                    QmScheduleCode = decryptedParameters["imsRoadID"].Split('$')[6];
                    eformIDfromGrid = decryptedParameters["imsRoadID"].Split('$')[7];
                    workStatus = decryptedParameters["imsRoadID"].Split('$')[8];
                    proposaltype = decryptedParameters["imsRoadID"].Split('$')[9];
                }

                int roadCode = Convert.ToInt32(decRoadCode);


                int eformIDfrmGrid = Convert.ToInt32(eformIDfromGrid);


                string Template_File = string.Empty;
                string filepath = ConfigurationManager.AppSettings["TEMPLATE_FILE"].ToString();
                if (workStatus.Equals("C"))
                {
                    Template_File = Path.Combine(filepath, "PIU_TEMPLATE_C.pdf");
                }
                else if (workStatus.Equals("P"))
                {

                    Template_File = Path.Combine(filepath, "PIU_TEMPLATE_P.pdf");

                }
                PdfLoadedDocument loadedDocument = new PdfLoadedDocument(Template_File);
                PdfLoadedForm loadedForm = loadedDocument.Form;
                string templateVersionValue = string.Empty;
                try
                {
                    PdfLoadedTextBoxField templateVersion = loadedForm.Fields["TEMPLATE_VERSION"] as PdfLoadedTextBoxField;
                    templateVersionValue = templateVersion.Text;
                }
                catch (Exception ex)
                {
                    ErrorLog.LogError(ex, "eformController.GetDownloadedPDF()");
                }


                EFORM_DU_LOG LogDetail = new EFORM_DU_LOG();
                string CheckSumString = decRoadCode + qmMonth + qmYear + qmCode + "PIU";
                string CheckSumByteCode = objDAL.ComputeStringToSha256Hash(CheckSumString);
                LogDetail.PDF_CHECKSUM = CheckSumByteCode;
                if (templateVersionValue != "")
                {
                    LogDetail.TEMPLATE_VERSION = templateVersionValue;
                }
                else
                {
                    LogDetail.TEMPLATE_VERSION = "V1.0";

                }
                LogDetail.EFORM_ID = dbContext.EFORM_MASTER.Where(s => s.EFORM_ID == eformIDfrmGrid).Select(d => d.EFORM_ID).FirstOrDefault();
                bool IsInserLogSuccess = objDAL.InsertDownloadLogDetails(LogDetail);




                if (IsInserLogSuccess == false)
                {

                    string error_File = ConfigurationManager.AppSettings["ERROR_FILE"].ToString();
                    string Errorfilename = "Error.pdf";
                    byte[] FileBytesFalse = System.IO.File.ReadAllBytes(error_File);
                    return File(FileBytesFalse, "application/pdf", Errorfilename);
                }


                PIU_GET_PREFILLED_DETAILS imsDetailsList = new PIU_GET_PREFILLED_DETAILS();
                imsDetailsList = objDAL.getPIUPrefilledList(roadCode, eformIDfrmGrid);  //change on 08-07-2022
                int schdcode = Convert.ToInt32(QmScheduleCode);
                int eform_id = Convert.ToInt32(eformIDfromGrid);


                //add on 01-08-2022
                double templateVers = 0;
                try
                {
                    PdfLoadedTextBoxField templateVersion = loadedForm.Fields["TEMPLATE_VERSION"] as PdfLoadedTextBoxField;
                    if (templateVersion.Text != "")
                    {
                        templateVers = Convert.ToDouble(templateVersion.Text.Replace("V", ""));
                    }

                }
                catch (Exception ex)
                {
                    ErrorLog.LogError(ex, "eformController.GetDownloadedPDF()");
                }
                //end add on 01-08-2022


                var list = (from ma in dbContext.EFORM_PIU_PREVIOUS_INSP_DETAILS
                            where ma.EFORM_ID == eform_id && ma.INSERT_OR_UPDATE != "I"
                            select new
                            {
                                ma.INSP_ID,
                                ma.VISIT_DATE,
                                ma.VISITOR_NAME_DESG,
                                ma.ROAD_FROM,
                                ma.ROAD_TO,

                            }).OrderBy(t => t.VISIT_DATE).ToList();



                PreFilledModel model = new PreFilledModel();

                for (int i = 0; i < list.Count && i < 8; i++)
                {
                    if (i == 0)
                    {
                        model.INSP_ID_1 = list[i].INSP_ID.ToString();
                        model.VISIT_DATE_1 = list[i].VISIT_DATE.ToString().Split(' ')[0];
                        model.VISITOR_NAME_DESG_1 = list[i].VISITOR_NAME_DESG.ToString();
                        model.ROAD_FROM_1 = list[i].ROAD_FROM.ToString();
                        model.ROAD_TO_1 = list[i].ROAD_TO.ToString();
                    }
                    if (i == 1)
                    {
                        model.INSP_ID_2 = list[i].INSP_ID.ToString();
                        model.VISIT_DATE_2 = list[i].VISIT_DATE.ToString().Split(' ')[0];
                        model.VISITOR_NAME_DESG_2 = list[i].VISITOR_NAME_DESG.ToString();
                        model.ROAD_FROM_2 = list[i].ROAD_FROM.ToString();
                        model.ROAD_TO_2 = list[i].ROAD_TO.ToString();
                    }
                    if (i == 2)
                    {
                        model.INSP_ID_3 = list[i].INSP_ID.ToString();
                        model.VISIT_DATE_3 = list[i].VISIT_DATE.ToString().Split(' ')[0];
                        model.VISITOR_NAME_DESG_3 = list[i].VISITOR_NAME_DESG.ToString();
                        model.ROAD_FROM_3 = list[i].ROAD_FROM.ToString();
                        model.ROAD_TO_3 = list[i].ROAD_TO.ToString();
                    }
                    if (i == 3)
                    {
                        model.INSP_ID_4 = list[i].INSP_ID.ToString();
                        model.VISIT_DATE_4 = list[i].VISIT_DATE.ToString().Split(' ')[0];
                        model.VISITOR_NAME_DESG_4 = list[i].VISITOR_NAME_DESG.ToString();
                        model.ROAD_FROM_4 = list[i].ROAD_FROM.ToString();
                        model.ROAD_TO_4 = list[i].ROAD_TO.ToString();
                    }
                    if (i == 4)
                    {
                        model.INSP_ID_5 = list[i].INSP_ID.ToString();
                        model.VISIT_DATE_5 = list[i].VISIT_DATE.ToString().Split(' ')[0];
                        model.VISITOR_NAME_DESG_5 = list[i].VISITOR_NAME_DESG.ToString();
                        model.ROAD_FROM_5 = list[i].ROAD_FROM.ToString();
                        model.ROAD_TO_5 = list[i].ROAD_TO.ToString();
                    }
                    if (i == 5)
                    {
                        model.INSP_ID_6 = list[i].INSP_ID.ToString();
                        model.VISIT_DATE_6 = list[i].VISIT_DATE.ToString().Split(' ')[0];
                        model.VISITOR_NAME_DESG_6 = list[i].VISITOR_NAME_DESG.ToString();
                        model.ROAD_FROM_6 = list[i].ROAD_FROM.ToString();
                        model.ROAD_TO_6 = list[i].ROAD_TO.ToString();
                    }
                    if (i == 6)
                    {
                        model.INSP_ID_7 = list[i].INSP_ID.ToString();
                        model.VISIT_DATE_7 = list[i].VISIT_DATE.ToString().Split(' ')[0];
                        model.VISITOR_NAME_DESG_7 = list[i].VISITOR_NAME_DESG.ToString();
                        model.ROAD_FROM_7 = list[i].ROAD_FROM.ToString();
                        model.ROAD_TO_7 = list[i].ROAD_TO.ToString();
                    }
                    if (i == 7)
                    {
                        model.INSP_ID_8 = list[i].INSP_ID.ToString();
                        model.VISIT_DATE_8 = list[i].VISIT_DATE.ToString().Split(' ')[0];
                        model.VISITOR_NAME_DESG_8 = list[i].VISITOR_NAME_DESG.ToString();
                        model.ROAD_FROM_8 = list[i].ROAD_FROM.ToString();
                        model.ROAD_TO_8 = list[i].ROAD_TO.ToString();
                    }
                }

                for (int i = list.Count; i < 8; i++)
                {
                    string inspLevel = "INSP_LEVEL_" + (i + 1);
                    string obs = "OBSERVATIONS_" + (i + 1);
                    string action = "ACTION_" + (i + 1);
                    //add on 01-08-2022
                    if (templateVers < 3.0)
                    {
                        PdfLoadedTextBoxField inspLevelField = loadedForm.Fields[inspLevel] as PdfLoadedTextBoxField;
                        inspLevelField.ReadOnly = true;
                    }
                    else
                    {
                        PdfLoadedComboBoxField inspLevelFieldCbx = loadedForm.Fields[inspLevel] as PdfLoadedComboBoxField;
                        inspLevelFieldCbx.ReadOnly = true;
                        inspLevelFieldCbx.Visibility = Syncfusion.Pdf.Interactive.PdfFormFieldVisibility.Hidden;


                    }
                    //end add on 01-08-2022
                    PdfLoadedTextBoxField obsField = loadedForm.Fields[obs] as PdfLoadedTextBoxField;
                    PdfLoadedTextBoxField actionField = loadedForm.Fields[action] as PdfLoadedTextBoxField;

                    obsField.ReadOnly = true;
                    actionField.ReadOnly = true;
                }

                model.RoadCode = URLEncrypt.EncryptParameters1(new string[] { "encRoadCode=" + imsDetailsList.RoadCode.ToString() + "$" + imsDetailsList.Eform_Id.ToString() });
                model.checksum = CheckSumByteCode;
                model.EFORM_ID = imsDetailsList.Eform_Id.ToString();
                model.WORK_STATUS = workStatus;
                model.CURRENT_STAGE = imsDetailsList.CURRENT_STAGE == "C" ? "3" : (imsDetailsList.STAGE_PHASE == "S1" ? "1" : "2");
                model.QM_NAME = imsDetailsList.QM_NAME;
                model.QM_TYPE = imsDetailsList.QM_TYPE == "S" ? "S" : "N";
                model.STATE = imsDetailsList.State;
                model.DISTRICT = imsDetailsList.District;
                model.BLOCK = imsDetailsList.Block;
                model.ROAD_NAME = imsDetailsList.RoadName;
                model.PACKAGE_NUMBER = imsDetailsList.PkgNumber;
                model.SANCTIONED_LENGTH = imsDetailsList.SancLength.ToString();
                model.EXEC_LENGTH = imsDetailsList.EXEC_LENGTH.ToString();
                model.ESTIMATED_COST = imsDetailsList.ESTIMATED_COST.ToString();
                model.AWARDED_COST = imsDetailsList.AWARDED_COST.ToString();
                if (workStatus.Equals("P"))
                {
                    model.EXPENDITURE_DONE = imsDetailsList.EXPENDITURE_DONE.ToString();
                }
                model.WORK_TYPE = imsDetailsList.WORK_TYPE == null ? "N" : imsDetailsList.WORK_TYPE;
                if (imsDetailsList.TERRAIN_TYPE != null)
                {
                    model.TERRAIN_TYPE = imsDetailsList.TERRAIN_TYPE == "2" ? "R" : (imsDetailsList.TERRAIN_TYPE == "1" ? "P" : (imsDetailsList.TERRAIN_TYPE == "3" ? "H" : "S"));

                }
                else
                {
                    model.TERRAIN_TYPE = imsDetailsList.TERRAIN_TYPE;
                }
                model.AWARD_OF_WORK_DATE = imsDetailsList.AWARD_OF_WORK_DATE.Split(' ')[0];
                model.START_OF_WORK_DATE = imsDetailsList.START_OF_WORK_DATE.Split(' ')[0];
                model.STIPULATED_COMPLETION_DATE = imsDetailsList.STIPULATED_COMPLETION_DATE.Split(' ')[0];



                new CommonFunctions().GeneratePDFPreFilledDataModel(model, loadedForm);

                PdfLoadedTextBoxField TOTAL_LEN_N = loadedForm.Fields["TOTAL_LEN_N"] as PdfLoadedTextBoxField;
                PdfLoadedRadioButtonListField CARRIAGEWAY_WIDTH_NEW = loadedForm.Fields["CARRIAGEWAY_WIDTH_NEW"] as PdfLoadedRadioButtonListField;
                PdfLoadedTextBoxField TOTAL_LEN_U = loadedForm.Fields["TOTAL_LEN_U"] as PdfLoadedTextBoxField;
                PdfLoadedRadioButtonListField CARRIAGEWAY_WIDTH_TYPE = loadedForm.Fields["CARRIAGEWAY_WIDTH_TYPE"] as PdfLoadedRadioButtonListField;
                if (model.WORK_TYPE == "N")
                {

                    TOTAL_LEN_N.ReadOnly = false;
                    CARRIAGEWAY_WIDTH_NEW.ReadOnly = false;
                    TOTAL_LEN_U.ReadOnly = true;
                    CARRIAGEWAY_WIDTH_TYPE.ReadOnly = true;
                }
                else if (model.WORK_TYPE == "U")
                {
                    TOTAL_LEN_N.ReadOnly = true;
                    CARRIAGEWAY_WIDTH_NEW.ReadOnly = true;
                    TOTAL_LEN_U.ReadOnly = false;
                    CARRIAGEWAY_WIDTH_TYPE.ReadOnly = false;
                }
                if (model.WORK_TYPE == null)
                {
                    PdfLoadedRadioButtonListField WORK_TYPE = loadedForm.Fields["WORK_TYPE"] as PdfLoadedRadioButtonListField;
                    WORK_TYPE.ReadOnly = true;
                    TOTAL_LEN_N.ReadOnly = true;
                    CARRIAGEWAY_WIDTH_NEW.ReadOnly = true;
                    TOTAL_LEN_U.ReadOnly = true;
                    CARRIAGEWAY_WIDTH_TYPE.ReadOnly = true;
                }


                if (workStatus.Equals("C"))
                {
                    PdfLoadedTextBoxField complCost = loadedForm.Fields["COMPLETION_COST"] as PdfLoadedTextBoxField;
                    complCost.ReadOnly = false;
                }

                //save prefilled pdf
                string Generated_Path = ConfigurationManager.AppSettings["TEMPORARY_FILES_PATH"].ToString();
                if (!Directory.Exists(Generated_Path))
                    Directory.CreateDirectory(Generated_Path);


                string fileName = eform_id + "PIU_" + System.DateTime.Now.ToString("dd_MM_yyyy_HHmmss") + ".pdf";
                var fullFilePath1 = Path.Combine(Generated_Path, fileName);
                loadedDocument.Save(fullFilePath1);
                loadedDocument.Close(true);
                //Close the document
                byte[] FileBytes = System.IO.File.ReadAllBytes(fullFilePath1);



                try
                {
                    string temp_Path = ConfigurationManager.AppSettings["TEMPORARY_FILES_PATH"];
                    System.IO.DirectoryInfo di = new DirectoryInfo(temp_Path);

                    foreach (FileInfo file in di.GetFiles())
                    {
                        try
                        {
                            file.Delete();
                        }
                        catch (Exception ex)
                        {
                            ErrorLog.LogError(ex, "eformcontroller.GetDownloadedPDF()");
                        }

                    }

                }
                catch (Exception ex)
                {
                    ErrorLog.LogError(ex, "eformcontroller.GetDownloadedPDF()");

                }
                return File(FileBytes, "application/pdf", fileName);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "eformController.GetDownloadedPdf()");
                string error_File = ConfigurationManager.AppSettings["ERROR_FILE"].ToString();
                string Errorfilename = "Error.pdf";
                byte[] FileBytes = System.IO.File.ReadAllBytes(error_File);
                return File(FileBytes, "application/pdf", Errorfilename);
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        [RequiredAuthentication]
        [HttpPost]
        public ActionResult UploadPdfFilePIU(string id)
        {
            string id1 = id;
            IPdfDataDAL objDAL = new PdfDataDAL();
            bool flag = true;
            string responseMessage = string.Empty;
            string fname;
            string uploaded_File, temp_uploaded_File;
            List<SelectListItem> result = new List<SelectListItem>();

            PMGSYEntities eformdbContext = new PMGSYEntities();
            string schdCode = id1.Split('$')[1];
            string schdMonth = id1.Split('$')[2];
            string schdYear = id1.Split('$')[3];
            string workStatus = id1.Split('$')[4];
            string qmCode = id1.Split('$')[5];
            string qmType = id1.Split('$')[6];
            id = id1.Split('$')[0];
            string eformIDFromGrid = id1.Split('$')[7];
            if (Request.Files.Count > 0)
            {
                HttpPostedFileBase file = Request.Files[0];

                List<SelectListItem> validationList = new List<SelectListItem>();
                //add more conditions like file type, file size etc as per your need.
                int fileSizeLimit = Convert.ToInt16(ConfigurationManager.AppSettings["PIU_UPLOADED_FILE_SIZE"]) * 1024 * 1024;
                if (file.ContentLength < fileSizeLimit)
                {
                    if (file != null && file.ContentLength > 0 && Path.GetExtension(file.FileName).ToLower() == ".pdf")
                    {

                        try
                        {


                            int RoadId = Convert.ToInt32(id);

                            fname = eformIDFromGrid + "_PIU_uploaded_" + System.DateTime.Now.ToString("dd_MM_yyyy_HHmmss") + ".pdf";
                            string temp_Uploaded_Path = ConfigurationManager.AppSettings["TEMPORARY_FILES_PATH"];
                            flag = true;
                            if (!Directory.Exists(temp_Uploaded_Path))
                                Directory.CreateDirectory(temp_Uploaded_Path);

                            temp_uploaded_File = Path.Combine(temp_Uploaded_Path, fname);
                            file.SaveAs(temp_uploaded_File);


                            String Uploaded_Path = ConfigurationManager.AppSettings["PIU_UPLOADED_PATH"];
                            if (!Directory.Exists(Uploaded_Path))
                                Directory.CreateDirectory(Uploaded_Path);
                            uploaded_File = Path.Combine(Uploaded_Path, fname);


                            PdfLoadedDocument loadedDocument = new PdfLoadedDocument(temp_uploaded_File);
                            PdfLoadedForm loadedForm = loadedDocument.Form;
                            try
                            {
                                PdfLoadedTextBoxField RoadCode1 = loadedForm.Fields["RoadCode"] as PdfLoadedTextBoxField;
                            }
                            catch (Exception e)
                            {
                                var roadAlert = "";

                                roadAlert = "Please Fill the downloaded e-Form Pdf in adobe acrobat reader dc and then upload.";
                                validationList.Add(new SelectListItem { Text = roadAlert });
                                var validationData1 = validationList.Select(x => x.Text).ToList();
                                flag = false;
                                ErrorLog.LogError(e, "eformController.UploadPdfFilePIU()");
                                return Json(new { success = flag, data = validationData1, responseMsg = roadAlert });

                            }


                            PdfLoadedTextBoxField RoadCode = loadedForm.Fields["RoadCode"] as PdfLoadedTextBoxField;
                            PdfLoadedTextBoxField checksum = loadedForm.Fields["checksum"] as PdfLoadedTextBoxField;
                            string decRoadCodetemp = string.Empty;
                            string decRoadCode = string.Empty;
                            string eformIdfrompdf = string.Empty;
                            string[] encRoadCode = RoadCode.Text.ToString().Split('/');
                            if (encRoadCode.Length > 1)
                            {
                                decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { encRoadCode[0], encRoadCode[1], encRoadCode[2] });

                                if (decryptedParameters.Count > 0)
                                {
                                    decRoadCodetemp = decryptedParameters["encRoadCode"];
                                    decRoadCode = decRoadCodetemp.Split('$')[0];
                                    eformIdfrompdf = decRoadCodetemp.Split('$')[1];
                                }
                            }
                            else
                            {
                                decRoadCode = RoadCode.Text;
                            }



                            double templateVers = 0;
                            try
                            {
                                PdfLoadedTextBoxField templateVersion = loadedForm.Fields["TEMPLATE_VERSION"] as PdfLoadedTextBoxField;
                                if (templateVersion.Text != "")
                                {
                                    templateVers = Convert.ToDouble(templateVersion.Text.Replace("V", ""));
                                }

                            }
                            catch (Exception ex)
                            {
                                ErrorLog.LogError(ex, "eformController.UploadPdfFilePIU");
                            }

                            if (!decRoadCode.Equals(id))
                            {

                                var roadAlert = "";
                                var str = eformdbContext.IMS_SANCTIONED_PROJECTS.Where(x => x.IMS_PR_ROAD_CODE == RoadId).Select(u => new { u.IMS_ROAD_NAME }).Single();
                                roadAlert = " Please Upload correct Pdf for road " + str.IMS_ROAD_NAME;

                                validationList.Add(new SelectListItem { Text = roadAlert });

                                var validationData1 = validationList.Select(x => x.Text).ToList();
                                flag = false;
                                return Json(new { success = flag, data = validationData1, responseMsg = roadAlert });
                            }

                            if (!eformIdfrompdf.Equals(eformIDFromGrid))
                            {
                                string eformIdAlert = " Please Upload correct Pdf for road " + eformIDFromGrid;

                                validationList.Add(new SelectListItem { Text = eformIdAlert });

                                var validationData1 = validationList.Select(x => x.Text).ToList();
                                flag = false;
                                return Json(new { success = flag, data = validationData1, responseMsg = eformIdAlert });
                            }

                            int roadCodeChck = Convert.ToInt32(decRoadCode);
                            int eformId = Convert.ToInt32(eformIDFromGrid);
                            string checksumfrmDB = eformdbContext.EFORM_DU_LOG.Where(s => s.EFORM_ID == eformId && s.USER_TYPE == "P").OrderByDescending(p => p.LOG_ID).Select(m => m.PDF_CHECKSUM).FirstOrDefault();
                            //  string RoadStatus = eformdbContext.EFORM_PREFILLED_DETAILS.Where(s => s.EFORM_ID == eformId).Select(s => s.PHYSICAL_WORK_STATUS).FirstOrDefault();
                            bool TemplateStatus = false;
                            if (workStatus == "C")
                            {
                                TemplateStatus = true;
                            }



                            if (!checksum.Text.Equals(checksumfrmDB))
                            {
                                string roadAlert = "Please upload latest downloaded pdf";
                                validationList.Add(new SelectListItem { Text = "Please upload latest downloaded pdf" });
                                var validationData1 = validationList.Select(x => x.Text).ToList();
                                flag = false;
                                return Json(new { success = flag, data = validationData1, responseMsg = roadAlert });
                            }

                            loadedDocument.Close(true);


                            EFORM_PIU_VIEWMODEL objViewModel = PdfDataToClassModel(temp_uploaded_File, TemplateStatus, eformIDFromGrid, templateVers);

                            StringBuilder strErrorMessage = new StringBuilder("");
                            int count = 0;
                            if (objViewModel.ErrorOccured)
                            {
                                objViewModel.ErrorList = objViewModel.ErrorList.Distinct().ToList();
                                #region 31-03-2022
                                List<string> errorslist = new List<string>();
                                for (int i = 1; i <= 5; i++)
                                {
                                    string Errorpage = "Page-";
                                    errorslist = objViewModel.ErrorList.Where(a => a.Contains(string.Concat(Errorpage + i))).ToList();


                                    if (errorslist.Count > 0)
                                    {
                                        strErrorMessage.Append("<h4 style='background-color:#efede5; color:black'> Page " + i + " </h4>");

                                    }

                                    foreach (var ierror in errorslist)
                                    {
                                        strErrorMessage.Append("<li style='margin-left:10px; list-style-type: square'> ");
                                        strErrorMessage.Append("Error-" + ++count + " " + ierror);
                                        strErrorMessage.Append("<br />");
                                        strErrorMessage.Append("</li>");
                                    }
                                }
                                #endregion



                                return Json(new { success = false, data = strErrorMessage.ToString(), IsValidationError = true, responseMsg = "Please fill all mandatory details in file" });
                            }
                            //save to db

                            result = objDAL.SaveToDb(temp_uploaded_File, eformIDFromGrid, fname, Uploaded_Path, file, uploaded_File, objViewModel, schdCode, schdMonth, schdYear, workStatus, qmCode, qmType, templateVers);

                            var validationData = result.Select(x => x.Text).ToList();

                            if (result.Count > 0)
                            {
                                responseMessage = "Error occured while uploading pdf";
                                flag = false;
                                return Json(new { success = flag, data = validationData, responseMsg = responseMessage });
                            }
                            else
                            {
                                loadedDocument.Close();
                                responseMessage = "Upload Successful.";
                            }


                        }
                        catch (Exception ex)
                        {
                            ErrorLog.LogError(ex, "eformcontroller.UploadPdfFilePIU()");
                            responseMessage = "Error occured while uploading pdf, please contact OMMAS team.";
                            validationList.Add(new SelectListItem { Text = "Error occured while uploading pdf, please contact OMMAS team." });
                            var validationData1 = validationList.Select(x => x.Text).ToList();
                            flag = false;
                            return Json(new { success = flag, data = validationData1, responseMsg = responseMessage });
                        }
                        finally
                        {
                            file.InputStream.Flush();
                            file.InputStream.Close();
                            eformdbContext.Dispose();
                        }

                    }
                    else
                    {
                        flag = false;
                        responseMessage = "File is invalid.";
                        validationList.Add(new SelectListItem { Text = "File is invalid." });
                        var validationData1 = validationList.Select(x => x.Text).ToList();
                        return Json(new { success = flag, data = validationData1, responseMsg = responseMessage });
                    }
                }
                else
                {
                    flag = false;
                    responseMessage = "Invalid file size. Please upload file upto 6 MB.";
                    validationList.Add(new SelectListItem { Text = "Invalid file size. Please upload file upto 6 MB." });
                    var validationData1 = validationList.Select(x => x.Text).ToList();
                    return Json(new { success = flag, data = validationData1, responseMsg = responseMessage });
                }
            }
            else
            {
                flag = false;
                responseMessage = "File Upload has no file.";
            }

            return Json(new { success = flag, message = responseMessage }, JsonRequestBehavior.AllowGet);
        }

        [RequiredAuthentication]
        public ActionResult ViewPdfSavedData(String encId)
        {
            try
            {
                string[] encParam = encId.Split('/');
                decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { encParam[0], encParam[1], encParam[2] });
                int id = 0;
                if (decryptedParameters.Count > 0)
                {
                    id = Convert.ToInt32(decryptedParameters["imsRoadID"].Split('$')[0]);
                }

                IPdfDataDAL objDAL = new PdfDataDAL();
                EFORM_PIU_VIEWMODEL objPIU_Model = objDAL.ViewPIU_PdfSavedData(id);

                return View("~/Areas/EFORMArea/Views/EFORM/Preview_PIU_SavedData.cshtml", objPIU_Model);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "eformcontroller.ViewPdfSavedData()");
                return View("~/Areas/EFORMArea/Views/EFORM/PDF_ErrorPage");
            }
        }

        [RequiredAuthentication]
        public JsonResult deleteEformDetail(string id)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            string roadIDdec = id.Split('$')[0];
            string eformIddec = id.Split('$')[1];
            int roadId = Convert.ToInt32(roadIDdec);
            string message = string.Empty;
            try
            {
                int eformId = Convert.ToInt32(eformIddec);
                EFORM_MASTER model = dbContext.EFORM_MASTER.Where(s => s.EFORM_ID == eformId).FirstOrDefault();
                var fileName = dbContext.EFORM_PDF_UPLOAD_DETAIL.Where(x => x.PR_ROAD_CODE == roadId && x.EFORM_ID == eformId && x.USER_TYPE == "P").Select(s => s.FILE_NAME).FirstOrDefault();
                //  string Uploaded_Path = dbContext.EFORM_PDF_UPLOAD_DETAIL.Where(x => x.PR_ROAD_CODE == roadId && x.EFORM_ID == eformId && x.USER_TYPE == "P").Select(s => s.FILE_PATH).FirstOrDefault();
                string Uploaded_Path = ConfigurationManager.AppSettings["PIU_UPLOADED_PATH"].ToString() + "/" + model.ADMIN_SCHEDULE_YEAR.ToString() + "/" + model.ADMIN_SCHEDULE_MONTH.ToString();
                using (var scope = new TransactionScope())
                {
                    dbContext.USP_EFORMS_DELETE_INSPECTION_DETAILS(roadId, eformId);
                    var insp_records = dbContext.EFORM_PIU_PREVIOUS_INSP_DETAILS.Where(x => x.EFORM_ID == eformId && x.INSERT_OR_UPDATE == "I");
                    dbContext.EFORM_PIU_PREVIOUS_INSP_DETAILS.RemoveRange(insp_records);
                    dbContext.SaveChanges();
                    var inspRecordsUpdate = dbContext.EFORM_PIU_PREVIOUS_INSP_DETAILS.Where(x => x.EFORM_ID == eformId && x.INSERT_OR_UPDATE == "U");
                    foreach (var item in inspRecordsUpdate)
                    {
                        item.INSP_LEVEL = null;
                        item.OBSERVATIONS = null;
                        item.ACTION = null;
                        dbContext.Entry(item).State = EntityState.Modified;
                        dbContext.SaveChanges();
                    }
                    var tech_records = dbContext.EFORM_PIU_NEW_TECHNOLOGY_DETAILS.Where(x => x.EFORM_ID == eformId);
                    dbContext.EFORM_PIU_NEW_TECHNOLOGY_DETAILS.RemoveRange(tech_records);
                    dbContext.SaveChanges();
                    EFORM_PREFILLED_DETAILS prefData = dbContext.EFORM_PREFILLED_DETAILS.Where(s => s.EFORM_ID == eformId).FirstOrDefault();
                    prefData.COMPLETION_COST = Convert.ToDecimal(0.0);
                    dbContext.Entry(prefData).State = EntityState.Modified;
                    dbContext.SaveChanges();
                    string fullPath = Path.Combine(Uploaded_Path, fileName);
                    FileInfo file = new FileInfo(fullPath);
                    if (file.Exists)
                    {
                        file.Delete();
                    }
                    scope.Complete();
                    return Json(new { success = true }, JsonRequestBehavior.AllowGet);
                }

            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "eformcontroller.deleteEformDetail()");
                return Json(new { success = false, message = "error occured during deleting the pdf." }, JsonRequestBehavior.AllowGet);
            }
            finally
            {
                dbContext.Dispose();
            }

        }



        [RequiredAuthentication]
        public JsonResult DeleteTempFilePIU(string id)
        {

            try
            {
                string temp_Path = ConfigurationManager.AppSettings["TEMPORARY_FILES_PATH"];
                System.IO.DirectoryInfo di = new DirectoryInfo(temp_Path);

                foreach (FileInfo file in di.GetFiles())
                {
                    try
                    {
                        file.Delete();
                    }
                    catch (Exception ex)
                    {
                        ErrorLog.LogError(ex, "eformcontroller.DeleteTempFilePIU()");
                    }

                }

            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "eformcontroller.DeleteTempFilePIU()");
                return Json(new { success = true }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { success = true }, JsonRequestBehavior.AllowGet);
        }




        [RequiredAuthentication]
        public JsonResult finalizeEformDetail(int id)
        {
            bool flag = true;
            int detailId = Convert.ToInt32(id);
            PMGSYEntities dbContext = new PMGSYEntities();
            try
            {
                using (TransactionScope ts = new TransactionScope())
                {
                    EFORM_PDF_UPLOAD_DETAIL obj = dbContext.EFORM_PDF_UPLOAD_DETAIL.Where(s => s.EFORM_ID == detailId && s.USER_TYPE == "P").FirstOrDefault();
                    obj.IS_FINALISED = "Y";
                    dbContext.Entry(obj).State = EntityState.Modified;
                    dbContext.SaveChanges();
                    ts.Complete();
                }

            }
            catch (Exception ex)
            {
                flag = false;
                ErrorLog.LogError(ex, "eformController.finalizeEformDetail()");
            }
            finally
            {
                dbContext.Dispose();
            }
            return Json(new { success = flag }, JsonRequestBehavior.AllowGet);
        }


        [RequiredAuthentication]
        public JsonResult IsPDFFileavaialble(string id)
        {
            bool flag = false;
            string message = string.Empty;
            PMGSYEntities dbContext = new PMGSYEntities();
            try
            {
                int eformId = Convert.ToInt32(id.Split('$')[7]);
                var fileName = dbContext.EFORM_PDF_UPLOAD_DETAIL.Where(x => x.EFORM_ID == eformId && x.USER_TYPE == "P").Select(s => s.FILE_NAME).FirstOrDefault();
                if (fileName == null)
                {

                    return Json(new { success = flag }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    flag = true;
                    message = "Pdf is already uploaded. If you want to upload again, please delete the existing file";
                }


            }
            catch (Exception ex)
            {
                flag = true;
                message = "Error occured during uploading file";
                ErrorLog.LogError(ex, "eformcontroller.IsPDFFileavaialble()");

            }
            finally
            {
                dbContext.Dispose();
            }
            return Json(new { success = flag, message = message }, JsonRequestBehavior.AllowGet);
        }

        [RequiredAuthentication]
        public FileResult GetReport(string idtemp)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            try
            {

                string id = null;
                string[] encParam = idtemp.Split('/');
                decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { encParam[0], encParam[1], encParam[2] });
                if (decryptedParameters.Count > 0)
                {
                    id = decryptedParameters["imsRoadID"];
                }

                int eformId = Convert.ToInt32(id);
              
              


                EFORM_MASTER model = dbContext.EFORM_MASTER.Where(s => s.EFORM_ID == eformId).FirstOrDefault();
                string uploaded_path = ConfigurationManager.AppSettings["PIU_UPLOADED_PATH"].ToString() + "/" + model.ADMIN_SCHEDULE_YEAR.ToString() + "/" + model.ADMIN_SCHEDULE_MONTH.ToString();
                // string uploaded_path = dbContext.EFORM_PDF_UPLOAD_DETAIL.Where(x => x.EFORM_ID == eformId && x.USER_TYPE == "P").Select(s => s.FILE_PATH).FirstOrDefault();
                var fileName = dbContext.EFORM_PDF_UPLOAD_DETAIL.Where(x => x.EFORM_ID == eformId && x.USER_TYPE == "P").Select(s => s.FILE_NAME).FirstOrDefault();
                var nameReport = id + "_" + "Part-1_Report.pdf";
                string fullPath = Path.Combine(uploaded_path, fileName);
                byte[] FileBytes = System.IO.File.ReadAllBytes(fullPath);
                return File(FileBytes, "application/pdf", nameReport);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "eformController.GetReport()");
                return null;
            }
            finally
            {
                dbContext.Dispose();
            }

        }


        [RequiredAuthentication]
        public JsonResult viewPart1PdfVirtualDir(string id)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            try
            {

                int eformId = Convert.ToInt32(id);
                #region to save EFORM_QM_QC_TEST_DETAILS & EFORM_QM_TEST_RESULT_VERIFICATION_DETAILS for version 2.0
                try
                {

                    EFORM_PDF_UPLOAD_DETAIL uploadDetail = dbContext.EFORM_PDF_UPLOAD_DETAIL.Where(s => s.EFORM_ID == eformId && s.USER_TYPE == "Q" && s.TEMPLATE_VERSION == "V2.0").FirstOrDefault();
                    if (uploadDetail != null && uploadDetail.UPLOAD_DATE < Convert.ToDateTime("17/02/2023"))
                    {
                        var fname = uploadDetail.FILE_NAME;
                        var temp_Uploaded_Path = uploadDetail.FILE_PATH;
                        var temp_uploaded_File = Path.Combine(temp_Uploaded_Path, fname);
                        PdfLoadedDocument loadedDocument = new PdfLoadedDocument(temp_uploaded_File);
                        PdfLoadedForm loadedForm = loadedDocument.Form;
                        EFORM_QM_VIEWMODEL objViewModel = new EFORM_QM_VIEWMODEL();
                        CommonFunctions objCommonFunction = new CommonFunctions();
                        List<string> errorListtemp = new List<string>();
                        objViewModel.ErrorList = new List<string>();

                        EFORM_MASTER master = dbContext.EFORM_MASTER.Where(s => s.EFORM_ID == eformId).FirstOrDefault();
                        PdfLoadedCheckBoxField CheckFieldItem3 = loadedForm.Fields["Item_3"] as PdfLoadedCheckBoxField;
                        if (CheckFieldItem3.Checked)
                        {
                            #region QM_QC_TEST_DETAILS_Temp2_0
                            objViewModel.QM_QC_TEST_DETAILS_Temp2_0 = new List<QMModels.EFORM_QM_QC_TEST_DETAILS_Temp2_0>();// add at 28 - 07 - 2022

                            if (!dbContext.EFORM_QM_QC_TEST_DETAILS.Any(s => s.EFORM_ID == eformId))
                            {
                                dbContext.EFORM_MASTER_WORK_ITEM.Where(m => m.ITEM_FLAG == "A").Select(t => t.ITEM_ID).ToList().ForEach(itemID =>
                                {
                                    for (int i = 1; i < 4; i++)
                                    {
                                        QMModels.EFORM_QM_QC_TEST_DETAILS_Temp2_0 detailedItem = new QMModels.EFORM_QM_QC_TEST_DETAILS_Temp2_0();
                                        detailedItem.ITEM_ID = itemID;
                                        detailedItem.ROW_ID = i;
                                        objViewModel.QM_QC_TEST_DETAILS_Temp2_0.Add(detailedItem);
                                    }

                                });

                                int countTemp = 1;
                                foreach (var item in objViewModel.QM_QC_TEST_DETAILS_Temp2_0.ToList())
                                {

                                    string v = "cb_8_1_" + countTemp;
                                    PdfLoadedCheckBoxField CheckField = loadedForm.Fields[v] as PdfLoadedCheckBoxField;
                                    if (CheckField.Checked)
                                    {
                                        errorListtemp = objCommonFunction.FetchPDFFilledDataToQMModel(item, loadedForm);
                                        objViewModel.ErrorList.AddRange(errorListtemp);

                                        if ((item.ROW_ID == 2 || item.ROW_ID == 3) && item.IS_TESTING_ADEQUATE == null && item.REQD_TEST_NUMBER == null && item.TEST_NAME == null && item.CONDUCTED_TEST_NUMBER == null)
                                        {
                                            objViewModel.QM_QC_TEST_DETAILS_Temp2_0.Remove(item);
                                        }

                                    }
                                    else
                                    {
                                        objViewModel.QM_QC_TEST_DETAILS_Temp2_0.Remove(item);
                                    }
                                    if (item.ROW_ID == 3)
                                    {
                                        countTemp++;
                                    }

                                }
                                if (objViewModel.QM_QC_TEST_DETAILS_Temp2_0.Count > 2)
                                {
                                    for (int i = 1; i < objViewModel.QM_QC_TEST_DETAILS_Temp2_0.Count; i++)
                                    {
                                        if (objViewModel.QM_QC_TEST_DETAILS_Temp2_0[i].EXECUTED_QUANTITY == null)
                                        {
                                            objViewModel.QM_QC_TEST_DETAILS_Temp2_0[i].EXECUTED_QUANTITY = objViewModel.QM_QC_TEST_DETAILS_Temp2_0[i - 1].EXECUTED_QUANTITY;
                                        }
                                        if (objViewModel.QM_QC_TEST_DETAILS_Temp2_0[i].DPR_QUANTITY == null)
                                        {
                                            objViewModel.QM_QC_TEST_DETAILS_Temp2_0[i].DPR_QUANTITY = objViewModel.QM_QC_TEST_DETAILS_Temp2_0[i - 1].DPR_QUANTITY;
                                        }
                                    }
                                }

                            }
                            using (var scope = new TransactionScope())
                            {
                                for (int i = 0; i < objViewModel.QM_QC_TEST_DETAILS_Temp2_0.Count; i++)
                                {
                                    int WorkID = dbContext.EFORM_QM_QC_TEST_DETAILS.Any() ? (from item in dbContext.EFORM_QM_QC_TEST_DETAILS select item.WORK_ID).Max() + 1 : 1;

                                    PMGSY.Models.EFORM_QM_QC_TEST_DETAILS QCDetail = new PMGSY.Models.EFORM_QM_QC_TEST_DETAILS();
                                    int t = i + 1;
                                    QCDetail.WORK_ID = WorkID;
                                    QCDetail.PR_ROAD_CODE = master.IMS_PR_ROAD_CODE;
                                    QCDetail.EFORM_ID = master.EFORM_ID;
                                    QCDetail.QM_USER_ID = PMGSYSession.Current.UserId;
                                    QCDetail.IPADD = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                                    QCDetail.ITEM_ID = objViewModel.QM_QC_TEST_DETAILS_Temp2_0[i].ITEM_ID;
                                    QCDetail.INFO_ID = dbContext.EFORM_QM_QUALITY_ATTENTION.Where(x => x.EFORM_ID == eformId).Select(x => x.INFO_ID).FirstOrDefault();
                                    QCDetail.DPR_QUANTITY = objViewModel.QM_QC_TEST_DETAILS_Temp2_0[i].DPR_QUANTITY;
                                    QCDetail.EXECUTED_QUANTITY = objViewModel.QM_QC_TEST_DETAILS_Temp2_0[i].EXECUTED_QUANTITY;
                                    QCDetail.TEST_NAME = objViewModel.QM_QC_TEST_DETAILS_Temp2_0[i].TEST_NAME;
                                    QCDetail.REQD_TEST_NUMBER = objViewModel.QM_QC_TEST_DETAILS_Temp2_0[i].REQD_TEST_NUMBER;
                                    QCDetail.CONDUCTED_TEST_NUMBER = objViewModel.QM_QC_TEST_DETAILS_Temp2_0[i].CONDUCTED_TEST_NUMBER;
                                    QCDetail.IS_TESTING_ADEQUATE = objViewModel.QM_QC_TEST_DETAILS_Temp2_0[i].IS_TESTING_ADEQUATE;
                                    dbContext.EFORM_QM_QC_TEST_DETAILS.Add(QCDetail);
                                    dbContext.SaveChanges();
                                    WorkID++;
                                }
                                scope.Complete();
                            }
                            #endregion

                            #region EFORM_QM_TEST_RESULT_VERIFICATION_DETAILS
                            objViewModel.QM_TEST_RESULT_VERIFICATION_DETAILS_List_Temp_2_0 = new List<QMModels.EFORM_QM_TEST_RESULT_VERIFICATION_DETAILS_Temp2_0>();   //add on 28-07-2022
                            VerificationModelListTempV2_0 verificationModelListTempV2_0 = new VerificationModelListTempV2_0();   //add at 28-07-2022
                            if (!dbContext.EFORM_QM_TEST_RESULT_VERIFICATION_DETAILS.Any(s => s.EFORM_ID == eformId))
                            {
                                verificationModelListTempV2_0.VerificationList.ForEach(item =>
                                {

                                    for (int i = 1; i <= item.RowCount; i++)
                                    {
                                        QMModels.EFORM_QM_TEST_RESULT_VERIFICATION_DETAILS_Temp2_0 detailedItem = new QMModels.EFORM_QM_TEST_RESULT_VERIFICATION_DETAILS_Temp2_0();
                                        detailedItem.VerificationType = item.VerificationType;
                                        detailedItem.RowID = i;
                                        objViewModel.QM_TEST_RESULT_VERIFICATION_DETAILS_List_Temp_2_0.Add(detailedItem);
                                    }

                                });
                                int countChecked = 0;
                                int countUnchecked = 0;
                                foreach (var item in objViewModel.QM_TEST_RESULT_VERIFICATION_DETAILS_List_Temp_2_0.ToList())
                                //  objViewModel.QM_TEST_RESULT_VERIFICATION_DETAILS_List.ForEach(item =>
                                {
                                    if (item.RowID == 1)
                                    {
                                        errorListtemp = objCommonFunction.FetchPDFFilledDataToQMModel(item, loadedForm);
                                        objViewModel.ErrorList.AddRange(errorListtemp);
                                    }
                                    else
                                    {

                                        string v = "cbrd_3_2_" + item.RowID;
                                        PdfLoadedCheckBoxField CheckField = loadedForm.Fields[v] as PdfLoadedCheckBoxField;

                                        if (CheckField.Checked)
                                        {
                                            countChecked++;
                                            errorListtemp = objCommonFunction.FetchPDFFilledDataToQMModel(item, loadedForm);
                                            objViewModel.ErrorList.AddRange(errorListtemp);
                                            if (item.RowID == 2 || item.RowID == 3 || item.RowID == 4)
                                            {
                                                objViewModel.QM_TEST_RESULT_VERIFICATION_DETAILS_List_Temp_2_0[countChecked].ROAD_LOC = objViewModel.QM_TEST_RESULT_VERIFICATION_DETAILS_List_Temp_2_0[0].ROAD_LOC;
                                            }
                                            else if (item.RowID == 6 || item.RowID == 7 || item.RowID == 8)
                                            {
                                                objViewModel.QM_TEST_RESULT_VERIFICATION_DETAILS_List_Temp_2_0[countChecked].ROAD_LOC = objViewModel.QM_TEST_RESULT_VERIFICATION_DETAILS_List_Temp_2_0[4 - countUnchecked].ROAD_LOC;
                                            }
                                            else if (item.RowID == 10 || item.RowID == 11 || item.RowID == 12)
                                            {
                                                objViewModel.QM_TEST_RESULT_VERIFICATION_DETAILS_List_Temp_2_0[countChecked].ROAD_LOC = objViewModel.QM_TEST_RESULT_VERIFICATION_DETAILS_List_Temp_2_0[8 - countUnchecked].ROAD_LOC;
                                            }
                                        }
                                        else
                                        {
                                            countUnchecked++;
                                            objViewModel.QM_TEST_RESULT_VERIFICATION_DETAILS_List_Temp_2_0.Remove(item);
                                        }
                                    }


                                }

                            }
                            using (var scope = new TransactionScope())
                            {
                                for (int i = 0; i < objViewModel.QM_TEST_RESULT_VERIFICATION_DETAILS_List_Temp_2_0.Count; i++)
                                {
                                    int TestID = dbContext.EFORM_QM_TEST_RESULT_VERIFICATION_DETAILS.Any() ? (from item in dbContext.EFORM_QM_TEST_RESULT_VERIFICATION_DETAILS select item.TEST_ID).Max() + 1 : 1;

                                    PMGSY.Models.EFORM_QM_TEST_RESULT_VERIFICATION_DETAILS TestVerification = new PMGSY.Models.EFORM_QM_TEST_RESULT_VERIFICATION_DETAILS();
                                    int t = i + 1;
                                    TestVerification.TEST_ID = TestID;
                                    TestVerification.INFO_ID = dbContext.EFORM_QM_QUALITY_ATTENTION.Where(x => x.EFORM_ID == eformId).Select(x => x.INFO_ID).FirstOrDefault(); // required foreign key
                                    TestVerification.IPADD = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                                    TestVerification.EFORM_ID = master.EFORM_ID;
                                    TestVerification.PR_ROAD_CODE = master.IMS_PR_ROAD_CODE;
                                    TestVerification.QM_USER_ID = PMGSYSession.Current.UserId;
                                    TestVerification.ROAD_LOC = Convert.ToDecimal(objViewModel.QM_TEST_RESULT_VERIFICATION_DETAILS_List_Temp_2_0[i].ROAD_LOC);
                                    TestVerification.TEST_NAME = objViewModel.QM_TEST_RESULT_VERIFICATION_DETAILS_List_Temp_2_0[i].TEST_NAME;
                                    TestVerification.TEST_CONDUCTED_RESULT = objViewModel.QM_TEST_RESULT_VERIFICATION_DETAILS_List_Temp_2_0[i].TEST_CONDUCTED_RESULT;
                                    TestVerification.TEST_RESULT_QCR1 = objViewModel.QM_TEST_RESULT_VERIFICATION_DETAILS_List_Temp_2_0[i].TEST_RESULT_QCR1;
                                    TestVerification.TEST_RESULT_PREVIOUS = objViewModel.QM_TEST_RESULT_VERIFICATION_DETAILS_List_Temp_2_0[i].TEST_RESULT_PREVIOUS;
                                    TestVerification.TEST_RESULT_CONFRM = objViewModel.QM_TEST_RESULT_VERIFICATION_DETAILS_List_Temp_2_0[i].TEST_RESULT_CONFRM;
                                    dbContext.EFORM_QM_TEST_RESULT_VERIFICATION_DETAILS.Add(TestVerification);
                                    dbContext.SaveChanges();
                                    TestID++;

                                }
                                scope.Complete();
                            }

                            #endregion


                        }




                    }
                }
                catch (Exception ex)
                {
                    ErrorLog.LogError(ex, "PdfDataDAL.ViewQMPdfSavedData");

                }


                #endregion


                string uploaded_path = dbContext.EFORM_PDF_UPLOAD_DETAIL.Where(x => x.EFORM_ID == eformId && x.USER_TYPE == "P").Select(s => s.FILE_PATH).FirstOrDefault();
                EFORM_MASTER model = dbContext.EFORM_MASTER.Where(s => s.EFORM_ID == eformId).FirstOrDefault();
                var fileName = dbContext.EFORM_PDF_UPLOAD_DETAIL.Where(x => x.EFORM_ID == eformId && x.USER_TYPE == "P").Select(s => s.FILE_NAME).FirstOrDefault();


                string VirtualDirectoryPath = ConfigurationManager.AppSettings["PIU_UPLOADED_EFORM_VIRTUAL_DIR_PATH"].ToString();
                string VirtualDirectoryfullPath = Path.Combine(VirtualDirectoryPath, Convert.ToString(model.ADMIN_SCHEDULE_YEAR.ToString() + "/" + model.ADMIN_SCHEDULE_MONTH.ToString()), fileName.ToString()).ToString().Replace(@"\\", @"//").Replace(@"\", @"/");

                string physicalFullPath = Path.Combine(uploaded_path, fileName);

                FileInfo file = new FileInfo(physicalFullPath);
                if (file.Exists)
                {
                    return Json(new { success = true, Message = VirtualDirectoryfullPath }, JsonRequestBehavior.AllowGet);

                }
                else
                {
                    return Json(new { success = false, Message = "File not found" }, JsonRequestBehavior.AllowGet);
                }

            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "eformController.viewPart1PdfVirtualDir()");
                return null;
            }
            finally
            {
                dbContext.Dispose();
            }

        }

        #endregion

        #region NQM_SQM_demo
        [RequiredAuthentication]
        public JsonResult GetQMWorkList(int? page, int? rows, string sidx, string sord)
        {
            int districtCode = 0, month = 0, year = 0, stateCode = 0;
            string eformStatus = string.Empty;
            string eFormType = string.Empty;
            long totalRecords = 0;
            IPdfDataDAL objDAL = new PdfDataDAL();
            CommonFunctions commonFunction = new CommonFunctions();
            try
            {
                if (!commonFunction.ValidateGridParameters(new GridParams(page, rows, sidx, sord, Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
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
                if (!string.IsNullOrEmpty(Request.Params["stateCode"]))
                {
                    stateCode = Convert.ToInt32(Request.Params["stateCode"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["districtCode"]))
                {
                    districtCode = Convert.ToInt32(Request.Params["districtCode"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["eformStatus"]))
                {
                    eformStatus = Request.Params["eformStatus"];
                }
                if (!string.IsNullOrEmpty(Request.Params["eFormType"]))
                {
                    eFormType = Request.Params["eFormType"];
                }
                var jsonData = new
                {
                    rows = objDAL.GetQMWorkListDal(month, year, stateCode, districtCode, eformStatus, eFormType, Convert.ToInt32(page) - 1, Convert.ToInt32(rows), sidx, sord, out totalRecords),
                    total = totalRecords <= Convert.ToInt32(rows) ? 1 : totalRecords / Convert.ToInt32(rows) + 1,
                    page = Convert.ToInt32(page),
                    records = totalRecords,
                };
                return Json(jsonData, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "eformController.GetQMWorkList()");
                return null;
            }
        }

        [RequiredAuthentication]
        public FileResult GetDownloadedQMPDF(string encRoadCode)
        {
            PMGSYEntities dbContext = new PMGSYEntities();

            try
            {

                IPdfDataDAL objDAL = new PdfDataDAL();
                string decRoadCode = string.Empty;
                string qmName = string.Empty;
                string qmType = string.Empty;
                string qmMonth = string.Empty;
                string qmYear = string.Empty;
                string qmCode = string.Empty;
                string QmScheduleCode = string.Empty;
                string eformIdFrom_grid = string.Empty;
                string workStatus = string.Empty;
                string[] encParam = encRoadCode.Split('/');
                decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { encParam[0], encParam[1], encParam[2] });
                if (decryptedParameters.Count > 0)
                {
                    decRoadCode = decryptedParameters["imsRoadID"].Split('$')[0];
                    qmName = decryptedParameters["imsRoadID"].Split('$')[1];
                    qmType = decryptedParameters["imsRoadID"].Split('$')[2];
                    qmMonth = decryptedParameters["imsRoadID"].Split('$')[3];
                    qmYear = decryptedParameters["imsRoadID"].Split('$')[4];
                    qmCode = decryptedParameters["imsRoadID"].Split('$')[5];
                    QmScheduleCode = decryptedParameters["imsRoadID"].Split('$')[6];
                    eformIdFrom_grid = decryptedParameters["imsRoadID"].Split('$')[7];
                    workStatus = decryptedParameters["imsRoadID"].Split('$')[8];

                }

                int eform_Id = Convert.ToInt32(eformIdFrom_grid);
                int roadCode = Convert.ToInt32(decRoadCode);




                string Template_File = string.Empty;



                string filepath = ConfigurationManager.AppSettings["TEMPLATE_FILE"].ToString();
                if (workStatus.Equals("C"))
                {
                    Template_File = Path.Combine(filepath, "QM_TEMPLATE_C.pdf");
                }
                else if (workStatus.Equals("P"))
                {
                    Template_File = Path.Combine(filepath, "QM_TEMPLATE_P.pdf");
                }




                PdfLoadedDocument loadedDocument = new PdfLoadedDocument(Template_File);
                PdfLoadedForm loadedForm = loadedDocument.Form;


                string templateVersionValue = string.Empty;
                try
                {

                    PdfLoadedTextBoxField templateVersion = loadedForm.Fields["TEMPLATE_VERSION"] as PdfLoadedTextBoxField;
                    templateVersionValue = templateVersion.Text;
                }
                catch (Exception ex)
                {
                    ErrorLog.LogError(ex, "eformController.GetDownloadedQMPDF()");
                }


                EFORM_DU_LOG LogDetail = new EFORM_DU_LOG();
                string CheckSumString = decRoadCode + qmMonth + qmYear + qmCode + "QM";
                string CheckSumByteCode = objDAL.ComputeStringToSha256Hash(CheckSumString);
                LogDetail.PDF_CHECKSUM = CheckSumByteCode;
                LogDetail.EFORM_ID = eform_Id;
                if (templateVersionValue != "")
                {
                    LogDetail.TEMPLATE_VERSION = templateVersionValue;
                }
                else
                {
                    LogDetail.TEMPLATE_VERSION = "V1.0";

                }
                bool IsInserLogSuccess = objDAL.InsertDownloadLogDetails(LogDetail);


                if (IsInserLogSuccess == false)
                {

                    string error_File = ConfigurationManager.AppSettings["ERROR_FILE"].ToString();
                    string Errorfilename = "Error.pdf";
                    byte[] FileBytesFalse = System.IO.File.ReadAllBytes(error_File);
                    return File(FileBytesFalse, "application/pdf", Errorfilename);
                }

                PIU_GET_PREFILLED_DETAILS imsDetailsList = new PIU_GET_PREFILLED_DETAILS();



                imsDetailsList = objDAL.getPIUPrefilledList(roadCode, eform_Id);  //change on 08-07-2022






                PrefilledQMmodel model = new PrefilledQMmodel();




                model.RoadCode = URLEncrypt.EncryptParameters1(new string[] { "encRoadCode=" + imsDetailsList.RoadCode.ToString() + "$" + imsDetailsList.Eform_Id.ToString() });
                model.checksum = CheckSumByteCode;
                model.EFORM_ID = eform_Id.ToString();
                model.WORK_STATUS = workStatus;
                model.CURRENT_STAGE = imsDetailsList.CURRENT_STAGE == "C" ? "3" : (imsDetailsList.STAGE_PHASE == "S1" ? "1" : "2");

                model.QM_NAME = imsDetailsList.QM_NAME;
                model.QM_TYPE = imsDetailsList.QM_TYPE == "S" ? "S" : "N";

                model.STATE = imsDetailsList.State;
                model.DISTRICT = imsDetailsList.District;
                model.BLOCK = imsDetailsList.Block;

                model.ROAD_NAME = imsDetailsList.RoadName;
                model.PACKAGE_NUMBER = imsDetailsList.PkgNumber;

                model.NAME_OF_QM_38 = imsDetailsList.QM_NAME;

                if (workStatus == "C" && imsDetailsList.STAGE_PHASE == "S1")
                {
                    PdfLoadedRadioButtonListField IS_MAINTANANCE_BOARD_FIXED_30_value = loadedForm.Fields["IS_MAINTANANCE_BOARD_FIXED_30"] as PdfLoadedRadioButtonListField;
                    IS_MAINTANANCE_BOARD_FIXED_30_value.ReadOnly = true;
                }

                new CommonFunctions().GeneratePDFPreFilledDataModel(model, loadedForm);

                //save prefilled pdf
                string Generated_Path = ConfigurationManager.AppSettings["TEMPORARY_FILES_PATH"].ToString();
                if (!Directory.Exists(Generated_Path))
                    Directory.CreateDirectory(Generated_Path);
                string fileName = eform_Id + "QM_" + System.DateTime.Now.ToString("dd_MM_yyyy_HHmmss") + ".pdf";
                var fullFilePath = Path.Combine(Generated_Path, fileName);
                loadedDocument.Save(fullFilePath);



                //Close the document
                loadedDocument.Close(true);
                byte[] FileBytes = System.IO.File.ReadAllBytes(fullFilePath);

                try
                {
                    string temp_Path = ConfigurationManager.AppSettings["TEMPORARY_FILES_PATH"];
                    System.IO.DirectoryInfo di = new DirectoryInfo(temp_Path);

                    foreach (FileInfo file in di.GetFiles())
                    {
                        try
                        {
                            file.Delete();
                        }
                        catch (Exception ex)
                        {
                            ErrorLog.LogError(ex, "eformcontroller.GetDownloadedQMPDF()");
                        }

                    }

                }
                catch (Exception ex)
                {
                    ErrorLog.LogError(ex, "eformcontroller.GetDownloadedQMPDF()");

                }
                return File(FileBytes, "application/pdf", fileName);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "eformController.GetDownloadedQMPDF()");
                string filepath = ConfigurationManager.AppSettings["TEMPLATE_FILE"].ToString();
                string error_File = Path.Combine(filepath, "TEMPLATE_ERROR_PAGE.pdf");
                string Errorfilename = "Error.pdf";
                byte[] FileBytes = System.IO.File.ReadAllBytes(error_File);
                return File(FileBytes, "application/pdf", Errorfilename);
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        [RequiredAuthentication]
        [HttpPost]
        public ActionResult UploadPdfFileQM(string id)
        {
            string id1 = id;
            IPdfDataDAL objDAL = new PdfDataDAL();
            bool flag = true;
            string responseMessage = string.Empty;
            string fname;
            string uploaded_File, temp_uploaded_File;
            List<SelectListItem> result = new List<SelectListItem>();

            PMGSYEntities eformdbContext = new PMGSYEntities();
            string schdCode = id1.Split('$')[1];
            string schdMonth = id1.Split('$')[2];
            string schdYear = id1.Split('$')[3];
            string workStatus = id1.Split('$')[4];
            string qmCode = id1.Split('$')[5];
            string qmType = id1.Split('$')[6];
            id = id1.Split('$')[0];
            string eformIDFromGrid = id1.Split('$')[7];
            int RoadId = Convert.ToInt32(id);
            int eformIdTemp = Convert.ToInt32(eformIDFromGrid);
            string PIUstatusfin = eformdbContext.EFORM_PDF_UPLOAD_DETAIL.Where(s => s.PR_ROAD_CODE == RoadId && s.EFORM_ID == eformIdTemp && s.USER_TYPE == "P").Select(x => x.IS_FINALISED).FirstOrDefault();

            var isPIUfilnalized = "N";
            if (PIUstatusfin == "Y")
            {
                isPIUfilnalized = "Y";
            }

            if (isPIUfilnalized != "Y")
            {
                List<SelectListItem> validationList1 = new List<SelectListItem>();

                flag = false;
                responseMessage = "Unable to upload e-form, As PIU is yet to be finalized Eform part-I";
                validationList1.Add(new SelectListItem { Text = "Unable to upload e-form, As PIU is yet to be finalized Eform part-I" });
                var validationData1 = validationList1.Select(x => x.Text).ToList();
                flag = false;
                return Json(new { success = flag, data = validationData1, responseMsg = responseMessage });
            }

            if (eformdbContext.EFORM_TEST_REPORT_FILE.Any(s => s.EFORM_ID == eformIdTemp && s.IS_EFORM == "Y"))
            {
                if (!eformdbContext.EFORM_TEST_REPORT_FILE.Any(s => s.EFORM_ID == eformIdTemp && s.IS_FINALISED == "Y" && s.IS_EFORM == "Y"))
                {
                    List<SelectListItem> validationList1 = new List<SelectListItem>();

                    flag = false;
                    responseMessage = "Please finalize Test Report e-Form before part-2 e-Form";
                    validationList1.Add(new SelectListItem { Text = "Please finalize Test Report e-Form before part-2 e-Form" });
                    var validationData1 = validationList1.Select(x => x.Text).ToList();
                    flag = false;
                    return Json(new { success = flag, data = validationData1, responseMsg = responseMessage });

                }

            }
            else
            {
                List<SelectListItem> validationList1 = new List<SelectListItem>();

                flag = false;
                responseMessage = "Please upload Test Report e-Form before part-2 e-Form";
                validationList1.Add(new SelectListItem { Text = "Please upload Test Report e-Form before part-2 e-Form" });
                var validationData1 = validationList1.Select(x => x.Text).ToList();
                flag = false;
                return Json(new { success = flag, data = validationData1, responseMsg = responseMessage });
            }





            if (Request.Files.Count > 0)
            {
                HttpPostedFileBase file = Request.Files[0];
                List<SelectListItem> validationList = new List<SelectListItem>();
                //add more conditions like file type, file size etc as per your need.
                int fileSizeLimit = Convert.ToInt32(ConfigurationManager.AppSettings["QM_UPLOADED_FILE_SIZE"]) * 1024 * 1024;
                if (file.ContentLength < fileSizeLimit)
                {
                    if (file != null && file.ContentLength > 0 && Path.GetExtension(file.FileName).ToLower() == ".pdf")
                    {

                        try
                        {

                            fname = eformIDFromGrid + "_" + qmType + "QM_uploaded" + System.DateTime.Now.ToString("dd_MM_yyyy_HHmmss") + ".pdf";
                            String temp_Uploaded_Path = ConfigurationManager.AppSettings["TEMPORARY_FILES_PATH"].ToString();
                            flag = true;
                            if (!Directory.Exists(temp_Uploaded_Path))
                                Directory.CreateDirectory(temp_Uploaded_Path);
                            temp_uploaded_File = Path.Combine(temp_Uploaded_Path, fname);
                            file.SaveAs(temp_uploaded_File);

                            String Uploaded_Path = ConfigurationManager.AppSettings["NQM_SQM_UPLOADED_PATH"].ToString();
                            if (!Directory.Exists(Uploaded_Path))
                                Directory.CreateDirectory(Uploaded_Path);
                            uploaded_File = Path.Combine(Uploaded_Path, fname);


                            PdfLoadedDocument loadedDocument = new PdfLoadedDocument(temp_uploaded_File);
                            PdfLoadedForm loadedForm = loadedDocument.Form;

                            try
                            {
                                PdfLoadedTextBoxField RoadCode1 = loadedForm.Fields["RoadCode"] as PdfLoadedTextBoxField;
                            }
                            catch (Exception e)
                            {
                                var roadAlert = "";

                                roadAlert = "Please Fill the downloaded e-Form Pdf in adobe acrobat reader dc and then upload.";
                                validationList.Add(new SelectListItem { Text = roadAlert });
                                var validationData1 = validationList.Select(x => x.Text).ToList();
                                flag = false;
                                ErrorLog.LogError(e, "eformController.UploadPdfFileQM()");
                                return Json(new { success = flag, data = validationData1, responseMsg = roadAlert });

                            }
                            PdfLoadedTextBoxField RoadCode = loadedForm.Fields["RoadCode"] as PdfLoadedTextBoxField;
                            PdfLoadedTextBoxField checksum = loadedForm.Fields["checksum"] as PdfLoadedTextBoxField;
                            string decRoadCode = string.Empty;
                            string decRoadCodetemp = string.Empty;

                            string eformIdfrmPDF = string.Empty;
                            string[] encRoadCode = RoadCode.Text.ToString().Split('/');
                            if (encRoadCode.Length > 1)
                            {
                                decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { encRoadCode[0], encRoadCode[1], encRoadCode[2] });

                                if (decryptedParameters.Count > 0)
                                {
                                    decRoadCodetemp = decryptedParameters["encRoadCode"];
                                    decRoadCode = decRoadCodetemp.Split('$')[0];
                                    eformIdfrmPDF = decRoadCodetemp.Split('$')[1];
                                }
                            }
                            else
                            {
                                decRoadCode = RoadCode.Text;
                            }


                            //fetch template version
                            double templateVers = 0;
                            try
                            {
                                PdfLoadedTextBoxField templateVersion = loadedForm.Fields["TEMPLATE_VERSION"] as PdfLoadedTextBoxField;
                                if (templateVersion.Text != "")
                                {
                                    templateVers = Convert.ToDouble(templateVersion.Text.Replace("V", ""));
                                }

                            }
                            catch (Exception ex)
                            {
                                ErrorLog.LogError(ex, "eformController.UploadPdfFileQM");
                            }

                            if (!decRoadCode.Equals(id))
                            {

                                var roadAlert = "";
                                var str = eformdbContext.IMS_SANCTIONED_PROJECTS.Where(x => x.IMS_PR_ROAD_CODE == RoadId).Select(u => new { u.IMS_ROAD_NAME }).Single();
                                roadAlert = " Please Upload correct Pdf for road " + str.IMS_ROAD_NAME;

                                validationList.Add(new SelectListItem { Text = roadAlert });

                                var validationData1 = validationList.Select(x => x.Text).ToList();
                                flag = false;
                                return Json(new { success = flag, data = validationData1, responseMsg = roadAlert });
                            }
                            if (!eformIdfrmPDF.Equals(eformIDFromGrid))
                            {
                                string eformIdAlert = " Please Upload correct Pdf for road " + eformIDFromGrid;

                                validationList.Add(new SelectListItem { Text = eformIdAlert });

                                var validationData1 = validationList.Select(x => x.Text).ToList();
                                flag = false;
                                return Json(new { success = flag, data = validationData1, responseMsg = eformIdAlert });
                            }
                            int roadCodeChck = Convert.ToInt32(decRoadCode);
                            int eformId = eformdbContext.EFORM_MASTER.Where(s => s.EFORM_ID == eformIdTemp).Select(m => m.EFORM_ID).FirstOrDefault();
                            string checksumfrmDB = eformdbContext.EFORM_DU_LOG.Where(s => s.EFORM_ID == eformId && (s.USER_TYPE == "N" || s.USER_TYPE == "S")).OrderByDescending(p => p.LOG_ID).Select(m => m.PDF_CHECKSUM).FirstOrDefault();
                            string RoadStatus = eformdbContext.EFORM_PREFILLED_DETAILS.Where(s => s.EFORM_ID == eformIdTemp).Select(s => s.PHYSICAL_WORK_STATUS).FirstOrDefault();
                            bool TemplateStatus = false;

                            if (workStatus == "C")
                            {
                                TemplateStatus = true;
                            }



                            if (!checksum.Text.Equals(checksumfrmDB))
                            {
                                string roadAlert = "Please upload latest downloaded pdf";
                                validationList.Add(new SelectListItem { Text = "Please upload latest downloaded pdf" });
                                var validationData1 = validationList.Select(x => x.Text).ToList();
                                flag = false;
                                return Json(new { success = flag, data = validationData1, responseMsg = roadAlert });
                            }

                            loadedDocument.Close(true);
                            char[] a1 = new char[22];


                            for (int i = 2; i <= 21; i++)
                            {
                                string chkbxName = "Item_" + i;
                                PdfLoadedCheckBoxField CheckField = loadedForm.Fields[chkbxName] as PdfLoadedCheckBoxField;
                                if (CheckField.Checked)
                                {
                                    a1[i] = 'Y';
                                }
                                else
                                {
                                    a1[i] = 'N';
                                }

                            }




                            EFORM_QM_VIEWMODEL objViewQMModel = PdfDataToClassQMModel(temp_uploaded_File, TemplateStatus, a1, templateVers);

                            StringBuilder strErrorMessage = new StringBuilder("");
                            int count = 0;
                            if (objViewQMModel.ErrorOccured)
                            {
                                objViewQMModel.ErrorList = objViewQMModel.ErrorList.Distinct().ToList();
                                #region 31-03-2022
                                List<string> errorslist = new List<string>();
                                for (int i = 6; i <= 38; i++)
                                {
                                    string Errorpage = "Page-";


                                    errorslist = objViewQMModel.ErrorList.Where(a => a.Contains(string.Concat(Errorpage + i))).ToList();
                                    if (errorslist.Count > 0)
                                    {
                                        strErrorMessage.Append("<h4 style='background-color:#efede5; color:black'> Page " + i + " </h4>");
                                    }

                                    foreach (var ierror in errorslist)
                                    {
                                        strErrorMessage.Append("<li style='margin-left:10px; list-style-type: square'> ");
                                        strErrorMessage.Append("Error-" + ++count + " " + ierror);
                                        strErrorMessage.Append("<br />");
                                        strErrorMessage.Append("</li>");
                                    }
                                }
                                #endregion



                                return Json(new { success = false, data = strErrorMessage.ToString(), IsValidationError = true, responseMsg = "Please fill all mandatory details in file" });
                            }
                            //save to db

                            result = objDAL.SaveToQMDb(temp_uploaded_File, eformIDFromGrid, fname, Uploaded_Path, file, uploaded_File, objViewQMModel, schdCode, schdMonth, schdYear, workStatus, qmCode, qmType, TemplateStatus, a1, templateVers);
                            var validationData = result.Select(x => x.Text).ToList();

                            if (result.Count > 0)
                            {

                                flag = false;
                                validationList.Add(new SelectListItem { Text = "Error occured while uploading Eform. Please contact OMMAS team " });
                                var validationData1 = validationList.Select(x => x.Text).ToList();
                                return Json(new { success = flag, data = validationData1, responseMsg = "Error occured while uploading Eform. Please contact OMMAS team" });
                            }
                            else
                            {
                                loadedDocument.Close(true);
                                responseMessage = "Upload Successful.";
                            }


                        }
                        catch (Exception ex)
                        {
                            flag = false;
                            ErrorLog.LogError(ex, "eformcontroller.UploadPdfFileQM()");
                            responseMessage = "Error occured while uploading pdf, please contact OMMAS team.";
                            validationList.Add(new SelectListItem { Text = "Error occured while uploading pdf, please contact OMMAS team." });
                            var validationData1 = validationList.Select(x => x.Text).ToList();
                            return Json(new { success = flag, data = validationData1, responseMsg = responseMessage });
                        }
                        finally
                        {
                            file.InputStream.Flush();
                            file.InputStream.Close();
                            eformdbContext.Dispose();
                        }
                    }
                    else
                    {
                        flag = false;
                        responseMessage = "File is invalid.";
                        validationList.Add(new SelectListItem { Text = "File is invalid." });
                        var validationData1 = validationList.Select(x => x.Text).ToList();
                        return Json(new { success = flag, data = validationData1, responseMsg = responseMessage });
                    }
                }
                else
                {
                    flag = false;
                    responseMessage = "Invalid file size. Please upload file upto 12 MB..";
                    validationList.Add(new SelectListItem { Text = "Invalid file size. Please upload file upto 12 MB." });
                    var validationData1 = validationList.Select(x => x.Text).ToList();
                    return Json(new { success = flag, data = validationData1, responseMsg = responseMessage });
                }
            }
            else
            {
                flag = false;
                responseMessage = "File Upload has no file.";
            }

            return Json(new { success = flag, message = responseMessage }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult DeleteTempFileQM(string id)
        {

            try
            {
                string temp_Path = ConfigurationManager.AppSettings["TEMPORARY_FILES_PATH"];
                System.IO.DirectoryInfo di = new DirectoryInfo(temp_Path);

                foreach (FileInfo file in di.GetFiles())
                {
                    try
                    {
                        file.Delete();
                    }
                    catch (Exception ex)
                    {
                        ErrorLog.LogError(ex, "eformcontroller.DeleteTempFileQM()");
                    }

                }

            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "eformcontroller.DeleteTempFileQM()");
                return Json(new { success = true }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { success = true }, JsonRequestBehavior.AllowGet);
        }
        [RequiredAuthentication]
        public ActionResult ViewQMPdfSavedData(string encIdtemp)
        {
            try
            {
                #region QM view

                string[] encParam = encIdtemp.Split('/');
                decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { encParam[0], encParam[1], encParam[2] });
                int id = 0;
                if (decryptedParameters.Count > 0)
                {
                    id = Convert.ToInt32(decryptedParameters["imsRoadID"].Split('$')[0]);
                }

                IPdfDataDAL objDAL = new PdfDataDAL();
                QM_ViewPdfModel model = objDAL.ViewQMPdfSavedData(id);
                //  return View("ViewQMPdfData", model);
                return View("~/Areas/EFORMArea/Views/EFORM/ViewQMPdfSavedData.cshtml", model);

                #endregion
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "eformcontroller.ViewQMPdfSavedData()");
                return View("~/Areas/EFORMArea/Views/EFORM/PDF_ErrorPage");
            }
        }

        [RequiredAuthentication]
        public JsonResult deleteQMEformDetail(string id)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            string roadIDdec = id.Split('$')[0];
            string eformIddec = id.Split('$')[1];
            int roadId = Convert.ToInt32(roadIDdec);
            string fileName = string.Empty;
            string Uploaded_Path_QM = string.Empty;
            string fileNameC = string.Empty;
            string Uploaded_Path_C = string.Empty;
            try
            {


                int eformId = Convert.ToInt32(eformIddec);
                using (var scope = new TransactionScope())
                {

                    EFORM_MASTER model = dbContext.EFORM_MASTER.Where(s => s.EFORM_ID == eformId).FirstOrDefault();
                    fileName = dbContext.EFORM_PDF_UPLOAD_DETAIL.Where(x => x.PR_ROAD_CODE == roadId && x.EFORM_ID == eformId && x.USER_TYPE == "Q").Select(s => s.FILE_NAME).FirstOrDefault();
                    //Uploaded_Path_QM = dbContext.EFORM_PDF_UPLOAD_DETAIL.Where(x => x.PR_ROAD_CODE == roadId && x.EFORM_ID == eformId && x.USER_TYPE == "Q").Select(s => s.FILE_PATH).FirstOrDefault();
                    Uploaded_Path_QM = ConfigurationManager.AppSettings["NQM_SQM_UPLOADED_PATH"].ToString() + "/" + model.ADMIN_SCHEDULE_YEAR.ToString() + "/" + model.ADMIN_SCHEDULE_MONTH.ToString();

                    fileNameC = dbContext.EFORM_PDF_UPLOAD_DETAIL.Where(x => x.PR_ROAD_CODE == roadId && x.EFORM_ID == eformId && x.USER_TYPE == "Q").Select(s => s.FILE_NAME_C).FirstOrDefault();
                    // Uploaded_Path_C = dbContext.EFORM_PDF_UPLOAD_DETAIL.Where(x => x.PR_ROAD_CODE == roadId && x.EFORM_ID == eformId && x.USER_TYPE == "Q").Select(s => s.FILE_PATH_C).FirstOrDefault();
                    Uploaded_Path_C = ConfigurationManager.AppSettings["COMBINE_UPLOADED_PATH"].ToString() + "/" + model.ADMIN_SCHEDULE_YEAR.ToString() + "/" + model.ADMIN_SCHEDULE_MONTH.ToString();

                    if (fileName == null)
                    {
                        return Json(new { success = false, message = "error occured during deleting the pdf." }, JsonRequestBehavior.AllowGet);
                    }
                    var QM_GENERAL_DETAILS = dbContext.EFORM_QM_GENERAL_DETAILS.Where(x => x.EFORM_ID == eformId);
                    dbContext.EFORM_QM_GENERAL_DETAILS.RemoveRange(QM_GENERAL_DETAILS);
                    dbContext.SaveChanges();
                    var item_appl_details = dbContext.EFORM_QM_ITEM_APPLICABLE_DETAILS.Where(x => x.EFORM_ID == eformId);
                    dbContext.EFORM_QM_ITEM_APPLICABLE_DETAILS.RemoveRange(item_appl_details);
                    dbContext.SaveChanges();
                    dbContext.USP_EFORMS_QM_DELETE_INSPECTION_DETAILS(roadId, eformId);
                    string fullPath_QM = Path.Combine(Uploaded_Path_QM, fileName);

                    FileInfo file_QM = new FileInfo(fullPath_QM);
                    if (file_QM.Exists)
                    {
                        file_QM.Delete();
                    }
                    if (fileNameC != null)
                    {
                        string fullPath_QM_C = Path.Combine(Uploaded_Path_C, fileNameC);
                        FileInfo file_QM_C = new FileInfo(fullPath_QM_C);
                        if (file_QM_C.Exists)
                        {
                            file_QM_C.Delete();
                        }
                    }
                    scope.Complete();
                    return Json(new { success = true }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "eformcontroller.deleteQMEformDetail()");
                return Json(new { success = false, message = "error occured during deleting the pdf." }, JsonRequestBehavior.AllowGet);

            }
            finally
            {
                dbContext.Dispose();

            }

        }

        [RequiredAuthentication]
        public JsonResult finalizeQMEformDetail(string id)
        {
            bool flag = true;
            PMGSYEntities dbContext = new PMGSYEntities();
            try
            {
                int detailId = Convert.ToInt32(id);
                string PIUstatusfin = dbContext.EFORM_PDF_UPLOAD_DETAIL.Where(s => s.EFORM_ID == detailId && s.USER_TYPE == "P").Select(x => x.IS_FINALISED).FirstOrDefault();
                var isPIUfilnalized = "N";
                if (PIUstatusfin == "Y")
                {
                    isPIUfilnalized = "Y";
                }
                if (isPIUfilnalized != "Y")
                {
                    List<SelectListItem> validationList1 = new List<SelectListItem>();
                    flag = false;
                    string responseMessage = "Unable to finalize e-form, as PIU is yet to finalized part-I";
                    return Json(new { success = flag, responseMsg = responseMessage });
                }
                var qmfile_Status = dbContext.EFORM_PDF_UPLOAD_DETAIL.Where(x => x.EFORM_ID == detailId && x.USER_TYPE == "Q").Select(x => x.FILE_NAME).FirstOrDefault();
                var isQMfileupload = "N";
                if (qmfile_Status != null)
                {
                    isQMfileupload = "Y";
                }
                if (isQMfileupload != "Y")
                {
                    List<SelectListItem> validationList1 = new List<SelectListItem>();
                    flag = false;
                    string responseMessage = "Unable to finalize e-form,as pdf is not uploaded yet";
                    return Json(new { success = flag, responseMsg = responseMessage });
                }
                using (TransactionScope ts = new TransactionScope())
                {

                    EFORM_MASTER masterModel = new EFORM_MASTER();
                    masterModel = dbContext.EFORM_MASTER.Where(s => s.EFORM_ID == detailId).Single();
                    masterModel.IS_COMPLETED = "Y";
                    dbContext.Entry(masterModel).State = EntityState.Modified;
                    EFORM_PDF_UPLOAD_DETAIL obj = dbContext.EFORM_PDF_UPLOAD_DETAIL.Where(s => s.EFORM_ID == detailId && s.USER_TYPE == "Q").FirstOrDefault();
                    obj.IS_FINALISED = "Y";
                    dbContext.Entry(obj).State = EntityState.Modified;
                    dbContext.SaveChanges();
                    ts.Complete();

                }
                return Json(new { success = flag }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                flag = false;
                ErrorLog.LogError(ex, "eformController.finalizeQMEformDetail()");
                return Json(new { success = flag }, JsonRequestBehavior.AllowGet);
            }
            finally
            {
                dbContext.Dispose();
            }

        }

        [RequiredAuthentication]
        [HttpPost]
        public JsonResult IsQMPDFFileavaialble(string id)
        {
            bool flag = false;
            string message = string.Empty;
            PMGSYEntities dbContext = new PMGSYEntities();
            try
            {

                int eformId = Convert.ToInt32(id.Split('$')[7]);
                var fileName = dbContext.EFORM_PDF_UPLOAD_DETAIL.Where(x => x.EFORM_ID == eformId && (x.USER_TYPE == "Q")).Select(s => s.FILE_NAME).FirstOrDefault();
                string fullPath = string.Empty;
                if (fileName == null)
                {

                    return Json(new { success = flag }, JsonRequestBehavior.AllowGet);

                }
                else
                {
                    flag = true;
                    message = "Pdf is already uploaded. If you want to upload again, please delete the existing file";

                }

            }
            catch (Exception ex)
            {
                flag = true;
                ErrorLog.LogError(ex, "eformcontroller.IsQMPDFFileavaialble()");
                message = "Error occured during uploading file";
                return Json(new { success = flag }, JsonRequestBehavior.AllowGet);
            }
            finally
            {
                dbContext.Dispose();
            }

            return Json(new { success = flag, message = message }, JsonRequestBehavior.AllowGet);
        }

        [RequiredAuthentication]
        public FileResult GetQMReport(string idtemp)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            try
            {

                string id = null;
                string[] encParam = idtemp.Split('/');
                decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { encParam[0], encParam[1], encParam[2] });
                if (decryptedParameters.Count > 0)
                {
                    id = decryptedParameters["imsRoadID"];
                }
                int eformId = Convert.ToInt32(id);
                var nameReport = id + "_Part-2_Report.pdf";
                EFORM_MASTER model = dbContext.EFORM_MASTER.Where(s => s.EFORM_ID == eformId).FirstOrDefault();
                string uploaded_path = ConfigurationManager.AppSettings["NQM_SQM_UPLOADED_PATH"].ToString() + "/" + model.ADMIN_SCHEDULE_YEAR.ToString() + "/" + model.ADMIN_SCHEDULE_MONTH.ToString();
                //  string uploaded_path = dbContext.EFORM_PDF_UPLOAD_DETAIL.Where(x => x.EFORM_ID == eformId && x.USER_TYPE == "Q").Select(s => s.FILE_PATH).FirstOrDefault();
                var fileName = dbContext.EFORM_PDF_UPLOAD_DETAIL.Where(x => x.EFORM_ID == eformId && x.USER_TYPE == "Q").Select(s => s.FILE_NAME).FirstOrDefault();






                string fullPath = Path.Combine(uploaded_path, fileName);
                byte[] FileBytes = System.IO.File.ReadAllBytes(fullPath);
                return File(FileBytes, "application/pdf", nameReport);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "eformcontroller.GetQMReport()");
                return null;
            }
            finally
            {
                dbContext.Dispose();
            }

        }

        [RequiredAuthentication]
        public JsonResult viewPart2PdfVirtualDir(string id)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            try
            {

                int eformId = Convert.ToInt32(id);
                string uploaded_path = dbContext.EFORM_PDF_UPLOAD_DETAIL.Where(x => x.EFORM_ID == eformId && x.USER_TYPE == "Q").Select(s => s.FILE_PATH).FirstOrDefault();
                EFORM_MASTER model = dbContext.EFORM_MASTER.Where(s => s.EFORM_ID == eformId).FirstOrDefault();
                var fileName = dbContext.EFORM_PDF_UPLOAD_DETAIL.Where(x => x.EFORM_ID == eformId && x.USER_TYPE == "Q").Select(s => s.FILE_NAME).FirstOrDefault();


                string VirtualDirectoryPath = ConfigurationManager.AppSettings["QM_UPLOADED_EFORM_VIRTUAL_DIR_PATH"].ToString();
                string VirtualDirectoryfullPath = Path.Combine(VirtualDirectoryPath, Convert.ToString(model.ADMIN_SCHEDULE_YEAR.ToString() + "/" + model.ADMIN_SCHEDULE_MONTH.ToString()), fileName.ToString()).ToString().Replace(@"\\", @"//").Replace(@"\", @"/");

                string physicalFullPath = Path.Combine(uploaded_path, fileName);

                FileInfo file = new FileInfo(physicalFullPath);
                if (file.Exists)
                {
                    return Json(new { success = true, Message = VirtualDirectoryfullPath }, JsonRequestBehavior.AllowGet);

                }
                else
                {
                    return Json(new { success = false, Message = "File not found" }, JsonRequestBehavior.AllowGet);
                }

            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "eformController.viewPart2PdfVirtualDir()");
                return null;
            }
            finally
            {
                dbContext.Dispose();
            }

        }

        #endregion

        #region CQC
        public FileResult GetCombineReport(string idtemp)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            try
            {
                string id = null;
                string[] encParam = idtemp.Split('/');
                decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { encParam[0], encParam[1], encParam[2] });
                if (decryptedParameters.Count > 0)
                {
                    id = decryptedParameters["imsRoadID"];
                }
                int eformId = Convert.ToInt32(id);
                var nameReport = id + "_Report.pdf";
                EFORM_MASTER model = dbContext.EFORM_MASTER.Where(s => s.EFORM_ID == eformId).FirstOrDefault();
                string uploaded_path = ConfigurationManager.AppSettings["COMBINE_UPLOADED_PATH"].ToString() + "/" + model.ADMIN_SCHEDULE_YEAR.ToString() + "/" + model.ADMIN_SCHEDULE_MONTH.ToString();
                //  string uploaded_path = dbContext.EFORM_PDF_UPLOAD_DETAIL.Where(x => x.EFORM_ID == eformId && x.USER_TYPE == "Q").Select(s => s.FILE_PATH_C).FirstOrDefault();
                var fileName = dbContext.EFORM_PDF_UPLOAD_DETAIL.Where(x => x.EFORM_ID == eformId && x.USER_TYPE == "Q").Select(s => s.FILE_NAME_C).FirstOrDefault();
                string fullPath = Path.Combine(uploaded_path, fileName);
                byte[] FileBytes = System.IO.File.ReadAllBytes(fullPath);
                return File(FileBytes, "application/pdf", nameReport);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "eformcontroller.GetCombineReport()");
                return null;
            }
            finally
            {
                dbContext.Dispose();
            }
        }
        [RequiredAuthentication]
        public JsonResult viewCombinePdfVirtualDir(string id)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            try
            {

                int eformId = Convert.ToInt32(id);
                string uploaded_path = dbContext.EFORM_PDF_UPLOAD_DETAIL.Where(x => x.EFORM_ID == eformId && x.USER_TYPE == "Q").Select(s => s.FILE_PATH_C).FirstOrDefault();
                EFORM_MASTER model = dbContext.EFORM_MASTER.Where(s => s.EFORM_ID == eformId).FirstOrDefault();
                var fileName = dbContext.EFORM_PDF_UPLOAD_DETAIL.Where(x => x.EFORM_ID == eformId && x.USER_TYPE == "Q").Select(s => s.FILE_NAME_C).FirstOrDefault();


                string VirtualDirectoryPath = ConfigurationManager.AppSettings["COMBINE_UPLOADED_EFORM_VIRTUAL_DIR_PATH"].ToString();
                string VirtualDirectoryfullPath = Path.Combine(VirtualDirectoryPath, Convert.ToString(model.ADMIN_SCHEDULE_YEAR.ToString() + "/" + model.ADMIN_SCHEDULE_MONTH.ToString()), fileName.ToString()).ToString().Replace(@"\\", @"//").Replace(@"\", @"/");

                string physicalFullPath = Path.Combine(uploaded_path, fileName);

                FileInfo file = new FileInfo(physicalFullPath);
                if (file.Exists)
                {
                    return Json(new { success = true, Message = VirtualDirectoryfullPath }, JsonRequestBehavior.AllowGet);

                }
                else
                {
                    return Json(new { success = false, Message = "File not found" }, JsonRequestBehavior.AllowGet);
                }

            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "eformController.viewCombinePdfVirtualDir()");
                return null;
            }
            finally
            {
                dbContext.Dispose();
            }

        }

        [RequiredAuthentication]
        public ActionResult IsFileAvail(string idtemp)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            try
            {

                string id = null;
                string[] encParam = idtemp.Split('/');
                decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { encParam[0], encParam[1], encParam[2] });
                if (decryptedParameters.Count > 0)
                {
                    id = decryptedParameters["imsRoadID"];
                }
                int eformId = Convert.ToInt32(id);
                var nameReport = id + "_Report.pdf";
                string uploaded_path = dbContext.EFORM_PDF_UPLOAD_DETAIL.Where(x => x.EFORM_ID == eformId && x.USER_TYPE == "Q").Select(s => s.FILE_PATH_C).FirstOrDefault();
                var fileName = dbContext.EFORM_PDF_UPLOAD_DETAIL.Where(x => x.EFORM_ID == eformId && x.USER_TYPE == "Q").Select(s => s.FILE_NAME_C).FirstOrDefault();
                string fullPath = Path.Combine(uploaded_path, fileName);
                FileInfo f = new FileInfo(fullPath);
                if (f.Exists)
                {
                    return Json(new { response = true }, JsonRequestBehavior.AllowGet);
                }
                return Json(new { response = false }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "eformcontroller.IsFileAvail()");
                return Json(new { response = false }, JsonRequestBehavior.AllowGet);
            }
            finally
            {
                dbContext.Dispose();
            }

        }


        public JsonResult isPart1Part2PdfAvail(string id)
        {
            PMGSYEntities dbContext = new PMGSYEntities();

            try
            {

                int eformId = Convert.ToInt32(id);
                EFORM_MASTER model = dbContext.EFORM_MASTER.Where(s => s.EFORM_ID == eformId).FirstOrDefault();

                var uploadpart1Model = dbContext.EFORM_PDF_UPLOAD_DETAIL.Where(s => s.EFORM_ID == eformId && s.USER_TYPE == "P").FirstOrDefault();
                var part1Fname = uploadpart1Model.FILE_NAME;
                var part1Path = ConfigurationManager.AppSettings["PIU_UPLOADED_PATH"].ToString() + "/" + model.ADMIN_SCHEDULE_YEAR.ToString() + "/" + model.ADMIN_SCHEDULE_MONTH.ToString();

                string physicalFullPathPart1 = Path.Combine(part1Path, part1Fname);
                FileInfo filePart1 = new FileInfo(physicalFullPathPart1);
                if (!filePart1.Exists)
                {
                    return Json(new { success = false, Message = "Part1 File not found" }, JsonRequestBehavior.AllowGet);
                }



                var uploadpart2Model = dbContext.EFORM_PDF_UPLOAD_DETAIL.Where(s => s.EFORM_ID == eformId && s.USER_TYPE == "Q").FirstOrDefault();
                var part2Fname = uploadpart2Model.FILE_NAME;
                var part2Path = ConfigurationManager.AppSettings["NQM_SQM_UPLOADED_PATH"].ToString() + "/" + model.ADMIN_SCHEDULE_YEAR.ToString() + "/" + model.ADMIN_SCHEDULE_MONTH.ToString();

                string physicalFullPathPart2 = Path.Combine(part2Path, part2Fname);
                FileInfo filePart2 = new FileInfo(physicalFullPathPart2);
                if (!filePart2.Exists)
                {
                    return Json(new { success = false, Message = "Part2 File not found" }, JsonRequestBehavior.AllowGet);
                }



                return Json(new { success = true, Message = "Test Report File found" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "eformcontroller.isPart1Part2PdfAvail()");
                return Json(new { success = false, Message = "Error occured while downloading pdf" }, JsonRequestBehavior.AllowGet);
            }
            finally
            {
                dbContext.Dispose();
            }

        }

        [RequiredAuthentication]
        public FileResult viewCombinedPdf12(string id)
        {
            PMGSYEntities dbContext = new PMGSYEntities();

            try
            {

                int eformId = Convert.ToInt32(id);
                EFORM_MASTER model = dbContext.EFORM_MASTER.Where(s => s.EFORM_ID == eformId).FirstOrDefault();

                var uploadpart1Model = dbContext.EFORM_PDF_UPLOAD_DETAIL.Where(s => s.EFORM_ID == eformId && s.USER_TYPE == "P").FirstOrDefault();
                var part1Fname = uploadpart1Model.FILE_NAME;
                var part1Path = ConfigurationManager.AppSettings["PIU_UPLOADED_PATH"].ToString() + "/" + model.ADMIN_SCHEDULE_YEAR.ToString() + "/" + model.ADMIN_SCHEDULE_MONTH.ToString();
                string physicalFullPathPart1 = Path.Combine(part1Path, part1Fname);

                var uploadpart2Model = dbContext.EFORM_PDF_UPLOAD_DETAIL.Where(s => s.EFORM_ID == eformId && s.USER_TYPE == "Q").FirstOrDefault();
                var part2Fname = uploadpart2Model.FILE_NAME;
                var part2Path = ConfigurationManager.AppSettings["NQM_SQM_UPLOADED_PATH"].ToString() + "/" + model.ADMIN_SCHEDULE_YEAR.ToString() + "/" + model.ADMIN_SCHEDULE_MONTH.ToString();
                string physicalFullPathPart2 = Path.Combine(part2Path, part2Fname);


                byte[] FileBytesPart1 = System.IO.File.ReadAllBytes(physicalFullPathPart1);
                Stream streamPart1 = new MemoryStream(FileBytesPart1);

                byte[] FileBytesPart2 = System.IO.File.ReadAllBytes(physicalFullPathPart2);
                Stream streamPart2 = new MemoryStream(FileBytesPart2);


                Stream[] streams = { streamPart1, streamPart2 };

                PdfDocument finalDoc = new PdfDocument();
                PdfDocument.Merge(finalDoc, streams);
                string fileName = eformId + "_Combined_Part1_Part2_" + System.DateTime.Now.ToString("dd_MM_yyyy_HHmmss") + ".pdf";
                var tempPath = ConfigurationManager.AppSettings["TEMPORARY_FILES_PATH"].ToString();
                if (!Directory.Exists(tempPath))
                {
                    Directory.CreateDirectory(tempPath);
                }
                var fullFilePath = Path.Combine(tempPath, fileName);
                finalDoc.Save(fullFilePath);
                byte[] FileBytes = System.IO.File.ReadAllBytes(fullFilePath);
                try
                {
                    string temp_Path = ConfigurationManager.AppSettings["TEMPORARY_FILES_PATH"];
                    System.IO.DirectoryInfo di = new DirectoryInfo(temp_Path);

                    foreach (FileInfo file in di.GetFiles())
                    {
                        try
                        {
                            file.Delete();
                        }
                        catch (Exception ex)
                        {
                            ErrorLog.LogError(ex, "eformcontroller.viewCombinedPdf12TR()");
                        }

                    }

                }
                catch (Exception ex)
                {
                    ErrorLog.LogError(ex, "eformcontroller.viewCombinedPdf12TR()");

                }
                return File(FileBytes, "application/pdf", fileName);

            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "eformController.viewCombinedPdf12TR()");
                string error_File = ConfigurationManager.AppSettings["ERROR_FILE"].ToString();
                string Errorfilename = "Error.pdf";
                byte[] FileBytes = System.IO.File.ReadAllBytes(error_File);
                return File(FileBytes, "application/pdf", Errorfilename);
            }
            finally
            {
                dbContext.Dispose();
            }

        }





        [RequiredAuthentication]
        public ActionResult ViewCombinePdfSavedData(string eid)
        {
            try
            {
                #region CQC View

                string[] encParam = eid.Split('/');
                decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { encParam[0], encParam[1], encParam[2] });
                int id = 0;
                if (decryptedParameters.Count > 0)
                {
                    id = Convert.ToInt32(decryptedParameters["imsRoadID"].Split('$')[0]);
                }

                IPdfDataDAL objDAL = new PdfDataDAL();

                EFORM_PIU_VIEWMODEL PIUmodel = objDAL.ViewPIU_PdfSavedData(id);
                QM_ViewPdfModel QMmodel = objDAL.ViewQMPdfSavedData(id);

                CQC_PreviewModel objCQC = new CQC_PreviewModel();
                objCQC.EFORM_PIU_VIEWMODEL = PIUmodel;
                objCQC.QM_ViewPdfModel = QMmodel;
                return View("~/Areas/EFORMArea/Views/EFORM/ViewCombinePdfSavedData.cshtml", objCQC);
                //  return View(objCQC);

                #endregion

            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "eformcontroller.ViewCombinePdfSavedData()");
                return View("~/Areas/EFORMArea/Views/EFORM/PDF_ErrorPage");
            }
        }


        #endregion

        private EFORM_PIU_VIEWMODEL PdfDataToClassModel(string filePath, bool TemplateStatus, string eformId, double tempVersion)
        {
            PdfLoadedDocument loadedDocument = new PdfLoadedDocument(filePath);
            PMGSYEntities dbContext = new PMGSYEntities();
            try
            {
                PDFFiledDataInspectorModel pDFFiledDataInspectorModel = new PDFFiledDataInspectorModel();

                PdfLoadedForm loadedForm = loadedDocument.Form;
                EFORM_PIU_VIEWMODEL objViewModel = new EFORM_PIU_VIEWMODEL();
                CommonFunctions objCommonFunction = new CommonFunctions();
                OffcialModelList offcialModelList = new OffcialModelList(TemplateStatus);
                InspectionModelList inspectionModelList = new InspectionModelList();
                NewTechModelList newTechList = new NewTechModelList();




                objViewModel.ErrorList = new List<string>();
                objViewModel.GENERAL_INFO_PIU = new EFORM_GENERAL_INFO_PIU(TemplateStatus);
                objViewModel.NEW_TECHNOLOGY_DETAILS_LIST = new List<EFORM_NEW_TECHNOLOGY_DETAILS_PIU>();

                objViewModel.Physical_Progress_List = new List<EFORM_PRGS_DETAILS_PIU>();
                objViewModel.MIX_DESIGN_DETAILS_LIST = new List<EFORM_MIX_DESIGN_DETAILS_PIU>();
                objViewModel.QC_DETAILS_PIU = new EFORM_QC_DETAILS_PIU(TemplateStatus);
                objViewModel.QC_OFFICIAL_DETAILS_LIST = new List<EFORM_QC_OFFICIAL_DETAILS_PIU>();
                objViewModel.PREVIOUS_INSP_DETAILS_LIST = new List<EFORM_PREVIOUS_INSP_DETAILS_PIU>();

                objViewModel.PREVIOUS_INSP_DETAILS_LIST_temp3_0 = new List<EFORM_PREVIOUS_INSP_DETAILS_PIU_Temp3_0>();  // add on 01-08-2022 

                List<string> errorListtemp = new List<string>();

                objViewModel.ErrorList = new List<string>();

                #region EFORM_GENERAL_INFO_PIU

                objViewModel.ErrorList = objCommonFunction.FetchPDFFilledDataToModel(objViewModel.GENERAL_INFO_PIU, loadedForm);
                #endregion

                #region EFORM_NEW_TECHNOLOGY_DETAILS_PIU
                for (int i = 1; i <= 3; i++)
                {
                    string techname = "NEW_TECHNOLOGY_NAME" + i;
                    string rdFrom = "ROAD_FROM" + i;
                    string rdTo = "ROAD_TO" + i;
                    string chkbox = "NEW_" + i;
                    PdfLoadedComboBoxField technameField = loadedForm.Fields[techname] as PdfLoadedComboBoxField;
                    PdfLoadedTextBoxField RdFromField = loadedForm.Fields[rdFrom] as PdfLoadedTextBoxField;
                    PdfLoadedTextBoxField RdtoField = loadedForm.Fields[rdTo] as PdfLoadedTextBoxField;
                    PdfLoadedCheckBoxField chkBx = loadedForm.Fields[chkbox] as PdfLoadedCheckBoxField;
                    if (chkBx.Checked)
                    {
                        if (Convert.ToInt32(technameField.SelectedValue) == 1)
                        {
                            objViewModel.ErrorList.Add("Page-1: Item 1. GENERAL XI. Please select new technology for selected checkbox:" + i);
                            objViewModel.ErrorOccured = true;
                        }
                        if (Convert.ToInt32(technameField.SelectedValue) > 1)
                        {
                            if (RdFromField.Text.Equals(""))
                            {
                                objViewModel.ErrorList.Add("Page-1: Item 1. GENERAL XI. Please enter road from for selected " + i + " new technology");
                                objViewModel.ErrorOccured = true;
                            }
                            if (RdtoField.Text.Equals(""))
                            {
                                objViewModel.ErrorList.Add("Page-1: Item 1. GENERAL XI. Please enter road to for selected " + i + " new technology");
                                objViewModel.ErrorOccured = true;
                            }


                        }
                    }
                }



                newTechList.NewTechList.ForEach(item =>
                {

                    for (int i = 1; i <= item.RowCount; i++)
                    {
                        EFORM_NEW_TECHNOLOGY_DETAILS_PIU detailedItem = new EFORM_NEW_TECHNOLOGY_DETAILS_PIU();
                        detailedItem.TECH_TYPE = item.NewTechType;
                        detailedItem.RowID = i;
                        objViewModel.NEW_TECHNOLOGY_DETAILS_LIST.Add(detailedItem);
                    }

                });

                objViewModel.NEW_TECHNOLOGY_DETAILS_LIST.ForEach(item =>
                {
                    errorListtemp = objCommonFunction.FetchPDFFilledDataToModel(item, loadedForm);
                    objViewModel.ErrorList.AddRange(errorListtemp);

                }
               );


                #endregion

                #region EFORM_PRGS_DETAILS_PIU

                int count = 0;
                for (int i = 1; i <= 10; i++)
                {
                    string chkBxName = "cb_2_" + i;
                    PdfLoadedCheckBoxField chkBx = loadedForm.Fields[chkBxName] as PdfLoadedCheckBoxField;
                    if (!chkBx.Checked)
                    {
                        count++;
                    }
                }
                if (count == 10)
                {
                    errorListtemp.Clear();
                    errorListtemp.Add("Page-2: Item 2. PHYSICAL PROGRESS- Table: Please fill physical progress details atleast for one work");
                    objViewModel.ErrorList.AddRange(errorListtemp);
                    errorListtemp.Clear();
                    objViewModel.ErrorOccured = true;
                }
                else
                {
                    dbContext.EFORM_MASTER_WORK_ITEM.Where(m => m.ITEM_FLAG == "P").Select(t => t.ITEM_ID).ToList().ForEach(itemID =>
                    {
                        EFORM_PRGS_DETAILS_PIU detailedItem = new EFORM_PRGS_DETAILS_PIU();
                        detailedItem.ITEM_ID = itemID;
                        objViewModel.Physical_Progress_List.Add(detailedItem);
                    });
                    int countTemp = 1;
                    foreach (var item in objViewModel.Physical_Progress_List.ToList())
                    {

                        string v = "cb_2_" + countTemp++;
                        PdfLoadedCheckBoxField CheckField = loadedForm.Fields[v] as PdfLoadedCheckBoxField;
                        if (CheckField.Checked)
                        {
                            errorListtemp = objCommonFunction.FetchPDFFilledDataToModel(item, loadedForm);
                            objViewModel.ErrorList.AddRange(errorListtemp);
                        }
                        else
                        {
                            objViewModel.Physical_Progress_List.Remove(item);
                        }

                    }

                }



                #endregion


                #region EFORM_QC_DETAILS_PIU


                errorListtemp = objCommonFunction.FetchPDFFilledDataToModel(objViewModel.QC_DETAILS_PIU, loadedForm);
                objViewModel.ErrorList.AddRange(errorListtemp);
                #endregion


                #region EFORM_QC_OFFICIAL_DETAILS_PIU
                offcialModelList.OfficialList.ForEach(item =>
                {

                    for (int i = 1; i <= item.RowCount; i++)
                    {
                        EFORM_QC_OFFICIAL_DETAILS_PIU detailedItem = new EFORM_QC_OFFICIAL_DETAILS_PIU();
                        detailedItem.OFFICIAL_TYPE = item.OfficialType;
                        detailedItem.RowID = i;
                        objViewModel.QC_OFFICIAL_DETAILS_LIST.Add(detailedItem);
                    }

                });



                foreach (var item in objViewModel.QC_OFFICIAL_DETAILS_LIST.ToList())
                {
                    if (item.RowID == 1)
                    {
                        errorListtemp = objCommonFunction.FetchPDFFilledDataToModel(item, loadedForm);
                        objViewModel.ErrorList.AddRange(errorListtemp);
                    }
                    else
                    {
                        if (item.OFFICIAL_TYPE == "C")
                        {
                            string v = "cb_3_1_" + item.RowID;
                            PdfLoadedCheckBoxField CheckField = loadedForm.Fields[v] as PdfLoadedCheckBoxField;
                            if (CheckField.Checked)
                            {

                                errorListtemp = objCommonFunction.FetchPDFFilledDataToModel(item, loadedForm);
                                objViewModel.ErrorList.AddRange(errorListtemp);
                            }
                            else
                            {
                                objViewModel.QC_OFFICIAL_DETAILS_LIST.Remove(item);
                            }

                        }
                        else if (item.OFFICIAL_TYPE == "E")
                        {
                            string v = "cb_3_2_" + item.RowID;
                            PdfLoadedCheckBoxField CheckField = loadedForm.Fields[v] as PdfLoadedCheckBoxField;
                            if (CheckField.Checked)
                            {
                                errorListtemp = objCommonFunction.FetchPDFFilledDataToModel(item, loadedForm);
                                objViewModel.ErrorList.AddRange(errorListtemp);
                            }
                            else
                            {
                                objViewModel.QC_OFFICIAL_DETAILS_LIST.Remove(item);
                            }


                        }
                        else if (item.OFFICIAL_TYPE == "L")
                        {
                            string v = "cb_3_3_" + item.RowID;
                            PdfLoadedCheckBoxField CheckField = loadedForm.Fields[v] as PdfLoadedCheckBoxField;
                            if (CheckField.Checked)
                            {
                                errorListtemp = objCommonFunction.FetchPDFFilledDataToModel(item, loadedForm);
                                objViewModel.ErrorList.AddRange(errorListtemp);
                            }
                            else
                            {
                                objViewModel.QC_OFFICIAL_DETAILS_LIST.Remove(item);
                            }


                        }
                        else if (item.OFFICIAL_TYPE == "S")
                        {
                            string v = "cb_3_4_" + item.RowID;
                            PdfLoadedCheckBoxField CheckField = loadedForm.Fields[v] as PdfLoadedCheckBoxField;
                            if (CheckField.Checked)
                            {
                                errorListtemp = objCommonFunction.FetchPDFFilledDataToModel(item, loadedForm);
                                objViewModel.ErrorList.AddRange(errorListtemp);
                            }
                            else
                            {
                                objViewModel.QC_OFFICIAL_DETAILS_LIST.Remove(item);
                            }


                        }
                        else if (item.OFFICIAL_TYPE == "A")
                        {
                            string v = "cb_3_5_" + item.RowID;
                            PdfLoadedCheckBoxField CheckField = loadedForm.Fields[v] as PdfLoadedCheckBoxField;
                            if (CheckField.Checked)
                            {
                                errorListtemp = objCommonFunction.FetchPDFFilledDataToModel(item, loadedForm);
                                objViewModel.ErrorList.AddRange(errorListtemp);
                            }
                            else
                            {
                                objViewModel.QC_OFFICIAL_DETAILS_LIST.Remove(item);
                            }


                        }
                        else if (item.OFFICIAL_TYPE == "J")
                        {
                            string v = "cb_4_1_" + item.RowID;
                            PdfLoadedCheckBoxField CheckField = loadedForm.Fields[v] as PdfLoadedCheckBoxField;
                            if (CheckField.Checked)
                            {
                                errorListtemp = objCommonFunction.FetchPDFFilledDataToModel(item, loadedForm);
                                objViewModel.ErrorList.AddRange(errorListtemp);
                            }
                            else
                            {
                                objViewModel.QC_OFFICIAL_DETAILS_LIST.Remove(item);
                            }


                        }
                    }
                }





                #endregion


                #region EFORM_PIU_MIX_DESIGN_DETAILS







                PdfLoadedRadioButtonListField MIX_DESIGN_APPLICABLE_Status = loadedForm.Fields["MIX_DESIGN_APPLICABLE"] as PdfLoadedRadioButtonListField;

                if (MIX_DESIGN_APPLICABLE_Status.SelectedValue == "Y")
                {
                    int count1 = 0;
                    for (int i = 1; i <= 5; i++)
                    {
                        string chkBxName = "cb_4_2_" + i;
                        PdfLoadedCheckBoxField chkBx = loadedForm.Fields[chkBxName] as PdfLoadedCheckBoxField;
                        if (!chkBx.Checked)
                        {
                            count1++;
                        }
                    }
                    if (count1 == 5)
                    {
                        errorListtemp.Clear();
                        errorListtemp.Add("Page-4: Item 4. DETAILS OF MIX DESIGN- Please fill MIX DESIGN details atleast for one design");
                        objViewModel.ErrorList.AddRange(errorListtemp);
                        errorListtemp.Clear();
                        objViewModel.ErrorOccured = true;
                    }
                    else
                    {
                        dbContext.EFORM_MASTER_WORK_ITEM.Where(m => m.ITEM_FLAG == "M").Select(t => t.ITEM_ID).ToList().ForEach(itemID =>
                        {
                            EFORM_MIX_DESIGN_DETAILS_PIU detailedItem = new EFORM_MIX_DESIGN_DETAILS_PIU();
                            detailedItem.ITEM_ID = itemID;
                            objViewModel.MIX_DESIGN_DETAILS_LIST.Add(detailedItem);
                        });
                        int countTemp = 1;
                        foreach (var item in objViewModel.MIX_DESIGN_DETAILS_LIST.ToList())
                        {

                            string v = "cb_4_2_" + countTemp++;
                            PdfLoadedCheckBoxField CheckField = loadedForm.Fields[v] as PdfLoadedCheckBoxField;
                            if (CheckField.Checked)
                            {
                                errorListtemp = objCommonFunction.FetchPDFFilledDataToModel(item, loadedForm);
                                objViewModel.ErrorList.AddRange(errorListtemp);
                            }
                            else
                            {
                                objViewModel.MIX_DESIGN_DETAILS_LIST.Remove(item);
                            }

                        }

                    }


                }

                #endregion

                #region EFORM_PREVIOUS_INSP_DETAILS_PIU
                if (tempVersion < 3.0)
                {
                    inspectionModelList.InspectionList.ForEach(item =>
                    {

                        for (int i = 1; i <= item.RowCount; i++)
                        {
                            EFORM_PREVIOUS_INSP_DETAILS_PIU detailedItem = new EFORM_PREVIOUS_INSP_DETAILS_PIU();
                            detailedItem.Inspection_TYPE = item.InspectionType;
                            detailedItem.RowID = i;
                            objViewModel.PREVIOUS_INSP_DETAILS_LIST.Add(detailedItem);
                        }

                    });

                    objViewModel.PREVIOUS_INSP_DETAILS_LIST.ForEach(item =>
                    {
                        errorListtemp = objCommonFunction.FetchPDFFilledDataToModel(item, loadedForm);
                        objViewModel.ErrorList.AddRange(errorListtemp);

                    }
                   );
                }
                else
                {
                    inspectionModelList.InspectionList.ForEach(item =>
                    {

                        for (int i = 1; i <= item.RowCount; i++)
                        {
                            EFORM_PREVIOUS_INSP_DETAILS_PIU_Temp3_0 detailedItem = new EFORM_PREVIOUS_INSP_DETAILS_PIU_Temp3_0();
                            detailedItem.Inspection_TYPE = item.InspectionType;
                            detailedItem.RowID = i;
                            objViewModel.PREVIOUS_INSP_DETAILS_LIST_temp3_0.Add(detailedItem);
                        }

                    });

                    objViewModel.PREVIOUS_INSP_DETAILS_LIST_temp3_0.ForEach(item =>
                    {
                        errorListtemp = objCommonFunction.FetchPDFFilledDataToModel(item, loadedForm);
                        objViewModel.ErrorList.AddRange(errorListtemp);

                    }
                   );
                    for (int i = 0; i < objViewModel.PREVIOUS_INSP_DETAILS_LIST_temp3_0.Count; i++)
                    {
                        if (objViewModel.PREVIOUS_INSP_DETAILS_LIST_temp3_0[i].INSP_LEVEL == "1")
                        {
                            objViewModel.PREVIOUS_INSP_DETAILS_LIST_temp3_0[i].INSP_LEVEL = "Earth Work& Subgrade";
                        }
                        else if (objViewModel.PREVIOUS_INSP_DETAILS_LIST_temp3_0[i].INSP_LEVEL == "2")
                        {
                            objViewModel.PREVIOUS_INSP_DETAILS_LIST_temp3_0[i].INSP_LEVEL = "Preparatory work";
                        }
                        else if (objViewModel.PREVIOUS_INSP_DETAILS_LIST_temp3_0[i].INSP_LEVEL == "3")
                        {
                            objViewModel.PREVIOUS_INSP_DETAILS_LIST_temp3_0[i].INSP_LEVEL = "CD Works";
                        }
                        else if (objViewModel.PREVIOUS_INSP_DETAILS_LIST_temp3_0[i].INSP_LEVEL == "4")
                        {
                            objViewModel.PREVIOUS_INSP_DETAILS_LIST_temp3_0[i].INSP_LEVEL = "Protection Works";
                        }
                        else if (objViewModel.PREVIOUS_INSP_DETAILS_LIST_temp3_0[i].INSP_LEVEL == "5")
                        {
                            objViewModel.PREVIOUS_INSP_DETAILS_LIST_temp3_0[i].INSP_LEVEL = "Subbase including shoulders";
                        }
                        else if (objViewModel.PREVIOUS_INSP_DETAILS_LIST_temp3_0[i].INSP_LEVEL == "6")
                        {
                            objViewModel.PREVIOUS_INSP_DETAILS_LIST_temp3_0[i].INSP_LEVEL = "Base Course";
                        }
                        else if (objViewModel.PREVIOUS_INSP_DETAILS_LIST_temp3_0[i].INSP_LEVEL == "7")
                        {
                            objViewModel.PREVIOUS_INSP_DETAILS_LIST_temp3_0[i].INSP_LEVEL = "Bituminous Base Course";
                        }
                        else if (objViewModel.PREVIOUS_INSP_DETAILS_LIST_temp3_0[i].INSP_LEVEL == "8")
                        {
                            objViewModel.PREVIOUS_INSP_DETAILS_LIST_temp3_0[i].INSP_LEVEL = "Bituminous Surface Course";
                        }
                        else if (objViewModel.PREVIOUS_INSP_DETAILS_LIST_temp3_0[i].INSP_LEVEL == "9")
                        {
                            objViewModel.PREVIOUS_INSP_DETAILS_LIST_temp3_0[i].INSP_LEVEL = "CC Pavement";
                        }
                        else if (objViewModel.PREVIOUS_INSP_DETAILS_LIST_temp3_0[i].INSP_LEVEL == "10")
                        {
                            objViewModel.PREVIOUS_INSP_DETAILS_LIST_temp3_0[i].INSP_LEVEL = "Signages";
                        }

                    }

                }


                #endregion



                if (objViewModel.ErrorList.Count > 0)
                {
                    objViewModel.ErrorOccured = true;
                }

                return objViewModel;
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "eformcontroller.PdfDataToClassModel()");
                throw;
            }
            finally
            {
                loadedDocument.Close(true);
                dbContext.Dispose();
            }
        }

        private EFORM_QM_VIEWMODEL PdfDataToClassQMModel(string filePath, bool TemplateStatus, char[] chckBoxArr, double tempVersion)
        {
            PdfLoadedDocument loadedDocument = new PdfLoadedDocument(filePath);
            PMGSYEntities dbContext = new PMGSYEntities();


            try
            {
                PDFFiledDataInspectorModel pDFFiledDataInspectorModel = new PDFFiledDataInspectorModel();

                PdfLoadedForm loadedForm = loadedDocument.Form;
                int eformIdtemp = 0;
                try
                {
                    PdfLoadedTextBoxField eformId = loadedForm.Fields["EFORM_ID"] as PdfLoadedTextBoxField;
                    eformIdtemp = Convert.ToInt32(eformId.Text);
                }
                catch (Exception ex)
                {
                    ErrorLog.LogError(ex, "eformComtroller.PdfDataToClassQMModel()");
                }


                EFORM_QM_VIEWMODEL objViewModel = new EFORM_QM_VIEWMODEL();
                CommonFunctions objCommonFunction = new CommonFunctions();


                List<string> errorListtemp = new List<string>();

                objViewModel.ErrorList = new List<string>();

                //-----------------Init-------------------//
                #region  ---- saurabh bhushan 6-13 init----

                VerificationModelList verificationModelList = new VerificationModelList();
                VerificationModelListTempV2_0 verificationModelListTempV2_0 = new VerificationModelListTempV2_0();   //add at 28-07-2022

                GeometricModelList geometricModelList = new GeometricModelList();
                GeometricModelList_Temp2_0 geometricModelList_Temp2_0 = new GeometricModelList_Temp2_0();

                objViewModel.general_details_model = new EFORM_GENERAL_DETAILS_QM();

                objViewModel.QM_ARRANGEMENT_OBS_DETAIL = new QMModels.EFORM_QM_ARRANGEMENTS_OBS_DETAILS(TemplateStatus);
                objViewModel.QM_QUALITY_ATTENTION = new QMModels.EFORM_QM_QUALITY_ATTENTION(TemplateStatus);
                objViewModel.QM_QC_TEST_DETAILS = new List<QMModels.EFORM_QM_QC_TEST_DETAILS>();
                objViewModel.QM_QC_TEST_DETAILS_Temp2_0 = new List<QMModels.EFORM_QM_QC_TEST_DETAILS_Temp2_0>();// add at 28 - 07 - 2022

                objViewModel.QM_TEST_RESULT_VERIFICATION_DETAILS_List = new List<QMModels.EFORM_QM_TEST_RESULT_VERIFICATION_DETAILS>();
                objViewModel.QM_TEST_RESULT_VERIFICATION_DETAILS_List_Temp_2_0 = new List<QMModels.EFORM_QM_TEST_RESULT_VERIFICATION_DETAILS_Temp2_0>();   //add on 28-07-2022

                objViewModel.QM_GEOMETRICS_DETAILS = new QMModels.EFORM_QM_GEOMETRICS_DETAILS();
                objViewModel.QM_GEOMETRICS_OBS_DETAILS_List = new List<QMModels.EFORM_QM_GEOMETRICS_OBS_DETAILS>();
                objViewModel.QM_PRESENT_WORK_DETAILS = new List<QMModels.EFORM_QM_PRESENT_WORK_DETAILS>();
                objViewModel.QM_SIDE_SLOPES = new QMModels.EFORM_QM_SIDE_SLOPES();
                objViewModel.QM_CHILD_SIDE_SLOPE_DETAIL_List = new List<QMModels.EFORM_QM_CHILD_SIDE_SLOPE_DETAIL>();
                objViewModel.QM_CHILD_CUT_SLOPE_DETAIL_List = new List<QMModels.EFORM_QM_CHILD_CUT_SLOPE_DETAIL>();

                PdfLoadedRadioButtonListField IS_NEW_TECH_USED_10_radio = loadedForm.Fields["IS_NEW_TECH_USED_10"] as PdfLoadedRadioButtonListField;
                bool IS_NEW_TECH_USED_10_val = false;
                if (IS_NEW_TECH_USED_10_radio.SelectedValue == "Y")
                {
                    IS_NEW_TECH_USED_10_val = true;
                }

                objViewModel.QM_NEW_TECHNOLOGY_DETAILS = new QMModels.EFORM_QM_NEW_TECHNOLOGY_DETAILS(IS_NEW_TECH_USED_10_val);
                objViewModel.QM_CHILD_UCS_DETAILS = new List<QMModels.EFORM_QM_CHILD_EARTHWORK_SUBGRADE_UCS_DETAILS>();
                objViewModel.QM_CHILD_CBR_DETAILS = new List<QMModels.EFORM_QM_CHILD_EARTHWORK_SUBGRADE_CBR_DETAILS>();
                objViewModel.QM_QOM_EMBANKMENT = new QMModels.EFORM_QM_QOM_EMBANKMENT();
                objViewModel.QM_CHILD_GROUP_SYMBOL_SOIL = new List<QMModels.EFORM_QM_CHILD_GROUP_SYMBOL_SOIL>();
                objViewModel.QM_COMPAQ_EMBANKMENT = new QMModels.EFORM_QM_COMPAQ_EMBANKMENT();
                objViewModel.QM_CHILD_DEGREE_OF_COMPAQ = new List<QMModels.EFORM_QM_CHILD_DEGREE_OF_COMPAQ>();






                #endregion

                #region ---vikky pages 14-18 init---

                Granular_UCS_details_list granular_ucs_details = new Granular_UCS_details_list();
                Granular_QOM_OBS_list granular_QOM_OBS_details = new Granular_QOM_OBS_list();

                Base_coarse_layer1_list base_coarse_layer1_list = new Base_coarse_layer1_list();
                Base_coarse_layer1_workmanship_list base_coarse_layer1_workmanship_list = new Base_coarse_layer1_workmanship_list();


                Base_coarse_layer2_list base_coarse_layer2_list = new Base_coarse_layer2_list();
                Base_coarse_layer2_workmanship_list base_coarse_layer2_workmanship_list = new Base_coarse_layer2_workmanship_list();

                Base_coarse_layer3_list base_coarse_layer3_list = new Base_coarse_layer3_list();
                Base_coarse_layer3_workmanship_list base_coarse_layer3_workmanship_list = new Base_coarse_layer3_workmanship_list();

                #endregion

                #region --- Srishti Page 19-23 init ---

                PdfLoadedRadioButtonListField IS_NEW_TECH_USED_19_radio = loadedForm.Fields["IS_NEW_TECH_USED_19"] as PdfLoadedRadioButtonListField;
                bool IS_NEW_TECH_USED_19_val = false;
                if (IS_NEW_TECH_USED_19_radio.SelectedValue == "Y")
                {
                    IS_NEW_TECH_USED_19_val = true;
                }

                objViewModel.QM_BITUMINOUS_BASE_COURSE = new QMModels.EFORM_QM_BITUMINOUS_BASE_COURSE(IS_NEW_TECH_USED_19_val);
                objViewModel.QM_CHILD_BITUMINOUS_OBSERVATION_DETAILS = new List<QMModels.EFORM_QM_CHILD_BITUMINOUS_OBSERVATION_DETAILS>();

                PdfLoadedRadioButtonListField NEW_TECH_QTY_USED_20_radio = loadedForm.Fields["NEW_TECH_QTY_USED_20"] as PdfLoadedRadioButtonListField;
                bool NEW_TECH_QTY_USED_20_val = false;
                if (NEW_TECH_QTY_USED_20_radio.SelectedValue == "Y")
                {
                    NEW_TECH_QTY_USED_20_val = true;
                }

                objViewModel.QM_BITUMINOUS_SURFACE_COURSE = new QMModels.EFORM_QM_BITUMINOUS_SURFACE_COURSE(NEW_TECH_QTY_USED_20_val);
                objViewModel.QM_CHILD_BITUMINOUS_SURFACE_COARSE_DETAILS = new List<QMModels.EFORM_QM_CHILD_BITUMINOUS_SURFACE_COARSE_DETAILS>();


                objViewModel.QM_CHILD_SHOULDERS_MATERIAL_DETAILS = new List<QMModels.EFORM_QM_CHILD_SHOULDERS_MATERIAL_DETAILS>();
                objViewModel.QM_CHILD_SHOULDERS_UCS_DETAILS = new List<QMModels.EFORM_QM_CHILD_SHOULDERS_UCS_DETAILS>();

                PdfLoadedRadioButtonListField IS_NEW_TECH_USED_22_radio = loadedForm.Fields["IS_NEW_TECH_USED_22"] as PdfLoadedRadioButtonListField;
                bool IS_NEW_TECH_USED_22_val = false;
                if (IS_NEW_TECH_USED_22_radio.SelectedValue == "Y")
                {
                    IS_NEW_TECH_USED_22_val = true;
                }
                objViewModel.QM_SHOULDERS = new QMModels.EFORM_QM_SHOULDERS(IS_NEW_TECH_USED_22_val);



                #endregion

                #region ---vikky pages 23-27 init----


                EFORM_CHILD_CDWORKS_PIPE_CULVERTS_List CDWORKS_PIPE_CULVERTS_List = new EFORM_CHILD_CDWORKS_PIPE_CULVERTS_List();

                EFORM_CHILD_CDWORKS_SLAB_CULVERTS_List CDWORKS_SLAB_CULVERTS_List = new EFORM_CHILD_CDWORKS_SLAB_CULVERTS_List();

                EFORM_CHILD_PROT_WORKS_QOM_List PROT_WORKS_QOM_List = new EFORM_CHILD_PROT_WORKS_QOM_List();

                EFORM_CHILD_PROT_WORKS_WORKMANSHIP_OF_RS_List PROT_WORKS_WORKMANSHIP_OF_RS_List = new EFORM_CHILD_PROT_WORKS_WORKMANSHIP_OF_RS_List();

                EFORM_CHILD_CRASH_BARRIERS_OBSERVATION_List CRASH_BARRIERS_OBSERVATION_List = new EFORM_CHILD_CRASH_BARRIERS_OBSERVATION_List();

                EFORM_CHILD_SD_AND_CW_DRAINS_List SD_AND_CW_DRAINS_List = new EFORM_CHILD_SD_AND_CW_DRAINS_List();


                objViewModel.CDWORKS_PIPE_CULVERTS_details = new EFORM_CDWORKS_PIPE_CULVERTS_QM();
                objViewModel.CHILD_CDWORKS_PIPE_CULVERTS_list = new List<EFORM_CHILD_CDWORKS_PIPE_CULVERTS_DETAILS_QM>();


                objViewModel.CDWORKS_SLAB_CULVERTS_details = new EFORM_CDWORKS_SLAB_CULVERTS_QM();
                objViewModel.CHILD_CDWORKS_SLAB_CULVERTS_list = new List<EFORM_CHILD_CDWORKS_SLAB_CULVERTS_DETAILS_QM>();

                objViewModel.PROTECTION_WORK_details = new EFORM_PROTECTION_WORK_QM();
                objViewModel.CHILD_PROT_WORKS_QOM_list = new List<EFORM_CHILD_PROT_WORKS_QOM_DETAILS_QM>();
                objViewModel.CHILD_PROT_WORKS_WORKMANSHIP_OF_RS_list = new List<EFORM_CHILD_PROT_WORKS_WORKMANSHIP_OF_RS_DETAILS_QM>();

                objViewModel.CRASH_BARRIERS_ROAD_SAFETY_details = new EFORM_CRASH_BARRIERS_ROAD_SAFETY_QM();
                objViewModel.CHILD_CRASH_BARRIERS_OBSERVATION_list = new List<EFORM_CHILD_CRASH_BARRIERS_OBSERVATION_DETAILS_QM>();

                objViewModel.SIDE_AND_CATCH_DRAINS_EARTHEN_details = new EFORM_SIDE_AND_CATCH_DRAINS_EARTHEN_QM();
                objViewModel.CHILD_SD_AND_CW_DRAINS_list = new List<EFORM_CHILD_SD_AND_CW_DRAINS_DETAILS_QM>();

                #endregion

                #region  ----Bhushan 27-30 init----

                objViewModel.QM_CC_SR_PVAEMENTS = new QMModels.EFORM_QM_CC_SR_PVAEMENTS();
                objViewModel.QM_CHILD_CC_AND_SR_PAVEMENTS_OBSERVATION_DETAILS = new List<QMModels.EFORM_QM_CHILD_CC_AND_SR_PAVEMENTS_OBSERVATION_DETAILS>();
                objViewModel.QM_CC_PUCCA_DRAINS = new QMModels.EFORM_QM_CC_PUCCA_DRAINS();
                objViewModel.QM_CHILD_CC_PUCCA_DRAINS_OBSERVATION_DETAILS = new List<QMModels.EFORM_QM_CHILD_CC_PUCCA_DRAINS_OBSERVATION_DETAILS>();

                objViewModel.QM_ROAD_FURNITURE_MARKINGS = new QMModels.EFORM_QM_ROAD_FURNITURE_MARKINGS(TemplateStatus);
                objViewModel.QM_CHILD_ROAD_FURNITURE_MARKINGS_OBSERVATION_DETAILS = new List<QMModels.EFORM_QM_CHILD_ROAD_FURNITURE_MARKINGS_OBSERVATION_DETAILS>();


                #endregion

                #region --- Srishti Page  34-38 init ---
                objViewModel.QM_QUALITY_GRADING = new QMModels.EFORM_QM_QUALITY_GRADING(TemplateStatus);
                objViewModel.QM_OVERALL_GRADING = new QMModels.EFORM_QM_OVERALL_GRADING(TemplateStatus);

                #endregion


                //-----------------Model reading Code-------------------//

                #region ---saurabh 6-10  Model reading Code----

                #region EFORM_GENERAL_DETAILS_QM

                objViewModel.ErrorList = objCommonFunction.FetchPDFFilledDataToQMModel(objViewModel.general_details_model, loadedForm);

                #endregion

                #region QM_PRESENT_WORK_DETAILS


                if (TemplateStatus != true)
                {
                    for (int i = 1; i <= 10; i++)
                    {

                        string ItemId = "ITEM_ID" + i;
                        string RoadFrom = "ROAD_FROM_" + i;
                        string RoadTo = "ROAD_TO_" + i;
                        PdfLoadedCheckBoxField CheckField = loadedForm.Fields[ItemId] as PdfLoadedCheckBoxField;
                        PdfLoadedTextBoxField RoadFromText = loadedForm.Fields[RoadFrom] as PdfLoadedTextBoxField;
                        PdfLoadedTextBoxField RoadToText = loadedForm.Fields[RoadTo] as PdfLoadedTextBoxField;

                        if (CheckField.Checked == true)
                        {
                            QMModels.EFORM_QM_PRESENT_WORK_DETAILS detailedItem = new QMModels.EFORM_QM_PRESENT_WORK_DETAILS();
                            if (RoadFromText.Text == "")
                            {
                                string Item = dbContext.EFORM_MASTER_WORK_ITEM.Where(s => s.ITEM_FLAG == "P" && s.ITEM_ID == i).Select(m => m.ITEM_DESC).FirstOrDefault();
                                errorListtemp.Clear();
                                errorListtemp.Add("Page-6: 1.GENERAL DETAILS- IX.Present status of work: Please Enter RD from (km) for " + Item);
                                objViewModel.ErrorList.AddRange(errorListtemp);
                                errorListtemp.Clear();
                                objViewModel.ErrorOccured = true;
                            }
                            if (RoadToText.Text == "")
                            {
                                string Item = dbContext.EFORM_MASTER_WORK_ITEM.Where(s => s.ITEM_FLAG == "P" && s.ITEM_ID == i).Select(m => m.ITEM_DESC).FirstOrDefault();
                                errorListtemp.Clear();
                                errorListtemp.Add("Page-6: 1.GENERAL DETAILS- IX.Present status of work: Please Enter RD To (km) for " + Item);
                                objViewModel.ErrorList.AddRange(errorListtemp);
                                errorListtemp.Clear();
                                objViewModel.ErrorOccured = true;
                            }

                            if (RoadFromText.Text != "" && RoadToText.Text != "")
                            {
                                detailedItem.ROAD_FROM = Convert.ToDecimal(RoadFromText.Text);
                                detailedItem.ROAD_TO = Convert.ToDecimal(RoadToText.Text);
                                detailedItem.ITEM_ID = i;
                                objViewModel.QM_PRESENT_WORK_DETAILS.Add(detailedItem);
                            }




                        }
                    }
                }

                #endregion

                #region EFORM_QM_ARRANGEMENTS_OBS_DETAILS
                if (chckBoxArr[2] == 'Y')
                {
                    errorListtemp = objCommonFunction.FetchPDFFilledDataToQMModel(objViewModel.QM_ARRANGEMENT_OBS_DETAIL, loadedForm);
                    objViewModel.ErrorList.AddRange(errorListtemp);

                }


                #endregion

                if (chckBoxArr[3] == 'Y')
                {
                    #region EFORM_QM_QUALITY_ATTENTION

                    errorListtemp = objCommonFunction.FetchPDFFilledDataToQMModel(objViewModel.QM_QUALITY_ATTENTION, loadedForm);
                    objViewModel.ErrorList.AddRange(errorListtemp);

                    #endregion

                    #region QM_QC_TEST_DETAILS


                    if (TemplateStatus != true)
                    {

                        int count = 0;
                        string testingAdequateStatus = "Y";
                        if (tempVersion < 2.0)
                        {
                            for (int i = 1; i <= 6; i++)
                            {
                                int t = 1;
                                string chkBxName = "cb_8_1_" + i;
                                string testingAdequate = string.Empty;


                                if (i >= 5)
                                {
                                    testingAdequate = "IS_TESTING_ADEQUATE_" + 2 + "" + (i - 5);
                                }
                                else
                                {
                                    testingAdequate = "IS_TESTING_ADEQUATE_" + t + "" + (i + 5);
                                }




                                PdfLoadedCheckBoxField chkBx = loadedForm.Fields[chkBxName] as PdfLoadedCheckBoxField;
                                PdfLoadedRadioButtonListField radBtn = loadedForm.Fields[testingAdequate] as PdfLoadedRadioButtonListField;
                                if (!chkBx.Checked)
                                {
                                    count++;
                                }
                                if (radBtn.SelectedValue == "N")
                                {
                                    testingAdequateStatus = "N";
                                }
                            }
                        }
                        else
                        {
                            for (int j = 0; j < 3; j++)
                            {
                                for (int i = 1; i <= 6; i++)
                                {
                                    int t = 1;
                                    string chkBxName = "cb_8_1_" + i;
                                    string testingAdequate = string.Empty;

                                    if (j == 0)
                                    {
                                        if (i >= 5)
                                        {
                                            testingAdequate = "IS_TESTING_ADEQUATE_" + 2 + "" + (i - 5);
                                        }
                                        else
                                        {
                                            testingAdequate = "IS_TESTING_ADEQUATE_" + t + "" + (i + 5);
                                        }
                                    }
                                    else
                                    {
                                        if (i >= 5)
                                        {
                                            testingAdequate = "IS_TESTING_ADEQUATE_" + j + "_" + 2 + "" + (i - 5);
                                        }
                                        else
                                        {
                                            testingAdequate = "IS_TESTING_ADEQUATE_" + j + "_" + t + "" + (i + 5);
                                        }
                                    }


                                    PdfLoadedRadioButtonListField radBtn = loadedForm.Fields[testingAdequate] as PdfLoadedRadioButtonListField;
                                    if (j == 0)
                                    {
                                        PdfLoadedCheckBoxField chkBx = loadedForm.Fields[chkBxName] as PdfLoadedCheckBoxField;
                                        if (!chkBx.Checked)
                                        {
                                            count++;
                                        }
                                    }

                                    if (radBtn.SelectedValue == "N")
                                    {
                                        testingAdequateStatus = "N";
                                    }
                                }
                            }

                        }
                        if (testingAdequateStatus == "N")
                        {
                            PdfLoadedCheckBoxField IS_NEGLIGENCE_CHECKED = loadedForm.Fields["IS_NEGLIGENCE"] as PdfLoadedCheckBoxField;
                            PdfLoadedCheckBoxField IS_LOE_CHECKED = loadedForm.Fields["IS_LOE"] as PdfLoadedCheckBoxField;
                            PdfLoadedCheckBoxField IS_LOK_CHECKED = loadedForm.Fields["IS_LOK"] as PdfLoadedCheckBoxField;
                            PdfLoadedCheckBoxField IS_OTHER_CHECKED = loadedForm.Fields["IS_OTHER"] as PdfLoadedCheckBoxField;
                            if (IS_NEGLIGENCE_CHECKED.Checked == false && IS_LOE_CHECKED.Checked == false && IS_LOK_CHECKED.Checked == false && IS_OTHER_CHECKED.Checked == false)
                            {
                                errorListtemp.Clear();
                                errorListtemp.Add("Page-8: Item 3. ATTENTION TO QUALITY- II. b) Please select atleast one reason for less testing");
                                objViewModel.ErrorList.AddRange(errorListtemp);

                            }


                        }
                        if (count == 6)
                        {
                            errorListtemp.Clear();
                            errorListtemp.Add("Page-8: Item 3. ATTENTION TO QUALITY- II. a) Adequacy of quality control tests, as per QCR-1: Please fill Adequacy of quality control tests, as per QCR-1 details atleast for one row");
                            objViewModel.ErrorList.AddRange(errorListtemp);
                            errorListtemp.Clear();
                            objViewModel.ErrorOccured = true;
                        }
                        else
                        {
                            if (tempVersion < 2.0)
                            {
                                dbContext.EFORM_MASTER_WORK_ITEM.Where(m => m.ITEM_FLAG == "A").Select(t => t.ITEM_ID).ToList().ForEach(itemID =>
                                {

                                    QMModels.EFORM_QM_QC_TEST_DETAILS detailedItem = new QMModels.EFORM_QM_QC_TEST_DETAILS();
                                    detailedItem.ITEM_ID = itemID;
                                    objViewModel.QM_QC_TEST_DETAILS.Add(detailedItem);


                                });

                                int countTemp = 1;
                                foreach (var item in objViewModel.QM_QC_TEST_DETAILS.ToList())
                                {

                                    string v = "cb_8_1_" + countTemp++;
                                    PdfLoadedCheckBoxField CheckField = loadedForm.Fields[v] as PdfLoadedCheckBoxField;
                                    if (CheckField.Checked)
                                    {
                                        errorListtemp = objCommonFunction.FetchPDFFilledDataToQMModel(item, loadedForm);
                                        objViewModel.ErrorList.AddRange(errorListtemp);
                                    }
                                    else
                                    {
                                        objViewModel.QM_QC_TEST_DETAILS.Remove(item);
                                    }

                                }
                            }
                            else
                            {
                                dbContext.EFORM_MASTER_WORK_ITEM.Where(m => m.ITEM_FLAG == "A").Select(t => t.ITEM_ID).ToList().ForEach(itemID =>
                                {
                                    for (int i = 1; i < 4; i++)
                                    {
                                        QMModels.EFORM_QM_QC_TEST_DETAILS_Temp2_0 detailedItem = new QMModels.EFORM_QM_QC_TEST_DETAILS_Temp2_0();
                                        detailedItem.ITEM_ID = itemID;
                                        detailedItem.ROW_ID = i;
                                        objViewModel.QM_QC_TEST_DETAILS_Temp2_0.Add(detailedItem);
                                    }

                                });

                                int countTemp = 1;
                                foreach (var item in objViewModel.QM_QC_TEST_DETAILS_Temp2_0.ToList())
                                {

                                    string v = "cb_8_1_" + countTemp;
                                    PdfLoadedCheckBoxField CheckField = loadedForm.Fields[v] as PdfLoadedCheckBoxField;
                                    if (CheckField.Checked)
                                    {
                                        errorListtemp = objCommonFunction.FetchPDFFilledDataToQMModel(item, loadedForm);
                                        objViewModel.ErrorList.AddRange(errorListtemp);

                                        if ((item.ROW_ID == 2 || item.ROW_ID == 3) && item.IS_TESTING_ADEQUATE == null && item.REQD_TEST_NUMBER == null && item.TEST_NAME == null && item.CONDUCTED_TEST_NUMBER == null)
                                        {
                                            objViewModel.QM_QC_TEST_DETAILS_Temp2_0.Remove(item);
                                        }

                                    }
                                    else
                                    {
                                        objViewModel.QM_QC_TEST_DETAILS_Temp2_0.Remove(item);
                                    }
                                    if (item.ROW_ID == 3)
                                    {
                                        countTemp++;
                                    }

                                }
                                if (objViewModel.QM_QC_TEST_DETAILS_Temp2_0.Count > 2)
                                {
                                    for (int i = 1; i < objViewModel.QM_QC_TEST_DETAILS_Temp2_0.Count; i++)
                                    {
                                        if (objViewModel.QM_QC_TEST_DETAILS_Temp2_0[i].EXECUTED_QUANTITY == null)
                                        {
                                            objViewModel.QM_QC_TEST_DETAILS_Temp2_0[i].EXECUTED_QUANTITY = objViewModel.QM_QC_TEST_DETAILS_Temp2_0[i - 1].EXECUTED_QUANTITY;
                                        }
                                        if (objViewModel.QM_QC_TEST_DETAILS_Temp2_0[i].DPR_QUANTITY == null)
                                        {
                                            objViewModel.QM_QC_TEST_DETAILS_Temp2_0[i].DPR_QUANTITY = objViewModel.QM_QC_TEST_DETAILS_Temp2_0[i - 1].DPR_QUANTITY;
                                        }
                                    }
                                }



                            }



                        }




                    }

                    #endregion

                    #region QM_TEST_RESULT_VERIFICATION_DETAILS
                    if (TemplateStatus != true)
                    {
                        if (tempVersion < 2.0)
                        {
                            verificationModelList.VerificationList.ForEach(item =>
                            {

                                for (int i = 1; i <= item.RowCount; i++)
                                {
                                    QMModels.EFORM_QM_TEST_RESULT_VERIFICATION_DETAILS detailedItem = new QMModels.EFORM_QM_TEST_RESULT_VERIFICATION_DETAILS();
                                    detailedItem.VerificationType = item.VerificationType;
                                    detailedItem.RowID = i;
                                    objViewModel.QM_TEST_RESULT_VERIFICATION_DETAILS_List.Add(detailedItem);
                                }

                            });
                            foreach (var item in objViewModel.QM_TEST_RESULT_VERIFICATION_DETAILS_List.ToList())
                            //  objViewModel.QM_TEST_RESULT_VERIFICATION_DETAILS_List.ForEach(item =>
                            {
                                if (item.RowID == 1)
                                {
                                    errorListtemp = objCommonFunction.FetchPDFFilledDataToQMModel(item, loadedForm);
                                    objViewModel.ErrorList.AddRange(errorListtemp);
                                }
                                else
                                {
                                    string v = "cb_8_2_" + item.RowID;
                                    PdfLoadedCheckBoxField CheckField = loadedForm.Fields[v] as PdfLoadedCheckBoxField;
                                    if (CheckField.Checked)
                                    {
                                        errorListtemp = objCommonFunction.FetchPDFFilledDataToQMModel(item, loadedForm);
                                        objViewModel.ErrorList.AddRange(errorListtemp);
                                    }
                                    else
                                    {
                                        objViewModel.QM_TEST_RESULT_VERIFICATION_DETAILS_List.Remove(item);
                                    }
                                }


                            }
                            //  );
                        }
                        else   //add on 28-07-2022
                        {
                            verificationModelListTempV2_0.VerificationList.ForEach(item =>
                            {

                                for (int i = 1; i <= item.RowCount; i++)
                                {
                                    QMModels.EFORM_QM_TEST_RESULT_VERIFICATION_DETAILS_Temp2_0 detailedItem = new QMModels.EFORM_QM_TEST_RESULT_VERIFICATION_DETAILS_Temp2_0();
                                    detailedItem.VerificationType = item.VerificationType;
                                    detailedItem.RowID = i;
                                    objViewModel.QM_TEST_RESULT_VERIFICATION_DETAILS_List_Temp_2_0.Add(detailedItem);
                                }

                            });
                            int countChecked = 0;
                            int countUnchecked = 0;
                            foreach (var item in objViewModel.QM_TEST_RESULT_VERIFICATION_DETAILS_List_Temp_2_0.ToList())
                            //  objViewModel.QM_TEST_RESULT_VERIFICATION_DETAILS_List.ForEach(item =>
                            {
                                if (item.RowID == 1)
                                {
                                    errorListtemp = objCommonFunction.FetchPDFFilledDataToQMModel(item, loadedForm);
                                    objViewModel.ErrorList.AddRange(errorListtemp);
                                }
                                else
                                {

                                    string v = "cbrd_3_2_" + item.RowID;
                                    PdfLoadedCheckBoxField CheckField = loadedForm.Fields[v] as PdfLoadedCheckBoxField;

                                    if (CheckField.Checked)
                                    {
                                        countChecked++;
                                        errorListtemp = objCommonFunction.FetchPDFFilledDataToQMModel(item, loadedForm);
                                        objViewModel.ErrorList.AddRange(errorListtemp);
                                        if (item.RowID == 2 || item.RowID == 3 || item.RowID == 4)
                                        {
                                            objViewModel.QM_TEST_RESULT_VERIFICATION_DETAILS_List_Temp_2_0[countChecked].ROAD_LOC = objViewModel.QM_TEST_RESULT_VERIFICATION_DETAILS_List_Temp_2_0[0].ROAD_LOC;
                                        }
                                        else if (item.RowID == 6 || item.RowID == 7 || item.RowID == 8)
                                        {
                                            objViewModel.QM_TEST_RESULT_VERIFICATION_DETAILS_List_Temp_2_0[countChecked].ROAD_LOC = objViewModel.QM_TEST_RESULT_VERIFICATION_DETAILS_List_Temp_2_0[4 - countUnchecked].ROAD_LOC;
                                        }
                                        else if (item.RowID == 10 || item.RowID == 11 || item.RowID == 12)
                                        {
                                            objViewModel.QM_TEST_RESULT_VERIFICATION_DETAILS_List_Temp_2_0[countChecked].ROAD_LOC = objViewModel.QM_TEST_RESULT_VERIFICATION_DETAILS_List_Temp_2_0[8 - countUnchecked].ROAD_LOC;
                                        }
                                    }
                                    else
                                    {
                                        countUnchecked++;
                                        objViewModel.QM_TEST_RESULT_VERIFICATION_DETAILS_List_Temp_2_0.Remove(item);
                                    }
                                }


                            }



                        }

                    }


                    #endregion
                }


                if (chckBoxArr[4] == 'Y')
                {
                    #region QM_GEOMETRICS_DETAILS

                    errorListtemp = objCommonFunction.FetchPDFFilledDataToQMModel(objViewModel.QM_GEOMETRICS_DETAILS, loadedForm);
                    objViewModel.ErrorList.AddRange(errorListtemp);
                    if (tempVersion < 2.0)
                    {

                        #endregion

                        #region  QM_GEOMETRICS_OBS_DETAILS_List
                        geometricModelList.GeometricList.ForEach(item =>
                        {

                            for (int i = 1; i <= item.RowCount; i++)
                            {
                                QMModels.EFORM_QM_GEOMETRICS_OBS_DETAILS detailedItem = new QMModels.EFORM_QM_GEOMETRICS_OBS_DETAILS();
                                detailedItem.TABLE_FLAG = item.GeometricType;
                                detailedItem.GEOMETRIC_TYPE = item.GeometricType;
                                detailedItem.RowID = i;
                                objViewModel.QM_GEOMETRICS_OBS_DETAILS_List.Add(detailedItem);
                            }

                        });

                        foreach (var item in objViewModel.QM_GEOMETRICS_OBS_DETAILS_List.ToList())
                        {
                            if (item.RowID == 1)
                            {
                                errorListtemp = objCommonFunction.FetchPDFFilledDataToQMModel(item, loadedForm);
                                objViewModel.ErrorList.AddRange(errorListtemp);
                            }
                            else
                            {
                                if (item.TABLE_FLAG == "R")
                                {
                                    string v = "cb_9_1_" + item.RowID;
                                    PdfLoadedCheckBoxField CheckField = loadedForm.Fields[v] as PdfLoadedCheckBoxField;
                                    if (CheckField.Checked)
                                    {

                                        errorListtemp = objCommonFunction.FetchPDFFilledDataToQMModel(item, loadedForm);
                                        objViewModel.ErrorList.AddRange(errorListtemp);
                                    }
                                    else
                                    {
                                        objViewModel.QM_GEOMETRICS_OBS_DETAILS_List.Remove(item);
                                    }

                                }
                                else if (item.TABLE_FLAG == "S")
                                {
                                    string v = "cb_9_2_" + item.RowID;
                                    PdfLoadedCheckBoxField CheckField = loadedForm.Fields[v] as PdfLoadedCheckBoxField;
                                    if (CheckField.Checked)
                                    {
                                        errorListtemp = objCommonFunction.FetchPDFFilledDataToQMModel(item, loadedForm);
                                        objViewModel.ErrorList.AddRange(errorListtemp);
                                    }
                                    else
                                    {
                                        objViewModel.QM_GEOMETRICS_OBS_DETAILS_List.Remove(item);
                                    }


                                }
                                else if (item.TABLE_FLAG == "L")
                                {
                                    bool flag = false;
                                    try
                                    {
                                        PdfLoadedCheckBoxField terrainChk = loadedForm.Fields["CB_TERRAIN"] as PdfLoadedCheckBoxField;
                                        if (terrainChk.Checked)
                                        {
                                            flag = true;
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        flag = true;
                                        ErrorLog.LogError(ex, "eformcontroller.PdfDataToClassQMModel()");
                                    }

                                    if (flag)
                                    {
                                        string v = "cb_10_1_" + item.RowID;
                                        PdfLoadedCheckBoxField CheckField = loadedForm.Fields[v] as PdfLoadedCheckBoxField;
                                        if (CheckField.Checked)
                                        {
                                            errorListtemp = objCommonFunction.FetchPDFFilledDataToQMModel(item, loadedForm);
                                            objViewModel.ErrorList.AddRange(errorListtemp);
                                        }
                                        else
                                        {
                                            objViewModel.QM_GEOMETRICS_OBS_DETAILS_List.Remove(item);
                                        }
                                    }



                                }
                            }

                            if ((item.TABLE_FLAG == "S") && item.ROAD_LOC == null && item.C4IIA_ELEVATION_PER_DPR == null && item.C4IIA_ELEVATION_PER_ACTUAL == null && item.C4IIA_ELEVATION_PER_GRADE == null && item.C4IIB_EXTRA_WIDENING_DPR == null && item.C4IIB_EXTRA_WIDENING_ACTUAL == null && item.C4IIB_EXTRA_WIDENING_GRADE == null)
                            {
                                objViewModel.QM_GEOMETRICS_OBS_DETAILS_List.Remove(item);
                            }
                        }

                    }
                    else
                    {

                        #endregion

                        #region  QM_GEOMETRICS_OBS_DETAILS_List
                        geometricModelList_Temp2_0.GeometricList.ForEach(item =>
                        {

                            for (int i = 1; i <= item.RowCount; i++)
                            {
                                QMModels.EFORM_QM_GEOMETRICS_OBS_DETAILS detailedItem = new QMModels.EFORM_QM_GEOMETRICS_OBS_DETAILS();
                                detailedItem.TABLE_FLAG = item.GeometricType;
                                detailedItem.GEOMETRIC_TYPE = item.GeometricType;
                                detailedItem.RowID = i;
                                objViewModel.QM_GEOMETRICS_OBS_DETAILS_List.Add(detailedItem);
                            }

                        });

                        foreach (var item in objViewModel.QM_GEOMETRICS_OBS_DETAILS_List.ToList())
                        {
                            if (item.RowID == 1)
                            {
                                errorListtemp = objCommonFunction.FetchPDFFilledDataToQMModel(item, loadedForm);
                                objViewModel.ErrorList.AddRange(errorListtemp);
                            }
                            else
                            {
                                if (item.TABLE_FLAG == "R")
                                {
                                    string v = "cb_9_1_" + item.RowID;
                                    PdfLoadedCheckBoxField CheckField = loadedForm.Fields[v] as PdfLoadedCheckBoxField;
                                    if (CheckField.Checked)
                                    {

                                        errorListtemp = objCommonFunction.FetchPDFFilledDataToQMModel(item, loadedForm);
                                        objViewModel.ErrorList.AddRange(errorListtemp);
                                    }
                                    else
                                    {
                                        objViewModel.QM_GEOMETRICS_OBS_DETAILS_List.Remove(item);
                                    }

                                }
                                else if (item.TABLE_FLAG == "S")
                                {
                                    string v = "cb_9_2_" + item.RowID;
                                    PdfLoadedCheckBoxField CheckField = loadedForm.Fields[v] as PdfLoadedCheckBoxField;
                                    if (CheckField.Checked)
                                    {
                                        errorListtemp = objCommonFunction.FetchPDFFilledDataToQMModel(item, loadedForm);
                                        objViewModel.ErrorList.AddRange(errorListtemp);
                                    }
                                    else
                                    {
                                        objViewModel.QM_GEOMETRICS_OBS_DETAILS_List.Remove(item);
                                    }


                                }
                                else if (item.TABLE_FLAG == "L")
                                {
                                    bool flag = false;
                                    try
                                    {
                                        PdfLoadedCheckBoxField terrainChk = loadedForm.Fields["CB_TERRAIN"] as PdfLoadedCheckBoxField;
                                        if (terrainChk.Checked)
                                        {
                                            flag = true;
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        flag = true;
                                        ErrorLog.LogError(ex, "eformcontroller.PdfDataToClassQMModel()");
                                    }

                                    if (flag)
                                    {
                                        string v = "cb_10_1_" + item.RowID;
                                        PdfLoadedCheckBoxField CheckField = loadedForm.Fields[v] as PdfLoadedCheckBoxField;
                                        if (CheckField.Checked)
                                        {
                                            errorListtemp = objCommonFunction.FetchPDFFilledDataToQMModel(item, loadedForm);
                                            objViewModel.ErrorList.AddRange(errorListtemp);
                                        }
                                        else
                                        {
                                            objViewModel.QM_GEOMETRICS_OBS_DETAILS_List.Remove(item);
                                        }
                                    }



                                }
                            }
                            if ((item.TABLE_FLAG == "S") && item.ROAD_LOC == null && item.C4IIA_ELEVATION_PER_DPR == null && item.C4IIA_ELEVATION_PER_ACTUAL == null && item.C4IIA_ELEVATION_PER_GRADE == null && item.C4IIB_EXTRA_WIDENING_DPR == null && item.C4IIB_EXTRA_WIDENING_ACTUAL == null && item.C4IIB_EXTRA_WIDENING_GRADE == null)
                            {
                                objViewModel.QM_GEOMETRICS_OBS_DETAILS_List.Remove(item);
                            }
                        }

                    }





                    #endregion

                }
                if (chckBoxArr[5] == 'Y')
                {
                    #region QM_SIDE_SLOPES

                    errorListtemp = objCommonFunction.FetchPDFFilledDataToQMModel(objViewModel.QM_SIDE_SLOPES, loadedForm);
                    objViewModel.ErrorList.AddRange(errorListtemp);
                    if (objViewModel.QM_SIDE_SLOPES.IS_ANALYSIS_DONE == "N" && objViewModel.QM_SIDE_SLOPES.OBSERVATIONS == null)
                    {
                        errorListtemp.Clear();
                        errorListtemp.Add("Page - 13: Item 5.EARTHWORK & SUB GRADE- IV.Side slopes and profile of embankment- (c) if stability analysis is NO then observations about adequacy of slopes should provided");

                        objViewModel.ErrorList.AddRange(errorListtemp);
                        errorListtemp.Clear();
                        objViewModel.ErrorOccured = true;
                    }

                    #endregion

                    #region QM_CHILD_SIDE_SLOPE_DETAIL_List
                    // cb_13_1_1
                    QMModels.EFORM_QM_CHILD_SIDE_SLOPE_DETAIL ChildSideSlope = new QMModels.EFORM_QM_CHILD_SIDE_SLOPE_DETAIL();
                    for (int i = 1; i <= ChildSideSlope.RowID; i++)
                    {
                        QMModels.EFORM_QM_CHILD_SIDE_SLOPE_DETAIL detailedItem = new QMModels.EFORM_QM_CHILD_SIDE_SLOPE_DETAIL();
                        detailedItem.RowID = i;
                        objViewModel.QM_CHILD_SIDE_SLOPE_DETAIL_List.Add(detailedItem);
                    }

                    //objViewModel.QM_CHILD_SIDE_SLOPE_DETAIL_List.ForEach(item =>
                    //{ 
                    foreach (var item in objViewModel.QM_CHILD_SIDE_SLOPE_DETAIL_List.ToList())
                    {
                        if (item.RowID == 1)
                        {
                            errorListtemp = objCommonFunction.FetchPDFFilledDataToQMModel(item, loadedForm);
                            objViewModel.ErrorList.AddRange(errorListtemp);
                        }
                        else
                        {
                            string v = "cb_13_1_" + item.RowID;
                            PdfLoadedCheckBoxField CheckField = loadedForm.Fields[v] as PdfLoadedCheckBoxField;
                            if (CheckField.Checked)
                            {
                                errorListtemp = objCommonFunction.FetchPDFFilledDataToQMModel(item, loadedForm);
                                objViewModel.ErrorList.AddRange(errorListtemp);
                            }
                            else
                            {
                                objViewModel.QM_CHILD_SIDE_SLOPE_DETAIL_List.Remove(item);
                            }
                        }


                    }

                    foreach (var item in objViewModel.QM_CHILD_SIDE_SLOPE_DETAIL_List.ToList())
                    {
                        if (item.LOCATION_RD_13_1 == null && item.SS_OBSERVED_BY_QM == null
                        && item.IS_SS_SATISFACTORY == null && item.IS_PROFILE_SATISFACTORY == null
                        && item.GRADING == null)
                        {
                            objViewModel.QM_CHILD_SIDE_SLOPE_DETAIL_List.Remove(item);
                        }

                    }
                    //      );

                    #endregion


                    #region QM_CHILD_CUT_SLOPE_DETAIL_List
                    QMModels.EFORM_QM_CHILD_CUT_SLOPE_DETAIL ChildCutModel = new QMModels.EFORM_QM_CHILD_CUT_SLOPE_DETAIL();
                    for (int i = 1; i <= ChildCutModel.RowID; i++)
                    {
                        QMModels.EFORM_QM_CHILD_CUT_SLOPE_DETAIL detailedItem = new QMModels.EFORM_QM_CHILD_CUT_SLOPE_DETAIL();
                        detailedItem.RowID = i;
                        objViewModel.QM_CHILD_CUT_SLOPE_DETAIL_List.Add(detailedItem);
                    }
                    //objViewModel.QM_CHILD_CUT_SLOPE_DETAIL_List.ForEach(item =>
                    //{
                    foreach (var item in objViewModel.QM_CHILD_CUT_SLOPE_DETAIL_List.ToList())
                    {
                        if (item.RowID == 1)
                        {
                            errorListtemp = objCommonFunction.FetchPDFFilledDataToQMModel(item, loadedForm);
                            objViewModel.ErrorList.AddRange(errorListtemp);
                        }
                        else
                        {
                            string v = "cb_13_2_" + item.RowID;
                            PdfLoadedCheckBoxField CheckField = loadedForm.Fields[v] as PdfLoadedCheckBoxField;
                            if (CheckField.Checked)
                            {
                                errorListtemp = objCommonFunction.FetchPDFFilledDataToQMModel(item, loadedForm);
                                objViewModel.ErrorList.AddRange(errorListtemp);
                            }
                            else
                            {
                                objViewModel.QM_CHILD_CUT_SLOPE_DETAIL_List.Remove(item);
                            }
                        }

                    }

                    foreach (var item in objViewModel.QM_CHILD_CUT_SLOPE_DETAIL_List.ToList())
                    {
                        if (item.LOCATION_RD_13_2 == null && item.IS_STABLE == null)
                        {
                            objViewModel.QM_CHILD_CUT_SLOPE_DETAIL_List.Remove(item);
                        }

                    }
                    //     }
                    //  );

                    #endregion

                }
                #endregion

                #region --- Bhushan 10-13 Model reading Code--

                if (chckBoxArr[5] == 'Y')
                {
                    #region EFORM_QM_NEW_TECHNOLOGY_DETAILS
                    errorListtemp = objCommonFunction.FetchPDFFilledDataToQMModel(objViewModel.QM_NEW_TECHNOLOGY_DETAILS, loadedForm);
                    objViewModel.ErrorList.AddRange(errorListtemp);


                    #endregion

                    #region  EFORM_QM_CHILD_EARTHWORK_SUBGRADE_UCS_DETAILS

                    if (IS_NEW_TECH_USED_10_radio.SelectedValue == "Y")
                    {
                        TableProperties_UCS TablePropUCSDETAILS = new TableProperties_UCS();

                        for (int i = 1; i <= TablePropUCSDETAILS.RowCount; i++)
                        {
                            QMModels.EFORM_QM_CHILD_EARTHWORK_SUBGRADE_UCS_DETAILS rowModel = new QMModels.EFORM_QM_CHILD_EARTHWORK_SUBGRADE_UCS_DETAILS();
                            rowModel.RowId = i;
                            objViewModel.QM_CHILD_UCS_DETAILS.Add(rowModel);
                        }

                        //objViewModel.QM_CHILD_UCS_DETAILS.ForEach(item =>
                        foreach (var item in objViewModel.QM_CHILD_UCS_DETAILS.ToList())
                        {
                            if (item.RowId == 1)
                            {
                                errorListtemp = objCommonFunction.FetchPDFFilledDataToQMModel(item, loadedForm);
                                objViewModel.ErrorList.AddRange(errorListtemp);
                            }
                            else
                            {
                                string v = "cb_10_2_" + item.RowId;
                                PdfLoadedCheckBoxField CheckField = loadedForm.Fields[v] as PdfLoadedCheckBoxField;
                                if (CheckField.Checked)
                                {
                                    errorListtemp = objCommonFunction.FetchPDFFilledDataToQMModel(item, loadedForm);
                                    objViewModel.ErrorList.AddRange(errorListtemp);
                                }
                                else
                                {
                                    objViewModel.QM_CHILD_UCS_DETAILS.Remove(item);
                                }


                            }

                        }
                        //);
                    }
                    #endregion

                    #region  EFORM_QM_CHILD_EARTHWORK_SUBGRADE_CBR_DETAILS

                    if (IS_NEW_TECH_USED_10_radio.SelectedValue == "Y")
                    {
                        TableProperties_UCS TablePropcbsDETAILS = new TableProperties_UCS();



                        PdfLoadedCheckBoxField CheckFieldG = loadedForm.Fields["cb_11"] as PdfLoadedCheckBoxField;

                        if (CheckFieldG.Checked)
                        {
                            for (int i = 1; i <= TablePropcbsDETAILS.RowCount; i++)
                            {
                                QMModels.EFORM_QM_CHILD_EARTHWORK_SUBGRADE_CBR_DETAILS rowModel = new QMModels.EFORM_QM_CHILD_EARTHWORK_SUBGRADE_CBR_DETAILS();
                                rowModel.RowId = i;
                                objViewModel.QM_CHILD_CBR_DETAILS.Add(rowModel);
                            }

                            //objViewModel.QM_CHILD_CBR_DETAILS.ForEach(item =>
                            foreach (var item in objViewModel.QM_CHILD_CBR_DETAILS.ToList())
                            {
                                if (item.RowId == 1)
                                {
                                    errorListtemp = objCommonFunction.FetchPDFFilledDataToQMModel(item, loadedForm);
                                    objViewModel.ErrorList.AddRange(errorListtemp);
                                }
                                else
                                {
                                    string v = "cb_11_1_" + item.RowId;
                                    PdfLoadedCheckBoxField CheckField = loadedForm.Fields[v] as PdfLoadedCheckBoxField;
                                    if (CheckField.Checked)
                                    {
                                        errorListtemp = objCommonFunction.FetchPDFFilledDataToQMModel(item, loadedForm);
                                        objViewModel.ErrorList.AddRange(errorListtemp);
                                    }
                                    else
                                    {
                                        objViewModel.QM_CHILD_CBR_DETAILS.Remove(item);
                                    }

                                }

                            }
                        }

                        //);
                    }
                    #endregion


                    #region EFORM_QM_QOM_EMBANKMENT
                    errorListtemp = objCommonFunction.FetchPDFFilledDataToQMModel(objViewModel.QM_QOM_EMBANKMENT, loadedForm);
                    objViewModel.ErrorList.AddRange(errorListtemp);
                    if (objViewModel.QM_QOM_EMBANKMENT.DIST_SOE > 5 && (objViewModel.QM_QOM_EMBANKMENT.APPROVED_SRC_REMARKS == null || objViewModel.QM_QOM_EMBANKMENT.APPROVED_SRC_REMARKS == string.Empty))
                    {
                        errorListtemp.Clear();
                        errorListtemp.Add("Page-12: Item 5.EARTHWORK & SUB GRADE- II. d) QM should mandatorily comment on the quality of local earth, if the lead of earth used in the project is more than 5km.");

                        objViewModel.ErrorList.AddRange(errorListtemp);
                        errorListtemp.Clear();
                        objViewModel.ErrorOccured = true;
                    }

                    #endregion

                    #region EFORM_QM_CHILD_GROUP_SYMBOL_SOIL
                    TableProperties_GrpSymSoil TablePropGrpSymSoil = new TableProperties_GrpSymSoil();

                    for (int i = 1; i <= TablePropGrpSymSoil.RowCount; i++)
                    {
                        QMModels.EFORM_QM_CHILD_GROUP_SYMBOL_SOIL rowModel = new QMModels.EFORM_QM_CHILD_GROUP_SYMBOL_SOIL();
                        rowModel.RowId = i;
                        objViewModel.QM_CHILD_GROUP_SYMBOL_SOIL.Add(rowModel);
                    }

                    //objViewModel.QM_CHILD_GROUP_SYMBOL_SOIL.ForEach(item =>
                    foreach (var item in objViewModel.QM_CHILD_GROUP_SYMBOL_SOIL.ToList())
                    {
                        if (item.RowId == 1)
                        {
                            errorListtemp = objCommonFunction.FetchPDFFilledDataToQMModel(item, loadedForm);
                            objViewModel.ErrorList.AddRange(errorListtemp);
                        }
                        else
                        {
                            string v = "cb_11_2_" + item.RowId;
                            PdfLoadedCheckBoxField CheckField = loadedForm.Fields[v] as PdfLoadedCheckBoxField;
                            if (CheckField.Checked)
                            {
                                errorListtemp = objCommonFunction.FetchPDFFilledDataToQMModel(item, loadedForm);
                                objViewModel.ErrorList.AddRange(errorListtemp);
                            }
                            else
                            {
                                objViewModel.QM_CHILD_GROUP_SYMBOL_SOIL.Remove(item);
                            }
                        }

                    }
                    //);
                    #endregion


                    #region EFORM_QM_COMPAQ_EMBANKMENT
                    errorListtemp = objCommonFunction.FetchPDFFilledDataToQMModel(objViewModel.QM_COMPAQ_EMBANKMENT, loadedForm);
                    objViewModel.ErrorList.AddRange(errorListtemp);

                    #endregion

                    #region EFORM_QM_CHILD_DEGREE_OF_COMPAQ
                    TableProperties_CompDegree TablePropCompDegree = new TableProperties_CompDegree();

                    for (int i = 1; i <= TablePropCompDegree.RowCount; i++)
                    {
                        QMModels.EFORM_QM_CHILD_DEGREE_OF_COMPAQ rowModel = new QMModels.EFORM_QM_CHILD_DEGREE_OF_COMPAQ();
                        rowModel.RowId = i;
                        objViewModel.QM_CHILD_DEGREE_OF_COMPAQ.Add(rowModel);
                    }

                    foreach (var item in objViewModel.QM_CHILD_DEGREE_OF_COMPAQ.ToList())
                    {
                        if (item.RowId == 1)
                        {
                            errorListtemp = objCommonFunction.FetchPDFFilledDataToQMModel(item, loadedForm);
                            objViewModel.ErrorList.AddRange(errorListtemp);
                        }
                        else
                        {
                            string v = "cb_12_1_" + item.RowId;
                            PdfLoadedCheckBoxField CheckField = loadedForm.Fields[v] as PdfLoadedCheckBoxField;
                            if (CheckField.Checked)
                            {
                                errorListtemp = objCommonFunction.FetchPDFFilledDataToQMModel(item, loadedForm);
                                objViewModel.ErrorList.AddRange(errorListtemp);
                            }
                            else
                            {
                                objViewModel.QM_CHILD_DEGREE_OF_COMPAQ.Remove(item);
                            }
                        }

                    }
                    #endregion
                }



                #endregion

                #region   ---vikky pages 14-18  Model reading Code---

                PdfLoadedRadioButtonListField IS_NEW_TECH_USED_14_btn = loadedForm.Fields["IS_NEW_TECH_USED_14"] as PdfLoadedRadioButtonListField;
                bool IsNewTechUsed_GS = false;
                if (IS_NEW_TECH_USED_14_btn.SelectedValue == "Y")
                {
                    IsNewTechUsed_GS = true;
                }
                objViewModel.granular_subbase_mod = new EFORM_GRANULAR_SUBBASE_QM(IsNewTechUsed_GS);
                objViewModel.child_granular_UCS_List = new List<EFORM_CHILD_GRANULAR_UCS_DETAILS_QM>();
                objViewModel.child_granular_QOM_OBS_list = new List<EFORM_CHILD_GRANULAR_QOM_OBS_DETAILS_QM>();


                PdfLoadedRadioButtonListField IS_NEW_TECH_USED_15_btn = loadedForm.Fields["IS_NEW_TECH_USED_15"] as PdfLoadedRadioButtonListField;
                bool IsNewTechUsed_BL1 = false;
                if (IS_NEW_TECH_USED_15_btn.SelectedValue == "Y")
                {
                    IsNewTechUsed_BL1 = true;
                }
                objViewModel.base_course_1_details = new EFORM_BASE_COURSE_I_QM(IsNewTechUsed_BL1);
                objViewModel.child_base_coarse_l1_ucs_list = new List<EFORM_CHILD_BASECOURSE_UCS_DETAILS_LAYER1_QM>();
                objViewModel.child_base_coarse_l1_workmanship_list = new List<EFORM_CHILD_OBSERVATION_WORKMANSHIP_LAYER1_QM>();


                PdfLoadedRadioButtonListField IS_NEW_TECH_USED_16_btn = loadedForm.Fields["IS_NEW_TECH_USED_16"] as PdfLoadedRadioButtonListField;
                bool IsNewTechUsed_BL2 = false;
                if (IS_NEW_TECH_USED_16_btn.SelectedValue == "Y")
                {
                    IsNewTechUsed_BL2 = true;
                }

                objViewModel.base_course_2_details = new EFORM_BASE_COURSE_2_QM(IsNewTechUsed_BL2);
                objViewModel.child_base_coarse_l2_ucs_list = new List<EFORM_CHILD_BASECOURSE_UCS_DETAILS_LAYER2_QM>();
                objViewModel.child_base_coarse_l2_workmanship_list = new List<EFORM_CHILD_OBSERVATION_WORKMANSHIP_LAYER2_QM>();


                PdfLoadedRadioButtonListField IS_NEW_TECH_USED_18_btn = loadedForm.Fields["IS_NEW_TECH_USED_18"] as PdfLoadedRadioButtonListField;
                bool IsNewTechUsed_BL3 = false;
                if (IS_NEW_TECH_USED_18_btn.SelectedValue == "Y")
                {
                    IsNewTechUsed_BL3 = true;
                }
                objViewModel.base_course_3_details = new EFORM_BASE_COURSE_3_QM(IsNewTechUsed_BL3);
                objViewModel.child_base_coarse_l3_ucs_list = new List<EFORM_CHILD_BASECOURSE_UCS_DETAILS_LAYER3_QM>();
                objViewModel.child_base_coarse_l3_workmanship_list = new List<EFORM_CHILD_OBSERVATION_WORKMANSHIP_LAYER3_QM>();



                if (chckBoxArr[6] == 'Y')
                {
                    #region EFORM_GRANULAR_SUBBASE_QM

                    errorListtemp = objCommonFunction.FetchPDFFilledDataToQMModel(objViewModel.granular_subbase_mod, loadedForm);
                    objViewModel.ErrorList.AddRange(errorListtemp);
                    #endregion

                    #region EFORM_CHILD_GRANULAR_UCS_DETAILS_QM

                    if (IS_NEW_TECH_USED_14_btn.SelectedValue == "Y")
                    {

                        granular_ucs_details.Granular_UCS_List.ForEach(item =>
                        {

                            for (int i = 1; i <= item.RowCount; i++)
                            {
                                EFORM_CHILD_GRANULAR_UCS_DETAILS_QM detailedItem = new EFORM_CHILD_GRANULAR_UCS_DETAILS_QM();
                                detailedItem.UCS_TYPE = item.UCSType;
                                detailedItem.RowID = i;
                                objViewModel.child_granular_UCS_List.Add(detailedItem);
                            }

                        });
                        foreach (var item in objViewModel.child_granular_UCS_List.ToList())
                        {
                            if (item.RowID == 1)
                            {
                                errorListtemp = objCommonFunction.FetchPDFFilledDataToQMModel(item, loadedForm);
                                objViewModel.ErrorList.AddRange(errorListtemp);
                            }
                            else
                            {
                                string v = "cb_14_1_" + item.RowID;
                                PdfLoadedCheckBoxField CheckField = loadedForm.Fields[v] as PdfLoadedCheckBoxField;
                                if (CheckField.Checked)
                                {
                                    errorListtemp = objCommonFunction.FetchPDFFilledDataToQMModel(item, loadedForm);
                                    objViewModel.ErrorList.AddRange(errorListtemp);
                                }
                                else
                                {
                                    objViewModel.child_granular_UCS_List.Remove(item);
                                }

                            }

                        }
                    }

                    #endregion

                    #region EFORM_CHILD_QOM_OBERVATION_DETAILS_QM

                    granular_QOM_OBS_details.Granular_QOM_OBS_List.ForEach(item =>
                    {

                        for (int i = 1; i <= item.RowCount; i++)
                        {
                            EFORM_CHILD_GRANULAR_QOM_OBS_DETAILS_QM detailedItem = new EFORM_CHILD_GRANULAR_QOM_OBS_DETAILS_QM();
                            detailedItem.QOM_TYPE = item.QOMType;
                            detailedItem.RowID = i;
                            objViewModel.child_granular_QOM_OBS_list.Add(detailedItem);
                        }

                    });




                    foreach (var item in objViewModel.child_granular_QOM_OBS_list.ToList())
                    {
                        if (item.RowID == 1)
                        {
                            errorListtemp = objCommonFunction.FetchPDFFilledDataToQMModel(item, loadedForm);
                            objViewModel.ErrorList.AddRange(errorListtemp);
                        }
                        else
                        {
                            string v = "cb_14_2_" + item.RowID;
                            PdfLoadedCheckBoxField CheckField = loadedForm.Fields[v] as PdfLoadedCheckBoxField;
                            if (CheckField.Checked)
                            {
                                errorListtemp = objCommonFunction.FetchPDFFilledDataToQMModel(item, loadedForm);
                                objViewModel.ErrorList.AddRange(errorListtemp);
                            }
                            else
                            {
                                objViewModel.child_granular_QOM_OBS_list.Remove(item);
                            }

                        }

                    }



                }
                #endregion


                if (chckBoxArr[7] == 'Y')
                {

                    #region EFORM_BASE_COARSE_LAYER1_QM

                    errorListtemp = objCommonFunction.FetchPDFFilledDataToQMModel(objViewModel.base_course_1_details, loadedForm);


                    objViewModel.ErrorList.AddRange(errorListtemp);
                    #endregion

                    #region EFORM_QM_CHILD_BASECOURSE_UCS_DETAILS_LAYER1
                    if (IS_NEW_TECH_USED_15_btn.SelectedValue == "Y")
                    {

                        base_coarse_layer1_list.Base_coarse_layer1.ForEach(item =>
                        {

                            for (int i = 1; i <= item.RowCount; i++)
                            {
                                EFORM_CHILD_BASECOURSE_UCS_DETAILS_LAYER1_QM detailedItem = new EFORM_CHILD_BASECOURSE_UCS_DETAILS_LAYER1_QM();
                                detailedItem.UCS_TYPE = item.UCSType;
                                detailedItem.RowID = i;
                                objViewModel.child_base_coarse_l1_ucs_list.Add(detailedItem);
                            }

                        });


                        foreach (var item in objViewModel.child_base_coarse_l1_ucs_list.ToList())
                        {
                            if (item.RowID == 1)
                            {
                                errorListtemp = objCommonFunction.FetchPDFFilledDataToQMModel(item, loadedForm);
                                objViewModel.ErrorList.AddRange(errorListtemp);
                            }
                            else
                            {
                                string v = "cb_15_1_" + item.RowID;
                                PdfLoadedCheckBoxField CheckField = loadedForm.Fields[v] as PdfLoadedCheckBoxField;
                                if (CheckField.Checked)
                                {
                                    errorListtemp = objCommonFunction.FetchPDFFilledDataToQMModel(item, loadedForm);
                                    objViewModel.ErrorList.AddRange(errorListtemp);
                                }
                                else
                                {
                                    objViewModel.child_base_coarse_l1_ucs_list.Remove(item);
                                }

                            }

                        }






                    }

                    #endregion

                    #region EFORM_QM_CHILD_OBSERVATION_WORKMANSHIP_LAYER1

                    base_coarse_layer1_workmanship_list.Base_coarse_workmanship_layer1.ForEach(item =>
                    {

                        for (int i = 1; i <= item.RowCount; i++)
                        {
                            EFORM_CHILD_OBSERVATION_WORKMANSHIP_LAYER1_QM detailedItem = new EFORM_CHILD_OBSERVATION_WORKMANSHIP_LAYER1_QM();
                            detailedItem.Work_TYPE = item.WorkType;
                            detailedItem.RowID = i;
                            objViewModel.child_base_coarse_l1_workmanship_list.Add(detailedItem);
                        }

                    });




                    foreach (var item in objViewModel.child_base_coarse_l1_workmanship_list.ToList())
                    {
                        if (item.RowID == 1)
                        {
                            errorListtemp = objCommonFunction.FetchPDFFilledDataToQMModel(item, loadedForm);
                            objViewModel.ErrorList.AddRange(errorListtemp);
                        }
                        else
                        {
                            string v = "cb_16_1_" + item.RowID;
                            PdfLoadedCheckBoxField CheckField = loadedForm.Fields[v] as PdfLoadedCheckBoxField;
                            if (CheckField.Checked)
                            {
                                errorListtemp = objCommonFunction.FetchPDFFilledDataToQMModel(item, loadedForm);
                                objViewModel.ErrorList.AddRange(errorListtemp);
                            }
                            else
                            {
                                objViewModel.child_base_coarse_l1_workmanship_list.Remove(item);
                            }

                        }

                    }




                    #endregion
                }
                if (chckBoxArr[8] == 'Y')
                {

                    #region EFORM_BASE_COARSE_LAYER2_QM

                    errorListtemp = objCommonFunction.FetchPDFFilledDataToQMModel(objViewModel.base_course_2_details, loadedForm);


                    objViewModel.ErrorList.AddRange(errorListtemp);
                    #endregion


                    #region EFORM_QM_CHILD_BASECOURSE_UCS_DETAILS_LAYER2


                    if (IS_NEW_TECH_USED_16_btn.SelectedValue == "Y")
                    {
                        base_coarse_layer2_list.Base_coarse_layer2.ForEach(item =>
                        {

                            for (int i = 1; i <= item.RowCount; i++)
                            {
                                EFORM_CHILD_BASECOURSE_UCS_DETAILS_LAYER2_QM detailedItem = new EFORM_CHILD_BASECOURSE_UCS_DETAILS_LAYER2_QM();
                                detailedItem.UCS_TYPE = item.UCSType;
                                detailedItem.RowID = i;
                                objViewModel.child_base_coarse_l2_ucs_list.Add(detailedItem);
                            }

                        });


                        foreach (var item in objViewModel.child_base_coarse_l2_ucs_list.ToList())
                        {
                            if (item.RowID == 1)
                            {
                                errorListtemp = objCommonFunction.FetchPDFFilledDataToQMModel(item, loadedForm);
                                objViewModel.ErrorList.AddRange(errorListtemp);
                            }
                            else
                            {
                                string v = "cb_17_1_" + item.RowID;
                                PdfLoadedCheckBoxField CheckField = loadedForm.Fields[v] as PdfLoadedCheckBoxField;
                                if (CheckField.Checked)
                                {
                                    errorListtemp = objCommonFunction.FetchPDFFilledDataToQMModel(item, loadedForm);
                                    objViewModel.ErrorList.AddRange(errorListtemp);
                                }
                                else
                                {
                                    objViewModel.child_base_coarse_l2_ucs_list.Remove(item);
                                }

                            }

                        }




                    }

                    #endregion

                    #region EFORM_QM_CHILD_OBSERVATION_WORKMANSHIP_LAYER2

                    base_coarse_layer2_workmanship_list.Base_coarse_workmanship_layer2.ForEach(item =>
                    {

                        for (int i = 1; i <= item.RowCount; i++)
                        {
                            EFORM_CHILD_OBSERVATION_WORKMANSHIP_LAYER2_QM detailedItem = new EFORM_CHILD_OBSERVATION_WORKMANSHIP_LAYER2_QM();
                            detailedItem.Work_TYPE = item.WorkType;
                            detailedItem.RowID = i;
                            objViewModel.child_base_coarse_l2_workmanship_list.Add(detailedItem);
                        }

                    });


                    foreach (var item in objViewModel.child_base_coarse_l2_workmanship_list.ToList())
                    {
                        if (item.RowID == 1)
                        {
                            errorListtemp = objCommonFunction.FetchPDFFilledDataToQMModel(item, loadedForm);
                            objViewModel.ErrorList.AddRange(errorListtemp);
                        }
                        else
                        {
                            string v = "cb_17_2_" + item.RowID;
                            PdfLoadedCheckBoxField CheckField = loadedForm.Fields[v] as PdfLoadedCheckBoxField;
                            if (CheckField.Checked)
                            {
                                errorListtemp = objCommonFunction.FetchPDFFilledDataToQMModel(item, loadedForm);
                                objViewModel.ErrorList.AddRange(errorListtemp);
                            }
                            else
                            {
                                objViewModel.child_base_coarse_l2_workmanship_list.Remove(item);
                            }

                        }

                    }

                    #endregion
                }

                if (chckBoxArr[9] == 'Y')
                {

                    #region EFORM_BASE_COARSE_LAYER3_QM

                    errorListtemp = objCommonFunction.FetchPDFFilledDataToQMModel(objViewModel.base_course_3_details, loadedForm);


                    objViewModel.ErrorList.AddRange(errorListtemp);
                    #endregion

                    #region EFORM_QM_CHILD_BASECOURSE_UCS_DETAILS_LAYER3


                    if (IS_NEW_TECH_USED_18_btn.SelectedValue == "Y")
                    {
                        base_coarse_layer3_list.Base_coarse_layer3.ForEach(item =>
                        {

                            for (int i = 1; i <= item.RowCount; i++)
                            {
                                EFORM_CHILD_BASECOURSE_UCS_DETAILS_LAYER3_QM detailedItem = new EFORM_CHILD_BASECOURSE_UCS_DETAILS_LAYER3_QM();
                                detailedItem.UCS_TYPE = item.UCSType;
                                detailedItem.RowID = i;
                                objViewModel.child_base_coarse_l3_ucs_list.Add(detailedItem);
                            }

                        });




                        foreach (var item in objViewModel.child_base_coarse_l3_ucs_list.ToList())
                        {
                            if (item.RowID == 1)
                            {
                                errorListtemp = objCommonFunction.FetchPDFFilledDataToQMModel(item, loadedForm);
                                objViewModel.ErrorList.AddRange(errorListtemp);
                            }
                            else
                            {
                                string v = "cb_18_1_" + item.RowID;
                                PdfLoadedCheckBoxField CheckField = loadedForm.Fields[v] as PdfLoadedCheckBoxField;
                                if (CheckField.Checked)
                                {
                                    errorListtemp = objCommonFunction.FetchPDFFilledDataToQMModel(item, loadedForm);
                                    objViewModel.ErrorList.AddRange(errorListtemp);
                                }
                                else
                                {
                                    objViewModel.child_base_coarse_l3_ucs_list.Remove(item);
                                }

                            }

                        }
                    }

                    #endregion

                    #region EFORM_QM_CHILD_OBSERVATION_WORKMANSHIP_LAYER3

                    base_coarse_layer3_workmanship_list.Base_coarse_workmanship_layer3.ForEach(item =>
                    {

                        for (int i = 1; i <= item.RowCount; i++)
                        {
                            EFORM_CHILD_OBSERVATION_WORKMANSHIP_LAYER3_QM detailedItem = new EFORM_CHILD_OBSERVATION_WORKMANSHIP_LAYER3_QM();
                            detailedItem.Work_TYPE = item.WorkType;
                            detailedItem.RowID = i;
                            objViewModel.child_base_coarse_l3_workmanship_list.Add(detailedItem);
                        }

                    });



                    foreach (var item in objViewModel.child_base_coarse_l3_workmanship_list.ToList())
                    {
                        if (item.RowID == 1)
                        {
                            errorListtemp = objCommonFunction.FetchPDFFilledDataToQMModel(item, loadedForm);
                            objViewModel.ErrorList.AddRange(errorListtemp);
                        }
                        else
                        {
                            string v = "cb_18_2_" + item.RowID;
                            PdfLoadedCheckBoxField CheckField = loadedForm.Fields[v] as PdfLoadedCheckBoxField;
                            if (CheckField.Checked)
                            {
                                errorListtemp = objCommonFunction.FetchPDFFilledDataToQMModel(item, loadedForm);
                                objViewModel.ErrorList.AddRange(errorListtemp);
                            }
                            else
                            {
                                objViewModel.child_base_coarse_l3_workmanship_list.Remove(item);
                            }

                        }

                    }

                    #endregion
                }

                #endregion

                #region  ----Srishti page 19-23--Model reading Code----

                if (chckBoxArr[10] == 'Y')
                {

                    errorListtemp = objCommonFunction.FetchPDFFilledDataToQMModel(objViewModel.QM_BITUMINOUS_BASE_COURSE, loadedForm);
                    objViewModel.ErrorList.AddRange(errorListtemp);

                    TableProperties_BITUMINOUS_OBSERVATION_DETAILS TablePropertiesBitObsDetails = new TableProperties_BITUMINOUS_OBSERVATION_DETAILS();

                    for (int i = 1; i <= TablePropertiesBitObsDetails.RowCount; i++)
                    {
                        QMModels.EFORM_QM_CHILD_BITUMINOUS_OBSERVATION_DETAILS rowModel = new QMModels.EFORM_QM_CHILD_BITUMINOUS_OBSERVATION_DETAILS();
                        rowModel.RowId = i;
                        objViewModel.QM_CHILD_BITUMINOUS_OBSERVATION_DETAILS.Add(rowModel);
                    }

                    //objViewModel.QM_CHILD_BITUMINOUS_OBSERVATION_DETAILS.ForEach(item =>
                    //{
                    foreach (var item in objViewModel.QM_CHILD_BITUMINOUS_OBSERVATION_DETAILS.ToList())
                    {
                        if (item.RowId == 1)
                        {
                            errorListtemp = objCommonFunction.FetchPDFFilledDataToQMModel(item, loadedForm);
                            objViewModel.ErrorList.AddRange(errorListtemp);
                        }
                        else
                        {
                            string v = "cb_20_1_" + item.RowId;
                            PdfLoadedCheckBoxField CheckField = loadedForm.Fields[v] as PdfLoadedCheckBoxField;
                            if (CheckField.Checked)
                            {
                                errorListtemp = objCommonFunction.FetchPDFFilledDataToQMModel(item, loadedForm);
                                objViewModel.ErrorList.AddRange(errorListtemp);
                            }
                            else
                            {
                                objViewModel.QM_CHILD_BITUMINOUS_OBSERVATION_DETAILS.Remove(item);
                            }
                        }
                    }
                    //     });
                }

                if (chckBoxArr[11] == 'Y')
                {
                    errorListtemp = objCommonFunction.FetchPDFFilledDataToQMModel(objViewModel.QM_BITUMINOUS_SURFACE_COURSE, loadedForm);
                    if (tempVersion == 2.0)
                    {
                        PdfLoadedRadioButtonListField isMixDesignNA = loadedForm.Fields["IS_MIX_DESIGN_21_1"] as PdfLoadedRadioButtonListField;
                        if (isMixDesignNA.SelectedValue == "O")
                        {

                            if (objViewModel.QM_BITUMINOUS_SURFACE_COURSE.IS_MIX_DESIGN_21 == null)
                            {
                                objViewModel.QM_BITUMINOUS_SURFACE_COURSE.IS_MIX_DESIGN_21 = "O";
                            }
                        }
                    }

                    objViewModel.ErrorList.AddRange(errorListtemp);
                    TableProperties_BITUMINOUS_SURFACE_COARSE_DETAILS TablePropertiesBitSurCoarDetails = new TableProperties_BITUMINOUS_SURFACE_COARSE_DETAILS();

                    for (int i = 1; i <= TablePropertiesBitSurCoarDetails.RowCount; i++)
                    {
                        QMModels.EFORM_QM_CHILD_BITUMINOUS_SURFACE_COARSE_DETAILS rowModel = new QMModels.EFORM_QM_CHILD_BITUMINOUS_SURFACE_COARSE_DETAILS();
                        rowModel.RowId = i;
                        objViewModel.QM_CHILD_BITUMINOUS_SURFACE_COARSE_DETAILS.Add(rowModel);
                    }

                    //objViewModel.QM_CHILD_BITUMINOUS_SURFACE_COARSE_DETAILS.ForEach(item =>
                    //{
                    foreach (var item in objViewModel.QM_CHILD_BITUMINOUS_SURFACE_COARSE_DETAILS.ToList())
                    {
                        if (item.RowId == 1)
                        {
                            errorListtemp = objCommonFunction.FetchPDFFilledDataToQMModel(item, loadedForm);
                            objViewModel.ErrorList.AddRange(errorListtemp);
                        }
                        else
                        {
                            string v = "cb_21_1_" + item.RowId;
                            PdfLoadedCheckBoxField CheckField = loadedForm.Fields[v] as PdfLoadedCheckBoxField;
                            if (CheckField.Checked)
                            {
                                errorListtemp = objCommonFunction.FetchPDFFilledDataToQMModel(item, loadedForm);
                                objViewModel.ErrorList.AddRange(errorListtemp);
                            }
                            else
                            {
                                objViewModel.QM_CHILD_BITUMINOUS_SURFACE_COARSE_DETAILS.Remove(item);
                            }

                        }
                    }
                    //     });
                }


                if (chckBoxArr[12] == 'Y')
                {
                    errorListtemp = objCommonFunction.FetchPDFFilledDataToQMModel(objViewModel.QM_SHOULDERS, loadedForm);
                    objViewModel.ErrorList.AddRange(errorListtemp);


                    if (IS_NEW_TECH_USED_22_radio.SelectedValue == "Y")
                    {
                        IS_NEW_TECH_USED_22_val = true;

                        // If Radio button value is yes then only read the value of the table EFORM_QM_CHILD_SHOULDERS_UCS_DETAILS
                        TableProperties_SHOULDERS_UCS_DETAILS TablePropShoulderUcsCoarDetails = new TableProperties_SHOULDERS_UCS_DETAILS();

                        for (int i = 1; i <= TablePropShoulderUcsCoarDetails.RowCount; i++)
                        {
                            QMModels.EFORM_QM_CHILD_SHOULDERS_UCS_DETAILS rowModel = new QMModels.EFORM_QM_CHILD_SHOULDERS_UCS_DETAILS();
                            rowModel.RowId = i;
                            objViewModel.QM_CHILD_SHOULDERS_UCS_DETAILS.Add(rowModel);
                        }

                        //objViewModel.QM_CHILD_SHOULDERS_UCS_DETAILS.ForEach(item =>
                        //{
                        foreach (var item in objViewModel.QM_CHILD_SHOULDERS_UCS_DETAILS.ToList())
                        {
                            if (item.RowId == 1)
                            {
                                errorListtemp = objCommonFunction.FetchPDFFilledDataToQMModel(item, loadedForm);
                                objViewModel.ErrorList.AddRange(errorListtemp);
                            }
                            else
                            {
                                string v = "cb_22_1_" + item.RowId;
                                PdfLoadedCheckBoxField CheckField = loadedForm.Fields[v] as PdfLoadedCheckBoxField;
                                if (CheckField.Checked)
                                {
                                    errorListtemp = objCommonFunction.FetchPDFFilledDataToQMModel(item, loadedForm);
                                    objViewModel.ErrorList.AddRange(errorListtemp);
                                }
                                else
                                {
                                    objViewModel.QM_CHILD_SHOULDERS_UCS_DETAILS.Remove(item);
                                }
                            }
                        }
                        //     });
                    }


                    if (TemplateStatus)
                    {
                        TableProperties_SHOULDERS_MATERIAL_DETAILS TablePropShoulderMaterialCoarDetails = new TableProperties_SHOULDERS_MATERIAL_DETAILS();

                        for (int i = 1; i <= TablePropShoulderMaterialCoarDetails.RowCount; i++)
                        {
                            QMModels.EFORM_QM_CHILD_SHOULDERS_MATERIAL_DETAILS rowModel = new QMModels.EFORM_QM_CHILD_SHOULDERS_MATERIAL_DETAILS();
                            rowModel.RowId = i;
                            objViewModel.QM_CHILD_SHOULDERS_MATERIAL_DETAILS.Add(rowModel);
                        }

                        //objViewModel.QM_CHILD_SHOULDERS_MATERIAL_DETAILS.ForEach(item =>
                        //{
                        foreach (var item in objViewModel.QM_CHILD_SHOULDERS_MATERIAL_DETAILS.ToList())
                        {
                            if (item.RowId == 1)
                            {
                                errorListtemp = objCommonFunction.FetchPDFFilledDataToQMModel(item, loadedForm);
                                objViewModel.ErrorList.AddRange(errorListtemp);
                            }
                            else
                            {
                                string v = "cb_22_2_" + item.RowId;
                                PdfLoadedCheckBoxField CheckField = loadedForm.Fields[v] as PdfLoadedCheckBoxField;
                                if (CheckField.Checked)
                                {
                                    errorListtemp = objCommonFunction.FetchPDFFilledDataToQMModel(item, loadedForm);
                                    objViewModel.ErrorList.AddRange(errorListtemp);
                                }
                                else
                                {
                                    objViewModel.QM_CHILD_SHOULDERS_MATERIAL_DETAILS.Remove(item);
                                }
                            }
                        }
                    }
                    //   });



                }





                #endregion

                #region   ---vikky pages 23-27 Model Reading code---

                if (chckBoxArr[13] == 'Y')
                {
                    PdfLoadedTextBoxField TOTAL_PIPE_CULVERTS = loadedForm.Fields["TOTAL_PIPE_CULVERTS"] as PdfLoadedTextBoxField;
                    decimal TOTAL_PIPE_CULVERTS_value = 0;
                    try
                    {
                        TOTAL_PIPE_CULVERTS_value = Convert.ToDecimal(TOTAL_PIPE_CULVERTS.Text);
                        if (TOTAL_PIPE_CULVERTS_value == 0)
                        {
                            errorListtemp.Clear();
                            errorListtemp.Add("Page-23: Item 13. CROSS DRAINAGE WORKS: (Pipe Culverts) is applicable only when I. Total number of pipe culverts as per sanctioned DPR is greater than 0");
                            objViewModel.ErrorOccured = true;
                            objViewModel.ErrorList.AddRange(errorListtemp);
                            errorListtemp.Clear();
                        }
                    }
                    catch (Exception e)
                    {
                        errorListtemp.Clear();
                        errorListtemp.Add("Page-23: Item 13. CROSS DRAINAGE WORKS: (Pipe Culverts) I. Please enter valid number in Total number of pipe culverts as per sanctioned DPR");
                        objViewModel.ErrorOccured = true;
                        objViewModel.ErrorList.AddRange(errorListtemp);
                        errorListtemp.Clear();
                    }



                    if (TOTAL_PIPE_CULVERTS_value > 0)
                    {

                        #region EFORM_CDWORKS_PIPE_CULVERTS_QM

                        errorListtemp = objCommonFunction.FetchPDFFilledDataToQMModel(objViewModel.CDWORKS_PIPE_CULVERTS_details, loadedForm);
                        objViewModel.ErrorList.AddRange(errorListtemp);
                        #endregion


                        #region EFORM_CHILD_CDWORKS_PIPE_CULVERTS_DETAILS_QM



                        CDWORKS_PIPE_CULVERTS_List.CHILD_CDWORKS_PIPE_CULVERTS.ForEach(item =>
                        {

                            for (int i = 1; i <= item.RowCount; i++)
                            {
                                EFORM_CHILD_CDWORKS_PIPE_CULVERTS_DETAILS_QM detailedItem = new EFORM_CHILD_CDWORKS_PIPE_CULVERTS_DETAILS_QM();
                                detailedItem.WORK_TYPE = item.WorkType;
                                detailedItem.RowID = i;
                                objViewModel.CHILD_CDWORKS_PIPE_CULVERTS_list.Add(detailedItem);
                            }

                        });





                        foreach (var item in objViewModel.CHILD_CDWORKS_PIPE_CULVERTS_list.ToList())
                        {
                            if (item.RowID == 1)
                            {
                                errorListtemp = objCommonFunction.FetchPDFFilledDataToQMModel(item, loadedForm);
                                objViewModel.ErrorList.AddRange(errorListtemp);
                            }
                            else
                            {
                                string v = "cb_23_1_" + item.RowID;
                                PdfLoadedCheckBoxField CheckField = loadedForm.Fields[v] as PdfLoadedCheckBoxField;
                                if (CheckField.Checked)
                                {
                                    errorListtemp = objCommonFunction.FetchPDFFilledDataToQMModel(item, loadedForm);
                                    objViewModel.ErrorList.AddRange(errorListtemp);
                                }
                                else
                                {
                                    objViewModel.CHILD_CDWORKS_PIPE_CULVERTS_list.Remove(item);
                                }

                            }

                        }

                        #endregion



                    }
                }

                if (chckBoxArr[14] == 'Y')
                {

                    PdfLoadedTextBoxField TOTAL_SLAB_CULVERTS = loadedForm.Fields["TOTAL_SLAB_CULVERTS"] as PdfLoadedTextBoxField;

                    decimal TOTAL_SLAB_CULVERTS_value = 0;
                    try
                    {
                        TOTAL_SLAB_CULVERTS_value = Convert.ToDecimal(TOTAL_SLAB_CULVERTS.Text);
                        if (TOTAL_SLAB_CULVERTS_value == 0)
                        {
                            errorListtemp.Clear();
                            errorListtemp.Add("Page-24: Item 14. CROSS DRAINAGE WORK: (Slab Culverts) is applicable only when II. Total number of slab culverts as per sanctioned DPR is greater than 0");
                            objViewModel.ErrorOccured = true;
                            objViewModel.ErrorList.AddRange(errorListtemp);
                            errorListtemp.Clear();
                        }
                    }
                    catch (Exception e)
                    {
                        errorListtemp.Clear();
                        errorListtemp.Add("Page-24: Item 14. CROSS DRAINAGE WORK: (Slab Culverts) is applicable only when II. Please enter valid number in Total number of slab culverts as per sanctioned DPR");
                        objViewModel.ErrorOccured = true;
                        objViewModel.ErrorList.AddRange(errorListtemp);
                        errorListtemp.Clear();
                    }




                    if (TOTAL_SLAB_CULVERTS_value > 0)
                    {

                        #region EFORM_CDWORKS_SLAB_CULVERTS_QM

                        errorListtemp = objCommonFunction.FetchPDFFilledDataToQMModel(objViewModel.CDWORKS_SLAB_CULVERTS_details, loadedForm);
                        objViewModel.ErrorList.AddRange(errorListtemp);
                        #endregion


                        #region EFORM_CHILD_CDWORKS_SLAB_CULVERTS_DETAILS_QM



                        CDWORKS_SLAB_CULVERTS_List.CHILD_CDWORKS_SLAB_CULVERTS.ForEach(item =>
                        {

                            for (int i = 1; i <= item.RowCount; i++)
                            {
                                EFORM_CHILD_CDWORKS_SLAB_CULVERTS_DETAILS_QM detailedItem = new EFORM_CHILD_CDWORKS_SLAB_CULVERTS_DETAILS_QM();
                                detailedItem.WORK_TYPE = item.WorkType;
                                detailedItem.RowID = i;
                                objViewModel.CHILD_CDWORKS_SLAB_CULVERTS_list.Add(detailedItem);
                            }

                        });


                        foreach (var item in objViewModel.CHILD_CDWORKS_SLAB_CULVERTS_list.ToList())
                        {
                            if (item.RowID == 1)
                            {
                                errorListtemp = objCommonFunction.FetchPDFFilledDataToQMModel(item, loadedForm);
                                objViewModel.ErrorList.AddRange(errorListtemp);
                            }
                            else
                            {
                                string v = "cb_24_1_" + item.RowID;
                                PdfLoadedCheckBoxField CheckField = loadedForm.Fields[v] as PdfLoadedCheckBoxField;
                                if (CheckField.Checked)
                                {
                                    errorListtemp = objCommonFunction.FetchPDFFilledDataToQMModel(item, loadedForm);
                                    objViewModel.ErrorList.AddRange(errorListtemp);
                                }
                                else
                                {
                                    objViewModel.CHILD_CDWORKS_SLAB_CULVERTS_list.Remove(item);
                                }

                            }

                        }





                        #endregion
                    }
                }

                if (chckBoxArr[15] == 'Y')
                {
                    #region EFORM_PROTECTION_WORK_QM

                    errorListtemp = objCommonFunction.FetchPDFFilledDataToQMModel(objViewModel.PROTECTION_WORK_details, loadedForm);
                    objViewModel.ErrorList.AddRange(errorListtemp);
                    #endregion

                    #region EFORM_CHILD_PROT_WORKS_QOM_DETAILS_QM



                    PROT_WORKS_QOM_List.CHILD_PROT_WORKS_QOM.ForEach(item =>
                    {

                        for (int i = 1; i <= item.RowCount; i++)
                        {
                            EFORM_CHILD_PROT_WORKS_QOM_DETAILS_QM detailedItem = new EFORM_CHILD_PROT_WORKS_QOM_DETAILS_QM();
                            detailedItem.WORK_TYPE = item.WorkType;
                            detailedItem.RowID = i;
                            objViewModel.CHILD_PROT_WORKS_QOM_list.Add(detailedItem);
                        }

                    });


                    foreach (var item in objViewModel.CHILD_PROT_WORKS_QOM_list.ToList())
                    {
                        if (item.RowID == 1)
                        {
                            errorListtemp = objCommonFunction.FetchPDFFilledDataToQMModel(item, loadedForm);
                            objViewModel.ErrorList.AddRange(errorListtemp);
                        }
                        else
                        {
                            string v = "cb_25_1_" + item.RowID;
                            PdfLoadedCheckBoxField CheckField = loadedForm.Fields[v] as PdfLoadedCheckBoxField;
                            if (CheckField.Checked)
                            {
                                errorListtemp = objCommonFunction.FetchPDFFilledDataToQMModel(item, loadedForm);
                                objViewModel.ErrorList.AddRange(errorListtemp);
                            }
                            else
                            {
                                objViewModel.CHILD_PROT_WORKS_QOM_list.Remove(item);
                            }

                        }

                    }


                    #endregion

                    #region EFORM_CHILD_PROT_WORKS_WORKMANSHIP_OF_RS_DETAILS_QM



                    PROT_WORKS_WORKMANSHIP_OF_RS_List.CHILD_PROT_WORKS_WORKMANSHIP_OF_RS.ForEach(item =>
                    {

                        for (int i = 1; i <= item.RowCount; i++)
                        {
                            EFORM_CHILD_PROT_WORKS_WORKMANSHIP_OF_RS_DETAILS_QM detailedItem = new EFORM_CHILD_PROT_WORKS_WORKMANSHIP_OF_RS_DETAILS_QM();
                            detailedItem.WORK_TYPE = item.WorkType;
                            detailedItem.RowID = i;
                            objViewModel.CHILD_PROT_WORKS_WORKMANSHIP_OF_RS_list.Add(detailedItem);
                        }

                    });



                    foreach (var item in objViewModel.CHILD_PROT_WORKS_WORKMANSHIP_OF_RS_list.ToList())
                    {
                        if (item.RowID == 1)
                        {
                            errorListtemp = objCommonFunction.FetchPDFFilledDataToQMModel(item, loadedForm);
                            objViewModel.ErrorList.AddRange(errorListtemp);
                        }
                        else
                        {
                            string v = "cb_25_2_" + item.RowID;
                            PdfLoadedCheckBoxField CheckField = loadedForm.Fields[v] as PdfLoadedCheckBoxField;
                            if (CheckField.Checked)
                            {
                                errorListtemp = objCommonFunction.FetchPDFFilledDataToQMModel(item, loadedForm);
                                objViewModel.ErrorList.AddRange(errorListtemp);
                            }
                            else
                            {
                                objViewModel.CHILD_PROT_WORKS_WORKMANSHIP_OF_RS_list.Remove(item);
                            }

                        }

                    }

                    #endregion

                }


                if (chckBoxArr[16] == 'Y')
                {
                    #region EFORM_CRASH_BARRIERS_ROAD_SAFETY_QM

                    errorListtemp = objCommonFunction.FetchPDFFilledDataToQMModel(objViewModel.CRASH_BARRIERS_ROAD_SAFETY_details, loadedForm);
                    objViewModel.ErrorList.AddRange(errorListtemp);
                    #endregion

                    #region EFORM_CHILD_CRASH_BARRIERS_OBSERVATION_DETAILS_QM



                    CRASH_BARRIERS_OBSERVATION_List.CHILD_CRASH_BARRIERS_OBSERVATION.ForEach(item =>
                    {

                        for (int i = 1; i <= item.RowCount; i++)
                        {
                            EFORM_CHILD_CRASH_BARRIERS_OBSERVATION_DETAILS_QM detailedItem = new EFORM_CHILD_CRASH_BARRIERS_OBSERVATION_DETAILS_QM();
                            detailedItem.WORK_TYPE = item.WorkType;
                            detailedItem.RowID = i;
                            objViewModel.CHILD_CRASH_BARRIERS_OBSERVATION_list.Add(detailedItem);
                        }

                    });




                    foreach (var item in objViewModel.CHILD_CRASH_BARRIERS_OBSERVATION_list.ToList())
                    {
                        if (item.RowID == 1)
                        {
                            errorListtemp = objCommonFunction.FetchPDFFilledDataToQMModel(item, loadedForm);
                            objViewModel.ErrorList.AddRange(errorListtemp);
                        }
                        else
                        {
                            string v = "cb_26_1_" + item.RowID;
                            PdfLoadedCheckBoxField CheckField = loadedForm.Fields[v] as PdfLoadedCheckBoxField;
                            if (CheckField.Checked)
                            {
                                errorListtemp = objCommonFunction.FetchPDFFilledDataToQMModel(item, loadedForm);
                                objViewModel.ErrorList.AddRange(errorListtemp);
                            }
                            else
                            {
                                objViewModel.CHILD_CRASH_BARRIERS_OBSERVATION_list.Remove(item);
                            }

                        }

                    }


                    #endregion
                }

                if (chckBoxArr[17] == 'Y')
                {
                    #region EFORM_SIDE_AND_CATCH_DRAINS_EARTHEN_QM

                    errorListtemp = objCommonFunction.FetchPDFFilledDataToQMModel(objViewModel.SIDE_AND_CATCH_DRAINS_EARTHEN_details, loadedForm);
                    objViewModel.ErrorList.AddRange(errorListtemp);
                    #endregion

                    #region EFORM_CHILD_SD_AND_CW_DRAINS_DETAILS_QM



                    SD_AND_CW_DRAINS_List.CHILD_SD_AND_CW_DRAINS.ForEach(item =>
                    {

                        for (int i = 1; i <= item.RowCount; i++)
                        {
                            EFORM_CHILD_SD_AND_CW_DRAINS_DETAILS_QM detailedItem = new EFORM_CHILD_SD_AND_CW_DRAINS_DETAILS_QM();
                            detailedItem.WORK_TYPE = item.WorkType;
                            detailedItem.RowID = i;
                            objViewModel.CHILD_SD_AND_CW_DRAINS_list.Add(detailedItem);
                        }

                    });



                    foreach (var item in objViewModel.CHILD_SD_AND_CW_DRAINS_list.ToList())
                    {
                        if (item.RowID == 1)
                        {
                            errorListtemp = objCommonFunction.FetchPDFFilledDataToQMModel(item, loadedForm);
                            objViewModel.ErrorList.AddRange(errorListtemp);
                        }
                        else
                        {
                            string v = "cb_27_1_" + item.RowID;
                            PdfLoadedCheckBoxField CheckField = loadedForm.Fields[v] as PdfLoadedCheckBoxField;
                            if (CheckField.Checked)
                            {
                                errorListtemp = objCommonFunction.FetchPDFFilledDataToQMModel(item, loadedForm);
                                objViewModel.ErrorList.AddRange(errorListtemp);
                            }
                            else
                            {
                                objViewModel.CHILD_SD_AND_CW_DRAINS_list.Remove(item);
                            }

                        }

                    }

                    #endregion

                }
                #endregion

                #region  ----bhushan 27-30 Model reading code---
                if (chckBoxArr[18] == 'Y')
                {

                    PdfLoadedTextBoxField CC_SR_PROPOSED_LENGTH_27 = loadedForm.Fields["CC_SR_PROPOSED_LENGTH_27"] as PdfLoadedTextBoxField;
                    //    decimal CC_SR_PROPOSED_LENGTH_27_val = Convert.ToDecimal(CC_SR_PROPOSED_LENGTH_27.Text);


                    decimal CC_SR_PROPOSED_LENGTH_27_val = 0;
                    try
                    {
                        CC_SR_PROPOSED_LENGTH_27_val = Convert.ToDecimal(CC_SR_PROPOSED_LENGTH_27.Text);
                        if (CC_SR_PROPOSED_LENGTH_27_val == 0)
                        {
                            errorListtemp.Clear();
                            errorListtemp.Add("Page-27: Item 18. CEMENT CONCRETE/SEMI-RIGID (SR) PAVEMENTS is applicable only when IV. Proposed length is greater than 0");
                            objViewModel.ErrorOccured = true;
                            objViewModel.ErrorList.AddRange(errorListtemp);
                            errorListtemp.Clear();

                        }

                    }
                    catch (Exception e)
                    {
                        errorListtemp.Clear();
                        errorListtemp.Add("Page-27: Item 18. CEMENT CONCRETE/SEMI-RIGID (SR) PAVEMENTS is applicable only when IV. Please enter valid number in PROPOSED LENGTH");
                        objViewModel.ErrorOccured = true;
                        objViewModel.ErrorList.AddRange(errorListtemp);
                        errorListtemp.Clear();

                    }


                    if (CC_SR_PROPOSED_LENGTH_27_val > 0 && CC_SR_PROPOSED_LENGTH_27.Text != "")
                    {

                        #region EFORM_QM_CC_SR_PVAEMENTS
                        errorListtemp = objCommonFunction.FetchPDFFilledDataToQMModel(objViewModel.QM_CC_SR_PVAEMENTS, loadedForm);
                        objViewModel.ErrorList.AddRange(errorListtemp);
                        #endregion

                        #region EFORM_QM_CHILD_CC_AND_SR_PAVEMENTS_OBSERVATION_DETAILS
                        TableProperties_CCSrPavementObsDetails TablePropCCSrPavementObsDetails = new TableProperties_CCSrPavementObsDetails();
                        for (int i = 1; i <= TablePropCCSrPavementObsDetails.RowCount; i++)
                        {
                            QMModels.EFORM_QM_CHILD_CC_AND_SR_PAVEMENTS_OBSERVATION_DETAILS rowModel = new QMModels.EFORM_QM_CHILD_CC_AND_SR_PAVEMENTS_OBSERVATION_DETAILS();
                            rowModel.RowId = i;
                            objViewModel.QM_CHILD_CC_AND_SR_PAVEMENTS_OBSERVATION_DETAILS.Add(rowModel);
                        }

                        foreach (var item in objViewModel.QM_CHILD_CC_AND_SR_PAVEMENTS_OBSERVATION_DETAILS.ToList())
                        {
                            if (item.RowId == 1)
                            {
                                errorListtemp = objCommonFunction.FetchPDFFilledDataToQMModel(item, loadedForm);
                                objViewModel.ErrorList.AddRange(errorListtemp);
                            }
                            else
                            {
                                string v = "cb_28_1_" + item.RowId;
                                PdfLoadedCheckBoxField CheckField = loadedForm.Fields[v] as PdfLoadedCheckBoxField;
                                if (CheckField.Checked)
                                {
                                    errorListtemp = objCommonFunction.FetchPDFFilledDataToQMModel(item, loadedForm);
                                    objViewModel.ErrorList.AddRange(errorListtemp);
                                }
                                else
                                {
                                    objViewModel.QM_CHILD_CC_AND_SR_PAVEMENTS_OBSERVATION_DETAILS.Remove(item);
                                }
                            }

                        }

                        #endregion
                    }
                }

                if (chckBoxArr[19] == 'Y')
                {
                    #region EFORM_QM_CC_PUCCA_DRAINS
                    errorListtemp = objCommonFunction.FetchPDFFilledDataToQMModel(objViewModel.QM_CC_PUCCA_DRAINS, loadedForm);
                    objViewModel.ErrorList.AddRange(errorListtemp);
                    #endregion

                    #region EFORM_QM_CHILD_CC_PUCCA_DRAINS_OBSERVATION_DETAILS

                    TableProperties_CCPuccaDrainObsDetails TablePropCCPuccaDrainObsDetails = new TableProperties_CCPuccaDrainObsDetails();
                    for (int i = 1; i <= TablePropCCPuccaDrainObsDetails.RowCount; i++)
                    {
                        QMModels.EFORM_QM_CHILD_CC_PUCCA_DRAINS_OBSERVATION_DETAILS rowModel = new QMModels.EFORM_QM_CHILD_CC_PUCCA_DRAINS_OBSERVATION_DETAILS();
                        rowModel.RowId = i;
                        objViewModel.QM_CHILD_CC_PUCCA_DRAINS_OBSERVATION_DETAILS.Add(rowModel);
                    }

                    foreach (var item in objViewModel.QM_CHILD_CC_PUCCA_DRAINS_OBSERVATION_DETAILS.ToList())
                    {
                        if (item.RowId == 1)
                        {
                            errorListtemp = objCommonFunction.FetchPDFFilledDataToQMModel(item, loadedForm);
                            objViewModel.ErrorList.AddRange(errorListtemp);
                        }
                        else
                        {
                            string v = "cb_29_1_" + item.RowId;
                            PdfLoadedCheckBoxField CheckField = loadedForm.Fields[v] as PdfLoadedCheckBoxField;
                            if (CheckField.Checked)
                            {
                                errorListtemp = objCommonFunction.FetchPDFFilledDataToQMModel(item, loadedForm);
                                objViewModel.ErrorList.AddRange(errorListtemp);
                            }
                            else
                            {
                                objViewModel.QM_CHILD_CC_PUCCA_DRAINS_OBSERVATION_DETAILS.Remove(item);
                            }
                        }

                    }
                    #endregion
                }

                if (chckBoxArr[20] == 'Y')
                {
                    #region EFORM_QM_ROAD_FURNITURE_MARKINGS
                    errorListtemp = objCommonFunction.FetchPDFFilledDataToQMModel(objViewModel.QM_ROAD_FURNITURE_MARKINGS, loadedForm);
                    objViewModel.ErrorList.AddRange(errorListtemp);
                    #endregion
                    if (TemplateStatus)
                    {

                        #region EFORM_QM_CHILD_ROAD_FURNITURE_MARKINGS_OBSERVATION_DETAILS


                        int count = 0;
                        for (int i = 1; i <= 4; i++)
                        {

                            string chkBxName = "cb_30_1_" + i;
                            PdfLoadedCheckBoxField chkBx = loadedForm.Fields[chkBxName] as PdfLoadedCheckBoxField;
                            if (!chkBx.Checked)
                            {
                                count++;
                            }
                        }
                        if (count == 4)
                        {
                            errorListtemp.Clear();
                            errorListtemp.Add("Page-30: Item 20.ROAD FURNITURE AND MARKINGS- Please fill I. Observations - Quality and Quantity of Road Furniture and Markings details atleast for one furniture type");
                            objViewModel.ErrorList.AddRange(errorListtemp);
                            errorListtemp.Clear();
                            objViewModel.ErrorOccured = true;
                        }
                        else
                        {
                            TableProperties_RdFurnMarkObsDetails TablePropRdFurnMarkObsDetails = new TableProperties_RdFurnMarkObsDetails();
                            var FurnitureItemList = dbContext.EFORM_MASTER_WORK_ITEM.Where(x => x.ITEM_FLAG == "Q").OrderBy(x => x.ITEM_ID).Select(x => x.ITEM_ID).ToList();
                            for (int i = 1; i <= TablePropRdFurnMarkObsDetails.RowCount; i++)
                            {
                                short itemId = FurnitureItemList[i - 1];
                                QMModels.EFORM_QM_CHILD_ROAD_FURNITURE_MARKINGS_OBSERVATION_DETAILS rowModel = new QMModels.EFORM_QM_CHILD_ROAD_FURNITURE_MARKINGS_OBSERVATION_DETAILS();
                                rowModel.RowId = i;
                                rowModel.ITEM_ID = itemId;
                                objViewModel.QM_CHILD_ROAD_FURNITURE_MARKINGS_OBSERVATION_DETAILS.Add(rowModel);
                            }



                            int countTemp = 1;
                            foreach (var item in objViewModel.QM_CHILD_ROAD_FURNITURE_MARKINGS_OBSERVATION_DETAILS.ToList())
                            {

                                string v = "cb_30_1_" + countTemp++;
                                PdfLoadedCheckBoxField CheckField = loadedForm.Fields[v] as PdfLoadedCheckBoxField;
                                if (CheckField.Checked)
                                {
                                    errorListtemp = objCommonFunction.FetchPDFFilledDataToQMModel(item, loadedForm);
                                    objViewModel.ErrorList.AddRange(errorListtemp);
                                }
                                else
                                {
                                    objViewModel.QM_CHILD_ROAD_FURNITURE_MARKINGS_OBSERVATION_DETAILS.Remove(item);
                                }

                            }

                        }











                        #endregion

                    }
                }

                #endregion

                #region  ----saurabh page30-33 Model reading Code------
                #region  ---saurabh----
                // Page 31

                if (chckBoxArr[21] == 'Y')
                {
                    #region  EFORM_DEFICIENCY_PREPARATION_QM

                    PdfLoadedRadioButtonListField DeficiacyradioButton = loadedForm.Fields["IS_NO_DEFICIENCY"] as PdfLoadedRadioButtonListField;
                    string DeficiasyStatus = DeficiacyradioButton.SelectedValue;

                    if (DeficiasyStatus != null)
                    {
                        objViewModel.EFORM_DEFICIENCY_PREPARATION_QM = new QMModels.EFORM_DEFICIENCY_PREPARATION_QM(DeficiasyStatus);
                        errorListtemp = objCommonFunction.FetchPDFFilledDataToQMModel(objViewModel.EFORM_DEFICIENCY_PREPARATION_QM, loadedForm);
                        objViewModel.ErrorList.AddRange(errorListtemp);
                    }
                    else
                    {
                        errorListtemp.Clear();
                        errorListtemp.Add("Page - 31: Item 21. General Observations of QM- A. deficiency in project preparation must be selected");
                        objViewModel.ErrorList.AddRange(errorListtemp);
                        errorListtemp.Clear();
                        objViewModel.ErrorOccured = true;
                    }

                    #endregion


                    #region EFORM_WORK_COMPLETED_IN_PROGRESS_ASPER_COST_QM
                    if (TemplateStatus == true)
                    {
                        PdfLoadedRadioButtonListField WorkCompleteInCost = loadedForm.Fields["WORK_STATUS_32"] as PdfLoadedRadioButtonListField;
                        string WorkCompleteCostStatus = WorkCompleteInCost.SelectedValue;

                        if (WorkCompleteCostStatus != null)
                        {
                            objViewModel.EFORM_WORK_COMPLETED_IN_PROGRESS_ASPER_COST_QM = new QMModels.EFORM_WORK_COMPLETED_IN_PROGRESS_ASPER_COST_QM(WorkCompleteCostStatus);
                            errorListtemp = objCommonFunction.FetchPDFFilledDataToQMModel(objViewModel.EFORM_WORK_COMPLETED_IN_PROGRESS_ASPER_COST_QM, loadedForm);
                            objViewModel.ErrorList.AddRange(errorListtemp);
                        }
                        else
                        {
                            errorListtemp.Clear();
                            errorListtemp.Add("Page - 32: Item 21. General Observations of QM- C. Whether the work is completed within the sanctioned cost must be selected");
                            objViewModel.ErrorList.AddRange(errorListtemp);
                            errorListtemp.Clear();
                            objViewModel.ErrorOccured = true;
                        }
                    }


                    #endregion


                    #region   EFORM_WORK_COMPLETED_IN_PROGRESS_ASPER_PROG_QM

                    PdfLoadedRadioButtonListField ProgramStatus = loadedForm.Fields["WORK_STATUS_32_1"] as PdfLoadedRadioButtonListField;
                    PdfLoadedRadioButtonListField ISDelay = loadedForm.Fields["C_IS_COMPLETED_WITH_DELAY"] as PdfLoadedRadioButtonListField;
                    PdfLoadedRadioButtonListField ISDateExtended = loadedForm.Fields["P_IS_AS_PER_SCHEDULE"] as PdfLoadedRadioButtonListField;
                    bool DateExtended = false;
                    bool TableStatus = false;
                    bool TableStatusInProgress = false;
                    string DelayStatus = ISDelay.SelectedValue;
                    bool DELStatus = false;
                    if (ProgramStatus.SelectedValue == "C" && ProgramStatus.SelectedValue != null)
                    {
                        TableStatus = true;
                    }
                    if (ProgramStatus.SelectedValue == "P" && ProgramStatus.SelectedValue != null)
                    {
                        TableStatusInProgress = true;
                    }
                    if (ISDateExtended.SelectedValue == "S" && ISDateExtended.SelectedValue != null)
                    {
                        DateExtended = true;
                    }
                    if (DelayStatus == "N" && DelayStatus != null)
                    {
                        DELStatus = true;
                    }
                    objViewModel.EFORM_WORK_COMPLETED_IN_PROGRESS_ASPER_PROG_QM = new EFORM_WORK_COMPLETED_IN_PROGRESS_ASPER_PROG_QM(TableStatus, DELStatus, DateExtended, TableStatusInProgress);
                    errorListtemp = objCommonFunction.FetchPDFFilledDataToQMModel(objViewModel.EFORM_WORK_COMPLETED_IN_PROGRESS_ASPER_PROG_QM, loadedForm);
                    objViewModel.ErrorList.AddRange(errorListtemp);

                    #endregion


                    #region EFORM_DIFFEENCE_IN_OBSERVATION_QM

                    PdfLoadedRadioButtonListField DifferneceInObserv = loadedForm.Fields["IS_DIFFERENCE_FOUND"] as PdfLoadedRadioButtonListField;
                    string QMDifference = DifferneceInObserv.SelectedValue;
                    if (QMDifference != null)
                    {
                        objViewModel.EFORM_DIFFEENCE_IN_OBSERVATION_QM = new QMModels.EFORM_DIFFEENCE_IN_OBSERVATION_QM(QMDifference);
                        errorListtemp = objCommonFunction.FetchPDFFilledDataToQMModel(objViewModel.EFORM_DIFFEENCE_IN_OBSERVATION_QM, loadedForm);
                        objViewModel.ErrorList.AddRange(errorListtemp);
                    }
                    else
                    {
                        errorListtemp.Clear();
                        errorListtemp.Add("Page - 33: Item 21. General Observations of QM- E. Whether any Difference found from previous observations of QMs must be selected");
                        objViewModel.ErrorList.AddRange(errorListtemp);
                        errorListtemp.Clear();
                        objViewModel.ErrorOccured = true;
                    }

                    #endregion


                    #region EFORM_ACTION_TAKEN_PIU_QM

                    EFORM_ACTION_TAKEN_PIU_QM Action = new EFORM_ACTION_TAKEN_PIU_QM();
                    objViewModel.EFORM_ACTION_TAKEN_PIU_QM_List = new List<EFORM_ACTION_TAKEN_PIU_QM>();
                    for (int i = 1; i <= Action.ROW_ID; i++)
                    {
                        EFORM_ACTION_TAKEN_PIU_QM detailedItem = new EFORM_ACTION_TAKEN_PIU_QM();
                        detailedItem.ROW_ID = i;
                        objViewModel.EFORM_ACTION_TAKEN_PIU_QM_List.Add(detailedItem);

                    }
                    objViewModel.EFORM_ACTION_TAKEN_PIU_QM_List.ForEach(item =>
                    {
                        errorListtemp = objCommonFunction.FetchPDFFilledDataToQMModel(item, loadedForm);
                        objViewModel.ErrorList.AddRange(errorListtemp);

                    }
                 );

                    #endregion
                }

                // page 33

                #endregion

                #endregion


                #region --- Srishti Page 34-38 Model reading code --- 



                errorListtemp = objCommonFunction.FetchPDFFilledDataToQMModel(objViewModel.QM_QUALITY_GRADING, loadedForm);
                objViewModel.ErrorList.AddRange(errorListtemp);
                errorListtemp = objCommonFunction.FetchPDFFilledDataToQMModel(objViewModel.QM_OVERALL_GRADING, loadedForm);
                objViewModel.ErrorList.AddRange(errorListtemp);
                #endregion


                if (objViewModel.ErrorList.Count > 0)
                {
                    objViewModel.ErrorOccured = true;
                }

                return objViewModel;
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "eformcontroller.PdfDataToClassQMModel()");
                throw;
            }
            finally
            {
                loadedDocument.Close(true);
                dbContext.Dispose();
            }
        }



        #region   EFORM_BRIDGE_PIU_DEVELOPMENT


        public FileResult GetDownloadedPDFBrige(string encRoadCode)
        {


            PMGSYEntities dbContext = new PMGSYEntities();
            try
            {
                IPdfDataDAL objDAL = new PdfDataDAL();
                string decRoadCode = string.Empty;
                string qmName = string.Empty;
                string qmType = string.Empty;
                string qmMonth = string.Empty;
                string qmYear = string.Empty;
                string qmCode = string.Empty;
                string QmScheduleCode = string.Empty;
                string eformIDfromGrid = string.Empty;
                string workStatus = string.Empty;
                string proposaltype = string.Empty;
                string execLength = string.Empty;
                string[] encParam = encRoadCode.Split('/');
                decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { encParam[0], encParam[1], encParam[2] });
                if (decryptedParameters.Count > 0)
                {
                    decRoadCode = decryptedParameters["imsRoadID"].Split('$')[0];
                    qmName = decryptedParameters["imsRoadID"].Split('$')[1];
                    qmType = decryptedParameters["imsRoadID"].Split('$')[2];
                    qmMonth = decryptedParameters["imsRoadID"].Split('$')[3];
                    qmYear = decryptedParameters["imsRoadID"].Split('$')[4];
                    qmCode = decryptedParameters["imsRoadID"].Split('$')[5];
                    QmScheduleCode = decryptedParameters["imsRoadID"].Split('$')[6];
                    eformIDfromGrid = decryptedParameters["imsRoadID"].Split('$')[7];
                    workStatus = decryptedParameters["imsRoadID"].Split('$')[8];
                    proposaltype = decryptedParameters["imsRoadID"].Split('$')[9];
                    execLength = decryptedParameters["imsRoadID"].Split('$')[10];
                }
                int roadCode = Convert.ToInt32(decRoadCode);


                int eformIDfrmGrid = Convert.ToInt32(eformIDfromGrid);


                if (Convert.ToInt32(qmMonth) <= 9 && Convert.ToInt32(qmYear) <= 2022)
                {
                    using (var scope = new TransactionScope())
                    {
                        var prefFetailslist = dbContext.EFORM_PREFILLED_DETAILS.Where(s => s.EFORM_ID == eformIDfrmGrid).ToList();

                        for (int i = 0; i < prefFetailslist.Count; i++)
                        {
                            int prefId = prefFetailslist[i].PREFILLED_ID;
                            EFORM_PREFILLED_DETAILS prefmodel = dbContext.EFORM_PREFILLED_DETAILS.Where(s => s.PREFILLED_ID == prefId && s.EFORM_ID == eformIDfrmGrid).FirstOrDefault();
                            prefmodel.PHYSICAL_WORK_STATUS = workStatus;
                            prefmodel.SANCTION_LENGTH = dbContext.IMS_SANCTIONED_PROJECTS.Where(s => s.IMS_PR_ROAD_CODE == roadCode).Select(m => m.IMS_BRIDGE_LENGTH).FirstOrDefault();
                            prefmodel.EXECUTED_LENGTH = Convert.ToDecimal(execLength);
                            dbContext.Entry(prefmodel).State = EntityState.Modified;
                            dbContext.SaveChanges();
                        }


                        scope.Complete();
                    }
                }






                string Template_File = string.Empty;
                string filepath = ConfigurationManager.AppSettings["TEMPLATE_FILE"].ToString();
                if (workStatus.Equals("C"))
                {
                    Template_File = Path.Combine(filepath, "PIU_BRIDGE_TEMPLET_C.pdf");
                }
                else if (workStatus.Equals("P"))
                {

                    Template_File = Path.Combine(filepath, "PIU_BRIDGE_TEMPLET_P.pdf");

                }
                PdfLoadedDocument loadedDocument = new PdfLoadedDocument(Template_File);
                PdfLoadedForm loadedForm = loadedDocument.Form;
                string templateVersionValue = string.Empty;
                try
                {
                    PdfLoadedTextBoxField templateVersion = loadedForm.Fields["TEMPLATE_VERSION"] as PdfLoadedTextBoxField;
                    templateVersionValue = templateVersion.Text;
                }
                catch (Exception ex)
                {
                    ErrorLog.LogError(ex, "eformController.GetDownloadedPDFBrige()");
                }

                EFORM_DU_LOG LogDetail = new EFORM_DU_LOG();
                string CheckSumString = decRoadCode + qmMonth + qmYear + qmCode + "PIUBRIDGE";
                string CheckSumByteCode = objDAL.ComputeStringToSha256Hash(CheckSumString);
                LogDetail.PDF_CHECKSUM = CheckSumByteCode;
                if (templateVersionValue != "")
                {
                    LogDetail.TEMPLATE_VERSION = templateVersionValue;
                }
                else
                {
                    LogDetail.TEMPLATE_VERSION = "V1.0";
                }
                LogDetail.EFORM_ID = dbContext.EFORM_MASTER.Where(s => s.EFORM_ID == eformIDfrmGrid).Select(d => d.EFORM_ID).FirstOrDefault();
                bool IsInserLogSuccess = objDAL.InsertDownloadLogDetails(LogDetail);




                if (IsInserLogSuccess == false)
                {

                    string error_File = ConfigurationManager.AppSettings["ERROR_FILE"].ToString();
                    string Errorfilename = "Error.pdf";
                    byte[] FileBytesFalse = System.IO.File.ReadAllBytes(error_File);
                    return File(FileBytesFalse, "application/pdf", Errorfilename);
                }


                PIU_BRIDGE_GET_PREFILLRD_DETAILS imsDetailsList = new PIU_BRIDGE_GET_PREFILLRD_DETAILS();   // CHANGED
                imsDetailsList = objDAL.getPIUBridgePrefilledList(roadCode, eformIDfrmGrid);      // CHANGED  on 03-08-2022 
                int schdcode = Convert.ToInt32(QmScheduleCode);
                int eform_id = Convert.ToInt32(eformIDfromGrid);





                var list = (from ma in dbContext.EFORM_PIU_PREVIOUS_INSP_DETAILS
                            where ma.EFORM_ID == eform_id && ma.INSERT_OR_UPDATE != "I"
                            select new
                            {
                                ma.INSP_ID,
                                ma.VISIT_DATE,
                                ma.VISITOR_NAME_DESG,
                                ma.ROAD_FROM,
                                ma.ROAD_TO,  // CONFIRM REQUIREMENT

                            }).OrderBy(t => t.VISIT_DATE).ToList();



                BridgePerfilledModel model = new BridgePerfilledModel();

                for (int i = 0; i < list.Count && i < 8; i++)
                {
                    if (i == 0)
                    {
                        model.INSP_ID_1 = list[i].INSP_ID.ToString();
                        model.VISIT_DATE_1 = list[i].VISIT_DATE.ToString().Split(' ')[0];
                        model.VISITOR_NAME_DESG_1 = list[i].VISITOR_NAME_DESG.ToString();
                        model.ROAD_FROM_1 = list[i].ROAD_FROM.ToString();
                        model.ROAD_TO_1 = list[i].ROAD_TO.ToString();
                    }
                    if (i == 1)
                    {
                        model.INSP_ID_2 = list[i].INSP_ID.ToString();
                        model.VISIT_DATE_2 = list[i].VISIT_DATE.ToString().Split(' ')[0];
                        model.VISITOR_NAME_DESG_2 = list[i].VISITOR_NAME_DESG.ToString();
                        model.ROAD_FROM_2 = list[i].ROAD_FROM.ToString();
                        model.ROAD_TO_2 = list[i].ROAD_TO.ToString();
                    }
                    if (i == 2)
                    {
                        model.INSP_ID_3 = list[i].INSP_ID.ToString();
                        model.VISIT_DATE_3 = list[i].VISIT_DATE.ToString().Split(' ')[0];
                        model.VISITOR_NAME_DESG_3 = list[i].VISITOR_NAME_DESG.ToString();
                        model.ROAD_FROM_3 = list[i].ROAD_FROM.ToString();
                        model.ROAD_TO_3 = list[i].ROAD_TO.ToString();
                    }
                    if (i == 3)
                    {
                        model.INSP_ID_4 = list[i].INSP_ID.ToString();
                        model.VISIT_DATE_4 = list[i].VISIT_DATE.ToString().Split(' ')[0];
                        model.VISITOR_NAME_DESG_4 = list[i].VISITOR_NAME_DESG.ToString();
                        model.ROAD_FROM_4 = list[i].ROAD_FROM.ToString();
                        model.ROAD_TO_4 = list[i].ROAD_TO.ToString();
                    }
                    if (i == 4)
                    {
                        model.INSP_ID_5 = list[i].INSP_ID.ToString();
                        model.VISIT_DATE_5 = list[i].VISIT_DATE.ToString().Split(' ')[0];
                        model.VISITOR_NAME_DESG_5 = list[i].VISITOR_NAME_DESG.ToString();
                        model.ROAD_FROM_5 = list[i].ROAD_FROM.ToString();
                        model.ROAD_TO_5 = list[i].ROAD_TO.ToString();
                    }
                    if (i == 5)
                    {
                        model.INSP_ID_6 = list[i].INSP_ID.ToString();
                        model.VISIT_DATE_6 = list[i].VISIT_DATE.ToString().Split(' ')[0];
                        model.VISITOR_NAME_DESG_6 = list[i].VISITOR_NAME_DESG.ToString();
                        model.ROAD_FROM_6 = list[i].ROAD_FROM.ToString();
                        model.ROAD_TO_6 = list[i].ROAD_TO.ToString();
                    }
                    if (i == 6)
                    {
                        model.INSP_ID_7 = list[i].INSP_ID.ToString();
                        model.VISIT_DATE_7 = list[i].VISIT_DATE.ToString().Split(' ')[0];
                        model.VISITOR_NAME_DESG_7 = list[i].VISITOR_NAME_DESG.ToString();
                        model.ROAD_FROM_7 = list[i].ROAD_FROM.ToString();
                        model.ROAD_TO_7 = list[i].ROAD_TO.ToString();
                    }
                    if (i == 7)
                    {
                        model.INSP_ID_8 = list[i].INSP_ID.ToString();
                        model.VISIT_DATE_8 = list[i].VISIT_DATE.ToString().Split(' ')[0];
                        model.VISITOR_NAME_DESG_8 = list[i].VISITOR_NAME_DESG.ToString();
                        model.ROAD_FROM_8 = list[i].ROAD_FROM.ToString();
                        model.ROAD_TO_8 = list[i].ROAD_TO.ToString();
                    }
                }

                for (int i = list.Count; i < 8; i++)
                {
                    string inspLevel = "INSP_LEVEL_" + (i + 1);
                    string obs = "OBSERVATIONS_" + (i + 1);
                    string action = "ACTION_" + (i + 1);
                    PdfLoadedComboBoxField inspLevelFieldCbx = loadedForm.Fields[inspLevel] as PdfLoadedComboBoxField;
                    PdfLoadedTextBoxField obsField = loadedForm.Fields[obs] as PdfLoadedTextBoxField;
                    PdfLoadedTextBoxField actionField = loadedForm.Fields[action] as PdfLoadedTextBoxField;
                    inspLevelFieldCbx.ReadOnly = true;
                    obsField.ReadOnly = true;
                    actionField.ReadOnly = true;
                    inspLevelFieldCbx.Visibility = Syncfusion.Pdf.Interactive.PdfFormFieldVisibility.Hidden;
                }

                model.RoadCode = URLEncrypt.EncryptParameters1(new string[] { "encRoadCode=" + imsDetailsList.RoadCode.ToString() + "$" + imsDetailsList.Eform_Id.ToString() });
                model.checksum = CheckSumByteCode;
                model.EFORM_ID = imsDetailsList.Eform_Id.ToString();
                model.WORK_STATUS = workStatus;
                model.QM_NAME = imsDetailsList.QM_NAME;
                model.QM_TYPE = imsDetailsList.QM_TYPE == "S" ? "S" : "N";
                model.STATE = imsDetailsList.State;
                model.DISTRICT = imsDetailsList.District;
                model.BLOCK = imsDetailsList.Block;
                model.BRIDGE_NAME = imsDetailsList.RoadName;
                model.PACKAGE_NUMBER = imsDetailsList.PkgNumber;
                model.SANC_LENGTH = imsDetailsList.SancLength.ToString();
                model.ESTIMATED_COST = imsDetailsList.ESTIMATED_COST.ToString();
                model.AWARDED_COST = imsDetailsList.AWARDED_COST.ToString();
                if (workStatus.Equals("P"))
                {
                    model.EXPENDITURE_DONE = imsDetailsList.EXPENDITURE_DONE.ToString();
                }

                model.AWARD_OF_WORK_DATE = imsDetailsList.AWARD_OF_WORK_DATE.Split(' ')[0];
                model.START_OF_WORK_DATE = imsDetailsList.START_OF_WORK_DATE.Split(' ')[0];
                model.STIPULATED_COMPLETION_DATE = imsDetailsList.STIPULATED_COMPLETION_DATE.Split(' ')[0];



                new CommonFunctions().GeneratePDFPreFilledDataModel(model, loadedForm);




                if (workStatus.Equals("C"))
                {
                    PdfLoadedTextBoxField complCost = loadedForm.Fields["COMPLETION_COST"] as PdfLoadedTextBoxField;
                    complCost.ReadOnly = false;
                }

                //save prefilled pdf
                string Generated_Path = ConfigurationManager.AppSettings["TEMPORARY_FILES_PATH"].ToString();
                if (!Directory.Exists(Generated_Path))
                    Directory.CreateDirectory(Generated_Path);


                string fileName = eform_id + "PIU_" + System.DateTime.Now.ToString("dd_MM_yyyy_HHmmss") + ".pdf";
                var fullFilePath1 = Path.Combine(Generated_Path, fileName);
                loadedDocument.Save(fullFilePath1);
                loadedDocument.Close(true);
                //Close the document
                byte[] FileBytes = System.IO.File.ReadAllBytes(fullFilePath1);



                try
                {
                    string temp_Path = ConfigurationManager.AppSettings["TEMPORARY_FILES_PATH"];
                    System.IO.DirectoryInfo di = new DirectoryInfo(temp_Path);

                    foreach (FileInfo file in di.GetFiles())
                    {
                        try
                        {
                            file.Delete();
                        }
                        catch (Exception ex)
                        {
                            ErrorLog.LogError(ex, "eformcontroller.GetDownloadedPDFBrige()");
                        }

                    }

                }
                catch (Exception ex)
                {
                    ErrorLog.LogError(ex, "eformcontroller.GetDownloadedPDFBrige()");

                }
                return File(FileBytes, "application/pdf", fileName);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "eformController.GetDownloadedPDFBrige()");
                string error_File = ConfigurationManager.AppSettings["ERROR_FILE"].ToString();
                string Errorfilename = "Error.pdf";
                byte[] FileBytes = System.IO.File.ReadAllBytes(error_File);
                return File(FileBytes, "application/pdf", Errorfilename);
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        [RequiredAuthentication]
        [HttpPost]
        public ActionResult UploadBridgePdfFilePIU(string id)
        {
            string id1 = id;

            bool flag = true;
            string responseMessage = string.Empty;
            string fname;
            string uploaded_File, temp_uploaded_File;

            string schdCode = id1.Split('$')[1];
            string schdMonth = id1.Split('$')[2];
            string schdYear = id1.Split('$')[3];
            string workStatus = id1.Split('$')[4];
            string qmCode = id1.Split('$')[5];
            string qmType = id1.Split('$')[6];

            id = id1.Split('$')[0];
            string eformIDFromGrid = id1.Split('$')[7];
            PMGSYEntities eformdbContext = new PMGSYEntities();
            List<SelectListItem> result = new List<SelectListItem>();
            IPdfDataDAL objDAL = new PdfDataDAL();
            if (Request.Files.Count > 0)
            {
                HttpPostedFileBase file = Request.Files[0];
                List<SelectListItem> validationList = new List<SelectListItem>();
                int fileSizeLimit = Convert.ToInt16(ConfigurationManager.AppSettings["PIU_UPLOADED_FILE_SIZE"]) * 1024 * 1024;
                if (file.ContentLength < fileSizeLimit)
                {
                    if (file != null && file.ContentLength > 0 && Path.GetExtension(file.FileName).ToLower() == ".pdf")
                    {
                        try
                        {
                            int RoadId = Convert.ToInt32(id);
                            fname = eformIDFromGrid + "_PIU_uploaded_" + System.DateTime.Now.ToString("dd_MM_yyyy_HHmmss") + ".pdf";
                            string temp_Uploaded_Path = ConfigurationManager.AppSettings["TEMPORARY_FILES_PATH"];
                            flag = true;
                            if (!Directory.Exists(temp_Uploaded_Path))
                                Directory.CreateDirectory(temp_Uploaded_Path);
                            temp_uploaded_File = Path.Combine(temp_Uploaded_Path, fname);
                            file.SaveAs(temp_uploaded_File);


                            String Uploaded_Path = ConfigurationManager.AppSettings["PIU_UPLOADED_PATH"];
                            if (!Directory.Exists(Uploaded_Path))
                                Directory.CreateDirectory(Uploaded_Path);
                            uploaded_File = Path.Combine(Uploaded_Path, fname);


                            PdfLoadedDocument loadedDocument = new PdfLoadedDocument(temp_uploaded_File);
                            PdfLoadedForm loadedForm = loadedDocument.Form;

                            try
                            {
                                PdfLoadedTextBoxField RoadCode1 = loadedForm.Fields["RoadCode"] as PdfLoadedTextBoxField;
                            }
                            catch (Exception e)
                            {
                                var roadAlert = "";

                                roadAlert = "Please Fill the downloaded e-Form Pdf in adobe acrobat reader dc and then upload.";
                                validationList.Add(new SelectListItem { Text = roadAlert });
                                var validationData1 = validationList.Select(x => x.Text).ToList();
                                flag = false;
                                ErrorLog.LogError(e, "eformController.UploadBridgePdfFilePIU()");
                                return Json(new { success = flag, data = validationData1, responseMsg = roadAlert });

                            }
                            PdfLoadedTextBoxField RoadCode = loadedForm.Fields["RoadCode"] as PdfLoadedTextBoxField;
                            PdfLoadedTextBoxField checksum = loadedForm.Fields["checksum"] as PdfLoadedTextBoxField;
                            string decRoadCodetemp = string.Empty;
                            string decRoadCode = string.Empty;
                            string eformIdfrompdf = string.Empty;
                            string[] encRoadCode = RoadCode.Text.ToString().Split('/');
                            if (encRoadCode.Length > 1)
                            {
                                decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { encRoadCode[0], encRoadCode[1], encRoadCode[2] });

                                if (decryptedParameters.Count > 0)
                                {
                                    decRoadCodetemp = decryptedParameters["encRoadCode"];
                                    decRoadCode = decRoadCodetemp.Split('$')[0];
                                    eformIdfrompdf = decRoadCodetemp.Split('$')[1];
                                }
                            }
                            else
                            {
                                decRoadCode = RoadCode.Text;
                            }

                            double templateVers = 0;
                            try
                            {
                                PdfLoadedTextBoxField templateVersion = loadedForm.Fields["TEMPLATE_VERSION"] as PdfLoadedTextBoxField;
                                if (templateVersion.Text != "")
                                {
                                    templateVers = Convert.ToDouble(templateVersion.Text.Replace("V", ""));
                                }

                            }
                            catch (Exception ex)
                            {
                                ErrorLog.LogError(ex, "eformController.UploadBridgePdfFilePIU");
                            }

                            if (!decRoadCode.Equals(id))
                            {
                                var roadAlert = "";
                                var str = eformdbContext.IMS_SANCTIONED_PROJECTS.Where(x => x.IMS_PR_ROAD_CODE == RoadId).Select(u => new { u.IMS_ROAD_NAME }).Single();
                                roadAlert = " Please Upload correct Pdf for bridge " + str.IMS_ROAD_NAME;
                                validationList.Add(new SelectListItem { Text = roadAlert });
                                var validationData1 = validationList.Select(x => x.Text).ToList();
                                flag = false;
                                return Json(new { success = flag, data = validationData1, responseMsg = roadAlert });
                            }

                            if (!eformIdfrompdf.Equals(eformIDFromGrid))
                            {
                                string eformIdAlert = " Please Upload correct Pdf for bridge for eform Id: " + eformIDFromGrid;
                                validationList.Add(new SelectListItem { Text = eformIdAlert });
                                var validationData1 = validationList.Select(x => x.Text).ToList();
                                flag = false;
                                return Json(new { success = flag, data = validationData1, responseMsg = eformIdAlert });
                            }


                            int eformId = Convert.ToInt32(eformIDFromGrid);
                            string checksumfrmDB = eformdbContext.EFORM_DU_LOG.Where(s => s.EFORM_ID == eformId && s.USER_TYPE == "P").OrderByDescending(p => p.LOG_ID).Select(m => m.PDF_CHECKSUM).FirstOrDefault();
                            //  string RoadStatus = eformdbContext.EFORM_PREFILLED_DETAILS.Where(s => s.EFORM_ID == eformId).Select(s => s.PHYSICAL_WORK_STATUS).FirstOrDefault();
                            bool TemplateStatus = false;
                            if (workStatus == "C")
                            {
                                TemplateStatus = true;
                            }

                            if (!checksum.Text.Equals(checksumfrmDB))
                            {
                                string roadAlert = "Please upload latest downloaded pdf";
                                validationList.Add(new SelectListItem { Text = "Please upload latest downloaded pdf" });
                                var validationData1 = validationList.Select(x => x.Text).ToList();
                                flag = false;
                                return Json(new { success = flag, data = validationData1, responseMsg = roadAlert });
                            }

                            loadedDocument.Close(true);


                            EFORM_BRIDGE_PIU_VIEW_MODEL objViewModel = PdfBridgeDataToClassModel(temp_uploaded_File, TemplateStatus, eformIDFromGrid);

                            StringBuilder strErrorMessage = new StringBuilder("");
                            int count = 0;
                            if (objViewModel.ErrorOccured)
                            {
                                objViewModel.ErrorList = objViewModel.ErrorList.Distinct().ToList();
                                #region 31-03-2022
                                List<string> errorslist = new List<string>();
                                for (int i = 1; i <= 8; i++)
                                {
                                    string Errorpage = "Page-";
                                    errorslist = objViewModel.ErrorList.Where(a => a.Contains(string.Concat(Errorpage + i))).ToList();


                                    if (errorslist.Count > 0)
                                    {
                                        strErrorMessage.Append("<h4 style='background-color:#efede5; color:black'> Page " + i + " </h4>");

                                    }

                                    foreach (var ierror in errorslist)
                                    {
                                        strErrorMessage.Append("<li style='margin-left:10px; list-style-type: square'> ");
                                        strErrorMessage.Append("Error-" + ++count + " " + ierror);
                                        strErrorMessage.Append("<br />");
                                        strErrorMessage.Append("</li>");
                                    }
                                }
                                #endregion



                                return Json(new { success = false, data = strErrorMessage.ToString(), IsValidationError = true, responseMsg = "Please fill all mandatory details in file" });
                            }
                            //save to db

                            result = objDAL.SaveBridgePIUToDb(temp_uploaded_File, eformIDFromGrid, fname, Uploaded_Path, file, uploaded_File, objViewModel, schdCode, schdMonth, schdYear, workStatus, qmCode, qmType, templateVers);

                            var validationData = result.Select(x => x.Text).ToList();

                            if (result.Count > 0)
                            {
                                responseMessage = "Error occured while uploading pdf";
                                flag = false;
                                return Json(new { success = flag, data = validationData, responseMsg = responseMessage });
                            }
                            else
                            {
                                loadedDocument.Close();
                                responseMessage = "Upload Successful.";
                            }


                        }
                        catch (Exception ex)
                        {
                            ErrorLog.LogError(ex, "eformcontroller.UploadBridgePdfFilePIU()");
                            responseMessage = "Error occured while uploading pdf, please contact OMMAS team.";
                            validationList.Add(new SelectListItem { Text = "Error occured while uploading pdf, please contact OMMAS team." });
                            var validationData1 = validationList.Select(x => x.Text).ToList();
                            flag = false;
                            return Json(new { success = flag, data = validationData1, responseMsg = responseMessage });
                        }
                        finally
                        {
                            file.InputStream.Flush();
                            file.InputStream.Close();
                            eformdbContext.Dispose();
                        }

                    }
                    else
                    {
                        flag = false;
                        responseMessage = "File is invalid.";
                        validationList.Add(new SelectListItem { Text = "File is invalid." });
                        var validationData1 = validationList.Select(x => x.Text).ToList();
                        return Json(new { success = flag, data = validationData1, responseMsg = responseMessage });
                    }
                }
                else
                {
                    flag = false;
                    responseMessage = "Invalid file size. Please upload file upto 6 MB.";
                    validationList.Add(new SelectListItem { Text = "Invalid file size. Please upload file upto 6 MB." });
                    var validationData1 = validationList.Select(x => x.Text).ToList();
                    return Json(new { success = flag, data = validationData1, responseMsg = responseMessage });
                }
            }
            else
            {
                flag = false;
                responseMessage = "File Upload has no file.";
            }

            return Json(new { success = flag, message = responseMessage }, JsonRequestBehavior.AllowGet);
        }
        [RequiredAuthentication]
        public JsonResult deleteEformBridgePIUDetail(string id)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            string roadIDdec = id.Split('$')[0];
            string eformIddec = id.Split('$')[1];
            int roadId = Convert.ToInt32(roadIDdec);
            try
            {
                int eformId = Convert.ToInt32(eformIddec);
                EFORM_MASTER model = dbContext.EFORM_MASTER.Where(s => s.EFORM_ID == eformId).FirstOrDefault();

                var fileName = dbContext.EFORM_PDF_UPLOAD_DETAIL.Where(x => x.PR_ROAD_CODE == roadId && x.EFORM_ID == eformId && x.USER_TYPE == "P").Select(s => s.FILE_NAME).FirstOrDefault();
                // string Uploaded_Path = dbContext.EFORM_PDF_UPLOAD_DETAIL.Where(x => x.PR_ROAD_CODE == roadId && x.EFORM_ID == eformId && x.USER_TYPE == "P").Select(s => s.FILE_PATH).FirstOrDefault();
                string Uploaded_Path = ConfigurationManager.AppSettings["PIU_UPLOADED_PATH"].ToString() + "/" + model.ADMIN_SCHEDULE_YEAR.ToString() + "/" + model.ADMIN_SCHEDULE_MONTH.ToString();

                using (var scope = new TransactionScope())
                {
                    dbContext.USP_EFORMS_BRIDGE_PIU_DELETE_INSPECTION_DETAILS(roadId, eformId);
                    dbContext.SaveChanges();
                    string fullPath = Path.Combine(Uploaded_Path, fileName);
                    FileInfo file = new FileInfo(fullPath);
                    if (file.Exists)
                    {
                        file.Delete();
                    }
                    scope.Complete();
                    return Json(new { success = true }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "eformcontroller.deleteEformBridgePIUDetail()");
                return Json(new { success = false, message = "error occured during deleting the pdf." }, JsonRequestBehavior.AllowGet);
            }
            finally
            {
                dbContext.Dispose();
            }

        }


        // added by rohit borse on 29-08-2022
        [RequiredAuthentication]
        public ActionResult PreviewBridgePIU_SavedData(String encId)
        {
            try
            {
                string[] encParam = encId.Split('/');
                decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { encParam[0], encParam[1], encParam[2] });
                int EformId = 0;

                if (decryptedParameters.Count > 0)
                {
                    EformId = Convert.ToInt32(decryptedParameters["imsRoadID"].Split('$')[0]);

                }

                IPdfDataDAL objDAL = new PdfDataDAL();
                EFORM_BRIDGE_PIU_PREVIEW_MODEL objPIU_Model = objDAL.PreviewBridgePIU_SavedDataDAL(EformId);

                return View("~/Areas/EFORMArea/Views/EFORM/PreviewBridgePIU_SavedData.cshtml", objPIU_Model);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "eformcontroller.PreviewBridgePIU_SavedData()");
                return View("~/Areas/EFORMArea/Views/EFORM/PDF_ErrorPage");
            }
        }






        private EFORM_BRIDGE_PIU_VIEW_MODEL PdfBridgeDataToClassModel(string filePath, bool TemplateStatus, string eformId)
        {
            PdfLoadedDocument loadedDocument = new PdfLoadedDocument(filePath);
            PMGSYEntities dbContext = new PMGSYEntities();

            try
            {
                #region added by rohit on 04-08-2022

                PDFFiledDataInspectorModel pDFFiledDataInspectorModel = new PDFFiledDataInspectorModel();

                PdfLoadedForm loadedForm = loadedDocument.Form;
                EFORM_BRIDGE_PIU_VIEW_MODEL objViewModel = new EFORM_BRIDGE_PIU_VIEW_MODEL();
                CommonFunctions objCommonFunction = new CommonFunctions();
                InspectionModelList inspectionModelList = new InspectionModelList();
                OffcialModelListBridge offcialModelList = new OffcialModelListBridge(TemplateStatus);
                objViewModel.ErrorList = new List<string>();
                objViewModel.BRIDGE_PIU_GENERAL_INFO = new PiuBridgeModel.EFORM_BRIDGE_PIU_GENERAL_INFO(TemplateStatus);
                objViewModel.BRIDGE_PIU_PRGS_DETAILS = new List<PiuBridgeModel.EFORM_BRIDGE_PIU_PRGS_DETAILS>();
                objViewModel.BRIDGE_PIU_PREVIOUS_INSP_DETAIL = new List<PiuBridgeModel.EFORM_BRIDGE_PIU_PREVIOUS_INSP_DETAILS>();
                objViewModel.BRIDGE_PIU_QC_DETAIL = new PiuBridgeModel.EFORM_BRIDGE_PIU_QC_DETAILS(TemplateStatus);
                objViewModel.BRIDGE_QC_OFFICIAL_DETAILS = new List<PiuBridgeModel.EFORM_BRIDGE_QC_OFFICIAL_DETAILS>();
                objViewModel.BRIDGE_PIU_MIX_DESIGN_DETAIL = new List<PiuBridgeModel.EFORM_BRIDGE_PIU_MIX_DESIGN_DETAILS>();
                objViewModel.BRIDGE_PIU_PARTICULAR = new PiuBridgeModel.EFORM_BRIDGE_PIU_PARTICULARS();


                List<string> errorListtemp = new List<string>();

                #region EFORM_BRIDGE_PIU_GENERAL_INFO

                objViewModel.ErrorList = objCommonFunction.FetchBridgePIUPDFFilledDataToModel(objViewModel.BRIDGE_PIU_GENERAL_INFO, loadedForm);

                #endregion EFORM_BRIDGE_PIU_GENERAL_INFO

                #region EFORM_BRIDGE_PIU_PRGS_DETAILS


                dbContext.EFORM_BRIDGE_MASTER_WORK_ITEM.Where(m => m.ITEM_FLAG == "P").Select(t => t.ITEM_ID).ToList().ForEach(itemID =>
                {
                    PiuBridgeModel.EFORM_BRIDGE_PIU_PRGS_DETAILS detailedItem = new PiuBridgeModel.EFORM_BRIDGE_PIU_PRGS_DETAILS();
                    detailedItem.ITEM_ID = itemID;
                    objViewModel.BRIDGE_PIU_PRGS_DETAILS.Add(detailedItem);
                });

                foreach (var item in objViewModel.BRIDGE_PIU_PRGS_DETAILS.ToList())
                {
                    errorListtemp = objCommonFunction.FetchBridgePIUPDFFilledDataToModel(item, loadedForm);
                    objViewModel.ErrorList.AddRange(errorListtemp);
                }





                #endregion EFORM_BRIDGE_PIU_PRGS_DETAILS

                #region PARTICULARS OF THE BRIDGE

                errorListtemp = objCommonFunction.FetchBridgePIUPDFFilledDataToModel(objViewModel.BRIDGE_PIU_PARTICULAR, loadedForm);
                objViewModel.ErrorList.AddRange(errorListtemp);

                #endregion PARTICULARS OF THE BRIDGE


                #region EFORM_BRIDGE_PIU_QC_DETAILS


                errorListtemp = objCommonFunction.FetchBridgePIUPDFFilledDataToModel(objViewModel.BRIDGE_PIU_QC_DETAIL, loadedForm);
                objViewModel.ErrorList.AddRange(errorListtemp);
                #endregion


                #region EFORM_QC_OFFICIAL_DETAILS_PIU
                offcialModelList.OfficialList.ForEach(item =>
                {

                    for (int i = 1; i <= item.RowCount; i++)
                    {
                        PiuBridgeModel.EFORM_BRIDGE_QC_OFFICIAL_DETAILS detailedItem = new PiuBridgeModel.EFORM_BRIDGE_QC_OFFICIAL_DETAILS();
                        detailedItem.OFFICIAL_TYPE = item.OfficialType;
                        detailedItem.RowID = i;
                        objViewModel.BRIDGE_QC_OFFICIAL_DETAILS.Add(detailedItem);
                    }

                });



                foreach (var item in objViewModel.BRIDGE_QC_OFFICIAL_DETAILS.ToList())
                {
                    if (item.RowID == 1)
                    {
                        errorListtemp = objCommonFunction.FetchBridgePIUPDFFilledDataToModel(item, loadedForm);
                        objViewModel.ErrorList.AddRange(errorListtemp);
                    }
                    else
                    {
                        if (item.OFFICIAL_TYPE == "C")
                        {
                            string v = "cb_3_1_" + item.RowID;
                            PdfLoadedCheckBoxField CheckField = loadedForm.Fields[v] as PdfLoadedCheckBoxField;
                            if (CheckField.Checked)
                            {

                                errorListtemp = objCommonFunction.FetchBridgePIUPDFFilledDataToModel(item, loadedForm);
                                objViewModel.ErrorList.AddRange(errorListtemp);
                            }
                            else
                            {
                                objViewModel.BRIDGE_QC_OFFICIAL_DETAILS.Remove(item);
                            }

                        }
                        else if (item.OFFICIAL_TYPE == "E")
                        {
                            string v = "cb_3_2_" + item.RowID;
                            PdfLoadedCheckBoxField CheckField = loadedForm.Fields[v] as PdfLoadedCheckBoxField;
                            if (CheckField.Checked)
                            {
                                errorListtemp = objCommonFunction.FetchBridgePIUPDFFilledDataToModel(item, loadedForm);
                                objViewModel.ErrorList.AddRange(errorListtemp);
                            }
                            else
                            {
                                objViewModel.BRIDGE_QC_OFFICIAL_DETAILS.Remove(item);
                            }


                        }

                        else if (item.OFFICIAL_TYPE == "S")
                        {
                            string v = "cb_3_4_" + item.RowID;
                            PdfLoadedCheckBoxField CheckField = loadedForm.Fields[v] as PdfLoadedCheckBoxField;
                            if (CheckField.Checked)
                            {
                                errorListtemp = objCommonFunction.FetchBridgePIUPDFFilledDataToModel(item, loadedForm);
                                objViewModel.ErrorList.AddRange(errorListtemp);
                            }
                            else
                            {
                                objViewModel.BRIDGE_QC_OFFICIAL_DETAILS.Remove(item);
                            }


                        }
                        else if (item.OFFICIAL_TYPE == "A")
                        {
                            string v = "cb_3_5_" + item.RowID;
                            PdfLoadedCheckBoxField CheckField = loadedForm.Fields[v] as PdfLoadedCheckBoxField;
                            if (CheckField.Checked)
                            {
                                errorListtemp = objCommonFunction.FetchBridgePIUPDFFilledDataToModel(item, loadedForm);
                                objViewModel.ErrorList.AddRange(errorListtemp);
                            }
                            else
                            {
                                objViewModel.BRIDGE_QC_OFFICIAL_DETAILS.Remove(item);
                            }


                        }
                        else if (item.OFFICIAL_TYPE == "J")
                        {
                            string v = "cb_4_1_" + item.RowID;
                            PdfLoadedCheckBoxField CheckField = loadedForm.Fields[v] as PdfLoadedCheckBoxField;
                            if (CheckField.Checked)
                            {
                                errorListtemp = objCommonFunction.FetchBridgePIUPDFFilledDataToModel(item, loadedForm);
                                objViewModel.ErrorList.AddRange(errorListtemp);
                            }
                            else
                            {
                                objViewModel.BRIDGE_QC_OFFICIAL_DETAILS.Remove(item);
                            }


                        }
                    }
                }





                #endregion


                #region EFORM_BRIDGE_MIX_DESIGN_DETAIL
                int countDesign = 0;
                for (int i = 1; i <= 9; i++)
                {
                    string chkBxName = "CB_5_" + i;
                    PdfLoadedCheckBoxField chkBx = loadedForm.Fields[chkBxName] as PdfLoadedCheckBoxField;
                    if (!chkBx.Checked)
                    {
                        countDesign++;
                    }
                }
                if (countDesign == 9)
                {
                    errorListtemp.Clear();
                    errorListtemp.Add("Page-7: Item 5. DETAILS OF MIX DESIGN- Please enter details for atleast one mix design ");
                    objViewModel.ErrorList.AddRange(errorListtemp);
                    errorListtemp.Clear();
                    objViewModel.ErrorOccured = true;
                }
                else
                {
                    dbContext.EFORM_BRIDGE_MASTER_WORK_ITEM.Where(m => m.ITEM_FLAG == "M").Select(x => x.ITEM_ID).ToList().ForEach(itemID =>
                    {
                        PiuBridgeModel.EFORM_BRIDGE_PIU_MIX_DESIGN_DETAILS DesignItem = new PiuBridgeModel.EFORM_BRIDGE_PIU_MIX_DESIGN_DETAILS();
                        DesignItem.ITEM_ID = itemID;
                        objViewModel.BRIDGE_PIU_MIX_DESIGN_DETAIL.Add(DesignItem);
                    });
                    int countTemp = 1;
                    foreach (var item in objViewModel.BRIDGE_PIU_MIX_DESIGN_DETAIL.ToList())
                    {
                        string v = "CB_5_" + countTemp++;  //CB_5_1
                        PdfLoadedCheckBoxField CheckField = loadedForm.Fields[v] as PdfLoadedCheckBoxField;
                        if (CheckField.Checked)
                        {
                            errorListtemp = objCommonFunction.FetchBridgePIUPDFFilledDataToModel(item, loadedForm);
                            objViewModel.ErrorList.AddRange(errorListtemp);
                        }
                        else
                        {
                            objViewModel.BRIDGE_PIU_MIX_DESIGN_DETAIL.Remove(item);
                        }
                    }
                }


                #endregion



                #region EFORM_BRIDGE_PREVIOUS_INSP_DETAILS_PIU


                inspectionModelList.InspectionList.ForEach(item =>
                {

                    for (int i = 1; i <= item.RowCount; i++)
                    {
                        PiuBridgeModel.EFORM_BRIDGE_PIU_PREVIOUS_INSP_DETAILS detailedItem = new PiuBridgeModel.EFORM_BRIDGE_PIU_PREVIOUS_INSP_DETAILS();
                        detailedItem.Inspection_TYPE = item.InspectionType;
                        detailedItem.RowID = i;
                        objViewModel.BRIDGE_PIU_PREVIOUS_INSP_DETAIL.Add(detailedItem);
                    }

                });

                objViewModel.BRIDGE_PIU_PREVIOUS_INSP_DETAIL.ForEach(item =>
                {
                    errorListtemp = objCommonFunction.FetchBridgePIUPDFFilledDataToModel(item, loadedForm);
                    objViewModel.ErrorList.AddRange(errorListtemp);

                }
               );
                for (int i = 0; i < objViewModel.BRIDGE_PIU_PREVIOUS_INSP_DETAIL.Count; i++)
                {
                    if (objViewModel.BRIDGE_PIU_PREVIOUS_INSP_DETAIL[i].INSP_LEVEL == "1")
                    {
                        objViewModel.BRIDGE_PIU_PREVIOUS_INSP_DETAIL[i].INSP_LEVEL = "Layout";
                    }
                    else if (objViewModel.BRIDGE_PIU_PREVIOUS_INSP_DETAIL[i].INSP_LEVEL == "2")
                    {
                        objViewModel.BRIDGE_PIU_PREVIOUS_INSP_DETAIL[i].INSP_LEVEL = "Foundation";
                    }
                    else if (objViewModel.BRIDGE_PIU_PREVIOUS_INSP_DETAIL[i].INSP_LEVEL == "3")
                    {
                        objViewModel.BRIDGE_PIU_PREVIOUS_INSP_DETAIL[i].INSP_LEVEL = "Substructure";
                    }
                    else if (objViewModel.BRIDGE_PIU_PREVIOUS_INSP_DETAIL[i].INSP_LEVEL == "4")
                    {
                        objViewModel.BRIDGE_PIU_PREVIOUS_INSP_DETAIL[i].INSP_LEVEL = "Superstructure";
                    }
                    else if (objViewModel.BRIDGE_PIU_PREVIOUS_INSP_DETAIL[i].INSP_LEVEL == "5")
                    {
                        objViewModel.BRIDGE_PIU_PREVIOUS_INSP_DETAIL[i].INSP_LEVEL = "Protection work";
                    }
                    else if (objViewModel.BRIDGE_PIU_PREVIOUS_INSP_DETAIL[i].INSP_LEVEL == "6")
                    {
                        objViewModel.BRIDGE_PIU_PREVIOUS_INSP_DETAIL[i].INSP_LEVEL = "Approaches";
                    }
                    else if (objViewModel.BRIDGE_PIU_PREVIOUS_INSP_DETAIL[i].INSP_LEVEL == "7")
                    {
                        objViewModel.BRIDGE_PIU_PREVIOUS_INSP_DETAIL[i].INSP_LEVEL = "Finishing stage";
                    }



                }




                #endregion






                // InCase Errors list have errors then Error Occured set as true
                if (objViewModel.ErrorList.Count > 0)
                {
                    objViewModel.ErrorOccured = true;
                }

                return objViewModel;
                #endregion
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "EFORMController.PdfBridgeDataToClassModel()");
            }
            finally
            {
                dbContext.Dispose();
            }
            return null;
        }


        #endregion

        #region EFORM_BRIDGE_QM_DEVELOPMENT
        [RequiredAuthentication]
        public FileResult GetDownloadedBRIDGEQMPDF(string encRoadCode)
        {
            PMGSYEntities dbContext = new PMGSYEntities();

            try
            {

                IPdfDataDAL objDAL = new PdfDataDAL();
                string decRoadCode = string.Empty;
                string qmName = string.Empty;
                string qmType = string.Empty;
                string qmMonth = string.Empty;
                string qmYear = string.Empty;
                string qmCode = string.Empty;
                string QmScheduleCode = string.Empty;
                string eformIdFrom_grid = string.Empty;
                string workStatus = string.Empty;
                string[] encParam = encRoadCode.Split('/');
                decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { encParam[0], encParam[1], encParam[2] });
                if (decryptedParameters.Count > 0)
                {
                    decRoadCode = decryptedParameters["imsRoadID"].Split('$')[0];
                    qmName = decryptedParameters["imsRoadID"].Split('$')[1];
                    qmType = decryptedParameters["imsRoadID"].Split('$')[2];
                    qmMonth = decryptedParameters["imsRoadID"].Split('$')[3];
                    qmYear = decryptedParameters["imsRoadID"].Split('$')[4];
                    qmCode = decryptedParameters["imsRoadID"].Split('$')[5];
                    QmScheduleCode = decryptedParameters["imsRoadID"].Split('$')[6];
                    eformIdFrom_grid = decryptedParameters["imsRoadID"].Split('$')[7];
                    workStatus = decryptedParameters["imsRoadID"].Split('$')[8];

                }

                int eform_Id = Convert.ToInt32(eformIdFrom_grid);
                int roadCode = Convert.ToInt32(decRoadCode);




                string Template_File = string.Empty;



                string filepath = ConfigurationManager.AppSettings["TEMPLATE_FILE"].ToString();
                if (workStatus.Equals("C"))
                {
                    Template_File = Path.Combine(filepath, "QM_BRIDGE_TEMPLET_C.pdf");
                }
                else if (workStatus.Equals("P"))
                {
                    Template_File = Path.Combine(filepath, "QM_BRIDGE_TEMPLET_P.pdf");
                }




                PdfLoadedDocument loadedDocument = new PdfLoadedDocument(Template_File);
                PdfLoadedForm loadedForm = loadedDocument.Form;


                string templateVersionValue = string.Empty;
                try
                {

                    PdfLoadedTextBoxField templateVersion = loadedForm.Fields["TEMPLATE_VERSION"] as PdfLoadedTextBoxField;
                    templateVersionValue = templateVersion.Text;
                }
                catch (Exception ex)
                {
                    ErrorLog.LogError(ex, "eformController.GetDownloadedBRIDGEQMPDF()");
                }


                EFORM_DU_LOG LogDetail = new EFORM_DU_LOG();
                string CheckSumString = decRoadCode + qmMonth + qmYear + qmCode + "QM";
                string CheckSumByteCode = objDAL.ComputeStringToSha256Hash(CheckSumString);
                LogDetail.PDF_CHECKSUM = CheckSumByteCode;
                LogDetail.EFORM_ID = eform_Id;
                if (templateVersionValue != "")
                {
                    LogDetail.TEMPLATE_VERSION = templateVersionValue;
                }
                else
                {
                    LogDetail.TEMPLATE_VERSION = "V1.0";

                }
                bool IsInserLogSuccess = objDAL.InsertDownloadLogDetails(LogDetail);


                if (IsInserLogSuccess == false)
                {

                    string error_File = ConfigurationManager.AppSettings["ERROR_FILE"].ToString();
                    string Errorfilename = "Error.pdf";
                    byte[] FileBytesFalse = System.IO.File.ReadAllBytes(error_File);
                    return File(FileBytesFalse, "application/pdf", Errorfilename);
                }

                PIU_GET_PREFILLED_DETAILS imsDetailsList = new PIU_GET_PREFILLED_DETAILS();



                imsDetailsList = objDAL.getPIUPrefilledList(roadCode, eform_Id);  //change on 08-07-2022






                BridgePrefilledModel model = new BridgePrefilledModel();




                model.RoadCode = URLEncrypt.EncryptParameters1(new string[] { "encRoadCode=" + imsDetailsList.RoadCode.ToString() + "$" + imsDetailsList.Eform_Id.ToString() });
                model.IMS_PR_ROAD_CODE_35 = URLEncrypt.EncryptParameters1(new string[] { "encRoadCode=" + imsDetailsList.RoadCode.ToString() + "$" + imsDetailsList.Eform_Id.ToString() });
                model.checksum = CheckSumByteCode;
                model.EFORM_ID = eform_Id.ToString();
                model.WORK_STATUS = workStatus;
                model.QM_NAME = imsDetailsList.QM_NAME;
                model.QM_TYPE = imsDetailsList.QM_TYPE == "S" ? "S" : "N";

                model.STATE = imsDetailsList.State;
                model.DISTRICT = imsDetailsList.District;
                model.BLOCK = imsDetailsList.Block;

                model.NAME_BRIDGE_LOCATION = imsDetailsList.RoadName;
                model.PACKAGE_NUMBER = imsDetailsList.PkgNumber;

                model.QM_NAME_34 = imsDetailsList.QM_NAME;
                model.QM_NAME_35 = imsDetailsList.QM_NAME;

                new CommonFunctions().GeneratePDFPreFilledDataModel(model, loadedForm);

                //save prefilled pdf
                string Generated_Path = ConfigurationManager.AppSettings["TEMPORARY_FILES_PATH"].ToString();
                if (!Directory.Exists(Generated_Path))
                    Directory.CreateDirectory(Generated_Path);
                string fileName = eform_Id + "QM_" + System.DateTime.Now.ToString("dd_MM_yyyy_HHmmss") + ".pdf";
                var fullFilePath = Path.Combine(Generated_Path, fileName);
                loadedDocument.Save(fullFilePath);



                //Close the document
                loadedDocument.Close(true);
                byte[] FileBytes = System.IO.File.ReadAllBytes(fullFilePath);

                try
                {
                    string temp_Path = ConfigurationManager.AppSettings["TEMPORARY_FILES_PATH"];
                    System.IO.DirectoryInfo di = new DirectoryInfo(temp_Path);

                    foreach (FileInfo file in di.GetFiles())
                    {
                        try
                        {
                            file.Delete();
                        }
                        catch (Exception ex)
                        {
                            ErrorLog.LogError(ex, "eformcontroller.GetDownloadedBRIDGEQMPDF()");
                        }

                    }

                }
                catch (Exception ex)
                {
                    ErrorLog.LogError(ex, "eformcontroller.GetDownloadedBRIDGEQMPDF()");

                }
                return File(FileBytes, "application/pdf", fileName);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "eformController.GetDownloadedBRIDGEQMPDF()");
                string filepath = ConfigurationManager.AppSettings["TEMPLATE_FILE"].ToString();
                string error_File = Path.Combine(filepath, "TEMPLATE_ERROR_PAGE.pdf");
                string Errorfilename = "Error.pdf";
                byte[] FileBytes = System.IO.File.ReadAllBytes(error_File);
                return File(FileBytes, "application/pdf", Errorfilename);
            }
            finally
            {
                dbContext.Dispose();
            }
        }


        [RequiredAuthentication]
        [HttpPost]
        public ActionResult UploadBRIDGEPdfFileQM(string id)
        {
            string id1 = id;
            IPdfDataDAL objDAL = new PdfDataDAL();
            bool flag = true;
            string responseMessage = string.Empty;
            string fname;
            string uploaded_File, temp_uploaded_File;
            List<SelectListItem> result = new List<SelectListItem>();

            PMGSYEntities eformdbContext = new PMGSYEntities();
            string schdCode = id1.Split('$')[1];
            string schdMonth = id1.Split('$')[2];
            string schdYear = id1.Split('$')[3];
            string workStatus = id1.Split('$')[4];
            string qmCode = id1.Split('$')[5];
            string qmType = id1.Split('$')[6];
            id = id1.Split('$')[0];
            string eformIDFromGrid = id1.Split('$')[7];
            int RoadId = Convert.ToInt32(id);
            int eformIdTemp = Convert.ToInt32(eformIDFromGrid);
            string PIUstatusfin = eformdbContext.EFORM_PDF_UPLOAD_DETAIL.Where(s => s.PR_ROAD_CODE == RoadId && s.EFORM_ID == eformIdTemp && s.USER_TYPE == "P").Select(x => x.IS_FINALISED).FirstOrDefault();

            var isPIUfilnalized = "N";
            if (PIUstatusfin == "Y")
            {
                isPIUfilnalized = "Y";
            }

            if (isPIUfilnalized != "Y")
            {
                List<SelectListItem> validationList1 = new List<SelectListItem>();

                flag = false;
                responseMessage = "Unable to upload e-form, As PIU is yet to be finalized Eform part-I";
                validationList1.Add(new SelectListItem { Text = "Unable to upload e-form, As PIU is yet to be finalized Eform part-I" });
                var validationData1 = validationList1.Select(x => x.Text).ToList();
                flag = false;
                return Json(new { success = flag, data = validationData1, responseMsg = responseMessage });
            }
            if (Request.Files.Count > 0)
            {
                HttpPostedFileBase file = Request.Files[0];
                List<SelectListItem> validationList = new List<SelectListItem>();
                //add more conditions like file type, file size etc as per your need.
                int fileSizeLimit = Convert.ToInt32(ConfigurationManager.AppSettings["QM_UPLOADED_FILE_SIZE"]) * 1024 * 1024;
                if (file.ContentLength < fileSizeLimit)
                {
                    if (file != null && file.ContentLength > 0 && Path.GetExtension(file.FileName).ToLower() == ".pdf")
                    {

                        try
                        {

                            fname = eformIDFromGrid + "_" + qmType + "QM_uploaded" + System.DateTime.Now.ToString("dd_MM_yyyy_HHmmss") + ".pdf";
                            String temp_Uploaded_Path = ConfigurationManager.AppSettings["TEMPORARY_FILES_PATH"].ToString();
                            flag = true;
                            if (!Directory.Exists(temp_Uploaded_Path))
                                Directory.CreateDirectory(temp_Uploaded_Path);
                            temp_uploaded_File = Path.Combine(temp_Uploaded_Path, fname);
                            file.SaveAs(temp_uploaded_File);

                            String Uploaded_Path = ConfigurationManager.AppSettings["NQM_SQM_UPLOADED_PATH"].ToString();
                            if (!Directory.Exists(Uploaded_Path))
                                Directory.CreateDirectory(Uploaded_Path);
                            uploaded_File = Path.Combine(Uploaded_Path, fname);


                            PdfLoadedDocument loadedDocument = new PdfLoadedDocument(temp_uploaded_File);
                            PdfLoadedForm loadedForm = loadedDocument.Form;

                            try
                            {
                                PdfLoadedTextBoxField RoadCode1 = loadedForm.Fields["RoadCode"] as PdfLoadedTextBoxField;
                            }
                            catch (Exception e)
                            {
                                var roadAlert = "";

                                roadAlert = "Please Fill the downloaded e-Form Pdf in adobe acrobat reader dc and then upload.";
                                validationList.Add(new SelectListItem { Text = roadAlert });
                                var validationData1 = validationList.Select(x => x.Text).ToList();
                                flag = false;
                                ErrorLog.LogError(e, "eformController.UploadBRIDGEPdfFileQM()");
                                return Json(new { success = flag, data = validationData1, responseMsg = roadAlert });

                            }
                            PdfLoadedTextBoxField RoadCode = loadedForm.Fields["RoadCode"] as PdfLoadedTextBoxField;
                            PdfLoadedTextBoxField checksum = loadedForm.Fields["checksum"] as PdfLoadedTextBoxField;
                            string decRoadCode = string.Empty;
                            string decRoadCodetemp = string.Empty;

                            string eformIdfrmPDF = string.Empty;
                            string[] encRoadCode = RoadCode.Text.ToString().Split('/');
                            if (encRoadCode.Length > 1)
                            {
                                decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { encRoadCode[0], encRoadCode[1], encRoadCode[2] });

                                if (decryptedParameters.Count > 0)
                                {
                                    decRoadCodetemp = decryptedParameters["encRoadCode"];
                                    decRoadCode = decRoadCodetemp.Split('$')[0];
                                    eformIdfrmPDF = decRoadCodetemp.Split('$')[1];
                                }
                            }
                            else
                            {
                                decRoadCode = RoadCode.Text;
                            }


                            if (!decRoadCode.Equals(id))
                            {

                                var roadAlert = "";
                                var str = eformdbContext.IMS_SANCTIONED_PROJECTS.Where(x => x.IMS_PR_ROAD_CODE == RoadId).Select(u => new { u.IMS_ROAD_NAME }).Single();
                                roadAlert = " Please Upload correct Pdf for road " + str.IMS_ROAD_NAME;

                                validationList.Add(new SelectListItem { Text = roadAlert });

                                var validationData1 = validationList.Select(x => x.Text).ToList();
                                flag = false;
                                return Json(new { success = flag, data = validationData1, responseMsg = roadAlert });
                            }
                            if (!eformIdfrmPDF.Equals(eformIDFromGrid))
                            {
                                string eformIdAlert = " Please Upload correct Pdf for road " + eformIDFromGrid;

                                validationList.Add(new SelectListItem { Text = eformIdAlert });

                                var validationData1 = validationList.Select(x => x.Text).ToList();
                                flag = false;
                                return Json(new { success = flag, data = validationData1, responseMsg = eformIdAlert });
                            }
                            int roadCodeChck = Convert.ToInt32(decRoadCode);
                            int eformId = eformdbContext.EFORM_MASTER.Where(s => s.EFORM_ID == eformIdTemp).Select(m => m.EFORM_ID).FirstOrDefault();
                            string checksumfrmDB = eformdbContext.EFORM_DU_LOG.Where(s => s.EFORM_ID == eformId && (s.USER_TYPE == "N" || s.USER_TYPE == "S")).OrderByDescending(p => p.LOG_ID).Select(m => m.PDF_CHECKSUM).FirstOrDefault();
                            string RoadStatus = eformdbContext.EFORM_PREFILLED_DETAILS.Where(s => s.EFORM_ID == eformIdTemp).Select(s => s.PHYSICAL_WORK_STATUS).FirstOrDefault();
                            bool TemplateStatus = false;

                            if (workStatus == "C")
                            {
                                TemplateStatus = true;
                            }



                            if (!checksum.Text.Equals(checksumfrmDB))
                            {
                                string roadAlert = "Please upload latest downloaded pdf";
                                validationList.Add(new SelectListItem { Text = "Please upload latest downloaded pdf" });
                                var validationData1 = validationList.Select(x => x.Text).ToList();
                                flag = false;
                                return Json(new { success = flag, data = validationData1, responseMsg = roadAlert });
                            }

                            loadedDocument.Close(true);
                            char[] a1 = new char[22];


                            for (int i = 2; i <= 11; i++)
                            {
                                string chkbxName = "Item_" + i;
                                PdfLoadedCheckBoxField CheckField = loadedForm.Fields[chkbxName] as PdfLoadedCheckBoxField;
                                if (CheckField.Checked)
                                {
                                    a1[i] = 'Y';
                                }
                                else
                                {
                                    a1[i] = 'N';
                                }

                            }




                            EFORM_BRIDGE_QM_VIEWMODEL objViewQMModel = PdfBridgeQMDataToClassModel(temp_uploaded_File, TemplateStatus, a1);  // A1

                            StringBuilder strErrorMessage = new StringBuilder("");
                            int count = 0;
                            if (objViewQMModel.ErrorOccured)
                            {
                                objViewQMModel.ErrorList = objViewQMModel.ErrorList.Distinct().ToList();
                                #region 31-03-2022
                                List<string> errorslist = new List<string>();
                                for (int i = 9; i <= 35; i++)
                                {
                                    string Errorpage = "Page-";


                                    errorslist = objViewQMModel.ErrorList.Where(a => a.Contains(string.Concat(Errorpage + i))).ToList();
                                    if (errorslist.Count > 0)
                                    {
                                        strErrorMessage.Append("<h4 style='background-color:#efede5; color:black'> Page " + i + " </h4>");
                                    }

                                    foreach (var ierror in errorslist)
                                    {
                                        strErrorMessage.Append("<li style='margin-left:10px; list-style-type: square'> ");
                                        strErrorMessage.Append("Error-" + ++count + " " + ierror);
                                        strErrorMessage.Append("<br />");
                                        strErrorMessage.Append("</li>");
                                    }
                                }
                                #endregion



                                return Json(new { success = false, data = strErrorMessage.ToString(), IsValidationError = true, responseMsg = "Please fill all mandatory details in file" });
                            }
                            //save to db
                            result = null;
                            result = objDAL.SaveToBridgeQMDb(temp_uploaded_File, eformIDFromGrid, fname, Uploaded_Path, file, uploaded_File, objViewQMModel, schdCode, schdMonth, schdYear, workStatus, qmCode, qmType, TemplateStatus, a1);
                            var validationData = result.Select(x => x.Text).ToList();

                            if (result.Count > 0)
                            {

                                flag = false;
                                validationList.Add(new SelectListItem { Text = "Error occured while uploading Eform. Please contact OMMAS team " });
                                var validationData1 = validationList.Select(x => x.Text).ToList();
                                return Json(new { success = flag, data = validationData1, responseMsg = "Error occured while uploading Eform. Please contact OMMAS team" });
                            }
                            else
                            {
                                loadedDocument.Close(true);
                                responseMessage = "Upload Successful.";
                            }


                        }
                        catch (Exception ex)
                        {
                            flag = false;
                            ErrorLog.LogError(ex, "eformcontroller.UploadBRIDGEPdfFileQM()");
                            responseMessage = "Error occured while uploading pdf, please contact OMMAS team.";
                            validationList.Add(new SelectListItem { Text = "Error occured while uploading pdf, please contact OMMAS team." });
                            var validationData1 = validationList.Select(x => x.Text).ToList();
                            return Json(new { success = flag, data = validationData1, responseMsg = responseMessage });
                        }
                        finally
                        {
                            file.InputStream.Flush();
                            file.InputStream.Close();
                            eformdbContext.Dispose();
                        }
                    }
                    else
                    {
                        flag = false;
                        responseMessage = "File is invalid.";
                        validationList.Add(new SelectListItem { Text = "File is invalid." });
                        var validationData1 = validationList.Select(x => x.Text).ToList();
                        return Json(new { success = flag, data = validationData1, responseMsg = responseMessage });
                    }
                }
                else
                {
                    flag = false;
                    responseMessage = "Invalid file size. Please upload file upto 12 MB..";
                    validationList.Add(new SelectListItem { Text = "Invalid file size. Please upload file upto 12 MB." });
                    var validationData1 = validationList.Select(x => x.Text).ToList();
                    return Json(new { success = flag, data = validationData1, responseMsg = responseMessage });
                }
            }
            else
            {
                flag = false;
                responseMessage = "File Upload has no file.";
            }

            return Json(new { success = flag, message = responseMessage }, JsonRequestBehavior.AllowGet);
        }

        [RequiredAuthentication]
        public JsonResult deleteBridgeQMEformDetail(string id)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            string roadIDdec = id.Split('$')[0];
            string eformIddec = id.Split('$')[1];
            int roadId = Convert.ToInt32(roadIDdec);
            string fileName = string.Empty;
            string Uploaded_Path_QM = string.Empty;
            string fileNameC = string.Empty;
            string Uploaded_Path_C = string.Empty;
            try
            {


                int eformId = Convert.ToInt32(eformIddec);
                using (var scope = new TransactionScope())
                {

                    EFORM_MASTER model = dbContext.EFORM_MASTER.Where(s => s.EFORM_ID == eformId).FirstOrDefault();
                    fileName = dbContext.EFORM_PDF_UPLOAD_DETAIL.Where(x => x.PR_ROAD_CODE == roadId && x.EFORM_ID == eformId && x.USER_TYPE == "Q").Select(s => s.FILE_NAME).FirstOrDefault();
                    // Uploaded_Path_QM = dbContext.EFORM_PDF_UPLOAD_DETAIL.Where(x => x.PR_ROAD_CODE == roadId && x.EFORM_ID == eformId && x.USER_TYPE == "Q").Select(s => s.FILE_PATH).FirstOrDefault();
                    Uploaded_Path_QM = ConfigurationManager.AppSettings["NQM_SQM_UPLOADED_PATH"].ToString() + "/" + model.ADMIN_SCHEDULE_YEAR.ToString() + "/" + model.ADMIN_SCHEDULE_MONTH.ToString();
                    fileNameC = dbContext.EFORM_PDF_UPLOAD_DETAIL.Where(x => x.PR_ROAD_CODE == roadId && x.EFORM_ID == eformId && x.USER_TYPE == "Q").Select(s => s.FILE_NAME_C).FirstOrDefault();
                    // Uploaded_Path_C = dbContext.EFORM_PDF_UPLOAD_DETAIL.Where(x => x.PR_ROAD_CODE == roadId && x.EFORM_ID == eformId && x.USER_TYPE == "Q").Select(s => s.FILE_PATH_C).FirstOrDefault();
                    Uploaded_Path_C = ConfigurationManager.AppSettings["COMBINE_UPLOADED_PATH"].ToString() + "/" + model.ADMIN_SCHEDULE_YEAR.ToString() + "/" + model.ADMIN_SCHEDULE_MONTH.ToString(); ;
                    if (fileName == null)
                    {
                        return Json(new { success = false, message = "error occured during deleting the pdf." }, JsonRequestBehavior.AllowGet);
                    }


                    dbContext.SaveChanges();
                    dbContext.USP_EFORMS_BRIDGE_QM_DELETE_INSPECTION_DETAILS(roadId, eformId);
                    string fullPath_QM = Path.Combine(Uploaded_Path_QM, fileName);

                    FileInfo file_QM = new FileInfo(fullPath_QM);
                    if (file_QM.Exists)
                    {
                        file_QM.Delete();
                    }
                    if (fileNameC != null)
                    {
                        string fullPath_QM_C = Path.Combine(Uploaded_Path_C, fileNameC);
                        FileInfo file_QM_C = new FileInfo(fullPath_QM_C);
                        if (file_QM_C.Exists)
                        {
                            file_QM_C.Delete();
                        }
                    }
                    scope.Complete();
                    return Json(new { success = true }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "eformcontroller.deleteBridgeQMEformDetail()");
                return Json(new { success = false, message = "error occured during deleting the pdf." }, JsonRequestBehavior.AllowGet);
            }
            finally
            {
                dbContext.Dispose();
            }

        }

        [RequiredAuthentication]
        public ActionResult PreviewBridgeQM_SavedData(string encIdtemp)
        {
            try
            {
                string[] encParam = encIdtemp.Split('/');
                decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { encParam[0], encParam[1], encParam[2] });
                int EformId = 0;
                if (decryptedParameters.Count > 0)
                {
                    EformId = Convert.ToInt32(decryptedParameters["imsRoadID"].Split('$')[0]);
                }
                IPdfDataDAL objDAL = new PdfDataDAL();
                EFORM_BRIDGE_QM_PREVIEW_MODEL objQMBridge_Model = objDAL.PreviewBridgeQM_SavedDataDAL(EformId);
                return View("~/Areas/EFORMArea/Views/EFORM/PreviewBridgeQM_SavedData.cshtml", objQMBridge_Model);

            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "eformcontroller.PreviewBridgeQM_SavedData()");
                return View("~/Areas/EFORMArea/Views/EFORM/PDF_ErrorPage");
            }
        }

        private EFORM_BRIDGE_QM_VIEWMODEL PdfBridgeQMDataToClassModel(string filePath, bool TemplateStatus, char[] chckBoxArr)
        {
            PdfLoadedDocument loadedDocument = new PdfLoadedDocument(filePath);
            PMGSYEntities dbContext = new PMGSYEntities();

            try
            {
                #region added by rohit on 04-08-2022

                PDFFiledDataInspectorModel pDFFiledDataInspectorModel = new PDFFiledDataInspectorModel();

                PdfLoadedForm loadedForm = loadedDocument.Form;
                EFORM_BRIDGE_QM_VIEWMODEL objViewModel = new EFORM_BRIDGE_QM_VIEWMODEL();
                CommonFunctions objCommonFunction = new CommonFunctions();

                objViewModel.BRIDGE_QM_SUBSTRUCTURE = new QMBridgeModel.EFORM_BRIDGE_QM_SUBSTRUCTURE();
                objViewModel.BRIDGE_QM_ONGOING_SUBSTRUCTURE = new QMBridgeModel.EFORM_BRIDGE_QM_ONGOING_SUBSTRUCTURE();
                objViewModel.BRIDGE_QM_COMPLETED_SUBSTRUCTURE = new QMBridgeModel.EFORM_BRIDGE_QM_COMPLETED_SUBSTRUCTURE();
                objViewModel.BRIDGE_QM_CHILD_ON_QOM_SUBSTRUCTURE_LIST = new List<QMBridgeModel.EFORM_BRIDGE_QM_CHILD_ON_QOM_SUBSTRUCTURE>();
                objViewModel.BRIDGE_QM_CHILD_ON_WORKMENSHIP_SUBSTRUCTURE_LIST = new List<QMBridgeModel.EFORM_BRIDGE_QM_CHILD_ON_WORKMENSHIP_SUBSTRUCTURE>();
                objViewModel.BRIDGE_QM_CHILD_COMPLETED_SUBSTRUCTURE_LIST = new List<QMBridgeModel.EFORM_BRIDGE_QM_CHILD_COMPLETED_SUBSTRUCTURE>();

                #region Vikky model page 9-15

                VerificationBridgeModelList verificationModelList = new VerificationBridgeModelList();
                OnQomFoundationList onQomFoundationList = new OnQomFoundationList();
                OnWorkmanshipFoundationList onWorkmanshipFoundationList = new OnWorkmanshipFoundationList();
                CompletedQomFoundationList completedQomFoundationList = new CompletedQomFoundationList();
                objViewModel.BRIDGE_QM_GENERAL_DETAILS = new QMBridgeModel.EFORM_BRIDGE_QM_GENERAL_DETAILS();
                objViewModel.BRIDGE_QM_ARRANGEMENT_OBS_DETAIL = new QMBridgeModel.EFORM_BRIDGE_QM_ARRANGEMENT_OBS_DETAIL();
                objViewModel.BRIDGE_QM_QUALITY_ATTENTION = new QMBridgeModel.EFORM_BRIDGE_QM_QUALITY_ATTENTION();
                objViewModel.BRIDGE_QM_QC_TEST_DETAILS_LIST = new List<QMBridgeModel.EFORM_BRIDGE_QM_QC_TEST_DETAILS>();
                objViewModel.BRIDGE_QM_TEST_RESULT_VERIFICATION_DETAILS_LIST = new List<QMBridgeModel.EFORM_BRIDGE_QM_TEST_RESULT_VERIFICATION_DETAILS>();
                objViewModel.BRIDGE_QM_FOUNDATION = new QMBridgeModel.EFORM_BRIDGE_QM_FOUNDATION();
                objViewModel.BRIDGE_QM_ONGOING_FOUNDATION = new QMBridgeModel.EFORM_BRIDGE_QM_ONGOING_FOUNDATION();
                objViewModel.BRIDGE_QM_CHILD_ON_QOM_FOUNDATION_LIST = new List<QMBridgeModel.EFORM_BRIDGE_QM_CHILD_ON_QOM_FOUNDATION>();
                objViewModel.BRIDGE_QM_CHILD_ON_WORKMENSHIP_FOUNDATION_LIST = new List<QMBridgeModel.EFORM_BRIDGE_QM_CHILD_ON_WORKMENSHIP_FOUNDATION>();
                objViewModel.BRIDGE_QM_COMPLETED_FOUNDATION = new QMBridgeModel.EFORM_BRIDGE_QM_COMPLETED_FOUNDATION();
                objViewModel.BRIDGE_QM_CHILD_COMPL_QOM_FOUNDATION_LIST = new List<QMBridgeModel.EFORM_BRIDGE_QM_CHILD_COMPL_QOM_FOUNDATION>();


                #endregion

                #region saurabh 15-22 mdoel
                objViewModel.BRIDGE_QM_SUBSTRUCTURE = new QMBridgeModel.EFORM_BRIDGE_QM_SUBSTRUCTURE();
                objViewModel.BRIDGE_QM_ONGOING_SUBSTRUCTURE = new QMBridgeModel.EFORM_BRIDGE_QM_ONGOING_SUBSTRUCTURE();
                objViewModel.BRIDGE_QM_COMPLETED_SUBSTRUCTURE = new QMBridgeModel.EFORM_BRIDGE_QM_COMPLETED_SUBSTRUCTURE();
                objViewModel.BRIDGE_QM_CHILD_ON_QOM_SUBSTRUCTURE_LIST = new List<QMBridgeModel.EFORM_BRIDGE_QM_CHILD_ON_QOM_SUBSTRUCTURE>();
                objViewModel.BRIDGE_QM_CHILD_ON_WORKMENSHIP_SUBSTRUCTURE_LIST = new List<QMBridgeModel.EFORM_BRIDGE_QM_CHILD_ON_WORKMENSHIP_SUBSTRUCTURE>();
                objViewModel.BRIDGE_QM_CHILD_COMPLETED_SUBSTRUCTURE_LIST = new List<QMBridgeModel.EFORM_BRIDGE_QM_CHILD_COMPLETED_SUBSTRUCTURE>();

                //superstructure
                objViewModel.BRIDGE_QM_SUPERSTRUCTURE = new QMBridgeModel.EFORM_BRIDGE_QM_SUPERSTRUCTURE();   // Parent Table

                objViewModel.BRIDGE_QM_CHILD_RCC_SUPERSTRUCTURE = new QMBridgeModel.EFORM_BRIDGE_QM_CHILD_RCC_SUPERSTRUCTURE_ONGOING();
                objViewModel.BRIDGE_QM_CHILD_STEEL_SUPERSTRUCTURE = new QMBridgeModel.EFORM_BRIDGE_QM_CHILD_STEEL_SUPERSTRUCTURE_ONGOING();
                objViewModel.BRIDGE_QM_CHILD_BAILEY_BRIDGE_SUPERSTRUCTURE = new QMBridgeModel.EFORM_BRIDGE_QM_CHILD_BAILEY_BRIDGE_SUPERSTRUCTURE_ONGOING();

                objViewModel.BRIDGE_QM_CHILD_RCC_SUPERSTRUCTURE_COMPLETED = new EFORM_BRIDGE_QM_CHILD_RCC_SUPERSTRUCTURE_COMPLETED();
                objViewModel.BRIDGE_QM_CHILD_STEEL_SUPERSTRUCTURE_COMPLETED = new EFORM_BRIDGE_QM_CHILD_STEEL_SUPERSTRUCTURE_COMPLETED();
                objViewModel.BRIDGE_QM_CHILD_BAILEY_BRIDGE_SUPERSTRUCTURE_COMPLETED = new EFORM_BRIDGE_QM_CHILD_BAILEY_BRIDGE_SUPERSTRUCTURE_COMPLETED();

                objViewModel.BRIDGE_QM_CHILD_ON_QOM_STEEL_SUPERSTRUCTURE_LIST = new List<QMBridgeModel.EFORM_BRIDGE_QM_CHILD_ON_QOM_STEEL_SUPERSTRUCTURE>();
                objViewModel.BRIDGE_QM_CHILD_CO_MATERIAL_STEEL_SUPERSTRUCTURE_LIST = new List<QMBridgeModel.EFORM_BRIDGE_QM_CHILD_CO_MATERIAL_STEEL_SUPERSTRUCTURE>();

                objViewModel.BRIDGE_QM_CHILD_ON_QOM_BAILEY_SUPERSTRUCTURE_LIST = new List<QMBridgeModel.EFORM_BRIDGE_QM_CHILD_ON_QOM_BAILEY_SUPERSTRUCTURE>();
                objViewModel.BRIDGE_QM_CHILD_CO_MATERIAL_BAILEY_SUPERSTRUCTURE_LIST = new List<QMBridgeModel.EFORM_BRIDGE_QM_CHILD_CO_MATERIAL_BAILEY_SUPERSTRUCTURE>();

                objViewModel.BRIDGE_QM_CHILD_ON_QOM_RCC_SUPERSTRUCTURE_LIST = new List<QMBridgeModel.EFORM_BRIDGE_QM_CHILD_ON_QOM_RCC_SUPERSTRUCTURE>();
                objViewModel.BRIDGE_QM_CHILD_COMPL_ACTIVITY_RCC_SUPERSTRUCTURE_LIST = new List<QMBridgeModel.EFORM_BRIDGE_QM_CHILD_COMPL_ACTIVITY_RCC_SUPERSTRUCTURE>();

                SubstructureQOMList substructureQOMList = new SubstructureQOMList();
                SubstructureWOMList substructureWOMList = new SubstructureWOMList();
                SubstrureCompletionList substrureCompletionList = new SubstrureCompletionList();

                // SuperStructure
                SteelQomSuperStructure steelQomSuperStructure = new SteelQomSuperStructure();
                SteelCompletedMaterialuperstructure steelCompletedMaterialuperstructure = new SteelCompletedMaterialuperstructure();
                RCCSuperStructure rCCSuperStructure = new RCCSuperStructure();
                RCCCompletedSuperStructure rCCCompletedSuperStructure = new RCCCompletedSuperStructure();
                BaileySuperstructure baileySuperstructure = new BaileySuperstructure();
                BaileyCompleteStructure baileyCompleteStructure = new BaileyCompleteStructure();
                #endregion

                #region vikky model page 23-28
                ApproachEmbarkList approachEmbarkList = new ApproachEmbarkList();
                ApproachSubBaseList approachSubBaseList = new ApproachSubBaseList();
                ApproachBaseCourseList approachBaseCourseList = new ApproachBaseCourseList();
                ApproachWearingCourseList approachWearingCourseList = new ApproachWearingCourseList();
                ApproachProtQOMList approachProtQOMList = new ApproachProtQOMList();
                ApproachProtWorkmanshipList approachProtWorkmanshipList = new ApproachProtWorkmanshipList();


                objViewModel.BRIDGE_QM_LOAD_TEST = new QMBridgeModel.EFORM_BRIDGE_QM_LOAD_TEST();
                objViewModel.BRIDGE_QM_BEARING = new QMBridgeModel.EFORM_BRIDGE_QM_BEARING();
                objViewModel.BRIDGE_QM_CHILD_BEARING_TYPE = new QMBridgeModel.EFORM_BRIDGE_QM_CHILD_BEARING_TYPE();
                objViewModel.BRIDGE_QM_EXPANSION = new QMBridgeModel.EFORM_BRIDGE_QM_EXPANSION();
                objViewModel.BRIDGE_QM_APPROACH = new QMBridgeModel.EFORM_BRIDGE_QM_APPROACH();
                objViewModel.BRIDGE_QM_CHILD_EMBANKMENT_APPROACH_LIST = new List<QMBridgeModel.EFORM_BRIDGE_QM_CHILD_EMBANKMENT_APPROACH>();
                objViewModel.BRIDGE_QM_CHILD_SUB_BASE_APPROACH_LIST = new List<QMBridgeModel.EFORM_BRIDGE_QM_CHILD_SUB_BASE_APPROACH>();
                objViewModel.BRIDGE_QM_CHILD_BASE_COURSE_APPROACH_LIST = new List<QMBridgeModel.EFORM_BRIDGE_QM_CHILD_BASE_COURSE_APPROACH>();
                objViewModel.BRIDGE_QM_CHILD_WEARING_COURSE_APPROACH_LIST = new List<QMBridgeModel.EFORM_BRIDGE_QM_CHILD_WEARING_COURSE_APPROACH>();

                objViewModel.BRIDGE_QM_CHILD_PROTECH_APPROACH = new QMBridgeModel.EFORM_BRIDGE_QM_CHILD_PROTECH_APPROACH();
                objViewModel.BRIDGE_QM_CHILD_QOM_APPROACH_LIST = new List<QMBridgeModel.EFORM_BRIDGE_QM_CHILD_QOM_APPROACH>();
                objViewModel.BRIDGE_QM_CHILD_WORKMENSHIP_APPROACH_LIST = new List<QMBridgeModel.EFORM_BRIDGE_QM_CHILD_WORKMENSHIP_APPROACH>();
                #endregion


                #region Bhushan model page 29-35
                objViewModel.BRIDGE_QM_FURNITURE_MARKINGS = new QMBridgeModel.EFORM_BRIDGE_QM_FURNITURE_MARKINGS();
                objViewModel.BRIDGE_QM_CHILD_OBS_FURNITURE_MARKING = new QMBridgeModel.EFORM_BRIDGE_QM_CHILD_OBS_FURNITURE_MARKING();
                objViewModel.BRIDGE_QM_DEFICIENCY = new QMBridgeModel.EFORM_BRIDGE_QM_DEFICIENCY();
                objViewModel.BRIDGE_QM_WORK_COMPLETED_IN_PROGRESS_ASPER_PROG = new QMBridgeModel.EFORM_BRIDGE_QM_WORK_COMPLETED_IN_PROGRESS_ASPER_PROG();
                objViewModel.BRIDGE_QM_WORK_COMPLETED_IN_PROGRESS_ASPER_COST = new QMBridgeModel.EFORM_BRIDGE_QM_WORK_COMPLETED_IN_PROGRESS_ASPER_COST();

                ActionTakenByPIUModelDetails actionTakenByPIUModelDetails = new ActionTakenByPIUModelDetails();
                objViewModel.BRIDGE_QM_ACTION_TAKEN_PIU_LIST = new List<QMBridgeModel.EFORM_BRIDGE_QM_ACTION_TAKEN_PIU>();

                objViewModel.BRIDGE_QM_DIFFERENCE_IN_OBSERVATION = new QMBridgeModel.EFORM_BRIDGE_QM_DIFFERENCE_IN_OBSERVATION();
                objViewModel.BRIDGE_QM_OVERALL_GRADING = new QMBridgeModel.EFORM_BRIDGE_QM_OVERALL_GRADING();
                objViewModel.BRIDGE_QM_QUALITY_GRADING = new QMBridgeModel.EFORM_BRIDGE_QM_QUALITY_GRADING();
                #endregion

                objViewModel.ErrorList = new List<string>();

                List<string> errorListtemp = new List<string>();


                #region Vikky page 09-15 reading

                #region EFORM_BRIDGE_QM_GENERAL_DETAILS
                errorListtemp = objCommonFunction.FetchBridgeQMPDFFilledDataToModel(objViewModel.BRIDGE_QM_GENERAL_DETAILS, loadedForm);
                objViewModel.ErrorList.AddRange(errorListtemp);
                if (objViewModel.BRIDGE_QM_GENERAL_DETAILS.IS_WORK_STAT_LAYOUT == "N" && objViewModel.BRIDGE_QM_GENERAL_DETAILS.IS_WORK_STAT_FOUNDATION == "N" && objViewModel.BRIDGE_QM_GENERAL_DETAILS.IS_WORK_STAT_SUBSTRUCTURE == "N" && objViewModel.BRIDGE_QM_GENERAL_DETAILS.IS_WORK_STAT_SUPERSTRUCTURE == "N" && objViewModel.BRIDGE_QM_GENERAL_DETAILS.IS_WORK_STAT_PROT_WORK == "N" && objViewModel.BRIDGE_QM_GENERAL_DETAILS.IS_WORK_STAT_APPROACH == "N" && objViewModel.BRIDGE_QM_GENERAL_DETAILS.IS_WORK_STAT_FINISHING_STAGE == "N")
                {
                    errorListtemp.Clear();
                    errorListtemp.Add("Page-9: Item 1. GENERAL:- VII. Please select atleast one present status of work");
                    objViewModel.ErrorList.AddRange(errorListtemp);
                    errorListtemp.Clear();
                    objViewModel.ErrorOccured = true;
                }
                #endregion


                #region EFORM_BRIDGE_QM_ARRANGEMENT_OBS_DETAIL
                if (TemplateStatus != true)
                {
                    if (chckBoxArr[2] == 'Y')
                    {
                        errorListtemp = objCommonFunction.FetchBridgeQMPDFFilledDataToModel(objViewModel.BRIDGE_QM_ARRANGEMENT_OBS_DETAIL, loadedForm);
                        objViewModel.ErrorList.AddRange(errorListtemp);
                    }

                }

                #endregion

                #region EFORM_BRIDGE_QM_QUALITY_ATTENTION
                if (TemplateStatus != true)
                {
                    if (chckBoxArr[3] == 'Y')
                    {
                        errorListtemp = objCommonFunction.FetchBridgeQMPDFFilledDataToModel(objViewModel.BRIDGE_QM_QUALITY_ATTENTION, loadedForm);
                        objViewModel.ErrorList.AddRange(errorListtemp);


                        #region  BRIDGE_QM_QC_TEST_DETAILS_LIST

                        int count = 0;
                        string testingAdequateStatus = "Y";
                        for (int j = 0; j < 3; j++)
                        {
                            for (int i = 1; i <= 5; i++)
                            {
                                int t = 1;
                                string chkBxName = "cb_8_1_" + i;
                                string testingAdequate = string.Empty;

                                if (j == 0)
                                {
                                    if (i >= 5)
                                    {
                                        testingAdequate = "IS_TESTING_ADEQUATE_" + 2 + "" + (i - 5);
                                    }
                                    else
                                    {
                                        testingAdequate = "IS_TESTING_ADEQUATE_" + t + "" + (i + 5);
                                    }
                                }
                                else
                                {
                                    if (i >= 5)
                                    {
                                        testingAdequate = "IS_TESTING_ADEQUATE_" + j + "_" + 2 + "" + (i - 5);
                                    }
                                    else
                                    {
                                        testingAdequate = "IS_TESTING_ADEQUATE_" + j + "_" + t + "" + (i + 5);
                                    }
                                }


                                PdfLoadedRadioButtonListField radBtn = loadedForm.Fields[testingAdequate] as PdfLoadedRadioButtonListField;
                                if (j == 0)
                                {
                                    PdfLoadedCheckBoxField chkBx = loadedForm.Fields[chkBxName] as PdfLoadedCheckBoxField;
                                    if (!chkBx.Checked)
                                    {
                                        count++;
                                    }
                                }

                                if (radBtn.SelectedValue == "N")
                                {
                                    testingAdequateStatus = "N";
                                }
                            }
                        }
                        if (testingAdequateStatus == "N")
                        {
                            PdfLoadedCheckBoxField IS_NEGLIGENCE_CHECKED = loadedForm.Fields["IS_NEGLIGENCE"] as PdfLoadedCheckBoxField;
                            PdfLoadedCheckBoxField IS_LOE_CHECKED = loadedForm.Fields["IS_LOE"] as PdfLoadedCheckBoxField;
                            PdfLoadedCheckBoxField IS_LOK_CHECKED = loadedForm.Fields["IS_LOK"] as PdfLoadedCheckBoxField;
                            PdfLoadedCheckBoxField IS_OTHER_CHECKED = loadedForm.Fields["IS_OTHER"] as PdfLoadedCheckBoxField;
                            if (IS_NEGLIGENCE_CHECKED.Checked == false && IS_LOE_CHECKED.Checked == false && IS_LOK_CHECKED.Checked == false && IS_OTHER_CHECKED.Checked == false)
                            {
                                errorListtemp.Clear();
                                errorListtemp.Add("Page-11: Item 3. ATTENTION TO QUALITY- II. b) Please select atleast one reason for less testing");
                                objViewModel.ErrorList.AddRange(errorListtemp);

                            }


                        }
                        if (count == 6)
                        {
                            errorListtemp.Clear();
                            errorListtemp.Add("Page-11: Item 3. ATTENTION TO QUALITY- II. a) Adequacy of quality control tests, as per QCR-1: Please fill Adequacy of quality control tests, as per QCR-1 details atleast for one row");
                            objViewModel.ErrorList.AddRange(errorListtemp);
                            errorListtemp.Clear();
                            objViewModel.ErrorOccured = true;
                        }
                        else
                        {
                            dbContext.EFORM_BRIDGE_MASTER_WORK_ITEM.Where(m => m.ITEM_FLAG == "A").Select(t => t.ITEM_ID).ToList().ForEach(itemID =>
                            {
                                for (int i = 1; i < 4; i++)
                                {
                                    QMBridgeModel.EFORM_BRIDGE_QM_QC_TEST_DETAILS detailedItem = new QMBridgeModel.EFORM_BRIDGE_QM_QC_TEST_DETAILS();
                                    detailedItem.ITEM_ID = itemID;
                                    detailedItem.ROW_ID = i;
                                    objViewModel.BRIDGE_QM_QC_TEST_DETAILS_LIST.Add(detailedItem);
                                }

                            });

                            int countTemp = 1;
                            foreach (var item in objViewModel.BRIDGE_QM_QC_TEST_DETAILS_LIST.ToList())
                            {

                                string v = "cb_8_1_" + countTemp;
                                PdfLoadedCheckBoxField CheckField = loadedForm.Fields[v] as PdfLoadedCheckBoxField;
                                if (CheckField.Checked)
                                {
                                    errorListtemp = objCommonFunction.FetchBridgeQMPDFFilledDataToModel(item, loadedForm);
                                    objViewModel.ErrorList.AddRange(errorListtemp);

                                    if ((item.ROW_ID == 2 || item.ROW_ID == 3) && item.IS_TESTING_ADEQUATE == null && item.REQD_TEST_NUMBER == null && item.TEST_NAME == null && item.CONDUCTED_TEST_NUMBER == null)
                                    {
                                        objViewModel.BRIDGE_QM_QC_TEST_DETAILS_LIST.Remove(item);
                                    }

                                }
                                else
                                {
                                    objViewModel.BRIDGE_QM_QC_TEST_DETAILS_LIST.Remove(item);
                                }
                                if (item.ROW_ID == 3)
                                {
                                    countTemp++;
                                }

                            }
                            if (objViewModel.BRIDGE_QM_QC_TEST_DETAILS_LIST.Count > 2)
                            {
                                for (int i = 1; i < objViewModel.BRIDGE_QM_QC_TEST_DETAILS_LIST.Count; i++)
                                {
                                    if (objViewModel.BRIDGE_QM_QC_TEST_DETAILS_LIST[i].EXECUTED_QUANTITY == null)
                                    {
                                        objViewModel.BRIDGE_QM_QC_TEST_DETAILS_LIST[i].EXECUTED_QUANTITY = objViewModel.BRIDGE_QM_QC_TEST_DETAILS_LIST[i - 1].EXECUTED_QUANTITY;
                                    }
                                    if (objViewModel.BRIDGE_QM_QC_TEST_DETAILS_LIST[i].DPR_QUANTITY == null)
                                    {
                                        objViewModel.BRIDGE_QM_QC_TEST_DETAILS_LIST[i].DPR_QUANTITY = objViewModel.BRIDGE_QM_QC_TEST_DETAILS_LIST[i - 1].DPR_QUANTITY;
                                    }
                                }
                            }

                        }

                        #endregion



                        #region EFORM_BRIDGE_QM_TEST_RESULT_VERIFICATION_DETAILS
                        if (TemplateStatus != true)
                        {

                            verificationModelList.VerificationList.ForEach(item =>
                            {

                                for (int i = 1; i <= item.RowCount; i++)
                                {
                                    QMBridgeModel.EFORM_BRIDGE_QM_TEST_RESULT_VERIFICATION_DETAILS detailedItem = new QMBridgeModel.EFORM_BRIDGE_QM_TEST_RESULT_VERIFICATION_DETAILS();
                                    detailedItem.VerificationType = item.VerificationType;
                                    detailedItem.RowID = i;
                                    objViewModel.BRIDGE_QM_TEST_RESULT_VERIFICATION_DETAILS_LIST.Add(detailedItem);
                                }

                            });
                            int countChecked = 0;
                            int countUnchecked = 0;
                            foreach (var item in objViewModel.BRIDGE_QM_TEST_RESULT_VERIFICATION_DETAILS_LIST.ToList())
                            //  objViewModel.QM_TEST_RESULT_VERIFICATION_DETAILS_List.ForEach(item =>
                            {
                                if (item.RowID == 1)
                                {
                                    errorListtemp = objCommonFunction.FetchBridgeQMPDFFilledDataToModel(item, loadedForm);
                                    objViewModel.ErrorList.AddRange(errorListtemp);
                                }
                                else
                                {

                                    string v = "cbrd_3_2_" + item.RowID;
                                    PdfLoadedCheckBoxField CheckField = loadedForm.Fields[v] as PdfLoadedCheckBoxField;

                                    if (CheckField.Checked)
                                    {
                                        countChecked++;
                                        errorListtemp = objCommonFunction.FetchBridgeQMPDFFilledDataToModel(item, loadedForm);
                                        objViewModel.ErrorList.AddRange(errorListtemp);
                                        if (item.RowID == 2 || item.RowID == 3 || item.RowID == 4)
                                        {
                                            objViewModel.BRIDGE_QM_TEST_RESULT_VERIFICATION_DETAILS_LIST[countChecked].ROAD_LOC = objViewModel.BRIDGE_QM_TEST_RESULT_VERIFICATION_DETAILS_LIST[0].ROAD_LOC;
                                        }
                                        else if (item.RowID == 6 || item.RowID == 7 || item.RowID == 8)
                                        {
                                            objViewModel.BRIDGE_QM_TEST_RESULT_VERIFICATION_DETAILS_LIST[countChecked].ROAD_LOC = objViewModel.BRIDGE_QM_TEST_RESULT_VERIFICATION_DETAILS_LIST[4 - countUnchecked].ROAD_LOC;
                                        }
                                        else if (item.RowID == 10 || item.RowID == 11 || item.RowID == 12)
                                        {
                                            objViewModel.BRIDGE_QM_TEST_RESULT_VERIFICATION_DETAILS_LIST[countChecked].ROAD_LOC = objViewModel.BRIDGE_QM_TEST_RESULT_VERIFICATION_DETAILS_LIST[8 - countUnchecked].ROAD_LOC;
                                        }
                                    }
                                    else
                                    {
                                        countUnchecked++;
                                        objViewModel.BRIDGE_QM_TEST_RESULT_VERIFICATION_DETAILS_LIST.Remove(item);
                                    }
                                }


                            }





                        }

                        #endregion
                    }

                }


                #endregion


                if (chckBoxArr[4] == 'Y')
                {


                    #region EFORM_BRIDGE_QM_FOUNDATION
                    errorListtemp = objCommonFunction.FetchBridgeQMPDFFilledDataToModel(objViewModel.BRIDGE_QM_FOUNDATION, loadedForm);
                    objViewModel.ErrorList.AddRange(errorListtemp);
                    #endregion


                    if (TemplateStatus != true)
                    {
                        #region EFORM_BRIDGE_QM_ONGOING_FOUNDATION
                        errorListtemp = objCommonFunction.FetchBridgeQMPDFFilledDataToModel(objViewModel.BRIDGE_QM_ONGOING_FOUNDATION, loadedForm);
                        objViewModel.ErrorList.AddRange(errorListtemp);
                        #endregion

                        #region EFORM_BRIDGE_QM_CHILD_ON_QOM_FOUNDATION
                        onQomFoundationList.OnQomFoundationDetailsList.ForEach(item =>
                        {

                            for (int i = 1; i <= item.RowCount; i++)
                            {
                                QMBridgeModel.EFORM_BRIDGE_QM_CHILD_ON_QOM_FOUNDATION detailedItem = new QMBridgeModel.EFORM_BRIDGE_QM_CHILD_ON_QOM_FOUNDATION();

                                detailedItem.ROW_ID = i;
                                objViewModel.BRIDGE_QM_CHILD_ON_QOM_FOUNDATION_LIST.Add(detailedItem);
                            }

                        });



                        foreach (var item in objViewModel.BRIDGE_QM_CHILD_ON_QOM_FOUNDATION_LIST.ToList())
                        {
                            if (item.ROW_ID == 1)
                            {
                                errorListtemp = objCommonFunction.FetchBridgeQMPDFFilledDataToModel(item, loadedForm);
                                objViewModel.ErrorList.AddRange(errorListtemp);
                            }
                            else
                            {
                                string v = "cb_4_1_" + item.ROW_ID;
                                PdfLoadedCheckBoxField CheckField = loadedForm.Fields[v] as PdfLoadedCheckBoxField;
                                if (CheckField.Checked)
                                {
                                    errorListtemp = objCommonFunction.FetchBridgeQMPDFFilledDataToModel(item, loadedForm);
                                    objViewModel.ErrorList.AddRange(errorListtemp);
                                }
                                else
                                {
                                    objViewModel.BRIDGE_QM_CHILD_ON_QOM_FOUNDATION_LIST.Remove(item);
                                }
                            }
                        }
                        #endregion


                        #region EFORM_BRIDGE_QM_CHILD_ON_WORKMENSHIP_FOUNDATION
                        onWorkmanshipFoundationList.OnWorkmanshipDetailsList.ForEach(item =>
                        {

                            for (int i = 1; i <= item.RowCount; i++)
                            {
                                QMBridgeModel.EFORM_BRIDGE_QM_CHILD_ON_WORKMENSHIP_FOUNDATION detailedItem = new QMBridgeModel.EFORM_BRIDGE_QM_CHILD_ON_WORKMENSHIP_FOUNDATION();

                                detailedItem.ROW_ID = i;
                                objViewModel.BRIDGE_QM_CHILD_ON_WORKMENSHIP_FOUNDATION_LIST.Add(detailedItem);
                            }

                        });



                        foreach (var item in objViewModel.BRIDGE_QM_CHILD_ON_WORKMENSHIP_FOUNDATION_LIST.ToList())
                        {
                            if (item.ROW_ID == 1)
                            {
                                errorListtemp = objCommonFunction.FetchBridgeQMPDFFilledDataToModel(item, loadedForm);
                                objViewModel.ErrorList.AddRange(errorListtemp);
                            }
                            else
                            {
                                string v = "cb_4_2_" + item.ROW_ID;
                                PdfLoadedCheckBoxField CheckField = loadedForm.Fields[v] as PdfLoadedCheckBoxField;
                                if (CheckField.Checked)
                                {
                                    errorListtemp = objCommonFunction.FetchBridgeQMPDFFilledDataToModel(item, loadedForm);
                                    objViewModel.ErrorList.AddRange(errorListtemp);
                                }
                                else
                                {
                                    objViewModel.BRIDGE_QM_CHILD_ON_WORKMENSHIP_FOUNDATION_LIST.Remove(item);
                                }
                            }
                        }
                        #endregion

                    }
                    else
                    {
                        #region EFORM_BRIDGE_QM_COMPLETED_FOUNDATION
                        errorListtemp = objCommonFunction.FetchBridgeQMPDFFilledDataToModel(objViewModel.BRIDGE_QM_COMPLETED_FOUNDATION, loadedForm);
                        objViewModel.ErrorList.AddRange(errorListtemp);
                        #endregion

                        #region EFORM_BRIDGE_QM_CHILD_COMPL_QOM_FOUNDATION
                        completedQomFoundationList.CompletedQomFoundationDetailsList.ForEach(item =>
                        {

                            for (int i = 1; i <= item.RowCount; i++)
                            {
                                QMBridgeModel.EFORM_BRIDGE_QM_CHILD_COMPL_QOM_FOUNDATION detailedItem = new QMBridgeModel.EFORM_BRIDGE_QM_CHILD_COMPL_QOM_FOUNDATION();

                                detailedItem.ROW_ID = i;
                                objViewModel.BRIDGE_QM_CHILD_COMPL_QOM_FOUNDATION_LIST.Add(detailedItem);
                            }

                        });



                        foreach (var item in objViewModel.BRIDGE_QM_CHILD_COMPL_QOM_FOUNDATION_LIST.ToList())
                        {
                            if (item.ROW_ID == 1)
                            {
                                errorListtemp = objCommonFunction.FetchBridgeQMPDFFilledDataToModel(item, loadedForm);
                                objViewModel.ErrorList.AddRange(errorListtemp);
                            }
                            else
                            {
                                string v = "cb_4_" + item.ROW_ID;
                                PdfLoadedCheckBoxField CheckField = loadedForm.Fields[v] as PdfLoadedCheckBoxField;
                                if (CheckField.Checked)
                                {
                                    errorListtemp = objCommonFunction.FetchBridgeQMPDFFilledDataToModel(item, loadedForm);
                                    objViewModel.ErrorList.AddRange(errorListtemp);
                                }
                                else
                                {
                                    objViewModel.BRIDGE_QM_CHILD_COMPL_QOM_FOUNDATION_LIST.Remove(item);
                                }
                            }
                        }
                        #endregion
                    }





                }
                #endregion

                #region saurabh 15-22 reading

                PdfLoadedRadioButtonListField StructureType = loadedForm.Fields["STRUCTURE_TYPE"] as PdfLoadedRadioButtonListField;

                if (chckBoxArr[5] == 'Y')  // Substructure DATA Reading
                {
                    #region   EFORM_BRIDGE_QM_SUBSTRUCTURE

                    errorListtemp = objCommonFunction.FetchBridgeQMPDFFilledDataToModel(objViewModel.BRIDGE_QM_SUBSTRUCTURE, loadedForm);
                    objViewModel.ErrorList.AddRange(errorListtemp);
                    #endregion

                    #region   EFORM_BRIDGE_QM_ONGOING_SUBSTRUCTURE
                    if (TemplateStatus != true) // TemplateStatus != true means in P
                    {
                        errorListtemp = objCommonFunction.FetchBridgeQMPDFFilledDataToModel(objViewModel.BRIDGE_QM_ONGOING_SUBSTRUCTURE, loadedForm);
                        objViewModel.ErrorList.AddRange(errorListtemp);
                    }

                    #endregion

                    #region EFORM_BRIDGE_QM_COMPLETED_SUBSTRUCTURE
                    if (TemplateStatus == true) // TemplateStatus != true means in P
                    {
                        errorListtemp = objCommonFunction.FetchBridgeQMPDFFilledDataToModel(objViewModel.BRIDGE_QM_COMPLETED_SUBSTRUCTURE, loadedForm);
                        objViewModel.ErrorList.AddRange(errorListtemp);
                    }
                    #endregion

                    #region   EFORM_BRIDGE_QM_CHILD_ON_QOM_SUBSTRUCTURE
                    if (TemplateStatus != true) // TemplateStatus != true means in P
                    {

                        substructureQOMList.QOMSubstruCtureList.ForEach(item =>
                        {
                            for (int i = 1; i <= item.RowCount; i++)
                            {
                                QMBridgeModel.EFORM_BRIDGE_QM_CHILD_ON_QOM_SUBSTRUCTURE detailedItem = new QMBridgeModel.EFORM_BRIDGE_QM_CHILD_ON_QOM_SUBSTRUCTURE();
                                detailedItem.SubstructureType = item.SubstructureType;
                                detailedItem.RowID = i;
                                objViewModel.BRIDGE_QM_CHILD_ON_QOM_SUBSTRUCTURE_LIST.Add(detailedItem);
                            }
                        });

                        foreach (var item in objViewModel.BRIDGE_QM_CHILD_ON_QOM_SUBSTRUCTURE_LIST.ToList())
                        {
                            if (item.RowID == 1)
                            {
                                errorListtemp = objCommonFunction.FetchBridgeQMPDFFilledDataToModel(item, loadedForm);
                                objViewModel.ErrorList.AddRange(errorListtemp);
                            }
                            else
                            {
                                string v = "cb_5_1_" + item.RowID;
                                PdfLoadedCheckBoxField CheckField = loadedForm.Fields[v] as PdfLoadedCheckBoxField;
                                if (CheckField.Checked)
                                {
                                    errorListtemp = objCommonFunction.FetchBridgeQMPDFFilledDataToModel(item, loadedForm);
                                    objViewModel.ErrorList.AddRange(errorListtemp);
                                }
                                else
                                {
                                    objViewModel.BRIDGE_QM_CHILD_ON_QOM_SUBSTRUCTURE_LIST.Remove(item);
                                }
                            }


                        }

                    }

                    #endregion

                    #region   EFORM_BRIDGE_QM_CHILD_ON_WORKMENSHIP_SUBSTRUCTURE
                    if (TemplateStatus != true) // TemplateStatus != true means in P temp
                    {
                        substructureWOMList.WOMSubstruCtureList.ForEach(item =>
                        {
                            for (int i = 1; i <= item.RowCount; i++)
                            {
                                QMBridgeModel.EFORM_BRIDGE_QM_CHILD_ON_WORKMENSHIP_SUBSTRUCTURE detailedItem = new QMBridgeModel.EFORM_BRIDGE_QM_CHILD_ON_WORKMENSHIP_SUBSTRUCTURE();
                                detailedItem.SubstructureType = item.SubstructureType;
                                detailedItem.RowID = i;
                                objViewModel.BRIDGE_QM_CHILD_ON_WORKMENSHIP_SUBSTRUCTURE_LIST.Add(detailedItem);
                            }
                        });

                        foreach (var item in objViewModel.BRIDGE_QM_CHILD_ON_WORKMENSHIP_SUBSTRUCTURE_LIST.ToList())
                        {
                            if (item.RowID == 1)
                            {
                                errorListtemp = objCommonFunction.FetchBridgeQMPDFFilledDataToModel(item, loadedForm);
                                objViewModel.ErrorList.AddRange(errorListtemp);
                            }
                            else
                            {
                                string v = "cb_5_2_" + item.RowID;
                                PdfLoadedCheckBoxField CheckField = loadedForm.Fields[v] as PdfLoadedCheckBoxField;
                                if (CheckField.Checked)
                                {
                                    errorListtemp = objCommonFunction.FetchBridgeQMPDFFilledDataToModel(item, loadedForm);
                                    objViewModel.ErrorList.AddRange(errorListtemp);
                                }
                                else
                                {
                                    objViewModel.BRIDGE_QM_CHILD_ON_WORKMENSHIP_SUBSTRUCTURE_LIST.Remove(item);
                                }
                            }
                        }
                    }

                    #endregion

                    #region EFORM_BRIDGE_QM_CHILD_COMPLETED_SUBSTRUCTURE
                    if (TemplateStatus == true)
                    {
                        substrureCompletionList.SubstruCtureCompletedList.ForEach(item =>
                        {
                            for (int i = 1; i <= item.RowCount; i++)
                            {
                                QMBridgeModel.EFORM_BRIDGE_QM_CHILD_COMPLETED_SUBSTRUCTURE detailedItem = new QMBridgeModel.EFORM_BRIDGE_QM_CHILD_COMPLETED_SUBSTRUCTURE();
                                detailedItem.SubstructureType = item.SubstructureType;
                                detailedItem.RowID = i;
                                objViewModel.BRIDGE_QM_CHILD_COMPLETED_SUBSTRUCTURE_LIST.Add(detailedItem);
                            }
                        });

                        foreach (var item in objViewModel.BRIDGE_QM_CHILD_COMPLETED_SUBSTRUCTURE_LIST.ToList())
                        {
                            if (item.RowID == 1)
                            {
                                errorListtemp = objCommonFunction.FetchBridgeQMPDFFilledDataToModel(item, loadedForm);
                                objViewModel.ErrorList.AddRange(errorListtemp);
                            }
                            else
                            {
                                string v = "cb_5_" + item.RowID;
                                PdfLoadedCheckBoxField CheckField = loadedForm.Fields[v] as PdfLoadedCheckBoxField;
                                if (CheckField.Checked)
                                {
                                    errorListtemp = objCommonFunction.FetchBridgeQMPDFFilledDataToModel(item, loadedForm);
                                    objViewModel.ErrorList.AddRange(errorListtemp);
                                }
                                else
                                {
                                    objViewModel.BRIDGE_QM_CHILD_COMPLETED_SUBSTRUCTURE_LIST.Remove(item);
                                }
                            }
                        }
                    }

                    #endregion

                }

                if (chckBoxArr[6] == 'Y')  // Super-Structure DATA Reading
                {

                    #region EFORM_BRIDGE_QM_SUPERSTRUCTURE

                    errorListtemp = objCommonFunction.FetchBridgeQMPDFFilledDataToModel(objViewModel.BRIDGE_QM_SUPERSTRUCTURE, loadedForm);
                    objViewModel.ErrorList.AddRange(errorListtemp);
                    #endregion

                    #region  STEEL_SUPERSTRUCTURE

                    if (StructureType.SelectedValue == "STEEL")
                    {

                        if (TemplateStatus != true)  //  TemplateStatus != true means in P
                        {
                            errorListtemp = objCommonFunction.FetchBridgeQMPDFFilledDataToModel(objViewModel.BRIDGE_QM_CHILD_STEEL_SUPERSTRUCTURE, loadedForm);
                            objViewModel.ErrorList.AddRange(errorListtemp);

                            steelQomSuperStructure.QomSuperStructureList.ForEach(item =>
                            {
                                for (int i = 1; i <= item.RowCount; i++)
                                {
                                    QMBridgeModel.EFORM_BRIDGE_QM_CHILD_ON_QOM_STEEL_SUPERSTRUCTURE detailedItem = new QMBridgeModel.EFORM_BRIDGE_QM_CHILD_ON_QOM_STEEL_SUPERSTRUCTURE();
                                    detailedItem.SuperStructureType = item.SuperStructureType;
                                    detailedItem.RowID = i;
                                    objViewModel.BRIDGE_QM_CHILD_ON_QOM_STEEL_SUPERSTRUCTURE_LIST.Add(detailedItem);

                                }
                            });

                            foreach (var item in objViewModel.BRIDGE_QM_CHILD_ON_QOM_STEEL_SUPERSTRUCTURE_LIST.ToList())
                            {
                                if (item.RowID == 1)
                                {
                                    errorListtemp = objCommonFunction.FetchBridgeQMPDFFilledDataToModel(item, loadedForm);
                                    objViewModel.ErrorList.AddRange(errorListtemp);
                                }
                                else
                                {
                                    string v = "cb_6_2_" + item.RowID;
                                    PdfLoadedCheckBoxField CheckField = loadedForm.Fields[v] as PdfLoadedCheckBoxField;
                                    if (CheckField.Checked)
                                    {
                                        errorListtemp = objCommonFunction.FetchBridgeQMPDFFilledDataToModel(item, loadedForm);
                                        objViewModel.ErrorList.AddRange(errorListtemp);
                                    }
                                    else
                                    {
                                        objViewModel.BRIDGE_QM_CHILD_ON_QOM_STEEL_SUPERSTRUCTURE_LIST.Remove(item);
                                    }
                                }
                            }
                        }
                        else  // means road is completed
                        {
                            errorListtemp = objCommonFunction.FetchBridgeQMPDFFilledDataToModel(objViewModel.BRIDGE_QM_CHILD_STEEL_SUPERSTRUCTURE_COMPLETED, loadedForm);
                            objViewModel.ErrorList.AddRange(errorListtemp);

                            steelCompletedMaterialuperstructure.MaterialCompleteSuperStructureList.ForEach(item =>
                            {
                                for (int i = 1; i <= item.RowCount; i++)
                                {
                                    QMBridgeModel.EFORM_BRIDGE_QM_CHILD_CO_MATERIAL_STEEL_SUPERSTRUCTURE detailedItem = new QMBridgeModel.EFORM_BRIDGE_QM_CHILD_CO_MATERIAL_STEEL_SUPERSTRUCTURE();
                                    detailedItem.SuperStructureType = item.SuperStructureType;
                                    detailedItem.RowID = i;
                                    objViewModel.BRIDGE_QM_CHILD_CO_MATERIAL_STEEL_SUPERSTRUCTURE_LIST.Add(detailedItem);
                                }
                            });

                            foreach (var item in objViewModel.BRIDGE_QM_CHILD_CO_MATERIAL_STEEL_SUPERSTRUCTURE_LIST.ToList())
                            {
                                if (item.RowID == 1)
                                {
                                    errorListtemp = objCommonFunction.FetchBridgeQMPDFFilledDataToModel(item, loadedForm);
                                    objViewModel.ErrorList.AddRange(errorListtemp);
                                }
                                else
                                {
                                    string v = "cb_20_" + item.RowID;
                                    PdfLoadedCheckBoxField CheckField = loadedForm.Fields[v] as PdfLoadedCheckBoxField;
                                    if (CheckField.Checked)
                                    {
                                        errorListtemp = objCommonFunction.FetchBridgeQMPDFFilledDataToModel(item, loadedForm);
                                        objViewModel.ErrorList.AddRange(errorListtemp);
                                    }
                                    else
                                    {
                                        objViewModel.BRIDGE_QM_CHILD_CO_MATERIAL_STEEL_SUPERSTRUCTURE_LIST.Remove(item);
                                    }
                                }
                            }
                        }
                    }

                    #endregion

                    #region RCC_SUPERSTRUCTURE

                    if (StructureType.SelectedValue == "RCC")
                    {

                        if (TemplateStatus != true)  //  TemplateStatus != true means in P
                        {

                            errorListtemp = objCommonFunction.FetchBridgeQMPDFFilledDataToModel(objViewModel.BRIDGE_QM_CHILD_RCC_SUPERSTRUCTURE, loadedForm);
                            objViewModel.ErrorList.AddRange(errorListtemp);

                            rCCSuperStructure.QomRCCStructureList.ForEach(item =>
                            {
                                for (int i = 1; i <= item.RowCount; i++)
                                {
                                    QMBridgeModel.EFORM_BRIDGE_QM_CHILD_ON_QOM_RCC_SUPERSTRUCTURE detailedItem = new QMBridgeModel.EFORM_BRIDGE_QM_CHILD_ON_QOM_RCC_SUPERSTRUCTURE();
                                    detailedItem.SuperStructureType = item.SuperStructureType;
                                    detailedItem.RowID = i;
                                    objViewModel.BRIDGE_QM_CHILD_ON_QOM_RCC_SUPERSTRUCTURE_LIST.Add(detailedItem);

                                }

                            });

                            foreach (var item in objViewModel.BRIDGE_QM_CHILD_ON_QOM_RCC_SUPERSTRUCTURE_LIST.ToList())
                            {
                                if (item.RowID == 1)
                                {
                                    errorListtemp = objCommonFunction.FetchBridgeQMPDFFilledDataToModel(item, loadedForm);
                                    objViewModel.ErrorList.AddRange(errorListtemp);
                                }
                                else
                                {
                                    string v = "cb_6_1_" + item.RowID;
                                    PdfLoadedCheckBoxField CheckField = loadedForm.Fields[v] as PdfLoadedCheckBoxField;
                                    if (CheckField.Checked)
                                    {
                                        errorListtemp = objCommonFunction.FetchBridgeQMPDFFilledDataToModel(item, loadedForm);
                                        objViewModel.ErrorList.AddRange(errorListtemp);
                                    }
                                    else
                                    {
                                        objViewModel.BRIDGE_QM_CHILD_ON_QOM_RCC_SUPERSTRUCTURE_LIST.Remove(item);
                                    }
                                }
                            }

                        }
                        else
                        {
                            errorListtemp = objCommonFunction.FetchBridgeQMPDFFilledDataToModel(objViewModel.BRIDGE_QM_CHILD_RCC_SUPERSTRUCTURE_COMPLETED, loadedForm);
                            objViewModel.ErrorList.AddRange(errorListtemp);


                            rCCCompletedSuperStructure.RCCCompletedStructureList.ForEach(item =>
                            {
                                for (int i = 1; i <= item.RowCount; i++)
                                {
                                    QMBridgeModel.EFORM_BRIDGE_QM_CHILD_COMPL_ACTIVITY_RCC_SUPERSTRUCTURE detailedItem = new QMBridgeModel.EFORM_BRIDGE_QM_CHILD_COMPL_ACTIVITY_RCC_SUPERSTRUCTURE();
                                    detailedItem.SuperStructureType = item.SuperStructureType;
                                    detailedItem.RowID = i;
                                    objViewModel.BRIDGE_QM_CHILD_COMPL_ACTIVITY_RCC_SUPERSTRUCTURE_LIST.Add(detailedItem);

                                }

                            });

                            foreach (var item in objViewModel.BRIDGE_QM_CHILD_COMPL_ACTIVITY_RCC_SUPERSTRUCTURE_LIST.ToList())
                            {
                                if (item.RowID == 1)
                                {
                                    errorListtemp = objCommonFunction.FetchBridgeQMPDFFilledDataToModel(item, loadedForm);
                                    objViewModel.ErrorList.AddRange(errorListtemp);
                                }
                                else
                                {
                                    string v = "cb_18_" + item.RowID;
                                    PdfLoadedCheckBoxField CheckField = loadedForm.Fields[v] as PdfLoadedCheckBoxField;
                                    if (CheckField.Checked)
                                    {
                                        errorListtemp = objCommonFunction.FetchBridgeQMPDFFilledDataToModel(item, loadedForm);
                                        objViewModel.ErrorList.AddRange(errorListtemp);
                                    }
                                    else
                                    {
                                        objViewModel.BRIDGE_QM_CHILD_COMPL_ACTIVITY_RCC_SUPERSTRUCTURE_LIST.Remove(item);
                                    }
                                }
                            }

                        }

                    }

                    #endregion

                    #region BAILEY_BRIDGE

                    if (StructureType.SelectedValue == "BAILEY")
                    {

                        if (TemplateStatus != true) //  TemplateStatus != true means in P
                        {
                            errorListtemp = objCommonFunction.FetchBridgeQMPDFFilledDataToModel(objViewModel.BRIDGE_QM_CHILD_BAILEY_BRIDGE_SUPERSTRUCTURE, loadedForm);
                            objViewModel.ErrorList.AddRange(errorListtemp);

                            baileySuperstructure.BaileyQOMStructureList.ForEach(item =>
                            {
                                for (int i = 1; i <= item.RowCount; i++)
                                {
                                    QMBridgeModel.EFORM_BRIDGE_QM_CHILD_ON_QOM_BAILEY_SUPERSTRUCTURE detailedItem = new QMBridgeModel.EFORM_BRIDGE_QM_CHILD_ON_QOM_BAILEY_SUPERSTRUCTURE();
                                    detailedItem.SuperStructureType = item.SuperStructureType;
                                    detailedItem.RowID = i;
                                    objViewModel.BRIDGE_QM_CHILD_ON_QOM_BAILEY_SUPERSTRUCTURE_LIST.Add(detailedItem);
                                }

                            });

                            foreach (var item in objViewModel.BRIDGE_QM_CHILD_ON_QOM_BAILEY_SUPERSTRUCTURE_LIST.ToList())
                            {
                                if (item.RowID == 1)
                                {
                                    errorListtemp = objCommonFunction.FetchBridgeQMPDFFilledDataToModel(item, loadedForm);
                                    objViewModel.ErrorList.AddRange(errorListtemp);
                                }
                                else
                                {
                                    string v = "cb_6_3_" + item.RowID;
                                    PdfLoadedCheckBoxField CheckField = loadedForm.Fields[v] as PdfLoadedCheckBoxField;
                                    if (CheckField.Checked)
                                    {
                                        errorListtemp = objCommonFunction.FetchBridgeQMPDFFilledDataToModel(item, loadedForm);
                                        objViewModel.ErrorList.AddRange(errorListtemp);
                                    }
                                    else
                                    {
                                        objViewModel.BRIDGE_QM_CHILD_ON_QOM_BAILEY_SUPERSTRUCTURE_LIST.Remove(item);
                                    }
                                }
                            }

                        }
                        else
                        {
                            errorListtemp = objCommonFunction.FetchBridgeQMPDFFilledDataToModel(objViewModel.BRIDGE_QM_CHILD_BAILEY_BRIDGE_SUPERSTRUCTURE_COMPLETED, loadedForm);
                            objViewModel.ErrorList.AddRange(errorListtemp);

                            baileyCompleteStructure.BaileyCompleteStructureList.ForEach(item =>
                            {
                                for (int i = 1; i <= item.RowCount; i++)
                                {
                                    QMBridgeModel.EFORM_BRIDGE_QM_CHILD_CO_MATERIAL_BAILEY_SUPERSTRUCTURE detailedItem = new QMBridgeModel.EFORM_BRIDGE_QM_CHILD_CO_MATERIAL_BAILEY_SUPERSTRUCTURE();
                                    detailedItem.SuperStructureType = item.SuperStructureType;
                                    detailedItem.RowID = i;
                                    objViewModel.BRIDGE_QM_CHILD_CO_MATERIAL_BAILEY_SUPERSTRUCTURE_LIST.Add(detailedItem);
                                }
                            });

                            foreach (var item in objViewModel.BRIDGE_QM_CHILD_CO_MATERIAL_BAILEY_SUPERSTRUCTURE_LIST.ToList())
                            {
                                if (item.RowID == 1)
                                {
                                    errorListtemp = objCommonFunction.FetchBridgeQMPDFFilledDataToModel(item, loadedForm);
                                    objViewModel.ErrorList.AddRange(errorListtemp);
                                }
                                else
                                {
                                    string v = "cb_22_" + item.RowID;
                                    PdfLoadedCheckBoxField CheckField = loadedForm.Fields[v] as PdfLoadedCheckBoxField;
                                    if (CheckField.Checked)
                                    {
                                        errorListtemp = objCommonFunction.FetchBridgeQMPDFFilledDataToModel(item, loadedForm);
                                        objViewModel.ErrorList.AddRange(errorListtemp);
                                    }
                                    else
                                    {
                                        objViewModel.BRIDGE_QM_CHILD_CO_MATERIAL_BAILEY_SUPERSTRUCTURE_LIST.Remove(item);
                                    }
                                }
                            }
                        }
                    }

                    #endregion

                }

                #endregion

                #region vikky page 23-28 reading


                #region EFORM_BRIDGE_QM_LOAD_TEST
                if (chckBoxArr[7] == 'Y')
                {
                    errorListtemp = objCommonFunction.FetchBridgeQMPDFFilledDataToModel(objViewModel.BRIDGE_QM_LOAD_TEST, loadedForm);
                    objViewModel.ErrorList.AddRange(errorListtemp);
                }

                #endregion


                if (chckBoxArr[8] == 'Y')
                {
                    #region EFORM_BRIDGE_QM_BEARING
                    errorListtemp = objCommonFunction.FetchBridgeQMPDFFilledDataToModel(objViewModel.BRIDGE_QM_BEARING, loadedForm);
                    objViewModel.ErrorList.AddRange(errorListtemp);
                    if (objViewModel.BRIDGE_QM_BEARING.ROLLER_ROCKER_BEARING == "N" && objViewModel.BRIDGE_QM_BEARING.ELASTOMERIC_BEARING == "N" && objViewModel.BRIDGE_QM_BEARING.POT_BEARING == "N" && objViewModel.BRIDGE_QM_BEARING.SPHERICAL_BEARING == "N" && objViewModel.BRIDGE_QM_BEARING.CYLINDRICAL_BEARING == "N" && objViewModel.BRIDGE_QM_BEARING.IS_OTHER_BEARING_TYPE == "N")
                    {
                        errorListtemp.Clear();
                        errorListtemp.Add("Page-23: Item 8. BEARINGS- A) Types of Bearing: Please select atleast one bearing");
                        objViewModel.ErrorList.AddRange(errorListtemp);
                        errorListtemp.Clear();
                        objViewModel.ErrorOccured = true;
                    }
                    #endregion

                    #region EFORM_BRIDGE_QM_CHILD_BEARING_TYPE
                    errorListtemp = objCommonFunction.FetchBridgeQMPDFFilledDataToModel(objViewModel.BRIDGE_QM_CHILD_BEARING_TYPE, loadedForm);
                    objViewModel.ErrorList.AddRange(errorListtemp);
                    #endregion
                }


                #region EFORM_BRIDGE_QM_EXPANSION

                if (chckBoxArr[9] == 'Y')
                {
                    List<string> errorlisttemp1 = new List<string>();
                    errorListtemp = objCommonFunction.FetchBridgeQMPDFFilledDataToModel(objViewModel.BRIDGE_QM_EXPANSION, loadedForm);
                    errorlisttemp1.AddRange(errorListtemp);
                    if (objViewModel.BRIDGE_QM_EXPANSION.BURIED_OF_EXPANSION == "N" && objViewModel.BRIDGE_QM_EXPANSION.FILLER_OF_EXPANSION == "N" && objViewModel.BRIDGE_QM_EXPANSION.ASPHALTIC_OF_EXPANSION == "N" && objViewModel.BRIDGE_QM_EXPANSION.COMPRESSION_OF_EXPANSION == "N" && objViewModel.BRIDGE_QM_EXPANSION.SINGLE_OF_EXPANSION == "N" && objViewModel.BRIDGE_QM_EXPANSION.REINFORCED_OF_EXPANSION == "N" && objViewModel.BRIDGE_QM_EXPANSION.MODULAR_OF_EXPANSION == "N" && objViewModel.BRIDGE_QM_EXPANSION.FINGER_OF_EXPANSION == "N" && objViewModel.BRIDGE_QM_EXPANSION.REINFORCED_COUPLED_OF_EXPANSION == "N")
                    {
                        errorListtemp.Clear();
                        errorListtemp.Add("Page-25: Item 9. EXPANSION JOINTS- A) Types of Expansion joint-  Please select atleast one Expansion joint");
                        objViewModel.ErrorList.AddRange(errorListtemp);
                        errorListtemp.Clear();
                        objViewModel.ErrorOccured = true;
                    }
                    objViewModel.ErrorList.AddRange(errorlisttemp1);


                }




                #endregion


                if (chckBoxArr[10] == 'Y')
                {
                    #region EFORM_BRIDGE_QM_APPROACH
                    errorListtemp = objCommonFunction.FetchBridgeQMPDFFilledDataToModel(objViewModel.BRIDGE_QM_APPROACH, loadedForm);
                    objViewModel.ErrorList.AddRange(errorListtemp);
                    #endregion

                    #region EFORM_BRIDGE_QM_CHILD_EMBANKMENT_APPROACH
                    PdfLoadedCheckBoxField Check_Item_10_1 = loadedForm.Fields["Item_10_1"] as PdfLoadedCheckBoxField;
                    if (Check_Item_10_1.Checked)
                    {
                        approachEmbarkList.EmbarkList.ForEach(item =>
                        {

                            for (int i = 1; i <= item.RowCount; i++)
                            {
                                QMBridgeModel.EFORM_BRIDGE_QM_CHILD_EMBANKMENT_APPROACH detailedItem = new QMBridgeModel.EFORM_BRIDGE_QM_CHILD_EMBANKMENT_APPROACH();

                                detailedItem.ROW_ID = i;
                                objViewModel.BRIDGE_QM_CHILD_EMBANKMENT_APPROACH_LIST.Add(detailedItem);
                            }

                        });



                        foreach (var item in objViewModel.BRIDGE_QM_CHILD_EMBANKMENT_APPROACH_LIST.ToList())
                        {
                            if (item.ROW_ID == 1)
                            {
                                errorListtemp = objCommonFunction.FetchBridgeQMPDFFilledDataToModel(item, loadedForm);
                                objViewModel.ErrorList.AddRange(errorListtemp);
                            }
                            else
                            {
                                string v = "cb_10_1_" + item.ROW_ID;
                                PdfLoadedCheckBoxField CheckField = loadedForm.Fields[v] as PdfLoadedCheckBoxField;
                                if (CheckField.Checked)
                                {
                                    errorListtemp = objCommonFunction.FetchBridgeQMPDFFilledDataToModel(item, loadedForm);
                                    objViewModel.ErrorList.AddRange(errorListtemp);
                                }
                                else
                                {
                                    objViewModel.BRIDGE_QM_CHILD_EMBANKMENT_APPROACH_LIST.Remove(item);
                                }
                            }
                        }
                    }


                    #endregion

                    #region EFORM_BRIDGE_QM_CHILD_SUB_BASE_APPROACH
                    PdfLoadedCheckBoxField Check_Item_10_2 = loadedForm.Fields["Item_10_2"] as PdfLoadedCheckBoxField;
                    if (Check_Item_10_2.Checked)
                    {
                        approachSubBaseList.SubBaseList.ForEach(item =>
                        {

                            for (int i = 1; i <= item.RowCount; i++)
                            {
                                QMBridgeModel.EFORM_BRIDGE_QM_CHILD_SUB_BASE_APPROACH detailedItem = new QMBridgeModel.EFORM_BRIDGE_QM_CHILD_SUB_BASE_APPROACH();
                                detailedItem.ROW_ID = i;
                                objViewModel.BRIDGE_QM_CHILD_SUB_BASE_APPROACH_LIST.Add(detailedItem);
                            }

                        });



                        foreach (var item in objViewModel.BRIDGE_QM_CHILD_SUB_BASE_APPROACH_LIST.ToList())
                        {
                            if (item.ROW_ID == 1)
                            {
                                errorListtemp = objCommonFunction.FetchBridgeQMPDFFilledDataToModel(item, loadedForm);
                                objViewModel.ErrorList.AddRange(errorListtemp);
                            }
                            else
                            {
                                string v = "cb_10_2_" + item.ROW_ID;
                                PdfLoadedCheckBoxField CheckField = loadedForm.Fields[v] as PdfLoadedCheckBoxField;
                                if (CheckField.Checked)
                                {
                                    errorListtemp = objCommonFunction.FetchBridgeQMPDFFilledDataToModel(item, loadedForm);
                                    objViewModel.ErrorList.AddRange(errorListtemp);
                                }
                                else
                                {
                                    objViewModel.BRIDGE_QM_CHILD_SUB_BASE_APPROACH_LIST.Remove(item);
                                }
                            }
                        }

                    }

                    #endregion

                    #region EFORM_BRIDGE_QM_CHILD_BASE_COURSE_APPROACH
                    PdfLoadedCheckBoxField Check_Item_10_3 = loadedForm.Fields["Item_10_3"] as PdfLoadedCheckBoxField;
                    if (Check_Item_10_3.Checked)
                    {
                        approachBaseCourseList.BaseCourseList.ForEach(item =>
                        {

                            for (int i = 1; i <= item.RowCount; i++)
                            {
                                QMBridgeModel.EFORM_BRIDGE_QM_CHILD_BASE_COURSE_APPROACH detailedItem = new QMBridgeModel.EFORM_BRIDGE_QM_CHILD_BASE_COURSE_APPROACH();
                                detailedItem.ROW_ID = i;
                                objViewModel.BRIDGE_QM_CHILD_BASE_COURSE_APPROACH_LIST.Add(detailedItem);
                            }

                        });



                        foreach (var item in objViewModel.BRIDGE_QM_CHILD_BASE_COURSE_APPROACH_LIST.ToList())
                        {
                            if (item.ROW_ID == 1)
                            {
                                errorListtemp = objCommonFunction.FetchBridgeQMPDFFilledDataToModel(item, loadedForm);
                                objViewModel.ErrorList.AddRange(errorListtemp);
                            }
                            else
                            {
                                string v = "cb_10_3_" + item.ROW_ID;
                                PdfLoadedCheckBoxField CheckField = loadedForm.Fields[v] as PdfLoadedCheckBoxField;
                                if (CheckField.Checked)
                                {
                                    errorListtemp = objCommonFunction.FetchBridgeQMPDFFilledDataToModel(item, loadedForm);
                                    objViewModel.ErrorList.AddRange(errorListtemp);
                                }
                                else
                                {
                                    objViewModel.BRIDGE_QM_CHILD_BASE_COURSE_APPROACH_LIST.Remove(item);
                                }
                            }
                        }
                    }

                    #endregion

                    #region EFORM_BRIDGE_QM_CHILD_WEARING_COURSE_APPROACH
                    PdfLoadedCheckBoxField Check_Item_10_4 = loadedForm.Fields["Item_10_4"] as PdfLoadedCheckBoxField;
                    if (Check_Item_10_4.Checked)
                    {
                        approachWearingCourseList.WearingCourseList.ForEach(item =>
                        {

                            for (int i = 1; i <= item.RowCount; i++)
                            {
                                QMBridgeModel.EFORM_BRIDGE_QM_CHILD_WEARING_COURSE_APPROACH detailedItem = new QMBridgeModel.EFORM_BRIDGE_QM_CHILD_WEARING_COURSE_APPROACH();
                                detailedItem.ROW_ID = i;
                                objViewModel.BRIDGE_QM_CHILD_WEARING_COURSE_APPROACH_LIST.Add(detailedItem);
                            }

                        });


                        foreach (var item in objViewModel.BRIDGE_QM_CHILD_WEARING_COURSE_APPROACH_LIST.ToList())
                        {
                            if (item.ROW_ID == 1)
                            {
                                errorListtemp = objCommonFunction.FetchBridgeQMPDFFilledDataToModel(item, loadedForm);
                                objViewModel.ErrorList.AddRange(errorListtemp);
                            }
                            else
                            {
                                string v = "cb_10_4_" + item.ROW_ID;
                                PdfLoadedCheckBoxField CheckField = loadedForm.Fields[v] as PdfLoadedCheckBoxField;
                                if (CheckField.Checked)
                                {
                                    errorListtemp = objCommonFunction.FetchBridgeQMPDFFilledDataToModel(item, loadedForm);
                                    objViewModel.ErrorList.AddRange(errorListtemp);
                                }
                                else
                                {
                                    objViewModel.BRIDGE_QM_CHILD_WEARING_COURSE_APPROACH_LIST.Remove(item);
                                }
                            }
                        }
                    }

                    #endregion

                    #region EFORM_BRIDGE_QM_CHILD_PROTECH_APPROACH
                    PdfLoadedCheckBoxField Check_Item_10_5 = loadedForm.Fields["Item_10_5"] as PdfLoadedCheckBoxField;
                    if (Check_Item_10_5.Checked)
                    {

                        errorListtemp = objCommonFunction.FetchBridgeQMPDFFilledDataToModel(objViewModel.BRIDGE_QM_CHILD_PROTECH_APPROACH, loadedForm);
                        objViewModel.ErrorList.AddRange(errorListtemp);

                        PdfLoadedRadioButtonListField Check_IS_DPR_PROV_PROTE_WORKS = loadedForm.Fields["IS_DPR_PROV_PROTE_WORKS"] as PdfLoadedRadioButtonListField;
                        if (Check_IS_DPR_PROV_PROTE_WORKS.SelectedValue == "Yes")
                        {
                            if (objViewModel.BRIDGE_QM_CHILD_PROTECH_APPROACH.IS_RETAINING_WALL == "N" && objViewModel.BRIDGE_QM_CHILD_PROTECH_APPROACH.IS_BREAST_WALL == "N" && objViewModel.BRIDGE_QM_CHILD_PROTECH_APPROACH.IS_PARAPET_WALL == "N" && objViewModel.BRIDGE_QM_CHILD_PROTECH_APPROACH.IS_ANY_OTHER_PROT_WORK == "N")
                            {
                                errorListtemp.Clear();
                                errorListtemp.Add("Page-27: Item 10. APPROACHES 10.5 Protection Work- II. Please select atleast one protection work");
                                objViewModel.ErrorList.AddRange(errorListtemp);
                                errorListtemp.Clear();
                                objViewModel.ErrorOccured = true;
                            }
                        }
                        #endregion

                        #region EFORM_BRIDGE_QM_CHILD_QOM_APPROACH


                        approachProtQOMList.ProtQOMList.ForEach(item =>
                        {

                            for (int i = 1; i <= item.RowCount; i++)
                            {
                                QMBridgeModel.EFORM_BRIDGE_QM_CHILD_QOM_APPROACH detailedItem = new QMBridgeModel.EFORM_BRIDGE_QM_CHILD_QOM_APPROACH();
                                detailedItem.ROW_ID = i;
                                objViewModel.BRIDGE_QM_CHILD_QOM_APPROACH_LIST.Add(detailedItem);
                            }

                        });



                        foreach (var item in objViewModel.BRIDGE_QM_CHILD_QOM_APPROACH_LIST.ToList())
                        {
                            if (item.ROW_ID == 1)
                            {
                                errorListtemp = objCommonFunction.FetchBridgeQMPDFFilledDataToModel(item, loadedForm);
                                objViewModel.ErrorList.AddRange(errorListtemp);
                            }
                            else
                            {
                                string v = "cb_10_5_1_" + item.ROW_ID;
                                PdfLoadedCheckBoxField CheckField = loadedForm.Fields[v] as PdfLoadedCheckBoxField;
                                if (CheckField.Checked)
                                {
                                    errorListtemp = objCommonFunction.FetchBridgeQMPDFFilledDataToModel(item, loadedForm);
                                    objViewModel.ErrorList.AddRange(errorListtemp);
                                }
                                else
                                {
                                    objViewModel.BRIDGE_QM_CHILD_QOM_APPROACH_LIST.Remove(item);
                                }
                            }
                        }


                        #endregion

                        #region EFORM_BRIDGE_QM_CHILD_WORKMENSHIP_APPROACH


                        approachProtWorkmanshipList.ProtWorkmanshipList.ForEach(item =>
                        {

                            for (int i = 1; i <= item.RowCount; i++)
                            {
                                QMBridgeModel.EFORM_BRIDGE_QM_CHILD_WORKMENSHIP_APPROACH detailedItem = new QMBridgeModel.EFORM_BRIDGE_QM_CHILD_WORKMENSHIP_APPROACH();
                                detailedItem.ROW_ID = i;
                                objViewModel.BRIDGE_QM_CHILD_WORKMENSHIP_APPROACH_LIST.Add(detailedItem);
                            }

                        });



                        foreach (var item in objViewModel.BRIDGE_QM_CHILD_WORKMENSHIP_APPROACH_LIST.ToList())
                        {
                            if (item.ROW_ID == 1)
                            {
                                errorListtemp = objCommonFunction.FetchBridgeQMPDFFilledDataToModel(item, loadedForm);
                                objViewModel.ErrorList.AddRange(errorListtemp);
                            }
                            else
                            {
                                string v = "cb_10_5_2_" + item.ROW_ID;
                                PdfLoadedCheckBoxField CheckField = loadedForm.Fields[v] as PdfLoadedCheckBoxField;
                                if (CheckField.Checked)
                                {
                                    errorListtemp = objCommonFunction.FetchBridgeQMPDFFilledDataToModel(item, loadedForm);
                                    objViewModel.ErrorList.AddRange(errorListtemp);
                                }
                                else
                                {
                                    objViewModel.BRIDGE_QM_CHILD_WORKMENSHIP_APPROACH_LIST.Remove(item);
                                }
                            }
                        }


                        #endregion

                    }
                }


                #endregion


                #region Bhushan page 29-35 reading
                if (chckBoxArr[11] == 'Y')
                {
                    #region EFORM_BRIDGE_QM_FURNITURE_MARKINGS
                    errorListtemp = objCommonFunction.FetchBridgeQMPDFFilledDataToModel(objViewModel.BRIDGE_QM_FURNITURE_MARKINGS, loadedForm);
                    objViewModel.ErrorList.AddRange(errorListtemp);
                    #endregion

                    #region EFORM_BRIDGE_QM_CHILD_OBS_FURNITURE_MARKING
                    errorListtemp = objCommonFunction.FetchBridgeQMPDFFilledDataToModel(objViewModel.BRIDGE_QM_CHILD_OBS_FURNITURE_MARKING, loadedForm);
                    objViewModel.ErrorList.AddRange(errorListtemp);
                    #endregion
                }

                #region EFORM_BRIDGE_QM_DEFICIENCY

                errorListtemp = objCommonFunction.FetchBridgeQMPDFFilledDataToModel(objViewModel.BRIDGE_QM_DEFICIENCY, loadedForm);
                objViewModel.ErrorList.AddRange(errorListtemp);

                PdfLoadedRadioButtonListField itemGrad = loadedForm.Fields["IS_NO_DEFICIENCY"] as PdfLoadedRadioButtonListField;
                if (itemGrad.SelectedValue == "Yes")
                {
                    PdfLoadedCheckBoxField IS_BOQ_NOT_CLEAR = loadedForm.Fields["IS_BOQ_NOT_CLEAR"] as PdfLoadedCheckBoxField;
                    PdfLoadedCheckBoxField IS_NO_SPANS_INSUFFICIENT = loadedForm.Fields["IS_NO_SPANS_INSUFFICIENT"] as PdfLoadedCheckBoxField;
                    PdfLoadedCheckBoxField IS_NO_PROVISION_PROTECTIVE_WORK = loadedForm.Fields["IS_NO_PROVISION_PROTECTIVE_WORK"] as PdfLoadedCheckBoxField;
                    PdfLoadedCheckBoxField IS_HYDROLIC_DESIGN_DPR = loadedForm.Fields["IS_HYDROLIC_DESIGN_DPR"] as PdfLoadedCheckBoxField;
                    PdfLoadedCheckBoxField IS_GUARD_STONE_IN_DPR = loadedForm.Fields["IS_GUARD_STONE_IN_DPR"] as PdfLoadedCheckBoxField;
                    PdfLoadedCheckBoxField IS_DEVIATION_ALIGNMENT = loadedForm.Fields["IS_DEVIATION_ALIGNMENT"] as PdfLoadedCheckBoxField;

                    if ((!IS_BOQ_NOT_CLEAR.Checked) && (!IS_NO_SPANS_INSUFFICIENT.Checked) && (!IS_NO_PROVISION_PROTECTIVE_WORK.Checked) && (!IS_HYDROLIC_DESIGN_DPR.Checked) && (!IS_GUARD_STONE_IN_DPR.Checked) && (!IS_DEVIATION_ALIGNMENT.Checked))
                    {
                        errorListtemp.Add("Page-30: Item 12. A. At Least one Deficiencies Observed must be Selected");
                        objViewModel.ErrorList.AddRange(errorListtemp);
                    }
                }

                #endregion

                #region EFORM_BRIDGE_QM_WORK_COMPLETED_IN_PROGRESS_ASPER_PROG

                errorListtemp = objCommonFunction.FetchBridgeQMPDFFilledDataToModel(objViewModel.BRIDGE_QM_WORK_COMPLETED_IN_PROGRESS_ASPER_PROG, loadedForm);
                objViewModel.ErrorList.AddRange(errorListtemp);
                #endregion

                #region EFORM_BRIDGE_QM_WORK_COMPLETED_IN_PROGRESS_ASPER_COST
                errorListtemp = objCommonFunction.FetchBridgeQMPDFFilledDataToModel(objViewModel.BRIDGE_QM_WORK_COMPLETED_IN_PROGRESS_ASPER_COST, loadedForm);
                objViewModel.ErrorList.AddRange(errorListtemp);
                #endregion

                #region EFORM_BRIDGE_QM_ACTION_TAKEN_PIU
                for (int i = 1; i <= actionTakenByPIUModelDetails.RowCount; i++)
                {
                    QMBridgeModel.EFORM_BRIDGE_QM_ACTION_TAKEN_PIU detailedItem = new QMBridgeModel.EFORM_BRIDGE_QM_ACTION_TAKEN_PIU();
                    detailedItem.ROW_ID = i;
                    objViewModel.BRIDGE_QM_ACTION_TAKEN_PIU_LIST.Add(detailedItem);
                }

                foreach (var item in objViewModel.BRIDGE_QM_ACTION_TAKEN_PIU_LIST)
                {
                    errorListtemp = objCommonFunction.FetchBridgeQMPDFFilledDataToModel(item, loadedForm);
                    objViewModel.ErrorList.AddRange(errorListtemp);
                }
                #endregion

                #region EFORM_BRIDGE_QM_DIFFERENCE_IN_OBSERVATION
                errorListtemp = objCommonFunction.FetchBridgeQMPDFFilledDataToModel(objViewModel.BRIDGE_QM_DIFFERENCE_IN_OBSERVATION, loadedForm);
                objViewModel.ErrorList.AddRange(errorListtemp);
                #endregion

                #region EFORM_BRIDGE_QM_OVERALL_GRADING
                errorListtemp = objCommonFunction.FetchBridgeQMPDFFilledDataToModel(objViewModel.BRIDGE_QM_OVERALL_GRADING, loadedForm);
                objViewModel.ErrorList.AddRange(errorListtemp);
                #endregion

                #region EFORM_BRIDGE_QM_QUALITY_GRADING
                errorListtemp = objCommonFunction.FetchBridgeQMPDFFilledDataToModel(objViewModel.BRIDGE_QM_QUALITY_GRADING, loadedForm);
                objViewModel.ErrorList.AddRange(errorListtemp);
                #endregion

                #endregion


                if (objViewModel.ErrorList.Count > 0)
                {
                    objViewModel.ErrorOccured = true;
                }

                return objViewModel;
                #endregion
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "EFORMController.PdfBridgeQMDataToClassModel()");
            }
            finally
            {
                dbContext.Dispose();
            }
            return null;
        }



        #endregion

        #region EFORM_BRIDGE_CQC_DEVELOPMENT

        public ActionResult Preview_BRIDGE_CQC(string eid)
        {
            try
            {
                string[] encParam = eid.Split('/');
                decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { encParam[0], encParam[1], encParam[2] });
                int EformId = 0;
                if (decryptedParameters.Count > 0)
                {
                    EformId = Convert.ToInt32(decryptedParameters["imsRoadID"].Split('$')[0]);
                }

                IPdfDataDAL objDAL = new PdfDataDAL();

                EFORM_BRIDGE_PIU_PREVIEW_MODEL objPIU_Bridge = objDAL.PreviewBridgePIU_SavedDataDAL(EformId);
                EFORM_BRIDGE_QM_PREVIEW_MODEL objQM_Bridge = objDAL.PreviewBridgeQM_SavedDataDAL(EformId);

                EFORM_BRIDGE_CQC_PREVIEW_MODEL objCQC_Bridge = new EFORM_BRIDGE_CQC_PREVIEW_MODEL();
                objCQC_Bridge.PIU_BRIDGE_VIEWMODEL = objPIU_Bridge;
                objCQC_Bridge.QM_BRIDGE_VIEWMODEL = objQM_Bridge;

                return View("~/Areas/EFORMArea/Views/EFORM/Preview_BRIDGE_CQC.cshtml", objCQC_Bridge);

            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "EFORMController.Preview_BRIDGE_CQC()");
                return View("~/Areas/EFORMArea/Views/EFORM/PDF_ErrorPage");
            }
        }






        #endregion






        #region eform PPT download
        public FileResult GetDownloadedPPT()
        {

            try
            {
                string pptFile = ConfigurationManager.AppSettings["EFORM_PPT"].ToString();
                string pptFileName = "Eform PPT.pdf";
                byte[] FileBytesFalse = System.IO.File.ReadAllBytes(pptFile);
                return File(FileBytesFalse, "application/pdf", pptFileName);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "eformController.GetDownloadedPPT()");
                string error_File = ConfigurationManager.AppSettings["ERROR_FILE"].ToString();
                string Errorfilename = "Error.pdf";
                byte[] FileBytes = System.IO.File.ReadAllBytes(error_File);
                return File(FileBytes, "application/pdf", Errorfilename);
            }

        }
        #endregion


        #region eform User Manual download
        public FileResult GetDownloadedUserManual()
        {

            try
            {
                string pptFile = ConfigurationManager.AppSettings["EFORM_MANUAL"].ToString();
                string pptFileName = "Eform user manual.pdf";
                byte[] FileBytesFalse = System.IO.File.ReadAllBytes(pptFile);
                return File(FileBytesFalse, "application/pdf", pptFileName);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "eformController.GetDownloadedUserManual()");
                string error_File = ConfigurationManager.AppSettings["ERROR_FILE"].ToString();
                string Errorfilename = "Error.pdf";
                byte[] FileBytes = System.IO.File.ReadAllBytes(error_File);
                return File(FileBytes, "application/pdf", Errorfilename);
            }

        }
        #endregion



        #region Ajinkya Test Report
        //Ajinkya

        public JsonResult IsTestReportPDFFileavaialble(string id)
        {
            bool flag = false;
            string message = string.Empty;
            PMGSYEntities dbContext = new PMGSYEntities();
            try
            {

                int eformId = Convert.ToInt32(id.Split('$')[7]);
                //var fileName = "";
                // var fileName = dbContext.EFORM_TEST_REPORT_FILE.Where(x => x.EFORM_ID == eformId && x.IS_EFORM=='N').Select(s => s.FILE_NAME).FirstOrDefault();
                //string fullPath = string.Empty;
                if (dbContext.EFORM_TEST_REPORT_FILE.Any(s => s.EFORM_ID == eformId && s.IS_EFORM == "N"))
                {

                    flag = true;
                    message = "Pdf is already uploaded. If you want to upload again, please delete the existing file";

                }
                else
                {

                    return Json(new { success = flag }, JsonRequestBehavior.AllowGet);
                }

            }
            catch (Exception ex)
            {
                flag = true;
                ErrorLog.LogError(ex, "eformcontroller.IsQMPDFFileavaialble()");
                message = "Error occured during uploading file";
                return Json(new { success = flag }, JsonRequestBehavior.AllowGet);
            }
            finally
            {
                dbContext.Dispose();
            }

            return Json(new { success = flag, message = message }, JsonRequestBehavior.AllowGet);
        }

        //Ajinkya

        public ActionResult uploadTestRepotPdfFile(string id)
        {


            IPdfDataDAL objDAL = new PdfDataDAL();
            bool flag = true;
            string responseMessage = string.Empty;
            string fname;
            string uploaded_File;
            List<SelectListItem> result = new List<SelectListItem>();

            PMGSYEntities dbContext = new PMGSYEntities();
            int UserId = PMGSYSession.Current.UserId;
            string ipAdd = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];


            int eformId = Convert.ToInt32(id.Split('$')[7]);
            if (Request.Files.Count > 0)
            {
                HttpPostedFileBase file = Request.Files[0];

                List<SelectListItem> validationList = new List<SelectListItem>();
                //add more conditions like file type, file size etc as per your need.
                int fileSizeLimit = Convert.ToInt16(ConfigurationManager.AppSettings["TEST_REPORT_UPLOADED_FILE_SIZE"]) * 1024 * 1024;
                if (file.ContentLength < fileSizeLimit)
                {
                    if (file != null && file.ContentLength > 0 && Path.GetExtension(file.FileName).ToLower() == ".pdf")
                    {

                        try
                        {

                            fname = "S_$" + eformId + "_TestReport_uploaded_" + System.DateTime.Now.ToString("dd_MM_yyyy_HHmmss") + ".pdf";

                            var eformModel = dbContext.EFORM_MASTER.Where(s => s.EFORM_ID == eformId).FirstOrDefault();
                            using (var scope = new TransactionScope())
                            {
                                if (dbContext.EFORM_TEST_REPORT_FILE.Any(s => s.EFORM_ID == eformId && s.IS_EFORM == "N"))
                                {

                                    responseMessage = "Pdf is already uploaded. If you want to upload again, please delete the existing file";
                                    flag = false;
                                    return Json(new { success = flag, responseMsg = responseMessage });
                                }
                                String Uploaded_Path = ConfigurationManager.AppSettings["TEST_REPORT_SCAN_COPY_UPLOADED_PATH"] + "\\" + eformModel.ADMIN_SCHEDULE_YEAR + "\\" + eformModel.ADMIN_SCHEDULE_MONTH;
                                if (!Directory.Exists(Uploaded_Path))
                                    Directory.CreateDirectory(Uploaded_Path);
                                uploaded_File = Path.Combine(Uploaded_Path, fname);
                                file.SaveAs(uploaded_File);


                                EFORM_TEST_REPORT_FILE uploadTestReport = new EFORM_TEST_REPORT_FILE();
                                uploadTestReport.FILE_ID = dbContext.EFORM_TEST_REPORT_FILE.Any() ? (from item in dbContext.EFORM_TEST_REPORT_FILE select item.FILE_ID).Max() + 1 : 1;
                                uploadTestReport.EFORM_ID = eformId;
                                uploadTestReport.FILE_NAME = fname;
                                // uploadTestReport.FILE_PATH = Uploaded_Path + "\\" + schdYear + "\\" + schdMonth;
                                uploadTestReport.FILE_PATH = Uploaded_Path;
                                uploadTestReport.FILE_UPLOAD_DATE = DateTime.Now;
                                uploadTestReport.IS_FINALISED = "N";
                                uploadTestReport.USERID = UserId;
                                uploadTestReport.IPADD = ipAdd;
                                uploadTestReport.IS_EFORM = "N";
                                dbContext.EFORM_TEST_REPORT_FILE.Add(uploadTestReport);
                                dbContext.SaveChanges();
                                scope.Complete();
                            }

                        }
                        catch (Exception ex)
                        {
                            ErrorLog.LogError(ex, "eformcontroller.uploadTestRepotPdfFile()");
                            responseMessage = "Error occured while uploading pdf, please contact OMMAS team.";
                            flag = false;
                            return Json(new { success = flag, responseMsg = responseMessage });
                        }
                        finally
                        {
                            file.InputStream.Flush();
                            file.InputStream.Close();
                            dbContext.Dispose();
                        }

                    }
                    else
                    {
                        flag = false;
                        responseMessage = "File is invalid.";
                        return Json(new { success = flag, responseMsg = responseMessage });
                    }
                }
                else
                {
                    flag = false;
                    responseMessage = "Invalid file size. Please upload file upto 15 MB.";
                    return Json(new { success = flag, responseMsg = responseMessage });
                }
            }
            else
            {
                flag = false;
                responseMessage = "File Upload has no file.";
            }

            return Json(new { success = flag, message = responseMessage }, JsonRequestBehavior.AllowGet);

        }
        //Ajinkya
        public JsonResult deleteTestReportPdf(string id)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            string eformIddec = id.Split('$')[1];
            try
            {

                using (var scope = new TransactionScope())
                {
                    int eformId = Convert.ToInt32(eformIddec);

                    var fileName = dbContext.EFORM_TEST_REPORT_FILE.Where(x => x.EFORM_ID == eformId && x.IS_EFORM == "N").Select(s => s.FILE_NAME).FirstOrDefault();
                    string Uploaded_Path = dbContext.EFORM_TEST_REPORT_FILE.Where(x => x.EFORM_ID == eformId && x.IS_EFORM == "N").Select(s => s.FILE_PATH).FirstOrDefault();


                    var testReport_records = dbContext.EFORM_TEST_REPORT_FILE.Where(x => x.EFORM_ID == eformId && x.IS_EFORM == "N");
                    dbContext.EFORM_TEST_REPORT_FILE.RemoveRange(testReport_records);
                    dbContext.SaveChanges();

                    string fullPath = Path.Combine(Uploaded_Path, fileName);
                    FileInfo file = new FileInfo(fullPath);
                    if (file.Exists)
                    {
                        file.Delete();
                    }



                    scope.Complete();

                    return Json(new { success = true }, JsonRequestBehavior.AllowGet);

                }

            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "eformcontroller.deleteTestReportPdf()");
                return Json(new { success = false, message = "error occured during deleting the pdf." }, JsonRequestBehavior.AllowGet);
            }
            finally
            {
                dbContext.Dispose();
            }

        }

        //Ajinkya
        public FileResult GetTestReport(string idtemp)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            try
            {

                string id = null;
                string[] encParam = idtemp.Split('/');
                decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { encParam[0], encParam[1], encParam[2] });
                if (decryptedParameters.Count > 0)
                {
                    id = decryptedParameters["imsRoadID"];
                }

                int eformId = Convert.ToInt32(id);
                EFORM_MASTER model = dbContext.EFORM_MASTER.Where(s => s.EFORM_ID == eformId).FirstOrDefault();
                string uploaded_path = string.Empty;

                var fileName = dbContext.EFORM_TEST_REPORT_FILE.Where(x => x.EFORM_ID == eformId && x.IS_EFORM == "N").Select(s => s.FILE_NAME).FirstOrDefault();

                uploaded_path = ConfigurationManager.AppSettings["TEST_REPORT_UPLOADED_PATH"].ToString();


                var nameReport = id + "_" + "Test_Report.pdf";
                string fullPath = Path.Combine(uploaded_path, fileName);
                byte[] FileBytes = System.IO.File.ReadAllBytes(fullPath);
                return File(FileBytes, "application/pdf", nameReport);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "eformController.GetTestReport()");
                return null;
            }
            finally
            {
                dbContext.Dispose();
            }

        }


        [RequiredAuthentication]
        public JsonResult viewTRScanPdfVirtualDir(string id)
        {
            PMGSYEntities dbContext = new PMGSYEntities();

            try
            {

                int eformId = Convert.ToInt32(id);
                EFORM_MASTER model = dbContext.EFORM_MASTER.Where(s => s.EFORM_ID == eformId).FirstOrDefault();
                string uploaded_path = string.Empty;
                var modelTestReportUploadFile = dbContext.EFORM_TEST_REPORT_FILE.Where(x => x.EFORM_ID == eformId && x.IS_EFORM == "N").FirstOrDefault();
                string VirtualDirectoryPath = string.Empty;
                var eformModel = dbContext.EFORM_MASTER.Where(s => s.EFORM_ID == eformId).FirstOrDefault();
                if (modelTestReportUploadFile.FILE_NAME.Contains('$'))
                {
                    VirtualDirectoryPath = ConfigurationManager.AppSettings["TEST_REPORT_UPLOADED_SCAN_COPY_VIRTUAL_DIR_PATH"].ToString() + "/" + eformModel.ADMIN_SCHEDULE_YEAR + "/" + eformModel.ADMIN_SCHEDULE_MONTH;
                }
                else
                {
                    VirtualDirectoryPath = ConfigurationManager.AppSettings["TEST_REPORT_UPLOADED_EFORM_VIRTUAL_DIR_PATH"].ToString();
                }


                string VirtualDirectoryfullPath = string.Empty;

                if (modelTestReportUploadFile.FILE_NAME.Contains('$'))
                {
                    uploaded_path = ConfigurationManager.AppSettings["TEST_REPORT_SCAN_COPY_UPLOADED_PATH"].ToString() + "/" + model.ADMIN_SCHEDULE_YEAR.ToString() + "/" + model.ADMIN_SCHEDULE_MONTH.ToString();
                }
                else
                {
                    uploaded_path = ConfigurationManager.AppSettings["TEST_REPORT_UPLOADED_PATH"].ToString();

                }





                VirtualDirectoryfullPath = Path.Combine(VirtualDirectoryPath, modelTestReportUploadFile.FILE_NAME.ToString()).ToString().Replace(@"\\", @"//").Replace(@"\", @"/");

                string physicalFullPath = Path.Combine(uploaded_path, modelTestReportUploadFile.FILE_NAME);

                FileInfo file = new FileInfo(physicalFullPath);
                if (file.Exists)
                {
                    return Json(new { success = true, Message = VirtualDirectoryfullPath }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { success = false, Message = "File not found" }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "eformcontroller.viewTRPdfVirtualDir()");
                return null;
            }
            finally
            {
                dbContext.Dispose();
            }

        }



        //Ajinkya
        public JsonResult finalizeScanTestReport(string id)
        {
            bool flag = true;
            PMGSYEntities dbContext = new PMGSYEntities();
            try
            {
                int detailId = Convert.ToInt32(id);

                var TRfile_Status = dbContext.EFORM_TEST_REPORT_FILE.Where(x => x.EFORM_ID == detailId && x.IS_EFORM == "N").Select(x => x.FILE_NAME).FirstOrDefault();
                var isTRfileupload = "N";
                if (TRfile_Status != null)
                {
                    isTRfileupload = "Y";
                }
                if (isTRfileupload != "Y")
                {
                    flag = false;
                    string responseMessage = "Unable to finalize Test Report,as pdf is not uploaded yet";
                    return Json(new { success = flag, responseMsg = responseMessage });
                }
                using (TransactionScope ts = new TransactionScope())
                {

                    EFORM_TEST_REPORT_FILE obj = dbContext.EFORM_TEST_REPORT_FILE.Where(s => s.EFORM_ID == detailId && s.IS_EFORM == "N").FirstOrDefault();
                    obj.IS_FINALISED = "Y";
                    dbContext.Entry(obj).State = EntityState.Modified;
                    dbContext.SaveChanges();
                    ts.Complete();

                }
                return Json(new { success = flag }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                flag = false;
                ErrorLog.LogError(ex, "eformController.finalizeScanTestReport()");
                return Json(new { success = flag }, JsonRequestBehavior.AllowGet);
            }
            finally
            {
                dbContext.Dispose();
            }

        }
        #endregion



        #region Test Report e-Form

        [RequiredAuthentication]
        public FileResult GetDownloadedTRPDF(string encRoadCode)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            try
            {
                IPdfDataDAL objDAL = new PdfDataDAL();

                string decRoadCode = string.Empty;
                string qmName = string.Empty;
                string qmType = string.Empty;
                string qmMonth = string.Empty;
                string qmYear = string.Empty;
                string qmCode = string.Empty;
                string QmScheduleCode = string.Empty;
                string eformIDfromGrid = string.Empty;
                string workStatus = string.Empty;
                string[] encParam = encRoadCode.Split('/');
                decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { encParam[0], encParam[1], encParam[2] });
                if (decryptedParameters.Count > 0)
                {
                    decRoadCode = decryptedParameters["imsRoadID"].Split('$')[0];
                    qmName = decryptedParameters["imsRoadID"].Split('$')[1];
                    qmType = decryptedParameters["imsRoadID"].Split('$')[2];
                    qmMonth = decryptedParameters["imsRoadID"].Split('$')[3];
                    qmYear = decryptedParameters["imsRoadID"].Split('$')[4];
                    qmCode = decryptedParameters["imsRoadID"].Split('$')[5];
                    QmScheduleCode = decryptedParameters["imsRoadID"].Split('$')[6];
                    eformIDfromGrid = decryptedParameters["imsRoadID"].Split('$')[7];
                    workStatus = decryptedParameters["imsRoadID"].Split('$')[8];

                }

                int roadCode = Convert.ToInt32(decRoadCode);
                int eformIDfrmGrid = Convert.ToInt32(eformIDfromGrid);
                string Template_File = string.Empty;

                string filepath = ConfigurationManager.AppSettings["TEMPLATE_FILE"].ToString();

                if (workStatus.Equals("C"))
                {
                    Template_File = Path.Combine(filepath, "TEST_REPORT_C.pdf");
                }
                else if (workStatus.Equals("P"))
                {

                    Template_File = Path.Combine(filepath, "TEST_REPORT_P.pdf");

                }
                PdfLoadedDocument loadedDocument = new PdfLoadedDocument(Template_File);

                #region COMMENTED CODE

                PdfLoadedForm loadedForm = loadedDocument.Form;
                string templateVersionValue = string.Empty;
                try
                {
                    PdfLoadedTextBoxField templateVersion = loadedForm.Fields["TEMPLATE_VERSION"] as PdfLoadedTextBoxField;
                    templateVersionValue = templateVersion.Text;
                }
                catch (Exception ex)
                {
                    ErrorLog.LogError(ex, "eformController.GetDownloadedTRPDF()");
                }


                EFORM_DU_LOG LogDetail = new EFORM_DU_LOG();
                string CheckSumString = decRoadCode + qmMonth + qmYear + qmCode + "TESTREPORT";
                string CheckSumByteCode = objDAL.ComputeStringToSha256Hash(CheckSumString);
                LogDetail.PDF_CHECKSUM = CheckSumByteCode;
                if (templateVersionValue != "")
                {
                    LogDetail.TEMPLATE_VERSION = templateVersionValue;
                }
                else
                {
                    LogDetail.TEMPLATE_VERSION = "V1.0";
                }
                LogDetail.EFORM_ID = dbContext.EFORM_MASTER.Where(s => s.EFORM_ID == eformIDfrmGrid).Select(d => d.EFORM_ID).FirstOrDefault();
                LogDetail.USER_TYPE = "T";
                bool IsInserLogSuccess = objDAL.InsertDownloadLogDetails(LogDetail);




                if (IsInserLogSuccess == false)
                {

                    string error_File = ConfigurationManager.AppSettings["ERROR_FILE"].ToString();
                    string Errorfilename = "Error.pdf";
                    byte[] FileBytesFalse = System.IO.File.ReadAllBytes(error_File);
                    return File(FileBytesFalse, "application/pdf", Errorfilename);
                }



                PIU_GET_PREFILLED_DETAILS imsDetailsList = new PIU_GET_PREFILLED_DETAILS();
                imsDetailsList = objDAL.getPIUPrefilledList(roadCode, eformIDfrmGrid);  //change on 08-07-2022              


                PrefilledTestReportModel model = new PrefilledTestReportModel();


                EFORM_PIU_GENERAL_INFO piuGenModel = dbContext.EFORM_PIU_GENERAL_INFO.Where(s => s.EFORM_ID == eformIDfrmGrid).FirstOrDefault();

                model.RoadCode = URLEncrypt.EncryptParameters1(new string[] { "encRoadCode=" + imsDetailsList.RoadCode.ToString() + "$" + imsDetailsList.Eform_Id.ToString() });
                model.checksum = CheckSumByteCode;
                model.EFORM_ID = imsDetailsList.Eform_Id.ToString();
                model.Road_Name = imsDetailsList.RoadName + " / " + imsDetailsList.PkgNumber;
                if (piuGenModel != null)
                {
                    model.TESTED_BY_PIU = piuGenModel.PIU_HEAD_NAME;
                }
                model.TEST_CONDUCTED_IN_PRESENCE = imsDetailsList.QM_NAME.Trim();


                new CommonFunctions().GeneratePDFPreFilledDataModel(model, loadedForm);


                #endregion

                PdfLoadedTextBoxField piuHeadName = loadedForm.Fields["TESTED_BY_PIU"] as PdfLoadedTextBoxField;
                piuHeadName.ReadOnly = false;



                //save prefilled pdf
                string Generated_Path = ConfigurationManager.AppSettings["TEMPORARY_FILES_PATH"].ToString();
                if (!Directory.Exists(Generated_Path))
                    Directory.CreateDirectory(Generated_Path);


                string fileName = eformIDfrmGrid + "TestReport_" + System.DateTime.Now.ToString("dd_MM_yyyy_HHmmss") + ".pdf";
                var fullFilePath1 = Path.Combine(Generated_Path, fileName);
                loadedDocument.Save(fullFilePath1);
                loadedDocument.Close(true);
                //Close the document
                byte[] FileBytes = System.IO.File.ReadAllBytes(fullFilePath1);



                try
                {
                    string temp_Path = ConfigurationManager.AppSettings["TEMPORARY_FILES_PATH"];
                    System.IO.DirectoryInfo di = new DirectoryInfo(temp_Path);

                    foreach (FileInfo file in di.GetFiles())
                    {
                        try
                        {
                            file.Delete();
                        }
                        catch (Exception ex)
                        {
                            ErrorLog.LogError(ex, "eformcontroller.GetDownloadedTRPDF()");
                        }

                    }

                }
                catch (Exception ex)
                {
                    ErrorLog.LogError(ex, "eformcontroller.GetDownloadedTRPDF()");

                }
                return File(FileBytes, "application/pdf", fileName);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "eformController.GetDownloadedTRPDF()");
                string error_File = ConfigurationManager.AppSettings["ERROR_FILE"].ToString();
                string Errorfilename = "Error.pdf";
                byte[] FileBytes = System.IO.File.ReadAllBytes(error_File);
                return File(FileBytes, "application/pdf", Errorfilename);
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        [RequiredAuthentication]
        public ActionResult UploadTRPdfFile(string id)
        {

            string id1 = id;
            IPdfDataDAL objDAL = new PdfDataDAL();
            bool flag = true;
            string responseMessage = string.Empty;
            string fname;
            string uploaded_File, temp_uploaded_File;
            List<SelectListItem> result = new List<SelectListItem>();

            PMGSYEntities eformdbContext = new PMGSYEntities();
            string schdCode = id1.Split('$')[1];
            string schdMonth = id1.Split('$')[2];
            string schdYear = id1.Split('$')[3];
            string workStatus = id1.Split('$')[4];
            string qmCode = id1.Split('$')[5];
            string qmType = id1.Split('$')[6];
            id = id1.Split('$')[0];
            string eformIDFromGrid = id1.Split('$')[7];
            int RoadId = Convert.ToInt32(id);
            int eformIdTemp = Convert.ToInt32(eformIDFromGrid);
            //   string qmFinStatus = eformdbContext.EFORM_PDF_UPLOAD_DETAIL.Where(s => s.PR_ROAD_CODE == RoadId && s.EFORM_ID == eformIdTemp && s.USER_TYPE == "Q").Select(x => x.IS_FINALISED).FirstOrDefault();


            if (!eformdbContext.EFORM_PDF_UPLOAD_DETAIL.Any(s => s.PR_ROAD_CODE == RoadId && s.EFORM_ID == eformIdTemp && s.USER_TYPE == "P" && s.IS_FINALISED == "Y"))
            {
                List<SelectListItem> validationList1 = new List<SelectListItem>();
                flag = false;
                responseMessage = "Unable to upload Test Report e-Form, As PIU is yet to be finalized Eform part-I e-Form";
                validationList1.Add(new SelectListItem { Text = "Unable to upload Test Report e-Form, As PIU is yet to be finalized Eform part-I e-Form" });
                var validationData1 = validationList1.Select(x => x.Text).ToList();
                flag = false;
                return Json(new { success = flag, data = validationData1, responseMsg = responseMessage });
            }


            if (Request.Files.Count > 0)
            {
                HttpPostedFileBase file = Request.Files[0];
                List<SelectListItem> validationList = new List<SelectListItem>();
                //add more conditions like file type, file size etc as per your need.
                int fileSizeLimit = Convert.ToInt32(ConfigurationManager.AppSettings["TEST_REPORT_UPLOADED_FILE_SIZE"]) * 1024 * 1024;
                if (file.ContentLength < fileSizeLimit)
                {
                    if (file != null && file.ContentLength > 0 && Path.GetExtension(file.FileName).ToLower() == ".pdf")
                    {

                        try
                        {


                            fname = eformIDFromGrid + "_$" + qmType + "TR_uploaded" + System.DateTime.Now.ToString("dd_MM_yyyy_HHmmss") + ".pdf";
                            String temp_Uploaded_Path = ConfigurationManager.AppSettings["TEMPORARY_FILES_PATH"].ToString();
                            flag = true;
                            if (!Directory.Exists(temp_Uploaded_Path))
                                Directory.CreateDirectory(temp_Uploaded_Path);
                            temp_uploaded_File = Path.Combine(temp_Uploaded_Path, fname);
                            file.SaveAs(temp_uploaded_File);

                            String Uploaded_Path = ConfigurationManager.AppSettings["TEST_REPORT_UPLOADED_PATH"].ToString();
                            if (!Directory.Exists(Uploaded_Path))
                                Directory.CreateDirectory(Uploaded_Path);
                            uploaded_File = Path.Combine(Uploaded_Path, fname);


                            PdfLoadedDocument loadedDocument = new PdfLoadedDocument(temp_uploaded_File);
                            PdfLoadedForm loadedForm = loadedDocument.Form;

                            try
                            {
                                PdfLoadedTextBoxField RoadCode1 = loadedForm.Fields["RoadCode"] as PdfLoadedTextBoxField;
                            }
                            catch (Exception e)
                            {
                                var roadAlert = "";

                                roadAlert = "Please Fill the downloaded Test report e-Form Pdf in adobe acrobat reader dc and then upload.";
                                validationList.Add(new SelectListItem { Text = roadAlert });
                                var validationData1 = validationList.Select(x => x.Text).ToList();
                                flag = false;
                                ErrorLog.LogError(e, "eformController.UploadTRPdfFile()");
                                return Json(new { success = flag, data = validationData1, responseMsg = roadAlert });

                            }
                            PdfLoadedTextBoxField RoadCode = loadedForm.Fields["RoadCode"] as PdfLoadedTextBoxField;
                            PdfLoadedTextBoxField checksum = loadedForm.Fields["checksum"] as PdfLoadedTextBoxField;
                            string decRoadCode = string.Empty;
                            string decRoadCodetemp = string.Empty;

                            string eformIdfrmPDF = string.Empty;
                            string[] encRoadCode = RoadCode.Text.ToString().Split('/');
                            if (encRoadCode.Length > 1)
                            {
                                decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { encRoadCode[0], encRoadCode[1], encRoadCode[2] });

                                if (decryptedParameters.Count > 0)
                                {
                                    decRoadCodetemp = decryptedParameters["encRoadCode"];
                                    decRoadCode = decRoadCodetemp.Split('$')[0];
                                    eformIdfrmPDF = decRoadCodetemp.Split('$')[1];
                                }
                            }
                            else
                            {
                                decRoadCode = RoadCode.Text;
                            }


                            //fetch template version
                            double templateVers = 0;
                            try
                            {
                                PdfLoadedTextBoxField templateVersion = loadedForm.Fields["TEMPLATE_VERSION"] as PdfLoadedTextBoxField;
                                if (templateVersion.Text != "")
                                {
                                    templateVers = Convert.ToDouble(templateVersion.Text.Replace("V", ""));
                                }

                            }
                            catch (Exception ex)
                            {
                                ErrorLog.LogError(ex, "eformController.UploadTRPdfFile()");
                            }

                            if (!decRoadCode.Equals(id))
                            {

                                var roadAlert = "";
                                var str = eformdbContext.IMS_SANCTIONED_PROJECTS.Where(x => x.IMS_PR_ROAD_CODE == RoadId).Select(u => new { u.IMS_ROAD_NAME }).Single();
                                roadAlert = " Please Upload correct Pdf for road " + str.IMS_ROAD_NAME;

                                validationList.Add(new SelectListItem { Text = roadAlert });

                                var validationData1 = validationList.Select(x => x.Text).ToList();
                                flag = false;
                                return Json(new { success = flag, data = validationData1, responseMsg = roadAlert });
                            }
                            if (!eformIdfrmPDF.Equals(eformIDFromGrid))
                            {
                                string eformIdAlert = " Please Upload correct Pdf for road " + eformIDFromGrid;

                                validationList.Add(new SelectListItem { Text = eformIdAlert });

                                var validationData1 = validationList.Select(x => x.Text).ToList();
                                flag = false;
                                return Json(new { success = flag, data = validationData1, responseMsg = eformIdAlert });
                            }
                            int roadCodeChck = Convert.ToInt32(decRoadCode);
                            int eformId = eformdbContext.EFORM_MASTER.Where(s => s.EFORM_ID == eformIdTemp).Select(m => m.EFORM_ID).FirstOrDefault();
                            string checksumfrmDB = eformdbContext.EFORM_DU_LOG.Where(s => s.EFORM_ID == eformId && (s.USER_TYPE == "T")).OrderByDescending(p => p.LOG_ID).Select(m => m.PDF_CHECKSUM).FirstOrDefault();
                            string RoadStatus = eformdbContext.EFORM_PREFILLED_DETAILS.Where(s => s.EFORM_ID == eformIdTemp).Select(s => s.PHYSICAL_WORK_STATUS).FirstOrDefault();
                            bool TemplateStatus = false;

                            if (workStatus == "C")
                            {
                                TemplateStatus = true;
                            }



                            if (!checksum.Text.Equals(checksumfrmDB))
                            {
                                string roadAlert = "Please upload latest downloaded pdf";
                                validationList.Add(new SelectListItem { Text = "Please upload latest downloaded pdf" });
                                var validationData1 = validationList.Select(x => x.Text).ToList();
                                flag = false;
                                return Json(new { success = flag, data = validationData1, responseMsg = roadAlert });
                            }
                            int schCodeFrmGrid = Convert.ToInt32(schdCode);

                            //if (!eformdbContext.QUALITY_QM_OBSERVATION_MASTER.Any(s => s.IMS_PR_ROAD_CODE == RoadId && s.ADMIN_SCHEDULE_CODE == schCodeFrmGrid))
                            //{
                            //    string qmsAppUploadAlert = "Test Report cant be upload, As obervation is not uploaded on QMS App";
                            //    validationList.Add(new SelectListItem { Text = "Test Report cant be upload, As obervation is not uploaded on QMS App" });
                            //    var validationData1 = validationList.Select(x => x.Text).ToList();
                            //    flag = false;
                            //    return Json(new { success = flag, data = validationData1, responseMsg = qmsAppUploadAlert });
                            //}

                            loadedDocument.Close(true);
                            char[] a1 = new char[55];

                            int countApplYes = 0;
                            for (int i = 1; i <= 54; i++)
                            {
                                string chkbxname = "Is_Applicable_" + i;
                                PdfLoadedCheckBoxField checkfield = loadedForm.Fields[chkbxname] as PdfLoadedCheckBoxField;
                                if (checkfield.Checked)
                                {
                                    a1[i] = 'Y';
                                    countApplYes++;
                                }
                                else
                                {
                                    a1[i] = 'N';
                                }

                            }
                            if (countApplYes == 0)
                            {
                                string allNotApplUploadAlert = "Test Report cant be upload, Please fill the details atleast for one Sheet";
                                validationList.Add(new SelectListItem { Text = "Test Report cant be upload, Please fill the details atleast for one Sheet" });
                                var validationData1 = validationList.Select(x => x.Text).ToList();
                                flag = false;
                                return Json(new { success = flag, data = validationData1, responseMsg = allNotApplUploadAlert });
                            }


                            // EFORM_TR_VIEWMODEL PdfDataToClassTRModel
                            EFORM_TR_VIEWMODEL objViewQMModel = PdfDataToClassTRModel(temp_uploaded_File, TemplateStatus, a1, templateVers);

                            StringBuilder strErrorMessage = new StringBuilder("");
                            int count = 0;
                            if (objViewQMModel.ErrorOccured)
                            {
                                objViewQMModel.ErrorList = objViewQMModel.ErrorList.Distinct().ToList();
                                #region 31-03-2022
                                List<string> errorslist = new List<string>();
                                for (int i = 1; i <= 23; i++)
                                {
                                    string Errorpage = "Page-";


                                    errorslist = objViewQMModel.ErrorList.Where(a => a.Contains(string.Concat(Errorpage + i + ":"))).ToList();
                                    if (errorslist.Count > 0)
                                    {
                                        strErrorMessage.Append("<h4 style='background-color:#efede5; color:black'> Page " + i + " </h4>");
                                    }

                                    foreach (var ierror in errorslist)
                                    {
                                        strErrorMessage.Append("<li style='margin-left:10px; list-style-type: square'> ");
                                        strErrorMessage.Append("Error-" + ++count + " " + ierror);
                                        strErrorMessage.Append("<br />");
                                        strErrorMessage.Append("</li>");
                                    }
                                }
                                #endregion



                                return Json(new { success = false, data = strErrorMessage.ToString(), IsValidationError = true, responseMsg = "Please fill all mandatory details in file" });
                            }
                            //  save to db

                            result = objDAL.SaveToTRDb(temp_uploaded_File, eformIDFromGrid, fname, Uploaded_Path, file, uploaded_File, objViewQMModel, schdCode, schdMonth, schdYear, workStatus, qmCode, qmType, TemplateStatus, a1);
                            var validationData = result.Select(x => x.Text).ToList();

                            if (result.Count > 0)
                            {

                                flag = false;
                                validationList.Add(new SelectListItem { Text = "Error occured while uploading Eform. Please contact OMMAS team " });
                                var validationData1 = validationList.Select(x => x.Text).ToList();
                                return Json(new { success = flag, data = validationData1, responseMsg = "Error occured while uploading Eform. Please contact OMMAS team" });
                            }
                            else
                            {
                                loadedDocument.Close(true);
                                responseMessage = "Upload Successful.";
                            }


                        }
                        catch (Exception ex)
                        {
                            flag = false;
                            ErrorLog.LogError(ex, "eformcontroller.UploadTRPdfFile()");
                            responseMessage = "Error occured while uploading pdf, please contact OMMAS team.";
                            validationList.Add(new SelectListItem { Text = "Error occured while uploading pdf, please contact OMMAS team." });
                            var validationData1 = validationList.Select(x => x.Text).ToList();
                            return Json(new { success = flag, data = validationData1, responseMsg = responseMessage });
                        }
                        finally
                        {
                            file.InputStream.Flush();
                            file.InputStream.Close();
                            eformdbContext.Dispose();
                        }
                    }
                    else
                    {
                        flag = false;
                        responseMessage = "File is invalid.";
                        validationList.Add(new SelectListItem { Text = "File is invalid." });
                        var validationData1 = validationList.Select(x => x.Text).ToList();
                        return Json(new { success = flag, data = validationData1, responseMsg = responseMessage });
                    }
                }
                else
                {
                    flag = false;
                    responseMessage = "Invalid file size. Please upload file upto 15 MB..";
                    validationList.Add(new SelectListItem { Text = "Invalid file size. Please upload file upto 15 MB." });
                    var validationData1 = validationList.Select(x => x.Text).ToList();
                    return Json(new { success = flag, data = validationData1, responseMsg = responseMessage });
                }
            }
            else
            {
                flag = false;
                responseMessage = "File Upload has no file.";
            }

            return Json(new { success = flag, message = responseMessage }, JsonRequestBehavior.AllowGet);
        }

        [RequiredAuthentication]
        public JsonResult IsTRPDFFileavaialble(string id)
        {
            bool flag = false;
            string message = string.Empty;
            PMGSYEntities dbContext = new PMGSYEntities();
            try
            {

                int eformId = Convert.ToInt32(id.Split('$')[7]);
                var fileName = dbContext.EFORM_TEST_REPORT_FILE.Where(x => x.EFORM_ID == eformId && x.IS_EFORM == "Y").Select(s => s.FILE_NAME).FirstOrDefault();
                string fullPath = string.Empty;
                if (fileName == null)
                {

                    return Json(new { success = flag }, JsonRequestBehavior.AllowGet);

                }
                else
                {
                    flag = true;
                    message = "Pdf is already uploaded. If you want to upload again, please delete the existing file";

                }

            }
            catch (Exception ex)
            {
                flag = true;
                ErrorLog.LogError(ex, "eformcontroller.IsTRPDFFileavaialble()");
                message = "Error occured during uploading file";
                return Json(new { success = flag }, JsonRequestBehavior.AllowGet);
            }
            finally
            {
                dbContext.Dispose();
            }

            return Json(new { success = flag, message = message }, JsonRequestBehavior.AllowGet);
        }

        [RequiredAuthentication]
        public JsonResult viewTRPdfVirtualDir(string id)
        {
            PMGSYEntities dbContext = new PMGSYEntities();

            try
            {

                int eformId = Convert.ToInt32(id);
                EFORM_MASTER model = dbContext.EFORM_MASTER.Where(s => s.EFORM_ID == eformId).FirstOrDefault();
                string uploaded_path = string.Empty;
                EFORM_TEST_REPORT_FILE modelTestReportUploadFile = dbContext.EFORM_TEST_REPORT_FILE.Where(x => x.EFORM_ID == eformId && x.IS_EFORM == "Y").FirstOrDefault();
                string VirtualDirectoryPath = ConfigurationManager.AppSettings["TEST_REPORT_UPLOADED_EFORM_VIRTUAL_DIR_PATH"].ToString();
                string VirtualDirectoryfullPath = string.Empty;
                var fileName = dbContext.EFORM_TEST_REPORT_FILE.Where(x => x.EFORM_ID == eformId && x.IS_EFORM == "Y").Select(s => s.FILE_NAME).FirstOrDefault();

                //if (modelTestReportUploadFile.TEMPLATE_VERSION==null)
                //{
                //    uploaded_path = ConfigurationManager.AppSettings["TEST_REPORT_UPLOADED_PATH"].ToString();
                //      VirtualDirectoryfullPath = Path.Combine(VirtualDirectoryPath, fileName.ToString()).ToString().Replace(@"\\", @"//").Replace(@"\", @"/");

                //}
                //else
                //{
                //    uploaded_path = ConfigurationManager.AppSettings["TEST_REPORT_UPLOADED_PATH"].ToString() + "/" + model.ADMIN_SCHEDULE_YEAR.ToString() + "/" + model.ADMIN_SCHEDULE_MONTH.ToString();
                //      VirtualDirectoryfullPath = Path.Combine(VirtualDirectoryPath, Convert.ToString(model.ADMIN_SCHEDULE_YEAR.ToString() + "/" + model.ADMIN_SCHEDULE_MONTH.ToString()), fileName.ToString()).ToString().Replace(@"\\", @"//").Replace(@"\", @"/");

                //}
                //if (fileName.Contains('$'))
                //{
                //    uploaded_path = ConfigurationManager.AppSettings["TEST_REPORT_UPLOADED_PATH"].ToString() + "/" + model.ADMIN_SCHEDULE_YEAR.ToString() + "/" + model.ADMIN_SCHEDULE_MONTH.ToString();
                //    VirtualDirectoryfullPath = Path.Combine(VirtualDirectoryPath, Convert.ToString(model.ADMIN_SCHEDULE_YEAR.ToString() + "/" + model.ADMIN_SCHEDULE_MONTH.ToString()), fileName.ToString()).ToString().Replace(@"\\", @"//").Replace(@"\", @"/");
                //}
                //else
                //{
                //    uploaded_path = ConfigurationManager.AppSettings["TEST_REPORT_UPLOADED_PATH"].ToString();
                //    VirtualDirectoryfullPath = Path.Combine(VirtualDirectoryPath, fileName.ToString()).ToString().Replace(@"\\", @"//").Replace(@"\", @"/");
                //}



                uploaded_path = ConfigurationManager.AppSettings["TEST_REPORT_UPLOADED_PATH"].ToString() + "/" + model.ADMIN_SCHEDULE_YEAR.ToString() + "/" + model.ADMIN_SCHEDULE_MONTH.ToString();
                VirtualDirectoryfullPath = Path.Combine(VirtualDirectoryPath, Convert.ToString(model.ADMIN_SCHEDULE_YEAR.ToString() + "/" + model.ADMIN_SCHEDULE_MONTH.ToString()), fileName.ToString()).ToString().Replace(@"\\", @"//").Replace(@"\", @"/");

                string physicalFullPath = Path.Combine(uploaded_path, fileName);

                FileInfo file = new FileInfo(physicalFullPath);
                if (file.Exists)
                {
                    return Json(new { success = true, Message = VirtualDirectoryfullPath }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { success = false, Message = "File not found" }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "eformcontroller.viewTRPdfVirtualDir()");
                return null;
            }
            finally
            {
                dbContext.Dispose();
            }

        }

        [HttpGet]
        public ActionResult TestReportPreview(string encIdtemp)
        {
            IPdfDataDAL objDAL = new PdfDataDAL();
            try
            {

                string[] encParam = encIdtemp.Split('/');
                decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { encParam[0], encParam[1], encParam[2] });
                int EformId = 0;
                if (decryptedParameters.Count > 0)
                {
                    EformId = Convert.ToInt32(decryptedParameters["imsRoadID"].Split('$')[0]);
                }
                EFORM_TR_PREVIEW_MODEL eformTrViewModel = objDAL.GetTestReportsPreviewDetails(EformId);

                return View("~/Areas/EFORMArea/Views/EFORM/TestReportPreview.cshtml", eformTrViewModel);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "EFORMController.TestReportPreview()");
                return View("~/Areas/EFORMArea/Views/EFORM/PDF_ErrorPage");
            }

        }

        [RequiredAuthentication]
        public JsonResult deleteTREformDetail(string id)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            string roadIDdec = id.Split('$')[0];
            string eformIddec = id.Split('$')[1];
            int roadId = Convert.ToInt32(roadIDdec);
            string fileName = string.Empty;
            string Uploaded_Path_QM = string.Empty;

            try
            {
                int eformId = Convert.ToInt32(eformIddec);
                EFORM_TEST_REPORT_FILE modelTestReportUploadFile = dbContext.EFORM_TEST_REPORT_FILE.Where(x => x.EFORM_ID == eformId && x.IS_EFORM == "Y").FirstOrDefault();
                EFORM_MASTER model = dbContext.EFORM_MASTER.Where(s => s.EFORM_ID == eformId).FirstOrDefault();

                using (var scope = new TransactionScope())
                {
                    fileName = dbContext.EFORM_TEST_REPORT_FILE.Where(x => x.EFORM_ID == eformId && x.IS_EFORM == "Y").Select(s => s.FILE_NAME).FirstOrDefault();
                    if (modelTestReportUploadFile.TEMPLATE_VERSION == null)
                    {
                        Uploaded_Path_QM = ConfigurationManager.AppSettings["TEST_REPORT_UPLOADED_PATH"].ToString();

                    }
                    else
                    {
                        Uploaded_Path_QM = ConfigurationManager.AppSettings["TEST_REPORT_UPLOADED_PATH"].ToString() + "/" + model.ADMIN_SCHEDULE_YEAR.ToString() + "/" + model.ADMIN_SCHEDULE_MONTH.ToString();

                    }

                    if (fileName == null)
                    {
                        return Json(new { success = false, message = "error occured during deleting the pdf." }, JsonRequestBehavior.AllowGet);
                    }
                    dbContext.USP_EFORMS_TR_DELETE_INSPECTION_DETAILS(roadId, eformId);
                    dbContext.SaveChanges();
                    string fullPathTR = Path.Combine(Uploaded_Path_QM, fileName);
                    FileInfo fileTR = new FileInfo(fullPathTR);
                    if (fileTR.Exists)
                    {
                        fileTR.Delete();
                    }
                    scope.Complete();
                    return Json(new { success = true }, JsonRequestBehavior.AllowGet);
                }

            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "eformcontroller.deleteTREformDetail()");
                return Json(new { success = false, message = "error occured during deleting the pdf." }, JsonRequestBehavior.AllowGet);
            }
            finally
            {
                dbContext.Dispose();
            }

        }

        public JsonResult finalizeETestReport(string id)
        {
            bool flag = true;
            PMGSYEntities dbContext = new PMGSYEntities();
            try
            {
                int detailId = Convert.ToInt32(id);

                var TRfile_Status = dbContext.EFORM_TEST_REPORT_FILE.Where(x => x.EFORM_ID == detailId && x.IS_EFORM == "Y").Select(x => x.FILE_NAME).FirstOrDefault();
                var isTRfileupload = "N";
                if (TRfile_Status != null)
                {
                    isTRfileupload = "Y";
                }
                if (isTRfileupload != "Y")
                {
                    flag = false;
                    string responseMessage = "Unable to finalize Test Report,as pdf is not uploaded yet";
                    return Json(new { success = flag, responseMsg = responseMessage });
                }
                using (TransactionScope ts = new TransactionScope())
                {

                    EFORM_TEST_REPORT_FILE obj = dbContext.EFORM_TEST_REPORT_FILE.Where(s => s.EFORM_ID == detailId && s.IS_EFORM == "Y").FirstOrDefault();
                    obj.IS_FINALISED = "Y";
                    dbContext.Entry(obj).State = EntityState.Modified;
                    dbContext.SaveChanges();
                    ts.Complete();

                }
                return Json(new { success = flag }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                flag = false;
                ErrorLog.LogError(ex, "eformController.finalizeETestReport()");
                return Json(new { success = flag }, JsonRequestBehavior.AllowGet);
            }
            finally
            {
                dbContext.Dispose();
            }

        }

        [RequiredAuthentication]
        public EFORM_TR_VIEWMODEL PdfDataToClassTRModel(string filePath, bool TemplateStatus, char[] naCheckArr, double tempVersion)
        {
            PdfLoadedDocument loadedDocument = new PdfLoadedDocument(filePath);
            PMGSYEntities dbContext = new PMGSYEntities();
            try
            {
                PDFFiledDataInspectorModel pDFFiledDataInspectorModel = new PDFFiledDataInspectorModel();
                PdfLoadedForm loadedForm = loadedDocument.Form;
                int eformIdtemp = 0;
                try
                {
                    PdfLoadedTextBoxField eformId = loadedForm.Fields["EFORM_ID"] as PdfLoadedTextBoxField;
                    eformIdtemp = Convert.ToInt32(eformId.Text);
                }
                catch (Exception ex)
                {
                    ErrorLog.LogError(ex, "eformComtroller.PdfDataToClassTRModel()");
                }

                EFORM_TR_VIEWMODEL TRobjViewModel = new EFORM_TR_VIEWMODEL();
                CommonFunctions objCommonFunction = new CommonFunctions();


                List<string> errorListtemp = new List<string>();

                TRobjViewModel.ErrorList = new List<string>();


                //bool isCH1Mand = false;
                //bool isCH2Mand = false;
                //bool isCH3Mand = false;
                //Decimal chainageValue = 0;
                EFORM_MASTER eformModel = dbContext.EFORM_MASTER.Where(s => s.EFORM_ID == eformIdtemp && s.IS_VALID == "Y").FirstOrDefault();
                //string endChanage = dbContext.QUALITY_QM_OBSERVATION_MASTER.Where(s => s.IMS_PR_ROAD_CODE == eformModel.IMS_PR_ROAD_CODE && s.ADMIN_SCHEDULE_CODE == eformModel.ADMIN_SCHEDULE_CODE).Select(m => m.QM_INSPECTED_END_CHAINAGE).FirstOrDefault().ToString();
                //string startChanage = dbContext.QUALITY_QM_OBSERVATION_MASTER.Where(s => s.IMS_PR_ROAD_CODE == eformModel.IMS_PR_ROAD_CODE && s.ADMIN_SCHEDULE_CODE == eformModel.ADMIN_SCHEDULE_CODE).Select(m => m.QM_INSPECTED_START_CHAINAGE).FirstOrDefault().ToString();

                //if (endChanage != null && startChanage != null)
                //{
                //    chainageValue = Convert.ToDecimal(endChanage) - Convert.ToDecimal(startChanage);
                //}
                //if (chainageValue <= 1)
                //{
                //    isCH1Mand = true;
                //}
                //if (chainageValue > 1 && chainageValue <= 2)
                //{
                //    isCH1Mand = true;
                //    isCH2Mand = true;
                //}
                //if (chainageValue > 2)
                //{
                //    isCH1Mand = true;
                //    isCH2Mand = true;
                //    isCH3Mand = true;
                //}



                #region -----MODEL DEFINATION: Vikky Type-A pages:1,5,17,9,11,15,16,18,19,22-------
                TRobjViewModel.MAIN_ITEM_OPTIONS_SELECTED_DETAIL_List = new List<TestReportModel.EFORM_TR_MAIN_ITEM_OPTIONS_SELECTED_DETAIL>();
                TRobjViewModel.SUBITEM_OPTIONS_SELECTED_DETAIL_List = new List<TestReportModel.EFORM_TR_SUBITEM_OPTIONS_SELECTED_DETAIL>();
                TRobjViewModel.TYPEA_SUMMARY_PAGE1_SRM_1 = new EFORM_TR_TYPEA_SUMMARY_PAGE1_SRM_1();
                TRobjViewModel.TYPEA_DETAIL_PAGE1_SRM_1_List = new List<TestReportModel.EFORM_TR_TYPEA_DETAIL_PAGE1_SRM_1>();
                TRobjViewModel.TYPEA_SUMMARY_PAGE1_CCM_2 = new EFORM_TR_TYPEA_SUMMARY_PAGE1_CCM_2();
                TRobjViewModel.TYPEA_DETAIL_PAGE1_CCM_2_List = new List<TestReportModel.EFORM_TR_TYPEA_DETAIL_PAGE1_CCM_2>();

                TRobjViewModel.TYPEA_SUMMARY_PAGE5_SRM_12 = new EFORM_TR_TYPEA_SUMMARY_PAGE5_SRM_12();
                TRobjViewModel.TYPEA_DETAIL_PAGE5_SRM_12_List = new List<TestReportModel.EFORM_TR_TYPEA_DETAIL_PAGE5_SRM_12>();

                TRobjViewModel.TYPEA_SUMMARY_PAGE7_SRM_16 = new EFORM_TR_TYPEA_SUMMARY_PAGE7_SRM_16();
                TRobjViewModel.TYPEA_DETAIL_PAGE7_SRM_16_List = new List<TestReportModel.EFORM_TR_TYPEA_DETAIL_PAGE7_SRM_16>();

                TRobjViewModel.TYPEA_SUMMARY_PAGE7_SRM_17 = new EFORM_TR_TYPEA_SUMMARY_PAGE7_SRM_17();
                TRobjViewModel.TYPEA_DETAIL_PAGE7_SRM_17_List = new List<TestReportModel.EFORM_TR_TYPEA_DETAIL_PAGE7_SRM_17>();

                TRobjViewModel.TYPEA_SUMMARY_PAGE9_SRM_21 = new EFORM_TR_TYPEA_SUMMARY_PAGE9_SRM_21();
                TRobjViewModel.TYPEA_DETAIL_PAGE9_SRM_21_List = new List<TestReportModel.EFORM_TR_TYPEA_DETAIL_PAGE9_SRM_21>();

                TRobjViewModel.TYPEA_SUMMARY_PAGE9_SRM_22 = new EFORM_TR_TYPEA_SUMMARY_PAGE9_SRM_22();
                TRobjViewModel.TYPEA_DETAIL_PAGE9_SRM_22_List = new List<TestReportModel.EFORM_TR_TYPEA_DETAIL_PAGE9_SRM_22>();

                TRobjViewModel.TYPEA_SUMMARY_PAGE11_SRM_26 = new EFORM_TR_TYPEA_SUMMARY_PAGE11_SRM_26();
                TRobjViewModel.TYPEA_DETAIL_PAGE11_SRM_26_List = new List<TestReportModel.EFORM_TR_TYPEA_DETAIL_PAGE11_SRM_26>();

                TRobjViewModel.TYPEA_SUMMARY_PAGE11_SRM_27 = new EFORM_TR_TYPEA_SUMMARY_PAGE11_SRM_27();
                TRobjViewModel.TYPEA_DETAIL_PAGE11_SRM_27_List = new List<TestReportModel.EFORM_TR_TYPEA_DETAIL_PAGE11_SRM_27>();

                TRobjViewModel.TYPEA_SUMMARY_PAGE15_SRM_37 = new EFORM_TR_TYPEA_SUMMARY_PAGE15_SRM_37();
                TRobjViewModel.TYPEA_DETAIL_PAGE15_SRM_37_List = new List<TestReportModel.EFORM_TR_TYPEA_DETAIL_PAGE15_SRM_37>();

                TRobjViewModel.TYPEA_SUMMARY_PAGE16_CCM_38 = new EFORM_TR_TYPEA_SUMMARY_PAGE16_CCM_38();
                TRobjViewModel.TYPEA_DETAIL_PAGE16_CCM_38_List = new List<TestReportModel.EFORM_TR_TYPEA_DETAIL_PAGE16_CCM_38>();

                TRobjViewModel.TYPEA_SUMMARY_PAGE16_SRM_39 = new EFORM_TR_TYPEA_SUMMARY_PAGE16_SRM_39();
                TRobjViewModel.TYPEA_DETAIL_PAGE16_SRM_39_List = new List<TestReportModel.EFORM_TR_TYPEA_DETAIL_PAGE16_SRM_39>();

                TRobjViewModel.TYPEA_SUMMARY_PAGE18_SRM_43 = new EFORM_TR_TYPEA_SUMMARY_PAGE18_SRM_43();
                TRobjViewModel.TYPEA_DETAIL_PAGE18_SRM_43_List = new List<TestReportModel.EFORM_TR_TYPEA_DETAIL_PAGE18_SRM_43>();

                TRobjViewModel.TYPEA_SUMMARY_PAGE19_SRM_44 = new EFORM_TR_TYPEA_SUMMARY_PAGE19_SRM_44();
                TRobjViewModel.TYPEA_DETAIL_PAGE19_SRM_44_List = new List<TestReportModel.EFORM_TR_TYPEA_DETAIL_PAGE19_SRM_44>();

                TRobjViewModel.TYPEA_SUMMARY_PAGE19_CCM_45 = new EFORM_TR_TYPEA_SUMMARY_PAGE19_CCM_45();
                TRobjViewModel.TYPEA_DETAIL_PAGE19_CCM_45_List = new List<TestReportModel.EFORM_TR_TYPEA_DETAIL_PAGE19_CCM_45>();

                TRobjViewModel.TYPEA_SUMMARY_PAGE22_SRM_52 = new EFORM_TR_TYPEA_SUMMARY_PAGE22_SRM_52();
                TRobjViewModel.TYPEA_DETAIL_PAGE22_SRM_52_List = new List<TestReportModel.EFORM_TR_TYPEA_DETAIL_PAGE22_SRM_52>();

                TRobjViewModel.TYPEA_SUMMARY_PAGE22_SRM_53 = new EFORM_TR_TYPEA_SUMMARY_PAGE22_SRM_53();

                TRobjViewModel.UCS_TEST_DETAIL_PAGE22_SRM_53_List = new List<TestReportModel.EFORM_TR_UCS_TEST_DETAIL_PAGE22_SRM_53>();

                TRobjViewModel.TYPEA_SUMMARY_PAGE23_SRM_54 = new EFORM_TR_TYPEA_SUMMARY_PAGE23_SRM_54();
                TRobjViewModel.TYPEA_DETAIL_PAGE23_SRM_54_List = new List<TestReportModel.EFORM_TR_TYPEA_DETAIL_PAGE23_SRM_54>();

                TypeASummaryPage1SRM1AssignedModel typeASummaryPage1SRM1AssignedModel = new TypeASummaryPage1SRM1AssignedModel();
                TypeASummaryPage1CCM2AssignedModel typeASummaryPage1CCM2AssignedModel = new TypeASummaryPage1CCM2AssignedModel();
                TypeASummaryPage5SRM12AssignedModel typeASummaryPage5SRM12AssignedModel = new TypeASummaryPage5SRM12AssignedModel();
                TypeASummaryPage7SRM16AssignedModel typeASummaryPage7SRM16AssignedModel = new TypeASummaryPage7SRM16AssignedModel();
                TypeASummaryPage7SRM17AssignedModel typeASummaryPage7SRM17AssignedModel = new TypeASummaryPage7SRM17AssignedModel();
                TypeASummaryPage9SRM21AssignedModel typeASummaryPage9SRM21AssignedModel = new TypeASummaryPage9SRM21AssignedModel();
                TypeASummaryPage9SRM22AssignedModel typeASummaryPage9SRM22AssignedModel = new TypeASummaryPage9SRM22AssignedModel();
                TypeASummaryPage11SRM26AssignedModel typeASummaryPage11SRM26AssignedModel = new TypeASummaryPage11SRM26AssignedModel();
                TypeASummaryPage11SRM27AssignedModel typeASummaryPage11SRM27AssignedModel = new TypeASummaryPage11SRM27AssignedModel();
                TypeASummaryPage15SRM37AssignedModel typeASummaryPage15SRM37AssignedModel = new TypeASummaryPage15SRM37AssignedModel();
                TypeASummaryPage15CCM38AssignedModel typeASummaryPage15CCM38AssignedModel = new TypeASummaryPage15CCM38AssignedModel();
                TypeASummaryPage16SRM39AssignedModel typeASummaryPage16SRM39AssignedModel = new TypeASummaryPage16SRM39AssignedModel();
                TypeASummaryPage18SRM43AssignedModel typeASummaryPage18SRM43AssignedModel = new TypeASummaryPage18SRM43AssignedModel();
                TypeASummaryPage19SRM44AssignedModel typeASummaryPage19SRM44AssignedModel = new TypeASummaryPage19SRM44AssignedModel();
                TypeASummaryPage19CCM45AssignedModel typeASummaryPage19CCM45AssignedModel = new TypeASummaryPage19CCM45AssignedModel();
                TypeASummaryPage22SRM52AssignedModel typeASummaryPage22SRM52AssignedModel = new TypeASummaryPage22SRM52AssignedModel();
                TypeASummaryPage22SRM53AssignedModel typeASummaryPage22SRM53AssignedModel = new TypeASummaryPage22SRM53AssignedModel();
                TypeASummaryPage22SRM54AssignedModel typeASummaryPage22SRM54AssignedModel = new TypeASummaryPage22SRM54AssignedModel();
                #endregion

                #region e-Form Test Report Model Initialize Region

                #region e-Form Test Report Page-2 Granular SubBase InIt

                //  e-Form Test Report Page-2 SUMMARY Granular SubBase
                TRobjViewModel.TYPEB_SUMMARY_PAGE2_GS_BASE_1 = new EFORM_TR_TYPEB_SUMMARY_PAGE2_GS_BASE_1();
                TRobjViewModel.TYPEB_SUMMARY_PAGE2_GS_BASE_2 = new EFORM_TR_TYPEB_SUMMARY_PAGE2_GS_BASE_2();
                TRobjViewModel.TYPEB_SUMMARY_PAGE2_GS_BASE_3 = new EFORM_TR_TYPEB_SUMMARY_PAGE2_GS_BASE_3();
                //  e-Form Test Report Page-2 DETAIL Granular SubBase
                TRobjViewModel.TYPEB_DETAIL_PAGE2_GS_BASE_1_List = new List<EFORM_TR_TYPEB_DETAIL_PAGE2_GS_BASE_1>();
                TRobjViewModel.TYPEB_DETAIL_PAGE2_GS_BASE_2_List = new List<EFORM_TR_TYPEB_DETAIL_PAGE2_GS_BASE_2>();
                TRobjViewModel.TYPEB_DETAIL_PAGE2_GS_BASE_3_List = new List<EFORM_TR_TYPEB_DETAIL_PAGE2_GS_BASE_3>();
                TypeBSummaryPage2GranSubBase1AssignModel typeBSummaryPage2GranSub1 = new TypeBSummaryPage2GranSubBase1AssignModel();
                TypeBSummaryPage2GranSubBase2AssignModel typeBSummaryPage2GranSub2 = new TypeBSummaryPage2GranSubBase2AssignModel();
                TypeBSummaryPage2GranSubBase3AssignModel typeBSummaryPage2GranSub3 = new TypeBSummaryPage2GranSubBase3AssignModel();

                #endregion

                #region e-Form Test Report Page-3 Gravel SubBase InIt
                //  e-Form Test Report Page-3 SUMMARY Gravel Subbase 
                TRobjViewModel.TYPEB_SUMMARY_PAGE3_GRAVEL_SUBBASE_1 = new EFORM_TR_TYPEB_SUMMARY_PAGE3_GRAVEL_SUBBASE_1();
                TRobjViewModel.TYPEB_SUMMARY_PAGE3_GRAVEL_SUBBASE_2 = new EFORM_TR_TYPEB_SUMMARY_PAGE3_GRAVEL_SUBBASE_2();
                TRobjViewModel.TYPEB_SUMMARY_PAGE3_GRAVEL_SUBBASE_3 = new EFORM_TR_TYPEB_SUMMARY_PAGE3_GRAVEL_SUBBASE_3();
                //  e-Form Test Report Page-3 DETAIL Gravel Subbase 
                TRobjViewModel.TYPEB_DETAIL_PAGE3_GRAVEL_SUBBASE_1_List = new List<EFORM_TR_TYPEB_DETAIL_PAGE3_GRAVEL_SUBBASE_1>();
                TRobjViewModel.TYPEB_DETAIL_PAGE3_GRAVEL_SUBBASE_2_List = new List<EFORM_TR_TYPEB_DETAIL_PAGE3_GRAVEL_SUBBASE_2>();
                TRobjViewModel.TYPEB_DETAIL_PAGE3_GRAVEL_SUBBASE_3_List = new List<EFORM_TR_TYPEB_DETAIL_PAGE3_GRAVEL_SUBBASE_3>();
                TypeBSummaryPage3GravelSubBase1AssignModel typeBSummaryPage3GravelSubBase1 = new TypeBSummaryPage3GravelSubBase1AssignModel();
                TypeBSummaryPage3GravelSubBase2AssignModel typeBSummaryPage3GravelSubBase2 = new TypeBSummaryPage3GravelSubBase2AssignModel();
                TypeBSummaryPage3GravelSubBase3AssignModel typeBSummaryPage3GravelSubBase3 = new TypeBSummaryPage3GravelSubBase3AssignModel();
                #endregion

                #region e-Form Test Report Page-4 Cement Stabilised Subbase InIt
                //  e-Form Test Report Page-4 SUMMARY Cement Stabilised 
                TRobjViewModel.TYPEB_SUMMARY_PAGE4_CS_SUBBASE_1 = new EFORM_TR_TYPEB_SUMMARY_PAGE4_CS_SUBBASE_1();
                TRobjViewModel.TYPEB_SUMMARY_PAGE4_CS_SUBBASE_2 = new EFORM_TR_TYPEB_SUMMARY_PAGE4_CS_SUBBASE_2();
                TRobjViewModel.TYPEB_SUMMARY_PAGE4_CS_SUBBASE_3 = new EFORM_TR_TYPEB_SUMMARY_PAGE4_CS_SUBBASE_3();
                //  e-Form Test Report Page-4 DETAIL Cement Stabilised 
                TRobjViewModel.TYPEB_DETAIL_PAGE4_CS_SUBBASE_1_List = new List<EFORM_TR_TYPEB_DETAIL_PAGE4_CS_SUBBASE_1>();
                TRobjViewModel.TYPEB_DETAIL_PAGE4_CS_SUBBASE_2_List = new List<EFORM_TR_TYPEB_DETAIL_PAGE4_CS_SUBBASE_2>();
                TRobjViewModel.TYPEB_DETAIL_PAGE4_CS_SUBBASE_3_List = new List<EFORM_TR_TYPEB_DETAIL_PAGE4_CS_SUBBASE_3>();
                TypeBSummaryPage4CementSubBase1AssignModel typeBSummaryPage4CementSubBase1 = new TypeBSummaryPage4CementSubBase1AssignModel();
                TypeBSummaryPage4CementSubBase2AssignModel typeBSummaryPage4CementSubBase2 = new TypeBSummaryPage4CementSubBase2AssignModel();
                TypeBSummaryPage4CementSubBase3AssignModel typeBSummaryPage4CementSubBase3 = new TypeBSummaryPage4CementSubBase3AssignModel();
                #endregion

                #region e-Form Test Report Page - 6 BASE COURSE: 1st Layer InIt

                //  e-Form Test Report Page-6 SUMMARY
                TypeBSummaryPage6BaseCourse13AssignModel typeBSummaryPage6BaseCourse13AssignModel = new TypeBSummaryPage6BaseCourse13AssignModel();
                TypeBSummaryPage6BaseCourse14AssignModel typeBSummaryPage6BaseCourse14AssignModel = new TypeBSummaryPage6BaseCourse14AssignModel();
                TypeBSummaryPage6BaseCourse15AssignModel typeBSummaryPage6BaseCourse15AssignModel = new TypeBSummaryPage6BaseCourse15AssignModel();

                TRobjViewModel.TYPEB_SUMMARY_PAGE6_BASE_COURSE_1 = new EFORM_TR_TYPEB_SUMMARY_PAGE6_BASE_COURSE_1();
                TRobjViewModel.TYPEB_SUMMARY_PAGE6_BASE_COURSE_2 = new EFORM_TR_TYPEB_SUMMARY_PAGE6_BASE_COURSE_2();
                TRobjViewModel.TYPEB_SUMMARY_PAGE6_BASE_COURSE_3 = new EFORM_TR_TYPEB_SUMMARY_PAGE6_BASE_COURSE_3();
                //  e-Form Test Report Page-6 DETAIL
                TRobjViewModel.TYPEB_DETAIL_PAGE6_BASE_COURSE_1_List = new List<EFORM_TR_TYPEB_DETAIL_PAGE6_BASE_COURSE_1>();
                TRobjViewModel.TYPEB_DETAIL_PAGE6_BASE_COURSE_2_List = new List<EFORM_TR_TYPEB_DETAIL_PAGE6_BASE_COURSE_2>();
                TRobjViewModel.TYPEB_DETAIL_PAGE6_BASE_COURSE_3_List = new List<EFORM_TR_TYPEB_DETAIL_PAGE6_BASE_COURSE_3>();

                #endregion

                #region e-Form Test Report Page - 8 BASE COURSE: 2nd Layer InIt
                TypeBSummaryPage8BaseCourse18AssignModel typeBSummaryPage8BaseCourse18AssignModel = new TypeBSummaryPage8BaseCourse18AssignModel();
                TypeBSummaryPage8BaseCourse19AssignModel typeBSummaryPage8BaseCourse19AssignModel = new TypeBSummaryPage8BaseCourse19AssignModel();
                TypeBSummaryPage8BaseCourse20AssignModel typeBSummaryPage8BaseCourse20AssignModel = new TypeBSummaryPage8BaseCourse20AssignModel();
                //  e-Form Test Report Page-8 SUMMARY
                TRobjViewModel.TYPEB_SUMMARY_PAGE8_BASE_COURSE2_1 = new EFORM_TR_TYPEB_SUMMARY_PAGE8_BASE_COURSE2_1();
                TRobjViewModel.TYPEB_SUMMARY_PAGE8_BASE_COURSE2_2 = new EFORM_TR_TYPEB_SUMMARY_PAGE8_BASE_COURSE2_2();
                TRobjViewModel.TYPEB_SUMMARY_PAGE8_BASE_COURSE2_3 = new EFORM_TR_TYPEB_SUMMARY_PAGE8_BASE_COURSE2_3();
                //  e-Form Test Report Page-8 DETAIL
                TRobjViewModel.TYPEB_DETAIL_PAGE8_BASE_COURSE2_1_List = new List<EFORM_TR_TYPEB_DETAIL_PAGE8_BASE_COURSE2_1>();
                TRobjViewModel.TYPEB_DETAIL_PAGE8_BASE_COURSE2_2_List = new List<EFORM_TR_TYPEB_DETAIL_PAGE8_BASE_COURSE2_2>();
                TRobjViewModel.TYPEB_DETAIL_PAGE8_BASE_COURSE2_3_List = new List<EFORM_TR_TYPEB_DETAIL_PAGE8_BASE_COURSE2_3>();

                #endregion

                #region  e-Form Test Report Page - 10 BASE COURSE: 3rd Layer InIt

                TypeBSummaryPage10BaseCourse23AssignModel typeBSummaryPage10BaseCourse23AssignModel = new TypeBSummaryPage10BaseCourse23AssignModel();
                TypeBSummaryPage10BaseCourse24AssignModel typeBSummaryPage10BaseCourse24AssignModel = new TypeBSummaryPage10BaseCourse24AssignModel();
                TypeBSummaryPage10BaseCourse25AssignModel typeBSummaryPage10BaseCourse25AssignModel = new TypeBSummaryPage10BaseCourse25AssignModel();

                //  e-Form Test Report Page-10 SUMMARY
                TRobjViewModel.TYPEB_SUMMARY_PAGE10_BASE_COURSE3_1 = new EFORM_TR_TYPEB_SUMMARY_PAGE10_BASE_COURSE3_1();
                TRobjViewModel.TYPEB_SUMMARY_PAGE10_BASE_COURSE3_2 = new EFORM_TR_TYPEB_SUMMARY_PAGE10_BASE_COURSE3_2();
                TRobjViewModel.TYPEB_SUMMARY_PAGE10_BASE_COURSE3_3 = new EFORM_TR_TYPEB_SUMMARY_PAGE10_BASE_COURSE3_3();
                //  e-Form Test Report Page-10 DETAIL
                TRobjViewModel.TYPEB_DETAIL_PAGE10_BASE_COURSE3_1_List = new List<EFORM_TR_TYPEB_DETAIL_PAGE10_BASE_COURSE3_1>();
                TRobjViewModel.TYPEB_DETAIL_PAGE10_BASE_COURSE3_2_List = new List<EFORM_TR_TYPEB_DETAIL_PAGE10_BASE_COURSE3_2>();
                TRobjViewModel.TYPEB_DETAIL_PAGE10_BASE_COURSE3_3_List = new List<EFORM_TR_TYPEB_DETAIL_PAGE10_BASE_COURSE3_3>();

                #endregion

                #region e-Form Test Report Page- 12 BASE COURSE: 3rd Layer InIt
                //  e-Form Test Report Page-12 BASE COURSE: 3rd Layer SUMMARY Cement Stabilised 
                TRobjViewModel.TYPEB_SUMMARY_PAGE12_CEMENT_SUBBASE_1 = new EFORM_TR_TYPEB_SUMMARY_PAGE12_CEMENT_SUBBASE_1();
                TRobjViewModel.TYPEB_SUMMARY_PAGE12_CEMENT_SUBBASE_2 = new EFORM_TR_TYPEB_SUMMARY_PAGE12_CEMENT_SUBBASE_2();
                TRobjViewModel.TYPEB_SUMMARY_PAGE12_CEMENT_SUBBASE_3 = new EFORM_TR_TYPEB_SUMMARY_PAGE12_CEMENT_SUBBASE_3();
                //  e-Form Test Report Page-12 BASE COURSE: 3rd Layer DETAIL Cement Stabilised
                TRobjViewModel.TYPEB_DETAIL_PAGE12_CEMENT_SUBBASE_1_List = new List<EFORM_TR_TYPEB_DETAIL_PAGE12_CEMENT_SUBBASE_1>();
                TRobjViewModel.TYPEB_DETAIL_PAGE12_CEMENT_SUBBASE_2_List = new List<EFORM_TR_TYPEB_DETAIL_PAGE12_CEMENT_SUBBASE_2>();
                TRobjViewModel.TYPEB_DETAIL_PAGE12_CEMENT_SUBBASE_3_List = new List<EFORM_TR_TYPEB_DETAIL_PAGE12_CEMENT_SUBBASE_3>();
                TypeBSummaryPage12BaseCourse3Cement1AssignModel typeBSummaryPage12BaseCourse3Cement1 = new TypeBSummaryPage12BaseCourse3Cement1AssignModel();
                TypeBSummaryPage12BaseCourse3Cement2AssignModel typeBSummaryPage12BaseCourse3Cement2 = new TypeBSummaryPage12BaseCourse3Cement2AssignModel();
                TypeBSummaryPage12BaseCourse3Cement3AssignModel typeBSummaryPage12BaseCourse3Cement3 = new TypeBSummaryPage12BaseCourse3Cement3AssignModel();
                #endregion

                #region e-Form Test Report Page-13 BASE COURSE: 3rd Layer InIt
                //  e-Form Test Report Page-12 BASE COURSE: 3rd Layer SUMMARY Cement FDR 
                TRobjViewModel.TYPEB_SUMMARY_PAGE13_CEMENT_FDR_1 = new EFORM_TR_TYPEB_SUMMARY_PAGE13_CEMENT_FDR_1();
                TRobjViewModel.TYPEB_SUMMARY_PAGE13_CEMENT_FDR_2 = new EFORM_TR_TYPEB_SUMMARY_PAGE13_CEMENT_FDR_2();
                TRobjViewModel.TYPEB_SUMMARY_PAGE13_CEMENT_FDR_3 = new EFORM_TR_TYPEB_SUMMARY_PAGE13_CEMENT_FDR_3();
                //  e-Form Test Report Page-12 BASE COURSE: 3rd Layer DETAIL Cement FDR
                TRobjViewModel.TYPEB_DETAIL_PAGE13_CEMENT_FDR_1_List = new List<EFORM_TR_TYPEB_DETAIL_PAGE13_CEMENT_FDR_1>();
                TRobjViewModel.TYPEB_DETAIL_PAGE13_CEMENT_FDR_2_List = new List<EFORM_TR_TYPEB_DETAIL_PAGE13_CEMENT_FDR_2>();
                TRobjViewModel.TYPEB_DETAIL_PAGE13_CEMENT_FDR_3_List = new List<EFORM_TR_TYPEB_DETAIL_PAGE13_CEMENT_FDR_3>();
                TypeBSummaryPage13BaseCourse3CS_FDR1AssignModel typeBSummaryPage13BaseCourse3CS_FDR1 = new TypeBSummaryPage13BaseCourse3CS_FDR1AssignModel();
                TypeBSummaryPage13BaseCourse3CS_FDR2AssignModel typeBSummaryPage13BaseCourse3CS_FDR2 = new TypeBSummaryPage13BaseCourse3CS_FDR2AssignModel();
                TypeBSummaryPage13BaseCourse3CS_FDR3AssignModel typeBSummaryPage13BaseCourse3CS_FDR3 = new TypeBSummaryPage13BaseCourse3CS_FDR3AssignModel();
                #endregion

                #region e-Form Test Report Page-14 BITUMINOUS BASE COURSE: InIt
                //  e-Form Test Report Page-12 BASE COURSE: 3rd Layer SUMMARY Cement FDR 
                TRobjViewModel.TYPEB_SUMMARY_PAGE14_BITUMN_COURSE_1 = new EFORM_TR_TYPEB_SUMMARY_PAGE14_BITUMN_COURSE_1();
                TRobjViewModel.TYPEB_SUMMARY_PAGE14_BITUMN_COURSE_2 = new EFORM_TR_TYPEB_SUMMARY_PAGE14_BITUMN_COURSE_2();
                TRobjViewModel.TYPEB_SUMMARY_PAGE15_BITUMN_COURSE_3 = new EFORM_TR_TYPEB_SUMMARY_PAGE15_BITUMN_COURSE_3();
                //  e-Form Test Report Page-12 BASE COURSE: 3rd Layer DETAIL Cement FDR
                TRobjViewModel.TYPEB_DETAIL_PAGE14_BITUMN_COURSE_1_List = new List<EFORM_TR_TYPEB_DETAIL_PAGE14_BITUMN_COURSE_1>();
                TRobjViewModel.TYPEB_DETAIL_PAGE14_BITUMN_COURSE_2_List = new List<EFORM_TR_TYPEB_DETAIL_PAGE14_BITUMN_COURSE_2>();
                TRobjViewModel.TYPEB_DETAIL_PAGE15_BITUMN_COURSE_3_List = new List<EFORM_TR_TYPEB_DETAIL_PAGE15_BITUMN_COURSE_3>();
                TypeBSummaryPage14BitumnBaseCourse1AssignModel typeBSummaryPage14BitumnBaseCourse1 = new TypeBSummaryPage14BitumnBaseCourse1AssignModel();
                TypeBSummaryPage14BitumnBaseCourse2AssignModel typeBSummaryPage14BitumnBaseCourse2 = new TypeBSummaryPage14BitumnBaseCourse2AssignModel();
                TypeBSummaryPage14BitumnBaseCourse3AssignModel typeBSummaryPage14BitumnBaseCourse3 = new TypeBSummaryPage14BitumnBaseCourse3AssignModel();

                #endregion

                #region e-Form Test Report Page:17-18 BITUMINOUS SURFACE COURSE : InIt

                //  e-Form Test Report Page:17-18 BITUMINOUS SURFACE COURSE:  SUMMARY 
                TRobjViewModel.TYPEC_SUMMARY_PAGE17_BITUMN_SURFACE_COURSE_1 = new EFORM_TR_TYPEC_SUMMARY_PAGE17_BITUMN_SURFACE_COURSE_1();
                TRobjViewModel.TYPEC_SUMMARY_PAGE17_BITUMN_SURFACE_COURSE_2 = new EFORM_TR_TYPEC_SUMMARY_PAGE17_BITUMN_SURFACE_COURSE_2();
                TRobjViewModel.TYPEC_SUMMARY_PAGE18_BITUMN_SURFACE_COURSE_3 = new EFORM_TR_TYPEC_SUMMARY_PAGE18_BITUMN_SURFACE_COURSE_3();
                //  e-Form Test Report Page:17-18 BITUMINOUS SURFACE COURSE:  DETAIL 
                TRobjViewModel.TYPEC_DETAIL_PAGE17_BITUMN_SURFACE_COURSE_1_List = new List<EFORM_TR_TYPEC_DETAIL_PAGE17_BITUMN_SURFACE_COURSE_1>();
                TRobjViewModel.TYPEC_DETAIL_PAGE17_BITUMN_SURFACE_COURSE_2_List = new List<EFORM_TR_TYPEC_DETAIL_PAGE17_BITUMN_SURFACE_COURSE_2>();
                TRobjViewModel.TYPEC_DETAIL_PAGE18_BITUMN_SURFACE_COURSE_3_List = new List<EFORM_TR_TYPEC_DETAIL_PAGE18_BITUMN_SURFACE_COURSE_3>();
                TypeBSummaryPage17BitumnSurfaceCourse1AssignModel typeBSummaryPage17BitumnSurfaceCourse1 = new TypeBSummaryPage17BitumnSurfaceCourse1AssignModel();
                TypeBSummaryPage17BitumnSurfaceCourse2AssignModel typeBSummaryPage17BitumnSurfaceCourse2 = new TypeBSummaryPage17BitumnSurfaceCourse2AssignModel();
                TypeBSummaryPage17BitumnSurfaceCourse3AssignModel typeBSummaryPage17BitumnSurfaceCourse3 = new TypeBSummaryPage17BitumnSurfaceCourse3AssignModel();

                #endregion

                #region e-Form Test Report Page:20 Shoulder - GSB (Granular) : InIt

                //  e-Form Test Report Page:20 Shoulder - GSB (Granular):  SUMMARY 
                TRobjViewModel.TYPEB_SUMMARY_PAGE20_SHOULDER_1 = new EFORM_TR_TYPEB_SUMMARY_PAGE20_SHOULDER_1();
                TRobjViewModel.TYPEB_SUMMARY_PAGE20_SHOULDER_2 = new EFORM_TR_TYPEB_SUMMARY_PAGE20_SHOULDER_2();
                TRobjViewModel.TYPEB_SUMMARY_PAGE20_SHOULDER_3 = new EFORM_TR_TYPEB_SUMMARY_PAGE20_SHOULDER_3();
                //  e-Form Test Report Page:20 Shoulder - GSB (Granular):  DETAIL 
                TRobjViewModel.TYPEB_DETAIL_PAGE20_SHOULDER_1_List = new List<EFORM_TR_TYPEB_DETAIL_PAGE20_SHOULDER_1>();
                TRobjViewModel.TYPEB_DETAIL_PAGE20_SHOULDER_2_List = new List<EFORM_TR_TYPEB_DETAIL_PAGE20_SHOULDER_2>();
                TRobjViewModel.TYPEB_DETAIL_PAGE20_SHOULDER_3_List = new List<EFORM_TR_TYPEB_DETAIL_PAGE20_SHOULDER_3>();
                TypeBSummaryPage20Shoulder1AssignModel typeBSummaryPage20Shoulder1 = new TypeBSummaryPage20Shoulder1AssignModel();
                TypeBSummaryPage20Shoulder2AssignModel typeBSummaryPage20Shoulder2 = new TypeBSummaryPage20Shoulder2AssignModel();
                TypeBSummaryPage20Shoulder3AssignModel typeBSummaryPage20Shoulder3 = new TypeBSummaryPage20Shoulder3AssignModel();

                #endregion

                #region e-Form Test Report Page:21 Shoulder - GSB (Gravel) : InIt

                //  e-Form Test Report Page:21 Shoulder - GSB (Gravel):  SUMMARY 
                TRobjViewModel.TYPEB_SUMMARY_PAGE21_SHOULDER_1 = new EFORM_TR_TYPEB_SUMMARY_PAGE21_SHOULDER_1();
                TRobjViewModel.TYPEB_SUMMARY_PAGE21_SHOULDER_2 = new EFORM_TR_TYPEB_SUMMARY_PAGE21_SHOULDER_2();
                TRobjViewModel.TYPEB_SUMMARY_PAGE21_SHOULDER_3 = new EFORM_TR_TYPEB_SUMMARY_PAGE21_SHOULDER_3();
                //  e-Form Test Report Page:21 Shoulder - GSB (Gravel):  DETAIL 
                TRobjViewModel.TYPEB_DETAIL_PAGE21_SHOULDER_1_List = new List<EFORM_TR_TYPEB_DETAIL_PAGE21_SHOULDER_1>();
                TRobjViewModel.TYPEB_DETAIL_PAGE21_SHOULDER_2_List = new List<EFORM_TR_TYPEB_DETAIL_PAGE21_SHOULDER_2>();
                TRobjViewModel.TYPEB_DETAIL_PAGE21_SHOULDER_3_List = new List<EFORM_TR_TYPEB_DETAIL_PAGE21_SHOULDER_3>();
                TypeBSummaryPage21Shoulder1AssignModel typeBSummaryPage21Shoulder1 = new TypeBSummaryPage21Shoulder1AssignModel();
                TypeBSummaryPage21Shoulder2AssignModel typeBSummaryPage21Shoulder2 = new TypeBSummaryPage21Shoulder2AssignModel();
                TypeBSummaryPage21Shoulder3AssignModel typeBSummaryPage21Shoulder3 = new TypeBSummaryPage21Shoulder3AssignModel();

                #endregion

                #endregion

                #region rEAD cODE

                #region e-Form Test Report Page-2 Granular SubBase 

                if (naCheckArr[3] == 'Y')
                {
                    #region  2.1 Gradation Analysis of Aggregates - Granular Subbase , Chaninage - 1 

                    errorListtemp = objCommonFunction.FetchTetsReportPDFFilledDataToModel(TRobjViewModel.TYPEB_SUMMARY_PAGE2_GS_BASE_1, loadedForm);
                    TRobjViewModel.ErrorList.AddRange(errorListtemp);
                    TRobjViewModel.TYPEB_SUMMARY_PAGE2_GS_BASE_1.SUBITEM_ID = typeBSummaryPage2GranSub1.Model.SUBITEM_ID;
                    TRobjViewModel.TYPEB_SUMMARY_PAGE2_GS_BASE_1.MAIN_ITEM_ID = typeBSummaryPage2GranSub1.Model.MAIN_ITEM_ID;
                    TRobjViewModel.TYPEB_SUMMARY_PAGE2_GS_BASE_1.TABLE_ID = typeBSummaryPage2GranSub1.Model.TABLE_ID;

                    dbContext.EFORM_TR_DETAIL_ITEM_MASTER.Where(m => m.MAIN_ITEM_ID == typeBSummaryPage2GranSub1.Model.MAIN_ITEM_ID && m.SUBITEM_ID == typeBSummaryPage2GranSub1.Model.SUBITEM_ID).Select(t => t.DETAIL_ITEM_ID).ToList().ForEach(itemID =>
                    {
                        EFORM_TR_TYPEB_DETAIL_PAGE2_GS_BASE_1 detailedItem = new EFORM_TR_TYPEB_DETAIL_PAGE2_GS_BASE_1();
                        detailedItem.MAIN_ITEM_ID = typeBSummaryPage2GranSub1.Model.MAIN_ITEM_ID;
                        detailedItem.SUBITEM_ID = typeBSummaryPage2GranSub1.Model.SUBITEM_ID;
                        detailedItem.DETAIL_ITEM_ID = itemID;
                        detailedItem.TABLE_ID = typeBSummaryPage2GranSub1.Model.TABLE_ID;
                        TRobjViewModel.TYPEB_DETAIL_PAGE2_GS_BASE_1_List.Add(detailedItem);
                    });

                    foreach (var item in TRobjViewModel.TYPEB_DETAIL_PAGE2_GS_BASE_1_List.ToList())
                    {
                        errorListtemp = objCommonFunction.FetchTetsReportPDFFilledDataToModel(item, loadedForm);
                        TRobjViewModel.ErrorList.AddRange(errorListtemp);
                    }

                    foreach (var item in TRobjViewModel.TYPEB_DETAIL_PAGE2_GS_BASE_1_List.ToList())
                    {
                        if (item.SAMPLE_WEIGHT == null && item.RETAINED_WEIGHT == null && item.CUMULATIVE_WEIGHT == null && item.PASSING_WEIGHT == null)
                        {
                            TRobjViewModel.TYPEB_DETAIL_PAGE2_GS_BASE_1_List.Remove(item);
                        }

                    }
                    if (TRobjViewModel.TYPEB_DETAIL_PAGE2_GS_BASE_1_List.Count == 0)
                    {

                        errorListtemp.Clear();
                        errorListtemp.Add("Page-2: Item 2.1.SUBBASE :- Granular Subbase: Table-Please enter Wt. of Sample Retained(g) for atleast one I. S. Sieve Designation for Chainage -I");
                        TRobjViewModel.ErrorOccured = true;
                        TRobjViewModel.ErrorList.AddRange(errorListtemp);
                        errorListtemp.Clear();
                    }

                    #endregion       
                }

                if (naCheckArr[4] == 'Y')
                {
                    #region 2.2 Gradation Analysis of Aggregates - Granular Subbase , Chainage - 2

                    errorListtemp = objCommonFunction.FetchTetsReportPDFFilledDataToModel(TRobjViewModel.TYPEB_SUMMARY_PAGE2_GS_BASE_2, loadedForm);
                    TRobjViewModel.ErrorList.AddRange(errorListtemp);
                    TRobjViewModel.TYPEB_SUMMARY_PAGE2_GS_BASE_2.SUBITEM_ID = typeBSummaryPage2GranSub2.Model.SUBITEM_ID;
                    TRobjViewModel.TYPEB_SUMMARY_PAGE2_GS_BASE_2.MAIN_ITEM_ID = typeBSummaryPage2GranSub2.Model.MAIN_ITEM_ID;
                    TRobjViewModel.TYPEB_SUMMARY_PAGE2_GS_BASE_2.TABLE_ID = typeBSummaryPage2GranSub2.Model.TABLE_ID;

                    dbContext.EFORM_TR_DETAIL_ITEM_MASTER.Where(m => m.MAIN_ITEM_ID == typeBSummaryPage2GranSub2.Model.MAIN_ITEM_ID && m.SUBITEM_ID == typeBSummaryPage2GranSub2.Model.SUBITEM_ID).Select(t => t.DETAIL_ITEM_ID).ToList().ForEach(itemID =>
                    {
                        EFORM_TR_TYPEB_DETAIL_PAGE2_GS_BASE_2 detailedItem = new EFORM_TR_TYPEB_DETAIL_PAGE2_GS_BASE_2();
                        detailedItem.MAIN_ITEM_ID = typeBSummaryPage2GranSub2.Model.MAIN_ITEM_ID;
                        detailedItem.SUBITEM_ID = typeBSummaryPage2GranSub2.Model.SUBITEM_ID;
                        detailedItem.DETAIL_ITEM_ID = itemID;
                        detailedItem.TABLE_ID = typeBSummaryPage2GranSub2.Model.TABLE_ID;
                        TRobjViewModel.TYPEB_DETAIL_PAGE2_GS_BASE_2_List.Add(detailedItem);
                    });

                    foreach (var item in TRobjViewModel.TYPEB_DETAIL_PAGE2_GS_BASE_2_List.ToList())
                    {
                        errorListtemp = objCommonFunction.FetchTetsReportPDFFilledDataToModel(item, loadedForm);
                        TRobjViewModel.ErrorList.AddRange(errorListtemp);
                    }

                    foreach (var item in TRobjViewModel.TYPEB_DETAIL_PAGE2_GS_BASE_2_List.ToList())
                    {
                        if (item.SAMPLE_WEIGHT == null && item.RETAINED_WEIGHT == null && item.CUMULATIVE_WEIGHT == null && item.PASSING_WEIGHT == null)
                        {
                            TRobjViewModel.TYPEB_DETAIL_PAGE2_GS_BASE_2_List.Remove(item);
                        }
                    }
                    if (TRobjViewModel.TYPEB_DETAIL_PAGE2_GS_BASE_2_List.Count == 0)
                    {

                        errorListtemp.Clear();
                        errorListtemp.Add("Page-2: Item 2.2.SUBBASE :- Granular Subbase: Table-Please enter Wt. of Sample Retained(g) for atleast one I. S. Sieve Designation for Chainage -II");
                        TRobjViewModel.ErrorOccured = true;
                        TRobjViewModel.ErrorList.AddRange(errorListtemp);
                        errorListtemp.Clear();
                    }
                    #endregion
                }

                if (naCheckArr[5] == 'Y')
                {
                    #region 2.3 Gradation Analysis of Aggregates - Granular Subbase , Chainage - 3

                    errorListtemp = objCommonFunction.FetchTetsReportPDFFilledDataToModel(TRobjViewModel.TYPEB_SUMMARY_PAGE2_GS_BASE_3, loadedForm);
                    TRobjViewModel.ErrorList.AddRange(errorListtemp);
                    TRobjViewModel.TYPEB_SUMMARY_PAGE2_GS_BASE_3.SUBITEM_ID = typeBSummaryPage2GranSub3.Model.SUBITEM_ID;
                    TRobjViewModel.TYPEB_SUMMARY_PAGE2_GS_BASE_3.MAIN_ITEM_ID = typeBSummaryPage2GranSub3.Model.MAIN_ITEM_ID;
                    TRobjViewModel.TYPEB_SUMMARY_PAGE2_GS_BASE_3.TABLE_ID = typeBSummaryPage2GranSub3.Model.TABLE_ID;

                    dbContext.EFORM_TR_DETAIL_ITEM_MASTER.Where(m => m.MAIN_ITEM_ID == typeBSummaryPage2GranSub3.Model.MAIN_ITEM_ID && m.SUBITEM_ID == typeBSummaryPage2GranSub3.Model.SUBITEM_ID).Select(t => t.DETAIL_ITEM_ID).ToList().ForEach(itemID =>
                    {
                        EFORM_TR_TYPEB_DETAIL_PAGE2_GS_BASE_3 detailedItem = new EFORM_TR_TYPEB_DETAIL_PAGE2_GS_BASE_3();
                        detailedItem.MAIN_ITEM_ID = typeBSummaryPage2GranSub3.Model.MAIN_ITEM_ID;
                        detailedItem.SUBITEM_ID = typeBSummaryPage2GranSub3.Model.SUBITEM_ID;
                        detailedItem.DETAIL_ITEM_ID = itemID;
                        detailedItem.TABLE_ID = typeBSummaryPage2GranSub3.Model.TABLE_ID;
                        TRobjViewModel.TYPEB_DETAIL_PAGE2_GS_BASE_3_List.Add(detailedItem);
                    });

                    foreach (var item in TRobjViewModel.TYPEB_DETAIL_PAGE2_GS_BASE_3_List.ToList())
                    {
                        errorListtemp = objCommonFunction.FetchTetsReportPDFFilledDataToModel(item, loadedForm);
                        TRobjViewModel.ErrorList.AddRange(errorListtemp);
                    }

                    foreach (var item in TRobjViewModel.TYPEB_DETAIL_PAGE2_GS_BASE_3_List.ToList())
                    {
                        if (item.SAMPLE_WEIGHT == null && item.RETAINED_WEIGHT == null && item.CUMULATIVE_WEIGHT == null && item.PASSING_WEIGHT == null)
                        {
                            TRobjViewModel.TYPEB_DETAIL_PAGE2_GS_BASE_3_List.Remove(item);
                        }
                    }
                    if (TRobjViewModel.TYPEB_DETAIL_PAGE2_GS_BASE_3_List.Count == 0)
                    {

                        errorListtemp.Clear();
                        errorListtemp.Add("Page-2: Item 2.3.SUBBASE :- Granular Subbase: Table-Please enter Wt. of Sample Retained(g) for atleast one I. S. Sieve Designation for Chainage -III");
                        TRobjViewModel.ErrorOccured = true;
                        TRobjViewModel.ErrorList.AddRange(errorListtemp);
                        errorListtemp.Clear();
                    }

                    #endregion
                }

                #endregion

                #region e-Form Test Report Page-3 Gravel SubBase 

                if (naCheckArr[6] == 'Y')
                {
                    #region Page -3, 2-2.1 Gradation Analysis of Aggregates - Gravel Subbase , Chaninage - 1 

                    errorListtemp = objCommonFunction.FetchTetsReportPDFFilledDataToModel(TRobjViewModel.TYPEB_SUMMARY_PAGE3_GRAVEL_SUBBASE_1, loadedForm);
                    TRobjViewModel.ErrorList.AddRange(errorListtemp);
                    TRobjViewModel.TYPEB_SUMMARY_PAGE3_GRAVEL_SUBBASE_1.SUBITEM_ID = typeBSummaryPage3GravelSubBase1.Model.SUBITEM_ID;
                    TRobjViewModel.TYPEB_SUMMARY_PAGE3_GRAVEL_SUBBASE_1.MAIN_ITEM_ID = typeBSummaryPage3GravelSubBase1.Model.MAIN_ITEM_ID;
                    TRobjViewModel.TYPEB_SUMMARY_PAGE3_GRAVEL_SUBBASE_1.TABLE_ID = typeBSummaryPage3GravelSubBase1.Model.TABLE_ID;

                    dbContext.EFORM_TR_DETAIL_ITEM_MASTER.Where(m => m.MAIN_ITEM_ID == typeBSummaryPage3GravelSubBase1.Model.MAIN_ITEM_ID && m.SUBITEM_ID == typeBSummaryPage3GravelSubBase1.Model.SUBITEM_ID).Select(t => t.DETAIL_ITEM_ID).ToList().ForEach(itemID =>
                    {
                        EFORM_TR_TYPEB_DETAIL_PAGE3_GRAVEL_SUBBASE_1 detailedItem = new EFORM_TR_TYPEB_DETAIL_PAGE3_GRAVEL_SUBBASE_1();
                        detailedItem.MAIN_ITEM_ID = typeBSummaryPage3GravelSubBase1.Model.MAIN_ITEM_ID;
                        detailedItem.SUBITEM_ID = typeBSummaryPage3GravelSubBase1.Model.SUBITEM_ID;
                        detailedItem.DETAIL_ITEM_ID = itemID;
                        detailedItem.TABLE_ID = typeBSummaryPage3GravelSubBase1.Model.TABLE_ID;
                        TRobjViewModel.TYPEB_DETAIL_PAGE3_GRAVEL_SUBBASE_1_List.Add(detailedItem);
                    });

                    foreach (var item in TRobjViewModel.TYPEB_DETAIL_PAGE3_GRAVEL_SUBBASE_1_List.ToList())
                    {
                        errorListtemp = objCommonFunction.FetchTetsReportPDFFilledDataToModel(item, loadedForm);
                        TRobjViewModel.ErrorList.AddRange(errorListtemp);
                    }

                    foreach (var item in TRobjViewModel.TYPEB_DETAIL_PAGE3_GRAVEL_SUBBASE_1_List.ToList())
                    {
                        if (item.SAMPLE_WEIGHT == null && item.RETAINED_WEIGHT == null && item.CUMULATIVE_WEIGHT == null && item.PASSING_WEIGHT == null)
                        {
                            TRobjViewModel.TYPEB_DETAIL_PAGE3_GRAVEL_SUBBASE_1_List.Remove(item);
                        }
                    }

                    if (TRobjViewModel.TYPEB_DETAIL_PAGE3_GRAVEL_SUBBASE_1_List.Count == 0)
                    {

                        errorListtemp.Clear();
                        errorListtemp.Add("Page-3: Item 2.1.SUBBASE :- Gravel Subbase: Table - Please enter Wt. of Sample Retained(g) for atleast one I. S. Sieve Designation for Chainage-I");
                        TRobjViewModel.ErrorOccured = true;
                        TRobjViewModel.ErrorList.AddRange(errorListtemp);
                        errorListtemp.Clear();
                    }

                    #endregion
                }

                if (naCheckArr[7] == 'Y')
                {
                    #region Page -3, 2-2.2 Gradation Analysis of Aggregates - Gravel Subbase , Chaninage - 2 

                    errorListtemp = objCommonFunction.FetchTetsReportPDFFilledDataToModel(TRobjViewModel.TYPEB_SUMMARY_PAGE3_GRAVEL_SUBBASE_2, loadedForm);
                    TRobjViewModel.ErrorList.AddRange(errorListtemp);
                    TRobjViewModel.TYPEB_SUMMARY_PAGE3_GRAVEL_SUBBASE_2.SUBITEM_ID = typeBSummaryPage3GravelSubBase2.Model.SUBITEM_ID;
                    TRobjViewModel.TYPEB_SUMMARY_PAGE3_GRAVEL_SUBBASE_2.MAIN_ITEM_ID = typeBSummaryPage3GravelSubBase2.Model.MAIN_ITEM_ID;
                    TRobjViewModel.TYPEB_SUMMARY_PAGE3_GRAVEL_SUBBASE_2.TABLE_ID = typeBSummaryPage3GravelSubBase2.Model.TABLE_ID;

                    dbContext.EFORM_TR_DETAIL_ITEM_MASTER.Where(m => m.MAIN_ITEM_ID == typeBSummaryPage3GravelSubBase2.Model.MAIN_ITEM_ID && m.SUBITEM_ID == typeBSummaryPage3GravelSubBase2.Model.SUBITEM_ID).Select(t => t.DETAIL_ITEM_ID).ToList().ForEach(itemID =>
                    {
                        EFORM_TR_TYPEB_DETAIL_PAGE3_GRAVEL_SUBBASE_2 detailedItem = new EFORM_TR_TYPEB_DETAIL_PAGE3_GRAVEL_SUBBASE_2();
                        detailedItem.MAIN_ITEM_ID = typeBSummaryPage3GravelSubBase2.Model.MAIN_ITEM_ID;
                        detailedItem.SUBITEM_ID = typeBSummaryPage3GravelSubBase2.Model.SUBITEM_ID;
                        detailedItem.DETAIL_ITEM_ID = itemID;
                        detailedItem.TABLE_ID = typeBSummaryPage3GravelSubBase2.Model.TABLE_ID;
                        TRobjViewModel.TYPEB_DETAIL_PAGE3_GRAVEL_SUBBASE_2_List.Add(detailedItem);
                    });

                    foreach (var item in TRobjViewModel.TYPEB_DETAIL_PAGE3_GRAVEL_SUBBASE_2_List.ToList())
                    {
                        errorListtemp = objCommonFunction.FetchTetsReportPDFFilledDataToModel(item, loadedForm);
                        TRobjViewModel.ErrorList.AddRange(errorListtemp);
                    }

                    foreach (var item in TRobjViewModel.TYPEB_DETAIL_PAGE3_GRAVEL_SUBBASE_2_List.ToList())
                    {
                        if (item.SAMPLE_WEIGHT == null && item.RETAINED_WEIGHT == null && item.CUMULATIVE_WEIGHT == null && item.PASSING_WEIGHT == null)
                        {
                            TRobjViewModel.TYPEB_DETAIL_PAGE3_GRAVEL_SUBBASE_2_List.Remove(item);
                        }
                    }

                    if (TRobjViewModel.TYPEB_DETAIL_PAGE3_GRAVEL_SUBBASE_2_List.Count == 0)
                    {

                        errorListtemp.Clear();
                        errorListtemp.Add("Page-3: Item 2.2.SUBBASE :- Gravel Subbase: Table - Please enter Wt. of Sample Retained(g) for atleast one I. S. Sieve Designation for Chainage-II");
                        TRobjViewModel.ErrorOccured = true;
                        TRobjViewModel.ErrorList.AddRange(errorListtemp);
                        errorListtemp.Clear();
                    }

                    #endregion
                }

                if (naCheckArr[8] == 'Y')
                {
                    #region  Page -3, 2-2.3 Gradation Analysis of Aggregates - Gravel Subbase , Chaninage - 3

                    errorListtemp = objCommonFunction.FetchTetsReportPDFFilledDataToModel(TRobjViewModel.TYPEB_SUMMARY_PAGE3_GRAVEL_SUBBASE_3, loadedForm);
                    TRobjViewModel.ErrorList.AddRange(errorListtemp);
                    TRobjViewModel.TYPEB_SUMMARY_PAGE3_GRAVEL_SUBBASE_3.SUBITEM_ID = typeBSummaryPage3GravelSubBase3.Model.SUBITEM_ID;
                    TRobjViewModel.TYPEB_SUMMARY_PAGE3_GRAVEL_SUBBASE_3.MAIN_ITEM_ID = typeBSummaryPage3GravelSubBase3.Model.MAIN_ITEM_ID;
                    TRobjViewModel.TYPEB_SUMMARY_PAGE3_GRAVEL_SUBBASE_3.TABLE_ID = typeBSummaryPage3GravelSubBase3.Model.TABLE_ID;

                    dbContext.EFORM_TR_DETAIL_ITEM_MASTER.Where(m => m.MAIN_ITEM_ID == typeBSummaryPage3GravelSubBase3.Model.MAIN_ITEM_ID && m.SUBITEM_ID == typeBSummaryPage3GravelSubBase3.Model.SUBITEM_ID).Select(t => t.DETAIL_ITEM_ID).ToList().ForEach(itemID =>
                    {
                        EFORM_TR_TYPEB_DETAIL_PAGE3_GRAVEL_SUBBASE_3 detailedItem = new EFORM_TR_TYPEB_DETAIL_PAGE3_GRAVEL_SUBBASE_3();
                        detailedItem.MAIN_ITEM_ID = typeBSummaryPage3GravelSubBase3.Model.MAIN_ITEM_ID;
                        detailedItem.SUBITEM_ID = typeBSummaryPage3GravelSubBase3.Model.SUBITEM_ID;
                        detailedItem.DETAIL_ITEM_ID = itemID;
                        detailedItem.TABLE_ID = typeBSummaryPage3GravelSubBase3.Model.TABLE_ID;
                        TRobjViewModel.TYPEB_DETAIL_PAGE3_GRAVEL_SUBBASE_3_List.Add(detailedItem);
                    });

                    foreach (var item in TRobjViewModel.TYPEB_DETAIL_PAGE3_GRAVEL_SUBBASE_3_List.ToList())
                    {
                        errorListtemp = objCommonFunction.FetchTetsReportPDFFilledDataToModel(item, loadedForm);
                        TRobjViewModel.ErrorList.AddRange(errorListtemp);
                    }

                    foreach (var item in TRobjViewModel.TYPEB_DETAIL_PAGE3_GRAVEL_SUBBASE_3_List.ToList())
                    {
                        if (item.SAMPLE_WEIGHT == null && item.RETAINED_WEIGHT == null && item.CUMULATIVE_WEIGHT == null && item.PASSING_WEIGHT == null)
                        {
                            TRobjViewModel.TYPEB_DETAIL_PAGE3_GRAVEL_SUBBASE_3_List.Remove(item);
                        }
                    }
                    if (TRobjViewModel.TYPEB_DETAIL_PAGE3_GRAVEL_SUBBASE_3_List.Count == 0)
                    {

                        errorListtemp.Clear();
                        errorListtemp.Add("Page-3: Item 2.3.SUBBASE :- Gravel Subbase: Table - Please enter Wt. of Sample Retained(g) for atleast one I. S. Sieve Designation for Chainage-III");
                        TRobjViewModel.ErrorOccured = true;
                        TRobjViewModel.ErrorList.AddRange(errorListtemp);
                        errorListtemp.Clear();
                    }
                    #endregion
                }

                #endregion

                #region e-Form Test Report Page-4 Cement Stabilised Subbase

                if (TemplateStatus == false)
                {
                    if (naCheckArr[9] == 'Y')
                    {
                        #region Page -4, 2-2.1 Gradation Analysis of Aggregates -  Cement Stabilised Subbase , Chaninage - 1 

                        errorListtemp = objCommonFunction.FetchTetsReportPDFFilledDataToModel(TRobjViewModel.TYPEB_SUMMARY_PAGE4_CS_SUBBASE_1, loadedForm);
                        TRobjViewModel.ErrorList.AddRange(errorListtemp);
                        TRobjViewModel.TYPEB_SUMMARY_PAGE4_CS_SUBBASE_1.SUBITEM_ID = typeBSummaryPage4CementSubBase1.Model.SUBITEM_ID;
                        TRobjViewModel.TYPEB_SUMMARY_PAGE4_CS_SUBBASE_1.MAIN_ITEM_ID = typeBSummaryPage4CementSubBase1.Model.MAIN_ITEM_ID;
                        TRobjViewModel.TYPEB_SUMMARY_PAGE4_CS_SUBBASE_1.TABLE_ID = typeBSummaryPage4CementSubBase1.Model.TABLE_ID;

                        dbContext.EFORM_TR_DETAIL_ITEM_MASTER.Where(m => m.MAIN_ITEM_ID == typeBSummaryPage4CementSubBase1.Model.MAIN_ITEM_ID && m.SUBITEM_ID == typeBSummaryPage4CementSubBase1.Model.SUBITEM_ID).Select(t => t.DETAIL_ITEM_ID).ToList().ForEach(itemID =>
                        {
                            EFORM_TR_TYPEB_DETAIL_PAGE4_CS_SUBBASE_1 detailedItem = new EFORM_TR_TYPEB_DETAIL_PAGE4_CS_SUBBASE_1();
                            detailedItem.MAIN_ITEM_ID = typeBSummaryPage4CementSubBase1.Model.MAIN_ITEM_ID;
                            detailedItem.SUBITEM_ID = typeBSummaryPage4CementSubBase1.Model.SUBITEM_ID;
                            detailedItem.DETAIL_ITEM_ID = itemID;
                            detailedItem.TABLE_ID = typeBSummaryPage4CementSubBase1.Model.TABLE_ID;
                            TRobjViewModel.TYPEB_DETAIL_PAGE4_CS_SUBBASE_1_List.Add(detailedItem);
                        });

                        foreach (var item in TRobjViewModel.TYPEB_DETAIL_PAGE4_CS_SUBBASE_1_List.ToList())
                        {
                            errorListtemp = objCommonFunction.FetchTetsReportPDFFilledDataToModel(item, loadedForm);
                            TRobjViewModel.ErrorList.AddRange(errorListtemp);
                        }

                        foreach (var item in TRobjViewModel.TYPEB_DETAIL_PAGE4_CS_SUBBASE_1_List.ToList())
                        {
                            if (item.SAMPLE_WEIGHT == null && item.RETAINED_WEIGHT == null && item.CUMULATIVE_WEIGHT == null && item.PASSING_WEIGHT == null)
                            {
                                TRobjViewModel.TYPEB_DETAIL_PAGE4_CS_SUBBASE_1_List.Remove(item);
                            }
                        }
                        if (TRobjViewModel.TYPEB_DETAIL_PAGE4_CS_SUBBASE_1_List.Count == 0)
                        {

                            errorListtemp.Clear();
                            errorListtemp.Add("Page-4: Item 2.1.SUBBASE :-Cement Stabilised Subbase: Table -Please Enter Wt. of Sample Retained(g) for atleast one I. S. Sieve Designation for Chainage-I");
                            TRobjViewModel.ErrorOccured = true;
                            TRobjViewModel.ErrorList.AddRange(errorListtemp);
                            errorListtemp.Clear();
                        }
                        #endregion
                    }

                    if (naCheckArr[10] == 'Y')
                    {
                        #region Page -4, 2-2.2 Gradation Analysis of Aggregates -  Cement Stabilised Subbase , Chaninage - 2

                        errorListtemp = objCommonFunction.FetchTetsReportPDFFilledDataToModel(TRobjViewModel.TYPEB_SUMMARY_PAGE4_CS_SUBBASE_2, loadedForm);
                        TRobjViewModel.ErrorList.AddRange(errorListtemp);
                        TRobjViewModel.TYPEB_SUMMARY_PAGE4_CS_SUBBASE_2.SUBITEM_ID = typeBSummaryPage4CementSubBase2.Model.SUBITEM_ID;
                        TRobjViewModel.TYPEB_SUMMARY_PAGE4_CS_SUBBASE_2.MAIN_ITEM_ID = typeBSummaryPage4CementSubBase2.Model.MAIN_ITEM_ID;
                        TRobjViewModel.TYPEB_SUMMARY_PAGE4_CS_SUBBASE_2.TABLE_ID = typeBSummaryPage4CementSubBase2.Model.TABLE_ID;

                        dbContext.EFORM_TR_DETAIL_ITEM_MASTER.Where(m => m.MAIN_ITEM_ID == typeBSummaryPage4CementSubBase2.Model.MAIN_ITEM_ID && m.SUBITEM_ID == typeBSummaryPage4CementSubBase2.Model.SUBITEM_ID).Select(t => t.DETAIL_ITEM_ID).ToList().ForEach(itemID =>
                        {
                            EFORM_TR_TYPEB_DETAIL_PAGE4_CS_SUBBASE_2 detailedItem = new EFORM_TR_TYPEB_DETAIL_PAGE4_CS_SUBBASE_2();
                            detailedItem.MAIN_ITEM_ID = typeBSummaryPage4CementSubBase2.Model.MAIN_ITEM_ID;
                            detailedItem.SUBITEM_ID = typeBSummaryPage4CementSubBase2.Model.SUBITEM_ID;
                            detailedItem.DETAIL_ITEM_ID = itemID;
                            detailedItem.TABLE_ID = typeBSummaryPage4CementSubBase2.Model.TABLE_ID;
                            TRobjViewModel.TYPEB_DETAIL_PAGE4_CS_SUBBASE_2_List.Add(detailedItem);
                        });

                        foreach (var item in TRobjViewModel.TYPEB_DETAIL_PAGE4_CS_SUBBASE_2_List.ToList())
                        {
                            errorListtemp = objCommonFunction.FetchTetsReportPDFFilledDataToModel(item, loadedForm);
                            TRobjViewModel.ErrorList.AddRange(errorListtemp);
                        }

                        foreach (var item in TRobjViewModel.TYPEB_DETAIL_PAGE4_CS_SUBBASE_2_List.ToList())
                        {
                            if (item.SAMPLE_WEIGHT == null && item.RETAINED_WEIGHT == null && item.CUMULATIVE_WEIGHT == null && item.PASSING_WEIGHT == null)
                            {
                                TRobjViewModel.TYPEB_DETAIL_PAGE4_CS_SUBBASE_2_List.Remove(item);
                            }
                        }
                        if (TRobjViewModel.TYPEB_DETAIL_PAGE4_CS_SUBBASE_2_List.Count == 0)
                        {

                            errorListtemp.Clear();
                            errorListtemp.Add("Page-4: Item 2.2.SUBBASE :-Cement Stabilised Subbase: Table -Please Enter Wt. of Sample Retained(g) for atleast one I. S. Sieve Designation for Chainage-II");
                            TRobjViewModel.ErrorOccured = true;
                            TRobjViewModel.ErrorList.AddRange(errorListtemp);
                            errorListtemp.Clear();
                        }
                        #endregion
                    }

                    if (naCheckArr[11] == 'Y')
                    {
                        #region Page -4, 2-2.3 Gradation Analysis of Aggregates -  Cement Stabilised Subbase , Chaninage - 3

                        errorListtemp = objCommonFunction.FetchTetsReportPDFFilledDataToModel(TRobjViewModel.TYPEB_SUMMARY_PAGE4_CS_SUBBASE_3, loadedForm);
                        TRobjViewModel.ErrorList.AddRange(errorListtemp);
                        TRobjViewModel.TYPEB_SUMMARY_PAGE4_CS_SUBBASE_3.SUBITEM_ID = typeBSummaryPage4CementSubBase3.Model.SUBITEM_ID;
                        TRobjViewModel.TYPEB_SUMMARY_PAGE4_CS_SUBBASE_3.MAIN_ITEM_ID = typeBSummaryPage4CementSubBase3.Model.MAIN_ITEM_ID;
                        TRobjViewModel.TYPEB_SUMMARY_PAGE4_CS_SUBBASE_3.TABLE_ID = typeBSummaryPage4CementSubBase3.Model.TABLE_ID;

                        dbContext.EFORM_TR_DETAIL_ITEM_MASTER.Where(m => m.MAIN_ITEM_ID == typeBSummaryPage4CementSubBase3.Model.MAIN_ITEM_ID && m.SUBITEM_ID == typeBSummaryPage4CementSubBase3.Model.SUBITEM_ID).Select(t => t.DETAIL_ITEM_ID).ToList().ForEach(itemID =>
                        {
                            EFORM_TR_TYPEB_DETAIL_PAGE4_CS_SUBBASE_3 detailedItem = new EFORM_TR_TYPEB_DETAIL_PAGE4_CS_SUBBASE_3();
                            detailedItem.MAIN_ITEM_ID = typeBSummaryPage4CementSubBase3.Model.MAIN_ITEM_ID;
                            detailedItem.SUBITEM_ID = typeBSummaryPage4CementSubBase3.Model.SUBITEM_ID;
                            detailedItem.DETAIL_ITEM_ID = itemID;
                            detailedItem.TABLE_ID = typeBSummaryPage4CementSubBase3.Model.TABLE_ID;
                            TRobjViewModel.TYPEB_DETAIL_PAGE4_CS_SUBBASE_3_List.Add(detailedItem);
                        });

                        foreach (var item in TRobjViewModel.TYPEB_DETAIL_PAGE4_CS_SUBBASE_3_List.ToList())
                        {
                            errorListtemp = objCommonFunction.FetchTetsReportPDFFilledDataToModel(item, loadedForm);
                            TRobjViewModel.ErrorList.AddRange(errorListtemp);
                        }

                        foreach (var item in TRobjViewModel.TYPEB_DETAIL_PAGE4_CS_SUBBASE_3_List.ToList())
                        {
                            if (item.SAMPLE_WEIGHT == null && item.RETAINED_WEIGHT == null && item.CUMULATIVE_WEIGHT == null && item.PASSING_WEIGHT == null)
                            {
                                TRobjViewModel.TYPEB_DETAIL_PAGE4_CS_SUBBASE_3_List.Remove(item);
                            }
                        }
                        if (TRobjViewModel.TYPEB_DETAIL_PAGE4_CS_SUBBASE_3_List.Count == 0)
                        {

                            errorListtemp.Clear();
                            errorListtemp.Add("Page-4: Item 2.3.SUBBASE :-Cement Stabilised Subbase: Table -Please Enter Wt. of Sample Retained(g) for atleast one I. S. Sieve Designation for Chainage-III");
                            TRobjViewModel.ErrorOccured = true;
                            TRobjViewModel.ErrorList.AddRange(errorListtemp);
                            errorListtemp.Clear();
                        }

                        #endregion
                    }
                }

                #endregion

                #region e-Form Test Report Page-6 BASE COURSE: 1st Layer

                #region  To get Detail Item Dynamically
                int OptionItemID = 0;
                // int SubItemId = 0;
                //   int MainItemID = 0;

                PdfLoadedRadioButtonListField BaseCourtse1 = loadedForm.Fields["Base_Course_1"] as PdfLoadedRadioButtonListField;
                OptionItemID = Convert.ToInt32(BaseCourtse1.SelectedValue);
                if (OptionItemID == 0)
                {
                    errorListtemp.Clear();
                    errorListtemp.Add("Page-6: BASE COURSE: 1st Layer: Please Select Base Course 1st Layer Type");
                    TRobjViewModel.ErrorOccured = true;
                    TRobjViewModel.ErrorList.AddRange(errorListtemp);
                    errorListtemp.Clear();
                }
                else
                {
                    var OptionMasterModel = dbContext.EFORM_TR_ITEM_OPTIONS_MASTER.Where(x => x.OPTION_ID == OptionItemID).Select(x => new { x.SUBITEM_ID, x.MAIN_ITEM_ID }).FirstOrDefault();
                    // SubItemId = Convert.ToInt32(OptionMasterModel.SUBITEM_ID);
                    // MainItemID = OptionMasterModel.MAIN_ITEM_ID;
                    TestReportModel.EFORM_TR_MAIN_ITEM_OPTIONS_SELECTED_DETAIL modelSub = new TestReportModel.EFORM_TR_MAIN_ITEM_OPTIONS_SELECTED_DETAIL();
                    modelSub.MAIN_ITEM_ID = typeBSummaryPage6BaseCourse13AssignModel.Model.MAIN_ITEM_ID;
                    modelSub.OPTION_ID = OptionItemID;
                    TRobjViewModel.MAIN_ITEM_OPTIONS_SELECTED_DETAIL_List.Add(modelSub);
                }

                #endregion

                if (naCheckArr[13] == 'Y' && ((BaseCourtse1.SelectedValue == "8") || (BaseCourtse1.SelectedValue == "9") || (BaseCourtse1.SelectedValue == "10")))
                {
                    #region Page -6, 4 - 4.1 BASE COURSE: 1st Layer - Chaninage - 1 

                    errorListtemp = objCommonFunction.FetchTetsReportPDFFilledDataToModel(TRobjViewModel.TYPEB_SUMMARY_PAGE6_BASE_COURSE_1, loadedForm);
                    TRobjViewModel.ErrorList.AddRange(errorListtemp);
                    TRobjViewModel.TYPEB_SUMMARY_PAGE6_BASE_COURSE_1.SUBITEM_ID = typeBSummaryPage6BaseCourse13AssignModel.Model.SUBITEM_ID;
                    TRobjViewModel.TYPEB_SUMMARY_PAGE6_BASE_COURSE_1.MAIN_ITEM_ID = typeBSummaryPage6BaseCourse13AssignModel.Model.MAIN_ITEM_ID;
                    TRobjViewModel.TYPEB_SUMMARY_PAGE6_BASE_COURSE_1.TABLE_ID = typeBSummaryPage6BaseCourse13AssignModel.Model.TABLE_ID;
                    int RowNo = 1;
                    dbContext.EFORM_TR_DETAIL_ITEM_MASTER.Where(m => m.MAIN_ITEM_ID == typeBSummaryPage6BaseCourse13AssignModel.Model.MAIN_ITEM_ID && m.SUBITEM_ID == typeBSummaryPage6BaseCourse13AssignModel.Model.SUBITEM_ID && m.OPTION_ID == OptionItemID).Select(x => x.DETAIL_ITEM_ID).ToList().ForEach(itemID =>
                    {
                        EFORM_TR_TYPEB_DETAIL_PAGE6_BASE_COURSE_1 detailedItem = new EFORM_TR_TYPEB_DETAIL_PAGE6_BASE_COURSE_1();
                        detailedItem.MAIN_ITEM_ID = typeBSummaryPage6BaseCourse13AssignModel.Model.MAIN_ITEM_ID;
                        detailedItem.SUBITEM_ID = typeBSummaryPage6BaseCourse13AssignModel.Model.SUBITEM_ID;
                        detailedItem.DETAIL_ITEM_ID = itemID;
                        detailedItem.TABLE_ID = typeBSummaryPage6BaseCourse13AssignModel.Model.TABLE_ID;
                        detailedItem.ROW_ID = RowNo;
                        TRobjViewModel.TYPEB_DETAIL_PAGE6_BASE_COURSE_1_List.Add(detailedItem);
                        RowNo++;
                    });
                    EFORM_TR_TYPEB_DETAIL_PAGE6_BASE_COURSE_1 detailedItemPAN = new EFORM_TR_TYPEB_DETAIL_PAGE6_BASE_COURSE_1();
                    detailedItemPAN.MAIN_ITEM_ID = typeBSummaryPage6BaseCourse13AssignModel.Model.MAIN_ITEM_ID;
                    detailedItemPAN.SUBITEM_ID = typeBSummaryPage6BaseCourse13AssignModel.Model.SUBITEM_ID;
                    detailedItemPAN.DETAIL_ITEM_ID = typeBSummaryPage6BaseCourse13AssignModel.Model.PAN;
                    detailedItemPAN.TABLE_ID = typeBSummaryPage6BaseCourse13AssignModel.Model.TABLE_ID;
                    detailedItemPAN.ROW_ID = 9;
                    TRobjViewModel.TYPEB_DETAIL_PAGE6_BASE_COURSE_1_List.Add(detailedItemPAN);

                    foreach (var item in TRobjViewModel.TYPEB_DETAIL_PAGE6_BASE_COURSE_1_List.ToList())
                    {
                        errorListtemp = objCommonFunction.FetchTetsReportPDFFilledDataToModel(item, loadedForm);
                        TRobjViewModel.ErrorList.AddRange(errorListtemp);
                    }

                    foreach (var item in TRobjViewModel.TYPEB_DETAIL_PAGE6_BASE_COURSE_1_List.ToList())
                    {
                        if (item.SAMPLE_WEIGHT_6_1 == null && item.RETAINED_WEIGHT_6_1 == null && item.CUMULATIVE_WEIGHT_6_1 == null && item.PASSING_WEIGHT_6_1 == null)
                        {
                            TRobjViewModel.TYPEB_DETAIL_PAGE6_BASE_COURSE_1_List.Remove(item);
                        }
                    }
                    if (TRobjViewModel.TYPEB_DETAIL_PAGE6_BASE_COURSE_1_List.Count == 0)
                    {

                        errorListtemp.Clear();
                        errorListtemp.Add("Page-6: Item 4.1.BASE COURSE: 1st Layer - Gradation Analysis of Aggregates: Table -Please enter Wt. of Sample Retained(g) for atleast one I. S Sieve Designation for Chainage-I");
                        TRobjViewModel.ErrorOccured = true;
                        TRobjViewModel.ErrorList.AddRange(errorListtemp);
                        errorListtemp.Clear();
                    }



                    #endregion
                }

                if (naCheckArr[14] == 'Y' && ((BaseCourtse1.SelectedValue == "8") || (BaseCourtse1.SelectedValue == "9") || (BaseCourtse1.SelectedValue == "10")))
                {
                    #region Page -6, 4 - 4.2 BASE COURSE: 1st Layer - Chaninage - 2

                    errorListtemp = objCommonFunction.FetchTetsReportPDFFilledDataToModel(TRobjViewModel.TYPEB_SUMMARY_PAGE6_BASE_COURSE_2, loadedForm);
                    TRobjViewModel.ErrorList.AddRange(errorListtemp);
                    TRobjViewModel.TYPEB_SUMMARY_PAGE6_BASE_COURSE_2.SUBITEM_ID = typeBSummaryPage6BaseCourse14AssignModel.Model.SUBITEM_ID;
                    TRobjViewModel.TYPEB_SUMMARY_PAGE6_BASE_COURSE_2.MAIN_ITEM_ID = typeBSummaryPage6BaseCourse14AssignModel.Model.MAIN_ITEM_ID;
                    TRobjViewModel.TYPEB_SUMMARY_PAGE6_BASE_COURSE_2.TABLE_ID = typeBSummaryPage6BaseCourse14AssignModel.Model.TABLE_ID;
                    int RowNo = 1;
                    dbContext.EFORM_TR_DETAIL_ITEM_MASTER.Where(m => m.MAIN_ITEM_ID == typeBSummaryPage6BaseCourse14AssignModel.Model.MAIN_ITEM_ID && m.SUBITEM_ID == typeBSummaryPage6BaseCourse14AssignModel.Model.SUBITEM_ID && m.OPTION_ID == OptionItemID).Select(x => x.DETAIL_ITEM_ID).ToList().ForEach(itemID =>
                    {
                        EFORM_TR_TYPEB_DETAIL_PAGE6_BASE_COURSE_2 detailedItem = new EFORM_TR_TYPEB_DETAIL_PAGE6_BASE_COURSE_2();
                        detailedItem.MAIN_ITEM_ID = typeBSummaryPage6BaseCourse14AssignModel.Model.MAIN_ITEM_ID;
                        detailedItem.SUBITEM_ID = typeBSummaryPage6BaseCourse14AssignModel.Model.SUBITEM_ID;
                        detailedItem.DETAIL_ITEM_ID = itemID;
                        detailedItem.TABLE_ID = typeBSummaryPage6BaseCourse14AssignModel.Model.TABLE_ID;
                        detailedItem.ROW_ID = RowNo;
                        TRobjViewModel.TYPEB_DETAIL_PAGE6_BASE_COURSE_2_List.Add(detailedItem);
                        RowNo++;
                    });
                    EFORM_TR_TYPEB_DETAIL_PAGE6_BASE_COURSE_2 detailedItemPAN = new EFORM_TR_TYPEB_DETAIL_PAGE6_BASE_COURSE_2();
                    detailedItemPAN.MAIN_ITEM_ID = typeBSummaryPage6BaseCourse14AssignModel.Model.MAIN_ITEM_ID;
                    detailedItemPAN.SUBITEM_ID = typeBSummaryPage6BaseCourse14AssignModel.Model.SUBITEM_ID;
                    detailedItemPAN.DETAIL_ITEM_ID = typeBSummaryPage6BaseCourse14AssignModel.Model.PAN; ;
                    detailedItemPAN.TABLE_ID = typeBSummaryPage6BaseCourse14AssignModel.Model.TABLE_ID;
                    detailedItemPAN.ROW_ID = 9;
                    TRobjViewModel.TYPEB_DETAIL_PAGE6_BASE_COURSE_2_List.Add(detailedItemPAN);

                    foreach (var item in TRobjViewModel.TYPEB_DETAIL_PAGE6_BASE_COURSE_2_List.ToList())
                    {
                        errorListtemp = objCommonFunction.FetchTetsReportPDFFilledDataToModel(item, loadedForm);
                        TRobjViewModel.ErrorList.AddRange(errorListtemp);
                    }

                    foreach (var item in TRobjViewModel.TYPEB_DETAIL_PAGE6_BASE_COURSE_2_List.ToList())
                    {
                        if (item.SAMPLE_WEIGHT_6_2 == null && item.RETAINED_WEIGHT_6_2 == null && item.CUMULATIVE_WEIGHT_6_2 == null && item.PASSING_WEIGHT_6_2 == null)
                        {
                            TRobjViewModel.TYPEB_DETAIL_PAGE6_BASE_COURSE_2_List.Remove(item);
                        }
                    }
                    if (TRobjViewModel.TYPEB_DETAIL_PAGE6_BASE_COURSE_2_List.Count == 0)
                    {

                        errorListtemp.Clear();
                        errorListtemp.Add("Page-6: Item 4.2.BASE COURSE: 1st Layer - Gradation Analysis of Aggregates: Table -Please enter Wt. of Sample Retained(g) for atleast one I. S Sieve Designation for Chainage-II");
                        TRobjViewModel.ErrorOccured = true;
                        TRobjViewModel.ErrorList.AddRange(errorListtemp);
                        errorListtemp.Clear();
                    }
                    #endregion
                }

                if (naCheckArr[15] == 'Y' && ((BaseCourtse1.SelectedValue == "8") || (BaseCourtse1.SelectedValue == "9") || (BaseCourtse1.SelectedValue == "10")))
                {
                    #region Page -6, 4 - 4.3 BASE COURSE: 1st Layer - Chaninage - 3 

                    errorListtemp = objCommonFunction.FetchTetsReportPDFFilledDataToModel(TRobjViewModel.TYPEB_SUMMARY_PAGE6_BASE_COURSE_3, loadedForm);
                    TRobjViewModel.ErrorList.AddRange(errorListtemp);
                    TRobjViewModel.TYPEB_SUMMARY_PAGE6_BASE_COURSE_3.SUBITEM_ID = typeBSummaryPage6BaseCourse15AssignModel.Model.SUBITEM_ID;
                    TRobjViewModel.TYPEB_SUMMARY_PAGE6_BASE_COURSE_3.MAIN_ITEM_ID = typeBSummaryPage6BaseCourse15AssignModel.Model.MAIN_ITEM_ID;
                    TRobjViewModel.TYPEB_SUMMARY_PAGE6_BASE_COURSE_3.TABLE_ID = typeBSummaryPage6BaseCourse15AssignModel.Model.TABLE_ID;
                    int RowNo = 1;
                    dbContext.EFORM_TR_DETAIL_ITEM_MASTER.Where(m => m.MAIN_ITEM_ID == typeBSummaryPage6BaseCourse15AssignModel.Model.MAIN_ITEM_ID && m.SUBITEM_ID == typeBSummaryPage6BaseCourse15AssignModel.Model.SUBITEM_ID && m.OPTION_ID == OptionItemID).Select(x => x.DETAIL_ITEM_ID).ToList().ForEach(itemID =>
                    {
                        EFORM_TR_TYPEB_DETAIL_PAGE6_BASE_COURSE_3 detailedItem = new EFORM_TR_TYPEB_DETAIL_PAGE6_BASE_COURSE_3();
                        detailedItem.MAIN_ITEM_ID = typeBSummaryPage6BaseCourse15AssignModel.Model.MAIN_ITEM_ID;
                        detailedItem.SUBITEM_ID = typeBSummaryPage6BaseCourse15AssignModel.Model.SUBITEM_ID;
                        detailedItem.DETAIL_ITEM_ID = itemID;
                        detailedItem.TABLE_ID = typeBSummaryPage6BaseCourse15AssignModel.Model.TABLE_ID;
                        detailedItem.ROW_ID = RowNo;
                        TRobjViewModel.TYPEB_DETAIL_PAGE6_BASE_COURSE_3_List.Add(detailedItem);
                        RowNo++;
                    });
                    EFORM_TR_TYPEB_DETAIL_PAGE6_BASE_COURSE_3 detailedItemPAN = new EFORM_TR_TYPEB_DETAIL_PAGE6_BASE_COURSE_3();
                    detailedItemPAN.MAIN_ITEM_ID = typeBSummaryPage6BaseCourse15AssignModel.Model.MAIN_ITEM_ID;
                    detailedItemPAN.SUBITEM_ID = typeBSummaryPage6BaseCourse15AssignModel.Model.SUBITEM_ID;
                    detailedItemPAN.DETAIL_ITEM_ID = typeBSummaryPage6BaseCourse15AssignModel.Model.PAN; ;
                    detailedItemPAN.TABLE_ID = typeBSummaryPage6BaseCourse15AssignModel.Model.TABLE_ID;
                    detailedItemPAN.ROW_ID = 9;
                    TRobjViewModel.TYPEB_DETAIL_PAGE6_BASE_COURSE_3_List.Add(detailedItemPAN);

                    foreach (var item in TRobjViewModel.TYPEB_DETAIL_PAGE6_BASE_COURSE_3_List.ToList())
                    {
                        errorListtemp = objCommonFunction.FetchTetsReportPDFFilledDataToModel(item, loadedForm);
                        TRobjViewModel.ErrorList.AddRange(errorListtemp);
                    }

                    foreach (var item in TRobjViewModel.TYPEB_DETAIL_PAGE6_BASE_COURSE_3_List.ToList())
                    {
                        if (item.SAMPLE_WEIGHT_6_3 == null && item.RETAINED_WEIGHT_6_3 == null && item.CUMULATIVE_WEIGHT_6_3 == null && item.PASSING_WEIGHT_6_3 == null)
                        {
                            TRobjViewModel.TYPEB_DETAIL_PAGE6_BASE_COURSE_3_List.Remove(item);
                        }
                    }
                    if (TRobjViewModel.TYPEB_DETAIL_PAGE6_BASE_COURSE_3_List.Count == 0)
                    {

                        errorListtemp.Clear();
                        errorListtemp.Add("Page-6: Item 4.3.BASE COURSE: 1st Layer - Gradation Analysis of Aggregates: Table -Please enter Wt. of Sample Retained(g) for atleast one I. S Sieve Designation for Chainage-III");
                        TRobjViewModel.ErrorOccured = true;
                        TRobjViewModel.ErrorList.AddRange(errorListtemp);
                        errorListtemp.Clear();
                    }
                    #endregion
                }

                #endregion

                #region  e-Form Test Report Page-8 BASE COURSE: 2nd Layer

                #region  To get Detail Item Dynamically
                int OptionItem_ID = 0;

                PdfLoadedRadioButtonListField BaseCourtse2 = loadedForm.Fields["Base_Course_2"] as PdfLoadedRadioButtonListField;
                OptionItem_ID = Convert.ToInt32(BaseCourtse2.SelectedValue);
                if (OptionItem_ID == 0)
                {
                    errorListtemp.Clear();
                    errorListtemp.Add("Page-8: BASE COURSE: 2nd Layer: Please Select Base Course 2nd Layer Type");
                    TRobjViewModel.ErrorOccured = true;
                    TRobjViewModel.ErrorList.AddRange(errorListtemp);
                    errorListtemp.Clear();
                }
                else
                {
                    var OptionMasterModel = dbContext.EFORM_TR_ITEM_OPTIONS_MASTER.Where(x => x.OPTION_ID == OptionItem_ID).Select(x => new { x.SUBITEM_ID, x.MAIN_ITEM_ID }).FirstOrDefault();
                    TestReportModel.EFORM_TR_MAIN_ITEM_OPTIONS_SELECTED_DETAIL modelSub = new TestReportModel.EFORM_TR_MAIN_ITEM_OPTIONS_SELECTED_DETAIL();
                    modelSub.MAIN_ITEM_ID = typeBSummaryPage8BaseCourse18AssignModel.Model.MAIN_ITEM_ID;
                    modelSub.OPTION_ID = OptionItem_ID;
                    TRobjViewModel.MAIN_ITEM_OPTIONS_SELECTED_DETAIL_List.Add(modelSub);
                }

                #endregion

                if (naCheckArr[18] == 'Y' && ((BaseCourtse2.SelectedValue == "14") || (BaseCourtse2.SelectedValue == "15") || (BaseCourtse2.SelectedValue == "16")))
                {
                    #region Page -8, 7 - 7.1 BASE COURSE: 2nd Layer - Chaninage - 1

                    errorListtemp = objCommonFunction.FetchTetsReportPDFFilledDataToModel(TRobjViewModel.TYPEB_SUMMARY_PAGE8_BASE_COURSE2_1, loadedForm);
                    TRobjViewModel.ErrorList.AddRange(errorListtemp);
                    TRobjViewModel.TYPEB_SUMMARY_PAGE8_BASE_COURSE2_1.SUBITEM_ID = typeBSummaryPage8BaseCourse18AssignModel.Model.SUBITEM_ID;
                    TRobjViewModel.TYPEB_SUMMARY_PAGE8_BASE_COURSE2_1.MAIN_ITEM_ID = typeBSummaryPage8BaseCourse18AssignModel.Model.MAIN_ITEM_ID;
                    TRobjViewModel.TYPEB_SUMMARY_PAGE8_BASE_COURSE2_1.TABLE_ID = typeBSummaryPage8BaseCourse18AssignModel.Model.TABLE_ID;
                    int RowNo = 1;
                    dbContext.EFORM_TR_DETAIL_ITEM_MASTER.Where(m => m.MAIN_ITEM_ID == typeBSummaryPage8BaseCourse18AssignModel.Model.MAIN_ITEM_ID && m.SUBITEM_ID == typeBSummaryPage8BaseCourse18AssignModel.Model.SUBITEM_ID && m.OPTION_ID == OptionItem_ID).Select(x => x.DETAIL_ITEM_ID).ToList().ForEach(itemID =>
                    {
                        EFORM_TR_TYPEB_DETAIL_PAGE8_BASE_COURSE2_1 detailedItem = new EFORM_TR_TYPEB_DETAIL_PAGE8_BASE_COURSE2_1();
                        detailedItem.MAIN_ITEM_ID = typeBSummaryPage8BaseCourse18AssignModel.Model.MAIN_ITEM_ID;
                        detailedItem.SUBITEM_ID = typeBSummaryPage8BaseCourse18AssignModel.Model.SUBITEM_ID;
                        detailedItem.DETAIL_ITEM_ID = itemID;
                        detailedItem.TABLE_ID = typeBSummaryPage8BaseCourse18AssignModel.Model.TABLE_ID;
                        detailedItem.ROW_ID = RowNo;
                        TRobjViewModel.TYPEB_DETAIL_PAGE8_BASE_COURSE2_1_List.Add(detailedItem);
                        RowNo++;
                    });
                    EFORM_TR_TYPEB_DETAIL_PAGE8_BASE_COURSE2_1 detailedItemPAN = new EFORM_TR_TYPEB_DETAIL_PAGE8_BASE_COURSE2_1();
                    detailedItemPAN.MAIN_ITEM_ID = typeBSummaryPage8BaseCourse18AssignModel.Model.MAIN_ITEM_ID;
                    detailedItemPAN.SUBITEM_ID = typeBSummaryPage8BaseCourse18AssignModel.Model.SUBITEM_ID;
                    detailedItemPAN.DETAIL_ITEM_ID = typeBSummaryPage8BaseCourse18AssignModel.Model.PAN;
                    detailedItemPAN.TABLE_ID = typeBSummaryPage8BaseCourse18AssignModel.Model.TABLE_ID;
                    detailedItemPAN.ROW_ID = 9;
                    TRobjViewModel.TYPEB_DETAIL_PAGE8_BASE_COURSE2_1_List.Add(detailedItemPAN);

                    foreach (var item in TRobjViewModel.TYPEB_DETAIL_PAGE8_BASE_COURSE2_1_List.ToList())
                    {
                        errorListtemp = objCommonFunction.FetchTetsReportPDFFilledDataToModel(item, loadedForm);
                        TRobjViewModel.ErrorList.AddRange(errorListtemp);
                    }

                    foreach (var item in TRobjViewModel.TYPEB_DETAIL_PAGE8_BASE_COURSE2_1_List.ToList())
                    {
                        if (item.SAMPLE_WEIGHT_8_1 == null && item.RETAINED_WEIGHT_8_1 == null && item.CUMULATIVE_WEIGHT_8_1 == null && item.PASSING_WEIGHT_8_1 == null)
                        {
                            TRobjViewModel.TYPEB_DETAIL_PAGE8_BASE_COURSE2_1_List.Remove(item);
                        }
                    }
                    if (TRobjViewModel.TYPEB_DETAIL_PAGE8_BASE_COURSE2_1_List.Count == 0)
                    {

                        errorListtemp.Clear();
                        errorListtemp.Add("Page-8: Item 7.1.BASE COURSE: 2nd Layer - Gradation Analysis of Aggregates: Table -Please enter Wt. of Sample Retained(g) for atleast one I. S. Sieve Designation for Chainage-I");
                        TRobjViewModel.ErrorOccured = true;
                        TRobjViewModel.ErrorList.AddRange(errorListtemp);
                        errorListtemp.Clear();
                    }
                    #endregion
                }

                if (naCheckArr[19] == 'Y' && ((BaseCourtse2.SelectedValue == "14") || (BaseCourtse2.SelectedValue == "15") || (BaseCourtse2.SelectedValue == "16")))
                {
                    #region Page -8, 7 - 7.2 BASE COURSE: 2nd Layer - Chaninage - 2             

                    errorListtemp = objCommonFunction.FetchTetsReportPDFFilledDataToModel(TRobjViewModel.TYPEB_SUMMARY_PAGE8_BASE_COURSE2_2, loadedForm);
                    TRobjViewModel.ErrorList.AddRange(errorListtemp);
                    TRobjViewModel.TYPEB_SUMMARY_PAGE8_BASE_COURSE2_2.SUBITEM_ID = typeBSummaryPage8BaseCourse19AssignModel.Model.SUBITEM_ID;
                    TRobjViewModel.TYPEB_SUMMARY_PAGE8_BASE_COURSE2_2.MAIN_ITEM_ID = typeBSummaryPage8BaseCourse19AssignModel.Model.MAIN_ITEM_ID;
                    TRobjViewModel.TYPEB_SUMMARY_PAGE8_BASE_COURSE2_2.TABLE_ID = typeBSummaryPage8BaseCourse19AssignModel.Model.TABLE_ID;
                    int RowNo = 1;
                    dbContext.EFORM_TR_DETAIL_ITEM_MASTER.Where(m => m.MAIN_ITEM_ID == typeBSummaryPage8BaseCourse19AssignModel.Model.MAIN_ITEM_ID && m.SUBITEM_ID == typeBSummaryPage8BaseCourse19AssignModel.Model.SUBITEM_ID && m.OPTION_ID == OptionItem_ID).Select(x => x.DETAIL_ITEM_ID).ToList().ForEach(itemID =>
                    {
                        EFORM_TR_TYPEB_DETAIL_PAGE8_BASE_COURSE2_2 detailedItem = new EFORM_TR_TYPEB_DETAIL_PAGE8_BASE_COURSE2_2();
                        detailedItem.MAIN_ITEM_ID = typeBSummaryPage8BaseCourse19AssignModel.Model.MAIN_ITEM_ID;
                        detailedItem.SUBITEM_ID = typeBSummaryPage8BaseCourse19AssignModel.Model.SUBITEM_ID;
                        detailedItem.DETAIL_ITEM_ID = itemID;
                        detailedItem.TABLE_ID = typeBSummaryPage8BaseCourse19AssignModel.Model.TABLE_ID;
                        detailedItem.ROW_ID = RowNo;
                        TRobjViewModel.TYPEB_DETAIL_PAGE8_BASE_COURSE2_2_List.Add(detailedItem);
                        RowNo++;
                    });
                    EFORM_TR_TYPEB_DETAIL_PAGE8_BASE_COURSE2_2 detailedItemPAN = new EFORM_TR_TYPEB_DETAIL_PAGE8_BASE_COURSE2_2();
                    detailedItemPAN.MAIN_ITEM_ID = typeBSummaryPage8BaseCourse19AssignModel.Model.MAIN_ITEM_ID;
                    detailedItemPAN.SUBITEM_ID = typeBSummaryPage8BaseCourse19AssignModel.Model.SUBITEM_ID;
                    detailedItemPAN.DETAIL_ITEM_ID = typeBSummaryPage8BaseCourse19AssignModel.Model.PAN;
                    detailedItemPAN.TABLE_ID = typeBSummaryPage8BaseCourse19AssignModel.Model.TABLE_ID;
                    detailedItemPAN.ROW_ID = 9;
                    TRobjViewModel.TYPEB_DETAIL_PAGE8_BASE_COURSE2_2_List.Add(detailedItemPAN);

                    foreach (var item in TRobjViewModel.TYPEB_DETAIL_PAGE8_BASE_COURSE2_2_List.ToList())
                    {
                        errorListtemp = objCommonFunction.FetchTetsReportPDFFilledDataToModel(item, loadedForm);
                        TRobjViewModel.ErrorList.AddRange(errorListtemp);
                    }

                    foreach (var item in TRobjViewModel.TYPEB_DETAIL_PAGE8_BASE_COURSE2_2_List.ToList())
                    {
                        if (item.SAMPLE_WEIGHT_8_2 == null && item.RETAINED_WEIGHT_8_2 == null && item.CUMULATIVE_WEIGHT_8_2 == null && item.PASSING_WEIGHT_8_2 == null)
                        {
                            TRobjViewModel.TYPEB_DETAIL_PAGE8_BASE_COURSE2_2_List.Remove(item);
                        }
                    }
                    if (TRobjViewModel.TYPEB_DETAIL_PAGE8_BASE_COURSE2_2_List.Count == 0)
                    {

                        errorListtemp.Clear();
                        errorListtemp.Add("Page-8: Item 7.2.BASE COURSE: 2nd Layer - Gradation Analysis of Aggregates: Table -Please enter Wt. of Sample Retained(g) for atleast one I. S. Sieve Designation for Chainage-II");
                        TRobjViewModel.ErrorOccured = true;
                        TRobjViewModel.ErrorList.AddRange(errorListtemp);
                        errorListtemp.Clear();
                    }
                    #endregion
                }

                if (naCheckArr[20] == 'Y' && ((BaseCourtse2.SelectedValue == "14") || (BaseCourtse2.SelectedValue == "15") || (BaseCourtse2.SelectedValue == "16")))
                {
                    #region Page -8, 7 - 7.3 BASE COURSE: 2nd Layer - Chaninage - 3

                    errorListtemp = objCommonFunction.FetchTetsReportPDFFilledDataToModel(TRobjViewModel.TYPEB_SUMMARY_PAGE8_BASE_COURSE2_3, loadedForm);
                    TRobjViewModel.ErrorList.AddRange(errorListtemp);
                    TRobjViewModel.TYPEB_SUMMARY_PAGE8_BASE_COURSE2_3.SUBITEM_ID = typeBSummaryPage8BaseCourse20AssignModel.Model.SUBITEM_ID;
                    TRobjViewModel.TYPEB_SUMMARY_PAGE8_BASE_COURSE2_3.MAIN_ITEM_ID = typeBSummaryPage8BaseCourse20AssignModel.Model.MAIN_ITEM_ID;
                    TRobjViewModel.TYPEB_SUMMARY_PAGE8_BASE_COURSE2_3.TABLE_ID = typeBSummaryPage8BaseCourse20AssignModel.Model.TABLE_ID;
                    int RowNo = 1;
                    dbContext.EFORM_TR_DETAIL_ITEM_MASTER.Where(m => m.MAIN_ITEM_ID == typeBSummaryPage8BaseCourse20AssignModel.Model.MAIN_ITEM_ID && m.SUBITEM_ID == typeBSummaryPage8BaseCourse20AssignModel.Model.SUBITEM_ID && m.OPTION_ID == OptionItem_ID).Select(x => x.DETAIL_ITEM_ID).ToList().ForEach(itemID =>
                    {
                        EFORM_TR_TYPEB_DETAIL_PAGE8_BASE_COURSE2_3 detailedItem = new EFORM_TR_TYPEB_DETAIL_PAGE8_BASE_COURSE2_3();
                        detailedItem.MAIN_ITEM_ID = typeBSummaryPage8BaseCourse20AssignModel.Model.MAIN_ITEM_ID;
                        detailedItem.SUBITEM_ID = typeBSummaryPage8BaseCourse20AssignModel.Model.SUBITEM_ID;
                        detailedItem.DETAIL_ITEM_ID = itemID;
                        detailedItem.TABLE_ID = typeBSummaryPage8BaseCourse20AssignModel.Model.TABLE_ID;
                        detailedItem.ROW_ID = RowNo;
                        TRobjViewModel.TYPEB_DETAIL_PAGE8_BASE_COURSE2_3_List.Add(detailedItem);
                        RowNo++;
                    });
                    EFORM_TR_TYPEB_DETAIL_PAGE8_BASE_COURSE2_3 detailedItemPAN = new EFORM_TR_TYPEB_DETAIL_PAGE8_BASE_COURSE2_3();
                    detailedItemPAN.MAIN_ITEM_ID = typeBSummaryPage8BaseCourse20AssignModel.Model.MAIN_ITEM_ID;
                    detailedItemPAN.SUBITEM_ID = typeBSummaryPage8BaseCourse20AssignModel.Model.SUBITEM_ID;
                    detailedItemPAN.DETAIL_ITEM_ID = typeBSummaryPage8BaseCourse20AssignModel.Model.PAN;
                    detailedItemPAN.TABLE_ID = typeBSummaryPage8BaseCourse20AssignModel.Model.TABLE_ID;
                    detailedItemPAN.ROW_ID = 9;
                    TRobjViewModel.TYPEB_DETAIL_PAGE8_BASE_COURSE2_3_List.Add(detailedItemPAN);

                    foreach (var item in TRobjViewModel.TYPEB_DETAIL_PAGE8_BASE_COURSE2_3_List.ToList())
                    {
                        errorListtemp = objCommonFunction.FetchTetsReportPDFFilledDataToModel(item, loadedForm);
                        TRobjViewModel.ErrorList.AddRange(errorListtemp);
                    }

                    foreach (var item in TRobjViewModel.TYPEB_DETAIL_PAGE8_BASE_COURSE2_3_List.ToList())
                    {
                        if (item.SAMPLE_WEIGHT_8_3 == null && item.RETAINED_WEIGHT_8_3 == null && item.CUMULATIVE_WEIGHT_8_3 == null && item.PASSING_WEIGHT_8_3 == null)
                        {
                            TRobjViewModel.TYPEB_DETAIL_PAGE8_BASE_COURSE2_3_List.Remove(item);
                        }
                    }
                    if (TRobjViewModel.TYPEB_DETAIL_PAGE8_BASE_COURSE2_3_List.Count == 0)
                    {

                        errorListtemp.Clear();
                        errorListtemp.Add("Page-8: Item 7.3.BASE COURSE: 2nd Layer - Gradation Analysis of Aggregates: Table -Please enter Wt. of Sample Retained(g) for atleast one I. S. Sieve Designation for Chainage-III");
                        TRobjViewModel.ErrorOccured = true;
                        TRobjViewModel.ErrorList.AddRange(errorListtemp);
                        errorListtemp.Clear();
                    }
                    #endregion
                }

                #endregion

                #region e-Form Test Report Page-10 BASE COURSE: 3rd Layer

                #region  To get Detail Item Dynamically
                int OptionID_BaseCourse3 = 0;


                PdfLoadedRadioButtonListField BaseCourtse3 = loadedForm.Fields["Base_Course_3"] as PdfLoadedRadioButtonListField;
                OptionID_BaseCourse3 = Convert.ToInt32(BaseCourtse3.SelectedValue);
                if (OptionID_BaseCourse3 == 0)
                {
                    errorListtemp.Clear();
                    errorListtemp.Add("Page-10: BASE COURSE: 3rd Layer: Please Select Base Course 3rd Layer Type");
                    TRobjViewModel.ErrorOccured = true;
                    TRobjViewModel.ErrorList.AddRange(errorListtemp);
                    errorListtemp.Clear();
                }
                else
                {
                    var OptionMasterModel = dbContext.EFORM_TR_ITEM_OPTIONS_MASTER.Where(x => x.OPTION_ID == OptionID_BaseCourse3).Select(x => new { x.SUBITEM_ID, x.MAIN_ITEM_ID }).FirstOrDefault();
                    TestReportModel.EFORM_TR_MAIN_ITEM_OPTIONS_SELECTED_DETAIL modelSub = new TestReportModel.EFORM_TR_MAIN_ITEM_OPTIONS_SELECTED_DETAIL();
                    modelSub.MAIN_ITEM_ID = typeBSummaryPage10BaseCourse23AssignModel.Model.MAIN_ITEM_ID;
                    modelSub.OPTION_ID = OptionID_BaseCourse3;
                    TRobjViewModel.MAIN_ITEM_OPTIONS_SELECTED_DETAIL_List.Add(modelSub);
                }
                #endregion

                if (naCheckArr[23] == 'Y' && (BaseCourtse3.SelectedValue == "20") || (BaseCourtse3.SelectedValue == "21") || (BaseCourtse3.SelectedValue == "22"))
                {
                    errorListtemp = objCommonFunction.FetchTetsReportPDFFilledDataToModel(TRobjViewModel.TYPEB_SUMMARY_PAGE10_BASE_COURSE3_1, loadedForm);
                    TRobjViewModel.ErrorList.AddRange(errorListtemp);
                    TRobjViewModel.TYPEB_SUMMARY_PAGE10_BASE_COURSE3_1.MAIN_ITEM_ID = typeBSummaryPage10BaseCourse23AssignModel.Model.MAIN_ITEM_ID;
                    TRobjViewModel.TYPEB_SUMMARY_PAGE10_BASE_COURSE3_1.SUBITEM_ID = typeBSummaryPage10BaseCourse23AssignModel.Model.SUBITEM_ID;
                    TRobjViewModel.TYPEB_SUMMARY_PAGE10_BASE_COURSE3_1.TABLE_ID = typeBSummaryPage10BaseCourse23AssignModel.Model.TABLE_ID;

                    int RowNo = 1;
                    dbContext.EFORM_TR_DETAIL_ITEM_MASTER.Where(m => m.MAIN_ITEM_ID == typeBSummaryPage10BaseCourse23AssignModel.Model.MAIN_ITEM_ID && m.SUBITEM_ID == typeBSummaryPage10BaseCourse23AssignModel.Model.SUBITEM_ID && m.OPTION_ID == OptionID_BaseCourse3).Select(x => x.DETAIL_ITEM_ID).ToList().ForEach(itemID =>
                    {
                        EFORM_TR_TYPEB_DETAIL_PAGE10_BASE_COURSE3_1 detailedItem = new EFORM_TR_TYPEB_DETAIL_PAGE10_BASE_COURSE3_1();
                        detailedItem.MAIN_ITEM_ID = typeBSummaryPage10BaseCourse23AssignModel.Model.MAIN_ITEM_ID;
                        detailedItem.SUBITEM_ID = typeBSummaryPage10BaseCourse23AssignModel.Model.SUBITEM_ID;
                        detailedItem.DETAIL_ITEM_ID = itemID;
                        detailedItem.TABLE_ID = typeBSummaryPage10BaseCourse23AssignModel.Model.TABLE_ID;
                        detailedItem.ROW_ID = RowNo;
                        TRobjViewModel.TYPEB_DETAIL_PAGE10_BASE_COURSE3_1_List.Add(detailedItem);
                        RowNo++;
                    });
                    EFORM_TR_TYPEB_DETAIL_PAGE10_BASE_COURSE3_1 detailedItemPAN = new EFORM_TR_TYPEB_DETAIL_PAGE10_BASE_COURSE3_1();
                    detailedItemPAN.MAIN_ITEM_ID = typeBSummaryPage10BaseCourse23AssignModel.Model.MAIN_ITEM_ID;
                    detailedItemPAN.SUBITEM_ID = typeBSummaryPage10BaseCourse23AssignModel.Model.SUBITEM_ID;
                    detailedItemPAN.DETAIL_ITEM_ID = typeBSummaryPage10BaseCourse23AssignModel.Model.PAN;
                    detailedItemPAN.TABLE_ID = typeBSummaryPage10BaseCourse23AssignModel.Model.TABLE_ID;
                    detailedItemPAN.ROW_ID = 9;
                    TRobjViewModel.TYPEB_DETAIL_PAGE10_BASE_COURSE3_1_List.Add(detailedItemPAN);

                    foreach (var item in TRobjViewModel.TYPEB_DETAIL_PAGE10_BASE_COURSE3_1_List.ToList())
                    {
                        errorListtemp = objCommonFunction.FetchTetsReportPDFFilledDataToModel(item, loadedForm);
                        TRobjViewModel.ErrorList.AddRange(errorListtemp);
                    }

                    foreach (var item in TRobjViewModel.TYPEB_DETAIL_PAGE10_BASE_COURSE3_1_List.ToList())
                    {
                        if (item.SAMPLE_WEIGHT_10_1 == null && item.RETAINED_WEIGHT_10_1 == null && item.CUMULATIVE_WEIGHT_10_1 == null && item.PASSING_WEIGHT_10_1 == null)
                        {
                            TRobjViewModel.TYPEB_DETAIL_PAGE10_BASE_COURSE3_1_List.Remove(item);
                        }
                    }
                    if (TRobjViewModel.TYPEB_DETAIL_PAGE10_BASE_COURSE3_1_List.Count == 0)
                    {

                        errorListtemp.Clear();
                        errorListtemp.Add("Page-10: Item 10.1.BASE COURSE: 3rd Layer - Gradation Analysis of Aggregates:Table -Please enter Wt. of Sample Retained(g) for atleast one I. S. Sieve Designation for Chainage-I");
                        TRobjViewModel.ErrorOccured = true;
                        TRobjViewModel.ErrorList.AddRange(errorListtemp);
                        errorListtemp.Clear();
                    }
                }

                if (naCheckArr[24] == 'Y' && ((BaseCourtse3.SelectedValue == "20") || (BaseCourtse3.SelectedValue == "21") || (BaseCourtse3.SelectedValue == "22")))
                {
                    errorListtemp = objCommonFunction.FetchTetsReportPDFFilledDataToModel(TRobjViewModel.TYPEB_SUMMARY_PAGE10_BASE_COURSE3_2, loadedForm);
                    TRobjViewModel.ErrorList.AddRange(errorListtemp);
                    TRobjViewModel.TYPEB_SUMMARY_PAGE10_BASE_COURSE3_2.MAIN_ITEM_ID = typeBSummaryPage10BaseCourse24AssignModel.Model.MAIN_ITEM_ID;
                    TRobjViewModel.TYPEB_SUMMARY_PAGE10_BASE_COURSE3_2.SUBITEM_ID = typeBSummaryPage10BaseCourse24AssignModel.Model.SUBITEM_ID;
                    TRobjViewModel.TYPEB_SUMMARY_PAGE10_BASE_COURSE3_2.TABLE_ID = typeBSummaryPage10BaseCourse24AssignModel.Model.TABLE_ID;

                    int RowNo = 1;
                    dbContext.EFORM_TR_DETAIL_ITEM_MASTER.Where(m => m.MAIN_ITEM_ID == typeBSummaryPage10BaseCourse24AssignModel.Model.MAIN_ITEM_ID && m.SUBITEM_ID == typeBSummaryPage10BaseCourse24AssignModel.Model.SUBITEM_ID && m.OPTION_ID == OptionID_BaseCourse3).Select(x => x.DETAIL_ITEM_ID).ToList().ForEach(itemID =>
                    {
                        EFORM_TR_TYPEB_DETAIL_PAGE10_BASE_COURSE3_2 detailedItem = new EFORM_TR_TYPEB_DETAIL_PAGE10_BASE_COURSE3_2();
                        detailedItem.MAIN_ITEM_ID = typeBSummaryPage10BaseCourse24AssignModel.Model.MAIN_ITEM_ID;
                        detailedItem.SUBITEM_ID = typeBSummaryPage10BaseCourse24AssignModel.Model.SUBITEM_ID;
                        detailedItem.DETAIL_ITEM_ID = itemID;
                        detailedItem.TABLE_ID = typeBSummaryPage10BaseCourse24AssignModel.Model.TABLE_ID;
                        detailedItem.ROW_ID = RowNo;
                        TRobjViewModel.TYPEB_DETAIL_PAGE10_BASE_COURSE3_2_List.Add(detailedItem);
                        RowNo++;
                    });
                    EFORM_TR_TYPEB_DETAIL_PAGE10_BASE_COURSE3_2 detailedItemPAN = new EFORM_TR_TYPEB_DETAIL_PAGE10_BASE_COURSE3_2();
                    detailedItemPAN.MAIN_ITEM_ID = typeBSummaryPage10BaseCourse24AssignModel.Model.MAIN_ITEM_ID;
                    detailedItemPAN.SUBITEM_ID = typeBSummaryPage10BaseCourse24AssignModel.Model.SUBITEM_ID;
                    detailedItemPAN.DETAIL_ITEM_ID = typeBSummaryPage10BaseCourse24AssignModel.Model.PAN;
                    detailedItemPAN.TABLE_ID = typeBSummaryPage10BaseCourse24AssignModel.Model.TABLE_ID;
                    detailedItemPAN.ROW_ID = 9;
                    TRobjViewModel.TYPEB_DETAIL_PAGE10_BASE_COURSE3_2_List.Add(detailedItemPAN);

                    foreach (var item in TRobjViewModel.TYPEB_DETAIL_PAGE10_BASE_COURSE3_2_List.ToList())
                    {
                        errorListtemp = objCommonFunction.FetchTetsReportPDFFilledDataToModel(item, loadedForm);
                        TRobjViewModel.ErrorList.AddRange(errorListtemp);
                    }

                    foreach (var item in TRobjViewModel.TYPEB_DETAIL_PAGE10_BASE_COURSE3_2_List.ToList())
                    {
                        if (item.SAMPLE_WEIGHT_10_2 == null && item.RETAINED_WEIGHT_10_2 == null && item.CUMULATIVE_WEIGHT_10_2 == null && item.PASSING_WEIGHT_10_2 == null)
                        {
                            TRobjViewModel.TYPEB_DETAIL_PAGE10_BASE_COURSE3_2_List.Remove(item);
                        }
                    }
                    if (TRobjViewModel.TYPEB_DETAIL_PAGE10_BASE_COURSE3_2_List.Count == 0)
                    {

                        errorListtemp.Clear();
                        errorListtemp.Add("Page-10: Item 10.2.BASE COURSE: 3rd Layer - Gradation Analysis of Aggregates:Table -Please enter Wt. of Sample Retained(g) for atleast one I. S. Sieve Designation for Chainage-II");
                        TRobjViewModel.ErrorOccured = true;
                        TRobjViewModel.ErrorList.AddRange(errorListtemp);
                        errorListtemp.Clear();
                    }
                }

                if (naCheckArr[25] == 'Y' && ((BaseCourtse3.SelectedValue == "20") || (BaseCourtse3.SelectedValue == "21") || (BaseCourtse3.SelectedValue == "22")))
                {
                    errorListtemp = objCommonFunction.FetchTetsReportPDFFilledDataToModel(TRobjViewModel.TYPEB_SUMMARY_PAGE10_BASE_COURSE3_3, loadedForm);
                    TRobjViewModel.ErrorList.AddRange(errorListtemp);
                    TRobjViewModel.TYPEB_SUMMARY_PAGE10_BASE_COURSE3_3.MAIN_ITEM_ID = typeBSummaryPage10BaseCourse25AssignModel.Model.MAIN_ITEM_ID;
                    TRobjViewModel.TYPEB_SUMMARY_PAGE10_BASE_COURSE3_3.SUBITEM_ID = typeBSummaryPage10BaseCourse25AssignModel.Model.SUBITEM_ID;
                    TRobjViewModel.TYPEB_SUMMARY_PAGE10_BASE_COURSE3_3.TABLE_ID = typeBSummaryPage10BaseCourse25AssignModel.Model.TABLE_ID;

                    int RowNo = 1;
                    dbContext.EFORM_TR_DETAIL_ITEM_MASTER.Where(m => m.MAIN_ITEM_ID == typeBSummaryPage10BaseCourse25AssignModel.Model.MAIN_ITEM_ID && m.SUBITEM_ID == typeBSummaryPage10BaseCourse25AssignModel.Model.SUBITEM_ID && m.OPTION_ID == OptionID_BaseCourse3).Select(x => x.DETAIL_ITEM_ID).ToList().ForEach(itemID =>
                    {
                        EFORM_TR_TYPEB_DETAIL_PAGE10_BASE_COURSE3_3 detailedItem = new EFORM_TR_TYPEB_DETAIL_PAGE10_BASE_COURSE3_3();
                        detailedItem.MAIN_ITEM_ID = typeBSummaryPage10BaseCourse25AssignModel.Model.MAIN_ITEM_ID;
                        detailedItem.SUBITEM_ID = typeBSummaryPage10BaseCourse25AssignModel.Model.SUBITEM_ID;
                        detailedItem.DETAIL_ITEM_ID = itemID;
                        detailedItem.TABLE_ID = typeBSummaryPage10BaseCourse25AssignModel.Model.TABLE_ID;
                        detailedItem.ROW_ID = RowNo;
                        TRobjViewModel.TYPEB_DETAIL_PAGE10_BASE_COURSE3_3_List.Add(detailedItem);
                        RowNo++;
                    });
                    EFORM_TR_TYPEB_DETAIL_PAGE10_BASE_COURSE3_3 detailedItemPAN = new EFORM_TR_TYPEB_DETAIL_PAGE10_BASE_COURSE3_3();
                    detailedItemPAN.MAIN_ITEM_ID = typeBSummaryPage10BaseCourse25AssignModel.Model.MAIN_ITEM_ID;
                    detailedItemPAN.SUBITEM_ID = typeBSummaryPage10BaseCourse25AssignModel.Model.SUBITEM_ID;
                    detailedItemPAN.DETAIL_ITEM_ID = typeBSummaryPage10BaseCourse25AssignModel.Model.PAN;
                    detailedItemPAN.TABLE_ID = typeBSummaryPage10BaseCourse25AssignModel.Model.TABLE_ID;
                    detailedItemPAN.ROW_ID = 9;
                    TRobjViewModel.TYPEB_DETAIL_PAGE10_BASE_COURSE3_3_List.Add(detailedItemPAN);

                    foreach (var item in TRobjViewModel.TYPEB_DETAIL_PAGE10_BASE_COURSE3_3_List.ToList())
                    {
                        errorListtemp = objCommonFunction.FetchTetsReportPDFFilledDataToModel(item, loadedForm);
                        TRobjViewModel.ErrorList.AddRange(errorListtemp);
                    }

                    foreach (var item in TRobjViewModel.TYPEB_DETAIL_PAGE10_BASE_COURSE3_3_List.ToList())
                    {
                        if (item.SAMPLE_WEIGHT_10_3 == null && item.RETAINED_WEIGHT_10_3 == null && item.CUMULATIVE_WEIGHT_10_3 == null && item.PASSING_WEIGHT_10_3 == null)
                        {
                            TRobjViewModel.TYPEB_DETAIL_PAGE10_BASE_COURSE3_3_List.Remove(item);
                        }
                    }
                    if (TRobjViewModel.TYPEB_DETAIL_PAGE10_BASE_COURSE3_3_List.Count == 0)
                    {

                        errorListtemp.Clear();
                        errorListtemp.Add("Page-10: Item 10.3.BASE COURSE: 3rd Layer - Gradation Analysis of Aggregates:Table -Please enter Wt. of Sample Retained(g) for atleast one I. S. Sieve Designation for Chainage-III");
                        TRobjViewModel.ErrorOccured = true;
                        TRobjViewModel.ErrorList.AddRange(errorListtemp);
                        errorListtemp.Clear();
                    }
                }

                #endregion

                #region e-Form Test Report Page-12 BASE COURSE: 3rd Layer

                if (TemplateStatus == false)
                {
                    if (naCheckArr[28] == 'Y')
                    {
                        #region Page -12, 13-13.1 BASE COURSE: 3rd Layer -  Cement Stabilised Subbase , Chaninage - 1 

                        errorListtemp = objCommonFunction.FetchTetsReportPDFFilledDataToModel(TRobjViewModel.TYPEB_SUMMARY_PAGE12_CEMENT_SUBBASE_1, loadedForm);
                        TRobjViewModel.ErrorList.AddRange(errorListtemp);
                        TRobjViewModel.TYPEB_SUMMARY_PAGE12_CEMENT_SUBBASE_1.MAIN_ITEM_ID = typeBSummaryPage12BaseCourse3Cement1.Model.MAIN_ITEM_ID;
                        TRobjViewModel.TYPEB_SUMMARY_PAGE12_CEMENT_SUBBASE_1.SUBITEM_ID = typeBSummaryPage12BaseCourse3Cement1.Model.SUBITEM_ID;
                        TRobjViewModel.TYPEB_SUMMARY_PAGE12_CEMENT_SUBBASE_1.TABLE_ID = typeBSummaryPage12BaseCourse3Cement1.Model.TABLE_ID;

                        dbContext.EFORM_TR_DETAIL_ITEM_MASTER.Where(m => m.MAIN_ITEM_ID == typeBSummaryPage12BaseCourse3Cement1.Model.MAIN_ITEM_ID && m.SUBITEM_ID == typeBSummaryPage12BaseCourse3Cement1.Model.SUBITEM_ID).Select(t => t.DETAIL_ITEM_ID).ToList().ForEach(itemID =>
                        {
                            EFORM_TR_TYPEB_DETAIL_PAGE12_CEMENT_SUBBASE_1 detailedItem = new EFORM_TR_TYPEB_DETAIL_PAGE12_CEMENT_SUBBASE_1();
                            detailedItem.MAIN_ITEM_ID = typeBSummaryPage12BaseCourse3Cement1.Model.MAIN_ITEM_ID;
                            detailedItem.SUBITEM_ID = typeBSummaryPage12BaseCourse3Cement1.Model.SUBITEM_ID;
                            detailedItem.TABLE_ID = typeBSummaryPage12BaseCourse3Cement1.Model.TABLE_ID;
                            detailedItem.DETAIL_ITEM_ID = itemID;
                            TRobjViewModel.TYPEB_DETAIL_PAGE12_CEMENT_SUBBASE_1_List.Add(detailedItem);
                        });

                        foreach (var item in TRobjViewModel.TYPEB_DETAIL_PAGE12_CEMENT_SUBBASE_1_List.ToList())
                        {
                            errorListtemp = objCommonFunction.FetchTetsReportPDFFilledDataToModel(item, loadedForm);
                            TRobjViewModel.ErrorList.AddRange(errorListtemp);
                        }


                        foreach (var item in TRobjViewModel.TYPEB_DETAIL_PAGE12_CEMENT_SUBBASE_1_List.ToList())
                        {
                            if (item.SAMPLE_WEIGHT == null && item.RETAINED_WEIGHT == null && item.CUMULATIVE_WEIGHT == null && item.PASSING_WEIGHT == null)
                            {
                                TRobjViewModel.TYPEB_DETAIL_PAGE12_CEMENT_SUBBASE_1_List.Remove(item);
                            }
                        }
                        if (TRobjViewModel.TYPEB_DETAIL_PAGE12_CEMENT_SUBBASE_1_List.Count == 0)
                        {

                            errorListtemp.Clear();
                            errorListtemp.Add("Page-12: Item 13.1.BASE COURSE: 3rd Layer - Cement Stabilised Subbase: Table -Please enter Wt. of Sample Retained(g) for atleast one I. S. Sieve Designation for Chainage-I");
                            TRobjViewModel.ErrorOccured = true;
                            TRobjViewModel.ErrorList.AddRange(errorListtemp);
                            errorListtemp.Clear();
                        }
                        #endregion
                    }
                    if (naCheckArr[29] == 'Y')
                    {
                        #region Page -12, 13-13.2 BASE COURSE: 3rd Layer -  Cement Stabilised Subbase , Chaninage - 2

                        errorListtemp = objCommonFunction.FetchTetsReportPDFFilledDataToModel(TRobjViewModel.TYPEB_SUMMARY_PAGE12_CEMENT_SUBBASE_2, loadedForm);
                        TRobjViewModel.ErrorList.AddRange(errorListtemp);
                        TRobjViewModel.TYPEB_SUMMARY_PAGE12_CEMENT_SUBBASE_2.MAIN_ITEM_ID = typeBSummaryPage12BaseCourse3Cement2.Model.MAIN_ITEM_ID;
                        TRobjViewModel.TYPEB_SUMMARY_PAGE12_CEMENT_SUBBASE_2.SUBITEM_ID = typeBSummaryPage12BaseCourse3Cement2.Model.SUBITEM_ID;
                        TRobjViewModel.TYPEB_SUMMARY_PAGE12_CEMENT_SUBBASE_2.TABLE_ID = typeBSummaryPage12BaseCourse3Cement2.Model.TABLE_ID;

                        dbContext.EFORM_TR_DETAIL_ITEM_MASTER.Where(m => m.MAIN_ITEM_ID == typeBSummaryPage12BaseCourse3Cement2.Model.MAIN_ITEM_ID && m.SUBITEM_ID == typeBSummaryPage12BaseCourse3Cement2.Model.SUBITEM_ID).Select(t => t.DETAIL_ITEM_ID).ToList().ForEach(itemID =>
                        {
                            EFORM_TR_TYPEB_DETAIL_PAGE12_CEMENT_SUBBASE_2 detailedItem = new EFORM_TR_TYPEB_DETAIL_PAGE12_CEMENT_SUBBASE_2();
                            detailedItem.MAIN_ITEM_ID = typeBSummaryPage12BaseCourse3Cement2.Model.MAIN_ITEM_ID;
                            detailedItem.SUBITEM_ID = typeBSummaryPage12BaseCourse3Cement2.Model.SUBITEM_ID;
                            detailedItem.TABLE_ID = typeBSummaryPage12BaseCourse3Cement2.Model.TABLE_ID;
                            detailedItem.DETAIL_ITEM_ID = itemID;
                            TRobjViewModel.TYPEB_DETAIL_PAGE12_CEMENT_SUBBASE_2_List.Add(detailedItem);
                        });

                        foreach (var item in TRobjViewModel.TYPEB_DETAIL_PAGE12_CEMENT_SUBBASE_2_List.ToList())
                        {
                            errorListtemp = objCommonFunction.FetchTetsReportPDFFilledDataToModel(item, loadedForm);
                            TRobjViewModel.ErrorList.AddRange(errorListtemp);
                        }

                        foreach (var item in TRobjViewModel.TYPEB_DETAIL_PAGE12_CEMENT_SUBBASE_2_List.ToList())
                        {
                            if (item.SAMPLE_WEIGHT == null && item.RETAINED_WEIGHT == null && item.CUMULATIVE_WEIGHT == null && item.PASSING_WEIGHT == null)
                            {
                                TRobjViewModel.TYPEB_DETAIL_PAGE12_CEMENT_SUBBASE_2_List.Remove(item);
                            }
                        }
                        if (TRobjViewModel.TYPEB_DETAIL_PAGE12_CEMENT_SUBBASE_2_List.Count == 0)
                        {

                            errorListtemp.Clear();
                            errorListtemp.Add("Page-12: Item 13.2.BASE COURSE: 3rd Layer - Cement Stabilised Subbase: Table -Please enter Wt. of Sample Retained(g) for atleast one I. S. Sieve Designation for Chainage-II");
                            TRobjViewModel.ErrorOccured = true;
                            TRobjViewModel.ErrorList.AddRange(errorListtemp);
                            errorListtemp.Clear();
                        }
                        #endregion
                    }
                    if (naCheckArr[30] == 'Y')
                    {
                        #region Page -12, 13-13.3 BASE COURSE: 3rd Layer -  Cement Stabilised Subbase , Chaninage - 3

                        errorListtemp = objCommonFunction.FetchTetsReportPDFFilledDataToModel(TRobjViewModel.TYPEB_SUMMARY_PAGE12_CEMENT_SUBBASE_3, loadedForm);
                        TRobjViewModel.ErrorList.AddRange(errorListtemp);
                        TRobjViewModel.TYPEB_SUMMARY_PAGE12_CEMENT_SUBBASE_3.MAIN_ITEM_ID = typeBSummaryPage12BaseCourse3Cement3.Model.MAIN_ITEM_ID;
                        TRobjViewModel.TYPEB_SUMMARY_PAGE12_CEMENT_SUBBASE_3.SUBITEM_ID = typeBSummaryPage12BaseCourse3Cement3.Model.SUBITEM_ID;
                        TRobjViewModel.TYPEB_SUMMARY_PAGE12_CEMENT_SUBBASE_3.TABLE_ID = typeBSummaryPage12BaseCourse3Cement3.Model.TABLE_ID;

                        dbContext.EFORM_TR_DETAIL_ITEM_MASTER.Where(m => m.MAIN_ITEM_ID == typeBSummaryPage12BaseCourse3Cement3.Model.MAIN_ITEM_ID && m.SUBITEM_ID == typeBSummaryPage12BaseCourse3Cement3.Model.SUBITEM_ID).Select(t => t.DETAIL_ITEM_ID).ToList().ForEach(itemID =>
                        {
                            EFORM_TR_TYPEB_DETAIL_PAGE12_CEMENT_SUBBASE_3 detailedItem = new EFORM_TR_TYPEB_DETAIL_PAGE12_CEMENT_SUBBASE_3();
                            detailedItem.MAIN_ITEM_ID = typeBSummaryPage12BaseCourse3Cement3.Model.MAIN_ITEM_ID;
                            detailedItem.SUBITEM_ID = typeBSummaryPage12BaseCourse3Cement3.Model.SUBITEM_ID;
                            detailedItem.TABLE_ID = typeBSummaryPage12BaseCourse3Cement3.Model.TABLE_ID;
                            detailedItem.DETAIL_ITEM_ID = itemID;
                            TRobjViewModel.TYPEB_DETAIL_PAGE12_CEMENT_SUBBASE_3_List.Add(detailedItem);
                        });

                        foreach (var item in TRobjViewModel.TYPEB_DETAIL_PAGE12_CEMENT_SUBBASE_3_List.ToList())
                        {
                            errorListtemp = objCommonFunction.FetchTetsReportPDFFilledDataToModel(item, loadedForm);
                            TRobjViewModel.ErrorList.AddRange(errorListtemp);
                        }

                        foreach (var item in TRobjViewModel.TYPEB_DETAIL_PAGE12_CEMENT_SUBBASE_3_List.ToList())
                        {
                            if (item.SAMPLE_WEIGHT == null && item.RETAINED_WEIGHT == null && item.CUMULATIVE_WEIGHT == null && item.PASSING_WEIGHT == null)
                            {
                                TRobjViewModel.TYPEB_DETAIL_PAGE12_CEMENT_SUBBASE_3_List.Remove(item);
                            }
                        }
                        if (TRobjViewModel.TYPEB_DETAIL_PAGE12_CEMENT_SUBBASE_3_List.Count == 0)
                        {

                            errorListtemp.Clear();
                            errorListtemp.Add("Page-12: Item 13.3.BASE COURSE: 3rd Layer - Cement Stabilised Subbase: Table -Please enter Wt. of Sample Retained(g) for atleast one I. S. Sieve Designation for Chainage-III");
                            TRobjViewModel.ErrorOccured = true;
                            TRobjViewModel.ErrorList.AddRange(errorListtemp);
                            errorListtemp.Clear();
                        }

                        #endregion
                    }
                }

                #endregion

                #region e-Form Test Report Page-13 BASE COURSE: 3rd Layer

                if (TemplateStatus == false)
                {
                    if (naCheckArr[31] == 'Y')
                    {
                        #region Page -13, 14-14.1 BASE COURSE: 3rd Layer - Cement Stabilised Subbase-Base (FDR) , Chaninage - 1 

                        errorListtemp = objCommonFunction.FetchTetsReportPDFFilledDataToModel(TRobjViewModel.TYPEB_SUMMARY_PAGE13_CEMENT_FDR_1, loadedForm);
                        TRobjViewModel.ErrorList.AddRange(errorListtemp);
                        TRobjViewModel.TYPEB_SUMMARY_PAGE13_CEMENT_FDR_1.MAIN_ITEM_ID = typeBSummaryPage13BaseCourse3CS_FDR1.Model.MAIN_ITEM_ID;
                        TRobjViewModel.TYPEB_SUMMARY_PAGE13_CEMENT_FDR_1.SUBITEM_ID = typeBSummaryPage13BaseCourse3CS_FDR1.Model.SUBITEM_ID;
                        TRobjViewModel.TYPEB_SUMMARY_PAGE13_CEMENT_FDR_1.TABLE_ID = typeBSummaryPage13BaseCourse3CS_FDR1.Model.TABLE_ID;

                        dbContext.EFORM_TR_DETAIL_ITEM_MASTER.Where(m => m.MAIN_ITEM_ID == 5 && m.SUBITEM_ID == 31).Select(t => t.DETAIL_ITEM_ID).ToList().ForEach(itemID =>
                        {
                            EFORM_TR_TYPEB_DETAIL_PAGE13_CEMENT_FDR_1 detailedItem = new EFORM_TR_TYPEB_DETAIL_PAGE13_CEMENT_FDR_1();
                            detailedItem.MAIN_ITEM_ID = typeBSummaryPage13BaseCourse3CS_FDR1.Model.MAIN_ITEM_ID;
                            detailedItem.SUBITEM_ID = typeBSummaryPage13BaseCourse3CS_FDR1.Model.SUBITEM_ID;
                            detailedItem.TABLE_ID = typeBSummaryPage13BaseCourse3CS_FDR1.Model.TABLE_ID;
                            detailedItem.DETAIL_ITEM_ID = itemID;
                            TRobjViewModel.TYPEB_DETAIL_PAGE13_CEMENT_FDR_1_List.Add(detailedItem);
                        });

                        foreach (var item in TRobjViewModel.TYPEB_DETAIL_PAGE13_CEMENT_FDR_1_List.ToList())
                        {
                            errorListtemp = objCommonFunction.FetchTetsReportPDFFilledDataToModel(item, loadedForm);
                            TRobjViewModel.ErrorList.AddRange(errorListtemp);
                        }

                        foreach (var item in TRobjViewModel.TYPEB_DETAIL_PAGE13_CEMENT_FDR_1_List.ToList())
                        {
                            if (item.SAMPLE_WEIGHT == null && item.RETAINED_WEIGHT == null && item.CUMULATIVE_WEIGHT == null && item.PASSING_WEIGHT == null)
                            {
                                TRobjViewModel.TYPEB_DETAIL_PAGE13_CEMENT_FDR_1_List.Remove(item);
                            }
                        }
                        if (TRobjViewModel.TYPEB_DETAIL_PAGE13_CEMENT_FDR_1_List.Count == 0)
                        {

                            errorListtemp.Clear();
                            errorListtemp.Add("Page-13: Item 14.1.BASE COURSE: 3rd Layer - Cement Stabilised Subbase (FDR): Table -Please enter Wt. of Sample Retained(g) for atleast one I. S. Sieve Designation for Chainage-I");
                            TRobjViewModel.ErrorOccured = true;
                            TRobjViewModel.ErrorList.AddRange(errorListtemp);
                            errorListtemp.Clear();
                        }
                        #endregion
                    }
                    if (naCheckArr[32] == 'Y')
                    {
                        #region Page -13, 14-14.2 BASE COURSE: 3rd Layer - Cement Stabilised Subbase-Base (FDR) , Chaninage - 2

                        errorListtemp = objCommonFunction.FetchTetsReportPDFFilledDataToModel(TRobjViewModel.TYPEB_SUMMARY_PAGE13_CEMENT_FDR_2, loadedForm);
                        TRobjViewModel.ErrorList.AddRange(errorListtemp);
                        TRobjViewModel.TYPEB_SUMMARY_PAGE13_CEMENT_FDR_2.MAIN_ITEM_ID = typeBSummaryPage13BaseCourse3CS_FDR2.Model.MAIN_ITEM_ID;
                        TRobjViewModel.TYPEB_SUMMARY_PAGE13_CEMENT_FDR_2.SUBITEM_ID = typeBSummaryPage13BaseCourse3CS_FDR2.Model.SUBITEM_ID;
                        TRobjViewModel.TYPEB_SUMMARY_PAGE13_CEMENT_FDR_2.TABLE_ID = typeBSummaryPage13BaseCourse3CS_FDR2.Model.TABLE_ID;

                        dbContext.EFORM_TR_DETAIL_ITEM_MASTER.Where(m => m.MAIN_ITEM_ID == typeBSummaryPage13BaseCourse3CS_FDR2.Model.MAIN_ITEM_ID && m.SUBITEM_ID == typeBSummaryPage13BaseCourse3CS_FDR2.Model.SUBITEM_ID).Select(t => t.DETAIL_ITEM_ID).ToList().ForEach(itemID =>
                        {
                            EFORM_TR_TYPEB_DETAIL_PAGE13_CEMENT_FDR_2 detailedItem = new EFORM_TR_TYPEB_DETAIL_PAGE13_CEMENT_FDR_2();
                            detailedItem.MAIN_ITEM_ID = typeBSummaryPage13BaseCourse3CS_FDR2.Model.MAIN_ITEM_ID;
                            detailedItem.SUBITEM_ID = typeBSummaryPage13BaseCourse3CS_FDR2.Model.SUBITEM_ID;
                            detailedItem.TABLE_ID = typeBSummaryPage13BaseCourse3CS_FDR2.Model.TABLE_ID;
                            detailedItem.DETAIL_ITEM_ID = itemID;
                            TRobjViewModel.TYPEB_DETAIL_PAGE13_CEMENT_FDR_2_List.Add(detailedItem);
                        });

                        foreach (var item in TRobjViewModel.TYPEB_DETAIL_PAGE13_CEMENT_FDR_2_List.ToList())
                        {
                            errorListtemp = objCommonFunction.FetchTetsReportPDFFilledDataToModel(item, loadedForm);
                            TRobjViewModel.ErrorList.AddRange(errorListtemp);
                        }

                        foreach (var item in TRobjViewModel.TYPEB_DETAIL_PAGE13_CEMENT_FDR_2_List.ToList())
                        {
                            if (item.SAMPLE_WEIGHT == null && item.RETAINED_WEIGHT == null && item.CUMULATIVE_WEIGHT == null && item.PASSING_WEIGHT == null)
                            {
                                TRobjViewModel.TYPEB_DETAIL_PAGE13_CEMENT_FDR_2_List.Remove(item);
                            }
                        }
                        if (TRobjViewModel.TYPEB_DETAIL_PAGE13_CEMENT_FDR_2_List.Count == 0)
                        {

                            errorListtemp.Clear();
                            errorListtemp.Add("Page-13: Item 14.2.BASE COURSE: 3rd Layer - Cement Stabilised Subbase (FDR): Table -Please enter Wt. of Sample Retained(g) for atleast one I. S. Sieve Designation for Chainage-II");
                            TRobjViewModel.ErrorOccured = true;
                            TRobjViewModel.ErrorList.AddRange(errorListtemp);
                            errorListtemp.Clear();
                        }
                        #endregion
                    }
                    if (naCheckArr[33] == 'Y')
                    {
                        #region Page -13, 14-14.3 BASE COURSE: 3rd Layer -  Cement Stabilised Subbase-Base (FDR) , Chaninage - 3

                        errorListtemp = objCommonFunction.FetchTetsReportPDFFilledDataToModel(TRobjViewModel.TYPEB_SUMMARY_PAGE13_CEMENT_FDR_3, loadedForm);
                        TRobjViewModel.ErrorList.AddRange(errorListtemp);
                        TRobjViewModel.TYPEB_SUMMARY_PAGE13_CEMENT_FDR_3.MAIN_ITEM_ID = typeBSummaryPage13BaseCourse3CS_FDR3.Model.MAIN_ITEM_ID;
                        TRobjViewModel.TYPEB_SUMMARY_PAGE13_CEMENT_FDR_3.SUBITEM_ID = typeBSummaryPage13BaseCourse3CS_FDR3.Model.SUBITEM_ID;
                        TRobjViewModel.TYPEB_SUMMARY_PAGE13_CEMENT_FDR_3.TABLE_ID = typeBSummaryPage13BaseCourse3CS_FDR3.Model.TABLE_ID;

                        dbContext.EFORM_TR_DETAIL_ITEM_MASTER.Where(m => m.MAIN_ITEM_ID == typeBSummaryPage13BaseCourse3CS_FDR3.Model.MAIN_ITEM_ID && m.SUBITEM_ID == typeBSummaryPage13BaseCourse3CS_FDR3.Model.SUBITEM_ID).Select(t => t.DETAIL_ITEM_ID).ToList().ForEach(itemID =>
                        {
                            EFORM_TR_TYPEB_DETAIL_PAGE13_CEMENT_FDR_3 detailedItem = new EFORM_TR_TYPEB_DETAIL_PAGE13_CEMENT_FDR_3();
                            detailedItem.MAIN_ITEM_ID = typeBSummaryPage13BaseCourse3CS_FDR3.Model.MAIN_ITEM_ID;
                            detailedItem.SUBITEM_ID = typeBSummaryPage13BaseCourse3CS_FDR3.Model.SUBITEM_ID;
                            detailedItem.TABLE_ID = typeBSummaryPage13BaseCourse3CS_FDR3.Model.TABLE_ID;
                            detailedItem.DETAIL_ITEM_ID = itemID;
                            TRobjViewModel.TYPEB_DETAIL_PAGE13_CEMENT_FDR_3_List.Add(detailedItem);
                        });

                        foreach (var item in TRobjViewModel.TYPEB_DETAIL_PAGE13_CEMENT_FDR_3_List.ToList())
                        {
                            errorListtemp = objCommonFunction.FetchTetsReportPDFFilledDataToModel(item, loadedForm);
                            TRobjViewModel.ErrorList.AddRange(errorListtemp);
                        }

                        foreach (var item in TRobjViewModel.TYPEB_DETAIL_PAGE13_CEMENT_FDR_3_List.ToList())
                        {
                            if (item.SAMPLE_WEIGHT == null && item.RETAINED_WEIGHT == null && item.CUMULATIVE_WEIGHT == null && item.PASSING_WEIGHT == null)
                            {
                                TRobjViewModel.TYPEB_DETAIL_PAGE13_CEMENT_FDR_3_List.Remove(item);
                            }
                        }
                        if (TRobjViewModel.TYPEB_DETAIL_PAGE13_CEMENT_FDR_3_List.Count == 0)
                        {

                            errorListtemp.Clear();
                            errorListtemp.Add("Page-13: Item 14.3.BASE COURSE: 3rd Layer - Cement Stabilised Subbase (FDR): Table -Please enter Wt. of Sample Retained(g) for atleast one I. S. Sieve Designation for Chainage-III");
                            TRobjViewModel.ErrorOccured = true;
                            TRobjViewModel.ErrorList.AddRange(errorListtemp);
                            errorListtemp.Clear();
                        }
                        #endregion
                    }
                }

                #endregion

                #region e-Form Test Report Page-14 BITUMINOUS BASE COURSE

                int BitumnId = 0;

                PdfLoadedRadioButtonListField buttonListField = loadedForm.Fields["Bituminous_Base_Course"] as PdfLoadedRadioButtonListField;

                if (buttonListField.SelectedValue == null)
                {
                    errorListtemp.Clear();
                    errorListtemp.Add("Page-14: BITUMINOUS BASE COURSE: Please Select Bituminous Base Course Type");
                    TRobjViewModel.ErrorOccured = true;
                    TRobjViewModel.ErrorList.AddRange(errorListtemp);
                    errorListtemp.Clear();
                }
                else
                {
                    BitumnId = Convert.ToInt32(buttonListField.SelectedValue);
                    TestReportModel.EFORM_TR_MAIN_ITEM_OPTIONS_SELECTED_DETAIL modelSub = new TestReportModel.EFORM_TR_MAIN_ITEM_OPTIONS_SELECTED_DETAIL();
                    modelSub.MAIN_ITEM_ID = typeBSummaryPage14BitumnBaseCourse1.Model.MAIN_ITEM_ID;
                    modelSub.OPTION_ID = BitumnId;
                    TRobjViewModel.MAIN_ITEM_OPTIONS_SELECTED_DETAIL_List.Add(modelSub);

                    if (naCheckArr[34] == 'Y')
                    {
                        #region Page -14, 15-15.1 BITUMINOUS BASE COURSE - , Chaninage - 1 

                        errorListtemp = objCommonFunction.FetchTetsReportPDFFilledDataToModel(TRobjViewModel.TYPEB_SUMMARY_PAGE14_BITUMN_COURSE_1, loadedForm);
                        TRobjViewModel.ErrorList.AddRange(errorListtemp);
                        TRobjViewModel.TYPEB_SUMMARY_PAGE14_BITUMN_COURSE_1.MAIN_ITEM_ID = typeBSummaryPage14BitumnBaseCourse1.Model.MAIN_ITEM_ID;
                        TRobjViewModel.TYPEB_SUMMARY_PAGE14_BITUMN_COURSE_1.SUBITEM_ID = typeBSummaryPage14BitumnBaseCourse1.Model.SUBITEM_ID;
                        TRobjViewModel.TYPEB_SUMMARY_PAGE14_BITUMN_COURSE_1.TABLE_ID = typeBSummaryPage14BitumnBaseCourse1.Model.TABLE_ID;

                        dbContext.EFORM_TR_DETAIL_ITEM_MASTER.Where(m => m.MAIN_ITEM_ID == typeBSummaryPage14BitumnBaseCourse1.Model.MAIN_ITEM_ID && m.SUBITEM_ID == typeBSummaryPage14BitumnBaseCourse1.Model.SUBITEM_ID).Select(t => t.DETAIL_ITEM_ID).ToList().ForEach(itemID =>
                        {
                            EFORM_TR_TYPEB_DETAIL_PAGE14_BITUMN_COURSE_1 detailedItem = new EFORM_TR_TYPEB_DETAIL_PAGE14_BITUMN_COURSE_1();
                            detailedItem.MAIN_ITEM_ID = typeBSummaryPage14BitumnBaseCourse1.Model.MAIN_ITEM_ID;
                            detailedItem.SUBITEM_ID = typeBSummaryPage14BitumnBaseCourse1.Model.SUBITEM_ID;
                            detailedItem.TABLE_ID = typeBSummaryPage14BitumnBaseCourse1.Model.TABLE_ID;
                            detailedItem.DETAIL_ITEM_ID = itemID;
                            TRobjViewModel.TYPEB_DETAIL_PAGE14_BITUMN_COURSE_1_List.Add(detailedItem);
                        });

                        foreach (var item in TRobjViewModel.TYPEB_DETAIL_PAGE14_BITUMN_COURSE_1_List.ToList())
                        {
                            errorListtemp = objCommonFunction.FetchTetsReportPDFFilledDataToModel(item, loadedForm);
                            TRobjViewModel.ErrorList.AddRange(errorListtemp);
                        }

                        foreach (var item in TRobjViewModel.TYPEB_DETAIL_PAGE14_BITUMN_COURSE_1_List.ToList())
                        {
                            if (item.SAMPLE_WEIGHT == null && item.RETAINED_WEIGHT == null && item.CUMULATIVE_WEIGHT == null && item.PASSING_WEIGHT == null)
                            {
                                TRobjViewModel.TYPEB_DETAIL_PAGE14_BITUMN_COURSE_1_List.Remove(item);
                            }
                        }
                        if (TRobjViewModel.TYPEB_DETAIL_PAGE14_BITUMN_COURSE_1_List.Count == 0)
                        {

                            errorListtemp.Clear();
                            errorListtemp.Add("Page-14: Item 15.1.BITUMINOUS BASE COURSE : Gradation Analysis of Aggregates: Table -Please enter Wt. of Sample Retained(g) for atleast one I. S. Sieve Designation for Chainage-I");
                            TRobjViewModel.ErrorOccured = true;
                            TRobjViewModel.ErrorList.AddRange(errorListtemp);
                            errorListtemp.Clear();
                        }
                        #endregion
                    }
                    if (naCheckArr[35] == 'Y')
                    {
                        #region Page -14, 15-15.2 BITUMINOUS BASE COURSE - , Chaninage - 2

                        errorListtemp = objCommonFunction.FetchTetsReportPDFFilledDataToModel(TRobjViewModel.TYPEB_SUMMARY_PAGE14_BITUMN_COURSE_2, loadedForm);
                        TRobjViewModel.ErrorList.AddRange(errorListtemp);
                        TRobjViewModel.TYPEB_SUMMARY_PAGE14_BITUMN_COURSE_2.MAIN_ITEM_ID = typeBSummaryPage14BitumnBaseCourse2.Model.MAIN_ITEM_ID;
                        TRobjViewModel.TYPEB_SUMMARY_PAGE14_BITUMN_COURSE_2.SUBITEM_ID = typeBSummaryPage14BitumnBaseCourse2.Model.SUBITEM_ID;
                        TRobjViewModel.TYPEB_SUMMARY_PAGE14_BITUMN_COURSE_2.TABLE_ID = typeBSummaryPage14BitumnBaseCourse2.Model.TABLE_ID;

                        dbContext.EFORM_TR_DETAIL_ITEM_MASTER.Where(m => m.MAIN_ITEM_ID == typeBSummaryPage14BitumnBaseCourse2.Model.MAIN_ITEM_ID && m.SUBITEM_ID == typeBSummaryPage14BitumnBaseCourse2.Model.SUBITEM_ID).Select(t => t.DETAIL_ITEM_ID).ToList().ForEach(itemID =>
                        {
                            EFORM_TR_TYPEB_DETAIL_PAGE14_BITUMN_COURSE_2 detailedItem = new EFORM_TR_TYPEB_DETAIL_PAGE14_BITUMN_COURSE_2();
                            detailedItem.MAIN_ITEM_ID = typeBSummaryPage14BitumnBaseCourse2.Model.MAIN_ITEM_ID;
                            detailedItem.SUBITEM_ID = typeBSummaryPage14BitumnBaseCourse2.Model.SUBITEM_ID;
                            detailedItem.TABLE_ID = typeBSummaryPage14BitumnBaseCourse2.Model.TABLE_ID;
                            detailedItem.DETAIL_ITEM_ID = itemID;
                            TRobjViewModel.TYPEB_DETAIL_PAGE14_BITUMN_COURSE_2_List.Add(detailedItem);
                        });

                        foreach (var item in TRobjViewModel.TYPEB_DETAIL_PAGE14_BITUMN_COURSE_2_List.ToList())
                        {
                            errorListtemp = objCommonFunction.FetchTetsReportPDFFilledDataToModel(item, loadedForm);
                            TRobjViewModel.ErrorList.AddRange(errorListtemp);
                        }

                        foreach (var item in TRobjViewModel.TYPEB_DETAIL_PAGE14_BITUMN_COURSE_2_List.ToList())
                        {
                            if (item.SAMPLE_WEIGHT == null && item.RETAINED_WEIGHT == null && item.CUMULATIVE_WEIGHT == null && item.PASSING_WEIGHT == null)
                            {
                                TRobjViewModel.TYPEB_DETAIL_PAGE14_BITUMN_COURSE_2_List.Remove(item);
                            }
                        }
                        if (TRobjViewModel.TYPEB_DETAIL_PAGE14_BITUMN_COURSE_2_List.Count == 0)
                        {

                            errorListtemp.Clear();
                            errorListtemp.Add("Page-14: Item 15.2.BITUMINOUS BASE COURSE : Gradation Analysis of Aggregates: Table -Please enter Wt. of Sample Retained(g) for atleast one I. S. Sieve Designation for Chainage-II");
                            TRobjViewModel.ErrorOccured = true;
                            TRobjViewModel.ErrorList.AddRange(errorListtemp);
                            errorListtemp.Clear();
                        }
                        #endregion
                    }
                    if (naCheckArr[36] == 'Y')
                    {
                        #region Page -14, 15-15.3 BITUMINOUS BASE COURSE - , Chaninage - 3

                        errorListtemp = objCommonFunction.FetchTetsReportPDFFilledDataToModel(TRobjViewModel.TYPEB_SUMMARY_PAGE15_BITUMN_COURSE_3, loadedForm);
                        TRobjViewModel.ErrorList.AddRange(errorListtemp);
                        TRobjViewModel.TYPEB_SUMMARY_PAGE15_BITUMN_COURSE_3.MAIN_ITEM_ID = typeBSummaryPage14BitumnBaseCourse3.Model.MAIN_ITEM_ID;
                        TRobjViewModel.TYPEB_SUMMARY_PAGE15_BITUMN_COURSE_3.SUBITEM_ID = typeBSummaryPage14BitumnBaseCourse3.Model.SUBITEM_ID;
                        TRobjViewModel.TYPEB_SUMMARY_PAGE15_BITUMN_COURSE_3.TABLE_ID = typeBSummaryPage14BitumnBaseCourse3.Model.TABLE_ID;

                        dbContext.EFORM_TR_DETAIL_ITEM_MASTER.Where(m => m.MAIN_ITEM_ID == typeBSummaryPage14BitumnBaseCourse3.Model.MAIN_ITEM_ID && m.SUBITEM_ID == typeBSummaryPage14BitumnBaseCourse3.Model.SUBITEM_ID).Select(t => t.DETAIL_ITEM_ID).ToList().ForEach(itemID =>
                        {
                            EFORM_TR_TYPEB_DETAIL_PAGE15_BITUMN_COURSE_3 detailedItem = new EFORM_TR_TYPEB_DETAIL_PAGE15_BITUMN_COURSE_3();
                            detailedItem.MAIN_ITEM_ID = typeBSummaryPage14BitumnBaseCourse3.Model.MAIN_ITEM_ID;
                            detailedItem.SUBITEM_ID = typeBSummaryPage14BitumnBaseCourse3.Model.SUBITEM_ID;
                            detailedItem.TABLE_ID = typeBSummaryPage14BitumnBaseCourse3.Model.TABLE_ID;
                            detailedItem.DETAIL_ITEM_ID = itemID;
                            TRobjViewModel.TYPEB_DETAIL_PAGE15_BITUMN_COURSE_3_List.Add(detailedItem);
                        });

                        foreach (var item in TRobjViewModel.TYPEB_DETAIL_PAGE15_BITUMN_COURSE_3_List.ToList())
                        {
                            errorListtemp = objCommonFunction.FetchTetsReportPDFFilledDataToModel(item, loadedForm);
                            TRobjViewModel.ErrorList.AddRange(errorListtemp);
                        }

                        foreach (var item in TRobjViewModel.TYPEB_DETAIL_PAGE15_BITUMN_COURSE_3_List.ToList())
                        {
                            if (item.SAMPLE_WEIGHT == null && item.RETAINED_WEIGHT == null && item.CUMULATIVE_WEIGHT == null && item.PASSING_WEIGHT == null)
                            {
                                TRobjViewModel.TYPEB_DETAIL_PAGE15_BITUMN_COURSE_3_List.Remove(item);
                            }
                        }
                        if (TRobjViewModel.TYPEB_DETAIL_PAGE15_BITUMN_COURSE_3_List.Count == 0)
                        {

                            errorListtemp.Clear();
                            errorListtemp.Add("Page-15: Item 15.3.BITUMINOUS BASE COURSE : Gradation Analysis of Aggregates: Table -Please enter Wt. of Sample Retained(g) for atleast one I. S. Sieve Designation for Chainage-III");
                            TRobjViewModel.ErrorOccured = true;
                            TRobjViewModel.ErrorList.AddRange(errorListtemp);
                            errorListtemp.Clear();
                        }
                        #endregion
                    }
                }


                #endregion

                #region e-Form Test Report Page-17-18 BITUMINOUS SURFACE COURSE
                int BitumnSurfaceID = 0;

                PdfLoadedRadioButtonListField SurfaceListField = loadedForm.Fields["Bituminous_Surface_Course"] as PdfLoadedRadioButtonListField;

                if (SurfaceListField.SelectedValue == null)
                {
                    errorListtemp.Clear();
                    errorListtemp.Add("Page-17: BITUMINOUS SURFACE COURSE: Please Select Bituminous Surface Course Type");
                    TRobjViewModel.ErrorOccured = true;
                    TRobjViewModel.ErrorList.AddRange(errorListtemp);
                    errorListtemp.Clear();
                }
                else
                {
                    BitumnSurfaceID = Convert.ToInt32(SurfaceListField.SelectedValue);
                    TestReportModel.EFORM_TR_MAIN_ITEM_OPTIONS_SELECTED_DETAIL modelSub = new TestReportModel.EFORM_TR_MAIN_ITEM_OPTIONS_SELECTED_DETAIL();
                    modelSub.MAIN_ITEM_ID = typeBSummaryPage17BitumnSurfaceCourse1.Model.MAIN_ITEM_ID;
                    modelSub.OPTION_ID = BitumnSurfaceID;
                    TRobjViewModel.MAIN_ITEM_OPTIONS_SELECTED_DETAIL_List.Add(modelSub);
                    string concText = string.Empty;
                    if (BitumnSurfaceID == 31)
                    {
                        concText = "SDBC";
                    }
                    if (BitumnSurfaceID == 32)
                    {
                        concText = "from MSS(Type-A), MSS(Type-B)";
                    }
                    if (BitumnSurfaceID == 33)
                    {
                        concText = "from Surface Course(19),Surface Course(13),Surface Course(10),Surface Course(6)";
                    }
                    if (BitumnSurfaceID == 34)
                    {
                        concText = "from BC(19),BC(13.2)";
                    }
                    if (naCheckArr[40] == 'Y')
                    {
                        //Summary 
                        errorListtemp = objCommonFunction.FetchTetsReportPDFFilledDataToModel(TRobjViewModel.TYPEC_SUMMARY_PAGE17_BITUMN_SURFACE_COURSE_1, loadedForm);
                        TRobjViewModel.ErrorList.AddRange(errorListtemp);
                        TRobjViewModel.TYPEC_SUMMARY_PAGE17_BITUMN_SURFACE_COURSE_1.MAIN_ITEM_ID = typeBSummaryPage17BitumnSurfaceCourse1.Model.MAIN_ITEM_ID;
                        TRobjViewModel.TYPEC_SUMMARY_PAGE17_BITUMN_SURFACE_COURSE_1.SUBITEM_ID = typeBSummaryPage17BitumnSurfaceCourse1.Model.SUBITEM_ID;
                        TRobjViewModel.TYPEC_SUMMARY_PAGE17_BITUMN_SURFACE_COURSE_1.TABLE_ID = typeBSummaryPage17BitumnSurfaceCourse1.Model.TABLE_ID;

                        int maxRowLength = 0;



                        if (TRobjViewModel.TYPEC_SUMMARY_PAGE17_BITUMN_SURFACE_COURSE_1.GRADATION_TYPE_ITEM_ID_17_1 > 1)
                        {
                            bool isGradationTypeValid = true;
                            if (!dbContext.EFORM_TR_GRADATION_TYPE_ITEM_MASTER.Any(s => s.ITEM_ID ==
                            TRobjViewModel.TYPEC_SUMMARY_PAGE17_BITUMN_SURFACE_COURSE_1.GRADATION_TYPE_ITEM_ID_17_1 && s.SUBITEM_FLAG == BitumnSurfaceID))
                            {
                                errorListtemp.Clear();
                                errorListtemp.Add("Page-17: BITUMINOUS SURFACE COURSE:18.1 Gradation Analysis of Aggregates: Please select " + (BitumnSurfaceID == 31 ? "" : "one of the") + " Gradation type " + concText + " only for chainage-I");
                                TRobjViewModel.ErrorOccured = true;
                                TRobjViewModel.ErrorList.AddRange(errorListtemp);
                                errorListtemp.Clear();
                                isGradationTypeValid = false;
                            }
                            if (isGradationTypeValid == true)
                            {
                                if (TRobjViewModel.TYPEC_SUMMARY_PAGE17_BITUMN_SURFACE_COURSE_1.GRADATION_TYPE_ITEM_ID_17_1 <= 5
                                && TRobjViewModel.TYPEC_SUMMARY_PAGE17_BITUMN_SURFACE_COURSE_1.GRADATION_TYPE_ITEM_ID_17_1 >= 2)
                                {
                                    maxRowLength = 11;
                                }
                                if (TRobjViewModel.TYPEC_SUMMARY_PAGE17_BITUMN_SURFACE_COURSE_1.GRADATION_TYPE_ITEM_ID_17_1 <= 7
                                   && TRobjViewModel.TYPEC_SUMMARY_PAGE17_BITUMN_SURFACE_COURSE_1.GRADATION_TYPE_ITEM_ID_17_1 >= 6)
                                {
                                    maxRowLength = 5;
                                }
                                if (TRobjViewModel.TYPEC_SUMMARY_PAGE17_BITUMN_SURFACE_COURSE_1.GRADATION_TYPE_ITEM_ID_17_1 == 8)
                                {
                                    maxRowLength = 9;
                                }
                                if (TRobjViewModel.TYPEC_SUMMARY_PAGE17_BITUMN_SURFACE_COURSE_1.GRADATION_TYPE_ITEM_ID_17_1 <= 10
                                   && TRobjViewModel.TYPEC_SUMMARY_PAGE17_BITUMN_SURFACE_COURSE_1.GRADATION_TYPE_ITEM_ID_17_1 >= 9)
                                {
                                    maxRowLength = 15;
                                }

                                for (int i = 1; i <= maxRowLength; i++)
                                {
                                    EFORM_TR_TYPEC_DETAIL_PAGE17_BITUMN_SURFACE_COURSE_1 detailedItem = new EFORM_TR_TYPEC_DETAIL_PAGE17_BITUMN_SURFACE_COURSE_1();
                                    detailedItem.MAIN_ITEM_ID = typeBSummaryPage17BitumnSurfaceCourse1.Model.MAIN_ITEM_ID;
                                    detailedItem.SUBITEM_ID = typeBSummaryPage17BitumnSurfaceCourse1.Model.SUBITEM_ID;
                                    detailedItem.TABLE_ID = typeBSummaryPage17BitumnSurfaceCourse1.Model.TABLE_ID;
                                    detailedItem.ROW_ID = i;
                                    TRobjViewModel.TYPEC_DETAIL_PAGE17_BITUMN_SURFACE_COURSE_1_List.Add(detailedItem);

                                }
                                EFORM_TR_TYPEC_DETAIL_PAGE17_BITUMN_SURFACE_COURSE_1 detailedItemPAN = new EFORM_TR_TYPEC_DETAIL_PAGE17_BITUMN_SURFACE_COURSE_1();
                                detailedItemPAN.MAIN_ITEM_ID = typeBSummaryPage17BitumnSurfaceCourse1.Model.MAIN_ITEM_ID;
                                detailedItemPAN.SUBITEM_ID = typeBSummaryPage17BitumnSurfaceCourse1.Model.SUBITEM_ID;
                                detailedItemPAN.TABLE_ID = typeBSummaryPage17BitumnSurfaceCourse1.Model.TABLE_ID;
                                detailedItemPAN.ROW_ID = 16;

                                TRobjViewModel.TYPEC_DETAIL_PAGE17_BITUMN_SURFACE_COURSE_1_List.Add(detailedItemPAN);
                                foreach (var item in TRobjViewModel.TYPEC_DETAIL_PAGE17_BITUMN_SURFACE_COURSE_1_List.ToList())
                                {
                                    errorListtemp = objCommonFunction.FetchTetsReportPDFFilledDataToModel(item, loadedForm);
                                    TRobjViewModel.ErrorList.AddRange(errorListtemp);
                                }
                                foreach (var item in TRobjViewModel.TYPEC_DETAIL_PAGE17_BITUMN_SURFACE_COURSE_1_List.ToList())
                                {
                                    if (item.SAMPLE_WEIGHT_17_1 == null && item.RETAINED_WEIGHT_17_1 == null && item.CUMULATIVE_WEIGHT_17_1 == null && item.PASSING_WEIGHT_17_1 == null && item.SIEVE_DESIGNATION_17_1 == null && item.PERMISSIBLE_RANGE_17_1 == null)
                                    {
                                        TRobjViewModel.TYPEC_DETAIL_PAGE17_BITUMN_SURFACE_COURSE_1_List.Remove(item);
                                    }
                                }
                                bool sampWtEntry = false;
                                foreach (var item in TRobjViewModel.TYPEC_DETAIL_PAGE17_BITUMN_SURFACE_COURSE_1_List.ToList())
                                {
                                    if (item.SAMPLE_WEIGHT_17_1 != null)
                                    {
                                        sampWtEntry = true;
                                    }
                                }


                                if (sampWtEntry == false)
                                {

                                    errorListtemp.Clear();
                                    errorListtemp.Add("Page-17: Item 18.1.BITUMINOUS SURFACE COURSE : Gradation Analysis of Aggregates: Table -Please enter Wt. of Sample Retained(g) for atleast one I. S. Sieve Designation for Chainage-I");
                                    TRobjViewModel.ErrorOccured = true;
                                    TRobjViewModel.ErrorList.AddRange(errorListtemp);
                                    errorListtemp.Clear();
                                }
                            }



                        }


                    }

                    if (naCheckArr[41] == 'Y')
                    {
                        //Summary 
                        errorListtemp = objCommonFunction.FetchTetsReportPDFFilledDataToModel(TRobjViewModel.TYPEC_SUMMARY_PAGE17_BITUMN_SURFACE_COURSE_2, loadedForm);
                        TRobjViewModel.ErrorList.AddRange(errorListtemp);
                        TRobjViewModel.TYPEC_SUMMARY_PAGE17_BITUMN_SURFACE_COURSE_2.MAIN_ITEM_ID = typeBSummaryPage17BitumnSurfaceCourse2.Model.MAIN_ITEM_ID;
                        TRobjViewModel.TYPEC_SUMMARY_PAGE17_BITUMN_SURFACE_COURSE_2.SUBITEM_ID = typeBSummaryPage17BitumnSurfaceCourse2.Model.SUBITEM_ID;
                        TRobjViewModel.TYPEC_SUMMARY_PAGE17_BITUMN_SURFACE_COURSE_2.TABLE_ID = typeBSummaryPage17BitumnSurfaceCourse2.Model.TABLE_ID;

                        int maxRowLength = 0;

                        if (TRobjViewModel.TYPEC_SUMMARY_PAGE17_BITUMN_SURFACE_COURSE_2.GRADATION_TYPE_ITEM_ID_17_2 > 1)
                        {
                            bool isGradationTypeValid = true;
                            if (!dbContext.EFORM_TR_GRADATION_TYPE_ITEM_MASTER.Any(s => s.ITEM_ID ==
                            TRobjViewModel.TYPEC_SUMMARY_PAGE17_BITUMN_SURFACE_COURSE_2.GRADATION_TYPE_ITEM_ID_17_2 && s.SUBITEM_FLAG == BitumnSurfaceID))
                            {
                                errorListtemp.Clear();
                                errorListtemp.Add("Page-17: BITUMINOUS SURFACE COURSE:18.2 Gradation Analysis of Aggregates: Please select " + (BitumnSurfaceID == 31 ? "" : "one of the") + " Gradation type " + concText + " only for chainage-II");
                                TRobjViewModel.ErrorOccured = true;
                                TRobjViewModel.ErrorList.AddRange(errorListtemp);
                                errorListtemp.Clear();
                                isGradationTypeValid = false;
                            }
                            if (isGradationTypeValid == true)
                            {
                                if (TRobjViewModel.TYPEC_SUMMARY_PAGE17_BITUMN_SURFACE_COURSE_2.GRADATION_TYPE_ITEM_ID_17_2 <= 5
                              && TRobjViewModel.TYPEC_SUMMARY_PAGE17_BITUMN_SURFACE_COURSE_2.GRADATION_TYPE_ITEM_ID_17_2 >= 2)
                                {
                                    maxRowLength = 11;
                                }
                                if (TRobjViewModel.TYPEC_SUMMARY_PAGE17_BITUMN_SURFACE_COURSE_2.GRADATION_TYPE_ITEM_ID_17_2 <= 7
                                   && TRobjViewModel.TYPEC_SUMMARY_PAGE17_BITUMN_SURFACE_COURSE_2.GRADATION_TYPE_ITEM_ID_17_2 >= 6)
                                {
                                    maxRowLength = 5;
                                }
                                if (TRobjViewModel.TYPEC_SUMMARY_PAGE17_BITUMN_SURFACE_COURSE_2.GRADATION_TYPE_ITEM_ID_17_2 == 8)
                                {
                                    maxRowLength = 9;
                                }
                                if (TRobjViewModel.TYPEC_SUMMARY_PAGE17_BITUMN_SURFACE_COURSE_2.GRADATION_TYPE_ITEM_ID_17_2 <= 10
                                   && TRobjViewModel.TYPEC_SUMMARY_PAGE17_BITUMN_SURFACE_COURSE_2.GRADATION_TYPE_ITEM_ID_17_2 >= 9)
                                {
                                    maxRowLength = 15;
                                }
                                for (int i = 1; i <= maxRowLength; i++)
                                {
                                    EFORM_TR_TYPEC_DETAIL_PAGE17_BITUMN_SURFACE_COURSE_2 detailedItem = new EFORM_TR_TYPEC_DETAIL_PAGE17_BITUMN_SURFACE_COURSE_2();
                                    detailedItem.MAIN_ITEM_ID = typeBSummaryPage17BitumnSurfaceCourse2.Model.MAIN_ITEM_ID;
                                    detailedItem.SUBITEM_ID = typeBSummaryPage17BitumnSurfaceCourse2.Model.SUBITEM_ID;
                                    detailedItem.TABLE_ID = typeBSummaryPage17BitumnSurfaceCourse2.Model.TABLE_ID;
                                    detailedItem.ROW_ID = i;
                                    TRobjViewModel.TYPEC_DETAIL_PAGE17_BITUMN_SURFACE_COURSE_2_List.Add(detailedItem);
                                }
                                EFORM_TR_TYPEC_DETAIL_PAGE17_BITUMN_SURFACE_COURSE_2 detailedItemPAN = new EFORM_TR_TYPEC_DETAIL_PAGE17_BITUMN_SURFACE_COURSE_2();
                                detailedItemPAN.MAIN_ITEM_ID = typeBSummaryPage17BitumnSurfaceCourse2.Model.MAIN_ITEM_ID;
                                detailedItemPAN.SUBITEM_ID = typeBSummaryPage17BitumnSurfaceCourse2.Model.SUBITEM_ID;
                                detailedItemPAN.TABLE_ID = typeBSummaryPage17BitumnSurfaceCourse2.Model.TABLE_ID;
                                detailedItemPAN.ROW_ID = 16;
                                TRobjViewModel.TYPEC_DETAIL_PAGE17_BITUMN_SURFACE_COURSE_2_List.Add(detailedItemPAN);

                                foreach (var item in TRobjViewModel.TYPEC_DETAIL_PAGE17_BITUMN_SURFACE_COURSE_2_List.ToList())
                                {
                                    errorListtemp = objCommonFunction.FetchTetsReportPDFFilledDataToModel(item, loadedForm);
                                    TRobjViewModel.ErrorList.AddRange(errorListtemp);
                                }
                                foreach (var item in TRobjViewModel.TYPEC_DETAIL_PAGE17_BITUMN_SURFACE_COURSE_2_List.ToList())
                                {
                                    if (item.SAMPLE_WEIGHT_17_2 == null && item.RETAINED_WEIGHT_17_2 == null && item.CUMULATIVE_WEIGHT_17_2 == null && item.PASSING_WEIGHT_17_2 == null && item.SIEVE_DESIGNATION_17_2 == null && item.PERMISSIBLE_RANGE_17_2 == null)
                                    {
                                        TRobjViewModel.TYPEC_DETAIL_PAGE17_BITUMN_SURFACE_COURSE_2_List.Remove(item);
                                    }
                                }

                                bool sampWtEntry = false;
                                foreach (var item in TRobjViewModel.TYPEC_DETAIL_PAGE17_BITUMN_SURFACE_COURSE_2_List.ToList())
                                {
                                    if (item.SAMPLE_WEIGHT_17_2 != null)
                                    {
                                        sampWtEntry = true;
                                    }
                                }


                                if (sampWtEntry == false)
                                {

                                    errorListtemp.Clear();
                                    errorListtemp.Add("Page-17: Item 18.2.BITUMINOUS SURFACE COURSE : Gradation Analysis of Aggregates: Table -Please enter Wt. of Sample Retained(g) for atleast one I. S. Sieve Designation for Chainage-II");
                                    TRobjViewModel.ErrorOccured = true;
                                    TRobjViewModel.ErrorList.AddRange(errorListtemp);
                                    errorListtemp.Clear();
                                }
                            }



                        }
                    }

                    if (naCheckArr[42] == 'Y')
                    {
                        //Summary 
                        errorListtemp = objCommonFunction.FetchTetsReportPDFFilledDataToModel(TRobjViewModel.TYPEC_SUMMARY_PAGE18_BITUMN_SURFACE_COURSE_3, loadedForm);
                        TRobjViewModel.ErrorList.AddRange(errorListtemp);
                        TRobjViewModel.TYPEC_SUMMARY_PAGE18_BITUMN_SURFACE_COURSE_3.MAIN_ITEM_ID = typeBSummaryPage17BitumnSurfaceCourse3.Model.MAIN_ITEM_ID;
                        TRobjViewModel.TYPEC_SUMMARY_PAGE18_BITUMN_SURFACE_COURSE_3.SUBITEM_ID = typeBSummaryPage17BitumnSurfaceCourse3.Model.SUBITEM_ID;
                        TRobjViewModel.TYPEC_SUMMARY_PAGE18_BITUMN_SURFACE_COURSE_3.TABLE_ID = typeBSummaryPage17BitumnSurfaceCourse3.Model.TABLE_ID;
                        int maxRowLength = 0;

                        if (TRobjViewModel.TYPEC_SUMMARY_PAGE18_BITUMN_SURFACE_COURSE_3.GRADATION_TYPE_ITEM_ID_18_1 > 1)
                        {
                            bool isGradationTypeValid = true;
                            if (!dbContext.EFORM_TR_GRADATION_TYPE_ITEM_MASTER.Any(s => s.ITEM_ID ==
                           TRobjViewModel.TYPEC_SUMMARY_PAGE18_BITUMN_SURFACE_COURSE_3.GRADATION_TYPE_ITEM_ID_18_1 && s.SUBITEM_FLAG == BitumnSurfaceID))
                            {
                                errorListtemp.Clear();
                                errorListtemp.Add("Page-18: BITUMINOUS SURFACE COURSE:18.3 Gradation Analysis of Aggregates: Please select " + (BitumnSurfaceID == 31 ? "" : "one of the") + " Gradation type " + concText + " only for chainage-III");
                                TRobjViewModel.ErrorOccured = true;
                                TRobjViewModel.ErrorList.AddRange(errorListtemp);
                                errorListtemp.Clear();
                                isGradationTypeValid = false;
                            }

                            if (isGradationTypeValid == true)
                            {
                                if (TRobjViewModel.TYPEC_SUMMARY_PAGE18_BITUMN_SURFACE_COURSE_3.GRADATION_TYPE_ITEM_ID_18_1 <= 5
                              && TRobjViewModel.TYPEC_SUMMARY_PAGE18_BITUMN_SURFACE_COURSE_3.GRADATION_TYPE_ITEM_ID_18_1 >= 2)
                                {
                                    maxRowLength = 11;
                                }
                                if (TRobjViewModel.TYPEC_SUMMARY_PAGE18_BITUMN_SURFACE_COURSE_3.GRADATION_TYPE_ITEM_ID_18_1 <= 7
                                   && TRobjViewModel.TYPEC_SUMMARY_PAGE18_BITUMN_SURFACE_COURSE_3.GRADATION_TYPE_ITEM_ID_18_1 >= 6)
                                {
                                    maxRowLength = 5;
                                }
                                if (TRobjViewModel.TYPEC_SUMMARY_PAGE18_BITUMN_SURFACE_COURSE_3.GRADATION_TYPE_ITEM_ID_18_1 == 8)
                                {
                                    maxRowLength = 9;
                                }
                                if (TRobjViewModel.TYPEC_SUMMARY_PAGE18_BITUMN_SURFACE_COURSE_3.GRADATION_TYPE_ITEM_ID_18_1 <= 10
                                   && TRobjViewModel.TYPEC_SUMMARY_PAGE18_BITUMN_SURFACE_COURSE_3.GRADATION_TYPE_ITEM_ID_18_1 >= 9)
                                {
                                    maxRowLength = 15;
                                }


                                for (int i = 1; i <= maxRowLength; i++)
                                {
                                    EFORM_TR_TYPEC_DETAIL_PAGE18_BITUMN_SURFACE_COURSE_3 detailedItem = new EFORM_TR_TYPEC_DETAIL_PAGE18_BITUMN_SURFACE_COURSE_3();
                                    detailedItem.MAIN_ITEM_ID = typeBSummaryPage17BitumnSurfaceCourse3.Model.MAIN_ITEM_ID;
                                    detailedItem.SUBITEM_ID = typeBSummaryPage17BitumnSurfaceCourse3.Model.SUBITEM_ID;
                                    detailedItem.TABLE_ID = typeBSummaryPage17BitumnSurfaceCourse3.Model.TABLE_ID;
                                    detailedItem.ROW_ID = i;
                                    TRobjViewModel.TYPEC_DETAIL_PAGE18_BITUMN_SURFACE_COURSE_3_List.Add(detailedItem);

                                }
                                EFORM_TR_TYPEC_DETAIL_PAGE18_BITUMN_SURFACE_COURSE_3 detailedItemPAN = new EFORM_TR_TYPEC_DETAIL_PAGE18_BITUMN_SURFACE_COURSE_3();
                                detailedItemPAN.MAIN_ITEM_ID = typeBSummaryPage17BitumnSurfaceCourse3.Model.MAIN_ITEM_ID;
                                detailedItemPAN.SUBITEM_ID = typeBSummaryPage17BitumnSurfaceCourse3.Model.SUBITEM_ID;
                                detailedItemPAN.TABLE_ID = typeBSummaryPage17BitumnSurfaceCourse3.Model.TABLE_ID;
                                detailedItemPAN.ROW_ID = 16;
                                TRobjViewModel.TYPEC_DETAIL_PAGE18_BITUMN_SURFACE_COURSE_3_List.Add(detailedItemPAN);

                                foreach (var item in TRobjViewModel.TYPEC_DETAIL_PAGE18_BITUMN_SURFACE_COURSE_3_List.ToList())
                                {
                                    errorListtemp = objCommonFunction.FetchTetsReportPDFFilledDataToModel(item, loadedForm);
                                    TRobjViewModel.ErrorList.AddRange(errorListtemp);
                                }
                                foreach (var item in TRobjViewModel.TYPEC_DETAIL_PAGE18_BITUMN_SURFACE_COURSE_3_List.ToList())
                                {
                                    if (item.SAMPLE_WEIGHT_18_1 == null && item.RETAINED_WEIGHT_18_1 == null && item.CUMULATIVE_WEIGHT_18_1 == null && item.PASSING_WEIGHT_18_1 == null && item.SIEVE_DESIGNATION_18_1 == null && item.PERMISSIBLE_RANGE_18_1 == null)
                                    {
                                        TRobjViewModel.TYPEC_DETAIL_PAGE18_BITUMN_SURFACE_COURSE_3_List.Remove(item);
                                    }
                                }

                                bool sampWtEntry = false;
                                foreach (var item in TRobjViewModel.TYPEC_DETAIL_PAGE18_BITUMN_SURFACE_COURSE_3_List.ToList())
                                {
                                    if (item.SAMPLE_WEIGHT_18_1 != null)
                                    {
                                        sampWtEntry = true;
                                    }
                                }


                                if (sampWtEntry == false)
                                {

                                    errorListtemp.Clear();
                                    errorListtemp.Add("Page-18: Item 18.3.BITUMINOUS SURFACE COURSE : Gradation Analysis of Aggregates: Table -Please enter Wt. of Sample Retained(g) for atleast one I. S. Sieve Designation for Chainage-III");
                                    TRobjViewModel.ErrorOccured = true;
                                    TRobjViewModel.ErrorList.AddRange(errorListtemp);
                                    errorListtemp.Clear();
                                }
                            }


                        }
                    }
                }


                #endregion

                #region e-Form Test Report Page-20 SHOULDER - GSB (Granular)
                if (naCheckArr[46] == 'Y')
                {
                    #region Page -20, 21-21.1 SHOULDER - , Chaninage - 1 

                    errorListtemp = objCommonFunction.FetchTetsReportPDFFilledDataToModel(TRobjViewModel.TYPEB_SUMMARY_PAGE20_SHOULDER_1, loadedForm);
                    TRobjViewModel.ErrorList.AddRange(errorListtemp);
                    TRobjViewModel.TYPEB_SUMMARY_PAGE20_SHOULDER_1.MAIN_ITEM_ID = typeBSummaryPage20Shoulder1.Model.MAIN_ITEM_ID;
                    TRobjViewModel.TYPEB_SUMMARY_PAGE20_SHOULDER_1.SUBITEM_ID = typeBSummaryPage20Shoulder1.Model.SUBITEM_ID;
                    TRobjViewModel.TYPEB_SUMMARY_PAGE20_SHOULDER_1.TABLE_ID = typeBSummaryPage20Shoulder1.Model.TABLE_ID;

                    dbContext.EFORM_TR_DETAIL_ITEM_MASTER.Where(m => m.MAIN_ITEM_ID == 8 && m.SUBITEM_ID == 46).Select(t => t.DETAIL_ITEM_ID).ToList().ForEach(itemID =>
                    {
                        EFORM_TR_TYPEB_DETAIL_PAGE20_SHOULDER_1 detailedItem = new EFORM_TR_TYPEB_DETAIL_PAGE20_SHOULDER_1();
                        detailedItem.MAIN_ITEM_ID = typeBSummaryPage20Shoulder1.Model.MAIN_ITEM_ID;
                        detailedItem.SUBITEM_ID = typeBSummaryPage20Shoulder1.Model.SUBITEM_ID;
                        detailedItem.TABLE_ID = typeBSummaryPage20Shoulder1.Model.TABLE_ID;
                        detailedItem.DETAIL_ITEM_ID = itemID;
                        TRobjViewModel.TYPEB_DETAIL_PAGE20_SHOULDER_1_List.Add(detailedItem);
                    });

                    foreach (var item in TRobjViewModel.TYPEB_DETAIL_PAGE20_SHOULDER_1_List.ToList())
                    {
                        errorListtemp = objCommonFunction.FetchTetsReportPDFFilledDataToModel(item, loadedForm);
                        TRobjViewModel.ErrorList.AddRange(errorListtemp);
                    }

                    foreach (var item in TRobjViewModel.TYPEB_DETAIL_PAGE20_SHOULDER_1_List.ToList())
                    {
                        if (item.SAMPLE_WEIGHT == null && item.RETAINED_WEIGHT == null && item.CUMULATIVE_WEIGHT == null && item.PASSING_WEIGHT == null)
                        {
                            TRobjViewModel.TYPEB_DETAIL_PAGE20_SHOULDER_1_List.Remove(item);
                        }
                    }
                    if (TRobjViewModel.TYPEB_DETAIL_PAGE20_SHOULDER_1_List.Count == 0)
                    {

                        errorListtemp.Clear();
                        errorListtemp.Add("Page-20: Item 21.1.SHOULDER : GSB (Granular): Table -Please enter Wt. of Sample Retained(g) for atleast one I. S. Sieve Designation for Chainage-I");
                        TRobjViewModel.ErrorOccured = true;
                        TRobjViewModel.ErrorList.AddRange(errorListtemp);
                        errorListtemp.Clear();
                    }

                    #endregion
                }
                if (naCheckArr[47] == 'Y')
                {
                    #region Page -20, 21-21.2 SHOULDER - , Chaninage - 2 

                    errorListtemp = objCommonFunction.FetchTetsReportPDFFilledDataToModel(TRobjViewModel.TYPEB_SUMMARY_PAGE20_SHOULDER_2, loadedForm);
                    TRobjViewModel.ErrorList.AddRange(errorListtemp);
                    TRobjViewModel.TYPEB_SUMMARY_PAGE20_SHOULDER_2.MAIN_ITEM_ID = typeBSummaryPage20Shoulder2.Model.MAIN_ITEM_ID;
                    TRobjViewModel.TYPEB_SUMMARY_PAGE20_SHOULDER_2.SUBITEM_ID = typeBSummaryPage20Shoulder2.Model.SUBITEM_ID;
                    TRobjViewModel.TYPEB_SUMMARY_PAGE20_SHOULDER_2.TABLE_ID = typeBSummaryPage20Shoulder2.Model.TABLE_ID;

                    dbContext.EFORM_TR_DETAIL_ITEM_MASTER.Where(m => m.MAIN_ITEM_ID == typeBSummaryPage20Shoulder2.Model.MAIN_ITEM_ID && m.SUBITEM_ID == typeBSummaryPage20Shoulder2.Model.SUBITEM_ID).Select(t => t.DETAIL_ITEM_ID).ToList().ForEach(itemID =>
                    {
                        EFORM_TR_TYPEB_DETAIL_PAGE20_SHOULDER_2 detailedItem = new EFORM_TR_TYPEB_DETAIL_PAGE20_SHOULDER_2();
                        detailedItem.MAIN_ITEM_ID = typeBSummaryPage20Shoulder2.Model.MAIN_ITEM_ID;
                        detailedItem.SUBITEM_ID = typeBSummaryPage20Shoulder2.Model.SUBITEM_ID;
                        detailedItem.TABLE_ID = typeBSummaryPage20Shoulder2.Model.TABLE_ID;
                        detailedItem.DETAIL_ITEM_ID = itemID;
                        TRobjViewModel.TYPEB_DETAIL_PAGE20_SHOULDER_2_List.Add(detailedItem);
                    });

                    foreach (var item in TRobjViewModel.TYPEB_DETAIL_PAGE20_SHOULDER_2_List.ToList())
                    {
                        errorListtemp = objCommonFunction.FetchTetsReportPDFFilledDataToModel(item, loadedForm);
                        TRobjViewModel.ErrorList.AddRange(errorListtemp);
                    }

                    foreach (var item in TRobjViewModel.TYPEB_DETAIL_PAGE20_SHOULDER_2_List.ToList())
                    {
                        if (item.SAMPLE_WEIGHT == null && item.RETAINED_WEIGHT == null && item.CUMULATIVE_WEIGHT == null && item.PASSING_WEIGHT == null)
                        {
                            TRobjViewModel.TYPEB_DETAIL_PAGE20_SHOULDER_2_List.Remove(item);
                        }
                    }
                    if (TRobjViewModel.TYPEB_DETAIL_PAGE20_SHOULDER_2_List.Count == 0)
                    {

                        errorListtemp.Clear();
                        errorListtemp.Add("Page-20: Item 21.2.SHOULDER : GSB (Granular): Table -Please enter Wt. of Sample Retained(g) for atleast one I. S. Sieve Designation for Chainage-II");
                        TRobjViewModel.ErrorOccured = true;
                        TRobjViewModel.ErrorList.AddRange(errorListtemp);
                        errorListtemp.Clear();
                    }

                    #endregion
                }
                if (naCheckArr[48] == 'Y')
                {
                    #region Page -20, 21-21.3 SHOULDER - , Chaninage - 3

                    errorListtemp = objCommonFunction.FetchTetsReportPDFFilledDataToModel(TRobjViewModel.TYPEB_SUMMARY_PAGE20_SHOULDER_3, loadedForm);
                    TRobjViewModel.ErrorList.AddRange(errorListtemp);
                    TRobjViewModel.TYPEB_SUMMARY_PAGE20_SHOULDER_3.MAIN_ITEM_ID = typeBSummaryPage20Shoulder3.Model.MAIN_ITEM_ID;
                    TRobjViewModel.TYPEB_SUMMARY_PAGE20_SHOULDER_3.SUBITEM_ID = typeBSummaryPage20Shoulder3.Model.SUBITEM_ID;
                    TRobjViewModel.TYPEB_SUMMARY_PAGE20_SHOULDER_3.TABLE_ID = typeBSummaryPage20Shoulder3.Model.TABLE_ID;

                    dbContext.EFORM_TR_DETAIL_ITEM_MASTER.Where(m => m.MAIN_ITEM_ID == typeBSummaryPage20Shoulder3.Model.MAIN_ITEM_ID && m.SUBITEM_ID == typeBSummaryPage20Shoulder3.Model.SUBITEM_ID).Select(t => t.DETAIL_ITEM_ID).ToList().ForEach(itemID =>
                    {
                        EFORM_TR_TYPEB_DETAIL_PAGE20_SHOULDER_3 detailedItem = new EFORM_TR_TYPEB_DETAIL_PAGE20_SHOULDER_3();
                        detailedItem.MAIN_ITEM_ID = typeBSummaryPage20Shoulder3.Model.MAIN_ITEM_ID;
                        detailedItem.SUBITEM_ID = typeBSummaryPage20Shoulder3.Model.SUBITEM_ID;
                        detailedItem.TABLE_ID = typeBSummaryPage20Shoulder3.Model.TABLE_ID;
                        detailedItem.DETAIL_ITEM_ID = itemID;
                        TRobjViewModel.TYPEB_DETAIL_PAGE20_SHOULDER_3_List.Add(detailedItem);
                    });

                    foreach (var item in TRobjViewModel.TYPEB_DETAIL_PAGE20_SHOULDER_3_List.ToList())
                    {
                        errorListtemp = objCommonFunction.FetchTetsReportPDFFilledDataToModel(item, loadedForm);
                        TRobjViewModel.ErrorList.AddRange(errorListtemp);
                    }

                    foreach (var item in TRobjViewModel.TYPEB_DETAIL_PAGE20_SHOULDER_3_List.ToList())
                    {
                        if (item.SAMPLE_WEIGHT == null && item.RETAINED_WEIGHT == null && item.CUMULATIVE_WEIGHT == null && item.PASSING_WEIGHT == null)
                        {
                            TRobjViewModel.TYPEB_DETAIL_PAGE20_SHOULDER_3_List.Remove(item);
                        }
                    }
                    if (TRobjViewModel.TYPEB_DETAIL_PAGE20_SHOULDER_3_List.Count == 0)
                    {

                        errorListtemp.Clear();
                        errorListtemp.Add("Page-20: Item 21.3.SHOULDER : GSB (Granular): Table -Please enter Wt. of Sample Retained(g) for atleast one I. S. Sieve Designation for Chainage-III");
                        TRobjViewModel.ErrorOccured = true;
                        TRobjViewModel.ErrorList.AddRange(errorListtemp);
                        errorListtemp.Clear();
                    }
                    #endregion

                }

                #endregion

                #region e-Form Test Report Page-21 SHOULDER - GSB (Gravel)

                if (naCheckArr[49] == 'Y')
                {
                    #region Page -21, 21-21.1 SHOULDER -GSB (Gravel) , Chaninage - 1 

                    errorListtemp = objCommonFunction.FetchTetsReportPDFFilledDataToModel(TRobjViewModel.TYPEB_SUMMARY_PAGE21_SHOULDER_1, loadedForm);
                    TRobjViewModel.ErrorList.AddRange(errorListtemp);
                    TRobjViewModel.TYPEB_SUMMARY_PAGE21_SHOULDER_1.MAIN_ITEM_ID = typeBSummaryPage21Shoulder1.Model.MAIN_ITEM_ID;
                    TRobjViewModel.TYPEB_SUMMARY_PAGE21_SHOULDER_1.SUBITEM_ID = typeBSummaryPage21Shoulder1.Model.SUBITEM_ID;
                    TRobjViewModel.TYPEB_SUMMARY_PAGE21_SHOULDER_1.TABLE_ID = typeBSummaryPage21Shoulder1.Model.TABLE_ID;

                    dbContext.EFORM_TR_DETAIL_ITEM_MASTER.Where(m => m.MAIN_ITEM_ID == typeBSummaryPage21Shoulder1.Model.MAIN_ITEM_ID && m.SUBITEM_ID == typeBSummaryPage21Shoulder1.Model.SUBITEM_ID).Select(t => t.DETAIL_ITEM_ID).ToList().ForEach(itemID =>
                    {
                        EFORM_TR_TYPEB_DETAIL_PAGE21_SHOULDER_1 detailedItem = new EFORM_TR_TYPEB_DETAIL_PAGE21_SHOULDER_1();
                        detailedItem.MAIN_ITEM_ID = typeBSummaryPage21Shoulder1.Model.MAIN_ITEM_ID;
                        detailedItem.SUBITEM_ID = typeBSummaryPage21Shoulder1.Model.SUBITEM_ID;
                        detailedItem.TABLE_ID = typeBSummaryPage21Shoulder1.Model.TABLE_ID;
                        detailedItem.DETAIL_ITEM_ID = itemID;
                        TRobjViewModel.TYPEB_DETAIL_PAGE21_SHOULDER_1_List.Add(detailedItem);
                    });

                    foreach (var item in TRobjViewModel.TYPEB_DETAIL_PAGE21_SHOULDER_1_List.ToList())
                    {
                        errorListtemp = objCommonFunction.FetchTetsReportPDFFilledDataToModel(item, loadedForm);
                        TRobjViewModel.ErrorList.AddRange(errorListtemp);
                    }

                    foreach (var item in TRobjViewModel.TYPEB_DETAIL_PAGE21_SHOULDER_1_List.ToList())
                    {
                        if (item.SAMPLE_WEIGHT == null && item.RETAINED_WEIGHT == null && item.CUMULATIVE_WEIGHT == null && item.PASSING_WEIGHT == null)
                        {
                            TRobjViewModel.TYPEB_DETAIL_PAGE21_SHOULDER_1_List.Remove(item);
                        }
                    }
                    if (TRobjViewModel.TYPEB_DETAIL_PAGE21_SHOULDER_1_List.Count == 0)
                    {

                        errorListtemp.Clear();
                        errorListtemp.Add("Page-21: Item 21.1.SHOULDER :GSB (Gravel): Table -Please enter Wt. of Sample Retained(g) for atleast one I. S. Sieve Designation for Chainage-I");
                        TRobjViewModel.ErrorOccured = true;
                        TRobjViewModel.ErrorList.AddRange(errorListtemp);
                        errorListtemp.Clear();
                    }
                    #endregion
                }
                if (naCheckArr[50] == 'Y')
                {
                    #region Page -21, 21-21.1 SHOULDER -GSB (Gravel) , Chaninage - 2

                    errorListtemp = objCommonFunction.FetchTetsReportPDFFilledDataToModel(TRobjViewModel.TYPEB_SUMMARY_PAGE21_SHOULDER_2, loadedForm);
                    TRobjViewModel.ErrorList.AddRange(errorListtemp);
                    TRobjViewModel.TYPEB_SUMMARY_PAGE21_SHOULDER_2.MAIN_ITEM_ID = typeBSummaryPage21Shoulder2.Model.MAIN_ITEM_ID;
                    TRobjViewModel.TYPEB_SUMMARY_PAGE21_SHOULDER_2.SUBITEM_ID = typeBSummaryPage21Shoulder2.Model.SUBITEM_ID;
                    TRobjViewModel.TYPEB_SUMMARY_PAGE21_SHOULDER_2.TABLE_ID = typeBSummaryPage21Shoulder2.Model.TABLE_ID;

                    dbContext.EFORM_TR_DETAIL_ITEM_MASTER.Where(m => m.MAIN_ITEM_ID == typeBSummaryPage21Shoulder2.Model.MAIN_ITEM_ID && m.SUBITEM_ID == typeBSummaryPage21Shoulder2.Model.SUBITEM_ID).Select(t => t.DETAIL_ITEM_ID).ToList().ForEach(itemID =>
                    {
                        EFORM_TR_TYPEB_DETAIL_PAGE21_SHOULDER_2 detailedItem = new EFORM_TR_TYPEB_DETAIL_PAGE21_SHOULDER_2();
                        detailedItem.MAIN_ITEM_ID = typeBSummaryPage21Shoulder2.Model.MAIN_ITEM_ID;
                        detailedItem.SUBITEM_ID = typeBSummaryPage21Shoulder2.Model.SUBITEM_ID;
                        detailedItem.TABLE_ID = typeBSummaryPage21Shoulder2.Model.TABLE_ID;
                        detailedItem.DETAIL_ITEM_ID = itemID;
                        TRobjViewModel.TYPEB_DETAIL_PAGE21_SHOULDER_2_List.Add(detailedItem);
                    });

                    foreach (var item in TRobjViewModel.TYPEB_DETAIL_PAGE21_SHOULDER_2_List.ToList())
                    {
                        errorListtemp = objCommonFunction.FetchTetsReportPDFFilledDataToModel(item, loadedForm);
                        TRobjViewModel.ErrorList.AddRange(errorListtemp);
                    }

                    foreach (var item in TRobjViewModel.TYPEB_DETAIL_PAGE21_SHOULDER_2_List.ToList())
                    {
                        if (item.SAMPLE_WEIGHT == null && item.RETAINED_WEIGHT == null && item.CUMULATIVE_WEIGHT == null && item.PASSING_WEIGHT == null)
                        {
                            TRobjViewModel.TYPEB_DETAIL_PAGE21_SHOULDER_2_List.Remove(item);
                        }
                    }
                    if (TRobjViewModel.TYPEB_DETAIL_PAGE21_SHOULDER_2_List.Count == 0)
                    {

                        errorListtemp.Clear();
                        errorListtemp.Add("Page-21: Item 21.2.SHOULDER :GSB (Gravel): Table -Please enter Wt. of Sample Retained(g) for atleast one I. S. Sieve Designation for Chainage-II");
                        TRobjViewModel.ErrorOccured = true;
                        TRobjViewModel.ErrorList.AddRange(errorListtemp);
                        errorListtemp.Clear();
                    }
                    #endregion
                }
                if (naCheckArr[51] == 'Y')
                {
                    #region Page -21, 21-21.1 SHOULDER -GSB (Gravel) , Chaninage - 3

                    errorListtemp = objCommonFunction.FetchTetsReportPDFFilledDataToModel(TRobjViewModel.TYPEB_SUMMARY_PAGE21_SHOULDER_3, loadedForm);
                    TRobjViewModel.ErrorList.AddRange(errorListtemp);
                    TRobjViewModel.TYPEB_SUMMARY_PAGE21_SHOULDER_3.MAIN_ITEM_ID = typeBSummaryPage21Shoulder3.Model.MAIN_ITEM_ID;
                    TRobjViewModel.TYPEB_SUMMARY_PAGE21_SHOULDER_3.SUBITEM_ID = typeBSummaryPage21Shoulder3.Model.SUBITEM_ID;
                    TRobjViewModel.TYPEB_SUMMARY_PAGE21_SHOULDER_3.TABLE_ID = typeBSummaryPage21Shoulder3.Model.TABLE_ID;

                    dbContext.EFORM_TR_DETAIL_ITEM_MASTER.Where(m => m.MAIN_ITEM_ID == typeBSummaryPage21Shoulder3.Model.MAIN_ITEM_ID && m.SUBITEM_ID == typeBSummaryPage21Shoulder3.Model.SUBITEM_ID).Select(t => t.DETAIL_ITEM_ID).ToList().ForEach(itemID =>
                    {
                        EFORM_TR_TYPEB_DETAIL_PAGE21_SHOULDER_3 detailedItem = new EFORM_TR_TYPEB_DETAIL_PAGE21_SHOULDER_3();
                        detailedItem.MAIN_ITEM_ID = typeBSummaryPage21Shoulder3.Model.MAIN_ITEM_ID;
                        detailedItem.SUBITEM_ID = typeBSummaryPage21Shoulder3.Model.SUBITEM_ID;
                        detailedItem.TABLE_ID = typeBSummaryPage21Shoulder3.Model.TABLE_ID;
                        detailedItem.DETAIL_ITEM_ID = itemID;
                        TRobjViewModel.TYPEB_DETAIL_PAGE21_SHOULDER_3_List.Add(detailedItem);
                    });

                    foreach (var item in TRobjViewModel.TYPEB_DETAIL_PAGE21_SHOULDER_3_List.ToList())
                    {
                        errorListtemp = objCommonFunction.FetchTetsReportPDFFilledDataToModel(item, loadedForm);
                        TRobjViewModel.ErrorList.AddRange(errorListtemp);
                    }

                    foreach (var item in TRobjViewModel.TYPEB_DETAIL_PAGE21_SHOULDER_3_List.ToList())
                    {
                        if (item.SAMPLE_WEIGHT == null && item.RETAINED_WEIGHT == null && item.CUMULATIVE_WEIGHT == null && item.PASSING_WEIGHT == null)
                        {
                            TRobjViewModel.TYPEB_DETAIL_PAGE21_SHOULDER_3_List.Remove(item);
                        }
                    }
                    if (TRobjViewModel.TYPEB_DETAIL_PAGE21_SHOULDER_3_List.Count == 0)
                    {

                        errorListtemp.Clear();
                        errorListtemp.Add("Page-21: Item 21.3.SHOULDER :GSB (Gravel): Table -Please enter Wt. of Sample Retained(g) for atleast one I. S. Sieve Designation for Chainage-III");
                        TRobjViewModel.ErrorOccured = true;
                        TRobjViewModel.ErrorList.AddRange(errorListtemp);
                        errorListtemp.Clear();
                    }

                    #endregion
                }

                #endregion

                #endregion

                #region ----PDF READING: Vikky Type-A pages:1,5,7,9,11,15,16,18,19,22-------

                if (naCheckArr[1] == 'Y')
                {
                    #region EFORM_TR_TYPEA_SUMMARY_PAGE1_SRM_1
                    errorListtemp.Clear();
                    errorListtemp = objCommonFunction.FetchTetsReportPDFFilledDataToModel(TRobjViewModel.TYPEA_SUMMARY_PAGE1_SRM_1, loadedForm);
                    TRobjViewModel.ErrorList.AddRange(errorListtemp);

                    TRobjViewModel.TYPEA_SUMMARY_PAGE1_SRM_1.MAIN_ITEM_ID = typeASummaryPage1SRM1AssignedModel.Model.MAIN_ITEM_ID;
                    TRobjViewModel.TYPEA_SUMMARY_PAGE1_SRM_1.SUBITEM_ID = typeASummaryPage1SRM1AssignedModel.Model.SUBITEM_ID;
                    TRobjViewModel.TYPEA_SUMMARY_PAGE1_SRM_1.TABLE_ID = typeASummaryPage1SRM1AssignedModel.Model.TABLE_ID;



                    TestReportModel.EFORM_TR_SUBITEM_OPTIONS_SELECTED_DETAIL modelSub = new TestReportModel.EFORM_TR_SUBITEM_OPTIONS_SELECTED_DETAIL();
                    modelSub.MAIN_ITEM_ID = typeASummaryPage1SRM1AssignedModel.Model.MAIN_ITEM_ID;
                    modelSub.SUBITEM_ID = typeASummaryPage1SRM1AssignedModel.Model.SUBITEM_ID;
                    modelSub.OPTION_ID = TRobjViewModel.TYPEA_SUMMARY_PAGE1_SRM_1.Field_Density_1_1;
                    TRobjViewModel.SUBITEM_OPTIONS_SELECTED_DETAIL_List.Add(modelSub);
                    #endregion

                    #region EFORM_TR_TYPEA_DETAIL_PAGE1_SRM_1
                    dbContext.EFORM_TR_DETAIL_ITEM_MASTER.Where(m => m.MAIN_ITEM_ID == typeASummaryPage1SRM1AssignedModel.Model.MAIN_ITEM_ID && m.SUBITEM_ID == typeASummaryPage1SRM1AssignedModel.Model.SUBITEM_ID).Select(x => x.DETAIL_ITEM_ID).ToList().ForEach(itemID =>
                    {
                        TestReportModel.EFORM_TR_TYPEA_DETAIL_PAGE1_SRM_1 detailItem = new TestReportModel.EFORM_TR_TYPEA_DETAIL_PAGE1_SRM_1();
                        detailItem.DETAIL_ITEM_ID = itemID;
                        TRobjViewModel.TYPEA_DETAIL_PAGE1_SRM_1_List.Add(detailItem);
                    });

                    foreach (var item in TRobjViewModel.TYPEA_DETAIL_PAGE1_SRM_1_List.ToList())
                    {
                        errorListtemp.Clear();
                        errorListtemp = objCommonFunction.FetchTetsReportPDFFilledDataToModel(item, loadedForm);
                        TRobjViewModel.ErrorList.AddRange(errorListtemp);

                    }


                    PdfLoadedRadioButtonListField MOISTURE_CONTENT_METHOD_1_1_Value = loadedForm.Fields["MOISTURE_CONTENT_METHOD_1_1"] as PdfLoadedRadioButtonListField;
                    if (MOISTURE_CONTENT_METHOD_1_1_Value.SelectedValue == "RMM")
                    {

                        TestReportModel.EFORM_TR_TYPEA_DETAIL_PAGE1_SRM_1 detailItem = new TestReportModel.EFORM_TR_TYPEA_DETAIL_PAGE1_SRM_1();
                        detailItem.DETAIL_ITEM_ID = typeASummaryPage1SRM1AssignedModel.Model.REPLACE_TO;
                        detailItem.CH1 = TRobjViewModel.TYPEA_DETAIL_PAGE1_SRM_1_List.Where(s => s.DETAIL_ITEM_ID == typeASummaryPage1SRM1AssignedModel.Model.REPLACE_FROM).Select(m => m.CH1).FirstOrDefault();
                        detailItem.CH2 = TRobjViewModel.TYPEA_DETAIL_PAGE1_SRM_1_List.Where(s => s.DETAIL_ITEM_ID == typeASummaryPage1SRM1AssignedModel.Model.REPLACE_FROM).Select(m => m.CH2).FirstOrDefault();
                        detailItem.CH3 = TRobjViewModel.TYPEA_DETAIL_PAGE1_SRM_1_List.Where(s => s.DETAIL_ITEM_ID == typeASummaryPage1SRM1AssignedModel.Model.REPLACE_FROM).Select(m => m.CH3).FirstOrDefault();
                        TRobjViewModel.TYPEA_DETAIL_PAGE1_SRM_1_List.Add(detailItem);
                        TestReportModel.EFORM_TR_TYPEA_DETAIL_PAGE1_SRM_1 removeDetailItem = new TestReportModel.EFORM_TR_TYPEA_DETAIL_PAGE1_SRM_1();
                        removeDetailItem = TRobjViewModel.TYPEA_DETAIL_PAGE1_SRM_1_List.Where(s => s.DETAIL_ITEM_ID == typeASummaryPage1SRM1AssignedModel.Model.REPLACE_FROM).FirstOrDefault();
                        TRobjViewModel.TYPEA_DETAIL_PAGE1_SRM_1_List.Remove(removeDetailItem);
                    }
                    foreach (var item in TRobjViewModel.TYPEA_DETAIL_PAGE1_SRM_1_List.ToList())
                    {
                        if (item.CH1 == null && item.CH2 == null && item.CH3 == null)
                        {
                            TRobjViewModel.TYPEA_DETAIL_PAGE1_SRM_1_List.Remove(item);
                        }
                    }
                    #endregion

                }

                if (naCheckArr[2] == 'Y')
                {
                    #region EFORM_TR_TYPEA_SUMMARY_PAGE1_CCM_2
                    errorListtemp.Clear();
                    errorListtemp = objCommonFunction.FetchTetsReportPDFFilledDataToModel(TRobjViewModel.TYPEA_SUMMARY_PAGE1_CCM_2, loadedForm);
                    TRobjViewModel.ErrorList.AddRange(errorListtemp);
                    TRobjViewModel.TYPEA_SUMMARY_PAGE1_CCM_2.MAIN_ITEM_ID = typeASummaryPage1CCM2AssignedModel.Model.MAIN_ITEM_ID;
                    TRobjViewModel.TYPEA_SUMMARY_PAGE1_CCM_2.SUBITEM_ID = typeASummaryPage1CCM2AssignedModel.Model.SUBITEM_ID;
                    TRobjViewModel.TYPEA_SUMMARY_PAGE1_CCM_2.TABLE_ID = typeASummaryPage1CCM2AssignedModel.Model.TABLE_ID;



                    TestReportModel.EFORM_TR_SUBITEM_OPTIONS_SELECTED_DETAIL modelSub = new TestReportModel.EFORM_TR_SUBITEM_OPTIONS_SELECTED_DETAIL();
                    modelSub.MAIN_ITEM_ID = typeASummaryPage1CCM2AssignedModel.Model.MAIN_ITEM_ID;
                    modelSub.SUBITEM_ID = typeASummaryPage1CCM2AssignedModel.Model.SUBITEM_ID;
                    modelSub.OPTION_ID = TRobjViewModel.TYPEA_SUMMARY_PAGE1_CCM_2.Field_Density_2_1;
                    TRobjViewModel.SUBITEM_OPTIONS_SELECTED_DETAIL_List.Add(modelSub);
                    #endregion

                    #region EFORM_TR_TYPEA_DETAIL_PAGE1_CCM_2
                    dbContext.EFORM_TR_DETAIL_ITEM_MASTER.Where(m => m.MAIN_ITEM_ID == typeASummaryPage1CCM2AssignedModel.Model.MAIN_ITEM_ID && m.SUBITEM_ID == typeASummaryPage1CCM2AssignedModel.Model.SUBITEM_ID).Select(x => x.DETAIL_ITEM_ID).ToList().ForEach(itemID =>
                    {
                        TestReportModel.EFORM_TR_TYPEA_DETAIL_PAGE1_CCM_2 detailItem = new TestReportModel.EFORM_TR_TYPEA_DETAIL_PAGE1_CCM_2();
                        detailItem.DETAIL_ITEM_ID = itemID;
                        TRobjViewModel.TYPEA_DETAIL_PAGE1_CCM_2_List.Add(detailItem);
                    });

                    foreach (var item in TRobjViewModel.TYPEA_DETAIL_PAGE1_CCM_2_List.ToList())
                    {
                        errorListtemp.Clear();
                        errorListtemp = objCommonFunction.FetchTetsReportPDFFilledDataToModel(item, loadedForm);
                        TRobjViewModel.ErrorList.AddRange(errorListtemp);

                    }


                    PdfLoadedRadioButtonListField MOISTURE_CONTENT_METHOD_1_2_Value = loadedForm.Fields["MOISTURE_CONTENT_METHOD_1_2"] as PdfLoadedRadioButtonListField;
                    if (MOISTURE_CONTENT_METHOD_1_2_Value.SelectedValue == "RMM")
                    {

                        TestReportModel.EFORM_TR_TYPEA_DETAIL_PAGE1_CCM_2 detailItem = new TestReportModel.EFORM_TR_TYPEA_DETAIL_PAGE1_CCM_2();
                        detailItem.DETAIL_ITEM_ID = typeASummaryPage1CCM2AssignedModel.Model.REPLACE_TO;
                        detailItem.CH1 = TRobjViewModel.TYPEA_DETAIL_PAGE1_CCM_2_List.Where(s => s.DETAIL_ITEM_ID == typeASummaryPage1CCM2AssignedModel.Model.REPLACE_FROM).Select(m => m.CH1).FirstOrDefault();
                        detailItem.CH2 = TRobjViewModel.TYPEA_DETAIL_PAGE1_CCM_2_List.Where(s => s.DETAIL_ITEM_ID == typeASummaryPage1CCM2AssignedModel.Model.REPLACE_FROM).Select(m => m.CH2).FirstOrDefault();
                        detailItem.CH3 = TRobjViewModel.TYPEA_DETAIL_PAGE1_CCM_2_List.Where(s => s.DETAIL_ITEM_ID == typeASummaryPage1CCM2AssignedModel.Model.REPLACE_FROM).Select(m => m.CH3).FirstOrDefault();
                        TRobjViewModel.TYPEA_DETAIL_PAGE1_CCM_2_List.Add(detailItem);
                        TestReportModel.EFORM_TR_TYPEA_DETAIL_PAGE1_CCM_2 removeDetailItem = new TestReportModel.EFORM_TR_TYPEA_DETAIL_PAGE1_CCM_2();
                        removeDetailItem = TRobjViewModel.TYPEA_DETAIL_PAGE1_CCM_2_List.Where(s => s.DETAIL_ITEM_ID == typeASummaryPage1CCM2AssignedModel.Model.REPLACE_FROM).FirstOrDefault();
                        TRobjViewModel.TYPEA_DETAIL_PAGE1_CCM_2_List.Remove(removeDetailItem);
                    }
                    foreach (var item in TRobjViewModel.TYPEA_DETAIL_PAGE1_CCM_2_List.ToList())
                    {
                        if (item.CH1 == null && item.CH2 == null && item.CH3 == null)
                        {
                            TRobjViewModel.TYPEA_DETAIL_PAGE1_CCM_2_List.Remove(item);
                        }
                    }
                    #endregion

                }

                if (naCheckArr[12] == 'Y')
                {
                    #region EFORM_TR_TYPEA_SUMMARY_PAGE5_SRM_12
                    errorListtemp.Clear();
                    errorListtemp = objCommonFunction.FetchTetsReportPDFFilledDataToModel(TRobjViewModel.TYPEA_SUMMARY_PAGE5_SRM_12, loadedForm);
                    TRobjViewModel.ErrorList.AddRange(errorListtemp);

                    TRobjViewModel.TYPEA_SUMMARY_PAGE5_SRM_12.MAIN_ITEM_ID = typeASummaryPage5SRM12AssignedModel.Model.MAIN_ITEM_ID;
                    TRobjViewModel.TYPEA_SUMMARY_PAGE5_SRM_12.SUBITEM_ID = typeASummaryPage5SRM12AssignedModel.Model.SUBITEM_ID;
                    TRobjViewModel.TYPEA_SUMMARY_PAGE5_SRM_12.TABLE_ID = typeASummaryPage5SRM12AssignedModel.Model.TABLE_ID;


                    #endregion

                    #region EFORM_TR_TYPEA_DETAIL_PAGE5_SRM_12
                    dbContext.EFORM_TR_DETAIL_ITEM_MASTER.Where(m => m.MAIN_ITEM_ID == typeASummaryPage5SRM12AssignedModel.Model.MAIN_ITEM_ID && m.SUBITEM_ID == typeASummaryPage5SRM12AssignedModel.Model.SUBITEM_ID).Select(x => x.DETAIL_ITEM_ID).ToList().ForEach(itemID =>
                    {
                        TestReportModel.EFORM_TR_TYPEA_DETAIL_PAGE5_SRM_12 detailItem = new TestReportModel.EFORM_TR_TYPEA_DETAIL_PAGE5_SRM_12();
                        detailItem.DETAIL_ITEM_ID = itemID;
                        TRobjViewModel.TYPEA_DETAIL_PAGE5_SRM_12_List.Add(detailItem);
                    });

                    foreach (var item in TRobjViewModel.TYPEA_DETAIL_PAGE5_SRM_12_List.ToList())
                    {
                        errorListtemp.Clear();
                        errorListtemp = objCommonFunction.FetchTetsReportPDFFilledDataToModel(item, loadedForm);
                        TRobjViewModel.ErrorList.AddRange(errorListtemp);

                    }


                    PdfLoadedRadioButtonListField MOISTURE_CONTENT_METHOD_5_1_Value = loadedForm.Fields["MOISTURE_CONTENT_METHOD_5_1"] as PdfLoadedRadioButtonListField;
                    if (MOISTURE_CONTENT_METHOD_5_1_Value.SelectedValue == "RMM")
                    {

                        TestReportModel.EFORM_TR_TYPEA_DETAIL_PAGE5_SRM_12 detailItem = new TestReportModel.EFORM_TR_TYPEA_DETAIL_PAGE5_SRM_12();
                        detailItem.DETAIL_ITEM_ID = typeASummaryPage5SRM12AssignedModel.Model.REPLACE_TO;
                        detailItem.CH1 = TRobjViewModel.TYPEA_DETAIL_PAGE5_SRM_12_List.Where(s => s.DETAIL_ITEM_ID == typeASummaryPage5SRM12AssignedModel.Model.REPLACE_FROM).Select(m => m.CH1).FirstOrDefault();
                        detailItem.CH2 = TRobjViewModel.TYPEA_DETAIL_PAGE5_SRM_12_List.Where(s => s.DETAIL_ITEM_ID == typeASummaryPage5SRM12AssignedModel.Model.REPLACE_FROM).Select(m => m.CH2).FirstOrDefault();
                        detailItem.CH3 = TRobjViewModel.TYPEA_DETAIL_PAGE5_SRM_12_List.Where(s => s.DETAIL_ITEM_ID == typeASummaryPage5SRM12AssignedModel.Model.REPLACE_FROM).Select(m => m.CH3).FirstOrDefault();
                        TRobjViewModel.TYPEA_DETAIL_PAGE5_SRM_12_List.Add(detailItem);
                        TestReportModel.EFORM_TR_TYPEA_DETAIL_PAGE5_SRM_12 removeDetailItem = new TestReportModel.EFORM_TR_TYPEA_DETAIL_PAGE5_SRM_12();
                        removeDetailItem = TRobjViewModel.TYPEA_DETAIL_PAGE5_SRM_12_List.Where(s => s.DETAIL_ITEM_ID == typeASummaryPage5SRM12AssignedModel.Model.REPLACE_FROM).FirstOrDefault();
                        TRobjViewModel.TYPEA_DETAIL_PAGE5_SRM_12_List.Remove(removeDetailItem);
                    }
                    foreach (var item in TRobjViewModel.TYPEA_DETAIL_PAGE5_SRM_12_List.ToList())
                    {
                        if (item.CH1 == null && item.CH2 == null && item.CH3 == null)
                        {
                            TRobjViewModel.TYPEA_DETAIL_PAGE5_SRM_12_List.Remove(item);
                        }
                    }
                    #endregion

                }

                if (naCheckArr[16] == 'Y')
                {
                    #region EFORM_TR_TYPEA_SUMMARY_PAGE7_SRM_16
                    errorListtemp.Clear();
                    errorListtemp = objCommonFunction.FetchTetsReportPDFFilledDataToModel(TRobjViewModel.TYPEA_SUMMARY_PAGE7_SRM_16, loadedForm);
                    TRobjViewModel.ErrorList.AddRange(errorListtemp);

                    TRobjViewModel.TYPEA_SUMMARY_PAGE7_SRM_16.MAIN_ITEM_ID = typeASummaryPage7SRM16AssignedModel.Model.MAIN_ITEM_ID;
                    TRobjViewModel.TYPEA_SUMMARY_PAGE7_SRM_16.SUBITEM_ID = typeASummaryPage7SRM16AssignedModel.Model.SUBITEM_ID;
                    TRobjViewModel.TYPEA_SUMMARY_PAGE7_SRM_16.TABLE_ID = typeASummaryPage7SRM16AssignedModel.Model.TABLE_ID;


                    #endregion

                    #region EFORM_TR_TYPEA_DETAIL_PAGE7_SRM_16
                    dbContext.EFORM_TR_DETAIL_ITEM_MASTER.Where(m => m.MAIN_ITEM_ID == typeASummaryPage7SRM16AssignedModel.Model.MAIN_ITEM_ID && m.SUBITEM_ID == typeASummaryPage7SRM16AssignedModel.Model.SUBITEM_ID).Select(x => x.DETAIL_ITEM_ID).ToList().ForEach(itemID =>
                    {
                        TestReportModel.EFORM_TR_TYPEA_DETAIL_PAGE7_SRM_16 detailItem = new TestReportModel.EFORM_TR_TYPEA_DETAIL_PAGE7_SRM_16();
                        detailItem.DETAIL_ITEM_ID = itemID;
                        TRobjViewModel.TYPEA_DETAIL_PAGE7_SRM_16_List.Add(detailItem);
                    });

                    foreach (var item in TRobjViewModel.TYPEA_DETAIL_PAGE7_SRM_16_List.ToList())
                    {
                        errorListtemp.Clear();
                        errorListtemp = objCommonFunction.FetchTetsReportPDFFilledDataToModel(item, loadedForm);
                        TRobjViewModel.ErrorList.AddRange(errorListtemp);

                    }


                    PdfLoadedRadioButtonListField MOISTURE_CONTENT_METHOD_7_1_Value = loadedForm.Fields["MOISTURE_CONTENT_METHOD_7_1"] as PdfLoadedRadioButtonListField;
                    if (MOISTURE_CONTENT_METHOD_7_1_Value.SelectedValue == "RMM")
                    {

                        TestReportModel.EFORM_TR_TYPEA_DETAIL_PAGE7_SRM_16 detailItem = new TestReportModel.EFORM_TR_TYPEA_DETAIL_PAGE7_SRM_16();
                        detailItem.DETAIL_ITEM_ID = typeASummaryPage7SRM16AssignedModel.Model.REPLACE_TO;
                        detailItem.CH1 = TRobjViewModel.TYPEA_DETAIL_PAGE7_SRM_16_List.Where(s => s.DETAIL_ITEM_ID == typeASummaryPage7SRM16AssignedModel.Model.REPLACE_FROM).Select(m => m.CH1).FirstOrDefault();
                        detailItem.CH2 = TRobjViewModel.TYPEA_DETAIL_PAGE7_SRM_16_List.Where(s => s.DETAIL_ITEM_ID == typeASummaryPage7SRM16AssignedModel.Model.REPLACE_FROM).Select(m => m.CH2).FirstOrDefault();
                        detailItem.CH3 = TRobjViewModel.TYPEA_DETAIL_PAGE7_SRM_16_List.Where(s => s.DETAIL_ITEM_ID == typeASummaryPage7SRM16AssignedModel.Model.REPLACE_FROM).Select(m => m.CH3).FirstOrDefault();
                        TRobjViewModel.TYPEA_DETAIL_PAGE7_SRM_16_List.Add(detailItem);
                        TestReportModel.EFORM_TR_TYPEA_DETAIL_PAGE7_SRM_16 removeDetailItem = new TestReportModel.EFORM_TR_TYPEA_DETAIL_PAGE7_SRM_16();
                        removeDetailItem = TRobjViewModel.TYPEA_DETAIL_PAGE7_SRM_16_List.Where(s => s.DETAIL_ITEM_ID == typeASummaryPage7SRM16AssignedModel.Model.REPLACE_FROM).FirstOrDefault();
                        TRobjViewModel.TYPEA_DETAIL_PAGE7_SRM_16_List.Remove(removeDetailItem);
                    }
                    foreach (var item in TRobjViewModel.TYPEA_DETAIL_PAGE7_SRM_16_List.ToList())
                    {
                        if (item.CH1 == null && item.CH2 == null && item.CH3 == null)
                        {
                            TRobjViewModel.TYPEA_DETAIL_PAGE7_SRM_16_List.Remove(item);
                        }
                    }
                    #endregion

                }

                PdfLoadedRadioButtonListField Base_Course_1_select = loadedForm.Fields["Base_Course_1"] as PdfLoadedRadioButtonListField;
                if (naCheckArr[17] == 'Y' && (Base_Course_1_select.SelectedValue == "8" || Base_Course_1_select.SelectedValue == "9" || Base_Course_1_select.SelectedValue == "10"))
                {
                    #region EFORM_TR_TYPEA_SUMMARY_PAGE7_SRM_17
                    errorListtemp.Clear();
                    errorListtemp = objCommonFunction.FetchTetsReportPDFFilledDataToModel(TRobjViewModel.TYPEA_SUMMARY_PAGE7_SRM_17, loadedForm);
                    TRobjViewModel.ErrorList.AddRange(errorListtemp);

                    TRobjViewModel.TYPEA_SUMMARY_PAGE7_SRM_17.MAIN_ITEM_ID = typeASummaryPage7SRM17AssignedModel.Model.MAIN_ITEM_ID;
                    TRobjViewModel.TYPEA_SUMMARY_PAGE7_SRM_17.SUBITEM_ID = typeASummaryPage7SRM17AssignedModel.Model.SUBITEM_ID;
                    TRobjViewModel.TYPEA_SUMMARY_PAGE7_SRM_17.TABLE_ID = typeASummaryPage7SRM17AssignedModel.Model.TABLE_ID;

                    TestReportModel.EFORM_TR_SUBITEM_OPTIONS_SELECTED_DETAIL modelSub = new TestReportModel.EFORM_TR_SUBITEM_OPTIONS_SELECTED_DETAIL();
                    modelSub.MAIN_ITEM_ID = typeASummaryPage7SRM17AssignedModel.Model.MAIN_ITEM_ID;
                    modelSub.SUBITEM_ID = typeASummaryPage7SRM17AssignedModel.Model.SUBITEM_ID;
                    modelSub.OPTION_ID = TRobjViewModel.TYPEA_SUMMARY_PAGE7_SRM_17.Type_7;
                    TRobjViewModel.SUBITEM_OPTIONS_SELECTED_DETAIL_List.Add(modelSub);
                    #endregion

                    #region EFORM_TR_TYPEA_DETAIL_PAGE7_SRM_17
                    dbContext.EFORM_TR_DETAIL_ITEM_MASTER.Where(m => m.MAIN_ITEM_ID == typeASummaryPage7SRM17AssignedModel.Model.MAIN_ITEM_ID && m.SUBITEM_ID == typeASummaryPage7SRM17AssignedModel.Model.SUBITEM_ID).Select(x => x.DETAIL_ITEM_ID).ToList().ForEach(itemID =>
                    {
                        TestReportModel.EFORM_TR_TYPEA_DETAIL_PAGE7_SRM_17 detailItem = new TestReportModel.EFORM_TR_TYPEA_DETAIL_PAGE7_SRM_17();
                        detailItem.DETAIL_ITEM_ID = itemID;
                        TRobjViewModel.TYPEA_DETAIL_PAGE7_SRM_17_List.Add(detailItem);
                    });

                    foreach (var item in TRobjViewModel.TYPEA_DETAIL_PAGE7_SRM_17_List.ToList())
                    {
                        errorListtemp.Clear();
                        errorListtemp = objCommonFunction.FetchTetsReportPDFFilledDataToModel(item, loadedForm);
                        TRobjViewModel.ErrorList.AddRange(errorListtemp);

                    }



                    foreach (var item in TRobjViewModel.TYPEA_DETAIL_PAGE7_SRM_17_List.ToList())
                    {
                        if (item.CH1 == null && item.CH2 == null && item.CH3 == null)
                        {
                            TRobjViewModel.TYPEA_DETAIL_PAGE7_SRM_17_List.Remove(item);
                        }
                    }
                    #endregion

                }

                if (naCheckArr[21] == 'Y')
                {
                    #region EFORM_TR_TYPEA_SUMMARY_PAGE9_SRM_21
                    errorListtemp.Clear();
                    errorListtemp = objCommonFunction.FetchTetsReportPDFFilledDataToModel(TRobjViewModel.TYPEA_SUMMARY_PAGE9_SRM_21, loadedForm);
                    TRobjViewModel.ErrorList.AddRange(errorListtemp);

                    TRobjViewModel.TYPEA_SUMMARY_PAGE9_SRM_21.MAIN_ITEM_ID = typeASummaryPage9SRM21AssignedModel.Model.MAIN_ITEM_ID;
                    TRobjViewModel.TYPEA_SUMMARY_PAGE9_SRM_21.SUBITEM_ID = typeASummaryPage9SRM21AssignedModel.Model.SUBITEM_ID;
                    TRobjViewModel.TYPEA_SUMMARY_PAGE9_SRM_21.TABLE_ID = typeASummaryPage9SRM21AssignedModel.Model.TABLE_ID;


                    #endregion

                    #region EFORM_TR_TYPEA_DETAIL_PAGE9_SRM_21
                    dbContext.EFORM_TR_DETAIL_ITEM_MASTER.Where(m => m.MAIN_ITEM_ID == typeASummaryPage9SRM21AssignedModel.Model.MAIN_ITEM_ID && m.SUBITEM_ID == typeASummaryPage9SRM21AssignedModel.Model.SUBITEM_ID).Select(x => x.DETAIL_ITEM_ID).ToList().ForEach(itemID =>
                    {
                        TestReportModel.EFORM_TR_TYPEA_DETAIL_PAGE9_SRM_21 detailItem = new TestReportModel.EFORM_TR_TYPEA_DETAIL_PAGE9_SRM_21();
                        detailItem.DETAIL_ITEM_ID = itemID;
                        TRobjViewModel.TYPEA_DETAIL_PAGE9_SRM_21_List.Add(detailItem);
                    });

                    foreach (var item in TRobjViewModel.TYPEA_DETAIL_PAGE9_SRM_21_List.ToList())
                    {
                        errorListtemp.Clear();
                        errorListtemp = objCommonFunction.FetchTetsReportPDFFilledDataToModel(item, loadedForm);
                        TRobjViewModel.ErrorList.AddRange(errorListtemp);

                    }


                    PdfLoadedRadioButtonListField MOISTURE_CONTENT_METHOD_9_1_Value = loadedForm.Fields["MOISTURE_CONTENT_METHOD_9_1"] as PdfLoadedRadioButtonListField;
                    if (MOISTURE_CONTENT_METHOD_9_1_Value.SelectedValue == "RMM")
                    {

                        TestReportModel.EFORM_TR_TYPEA_DETAIL_PAGE9_SRM_21 detailItem = new TestReportModel.EFORM_TR_TYPEA_DETAIL_PAGE9_SRM_21();
                        detailItem.DETAIL_ITEM_ID = typeASummaryPage9SRM21AssignedModel.Model.REPLACE_TO;
                        detailItem.CH1 = TRobjViewModel.TYPEA_DETAIL_PAGE9_SRM_21_List.Where(s => s.DETAIL_ITEM_ID == typeASummaryPage9SRM21AssignedModel.Model.REPLACE_FROM).Select(m => m.CH1).FirstOrDefault();
                        detailItem.CH2 = TRobjViewModel.TYPEA_DETAIL_PAGE9_SRM_21_List.Where(s => s.DETAIL_ITEM_ID == typeASummaryPage9SRM21AssignedModel.Model.REPLACE_FROM).Select(m => m.CH2).FirstOrDefault();
                        detailItem.CH3 = TRobjViewModel.TYPEA_DETAIL_PAGE9_SRM_21_List.Where(s => s.DETAIL_ITEM_ID == typeASummaryPage9SRM21AssignedModel.Model.REPLACE_FROM).Select(m => m.CH3).FirstOrDefault();
                        TRobjViewModel.TYPEA_DETAIL_PAGE9_SRM_21_List.Add(detailItem);
                        TestReportModel.EFORM_TR_TYPEA_DETAIL_PAGE9_SRM_21 removeDetailItem = new TestReportModel.EFORM_TR_TYPEA_DETAIL_PAGE9_SRM_21();
                        removeDetailItem = TRobjViewModel.TYPEA_DETAIL_PAGE9_SRM_21_List.Where(s => s.DETAIL_ITEM_ID == typeASummaryPage9SRM21AssignedModel.Model.REPLACE_FROM).FirstOrDefault();
                        TRobjViewModel.TYPEA_DETAIL_PAGE9_SRM_21_List.Remove(removeDetailItem);
                    }
                    foreach (var item in TRobjViewModel.TYPEA_DETAIL_PAGE9_SRM_21_List.ToList())
                    {
                        if (item.CH1 == null && item.CH2 == null && item.CH3 == null)
                        {
                            TRobjViewModel.TYPEA_DETAIL_PAGE9_SRM_21_List.Remove(item);
                        }
                    }
                    #endregion

                }

                PdfLoadedRadioButtonListField Base_Course_2_select = loadedForm.Fields["Base_Course_2"] as PdfLoadedRadioButtonListField;
                if (naCheckArr[22] == 'Y' && (Base_Course_2_select.SelectedValue == "14" || Base_Course_2_select.SelectedValue == "15" || Base_Course_2_select.SelectedValue == "16"))
                {
                    #region EFORM_TR_TYPEA_SUMMARY_PAGE9_SRM_22
                    errorListtemp.Clear();
                    errorListtemp = objCommonFunction.FetchTetsReportPDFFilledDataToModel(TRobjViewModel.TYPEA_SUMMARY_PAGE9_SRM_22, loadedForm);
                    TRobjViewModel.ErrorList.AddRange(errorListtemp);

                    TRobjViewModel.TYPEA_SUMMARY_PAGE9_SRM_22.MAIN_ITEM_ID = typeASummaryPage9SRM22AssignedModel.Model.MAIN_ITEM_ID;
                    TRobjViewModel.TYPEA_SUMMARY_PAGE9_SRM_22.SUBITEM_ID = typeASummaryPage9SRM22AssignedModel.Model.SUBITEM_ID;
                    TRobjViewModel.TYPEA_SUMMARY_PAGE9_SRM_22.TABLE_ID = typeASummaryPage9SRM22AssignedModel.Model.TABLE_ID;

                    TestReportModel.EFORM_TR_SUBITEM_OPTIONS_SELECTED_DETAIL modelSub = new TestReportModel.EFORM_TR_SUBITEM_OPTIONS_SELECTED_DETAIL();
                    modelSub.MAIN_ITEM_ID = typeASummaryPage9SRM22AssignedModel.Model.MAIN_ITEM_ID;
                    modelSub.SUBITEM_ID = typeASummaryPage9SRM22AssignedModel.Model.SUBITEM_ID;
                    modelSub.OPTION_ID = TRobjViewModel.TYPEA_SUMMARY_PAGE9_SRM_22.Type_9;
                    TRobjViewModel.SUBITEM_OPTIONS_SELECTED_DETAIL_List.Add(modelSub);
                    #endregion

                    #region EFORM_TR_TYPEA_DETAIL_PAGE9_SRM_22
                    dbContext.EFORM_TR_DETAIL_ITEM_MASTER.Where(m => m.MAIN_ITEM_ID == typeASummaryPage9SRM22AssignedModel.Model.MAIN_ITEM_ID && m.SUBITEM_ID == typeASummaryPage9SRM22AssignedModel.Model.SUBITEM_ID).Select(x => x.DETAIL_ITEM_ID).ToList().ForEach(itemID =>
                    {
                        TestReportModel.EFORM_TR_TYPEA_DETAIL_PAGE9_SRM_22 detailItem = new TestReportModel.EFORM_TR_TYPEA_DETAIL_PAGE9_SRM_22();
                        detailItem.DETAIL_ITEM_ID = itemID;
                        TRobjViewModel.TYPEA_DETAIL_PAGE9_SRM_22_List.Add(detailItem);
                    });

                    foreach (var item in TRobjViewModel.TYPEA_DETAIL_PAGE9_SRM_22_List.ToList())
                    {
                        errorListtemp.Clear();
                        errorListtemp = objCommonFunction.FetchTetsReportPDFFilledDataToModel(item, loadedForm);
                        TRobjViewModel.ErrorList.AddRange(errorListtemp);

                    }



                    foreach (var item in TRobjViewModel.TYPEA_DETAIL_PAGE9_SRM_22_List.ToList())
                    {
                        if (item.CH1 == null && item.CH2 == null && item.CH3 == null)
                        {
                            TRobjViewModel.TYPEA_DETAIL_PAGE9_SRM_22_List.Remove(item);
                        }
                    }
                    #endregion

                }

                if (naCheckArr[26] == 'Y')
                {
                    #region EFORM_TR_TYPEA_SUMMARY_PAGE11_SRM_26
                    errorListtemp.Clear();
                    errorListtemp = objCommonFunction.FetchTetsReportPDFFilledDataToModel(TRobjViewModel.TYPEA_SUMMARY_PAGE11_SRM_26, loadedForm);
                    TRobjViewModel.ErrorList.AddRange(errorListtemp);

                    TRobjViewModel.TYPEA_SUMMARY_PAGE11_SRM_26.MAIN_ITEM_ID = typeASummaryPage11SRM26AssignedModel.Model.MAIN_ITEM_ID;
                    TRobjViewModel.TYPEA_SUMMARY_PAGE11_SRM_26.SUBITEM_ID = typeASummaryPage11SRM26AssignedModel.Model.SUBITEM_ID;
                    TRobjViewModel.TYPEA_SUMMARY_PAGE11_SRM_26.TABLE_ID = typeASummaryPage11SRM26AssignedModel.Model.TABLE_ID;


                    #endregion

                    #region EFORM_TR_TYPEA_DETAIL_PAGE11_SRM_26
                    dbContext.EFORM_TR_DETAIL_ITEM_MASTER.Where(m => m.MAIN_ITEM_ID == typeASummaryPage11SRM26AssignedModel.Model.MAIN_ITEM_ID && m.SUBITEM_ID == typeASummaryPage11SRM26AssignedModel.Model.SUBITEM_ID).Select(x => x.DETAIL_ITEM_ID).ToList().ForEach(itemID =>
                    {
                        TestReportModel.EFORM_TR_TYPEA_DETAIL_PAGE11_SRM_26 detailItem = new TestReportModel.EFORM_TR_TYPEA_DETAIL_PAGE11_SRM_26();
                        detailItem.DETAIL_ITEM_ID = itemID;
                        TRobjViewModel.TYPEA_DETAIL_PAGE11_SRM_26_List.Add(detailItem);
                    });

                    foreach (var item in TRobjViewModel.TYPEA_DETAIL_PAGE11_SRM_26_List.ToList())
                    {
                        errorListtemp.Clear();
                        errorListtemp = objCommonFunction.FetchTetsReportPDFFilledDataToModel(item, loadedForm);
                        TRobjViewModel.ErrorList.AddRange(errorListtemp);

                    }


                    PdfLoadedRadioButtonListField MOISTURE_CONTENT_METHOD_11_1_Value = loadedForm.Fields["MOISTURE_CONTENT_METHOD_11_1"] as PdfLoadedRadioButtonListField;
                    if (MOISTURE_CONTENT_METHOD_11_1_Value.SelectedValue == "RMM")
                    {

                        TestReportModel.EFORM_TR_TYPEA_DETAIL_PAGE11_SRM_26 detailItem = new TestReportModel.EFORM_TR_TYPEA_DETAIL_PAGE11_SRM_26();
                        detailItem.DETAIL_ITEM_ID = typeASummaryPage11SRM26AssignedModel.Model.REPLACE_TO;
                        detailItem.CH1 = TRobjViewModel.TYPEA_DETAIL_PAGE11_SRM_26_List.Where(s => s.DETAIL_ITEM_ID == typeASummaryPage11SRM26AssignedModel.Model.REPLACE_FROM).Select(m => m.CH1).FirstOrDefault();
                        detailItem.CH2 = TRobjViewModel.TYPEA_DETAIL_PAGE11_SRM_26_List.Where(s => s.DETAIL_ITEM_ID == typeASummaryPage11SRM26AssignedModel.Model.REPLACE_FROM).Select(m => m.CH2).FirstOrDefault();
                        detailItem.CH3 = TRobjViewModel.TYPEA_DETAIL_PAGE11_SRM_26_List.Where(s => s.DETAIL_ITEM_ID == typeASummaryPage11SRM26AssignedModel.Model.REPLACE_FROM).Select(m => m.CH3).FirstOrDefault();
                        TRobjViewModel.TYPEA_DETAIL_PAGE11_SRM_26_List.Add(detailItem);
                        TestReportModel.EFORM_TR_TYPEA_DETAIL_PAGE11_SRM_26 removeDetailItem = new TestReportModel.EFORM_TR_TYPEA_DETAIL_PAGE11_SRM_26();
                        removeDetailItem = TRobjViewModel.TYPEA_DETAIL_PAGE11_SRM_26_List.Where(s => s.DETAIL_ITEM_ID == typeASummaryPage11SRM26AssignedModel.Model.REPLACE_FROM).FirstOrDefault();
                        TRobjViewModel.TYPEA_DETAIL_PAGE11_SRM_26_List.Remove(removeDetailItem);
                    }
                    foreach (var item in TRobjViewModel.TYPEA_DETAIL_PAGE11_SRM_26_List.ToList())
                    {
                        if (item.CH1 == null && item.CH2 == null && item.CH3 == null)
                        {
                            TRobjViewModel.TYPEA_DETAIL_PAGE11_SRM_26_List.Remove(item);
                        }
                    }
                    #endregion

                }

                PdfLoadedRadioButtonListField Base_Course_3_select = loadedForm.Fields["Base_Course_3"] as PdfLoadedRadioButtonListField;
                if (naCheckArr[27] == 'Y' && (Base_Course_3_select.SelectedValue == "20" || Base_Course_3_select.SelectedValue == "21" || Base_Course_3_select.SelectedValue == "22"))
                {
                    #region EFORM_TR_TYPEA_SUMMARY_PAGE11_SRM_27
                    errorListtemp.Clear();
                    errorListtemp = objCommonFunction.FetchTetsReportPDFFilledDataToModel(TRobjViewModel.TYPEA_SUMMARY_PAGE11_SRM_27, loadedForm);
                    TRobjViewModel.ErrorList.AddRange(errorListtemp);

                    TRobjViewModel.TYPEA_SUMMARY_PAGE11_SRM_27.MAIN_ITEM_ID = typeASummaryPage11SRM27AssignedModel.Model.MAIN_ITEM_ID;
                    TRobjViewModel.TYPEA_SUMMARY_PAGE11_SRM_27.SUBITEM_ID = typeASummaryPage11SRM27AssignedModel.Model.SUBITEM_ID;
                    TRobjViewModel.TYPEA_SUMMARY_PAGE11_SRM_27.TABLE_ID = typeASummaryPage11SRM27AssignedModel.Model.TABLE_ID;

                    TestReportModel.EFORM_TR_SUBITEM_OPTIONS_SELECTED_DETAIL modelSub = new TestReportModel.EFORM_TR_SUBITEM_OPTIONS_SELECTED_DETAIL();
                    modelSub.MAIN_ITEM_ID = typeASummaryPage11SRM27AssignedModel.Model.MAIN_ITEM_ID;
                    modelSub.SUBITEM_ID = typeASummaryPage11SRM27AssignedModel.Model.SUBITEM_ID;
                    modelSub.OPTION_ID = TRobjViewModel.TYPEA_SUMMARY_PAGE11_SRM_27.Type_11;
                    TRobjViewModel.SUBITEM_OPTIONS_SELECTED_DETAIL_List.Add(modelSub);
                    #endregion

                    #region EFORM_TR_TYPEA_DETAIL_PAGE11_SRM_27
                    dbContext.EFORM_TR_DETAIL_ITEM_MASTER.Where(m => m.MAIN_ITEM_ID == typeASummaryPage11SRM27AssignedModel.Model.MAIN_ITEM_ID && m.SUBITEM_ID == typeASummaryPage11SRM27AssignedModel.Model.SUBITEM_ID).Select(x => x.DETAIL_ITEM_ID).ToList().ForEach(itemID =>
                    {
                        TestReportModel.EFORM_TR_TYPEA_DETAIL_PAGE11_SRM_27 detailItem = new TestReportModel.EFORM_TR_TYPEA_DETAIL_PAGE11_SRM_27();
                        detailItem.DETAIL_ITEM_ID = itemID;
                        TRobjViewModel.TYPEA_DETAIL_PAGE11_SRM_27_List.Add(detailItem);
                    });

                    foreach (var item in TRobjViewModel.TYPEA_DETAIL_PAGE11_SRM_27_List.ToList())
                    {
                        errorListtemp.Clear();
                        errorListtemp = objCommonFunction.FetchTetsReportPDFFilledDataToModel(item, loadedForm);
                        TRobjViewModel.ErrorList.AddRange(errorListtemp);

                    }



                    foreach (var item in TRobjViewModel.TYPEA_DETAIL_PAGE11_SRM_27_List.ToList())
                    {
                        if (item.CH1 == null && item.CH2 == null && item.CH3 == null)
                        {
                            TRobjViewModel.TYPEA_DETAIL_PAGE11_SRM_27_List.Remove(item);
                        }
                    }
                    #endregion

                }

                if (naCheckArr[37] == 'Y')
                {
                    #region EFORM_TR_TYPEA_SUMMARY_PAGE15_SRM_37
                    errorListtemp.Clear();
                    errorListtemp = objCommonFunction.FetchTetsReportPDFFilledDataToModel(TRobjViewModel.TYPEA_SUMMARY_PAGE15_SRM_37, loadedForm);
                    TRobjViewModel.ErrorList.AddRange(errorListtemp);

                    TRobjViewModel.TYPEA_SUMMARY_PAGE15_SRM_37.MAIN_ITEM_ID = typeASummaryPage15SRM37AssignedModel.Model.MAIN_ITEM_ID;
                    TRobjViewModel.TYPEA_SUMMARY_PAGE15_SRM_37.SUBITEM_ID = typeASummaryPage15SRM37AssignedModel.Model.SUBITEM_ID;
                    TRobjViewModel.TYPEA_SUMMARY_PAGE15_SRM_37.TABLE_ID = typeASummaryPage15SRM37AssignedModel.Model.TABLE_ID;




                    #endregion

                    #region EFORM_TR_TYPEA_DETAIL_PAGE15_SRM_37
                    dbContext.EFORM_TR_DETAIL_ITEM_MASTER.Where(m => m.MAIN_ITEM_ID == typeASummaryPage15SRM37AssignedModel.Model.MAIN_ITEM_ID && m.SUBITEM_ID == typeASummaryPage15SRM37AssignedModel.Model.SUBITEM_ID).Select(x => x.DETAIL_ITEM_ID).ToList().ForEach(itemID =>
                    {
                        TestReportModel.EFORM_TR_TYPEA_DETAIL_PAGE15_SRM_37 detailItem = new TestReportModel.EFORM_TR_TYPEA_DETAIL_PAGE15_SRM_37();
                        detailItem.DETAIL_ITEM_ID = itemID;
                        TRobjViewModel.TYPEA_DETAIL_PAGE15_SRM_37_List.Add(detailItem);
                    });

                    foreach (var item in TRobjViewModel.TYPEA_DETAIL_PAGE15_SRM_37_List.ToList())
                    {
                        errorListtemp.Clear();
                        errorListtemp = objCommonFunction.FetchTetsReportPDFFilledDataToModel(item, loadedForm);
                        TRobjViewModel.ErrorList.AddRange(errorListtemp);

                    }


                    //PdfLoadedRadioButtonListField MOISTURE_CONTENT_METHOD_15_1_Value = loadedForm.Fields["MOISTURE_CONTENT_METHOD_15_1"] as PdfLoadedRadioButtonListField;
                    //if (MOISTURE_CONTENT_METHOD_15_1_Value.SelectedValue == "RMM")
                    //{

                    //    TestReportModel.EFORM_TR_TYPEA_DETAIL_PAGE15_SRM_37 detailItem = new TestReportModel.EFORM_TR_TYPEA_DETAIL_PAGE15_SRM_37();
                    //    detailItem.DETAIL_ITEM_ID = typeASummaryPage15SRM37AssignedModel.Model.REPLACE_TO;
                    //    detailItem.CH1 = TRobjViewModel.TYPEA_DETAIL_PAGE15_SRM_37_List.Where(s => s.DETAIL_ITEM_ID == typeASummaryPage15SRM37AssignedModel.Model.REPLACE_FROM).Select(m => m.CH1).FirstOrDefault();
                    //    detailItem.CH2 = TRobjViewModel.TYPEA_DETAIL_PAGE15_SRM_37_List.Where(s => s.DETAIL_ITEM_ID == typeASummaryPage15SRM37AssignedModel.Model.REPLACE_FROM).Select(m => m.CH2).FirstOrDefault();
                    //    detailItem.CH3 = TRobjViewModel.TYPEA_DETAIL_PAGE15_SRM_37_List.Where(s => s.DETAIL_ITEM_ID == typeASummaryPage15SRM37AssignedModel.Model.REPLACE_FROM).Select(m => m.CH3).FirstOrDefault();
                    //    TRobjViewModel.TYPEA_DETAIL_PAGE15_SRM_37_List.Add(detailItem);
                    //    TestReportModel.EFORM_TR_TYPEA_DETAIL_PAGE15_SRM_37 removeDetailItem = new TestReportModel.EFORM_TR_TYPEA_DETAIL_PAGE15_SRM_37();
                    //    removeDetailItem = TRobjViewModel.TYPEA_DETAIL_PAGE15_SRM_37_List.Where(s => s.DETAIL_ITEM_ID == typeASummaryPage15SRM37AssignedModel.Model.REPLACE_FROM).FirstOrDefault();
                    //    TRobjViewModel.TYPEA_DETAIL_PAGE15_SRM_37_List.Remove(removeDetailItem);
                    //}
                    foreach (var item in TRobjViewModel.TYPEA_DETAIL_PAGE15_SRM_37_List.ToList())
                    {
                        if (item.CH1 == null && item.CH2 == null && item.CH3 == null)
                        {
                            TRobjViewModel.TYPEA_DETAIL_PAGE15_SRM_37_List.Remove(item);
                        }
                    }
                    #endregion

                }

                if (naCheckArr[38] == 'Y')
                {
                    #region EFORM_TR_TYPEA_SUMMARY_PAGE16_CCM_38
                    errorListtemp.Clear();
                    errorListtemp = objCommonFunction.FetchTetsReportPDFFilledDataToModel(TRobjViewModel.TYPEA_SUMMARY_PAGE16_CCM_38, loadedForm);
                    TRobjViewModel.ErrorList.AddRange(errorListtemp);
                    TRobjViewModel.TYPEA_SUMMARY_PAGE16_CCM_38.MAIN_ITEM_ID = typeASummaryPage15CCM38AssignedModel.Model.MAIN_ITEM_ID;
                    TRobjViewModel.TYPEA_SUMMARY_PAGE16_CCM_38.SUBITEM_ID = typeASummaryPage15CCM38AssignedModel.Model.SUBITEM_ID;
                    TRobjViewModel.TYPEA_SUMMARY_PAGE16_CCM_38.TABLE_ID = typeASummaryPage15CCM38AssignedModel.Model.TABLE_ID;


                    #endregion

                    #region EFORM_TR_TYPEA_DETAIL_PAGE15_CCM_38
                    dbContext.EFORM_TR_DETAIL_ITEM_MASTER.Where(m => m.MAIN_ITEM_ID == typeASummaryPage15CCM38AssignedModel.Model.MAIN_ITEM_ID && m.SUBITEM_ID == typeASummaryPage15CCM38AssignedModel.Model.SUBITEM_ID).Select(x => x.DETAIL_ITEM_ID).ToList().ForEach(itemID =>
                    {
                        TestReportModel.EFORM_TR_TYPEA_DETAIL_PAGE16_CCM_38 detailItem = new TestReportModel.EFORM_TR_TYPEA_DETAIL_PAGE16_CCM_38();
                        detailItem.DETAIL_ITEM_ID = itemID;
                        TRobjViewModel.TYPEA_DETAIL_PAGE16_CCM_38_List.Add(detailItem);
                    });

                    foreach (var item in TRobjViewModel.TYPEA_DETAIL_PAGE16_CCM_38_List.ToList())
                    {
                        errorListtemp.Clear();
                        errorListtemp = objCommonFunction.FetchTetsReportPDFFilledDataToModel(item, loadedForm);
                        TRobjViewModel.ErrorList.AddRange(errorListtemp);

                    }


                    //PdfLoadedRadioButtonListField MOISTURE_CONTENT_METHOD_15_2_Value = loadedForm.Fields["MOISTURE_CONTENT_METHOD_15_2"] as PdfLoadedRadioButtonListField;
                    //if (MOISTURE_CONTENT_METHOD_15_2_Value.SelectedValue == "RMM")
                    //{

                    //    TestReportModel.EFORM_TR_TYPEA_DETAIL_PAGE15_CCM_38 detailItem = new TestReportModel.EFORM_TR_TYPEA_DETAIL_PAGE15_CCM_38();
                    //    detailItem.DETAIL_ITEM_ID = typeASummaryPage15CCM38AssignedModel.Model.REPLACE_TO;
                    //    detailItem.CH1 = TRobjViewModel.TYPEA_DETAIL_PAGE16_CCM_38_List.Where(s => s.DETAIL_ITEM_ID == typeASummaryPage15CCM38AssignedModel.Model.REPLACE_FROM).Select(m => m.CH1).FirstOrDefault();
                    //    detailItem.CH2 = TRobjViewModel.TYPEA_DETAIL_PAGE16_CCM_38_List.Where(s => s.DETAIL_ITEM_ID == typeASummaryPage15CCM38AssignedModel.Model.REPLACE_FROM).Select(m => m.CH2).FirstOrDefault();
                    //    detailItem.CH3 = TRobjViewModel.TYPEA_DETAIL_PAGE16_CCM_38_List.Where(s => s.DETAIL_ITEM_ID == typeASummaryPage15CCM38AssignedModel.Model.REPLACE_FROM).Select(m => m.CH3).FirstOrDefault();
                    //    TRobjViewModel.TYPEA_DETAIL_PAGE16_CCM_38_List.Add(detailItem);
                    //    TestReportModel.EFORM_TR_TYPEA_DETAIL_PAGE15_CCM_38 removeDetailItem = new TestReportModel.EFORM_TR_TYPEA_DETAIL_PAGE15_CCM_38();
                    //    removeDetailItem = TRobjViewModel.TYPEA_DETAIL_PAGE16_CCM_38_List.Where(s => s.DETAIL_ITEM_ID == typeASummaryPage15CCM38AssignedModel.Model.REPLACE_FROM).FirstOrDefault();
                    //    TRobjViewModel.TYPEA_DETAIL_PAGE16_CCM_38_List.Remove(removeDetailItem);
                    //}
                    foreach (var item in TRobjViewModel.TYPEA_DETAIL_PAGE16_CCM_38_List.ToList())
                    {
                        if (item.CH1 == null && item.CH2 == null && item.CH3 == null)
                        {
                            TRobjViewModel.TYPEA_DETAIL_PAGE16_CCM_38_List.Remove(item);
                        }
                    }
                    #endregion

                }

                if (naCheckArr[39] == 'Y')
                {
                    #region EFORM_TR_TYPEA_SUMMARY_PAGE16_SRM_39
                    errorListtemp.Clear();
                    errorListtemp = objCommonFunction.FetchTetsReportPDFFilledDataToModel(TRobjViewModel.TYPEA_SUMMARY_PAGE16_SRM_39, loadedForm);
                    TRobjViewModel.ErrorList.AddRange(errorListtemp);

                    TRobjViewModel.TYPEA_SUMMARY_PAGE16_SRM_39.MAIN_ITEM_ID = typeASummaryPage16SRM39AssignedModel.Model.MAIN_ITEM_ID;
                    TRobjViewModel.TYPEA_SUMMARY_PAGE16_SRM_39.SUBITEM_ID = typeASummaryPage16SRM39AssignedModel.Model.SUBITEM_ID;
                    TRobjViewModel.TYPEA_SUMMARY_PAGE16_SRM_39.TABLE_ID = typeASummaryPage16SRM39AssignedModel.Model.TABLE_ID;




                    #endregion

                    #region EFORM_TR_TYPEA_DETAIL_PAGE16_SRM_39
                    dbContext.EFORM_TR_DETAIL_ITEM_MASTER.Where(m => m.MAIN_ITEM_ID == typeASummaryPage16SRM39AssignedModel.Model.MAIN_ITEM_ID && m.SUBITEM_ID == typeASummaryPage16SRM39AssignedModel.Model.SUBITEM_ID).Select(x => x.DETAIL_ITEM_ID).ToList().ForEach(itemID =>
                    {
                        TestReportModel.EFORM_TR_TYPEA_DETAIL_PAGE16_SRM_39 detailItem = new TestReportModel.EFORM_TR_TYPEA_DETAIL_PAGE16_SRM_39();
                        detailItem.DETAIL_ITEM_ID = itemID;
                        TRobjViewModel.TYPEA_DETAIL_PAGE16_SRM_39_List.Add(detailItem);
                    });

                    foreach (var item in TRobjViewModel.TYPEA_DETAIL_PAGE16_SRM_39_List.ToList())
                    {
                        errorListtemp.Clear();
                        errorListtemp = objCommonFunction.FetchTetsReportPDFFilledDataToModel(item, loadedForm);
                        TRobjViewModel.ErrorList.AddRange(errorListtemp);

                    }



                    foreach (var item in TRobjViewModel.TYPEA_DETAIL_PAGE16_SRM_39_List.ToList())
                    {
                        if (item.CH1 == null && item.CH2 == null && item.CH3 == null)
                        {
                            TRobjViewModel.TYPEA_DETAIL_PAGE16_SRM_39_List.Remove(item);
                        }
                    }
                    #endregion

                }

                if (naCheckArr[43] == 'Y')
                {
                    #region EFORM_TR_TYPEA_SUMMARY_PAGE18_SRM_43
                    errorListtemp.Clear();
                    errorListtemp = objCommonFunction.FetchTetsReportPDFFilledDataToModel(TRobjViewModel.TYPEA_SUMMARY_PAGE18_SRM_43, loadedForm);
                    TRobjViewModel.ErrorList.AddRange(errorListtemp);

                    TRobjViewModel.TYPEA_SUMMARY_PAGE18_SRM_43.MAIN_ITEM_ID = typeASummaryPage18SRM43AssignedModel.Model.MAIN_ITEM_ID;
                    TRobjViewModel.TYPEA_SUMMARY_PAGE18_SRM_43.SUBITEM_ID = typeASummaryPage18SRM43AssignedModel.Model.SUBITEM_ID;
                    TRobjViewModel.TYPEA_SUMMARY_PAGE18_SRM_43.TABLE_ID = typeASummaryPage18SRM43AssignedModel.Model.TABLE_ID;




                    #endregion

                    #region EFORM_TR_TYPEA_DETAIL_PAGE18_SRM_43
                    dbContext.EFORM_TR_DETAIL_ITEM_MASTER.Where(m => m.MAIN_ITEM_ID == typeASummaryPage18SRM43AssignedModel.Model.MAIN_ITEM_ID && m.SUBITEM_ID == typeASummaryPage18SRM43AssignedModel.Model.SUBITEM_ID).Select(x => x.DETAIL_ITEM_ID).ToList().ForEach(itemID =>
                    {
                        TestReportModel.EFORM_TR_TYPEA_DETAIL_PAGE18_SRM_43 detailItem = new TestReportModel.EFORM_TR_TYPEA_DETAIL_PAGE18_SRM_43();
                        detailItem.DETAIL_ITEM_ID = itemID;
                        TRobjViewModel.TYPEA_DETAIL_PAGE18_SRM_43_List.Add(detailItem);
                    });

                    foreach (var item in TRobjViewModel.TYPEA_DETAIL_PAGE18_SRM_43_List.ToList())
                    {
                        errorListtemp.Clear();
                        errorListtemp = objCommonFunction.FetchTetsReportPDFFilledDataToModel(item, loadedForm);
                        TRobjViewModel.ErrorList.AddRange(errorListtemp);

                    }



                    foreach (var item in TRobjViewModel.TYPEA_DETAIL_PAGE18_SRM_43_List.ToList())
                    {
                        if (item.CH1 == null && item.CH2 == null && item.CH3 == null)
                        {
                            TRobjViewModel.TYPEA_DETAIL_PAGE18_SRM_43_List.Remove(item);
                        }
                    }
                    #endregion

                }


                TestReportModel.EFORM_TR_MAIN_ITEM_OPTIONS_SELECTED_DETAIL modelMain = new TestReportModel.EFORM_TR_MAIN_ITEM_OPTIONS_SELECTED_DETAIL();
                modelMain.MAIN_ITEM_ID = typeASummaryPage19SRM44AssignedModel.Model.MAIN_ITEM_ID;

                PdfLoadedRadioButtonListField Shoulder = loadedForm.Fields["Shoulder"] as PdfLoadedRadioButtonListField;
                if (Shoulder.SelectedValue != null)
                {
                    modelMain.OPTION_ID = Convert.ToInt32(Shoulder.SelectedValue);
                }
                else
                {
                    errorListtemp.Clear();
                    errorListtemp.Add("Page-19: SHOULDER:- Please select shoulder type");
                    TRobjViewModel.ErrorList.AddRange(errorListtemp);
                    errorListtemp.Clear();
                    TRobjViewModel.ErrorOccured = true;
                }

                TRobjViewModel.MAIN_ITEM_OPTIONS_SELECTED_DETAIL_List.Add(modelMain);

                if (naCheckArr[44] == 'Y' && (Shoulder.SelectedValue == "36" || Shoulder.SelectedValue == "37"))
                {
                    #region EFORM_TR_TYPEA_SUMMARY_PAGE19_SRM_44
                    errorListtemp.Clear();
                    errorListtemp = objCommonFunction.FetchTetsReportPDFFilledDataToModel(TRobjViewModel.TYPEA_SUMMARY_PAGE19_SRM_44, loadedForm);
                    TRobjViewModel.ErrorList.AddRange(errorListtemp);

                    TRobjViewModel.TYPEA_SUMMARY_PAGE19_SRM_44.MAIN_ITEM_ID = typeASummaryPage19SRM44AssignedModel.Model.MAIN_ITEM_ID;
                    TRobjViewModel.TYPEA_SUMMARY_PAGE19_SRM_44.SUBITEM_ID = typeASummaryPage19SRM44AssignedModel.Model.SUBITEM_ID;
                    TRobjViewModel.TYPEA_SUMMARY_PAGE19_SRM_44.TABLE_ID = typeASummaryPage19SRM44AssignedModel.Model.TABLE_ID;




                    #endregion

                    #region EFORM_TR_TYPEA_DETAIL_PAGE19_SRM_44
                    dbContext.EFORM_TR_DETAIL_ITEM_MASTER.Where(m => m.MAIN_ITEM_ID == typeASummaryPage19SRM44AssignedModel.Model.MAIN_ITEM_ID && m.SUBITEM_ID == typeASummaryPage19SRM44AssignedModel.Model.SUBITEM_ID).Select(x => x.DETAIL_ITEM_ID).ToList().ForEach(itemID =>
                    {
                        TestReportModel.EFORM_TR_TYPEA_DETAIL_PAGE19_SRM_44 detailItem = new TestReportModel.EFORM_TR_TYPEA_DETAIL_PAGE19_SRM_44();
                        detailItem.DETAIL_ITEM_ID = itemID;
                        TRobjViewModel.TYPEA_DETAIL_PAGE19_SRM_44_List.Add(detailItem);
                    });

                    foreach (var item in TRobjViewModel.TYPEA_DETAIL_PAGE19_SRM_44_List.ToList())
                    {
                        errorListtemp.Clear();
                        errorListtemp = objCommonFunction.FetchTetsReportPDFFilledDataToModel(item, loadedForm);
                        TRobjViewModel.ErrorList.AddRange(errorListtemp);

                    }


                    PdfLoadedRadioButtonListField MOISTURE_CONTENT_METHOD_19_1_Value = loadedForm.Fields["MOISTURE_CONTENT_METHOD_19_1"] as PdfLoadedRadioButtonListField;
                    if (MOISTURE_CONTENT_METHOD_19_1_Value.SelectedValue == "RMM")
                    {

                        TestReportModel.EFORM_TR_TYPEA_DETAIL_PAGE19_SRM_44 detailItem = new TestReportModel.EFORM_TR_TYPEA_DETAIL_PAGE19_SRM_44();
                        detailItem.DETAIL_ITEM_ID = typeASummaryPage19SRM44AssignedModel.Model.REPLACE_TO;
                        detailItem.CH1 = TRobjViewModel.TYPEA_DETAIL_PAGE19_SRM_44_List.Where(s => s.DETAIL_ITEM_ID == typeASummaryPage19SRM44AssignedModel.Model.REPLACE_FROM).Select(m => m.CH1).FirstOrDefault();
                        detailItem.CH2 = TRobjViewModel.TYPEA_DETAIL_PAGE19_SRM_44_List.Where(s => s.DETAIL_ITEM_ID == typeASummaryPage19SRM44AssignedModel.Model.REPLACE_FROM).Select(m => m.CH2).FirstOrDefault();
                        detailItem.CH3 = TRobjViewModel.TYPEA_DETAIL_PAGE19_SRM_44_List.Where(s => s.DETAIL_ITEM_ID == typeASummaryPage19SRM44AssignedModel.Model.REPLACE_FROM).Select(m => m.CH3).FirstOrDefault();
                        TRobjViewModel.TYPEA_DETAIL_PAGE19_SRM_44_List.Add(detailItem);
                        TestReportModel.EFORM_TR_TYPEA_DETAIL_PAGE19_SRM_44 removeDetailItem = new TestReportModel.EFORM_TR_TYPEA_DETAIL_PAGE19_SRM_44();
                        removeDetailItem = TRobjViewModel.TYPEA_DETAIL_PAGE19_SRM_44_List.Where(s => s.DETAIL_ITEM_ID == typeASummaryPage19SRM44AssignedModel.Model.REPLACE_FROM).FirstOrDefault();
                        TRobjViewModel.TYPEA_DETAIL_PAGE19_SRM_44_List.Remove(removeDetailItem);
                    }
                    foreach (var item in TRobjViewModel.TYPEA_DETAIL_PAGE19_SRM_44_List.ToList())
                    {
                        if (item.CH1 == null && item.CH2 == null && item.CH3 == null)
                        {
                            TRobjViewModel.TYPEA_DETAIL_PAGE19_SRM_44_List.Remove(item);
                        }
                    }
                    #endregion

                }

                if (naCheckArr[45] == 'Y' && (Shoulder.SelectedValue == "35" || Shoulder.SelectedValue == "36"))
                {
                    #region EFORM_TR_TYPEA_SUMMARY_PAGE19_CCM_45
                    errorListtemp.Clear();
                    errorListtemp = objCommonFunction.FetchTetsReportPDFFilledDataToModel(TRobjViewModel.TYPEA_SUMMARY_PAGE19_CCM_45, loadedForm);
                    TRobjViewModel.ErrorList.AddRange(errorListtemp);
                    TRobjViewModel.TYPEA_SUMMARY_PAGE19_CCM_45.MAIN_ITEM_ID = typeASummaryPage19CCM45AssignedModel.Model.MAIN_ITEM_ID;
                    TRobjViewModel.TYPEA_SUMMARY_PAGE19_CCM_45.SUBITEM_ID = typeASummaryPage19CCM45AssignedModel.Model.SUBITEM_ID;
                    TRobjViewModel.TYPEA_SUMMARY_PAGE19_CCM_45.TABLE_ID = typeASummaryPage19CCM45AssignedModel.Model.TABLE_ID;




                    #endregion

                    #region EFORM_TR_TYPEA_DETAIL_PAGE19_CCM_45
                    dbContext.EFORM_TR_DETAIL_ITEM_MASTER.Where(m => m.MAIN_ITEM_ID == typeASummaryPage19CCM45AssignedModel.Model.MAIN_ITEM_ID && m.SUBITEM_ID == typeASummaryPage19CCM45AssignedModel.Model.SUBITEM_ID).Select(x => x.DETAIL_ITEM_ID).ToList().ForEach(itemID =>
                    {
                        TestReportModel.EFORM_TR_TYPEA_DETAIL_PAGE19_CCM_45 detailItem = new TestReportModel.EFORM_TR_TYPEA_DETAIL_PAGE19_CCM_45();
                        detailItem.DETAIL_ITEM_ID = itemID;
                        TRobjViewModel.TYPEA_DETAIL_PAGE19_CCM_45_List.Add(detailItem);
                    });

                    foreach (var item in TRobjViewModel.TYPEA_DETAIL_PAGE19_CCM_45_List.ToList())
                    {
                        errorListtemp.Clear();
                        errorListtemp = objCommonFunction.FetchTetsReportPDFFilledDataToModel(item, loadedForm);
                        TRobjViewModel.ErrorList.AddRange(errorListtemp);

                    }


                    PdfLoadedRadioButtonListField MOISTURE_CONTENT_METHOD_19_2_Value = loadedForm.Fields["MOISTURE_CONTENT_METHOD_19_2"] as PdfLoadedRadioButtonListField;
                    if (MOISTURE_CONTENT_METHOD_19_2_Value.SelectedValue == "RMM")
                    {

                        TestReportModel.EFORM_TR_TYPEA_DETAIL_PAGE19_CCM_45 detailItem = new TestReportModel.EFORM_TR_TYPEA_DETAIL_PAGE19_CCM_45();
                        detailItem.DETAIL_ITEM_ID = typeASummaryPage19CCM45AssignedModel.Model.REPLACE_TO;
                        detailItem.CH1 = TRobjViewModel.TYPEA_DETAIL_PAGE19_CCM_45_List.Where(s => s.DETAIL_ITEM_ID == typeASummaryPage19CCM45AssignedModel.Model.REPLACE_FROM).Select(m => m.CH1).FirstOrDefault();
                        detailItem.CH2 = TRobjViewModel.TYPEA_DETAIL_PAGE19_CCM_45_List.Where(s => s.DETAIL_ITEM_ID == typeASummaryPage19CCM45AssignedModel.Model.REPLACE_FROM).Select(m => m.CH2).FirstOrDefault();
                        detailItem.CH3 = TRobjViewModel.TYPEA_DETAIL_PAGE19_CCM_45_List.Where(s => s.DETAIL_ITEM_ID == typeASummaryPage19CCM45AssignedModel.Model.REPLACE_FROM).Select(m => m.CH3).FirstOrDefault();
                        TRobjViewModel.TYPEA_DETAIL_PAGE19_CCM_45_List.Add(detailItem);
                        TestReportModel.EFORM_TR_TYPEA_DETAIL_PAGE19_CCM_45 removeDetailItem = new TestReportModel.EFORM_TR_TYPEA_DETAIL_PAGE19_CCM_45();
                        removeDetailItem = TRobjViewModel.TYPEA_DETAIL_PAGE19_CCM_45_List.Where(s => s.DETAIL_ITEM_ID == typeASummaryPage19CCM45AssignedModel.Model.REPLACE_FROM).FirstOrDefault();
                        TRobjViewModel.TYPEA_DETAIL_PAGE19_CCM_45_List.Remove(removeDetailItem);
                    }
                    foreach (var item in TRobjViewModel.TYPEA_DETAIL_PAGE19_CCM_45_List.ToList())
                    {
                        if (item.CH1 == null && item.CH2 == null && item.CH3 == null)
                        {
                            TRobjViewModel.TYPEA_DETAIL_PAGE19_CCM_45_List.Remove(item);
                        }
                    }
                    #endregion

                }

                if (naCheckArr[52] == 'Y')
                {
                    #region EFORM_TR_TYPEA_SUMMARY_PAGE22_SRM_52
                    errorListtemp.Clear();
                    errorListtemp = objCommonFunction.FetchTetsReportPDFFilledDataToModel(TRobjViewModel.TYPEA_SUMMARY_PAGE22_SRM_52, loadedForm);
                    TRobjViewModel.ErrorList.AddRange(errorListtemp);

                    TRobjViewModel.TYPEA_SUMMARY_PAGE22_SRM_52.MAIN_ITEM_ID = typeASummaryPage22SRM52AssignedModel.Model.MAIN_ITEM_ID;
                    TRobjViewModel.TYPEA_SUMMARY_PAGE22_SRM_52.SUBITEM_ID = typeASummaryPage22SRM52AssignedModel.Model.SUBITEM_ID;
                    TRobjViewModel.TYPEA_SUMMARY_PAGE22_SRM_52.TABLE_ID = typeASummaryPage22SRM52AssignedModel.Model.TABLE_ID;




                    #endregion

                    #region EFORM_TR_TYPEA_DETAIL_PAGE22_SRM_52
                    dbContext.EFORM_TR_DETAIL_ITEM_MASTER.Where(m => m.MAIN_ITEM_ID == typeASummaryPage22SRM52AssignedModel.Model.MAIN_ITEM_ID && m.SUBITEM_ID == typeASummaryPage22SRM52AssignedModel.Model.SUBITEM_ID).Select(x => x.DETAIL_ITEM_ID).ToList().ForEach(itemID =>
                    {
                        TestReportModel.EFORM_TR_TYPEA_DETAIL_PAGE22_SRM_52 detailItem = new TestReportModel.EFORM_TR_TYPEA_DETAIL_PAGE22_SRM_52();
                        detailItem.DETAIL_ITEM_ID = itemID;
                        TRobjViewModel.TYPEA_DETAIL_PAGE22_SRM_52_List.Add(detailItem);
                    });

                    foreach (var item in TRobjViewModel.TYPEA_DETAIL_PAGE22_SRM_52_List.ToList())
                    {
                        errorListtemp.Clear();
                        errorListtemp = objCommonFunction.FetchTetsReportPDFFilledDataToModel(item, loadedForm);
                        TRobjViewModel.ErrorList.AddRange(errorListtemp);

                    }
                    #region to make atleast two blows mandatory
                    int countFilledBlowsCH1 = 0;
                    int countFilledBlowsCH2 = 0;
                    int countFilledBlowsCH3 = 0;
                    foreach (var item in TRobjViewModel.TYPEA_DETAIL_PAGE22_SRM_52_List.ToList())
                    {
                        if (item.DETAIL_ITEM_ID >= 233 && item.DETAIL_ITEM_ID <= 248)
                        {
                            if (item.CH1 != null)
                            {
                                countFilledBlowsCH1++;
                            }
                            //if (item.CH2 != null)
                            //{
                            //    countFilledBlowsCH2++;
                            //}
                            //if (item.CH3 != null)
                            //{
                            //    countFilledBlowsCH3++;
                            //}
                        }

                    }
                    if (countFilledBlowsCH1 < 2)
                    {
                        errorListtemp.Clear();
                        errorListtemp.Add("Page-22: NEW TECHNOLOGY TEST (Dynamic Cone Penetrometer (DCP) Test) Table:- Please enter Reading (mm) atleast for two blows for CH1");
                        TRobjViewModel.ErrorList.AddRange(errorListtemp);
                        errorListtemp.Clear();
                        TRobjViewModel.ErrorOccured = true;
                    }

                    //if (isCH1Mand == true && countFilledBlowsCH1 < 2)
                    //{
                    //    errorListtemp.Clear();
                    //    errorListtemp.Add("Page-22: NEW TECHNOLOGY TEST (Dynamic Cone Penetrometer (DCP) Test) Table:- Please enter Reading (mm) atleast for two blows for CH1");
                    //    TRobjViewModel.ErrorList.AddRange(errorListtemp);
                    //    errorListtemp.Clear();
                    //    TRobjViewModel.ErrorOccured = true;
                    //}
                    //if (isCH2Mand == true && countFilledBlowsCH2 < 2)
                    //{
                    //    errorListtemp.Clear();
                    //    errorListtemp.Add("Page-22: NEW TECHNOLOGY TEST (Dynamic Cone Penetrometer (DCP) Test) Table:- Please enter Reading (mm) atleast for two blows for CH2");
                    //    TRobjViewModel.ErrorList.AddRange(errorListtemp);
                    //    errorListtemp.Clear();
                    //    TRobjViewModel.ErrorOccured = true;
                    //}
                    //if (isCH3Mand == true && countFilledBlowsCH3 < 2)
                    //{
                    //    errorListtemp.Clear();
                    //    errorListtemp.Add("Page-22: NEW TECHNOLOGY TEST (Dynamic Cone Penetrometer (DCP) Test) Table:- Please enter Reading (mm) atleast for two blows for CH3");
                    //    TRobjViewModel.ErrorList.AddRange(errorListtemp);
                    //    errorListtemp.Clear();
                    //    TRobjViewModel.ErrorOccured = true;
                    //}

                    #endregion
                    foreach (var item in TRobjViewModel.TYPEA_DETAIL_PAGE22_SRM_52_List.ToList())
                    {
                        if (item.CH1 == null && item.CH2 == null && item.CH3 == null)
                        {
                            TRobjViewModel.TYPEA_DETAIL_PAGE22_SRM_52_List.Remove(item);
                        }
                    }
                    #endregion

                }

                if (naCheckArr[53] == 'Y')
                {
                    #region EFORM_TR_TYPEA_SUMMARY_PAGE22_SRM_53
                    errorListtemp.Clear();
                    errorListtemp = objCommonFunction.FetchTetsReportPDFFilledDataToModel(TRobjViewModel.TYPEA_SUMMARY_PAGE22_SRM_53, loadedForm);
                    TRobjViewModel.ErrorList.AddRange(errorListtemp);

                    TRobjViewModel.TYPEA_SUMMARY_PAGE22_SRM_53.MAIN_ITEM_ID = typeASummaryPage22SRM53AssignedModel.Model.MAIN_ITEM_ID;
                    TRobjViewModel.TYPEA_SUMMARY_PAGE22_SRM_53.SUBITEM_ID = typeASummaryPage22SRM53AssignedModel.Model.SUBITEM_ID;
                    TRobjViewModel.TYPEA_SUMMARY_PAGE22_SRM_53.TABLE_ID = typeASummaryPage22SRM53AssignedModel.Model.TABLE_ID;




                    #endregion

                    #region EFORM_TR_UCS_TEST_DETAIL_PAGE22_SRM_53
                    for (int i = 1; i <= 3; i++)
                    {
                        EFORM_TR_UCS_TEST_DETAIL_PAGE22_SRM_53 detailedItem = new EFORM_TR_UCS_TEST_DETAIL_PAGE22_SRM_53();
                        detailedItem.ROW_ID = i;
                        TRobjViewModel.UCS_TEST_DETAIL_PAGE22_SRM_53_List.Add(detailedItem);
                    }


                    foreach (var item in TRobjViewModel.UCS_TEST_DETAIL_PAGE22_SRM_53_List.ToList())
                    {
                        errorListtemp = objCommonFunction.FetchTetsReportPDFFilledDataToModel(item, loadedForm);
                        TRobjViewModel.ErrorList.AddRange(errorListtemp);
                    }

                    foreach (var item in TRobjViewModel.UCS_TEST_DETAIL_PAGE22_SRM_53_List.ToList())
                    {
                        if (item.CHAINAGE_22_2 == null && item.SAMPLE_WT_22_2 == null && item.DENSITY_22_2 == null && item.TESTING_DATE_22_2 == null && item.SAMPLE_VOL_22_2 == null && item.LOAD_KN_22_2 == null && item.COMPR_STREANGTH_22_2 == null && item.IS_STD_CONFIRM_22_2 == null)
                        {
                            TRobjViewModel.UCS_TEST_DETAIL_PAGE22_SRM_53_List.Remove(item);
                        }
                    }

                    #endregion



                }

                if (naCheckArr[54] == 'Y')
                {
                    #region EFORM_TR_TYPEA_SUMMARY_PAGE23_SRM_54
                    errorListtemp.Clear();
                    errorListtemp = objCommonFunction.FetchTetsReportPDFFilledDataToModel(TRobjViewModel.TYPEA_SUMMARY_PAGE23_SRM_54, loadedForm);
                    TRobjViewModel.ErrorList.AddRange(errorListtemp);

                    TRobjViewModel.TYPEA_SUMMARY_PAGE23_SRM_54.MAIN_ITEM_ID = typeASummaryPage22SRM54AssignedModel.Model.MAIN_ITEM_ID;
                    TRobjViewModel.TYPEA_SUMMARY_PAGE23_SRM_54.SUBITEM_ID = typeASummaryPage22SRM54AssignedModel.Model.SUBITEM_ID;
                    TRobjViewModel.TYPEA_SUMMARY_PAGE23_SRM_54.TABLE_ID = typeASummaryPage22SRM54AssignedModel.Model.TABLE_ID;




                    #endregion

                    #region EFORM_TR_TYPEA_DETAIL_PAGE22_SRM_54
                    dbContext.EFORM_TR_DETAIL_ITEM_MASTER.Where(m => m.MAIN_ITEM_ID == typeASummaryPage22SRM54AssignedModel.Model.MAIN_ITEM_ID && m.SUBITEM_ID == typeASummaryPage22SRM54AssignedModel.Model.SUBITEM_ID).Select(x => x.DETAIL_ITEM_ID).ToList().ForEach(itemID =>
                    {
                        TestReportModel.EFORM_TR_TYPEA_DETAIL_PAGE23_SRM_54 detailItem = new TestReportModel.EFORM_TR_TYPEA_DETAIL_PAGE23_SRM_54();
                        detailItem.DETAIL_ITEM_ID = itemID;
                        TRobjViewModel.TYPEA_DETAIL_PAGE23_SRM_54_List.Add(detailItem);
                    });

                    foreach (var item in TRobjViewModel.TYPEA_DETAIL_PAGE23_SRM_54_List.ToList())
                    {
                        errorListtemp.Clear();
                        errorListtemp = objCommonFunction.FetchTetsReportPDFFilledDataToModel(item, loadedForm);
                        TRobjViewModel.ErrorList.AddRange(errorListtemp);

                    }


                    foreach (var item in TRobjViewModel.TYPEA_DETAIL_PAGE23_SRM_54_List.ToList())
                    {
                        if (item.CH1 == null && item.CH2 == null && item.CH3 == null)
                        {
                            TRobjViewModel.TYPEA_DETAIL_PAGE23_SRM_54_List.Remove(item);
                        }
                    }
                    #endregion

                }

                #endregion

                if (TRobjViewModel.ErrorList.Count > 0)
                {
                    TRobjViewModel.ErrorOccured = true;
                }
                return TRobjViewModel;
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "eformcontroller.PdfDataToClassTRModel()");
                throw;
            }
            finally
            {
                loadedDocument.Close(true);
                dbContext.Dispose();
            }

        }



        public JsonResult isAllPdfAvail(string id)
        {
            PMGSYEntities dbContext = new PMGSYEntities();

            try
            {

                int eformId = Convert.ToInt32(id);
                EFORM_MASTER model = dbContext.EFORM_MASTER.Where(s => s.EFORM_ID == eformId).FirstOrDefault();

                var uploadpart1Model = dbContext.EFORM_PDF_UPLOAD_DETAIL.Where(s => s.EFORM_ID == eformId && s.USER_TYPE == "P").FirstOrDefault();
                var part1Fname = uploadpart1Model.FILE_NAME;
                var part1Path = ConfigurationManager.AppSettings["PIU_UPLOADED_PATH"].ToString() + "/" + model.ADMIN_SCHEDULE_YEAR.ToString() + "/" + model.ADMIN_SCHEDULE_MONTH.ToString();

                string physicalFullPathPart1 = Path.Combine(part1Path, part1Fname);
                FileInfo filePart1 = new FileInfo(physicalFullPathPart1);
                if (!filePart1.Exists)
                {
                    return Json(new { success = false, Message = "Part1 File not found" }, JsonRequestBehavior.AllowGet);
                }



                var uploadpart2Model = dbContext.EFORM_PDF_UPLOAD_DETAIL.Where(s => s.EFORM_ID == eformId && s.USER_TYPE == "Q").FirstOrDefault();
                var part2Fname = uploadpart2Model.FILE_NAME;
                var part2Path = ConfigurationManager.AppSettings["NQM_SQM_UPLOADED_PATH"].ToString() + "/" + model.ADMIN_SCHEDULE_YEAR.ToString() + "/" + model.ADMIN_SCHEDULE_MONTH.ToString();

                string physicalFullPathPart2 = Path.Combine(part2Path, part2Fname);
                FileInfo filePart2 = new FileInfo(physicalFullPathPart2);
                if (!filePart2.Exists)
                {
                    return Json(new { success = false, Message = "Part2 File not found" }, JsonRequestBehavior.AllowGet);
                }


                var uploadTRModel = dbContext.EFORM_TEST_REPORT_FILE.Where(s => s.EFORM_ID == eformId && s.IS_EFORM == "Y").FirstOrDefault();
                var TRFname = uploadTRModel.FILE_NAME;
                var TRPath = ConfigurationManager.AppSettings["TEST_REPORT_UPLOADED_PATH"].ToString();
                if (uploadTRModel.FILE_NAME.Contains('$'))
                {
                    TRPath = ConfigurationManager.AppSettings["TEST_REPORT_UPLOADED_PATH"].ToString() + "/" + model.ADMIN_SCHEDULE_YEAR.ToString() + "/" + model.ADMIN_SCHEDULE_MONTH.ToString();
                }

                string physicalFullPathTR = Path.Combine(TRPath, TRFname);
                FileInfo fileTR = new FileInfo(physicalFullPathTR);
                if (!fileTR.Exists)
                {
                    return Json(new { success = false, Message = "Test Report File not found" }, JsonRequestBehavior.AllowGet);
                }
                return Json(new { success = true, Message = "Test Report File found" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "eformcontroller.isAllPdfAvail()");
                return Json(new { success = false, Message = "Error occured while downloading pdf" }, JsonRequestBehavior.AllowGet);
            }
            finally
            {
                dbContext.Dispose();
            }

        }

        [RequiredAuthentication]
        public FileResult viewCombinedPdf12TR(string id)
        {
            PMGSYEntities dbContext = new PMGSYEntities();

            try
            {

                int eformId = Convert.ToInt32(id);
                EFORM_MASTER model = dbContext.EFORM_MASTER.Where(s => s.EFORM_ID == eformId).FirstOrDefault();

                var uploadpart1Model = dbContext.EFORM_PDF_UPLOAD_DETAIL.Where(s => s.EFORM_ID == eformId && s.USER_TYPE == "P").FirstOrDefault();
                var part1Fname = uploadpart1Model.FILE_NAME;
                var part1Path = ConfigurationManager.AppSettings["PIU_UPLOADED_PATH"].ToString() + "/" + model.ADMIN_SCHEDULE_YEAR.ToString() + "/" + model.ADMIN_SCHEDULE_MONTH.ToString();
                string physicalFullPathPart1 = Path.Combine(part1Path, part1Fname);

                var uploadpart2Model = dbContext.EFORM_PDF_UPLOAD_DETAIL.Where(s => s.EFORM_ID == eformId && s.USER_TYPE == "Q").FirstOrDefault();
                var part2Fname = uploadpart2Model.FILE_NAME;
                var part2Path = ConfigurationManager.AppSettings["NQM_SQM_UPLOADED_PATH"].ToString() + "/" + model.ADMIN_SCHEDULE_YEAR.ToString() + "/" + model.ADMIN_SCHEDULE_MONTH.ToString();
                string physicalFullPathPart2 = Path.Combine(part2Path, part2Fname);


                var uploadTRModel = dbContext.EFORM_TEST_REPORT_FILE.Where(s => s.EFORM_ID == eformId && s.IS_EFORM == "Y").FirstOrDefault();
                var TRFname = uploadTRModel.FILE_NAME;
                var TRPath = ConfigurationManager.AppSettings["TEST_REPORT_UPLOADED_PATH"].ToString();
                if (uploadTRModel.FILE_NAME.Contains('$'))
                {
                    TRPath = ConfigurationManager.AppSettings["TEST_REPORT_UPLOADED_PATH"].ToString() + "/" + model.ADMIN_SCHEDULE_YEAR.ToString() + "/" + model.ADMIN_SCHEDULE_MONTH.ToString();
                }
                string physicalFullPathTR = Path.Combine(TRPath, TRFname);



                byte[] FileBytesPart1 = System.IO.File.ReadAllBytes(physicalFullPathPart1);
                Stream streamPart1 = new MemoryStream(FileBytesPart1);

                byte[] FileBytesPart2 = System.IO.File.ReadAllBytes(physicalFullPathPart2);
                Stream streamPart2 = new MemoryStream(FileBytesPart2);

                byte[] FileBytesTR = System.IO.File.ReadAllBytes(physicalFullPathTR);
                Stream streamTR = new MemoryStream(FileBytesTR);


                Stream[] streams = { streamPart1, streamPart2, streamTR };

                PdfDocument finalDoc = new PdfDocument();
                PdfDocument.Merge(finalDoc, streams);
                string fileName = eformId + "_Combined_" + System.DateTime.Now.ToString("dd_MM_yyyy_HHmmss") + ".pdf";
                var tempPath = ConfigurationManager.AppSettings["TEMPORARY_FILES_PATH"].ToString();
                if (!Directory.Exists(tempPath))
                {
                    Directory.CreateDirectory(tempPath);
                }
                var fullFilePath = Path.Combine(tempPath, fileName);
                finalDoc.Save(fullFilePath);
                byte[] FileBytes = System.IO.File.ReadAllBytes(fullFilePath);
                try
                {
                    string temp_Path = ConfigurationManager.AppSettings["TEMPORARY_FILES_PATH"];
                    System.IO.DirectoryInfo di = new DirectoryInfo(temp_Path);

                    foreach (FileInfo file in di.GetFiles())
                    {
                        try
                        {
                            file.Delete();
                        }
                        catch (Exception ex)
                        {
                            ErrorLog.LogError(ex, "eformcontroller.viewCombinedPdf12TR()");
                        }

                    }

                }
                catch (Exception ex)
                {
                    ErrorLog.LogError(ex, "eformcontroller.viewCombinedPdf12TR()");

                }
                return File(FileBytes, "application/pdf", fileName);

            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "eformController.viewCombinedPdf12TR()");
                string error_File = ConfigurationManager.AppSettings["ERROR_FILE"].ToString();
                string Errorfilename = "Error.pdf";
                byte[] FileBytes = System.IO.File.ReadAllBytes(error_File);
                return File(FileBytes, "application/pdf", Errorfilename);
            }
            finally
            {
                dbContext.Dispose();
            }

        }

        #endregion
    }
}
