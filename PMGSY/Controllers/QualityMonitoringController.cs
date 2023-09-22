#region File Header
/*
        * Project Id    :
        * Project Name  :   OMMAS II
        * Name          :   QualityMonitoringController.cs        
        * Description   :   Creation of Schedules, Fill Observations, Uploading Images & ATRs, Correcting Observations, Creation of Monitors etc.
        * Author        :   Shyam Yadav 
        * Creation Date :   10/Jun/2013
 **/
#endregion


using Mvc.Mailer;
using PMGSY.BAL.Proposal;
using PMGSY.BAL.QualityMonitoring;
using PMGSY.Common;
using PMGSY.Extensions;
using PMGSY.Mailers;
using PMGSY.Models;
using PMGSY.Models.Common;
using PMGSY.Models.Master;
using PMGSY.Models.Proposal;
using PMGSY.Models.QualityMonitoring;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity.Validation;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using PMGSY.BAL.QualityMonitoringHelpDesk;
using PMGSY.DAL.QualityMonitoring;
using System.Transactions;
using Syncfusion.XlsIO;
using System.Net;

namespace PMGSY.Controllers
{
    [RequiredAuthentication]
    [RequiredAuthorization]
    public class QualityMonitoringController : Controller
    {
        QualityMonitoringHelpDeskBAL qualityMonitoringHelpDeskBAL = null;
        public QualityMonitoringController()
        {
            PMGSYSession.Current.ModuleName = "Quality Monitoring";
        }

        private IQualityMonitoringBAL qualityBAL;
        private PMGSYEntities dbContext;
        Dictionary<string, string> decryptedParameters = null;

        #region Layouts

        /// <summary>
        /// Get to render Quality Layout Page
        /// </summary>
        /// <returns></returns>
        [Audit]
        public ActionResult QualityLayout()
        {
            if (PMGSYSession.Current.RoleCode == 8 || PMGSYSession.Current.RoleCode == 69 || PMGSYSession.Current.RoleCode == 48)                                             // SQC
            {
                return View("QualityLayoutSQC");
            }

            else if (PMGSYSession.Current.RoleCode == 22 || PMGSYSession.Current.RoleCode == 38 || PMGSYSession.Current.RoleCode == 54)  // PIU or PIUOA or PIURCPLWE
            {
                return View("QualityLayoutPIU");
            }
            else if (PMGSYSession.Current.RoleCode == 6 || PMGSYSession.Current.RoleCode == 7)  // NQM OR SQM
            {
                return View("QualityMonitorsLayout");
            }
            else if (PMGSYSession.Current.RoleCode == 9)                                        // CQC
            {
                //return View("CQCLayout");
                return View("QualityLayout");
            }

            else
                //return View("QualityLayout");                                                   // For CQCAdmin
                return View("QualityLayoutPIU");
        }

        [Audit]
        public ActionResult QMATROADetails() // For SQCOA
        {

            QMFilterViewModel qmFilterModel = new QMFilterViewModel();
            CommonFunctions objCommonFunctions = new CommonFunctions();

            qmFilterModel.MAST_STATE_CODE = 0;
            qmFilterModel.ADMIN_QM_CODE = 0;
            qmFilterModel.ATR_STATUS = "0";
            qmFilterModel.ROAD_STATUS = "A";

            qmFilterModel.FROM_MONTH = DateTime.Now.Month;
            qmFilterModel.FROM_YEAR = DateTime.Now.Year;
            qmFilterModel.TO_MONTH = DateTime.Now.Month;
            qmFilterModel.TO_YEAR = DateTime.Now.Year;

            if (PMGSYSession.Current.RoleCode == 5)  //CQC
            {
                qmFilterModel.STATES = objCommonFunctions.PopulateStates(false);
                qmFilterModel.STATES.Insert(0, (new SelectListItem { Text = "All", Value = "0", Selected = true }));

                qmFilterModel.MONITORS = objCommonFunctions.PopulateMonitors("false", "0", 0); //Purposely taken String "false" as argument
            }
            else if (PMGSYSession.Current.RoleCode == 8 || PMGSYSession.Current.RoleCode == 48)  //SQC
            {
                qmFilterModel.MAST_STATE_CODE = PMGSYSession.Current.StateCode;
                qmFilterModel.MONITORS = objCommonFunctions.PopulateMonitors("false", "I", PMGSYSession.Current.StateCode); //Purposely taken String "false" as argument
            }
            else if (PMGSYSession.Current.RoleCode == 22 || PMGSYSession.Current.RoleCode == 38)  //PIU
            {
                qmFilterModel.MAST_STATE_CODE = PMGSYSession.Current.StateCode;
                qmFilterModel.MONITORS = objCommonFunctions.PopulateMonitorsDistrictWise("false", "I", PMGSYSession.Current.StateCode, PMGSYSession.Current.DistrictCode); //Purposely taken String "false" as argument
            }

            qmFilterModel.FROM_MONTHS_LIST = objCommonFunctions.PopulateMonths(false);
            qmFilterModel.FROM_YEARS_LIST = objCommonFunctions.PopulateYears(false).Where(a => a.Value != "0").ToList();
            qmFilterModel.TO_MONTHS_LIST = objCommonFunctions.PopulateMonths(false);
            qmFilterModel.TO_YEARS_LIST = objCommonFunctions.PopulateYears(false).Where(a => a.Value != "0").ToList();
            qmFilterModel.ATR_STATUS_LIST = objCommonFunctions.QualityATRStatus();
            qmFilterModel.ROAD_STATUS_LIST = objCommonFunctions.PopulateRoadStatus();
            return View(qmFilterModel);
        }

        /// <summary>
        /// View for ATR Details in  HTML Report
        /// </summary>
        /// <param name="formCollection"></param>
        /// <returns></returns>
        public ActionResult QmAtrOaHtmlList(FormCollection formCollection)
        {
            QualityMonitoringBAL qmBAL = new QualityMonitoringBAL();
            QMATRDetailsModel atrDetailsModel = new QMATRDetailsModel();
            atrDetailsModel.ATR_LIST = new List<QMATRModel>();
            atrDetailsModel.OBS_LIST = new List<QMObsATRModel>();
            dbContext = new PMGSYEntities();
            try
            {
                atrDetailsModel.OBS_ATR_LIST = qmBAL.ATRDetailsBAL(Convert.ToInt32(formCollection["MAST_STATE_CODE"]), Convert.ToInt32(formCollection["ADMIN_QM_CODE"]),
                                                                Convert.ToInt32(formCollection["FROM_MONTH"]), Convert.ToInt32(formCollection["FROM_YEAR"]),
                                                                Convert.ToInt32(formCollection["TO_MONTH"]), Convert.ToInt32(formCollection["TO_YEAR"]),
                                                                formCollection["ATR_STATUS"], formCollection["ROAD_STATUS"], 0);

                var distinctObsList = (from obs in atrDetailsModel.OBS_ATR_LIST
                                       select new
                                       {
                                           obs.QM_OBSERVATION_ID,
                                           obs.MONITOR_NAME,
                                           obs.STATE_NAME,
                                           obs.DISTRICT_NAME,
                                           obs.BLOCK_NAME,
                                           obs.IMS_PACKAGE_ID,
                                           obs.IMS_YEAR,
                                           obs.IMS_ROAD_NAME,
                                           obs.QM_INSPECTED_START_CHAINAGE,
                                           obs.QM_INSPECTED_END_CHAINAGE,
                                           obs.QM_INSPECTION_DATE,
                                           obs.IMS_ISCOMPLETED,
                                           obs.OVERALL_GRADE,
                                           obs.NO_OF_PHOTO_UPLOADED,
                                           obs.QM_ATR_STATUS,
                                           obs.ADMIN_IS_ENQUIRY,
                                           obs.IMS_PROPOSAL_TYPE
                                       }).Distinct().ToList();

                foreach (var item in distinctObsList)
                {
                    QMObsATRModel obsModel = new QMObsATRModel();
                    obsModel.QM_OBSERVATION_ID = item.QM_OBSERVATION_ID;
                    obsModel.MONITOR_NAME = item.MONITOR_NAME;
                    obsModel.STATE_NAME = item.STATE_NAME;
                    obsModel.DISTRICT_NAME = item.DISTRICT_NAME;
                    obsModel.BLOCK_NAME = item.BLOCK_NAME;
                    obsModel.IMS_PACKAGE_ID = item.IMS_PACKAGE_ID;
                    obsModel.IMS_YEAR = item.IMS_YEAR.ToString() + "-" + (item.IMS_YEAR + 1).ToString();
                    obsModel.IMS_ROAD_NAME = item.IMS_ROAD_NAME;
                    obsModel.QM_INSPECTED_START_CHAINAGE = item.QM_INSPECTED_START_CHAINAGE;
                    obsModel.QM_INSPECTED_END_CHAINAGE = item.QM_INSPECTED_END_CHAINAGE;
                    obsModel.QM_INSPECTION_DATE = item.QM_INSPECTION_DATE;
                    obsModel.IMS_ISCOMPLETED = item.IMS_ISCOMPLETED;
                    obsModel.OVERALL_GRADE = item.OVERALL_GRADE;
                    obsModel.NO_OF_PHOTO_UPLOADED = item.NO_OF_PHOTO_UPLOADED;
                    obsModel.QM_ATR_STATUS = item.QM_ATR_STATUS;
                    obsModel.ADMIN_IS_ENQUIRY = item.ADMIN_IS_ENQUIRY.Equals("Y") ? "Yes" : "No";
                    obsModel.IMS_PROPOSAL_TYPE = item.IMS_PROPOSAL_TYPE.Equals("P") ? "Road" : "LSB";
                    obsModel.SHOW_OBS_LINK = "<a href='#' title='Click here to view details' class='ui-icon ui-icon-zoomin ui-align-center' onClick='ShowATRObsDetails(\"" + item.QM_OBSERVATION_ID.ToString().Trim() + "\"); return false;'>View</a>";

                    atrDetailsModel.OBS_LIST.Add(obsModel);
                }

                //int index = 0;
                foreach (var item in atrDetailsModel.OBS_ATR_LIST)
                {
                    // Populate & Add ATR Details for each Observation Id
                    QMATRModel atrModel = new QMATRModel();
                    atrModel.QM_OBSERVATION_ID = item.QM_OBSERVATION_ID;
                    atrModel.QM_ATR_ID = item.QM_ATR_ID;
                    atrModel.ATR_ENTRY_DATE = item.ATR_ENTRY_DATE;
                    atrModel.ATR_REGRADE_STATUS = item.ATR_REGRADE_STATUS.Trim().Equals("N") ? "No" : "Yes";
                    atrModel.ATR_REGRADE_REMARKS = item.ATR_REGRADE_REMARKS;
                    atrModel.ATR_REGRADE_DATE = item.ATR_REGRADE_DATE;
                    atrModel.ATR_IS_DELETED = item.ATR_IS_DELETED;
                    atrModel.QM_ATR_STATUS = item.QM_ATR_STATUS;

                    atrModel.IS_SUBMITTED = item.QM_ATR_STATUS.Trim().Equals("N") ? "No" : "Yes";
                    atrModel.ATR_UPLOAD_VIEW_LINK =
                        (PMGSYSession.Current.RoleCode == 22 || PMGSYSession.Current.RoleCode == 38)//for PIU view only
                        ? (item.QM_ATR_ID != null)
                                            ? "<a href='#' title='Click here to view uploaded ATR' class='ui-icon ui-icon-zoomin ui-align-center' onClick='DownloadATR(\"" + URLEncrypt.EncryptParameters(new string[] { item.QM_ATR_ID + ".pdf" + "$" + item.QM_ATR_ID }) + "\"); return false;'>Download</a>"
                                            : "<a href='#' class='ui-icon ui-icon-locked ui-align-center' />"
                        : (item.QM_ATR_STATUS.Trim().Equals("N")) // Upload/View
                                        ? "<a href='#' title='Click here to upload ATR' class='ui-icon ui-icon-plusthick ui-align-center' onClick='UploadATR(\"" + item.QM_OBSERVATION_ID.ToString().Trim() + "$" + Convert.ToString(item.QM_ATR_ID).Trim() + "\"); return false;'>Upload</a>"
                                        : (item.QM_ATR_ID != null)
                                            ? "<a href='#' title='Click here to view uploaded ATR' class='ui-icon ui-icon-zoomin ui-align-center' onClick='DownloadATR(\"" + URLEncrypt.EncryptParameters(new string[] { item.QM_ATR_ID + ".pdf" + "$" + item.QM_ATR_ID }) + "\"); return false;'>Download</a>"
                                            : "<a href='#' class='ui-icon ui-icon-locked ui-align-center' />";

                    atrModel.ATR_ACCEPTANCE_LINK =
                         (PMGSYSession.Current.RoleCode == 22 || PMGSYSession.Current.RoleCode == 38)//for PIU view only
                         ? item.ATR_REGRADE_STATUS.Trim().Equals("A")
                                        ? "Accepted"
                                        : item.ATR_REGRADE_STATUS.Trim().Equals("R")
                                            ? "Rejected"
                                            : ""
                         : item.ATR_REGRADE_STATUS.Trim().Equals("A")     // Acceptance
                                        ? "Accepted"
                                        : item.ATR_REGRADE_STATUS.Trim().Equals("R") // if any of the ATR against Obs Id is Accepted then dont provide link to upload.
                                            ? item.QM_ATR_STATUS.Equals("A")
                                                ? "Rejected"
                                                : "Rejected" + //If Rejected atr is last against Observation, then append + sign to upload again
                                                    (atrModel.QM_ATR_ID == (dbContext.QUALITY_ATR_FILE.Where(c => c.QM_OBSERVATION_ID == atrModel.QM_OBSERVATION_ID).Select(c => c.QM_ATR_ID).Max())
                                                    ? "<a href='#' title='Click here to upload ATR' class='ui-icon ui-icon-plusthick ui-align-center' onClick='UploadATR(\"" + item.QM_OBSERVATION_ID.ToString().Trim() + "$" + Convert.ToString(item.QM_ATR_ID).Trim() + "\"); return false;'>Upload</a>"
                                                    : "")
                                            : "";

                    atrModel.ATR_REGRADE_LINK =
                        (PMGSYSession.Current.RoleCode == 22 || PMGSYSession.Current.RoleCode == 38)//for PIU view only
                        ? (item.ATR_REGRADE_STATUS.Trim().Equals("A"))
                            ? "<a href='#' title='Click here to see observation details against ATR' class='ui-icon ui-icon-zoomin ui-align-center' onClick='ShowATRGradingDetails(\"" + item.QM_OBSERVATION_ID.ToString().Trim() + "\"); return false;'>Regrade</a>"
                            : "<a href='#' class='ui-icon ui-icon-locked ui-align-center' />"
                        : (item.ATR_REGRADE_STATUS.Trim().Equals("U") && item.ATR_IS_DELETED.Equals("N")) // Regrade, for recent entry only
                                        ? "<a href='#' title='Click here to regrade ATR' class='ui-icon ui-icon-plusthick ui-align-center' onClick='RegradeATR(\"" + item.QM_OBSERVATION_ID.ToString().Trim() + "\"); return false;'>Regrade</a>"
                                            : (item.ATR_REGRADE_STATUS.Trim().Equals("A"))
                                            ? "<a href='#' title='Click here to see observation details against ATR' class='ui-icon ui-icon-zoomin ui-align-center' onClick='ShowATRGradingDetails(\"" + item.QM_OBSERVATION_ID.ToString().Trim() + "\"); return false;'>Regrade</a>"
                                                : "<a href='#' class='ui-icon ui-icon-locked ui-align-center' />";

                    atrModel.ATR_DELETE_LINK =
                         (PMGSYSession.Current.RoleCode == 22 || PMGSYSession.Current.RoleCode == 38)//for PIU view only
                         ? "<a href='#' class='ui-icon ui-icon-locked ui-align-center' />"
                         : (item.QM_ATR_ID != null && item.QM_ATR_STATUS.Trim() != "A" && item.QM_ATR_STATUS.Trim() != "N" && (item.QM_ATR_ID == dbContext.QUALITY_ATR_FILE.Where(c => c.QM_OBSERVATION_ID == item.QM_OBSERVATION_ID).Select(c => c.QM_ATR_ID).Max()))
                                ? "<a href='#' title='Click here to delete ATR details' class='ui-icon ui-icon-trash ui-align-center' onClick='DeleteATR(\"" +
                                    item.QM_OBSERVATION_ID.ToString().Trim() + "\",\"" + dbContext.QUALITY_ATR_FILE.Where(a => a.QM_OBSERVATION_ID == item.QM_OBSERVATION_ID && a.ATR_IS_DELETED == "N").Select(a => a.QM_ATR_ID).FirstOrDefault()
                                    + "\"); return false;'>Delete</a>"
                                : "<a href='#' class='ui-icon ui-icon-locked ui-align-center' />";

                    atrDetailsModel.ATR_LIST.Add(atrModel);

                    //index++; // increment index for each record.
                }

                return View(atrDetailsModel);
            }
            catch
            {
                atrDetailsModel.ERROR = "Error occurred while processing your request";
                return View(atrDetailsModel); //return model as null
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        /// <summary>
        /// Layout for SQC
        /// </summary>
        /// <returns></returns>
        [Audit]
        public ActionResult QualityLayoutSQC()
        {
            if (PMGSYSession.Current.RoleCode == 5)
            {
                return View("QualityLayout2TierCQC");

            }
            else if (PMGSYSession.Current.RoleCode == 8 || PMGSYSession.Current.RoleCode == 69 || PMGSYSession.Current.RoleCode == 48)
            {
                return View("QualityLayout");
            }
            else if (PMGSYSession.Current.RoleCode == 22 || PMGSYSession.Current.RoleCode == 38)
            {
                return View("QualityLayoutPIU");
            }
            else
                return null;
        }

        /// <summary>
        /// Layout for Monitors
        /// </summary>
        /// <returns></returns>
        [Audit]
        public ActionResult QualityMonitorsLayout()
        {
            return View();
        }

        /// <summary>
        /// Layout for 2tier in PIU 
        /// </summary>
        /// <returns></returns>
        [Audit]
        public ActionResult QM2TierPIU()
        {
            return View();
        }

        /// <summary>
        /// Layout for 1tier in PIU 
        /// </summary>
        /// <returns></returns>
        [Audit]
        public ActionResult QM1TierPIU()
        {
            return View();
        }

        /// <summary>
        /// Layout for 3tier in PIU 
        /// </summary>
        /// <returns></returns>
        [Audit]
        public ActionResult QM3TierPIU()
        {
            return View();
        }

        /// <summary>
        /// Layout for CQC Login
        /// </summary>
        /// <returns></returns>
        [Audit]
        public ActionResult CQCLayout()
        {
            return View();
        }

        /// <summary>
        /// Layout for 2tier in CQC Login 
        /// </summary>
        /// <returns></returns>
        [Audit]
        public ActionResult QualityLayout2TierCQC()
        {
            return View();
        }
        #endregion

        #region QCR Part-I PDF by Srishti and Vikki

        [HttpPost]
        public ActionResult PopulateDistrictsbyStateCode(int stateCode)
        {
            try
            {
                CommonFunctions objCommon = new CommonFunctions();
                List<SelectListItem> lstDistrict = new List<SelectListItem>();
                lstDistrict = objCommon.PopulateDistrict(stateCode, false);
                return Json(lstDistrict);
            }
            catch
            {
                return Json(new { string.Empty });
            }
        }

        [HttpGet]
        public ActionResult UploadQCRLayout()
        {

            try
            {

                PMGSYEntities dbContext = new PMGSYEntities();
                CommonFunctions objCommon = new CommonFunctions();

                if (PMGSYSession.Current.RoleCode == 8 || PMGSYSession.Current.RoleCode == 69)                                             // SQC
                {
                    UploadQCRModel uploadQCR = new UploadQCRModel();

                    // String StateName = dbContext.MASTER_STATE.Where(x => x.MAST_STATE_CODE == PMGSYSession.Current.StateCode).Select(x => x.MAST_STATE_NAME).FirstOrDefault();
                    //  uploadQCR.StateName = StateName;
                    //   uploadQCR.lstStates = objCommon.PopulateStates();
                    uploadQCR.StateName = PMGSYSession.Current.StateName; //dbContext.MASTER_STATE.Where(x => x.MAST_STATE_CODE == PMGSYSession.Current.StateCode).Select()
                    uploadQCR.stateCode = PMGSYSession.Current.StateCode;
                    uploadQCR.lstDistricts = new List<SelectListItem>();
                    //  uploadQCR.lstDistricts.Insert(0, new SelectListItem() { Text = "Select District", Value = "-1" });
                    uploadQCR.lstDistricts = objCommon.PopulateDistrict(uploadQCR.stateCode, false);
                    uploadQCR.Years = new List<SelectListItem>();
                    uploadQCR.Years = PopulateYear(0, true, true);
                    return View(uploadQCR);
                }
                // Added on 23-02-2022 by Srishti Tyagi 
                else if (PMGSYSession.Current.RoleCode == 9)
                {
                    UploadQCRModel uploadQCR = new UploadQCRModel();

                    uploadQCR.lstStates = objCommon.PopulateStates();
                    uploadQCR.lstDistricts = new List<SelectListItem>();
                    uploadQCR.lstDistricts.Insert(0, new SelectListItem() { Text = "Select District", Value = "-1" });
                    //     uploadQCR.lstDistricts = objCommon.PopulateDistrict(, false);
                    uploadQCR.Years = new List<SelectListItem>();
                    uploadQCR.Years = PopulateYear(0, true, true);
                    return View(uploadQCR);

                }
                else
                    return View("QualityLayoutPIU");

            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "UploadQCRLayoutModel()");
                return null;
            }
        }

        public ActionResult AddQCR(string idtemp)
        {

            try
            {
                string id1 = "0";
                Int32 id = 0;
                if (idtemp != null)
                {
                    string[] encParam = idtemp.Split('/');
                    decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { encParam[0], encParam[1], encParam[2] });
                    if (decryptedParameters.Count > 0)
                    {
                        id1 = decryptedParameters["imsRoadID"];

                    }

                }

                id = Convert.ToInt32(id1);
                PMGSYEntities dbContext = new PMGSYEntities();
                AddUploadQCRDetailsModel model = new AddUploadQCRDetailsModel();
                DateTime currentDate = DateTime.Now;
                model.UploadedDate = currentDate.ToString("dd/MM/yyyy");


                Int32 stat = (dbContext.QM_QCR_DETAILS.Where(s => s.IMS_PR_ROAD_CODE == id).FirstOrDefault() == null ? 0 : 1);
                if (stat > 0)
                {
                    model.IS_LATEST = 'Y';
                }
                else
                {
                    model.IS_LATEST = 'N';
                }

                // Added on 09-02-2022 by Srishti Tyagi
                List<SelectListItem> items = new List<SelectListItem>();
                items.Add(new SelectListItem() { Text = "A", Value = "A" });
                items.Add(new SelectListItem() { Text = "B", Value = "B" });
                items.Add(new SelectListItem() { Text = "C", Value = "C" });
                items.Insert(0, (new SelectListItem { Text = "Select Grade", Value = String.Empty, Selected = true }));

                // items.Insert(items.Count, new SelectListItem { Text = "A", Value = items.Count.ToString() });
                // AddUploadQCRDetailsModel model1 = new AddUploadQCRDetailsModel();
                // model.awardGrade = "0";
                model.gradeSE = items;
                model.gradeSQC = items;

                // end changes


                return View(model);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "AddQCR");
                return null;
            }



        }


        [HttpPost]
        //    [ValidateAntiForgeryToken]
        public ActionResult AddQCRdDetails(AddUploadQCRDetailsModel model)
        {
            int roadCode = 0;
            bool status = false;
            string isValidMsg = String.Empty;
            String message = " QCR details not added";
            IQualityMonitoringBAL objBAL = new QualityMonitoringBAL();
            CommonFunctions commonFunction = new CommonFunctions();
            try
            {
                string[] encParam = model.IMS_PR_ROAD_CODE.Split('/');
                decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { encParam[0], encParam[1], encParam[2] });



                if (Request.Files.Count == 0)
                {
                    return Json(new { success = false, file = false, message = "No file selected. Please select file" });
                }
                else
                {
                    HttpPostedFileBase postedBgFile = Request.Files[0];
                    int maxSize = 1024 * 1024 * 15;

                    if (postedBgFile.FileName.Substring(postedBgFile.FileName.LastIndexOf('.') + 1).Trim().ToLower() != "pdf")
                    {
                        return Json(new { success = false, file = false, message = "Invalid file. Please upload only pdf files." });
                    }

                    if (postedBgFile.ContentLength > maxSize)
                    {
                        return Json(new { success = false, file = false, message = "Invalid file size. Please upload file upto 15 MB." });
                    }

                }

                // Added on 10-02-2022 by Srishti Tyagi
                if (model.awardGradeSE == null)
                    return Json(new { success = false, message = "Please enter Grading by SE. " });
                if (model.awardGradeSQC == null)
                    return Json(new { success = false, message = "Please enter Grading by SQC. " });

                if (decryptedParameters.Count > 0)
                {
                    roadCode = Convert.ToInt32(decryptedParameters["IMS_road_code"]);
                    model.IMS_PR_ROAD_CODE = roadCode.ToString();

                    TryValidateModel(model);
                    if (ModelState.IsValid)
                    {
                        status = objBAL.AddQCRdDetails(model, out isValidMsg);

                        return Json(new { success = status, message = isValidMsg });
                    }

                }

                return Json(new { success = status, message = message });

            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "AddBankGuaranteeDetails");
                return Json(new { success = status, message = message });
            }
        }




        public FileResult GetQCRPdf(string id)
        {
            try
            {
                String FileName = String.Empty;
                string[] encParam = id.Split('/');
                string year = string.Empty;
                decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { encParam[0], encParam[1], encParam[2] });
                if (decryptedParameters.Count > 0)
                {
                    FileName = decryptedParameters["QCRFile"];
                }

                year = FileName.Split('_')[0];

                string path = ConfigurationManager.AppSettings["QCR_DETAILS_PATH"].ToString();

                string fullPath = Path.Combine(path, year, FileName);
                byte[] FileBytes = System.IO.File.ReadAllBytes(fullPath);
                return File(FileBytes, "application/pdf");

            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "GetQCRPdf()");
                return null;
            }

        }






        [Audit]
        public ActionResult GetExecutionProgressList(int? page, int? rows, string sidx, string sord)
        {
            try
            {
                int yearCode = 0;
                int districtCode = 0;

                long totalRecords = 0;
                IQualityMonitoringBAL objBAL = new QualityMonitoringBAL();

                using (CommonFunctions commonFunction = new CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new GridParams(page, rows, sidx, sord, Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                    {
                        return null;
                    }
                }

                if (!string.IsNullOrEmpty(Request.Params["yearCode"]))
                {
                    yearCode = Convert.ToInt32(Request.Params["yearCode"]);
                }

                if (!string.IsNullOrEmpty(Request.Params["districtCode"]))
                {
                    districtCode = Convert.ToInt32(Request.Params["districtCode"]);
                }

                // AddUploadQCRDetailsModel model = new AddUploadQCRDetailsModel();                  
                //model.grade.Insert(0, new SelectListItem() { Text = "A", Value = "0" });
                //model.grade.Insert(1, new SelectListItem() { Text = "B", Value = "1" });
                //model.grade.Insert(2, new SelectListItem() { Text = "C", Value = "2" });
                // model.grade.Add(new SelectListItem() { Text = "A", Value = "0" });
                // model.grade.Add(new SelectListItem() { Text = "B", Value = "1" });
                // model.grade.Add(new SelectListItem() { Text = "C", Value = "2" });

                // Added on 31-01-2022 by Srishti Tyagi
                //List<SelectListItem> items = new List<SelectListItem>();
                //items.Add(new SelectListItem() { Text = "A", Value = "0" });
                //items.Add(new SelectListItem() { Text = "B", Value = "1" });
                //items.Add(new SelectListItem() { Text = "C", Value = "2" });

                //  items.Insert(items.Count, new SelectListItem { Text = "A", Value = items.Count.ToString() });
                //   AddUploadQCRDetailsModel model = new AddUploadQCRDetailsModel();
                // model.awardGrade = "0";
                // model.grade = items;

                // Changes end 

                var jsonData = new
                {
                    rows = objBAL.GetExecutionList(yearCode, districtCode, Convert.ToInt32(page) - 1, Convert.ToInt32(rows), sidx, sord, out totalRecords),
                    total = totalRecords <= Convert.ToInt32(rows) ? 1 : totalRecords / Convert.ToInt32(rows) + 1,
                    page = Convert.ToInt32(page),
                    records = totalRecords,
                };

                return Json(jsonData, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "GetExecutionProgressList()");
                return null;
            }
        }


        // Added on 01-02-2022 by Srishti Tyagi
        //[HttpPost]
        //public ActionResult PopulateAwardGrade()
        //{
        //    try
        //    {
        //        List<SelectListItem> items = new List<SelectListItem>();
        //        items.Add(new SelectListItem() { Text = "A", Value = "0" });
        //        items.Add(new SelectListItem() { Text = "B", Value = "1" });
        //        items.Add(new SelectListItem() { Text = "C", Value = "2" });

        //        // items.Insert(items.Count, new SelectListItem { Text = "A", Value = items.Count.ToString() });
        //        AddUploadQCRDetailsModel model = new AddUploadQCRDetailsModel();
        //        // model.awardGrade = "0";
        //        model.gradeSE = items;
        //        model.gradeSQC = items;

        //        return Json(model.grade);
        //    }
        //    catch
        //    {
        //        return Json(new { string.Empty });
        //    }
        //}  

        // Added on 04-02-2022 by Srishti Tyagi
        [Audit]
        public ActionResult GetQCRDetailsList(string idtemp, int? page, int? rows, string sidx, string sord)
        {


            try
            {

                string id1 = "0";
                Int32 id = 0;
                if (idtemp != null)
                {
                    string[] encParam = idtemp.Split('/');
                    decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { encParam[0], encParam[1], encParam[2] });
                    if (decryptedParameters.Count > 0)
                    {
                        id1 = decryptedParameters["imsRoadID"];

                    }

                }

                id = Convert.ToInt32(id1);

                long totalRecords = 0;

                IQualityMonitoringBAL objBAL = new QualityMonitoringBAL();

                using (CommonFunctions commonFunction = new CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new GridParams(page, rows, sidx, sord, Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                    {
                        return null;
                    }
                }


                var jsonData = new
                {
                    rows = objBAL.GetQCRList(id, Convert.ToInt32(page) - 1, Convert.ToInt32(rows), sidx, sord, out totalRecords),
                    total = totalRecords <= Convert.ToInt32(rows) ? 1 : totalRecords / Convert.ToInt32(rows) + 1,
                    page = Convert.ToInt32(page),
                    records = totalRecords,
                };

                return Json(jsonData, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "GetQCRDetailsList()");
                return null;
            }
        }

        // Added on 07-02-2022 by Srishti Tyagi
        [HttpGet]
        public ActionResult DeleteQCRPdf(string id)
        {
            try
            {
                PMGSYEntities dbContext = new PMGSYEntities();
                QM_QCR_DETAILS model = new QM_QCR_DETAILS();
                string message = string.Empty;
                bool status = false;


                String FileName = String.Empty;
                String qcrId = String.Empty;
                int qcrid = 0;
                string[] encParam = id.Split('/');
                string year = string.Empty;
                decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { encParam[0], encParam[1], encParam[2] });
                if (decryptedParameters.Count > 0)
                {
                    qcrId = decryptedParameters["QCRFile"];
                }

                qcrid = Convert.ToInt32(qcrId);
                model = dbContext.QM_QCR_DETAILS.Where(x => x.QCR_ID == qcrid).FirstOrDefault<QM_QCR_DETAILS>();
                FileName = model.FILE_NAME;
                year = FileName.Split('_')[0];

                string path = ConfigurationManager.AppSettings["QCR_DETAILS_PATH"].ToString();

                string fullPath = Path.Combine(path, year, FileName);

                FileInfo file = new FileInfo(fullPath);
                if (file.Exists)
                {
                    file.Delete();

                }

                dbContext.QM_QCR_DETAILS.Remove(dbContext.QM_QCR_DETAILS.Where(s => s.FILE_NAME == FileName).FirstOrDefault());
                dbContext.SaveChanges();

                message = "File deleted successfully.";
                status = true;

                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "DeleteQCRPdf()");
                return null;
            }

        }

        // Added on 08-02-2022 by Srishti Tyagi
        [HttpGet]
        public ActionResult FinalizeDetails(string id)
        {
            try
            {
                PMGSYEntities dbContext = new PMGSYEntities();
                QM_QCR_DETAILS model = new QM_QCR_DETAILS();
                string message = string.Empty;
                bool status = false;

                //   String FileName = String.Empty;
                String qcrId = String.Empty;
                int qcrid = 0;
                string[] encParam = id.Split('/');
                decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { encParam[0], encParam[1], encParam[2] });
                if (decryptedParameters.Count > 0)
                {
                    qcrId = decryptedParameters["QCRFile"];
                }

                qcrid = Convert.ToInt32(qcrId);

                model = dbContext.QM_QCR_DETAILS.Where(x => x.QCR_ID == qcrid).FirstOrDefault<QM_QCR_DETAILS>();
                model.IS_FINALIZE = "Y";
                dbContext.Entry(model).State = System.Data.Entity.EntityState.Modified;
                dbContext.SaveChanges();

                message = "Data finalized successfully.";
                status = true;

                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "FinalizeDetails()");
                return null;
            }

        }


        #endregion

        #region QCR Management Report for CQC

        [HttpGet]
        public ActionResult QCRReportView()
        {

            try
            {
                CommonFunctions objCommon = new CommonFunctions();
                QCRReportViewModel modelView = new QCRReportViewModel();

                modelView.lstStates = objCommon.PopulateStates(false);
                modelView.lstDistricts = new List<SelectListItem>();
                modelView.lstDistricts.Insert(0, new SelectListItem() { Text = "All District", Value = "0" });
                //     uploadQCR.lstDistricts = objCommon.PopulateDistrict(, false);
                modelView.Years = new List<SelectListItem>();
                modelView.Years = PopulateYear(0, true, true);
                modelView.lstScheme = objCommon.PopulateScheme();
                //modelView.lstScheme.Insert(0, new SelectListItem() { Text = "Select scheme", Value = "1" });
                return View(modelView);

            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "QCRReportView()");
                return null;
            }
        }

        public ActionResult PopulateAllDistrictsbyStateCode(int stateCode)
        {
            try
            {
                CommonFunctions objCommon = new CommonFunctions();
                List<SelectListItem> lstDistrict = new List<SelectListItem>();
                lstDistrict = objCommon.PopulateDistrict(stateCode, true);
                return Json(lstDistrict);
            }
            catch
            {
                return Json(new { string.Empty });
            }
        }

        public ActionResult ViewQCRManagementReport()
        {
            //int stateCode, districtCode, year, scheme;

            if (!string.IsNullOrEmpty(Request.Params["stateCode"]))
            {
                ViewBag.stateCode = Convert.ToInt32(Request.Params["stateCode"]);
            }

            if (!string.IsNullOrEmpty(Request.Params["districtCode"]))
            {
                ViewBag.districtCode = (Convert.ToInt32(Request.Params["districtCode"])) == -1 ? 0 : Convert.ToInt32(Request.Params["districtCode"]);
            }

            if (!string.IsNullOrEmpty(Request.Params["year"]))
            {
                ViewBag.year = Convert.ToInt32(Request.Params["year"]) == -1 ? 0 : Convert.ToInt32(Request.Params["year"]);
            }

            if (!string.IsNullOrEmpty(Request.Params["scheme"]))
            {
                ViewBag.scheme = Convert.ToInt32(Request.Params["scheme"]);
            }

            return View();
        }

        public ActionResult DownloadQcrPdf(int qcrId)
        {
            QM_QCR_DETAILS model = new QM_QCR_DETAILS();
            string fileName = string.Empty;
            string filePath = string.Empty;
            string year = string.Empty;
            PMGSYEntities dbContext = new PMGSYEntities();
            model = dbContext.QM_QCR_DETAILS.Where(x => x.QCR_ID == qcrId).FirstOrDefault();

            fileName = model.FILE_NAME;
            filePath = model.FILE_PATH;
            year = fileName.Split('_')[0];

            string path = ConfigurationManager.AppSettings["QCR_DETAILS_PATH"].ToString();

            string fullPath = Path.Combine(path, year, fileName);
            byte[] FileBytes = System.IO.File.ReadAllBytes(fullPath);
            return File(FileBytes, "application/octet-stream", fileName);
        }

        #endregion

        #region View Uploaded QCR PDF

        [HttpGet]
        public ActionResult ViewUploadedQCR()
        {
            try
            {

                PMGSYEntities dbContext = new PMGSYEntities();
                CommonFunctions objCommon = new CommonFunctions();

                UploadedQCRModel uploadQCR = new UploadedQCRModel();

                uploadQCR.StateName = PMGSYSession.Current.StateName;
                uploadQCR.stateCode = PMGSYSession.Current.StateCode;
                uploadQCR.lstDistricts = new List<SelectListItem>();
                uploadQCR.lstDistricts = objCommon.PopulateDistrict(uploadQCR.stateCode, true);
                uploadQCR.Years = new List<SelectListItem>();
                uploadQCR.Years = PopulateYear(0, true, true);
                return View(uploadQCR);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "ViewUploadedQCR()");
                return null;
            }
        }

        [Audit]
        public ActionResult GetExecutionList(int? page, int? rows, string sidx, string sord)
        {
            try
            {
                int yearCode = 0;
                int districtCode = 0;

                long totalRecords = 0;
                IQualityMonitoringBAL objBAL = new QualityMonitoringBAL();

                using (CommonFunctions commonFunction = new CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new GridParams(page, rows, sidx, sord, Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                    {
                        return null;
                    }
                }

                if (!string.IsNullOrEmpty(Request.Params["yearCode"]))
                {
                    yearCode = Convert.ToInt32(Request.Params["yearCode"]);
                }

                if (!string.IsNullOrEmpty(Request.Params["districtCode"]))
                {
                    districtCode = Convert.ToInt32(Request.Params["districtCode"]);
                }

                var jsonData = new
                {
                    rows = objBAL.GetExecutionListView(yearCode, districtCode, Convert.ToInt32(page) - 1, Convert.ToInt32(rows), sidx, sord, out totalRecords),
                    total = totalRecords <= Convert.ToInt32(rows) ? 1 : totalRecords / Convert.ToInt32(rows) + 1,
                    page = Convert.ToInt32(page),
                    records = totalRecords,
                };

                return Json(jsonData, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "GetExecutionProgressList()");
                return null;
            }
        }

        [Audit]
        public ActionResult GetQCRDetailsListToView(string idtemp, int? page, int? rows, string sidx, string sord)
        {
            try
            {

                string id1 = "0";
                Int32 id = 0;
                if (idtemp != null)
                {
                    string[] encParam = idtemp.Split('/');
                    decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { encParam[0], encParam[1], encParam[2] });
                    if (decryptedParameters.Count > 0)
                    {
                        id1 = decryptedParameters["imsRoadID"];

                    }

                }

                id = Convert.ToInt32(id1);

                long totalRecords = 0;

                IQualityMonitoringBAL objBAL = new QualityMonitoringBAL();

                using (CommonFunctions commonFunction = new CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new GridParams(page, rows, sidx, sord, Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                    {
                        return null;
                    }
                }


                var jsonData = new
                {
                    rows = objBAL.GetQCRListToView(id, Convert.ToInt32(page) - 1, Convert.ToInt32(rows), sidx, sord, out totalRecords),
                    total = totalRecords <= Convert.ToInt32(rows) ? 1 : totalRecords / Convert.ToInt32(rows) + 1,
                    page = Convert.ToInt32(page),
                    records = totalRecords,
                };

                return Json(jsonData, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "GetQCRDetailsListToView()");
                return null;
            }
        }


        #endregion


        #region CQCAdmin + SQC Common


        /// <summary>
        /// Get NQM Names
        /// </summary>
        /// <param name="selectedState"></param>
        /// <param name="query"></param>
        /// <returns></returns>
        [Audit]
        public ActionResult GetNQMNames(int selectedState)
        {
            try
            {
                //return Json(new CommonFunctions().PopulateMonitors("false", "I", selectedState));
                return Json(new CommonFunctions().PopulateAllMonitors("false", "I", selectedState));  //Edited on 13-12-2022 to display all monitors

            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return null;
            }
        }


        /// <summary>
        /// Get SQM Names as per selected State
        /// </summary>
        /// <param name="selectedState"></param>
        /// <returns></returns>
        [Audit]
        public ActionResult GetSQMNames(int selectedState)
        {
            try
            {
                return Json(new CommonFunctions().PopulateMonitors("false", "S", selectedState));
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return null;
            }
        }

        /// <summary>
        /// Common Filters for Quality Module
        /// </summary>
        /// <returns></returns>
        [Audit]
        public ActionResult QualityFilters()
        {
            QMFilterViewModel qmFilterModel = new QMFilterViewModel();
            CommonFunctions objCommonFunctions = new CommonFunctions();

            //qmFilterModel.QM_TYPE_CODE = "I";
            qmFilterModel.MAST_STATE_CODE = 0;
            qmFilterModel.ADMIN_QM_CODE = 0;
            qmFilterModel.schemeList = new List<SelectListItem> {
                                                            new SelectListItem{ Text = "All Schemes", Value ="0" , Selected = true },
                                                            new SelectListItem{ Text = "PMGSY I", Value ="1" },
                                                            new SelectListItem{ Text = "PMGSY II", Value ="2" } ,
                                                            new SelectListItem{ Text = "RCPLWE", Value ="3" },
                                                            new SelectListItem{ Text = "PMGSY III", Value ="4" }


                                                            };



            qmFilterModel.ROAD_STATUS_LIST = objCommonFunctions.PopulateRoadStatus();
            qmFilterModel.ROAD_STATUS_LIST.Insert(3, new SelectListItem { Text = "Maintenance", Value = "M" });

            qmFilterModel.FROM_MONTH = DateTime.Now.Month;
            qmFilterModel.FROM_YEAR = DateTime.Now.Year;
            qmFilterModel.TO_MONTH = DateTime.Now.Month;
            qmFilterModel.TO_YEAR = DateTime.Now.Year;

            //if (PMGSYSession.Current.RoleCode == 5)  //CQC
            if (PMGSYSession.Current.RoleCode == 9)  //CQC or CQCAdmin
            {
                qmFilterModel.STATES = objCommonFunctions.PopulateStates(false);
                qmFilterModel.STATES.Insert(0, (new SelectListItem { Text = "All", Value = "0", Selected = true }));
                qmFilterModel.qmType = "NQM";

                qmFilterModel.MONITORS = objCommonFunctions.PopulateAllMonitors("false", "I", 0); //Purposely taken String "false" as argument
            }
            else if (PMGSYSession.Current.RoleCode == 8 || PMGSYSession.Current.RoleCode == 69 || PMGSYSession.Current.RoleCode == 48)  //SQC
            {
                qmFilterModel.MAST_STATE_CODE = PMGSYSession.Current.StateCode;
                qmFilterModel.MONITORS = objCommonFunctions.PopulateAllMonitors("false", "S", PMGSYSession.Current.StateCode); //Purposely taken String "false" as argument
                qmFilterModel.qmType = "SQM";
            }
            else if (PMGSYSession.Current.RoleCode == 22 || PMGSYSession.Current.RoleCode == 38 || PMGSYSession.Current.RoleCode == 54)  //PIU or PIUOA  or PIURCPLWE
            {
                qmFilterModel.MAST_STATE_CODE = PMGSYSession.Current.StateCode;
                qmFilterModel.MONITORS = objCommonFunctions.PopulateMonitorsDistrictWise("false", "S", PMGSYSession.Current.StateCode, PMGSYSession.Current.DistrictCode); //Purposely taken String "false" as argument
            }
            else if (PMGSYSession.Current.RoleCode == 5)
            {
                qmFilterModel.STATES = objCommonFunctions.PopulateStates(false);
                qmFilterModel.STATES.Insert(0, (new SelectListItem { Text = "All", Value = "0", Selected = true }));
                //  qmFilterModel.MONITORS = objCommonFunctions.PopulateAllMonitors("false", "S", 0); 
                qmFilterModel.MONITORS = objCommonFunctions.PopulateMonitors("false", "0", 0);
                qmFilterModel.qmType = "SQM";

            }

            qmFilterModel.FROM_MONTHS_LIST = objCommonFunctions.PopulateMonths(false);
            qmFilterModel.FROM_YEARS_LIST = objCommonFunctions.PopulateYears(false).Where(a => a.Value != "0").ToList();
            qmFilterModel.TO_MONTHS_LIST = objCommonFunctions.PopulateMonths(false);
            qmFilterModel.TO_YEARS_LIST = objCommonFunctions.PopulateYears(false).Where(a => a.Value != "0").ToList();

            //    30 - 06 - 2022  vikky 


            qmFilterModel.roadOrBridgeList = new List<SelectListItem> {
                                                            new SelectListItem{ Text = "All", Value ="0" , Selected = true },
                                                            new SelectListItem{ Text = "Road", Value ="P" },
                                                            new SelectListItem{ Text = "Bridge", Value ="L" }

            };





            qmFilterModel.gradeTypeList = new List<SelectListItem> {
                                                            new SelectListItem{ Text = "All", Value ="0" , Selected = true },
                                                            new SelectListItem{ Text = "S", Value ="1" },
                                                            new SelectListItem{ Text = "SRI", Value ="2" },
                                                             new SelectListItem{ Text = "U", Value ="3" }


            };

            qmFilterModel.eFormStatusTypeList = new List<SelectListItem> {
                                                            new SelectListItem{ Text = "All", Value ="0" , Selected = true },
                                                            new SelectListItem{ Text = "Pending at PIU", Value ="1" },
                                                            new SelectListItem{ Text = "Pending at QM", Value ="2" },
                                                             new SelectListItem{ Text = "Submitted", Value ="3" }


            };
            //    30 - 06 - 2022  vikky 
            return View(qmFilterModel);
        }


        /// <summary>
        /// Filters for Inspection Report of SQM for CQC 
        /// </summary>
        /// <returns></returns>
        [Audit]
        public ActionResult QualityFilters2TierCQC()
        {
            QMFilterViewModel qmFilterModel = new QMFilterViewModel();
            CommonFunctions objCommonFunctions = new CommonFunctions();

            qmFilterModel.MAST_STATE_CODE = 0;
            qmFilterModel.ADMIN_QM_CODE = 0;

            qmFilterModel.FROM_MONTH = DateTime.Now.Month;
            qmFilterModel.FROM_YEAR = DateTime.Now.Year;
            qmFilterModel.TO_MONTH = DateTime.Now.Month;
            qmFilterModel.TO_YEAR = DateTime.Now.Year;

            qmFilterModel.schemeList = new List<SelectListItem> {
                                                            new SelectListItem{ Text = "All Schemes", Value ="0" , Selected = true },
                                                            new SelectListItem{ Text = "PMGSY I", Value ="1" },
                                                            new SelectListItem{ Text = "PMGSY II", Value ="2" } ,
                                                            new SelectListItem{ Text = "RCPLWE", Value ="3" },
                                                            new SelectListItem{ Text = "PMGSY III", Value ="4" }


                                                            };
            qmFilterModel.ROAD_STATUS_LIST = objCommonFunctions.PopulateRoadStatus();
            qmFilterModel.ROAD_STATUS_LIST.Insert(3, new SelectListItem { Text = "Maintenance", Value = "M" });

            qmFilterModel.STATES = objCommonFunctions.PopulateStates(false);
            qmFilterModel.STATES.Insert(0, (new SelectListItem { Text = "All", Value = "0", Selected = true }));
            qmFilterModel.MONITORS = objCommonFunctions.PopulateMonitors("false", "0", 0); //Purposely taken String "false" as argument

            qmFilterModel.FROM_MONTHS_LIST = objCommonFunctions.PopulateMonths(false);
            qmFilterModel.FROM_YEARS_LIST = objCommonFunctions.PopulateYears(false).Where(a => a.Value != "0").ToList();
            qmFilterModel.TO_MONTHS_LIST = objCommonFunctions.PopulateMonths(false);
            qmFilterModel.TO_YEARS_LIST = objCommonFunctions.PopulateYears(false).Where(a => a.Value != "0").ToList();

            //    30 - 06 - 2022  vikky 


            qmFilterModel.roadOrBridgeList = new List<SelectListItem> {
                                                            new SelectListItem{ Text = "All", Value ="0" , Selected = true },
                                                            new SelectListItem{ Text = "Road", Value ="P" },
                                                            new SelectListItem{ Text = "Bridge", Value ="L" }

            };

            qmFilterModel.qmType = "SQM";



            qmFilterModel.gradeTypeList = new List<SelectListItem> {
                                                            new SelectListItem{ Text = "All", Value ="0" , Selected = true },
                                                            new SelectListItem{ Text = "S", Value ="1" },
                                                            new SelectListItem{ Text = "SRI", Value ="2" },
                                                             new SelectListItem{ Text = "U", Value ="3" }


            };

            qmFilterModel.eFormStatusTypeList = new List<SelectListItem> {
                                                            new SelectListItem{ Text = "All", Value ="0" , Selected = true },
                                                            new SelectListItem{ Text = "Pending at PIU", Value ="1" },
                                                            new SelectListItem{ Text = "Pending at QM", Value ="2" },
                                                             new SelectListItem{ Text = "Submitted", Value ="3" }


            };
            //    30 - 06 - 2022  vikky 
            return View(qmFilterModel);
        }


        /// <summary>
        /// Filters for ATR Details
        /// </summary>
        /// <returns></returns>
        [Audit]
        public ActionResult QualityATRFilters()
        {
            QMFilterViewModel qmFilterModel = new QMFilterViewModel();
            CommonFunctions objCommonFunctions = new CommonFunctions();

            qmFilterModel.MAST_STATE_CODE = 0;
            qmFilterModel.ADMIN_QM_CODE = 0;
            qmFilterModel.ATR_STATUS = "0";
            qmFilterModel.ROAD_STATUS = "A";

            qmFilterModel.FROM_MONTH = DateTime.Now.Month;
            qmFilterModel.FROM_YEAR = DateTime.Now.Year;
            qmFilterModel.TO_MONTH = DateTime.Now.Month;
            qmFilterModel.TO_YEAR = DateTime.Now.Year;

            //if (PMGSYSession.Current.RoleCode == 5)  //CQC
            if (PMGSYSession.Current.RoleCode == 5 || PMGSYSession.Current.RoleCode == 9)
            {
                qmFilterModel.STATES = objCommonFunctions.PopulateStates(false);
                qmFilterModel.STATES.Insert(0, (new SelectListItem { Text = "All", Value = "0", Selected = true }));

                //qmFilterModel.MONITORS = objCommonFunctions.PopulateMonitors("false", "I", 0).ToList(); //Purposely taken String "false" as argument
                qmFilterModel.MONITORS = objCommonFunctions.PopulateAllMonitors("false", "I", 0).ToList(); //Purposely taken String "false" as argument //Edited on 13-12-2022 to display all monitors

            }
            else if (PMGSYSession.Current.RoleCode == 8 || PMGSYSession.Current.RoleCode == 69 || PMGSYSession.Current.RoleCode == 48)  //ATRDetails//SQC
            {
                qmFilterModel.MAST_STATE_CODE = PMGSYSession.Current.StateCode;
                //qmFilterModel.MONITORS = objCommonFunctions.PopulateMonitors("false", "I", PMGSYSession.Current.StateCode).ToList(); //Purposely taken String "false" as argument
                qmFilterModel.MONITORS = objCommonFunctions.PopulateAllMonitors("false", "I", PMGSYSession.Current.StateCode).ToList(); //Purposely taken String "false" as argument //Edited on 13-12-2022 to display all monitors

            }
            else if (PMGSYSession.Current.RoleCode == 22 || PMGSYSession.Current.RoleCode == 38 || PMGSYSession.Current.RoleCode == 54)  //PIU or PIURCPLWE
            {
                qmFilterModel.MAST_STATE_CODE = PMGSYSession.Current.StateCode;
                qmFilterModel.MONITORS = objCommonFunctions.PopulateMonitorsDistrictWise("false", "I", PMGSYSession.Current.StateCode, PMGSYSession.Current.DistrictCode).ToList(); //Purposely taken String "false" as argument
            }

            qmFilterModel.schemeList = new List<SelectListItem> {
                                                            new SelectListItem{ Text = "All Schemes", Value ="0" , Selected = true },
                                                            new SelectListItem{ Text = "PMGSY I", Value ="1" },
                                                            new SelectListItem{ Text = "PMGSY II", Value ="2" } ,
                                                            new SelectListItem{ Text = "RCPLWE", Value ="3" },
                                                            new SelectListItem{ Text = "PMGSY III", Value ="4" }


                                                            };//ATR_Change

            qmFilterModel.FROM_MONTHS_LIST = objCommonFunctions.PopulateMonths(false);
            qmFilterModel.FROM_YEARS_LIST = objCommonFunctions.PopulateYears(false).Where(a => a.Value != "0").ToList();
            qmFilterModel.TO_MONTHS_LIST = objCommonFunctions.PopulateMonths(false);
            qmFilterModel.TO_YEARS_LIST = objCommonFunctions.PopulateYears(false).Where(a => a.Value != "0").ToList();
            qmFilterModel.ATR_STATUS_LIST = objCommonFunctions.QualityATRStatus();

            qmFilterModel.imsSanctionedList = new List<SelectListItem> {
                                                        new SelectListItem{ Text = "Sanctioned", Value ="Y" , Selected = true },
                                                        new SelectListItem{ Text = "Dropped", Value ="D" },
                                                       };


            qmFilterModel.ROAD_STATUS_LIST = objCommonFunctions.PopulateRoadStatus();
            return View(qmFilterModel);
        }

        /// <summary>
        /// SQC 3rd Tier Inspection Filters
        /// </summary>
        /// <returns></returns>
        /// <returns></returns>SQM
        public ActionResult Quality3TierSQCInspFilters()
        {
            QMFilterViewModel qmFilterModel = new QMFilterViewModel();
            CommonFunctions objCommonFunctions = new CommonFunctions();

            qmFilterModel.MAST_STATE_CODE = 0;
            qmFilterModel.ADMIN_QM_CODE = 0;

            qmFilterModel.FROM_MONTH = DateTime.Now.Month;
            qmFilterModel.FROM_YEAR = DateTime.Now.Year;
            qmFilterModel.TO_MONTH = DateTime.Now.Month;
            qmFilterModel.TO_YEAR = DateTime.Now.Year;

            qmFilterModel.MAST_STATE_CODE = PMGSYSession.Current.StateCode;


            qmFilterModel.schemeList = new List<SelectListItem> {
                                                            new SelectListItem{ Text = "All Schemes", Value ="0" , Selected = true },
                                                            new SelectListItem{ Text = "PMGSY I", Value ="1" },
                                                            new SelectListItem{ Text = "PMGSY II", Value ="2" } ,
                                                            new SelectListItem{ Text = "RCPLWE", Value ="3" },
                                                            new SelectListItem{ Text = "PMGSY III", Value ="4" }
            };

            qmFilterModel.ROAD_STATUS_LIST = objCommonFunctions.PopulateRoadStatus();
            qmFilterModel.ROAD_STATUS_LIST.Insert(3, new SelectListItem { Text = "Maintenance", Value = "M" });

            if (PMGSYSession.Current.RoleCode == 5)
            {
                qmFilterModel.STATES = objCommonFunctions.PopulateStates(false);
                qmFilterModel.STATES.Insert(0, (new SelectListItem { Text = "All", Value = "0", Selected = true }));

                qmFilterModel.MONITORS = objCommonFunctions.PopulateAllMonitors("false", "I", 0); //Purposely taken String "false" as argument
            }

            if (PMGSYSession.Current.RoleCode == 8 || PMGSYSession.Current.RoleCode == 69 || PMGSYSession.Current.RoleCode == 48)
            {
                qmFilterModel.MONITORS = objCommonFunctions.PopulateAllMonitors("false", "I", PMGSYSession.Current.StateCode); //Purposely taken String "false" as argument
            }
            else if (PMGSYSession.Current.RoleCode == 22 || PMGSYSession.Current.RoleCode == 38 || PMGSYSession.Current.RoleCode == 54)  //PIU or PIURCPLWE
            {
                qmFilterModel.MONITORS = objCommonFunctions.PopulateMonitorsDistrictWise("false", "I", PMGSYSession.Current.StateCode, PMGSYSession.Current.DistrictCode); //Purposely taken String "false" as argument
            }
            qmFilterModel.qmType = "NQM";

            qmFilterModel.roadOrBridgeList = new List<SelectListItem> {
                                                            new SelectListItem{ Text = "All", Value ="0" , Selected = true },
                                                            new SelectListItem{ Text = "Road", Value ="P" },
                                                            new SelectListItem{ Text = "Bridge", Value ="L" }

            };

            qmFilterModel.gradeTypeList = new List<SelectListItem> {
                                                            new SelectListItem{ Text = "All", Value ="0" , Selected = true },
                                                            new SelectListItem{ Text = "S", Value ="1" },
                                                            new SelectListItem{ Text = "SRI", Value ="2" },
                                                             new SelectListItem{ Text = "U", Value ="3" }


            };

            qmFilterModel.eFormStatusTypeList = new List<SelectListItem> {
                                                            new SelectListItem{ Text = "All", Value ="0" , Selected = true },
                                                            new SelectListItem{ Text = "Pending at PIU", Value ="1" },
                                                            new SelectListItem{ Text = "Pending at QM", Value ="2" },
                                                             new SelectListItem{ Text = "Submitted", Value ="3" }
            };

            qmFilterModel.FROM_MONTHS_LIST = objCommonFunctions.PopulateMonths(false);
            qmFilterModel.FROM_YEARS_LIST = objCommonFunctions.PopulateYears(false).Where(a => a.Value != "0").ToList();
            qmFilterModel.TO_MONTHS_LIST = objCommonFunctions.PopulateMonths(false);
            qmFilterModel.TO_YEARS_LIST = objCommonFunctions.PopulateYears(false).Where(a => a.Value != "0").ToList();

            return View(qmFilterModel);
        }


        /// <summary>
        /// Populate District as per selected state
        /// </summary>
        /// <param name="selectedState"></param>
        /// <returns></returns>
        [Audit]
        public ActionResult GetDistricts(int selectedState, int month, int year)
        {
            dbContext = new PMGSYEntities();
            try
            {
                // return Json(new SelectList(dbContext.MASTER_DISTRICT.Where(m => m.MAST_STATE_CODE == selectedState && m.MAST_DISTRICT_ACTIVE == "Y"), "MAST_DISTRICT_CODE", "MAST_DISTRICT_NAME").ToList());
                return Json(new QualityMonitoringBAL().GetDistrictForScheduleCreationBAL(selectedState, month, year));
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);

                return null;
            }
            finally
            {
                dbContext.Dispose();
            }
        }


        /// <summary>
        /// Populate Districts of selected state
        /// </summary>
        /// <param name="selectedState"></param>
        /// <returns></returns>
        [Audit]
        public ActionResult PopulateDistricts(int selectedState)
        {
            dbContext = new PMGSYEntities();
            try
            {
                return Json(new SelectList(dbContext.MASTER_DISTRICT.Where(m => m.MAST_STATE_CODE == selectedState && m.MAST_DISTRICT_ACTIVE == "Y"), "MAST_DISTRICT_CODE", "MAST_DISTRICT_NAME").ToList(), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return null;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        ///// <summary>
        ///// Populate states as per selected monitor
        ///// state not included in list to which monitor belongs
        ///// </summary>
        ///// <param name="selectedState"></param>
        ///// <returns></returns>
        //[Audit]
        //public ActionResult GetMonitorStates(int selectedMonitor)
        //{
        //    PMGSYEntities dbContext = new PMGSYEntities();
        //    CommonFunctions objCommonFunctions = new CommonFunctions();
        //    try
        //    {
        //        string monitorState = dbContext.ADMIN_QUALITY_MONITORS.Where(m => m.ADMIN_QM_CODE == selectedMonitor).Select(m => m.MAST_STATE_CODE).First().ToString();

        //        SelectListItem item = new SelectListItem();
        //        item.Value = monitorState.ToString();

        //        List<SelectListItem> stateList = objCommonFunctions.PopulateStates().Where(a => a.Value != monitorState).ToList();


        //        return Json(stateList);
        //    }
        //    catch (Exception ex)
        //    {
        //        Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
        //        return null;
        //    }
        //    finally
        //    {
        //        dbContext.Dispose();
        //    }
        //}

        //Added By Ananad 10 June 2015
        /// <summary>
        /// Populate states as per selected monitor
        /// state not included in list to which monitor belongs
        /// </summary>
        /// <param name="selectedState"></param>
        /// <returns></returns>
        [Audit]
        public ActionResult GetMonitorStates(int selectedMonitor)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            CommonFunctions objCommonFunctions = new CommonFunctions();
            try
            {

                List<SelectListItem> stateList;
                var monitor = dbContext.ADMIN_QUALITY_MONITORS.Where(m => m.ADMIN_QM_CODE == selectedMonitor).FirstOrDefault();

                if (monitor.ADMIN_QM_SERVICE_TYPE == "A")
                {
                    int[] states = dbContext.QUALITY_QM_CADRE_STATE.Where(m => m.ADMIN_QM_CODE == selectedMonitor).Select(m => m.MAST_STATE_CODE).ToArray();

                    stateList = objCommonFunctions.PopulateStates().Where(a => !states.Contains(Convert.ToInt32(a.Value))).ToList();

                }
                else if (monitor.ADMIN_QM_SERVICE_TYPE == "S")
                {
                    var monitorState = dbContext.ADMIN_QUALITY_MONITORS.Where(m => m.ADMIN_QM_CODE == selectedMonitor).Select(m => m.MAST_STATE_CODE).First().ToString();
                    stateList = objCommonFunctions.PopulateStates().Where(a => a.Value != monitorState).ToList();

                }
                else
                {
                    stateList = objCommonFunctions.PopulateStates().ToList();
                }

                return Json(stateList);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return null;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        /// <summary>
        /// Get List of States in which monitor have done inspections.
        /// </summary>
        /// <param name="selectedMonitor"></param>
        /// <returns></returns>
        //[Audit]
        //public ActionResult GetMonitorsInspectedStates()
        //{
        //    try
        //    {
        //        CommonFunctions objCommonFunctions = new CommonFunctions();
        //        return Json(objCommonFunctions.PopulateMonitorsInspectedStates(Convert.ToInt32(Request.Params["selectedMonitor"]),
        //            Convert.ToInt32(Request.Params["frmMonth"]), Convert.ToInt32(Request.Params["frmYear"]), Convert.ToInt32(Request.Params["toMonth"]), Convert.ToInt32(Request.Params["toYear"])).ToList());
        //    }
        //    catch {
        //        return null;
        //    }
        //}


        /// <summary>
        ///  Populate NQMs who have done inspection during particular time span
        /// </summary>
        /// <returns></returns>
        [Audit]
        public ActionResult PopulateNQM()
        {
            try
            {
                CommonFunctions objCommonFunctions = new CommonFunctions();
                return Json(objCommonFunctions.PopulateNQM(Convert.ToInt32(Request.Params["frmMonth"]), Convert.ToInt32(Request.Params["frmYear"]), Convert.ToInt32(Request.Params["toMonth"]), Convert.ToInt32(Request.Params["toYear"])).ToList());
            }
            catch
            {
                return null;
            }
        }


        /// <summary>
        /// Schedule Filters
        /// </summary>
        /// <returns></returns>
        public ActionResult ScheduleFilters()
        {
            QMFilterViewModel qmFilterModel = new QMFilterViewModel();
            CommonFunctions objCommonFunctions = new CommonFunctions();

            qmFilterModel.FROM_MONTH = DateTime.Now.Month;
            qmFilterModel.FROM_YEAR = DateTime.Now.Year;
            qmFilterModel.FROM_MONTHS_LIST = objCommonFunctions.PopulateMonths(false);
            qmFilterModel.FROM_YEARS_LIST = objCommonFunctions.PopulateYears(false).Where(a => a.Value != "0").ToList();
            if (DateTime.Now.Month == 12)
            {
                string nextYear = (DateTime.Now.Year + 1).ToString();
                qmFilterModel.FROM_YEARS_LIST.Add(new SelectListItem { Text = nextYear, Value = nextYear });
                qmFilterModel.FROM_YEARS_LIST = qmFilterModel.FROM_YEARS_LIST.OrderByDescending(c => c.Value).ToList();
            }
            return View(qmFilterModel);
        }

        /// <summary>
        /// Schedule Filters
        /// </summary>
        /// <returns></returns>
        public ActionResult ScheduleFilters3TierSQC()
        {
            QMFilterViewModel qmFilterModel = new QMFilterViewModel();
            CommonFunctions objCommonFunctions = new CommonFunctions();

            qmFilterModel.FROM_MONTH = DateTime.Now.Month;
            qmFilterModel.FROM_YEAR = DateTime.Now.Year;
            qmFilterModel.FROM_MONTHS_LIST = objCommonFunctions.PopulateMonths(false);
            qmFilterModel.FROM_YEARS_LIST = objCommonFunctions.PopulateYears(false).Where(a => a.Value != "0").ToList();
            if (DateTime.Now.Month == 12)
            {
                string nextYear = (DateTime.Now.Year + 1).ToString();
                qmFilterModel.FROM_YEARS_LIST.Add(new SelectListItem { Text = nextYear, Value = nextYear });
                //qmFilterModel.FROM_YEARS_LIST.Add(new SelectListItem { Text = "2015", Value = "2015" });
                qmFilterModel.FROM_YEARS_LIST = qmFilterModel.FROM_YEARS_LIST.OrderByDescending(c => c.Value).ToList();
            }
            return View(qmFilterModel);
        }


        /// <summary>
        /// Schedule Filters - as Month & Year to display monthwise schedule
        /// </summary>
        /// <returns></returns>
        public ActionResult ScheduleFiltersMonitors()
        {
            QMFilterViewModel qmFilterModel = new QMFilterViewModel();
            CommonFunctions objCommonFunctions = new CommonFunctions();

            qmFilterModel.FROM_MONTH = DateTime.Now.Month;
            qmFilterModel.FROM_YEAR = DateTime.Now.Year;
            qmFilterModel.FROM_MONTHS_LIST = objCommonFunctions.PopulateMonths(false);
            qmFilterModel.FROM_YEARS_LIST = objCommonFunctions.PopulateYears(false).Where(a => a.Value != "0").ToList();
            if (DateTime.Now.Month == 12)
            {
                string nextYear = (DateTime.Now.Year + 1).ToString();
                qmFilterModel.FROM_YEARS_LIST.Add(new SelectListItem { Text = nextYear, Value = nextYear });
                //qmFilterModel.FROM_YEARS_LIST.Add(new SelectListItem { Text = "2015", Value = "2015" });
                qmFilterModel.FROM_YEARS_LIST = qmFilterModel.FROM_YEARS_LIST.OrderByDescending(c => c.Value).ToList();
            }
            return View(qmFilterModel);
        }


        /// <summary>
        /// Populate Schedule List Current Month Onwards
        /// </summary>
        /// <param name="formCollection"></param>
        /// <returns></returns>
        [HttpPost]
        [Audit]
        public ActionResult GetScheduleList(FormCollection formCollection)
        {
            qualityBAL = new QualityMonitoringBAL();
            int totalRecords;
            try
            {
                //Adde By Abhishek kamble 29-Apr-2014 start
                using (CommonFunctions commonFunction = new CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new GridParams(Convert.ToInt32(formCollection["page"]), Convert.ToInt32(formCollection["rows"]), formCollection["sidx"], formCollection["sord"], Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                    {
                        return null;
                    }
                }
                //Adde By Abhishek kamble 29-Apr-2014 end
                var jsonData = new
                {
                    rows = qualityBAL.GetScheduleListBAL(Convert.ToInt32(formCollection["month"]), Convert.ToInt32(formCollection["year"]),
                                                         Convert.ToInt32(formCollection["page"]) - 1, Convert.ToInt32(formCollection["rows"]),
                                                            formCollection["sidx"], formCollection["sord"], out totalRecords, formCollection["filters"]),
                    total = totalRecords <= Convert.ToInt32(formCollection["rows"]) ? 1 : (totalRecords % Convert.ToInt32(formCollection["rows"]) == 0 ? totalRecords / Convert.ToInt32(formCollection["rows"]) : (totalRecords / Convert.ToInt32(formCollection["rows"]) + 1)),
                    page = Convert.ToInt32(formCollection["page"]),
                    records = totalRecords
                };
                return Json(jsonData);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return Json(String.Empty);
            }
        }


        /// <summary>
        /// Get To Create Schedule
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Audit]
        public ActionResult QMCreateSchedule()
        {
            QMScheduleViewModel qmScheduleModel = new QMScheduleViewModel();
            CommonFunctions objCommonFunctions = new CommonFunctions();
            qualityBAL = new QualityMonitoringBAL();
            dbContext = new PMGSYEntities();
            try
            {
                qmScheduleModel.OPERATION = "C";

                //if (PMGSYSession.Current.RoleCode == 5)  //CQC
                if (PMGSYSession.Current.RoleCode == 9 || PMGSYSession.Current.RoleCode == 5)  //CQC
                {
                    qmScheduleModel.MAST_STATE_CODE = 0;
                    qmScheduleModel.STATES = new SelectList(new List<SelectListItem>().AsEnumerable<SelectListItem>(), "Value", "Text").ToList();
                    qmScheduleModel.MONITORS = objCommonFunctions.PopulateMonitorsForScheduleCreation(String.Empty, "I", 0, DateTime.Now.Month, DateTime.Now.Year);
                    qmScheduleModel.DISTRICTS = new SelectList(new List<SelectListItem>().AsEnumerable<SelectListItem>(), "Value", "Text").ToList();
                }
                else if (PMGSYSession.Current.RoleCode == 8 || PMGSYSession.Current.RoleCode == 69 || PMGSYSession.Current.RoleCode == 48)  //SQC
                {
                    qmScheduleModel.MAST_STATE_CODE = PMGSYSession.Current.StateCode;
                    qmScheduleModel.MONITORS = objCommonFunctions.PopulateMonitorsForScheduleCreation(String.Empty, "S", PMGSYSession.Current.StateCode, DateTime.Now.Month, DateTime.Now.Year);
                    qmScheduleModel.DISTRICTS = qualityBAL.GetDistrictForScheduleCreationBAL(PMGSYSession.Current.StateCode, DateTime.Now.Month, DateTime.Now.Year);
                    new SelectList(dbContext.MASTER_DISTRICT.Where(m => m.MAST_STATE_CODE == PMGSYSession.Current.StateCode && m.MAST_DISTRICT_ACTIVE == "Y"), "MAST_DISTRICT_CODE", "MAST_DISTRICT_NAME").ToList();
                }


                qmScheduleModel.ADMIN_QM_CODE = 0;
                qmScheduleModel.ADMIN_IM_MONTH = DateTime.Now.Month;
                qmScheduleModel.ADMIN_IM_YEAR = DateTime.Now.Year;

                qmScheduleModel.MONTHS_LIST = objCommonFunctions.PopulateCurrNextMonths(qmScheduleModel.ADMIN_IM_MONTH, false);//objCommonFunctions.PopulateMonths(false);
                qmScheduleModel.YEARS_LIST = objCommonFunctions.PopulateCurrYear(qmScheduleModel.ADMIN_IM_YEAR, false);//objCommonFunctions.PopulateYears(false);
                if (DateTime.Now.Month == 12)
                {
                    qmScheduleModel.MONTHS_LIST = objCommonFunctions.PopulateMonths(false);

                    string nextYear = (DateTime.Now.Year + 1).ToString();
                    qmScheduleModel.YEARS_LIST.Add(new SelectListItem { Text = nextYear, Value = nextYear });
                    //qmScheduleModel.YEARS_LIST.Add(new SelectListItem { Text = "2015", Value = "2015" });
                    qmScheduleModel.YEARS_LIST = qmScheduleModel.YEARS_LIST.OrderByDescending(c => c.Value).ToList();
                }

                return View(qmScheduleModel);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return null;
            }
            finally
            {
                dbContext.Dispose();
            }
        }


        /// <summary>
        /// SQC Letter Layout
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult QMSQCLetter()
        {
            QMScheduleViewModel qmScheduleModel = new QMScheduleViewModel();
            CommonFunctions objCommonFunctions = new CommonFunctions();
            qualityBAL = new QualityMonitoringBAL();
            dbContext = new PMGSYEntities();
            try
            {
                qmScheduleModel.ADMIN_IM_MONTH = DateTime.Now.Month;
                qmScheduleModel.ADMIN_IM_YEAR = DateTime.Now.Year;

                qmScheduleModel.MONTHS_LIST = objCommonFunctions.PopulateMonths(false);
                qmScheduleModel.YEARS_LIST = objCommonFunctions.PopulateYears(false);
                if (DateTime.Now.Month == 12)
                {
                    string nextYear = (DateTime.Now.Year + 1).ToString();
                    qmScheduleModel.YEARS_LIST.Add(new SelectListItem { Text = nextYear, Value = nextYear });
                    qmScheduleModel.YEARS_LIST = qmScheduleModel.YEARS_LIST.OrderByDescending(c => c.Value).ToList();
                }
                return View(qmScheduleModel);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return null;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        /// <summary>
        /// View Listing to generate SQC Letter
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult QMSQCLetterList(FormCollection formCollection)
        {
            qualityBAL = new QualityMonitoringBAL();
            int totalRecords;
            try
            {
                //Added By Abhishek kamble 29-Apr-2014 start
                using (CommonFunctions commonFunction = new CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new GridParams(Convert.ToInt32(formCollection["page"]), Convert.ToInt32(formCollection["rows"]), formCollection["sidx"], formCollection["sord"], Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                    {
                        return null;
                    }
                }
                //Added By Abhishek kamble 29-Apr-2014 end
                var jsonData = new
                {
                    rows = qualityBAL.QMSQCLetterListBAL(Convert.ToInt32(formCollection["page"]) - 1, Convert.ToInt32(formCollection["rows"]),
                                                            formCollection["sidx"], formCollection["sord"], out totalRecords, formCollection["filters"],
                                                            Convert.ToInt32(formCollection["inspMonth"]), Convert.ToInt32(formCollection["inspYear"])),
                    total = totalRecords <= Convert.ToInt32(formCollection["rows"]) ? 1 : (totalRecords % Convert.ToInt32(formCollection["rows"]) == 0 ? totalRecords / Convert.ToInt32(formCollection["rows"]) : (totalRecords / Convert.ToInt32(formCollection["rows"]) + 1)),
                    page = Convert.ToInt32(formCollection["page"]),
                    records = totalRecords
                };
                return Json(jsonData);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return Json(String.Empty);
            }
        }


        /// <summary>
        /// Add SQC Letter Details
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult AddLetterDetails()
        {
            string message = string.Empty;
            qualityBAL = new QualityMonitoringBAL();
            dbContext = new PMGSYEntities();
            bool Status = false;
            try
            {
                if (Request.Params["userType"].Equals("S")) //SQC
                {
                    Status = qualityBAL.AddSQCLetterDetailsBAL(Convert.ToInt32(Request.Params["id"]), Convert.ToInt16(Request.Params["inspMonth"]), Convert.ToInt16(Request.Params["inspYear"]), ref message);
                }
                else if (Request.Params["userType"].Equals("I")) //NQM
                {
                    Int32 scheduleCode = Convert.ToInt32(Request.Params["scheduleCode"]);
                    if (!(dbContext.QUALITY_QM_TEAM.Where(c => c.ADMIN_SCHEDULE_CODE == scheduleCode).Any()))
                    {
                        Status = qualityBAL.AddNQMLetterDetailsBAL(Convert.ToInt32(Request.Params["scheduleCode"]), ref message);
                    }
                    else
                    {
                        return Json(new { success = false, status = "T", message = "E-Mail not sent because Monitor is a member of team inspection." });
                    }
                }

                if (Status)
                    return Json(new { success = true, message = message });
                else
                    return Json(new { success = false, message = message });
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "QualityMonitoring.AddLetterDetails()");
                return Json(new { success = false, message = "Error occurred while processing your request." });
            }
        }

        /// <summary>
        /// View Generated Letter in new window
        /// </summary>
        /// <returns></returns>
        //public ActionResult ViewLetter()
        //{
        //    dbContext = new PMGSYEntities();
        //    try
        //    {
        //        bool isLettterId = Convert.ToBoolean(Request.Params["isLettterId"]);
        //        QUALITY_QM_LETTER letterDetails = null;

        //        Microsoft.Reporting.WebForms.ReportViewer rview = new Microsoft.Reporting.WebForms.ReportViewer();
        //        rview.ServerReport.ReportServerUrl = new Uri(System.Web.Configuration.WebConfigurationManager.AppSettings["MvcReportViewer.ReportServerUrl"]);

        //        System.Collections.Generic.List<Microsoft.Reporting.WebForms.ReportParameter> paramList = new System.Collections.Generic.List<Microsoft.Reporting.WebForms.ReportParameter>();
        //        if (Request.Params["userType"].Equals("S"))//SQC
        //        {
        //            if (isLettterId)
        //            {
        //                letterDetails = dbContext.QUALITY_QM_LETTER.Find(Convert.ToInt32(Request.Params["id"]));
        //            }
        //            else
        //            {
        //                int sqcCode = Convert.ToInt32(Request.Params["id"]);
        //                int inspMonth = Convert.ToInt32(Request.Params["inspMonth"]);
        //                int inspYear = Convert.ToInt32(Request.Params["inspYear"]);
        //                letterDetails = dbContext.QUALITY_QM_LETTER.Where(c => c.ADMIN_SQC_CODE == sqcCode && c.ADMIN_IM_MONTH == inspMonth && c.ADMIN_IM_YEAR == inspYear).First();
        //            }

        //            paramList.Add(new Microsoft.Reporting.WebForms.ReportParameter("sqcCode", letterDetails.ADMIN_SQC_CODE.ToString()));
        //            paramList.Add(new Microsoft.Reporting.WebForms.ReportParameter("inspMonth", letterDetails.ADMIN_IM_MONTH.ToString()));
        //            paramList.Add(new Microsoft.Reporting.WebForms.ReportParameter("inspYear", letterDetails.ADMIN_IM_YEAR.ToString()));
        //            paramList.Add(new Microsoft.Reporting.WebForms.ReportParameter("stateCode", dbContext.ADMIN_SQC.Where(c => c.ADMIN_QC_CODE == letterDetails.ADMIN_SQC_CODE).Select(c => c.MAST_STATE_CODE).First().ToString()));
        //            paramList.Add(new Microsoft.Reporting.WebForms.ReportParameter("qmType", "I"));
        //            DateTime dtDate = new DateTime(letterDetails.ADMIN_IM_YEAR, letterDetails.ADMIN_IM_MONTH, 1);
        //            paramList.Add(new Microsoft.Reporting.WebForms.ReportParameter("inspMonthText", dtDate.ToString("MMMM")));
        //            paramList.Add(new Microsoft.Reporting.WebForms.ReportParameter("inspYearText", letterDetails.ADMIN_IM_YEAR.ToString()));
        //        }
        //        else if (Request.Params["userType"].Equals("I"))//NQM - Independent Monitor
        //        {
        //            //code remained
        //        }

        //        Microsoft.Reporting.WebForms.IReportServerCredentials irsc = new CustomReportCredentials(System.Web.Configuration.WebConfigurationManager.AppSettings["MvcReportViewer.Username"], System.Web.Configuration.WebConfigurationManager.AppSettings["MvcReportViewer.Password"]);
        //        rview.ServerReport.ReportServerCredentials = irsc;
        //        rview.ServerReport.ReportPath = letterDetails.TYPE.Equals("S") ? "/PMGSYCitizen/SQC_Letter" : "/PMGSYCitizen/NQM_Letter";
        //        rview.ServerReport.SetParameters(paramList);
        //        string mimeType, encoding, extension, deviceInfo;
        //        string[] streamids;
        //        Microsoft.Reporting.WebForms.Warning[] warnings;
        //        string format = "PDF"; //Desired format goes here (PDF, Excel, or Image)
        //        deviceInfo = "<DeviceInfo>" + "<SimplePageHeaders>True</SimplePageHeaders>" + "</DeviceInfo>";
        //        byte[] bytes = rview.ServerReport.Render(format, deviceInfo, out mimeType, out encoding, out extension, out streamids, out warnings);
        //        string filePath = letterDetails.TYPE.Equals("S") ? ConfigurationManager.AppSettings["QUALITY_QM_LETTER_SQC"].ToString() + letterDetails.FILE_NAME : ConfigurationManager.AppSettings["QUALITY_QM_LETTER_NQM"].ToString() + letterDetails.FILE_NAME;
        //        Response.Clear();

        //        var cd = new System.Net.Mime.ContentDisposition
        //        {
        //            // for example foo.bak
        //            FileName = letterDetails.FILE_NAME,

        //            // always prompt the user for downloading, set to true if you want 
        //            // the browser to try to show the file inline
        //            Inline = false,
        //        };

        //        Response.AppendHeader("Content-Disposition", cd.ToString());

        //        return File(bytes, "application/pdf");
        //    }
        //    catch (Exception)
        //    {
        //        return null;
        //    }
        //    finally {
        //        dbContext.Dispose();
        //    }
        //}


        /// <summary>
        /// View Generated Letter in new window
        /// </summary>
        /// <returns></returns>
        public ActionResult DownloadLetter()
        {
            dbContext = new PMGSYEntities();
            try
            {
                bool isLettterId = Convert.ToBoolean(Request.Params["isLettterId"]);
                QUALITY_QM_LETTER letterDetails = null;

                if (Request.Params["userType"].Equals("S"))//SQC
                {
                    if (isLettterId)
                    {
                        letterDetails = dbContext.QUALITY_QM_LETTER.Find(Convert.ToInt32(Request.Params["id"]));
                    }
                    else
                    {
                        int sqcCode = Convert.ToInt32(Request.Params["id"]);
                        int inspMonth = Convert.ToInt32(Request.Params["inspMonth"]);
                        int inspYear = Convert.ToInt32(Request.Params["inspYear"]);
                        letterDetails = dbContext.QUALITY_QM_LETTER.Where(c => c.ADMIN_SQC_CODE == sqcCode && c.ADMIN_IM_MONTH == inspMonth && c.ADMIN_IM_YEAR == inspYear).OrderByDescending(z => z.LETTER_ID).FirstOrDefault();
                    }
                }
                else if (Request.Params["userType"].Equals("I"))//NQM - Independent Monitor
                {
                    if (isLettterId)
                    {
                        letterDetails = dbContext.QUALITY_QM_LETTER.Find(Convert.ToInt32(Request.Params["id"]));
                    }
                    else
                    {
                        int scheduleCode = Convert.ToInt32(Request.Params["id"]);
                        letterDetails = dbContext.QUALITY_QM_LETTER.Where(c => c.ADMIN_SCHEDULE_CODE == scheduleCode).OrderByDescending(z => z.LETTER_ID).FirstOrDefault();
                    }
                }

                string filePath = letterDetails.TYPE.Equals("S") ? ConfigurationManager.AppSettings["QUALITY_QM_LETTER_SQC"].ToString() + letterDetails.FILE_NAME : ConfigurationManager.AppSettings["QUALITY_QM_LETTER_NQM"].ToString() + letterDetails.FILE_NAME;

                if (System.IO.File.Exists(filePath))
                {
                    return File(filePath, "Application/pdf", DateTime.Now.ToShortDateString() + "_" + new Random().Next(1000000000) + ".pdf");
                }
                else
                {
                    return Json(new { Error = "File Not Exist" }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "QualityMonitoring.DownloadLetter()");
                return Json(new { Success = "false" }, JsonRequestBehavior.AllowGet);
            }
            finally
            {
                dbContext.Dispose();
            }
        }


        /// <summary>
        /// Send Mail to SQC & NQM
        /// </summary>
        /// <returns></returns>
        public ActionResult SendLetter()
        {
            QMLetterModel qmLetterModel = new QMLetterModel();
            String ErrorMessage = "";
            QualityMonitoringBAL qmBAL = new QualityMonitoringBAL();
            dbContext = new PMGSYEntities();
            try
            {
                if (Request.Params["userType"] != null)
                {
                    if (Request.Params["userType"].Equals("S"))
                    {
                        qmLetterModel.QC_CODE = Convert.ToInt32(Request.Params["userId"]);
                        qmLetterModel.QC_TYPE = Request.Params["userType"];
                        qmLetterModel.LETTER_ID = Convert.ToInt32(Request.Params["letterId"]);
                        // Commented by Srishti on 13-03-2023
                        //qmBAL.SendMailCustomFunc(qmLetterModel, ref ErrorMessage).Send();

                        // Added by Srishti on 13-03-2023
                        ServicePointManager.ServerCertificateValidationCallback += (s, ce, ca, p) => true;
                        ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                        MailMessage ms = qmBAL.SendMailCustomFunc(qmLetterModel, ref ErrorMessage);

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

                        return Json(new { Success = "true", Message = "Email Sent Successfully." }, JsonRequestBehavior.AllowGet);
                    }
                    else if (Request.Params["userType"].Equals("I"))
                    {
                        qmLetterModel.SCHEDULE_CODE = Convert.ToInt32(Request.Params["scheduleCode"]);
                        qmLetterModel.QC_TYPE = Request.Params["userType"];

                        if (!(dbContext.QUALITY_QM_TEAM.Where(c => c.ADMIN_SCHEDULE_CODE == qmLetterModel.SCHEDULE_CODE).Any()))
                        {
                            // Commented by Srishti on 13-03-2023
                            //qmBAL.SendMailCustomFunc(qmLetterModel, ref ErrorMessage).Send();

                            // Added by Srishti on 13-03-2023
                            ServicePointManager.ServerCertificateValidationCallback += (s, ce, ca, p) => true;
                            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                            MailMessage mailMessage = qmBAL.SendMailCustomFunc(qmLetterModel, ref ErrorMessage);

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
                            client.Send(mailMessage);

                            return Json(new { Success = "true", Message = "Email Sent Successfully." }, JsonRequestBehavior.AllowGet);
                        }
                        else
                        {
                            return Json(new { Success = "false", Message = "Mail not sent because Monitor is a member of team inspection." }, JsonRequestBehavior.AllowGet);
                        }
                    }
                }

                return Json(new { Success = "false", Message = "Error occurred while processing request" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "QualityMonitoring.SendLetter()");
                //return Json(new { Success = "false", Message = ex.Message }, JsonRequestBehavior.AllowGet);
                return Json(new { Success = "false", Message = ex.ToString() }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// Populate Monitors for Schedule Creation
        /// </summary>
        /// <param name="month"></param>
        /// <param name="year"></param>
        /// <returns></returns>
        [Audit]
        public ActionResult GetMonitors(int month, int year)
        {
            try
            {
                if (PMGSYSession.Current.RoleCode == 5 || PMGSYSession.Current.RoleCode == 9)  //CQC
                {
                    return Json(new CommonFunctions().PopulateMonitorsForScheduleCreation(String.Empty, "I", 0, month, year));
                }
                else if (PMGSYSession.Current.RoleCode == 8 || PMGSYSession.Current.RoleCode == 69 || PMGSYSession.Current.RoleCode == 48)  //SQC
                {
                    return Json(new CommonFunctions().PopulateMonitorsForScheduleCreation(String.Empty, "S", PMGSYSession.Current.StateCode, month, year));
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return null;
            }
        }


        /// <summary>
        /// Post to Create new Schedule
        /// </summary>
        /// <param name="qmScheduleModel"></param>
        /// <returns></returns>
        [HttpPost]
        [Audit]
        public ActionResult QMCreateSchedule(QMScheduleViewModel qmScheduleModel)
        {
            CommonFunctions objCommonFunctions = new CommonFunctions();
            qualityBAL = new QualityMonitoringBAL();
            dbContext = new PMGSYEntities();
            if (ModelState.IsValid)
            {
                #region proficinecy test score check & block user to schedule
                //   int yearFrom = qmScheduleModel.ADMIN_IM_YEAR - 2;
                //DateTime dateFrom = Convert.ToDateTime("01/" + qmScheduleModel.ADMIN_IM_MONTH + "/" + yearFrom);

                var qmtestScoreList = (from QPTS in dbContext.QM_PROFICIENCY_TEST_SCORE
                                       join QPTM in dbContext.QM_PROFICIENCY_TEST_MASTER on QPTS.EXAM_ID equals QPTM.EXAM_ID
                                       where
                                            QPTS.ADMIN_QM_CODE == qmScheduleModel.ADMIN_QM_CODE &&
                                            QPTM.IS_FINALIZED == "Y" &&
                                           //  QPTS.EXAM_STATUS != "NA" &&
                                           QPTS.EXAM_STATUS == "P"
                                       //&& QPTM.DATE_OF_EXAM >= dateFrom
                                       select new
                                       {
                                           // ID = QPTS.ID,
                                           MARKS = QPTS.MARKS,
                                           // PERCENTAGE = QPTS.PERCENTAGE,
                                           EXAM_STATUS = QPTS.EXAM_STATUS,
                                           EXAM_ID = QPTS.EXAM_ID,
                                           ADMIN_QM_CODE = QPTS.ADMIN_QM_CODE,
                                           // IS_LATEST = QPTS.IS_LATEST,
                                           DATE_OF_EXAM = QPTM.DATE_OF_EXAM,
                                           // DATE_OF_FINALIZATION = QPTM.DATE_OF_FINALIZATION
                                       }).OrderByDescending(x => x.DATE_OF_EXAM).ThenByDescending(x => x.EXAM_ID).FirstOrDefault();

                #region old condtions

                ////If exam is taken for only one time & qm is fail
                //if (qmtestScoreList.Count == 1)
                //{
                //    if (qmtestScoreList.Any(s => s.EXAM_STATUS == "P" && s.IS_LATEST == "Y" && s.MARKS < 21))
                //    {
                //        return Json(new { Success = false, ErrorMessage = "Not available for assignment based on performance in proficiency test" });

                //    }
                //}


                //if (qmtestScoreList.Count > 1)
                //{
                //    //if exam is (not given/absent/not registered) in last two years by QM
                //    if (!qmtestScoreList.Any(s => s.EXAM_STATUS == "P"))
                //    {
                //        return Json(new { Success = false, ErrorMessage = "Not available for assignment based on performance in proficiency test" });
                //    }

                //    //if latest one is failed                           
                //    if (qmtestScoreList.Any(s => s.EXAM_STATUS == "P" && s.IS_LATEST == "Y" && s.MARKS < 21))
                //    {
                //        return Json(new { Success = false, ErrorMessage = "Not available for assignment based on performance in proficiency test" });

                //    }

                //    //if latest two exam are not registered or absent
                //    int latestRecCount = 0;
                //    int countNTRorAb = 0;
                //    foreach (var item in qmtestScoreList)
                //    {
                //        if (latestRecCount == 2)
                //        {
                //            break;
                //        }

                //        if (item.EXAM_STATUS == "NRT" || item.EXAM_STATUS == "AB")
                //        {
                //            countNTRorAb++;
                //        }
                //        else
                //        {
                //            break;
                //        }
                //        latestRecCount++;
                //    }
                //    if (countNTRorAb == 2)
                //    {
                //        return Json(new { Success = false, ErrorMessage = "Not available for assignment based on performance in proficiency test" });
                //    }

                //    //if latest is NTR or absent & second last is fail 
                //    latestRecCount = 0;
                //    int countNTRorAbFail = 0;
                //    foreach (var item in qmtestScoreList)
                //    {
                //        if (latestRecCount == 2)
                //        {
                //            break;
                //        }
                //        if (latestRecCount == 0)
                //        {
                //            if (item.EXAM_STATUS == "NRT" || item.EXAM_STATUS == "AB")
                //            {
                //                countNTRorAbFail++;
                //            }
                //            else
                //            {
                //                break;
                //            }
                //        }
                //        if (latestRecCount == 1)
                //        {
                //            if (item.EXAM_STATUS == "P" && item.MARKS < 21)
                //            {
                //                countNTRorAbFail++;
                //            }
                //        }
                //        latestRecCount++;
                //    }


                //    if (countNTRorAbFail == 2)
                //    {
                //        return Json(new { Success = false, ErrorMessage = "Not available for assignment based on performance in proficiency test" });
                //    }


                //}


                #endregion



                #region new condtions

                if (qmtestScoreList != null)
                {
                    //if latest one is failed                           
                    if (qmtestScoreList.MARKS < 21)
                    {
                        return Json(new { Success = false, ErrorMessage = "Not available for assignment based on performance in proficiency test" });
                    }
                }

                #endregion

                // return Json(new { Success = false, ErrorMessage = "For tseting purpose(CODE WILL REDIRECTED TO SCHDEULE CREATION)" });
                #endregion
                string Status = qualityBAL.QMCreateScheduleBAL(qmScheduleModel);
                if (Status == string.Empty)
                    return Json(new { Success = true });
                else
                    return Json(new { Success = false, ErrorMessage = Status });
            }
            else
            {
                StringBuilder errorMessages = new StringBuilder();
                foreach (var modelStateValue in ModelState.Values)
                {
                    foreach (var error in modelStateValue.Errors)
                    {
                        errorMessages.Append(error.ErrorMessage);
                    }
                }
                return Json(new { Success = false, ErrorMessage = errorMessages.ToString() });
            }

        }



        /// <summary>
        /// Display Monitor Details
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Audit]
        public ActionResult MonitorDetails(string id)
        {
            qualityBAL = new QualityMonitoringBAL();
            try
            {
                MasterAdminQualityMonitorViewModel masterQualityMonitorViewMode = qualityBAL.MonitorDetailsBAL(Convert.ToInt32(id));
                return View(masterQualityMonitorViewMode);
            }
            catch (Exception ex)
            {
                //Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                ErrorLog.LogError(ex, "QM.MonitorDetails()");
                return null;
            }
        }


        /// <summary>
        /// Get To Add Districts to Schedule
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Audit]
        public ActionResult CQCAddDistrict(string id)
        {
            qualityBAL = new QualityMonitoringBAL();
            try
            {
                QMCQCAddDistrictModel qmCQCAddDistrict = new QMCQCAddDistrictModel();
                qmCQCAddDistrict.DISTRICTS = qualityBAL.GetScheduledDistrictListBAL(Convert.ToInt32(id), false);
                qmCQCAddDistrict.ADMIN_SCHEDULE_CODE = Convert.ToInt32(id);
                return View(qmCQCAddDistrict);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return null;
            }
        }


        /// <summary>
        /// Post to add District to Schedule
        /// </summary>
        /// <param name="qmCQCAddDistrict"></param>
        /// <returns></returns>
        [HttpPost]
        [Audit]
        public ActionResult CQCAddDistricts(QMCQCAddDistrictModel qmCQCAddDistrict)
        {
            qualityBAL = new QualityMonitoringBAL();
            try
            {
                if (ModelState.IsValid)
                {
                    string Status = qualityBAL.CQCAddDistrictsBAL(qmCQCAddDistrict);
                    if (Status == string.Empty)
                        return Json(new { Success = true });
                    else
                        return Json(new { Success = false, ErrorMessage = Status });
                }
                else
                {
                    StringBuilder errorMessages = new StringBuilder();
                    foreach (var modelStateValue in ModelState.Values)
                    {
                        foreach (var error in modelStateValue.Errors)
                        {
                            errorMessages.Append(error.ErrorMessage);
                        }
                    }

                    return Json(new { Success = false, ErrorMessage = errorMessages.ToString() });
                }
            }
            catch (DbEntityValidationException e)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(e, HttpContext.ApplicationInstance.Context);
                foreach (var eve in e.EntityValidationErrors)
                {
                    foreach (var ve in eve.ValidationErrors)
                    {
                        ModelState.AddModelError("", eve.ValidationErrors.ToString());
                    }
                }
            }

            qmCQCAddDistrict.DISTRICTS = qualityBAL.GetScheduledDistrictListBAL(qmCQCAddDistrict.ADMIN_SCHEDULE_CODE, false);
            qmCQCAddDistrict.ADMIN_SCHEDULE_CODE = qmCQCAddDistrict.ADMIN_SCHEDULE_CODE;
            return View(qmCQCAddDistrict);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// []
        [Audit]
        public ActionResult QMAssignRoads(string id)
        {
            qualityBAL = new QualityMonitoringBAL();
            CommonFunctions objCommonFunctions = new CommonFunctions();
            try
            {
                QMAssignRoadsModel qmAssignRoads = new QMAssignRoadsModel();
                qmAssignRoads.ROAD_STATUS = "A";
                qmAssignRoads.DISTRICTS = qualityBAL.GetScheduledDistrictListBAL(Convert.ToInt32(id), true);
                if (PMGSYSession.Current.RoleCode == 22 || PMGSYSession.Current.RoleCode == 38)        //For PIU
                    qmAssignRoads.YEARS = objCommonFunctions.PopulateFinancialYear(false, false).Where(a => a.Value != "0").ToList();
                else
                    qmAssignRoads.YEARS = new SelectList(new List<SelectListItem>().AsEnumerable<SelectListItem>(), "Value", "Text").ToList();  //objCommonFunctions.PopulateYears(true);

                qmAssignRoads.ROAD_STATUS_LIST = objCommonFunctions.PopulateRoadStatus();
                qmAssignRoads.ADMIN_SCHEDULE_CODE = Convert.ToInt32(id);

                List<SelectListItem> lstTechnology = new List<SelectListItem>(); // Added by Shreyas
                lstTechnology.Insert(0, (new SelectListItem { Text = "All", Value = "0", Selected = true }));
                lstTechnology.Insert(1, (new SelectListItem { Text = "FDR", Value = "1" }));
                ViewBag.lstTechnology = lstTechnology;


                return View(qmAssignRoads);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return null;
            }
        }


        /// <summary>
        /// Post for Assign Roads to particular schedule as per selected Districts & Sanction Year
        /// </summary>
        /// <param name="prRoadCode"></param>
        /// <param name="adminSchCode"></param>
        /// <returns></returns>
        //[HttpPost]
        //[Audit]
        //public ActionResult QMAssignRoads()
        //{
        //    qualityBAL = new QualityMonitoringBAL();
        //    QMAssignRoadsModel qmAssignRoadsModel = new QMAssignRoadsModel();
        //    try
        //    {
        //        Int32 prRoadCode = Convert.ToInt32(Request.Params["prRoadCode"]);
        //        Int32 scheduleCode = Convert.ToInt32(Request.Params["adminSchCode"]);
        //        String isEnquiry = Request.Params["isEnquiry"];

        //        if (prRoadCode != 0)
        //        {
        //            string Status = qualityBAL.QMAssignRoadsBAL(prRoadCode, scheduleCode, isEnquiry);
        //            if (Status == string.Empty)
        //                return Json(new { Success = true });
        //            else
        //                return Json(new { Success = false, ErrorMessage = Status });
        //        }
        //        else
        //        {
        //            return Json(new { Success = false, ErrorMessage = "An Error Occurred While Processing Your Request." });
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
        //        return Json(new { Success = false, ErrorMessage = "An Error Occurred While Processing Your Request." });
        //    }
        //}


        /// <summary>
        /// Assign Selected works by CQCAdmin/SQC/NQM/SQM/PIU to Schedule
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Audit]
        public ActionResult QMAssignWorks()
        {
            qualityBAL = new QualityMonitoringBAL();
            QMAssignRoadsModel qmAssignRoadsModel = new QMAssignRoadsModel();
            try
            {
                if (Request.Params["arrWorks[]"] == null || Request.Params["arrWorks[]"].Equals(""))
                {
                    return Json(new { Success = false, ErrorMessage = "Please select atleast one of the works." });
                }

                String[] arrWorks = Request.Params["arrWorks[]"].Split(',');
                Int32 scheduleCode = Convert.ToInt32(Request.Params["adminSchCode"]);
                String[] arrEnquiry = null;
                if (Request.Params["arrEnquiry[]"] != null && !Request.Params["arrEnquiry[]"].Equals(""))
                {
                    arrEnquiry = Request.Params["arrEnquiry[]"].Split(',');
                }

                string Status = qualityBAL.QMAssignWorksBAL(arrWorks, scheduleCode, arrEnquiry);
                if (Status == string.Empty)
                    return Json(new { Success = true });
                else
                    return Json(new { Success = false, ErrorMessage = Status });

            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return Json(new { Success = false, ErrorMessage = "An Error Occurred While Processing Your Request." });
            }
        }


        /// <summary>
        /// Populate District as per selected state
        /// </summary>
        /// <param name="selectedState"></param>
        /// <returns></returns>
        [Audit]
        public ActionResult PopulateYears()
        {
            CommonFunctions objCommonFunctions = new CommonFunctions();
            try
            {
                return Json(objCommonFunctions.PopulateFinancialYear(true, false).ToList());
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return null;
            }
        }


        /// <summary>
        /// Listing of Roads in selcted Districts & Sanction Year to be assigned
        /// </summary>
        /// <param name="formCollection"></param>
        /// <param name="districtCode"></param>
        /// <param name="sanctionYear"></param>
        /// <param name="rdStatus"></param>
        /// <param name="adminSchCode"></param>
        /// <returns></returns>
        [HttpPost]
        [Audit]
        public ActionResult GetRoadListToAssign(FormCollection formCollection)
        {
            qualityBAL = new QualityMonitoringBAL();
            int totalRecords;
            try
            {
                //Adde By Abhishek kamble 29-Apr-2014 start
                using (CommonFunctions commonFunction = new CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new GridParams(Convert.ToInt32(formCollection["page"]), Convert.ToInt32(formCollection["rows"]), formCollection["sidx"], formCollection["sord"], Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                    {
                        return null;
                    }
                }
                //Adde By Abhishek kamble 29-Apr-2014 end
                int distCode = Convert.ToInt32(formCollection["districtCode"]);
                int techCode = Convert.ToInt32(formCollection["techid"]);
                var jsonData = new
                {
                    rows = qualityBAL.GetRoadListToAssignBAL(Convert.ToInt32(formCollection["page"]) - 1, Convert.ToInt32(formCollection["rows"]),
                                                            formCollection["sidx"], formCollection["sord"], out totalRecords, formCollection["filters"],
                                                            Convert.ToInt32(formCollection["districtCode"]), Convert.ToInt32(formCollection["adminSchCode"]), Convert.ToInt32(formCollection["techid"])),
                    total = totalRecords <= Convert.ToInt32(formCollection["rows"]) ? 1 : (totalRecords % Convert.ToInt32(formCollection["rows"]) == 0 ? totalRecords / Convert.ToInt32(formCollection["rows"]) : (totalRecords / Convert.ToInt32(formCollection["rows"]) + 1)),
                    page = Convert.ToInt32(formCollection["page"]),
                    records = totalRecords
                };
                return Json(jsonData);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return Json(String.Empty);
            }
        }



        /// <summary>
        /// View Physical Progress Of Road
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult ViewPhysicalProgress(string id)
        {
            QMAssignRoadsModel qmAssignRoads = new QMAssignRoadsModel();
            qmAssignRoads.IMS_PR_ROAD_CODE = Convert.ToInt32(id);
            return View(qmAssignRoads);
        }


        /// <summary>
        /// Physical Progress Of Road
        /// </summary>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <param name="sidx"></param>
        /// <param name="sord"></param>
        /// <returns></returns>
        public ActionResult GetRoadPhysicalProgressList(int? page, int? rows, string sidx, string sord)
        {
            qualityBAL = new QualityMonitoringBAL();
            long totalRecords = 0;
            int imsRoadCode = 0;

            if (!(string.IsNullOrEmpty(Request.Params["prRoadCode"])))
            {
                imsRoadCode = Convert.ToInt32(Request.Params["prRoadCode"]);
            }

            var jsonData = new
            {
                rows = qualityBAL.GetRoadPhysicalProgressList(Convert.ToInt32(page) - 1, Convert.ToInt32(rows), sidx, sord, out totalRecords, imsRoadCode),
                total = totalRecords <= Convert.ToInt32(rows) ? 1 : totalRecords / Convert.ToInt32(rows) + 1,
                page = Convert.ToInt32(page),
                records = totalRecords,
            };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// Render view for District Wise Schedules
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Audit]
        public ActionResult QMDistrictwiseSchDetails(string id)
        {
            dbContext = new PMGSYEntities();

            try
            {
                String[] urlSplitParams = id.Split('$');
                Int32 scheduleCode = Convert.ToInt32(urlSplitParams[0]);
                QMDistrictwiseSchDetailsModel model = new QMDistrictwiseSchDetailsModel();

                QUALITY_QM_SCHEDULE qualityQMSchedule = dbContext.QUALITY_QM_SCHEDULE.Find(scheduleCode);
                model.INSP_STATUS_FLAG = qualityQMSchedule.INSP_STATUS_FLAG;
                model.ADMIN_SCHEDULE_CODE = scheduleCode;
                return View(model);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return null;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        /// <summary>
        /// View District Wise Schedule details for Finalization
        /// </summary>
        /// <param name="formCollection"></param>
        /// <param name="districtCode"></param>
        /// <param name="sanctionYear"></param>
        /// <param name="rdStatus"></param>
        /// <param name="adminSchCode"></param>
        /// <returns></returns>
        [HttpPost]
        [Audit]
        public ActionResult QMViewScheduleDetails(FormCollection formCollection)
        {
            qualityBAL = new QualityMonitoringBAL();
            int totalRecords;
            try
            {
                var jsonData = new
                {
                    rows = qualityBAL.QMViewScheduleDetailsBAL(Convert.ToInt32(formCollection["page"]) - 1, Convert.ToInt32(formCollection["rows"]),
                                                            formCollection["sidx"], formCollection["sord"], out totalRecords,
                                                            Convert.ToInt32(formCollection["adminSchCode"])),
                    total = totalRecords <= Convert.ToInt32(formCollection["rows"]) ? 1 : (totalRecords % Convert.ToInt32(formCollection["rows"]) == 0 ? totalRecords / Convert.ToInt32(formCollection["rows"]) : (totalRecords / Convert.ToInt32(formCollection["rows"]) + 1)),
                    page = Convert.ToInt32(formCollection["page"]),
                    records = totalRecords
                };
                return Json(jsonData);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return Json(String.Empty);
            }
        }


        /// <summary>
        /// Delete assigned roads in schedule
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="hash"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        [HttpPost]
        [Audit]
        public ActionResult QMDeleteSchRoads()
        {
            try
            {
                qualityBAL = new QualityMonitoringBAL();
                Int32 prRoadCode = Convert.ToInt32(Request.Params["prRoadCode"]);
                Int32 scheduleCode = Convert.ToInt32(Request.Params["adminSchCode"]);
                if (prRoadCode != 0)
                {
                    string Status = qualityBAL.QMDeleteSchRoadsBAL(prRoadCode, scheduleCode);
                    if (Status == string.Empty)
                        return Json(new { Success = true });
                    else
                        return Json(new { Success = false, ErrorMessage = Status });
                }
                else
                {
                    return Json(new { Success = false, ErrorMessage = "An Error Occurred While Processing Your Request." });
                }
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return Json(new { Success = false, ErrorMessage = "An Error Occurred While Processing Your Request." });
            }

        }




        [HttpPost]
        [Audit]
        public ActionResult QMDeleteSelectedRoads(List<QMDeleteSelectedArrayModel> submitArray)
        {
            string errString = "";
            try
            {
                bool flag = true;
                qualityBAL = new QualityMonitoringBAL();
                //Int32 prRoadCode = Convert.ToInt32(Request.Params["prRoadCode"]);
                //Int32 scheduleCode = Convert.ToInt32(Request.Params["adminSchCode"]);

                List<QMDeleteSelectedArrayModel> arr = submitArray;

                //vikky 23-06-2022
                PMGSYEntities dbContext = new PMGSYEntities();
                List<QMDeleteSelectedArrayModel> arrTemp = new List<QMDeleteSelectedArrayModel>();

                foreach (QMDeleteSelectedArrayModel item in submitArray)
                {
                    int eformId = dbContext.EFORM_MASTER.Where(s => s.ADMIN_SCHEDULE_CODE == item.scheduleCode && s.IMS_PR_ROAD_CODE == item.prRoadCode).OrderByDescending(m => m.EFORM_ID).Select(m => m.EFORM_ID).FirstOrDefault();
                    item.uploadStatus = "N";
                    if (eformId != 0)
                    {
                        if (dbContext.EFORM_PDF_UPLOAD_DETAIL.Any(x => x.EFORM_ID == eformId))
                        {
                            item.uploadStatus = "Y";
                        }
                        eformId = 0;
                    }


                }
                arrTemp = arr.OrderBy(m => m.uploadStatus).ToList();



                foreach (QMDeleteSelectedArrayModel item in arrTemp)
                {
                    //if (item.prRoadCode != 0)
                    //{

                    string Status = qualityBAL.QMDeleteSchRoadsBAL(item.prRoadCode, item.scheduleCode);

                    if (Status != string.Empty)
                    {
                        flag = false;
                        //   errString = "<br>"+errString + Status;
                        errString = errString + Status;
                    }
                    //else
                    //{
                    //    return Json(new { Success = false, ErrorMessage = "An Error Occurred While Processing Your Request." });
                    //}
                }
                return Json(new { Success = flag, ErrorMessage = errString });
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return Json(new { Success = false, ErrorMessage = "An Error Occurred While Processing Your Request." });
            }

        }


        /// <summary>
        /// Get To Delete Districts in Schedule
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Audit]
        public ActionResult QMDeleteDistrict(string id)
        {
            qualityBAL = new QualityMonitoringBAL();
            try
            {
                QMCQCAddDistrictModel qmCQCAddDistrict = new QMCQCAddDistrictModel();
                qmCQCAddDistrict.DISTRICTS = qualityBAL.GetScheduledDistrictListBAL(Convert.ToInt32(id), true);
                qmCQCAddDistrict.ADMIN_SCHEDULE_CODE = Convert.ToInt32(id);
                return View(qmCQCAddDistrict);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return null;
            }
        }


        /// <summary>
        /// Post to Delete District, This method is for CQC Only
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="hash"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        [HttpPost]
        [Audit]
        public ActionResult QMDeleteDistricts(QMCQCAddDistrictModel qmCQCAddDistrict)
        {
            string PhysicalPath = string.Empty;
            string QM_FILE_NAME = string.Empty;
            try
            {
                qualityBAL = new QualityMonitoringBAL();
                if (ModelState.IsValid)
                {
                    Int32 scheduleCode = qmCQCAddDistrict.ADMIN_SCHEDULE_CODE;
                    Int32 districtCode = qmCQCAddDistrict.MAST_DISTRICT_CODE;

                    dbContext = new PMGSYEntities();
                    QM_FILE_NAME = dbContext.QUALITY_QM_TEAM.Where(x => x.ADMIN_SCHEDULE_CODE == scheduleCode).Select(s => s.QM_TEAM_CODE).FirstOrDefault();



                    string Status = qualityBAL.CQCDeleteDistrictBAL(districtCode, scheduleCode);
                    if (Status == string.Empty)
                    {
                        if (!(string.IsNullOrEmpty(QM_FILE_NAME)))
                        {
                            PhysicalPath = ConfigurationManager.AppSettings["QUALITY_QM_LETTER_TEAM"];
                            PhysicalPath = Path.Combine(PhysicalPath, QM_FILE_NAME);
                            if (System.IO.File.Exists(PhysicalPath))
                            {
                                System.IO.File.Delete(PhysicalPath);
                            }
                        }
                        return Json(new { Success = true });
                    }
                    else
                        return Json(new { Success = false, ErrorMessage = Status });
                }
                else
                {
                    StringBuilder errorMessages = new StringBuilder();
                    foreach (var modelStateValue in ModelState.Values)
                    {
                        foreach (var error in modelStateValue.Errors)
                        {
                            errorMessages.Append(error.ErrorMessage);
                        }
                    }
                    return Json(new { Success = false, ErrorMessage = errorMessages.ToString() });
                }
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return Json(new { Success = false, ErrorMessage = "An Error Occurred While Processing Your Request." });
            }
        }


        /// <summary>
        /// Delete Schedule for SQC
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        [Audit]
        public ActionResult QMDeleteSchedule()
        {
            try
            {
                qualityBAL = new QualityMonitoringBAL();
                Int32 scheduleCode = Convert.ToInt32(Request.Params["adminSchCode"]);

                if (scheduleCode != 0)
                {
                    string Status = qualityBAL.CQCDeleteDistrictBAL(0, scheduleCode);
                    if (Status == string.Empty)
                        return Json(new { Success = true });
                    else
                        return Json(new { Success = false, ErrorMessage = Status });
                }
                else
                {
                    return Json(new { Success = false, ErrorMessage = "An Error Occurred While Processing Your Request." });
                }
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return Json(new { Success = false, ErrorMessage = "An Error Occurred While Processing Your Request." });
            }
        }


        /// <summary>
        /// Finalize Districts in Schedule
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Audit]
        public ActionResult FinalizeDistricts()
        {
            dbContext = new PMGSYEntities();
            try
            {
                qualityBAL = new QualityMonitoringBAL();
                Int32 scheduleCode = Convert.ToInt32(Request.Params["adminSchCode"]);

                if (scheduleCode != 0)
                {
                    string Status = qualityBAL.FinalizeDistrictsBAL(Convert.ToInt32(scheduleCode));
                    if (Status == string.Empty)
                    {
                        // var scheduleDetails = dbContext.QUALITY_QM_SCHEDULE.Find(Convert.ToInt32(scheduleCode));
                        // Message = scheduleDetails.ADMIN_QM_CODE + "$" + scheduleDetails.ADMIN_IM_MONTH + "$" + scheduleDetails.ADMIN_IM_YEAR 
                        return Json(new { Success = true });
                    }
                    else
                        return Json(new { Success = false, ErrorMessage = Status });
                }
                else
                {
                    return Json(new { Success = false, ErrorMessage = "An Error Occurred While Processing Your Request." });
                }
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return Json(new { Success = false, ErrorMessage = "An Error Occurred While Processing Your Request." });
            }
            finally
            {
                dbContext.Dispose();
            }

        }

        /// <summary>
        /// Finalize Roads
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Audit]
        public ActionResult FinalizeRoad()
        {
            try
            {
                qualityBAL = new QualityMonitoringBAL();
                Int32 scheduleCode = Convert.ToInt32(Request.Params["adminSchCode"]);
                Int32 prRoadCode = Convert.ToInt32(Request.Params["prRoadCode"]);
                bool isFinalizeAllRoads = Convert.ToBoolean(Request.Params["isFinalizeAllRoads"]);

                if (scheduleCode != 0)
                {
                    string Status = qualityBAL.FinalizeRoadBAL(prRoadCode, scheduleCode, isFinalizeAllRoads);
                    if (Status == string.Empty)
                        return Json(new { Success = true });
                    else
                        return Json(new { Success = false, ErrorMessage = Status });
                }
                else
                {
                    return Json(new { Success = false, ErrorMessage = "An Error Occurred While Processing Your Request." });
                }
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return Json(new { Success = false, ErrorMessage = "An Error Occurred While Processing Your Request." });
            }

        }


        /// <summary>
        /// Forward Schedule to NQM/SQM
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Audit]
        public ActionResult ForwardSchedule()
        {
            try
            {
                qualityBAL = new QualityMonitoringBAL();
                Int32 scheduleCode = Convert.ToInt32(Request.Params["adminSchCode"]);

                if (scheduleCode != 0)
                {
                    string Status = qualityBAL.ForwardScheduleBAL(Convert.ToInt32(scheduleCode));
                    if (Status == string.Empty)
                        return Json(new { Success = true });
                    else
                        return Json(new { Success = false, ErrorMessage = Status });
                }
                else
                {
                    return Json(new { Success = false, ErrorMessage = "An Error Occurred While Processing Your Request." });
                }
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return Json(new { Success = false, ErrorMessage = "An Error Occurred While Processing Your Request." });
            }

        }

        //[HttpPost]
        //[Audit]
        //public ActionResult UnlockSchedule()
        //{
        //    try
        //    {
        //        qualityBAL = new QualityMonitoringBAL();
        //        Int32 scheduleCode = Convert.ToInt32(Request.Params["adminSchCode"]);

        //        if (scheduleCode != 0)
        //        {
        //            string Status = qualityBAL.UnlockScheduleBAL(Convert.ToInt32(scheduleCode));
        //            if (Status == string.Empty)
        //                return Json(new { Success = true });
        //            else
        //                return Json(new { Success = false, ErrorMessage = Status });
        //        }
        //        else
        //        {
        //            return Json(new { Success = false, ErrorMessage = "An Error Occurred While Processing Your Request." });
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
        //        return Json(new { Success = false, ErrorMessage = "An Error Occurred While Processing Your Request." });
        //    }

        //}

        public ActionResult UnlockSchedule()
        {
            Int32 scheduleCode = Convert.ToInt32(Request.Params["adminSchCode"]);
            qualityMonitoringHelpDeskBAL = new QualityMonitoringHelpDeskBAL();

            try
            {
                dbContext = new PMGSYEntities();

                QUALITY_QM_SCHEDULE qmDetails = dbContext.QUALITY_QM_SCHEDULE.Where(m => m.ADMIN_SCHEDULE_CODE == scheduleCode).FirstOrDefault();
                int adminQMcODE = (
                                     from result1 in dbContext.QUALITY_QM_SCHEDULE
                                     where result1.ADMIN_SCHEDULE_CODE == scheduleCode
                                     select result1.ADMIN_QM_CODE
                                    ).First();
                string adminQMType = (
                                       from result2 in dbContext.ADMIN_QUALITY_MONITORS
                                       where result2.ADMIN_QM_CODE == adminQMcODE
                                       select result2.ADMIN_QM_TYPE
                                     ).First();

                int getSelectedMonth = Convert.ToInt32(qmDetails.ADMIN_IM_MONTH);
                string getSelectedYear = qmDetails.ADMIN_IM_YEAR.ToString();
                string getSelectedQmType = adminQMType;
                string rowID = qmDetails.ADMIN_QM_CODE.ToString();

                int result = qualityMonitoringHelpDeskBAL.QMDefinalizeScheduleBAL(Convert.ToInt32(adminQMcODE), Convert.ToInt32(getSelectedYear), getSelectedMonth, getSelectedQmType);

                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(string.Empty, JsonRequestBehavior.AllowGet);
            }


        }

        /// <summary>
        /// View Inspection Details
        /// </summary>
        /// <param name="formCollection"></param>
        /// <returns></returns>
        [HttpPost]
        [Audit]
        public ActionResult QMViewInspectionDetails(FormCollection formCollection)
        {
            qualityBAL = new QualityMonitoringBAL();
            int totalRecords;
            try
            {
                //Adde By Abhishek kamble 29-Apr-2014 start
                using (CommonFunctions commonFunction = new CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new GridParams(Convert.ToInt32(formCollection["page"]), Convert.ToInt32(formCollection["rows"]), formCollection["sidx"], formCollection["sord"], Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                    {
                        return null;
                    }
                }
                //Adde By Abhishek kamble 29-Apr-2014 end
                //SACHIN
                var jsonData = new
                {
                    rows = qualityBAL.QMViewInspectionDetailsBAL(Convert.ToInt32(formCollection["page"]) - 1, Convert.ToInt32(formCollection["rows"]),
                                                            formCollection["sidx"], formCollection["sord"], out totalRecords,
                                                            Convert.ToInt32(formCollection["stateCode"]), Convert.ToInt32(formCollection["monitorCode"]),
                                                            Convert.ToInt32(formCollection["fromInspMonth"]), Convert.ToInt32(formCollection["fromInspYear"]),
                                                            Convert.ToInt32(formCollection["toInspMonth"]), Convert.ToInt32(formCollection["toInspYear"]), Convert.ToInt32(formCollection["schemeType"]), formCollection["roadStatus"], formCollection["roadOrBridge"], formCollection["gradeType"], formCollection["eFormStatusType"]), //add parameter on 30-06-2022 by vikky
                    total = totalRecords <= Convert.ToInt32(formCollection["rows"]) ? 1 : (totalRecords % Convert.ToInt32(formCollection["rows"]) == 0 ? totalRecords / Convert.ToInt32(formCollection["rows"]) : (totalRecords / Convert.ToInt32(formCollection["rows"]) + 1)),
                    page = Convert.ToInt32(formCollection["page"]),
                    records = totalRecords
                };
                return Json(jsonData);
            }
            catch (Exception ex)
            {
                //Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                ErrorLog.LogError(ex, "QMViewInspectionDetails()");
                return Json(String.Empty);
            }
        }

        public ActionResult QMViewInspectionListAgainstRoadNewTab(String roadCode1)
        {
            QualityMonitoringDAL qmDAL = new QualityMonitoringDAL();
            QMATRInspdetailsModel inspDetailsModel = new QMATRInspdetailsModel();
            inspDetailsModel.INSP_AGAINST_ROAD_LIST = new List<QMATRINSPModel>();

            dbContext = new PMGSYEntities();

            try
            {

                string[] encParam = roadCode1.Split('/');
                decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { encParam[0], encParam[1], encParam[2] });

                int roadCode = 0;
                if (decryptedParameters.Count > 0)
                {
                    roadCode = Convert.ToInt32(decryptedParameters["imsRoadID"].Split('$')[0]);

                }




                // int roadCode = Convert.ToInt32(roadCode1.Split('$')[0]);

                inspDetailsModel.INSP_LIST = qmDAL.ATRInspDetailsDAL(roadCode);//ATR_Chang


                var inspectionList = inspDetailsModel.INSP_LIST.ToList();





                foreach (var item in inspectionList)
                {
                    QMATRINSPModel obsModel = new QMATRINSPModel();
                    obsModel.ADMIN_QM_TYPE = item.ADMIN_QM_TYPE == "S" ? "SQM" : "NQM";
                    obsModel.ATR_VERIFICATION_FINALIZED = item.ATR_VERIFICATION_FINALIZED == "Y" ? "Yes" : "No";
                    obsModel.MONITOR_NAME = item.MONITOR_NAME.ToString().Split('-')[0];
                    obsModel.STATE_NAME = item.STATE_NAME;
                    obsModel.DISTRICT_NAME = item.DISTRICT_NAME;
                    obsModel.BLOCK_NAME = item.BLOCK_NAME;
                    obsModel.IMS_ROAD_NAME = item.IMS_ROAD_NAME;
                    obsModel.IMS_PACKAGE_ID = item.IMS_PACKAGE_ID;

                    obsModel.IMS_PROPOSAL_TYPE = item.IMS_PROPOSAL_TYPE.Equals("P") ? "Road" : "LSB";
                    obsModel.PMGSY_SCHEME = item.PMGSY_SCHEME;
                    obsModel.IMS_ISCOMPLETED = item.IMS_ISCOMPLETED;
                    obsModel.IMS_PAV_LENGTH = item.IMS_PROPOSAL_TYPE.Equals("P") ? item.IMS_PAV_LENGTH.ToString() : (item.IMS_BRIDGE_LENGTH == null ? "" : item.IMS_BRIDGE_LENGTH.ToString());
                    obsModel.QM_INSPECTED_START_CHAINAGE = item.QM_INSPECTED_START_CHAINAGE;
                    obsModel.QM_INSPECTED_END_CHAINAGE = item.QM_INSPECTED_END_CHAINAGE;
                    obsModel.QM_INSPECTION_DATE = item.QM_INSPECTION_DATE.ToString();
                    obsModel.OVERALL_GRADE = item.OVERALL_GRADE;
                    obsModel.NO_OF_PHOTO_UPLOADED = item.NO_OF_PHOTO_UPLOADED;

                    obsModel.INSPECTION_REPORT_LINK = (dbContext.QUALITY_INSPECTION_REPORT_FILE.Where(c => c.QM_OBSERVATION_ID == item.QM_OBSERVATION_ID).Any())
                                        ? "<a href='#' title='Click here to view details' class='ui-icon ui-icon-zoomin ui-align-center' onClick='ViewInspectionReportATR(\"" + item.QM_OBSERVATION_ID.ToString().Trim() + "\"); return false;'>View</a>" : "-";

                    obsModel.OBS_LINK = "<a href='#' title='Click here to view details' class='ui-icon ui-icon-zoomin ui-align-center' onClick='ShowATRObsDetailsNewtab(\"" + item.QM_OBSERVATION_ID.ToString().Trim() + "\"); return false;'>View</a>";


                    inspDetailsModel.INSP_AGAINST_ROAD_LIST.Add(obsModel);
                }

                return View("~/Views/QualityMonitoring/InspectionListAgainstRoad.cshtml", inspDetailsModel);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "QualitymonitoringController.QMViewInspectionListAgainstRoadNewTab()");
                inspDetailsModel.ERROR = "Error occurred while processing your request";
                return View("~/Views/QualityMonitoring/InspectionListAgainstRoad.cshtml", inspDetailsModel);
            }
            finally
            {
                dbContext.Dispose();
            }
        }



        #region technology details against road vikky
        public ActionResult ShowTechDetailsAgainstRoad(String roadCode1)
        {
            QualityMonitoringDAL qmDAL = new QualityMonitoringDAL();
            TechDetailsAgainstRoadModel fdrTechDetailsModel = new TechDetailsAgainstRoadModel();
            fdrTechDetailsModel.TECH_DETAILS_AGAINST_ROAD_LIST = new List<FDR_TECH_DETAILS>();

            dbContext = new PMGSYEntities();

            try
            {

                string[] encParam = roadCode1.Split('/');
                decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { encParam[0], encParam[1], encParam[2] });

                int roadCode = 0;
                if (decryptedParameters.Count > 0)
                {
                    roadCode = Convert.ToInt32(decryptedParameters["imsRoadID"].Split('$')[0]);

                }




                // int roadCode = Convert.ToInt32(roadCode1.Split('$')[0]);

                fdrTechDetailsModel.TECH_LIST = qmDAL.FDRTechDetailsDAL(roadCode);//ATR_Chang
                var techList = fdrTechDetailsModel.TECH_LIST.ToList();

                foreach (var item in techList)
                {
                    FDR_TECH_DETAILS techModel = new FDR_TECH_DETAILS();

                    techModel.STATE_NAME = item.STATE_NAME;
                    techModel.DISTRICT_NAME = item.DISTRICT_NAME;
                    techModel.BLOCK_NAME = item.BLOCK_NAME;
                    techModel.IMS_ROAD_NAME = item.IMS_ROAD_NAME;
                    techModel.IMS_PACKAGE_ID = item.IMS_PACKAGE_ID;

                    techModel.IMS_PROPOSAL_TYPE = item.IMS_PROPOSAL_TYPE.Equals("P") ? "Road" : "LSB";
                    techModel.PMGSY_SCHEME = item.PMGSY_SCHEME;
                    techModel.TECHNLOGY = item.TECHNLOGY.Trim().Remove(item.TECHNLOGY.Length - 1, 1);
                    techModel.ALL_ADDITIVE_NAME = item.ALL_ADDITIVE_NAME == null ? "" : item.ALL_ADDITIVE_NAME.Trim().Remove(item.ALL_ADDITIVE_NAME.Length - 1, 1);
                    techModel.STRETCH_CONSTR_DATE = item.STRETCH_CONSTR_DATE == null ? "" : DateTime.Parse(item.STRETCH_CONSTR_DATE.ToString()).ToShortDateString();
                    techModel.EXEC_LENGTH_STAB_BC = item.EXEC_LENGTH_STAB_BC;


                    fdrTechDetailsModel.TECH_DETAILS_AGAINST_ROAD_LIST.Add(techModel);
                }

                return View("~/Views/QualityMonitoring/FDRTechDetailsAgainstRoad.cshtml", fdrTechDetailsModel);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "QualitymonitoringController.ShowTechDetailsAgainstRoad()");
                fdrTechDetailsModel.ERROR = "Error occurred while processing your request";
                return View("~/Views/QualityMonitoring/FDRTechDetailsAgainstRoad.cshtml", fdrTechDetailsModel);
            }
            finally
            {
                dbContext.Dispose();
            }
        }


        #endregion

        //[HttpPost]
        //[Audit]
        //public ActionResult QMViewInspectionListAgainstRoad(FormCollection formCollection)
        //{
        //    QualityMonitoringDAL qualityDAL = new QualityMonitoringDAL();
        //    int totalRecords;
        //    try
        //    {
        //        //Adde By Abhishek kamble 29-Apr-2014 start
        //        using (CommonFunctions commonFunction = new CommonFunctions())
        //        {
        //            if (!commonFunction.ValidateGridParameters(new GridParams(Convert.ToInt32(formCollection["page"]), Convert.ToInt32(formCollection["rows"]), formCollection["sidx"], formCollection["sord"], Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
        //            {
        //                return null;
        //            }
        //        }
        //        //Adde By Abhishek kamble 29-Apr-2014 end
        //        //SACHIN
        //        var jsonData = new
        //        {
        //            rows = qualityDAL.QMViewInspDetailsAgainstRoadDAL(Convert.ToInt32(formCollection["page"]) - 1, Convert.ToInt32(formCollection["rows"]),
        //                                                    formCollection["sidx"], formCollection["sord"], out totalRecords,
        //                                                    Convert.ToInt32(formCollection["roadCode"])),
        //            //add parameter on 30-06-2022 by vikky
        //            total = totalRecords <= Convert.ToInt32(formCollection["rows"]) ? 1 : (totalRecords % Convert.ToInt32(formCollection["rows"]) == 0 ? totalRecords / Convert.ToInt32(formCollection["rows"]) : (totalRecords / Convert.ToInt32(formCollection["rows"]) + 1)),
        //            page = Convert.ToInt32(formCollection["page"]),
        //            records = totalRecords
        //        };
        //        return Json(jsonData);
        //    }
        //    catch (Exception ex)
        //    {
        //        //Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
        //        ErrorLog.LogError(ex, "QMViewInspectionListAgainstRoad()");
        //        return Json(String.Empty);
        //    }
        //}




        /// <summary>
        /// View 2 Tier Inspection Details TO CQC 
        /// </summary>
        /// <param name="formCollection"></param>
        /// <returns></returns>
        [HttpPost]
        [Audit]
        public ActionResult QMViewInspectionDetails2TierCQC(FormCollection formCollection)
        {
            qualityBAL = new QualityMonitoringBAL();
            int totalRecords;
            try
            {
                //Adde By Abhishek kamble 29-Apr-2014 start
                using (CommonFunctions commonFunction = new CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new GridParams(Convert.ToInt32(formCollection["page"]), Convert.ToInt32(formCollection["rows"]), formCollection["sidx"], formCollection["sord"], Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                    {
                        return null;
                    }
                }
                //Adde By Abhishek kamble 29-Apr-2014 end
                var jsonData = new
                {
                    rows = qualityBAL.QMViewInspectionDetails2TierCQCBAL(Convert.ToInt32(formCollection["page"]) - 1, Convert.ToInt32(formCollection["rows"]),
                                                            formCollection["sidx"], formCollection["sord"], out totalRecords,
                                                            Convert.ToInt32(formCollection["stateCode"]), Convert.ToInt32(formCollection["monitorCode"]),
                                                            Convert.ToInt32(formCollection["fromInspMonth"]), Convert.ToInt32(formCollection["fromInspYear"]),
                                                            Convert.ToInt32(formCollection["toInspMonth"]), Convert.ToInt32(formCollection["toInspYear"]), formCollection["ROAD_STATUS"], Convert.ToInt32(formCollection["schemeType"]), formCollection["roadOrBridge"], formCollection["gradeType"], formCollection["eFormStatus"]),
                    total = totalRecords <= Convert.ToInt32(formCollection["rows"]) ? 1 : (totalRecords % Convert.ToInt32(formCollection["rows"]) == 0 ? totalRecords / Convert.ToInt32(formCollection["rows"]) : (totalRecords / Convert.ToInt32(formCollection["rows"]) + 1)),
                    page = Convert.ToInt32(formCollection["page"]),
                    records = totalRecords
                };
                return Json(jsonData);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return Json(String.Empty);
            }
        }







        #region ATR DETAILS PIU VIKKY

        /// <summary>
        /// Filters for ATR Details
        /// </summary>
        /// <returns></returns>
        [Audit]
        public ActionResult Quality2TierATRFilters()
        {
            QMFilterViewModel qmFilterModel = new QMFilterViewModel();
            CommonFunctions objCommonFunctions = new CommonFunctions();
            try
            {
                qmFilterModel.MAST_STATE_CODE = 0;
                qmFilterModel.ADMIN_QM_CODE = 0;
                qmFilterModel.ATR_STATUS = "0";
                qmFilterModel.ROAD_STATUS = "A";

                qmFilterModel.FROM_MONTH = DateTime.Now.Month;
                qmFilterModel.FROM_YEAR = DateTime.Now.Year;
                qmFilterModel.TO_MONTH = DateTime.Now.Month;
                qmFilterModel.TO_YEAR = DateTime.Now.Year;

                //CQC & cqcadmin
                if (PMGSYSession.Current.RoleCode == 5 || PMGSYSession.Current.RoleCode == 9)
                {
                    qmFilterModel.STATES = objCommonFunctions.PopulateStates(false);
                    qmFilterModel.STATES.Insert(0, (new SelectListItem { Text = "All", Value = "0", Selected = true }));

                    //qmFilterModel.MONITORS = objCommonFunctions.PopulateMonitors("false", "I", 0).ToList(); //Purposely taken String "false" as argument
                    qmFilterModel.MONITORS = objCommonFunctions.PopulateAllMonitors("false", "S", 0).ToList(); //Purposely taken String "false" as argument //Edited on 13-12-2022 to display all monitors

                }
                else if (PMGSYSession.Current.RoleCode == 8 || PMGSYSession.Current.RoleCode == 69 || PMGSYSession.Current.RoleCode == 48)  //ATRDetails//SQC
                {
                    qmFilterModel.MAST_STATE_CODE = PMGSYSession.Current.StateCode;
                    //qmFilterModel.MONITORS = objCommonFunctions.PopulateMonitors("false", "I", PMGSYSession.Current.StateCode).ToList(); //Purposely taken String "false" as argument
                    qmFilterModel.MONITORS = objCommonFunctions.PopulateAllMonitors("false", "S", PMGSYSession.Current.StateCode).ToList(); //Purposely taken String "false" as argument //Edited on 13-12-2022 to display all monitors
                    qmFilterModel.PIU_OR_SQC = "S";
                }
                else if (PMGSYSession.Current.RoleCode == 22 || PMGSYSession.Current.RoleCode == 38 || PMGSYSession.Current.RoleCode == 54)  //PIU or PIURCPLWE
                {
                    qmFilterModel.MAST_STATE_CODE = PMGSYSession.Current.StateCode;
                    qmFilterModel.MONITORS = objCommonFunctions.PopulateMonitorsDistrictWise("false", "S", PMGSYSession.Current.StateCode, PMGSYSession.Current.DistrictCode).ToList(); //Purposely taken String "false" as argument
                    qmFilterModel.PIU_OR_SQC = "P";
                }

                qmFilterModel.schemeList = new List<SelectListItem> {
                                                            new SelectListItem{ Text = "All Schemes", Value ="0" , Selected = true },
                                                            new SelectListItem{ Text = "PMGSY I", Value ="1" },
                                                            new SelectListItem{ Text = "PMGSY II", Value ="2" } ,
                                                            new SelectListItem{ Text = "RCPLWE", Value ="3" },
                                                            new SelectListItem{ Text = "PMGSY III", Value ="4" }
                                                            };//ATR_Change

                qmFilterModel.FROM_MONTHS_LIST = objCommonFunctions.PopulateMonths(false);
                qmFilterModel.FROM_YEARS_LIST = objCommonFunctions.PopulateYears(false).Where(a => a.Value != "0").ToList();
                qmFilterModel.TO_MONTHS_LIST = objCommonFunctions.PopulateMonths(false);
                qmFilterModel.TO_YEARS_LIST = objCommonFunctions.PopulateYears(false).Where(a => a.Value != "0").ToList();
                qmFilterModel.ATR_STATUS_LIST = objCommonFunctions.Quality2TierATRStatus();
                qmFilterModel.imsSanctionedList = new List<SelectListItem> {
                                                        new SelectListItem{ Text = "Sanctioned", Value ="Y" , Selected = true },
                                                        new SelectListItem{ Text = "Dropped", Value ="D" },
                                                       };

                qmFilterModel.ROAD_STATUS_LIST = objCommonFunctions.PopulateRoadStatus();
                return View(qmFilterModel);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "QualityMonitoring.Quality2TierATRFilters()");
                return View(qmFilterModel); //return model as null

            }
        }


        public ActionResult ATR2TierDetails(FormCollection formCollection)
        {
            QualityMonitoringBAL qmBAL = new QualityMonitoringBAL();
            QMATR2TierDetailsModel atrDetailsModel = new QMATR2TierDetailsModel();
            atrDetailsModel.ATR_LIST = new List<QMATRModel>();
            atrDetailsModel.OBS_LIST = new List<QMObsATRModel>();
            dbContext = new PMGSYEntities();

            int PmgsyScheme = Convert.ToInt32(formCollection["schemeType"]);//ATR_Change
            if (PmgsyScheme < 0 || PmgsyScheme > 4)
            {
                ModelState.AddModelError("SchemeError", "Scheme is not valid");
                return View(atrDetailsModel); //return model as null
            }//ATR_Change

            try
            {
                if (PMGSYSession.Current.RoleCode == 22 || PMGSYSession.Current.RoleCode == 38 || PMGSYSession.Current.RoleCode == 54)
                {
                    atrDetailsModel.SQC_OR_PIU = "P";
                }
                else
                {
                    atrDetailsModel.SQC_OR_PIU = "S";
                }

                atrDetailsModel.OBSS_ATR_LIST = qmBAL.ATR2TierDetailssBAL(Convert.ToInt32(formCollection["MAST_STATE_CODE"]), Convert.ToInt32(formCollection["ADMIN_QM_CODE"]),
                                                                Convert.ToInt32(formCollection["FROM_MONTH"]), Convert.ToInt32(formCollection["FROM_YEAR"]),
                                                                Convert.ToInt32(formCollection["TO_MONTH"]), Convert.ToInt32(formCollection["TO_YEAR"]),
                                                                formCollection["ATR_STATUS"], formCollection["ROAD_STATUS"], PmgsyScheme, formCollection["imsSanctioned"]);//ATR_Chang




                var distinctObsList = (from obs in atrDetailsModel.OBSS_ATR_LIST
                                       select new
                                       {
                                           obs.QM_OBSERVATION_ID,
                                           obs.MONITOR_NAME,
                                           obs.STATE_NAME,
                                           obs.DISTRICT_NAME,
                                           obs.BLOCK_NAME,
                                           obs.IMS_PACKAGE_ID,
                                           obs.IMS_YEAR,
                                           obs.IMS_ROAD_NAME,
                                           obs.STAGE_PHASE,   //Added By Chandra Darshan Agrawal
                                           obs.IMS_IS_STAGED, //Added By Chandra Darshan Agrawal
                                           obs.QM_INSPECTED_START_CHAINAGE,
                                           obs.QM_INSPECTED_END_CHAINAGE,
                                           obs.QM_INSPECTED_END_CHAINAGE_BRIDGE,
                                           obs.QM_INSPECTION_DATE,
                                           obs.WORK_LENGTH,
                                           obs.IMS_ISCOMPLETED,
                                           obs.OVERALL_GRADE,
                                           obs.NO_OF_PHOTO_UPLOADED,
                                           obs.QM_ATR_STATUS,
                                           obs.PMGSY_SCHEME,
                                           obs.ADMIN_IS_ENQUIRY,
                                           obs.IMS_PROPOSAL_TYPE,
                                           obs.IMS_ISLABUPLOADED,
                                           obs.EXEC_COMPLETION_DATE,
                                           obs.EFORM_ID,
                                           obs.ATR_UPLOAD_ELIGIBILITY,

                                           //----------------- ADDED BY ROHIT BORSE for ATR MARK VERIFICATION
                                           obs.IMS_PR_ROAD_CODE,
                                           obs.ADMIN_QM_CODE
                                       }).Distinct().ToList();


                foreach (var item in distinctObsList)
                {
                    QMObsATRModel obsModel = new QMObsATRModel();

                    obsModel.QM_OBSERVATION_ID = item.QM_OBSERVATION_ID;
                    obsModel.MONITOR_NAME = item.MONITOR_NAME;
                    obsModel.STATE_NAME = item.STATE_NAME;
                    obsModel.DISTRICT_NAME = item.DISTRICT_NAME;
                    obsModel.BLOCK_NAME = item.BLOCK_NAME;
                    obsModel.IMS_PACKAGE_ID = item.IMS_PACKAGE_ID;
                    obsModel.IMS_YEAR = item.IMS_YEAR.ToString() + "-" + (item.IMS_YEAR + 1).ToString();
                    obsModel.IMS_ROAD_NAME = "<a href='#' style='color:blue' title='Click here to view Inspections' onClick=Show2TierInspAgainstRoad('" + URLEncrypt.EncryptParameters1(new String[] { "imsRoadID =" + item.IMS_PR_ROAD_CODE.ToString().Trim() }) + "'); return false;'>" + item.IMS_ROAD_NAME + "</a>";
                    obsModel.STAGE_PHASE = item.IMS_PROPOSAL_TYPE.Equals("P") ? (item.IMS_IS_STAGED == "C" ? "Completed" : item.STAGE_PHASE) : "--";
                    obsModel.QM_INSPECTED_START_CHAINAGE = item.QM_INSPECTED_START_CHAINAGE;
                    obsModel.QM_INSPECTED_END_CHAINAGE = item.QM_INSPECTED_END_CHAINAGE;
                    obsModel.QM_INSPECTED_END_CHAINAGE_BRIDGE = item.QM_INSPECTED_END_CHAINAGE_BRIDGE;
                    obsModel.QM_INSPECTION_DATE = item.QM_INSPECTION_DATE;
                    obsModel.WORK_LENGTH = item.WORK_LENGTH;
                    obsModel.IMS_ISCOMPLETED = item.IMS_ISCOMPLETED;
                    obsModel.PMGSY_SCHEME = item.PMGSY_SCHEME;
                    obsModel.OVERALL_GRADE = item.OVERALL_GRADE;
                    obsModel.NO_OF_PHOTO_UPLOADED = item.NO_OF_PHOTO_UPLOADED;
                    obsModel.QM_ATR_STATUS = item.QM_ATR_STATUS;
                    obsModel.ADMIN_IS_ENQUIRY = item.ADMIN_IS_ENQUIRY.Equals("Y") ? "Yes" : "No";
                    obsModel.IMS_PROPOSAL_TYPE = item.IMS_PROPOSAL_TYPE.Equals("P") ? "Road" : "LSB";
                    obsModel.SHOW_OBS_LINK = "<a href='#' title='Click here to view details' class='ui-icon ui-icon-zoomin ui-align-center' onClick='Show2TierATRObsDetails(\"" + item.QM_OBSERVATION_ID.ToString().Trim() + "\"); return false;'>View</a>";
                    obsModel.IMS_ISLABUPLOADED = ((item.IMS_ISLABUPLOADED.Trim().Equals("Yes"))
                                                     ?
                                                    "<a href='#' title='Click here to view uploaded lab photograph against this package' class='ui-icon ui-icon-zoomin ui-align-center' onClick='Show2TierLabDetails(\"" + dbContext.QUALITY_QM_LAB_MASTER.Where(x => x.IMS_PACKAGE_ID.Equals(item.IMS_PACKAGE_ID)).FirstOrDefault().QM_LAB_ID + "\"); return false;'>Lab Details</a>"
                                                    :
                                                    "<a href='#' title='Lab photograph is not available' class='ui-icon ui-icon-locked ui-align-center' />"); //Added by Deendayal 

                    obsModel.EXEC_COMPLETION_DATE = item.EXEC_COMPLETION_DATE == null ? "-" : item.EXEC_COMPLETION_DATE.ToString().ToString().Split(' ')[0].Replace('-', '/');
                    obsModel.VIEW_INSPECTION_REPORT_LINK =
                    (dbContext.QUALITY_INSPECTION_REPORT_FILE.Where(c => c.QM_OBSERVATION_ID == item.QM_OBSERVATION_ID).Any())
                                        ? "<a href='#' title='Click here to view details' class='ui-icon ui-icon-zoomin ui-align-center' onClick='ViewInspectionReport2TierATR(\"" + item.QM_OBSERVATION_ID.ToString().Trim() + "$ATR" + "\"); return false;'>View</a>"
                                        : "<a href='#' title='No files uploaded yet'><center>_</center></a>";
                    //Physical path pdf view 
                    obsModel.EFORM_PDF_VIEW = item.EFORM_ID == null ? "-" : (dbContext.EFORM_PDF_UPLOAD_DETAIL.Any(s => s.EFORM_ID == item.EFORM_ID && s.USER_TYPE == "Q" && s.IS_FINALISED == "Y") ? "<a  href='#'  onClick=viewCombinedPart_1_2_Pdf('" + item.EFORM_ID.ToString().Trim() + "'); title='Click here to view part-I,II pdf'><input type='button' value='View'/></a>" : "-");
                    //Virtual path pdf view 
                    // obsModel.EFORM_PDF_VIEW = item.EFORM_ID == null ? "-" : (dbContext.EFORM_PDF_UPLOAD_DETAIL.Any(s => s.EFORM_ID == item.EFORM_ID && s.USER_TYPE == "Q" && s.IS_FINALISED == "Y") ? "<a href='#' title='title='Click here to view part-I,II pdf'  onClick='viewCombinePdfVirtualDir(\"" + item.EFORM_ID.ToString() + "\");'   ><input type='button' value='View'/></a>" : "-");

                    obsModel.EFORM_PDF_PREVIEW = item.EFORM_ID == null ? "-" : (dbContext.EFORM_PDF_UPLOAD_DETAIL.Any(s => s.EFORM_ID == item.EFORM_ID && s.USER_TYPE == "Q" && s.IS_FINALISED == "Y") ?
                        (item.IMS_PROPOSAL_TYPE.Equals("P") ? "<input type='button' id='btnViewPDF' value='Preview' onClick=viewCombinePdfData('" + URLEncrypt.EncryptParameters1(new String[] { "imsRoadID =" + item.EFORM_ID.ToString().Trim() }) + "');   target=_blank />"
                        : "<input type='button' id='btnViewPDF' value='Preview' onClick=viewBridgeCombinePdfData('" + URLEncrypt.EncryptParameters1(new String[] { "imsRoadID =" + item.EFORM_ID.ToString().Trim() }) + "');   target=_blank />")
                        : "-");


                    atrDetailsModel.OBS_LIST.Add(obsModel);
                }


                string PhysicalPath = ConfigurationManager.AppSettings["QUALITY_INSPECTION_FILE_UPLOAD_2TIER_ATR"];


                foreach (var item in atrDetailsModel.OBSS_ATR_LIST)
                {
                    // Populate & Add ATR Details for each Observation Id
                    QMATRModel atrModel = new QMATRModel();
                    atrModel.QM_OBSERVATION_ID = item.QM_OBSERVATION_ID;
                    atrModel.QM_ATR_ID = item.QM_ATR_ID;
                    atrModel.ATR_ENTRY_DATE = item.ATR_ENTRY_DATE;
                    atrModel.ATR_REGRADE_STATUS = item.ATR_REGRADE_STATUS.Trim().Equals("N") ? "No" : "Yes";
                    atrModel.ATR_REGRADE_REMARKS = item.ATR_REGRADE_REMARKS;
                    atrModel.ATR_REGRADE_DATE = item.ATR_REGRADE_DATE;
                    atrModel.ATR_IS_DELETED = item.ATR_IS_DELETED;
                    atrModel.QM_ATR_STATUS = item.QM_ATR_STATUS;
                    atrModel.IS_SUBMITTED = item.QM_ATR_STATUS.Trim().Equals("N") ? "No" : "Yes";
                    //atrModel.ATR_UPLOAD_VIEW_LINK = (item.QM_ATR_ID != null)
                    //                              ? item.ATR_IS_DELETED == "Y"
                    //                                ? "--"
                    //                                : (System.IO.File.Exists(Path.Combine(PhysicalPath, HttpUtility.UrlEncode(item.QM_ATR_ID + ".pdf")).ToString().Replace(@"\\", @"//").Replace(@"\", @"/"))) ? "<a href='#' title='Click here to view uploaded ATR' class='ui-icon ui-icon-zoomin ui-align-center' onClick='DownloadATR(\"" + URLEncrypt.EncryptParameters(new string[] { item.QM_ATR_ID + ".pdf" + "$" + item.QM_ATR_ID }) + "\"); return false;'>Download</a>" : "<a href='#' title='Click here to upload missing ATR File' class='ui-icon ui-icon-plusthick ui-align-center' onClick='UploadMissingATR(\"" + item.QM_OBSERVATION_ID.ToString().Trim() + "$" + Convert.ToString(item.QM_ATR_ID).Trim() + "\"); return false;'>Upload</a>"
                    //                              : "<a href='#' class='ui-icon ui-icon-locked ui-align-center' title='ATR is not uploaded' />";


                    Check2TierSQMInspAvailableCount(item.IMS_PR_ROAD_CODE, item.QM_INSPECTION_DATE.ToString().Trim(), out long totalrecords);

                    //var noOfRec = dbContext.QUALITY_SQM_ATR_FILE.Where(a => a.QM_OBSERVATION_ID == item.QM_OBSERVATION_ID).Count();
                    //atrModel.VERIFICATION_ATR_CODE = noOfRec > 0
                    //                                ? "<a href='#' class='ui-icon ui-icon-locked ui-align-center'  />"
                    //                              " + item.IMS_PR_ROAD_CODE.ToString().Trim() + "\"); return false;' />"
                    //                                 : (item.ATR_UPLOAD_ELIGIBILITY == 0 ? "Not Eligible For ATR Upload" :
                    //                                                        (PMGSYSession.Current.RoleCode == 22 || PMGSYSession.Current.RoleCode == 38 || PMGSYSession.Current.RoleCode == 54)
                    //                                                        ? dbContext.QUALITY_SQM_ATR_FILE.Where(a => a.QM_ATR_ID == item.QM_ATR_ID).Select(a => a.ATR_VERIFICATION_FINALIZED).FirstOrDefault() == "Y"
                    //                                                            ? "<a href='#' class='ui-icon ui-icon-zoomout ui-align-center' onClick='ViewUploadedATR2TierFile(\"" + item.QM_OBSERVATION_ID.ToString().Trim() + "$" + Convert.ToString(item.QM_ATR_ID).Trim() + "$" + item.QM_INSPECTION_DATE.ToString().Trim().Replace(" ", "+") + "$" + item.IMS_PR_ROAD_CODE.ToString().Trim() + "\"); return false;' />"
                    //                                                            : totalrecords > 0
                    //                                                                    ? item.ATR_REGRADE_STATUS.Trim().Equals("R") ? "<a href='#' class='ui-icon ui-icon-locked ui-align-center'; return false;' />" : "<a href='#' title='Click here to view list for Verification of ATR' class='ui-icon ui-icon-plusthick ui-align-center' onClick='UploadATR2TierFile(\"" + item.QM_OBSERVATION_ID.ToString().Trim() + "$" + Convert.ToString(item.QM_ATR_ID).Trim() + "$" + item.QM_INSPECTION_DATE.ToString().Trim().Replace(" ", "+") + "$" + item.IMS_PR_ROAD_CODE.ToString().Trim() + "\");return false;'></a>"
                    //                                                                    : "<a href='#' class='ui-icon ui-icon-locked ui-align-center' title='No inspection is available for ATR mark'; onClick='noInspAvailableAlert();  return false;' />"
                    //                                                    : (dbContext.QUALITY_SQM_ATR_FILE.Where(a => a.QM_ATR_ID == item.QM_ATR_ID).Select(a => a.ATR_VERIFICATION_FINALIZED).FirstOrDefault() == "Y" ? "<a href='#' title='Click here to view details' class='ui-icon ui-icon-zoomin ui-align-center' onClick='ViewUploadedATR2TierSQCFile(\"" + item.QM_OBSERVATION_ID.ToString().Trim() + "$" + Convert.ToString(item.QM_ATR_ID).Trim() + "$" + item.QM_INSPECTION_DATE.ToString().Trim().Replace(" ", "+") + "$" + item.IMS_PR_ROAD_CODE.ToString().Trim() + "\"); return false;'>View</a>" : "<a href='#' class='ui-icon ui-icon-locked ui-align-center' onClick='lock_Mark2TierATRVerification(); return false;' />"));


                    // var noOfRec = dbContext.QUALITY_SQM_ATR_FILE.Where(a => a.QM_OBSERVATION_ID == item.QM_OBSERVATION_ID).Count();
                    atrModel.VERIFICATION_ATR_CODE =
                                                   (item.ATR_UPLOAD_ELIGIBILITY == 0 ? "Not Eligible For ATR Upload" :
                                                                            (PMGSYSession.Current.RoleCode == 22 || PMGSYSession.Current.RoleCode == 38 || PMGSYSession.Current.RoleCode == 54)
                                                                            ? dbContext.QUALITY_SQM_ATR_FILE.Where(a => a.QM_ATR_ID == item.QM_ATR_ID).Select(a => a.ATR_VERIFICATION_FINALIZED).FirstOrDefault() == "Y"
                                                                                ? "<a href='#' class='ui-icon ui-icon-zoomout ui-align-center' onClick='ViewUploadedATR2TierFile(\"" + item.QM_OBSERVATION_ID.ToString().Trim() + "$" + Convert.ToString(item.QM_ATR_ID).Trim() + "$" + item.QM_INSPECTION_DATE.ToString().Trim().Replace(" ", "+") + "$" + item.IMS_PR_ROAD_CODE.ToString().Trim() + "\"); return false;' />"
                                                                                : totalrecords > 0
                                                                                        ? "<a href='#' title='Click here to view list for Verification of ATR' class='ui-icon ui-icon-plusthick ui-align-center' onClick='UploadATR2TierFile(\"" + item.QM_OBSERVATION_ID.ToString().Trim() + "$" + Convert.ToString(item.QM_ATR_ID).Trim() + "$" + item.QM_INSPECTION_DATE.ToString().Trim().Replace(" ", "+") + "$" + item.IMS_PR_ROAD_CODE.ToString().Trim() + "\");return false;'></a>"
                                                                                        : "<a href='#' class='ui-icon ui-icon-locked ui-align-center' title='No inspection is available for ATR mark'; onClick='noInspAvailableAlert();  return false;' />"
                                                                        : (dbContext.QUALITY_SQM_ATR_FILE.Where(a => a.QM_ATR_ID == item.QM_ATR_ID).Select(a => a.ATR_VERIFICATION_FINALIZED).FirstOrDefault() == "Y" ? "<a href='#' title='Click here to view details' class='ui-icon ui-icon-zoomin ui-align-center' onClick='ViewUploadedATR2TierSQCFile(\"" + item.QM_OBSERVATION_ID.ToString().Trim() + "$" + Convert.ToString(item.QM_ATR_ID).Trim() + "$" + item.QM_INSPECTION_DATE.ToString().Trim().Replace(" ", "+") + "$" + item.IMS_PR_ROAD_CODE.ToString().Trim() + "\"); return false;'>View</a>" : "<a href='#' class='ui-icon ui-icon-locked ui-align-center' onClick='lock_Mark2TierATRVerification(); return false;' />"));




                    //: (dbContext.QUALITY_SQM_ATR_FILE.Where(a => a.QM_ATR_ID == item.QM_ATR_ID).Select(a => a.ATR_VERIFICATION_FINALIZED).FirstOrDefault() == "Y" ? "<a href='#' title='Click here to view details' class='ui-icon ui-icon-zoomin ui-align-center' onClick='Show2TierATRObsDetails(\"" + (dbContext.QUALITY_SQM_ATR_FILE.Where(a => a.QM_ATR_ID == item.QM_ATR_ID).Select(a => a.SQM_OBSERVATION_ID).FirstOrDefault()) + "\"); return false;'>View</a>" : "<a href='#' class='ui-icon ui-icon-locked ui-align-center' onClick='lock_Mark2TierATRVerification(); return false;' />"));




                    Check2TierRejectFlagStatus(item.QM_OBSERVATION_ID, out bool rejectLockStatus);//rejectLockStatus=true , Lock Reject ;rejectLockStatus=false , active '+' sign
                    atrModel.ATR_ACCEPTANCE_LINK =
                         (PMGSYSession.Current.RoleCode == 8 || PMGSYSession.Current.RoleCode == 48 || PMGSYSession.Current.RoleCode == 69)//for PIU or PIURCPLWE view only
                         ? item.ATR_REGRADE_STATUS.Trim().Equals("A")
                                        ? "Accepted"
                                        : item.ATR_REGRADE_STATUS.Trim().Equals("R")
                                            ? "Rejected"
                                            : item.ATR_REGRADE_STATUS.Equals("V")
                                                ? "To be Verified"
                                                : item.ATR_REGRADE_STATUS.Equals("D")
                                                    ? "Non Rectifible Deffect"
                                                    : ""
                         : item.ATR_REGRADE_STATUS.Trim().Equals("A")     // Acceptance
                                        ? "Accepted"
                                        : item.ATR_REGRADE_STATUS.Equals("V")
                                            ? "To be Verified"
                                            : item.ATR_REGRADE_STATUS.Equals("D")
                                                ? "Non Rectifiable Deffect"
                                                //: (item.ATR_REGRADE_STATUS.Equals("C") && PMGSYSession.Current.RoleCode == 5) // Changes done on 13-04-2015 as per given ECR by Aanad. 
                                                : (item.ATR_REGRADE_STATUS.Equals("C"))
                                                    ? "Technical Committee"  // If Tech Committee, then append + sign to upload again


                                                    + (atrModel.QM_ATR_ID == (dbContext.QUALITY_SQM_ATR_FILE.Where(c => c.QM_OBSERVATION_ID == atrModel.QM_OBSERVATION_ID).Select(c => c.QM_ATR_ID).Max())
                                                       ? "<a href='#' title='Click here to upload ATR' class='ui-icon ui-icon-plusthick ui-align-center' onClick='UploadATR(\"" + item.QM_OBSERVATION_ID.ToString().Trim() + "$" + Convert.ToString(item.QM_ATR_ID).Trim() + "\"); return false;'>Upload</a>"
                                                       : "")

                                                    : item.ATR_REGRADE_STATUS.Trim().Equals("R") // if any of the ATR against Obs Id is Accepted then dont provide link to upload.
                                                        ? item.QM_ATR_STATUS.Equals("A")
                                                            ? "Rejected"
                                                            : "Rejected" + //If Rejected atr is last against Observation, then append + sign to upload again
                                                                           //Below line is commented on 18-01-2023
                                                                           //  (atrModel.QM_ATR_ID == (dbContext.QUALITY_SQM_ATR_FILE.Where(c => c.QM_OBSERVATION_ID == atrModel.QM_OBSERVATION_ID).Select(c => c.QM_ATR_ID).Max() )
                                                                           //Below line is Added on 30-01-2023
                                                                ((atrModel.QM_ATR_ID == (dbContext.QUALITY_SQM_ATR_FILE.Where(c => c.QM_OBSERVATION_ID == atrModel.QM_OBSERVATION_ID).Select(c => c.QM_ATR_ID).Max()) && (PMGSYSession.Current.RoleCode == 22 || PMGSYSession.Current.RoleCode == 38 || PMGSYSession.Current.RoleCode == 54))
                                                                //Below line is commented on 30-01-2023
                                                                //? "<a href='#' title='Click here to upload ATR' class='ui-icon ui-icon-plusthick ui-align-center' onClick='UploadATR(\"" + item.QM_OBSERVATION_ID.ToString().Trim() + "$" + Convert.ToString(item.QM_ATR_ID).Trim() + "\"); return false;'>Upload</a>"
                                                                //Below line is commented on 20-03-2023
                                                                //? "<a href='#' title='Click here to upload ATR' class='ui-icon ui-icon-plusthick ui-align-center' onClick='UploadATR2TierFile(\"" + item.QM_OBSERVATION_ID.ToString().Trim() + "$" + Convert.ToString(item.QM_ATR_ID).Trim() + "$" + item.QM_INSPECTION_DATE.ToString().Trim().Replace(" ", "+") + "$" + item.IMS_PR_ROAD_CODE.ToString().Trim() + "\");return false;'>Upload</a>"
                                                                //Below line is Added on 20-03-2023
                                                                ? rejectLockStatus ? "<a href='#' class='ui-icon ui-icon-locked ui-align-center' title='ATR with regrade status Technical committee , Verification , non-rectifiable defect cannot upload again.'; return false;' />" : "<a href='#' title='Click here to upload ATR' class='ui-icon ui-icon-plusthick ui-align-center' onClick='UploadATR2TierFile(\"" + item.QM_OBSERVATION_ID.ToString().Trim() + "$" + Convert.ToString(item.QM_ATR_ID).Trim() + "$" + item.QM_INSPECTION_DATE.ToString().Trim().Replace(" ", "+") + "$" + item.IMS_PR_ROAD_CODE.ToString().Trim() + "\");return false;'>Upload</a>"
                                                                : "")
                                                        : "";

                    atrModel.ATR_REGRADE_LINK =
                        (PMGSYSession.Current.RoleCode == 22 || PMGSYSession.Current.RoleCode == 38 || PMGSYSession.Current.RoleCode == 5 || PMGSYSession.Current.RoleCode == 54)//for PIU or PIURCPLWE view only
                        ? (item.ATR_REGRADE_STATUS.Trim().Equals("A"))
                            ? "<a href='#' title='Click here to see observation details against ATR' class='ui-icon ui-icon-zoomin ui-align-center' onClick='ShowATRGradingDetails(\"" + item.QM_OBSERVATION_ID.ToString().Trim() + "\"); return false;'>Regrade</a>"
                            : "<a href='#' class='ui-icon ui-icon-locked ui-align-center' />"
                        : (item.QM_ATR_ID != null) ? ((dbContext.QUALITY_SQM_ATR_FILE.Where(x => x.QM_OBSERVATION_ID == item.QM_OBSERVATION_ID).Max(y => y.QM_ATR_ID) != item.QM_ATR_ID) ? "<a href='#' class='ui-icon ui-icon-locked ui-align-center' />" :
                        ((item.ATR_REGRADE_STATUS.Trim().Equals("U") || item.ATR_REGRADE_STATUS.Trim().Equals("C") || item.ATR_REGRADE_STATUS.Trim().Equals("V")) && item.ATR_IS_DELETED.Equals("N")) // Change done by Chandra Darshan Agrawal
                            ? "<a href='#' title='Click here to regrade ATR' class='ui-icon ui-icon-plusthick ui-align-center' onClick='Regrade2TierATR(\"" + item.QM_OBSERVATION_ID.ToString().Trim() + "\"); return false;'>Regrade</a>"
                            : (item.ATR_REGRADE_STATUS.Trim().Equals("A"))
                                ? "<a href='#' title='Click here to see observation details against ATR' class='ui-icon ui-icon-zoomin ui-align-center' onClick='ShowATRGradingDetails(\"" + item.QM_OBSERVATION_ID.ToString().Trim() + "\"); return false;'>Regrade</a>"
                                : "<a href='#' class='ui-icon ui-icon-locked ui-align-center' />") : "<a href='#' class='ui-icon ui-icon-locked ui-align-center' />";

                    atrModel.ATR_DELETE_LINK =
                         (PMGSYSession.Current.RoleCode == 22 || PMGSYSession.Current.RoleCode == 38 || PMGSYSession.Current.RoleCode == 5 || PMGSYSession.Current.RoleCode == 54)//for PIU or PIURCPLWE view only
                         ? "<a href='#' class='ui-icon ui-icon-locked ui-align-center' />"
                         : (item.QM_ATR_ID != null && item.QM_ATR_STATUS.Trim() != "A" && item.QM_ATR_STATUS.Trim() != "N" && (item.QM_ATR_ID == dbContext.QUALITY_SQM_ATR_FILE.Where(c => c.QM_OBSERVATION_ID == item.QM_OBSERVATION_ID).Select(c => c.QM_ATR_ID).Max()))
                                ? "<a href='#' title='Click here to delete ATR details' class='ui-icon ui-icon-trash ui-align-center' onClick='DeleteATR(\"" +
                                    item.QM_OBSERVATION_ID.ToString().Trim() + "\",\"" + dbContext.QUALITY_SQM_ATR_FILE.Where(a => a.QM_OBSERVATION_ID == item.QM_OBSERVATION_ID && a.ATR_IS_DELETED == "N").Select(a => a.QM_ATR_ID).FirstOrDefault()
                                    + "\"); return false;'>Delete</a>"
                                : "<a href='#' class='ui-icon ui-icon-locked ui-align-center' />";

                    atrDetailsModel.ATR_LIST.Add(atrModel);

                    //index++; // increment index for each record.
                }



                // return View(atrDetailsModel);
                if (PMGSYSession.Current.RoleCode == 22 || PMGSYSession.Current.RoleCode == 38 || PMGSYSession.Current.RoleCode == 54)
                {
                    return View("~/Views/QualityMonitoring/ATR2TierPIUDetails.cshtml", atrDetailsModel);
                }
                else
                {
                    return View("~/Views/QualityMonitoring/ATR2TierSQCDetails.cshtml", atrDetailsModel);
                }

            }
            catch (Exception ex)
            {


                ErrorLog.LogError(ex, "QualityMonitoring.ATR2TierDetails()");
                atrDetailsModel.ERROR = "Error occurred while processing your request";
                //return View(atrDetailsModel); //return model as null
                return View("~/Views/QualityMonitoring/ATR2TierPIUDetails.cshtml", atrDetailsModel);

            }
            finally
            {
                dbContext.Dispose();
            }
        }





        public void Check2TierSQMInspAvailableCount(int roadCode, string inspectionDate, out long totalrecords)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            string inspDate = DateTime.Parse(inspectionDate).ToShortDateString();
            var splittedDate = inspDate.Split('/');
            DateTime? qmInspDate = new DateTime(int.Parse(splittedDate[2]), int.Parse(splittedDate[1]), int.Parse(splittedDate[0]));

            try
            {

                // If Role Not CQC
                if (PMGSYSession.Current.RoleCode == 22 || PMGSYSession.Current.RoleCode == 38 || PMGSYSession.Current.RoleCode == 54)
                {
                    var list1 = (from obsm in dbContext.QUALITY_QM_OBSERVATION_MASTER
                                 join isp in dbContext.IMS_SANCTIONED_PROJECTS on obsm.IMS_PR_ROAD_CODE equals isp.IMS_PR_ROAD_CODE
                                 join qmschedule in dbContext.QUALITY_QM_SCHEDULE on obsm.ADMIN_SCHEDULE_CODE equals qmschedule.ADMIN_SCHEDULE_CODE
                                 join aqm in dbContext.ADMIN_QUALITY_MONITORS on qmschedule.ADMIN_QM_CODE equals aqm.ADMIN_QM_CODE
                                 join qmscheduleDetail in dbContext.QUALITY_QM_SCHEDULE_DETAILS on qmschedule.ADMIN_SCHEDULE_CODE equals qmscheduleDetail.ADMIN_SCHEDULE_CODE
                                 where isp.IMS_PR_ROAD_CODE == roadCode && obsm.IMS_PR_ROAD_CODE == roadCode &&
                                 aqm.ADMIN_QM_TYPE == "S" &&
                                 (obsm.QM_OVERALL_GRADE == 1) &&
                                 isp.MAST_STATE_CODE == isp.MAST_STATE_CODE &&
                                 obsm.QM_INSPECTION_DATE >= qmInspDate &&
                                 isp.IMS_PR_ROAD_CODE == qmscheduleDetail.IMS_PR_ROAD_CODE
                                 select new
                                 {
                                     obsm.QM_OBSERVATION_ID,
                                     qmschedule.ADMIN_QM_CODE,
                                     isp.MAST_STATE_CODE,
                                     isp.MAST_DISTRICT_CODE,
                                     isp.MAST_BLOCK_CODE,
                                     isp.IMS_PACKAGE_ID,
                                     IMS_YEAR1 = isp.IMS_YEAR,
                                     IMS_YEAR2 = isp.IMS_YEAR + 1,
                                     isp.IMS_ROAD_NAME,
                                     isp.IMS_PROPOSAL_TYPE,
                                     obsm.QM_INSPECTED_START_CHAINAGE,
                                     obsm.QM_INSPECTED_END_CHAINAGE,
                                     obsm.QM_INSPECTION_DATE,
                                     isp.IMS_BRIDGE_LENGTH,
                                     isp.IMS_PAV_LENGTH,
                                     isp.IMS_ISCOMPLETED,
                                     isp.MAST_PMGSY_SCHEME,
                                     obsm.QM_OVERALL_GRADE,
                                     isp.IMS_PR_ROAD_CODE,
                                     aqm.ADMIN_QM_TYPE,
                                     aqm.ADMIN_QM_LNAME,
                                     aqm.ADMIN_QM_MNAME,
                                     aqm.ADMIN_QM_FNAME,
                                     qmscheduleDetail.ADMIN_IS_ENQUIRY
                                 }).OrderBy(s => s.QM_OBSERVATION_ID);

                    totalrecords = list1.Count();
                }
                else
                {
                    totalrecords = 0;
                }
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "QualityMonitoringController.Check2TierSQMInspAvailableCount()");
                totalrecords = 0;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        public void Check2TierRejectFlagStatus(int QM_OBSERVATION_ID, out bool rejectLockStatus)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            try
            {
                rejectLockStatus = dbContext.QUALITY_SQM_ATR_FILE.Where(x => x.QM_OBSERVATION_ID == QM_OBSERVATION_ID).Where(x => x.ATR_REGRADE_STATUS == "C" || x.ATR_REGRADE_STATUS == "D" || x.ATR_REGRADE_STATUS == "V").Any() ? true : false;

            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "QualityMonitoring.Check2TierRejectFlagStatus(item.QM_OBSERVATION_ID, out bool rejectLockStatus)");
                rejectLockStatus = true;
                throw;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        [HttpGet]
        [Audit]
        public ActionResult PdfFileUpload2TierATR(string id)
        {
            Int32 QM_OBSERVATION_ID = 0, QM_ATR_ID = 0;

            //Below fields added on 30-01-2023
            int roadCode = Convert.ToInt32(id.Trim().Split('$')[3]);
            string inspectionDate = (id.Split('$')[2] != null || id.Split('$')[2] != string.Empty) ? id.Split('$')[2].Replace("+", " ") : id.Split('$')[2];
            bool hideAddATRFileButton = false;//false=show Add ATR file button, true=hide Add ATR file button 
            bool hideATRFileList = false;

            dbContext = new PMGSYEntities();
            try
            {
                PMGSY.Models.QualityMonitoring.FileUploadViewModel fileUploadViewModel = new PMGSY.Models.QualityMonitoring.FileUploadViewModel();
                if (id.Contains('$'))
                {
                    //1)if sqc is visiting the Mark for ATR verification screen from (+) icon below rejected status , then  QM_ATR_Id = QM_ATR_Id of the rejected record 
                    //2)For New Entry of Mark for ATR Verification QM_ATR_ID=0
                    //3)If ATR is already uploaded but inspection is not mark , then QM_ATR_ID=uploaded file atr id

                    QM_OBSERVATION_ID = Convert.ToInt32(id.Trim().Split('$')[0]);
                    QM_ATR_ID = string.IsNullOrEmpty(id.Trim().Split('$')[1]) ? 0 : Convert.ToInt32(id.Trim().Split('$')[1]);
                    //flagAddATRFileButton = QM_ATR_ID != 0
                    //                       ?
                    //                           (dbContext.QUALITY_SQM_ATR_FILE.Where(x => x.QM_ATR_ID == QM_ATR_ID).Select(x => x.ATR_VERIFICATION_FINALIZED).FirstOrDefault() != "Y") ? true : false
                    //                       : false;

                    hideAddATRFileButton = QM_ATR_ID != 0
                                           ?
                                            dbContext.QUALITY_SQM_ATR_FILE.Where(x => x.QM_ATR_ID == QM_ATR_ID).Select(x => x.ATR_REGRADE_STATUS).FirstOrDefault() == "R"
                                                ?
                                                    false
                                                :
                                               (dbContext.QUALITY_SQM_ATR_FILE.Where(x => x.QM_ATR_ID == QM_ATR_ID).Select(x => x.ATR_VERIFICATION_FINALIZED).FirstOrDefault() == "Y" ? true : false)
                                           : false;

                    hideATRFileList = QM_ATR_ID != 0
                                           ?
                                               dbContext.QUALITY_SQM_ATR_FILE.Where(x => x.QM_ATR_ID == QM_ATR_ID).Select(x => x.ATR_REGRADE_STATUS).FirstOrDefault() == "R"
                                               ? true
                                               : false
                                           : true;

                    ViewBag.ShowAddATRFileButton = hideAddATRFileButton ? false : true;
                    ViewBag.HideATRFileList = hideATRFileList;//
                }
                else
                {
                    QM_OBSERVATION_ID = Convert.ToInt32(id);
                }
                fileUploadViewModel.QM_OBSERVATION_ID = QM_OBSERVATION_ID;
                fileUploadViewModel.QM_ATR_ID = QM_ATR_ID;

                fileUploadViewModel.allowRejectedAtrView = false;
                fileUploadViewModel.selectedSgradeObsId = 0;
                //if (dbContext.QUALITY_SQM_ATR_FILE.Any(x => x.QM_ATR_ID == QM_ATR_ID))
                //{
                //    fileUploadViewModel.selectedSgradeObsId = dbContext.QUALITY_SQM_ATR_FILE.Where(x => x.QM_ATR_ID == QM_ATR_ID).Select(s => s.SQM_OBSERVATION_ID).FirstOrDefault();
                //}
                //Below fields added on 30-01-2023
                fileUploadViewModel.IMS_PR_ROAD_CODE = roadCode;
                fileUploadViewModel.QM_INSPECTION_DATE = inspectionDate;

                ////-------------------Added by Shreyas on 02-05-2023 -------------------//If Eform OR Insp.Report is not uploaded then ATR upload provision will not be available
                //int scheduleCode = Convert.ToInt32(dbContext.QUALITY_QM_OBSERVATION_MASTER.Where(s => s.QM_OBSERVATION_ID == QM_OBSERVATION_ID).Select(s => s.ADMIN_SCHEDULE_CODE).FirstOrDefault());
                //var EformData = dbContext.EFORM_PDF_UPLOAD_DETAIL.
                //               Join(dbContext.EFORM_MASTER,
                //               pdf => pdf.EFORM_ID,
                //               em => em.EFORM_ID,
                //               (pdf, em) => new { PDF = pdf, EM = em })
                //               .Where(x => x.PDF.USER_TYPE == "Q" && x.PDF.IS_FINALISED == "Y" && x.EM.IS_VALID == "Y" && x.EM.IMS_PR_ROAD_CODE == roadCode && x.EM.ADMIN_SCHEDULE_CODE == scheduleCode).Any();

                //var InspReportData = dbContext.QUALITY_INSPECTION_REPORT_FILE.Where(s => s.QM_OBSERVATION_ID == QM_OBSERVATION_ID).Any();

                //if (EformData == false && InspReportData == false)
                //{
                //    return Json(new { success = false, message = "Eform or Inspection report is not uploaded for selected inspection." }, JsonRequestBehavior.AllowGet);
                //}
                ////---------------

                fileUploadViewModel.ErrorMessage = string.Empty;

                if (dbContext.QUALITY_SQM_ATR_FILE.Where(a => a.QM_OBSERVATION_ID == QM_OBSERVATION_ID).Any())
                {
                    var maxAtrId = dbContext.QUALITY_SQM_ATR_FILE.Where(a => a.QM_OBSERVATION_ID == QM_OBSERVATION_ID).Select(a => a.QM_ATR_ID).Max();
                    var maxRecordGrade = dbContext.QUALITY_SQM_ATR_FILE.Where(a => a.QM_OBSERVATION_ID == QM_OBSERVATION_ID
                                                                         && a.QM_ATR_ID == maxAtrId
                        ///Changes by DEENDAYAL SHARMA for Uploading of Deleted ATRs
                        /*&& a.ATR_IS_DELETED == "N"*/).Select(a => a.ATR_REGRADE_STATUS).FirstOrDefault();
                    if (maxRecordGrade != null && (maxRecordGrade.Equals("R") || maxRecordGrade.Equals("C")))
                    {
                        fileUploadViewModel.NumberofPdfs = 0;
                    }
                    else
                    {
                        fileUploadViewModel.NumberofPdfs = dbContext.QUALITY_SQM_ATR_FILE.Where(a => a.QM_OBSERVATION_ID == QM_OBSERVATION_ID && a.ATR_IS_DELETED == "N").Count();
                    }
                }
                else
                {
                    fileUploadViewModel.NumberofPdfs = 0;
                }
                return View("ATR2TierFileUpload", fileUploadViewModel);
            }
            catch (Exception ex)
            {
                //Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                ErrorLog.LogError(ex, "QualityMonitoring.PdfFileUpload2TierATR(string id)");
                return Json(string.Empty, JsonRequestBehavior.AllowGet);
            }
            finally
            {
                dbContext.Dispose();
            }
        }


        [HttpGet]
        [Audit]
        public ActionResult viewPdfFileUpload2TierATR(string id)
        {
            Int32 QM_OBSERVATION_ID = 0, QM_ATR_ID = 0;

            //Below fields added on 30-01-2023
            int roadCode = Convert.ToInt32(id.Trim().Split('$')[3]);
            string inspectionDate = (id.Split('$')[2] != null || id.Split('$')[2] != string.Empty) ? id.Split('$')[2].Replace("+", " ") : id.Split('$')[2];
            bool hideAddATRFileButton = false;//false=show Add ATR file button, true=hide Add ATR file button 
            bool hideATRFileList = false;

            dbContext = new PMGSYEntities();
            try
            {
                PMGSY.Models.QualityMonitoring.FileUploadViewModel fileUploadViewModel = new PMGSY.Models.QualityMonitoring.FileUploadViewModel();
                if (id.Contains('$'))
                {
                    //1)if sqc is visiting the Mark for ATR verification screen from (+) icon below rejected status , then  QM_ATR_Id = QM_ATR_Id of the rejected record 
                    //2)For New Entry of Mark for ATR Verification QM_ATR_ID=0
                    //3)If ATR is already uploaded but inspection is not mark , then QM_ATR_ID=uploaded file atr id

                    QM_OBSERVATION_ID = Convert.ToInt32(id.Trim().Split('$')[0]);
                    QM_ATR_ID = string.IsNullOrEmpty(id.Trim().Split('$')[1]) ? 0 : Convert.ToInt32(id.Trim().Split('$')[1]);
                    //flagAddATRFileButton = QM_ATR_ID != 0
                    //                       ?
                    //                           (dbContext.QUALITY_SQM_ATR_FILE.Where(x => x.QM_ATR_ID == QM_ATR_ID).Select(x => x.ATR_VERIFICATION_FINALIZED).FirstOrDefault() != "Y") ? true : false
                    //                       : false;

                    //hideAddATRFileButton = QM_ATR_ID != 0
                    //                       ?
                    //                        dbContext.QUALITY_SQM_ATR_FILE.Where(x => x.QM_ATR_ID == QM_ATR_ID).Select(x => x.ATR_REGRADE_STATUS).FirstOrDefault() == "R"
                    //                            ?
                    //                                false
                    //                            :
                    //                           (dbContext.QUALITY_SQM_ATR_FILE.Where(x => x.QM_ATR_ID == QM_ATR_ID).Select(x => x.ATR_VERIFICATION_FINALIZED).FirstOrDefault() == "Y" ? true : false)
                    //                       : false;

                    //hideATRFileList = QM_ATR_ID != 0
                    //                       ?
                    //                           dbContext.QUALITY_SQM_ATR_FILE.Where(x => x.QM_ATR_ID == QM_ATR_ID).Select(x => x.ATR_REGRADE_STATUS).FirstOrDefault() == "R"
                    //                           ? true
                    //                           : false
                    //                       : true;

                    //ViewBag.ShowAddATRFileButton = hideAddATRFileButton ? false : true;
                    //ViewBag.HideATRFileList = hideATRFileList;//
                    hideAddATRFileButton = true;
                    hideATRFileList = false;
                    ViewBag.ShowAddATRFileButton = hideAddATRFileButton ? false : true;
                    ViewBag.HideATRFileList = hideATRFileList;//
                }
                else
                {
                    QM_OBSERVATION_ID = Convert.ToInt32(id);
                }
                fileUploadViewModel.QM_OBSERVATION_ID = QM_OBSERVATION_ID;
                fileUploadViewModel.QM_ATR_ID = QM_ATR_ID;


                //Below fields added on 30-01-2023
                fileUploadViewModel.IMS_PR_ROAD_CODE = roadCode;
                fileUploadViewModel.QM_INSPECTION_DATE = inspectionDate;
                fileUploadViewModel.allowRejectedAtrView = false;
                if (dbContext.QUALITY_SQM_ATR_FILE.Any(s => s.QM_ATR_ID == QM_ATR_ID && s.ATR_REGRADE_STATUS.Equals("R")))
                {
                    fileUploadViewModel.allowRejectedAtrView = true;
                }
                if (dbContext.QUALITY_SQM_ATR_FILE.Any(x => x.QM_ATR_ID == QM_ATR_ID))
                {
                    fileUploadViewModel.selectedSgradeObsId = dbContext.QUALITY_SQM_ATR_FILE.Where(x => x.QM_ATR_ID == QM_ATR_ID).Select(s => s.SQM_OBSERVATION_ID).FirstOrDefault();
                }
                ////-------------------Added by Shreyas on 02-05-2023 -------------------//If Eform OR Insp.Report is not uploaded then ATR upload provision will not be available
                //int scheduleCode = Convert.ToInt32(dbContext.QUALITY_QM_OBSERVATION_MASTER.Where(s => s.QM_OBSERVATION_ID == QM_OBSERVATION_ID).Select(s => s.ADMIN_SCHEDULE_CODE).FirstOrDefault());
                //var EformData = dbContext.EFORM_PDF_UPLOAD_DETAIL.
                //               Join(dbContext.EFORM_MASTER,
                //               pdf => pdf.EFORM_ID,
                //               em => em.EFORM_ID,
                //               (pdf, em) => new { PDF = pdf, EM = em })
                //               .Where(x => x.PDF.USER_TYPE == "Q" && x.PDF.IS_FINALISED == "Y" && x.EM.IS_VALID == "Y" && x.EM.IMS_PR_ROAD_CODE == roadCode && x.EM.ADMIN_SCHEDULE_CODE == scheduleCode).Any();

                //var InspReportData = dbContext.QUALITY_INSPECTION_REPORT_FILE.Where(s => s.QM_OBSERVATION_ID == QM_OBSERVATION_ID).Any();

                //if (EformData == false && InspReportData == false)
                //{
                //    return Json(new { success = false, message = "Eform or Inspection report is not uploaded for selected inspection." }, JsonRequestBehavior.AllowGet);
                //}
                ////---------------

                fileUploadViewModel.ErrorMessage = string.Empty;

                if (dbContext.QUALITY_SQM_ATR_FILE.Where(a => a.QM_OBSERVATION_ID == QM_OBSERVATION_ID).Any())
                {
                    var maxAtrId = dbContext.QUALITY_SQM_ATR_FILE.Where(a => a.QM_OBSERVATION_ID == QM_OBSERVATION_ID).Select(a => a.QM_ATR_ID).Max();
                    var maxRecordGrade = dbContext.QUALITY_SQM_ATR_FILE.Where(a => a.QM_OBSERVATION_ID == QM_OBSERVATION_ID
                                                                         && a.QM_ATR_ID == maxAtrId
                        ///Changes by DEENDAYAL SHARMA for Uploading of Deleted ATRs
                        /*&& a.ATR_IS_DELETED == "N"*/).Select(a => a.ATR_REGRADE_STATUS).FirstOrDefault();
                    if (maxRecordGrade != null && (maxRecordGrade.Equals("R") || maxRecordGrade.Equals("C")))
                    {
                        fileUploadViewModel.NumberofPdfs = 0;
                    }
                    else
                    {
                        fileUploadViewModel.NumberofPdfs = dbContext.QUALITY_SQM_ATR_FILE.Where(a => a.QM_OBSERVATION_ID == QM_OBSERVATION_ID && a.ATR_IS_DELETED == "N").Count();
                    }
                }
                else
                {
                    fileUploadViewModel.NumberofPdfs = 0;
                }
                return View("ATR2TierFileUpload", fileUploadViewModel);
            }
            catch (Exception ex)
            {
                //Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                ErrorLog.LogError(ex, "QualityMonitoring.viewPdfFileUpload2TierATR(string id)");
                return Json(string.Empty, JsonRequestBehavior.AllowGet);
            }
            finally
            {
                dbContext.Dispose();
            }
        }


        [HttpGet]
        [Audit]
        public ActionResult ViewVerificationATR_ListByRoadCode_Inspdate_ObsId_ATRId_2Tier(string VerificationATRCode, int? page, int? rows, String sidx, String sord, string filters)
        {
            QualityMonitoringDAL qualityDAL = new QualityMonitoringDAL();

            long totalrecords;

            try
            {
                string[] tempVerificationATRcode = Request.Params["VerificationATRCode"].Split('$');

                // Get Observation ID and Road Code
                int roadCode = Convert.ToInt32(tempVerificationATRcode[0]);
                string inspectionDate = tempVerificationATRcode[1];
                int SRI_or_U_SQM_ObservationId = Convert.ToInt32(tempVerificationATRcode[2]);

                //Below line is commented on 20-01-2023
                //int NQM_ATRId = Convert.ToInt32(tempVerificationATRcode[3]);

                //Below  two lines are  Added on 20-01-2023
                dbContext = new PMGSYEntities();
                int NQM_ATRId = dbContext.QUALITY_SQM_ATR_FILE.Where(x => x.QM_OBSERVATION_ID == SRI_or_U_SQM_ObservationId && x.ATR_REGRADE_STATUS != "R").Select(x => x.QM_ATR_ID).FirstOrDefault();

                List<String> selectedIdList = new List<String>();

                var jsonData = new
                {
                    rows = qualityDAL.Verification2TierATRListByObsId_RoadCodeDAL(roadCode, inspectionDate, SRI_or_U_SQM_ObservationId, NQM_ATRId, page, rows, sidx, sord, out totalrecords, filters, out selectedIdList),
                    total = totalrecords <= Convert.ToInt32(rows) ? 1 : (totalrecords % Convert.ToInt32(rows) == 0 ? totalrecords / Convert.ToInt32(rows) : totalrecords / Convert.ToInt32(rows) + 1),
                    page = Convert.ToInt32(page),
                    records = totalrecords,
                    userdata = new { ids = selectedIdList.ToArray<string>() }
                };
                return Json(jsonData, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "QualityMonitoringController.ViewVerificationATR_ListByRoadCode_Inspdate_ObsId_ATRId_2Tier()");
                return Json(new { Success = false, ErrorMessage = "An Error Occurred While Processing Your Request." });
            }
        }


        [HttpGet]
        [Audit]
        public ActionResult ViewVerificationRejectedATR_ListByRoadCode_Inspdate_ObsId_ATRId_2Tier(string VerificationATRCode, int? page, int? rows, String sidx, String sord, string filters)
        {
            QualityMonitoringDAL qualityDAL = new QualityMonitoringDAL();

            long totalrecords;

            try
            {
                string[] tempVerificationATRcode = Request.Params["VerificationATRCode"].Split('$');

                // Get Observation ID and Road Code
                int roadCode = Convert.ToInt32(tempVerificationATRcode[0]);
                string inspectionDate = tempVerificationATRcode[1];
                int SRI_or_U_SQM_ObservationId = Convert.ToInt32(tempVerificationATRcode[2]);

                //Below line is commented on 20-01-2023
                //int NQM_ATRId = Convert.ToInt32(tempVerificationATRcode[3]);

                //Below  two lines are  Added on 20-01-2023
                dbContext = new PMGSYEntities();
                int NQM_ATRId = dbContext.QUALITY_SQM_ATR_FILE.Where(x => x.QM_OBSERVATION_ID == SRI_or_U_SQM_ObservationId && x.ATR_REGRADE_STATUS == "R").Select(x => x.QM_ATR_ID).FirstOrDefault();

                List<String> selectedIdList = new List<String>();

                var jsonData = new
                {
                    rows = qualityDAL.Verification2TierATRListByObsId_RoadCodeDAL(roadCode, inspectionDate, SRI_or_U_SQM_ObservationId, NQM_ATRId, page, rows, sidx, sord, out totalrecords, filters, out selectedIdList),
                    total = totalrecords <= Convert.ToInt32(rows) ? 1 : (totalrecords % Convert.ToInt32(rows) == 0 ? totalrecords / Convert.ToInt32(rows) : totalrecords / Convert.ToInt32(rows) + 1),
                    page = Convert.ToInt32(page),
                    records = totalrecords,
                    userdata = new { ids = selectedIdList.ToArray<string>() }
                };
                return Json(jsonData, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "QualityMonitoringController.ViewVerificationRejectedATR_ListByRoadCode_Inspdate_ObsId_ATRId_2Tier()");
                return Json(new { Success = false, ErrorMessage = "An Error Occurred While Processing Your Request." });
            }
        }
        [Audit]
        public JsonResult List2TierPDFFiles(FormCollection formCollection)
        {
            dbContext = new PMGSYEntities();
            qualityBAL = new QualityMonitoringBAL();
            try
            {
                //Adde By Abhishek kamble 29-Apr-2014 start
                using (CommonFunctions commonFunction = new CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new GridParams(Convert.ToInt32(formCollection["page"]), Convert.ToInt32(formCollection["rows"]), formCollection["sidx"], formCollection["sord"], Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                    {
                        return null;
                    }
                }
                //Adde By Abhishek kamble 29-Apr-2014 end
                Int32 QM_OBSERVATION_ID = 0, QM_ATR_ID = 0;
                if (Request.Params["QM_OBSERVATION_ID"] != "0" && Request.Params["QM_OBSERVATION_ID"] != null && Request.Params["QM_OBSERVATION_ID"] != string.Empty)
                {
                    QM_OBSERVATION_ID = Convert.ToInt32(Request.Params["QM_OBSERVATION_ID"]);
                }
                if (Request.Params["QM_ATR_ID"] != "0" && Request.Params["QM_ATR_ID"] != null && Request.Params["QM_ATR_ID"] != string.Empty)
                {
                    QM_ATR_ID = Convert.ToInt32(Request.Params["QM_ATR_ID"]);
                }

                int totalRecords;
                var jsonData = new
                {
                    rows = qualityBAL.Get2TierPDFFilesListBAL(Convert.ToInt32(formCollection["page"]) - 1, Convert.ToInt32(formCollection["rows"]), formCollection["sidx"], formCollection["sord"], out totalRecords, QM_OBSERVATION_ID, QM_ATR_ID),
                    total = totalRecords <= Convert.ToInt32(formCollection["rows"]) ? 1 : (totalRecords % Convert.ToInt32(formCollection["rows"]) == 0 ? totalRecords / Convert.ToInt32(formCollection["rows"]) : (totalRecords / Convert.ToInt32(formCollection["rows"]) + 1)),
                    page = Convert.ToInt32(formCollection["page"]),
                    records = totalRecords
                };
                return Json(jsonData, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "QualityMonitoringController.List2TierPDFFiles()");
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return Json(string.Empty);
            }
            finally
            {
                dbContext.Dispose();
            }
        }


        public JsonResult savefinalizedMarkVerification2TierATRFile(FormCollection formData)
        {

            string message = null;
            try
            {

                //string[] tempVerificationATRcode = nqmobsidatrid.Split('$');

                //int SQM_ObservationId = submitarray[0]; //Convert.ToInt32(Request.Params["submitSQMId"]);
                //int NQM_ObservationId = Convert.ToInt32(tempVerificationATRcode[2]);
                //int NQM_ATRid = Convert.ToInt32(tempVerificationATRcode[3]);



                //else if (count > 0 && missFileFlag.Equals("N"))
                //{
                //    //PdfFileUpload(formData);
                //}
                string[] tempVerificationATRcode = formData["sqmobsidatrid"].Split('$');

                int SQM_ObservationId = Convert.ToInt32(formData["submitarray"]); //Convert.ToInt32(Request.Params["submitSQMId"]);
                int SRIorU_NQM_ObservationId = Convert.ToInt32(tempVerificationATRcode[2]);
                int SQM_ATRid = Convert.ToInt32(tempVerificationATRcode[3]);

                //-------------------Added by Shreyas on 14-06-2023 -------------------//If Eform OR Insp.Report is not uploaded then ATR upload provision will not be available
                dbContext = new PMGSYEntities();
                var scheduleDetails = dbContext.QUALITY_QM_OBSERVATION_MASTER.Where(s => s.QM_OBSERVATION_ID == SQM_ObservationId).Select(s => new { s.ADMIN_SCHEDULE_CODE, s.IMS_PR_ROAD_CODE }).FirstOrDefault();
                var EformData = dbContext.EFORM_PDF_UPLOAD_DETAIL.
                               Join(dbContext.EFORM_MASTER,
                               pdf => pdf.EFORM_ID,
                               em => em.EFORM_ID,
                               (pdf, em) => new { PDF = pdf, EM = em })
                               .Where(x => x.PDF.USER_TYPE == "Q" && x.PDF.IS_FINALISED == "Y" && x.EM.IS_VALID == "Y" && x.EM.IMS_PR_ROAD_CODE == scheduleDetails.IMS_PR_ROAD_CODE && x.EM.ADMIN_SCHEDULE_CODE == scheduleDetails.ADMIN_SCHEDULE_CODE).Any();

                var InspReportData = dbContext.QUALITY_INSPECTION_REPORT_FILE.Where(s => s.QM_OBSERVATION_ID == SQM_ObservationId).Any();

                if (EformData == false && InspReportData == false)
                {
                    return Json(new { success = false, message = "Eform or Inspection report is not uploaded for selected inspection." }, JsonRequestBehavior.AllowGet);
                }
                //---------------


                #region File Upload 
                int count = Request.Files.Count;
                //string missFileFlag = formData["missFileflag"];
                //if (count > 0 && missFileFlag.Equals("N"))
                //{
                //    string ErrorMessage;
                //    ErrorMessage = PdfFileUploadATR(formData, Request);
                //    if (ErrorMessage != string.Empty && ErrorMessage != null)
                //    {
                //        return Json(new { success = false, message = ErrorMessage });
                //    }
                //}
                //else
                //{
                if (count > 0)
                {
                    HttpPostedFileBase file = Request.Files[0];
                    int fileSizeLimit = 15 * 1024 * 1024; //15mb
                    if (file.ContentLength < fileSizeLimit)
                    {
                        if (file == null || file.ContentLength == 0 && Path.GetExtension(file.FileName).ToLower() != ".pdf")
                        {

                            return Json(new { success = false, message = "File is invalid." });

                        }

                    }
                    else
                    {
                        return Json(new { success = false, message = "Invalid file size. Please upload file upto 15 MB." });
                    }
                }

                if (count == 0)
                {
                    dbContext = new PMGSYEntities();
                    //if file already uploaded and mark for atr verfication is pending , then atrRegradeStatus will not be rejected
                    //if file already uploaded but it is rejected by cqc , then atrRegradeStatus will be rejected

                    string atrRegradeStatus = dbContext.QUALITY_SQM_ATR_FILE.Where(x => x.QM_ATR_ID == SQM_ATRid).Any() ? dbContext.QUALITY_SQM_ATR_FILE.Where(x => x.QM_ATR_ID == SQM_ATRid).Select(x => x.ATR_REGRADE_STATUS).FirstOrDefault() : null;
                    if (!(dbContext.QUALITY_SQM_ATR_FILE.Where(x => x.QM_ATR_ID == SQM_ATRid).Any() && atrRegradeStatus != "R"))
                    {
                        return Json(new { success = false, message = "Please select ATR file." });
                    }
                }


                // }
                #endregion

                QualityMonitoringDAL qualityDAL = new QualityMonitoringDAL();

                //bool status = qualityDAL.savefinalizedMarkVerificationATRDAL(SQM_ObservationId, NQM_ObservationId, NQM_ATRid);
                bool status = qualityDAL.savefinalizedMarkVerification2TierATRFileDAL(SQM_ObservationId, SRIorU_NQM_ObservationId, SQM_ATRid, Request);
                if (status)
                    message = "Mark for ATR Verification Finalization Successfully";
                else
                    message = "ERROR !!  Changes are not saved.";

                return Json(new { success = status, message = message });
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "QualityMonitoringController.savefinalizedMarkVerification2TierATRFile()");
                return Json(new { success = false, message = "An error occured while processing your request." });
            }
            finally
            {
                dbContext.Dispose();
            }
        }



        [RequiredAuthentication]
        public JsonResult viewATR2PdfVirtualDir(String parameter, String hash, String key)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            try
            {
                Int32 AtrId = 0;
                if (!String.IsNullOrEmpty(parameter) && !String.IsNullOrEmpty(hash) && !String.IsNullOrEmpty(key))
                {
                    String[] urlParams = URLEncrypt.DecryptParameters(new String[] { parameter, hash, key });
                    if (urlParams.Length >= 1)
                    {
                        AtrId = Convert.ToInt32(urlParams[0]);
                    }
                }


                string uploaded_path = dbContext.QUALITY_SQM_ATR_FILE.Where(x => x.QM_ATR_ID == AtrId).Select(s => s.ATR_FILE_PATH).FirstOrDefault();
                QUALITY_SQM_ATR_FILE model = dbContext.QUALITY_SQM_ATR_FILE.Where(s => s.QM_ATR_ID == AtrId).FirstOrDefault();
                var fileName = dbContext.QUALITY_SQM_ATR_FILE.Where(x => x.QM_ATR_ID == AtrId).Select(s => s.ATR_FILE_NAME).FirstOrDefault();


                string VirtualDirectoryPath = ConfigurationManager.AppSettings["QUALITY_INSPECTION_FILE_UPLOAD_2TIER_ATR_VIRTUAL_DIR_PATH"].ToString();
                string VirtualDirectoryfullPath = Path.Combine(VirtualDirectoryPath, Convert.ToString(model.ATR_ENTRY_DATE.Year + "/" + model.ATR_ENTRY_DATE.Month), fileName.ToString()).ToString().Replace(@"\\", @"//").Replace(@"\", @"/");

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
                ErrorLog.LogError(ex, "qualityController.viewATR2PdfVirtualDir()");
                return null;
            }
            finally
            {
                dbContext.Dispose();
            }

        }



        [HttpGet]
        [Audit]
        public ActionResult Download2TierATRFile(String parameter, String hash, String key)
        {
            string FileName = string.Empty;
            string FullFileLogicalPathName = string.Empty;
            string FullfilePhysicalPathName = string.Empty;
            string FullFileLogicalPathId = string.Empty;
            string FullfilePhysicalPathId = string.Empty;
            string FileExtension = string.Empty;
            string PhysicalPath = string.Empty;
            string VirtualPath = string.Empty;
            // changes for above two lines by saurabh...
            Int32 ObsOrAtrId = 0;
            dbContext = new PMGSYEntities();
            try
            {
                string DownloadFilePath = "";
                if (!String.IsNullOrEmpty(parameter) && !String.IsNullOrEmpty(hash) && !String.IsNullOrEmpty(key))
                {
                    String[] urlParams = URLEncrypt.DecryptParameters(new String[] { parameter, hash, key });
                    if (urlParams.Length >= 1)
                    {
                        String[] urlSplitParams = urlParams[0].Split('$');
                        // Changes done by saurabh
                        if (urlSplitParams.Length == 2)
                        {
                            FileName = (urlSplitParams[0]);
                            ObsOrAtrId = Convert.ToInt32(urlSplitParams[1]);
                        }
                        else
                        {
                            FileName = (urlSplitParams[0]) + '$' + (urlSplitParams[1]);
                            ObsOrAtrId = Convert.ToInt32(urlSplitParams[2]);
                        }
                        // changes ended by saurabh
                    }
                }
                FileExtension = Path.GetExtension(FileName).ToLower();

                if (FileExtension == ".pdf")
                {
                    var twoTierPhysicalPath = dbContext.QUALITY_SQM_ATR_FILE.Where(s => s.QM_ATR_ID == ObsOrAtrId).Select(x => x.ATR_FILE_PATH).FirstOrDefault();
                    var twoTierFileName = dbContext.QUALITY_SQM_ATR_FILE.Where(s => s.QM_ATR_ID == ObsOrAtrId).Select(x => x.ATR_FILE_NAME).FirstOrDefault();
                    var model = dbContext.QUALITY_SQM_ATR_FILE.Where(s => s.QM_ATR_ID == ObsOrAtrId).FirstOrDefault();

                    //In case of if File With Name not Found then find with Id, This is case particularly for ATR
                    FullFileLogicalPathId = Path.Combine(ConfigurationManager.AppSettings["QUALITY_INSPECTION_FILE_UPLOAD_2TIER_ATR_VIRTUAL_DIR_PATH"], ObsOrAtrId.ToString() + ".pdf");
                    FullfilePhysicalPathId = Path.Combine(ConfigurationManager.AppSettings["QUALITY_INSPECTION_FILE_UPLOAD_2TIER_ATR"], ObsOrAtrId.ToString() + ".pdf");

                    FullFileLogicalPathName = Path.Combine(ConfigurationManager.AppSettings["QUALITY_INSPECTION_FILE_UPLOAD_2TIER_ATR_VIRTUAL_DIR_PATH"] + "//" + model.ATR_ENTRY_DATE.Year + "//" + model.ATR_ENTRY_DATE.Month, FileName);
                    FullfilePhysicalPathName = Path.Combine(twoTierPhysicalPath, FileName);

                    #region
                    using (System.IO.StreamWriter sw = System.IO.File.AppendText(System.IO.Path.Combine("D:\\ErrorLog\\OMMASErrorLog_" + System.DateTime.Now.Date.ToString("dd_MM_yyyy") + ".txt", "")))
                    {
                        sw.WriteLine("---------------------------------------------------------------------------------------");
                        sw.WriteLine("webconfigpath : " + ConfigurationManager.AppSettings["QUALITY_INSPECTION_FILE_UPLOAD_2TIER_ATR"]);
                        sw.WriteLine("QMDownloadPath : " + FullfilePhysicalPathName);
                        sw.WriteLine("QMvirtualpath : " + FullFileLogicalPathId);
                        sw.WriteLine("logicalpathname : " + FullFileLogicalPathName);
                        sw.WriteLine("physicalpathname : " + FullfilePhysicalPathName);
                        sw.WriteLine("---------------------------------------------------------------------------------------");
                        sw.Close();
                    }
                    #endregion
                }
                else if (FileExtension == ".jpg" || FileExtension == ".jpeg" || FileExtension == ".bmp" || FileExtension == ".png" || FileExtension == ".gif" || FileExtension == ".tiff")
                {
                    string QM_FILE_NAME = Request.Params["QM_FILE_NAME"];
                    var qmTypeForUser = (from aqm in dbContext.ADMIN_QUALITY_MONITORS
                                         join qqs in dbContext.QUALITY_QM_SCHEDULE on aqm.ADMIN_QM_CODE equals qqs.ADMIN_QM_CODE
                                         join qqom in dbContext.QUALITY_QM_OBSERVATION_MASTER on qqs.ADMIN_SCHEDULE_CODE equals qqom.ADMIN_SCHEDULE_CODE
                                         where qqom.QM_OBSERVATION_ID == ObsOrAtrId
                                         select aqm).First();
                    // Changes Done by saurabh Starts Here....
                    if (FileName.ToString().Contains("$") == true)
                    {
                        DownloadFilePath = Convert.ToString(FileName.ToString().Split('$')[0]);  // CHANGES BY SAURABH TO SPLIT FILE NAME

                        ///Path to upload files for NQM/SQM/CQC/SQC
                        if (qmTypeForUser.ADMIN_QM_TYPE.Equals("I"))   //CQC & NQM
                        {
                            VirtualPath = Path.Combine(ConfigurationManager.AppSettings["QUALITY_INSPECTION_FILE_UPLOAD_NQM_VIRTUAL_DIR_PATH_NEW"], DownloadFilePath);
                            PhysicalPath = Path.Combine(ConfigurationManager.AppSettings["QUALITY_INSPECTION_FILE_UPLOAD_NQM_NEW"], DownloadFilePath);

                            FullfilePhysicalPathName = Path.Combine(PhysicalPath, FileName);
                            FullFileLogicalPathName = Path.Combine(VirtualPath, FileName);


                        }
                        else if (qmTypeForUser.ADMIN_QM_TYPE.Equals("S"))   //SQM & SQC
                        {
                            VirtualPath = Path.Combine(ConfigurationManager.AppSettings["QUALITY_INSPECTION_FILE_UPLOAD_SQM_VIRTUAL_DIR_PATH_NEW"], DownloadFilePath);
                            PhysicalPath = Path.Combine(ConfigurationManager.AppSettings["QUALITY_INSPECTION_FILE_UPLOAD_SQM_NEW"], DownloadFilePath);

                            FullfilePhysicalPathName = Path.Combine(PhysicalPath, FileName);
                            FullFileLogicalPathName = Path.Combine(VirtualPath, FileName);


                        }
                    }
                    else
                    {


                        ///Path to upload files for NQM/SQM/CQC/SQC
                        if (qmTypeForUser.ADMIN_QM_TYPE.Equals("I"))   //CQC & NQM
                        {
                            FullFileLogicalPathName = Path.Combine(ConfigurationManager.AppSettings["QUALITY_INSPECTION_FILE_UPLOAD_NQM_VIRTUAL_DIR_PATH"], FileName);
                            FullfilePhysicalPathName = Path.Combine(ConfigurationManager.AppSettings["QUALITY_INSPECTION_FILE_UPLOAD_NQM"], FileName);
                        }
                        else if (qmTypeForUser.ADMIN_QM_TYPE.Equals("S"))   //SQM & SQC
                        {
                            FullFileLogicalPathName = Path.Combine(ConfigurationManager.AppSettings["QUALITY_INSPECTION_FILE_UPLOAD_SQM_VIRTUAL_DIR_PATH"], FileName);
                            FullfilePhysicalPathName = Path.Combine(ConfigurationManager.AppSettings["QUALITY_INSPECTION_FILE_UPLOAD_SQM"], FileName);
                        }
                    }
                    // Changes done by saurabh ends here....
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
                        case ".doc":
                        case ".docx":
                            type = "Application/msword";
                            break;
                        case ".jpg":
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

                if (System.IO.File.Exists(FullfilePhysicalPathName))
                {
                    return File(FullfilePhysicalPathName, type, DateTime.Now.ToShortDateString() + "_" + new Random().Next(1000000000) + FileExtension);
                }
                else if (System.IO.File.Exists(FullfilePhysicalPathId))
                {
                    return File(FullfilePhysicalPathId, type, DateTime.Now.ToShortDateString() + "_" + new Random().Next(1000000000) + FileExtension);
                }
                else
                {
                    return Json(new { Error = "File Not Exist" }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                //Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                ErrorLog.LogError(ex, "QualityMonitoring.Download2TierATRFile()");
                return Json(new { Success = "false" }, JsonRequestBehavior.AllowGet);
            }
            finally
            {
                dbContext.Dispose();
            }
        }




        [Audit]
        public ActionResult QM2TierATRRegrade(string id)
        {
            QMATRRegradeModel qmATRRegradeModel = new QMATRRegradeModel();
            qmATRRegradeModel.QM_OBSERVATION_ID = Convert.ToInt32(id);
            try
            {
                dbContext = new PMGSYEntities();
                Int32 rdcode = Convert.ToInt32(id);
                Int32 RoadCode = dbContext.QUALITY_QM_OBSERVATION_MASTER.Where(x => x.QM_OBSERVATION_ID == rdcode).Select(m => m.IMS_PR_ROAD_CODE).FirstOrDefault();
                string RoadName = dbContext.IMS_SANCTIONED_PROJECTS.Where(x => x.IMS_PR_ROAD_CODE == RoadCode).Select(m => m.IMS_ROAD_NAME).FirstOrDefault();

                string PackageID = dbContext.IMS_SANCTIONED_PROJECTS.Where(x => x.IMS_PR_ROAD_CODE == RoadCode).Select(m => m.IMS_PACKAGE_ID).FirstOrDefault();
                Int32 StateCode = dbContext.IMS_SANCTIONED_PROJECTS.Where(x => x.IMS_PR_ROAD_CODE == RoadCode).Select(m => m.MAST_STATE_CODE).FirstOrDefault();
                String StateName = dbContext.MASTER_STATE.Where(x => x.MAST_STATE_CODE == StateCode).Select(m => m.MAST_STATE_NAME).FirstOrDefault();

                Int32 DistrictCode = dbContext.IMS_SANCTIONED_PROJECTS.Where(x => x.IMS_PR_ROAD_CODE == RoadCode).Select(m => m.MAST_DISTRICT_CODE).FirstOrDefault();
                string DistrictName = dbContext.MASTER_DISTRICT.Where(x => x.MAST_DISTRICT_CODE == DistrictCode).Select(m => m.MAST_DISTRICT_NAME).FirstOrDefault();

                Int32 BlockCode = dbContext.IMS_SANCTIONED_PROJECTS.Where(x => x.IMS_PR_ROAD_CODE == RoadCode).Select(m => m.MAST_BLOCK_CODE).FirstOrDefault();
                string BlockName = dbContext.MASTER_BLOCK.Where(x => x.MAST_BLOCK_CODE == BlockCode).Select(m => m.MAST_BLOCK_NAME).FirstOrDefault();


                var atrReasonList = dbContext.MASTER_ATR_REGRADE_REASONS.Where(s => s.IS_ACTIVE == "Y" && s.TIER_TYPE == 2).ToList();
                qmATRRegradeModel.lstReasons = new List<SelectListItem>();
                qmATRRegradeModel.reasonCode = 0;
                List<SelectListItem> reasonList = new List<SelectListItem>();
                SelectListItem item1;
                item1 = new SelectListItem();
                item1.Text = "----------------------------------------------------------Select Reason-----------------------------------------------------------";
                item1.Value = "0";
                item1.Selected = true;
                reasonList.Add(item1);
                foreach (var item in atrReasonList)
                {
                    item1 = new SelectListItem();
                    item1.Text = item.REASON_NAME;
                    item1.Value = item.REASON_ID.ToString();
                    reasonList.Add(item1);
                }
                qmATRRegradeModel.lstReasons = reasonList;



                qmATRRegradeModel.ROAD_NAME = RoadName;

                qmATRRegradeModel.PACKAGE_ID = PackageID;
                qmATRRegradeModel.StateName = StateName;
                qmATRRegradeModel.DistrictName = DistrictName;
                qmATRRegradeModel.BlockName = BlockName;


                // qmATRRegradeModel.ROAD_NAME = RoadName;
                // qmATRRegradeModel.PACKAGE_ID = PackageID;
                //fetch monitor name 
                int? schCode = dbContext.QUALITY_QM_OBSERVATION_MASTER.Where(x => x.QM_OBSERVATION_ID == rdcode).Select(m => m.ADMIN_SCHEDULE_CODE).FirstOrDefault();



                if (PMGSYSession.Current.RoleCode == 8 || PMGSYSession.Current.RoleCode == 48 || PMGSYSession.Current.RoleCode == 69)
                {
                    return View(qmATRRegradeModel);
                }
                else
                {
                    return View("QMMAINTATRRegrade", qmATRRegradeModel);
                }
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "QualityMonitoringController.QM2TierATRRegrade(string id)");
                return View(qmATRRegradeModel);
            }
            finally
            {
                dbContext.Dispose();
            }
        }


        [HttpPost]
        [Audit]
        public ActionResult QM2TierSaveATRRegrade(QMATRRegradeModel qmATRRegradeModel)
        {
            qualityBAL = new QualityMonitoringBAL();

            try
            {
                if (ModelState.IsValid)
                {
                    string Status = qualityBAL.QM2TierSaveATRRegradeBAL(qmATRRegradeModel);
                    if (Status == string.Empty)
                        return Json(new { Success = true });
                    else
                        return Json(new { Success = false, ErrorMessage = Status });
                }
                else
                {
                    StringBuilder errorMessages = new StringBuilder();
                    foreach (var modelStateValue in ModelState.Values)
                    {
                        foreach (var error in modelStateValue.Errors)
                        {
                            errorMessages.Append(error.ErrorMessage);
                        }
                    }
                    return Json(new { Success = false, ErrorMessage = errorMessages.ToString() });
                }
            }
            catch (Exception ex)
            {
                //Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                ErrorLog.LogError(ex, "QualityMonitoring.QM2TierSaveATRRegrade()");
                return Json(new { Success = false, ErrorMessage = "Error occured while saving details" });
            }



        }





        public ActionResult QMView2TierInspectionListAgainstRoadNewTab(String roadCode1)
        {
            QualityMonitoringDAL qmDAL = new QualityMonitoringDAL();
            QM2TierATRInspdetailsModel inspDetailsModel = new QM2TierATRInspdetailsModel();
            inspDetailsModel.INSP_AGAINST_ROAD_LIST = new List<QMATRINSPModel>();

            dbContext = new PMGSYEntities();

            try
            {

                string[] encParam = roadCode1.Split('/');
                decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { encParam[0], encParam[1], encParam[2] });

                int roadCode = 0;
                if (decryptedParameters.Count > 0)
                {
                    roadCode = Convert.ToInt32(decryptedParameters["imsRoadID"].Split('$')[0]);

                }




                // int roadCode = Convert.ToInt32(roadCode1.Split('$')[0]);

                inspDetailsModel.INSP_LIST = qmDAL.ATR2TierInspDetailsDAL(roadCode);//ATR_Chang


                var inspectionList = inspDetailsModel.INSP_LIST.ToList();





                foreach (var item in inspectionList)
                {
                    QMATRINSPModel obsModel = new QMATRINSPModel();
                    obsModel.ADMIN_QM_TYPE = item.ADMIN_QM_TYPE == "S" ? "SQM" : "NQM";
                    obsModel.ATR_VERIFICATION_FINALIZED = item.ATR_VERIFICATION_FINALIZED == "Y" ? "Yes" : "No";
                    obsModel.MONITOR_NAME = item.MONITOR_NAME.ToString().Split('-')[0];
                    obsModel.STATE_NAME = item.STATE_NAME;
                    obsModel.DISTRICT_NAME = item.DISTRICT_NAME;
                    obsModel.BLOCK_NAME = item.BLOCK_NAME;
                    obsModel.IMS_ROAD_NAME = item.IMS_ROAD_NAME;
                    obsModel.IMS_PACKAGE_ID = item.IMS_PACKAGE_ID;

                    obsModel.IMS_PROPOSAL_TYPE = item.IMS_PROPOSAL_TYPE.Equals("P") ? "Road" : "LSB";
                    obsModel.PMGSY_SCHEME = item.PMGSY_SCHEME;
                    obsModel.IMS_ISCOMPLETED = item.IMS_ISCOMPLETED;
                    obsModel.IMS_PAV_LENGTH = item.IMS_PROPOSAL_TYPE.Equals("P") ? item.IMS_PAV_LENGTH.ToString() : (item.IMS_BRIDGE_LENGTH == null ? "" : item.IMS_BRIDGE_LENGTH.ToString());
                    obsModel.QM_INSPECTED_START_CHAINAGE = item.QM_INSPECTED_START_CHAINAGE;
                    obsModel.QM_INSPECTED_END_CHAINAGE = item.QM_INSPECTED_END_CHAINAGE;
                    obsModel.QM_INSPECTION_DATE = item.QM_INSPECTION_DATE.ToString();
                    obsModel.OVERALL_GRADE = item.OVERALL_GRADE;
                    obsModel.NO_OF_PHOTO_UPLOADED = item.NO_OF_PHOTO_UPLOADED;

                    obsModel.INSPECTION_REPORT_LINK = (dbContext.QUALITY_INSPECTION_REPORT_FILE.Where(c => c.QM_OBSERVATION_ID == item.QM_OBSERVATION_ID).Any())
                                        ? "<a href='#' title='Click here to view details' class='ui-icon ui-icon-zoomin ui-align-center' onClick='ViewInspectionReportATR(\"" + item.QM_OBSERVATION_ID.ToString().Trim() + "\"); return false;'>View</a>" : "-";

                    obsModel.OBS_LINK = "<a href='#' title='Click here to view details' class='ui-icon ui-icon-zoomin ui-align-center' onClick='ShowATRObsDetailsNewtab(\"" + item.QM_OBSERVATION_ID.ToString().Trim() + "\"); return false;'>View</a>";


                    inspDetailsModel.INSP_AGAINST_ROAD_LIST.Add(obsModel);
                }

                return View("~/Views/QualityMonitoring/InspectionListAgainstRoadsqmATR.cshtml", inspDetailsModel);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "QualitymonitoringController.QMView2TierInspectionListAgainstRoadNewTab()");
                inspDetailsModel.ERROR = "Error occurred while processing your request";
                return View("~/Views/QualityMonitoring/InspectionListAgainstRoadsqmATR.cshtml", inspDetailsModel);
            }
            finally
            {
                dbContext.Dispose();
            }
        }
        #endregion























        /// <summary>
        /// View Observation List for ATR
        /// </summary>
        /// <param name="formCollection"></param>
        /// <returns></returns>
        [HttpPost]
        [Audit]
        public ActionResult QMViewATRDetails(FormCollection formCollection)
        {
            qualityBAL = new QualityMonitoringBAL();
            int totalRecords;
            try
            {
                //Adde By Abhishek kamble 29-Apr-2014 start
                using (CommonFunctions commonFunction = new CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new GridParams(Convert.ToInt32(formCollection["page"]), Convert.ToInt32(formCollection["rows"]), formCollection["sidx"], formCollection["sord"], Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                    {
                        return null;
                    }
                }
                //Adde By Abhishek kamble 29-Apr-2014 end
                var jsonData = new
                {
                    rows = qualityBAL.QMViewATRDetailsBAL(Convert.ToInt32(formCollection["page"]) - 1, Convert.ToInt32(formCollection["rows"]),
                                                            formCollection["sidx"], formCollection["sord"], out totalRecords,
                                                            Convert.ToInt32(formCollection["stateCode"]), Convert.ToInt32(formCollection["monitorCode"]),
                                                            Convert.ToInt32(formCollection["fromInspMonth"]), Convert.ToInt32(formCollection["fromInspYear"]),
                                                            Convert.ToInt32(formCollection["toInspMonth"]), Convert.ToInt32(formCollection["toInspYear"]),
                                                            formCollection["atrStatus"], formCollection["rdStatusATR"]),
                    total = totalRecords <= Convert.ToInt32(formCollection["rows"]) ? 1 : (totalRecords % Convert.ToInt32(formCollection["rows"]) == 0 ? totalRecords / Convert.ToInt32(formCollection["rows"]) : (totalRecords / Convert.ToInt32(formCollection["rows"]) + 1)),
                    page = Convert.ToInt32(formCollection["page"]),
                    records = totalRecords
                };
                return Json(jsonData);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return Json(String.Empty);
            }
        }

        /// <summary>
        /// View for Bulk ATR Details
        /// </summary>
        /// <returns></returns>
        public ActionResult QMBulkATRDetails()
        {
            QMFilterViewModel qmFilterModel = new QMFilterViewModel();
            CommonFunctions objCommonFunctions = new CommonFunctions();
            qmFilterModel.MAST_STATE_CODE = 0;
            qmFilterModel.STATES = objCommonFunctions.PopulateStates(false);
            qmFilterModel.ATR_SUBMIT_DURATION = 1;
            qmFilterModel.ATR_SUBMIT_DURATION_LIST = objCommonFunctions.QualityATRSubmitDuration();
            return View(qmFilterModel);
        }

        /// <summary>
        /// View ATR List for Bulk Regrade
        /// </summary>
        /// <param name="formCollection"></param>
        /// <returns></returns>
        [HttpPost]
        [Audit]
        public ActionResult QMViewBulkATRList(FormCollection formCollection)
        {
            qualityBAL = new QualityMonitoringBAL();
            int totalRecords;
            try
            {
                //Added By Abhishek kamble 29-Apr-2014 start
                using (CommonFunctions commonFunction = new CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new GridParams(Convert.ToInt32(formCollection["page"]), Convert.ToInt32(formCollection["rows"]), formCollection["sidx"], formCollection["sord"], Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                    {
                        return null;
                    }
                }
                var jsonData = new
                {
                    rows = qualityBAL.QMViewBulkATRListBAL(Convert.ToInt32(formCollection["page"]) - 1, Convert.ToInt32(formCollection["rows"]),
                                                            formCollection["sidx"], formCollection["sord"], out totalRecords,
                                                            Convert.ToInt32(formCollection["stateCode"]), Convert.ToInt32(formCollection["duration"])),
                    total = totalRecords <= Convert.ToInt32(formCollection["rows"]) ? 1 : (totalRecords % Convert.ToInt32(formCollection["rows"]) == 0 ? totalRecords / Convert.ToInt32(formCollection["rows"]) : (totalRecords / Convert.ToInt32(formCollection["rows"]) + 1)),
                    page = Convert.ToInt32(formCollection["page"]),
                    records = totalRecords
                };
                return Json(jsonData);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return Json(String.Empty);
            }
        }


        /// <summary>
        /// Regrade all selected ATRs
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult QMBulkRegrade()
        {
            //string[] obsIds = null;
            //QMATRRegradeModel qmATRRegradeModel = new QMATRRegradeModel();
            //qualityBAL = new QualityMonitoringBAL();
            dbContext = new PMGSYEntities();
            try
            {
                //if (!string.IsNullOrEmpty(Request.Params["regradeData[]"]))
                //{
                //    obsIds = (Request.Params["regradeData[]"].Split(','));
                //}

                //// May be in future requirement of reject ATRs in bulk may come so provision of remarks & regradeStatus is here
                //foreach (var obsId in obsIds)
                //{
                //    qmATRRegradeModel.QM_OBSERVATION_ID = Convert.ToInt32(obsId);
                //    qmATRRegradeModel.ATR_REGRADE_STATUS = Request.Params["regradeStatus"];
                //    qmATRRegradeModel.ATR_REGRADE_REMARKS = Request.Params["remarks"];
                //    qualityBAL.QMATRRegradeBAL(qmATRRegradeModel);
                //}

                if (!string.IsNullOrEmpty(Request.Params["regradeData[]"]))
                {
                    dbContext.USP_QM_ATR_REGRADE_BULK(Request.Params["regradeData[]"]);
                    dbContext.SaveChanges();
                    return Json(new { Success = true, Message = "Selected ATRs Regraded successfully." });
                }
                else
                    return Json(new { Success = false, Message = "Please select atleast one record to Regrade." });
            }
            catch
            {
                return Json(new { Success = false, Message = "An Error Occurred While Processing Your Request." });
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        /// <summary>
        /// Render View for Previous Schedules
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Audit]
        public ActionResult QMPreviousSchedules()
        {
            qualityBAL = new QualityMonitoringBAL();
            CommonFunctions objCommonFunctions = new CommonFunctions();
            try
            {
                QMPreviousScheduleModel qmPrevSchedules = new QMPreviousScheduleModel();

                qmPrevSchedules.ADMIN_QM_CODE = 0;
                qmPrevSchedules.ADMIN_IM_MONTH = DateTime.Now.Month;
                qmPrevSchedules.ADMIN_IM_YEAR = DateTime.Now.Year;

                //if (PMGSYSession.Current.RoleCode == 5)  //CQC
                if (PMGSYSession.Current.RoleCode == 9 || PMGSYSession.Current.RoleCode == 5)  //CQC
                {
                    qmPrevSchedules.MONITORS = new CommonFunctions().PopulateMonitors("false", "I", 0);
                }
                if (PMGSYSession.Current.RoleCode == 8 || PMGSYSession.Current.RoleCode == 69 || PMGSYSession.Current.RoleCode == 48) //SQC
                {
                    qmPrevSchedules.MONITORS = new CommonFunctions().PopulateMonitors("false", "S", PMGSYSession.Current.StateCode);
                }

                qmPrevSchedules.MONTHS_LIST = objCommonFunctions.PopulateMonths(false);
                qmPrevSchedules.YEARS_LIST = objCommonFunctions.PopulateYears(false);

                return View(qmPrevSchedules);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return null;
            }
        }


        /// <summary>
        /// Populate Schedule List Current Month Onwards
        /// </summary>
        /// <param name="formCollection"></param>
        /// <returns></returns>
        [HttpPost]
        [Audit]
        public ActionResult GetPrevScheduleList(FormCollection formCollection)
        {
            qualityBAL = new QualityMonitoringBAL();
            int totalRecords;
            try
            {
                //Adde By Abhishek kamble 29-Apr-2014 start
                using (CommonFunctions commonFunction = new CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new GridParams(Convert.ToInt32(formCollection["page"]), Convert.ToInt32(formCollection["rows"]), formCollection["sidx"], formCollection["sord"], Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                    {
                        return null;
                    }
                }
                //Adde By Abhishek kamble 29-Apr-2014 end
                var jsonData = new
                {
                    rows = qualityBAL.GetPrevScheduleListBAL(Convert.ToInt32(formCollection["page"]) - 1, Convert.ToInt32(formCollection["rows"]),
                                                            formCollection["sidx"], formCollection["sord"], out totalRecords,
                                                            Convert.ToInt32(formCollection["prevSchMonitorId"]), Convert.ToInt32(formCollection["prevSchMonthID"]),
                                                            Convert.ToInt32(formCollection["prevSchYearID"]), false),
                    total = totalRecords <= Convert.ToInt32(formCollection["rows"]) ? 1 : (totalRecords % Convert.ToInt32(formCollection["rows"]) == 0 ? totalRecords / Convert.ToInt32(formCollection["rows"]) : (totalRecords / Convert.ToInt32(formCollection["rows"]) + 1)),
                    page = Convert.ToInt32(formCollection["page"]),
                    records = totalRecords
                };
                return Json(jsonData);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return Json(String.Empty);
            }
        }



        /// <summary>
        /// Get for Displaying Grades for Correction
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Audit]
        public ActionResult QMGradingCorrection(string id)
        {
            dbContext = new PMGSYEntities();
            qualityBAL = new QualityMonitoringBAL();
            try
            {
                QMFillObservationModel fillObservationModel = qualityBAL.QMGradingCorrectionBAL(Convert.ToInt32(id));
                fillObservationModel.QM_OBSERVATION_ID = Convert.ToInt32(id);
                return View(fillObservationModel);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return null;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        /// <summary>
        /// Post to save Corrected Grades
        /// </summary>
        /// <param name="formCollection"></param>
        /// <returns></returns>
        [HttpPost]
        [Audit]
        public ActionResult QMGradingCorrection(FormCollection formCollection)
        {
            qualityBAL = new QualityMonitoringBAL();

            if (ModelState.IsValid)
            {
                string Status = qualityBAL.QMGradingCorrectionBAL(formCollection);
                if (Status == string.Empty)
                    return Json(new { Success = true });
                else
                    return Json(new { Success = false, ErrorMessage = Status });
            }
            else
            {
                StringBuilder errorMessages = new StringBuilder();
                foreach (var modelStateValue in ModelState.Values)
                {
                    foreach (var error in modelStateValue.Errors)
                    {
                        errorMessages.Append(error.ErrorMessage);
                    }
                }
                return Json(new { Success = false, ErrorMessage = errorMessages.ToString() });
            }
        }


        /// <summary>
        /// Delete Observation Details from Master/Details/Inspection File
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Audit]
        public ActionResult QMDeleteObservation()
        {
            qualityBAL = new QualityMonitoringBAL();

            try
            {
                Int32 ObservationId = Convert.ToInt32(Request.Params["obsId"]);
                if (ObservationId != 0)
                {
                    string Status = qualityBAL.QMDeleteObservationBAL(ObservationId);
                    if (Status == string.Empty)
                        return Json(new { Success = true });
                    else
                        return Json(new { Success = false, ErrorMessage = Status });
                }
                else
                {
                    return Json(new { Success = false, ErrorMessage = "An Error Occurred While Processing Your Request." });
                }
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return Json(new { Success = false, ErrorMessage = "An Error Occurred While Processing Your Request." });
            }
        }


        /// <summary>
        /// Get Observation Details as  for particular Observation
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Audit]
        public ActionResult QMObservationDetails(string id)
        {
            qualityBAL = new QualityMonitoringBAL();
            try
            {
                QMFillObservationModel fillObservationModel = qualityBAL.QMObservationDetailsBAL(Convert.ToInt32(id));
                fillObservationModel.QM_OBSERVATION_ID = Convert.ToInt32(id);
                fillObservationModel.IS_ATR_PAGE = "N";
                return View(fillObservationModel);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return null;
            }
        }

        /// <summary>
        /// 2Tier Details on Cqc Login
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Audit]
        public ActionResult QMObservationDetails2TierCQC(string id)
        {
            qualityBAL = new QualityMonitoringBAL();
            try
            {
                QMFillObservationModel fillObservationModel = qualityBAL.QMObservationDetails2TierCQCBAL(Convert.ToInt32(id));
                fillObservationModel.QM_OBSERVATION_ID = Convert.ToInt32(id);
                return View(fillObservationModel);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return null;
            }
        }


        /// <summary>
        /// Schedule Filters View to 2 Tier in CQCAdmin
        /// </summary>
        /// <returns></returns>
        public ActionResult QualityFilters2TierCQCSchedule()
        {
            QMFilterViewModel qmFilterModel = new QMFilterViewModel();
            CommonFunctions objCommonFunctions = new CommonFunctions();

            qmFilterModel.MAST_STATE_CODE = 0;
            qmFilterModel.FROM_MONTH = DateTime.Now.Month;
            qmFilterModel.FROM_YEAR = DateTime.Now.Year;
            qmFilterModel.FROM_MONTHS_LIST = objCommonFunctions.PopulateMonths(false);
            qmFilterModel.FROM_YEARS_LIST = objCommonFunctions.PopulateYears(false).Where(a => a.Value != "0").ToList();
            qmFilterModel.STATES = objCommonFunctions.PopulateStates(true);
            return View(qmFilterModel);
        }

        [HttpPost]
        public ActionResult Get2TierScheduleListCQC(FormCollection formCollection)
        {
            qualityBAL = new QualityMonitoringBAL();
            int totalRecords;
            try
            {
                //Adde By Abhishek kamble 29-Apr-2014 start
                using (CommonFunctions commonFunction = new CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new GridParams(Convert.ToInt32(formCollection["page"]), Convert.ToInt32(formCollection["rows"]), formCollection["sidx"], formCollection["sord"], Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                    {
                        return null;
                    }
                }
                //Adde By Abhishek kamble 29-Apr-2014 end
                var jsonData = new
                {
                    rows = qualityBAL.Get2TierScheduleListCQCBAL(Convert.ToInt32(formCollection["state"]), Convert.ToInt32(formCollection["month"]), Convert.ToInt32(formCollection["year"]),
                                                         Convert.ToInt32(formCollection["page"]) - 1, Convert.ToInt32(formCollection["rows"]),
                                                            formCollection["sidx"], formCollection["sord"], out totalRecords, formCollection["filters"]),
                    total = totalRecords <= Convert.ToInt32(formCollection["rows"]) ? 1 : (totalRecords % Convert.ToInt32(formCollection["rows"]) == 0 ? totalRecords / Convert.ToInt32(formCollection["rows"]) : (totalRecords / Convert.ToInt32(formCollection["rows"]) + 1)),
                    page = Convert.ToInt32(formCollection["page"]),
                    records = totalRecords
                };
                return Json(jsonData);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return Json(String.Empty);
            }
        }


        /// <summary>
        /// Monitor Details(SQM) for CQCAdmin 
        /// </summary>
        /// <returns></returns>
        public ActionResult QualityFilters2TierCQCMonitorList()
        {
            try
            {
                QMFilterViewModel qmFilterModel = new QMFilterViewModel();
                CommonFunctions objCommonFunctions = new CommonFunctions();
                qmFilterModel.MAST_STATE_CODE = 0;
                qmFilterModel.STATES = objCommonFunctions.PopulateStates(true);

                List<SelectListItem> empanelledList = new List<SelectListItem>();
                empanelledList.Insert(0, (new SelectListItem { Text = "Yes", Value = "Y", Selected = true }));
                empanelledList.Insert(1, (new SelectListItem { Text = "No", Value = "N" }));
                qmFilterModel.EmpanelledList = empanelledList;

                return View(qmFilterModel);
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// Get Monitor List
        /// </summary>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <param name="sidx"></param>
        /// <param name="sord"></param>
        /// <returns></returns>
        public ActionResult ListQualityMonitors(FormCollection formCollection)
        {
            String searchParameters = String.Empty;
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            long totalRecords;
            int stateCode = 0;
            int districtCode = 0;
            string isEmpanelled = String.Empty;
            string firstName = String.Empty;
            string qmTypeName = String.Empty;

            try
            {
                qualityBAL = new QualityMonitoringBAL();
                using (CommonFunctions commonFunction = new CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new GridParams(Convert.ToInt32(formCollection["page"]), Convert.ToInt32(formCollection["rows"]), formCollection["sidx"], formCollection["sord"], Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                    {
                        return null;
                    }
                }

                if (!string.IsNullOrEmpty(Request.Params["QmTypeName"]))
                {
                    qmTypeName = Request.Params["QmTypeName"].Replace('+', ' ').Trim();
                }

                if (!string.IsNullOrEmpty(Request.Params["stateCode"]))
                {
                    stateCode = Convert.ToInt32(Request.Params["stateCode"]);
                }

                if (!string.IsNullOrEmpty(Request.Params["districtCode"]))
                {
                    districtCode = Convert.ToInt32(Request.Params["districtCode"]);
                }

                if (!string.IsNullOrEmpty(Request.Params["isEmpanelled"]))
                {
                    isEmpanelled = Request.Params["isEmpanelled"];
                }

                var jsonData = new
                {
                    rows = qualityBAL.ListQualityMonitor(qmTypeName, stateCode, districtCode, isEmpanelled, Request.Params["filters"], Convert.ToInt32(formCollection["page"]) - 1, Convert.ToInt32(formCollection["rows"]), formCollection["sidx"], formCollection["sord"], out totalRecords),
                    total = totalRecords <= Convert.ToInt32(formCollection["rows"]) ? 1 : (totalRecords % Convert.ToInt32(formCollection["rows"]) == 0 ? totalRecords / Convert.ToInt32(formCollection["rows"]) : (totalRecords / Convert.ToInt32(formCollection["rows"]) + 1)),
                    page = Convert.ToInt32(formCollection["page"]),
                    records = totalRecords
                };
                return Json(jsonData, JsonRequestBehavior.AllowGet);

            }
            catch (Exception)
            {
                return null;
            }
        }
        #endregion


        #region SQC 3-Tier


        /// <summary>
        /// View Inspection Details - 3rd Tier to SQC
        /// </summary>
        /// <param name="formCollection"></param>
        /// <returns></returns>
        [HttpPost]
        [Audit]
        public ActionResult QMViewInspectionDetailsSQCPIU(FormCollection formCollection)
        {
            qualityBAL = new QualityMonitoringBAL();
            int totalRecords;
            try
            {
                //Added By Abhishek kamble 29-Apr-2014 start to Validate Grid Params
                using (CommonFunctions commonFunction = new CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new GridParams(Convert.ToInt32(formCollection["page"]), Convert.ToInt32(formCollection["rows"]), formCollection["sidx"], formCollection["sord"], Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                    {
                        return null;
                    }
                }
                //Added By Abhishek kamble 29-Apr-2014 end
                var jsonData = new
                {
                    rows = qualityBAL.QMViewInspectionDetailsSQCPIUBAL(Convert.ToInt32(formCollection["page"]) - 1, Convert.ToInt32(formCollection["rows"]),
                                                            formCollection["sidx"], formCollection["sord"], out totalRecords,
                                                            Convert.ToInt32(formCollection["stateCode"]), Convert.ToInt32(formCollection["monitorCode"]),
                                                            Convert.ToInt32(formCollection["fromInspMonth"]), Convert.ToInt32(formCollection["fromInspYear"]),
                                                            Convert.ToInt32(formCollection["toInspMonth"]), Convert.ToInt32(formCollection["toInspYear"]), formCollection["qmType"], Convert.ToInt32(formCollection["schemeType"]), formCollection["roadStatus"], formCollection["roadOrBridge"], formCollection["gradeType"], formCollection["eFormStatus"]),
                    total = totalRecords <= Convert.ToInt32(formCollection["rows"]) ? 1 : (totalRecords % Convert.ToInt32(formCollection["rows"]) == 0 ? totalRecords / Convert.ToInt32(formCollection["rows"]) : (totalRecords / Convert.ToInt32(formCollection["rows"]) + 1)),
                    page = Convert.ToInt32(formCollection["page"]),
                    records = totalRecords
                };
                return Json(jsonData);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return Json(String.Empty);
            }
        }

        /// <summary>
        /// Schedule List for 3 Tier in SQC account
        /// </summary>
        /// <param name="formCollection"></param>
        /// <returns></returns>
        [HttpPost]
        [Audit]
        public ActionResult GetSqc3TierScheduleList(FormCollection formCollection)
        {
            qualityBAL = new QualityMonitoringBAL();
            int totalRecords;
            try
            {
                //Adde By Abhishek kamble 29-Apr-2014 start
                using (CommonFunctions commonFunction = new CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new GridParams(Convert.ToInt32(formCollection["page"]), Convert.ToInt32(formCollection["rows"]), formCollection["sidx"], formCollection["sord"], Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                    {
                        return null;
                    }
                }
                //Adde By Abhishek kamble 29-Apr-2014 end
                var jsonData = new
                {
                    rows = qualityBAL.GetSqc3TierScheduleListBAL(Convert.ToInt32(formCollection["month"]), Convert.ToInt32(formCollection["year"]),
                                                            Convert.ToInt32(formCollection["page"]) - 1, Convert.ToInt32(formCollection["rows"]),
                                                            formCollection["sidx"], formCollection["sord"], out totalRecords, formCollection["filters"]),
                    total = totalRecords <= Convert.ToInt32(formCollection["rows"]) ? 1 : (totalRecords % Convert.ToInt32(formCollection["rows"]) == 0 ? totalRecords / Convert.ToInt32(formCollection["rows"]) : (totalRecords / Convert.ToInt32(formCollection["rows"]) + 1)),
                    page = Convert.ToInt32(formCollection["page"]),
                    records = totalRecords
                };
                return Json(jsonData);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return Json(String.Empty);
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [Audit]
        public ActionResult QMPreviousSchedules3TierSQC()
        {
            qualityBAL = new QualityMonitoringBAL();
            CommonFunctions objCommonFunctions = new CommonFunctions();
            try
            {
                QMPreviousScheduleModel qmPrevSchedules = new QMPreviousScheduleModel();

                qmPrevSchedules.ADMIN_QM_CODE = 0;
                qmPrevSchedules.ADMIN_IM_MONTH = DateTime.Now.Month;
                qmPrevSchedules.ADMIN_IM_YEAR = DateTime.Now.Year;

                qmPrevSchedules.MONITORS = qualityBAL.PopulateMonitorsBAL(PMGSYSession.Current.StateCode, DateTime.Now.Month, DateTime.Now.Year, PMGSYSession.Current.DistrictCode, "I");
                qmPrevSchedules.MONTHS_LIST = objCommonFunctions.PopulateMonths(false);
                qmPrevSchedules.YEARS_LIST = objCommonFunctions.PopulateYears(false);
                qmPrevSchedules.QM_TYPE = "I";

                return View(qmPrevSchedules);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return null;
            }
        }


        /// <summary>
        /// Get Listing for Previous schedules for NQM in SQC Login
        /// </summary>
        /// <param name="formCollection"></param>
        /// <returns></returns>
        [HttpPost]
        [Audit]
        public ActionResult GetPrevScheduleList3TierSQC(FormCollection formCollection)
        {
            qualityBAL = new QualityMonitoringBAL();
            int totalRecords;
            try
            {
                //Adde By Abhishek kamble 29-Apr-2014 start
                using (CommonFunctions commonFunction = new CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new GridParams(Convert.ToInt32(formCollection["page"]), Convert.ToInt32(formCollection["rows"]), formCollection["sidx"], formCollection["sord"], Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                    {
                        return null;
                    }
                }
                //Adde By Abhishek kamble 29-Apr-2014 end
                bool is3Tier = false;
                if (PMGSYSession.Current.RoleCode == 8) //SQC
                {
                    is3Tier = true;                     //for SQC is3Tier sent to true
                }
                else if (PMGSYSession.Current.RoleCode == 22 || PMGSYSession.Current.RoleCode == 38 || PMGSYSession.Current.RoleCode == 54) //PIU or PIURCPLWE
                {
                    if (formCollection["is3TierPIU"].Equals("3Tier"))
                    {
                        is3Tier = true;              //It is for PIU 3 Tier            
                    }
                    else
                        is3Tier = false;             //It is for PIU 2 Tier
                }
                var jsonData = new
                {
                    //Difference for GetPrevScheduleList & this method is of Last parameter as true or false
                    rows = qualityBAL.GetPrevScheduleListBAL(Convert.ToInt32(formCollection["page"]) - 1, Convert.ToInt32(formCollection["rows"]),
                                                            formCollection["sidx"], formCollection["sord"], out totalRecords,
                                                            Convert.ToInt32(formCollection["prevSchMonitorId"]), Convert.ToInt32(formCollection["prevSchMonthID"]),
                                                            Convert.ToInt32(formCollection["prevSchYearID"]), is3Tier),
                    total = totalRecords <= Convert.ToInt32(formCollection["rows"]) ? 1 : (totalRecords % Convert.ToInt32(formCollection["rows"]) == 0 ? totalRecords / Convert.ToInt32(formCollection["rows"]) : (totalRecords / Convert.ToInt32(formCollection["rows"]) + 1)),
                    page = Convert.ToInt32(formCollection["page"]),
                    records = totalRecords
                };
                return Json(jsonData);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return Json(String.Empty);
            }
        }

        /// <summary>
        /// Populate NQMs in SQC Login
        /// NOMs should be populated excluding NQMs of current state
        /// </summary>
        /// <param name="inspMonth"></param>
        /// <param name="inspYear"></param>
        /// <param name="districtCode"></param>
        /// <returns></returns>
        [Audit]
        public ActionResult Populate3TierSQCMonitors(int inspMonth, int inspYear, int districtCode, string qmType)
        {
            qualityBAL = new QualityMonitoringBAL();
            try
            {
                return Json(qualityBAL.PopulateMonitorsBAL(PMGSYSession.Current.StateCode, inspMonth, inspYear, (PMGSYSession.Current.DistrictCode == 0 ? districtCode : PMGSYSession.Current.DistrictCode), qmType));
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return null;
            }
        }

        /// <summary>
        /// Get Observation Details as  for particular Observation for SQC (3rd Tier)
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Audit]
        public ActionResult QMObservationDetails3TierSQC(string id)
        {
            qualityBAL = new QualityMonitoringBAL();
            try
            {
                QMFillObservationModel fillObservationModel = qualityBAL.QMObservationDetails3TierSQCBAL(Convert.ToInt32(id));
                fillObservationModel.QM_OBSERVATION_ID = Convert.ToInt32(id);
                return View(fillObservationModel);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return null;
            }
        }

        #endregion


        #region PIU

        /// <summary>
        /// Render Schedule List of NQMs for PIU 
        /// </summary>
        /// <param name="formCollection"></param>
        /// <returns></returns>
        [HttpPost]
        [Audit]
        public ActionResult GetPIU3TierScheduleList(FormCollection formCollection)
        {
            qualityBAL = new QualityMonitoringBAL();
            int totalRecords;
            try
            {
                //Adde By Abhishek kamble 29-Apr-2014 start
                using (CommonFunctions commonFunction = new CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new GridParams(Convert.ToInt32(formCollection["page"]), Convert.ToInt32(formCollection["rows"]), formCollection["sidx"], formCollection["sord"], Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                    {
                        return null;
                    }
                }
                //Adde By Abhishek kamble 29-Apr-2014 end
                var jsonData = new
                {
                    rows = qualityBAL.GetPIU3TierScheduleListBAL(Convert.ToInt32(formCollection["page"]) - 1, Convert.ToInt32(formCollection["rows"]),
                                                            formCollection["sidx"], formCollection["sord"], out totalRecords, formCollection["filters"]),
                    total = totalRecords <= Convert.ToInt32(formCollection["rows"]) ? 1 : (totalRecords % Convert.ToInt32(formCollection["rows"]) == 0 ? totalRecords / Convert.ToInt32(formCollection["rows"]) : (totalRecords / Convert.ToInt32(formCollection["rows"]) + 1)),
                    page = Convert.ToInt32(formCollection["page"]),
                    records = totalRecords
                };
                return Json(jsonData);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return Json(String.Empty);
            }
        }

        /// <summary>
        /// Render Schedule List of SQMs for PIU 
        /// </summary>
        /// <param name="formCollection"></param>
        /// <returns></returns>
        [Audit]
        public ActionResult GetPIU2TierScheduleList(FormCollection formCollection)
        {
            qualityBAL = new QualityMonitoringBAL();
            int totalRecords;
            try
            {
                //Adde By Abhishek kamble 29-Apr-2014 start
                using (CommonFunctions commonFunction = new CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new GridParams(Convert.ToInt32(formCollection["page"]), Convert.ToInt32(formCollection["rows"]), formCollection["sidx"], formCollection["sord"], Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                    {
                        return null;
                    }
                }
                //Adde By Abhishek kamble 29-Apr-2014 end

                var jsonData = new
                {
                    rows = qualityBAL.GetPIU2TierScheduleListBAL(Convert.ToInt32(formCollection["page"]) - 1, Convert.ToInt32(formCollection["rows"]),
                                                            formCollection["sidx"], formCollection["sord"], out totalRecords, formCollection["filters"]),
                    total = totalRecords <= Convert.ToInt32(formCollection["rows"]) ? 1 : (totalRecords % Convert.ToInt32(formCollection["rows"]) == 0 ? totalRecords / Convert.ToInt32(formCollection["rows"]) : (totalRecords / Convert.ToInt32(formCollection["rows"]) + 1)),
                    page = Convert.ToInt32(formCollection["page"]),
                    records = totalRecords
                };
                return Json(jsonData);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return Json(String.Empty);
            }
        }


        /// <summary>
        /// Render Previous Schedule's List of SQMs in PIU Login
        /// </summary>
        /// <returns></returns>
        [Audit]
        public ActionResult QMPreviousSchedules2TierPIU()
        {
            qualityBAL = new QualityMonitoringBAL();
            CommonFunctions objCommonFunctions = new CommonFunctions();
            try
            {
                QMPreviousScheduleModel qmPrevSchedules = new QMPreviousScheduleModel();

                qmPrevSchedules.ADMIN_QM_CODE = 0;
                qmPrevSchedules.ADMIN_IM_MONTH = DateTime.Now.Month;
                qmPrevSchedules.ADMIN_IM_YEAR = DateTime.Now.Year;

                qmPrevSchedules.MONITORS = qualityBAL.PopulateMonitorsBAL(PMGSYSession.Current.StateCode, DateTime.Now.Month, DateTime.Now.Year, PMGSYSession.Current.DistrictCode, "S");
                qmPrevSchedules.MONTHS_LIST = objCommonFunctions.PopulateMonths(false);
                qmPrevSchedules.YEARS_LIST = objCommonFunctions.PopulateYears(false);
                qmPrevSchedules.QM_TYPE = "S";

                return View("QMPreviousSchedules3TierSQC", qmPrevSchedules);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return null;
            }
        }


        #endregion


        #region Monitors
        /// <summary>
        /// Get List of Assigned Districts for Monitor for current month onwards
        /// </summary>
        /// <param name="formCollection"></param>
        /// <returns></returns>
        [HttpPost]
        [Audit]
        public ActionResult GetMonitorsCurrScheduleList(FormCollection formCollection)
        {
            qualityBAL = new QualityMonitoringBAL();
            int totalRecords;
            try
            {   //Adde By Abhishek kamble 29-Apr-2014 start
                using (CommonFunctions commonFunction = new CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new GridParams(Convert.ToInt32(formCollection["page"]), Convert.ToInt32(formCollection["rows"]), formCollection["sidx"], formCollection["sord"], Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                    {
                        return null;
                    }
                }
                //Adde By Abhishek kamble 29-Apr-2014 end
                var jsonData = new
                {
                    rows = qualityBAL.GetMonitorsCurrScheduleListBAL(Convert.ToInt32(formCollection["month"]), Convert.ToInt32(formCollection["year"]),
                                                            Convert.ToInt32(formCollection["page"]) - 1, Convert.ToInt32(formCollection["rows"]),
                                                            formCollection["sidx"], formCollection["sord"], out totalRecords),
                    total = totalRecords <= Convert.ToInt32(formCollection["rows"]) ? 1 : (totalRecords % Convert.ToInt32(formCollection["rows"]) == 0 ? totalRecords / Convert.ToInt32(formCollection["rows"]) : (totalRecords / Convert.ToInt32(formCollection["rows"]) + 1)),
                    page = Convert.ToInt32(formCollection["page"]),
                    records = totalRecords
                };
                return Json(jsonData);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return Json(String.Empty);
            }
        }


        /// <summary>
        /// Inspection List for Monitor as per selected Month & Year
        /// </summary>
        /// <param name="formCollection"></param>
        /// <returns></returns>
        [HttpPost]
        [Audit]
        public ActionResult QMMonitorInspList(FormCollection formCollection)
        {
            qualityBAL = new QualityMonitoringBAL();
            int totalRecords;
            try
            {
                //Adde By Abhishek kamble 29-Apr-2014 start
                using (CommonFunctions commonFunction = new CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new GridParams(Convert.ToInt32(formCollection["page"]), Convert.ToInt32(formCollection["rows"]), formCollection["sidx"], formCollection["sord"], Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                    {
                        return null;
                    }
                }
                //Adde By Abhishek kamble 29-Apr-2014 end
                var jsonData = new
                {
                    rows = qualityBAL.QMMonitorInspListBAL(Convert.ToInt32(formCollection["page"]) - 1, Convert.ToInt32(formCollection["rows"]),
                                                            formCollection["sidx"], formCollection["sord"], out totalRecords,
                                                            Convert.ToInt32(formCollection["fromInspMonth"]), Convert.ToInt32(formCollection["fromInspYear"]),
                                                            Convert.ToInt32(formCollection["toInspMonth"]), Convert.ToInt32(formCollection["toInspYear"])),
                    total = totalRecords <= Convert.ToInt32(formCollection["rows"]) ? 1 : (totalRecords % Convert.ToInt32(formCollection["rows"]) == 0 ? totalRecords / Convert.ToInt32(formCollection["rows"]) : (totalRecords / Convert.ToInt32(formCollection["rows"]) + 1)),
                    page = Convert.ToInt32(formCollection["page"]),
                    records = totalRecords
                };
                return Json(jsonData);
            }
            catch (Exception ex)
            {
                //Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                ErrorLog.LogError(ex, "QualityMonitoring.QMMonitorInspList()");
                return Json(String.Empty);
            }
        }


        /// <summary>
        /// Filters for Monitors
        /// </summary>
        /// <returns></returns>
        [Audit]
        public ActionResult QualityMonitorFilters()
        {
            QMFilterViewModel qmFilterModel = new QMFilterViewModel();
            CommonFunctions objCommonFunctions = new CommonFunctions();

            //qmFilterModel.QM_TYPE_CODE = "I";
            qmFilterModel.MAST_STATE_CODE = 0;
            qmFilterModel.ADMIN_QM_CODE = 0;

            qmFilterModel.FROM_MONTH = DateTime.Now.AddMonths(-1).Month;
            qmFilterModel.FROM_YEAR = DateTime.Now.Month == 1 ? (DateTime.Now.Year - 1) : DateTime.Now.Year;
            qmFilterModel.TO_MONTH = DateTime.Now.Month;
            qmFilterModel.TO_YEAR = DateTime.Now.Year;

            qmFilterModel.FROM_MONTHS_LIST = objCommonFunctions.PopulateMonths(false);
            qmFilterModel.FROM_YEARS_LIST = objCommonFunctions.PopulateYears(false);
            qmFilterModel.TO_MONTHS_LIST = objCommonFunctions.PopulateMonths(false);
            qmFilterModel.TO_YEARS_LIST = objCommonFunctions.PopulateYears(false);
            return View(qmFilterModel);
        }


        /// <summary>
        /// Render Observation Details View
        /// </summary>
        /// <returns></returns>
        [Audit]
        public ActionResult QualityMonitorsObsDetails()
        {
            QMObsSchListModel qmObsSchListModel = new QMObsSchListModel();
            CommonFunctions objCommonFunctions = new CommonFunctions();

            qmObsSchListModel.ADMIN_IM_MONTH = DateTime.Now.Month;
            qmObsSchListModel.ADMIN_IM_YEAR = DateTime.Now.Year;

            qmObsSchListModel.MONTHS_LIST = objCommonFunctions.PopulateMonths(false);
            qmObsSchListModel.YEARS_LIST = objCommonFunctions.PopulateYears(false);

            return View(qmObsSchListModel);
        }


        /// <summary>
        /// Actual Schedule for selcted month & year for Monitors,
        /// against which they can fill observations
        /// </summary>
        /// <param name="formCollection"></param>
        /// <returns></returns>
        [HttpPost]
        [Audit]
        public ActionResult GetMonitorsScheduledRoadList(FormCollection formCollection)
        {
            qualityBAL = new QualityMonitoringBAL();
            int totalRecords;
            try
            {
                //Adde By Abhishek kamble 29-Apr-2014 start
                using (CommonFunctions commonFunction = new CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new GridParams(Convert.ToInt32(formCollection["page"]), Convert.ToInt32(formCollection["rows"]), formCollection["sidx"], formCollection["sord"], Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                    {
                        return null;
                    }
                }
                //Adde By Abhishek kamble 29-Apr-2014 end
                var jsonData = new
                {
                    rows = qualityBAL.GetMonitorsScheduledRoadListBAL(Convert.ToInt32(formCollection["page"]) - 1, Convert.ToInt32(formCollection["rows"]),
                                                            formCollection["sidx"], formCollection["sord"], out totalRecords,
                                                            Convert.ToInt32(formCollection["inspMonth"]), Convert.ToInt32(formCollection["inspYear"])),
                    total = totalRecords <= Convert.ToInt32(formCollection["rows"]) ? 1 : (totalRecords % Convert.ToInt32(formCollection["rows"]) == 0 ? totalRecords / Convert.ToInt32(formCollection["rows"]) : (totalRecords / Convert.ToInt32(formCollection["rows"]) + 1)),
                    page = Convert.ToInt32(formCollection["page"]),
                    records = totalRecords
                };
                return Json(jsonData);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return Json(String.Empty);
            }
        }


        [Audit]
        //public ActionResult QMFillObservations(string id1, string id2)
        public ActionResult QMFillObservations(string data)
        {
            dbContext = new PMGSYEntities();
            qualityBAL = new QualityMonitoringBAL();
            try
            {

                String[] urlSplitParams = data.Split('$');

                string id1 = urlSplitParams[0];
                string id2 = urlSplitParams[1];
                string id3 = urlSplitParams[2];


                QMFillObservationModel fillObservationsModel = qualityBAL.QMFillObservationsBAL(Convert.ToInt32(id1), Convert.ToInt32(id2), id3);
                fillObservationsModel.ADMIN_SCHEDULE_CODE = Convert.ToInt32(id1);
                fillObservationsModel.IMS_PR_ROAD_CODE = Convert.ToInt32(id2);


                fillObservationsModel.BearingTypeList = new List<SelectListItem>();

                fillObservationsModel.BearingTypeList.Insert(0, new SelectListItem { Text = "Rocker and Pin Bearing", Value = "1", Selected = true });

                fillObservationsModel.BearingTypeList.Insert(1, new SelectListItem { Text = "Roller Bearing", Value = "2", Selected = true });

                fillObservationsModel.BearingTypeList.Insert(2, new SelectListItem { Text = "Pot Bearing", Value = "3", Selected = true });

                fillObservationsModel.BearingTypeList.Insert(3, new SelectListItem { Text = "Elastomeric Bearing", Value = "4", Selected = true });

                fillObservationsModel.BearingTypeList.Insert(4, new SelectListItem { Text = "Any other Bearing", Value = "5", Selected = true });
                //Set Date 1 as schedule months & Years date for Comparision, 
                var schDetails = dbContext.QUALITY_QM_SCHEDULE.Find(fillObservationsModel.ADMIN_SCHEDULE_CODE);
                fillObservationsModel.SCHEDULE_MONTH_YEAR_START_DATE = "01/" + (schDetails.ADMIN_IM_MONTH < 10 ? ("0" + schDetails.ADMIN_IM_MONTH) : schDetails.ADMIN_IM_MONTH.ToString()) + "/" + schDetails.ADMIN_IM_YEAR;
                fillObservationsModel.SCHEDULE_MONTH_YEAR_END_DATE = DateTime.DaysInMonth(schDetails.ADMIN_IM_YEAR, schDetails.ADMIN_IM_MONTH).ToString() + "/" + (schDetails.ADMIN_IM_MONTH < 10 ? ("0" + schDetails.ADMIN_IM_MONTH) : schDetails.ADMIN_IM_MONTH.ToString()) + "/" + schDetails.ADMIN_IM_YEAR;// Added by deendayal on 15/9/2017 to restrict the monitor to select any other month's day
                return View(fillObservationsModel);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return Json(String.Empty);
            }
            finally
            {
                dbContext.Dispose();
            }
        }


        /// <summary>
        /// Save Grading in Observation for Monitors
        /// </summary>
        /// <param name="fillObservationModel"></param>
        /// <returns></returns>
        [HttpPost]
        [Audit]
        public ActionResult QMFillObservations(QMFillObservationModel fillObservationModel, FormCollection formCollection)
        {
            qualityBAL = new QualityMonitoringBAL();
            try
            {

                if (ModelState.IsValid)
                {
                    // Validate Remarks against unsatisfactory grades
                    foreach (var key in formCollection.AllKeys)
                    {
                        if (key.Contains("remarks"))
                        {
                            var Remarks = formCollection[key];
                            if (Remarks != null && Remarks != string.Empty)
                            {
                                Match match = Regex.Match(Remarks, @"^[a-zA-Z0-9 ,.()-]+$");
                                if (!match.Success)
                                {
                                    return Json(new { Success = false, ErrorMessage = "Invalid Remarks, It is Required Field and Can only contains AlphaNumeric values and ,.()-" });
                                }
                            }
                        }
                    }

                    string Status = qualityBAL.QMSaveObservationsBAL(formCollection);
                    if (Status == string.Empty)
                        return Json(new { Success = true });
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
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return Json(new { Success = false, ErrorMessage = "Error ocurred while processing your request" });
            }
        }

        #endregion


        #region Tour Details


        /// <summary>
        /// Common Filters for Quality Module
        /// </summary>
        /// <returns></returns>
        [Audit]
        public ActionResult QualityTourFilters()
        {
            QMFilterViewModel qmFilterModel = new QMFilterViewModel();
            CommonFunctions objCommonFunctions = new CommonFunctions();

            qmFilterModel.MAST_STATE_CODE = 0;
            qmFilterModel.ADMIN_QM_CODE = 0;

            qmFilterModel.FROM_MONTH = DateTime.Now.Month;
            qmFilterModel.FROM_YEAR = DateTime.Now.Year;
            qmFilterModel.TO_MONTH = DateTime.Now.Month;
            qmFilterModel.TO_YEAR = DateTime.Now.Year;
            //if (PMGSYSession.Current.RoleCode == 5)  //CQC
            if (PMGSYSession.Current.RoleCode == 9 || PMGSYSession.Current.RoleCode == 5)  //CQCAdmin
            {
                qmFilterModel.STATES = objCommonFunctions.PopulateStates(false);
                qmFilterModel.STATES.Insert(0, (new SelectListItem { Text = "All", Value = "0", Selected = true }));

                qmFilterModel.MONITORS = objCommonFunctions.PopulateMonitors("false", "I", 0); //Purposely taken String "false" as argument
            }
            else if (PMGSYSession.Current.RoleCode == 8)  //SQC
            {
                qmFilterModel.MAST_STATE_CODE = PMGSYSession.Current.StateCode;
                qmFilterModel.MONITORS = objCommonFunctions.PopulateMonitors("false", "S", PMGSYSession.Current.StateCode); //Purposely taken String "false" as argument
            }
            qmFilterModel.FROM_MONTHS_LIST = objCommonFunctions.PopulateMonths(false);
            qmFilterModel.FROM_YEARS_LIST = objCommonFunctions.PopulateYears(false).Where(a => a.Value != "0").ToList();
            qmFilterModel.TO_MONTHS_LIST = objCommonFunctions.PopulateMonths(false);
            qmFilterModel.TO_YEARS_LIST = objCommonFunctions.PopulateYears(false).Where(a => a.Value != "0").ToList();
            return View(qmFilterModel);
        }

        /// <summary>
        /// Update or View Monitor Tour Details
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Audit]
        public ActionResult QMTourDetails(string id)
        {
            qualityBAL = new QualityMonitoringBAL();
            try
            {
                QMTourViewModel model = new QMTourViewModel();

                model = qualityBAL.GetTourDetailsBAL(Convert.ToInt32(id));

                model.AdminScheduleCode = Convert.ToInt32(id);
                model.Operation = "C";
                return View(model);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return null;
            }
        }


        /// <summary>
        /// Save Tour Details
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Audit]
        public ActionResult QMTourDetails(QMTourViewModel model)
        {
            CommonFunctions comm = new CommonFunctions();
            qualityBAL = new QualityMonitoringBAL();
            try
            {
                //if (model.Operation == "C")
                {
                    ModelState.Remove("tourReport");
                    ModelState.Remove("tourSubmissionDate");
                }
                if (ModelState.IsValid)
                {
                    if (comm.GetStringToDateTime(model.FlightArrivalDate) >= comm.GetStringToDateTime(model.FlightDepartureDate))
                    {
                        return Json(new { Success = false, ErrorMessage = "Flight Departure Date should be greater than Flight Arrival Date." });
                    }

                    string status = qualityBAL.SaveTourDetailsBAL(model);
                    if (status == string.Empty)
                        return Json(new { Success = true });
                    else
                        return Json(new { Success = false, ErrorMessage = status });
                }
                else
                {
                    return Json(new { Success = false, ErrorMessage = new CommonFunctions().FormatErrorMessage(ModelState) });
                }
            }
            catch (Exception ex)
            {
                return Json(new { Success = false, ErrorMessage = "Error ocurred while processing your request" });
            }
        }


        /// <summary>
        /// Tour List for Monitor as per selected Month & Year
        /// </summary>
        /// <param name="formCollection"></param>
        /// <returns></returns>
        [HttpPost]
        [Audit]
        public ActionResult QMTourList(FormCollection formCollection)
        {
            qualityBAL = new QualityMonitoringBAL();
            int totalRecords;
            try
            {
                //Adde By Abhishek kamble 29-Apr-2014 start
                using (CommonFunctions commonFunction = new CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new GridParams(Convert.ToInt32(formCollection["page"]), Convert.ToInt32(formCollection["rows"]), formCollection["sidx"], formCollection["sord"], Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                    {
                        return null;
                    }
                }
                var jsonData = new
                {
                    rows = qualityBAL.GetTourListBAL(Convert.ToInt32(formCollection["page"]) - 1, Convert.ToInt32(formCollection["rows"]),
                                                            formCollection["sidx"], formCollection["sord"], out totalRecords,
                                                            Convert.ToInt32(formCollection["state"]), Convert.ToInt32(formCollection["qmCode"]),
                                                            Convert.ToInt32(formCollection["frmMonth"]), Convert.ToInt32(formCollection["frmYear"]),
                                                            Convert.ToInt32(formCollection["toMonth"]), Convert.ToInt32(formCollection["toYear"])),
                    total = totalRecords <= Convert.ToInt32(formCollection["rows"]) ? 1 : (totalRecords % Convert.ToInt32(formCollection["rows"]) == 0 ? totalRecords / Convert.ToInt32(formCollection["rows"]) : (totalRecords / Convert.ToInt32(formCollection["rows"]) + 1)),
                    page = Convert.ToInt32(formCollection["page"]),
                    records = totalRecords
                };
                return Json(jsonData);
            }
            catch (Exception ex)
            {
                return Json(String.Empty);
            }
        }


        /// <summary>
        /// Get method to Update Tour Details
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Audit]
        public ActionResult QMEditTourDetails(string id)
        {
            qualityBAL = new QualityMonitoringBAL();
            try
            {
                QMTourViewModel model = new QMTourViewModel();

                model = qualityBAL.GetTourDetailsForUpdateBAL(Convert.ToInt32(id));

                model.TourId = Convert.ToInt32(id);
                model.Operation = "U";
                return View("QMTourDetails", model);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return null;
            }
        }


        /// <summary>
        /// Update Tour Details
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Audit]
        public ActionResult QMEditTourDetails(QMTourViewModel model)
        {
            CommonFunctions comm = new CommonFunctions();
            qualityBAL = new QualityMonitoringBAL();

            bool flag = false;
            string[] arr = null;

            bool fileExt = false;

            HttpPostedFileBase file = null;
            string fileTypes = string.Empty;

            string fileId = string.Empty;
            int fileSize = 0;

            string filePath = string.Empty;
            string fileSaveExt = string.Empty;
            int fileValidSize = 0;

            try
            {
                ModelState.Remove("tourReport");
                if (ModelState.IsValid)
                {
                    if (comm.GetStringToDateTime(model.FlightArrivalDate) >= comm.GetStringToDateTime(model.FlightDepartureDate))
                    {
                        return Json(new { Success = false, ErrorMessage = "Flight Departure Date should be greater than Flight Arrival Date." });
                    }
                    if (Request.Files.AllKeys.Count() > 0)
                    {
                        file = Request.Files[0];
                        if (file != null)
                        {
                            //ModelState.Add("tourReport");
                            fileTypes = ConfigurationManager.AppSettings["TOUR_FILE_VALID_FORMAT"];
                            filePath = ConfigurationManager.AppSettings["TOUR_FILE_UPLOAD"];
                            fileSize = Convert.ToInt32(ConfigurationManager.AppSettings["TOUR_FILE_MAX_SIZE"]);

                            fileExt = true;
                            fileSaveExt = Path.GetExtension(file.FileName);//file.ContentType;
                            fileSaveExt = fileSaveExt.Contains('.') ? fileSaveExt.Trim().Remove(0, 1) : fileSaveExt;
                            fileValidSize = file.ContentLength;

                            arr = fileTypes.Split('$');
                            for (int i = 0; i < arr.Length; i++)
                            {
                                if (fileSaveExt == arr[i])
                                {
                                    flag = true;
                                }
                            }

                            model.tourReport = file.FileName;
                            if (flag)
                            {
                                //model.fileSize = (short)fileValidSize;
                                //model.filePath = filePath;
                                if (Request.Files.AllKeys.Count() > 0)
                                {

                                    string status = qualityBAL.UpdateTourDetailsBAL(model);
                                    if (status == string.Empty)
                                    {
                                        Request.Files[0].SaveAs(Path.Combine(filePath, file.FileName));
                                        return Json(new { Success = true });
                                    }
                                    else
                                    {
                                        return Json(new { Success = false, ErrorMessage = status });
                                    }
                                }
                                else
                                {
                                    return Json(new { Success = false, ErrorMessage = "An Error Occurred While Processing Your Request." });
                                }
                            }
                            else
                            {
                                return Json(new { Success = false, ErrorMessage = "Invalid Extension." });
                            }
                        }
                        else
                        {
                            return Json(new { Success = false, ErrorMessage = "An Error Occurred While Processing Your Request." });
                        }
                    }
                    else
                    {
                        return Json(new { Success = false, ErrorMessage = "Please select a pdf file for Tour Report." });
                    }
                }
                else
                {
                    return Json(new { Success = false, ErrorMessage = new CommonFunctions().FormatErrorMessage(ModelState) });
                }
            }
            catch (Exception ex)
            {
                return Json(new { Success = false, ErrorMessage = "Error ocurred while processing your request" });
            }
        }


        /// <summary>
        /// Delete Tour Details
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Audit]
        public ActionResult DeleteTourDetails()
        {
            String PhysicalPath = string.Empty;
            String FullfilePhysicalPath = string.Empty;
            string IMS_FILE_NAME = Request.Params["IMS_FILE_NAME"];

            qualityBAL = new QualityMonitoringBAL();
            PMGSY.DAL.QualityMonitoring.QualityMonitoringDAL qualityDAL = new PMGSY.DAL.QualityMonitoring.QualityMonitoringDAL();
            try
            {
                Int32 tourId = Convert.ToInt32(Request.Params["tourId"]);
                if (tourId != 0)
                {
                    IMS_FILE_NAME = qualityDAL.GetTourFileDetailsDAL(tourId);
                    PhysicalPath = ConfigurationManager.AppSettings["TOUR_FILE_UPLOAD"];
                    //ThumbnailPath = Path.Combine(Path.Combine(PhysicalPath, "thumbnails"), IMS_FILE_NAME);
                    FullfilePhysicalPath = Path.Combine(ConfigurationManager.AppSettings["TOUR_FILE_UPLOAD"], IMS_FILE_NAME);
                    if (System.IO.File.Exists(FullfilePhysicalPath))
                    {
                        System.IO.File.Delete(FullfilePhysicalPath);
                    }

                    string Status = qualityBAL.DeleteTourDetailsBAL(tourId);

                    if (Status == string.Empty)
                        return Json(new { Success = true });
                    else
                        return Json(new { Success = false, ErrorMessage = Status });
                }
                else
                {
                    return Json(new { Success = false, ErrorMessage = "An Error Occurred While Processing Your Request." });
                }
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return Json(new { Success = false, ErrorMessage = "An Error Occurred While Processing Your Request." });
            }
        }


        /// <summary>
        /// Finalize Tour Details
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Audit]
        public ActionResult FinalizeTourDetails()
        {
            qualityBAL = new QualityMonitoringBAL();
            try
            {
                Int32 tourId = Convert.ToInt32(Request.Params["tourId"]);
                if (tourId != 0)
                {
                    string Status = qualityBAL.FinalizeTourDetailsBAL(tourId, "Y");
                    if (Status == string.Empty)
                        return Json(new { Success = true });
                    else
                        return Json(new { Success = false, ErrorMessage = Status });
                }
                else
                {
                    return Json(new { Success = false, ErrorMessage = "An Error Occurred While Processing Your Request." });
                }
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return Json(new { Success = false, ErrorMessage = "An Error Occurred While Processing Your Request." });
            }
        }


        /// <summary>
        /// DeFinalize Tour Details
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Audit]
        public ActionResult DeFinalizeTourDetails()
        {
            qualityBAL = new QualityMonitoringBAL();
            try
            {
                Int32 tourId = Convert.ToInt32(Request.Params["tourId"]);
                if (tourId != 0)
                {
                    string Status = qualityBAL.FinalizeTourDetailsBAL(tourId, "N");
                    if (Status == string.Empty)
                        return Json(new { Success = true });
                    else
                        return Json(new { Success = false, ErrorMessage = Status });
                }
                else
                {
                    return Json(new { Success = false, ErrorMessage = "An Error Occurred While Processing Your Request." });
                }
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return Json(new { Success = false, ErrorMessage = "An Error Occurred While Processing Your Request." });
            }
        }

        #endregion


        #region Image Upload

        /// <summary>
        /// Get mrthod to render Image Upload View
        /// </summary>
        /// <param name="id1"></param>
        /// <param name="id2"></param>
        /// <returns></returns>
        [HttpGet]
        [Audit]
        public ActionResult ImageUpload(string id)
        {
            dbContext = new PMGSYEntities();
            PMGSY.Models.QualityMonitoring.FileUploadViewModel fileUploadViewModel = new PMGSY.Models.QualityMonitoring.FileUploadViewModel();
            try
            {
                String[] urlSplitParams = id.Split('$');

                Int32 ScheduleCode = Convert.ToInt32(urlSplitParams[0]);
                Int32 PRRoadCode = Convert.ToInt32(urlSplitParams[1]);
                Int32 ObsId = Convert.ToInt32(urlSplitParams[2] != null ? urlSplitParams[2] : "0");



                fileUploadViewModel.ADMIN_SCHEDULE_CODE = ScheduleCode;
                fileUploadViewModel.IMS_PR_ROAD_CODE = PRRoadCode;
                fileUploadViewModel.QM_OBSERVATION_ID = ObsId;

                if (ObsId > 0)
                {
                    if (dbContext.QUALITY_QM_INSPECTION_FILE.Where(a => a.QM_OBSERVATION_ID == ObsId).Any())
                    {
                        fileUploadViewModel.NumberofFiles = dbContext.QUALITY_QM_INSPECTION_FILE.Where(a => a.QM_OBSERVATION_ID == ObsId).Count();
                    }
                    else
                    {
                        fileUploadViewModel.NumberofFiles = 0;
                    }
                }
                else
                {
                    if (dbContext.QUALITY_QM_INSPECTION_FILE.Where(a => a.ADMIN_SCHEDULE_CODE == fileUploadViewModel.ADMIN_SCHEDULE_CODE && a.IMS_PR_ROAD_CODE == fileUploadViewModel.IMS_PR_ROAD_CODE).Any())
                    {
                        fileUploadViewModel.NumberofFiles = dbContext.QUALITY_QM_INSPECTION_FILE.Where(a => a.ADMIN_SCHEDULE_CODE == fileUploadViewModel.ADMIN_SCHEDULE_CODE && a.IMS_PR_ROAD_CODE == fileUploadViewModel.IMS_PR_ROAD_CODE).Count();
                    }
                    else
                    {
                        fileUploadViewModel.NumberofFiles = 0;
                    }
                }


                return View("ImageUpload", fileUploadViewModel);
            }
            catch (Exception)
            {
                return null;
            }
            finally
            {
                dbContext.Dispose();
            }
        }


        /// <summary>
        /// Listing of uploaded image files
        /// </summary>
        /// <param name="formCollection"></param>
        /// <returns></returns>
        [Audit]
        public JsonResult ListFiles(FormCollection formCollection)
        {
            dbContext = new PMGSYEntities();
            qualityBAL = new QualityMonitoringBAL();
            try
            {
                //Adde By Abhishek kamble 29-Apr-2014 start
                using (CommonFunctions commonFunction = new CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new GridParams(Convert.ToInt32(formCollection["page"]), Convert.ToInt32(formCollection["rows"]), formCollection["sidx"], formCollection["sord"], Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                    {
                        return null;
                    }
                }
                //Adde By Abhishek kamble 29-Apr-2014 end
                Int32 QM_OBSERVATION_ID = 0;
                string requestObsId = Request.Params["QM_OBSERVATION_ID"];
                if (requestObsId != null && requestObsId != "0" && requestObsId != "" && !requestObsId.Equals(string.Empty))
                {
                    QM_OBSERVATION_ID = Convert.ToInt32(Request.Params["QM_OBSERVATION_ID"]);
                }
                else
                {
                    Int32 ADMIN_SCHEDULE_CODE = Convert.ToInt32(Request.Params["ADMIN_SCHEDULE_CODE"]);
                    Int32 IMS_PR_ROAD_CODE = Convert.ToInt32(Request.Params["IMS_PR_ROAD_CODE"]);
                    QM_OBSERVATION_ID = dbContext.QUALITY_QM_OBSERVATION_MASTER.Where(c => c.ADMIN_SCHEDULE_CODE == ADMIN_SCHEDULE_CODE &&
                                        c.IMS_PR_ROAD_CODE == IMS_PR_ROAD_CODE).Select(c => c.QM_OBSERVATION_ID).First();
                }

                int totalRecords;
                var jsonData = new
                {
                    rows = qualityBAL.GetFilesListBAL(Convert.ToInt32(formCollection["page"]) - 1, Convert.ToInt32(formCollection["rows"]), formCollection["sidx"], formCollection["sord"], out totalRecords, QM_OBSERVATION_ID),
                    total = totalRecords <= Convert.ToInt32(formCollection["rows"]) ? 1 : (totalRecords % Convert.ToInt32(formCollection["rows"]) == 0 ? totalRecords / Convert.ToInt32(formCollection["rows"]) : (totalRecords / Convert.ToInt32(formCollection["rows"]) + 1)),
                    page = Convert.ToInt32(formCollection["page"]),
                    records = totalRecords
                };
                return Json(jsonData);
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
        /// Get Start End Latitude/Longitude
        /// </summary>
        /// <returns></returns>
        [Audit]
        public ActionResult GetStartEndLatLong()
        {
            qualityBAL = new QualityMonitoringBAL();
            try
            {
                int obsId = Convert.ToInt32(Request.Params["obsId"]);
                return Json(new { Success = true, Message = qualityBAL.GetStartEndLatLongBAL(obsId) }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// Fetch latlongs for all images against particular Observations
        /// </summary>
        /// <returns></returns>
        public ActionResult GetLatLong()
        {
            qualityBAL = new QualityMonitoringBAL();
            try
            {
                int obsId = Convert.ToInt32(Request.Params["obsId"]);
                return Json(new { Success = true, Message = qualityBAL.GetLatLongBAL(obsId) }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// Post Method for Uploading IMAGE File
        /// </summary>
        /// <param name="fileUploadViewModel"></param>
        /// <returns></returns>
        [HttpPost]
        [AcceptVerbs(HttpVerbs.Post)]
        [Audit]
        public ActionResult Uploads(PMGSY.Models.QualityMonitoring.FileUploadViewModel fileUploadViewModel)
        {
            //Added By Abhishek kamble 26-Apr-2014 to validate File Type
            CommonFunctions objCommonFunc = new CommonFunctions();
            //Array of File Types to Validate             
            String[] fileTypes = new String[] { "jpeg", String.Empty, String.Empty };
            String StorageRoot = string.Empty;

            ///Path to upload files for NQM/SQM/CQC/SQC
            if (PMGSYSession.Current.RoleCode == 5 || PMGSYSession.Current.RoleCode == 6 || PMGSYSession.Current.RoleCode == 9)   //CQCAdmin or NQM or CQC
            {
                StorageRoot = ConfigurationManager.AppSettings["QUALITY_INSPECTION_FILE_UPLOAD_NQM_NEW"];   // changes made by saurabh here
            }
            else if (PMGSYSession.Current.RoleCode == 7 || PMGSYSession.Current.RoleCode == 8 || PMGSYSession.Current.RoleCode == 69 || PMGSYSession.Current.RoleCode == 48)   //SQM & SQC
            {
                StorageRoot = ConfigurationManager.AppSettings["QUALITY_INSPECTION_FILE_UPLOAD_SQM_NEW"];   // changes made by saurabh here
            }
            if (!(objCommonFunc.IsValidImageFile(StorageRoot, Request, fileTypes)))
            {
                fileUploadViewModel.ErrorMessage = "File Type is Not Allowed.";
                return View("ImageUpload", fileUploadViewModel.ErrorMessage);
            }

            qualityBAL = new QualityMonitoringBAL();
            ////Apply Later

            foreach (string file in Request.Files)
            {
                string status = new ProposalBAL().ValidateImageFile(Request.Files[0].ContentLength, Path.GetExtension(Request.Files[0].FileName));
                if (status != string.Empty)
                {
                    fileUploadViewModel.ErrorMessage = status;
                    return View("ImageUpload", fileUploadViewModel.ErrorMessage);
                }
            }

            var fileData = new List<PMGSY.Models.QualityMonitoring.FileUploadViewModel>();

            int IMS_PR_ROAD_CODE = 0;
            int ADMIN_SCHEDULE_CODE = 0;
            int QM_OBSERVATION_ID = 0;
            if (fileUploadViewModel.IMS_PR_ROAD_CODE != 0 && fileUploadViewModel.ADMIN_SCHEDULE_CODE != 0 && fileUploadViewModel.QM_OBSERVATION_ID != 0)
            {
                ADMIN_SCHEDULE_CODE = fileUploadViewModel.ADMIN_SCHEDULE_CODE;
                IMS_PR_ROAD_CODE = fileUploadViewModel.IMS_PR_ROAD_CODE;
                QM_OBSERVATION_ID = fileUploadViewModel.QM_OBSERVATION_ID;
            }
            else
            {
                try
                {
                    ADMIN_SCHEDULE_CODE = Convert.ToInt32(Request["ADMIN_SCHEDULE_CODE"]);
                    IMS_PR_ROAD_CODE = Convert.ToInt32(Request["IMS_PR_ROAD_CODE"]);
                    QM_OBSERVATION_ID = Convert.ToInt32(Request["QM_OBSERVATION_ID"]);
                }
                catch (Exception ex)
                {
                    Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                    if (Request["IMS_PR_ROAD_CODE"].Contains(','))
                    {
                        IMS_PR_ROAD_CODE = Convert.ToInt32(Request["IMS_PR_ROAD_CODE"].Split(',')[0]);
                    }
                    if (Request["ADMIN_SCHEDULE_CODE"].Contains(','))
                    {
                        IMS_PR_ROAD_CODE = Convert.ToInt32(Request["ADMIN_SCHEDULE_CODE"].Split(',')[0]);
                    }
                }
            }

            foreach (string file in Request.Files)
            {
                UploadImageFile(Request, fileData, ADMIN_SCHEDULE_CODE, IMS_PR_ROAD_CODE, QM_OBSERVATION_ID);
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
        /// Save Details of file tobe uploaded 
        /// Compress file & save to destination
        /// </summary>
        /// <param name="request"></param>
        /// <param name="statuses"></param>
        /// <param name="ADMIN_SCHEDULE_CODE"></param>
        /// <param name="IMS_PR_ROAD_CODE"></param>
        [Audit]
        public void UploadImageFile(HttpRequestBase request, List<PMGSY.Models.QualityMonitoring.FileUploadViewModel> statuses, int ADMIN_SCHEDULE_CODE, int IMS_PR_ROAD_CODE, int OBS_ID)
        {
            dbContext = new PMGSYEntities();
            qualityBAL = new QualityMonitoringBAL();
            String StorageRoot = string.Empty;
            int MaxFileID = 0;
            try
            {
                for (int i = 0; i < request.Files.Count; i++)
                {
                    HttpPostedFileBase file = request.Files[i];

                    if (dbContext.QUALITY_QM_INSPECTION_FILE.Count() == 0)
                    {
                        MaxFileID = 0;
                    }
                    else
                    {
                        MaxFileID = (from c in dbContext.QUALITY_QM_INSPECTION_FILE select (Int32?)c.QM_FILE_ID ?? 0).Max();
                    }

                    Int32 countOfFilesUploaded = 0;
                    if (OBS_ID > 0)
                    {
                        countOfFilesUploaded = (from c in dbContext.QUALITY_QM_INSPECTION_FILE
                                                where c.QM_OBSERVATION_ID == OBS_ID
                                                select c).Count();
                    }
                    else
                    {
                        countOfFilesUploaded = (from c in dbContext.QUALITY_QM_INSPECTION_FILE
                                                where c.ADMIN_SCHEDULE_CODE == ADMIN_SCHEDULE_CODE
                                                && c.IMS_PR_ROAD_CODE == IMS_PR_ROAD_CODE
                                                select c).Count();
                    }


                    if (countOfFilesUploaded == 0)
                    {
                        countOfFilesUploaded = 1;
                    }
                    else
                    {
                        countOfFilesUploaded = countOfFilesUploaded + 1;
                    }

                    var fileId = MaxFileID + 1;

                    int intYear = DateTime.Now.Year;


                    //var fileNameWithoutExt = Path.GetFileNameWithoutExtension(request.Files[i].FileName).ToString();
                    var fileName = intYear + "$" + fileId + Path.GetExtension(request.Files[i].FileName).ToString();  // Changes made by saurabh here

                    ///Path to upload files for NQM/SQM/CQC/SQC
                    if (PMGSYSession.Current.RoleCode == 5 || PMGSYSession.Current.RoleCode == 6 || PMGSYSession.Current.RoleCode == 9)   //CQCAdmin or NQM or CQC
                    {
                        StorageRoot = ConfigurationManager.AppSettings["QUALITY_INSPECTION_FILE_UPLOAD_NQM_NEW"];    // Changes made by saurabh here
                    }
                    else if (PMGSYSession.Current.RoleCode == 7 || PMGSYSession.Current.RoleCode == 8 || PMGSYSession.Current.RoleCode == 69 || PMGSYSession.Current.RoleCode == 48)   //SQM & SQC
                    {
                        StorageRoot = ConfigurationManager.AppSettings["QUALITY_INSPECTION_FILE_UPLOAD_SQM_NEW"];    // Changes made by saurabh here
                    }

                    // start of new code by saurabh
                    var YearDictionary = Path.Combine(StorageRoot, intYear.ToString());

                    var ThumbnailPath = Path.Combine(YearDictionary, "thumbnails");
                    var FullThumbnailPath = Path.Combine(ThumbnailPath, fileName);
                    var fullPath = Path.Combine(YearDictionary, fileName);

                    if (!(Directory.Exists(YearDictionary)))
                    {
                        Directory.CreateDirectory(YearDictionary);
                    }
                    if (!(Directory.Exists(ThumbnailPath)))
                    {
                        Directory.CreateDirectory(ThumbnailPath);
                    }

                    //  end of new code by saurabh on date 08-02-2022
                    statuses.Add(new PMGSY.Models.QualityMonitoring.FileUploadViewModel()
                    {
                        url = fullPath,
                        thumbnail_url = fullPath,
                        name = fileName,
                        type = file.ContentType,
                        size = file.ContentLength,
                        delete_url = "",
                        delete_type = "DELETE",
                        Image_Description = request.Params["remark[]"],  //.Split(',')[i]

                        ADMIN_SCHEDULE_CODE = ADMIN_SCHEDULE_CODE,
                        IMS_PR_ROAD_CODE = IMS_PR_ROAD_CODE,
                        QM_OBSERVATION_ID = OBS_ID
                    });

                    string status = qualityBAL.AddFileUploadDetailsBAL(statuses);
                    if (status == string.Empty)
                    {
                        new ProposalBAL().CompressImage(request.Files[0], fullPath, FullThumbnailPath);
                    }
                    else
                    {
                        // show an error over here
                    }
                }
            }
            finally
            {
                dbContext.Dispose();
            }
        }



        /// <summary>
        /// Post to Update Image Description
        /// </summary>
        /// <param name="formCollection"></param>
        /// <returns></returns>
        [HttpPost]
        [Audit]
        public JsonResult UpdateImageDetails(FormCollection formCollection)
        {
            qualityBAL = new QualityMonitoringBAL();
            PMGSY.Models.QualityMonitoring.FileUploadViewModel fileuploadViewModel = new PMGSY.Models.QualityMonitoring.FileUploadViewModel();
            fileuploadViewModel.QM_FILE_ID = Convert.ToInt32(formCollection["id"]);
            try
            {
                Regex regex = new Regex(@"^[a-zA-Z0-9 ,.()-]+$");
                if (formCollection["Description"].Trim() == "")
                {
                    return Json("Invalid Description, Only Alphabets, Numbers and  [,.()-] value are allowed");
                }
                else if (regex.IsMatch(formCollection["Description"]))
                {
                    fileuploadViewModel.Image_Description = formCollection["Description"];
                }
                else
                {
                    return Json("Invalid Description, Only Alphabets, Numbers and  [,.()-] value are allowed");
                }

                #region Latitude and Longitude
                //Regex regexNumberDecimal = new Regex(@"^\s*(?=.*[0-9])\d*(?:\.\d{1,12})?\s*$");
                //if (formCollection["Latitude"].Trim() == "")
                //{
                //    return Json("Invalid Latitude, Can only contains Numeric values and 12 digit after decimal place.");
                //}
                //else if (regex.IsMatch(formCollection["Latitude"]))
                //{
                //    fileuploadViewModel.Latitude = formCollection["Latitude"];
                //}
                //else
                //{
                //    return Json("Invalid Latitude, Can only contains Numeric values and 12 digit after decimal place.");
                //}

                //if (formCollection["Longitude"].Trim() == "")
                //{
                //    return Json("Invalid Longitude, Can only contains Numeric values and 12 digit after decimal place.");
                //}
                //else if (regex.IsMatch(formCollection["Longitude"]))
                //{
                //    fileuploadViewModel.Longitude = formCollection["Longitude"];
                //}
                //else
                //{
                //    return Json("Invalid Longitude, Can only contains Numeric values and 12 digit after decimal place.");
                //}
                #endregion
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return Json("Please Enter Valid Description.");
            }

            string status = qualityBAL.UpdateImageDetailsBAL(fileuploadViewModel);

            if (status == string.Empty)
                return Json(true);
            else
                return Json("Error Occured While Processing Your Request.");
        }


        /// <summary>
        /// DownLoad Image OR Pdf File (ATR)
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="hash"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        [HttpGet]
        [Audit]
        public ActionResult DownloadFile(String parameter, String hash, String key)
        {
            string FileName = string.Empty;
            string FullFileLogicalPathName = string.Empty;
            string FullfilePhysicalPathName = string.Empty;
            string FullFileLogicalPathId = string.Empty;
            string FullfilePhysicalPathId = string.Empty;
            string FileExtension = string.Empty;
            string PhysicalPath = string.Empty;
            string VirtualPath = string.Empty;
            // changes for above two lines by saurabh...
            Int32 ObsOrAtrId = 0;
            dbContext = new PMGSYEntities();
            try
            {
                string DownloadFilePath = "";
                if (!String.IsNullOrEmpty(parameter) && !String.IsNullOrEmpty(hash) && !String.IsNullOrEmpty(key))
                {
                    String[] urlParams = URLEncrypt.DecryptParameters(new String[] { parameter, hash, key });
                    if (urlParams.Length >= 1)
                    {
                        String[] urlSplitParams = urlParams[0].Split('$');
                        // Changes done by saurabh
                        if (urlSplitParams.Length == 2)
                        {
                            FileName = (urlSplitParams[0]);
                            ObsOrAtrId = Convert.ToInt32(urlSplitParams[1]);
                        }
                        else
                        {
                            FileName = (urlSplitParams[0]) + '$' + (urlSplitParams[1]);
                            ObsOrAtrId = Convert.ToInt32(urlSplitParams[2]);
                        }
                        // changes ended by saurabh
                    }
                }
                FileExtension = Path.GetExtension(FileName).ToLower();

                if (FileExtension == ".pdf")
                {
                    //In case of if File With Name not Found then find with Id, This is case particularly for ATR
                    FullFileLogicalPathId = Path.Combine(ConfigurationManager.AppSettings["QUALITY_INSPECTION_FILE_UPLOAD_ATR_VIRTUAL_DIR_PATH"], ObsOrAtrId.ToString() + ".pdf");
                    FullfilePhysicalPathId = Path.Combine(ConfigurationManager.AppSettings["QUALITY_INSPECTION_FILE_UPLOAD_ATR"], ObsOrAtrId.ToString() + ".pdf");

                    FullFileLogicalPathName = Path.Combine(ConfigurationManager.AppSettings["QUALITY_INSPECTION_FILE_UPLOAD_ATR_VIRTUAL_DIR_PATH"], FileName);
                    FullfilePhysicalPathName = Path.Combine(ConfigurationManager.AppSettings["QUALITY_INSPECTION_FILE_UPLOAD_ATR"], FileName);

                    #region
                    using (System.IO.StreamWriter sw = System.IO.File.AppendText(System.IO.Path.Combine("D:\\ErrorLog\\OMMASErrorLog_" + System.DateTime.Now.Date.ToString("dd_MM_yyyy") + ".txt", "")))
                    {
                        sw.WriteLine("---------------------------------------------------------------------------------------");
                        sw.WriteLine("webconfigpath : " + ConfigurationManager.AppSettings["QUALITY_INSPECTION_FILE_UPLOAD_ATR"]);
                        sw.WriteLine("QMDownloadPath : " + FullfilePhysicalPathName);
                        sw.WriteLine("QMvirtualpath : " + FullFileLogicalPathId);
                        sw.WriteLine("logicalpathname : " + FullFileLogicalPathName);
                        sw.WriteLine("physicalpathname : " + FullfilePhysicalPathName);
                        sw.WriteLine("---------------------------------------------------------------------------------------");
                        sw.Close();
                    }
                    #endregion
                }
                else if (FileExtension == ".jpg" || FileExtension == ".jpeg" || FileExtension == ".bmp" || FileExtension == ".png" || FileExtension == ".gif" || FileExtension == ".tiff")
                {
                    string QM_FILE_NAME = Request.Params["QM_FILE_NAME"];
                    var qmTypeForUser = (from aqm in dbContext.ADMIN_QUALITY_MONITORS
                                         join qqs in dbContext.QUALITY_QM_SCHEDULE on aqm.ADMIN_QM_CODE equals qqs.ADMIN_QM_CODE
                                         join qqom in dbContext.QUALITY_QM_OBSERVATION_MASTER on qqs.ADMIN_SCHEDULE_CODE equals qqom.ADMIN_SCHEDULE_CODE
                                         where qqom.QM_OBSERVATION_ID == ObsOrAtrId
                                         select aqm).First();
                    // Changes Done by saurabh Starts Here....
                    if (FileName.ToString().Contains("$") == true)
                    {
                        DownloadFilePath = Convert.ToString(FileName.ToString().Split('$')[0]);  // CHANGES BY SAURABH TO SPLIT FILE NAME

                        ///Path to upload files for NQM/SQM/CQC/SQC
                        if (qmTypeForUser.ADMIN_QM_TYPE.Equals("I"))   //CQC & NQM
                        {
                            VirtualPath = Path.Combine(ConfigurationManager.AppSettings["QUALITY_INSPECTION_FILE_UPLOAD_NQM_VIRTUAL_DIR_PATH_NEW"], DownloadFilePath);
                            PhysicalPath = Path.Combine(ConfigurationManager.AppSettings["QUALITY_INSPECTION_FILE_UPLOAD_NQM_NEW"], DownloadFilePath);

                            FullfilePhysicalPathName = Path.Combine(PhysicalPath, FileName);
                            FullFileLogicalPathName = Path.Combine(VirtualPath, FileName);


                        }
                        else if (qmTypeForUser.ADMIN_QM_TYPE.Equals("S"))   //SQM & SQC
                        {
                            VirtualPath = Path.Combine(ConfigurationManager.AppSettings["QUALITY_INSPECTION_FILE_UPLOAD_SQM_VIRTUAL_DIR_PATH_NEW"], DownloadFilePath);
                            PhysicalPath = Path.Combine(ConfigurationManager.AppSettings["QUALITY_INSPECTION_FILE_UPLOAD_SQM_NEW"], DownloadFilePath);

                            FullfilePhysicalPathName = Path.Combine(PhysicalPath, FileName);
                            FullFileLogicalPathName = Path.Combine(VirtualPath, FileName);


                        }
                    }
                    else
                    {


                        ///Path to upload files for NQM/SQM/CQC/SQC
                        if (qmTypeForUser.ADMIN_QM_TYPE.Equals("I"))   //CQC & NQM
                        {
                            FullFileLogicalPathName = Path.Combine(ConfigurationManager.AppSettings["QUALITY_INSPECTION_FILE_UPLOAD_NQM_VIRTUAL_DIR_PATH"], FileName);
                            FullfilePhysicalPathName = Path.Combine(ConfigurationManager.AppSettings["QUALITY_INSPECTION_FILE_UPLOAD_NQM"], FileName);
                        }
                        else if (qmTypeForUser.ADMIN_QM_TYPE.Equals("S"))   //SQM & SQC
                        {
                            FullFileLogicalPathName = Path.Combine(ConfigurationManager.AppSettings["QUALITY_INSPECTION_FILE_UPLOAD_SQM_VIRTUAL_DIR_PATH"], FileName);
                            FullfilePhysicalPathName = Path.Combine(ConfigurationManager.AppSettings["QUALITY_INSPECTION_FILE_UPLOAD_SQM"], FileName);
                        }
                    }
                    // Changes done by saurabh ends here....
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
                        case ".doc":
                        case ".docx":
                            type = "Application/msword";
                            break;
                        case ".jpg":
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

                if (System.IO.File.Exists(FullfilePhysicalPathName))
                {
                    return File(FullfilePhysicalPathName, type, DateTime.Now.ToShortDateString() + "_" + new Random().Next(1000000000) + FileExtension);
                }
                else if (System.IO.File.Exists(FullfilePhysicalPathId))
                {
                    return File(FullfilePhysicalPathId, type, DateTime.Now.ToShortDateString() + "_" + new Random().Next(1000000000) + FileExtension);
                }
                else
                {
                    return Json(new { Error = "File Not Exist" }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                //Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                ErrorLog.LogError(ex, "QualityMonitoring.DownloadFile()");
                return Json(new { Success = "false" }, JsonRequestBehavior.AllowGet);
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        /// <summary>
        /// DownLoad Image OR Pdf File Lab
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="hash"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        [HttpGet]
        [Audit]
        public ActionResult DownloadLabFile(String parameter, String hash, String key)
        {
            string FileName = string.Empty;
            string FullFileLogicalPathName = string.Empty;
            string FullfilePhysicalPathName = string.Empty;
            string FullFileLogicalPathId = string.Empty;
            string FullfilePhysicalPathId = string.Empty;
            string FileExtension = string.Empty;
            Int32 ObsOrAtrId = 0;
            dbContext = new PMGSYEntities();
            try
            {
                if (!String.IsNullOrEmpty(parameter) && !String.IsNullOrEmpty(hash) && !String.IsNullOrEmpty(key))
                {
                    String[] urlParams = URLEncrypt.DecryptParameters(new String[] { parameter, hash, key });
                    if (urlParams.Length >= 1)
                    {
                        String[] urlSplitParams = urlParams[0].Split('$');
                        FileName = (urlSplitParams[0]);
                        ObsOrAtrId = Convert.ToInt32(urlSplitParams[1]);
                    }
                }
                FileExtension = Path.GetExtension(FileName).ToLower();

                if (FileExtension == ".jpg" || FileExtension == ".jpeg" || FileExtension == ".bmp" || FileExtension == ".png" || FileExtension == ".gif" || FileExtension == ".tiff")
                {

                    ///Path to upload files for NQM/SQM/CQC/SQC

                    FullFileLogicalPathName = Path.Combine(ConfigurationManager.AppSettings["QUALITY_LAB_FILE_UPLOAD_VIRTUAL_DIR_PATH"], FileName);
                    FullfilePhysicalPathName = Path.Combine(ConfigurationManager.AppSettings["QUALITY_LAB_FILE_UPLOAD"], FileName);

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
                        case ".doc":
                        case ".docx":
                            type = "Application/msword";
                            break;
                        case ".jpg":
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

                if (System.IO.File.Exists(FullfilePhysicalPathName))
                {
                    return File(FullfilePhysicalPathName, type, DateTime.Now.ToShortDateString() + "_" + new Random().Next(1000000000) + FileExtension);
                }
                else if (System.IO.File.Exists(FullfilePhysicalPathId))
                {
                    return File(FullfilePhysicalPathId, type, DateTime.Now.ToShortDateString() + "_" + new Random().Next(1000000000) + FileExtension);
                }
                else
                {
                    return Json(new { Success = "false", Error = "File Not Exist" }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return Json(new { Success = "false" }, JsonRequestBehavior.AllowGet);
            }
            finally
            {
                dbContext.Dispose();
            }
        }


        /// <summary>
        /// Delete database details of image & actual image from physical location
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Audit]
        public JsonResult DeleteFileDetails()
        {
            String PhysicalPath = string.Empty;
            String ThumbnailPath = string.Empty;
            dbContext = new PMGSYEntities();
            qualityBAL = new QualityMonitoringBAL();

            try
            {

                string[] arrParam = Request.Params["QM_OBSERVATION_ID"].Split('$');

                int QM_FILE_ID = Convert.ToInt32(arrParam[0]);
                int QM_OBSERVATION_ID = Convert.ToInt32(arrParam[1]);


                string QM_FILE_NAME = Request.Params["QM_FILE_NAME"];
                var qmTypeForUser = (from aqm in dbContext.ADMIN_QUALITY_MONITORS
                                     join qqs in dbContext.QUALITY_QM_SCHEDULE on aqm.ADMIN_QM_CODE equals qqs.ADMIN_QM_CODE
                                     join qqom in dbContext.QUALITY_QM_OBSERVATION_MASTER on qqs.ADMIN_SCHEDULE_CODE equals qqom.ADMIN_SCHEDULE_CODE
                                     where qqom.QM_OBSERVATION_ID == QM_OBSERVATION_ID
                                     select aqm).First();

                ///Path to upload files for NQM/SQM/CQC/SQC
                if (qmTypeForUser.ADMIN_QM_TYPE.Equals("I"))   //CQC & NQM
                {
                    PhysicalPath = ConfigurationManager.AppSettings["QUALITY_INSPECTION_FILE_UPLOAD_NQM"];
                }
                else if (qmTypeForUser.ADMIN_QM_TYPE.Equals("S"))   //SQM & SQC
                {
                    PhysicalPath = ConfigurationManager.AppSettings["QUALITY_INSPECTION_FILE_UPLOAD_SQM"];
                }
                ThumbnailPath = Path.Combine(Path.Combine(PhysicalPath, "thumbnails"), QM_FILE_NAME);

                PhysicalPath = Path.Combine(PhysicalPath, QM_FILE_NAME);

                if (!System.IO.File.Exists(PhysicalPath) && !System.IO.File.Exists(ThumbnailPath))
                {
                    return Json(new { Success = false, ErrorMessage = "File Not Found." });
                }

                string status = qualityBAL.DeleteFileDetails(QM_FILE_ID);

                if (status == string.Empty)
                {
                    try
                    {
                        System.IO.File.Delete(PhysicalPath);
                        System.IO.File.Delete(ThumbnailPath);
                    }
                    catch (Exception ex)
                    {
                        Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);

                        return Json(new { Success = true, ErrorMessage = ex.Message });
                    }
                    return Json(new { Success = true, ErrorMessage = "Error Ocurred While Processing Your Request." });
                }
                else
                {
                    return Json(new { Success = false, ErrorMessage = status });
                }
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);

                return Json(new { Success = true, ErrorMessage = "Error Ocurred While Processing Your Request." });
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        /// <summary>
        /// This Compresses Image and Creates the Thumbnail
        /// </summary>
        /// <param name="httpPostedFileBase"></param>
        /// <param name="DestinitionPath"></param>
        /// <param name="ThumbnailPath"></param>
        public void CompressImage(HttpPostedFileBase httpPostedFileBase, string DestinitionPath, string ThumbnailPath)
        {
            // For Thumbnail Image            
            ImageResizer.ImageJob ThumbnailJob = new ImageResizer.ImageJob(httpPostedFileBase, ThumbnailPath,
                new ImageResizer.ResizeSettings("width=100;height=75;format=jpg;mode=max"));

            ThumbnailJob.Build();

            HttpPostedFileBase ForResizeConditions = httpPostedFileBase;

            Image image = Image.FromStream(ForResizeConditions.InputStream);
            if (image.Height < 768 || image.Width < 1024)
            {
                httpPostedFileBase.InputStream.Seek(0, SeekOrigin.Begin);
                // For Original Image
                ImageResizer.ImageJob job = new ImageResizer.ImageJob(httpPostedFileBase, DestinitionPath,
                    new ImageResizer.ResizeSettings("width=" + image.Width + ";height=" + image.Height + ";format=jpg;mode=min"));

                job.Build();
            }
            else
            {
                httpPostedFileBase.InputStream.Seek(0, SeekOrigin.Begin);
                // For Original Image
                ImageResizer.ImageJob job = new ImageResizer.ImageJob(httpPostedFileBase, DestinitionPath,
                    new ImageResizer.ResizeSettings("width=1024;height=768;format=jpg;mode=max"));

                job.Build();
            }
        }


        /// <summary>
        /// Validates the Image File
        /// </summary>
        /// <param name="FileSize"></param>
        /// <param name="FileExtension"></param>
        /// <returns></returns>
        public string ValidateImageFile(int FileSize, string FileExtension)
        {
            string ValidExtensions = ConfigurationManager.AppSettings["PROPOSAL_IMAGE_VALID_FORMAT"];
            string[] arrValidFormats = ValidExtensions.Split('$');


            if (!arrValidFormats.Contains(FileExtension.ToLower()))
            {
                return "File is not Valid Image File";
            }
            if (FileSize > Convert.ToInt32(ConfigurationManager.AppSettings["PROPOSAL_IMAGE_FILE_MAX_SIZE"]))
            {
                return "File Size Exceed the Maximum File Limit";
            }

            return string.Empty;
        }
        #endregion


        #region PdfUpload by Monitor

        /// <summary>
        /// Get the PDF Files List
        /// </summary>
        /// <param name="formCollection"></param>
        /// <returns></returns>
        [Audit]
        public JsonResult ListInspPDFFiles(FormCollection formCollection)
        {
            qualityBAL = new QualityMonitoringBAL();
            //Adde By Abhishek kamble 30-Apr-2014 start
            using (CommonFunctions commonFunction = new CommonFunctions())
            {
                if (!commonFunction.ValidateGridParameters(new GridParams(Convert.ToInt32(formCollection["page"]), Convert.ToInt32(formCollection["rows"]), formCollection["sidx"], formCollection["sord"], Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                {
                    return null;
                }
            }
            //Adde By Abhishek kamble 30-Apr-2014 end
            int obsId = Convert.ToInt32(Request["QM_OBSERVATION_ID"]);
            string isAtrPage = "N";

            isAtrPage = (Request["isATRPage"]);


            int totalRecords;
            var jsonData = new
            {
                rows = qualityBAL.GetInspReportFilesListDAL(Convert.ToInt32(formCollection["page"]) - 1, Convert.ToInt32(formCollection["rows"]), formCollection["sidx"], formCollection["sord"], out totalRecords, obsId, isAtrPage),
                total = totalRecords <= Convert.ToInt32(formCollection["rows"]) ? 1 : totalRecords / Convert.ToInt32(formCollection["rows"]) + 1,
                page = Convert.ToInt32(formCollection["page"]),
                records = totalRecords
            };
            return Json(jsonData);
        }


        /// <summary>
        /// Get the PDF File Upload View
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Audit]
        public ActionResult InspPdfFileUpload(string id)
        {
            dbContext = new PMGSYEntities();
            try
            {

                PMGSY.Models.QualityMonitoring.FileUploadViewModel fileUploadViewModel = new PMGSY.Models.QualityMonitoring.FileUploadViewModel();
                if (id.Contains("$ATR"))
                {
                    fileUploadViewModel.isATRPage = "Y";
                    id = id.Split('$')[0];
                }
                else
                {
                    fileUploadViewModel.isATRPage = "N";
                }
                fileUploadViewModel.QM_OBSERVATION_ID = Convert.ToInt32(id);
                fileUploadViewModel.ErrorMessage = string.Empty;
                if (dbContext.QUALITY_INSPECTION_REPORT_FILE.Where(a => a.QM_OBSERVATION_ID == fileUploadViewModel.QM_OBSERVATION_ID && a.FILE_TYPE.ToUpper() == "I").Any())
                {
                    fileUploadViewModel.NumberofPdfs = dbContext.QUALITY_INSPECTION_REPORT_FILE.Where(a => a.QM_OBSERVATION_ID == fileUploadViewModel.QM_OBSERVATION_ID && a.FILE_TYPE.ToUpper() == "I").Count();
                }
                else
                {
                    fileUploadViewModel.NumberofPdfs = 0;
                }

                fileUploadViewModel.CurDate = System.DateTime.Now.Date;
                //fileUploadViewModel.IMS_PR_ROAD_CODE = dbContext.QUALITY_QM_OBSERVATION_MASTER.Where(a => a.QM_OBSERVATION_ID == fileUploadViewModel.QM_OBSERVATION_ID).Select(m => m.IMS_PR_ROAD_CODE).FirstOrDefault();
                //Edited by Shreyas on 06-09-2022
                var result1 = (from qom in dbContext.QUALITY_QM_OBSERVATION_MASTER
                               where qom.QM_OBSERVATION_ID == fileUploadViewModel.QM_OBSERVATION_ID
                               select new
                               {
                                   qom.IMS_PR_ROAD_CODE,
                                   qom.QM_ROAD_STATUS
                               }).FirstOrDefault();


                var result2 = (from isp in dbContext.IMS_SANCTIONED_PROJECTS
                               where isp.IMS_PR_ROAD_CODE == result1.IMS_PR_ROAD_CODE
                               select new
                               {
                                   isp.IMS_PROPOSAL_TYPE,

                               }).FirstOrDefault();


                // var result= dbContext.IMS_SANCTIONED_PROJECTS.Where(a=>a.IMS_PR_ROAD_CODE == fileUploadViewModel.IMS_PR_ROAD_CODE).Select(m=> m.IMS_PROPOSAL_TYPE,m.IMS_ISCOMPLETED).FirstOrDefault();
                fileUploadViewModel.IMS_PR_ROAD_CODE = result1.IMS_PR_ROAD_CODE;
                fileUploadViewModel.workType = result2.IMS_PROPOSAL_TYPE;
                fileUploadViewModel.workStatus = result1.QM_ROAD_STATUS;

                return View(fileUploadViewModel);
            }
            catch (Exception)
            {
                return null;
            }
        }


        /// <summary>
        /// Post Method for Uploading PDF File
        /// </summary>
        /// <param name="fileUploadViewModel"></param>
        /// <returns></returns>
        [HttpPost]
        [Audit]
        public ActionResult InspPdfFileUpload(PMGSY.Models.QualityMonitoring.FileUploadViewModel fileUploadViewModel)
        {
            try
            {
                qualityBAL = new QualityMonitoringBAL();
                dbContext = new PMGSYEntities();
                CommonFunctions objCommonFunc = new CommonFunctions();

                foreach (string file in Request.Files)
                {
                    string status = qualityBAL.ValidatePDFFile(Request.Files[0].ContentLength, Path.GetExtension(Request.Files[0].FileName));
                    if (status != string.Empty)
                    {
                        fileUploadViewModel.ErrorMessage = status;
                        return View("InspPdfFileUpload", fileUploadViewModel.ErrorMessage);
                    }
                }

                var fileData = new List<PMGSY.Models.QualityMonitoring.FileUploadViewModel>();

                int obsId = 0;
                if (fileUploadViewModel.QM_OBSERVATION_ID != 0)
                {
                    obsId = fileUploadViewModel.QM_OBSERVATION_ID;
                }
                else
                {
                    try
                    {
                        obsId = Convert.ToInt32(Request["QM_OBSERVATION_ID"]);
                    }
                    catch
                    {
                        if (Request["QM_OBSERVATION_ID"].Contains(','))
                        {
                            obsId = Convert.ToInt32(Request["QM_OBSERVATION_ID"].Split(',')[0]);
                        }
                    }
                }

                var qmTypeForUser = (from aqm in dbContext.ADMIN_QUALITY_MONITORS
                                     join qqs in dbContext.QUALITY_QM_SCHEDULE on aqm.ADMIN_QM_CODE equals qqs.ADMIN_QM_CODE
                                     join qqom in dbContext.QUALITY_QM_OBSERVATION_MASTER on qqs.ADMIN_SCHEDULE_CODE equals qqom.ADMIN_SCHEDULE_CODE
                                     where qqom.QM_OBSERVATION_ID == obsId
                                     select aqm).First();

                //string VirtualDirectoryUrl = string.Empty;
                string PhysicalPath = string.Empty;
                if (qmTypeForUser.ADMIN_QM_TYPE.Equals("I"))
                {
                    //VirtualDirectoryUrl = ConfigurationManager.AppSettings["QUALITY_INSPECTION_REPORT_FILE_NQM_VIRTUAL_PATH"];
                    PhysicalPath = ConfigurationManager.AppSettings["QUALITY_INSPECTION_REPORT_FILE_NQM"];
                }
                else if (qmTypeForUser.ADMIN_QM_TYPE.Equals("S"))
                {
                    //VirtualDirectoryUrl = ConfigurationManager.AppSettings["QUALITY_INSPECTION_REPORT_FILE_SQM_VIRTUAL_PATH"];
                    PhysicalPath = ConfigurationManager.AppSettings["QUALITY_INSPECTION_REPORT_FILE_SQM"];
                }

                if (!(objCommonFunc.ValidateIsPdf(PhysicalPath, Request)))
                {
                    fileUploadViewModel.ErrorMessage = "File Type is Not Allowed.";
                    return View("InspPdfFileUpload", fileUploadViewModel.ErrorMessage);
                }

                foreach (string file in Request.Files)
                {
                    UploadInspPDFFile(Request, fileData, obsId);
                }

                if (dbContext.QUALITY_INSPECTION_REPORT_FILE.Where(a => a.QM_OBSERVATION_ID == obsId && a.FILE_TYPE.ToUpper() == "I").Any())
                {
                    fileUploadViewModel.NumberofPdfs = dbContext.QUALITY_INSPECTION_REPORT_FILE.Where(a => a.QM_OBSERVATION_ID == obsId && a.FILE_TYPE.ToUpper() == "I").Count();
                }
                else
                {
                    fileUploadViewModel.NumberofPdfs = 0;
                }

                var serializer = new JavaScriptSerializer();
                serializer.MaxJsonLength = Int32.MaxValue;

                var result = new ContentResult
                {
                    Content = "{\"files\":" + serializer.Serialize(fileData) + "}",
                };
                return result;
            }
            catch (Exception)
            {
                fileUploadViewModel.ErrorMessage = "An Error Occurred While Processing Your Request.";
                return View("InspPdfFileUpload", fileUploadViewModel.ErrorMessage);
            }
            finally
            {
                dbContext.Dispose();
            }
        }


        /// <summary>
        /// This Method Uploads PDF File
        /// </summary>
        /// <param name="request"></param>
        /// <param name="statuses"></param>
        /// <param name="IMS_PR_ROAD_CODE"></param>
        [Audit]
        public void UploadInspPDFFile(HttpRequestBase request, List<PMGSY.Models.QualityMonitoring.FileUploadViewModel> statuses, int obsId)
        {
            dbContext = new PMGSYEntities();
            qualityBAL = new QualityMonitoringBAL();
            int MaxID = 0;
            try
            {
                for (int i = 0; i < request.Files.Count; i++)
                {
                    HttpPostedFileBase file = request.Files[i];
                    if (dbContext.QUALITY_INSPECTION_REPORT_FILE.Count() == 0)
                    {
                        MaxID = 1;
                    }
                    else
                    {
                        MaxID = (from c in dbContext.QUALITY_INSPECTION_REPORT_FILE select c.FILE_ID).Max();
                    }

                    var fileName = obsId + "_" + MaxID + Path.GetExtension(request.Files[i].FileName).ToString();

                    var qmTypeForUser = (from aqm in dbContext.ADMIN_QUALITY_MONITORS
                                         join qqs in dbContext.QUALITY_QM_SCHEDULE on aqm.ADMIN_QM_CODE equals qqs.ADMIN_QM_CODE
                                         join qqom in dbContext.QUALITY_QM_OBSERVATION_MASTER on qqs.ADMIN_SCHEDULE_CODE equals qqom.ADMIN_SCHEDULE_CODE
                                         where qqom.QM_OBSERVATION_ID == obsId
                                         select aqm).First();

                    //string VirtualDirectoryUrl = string.Empty;
                    string PhysicalPath = string.Empty;
                    if (qmTypeForUser.ADMIN_QM_TYPE.Equals("I"))
                    {
                        //VirtualDirectoryUrl = ConfigurationManager.AppSettings["QUALITY_INSPECTION_REPORT_FILE_NQM_VIRTUAL_PATH"];
                        PhysicalPath = ConfigurationManager.AppSettings["QUALITY_INSPECTION_REPORT_FILE_NQM"];
                    }
                    else if (qmTypeForUser.ADMIN_QM_TYPE.Equals("S"))
                    {
                        //VirtualDirectoryUrl = ConfigurationManager.AppSettings["QUALITY_INSPECTION_REPORT_FILE_SQM_VIRTUAL_PATH"];
                        PhysicalPath = ConfigurationManager.AppSettings["QUALITY_INSPECTION_REPORT_FILE_SQM"];
                    }

                    var fullPath = Path.Combine(PhysicalPath, fileName);

                    statuses.Add(new PMGSY.Models.QualityMonitoring.FileUploadViewModel()
                    {
                        url = fullPath,
                        thumbnail_url = fullPath,
                        name = fileName,
                        type = file.ContentType,
                        size = file.ContentLength,
                        delete_url = "",
                        delete_type = "DELETE",

                        PdfDescription = request.Params["InspPdfDescription[]"],

                        QM_OBSERVATION_ID = obsId
                    });

                    string status = qualityBAL.AddPdfUploadDetailsBAL(statuses);
                    if (status == string.Empty)
                    {
                        //file.SaveAs(fullPath);
                        file.SaveAs(Path.Combine(PhysicalPath, fileName));
                    }
                    else
                    {
                        // show an error over here
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Downloads the File
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="hash"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        [HttpGet]
        [Audit]
        public ActionResult DownloadInspFile(String parameter, String hash, String key)
        {
            try
            {
                dbContext = new PMGSYEntities();
                string FileName = string.Empty;
                int obsId = 0;
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
                        obsId = Convert.ToInt32(urlSplitParams[1]);
                    }
                }
                FileExtension = Path.GetExtension(FileName).ToLower();

                var qmTypeForUser = (from aqm in dbContext.ADMIN_QUALITY_MONITORS
                                     join qqs in dbContext.QUALITY_QM_SCHEDULE on aqm.ADMIN_QM_CODE equals qqs.ADMIN_QM_CODE
                                     join qqom in dbContext.QUALITY_QM_OBSERVATION_MASTER on qqs.ADMIN_SCHEDULE_CODE equals qqom.ADMIN_SCHEDULE_CODE
                                     where qqom.QM_OBSERVATION_ID == obsId
                                     select aqm).First();

                string VirtualDirectoryUrl = string.Empty;
                string PhysicalPath = string.Empty;
                if (qmTypeForUser.ADMIN_QM_TYPE.Equals("I"))
                {
                    VirtualDirectoryUrl = ConfigurationManager.AppSettings["QUALITY_INSPECTION_REPORT_FILE_NQM_VIRTUAL_PATH"];
                    PhysicalPath = ConfigurationManager.AppSettings["QUALITY_INSPECTION_REPORT_FILE_NQM"];
                }
                else if (qmTypeForUser.ADMIN_QM_TYPE.Equals("S"))
                {
                    VirtualDirectoryUrl = ConfigurationManager.AppSettings["QUALITY_INSPECTION_REPORT_FILE_SQM_VIRTUAL_PATH"];
                    PhysicalPath = ConfigurationManager.AppSettings["QUALITY_INSPECTION_REPORT_FILE_SQM"];
                }

                FullFileLogicalPath = Path.Combine(VirtualDirectoryUrl, FileName);
                FullfilePhysicalPath = Path.Combine(PhysicalPath, FileName);


                string name = Path.GetFileName(FileName);
                string ext = Path.GetExtension(FileName);

                if (System.IO.File.Exists(FullfilePhysicalPath))
                {
                    return File(FullfilePhysicalPath, "Application/pdf", DateTime.Now.ToShortDateString() + "_" + new Random().Next(1000000000) + FileExtension);
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


        /// <summary>
        /// Update the PDF File Details
        /// </summary>
        /// <param name="formCollection"></param>
        /// <returns></returns>
        [HttpPost]
        [Audit]
        public JsonResult UpdateInspPDFDetails(FormCollection formCollection)
        {
            qualityBAL = new QualityMonitoringBAL();
            try
            {
                string[] arrKey = formCollection["id"].Split('$');
                PMGSY.Models.QualityMonitoring.FileUploadViewModel fileuploadViewModel = new PMGSY.Models.QualityMonitoring.FileUploadViewModel();
                fileuploadViewModel.QM_OBSERVATION_ID = Convert.ToInt32(arrKey[1]);
                fileuploadViewModel.QM_FILE_ID = Convert.ToInt32(arrKey[0]);

                Regex regex = new Regex(@"^[a-zA-Z0-9 ,.()-]+$");
                if (regex.IsMatch(formCollection["Description"]) && formCollection["Description"].Trim().Length != 0)
                {
                    fileuploadViewModel.PdfDescription = formCollection["Description"];
                }
                else
                {
                    return Json("Invalid PDF Description, Only Alphabets,Numbers and ,.()- are allowed");
                }

                string status = qualityBAL.UpdateInspPDFDetailsBAL(fileuploadViewModel);

                if (status == string.Empty)
                    return Json(true);
                else
                    return Json("There is an error occurred while processing your request.");
            }
            catch (Exception)
            {
                return Json("There is an error occurred while processing your request.");
            }
        }


        /// <summary>
        /// Delete File and File Details
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        [Audit]
        public JsonResult DeleteInspFileDetails(string id)
        {
            dbContext = new PMGSYEntities();
            qualityBAL = new QualityMonitoringBAL();
            try
            {
                string FILE_NAME = Request.Params["FILE_NAME"];

                //if (FILE_NAME.Contains('_'))
                //{
                //    FILE_NAME = FILE_NAME.Replace('_', ' ');
                //}

                string[] arrParam = Request.Params["QM_OBSERVATION_ID"].Split('$');

                int FILE_ID = Convert.ToInt32(arrParam[0]);
                int QM_OBSERVATION_ID = Convert.ToInt32(arrParam[1]);

                var qmTypeForUser = (from aqm in dbContext.ADMIN_QUALITY_MONITORS
                                     join qqs in dbContext.QUALITY_QM_SCHEDULE on aqm.ADMIN_QM_CODE equals qqs.ADMIN_QM_CODE
                                     join qqom in dbContext.QUALITY_QM_OBSERVATION_MASTER on qqs.ADMIN_SCHEDULE_CODE equals qqom.ADMIN_SCHEDULE_CODE
                                     where qqom.QM_OBSERVATION_ID == QM_OBSERVATION_ID
                                     select aqm).First();

                //string VirtualDirectoryUrl = string.Empty;
                string PhysicalPath = string.Empty;
                if (qmTypeForUser.ADMIN_QM_TYPE.Equals("I"))
                {
                    //VirtualDirectoryUrl = ConfigurationManager.AppSettings["QUALITY_INSPECTION_REPORT_FILE_NQM_VIRTUAL_PATH"];
                    PhysicalPath = ConfigurationManager.AppSettings["QUALITY_INSPECTION_REPORT_FILE_NQM"];
                }
                else if (qmTypeForUser.ADMIN_QM_TYPE.Equals("S"))
                {
                    //VirtualDirectoryUrl = ConfigurationManager.AppSettings["QUALITY_INSPECTION_REPORT_FILE_SQM_VIRTUAL_PATH"];
                    PhysicalPath = ConfigurationManager.AppSettings["QUALITY_INSPECTION_REPORT_FILE_SQM"];
                }

                PhysicalPath = Path.Combine(PhysicalPath, FILE_NAME);
                string status = qualityBAL.DeleteInspFileDetailsBAL(FILE_ID, QM_OBSERVATION_ID, "I");

                if (status == string.Empty)
                {
                    try
                    {
                        System.IO.File.Delete(PhysicalPath);
                    }
                    catch (Exception ex)
                    {
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

        #endregion


        #region ATR


        /// <summary>
        /// View for ATR Details in  HTML Report
        /// </summary>//changed by deendayal on 05-02-2018
        /// //change by Sachin Solanki on 20-08-2020
        /// <param name="formCollection"></param>
        /// <returns></returns>
        /// 

        public ActionResult ATRDetails(FormCollection formCollection)
        {
            QualityMonitoringBAL qmBAL = new QualityMonitoringBAL();
            QMATRDetailsModel atrDetailsModel = new QMATRDetailsModel();
            atrDetailsModel.ATR_LIST = new List<QMATRModel>();
            atrDetailsModel.OBS_LIST = new List<QMObsATRModel>();
            dbContext = new PMGSYEntities();

            int PmgsyScheme = Convert.ToInt32(formCollection["schemeType"]);//ATR_Change
            if (PmgsyScheme < 0 || PmgsyScheme > 4)
            {
                ModelState.AddModelError("SchemeError", "Scheme is not valid");
                return View(atrDetailsModel); //return model as null
            }//ATR_Change

            try
            {


                //using (System.IO.StreamWriter sw = System.IO.File.AppendText(System.IO.Path.Combine("D:\\ErrorLog\\1QMErrorLog_" + System.DateTime.Now.Date.ToString("dd_MM_yyyy") + ".txt", "")))
                //{
                //    sw.WriteLine("---------------------------------------------------------------------------------------");
                //    sw.WriteLine("AT setp 1 ");
                //    sw.WriteLine("---------------------------------------------------------------------------------------");
                //    sw.Close();
                //}

                //changed by rahul on 11/1/22
                atrDetailsModel.OBSS_ATR_LIST = qmBAL.ATRDetailssBAL(Convert.ToInt32(formCollection["MAST_STATE_CODE"]), Convert.ToInt32(formCollection["ADMIN_QM_CODE"]),
                                                                Convert.ToInt32(formCollection["FROM_MONTH"]), Convert.ToInt32(formCollection["FROM_YEAR"]),
                                                                Convert.ToInt32(formCollection["TO_MONTH"]), Convert.ToInt32(formCollection["TO_YEAR"]),
                                                                formCollection["ATR_STATUS"], formCollection["ROAD_STATUS"], PmgsyScheme, formCollection["imsSanctioned"]);//ATR_Chang




                var distinctObsList = (from obs in atrDetailsModel.OBSS_ATR_LIST
                                       select new
                                       {
                                           obs.QM_OBSERVATION_ID,
                                           obs.MONITOR_NAME,
                                           obs.STATE_NAME,
                                           obs.DISTRICT_NAME,
                                           obs.BLOCK_NAME,
                                           obs.IMS_PACKAGE_ID,
                                           obs.IMS_YEAR,
                                           obs.IMS_ROAD_NAME,
                                           obs.STAGE_PHASE,   //Added By Chandra Darshan Agrawal
                                           obs.IMS_IS_STAGED, //Added By Chandra Darshan Agrawal
                                           obs.QM_INSPECTED_START_CHAINAGE,
                                           obs.QM_INSPECTED_END_CHAINAGE,
                                           obs.QM_INSPECTED_END_CHAINAGE_BRIDGE,
                                           obs.QM_INSPECTION_DATE,
                                           obs.WORK_LENGTH,
                                           obs.IMS_ISCOMPLETED,
                                           obs.OVERALL_GRADE,
                                           obs.NO_OF_PHOTO_UPLOADED,
                                           obs.QM_ATR_STATUS,
                                           obs.PMGSY_SCHEME,
                                           obs.ADMIN_IS_ENQUIRY,
                                           obs.IMS_PROPOSAL_TYPE,
                                           obs.IMS_ISLABUPLOADED,
                                           obs.EXEC_COMPLETION_DATE,
                                           obs.EFORM_ID,
                                           obs.ATR_UPLOAD_ELIGIBILITY,

                                           //----------------- ADDED BY ROHIT BORSE for ATR MARK VERIFICATION
                                           obs.IMS_PR_ROAD_CODE,
                                           obs.ADMIN_QM_CODE
                                       }).Distinct().ToList();


                foreach (var item in distinctObsList)
                {
                    QMObsATRModel obsModel = new QMObsATRModel();
                    obsModel.QM_OBSERVATION_ID = item.QM_OBSERVATION_ID;
                    obsModel.MONITOR_NAME = item.MONITOR_NAME;
                    obsModel.STATE_NAME = item.STATE_NAME;
                    obsModel.DISTRICT_NAME = item.DISTRICT_NAME;
                    obsModel.BLOCK_NAME = item.BLOCK_NAME;
                    obsModel.IMS_PACKAGE_ID = item.IMS_PACKAGE_ID;
                    obsModel.IMS_YEAR = item.IMS_YEAR.ToString() + "-" + (item.IMS_YEAR + 1).ToString();
                    // obsModel.IMS_ROAD_NAME =  "<a href='#' style='color:blue' title='Click here to view Inspections' onClick='ShowInspAgainstRoad(\"" + item.IMS_PR_ROAD_CODE.ToString().Trim() + "$tab" + 3 + "\"); return false;'>" + item.IMS_ROAD_NAME + "</a>" ;
                    obsModel.IMS_ROAD_NAME = "<a href='#' style='color:blue' title='Click here to view Inspections' onClick=ShowInspAgainstRoad('" + URLEncrypt.EncryptParameters1(new String[] { "imsRoadID =" + item.IMS_PR_ROAD_CODE.ToString().Trim() }) + "'); return false;'>" + item.IMS_ROAD_NAME + "</a>";

                    //Added By Chandra Darshan Agrawal
                    obsModel.STAGE_PHASE = item.IMS_PROPOSAL_TYPE.Equals("P") ? (item.IMS_IS_STAGED == "C" ? "Completed" : item.STAGE_PHASE) : "--";

                    obsModel.QM_INSPECTED_START_CHAINAGE = item.QM_INSPECTED_START_CHAINAGE;
                    obsModel.QM_INSPECTED_END_CHAINAGE = item.QM_INSPECTED_END_CHAINAGE;
                    obsModel.QM_INSPECTED_END_CHAINAGE_BRIDGE = item.QM_INSPECTED_END_CHAINAGE_BRIDGE;
                    obsModel.QM_INSPECTION_DATE = item.QM_INSPECTION_DATE;

                    obsModel.WORK_LENGTH = item.WORK_LENGTH;
                    obsModel.IMS_ISCOMPLETED = item.IMS_ISCOMPLETED;
                    obsModel.PMGSY_SCHEME = item.PMGSY_SCHEME;
                    obsModel.OVERALL_GRADE = item.OVERALL_GRADE;
                    obsModel.NO_OF_PHOTO_UPLOADED = item.NO_OF_PHOTO_UPLOADED;
                    obsModel.QM_ATR_STATUS = item.QM_ATR_STATUS;
                    obsModel.ADMIN_IS_ENQUIRY = item.ADMIN_IS_ENQUIRY.Equals("Y") ? "Yes" : "No";
                    obsModel.IMS_PROPOSAL_TYPE = item.IMS_PROPOSAL_TYPE.Equals("P") ? "Road" : "LSB";
                    obsModel.SHOW_OBS_LINK = "<a href='#' title='Click here to view details' class='ui-icon ui-icon-zoomin ui-align-center' onClick='ShowATRObsDetails(\"" + item.QM_OBSERVATION_ID.ToString().Trim() + "\"); return false;'>View</a>";
                    obsModel.IMS_ISLABUPLOADED = ((item.IMS_ISLABUPLOADED.Trim().Equals("Yes"))
                                                     ?
                                                    "<a href='#' title='Click here to view uploaded lab photograph against this package' class='ui-icon ui-icon-zoomin ui-align-center' onClick='ShowLabDetails(\"" + dbContext.QUALITY_QM_LAB_MASTER.Where(x => x.IMS_PACKAGE_ID.Equals(item.IMS_PACKAGE_ID)).FirstOrDefault().QM_LAB_ID + "\"); return false;'>Lab Details</a>"
                                                    :
                                                    "<a href='#' title='Lab photograph is not available' class='ui-icon ui-icon-locked ui-align-center' />"); //Added by Deendayal 

                    obsModel.EXEC_COMPLETION_DATE = item.EXEC_COMPLETION_DATE == null ? "-" : item.EXEC_COMPLETION_DATE.ToString().ToString().Split(' ')[0].Replace('-', '/');

                    obsModel.VIEW_INSPECTION_REPORT_LINK =
                    (dbContext.QUALITY_INSPECTION_REPORT_FILE.Where(c => c.QM_OBSERVATION_ID == item.QM_OBSERVATION_ID).Any())
                                        ? "<a href='#' title='Click here to view details' class='ui-icon ui-icon-zoomin ui-align-center' onClick='ViewInspectionReportATR(\"" + item.QM_OBSERVATION_ID.ToString().Trim() + "$ATR" + "\"); return false;'>View</a>"
                                        : (PMGSYSession.Current.RoleCode == 8 || PMGSYSession.Current.RoleCode == 9)
                                            ? "<a href='#' title='No files uploaded yet'><center>_</center></a>"
                                            : "NA";
                    //Physical path pdf view 
                   obsModel.EFORM_PDF_VIEW = item.EFORM_ID == null ? "-" : (dbContext.EFORM_PDF_UPLOAD_DETAIL.Any(s => s.EFORM_ID == item.EFORM_ID && s.USER_TYPE == "Q" && s.IS_FINALISED == "Y") ? "<a  href='#'  onClick=viewCombinedPart_1_2_Pdf('" + item.EFORM_ID.ToString().Trim()+ "'); title='Click here to view part-I,II pdf'><input type='button' value='View'/></a>" : "-");
                    //Virtual path pdf view 
                    // obsModel.EFORM_PDF_VIEW = item.EFORM_ID == null ? "-" : (dbContext.EFORM_PDF_UPLOAD_DETAIL.Any(s => s.EFORM_ID == item.EFORM_ID && s.USER_TYPE == "Q" && s.IS_FINALISED == "Y") ? "<a href='#' title='title='Click here to view part-I,II pdf'  onClick='viewCombinePdfVirtualDir(\"" + item.EFORM_ID.ToString() + "\");'   ><input type='button' value='View'/></a>" : "-");

                    // obsModel.EFORM_PDF_PREVIEW = item.EFORM_ID == null ? "-" : (dbContext.EFORM_PDF_UPLOAD_DETAIL.Any(s => s.EFORM_ID == item.EFORM_ID && s.USER_TYPE == "Q" && s.IS_FINALISED == "Y") ? "<input type='button' id='btnViewPDF' value='Preview' onClick=viewCombinePdfData('" + URLEncrypt.EncryptParameters1(new String[] { "imsRoadID =" + item.EFORM_ID.ToString().Trim() }) + "');   target=_blank />" : "-");
                    obsModel.EFORM_PDF_PREVIEW = item.EFORM_ID == null ? "-" : (dbContext.EFORM_PDF_UPLOAD_DETAIL.Any(s => s.EFORM_ID == item.EFORM_ID && s.USER_TYPE == "Q" && s.IS_FINALISED == "Y") ?
                        (item.IMS_PROPOSAL_TYPE.Equals("P") ? "<input type='button' id='btnViewPDF' value='Preview' onClick=viewCombinePdfData('" + URLEncrypt.EncryptParameters1(new String[] { "imsRoadID =" + item.EFORM_ID.ToString().Trim() }) + "');   target=_blank />"
                        : "<input type='button' id='btnViewPDF' value='Preview' onClick=viewBridgeCombinePdfData('" + URLEncrypt.EncryptParameters1(new String[] { "imsRoadID =" + item.EFORM_ID.ToString().Trim() }) + "');   target=_blank />")
                        : "-");

                    atrDetailsModel.OBS_LIST.Add(obsModel);
                }

                //using (System.IO.StreamWriter sw = System.IO.File.AppendText(System.IO.Path.Combine("D:\\ErrorLog\\1QMErrorLog_" + System.DateTime.Now.Date.ToString("dd_MM_yyyy") + ".txt", "")))
                //{
                //    sw.WriteLine("---------------------------------------------------------------------------------------");
                //    sw.WriteLine("AT setp 4 ");
                //    sw.WriteLine("---------------------------------------------------------------------------------------");
                //    sw.Close();
                //}


                #region Changes by SAMMED A. PATIL on 08JAN2018 for uploading missing ATR files
                string PhysicalPath = ConfigurationManager.AppSettings["QUALITY_INSPECTION_FILE_UPLOAD_ATR"];
                //For self Reference
                System.IO.File.Exists(Path.Combine(PhysicalPath, HttpUtility.UrlEncode("2636.pdf")).ToString().Replace(@"\\", @"//").Replace(@"\", @"/"));
                #endregion

                //using (System.IO.StreamWriter sw = System.IO.File.AppendText(System.IO.Path.Combine("D:\\ErrorLog\\1QMErrorLog_" + System.DateTime.Now.Date.ToString("dd_MM_yyyy") + ".txt", "")))
                //{
                //    sw.WriteLine("---------------------------------------------------------------------------------------");
                //    sw.WriteLine("AT setp 5 ");
                //    sw.WriteLine("---------------------------------------------------------------------------------------");
                //    sw.Close();
                //}


                //int index = 0;
                foreach (var item in atrDetailsModel.OBSS_ATR_LIST)
                {
                    // Populate & Add ATR Details for each Observation Id
                    QMATRModel atrModel = new QMATRModel();
                    atrModel.QM_OBSERVATION_ID = item.QM_OBSERVATION_ID;
                    atrModel.QM_ATR_ID = item.QM_ATR_ID;
                    atrModel.ATR_ENTRY_DATE = item.ATR_ENTRY_DATE;
                    atrModel.ATR_REGRADE_STATUS = item.ATR_REGRADE_STATUS.Trim().Equals("N") ? "No" : "Yes";
                    atrModel.ATR_REGRADE_REMARKS = item.ATR_REGRADE_REMARKS;
                    atrModel.ATR_REGRADE_DATE = item.ATR_REGRADE_DATE;
                    atrModel.ATR_IS_DELETED = item.ATR_IS_DELETED;
                    atrModel.QM_ATR_STATUS = item.QM_ATR_STATUS;

                    atrModel.IS_SUBMITTED = item.QM_ATR_STATUS.Trim().Equals("N") ? "No" : "Yes";

                    //Below Code Commented on 30-01-2023
                    //atrModel.ATR_UPLOAD_VIEW_LINK =
                    //    item.ATR_UPLOAD_ELIGIBILITY ==0 ? "Not Eligible For ATR Upload" :
                    //    item.IMS_SANCTIONED == "D" ? "-" :
                    //    ((PMGSYSession.Current.RoleCode == 22 || PMGSYSession.Current.RoleCode == 38 || PMGSYSession.Current.RoleCode == 5 || PMGSYSession.Current.RoleCode == 54)//for PIU or PIURCPLWE view only 
                    //    ? (item.QM_ATR_ID != null)
                    //                ? item.ATR_IS_DELETED == "Y" ? "--" : (PMGSYSession.Current.RoleCode == 5 && item.ATR_IS_DELETED == "N" && !(System.IO.File.Exists(Path.Combine(PhysicalPath, HttpUtility.UrlEncode(item.QM_ATR_ID + ".pdf")).ToString().Replace(@"\\", @"//").Replace(@"\", @"/"))))
                    //                    ? "File Not Available"
                    //                        : "<a href='#' title='Click here to view uploaded ATR' class='ui-icon ui-icon-zoomin ui-align-center' onClick='DownloadATR(\"" + URLEncrypt.EncryptParameters(new string[] { item.QM_ATR_ID + ".pdf" + "$" + item.QM_ATR_ID }) + "\"); return false;'>Download</a>"
                    //                            : "<a href='#' class='ui-icon ui-icon-locked ui-align-center' />"
                    //                    : (item.QM_ATR_STATUS.Trim().Equals("N")) // Upload/View
                    //                    ? (item.IMS_ISLABUPLOADED.Equals("Yes") || qmBAL.GetCurrentworkStatus(item.IMS_PR_ROAD_CODE).Equals("C") || qmBAL.GetCurrentworkStatus(item.IMS_PR_ROAD_CODE).Equals("X") || qmBAL.GetCurrentworkStatus(item.IMS_PR_ROAD_CODE).Equals("")
                    //                        ?
                    //#region
                    //                            //DO NOT DELETE BELOW COMMENTED CODE.
                    //                            //Commented out for a temporary period to enable ATR upload option
                    //                            //below both conditions added by abhinav pathak on 18-08-2019
                    //                            //(dbContext.QUALITY_QM_INSPECTION_FILES.Where(obs => obs.QM_OBSERVATION_ID == item.QM_OBSERVATION_ID && obs.QM_FILES_FINALIZED == "Y").ToList().SequenceEqual(dbContext.QUALITY_QM_INSPECTION_FILES.Where
                    //                            //(obj => obj.QM_OBSERVATION_ID == item.QM_OBSERVATION_ID).ToList())
                    //                            //&& dbContext.QUALITY_QM_INSPECTION_FILES.Any(obj => obj.QM_OBSERVATION_ID == item.QM_OBSERVATION_ID))
                    //                            //?
                    //#endregion
                    //                            "<a href='#' title='Click here to upload ATR' class='ui-icon ui-icon-plusthick ui-align-center' onClick='UploadATR(\"" + item.QM_OBSERVATION_ID.ToString().Trim() + "$" + Convert.ToString(item.QM_ATR_ID).Trim() + "\"); return false;'>Upload</a>" //: "-"
                    //                          : "<a href='#' title='Lab photos are not available' class='ui-icon ui-icon-locked ui-align-center' />")
                    //                        : (item.QM_ATR_ID != null)
                    //                        ? item.ATR_IS_DELETED == "Y" ? "--"
                    //                        : !(System.IO.File.Exists(Path.Combine(PhysicalPath, HttpUtility.UrlEncode(item.QM_ATR_ID + ".pdf")).ToString().Replace(@"\\", @"//").Replace(@"\", @"/")))
                    //                            ? "<a href='#' title='Click here to upload missing ATR File' class='ui-icon ui-icon-plusthick ui-align-center' onClick='UploadMissingATR(\"" + item.QM_OBSERVATION_ID.ToString().Trim() + "$" + Convert.ToString(item.QM_ATR_ID).Trim() + "\"); return false;'>Upload</a>"
                    //                            : "<a href='#' title='Click here to view uploaded ATR' class='ui-icon ui-icon-zoomin ui-align-center' onClick='DownloadATR(\"" + URLEncrypt.EncryptParameters(new string[] { item.QM_ATR_ID + ".pdf" + "$" + item.QM_ATR_ID }) + "\"); return false;'>Download</a>"
                    //                        : "<a href='#' class='ui-icon ui-icon-locked ui-align-center' />");

                    //Below Code Added on 30-01-2023
                    atrModel.ATR_UPLOAD_VIEW_LINK = (item.QM_ATR_ID != null)
                                                  ? item.ATR_IS_DELETED == "Y"
                                                    ? "--"
                                                    : (System.IO.File.Exists(Path.Combine(PhysicalPath, HttpUtility.UrlEncode(item.QM_ATR_ID + ".pdf")).ToString().Replace(@"\\", @"//").Replace(@"\", @"/"))) ? "<a href='#' title='Click here to view uploaded ATR' class='ui-icon ui-icon-zoomin ui-align-center' onClick='DownloadATR(\"" + URLEncrypt.EncryptParameters(new string[] { item.QM_ATR_ID + ".pdf" + "$" + item.QM_ATR_ID }) + "\"); return false;'>Download</a>" : "<a href='#' title='Click here to upload missing ATR File' class='ui-icon ui-icon-plusthick ui-align-center' onClick='UploadMissingATR(\"" + item.QM_OBSERVATION_ID.ToString().Trim() + "$" + Convert.ToString(item.QM_ATR_ID).Trim() + "\"); return false;'>Upload</a>"
                                                  : "<a href='#' class='ui-icon ui-icon-locked ui-align-center' title='ATR is not uploaded' />";

                    //----------------- ADDED BY ROHIT BORSE for ATR MARK VERIFICATION                    

                    //atrModel.VERIFICATION_ATR_CODE = item.QM_ATR_ID != null
                    //                                ? PMGSYSession.Current.RoleCode == 9
                    //                                    ? "<a href='#' title='Click here to view list for Verification of ATR' class='ui-icon ui-icon-zoomin ui-align-center' onClick='ViewVerificationATR_ListByRoadCode_Inspdate_ObsId_ATRId(\"" + item.IMS_PR_ROAD_CODE.ToString().Trim() + "$" + item.QM_INSPECTION_DATE.ToString().Trim() + "$" + item.QM_OBSERVATION_ID.ToString().Trim() + "$" + item.QM_ATR_ID.ToString().Trim() + "\"); return false;'></a>"
                    //                                    : dbContext.QUALITY_ATR_FILE.Where(a => a.QM_ATR_ID == item.QM_ATR_ID).Select(a => a.ATR_VERIFICATION_FINALIZED).First() == "Y"
                    //                                        ? "<a href='#' class='ui-icon ui-icon-locked ui-align-center' onClick='ATR_AlreadyFinilized(); return false;' />"
                    //                                        : "<a href='#' title='Click here to view list for Verification of ATR' class='ui-icon ui-icon-plusthick ui-align-center' onClick='ViewVerificationATR_ListByRoadCode_Inspdate_ObsId_ATRId(\"" + item.IMS_PR_ROAD_CODE.ToString().Trim() + "$" + item.QM_INSPECTION_DATE.ToString().Trim() + "$" + item.QM_OBSERVATION_ID.ToString().Trim() + "$" + item.QM_ATR_ID.ToString().Trim() + "\"); return false;'></a>"
                    //                                : "<a href='#' class='ui-icon ui-icon-locked ui-align-center' onClick='lock_MarkATRVerification(); return false;' />";

                    //atrModel.VERIFICATION_ATR_CODE = item.QM_ATR_ID != null
                    //                             ? (PMGSYSession.Current.RoleCode == 8 || PMGSYSession.Current.RoleCode == 48 || PMGSYSession.Current.RoleCode == 69)
                    //                               ? dbContext.QUALITY_ATR_FILE.Where(a => a.QM_ATR_ID == item.QM_ATR_ID).Select(a => a.ATR_VERIFICATION_FINALIZED).First() == "Y"
                    //                                   ? "<a href='#' class='ui-icon ui-icon-locked ui-align-center' onClick='lock_MarkATRVerification(); return false;' />"
                    //                                   : "<a href='#' title='Click here to view list for Verification of ATR' class='ui-icon ui-icon-plusthick ui-align-center' onClick='ViewVerificationATR_ListByRoadCode_Inspdate_ObsId_ATRId(\"" + item.IMS_PR_ROAD_CODE.ToString().Trim() + "$" + item.QM_INSPECTION_DATE.ToString().Trim() + "$" + item.QM_OBSERVATION_ID.ToString().Trim() + "$" + item.QM_ATR_ID.ToString().Trim() + "\"); return false;'></a>"
                    //                               : "<a href='#' title='Click here to view list for Verification of ATR' class='ui-icon ui-icon-zoomin ui-align-center' onClick='ViewVerificationATR_ListByRoadCode_Inspdate_ObsId_ATRId(\"" + item.IMS_PR_ROAD_CODE.ToString().Trim() + "$" + item.QM_INSPECTION_DATE.ToString().Trim() + "$" + item.QM_OBSERVATION_ID.ToString().Trim() + "$" + item.QM_ATR_ID.ToString().Trim() + "\"); return false;'></a>"
                    //                             : "<a href='#' class='ui-icon ui-icon-locked ui-align-center' onClick='lock_MarkATRVerification(); return false;' />";

                    //Below code commented on 30-01-2023
                    //atrModel.VERIFICATION_ATR_CODE = item.QM_ATR_ID != null
                    //                          ? (PMGSYSession.Current.RoleCode == 8 || PMGSYSession.Current.RoleCode == 48 || PMGSYSession.Current.RoleCode == 69)
                    //                            ? dbContext.QUALITY_ATR_FILE.Where(a => a.QM_ATR_ID == item.QM_ATR_ID).Select(a => a.ATR_VERIFICATION_FINALIZED).First() == "Y"
                    //                                ? "<a href='#' class='ui-icon ui-icon-locked ui-align-center' onClick='ATR_AlreadyFinilized(); return false;' />"
                    //                                : "<a href='#' title='Click here to view list for Verification of ATR' class='ui-icon ui-icon-plusthick ui-align-center' onClick='ViewVerificationATR_ListByRoadCode_Inspdate_ObsId_ATRId(\"" + item.IMS_PR_ROAD_CODE.ToString().Trim() + "$" + item.QM_INSPECTION_DATE.ToString().Trim() + "$" + item.QM_OBSERVATION_ID.ToString().Trim() + "$" + item.QM_ATR_ID.ToString().Trim() + "\"); return false;'></a>"
                    //                            : (dbContext.QUALITY_ATR_FILE.Where(a => a.QM_ATR_ID == item.QM_ATR_ID).Select(a => a.ATR_VERIFICATION_FINALIZED).First() == "Y" ? "<a href='#' title='Click here to view details' class='ui-icon ui-icon-zoomin ui-align-center' onClick='ShowATRObsDetails(\"" + (dbContext.QUALITY_ATR_FILE.Where(a => a.QM_ATR_ID == item.QM_ATR_ID).Select(a => a.SQM_OBSERVATION_ID).First()) + "\"); return false;'>View</a>" : "<a href='#' class='ui-icon ui-icon-locked ui-align-center' onClick='lock_MarkATRVerification(); return false;' />")
                    //                          : "<a href='#' class='ui-icon ui-icon-locked ui-align-center' onClick='lock_MarkATRVerification(); return false;' />";

                    //Below code Added on 30-01-2023
                    CheckInspAvailableCount(item.IMS_PR_ROAD_CODE, item.QM_INSPECTION_DATE.ToString().Trim(), out long totalrecords);

                    var noOfRec = dbContext.QUALITY_ATR_FILE.Where(a => a.QM_OBSERVATION_ID == item.QM_OBSERVATION_ID).Count();
                    atrModel.VERIFICATION_ATR_CODE = noOfRec > 0
                                                     ? "<a href='#' class='ui-icon ui-icon-locked ui-align-center'; return false;' />"
                                                     : (item.ATR_UPLOAD_ELIGIBILITY == 0 ? "Not Eligible For ATR Upload" :
                                                                            (PMGSYSession.Current.RoleCode == 8 || PMGSYSession.Current.RoleCode == 48 || PMGSYSession.Current.RoleCode == 69)
                                                                            ? dbContext.QUALITY_ATR_FILE.Where(a => a.QM_ATR_ID == item.QM_ATR_ID).Select(a => a.ATR_VERIFICATION_FINALIZED).FirstOrDefault() == "Y"
                                                                                ? "<a href='#' class='ui-icon ui-icon-locked ui-align-center' onClick='ATR_AlreadyFinilized(); return false;' />"
                                                                                : totalrecords > 0
                                                                                        ? item.ATR_REGRADE_STATUS.Trim().Equals("R") ? "<a href='#' class='ui-icon ui-icon-locked ui-align-center'; return false;' />" : "<a href='#' title='Click here to view list for Verification of ATR' class='ui-icon ui-icon-plusthick ui-align-center' onClick='UploadATRFile(\"" + item.QM_OBSERVATION_ID.ToString().Trim() + "$" + Convert.ToString(item.QM_ATR_ID).Trim() + "$" + item.QM_INSPECTION_DATE.ToString().Trim().Replace(" ", "+") + "$" + item.IMS_PR_ROAD_CODE.ToString().Trim() + "\");return false;'></a>"
                                                                                        : "<a href='#' class='ui-icon ui-icon-locked ui-align-center' title='No inspection is available for ATR mark'; return false;' />"
                                                                            : (dbContext.QUALITY_ATR_FILE.Where(a => a.QM_ATR_ID == item.QM_ATR_ID).Select(a => a.ATR_VERIFICATION_FINALIZED).FirstOrDefault() == "Y" ? "<a href='#' title='Click here to view details' class='ui-icon ui-icon-zoomin ui-align-center' onClick='ShowATRObsDetails(\"" + (dbContext.QUALITY_ATR_FILE.Where(a => a.QM_ATR_ID == item.QM_ATR_ID).Select(a => a.SQM_OBSERVATION_ID).FirstOrDefault()) + "\"); return false;'>View</a>" : "<a href='#' class='ui-icon ui-icon-locked ui-align-center' onClick='lock_MarkATRVerification(); return false;' />"));



                    //Below code Commented on 30-01-2023
                    //atrModel.ATR_ACCEPTANCE_LINK =
                    //     (PMGSYSession.Current.RoleCode == 22 || PMGSYSession.Current.RoleCode == 38 || PMGSYSession.Current.RoleCode == 5 || PMGSYSession.Current.RoleCode == 54)//for PIU or PIURCPLWE view only
                    //     ? item.ATR_REGRADE_STATUS.Trim().Equals("A")
                    //                    ? "Accepted"
                    //                    : item.ATR_REGRADE_STATUS.Trim().Equals("R")
                    //                        ? "Rejected"
                    //                        : item.ATR_REGRADE_STATUS.Equals("V")
                    //                            ? "To be Verified"
                    //                            : item.ATR_REGRADE_STATUS.Equals("D")
                    //                                ? "Non Rectifible Deffect"
                    //                                : ""
                    //     : item.ATR_REGRADE_STATUS.Trim().Equals("A")     // Acceptance
                    //                    ? "Accepted"
                    //                    : item.ATR_REGRADE_STATUS.Equals("V")
                    //                        ? "To be Verified"
                    //                        : item.ATR_REGRADE_STATUS.Equals("D")
                    //                            ? "Non Rectifiable Deffect"
                    //    //: (item.ATR_REGRADE_STATUS.Equals("C") && PMGSYSession.Current.RoleCode == 5) // Changes done on 13-04-2015 as per given ECR by Aanad. 
                    //                            : (item.ATR_REGRADE_STATUS.Equals("C") && PMGSYSession.Current.RoleCode == 9)
                    //                                ? "Technical Committee" + // If Tech Committee, then append + sign to upload again
                    //                                          (atrModel.QM_ATR_ID == (dbContext.QUALITY_ATR_FILE.Where(c => c.QM_OBSERVATION_ID == atrModel.QM_OBSERVATION_ID).Select(c => c.QM_ATR_ID).Max())
                    //                                             ? "<a href='#' title='Click here to upload ATR' class='ui-icon ui-icon-plusthick ui-align-center' onClick='UploadATR(\"" + item.QM_OBSERVATION_ID.ToString().Trim() + "$" + Convert.ToString(item.QM_ATR_ID).Trim() + "\"); return false;'>Upload</a>"
                    //                                             : "")
                    //                                : item.ATR_REGRADE_STATUS.Trim().Equals("R") // if any of the ATR against Obs Id is Accepted then dont provide link to upload.
                    //                                    ? item.QM_ATR_STATUS.Equals("A")
                    //                                        ? "Rejected"
                    //                                        : "Rejected" + //If Rejected atr is last against Observation, then append + sign to upload again
                    //                                          (atrModel.QM_ATR_ID == (dbContext.QUALITY_ATR_FILE.Where(c => c.QM_OBSERVATION_ID == atrModel.QM_OBSERVATION_ID).Select(c => c.QM_ATR_ID).Max())
                    //                                             ? "<a href='#' title='Click here to upload ATR' class='ui-icon ui-icon-plusthick ui-align-center' onClick='UploadATR(\"" + item.QM_OBSERVATION_ID.ToString().Trim() + "$" + Convert.ToString(item.QM_ATR_ID).Trim() + "\"); return false;'>Upload</a>"
                    //                                             : "")
                    //                                    : "";

                    //Below code added on 20-03-2023
                    CheckRejectFlagStatus(item.QM_OBSERVATION_ID, out bool rejectLockStatus);//rejectLockStatus=true , Lock Reject ;rejectLockStatus=false , active '+' sign
                    atrModel.ATR_ACCEPTANCE_LINK =
                         (PMGSYSession.Current.RoleCode == 22 || PMGSYSession.Current.RoleCode == 38 || PMGSYSession.Current.RoleCode == 5 || PMGSYSession.Current.RoleCode == 54)//for PIU or PIURCPLWE view only
                         ? item.ATR_REGRADE_STATUS.Trim().Equals("A")
                                        ? "Accepted"
                                        : item.ATR_REGRADE_STATUS.Trim().Equals("R")
                                            ? "Rejected"
                                            : item.ATR_REGRADE_STATUS.Equals("V")
                                                ? "To be Verified"
                                                : item.ATR_REGRADE_STATUS.Equals("D")
                                                    ? "Non Rectifible Deffect"
                                                    : ""
                         : item.ATR_REGRADE_STATUS.Trim().Equals("A")     // Acceptance
                                        ? "Accepted"
                                        : item.ATR_REGRADE_STATUS.Equals("V")
                                            ? "To be Verified"
                                            : item.ATR_REGRADE_STATUS.Equals("D")
                                                ? "Non Rectifiable Deffect"
                                                //: (item.ATR_REGRADE_STATUS.Equals("C") && PMGSYSession.Current.RoleCode == 5) // Changes done on 13-04-2015 as per given ECR by Aanad. 
                                                : (item.ATR_REGRADE_STATUS.Equals("C") && PMGSYSession.Current.RoleCode == 9)
                                                    ? "Technical Committee"  // If Tech Committee, then append + sign to upload again


                                                    + (atrModel.QM_ATR_ID == (dbContext.QUALITY_ATR_FILE.Where(c => c.QM_OBSERVATION_ID == atrModel.QM_OBSERVATION_ID).Select(c => c.QM_ATR_ID).Max())
                                                       ? "<a href='#' title='Click here to upload ATR' class='ui-icon ui-icon-plusthick ui-align-center' onClick='UploadATR(\"" + item.QM_OBSERVATION_ID.ToString().Trim() + "$" + Convert.ToString(item.QM_ATR_ID).Trim() + "\"); return false;'>Upload</a>"
                                                       : "")

                                                    : item.ATR_REGRADE_STATUS.Trim().Equals("R") // if any of the ATR against Obs Id is Accepted then dont provide link to upload.
                                                        ? item.QM_ATR_STATUS.Equals("A")
                                                            ? "Rejected"
                                                            : "Rejected" + //If Rejected atr is last against Observation, then append + sign to upload again
                                                                           //Below line is commented on 18-01-2023
                                                                           //  (atrModel.QM_ATR_ID == (dbContext.QUALITY_ATR_FILE.Where(c => c.QM_OBSERVATION_ID == atrModel.QM_OBSERVATION_ID).Select(c => c.QM_ATR_ID).Max() )
                                                                           //Below line is Added on 30-01-2023
                                                                ((atrModel.QM_ATR_ID == (dbContext.QUALITY_ATR_FILE.Where(c => c.QM_OBSERVATION_ID == atrModel.QM_OBSERVATION_ID).Select(c => c.QM_ATR_ID).Max()) && (PMGSYSession.Current.RoleCode == 8 || PMGSYSession.Current.RoleCode == 48 || PMGSYSession.Current.RoleCode == 69))
                                                                //Below line is commented on 30-01-2023
                                                                //? "<a href='#' title='Click here to upload ATR' class='ui-icon ui-icon-plusthick ui-align-center' onClick='UploadATR(\"" + item.QM_OBSERVATION_ID.ToString().Trim() + "$" + Convert.ToString(item.QM_ATR_ID).Trim() + "\"); return false;'>Upload</a>"
                                                                //Below line is commented on 20-03-2023
                                                                //? "<a href='#' title='Click here to upload ATR' class='ui-icon ui-icon-plusthick ui-align-center' onClick='UploadATRFile(\"" + item.QM_OBSERVATION_ID.ToString().Trim() + "$" + Convert.ToString(item.QM_ATR_ID).Trim() + "$" + item.QM_INSPECTION_DATE.ToString().Trim().Replace(" ", "+") + "$" + item.IMS_PR_ROAD_CODE.ToString().Trim() + "\");return false;'>Upload</a>"
                                                                //Below line is Added on 20-03-2023
                                                                ? rejectLockStatus ? "<a href='#' class='ui-icon ui-icon-locked ui-align-center' title='ATR with regrade status Technical committee , Verification , non-rectifiable defect cannot upload again.'; return false;' />" : "<a href='#' title='Click here to upload ATR' class='ui-icon ui-icon-plusthick ui-align-center' onClick='UploadATRFile(\"" + item.QM_OBSERVATION_ID.ToString().Trim() + "$" + Convert.ToString(item.QM_ATR_ID).Trim() + "$" + item.QM_INSPECTION_DATE.ToString().Trim().Replace(" ", "+") + "$" + item.IMS_PR_ROAD_CODE.ToString().Trim() + "\");return false;'>Upload</a>"
                                                                : "")
                                                        : "";

                    atrModel.ATR_REGRADE_LINK =
                        (PMGSYSession.Current.RoleCode == 22 || PMGSYSession.Current.RoleCode == 38 || PMGSYSession.Current.RoleCode == 5 || PMGSYSession.Current.RoleCode == 54)//for PIU or PIURCPLWE view only
                        ? (item.ATR_REGRADE_STATUS.Trim().Equals("A"))
                            ? "<a href='#' title='Click here to see observation details against ATR' class='ui-icon ui-icon-zoomin ui-align-center' onClick='ShowATRGradingDetails(\"" + item.QM_OBSERVATION_ID.ToString().Trim() + "\"); return false;'>Regrade</a>"
                            : "<a href='#' class='ui-icon ui-icon-locked ui-align-center' />"
                        : (item.QM_ATR_ID != null) ? ((dbContext.QUALITY_ATR_FILE.Where(x => x.QM_OBSERVATION_ID == item.QM_OBSERVATION_ID).Max(y => y.QM_ATR_ID) != item.QM_ATR_ID) ? "<a href='#' class='ui-icon ui-icon-locked ui-align-center' />" :
                        ((item.ATR_REGRADE_STATUS.Trim().Equals("U") || item.ATR_REGRADE_STATUS.Trim().Equals("C") || item.ATR_REGRADE_STATUS.Trim().Equals("V")) && item.ATR_IS_DELETED.Equals("N")) // Change done by Chandra Darshan Agrawal
                            ? "<a href='#' title='Click here to regrade ATR' class='ui-icon ui-icon-plusthick ui-align-center' onClick='RegradeATR(\"" + item.QM_OBSERVATION_ID.ToString().Trim() + "\"); return false;'>Regrade</a>"
                            : (item.ATR_REGRADE_STATUS.Trim().Equals("A"))
                                ? "<a href='#' title='Click here to see observation details against ATR' class='ui-icon ui-icon-zoomin ui-align-center' onClick='ShowATRGradingDetails(\"" + item.QM_OBSERVATION_ID.ToString().Trim() + "\"); return false;'>Regrade</a>"
                                : "<a href='#' class='ui-icon ui-icon-locked ui-align-center' />") : "<a href='#' class='ui-icon ui-icon-locked ui-align-center' />";

                    atrModel.ATR_DELETE_LINK =
                         (PMGSYSession.Current.RoleCode == 22 || PMGSYSession.Current.RoleCode == 38 || PMGSYSession.Current.RoleCode == 5 || PMGSYSession.Current.RoleCode == 54)//for PIU or PIURCPLWE view only
                         ? "<a href='#' class='ui-icon ui-icon-locked ui-align-center' />"
                         : (item.QM_ATR_ID != null && item.QM_ATR_STATUS.Trim() != "A" && item.QM_ATR_STATUS.Trim() != "N" && (item.QM_ATR_ID == dbContext.QUALITY_ATR_FILE.Where(c => c.QM_OBSERVATION_ID == item.QM_OBSERVATION_ID).Select(c => c.QM_ATR_ID).Max()))
                                ? "<a href='#' title='Click here to delete ATR details' class='ui-icon ui-icon-trash ui-align-center' onClick='DeleteATR(\"" +
                                    item.QM_OBSERVATION_ID.ToString().Trim() + "\",\"" + dbContext.QUALITY_ATR_FILE.Where(a => a.QM_OBSERVATION_ID == item.QM_OBSERVATION_ID && a.ATR_IS_DELETED == "N").Select(a => a.QM_ATR_ID).FirstOrDefault()
                                    + "\"); return false;'>Delete</a>"
                                : "<a href='#' class='ui-icon ui-icon-locked ui-align-center' />";

                    atrDetailsModel.ATR_LIST.Add(atrModel);

                    //index++; // increment index for each record.
                }
                //using (System.IO.StreamWriter sw = System.IO.File.AppendText(System.IO.Path.Combine("D:\\ErrorLog\\1QMErrorLog_" + System.DateTime.Now.Date.ToString("dd_MM_yyyy") + ".txt", "")))
                //{
                //    sw.WriteLine("---------------------------------------------------------------------------------------");
                //    sw.WriteLine("AT setp 6 ");
                //    sw.WriteLine("---------------------------------------------------------------------------------------");
                //    sw.Close();
                //}


                return View(atrDetailsModel);
            }
            catch (Exception ex)
            {
                //using (System.IO.StreamWriter sw = System.IO.File.AppendText(System.IO.Path.Combine("D:\\ErrorLog\\1QMErrorLog_" + System.DateTime.Now.Date.ToString("dd_MM_yyyy") + ".txt", "")))
                //{
                //    sw.WriteLine("---------------------------------------------------------------------------------------");
                //    sw.WriteLine("AT setp  7  " + ex.Message.ToString()  );
                //    sw.WriteLine("---------------------------------------------------------------------------------------");
                //    sw.Close();
                //}

                ErrorLog.LogError(ex, "QualityMonitoring.ATRDetails()");
                atrDetailsModel.ERROR = "Error occurred while processing your request";
                return View(atrDetailsModel); //return model as null

            }
            finally
            {
                dbContext.Dispose();
            }
        }




        //public ActionResult ATRDetails(FormCollection formCollection)
        //{
        //    QualityMonitoringBAL qmBAL = new QualityMonitoringBAL();
        //    QMATRDetailsModel atrDetailsModel = new QMATRDetailsModel();
        //    atrDetailsModel.ATR_LIST = new List<QMATRModel>();
        //    atrDetailsModel.OBS_LIST = new List<QMObsATRModel>();
        //    dbContext = new PMGSYEntities();

        //    int PmgsyScheme = Convert.ToInt32(formCollection["schemeType"]);//ATR_Change
        //    if (PmgsyScheme < 0 || PmgsyScheme > 4)
        //    {
        //        ModelState.AddModelError("SchemeError", "Scheme is not valid");
        //        return View(atrDetailsModel); //return model as null
        //    }//ATR_Change

        //    var imsSnc = formCollection["imsSanctioned"];

        //    try
        //    {


        //        //using (System.IO.StreamWriter sw = System.IO.File.AppendText(System.IO.Path.Combine("D:\\ErrorLog\\1QMErrorLog_" + System.DateTime.Now.Date.ToString("dd_MM_yyyy") + ".txt", "")))
        //        //{
        //        //    sw.WriteLine("---------------------------------------------------------------------------------------");
        //        //    sw.WriteLine("AT setp 1 ");
        //        //    sw.WriteLine("---------------------------------------------------------------------------------------");
        //        //    sw.Close();
        //        //}



        //        atrDetailsModel.OBS_ATR_LIST = qmBAL.ATRDetailsBAL(Convert.ToInt32(formCollection["MAST_STATE_CODE"]), Convert.ToInt32(formCollection["ADMIN_QM_CODE"]),
        //                                                        Convert.ToInt32(formCollection["FROM_MONTH"]), Convert.ToInt32(formCollection["FROM_YEAR"]),
        //                                                        Convert.ToInt32(formCollection["TO_MONTH"]), Convert.ToInt32(formCollection["TO_YEAR"]),
        //                                                        formCollection["ATR_STATUS"], formCollection["ROAD_STATUS"], PmgsyScheme);//ATR_Change
        //        //using (System.IO.StreamWriter sw = System.IO.File.AppendText(System.IO.Path.Combine("D:\\ErrorLog\\1QMErrorLog_" + System.DateTime.Now.Date.ToString("dd_MM_yyyy") + ".txt", "")))
        //        //{
        //        //    sw.WriteLine("---------------------------------------------------------------------------------------");
        //        //    sw.WriteLine("AT setp 2");
        //        //    sw.WriteLine("---------------------------------------------------------------------------------------");
        //        //    sw.Close();
        //        //}



        //        var distinctObsList = (from obs in atrDetailsModel.OBS_ATR_LIST
        //                               select new
        //                               {
        //                                   obs.QM_OBSERVATION_ID,
        //                                   obs.MONITOR_NAME,
        //                                   obs.STATE_NAME,
        //                                   obs.DISTRICT_NAME,
        //                                   obs.BLOCK_NAME,
        //                                   obs.IMS_PACKAGE_ID,
        //                                   obs.IMS_YEAR,
        //                                   obs.IMS_ROAD_NAME,
        //                                   obs.QM_INSPECTED_START_CHAINAGE,
        //                                   obs.QM_INSPECTED_END_CHAINAGE,
        //                                   obs.QM_INSPECTED_END_CHAINAGE_BRIDGE,
        //                                   obs.QM_INSPECTION_DATE,
        //                                   obs.WORK_LENGTH,
        //                                   obs.IMS_ISCOMPLETED,
        //                                   obs.OVERALL_GRADE,
        //                                   obs.NO_OF_PHOTO_UPLOADED,
        //                                   obs.QM_ATR_STATUS,
        //                                   obs.PMGSY_SCHEME,
        //                                   obs.ADMIN_IS_ENQUIRY,
        //                                   obs.IMS_PROPOSAL_TYPE,
        //                                   obs.IMS_ISLABUPLOADED,
        //                                   obs.IMS_PR_ROAD_CODE
        //                               }).Distinct().ToList();

        //        //using (System.IO.StreamWriter sw = System.IO.File.AppendText(System.IO.Path.Combine("D:\\ErrorLog\\1QMErrorLog_" + System.DateTime.Now.Date.ToString("dd_MM_yyyy") + ".txt", "")))
        //        //{
        //        //    sw.WriteLine("---------------------------------------------------------------------------------------");
        //        //    sw.WriteLine("AT setp 3 ");
        //        //    sw.WriteLine("---------------------------------------------------------------------------------------");
        //        //    sw.Close();
        //        //}

        //        List<string> rdCodeList = new List<string>();
        //        List<int> obsCodeList = new List<int>();
        //        int matchCnt = 0;


        //        foreach (var item in distinctObsList)
        //        {
        //            QMObsATRModel obsModel = new QMObsATRModel();
        //            obsModel.QM_OBSERVATION_ID = item.QM_OBSERVATION_ID;
        //            obsModel.MONITOR_NAME = item.MONITOR_NAME;
        //            obsModel.STATE_NAME = item.STATE_NAME;
        //            obsModel.DISTRICT_NAME = item.DISTRICT_NAME;
        //            obsModel.BLOCK_NAME = item.BLOCK_NAME;
        //            obsModel.IMS_PACKAGE_ID = item.IMS_PACKAGE_ID;
        //            obsModel.IMS_YEAR = item.IMS_YEAR.ToString() + "-" + (item.IMS_YEAR + 1).ToString();
        //            obsModel.IMS_ROAD_NAME = item.IMS_ROAD_NAME;
        //            obsModel.QM_INSPECTED_START_CHAINAGE = item.QM_INSPECTED_START_CHAINAGE;
        //            obsModel.QM_INSPECTED_END_CHAINAGE = item.QM_INSPECTED_END_CHAINAGE;
        //            obsModel.QM_INSPECTED_END_CHAINAGE_BRIDGE = item.QM_INSPECTED_END_CHAINAGE_BRIDGE;
        //            obsModel.QM_INSPECTION_DATE = item.QM_INSPECTION_DATE;

        //            obsModel.VIEW_INSPECTION_REPORT_LINK =
        //            (dbContext.QUALITY_INSPECTION_REPORT_FILE.Where(c => c.QM_OBSERVATION_ID == item.QM_OBSERVATION_ID).Any())
        //                                ? "<a href='#' title='Click here to view details' class='ui-icon ui-icon-zoomin ui-align-center' onClick='ViewInspectionReportATR(\"" + item.QM_OBSERVATION_ID.ToString().Trim() + "\"); return false;'>View</a>"
        //                                : (PMGSYSession.Current.RoleCode == 8 || PMGSYSession.Current.RoleCode == 9)
        //                                    ? "<a href='#' title='No files uploaded yet'><center>_</center></a>"
        //                                    : "NA";

        //            obsModel.WORK_LENGTH = item.WORK_LENGTH;
        //            obsModel.IMS_ISCOMPLETED = item.IMS_ISCOMPLETED;
        //            obsModel.PMGSY_SCHEME = item.PMGSY_SCHEME;
        //            obsModel.OVERALL_GRADE = item.OVERALL_GRADE;
        //            obsModel.NO_OF_PHOTO_UPLOADED = item.NO_OF_PHOTO_UPLOADED;
        //            obsModel.QM_ATR_STATUS = item.QM_ATR_STATUS;
        //            obsModel.ADMIN_IS_ENQUIRY = item.ADMIN_IS_ENQUIRY.Equals("Y") ? "Yes" : "No";
        //            obsModel.IMS_PROPOSAL_TYPE = item.IMS_PROPOSAL_TYPE.Equals("P") ? "Road" : "LSB";
        //            obsModel.SHOW_OBS_LINK = "<a href='#' title='Click here to view details' class='ui-icon ui-icon-zoomin ui-align-center' onClick='ShowATRObsDetails(\"" + item.QM_OBSERVATION_ID.ToString().Trim() + "\"); return false;'>View</a>";
        //            obsModel.IMS_ISLABUPLOADED = ((item.IMS_ISLABUPLOADED.Trim().Equals("Yes"))
        //                                             ?
        //                                            "<a href='#' title='Click here to view uploaded lab photograph against this package' class='ui-icon ui-icon-zoomin ui-align-center' onClick='ShowLabDetails(\"" + dbContext.QUALITY_QM_LAB_MASTER.Where(x => x.IMS_PACKAGE_ID.Equals(item.IMS_PACKAGE_ID)).FirstOrDefault().QM_LAB_ID + "\"); return false;'>Lab Details</a>"
        //                                            :
        //                                            "<a href='#' title='Lab photograph is not available' class='ui-icon ui-icon-locked ui-align-center' />"); //Added by Deendayal 
        //            atrDetailsModel.OBS_LIST.Add(obsModel);
        //        }

        //        //using (System.IO.StreamWriter sw = System.IO.File.AppendText(System.IO.Path.Combine("D:\\ErrorLog\\1QMErrorLog_" + System.DateTime.Now.Date.ToString("dd_MM_yyyy") + ".txt", "")))
        //        //{
        //        //    sw.WriteLine("---------------------------------------------------------------------------------------");
        //        //    sw.WriteLine("AT setp 4 ");
        //        //    sw.WriteLine("---------------------------------------------------------------------------------------");
        //        //    sw.Close();
        //        //}


        //        #region Changes by SAMMED A. PATIL on 08JAN2018 for uploading missing ATR files
        //        string PhysicalPath = ConfigurationManager.AppSettings["QUALITY_INSPECTION_FILE_UPLOAD_ATR"];
        //        //For self Reference
        //        System.IO.File.Exists(Path.Combine(PhysicalPath, HttpUtility.UrlEncode("2636.pdf")).ToString().Replace(@"\\", @"//").Replace(@"\", @"/"));
        //        #endregion

        //        //using (System.IO.StreamWriter sw = System.IO.File.AppendText(System.IO.Path.Combine("D:\\ErrorLog\\1QMErrorLog_" + System.DateTime.Now.Date.ToString("dd_MM_yyyy") + ".txt", "")))
        //        //{
        //        //    sw.WriteLine("---------------------------------------------------------------------------------------");
        //        //    sw.WriteLine("AT setp 5 ");
        //        //    sw.WriteLine("---------------------------------------------------------------------------------------");
        //        //    sw.Close();
        //        //}


        //        //int index = 0;
        //        foreach (var item in atrDetailsModel.OBS_ATR_LIST)
        //        {
        //            // Populate & Add ATR Details for each Observation Id
        //            QMATRModel atrModel = new QMATRModel();
        //            atrModel.QM_OBSERVATION_ID = item.QM_OBSERVATION_ID;
        //            atrModel.QM_ATR_ID = item.QM_ATR_ID;
        //            atrModel.ATR_ENTRY_DATE = item.ATR_ENTRY_DATE;
        //            atrModel.ATR_REGRADE_STATUS = item.ATR_REGRADE_STATUS.Trim().Equals("N") ? "No" : "Yes";
        //            atrModel.ATR_REGRADE_REMARKS = item.ATR_REGRADE_REMARKS;
        //            atrModel.ATR_REGRADE_DATE = item.ATR_REGRADE_DATE;
        //            atrModel.ATR_IS_DELETED = item.ATR_IS_DELETED;
        //            atrModel.QM_ATR_STATUS = item.QM_ATR_STATUS;

        //            atrModel.IS_SUBMITTED = item.QM_ATR_STATUS.Trim().Equals("N") ? "No" : "Yes";
        //            atrModel.ATR_UPLOAD_VIEW_LINK =
        //                (PMGSYSession.Current.RoleCode == 22 || PMGSYSession.Current.RoleCode == 38 || PMGSYSession.Current.RoleCode == 5 || PMGSYSession.Current.RoleCode == 54)//for PIU or PIURCPLWE view only 
        //                ? (item.QM_ATR_ID != null)
        //                            ? item.ATR_IS_DELETED == "Y" ? "--" : (PMGSYSession.Current.RoleCode == 5 && item.ATR_IS_DELETED == "N" && !(System.IO.File.Exists(Path.Combine(PhysicalPath, HttpUtility.UrlEncode(item.QM_ATR_ID + ".pdf")).ToString().Replace(@"\\", @"//").Replace(@"\", @"/"))))
        //                                ? "File Not Available"
        //                                    : "<a href='#' title='Click here to view uploaded ATR' class='ui-icon ui-icon-zoomin ui-align-center' onClick='DownloadATR(\"" + URLEncrypt.EncryptParameters(new string[] { item.QM_ATR_ID + ".pdf" + "$" + item.QM_ATR_ID }) + "\"); return false;'>Download</a>"
        //                                        : "<a href='#' class='ui-icon ui-icon-locked ui-align-center' />"
        //                                : (item.QM_ATR_STATUS.Trim().Equals("N")) // Upload/View
        //                                ? (item.IMS_ISLABUPLOADED.Equals("Yes") || qmBAL.GetCurrentworkStatus(item.IMS_PR_ROAD_CODE).Equals("C") || qmBAL.GetCurrentworkStatus(item.IMS_PR_ROAD_CODE).Equals("X") || qmBAL.GetCurrentworkStatus(item.IMS_PR_ROAD_CODE).Equals("")
        //                                    ?
        //                //DO NOT DELETE BELOW COMMENTED CODE.
        //                //Commented out for a temporary period to enable ATR upload option
        //                //below both conditions added by abhinav pathak on 18-08-2019
        //                //(dbContext.QUALITY_QM_INSPECTION_FILES.Where(obs => obs.QM_OBSERVATION_ID == item.QM_OBSERVATION_ID && obs.QM_FILES_FINALIZED == "Y").ToList().SequenceEqual(dbContext.QUALITY_QM_INSPECTION_FILES.Where
        //                //(obj => obj.QM_OBSERVATION_ID == item.QM_OBSERVATION_ID).ToList())
        //                //&& dbContext.QUALITY_QM_INSPECTION_FILES.Any(obj => obj.QM_OBSERVATION_ID == item.QM_OBSERVATION_ID))
        //                //?
        //                                        "<a href='#' title='Click here to upload ATR' class='ui-icon ui-icon-plusthick ui-align-center' onClick='UploadATR(\"" + item.QM_OBSERVATION_ID.ToString().Trim() + "$" + Convert.ToString(item.QM_ATR_ID).Trim() + "\"); return false;'>Upload</a>" //: "-"
        //                                      : "<a href='#' title='Lab photos are not available' class='ui-icon ui-icon-locked ui-align-center' />")
        //                                    : (item.QM_ATR_ID != null)
        //                                    ? item.ATR_IS_DELETED == "Y" ? "--"
        //                                    : !(System.IO.File.Exists(Path.Combine(PhysicalPath, HttpUtility.UrlEncode(item.QM_ATR_ID + ".pdf")).ToString().Replace(@"\\", @"//").Replace(@"\", @"/")))
        //                                        ? "<a href='#' title='Click here to upload missing ATR File' class='ui-icon ui-icon-plusthick ui-align-center' onClick='UploadMissingATR(\"" + item.QM_OBSERVATION_ID.ToString().Trim() + "$" + Convert.ToString(item.QM_ATR_ID).Trim() + "\"); return false;'>Upload</a>"
        //                                        : "<a href='#' title='Click here to view uploaded ATR' class='ui-icon ui-icon-zoomin ui-align-center' onClick='DownloadATR(\"" + URLEncrypt.EncryptParameters(new string[] { item.QM_ATR_ID + ".pdf" + "$" + item.QM_ATR_ID }) + "\"); return false;'>Download</a>"
        //                                    : "<a href='#' class='ui-icon ui-icon-locked ui-align-center' />";

        //            atrModel.ATR_ACCEPTANCE_LINK =
        //                 (PMGSYSession.Current.RoleCode == 22 || PMGSYSession.Current.RoleCode == 38 || PMGSYSession.Current.RoleCode == 5 || PMGSYSession.Current.RoleCode == 54)//for PIU or PIURCPLWE view only
        //                 ? item.ATR_REGRADE_STATUS.Trim().Equals("A")
        //                                ? "Accepted"
        //                                : item.ATR_REGRADE_STATUS.Trim().Equals("R")
        //                                    ? "Rejected"
        //                                    : item.ATR_REGRADE_STATUS.Equals("V")
        //                                        ? "To be Verified"
        //                                        : item.ATR_REGRADE_STATUS.Equals("D")
        //                                            ? "Non Rectifible Deffect"
        //                                            : ""
        //                 : item.ATR_REGRADE_STATUS.Trim().Equals("A")     // Acceptance
        //                                ? "Accepted"
        //                                : item.ATR_REGRADE_STATUS.Equals("V")
        //                                    ? "To be Verified"
        //                                    : item.ATR_REGRADE_STATUS.Equals("D")
        //                                        ? "Non Rectifiable Deffect"
        //                //: (item.ATR_REGRADE_STATUS.Equals("C") && PMGSYSession.Current.RoleCode == 5) // Changes done on 13-04-2015 as per given ECR by Aanad. 
        //                                        : (item.ATR_REGRADE_STATUS.Equals("C") && PMGSYSession.Current.RoleCode == 9)
        //                                            ? "Technical Committee" + // If Tech Committee, then append + sign to upload again
        //                                                      (atrModel.QM_ATR_ID == (dbContext.QUALITY_ATR_FILE.Where(c => c.QM_OBSERVATION_ID == atrModel.QM_OBSERVATION_ID).Select(c => c.QM_ATR_ID).Max())
        //                                                         ? "<a href='#' title='Click here to upload ATR' class='ui-icon ui-icon-plusthick ui-align-center' onClick='UploadATR(\"" + item.QM_OBSERVATION_ID.ToString().Trim() + "$" + Convert.ToString(item.QM_ATR_ID).Trim() + "\"); return false;'>Upload</a>"
        //                                                         : "")
        //                                            : item.ATR_REGRADE_STATUS.Trim().Equals("R") // if any of the ATR against Obs Id is Accepted then dont provide link to upload.
        //                                                ? item.QM_ATR_STATUS.Equals("A")
        //                                                    ? "Rejected"
        //                                                    : "Rejected" + //If Rejected atr is last against Observation, then append + sign to upload again
        //                                                      (atrModel.QM_ATR_ID == (dbContext.QUALITY_ATR_FILE.Where(c => c.QM_OBSERVATION_ID == atrModel.QM_OBSERVATION_ID).Select(c => c.QM_ATR_ID).Max())
        //                                                         ? "<a href='#' title='Click here to upload ATR' class='ui-icon ui-icon-plusthick ui-align-center' onClick='UploadATR(\"" + item.QM_OBSERVATION_ID.ToString().Trim() + "$" + Convert.ToString(item.QM_ATR_ID).Trim() + "\"); return false;'>Upload</a>"
        //                                                         : "")
        //                                                : "";

        //            atrModel.ATR_REGRADE_LINK =
        //                (PMGSYSession.Current.RoleCode == 22 || PMGSYSession.Current.RoleCode == 38 || PMGSYSession.Current.RoleCode == 5 || PMGSYSession.Current.RoleCode == 54)//for PIU or PIURCPLWE view only
        //                ? (item.ATR_REGRADE_STATUS.Trim().Equals("A"))
        //                    ? "<a href='#' title='Click here to see observation details against ATR' class='ui-icon ui-icon-zoomin ui-align-center' onClick='ShowATRGradingDetails(\"" + item.QM_OBSERVATION_ID.ToString().Trim() + "\"); return false;'>Regrade</a>"
        //                    : "<a href='#' class='ui-icon ui-icon-locked ui-align-center' />"
        //                : ((item.ATR_REGRADE_STATUS.Trim().Equals("U") || item.ATR_REGRADE_STATUS.Trim().Equals("V")) && item.ATR_IS_DELETED.Equals("N")) // Regrade, for recent entry only
        //                    ? "<a href='#' title='Click here to regrade ATR' class='ui-icon ui-icon-plusthick ui-align-center' onClick='RegradeATR(\"" + item.QM_OBSERVATION_ID.ToString().Trim() + "\"); return false;'>Regrade</a>"
        //                    : (item.ATR_REGRADE_STATUS.Trim().Equals("A"))
        //                        ? "<a href='#' title='Click here to see observation details against ATR' class='ui-icon ui-icon-zoomin ui-align-center' onClick='ShowATRGradingDetails(\"" + item.QM_OBSERVATION_ID.ToString().Trim() + "\"); return false;'>Regrade</a>"
        //                        : "<a href='#' class='ui-icon ui-icon-locked ui-align-center' />";

        //            atrModel.ATR_DELETE_LINK =
        //                 (PMGSYSession.Current.RoleCode == 22 || PMGSYSession.Current.RoleCode == 38 || PMGSYSession.Current.RoleCode == 5 || PMGSYSession.Current.RoleCode == 54)//for PIU or PIURCPLWE view only
        //                 ? "<a href='#' class='ui-icon ui-icon-locked ui-align-center' />"
        //                 : (item.QM_ATR_ID != null && item.QM_ATR_STATUS.Trim() != "A" && item.QM_ATR_STATUS.Trim() != "N" && (item.QM_ATR_ID == dbContext.QUALITY_ATR_FILE.Where(c => c.QM_OBSERVATION_ID == item.QM_OBSERVATION_ID).Select(c => c.QM_ATR_ID).Max()))
        //                        ? "<a href='#' title='Click here to delete ATR details' class='ui-icon ui-icon-trash ui-align-center' onClick='DeleteATR(\"" +
        //                            item.QM_OBSERVATION_ID.ToString().Trim() + "\",\"" + dbContext.QUALITY_ATR_FILE.Where(a => a.QM_OBSERVATION_ID == item.QM_OBSERVATION_ID && a.ATR_IS_DELETED == "N").Select(a => a.QM_ATR_ID).FirstOrDefault()
        //                            + "\"); return false;'>Delete</a>"
        //                        : "<a href='#' class='ui-icon ui-icon-locked ui-align-center' />";

        //            atrDetailsModel.ATR_LIST.Add(atrModel);

        //            //index++; // increment index for each record.
        //        }
        //        //using (System.IO.StreamWriter sw = System.IO.File.AppendText(System.IO.Path.Combine("D:\\ErrorLog\\1QMErrorLog_" + System.DateTime.Now.Date.ToString("dd_MM_yyyy") + ".txt", "")))
        //        //{
        //        //    sw.WriteLine("---------------------------------------------------------------------------------------");
        //        //    sw.WriteLine("AT setp 6 ");
        //        //    sw.WriteLine("---------------------------------------------------------------------------------------");
        //        //    sw.Close();
        //        //}

        //        int count = rdCodeList.Count();

        //        return View(atrDetailsModel);
        //    }
        //    catch (Exception ex)
        //    {
        //        //using (System.IO.StreamWriter sw = System.IO.File.AppendText(System.IO.Path.Combine("D:\\ErrorLog\\1QMErrorLog_" + System.DateTime.Now.Date.ToString("dd_MM_yyyy") + ".txt", "")))
        //        //{
        //        //    sw.WriteLine("---------------------------------------------------------------------------------------");
        //        //    sw.WriteLine("AT setp  7  " + ex.Message.ToString()  );
        //        //    sw.WriteLine("---------------------------------------------------------------------------------------");
        //        //    sw.Close();
        //        //}

        //        ErrorLog.LogError(ex, "QualityMonitoring.ATRDetails()");
        //        atrDetailsModel.ERROR = "Error occurred while processing your request";
        //        return View(atrDetailsModel); //return model as null

        //    }
        //    finally
        //    {
        //        dbContext.Dispose();
        //    }
        //}


        /// <summary>
        /// Render View to upload ATR Pdfs
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Audit]
        public ActionResult PdfFileUpload(string id)
        {
            Int32 QM_OBSERVATION_ID = 0, QM_ATR_ID = 0;
            dbContext = new PMGSYEntities();
            try
            {
                PMGSY.Models.QualityMonitoring.FileUploadViewModel fileUploadViewModel = new PMGSY.Models.QualityMonitoring.FileUploadViewModel();
                if (id.Contains('$'))
                {
                    QM_OBSERVATION_ID = Convert.ToInt32(id.Trim().Split('$')[0]);
                    QM_ATR_ID = string.IsNullOrEmpty(id.Trim().Split('$')[1]) ? 0 : Convert.ToInt32(id.Trim().Split('$')[1]);
                }
                else
                {
                    QM_OBSERVATION_ID = Convert.ToInt32(id);
                }
                fileUploadViewModel.QM_OBSERVATION_ID = QM_OBSERVATION_ID;
                fileUploadViewModel.QM_ATR_ID = QM_ATR_ID;

                fileUploadViewModel.ErrorMessage = string.Empty;

                if (dbContext.QUALITY_ATR_FILE.Where(a => a.QM_OBSERVATION_ID == QM_OBSERVATION_ID).Any())
                {
                    var maxAtrId = dbContext.QUALITY_ATR_FILE.Where(a => a.QM_OBSERVATION_ID == QM_OBSERVATION_ID).Select(a => a.QM_ATR_ID).Max();
                    var maxRecordGrade = dbContext.QUALITY_ATR_FILE.Where(a => a.QM_OBSERVATION_ID == QM_OBSERVATION_ID
                                                                         && a.QM_ATR_ID == maxAtrId
                        ///Changes by DEENDAYAL SHARMA for Uploading of Deleted ATRs
                        /*&& a.ATR_IS_DELETED == "N"*/).Select(a => a.ATR_REGRADE_STATUS).FirstOrDefault();
                    if (maxRecordGrade != null && (maxRecordGrade.Equals("R") || maxRecordGrade.Equals("C")))
                    {
                        fileUploadViewModel.NumberofPdfs = 0;
                    }
                    else
                    {
                        fileUploadViewModel.NumberofPdfs = dbContext.QUALITY_ATR_FILE.Where(a => a.QM_OBSERVATION_ID == QM_OBSERVATION_ID && a.ATR_IS_DELETED == "N").Count();
                    }
                }
                else
                {
                    fileUploadViewModel.NumberofPdfs = 0;
                }
                return View("ATRUpload", fileUploadViewModel);
            }
            catch (Exception ex)
            {
                //Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                ErrorLog.LogError(ex, "QualityMonitoring.PdfFileUpload(string id)");
                return Json(string.Empty, JsonRequestBehavior.AllowGet);
            }
            finally
            {
                dbContext.Dispose();
            }
        }


        #region New Region For Merging ATR Upload and 'Mark SQM inspection for ATR Verification'

        //Below code added on 30-01-2023
        public void CheckInspAvailableCount(int roadCode, string inspectionDate, out long totalrecords)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            string inspDate = DateTime.Parse(inspectionDate).ToShortDateString();
            var splittedDate = inspDate.Split('/');
            DateTime? qmInspDate = new DateTime(int.Parse(splittedDate[2]), int.Parse(splittedDate[1]), int.Parse(splittedDate[0]));

            try
            {

                // If Role Not CQC
                if (PMGSYSession.Current.RoleCode != 9)
                {
                    var list1 = (from obsm in dbContext.QUALITY_QM_OBSERVATION_MASTER
                                 join isp in dbContext.IMS_SANCTIONED_PROJECTS on obsm.IMS_PR_ROAD_CODE equals isp.IMS_PR_ROAD_CODE
                                 join qmschedule in dbContext.QUALITY_QM_SCHEDULE on obsm.ADMIN_SCHEDULE_CODE equals qmschedule.ADMIN_SCHEDULE_CODE
                                 join aqm in dbContext.ADMIN_QUALITY_MONITORS on qmschedule.ADMIN_QM_CODE equals aqm.ADMIN_QM_CODE
                                 join qmscheduleDetail in dbContext.QUALITY_QM_SCHEDULE_DETAILS on qmschedule.ADMIN_SCHEDULE_CODE equals qmscheduleDetail.ADMIN_SCHEDULE_CODE
                                 where isp.IMS_PR_ROAD_CODE == roadCode && obsm.IMS_PR_ROAD_CODE == roadCode &&
                                 aqm.ADMIN_QM_TYPE == "S" &&
                                // (obsm.QM_OVERALL_GRADE == 1 || obsm.QM_OVERALL_GRADE == 2 || obsm.QM_OVERALL_GRADE == 3) &&
                                (obsm.QM_OVERALL_GRADE == 1 ) &&
                                 isp.MAST_STATE_CODE == isp.MAST_STATE_CODE &&
                                 obsm.QM_INSPECTION_DATE >= qmInspDate &&
                                 isp.IMS_PR_ROAD_CODE == qmscheduleDetail.IMS_PR_ROAD_CODE
                                 select new
                                 {
                                     obsm.QM_OBSERVATION_ID,
                                     qmschedule.ADMIN_QM_CODE,
                                     isp.MAST_STATE_CODE,
                                     isp.MAST_DISTRICT_CODE,
                                     isp.MAST_BLOCK_CODE,
                                     isp.IMS_PACKAGE_ID,
                                     IMS_YEAR1 = isp.IMS_YEAR,
                                     IMS_YEAR2 = isp.IMS_YEAR + 1,
                                     isp.IMS_ROAD_NAME,
                                     isp.IMS_PROPOSAL_TYPE,
                                     obsm.QM_INSPECTED_START_CHAINAGE,
                                     obsm.QM_INSPECTED_END_CHAINAGE,
                                     obsm.QM_INSPECTION_DATE,
                                     isp.IMS_BRIDGE_LENGTH,
                                     isp.IMS_PAV_LENGTH,
                                     isp.IMS_ISCOMPLETED,
                                     isp.MAST_PMGSY_SCHEME,
                                     obsm.QM_OVERALL_GRADE,
                                     isp.IMS_PR_ROAD_CODE,
                                     aqm.ADMIN_QM_TYPE,
                                     aqm.ADMIN_QM_LNAME,
                                     aqm.ADMIN_QM_MNAME,
                                     aqm.ADMIN_QM_FNAME,
                                     qmscheduleDetail.ADMIN_IS_ENQUIRY
                                 }).OrderBy(s => s.QM_OBSERVATION_ID);

                    totalrecords = list1.Count();
                }
                else
                {
                    totalrecords = 0;
                }
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "QualityMonitoringController.CheckInspAvailableCount");
                totalrecords = 0;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        //Below Code Added on 20-03-2023
        public void CheckRejectFlagStatus(int QM_OBSERVATION_ID, out bool rejectLockStatus)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            try
            {
                rejectLockStatus = dbContext.QUALITY_ATR_FILE.Where(x => x.QM_OBSERVATION_ID == QM_OBSERVATION_ID).Where(x => x.ATR_REGRADE_STATUS == "C" || x.ATR_REGRADE_STATUS == "D" || x.ATR_REGRADE_STATUS == "V").Any() ? true : false;

            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "QualityMonitoring.CheckRejectFlagStatus(item.QM_OBSERVATION_ID, out bool rejectLockStatus)");
                rejectLockStatus = true;
                throw;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        //Below Code Added on 30-01-2023

        [HttpGet]
        [Audit]
        public ActionResult PdfFileUploadATR(string id)
        {
            Int32 QM_OBSERVATION_ID = 0, QM_ATR_ID = 0;

            //Below fields added on 30-01-2023
            int roadCode = Convert.ToInt32(id.Trim().Split('$')[3]);
            string inspectionDate = (id.Split('$')[2] != null || id.Split('$')[2] != string.Empty) ? id.Split('$')[2].Replace("+", " ") : id.Split('$')[2];
            bool hideAddATRFileButton = false;//false=show Add ATR file button, true=hide Add ATR file button 
            bool hideATRFileList = false;

            dbContext = new PMGSYEntities();
            try
            {
                PMGSY.Models.QualityMonitoring.FileUploadViewModel fileUploadViewModel = new PMGSY.Models.QualityMonitoring.FileUploadViewModel();
                if (id.Contains('$'))
                {
                    //1)if sqc is visiting the Mark for ATR verification screen from (+) icon below rejected status , then  QM_ATR_Id = QM_ATR_Id of the rejected record 
                    //2)For New Entry of Mark for ATR Verification QM_ATR_ID=0
                    //3)If ATR is already uploaded but inspection is not mark , then QM_ATR_ID=uploaded file atr id

                    QM_OBSERVATION_ID = Convert.ToInt32(id.Trim().Split('$')[0]);
                    QM_ATR_ID = string.IsNullOrEmpty(id.Trim().Split('$')[1]) ? 0 : Convert.ToInt32(id.Trim().Split('$')[1]);
                    //flagAddATRFileButton = QM_ATR_ID != 0
                    //                       ?
                    //                           (dbContext.QUALITY_ATR_FILE.Where(x => x.QM_ATR_ID == QM_ATR_ID).Select(x => x.ATR_VERIFICATION_FINALIZED).FirstOrDefault() != "Y") ? true : false
                    //                       : false;

                    hideAddATRFileButton = QM_ATR_ID != 0
                                           ?
                                            dbContext.QUALITY_ATR_FILE.Where(x => x.QM_ATR_ID == QM_ATR_ID).Select(x => x.ATR_REGRADE_STATUS).FirstOrDefault() == "R"
                                                ?
                                                    false
                                                :
                                               (dbContext.QUALITY_ATR_FILE.Where(x => x.QM_ATR_ID == QM_ATR_ID).Select(x => x.ATR_VERIFICATION_FINALIZED).FirstOrDefault() != "Y" ? true : false)
                                           : false;

                    hideATRFileList = QM_ATR_ID != 0
                                           ?
                                               dbContext.QUALITY_ATR_FILE.Where(x => x.QM_ATR_ID == QM_ATR_ID).Select(x => x.ATR_REGRADE_STATUS).FirstOrDefault() == "R"
                                               ? true
                                               : false
                                           : true;

                    ViewBag.ShowAddATRFileButton = hideAddATRFileButton;
                    ViewBag.HideATRFileList = hideATRFileList;//
                }
                else
                {
                    QM_OBSERVATION_ID = Convert.ToInt32(id);
                }
                fileUploadViewModel.QM_OBSERVATION_ID = QM_OBSERVATION_ID;
                fileUploadViewModel.QM_ATR_ID = QM_ATR_ID;


                //Below fields added on 30-01-2023
                fileUploadViewModel.IMS_PR_ROAD_CODE = roadCode;
                fileUploadViewModel.QM_INSPECTION_DATE = inspectionDate;

                ////-------------------Added by Shreyas on 02-05-2023 -------------------//If Eform OR Insp.Report is not uploaded then ATR upload provision will not be available
                //int scheduleCode = Convert.ToInt32(dbContext.QUALITY_QM_OBSERVATION_MASTER.Where(s => s.QM_OBSERVATION_ID == QM_OBSERVATION_ID).Select(s => s.ADMIN_SCHEDULE_CODE).FirstOrDefault());
                //var EformData = dbContext.EFORM_PDF_UPLOAD_DETAIL.
                //               Join(dbContext.EFORM_MASTER,
                //               pdf => pdf.EFORM_ID,
                //               em => em.EFORM_ID,
                //               (pdf, em) => new { PDF = pdf, EM = em })
                //               .Where(x => x.PDF.USER_TYPE == "Q" && x.PDF.IS_FINALISED == "Y" && x.EM.IS_VALID == "Y" && x.EM.IMS_PR_ROAD_CODE == roadCode && x.EM.ADMIN_SCHEDULE_CODE == scheduleCode).Any();

                //var InspReportData = dbContext.QUALITY_INSPECTION_REPORT_FILE.Where(s => s.QM_OBSERVATION_ID == QM_OBSERVATION_ID).Any();

                //if (EformData == false && InspReportData == false)
                //{
                //    return Json(new { success = false, message = "Eform or Inspection report is not uploaded for selected inspection." }, JsonRequestBehavior.AllowGet);
                //}
                ////---------------

                fileUploadViewModel.ErrorMessage = string.Empty;

                if (dbContext.QUALITY_ATR_FILE.Where(a => a.QM_OBSERVATION_ID == QM_OBSERVATION_ID).Any())
                {
                    var maxAtrId = dbContext.QUALITY_ATR_FILE.Where(a => a.QM_OBSERVATION_ID == QM_OBSERVATION_ID).Select(a => a.QM_ATR_ID).Max();
                    var maxRecordGrade = dbContext.QUALITY_ATR_FILE.Where(a => a.QM_OBSERVATION_ID == QM_OBSERVATION_ID
                                                                         && a.QM_ATR_ID == maxAtrId
                        ///Changes by DEENDAYAL SHARMA for Uploading of Deleted ATRs
                        /*&& a.ATR_IS_DELETED == "N"*/).Select(a => a.ATR_REGRADE_STATUS).FirstOrDefault();
                    if (maxRecordGrade != null && (maxRecordGrade.Equals("R") || maxRecordGrade.Equals("C")))
                    {
                        fileUploadViewModel.NumberofPdfs = 0;
                    }
                    else
                    {
                        fileUploadViewModel.NumberofPdfs = dbContext.QUALITY_ATR_FILE.Where(a => a.QM_OBSERVATION_ID == QM_OBSERVATION_ID && a.ATR_IS_DELETED == "N").Count();
                    }
                }
                else
                {
                    fileUploadViewModel.NumberofPdfs = 0;
                }
                return View("ATRFileUpload", fileUploadViewModel);
            }
            catch (Exception ex)
            {
                //Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                ErrorLog.LogError(ex, "QualityMonitoring.PdfFileUploadATR(string id)");
                return Json(string.Empty, JsonRequestBehavior.AllowGet);
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        //Below Code Added on 30-01-2023
        [HttpPost]
        [Audit]
        public string PdfFileUploadATR(FormCollection formData, HttpRequestBase Request)
        {
            dbContext = new PMGSYEntities();
            qualityBAL = new QualityMonitoringBAL();
            string filePath = string.Empty;
            try
            {

                if (PMGSYSession.Current.RoleCode == 22 || PMGSYSession.Current.RoleCode == 38)
                {
                    filePath = ConfigurationManager.AppSettings["QUALITY_INSPECTION_FILE_UPLOAD_MAINTENANCE_ATR"];
                }
                else
                {
                    filePath = ConfigurationManager.AppSettings["QUALITY_INSPECTION_FILE_UPLOAD_ATR"];
                }

                CommonFunctions objCommonFunc = new CommonFunctions();
                if (!(objCommonFunc.ValidateIsPdf(filePath, Request)))
                {
                    //fileUploadViewModel.ErrorMessage = "File Type is Not Allowed.";
                    //return View("PdfFileUpload", fileUploadViewModel.ErrorMessage);
                    //ErrorMessage = "File Type is Not Allowed.";
                    return "File Type is Not Allowed.";
                }

                foreach (string file in Request.Files)
                {
                    string status = new ProposalBAL().ValidatePDFFile(Request.Files[0].ContentLength, Path.GetExtension(Request.Files[0].FileName));
                    if (status != string.Empty)
                    {
                        //fileUploadViewModel.ErrorMessage = status;
                        //return View("PdfFileUpload", fileUploadViewModel.ErrorMessage);
                        //ErrorMessage = status;
                        return status;
                    }
                }

                //var fileData = new List<PMGSY.Models.QualityMonitoring.FileUploadViewModel>();

                //Int32 QM_OBSERVATION_ID = 0;

                //if (fileUploadViewModel.QM_OBSERVATION_ID != 0)
                //{
                //    QM_OBSERVATION_ID = fileUploadViewModel.QM_OBSERVATION_ID;
                //}
                //else
                //{
                //    try
                //    {
                //        QM_OBSERVATION_ID = Convert.ToInt32(Request["QM_OBSERVATION_ID"]);
                //    }
                //    catch (Exception ex)
                //    {
                //        ErrorLog.LogError(ex, "PMGSY.Models.QualityMonitoring.PdfFileUpload.PdfFileUpload");
                //        Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                //        if (Request["QM_OBSERVATION_ID"].Contains(','))
                //        {
                //            QM_OBSERVATION_ID = Convert.ToInt32(Request["QM_OBSERVATION_ID"].Split(',')[0]);
                //        }
                //        if (Request["ADMIN_SCHEDULE_CODE"].Contains(','))
                //        {
                //            QM_OBSERVATION_ID = Convert.ToInt32(Request["QM_OBSERVATION_ID"].Split(',')[0]);
                //        }
                //    }
                //}

                //foreach (string file in Request.Files)
                //{
                //    UploadPDFFile(Request, fileData, QM_OBSERVATION_ID);
                //}

                //if (dbContext.QUALITY_ATR_FILE.Where(a => a.QM_OBSERVATION_ID == QM_OBSERVATION_ID).Any())
                //{
                //    fileUploadViewModel.NumberofPdfs = dbContext.QUALITY_ATR_FILE.Where(a => a.QM_OBSERVATION_ID == QM_OBSERVATION_ID).Count();
                //}
                //else
                //{
                //    fileUploadViewModel.NumberofPdfs = 0;
                //}

                //var serializer = new JavaScriptSerializer();
                //serializer.MaxJsonLength = Int32.MaxValue;

                //var result = new ContentResult
                //{
                //    Content = "{\"files\":" + serializer.Serialize(fileData) + "}",
                //};
                //ErrorMessage = string.Empty;
                return string.Empty;
            }
            catch (Exception ex)
            {
                //Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                ErrorLog.LogError(ex, "QualityMonitoring.PdfFileUploadATR(FormCollection formData,HttpRequestBase Request,out string ErrorMessage)");
                //ErrorMessage = "Error in File Saving.";
                return "Error while Saving File.";
            }
            finally
            {
                dbContext.Dispose();
            }


        }

        //Below Code Added on 30-01-2023
        [HttpPost]
        public JsonResult savefinalizedMarkVerificationATRFile(FormCollection formData)
        {

            string message = null;
            try
            {
                //string[] tempVerificationATRcode = nqmobsidatrid.Split('$');

                //int SQM_ObservationId = submitarray[0]; //Convert.ToInt32(Request.Params["submitSQMId"]);
                //int NQM_ObservationId = Convert.ToInt32(tempVerificationATRcode[2]);
                //int NQM_ATRid = Convert.ToInt32(tempVerificationATRcode[3]);



                //else if (count > 0 && missFileFlag.Equals("N"))
                //{
                //    //PdfFileUpload(formData);
                //}
                string[] tempVerificationATRcode = formData["nqmobsidatrid"].Split('$');

                int SQM_ObservationId = Convert.ToInt32(formData["submitarray"]); //Convert.ToInt32(Request.Params["submitSQMId"]);
                int NQM_ObservationId = Convert.ToInt32(tempVerificationATRcode[2]);
                int NQM_ATRid = Convert.ToInt32(tempVerificationATRcode[3]);

                //-------------------Added by Shreyas on 14-06-2023 -------------------//If Eform OR Insp.Report is not uploaded then ATR upload provision will not be available
                dbContext = new PMGSYEntities();
                var scheduleDetails = dbContext.QUALITY_QM_OBSERVATION_MASTER.Where(s => s.QM_OBSERVATION_ID == SQM_ObservationId).Select(s => new { s.ADMIN_SCHEDULE_CODE, s.IMS_PR_ROAD_CODE }).FirstOrDefault();
                var EformData = dbContext.EFORM_PDF_UPLOAD_DETAIL.
                               Join(dbContext.EFORM_MASTER,
                               pdf => pdf.EFORM_ID,
                               em => em.EFORM_ID,
                               (pdf, em) => new { PDF = pdf, EM = em })
                               .Where(x => x.PDF.USER_TYPE == "Q" && x.PDF.IS_FINALISED == "Y" && x.EM.IS_VALID == "Y" && x.EM.IMS_PR_ROAD_CODE == scheduleDetails.IMS_PR_ROAD_CODE && x.EM.ADMIN_SCHEDULE_CODE == scheduleDetails.ADMIN_SCHEDULE_CODE).Any();

                var InspReportData = dbContext.QUALITY_INSPECTION_REPORT_FILE.Where(s => s.QM_OBSERVATION_ID == SQM_ObservationId).Any();

                if (EformData == false && InspReportData == false)
                {
                    return Json(new { success = false, message = "Eform or Inspection report is not uploaded for selected inspection." }, JsonRequestBehavior.AllowGet);
                }
                //---------------


                #region File Upload 
                int count = Request.Files.Count;
                string missFileFlag = formData["missFileflag"];
                if (count > 0 && missFileFlag.Equals("N"))
                {
                    string ErrorMessage;
                    ErrorMessage = PdfFileUploadATR(formData, Request);
                    if (ErrorMessage != string.Empty && ErrorMessage != null)
                    {
                        return Json(new { success = false, message = ErrorMessage });
                    }
                }
                else
                {
                    dbContext = new PMGSYEntities();
                    //if file already uploaded and mark for atr verfication is pending , then atrRegradeStatus will not be rejected
                    //if file already uploaded but it is rejected by cqc , then atrRegradeStatus will be rejected

                    string atrRegradeStatus = dbContext.QUALITY_ATR_FILE.Where(x => x.QM_ATR_ID == NQM_ATRid).Any() ? dbContext.QUALITY_ATR_FILE.Where(x => x.QM_ATR_ID == NQM_ATRid).Select(x => x.ATR_REGRADE_STATUS).FirstOrDefault() : null;
                    if (!(dbContext.QUALITY_ATR_FILE.Where(x => x.QM_ATR_ID == NQM_ATRid).Any() && atrRegradeStatus != "R"))
                    {
                        return Json(new { success = false, message = "Please select ATR file." });
                    }

                }
                #endregion

                QualityMonitoringDAL qualityDAL = new QualityMonitoringDAL();

                //bool status = qualityDAL.savefinalizedMarkVerificationATRDAL(SQM_ObservationId, NQM_ObservationId, NQM_ATRid);
                bool status = qualityDAL.savefinalizedMarkVerificationATRFileDAL(SQM_ObservationId, NQM_ObservationId, NQM_ATRid, Request);
                if (status)
                    message = "Mark for ATR Verification Finalization Successfully";
                else
                    message = "ERROR !!  Changes are not saved.";

                return Json(new { success = status, message = message });
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "QualityMonitoringController.savefinalizedMarkVerificationATR()");
                return Json(new { success = false, message = "An error occured while processing your request." });
            }
        }


        #endregion

        /// <summary>
        /// List ATR Files
        /// </summary>
        /// <param name="formCollection"></param>
        /// <returns></returns>
        [Audit]
        public JsonResult ListPDFFiles(FormCollection formCollection)
        {
            dbContext = new PMGSYEntities();
            qualityBAL = new QualityMonitoringBAL();
            try
            {
                //Adde By Abhishek kamble 29-Apr-2014 start
                using (CommonFunctions commonFunction = new CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new GridParams(Convert.ToInt32(formCollection["page"]), Convert.ToInt32(formCollection["rows"]), formCollection["sidx"], formCollection["sord"], Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                    {
                        return null;
                    }
                }
                //Adde By Abhishek kamble 29-Apr-2014 end
                Int32 QM_OBSERVATION_ID = 0, QM_ATR_ID = 0;
                if (Request.Params["QM_OBSERVATION_ID"] != "0" && Request.Params["QM_OBSERVATION_ID"] != null && Request.Params["QM_OBSERVATION_ID"] != string.Empty)
                {
                    QM_OBSERVATION_ID = Convert.ToInt32(Request.Params["QM_OBSERVATION_ID"]);
                }
                if (Request.Params["QM_ATR_ID"] != "0" && Request.Params["QM_ATR_ID"] != null && Request.Params["QM_ATR_ID"] != string.Empty)
                {
                    QM_ATR_ID = Convert.ToInt32(Request.Params["QM_ATR_ID"]);
                }

                int totalRecords;
                var jsonData = new
                {
                    rows = qualityBAL.GetPDFFilesListBAL(Convert.ToInt32(formCollection["page"]) - 1, Convert.ToInt32(formCollection["rows"]), formCollection["sidx"], formCollection["sord"], out totalRecords, QM_OBSERVATION_ID, QM_ATR_ID),
                    total = totalRecords <= Convert.ToInt32(formCollection["rows"]) ? 1 : (totalRecords % Convert.ToInt32(formCollection["rows"]) == 0 ? totalRecords / Convert.ToInt32(formCollection["rows"]) : (totalRecords / Convert.ToInt32(formCollection["rows"]) + 1)),
                    page = Convert.ToInt32(formCollection["page"]),
                    records = totalRecords
                };
                return Json(jsonData);
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
        /// Post Method for Uploading PDF File
        /// </summary>
        /// <param name="fileUploadViewModel"></param>
        /// <returns></returns>
        [HttpPost]
        [Audit]
        public ActionResult PdfFileUpload(PMGSY.Models.QualityMonitoring.FileUploadViewModel fileUploadViewModel)
        {
            dbContext = new PMGSYEntities();
            qualityBAL = new QualityMonitoringBAL();
            string filePath = string.Empty;
            try
            {

                if (PMGSYSession.Current.RoleCode == 22 || PMGSYSession.Current.RoleCode == 38)
                {
                    filePath = ConfigurationManager.AppSettings["QUALITY_INSPECTION_FILE_UPLOAD_MAINTENANCE_ATR"];
                }
                else
                {
                    filePath = ConfigurationManager.AppSettings["QUALITY_INSPECTION_FILE_UPLOAD_ATR"];
                }

                CommonFunctions objCommonFunc = new CommonFunctions();
                if (!(objCommonFunc.ValidateIsPdf(filePath, Request)))
                {
                    fileUploadViewModel.ErrorMessage = "File Type is Not Allowed.";
                    return View("PdfFileUpload", fileUploadViewModel.ErrorMessage);
                }

                foreach (string file in Request.Files)
                {
                    string status = new ProposalBAL().ValidatePDFFile(Request.Files[0].ContentLength, Path.GetExtension(Request.Files[0].FileName));
                    if (status != string.Empty)
                    {
                        fileUploadViewModel.ErrorMessage = status;
                        return View("PdfFileUpload", fileUploadViewModel.ErrorMessage);
                    }
                }

                var fileData = new List<PMGSY.Models.QualityMonitoring.FileUploadViewModel>();

                Int32 QM_OBSERVATION_ID = 0;

                if (fileUploadViewModel.QM_OBSERVATION_ID != 0)
                {
                    QM_OBSERVATION_ID = fileUploadViewModel.QM_OBSERVATION_ID;
                }
                else
                {
                    try
                    {
                        QM_OBSERVATION_ID = Convert.ToInt32(Request["QM_OBSERVATION_ID"]);
                    }
                    catch (Exception ex)
                    {
                        ErrorLog.LogError(ex, "PMGSY.Models.QualityMonitoring.PdfFileUpload.PdfFileUpload");
                        Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                        if (Request["QM_OBSERVATION_ID"].Contains(','))
                        {
                            QM_OBSERVATION_ID = Convert.ToInt32(Request["QM_OBSERVATION_ID"].Split(',')[0]);
                        }
                        if (Request["ADMIN_SCHEDULE_CODE"].Contains(','))
                        {
                            QM_OBSERVATION_ID = Convert.ToInt32(Request["QM_OBSERVATION_ID"].Split(',')[0]);
                        }
                    }
                }

                foreach (string file in Request.Files)
                {
                    UploadPDFFile(Request, fileData, QM_OBSERVATION_ID);
                }

                if (dbContext.QUALITY_ATR_FILE.Where(a => a.QM_OBSERVATION_ID == QM_OBSERVATION_ID).Any())
                {
                    fileUploadViewModel.NumberofPdfs = dbContext.QUALITY_ATR_FILE.Where(a => a.QM_OBSERVATION_ID == QM_OBSERVATION_ID).Count();
                }
                else
                {
                    fileUploadViewModel.NumberofPdfs = 0;
                }

                var serializer = new JavaScriptSerializer();
                serializer.MaxJsonLength = Int32.MaxValue;

                var result = new ContentResult
                {
                    Content = "{\"files\":" + serializer.Serialize(fileData) + "}",
                };
                return result;
            }
            catch (Exception ex)
            {
                //Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                ErrorLog.LogError(ex, "QualityMonitoring.PdfFileUpload(PMGSY.Models.QualityMonitoring.FileUploadViewModel fileUploadViewModel)");
                return null;
            }
            finally
            {
                dbContext.Dispose();
            }


        }








        /// <summary>
        /// This Method Uploads PDF File
        /// </summary>
        /// <param name="request"></param>
        /// <param name="statuses"></param>
        /// <param name="IMS_PR_ROAD_CODE"></param>
        [Audit]
        public void UploadPDFFile(HttpRequestBase request, List<PMGSY.Models.QualityMonitoring.FileUploadViewModel> statuses, int QM_OBSERVATION_ID)
        {
            String StorageRoot = (PMGSYSession.Current.RoleCode == 22 || PMGSYSession.Current.RoleCode == 38) ? ConfigurationManager.AppSettings["QUALITY_INSPECTION_FILE_UPLOAD_MAINTENANCE_ATR"] : ConfigurationManager.AppSettings["QUALITY_INSPECTION_FILE_UPLOAD_ATR"];
            dbContext = new PMGSYEntities();
            qualityBAL = new QualityMonitoringBAL();
            int MaxCount = 0;
            try
            {
                for (int i = 0; i < request.Files.Count; i++)
                {
                    HttpPostedFileBase file = request.Files[i];
                    //var fileId = IMS_PR_ROAD_CODE;
                    MaxCount = dbContext.QUALITY_ATR_FILE.Select(c => c.QM_ATR_ID).Max();
                    var fileId = MaxCount + 1;

                    var fileName = request.Files[i].FileName.ToString();
                    var fullPath = Path.Combine(StorageRoot, fileName);

                    statuses.Add(new PMGSY.Models.QualityMonitoring.FileUploadViewModel()
                    {
                        url = fullPath,
                        thumbnail_url = fullPath,
                        name = fileName,
                        type = file.ContentType,
                        size = file.ContentLength,
                        delete_url = "",
                        delete_type = "DELETE",

                        //PdfDescription = request.Params["PdfDescription[]"],

                        QM_OBSERVATION_ID = QM_OBSERVATION_ID
                    });

                    string status = qualityBAL.AddATRDetailsBAL(statuses);
                    if (status == string.Empty)
                    {

                        //file.SaveAs(fullPath);
                        file.SaveAs(Path.Combine(StorageRoot, fileId.ToString() + ".pdf"));//File Will be saved as FileId.pdf
                    }
                    else
                    {
                        // show an error over here
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "QualityMonitoring.UploadPDFFile()");
            }
            finally
            {

            }
        }


        [HttpPost]
        public ActionResult selectATRReasonStatus(int reasonCode)
        {
            try
            {
                dbContext = new PMGSYEntities();
                int reasCode = Convert.ToInt32(reasonCode);
                bool flag = true;

                string reasonStatus = dbContext.MASTER_ATR_REGRADE_REASONS.Where(s => s.REASON_ID == reasCode).Select(m => m.REASON_STATUS).FirstOrDefault();
                string reason = dbContext.MASTER_ATR_REGRADE_REASONS.Where(s => s.REASON_ID == reasCode).Select(m => m.REASON_NAME).FirstOrDefault();
                return Json(new { success = flag, reasonStatus = reasonStatus, reason = reason });
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "qualityMonitoringController.selectATRReasonStatus()");
                return Json(new { string.Empty });
            }
        }

        /// <summary>
        /// Render form to Accept or reject ATR
        /// </summary>//changed by deendayal on 05-02-2018
        /// // change by sachin on 20 aug 2020
        /// <param name="id"></param>
        /// <returns></returns>
        [Audit]
        public ActionResult QMATRRegrade(string id)
        {
            QMATRRegradeModel qmATRRegradeModel = new QMATRRegradeModel();
            qmATRRegradeModel.QM_OBSERVATION_ID = Convert.ToInt32(id);
            try
            {
                dbContext = new PMGSYEntities();
                Int32 rdcode = Convert.ToInt32(id);
                Int32 RoadCode = dbContext.QUALITY_QM_OBSERVATION_MASTER.Where(x => x.QM_OBSERVATION_ID == rdcode).Select(m => m.IMS_PR_ROAD_CODE).FirstOrDefault();
                string RoadName = dbContext.IMS_SANCTIONED_PROJECTS.Where(x => x.IMS_PR_ROAD_CODE == RoadCode).Select(m => m.IMS_ROAD_NAME).FirstOrDefault();

                string PackageID = dbContext.IMS_SANCTIONED_PROJECTS.Where(x => x.IMS_PR_ROAD_CODE == RoadCode).Select(m => m.IMS_PACKAGE_ID).FirstOrDefault();
                Int32 StateCode = dbContext.IMS_SANCTIONED_PROJECTS.Where(x => x.IMS_PR_ROAD_CODE == RoadCode).Select(m => m.MAST_STATE_CODE).FirstOrDefault();
                String StateName = dbContext.MASTER_STATE.Where(x => x.MAST_STATE_CODE == StateCode).Select(m => m.MAST_STATE_NAME).FirstOrDefault();

                Int32 DistrictCode = dbContext.IMS_SANCTIONED_PROJECTS.Where(x => x.IMS_PR_ROAD_CODE == RoadCode).Select(m => m.MAST_DISTRICT_CODE).FirstOrDefault();
                string DistrictName = dbContext.MASTER_DISTRICT.Where(x => x.MAST_DISTRICT_CODE == DistrictCode).Select(m => m.MAST_DISTRICT_NAME).FirstOrDefault();

                Int32 BlockCode = dbContext.IMS_SANCTIONED_PROJECTS.Where(x => x.IMS_PR_ROAD_CODE == RoadCode).Select(m => m.MAST_BLOCK_CODE).FirstOrDefault();
                string BlockName = dbContext.MASTER_BLOCK.Where(x => x.MAST_BLOCK_CODE == BlockCode).Select(m => m.MAST_BLOCK_NAME).FirstOrDefault();


               // var atrReasonList = dbContext.MASTER_ATR_REGRADE_REASONS.Where(s => s.IS_ACTIVE == "Y").ToList();

                var atrReasonList = dbContext.MASTER_ATR_REGRADE_REASONS.Where(s => s.IS_ACTIVE == "Y" && s.TIER_TYPE == 3).ToList();
                qmATRRegradeModel.lstReasons = new List<SelectListItem>();
                qmATRRegradeModel.reasonCode = 0;
                List<SelectListItem> reasonList = new List<SelectListItem>();
                SelectListItem item1;
                item1 = new SelectListItem();
                item1.Text = "----------------------------------------------------------Select Reason-----------------------------------------------------------";
                item1.Value = "0";
                item1.Selected = true;
                reasonList.Add(item1);
                foreach (var item in atrReasonList)
                {
                    item1 = new SelectListItem();
                    item1.Text = item.REASON_NAME;
                    item1.Value = item.REASON_ID.ToString();
                    reasonList.Add(item1);
                }
                qmATRRegradeModel.lstReasons = reasonList;



                qmATRRegradeModel.ROAD_NAME = RoadName;

                qmATRRegradeModel.PACKAGE_ID = PackageID;
                qmATRRegradeModel.StateName = StateName;
                qmATRRegradeModel.DistrictName = DistrictName;
                qmATRRegradeModel.BlockName = BlockName;


                // qmATRRegradeModel.ROAD_NAME = RoadName;
                // qmATRRegradeModel.PACKAGE_ID = PackageID;
                //fetch monitor name 
                int? schCode = dbContext.QUALITY_QM_OBSERVATION_MASTER.Where(x => x.QM_OBSERVATION_ID == rdcode).Select(m => m.ADMIN_SCHEDULE_CODE).FirstOrDefault();



                if (PMGSYSession.Current.RoleCode == 9)
                {
                    return View(qmATRRegradeModel);
                }
                else
                {
                    return View("QMMAINTATRRegrade", qmATRRegradeModel);
                }
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "QualityMonitoringController.QMATRRegrade(string id)");
                return View(qmATRRegradeModel);
            }

        }


        /// <summary>
        /// Post to save Accept or Reject Status for ATR
        /// </summary>
        /// <param name="qmATRRegradeModel"></param>
        /// <returns></returns>
        [HttpPost]
        [Audit]
        public ActionResult QMATRRegrade(QMATRRegradeModel qmATRRegradeModel)
        {
            qualityBAL = new QualityMonitoringBAL();

            if (ModelState.IsValid)
            {
                string Status = qualityBAL.QMATRRegradeBAL(qmATRRegradeModel);
                if (Status == string.Empty)
                    return Json(new { Success = true });
                else
                    return Json(new { Success = false, ErrorMessage = Status });
            }
            else
            {
                StringBuilder errorMessages = new StringBuilder();
                foreach (var modelStateValue in ModelState.Values)
                {
                    foreach (var error in modelStateValue.Errors)
                    {
                        errorMessages.Append(error.ErrorMessage);
                    }
                }
                return Json(new { Success = false, ErrorMessage = errorMessages.ToString() });
            }
        }


        /// <summary>
        /// Accpted ATR Observation Details
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Audit]
        public ActionResult QMATRAccpetedObsDetails(string id1, string id2)
        {
            dbContext = new PMGSYEntities();
            qualityBAL = new QualityMonitoringBAL();
            QMFillObservationModel fillObservationModel = null;
            try
            {
                if (Convert.ToBoolean(id2)) //ATR Corrected Grading Details
                {
                    fillObservationModel = qualityBAL.QMObservationDetailsATRBAL(Convert.ToInt32(id1));
                    fillObservationModel.QM_OBSERVATION_ID = Convert.ToInt32(id1);
                    fillObservationModel.IS_ATR_PAGE = "Y";
                    return View("QMATRAccpetedObsDetails", fillObservationModel);
                }
                else
                {
                    fillObservationModel = qualityBAL.QMObservationDetailsBAL(Convert.ToInt32(id1));
                    fillObservationModel.QM_OBSERVATION_ID = Convert.ToInt32(id1);
                    fillObservationModel.IS_ATR_PAGE = "Y";
                    return View("QMATRObsDetails", fillObservationModel);
                }

            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return null;
            }
            finally
            {
                dbContext.Dispose();
            }
        }


        /// <summary>
        /// Delete ATR Details against observations
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        [Audit]
        public ActionResult QMDeleteATRDetails()
        {
            qualityBAL = new QualityMonitoringBAL();
            try
            {
                Int32 ObservationId = Convert.ToInt32(Request.Params["obsId"]);
                Int32 ATRId = Convert.ToInt32(Request.Params["atrId"]);
                if (ATRId != 0)
                {
                    string Status = qualityBAL.QMDeleteATRDetailsBAL(ObservationId, ATRId);
                    if (Status == string.Empty)
                        return Json(new { Success = true });
                    else
                        return Json(new { Success = false, ErrorMessage = Status });
                }
                else
                {
                    return Json(new { Success = false, ErrorMessage = "An Error Occurred While Processing Your Request." });
                }
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return Json(new { Success = false, ErrorMessage = "An Error Occurred While Processing Your Request." });
            }
        }


        //----------------- ADDED BY ROHIT BORSE for ATR MARK VERIFICATION

        [HttpGet]
        [Audit]
        public ActionResult ViewVerificationATR_ListByRoadCode_Inspdate_ObsId_ATRId(string VerificationATRCode, int? page, int? rows, String sidx, String sord, string filters)
        {
            QualityMonitoringDAL qualityDAL = new QualityMonitoringDAL();

            long totalrecords;

            try
            {
                string[] tempVerificationATRcode = Request.Params["VerificationATRCode"].Split('$');

                // Get Observation ID and Road Code
                int roadCode = Convert.ToInt32(tempVerificationATRcode[0]);
                string inspectionDate = tempVerificationATRcode[1];
                int NQM_ObservationId = Convert.ToInt32(tempVerificationATRcode[2]);

                //Below line is commented on 20-01-2023
                //int NQM_ATRId = Convert.ToInt32(tempVerificationATRcode[3]);

                //Below  two lines are  Added on 20-01-2023
                dbContext = new PMGSYEntities();
                int NQM_ATRId = dbContext.QUALITY_ATR_FILE.Where(x => x.QM_OBSERVATION_ID == NQM_ObservationId && x.ATR_REGRADE_STATUS != "R").Select(x => x.QM_ATR_ID).FirstOrDefault();

                List<String> selectedIdList = new List<String>();

                var jsonData = new
                {
                    rows = qualityDAL.VerificationATRListByObsId_RoadCodeDAL(roadCode, inspectionDate, NQM_ObservationId, NQM_ATRId, page, rows, sidx, sord, out totalrecords, filters, out selectedIdList),
                    total = totalrecords <= Convert.ToInt32(rows) ? 1 : (totalrecords % Convert.ToInt32(rows) == 0 ? totalrecords / Convert.ToInt32(rows) : totalrecords / Convert.ToInt32(rows) + 1),
                    page = Convert.ToInt32(page),
                    records = totalrecords,
                    userdata = new { ids = selectedIdList.ToArray<string>() }
                };
                return Json(jsonData, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "QualityMonitoringController.VerificationATRListByObsId_RoadCode()");
                return Json(new { Success = false, ErrorMessage = "An Error Occurred While Processing Your Request." });
            }
        }

        //----------------- ADDED BY ROHIT BORSE for ATR MARK VERIFICATION
        [HttpPost]
        public JsonResult savefinalizedMarkVerificationATR(string nqmobsidatrid, int[] submitarray)
        {

            string message = null;
            try
            {
                string[] tempVerificationATRcode = nqmobsidatrid.Split('$');

                int SQM_ObservationId = submitarray[0]; //Convert.ToInt32(Request.Params["submitSQMId"]);
                int NQM_ObservationId = Convert.ToInt32(tempVerificationATRcode[2]);
                int NQM_ATRid = Convert.ToInt32(tempVerificationATRcode[3]);

                QualityMonitoringDAL qualityDAL = new QualityMonitoringDAL();

                bool status = qualityDAL.savefinalizedMarkVerificationATRDAL(SQM_ObservationId, NQM_ObservationId, NQM_ATRid);
                if (status)
                    message = "Mark for ATR Verification Finalization Successfully";
                else
                    message = "ERROR !!  Changes are not saved.";

                return Json(new { success = status, message = message });
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "QualityMonitoringController.savefinalizedMarkVerificationATR()");
                return Json(new { success = false, message = "An error occured while processing your request." });
            }
        }




        #endregion


        #region CQC


        /// <summary>
        /// Filters for CQC Login
        /// </summary>
        /// <returns></returns>
        [Audit]
        public ActionResult CQCFilters()
        {
            QMFilterViewModel qmFilterModel = new QMFilterViewModel();
            CommonFunctions objCommonFunctions = new CommonFunctions();

            qmFilterModel.MAST_STATE_CODE = 0;
            qmFilterModel.ADMIN_QM_CODE = 0;

            qmFilterModel.FROM_MONTH = DateTime.Now.Month;
            qmFilterModel.FROM_YEAR = DateTime.Now.Year;

            qmFilterModel.STATES = objCommonFunctions.PopulateStates(false);
            qmFilterModel.STATES.Insert(0, (new SelectListItem { Text = "Select State", Value = "0", Selected = true }));

            qmFilterModel.MONITORS = new SelectList(new List<SelectListItem>().AsEnumerable<SelectListItem>(), "Value", "Text").ToList();      //Purposely taken String "false" as argument
            qmFilterModel.MONITORS.Insert(0, (new SelectListItem { Text = "Select Monitor", Value = "0", Selected = true }));

            qmFilterModel.FROM_MONTHS_LIST = objCommonFunctions.PopulateMonths(false);
            qmFilterModel.FROM_YEARS_LIST = objCommonFunctions.PopulateYears(false).Where(a => a.Value != "0").ToList();
            return View(qmFilterModel);
        }


        /// <summary>
        /// Populate those monitors who have schedule in particular state & particular month-year
        /// </summary>
        /// <param name="selectedState"></param>
        /// <param name="inspMonth"></param>
        /// <param name="inspYear"></param>
        /// <returns></returns>
        [Audit]
        public ActionResult GetScheduledMonitors(int selectedState, int inspMonth, int inspYear)
        {
            dbContext = new PMGSYEntities();
            try
            {
                List<SelectListItem> lstMonitors = new List<SelectListItem>();
                SelectListItem item;

                item = new SelectListItem();
                item.Text = "Select Monitor";
                item.Value = "0";
                lstMonitors.Add(item);


                var query = (from aqm in dbContext.ADMIN_QUALITY_MONITORS
                             join qqs in dbContext.QUALITY_QM_SCHEDULE on aqm.ADMIN_QM_CODE equals qqs.ADMIN_QM_CODE
                             where aqm.ADMIN_QM_TYPE == "I"
                             && aqm.ADMIN_QM_EMPANELLED == "Y"
                             && qqs.MAST_STATE_CODE == selectedState
                             && qqs.ADMIN_IM_MONTH == inspMonth
                             && qqs.ADMIN_IM_YEAR == inspYear
                             select new
                             {
                                 Value = aqm.ADMIN_QM_CODE,
                                 Text = (aqm.ADMIN_QM_LNAME.Equals(null) ? "" : aqm.ADMIN_QM_LNAME) + " " + (aqm.ADMIN_QM_FNAME.Equals(null) ? "" : aqm.ADMIN_QM_FNAME) + " " + (aqm.ADMIN_QM_MNAME.Equals(null) ? "" : aqm.ADMIN_QM_MNAME)
                             }).OrderBy(c => c.Text).Distinct().OrderBy(a => a.Text).ToList();

                foreach (var data in query)
                {
                    item = new SelectListItem();
                    item.Text = data.Text;
                    item.Value = data.Value.ToString();
                    lstMonitors.Add(item);
                }

                return Json(lstMonitors);
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
        /// Monitor's Scheduled Road List in CQC Login
        /// </summary>
        /// <param name="formCollection"></param>
        /// <returns></returns>
        [HttpPost]
        [Audit]
        public ActionResult CQCMonitorsScheduledRoadList(FormCollection formCollection)
        {
            qualityBAL = new QualityMonitoringBAL();
            int totalRecords;
            try
            {
                //Adde By Abhishek kamble 29-Apr-2014 start
                using (CommonFunctions commonFunction = new CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new GridParams(Convert.ToInt32(formCollection["page"]), Convert.ToInt32(formCollection["rows"]), formCollection["sidx"], formCollection["sord"], Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                    {
                        return null;
                    }
                }
                //Adde By Abhishek kamble 29-Apr-2014 end
                var jsonData = new
                {
                    rows = qualityBAL.CQCMonitorsScheduledRoadListBAL(Convert.ToInt32(formCollection["page"]) - 1, Convert.ToInt32(formCollection["rows"]),
                                                            formCollection["sidx"], formCollection["sord"], out totalRecords,
                                                            Convert.ToInt32(formCollection["monitorCode"]), Convert.ToInt32(formCollection["inspMonth"]), Convert.ToInt32(formCollection["inspYear"])),
                    total = totalRecords <= Convert.ToInt32(formCollection["rows"]) ? 1 : (totalRecords % Convert.ToInt32(formCollection["rows"]) == 0 ? totalRecords / Convert.ToInt32(formCollection["rows"]) : (totalRecords / Convert.ToInt32(formCollection["rows"]) + 1)),
                    page = Convert.ToInt32(formCollection["page"]),
                    records = totalRecords
                };
                return Json(jsonData, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return Json(String.Empty);
            }
        }

        #endregion


        #region Reports
        /// <summary>
        /// QM Inspection Report View
        /// </summary>
        /// <returns></returns>
        [Audit]
        public ActionResult QMInspectionReport()
        {
            return View();
        }


        /// <summary>
        /// StateWise List of All roads with NQM & SQM Inspection Count
        /// </summary>
        /// <param name="formCollection"></param>
        /// <returns></returns>
        [HttpPost]
        [Audit]
        public ActionResult QMInspectionReportList(FormCollection formCollection)
        {
            qualityBAL = new QualityMonitoringBAL();
            int totalRecords;
            try
            {
                //Adde By Abhishek kamble 29-Apr-2014 start
                using (CommonFunctions commonFunction = new CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new GridParams(Convert.ToInt32(formCollection["page"]), Convert.ToInt32(formCollection["rows"]), formCollection["sidx"], formCollection["sord"], Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                    {
                        return null;
                    }
                }
                //Adde By Abhishek kamble 29-Apr-2014 end
                var jsonData = new
                {
                    rows = qualityBAL.QMInspectionReportBAL(Convert.ToInt32(formCollection["page"]) - 1, Convert.ToInt32(formCollection["rows"]),
                                                            formCollection["sidx"], formCollection["sord"], out totalRecords),
                    total = totalRecords <= Convert.ToInt32(formCollection["rows"]) ? 1 : (totalRecords % Convert.ToInt32(formCollection["rows"]) == 0 ? totalRecords / Convert.ToInt32(formCollection["rows"]) : (totalRecords / Convert.ToInt32(formCollection["rows"]) + 1)),
                    page = Convert.ToInt32(formCollection["page"]),
                    records = totalRecords
                };
                return Json(jsonData, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return Json(String.Empty);
            }
        }



        /// <summary>
        /// Overall Inspected Roads Details (State Wise)
        /// </summary>
        /// <param name="formCollection"></param>
        /// <returns></returns>
        [HttpPost]
        [Audit]
        public ActionResult QMOverallDistrictwiseInspDetailsReport(FormCollection formCollection)
        {
            qualityBAL = new QualityMonitoringBAL();
            int totalRecords;
            try
            {
                //Adde By Abhishek kamble 29-Apr-2014 start
                using (CommonFunctions commonFunction = new CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new GridParams(Convert.ToInt32(formCollection["page"]), Convert.ToInt32(formCollection["rows"]), formCollection["sidx"], formCollection["sord"], Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                    {
                        return null;
                    }
                }
                //Adde By Abhishek kamble 29-Apr-2014 end
                var jsonData = new
                {
                    rows = qualityBAL.QMOverallDistrictwiseInspDetailsReportBAL(Convert.ToInt32(formCollection["page"]) - 1, Convert.ToInt32(formCollection["rows"]),
                                                            formCollection["sidx"], formCollection["sord"], out totalRecords,
                                                            Convert.ToInt32(formCollection["stateCode"]), Convert.ToInt32(formCollection["districtCode"]), formCollection["qmType"]),
                    total = totalRecords <= Convert.ToInt32(formCollection["rows"]) ? 1 : (totalRecords % Convert.ToInt32(formCollection["rows"]) == 0 ? totalRecords / Convert.ToInt32(formCollection["rows"]) : (totalRecords / Convert.ToInt32(formCollection["rows"]) + 1)),
                    page = Convert.ToInt32(formCollection["page"]),
                    records = totalRecords
                };
                return Json(jsonData, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return Json(String.Empty);
            }
        }


        /// <summary>
        /// View for State Wise Abstract of Grading & ATR Details
        /// </summary>
        /// <returns></returns>
        [Audit]
        public ActionResult QMGradingAndATRDetails()
        {
            QMReportsViewModel qmReportsViewModel = new QMReportsViewModel();
            CommonFunctions commonFunctions = new CommonFunctions();
            try
            {
                qmReportsViewModel.FROM_YEAR = DateTime.Now.Year;
                qmReportsViewModel.TO_YEAR = DateTime.Now.Year;

                qmReportsViewModel.FROM_MONTH = 1;
                qmReportsViewModel.TO_MONTH = DateTime.Now.Month;

                qmReportsViewModel.FROM_YEAR_LIST = commonFunctions.PopulateYears(false);
                qmReportsViewModel.TO_YEAR_LIST = commonFunctions.PopulateYears(false);
                qmReportsViewModel.FROM_MONTH_LIST = commonFunctions.PopulateMonths(false);
                qmReportsViewModel.TO_MONTH_LIST = commonFunctions.PopulateMonths(false);

                return View(qmReportsViewModel);
            }
            catch (Exception)
            {
                return View();
            }
        }


        /// <summary>
        /// Report Listing for State Wise Abstract of Grading & ATR Details
        /// </summary>
        /// <returns></returns>
        [Audit]
        public ActionResult QMGradingAndATRListing(FormCollection formCollection)
        {
            qualityBAL = new QualityMonitoringBAL();
            int totalRecords;
            try
            {
                //Adde By Abhishek kamble 29-Apr-2014 start
                using (CommonFunctions commonFunction = new CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new GridParams(Convert.ToInt32(formCollection["page"]), Convert.ToInt32(formCollection["rows"]), formCollection["sidx"], formCollection["sord"], Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                    {
                        return null;
                    }
                }
                //Adde By Abhishek kamble 29-Apr-2014 end
                var jsonData = new
                {
                    rows = qualityBAL.QMGradingAndATRListingBAL(Convert.ToInt32(formCollection["page"]) - 1, Convert.ToInt32(formCollection["rows"]),
                                                            formCollection["sidx"], formCollection["sord"], out totalRecords,
                                                            Convert.ToInt32(formCollection["fromYear"]), Convert.ToInt32(formCollection["toYear"]),
                                                            Convert.ToInt32(formCollection["fromMonth"]), Convert.ToInt32(formCollection["toMonth"]),
                                                            formCollection["qmType"]),
                    total = totalRecords <= Convert.ToInt32(formCollection["rows"]) ? 1 : (totalRecords % Convert.ToInt32(formCollection["rows"]) == 0 ? totalRecords / Convert.ToInt32(formCollection["rows"]) : (totalRecords / Convert.ToInt32(formCollection["rows"]) + 1)),
                    page = Convert.ToInt32(formCollection["page"]),
                    records = totalRecords
                };
                return Json(jsonData, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return Json(String.Empty);
            }
        }


        /// <summary>
        /// Renders view for NQM & SQMs grading comparision Report
        /// </summary>
        /// <returns></returns>
        [Audit]

        public ActionResult QMGradingComparision()
        {
            QMReportsViewModel qmReportsViewModel = new QMReportsViewModel();
            CommonFunctions commonFunctions = new CommonFunctions();
            try
            {
                qmReportsViewModel.YEAR = DateTime.Now.Year;
                qmReportsViewModel.STATE = PMGSYSession.Current.StateCode;
                qmReportsViewModel.STATE_LIST = commonFunctions.PopulateStates(true);
                if (PMGSYSession.Current.RoleCode == 8) //SQC
                {
                    qmReportsViewModel.DISTRICT_LIST = commonFunctions.PopulateDistrict(PMGSYSession.Current.StateCode, false, 0);
                    qmReportsViewModel.DISTRICT_LIST.RemoveAt(0);
                }
                else
                {
                    qmReportsViewModel.DISTRICT_LIST = new SelectList(new List<SelectListItem>().AsEnumerable<SelectListItem>(), "Value", "Text").ToList();

                }
                qmReportsViewModel.DISTRICT_LIST.Insert(0, (new SelectListItem { Text = "All", Value = "0", Selected = true }));
                qmReportsViewModel.YEAR_LIST = commonFunctions.PopulateYears(false);
                qmReportsViewModel.MONTH_LIST = commonFunctions.PopulateMonths(false);
                qmReportsViewModel.MONTH_LIST.Insert(0, (new SelectListItem { Text = "All", Value = "0", Selected = true }));
                return View(qmReportsViewModel);
            }
            catch (Exception)
            {
                return View();
            }
        }


        /// <summary>
        /// Listing for NQM SQM Grading comparision (Monthly or Yearly)
        /// </summary>
        /// <param name="formCollection"></param>
        /// <returns></returns>
        [Audit]
        public ActionResult QMGradingComparisionListing(FormCollection formCollection)
        {
            qualityBAL = new QualityMonitoringBAL();
            int totalRecords;
            try
            {
                //Adde By Abhishek kamble 29-Apr-2014 start
                using (CommonFunctions commonFunction = new CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new GridParams(Convert.ToInt32(formCollection["page"]), Convert.ToInt32(formCollection["rows"]), formCollection["sidx"], formCollection["sord"], Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                    {
                        return null;
                    }
                }
                //Adde By Abhishek kamble 29-Apr-2014 end
                var jsonData = new
                {
                    rows = qualityBAL.QMGradingComparisionListingBAL(Convert.ToInt32(formCollection["page"]) - 1, Convert.ToInt32(formCollection["rows"]),
                                                            formCollection["sidx"], formCollection["sord"], out totalRecords,
                                                            Convert.ToInt32(formCollection["state"]), formCollection["district"],
                                                            Convert.ToInt32(formCollection["year"]), formCollection["month"]),
                    total = totalRecords <= Convert.ToInt32(formCollection["rows"]) ? 1 : (totalRecords % Convert.ToInt32(formCollection["rows"]) == 0 ? totalRecords / Convert.ToInt32(formCollection["rows"]) : (totalRecords / Convert.ToInt32(formCollection["rows"]) + 1)),
                    page = Convert.ToInt32(formCollection["page"]),
                    records = totalRecords
                };
                return Json(jsonData, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return Json(String.Empty);
            }
        }



        /// <summary>
        /// Renders view for Monthwise Inspections for NQM & SQMs
        /// </summary>
        /// <returns></returns>
        [Audit]
        public ActionResult QMMonthwiseInspections()
        {
            QMReportsViewModel qmReportsViewModel = new QMReportsViewModel();
            CommonFunctions commonFunctions = new CommonFunctions();
            try
            {
                qmReportsViewModel.YEAR = DateTime.Now.Year;
                qmReportsViewModel.STATE = PMGSYSession.Current.StateCode;
                qmReportsViewModel.STATE_LIST = commonFunctions.PopulateStates(true);
                qmReportsViewModel.STATE_LIST.RemoveAt(0);
                qmReportsViewModel.STATE_LIST.Insert(0, (new SelectListItem { Text = "All", Value = "0", Selected = true }));
                qmReportsViewModel.YEAR_LIST = commonFunctions.PopulateYears(false);
                return View(qmReportsViewModel);
            }
            catch (Exception)
            {
                return View();
            }
        }


        /// <summary>
        /// Listing for Monthwise Inspections for NQM & SQMs
        /// Now only NQMs data is shown but same action can be used to show SQM Data
        /// </summary>
        /// <param name="formCollection"></param>
        /// <returns></returns>
        [Audit]
        public ActionResult QMMonthwiseInspectionsListing(FormCollection formCollection)
        {
            qualityBAL = new QualityMonitoringBAL();
            int totalRecords;
            try
            {
                //Adde By Abhishek kamble 29-Apr-2014 start
                using (CommonFunctions commonFunction = new CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new GridParams(Convert.ToInt32(formCollection["page"]), Convert.ToInt32(formCollection["rows"]), formCollection["sidx"], formCollection["sord"], Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                    {
                        return null;
                    }
                }
                //Adde By Abhishek kamble 29-Apr-2014 end
                var jsonData = new
                {
                    rows = qualityBAL.QMMonthwiseInspectionListingBAL(Convert.ToInt32(formCollection["page"]) - 1, Convert.ToInt32(formCollection["rows"]),
                                                            formCollection["sidx"], formCollection["sord"], out totalRecords,
                                                            Convert.ToInt32(formCollection["state"]), Convert.ToInt32(formCollection["year"]), formCollection["qmtype"]),
                    total = totalRecords <= Convert.ToInt32(formCollection["rows"]) ? 1 : (totalRecords % Convert.ToInt32(formCollection["rows"]) == 0 ? totalRecords / Convert.ToInt32(formCollection["rows"]) : (totalRecords / Convert.ToInt32(formCollection["rows"]) + 1)),
                    page = Convert.ToInt32(formCollection["page"]),
                    records = totalRecords
                };
                return Json(jsonData, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return Json(String.Empty);
            }
        }


        /// <summary>
        /// Renders view for Summary of Deficiencies Reported in NQM Inspection
        /// </summary>
        /// <returns></returns>
        [Audit]
        public ActionResult QMItemwiseInspections()
        {
            qualityBAL = new QualityMonitoringBAL();
            QMReportsViewModel qmReportsViewModel = new QMReportsViewModel();
            CommonFunctions commonFunctions = new CommonFunctions();
            dbContext = new PMGSYEntities();
            try
            {
                qmReportsViewModel.ITEM_LIST = new SelectList(dbContext.MASTER_QM_ITEM.Where(m => m.MAST_SUB_ITEM_CODE == 0 &&
                                                m.MAST_QM_TYPE == "N" && m.MAST_ITEM_STATUS != "M" && m.MAST_ITEM_STATUS != "O"
                                                ), "MAST_ITEM_NO", "MAST_ITEM_NAME").ToList();

                qmReportsViewModel.ITEM_LIST.Insert(0, (new SelectListItem { Text = "All", Value = "0", Selected = true }));

                qmReportsViewModel.STATE = PMGSYSession.Current.StateCode;
                qmReportsViewModel.STATE_LIST = commonFunctions.PopulateStates(true);
                qmReportsViewModel.STATE_LIST.RemoveAt(0);
                qmReportsViewModel.STATE_LIST.Insert(0, (new SelectListItem { Text = "All", Value = "0", Selected = true }));

                qmReportsViewModel.FROM_YEAR = DateTime.Now.Year - 1;
                qmReportsViewModel.TO_YEAR = DateTime.Now.Year;

                qmReportsViewModel.FROM_MONTH = 1;
                qmReportsViewModel.TO_MONTH = DateTime.Now.Month;

                qmReportsViewModel.FROM_YEAR_LIST = commonFunctions.PopulateYears(false);
                qmReportsViewModel.TO_YEAR_LIST = commonFunctions.PopulateYears(false);
                qmReportsViewModel.FROM_MONTH_LIST = commonFunctions.PopulateMonths(false);
                qmReportsViewModel.TO_MONTH_LIST = commonFunctions.PopulateMonths(false);

                qmReportsViewModel.GRADE = 2;
                return View(qmReportsViewModel);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return null;
            }
            finally
            {
                dbContext.Dispose();
            }
        }


        /// <summary>
        /// Listing for Summary of Deficiencies Reported in NQM Inspection
        /// </summary>
        /// <param name="formCollection"></param>
        /// <returns></returns>
        [Audit]
        public ActionResult QMItemwiseInspectionsListing(FormCollection formCollection)
        {
            qualityBAL = new QualityMonitoringBAL();
            int totalRecords;
            try
            {
                //Adde By Abhishek kamble 29-Apr-2014 start
                using (CommonFunctions commonFunction = new CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new GridParams(Convert.ToInt32(formCollection["page"]), Convert.ToInt32(formCollection["rows"]), formCollection["sidx"], formCollection["sord"], Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                    {
                        return null;
                    }
                }
                //Adde By Abhishek kamble 29-Apr-2014 end
                var jsonData = new
                {
                    rows = qualityBAL.QMItemwiseNQMInspectionReportBAL(Convert.ToInt32(formCollection["page"]) - 1, Convert.ToInt32(formCollection["rows"]),
                                                            formCollection["sidx"], formCollection["sord"], out totalRecords,
                                                            formCollection["state"], Convert.ToInt32(formCollection["grade"]),
                                                            Convert.ToInt32(formCollection["fromyear"]), Convert.ToInt32(formCollection["frommonth"]),
                                                            Convert.ToInt32(formCollection["toyear"]), Convert.ToInt32(formCollection["tomonth"]),
                                                            Convert.ToInt32(formCollection["citem"]), formCollection["qmtype"]),
                    total = totalRecords <= Convert.ToInt32(formCollection["rows"]) ? 1 : (totalRecords % Convert.ToInt32(formCollection["rows"]) == 0 ? totalRecords / Convert.ToInt32(formCollection["rows"]) : (totalRecords / Convert.ToInt32(formCollection["rows"]) + 1)),
                    page = Convert.ToInt32(formCollection["page"]),
                    records = totalRecords
                };
                return Json(jsonData, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return Json(String.Empty);
            }
        }



        /// <summary>
        /// ATR Details Report
        /// </summary>
        /// <returns></returns>
        [Audit]
        public ActionResult QMATRDetailsReport()
        {
            qualityBAL = new QualityMonitoringBAL();
            QMReportsViewModel qmReportsViewModel = new QMReportsViewModel();
            CommonFunctions commonFunctions = new CommonFunctions();
            try
            {
                qmReportsViewModel.FROM_YEAR = DateTime.Now.Year - 1;
                qmReportsViewModel.TO_YEAR = DateTime.Now.Year;

                qmReportsViewModel.FROM_MONTH = 1;
                qmReportsViewModel.TO_MONTH = DateTime.Now.Month;

                qmReportsViewModel.FROM_YEAR_LIST = commonFunctions.PopulateYears(false);
                qmReportsViewModel.TO_YEAR_LIST = commonFunctions.PopulateYears(false);
                qmReportsViewModel.FROM_MONTH_LIST = commonFunctions.PopulateMonths(false);
                qmReportsViewModel.TO_MONTH_LIST = commonFunctions.PopulateMonths(false);

                return View(qmReportsViewModel);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return null;
            }
        }


        /// <summary>
        /// ATR  Details (State Wise)
        /// </summary>
        /// <param name="formCollection"></param>
        /// <returns></returns>
        [Audit]
        [HttpPost]
        public ActionResult QMATRDetailsReportList(FormCollection formCollection)
        {
            qualityBAL = new QualityMonitoringBAL();
            int totalRecords;
            try
            {
                //Adde By Abhishek kamble 29-Apr-2014 start
                using (CommonFunctions commonFunction = new CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new GridParams(Convert.ToInt32(formCollection["page"]), Convert.ToInt32(formCollection["rows"]), formCollection["sidx"], formCollection["sord"], Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                    {
                        return null;
                    }
                }
                //Adde By Abhishek kamble 29-Apr-2014 end
                var jsonData = new
                {
                    rows = qualityBAL.QMATRDetailsReportBAL(Convert.ToInt32(formCollection["page"]) - 1, Convert.ToInt32(formCollection["rows"]),
                                                            formCollection["sidx"], formCollection["sord"], out totalRecords,
                                                            Convert.ToInt32(formCollection["fromyear"]), Convert.ToInt32(formCollection["frommonth"]),
                                                            Convert.ToInt32(formCollection["toyear"]), Convert.ToInt32(formCollection["tomonth"])),
                    total = totalRecords <= Convert.ToInt32(formCollection["rows"]) ? 1 : (totalRecords % Convert.ToInt32(formCollection["rows"]) == 0 ? totalRecords / Convert.ToInt32(formCollection["rows"]) : (totalRecords / Convert.ToInt32(formCollection["rows"]) + 1)),
                    page = Convert.ToInt32(formCollection["page"]),
                    records = totalRecords
                };
                return Json(jsonData, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return Json(String.Empty);
            }
        }


        /// <summary>
        /// View & Filters for Unsatisfactory Work Details
        /// </summary>
        /// <returns></returns>
        public ActionResult QMUnsatisfactoryWorkDetailsFilters()
        {
            qualityBAL = new QualityMonitoringBAL();
            QMReportsViewModel qmReportsViewModel = new QMReportsViewModel();
            CommonFunctions commonFunctions = new CommonFunctions();
            try
            {
                qmReportsViewModel.QM_TYPE = "I";
                qmReportsViewModel.STATE_LIST = commonFunctions.PopulateStates(true);
                qmReportsViewModel.QM_TYPE_LIST = commonFunctions.PopulateMonitorTypes();

                return View(qmReportsViewModel);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return null;
            }
        }


        /// <summary>
        /// On Report Layout Observations Details not rendered properly, (Images)
        /// so created another view specially for reporting
        /// </summary>
        /// <returns></returns>
        public ActionResult QMObservationDetailsRpt(string id)
        {
            qualityBAL = new QualityMonitoringBAL();
            try
            {
                QMFillObservationModel fillObservationModel = qualityBAL.QMObservationDetailsBAL(Convert.ToInt32(id));
                fillObservationModel.QM_OBSERVATION_ID = Convert.ToInt32(id);
                return View(fillObservationModel);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return null;
            }
        }

        /// <summary>
        /// List all Unsatisfactory works till date in a state for NQM or SQM
        /// </summary>
        /// <returns></returns>
        [Audit]
        [HttpPost]
        public ActionResult QMUnsatisfactoryWorkDetails()
        {
            QualityMonitoringBAL qmBAL = new QualityMonitoringBAL();
            QMUnsatisfactoryWorkModel unsatisfactoryDetailsModel = new QMUnsatisfactoryWorkModel();
            unsatisfactoryDetailsModel.ROAD_LIST = new List<QMUnsatisfactoryRoadModel>();
            unsatisfactoryDetailsModel.OBS_LIST = new List<QMUnsatisfactoryObsModel>();
            dbContext = new PMGSYEntities();
            try
            {
                int stateCode = Convert.ToInt32(Request.Params["stateCode"]);
                unsatisfactoryDetailsModel.ROAD_OBS_LIST = qmBAL.UnsatisfactoryWorkDetailsBAL(stateCode, Request.Params["qmType"]);
                var stateOfficersDetails = (from sqc in dbContext.ADMIN_SQC
                                            join
                                                ms in dbContext.MASTER_STATE on sqc.MAST_STATE_CODE equals ms.MAST_STATE_CODE
                                            join
                                                ad in dbContext.ADMIN_DEPARTMENT on ms.MAST_STATE_CODE equals ad.MAST_STATE_CODE
                                            join
                                                ano in dbContext.ADMIN_NODAL_OFFICERS on ad.ADMIN_ND_CODE equals ano.ADMIN_ND_CODE
                                            where
                                                 sqc.MAST_STATE_CODE == stateCode &&
                                                 ad.MAST_ND_TYPE == "S" &&
                                                 ano.ADMIN_NO_DESIGNATION == 30
                                            select new
                                            {
                                                STATE_NAME = ms.MAST_STATE_NAME,
                                                QC_NAME = sqc.ADMIN_QC_NAME,
                                                QC_PHONE = sqc.ADMIN_QC_PHONE1 + ((sqc.ADMIN_QC_PHONE2 != null && !sqc.ADMIN_QC_PHONE2.Equals(string.Empty)) ? (", " + sqc.ADMIN_QC_PHONE2) : ""),
                                                CEO_NAME = ano.ADMIN_NO_FNAME + (ano.ADMIN_NO_MNAME != null ? (" " + ano.ADMIN_NO_MNAME) : "") + (ano.ADMIN_NO_LNAME != null ? (" " + ano.ADMIN_NO_LNAME) : ""),
                                                CEO_PHONE = ano.ADMIN_NO_OFFICE_PHONE
                                            }).ToList();

                foreach (var item in stateOfficersDetails)
                {
                    unsatisfactoryDetailsModel.STATE_NAME = item.STATE_NAME;
                    unsatisfactoryDetailsModel.QC_NAME = item.QC_NAME;
                    unsatisfactoryDetailsModel.QC_PHONE = item.QC_PHONE;
                    unsatisfactoryDetailsModel.CEO_NAME = item.CEO_NAME;
                    unsatisfactoryDetailsModel.CEO_PHONE = item.CEO_PHONE;
                }


                var distinctRoadList = (from road in unsatisfactoryDetailsModel.ROAD_OBS_LIST
                                        select new
                                        {
                                            road.IMS_PR_ROAD_CODE,
                                            road.DISTRICT_NAME,
                                            road.IMS_PACKAGE_ID,
                                            road.IMS_YEAR,
                                            road.IMS_ROAD_NAME,
                                            road.IMS_PROPOSAL_TYPE
                                        }).Distinct().ToList();

                foreach (var item in distinctRoadList)
                {
                    QMUnsatisfactoryRoadModel roadModel = new QMUnsatisfactoryRoadModel();
                    roadModel.IMS_PR_ROAD_CODE = item.IMS_PR_ROAD_CODE;
                    roadModel.DISTRICT_NAME = item.DISTRICT_NAME;
                    roadModel.IMS_PACKAGE_ID = item.IMS_PACKAGE_ID;
                    roadModel.IMS_YEAR = item.IMS_YEAR.ToString() + "-" + (item.IMS_YEAR + 1).ToString();
                    roadModel.IMS_ROAD_NAME = item.IMS_ROAD_NAME;
                    //roadModel.IMS_PROPOSAL_TYPE = item.IMS_PROPOSAL_TYPE.Equals("P") ? "Road" : "LSB";

                    unsatisfactoryDetailsModel.ROAD_LIST.Add(roadModel);
                }

                foreach (var item in unsatisfactoryDetailsModel.ROAD_OBS_LIST)
                {
                    // Populate & Add obs Details for each PR_ROAD_CODE
                    QMUnsatisfactoryObsModel obsModel = new QMUnsatisfactoryObsModel();
                    obsModel.IMS_PR_ROAD_CODE = item.IMS_PR_ROAD_CODE;
                    obsModel.QM_OBSERVATION_ID = item.QM_OBSERVATION_ID;
                    obsModel.ADMIN_SCHEDULE_CODE = item.ADMIN_SCHEDULE_CODE;
                    obsModel.ADMIN_QM_CODE = item.ADMIN_QM_CODE;
                    obsModel.ADMIN_QM_TYPE = item.ADMIN_QM_TYPE.Equals("I") ? "NQM" : "SQM";
                    obsModel.MONITOR_NAME = item.MONITOR_NAME;
                    obsModel.QM_INSPECTION_DATE = item.QM_INSPECTION_DATE;
                    obsModel.IMS_ISCOMPLETED = item.IMS_ISCOMPLETED;
                    obsModel.OVERALL_GRADE = item.OVERALL_GRADE;
                    obsModel.OBS_LINK = "<a href='#' title='Click here to see observation details' class='ui-icon ui-icon-zoomin ui-align-center' onClick='ShowObsDetails(\"" + item.QM_OBSERVATION_ID.ToString().Trim() + "\"); return false;'></a>";

                    unsatisfactoryDetailsModel.OBS_LIST.Add(obsModel);
                }

                return View(unsatisfactoryDetailsModel);
            }
            catch
            {
                unsatisfactoryDetailsModel.ERROR = "Error occurred while processing your request";
                return View(unsatisfactoryDetailsModel); //return model as null
            }
            finally
            {
                dbContext.Dispose();
            }
        }


        /// <summary>
        /// Commenced Work List
        /// </summary>
        /// <returns></returns>
        public ActionResult QMCommencedWorkDetails()
        {
            QualityMonitoringBAL qmBAL = new QualityMonitoringBAL();
            QMCommencedWorkModel model = new QMCommencedWorkModel();
            try
            {
                model.WORK_LIST = qmBAL.CommencedWorkDetailsBAL();
                return View(model);
            }
            catch
            {
                model.ERROR = "Error occurred while processing your request";
                return View(model);
            }
        }


        /// <summary>
        /// Render view for Commenced Inspection Details
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult QMCommencedInspDetails(string id)
        {
            QualityMonitoringBAL qmBAL = new QualityMonitoringBAL();
            QMCommencedWorkModel model = new QMCommencedWorkModel();
            dbContext = new PMGSYEntities();
            try
            {
                string[] param = id.Split('$');
                model.DURATION = (param[1].Equals("1") ? "Less Than 1 Month" : (param[1].Equals("2"))
                                                          ? "1-3 Month" : (param[1].Equals("3"))
                                                          ? "3-6 Month" : (param[1].Equals("4"))
                                                          ? "6-9 Month" : (param[1].Equals("5"))
                                                          ? "9-12 Month" :
                                                          "More Than 12 Months");
                int stateCode = Convert.ToInt32(param[0]);
                model.STATE_NAME = dbContext.MASTER_STATE.Where(c => c.MAST_STATE_CODE == stateCode).Select(c => c.MAST_STATE_NAME).First();
                model.INSP_LIST = qmBAL.CommencedInspDetailsBAL(stateCode, Convert.ToInt32(param[1]), param[2]);
                return View(model);
            }
            catch
            {
                model.ERROR = "Error occurred while processing your request";
                return View(model);
            }
            finally
            {
                dbContext.Dispose();
            }
        }


        /// <summary>
        /// Commenced Works - view all commencement Details
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult QMCommencedRoadDetails(string id)
        {
            QualityMonitoringBAL qmBAL = new QualityMonitoringBAL();
            QMCommencedWorkModel model = new QMCommencedWorkModel();
            dbContext = new PMGSYEntities();
            try
            {
                string[] param = id.Split('$');
                int stateCode = Convert.ToInt32(param[0]);
                int duration = Convert.ToInt32(param[1]);
                model.DURATION = (param[1].Equals("1") ? "Less Than 1 Month" : (param[1].Equals("2"))
                                                          ? "1-3 Month" : (param[1].Equals("3"))
                                                          ? "3-6 Month" : (param[1].Equals("4"))
                                                          ? "6-9 Month" : (param[1].Equals("5"))
                                                          ? "9-12 Month" :
                                                          "More Than 12 Months");
                model.STATE_NAME = dbContext.MASTER_STATE.Where(c => c.MAST_STATE_CODE == stateCode).Select(c => c.MAST_STATE_NAME).First();
                model.COMMENCEMENT_LIST = qmBAL.CommencedRoadDetailsBAL(stateCode, duration);
                return View(model);
            }
            catch
            {
                model.ERROR = "Error occurred while processing your request";
                return View(model);
            }
            finally
            {
                dbContext.Dispose();
            }
        }


        /// <summary>
        /// Renders date filetrs for Completed Works Report 
        /// </summary>
        /// <returns></returns>
        public ActionResult QMCompletedWorkFilters()
        {
            QMCompletedWorkModel model = new QMCompletedWorkModel();
            model.FROM_DATE = DateTime.Now.ToString("dd/MM/yyyy");
            model.TO_DATE = DateTime.Now.ToString("dd/MM/yyyy");
            return View(model);
        }

        /// <summary>
        /// Renders Completed works report as per selected date
        /// </summary>
        /// <returns></returns>
        public ActionResult QMCompletedWorkDetails()
        {
            QualityMonitoringBAL qmBAL = new QualityMonitoringBAL();
            QMCompletedWorkModel model = new QMCompletedWorkModel();
            try
            {
                model.WORK_LIST = qmBAL.CompletedWorksBAL(Request.Params["frmDate"], Request.Params["toDate"]);
                model.FROM_DATE = Request.Params["frmDate"];
                model.TO_DATE = Request.Params["toDate"];
                return View(model);
            }
            catch
            {
                model.ERROR = "Error occurred while processing your request";
                return View(model);
            }
        }


        /// <summary>
        /// Completed Inspection Details
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult QMCompletedInspDetails(string id)
        {
            QualityMonitoringBAL qmBAL = new QualityMonitoringBAL();
            QMCompletedWorkModel model = new QMCompletedWorkModel();
            try
            {
                string[] param = id.Split('$');
                int roadCode = Convert.ToInt32(param[0]);
                model.INSP_LIST = qmBAL.CompletedInspDetailsBAL(roadCode, param[1]);
                model.WORK_NAME = model.INSP_LIST.Where(c => c.IMS_PR_ROAD_CODE == roadCode).Select(c => c.IMS_ROAD_NAME).First();
                return View(model);
            }
            catch
            {
                model.ERROR = "Error occurred while processing your request";
                return View(model);
            }
        }


        /// <summary>
        /// Layout for Yearly Quarterwise Defective grading Linechart
        /// </summary>
        /// <returns></returns>
        public ActionResult DefectiveGradingReport()
        {
            DefectiveGraphFiltersModel model = new DefectiveGraphFiltersModel();
            CommonFunctions objCommonFunctions = new CommonFunctions();

            if (PMGSYSession.Current.RoleCode == 8)
            {
                model.StateList = new List<SelectListItem>();
                model.StateList.Insert(0, (new SelectListItem { Text = PMGSYSession.Current.StateName, Value = PMGSYSession.Current.StateCode.ToString(), Selected = true }));
            }
            else
            {
                model.StateList = objCommonFunctions.PopulateStates(false);
                model.StateList.Insert(0, new SelectListItem { Text = "All States", Value = "0" });
            }

            model.Year = DateTime.Now.Year;
            model.YearList = objCommonFunctions.PopulateFinancialYear(false).ToList();
            //model.RdStatusList = objCommonFunctions.PopulateRoadStatus();
            //model.RdStatusList.Insert(3, new SelectListItem { Text = "Maintenance", Value = "M" });
            model.RdStatusList = new List<SelectListItem>();
            model.RdStatusList.Insert(0, new SelectListItem { Text = "All", Value = "A" });
            model.RdStatusList.Insert(1, new SelectListItem { Text = "Completed", Value = "C" });
            model.RdStatusList.Insert(2, new SelectListItem { Text = "In Progress", Value = "P" });
            model.RdStatusList.Insert(3, new SelectListItem { Text = "Maintenance", Value = "M" });
            model.ValueType = "V";

            return View(model);
        }

        /// <summary>
        /// Yearly Quarterwise Defective grading Linechart
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Audit]
        public ActionResult DefectiveGradingLineChart()
        {
            QualityMonitoringBAL qmBAL = new QualityMonitoringBAL();
            try
            {
                List<USP_QM_DEFFECTIVE_GRAPH_Result> List = qmBAL.DefectiveGradingLineChartBAL(Convert.ToInt32(Request.Params["state"]), Convert.ToInt32(Request.Params["year"]), Request.Params["rdStatus"], Request.Params["valueType"]);

                List<DefectiveGradingLineChartModel> lstChartYearwise = new List<DefectiveGradingLineChartModel>();

                foreach (var p in List)
                {
                    DefectiveGradingLineChartModel chart1 = new DefectiveGradingLineChartModel();
                    chart1.Quarter = "1";
                    chart1.NQMSRICount = Convert.ToDecimal(p.NQM_SRI_U_QTR1);
                    chart1.NQMUCount = Convert.ToDecimal(p.NQM_U_QTR1);
                    chart1.SQMSRICount = Convert.ToDecimal(p.SQM_SRI_U_QTR1);
                    chart1.SQMUCount = Convert.ToDecimal(p.SQM_U_QTR1);
                    lstChartYearwise.Add(chart1);

                    DefectiveGradingLineChartModel chart2 = new DefectiveGradingLineChartModel();
                    chart2.Quarter = "2";
                    chart2.NQMSRICount = Convert.ToDecimal(p.NQM_SRI_U_QTR2);
                    chart2.NQMUCount = Convert.ToDecimal(p.NQM_U_QTR2);
                    chart2.SQMSRICount = Convert.ToDecimal(p.SQM_SRI_U_QTR2);
                    chart2.SQMUCount = Convert.ToDecimal(p.SQM_U_QTR2);
                    lstChartYearwise.Add(chart2);

                    DefectiveGradingLineChartModel chart3 = new DefectiveGradingLineChartModel();
                    chart3.Quarter = "3";
                    chart3.NQMSRICount = Convert.ToDecimal(p.NQM_SRI_U_QTR3);
                    chart3.NQMUCount = Convert.ToDecimal(p.NQM_U_QTR3);
                    chart3.SQMSRICount = Convert.ToDecimal(p.SQM_SRI_U_QTR3);
                    chart3.SQMUCount = Convert.ToDecimal(p.SQM_U_QTR3);
                    lstChartYearwise.Add(chart3);

                    DefectiveGradingLineChartModel chart4 = new DefectiveGradingLineChartModel();
                    chart4.Quarter = "4";
                    chart4.NQMSRICount = Convert.ToDecimal(p.NQM_SRI_U_QTR4);
                    chart4.NQMUCount = Convert.ToDecimal(p.NQM_U_QTR4);
                    chart4.SQMSRICount = Convert.ToDecimal(p.SQM_SRI_U_QTR4);
                    chart4.SQMUCount = Convert.ToDecimal(p.SQM_U_QTR4);
                    lstChartYearwise.Add(chart4);
                }

                return new JsonResult
                {
                    Data = lstChartYearwise
                };
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return new JsonResult
                {
                    Data = string.Empty
                };
            }
        }

        #endregion


        #region Maintennace_Inspection

        /// <summary>
        /// returns view for adding the inspection details
        /// </summary>
        /// <returns></returns>
        public ActionResult MaintenanceInspection()
        {
            try
            {
                qualityBAL = new QualityMonitoringBAL();
                CommonFunctions objCommon = new CommonFunctions();
                MaintenanceInspectionViewModel model = new MaintenanceInspectionViewModel();
                model.lstDistricts.Insert(0, new SelectListItem { Value = "0", Text = "Select District" });
                model.lstStates = objCommon.PopulateStates(true);
                model.lstYears = new SelectList(objCommon.PopulateFinancialYear(true, true).ToList(), "Value", "Text").ToList();
                model.lstMonitors = qualityBAL.PopulateMaintenanceMonitorsBAL();
                model.lstProposalType.Insert(0, new SelectListItem { Value = "0", Text = "Select Proposal Type" });
                model.lstProposalType.Insert(1, new SelectListItem { Value = "P", Text = "Road" });
                model.lstProposalType.Insert(2, new SelectListItem { Value = "L", Text = "LSB" });
                model.lstProposals.Insert(0, new SelectListItem { Value = "0", Text = "Select Road" });
                model.lstGrades = objCommon.PopulateQualityGrading(false);
                model.lstPackages.Insert(0, new SelectListItem { Value = "0", Text = "Select Package" });
                model.MaxInspectionDate = new DateTime(2014, 05, 31);
                return PartialView(model);
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// post method which recieves the maintenance inspection details 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult MaintenanceInspection(MaintenanceInspectionViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    CommonFunctions objCommon = new CommonFunctions();
                    qualityBAL = new QualityMonitoringBAL();
                    string message = string.Empty;
                    if (objCommon.GetStringToDateTime(model.InspectionDate).Year < model.SanctionYear)
                    {
                        return Json(new { success = false, message = "Sanction Year must be less than or equal to Inspection Date." });
                    }
                    bool status = qualityBAL.SaveMaintenanceInspectionBAL(model, out message);
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
                    return Json(new { success = false, message = "Error occurred while processing your request." });
                }
            }
            catch (Exception)
            {
                return Json(new { success = false, message = new CommonFunctions().FormatErrorMessage(ModelState) });
            }
        }

        /// <summary>
        /// populates road according to the state , district , road type and sanction year
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult PopulateRoads(string id)
        {
            qualityBAL = new QualityMonitoringBAL();

            try
            {
                List<SelectListItem> lstRoads = qualityBAL.PopulateMaintenanceInspectionRoadsBAL(id);

                return Json(lstRoads);
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// returns the length of proposal
        /// </summary>
        /// <param name="proposalCode"></param>
        /// <returns></returns>
        public ActionResult GetProposalLength(int proposalCode)
        {
            try
            {
                qualityBAL = new QualityMonitoringBAL();
                return Json(qualityBAL.GetProposalLengthBAL(proposalCode));
            }
            catch (Exception)
            {
                return null;
            }
        }


        /// <summary>
        /// Return Json of Packages 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult PopulatePackage(string id)
        {
            qualityBAL = new QualityMonitoringBAL();

            try
            {
                List<SelectListItem> lstRoads = qualityBAL.PopulatePackageBAL(id);

                return Json(lstRoads);
            }
            catch (Exception)
            {
                return null;
            }
        }


        /// <summary>
        /// Return Json of Roads as per given Package Id 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult PopulateRoadsByPackage(string id)
        {
            qualityBAL = new QualityMonitoringBAL();

            try
            {
                List<SelectListItem> lstRoads = qualityBAL.PopulateRoadByPackageBAL(id);

                return Json(lstRoads);
            }
            catch (Exception)
            {
                return null;
            }
        }

        #endregion


        #region SSRS  QM Commence, Completed, Phase Progress, Phase Inspection Details --- Added By Rohit Jadhav On 29 th Aug 2014

        public ActionResult QMCommenceInspDetailsLayout()
        {
            CommonFunctions commonFunctions = new CommonFunctions();
            QMDetailsViewModel qmDetails = new QMDetailsViewModel();

            if (PMGSYSession.Current.RoleCode == 8)
            {
                qmDetails.lstStates = new List<SelectListItem>();
                qmDetails.lstStates.Insert(0, (new SelectListItem { Text = PMGSYSession.Current.StateName, Value = PMGSYSession.Current.StateCode.ToString(), Selected = true }));
            }
            else
            {
                qmDetails.lstStates = commonFunctions.PopulateStates(false);
                qmDetails.lstStates.Insert(0, (new SelectListItem { Text = "Select State", Value = "0", Selected = true }));
            }


            return View(qmDetails);
        }

        [HttpPost]
        public ActionResult QMCommenceInspDetailsReport(QMDetailsViewModel model)
        {
            try
            {
                if (model.StateCode == 0 || model.StateCode > 0)
                {
                    model.LevelID = 1;
                }
                else
                {
                    model.LevelID = 2;

                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return View(model);

        }

        public ActionResult QMCompletedInspDetailsLayout()
        {
            CommonFunctions commonFunctions = new CommonFunctions();
            QMCompletedInspDetailsViewModel qmcDetails = new QMCompletedInspDetailsViewModel();
            qmcDetails.StateCode = 0;
            if (PMGSYSession.Current.RoleCode == 8)
            {
                qmcDetails.lstStates = new List<SelectListItem>();
                qmcDetails.lstStates.Insert(0, (new SelectListItem { Text = PMGSYSession.Current.StateName, Value = PMGSYSession.Current.StateCode.ToString(), Selected = true }));
            }
            else
            {
                qmcDetails.lstStates = commonFunctions.PopulateStates(false);
                qmcDetails.lstStates.Insert(0, (new SelectListItem { Text = "Select State", Value = "0", Selected = true }));
            }

            return View(qmcDetails);
        }

        [HttpPost]
        public ActionResult QMCompletedInspDetailsReport(QMCompletedInspDetailsViewModel qmmodel)
        {
            try
            {
                if (qmmodel.StateCode == 0 || qmmodel.StateCode > 0)
                {
                    qmmodel.LevelID = 1;
                }
                else
                {
                    qmmodel.LevelID = 2;

                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return View(qmmodel);

        }

        public ActionResult QMPhaseProgressLayout()
        {
            CommonFunctions commonFunctions = new CommonFunctions();
            QMPahseProgressViewModel qmphase = new QMPahseProgressViewModel();
            if (PMGSYSession.Current.RoleCode == 8)
            {
                qmphase.lstStates = new List<SelectListItem>();
                qmphase.lstStates.Insert(0, (new SelectListItem { Text = PMGSYSession.Current.StateName, Value = PMGSYSession.Current.StateCode.ToString(), Selected = true }));
            }
            else
            {
                qmphase.lstStates = commonFunctions.PopulateStates(false);
                qmphase.lstStates.Insert(0, (new SelectListItem { Text = "Select State", Value = "0", Selected = true }));
            }

            return View(qmphase);
        }

        [HttpPost]
        public ActionResult QMPhaseProgressReport(QMPahseProgressViewModel qmphaseProgress)
        {
            try
            {
                if (qmphaseProgress.StateCode == 0 || qmphaseProgress.StateCode > 0)
                {
                    qmphaseProgress.LevelID = 1;
                }
                else
                {
                    qmphaseProgress.LevelID = 2;

                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return View(qmphaseProgress);

        }

        public ActionResult QMPhaseInspectionViewModelLayout()
        {
            QMPhaseInspectionViewModel qmPhaseInspectionViewModel = new QMPhaseInspectionViewModel();
            return View(qmPhaseInspectionViewModel);
        }

        public ActionResult QMPhaseInspectionViewModelReport(QMPhaseInspectionViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    model.Mast_State_Code = model.StateCode > 0 ? model.StateCode : model.Mast_State_Code;
                    model.Mast_District_Code = model.DistrictCode > 0 ? model.DistrictCode : model.Mast_District_Code;
                    model.Mast_Block_Code = model.BlockCode > 0 ? model.BlockCode : model.Mast_Block_Code;
                    //model.Mast_Funding_Agency_Code=model.FundingAgencyCode> 0 ? model.FundingAgencyCode : model.Mast_Funding_Agency_Code;
                    model.Mast_Agency_Code = model.AgencyCode > 0 ? model.AgencyCode : model.Mast_Agency_Code;

                    model.qmType = "0";
                    model.RoadStatus = "0";
                    model.Status = "0";

                    return View("QMPhaseInspectionViewModelReport", model);
                }
                else
                {
                    return View("QMPhaseInspectionViewModelReport", model);
                }
            }
            catch
            {
                return View(model);
            }
        }


        // Common Functions 
        // Added By Rohit Jadhav On 1 Sept 2014
        public ActionResult DistrictDetails(FormCollection frmCollection)
        {
            CommonFunctions objCommonFunctions = new CommonFunctions();
            List<SelectListItem> list = objCommonFunctions.PopulateDistrict(Convert.ToInt32(frmCollection["StateCode"]), true);
            list.Find(x => x.Value == "-1").Value = "0";
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public ActionResult BlockDetails(FormCollection frmCollection)
        {
            CommonFunctions objCommonFunctions = new CommonFunctions();
            List<SelectListItem> list = objCommonFunctions.PopulateBlocks(Convert.ToInt32(frmCollection["DistrictCode"]), true);
            list.Find(x => x.Value == "-1").Value = "0";
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public ActionResult DistrictSelectDetails(FormCollection frmCollection)
        {
            CommonFunctions objCommonFunctions = new CommonFunctions();
            List<SelectListItem> list = objCommonFunctions.PopulateDistrict(Convert.ToInt32(frmCollection["StateCode"]), false);
            // list.Find(x => x.Value == "-1").Value = "0";
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public ActionResult BlockSelectDetails(FormCollection frmCollection)
        {
            CommonFunctions objCommonFunctions = new CommonFunctions();
            List<SelectListItem> list = objCommonFunctions.PopulateBlocks(Convert.ToInt32(frmCollection["DistrictCode"]), false);
            //list.Find(x => x.Value == "-1").Value = "0";
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public ActionResult AgencyDetails(FormCollection frmCollection)
        {
            CommonFunctions objCommonFunctions = new CommonFunctions();
            List<SelectListItem> list = objCommonFunctions.PopulateAgencies(Convert.ToInt32(frmCollection["StateCode"]), true);
            // list.Find(x => x.Value == "-1").Value = "0";
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public ActionResult AgencySelectDetails(FormCollection frmCollection)
        {
            CommonFunctions objCommonFunctions = new CommonFunctions();
            List<SelectListItem> list = objCommonFunctions.PopulateAgencies(Convert.ToInt32(frmCollection["StateCode"]), false);
            // list.Find(x => x.Value == "-1").Value = "0";
            return Json(list, JsonRequestBehavior.AllowGet);
        }


        public ActionResult AllotmentDutiesOneLayout()
        {
            QMAllotmentDutiesFormatOne allotmentDuties = new QMAllotmentDutiesFormatOne();
            CommonFunctions commonFunctions = new CommonFunctions();

            allotmentDuties.Year = DateTime.Now.Year;
            allotmentDuties.YearList = commonFunctions.PopulateYears(DateTime.Now.Year);

            allotmentDuties.MonthList = commonFunctions.PopulateMonths(DateTime.Now.Month);
            allotmentDuties.Month = DateTime.Now.Month;

            return View(allotmentDuties);
        }
        public ActionResult AllotmentDutiesOneReport(QMAllotmentDutiesFormatOne allotmentDuties)
        {

            try
            {
                if (ModelState.IsValid)
                {
                    return View("AllotmentDutiesOneReport", allotmentDuties);
                }
                else
                {
                }
            }
            catch (Exception)
            {
                return null;
            }


            return View();
        }




        #endregion


        /// <summary>
        /// Overriden Json for MaxJsonLength
        /// </summary>
        /// <param name="data"></param>
        /// <param name="contentType"></param>
        /// <param name="contentEncoding"></param>
        /// <param name="behavior"></param>
        /// <returns></returns>
        /// 
        protected override JsonResult Json(object data, string contentType, System.Text.Encoding contentEncoding, JsonRequestBehavior behavior)
        {
            return new JsonResult()
            {
                Data = data,
                ContentType = contentType,
                ContentEncoding = contentEncoding,
                JsonRequestBehavior = behavior,
                MaxJsonLength = Int32.MaxValue
            };
        }


        #region LabDetails --- developed by Anand Singh (Integrated on 09/09/2014 by Shyam Yadav)


        [Audit]
        public ActionResult LabDetail()
        {

            return View();

        }

        [Audit]
        public ActionResult LabDetailSQC()
        {

            return View();

        }

        [Audit]
        public ActionResult LabTest()
        {

            return View();

        }
        [Audit]
        public ActionResult LabPositoinGoogleMap()
        {

            return View();

        }
        [Audit]
        public ActionResult AddLabCommencementDateDetails(string stateName, string imsYear, string packageId, string dateOfAwardWork, string dateofCommencement, string dateofCompletion, int agreementCode)
        {
            dbContext = new PMGSYEntities();
            CommonFunctions objCommon = new CommonFunctions();
            try
            {

                LabDateViewModel labdateViewModel = new LabDateViewModel();
                TEND_AGREEMENT_MASTER tendAgeement = dbContext.TEND_AGREEMENT_MASTER.Where(a => a.TEND_AGREEMENT_CODE == agreementCode).FirstOrDefault();
                labdateViewModel.hdCommencementDate = tendAgeement.TEND_DATE_OF_COMMENCEMENT == null ? "01/01/1900" : objCommon.GetDateTimeToString(Convert.ToDateTime(tendAgeement.TEND_DATE_OF_COMMENCEMENT));
                labdateViewModel.hdAgreementDate = tendAgeement.TEND_DATE_OF_AGREEMENT == null ? "01/01/1900" : objCommon.GetDateTimeToString(Convert.ToDateTime(tendAgeement.TEND_DATE_OF_AGREEMENT));
                labdateViewModel.Agreement_Date = tendAgeement.TEND_DATE_OF_AGREEMENT == null ? "01 JAN 1900" : tendAgeement.TEND_DATE_OF_AGREEMENT.ToString("dd MMM yyyy");
                labdateViewModel.State_Name = stateName;
                labdateViewModel.Sanction_Year = imsYear;
                // labdateViewModel.Current_Date = DateTime.Now.ToString();
                labdateViewModel.Package = packageId;
                labdateViewModel.Date_Of_Award = dateOfAwardWork;
                labdateViewModel.Commencemend_Date = dateofCommencement;
                labdateViewModel.Completed_Date = dateofCompletion;
                labdateViewModel.Agreement_No = agreementCode;


                return View(labdateViewModel);

            }
            catch
            {
                return View(new TestResultViewModel());
            }
            finally
            {
                if (dbContext != null)
                {
                    dbContext.Dispose();
                }
            }
        }
        [Audit]
        public ActionResult LabPositoinGoogleMapTest(string id)
        {

            qualityBAL = new QualityMonitoringBAL();
            try
            {
                QMFillObservationModel fillObservationModel = qualityBAL.QMObservationDetailsBAL(Convert.ToInt32(id));
                fillObservationModel.QM_OBSERVATION_ID = Convert.ToInt32(id);
                return View(fillObservationModel);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return null;
            }

        }

        [Audit]
        public JsonResult LabDetailSave(int agreementCode, string packageId, string labEshtablishedDate)
        {
            string message = string.Empty;
            qualityBAL = new QualityMonitoringBAL();

            if (qualityBAL.LabDetailSave(agreementCode, packageId, labEshtablishedDate, ref message))
            {
                message = message == string.Empty ? "Lab detail saved successfully." : message;
                return Json(new { success = true, message = message });
            }
            else
            {
                message = message == string.Empty ? "Lab detail not saved." : message;
                return Json(new { success = false, message = message });
            }

            return Json(new { success = false, message = message });


        }

        [Audit]
        public JsonResult LabDetailDeleteFinalize(int id, string type)
        {
            string message = string.Empty;
            qualityBAL = new QualityMonitoringBAL();

            if (type == "PLD")
            {
                if (qualityBAL.LabDetailDeleteFinalizeBAL(id, type, ref message))
                {
                    message = message == string.Empty ? "Lab detail deleted successfully." : message;
                    return Json(new { success = true, message = message });
                }
                else
                {
                    message = message == string.Empty ? "Lab detail not deleted." : message;
                    return Json(new { success = false, message = message });
                }
            }
            else if (type == "PLF" || type == "SLF")
            {
                if (qualityBAL.LabDetailDeleteFinalizeBAL(id, type, ref message))
                {
                    message = message == string.Empty ? "Lab detail finalized successfully." : message;
                    return Json(new { success = true, message = message });
                }
                else
                {
                    message = message == string.Empty ? "Lab detail not finalized." : message;
                    return Json(new { success = false, message = message });
                }
            }

            return Json(new { success = true, message = message });
            //return Json(message, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// Add Test result details
        /// </summary>
        /// <param name="proposalAdditionalCostModel"></param>
        /// <returns></returns>
        [HttpPost]
        [Audit]
        [ValidateAntiForgeryToken]
        public ActionResult AddLabSaveDetails(LabDateViewModel labDateViewModel)
        {
            string message = string.Empty;
            qualityBAL = new QualityMonitoringBAL();

            try
            {
                if (ModelState.IsValid)
                {

                    if (qualityBAL.AddLABSaveDetailsBAL(labDateViewModel, ref message))
                    {
                        message = message == string.Empty ? "Lab details saved successfully." : message;
                        return Json(new { success = true, message = message });
                    }
                    else
                    {
                        message = message == string.Empty ? "Lab details not saved." : message;
                        return Json(new { success = false, message = message });
                    }
                }
                else
                {

                    string messages = string.Join("; ", ModelState.Values
                                           .SelectMany(x => x.Errors)
                                           .Select(x => x.ErrorMessage));

                    return PartialView("AddLabCommencementDateDetails", labDateViewModel);
                }
            }
            catch (DbEntityValidationException ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                foreach (var eve in ex.EntityValidationErrors)
                {
                    ModelState.AddModelError("", eve.ValidationErrors.ToString());
                    message += eve.ValidationErrors.ToString();
                }
                return Json(new { success = false, message = message }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                message = "Additional Cost details not saved because " + ex.Message;
                return Json(new { success = false, message = message }, JsonRequestBehavior.AllowGet);
            }
        }


        [Audit]
        [HttpPost]
        public JsonResult GetLabDetailList(FormCollection formCollection)
        {
            long totalRecords;
            qualityBAL = new QualityMonitoringBAL();
            try
            {
                var arr = qualityBAL.GetPIU1TierLabDetailListBAL(PMGSYSession.Current.StateCode, PMGSYSession.Current.DistrictCode, formCollection["level"], out totalRecords);
                var jsonData = new
                {
                    rows = arr,
                    total = totalRecords <= Convert.ToInt32(formCollection["rows"]) ? 1 : (totalRecords % Convert.ToInt32(formCollection["rows"]) == 0 ? totalRecords / Convert.ToInt32(formCollection["rows"]) : (totalRecords / Convert.ToInt32(formCollection["rows"]) + 1)),
                    page = Convert.ToInt32(formCollection["page"]),
                    records = totalRecords
                };

                return Json(jsonData, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return Json(String.Empty);
            }
        }

        [HttpGet]
        [Audit]
        public ActionResult LabImageUpload(int id)
        {
            dbContext = new PMGSYEntities();
            try
            {
                PMGSY.Models.QualityMonitoring.LabFileUploadViewModel labfileUploadViewModel = new PMGSY.Models.QualityMonitoring.LabFileUploadViewModel();

                labfileUploadViewModel.QM_LAB_ID = id;
                QUALITY_QM_LAB_MASTER qm_LabMaster_Details = dbContext.QUALITY_QM_LAB_MASTER.Where(a => a.QM_LAB_ID == id).FirstOrDefault();
                labfileUploadViewModel.QM_SQC_APPROVAL = qm_LabMaster_Details.QM_SQC_APPROVAL;
                labfileUploadViewModel.QM_LOCK_STATUS = qm_LabMaster_Details.QM_LOCK_STATUS;

                if (dbContext.QUALITY_QM_LAB_DETAILS.Where(a => a.QM_LAB_ID == id).Any())
                {
                    labfileUploadViewModel.NumberofFiles = dbContext.QUALITY_QM_LAB_DETAILS.Where(a => a.QM_LAB_ID == id).Count();
                }
                else
                {
                    labfileUploadViewModel.NumberofFiles = 0;
                }

                return View(labfileUploadViewModel);
            }
            catch (Exception)
            {

                throw;
            }
            finally
            {
                if (dbContext != null)
                {
                    dbContext.Dispose();
                }
            }

        }


        /// <summary>
        /// Post Method for Lab Uploading IMAGE File
        /// </summary>
        /// <param name="fileUploadViewModel"></param>
        /// <returns></returns>
        [HttpPost]
        [AcceptVerbs(HttpVerbs.Post)]
        [Audit]
        public ActionResult LabUploads(PMGSY.Models.QualityMonitoring.LabFileUploadViewModel labFileUploadViewModel)
        {
            //Added By Abhishek kamble 26-Apr-2014 to validate File Type
            CommonFunctions objCommonFunc = new CommonFunctions();
            //Array of File Types to Validate             
            String[] fileTypes = new String[] { "jpeg", String.Empty, String.Empty };
            String StorageRoot = string.Empty;

            ///Path to upload files for NQM/SQM/CQC/SQC
            //if (PMGSYSession.Current.RoleCode == 5 || PMGSYSession.Current.RoleCode == 6 || PMGSYSession.Current.RoleCode == 9)   //CQCAdmin or NQM or CQC
            //{
            StorageRoot = ConfigurationManager.AppSettings["QUALITY_LAB_FILE_UPLOAD"];
            //}

            if (!(objCommonFunc.IsValidImageFile(StorageRoot, Request, fileTypes)))
            {
                labFileUploadViewModel.ErrorMessage = "File Type is Not Allowed.";
                return View("ImageUpload", labFileUploadViewModel.ErrorMessage);
            }

            qualityBAL = new QualityMonitoringBAL();
            ////Apply Later

            foreach (string file in Request.Files)
            {
                string status = ValidateImageFile(Request.Files[0].ContentLength, Path.GetExtension(Request.Files[0].FileName));
                if (status != string.Empty)
                {
                    labFileUploadViewModel.ErrorMessage = status;
                    return View("ImageUpload", labFileUploadViewModel.ErrorMessage);
                }
            }

            var fileData = new List<PMGSY.Models.QualityMonitoring.LabFileUploadViewModel>();


            int QM_LAB_ID = 0;
            if (labFileUploadViewModel.QM_LAB_ID == 0)
            {
                QM_LAB_ID = labFileUploadViewModel.QM_LAB_ID;
            }
            else
            {
                try
                {
                    QM_LAB_ID = Convert.ToInt32(Request["QM_LAB_ID"]);

                }
                catch (Exception ex)
                {
                    Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                    if (Request["QM_LAB_ID"].Contains(','))
                    {
                        QM_LAB_ID = Convert.ToInt32(Request["QM_LAB_ID"].Split(',')[0]);
                    }

                }
            }

            foreach (string file in Request.Files)
            {
                UploadLabImageFile(Request, fileData, QM_LAB_ID);
            }
            var serializer = new JavaScriptSerializer();
            serializer.MaxJsonLength = Int32.MaxValue;

            var result = new ContentResult
            {
                Content = "{\"files\":" + serializer.Serialize(fileData) + "}",
            };
            return result;
        }

        //Lab Upload
        [Audit]
        public void UploadLabImageFile(HttpRequestBase request, List<PMGSY.Models.QualityMonitoring.LabFileUploadViewModel> statuses, int labId)
        {
            dbContext = new PMGSYEntities();
            qualityBAL = new QualityMonitoringBAL();
            String StorageRoot = string.Empty;
            int MaxFileID = 0;
            try
            {
                for (int i = 0; i < request.Files.Count; i++)
                {
                    HttpPostedFileBase file = request.Files[i];

                    if (dbContext.QUALITY_QM_LAB_DETAILS.Count() == 0)
                    {
                        MaxFileID = 0;
                    }
                    else
                    {
                        MaxFileID = (from c in dbContext.QUALITY_QM_LAB_DETAILS
                                     select (Int32?)c.QM_LAB_FILE_ID ?? 0).Max();
                    }

                    Int32 countOfFilesUploaded = (from c in dbContext.QUALITY_QM_LAB_DETAILS
                                                  where c.QM_LAB_ID == labId
                                                  select c).Count();

                    if (countOfFilesUploaded == 0)
                    {
                        countOfFilesUploaded = 1;
                    }
                    else
                    {
                        countOfFilesUploaded = countOfFilesUploaded + 1;
                    }

                    var fileId = MaxFileID + 1;


                    //var fileNameWithoutExt = Path.GetFileNameWithoutExtension(request.Files[i].FileName).ToString();
                    var fileName = fileId + "_" + countOfFilesUploaded + Path.GetExtension(request.Files[i].FileName).ToString();

                    ///Path to upload files for NQM/SQM/CQC/SQC
                    //if (PMGSYSession.Current.RoleCode == 5 || PMGSYSession.Current.RoleCode == 6 || PMGSYSession.Current.RoleCode == 9)   //CQCAdmin or NQM or CQC
                    //{
                    StorageRoot = ConfigurationManager.AppSettings["QUALITY_LAB_FILE_UPLOAD"];
                    //}


                    var fullPath = Path.Combine(StorageRoot, fileName);

                    var ThumbnailPath = Path.Combine(StorageRoot, "thumbnails");
                    var FullThumbnailPath = Path.Combine(ThumbnailPath, fileName);

                    if (!(Directory.Exists(ThumbnailPath)))
                    {
                        Directory.CreateDirectory(ThumbnailPath);
                    }

                    statuses.Add(new PMGSY.Models.QualityMonitoring.LabFileUploadViewModel()
                    {
                        QM_LAB_ID = labId,
                        url = fullPath,
                        thumbnail_url = fullPath,
                        name = fileName,
                        type = file.ContentType,
                        size = file.ContentLength,
                        delete_url = "",
                        delete_type = "DELETE",
                        Image_Description = request.Params["remark[]"],  //.Split(',')[i]
                        Latitude = Convert.ToDecimal(request.Params["latitude[]"]),
                        Longitude = Convert.ToDecimal(request.Params["longitude[]"]),

                    });

                    string status = qualityBAL.AddLabFileUploadDetailsBAL(statuses);
                    if (status == string.Empty)
                    {
                        CompressImage(request.Files[0], fullPath, FullThumbnailPath);
                    }
                    else
                    {
                        // show an error over here
                    }
                }
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        /// <summary>
        /// Post to Update Image Description , Latitude and Longitude
        /// </summary>
        /// <param name="formCollection"></param>
        /// <returns></returns>
        [HttpPost]
        [Audit]
        public JsonResult UpdateLabImageDetails(FormCollection formCollection)
        {
            qualityBAL = new QualityMonitoringBAL();
            PMGSY.Models.QualityMonitoring.LabFileUploadViewModel fileuploadViewModel = new PMGSY.Models.QualityMonitoring.LabFileUploadViewModel();
            fileuploadViewModel.QM_FILE_ID = Convert.ToInt32(formCollection["id"]);
            try
            {
                Regex regex = new Regex(@"^[a-zA-Z0-9 ,.()-]+$");
                if (string.IsNullOrEmpty(formCollection["Description"]))
                {
                    return Json("Invalid Description, Only Alphabets, Numbers and  [,.()-] value are allowed");
                }
                else if (regex.IsMatch(formCollection["Description"]))
                {
                    fileuploadViewModel.Image_Description = formCollection["Description"];
                }
                else
                {
                    return Json("Invalid Description, Only Alphabets, Numbers and  [,.()-] value are allowed");
                }

                Regex regexNumberDecimal = new Regex(@"^[0-9]{1,6}(\.[0-9]{1,12})?$");
                if (string.IsNullOrEmpty(formCollection["Latitude"]))
                {
                    return Json("Invalid Latitude, Can only contains Numeric values and 12 digit after decimal place.");
                }
                else if (regexNumberDecimal.IsMatch(formCollection["Latitude"]))
                {
                    fileuploadViewModel.Latitude = Convert.ToDecimal(formCollection["Latitude"]);
                }
                else
                {
                    return Json("Invalid Latitude, Can only contains Numeric values and 12 digit after decimal place.");
                }

                if (string.IsNullOrEmpty(formCollection["Longitude"]))
                {
                    return Json("Invalid Longitude, Can only contains Numeric values and 12 digit after decimal place.");
                }
                else if (regexNumberDecimal.IsMatch(formCollection["Longitude"]))
                {
                    fileuploadViewModel.Longitude = Convert.ToDecimal(formCollection["Longitude"]);
                }
                else
                {
                    return Json("Invalid Longitude, Can only contains Numeric values and 12 digit after decimal place.");
                }

            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return Json("Please Enter Valid Description, Latitude, Longitude.");
            }
            string status = qualityBAL.UpdateLabImageDetailsBAL(fileuploadViewModel);

            if (status == string.Empty)
                return Json(true);
            else
                return Json("Error Occured While Processing Your Request.");
        }
        [Audit]
        public JsonResult ListLabFiles(FormCollection formCollection)
        {
            dbContext = new PMGSYEntities();
            qualityBAL = new QualityMonitoringBAL();
            try
            {
                //Adde By Abhishek kamble 29-Apr-2014 start
                using (CommonFunctions commonFunction = new CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new GridParams(Convert.ToInt32(formCollection["page"]), Convert.ToInt32(formCollection["rows"]), formCollection["sidx"], formCollection["sord"], Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                    {
                        return null;
                    }
                }
                //Adde By Abhishek kamble 29-Apr-2014 end
                Int32 labId = 0;
                string requestLabId = Request.Params["QM_LAB_ID"];
                if (requestLabId != null && requestLabId != "0" && requestLabId != "" && !requestLabId.Equals(string.Empty))
                {
                    labId = Convert.ToInt32(Request.Params["QM_LAB_ID"]);
                }


                int totalRecords;
                var jsonData = new
                {
                    rows = qualityBAL.GetLabFilesListBAL(Convert.ToInt32(formCollection["page"]) - 1, Convert.ToInt32(formCollection["rows"]), formCollection["sidx"], formCollection["sord"], out totalRecords, labId),
                    total = totalRecords <= Convert.ToInt32(formCollection["rows"]) ? 1 : (totalRecords % Convert.ToInt32(formCollection["rows"]) == 0 ? totalRecords / Convert.ToInt32(formCollection["rows"]) : (totalRecords / Convert.ToInt32(formCollection["rows"]) + 1)),
                    page = Convert.ToInt32(formCollection["page"]),
                    records = totalRecords
                };
                return Json(jsonData);
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

        public ActionResult GetMasterQualityMonitorList(int? page, int? rows, string sidx, string sord)
        {
            String searchParameters = String.Empty;
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            long totalRecords;
            int stateCode = 0;
            int districtCode = 0;
            string isEmpanelled = String.Empty;
            string firstName = String.Empty;
            string qmTypeName = String.Empty;

            try
            {
                PMGSY.BAL.Master.MasterBAL objBAL = new PMGSY.BAL.Master.MasterBAL();
                using (CommonFunctions commonFunction = new CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new GridParams(page, rows, sidx, sord, Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                    {
                        return null;
                    }
                }

                if (!string.IsNullOrEmpty(Request.Params["QmTypeName"]))
                {
                    qmTypeName = Request.Params["QmTypeName"].Replace('+', ' ').Trim();
                }

                if (!string.IsNullOrEmpty(Request.Params["stateCode"]))
                {
                    stateCode = Convert.ToInt32(Request.Params["stateCode"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["districtCode"]))
                {
                    districtCode = Convert.ToInt32(Request.Params["districtCode"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["isEmpanelled"]))
                {
                    isEmpanelled = Request.Params["isEmpanelled"];
                }


                var jsonData = new
                {
                    rows = objBAL.ListQualityMonitor(qmTypeName, stateCode, districtCode, isEmpanelled, Request.Params["filters"], Convert.ToInt32(page) - 1, Convert.ToInt32(rows), sidx, sord, out totalRecords),
                    total = totalRecords <= Convert.ToInt32(rows) ? 1 : totalRecords / Convert.ToInt32(rows) + 1,
                    page = Convert.ToInt32(page),
                    records = totalRecords
                };
                return Json(jsonData, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return null;
            }
        }

        [HttpPost]
        [Audit]
        public JsonResult DeleteLabImagesDetails()
        {
            String PhysicalPath = string.Empty;
            String ThumbnailPath = string.Empty;
            dbContext = new PMGSYEntities();
            qualityBAL = new QualityMonitoringBAL();

            try
            {
                Dictionary<string, string> decryptedParameters = null;
                string[] encryptedParams = Request.Params["QM_LAB_DETAIL_ID"].Split('/');
                decryptedParameters = URLEncrypt.DecryptParameters1(new string[] { encryptedParams[0], encryptedParams[1], encryptedParams[2] });
                int QM_LAB_ID = Convert.ToInt32(decryptedParameters["QM_LAB_ID"]);
                int QM_LAB_FILE_ID = Convert.ToInt32(decryptedParameters["QM_LAB_FILE_ID"]);
                string QM_FILE_NAME = decryptedParameters["QM_LAB_FILE_NAME"];


                ///Path to upload files for NQM/SQM/CQC/SQC

                PhysicalPath = ConfigurationManager.AppSettings["QUALITY_LAB_FILE_UPLOAD"];

                ThumbnailPath = Path.Combine(Path.Combine(PhysicalPath, "thumbnails"), QM_FILE_NAME);

                PhysicalPath = Path.Combine(PhysicalPath, QM_FILE_NAME);

                if (!System.IO.File.Exists(PhysicalPath) && !System.IO.File.Exists(ThumbnailPath))
                {
                    return Json(new { Success = false, ErrorMessage = "File Not Found." });
                }

                string status = qualityBAL.DeleteLabFileDetailsBAL(QM_LAB_FILE_ID);

                if (status == string.Empty)
                {
                    try
                    {
                        System.IO.File.Delete(PhysicalPath);
                        System.IO.File.Delete(ThumbnailPath);
                    }
                    catch (Exception ex)
                    {
                        Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);

                        return Json(new { Success = true, ErrorMessage = ex.Message });
                    }
                    return Json(new { Success = true, ErrorMessage = "Error Ocurred While Processing Your Request." });
                }
                else
                {
                    return Json(new { Success = false, ErrorMessage = "Image details not deleted." });
                }
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);

                return Json(new { Success = false, ErrorMessage = "Error Ocurred While Processing Your Request." });
            }
            finally
            {
                dbContext.Dispose();
            }
        }
        #endregion


        #region MP Visit

        public ActionResult QMMPVisit()
        {
            QMMPVisitModel model = new QMMPVisitModel();
            CommonFunctions objCommon = new CommonFunctions();
            if (PMGSYSession.Current.RoleCode == 22 || PMGSYSession.Current.RoleCode == 38 || PMGSYSession.Current.RoleCode == 54) //PIU OR PIUOA or PIURCPLWE
            {
                //model.StateList = objCommon.PopulateStates(false);
                //model.DistrictList = objCommon.PopulateDistrict(PMGSYSession.Current.StateCode, true);
                model.StateCode = PMGSYSession.Current.StateCode;
                model.StateName = PMGSYSession.Current.StateName;
                model.DistrictCode = PMGSYSession.Current.DistrictCode;
                model.DistrictName = PMGSYSession.Current.DistrictName;
                model.BlockList = objCommon.PopulateBlocks(PMGSYSession.Current.DistrictCode, true);
            }
            else if (PMGSYSession.Current.RoleCode == 2 || PMGSYSession.Current.RoleCode == 37 || PMGSYSession.Current.RoleCode == 36) //SRRDA or SRRDAOA or ITNO
            {
                model.StateCode = PMGSYSession.Current.StateCode;
                model.StateName = PMGSYSession.Current.StateName;
                model.DistrictList = objCommon.PopulateDistrict(PMGSYSession.Current.StateCode, true);
                model.BlockList = objCommon.PopulateBlocks(0, true);
            }

            return View(model);
        }


        /// <summary>
        /// Populate Blocks of selected District
        /// </summary>
        /// <param name="selectedDistrict"></param>
        /// <returns></returns>
        [Audit]
        public ActionResult PopulateBlocks(int selectedDistrict)
        {
            dbContext = new PMGSYEntities();
            CommonFunctions objCommon = new CommonFunctions();
            try
            {
                return Json(objCommon.PopulateBlocks(selectedDistrict, true), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return null;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        /// <summary>
        /// Listing of Roads in selcted Districts & Sanction Year to be assigned
        /// </summary>
        /// <param name="formCollection"></param>
        /// <param name="districtCode"></param>
        /// <param name="sanctionYear"></param>
        /// <param name="rdStatus"></param>
        /// <param name="adminSchCode"></param>
        /// <returns></returns>
        [HttpPost]
        [Audit]
        public ActionResult GetRoadListForMPVisit(FormCollection formCollection)
        {
            qualityBAL = new QualityMonitoringBAL();
            int totalRecords;
            try
            {
                //Adde By Abhishek kamble 29-Apr-2014 start
                using (CommonFunctions commonFunction = new CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new GridParams(Convert.ToInt32(formCollection["page"]), Convert.ToInt32(formCollection["rows"]), formCollection["sidx"], formCollection["sord"], Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                    {
                        return null;
                    }
                }
                int stateCode = 0;
                int districtCode = 0;
                int blockCode = 0;

                if (!(string.IsNullOrEmpty(formCollection["stateCode"])))
                {
                    stateCode = Convert.ToInt32(formCollection["stateCode"]);
                }

                if (!(string.IsNullOrEmpty(formCollection["districtCode"])))
                {
                    districtCode = Convert.ToInt32(formCollection["districtCode"]);
                }

                if (!(string.IsNullOrEmpty(formCollection["blockCode"])))
                {
                    blockCode = Convert.ToInt32(formCollection["blockCode"]);
                }

                var jsonData = new
                {
                    rows = qualityBAL.GetRoadListForMPVisitBAL(Convert.ToInt32(formCollection["page"]) - 1, Convert.ToInt32(formCollection["rows"]),
                                                            formCollection["sidx"], formCollection["sord"], out totalRecords, formCollection["filters"],
                                                            stateCode, districtCode, blockCode),
                    total = totalRecords <= Convert.ToInt32(formCollection["rows"]) ? 1 : (totalRecords % Convert.ToInt32(formCollection["rows"]) == 0 ? totalRecords / Convert.ToInt32(formCollection["rows"]) : (totalRecords / Convert.ToInt32(formCollection["rows"]) + 1)),
                    page = Convert.ToInt32(formCollection["page"]),
                    records = totalRecords
                };
                return Json(jsonData);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return Json(String.Empty);
            }
        }


        /// <summary>
        /// Get To Fill MP Visit Details
        /// </summary>
        /// <returns></returns>
        /// 
        public ActionResult FillMPVisitDetails(string id)
        {
            FillMPVisitModel model = new FillMPVisitModel();
            CommonFunctions objCommon = new CommonFunctions();
            qualityBAL = new QualityMonitoringBAL();
            //string[] Array = id.Split('$');

            model.Operation = "A";
            model.PrRoadCode = Convert.ToInt32(id);
            //Call GetBlockCode method to get the Block Code on giving PR Road Code.
            model.ConstituencyList = objCommon.PopulateMPConstituency(qualityBAL.GetBlockCode(model.PrRoadCode));

            if (PMGSYSession.Current.RoleCode == 22 || PMGSYSession.Current.RoleCode == 38) //PIU OR PIUOA
            {
                model.PIUNameCode = PMGSYSession.Current.ParentNDCode.Value;
                model.PIUName = PMGSYSession.Current.DepartmentName.ToString();
            }
            else if (PMGSYSession.Current.RoleCode == 2 || PMGSYSession.Current.RoleCode == 37 || PMGSYSession.Current.RoleCode == 36) //SRRDA or SRRDAOA or ITNO
            {
                model.PIUNameList = objCommon.PopulateDPIUOfSRRDA(PMGSY.Extensions.PMGSYSession.Current.ParentNDCode.Value);
            }

            IMS_SANCTIONED_PROJECTS RoadDetails = qualityBAL.GetRoadDetailsBAL(model.PrRoadCode);
            model.BlockName = RoadDetails.MASTER_BLOCK.MAST_BLOCK_NAME;
            model.PackageName = RoadDetails.IMS_PACKAGE_ID;
            model.SanctionYear = RoadDetails.IMS_YEAR.ToString() + "-" + (RoadDetails.IMS_YEAR + 1).ToString();
            model.RoadName = RoadDetails.IMS_ROAD_NAME;

            return View(model);
        }


        //Save MP Visit Details
        [HttpPost]
        public ActionResult AddMPVisitDetails(FillMPVisitModel model)
        {
            bool status = false;
            String message = String.Empty;
            CommonFunctions objCommon = new CommonFunctions();
            try
            {
                qualityBAL = new QualityMonitoringBAL();

                if (ModelState.IsValid)
                {
                    //if (model.Remarks.Trim().Equals(String.Empty))
                    //{
                    //    ModelState.AddModelError("Remarks","Invalid Remarks.Can be AlphaNumeric,Can Contain ._@,&()-");
                    //    return View("FillMPVisitDetails", model);
                    //}

                    if (qualityBAL.AddMPVisitDetails(model, ref message))
                    {
                        message = message == string.Empty ? "MP Visit details saved successfully." : message;
                        status = true;
                    }
                    else
                    {
                        message = message == string.Empty ? "Error occurred while processing your request." : message;
                    }
                }
                else
                {
                    model.PIUNameList = objCommon.PopulateDPIUOfSRRDA(PMGSY.Extensions.PMGSYSession.Current.ParentNDCode.Value);
                    model.ConstituencyList = objCommon.PopulateMPConstituency(qualityBAL.GetBlockCode(model.PrRoadCode));
                    model.Operation = "A";
                    return View("FillMPVisitDetails", model);
                }
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);

                message = message == string.Empty ? "Error occurred while processing your request." : message;
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }

        }

        //Get List
        [HttpPost]
        public ActionResult GetMPVisitList(int? page, int? rows, string sidx, string sord)
        {
            long totalRecords = 0;
            int prRoadCode = Convert.ToInt32(Request.Params["PrRoadCode"]);
            qualityBAL = new QualityMonitoringBAL();
            try
            {

                using (CommonFunctions commonFunction = new CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new GridParams(page, rows, sidx, sord, Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                    {
                        return null;
                    }
                }

                var jsonData = new
                {
                    rows = qualityBAL.GetMPVisitListBAL(Convert.ToInt32(page) - 1, Convert.ToInt32(rows), sidx, sord, out totalRecords, prRoadCode),
                    total = totalRecords <= Convert.ToInt32(rows) ? 1 : (totalRecords % Convert.ToInt32(rows) == 0 ? totalRecords / Convert.ToInt32(rows) : totalRecords / Convert.ToInt32(rows) + 1),//totalRecords <= Convert.ToInt32(rows) ? 1 : totalRecords / Convert.ToInt32(rows) + 1,
                    page = Convert.ToInt32(page),
                    records = totalRecords
                };
                return Json(jsonData, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return null;
            }

        }


        //Edit

        public ActionResult EditMPVisitDetails(String parameter, String hash, String key)
        {
            Dictionary<string, string> decryptedParameters = null;
            CommonFunctions objCommon = new CommonFunctions();
            FillMPVisitModel model = new FillMPVisitModel();


            qualityBAL = new QualityMonitoringBAL();
            //int taxCode = 0;
            try
            {
                decryptedParameters = URLEncrypt.DecryptParameters1(new string[] { parameter, hash, key });
                model = qualityBAL.GetMPVisitDetailsBAL(Convert.ToInt32(decryptedParameters["VisitCode"]));

                model.ConstituencyList = objCommon.PopulateMPConstituency(qualityBAL.GetBlockCode(Convert.ToInt32(decryptedParameters["PRCode"])));
                if (PMGSYSession.Current.RoleCode == 22 || PMGSYSession.Current.RoleCode == 38) //PIU OR PIUOA
                {
                    model.PIUNameCode = PMGSYSession.Current.ParentNDCode.Value;
                    model.PIUName = PMGSYSession.Current.DepartmentName.ToString();
                }
                else if (PMGSYSession.Current.RoleCode == 2 || PMGSYSession.Current.RoleCode == 37 || PMGSYSession.Current.RoleCode == 36) //SRRDA or SRRDAOA or ITNO
                {
                    model.PIUNameList = objCommon.PopulateDPIUOfSRRDA(PMGSY.Extensions.PMGSYSession.Current.ParentNDCode.Value);
                }

                IMS_SANCTIONED_PROJECTS RoadDetails = qualityBAL.GetRoadDetailsBAL(Convert.ToInt32(decryptedParameters["PRCode"]));
                model.BlockName = RoadDetails.MASTER_BLOCK.MAST_BLOCK_NAME;
                model.PackageName = RoadDetails.IMS_PACKAGE_ID;
                model.SanctionYear = RoadDetails.IMS_YEAR.ToString() + "-" + (RoadDetails.IMS_YEAR + 1).ToString();

                model.RoadName = RoadDetails.IMS_ROAD_NAME;
                model.PrRoadCode = Convert.ToInt32(decryptedParameters["PRCode"]);
                return PartialView("FillMPVisitDetails", model);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return Json(new { success = false, message = "Error occurred while processing your request." });
            }
        }

        //Update 
        [HttpPost]
        public ActionResult UpdateMPVisitDetails(FillMPVisitModel mpvisitModel)
        {
            bool status = false;
            String message = String.Empty;
            try
            {
                qualityBAL = new QualityMonitoringBAL();
                if (ModelState.IsValid)
                {
                    if (qualityBAL.UpdateMPVisitBAL(mpvisitModel, ref message))
                    {
                        message = message == string.Empty ? "MP visit details updated successfully." : message;
                        status = true;
                    }
                    else
                    {
                        message = message == string.Empty ? "Error occurred while processing your request." : message;
                    }
                }
                else
                {
                    return PartialView("FillMPVisitDetails", mpvisitModel);
                }
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                message = message == string.Empty ? "Error occurred while processing your request." : message;
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
        }

        //Delete
        [HttpPost]
        public ActionResult DeleteMPVisitDetails(String parameter, String hash, String key)
        {
            Dictionary<string, string> decryptedParameters = null;
            int visitCode = 0;
            try
            {
                decryptedParameters = URLEncrypt.DecryptParameters1(new string[] { parameter, hash, key });
                visitCode = Convert.ToInt32(decryptedParameters["VisitCode"]);
                qualityBAL = new QualityMonitoringBAL();

                bool status = qualityBAL.DeleteMPBAL(visitCode);
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

        #region  MP Visit File Uploads.
        /// <summary>
        /// Get Main File Upload View
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Audit]

        public ActionResult FileUploadMPVisit(string id)
        {
            QMMPVisitFileUploadViewModel fileUploadViewModel = new QMMPVisitFileUploadViewModel();
            fileUploadViewModel.MP_VISIT_ID = Convert.ToInt32(id);

            //QUALITY_QM_MP_VISIT_FILES master = dbContext.QUALITY_QM_MP_VISIT_FILES.Where(a => a.MP_VISIT_ID == fileUploadViewModel.MP_VISIT_ID).FirstOrDefault();

            //fileUploadViewModel.FILE_NAME = master.FILE_NAME;
            //fileUploadViewModel.FILE_UPLOAD_DATE = Convert.ToString(master.FILE_UPLOAD_DATE);
            QualityMonitoringBAL qualityObj = new QualityMonitoringBAL();
            QUALITY_QM_MP_VISIT qualityMPvisit = qualityObj.GetVisitDetailsBAL(fileUploadViewModel.MP_VISIT_ID);
            fileUploadViewModel.DateOfVisit = qualityMPvisit.DATE_OF_VISIT.ToString("dd/MM/yyyy");
            fileUploadViewModel.MPName = qualityMPvisit.MP_NAME;
            fileUploadViewModel.MPHouse = qualityMPvisit.MP_HOUSE == "R" ? "Rajyasabha" : "	Loksabha";
            //fileUploadViewModel.PIUName = qualityMPvisit.PIU_NAME;


            return View("FileUploadMPVisit", fileUploadViewModel);
        }

        //Pdf upload view
        public ActionResult PdfFileUploadMPVisit(string id)
        {
            QMMPVisitFileUploadViewModel fileUploadViewModel = new QMMPVisitFileUploadViewModel();
            dbContext = new PMGSYEntities();
            fileUploadViewModel.MP_VISIT_ID = Convert.ToInt32(id);

            if (dbContext.QUALITY_QM_MP_VISIT_FILES.Where(a => a.MP_VISIT_ID == fileUploadViewModel.MP_VISIT_ID && a.IS_PDF.ToUpper() == "Y").Any())
            {
                fileUploadViewModel.NumberofPdfs = dbContext.QUALITY_QM_MP_VISIT_FILES.Where(a => a.MP_VISIT_ID == fileUploadViewModel.MP_VISIT_ID && a.IS_PDF.ToUpper() == "Y").Count();
            }
            else
            {
                fileUploadViewModel.NumberofPdfs = 0;
            }

            fileUploadViewModel.ErrorMessage = string.Empty;
            return View(fileUploadViewModel);
        }

        //img upload view
        /// <summary>
        ///  Get the Image Upload View
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Audit]
        public ActionResult ImageUploadMPVisit(string id)
        {
            QMMPVisitFileUploadViewModel fileUploadViewModel = new QMMPVisitFileUploadViewModel();
            dbContext = new PMGSYEntities();
            fileUploadViewModel.MP_VISIT_ID = Convert.ToInt32(id);

            fileUploadViewModel.ErrorMessage = string.Empty;

            if (dbContext.QUALITY_QM_MP_VISIT_FILES.Where(a => a.MP_VISIT_ID == fileUploadViewModel.MP_VISIT_ID && a.IS_PDF.ToUpper() == "N").Any())
            {
                fileUploadViewModel.NumberofImages = dbContext.QUALITY_QM_MP_VISIT_FILES.Where(a => a.MP_VISIT_ID == fileUploadViewModel.MP_VISIT_ID && a.IS_PDF.ToUpper() == "N").Count();
            }
            else
            {
                fileUploadViewModel.NumberofImages = 0;
            }
            fileUploadViewModel.ErrorMessage = string.Empty;
            return View(fileUploadViewModel);
        }


        /// <summary>
        /// Lists the Files
        /// </summary>
        /// <param name="formCollection"></param>
        /// <returns></returns>
        [Audit]
        public JsonResult ListImageFileMPVisit(FormCollection formCollection)
        {
            QualityMonitoringBAL qualityObj = new QualityMonitoringBAL();

            using (CommonFunctions commonFunction = new CommonFunctions())
            {
                if (!commonFunction.ValidateGridParameters(new GridParams(Convert.ToInt32(formCollection["page"]), Convert.ToInt32(formCollection["rows"]), formCollection["sidx"], formCollection["sord"], Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                {
                    return null;
                }
            }

            int MP_VISIT_ID = Convert.ToInt32(Request["MP_VISIT_ID"]);
            int totalRecords;
            var jsonData = new
            {
                rows = qualityObj.GetImageListMPVisitBAL(Convert.ToInt32(formCollection["page"]) - 1, Convert.ToInt32(formCollection["rows"]), formCollection["sidx"], formCollection["sord"], out totalRecords, MP_VISIT_ID),
                total = totalRecords <= Convert.ToInt32(formCollection["rows"]) ? 1 : totalRecords / Convert.ToInt32(formCollection["rows"]) + 1,
                page = Convert.ToInt32(formCollection["page"]),
                records = totalRecords
            };
            return Json(jsonData);
        }

        /// <summary>
        /// Get the PDF Files List
        /// </summary>
        /// <param name="formCollection"></param>
        /// <returns></returns>
        [Audit]
        public JsonResult ListPDFFileMPVisit(FormCollection formCollection)
        {
            QualityMonitoringBAL qualityObj = new QualityMonitoringBAL();

            using (CommonFunctions commonFunction = new CommonFunctions())
            {
                if (!commonFunction.ValidateGridParameters(new GridParams(Convert.ToInt32(formCollection["page"]), Convert.ToInt32(formCollection["rows"]), formCollection["sidx"], formCollection["sord"], Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                {
                    return null;
                }
            }

            int MP_VISIT_ID = Convert.ToInt32(Request["MP_VISIT_ID"]);
            int totalRecords;
            var jsonData = new
            {
                rows = qualityObj.GetPDFListMPVisitBAL(Convert.ToInt32(formCollection["page"]) - 1, Convert.ToInt32(formCollection["rows"]), formCollection["sidx"], formCollection["sord"], out totalRecords, MP_VISIT_ID),
                total = totalRecords <= Convert.ToInt32(formCollection["rows"]) ? 1 : totalRecords / Convert.ToInt32(formCollection["rows"]) + 1,
                page = Convert.ToInt32(formCollection["page"]),
                records = totalRecords
            };
            return Json(jsonData);
        }

        /// <summary>
        /// Post Method for Uploading IMAGE File
        /// </summary>
        /// <param name="fileUploadViewModel"></param>
        /// <returns></returns>
        [HttpPost]
        [AcceptVerbs(HttpVerbs.Post)]
        [Audit]
        public ActionResult UploadsImageMPVisit(QMMPVisitFileUploadViewModel fileUploadViewModel)
        {
            CommonFunctions objCommonFunc = new CommonFunctions();
            //Array of File Types to Validate     
            QualityMonitoringBAL qualityObj = new QualityMonitoringBAL();
            String[] fileTypes = new String[] { "jpeg", String.Empty, String.Empty };
            if (!(objCommonFunc.IsValidImageFile(ConfigurationManager.AppSettings["MPVISIT_IMAGE_FILE_UPLOAD"], Request, fileTypes)))
            {
                fileUploadViewModel.ErrorMessage = "File Type is Not Allowed.";
                return View("ImageUploadMPVisit", fileUploadViewModel.ErrorMessage);
            }

            foreach (string file in Request.Files)
            {
                string status = qualityObj.ValidateMPVisitImageFile(Request.Files[0].ContentLength, Path.GetExtension(Request.Files[0].FileName));
                if (status != string.Empty)
                {
                    fileUploadViewModel.ErrorMessage = status;
                    return View("ImageUploadMPVisit", fileUploadViewModel.ErrorMessage);
                }
            }

            var fileData = new List<QMMPVisitFileUploadViewModel>();

            int MP_VISIT_ID = 0;
            if (fileUploadViewModel.MP_VISIT_ID != 0)
            {
                MP_VISIT_ID = fileUploadViewModel.MP_VISIT_ID;
            }
            else
            {
                try
                {
                    MP_VISIT_ID = Convert.ToInt32(Request["MP_VISIT_ID"]);
                }
                catch
                {
                    if (Request["MP_VISIT_ID"].Contains(','))
                    {
                        MP_VISIT_ID = Convert.ToInt32(Request["MP_VISIT_ID"].Split(',')[0]);
                    }
                }
            }
            foreach (string file in Request.Files)
            {
                //This UploadImageFileMPVisit(Request, fileData, MP_VISIT_ID) function calls the function below.
                UploadImageFileMPVisit(Request, fileData, MP_VISIT_ID);
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
        /// Uploads the Image File
        /// </summary>
        /// <param name="request"></param>
        /// <param name="statuses"></param>
        /// <param name="IMS_PR_ROAD_CODE"></param>
        public void UploadImageFileMPVisit(HttpRequestBase request, List<QMMPVisitFileUploadViewModel> statuses, int MP_VISIT_ID)
        {
            String StorageRoot = ConfigurationManager.AppSettings["MPVISIT_IMAGE_FILE_UPLOAD"];
            int MaxCount = 0;
            QualityMonitoringBAL objBAL = new QualityMonitoringBAL();
            for (int i = 0; i < request.Files.Count; i++)
            {
                HttpPostedFileBase file = request.Files[i];
                dbContext = new PMGSYEntities();
                var fileId = MP_VISIT_ID;
                if (dbContext.QUALITY_QM_MP_VISIT_FILES.Where(a => a.MP_VISIT_ID == MP_VISIT_ID).Any())
                {
                    //Commented Max Count Logic By Shyam, it fails when user deletes intermediate file.
                    //MaxCount = db.IMS_PROPOSAL_FILES.Where(a => a.IMS_PR_ROAD_CODE == IMS_PR_ROAD_CODE).Count(); 

                    //Take Max File Id respective to IMS_PR_ROAD_CODE
                    MaxCount = dbContext.QUALITY_QM_MP_VISIT_FILES.Where(a => a.MP_VISIT_ID == MP_VISIT_ID).Select(a => a.FILE_ID).Max();
                }
                MaxCount++;

                var fileName = MP_VISIT_ID + "-" + MaxCount + Path.GetExtension(request.Files[i].FileName).ToString();
                var fullPath = Path.Combine(StorageRoot, fileName);

                var ThumbnailPath = Path.Combine(StorageRoot, "thumbnails");
                var FullThumbnailPath = Path.Combine(ThumbnailPath, fileName);

                statuses.Add(new QMMPVisitFileUploadViewModel()
                {
                    url = fullPath,
                    thumbnail_url = fullPath,
                    name = fileName,
                    type = file.ContentType,
                    size = file.ContentLength,
                    delete_url = "",
                    delete_type = "DELETE",

                    // chainage = Convert.ToDecimal(request.Params["chainageValue[]"]),
                    // Image_Description = request.Params["remark[]"],
                    MP_VISIT_ID = MP_VISIT_ID,

                });

                string status = objBAL.AddFileUploadMPVisitDetailsBAL(statuses, "N");
                if (status == string.Empty)
                {
                    objBAL.CompressImage(request.Files[0], fullPath, FullThumbnailPath);
                    //file.SaveAs(fullPath);
                }
                else
                {
                    // show an error over here
                }
            }
        }


        /// <summary>
        /// Post Method for Uploading PDF File
        /// </summary>
        /// <param name="fileUploadViewModel"></param>
        /// <returns></returns>
        [HttpPost]
        [Audit]
        public ActionResult UploadsPdfMPVisit(QMMPVisitFileUploadViewModel fileUploadViewModel)
        {
            CommonFunctions objCommonFunc = new CommonFunctions();
            QualityMonitoringBAL objBAL = new QualityMonitoringBAL();
            dbContext = new PMGSYEntities();
            if (!(objCommonFunc.ValidateIsPdf(ConfigurationManager.AppSettings["MPVISIT_PDF_FILE_UPLOAD"], Request)))
            {
                fileUploadViewModel.ErrorMessage = "File Type is Not Allowed.";
                return View("PdfFileUploadMPVisit", fileUploadViewModel.ErrorMessage);
            }


            foreach (string file in Request.Files)
            {
                string status = objBAL.ValidateMPVisitPDFFile(Request.Files[0].ContentLength, Path.GetExtension(Request.Files[0].FileName));
                if (status != string.Empty)
                {
                    fileUploadViewModel.ErrorMessage = status;
                    return View("PdfFileUploadMPVisit", fileUploadViewModel.ErrorMessage);
                }
            }

            var fileData = new List<QMMPVisitFileUploadViewModel>();

            int MP_VISIT_ID = 0;
            if (fileUploadViewModel.MP_VISIT_ID != 0)
            {
                MP_VISIT_ID = fileUploadViewModel.MP_VISIT_ID;
            }
            else
            {
                try
                {
                    MP_VISIT_ID = Convert.ToInt32(Request["MP_VISIT_ID"]);
                }
                catch
                {
                    if (Request["MP_VISIT_ID"].Contains(','))
                    {
                        MP_VISIT_ID = Convert.ToInt32(Request["MP_VISIT_ID"].Split(',')[0]);
                    }
                }
            }
            foreach (string file in Request.Files)
            {
                UploadPdfFileMPVisit(Request, fileData, MP_VISIT_ID);
            }

            if (dbContext.QUALITY_QM_MP_VISIT_FILES.Where(a => a.MP_VISIT_ID == MP_VISIT_ID && a.IS_PDF.ToUpper() == "Y").Any())
            {
                fileUploadViewModel.NumberofPdfs = dbContext.QUALITY_QM_MP_VISIT_FILES.Where(a => a.MP_VISIT_ID == fileUploadViewModel.MP_VISIT_ID && a.IS_PDF.ToUpper() == "Y").Count();
            }
            else
            {
                fileUploadViewModel.NumberofPdfs = 0;
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
        /// This Method Uploads PDF File
        /// </summary>
        /// <param name="request"></param>
        /// <param name="statuses"></param>
        /// <param name="IMS_PR_ROAD_CODE"></param>
        [Audit]
        public void UploadPdfFileMPVisit(HttpRequestBase request, List<QMMPVisitFileUploadViewModel> statuses, int MP_VISIT_ID)
        {
            String StorageRoot = ConfigurationManager.AppSettings["MPVISIT_PDF_FILE_UPLOAD"];
            dbContext = new PMGSYEntities();
            QualityMonitoringBAL objBAL = new QualityMonitoringBAL();
            int MaxCount = 0;

            for (int i = 0; i < request.Files.Count; i++)
            {
                HttpPostedFileBase file = request.Files[i];
                var fileId = MP_VISIT_ID;
                if (dbContext.QUALITY_QM_MP_VISIT_FILES.Where(a => a.MP_VISIT_ID == MP_VISIT_ID).Any())
                {
                    MaxCount = dbContext.QUALITY_QM_MP_VISIT_FILES.Where(a => a.MP_VISIT_ID == MP_VISIT_ID).Count();
                }
                MaxCount++;

                var fileName = MP_VISIT_ID + "-" + MaxCount + Path.GetExtension(request.Files[i].FileName).ToString();
                var fullPath = Path.Combine(StorageRoot, fileName);

                statuses.Add(new QMMPVisitFileUploadViewModel()
                {
                    url = fullPath,
                    thumbnail_url = fullPath,
                    name = fileName,
                    type = file.ContentType,
                    size = file.ContentLength,
                    delete_url = "",
                    delete_type = "DELETE",

                    PdfDescription = request.Params["PdfDescription[]"],

                    MP_VISIT_ID = MP_VISIT_ID
                });

                string status = objBAL.AddFileUploadMPVisitDetailsBAL(statuses, "Y");
                if (status == string.Empty)
                {
                    //file.SaveAs(fullPath);
                    file.SaveAs(Path.Combine(ConfigurationManager.AppSettings["MPVISIT_PDF_FILE_UPLOAD"], fileName));
                }
                else
                {
                    // show an error over here
                }
            }
        }

        /// <summary>
        /// Downloads the File
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="hash"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        [HttpGet]
        [Audit]
        public ActionResult DownloadMPVisitFiles(String parameter, String hash, String key)
        {
            try
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
                    FullFileLogicalPath = Path.Combine(ConfigurationManager.AppSettings["MPVISIT_PDF_FILE_UPLOAD_VIRTUAL_DIR_PATH"], FileName);
                    FullfilePhysicalPath = Path.Combine(ConfigurationManager.AppSettings["MPVISIT_PDF_FILE_UPLOAD"], FileName);
                }
                else if (FileExtension == ".jpg" || FileExtension == ".jpeg" || FileExtension == ".bmp" || FileExtension == ".png" || FileExtension == ".gif" || FileExtension == ".tiff")
                {
                    FullFileLogicalPath = Path.Combine(ConfigurationManager.AppSettings["MPVISIT_IMAGE_FILE_UPLOAD_VIRTUAL_DIR_PATH"], FileName);
                    FullfilePhysicalPath = Path.Combine(ConfigurationManager.AppSettings["MPVISIT_IMAGE_FILE_UPLOAD"], FileName);
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
                        case ".doc":
                        case ".docx":
                            type = "Application/msword";
                            break;
                        case ".jpg":
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
            catch
            {
                return Json(new { Success = "false", ErrorMessage = "Error While Processing Your Request." }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// Delete File and File Details
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        [Audit]
        public JsonResult DeleteMPVisitFilesDetails(string id)
        {
            try
            {
                String PhysicalPath = string.Empty;
                String ThumbnailPath = string.Empty;
                string FILE_NAME = Request.Params["FILE_NAME"];

                if (FILE_NAME.Contains('_'))
                {
                    FILE_NAME = FILE_NAME.Replace('_', ' ');
                }

                if (Request.Params["IS_PDF"].ToUpper() == "N")
                {
                    PhysicalPath = ConfigurationManager.AppSettings["MPVISIT_IMAGE_FILE_UPLOAD"];
                    ThumbnailPath = Path.Combine(Path.Combine(PhysicalPath, "thumbnails"), FILE_NAME);
                }
                else if (Request.Params["IS_PDF"].ToUpper() == "Y")
                {
                    PhysicalPath = ConfigurationManager.AppSettings["MPVISIT_PDF_FILE_UPLOAD"];
                }

                string[] arrParam = Request.Params["FILE_ID_MP_VISIT_ID"].Split('$');

                int FILE_ID = Convert.ToInt32(arrParam[0]);
                int MP_VISIT_ID = Convert.ToInt32(arrParam[1]);

                PhysicalPath = Path.Combine(PhysicalPath, FILE_NAME);

                if (!System.IO.File.Exists(PhysicalPath) && !System.IO.File.Exists(ThumbnailPath))
                {
                    //return Json(new { Success = false, ErrorMessage = "File Not Found." });
                }
                QualityMonitoringBAL objBAL = new QualityMonitoringBAL();

                string status = objBAL.DeleteMPVisitFileDetailsBAL(FILE_ID, MP_VISIT_ID, FILE_NAME, Request.Params["IS_PDF"]);

                if (status == string.Empty)
                {
                    try
                    {
                        System.IO.File.Delete(PhysicalPath);
                        System.IO.File.Delete(ThumbnailPath);
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



        #endregion

        #region Tour Invoice

        public List<SelectListItem> PopulateYear(int SelectedYear = 0, bool populateFirstItem = true, bool isAllYearsSelected = false)
        {
            List<SelectListItem> lstYears = new List<SelectListItem>();
            SelectListItem item = new SelectListItem();
            if (populateFirstItem && isAllYearsSelected == false)
            {
                item.Text = "Select Year";
                item.Value = "0";
                item.Selected = true;
                lstYears.Add(item);
            }
            if (populateFirstItem && isAllYearsSelected)
            {
                item.Text = "All Years";
                item.Value = "-1";
                item.Selected = true;
                lstYears.Add(item);
            }
            for (int i = 2000; i < DateTime.Now.Year + 1; i++)
            {
                item = new SelectListItem();
                item.Text = i + " - " + (i + 1);
                item.Value = i.ToString();
                //if (i == DateTime.Now.Year && SelectedYear == 0)
                //{
                //    //item.Selected = true;
                //}
                //if (i == SelectedYear)
                //{
                //   // item.Selected = true;
                //}
                lstYears.Add(item);
            }

            return lstYears;
        }

        [Audit]
        public ActionResult TourGenerateInvoiceLayout()
        {
            QMTourDetails filterViewModel = new QMTourDetails();
            CommonFunctions objCommonFuntion = new CommonFunctions();
            //List<SelectListItem> lstProposalTypes = new List<SelectListItem>();
            //lstProposalTypes.Insert(0, new SelectListItem { Value = "0", Text = "Both" });
            //lstProposalTypes.Insert(1, new SelectListItem { Value = "P", Text = "Road" });
            //lstProposalTypes.Insert(2, new SelectListItem { Value = "L", Text = "Bridge" });
            //filterViewModel.STATES = objCommonFuntion.PopulateStates();
            //filterViewModel.BATCHS = objCommonFuntion.PopulateBatch();
            ////filterViewModel.PROPOSAL_TYPES = objCommonFuntion.PopulateProposalTypes();
            //filterViewModel.PROPOSAL_TYPES = lstProposalTypes;
            filterViewModel.IMS_YEAR = DateTime.Now.Year;
            filterViewModel.Years = PopulateYear(DateTime.Now.Year, true, false);
            filterViewModel.MonthList = objCommonFuntion.PopulateMonths();
            filterViewModel.MonthList.Find(x => x.Value == "0").Value = "-1";
            filterViewModel.MonitorList = objCommonFuntion.PopulateMonitors("false", "I", 0);
            return View("TourGenerateInvoiceLayout", filterViewModel);
        }

        [HttpPost]
        [Audit]
        public ActionResult ListTourPaymentDetails(FormCollection formCollection)
        {
            QualityMonitoringBAL objBAL = new QualityMonitoringBAL();
            QMTourDetails model = new QMTourDetails();
            if (PMGSYSession.Current == null)
            {
                Response.Redirect("/Login/Login");
            }

            //Added By Abhishek kamble 30-Apr-2014 Start
            using (PMGSY.Common.CommonFunctions commonFunction = new PMGSY.Common.CommonFunctions())
            {
                if (!commonFunction.ValidateGridParameters(new PMGSY.Common.GridParams(Convert.ToInt32(formCollection["page"]), Convert.ToInt32(formCollection["rows"]), formCollection["sidx"], formCollection["sord"], Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                {
                    return null;
                }
            }
            //Added By Abhishek kamble 30-Apr-2014 end

            int IMS_YEAR = Convert.ToInt32(Request.Params["IMS_YEAR"]);
            int Month = Convert.ToInt32(Request.Params["Month"]);
            int Monitor = Convert.ToInt32(Request.Params["Monitor"]);
            int totalRecords;

            var jsonData = new
            {
                rows = objBAL.GetTourPaymentListBAL(Convert.ToInt32(formCollection["page"]) - 1, Convert.ToInt32(formCollection["rows"]), formCollection["sidx"], formCollection["sord"], out totalRecords, IMS_YEAR, Month, Monitor),
                total = totalRecords <= Convert.ToInt32(formCollection["rows"]) ? 1 : totalRecords / Convert.ToInt32(formCollection["rows"]) + 1,
                page = Convert.ToInt32(formCollection["page"]),
                records = totalRecords,
                TotalModel = model
            };
            return Json(jsonData);
        }

        [HttpPost]
        [Audit]
        public ActionResult ListTourGeneratedInvoice(FormCollection formCollection)
        {
            QualityMonitoringBAL objBAL = new QualityMonitoringBAL();
            //STAPaymentTotalViewModel model = new STAPaymentTotalViewModel();
            if (PMGSYSession.Current == null)
            {
                Response.Redirect("/Login/Login");
            }
            //Added By Abhishek kamble 30-Apr-2014 Start
            using (PMGSY.Common.CommonFunctions commonFunction = new PMGSY.Common.CommonFunctions())
            {
                if (!commonFunction.ValidateGridParameters(new PMGSY.Common.GridParams(Convert.ToInt32(formCollection["page"]), Convert.ToInt32(formCollection["rows"]), formCollection["sidx"], formCollection["sord"], Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                {
                    return null;
                }
            }
            //Added By Abhishek kamble 30-Apr-2014 end

            //Int16 IMS_YEAR = Convert.ToInt16(Request.Params["IMS_YEAR"]);
            //int invoiceId = Convert.ToInt32(Request.Params["invoiceId"]);
            int scheduleCode = Convert.ToInt32(Request.Params["scheduleCode"]);
            int totalRecords;
            var jsonData = new
            {
                rows = objBAL.GetTourInvoiceListBAL(Convert.ToInt32(formCollection["page"]) - 1, Convert.ToInt32(formCollection["rows"]), formCollection["sidx"], formCollection["sord"], out totalRecords, scheduleCode),
                total = 0 <= Convert.ToInt32(formCollection["rows"]) ? 1 : totalRecords / Convert.ToInt32(formCollection["rows"]) + 1,
                page = Convert.ToInt32(formCollection["page"]),
                records = 0,
                //TotalModel = model
            };
            return Json(jsonData);
        }

        [HttpGet]
        [Audit]
        public ActionResult AddTourInvoiceDetails()
        {
            dbContext = new PMGSYEntities();
            try
            {
                int scheduleCode = Convert.ToInt32(Request.Params["ADMIN_SCHEDULE_CODE"]);
                QMTourGenerateInvoice model = new QMTourGenerateInvoice();
                #region
                //int MAST_STATE_CODE = Convert.ToInt32(Request.Params["MAST_STATE_ID"]);
                //int IMS_YEAR = Convert.ToInt32(Request.Params["IMS_YEAR"]);
                //int IMS_BATCH = Convert.ToInt32(Request.Params["IMS_BATCH"]);
                //int IMS_STREAMS = Convert.ToInt32(Request.Params["IMS_STREAM"]);
                //string IMS_PROPOSAL_TYPE = Request.Params["IMS_PROPOSAL_TYPE"];
                //string STA_SANCTIONED_BY = Request.Params["STA_SANCTIONED_BY"];
                //string STA_INSTITUTE_NAME = Request.Params["STA_INSTITUTE_NAME"];
                //decimal HON_AMOUNT = Convert.ToDecimal(Request.Params["HON_AMOUNT"]);
                //int PMGSY_SCHEME = Convert.ToInt32(Request.Params["PMGSY_SCHEME"]);
                //decimal honAmountTillDate = 0;


                //staInvoiceViewModel.MAST_STATE_CODE = MAST_STATE_CODE;
                //staInvoiceViewModel.IMS_YEAR = Convert.ToInt16(IMS_YEAR);
                //staInvoiceViewModel.IMS_BATCH = IMS_BATCH;
                //staInvoiceViewModel.IMS_STREAM = IMS_STREAMS;
                //staInvoiceViewModel.IMS_PROPOSAL_TYPE = IMS_PROPOSAL_TYPE;
                //staInvoiceViewModel.STA_SANCTIONED_BY = STA_SANCTIONED_BY;
                //staInvoiceViewModel.SAS_ABBREVATION = STA_INSTITUTE_NAME;
                //staInvoiceViewModel.PMGSY_SCHEME = PMGSY_SCHEME;

                //if (
                //    db.IMS_GENERATED_INVOICE.Where(
                //                            c => c.MAST_STATE_CODE == MAST_STATE_CODE &&
                //                            c.IMS_YEAR == IMS_YEAR &&
                //                            c.IMS_STREAM == IMS_STREAMS &&
                //                            c.IMS_BATCH == IMS_BATCH &&
                //                            c.IMS_PROPOSAL_TYPE == IMS_PROPOSAL_TYPE &&
                //                            c.STA_SANCTIONED_BY == STA_SANCTIONED_BY &&
                //                            c.MAST_PMGSY_SCHEME == PMGSY_SCHEME
                //                            ).Any())
                //{
                //    honAmountTillDate = db.IMS_GENERATED_INVOICE.Where(
                //                                c => c.MAST_STATE_CODE == MAST_STATE_CODE &&
                //                                c.IMS_YEAR == IMS_YEAR &&
                //                                c.IMS_STREAM == IMS_STREAMS &&
                //                                c.IMS_BATCH == IMS_BATCH &&
                //                                c.IMS_PROPOSAL_TYPE == IMS_PROPOSAL_TYPE &&
                //                                c.STA_SANCTIONED_BY == STA_SANCTIONED_BY &&
                //                                c.MAST_PMGSY_SCHEME == PMGSY_SCHEME
                //                                ).Sum(c => c.HONORARIUM_AMOUNT);

                //}

                //staInvoiceViewModel.HONORARIUM_AMOUNT = HON_AMOUNT;
                //staInvoiceViewModel.Balance_Amount = HON_AMOUNT - Convert.ToDecimal(honAmountTillDate);
                #endregion
                var Master_tax = (from c in dbContext.MASTER_TAXES
                                  where c.MAST_TDS_ID == (dbContext.MASTER_TAXES.Select(a => a.MAST_TDS_ID).Max())
                                  select c);
                MASTER_TAXES master_taxes = new MASTER_TAXES();
                foreach (var item in Master_tax)
                {
                    master_taxes.MAST_TDS_ID = item.MAST_TDS_ID;
                    master_taxes.MAST_TDS_SC = item.MAST_TDS_SC;
                    master_taxes.MAST_TDS = item.MAST_TDS;
                    master_taxes.SERVICE_TAX = item.SERVICE_TAX;
                }
                model.Per_Sc = master_taxes.MAST_TDS_SC;
                model.Per_Tds = master_taxes.MAST_TDS;
                model.MAST_TDS_ID = master_taxes.MAST_TDS_ID;
                model.Per_Service_Tax = Convert.ToDecimal(master_taxes.SERVICE_TAX);
                model.MAST_TDS = (master_taxes.MAST_TDS / 100);

                //model.ServiceTaxNo = dbContext.ADMIN_TECHNICAL_AGENCY.Any(m => m.ADMIN_TA_NAME == STA_INSTITUTE_NAME) ? db.ADMIN_TECHNICAL_AGENCY.Where(m => m.ADMIN_TA_NAME == STA_INSTITUTE_NAME).Select(s => s.ADMIN_TA_SERVICE_TAX).FirstOrDefault() : "0";

                model.TOUR_EXPENDITURE = Convert.ToInt32(dbContext.QUALITY_QM_TOUR_DETAILS.Where(x => x.ADMIN_SCHEDULE_CODE == scheduleCode).Select(x => x.TOUR_EXPENDITURE).FirstOrDefault());
                model.ADMIN_SCHEDULE_CODE = scheduleCode;
                return View(model);
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        [Audit]
        [HttpPost]
        public JsonResult AddTourInvoiceDetails(QMTourGenerateInvoice model)
        {
            if (ModelState.IsValid)
            {
                QualityMonitoringBAL objBAL = new QualityMonitoringBAL();
                string Status = objBAL.AddTourInvoiceDetailsBAL(model);

                if (Status == string.Empty)
                    return Json(new { Success = true });
                else
                    return Json(new { Success = false, ErrorMessage = Status });
            }
            else
            {
                return Json(new { Success = false, ErrorMessage = new CommonFunctions().FormatErrorMessage(ModelState) });
            }
        }

        public JsonResult DeleteTourGeneratedInvoice(int invoiceID)
        {
            try
            {
                string message = "";
                QualityMonitoringBAL objBAL = new QualityMonitoringBAL();
                bool Status = objBAL.DeleteTourGeneratedInvoiceBAL(invoiceID, ref message);

                if (Status == true)
                    return Json(new { Success = true });
                else
                    return Json(new { Success = false, ErrorMessage = Status });
            }
            catch (Exception)
            {
                return Json(new { Success = false, ErrorMessage = "Error occurred while processing your request." });
            }
        }

        #endregion

        #region Tour Payment
        [Audit]
        public ActionResult TourPaymentInoviceLayout()
        {
            QMTourDetails filterViewModel = new QMTourDetails();
            CommonFunctions objCommonFuntion = new CommonFunctions();

            filterViewModel.IMS_YEAR = DateTime.Now.Year;
            filterViewModel.Years = PopulateYear(DateTime.Now.Year, true, false);
            filterViewModel.MonthList = objCommonFuntion.PopulateMonths();
            filterViewModel.MonthList.Find(x => x.Value == "0").Value = "-1";
            filterViewModel.MonitorList = objCommonFuntion.PopulateMonitors("false", "I", 0);
            return View(filterViewModel);
        }

        [HttpPost]
        [Audit]
        public ActionResult ListTourPaymentInovice(FormCollection formCollection)
        {
            QualityMonitoringBAL objBAL = new QualityMonitoringBAL();
            //STAPaymentTotalViewModel model = new STAPaymentTotalViewModel();
            if (PMGSYSession.Current == null)
            {
                Response.Redirect("/Login/Login");
            }
            //Added By Abhishek kamble 30-Apr-2014 Start
            using (PMGSY.Common.CommonFunctions commonFunction = new PMGSY.Common.CommonFunctions())
            {
                if (!commonFunction.ValidateGridParameters(new PMGSY.Common.GridParams(Convert.ToInt32(formCollection["page"]), Convert.ToInt32(formCollection["rows"]), formCollection["sidx"], formCollection["sord"], Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                {
                    return null;
                }
            }
            //Added By Abhishek kamble 30-Apr-2014 end

            int IMS_YEAR = Convert.ToInt32(Request.Params["IMS_YEAR"]);
            int Month = Convert.ToInt32(Request.Params["Month"]);
            int Monitor = Convert.ToInt32(Request.Params["Monitor"]);
            int totalRecords;
            var jsonData = new
            {
                rows = objBAL.ListTourPaymentInvoiceBAL(Convert.ToInt32(formCollection["page"]) - 1, Convert.ToInt32(formCollection["rows"]), formCollection["sidx"], formCollection["sord"], out totalRecords, IMS_YEAR, Month, Monitor),
                total = 0 <= Convert.ToInt32(formCollection["rows"]) ? 1 : totalRecords / Convert.ToInt32(formCollection["rows"]) + 1,
                page = Convert.ToInt32(formCollection["page"]),
                records = 0,
                //TotalModel = model
            };
            return Json(jsonData);
        }

        public ActionResult TourPaymentDetail(string id)
        {

            QualityMonitoringBAL objBAL = new QualityMonitoringBAL();
            Dictionary<string, string> decryptedParameters = null;
            string[] encryptedParams = id.Split('/');
            decryptedParameters = URLEncrypt.DecryptParameters1(new string[] { encryptedParams[0], encryptedParams[1], encryptedParams[2] });
            int invoiceCode = Convert.ToInt32(decryptedParameters["IMS_INVOICE_ID"]);

            ViewData["IMS_INVOICE_CODE"] = invoiceCode;
            ViewData["EncryptedIMS_Invoice_Code"] = URLEncrypt.EncryptParameters1(new string[] { "IMS_INVOICE_ID=" + invoiceCode.ToString().Trim() });
            return View();
        }

        [Audit]
        public ActionResult GetTourPaymentList(FormCollection formCollection)
        {

            using (CommonFunctions commonFunction = new CommonFunctions())
            {
                if (!commonFunction.ValidateGridParameters(new GridParams(Convert.ToInt32(formCollection["page"]), Convert.ToInt32(formCollection["rows"]), formCollection["sidx"], formCollection["sord"], Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                {
                    return null;
                }
            }
            long totalRecords = 0;
            int IMS_INVOICE_Code = 0;
            QualityMonitoringBAL objBAL = new QualityMonitoringBAL();

            try
            {
                //Dictionary<string, string> decryptedParameters = null;
                //string[] encryptedParams = formCollection["IMS_INVOICE_CODE"].Split('/');
                //decryptedParameters = URLEncrypt.DecryptParameters1(new string[] { encryptedParams[0], encryptedParams[1], encryptedParams[2] });
                //IMS_INVOICE_Code = Convert.ToInt32(decryptedParameters["IMS_INVOICE_ID"]);

                if (!string.IsNullOrEmpty(formCollection["IMS_INVOICE_CODE"]))
                {
                    IMS_INVOICE_Code = Convert.ToInt32(formCollection["IMS_INVOICE_CODE"]);
                }


                var jsonData = new
                {
                    rows = objBAL.GetTourPaymentListBAL(IMS_INVOICE_Code, Convert.ToInt32(formCollection["page"]) - 1, Convert.ToInt32(formCollection["rows"]), formCollection["sidx"], formCollection["sord"], out totalRecords),
                    total = totalRecords <= Convert.ToInt32(formCollection["rows"]) ? 1 : totalRecords / Convert.ToInt32(formCollection["rows"]) + 1,
                    page = Convert.ToInt32(formCollection["page"]),
                    records = totalRecords
                };
                return Json(jsonData, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return null;
            }
        }

        public ActionResult TourPaymentAdd(string id)
        {
            TourAddPaymentModel model = new TourAddPaymentModel();
            QualityMonitoringBAL objBAL = new QualityMonitoringBAL();
            Dictionary<string, string> decryptedParameters = null;
            string[] encryptedParams = id.Split('/');
            decryptedParameters = URLEncrypt.DecryptParameters1(new string[] { encryptedParams[0], encryptedParams[1], encryptedParams[2] });
            int invoiceCode = Convert.ToInt32(decryptedParameters["IMS_INVOICE_ID"]);
            ///Tour Report to be added later
            //STAInvoiceViewModel invoiceModel = objStaPaymentBAL.StaPaymentReportBAL(invoiceCode); 
            //model.Invoice_Generate_DATE = invoiceModel.Invoice_Generate_DATE;
            model.IMS_INVOICE_CODE = invoiceCode;
            model.EncryptedIMS_Invoice_Code = id;
            return View(model);
        }

        /// <summary>
        /// Add Test result details
        /// </summary>
        /// <param name="sTAPayemntInvoiceViewModel"></param>
        /// <returns></returns>
        [HttpPost]
        [Audit]
        [ValidateAntiForgeryToken]
        public ActionResult AddTourPaymentDetails(TourAddPaymentModel model)
        {
            string message = string.Empty;
            QualityMonitoringBAL objBAL = new QualityMonitoringBAL();

            try
            {
                if (ModelState.IsValid)
                {
                    if (objBAL.AddTourPaymentDetailsBAL(model, ref message))
                    {
                        message = message == string.Empty ? "Tour Payment details saved successfully." : message;
                        return Json(new { success = true, message = message });
                    }
                    else
                    {
                        message = message == string.Empty ? "Tour Payment details not saved." : message;
                        return Json(new { success = false, message = message });
                    }
                }
                else
                {

                    string messages = string.Join("; ", ModelState.Values
                                           .SelectMany(x => x.Errors)
                                           .Select(x => x.ErrorMessage));

                    return PartialView("TourPaymentAdd", model);
                }
            }
            catch (DbEntityValidationException ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                foreach (var eve in ex.EntityValidationErrors)
                {
                    ModelState.AddModelError("", eve.ValidationErrors.ToString());
                    message += eve.ValidationErrors.ToString();
                }
                return Json(new { success = false, message = message }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                message = "Tour Payment details not saved because " + ex.Message;
                return Json(new { success = false, message = message }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        [Audit]
        public ActionResult EditTourPaymentDetails(String parameter, String hash, String key)
        {
            try
            {
                QualityMonitoringBAL objBAL = new QualityMonitoringBAL();

                Dictionary<string, string> decryptedParameters = null;

                decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { parameter, hash, key });

                if (decryptedParameters.Count > 0)
                {
                    int paymentCode = Convert.ToInt32(decryptedParameters["IMS_PAYMENT_ID"].ToString());
                    int imsInvoiceCode = Convert.ToInt32(decryptedParameters["IMS_INVOICE_ID"].ToString());

                    TourAddPaymentModel model = objBAL.GetTourPaymentDetailsBAL(paymentCode, imsInvoiceCode);
                    model.IMS_INVOICE_CODE = imsInvoiceCode;

                    if (model == null)
                    {
                        ModelState.AddModelError("", "Tour Payment Details not exist.");
                        return PartialView("TourPaymentAdd", new TourAddPaymentModel());
                    }


                    return PartialView("TourPaymentAdd", model);
                }
                return PartialView("TourPaymentAdd", new TourAddPaymentModel());
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                ModelState.AddModelError("", "Tour Payment details not exist.");
                return PartialView("TourPaymentAdd", new TourAddPaymentModel());
            }
        }

        [HttpPost]
        [Audit]
        public ActionResult UpdateTourPaymentDetails(TourAddPaymentModel model)
        {
            string message = string.Empty;
            try
            {

                if (ModelState.IsValid)
                {
                    QualityMonitoringBAL objBAL = new QualityMonitoringBAL();

                    if (objBAL.UpdateTourPaymentDetailsBAL(model, ref message))
                    {
                        message = message == string.Empty ? "Tour Payment details Updated successfully." : message;
                        return Json(new { success = true, message = message });
                    }
                    else
                    {
                        message = message == string.Empty ? "Tour Payment details not updated." : message;
                        return Json(new { success = false, message = message });
                    }
                }
                else
                {
                    message = string.Join("; ", ModelState.Values
                        .SelectMany(x => x.Errors)
                        .Select(x => x.ErrorMessage)
                        );

                    return Json(new { success = false, message = message }, JsonRequestBehavior.AllowGet);
                }

            }
            catch (DbEntityValidationException ex)
            {

                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                foreach (var eve in ex.EntityValidationErrors)
                {
                    ModelState.AddModelError("", eve.ValidationErrors.ToString());
                    message += eve.ValidationErrors.ToString();
                }
                return Json(new { success = false, message = message });
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                message = "Additional Cost details not saved because " + ex.Message;
                return Json(new { succes = false, message = message }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        [Audit]
        public ActionResult DeleteTourPaymentDetails(string parameter, string hash, string key)
        {
            string message = string.Empty;

            try
            {
                Dictionary<string, string> decryptedParameters = null;

                QualityMonitoringBAL objBAL = new QualityMonitoringBAL();

                decryptedParameters = URLEncrypt.DecryptParameters1(new string[] { parameter, hash, key });

                if (decryptedParameters.Count > 0)
                {
                    int paymentCode = Convert.ToInt32(decryptedParameters["IMS_PAYMENT_ID"].ToString());
                    int imsInvoiceCode = Convert.ToInt32(decryptedParameters["IMS_INVOICE_ID"].ToString());

                    if (objBAL.DeleteTourPaymentDetailsBAL(paymentCode, imsInvoiceCode, ref message))
                    {
                        message = message == string.Empty ? "Tour Payment details deleted successfully." : message;

                        return Json(new { success = true, message = message }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        message = message == string.Empty ? "Tour Payment details not deleted." : message;
                        return Json(new { success = false, message = message }, JsonRequestBehavior.AllowGet);
                    }
                }
                message = "An error occured while processing you request.";
                return Json(new { success = false, message = message }, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                message = "An error occured while proccessing your request.";
                return Json(new { success = false, message = message }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        [Audit]
        public ActionResult FinalizeTourPaymentDetails(string parameter, string hash, string key)
        {
            string message = string.Empty;

            try
            {
                Dictionary<string, string> decryptedParameters = null;

                QualityMonitoringBAL objBAL = new QualityMonitoringBAL();

                decryptedParameters = URLEncrypt.DecryptParameters1(new string[] { parameter, hash, key });

                if (decryptedParameters.Count > 0)
                {
                    int paymentCode = Convert.ToInt32(decryptedParameters["IMS_PAYMENT_ID"].ToString());
                    int imsInvoiceCode = Convert.ToInt32(decryptedParameters["IMS_INVOICE_ID"].ToString());

                    if (objBAL.FinalizeTourPaymentDetailsBAL(paymentCode, imsInvoiceCode, ref message))
                    {
                        message = message == string.Empty ? "Tour Payment details finalized successfully." : message;

                        return Json(new { success = true, message = message }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        message = message == string.Empty ? "Tour Payment details not finalized." : message;
                        return Json(new { success = false, message = message }, JsonRequestBehavior.AllowGet);
                    }
                }
                message = "An error occured while processing you request.";
                return Json(new { success = false, message = message }, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                message = "An error occured while proccessing your request.";
                return Json(new { success = false, message = message }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        [Audit]
        public ActionResult DeFinalizeTourPaymentDetails(string parameter, string hash, string key)
        {
            try
            {
                PMGSYEntities db = new PMGSYEntities();
                Dictionary<string, string> decryptedParameters = null;

                decryptedParameters = URLEncrypt.DecryptParameters1(new string[] { parameter, hash, key });

                if (decryptedParameters.Count > 0)
                {
                    int paymentCode = Convert.ToInt32(decryptedParameters["IMS_PAYMENT_ID"].ToString());
                    int imsInvoiceCode = Convert.ToInt32(decryptedParameters["IMS_INVOICE_ID"].ToString());

                    if (db.QM_MONITOR_PAYMENTS.Any(m => m.IMS_INVOICE_ID == imsInvoiceCode && m.IMS_PAYMENT_ID == paymentCode))
                    {
                        QM_MONITOR_PAYMENTS staPayment = db.QM_MONITOR_PAYMENTS.Where(m => m.IMS_PAYMENT_ID == paymentCode && m.IMS_INVOICE_ID == imsInvoiceCode).FirstOrDefault();
                        staPayment.IMS_PAYMENT_FINALIZE = "N";
                        staPayment.USERID = PMGSYSession.Current.UserId;
                        staPayment.IPADD = HttpContext.Request.ServerVariables["REMOTE_ADDR"];
                        db.Entry(staPayment).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();
                        return Json(new { Success = true }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json(new { Success = false, message = "Payment details not found" }, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    return Json(new { Success = false, message = "Payment details not found" }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception)
            {
                return Json(new { Success = false, message = "Error occurred while processing your request." }, JsonRequestBehavior.AllowGet);
            }
        }

        [Audit]
        //[HttpPost]
        public ActionResult TourPaymentSSRSReport(string id)
        {
            PMGSY.DAL.QualityMonitoring.QualityMonitoringDAL objDAL = new PMGSY.DAL.QualityMonitoring.QualityMonitoringDAL();
            QMTourGenerateInvoice model = new QMTourGenerateInvoice();
            try
            {

                model.IMS_INVOICE_ID = Convert.ToInt32(id);
                model = objDAL.GetTourInvoiceDetailsDAL(Convert.ToInt32(id));
                Microsoft.Reporting.WebForms.ReportViewer rview = new Microsoft.Reporting.WebForms.ReportViewer();
                rview.ServerReport.ReportServerUrl = new Uri(System.Web.Configuration.WebConfigurationManager.AppSettings["MvcReportViewer.ReportServerUrl"]);

                System.Collections.Generic.List<Microsoft.Reporting.WebForms.ReportParameter> paramList = new System.Collections.Generic.List<Microsoft.Reporting.WebForms.ReportParameter>();
                paramList.Add(new Microsoft.Reporting.WebForms.ReportParameter("InvoiceId", model.IMS_INVOICE_ID.ToString()));
                //paramList.Add(new Microsoft.Reporting.WebForms.ReportParameter("MonitorName", model.MonitorName.Trim()));

                Microsoft.Reporting.WebForms.IReportServerCredentials irsc = new CustomReportCredentials(System.Web.Configuration.WebConfigurationManager.AppSettings["MvcReportViewer.Username"], System.Web.Configuration.WebConfigurationManager.AppSettings["MvcReportViewer.Password"]);
                rview.ServerReport.ReportServerCredentials = irsc;
                rview.ServerReport.ReportPath = "/PMGSYCitizen/QM_PAYMENT_REPORT";
                // rview.ServerReport.ReportPath = "/PMGSYReports/STA_PAYMENT_REPORTS";
                rview.ServerReport.SetParameters(paramList);
                string mimeType, encoding, extension, deviceInfo;
                string[] streamids;
                Microsoft.Reporting.WebForms.Warning[] warnings;
                string format = "PDF"; //Desired format goes here (PDF, Excel, or Image)

                deviceInfo = "<DeviceInfo>" + "<SimplePageHeaders>True</SimplePageHeaders>" + "</DeviceInfo>";
                //var fileName = "STA_Payment.pdf";
                var fileName = model.INVOICE_FILE_NO + ".pdf";
                byte[] bytes = rview.ServerReport.Render(format, deviceInfo, out mimeType, out encoding, out extension, out streamids, out warnings);

                Response.Clear();


                var cd = new System.Net.Mime.ContentDisposition
                {
                    // for example foo.bak
                    FileName = fileName,

                    // always prompt the user for downloading, set to true if you want 
                    // the browser to try to show the file inline
                    Inline = false,

                };

                Response.AppendHeader("Content-Disposition", cd.ToString());
                return File(bytes, "application/pdf");
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);

                return null;
            }
        }

        [HttpGet]
        [Audit]
        public ActionResult DownloadFileTour(String parameter, String hash, String key)
        {
            try
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
                    FullFileLogicalPath = Path.Combine(ConfigurationManager.AppSettings["TOUR_FILE_UPLOAD_VIRTUAL_DIR_PATH"], FileName);
                    FullfilePhysicalPath = Path.Combine(ConfigurationManager.AppSettings["TOUR_FILE_UPLOAD"], FileName);
                }
                //else if (FileExtension == ".jpg" || FileExtension == ".jpeg" || FileExtension == ".bmp" || FileExtension == ".png" || FileExtension == ".gif" || FileExtension == ".tiff")
                //{
                //    FullFileLogicalPath = Path.Combine(PMGSYSession.Current.PMGSYScheme == 1 ? ConfigurationManager.AppSettings["PROPOSAL_FILE_UPLOAD_VIRTUAL_DIR_PATH_PMGSYI"] : ConfigurationManager.AppSettings["PROPOSAL_FILE_UPLOAD_VIRTUAL_DIR_PATH_PMGSYII"], FileName);
                //    FullfilePhysicalPath = Path.Combine(PMGSYSession.Current.PMGSYScheme == 1 ? ConfigurationManager.AppSettings["PROPOSAL_FILE_UPLOAD_PMGSYI"] : ConfigurationManager.AppSettings["PROPOSAL_FILE_UPLOAD_PMGSYII"], FileName);
                //}

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
            catch
            {
                return Json(new { Success = "false", ErrorMessage = "Error While Processing Your Request." }, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion

        // Added By Anand 10 DEC 2015
        #region Team Details
        [Audit]
        public ActionResult QualityTeam()
        {
            QMFilterViewModel qmFilterModel = new QMFilterViewModel();
            CommonFunctions objCommonFunctions = new CommonFunctions();

            qmFilterModel.FROM_MONTH = DateTime.Now.Month;
            qmFilterModel.FROM_YEAR = DateTime.Now.Year;
            qmFilterModel.FROM_MONTHS_LIST = objCommonFunctions.PopulateMonths(false);
            qmFilterModel.FROM_YEARS_LIST = objCommonFunctions.PopulateYears(false).Where(a => a.Value != "0").ToList();
            if (DateTime.Now.Month == 12)
            {
                qmFilterModel.FROM_YEARS_LIST.Add(new SelectListItem { Text = "2015", Value = "2015" });
                qmFilterModel.FROM_YEARS_LIST = qmFilterModel.FROM_YEARS_LIST.OrderByDescending(c => c.Value).ToList();
            }
            return View(qmFilterModel);
        }


        public ActionResult QualityTeamReport(int smonth, int syear)
        {
            ViewBag.smonth = smonth;
            ViewBag.syear = syear;
            return View();
        }
        #endregion

        #region Team Inspection
        [Audit]
        [HttpPost]
        public ActionResult QMTeamCreate(string teamcode, string isteamlead, int scode)
        {
            //QualityMonitoring/QMTeamCreate
            QualityMonitoringBAL qmBAL = new QualityMonitoringBAL();
            QUALITY_QM_TEAM team = new QUALITY_QM_TEAM
            {
                QM_TEAM_CODE = teamcode,
                IS_TEAM_LEADER = isteamlead,
                ADMIN_SCHEDULE_CODE = scode
            };

            return Json(new { flag = qmBAL.QMTeamCreateDAL(team) });
        }
        [Audit]
        [HttpPost]
        public ActionResult QMTeamDeActivate(int teamid)
        {
            //QualityMonitoring/QMTeamCreate
            QualityMonitoringBAL qmBAL = new QualityMonitoringBAL();

            return Json(new { flag = qmBAL.QMTeamDeActivateBAL(teamid) });
        }


        /// <summary>
        /// Send Mail to SQC & NQM
        /// </summary>
        /// <returns></returns>
        public ActionResult SendLetterToTeam()
        {
            QMLetterModel qmLetterModel = new QMLetterModel();
            String ErrorMessage = "";
            QualityMonitoringBAL qmBAL = new QualityMonitoringBAL();
            dbContext = new PMGSYEntities();
            try
            {
                if (Request.Params["teamCode"] != null)
                {
                    string teamCode = Request.Params["teamCode"].Trim();

                    // if letter is already generated, Display It
                    if (System.IO.File.Exists(ConfigurationManager.AppSettings["QUALITY_QM_LETTER_TEAM"].ToString() + teamCode + ".pdf"))
                    {
                        return Json(new { Success = true, Uploaded = true, Path = (ConfigurationManager.AppSettings["QUALITY_QM_LETTER_TEAM_VIRTUAL"].ToString() + teamCode + ".pdf") }, JsonRequestBehavior.AllowGet);
                    }


                    var lstSchedules = dbContext.QUALITY_QM_TEAM.Where(c => c.QM_TEAM_CODE == teamCode).ToList();
                    Int16 scheduleMonth = 0;
                    Int16 scheduleYear = 0;
                    bool isLetterGenerated = false;
                    bool isMailSent = false;
                    Int32 letterId = 0;
                    foreach (var item in lstSchedules)
                    {
                        var scheduleDetails = dbContext.QUALITY_QM_SCHEDULE.Where(c => c.ADMIN_SCHEDULE_CODE == item.ADMIN_SCHEDULE_CODE).First();

                        qmLetterModel.QC_CODE = (qmLetterModel.QC_CODE == null || qmLetterModel.QC_CODE == 0)
                                                ? dbContext.ADMIN_SQC.Where(c => c.MAST_STATE_CODE ==
                                                    dbContext.QUALITY_QM_SCHEDULE.Where(a => a.ADMIN_SCHEDULE_CODE == item.ADMIN_SCHEDULE_CODE).Select(a => a.MAST_STATE_CODE).FirstOrDefault()
                                                    && c.ADMIN_ACTIVE_STATUS == "Y").Select(c => c.ADMIN_QC_CODE).First()
                                                : qmLetterModel.QC_CODE;
                        scheduleMonth = (scheduleMonth == 0 ? Convert.ToInt16(scheduleDetails.ADMIN_IM_MONTH) : scheduleMonth);
                        scheduleYear = (scheduleYear == 0 ? Convert.ToInt16(scheduleDetails.ADMIN_IM_YEAR) : scheduleYear);


                        using (var transction = new System.Transactions.TransactionScope())
                        {
                            if (isLetterGenerated == false)
                            {
                                //--------------------------SQC Letter Code -----------------------------------//
                                QMLetterModel letterModelQC = new QMLetterModel();
                                //First generate file and store details to Letter Details table
                                var letterModel = new QUALITY_QM_LETTER()
                                {
                                    LETTER_ID = dbContext.QUALITY_QM_LETTER.Select(c => c.LETTER_ID).Max() + 1,
                                    ADMIN_QM_CODE = null,
                                    ADMIN_SQC_CODE = null,
                                    TYPE = "T",
                                    ADMIN_SCHEDULE_CODE = null,
                                    ADMIN_IM_MONTH = scheduleMonth,
                                    ADMIN_IM_YEAR = scheduleYear,
                                    FILE_NAME = teamCode + ".pdf",
                                    GENERATION_DATE = DateTime.Now,
                                    MAIL_DELIVERY_STATUS = false,
                                    MAIL_DELIVERY_DATE = null,
                                    USERID = PMGSYSession.Current.UserId,
                                    IPADD = HttpContext.Request.ServerVariables["REMOTE_ADDR"]
                                };

                                dbContext.QUALITY_QM_LETTER.Add(letterModel);
                                if (dbContext.SaveChanges() > 0)
                                {
                                    letterModelQC.FILE_NAME = letterModel.FILE_NAME;
                                    letterModelQC.QC_TYPE = "S";
                                    letterModelQC.QC_CODE = qmLetterModel.QC_CODE;
                                    letterModelQC.INSP_MONTH = scheduleMonth;
                                    letterModelQC.INSP_YEAR = scheduleYear;
                                    DateTime dtDate = new DateTime(scheduleYear, scheduleMonth, 1);
                                    letterModelQC.MONTH_TEXT = dtDate.ToString("MMMM");
                                    letterModelQC.YEAR_TEXT = scheduleYear.ToString();
                                    letterModelQC.SQC_STATE_CODE = dbContext.ADMIN_SQC.Where(c => c.ADMIN_QC_CODE == qmLetterModel.QC_CODE).Select(c => c.MAST_STATE_CODE).First();

                                    qmBAL.GenerateLetterForTeam(letterModelQC);

                                    // Add Letter ID to QUALITY_QM_TEAM_LETTER
                                    var teamLetter = new QUALITY_QM_TEAM_LETTER()
                                    {
                                        ID = dbContext.QUALITY_QM_LETTER.Any() ? (dbContext.QUALITY_QM_LETTER.Select(c => c.LETTER_ID).Max() + 1) : 1,
                                        LETTER_ID = letterModel.LETTER_ID,
                                        TEAM_CODE = teamCode
                                    };

                                    dbContext.QUALITY_QM_TEAM_LETTER.Add(teamLetter);
                                    dbContext.SaveChanges();
                                }

                                letterId = letterModel.LETTER_ID;
                                letterModelQC.QC_TYPE = "S";
                                letterModelQC.FILE_NAME = teamCode;
                                letterModelQC.LETTER_ID = letterModel.LETTER_ID;
                                // Commented by srishti on 13-0-2023
                                //qmBAL.SendMailCustomFuncToTeam(letterModelQC, ref ErrorMessage).Send();

                                // Added by Srishti on 13-03-2023
                                ServicePointManager.ServerCertificateValidationCallback += (s, ce, ca, p) => true;
                                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                                MailMessage ms = qmBAL.SendMailCustomFuncToTeam(letterModelQC, ref ErrorMessage);

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

                                //-------------------------- SQC Letter Code -----------------------------------//
                                isLetterGenerated = true;
                            }



                            qmLetterModel.SCHEDULE_CODE = item.ADMIN_SCHEDULE_CODE;
                            qmLetterModel.FILE_NAME = teamCode;
                            qmLetterModel.LETTER_ID = letterId;
                            qmLetterModel.QC_TYPE = "I";

                            // Commented by srishti on 13-0-2023
                            //qmBAL.SendMailCustomFuncToTeam(qmLetterModel, ref ErrorMessage).Send();

                            // Added by Srishti on 13-03-2023
                            ServicePointManager.ServerCertificateValidationCallback += (s, ce, ca, p) => true;
                            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                            MailMessage mailmessage = qmBAL.SendMailCustomFuncToTeam(qmLetterModel, ref ErrorMessage);

                            SmtpClient client2 = new SmtpClient();

                            string e_EuthHost2 = ConfigurationManager.AppSettings["e_EuthHost"];
                            string e_EuthPort2 = ConfigurationManager.AppSettings["e_EuthPort"];
                            string e_EuthMailUserName2 = ConfigurationManager.AppSettings["e_EuthMailUserName"];
                            string e_EuthMailPassword2 = ConfigurationManager.AppSettings["e_EuthMailPassword"];

                            client2.Host = e_EuthHost2;
                            client2.Port = Convert.ToInt32(e_EuthPort2);
                            client2.UseDefaultCredentials = false;
                            client2.EnableSsl = true; // Change to true
                            client2.Credentials = new NetworkCredential(e_EuthMailUserName2, e_EuthMailPassword2);
                            client2.Send(mailmessage);

                            transction.Complete();
                            isMailSent = (isMailSent == false ? true : true); // for one record mail sent then it becomes true always
                        }//Transaction ENds
                    }//Foreach Ends

                    if (isMailSent)
                    {
                        QUALITY_QM_LETTER letterDetailsToUpdate = dbContext.QUALITY_QM_LETTER.Find(letterId);
                        letterDetailsToUpdate.MAIL_DELIVERY_DATE = DateTime.Now;
                        letterDetailsToUpdate.MAIL_DELIVERY_STATUS = true;
                        dbContext.Entry(letterDetailsToUpdate).State = System.Data.Entity.EntityState.Modified;
                        dbContext.SaveChanges();
                    }

                    return Json(new { Success = true, Message = "Email Sent Successfully." }, JsonRequestBehavior.AllowGet);
                }

                return Json(new { Success = false, Message = "Error occurred while processing request" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { Success = false, Message = "Error occurred while processing request" }, JsonRequestBehavior.AllowGet);
            }
        }

        #endregion

        #region BankDetails QM
        /// <summary>
        /// This method is for loading the list of contractor bank details.
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Returns view of list</returns>
        [HttpGet]
        public ActionResult QMListBankDetails(/*String id*/)
        {
            Dictionary<string, string> decryptedParameters = null;
            String[] encryptedParameters = null;
            QMBankDetailsModel model = new QMBankDetailsModel();
            int ContractorId = 0;
            int RegState = 0;
            try
            {
                //if (id != string.Empty)
                //{
                //    encryptedParameters = id.Split('/');

                //    if (encryptedParameters.Length == 3)
                //    {
                //        decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { encryptedParameters[0], encryptedParameters[1], encryptedParameters[2] });
                //        //ViewBag.ContractorId = decryptedParameters["ContRegID"].ToString();
                //        //ViewData["RegStateCode"] = decryptedParameters["RegState"].ToString();
                //        //ContractorId = Convert.ToInt32(decryptedParameters["ContRegID"]);
                //        //RegState = Convert.ToInt32(decryptedParameters["RegState"]);

                //        contractorBankDetails.encrNodalOfficerCode = id;
                //        contractorBankDetails.NodalOfficerCode = Convert.ToInt32(decryptedParameters["NodalOfficerCode"]);
                //    }

                //    return PartialView("QMListBankDetails", contractorBankDetails);
                //}
                dbContext = new PMGSYEntities();
                model.ADMIN_QM_CODE = dbContext.ADMIN_QUALITY_MONITORS.Where(x => x.ADMIN_USER_ID == PMGSYSession.Current.UserId).Select(x => x.ADMIN_QM_CODE).FirstOrDefault();
                return PartialView("QMListBankDetails", model);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return PartialView("QMListBankDetails", model);
            }
        }

        /// <summary>
        /// This method is for loading the Add functionality form.
        /// </summary>
        /// <param name="id"></param>
        /// <returns>View of Add screen</returns>
        [HttpGet]
        public ActionResult AddBankDetailsQM(string id)
        {
            //PMGSY.DAL.QualityMonitoring.QualityMonitoringDAL objDAL = new PMGSY.DAL.QualityMonitoring.QualityMonitoringDAL();
            PMGSY.DAL.Master.MasterDAL objDAL = new PMGSY.DAL.Master.MasterDAL();
            QMBankDetailsModel model = new QMBankDetailsModel();
            string[] strsplit = id.Split('$');
            ViewBag.NodalOfficerCode = strsplit[0].ToString();
            List<MASTER_DISTRICT> lstDistricts = null;
            List<MASTER_STATE> lstState = null;
            lstState = objDAL.GetStates();
            lstState.Insert(0, new MASTER_STATE() { MAST_STATE_CODE = 0, MAST_STATE_NAME = "--Select--" });
            int stateCode = 0;

            stateCode = PMGSYSession.Current.StateCode;
            lstDistricts = objDAL.GetAllDistrictsByStateCode(stateCode);
            lstDistricts.Insert(0, new MASTER_DISTRICT() { MAST_DISTRICT_CODE = 0, MAST_DISTRICT_NAME = "--Select--" });

            model.Mast_State_Code = stateCode;
            ViewData["Districts"] = new SelectList(lstDistricts, "MAST_DISTRICT_CODE", " MAST_DISTRICT_NAME");
            ViewData["State"] = new SelectList(lstState, "MAST_STATE_CODE", " MAST_STATE_NAME", stateCode);

            model.ADMIN_QM_CODE = Convert.ToInt32(id);
            return PartialView("AddBankDetailsQM", model);
        }

        /// <summary>
        /// This method is for Adding the bank details
        /// </summary>
        /// <param name="contractorBankDetails"></param>
        /// <returns>View for Add form </returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddBankDetailsQM(QMBankDetailsModel model)
        {
            bool status = false;
            string message = string.Empty;
            try
            {
                PMGSY.BAL.QualityMonitoring.QualityMonitoringBAL objBAL = new PMGSY.BAL.QualityMonitoring.QualityMonitoringBAL();
                //PMGSY.BAL.Master.MasterBAL objBAL = new PMGSY.BAL.Master.MasterBAL();
                if (ModelState.IsValid)
                {
                    if (objBAL.AddBankDetailsQM(model, ref message))
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
                    return PartialView("AddBankDetailsQM", model);
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

        /// <summary>
        /// This method is for checking existing bank details.
        /// </summary>
        /// <param name="contractorBankDetails"></param>
        /// <returns>Returns true if  bank details already exists.</returns>
        [HttpPost]
        public ActionResult CheckExistRecordQM(QMBankDetailsModel model)
        {
            bool status = true;
            bool alreadyExists = false;
            bool isBankDetailsExists = false;
            string message = string.Empty;

            try
            {
                if (ModelState.IsValid)
                {
                    PMGSY.DAL.QualityMonitoring.QualityMonitoringDAL objDAL = new PMGSY.DAL.QualityMonitoring.QualityMonitoringDAL();

                    //Added By Abhishke kamble 20-feb-2014 
                    if (objDAL.IsBankDetailsExistsQM(model, ref message))
                    {
                        status = true;
                        return Json(new { success = status, isBankDetailsExists = true, message = message });
                    }

                    if (objDAL.checkAlreadyExistsQM(model, ref message))
                    {
                        status = true;
                        alreadyExists = true;
                    }
                }

                return Json(new { success = status, alreadyExists = alreadyExists, isBankDetailsExists = isBankDetailsExists }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                message = "Bank details not saved.";
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// This method is to delete the bank details.
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="hash"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult DeleteBankDetailsQM(String parameter, String hash, String key)
        {
            Dictionary<string, string> decryptedParameters = null;
            bool status = false;
            string message = string.Empty;
            try
            {
                PMGSY.BAL.QualityMonitoring.QualityMonitoringBAL objBAL = new PMGSY.BAL.QualityMonitoring.QualityMonitoringBAL();
                decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { parameter, hash, key });
                int AccountCode = Convert.ToInt32(decryptedParameters["AccountCode"]);
                int AdminQMCode = Convert.ToInt32(decryptedParameters["AdminQMCode"]);

                if (decryptedParameters.Count() > 0)
                {
                    if (objBAL.DeleteBankDetailsQM(AccountCode, AdminQMCode, ref message))
                    {
                        message = "Bank details deleted successfully.";
                        status = true;
                    }
                    else
                    {
                        message = message == string.Empty ? "You can not delete this Bank details." : message;
                    }
                    return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
                }
                message = "You can not delete this Bank details.";
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                message = "You can not delete this Bank details.";
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// This method is loading the grid for bank details.
        /// </summary>
        /// <param name="homeFormCollection"></param>
        /// <returns></returns>


        public ActionResult GetContractorBankDetailsQM(int? page, int? rows, string sidx, string sord)
        {
            long totalRecords;
            string encryptedParameters = "";
            Dictionary<string, string> decryptedParameters = null;
            int adminQmCode = 0;

            try
            {
                //Added By Abhishek kamble 29-Apr-2014 start
                using (CommonFunctions commonFunction = new CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new GridParams(page, rows, sidx, sord, Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                    {
                        return null;
                    }
                }
                //Added By Abhishek kamble 29-Apr-2014 end

                PMGSY.BAL.QualityMonitoring.QualityMonitoringBAL objBAL = new PMGSY.BAL.QualityMonitoring.QualityMonitoringBAL();
                //encryptedParameters = Request.Params["adminQMCode"].Split('/');
                //if (encryptedParameters.Length == 3)
                //{
                //    decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { encryptedParameters[0], encryptedParameters[1], encryptedParameters[2] });
                //    NodalOfficerCode = Convert.ToInt32(decryptedParameters["NodalOfficerCode"].ToString());
                //}
                adminQmCode = Convert.ToInt32(Request.Params["adminQMCode"]);
                var jsonData = new
                {
                    rows = objBAL.ListBankDetailsQM(adminQmCode, Convert.ToInt32(page) - 1, Convert.ToInt32(rows), sidx, sord, out totalRecords),
                    total = totalRecords <= Convert.ToInt32(rows) ? 1 : totalRecords / Convert.ToInt32(rows) + 1,
                    page = Convert.ToInt32(page),
                    records = totalRecords
                };

                return Json(jsonData, JsonRequestBehavior.AllowGet);
                //return Json("", JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return null;
            }
        }
        #endregion BankDetails

        #region Joint Inspection

        public ActionResult QMJILayout()
        {
            QMJIFilterModel model = new QMJIFilterModel();
            CommonFunctions objCommon = new CommonFunctions();
            if (PMGSYSession.Current.RoleCode == 22 || PMGSYSession.Current.RoleCode == 38 || PMGSYSession.Current.RoleCode == 54) //PIU OR PIUOA
            {
                //model.StateList = objCommon.PopulateStates(false);
                //model.DistrictList = objCommon.PopulateDistrict(PMGSYSession.Current.StateCode, true);
                model.StateCode = PMGSYSession.Current.StateCode;
                model.StateName = PMGSYSession.Current.StateName;
                model.DistrictCode = PMGSYSession.Current.DistrictCode;
                model.DistrictName = PMGSYSession.Current.DistrictName;
                model.BlockList = objCommon.PopulateBlocks(PMGSYSession.Current.DistrictCode, true);
                model.BlockList.Find(x => x.Value == "-1").Value = "0";
            }

            model.ProposalTypeList = objCommon.PopulateProposalTypes();
            model.InspectionStatusList = new List<SelectListItem>();
            model.InspectionStatusList.Add(new SelectListItem() { Selected = true, Text = "All", Value = "A" });
            model.InspectionStatusList.Add(new SelectListItem() { Selected = true, Text = "Yes", Value = "Y" });
            model.InspectionStatusList.Add(new SelectListItem() { Selected = true, Text = "No", Value = "N" });

            return View(model);
        }

        [HttpPost]
        [Audit]
        public ActionResult QMJIList(int block, string ptype, string inspstatus, int page, int rows, string sidx, string sord)
        {
            QualityMonitoringBAL objBAL = new QualityMonitoringBAL();
            if (PMGSYSession.Current == null)
            {
                Response.Redirect("/Login/Login");
            }

            int totalRecords;
            var jsonData = new
            {
                rows = objBAL.GetJointInspectionDetailsList(block, ptype, inspstatus, page, rows, sidx, sord, out totalRecords),
                total = 0 <= rows ? 1 : totalRecords / rows + 1,
                page = page,
                records = totalRecords,
            };
            return Json(jsonData);
        }

        /// <summary>
        /// View Generated Letter in new window
        /// </summary>
        /// <returns></returns>
        public ActionResult GenerateJIFormat(int id)
        {
            dbContext = new PMGSYEntities();
            try
            {
                QualityMonitoringBAL objBAL = new QualityMonitoringBAL();
                byte[] byteArray = objBAL.GenerateJointInspectionPDF(id);
                MemoryStream pdfStream = new MemoryStream();
                pdfStream.Write(byteArray, 0, byteArray.Length);
                pdfStream.Position = 0;

                //return new FileStreamResult(pdfStream, "application/pdf");
                return File(pdfStream, "application/pdf", "JointInspectionFormat.pdf");

            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return Json(new { Success = "false" }, JsonRequestBehavior.AllowGet);
            }
            finally
            {
                dbContext.Dispose();
            }
        }
        public ActionResult QMJIDownloadPDF(int id)
        {
            try
            {
                dbContext = new PMGSYEntities();
                string FullfilePhysicalPath = string.Empty;
                string FileExtension = string.Empty;
                string PhysicalPath = PhysicalPath = ConfigurationManager.AppSettings["QM_JI_FILE_UPLOAD"];
                //string VirtualPath = ConfigurationManager.AppSettings["QM_JI_FILE_VIRTUAL_UPLOAD"];
                string FileName = dbContext.QUALITY_QM_JOINT_INSPECTION_FILE.Find(id).QM_JNT_INSP_FILE_NAME;
                FileExtension = Path.GetExtension(FileName).ToLower();
                FullfilePhysicalPath = Path.Combine(PhysicalPath, FileName);
                
                if (System.IO.File.Exists(FullfilePhysicalPath))
                {
                    return File(FullfilePhysicalPath, "Application/pdf", DateTime.Now.ToShortDateString() + "_" + new Random().Next(1000000000) + FileExtension);
                }
                else
                {
                    return Json(new { Success = "false", ErrorMessage = "File Not Found." }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return Json(new { Success = "false", ErrorMessage = "Error While Processing Your Request." }, JsonRequestBehavior.AllowGet);
            }
        }

        [Audit]
        public ActionResult AddJIDetails(int id)//id : It is ims_pr_road_code,
        {
            try
            {
                CommonFunctions comm = new CommonFunctions();
                QualityMonitoringBAL objBAL = new QualityMonitoringBAL();
                QMJIViewModel model = new QMJIViewModel();
                model = objBAL.QMJIHeader(id);
                model.roadCode = id;
                model.dbOperation = "A";

                return PartialView("AddEditJIDetails", model);

            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "AddJIDetails");
                return null;
            }
            finally
            {

            }
        }

        [Audit]
        public ActionResult EditJIDetails(int id)//id : It is ims_pr_road_code,
        {
            try
            {
                CommonFunctions comm = new CommonFunctions();
                QualityMonitoringBAL objBAL = new QualityMonitoringBAL();
                QMJIViewModel model = objBAL.GetJIDetailsBAL(id);
                model.dbOperation = "E";
                return PartialView("AddEditJIDetails", model);
            }
            catch (Exception ex)
            {
                return null;
            }
            finally
            {

            }
        }

        [HttpPost]
        [Audit]
        [ValidateAntiForgeryToken]
        public ActionResult AddEditJIDetails(QMJIViewModel model)
        {
            //var df=model.progressStatus;


            string message = string.Empty;
            QualityMonitoringBAL objBAL = new QualityMonitoringBAL();

            CommonFunctions comm = new CommonFunctions();
            bool flag = false;
            string[] arr = null;

            bool fileExt = false;

            HttpPostedFileBase file = null;
            string fileTypes = string.Empty;

            string fileId = string.Empty;
            int fileSize = 0;

            string filePath = string.Empty;
            string fileSaveExt = string.Empty;
            int fileValidSize = 0;
            string status = string.Empty;

            try
            {
                // if (ModelState.IsValid)
                {

                    #region File Upload
                    if (Request.Files.AllKeys.Count() > 0)
                    {
                        file = Request.Files[0];
                        if (file != null)
                        {
                            //ModelState.Add("tourReport");
                            fileTypes = ConfigurationManager.AppSettings["QM_JI_FILE_VALID_FORMAT"];
                            filePath = ConfigurationManager.AppSettings["QM_JI_FILE_UPLOAD"];
                            fileSize = Convert.ToInt32(ConfigurationManager.AppSettings["QM_JI_FILE_MAX_SIZE"]);

                            fileExt = true;
                            fileSaveExt = Path.GetExtension(file.FileName);//file.ContentType;
                            fileSaveExt = fileSaveExt.Contains('.') ? fileSaveExt.Trim().Remove(0, 1) : fileSaveExt;
                            fileValidSize = file.ContentLength;

                            if (fileValidSize > fileSize)
                            {
                                message = message == string.Empty ? "Pdf files upto " + fileSize / (1024 * 1024) + " MB are allowed." : message;
                                return Json(new { success = false, message = message });
                            }

                            arr = fileTypes.Split('$');
                            for (int i = 0; i < arr.Length; i++)
                            {
                                if (fileSaveExt == arr[i])
                                {
                                    flag = true;
                                }
                            }
                            if (flag)
                            {
                                //  model.inspectionFileName = file.FileName;
                                model.inspectionFileName = model.roadCode.ToString() + DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Day.ToString() + DateTime.Now.Hour.ToString() + DateTime.Now.Minute.ToString() + DateTime.Now.Second.ToString() + "." + fileSaveExt;

                                if (objBAL.SaveQMJointInspectionDetailsBAL(model, ref message))
                                {

                                    if (Request.Files.AllKeys.Count() > 0)
                                    {
                                        Request.Files[0].SaveAs(Path.Combine(filePath, model.inspectionFileName));
                                    }
                                    message = message == string.Empty ? "Joint Inspection details saved successfully." : message;
                                    return Json(new { success = true, message = message });
                                }
                                else
                                {
                                    message = message == string.Empty ? "Joint Inspection details not saved." : message;
                                    return Json(new { success = false, message = message });
                                }
                            }
                            else
                            {
                                message = message == string.Empty ? "Invalid file format, only pdf files allowed." : message;
                                return Json(new { success = false, message = message });
                            }
                        }
                        else
                        {
                            message = message == string.Empty ? "Joint Inspection details not saved, please select a pdf file" : message;
                            return Json(new { success = false, message = message });
                        }
                    }
                    else
                    {
                        message = message == string.Empty ? "Joint Inspection details not saved, please select a pdf file" : message;
                        return Json(new { success = false, message = message });
                    }
                    #endregion

                }
                /*else
                {
                    string messages = string.Join("; ", ModelState.Values
                                           .SelectMany(x => x.Errors)
                                           .Select(x => x.ErrorMessage));

                    //return PartialView("AddJointInspectionDetailsLayout", model);
                    return Json(new { success = false, message = messages });
                }*/
            }
            catch (DbEntityValidationException ex)
            {
                //Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                foreach (var eve in ex.EntityValidationErrors)
                {
                    ModelState.AddModelError("", eve.ValidationErrors.ToString());
                    message += eve.ValidationErrors.ToString();
                }
                ErrorLog.LogError(ex, "AddEditJIDetails().DbEntityValidationException");
                return Json(new { success = false, message = message }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                //Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                ErrorLog.LogError(ex, "AddEditJIDetails()");
                message = "Joint Inspection details not saved because " + ex.Message;
                return Json(new { success = false, message = message }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        [Audit]
        //[ValidateAntiForgeryToken]
        public ActionResult UpdateQMJointInspectionDetails(QMJIViewModel model)
        {
            string message = string.Empty;
            QualityMonitoringBAL objBAL = new QualityMonitoringBAL();

            try
            {
                bool flag = false;
                string[] arr = null;

                HttpPostedFileBase file = null;
                string fileTypes = string.Empty;

                string fileId = string.Empty;
                int fileSize = 0;

                string filePath = string.Empty;
                string fileSaveExt = string.Empty;
                int fileValidSize = 0;
                string status = string.Empty;

                if (Request.Files.AllKeys.Count() > 0)
                {
                    file = Request.Files[0];
                    if (file != null)
                    {
                        //ModelState.Add("tourReport");
                        fileTypes = ConfigurationManager.AppSettings["QM_JI_FILE_VALID_FORMAT"];
                        filePath = ConfigurationManager.AppSettings["QM_JI_FILE_UPLOAD"];
                        fileSize = Convert.ToInt32(ConfigurationManager.AppSettings["QM_JI_FILE_MAX_SIZE"]);

                        fileSaveExt = Path.GetExtension(file.FileName);//file.ContentType;
                        fileSaveExt = fileSaveExt.Contains('.') ? fileSaveExt.Trim().Remove(0, 1) : fileSaveExt;
                        fileValidSize = file.ContentLength;

                        if (fileValidSize > fileSize)
                        {
                            message = message == string.Empty ? "Pdf files upto " + fileSize / (1024 * 1024) + " MB are allowed." : message;
                            return Json(new { success = false, message = message });
                        }
                        arr = fileTypes.Split('$');
                        for (int i = 0; i < arr.Length; i++)
                        {
                            if (fileSaveExt == arr[i])
                            {
                                flag = true;
                            }
                        }
                        if (flag)
                        {
                            model.inspectionFileName = model.roadCode.ToString() + DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Day.ToString() + DateTime.Now.Hour.ToString() + DateTime.Now.Minute.ToString() + DateTime.Now.Second.ToString() + "." + fileSaveExt;

                            message = objBAL.UpdateQMJointInspectionDetailsBAL(model);
                            if (message == string.Empty)
                            {
                                var PhysicalPath = Path.Combine(ConfigurationManager.AppSettings["QM_JI_FILE_UPLOAD"], model.previousFileName);
                                if (System.IO.File.Exists(PhysicalPath))
                                {
                                    System.IO.File.Delete(PhysicalPath);
                                }
                                Request.Files[0].SaveAs(Path.Combine(filePath, model.inspectionFileName));
                                message = message == string.Empty ? "Joint Inspection details updated successfully." : message;
                                return Json(new { success = true, message = message });
                            }
                        }
                    }
                }
                else
                {
                    message = objBAL.UpdateQMJointInspectionDetailsBAL(model);
                    if (message == string.Empty)
                    {
                        message = message == string.Empty ? "Joint Inspection details updated successfully." : message;
                        return Json(new { success = true, message = message });
                    }
                }
                message = message == string.Empty ? "Joint Inspection details not updated." : message;
                return Json(new { success = false, message = message });
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "UpdateQMJointInspectionDetails()");
                return Json(new { success = false, message = "Error occured on Joint Inspection update" });
            }
            finally
            {
            }
        }

        [HttpPost]
        [Audit]
        //[ValidateAntiForgeryToken]
        public ActionResult QMJIDelete(int id)
        {
            string message = string.Empty;
            QualityMonitoringBAL objBAL = new QualityMonitoringBAL();
            try
            {
                message = objBAL.QMJIDeleteBAL(id);
                if (message == string.Empty)
                {
                    message = message == string.Empty ? "Joint Inspection details Delete successfully." : message;
                    return Json(new { success = true, message = message });
                }
                else
                {
                    message = "Joint Inspection details not Deleted.";
                    return Json(new { success = false, message = message });
                }
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "QMJIDelete()");
                return Json(new { success = false, message = "Error occured on Joint Inspection update" });
            }
            finally
            {
            }
        }

        [Audit]
        public ActionResult GetQMJIDetail(int id)
        {
            try
            {
                QualityMonitoringBAL objBAL = new QualityMonitoringBAL();
                QMJIViewModel model = objBAL.GetJIDetailsBAL(id);
                model.mpName = model.mpName == string.Empty ? "---" : model.mpName;
                model.mlaName = model.mlaName == string.Empty ? "---" : model.mlaName;
                model.gpName = model.gpName == string.Empty ? "---" : model.gpName;
                model.otherRepresentativeName = model.otherRepresentativeName == string.Empty ? "---" : model.otherRepresentativeName;

                model.seName = model.seName == string.Empty ? "---" : model.seName;
                model.piuName = model.piuName == string.Empty ? "---" : model.piuName;
                model.aeName = model.aeName == string.Empty ? "---" : model.aeName;
                model.districtOfficerName = model.districtOfficerName == string.Empty ? "---" : model.districtOfficerName;

                model.contractorName = model.contractorName == string.Empty ? "---" : model.contractorName;

                model.serveConnectivity = model.serveConnectivity == "Y" ? "Yes" : "No";
                model.workProgressSatisfactory = model.workProgressSatisfactory == "Y" ? "Yes" : "No";
                model.cdWorkSufficient = model.cdWorkSufficient == "Y" ? "Yes" : "No";
                model.variationExecLengthReason = model.variationExecLengthReason == string.Empty ? "---" : model.variationExecLengthReason;
                //model.inspectionDate = model.inspectionDate;
                // model.qualityGrading = model.qualityGrading ==  "Y" ? "Yes" : "No";
                switch (model.qualityGrading)
                {
                    case "G":
                        model.qualityGrading = "Good";
                        break;
                    case "I":
                        model.qualityGrading = "Improvment Required";
                        break;
                    case "U":
                        model.qualityGrading = "Unsatisfactory";
                        break;
                }
                return PartialView(model);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "GetQMJIDetail()");
                return null;
            }
        }

        [Audit]
        public ActionResult QMJIATRLayout(int id)
        {
            try
            {
                ViewBag.jicode = id;
                return PartialView();
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "QMJIATRLayout()");
                return null;
            }
        }

        [Audit]
        public ActionResult QMJIATRAdd(int id)
        {
            dbContext = new PMGSYEntities();
            try
            {
                QMJIATRModel model = new QMJIATRModel();
                model.jiCode = id;
                model.ATRStatus = "P";

                var atrToList = dbContext.QUALITY_QM_JOINT_INSPECTION_ATR.Where(m => m.QM_JNT_INSP_CODE == id).ToList();
                if (atrToList.Count > 0)
                {
                    foreach (var itm in atrToList)
                    {
                        if (itm.QM_JNT_INSP_STATUS == "F")
                        {
                            model.qmATRStatus = itm.QM_JNT_INSP_STATUS;
                        }
                        model.prevActionTakendDate = Convert.ToString(itm.IMS_JNT_INSP_ACTION_DATE.Value.AddDays(1));
                    }
                }
                return PartialView(model);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "QMJIATRAdd()");
                return null;
            }
            finally
            {
                if (dbContext != null)
                {
                    dbContext.Dispose();
                }
            }
        }

        [Audit]
        [HttpPost]
        public ActionResult QMJIATRAdd(QMJIATRModel model)
        {
            string message = string.Empty;
            try
            {
                QualityMonitoringBAL objBAL = new QualityMonitoringBAL();

                bool flag = false;
                string[] arr = null;

                HttpPostedFileBase file = null;
                string fileTypes = string.Empty;

                string fileId = string.Empty;
                int fileSize = 0;

                string filePath = string.Empty;
                string fileSaveExt = string.Empty;
                int fileValidSize = 0;
                string status = string.Empty;

                if (Request.Files.AllKeys.Count() > 0)
                {
                    file = Request.Files[0];
                    if (file != null)
                    {

                        fileTypes = ConfigurationManager.AppSettings["QM_JI_FILE_VALID_FORMAT"];
                        filePath = ConfigurationManager.AppSettings["QM_JI_FILE_UPLOAD_ATR"];
                        fileSize = Convert.ToInt32(ConfigurationManager.AppSettings["QM_JI_FILE_MAX_SIZE"]);

                        fileSaveExt = Path.GetExtension(file.FileName);//file.ContentType;
                        fileSaveExt = fileSaveExt.Contains('.') ? fileSaveExt.Trim().Remove(0, 1) : fileSaveExt;
                        fileValidSize = file.ContentLength;

                        if (fileValidSize > fileSize)
                        {
                            message = message == string.Empty ? "Pdf files upto " + fileSize / (1024 * 1024) + " MB are allowed." : message;
                            return Json(new { success = false, message = message });
                        }

                        arr = fileTypes.Split('$');
                        for (int i = 0; i < arr.Length; i++)
                        {
                            if (fileSaveExt == arr[i])
                            {
                                flag = true;
                            }
                        }
                        if (flag)
                        {
                            //  model.inspectionFileName = file.FileName;
                            model.ATRFileName = model.jiCode.ToString() + DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Day.ToString() + DateTime.Now.Hour.ToString() + DateTime.Now.Minute.ToString() + DateTime.Now.Second.ToString() + "." + fileSaveExt;

                            message = objBAL.QMJIATRAddBAL(model);
                            if (message == string.Empty)
                            {
                                Request.Files[0].SaveAs(Path.Combine(filePath, model.ATRFileName));
                                message = message == string.Empty ? "Action Taken details saved successfully." : message;

                                return Json(new { success = true, message = message });
                            }
                            else
                            {
                                message = message == string.Empty ? "Action Taken details not saved." : message;
                                return Json(new { success = false, message = message });
                            }
                        }
                        else
                        {
                            message = message == string.Empty ? "Invalid format, only pdf allowed." : message;
                            return Json(new { success = false, message = message });
                        }
                    }
                    else
                    {
                        message = message == string.Empty ? "Please select Action Taken Report." : message;
                        return Json(new { success = false, message = message });
                    }
                }
                else
                {
                    message = message == string.Empty ? "Please select Action Taken Report." : message;
                    return Json(new { success = false, message = message });
                }

                //if (flag)
                //{
                //    if (Request.Files.AllKeys.Count() > 0)
                //    {

                //        objBAL.QMJIATRAddBAL(model);
                //        Request.Files[0].SaveAs(Path.Combine(filePath, model.ATRFileName));
                //    }

                //    message = message == string.Empty ? "Action Taken details saved successfully." : message;
                //    return Json(new { success = true, message = message });

                //}
                //else
                //{
                //    message = message == string.Empty ? "Action Taken details not saved." : message;
                //    return Json(new { success = false, message = message });
                //}
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "QMJIATRAdd(QMJIATRModel model)");
                message = message == string.Empty ? "Action Taken details not saved." : message;
                return Json(new { success = false, message = message });
            }
        }

        [Audit]
        [HttpPost]
        public ActionResult QMJIATRDelete(int id)
        {
            string message = string.Empty;
            try
            {

                QualityMonitoringBAL objBAL = new QualityMonitoringBAL();
                objBAL.QMJIATRDeleteBAL(id);
                message = "Action Taken Details Deleted Successfully.";
                return Json(new { success = true, message = message });
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "QMJIATRDelete()");
                message = "Action Taken Details not Deleted.";
                return Json(new { success = false, message = message });
            }
        }

        [Audit]
        public ActionResult QMJIATRList(int id)
        {
            string message = string.Empty;
            try
            {
                QualityMonitoringBAL objBAL = new QualityMonitoringBAL();
                return PartialView(objBAL.QMJIATRListBAL(id));
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "QMJIATRList()");
                return null;
            }
        }

        [Audit]
        public ActionResult QMJIATRDownloadPdf(int id)
        {
            try
            {
                dbContext = new PMGSYEntities();
                string FullfilePhysicalPath = string.Empty;
                string FileExtension = string.Empty;
                string PhysicalPath = PhysicalPath = ConfigurationManager.AppSettings["QM_JI_FILE_UPLOAD_ATR"];
                string FileName = dbContext.QUALITY_QM_JOINT_INSPECTION_ATR.Find(id).QM_JNT_INSP_FILE_NAME;
                FileExtension = Path.GetExtension(FileName).ToLower();
                FullfilePhysicalPath = Path.Combine(PhysicalPath, FileName);
                if (System.IO.File.Exists(FullfilePhysicalPath))
                {
                    return File(FullfilePhysicalPath, "Application/pdf", DateTime.Now.ToShortDateString() + "_" + new Random().Next(1000000000) + FileExtension);
                }
                else
                {
                    return Json(new { Success = "false", ErrorMessage = "File Not Found." }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "QMJIATRDownloadPdf()");
                return Json(new { Success = "false", ErrorMessage = "Error While Processing Your Request." }, JsonRequestBehavior.AllowGet);
            }
        }

        [Audit]
        public ActionResult QMJIDetailsLayout(int id)
        {
            string message = string.Empty;
            try
            {
                ViewBag.prRoadCode = id;
                return PartialView("QMJIDetailsLayout");
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "QMJIDetailsLayout()");
                return null;
            }
        }

        [HttpPost]
        [Audit]
        public ActionResult ListJIDetails(int prRoadCode, int page, int rows, string sidx, string sord)
        {
            QualityMonitoringBAL objBAL = new QualityMonitoringBAL();
            if (PMGSYSession.Current == null)
            {
                Response.Redirect("/Login/Login");
            }

            int totalRecords;
            var jsonData = new
            {
                rows = objBAL.GetJIDetailsListBAL(prRoadCode, page, rows, sidx, sord, out totalRecords),
                total = 0 <= rows ? 1 : totalRecords / rows + 1,
                page = page,
                records = totalRecords,
            };
            return Json(jsonData);
        }
        #endregion

        #region Quality Complain
        public ActionResult QMComplainLayout()
        {

            return View();


        }

        public ActionResult GetQMComplainList()
        {
            QMComplainFilterViewModel complainFilterModel = new QMComplainFilterViewModel();
            CommonFunctions common = new CommonFunctions();
            complainFilterModel.RecievedThroughList = common.PopulateQMComplainItem(5);
            complainFilterModel.RecievedThroughList.RemoveAt(0);
            complainFilterModel.RecievedThroughList.Insert(0, new SelectListItem { Text = "All", Value = "0", Selected = true });

            complainFilterModel.statusList = common.PopulateQMComplainItem(18);
            complainFilterModel.statusList.RemoveAt(0);
            complainFilterModel.statusList.Insert(0, new SelectListItem { Text = "All", Value = "0", Selected = true });

            complainFilterModel.FROM_MONTHS_LIST = common.PopulateMonths(DateTime.Now.Month);
            complainFilterModel.TO_MONTHS_LIST = common.PopulateMonths(DateTime.Now.Month);
            complainFilterModel.FROM_YEARS_LIST = common.PopulateYears(DateTime.Now.Year);
            complainFilterModel.TO_YEARS_LIST = common.PopulateYears(DateTime.Now.Year);

            complainFilterModel.FROM_MONTHS_LIST.RemoveAt(0);
            complainFilterModel.TO_MONTHS_LIST.RemoveAt(0);
            complainFilterModel.FROM_YEARS_LIST.RemoveAt(0);
            complainFilterModel.TO_YEARS_LIST.RemoveAt(0);

            if (PMGSYSession.Current.RoleCode == 5 || PMGSYSession.Current.RoleCode == 9)
            {
                complainFilterModel.StateList = common.PopulateStates(false);
                complainFilterModel.StateList.Insert(0, new SelectListItem { Text = "All", Value = "0", Selected = true });
            }
            else
            {
                complainFilterModel.StateCode = PMGSYSession.Current.StateCode;
                complainFilterModel.StateList = new List<SelectListItem>();
                complainFilterModel.StateList.Insert(0, new SelectListItem { Text = PMGSYSession.Current.StateName, Value = PMGSYSession.Current.StateCode.ToString(), Selected = true });
            }
            complainFilterModel.RoleCode = PMGSYSession.Current.RoleCode;
            complainFilterModel.TO_MONTH = DateTime.Now.Month;
            return View(complainFilterModel);
        }

        [HttpPost]
        public JsonResult GetQMComplainList(QMComplainFilterViewModel complainFilterModel)
        {
            QualityMonitoringBAL objBAL = new QualityMonitoringBAL();
            var arrComplain = objBAL.QMComplainListBAL(complainFilterModel);
            var jsonData = new
            {
                rows = arrComplain,
                total = 1,
                page = 1,
                records = arrComplain.Length,
            };
            return Json(jsonData);
        }


        public ActionResult QMComplainCreate()
        {
            QMComplainViewModel complainModel = new QMComplainViewModel();
            CommonFunctions common = new CommonFunctions();
            complainModel.ComplainantList = common.PopulateQMComplainItem(1);
            complainModel.ComplainantList.Find(x => x.Value == "0").Value = "-1";

            complainModel.ForwardedToList = common.PopulateQMComplainItem(9);
            complainModel.ForwardedToList.Find(x => x.Value == "0").Value = "-1";

            complainModel.NatureComplaintList = common.PopulateQMComplainItem(12);
            complainModel.NatureComplaintList.Find(x => x.Value == "0").Value = "-1";

            complainModel.RecievedThroughList = common.PopulateQMComplainItem(5);
            complainModel.RecievedThroughList.Find(x => x.Value == "0").Value = "-1";

            complainModel.StateList = common.PopulateStates(true);
            complainModel.StateList.Find(x => x.Value == "0").Value = "-1";
            return View(complainModel);

        }


        [HttpPost]
        public JsonResult QMComplainCreate(QMComplainViewModel complainModel)
        {
            QualityMonitoringBAL objBAL = new QualityMonitoringBAL();
            objBAL.QMComplainAddBAL(complainModel);

            return Json(new { Success = "true", ErrorMessage = "Record Added Successfully." });

        }


        [HttpPost]
        public JsonResult QMComplainDelete(int id)
        {
            QualityMonitoringBAL objBAL = new QualityMonitoringBAL();
            objBAL.QMComplainDeleteBAL(id);

            return Json(new { Success = "true", ErrorMessage = "Record Deleted Successfully." });

        }


        public ActionResult QMComplainDetail(int id)
        {
            QualityMonitoringBAL objBAL = new QualityMonitoringBAL();

            QMComplainDetailViewModel detailModel = objBAL.GetQMComplainBAL(id);
            return View(detailModel);
        }


        public ActionResult QMComplainUpload(int id)
        {
            QMComplainUploadViewModel uploadModel = new QMComplainUploadViewModel { ComplainId = id };
            return View("QMComplainUpload", uploadModel);
        }

        [HttpPost]
        public ActionResult QMComplainUpload(QMComplainUploadViewModel uploadModel)
        {
            string message = string.Empty;
            try
            {
                QualityMonitoringBAL objBAL = new QualityMonitoringBAL();

                bool flag = false;
                string[] arr = null;

                HttpPostedFileBase file = null;
                string fileTypes = string.Empty;

                string fileId = string.Empty;
                int fileSize = 0;

                string filePath = string.Empty;
                string fileSaveExt = string.Empty;
                int fileValidSize = 0;
                string status = string.Empty;

                if (Request.Files.AllKeys.Count() > 0)
                {
                    file = Request.Files[0];
                    if (file != null)
                    {

                        fileTypes = ConfigurationManager.AppSettings["QM_COMPLAIN_FILE_VALID_FORMAT"];
                        filePath = ConfigurationManager.AppSettings["QM_COMPLAIN_FILE_UPLOAD"];
                        fileSize = Convert.ToInt32(ConfigurationManager.AppSettings["QM_COMPLAIN_FILE_MAX_SIZE"]);


                        fileSaveExt = Path.GetExtension(file.FileName);//file.ContentType;
                        fileSaveExt = fileSaveExt.Contains('.') ? fileSaveExt.Trim().Remove(0, 1) : fileSaveExt;
                        fileValidSize = file.ContentLength;

                        if (fileValidSize > fileSize)
                        {
                            message = message == string.Empty ? "Pdf files upto " + fileSize / (1024 * 1024) + " MB are allowed." : message;
                            return Json(new { success = false, message = message });
                        }

                        arr = fileTypes.Split('$');
                        for (int i = 0; i < arr.Length; i++)
                        {
                            if (fileSaveExt == arr[i])
                            {
                                flag = true;
                            }
                        }
                        if (flag)
                        {
                            //  model.inspectionFileName = file.FileName;
                            uploadModel.FileName = uploadModel.ComplainId.ToString() + uploadModel.StageId.ToString() + DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Day.ToString() + DateTime.Now.Hour.ToString() + DateTime.Now.Minute.ToString() + DateTime.Now.Second.ToString() + DateTime.Now.Millisecond.ToString() + "." + fileSaveExt;

                            if (objBAL.QMComplainFileUploadBAL(uploadModel))
                            {
                                Request.Files[0].SaveAs(Path.Combine(filePath, uploadModel.FileName));
                                message = "Document uploaded successfully.";
                                return Json(new { success = true, message = message });
                            }
                            else
                            {
                                message = message == string.Empty ? "Document not uploaded." : message;
                                return Json(new { success = false, message = message });
                            }
                        }
                        else
                        {
                            message = message == string.Empty ? "Invalid format, only pdf allowed." : message;
                            return Json(new { success = false, message = message });
                        }
                    }
                    else
                    {
                        message = message == string.Empty ? "Please select Document." : message;
                        return Json(new { success = false, message = message });
                    }
                }
                else
                {
                    message = message == string.Empty ? "Please select Document." : message;
                    return Json(new { success = false, message = message });
                }
            }
            catch (Exception ex)
            {
                message = message == string.Empty ? "Document not uploaded." : message;
                return Json(new { success = false, message = message });
            }
        }

        [Audit]
        public ActionResult QMComplainDownloadPdf(int id)
        {
            try
            {
                dbContext = new PMGSYEntities();
                string FullfilePhysicalPath = string.Empty;
                string FileExtension = string.Empty;
                string PhysicalPath = PhysicalPath = ConfigurationManager.AppSettings["QM_COMPLAIN_FILE_UPLOAD"];
                string FileName = dbContext.QUALITY_QM_COMPLAIN_FILE.Find(id).FileName;
                FileExtension = Path.GetExtension(FileName).ToLower();
                FullfilePhysicalPath = Path.Combine(PhysicalPath, FileName);
                if (System.IO.File.Exists(FullfilePhysicalPath))
                {
                    return File(FullfilePhysicalPath, "Application/pdf", DateTime.Now.ToShortDateString() + "_" + new Random().Next(1000000000) + FileExtension);
                }
                else
                {
                    return Json(new { Success = "false", ErrorMessage = "File Not Found." }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return Json(new { Success = "false", ErrorMessage = "Error While Processing Your Request." }, JsonRequestBehavior.AllowGet);
            }

        }
        [HttpPost]
        public ActionResult QMComplainDetailUploadView(QMComplainUploadViewModel uploadModel)
        {
            // QMComplainUploadViewModel uploadModel = new QMComplainUploadViewModel();
            uploadModel.Type = uploadModel.StageId == 26 ? "Y" : "N";
            uploadModel.NRRDAAction = "N";
            switch (uploadModel.StageId)
            {
                case 19:
                    uploadModel.RemarkLabel = "Details";
                    uploadModel.HeaderLabel = "NQM Deployment Details";
                    break;

                case 20:
                    uploadModel.RemarkLabel = "Abstract";
                    uploadModel.HeaderLabel = "NQM Enquiry Finding Details";
                    break;

                case 21:
                    uploadModel.RemarkLabel = "Details";
                    uploadModel.HeaderLabel = "State Forwarding Details";
                    break;

                case 22:
                    uploadModel.RemarkLabel = "Details";
                    uploadModel.HeaderLabel = "Enquiry Team Details";
                    break;

                case 23:
                    uploadModel.RemarkLabel = "Abstract";
                    uploadModel.HeaderLabel = "State Enquiry Finding";
                    break;

                case 24:
                    uploadModel.RemarkLabel = "Abstract";
                    uploadModel.HeaderLabel = "State Enquiry Finding Cover Letter";
                    break;
                case 25:
                    uploadModel.RemarkLabel = "Abstract";
                    uploadModel.HeaderLabel = "State Enquiry Finding";

                    break;
                case 26:
                    uploadModel.RemarkLabel = "Status";
                    uploadModel.HeaderLabel = "Reply to Complainant";
                    break;
            }
            return View("QMComplainDetailUpload", uploadModel);
        }

        [HttpPost]
        public ActionResult QMComplainDetailUpload(QMComplainUploadViewModel uploadModel)
        {
            string message = string.Empty;
            try
            {
                QualityMonitoringBAL objBAL = new QualityMonitoringBAL();

                bool flag = false;
                string[] arr = null;

                HttpPostedFileBase file = null;
                string fileTypes = string.Empty;

                string fileId = string.Empty;
                int fileSize = 0;

                string filePath = string.Empty;
                string fileSaveExt = string.Empty;
                int fileValidSize = 0;
                string status = string.Empty;

                if (Request.Files.AllKeys.Count() > 0)
                {
                    file = Request.Files[0];
                    if (file != null)
                    {
                        fileTypes = ConfigurationManager.AppSettings["QM_COMPLAIN_FILE_VALID_FORMAT"];
                        filePath = ConfigurationManager.AppSettings["QM_COMPLAIN_FILE_UPLOAD_DETAIL"];
                        fileSize = Convert.ToInt32(ConfigurationManager.AppSettings["QM_COMPLAIN_FILE_MAX_SIZE"]);

                        fileSaveExt = Path.GetExtension(file.FileName);//file.ContentType;
                        fileSaveExt = fileSaveExt.Contains('.') ? fileSaveExt.Trim().Remove(0, 1) : fileSaveExt;
                        fileValidSize = file.ContentLength;

                        if (fileValidSize > fileSize)
                        {
                            message = message == string.Empty ? "Pdf files upto " + fileSize / (1024 * 1024) + " MB are allowed." : message;
                            return Json(new { success = false, message = message });
                        }

                        arr = fileTypes.Split('$');
                        for (int i = 0; i < arr.Length; i++)
                        {
                            if (fileSaveExt == arr[i])
                            {
                                flag = true;
                            }
                        }
                        if (flag)
                        {
                            //  model.inspectionFileName = file.FileName;
                            uploadModel.FileName = uploadModel.ComplainId.ToString() + uploadModel.StageId.ToString() + DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Day.ToString() + DateTime.Now.Hour.ToString() + DateTime.Now.Minute.ToString() + DateTime.Now.Second.ToString() + DateTime.Now.Millisecond.ToString() + "." + fileSaveExt;

                            if (objBAL.QMComplainDetailFileUploadBAL(uploadModel))
                            {
                                Request.Files[0].SaveAs(Path.Combine(filePath, uploadModel.FileName));
                                message = "File details saved successfully.";

                                return Json(new { success = true, message = message });
                            }
                            else
                            {
                                message = message == string.Empty ? "File details not saved." : message;
                                return Json(new { success = false, message = message });
                            }
                        }
                        else
                        {
                            message = message == string.Empty ? "Invalid format, only pdf allowed." : message;
                            return Json(new { success = false, message = message });
                        }
                    }
                    else
                    {
                        message = message == string.Empty ? "Please select File." : message;
                        return Json(new { success = false, message = message });
                    }
                }
                else
                {
                    message = message == string.Empty ? "Please select File." : message;
                    return Json(new { success = false, message = message });
                }
            }
            catch (Exception ex)
            {
                message = message == string.Empty ? "File details not saved." : message;
                return Json(new { success = false, message = message });
            }
        }

        [Audit]
        public ActionResult QMComplainDetailStageDownloadPdf(int id)
        {
            try
            {
                dbContext = new PMGSYEntities();
                string FullfilePhysicalPath = string.Empty;
                string FileExtension = string.Empty;
                string PhysicalPath = PhysicalPath = ConfigurationManager.AppSettings["QM_COMPLAIN_FILE_UPLOAD_DETAIL"];
                string FileName = dbContext.QUALITY_QM_COMPLAIN_DETAIL.Find(id).FileName;
                FileExtension = Path.GetExtension(FileName).ToLower();
                FullfilePhysicalPath = Path.Combine(PhysicalPath, FileName);
                if (System.IO.File.Exists(FullfilePhysicalPath))
                {
                    return File(FullfilePhysicalPath, "Application/pdf", DateTime.Now.ToShortDateString() + "_" + new Random().Next(1000000000) + FileExtension);
                }
                else
                {
                    return Json(new { Success = "false", ErrorMessage = "File Not Found." }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return Json(new { Success = "false", ErrorMessage = "Error While Processing Your Request." }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult QMComplainDetailDelete(int id)
        {
            QualityMonitoringBAL objBAL = new QualityMonitoringBAL();
            objBAL.QMComplainDetailDeleteBAL(id);

            return Json(new { Success = "true", ErrorMessage = "Record Successfully Added." });
        }
        #endregion

        #region QualityControlPPT
        public ActionResult QMPhaseProgressInspectionLayout()
        {
            CommonFunctions commonFunctions = new CommonFunctions();
            QMPahseInspectionProgressViewModel qmphase = new QMPahseInspectionProgressViewModel();
            if (PMGSYSession.Current.RoleCode == 8)
            {
                qmphase.lstStates = new List<SelectListItem>();
                qmphase.lstStates.Insert(0, (new SelectListItem { Text = PMGSYSession.Current.StateName, Value = PMGSYSession.Current.StateCode.ToString(), Selected = true }));

                qmphase.lstYears = commonFunctions.PopulateYears();
                qmphase.lstYears.Insert(0, (new SelectListItem { Text = "Select Year", Value = "0", Selected = true }));
            }
            else
            {
                qmphase.lstStates = commonFunctions.PopulateStates(false);
                qmphase.lstStates.Insert(0, (new SelectListItem { Text = "Select State", Value = "0", Selected = true }));
                qmphase.lstYears = commonFunctions.PopulateYears();
                //  qmphase.lstYears.Insert(0, (new SelectListItem { Text = "Select Year", Value = "0", Selected = true }));
            }
            return View(qmphase);
        }

        [HttpPost]
        public ActionResult QMPhaseProgressInspectionReport(QMPahseInspectionProgressViewModel qmphaseProgressInsp)
        {
            try
            {
                if (qmphaseProgressInsp.StateCode > 0)
                {
                    using (var dbContext = new PMGSYEntities())
                    {
                        qmphaseProgressInsp.StateName = dbContext.MASTER_STATE.Where(x => x.MAST_STATE_CODE == qmphaseProgressInsp.StateCode).SingleOrDefault().MAST_STATE_NAME;
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "QMPhaseProgressInspectionReport");
            }
            return View(qmphaseProgressInsp);
        }
        #endregion

        #region Itemwise Grading Details  [Added by Deendayal 03/10/2017]
        public ActionResult QMItemwiseGradingDetailsLayout()
        {
            try
            {
                CommonFunctions commonFunctions = new CommonFunctions();
                QMItemwiseGradingDetailsModel qmgrading = new QMItemwiseGradingDetailsModel();

                qmgrading.yearList = new List<SelectListItem>();

                qmgrading.yearList = commonFunctions.PopulateYears(false);

                // qmgrading.yearList.Insert(0, (new SelectListItem { Text = "Select Year", Value = "0", Selected = true }));

                qmgrading.monthList = new List<SelectListItem>();
                qmgrading.monthList = commonFunctions.PopulateMonths(false);
                // qmgrading.yearList.Insert(0, (new SelectListItem { Text = "Select Month", Value = "0", Selected = true }));

                qmgrading.gradeList = new List<SelectListItem>();

                qmgrading.gradeList.Add(new SelectListItem { Text = "Select Grade", Value = string.Empty });
                qmgrading.gradeList.Add(new SelectListItem { Text = "Satisfactory", Value = "1" });
                qmgrading.gradeList.Add(new SelectListItem { Text = "SRI", Value = "2" });
                qmgrading.gradeList.Add(new SelectListItem { Text = "UnSatisfactory", Value = "3" });
                qmgrading.gradeList.Add(new SelectListItem { Text = "SRI+U", Value = "4" });

                qmgrading.workTypeList = new List<SelectListItem>();

                qmgrading.workTypeList.Add(new SelectListItem { Text = "Select Proposal Type", Value = string.Empty });
                qmgrading.workTypeList.Add(new SelectListItem { Text = "Road", Value = "P" });
                qmgrading.workTypeList.Add(new SelectListItem { Text = "Bridge", Value = "L" });


                qmgrading.workStatusList = new List<SelectListItem>();

                qmgrading.workStatusList.Add(new SelectListItem { Text = "Select Status", Value = string.Empty });
                qmgrading.workStatusList.Add(new SelectListItem { Text = "Completed+Ongoing", Value = "A" });
                qmgrading.workStatusList.Add(new SelectListItem { Text = "Completed", Value = "C" });
                qmgrading.workStatusList.Add(new SelectListItem { Text = "In-Progress", Value = "P" });
                qmgrading.workStatusList.Add(new SelectListItem { Text = "Maintenance", Value = "M" });


                return View(qmgrading);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "QMItemwiseGradingDetailsLayout");
                return null;
            }
        }

        [HttpPost]
        public ActionResult QMItemwiseGradingDetailsReport(QMItemwiseGradingDetailsModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    model.Grade_Code = Convert.ToInt32(model.Grade_Code);
                    return View(model);

                }
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "QMItemwiseGradingDetailsReport");

            }
            // return View(model);
            return null;
        }

        #endregion

        #region Upload Missing Images
        [HttpGet]
        public ActionResult UploadMissingImageLayout(string id)
        {
            try
            {
                QMMissingImagesViewModel model = new QMMissingImagesViewModel();
                model.qmFileName = id;
                model.PrRoadCode = Convert.ToInt32(Request.Params["prRoadCode"]);
                model.QMObsId = Convert.ToInt32(Request.Params["qmObsId"]);
                model.QMSchCode = Convert.ToInt32(Request.Params["qmSchCode"]);
                return View(model);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "QualityMonitoring.UploadMissingImageLayout()");
                return null;
            }
        }

        /// <summary>
        /// Save Details of file tobe uploaded 
        /// Compress file & save to destination
        /// </summary>
        /// <param name="request"></param>
        /// <param name="statuses"></param>
        /// <param name="ADMIN_SCHEDULE_CODE"></param>
        /// <param name="IMS_PR_ROAD_CODE"></param>
        [Audit]
        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult UploadMissingImageFile(QMMissingImagesViewModel model)
        {
            dbContext = new PMGSYEntities();
            qualityBAL = new QualityMonitoringBAL();
            //Array of File Types to Validate             
            String[] fileTypes = new String[] { "jpeg", String.Empty, String.Empty };
            String StorageRoot = string.Empty;
            int MaxFileID = 0;
            try
            {
                ///Path to upload files for NQM/SQM/CQC/SQC
                if (PMGSYSession.Current.RoleCode == 5 || PMGSYSession.Current.RoleCode == 6 || PMGSYSession.Current.RoleCode == 9)   //CQCAdmin or NQM or CQC
                {
                    StorageRoot = ConfigurationManager.AppSettings["QUALITY_INSPECTION_FILE_UPLOAD_NQM"];
                }
                else if (PMGSYSession.Current.RoleCode == 7 || PMGSYSession.Current.RoleCode == 8)   //SQM & SQC
                {
                    StorageRoot = ConfigurationManager.AppSettings["QUALITY_INSPECTION_FILE_UPLOAD_SQM"];
                }

                //model.qmFileName = model.qmFileName == string.Empty ? Convert.ToString(ViewBag.fileName) : model.qmFileName;
                //for (int i = 0; i < request.Files.Count; i++)
                //{

                if (Request.Files.AllKeys.Count() <= 0)
                {
                    return Json(new { success = false, message = "Please select a jpeg file to upload" });
                }

                HttpPostedFileBase file = Request.Files[0];

                //Check for valid Image File
                CommonFunctions objCommonFunc = new CommonFunctions();
                //Check for Valid Image File
                if (!(objCommonFunc.IsValidImageFile(StorageRoot, Request, fileTypes)))
                {
                    return Json(new { success = false, message = "Please select a valid image file" });
                }

                //Check for Image size
                if (file.ContentLength > (4 * 1024 * 1024))
                {
                    return Json(new { success = false, message = "Image files upto 4 MB are allowed." });
                }

                var fullPath = Path.Combine(StorageRoot, model.qmFileName);

                var ThumbnailPath = Path.Combine(StorageRoot, "thumbnails");
                var FullThumbnailPath = Path.Combine(ThumbnailPath, model.qmFileName);

                if (!(Directory.Exists(ThumbnailPath)))
                {
                    Directory.CreateDirectory(ThumbnailPath);
                }

                //if (status == string.Empty)
                {
                    new ProposalBAL().CompressImage(file, fullPath, FullThumbnailPath);
                }

                return Json(new { success = true, message = "File uploaded successfully" });
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "QualityMonitoring.UploadMissingImageFile()");
                return Json(new { success = true, message = "Error occured on file upload" });
            }
            finally
            {
                dbContext.Dispose();
            }
        }
        #endregion

        ///Changes by SAMMED A. PATIL on 08JAN2018 for Missing ATR Files
        #region PdfATRMissingFileUpload
        /// <summary>
        /// Render View to upload ATR Pdfs
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Audit]
        public ActionResult ATRPdfMissingFileUpload(string id)
        {
            dbContext = new PMGSYEntities();
            try
            {
                string[] arrParams = id.Split('$');
                #region
                //PMGSY.Models.QualityMonitoring.FileUploadViewModel fileUploadViewModel = new PMGSY.Models.QualityMonitoring.FileUploadViewModel();

                //Int32 QM_OBSERVATION_ID = Convert.ToInt32(id);
                //fileUploadViewModel.QM_OBSERVATION_ID = QM_OBSERVATION_ID;

                //fileUploadViewModel.ErrorMessage = string.Empty;

                //if (dbContext.QUALITY_ATR_FILE.Where(a => a.QM_OBSERVATION_ID == QM_OBSERVATION_ID).Any())
                //{
                //    var maxAtrId = dbContext.QUALITY_ATR_FILE.Where(a => a.QM_OBSERVATION_ID == QM_OBSERVATION_ID).Select(a => a.QM_ATR_ID).Max();
                //    var maxRecordGrade = dbContext.QUALITY_ATR_FILE.Where(a => a.QM_OBSERVATION_ID == QM_OBSERVATION_ID
                //                                                         && a.QM_ATR_ID == maxAtrId
                //                                                         && a.ATR_IS_DELETED == "N").Select(a => a.ATR_REGRADE_STATUS).FirstOrDefault();
                //    if (maxRecordGrade != null && (maxRecordGrade.Equals("R") || maxRecordGrade.Equals("C")))
                //    {
                //        fileUploadViewModel.NumberofPdfs = 0;
                //    }
                //    else
                //    {
                //        fileUploadViewModel.NumberofPdfs = dbContext.QUALITY_ATR_FILE.Where(a => a.QM_OBSERVATION_ID == QM_OBSERVATION_ID && a.ATR_IS_DELETED == "N").Count();
                //    }
                //}
                //else
                //{
                //    fileUploadViewModel.NumberofPdfs = 0;
                //}
                //return View("ATRPdfMissingFileUpload", fileUploadViewModel);
                #endregion

                ViewBag.qmATRObsId = arrParams[0];
                ViewBag.qmATRFileId = arrParams[1];

                ViewBag.NumberofPdfs = 0;

                return View();
            }
            catch (Exception ex)
            {
                //Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                ErrorLog.LogError(ex, "QualityMonitoring.ATRPdfMissingFileUpload(string id)");
                return Json(string.Empty);
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        /// <summary>
        /// This Method Uploads PDF File
        /// </summary>
        /// <param name="request"></param>
        /// <param name="statuses"></param>
        /// <param name="IMS_PR_ROAD_CODE"></param>
        [Audit]
        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult UploadARMissingPDFFile(FormCollection frmCollection)
        {
            String StorageRoot = ConfigurationManager.AppSettings["QUALITY_INSPECTION_FILE_UPLOAD_ATR"];
            dbContext = new PMGSYEntities();
            qualityBAL = new QualityMonitoringBAL();
            string qmATRFileName = string.Empty;
            int MaxCount = 0;
            try
            {
                HttpPostedFileBase file = Request.Files[0];

                qmATRFileName = frmCollection["qmATRFileId"];

                //Check for valid Image File
                CommonFunctions objCommonFunc = new CommonFunctions();
                if (!(objCommonFunc.ValidateIsPdf(ConfigurationManager.AppSettings["QUALITY_INSPECTION_FILE_UPLOAD_ATR"], Request)))
                {
                    return Json(new { success = false, message = "Please select a valid pdf file" });
                }

                //string status = new ProposalBAL().ValidatePDFFile(file.ContentLength, Path.GetExtension(file.FileName));
                //if (status != string.Empty)
                //{
                //    return Json(new { success = false, message = "Please select a valid pdf file" });
                //}

                //Check for Image size
                if (file.ContentLength > (10 * 1024 * 1024))
                {
                    return Json(new { success = false, message = "Image files upto 10 MB are allowed." });
                }

                var fullPath = Path.Combine(StorageRoot, qmATRFileName);

                //var ThumbnailPath = Path.Combine(StorageRoot, "thumbnails");
                //var FullThumbnailPath = Path.Combine(ThumbnailPath, qmATRFileName);

                //if (!(Directory.Exists(ThumbnailPath)))
                //{
                //    Directory.CreateDirectory(ThumbnailPath);
                //}

                //new ProposalBAL().CompressImage(file, fullPath, FullThumbnailPath);

                //file.SaveAs(Path.Combine(ConfigurationManager.AppSettings["QUALITY_INSPECTION_FILE_UPLOAD_ATR"], fileId.ToString() + ".pdf"));//File Will be saved as FileId.pdf

                file.SaveAs(fullPath + ".pdf");

                return Json(new { success = true, message = "File uploaded successfully" });
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "QualityMonitoring.UploadPDFFile()");
                return Json(new { success = false, message = "Error occured on file upload" });
            }
            finally
            {

            }
        }
        #endregion

        /// Added by deendayal on 22-01-2018 for Maintenance ATRs
        #region Maintenance ATR in SQC

        /// <summary>
        /// Filters for Maintenance ATR Details in SQC login
        /// </summary>//changed by deendayal on 05-02-2018
        /// <returns></returns>
        [Audit]
        public ActionResult QualityMaintenanceATRFilters()
        {
            QMFilterViewModel qmFilterModel = new QMFilterViewModel();
            CommonFunctions objCommonFunctions = new CommonFunctions();

            try
            {

                qmFilterModel.MAST_STATE_CODE = 0;
                qmFilterModel.ADMIN_QM_CODE = 0;
                qmFilterModel.ATR_STATUS = "0";
                qmFilterModel.ROAD_STATUS = "A";

                qmFilterModel.FROM_MONTH = DateTime.Now.Month;
                qmFilterModel.FROM_YEAR = DateTime.Now.Year;
                qmFilterModel.TO_MONTH = DateTime.Now.Month;
                qmFilterModel.TO_YEAR = DateTime.Now.Year;

                //if (PMGSYSession.Current.RoleCode == 5)  //CQC
                if (PMGSYSession.Current.RoleCode == 5 || PMGSYSession.Current.RoleCode == 9)
                {
                    qmFilterModel.STATES = objCommonFunctions.PopulateStates(false);
                    qmFilterModel.STATES.Insert(0, (new SelectListItem { Text = "All", Value = "0", Selected = true }));

                    qmFilterModel.MONITORS = objCommonFunctions.PopulateMonitors("false", "I", 0); //Purposely taken String "false" as argument
                }
                else if (PMGSYSession.Current.RoleCode == 8 || PMGSYSession.Current.RoleCode == 69 || PMGSYSession.Current.RoleCode == 48)  //SQC
                {
                    qmFilterModel.MAST_STATE_CODE = PMGSYSession.Current.StateCode;
                    qmFilterModel.MONITORS = objCommonFunctions.PopulateMonitors("false", "I", PMGSYSession.Current.StateCode); //Purposely taken String "false" as argument
                }
                else if (PMGSYSession.Current.RoleCode == 22 || PMGSYSession.Current.RoleCode == 38 || PMGSYSession.Current.RoleCode == 54)  //PIU or PIURCPLWE
                {
                    qmFilterModel.MAST_STATE_CODE = PMGSYSession.Current.StateCode;
                    qmFilterModel.MONITORS = objCommonFunctions.PopulateMonitorsDistrictWise("false", "I", PMGSYSession.Current.StateCode, PMGSYSession.Current.DistrictCode); //Purposely taken String "false" as argument
                }

                qmFilterModel.FROM_MONTHS_LIST = objCommonFunctions.PopulateMonths(false);
                qmFilterModel.FROM_YEARS_LIST = objCommonFunctions.PopulateYears(false).Where(a => a.Value != "0").ToList();
                qmFilterModel.TO_MONTHS_LIST = objCommonFunctions.PopulateMonths(false);
                qmFilterModel.TO_YEARS_LIST = objCommonFunctions.PopulateYears(false).Where(a => a.Value != "0").ToList();
                qmFilterModel.ATR_STATUS_LIST = objCommonFunctions.QualityMaintenaceATRStatus();
                qmFilterModel.ROAD_STATUS_LIST = objCommonFunctions.PopulateRoadStatus();
                return View(qmFilterModel);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "QualityMonitoring.QualityMaintenanceATRFilters()");
                return null;
            }
        }


        /// <summary>
        /// View for ATR Details in  HTML Report
        /// </summary>//changed by deendayal on 05-02-2018
        /// <param name="formCollection"></param>
        /// <returns></returns>
        public ActionResult MaintenanceATRDetails(FormCollection formCollection)
        {
            QualityMonitoringBAL qmBAL = new QualityMonitoringBAL();
            QMATRDetailsModel atrDetailsModel = new QMATRDetailsModel();
            atrDetailsModel.ATR_LIST = new List<QMATRModel>();
            atrDetailsModel.OBS_LIST = new List<QMObsATRModel>();
            dbContext = new PMGSYEntities();
            try
            {
                atrDetailsModel.OBS_ATR_LIST = qmBAL.ATRDetailsBAL(Convert.ToInt32(formCollection["MAST_STATE_CODE"]), Convert.ToInt32(formCollection["ADMIN_QM_CODE"]),
                                                                Convert.ToInt32(formCollection["FROM_MONTH"]), Convert.ToInt32(formCollection["FROM_YEAR"]),
                                                                Convert.ToInt32(formCollection["TO_MONTH"]), Convert.ToInt32(formCollection["TO_YEAR"]),
                                                                formCollection["ATR_STATUS"], "M", 0);

                var distinctObsList = (from obs in atrDetailsModel.OBS_ATR_LIST
                                       select new
                                       {
                                           obs.QM_OBSERVATION_ID,
                                           obs.MONITOR_NAME,
                                           obs.STATE_NAME,
                                           obs.DISTRICT_NAME,
                                           obs.BLOCK_NAME,
                                           obs.IMS_PACKAGE_ID,
                                           obs.IMS_YEAR,
                                           obs.IMS_ROAD_NAME,
                                           obs.QM_INSPECTED_START_CHAINAGE,
                                           obs.QM_INSPECTED_END_CHAINAGE,
                                           obs.QM_INSPECTION_DATE,
                                           obs.IMS_ISCOMPLETED,
                                           obs.OVERALL_GRADE,
                                           obs.NO_OF_PHOTO_UPLOADED,
                                           obs.QM_ATR_STATUS,
                                           obs.PMGSY_SCHEME,
                                           obs.ADMIN_IS_ENQUIRY,
                                           obs.IMS_PROPOSAL_TYPE
                                       }).Distinct().ToList();

                foreach (var item in distinctObsList)
                {
                    QMObsATRModel obsModel = new QMObsATRModel();
                    obsModel.QM_OBSERVATION_ID = item.QM_OBSERVATION_ID;
                    obsModel.MONITOR_NAME = item.MONITOR_NAME;
                    obsModel.STATE_NAME = item.STATE_NAME;
                    obsModel.DISTRICT_NAME = item.DISTRICT_NAME;
                    obsModel.BLOCK_NAME = item.BLOCK_NAME;
                    obsModel.IMS_PACKAGE_ID = item.IMS_PACKAGE_ID;
                    obsModel.IMS_YEAR = item.IMS_YEAR.ToString() + "-" + (item.IMS_YEAR + 1).ToString();
                    obsModel.IMS_ROAD_NAME = item.IMS_ROAD_NAME;
                    obsModel.QM_INSPECTED_START_CHAINAGE = item.QM_INSPECTED_START_CHAINAGE;
                    obsModel.QM_INSPECTED_END_CHAINAGE = item.QM_INSPECTED_END_CHAINAGE;
                    obsModel.QM_INSPECTION_DATE = item.QM_INSPECTION_DATE;
                    obsModel.IMS_ISCOMPLETED = item.IMS_ISCOMPLETED;
                    obsModel.PMGSY_SCHEME = item.PMGSY_SCHEME;
                    obsModel.OVERALL_GRADE = item.OVERALL_GRADE;
                    obsModel.NO_OF_PHOTO_UPLOADED = item.NO_OF_PHOTO_UPLOADED;
                    obsModel.QM_ATR_STATUS = item.QM_ATR_STATUS;
                    obsModel.ADMIN_IS_ENQUIRY = item.ADMIN_IS_ENQUIRY.Equals("Y") ? "Yes" : "No";
                    obsModel.IMS_PROPOSAL_TYPE = item.IMS_PROPOSAL_TYPE.Equals("P") ? "Road" : "LSB";
                    obsModel.SHOW_OBS_LINK = "<a href='#' title='Click here to view details' class='ui-icon ui-icon-zoomin ui-align-center' onClick='ShowSQCATRObsDetails(\"" + item.QM_OBSERVATION_ID.ToString().Trim() + "\"); return false;'>View</a>";

                    atrDetailsModel.OBS_LIST.Add(obsModel);
                }

                //int index = 0;
                foreach (var item in atrDetailsModel.OBS_ATR_LIST)
                {
                    // Populate & Add ATR Details for each Observation Id
                    QMATRModel atrModel = new QMATRModel();
                    atrModel.QM_OBSERVATION_ID = item.QM_OBSERVATION_ID;
                    atrModel.QM_ATR_ID = item.QM_ATR_ID;
                    atrModel.ATR_ENTRY_DATE = item.ATR_ENTRY_DATE;
                    atrModel.ATR_REGRADE_STATUS = item.ATR_REGRADE_STATUS.Trim().Equals("N") ? "No" : "Yes";
                    atrModel.ATR_REGRADE_REMARKS = item.ATR_REGRADE_REMARKS;
                    atrModel.ATR_REGRADE_DATE = item.ATR_REGRADE_DATE;
                    atrModel.ATR_IS_DELETED = item.ATR_IS_DELETED;
                    atrModel.QM_ATR_STATUS = item.QM_ATR_STATUS;

                    atrModel.IS_SUBMITTED = item.QM_ATR_STATUS.Trim().Equals("N") ? "No" : "Yes";

                    atrModel.ATR_UPLOAD_VIEW_LINK =
                       (PMGSYSession.Current.RoleCode == 8 || PMGSYSession.Current.RoleCode == 5 || PMGSYSession.Current.RoleCode == 9)//for PIU view only
                                                                                                                                       //modified by abhinav pathak 0n 14-aug-2019

                       ?

                       ((item.QM_ATR_ID != null)
                                           ? "<a href='#' title='Click here to view uploaded ATR' class='ui-icon ui-icon-zoomin ui-align-center' onClick='DownloadMaintenanceATR(\"" + URLEncrypt.EncryptParameters(new string[] { item.QM_ATR_ID + ".pdf" + "$" + item.QM_ATR_ID }) + "\"); return false;'>Download</a>"


                                           : "-")  //"<a href='#' title='Click here to upload ATR' class='ui-icon ui-icon-plusthick ui-align-center' onClick='UploadSQCATR(\"" + item.QM_OBSERVATION_ID.ToString().Trim() + "\"); return false;'>Upload</a>")


                                               //: (dbContext.QUALITY_QM_INSPECTION_FILES.Where(obs => obs.QM_OBSERVATION_ID == item.QM_OBSERVATION_ID && obs.QM_FILES_FINALIZED == "Y").ToList().SequenceEqual(dbContext.QUALITY_QM_INSPECTION_FILES.Where(obj => obj.QM_OBSERVATION_ID == item.QM_OBSERVATION_ID).ToList()))

                                               //    ? "<a href='#' title='Click here to upload ATR' class='ui-icon ui-icon-plusthick ui-align-center' onClick='UploadSQCATR(\"" + item.QM_OBSERVATION_ID.ToString().Trim() + "\"); return false;'>Upload</a>"
                                               //        : "-"

                                               : (item.QM_ATR_STATUS.Trim().Equals("N")
                                               ? "<a href='#' title='Click here to upload ATR' class='ui-icon ui-icon-plusthick ui-align-center' onClick='UploadSQCATR(\"" + item.QM_OBSERVATION_ID.ToString().Trim() + "\"); return false;'>Upload</a>"
                                               : "<a href='#' title='Click here to view uploaded ATR' class='ui-icon ui-icon-zoomin ui-align-center' onClick='DownloadMaintenanceATR(\"" + URLEncrypt.EncryptParameters(new string[] { item.QM_ATR_ID + ".pdf" + "$" + item.QM_ATR_ID }) + "\"); return false;'>Download</a>");



                    //atrModel.ATR_UPLOAD_VIEW_LINK =
                    //    (PMGSYSession.Current.RoleCode == 8 || PMGSYSession.Current.RoleCode == 5 || PMGSYSession.Current.RoleCode == 9)//for PIU view only
                    //    //modified by abhinav pathak 0n 14-aug-2019

                    //    ?

                    //    (item.QM_ATR_ID != null)
                    //                        ? "<a href='#' title='Click here to view uploaded ATR' class='ui-icon ui-icon-zoomin ui-align-center' onClick='DownloadMaintenanceATR(\"" + URLEncrypt.EncryptParameters(new string[] { item.QM_ATR_ID + ".pdf" + "$" + item.QM_ATR_ID }) + "\"); return false;'>Download</a>"

                    //                            : (dbContext.QUALITY_QM_INSPECTION_FILES.Where(obs => obs.QM_OBSERVATION_ID == item.QM_OBSERVATION_ID && obs.QM_FILES_FINALIZED == "Y").ToList().SequenceEqual(dbContext.QUALITY_QM_INSPECTION_FILES.Where(obj => obj.QM_OBSERVATION_ID == item.QM_OBSERVATION_ID).ToList()))

                    //                                ? "<a href='#' title='Click here to upload ATR' class='ui-icon ui-icon-plusthick ui-align-center' onClick='UploadSQCATR(\"" + item.QM_OBSERVATION_ID.ToString().Trim() + "\"); return false;'>Upload</a>"
                    //                                    : "-"

                    //                       : (item.QM_ATR_STATUS.Trim().Equals("N")) // Upload/View
                    //                            ? (dbContext.QUALITY_QM_INSPECTION_FILES.Where(obs => obs.QM_OBSERVATION_ID == item.QM_OBSERVATION_ID && obs.QM_FILES_FINALIZED == "Y").ToList().SequenceEqual(dbContext.QUALITY_QM_INSPECTION_FILES.Where(obj => obj.QM_OBSERVATION_ID == item.QM_OBSERVATION_ID).ToList()))
                    //                                ? "<a href='#' title='Click here to upload ATR' class='ui-icon ui-icon-plusthick ui-align-center' onClick='UploadSQCATR(\"" + item.QM_OBSERVATION_ID.ToString().Trim() + "\"); return false;'>Upload</a>" : "-"
                    //                                    : (item.QM_ATR_ID != null)
                    //                                        ? "<a href='#' title='Click here to view uploaded ATR' class='ui-icon ui-icon-zoomin ui-align-center' onClick='DownloadMaintenanceATR(\"" + URLEncrypt.EncryptParameters(new string[] { item.QM_ATR_ID + ".pdf" + "$" + item.QM_ATR_ID }) + "\"); return false;'>Download</a>"
                    //                       : "<a href='#' class='ui-icon ui-icon-locked ui-align-center' />";

                    atrModel.ATR_ACCEPTANCE_LINK =
                         (PMGSYSession.Current.RoleCode == 8 || PMGSYSession.Current.RoleCode == 5 || PMGSYSession.Current.RoleCode == 9)//for PIU view only
                         ? item.ATR_REGRADE_STATUS.Trim().Equals("A")
                                        ? "Accepted"
                                        : item.ATR_REGRADE_STATUS.Trim().Equals("R")
                                            ? "Rejected"
                                            : item.ATR_REGRADE_STATUS.Equals("V")
                                                ? "To be Verified"
                                                : item.ATR_REGRADE_STATUS.Equals("D")
                                                    ? "Non Rectifible Deffect"
                                                    : ""
                         : item.ATR_REGRADE_STATUS.Trim().Equals("A")     // Acceptance
                                        ? "Accepted"
                                        : item.ATR_REGRADE_STATUS.Equals("V")
                                            ? "To be Verified"
                                            : item.ATR_REGRADE_STATUS.Equals("D")
                                                ? "Non Rectifiable Deffect"
                                                //: (item.ATR_REGRADE_STATUS.Equals("C") && PMGSYSession.Current.RoleCode == 5) // Changes done on 13-04-2015 as per given ECR by Aanad. 
                                                : (item.ATR_REGRADE_STATUS.Equals("C") && (PMGSYSession.Current.RoleCode == 22 || PMGSYSession.Current.RoleCode == 38 || PMGSYSession.Current.RoleCode == 54))//PIURCPLWE
                                                    ? "Technical Committee" + // If Tech Committee, then append + sign to upload again
                                                              (atrModel.QM_ATR_ID == (dbContext.QUALITY_ATR_FILE.Where(c => c.QM_OBSERVATION_ID == atrModel.QM_OBSERVATION_ID).Select(c => c.QM_ATR_ID).Max())
                                                                 ? "<a href='#' title='Click here to upload ATR' class='ui-icon ui-icon-plusthick ui-align-center' onClick='UploadSQCATR(\"" + item.QM_OBSERVATION_ID.ToString().Trim() + "\"); return false;'>Upload</a>"
                                                                 : "")
                                                    : item.ATR_REGRADE_STATUS.Trim().Equals("R") // if any of the ATR against Obs Id is Accepted then dont provide link to upload.
                                                        ? item.QM_ATR_STATUS.Equals("A")
                                                            ? "Rejected"
                                                            : "Rejected" + //If Rejected atr is last against Observation, then append + sign to upload again
                                                              (atrModel.QM_ATR_ID == (dbContext.QUALITY_ATR_FILE.Where(c => c.QM_OBSERVATION_ID == atrModel.QM_OBSERVATION_ID).Select(c => c.QM_ATR_ID).Max())
                                                                 ? "<a href='#' title='Click here to upload ATR' class='ui-icon ui-icon-plusthick ui-align-center' onClick='UploadSQCATR(\"" + item.QM_OBSERVATION_ID.ToString().Trim() + "\"); return false;'>Upload</a>"
                                                                 : "")
                                                        : "";

                    atrModel.ATR_REGRADE_LINK =
                        (PMGSYSession.Current.RoleCode == 22 || PMGSYSession.Current.RoleCode == 38 || PMGSYSession.Current.RoleCode == 5 || PMGSYSession.Current.RoleCode == 9 || PMGSYSession.Current.RoleCode == 54)//for PIU or PIURCPLWE view only
                        ? (item.ATR_REGRADE_STATUS.Trim().Equals("A"))
                            ? "<a href='#' title='Click here to see observation details against ATR' class='ui-icon ui-icon-zoomin ui-align-center' onClick='ShowSQCATRGradingDetails(\"" + item.QM_OBSERVATION_ID.ToString().Trim() + "\"); return false;'>Regrade</a>"
                            : "<a href='#' class='ui-icon ui-icon-locked ui-align-center' />"
                        : ((item.ATR_REGRADE_STATUS.Trim().Equals("U") || item.ATR_REGRADE_STATUS.Trim().Equals("V")) && item.ATR_IS_DELETED.Equals("N")) // Regrade, for recent entry only
                            ? "<a href='#' title='Click here to regrade ATR' class='ui-icon ui-icon-plusthick ui-align-center' onClick='RegradeSQCATR(\"" + item.QM_OBSERVATION_ID.ToString().Trim() + "\"); return false;'>Regrade</a>"
                            : (item.ATR_REGRADE_STATUS.Trim().Equals("A"))
                                ? "<a href='#' title='Click here to see observation details against ATR' class='ui-icon ui-icon-zoomin ui-align-center' onClick='ShowSQCATRGradingDetails(\"" + item.QM_OBSERVATION_ID.ToString().Trim() + "\"); return false;'>Regrade</a>"
                                : "<a href='#' class='ui-icon ui-icon-locked ui-align-center' />";

                    atrModel.ATR_DELETE_LINK =
                         (PMGSYSession.Current.RoleCode == 22 || PMGSYSession.Current.RoleCode == 38 || PMGSYSession.Current.RoleCode == 5 || PMGSYSession.Current.RoleCode == 9 || PMGSYSession.Current.RoleCode == 54)//for PIU or PIURCPLWE view only
                         ? "<a href='#' class='ui-icon ui-icon-locked ui-align-center' />"
                         : (item.QM_ATR_ID != null && item.QM_ATR_STATUS.Trim() != "A" && item.QM_ATR_STATUS.Trim() != "N" && (item.QM_ATR_ID == dbContext.QUALITY_ATR_FILE.Where(c => c.QM_OBSERVATION_ID == item.QM_OBSERVATION_ID).Select(c => c.QM_ATR_ID).Max()))
                                ? "<a href='#' title='Click here to delete ATR details' class='ui-icon ui-icon-trash ui-align-center' onClick='DeleteSQCATR(\"" +
                                    item.QM_OBSERVATION_ID.ToString().Trim() + "\",\"" + dbContext.QUALITY_ATR_FILE.Where(a => a.QM_OBSERVATION_ID == item.QM_OBSERVATION_ID && a.ATR_IS_DELETED == "N").Select(a => a.QM_ATR_ID).FirstOrDefault()
                                    + "\"); return false;'>Delete</a>"
                                : "<a href='#' class='ui-icon ui-icon-locked ui-align-center' />";

                    atrDetailsModel.ATR_LIST.Add(atrModel);

                    //index++; // increment index for each record.
                }

                return View(atrDetailsModel);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "QualityMonitoring.MaintenanceATRDetails(FormCollection formCollection)");
                atrDetailsModel.ERROR = "Error occurred while processing your request";
                return View(atrDetailsModel); //return model as null
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        /// <summary>
        /// DownLoad Maintenance ATR Pdf File 
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="hash"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        [HttpGet]
        [Audit]
        public ActionResult DownloadMaintenanceATRFile(String parameter, String hash, String key)
        {
            string FileName = string.Empty;
            string FullFileLogicalPathName = string.Empty;
            string FullfilePhysicalPathName = string.Empty;
            string FullFileLogicalPathId = string.Empty;
            string FullfilePhysicalPathId = string.Empty;
            string FileExtension = string.Empty;
            Int32 ObsOrAtrId = 0;
            dbContext = new PMGSYEntities();
            try
            {
                if (!String.IsNullOrEmpty(parameter) && !String.IsNullOrEmpty(hash) && !String.IsNullOrEmpty(key))
                {
                    String[] urlParams = URLEncrypt.DecryptParameters(new String[] { parameter, hash, key });
                    if (urlParams.Length >= 1)
                    {
                        String[] urlSplitParams = urlParams[0].Split('$');
                        FileName = (urlSplitParams[0]);
                        ObsOrAtrId = Convert.ToInt32(urlSplitParams[1]);
                    }
                }
                FileExtension = Path.GetExtension(FileName).ToLower();

                //In case of if File With Name not Found then find with Id, This is case particularly for ATR
                FullFileLogicalPathId = Path.Combine(ConfigurationManager.AppSettings["QUALITY_INSPECTION_FILE_UPLOAD_MAINTENANCE_ATR_VIRTUAL_DIR_PATH"], ObsOrAtrId.ToString() + ".pdf");
                FullfilePhysicalPathId = Path.Combine(ConfigurationManager.AppSettings["QUALITY_INSPECTION_FILE_UPLOAD_MAINTENANCE_ATR"], ObsOrAtrId.ToString() + ".pdf");

                FullFileLogicalPathName = Path.Combine(ConfigurationManager.AppSettings["QUALITY_INSPECTION_FILE_UPLOAD_MAINTENANCE_ATR_VIRTUAL_DIR_PATH"], FileName);
                FullfilePhysicalPathName = Path.Combine(ConfigurationManager.AppSettings["QUALITY_INSPECTION_FILE_UPLOAD_MAINTENANCE_ATR"], FileName);

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

                if (System.IO.File.Exists(FullfilePhysicalPathName))
                {
                    return File(FullfilePhysicalPathName, type, DateTime.Now.ToShortDateString() + "_" + new Random().Next(1000000000) + FileExtension);
                }
                else if (System.IO.File.Exists(FullfilePhysicalPathId))
                {
                    return File(FullfilePhysicalPathId, type, DateTime.Now.ToShortDateString() + "_" + new Random().Next(1000000000) + FileExtension);
                }
                else
                {
                    return Json(new { Error = "File Not Exist" }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                //Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                ErrorLog.LogError(ex, "QualityMonitoring.DownloadMaintenanceATRFile()");
                return Json(new { Success = "false" }, JsonRequestBehavior.AllowGet);
            }
            finally
            {
                dbContext.Dispose();
            }
        }
        #endregion


        /// Added by deendayal on 12-02-2018 to assign contractors          
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// []
        [Audit]
        public ActionResult QMAssignContractors(string id)
        {
            qualityBAL = new QualityMonitoringBAL();
            CommonFunctions objCommonFunctions = new CommonFunctions();
            try
            {
                QMAssignRoadsModel qmAssignRoads = new QMAssignRoadsModel();
                qmAssignRoads.ROAD_STATUS = "A";
                qmAssignRoads.DISTRICTS = qualityBAL.GetScheduledDistrictListBAL(Convert.ToInt32(id), true);
                if (PMGSYSession.Current.RoleCode == 22 || PMGSYSession.Current.RoleCode == 38)        //For PIU
                    qmAssignRoads.YEARS = objCommonFunctions.PopulateFinancialYear(false, false).Where(a => a.Value != "0").ToList();
                else
                    qmAssignRoads.YEARS = new SelectList(new List<SelectListItem>().AsEnumerable<SelectListItem>(), "Value", "Text").ToList();  //objCommonFunctions.PopulateYears(true);


                qmAssignRoads.YEARS = objCommonFunctions.PopulateFinancialYear(false, false).Where(a => Convert.ToInt32(a.Value) > 2012 && Convert.ToInt32(a.Value) < 2018).ToList();
                qmAssignRoads.ROAD_STATUS_LIST = objCommonFunctions.PopulateRoadStatus();
                qmAssignRoads.ADMIN_SCHEDULE_CODE = Convert.ToInt32(id);

                return View(qmAssignRoads);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "QualityMonitoring.QMAssignContractors(String id)");
                return null;
            }
        }

        /// Listing of Roads in selcted Districts & Sanction Year to be assigned contractor wise added by deendayal
        /// </summary>
        /// <param name="formCollection"></param>
        /// <param name="districtCode"></param>
        /// <param name="sanctionYear"></param>
        /// <param name="rdStatus"></param>
        /// <param name="adminSchCode"></param>
        /// <returns></returns>
        [HttpPost]
        [Audit]
        public ActionResult GetRoadListToAssignContractorWise(FormCollection formCollection)
        {
            qualityBAL = new QualityMonitoringBAL();
            int totalRecords;
            try
            {
                //Adde By Abhishek kamble 29-Apr-2014 start
                using (CommonFunctions commonFunction = new CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new GridParams(Convert.ToInt32(formCollection["page"]), Convert.ToInt32(formCollection["rows"]), formCollection["sidx"], formCollection["sord"], Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                    {
                        return null;
                    }
                }
                //Adde By Abhishek kamble 29-Apr-2014 end
                int distCode = Convert.ToInt32(formCollection["districtCode"]);
                var jsonData = new
                {
                    rows = qualityBAL.GetRoadListToAssignContractorwiseBAL(Convert.ToInt32(formCollection["page"]) - 1, Convert.ToInt32(formCollection["rows"]),
                                                            formCollection["sidx"], formCollection["sord"], out totalRecords, formCollection["filters"],
                                                            Convert.ToInt32(formCollection["districtCode"]), Convert.ToInt32(formCollection["adminSchCode"]), Convert.ToInt32(formCollection["sanctionYear"])),
                    total = totalRecords <= Convert.ToInt32(formCollection["rows"]) ? 1 : (totalRecords % Convert.ToInt32(formCollection["rows"]) == 0 ? totalRecords / Convert.ToInt32(formCollection["rows"]) : (totalRecords / Convert.ToInt32(formCollection["rows"]) + 1)),
                    page = Convert.ToInt32(formCollection["page"]),
                    records = totalRecords
                };
                return Json(jsonData);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "QualitymonitoringController.GetRoadListToAssignContractorWise()");
                return Json(String.Empty);
            }
        }

        #region Auto Schedule
        [HttpGet]
        public ActionResult AllocateDistrictsToNQMLayout()
        {
            AllocateDistrictsToNQMViewModel model = new AllocateDistrictsToNQMViewModel();
            CommonFunctions comm = new CommonFunctions();
            try
            {
                model.lstState = comm.PopulateStates(false);
                model.MonthList = comm.PopulateMonths(false);
                model.YearList = comm.PopulateYears(false);
                //model.YearList.Insert(0, new SelectListItem { Text = (DateTime.Now.Year + 1).ToString(), Value = (DateTime.Now.Year + 1).ToString() });
                if (System.DateTime.Now.Month == 12)
                {
                    model.YearList.Insert(0, (new SelectListItem { Text = (System.DateTime.Now.Year + 1).ToString(), Value = (System.DateTime.Now.Year + 1).ToString() }));
                }



                return View(model);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "QualityMonitoring.AllocateDistrictsToNQMLayout()");
                return null;
            }
        }

        [HttpGet]
        public ActionResult QMGetInspectionTargetList(int? page, int? rows, string sidx, string sord)
        {
            int stateCode = 0, month = 0, year = 0;
            try
            {
                PMGSY.DAL.QualityMonitoring.QualityMonitoringDAL objDAL = new PMGSY.DAL.QualityMonitoring.QualityMonitoringDAL();

                //Adde By Abhishek kamble 29-Apr-2014 start
                using (CommonFunctions commonFunction = new CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new GridParams(page, rows, sidx, sord, Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                    {
                        return null;
                    }
                }

                stateCode = Convert.ToInt32(Request.Params["stateCode"]);
                month = Convert.ToInt32(Request.Params["month"]);
                year = Convert.ToInt32(Request.Params["year"]);
                long totalRecords;

                var jsonData = new
                {
                    rows = objDAL.QMGetInspectionTargetListDAL(stateCode, month, year, page - 1, rows, sidx, sord, out totalRecords),
                    total = (totalRecords <= rows ? 1 : totalRecords / rows + 1),
                    page = page,
                    records = totalRecords,
                };
                return Json(jsonData, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "QualityMonitoring.QMGetInspectionTargetList()");
                return null;
            }
        }

        [HttpPost]
        [Audit]
        public ActionResult QMAssignTargetDistricts(String parameter, String hash, String key)
        {
            PMGSY.DAL.QualityMonitoring.QualityMonitoringDAL qualityDAL = new PMGSY.DAL.QualityMonitoring.QualityMonitoringDAL();
            int targetId = 0;

            try
            {
                if (!String.IsNullOrEmpty(parameter) && !String.IsNullOrEmpty(hash) && !String.IsNullOrEmpty(key))
                {
                    String[] urlParams = URLEncrypt.DecryptParameters(new String[] { parameter, hash, key });
                    if (urlParams.Length >= 1)
                    {
                        String[] urlSplitParams = urlParams[0].Split('$');

                        targetId = Convert.ToInt32(urlParams[0]);
                    }
                }
                dbContext = new Models.PMGSYEntities();
                //string DisAllocationCheck = dbContext.QUALITY_QM_AUTO_SCHEDULE.Where(m=>m.MAST_STATE_CODE && m.SCHEDULE_YEAR)
                string Status = qualityDAL.QMAssignDistrictsDAL(targetId);
                if (Status == string.Empty)
                    return Json(new { Success = true, message = "Districts assigned successfully" });
                else
                    return Json(new { Success = false, message = "Districts are not available for Assignment as per Autoschedule Requirement" });

            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "QualityMonitoringDAL.QMAssignTargetDistricts()");
                return Json(new { Success = false, ErrorMessage = "An Error Occurred While Processing Your Request." });
            }
        }


        #endregion

        #region Allocate Roads to NQM
        public ActionResult AllocateRoadsToNQMLayout()
        {
            AllocateRoadsToNQMModel model = null;
            CommonFunctions common = null;
            try
            {
                model = new AllocateRoadsToNQMModel();
                common = new CommonFunctions();
                PMGSYEntities dbContext = new PMGSYEntities();

                List<SelectListItem> lstYears = new SelectList(dbContext.MASTER_YEAR.Where(m => m.MAST_YEAR_CODE < (DateTime.Now.Year + 2)).OrderByDescending(m => m.MAST_YEAR_CODE), "MAST_YEAR_CODE", "MAST_YEAR_CODE").ToList();

                lstYears.Insert(0, (new SelectListItem { Text = "Select Year", Value = "0", Selected = true }));


                model.STATE_LIST = common.PopulateStates(true);
                model.MONTH_LIST = common.PopulateMonths();
                //model.YEAR_LIST = lstYears;
                model.YEAR_LIST = common.PopulateYears(true);
                if (System.DateTime.Now.Month == 12)
                {
                    model.YEAR_LIST.Insert(1, (new SelectListItem { Text = (System.DateTime.Now.Year + 1).ToString(), Value = (System.DateTime.Now.Year + 1).ToString() }));
                }

            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "QualityMonitoringController.AllocateRoadsToNQMLayout()");
                return null;
            }

            return View(model);
        }



        /// <summary>
        /// On View Details Click
        /// </summary>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <param name="sidx"></param>
        /// <param name="sord"></param>
        /// <returns></returns>
        [HttpPost]
        [Audit]
        public ActionResult AllocateRoadsToNQmList(int? page, int? rows, string sidx, string sord)
        {
            AllocateRoadsToNQMModel objFilter = null;

            try
            {
                objFilter = new AllocateRoadsToNQMModel();
                qualityBAL = new QualityMonitoringBAL();



                long totalRecords;
                string loadGrid = Convert.ToString(Request.Params["Load"]);


                //if (loadGrid == "Load")
                //{

                //}
                objFilter.STATEID = Convert.ToInt32(Request.Params["state"]);
                objFilter.MONTHID = Convert.ToInt32(Request.Params["month"]);
                objFilter.YEARID = Convert.ToInt32(Request.Params["year"]);




                var jsonData = new
                {
                    rows = qualityBAL.AllocateRoadsToNQmList(objFilter, out totalRecords),
                    //total = totalRecords <= objFilter.rows ? 1 : totalRecords / objFilter.rows + 1,
                    page = objFilter.page + 1,
                    records = totalRecords
                };

                return Json(jsonData);




            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "QualityMonitoringController.AllocateRoadsToNQmList()");
                return null;
            }


        }

        /// <summary>
        /// This Methods Saves EO Approval/Rejected 
        /// </summary>
        /// <param name="ValueToAssign"></param>
        /// <returns></returns>
        [HttpPost]
        [Audit]
        [ValidateAntiForgeryToken]
        public ActionResult AssignRoadToNQM(String parameter, String hash, String key)
        {


            string StateID = String.Empty;
            string DistrictCode1 = String.Empty;
            string DistrictCode2 = String.Empty;
            Int32 AUTO_SCHEDULED_ID = 0;
            QUALITY_QM_AUTO_SCHEDULE obj = null;
            PMGSYEntities dbcontext = null;
            try
            {
                dbcontext = new PMGSYEntities();
                obj = new QUALITY_QM_AUTO_SCHEDULE();
                if (!String.IsNullOrEmpty(parameter) && !String.IsNullOrEmpty(hash) && !String.IsNullOrEmpty(key))
                {
                    String[] urlParams = URLEncrypt.DecryptParameters(new String[] { parameter, hash, key });
                    if (urlParams.Length >= 1)
                    {
                        String[] urlSplitParams = urlParams[0].Split('$');
                        AUTO_SCHEDULED_ID = Convert.ToInt32(urlSplitParams[0]);
                    }
                    obj = dbcontext.QUALITY_QM_AUTO_SCHEDULE.Where(x => x.AUTO_SCHEDULE_ID == AUTO_SCHEDULED_ID).FirstOrDefault();
                    Int32? DISTRICT1 = obj.DISTRICT1;
                    Int32? DISTRICT2 = obj.DISTRICT2;

                    if (dbcontext.QM_AUTO_SCHEDULE_WORK_MAPPING.Where(x => x.AUTO_SCHEDULE_ID == AUTO_SCHEDULED_ID).Any())
                    {
                        return Json(new { success = false, ErrMessage = "Road Already Assigned to NQM" });

                    }
                    else
                    {
                        int Result = dbcontext.QM_GET_WORK_AUTO_SCHEDULE(obj.MAST_STATE_CODE, DISTRICT1, DISTRICT2, AUTO_SCHEDULED_ID);
                        if (Result == 0)
                        {
                            return Json(new { success = false, ErrMessage = "Error in Assiging Road to NQM,Please try Again" });

                        }
                        else
                        {
                            return Json(new { success = true });

                        }
                    }

                }
                else
                {

                    return Json(new { success = false, ErrMessage = "Error in Assiging Road to NQM,Please try Again" });
                }




            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "QualityMonitoringController.AssignRoadToNQM()");
                return null;
            }


        }



        #endregion

        #region NQM Target Inspection
        [HttpGet]
        public ActionResult InspectionTargetLayout()
        {
            InspectionTargetModel model = new InspectionTargetModel();
            CommonFunctions comm = new CommonFunctions();
            PMGSY.DAL.QualityMonitoring.QualityMonitoringDAL objDAL = new PMGSY.DAL.QualityMonitoring.QualityMonitoringDAL();

            try
            {
                model.MONTH_LIST = comm.PopulateMonths(true);
                model.YEAR_LIST = comm.PopulateYears(true);


                if (System.DateTime.Now.Month == 12)
                {
                    model.YEAR_LIST.Insert(1, (new SelectListItem { Text = (System.DateTime.Now.Year + 1).ToString(), Value = (System.DateTime.Now.Year + 1).ToString() }));
                }

                //model.flg = objDAL.checkIsTargetEntered();

                return View(model);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "QualityMonitoring.InspectionTargetLayout()");
                return null;
            }
        }

        [HttpGet]
        public ActionResult CheckTargetEntered()
        {
            InspectionTargetModel model = new InspectionTargetModel();
            PMGSY.DAL.QualityMonitoring.QualityMonitoringDAL objDAL = new PMGSY.DAL.QualityMonitoring.QualityMonitoringDAL();
            int Month = 0, Year = 0;
            try
            {
                Month = Convert.ToInt32(Request.Params["Month"]);
                Year = Convert.ToInt32(Request.Params["Year"]);

                if (Month == 0 || Year == 0)
                {
                    return null;
                }

                model.flg = objDAL.checkIsTargetEntered(Month, Year);

                return Json(new { success = model.flg }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "QualityMonitoring.CheckTargetEntered()");
                return null;
            }
        }

        [HttpPost]
        public ActionResult ListInspectionTarget(FormCollection formCollection)
        {
            if (PMGSYSession.Current == null)
            {
                Response.Redirect("/Login/Login");
            }
            int month = Convert.ToInt32(Request.Params["month"]);
            int year = Convert.ToInt32(Request.Params["year"]);
            long totalRecords = 0;
            PMGSY.DAL.QualityMonitoring.QualityMonitoringDAL objDAL = new PMGSY.DAL.QualityMonitoring.QualityMonitoringDAL();

            //objProposalBAL
            try
            {
                using (CommonFunctions commonFunction = new CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new GridParams(Convert.ToInt32(formCollection["page"]), Convert.ToInt32(formCollection["rows"]), formCollection["sidx"], formCollection["sord"], Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                    {
                        return null;
                    }
                }

                var jsonData = new
                {
                    rows = objDAL.ListInspectionTargetDAL(month, year, Convert.ToInt32(formCollection["page"]) - 1, Convert.ToInt32(formCollection["rows"]), formCollection["sidx"], formCollection["sord"], out totalRecords),
                    total = totalRecords <= Convert.ToInt32(formCollection["rows"]) ? 1 : (totalRecords % Convert.ToInt32(formCollection["rows"]) == 0 ? totalRecords / Convert.ToInt32(formCollection["rows"]) : totalRecords / Convert.ToInt32(formCollection["rows"]) + 1),//totalRecords <= Convert.ToInt32(rows) ? 1 : totalRecords / Convert.ToInt32(rows) + 1,
                    page = Convert.ToInt32(formCollection["page"]),
                    records = totalRecords
                };
                return Json(jsonData, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                //Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                ErrorLog.LogError(ex, "QualityMonitoring.ListInspectionTarget()");
                return null;
            }
        }

        [HttpPost]
        public ActionResult ListInspectionTargetEntered(FormCollection formCollection)
        {
            if (PMGSYSession.Current == null)
            {
                Response.Redirect("/Login/Login");
            }
            int month = Convert.ToInt32(Request.Params["month"]);
            int year = Convert.ToInt32(Request.Params["year"]);
            long totalRecords = 0;
            PMGSY.DAL.QualityMonitoring.QualityMonitoringDAL objDAL = new PMGSY.DAL.QualityMonitoring.QualityMonitoringDAL();

            //objProposalBAL
            try
            {
                using (CommonFunctions commonFunction = new CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new GridParams(Convert.ToInt32(formCollection["page"]), Convert.ToInt32(formCollection["rows"]), formCollection["sidx"], formCollection["sord"], Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                    {
                        return null;
                    }
                }

                var jsonData = new
                {
                    rows = objDAL.ListInspectionTargetEnteredDAL(month, year, Convert.ToInt32(formCollection["page"]) - 1, Convert.ToInt32(formCollection["rows"]), formCollection["sidx"], formCollection["sord"], out totalRecords),
                    total = totalRecords <= Convert.ToInt32(formCollection["rows"]) ? 1 : (totalRecords % Convert.ToInt32(formCollection["rows"]) == 0 ? totalRecords / Convert.ToInt32(formCollection["rows"]) : totalRecords / Convert.ToInt32(formCollection["rows"]) + 1),//totalRecords <= Convert.ToInt32(rows) ? 1 : totalRecords / Convert.ToInt32(rows) + 1,
                    page = Convert.ToInt32(formCollection["page"]),
                    records = totalRecords
                };
                return Json(jsonData, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                //Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                ErrorLog.LogError(ex, "QualityMonitoring.ListInspectionTargetEntered()");
                return null;
            }
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        [Audit]
        public ActionResult AddInspTargetDetails(string[] MatrixParams)
        {
            if (PMGSYSession.Current == null)
            {
                Response.Redirect("/Login/Login");
            }

            decimal parentValue = 0, childValue = 0;
            string prevClass = string.Empty;
            string[] item = null;
            bool flag = false;
            string message = string.Empty;
            string prevGrowthType = string.Empty;

            PMGSY.DAL.QualityMonitoring.QualityMonitoringDAL objDAL = new PMGSY.DAL.QualityMonitoring.QualityMonitoringDAL();

            try
            {


                if (MatrixParams != null)
                {
                    flag = true;
                }
                if (flag)
                {
                    bool status = objDAL.AddTargetDetailsDAL(MatrixParams, ref message);
                    if (status == true)
                    {
                        return Json(new { success = true, message = "Target Details added successfully" });
                    }
                    else
                    {
                        return Json(new { success = false, message = "Target Details could not be added" });
                    }
                }
                else
                {
                    return Json(new { success = false, message = "Invalid Target Details" });
                }
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "QualityMonitoring.AddMatrixMasterDetails()");
                if (ex.Message == "Input string was not in a correct format.")
                {
                    return Json(new { success = false, message = "Please enter only numbers" });
                }
                else
                {
                    return Json(new { success = false, message = "Error occured while adding Matrix Details" });
                }
            }
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        [Audit]
        public ActionResult DeleteInspTargetDetails()
        {
            if (PMGSYSession.Current == null)
            {
                Response.Redirect("/Login/Login");
            }

            PMGSY.DAL.QualityMonitoring.QualityMonitoringDAL objDAL = new PMGSY.DAL.QualityMonitoring.QualityMonitoringDAL();
            int month = Convert.ToInt32(Request.Params["month"]);
            int year = Convert.ToInt32(Request.Params["year"]);

            string message = string.Empty;
            try
            {

                bool status = objDAL.DeleteInspTargetDetailsDAL(month, year, ref message);
                if (status == true)
                {
                    return Json(new { success = true, message = "Target Details added successfully" });
                }
                else
                {
                    return Json(new { success = false, message = "Target Details could not be added" });
                }
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "QualityMonitoring.AddMatrixMasterDetails()");
                if (ex.Message == "Input string was not in a correct format.")
                {
                    return Json(new { success = false, message = "Please enter only numbers" });
                }
                else
                {
                    return Json(new { success = false, message = "Error occured while adding Matrix Details" });
                }
            }
        }

        [HttpGet]
        public ActionResult NQMNAForInspectionLayout()
        {
            try
            {
                return View();
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "QualityMonitoring.NQMNAForInspectionLayout()");
                return null;
            }
        }

        [HttpGet]
        public ActionResult AddNQMNAForInspectionLayout()
        {
            NQMNotAvailableInspectionViewModel model = new NQMNotAvailableInspectionViewModel();
            CommonFunctions comm = new CommonFunctions();
            PMGSY.DAL.QualityMonitoring.QualityMonitoringDAL objDAL = new PMGSY.DAL.QualityMonitoring.QualityMonitoringDAL();
            PMGSYEntities dbContext = new PMGSYEntities();

            List<SelectListItem> monthslist1 = new List<SelectListItem>();
            List<SelectListItem> monthslist2 = new List<SelectListItem>();
            List<SelectListItem> yearlist1 = new List<SelectListItem>();
            List<SelectListItem> yearlist2 = new List<SelectListItem>();
            List<SelectListItem> itemList = new List<SelectListItem>();

            //Changes by Shreyas on 28-07-2022

            try
            {
                SelectListItem item = new SelectListItem();
                monthslist1 = comm.PopulateMonths(true);
                if (PMGSYSession.Current.RoleCode == 5 || PMGSYSession.Current.RoleCode == 9)
                {
                    monthslist2.Add(monthslist1[DateTime.Now.Month]);

                }
                //monthslist2.Add(monthslist1[DateTime.Now.Month]);
                monthslist2.Add(DateTime.Now.Month != 12 ? monthslist1[DateTime.Now.Month + 1] : monthslist1[1]);
                //monthslist2.Add(DateTime.Now.Month != 12 ? monthslist1[DateTime.Now.Month + 2] : monthslist1[2]);
                monthslist2.Add(DateTime.Now.Month != 12 ? (DateTime.Now.Month != 11 ? monthslist1[DateTime.Now.Month + 2] : monthslist1[1]) : monthslist1[2]);  //changes on 01-11-2022
                model.lstMonth = monthslist2;
                //yearlist1= comm.PopulateYears(true);
                //yearlist2.Add(DateTime.Now.Year);
                yearlist2.Add(new SelectListItem { Text = DateTime.Now.Year.ToString(), Value = DateTime.Now.Year.ToString() });
                if (DateTime.Now.Month == 12)
                {
                    yearlist2.Add(new SelectListItem { Text = (DateTime.Now.Year + 1).ToString(), Value = (DateTime.Now.Year + 1).ToString() });
                }
                model.lstYear = yearlist2;
                //model.lstMonth = comm.PopulateMonths(true);
                //model.lstYear = comm.PopulateYears(true);
                if (PMGSYSession.Current.RoleCode == 6)
                {
                    var a = PMGSYSession.Current.UserId;

                    var query = (from aqm in dbContext.ADMIN_QUALITY_MONITORS
                                 where aqm.ADMIN_USER_ID == PMGSYSession.Current.UserId
                                 && aqm.ADMIN_QM_EMPANELLED == "Y"

                                 select new
                                 {
                                     Value = aqm.ADMIN_QM_CODE,
                                     Text = (aqm.ADMIN_QM_LNAME.Equals(null) ? "" : aqm.ADMIN_QM_LNAME) + " " + (aqm.ADMIN_QM_FNAME.Equals(null) ? "" : aqm.ADMIN_QM_FNAME) + " " + (aqm.ADMIN_QM_MNAME.Equals(null) ? "" : aqm.ADMIN_QM_MNAME),
                                     //+ " (" + aqm.ADMIN_QM_SERVICE_TYPE == "C" ? "Cental Govt." : aqm.ADMIN_QM_SERVICE_TYPE == "A" ? "Central Agency" : aqm.MAST_STATE_CODE + ")",
                                     aqm.ADMIN_QM_SERVICE_TYPE,
                                     aqm.MASTER_STATE.MAST_STATE_NAME
                                 }).FirstOrDefault();

                    item = new SelectListItem();
                    item.Text = query.Text.Trim();
                    item.Value = query.Value.ToString();
                    itemList.Add(item);
                    ViewBag.UserData = item;
                    model.lstAdminQmCode = itemList;
                }
                else
                {
                    model.lstAdminQmCode = comm.PopulateNQMForScheduleCreation(String.Empty, "I", 0, DateTime.Now.Month, DateTime.Now.Year).OrderBy(x => x.Text).ToList();
                }


                //model.lstAdminQmCode.OrderBy(x => x.Text);

                model.Month = DateTime.Now.Month;
                model.Year = DateTime.Now.Year;

                return View(model);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "QualityMonitoring.AddNQMNAForInspectionLayout()");
                return null;
            }
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult AddNQMNAForInspection(NQMNotAvailableInspectionViewModel model)
        {
            //NQMNotAvailableInspectionViewModel model = new NQMNotAvailableInspectionViewModel();
            CommonFunctions comm = new CommonFunctions();
            PMGSY.DAL.QualityMonitoring.QualityMonitoringDAL objDAL = new PMGSY.DAL.QualityMonitoring.QualityMonitoringDAL();
            string message = string.Empty;
            bool status = false;
            try
            {
                //Changes by Shreyas on 28-07-2022
                if (PMGSYSession.Current.RoleCode == 5 || PMGSYSession.Current.RoleCode == 9)
                {

                    if (string.IsNullOrEmpty(model.ASSIGNED_lstAdminQmCode))
                    {
                        return Json(new { success = false, message = "Please select at least one Monitor" }, JsonRequestBehavior.AllowGet);
                    }
                    if (ModelState.IsValid)
                    {

                        status = objDAL.AddNQMNAInspectionDAL(model, ref message);
                        if (status)
                        {
                            //return Json(new { success = status, message = "NQM details added successfully!!" }, JsonRequestBehavior.AllowGet);
                            return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
                        }
                        else
                        {
                            return Json(new { success = false, message = "NQM details deleted." }, JsonRequestBehavior.AllowGet);
                        }
                    }
                    else
                    {
                        // return Json(new { success = false, message = "Error occurred " }, JsonRequestBehavior.AllowGet);
                        return Json(new { Success = false, ErrorMessage = HttpUtility.HtmlEncode(new CommonFunctions().FormatErrorMessage(ModelState)) });
                    }
                    return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    if ((DateTime.Now.Month + 1 == model.Month && DateTime.Now.Day < 25 && DateTime.Now.Year == model.Year) || (DateTime.Now.Month + 1 < model.Month && DateTime.Now.Year == model.Year) || (DateTime.Now.Month == 12 && 1 == model.Month && DateTime.Now.Day < 25 && DateTime.Now.Year + 1 == model.Year) || (DateTime.Now.Month == 12 && 1 < model.Month && DateTime.Now.Year + 1 == model.Year))  //Shreyas--Edited on 21-07-2022 to allow to add till 25 of prev month
                    {


                        if (string.IsNullOrEmpty(model.ASSIGNED_lstAdminQmCode))
                        {
                            return Json(new { success = false, message = "Please select at least one Monitor" }, JsonRequestBehavior.AllowGet);
                        }
                        if (ModelState.IsValid)
                        {

                            status = objDAL.AddNQMNAInspectionDAL(model, ref message);
                            if (status)
                            {
                                //return Json(new { success = status, message = "NQM details added successfully!!" }, JsonRequestBehavior.AllowGet);
                                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
                            }
                            else
                            {
                                return Json(new { success = false, message = "NQM details deleted." }, JsonRequestBehavior.AllowGet);
                            }
                        }
                        else
                        {
                            // return Json(new { success = false, message = "Error occurred " }, JsonRequestBehavior.AllowGet);
                            return Json(new { Success = false, ErrorMessage = HttpUtility.HtmlEncode(new CommonFunctions().FormatErrorMessage(ModelState)) });
                        }
                        return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json(new { success = false, message = "NQM details cannot be added for the selected month" }, JsonRequestBehavior.AllowGet);
                    }
                }

            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "QualityMonitoring.AddNQMNAForInspection()");
                return Json(new { success = false, message = "Error occurred while deleting NQM details" }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public ActionResult SearchNQMNAForInspectionLayout()
        {
            NQMNotAvailableInspectionViewModel model = new NQMNotAvailableInspectionViewModel();
            CommonFunctions comm = new CommonFunctions();
            PMGSY.DAL.QualityMonitoring.QualityMonitoringDAL objDAL = new PMGSY.DAL.QualityMonitoring.QualityMonitoringDAL();

            try
            {
                model.lstMonth = comm.PopulateMonths(true);
                model.lstYear = comm.PopulateYears(true);
                model.lstAdminQmCode = comm.PopulateMonitorsForScheduleCreation(String.Empty, "I", 0, DateTime.Now.Month, DateTime.Now.Year);
                if (System.DateTime.Now.Month == 12)
                {
                    model.lstYear.Insert(1, (new SelectListItem { Text = (System.DateTime.Now.Year + 1).ToString(), Value = (System.DateTime.Now.Year + 1).ToString() }));
                }
                model.Month = DateTime.Now.Month;
                model.Year = DateTime.Now.Year;

                return View(model);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "QualityMonitoring.SearchNQMNAForInspectionLayout()");
                return null;
            }
        }

        [HttpPost]
        public ActionResult ListNQMNotAvailableInspections(FormCollection formCollection)
        {
            if (PMGSYSession.Current == null)
            {
                Response.Redirect("/Login/Login");
            }
            int month = Convert.ToInt32(Request.Params["month"]);
            int year = Convert.ToInt32(Request.Params["year"]);
            long totalRecords = 0;
            PMGSY.DAL.QualityMonitoring.QualityMonitoringDAL objDAL = new PMGSY.DAL.QualityMonitoring.QualityMonitoringDAL();

            //objProposalBAL
            try
            {
                using (CommonFunctions commonFunction = new CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new GridParams(Convert.ToInt32(formCollection["page"]), Convert.ToInt32(formCollection["rows"]), formCollection["sidx"], formCollection["sord"], Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                    {
                        return null;
                    }
                }

                var jsonData = new
                {
                    rows = objDAL.ListNQMNotAvailableInspectionsDAL(month, year, Convert.ToInt32(formCollection["page"]) - 1, Convert.ToInt32(formCollection["rows"]), formCollection["sidx"], formCollection["sord"], out totalRecords),
                    total = totalRecords <= Convert.ToInt32(formCollection["rows"]) ? 1 : (totalRecords % Convert.ToInt32(formCollection["rows"]) == 0 ? totalRecords / Convert.ToInt32(formCollection["rows"]) : totalRecords / Convert.ToInt32(formCollection["rows"]) + 1),//totalRecords <= Convert.ToInt32(rows) ? 1 : totalRecords / Convert.ToInt32(rows) + 1,
                    page = Convert.ToInt32(formCollection["page"]),
                    records = totalRecords
                };
                return Json(jsonData, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                //Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                ErrorLog.LogError(ex, "QualityMonitoring.ListInspectionTargetEntered()");
                return null;
            }
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        [Audit]
        public ActionResult DeleteNQMNADetails(string parameter, string hash, string key)
        {
            if (PMGSYSession.Current == null)
            {
                Response.Redirect("/Login/Login");
            }

            PMGSY.DAL.QualityMonitoring.QualityMonitoringDAL objDAL = new PMGSY.DAL.QualityMonitoring.QualityMonitoringDAL();
            //int month = Convert.ToInt32(Request.Params["month"]);
            //int year = Convert.ToInt32(Request.Params["year"]);
            //int qmCode = Convert.ToInt32(Request.Params["qmCode"]);

            string message = string.Empty;

            Dictionary<string, string> decryptedParameters = null;

            QualityMonitoringBAL objBAL = new QualityMonitoringBAL();

            decryptedParameters = URLEncrypt.DecryptParameters1(new string[] { parameter, hash, key });


            try
            {
                if (decryptedParameters.Count > 0)
                {
                    int month = Convert.ToInt32(decryptedParameters["Month"].ToString());
                    int year = Convert.ToInt32(decryptedParameters["Year"].ToString());
                    int qmCode = Convert.ToInt32(decryptedParameters["QmCode"].ToString());

                    bool status = objDAL.DeleteNQMNADetailsDAL(qmCode, month, year, ref message);
                    if (status == true)
                    {
                        return Json(new { success = true, message = "NQM Details Deleted successfully" });
                    }
                    else
                    {
                        return Json(new { success = false, message = "NQM Details could not be deleted" });
                    }
                }
                message = "An error occured while processing you request.";
                return Json(new { success = false, message = message }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "QualityMonitoring.DeleteNQMNADetails()");
                return Json(new { success = false, message = "Error occured while deleting NQM Details" });
            }
        }

        [HttpGet]
        public ActionResult QMAutoScheduleDetailsLayout()
        {
            AutoScheduleViewModel model = new AutoScheduleViewModel();
            CommonFunctions comm = new CommonFunctions();
            PMGSY.DAL.QualityMonitoring.QualityMonitoringDAL qualityDAL = new PMGSY.DAL.QualityMonitoring.QualityMonitoringDAL();
            int year = 0;
            string monthName = string.Empty;
            string monthName2 = string.Empty;
            try
            {
                model.Month = qualityDAL.GetLastMonthInspected1(out year);
                model.Year = year;

                //model.lstYear = comm.PopulateYears(true);

                model.lstMonth = comm.PopulateMonths(true);
                //model.lstMonth.RemoveRange(0, model.Month + 1);

                model.lstYear = new List<SelectListItem>();

                //if (model.Month == 12)
                /// {

                // model.lstMonth.Insert(0, new SelectListItem() { Text = "Janaury", Value = 1.ToString() });
                // model.lstMonth.Insert(1, new SelectListItem() { Text = "February", Value = 2.ToString() });
                // model.lstYear.Insert(0, new SelectListItem() { Text = (model.Year + 1).ToString(), Value = (model.Year + 1).ToString() });
                //}

                //  else if (model.Month == 11)
                // {
                // model.lstMonth.Clear();
                //model.lstMonth.Insert(0, new SelectListItem() { Text = "December", Value = 12.ToString() });
                //model.lstMonth.Insert(1, new SelectListItem() { Text = "Janaury", Value = 1.ToString() });
                // model.lstYear.Insert(0, new SelectListItem() { Text = (model.Year).ToString(), Value = (model.Year).ToString() });
                //model.lstYear.Insert(1, new SelectListItem() { Text = (model.Year + 1).ToString(), Value = (model.Year + 1).ToString() });

                //  }

                //  else
                //{
                monthName = model.lstMonth.Find(c => c.Value == (model.Month).ToString()).Text;
                model.lstMonth.Clear();
                // monthName2 = model.lstMonth.Find(c => c.Value == (model.Month + 2).ToString()).Text;

                // model.lstMonth.Clear();
                model.lstMonth.Insert(0, new SelectListItem() { Text = monthName, Value = (model.Month).ToString() });
                //model.lstMonth.Insert(1, new SelectListItem() { Text = monthName2, Value = (model.Month + 2).ToString() });
                model.lstYear.Insert(0, new SelectListItem() { Text = (model.Year).ToString(), Value = (model.Year).ToString() });
                // }
                return View(model);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "QualityMonitoring.QMAutoScheduleDetailsLayout()");
                return null;
            }
        }




        [ValidateAntiForgeryToken]
        [HttpPost]
        [Audit]
        public ActionResult QMAddAutoScheduleDetails(AutoScheduleViewModel model)
        {
            PMGSY.DAL.QualityMonitoring.QualityMonitoringDAL qualityDAL = new PMGSY.DAL.QualityMonitoring.QualityMonitoringDAL();

            try
            {
                //if (!String.IsNullOrEmpty(parameter) && !String.IsNullOrEmpty(hash) && !String.IsNullOrEmpty(key))
                //{
                //    String[] urlParams = URLEncrypt.DecryptParameters(new String[] { parameter, hash, key });
                //    if (urlParams.Length >= 1)
                //    {
                //        String[] urlSplitParams = urlParams[0].Split('$');

                //        targetId = Convert.ToInt32(urlParams[0]);
                //    }
                //}
                //  int year=0; 
                int year2 = qualityDAL.GetLastYearInspected();
                int monthPrevieous = 0;
                int year = 0;
                monthPrevieous = qualityDAL.GetLastMonthInspected(out year);
                // change by sachin 27 feb 2020
                if ((model.Month == monthPrevieous) && (model.Year == year2))
                {
                    return Json(new { Success = false, ErrorMessage = "Schedule has already been assigned for this month and year in autoscheduling" });
                }

                if (model.Month == 12 && model.Year != year2)
                {
                    return Json(new { Success = false, ErrorMessage = "Please select valid year and month" });
                }

                string Status = qualityDAL.QMAddAutoScheduleDetailsDAL(model);


                if (Status == string.Empty)
                    return Json(new { Success = true, message = "NQM Auto Schedule Details added successfully!!" });

                else
                    return Json(new { Success = false, ErrorMessage = "Cannot add NQM Auto Schedule Details, please follow all steps for assigning schedule." });

            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "QualityMonitoring.QMAddAutoScheduleDetails()");
                return Json(new { Success = false, ErrorMessage = "An Error Occurred While Processing Your Request." });
            }
        }




        #endregion

        //added by abhinav pathak
        #region Delete uploaded inspection image uploaded through web
        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult DeleteinspectionImageFiles()
        {
            bool success = false;
            try
            {
                // changes done by saurabh starts here
                string SQM_Path = string.Empty; string NQM_Path = string.Empty; string SQM_ThumbNail = string.Empty;
                string NQM_ThumbNail = string.Empty;
                bool SQM_Status = false;
                bool NQM_Status = false;

                //string NQM_Path_NEW = string.Empty; string SQM_Path_NEW = string.Empty; string fULL_NQMPath = string.Empty; string fULL_SQMPath = string.Empty;
                //string NQM_Path_NEW_Thumbnail = string.Empty; string SQM_Path_NEW_Thumbnail = string.Empty; string fULL_NQMPath_Thumb = string.Empty;
                //string fULL_SQMPath_Thumb = string.Empty;
                // changes done by saurabh ended here...
                qualityBAL = new QualityMonitoringBAL();
                int fileId = Convert.ToInt32(Request.Params["fileID"]);
                string filename = Request.Params["filename"];
                string Year = Request.Params["DictionYear"];   // changes by saurabh

                bool isFileDetailsDeleted = qualityBAL.DeleteInspectionImageBAL(fileId, filename, Convert.ToInt32(Request.Params["observationID"]), Year);  // changes by saurabh
                //string FileVirtualPath = Path.Combine(ConfigurationManager.AppSettings["QUALITY_INSPECTION_FILE_UPLOAD_NQM"], Request.Params["filename"]);
                // Changes by saurabh starts here...
                if (!(filename.Contains("$")))
                {
                    SQM_Path = Path.Combine(ConfigurationManager.AppSettings["QUALITY_INSPECTION_FILE_UPLOAD_SQM"], filename).Replace("\\\\", "\\");
                    NQM_Path = Path.Combine(ConfigurationManager.AppSettings["QUALITY_INSPECTION_FILE_UPLOAD_NQM"], filename).Replace("\\\\", "\\");
                    SQM_Status = System.IO.File.Exists(SQM_Path);
                    NQM_Status = System.IO.File.Exists(NQM_Path);
                }
                else
                {

                    NQM_Path = Path.Combine(ConfigurationManager.AppSettings["QUALITY_INSPECTION_FILE_UPLOAD_NQM_NEW"], Year, filename);
                    SQM_Path = Path.Combine(ConfigurationManager.AppSettings["QUALITY_INSPECTION_FILE_UPLOAD_SQM_NEW"], Year, filename);
                    NQM_ThumbNail = Path.Combine(ConfigurationManager.AppSettings["QUALITY_INSPECTION_FILE_UPLOAD_NQM_NEW"], Year, "thumbnails", filename);
                    SQM_ThumbNail = Path.Combine(ConfigurationManager.AppSettings["QUALITY_INSPECTION_FILE_UPLOAD_SQM_NEW"], Year, "thumbnails", filename);
                    NQM_Status = System.IO.File.Exists(NQM_Path);
                    SQM_Status = System.IO.File.Exists(SQM_Path);
                }
                if (isFileDetailsDeleted)
                {
                    if (!(filename.Contains("$")))
                    {
                        if (SQM_Status == true)
                        {
                            System.IO.File.Move(SQM_Path, Path.Combine(ConfigurationManager.AppSettings["QUALITY_INSPECTION_FILE_UPLOAD_SQM_DELETED_IMAGES"], Request.Params["filename"]));
                        }
                        else if (NQM_Status == true)
                        {
                            System.IO.File.Move(NQM_Path, Path.Combine(ConfigurationManager.AppSettings["QUALITY_INSPECTION_FILE_UPLOAD_NQM_DELETED_IMAGES"], Request.Params["filename"]));
                        }
                    }
                    else
                    {
                        if (NQM_Status == true)
                        {
                            System.IO.File.Move(NQM_ThumbNail, Path.Combine(ConfigurationManager.AppSettings["QUALITY_INSPECTION_FILE_UPLOAD_NQM_DELETED_IMAGES"], "thumbnails", filename));

                            System.IO.File.Move(NQM_Path, Path.Combine(ConfigurationManager.AppSettings["QUALITY_INSPECTION_FILE_UPLOAD_NQM_DELETED_IMAGES"], filename));

                        }
                        else if (SQM_Status == true)
                        {
                            System.IO.File.Move(SQM_ThumbNail, Path.Combine(ConfigurationManager.AppSettings["QUALITY_INSPECTION_FILE_UPLOAD_SQM_DELETED_IMAGES"], "thumbnails", filename));

                            System.IO.File.Move(SQM_Path, Path.Combine(ConfigurationManager.AppSettings["QUALITY_INSPECTION_FILE_UPLOAD_SQM_DELETED_IMAGES"], filename));

                        }

                    }

                    return Json(success = true);

                }
                return Json(success);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "QualityMonitoring/DeleteinspectionImageFiles");
                return Json(success);
            }

        }

        #endregion

        #region To upload pdf file by NQM,SQM,SQC,CQC,PIU
        [HttpGet]
        public ActionResult PdfFileUploadView(string id)
        {
            dbContext = new PMGSYEntities();
            try
            {
                PMGSY.Models.QualityMonitoring.FileUploadViewModel fileUploadViewModel = new PMGSY.Models.QualityMonitoring.FileUploadViewModel();
                fileUploadViewModel.QM_OBSERVATION_ID = Convert.ToInt32(id.Split(',')[0]);
                fileUploadViewModel.IMS_PR_ROAD_CODE = Convert.ToInt32(id.Split(',')[1]);
                fileUploadViewModel.ErrorMessage = string.Empty;
                fileUploadViewModel.NumberofPdfs = 0;
                return View(fileUploadViewModel);
            }
            catch (Exception)
            {
                return null;
            }
        }

        [HttpPost]
        public ActionResult InspPdfFileUploadPost(PMGSY.Models.QualityMonitoring.FileUploadViewModel fileUploadViewModel)
        {
            try
            {
                qualityBAL = new QualityMonitoringBAL();
                dbContext = new PMGSYEntities();
                CommonFunctions objCommonFunc = new CommonFunctions();

                foreach (string file in Request.Files)
                {
                    string status = qualityBAL.ValidatePDFFile(Request.Files[0].ContentLength, Path.GetExtension(Request.Files[0].FileName));
                    if (status != string.Empty)
                    {
                        fileUploadViewModel.ErrorMessage = status;
                        return View("PdfFileUploadView", fileUploadViewModel.ErrorMessage);
                    }
                }

                var fileData = new List<PMGSY.Models.QualityMonitoring.FileUploadViewModel>();

                int obsId = 0;
                if (fileUploadViewModel.QM_OBSERVATION_ID != 0)
                {
                    obsId = fileUploadViewModel.QM_OBSERVATION_ID;
                }
                else
                {
                    try
                    {
                        obsId = Convert.ToInt32(Request["QM_OBSERVATION_ID"]);
                    }
                    catch
                    {
                        if (Request["QM_OBSERVATION_ID"].Contains(','))
                        {
                            obsId = Convert.ToInt32(Request["QM_OBSERVATION_ID"].Split(',')[0]);
                        }
                    }
                }

                var qmTypeForUser = (from aqm in dbContext.ADMIN_QUALITY_MONITORS
                                     join qqs in dbContext.QUALITY_QM_SCHEDULE on aqm.ADMIN_QM_CODE equals qqs.ADMIN_QM_CODE
                                     join qqom in dbContext.QUALITY_QM_OBSERVATION_MASTER on qqs.ADMIN_SCHEDULE_CODE equals qqom.ADMIN_SCHEDULE_CODE
                                     where qqom.QM_OBSERVATION_ID == obsId
                                     select aqm).First();

                //string VirtualDirectoryUrl = string.Empty;
                string PhysicalPath = string.Empty;
                if (qmTypeForUser.ADMIN_QM_TYPE.Equals("I"))
                {
                    //VirtualDirectoryUrl = ConfigurationManager.AppSettings["QUALITY_INSPECTION_REPORT_FILE_NQM_VIRTUAL_PATH"];
                    PhysicalPath = ConfigurationManager.AppSettings["QUALITY_INSPECTION_PDF_FILE_NQM"];
                }
                else if (qmTypeForUser.ADMIN_QM_TYPE.Equals("S"))
                {
                    //VirtualDirectoryUrl = ConfigurationManager.AppSettings["QUALITY_INSPECTION_REPORT_FILE_SQM_VIRTUAL_PATH"];
                    PhysicalPath = ConfigurationManager.AppSettings["QUALITY_INSPECTION_PDF_FILE_SQM"];
                }

                if (!(objCommonFunc.ValidateIsPdf(PhysicalPath, Request)))
                {
                    fileUploadViewModel.ErrorMessage = "File Type is Not Allowed.";
                    return View("PdfFileUploadView", fileUploadViewModel.ErrorMessage);
                }

                foreach (string file in Request.Files)
                {
                    UploadMultipleInspPDFFile(Request, fileData, obsId);
                }


                fileUploadViewModel.NumberofPdfs = 0;

                var serializer = new JavaScriptSerializer();
                serializer.MaxJsonLength = Int32.MaxValue;

                var result = new ContentResult
                {
                    Content = "{\"files\":" + serializer.Serialize(fileData) + "}",
                };
                return result;
            }
            catch (Exception)
            {
                fileUploadViewModel.ErrorMessage = "An Error Occurred While Processing Your Request.";
                return View("PdfFileUploadView", fileUploadViewModel.ErrorMessage);
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        //Save pdf file.
        public void UploadMultipleInspPDFFile(HttpRequestBase request, List<PMGSY.Models.QualityMonitoring.FileUploadViewModel> statuses, int obsId)
        {
            dbContext = new PMGSYEntities();
            qualityBAL = new QualityMonitoringBAL();
            int MaxID = 0;
            try
            {
                for (int i = 0; i < request.Files.Count; i++)
                {
                    HttpPostedFileBase file = request.Files[i];
                    if (dbContext.QUALITY_QM_INSPECTION_FILES.Count() == 0)
                    {
                        MaxID = 1;
                    }
                    else
                    {
                        MaxID = (from c in dbContext.QUALITY_QM_INSPECTION_FILES select c.QM_FILE_ID).Max();
                        ++MaxID;
                    }

                    var fileName = obsId + "_" + MaxID + Path.GetExtension(request.Files[i].FileName).ToString();

                    var qmTypeForUser = (from aqm in dbContext.ADMIN_QUALITY_MONITORS
                                         join qqs in dbContext.QUALITY_QM_SCHEDULE on aqm.ADMIN_QM_CODE equals qqs.ADMIN_QM_CODE
                                         join qqom in dbContext.QUALITY_QM_OBSERVATION_MASTER on qqs.ADMIN_SCHEDULE_CODE equals qqom.ADMIN_SCHEDULE_CODE
                                         where qqom.QM_OBSERVATION_ID == obsId
                                         select aqm).First();

                    string PhysicalPath = string.Empty;
                    if (qmTypeForUser.ADMIN_QM_TYPE.Equals("I"))
                    {
                        //VirtualDirectoryUrl = ConfigurationManager.AppSettings["QUALITY_INSPECTION_REPORT_FILE_NQM_VIRTUAL_PATH"];
                        PhysicalPath = ConfigurationManager.AppSettings["QUALITY_INSPECTION_PDF_FILE_NQM"];
                    }
                    else if (qmTypeForUser.ADMIN_QM_TYPE.Equals("S"))
                    {
                        //VirtualDirectoryUrl = ConfigurationManager.AppSettings["QUALITY_INSPECTION_REPORT_FILE_SQM_VIRTUAL_PATH"];
                        PhysicalPath = ConfigurationManager.AppSettings["QUALITY_INSPECTION_PDF_FILE_SQM"];
                    }

                    var fullPath = Path.Combine(PhysicalPath, fileName);

                    statuses.Add(new PMGSY.Models.QualityMonitoring.FileUploadViewModel()
                    {
                        url = fullPath,
                        thumbnail_url = fullPath,
                        name = fileName,
                        type = file.ContentType,
                        size = file.ContentLength,
                        delete_url = "",
                        delete_type = "DELETE",

                        PdfDescription = request.Params["InspPdfDescription[]"],

                        QM_OBSERVATION_ID = obsId
                    });

                    string status = qualityBAL.AddMultiplePdfUploadDetailsBAL(statuses);
                    if (status == string.Empty)
                    {
                        file.SaveAs(Path.Combine(PhysicalPath, fileName));
                    }
                    else
                    {

                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        //List uploaded pdf.
        [HttpPost]
        public JsonResult ListInspMultiplePDFFiles(FormCollection formCollection)
        {
            qualityBAL = new QualityMonitoringBAL();
            using (CommonFunctions commonFunction = new CommonFunctions())
            {
                if (!commonFunction.ValidateGridParameters(new GridParams(Convert.ToInt32(formCollection["page"]), Convert.ToInt32(formCollection["rows"]), formCollection["sidx"], formCollection["sord"], Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                {
                    return null;
                }
            }

            int obsId = Convert.ToInt32(Request["QM_OBSERVATION_ID"]);
            int totalRecords;
            var jsonData = new
            {
                rows = qualityBAL.GetInspMultipleFilesListBAL(Convert.ToInt32(formCollection["page"]) - 1, Convert.ToInt32(formCollection["rows"]), formCollection["sidx"], formCollection["sord"], out totalRecords, obsId),
                total = totalRecords <= Convert.ToInt32(formCollection["rows"]) ? 1 : totalRecords / Convert.ToInt32(formCollection["rows"]) + 1,
                page = Convert.ToInt32(formCollection["page"]),
                records = totalRecords
            };
            return Json(jsonData);
        }

        //Download uploaded pdf
        [HttpGet]
        public ActionResult DownloadMultipleInspFile(String parameter, String hash, String key)
        {
            try
            {
                dbContext = new PMGSYEntities();
                string FileName = string.Empty;
                int obsId = 0;
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
                        obsId = Convert.ToInt32(urlSplitParams[1]);
                    }
                }
                FileExtension = Path.GetExtension(FileName).ToLower();

                var qmTypeForUser = (from aqm in dbContext.ADMIN_QUALITY_MONITORS
                                     join qqs in dbContext.QUALITY_QM_SCHEDULE on aqm.ADMIN_QM_CODE equals qqs.ADMIN_QM_CODE
                                     join qqom in dbContext.QUALITY_QM_OBSERVATION_MASTER on qqs.ADMIN_SCHEDULE_CODE equals qqom.ADMIN_SCHEDULE_CODE
                                     where qqom.QM_OBSERVATION_ID == obsId
                                     select aqm).First();

                string VirtualDirectoryUrl = string.Empty;
                string PhysicalPath = string.Empty;
                if (qmTypeForUser.ADMIN_QM_TYPE.Equals("I"))
                {
                    VirtualDirectoryUrl = ConfigurationManager.AppSettings["QUALITY_INSPECTION_PDF_FILE_NQM_VIRTUAL_DIR_PATH"];
                    PhysicalPath = ConfigurationManager.AppSettings["QUALITY_INSPECTION_PDF_FILE_NQM"];
                }
                else if (qmTypeForUser.ADMIN_QM_TYPE.Equals("S"))
                {
                    VirtualDirectoryUrl = ConfigurationManager.AppSettings["QUALITY_INSPECTION_PDF_FILE_SQM_VIRTUAL_DIR_PATH"];
                    PhysicalPath = ConfigurationManager.AppSettings["QUALITY_INSPECTION_PDF_FILE_SQM"];
                }

                FullFileLogicalPath = Path.Combine(VirtualDirectoryUrl, FileName);
                FullfilePhysicalPath = Path.Combine(PhysicalPath, FileName);


                string name = Path.GetFileName(FileName);
                string ext = Path.GetExtension(FileName);

                if (System.IO.File.Exists(FullfilePhysicalPath))
                {
                    return File(FullfilePhysicalPath, "Application/pdf", DateTime.Now.ToShortDateString() + "_" + new Random().Next(1000000000) + FileExtension);
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

        [HttpPost]
        public ActionResult FinaliseMultipleInspFile()
        {
            try
            {
                qualityBAL = new QualityMonitoringBAL();
                var status = qualityBAL.FinalisePDFDeatilsBAL(Convert.ToInt32(Request.Params["QM_FILE_ID"]));
                return Json(new { success = status }, JsonRequestBehavior.DenyGet);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "Qualitymonitoring/FinaliseMultipleInspFile");
                return null;
            }
        }

        //TO UPDATE THE DETAILS OF PDF FILES
        [HttpPost]
        public JsonResult UpdateMultipleInspPDFDetails(FormCollection formCollection)
        {
            qualityBAL = new QualityMonitoringBAL();
            try
            {
                string[] arrKey = formCollection["id"].Split('$');
                PMGSY.Models.QualityMonitoring.FileUploadViewModel fileuploadViewModel = new PMGSY.Models.QualityMonitoring.FileUploadViewModel();
                fileuploadViewModel.QM_OBSERVATION_ID = Convert.ToInt32(arrKey[1]);
                fileuploadViewModel.QM_FILE_ID = Convert.ToInt32(arrKey[0]);

                Regex regex = new Regex(@"^[a-zA-Z0-9 ,.()-]+$");
                if (regex.IsMatch(formCollection["Description"]) && formCollection["Description"].Trim().Length != 0)
                {
                    fileuploadViewModel.PdfDescription = formCollection["Description"];
                }
                else
                {
                    return Json("Invalid PDF Description, Only Alphabets,Numbers and ,.()- are allowed");
                }

                string status = qualityBAL.UpdateMultipleInspPDFDetailsBAL(fileuploadViewModel);

                if (status == string.Empty)
                    return Json(true);
                else
                    return Json("There is an error occurred while processing your request.");
            }
            catch (Exception)
            {
                return Json("There is an error occurred while processing your request.");
            }
        }

        [HttpPost]
        public JsonResult DeleteMultipleInspFileDetails(string id)
        {
            dbContext = new PMGSYEntities();
            qualityBAL = new QualityMonitoringBAL();
            try
            {
                string FILE_NAME = Request.Params["FILE_NAME"];

                string[] arrParam = Request.Params["QM_OBSERVATION_ID"].Split('$');

                int FILE_ID = Convert.ToInt32(arrParam[0]);
                int QM_OBSERVATION_ID = Convert.ToInt32(arrParam[1]);

                var qmTypeForUser = (from aqm in dbContext.ADMIN_QUALITY_MONITORS
                                     join qqs in dbContext.QUALITY_QM_SCHEDULE on aqm.ADMIN_QM_CODE equals qqs.ADMIN_QM_CODE
                                     join qqom in dbContext.QUALITY_QM_OBSERVATION_MASTER on qqs.ADMIN_SCHEDULE_CODE equals qqom.ADMIN_SCHEDULE_CODE
                                     where qqom.QM_OBSERVATION_ID == QM_OBSERVATION_ID
                                     select aqm).First();

                //string VirtualDirectoryUrl = string.Empty;
                string PhysicalPath = string.Empty;
                if (qmTypeForUser.ADMIN_QM_TYPE.Equals("I"))
                {
                    //VirtualDirectoryUrl = ConfigurationManager.AppSettings["QUALITY_INSPECTION_REPORT_FILE_NQM_VIRTUAL_PATH"];
                    PhysicalPath = ConfigurationManager.AppSettings["QUALITY_INSPECTION_PDF_FILE_NQM"];
                }
                else if (qmTypeForUser.ADMIN_QM_TYPE.Equals("S"))
                {
                    //VirtualDirectoryUrl = ConfigurationManager.AppSettings["QUALITY_INSPECTION_REPORT_FILE_SQM_VIRTUAL_PATH"];
                    PhysicalPath = ConfigurationManager.AppSettings["QUALITY_INSPECTION_PDF_FILE_SQM"];
                }

                PhysicalPath = Path.Combine(PhysicalPath, FILE_NAME);
                string status = qualityBAL.DeleteMultipleInspFileDetailsBAL(FILE_ID, QM_OBSERVATION_ID);

                if (status == string.Empty)
                {
                    try
                    {
                        System.IO.File.Delete(PhysicalPath);
                    }
                    catch (Exception ex)
                    {
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

        #endregion

        [HttpPost]
        public JsonResult CheckDeleteStatus(int scheduleCode)
        {
            dbContext = new PMGSYEntities();
            qualityBAL = new QualityMonitoringBAL();
            if (scheduleCode != null)
            {
                //int schedule_Code = Int32.Parse(scheduleCode);
                //qualityBAL.CheckDeleteStatusBAL(scheduleCode);
                var details = dbContext.QUALITY_QM_SCHEDULE_DETAILS.Where
                                 (x => x.ADMIN_SCHEDULE_CODE == scheduleCode).FirstOrDefault();


                //var result = (PMGSYSession.Current.RoleCode == 9 || PMGSYSession.Current.RoleCode == 5)
                //                              ? (details.FINALIZE_FLAG.ToUpper() == "CQC") ? "Y" : "N"
                //                              : ((PMGSYSession.Current.RoleCode == 8 || PMGSYSession.Current.RoleCode == 69 || PMGSYSession.Current.RoleCode == 48)
                //                                   ? (details.FINALIZE_FLAG.ToUpper() != "FSQC") ? "Y" : "N"
                //                                   : "N");

                if (details != null)
                {
                    var result = (PMGSYSession.Current.RoleCode == 9 || PMGSYSession.Current.RoleCode == 5)
                                      ? (details.FINALIZE_FLAG.Trim() != "FSQC" && details.SCHEDULE_ASSIGNED.Trim() == "C" && details.INSP_STATUS_FLAG.Trim().Trim() == "S") ? "Y" : "N"
                                      : (PMGSYSession.Current.RoleCode == 8 || PMGSYSession.Current.RoleCode == 69 || PMGSYSession.Current.RoleCode == 48)
                                           ? (details.FINALIZE_FLAG.Trim() != "FSQC" && details.SCHEDULE_ASSIGNED.Trim() != "C" && (dbContext.QUALITY_QM_SCHEDULE.Where(c => c.ADMIN_SCHEDULE_CODE == scheduleCode).Select(c => c.INSP_STATUS_FLAG).FirstOrDefault().ToUpper()) != "UPGF") ? "Y" : "N"
                                           : (PMGSYSession.Current.RoleCode == 22 || PMGSYSession.Current.RoleCode == 38)
                                              ? (details.FINALIZE_FLAG.Trim() != "FSQC" && details.SCHEDULE_ASSIGNED.Trim() == "D" && details.INSP_STATUS_FLAG.Trim().Trim() == "S") ? "Y" : "N"
                                              : PMGSYSession.Current.RoleCode == 6
                                                 ? (details.FINALIZE_FLAG.Trim() == "NQM" && details.INSP_STATUS_FLAG.Trim().Trim() == "S") ? "Y" : "N"
                                                 : PMGSYSession.Current.RoleCode == 7
                                                    ? (details.FINALIZE_FLAG.Trim() == "SQM" && details.INSP_STATUS_FLAG.Trim().Trim() == "S") ? "Y" : "N"
                                                    : "N";

                    return Json(new { Success = true, status = result });

                }
                else
                {
                    return Json(new { Success = false });

                }




            }
            else
            {
                return Json(new { Success = false });
            }


        }

        public ActionResult AnalysisOfLowQuotedPackage()
        {
            return View();
        }

        #region  Upload Inspection by NRIDA Officials

        public ActionResult InspByNRIDAOfficials()
        {
            CommonFunctions objCommon = new CommonFunctions();
            ViewFilterInspectionByNRIDA model = new ViewFilterInspectionByNRIDA();

            if (PMGSYSession.Current.RoleCode == 25 || PMGSYSession.Current.RoleCode == 5 || PMGSYSession.Current.RoleCode == 9)  // for MORD 
            {
                model.STATES = objCommon.PopulateStates(true);
                model.DISTRICTS.Insert(0, (new SelectListItem { Text = "Select Districts", Value = "-1", Selected = true }));
                model.BLOCKS.Insert(0, (new SelectListItem { Text = "All Blocks", Value = "-1", Selected = true }));
                model.Years = PopulateYear(0, true, true);
                model.BATCHS = objCommon.PopulateBatch();
                model.BATCHS.RemoveAt(0);
                model.BATCHS.Insert(0, (new SelectListItem { Text = "All Batches", Value = "0", Selected = true }));
            }
            else if (PMGSYSession.Current.RoleCode == 2)  // for SRRDA
            {
                model.MAST_STATE_CODE = PMGSYSession.Current.StateCode;
                model.STATES = objCommon.PopulateStates(false);
                model.DISTRICTS = objCommon.PopulateDistrict(model.MAST_STATE_CODE, true);
                model.DISTRICTS.RemoveAt(0);
                model.DISTRICTS.Insert(0, (new SelectListItem { Text = "All Districts", Value = "0", Selected = true }));
                model.BLOCKS.Insert(0, (new SelectListItem { Text = "All Blocks", Value = "-1", Selected = true }));
                model.Years = PopulateYear(0, true, true);
                model.BATCHS = objCommon.PopulateBatch();
                model.BATCHS.RemoveAt(0);
                model.BATCHS.Insert(0, (new SelectListItem { Text = "All Batches", Value = "0", Selected = true }));
            }

            return View(model);
        }

        [HttpPost]
        [Audit]
        public JsonResult GetDistrictsByNRIDA(int stateCode)
        {
            try
            {
                CommonFunctions objCommon = new CommonFunctions();
                List<SelectListItem> lstDistrict = new List<SelectListItem>();
                lstDistrict = objCommon.PopulateDistrict(stateCode, false);
                lstDistrict.RemoveAt(0);
                lstDistrict.Insert(0, (new SelectListItem { Text = "Select Districts", Value = "-1", Selected = true }));
                return Json(lstDistrict);
            }
            catch
            {
                return Json(new { string.Empty });
            }
        }

        [Audit]
        public ActionResult GetInspRoadList(int? page, int? rows, string sidx, string sord)
        {
            try
            {
                int stateCode, yearCode, districtCode, blockCode, batch, scheme;
                string proposalType;

                long totalRecords = 0;
                IQualityMonitoringBAL objBAL = new QualityMonitoringBAL();

                using (CommonFunctions commonFunction = new CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new GridParams(page, rows, sidx, sord, Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                    {
                        return null;
                    }
                }

                stateCode = Convert.ToInt32(Request.Params["stateCode"]);
                districtCode = Convert.ToInt32(Request.Params["districtCode"]);
                blockCode = Convert.ToInt32(Request.Params["blockCode"]);
                yearCode = Convert.ToInt32(Request.Params["yearCode"]);
                batch = Convert.ToInt32(Request.Params["batch"]);
                scheme = Convert.ToInt32(Request.Params["scheme"]);
                proposalType = Request.Params["proposalType"];

                var jsonData = new
                {
                    rows = objBAL.GetInspRoadList(stateCode, districtCode, blockCode, yearCode, batch, scheme, proposalType, Convert.ToInt32(page) - 1, Convert.ToInt32(rows), sidx, sord, out totalRecords),
                    total = totalRecords <= Convert.ToInt32(rows) ? 1 : totalRecords / Convert.ToInt32(rows) + 1,
                    page = Convert.ToInt32(page),
                    records = totalRecords,
                };

                return Json(jsonData, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "GetInspRoadList()");
                return null;
            }
        }

        public ActionResult AddInspByNRIDA(string idtemp)
        {
            try
            {
                PMGSYEntities dbContext = new PMGSYEntities();
                AddUploadInspByNRIDADetailsModel model = new AddUploadInspByNRIDADetailsModel();
                string id1 = "0";
                Int32 id = 0;

                if (idtemp != null)
                {
                    string[] encParam = idtemp.Split('/');
                    decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { encParam[0], encParam[1], encParam[2] });
                    if (decryptedParameters.Count > 0)
                    {
                        id1 = decryptedParameters["imsRoadID"];
                    }
                }

                id = Convert.ToInt32(id1);
                DateTime currentDate = DateTime.Now;
                model.IMS_PR_ROAD_CODE = id1;
                model.UploadedDate = currentDate.ToString("dd/MM/yyyy");

                model.GRADE_LIST = new List<SelectListItem>();
                model.GRADE_LIST.Insert(0, (new SelectListItem { Text = "Select Grade", Value = string.Empty, Selected = true }));
                model.GRADE_LIST.Insert(1, (new SelectListItem { Text = "S", Value = "S", Selected = true }));
                model.GRADE_LIST.Insert(2, (new SelectListItem { Text = "SRI", Value = "SRI", Selected = true }));
                model.GRADE_LIST.Insert(3, (new SelectListItem { Text = "U", Value = "U", Selected = true }));

                if (dbContext.QM_IR_ATR_DETAILS.Where(x => x.IMS_PR_ROAD_CODE == id && x.IS_STATE_OR_NRIDA == "S" && x.IS_ACCEPTED == "A").Any())
                    model.ACCEPTED_STATUS = "A";
                else
                    model.ACCEPTED_STATUS = "N";

                if (dbContext.QM_IR_ATR_DETAILS.Where(x => x.IMS_PR_ROAD_CODE == id && x.IS_STATE_OR_NRIDA == "N" && x.IS_FINALIZE == "Y").Any())
                    model.ACCEPTED_STATUS_NRIDA = "Y";
                else
                    model.ACCEPTED_STATUS_NRIDA = "N";

                return View(model);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "AddInspByNRIDA");
                return null;
            }
        }

        [HttpPost]
        public ActionResult AddInspByNRIDADetails(FormCollection formCollection)
        {
            bool status = false;
            string isValidMsg = String.Empty;
            String message = "Details not added";
            IQualityMonitoringBAL objBAL = new QualityMonitoringBAL();
            HttpPostedFileBase postedBgFile = Request.Files[0];
            QM_IR_ATR_DETAILS model = new QM_IR_ATR_DETAILS();
            dbContext = new PMGSYEntities();
            DateTime inspDate = Convert.ToDateTime(formCollection["InspectionDate"]); ;

            try
            {
                int roadCode = Convert.ToInt32(formCollection["IMS_PR_ROAD_CODE"]);

                if ((PMGSYSession.Current.RoleCode == 25 || PMGSYSession.Current.RoleCode == 5 || PMGSYSession.Current.RoleCode == 9) && (inspDate > DateTime.Now))
                    return Json(new { success = false, file = false, message = "Inspection date cannot be greater than today's date." });

                if (PMGSYSession.Current.RoleCode == 2 && dbContext.QM_IR_ATR_DETAILS.Where(x => x.IMS_PR_ROAD_CODE == roadCode && x.IS_STATE_OR_NRIDA == "S").Any())
                {
                    int inspId = (from item in dbContext.QM_IR_ATR_DETAILS where item.IMS_PR_ROAD_CODE == roadCode && item.IS_STATE_OR_NRIDA == "S" select item.INSPECTION_ID).Max();
                    model = dbContext.QM_IR_ATR_DETAILS.Where(x => x.INSPECTION_ID == inspId).FirstOrDefault();

                    if (model.IS_ACCEPTED == null || model.IS_ACCEPTED == string.Empty || model.IS_ACCEPTED == "N" || model.IS_ACCEPTED == "A")
                        return Json(new { success = false, file = false, message = "File cannot be uploaded. File can be uploaded only when the previous uploaded file status is rejected." });
                }
                else if ((PMGSYSession.Current.RoleCode == 25 || PMGSYSession.Current.RoleCode == 5 || PMGSYSession.Current.RoleCode == 9) && dbContext.QM_IR_ATR_DETAILS.Where(x => x.IMS_PR_ROAD_CODE == roadCode && x.IS_STATE_OR_NRIDA == "N").Any())
                    return Json(new { success = false, file = false, message = "Only one file is allowed to upload." });

                if (Request.Files.Count == 0)
                {
                    return Json(new { success = false, file = false, message = "No file selected. Please select file" });
                }
                else
                {
                    int maxSize = 1024 * 1024 * 4;

                    if (postedBgFile.FileName.Substring(postedBgFile.FileName.LastIndexOf('.') + 1).Trim().ToLower() != "pdf")
                        return Json(new { success = false, file = false, message = "Please upload only pdf files." });

                    if (postedBgFile.ContentLength > maxSize)
                        return Json(new { success = false, file = false, message = "Invalid file size. Please upload file upto 4 MB." });
                }

                if (ModelState.IsValid)
                {
                    status = objBAL.AddInspByNRIDADetails(formCollection, postedBgFile, out isValidMsg);

                    return Json(new { success = status, message = isValidMsg });
                }

                return Json(new { success = status, message = message });
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "AddInspByNRIDADetails()");
                return Json(new { success = status, message = message });
            }
        }

        [Audit]
        public ActionResult GetInspByNRIDADetailsList(string idtemp, int? page, int? rows, string sidx, string sord)
        {
            try
            {
                string id1 = "0";
                Int32 id = 0;
                if (idtemp != null)
                {
                    string[] encParam = idtemp.Split('/');
                    decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { encParam[0], encParam[1], encParam[2] });
                    if (decryptedParameters.Count > 0)
                        id1 = decryptedParameters["imsRoadID"];
                }

                id = Convert.ToInt32(id1);

                long totalRecords = 0;

                IQualityMonitoringBAL objBAL = new QualityMonitoringBAL();

                using (CommonFunctions commonFunction = new CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new GridParams(page, rows, sidx, sord, Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                        return null;
                }

                var jsonData = new
                {
                    rows = objBAL.GetInspByNRIDADetailsList(id, Convert.ToInt32(page) - 1, Convert.ToInt32(rows), sidx, sord, out totalRecords),
                    total = totalRecords <= Convert.ToInt32(rows) ? 1 : totalRecords / Convert.ToInt32(rows) + 1,
                    page = Convert.ToInt32(page),
                    records = totalRecords,
                };

                return Json(jsonData, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "GetInspByNRIDADetailsList()");
                return null;
            }
        }

        public FileResult GetInspByNRIDAPdf(string id)
        {
            try
            {
                PMGSYEntities dbContext = new PMGSYEntities();
                QM_IR_ATR_DETAILS model = new QM_IR_ATR_DETAILS();
                int inspId = Convert.ToInt32(id);

                model = dbContext.QM_IR_ATR_DETAILS.Where(x => x.INSPECTION_ID == inspId).FirstOrDefault();

                string FileName = model.FILE_NAME;
                string path = ConfigurationManager.AppSettings["INSPECTION_BY_NRIDA_AND_STATE"].ToString();
                string fullPath = string.Empty;

                if (model.IS_STATE_OR_NRIDA == "N")
                    fullPath = Path.Combine(path, "NRIDA", FileName);
                else if (model.IS_STATE_OR_NRIDA == "S")
                    fullPath = Path.Combine(path, "STATE", FileName);

                byte[] FileBytes = System.IO.File.ReadAllBytes(fullPath);
                return File(FileBytes, "application/pdf");

            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "GetInspByNRIDAPdf()");
                return null;
            }
        }

        [HttpGet]
        public ActionResult DeleteInspByNRIDAPdf(string id)
        {
            try
            {
                PMGSYEntities dbContext = new PMGSYEntities();
                QM_IR_ATR_DETAILS model = new QM_IR_ATR_DETAILS();
                string message = string.Empty;
                bool status = false;

                int inspid = Convert.ToInt32(id);

                model = dbContext.QM_IR_ATR_DETAILS.Where(x => x.INSPECTION_ID == inspid).FirstOrDefault();
                string FileName = model.FILE_NAME;
                string currYear = FileName.Split('_')[0];

                dbContext.QM_IR_ATR_DETAILS.Remove(dbContext.QM_IR_ATR_DETAILS.Where(s => s.INSPECTION_ID == inspid).FirstOrDefault());
                dbContext.SaveChanges();

                string path = ConfigurationManager.AppSettings["INSPECTION_BY_NRIDA_AND_STATE"].ToString();
                string fullPath = string.Empty;

                if (model.IS_STATE_OR_NRIDA == "N")
                    fullPath = Path.Combine(path, "NRIDA", currYear, FileName);
                else if (model.IS_STATE_OR_NRIDA == "S")
                    fullPath = Path.Combine(path, "STATE", currYear, FileName);

                FileInfo file = new FileInfo(fullPath);
                if (file.Exists)
                {
                    file.Delete();
                }

                message = "File deleted successfully.";
                status = true;

                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "DeleteInspByNRIDAPdf()");
                return null;
            }

        }

        [HttpGet]
        public ActionResult FinalizeInspByNRIDAPDetails(string id)
        {
            try
            {
                PMGSYEntities dbContext = new PMGSYEntities();
                QM_IR_ATR_DETAILS model = new QM_IR_ATR_DETAILS();
                string message = string.Empty;
                bool status = false;

                int inspid = Convert.ToInt32(id);

                model = dbContext.QM_IR_ATR_DETAILS.Where(x => x.INSPECTION_ID == inspid).FirstOrDefault();
                model.IS_FINALIZE = "Y";
                dbContext.Entry(model).State = System.Data.Entity.EntityState.Modified;
                dbContext.SaveChanges();

                message = "Data finalized successfully.";
                status = true;

                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "FinalizeInspByNRIDAPDetails()");
                return null;
            }

        }

        [Audit]
        public ActionResult GetInspUploadedDetailsList(string idtemp, int? page, int? rows, string sidx, string sord)
        {
            try
            {
                string id1 = "0";
                Int32 id = 0;
                if (idtemp != null)
                {
                    string[] encParam = idtemp.Split('/');
                    decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { encParam[0], encParam[1], encParam[2] });
                    if (decryptedParameters.Count > 0)
                    {
                        id1 = decryptedParameters["imsRoadID"];
                    }
                }

                id = Convert.ToInt32(id1);

                long totalRecords = 0;

                IQualityMonitoringBAL objBAL = new QualityMonitoringBAL();

                using (CommonFunctions commonFunction = new CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new GridParams(page, rows, sidx, sord, Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                    {
                        return null;
                    }
                }

                var jsonData = new
                {
                    rows = objBAL.GetInspUploadedDetailsList(id, Convert.ToInt32(page) - 1, Convert.ToInt32(rows), sidx, sord, out totalRecords),
                    total = totalRecords <= Convert.ToInt32(rows) ? 1 : totalRecords / Convert.ToInt32(rows) + 1,
                    page = Convert.ToInt32(page),
                    records = totalRecords,
                };

                return Json(jsonData, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "GetInspUploadedDetailsList()");
                return null;
            }
        }

        [HttpPost]
        public ActionResult AcceptOrRejectATR(string inspId, string acceptedStatus, string remark)
        {
            dbContext = new PMGSYEntities();
            try
            {
                QM_IR_ATR_DETAILS model = new QM_IR_ATR_DETAILS();
                using (var scope = new TransactionScope())
                {
                    bool status = false;
                    String message = "Data not updated";
                    int id = Convert.ToInt32(inspId);

                    model = dbContext.QM_IR_ATR_DETAILS.Where(x => x.INSPECTION_ID == id).FirstOrDefault();

                    model.IS_ACCEPTED = acceptedStatus;
                    model.ACCEPT_REJECT_DATE = DateTime.Now;
                    model.ACCEPT_REJECT_REMARK = remark == "" ? null : remark;

                    dbContext.Entry(model).State = System.Data.Entity.EntityState.Modified;
                    dbContext.SaveChanges();

                    status = true;
                    message = "Data updated successfully";

                    scope.Complete();

                    return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Data not updated." });
            }

        }

        #endregion


        #region Work List Added by Chandra Darshan Agrawal

        [HttpGet]
        public ActionResult ViewWorkList()
        {
            try
            {
                PMGSYEntities dbContext = new PMGSYEntities();
                CommonFunctions objCommon = new CommonFunctions();
                WorkListModel model = new WorkListModel();

                if (PMGSYSession.Current.RoleCode == 8 || PMGSYSession.Current.RoleCode == 69)  // SQC
                {
                    model.StateName = PMGSYSession.Current.StateName;
                    model.stateCode = PMGSYSession.Current.StateCode;
                    model.lstDistricts = new List<SelectListItem>();
                    model.lstDistricts = objCommon.PopulateDistrict(model.stateCode, false);

                    model.lstTechnology = new List<SelectListItem>(); // Added by Shreyas
                    model.lstTechnology.Insert(0, (new SelectListItem { Text = "All", Value = "0", Selected = true }));
                    model.lstTechnology.Insert(1, (new SelectListItem { Text = "FDR", Value = "1" }));
                    return View(model);
                }
                else if (PMGSYSession.Current.RoleCode == 9)
                {
                    model.lstStates = objCommon.PopulateStates();
                    model.lstDistricts = new List<SelectListItem>();
                    model.lstDistricts.Insert(0, new SelectListItem() { Text = "Select District", Value = "-1" });

                    model.lstTechnology = new List<SelectListItem>(); // Added by Shreyas
                    model.lstTechnology.Insert(0, (new SelectListItem { Text = "All", Value = "0", Selected = true }));
                    model.lstTechnology.Insert(1, (new SelectListItem { Text = "FDR", Value = "1" }));
                    return View(model);
                }
                else
                    return View("ViewWorkList", model);

            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "ViewWorkList()");
                return null;
            }
        }

        [HttpPost]
        [Audit]
        public ActionResult GetRoadList(FormCollection formCollection)
        {
            qualityBAL = new QualityMonitoringBAL();
            long totalRecords = 0;
            try
            {
                using (CommonFunctions commonFunction = new CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new GridParams(Convert.ToInt32(formCollection["page"]), Convert.ToInt32(formCollection["rows"]), formCollection["sidx"], formCollection["sord"], Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                    {
                        return null;
                    }
                }
                int distCode = Convert.ToInt32(formCollection["districtCode"]);
                int ddlTech = Convert.ToInt32(formCollection["ddlTech"]);  // Added by Shreyas for ddlTech
                int stateCode = PMGSYSession.Current.StateCode;

                var jsonData = new
                {
                    rows = qualityBAL.GetRoadListBAL(stateCode, distCode, ddlTech, Convert.ToInt32(formCollection["page"]) - 1, Convert.ToInt32(formCollection["rows"]), formCollection["sidx"], formCollection["sord"], out totalRecords, formCollection["filters"]),
                    total = totalRecords <= Convert.ToInt32(formCollection["rows"]) ? 1 : (totalRecords % Convert.ToInt32(formCollection["rows"]) == 0 ? totalRecords / Convert.ToInt32(formCollection["rows"]) : (totalRecords / Convert.ToInt32(formCollection["rows"]) + 1)),
                    page = Convert.ToInt32(formCollection["page"]),
                    records = totalRecords
                };
                return Json(jsonData);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return Json(String.Empty);
            }
        }

        #endregion

        #region Monitor Proficiency Test Score

        public ActionResult MonitorProficiencyTestScore()
        {
            string EnabledUserNameForProficiencyTest = System.Configuration.ConfigurationManager.AppSettings["EnabledUserNameForProficiencyTest"];

            ViewData["isUserEnableProficiencyTest"] = "N";

            if (!string.IsNullOrEmpty(EnabledUserNameForProficiencyTest))
            {
                string[] EnabledUserNameList = EnabledUserNameForProficiencyTest.Split(',');

                if (EnabledUserNameList.Contains(PMGSYSession.Current.UserName))
                {
                    ViewData["isUserEnableProficiencyTest"] = "Y";
                }
            }

            if (PMGSYSession.Current.RoleCode == 9)
            {
                return RedirectToAction("MonitorProficiencyTestScoreCQC");
            }

            return View();
        }

        [HttpGet]
        public ActionResult AddProficiencyScore(int examId)
        {
            ProficiencyTestScoreModel model = new ProficiencyTestScoreModel();
            QM_PROFICIENCY_TEST_MASTER masterModel = new QM_PROFICIENCY_TEST_MASTER();
            CommonFunctions objCommon = new CommonFunctions();
            dbContext = new PMGSYEntities();

            masterModel = dbContext.QM_PROFICIENCY_TEST_MASTER.Where(x => x.EXAM_ID == examId).FirstOrDefault();

            model.EXAM_ID = masterModel.EXAM_ID;
            model.INSTITUTION = masterModel.INSTITUTE_NAME;
            model.DATE_OF_EXAM = masterModel.DATE_OF_EXAM;
            model.NQM_SQM = masterModel.MONITOR_TYPE == "I" ? "NQM" : "SQM";

            List<SelectListItem> examStatus = new List<SelectListItem>();
            examStatus.Add(new SelectListItem() { Text = "P", Value = "P" });
            examStatus.Add(new SelectListItem() { Text = "AB", Value = "AB" });
            examStatus.Add(new SelectListItem() { Text = "NRT", Value = "NRT" });
            examStatus.Add(new SelectListItem() { Text = "NA", Value = "NA" });
            examStatus.Insert(0, (new SelectListItem { Text = "Select Exam Status", Value = String.Empty, Selected = true }));
            model.EXAM_STATUS_LIST = examStatus;

            int sCode = 0;
            if (masterModel.STATE_CODE == null)
            {
                sCode = 0;
            }
            else
            {
                sCode = (int)masterModel.STATE_CODE;
            }

            if (masterModel.MONITOR_TYPE == "S")
                model.MONITORS_LIST = objCommon.PopulateMonitorsForScheduleCreation("true", masterModel.MONITOR_TYPE, sCode, 0, 0);
            else
                model.MONITORS_LIST = objCommon.PopulateMonitorsForScheduleCreation("true", masterModel.MONITOR_TYPE, sCode, 0, 0);

            return PartialView(model);
        }

        [HttpPost]
        public ActionResult AddProficiencyTestScore(FormCollection formCollection)
        {
            bool status = false;
            string isValidMsg = String.Empty;
            String message = "Details not added";
            IQualityMonitoringBAL objBAL = new QualityMonitoringBAL();
            int adminQmCode = Convert.ToInt32(formCollection["MONITOR_NAME"]);
            int examId = Convert.ToInt32(formCollection["EXAM_ID"]);
            string monitorStatus = formCollection["MONITOR_STATUS"];
            dbContext = new PMGSYEntities();

            decimal marks;
            string val = formCollection["PERCENTAGE"];
            if (val == "")
                marks = -1;
            else
                marks = Convert.ToDecimal(formCollection["PERCENTAGE"]);

            Regex re = new Regex(@"(^([0-5]?[0-9]|60)$)");
            bool regVal = re.IsMatch(Convert.ToString(marks));

            if (adminQmCode == 0)
                return Json(new { success = false, message = "Please enter monitor name." });

            if (dbContext.QM_PROFICIENCY_TEST_SCORE.Where(x => x.EXAM_ID == examId && x.ADMIN_QM_CODE == adminQmCode).Any())
                return Json(new { success = false, message = "Record for same monitor already exists." });

            if (monitorStatus == "P" && marks == -1)
            {
                return Json(new { success = false, message = "Please enter marks." });
            }

            if (monitorStatus == "P" && !regVal)
                return Json(new { success = false, message = "Please enter marks between 0 to 60 (decimals not allowed)." });

            if (ModelState.IsValid)
            {
                status = objBAL.AddProficiencyScore(formCollection, out isValidMsg);

                return Json(new { success = status, message = isValidMsg });
            }

            return Json(new { success = status, message = message });
        }

        [Audit]
        public ActionResult GetProficiencyTestScoreList(int? page, int? rows, string sidx, string sord)
        {
            try
            {
                long totalRecords = 0;
                IQualityMonitoringBAL objBAL = new QualityMonitoringBAL();

                using (CommonFunctions commonFunction = new CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new GridParams(page, rows, sidx, sord, Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                    {
                        return null;
                    }
                }

                var jsonData = new
                {
                    rows = objBAL.GetProficiencyTestScoreList(Request.Params["filters"], Convert.ToInt32(page) - 1, Convert.ToInt32(rows), sidx, sord, out totalRecords),
                    total = totalRecords <= Convert.ToInt32(rows) ? 1 : totalRecords / Convert.ToInt32(rows) + 1,
                    page = Convert.ToInt32(page),
                    records = totalRecords,
                };

                return Json(jsonData, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "GetProficiencyTestScoreList()");
                return null;
            }
        }

        [Audit]
        public ActionResult GetProficiencyTestScoreListDetails(int examId, int? page, int? rows, string sidx, string sord)
        {
            try
            {
                long totalRecords = 0;
                IQualityMonitoringBAL objBAL = new QualityMonitoringBAL();

                using (CommonFunctions commonFunction = new CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new GridParams(page, rows, sidx, sord, Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                    {
                        return null;
                    }
                }

                var jsonData = new
                {
                    rows = objBAL.GetProficiencyTestScoreListDetails(examId, Request.Params["filters"], Convert.ToInt32(page) - 1, Convert.ToInt32(rows), sidx, sord, out totalRecords),
                    total = totalRecords <= Convert.ToInt32(rows) ? 1 : totalRecords / Convert.ToInt32(rows) + 1,
                    page = Convert.ToInt32(page),
                    records = totalRecords,
                };

                return Json(jsonData, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "GetProficiencyTestScoreList()");
                return null;
            }
        }

        [HttpGet]
        public ActionResult ProficiencyTestScoreForm()
        {
            ProficiencyTestScoreModel model = new ProficiencyTestScoreModel();
            CommonFunctions objCommon = new CommonFunctions();

            List<SelectListItem> institutions = new List<SelectListItem>();
            institutions.Add(new SelectListItem() { Text = "IIT Bhubaneswar", Value = "1" });
            institutions.Add(new SelectListItem() { Text = "SVNIT Surat", Value = "2" });
            //institutions.Add(new SelectListItem() { Text = "IIT BHU", Value = "3" });
            //institutions.Add(new SelectListItem() { Text = "IIT Guwahati", Value = "4" });
            institutions.Insert(0, (new SelectListItem { Text = "Select Institution", Value = "-1", Selected = true }));
            model.INSTITUTION_LIST = institutions;

            List<SelectListItem> items = new List<SelectListItem>();
            items.Add(new SelectListItem() { Text = "NQM", Value = "I" });
            items.Add(new SelectListItem() { Text = "SQM", Value = "S" });
            items.Insert(0, (new SelectListItem { Text = "Select Monitor Type", Value = String.Empty, Selected = true }));
            model.NQM_SQM_LIST = items;

            model.STATE_LIST = objCommon.PopulateStates();

            return PartialView(model);
        }

        #region Added By Hrishikesh on 09-05-2023 To show Monitor List(Monitor's Exam) [cqc, cqcAdmin, sqc logins]
        //Added By Hrishikesh on 09-05-2023 To show Monitor List(Monitor's Exam) --start
        [HttpGet]
        [Audit]
        public ActionResult GetMonitorFilter()
        {
            CommonFunctions objCommon = new CommonFunctions();
            ProficiencyTestScoreModel model = new ProficiencyTestScoreModel();
            dbContext = new PMGSYEntities();
            //for seletect sqm or sqm type
            model.NQM_SQM_LIST = new List<SelectListItem>();


            if (PMGSYSession.Current.RoleCode == 8) //sqc=8 cqc=9 cqcAdmin=5
            {
                List<SelectListItem> item = new List<SelectListItem>();
                //item.Add(new SelectListItem() { Text = "NQM", Value = "I" });   //I:- NQM  and s:- SQM
                item.Add(new SelectListItem() { Text = "SQM", Value = "S", Selected = true });
                //item.Insert(0, (new SelectListItem() { Text = "Select Monitor Type", Value = string.Empty, Selected = true }));  //  Selected= true denotes - bydefault this will selected and shown in Dropdown
                model.NQM_SQM_LIST = item;
                int stateCode = PMGSYSession.Current.StateCode;
                string stateName = dbContext.MASTER_STATE.Where(x => x.MAST_STATE_CODE == stateCode).Select(y => y.MAST_STATE_NAME).FirstOrDefault();
                model.STATE_LIST = new List<SelectListItem>();
                model.STATE_LIST.Add(new SelectListItem() { Text = stateName, Value = stateCode.ToString(), Selected = true });

            }
            else
            {
                List<SelectListItem> item = new List<SelectListItem>();
                item.Add(new SelectListItem() { Text = "NQM", Value = "I" });   //I:- NQM  and s:- SQM
                item.Add(new SelectListItem() { Text = "SQM", Value = "S" });
                item.Insert(0, (new SelectListItem() { Text = "Select Monitor Type", Value = string.Empty, Selected = true }));  //  Selected= true denotes - bydefault this will selected and shown in Dropdown
                model.NQM_SQM_LIST = item;
                model.STATE_LIST = objCommon.PopulateStates();
            }

            return View(model);
        }//end GetMonitorFilter()

        //[HttpPost]
        [Audit]
        public ActionResult GetmonitorDetailsListJSON(string data, int? page, int? rows, string sidx, string sord)
        {
            try
            {
                long totalRecords = 0;
                IQualityMonitoringBAL objBAL = new QualityMonitoringBAL();

                using (CommonFunctions commonFunction = new CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new GridParams(page, rows, sidx, sord, Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                    {
                        return null;
                    }
                }

                var jsonData = new
                {
                    rows = objBAL.GetmonitorDetailsListJSONBAL(data, Request.Params["filters"], Convert.ToInt32(page) - 1, Convert.ToInt32(rows), sidx, sord, out totalRecords),
                    total = totalRecords <= Convert.ToInt32(rows) ? 1 : totalRecords / Convert.ToInt32(rows) + 1,
                    page = Convert.ToInt32(page),
                    records = totalRecords,
                };

                return Json(jsonData, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "GetmonitorDetailsListJSON()");
                return null;
            }
            //return Json(string.Empty); 
        }

        [HttpPost]
        public ActionResult CreateReportForMoniors(int[] submitarray)
        {

            ViewBag.AdminQmCodeList = submitarray; //submitarray;

            return PartialView();
           
        }
        //Added By Hrishikesh on 09-05-2023 To show Monitor List(Monitor's Exam) --end
        #endregion

        [HttpGet]
        public ActionResult DownloadTemplateForm()
        {
            CommonFunctions objCommon = new CommonFunctions();
            ProficiencyTestScoreModel model = new ProficiencyTestScoreModel();
            model.NQM_SQM_LIST = new List<SelectListItem>();

            List<SelectListItem> items = new List<SelectListItem>();
            items.Add(new SelectListItem() { Text = "NQM", Value = "I" });
            items.Add(new SelectListItem() { Text = "SQM", Value = "S" });
            items.Insert(0, (new SelectListItem { Text = "Select Monitor Type", Value = String.Empty, Selected = true }));
            model.NQM_SQM_LIST = items;

            model.STATE_LIST = objCommon.PopulateStates();

            return PartialView(model);
        }

        // Create and download excel

        [HttpGet]
        public void DownloadTemplateSheet(string mergeValue)
        {
            string monitorType = mergeValue.Split('$')[0];
            int stateCode = Convert.ToInt32(mergeValue.Split('$')[1]);
            string mType = monitorType == "I" ? "NQM" : "SQM";
            string fileName = mType + "_" + stateCode + "_Monitors_List.xlsx";
            List<ProficiencyTestTemplateModel> dataList = new List<ProficiencyTestTemplateModel>();
            //List<GetMonitorListTemplateModel> dataList = new List<GetMonitorListTemplateModel>();
            CommonFunctions objCommon = new CommonFunctions();
            int i = 2;

            using (ExcelEngine excelEngine = new ExcelEngine())
            {
                IApplication application = excelEngine.Excel;
                application.DefaultVersion = ExcelVersion.Xlsx;

                //Create a workbook
                IWorkbook workbook = application.Workbooks.Create(1);
                IWorksheet worksheet = workbook.Worksheets[0];

                //Add a picture
                //IPictureShape shape = worksheet.Pictures.AddPicture(1, 1, Server.MapPath("App_Data/AdventureCycles-Logo.png"), 20, 20);

                if (monitorType == "S")
                    dataList = objCommon.PopulateMonitorsListForProficiencyTestScore("S", stateCode);
                else if (monitorType == "I")
                    dataList = objCommon.PopulateMonitorsListForProficiencyTestScore("I", 0);

                //Disable gridlines in the worksheet
                worksheet.IsGridLinesVisible = false;

                //Enter values to the cells from A3 to A5
                worksheet.Range["A1"].Text = "Sr. No";
                worksheet.Range["B1"].Text = "Monitor Name";
                worksheet.Range["C1"].Text = "Email ID";
                worksheet.Range["D1"].Text = "Mobile Number";
                worksheet.Range["E1"].Text = "Marks";
                worksheet.Range["F1"].Text = "Monitor Status";
                worksheet.Range["G1"].Text = "AdminCode";

                worksheet.Range["A1"].CellStyle.Font.Bold = true;
                worksheet.Range["A1"].CellStyle.Font.Size = 15;
                worksheet.Range["B1"].CellStyle.Font.Bold = true;
                worksheet.Range["B1"].CellStyle.Font.Size = 15;
                worksheet.Range["C1"].CellStyle.Font.Bold = true;
                worksheet.Range["C1"].CellStyle.Font.Size = 15;
                worksheet.Range["D1"].CellStyle.Font.Bold = true;
                worksheet.Range["D1"].CellStyle.Font.Size = 15;
                worksheet.Range["E1"].CellStyle.Font.Bold = true;
                worksheet.Range["E1"].CellStyle.Font.Size = 15;
                worksheet.Range["F1"].CellStyle.Font.Bold = true;
                worksheet.Range["F1"].CellStyle.Font.Size = 15;
                worksheet.Range["G1"].CellStyle.Font.Bold = true;
                worksheet.Range["G1"].CellStyle.Font.Size = 15;

                /*IName status1 = workbook.Names.Add("PRS");
                status1.RefersToRange = worksheet["D2:D500"];
                status1.Value = "Sheet1!$D$2:$D$500";

                IName status2 = workbook.Names.Add("ABS");
                status2.RefersToRange = worksheet["D2:D500"];
                status2.Value = "Sheet1!$D$2:$D$500";

                IName status3 = workbook.Names.Add("NRT");
                status3.RefersToRange = worksheet["D2:D500"];
                status3.Value = "Sheet1!$D$2:$D$500";

                IName status4 = workbook.Names.Add("NA");
                status4.RefersToRange = worksheet["D2:D500"];
                status4.Value = "Sheet1!$D$2:$D$500";*/

                //Defining name for range of cells
                /*IName name1 = workbook.Names.Add("PRS");
                name1.RefersToRange = worksheet["D1:D500"];
                IName name2 = workbook.Names.Add("ABS");
                name2.RefersToRange = worksheet["D1:D500"];
                IName name3 = workbook.Names.Add("NRT");
                name3.RefersToRange = worksheet["D1:D500"];
                IName name4 = workbook.Names.Add("NA");
                name4.RefersToRange = worksheet["D1:D500"];*/

                IName name1 = workbook.Names.Add("Status");
                name1.RefersToRange = worksheet.Range["H1:H4"];

                worksheet.Range["H1"].Value = "P (Present)";
                worksheet.Range["H2"].Value = "AB (Absent)";
                worksheet.Range["H3"].Value = "NRT (Not registered)";
                worksheet.Range["H4"].Value = "NA (Not Applicable)";


                IDataValidation validation1 = worksheet.Range["F2:F1000"].DataValidation;
                validation1.AllowType = ExcelDataType.User;
                validation1.FirstFormula = "=Status";

                foreach (var item in dataList)
                {
                    worksheet.Range["A" + i].Text = item.ID.ToString();
                    worksheet.Range["B" + i].Text = item.MONITOR_NAME;
                    worksheet.Range["C" + i].Text = item.EMAIL;
                    worksheet.Range["D" + i].Text = item.MOBILE_NUMBER;
                    worksheet.Range["E" + i].Text = item.MARKS.ToString();
                    worksheet.Range["F" + i].Text = item.MONITOR_STATUS;
                    worksheet.Range["G" + i].Text = item.ADMIN_QM_CODE.ToString();
                    i++;
                }

                worksheet.Range["A1:F1000"].CellStyle.Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.Thin;
                worksheet.Range["A1:F1000"].CellStyle.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Thin;
                worksheet.Range["A1:F1000"].CellStyle.Borders[ExcelBordersIndex.EdgeLeft].LineStyle = ExcelLineStyle.Thin;
                worksheet.Range["A1:F1000"].CellStyle.Borders[ExcelBordersIndex.EdgeRight].LineStyle = ExcelLineStyle.Thin;

                worksheet.Range["A1:F1000"].CellStyle.Borders[ExcelBordersIndex.EdgeTop].Color = ExcelKnownColors.Black;
                worksheet.Range["A1:F1000"].CellStyle.Borders[ExcelBordersIndex.EdgeBottom].Color = ExcelKnownColors.Black;
                worksheet.Range["A1:F1000"].CellStyle.Borders[ExcelBordersIndex.EdgeLeft].Color = ExcelKnownColors.Black;
                worksheet.Range["A1:F1000"].CellStyle.Borders[ExcelBordersIndex.EdgeRight].Color = ExcelKnownColors.Black;


                worksheet.Range["A1:F1"].CellStyle.Color = Color.FromArgb(245, 245, 220);
                worksheet.Range["B2:D1000"].CellStyle.Color = Color.FromArgb(223, 211, 195);

                worksheet.Range["A1:A1000"].ColumnWidth = 10;
                worksheet.Range["B1:B1000"].ColumnWidth = 50;
                worksheet.Range["C1:C1000"].ColumnWidth = 50;
                worksheet.Range["D1:D1000"].ColumnWidth = 25;
                worksheet.Range["E1:E1000"].ColumnWidth = 20;
                worksheet.Range["F1:F1000"].ColumnWidth = 25;
                worksheet.Range["G1:G1000"].ColumnWidth = 10;


                worksheet.Range["A1:D1000"].CellStyle.HorizontalAlignment = ExcelHAlign.HAlignCenter;
                worksheet.Range["G1:H1000"].CellStyle.HorizontalAlignment = ExcelHAlign.HAlignCenter;

                worksheet.Range["G1:G1000"].CellStyle.Font.Color = ExcelKnownColors.White;
                worksheet.Range["G1:G1000"].IgnoreErrorOptions = ExcelIgnoreError.NumberAsText;
                worksheet.Range["H1:H1000"].CellStyle.Font.Color = ExcelKnownColors.White;

                workbook.SaveAs(fileName, HttpContext.ApplicationInstance.Response, ExcelDownloadType.Open);
            }
        }

        [HttpPost]
        public ActionResult GetUploadDetails(FormCollection formCollection)
        {
            bool success = false;
            bool flg = false;
            string message = string.Empty;
            IQualityMonitoringBAL objBAL = new QualityMonitoringBAL();
            HttpPostedFileBase file = Request.Files[0];
            dbContext = new PMGSYEntities();
            string date, institutionName;

            DateTime examDate = Convert.ToDateTime(formCollection["DATE_OF_EXAM"]);
            string instName = formCollection["INSTITUTION"] == "1" ? "IIT Bhubaneswar" : formCollection["INSTITUTION"] == "2" ? "SVNIT Surat" : formCollection["INSTITUTION"] == "3" ? "IIT BHU" : formCollection["INSTITUTION"] == "4" ? "IIT Guwahati" : "--";
            string monitorType = formCollection["NQM_SQM"];
            int stateCode = Convert.ToInt32(formCollection["STATE_CODE"]);

            //if (dbContext.QM_PROFICIENCY_TEST_MASTER.Where(x => x.DATE_OF_EXAM == examDate && x.INSTITUTE_NAME == instName && x.MONITOR_TYPE == monitorType).Any())
            //    return Json(new { success = false, file = false, message = "Data for " + instName + " for date " + examDate.ToShortDateString() + " already exists." });

            //if (dbContext.QM_PROFICIENCY_TEST_MASTER.Where(x => x.DATE_OF_EXAM == examDate && x.INSTITUTE_NAME == instName && x.MONITOR_TYPE == monitorType && x.STATE_CODE == stateCode).Any())
            //    return Json(new { success = false, file = false, message = "Data for " + instName + " for date " + examDate.ToShortDateString() + " already exists." });

            //if (dbContext.QM_PROFICIENCY_TEST_MASTER.Where(x => x.DATE_OF_EXAM >= examDate && x.INSTITUTE_NAME == instName && x.MONITOR_TYPE == monitorType && x.IS_FINALIZED == "Y").Any())
            //{
            //    date = dbContext.QM_PROFICIENCY_TEST_MASTER.Where(x => x.DATE_OF_EXAM >= examDate && x.INSTITUTE_NAME == instName && x.MONITOR_TYPE == monitorType && x.IS_FINALIZED == "Y").Select(y => y.DATE_OF_EXAM).FirstOrDefault().ToShortDateString();
            //    institutionName = dbContext.QM_PROFICIENCY_TEST_MASTER.Where(x => x.DATE_OF_EXAM >= examDate && x.INSTITUTE_NAME == instName && x.MONITOR_TYPE == monitorType && x.IS_FINALIZED == "Y").Select(y => y.INSTITUTE_NAME).FirstOrDefault();
            //    return Json(new { success = false, file = false, message = "Data cannot be entered before date : " + date + " for " + institutionName + "." });
            //}


            //if (dbContext.QM_PROFICIENCY_TEST_MASTER.Where(x => x.DATE_OF_EXAM >= examDate && x.INSTITUTE_NAME == instName && x.MONITOR_TYPE == monitorType && x.STATE_CODE == stateCode && x.IS_FINALIZED == "Y").Any())
            //{
            //    date = dbContext.QM_PROFICIENCY_TEST_MASTER.Where(x => x.DATE_OF_EXAM >= examDate && x.INSTITUTE_NAME == instName && x.MONITOR_TYPE == monitorType && x.STATE_CODE == stateCode && x.IS_FINALIZED == "Y").Select(y => y.DATE_OF_EXAM).FirstOrDefault().ToShortDateString();
            //    institutionName = dbContext.QM_PROFICIENCY_TEST_MASTER.Where(x => x.DATE_OF_EXAM >= examDate && x.INSTITUTE_NAME == instName && x.MONITOR_TYPE == monitorType && x.STATE_CODE == stateCode && x.IS_FINALIZED == "Y").Select(y => y.INSTITUTE_NAME).FirstOrDefault();
            //    return Json(new { success = false, file = false, message = "Data cannot be entered before date : " + date + " for " + institutionName + "." });
            //}

            if (monitorType == String.Empty)
            {
                return Json(new { success = false, file = false, message = "Please select Monitor Type." });
            }
            if (monitorType == "S" && stateCode == 0)
            {
                return Json(new { success = false, file = false, message = "Please select State." });
            }

            try
            {
                if (Request.Files.Count == 0)
                {
                    return Json(new { success = false, file = false, message = "No file selected. Please select file" });
                }
                else
                {
                    int maxSize = 1024 * 1024 * 4;

                    if (file.FileName.Substring(file.FileName.LastIndexOf('.') + 1).Trim().ToLower() != "xlsx")
                        return Json(new { success = false, file = false, message = "Please upload only downloaded template excel file." });

                    if (file.ContentLength > maxSize)
                        return Json(new { success = false, file = false, message = "Invalid file size. Please upload file upto 4 MB." });
                }

                flg = objBAL.GetUploadDetails(formCollection, file, ref message);

                if (flg)
                {
                    return Json(new { message = "Data uploaded successfully.", success = true }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { message = message, success = false }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "QualityMonitoring.GetUploadDetails");
            }
            return Json(new { success = flg, message = message }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult EditProficiencyScore(int scoreId)
        {
            ProficiencyTestScoreModel model = new ProficiencyTestScoreModel();
            QM_PROFICIENCY_TEST_MASTER masterModel = new QM_PROFICIENCY_TEST_MASTER();
            QM_PROFICIENCY_TEST_SCORE scoreModel = new QM_PROFICIENCY_TEST_SCORE();
            ADMIN_QUALITY_MONITORS adminModel = new ADMIN_QUALITY_MONITORS();
            CommonFunctions objCommon = new CommonFunctions();
            dbContext = new PMGSYEntities();

            scoreModel = dbContext.QM_PROFICIENCY_TEST_SCORE.Where(x => x.ID == scoreId).FirstOrDefault();
            masterModel = dbContext.QM_PROFICIENCY_TEST_MASTER.Where(x => x.EXAM_ID == scoreModel.EXAM_ID).FirstOrDefault();
            adminModel = dbContext.ADMIN_QUALITY_MONITORS.Where(x => x.ADMIN_QM_CODE == scoreModel.ADMIN_QM_CODE).FirstOrDefault();

            model.ID = scoreId;
            model.EXAM_ID = masterModel.EXAM_ID;
            model.INSTITUTION = masterModel.INSTITUTE_NAME;
            model.DATE_OF_EXAM = masterModel.DATE_OF_EXAM;
            model.NQM_SQM = masterModel.MONITOR_TYPE == "I" ? "NQM" : "SQM";
            model.PERCENTAGE = scoreModel.MARKS;
            model.MONITOR_STATUS = scoreModel.EXAM_STATUS;
            model.MONITOR_NAME = (adminModel.ADMIN_QM_FNAME == null || adminModel.ADMIN_QM_FNAME == string.Empty ? "" : adminModel.ADMIN_QM_FNAME) + " " + (adminModel.ADMIN_QM_MNAME == null || adminModel.ADMIN_QM_MNAME == string.Empty ? " " : adminModel.ADMIN_QM_MNAME) + " " + (adminModel.ADMIN_QM_LNAME == null || adminModel.ADMIN_QM_LNAME == string.Empty ? " " : adminModel.ADMIN_QM_LNAME);

            List<SelectListItem> examStatus = new List<SelectListItem>();
            examStatus.Add(new SelectListItem() { Text = "P", Value = "P" });
            examStatus.Add(new SelectListItem() { Text = "AB", Value = "AB" });
            examStatus.Add(new SelectListItem() { Text = "NRT", Value = "NRT" });
            examStatus.Add(new SelectListItem() { Text = "NA", Value = "NA" });
            examStatus.Insert(0, (new SelectListItem { Text = "Select Exam Status", Value = String.Empty, Selected = true }));
            model.EXAM_STATUS_LIST = examStatus;

            return PartialView(model);
        }

        [HttpPost]
        public ActionResult EditProficiencyTestScore(FormCollection formCollection)
        {
            bool status = false;
            string isValidMsg = String.Empty;
            String message = "Details not added";
            IQualityMonitoringBAL objBAL = new QualityMonitoringBAL();
            QM_PROFICIENCY_TEST_SCORE model = new QM_PROFICIENCY_TEST_SCORE();
            int id = Convert.ToInt32(formCollection["ID"]);
            string monitorStatus = formCollection["MONITOR_STATUS"];
            dbContext = new PMGSYEntities();

            if (monitorStatus == "")
                return Json(new { success = false, message = "Please select exam status." });

            decimal marks;
            string val = formCollection["PERCENTAGE"];
            if (val == "")
                marks = -1;
            else
                marks = Convert.ToDecimal(formCollection["PERCENTAGE"]);

            Regex re = new Regex(@"(^([0-5]?[0-9]|60)$)");
            bool regVal = re.IsMatch(Convert.ToString(marks));

            try
            {
                if (monitorStatus == "P" && marks == -1)
                {
                    return Json(new { success = false, message = "Please enter marks." });
                }

                if (monitorStatus == "P" && !regVal)
                    return Json(new { success = false, message = "Please enter marks between 0 to 60 (decimals not allowed)." });


                if (ModelState.IsValid)
                {
                    status = objBAL.EditProficiencyTestScore(formCollection, out isValidMsg);

                    return Json(new { success = status, message = isValidMsg });
                }

                return Json(new { success = status, message = message });
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "EditProficiencyTestScore()");
                return Json(new { success = status, message = message });
            }
        }

        public ActionResult DeleteProficiencyScore(int idtemp)
        {
            try
            {
                PMGSYEntities dbContext = new PMGSYEntities();
                List<QM_PROFICIENCY_TEST_SCORE> model = new List<QM_PROFICIENCY_TEST_SCORE>();
                QM_PROFICIENCY_TEST_SCORE testModel = new QM_PROFICIENCY_TEST_SCORE();
                QM_PROFICIENCY_TEST_MASTER masterModel = new QM_PROFICIENCY_TEST_MASTER();
                string message = string.Empty;
                bool status = false;
                List<int> adminQmCode = new List<int>();

                model = dbContext.QM_PROFICIENCY_TEST_SCORE.Where(x => x.EXAM_ID == idtemp).ToList();
                masterModel = dbContext.QM_PROFICIENCY_TEST_MASTER.Where(s => s.EXAM_ID == idtemp).FirstOrDefault();
                string fileName = masterModel.UPLOADED_FILE_NAME;
                string filePath = masterModel.UPLOADED_FILE_PATH;
                string fileYear = fileName.Split('_')[1];

                foreach (var item in model)
                {
                    adminQmCode.Add(item.ADMIN_QM_CODE);
                }

                foreach (var item in model)
                {
                    dbContext.QM_PROFICIENCY_TEST_SCORE.Remove(dbContext.QM_PROFICIENCY_TEST_SCORE.Where(s => s.ID == item.ID).FirstOrDefault());
                    dbContext.SaveChanges();
                }

                foreach (var item in adminQmCode)
                {
                    if (dbContext.QM_PROFICIENCY_TEST_SCORE.Where(x => x.ADMIN_QM_CODE == item).Any())
                    {
                        testModel = dbContext.QM_PROFICIENCY_TEST_SCORE.Where(x => x.ADMIN_QM_CODE == item).OrderByDescending(y => y.ID).FirstOrDefault();
                        testModel.IS_LATEST = "Y";
                        dbContext.Entry(testModel).State = System.Data.Entity.EntityState.Modified;
                        dbContext.SaveChanges();
                    }
                }

                dbContext.QM_PROFICIENCY_TEST_MASTER.Remove(dbContext.QM_PROFICIENCY_TEST_MASTER.Where(s => s.EXAM_ID == idtemp).FirstOrDefault());
                dbContext.SaveChanges();

                var YearDictionary = Path.Combine(filePath, fileYear);

                FileInfo file = new FileInfo(Path.Combine(YearDictionary, fileName));
                if (file.Exists)
                {
                    file.Delete();
                }

                message = "Data deleted successfully.";
                status = true;

                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "DeleteProficiencyScore()");
                return null;
            }
        }

        public ActionResult FinalizeProficiencyScore(int idtemp)
        {
            try
            {
                PMGSYEntities dbContext = new PMGSYEntities();
                QM_PROFICIENCY_TEST_MASTER model = new QM_PROFICIENCY_TEST_MASTER();
                List<QM_PROFICIENCY_TEST_SCORE> scoreModelList = new List<QM_PROFICIENCY_TEST_SCORE>();
                QM_PROFICIENCY_TEST_SCORE scoreModel = new QM_PROFICIENCY_TEST_SCORE();
                string message = string.Empty;
                bool status = false;

                model = dbContext.QM_PROFICIENCY_TEST_MASTER.Where(x => x.EXAM_ID == idtemp).FirstOrDefault();


                //if (model.MONITOR_TYPE == "I" && (dbContext.QM_PROFICIENCY_TEST_MASTER.Where(x => x.DATE_OF_EXAM < model.DATE_OF_EXAM && x.INSTITUTE_NAME == model.INSTITUTE_NAME && x.IS_FINALIZED == "N" && x.MONITOR_TYPE == "I" && x.EXAM_ID != model.EXAM_ID).Any()))
                //{
                //    message = "Please finalize previous data first.";
                //    status = false;
                //}
                //else if (model.MONITOR_TYPE == "S" && (dbContext.QM_PROFICIENCY_TEST_MASTER.Where(x => x.DATE_OF_EXAM < model.DATE_OF_EXAM && x.INSTITUTE_NAME == model.INSTITUTE_NAME && x.STATE_CODE == model.STATE_CODE && x.IS_FINALIZED == "N" && x.MONITOR_TYPE == "S" && x.EXAM_ID != model.EXAM_ID).Any()))
                //{
                //    message = "Please finalize previous data first.";
                //    status = false;
                //}
                //else
                //{
                    if (dbContext.QM_PROFICIENCY_TEST_SCORE.Where(x => x.EXAM_ID == model.EXAM_ID && x.EXAM_STATUS == "P").Any())
                    {
                        scoreModelList = dbContext.QM_PROFICIENCY_TEST_SCORE.Where(x => x.EXAM_ID == model.EXAM_ID && x.EXAM_STATUS == "P").ToList();

                        foreach (var item in scoreModelList)
                        {
                            if (item.ADMIN_QM_CODE == (dbContext.QM_PROFICIENCY_TEST_SCORE.Where(x => x.EXAM_ID != item.EXAM_ID && x.ADMIN_QM_CODE == item.ADMIN_QM_CODE && x.IS_LATEST == "Y").Select(y => y.ADMIN_QM_CODE).FirstOrDefault()))
                            {
                                scoreModel = dbContext.QM_PROFICIENCY_TEST_SCORE.Where(x => x.EXAM_ID != item.EXAM_ID && x.ADMIN_QM_CODE == item.ADMIN_QM_CODE && x.IS_LATEST == "Y").FirstOrDefault();
                                scoreModel.IS_LATEST = "N";
                                dbContext.Entry(scoreModel).State = System.Data.Entity.EntityState.Modified;
                                dbContext.SaveChanges();
                            }
                        }

                        foreach (var item in scoreModelList)
                        {
                            item.IS_LATEST = "Y";
                            dbContext.Entry(item).State = System.Data.Entity.EntityState.Modified;
                            dbContext.SaveChanges();
                        }
                    }

                    model.IS_FINALIZED = "Y";
                    model.DATE_OF_FINALIZATION = DateTime.Now;
                    dbContext.Entry(model).State = System.Data.Entity.EntityState.Modified;
                    dbContext.SaveChanges();
                    message = "Record finalized successfully.";
                    status = true;
                //}

                //scoreModelList = dbContext.QM_PROFICIENCY_TEST_SCORE.Where(x => x.EXAM_ID == idtemp).ToList();

                //foreach (var item in scoreModelList)
                //{
                //    if(item.EXAM_STATUS == "P")
                //    {
                //        if(dbContext.QM_PROFICIENCY_TEST_SCORE.Where(x => x.ADMIN_QM_CODE == item.ADMIN_QM_CODE && x.EXAM_STATUS == "P" && x.IS_LATEST == "Y").Any())
                //        {
                //            scoreModel = dbContext.QM_PROFICIENCY_TEST_SCORE.Where(x => x.ADMIN_QM_CODE == item.ADMIN_QM_CODE && x.EXAM_STATUS == "P" && x.IS_LATEST == "Y").OrderByDescending(y => y.ID).FirstOrDefault();
                //            scoreModel.IS_LATEST = "N";
                //            dbContext.Entry(scoreModel).State = System.Data.Entity.EntityState.Modified;
                //            dbContext.SaveChanges();

                //            item.IS_LATEST = "Y";
                //            dbContext.Entry(item).State = System.Data.Entity.EntityState.Modified;
                //            dbContext.SaveChanges();
                //        }
                //    }
                //}



                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "FinalizeProficiencyScore()");
                return null;
            }
        }

        public ActionResult MonitorProficiencyTestScoreCQC()
        {
            CommonFunctions objCommon = new CommonFunctions();
            ProficiencyTestScoreModel model = new ProficiencyTestScoreModel();

            List<SelectListItem> items = new List<SelectListItem>();
            items.Add(new SelectListItem() { Text = "NQM", Value = "I" });
            items.Add(new SelectListItem() { Text = "SQM", Value = "S" });
            items.Insert(0, (new SelectListItem { Text = "All Monitors", Value = "A", Selected = true }));

            model.NQM_SQM_LIST = items;

            return View(model);
        }

        [Audit]
        public ActionResult GetProficiencyTestScoreListCQC(string monitorType, int? page, int? rows, string sidx, string sord)
        {
            try
            {
                long totalRecords = 0;
                IQualityMonitoringBAL objBAL = new QualityMonitoringBAL();

                using (CommonFunctions commonFunction = new CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new GridParams(page, rows, sidx, sord, Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                    {
                        return null;
                    }
                }

                var jsonData = new
                {
                    rows = objBAL.GetProficiencyTestScoreListCQC(Request.Params["filters"], monitorType, Convert.ToInt32(page) - 1, Convert.ToInt32(rows), sidx, sord, out totalRecords),
                    total = totalRecords <= Convert.ToInt32(rows) ? 1 : totalRecords / Convert.ToInt32(rows) + 1,
                    page = Convert.ToInt32(page),
                    records = totalRecords,
                };

                return Json(jsonData, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "GetProficiencyTestScoreListCQC()");
                return null;
            }
        }

        public ActionResult EditProficiencyScoreCQC(int idtemp)
        {
            try
            {
                PMGSYEntities dbContext = new PMGSYEntities();
                QM_PROFICIENCY_TEST_SCORE model = new QM_PROFICIENCY_TEST_SCORE();
                ProficiencyTestScoreModel viewModel = new ProficiencyTestScoreModel();
                ADMIN_QUALITY_MONITORS adminMonitor = new ADMIN_QUALITY_MONITORS();
                string message = string.Empty;
                bool status = false;

                model = dbContext.QM_PROFICIENCY_TEST_SCORE.Where(x => x.ID == idtemp).FirstOrDefault();
                adminMonitor = dbContext.ADMIN_QUALITY_MONITORS.Where(x => x.ADMIN_QM_CODE == model.ADMIN_QM_CODE).FirstOrDefault();

                viewModel.ID = model.ID;
                viewModel.MONITOR_NAME = (adminMonitor.ADMIN_QM_FNAME == null || adminMonitor.ADMIN_QM_FNAME == string.Empty ? "" : adminMonitor.ADMIN_QM_FNAME) + " " + (adminMonitor.ADMIN_QM_MNAME == null || adminMonitor.ADMIN_QM_MNAME == string.Empty ? " " : adminMonitor.ADMIN_QM_MNAME) + " " + (adminMonitor.ADMIN_QM_LNAME == null || adminMonitor.ADMIN_QM_LNAME == string.Empty ? " " : adminMonitor.ADMIN_QM_LNAME);
                viewModel.DATE_OF_EXAM = dbContext.QM_PROFICIENCY_TEST_MASTER.Where(x => x.EXAM_ID == model.EXAM_ID).Select(y => y.DATE_OF_EXAM).FirstOrDefault();
                viewModel.PERCENTAGE = model.MARKS == null ? 0 : model.MARKS;
                viewModel.CQC_REMARK = model.CQC_REMARK == null ? "" : model.CQC_REMARK;

                return PartialView(viewModel);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "EditProficiencyScoreCQC()");
                return null;
            }
        }

        [HttpPost]
        public ActionResult EditProficiencyScoreDetails(FormCollection formCollection)
        {
            bool status = false;
            string isValidMsg = String.Empty;
            String message = "Details not updated";
            IQualityMonitoringBAL objBAL = new QualityMonitoringBAL();
            dbContext = new PMGSYEntities();

            try
            {
                if (ModelState.IsValid)
                {
                    status = objBAL.EditProficiencyScoreDetails(formCollection, out isValidMsg);

                    return Json(new { success = status, message = isValidMsg });
                }

                return Json(new { success = status, message = message });
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "EditProficiencyScoreDetails()");
                return Json(new { success = status, message = message });
            }
        }

        //public ActionResult FinalizeProficiencyScoreCQC(int idtemp)
        //{
        //    try
        //    {
        //        PMGSYEntities dbContext = new PMGSYEntities();
        //        QM_PROFICIENCY_TEST_SCORE model = new QM_PROFICIENCY_TEST_SCORE();
        //        string message = string.Empty;
        //        bool status = false;

        //        model = dbContext.QM_PROFICIENCY_TEST_SCORE.Where(x => x.ID == idtemp).FirstOrDefault();
        //        //   model.CQC_IS_FINALIZED = "Y";
        //        // model.CQC_FINALIZED_DATE = DateTime.Now;
        //        model.CQC_USER_ID = PMGSYSession.Current.UserId;
        //        model.CQC_IP_ADDRESS = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
        //        dbContext.Entry(model).State = System.Data.Entity.EntityState.Modified;
        //        dbContext.SaveChanges();

        //        message = "Record finalized successfully.";
        //        status = true;

        //        return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
        //    }
        //    catch (Exception ex)
        //    {
        //        ErrorLog.LogError(ex, "FinalizeProficiencyScoreCQC()");
        //        return null;
        //    }
        //}

        public ActionResult TestScoreReport()
        {
            ProficiencyTestScoreModel model = new ProficiencyTestScoreModel();
            CommonFunctions objCommon = new CommonFunctions();

            List<SelectListItem> institutions = new List<SelectListItem>();
            institutions.Add(new SelectListItem() { Text = "IIT Bhubaneswar", Value = "1" });
            institutions.Add(new SelectListItem() { Text = "SVNIT Surat", Value = "2" });
            //institutions.Add(new SelectListItem() { Text = "IIT BHU", Value = "3" });
            //institutions.Add(new SelectListItem() { Text = "IIT Guwahati", Value = "4" });
            institutions.Insert(0, (new SelectListItem { Text = "All Institutions", Value = "0", Selected = true }));
            model.INSTITUTION_LIST = institutions;

            List<SelectListItem> items = new List<SelectListItem>();
            items.Add(new SelectListItem() { Text = "NQM", Value = "I" });
            items.Add(new SelectListItem() { Text = "SQM", Value = "S" });
            items.Insert(0, (new SelectListItem { Text = "All Monitors", Value = "A", Selected = true }));
            model.NQM_SQM_LIST = items;

            model.STATE_LIST = objCommon.PopulateStates(false);

            return View(model);
        }

        public ActionResult ViewTestScoreReport(string combinedParam)
        {
            string institutionName = combinedParam.Split('$')[0];
            
            ViewBag.institutionName = institutionName == "1" ? "IIT Bhubaneswar" : institutionName == "2" ? "SVNIT Surat" : institutionName == "3" ? "IIT BHU" : institutionName == "4" ? "IIT Guwahati" : "A";
            @ViewBag.monitorType = combinedParam.Split('$')[1];
            @ViewBag.stateCode = Convert.ToInt32(combinedParam.Split('$')[2]);

            return View();
        }

        [HttpPost]
        public ActionResult PopulateMonitorsList(string typeAndState)
        {
            try
            {
                CommonFunctions objCommon = new CommonFunctions();
                List<SelectListItem> nqmList = new List<SelectListItem>();
                List<SelectListItem> sqmList = new List<SelectListItem>();
                List<SelectListItem> monitorNamesList = new List<SelectListItem>();

                string monitorType = typeAndState.Split('$')[0];
                int stateCode = Convert.ToInt32(typeAndState.Split('$')[1]);

                if (monitorType == "A")
                {
                    nqmList = objCommon.PopulateMonitorsForScheduleCreation("false", "I", 0, 0, 0);
                    sqmList = objCommon.PopulateMonitorsForScheduleCreation("false", "S", 0, 0, 0);
                    sqmList.RemoveAt(0);

                    monitorNamesList = nqmList.Concat(sqmList).ToList();
                }

                if (monitorType == "I")
                    monitorNamesList = objCommon.PopulateMonitorsForScheduleCreation("false", "I", 0, 0, 0);

                if (monitorType == "S")
                    monitorNamesList = objCommon.PopulateMonitorsForScheduleCreation("false", "S", stateCode, 0, 0);

                return Json(monitorNamesList);
            }
            catch
            {
                return Json(new { string.Empty });
            }
        }

        public ActionResult TestScoreDetailedReport()
        {
            ProficiencyTestScoreModel model = new ProficiencyTestScoreModel();
            CommonFunctions objCommon = new CommonFunctions();
            List<SelectListItem> nqmList = new List<SelectListItem>();
            List<SelectListItem> sqmList = new List<SelectListItem>();
            List<SelectListItem> items = new List<SelectListItem>();
            List<SelectListItem> stateList = new List<SelectListItem>();
            dbContext = new PMGSYEntities();

            nqmList = objCommon.PopulateMonitorsForScheduleCreation("false", "I", 0, 0, 0);
            sqmList = objCommon.PopulateMonitorsForScheduleCreation("false", "S", 0, 0, 0);

            if (PMGSYSession.Current.RoleCode == 8 || PMGSYSession.Current.RoleCode == 48 || PMGSYSession.Current.RoleCode == 69)
            {
                items.Add(new SelectListItem() { Text = "SQM", Value = "S" });
                model.NQM_SQM_LIST = items;

                model.MONITORS_LIST = sqmList;

                string stateName = dbContext.MASTER_STATE.Where(x => x.MAST_STATE_CODE == PMGSYSession.Current.StateCode).Select(y => y.MAST_STATE_NAME).FirstOrDefault();
                stateList.Add(new SelectListItem() { Text = stateName, Value = PMGSYSession.Current.StateCode.ToString() });
                model.STATE_LIST = stateList;
            }
            else
            {
                items.Add(new SelectListItem() { Text = "NQM", Value = "I" });
                items.Add(new SelectListItem() { Text = "SQM", Value = "S" });
                items.Insert(0, (new SelectListItem { Text = "All Monitors", Value = "A", Selected = true }));
                model.NQM_SQM_LIST = items;

                sqmList.RemoveAt(0);
                model.MONITORS_LIST = nqmList.Concat(sqmList).ToList();

                model.STATE_LIST = objCommon.PopulateStates(false);
            }



            return View(model);
        }

        public ActionResult ViewTestScoreDetailedReport(string mNameMTypeState)
        {
            @ViewBag.adminQmCode = Convert.ToInt32(mNameMTypeState.Split('$')[0]);
            @ViewBag.monitorType = mNameMTypeState.Split('$')[1];
            @ViewBag.stateCode = Convert.ToInt32(mNameMTypeState.Split('$')[2]);

            return View();
        }

        #endregion


        public ActionResult InProgressPriorityWorkView()
        {

            try
            {
                CommonFunctions objCommon = new CommonFunctions();
                InProgressPriorityWorkModel modelView = new InProgressPriorityWorkModel();

                modelView.lstStates = objCommon.PopulateStates(true);
                modelView.lstDistricts = new List<SelectListItem>();
                modelView.lstDistricts.Insert(0, new SelectListItem() { Text = "Select District", Value = "0" });
                //     uploadQCR.lstDistricts = objCommon.PopulateDistrict(, false);

                //modelView.lstScheme.Insert(0, new SelectListItem() { Text = "Select scheme", Value = "1" });
                return View(modelView);

            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "QCRReportView()");
                return null;
            }
        }

        public ActionResult ViewInProgressPriorityWorkReport()
        {
            //int stateCode, districtCode, year, scheme;

            if (!string.IsNullOrEmpty(Request.Params["stateCode"]))
            {
                ViewBag.stateCode = Convert.ToInt32(Request.Params["stateCode"]);
            }

            if (!string.IsNullOrEmpty(Request.Params["districtCode"]))
            {
                ViewBag.districtCode = (Convert.ToInt32(Request.Params["districtCode"])) == -1 ? 0 : Convert.ToInt32(Request.Params["districtCode"]);
            }

            return View();
        }

        public ActionResult CompletedPriorityWorkView()
        {

            try
            {
                CommonFunctions objCommon = new CommonFunctions();
                InProgressPriorityWorkModel modelView = new InProgressPriorityWorkModel();

                modelView.lstStates = objCommon.PopulateStates(true);
                modelView.lstDistricts = new List<SelectListItem>();
                modelView.lstDistricts.Insert(0, new SelectListItem() { Text = "Select District", Value = "0" });
                //     uploadQCR.lstDistricts = objCommon.PopulateDistrict(, false);

                //modelView.lstScheme.Insert(0, new SelectListItem() { Text = "Select scheme", Value = "1" });
                return View(modelView);

            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "QCRReportView()");
                return null;
            }
        }

        public ActionResult ViewCompletedPriorityWorkReport()
        {
            //int stateCode, districtCode, year, scheme;

            if (!string.IsNullOrEmpty(Request.Params["stateCode"]))
            {
                ViewBag.stateCode = Convert.ToInt32(Request.Params["stateCode"]);
            }

            if (!string.IsNullOrEmpty(Request.Params["districtCode"]))
            {
                ViewBag.districtCode = (Convert.ToInt32(Request.Params["districtCode"])) == -1 ? 0 : Convert.ToInt32(Request.Params["districtCode"]);
            }

            return View();
        }

        #region Allocate Works to Technical Expert

        #region Add TE details and create user

        public ActionResult AddTechnicalExpert()
        {
            try
            {
                return View();
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "QualityMonitoringController.AddTechnicalExpert()");
                return null;
            }

        }

        [Audit]
        public ActionResult LoadTechnicalExpertDetailsGrid(int? page, int? rows, string sidx, string sord)
        {
            try
            {
                long totalRecords = 0;
                IQualityMonitoringBAL objBAL = new QualityMonitoringBAL();

                using (CommonFunctions commonFunction = new CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new GridParams(page, rows, sidx, sord, Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                    {
                        return null;
                    }
                }

                var jsonData = new
                {
                    rows = objBAL.LoadTechnicalExpertDetailsGrid(Convert.ToInt32(page) - 1, Convert.ToInt32(rows), sidx, sord, out totalRecords),
                    total = totalRecords <= Convert.ToInt32(rows) ? 1 : totalRecords / Convert.ToInt32(rows) + 1,
                    page = Convert.ToInt32(page),
                    records = totalRecords,
                };

                return Json(jsonData, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "QualityMonitoringController.LoadTechnicalExpertDetailsGrid()");
                return null;
            }
        }

        public ActionResult AddNewTechnicalExpertDetails()
        {
            try
            {
                AddTechnicalExpert model = new AddTechnicalExpert();

                model.ADD_EDIT_FLAG = "A";

                return PartialView(model);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "QualityMonitoringController.AddNewTechnicalExpertDetails()");
                return null;
            }

        }

        [HttpPost]
        public ActionResult AddTechnicalExpertDetails(FormCollection formCollection)
        {
            bool status = false;
            string isValidMsg = String.Empty;
            String message = "Technical Expert details not saved";
            IQualityMonitoringBAL objBAL = new QualityMonitoringBAL();
            string mobileNumber = formCollection["TECHNICAL_EXPERT_MOBILE"];
            string email = formCollection["TECHNICAL_EXPERT_EMAIL"];
            string pan = formCollection["PAN_NUMBER"];
            dbContext = new PMGSYEntities();

            try
            {
                if (dbContext.MASTER_TECHNICAL_EXPERT.Where(x => x.TECHNICAL_EXPERT_PAN == pan).Any())
                    return Json(new { success = status, message = "PAN number already exist." });
                if (dbContext.MASTER_TECHNICAL_EXPERT.Where(x => x.TECHNICAL_EXPERT_MOBILE == mobileNumber).Any())
                    return Json(new { success = status, message = "Mobile number already exist." });
                if (dbContext.MASTER_TECHNICAL_EXPERT.Where(x => x.TECHNICAL_EXPERT_EMAIL == email).Any())
                    return Json(new { success = status, message = "Email already exist." });

                if (ModelState.IsValid)
                {
                    status = objBAL.AddTechnicalExpertDetails(formCollection, out isValidMsg);

                    return Json(new { success = status, message = isValidMsg });
                }

                return Json(new { success = status, message = message });

            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "QualityMonitoringController.AddTechnicalExpertDetails");
                return Json(new { success = status, message = message });
            }
        }

        public ActionResult EditTechnicalExpertDetails(int technicalExpertId)
        {
            try
            {
                AddTechnicalExpert model = new AddTechnicalExpert();
                MASTER_TECHNICAL_EXPERT detailModel = new MASTER_TECHNICAL_EXPERT();
                dbContext = new PMGSYEntities();

                detailModel = dbContext.MASTER_TECHNICAL_EXPERT.Where(x => x.ID == technicalExpertId).FirstOrDefault();

                model.ID = detailModel.ID;
                model.TECHNICAL_EXPERT_FNAME = string.IsNullOrWhiteSpace(detailModel.TECHNICAL_EXPERT_FNAME) ? "" : detailModel.TECHNICAL_EXPERT_FNAME;
                model.TECHNICAL_EXPERT_MNAME = string.IsNullOrWhiteSpace(detailModel.TECHNICAL_EXPERT_MNAME) ? "" : detailModel.TECHNICAL_EXPERT_MNAME;
                model.TECHNICAL_EXPERT_LNAME = string.IsNullOrWhiteSpace(detailModel.TECHNICAL_EXPERT_LNAME) ? "" : detailModel.TECHNICAL_EXPERT_LNAME;
                model.PAN_NUMBER = detailModel.TECHNICAL_EXPERT_PAN;
                model.TECHNICAL_EXPERT_MOBILE = detailModel.TECHNICAL_EXPERT_MOBILE;
                model.TECHNICAL_EXPERT_EMAIL = detailModel.TECHNICAL_EXPERT_EMAIL;
                model.ADD_EDIT_FLAG = "E";

                return PartialView("AddNewTechnicalExpertDetails", model);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "QualityMonitoringController.EditTechnicalExpertDetails");
                return null;
            }

        }

        [HttpPost]
        public ActionResult UpdateTechnicalExpertDetails(FormCollection formCollection)
        {
            bool status = false;
            string isValidMsg = String.Empty;
            String message = "Technical Expert details not updated";
            IQualityMonitoringBAL objBAL = new QualityMonitoringBAL();
            int id = Convert.ToInt32(formCollection["ID"]);
            string mobileNumber = formCollection["TECHNICAL_EXPERT_MOBILE"];
            string email = formCollection["TECHNICAL_EXPERT_EMAIL"];
            string pan = formCollection["PAN_NUMBER"];
            dbContext = new PMGSYEntities();

            try
            {
                if (dbContext.MASTER_TECHNICAL_EXPERT.Where(x => x.TECHNICAL_EXPERT_PAN == pan && x.ID != id).Any())
                    return Json(new { success = status, message = "PAN number already exist." });
                if (dbContext.MASTER_TECHNICAL_EXPERT.Where(x => x.TECHNICAL_EXPERT_MOBILE == mobileNumber && x.ID != id).Any())
                    return Json(new { success = status, message = "Mobile number already exist." });
                if (dbContext.MASTER_TECHNICAL_EXPERT.Where(x => x.TECHNICAL_EXPERT_EMAIL == email && x.ID != id).Any())
                    return Json(new { success = status, message = "Email already exist." });

                if (ModelState.IsValid)
                {
                    status = objBAL.UpdateTechnicalExpertDetails(formCollection, out isValidMsg);

                    return Json(new { success = status, message = isValidMsg });
                }

                return Json(new { success = status, message = message });

            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "QualityMonitoringController.UpdateTechnicalExpertDetails");
                return Json(new { success = status, message = message });
            }
        }

        [HttpPost]
        [Audit]
        public ActionResult CreateTechnicalExpertUser(int technicalExpertId)
        {
            dbContext = new PMGSYEntities();
            CommonFilterWrapper commonFilterWrapper = new CommonFilterWrapper();
            IQualityMonitoringBAL objBAL = new QualityMonitoringBAL();

            try
            {
                if (ModelState.IsValid)
                {
                    bool isCreated = objBAL.CreateTechnicalExpertUserBAL(technicalExpertId);

                    if (isCreated)
                    {
                        return Json(new { Success = true, message = "User created successfully." });
                    }
                    else
                    {
                        return Json(new { Success = false, message = "Error occured while creation of new user." });
                    }
                }
                else
                {
                    StringBuilder errorMessages = new StringBuilder();
                    foreach (var modelStateValue in ModelState.Values)
                    {
                        foreach (var error in modelStateValue.Errors)
                        {
                            errorMessages.Append(error.ErrorMessage);
                        }
                    }
                    return Json(new { Success = false, message = errorMessages.ToString() });
                }
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "QualityMonitoring.CreateTechnicalExpertUser()");
                ModelState.Clear();
                return Json(new { Success = false, message = "Error occured while creation of new user." });
            }
            finally
            {
                if (dbContext != null)
                    dbContext.Dispose();
            }
        }

        [HttpPost]
        public ActionResult DeactivateTechnicalExpertDetails(int technicalExpertId)
        {
            bool status = false;
            string isValidMsg = String.Empty;
            String message = "Technical Expert details cannot be deactivated";
            dbContext = new PMGSYEntities();
            MASTER_TECHNICAL_EXPERT model = new MASTER_TECHNICAL_EXPERT();
            UM_User_Master userModel = new UM_User_Master();

            try
            {
                model = dbContext.MASTER_TECHNICAL_EXPERT.Where(x => x.ID == technicalExpertId).FirstOrDefault();
                model.IS_ACTIVE = "N";
                model.DEACTIVATION_DATE = DateTime.Now;

                dbContext.Entry(model).State = System.Data.Entity.EntityState.Modified;

                // Deactivate associated user
                if (model.TECHNICAL_EXPERT_USER_ID != null)
                {
                    userModel = dbContext.UM_User_Master.Where(x => x.UserID == model.TECHNICAL_EXPERT_USER_ID).FirstOrDefault();
                    userModel.IsActive = false;

                    dbContext.Entry(userModel).State = System.Data.Entity.EntityState.Modified;
                }

                dbContext.SaveChanges();
                status = true;
                message = "Technical Expert details deactivated successfully.";


                return Json(new { success = status, message = message });

            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "QualityMonitoringController.DeactivateTechnicalExpertDetails");
                return Json(new { success = status, message = message });
            }
        }

        #endregion

        #region Allocate TE at CQC

        public ActionResult AllocateTechnicalExpertFilter()
        {
            dbContext = new PMGSYEntities();

            try
            {
                QMFilterViewModel qmFilterModel = new QMFilterViewModel();
                CommonFunctions objCommonFunctions = new CommonFunctions();
                ADMIN_QUALITY_MONITORS model = new ADMIN_QUALITY_MONITORS();

                qmFilterModel.MAST_STATE_CODE = 0;
                qmFilterModel.ADMIN_QM_CODE = 0;

                qmFilterModel.FROM_MONTH = DateTime.Now.Month;
                qmFilterModel.FROM_YEAR = DateTime.Now.Year;
                qmFilterModel.TO_MONTH = DateTime.Now.Month;
                qmFilterModel.TO_YEAR = DateTime.Now.Year;

                qmFilterModel.MAST_STATE_CODE = PMGSYSession.Current.StateCode;


                qmFilterModel.schemeList = new List<SelectListItem> {
                                                            new SelectListItem{ Text = "All Schemes", Value ="0" , Selected = true },
                                                            new SelectListItem{ Text = "PMGSY I", Value ="1" },
                                                            new SelectListItem{ Text = "PMGSY II", Value ="2" } ,
                                                            new SelectListItem{ Text = "RCPLWE", Value ="3" },
                                                            new SelectListItem{ Text = "PMGSY III", Value ="4" }
            };

                qmFilterModel.ROAD_STATUS_LIST = objCommonFunctions.PopulateRoadStatus();
                qmFilterModel.ROAD_STATUS_LIST.Insert(3, new SelectListItem { Text = "Maintenance", Value = "M" });

                qmFilterModel.STATES = objCommonFunctions.PopulateStates(false);
                qmFilterModel.STATES.Insert(0, (new SelectListItem { Text = "All", Value = "0", Selected = true }));

                qmFilterModel.MONITORS = objCommonFunctions.PopulateAllMonitors("false", "I", 0);

                // Added by Srishti on 10-04-2023
                if (PMGSYSession.Current.RoleCode == 6)
                {
                    model = dbContext.ADMIN_QUALITY_MONITORS.Where(x => x.ADMIN_USER_ID == PMGSYSession.Current.UserId).FirstOrDefault();

                    string fName = model.ADMIN_QM_FNAME == null ? "" : model.ADMIN_QM_FNAME;
                    string mName = model.ADMIN_QM_MNAME == null ? "" : model.ADMIN_QM_MNAME;
                    string lName = model.ADMIN_QM_LNAME == null ? "" : model.ADMIN_QM_LNAME;
                    qmFilterModel.MONITOR_NAME = fName + " " + mName + " " + lName;
                    qmFilterModel.ADMIN_QM_CODE = model.ADMIN_QM_CODE;
                }

                qmFilterModel.qmType = "NQM";

                qmFilterModel.roadOrBridgeList = new List<SelectListItem> {
                                                            new SelectListItem{ Text = "All", Value ="0" , Selected = true },
                                                            new SelectListItem{ Text = "Road", Value ="P" },
                                                            new SelectListItem{ Text = "Bridge", Value ="L" }

            };

                qmFilterModel.gradeTypeList = new List<SelectListItem> {
                                                            new SelectListItem{ Text = "All", Value ="0" , Selected = true },
                                                            new SelectListItem{ Text = "S", Value ="1" },
                                                            new SelectListItem{ Text = "SRI", Value ="2" },
                                                             new SelectListItem{ Text = "U", Value ="3" }


            };

                qmFilterModel.FROM_MONTHS_LIST = objCommonFunctions.PopulateMonths(false);
                qmFilterModel.FROM_YEARS_LIST = objCommonFunctions.PopulateYears(false).Where(a => a.Value != "0").ToList();
                qmFilterModel.TO_MONTHS_LIST = objCommonFunctions.PopulateMonths(false);
                qmFilterModel.TO_YEARS_LIST = objCommonFunctions.PopulateYears(false).Where(a => a.Value != "0").ToList();

                qmFilterModel.TechnicalExpertList = objCommonFunctions.PopulateTechnicalExpert();

                return View(qmFilterModel);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "QualityMonitoringController.AllocateTechnicalExpertFilter");
                return null;
            }
        }

        [HttpPost]
        [Audit]
        public ActionResult QMInspectionDetailsAllocateTechExpert(FormCollection formCollection)
        {
            qualityBAL = new QualityMonitoringBAL();
            int totalRecords;
            List<int> allocatedWorksObservationIdsList;
            try
            {

                using (CommonFunctions commonFunction = new CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new GridParams(Convert.ToInt32(formCollection["page"]), Convert.ToInt32(formCollection["rows"]), formCollection["sidx"], formCollection["sord"], Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                    {
                        return null;
                    }
                }

                var jsonData = new
                {
                    rows = qualityBAL.QMInspectionDetailsAllocateTechExpertBAL(Convert.ToInt32(formCollection["page"]) - 1, Convert.ToInt32(formCollection["rows"]),
                                                            formCollection["sidx"], formCollection["sord"], out totalRecords,
                                                            Convert.ToInt32(formCollection["stateCode"]), Convert.ToInt32(formCollection["monitorCode"]),
                                                            Convert.ToInt32(formCollection["fromInspMonth"]), Convert.ToInt32(formCollection["fromInspYear"]),
                                                            Convert.ToInt32(formCollection["toInspMonth"]), Convert.ToInt32(formCollection["toInspYear"]), formCollection["qmType"], Convert.ToInt32(formCollection["schemeType"]), formCollection["roadStatus"], formCollection["roadOrBridge"], formCollection["gradeType"], out allocatedWorksObservationIdsList),
                    total = totalRecords <= Convert.ToInt32(formCollection["rows"]) ? 1 : (totalRecords % Convert.ToInt32(formCollection["rows"]) == 0 ? totalRecords / Convert.ToInt32(formCollection["rows"]) : (totalRecords / Convert.ToInt32(formCollection["rows"]) + 1)),
                    page = Convert.ToInt32(formCollection["page"]),
                    records = totalRecords,
                    userdata = new { ids = allocatedWorksObservationIdsList.ToArray<int>() }
                };
                return Json(jsonData);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "QualityMonitoringController.QMInspectionDetailsAllocateTechExpert");
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return Json(String.Empty);
            }
        }

        [HttpPost]
        public ActionResult AssignTechExpert(int[] submitarray)
        {
            try
            {
                bool status = false;
                string isValidMsg = "Technical Expert not assigned"; ;

                qualityBAL = new QualityMonitoringBAL();
                int TechExpertID = submitarray[0];
                submitarray = submitarray.Skip(1).ToArray();

                status = qualityBAL.AssignTechExpertBAL(TechExpertID, submitarray, out isValidMsg);

                return Json(new { success = status, message = isValidMsg });
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "QualityMonitoringController.AssignTechExpert");
                return Json(new { success = false }, JsonRequestBehavior.DenyGet);
            }
        }

        // Added by Srishti on 10-04-2023
        [HttpPost]
        public ActionResult RemoveTechnicalExpert(int observationId)
        {
            try
            {
                bool status = false;
                string isValidMsg = "Technical Expert cannot be removed.";

                qualityBAL = new QualityMonitoringBAL();

                status = qualityBAL.RemoveTechnicalExpertBAL(observationId, out isValidMsg);

                return Json(new { success = status, message = isValidMsg });

            }
            catch (Exception ex)
            {

                ErrorLog.LogError(ex, "QualityMonitoringController.RemoveTechnicalExpert");
                return Json(new { success = false }, JsonRequestBehavior.DenyGet);
            }

        }

        [HttpPost]
        public ActionResult FinalizeTechnicalExpert()
        {
            try
            {
                bool status = false;
                string isValidMsg = "Technical Expert not finalized";

                qualityBAL = new QualityMonitoringBAL();

                if (Request.Params["arrFinalizeAndForward[]"] == null || Request.Params["arrFinalizeAndForward[]"].Equals(""))
                {
                    return Json(new { success = status, message = "Please select atleast one of the works." });
                }

                String[] arrWorksToFinalize = Request.Params["arrFinalizeAndForward[]"].Split(',');

                status = qualityBAL.FinalizeTechnicalExpertBAL(arrWorksToFinalize, out isValidMsg);

                return Json(new { success = status, message = isValidMsg });

            }
            catch (Exception ex)
            {

                ErrorLog.LogError(ex, "QualityMonitoringController.FinalizeTechnicalExpert");
                return Json(new { success = false }, JsonRequestBehavior.DenyGet);
            }

        }

        [HttpPost]
        public ActionResult ForwardToNQM(int observationId)
        {
            try
            {
                bool status = false;
                string isValidMsg = "Cannot forward";
                PMGSYEntities dbContext = new PMGSYEntities();
                QUALITY_QM_ASSIGN_TE_MASTER model = new QUALITY_QM_ASSIGN_TE_MASTER();

                model = dbContext.QUALITY_QM_ASSIGN_TE_MASTER.Where(x => x.OBSERVATION_ID == observationId).FirstOrDefault();

                model.FORWARD_STATUS = "N";

                dbContext.Entry(model).State = System.Data.Entity.EntityState.Modified;
                dbContext.SaveChanges();

                status = true;
                isValidMsg = "Remarks forwarded successfully.";

                return Json(new { success = status, message = isValidMsg });

            }
            catch (Exception ex)
            {

                ErrorLog.LogError(ex, "QualityMonitoringController.ForwardToNQM");
                return Json(new { success = false }, JsonRequestBehavior.DenyGet);
            }

        }
        [Audit]
        public ActionResult ViewTEQMRemarkForCQC(string id)
        {
            qualityBAL = new QualityMonitoringBAL();
            PMGSYEntities dbContext = new PMGSYEntities();
            try
            {
                //QMFillObservationModel fillObservationModel = qualityBAL.QMObservationDetails3TierSQCBAL(Convert.ToInt32(id));
                int obsId = id != "" ? Convert.ToInt32(id.Split('$')[0]) : 0;
                TEQMFillObservationModel fillObservationModel = qualityBAL.TEQMObservationDetailsBAL(Convert.ToInt32(obsId));
                fillObservationModel.QM_OBSERVATION_ID = Convert.ToInt32(obsId);
                ViewBag.AddEditView = id != "" ? id.Split('$')[1] : "";
                ViewBag.TE_STATUS = dbContext.QUALITY_QM_ASSIGN_TE_MASTER.Where(x => x.OBSERVATION_ID == obsId).Select(x => x.FORWARD_STATUS).FirstOrDefault();

                #region Consolidate Remarks added by Tech Expert
                if (ViewBag.AddEditView == "V" && (PMGSY.Extensions.PMGSYSession.Current.RoleCode == 5 || PMGSY.Extensions.PMGSYSession.Current.RoleCode == 9))
                {
                    List<QUALITY_QM_TE_REMARK_DETAILS> consTERemarks = new List<QUALITY_QM_TE_REMARK_DETAILS>();
                    consTERemarks = dbContext.QUALITY_QM_ASSIGN_TE_MASTER.Where(x => x.OBSERVATION_ID == obsId && x.IS_REVIEWED_BY_TECHNICAL_EXPERT == "Y").Any() ? dbContext.QUALITY_QM_TE_REMARK_DETAILS.Where(x => x.QUALITY_QM_ASSIGN_TE_MASTER.OBSERVATION_ID == obsId).OrderBy(x => x.MAST_ITEM_NUMBER).ToList() : null;
                    ViewBag.ConsTERemarks = consTERemarks;
                }
                #endregion

                return View(fillObservationModel);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return null;
            }
        }

        #endregion

        #region Add remark by TE 
        [HttpPost]
        [Audit]
        public ActionResult QMInspectionDetailsTechExpertReviewList(FormCollection formCollection)
        {
            qualityBAL = new QualityMonitoringBAL();
            int totalRecords;
            try
            {

                using (CommonFunctions commonFunction = new CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new GridParams(Convert.ToInt32(formCollection["page"]), Convert.ToInt32(formCollection["rows"]), formCollection["sidx"], formCollection["sord"], Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                    {
                        return null;
                    }
                }

                var jsonData = new
                {
                    rows = qualityBAL.QMInspectionDetailsTechExpertReviewBAL(Convert.ToInt32(formCollection["page"]) - 1, Convert.ToInt32(formCollection["rows"]),
                                                            formCollection["sidx"], formCollection["sord"], out totalRecords,
                                                            Convert.ToInt32(formCollection["stateCode"]), Convert.ToInt32(formCollection["monitorCode"]),
                                                            Convert.ToInt32(formCollection["fromInspMonth"]), Convert.ToInt32(formCollection["fromInspYear"]),
                                                            Convert.ToInt32(formCollection["toInspMonth"]), Convert.ToInt32(formCollection["toInspYear"]), formCollection["qmType"], Convert.ToInt32(formCollection["schemeType"]), formCollection["roadStatus"], formCollection["roadOrBridge"], formCollection["gradeType"]),
                    total = totalRecords <= Convert.ToInt32(formCollection["rows"]) ? 1 : (totalRecords % Convert.ToInt32(formCollection["rows"]) == 0 ? totalRecords / Convert.ToInt32(formCollection["rows"]) : (totalRecords / Convert.ToInt32(formCollection["rows"]) + 1)),
                    page = Convert.ToInt32(formCollection["page"]),
                    records = totalRecords
                };
                return Json(jsonData);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "QualityMonitoringController.QMInspectionDetailsTechExpertReviewList");
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return Json(String.Empty);
            }
        }


        [Audit]
        //[HttpPost]
        public ActionResult TEQMObservationDetails(string id)
        {
            qualityBAL = new QualityMonitoringBAL();
            PMGSYEntities dbContext = new PMGSYEntities();
            try
            {
                //QMFillObservationModel fillObservationModel = qualityBAL.QMObservationDetails3TierSQCBAL(Convert.ToInt32(id));
                int obsId = id != "" ? Convert.ToInt32(id.Split('$')[0]) : 0;
                TEQMFillObservationModel fillObservationModel = qualityBAL.TEQMObservationDetailsBAL(Convert.ToInt32(obsId));
                fillObservationModel.QM_OBSERVATION_ID = Convert.ToInt32(obsId);
                ViewBag.AddEditView = id != "" ? id.Split('$')[1] : "";

                #region Consolidate Remarks added by Tech Expert
                if (ViewBag.AddEditView == "V" && PMGSYSession.Current.RoleCode == 81)
                {
                    List<QUALITY_QM_TE_REMARK_DETAILS> consTERemarks = new List<QUALITY_QM_TE_REMARK_DETAILS>();
                    consTERemarks = dbContext.QUALITY_QM_ASSIGN_TE_MASTER.Where(x => x.OBSERVATION_ID == obsId && x.IS_REVIEWED_BY_TECHNICAL_EXPERT == "Y").Any() ? dbContext.QUALITY_QM_TE_REMARK_DETAILS.Where(x => x.QUALITY_QM_ASSIGN_TE_MASTER.OBSERVATION_ID == obsId).OrderBy(x => x.MAST_ITEM_NUMBER).ToList() : null;
                    ViewBag.ConsTERemarks = consTERemarks;
                }
                #endregion

                return View(fillObservationModel);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "QualityMonitoringController.DeactivateTechnicalExpertDetails");
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return null;
            }
        }

        [Audit]
        [HttpPost]
        public ActionResult SaveTechExpertRemarks(FormCollection formdata)
        {
            qualityBAL = new QualityMonitoringBAL();
            try
            {
                //int obsId = formdata.Keys.ToString() == "QM_OBSERVATION_ID" ? Convert.ToInt32(formdata["QM_OBSERVATION_ID"]): 0;
                TEQMFillObservationModel fillObservationModel = new TEQMFillObservationModel();
                Dictionary<int, String> itemwiseRemark = new Dictionary<int, String>();
                int obsId = 0;
                string propType = formdata["IMS_PROPOSAL_TYPE"];
                //int itemIndex = (propType == "P" && formdata["IMS_ISCOMPLETED"] == "M") ? 0 :1;
                int itemIndex = 0;
                int keyCount = formdata.AllKeys.Count();
                bool status = false;
                string message = "Failed to add Remarks!!";
                string generalObs = PMGSYSession.Current.RoleCode == 81 ? formdata["GENERALREMARKS"] : null;

                //foreach (var key in formdata.AllKeys)

                for (int i = 0; i < formdata.AllKeys.Count(); i++)
                {
                    var key = formdata.AllKeys[i];
                    if (key.Contains("QM_OBSERVATION_ID"))
                    {
                        obsId = Convert.ToInt32(formdata[key]);
                        //break;
                    }
                    else if (PMGSYSession.Current.RoleCode == 81 && key.Equals("GRADE_DETAILS_LIST[" + itemIndex + "].TEREMARKS"))
                    {
                        string remarks = formdata["GRADE_DETAILS_LIST[" + itemIndex + "].TEREMARKS"].Trim();
                        string item_no = formdata["GRADE_DETAILS_LIST[" + itemIndex + "].MAST_ITEM_NO"];
                        if (!(remarks == "" || remarks == null) && !(item_no == "" || item_no == null))
                        {
                            itemwiseRemark.Add(Convert.ToInt32(item_no), remarks);
                        }

                        itemIndex++;
                    }
                    else if (PMGSYSession.Current.RoleCode == 6 && key.Equals("GRADE_DETAILS_LIST[" + itemIndex + "].NQMREMARKS"))
                    {
                        PMGSYEntities dbContext = new PMGSYEntities();
                        try
                        {

                            string nqmRemarks = formdata["GRADE_DETAILS_LIST[" + itemIndex + "].NQMREMARKS"].Trim();
                            //string teRemarks = formdata["GRADE_DETAILS_LIST[" + itemIndex + "].TEREMARKS"].Trim();
                            string item_no = formdata["GRADE_DETAILS_LIST[" + itemIndex + "].MAST_ITEM_NO"];
                            int teRemarkItemNo = item_no != null ? Convert.ToInt32(item_no) : 0;
                            string teRemarks = dbContext.QUALITY_QM_TE_REMARK_DETAILS.Where(x => x.QUALITY_QM_ASSIGN_TE_MASTER.OBSERVATION_ID == obsId && x.MAST_ITEM_NUMBER == teRemarkItemNo).Select(x => x.TECH_EXPERT_REMARK).FirstOrDefault();

                            if (!(teRemarks == "" || teRemarks == null) && (nqmRemarks == "" || nqmRemarks == null) && !(teRemarkItemNo == 75 || teRemarkItemNo == 364 || teRemarkItemNo == 283 || teRemarkItemNo == 438))
                            {
                                ModelState.AddModelError(formdata["GRADE_DETAILS_LIST[" + itemIndex + "].NQMREMARKS"], @"Please fill all the NQM remarks");
                                //return Json(new { Success = false, status = "Please fill all the NQM remarks" });

                            }
                            else if (!(nqmRemarks == "" || nqmRemarks == null) && !(item_no == "" || item_no == null))
                            {
                                itemwiseRemark.Add(Convert.ToInt32(item_no), nqmRemarks);
                            }

                            //if (!(nqmRemarks == "" || nqmRemarks == null) && !(item_no == "" || item_no == null))
                            //{
                            //    itemwiseRemark.Add(Convert.ToInt32(item_no), nqmRemarks);
                            //}



                            itemIndex++;
                        }
                        catch (Exception ex)
                        {
                            ErrorLog.LogError(ex, "QualityMonitoringController.SaveTechExpertRemarks");
                            return Json(new { success = false, status = "Failed to add Remarks!!" }, JsonRequestBehavior.DenyGet);
                            //throw;
                        }
                        finally
                        {
                            if (dbContext != null)
                            {
                                dbContext.Dispose();
                            }

                        }

                    }


                    //itemIndex++;

                }

                //fillObservationModel = obsId != 0 ? qualityBAL.TEQMObservationDetailsBAL(Convert.ToInt32(obsId)) :null;

                if (itemwiseRemark != null)
                {
                    if (ModelState.IsValid)
                    {
                        status = qualityBAL.SaveTechExpertRemarksBAL(itemwiseRemark, obsId, out message, generalObs);
                        return Json(new { Success = status, status = message });
                    }
                    else
                    {
                        //TEQMObservationDetails(obsId.ToString()+"$"+"A");
                        return Json(new { Success = false, status = "All the fields are mandatory." });
                    }

                }
                else
                {
                    return Json(new { Success = true, status = message });

                }


            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "QualityMonitoringController.SaveTechExpertRemarks");
                return Json(new { success = false, status = "Failed to add Remarks!!" }, JsonRequestBehavior.DenyGet);
                //throw;
            }

        }

        [Audit]
        [HttpPost]
        public ActionResult UpdateTechExpertRemarks(FormCollection formdata)
        {
            qualityBAL = new QualityMonitoringBAL();
            try
            {
                //int obsId = formdata.Keys.ToString() == "QM_OBSERVATION_ID" ? Convert.ToInt32(formdata["QM_OBSERVATION_ID"]): 0;
                TEQMFillObservationModel fillObservationModel = new TEQMFillObservationModel();
                Dictionary<int, String> itemwiseRemark = new Dictionary<int, String>();
                int obsId = 0;
                string propType = formdata["IMS_PROPOSAL_TYPE"];
                //int itemIndex = (propType == "P" && formdata["IMS_ISCOMPLETED"] == "M") ? 0 :1;
                int itemIndex = 0;
                int keyCount = formdata.AllKeys.Count();
                bool status = false;
                string message = "Failed to Update Remarks!!";
                string generalObs = PMGSYSession.Current.RoleCode == 81 ? formdata["GENERALREMARKS"] : null;

                //foreach (var key in formdata.AllKeys)

                for (int i = 0; i < formdata.AllKeys.Count(); i++)
                {
                    var key = formdata.AllKeys[i];
                    if (key.Contains("QM_OBSERVATION_ID"))
                    {
                        obsId = Convert.ToInt32(formdata[key]);
                        //break;
                    }
                    else if (PMGSYSession.Current.RoleCode == 81 && key.Equals("GRADE_DETAILS_LIST[" + itemIndex + "].TEREMARKS"))
                    {
                        string remarks = formdata["GRADE_DETAILS_LIST[" + itemIndex + "].TEREMARKS"].Trim();
                        string item_no = formdata["GRADE_DETAILS_LIST[" + itemIndex + "].MAST_ITEM_NO"];
                        if (!(item_no == "" || item_no == null))
                        {
                            itemwiseRemark.Add(Convert.ToInt32(item_no), remarks);
                        }

                        itemIndex++;
                    }
                    else if (PMGSYSession.Current.RoleCode == 6 && key.Equals("GRADE_DETAILS_LIST[" + itemIndex + "].NQMREMARKS"))
                    {
                        PMGSYEntities dbContext = new PMGSYEntities();
                        try
                        {
                            string nqmRemarks = formdata["GRADE_DETAILS_LIST[" + itemIndex + "].NQMREMARKS"].Trim();
                            string item_no = formdata["GRADE_DETAILS_LIST[" + itemIndex + "].MAST_ITEM_NO"];
                            int teRemarkItemNo = item_no != null ? Convert.ToInt32(item_no) : 0;
                            string teRemarks = dbContext.QUALITY_QM_TE_REMARK_DETAILS.Where(x => x.QUALITY_QM_ASSIGN_TE_MASTER.OBSERVATION_ID == obsId && x.MAST_ITEM_NUMBER == teRemarkItemNo).Select(x => x.TECH_EXPERT_REMARK).FirstOrDefault();

                            //if ( !(item_no == "" || item_no == null))
                            //{
                            //    itemwiseRemark.Add(Convert.ToInt32(item_no), remarks);
                            //}

                            if (!(teRemarks == "" || teRemarks == null) && (nqmRemarks == "" || nqmRemarks == null) && !(teRemarkItemNo == 75 || teRemarkItemNo == 364 || teRemarkItemNo == 283 || teRemarkItemNo == 438))
                            {
                                ModelState.AddModelError(formdata["GRADE_DETAILS_LIST[" + itemIndex + "].NQMREMARKS"], @"Please fill all the NQM remarks");

                            }
                            else if (!(nqmRemarks == "" || nqmRemarks == null) && !(item_no == "" || item_no == null))
                            {
                                itemwiseRemark.Add(Convert.ToInt32(item_no), nqmRemarks);
                            }

                            itemIndex++;
                        }
                        catch (Exception ex)
                        {
                            ErrorLog.LogError(ex, "QualityMonitoringController.UpdateTechExpertRemarks");
                            return Json(new { success = false, status = "Failed to Update Remarks!!" }, JsonRequestBehavior.DenyGet);
                            //throw;
                        }
                        finally
                        {
                            if (dbContext != null)
                            {
                                dbContext.Dispose();
                            }

                        }
                    }


                    //itemIndex++;

                }

                //fillObservationModel = obsId != 0 ? qualityBAL.TEQMObservationDetailsBAL(Convert.ToInt32(obsId)) :null;

                if (itemwiseRemark != null)
                {
                    if (ModelState.IsValid)
                    {
                        status = qualityBAL.UpdateTechExpertRemarksBAL(itemwiseRemark, obsId, out message, generalObs);
                        return Json(new { Success = status, status = message });
                    }
                    else
                    {
                        //TEQMObservationDetails(obsId.ToString()+"$"+"A");
                        return Json(new { Success = false, status = "All the fields are mandatory." });
                    }

                }
                else
                {
                    return Json(new { Success = true, status = message });

                }


            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "QualityMonitoringController.UpdateTechExpertRemarks");
                return Json(new { success = false, status = "Failed to Update Tecnical Expert Remarks!!" }, JsonRequestBehavior.DenyGet);
                //throw;
            }

        }


        [HttpPost]
        [Audit]
        public ActionResult ForwardTEQMReplyToCQC(String id)
        {
            try
            {

                QUALITY_QM_ASSIGN_TE_MASTER qualityQmAssignTechMaster = new QUALITY_QM_ASSIGN_TE_MASTER();
                int obsId = id != null ? Convert.ToInt32(id) : 0;

                using (PMGSYEntities dbContext = new PMGSYEntities())
                {
                    qualityQmAssignTechMaster = dbContext.QUALITY_QM_ASSIGN_TE_MASTER.Where(x => x.OBSERVATION_ID == obsId).FirstOrDefault();



                    if (qualityQmAssignTechMaster != null)
                    {
                        if (PMGSYSession.Current.RoleCode == 6)//Nqm
                        {
                            qualityQmAssignTechMaster.FORWARD_STATUS = "F"; //fowarded to cqc from Nqm
                            qualityQmAssignTechMaster.IS_REVIEWED_BY_NQM = "Y";
                            qualityQmAssignTechMaster.NQM_REVIEW_DATE = DateTime.Now;
                            dbContext.Entry(qualityQmAssignTechMaster).State = System.Data.Entity.EntityState.Modified;
                            dbContext.SaveChanges();
                            return Json(new { success = true, message = "Quality Monitor remarks forwarded to CQC successfully !!" });
                        }
                        else if (PMGSYSession.Current.RoleCode == 81)//Technical Expert
                        {
                            if (!dbContext.QUALITY_QM_TE_REMARK_DETAILS.Where(x => x.TE_MASTER_ID == qualityQmAssignTechMaster.TE_MASTER_ID).Any())
                            {
                                return Json(new { success = false, message = "All the remarks cannot be empty !!" });
                            }


                            qualityQmAssignTechMaster.FORWARD_STATUS = "C";//forwarded to cqc from  Technical Expert
                            qualityQmAssignTechMaster.IS_REVIEWED_BY_TECHNICAL_EXPERT = "Y";
                            qualityQmAssignTechMaster.TE_REVIEW_DATE = DateTime.Now;
                            dbContext.Entry(qualityQmAssignTechMaster).State = System.Data.Entity.EntityState.Modified;
                            dbContext.SaveChanges();
                            return Json(new { success = true, message = "Technical Expert remarks forwarded to CQC successfully !!" });
                        }

                    }
                }
                return Json(new { success = false, message = "Forwarding failed !!" });
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "QualityMonitoringController.QMInspectionDetailsTechExpertReviewList");
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return Json(new { success = false, message = "Forwarding failed !!" });
            }

        }

        #endregion



        #region Add Payment Information For TE

        public ActionResult AddFilterTePayment()
        {
            return View();
        }

        public ActionResult TechnicalExpertPaymentDetails(int? page, int? rows, string sidx, string sord)
        {
            qualityBAL = new QualityMonitoringBAL();
            int totalRecords;
            try
            {

                using (CommonFunctions commonFunction = new CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new GridParams(page, rows, sidx, sord, Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                    {
                        return null;
                    }
                }

                var jsonData = new
                {
                    rows = qualityBAL.TechnicalExpertPaymentDetailsBAL(Convert.ToInt32(page) - 1, Convert.ToInt32(rows), sidx, sord, out totalRecords),
                    total = totalRecords <= Convert.ToInt32(rows) ? 1 : totalRecords / Convert.ToInt32(rows) + 1,
                    page = Convert.ToInt32(page),
                    records = totalRecords
                };
                return Json(jsonData, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "QualityMonitoringController.TechnicalExpertPaymentDetails");
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return Json(String.Empty);
            }
        }

        [HttpPost]
        [Audit]
        public ActionResult AddTechnicalExpertPaymentList(FormCollection formCollection)
        {
            qualityBAL = new QualityMonitoringBAL();
            int totalRecords;
            List<int> allocatedWorksObservationIdsList;
            try
            {

                using (CommonFunctions commonFunction = new CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new GridParams(Convert.ToInt32(formCollection["page"]), Convert.ToInt32(formCollection["rows"]), formCollection["sidx"], formCollection["sord"], Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                    {
                        return null;
                    }
                }

                var jsonData = new
                {
                    rows = qualityBAL.AddTechnicalExpertPaymentListBAL(Convert.ToInt32(formCollection["page"]) - 1, Convert.ToInt32(formCollection["rows"]), formCollection["sidx"], formCollection["sord"], out totalRecords, Convert.ToInt32(formCollection["teId"]), out allocatedWorksObservationIdsList),
                    total = totalRecords <= Convert.ToInt32(formCollection["rows"]) ? 1 : (totalRecords % Convert.ToInt32(formCollection["rows"]) == 0 ? totalRecords / Convert.ToInt32(formCollection["rows"]) : (totalRecords / Convert.ToInt32(formCollection["rows"]) + 1)),
                    page = Convert.ToInt32(formCollection["page"]),
                    records = totalRecords,
                    userdata = new { ids = allocatedWorksObservationIdsList.ToArray<int>() }
                };
                return Json(jsonData);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "QualityMonitoringController.AddTechnicalExpertPaymentList");
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return Json(String.Empty);
            }
        }

        [HttpPost]
        [Audit]
        public ActionResult AddTechnicalExpertPaymentWiseList(FormCollection formCollection)
        {
            qualityBAL = new QualityMonitoringBAL();
            int totalRecords;
            List<int> allocatedWorksObservationIdsList;
            try
            {

                using (CommonFunctions commonFunction = new CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new GridParams(Convert.ToInt32(formCollection["page"]), Convert.ToInt32(formCollection["rows"]), formCollection["sidx"], formCollection["sord"], Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                    {
                        return null;
                    }
                }

                var jsonData = new
                {
                    rows = qualityBAL.AddTechnicalExpertPaymentWiseListBAL(Convert.ToInt32(formCollection["page"]) - 1, Convert.ToInt32(formCollection["rows"]), formCollection["sidx"], formCollection["sord"], out totalRecords, formCollection["teId"], out allocatedWorksObservationIdsList),
                    total = totalRecords <= Convert.ToInt32(formCollection["rows"]) ? 1 : (totalRecords % Convert.ToInt32(formCollection["rows"]) == 0 ? totalRecords / Convert.ToInt32(formCollection["rows"]) : (totalRecords / Convert.ToInt32(formCollection["rows"]) + 1)),
                    page = Convert.ToInt32(formCollection["page"]),
                    records = totalRecords,
                    userdata = new { ids = allocatedWorksObservationIdsList.ToArray<int>() }
                };
                return Json(jsonData);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "QualityMonitoringController.AddTechnicalExpertPaymentWiseList");
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return Json(String.Empty);
            }
        }

        [HttpPost]
        public ActionResult AddPaymentDetails(int[] submitarray)
        {
            try
            {
                bool status = false;
                string isValidMsg = "Payment details cannot be added."; ;

                qualityBAL = new QualityMonitoringBAL();

                status = qualityBAL.AddPaymentDetailsBAL(submitarray, out isValidMsg);

                return Json(new { success = status, message = isValidMsg });
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "QualityMonitoringController.AddPaymentDetails");
                return Json(new { success = false }, JsonRequestBehavior.DenyGet);
            }
        }

        #endregion

        #endregion

    }
}
