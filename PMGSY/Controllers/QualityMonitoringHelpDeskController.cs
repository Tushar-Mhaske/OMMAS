using PMGSY.BAL.QualityMonitoringHelpDesk;
using PMGSY.Common;
using PMGSY.Models;
using PMGSY.Models.QualityMonitoringHelpDesk;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PMGSY.Controllers
{
    [RequiredAuthentication]
    [RequiredAuthorization]
    [Audit]
    public class QualityMonitoringHelpDeskController : Controller
    {
        QualityMonitoringHelpDeskBAL qualityMonitoringHelpDeskBAL = null;
        string message = string.Empty;
        public ActionResult QMHelpDesk()
        {
            CommonFunctions objCommonFunctions = new CommonFunctions();
            try
            {
                var yearRows = objCommonFunctions.PopulateYears(false);

                List<SelectListItem> mnthRows = objCommonFunctions.PopulateMonths(false);
                mnthRows.Find(x => x.Text.Equals(DateTime.Now.ToString("MMMM"))).Selected = true;

                // var mnthRows=Enumerable.Range(mnthRows2).Select(

                List<SelectListItem> lstMonitorType = objCommonFunctions.PopulateMonitorTypes();
                var qmType = lstMonitorType;
                List<SelectListItem> ddState = objCommonFunctions.PopulateStates(false);
                SelectListItem all = new SelectListItem
                {
                    Selected = true,
                    Text = "All State",
                    Value = "0"
                };               
                ddState.Insert(0, all);
                ViewData["mnthRows"] = mnthRows;
                ViewData["yearRows"] = yearRows;
                ViewData["qmType"] = qmType;
                ViewData["userName"] = string.Empty;
                ViewData["state"] = ddState;

                return View();
            }
            catch (Exception)
            {
                return null;
            }
        }


        #region MonitorDetails
        /// <summary>
        /// Monitor Details on selected year, month and QM Type
        /// </summary>
        /// <param name="formCollection"></param>
        /// <returns></returns>
        [HttpPost]
        [Audit]
        public ActionResult ListUsers(FormCollection formCollection)
        {
            qualityMonitoringHelpDeskBAL = new QualityMonitoringHelpDeskBAL();
            try
            {
                //Adde By Abhishek kamble 1-May-2014 start
                using (CommonFunctions commonFunction = new CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new GridParams(Convert.ToInt32(formCollection["page"]), Convert.ToInt32(formCollection["rows"]), formCollection["sidx"], formCollection["sord"], Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                    {
                        return null;
                    }
                }
                //Adde By Abhishek kamble 1-May-2014 end
                int totalRecords;
                Int32 getSelectedMonth = Convert.ToInt32(Request.Params["mnth"]);
                string getSelectedYear = Request.Params["yr"].Trim();
                string getSelectedQmType = Request.Params["qm"].Trim();

                var jsonData = new
                {
                    rows = qualityMonitoringHelpDeskBAL.QMMonitorDetailsBAL(Convert.ToInt32(formCollection["page"]) - 1, Convert.ToInt32(formCollection["rows"]),
                                                                        formCollection["sidx"], formCollection["sord"], out totalRecords, getSelectedMonth, getSelectedYear, getSelectedQmType, formCollection["filters"]),

                    total = totalRecords <= Convert.ToInt32(formCollection["rows"]) ? 1 : (totalRecords % Convert.ToInt32(formCollection["rows"]) == 0 ? totalRecords / Convert.ToInt32(formCollection["rows"]) : (totalRecords / Convert.ToInt32(formCollection["rows"]) + 1)),
                    page = Convert.ToInt32(formCollection["page"]),
                    records = totalRecords,

                };
                return Json(jsonData, JsonRequestBehavior.AllowGet);

            }
            catch (Exception)
            {
                return Json(String.Empty, JsonRequestBehavior.AllowGet);
            }

        }
        #endregion


        #region ScheduleDetails
        /// <summary>
        /// Schedule Details on selecting Montior Name
        /// </summary>
        /// <param name="formCollection"></param>
        /// <returns></returns>
        [HttpPost]
        [Audit]
        public ActionResult ListScheduleResult(FormCollection formCollection)
        {

            qualityMonitoringHelpDeskBAL = new QualityMonitoringHelpDeskBAL();
            try
            {
                //Adde By Abhishek kamble 1-May-2014 start
                using (CommonFunctions commonFunction = new CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new GridParams(Convert.ToInt32(formCollection["page"]), Convert.ToInt32(formCollection["rows"]), formCollection["sidx"], formCollection["sord"], Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                    {
                        return null;
                    }
                }
                //Adde By Abhishek kamble 1-May-2014 end
                int totalRecords;
                Int32 getSelectedMonth = Convert.ToInt32(Request.Params["mnth"]);
                string getSelectedYear = Request.Params["yr"].Trim();
                string getSelectedQmType = Request.Params["qm"].Trim();
                string rowID = Request.Params["selectRowId"].Trim();

                var jsonData = new
                {
                    rows = qualityMonitoringHelpDeskBAL.QMScheduleDetailsBAL(Convert.ToInt32(formCollection["page"]) - 1, Convert.ToInt32(formCollection["rows"]),
                                                                        formCollection["sidx"], formCollection["sord"], out totalRecords, Convert.ToInt32(rowID), getSelectedMonth, getSelectedYear, getSelectedQmType, formCollection["filters"]),

                    total = totalRecords <= Convert.ToInt32(formCollection["rows"]) ? 1 : (totalRecords % Convert.ToInt32(formCollection["rows"]) == 0 ? totalRecords / Convert.ToInt32(formCollection["rows"]) : (totalRecords / Convert.ToInt32(formCollection["rows"]) + 1)),
                    page = Convert.ToInt32(formCollection["page"]),
                    records = totalRecords,

                };
                return Json(jsonData, JsonRequestBehavior.AllowGet);

            }
            catch (Exception)
            {
                return Json(String.Empty, JsonRequestBehavior.AllowGet);
            }

        }
        #endregion


        #region LogDetails
        /// <summary>
        /// Log Details on desired Admin Code (rowID)
        /// </summary>
        /// <param name="formCollection"></param>
        /// <returns></returns>
        [HttpPost]
        [Audit]
        public ActionResult LogDetails(FormCollection formCollection)
        {



            qualityMonitoringHelpDeskBAL = new QualityMonitoringHelpDeskBAL();
            try
            {
                //Adde By Abhishek kamble 1-May-2014 start
                using (CommonFunctions commonFunction = new CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new GridParams(Convert.ToInt32(formCollection["page"]), Convert.ToInt32(formCollection["rows"]), formCollection["sidx"], formCollection["sord"], Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                    {
                        return null;
                    }
                }
                //Adde By Abhishek kamble 1-May-2014 end
                int totalRecords;

                string rowID = Request.Params["selectRowId"].Trim();

                var jsonData = new
                {
                    rows = qualityMonitoringHelpDeskBAL.QMLogDetailsBAL(Convert.ToInt32(formCollection["page"]) - 1, Convert.ToInt32(formCollection["rows"]),
                                                                        formCollection["sidx"], formCollection["sord"], out totalRecords, Convert.ToInt32(rowID)),

                    total = totalRecords <= Convert.ToInt32(formCollection["rows"]) ? 1 : (totalRecords % Convert.ToInt32(formCollection["rows"]) == 0 ? totalRecords / Convert.ToInt32(formCollection["rows"]) : (totalRecords / Convert.ToInt32(formCollection["rows"]) + 1)),
                    page = Convert.ToInt32(formCollection["page"]),
                    records = totalRecords,

                };
                return Json(jsonData, JsonRequestBehavior.AllowGet);

            }
            catch (Exception)
            {
                return Json(String.Empty, JsonRequestBehavior.AllowGet);
            }


        }
        #endregion


        #region StoreImeiNumber
        /// <summary>
        /// Storing the IMEI number for Log and Updating the new IMEI number
        /// </summary>
        /// 
        /// <returns></returns>
        [HttpPost]
        [Audit]
        public ActionResult StoreIMEINumber()
        {


            qualityMonitoringHelpDeskBAL = new QualityMonitoringHelpDeskBAL();
            try
            {
                string adminQmCode = Request.Params["AdminQmCode"].Trim();
                string imeiNumber = string.Empty;

                int result = qualityMonitoringHelpDeskBAL.QMStoreImeiNumberBAL(Convert.ToInt32(adminQmCode), imeiNumber);

                return Json(result, JsonRequestBehavior.AllowGet);

            }
            catch (Exception)
            {
                return Json(String.Empty, JsonRequestBehavior.AllowGet);
            }


        }
        #endregion


        #region MonitorName
        /// <summary>
        ///  Getting the Monitor Name for confirmation while updating the new IMEI number
        /// </summary>
        /// 
        /// <returns></returns>
        [HttpPost]
        [Audit]
        public ActionResult GetUserName()
        {

            qualityMonitoringHelpDeskBAL = new QualityMonitoringHelpDeskBAL();
            try
            {
                string adminQmCode = Request.Params["AdminQmCode"].Trim();

                string result = qualityMonitoringHelpDeskBAL.QMUserNameBAL(Convert.ToInt32(adminQmCode));

                return Json(result, JsonRequestBehavior.AllowGet);

            }
            catch (Exception)
            {
                return Json(String.Empty, JsonRequestBehavior.AllowGet);
            }

        }
        #endregion


        #region ImageDetails
        /// <summary>
        /// Getting the Image Details of desired Road and the Admin Code
        /// </summary>
        /// <param name="formCollection"></param>
        /// <returns></returns>
        [HttpPost]
        [Audit]
        public ActionResult GetImageDetails(FormCollection formCollection)
        {
            qualityMonitoringHelpDeskBAL = new QualityMonitoringHelpDeskBAL();
            try
            {
                //Adde By Abhishek kamble 1-May-2014 start
                using (CommonFunctions commonFunction = new CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new GridParams(Convert.ToInt32(formCollection["page"]), Convert.ToInt32(formCollection["rows"]), formCollection["sidx"], formCollection["sord"], Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                    {
                        return null;
                    }
                }
                //Adde By Abhishek kamble 1-May-2014 end
                int totalRecords;

                string adminScheduleCode = Request.Params["adminScheduleCode"].Trim();
                string roadCode = Request.Params["roadCode"].Trim();

                var jsonData = new
                {
                    rows = qualityMonitoringHelpDeskBAL.QMImageDetailsBAL(Convert.ToInt32(formCollection["page"]) - 1, Convert.ToInt32(formCollection["rows"]),
                                                                        formCollection["sidx"], formCollection["sord"], out totalRecords, Convert.ToInt32(adminScheduleCode), Convert.ToInt32(roadCode)),

                    total = totalRecords <= Convert.ToInt32(formCollection["rows"]) ? 1 : (totalRecords % Convert.ToInt32(formCollection["rows"]) == 0 ? totalRecords / Convert.ToInt32(formCollection["rows"]) : (totalRecords / Convert.ToInt32(formCollection["rows"]) + 1)),
                    page = Convert.ToInt32(formCollection["page"]),
                    records = totalRecords,

                };
                return Json(jsonData, JsonRequestBehavior.AllowGet);

            }
            catch (Exception)
            {
                return Json(String.Empty, JsonRequestBehavior.AllowGet);
            }

        }
        #endregion


        #region ObservationDetails
        /// <summary>
        /// Getting the Observation Details Between two Dates And the QMType("NQM" or "SQM")
        /// </summary>
        /// <param name="formCollection"></param>
        /// <returns></returns>
        [HttpPost]
        [Audit]
        public ActionResult ObservationDetails(FormCollection formCollection)
        {
            qualityMonitoringHelpDeskBAL = new QualityMonitoringHelpDeskBAL();
            CommonFunctions objCommonFunctions = new CommonFunctions();
            try
            {
                //Adde By Abhishek kamble 1-May-2014 start
                using (CommonFunctions commonFunction = new CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new GridParams(Convert.ToInt32(formCollection["page"]), Convert.ToInt32(formCollection["rows"]), formCollection["sidx"], formCollection["sord"], Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                    {
                        return null;
                    }
                }
                //Adde By Abhishek kamble 1-May-2014 end
                int totalRecords;
                string getSelectedQmType = Request.Params["qm"].Trim();
                DateTime fromDate = objCommonFunctions.GetStringToDateTime(Request.Params["FromDate"].Trim());
                DateTime toDate = objCommonFunctions.GetStringToDateTime(Request.Params["ToDate"].Trim()); //Convert.ToDateTime(Request.Params["ToDate"].Trim());

                var jsonData = new
                {
                    rows = qualityMonitoringHelpDeskBAL.QMObservationDetailsBAL(Convert.ToInt32(formCollection["page"]) - 1, Convert.ToInt32(formCollection["rows"]),
                                                                        formCollection["sidx"], formCollection["sord"], out totalRecords, formCollection["filters"], fromDate, toDate, getSelectedQmType),

                    total = totalRecords <= Convert.ToInt32(formCollection["rows"]) ? 1 : (totalRecords % Convert.ToInt32(formCollection["rows"]) == 0 ? totalRecords / Convert.ToInt32(formCollection["rows"]) : (totalRecords / Convert.ToInt32(formCollection["rows"]) + 1)),
                    page = Convert.ToInt32(formCollection["page"]),
                    records = totalRecords

                };
                return Json(jsonData, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                return Json(String.Empty, JsonRequestBehavior.AllowGet);
            }

        }
        #endregion


        #region IsScheduleDownload
        /// <summary>
        /// Getting the value of IsScheduleDownload Whether 'Y' or 'N'
        /// </summary>
        /// 
        /// <returns></returns>
        [HttpPost]
        [Audit]
        public ActionResult GetIsScheduleDownload()
        {

            qualityMonitoringHelpDeskBAL = new QualityMonitoringHelpDeskBAL();

            try
            {

                int getSelectedMonth = Convert.ToInt32(Request.Params["mnth"].Trim());
                string getSelectedYear = Request.Params["yr"].Trim();
                string getSelectedQmType = Request.Params["qm"].Trim();
                string rowID = Request.Params["selectRowId"].Trim();

                string isScehduleDownload = qualityMonitoringHelpDeskBAL.QMIsScheduleDownloadBAL(Convert.ToInt32(rowID), getSelectedYear, getSelectedMonth, getSelectedQmType);

                return Json(isScehduleDownload, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(string.Empty, JsonRequestBehavior.AllowGet);
            }

        }
        #endregion


        #region DefinalizeSchedule
        /// <summary>
        /// Definalizing the Schedule Details (updating some parameters in ScheduleDetails and Schedule if IsScheduleDownload is 'N')
        /// </summary>
        /// 
        /// <returns></returns>
        [HttpPost]

        public ActionResult UnlockScheduleData()
        {


            qualityMonitoringHelpDeskBAL = new QualityMonitoringHelpDeskBAL();

            try
            {

                int getSelectedMonth = Convert.ToInt32(Request.Params["mnth"].Trim());
                string getSelectedYear = Request.Params["yr"].Trim();
                string getSelectedQmType = Request.Params["qm"].Trim();
                string rowID = Request.Params["selectRowId"].Trim();

                int result = qualityMonitoringHelpDeskBAL.QMDefinalizeScheduleBAL(Convert.ToInt32(rowID), Convert.ToInt32(getSelectedYear), getSelectedMonth, getSelectedQmType);

                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(string.Empty, JsonRequestBehavior.AllowGet);
            }


        }
        #endregion

        #region Message Notification
        [HttpPost]
        public ActionResult ListQMMessageNotification(FormCollection formCollection)
        {
            qualityMonitoringHelpDeskBAL = new QualityMonitoringHelpDeskBAL();
            try
            {
                //Adde By Abhishek kamble 1-May-2014 start
                using (CommonFunctions commonFunction = new CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new GridParams(Convert.ToInt32(formCollection["page"]), Convert.ToInt32(formCollection["rows"]), formCollection["sidx"], formCollection["sord"], Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                    {
                        return null;
                    }
                }
                //Adde By Abhishek kamble 1-May-2014 end
                int totalRecords;
                string QmType = Request.Params["qm"].Trim();
                int StateCode = Request.Params["State"] == null ? 0 : Convert.ToInt32(Request.Params["State"].Trim().ToString());

                var jsonData = new
                {
                    rows = qualityMonitoringHelpDeskBAL.QMNotificationDetailsBAL(Convert.ToInt32(formCollection["page"]) - 1, Convert.ToInt32(formCollection["rows"]),formCollection["sidx"], formCollection["sord"], out totalRecords, QmType,StateCode),

                    total = totalRecords <= Convert.ToInt32(formCollection["rows"]) ? 1 : (totalRecords % Convert.ToInt32(formCollection["rows"]) == 0 ? totalRecords / Convert.ToInt32(formCollection["rows"]) : (totalRecords / Convert.ToInt32(formCollection["rows"]) + 1)),
                    page = Convert.ToInt32(formCollection["page"]),
                    records = totalRecords,

                };
                return Json(jsonData, JsonRequestBehavior.AllowGet);

            }
            catch (Exception)
            {
                return Json(String.Empty, JsonRequestBehavior.AllowGet);
            }

        }

        public ActionResult CreateQMMessageNotification(string id)
        {
            try
            {
                QualityMonitoringHelpDeskModel model = new QualityMonitoringHelpDeskModel();
                model.QM_Type = id;
               model.Monitor_LIST = PopulateMonitors("true", model.QM_Type, model.MAST_STATE_CODE);  
               // model.Monitor_LIST.Insert(0, new SelectListItem { Value = "0", Text = "Select Monitor",Selected=true });
                return PartialView("CreateQMMessageNotification", model);
            }
            catch (Exception)
            {

                return null;
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SaveQMMessageNotification(QualityMonitoringHelpDeskModel modelQMNotification)
        {
            bool status = false;
            qualityMonitoringHelpDeskBAL = new QualityMonitoringHelpDeskBAL();
            try
            {
                //if (modelQMNotification.QM_Type == "I")
                //{
                //    ModelState["MAST_STATE_CODE"].Errors.Clear();
                //}
                if (modelQMNotification.QM_Type == "S")
                {
                    ModelState["Monitor_CODE"].Errors.Clear();
                }

                if (ModelState.IsValid)
                {
                    if (qualityMonitoringHelpDeskBAL.SaveQMNotificationDetailsBAL(modelQMNotification, ref message))
                    {
                        message = message == string.Empty ? "Message Notification details saved successfully." : message;
                        status = true;
                    }
                    else
                    {

                        message = message == string.Empty ? "Message Notification details not saved." : message;
                    }

                }
                else
                {
                    message = "Message Notification details not saved.";
                    return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);


                }

                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                message = "Message Notification details not saved.";
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult EditQMMessageNotification(String id)
        {

            //Dictionary<string, string> decryptedParameters = null;
            String[] urlSplitParams = id.Split('$');
            Int32 userId = Convert.ToInt32(urlSplitParams[0]);
            Int32 messageId = Convert.ToInt32(urlSplitParams[1]);
            string qmType = urlSplitParams[2];
            qualityMonitoringHelpDeskBAL = new QualityMonitoringHelpDeskBAL();

            try
            {

                if (userId > 0 && messageId > 0)
                {
                    // IMasterDataEntryBAL masterDataEntryBAL = new MasterDataEntryBAL();

                    QualityMonitoringHelpDeskModel modelQM = qualityMonitoringHelpDeskBAL.GetQMNotificationDetailsBAL(userId, messageId, qmType);
                    CommonFunctions objCommonFunctions = new CommonFunctions();
                    if (modelQM == null)
                    {
                        ModelState.AddModelError(string.Empty, "Message Notification details not exist.");
                        return PartialView("CreateQMMessageNotification", new QualityMonitoringHelpDeskModel());
                    }
                    modelQM.QM_Type = qmType;
                    modelQM.Monitor_LIST =PopulateMonitors("true", modelQM.QM_Type, modelQM.MAST_STATE_CODE);   
                   
                    return PartialView("CreateQMMessageNotification", modelQM);
                }
                return PartialView("CreateQMMessageNotification", new QualityMonitoringHelpDeskModel());
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                ModelState.AddModelError(string.Empty, "Message Notification details not exist.");
                //return View();
                return PartialView("CreateQMMessageNotification", new QualityMonitoringHelpDeskModel());
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateQMMessageNotification(QualityMonitoringHelpDeskModel modelQMNotification)
        {
            bool status = false;
            qualityMonitoringHelpDeskBAL = new QualityMonitoringHelpDeskBAL();
            CommonFunctions objcommonFun=new CommonFunctions();
            try
            {
                //if (modelQMNotification.QM_Type == "I")
                //{
                //    ModelState["MAST_STATE_CODE"].Errors.Clear();
                //}
                if (ModelState.IsValid)
                {


                    if (qualityMonitoringHelpDeskBAL.UpdateQMNotificationDetailsBAL(modelQMNotification, ref message))
                    {
                        message = message == string.Empty ? "Message Notification details updated successfully." : message;
                        status = true;
                    }
                    else
                    {
                        message = message == string.Empty ? "Message Notification details not updated." : message;
                    }
                }
                else
                {
                   // return PartialView("CreateQMMessageNotification", modelQMNotification);
                    message = message == string.Empty ? objcommonFun.FormatErrorMessage( ModelState): message;
                }

                // return View(master_block);
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                message = message == string.Empty ? "Message Notification details not updated." : message;
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }


        }

        public ActionResult DeleteQMMessageNotification(String id)
        {
            bool status = false;
            try
            {
                String[] urlSplitParams = id.Split('$');
                Int32 userId = Convert.ToInt32(urlSplitParams[0]);
                Int32 messageId = Convert.ToInt32(urlSplitParams[1]);
                string qmType = urlSplitParams[2];
                qualityMonitoringHelpDeskBAL = new QualityMonitoringHelpDeskBAL();

                if (userId > 0 && messageId > 0)
                {
                    if (qualityMonitoringHelpDeskBAL.DeleteQMNotificationDetailsBAL(userId, messageId, ref message))
                    {
                        message = "Message Notification  details deleted successfully.";
                        status = true;
                    }
                    else
                    {
                        message = message == string.Empty ? "Message Notification  details not deleted." : message;
                    }

                    return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
                }

                message = "Message Notification  details not deleted.";
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                message = "Message Notification  details not deleted.";
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult PopulateMonitorsDetails(FormCollection frmCollection)
        {
            CommonFunctions objCommonFunctions = new CommonFunctions();
            string QmType = Request.Params["qm"].Trim();
            int stateCode = Convert.ToInt32(Request.Params["stateCode"].Trim());
            List<SelectListItem> list;
            list = PopulateMonitors("true", QmType, stateCode);
            // list.RemoveAt(0);
            // list.Find(x => x.Value == "-1").Value = "0";
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        #endregion


        #region BroadCast Notification

        [HttpPost]
        public ActionResult ListQMBroadCastMessageNotification(FormCollection formCollection)
        {
            qualityMonitoringHelpDeskBAL = new QualityMonitoringHelpDeskBAL();
            try
            {
                //Adde By Abhishek kamble 1-May-2014 start
                using (CommonFunctions commonFunction = new CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new GridParams(Convert.ToInt32(formCollection["page"]), Convert.ToInt32(formCollection["rows"]), formCollection["sidx"], formCollection["sord"], Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                    {
                        return null;
                    }
                }
                //Adde By Abhishek kamble 1-May-2014 end
                int totalRecords;
                string QmType = Request.Params["qm"].Trim();
                int StateCode = Request.Params["State"] == null ? 0 : Convert.ToInt32(Request.Params["State"].Trim().ToString());
               
                var jsonData = new
                {
                    rows = qualityMonitoringHelpDeskBAL.QMBroadCastNotificationDetailsBAL(Convert.ToInt32(formCollection["page"]) - 1, Convert.ToInt32(formCollection["rows"]),
                                                                        formCollection["sidx"], formCollection["sord"], out totalRecords, QmType,StateCode),

                    total = totalRecords <= Convert.ToInt32(formCollection["rows"]) ? 1 : (totalRecords % Convert.ToInt32(formCollection["rows"]) == 0 ? totalRecords / Convert.ToInt32(formCollection["rows"]) : (totalRecords / Convert.ToInt32(formCollection["rows"]) + 1)),
                    page = Convert.ToInt32(formCollection["page"]),
                    records = totalRecords,

                };
                return Json(jsonData, JsonRequestBehavior.AllowGet);

            }
            catch (Exception)
            {
                return Json(String.Empty, JsonRequestBehavior.AllowGet);
            }

        }

        public ActionResult CreateQMBroadCastMessageNotification(string id)
        {
            try
            {
                QualityMonitoringBroadCastNotificationModel model = new QualityMonitoringBroadCastNotificationModel();
                CommonFunctions objCommonFunctions = new CommonFunctions();
                model.QM_Type = id;
                return PartialView("CreateQMBroadCastMessageNotification", model);
            }
            catch (Exception)
            {

                return null;
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SaveQMBroadCastMessageNotification(QualityMonitoringBroadCastNotificationModel modelQMNotification)
        {
            bool status = false;
            qualityMonitoringHelpDeskBAL = new QualityMonitoringHelpDeskBAL();
            try
            {
                //if (modelQMNotification.QM_Type == "I")
                //{
                //    ModelState["MAST_STATE_CODE"].Errors.Clear();
                //}


                if (ModelState.IsValid)
                {
                    if (qualityMonitoringHelpDeskBAL.SaveQMBroadCastNotificationDetailsBAL(modelQMNotification, ref message))
                    {
                        message = message == string.Empty ? "Broadcast Message Notification details saved successfully." : message;
                        status = true;
                    }
                    else
                    {

                        message = message == string.Empty ? "Broadcast Message Notification details not saved." : message;
                    }

                }
                else
                {
                    message = "Broadcast Message Notification details not saved.";
                    return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);


                }

                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                message = "Broadcast Message Notification details not saved.";
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult EditQMBroadCastMessageNotification(String id)
        {

            //Dictionary<string, string> decryptedParameters = null;
            String[] urlSplitParams = id.Split('$');
            Int32 broadCastId = Convert.ToInt32(urlSplitParams[0]);
            string qmType = urlSplitParams[1];
            qualityMonitoringHelpDeskBAL = new QualityMonitoringHelpDeskBAL();

            try
            {

                if (broadCastId > 0)
                {
                    // IMasterDataEntryBAL masterDataEntryBAL = new MasterDataEntryBAL();

                    QualityMonitoringBroadCastNotificationModel modelQM = qualityMonitoringHelpDeskBAL.GetQMBroadCastNotificationDetailsBAL(broadCastId, qmType);
                    CommonFunctions objCommonFunctions = new CommonFunctions();
                    if (modelQM == null)
                    {
                        ModelState.AddModelError(string.Empty, "Broadcast Message Notification details not exist.");
                        return PartialView("CreateQMBroadCastMessageNotification", new QualityMonitoringBroadCastNotificationModel());
                    }
                    modelQM.QM_Type = qmType;
                    return PartialView("CreateQMBroadCastMessageNotification", modelQM);
                }
                return PartialView("CreateQMBroadCastMessageNotification", new QualityMonitoringBroadCastNotificationModel());
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                ModelState.AddModelError(string.Empty, "Broadcast Message Notification details not exist.");
                //return View();
                return PartialView("CreateQMBroadCastMessageNotification", new QualityMonitoringBroadCastNotificationModel());
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateQMBroadCastMessageNotification(QualityMonitoringBroadCastNotificationModel modelQMNotification)
        {
            bool status = false;
            qualityMonitoringHelpDeskBAL = new QualityMonitoringHelpDeskBAL();
            try
            {
                //if (modelQMNotification.QM_Type == "I")
                //{
                //    ModelState["MAST_STATE_CODE"].Errors.Clear();
                //}
                if (ModelState.IsValid)
                {


                    if (qualityMonitoringHelpDeskBAL.UpdateQMBroadCastNotificationDetailsBAL(modelQMNotification, ref message))
                    {
                        message = message == string.Empty ? "Broadcast Message Notification details updated successfully." : message;
                        status = true;
                    }
                    else
                    {
                        message = message == string.Empty ? "Broadcast Message Notification details not updated." : message;
                    }
                }
                else
                {
                    return PartialView("CreateQMBroadCastMessageNotification", modelQMNotification);
                }

                // return View(master_block);
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                message = message == string.Empty ? "Broadcast Message Notification details not updated." : message;
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }


        }

        public ActionResult DeleteQMBroadCastMessageNotification(String id)
        {
            bool status = false;
            try
            {
                String[] urlSplitParams = id.Split('$');
                Int32 broadCastId = Convert.ToInt32(urlSplitParams[0]);
                string qmType = urlSplitParams[1];
                qualityMonitoringHelpDeskBAL = new QualityMonitoringHelpDeskBAL();

                if (broadCastId > 0)
                {
                    if (qualityMonitoringHelpDeskBAL.DeleteQMBroadCastNotificationDetailsBAL(broadCastId, ref message))
                    {
                        message = "Broadcast Message Notification  details deleted successfully.";
                        status = true;
                    }
                    else
                    {
                        message = message == string.Empty ? "Broadcast Message Notification  details not deleted." : message;
                    }

                    return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
                }

                message = "Broadcast Message Notification  details not deleted.";
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                message = "Broadcast Message Notification  details not deleted.";
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
        }


        #endregion


        #region added by Hrishikesh  "reset IMEI and REsetIMEI Count" Functionality to CQCAdmin Role
        //added by Hrishikesh to Provide "reset IMEI and REsetIMEI Count" Functionality to CQCAdmin Role
        //[HttpPost]
        //[Audit]
        public ActionResult ResetIMEINo()
        {
            /*var roleCode = PMGSYSession.Current.RoleCode;
            ViewBag.roleCode = roleCode;*/
            return View();
        }
        //added by Hrishikesh to Provide "reset IMEI and REsetIMEI Count" Functionality to CQCAdmin -(To fetch the reset imei detail)
        [HttpPost]
        [Audit]
        public ActionResult GetIMEIResetDetails(FormCollection formCollection)  //user id will get from formcollection
        {
            qualityMonitoringHelpDeskBAL = new QualityMonitoringHelpDeskBAL(); //initiatize obj
            try
            {
                using (CommonFunctions commonFunction = new CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new GridParams(Convert.ToInt32(formCollection["page"]), Convert.ToInt32(formCollection["rows"]), formCollection["sidx"], formCollection["sord"], Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                    {
                        return null;
                    }
                }
                //var a = Request.Params["selectedUserID"];
                //string username = Request.Params["userName"];
                int userId = Convert.ToInt32(Request.Params["selectedUserID"].Trim());
                int totalRecords;
                var jsonData = new
                {
                    rows = qualityMonitoringHelpDeskBAL.GetIMEIResetDetailsBAL(userId, out totalRecords, Convert.ToInt32(formCollection["page"]) - 1, Convert.ToInt32(formCollection["rows"]), formCollection["sidx"], formCollection["sord"], formCollection["filters"]),
                    total = totalRecords <= Convert.ToInt32(formCollection["rows"]) ? 1 : (totalRecords % Convert.ToInt32(formCollection["rows"]) == 0 ? totalRecords / Convert.ToInt32(formCollection["rows"]) : (totalRecords / Convert.ToInt32(formCollection["rows"]) + 1)),
                    page = Convert.ToInt32(formCollection["page"]),
                    records = totalRecords,

                };

                //return Json(jsonData, JsonRequestBehavior.AllowGet);

                var jsonResult = Json(jsonData, JsonRequestBehavior.AllowGet);
                jsonResult.MaxJsonLength = int.MaxValue;
                return jsonResult;

            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "GetIMEIResetDetails()");
                return Json(string.Empty, JsonRequestBehavior.AllowGet);
            }
        }//end GetIMEIResetDetails()

        #endregion


        #region Reset IMEI No Details
        [HttpPost]
        [Audit]
        public ActionResult ListUsersIMEINoDetail(FormCollection formCollection)
        {
            qualityMonitoringHelpDeskBAL = new QualityMonitoringHelpDeskBAL();
            try
            {

                using (CommonFunctions commonFunction = new CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new GridParams(Convert.ToInt32(formCollection["page"]), Convert.ToInt32(formCollection["rows"]), formCollection["sidx"], formCollection["sord"], Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                    {
                        return null;
                    }
                }

                int totalRecords;
                var jsonData = new
                {
                    rows = qualityMonitoringHelpDeskBAL.QMResetIMEINoDetailsBAL(Convert.ToInt32(formCollection["page"]) - 1, Convert.ToInt32(formCollection["rows"]),
                                                                        formCollection["sidx"], formCollection["sord"], out totalRecords, formCollection["filters"]),
                    total = totalRecords <= Convert.ToInt32(formCollection["rows"]) ? 1 : (totalRecords % Convert.ToInt32(formCollection["rows"]) == 0 ? totalRecords / Convert.ToInt32(formCollection["rows"]) : (totalRecords / Convert.ToInt32(formCollection["rows"]) + 1)),
                    page = Convert.ToInt32(formCollection["page"]),
                    records = totalRecords,

                };
                    //return Json(jsonData, JsonRequestBehavior.AllowGet);

                var jsonResult= Json(jsonData, JsonRequestBehavior.AllowGet);
                jsonResult.MaxJsonLength = int.MaxValue;
                return jsonResult;


            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "ListUsersIMEINoDetail()");
                return Json(String.Empty, JsonRequestBehavior.AllowGet);
            }

        }
        [HttpGet]
        public ActionResult UpdateIMEIApplicationMode(String id)
        {
            bool status = false;
            String[] urlSplitParams = id.Split('$');
            qualityMonitoringHelpDeskBAL = new QualityMonitoringHelpDeskBAL();
            QM_HELPDESK_IMEI_MODEL_DETAILS QMIMEIModelDetails = new QM_HELPDESK_IMEI_MODEL_DETAILS();
            try
            {                
                if (ModelState.IsValid)
                {
                    QMIMEIModelDetails.UserId = Convert.ToInt32(urlSplitParams[0]);
                    //QMIMEIModelDetails.ImeiNo = urlSplitParams[1];
                    QMIMEIModelDetails.ApplicationMode = urlSplitParams[2];


                    if (qualityMonitoringHelpDeskBAL.UpdateIMEIApplicationModeDetailsBAL(QMIMEIModelDetails, ref message))
                    {
                        message = message == string.Empty ? "IMEI Application Mode details updated successfully." : message;
                        status = true;
                    }
                    else
                    {
                        message = message == string.Empty ? "IMEI Application Mode details not updated." : message;
                    }
                }


                // return View(master_block);
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                message = message == string.Empty ? "IMEI Application Mode details  not updated." : message;
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }


        }

        [HttpGet]
        public ActionResult UpdateIMEINoResetDetails(String id)
        {
            bool status = false;
           // String[] urlSplitParams = id.Split('$');
            qualityMonitoringHelpDeskBAL = new QualityMonitoringHelpDeskBAL();
            QM_HELPDESK_IMEI_MODEL_DETAILS QMIMEIModelDetails = new QM_HELPDESK_IMEI_MODEL_DETAILS();
            try
            {
               
                if (ModelState.IsValid)
                {
                    QMIMEIModelDetails.UserId = Convert.ToInt32(id);
                  

                    if (qualityMonitoringHelpDeskBAL.UpdateIMEINoResetDetailsBAL(QMIMEIModelDetails, ref message))
                    {
                        message = message == string.Empty ? "Reset IMEI details  successfully." : message;
                        status = true;
                    }
                    else
                    {
                        message = message == string.Empty ? "Reset IMEI details not updated." : message;
                    }
                }


                // return View(master_block);
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                message = message == string.Empty ? "Reset IMEI details not updated." : message;
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }


        }
        #endregion


        #region Common Methods
        public List<SelectListItem> PopulateMonitors(string isPopulateFirstSelect, string qmType, int stateCode)
        {
            List<SelectListItem> lstProfileNames = new List<SelectListItem>();
            PMGSYEntities dbContext = new PMGSYEntities();
            try
            {
                SelectListItem item = new SelectListItem();
                if (isPopulateFirstSelect.Equals("true"))
                {
                    item.Text = "Select Monitor";
                    item.Value = "0";
                    item.Selected = true;
                    lstProfileNames.Add(item);
                }
                else if (isPopulateFirstSelect.Equals("false"))
                {
                    item.Text = "All Monitors";
                    item.Value = "0";
                    item.Selected = true;
                    lstProfileNames.Add(item);
                }

                if (qmType.Equals("0"))
                {
                    return lstProfileNames;
                }

              
              
                var query = (from c in dbContext.ADMIN_QUALITY_MONITORS
                                 where c.ADMIN_QM_TYPE == qmType
                                 && c.ADMIN_QM_EMPANELLED == "Y" &&
                                 c.ADMIN_USER_ID != null && 
                                 ((stateCode==0?1:c.MAST_STATE_CODE) == (stateCode==0?1:stateCode))
                                 select new
                                 {
                                     Value = c.ADMIN_QM_CODE,
                                     Text = (c.ADMIN_QM_LNAME.Equals(null) ? "" : c.ADMIN_QM_LNAME.Trim() + " ") + (c.ADMIN_QM_FNAME.Equals(null) ? "" : c.ADMIN_QM_FNAME.Trim() + " ") + (c.ADMIN_QM_MNAME.Equals(null) ? "" : c.ADMIN_QM_MNAME.Trim())
                                 }).OrderBy(c => c.Text.Trim()).Distinct().ToList();

                query = query.OrderBy(c => c.Text).ToList();
                    foreach (var data in query)
                    {
                        item = new SelectListItem();
                        item.Text = data.Text;
                        item.Value = data.Value.ToString();
                        lstProfileNames.Add(item);
                    }            



                return lstProfileNames;
            }
            catch (DbEntityValidationException dbEx)
            {
                foreach (var validationErrors in dbEx.EntityValidationErrors)
                {
                    foreach (var validationError in validationErrors.ValidationErrors)
                    {
                        Trace.TraceInformation("Property: {0} Error: {1}", validationError.PropertyName, validationError.ErrorMessage);
                    }
                }
                return null;
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
        #endregion


        #region ATR Deletion

        public ActionResult ChangeATRStatus()
        {
            return View();
        }

        /// <summary>
        /// Update Status of ATR 
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult UpdateATRStatus()
        {
            try
            {
                qualityMonitoringHelpDeskBAL = new QualityMonitoringHelpDeskBAL();
                
                string Status = qualityMonitoringHelpDeskBAL.UpdateATRStatusBAL(Convert.ToInt32(Request.Params["obsId"]));
                if (Status == string.Empty)
                    return Json(new { Success = true });
                else
                    return Json(new { Success = false, ErrorMessage = Status });
                
            }
            catch (Exception ex)
            {
                return Json(new { Success = false, ErrorMessage = "An Error Occurred While Processing Your Request." });
            }
        }

        // To update Maintenance ATR status added by deendayal
        [HttpPost]
        public ActionResult UpdateMaintnenanceATRStatus()
        {
            try
            {
                qualityMonitoringHelpDeskBAL = new QualityMonitoringHelpDeskBAL();

                string Status = qualityMonitoringHelpDeskBAL.UpdateATRStatusBAL(Convert.ToInt32(Request.Params["obsId"]));
                if (Status == string.Empty)
                    return Json(new { Success = true });
                else
                    return Json(new { Success = false, ErrorMessage = Status });

            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "QualityMonitoringHelpDeskController.UpdateMaintnenanceATRStatus()");
                return Json(new { Success = false, ErrorMessage = "An Error Occurred While Processing Your Request." });
            }
        }
        #endregion

        #region 2 tier atr vikky
        [HttpPost]
        public ActionResult Update2TierATRStatus()
        {
            try
            {
                qualityMonitoringHelpDeskBAL = new QualityMonitoringHelpDeskBAL();

                string Status = qualityMonitoringHelpDeskBAL.Update2TierATRStatusBAL(Convert.ToInt32(Request.Params["obsId"]));
                if (Status == string.Empty)
                    return Json(new { Success = true });
                else
                    return Json(new { Success = false, ErrorMessage = Status });

            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "QualitymonitoringControllerHelpdesk.Update2TierATRStatus()");
                return Json(new { Success = false, ErrorMessage = "An Error Occurred While Processing Your Request." });
            }
        }

        #endregion

    }

}
